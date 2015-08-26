using System;
using System.Drawing;
using System.Windows.Forms;

namespace Refaccionaria.Negocio
{
    public class TextoMod : TextBox
    {
        public TextoMod()
        {
            this.InitializeComponent();
        }

        private Label lblEtiqueta;
        private void InitializeComponent()
        {
            this.lblEtiqueta = new Label();

            // lblEtiqueta
            // 
            this.lblEtiqueta.AutoSize = false;
            this.lblEtiqueta.ForeColor = System.Drawing.Color.Gray;
            this.lblEtiqueta.Location = new System.Drawing.Point(0, 0);
            this.lblEtiqueta.Name = "lblEtiqueta";
            this.lblEtiqueta.Text = "";
            this.lblEtiqueta.Dock = DockStyle.Fill;
            this.lblEtiqueta.Click += new EventHandler((object sender, EventArgs e) =>
            {
                if (this.Focused)
                    this.lblEtiqueta.Visible = false;
                else
                    this.Focus();
                this.OnClick(new EventArgs());
            });

            // 
            // Texto
            // 
            this.PasarEnfoqueConEnter = true;
            this.FontChanged += new EventHandler(TextoMod_FontChanged);
            this.Enter += new EventHandler(TextoMod_Enter);
            this.TextChanged += new EventHandler(TextoMod_TextChanged);
            this.KeyDown += new KeyEventHandler(TextoMod_KeyDown);
            this.Leave += new EventHandler(TextoMod_Leave);
            this.Controls.Add(this.lblEtiqueta);
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

        private bool _PasarEnfoqueConEnter = true;  // Valor predeterminado
        public bool PasarEnfoqueConEnter
        {
            get { return this._PasarEnfoqueConEnter; }
            set { this._PasarEnfoqueConEnter = value; }
        }

        private bool _SeleccionarTextoAlEnfoque = false;  // Valor predeterminado
        public bool SeleccionarTextoAlEnfoque
        {
            get { return this._SeleccionarTextoAlEnfoque; }
            set { this._SeleccionarTextoAlEnfoque = value; }
        }

        #endregion

        #region [ Eventos ]

        void TextoMod_FontChanged(object sender, EventArgs e)
        {
            this.lblEtiqueta.Font = this.Font;
        }

        private void TextoMod_Enter(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = false;

            if (this.SeleccionarTextoAlEnfoque)
                this.SelectAll();
        }

        private void TextoMod_TextChanged(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = (!this.Focused && this.Text == "");
        }

        void TextoMod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.PasarEnfoqueConEnter)
            {
                if (this.Parent != null)
                    this.Parent.SelectNextControl(this, true, true, true, true);
            }
        }

        private void TextoMod_Leave(object sender, EventArgs e)
        {
            this.lblEtiqueta.Visible = (this.Text == "");
        }

        #endregion
    }
}
