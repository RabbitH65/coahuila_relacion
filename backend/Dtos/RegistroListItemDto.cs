namespace CoahuilaRelacion.Api.Dtos;

public class RegistroListItemDto
{
    public Guid Id { get; set; }
    public string? Numero { get; set; }
    public string? Ap { get; set; }
    public string? Tomo { get; set; }
    public string? Foja { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Entrevistado { get; set; }
    public string? Declarante { get; set; }
    public int DeclaracionesCount { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
