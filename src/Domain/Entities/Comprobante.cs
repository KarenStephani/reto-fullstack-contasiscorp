using Contasiscorp.Domain.Enums;

namespace Contasiscorp.Domain.Entities;

public class Comprobante
{
    public Guid Id { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
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
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public string UsuarioCreacion { get; set; } = string.Empty;
    public string? UsuarioModificacion { get; set; }

    public ICollection<ComprobanteItem> Items { get; set; } = new List<ComprobanteItem>();

    public void Anular()
    {
        if (Estado == EstadoComprobante.Anulado)
            throw new InvalidOperationException("El comprobante ya está anulado");

        Estado = EstadoComprobante.Anulado;
    }

    public void CalcularTotales()
    {
        MontoSubtotal = Items.Sum(i => i.SubTotal);
        MontoIGV = MontoSubtotal * 0.18m;
        MontoTotal = MontoSubtotal + MontoIGV;
    }
}
