using Contasiscorp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contasiscorp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Comprobante> Comprobantes { get; set; }
    public DbSet<ComprobanteItem> ComprobanteItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
