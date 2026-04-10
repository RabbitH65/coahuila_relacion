using Microsoft.AspNetCore.Http;

namespace CoahuilaRelacion.Api.Dtos;

public class DeclaracionImagenUploadDto
{
    public IFormFile? Archivo { get; set; }
    public string? Descripcion { get; set; }
    public string? ElementoRelevante { get; set; }
    public string? TipoImagen { get; set; }
    public int? Orden { get; set; }
    public DateTime? FechaCaptura { get; set; }
}
