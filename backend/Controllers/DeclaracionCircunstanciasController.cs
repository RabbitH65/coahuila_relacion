using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/declaraciones/{declaracionId:guid}/circunstancias")]
public class DeclaracionCircunstanciasController : ControllerBase
{
    private readonly AppDbContext _db;

    public DeclaracionCircunstanciasController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeclaracionCircunstancia>>> List(Guid declaracionId)
    {
        var items = await _db.DeclaracionCircunstancias
            .AsNoTracking()
            .Include(c => c.Circunstancia)
            .Where(c => c.DeclaracionId == declaracionId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<DeclaracionCircunstancia>> Add(Guid declaracionId, DeclaracionCircunstanciaLinkDto dto)
    {
        var declaracion = await _db.Declaraciones.FirstOrDefaultAsync(d => d.Id == declaracionId);
        if (declaracion == null)
        {
            return NotFound();
        }

        Circunstancia? circunstancia = null;
        if (dto.CircunstanciaId.HasValue)
        {
            circunstancia = await _db.Circunstancias.FirstOrDefaultAsync(c => c.Id == dto.CircunstanciaId.Value);
            if (circunstancia == null)
            {
                return BadRequest("CircunstanciaId not found.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(dto.Nombre))
        {
            var name = dto.Nombre.Trim();
            var normalized = TextNormalize.NormalizeKey(name);
            circunstancia = await _db.Circunstancias.FirstOrDefaultAsync(c => c.NombreNormalizado == normalized);
            if (circunstancia == null)
            {
                circunstancia = new Circunstancia
                {
                    Id = Guid.NewGuid(),
                    Nombre = name,
                    NombreNormalizado = normalized,
                    Descripcion = dto.Descripcion?.Trim(),
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                };
                _db.Circunstancias.Add(circunstancia);
            }
        }
        else
        {
            return BadRequest("Nombre or CircunstanciaId is required.");
        }

        var exists = await _db.DeclaracionCircunstancias
            .AnyAsync(c => c.DeclaracionId == declaracionId && c.CircunstanciaId == circunstancia!.Id);
        if (exists)
        {
            var existing = await _db.DeclaracionCircunstancias
                .Include(c => c.Circunstancia)
                .FirstAsync(c => c.DeclaracionId == declaracionId && c.CircunstanciaId == circunstancia!.Id);
            return Ok(existing);
        }

        var link = new DeclaracionCircunstancia
        {
            Id = Guid.NewGuid(),
            DeclaracionId = declaracionId,
            CircunstanciaId = circunstancia!.Id,
            Origen = string.IsNullOrWhiteSpace(dto.Origen) ? "manual" : dto.Origen.Trim(),
            Confianza = dto.Confianza,
            Confirmado = dto.Confirmado ?? true,
        };

        _db.DeclaracionCircunstancias.Add(link);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(List), new { declaracionId }, link);
    }

    [HttpDelete("{circunstanciaId:guid}")]
    public async Task<IActionResult> Remove(Guid declaracionId, Guid circunstanciaId)
    {
        var link = await _db.DeclaracionCircunstancias
            .FirstOrDefaultAsync(c => c.DeclaracionId == declaracionId && c.CircunstanciaId == circunstanciaId);
        if (link == null)
        {
            return NotFound();
        }

        _db.DeclaracionCircunstancias.Remove(link);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
