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
    public partial class tipoPagos : ListadoBase
    {
        // Para el Singleton
        private static tipoPagos _Instance;
        public static tipoPagos Instance
        {
            get
            {
                if (tipoPagos._Instance == null || tipoPagos._Instance.IsDisposed)
                    tipoPagos._Instance = new tipoPagos();
                return tipoPagos._Instance;
            }
        }
        //

        public tipoPagos()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<TipoPagosView>("TipoPagos", Negocio.General.GetListOf<TipoPagosView>(t => t.TipoPagoID > 0));
                this.IniciarCarga(dt, "Tipo de Pagos");
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
