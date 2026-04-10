namespace CoahuilaRelacion.Api.Dtos;

public class DeclaracionCreateDto
{
    public Guid? Id { get; set; }
    public string? TextoDeclaracion { get; set; }
    public int Orden { get; set; }
    public string? Tipo { get; set; }
    public DateTime? FechaCaptura { get; set; }
    public string? Observaciones { get; set; }
    public List<DeclaracionCoordenadaDto> Coordenadas { get; set; } = new();
    public List<DeclaracionFechaDto> Fechas { get; set; } = new();
}
