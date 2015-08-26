using System;
using System.Drawing;
using System.Windows.Forms;

namespace Refaccionaria.Negocio
{
    // Intento fallido de hacer un ComboBox con etiqueta :( (tal vez después se intente nuevamente)
    public class ComboMod : ComboBox
    {
        const int AnchoFlecha = 22;

        public ComboMod()
        {
            this.InitializeComponent();
        }

        private Label lblEtiqueta;
        private void InitializeComponent()
        {
            this.lblEtiqueta = new Label();

            this.SuspendLayout();

            // lblEtiqueta
            // 
            this.lblEtiqueta.AutoSize = false;
            this.lblEtiqueta.BackColor = SystemColors.Control;
            this.lblEtiqueta.ForeColor = System.Drawing.Color.Gray;
            this.lblEtiqueta.Location = new System.Drawing.Point(3, 3);
            this.lblEtiqueta.Size = new Size(this.Width - ComboMod.AnchoFlecha, this.Height - 6);
            this.lblEtiqueta.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
            this.lblEtiqueta.Name = "lblEtiqueta";
            this.lblEtiqueta.Text = "";
            this.lblEtiqueta.Click += new EventHandler((object sender, EventArgs e) =>
            {
                this.Focus();
                this.OnClick(new EventArgs());
            });

            // 
            // Combo
            // 
            this.FontChanged += new EventHandler(ComboMod_FontChanged);
            this.Enter += new EventHandler(ComboMod_Enter);
            this.Leave += new EventHandler(ComboMod_Leave);
            this.Controls.Add(this.lblEtiqueta);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #region [ Propiedades ]

        public string Etiqueta
        {
            get { return this.lblEtiqueta.Text; }
            set { this.lblEtiqueta.Text = value; }
        }

        public Color EtiquetaColor
        {
            get { return this.lblEtiqueta.ForeColor; }
            set { this.lblEtiqueta.ForeColor = value; }
        }

        #endregion

        #region [ Eventos ]

        void ComboMod_FontChanged(object sender, EventArgs e)
        {
            this.lblEtiqueta.Font = this.Font;
        }

        private void ComboMod_Enter(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = false;
        }

        private void ComboMod_Leave(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = (this.Text == "");
        }

        #endregion
    }
}
