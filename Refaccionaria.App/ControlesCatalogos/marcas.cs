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
    public partial class marcas : ListadoBase
    {
        // Para el Singleton
        private static marcas _Instance;
        public static marcas Instance
        {
            get
            {
                if (marcas._Instance == null || marcas._Instance.IsDisposed)
                    marcas._Instance = new marcas();
                return marcas._Instance;
            }
        }
        //

        public marcas()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<MarcasView>("Marcas", Negocio.General.GetListOf<MarcasView>(m => m.MarcaID > 0));
                this.IniciarCarga(dt, "Marcas");
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
            DetalleMarca m = new DetalleMarca();
            m.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleMarca m = new DetalleMarca(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["MarcaID"].Value));
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
