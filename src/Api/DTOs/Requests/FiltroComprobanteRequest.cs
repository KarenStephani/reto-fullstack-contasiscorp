namespace Contasiscorp.Api.DTOs.Requests;

public class FiltroComprobanteRequest
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Serie { get; set; }
    public string? RucReceptor { get; set; }
    public string? Tipo { get; set; }
    public string? Estado { get; set; }
}
