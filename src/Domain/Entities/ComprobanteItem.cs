namespace Contasiscorp.Domain.Entities;

public class ComprobanteItem
{
    public Guid Id { get; set; }
    public Guid ComprobanteId { get; set; }
    public string CodigoProducto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal { get; set; }
    public string UnidadMedida { get; set; } = "NIU";

    public Comprobante Comprobante { get; set; } = null!;

    public void CalcularSubTotal()
    {
        SubTotal = Cantidad * PrecioUnitario;
    }
}
