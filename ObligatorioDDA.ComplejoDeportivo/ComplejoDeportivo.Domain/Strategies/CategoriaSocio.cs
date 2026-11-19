using ComplejoDeportivo.Domain.Enums;
using ComplejoDeportivo.Domain.Excepciones;

namespace ComplejoDeportivo.Domain.Strategies
{
    public interface CategoriaSocio
    {
        decimal CalcularDescuento(decimal montoBase);
    }

    public class SocioRegular : CategoriaSocio
    {
        public decimal CalcularDescuento(decimal montoBase) => 0m;
    }

    public class SocioPremium : CategoriaSocio
    {
        private const decimal PorcentajeDescuento = 0.15m; // 15% de descuento (ajustable)
        public decimal CalcularDescuento(decimal montoBase) => montoBase * PorcentajeDescuento;
    }

    public static class FabricaCategoriaSocio
    {
        public static CategoriaSocio Crear(TipoCategoriaSocio tipo) => tipo switch
        {
            TipoCategoriaSocio.Regular => new SocioRegular(),
            TipoCategoriaSocio.Premium => new SocioPremium(),
            _ => throw new ExcepcionDeDominio($"Categoría de socio no soportada: {tipo}.")
        };
    }
}