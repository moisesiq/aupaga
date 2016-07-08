using System;

namespace Refaccionaria.App
{
    public class ProductosComprados
    {
        public int ParteID { get; set; }
        public string NumeroParte { get; set; }
        public string Linea { get; set; }
        public string Marca { get; set; }
        public string Descripcion { get; set; }
        public decimal UNS { get; set; }
        public decimal Importe { get; set; }        
    }
}
