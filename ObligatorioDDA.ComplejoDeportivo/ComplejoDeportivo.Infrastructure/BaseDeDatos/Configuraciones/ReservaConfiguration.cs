using ComplejoDeportivo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplejoDeportivo.Infrastructure.BaseDeDatos.Configuraciones
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.ToTable("Reservas");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Fecha).IsRequired();
            builder.Property(r => r.HoraInicio).IsRequired();
            builder.Property(r => r.DuracionHoras).IsRequired();
            builder.Property(r => r.MontoTotal).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(r => r.MontoReintegrado).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(r => r.FechaCreacionUtc).IsRequired();

            builder.Property(r => r.Estado)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            // Token de concurrencia optimista (RowVersion en SQL Server)
            builder.Property<byte[]>("RowVersion")
                   .IsRowVersion();

            builder.HasOne(r => r.Socio)
                   .WithMany()
                   .HasForeignKey(r => r.SocioId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Espacio)
                   .WithMany(e => e.Reservas)
                   .HasForeignKey(r => r.EspacioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}