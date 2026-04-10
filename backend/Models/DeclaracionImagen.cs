using System.Text.Json.Serialization;

namespace CoahuilaRelacion.Api.Models;

public class DeclaracionImagen
{
    public Guid Id { get; set; }
    public Guid DeclaracionId { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ElementoRelevante { get; set; }
    public string? TipoImagen { get; set; }
    public int Orden { get; set; }
    public DateTime FechaCarga { get; set; }
    public DateTime? FechaCaptura { get; set; }

    [JsonIgnore]
    public Declaracion? Declaracion { get; set; }
}
