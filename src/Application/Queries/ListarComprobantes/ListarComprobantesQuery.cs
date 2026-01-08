using Contasiscorp.Application.Queries.ObtenerComprobante;
using MediatR;

namespace Contasiscorp.Application.Queries.ListarComprobantes;

public class ListarComprobantesQuery : IRequest<List<ComprobanteDto>>
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Serie { get; set; }
    public string? RucReceptor { get; set; }
}
