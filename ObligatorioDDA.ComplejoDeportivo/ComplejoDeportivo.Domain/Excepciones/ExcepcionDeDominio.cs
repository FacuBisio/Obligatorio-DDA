namespace ComplejoDeportivo.Domain.Excepciones
{
    public class ExcepcionDeDominio : Exception
    {
        public ExcepcionDeDominio(string mensaje) : base(mensaje) { }
    }

    public class ExcepcionEntidadNoEncontrada : ExcepcionDeDominio
    {
        public ExcepcionEntidadNoEncontrada(string nombreEntidad, int id)
            : base($"No se encontró {nombreEntidad} con id '{id}'.") { }
    }
}