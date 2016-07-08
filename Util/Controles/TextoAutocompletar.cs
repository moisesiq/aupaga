using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

namespace LibUtil
{
    public partial class TextoAutocompletar : UserControl
    {
        object oFuenteOriginal;
        BindingSource oFuenteFiltro = new BindingSource();

        // Form frmLista;
        // DataGridView dgvLista;

        public TextoAutocompletar()
        {
            InitializeComponent();
            // this.dgvDatos.DataSource = this.oFuenteFiltro;
            // this.OcultarAutocompletar();

            /* 
            // Se intentó hacer la lista desplegable con un formulario, pero no se ha concluido
            this.dgvLista = new DataGridView();
            this.dgvLista.Dock = DockStyle.Fill;
            this.dgvLista.AllowUserToAddRows = false;
            this.dgvLista.AllowUserToDeleteRows = false;
            this.dgvLista.AllowUserToResizeColumns = false;
            this.dgvLista.AllowUserToResizeRows = false;
            this.dgvLista.BackgroundColor = Color.White;
            this.dgvLista.ColumnHeadersVisible = false;
            this.dgvLista.MultiSelect = false;
            this.dgvLista.ReadOnly = true;
            this.dgvLista.RowHeadersVisible = false;
            this.dgvLista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvLista.StandardTab = true;

            this.frmLista = new Form();
            this.frmLista.FormBorderStyle = FormBorderStyle.None;
            this.frmLista.ShowInTaskbar = false;
            this.frmLista.Top = this.Bottom;
            this.frmLista.Left = this.Left;
            this.frmLista.Height = 200;
            this.frmLista.TopMost = true;

            this.Controls.Add(this.dgvLista);
            */
        }

        #region [ Propiedades ]

        private Color _ColorDeFondo = Color.White;
        public Color ColorDeFondo
        {
            get { return this.txtBusqueda.BackColor; }
            set
            {
                this._ColorDeFondo = value;
                this.txtBusqueda.BackColor = value;
                this.dgvDatos.BackgroundColor = value;
            }
        }

        public Color ColorDeLetra {
            get { return this.txtBusqueda.ForeColor; }
            set
            {
                this.txtBusqueda.ForeColor = value;
                this.dgvDatos.ForeColor = value;
            }
        }

        [DefaultValue(200)]
        public int AltoLista {
            get { return this.dgvDatos.Height; }
            set
            {
                this.dgvDatos.Height = value;
            }
        }

        public string CampoValor { get; set; }

        public string CampoMostrar { get; set; }

        public object FuenteDeDatos {
            get
            {
                return this.oFuenteFiltro.DataSource;
            }
            set
            {
                this.oFuenteOriginal = value;

                this.dgvDatos.DataSource = null;
                this.dgvDatos.DataSource = value;

                if (value == null) return;

                var oTabla = this.dgvDatos.ADataTable();
                this.oFuenteFiltro.DataSource = oTabla;
                this.dgvDatos.DataSource = null;
                this.dgvDatos.DataSource = this.oFuenteFiltro;

                this.dgvDatos.Visible = true;
                this.dgvDatos.MostrarColumnas(this.CampoMostrar);
                this.dgvDatos.Columns[this.CampoMostrar].Width = (this.dgvDatos.Width - 2 - 17);
                this.dgvDatos.Visible = false;
            }
        }

        private object _ValorSel;
        public object ValorSel
        {
            get
            {
                return this._ValorSel;
            }
            set
            {
                int iFila = this.SeleccionarFilaDeValor(value);
                this._ValorSel = (iFila >= 0 ? value : null);
            }
        }

        /* private object _ElementoSel;
        public object ElementoSel
        {
            get
            {
                return this._ElementoSel;
            }
        } */

        public string Texto
        {
            get { return this.txtBusqueda.Text; }
            set { this.txtBusqueda.Text = value; }
        }

        public string Etiqueta
        {
            get { return this.txtBusqueda.Etiqueta; }
            set { this.txtBusqueda.Etiqueta = value; }
        }

        public bool AutocompletarVisible { get { return (this.dgvDatos.Visible); } }

        #endregion

        #region [ Eventos propios ]

        public event EventHandler SeleccionCambiada;
        public event EventHandler TextoCambiado;

        #endregion

        #region [ Eventos ]

        private void TextoAutocompletar_Leave(object sender, EventArgs e)
        {
            this.OcultarAutocompletar();
        }

        private void txtBusqueda_Click(object sender, EventArgs e)
        {
            this.txtBusqueda.SelectAll();
        }

        private void txtBusqueda_Enter(object sender, EventArgs e)
        {
            this.txtBusqueda.SelectAll();
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                    if (e.Alt)
                    {
                        if (e.KeyCode == Keys.Down)
                            this.MostrarAutocompletar();
                        else
                            this.OcultarAutocompletar();
                    }
                    else
                    {
                        if (this.dgvDatos.Rows.Count <= 0) break;
                        this.SeleccionarFila(e.KeyCode);
                        this.dgvDatos.Focus();
                    }
                    break;
                case Keys.Enter:
                    if (this.AutocompletarVisible)
                        this.AplicarSeleccion(this.dgvDatos.CurrentRow);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Escape:
                    this.CancelarSeleccion();
                    e.SuppressKeyPress = true;
                    break;
            }
        }
                
        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (this.TextoCambiado != null)
                this.TextoCambiado.Invoke(sender, e);

            if (!this.txtBusqueda.Focused) return;

            if (this.txtBusqueda.Text == "")
            {
                this.oFuenteFiltro.Filter = "";
                return;
            }

            this.oFuenteFiltro.Filter = Util.ObtenerCadenaDeFiltro(this.dgvDatos, this.txtBusqueda.Text);

            this.MostrarAutocompletar();
        }

        private void btnMostrarAutocompletar_Click(object sender, EventArgs e)
        {
            this.txtBusqueda.Focus();
            if (this.AutocompletarVisible)
                this.OcultarAutocompletar();
            else
                this.MostrarAutocompletar();
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.AplicarSeleccion(this.dgvDatos.CurrentRow);
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    this.CancelarSeleccion();
                    break;
            }
        }
        
        private void dgvDatos_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDatos[e.ColumnIndex, e.RowIndex].Displayed)
                this.dgvDatos[e.ColumnIndex, e.RowIndex].Selected = true;
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.AplicarSeleccion(this.dgvDatos.CurrentRow);
        }

        private void dgvDatos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (!this.dgvDatos.Focused) return;

            this.MostrarSeleccion(this.dgvDatos.CurrentRow);
        }

        #endregion

        #region [ Métodos ]

        private void MostrarAutocompletar()
        {
            this.dgvDatos.Visible = true;
            return;
            if (!this.AutocompletarVisible)
                this.Height += (this.dgvDatos.Height + 1);
        }

        private void OcultarAutocompletar()
        {
            this.dgvDatos.Visible = false;
            this.oFuenteFiltro.RemoveFilter();

            return;
            if (this.AutocompletarVisible)
                this.Height -= (this.dgvDatos.Height + 1);
        }

        private void SeleccionarFila(Keys oTecla)
        {
            if (oTecla == Keys.Down)
            {
                if (this.dgvDatos.CurrentRow == null)
                {
                    this.dgvDatos.Rows[this.dgvDatos.Rows.Count - 1].Cells[this.CampoMostrar].Selected = true;
                }
                else
                {
                    if ((this.dgvDatos.CurrentRow.Index + 1) < this.dgvDatos.Rows.Count)
                        this.dgvDatos[this.CampoMostrar, this.dgvDatos.CurrentRow.Index + 1].Selected = true;
                }
            }
            else
            {
                if (this.dgvDatos.CurrentRow == null)
                {
                    this.dgvDatos.Rows[0].Cells[this.CampoMostrar].Selected = true;
                }
                else
                {
                    if ((this.dgvDatos.CurrentRow.Index - 1) >= 0)
                        this.dgvDatos[this.CampoMostrar, this.dgvDatos.CurrentRow.Index - 1].Selected = true;
                }
            }
        }

        private int SeleccionarFilaDeValor(object oValor)
        {
            int iFila = this.dgvDatos.EncontrarIndiceDeValor(this.CampoValor, oValor);
            if (iFila >= 0)
                this.AplicarSeleccion(this.dgvDatos.Rows[iFila]);
            else
                this.dgvDatos.ClearSelection();

            return iFila;
        }

        private void MostrarSeleccion(DataGridViewRow oFila)
        {
            if (oFila == null)
            {
                this.txtBusqueda.Clear();
            }
            else
            {
                this.txtBusqueda.Text = Util.Cadena(oFila.Cells[this.CampoMostrar].Value);
                this.txtBusqueda.SelectAll();
            }
        }

        private void AplicarSeleccion(DataGridViewRow oFila)
        {
            this._ValorSel = (oFila == null ? null : oFila.Cells[this.CampoValor].Value);
            // this._ElementoSel = (oFila == null ? null : (oFila.DataBoundItem));
            this.MostrarSeleccion(oFila);
            
            this.OcultarAutocompletar();

            if (this.SeleccionCambiada != null)
                this.SeleccionCambiada.Invoke(this, new EventArgs());
        }

        private void CancelarSeleccion()
        {
            this.OcultarAutocompletar();
            this.txtBusqueda.Clear();
            this._ValorSel = null;
        }

        #endregion

        #region [ Públicos ]

        public void CargarDatos(string sCampoValor, string sCampoMostrar, object oDatos)
        {
            this.CampoValor = sCampoValor;
            this.CampoMostrar = sCampoMostrar;
            this.FuenteDeDatos = oDatos;
        }

        public void MostrarVariasColumnas(params string[] aColumnas)
        {
            this.dgvDatos.MostrarColumnas(aColumnas);
            // Se ordenan las columnas
            int iCol = 0;
            int iAncho = ((this.dgvDatos.Width - 19) / aColumnas.Length);
            foreach (string sCol in aColumnas)
            {
                this.dgvDatos.Columns[sCol].DisplayIndex = iCol++;
                this.dgvDatos.Columns[sCol].Width = iAncho;
            }
        }

        public void AnchosDeColumnas(params int[] aAnchos) {
            int iCol = 0;
            foreach (int iAncho in aAnchos) {
                foreach (DataGridViewColumn oCol in this.dgvDatos.Columns) {
                    if (oCol.DisplayIndex == iCol) {
                        oCol.Width = iAncho;
                        break;
                    }
                }
                iCol++;
            }
        }
        
        #endregion
                
    }
}
