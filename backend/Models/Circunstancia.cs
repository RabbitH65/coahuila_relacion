namespace CoahuilaRelacion.Api.Models;

public class Circunstancia
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? NombreNormalizado { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
}
