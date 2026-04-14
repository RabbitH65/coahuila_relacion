using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/registros")]
public class RegistrosController : ControllerBase
{
    private readonly AppDbContext _db;

    public RegistrosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistroListItemDto>>> GetAll([FromQuery] string? q)
    {
        var query = _db.Registros.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            var pattern = $"%{term}%";
            query = query.Where(r =>
                (r.Numero != null && EF.Functions.Like(r.Numero, pattern)) ||
                (r.Ap != null && EF.Functions.Like(r.Ap, pattern)) ||
                (r.Tomo != null && EF.Functions.Like(r.Tomo, pattern)) ||
                (r.Foja != null && EF.Functions.Like(r.Foja, pattern)) ||
                (r.Entrevistado != null && EF.Functions.Like(r.Entrevistado, pattern)) ||
                (r.Declarante != null && EF.Functions.Like(r.Declarante, pattern)));
        }

        var items = await query
            .OrderByDescending(r => r.FechaCreacion)
            .Select(r => new RegistroListItemDto
            {
                Id = r.Id,
                Numero = r.Numero,
                Ap = r.Ap,
                Tomo = r.Tomo,
                Foja = r.Foja,
                Fecha = r.Fecha,
                Entrevistado = r.Entrevistado,
                Declarante = r.Declarante,
                DeclaracionesCount = r.Declaraciones.Count,
                Activo = r.Activo,
                FechaCreacion = r.FechaCreacion
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Registro>> GetById(Guid id, [FromQuery] bool includeDeclaraciones = true)
    {
        var query = _db.Registros.AsNoTracking().AsQueryable();

        if (includeDeclaraciones)
        {
            query = query
                .Include(r => r.Declaraciones)
                    .ThenInclude(d => d.Coordenadas)
                .Include(r => r.Declaraciones)
                    .ThenInclude(d => d.Fechas)
                .Include(r => r.Declaraciones)
                    .ThenInclude(d => d.Imagenes)
                .Include(r => r.Declaraciones)
                    .ThenInclude(d => d.Actores)
                        .ThenInclude(a => a.Actor)
                .Include(r => r.Declaraciones)
                    .ThenInclude(d => d.Lugares)
                        .ThenInclude(l => l.Lugar)
                .Include(r => r.Declaraciones)
                    .ThenInclude(d => d.Circunstancias)
                        .ThenInclude(c => c.Circunstancia);
        }

        var registro = await query.FirstOrDefaultAsync(r => r.Id == id);
        if (registro == null)
        {
            return NotFound();
        }

        return Ok(registro);
    }

    [HttpPost]
    public async Task<ActionResult<Registro>> Create(RegistroCreateDto dto)
    {
        var now = DateTime.UtcNow;
        var registro = new Registro
        {
            Id = Guid.NewGuid(),
            Numero = dto.Numero?.Trim(),
            Ap = dto.Ap?.Trim(),
            Tomo = dto.Tomo?.Trim(),
            Foja = dto.Foja?.Trim(),
            Fecha = dto.Fecha,
            Entrevistado = dto.Entrevistado?.Trim(),
            Declarante = dto.Declarante?.Trim(),
            Observaciones = dto.Observaciones?.Trim(),
            Activo = true,
            FechaCreacion = now,
            FechaActualizacion = now,
        };

        foreach (var declaracionDto in dto.Declaraciones)
        {
            var declaracion = new Declaracion
            {
                Id = Guid.NewGuid(),
                RegistroId = registro.Id,
                TextoDeclaracion = declaracionDto.TextoDeclaracion?.Trim(),
                Orden = declaracionDto.Orden,
                Tipo = declaracionDto.Tipo?.Trim(),
                FechaCaptura = declaracionDto.FechaCaptura,
                Observaciones = declaracionDto.Observaciones?.Trim(),
            };

            declaracion.Coordenadas = declaracionDto.Coordenadas.Select(coord => new DeclaracionCoordenada
            {
                Id = Guid.NewGuid(),
                DeclaracionId = declaracion.Id,
                Latitud = coord.Latitud,
                Longitud = coord.Longitud,
                PrecisionM = coord.PrecisionM,
                Fuente = coord.Fuente?.Trim(),
                Observacion = coord.Observacion?.Trim(),
            }).ToList();

            declaracion.Fechas = declaracionDto.Fechas.Select(fecha => new DeclaracionFecha
            {
                Id = Guid.NewGuid(),
                DeclaracionId = declaracion.Id,
                Fecha = fecha.Fecha,
                TipoFecha = fecha.TipoFecha?.Trim(),
                Observacion = fecha.Observacion?.Trim(),
            }).ToList();

            registro.Declaraciones.Add(declaracion);
        }

        _db.Registros.Add(registro);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = registro.Id }, registro);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Registro>> Update(Guid id, RegistroUpdateDto dto)
    {
        var registro = await _db.Registros
            .Include(r => r.Declaraciones)
                .ThenInclude(d => d.Coordenadas)
            .Include(r => r.Declaraciones)
                .ThenInclude(d => d.Fechas)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (registro == null)
        {
            return NotFound();
        }

        if (dto.Numero != null) registro.Numero = dto.Numero.Trim();
        if (dto.Ap != null) registro.Ap = dto.Ap.Trim();
        if (dto.Tomo != null) registro.Tomo = dto.Tomo.Trim();
        if (dto.Foja != null) registro.Foja = dto.Foja.Trim();
        if (dto.Fecha.HasValue) registro.Fecha = dto.Fecha;
        if (dto.Entrevistado != null) registro.Entrevistado = dto.Entrevistado.Trim();
        if (dto.Declarante != null) registro.Declarante = dto.Declarante.Trim();
        if (dto.Observaciones != null) registro.Observaciones = dto.Observaciones.Trim();
        if (dto.Activo.HasValue) registro.Activo = dto.Activo.Value;

        if (dto.Declaraciones != null)
        {
            var incomingIds = dto.Declaraciones
                .Where(d => d.Id.HasValue)
                .Select(d => d.Id!.Value)
                .ToHashSet();

            var toRemove = registro.Declaraciones
                .Where(d => !incomingIds.Contains(d.Id))
                .ToList();

            if (toRemove.Count > 0)
            {
                _db.Declaraciones.RemoveRange(toRemove);
            }

            foreach (var declaracionDto in dto.Declaraciones)
            {
                Declaracion? declaracion = null;
                if (declaracionDto.Id.HasValue)
                {
                    declaracion = registro.Declaraciones.FirstOrDefault(d => d.Id == declaracionDto.Id.Value);
                }

                if (declaracion == null)
                {
                    declaracion = new Declaracion
                    {
                        Id = Guid.NewGuid(),
                        RegistroId = registro.Id,
                    };
                    registro.Declaraciones.Add(declaracion);
                }

                declaracion.TextoDeclaracion = declaracionDto.TextoDeclaracion?.Trim();
                declaracion.Orden = declaracionDto.Orden;
                declaracion.Tipo = declaracionDto.Tipo?.Trim();
                declaracion.FechaCaptura = declaracionDto.FechaCaptura;
                declaracion.Observaciones = declaracionDto.Observaciones?.Trim();

                _db.DeclaracionCoordenadas.RemoveRange(declaracion.Coordenadas);
                declaracion.Coordenadas = declaracionDto.Coordenadas.Select(coord => new DeclaracionCoordenada
                {
                    Id = Guid.NewGuid(),
                    DeclaracionId = declaracion.Id,
                    Latitud = coord.Latitud,
                    Longitud = coord.Longitud,
                    PrecisionM = coord.PrecisionM,
                    Fuente = coord.Fuente?.Trim(),
                    Observacion = coord.Observacion?.Trim(),
                }).ToList();

                _db.DeclaracionFechas.RemoveRange(declaracion.Fechas);
                declaracion.Fechas = declaracionDto.Fechas.Select(fecha => new DeclaracionFecha
                {
                    Id = Guid.NewGuid(),
                    DeclaracionId = declaracion.Id,
                    Fecha = fecha.Fecha,
                    TipoFecha = fecha.TipoFecha?.Trim(),
                    Observacion = fecha.Observacion?.Trim(),
                }).ToList();
            }
        }

        registro.FechaActualizacion = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(registro);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var registro = await _db.Registros.FirstOrDefaultAsync(r => r.Id == id);
        if (registro == null)
        {
            return NotFound();
        }

        _db.Registros.Remove(registro);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
