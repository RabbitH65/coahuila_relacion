using System.Text.Json.Serialization;

namespace CoahuilaRelacion.Api.Models;

public class Declaracion
{
    public Guid Id { get; set; }
    public Guid RegistroId { get; set; }
    public string? TextoDeclaracion { get; set; }
    public int Orden { get; set; }
    public string? Tipo { get; set; }
    public DateTime? FechaCaptura { get; set; }
    public string? Observaciones { get; set; }

    [JsonIgnore]
    public Registro? Registro { get; set; }

    public List<DeclaracionCoordenada> Coordenadas { get; set; } = new();
    public List<DeclaracionFecha> Fechas { get; set; } = new();
    public List<DeclaracionImagen> Imagenes { get; set; } = new();
    public List<DeclaracionActor> Actores { get; set; } = new();
    public List<DeclaracionLugar> Lugares { get; set; } = new();
    public List<DeclaracionCircunstancia> Circunstancias { get; set; } = new();
}
