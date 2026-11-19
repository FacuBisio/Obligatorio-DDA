using ComplejoDeportivo.Domain.Entities;

namespace ComplejoDeportivo.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorId(int id);
        Task<Usuario?> ObtenerPorEmail(string email);
        Task<bool> ExisteEmail(string email);
        Task Guardar(Usuario usuario);
    }
}