using ComplejoDeportivo.Domain.Entities;
using ComplejoDeportivo.Domain.Repositories;
using ComplejoDeportivo.Infrastructure.BaseDeDatos;
using Microsoft.EntityFrameworkCore;

namespace ComplejoDeportivo.Infrastructure.Repositorios
{
    public class EspacioRepository : IEspacioRepository
    {
        private readonly AppDbContext _context;

        public EspacioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Espacio?> ObtenerPorId(int id)
        {
            return await _context.Espacios
                .Include(e => e.SubEspacios)
                .Include(e => e.EspacioPadre)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Espacio?> ObtenerConJerarquiaYReservas(int id, DateTime fecha)
        {
            var fechaSolo = fecha.Date;

            return await _context.Espacios
                .Include(e => e.Reservas.Where(r => r.Fecha == fechaSolo))
                .Include(e => e.EspacioPadre)
                    .ThenInclude(p => p!.Reservas.Where(r => r.Fecha == fechaSolo))
                .Include(e => e.SubEspacios)
                    .ThenInclude(s => s.Reservas.Where(r => r.Fecha == fechaSolo))
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<(List<Espacio> Items, int TotalRegistros)> ObtenerPaginado(int pagina, int tamanioPagina)
        {
            var query = _context.Espacios
                .Include(e => e.SubEspacios)
                .AsQueryable();

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.Id)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();

            return (items, total);
        }

        public async Task Guardar(Espacio espacio)
        {
            await _context.Espacios.AddAsync(espacio);
        }
    }
}