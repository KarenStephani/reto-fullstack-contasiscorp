using Contasiscorp.Application.Commands.CrearComprobante;
using FluentValidation;

namespace Contasiscorp.Application.Validators;

public class CrearComprobanteValidator : AbstractValidator<CrearComprobanteCommand>
{
    public CrearComprobanteValidator()
    {
        RuleFor(x => x.Serie)
            .NotEmpty().WithMessage("La serie es requerida")
            .MaximumLength(4).WithMessage("La serie no puede exceder 4 caracteres");

        RuleFor(x => x.Numero)
            .NotEmpty().WithMessage("El número es requerido")
            .MaximumLength(8).WithMessage("El número no puede exceder 8 caracteres");

        RuleFor(x => x.RucEmisor)
            .NotEmpty().WithMessage("El RUC del emisor es requerido")
            .Length(11).WithMessage("El RUC debe tener 11 dígitos");

        RuleFor(x => x.RazonSocialEmisor)
            .NotEmpty().WithMessage("La razón social del emisor es requerida")
            .MaximumLength(200).WithMessage("La razón social no puede exceder 200 caracteres");

        RuleFor(x => x.RucReceptor)
            .NotEmpty().WithMessage("El RUC del receptor es requerido")
            .Length(11).WithMessage("El RUC debe tener 11 dígitos");

        RuleFor(x => x.RazonSocialReceptor)
            .NotEmpty().WithMessage("La razón social del receptor es requerida")
            .MaximumLength(200).WithMessage("La razón social no puede exceder 200 caracteres");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Debe incluir al menos un item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida");

            item.RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

            item.RuleFor(x => x.PrecioUnitario)
                .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0");
        });
    }
}
