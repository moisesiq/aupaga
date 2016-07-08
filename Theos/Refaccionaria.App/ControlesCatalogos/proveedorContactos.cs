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
    public partial class proveedorContactos : ListadoBase
    {        
        private int proveedorId = 0;

        public static proveedorContactos Instance
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

            internal static readonly proveedorContactos instance = new proveedorContactos();
        }

        public proveedorContactos()
        {
            InitializeComponent();            
        }

        #region [ Eventos ]
        
        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetalleProveedorContacto pc = new DetalleProveedorContacto();
            pc.ProveedorId = proveedorId;
            pc.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleProveedorContacto pc = new DetalleProveedorContacto(Util.Entero(this.dgvDatos.CurrentRow.Cells["ProveedorContactoID"].Value));
            pc.ShowDialog();
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

        #region [ Metodos ]

        public void ActualizarListado(int Id)
        {
            proveedorId = Id;
            var dt = UtilLocal.newTable<ProveedorContactosView>("PContactos", Datos.GetListOf<ProveedorContactosView>(pc => pc.ProveedorID.Equals(Id)));
            this.IniciarCarga(dt, "Contactos");
            Util.OcultarColumnas(this.dgvDatos, new string[] { "ProveedorContactoID", "ProveedorID" });
        }

        #endregion

    }
}
