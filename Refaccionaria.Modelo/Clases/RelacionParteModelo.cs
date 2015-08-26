using System;
using System.Collections.Generic;

namespace Refaccionaria.Modelo
{
    public class RelacionParteModelo
    {
        public int ParteID { get; set; }
        public int TipoFuenteID { get; set; }
        public int ModeloID { get; set; }
        public List<int> MotorIDs { get; set; }
        public List<int> Anios { get; set; }
    }
}
