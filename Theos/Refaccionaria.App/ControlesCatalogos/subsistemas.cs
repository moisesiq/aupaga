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
    public partial class subsistemas : ListadoBase
    {
        // Para el Singleton
        private static subsistemas _Instance;
        public static subsistemas Instance
        {
            get
            {
                if (subsistemas._Instance == null || subsistemas._Instance.IsDisposed)
                    subsistemas._Instance = new subsistemas();
                return subsistemas._Instance;
            }
        }
        //

        public subsistemas()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<SubsistemasView>("Subsistemas", Datos.GetListOf<SubsistemasView>(s => s.SubsistemaID > 0));
                this.IniciarCarga(dt, "Subsistemas");
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
            DetalleSubsistema s = new DetalleSubsistema();
            s.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleSubsistema s = new DetalleSubsistema(Util.Entero(this.dgvDatos.CurrentRow.Cells["SubsistemaID"].Value));
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
