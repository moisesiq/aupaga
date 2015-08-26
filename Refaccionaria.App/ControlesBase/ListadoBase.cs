using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ListadoBase : UserControl
    {
        BindingSource fuenteDatos;
        public bool Seleccionado = false;
        public Dictionary<string, object> Sel;

        public ListadoBase()
        {
            InitializeComponent();
        }

        #region [ Metodos ]

        public void IniciarCarga(object FuenteDatos, string sTitulo)
        {
            this.fuenteDatos = new BindingSource();
            this.Sel = new Dictionary<string, object>();
            this.groupGenerico.Text = sTitulo;
            this.fuenteDatos.DataSource = FuenteDatos;
            this.EstablecerFuenteDeDatos(this.fuenteDatos.DataSource);
        }

        public void EstablecerFuenteDeDatos(object Fuente)
        {
            this.fuenteDatos.DataSource = Fuente;
            this.dgvDatos.DataSource = null;
            this.dgvDatos.DataSource = this.fuenteDatos;
            this.dgvDatos.AutoResizeColumns();
            Helper.OcultarColumnas(this.dgvDatos, new string[] { "Busqueda", "EntityState", "EntityKey" });
            Helper.ColumnasToHeaderText(this.dgvDatos);            
        }

        #endregion

        #region [ Eventos ]
        
        private void ListadoBase_Load(object sender, EventArgs e)
        {
            this.txtBusqueda.Focus();
            //this.groupGenerico.ForeColor = Color.White;
        }

        protected virtual void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvDatos.CurrentRow == null)            
                return;

            string sColumna;
            for (int iCont = 0; iCont < this.dgvDatos.Columns.Count; iCont++)
            {
                sColumna = this.dgvDatos.Columns[iCont].Name;
                this.Sel[sColumna] = this.dgvDatos.CurrentRow.Cells[sColumna].Value;
            }
            this.Seleccionado = true;
        }

        private void dgvDatos_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dgvDatos_CellDoubleClick(sender, null);
        }

        protected virtual void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                this.dgvDatos_CellDoubleClick(sender, null);
            }
            if (e.KeyCode == Keys.F5)
            {
                this.txtBusqueda.Clear();
                this.txtBusqueda.Focus();
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBusqueda.Text.Length > 0)
                {
                    string Value = txtBusqueda.Text; //"M a";
                    string filter = string.Empty;
                    if (Value.Contains(" ")) //revisar si existe espacio en blanco
                    {
                        string[] Values = Value.Split(' '); //separar valores
                        filter += "(Busqueda like '%" + Values[0].Trim() + "%') AND ";
                        for (int i = 1; i < Values.Length; i++)
                        {
                            filter += "(Busqueda like '%" + Values[i].Trim() + "%') AND ";
                        }
                        filter = filter.Substring(0, filter.LastIndexOf("AND ") - 1);
                    }
                    else
                    {
                        filter = "Busqueda like '%" + Value + "%'";
                    }
                    if (fuenteDatos != null)
                        fuenteDatos.Filter = filter;
                }
                else
                {
                    if (fuenteDatos != null)
                        fuenteDatos.Filter = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        protected virtual void btnAgregar_Click(object sender, EventArgs e)
        {

        }

        protected virtual void btnModificar_Click(object sender, EventArgs e)
        {

        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtBusqueda.Clear();
            }
            if (e.KeyCode == Keys.Down)
            {
                this.dgvDatos.Focus();
            }
        }

        #endregion
        
    }
}
