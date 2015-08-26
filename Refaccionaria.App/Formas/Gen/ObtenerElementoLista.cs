using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ObtenerElementoLista : Form
    {
        bool bMultiple;

        public Dictionary<string, object> Sel = new Dictionary<string, object>();
        public object Seleccion;
        public List<object> SeleccionLista;

        #region [ Constructor ]

        public ObtenerElementoLista()
        {
            InitializeComponent();
        }

        public ObtenerElementoLista(string sMensaje, object FuenteDatos)
        {
            InitializeComponent();
            this.lblMensaje.Text = sMensaje;
            this.dgvDatos.DataSource = FuenteDatos;
        }

        #endregion

        #region [ Eventos ]

        private void ObtenerElementoLista_Shown(object sender, EventArgs e)
        {
            this.dgvDatos.AutoResizeColumns();
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnAceptar_Click(sender, e);
                e.Handled = true;
            }
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.bMultiple) return;

            this.btnAceptar_Click(sender, e);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.AccionAceptar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            // this.DialogResult = DialogResult.Ignore;
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        public void OcultarColumnas(params string[] Columnas)
        {
            foreach (string sColumna in Columnas)
            {
                if (this.dgvDatos.Columns.Contains(sColumna))
                    this.dgvDatos.Columns[sColumna].Visible = false;
            }
        }

        public void MostrarColumnas(params string[] Columnas)
        {
            var ListaCols = new List<string>();
            ListaCols.AddRange(Columnas);

            foreach (DataGridViewColumn Col in this.dgvDatos.Columns)
            {
                if (!ListaCols.Contains(Col.Name))
                    Col.Visible = false;
            }
        }

        private bool AccionAceptar()
        {
            if (this.bMultiple)
            {
                this.SeleccionLista = new List<object>();
                foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
                {
                    if (Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                        this.SeleccionLista.Add(oFila.DataBoundItem);
                }
            }
            else
            {
                if (this.dgvDatos.CurrentRow == null) return false;

                string sColumna;
                for (int iCont = 0; iCont < this.dgvDatos.Columns.Count; iCont++)
                {
                    sColumna = this.dgvDatos.Columns[iCont].Name;
                    this.Sel[sColumna] = this.dgvDatos.CurrentRow.Cells[sColumna].Value;
                }

                this.Seleccion = this.dgvDatos.CurrentRow.DataBoundItem;
            }

            return true;
        }

        #endregion

        #region [ Públicos ]

        public void HacerMultiple()
        {
            this.dgvDatos.ReadOnly = false;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
                oCol.ReadOnly = true;

            var oColCheck = new DataGridViewCheckBoxColumn() { Name = "Sel", HeaderText = "Sel." };
            this.dgvDatos.Columns.Insert(0, oColCheck);

            this.bMultiple = true;
        }

        #endregion

    }
}
