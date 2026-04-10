namespace CoahuilaRelacion.Api.Models;

public class Registro
{
    public Guid Id { get; set; }
    public string? Numero { get; set; }
    public string? Ap { get; set; }
    public string? Tomo { get; set; }
    public string? Foja { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Entrevistado { get; set; }
    public string? Declarante { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    public List<Declaracion> Declaraciones { get; set; } = new();
}
