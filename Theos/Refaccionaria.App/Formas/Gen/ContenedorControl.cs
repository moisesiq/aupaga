using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Refaccionaria.App
{
    public partial class ContenedorControl : Form
    {
        const int AnchoAdicional = 6;
        const int AltoAdicional = 28;
                        
        public Dictionary<string, object> Sel = new Dictionary<string, object>();

        public ContenedorControl(string sTitulo, Control oControl)
        {
            this.Constructor(sTitulo, oControl, new Size(oControl.Width + ContenedorControl.AnchoAdicional, oControl.Height + ContenedorControl.AltoAdicional));
        }

        public ContenedorControl(string sTitulo, Control oControl, Size Tamanio)
        {
            this.Constructor(sTitulo, oControl, Tamanio);
        }

        private void Constructor(string sTitulo, Control oControl, Size Tamanio)
        {
            this.InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;

            this.Text = sTitulo;
            this.oControl = oControl;
            
            if (Tamanio != this.Size)
                this.Size = Tamanio;
            
            this.Controls.Add(this.oControl);
            this.oControl.Dock = DockStyle.Fill;

            this.oControl.Disposed += new EventHandler((s, e) =>
            {
                this.Close();
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
                this.Close();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public Control oControl { get; set; }
    }
}
