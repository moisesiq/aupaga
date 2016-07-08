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
    public partial class sistemas : ListadoBase
    {
        // Para el Singleton
        private static sistemas _Instance;
        public static sistemas Instance
        {
            get
            {
                if (sistemas._Instance == null || sistemas._Instance.IsDisposed)
                    sistemas._Instance = new sistemas();
                return sistemas._Instance;
            }
        }
        //

        public sistemas()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<SistemasView>("Sistemas", Datos.GetListOf<SistemasView>(s => s.SistemaID > 0));
                this.IniciarCarga(dt, "Sistemas");
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
            DetalleSistema s = new DetalleSistema();
            s.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleSistema s = new DetalleSistema(Util.Entero(this.dgvDatos.CurrentRow.Cells["SistemaID"].Value));
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
