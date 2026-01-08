using Contasiscorp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contasiscorp.Infrastructure.Data.Configurations;

public class ComprobanteItemConfiguration : IEntityTypeConfiguration<ComprobanteItem>
{
    public void Configure(EntityTypeBuilder<ComprobanteItem> builder)
    {
        builder.ToTable("comprobante_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(i => i.ComprobanteId)
            .HasColumnName("comprobante_id")
            .IsRequired();

        builder.Property(i => i.CodigoProducto)
            .HasColumnName("codigo_producto")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(i => i.Cantidad)
            .HasColumnName("cantidad")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(i => i.PrecioUnitario)
            .HasColumnName("precio_unitario")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.SubTotal)
            .HasColumnName("sub_total")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.UnidadMedida)
            .HasColumnName("unidad_medida")
            .HasMaxLength(10)
            .HasDefaultValue("NIU")
            .IsRequired();
    }
}
