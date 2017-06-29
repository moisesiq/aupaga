using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class Validaciones : UserControl
    {
        // Para el Singleton *
        private static Validaciones instance;
        public static Validaciones Instance
        {
            get
            {
                if (Validaciones.instance == null || Validaciones.instance.IsDisposed)
                    Validaciones.instance = new Validaciones();
                return Validaciones.instance;
            }
        }
        //

        public Validaciones()
        {
            InitializeComponent();
            this.tbpAutorizaciones.Controls.Add(new Autorizaciones() { Dock = DockStyle.Fill });
        }

        private void tabValidaciones_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabValidaciones.SelectedTab.Name)
            {
                case "tbpAutorizaciones":
                    if (this.tbpAutorizaciones.Controls.Count <= 0)
                        this.tbpAutorizaciones.Controls.Add(new Autorizaciones() { Dock = DockStyle.Fill });
                    break;
                case "tbpCortes":
                    if (this.tbpCortes.Controls.Count <= 0)
                        this.tbpCortes.Controls.Add(new ValCortes() { Dock = DockStyle.Fill });
                    break;
            }
        }


    }
}
