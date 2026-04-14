namespace CoahuilaRelacion.Api.Dtos;

public class MapaPuntoDto
{
    public Guid CoordenadaId { get; set; }
    public Guid RegistroId { get; set; }
    public Guid DeclaracionId { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public string? ObservacionCoordenada { get; set; }
    public string? Numero { get; set; }
    public string? Ap { get; set; }
    public string? Tomo { get; set; }
    public string? Foja { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Declarante { get; set; }
    public int DeclaracionOrden { get; set; }
    public string? TextoResumen { get; set; }
    public List<MapaImagenDto> Imagenes { get; set; } = new();
}

public class MapaImagenDto
{
    public Guid Id { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ElementoRelevante { get; set; }
    public string? TipoImagen { get; set; }
}
