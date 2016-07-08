
namespace TheosProc
{
    public class modDetalleTraspaso
    {
        public int MovimientoInventarioDetalleID { get; set; }
        public int ParteID { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Recibido { get; set; }
    }

    public class modConteoInventario
    {
        public int InventarioConteoID { get; set; }
        public int InventarioLineaID { get; set; }
        public decimal Conteo { get; set; }
    }
}
