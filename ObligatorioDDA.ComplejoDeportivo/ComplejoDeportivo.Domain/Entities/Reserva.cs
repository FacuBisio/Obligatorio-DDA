using ComplejoDeportivo.Domain.Enums;
using ComplejoDeportivo.Domain.Excepciones;

namespace ComplejoDeportivo.Domain.Entities
{
    public class Reserva
    {
        private const int MinutosVencimientoPago = 30;
        private const int HorasAnticipacionMinima = 2;
        private const int HorasPenalizacionCancelacion = 24;

        public int Id { get; private set; }
        public DateTime Fecha { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public int DuracionHoras { get; private set; }
        public decimal MontoTotal { get; private set; }
        public decimal MontoReintegrado { get; private set; }
        public EstadoReserva Estado { get; private set; }
        public DateTime FechaCreacionUtc { get; private set; }

        public int SocioId { get; private set; }
        public Socio Socio { get; private set; } = null!;

        public int EspacioId { get; private set; }
        public Espacio Espacio { get; private set; } = null!;

        private Reserva() { }

        public Reserva(Socio socio, Espacio espacio, DateTime fecha, TimeSpan horaInicio, int duracionHoras, DateTime fechaActualUtc)
        {
            if (socio is null)
                throw new ExcepcionDeDominio("El socio es obligatorio.");
            if (!socio.PuedeReservar())
                throw new ExcepcionDeDominio("El socio se encuentra moroso y no puede realizar reservas.");
            if (espacio is null)
                throw new ExcepcionDeDominio("El espacio es obligatorio.");
            if (duracionHoras < 1 || duracionHoras > 3)
                throw new ExcepcionDeDominio("La reserva debe durar entre 1 y 3 horas.");

            var inicioReservaLocal = fecha.Date.Add(horaInicio);
            var ahoraLocal = ConvertirAMontevideo(fechaActualUtc);

            if (inicioReservaLocal < ahoraLocal.AddHours(HorasAnticipacionMinima))
                throw new ExcepcionDeDominio("La reserva debe realizarse con al menos 2 horas de anticipación.");

            if (!espacio.EstaDisponible(fecha, horaInicio, duracionHoras, fechaActualUtc))
                throw new ExcepcionDeDominio("El espacio no está disponible en la fecha y franja horaria solicitadas.");

            Socio = socio;
            SocioId = socio.Id;
            Espacio = espacio;
            EspacioId = espacio.Id;
            Fecha = fecha.Date;
            HoraInicio = horaInicio;
            DuracionHoras = duracionHoras;
            Estado = EstadoReserva.PendientePago;
            FechaCreacionUtc = fechaActualUtc;
            MontoTotal = 0m;
            MontoReintegrado = 0m;
        }

        public void AsignarMontoTotal(decimal montoTotal)
        {
            if (montoTotal < 0)
                throw new ExcepcionDeDominio("El monto total no puede ser negativo.");

            MontoTotal = Math.Round(montoTotal, 2, MidpointRounding.AwayFromZero);
        }

        public bool EstaVencida(DateTime fechaActualUtc)
        {
            return Estado == EstadoReserva.PendientePago &&
                   fechaActualUtc > FechaCreacionUtc.AddMinutes(MinutosVencimientoPago);
        }

        public void MarcarComoVencida(DateTime fechaActualUtc)
        {
            if (Estado != EstadoReserva.PendientePago)
                throw new ExcepcionDeDominio("Solo se puede marcar como vencida una reserva pendiente de pago.");
            if (!EstaVencida(fechaActualUtc))
                throw new ExcepcionDeDominio("La reserva aún no superó los 30 minutos para considerarse vencida.");

            Estado = EstadoReserva.Vencida;
        }

        public void ConfirmarPago(DateTime fechaActualUtc)
        {
            if (Estado != EstadoReserva.PendientePago)
                throw new ExcepcionDeDominio($"No se puede confirmar el pago de una reserva en estado '{Estado}'.");

            if (EstaVencida(fechaActualUtc))
            {
                Estado = EstadoReserva.Vencida;
                throw new ExcepcionDeDominio("El plazo de 30 minutos para confirmar el pago ha vencido.");
            }

            Estado = EstadoReserva.Confirmada;
        }

        public decimal Cancelar(decimal porcentajeReintegroMenos24Hs, DateTime fechaActualUtc)
        {
            if (Estado == EstadoReserva.Cancelada)
                throw new ExcepcionDeDominio("La reserva ya se encuentra cancelada.");
            if (Estado == EstadoReserva.Finalizada || Estado == EstadoReserva.Vencida)
                throw new ExcepcionDeDominio($"No se puede cancelar una reserva en estado '{Estado}'.");

            if (Estado == EstadoReserva.PendientePago)
            {
                Estado = EstadoReserva.Cancelada;
                MontoReintegrado = 0m;
                return MontoReintegrado;
            }

            var inicioReservaLocal = Fecha.Date.Add(HoraInicio);
            var ahoraLocal = ConvertirAMontevideo(fechaActualUtc);
            var horasAnticipacion = (inicioReservaLocal - ahoraLocal).TotalHours;

            if (horasAnticipacion >= HorasPenalizacionCancelacion)
            {
                MontoReintegrado = MontoTotal;
            }
            else
            {
                if (porcentajeReintegroMenos24Hs < 0 || porcentajeReintegroMenos24Hs > 100)
                    throw new ExcepcionDeDominio("El porcentaje de reintegro debe estar entre 0 y 100.");

                MontoReintegrado = Math.Round(MontoTotal * (porcentajeReintegroMenos24Hs / 100m), 2, MidpointRounding.AwayFromZero);
            }

            Estado = EstadoReserva.Cancelada;
            return MontoReintegrado;
        }

        public void Finalizar(DateTime fechaActualUtc)
        {
            if (Estado != EstadoReserva.Confirmada)
                throw new ExcepcionDeDominio("Solo se puede finalizar una reserva confirmada.");

            var finReservaLocal = Fecha.Date.Add(HoraInicio).AddHours(DuracionHoras);
            var ahoraLocal = ConvertirAMontevideo(fechaActualUtc);

            if (ahoraLocal < finReservaLocal)
                throw new ExcepcionDeDominio("No se puede finalizar una reserva cuyo horario aún no transcurrió.");

            Estado = EstadoReserva.Finalizada;
        }

        public bool EstaSuperpuesta(DateTime fecha, TimeSpan horaInicio, int duracionHoras, DateTime fechaActualUtc)
        {
            if (Estado == EstadoReserva.Cancelada || Estado == EstadoReserva.Vencida || Estado == EstadoReserva.Finalizada)
                return false;

            if (EstaVencida(fechaActualUtc))
                return false;

            if (Fecha.Date != fecha.Date)
                return false;

            var finExistente = HoraInicio.Add(TimeSpan.FromHours(DuracionHoras));
            var finNueva = horaInicio.Add(TimeSpan.FromHours(duracionHoras));

            return HoraInicio < finNueva && horaInicio < finExistente;
        }

        private static DateTime ConvertirAMontevideo(DateTime utc)
        {
            var fechaUtc = utc.Kind == DateTimeKind.Utc ? utc : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Montevideo");
                return TimeZoneInfo.ConvertTimeFromUtc(fechaUtc, tz);
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback estándar en Windows si no encuentra el ID IANA
                var tzWindows = TimeZoneInfo.FindSystemTimeZoneById("Montevideo Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(fechaUtc, tzWindows);
            }
        }
    }
}