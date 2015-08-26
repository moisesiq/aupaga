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
                var dt = Negocio.Helper.newTable<SubsistemasView>("Subsistemas", Negocio.General.GetListOf<SubsistemasView>(s => s.SubsistemaID > 0));
                this.IniciarCarga(dt, "Subsistemas");
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
            DetalleSubsistema s = new DetalleSubsistema();
            s.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleSubsistema s = new DetalleSubsistema(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["SubsistemaID"].Value));
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
