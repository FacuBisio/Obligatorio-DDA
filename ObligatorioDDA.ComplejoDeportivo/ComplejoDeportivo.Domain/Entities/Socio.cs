using ComplejoDeportivo.Domain.Enums;
using ComplejoDeportivo.Domain.Excepciones;
using ComplejoDeportivo.Domain.Strategies;

namespace ComplejoDeportivo.Domain.Entities
{
    public class Socio
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public bool EsMoroso { get; private set; }
        public TipoCategoriaSocio TipoCategoria { get; private set; }
        public CategoriaSocio Categoria { get; private set; } = null!;

        private Socio() { }

        public Socio(string nombre, string email, TipoCategoriaSocio tipoCategoria = TipoCategoriaSocio.Regular)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDeDominio("El nombre del socio es obligatorio.");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ExcepcionDeDominio("El email del socio no es válido.");

            Nombre = nombre.Trim();
            Email = email.Trim().ToLowerInvariant();
            EsMoroso = false;
            CambiarCategoria(tipoCategoria);
        }

        public void MarcarMoroso() => EsMoroso = true;

        public void LiberarMoroso() => EsMoroso = false;

        public bool PuedeReservar() => !EsMoroso;

        public void CambiarCategoria(TipoCategoriaSocio nuevaCategoria)
        {
            Categoria = FabricaCategoriaSocio.Crear(nuevaCategoria);
            TipoCategoria = nuevaCategoria;
        }

        public decimal CalcularDescuentoCategoria(decimal montoBase)
        {
            Categoria ??= FabricaCategoriaSocio.Crear(TipoCategoria);
            return Categoria.CalcularDescuento(montoBase);
        }
    }
}