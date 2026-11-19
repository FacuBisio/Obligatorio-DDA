namespace ComplejoDeportivo.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEspacioRepository EspacioRepository { get; }
        ISocioRepository SocioRepository { get; }
        IReservaRepository ReservaRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        Task<int> Commit();
    }
}