using ComplejoDeportivo.Domain.Entities;

namespace ComplejoDeportivo.Domain.Repositories
{
    public interface IReservaRepository
    {
        Task<Reserva?> ObtenerPorId(int id);
        Task<(List<Reserva> Items, int TotalRegistros)> ObtenerPorSocioPaginado(int socioId, int pagina, int tamanioPagina);
        Task<List<Reserva>> ObtenerPorEspacioYFecha(int espacioId, DateTime fecha);
        Task<List<Reserva>> ObtenerPorRangoFechas(DateTime desde, DateTime hasta);
        Task Guardar(Reserva reserva);
    }
}