using Contasiscorp.Application.Commands.CrearComprobante;
using Contasiscorp.Application.Validators;
using Contasiscorp.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Contasiscorp.Tests.Application;

public class ValidatorTests
{
    private readonly CrearComprobanteValidator _validator;

    public ValidatorTests()
    {
        _validator = new CrearComprobanteValidator();
    }

    [Fact]
    public async Task Validator_Succeeds_WithValidFactura()
    {
        var dto = CreateValidFacturaDto();

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_Fails_WhenSerieIsEmpty()
    {
        var dto = CreateValidFacturaDto();
        dto.Serie = "";

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Serie");
    }

    [Fact]
    public async Task Validator_Fails_WhenRucEmisorIsInvalid()
    {
        var dto = CreateValidFacturaDto();
        dto.RucEmisor = "123";

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RucEmisor");
    }

    [Fact]
    public async Task Validator_Fails_WhenFacturaHasNoRucReceptor()
    {
        var dto = CreateValidFacturaDto();
        dto.RucReceptor = "";

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RucReceptor");
    }

    [Fact]
    public async Task Validator_Fails_WhenSerieFormatIsWrong()
    {
        var dto = CreateValidFacturaDto();
        dto.Serie = "B001";

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Serie");
    }

    [Fact]
    public async Task Validator_Fails_WhenNoItems()
    {
        var dto = CreateValidFacturaDto();
        dto.Items = new List<CrearComprobanteItemDto>();

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }

    [Fact]
    public async Task Validator_Succeeds_WithValidBoleta()
    {
        var dto = new CrearComprobanteCommand
        {
            Tipo = TipoComprobante.Boleta,
            Serie = "B001",
            Numero = "00000001",
            FechaEmision = DateTime.Now,
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Empresa Test SAC",
            UsuarioCreacion = "test",
            Items = new List<CrearComprobanteItemDto>
            {
                new CrearComprobanteItemDto
                {
                    CodigoProducto = "P001",
                    Descripcion = "Producto Test",
                    Cantidad = 1,
                    PrecioUnitario = 100,
                    UnidadMedida = "NIU"
                }
            }
        };

        var result = await _validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    private static CrearComprobanteCommand CreateValidFacturaDto()
    {
        return new CrearComprobanteCommand
        {
            Tipo = TipoComprobante.Factura,
            Serie = "F001",
            Numero = "00000001",
            FechaEmision = DateTime.Now,
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Empresa Test SAC",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente Test SAC",
            UsuarioCreacion = "test",
            Items = new List<CrearComprobanteItemDto>
            {
                new CrearComprobanteItemDto
                {
                    CodigoProducto = "P001",
                    Descripcion = "Producto Test",
                    Cantidad = 1,
                    PrecioUnitario = 100,
                    UnidadMedida = "NIU"
                }
            }
        };
    }
}
