using ComplejoDeportivo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComplejoDeportivo.Infrastructure.BaseDeDatos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Espacio> Espacios => Set<Espacio>();
        public DbSet<Socio> Socios => Set<Socio>();
        public DbSet<Reserva> Reservas => Set<Reserva>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}