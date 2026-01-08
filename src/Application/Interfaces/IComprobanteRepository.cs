using Contasiscorp.Domain.Entities;

namespace Contasiscorp.Application.Interfaces;

public interface IComprobanteRepository
{
    Task<Comprobante?> GetByIdAsync(Guid id);
    Task<List<Comprobante>> GetAllAsync(
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string? serie = null,
        string? rucReceptor = null,
        string? tipo = null,
        string? estado = null
    );
    Task AddAsync(Comprobante comprobante);
    void Update(Comprobante comprobante);
    Task<bool> SaveChangesAsync();
}
