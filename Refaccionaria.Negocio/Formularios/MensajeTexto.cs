using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class MensajeTexto : Form
    {
        public MensajeTexto()
        {
            InitializeComponent();
        }

        public MensajeTexto(string sTitulo, string sTexto)
        {
            InitializeComponent();
            
            this.Text = sTitulo;
            this.rtbTexto.Text = sTexto;
        }

        #region [ Eventos ]

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

        #endregion

        #region [ Públicos ]

        public void AgregarTexto(string sTexto)
        {
            this.rtbTexto.AppendText(sTexto + "\r\n");
        }

        #endregion
    }
}
