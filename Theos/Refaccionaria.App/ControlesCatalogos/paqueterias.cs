using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheosProc;

using LibUtil;

namespace Refaccionaria.App
{
    public partial class paqueterias : ListadoBase
    {
        // Para el Singleton
        private static paqueterias _Instance;
        public static paqueterias Instance
        {
            get
            {
                if (paqueterias._Instance == null || paqueterias._Instance.IsDisposed)
                    paqueterias._Instance = new paqueterias();
                return paqueterias._Instance;
            }
        }
        //

        public paqueterias()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<ProveedorPaqueteriasView>("Paqueterias", Datos.GetListOf<ProveedorPaqueteriasView>(p => p.ProveedorPaqueteriaID > 0));
                this.IniciarCarga(dt, "Paqueterias");
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos ]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetallePaqueteria p = new DetallePaqueteria();
            p.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetallePaqueteria p = new DetallePaqueteria(Util.Entero(this.dgvDatos.CurrentRow.Cells["ProveedorPaqueteriaID"].Value));
            p.ShowDialog();
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
