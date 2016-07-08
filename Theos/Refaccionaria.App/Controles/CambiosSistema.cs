using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CambiosSistema : UserControl
    {
        // Para el Singleton *
        private static CambiosSistema instance;
        public static CambiosSistema Instance
        {
            get
            {
                if (CambiosSistema.instance == null || CambiosSistema.instance.IsDisposed)
                    CambiosSistema.instance = new CambiosSistema();
                return CambiosSistema.instance;
            }
        }
        //

        BindingSource Lista;

        public string PermisoAgregar;
        public string PermisoModificar;

        public CambiosSistema()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CambiosSistema_Load(object sender, System.EventArgs e)
        {
            this.Lista = new BindingSource();
            this.dgvDatos.DataSource = this.Lista;
            this.ActualizarDatos();
        }

        private void btnAgregar_Click(object sender, System.EventArgs e)
        {
            // Se valida el permiso
            if (this.PermisoAgregar != "")
            {
                UtilLocal.MensajeAdvertencia(this.PermisoAgregar);
                return;
            }

            var frmCambio = new CambiosSistemaDetalle();
            frmCambio.ShowDialog(Principal.Instance);
            if (frmCambio.DialogResult == DialogResult.OK)
                this.ActualizarDatos();
        }

        private void btnModificar_Click(object sender, System.EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            // Se valida el permiso
            if (this.PermisoModificar != "")
            {
                UtilLocal.MensajeAdvertencia(this.PermisoModificar);
                return;
            }

            int iCambioSistemaID = Util.Entero(this.dgvDatos.CurrentRow.Cells["CambioSistemaID"].Value);
            var frmCambio = new CambiosSistemaDetalle(iCambioSistemaID);
            frmCambio.ShowDialog(Principal.Instance);
            if (frmCambio.DialogResult == DialogResult.OK)
                this.ActualizarDatos();
        }

        private void cmbVersion_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.Lista.Filter = string.Format("Version = '{0}'", this.cmbVersion.Text);
        }

        private void cmbVersion_TextChanged(object sender, System.EventArgs e)
        {
            if (this.cmbVersion.Text == "")
            {
                this.Lista.RemoveFilter();
                return;
            }
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.btnModificar_Click(sender, e);
        }

        #endregion

        #region [ Métodos ]

        private void ActualizarDatos()
        {
            Cargando.Mostrar();

            var oDatos = Datos.GetListOf<CambioSistema>();
            this.Lista.DataSource = Util.ListaEntityADataTable<CambioSistema>(oDatos);

            this.dgvDatos.DataSource = null;
            this.dgvDatos.DataSource = this.Lista;

            // Se modifican los encabezados de algunas columnas
            if (this.dgvDatos.Columns.Count > 0)
            {
                this.dgvDatos.Columns["CambioSistemaID"].HeaderText = "No.";
                this.dgvDatos.Columns["Version"].HeaderText = "Versión";
                this.dgvDatos.Columns["Modificacion"].HeaderText = "Modificación";
            }
            this.dgvDatos.AutoResizeColumns();
            this.dgvDatos.Columns["Modificacion"].Width = 400;
            this.dgvDatos.Columns["Observaciones"].Width = 400;
            this.dgvDatos.Columns["Modificacion"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dgvDatos.Columns["Observaciones"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dgvDatos.AutoResizeRows();

            // Se obtienen las versiones disponibles, para el combo
            this.cmbVersion.Items.Clear();
            var Versiones = oDatos.Select(q => q.Version).Distinct().ToList();
            foreach (string sVersion in Versiones)
                this.cmbVersion.Items.Add(sVersion);

            Cargando.Cerrar();
        }

        #endregion
                                                
    }
}
