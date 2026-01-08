using Contasiscorp.Domain.Entities;
using Contasiscorp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contasiscorp.Infrastructure.Data.Configurations;

public class ComprobanteConfiguration : IEntityTypeConfiguration<Comprobante>
{
    public void Configure(EntityTypeBuilder<Comprobante> builder)
    {
        builder.ToTable("comprobantes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(c => c.Serie)
            .HasColumnName("serie")
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(c => c.Numero)
            .HasColumnName("numero")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(c => c.Tipo)
            .HasColumnName("tipo")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.Estado)
            .HasColumnName("estado")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.FechaEmision)
            .HasColumnName("fecha_emision")
            .IsRequired();

        builder.Property(c => c.RucEmisor)
            .HasColumnName("ruc_emisor")
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(c => c.RazonSocialEmisor)
            .HasColumnName("razon_social_emisor")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.RucReceptor)
            .HasColumnName("ruc_receptor")
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(c => c.RazonSocialReceptor)
            .HasColumnName("razon_social_receptor")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.MontoTotal)
            .HasColumnName("monto_total")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.MontoIGV)
            .HasColumnName("monto_igv")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.MontoSubtotal)
            .HasColumnName("monto_subtotal")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.Moneda)
            .HasColumnName("moneda")
            .HasMaxLength(3)
            .HasDefaultValue("PEN")
            .IsRequired();

        builder.Property(c => c.Observaciones)
            .HasColumnName("observaciones")
            .HasMaxLength(500);

        builder.Property(c => c.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .IsRequired();

        builder.Property(c => c.FechaModificacion)
            .HasColumnName("fecha_modificacion");

        builder.Property(c => c.UsuarioCreacion)
            .HasColumnName("usuario_creacion")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.UsuarioModificacion)
            .HasColumnName("usuario_modificacion")
            .HasMaxLength(100);

        builder.HasMany(c => c.Items)
            .WithOne(i => i.Comprobante)
            .HasForeignKey(i => i.ComprobanteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.Serie, c.Numero })
            .IsUnique();

        builder.HasIndex(c => c.FechaEmision);
        builder.HasIndex(c => c.RucReceptor);
    }
}
