using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class ListadoSimpleBotones : ListadoSimple
    {
        public ListadoSimpleBotones()
        {
            InitializeComponent();
        }
                                
        protected virtual void btnNuevo_Click(object sender, EventArgs e)
        {

        }

        protected virtual void btnModificar_Click(object sender, EventArgs e)
        {

        }

        protected virtual void btnEliminar_Click(object sender, EventArgs e)
        {

        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.btnModificar_Click(sender, e);
        }

        protected override void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.btnModificar_Click(sender, e);
        }
                
    }
}
