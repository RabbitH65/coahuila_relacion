using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/mapa")]
public class MapaController : ControllerBase
{
    private readonly AppDbContext _db;

    public MapaController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("puntos")]
    public async Task<ActionResult<IEnumerable<MapaPuntoDto>>> GetPuntos([FromQuery] string? q)
    {
        var coordenadas = await _db.DeclaracionCoordenadas
            .AsNoTracking()
            .Include(c => c.Declaracion)
                .ThenInclude(d => d!.Registro)
            .Include(c => c.Declaracion)
                .ThenInclude(d => d!.Imagenes)
            .Where(c => c.Declaracion != null && c.Declaracion.Registro != null)
            .OrderByDescending(c => c.Declaracion!.Registro!.Fecha)
            .ToListAsync();

        var term = TextNormalize.NormalizeKey(q);
        var puntos = coordenadas.Select(c =>
        {
            var declaracion = c.Declaracion!;
            var registro = declaracion.Registro!;

            return new MapaPuntoDto
            {
                CoordenadaId = c.Id,
                RegistroId = registro.Id,
                DeclaracionId = declaracion.Id,
                Latitud = c.Latitud,
                Longitud = c.Longitud,
                ObservacionCoordenada = c.Observacion,
                Numero = registro.Numero,
                Ap = registro.Ap,
                Tomo = registro.Tomo,
                Foja = registro.Foja,
                Fecha = registro.Fecha,
                Declarante = registro.Declarante,
                DeclaracionOrden = declaracion.Orden,
                TextoResumen = Resume(declaracion.TextoDeclaracion),
                Imagenes = declaracion.Imagenes
                    .OrderBy(i => i.Orden)
                    .Select(i => new MapaImagenDto
                    {
                        Id = i.Id,
                        RutaArchivo = i.RutaArchivo,
                        Descripcion = i.Descripcion,
                        ElementoRelevante = i.ElementoRelevante,
                        TipoImagen = i.TipoImagen
                    })
                    .ToList()
            };
        });

        if (!string.IsNullOrWhiteSpace(term))
        {
            puntos = puntos.Where(p =>
            {
                var normalized = TextNormalize.NormalizeKey(string.Join(" ", new[]
                {
                    p.Numero,
                    p.Ap,
                    p.Tomo,
                    p.Foja,
                    p.Declarante,
                    p.ObservacionCoordenada,
                    p.TextoResumen
                }));

                return normalized?.Contains(term) == true;
            });
        }

        return Ok(puntos.ToList());
    }

    private static string? Resume(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var compact = string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return compact.Length <= 180 ? compact : $"{compact[..180]}...";
    }
}
