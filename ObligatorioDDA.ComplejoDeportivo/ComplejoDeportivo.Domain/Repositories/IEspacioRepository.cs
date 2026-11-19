using ComplejoDeportivo.Domain.Entities;

namespace ComplejoDeportivo.Domain.Repositories
{
    public interface IEspacioRepository
    {
        Task<Espacio?> ObtenerPorId(int id);
        Task<Espacio?> ObtenerConJerarquiaYReservas(int id, DateTime fecha);
        Task<(List<Espacio> Items, int TotalRegistros)> ObtenerPaginado(int pagina, int tamanioPagina);
        Task Guardar(Espacio espacio);
    }
}