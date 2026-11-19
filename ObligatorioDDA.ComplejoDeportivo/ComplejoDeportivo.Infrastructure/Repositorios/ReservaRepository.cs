using ComplejoDeportivo.Domain.Entities;
using ComplejoDeportivo.Domain.Repositories;
using ComplejoDeportivo.Infrastructure.BaseDeDatos;
using Microsoft.EntityFrameworkCore;

namespace ComplejoDeportivo.Infrastructure.Repositorios
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Reserva?> ObtenerPorId(int id)
        {
            return await _context.Reservas
                .Include(r => r.Socio)
                .Include(r => r.Espacio)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<(List<Reserva> Items, int TotalRegistros)> ObtenerPorSocioPaginado(int socioId, int pagina, int tamanioPagina)
        {
            var query = _context.Reservas
                .Include(r => r.Espacio)
                .Where(r => r.SocioId == socioId);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.Fecha)
                .ThenBy(r => r.HoraInicio)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();

            return (items, total);
        }

        public async Task<List<Reserva>> ObtenerPorEspacioYFecha(int espacioId, DateTime fecha)
        {
            var fechaSolo = fecha.Date;
            return await _context.Reservas
                .Where(r => r.EspacioId == espacioId && r.Fecha == fechaSolo)
                .ToListAsync();
        }

        public async Task<List<Reserva>> ObtenerPorRangoFechas(DateTime desde, DateTime hasta)
        {
            var d = desde.Date;
            var h = hasta.Date;
            return await _context.Reservas
                .Include(r => r.Espacio)
                .Where(r => r.Fecha >= d && r.Fecha <= h)
                .ToListAsync();
        }

        public async Task Guardar(Reserva reserva)
        {
            await _context.Reservas.AddAsync(reserva);
        }
    }
}