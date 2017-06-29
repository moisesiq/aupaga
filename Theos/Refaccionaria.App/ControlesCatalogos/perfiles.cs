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
    public partial class perfiles : ListadoBase
    {
        // Para el Singleton
        private static perfiles _Instance;
        public static perfiles Instance
        {
            get
            {
                if (perfiles._Instance == null || perfiles._Instance.IsDisposed)
                    perfiles._Instance = new perfiles();
                return perfiles._Instance;
            }
        }
        //

        public perfiles()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<PerfilesView>("Perfiles", Datos.GetListOf<PerfilesView>(p => p.PerfilID > 0));
                this.IniciarCarga(dt, "Perfiles");
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
            DetallePerfil p = new DetallePerfil();
            p.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetallePerfil p = new DetallePerfil(Util.Entero(this.dgvDatos.CurrentRow.Cells["PerfilID"].Value));
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
