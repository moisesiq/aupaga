using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Refaccionaria.Negocio
{
    public partial class ComboMultiSel : UserControl
    {
        bool bListaVisible = false;
        
        public ComboMultiSel()
        {
            InitializeComponent();

            this.MostrarLista(false);
        }

        #region [ Propiedades ]

        public CheckedListBox.ObjectCollection Items
        {
            get { return this.clbSeleccion.Items; }
        }

        public List<string> CheckedItems
        {
            get
            {
                return this.clbSeleccion.CheckedItems.Cast<string>().ToList();
            }
        }

        public override string Text
        {
            get { return this.txtSeleccion.Text; }
            set
            {
                var oLista = value.Split(',');
                for (int i = 0; i < this.clbSeleccion.Items.Count; i++)
                {
                    if (oLista.Contains(this.clbSeleccion.Items[i].ToString()))
                        this.clbSeleccion.SetItemChecked(i, true);
                }
                this.txtSeleccion.Text = value;
            }
        }

        public List<EnteroCadena> ElementosSeleccionados
        {
            get
            {
                var oLista = new List<EnteroCadena>();
                foreach (var oItem in this.clbSeleccion.CheckedItems)
                    oLista.Add(oItem as EnteroCadena);
                return oLista;
            }
        }

        public List<int> ValoresSeleccionados
        {
            get
            {
                var oLista = new List<int>();
                foreach (var oItem in this.clbSeleccion.CheckedItems)
                    oLista.Add((oItem as EnteroCadena).Entero);
                return oLista;
            }
        }

        private string _Etiqueta;
        public string Etiqueta
        {
            get
            {
                return this._Etiqueta;
            }
            set
            {
                this._Etiqueta = value;
                this.VerEtiqueta();
            }
        }

        #endregion
                
        #region [ Eventos ]

        private void ComboMultiSel_Load(object sender, EventArgs e)
        {
            // Truco para que respete el forecolor cuando el control es ReadOnly ;)
            this.txtSeleccion.BackColor = this.txtSeleccion.BackColor;
        }

        private void ComboMultiSel_Leave(object sender, EventArgs e)
        {
            if (this.bListaVisible)
                this.MostrarLista(false);
        }

        private void txtSeleccion_Click(object sender, EventArgs e)
        {
            this.txtSeleccion.SelectAll();
        }

        private void txtSeleccion_Enter(object sender, EventArgs e)
        {
            this.txtSeleccion.ForeColor = Color.Black;
            this.txtSeleccion.SelectAll(); // No funciona :(
            this.txtSeleccion.Text = "";
        }
                
        private void txtSeleccion_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (this.clbSeleccion.Items.Count > (this.clbSeleccion.SelectedIndex + 1))
                        this.clbSeleccion.SelectedIndex++;
                    break;
                case Keys.Up:
                    if (this.clbSeleccion.SelectedIndex > 0)
                        this.clbSeleccion.SelectedIndex--;
                    break;
            }

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                e.Handled = true;
        }

        private void txtSeleccion_TextChanged(object sender, EventArgs e)
        {
            if (this.txtSeleccion.Text == "")
            {
                this.MostrarLista(false);
            }
            else
            {
                this.MostrarLista(true);
                this.BuscarEnLista(this.txtSeleccion.Text);
            }
        }

        private void txtSeleccion_Leave(object sender, EventArgs e)
        {
            this.VerEtiqueta();
        }

        private void btnCombo_Click(object sender, EventArgs e)
        {
            this.MostrarLista(!this.bListaVisible);
        }

        private void clbSeleccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this.MostrarValor();   
        }

        private void clbSeleccion_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
        }

        #endregion

        #region [ Métodos ]

        private void MostrarValor()
        {
            string sValor = "";
            foreach (var oElemento in this.clbSeleccion.CheckedItems)
                sValor += ("," + oElemento.ToString());
            this.txtSeleccion.Text = (sValor == "" ? "" : sValor.Substring(1));
            this.VerEtiqueta();            
        }

        private void VerEtiqueta()
        {
            this.txtSeleccion.Text = this._Etiqueta;
            if (this.clbSeleccion.CheckedItems.Count > 0)
            {
                this.txtSeleccion.ForeColor = Color.Black;
            }
            else
            {
                this.txtSeleccion.ForeColor = Color.Gray;
            }
        }

        private void MostrarLista(bool bMostrar)
        {
            if (bMostrar)
            {
                this.Height = 203;
                //this.BringToFront();
                // this.clbSeleccion.Parent = this.ParentForm;
                this.clbSeleccion.BringToFront();
            }
            else
            {
                this.Height = 20;
            }
            this.clbSeleccion.Visible = bMostrar;
            this.bListaVisible = bMostrar;
        }

        private void BuscarEnLista(string sBusqueda)
        {
            sBusqueda = sBusqueda.ToLower();
            for (int i = 0; i < this.clbSeleccion.Items.Count; i++)
            {
                if (this.clbSeleccion.Items[i].ToString().ToLower().Contains(sBusqueda))
                {
                    this.clbSeleccion.SelectedIndex = i;
                    break;
                }
            }
        }

        #endregion

        #region [ Públicos ]

        public void AgregarElemento(int iValor, string sTexto, bool bSeleccionado)
        {
            this.clbSeleccion.Items.Add(new ElementoLista(iValor, sTexto), bSeleccionado);
        }

        public void AgregarElemento(int iValor, string sTexto)
        {
            this.clbSeleccion.Items.Add(new ElementoLista(iValor, sTexto), false);
        }

        #endregion

    }

    class ElementoLista : EnteroCadena
    {
        public ElementoLista(int iValor, string sTexto)
        {
            this.Entero = iValor;
            this.Cadena = sTexto;
        }

        public override string ToString()
        {
            return this.Cadena;
        }
    }
}