namespace CoahuilaRelacion.Api.Dtos;

public class RegistroUpdateDto
{
    public string? Numero { get; set; }
    public string? Ap { get; set; }
    public string? Tomo { get; set; }
    public string? Foja { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Entrevistado { get; set; }
    public string? Declarante { get; set; }
    public string? Observaciones { get; set; }
    public bool? Activo { get; set; }
    public List<DeclaracionCreateDto>? Declaraciones { get; set; }
}
