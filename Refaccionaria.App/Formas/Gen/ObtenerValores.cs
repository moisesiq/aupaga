using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class ObtenerValores : Form
    {
        public ObtenerValores(string sEtiqueta1, string sEtiqueta2)
        {
            InitializeComponent();

            this.lblValor1.Text = sEtiqueta1;
            this.lblValor2.Text = sEtiqueta2;
        }

        public string Valor1 { get; set; }
        public string Valor2 { get; set; }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Valor1 = this.txtValor1.Text;
            this.Valor2 = this.txtValor2.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
