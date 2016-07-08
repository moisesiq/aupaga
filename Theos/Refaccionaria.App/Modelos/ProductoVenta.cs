namespace Refaccionaria.App
{
    public class ProductoVenta
    {
        public int ParteID { get; set; }
        public string NumeroDeParte { get; set; }
        public string NombreDeParte { get; set; }
        public string UnidadDeMedida { get; set; }
        public decimal[] Precios { get; set; }
        public decimal[] Existencias { get; set; }
        public decimal Costo { get; set; }
        public decimal CostoConDescuento { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Iva { get; set; }

        public string Descripcion
        {
            get
            {
                return (this.NumeroDeParte + "    " + this.NombreDeParte + "\n" +
                    this.Cantidad.ToString() + " PIEZA" + (this.Cantidad == 1 ? "" : "S") + "        P.U. " +
                    (this.PrecioUnitario + this.Iva).ToString(GlobalClass.FormatoMoneda));
            }
        }
        public decimal PrecioConIva { get { return (this.PrecioUnitario + this.Iva); } }
        public decimal Importe { get { return (this.PrecioConIva * this.Cantidad); } }

        public bool EsServicio { get; set; }
        public bool Es9500 { get; set; }
        public bool AGranel { get; set; }
    }
}
