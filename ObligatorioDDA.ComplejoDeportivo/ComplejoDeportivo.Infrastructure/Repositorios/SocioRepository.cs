using ComplejoDeportivo.Domain.Entities;
using ComplejoDeportivo.Domain.Repositories;
using ComplejoDeportivo.Infrastructure.BaseDeDatos;
using Microsoft.EntityFrameworkCore;

namespace ComplejoDeportivo.Infrastructure.Repositorios
{
    public class SocioRepository : ISocioRepository
    {
        private readonly AppDbContext _context;

        public SocioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Socio?> ObtenerPorId(int id)
        {
            return await _context.Socios.FindAsync(id);
        }

        public async Task<Socio?> ObtenerPorEmail(string email)
        {
            var normalizado = email.Trim().ToLowerInvariant();
            return await _context.Socios.FirstOrDefaultAsync(s => s.Email == normalizado);
        }

        public async Task<(List<Socio> Items, int TotalRegistros)> ObtenerPaginado(int pagina, int tamanioPagina)
        {
            var query = _context.Socios.AsQueryable();

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.Id)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();

            return (items, total);
        }

        public async Task Guardar(Socio socio)
        {
            await _context.Socios.AddAsync(socio);
        }
    }
}