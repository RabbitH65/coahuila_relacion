namespace CoahuilaRelacion.Api.Dtos;

public class DeclaracionActorLinkDto
{
    public Guid? ActorId { get; set; }
    public string? Nombre { get; set; }
    public string? Alias { get; set; }
    public string? Notas { get; set; }
    public string? Origen { get; set; }
    public double? Confianza { get; set; }
    public bool? Confirmado { get; set; }
}
