using System;
using System.Collections.Generic;

namespace Refaccionaria.App
{
    public class Etiquetas
    {
        public int MovimientoInventarioEtiquetaID { get; set; }
        public int MovimientoInventarioID { get; set; }
        public int ParteID { get; set; }
        public string NumeroParte { get; set; }
        public string NombreParte { get; set; }
        public string CodigoBarra { get; set; }
        public int NumeroEtiquetas { get; set; }
    }
}
