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
    public partial class medidas : ListadoBase
    {
        // Para el Singleton
        private static medidas _Instance;
        public static medidas Instance
        {
            get
            {
                if (medidas._Instance == null || medidas._Instance.IsDisposed)
                    medidas._Instance = new medidas();
                return medidas._Instance;
            }
        }
        //

        public medidas()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<MedidasView>("Medidas", Negocio.General.GetListOf<MedidasView>(m => m.MedidaID > 0));
                this.IniciarCarga(dt, "Medidas");
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
            DetalleMedida m = new DetalleMedida();
            m.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleMedida m = new DetalleMedida(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["MedidaID"].Value));
            m.ShowDialog();
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
