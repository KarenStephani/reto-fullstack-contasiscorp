using Contasiscorp.Domain.Enums;

namespace Contasiscorp.Api.DTOs.Responses;

public class ComprobanteResponse
{
    public Guid Id { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string NumeroCompleto => $"{Serie}-{Numero}";
    public TipoComprobante Tipo { get; set; }
    public EstadoComprobante Estado { get; set; }
    public DateTime FechaEmision { get; set; }
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string RucReceptor { get; set; } = string.Empty;
    public string RazonSocialReceptor { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public decimal MontoIGV { get; set; }
    public decimal MontoSubtotal { get; set; }
    public string Moneda { get; set; } = "PEN";
    public string? Observaciones { get; set; }
    public List<ComprobanteItemResponse> Items { get; set; } = new();
}

public class ComprobanteItemResponse
{
    public Guid Id { get; set; }
    public string CodigoProducto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal { get; set; }
    public string UnidadMedida { get; set; } = "NIU";
}
