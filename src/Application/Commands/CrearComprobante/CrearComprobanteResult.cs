namespace Contasiscorp.Application.Commands.CrearComprobante;

public class CrearComprobanteResult
{
    public Guid Id { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string NumeroCompleto => $"{Serie}-{Numero}";
    public decimal MontoTotal { get; set; }
}
