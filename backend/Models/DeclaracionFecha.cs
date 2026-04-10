using System.Text.Json.Serialization;

namespace CoahuilaRelacion.Api.Models;

public class DeclaracionFecha
{
    public Guid Id { get; set; }
    public Guid DeclaracionId { get; set; }
    public DateTime Fecha { get; set; }
    public string? TipoFecha { get; set; }
    public string? Observacion { get; set; }

    [JsonIgnore]
    public Declaracion? Declaracion { get; set; }
}
