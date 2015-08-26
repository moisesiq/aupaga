using System.Collections.Generic;

namespace Refaccionaria.App
{
    public class ConteoCaja : ConteoMonedas
    {
        public List<ConteoComprobante> Comprobantes { get; set; }

        public ConteoCaja()
        {
            this.Comprobantes = new List<ConteoComprobante>();
        }

        #region [ Propiedades calculadas ]

        public decimal TotalConteoComprobantes
        {
            get
            {
                decimal mTotal = 0;
                foreach (var oComprobante in this.Comprobantes)
                    mTotal += oComprobante.Importe;
                return mTotal;
            }
        }

        public decimal TotalConteo { get { return (this.TotalConteoMonedas + this.TotalConteoComprobantes); } }

        #endregion
    }

    public class ConteoComprobante
    {
        public string Tipo { get; set; }
        public decimal Importe { get; set; }
        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public string Folio { get; set; }
    }
}
