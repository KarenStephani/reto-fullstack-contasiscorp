using Contasiscorp.Domain.Enums;

namespace Contasiscorp.Api.DTOs.Requests;

public class CrearComprobanteRequest
{
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public TipoComprobante Tipo { get; set; }
    public DateTime FechaEmision { get; set; }
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string RucReceptor { get; set; } = string.Empty;
    public string RazonSocialReceptor { get; set; } = string.Empty;
    public string Moneda { get; set; } = "PEN";
    public string? Observaciones { get; set; }
    public List<CrearComprobanteItemRequest> Items { get; set; } = new();
}

public class CrearComprobanteItemRequest
{
    public string CodigoProducto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string UnidadMedida { get; set; } = "NIU";
}
