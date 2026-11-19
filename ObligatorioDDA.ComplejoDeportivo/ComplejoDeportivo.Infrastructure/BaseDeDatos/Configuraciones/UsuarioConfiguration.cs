using ComplejoDeportivo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplejoDeportivo.Infrastructure.BaseDeDatos.Configuraciones
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

            builder.Property(u => u.Rol)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.HasOne(u => u.Socio)
                   .WithOne()
                   .HasForeignKey<Usuario>(u => u.SocioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}