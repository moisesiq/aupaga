using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Refaccionaria.App
{
    public partial class CobroCobranza : Cobro
    {
        public CobroCobranza()
        {
            InitializeComponent();
            this.MostrarFacturar = false;
        }

        public override bool Validar()
        {
            Control lblEtSuma = (this.Controls["lblEtSuma"] as Label);
            Control lblRestante = (this.Controls["lblRestante"] as Label);
            Control lblEtRestante = (this.Controls["lblEtRestante"] as Label);

            if (!base.Validar())
            {
                if (lblRestante.Text != "0.00")
                    this.ctlError.QuitarError(lblEtRestante);
            }

            if (this.Suma <= 0)
                this.ctlError.PonerError(lblEtSuma, "La cantidad a pagar no puede ser cero.");

            return (this.ctlError.NumeroDeErrores == 0);
        }

        public Dictionary<int, decimal> GenerarFormasDePago()
        {
            var oFormasDePago = new Dictionary<int, decimal>();
            var oPagoDetalle = this.GenerarPagoDetalle();
            foreach (var oFormaDePago in oPagoDetalle)
                oFormasDePago.Add(oFormaDePago.TipoFormaPagoID, oFormaDePago.Importe);
            return oFormasDePago;
        }
    }
}
