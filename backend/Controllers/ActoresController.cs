using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/actores")]
public class ActoresController : ControllerBase
{
    private readonly AppDbContext _db;

    public ActoresController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Actor>>> GetAll([FromQuery] string? q)
    {
        var query = _db.Actores.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            var normalized = TextNormalize.NormalizeKey(term);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                query = query.Where(a => a.NombreNormalizado == normalized ||
                                          (a.Nombre != null && EF.Functions.Like(a.Nombre, $"%{term}%")) ||
                                          (a.Alias != null && EF.Functions.Like(a.Alias, $"%{term}%")));
            }
        }

        var items = await query.OrderBy(a => a.Nombre).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Actor>> GetById(Guid id)
    {
        var actor = await _db.Actores.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (actor == null)
        {
            return NotFound();
        }

        return Ok(actor);
    }

    [HttpPost]
    public async Task<ActionResult<Actor>> Create(ActorCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest("Nombre is required.");
        }

        var name = dto.Nombre.Trim();
        var normalized = TextNormalize.NormalizeKey(name);
        var existing = await _db.Actores.FirstOrDefaultAsync(a => a.NombreNormalizado == normalized);
        if (existing != null)
        {
            return Ok(existing);
        }

        var actor = new Actor
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
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = actor.Id }, actor);
    }
}
