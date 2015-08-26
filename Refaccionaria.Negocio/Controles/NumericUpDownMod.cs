using System;
using System.Windows.Forms;

namespace Refaccionaria.Negocio
{
    public class NumericUpDownMod : NumericUpDown
    {
        TextBox Texto;

        public NumericUpDownMod()
        {
            this.Texto = (this.Controls[1] as TextBox);
            this.Enter += new EventHandler(NumericUpDownMod_Enter);

            this.Texto.MaxLength = this.Maximum.ToString().Length;
            this.Texto.TextChanged += new EventHandler(Texto_TextChanged);
        }

        void Texto_TextChanged(object sender, EventArgs e)
        {
            if (this.Texto.Text == "")
            {
                this.Value = 0;
                this.Texto.Text = "";
            }
        }
                
        #region [ Propiedades ]

        private bool _SeleccionarTextoAlEnfoque = false;  // Valor predeterminado
        public bool SeleccionarTextoAlEnfoque
        {
            get { return this._SeleccionarTextoAlEnfoque; }
            set { this._SeleccionarTextoAlEnfoque = value; }
        }
        
        #endregion

        #region [ Eventos ]

        private void NumericUpDownMod_Enter(object sender, EventArgs e)
        {
            if (this.SeleccionarTextoAlEnfoque)
                this.Select(0, this.Value.ToString().Length);
        }
                
        #endregion

    }
}
