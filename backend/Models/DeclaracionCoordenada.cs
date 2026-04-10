using System.Text.Json.Serialization;

namespace CoahuilaRelacion.Api.Models;

public class DeclaracionCoordenada
{
    public Guid Id { get; set; }
    public Guid DeclaracionId { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public double? PrecisionM { get; set; }
    public string? Fuente { get; set; }
    public string? Observacion { get; set; }

    [JsonIgnore]
    public Declaracion? Declaracion { get; set; }
}
