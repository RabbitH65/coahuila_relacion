namespace CoahuilaRelacion.Api.Models;

public class Actor
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? NombreNormalizado { get; set; }
    public string? Alias { get; set; }
    public string? Notas { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
}
