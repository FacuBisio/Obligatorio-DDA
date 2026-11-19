using ComplejoDeportivo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplejoDeportivo.Infrastructure.BaseDeDatos.Configuraciones
{
    public class SocioConfiguration : IEntityTypeConfiguration<Socio>
    {
        public void Configure(EntityTypeBuilder<Socio> builder)
        {
            builder.ToTable("Socios");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Email).IsRequired().HasMaxLength(150);
            builder.HasIndex(s => s.Email).IsUnique();
            builder.Property(s => s.EsMoroso).IsRequired();

            builder.Property(s => s.TipoCategoria)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            // La instancia de Strategy se reconstruye desde TipoCategoria, no es columna
            builder.Ignore(s => s.Categoria);
        }
    }
}