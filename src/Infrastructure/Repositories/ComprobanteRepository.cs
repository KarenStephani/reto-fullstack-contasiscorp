using Contasiscorp.Application.Interfaces;
using Contasiscorp.Domain.Entities;
using Contasiscorp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Contasiscorp.Infrastructure.Repositories;

public class ComprobanteRepository : IComprobanteRepository
{
    private readonly AppDbContext _context;

    public ComprobanteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Comprobante?> GetByIdAsync(Guid id)
    {
        return await _context.Comprobantes
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Comprobante>> GetAllAsync(
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string? serie = null,
        string? rucReceptor = null)
    {
        var query = _context.Comprobantes
            .Include(c => c.Items)
            .AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(c => c.FechaEmision >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(c => c.FechaEmision <= fechaFin.Value);

        if (!string.IsNullOrEmpty(serie))
            query = query.Where(c => c.Serie == serie);

        if (!string.IsNullOrEmpty(rucReceptor))
            query = query.Where(c => c.RucReceptor == rucReceptor);

        return await query
            .OrderByDescending(c => c.FechaEmision)
            .ToListAsync();
    }

    public async Task AddAsync(Comprobante comprobante)
    {
        await _context.Comprobantes.AddAsync(comprobante);
    }

    public void Update(Comprobante comprobante)
    {
        _context.Comprobantes.Update(comprobante);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
