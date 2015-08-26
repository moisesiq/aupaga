using System;
using System.Windows.Forms;

using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Efectivo : UserControl
    {
        public Efectivo(decimal mEfectivo)
        {
            InitializeComponent();

            this.txtEfectivo.Text = mEfectivo.ToString(GlobalClass.FormatoDecimal);
        }

        private void Efectivo_Load(object sender, EventArgs e)
        {
            this.ActiveControl = this.txtRecibido;

            // Se activan botones de Aceptar y Cancelar
            if (this.Parent is ContenedorControl)
            {
                (this.Parent as ContenedorControl).AcceptButton = this.btnAceptar;
                (this.Parent as ContenedorControl).CancelButton = this.btnCancelar;
            }
        }

        private void txtRecibido_Enter(object sender, EventArgs e)
        {
            this.txtRecibido.SelectAll();
        }

        private void txtRecibido_TextChanged(object sender, EventArgs e)
        {
            this.txtCambio.Text = Math.Round(Helper.ConvertirDecimal(this.txtRecibido.Text) - Helper.ConvertirDecimal(this.txtEfectivo.Text), 2)
                .ToString(GlobalClass.FormatoDecimal);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Se reporta el resultado
            if (this.Parent is ContenedorControl)
            {
                (this.Parent as ContenedorControl).DialogResult = DialogResult.OK;
            }
        
            this.Dispose();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
                
        
    }
}
