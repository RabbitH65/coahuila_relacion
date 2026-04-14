using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/circunstancias")]
public class CircunstanciasController : ControllerBase
{
    private readonly AppDbContext _db;

    public CircunstanciasController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Circunstancia>>> GetAll([FromQuery] string? q)
    {
        var query = _db.Circunstancias.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            var normalized = TextNormalize.NormalizeKey(term);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                query = query.Where(c => c.NombreNormalizado == normalized ||
                                          (c.Nombre != null && EF.Functions.Like(c.Nombre, $"%{term}%")));
            }
        }

        var items = await query.OrderBy(c => c.Nombre).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Circunstancia>> GetById(Guid id)
    {
        var circunstancia = await _db.Circunstancias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (circunstancia == null)
        {
            return NotFound();
        }

        return Ok(circunstancia);
    }

    [HttpPost]
    public async Task<ActionResult<Circunstancia>> Create(CircunstanciaCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest("Nombre is required.");
        }

        var name = dto.Nombre.Trim();
        var normalized = TextNormalize.NormalizeKey(name);
        var existing = await _db.Circunstancias.FirstOrDefaultAsync(c => c.NombreNormalizado == normalized);
        if (existing != null)
        {
            return Ok(existing);
        }

        var circunstancia = new Circunstancia
        {
            Id = Guid.NewGuid(),
            Nombre = name,
            NombreNormalizado = normalized,
            Descripcion = dto.Descripcion?.Trim(),
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
        };

        _db.Circunstancias.Add(circunstancia);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = circunstancia.Id }, circunstancia);
    }
}
