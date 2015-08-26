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
    public partial class tipoMovimientos : ListadoBase
    {
        // Para el Singleton
        private static tipoMovimientos _Instance;
        public static tipoMovimientos Instance
        {
            get
            {
                if (tipoMovimientos._Instance == null || tipoMovimientos._Instance.IsDisposed)
                    tipoMovimientos._Instance = new tipoMovimientos();
                return tipoMovimientos._Instance;
            }
        }
        //

        public tipoMovimientos()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<TipoMovimientosView>("TipoMovimientos", Negocio.General.GetListOf<TipoMovimientosView>(t => t.TipoMovimientoID > 0));
                this.IniciarCarga(dt, "Tipo de Movimientos");
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
