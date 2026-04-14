using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/declaraciones/{declaracionId:guid}/actores")]
public class DeclaracionActoresController : ControllerBase
{
    private readonly AppDbContext _db;

    public DeclaracionActoresController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeclaracionActor>>> List(Guid declaracionId)
    {
        var items = await _db.DeclaracionActores
            .AsNoTracking()
            .Include(a => a.Actor)
            .Where(a => a.DeclaracionId == declaracionId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<DeclaracionActor>> Add(Guid declaracionId, DeclaracionActorLinkDto dto)
    {
        var declaracion = await _db.Declaraciones.FirstOrDefaultAsync(d => d.Id == declaracionId);
        if (declaracion == null)
        {
            return NotFound();
        }

        Actor? actor = null;
        if (dto.ActorId.HasValue)
        {
            actor = await _db.Actores.FirstOrDefaultAsync(a => a.Id == dto.ActorId.Value);
            if (actor == null)
            {
                return BadRequest("ActorId not found.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(dto.Nombre))
        {
            var name = dto.Nombre.Trim();
            var normalized = TextNormalize.NormalizeKey(name);
            actor = await _db.Actores.FirstOrDefaultAsync(a => a.NombreNormalizado == normalized);
            if (actor == null)
            {
                actor = new Actor
                {
                    Id = Guid.NewGuid(),
                    Nombre = name,
                    NombreNormalizado = normalized,
                    Alias = dto.Alias?.Trim(),
                    Notas = dto.Notas?.Trim(),
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                };
                _db.Actores.Add(actor);
            }
        }
        else
        {
            return BadRequest("Nombre or ActorId is required.");
        }

        var exists = await _db.DeclaracionActores
            .AnyAsync(a => a.DeclaracionId == declaracionId && a.ActorId == actor!.Id);
        if (exists)
        {
            var existing = await _db.DeclaracionActores
                .Include(a => a.Actor)
                .FirstAsync(a => a.DeclaracionId == declaracionId && a.ActorId == actor!.Id);
            return Ok(existing);
        }

        var link = new DeclaracionActor
        {
            Id = Guid.NewGuid(),
            DeclaracionId = declaracionId,
            ActorId = actor!.Id,
            Origen = string.IsNullOrWhiteSpace(dto.Origen) ? "manual" : dto.Origen.Trim(),
            Confianza = dto.Confianza,
            Confirmado = dto.Confirmado ?? true,
        };

        _db.DeclaracionActores.Add(link);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(List), new { declaracionId }, link);
    }

    [HttpDelete("{actorId:guid}")]
    public async Task<IActionResult> Remove(Guid declaracionId, Guid actorId)
    {
        var link = await _db.DeclaracionActores
            .FirstOrDefaultAsync(a => a.DeclaracionId == declaracionId && a.ActorId == actorId);
        if (link == null)
        {
            return NotFound();
        }

        _db.DeclaracionActores.Remove(link);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
