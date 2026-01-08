using MediatR;

namespace Contasiscorp.Application.Commands.AnularComprobante;

public class AnularComprobanteCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string UsuarioModificacion { get; set; } = string.Empty;
}
