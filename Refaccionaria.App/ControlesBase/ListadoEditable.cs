using System;
using System.Windows.Forms;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ListadoEditable : UserControl
    {
        protected ControlError ctlError = new ControlError();

        public ListadoEditable()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ListadoEditable_Load(object sender, EventArgs e)
        {
            // Se manda inicializar el grid editable
            this.dgvDatos.Inicializar();
            // Se manda cargar los datos
            this.CargarDatos();
        }

        protected virtual void btnGuardar_Click(object sender, EventArgs e)
        {
            this.AccionGuardar();
        }

        #endregion
                               
        #region [ Métodos de utilidad ]

        protected DataGridViewColumn AgregarColumna(string sNombre, string sEncabezado, int iAncho)
        {
            this.dgvDatos.Columns.Add(sNombre, sEncabezado);
            this.dgvDatos.Columns[sNombre].Width = iAncho;

            return this.dgvDatos.Columns[sNombre];
        }

        protected DataGridViewColumn AgregarColumnaImporte(string sNombre, string sEncabezado)
        {
            var oColumna = this.AgregarColumna(sNombre, sEncabezado, 80);
            oColumna.FormatoMoneda();

            return oColumna;
        }

        protected DataGridViewComboBoxColumn AgregarColumnaCombo(string sNombre, string sEncabezado, int iAncho)
        {
            var oColumna = new DataGridViewComboBoxColumn()
            {
                Name = sNombre,
                HeaderText = sEncabezado,
                Width = iAncho,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                FlatStyle = FlatStyle.Flat
            };
            this.dgvDatos.Columns.Add(oColumna);

            return oColumna;
        }

        protected DataGridViewCheckBoxColumn AgregarColumnaCheck(string sNombre, string sEncabezado, int iAncho)
        {
            var oColumna = new DataGridViewCheckBoxColumn()
            {
                Name = sNombre,
                HeaderText = sEncabezado,
                Width = iAncho,
            };
            this.dgvDatos.Columns.Add(oColumna);

            return oColumna;
        }

        #endregion

        #region [ Métodos a sobreescribir ]

        protected virtual void CargarDatos()
        {

        }

        protected virtual bool AccionGuardar()
        {
            return true;
        }

        #endregion
    }
}
