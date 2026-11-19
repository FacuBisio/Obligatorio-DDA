using ComplejoDeportivo.Domain.Entities;

namespace ComplejoDeportivo.Domain.Repositories
{
    public interface ISocioRepository
    {
        Task<Socio?> ObtenerPorId(int id);
        Task<Socio?> ObtenerPorEmail(string email);
        Task<(List<Socio> Items, int TotalRegistros)> ObtenerPaginado(int pagina, int tamanioPagina);
        Task Guardar(Socio socio);
    }
}