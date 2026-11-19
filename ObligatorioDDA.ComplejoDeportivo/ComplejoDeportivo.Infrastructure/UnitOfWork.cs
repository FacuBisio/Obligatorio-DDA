using ComplejoDeportivo.Domain.Repositories;
using ComplejoDeportivo.Infrastructure.BaseDeDatos;
using ComplejoDeportivo.Infrastructure.Repositorios;

namespace ComplejoDeportivo.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IEspacioRepository? _espacioRepository;
        private ISocioRepository? _socioRepository;
        private IReservaRepository? _reservaRepository;
        private IUsuarioRepository? _usuarioRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IEspacioRepository EspacioRepository =>
            _espacioRepository ??= new EspacioRepository(_context);

        public ISocioRepository SocioRepository =>
            _socioRepository ??= new SocioRepository(_context);

        public IReservaRepository ReservaRepository =>
            _reservaRepository ??= new ReservaRepository(_context);

        
        public IUsuarioRepository UsuarioRepository =>
            _usuarioRepository ??= new UsuarioRepository(_context);

        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}