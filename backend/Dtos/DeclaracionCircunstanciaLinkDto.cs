namespace CoahuilaRelacion.Api.Dtos;

public class DeclaracionCircunstanciaLinkDto
{
    public Guid? CircunstanciaId { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Origen { get; set; }
    public double? Confianza { get; set; }
    public bool? Confirmado { get; set; }
}
