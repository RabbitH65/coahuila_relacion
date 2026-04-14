namespace CoahuilaRelacion.Api.Dtos;

public class DeclaracionLugarLinkDto
{
    public Guid? LugarId { get; set; }
    public string? Nombre { get; set; }
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
    public string? Municipio { get; set; }
    public string? Notas { get; set; }
    public string? Origen { get; set; }
    public double? Confianza { get; set; }
    public bool? Confirmado { get; set; }
}
