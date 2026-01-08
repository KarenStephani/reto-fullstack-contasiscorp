using Contasiscorp.Application.Commands.AnularComprobante;
using FluentValidation;

namespace Contasiscorp.Application.Validators;

public class AnularComprobanteValidator : AbstractValidator<AnularComprobanteCommand>
{
    public AnularComprobanteValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del comprobante es requerido");

        RuleFor(x => x.UsuarioModificacion)
            .NotEmpty().WithMessage("El usuario que anula es requerido");
    }
}
