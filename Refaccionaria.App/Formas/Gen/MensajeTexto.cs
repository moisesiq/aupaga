using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class MensajeTexto : Form
    {
        public MensajeTexto()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;
        }

        public MensajeTexto(string sTitulo, string sTexto)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;
            
            this.Text = sTitulo;
            this.rtbTexto.Text = sTexto;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
