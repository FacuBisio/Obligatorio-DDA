using ComplejoDeportivo.Domain.Entities;
using ComplejoDeportivo.Domain.Excepciones;
using ComplejoDeportivo.Domain.Strategies;

namespace ComplejoDeportivo.DomainServices
{
    public class CalculadoraPrecioReserva
    {
        private readonly List<IReglaPrecio> _reglas;

        public IReadOnlyCollection<IReglaPrecio> Reglas => _reglas;

        public CalculadoraPrecioReserva(IEnumerable<IReglaPrecio>? reglas = null)
        {
            _reglas = reglas?.ToList() ?? new List<IReglaPrecio>();
        }

        public void AgregarRegla(IReglaPrecio regla)
        {
            if (regla is null)
                throw new ExcepcionDeDominio("La regla de precio no puede ser nula.");

            _reglas.Add(regla);
        }

        public void QuitarRegla(IReglaPrecio regla)
        {
            _reglas.Remove(regla);
        }

        public decimal CalcularMontoFinal(Reserva reserva, Socio socio, Espacio espacio)
        {
            if (reserva is null || socio is null || espacio is null)
                throw new ExcepcionDeDominio("Reserva, socio y espacio son obligatorios para calcular el precio.");

            decimal monto = espacio.CalcularTarifaBase(reserva.DuracionHoras);

            foreach (var regla in _reglas)
            {
                monto = regla.AplicarAjuste(monto, reserva, socio, espacio);
            }

            if (monto < 0) monto = 0m;

            return Math.Round(monto, 2, MidpointRounding.AwayFromZero);
        }
    }
}