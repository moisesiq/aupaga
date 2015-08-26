using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Refaccionaria.Negocio
{
    public partial class SeleccionListado : Form
    {
        BindingSource oBinding;

        public SeleccionListado(object oDatos)
        {
            InitializeComponent();

            this.CargarDatos(oDatos);
        }

        #region [ Propiedades ]

        private Dictionary<string, object> _Seleccion;
        public Dictionary<string, object> Seleccion {
            get {
                return this._Seleccion;
            }
        }

        public Label Busqueda { get { return this.lblBusqueda; } }

        #endregion

        #region [ Eventos ]

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            char cCaracter = (char)keyData;
            if (char.IsLetterOrDigit(cCaracter) || keyData == Keys.Space)
            {
                this.lblBusqueda.Text += cCaracter;
            }
            else
            {
                switch (keyData)
                {
                    case Keys.Escape:
                        if (this.lblBusqueda.Text == "")
                            this.Close();
                        else
                            this.lblBusqueda.Text = "";
                        break;
                    case Keys.Back:
                        if (this.lblBusqueda.Text != "")
                            this.lblBusqueda.Text = this.lblBusqueda.Text.Izquierda(this.lblBusqueda.Text.Length - 1);
                        break;
                    case Keys.Enter:
                        this.dgvListado_CellDoubleClick(this, null);
                        break;
                    case Keys.Up:
                        break;
                    case Keys.Down:
                        break;
                    // default:
                        // return base.ProcessCmdKey(ref msg, keyData);
                }
            }
            return false;
        }

        private void SeleccionListado_Load(object sender, EventArgs e)
        {
            // this.oBinding.RemoveFilter();
        }

        private void SeleccionListado_Shown(object sender, EventArgs e)
        {
            this.dgvListado.AutoResizeColumns();
        }

        private void dgvListado_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void dgvListado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvListado.CurrentRow == null)
            {
                this._Seleccion = null;
            }
            else
            {
                this._Seleccion = this.dgvListado.CurrentRow.ADiccionario();
                this.DialogResult = DialogResult.OK;
            }
            this.Close();
        }

        private void lblBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.oBinding.Filter = this.dgvListado.ObtenerCadenaDeFiltro(this.lblBusqueda.Text);
        }

        #endregion

        #region [ Métodos ]



        #endregion

        #region [ Públicos ]

        public void CargarDatos(object oDatos)
        {
            this.oBinding = new BindingSource(oDatos, "");
            this.dgvListado.DataSource = null;
            this.dgvListado.DataSource = this.oBinding;
            this.dgvListado.AutoResizeColumns();
        }

        public void MostrarColumnas(params string[] aColumnas)
        {
            this.dgvListado.MostrarColumnas(aColumnas);
        }

        public void OcultarColumnas(params string[] aColumnas)
        {
            this.dgvListado.OcultarColumnas(aColumnas);
        }

        #endregion

    }
}
