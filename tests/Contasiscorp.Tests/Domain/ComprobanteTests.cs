using Contasiscorp.Domain.Entities;
using Contasiscorp.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Contasiscorp.Tests.Domain;

public class ComprobanteTests
{
    [Fact]
    public void Comprobante_CalculatesSubTotalCorrectly()
    {
        var comprobante = CreateValidFactura();
        comprobante.CalcularTotales();

        comprobante.MontoSubtotal.Should().Be(100m);
    }

    [Fact]
    public void Comprobante_CalculatesIGVCorrectly()
    {
        var comprobante = CreateValidFactura();
        comprobante.CalcularTotales();

        comprobante.MontoIGV.Should().Be(18m);
    }

    [Fact]
    public void Comprobante_CalculatesTotalCorrectly()
    {
        var comprobante = CreateValidFactura();
        comprobante.CalcularTotales();

        comprobante.MontoTotal.Should().Be(118m);
    }

    [Fact]
    public void Anular_ChangesEstadoToAnulado()
    {
        var comprobante = CreateValidFactura();

        comprobante.Anular();

        comprobante.Estado.Should().Be(EstadoComprobante.Anulado);
    }

    [Fact]
    public void Anular_ThrowsException_WhenAlreadyAnulado()
    {
        var comprobante = CreateValidFactura();
        comprobante.Anular();

        var act = () => comprobante.Anular();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Comprobante_CanBeCreatedWithValidData()
    {
        var comprobante = CreateValidFactura();

        comprobante.Should().NotBeNull();
        comprobante.RucEmisor.Should().HaveLength(11);
        comprobante.Serie.Should().Be("F001");
        comprobante.Items.Should().HaveCount(1);
    }

    [Fact]
    public void CalcularTotales_UpdatesAllMontoFields()
    {
        var comprobante = new Comprobante
        {
            Tipo = TipoComprobante.Factura,
            Serie = "F001",
            Numero = "00000001",
            FechaEmision = DateTime.UtcNow,
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Empresa Test SAC",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente Test SAC",
            Estado = EstadoComprobante.Vigente,
            Items = new List<ComprobanteItem>
            {
                new ComprobanteItem
                {
                    CodigoProducto = "P001",
                    Descripcion = "Producto 1",
                    Cantidad = 2,
                    PrecioUnitario = 50,
                    SubTotal = 100
                },
                new ComprobanteItem
                {
                    CodigoProducto = "P002",
                    Descripcion = "Producto 2",
                    Cantidad = 1,
                    PrecioUnitario = 100,
                    SubTotal = 100
                }
            }
        };

        comprobante.CalcularTotales();

        comprobante.MontoSubtotal.Should().Be(200m);
        comprobante.MontoIGV.Should().Be(36m);
        comprobante.MontoTotal.Should().Be(236m);
    }

    private static Comprobante CreateValidFactura()
    {
        return new Comprobante
        {
            Tipo = TipoComprobante.Factura,
            Serie = "F001",
            Numero = "00000001",
            FechaEmision = DateTime.UtcNow,
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Empresa Test SAC",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente Test SAC",
            Estado = EstadoComprobante.Vigente,
            Items = new List<ComprobanteItem>
            {
                new ComprobanteItem
                {
                    CodigoProducto = "P001",
                    Descripcion = "Producto Test",
                    Cantidad = 2,
                    PrecioUnitario = 50,
                    SubTotal = 100
                }
            }
        };
    }
}
