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
    public partial class tipoOperaciones : ListadoBase
    {
        // Para el Singleton
        private static tipoOperaciones _Instance;
        public static tipoOperaciones Instance
        {
            get
            {
                if (tipoOperaciones._Instance == null || tipoOperaciones._Instance.IsDisposed)
                    tipoOperaciones._Instance = new tipoOperaciones();
                return tipoOperaciones._Instance;
            }
        }
        //

        public tipoOperaciones()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<TipoOperacionesView>("TipoOperaciones", Negocio.General.GetListOf<TipoOperacionesView>(t => t.TipoOperacionID > 0));
                this.IniciarCarga(dt, "Tipo de Operaciones");
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
