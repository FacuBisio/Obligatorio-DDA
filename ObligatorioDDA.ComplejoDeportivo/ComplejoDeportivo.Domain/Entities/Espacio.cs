using ComplejoDeportivo.Domain.Excepciones;

namespace ComplejoDeportivo.Domain.Entities
{
    public class Espacio
    {
        private readonly List<Espacio> _subEspacios = new();
        private readonly List<Reserva> _reservas = new();

        public int Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public int Capacidad { get; private set; }
        public bool FueraDeServicio { get; private set; }
        public decimal PrecioHoraBase { get; private set; }

        public int? EspacioPadreId { get; private set; }
        public Espacio? EspacioPadre { get; private set; }

        public IReadOnlyCollection<Espacio> SubEspacios => _subEspacios;
        public IReadOnlyCollection<Reserva> Reservas => _reservas;

        private Espacio() { }

        public Espacio(string nombre, int capacidad, decimal precioHoraBase)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDeDominio("El nombre del espacio es obligatorio.");
            if (capacidad <= 0)
                throw new ExcepcionDeDominio("La capacidad debe ser mayor que cero.");
            if (precioHoraBase <= 0)
                throw new ExcepcionDeDominio("El precio por hora base debe ser mayor que cero.");

            Nombre = nombre.Trim();
            Capacidad = capacidad;
            PrecioHoraBase = precioHoraBase;
            FueraDeServicio = false;
        }

        public void PonerFueraDeServicio() => FueraDeServicio = true;

        public void Habilitar() => FueraDeServicio = false;

        public decimal CalcularTarifaBase(int duracionHoras)
        {
            if (duracionHoras <= 0)
                throw new ExcepcionDeDominio("La duración en horas debe ser mayor que cero.");

            return PrecioHoraBase * duracionHoras;
        }

        public bool EsDividido() => _subEspacios.Any();

        public void AgregarSubEspacio(Espacio subEspacio)
        {
            if (subEspacio is null)
                throw new ExcepcionDeDominio("El sub-espacio no puede ser nulo.");
            if (ReferenceEquals(this, subEspacio) || (Id != 0 && Id == subEspacio.Id))
                throw new ExcepcionDeDominio("Un espacio no puede contenerse a sí mismo.");
            if (EsAncestro(subEspacio))
                throw new ExcepcionDeDominio("No se puede agregar un espacio padre como sub-espacio (ciclo detectado).");
            if (_subEspacios.Any(s => s == subEspacio || (s.Id != 0 && s.Id == subEspacio.Id)))
                throw new ExcepcionDeDominio("El sub-espacio ya pertenece a este espacio.");

            subEspacio.AsignarPadre(this);
            _subEspacios.Add(subEspacio);
        }

        public void QuitarSubEspacio(Espacio subEspacio)
        {
            if (!_subEspacios.Remove(subEspacio))
                throw new ExcepcionDeDominio("El espacio indicado no es un sub-espacio de este espacio.");

            subEspacio.QuitarPadre();
        }

        public bool EstaDisponible(DateTime fecha, TimeSpan horaInicio, int duracionHoras, DateTime fechaActualUtc)
        {
            if (EstaFueraDeServicioRecursivo())
                return false;

            if (_reservas.Any(r => r.EstaSuperpuesta(fecha, horaInicio, duracionHoras, fechaActualUtc)))
                return false;

            if (TieneAncestroReservado(fecha, horaInicio, duracionHoras, fechaActualUtc))
                return false;

            if (TieneDescendienteReservado(fecha, horaInicio, duracionHoras, fechaActualUtc))
                return false;

            return true;
        }

        private void AsignarPadre(Espacio padre)
        {
            EspacioPadre = padre;
            EspacioPadreId = padre.Id != 0 ? padre.Id : null;
        }

        private void QuitarPadre()
        {
            EspacioPadre = null;
            EspacioPadreId = null;
        }

        private bool EsAncestro(Espacio candidato)
        {
            var actual = EspacioPadre;
            while (actual != null)
            {
                if (ReferenceEquals(actual, candidato) || (actual.Id != 0 && actual.Id == candidato.Id))
                    return true;
                actual = actual.EspacioPadre;
            }
            return false;
        }

        private bool EstaFueraDeServicioRecursivo()
        {
            if (FueraDeServicio) return true;
            return EspacioPadre != null && EspacioPadre.EstaFueraDeServicioRecursivo();
        }

        private bool TieneAncestroReservado(DateTime fecha, TimeSpan horaInicio, int duracionHoras, DateTime fechaActualUtc)
        {
            var actual = EspacioPadre;
            while (actual != null)
            {
                if (actual.Reservas.Any(r => r.EstaSuperpuesta(fecha, horaInicio, duracionHoras, fechaActualUtc)))
                    return true;
                actual = actual.EspacioPadre;
            }
            return false;
        }

        private bool TieneDescendienteReservado(DateTime fecha, TimeSpan horaInicio, int duracionHoras, DateTime fechaActualUtc)
        {
            foreach (var hijo in _subEspacios)
            {
                if (hijo.FueraDeServicio) continue;
                if (hijo.Reservas.Any(r => r.EstaSuperpuesta(fecha, horaInicio, duracionHoras, fechaActualUtc)))
                    return true;
                if (hijo.TieneDescendienteReservado(fecha, horaInicio, duracionHoras, fechaActualUtc))
                    return true;
            }
            return false;
        }
    }
}