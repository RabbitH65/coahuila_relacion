using System.Text.Json.Serialization;

namespace CoahuilaRelacion.Api.Models;

public class DeclaracionLugar
{
    public Guid Id { get; set; }
    public Guid DeclaracionId { get; set; }
    public Guid LugarId { get; set; }
    public string? Origen { get; set; }
    public double? Confianza { get; set; }
    public bool Confirmado { get; set; }

    [JsonIgnore]
    public Declaracion? Declaracion { get; set; }

    public Lugar? Lugar { get; set; }
}
