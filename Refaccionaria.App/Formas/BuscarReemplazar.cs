using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class BuscarReemplazar : Form
    {
        public BuscarReemplazar()
        {
            InitializeComponent();
        }

        public string Buscar { get; set; }
        public string Reemplazar { get; set; }

        private void btnReemplazar_Click(object sender, EventArgs e)
        {
            this.Buscar = this.txtBuscar.Text;
            this.Reemplazar = this.txtReemplazar.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
                
    }
}
