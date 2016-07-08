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
    public partial class proveedores : ListadoBase
    {
        public static proveedores Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly proveedores instance = new proveedores();
        }

        public proveedores()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<ProveedoresView>("Proveedores", Datos.GetListOf<ProveedoresView>(p => p.ProveedorID > 0));
                this.IniciarCarga(dt, "Proveedores");
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
            DetalleProveedor p = new DetalleProveedor();
            p.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleProveedor p = new DetalleProveedor(Util.Entero(this.dgvDatos.CurrentRow.Cells["ProveedorID"].Value));
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
