using ComplejoDeportivo.Domain.Entities;

namespace ComplejoDeportivo.Domain.Strategies
{
    public interface IReglaPrecio
    {
        decimal AplicarAjuste(decimal montoActual, Reserva reserva, Socio socio, Espacio espacio);
    }

    public class ReglaAltaDemanda : IReglaPrecio
    {
        private const decimal RecargoPorcentaje = 0.20m; // +20% en alta demanda

        public decimal AplicarAjuste(decimal montoActual, Reserva reserva, Socio socio, Espacio espacio)
        {
            var dia = reserva.Fecha.DayOfWeek;
            bool esFinDeSemana = dia == DayOfWeek.Saturday || dia == DayOfWeek.Sunday;
            bool esSemanaDespues18 = !esFinDeSemana && reserva.HoraInicio >= new TimeSpan(18, 0, 0);

            return (esFinDeSemana || esSemanaDespues18)
                ? montoActual * (1 + RecargoPorcentaje)
                : montoActual;
        }
    }

    public class ReglaDescuentoCategoria : IReglaPrecio
    {
        public decimal AplicarAjuste(decimal montoActual, Reserva reserva, Socio socio, Espacio espacio)
        {
            var descuento = socio.CalcularDescuentoCategoria(montoActual);
            return montoActual - descuento;
        }
    }

    public class PromoMartesAJuevesTarde : IReglaPrecio
    {
        private const decimal DescuentoPorcentaje = 0.10m; // 10% descuento martes a jueves de 13:00 a 18:00

        public decimal AplicarAjuste(decimal montoActual, Reserva reserva, Socio socio, Espacio espacio)
        {
            var dia = reserva.Fecha.DayOfWeek;
            bool esMartesAJueves = dia is DayOfWeek.Tuesday or DayOfWeek.Wednesday or DayOfWeek.Thursday;
            bool esTarde = reserva.HoraInicio >= new TimeSpan(13, 0, 0) && reserva.HoraInicio < new TimeSpan(18, 0, 0);

            return (esMartesAJueves && esTarde)
                ? montoActual * (1 - DescuentoPorcentaje)
                : montoActual;
        }
    }

    public class PromoMasDeUnaFranja : IReglaPrecio
    {
        private const decimal DescuentoPorcentaje = 0.05m; // 5% descuento por 2 o más horas

        public decimal AplicarAjuste(decimal montoActual, Reserva reserva, Socio socio, Espacio espacio)
        {
            return reserva.DuracionHoras > 1
                ? montoActual * (1 - DescuentoPorcentaje)
                : montoActual;
        }
    }
}