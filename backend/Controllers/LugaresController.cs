using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/lugares")]
public class LugaresController : ControllerBase
{
    private readonly AppDbContext _db;

    public LugaresController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lugar>>> GetAll([FromQuery] string? q)
    {
        var query = _db.Lugares.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            var normalized = TextNormalize.NormalizeKey(term);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                query = query.Where(l => l.NombreNormalizado == normalized ||
                                          (l.Nombre != null && EF.Functions.Like(l.Nombre, $"%{term}%")) ||
                                          (l.Municipio != null && EF.Functions.Like(l.Municipio, $"%{term}%")) ||
                                          (l.Estado != null && EF.Functions.Like(l.Estado, $"%{term}%")));
            }
        }

        var items = await query.OrderBy(l => l.Nombre).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Lugar>> GetById(Guid id)
    {
        var lugar = await _db.Lugares.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
        if (lugar == null)
        {
            return NotFound();
        }

        return Ok(lugar);
    }

    [HttpPost]
    public async Task<ActionResult<Lugar>> Create(LugarCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest("Nombre is required.");
        }

        var name = dto.Nombre.Trim();
        var normalized = TextNormalize.NormalizeKey(name);
        var existing = await _db.Lugares.FirstOrDefaultAsync(l => l.NombreNormalizado == normalized);
        if (existing != null)
        {
            return Ok(existing);
        }

        var lugar = new Lugar
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
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = lugar.Id }, lugar);
    }
}
