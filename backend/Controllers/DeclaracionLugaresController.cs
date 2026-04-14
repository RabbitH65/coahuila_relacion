using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/declaraciones/{declaracionId:guid}/lugares")]
public class DeclaracionLugaresController : ControllerBase
{
    private readonly AppDbContext _db;

    public DeclaracionLugaresController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeclaracionLugar>>> List(Guid declaracionId)
    {
        var items = await _db.DeclaracionLugares
            .AsNoTracking()
            .Include(l => l.Lugar)
            .Where(l => l.DeclaracionId == declaracionId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<DeclaracionLugar>> Add(Guid declaracionId, DeclaracionLugarLinkDto dto)
    {
        var declaracion = await _db.Declaraciones.FirstOrDefaultAsync(d => d.Id == declaracionId);
        if (declaracion == null)
        {
            return NotFound();
        }

        Lugar? lugar = null;
        if (dto.LugarId.HasValue)
        {
            lugar = await _db.Lugares.FirstOrDefaultAsync(l => l.Id == dto.LugarId.Value);
            if (lugar == null)
            {
                return BadRequest("LugarId not found.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(dto.Nombre))
        {
            var name = dto.Nombre.Trim();
            var normalized = TextNormalize.NormalizeKey(name);
            lugar = await _db.Lugares.FirstOrDefaultAsync(l => l.NombreNormalizado == normalized);
            if (lugar == null)
            {
                lugar = new Lugar
                {
                    Id = Guid.NewGuid(),
                    Nombre = name,
                    NombreNormalizado = normalized,
                    Tipo = dto.Tipo?.Trim(),
                    Estado = dto.Estado?.Trim(),
                    Municipio = dto.Municipio?.Trim(),
                    Notas = dto.Notas?.Trim(),
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                };
                _db.Lugares.Add(lugar);
            }
        }
        else
        {
            return BadRequest("Nombre or LugarId is required.");
        }

        var exists = await _db.DeclaracionLugares
            .AnyAsync(l => l.DeclaracionId == declaracionId && l.LugarId == lugar!.Id);
        if (exists)
        {
            var existing = await _db.DeclaracionLugares
                .Include(l => l.Lugar)
                .FirstAsync(l => l.DeclaracionId == declaracionId && l.LugarId == lugar!.Id);
            return Ok(existing);
        }

        var link = new DeclaracionLugar
        {
            Id = Guid.NewGuid(),
            DeclaracionId = declaracionId,
            LugarId = lugar!.Id,
            Origen = string.IsNullOrWhiteSpace(dto.Origen) ? "manual" : dto.Origen.Trim(),
            Confianza = dto.Confianza,
            Confirmado = dto.Confirmado ?? true,
        };

        _db.DeclaracionLugares.Add(link);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(List), new { declaracionId }, link);
    }

    [HttpDelete("{lugarId:guid}")]
    public async Task<IActionResult> Remove(Guid declaracionId, Guid lugarId)
    {
        var link = await _db.DeclaracionLugares
            .FirstOrDefaultAsync(l => l.DeclaracionId == declaracionId && l.LugarId == lugarId);
        if (link == null)
        {
            return NotFound();
        }

        _db.DeclaracionLugares.Remove(link);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
