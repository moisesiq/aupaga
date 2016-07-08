using System;
using System.Windows.Forms;

using LibUtil;

namespace Refaccionaria.App
{
    public partial class CajaMonedas : UserControl
    {
        ControlError ctlError = new ControlError();

        public CajaMonedas()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public bool MostrarTotal
        {
            get { return this.lblTotal.Visible; }
            set
            {
                this.lblEtTotal.Visible = value;
                this.lblTotal.Visible = value;
            }
        }

        public decimal MonedasImporteTotal
        {
            get
            {
                return (
                    (1000 * this.nudMoneda1000.Value)
                    + (500 * this.nudMoneda500.Value)
                    + (200 * this.nudMoneda200.Value)
                    + (100 * this.nudMoneda100.Value)
                    + (50 * this.nudMoneda50.Value)
                    + (20 * this.nudMoneda20.Value)
                    + (10 * this.nudMoneda10.Value)
                    + (5 * this.nudMoneda5.Value)
                    + (2 * this.nudMoneda2.Value)
                    + (1 * this.nudMoneda1.Value)
                    + (0.5M * this.nudMoneda0_5.Value)
                    + (0.2M * this.nudMoneda0_2.Value)
                    + (0.1M * this.nudMoneda0_1.Value)
                );
            }
        }

        #endregion

        #region [ Eventos ]

        private void nudMoneda1000_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void nudMoneda500_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda200_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda100_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda50_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda20_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda10_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void nudMoneda5_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void nudMoneda2_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda1_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda0_5_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda0_2_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        private void numMoneda0_1_ValueChanged(object sender, System.EventArgs e)
        {
            this.MonedasCambio();
        }

        #endregion

        #region [ Métodos Virtual ]

        protected virtual void MonedasCambio()
        {
            this.lblTotal.Text = this.MonedasImporteTotal.ToString(GlobalClass.FormatoDecimal);
        }

        #endregion

        #region [ Públicos ]

        public virtual void LimpiarMonedas()
        {
            foreach (Control oControl in this.pnlMonedas.Controls)
                if (oControl is NumericUpDown)
                    (oControl as NumericUpDown).Value = 0;
        }

        public virtual ConteoCaja GenerarConteo()
        {
            var oConteo = new ConteoCaja();
            oConteo.Monedas1000 = (int)this.nudMoneda1000.Value;
            oConteo.Monedas500 = (int)this.nudMoneda500.Value;
            oConteo.Monedas200 = (int)this.nudMoneda200.Value;
            oConteo.Monedas100 = (int)this.nudMoneda100.Value;
            oConteo.Monedas50 = (int)this.nudMoneda50.Value;
            oConteo.Monedas20 = (int)this.nudMoneda20.Value;
            oConteo.Monedas10 = (int)this.nudMoneda10.Value;
            oConteo.Monedas5 = (int)this.nudMoneda5.Value;
            oConteo.Monedas2 = (int)this.nudMoneda2.Value;
            oConteo.Monedas1 = (int)this.nudMoneda1.Value;
            oConteo.Monedas05 = (int)this.nudMoneda0_5.Value;
            oConteo.Monedas02 = (int)this.nudMoneda0_2.Value;
            oConteo.Monedas01 = (int)this.nudMoneda0_1.Value;

            return oConteo;
        }

        public virtual bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (this.MonedasImporteTotal == 0)
                this.ctlError.PonerError(this.lblEtTotal, "No se ha especificado ningún importe.", ErrorIconAlignment.MiddleLeft);
            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
    }
}
