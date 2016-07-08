using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class ProveedorNotaDeCreditoForma : Form
    {
        public ProveedorNotaDeCreditoForma()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public string Folio { get { return this.txtFolio.Text; } }
        public DateTime Fecha { get { return this.dtpFecha.Value; } }
        public decimal Subtotal { get { return Util.Decimal(this.txtSubtotal.Text); } }
        public decimal Iva { get { return Util.Decimal(this.txtIva.Text); } }
        public decimal Total { get { return Util.Decimal(this.txtTotal.Text); } }
        public string Observacion { get { return this.txtObservacion.Text; } }

        #endregion

        #region [ Eventos ]

        private void txtSubtotal_TextChanged(object sender, EventArgs e)
        {
            this.txtTotal.Text = (Util.Decimal(this.txtSubtotal.Text) + Util.Decimal(this.txtIva.Text)).ToString(GlobalClass.FormatoMoneda);
        }

        private void txtIva_TextChanged(object sender, EventArgs e)
        {
            this.txtTotal.Text = (Util.Decimal(this.txtSubtotal.Text) + Util.Decimal(this.txtIva.Text)).ToString(GlobalClass.FormatoMoneda);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
        
        #region [ Métodos ]



        #endregion
    }
}
