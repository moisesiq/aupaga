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
    public partial class sucursales : ListadoBase
    {
        // Para el Singleton
        private static sucursales _Instance;
        public static sucursales Instance
        {
            get
            {
                if (sucursales._Instance == null || sucursales._Instance.IsDisposed)
                    sucursales._Instance = new sucursales();
                return sucursales._Instance;
            }
        }
        //

        public sucursales()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<SucursalesView>("Sucursales", Negocio.General.GetListOf<SucursalesView>(s => s.SucursalID > 0));
                this.IniciarCarga(dt, "Sucursales");
                this.txtBusqueda.Focus();
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
            DetalleSucursal s = new DetalleSucursal();
            s.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleSucursal s = new DetalleSucursal(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["SucursalID"].Value));
            s.ShowDialog();
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
