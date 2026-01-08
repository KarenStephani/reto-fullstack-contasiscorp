using Contasiscorp.Api.DTOs.Requests;
using Contasiscorp.Api.DTOs.Responses;
using Contasiscorp.Application.Commands.AnularComprobante;
using Contasiscorp.Application.Commands.CrearComprobante;
using Contasiscorp.Application.Queries.ListarComprobantes;
using Contasiscorp.Application.Queries.ObtenerComprobante;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contasiscorp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprobantesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ComprobantesController> _logger;

    public ComprobantesController(
        IMediator mediator,
        ILogger<ComprobantesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ComprobanteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ComprobanteResponse>>> GetComprobantes(
        [FromQuery] FiltroComprobanteRequest filtro)
    {
        var query = new ListarComprobantesQuery
        {
            FechaInicio = filtro.FechaInicio,
            FechaFin = filtro.FechaFin,
            Serie = filtro.Serie,
            RucReceptor = filtro.RucReceptor,
            Tipo = filtro.Tipo,
            Estado = filtro.Estado
        };

        var comprobantes = await _mediator.Send(query);

        var response = comprobantes.Select(c => new ComprobanteResponse
        {
            Id = c.Id,
            Serie = c.Serie,
            Numero = c.Numero,
            Tipo = c.Tipo,
            Estado = c.Estado,
            FechaEmision = c.FechaEmision,
            RucEmisor = c.RucEmisor,
            RazonSocialEmisor = c.RazonSocialEmisor,
            RucReceptor = c.RucReceptor,
            RazonSocialReceptor = c.RazonSocialReceptor,
            MontoTotal = c.MontoTotal,
            MontoIGV = c.MontoIGV,
            MontoSubtotal = c.MontoSubtotal,
            Moneda = c.Moneda,
            Observaciones = c.Observaciones,
            Items = c.Items.Select(i => new ComprobanteItemResponse
            {
                Id = i.Id,
                CodigoProducto = i.CodigoProducto,
                Descripcion = i.Descripcion,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                SubTotal = i.SubTotal,
                UnidadMedida = i.UnidadMedida
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ComprobanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComprobanteResponse>> GetComprobante(Guid id)
    {
        var query = new ObtenerComprobanteQuery { Id = id };
        var comprobante = await _mediator.Send(query);

        if (comprobante == null)
            return NotFound(new { message = $"Comprobante {id} no encontrado" });

        var response = new ComprobanteResponse
        {
            Id = comprobante.Id,
            Serie = comprobante.Serie,
            Numero = comprobante.Numero,
            Tipo = comprobante.Tipo,
            Estado = comprobante.Estado,
            FechaEmision = comprobante.FechaEmision,
            RucEmisor = comprobante.RucEmisor,
            RazonSocialEmisor = comprobante.RazonSocialEmisor,
            RucReceptor = comprobante.RucReceptor,
            RazonSocialReceptor = comprobante.RazonSocialReceptor,
            MontoTotal = comprobante.MontoTotal,
            MontoIGV = comprobante.MontoIGV,
            MontoSubtotal = comprobante.MontoSubtotal,
            Moneda = comprobante.Moneda,
            Observaciones = comprobante.Observaciones,
            Items = comprobante.Items.Select(i => new ComprobanteItemResponse
            {
                Id = i.Id,
                CodigoProducto = i.CodigoProducto,
                Descripcion = i.Descripcion,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                SubTotal = i.SubTotal,
                UnidadMedida = i.UnidadMedida
            }).ToList()
        };

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ComprobanteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ComprobanteResponse>> CreateComprobante(
        [FromBody] CrearComprobanteRequest request)
    {
        var command = new CrearComprobanteCommand
        {
            Serie = request.Serie,
            Numero = request.Numero,
            Tipo = request.Tipo,
            FechaEmision = request.FechaEmision,
            RucEmisor = request.RucEmisor,
            RazonSocialEmisor = request.RazonSocialEmisor,
            RucReceptor = request.RucReceptor,
            RazonSocialReceptor = request.RazonSocialReceptor,
            Moneda = request.Moneda,
            Observaciones = request.Observaciones,
            UsuarioCreacion = "system",
            Items = request.Items.Select(i => new CrearComprobanteItemDto
            {
                CodigoProducto = i.CodigoProducto,
                Descripcion = i.Descripcion,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                UnidadMedida = i.UnidadMedida
            }).ToList()
        };

        var result = await _mediator.Send(command);

        _logger.LogInformation("Comprobante creado: {Id}", result.Id);

        var response = new ComprobanteResponse
        {
            Id = result.Id,
            Serie = result.Serie,
            Numero = result.Numero,
            MontoTotal = result.MontoTotal
        };

        return CreatedAtAction(nameof(GetComprobante), new { id = result.Id }, response);
    }

    [HttpPut("{id}/anular")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AnularComprobante(Guid id)
    {
        var command = new AnularComprobanteCommand
        {
            Id = id,
            UsuarioModificacion = "system"
        };

        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = $"Comprobante {id} no encontrado" });

        _logger.LogInformation("Comprobante anulado: {Id}", id);

        return Ok(new { message = "Comprobante anulado exitosamente" });
    }
}
