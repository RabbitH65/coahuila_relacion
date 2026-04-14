namespace CoahuilaRelacion.Api.Dtos;

public class LugarCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
    public string? Municipio { get; set; }
    public string? Notas { get; set; }
}
