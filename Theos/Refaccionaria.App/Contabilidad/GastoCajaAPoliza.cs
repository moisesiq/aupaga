using System;
using System.Windows.Forms;

using LibUtil;
using TheosProc;

namespace Refaccionaria.App
{
    public partial class GastoCajaAPoliza : Form
    {
        ControlError ctlError = new ControlError();

        public GastoCajaAPoliza(decimal mImporte)
        {
            InitializeComponent();

            this.txtImporte.Text = mImporte.ToString();
            this.txtSubtotal.Text = UtilTheos.ObtenerPrecioSinIva(mImporte).ToString();
            this.txtIva.Text = UtilTheos.ObtenerIvaDePrecio(mImporte).ToString();
        }

        #region [ Propiedades ]

        public bool Facturado { get { return this.rdbFactura.Checked; } }
        public string Folio { get { return this.txtFolio.Text; } }
        public DateTime Fecha { get { return this.dtpFecha.Value; } }
        public decimal Subtotal { get { return Util.Decimal(this.txtSubtotal.Text); } }
        public decimal Iva { get { return Util.Decimal(this.txtIva.Text); } }

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
            if ((this.Subtotal + this.Iva) != Util.Decimal(this.txtImporte.Text))
                this.ctlError.PonerError(this.txtIva, "Los importes especificados no coinciden con el importe total.", ErrorIconAlignment.MiddleLeft);
            return this.ctlError.Valido;
        }

        #endregion

    }
}
