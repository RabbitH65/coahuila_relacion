namespace CoahuilaRelacion.Api.Dtos;

public class DeclaracionCoordenadaDto
{
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public double? PrecisionM { get; set; }
    public string? Fuente { get; set; }
    public string? Observacion { get; set; }
}
