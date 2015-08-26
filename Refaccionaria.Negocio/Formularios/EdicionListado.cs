using System;
using System.Windows.Forms;
using System.Linq;
using System.Data;

namespace Refaccionaria.Negocio
{
    public partial class EdicionListado : Form
    {
        public EdicionListado()
        {
            InitializeComponent();
        }
        
        public EdicionListado(object oDatos)
        {
            InitializeComponent();

            this.CargarDatos(oDatos);
        }

        #region [ Eventos propios ]

        public event EventHandler<FormClosingEventArgs> Aceptado;

        #endregion

        #region [ Propiedades ]
        #endregion

        #region [ Eventos ]

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Se ejecuta el evento "Aceptado", si se hubiera usado
            if (this.Aceptado != null)
            {
                var oCerrando = new FormClosingEventArgs(CloseReason.None, false);
                this.Invoke(this.Aceptado, this, oCerrando);
                if (oCerrando.Cancel)
                    return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Públicos ]

        public void CargarDatos(object oDatos)
        {
            this.dgvListado.DataSource = oDatos;
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

        public void HabilitarColumnas(params string[] aColumnas)
        {
            foreach (DataGridViewColumn oCol in this.dgvListado.Columns)
                oCol.ReadOnly = !aColumnas.Contains(oCol.Name);
        }

        public void DeshabilitarColumnas(params string[] aColumnas)
        {
            foreach (DataGridViewColumn oCol in this.dgvListado.Columns)
                oCol.ReadOnly = aColumnas.Contains(oCol.Name);
        }

        public DataGridViewColumn AgregarColumnaEdicion(string sNombre, string sEncabezado)
        {
            int iCol = this.dgvListado.Columns.Add(sNombre, sEncabezado);
            return this.dgvListado.Columns[iCol];
        }

        public DataGridViewCheckBoxColumn AgregarColumnaSeleccion(string sNombre)
        {
            var oCol = new DataGridViewCheckBoxColumn()
            {
                Name = sNombre,
                HeaderText = "Sel",
                Width = 40
            };
            this.dgvListado.Columns.Insert(0, oCol);
            return oCol;
        }

        public DataTable ObtenerResultado()
        {
            return this.dgvListado.ADataTable();
        }

        #endregion
    }
}
