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
    public partial class sistemasCaracteristicas : ListadoBase
    {
        // Para el Singleton
        private static sistemasCaracteristicas _Instance;
        public static sistemasCaracteristicas Instance
        {
            get
            {
                if (sistemasCaracteristicas._Instance == null || sistemasCaracteristicas._Instance.IsDisposed)
                    sistemasCaracteristicas._Instance = new sistemasCaracteristicas();
                return sistemasCaracteristicas._Instance;
            }
        }
        //

        public sistemasCaracteristicas()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<PartesSistemaView>("Sistema", Datos.GetListOf<PartesSistemaView>(l => l.ParteSistemaID > 0));
                this.IniciarCarga(dt, "Sistema");
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetalleSistemaCaracteristica s = new DetalleSistemaCaracteristica();
            s.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleSistemaCaracteristica s = new DetalleSistemaCaracteristica(Util.Entero(this.dgvDatos.CurrentRow.Cells["ParteSistemaID"].Value));
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
