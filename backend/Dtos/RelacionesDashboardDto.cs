namespace CoahuilaRelacion.Api.Dtos;

public class RelacionesDashboardDto
{
    public List<RelacionResumenDto> Actores { get; set; } = new();
    public List<RelacionResumenDto> Lugares { get; set; } = new();
    public List<RelacionResumenDto> Circunstancias { get; set; } = new();
}

public class RelacionResumenDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int TotalDeclaraciones { get; set; }
    public int TotalRegistros { get; set; }
    public List<RelacionCoincidenciaDto> Coincidencias { get; set; } = new();
}

public class RelacionCoincidenciaDto
{
    public Guid RegistroId { get; set; }
    public Guid DeclaracionId { get; set; }
    public string? Numero { get; set; }
    public string? Ap { get; set; }
    public string? Tomo { get; set; }
    public string? Foja { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Declarante { get; set; }
    public int DeclaracionOrden { get; set; }
    public string? TextoResumen { get; set; }
}
