using Contasiscorp.Application.Interfaces;
using MediatR;

namespace Contasiscorp.Application.Queries.ObtenerComprobante;

public class ObtenerComprobanteHandler : IRequestHandler<ObtenerComprobanteQuery, ComprobanteDto?>
{
    private readonly IComprobanteRepository _repository;

    public ObtenerComprobanteHandler(IComprobanteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ComprobanteDto?> Handle(ObtenerComprobanteQuery request, CancellationToken cancellationToken)
    {
        var comprobante = await _repository.GetByIdAsync(request.Id);

        if (comprobante == null)
            return null;

        return new ComprobanteDto
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
            Items = comprobante.Items.Select(i => new ComprobanteItemDto
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
    }
}
