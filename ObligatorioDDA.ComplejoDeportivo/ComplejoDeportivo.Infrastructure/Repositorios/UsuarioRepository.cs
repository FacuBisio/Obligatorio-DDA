using ComplejoDeportivo.Domain.Entities;
using ComplejoDeportivo.Domain.Repositories;
using ComplejoDeportivo.Infrastructure.BaseDeDatos;
using Microsoft.EntityFrameworkCore;

namespace ComplejoDeportivo.Infrastructure.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorId(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Socio)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> ObtenerPorEmail(string email)
        {
            var normalizado = email.Trim().ToLowerInvariant();
            return await _context.Usuarios
                .Include(u => u.Socio)
                .FirstOrDefaultAsync(u => u.Email == normalizado);
        }

        public async Task<bool> ExisteEmail(string email)
        {
            var normalizado = email.Trim().ToLowerInvariant();
            return await _context.Usuarios.AnyAsync(u => u.Email == normalizado);
        }

        public async Task Guardar(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }
    }
}