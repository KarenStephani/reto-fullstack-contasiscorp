using Contasiscorp.Application.Interfaces;
using Contasiscorp.Domain.Entities;
using Contasiscorp.Domain.Enums;
using MediatR;

namespace Contasiscorp.Application.Commands.CrearComprobante;

public class CrearComprobanteHandler : IRequestHandler<CrearComprobanteCommand, CrearComprobanteResult>
{
    private readonly IComprobanteRepository _repository;

    public CrearComprobanteHandler(IComprobanteRepository repository)
    {
        _repository = repository;
    }

    public async Task<CrearComprobanteResult> Handle(CrearComprobanteCommand request, CancellationToken cancellationToken)
    {
        var comprobante = new Comprobante
        {
            Id = Guid.NewGuid(),
            Serie = request.Serie,
            Numero = request.Numero,
            Tipo = request.Tipo,
            Estado = EstadoComprobante.Emitido,
            FechaEmision = request.FechaEmision,
            RucEmisor = request.RucEmisor,
            RazonSocialEmisor = request.RazonSocialEmisor,
            RucReceptor = request.RucReceptor,
            RazonSocialReceptor = request.RazonSocialReceptor,
            Moneda = request.Moneda,
            Observaciones = request.Observaciones,
            FechaCreacion = DateTime.UtcNow,
            UsuarioCreacion = request.UsuarioCreacion
        };

        foreach (var itemDto in request.Items)
        {
            var item = new ComprobanteItem
            {
                Id = Guid.NewGuid(),
                CodigoProducto = itemDto.CodigoProducto,
                Descripcion = itemDto.Descripcion,
                Cantidad = itemDto.Cantidad,
                PrecioUnitario = itemDto.PrecioUnitario,
                UnidadMedida = itemDto.UnidadMedida
            };
            item.CalcularSubTotal();
            comprobante.Items.Add(item);
        }

        comprobante.CalcularTotales();

        await _repository.AddAsync(comprobante);
        await _repository.SaveChangesAsync();

        return new CrearComprobanteResult
        {
            Id = comprobante.Id,
            Serie = comprobante.Serie,
            Numero = comprobante.Numero,
            MontoTotal = comprobante.MontoTotal
        };
    }
}
