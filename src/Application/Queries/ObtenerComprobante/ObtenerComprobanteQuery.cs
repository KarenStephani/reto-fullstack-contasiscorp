using MediatR;

namespace Contasiscorp.Application.Queries.ObtenerComprobante;

public class ObtenerComprobanteQuery : IRequest<ComprobanteDto?>
{
    public Guid Id { get; set; }
}
