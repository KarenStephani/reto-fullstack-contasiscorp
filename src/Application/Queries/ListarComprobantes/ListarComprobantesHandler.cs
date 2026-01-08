using Contasiscorp.Application.Interfaces;
using Contasiscorp.Application.Queries.ObtenerComprobante;
using MediatR;

namespace Contasiscorp.Application.Queries.ListarComprobantes;

public class ListarComprobantesHandler : IRequestHandler<ListarComprobantesQuery, List<ComprobanteDto>>
{
    private readonly IComprobanteRepository _repository;

    public ListarComprobantesHandler(IComprobanteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ComprobanteDto>> Handle(ListarComprobantesQuery request, CancellationToken cancellationToken)
    {
        var comprobantes = await _repository.GetAllAsync(
            request.FechaInicio,
            request.FechaFin,
            request.Serie,
            request.RucReceptor,
            request.Tipo,
            request.Estado
        );

        return comprobantes.Select(c => new ComprobanteDto
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
            Items = c.Items.Select(i => new ComprobanteItemDto
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
    }
}
