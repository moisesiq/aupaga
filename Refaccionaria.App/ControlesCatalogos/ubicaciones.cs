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
    public partial class ubicaciones : ListadoBase
    {
        // Para el Singleton
        private static ubicaciones _Instance;
        public static ubicaciones Instance
        {
            get
            {
                if (ubicaciones._Instance == null || ubicaciones._Instance.IsDisposed)
                    ubicaciones._Instance = new ubicaciones();
                return ubicaciones._Instance;
            }
        }
        //

        public ubicaciones()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<PartesUbicacionView>("Ubicacion", Negocio.General.GetListOf<PartesUbicacionView>(l => l.ParteUbicacionID > 0));
                this.IniciarCarga(dt, "Ubicación");
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetalleUbicacion u = new DetalleUbicacion();
            u.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleUbicacion u = new DetalleUbicacion(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ParteUbicacionID"].Value));
            u.ShowDialog();
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
