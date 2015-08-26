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
                var dt = Negocio.Helper.newTable<SistemasView>("Sistemas", Negocio.General.GetListOf<SistemasView>(s => s.SistemaID > 0));
                this.IniciarCarga(dt, "Sistemas");
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
            DetalleSistema s = new DetalleSistema();
            s.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleSistema s = new DetalleSistema(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["SistemaID"].Value));
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
