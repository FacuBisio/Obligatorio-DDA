using ComplejoDeportivo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplejoDeportivo.Infrastructure.BaseDeDatos.Configuraciones
{
    public class EspacioConfiguration : IEntityTypeConfiguration<Espacio>
    {
        public void Configure(EntityTypeBuilder<Espacio> builder)
        {
            builder.ToTable("Espacios");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Capacidad).IsRequired();
            builder.Property(e => e.FueraDeServicio).IsRequired();
            builder.Property(e => e.PrecioHoraBase).HasColumnType("decimal(18,2)").IsRequired();

            // Autorrelación recursiva (Composite simplificado)
            builder.HasOne(e => e.EspacioPadre)
                   .WithMany(e => e.SubEspacios)
                   .HasForeignKey(e => e.EspacioPadreId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(e => e.SubEspacios)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}