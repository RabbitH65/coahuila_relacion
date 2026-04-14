namespace CoahuilaRelacion.Api.Dtos;

public class ActorCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string? Notas { get; set; }
}
