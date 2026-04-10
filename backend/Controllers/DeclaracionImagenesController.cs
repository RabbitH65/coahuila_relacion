using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/declaraciones/{declaracionId:guid}/imagenes")]
public class DeclaracionImagenesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly FileStorageService _storage;

    public DeclaracionImagenesController(AppDbContext db, FileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeclaracionImagen>>> List(Guid declaracionId)
    {
        var exists = await _db.Declaraciones.AsNoTracking().AnyAsync(d => d.Id == declaracionId);
        if (!exists)
        {
            return NotFound();
        }

        var imagenes = await _db.DeclaracionImagenes.AsNoTracking()
            .Where(i => i.DeclaracionId == declaracionId)
            .OrderBy(i => i.Orden)
            .ThenBy(i => i.FechaCarga)
            .ToListAsync();

        return Ok(imagenes);
    }

    [HttpPost]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<DeclaracionImagen>> Upload(
        Guid declaracionId,
        [FromForm] DeclaracionImagenUploadDto dto,
        CancellationToken ct)
    {
        var declaracion = await _db.Declaraciones.FirstOrDefaultAsync(d => d.Id == declaracionId, ct);
        if (declaracion == null)
        {
            return NotFound();
        }

        if (dto.Archivo == null || dto.Archivo.Length == 0)
        {
            return BadRequest("Archivo is required.");
        }

        (string relativePath, string fileName) result;
        try
        {
            result = await _storage.SaveImageAsync(dto.Archivo, ct);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        var imagen = new DeclaracionImagen
        {
            Id = Guid.NewGuid(),
            DeclaracionId = declaracionId,
            RutaArchivo = result.relativePath,
            Descripcion = dto.Descripcion?.Trim(),
            ElementoRelevante = dto.ElementoRelevante?.Trim(),
            TipoImagen = dto.TipoImagen?.Trim(),
            Orden = dto.Orden ?? 0,
            FechaCarga = DateTime.UtcNow,
            FechaCaptura = dto.FechaCaptura,
        };

        _db.DeclaracionImagenes.Add(imagen);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(List), new { declaracionId }, imagen);
    }
}
