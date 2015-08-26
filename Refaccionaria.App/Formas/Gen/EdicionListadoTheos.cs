using System;
using System.Windows.Forms;
using System.Drawing;

using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class EdicionListadoTheos : EdicionListado
    {
        public EdicionListadoTheos()
        {
            InitializeComponent();
        }

        public EdicionListadoTheos(object oDatos)
        {
            InitializeComponent();

            base.CargarDatos(oDatos);
        }

        private void EdicionListadoTheos_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria;
            this.ShowIcon = true;
            this.dgvListado.BorderStyle = BorderStyle.None;
            this.dgvListado.BackgroundColor = Color.FromArgb(67, 87, 123);
        }
    }
}
