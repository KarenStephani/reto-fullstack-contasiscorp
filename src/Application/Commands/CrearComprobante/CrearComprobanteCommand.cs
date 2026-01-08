using Contasiscorp.Domain.Enums;
using MediatR;

namespace Contasiscorp.Application.Commands.CrearComprobante;

public class CrearComprobanteCommand : IRequest<CrearComprobanteResult>
{
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public TipoComprobante Tipo { get; set; }
    public DateTime FechaEmision { get; set; }
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string? RucReceptor { get; set; }
    public string? RazonSocialReceptor { get; set; }
    public string Moneda { get; set; } = "PEN";
    public string? Observaciones { get; set; }
    public string UsuarioCreacion { get; set; } = "system";
    public List<CrearComprobanteItemDto> Items { get; set; } = new();
}

public class CrearComprobanteItemDto
{
    public string CodigoProducto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string UnidadMedida { get; set; } = "NIU";
}
