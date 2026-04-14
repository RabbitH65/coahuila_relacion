namespace CoahuilaRelacion.Api.Models;

public class Lugar
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? NombreNormalizado { get; set; }
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
    public string? Municipio { get; set; }
    public string? Notas { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
}
