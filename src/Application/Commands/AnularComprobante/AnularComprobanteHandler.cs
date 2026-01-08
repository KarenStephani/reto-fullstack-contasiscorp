using Contasiscorp.Application.Interfaces;
using MediatR;

namespace Contasiscorp.Application.Commands.AnularComprobante;

public class AnularComprobanteHandler : IRequestHandler<AnularComprobanteCommand, bool>
{
    private readonly IComprobanteRepository _repository;

    public AnularComprobanteHandler(IComprobanteRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(AnularComprobanteCommand request, CancellationToken cancellationToken)
    {
        var comprobante = await _repository.GetByIdAsync(request.Id);

        if (comprobante == null)
            return false;

        comprobante.Anular();
        comprobante.FechaModificacion = DateTime.UtcNow;
        comprobante.UsuarioModificacion = request.UsuarioModificacion;

        _repository.Update(comprobante);
        await _repository.SaveChangesAsync();

        return true;
    }
}
