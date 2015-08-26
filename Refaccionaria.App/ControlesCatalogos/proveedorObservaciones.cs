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
    public partial class proveedorObservaciones : ListadoBase
    {
        private int proveedorId = 0;

        public static proveedorObservaciones Instance
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

            internal static readonly proveedorObservaciones instance = new proveedorObservaciones();
        }

        public proveedorObservaciones()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetalleProveedorObservacion po = new DetalleProveedorObservacion();
            po.ProveedorId = proveedorId;
            po.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleProveedorObservacion po = new DetalleProveedorObservacion(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ProveedorObservacionID"].Value));
            po.ShowDialog();
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
            var dt = Negocio.Helper.newTable<ProveedorObservacionesView>("PObservaciones", Negocio.General.GetListOf<ProveedorObservacionesView>(po => po.ProveedorID.Equals(Id)));
            this.IniciarCarga(dt, "Observaciones");
            Negocio.Helper.OcultarColumnas(this.dgvDatos, new string[] { "ProveedorObservacionID", "ProveedorID" });
        }

        #endregion

    }
}
