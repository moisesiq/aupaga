using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class permisos : Refaccionaria.App.ListadoBase
    {
        // Para el Singleton
        private static permisos _Instance;
        public static permisos Instance
        {
            get
            {
                if (permisos._Instance == null || permisos._Instance.IsDisposed)
                    permisos._Instance = new permisos();
                return permisos._Instance;
            }
        }
        //

        public permisos()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<PermisosView>("Permisos", Negocio.General.GetListOf<PermisosView>(p => p.PermisoID > 0));
                this.IniciarCarga(dt, "Permisos");
                this.btnAgregar.Visible = false;
                this.btnModificar.Visible = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos ]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            //DetalleMedida m = new DetalleMedida();
            //m.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            //DetalleMedida m = new DetalleMedida(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["MedidaID"].Value));
            //m.ShowDialog();
        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            btnModificar_Click(sender, null);
        }

        #endregion

    }
}
