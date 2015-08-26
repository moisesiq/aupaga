using System;
using System.Windows.Forms;

using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class GastoCajaAPoliza : Form
    {
        ControlError ctlError = new ControlError();

        public GastoCajaAPoliza(decimal mImporte)
        {
            InitializeComponent();

            this.txtImporte.Text = mImporte.ToString();
            this.txtSubtotal.Text = Math.Round(UtilLocal.ObtenerPrecioSinIva(mImporte), 2).ToString();
            this.txtIva.Text = Math.Round(UtilLocal.ObtenerIvaDePrecio(mImporte), 2).ToString();
        }

        #region [ Propiedades ]

        public bool Facturado { get { return this.rdbFactura.Checked; } }
        public string Folio { get { return this.txtFolio.Text; } }
        public DateTime Fecha { get { return this.dtpFecha.Value; } }
        public decimal Subtotal { get { return Helper.ConvertirDecimal(this.txtSubtotal.Text); } }
        public decimal Iva { get { return Helper.ConvertirDecimal(this.txtIva.Text); } }

        #endregion

        #region [ Eventos ]

        private void rdbFactura_CheckedChanged(object sender, EventArgs e)
        {
            this.gpbFactura.Enabled = this.rdbFactura.Checked;
        }

        private void rdbNota_CheckedChanged(object sender, EventArgs e)
        {
            this.gpbFactura.Enabled = this.rdbFactura.Checked;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!this.Validar())
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        #endregion
        
        #region [ Métodos ]

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if ((this.Subtotal + this.Iva) != Helper.ConvertirDecimal(this.txtImporte.Text))
                this.ctlError.PonerError(this.txtIva, "Los importes especificados no coinciden con el importe total.", ErrorIconAlignment.MiddleLeft);
            return this.ctlError.Valido;
        }

        #endregion

    }
}
