using System;
using System.Windows.Forms;

using LibUtil;

namespace Refaccionaria.App
{
    public partial class Efectivo : Form
    {
        public Efectivo(decimal mEfectivo)
        {
            InitializeComponent();

            this.txtEfectivo.Text = mEfectivo.ToString(GlobalClass.FormatoDecimal);
        }

        #region [ Propiedades ]

        public decimal Recibido { get; set; }

        #endregion

        #region [ Eventos ]

        private void Efectivo_Load(object sender, EventArgs e)
        {
            this.ActiveControl = this.txtRecibido;
        }

        private void txtRecibido_Enter(object sender, EventArgs e)
        {
            this.txtRecibido.SelectAll();
        }

        private void txtRecibido_TextChanged(object sender, EventArgs e)
        {
            this.txtCambio.Text = Math.Round(Util.Decimal(this.txtRecibido.Text) - Util.Decimal(this.txtEfectivo.Text), 2)
                .ToString(GlobalClass.FormatoDecimal);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Recibido = Util.Decimal(this.txtRecibido.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
