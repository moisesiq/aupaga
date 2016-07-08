using System;

namespace Refaccionaria.App
{
    public class ContaModelos
    {
        public class IndiceCuentasContables
        {
            public string Cuenta { get; set; }
            public int Nivel { get; set; }
            
            public int? IndiceCuenta { get; set; }
            public int? IndiceSubcuenta { get; set; }
            public int? IndiceCuentaDeMayor { get; set; }
            public int? IndiceCuentaAuxiliar { get; set; }
        }
    }
}
