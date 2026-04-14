using CoahuilaRelacion.Api.Data;
using CoahuilaRelacion.Api.Dtos;
using CoahuilaRelacion.Api.Models;
using CoahuilaRelacion.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Controllers;

[ApiController]
[Route("api/relaciones")]
public class RelacionesController : ControllerBase
{
    private readonly AppDbContext _db;

    public RelacionesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<RelacionesDashboardDto>> Get([FromQuery] string? q)
    {
        var term = TextNormalize.NormalizeKey(q);

        var actores = await _db.DeclaracionActores
            .AsNoTracking()
            .Include(x => x.Actor)
            .Include(x => x.Declaracion)
                .ThenInclude(d => d!.Registro)
            .ToListAsync();

        var lugares = await _db.DeclaracionLugares
            .AsNoTracking()
            .Include(x => x.Lugar)
            .Include(x => x.Declaracion)
                .ThenInclude(d => d!.Registro)
            .ToListAsync();

        var circunstancias = await _db.DeclaracionCircunstancias
            .AsNoTracking()
            .Include(x => x.Circunstancia)
            .Include(x => x.Declaracion)
                .ThenInclude(d => d!.Registro)
            .ToListAsync();

        return Ok(new RelacionesDashboardDto
        {
            Actores = actores
                .Where(x => x.Actor != null)
                .GroupBy(x => new { x.ActorId, x.Actor!.Nombre, x.Actor.NombreNormalizado })
                .Select(g => BuildResumen("Actor", g.Key.ActorId, g.Key.Nombre, g.Select(x => x.Declaracion)))
                .Where(x => Matches(x, term))
                .OrderByDescending(x => x.TotalRegistros)
                .ThenBy(x => x.Nombre)
                .ToList(),
            Lugares = lugares
                .Where(x => x.Lugar != null)
                .GroupBy(x => new { x.LugarId, x.Lugar!.Nombre, x.Lugar.NombreNormalizado })
                .Select(g => BuildResumen("Lugar", g.Key.LugarId, g.Key.Nombre, g.Select(x => x.Declaracion)))
                .Where(x => Matches(x, term))
                .OrderByDescending(x => x.TotalRegistros)
                .ThenBy(x => x.Nombre)
                .ToList(),
            Circunstancias = circunstancias
                .Where(x => x.Circunstancia != null)
                .GroupBy(x => new { x.CircunstanciaId, x.Circunstancia!.Nombre, x.Circunstancia.NombreNormalizado })
                .Select(g => BuildResumen("Circunstancia", g.Key.CircunstanciaId, g.Key.Nombre, g.Select(x => x.Declaracion)))
                .Where(x => Matches(x, term))
                .OrderByDescending(x => x.TotalRegistros)
                .ThenBy(x => x.Nombre)
                .ToList()
        });
    }

    private static RelacionResumenDto BuildResumen(
        string tipo,
        Guid id,
        string nombre,
        IEnumerable<Declaracion?> declaraciones)
    {
        var coincidencias = declaraciones
            .Where(d => d != null && d.Registro != null)
            .Select(d => new RelacionCoincidenciaDto
            {
                RegistroId = d!.RegistroId,
                DeclaracionId = d.Id,
                Numero = d.Registro!.Numero,
                Ap = d.Registro.Ap,
                Tomo = d.Registro.Tomo,
                Foja = d.Registro.Foja,
                Fecha = d.Registro.Fecha,
                Declarante = d.Registro.Declarante,
                DeclaracionOrden = d.Orden,
                TextoResumen = Resume(d.TextoDeclaracion)
            })
            .OrderByDescending(x => x.Fecha)
            .ThenBy(x => x.Ap)
            .ToList();

        return new RelacionResumenDto
        {
            Id = id,
            Tipo = tipo,
            Nombre = nombre,
            TotalDeclaraciones = coincidencias.Select(x => x.DeclaracionId).Distinct().Count(),
            TotalRegistros = coincidencias.Select(x => x.RegistroId).Distinct().Count(),
            Coincidencias = coincidencias
        };
    }

    private static bool Matches(RelacionResumenDto item, string? normalizedTerm)
    {
        if (string.IsNullOrWhiteSpace(normalizedTerm))
        {
            return true;
        }

        var haystack = TextNormalize.NormalizeKey(string.Join(" ", new[]
        {
            item.Nombre,
            item.Tipo,
            string.Join(" ", item.Coincidencias.Select(x => $"{x.Numero} {x.Ap} {x.Tomo} {x.Foja} {x.Declarante} {x.TextoResumen}"))
        }));

        return haystack?.Contains(normalizedTerm) == true;
    }

    private static string? Resume(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var compact = string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return compact.Length <= 160 ? compact : $"{compact[..160]}...";
    }
}
