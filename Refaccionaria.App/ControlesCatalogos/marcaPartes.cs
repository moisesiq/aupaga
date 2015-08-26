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
    public partial class marcaPartes : ListadoBase
    {
        // Para el Singleton
        private static marcaPartes _Instance;
        public static marcaPartes Instance
        {
            get
            {
                if (marcaPartes._Instance == null || marcaPartes._Instance.IsDisposed)
                    marcaPartes._Instance = new marcaPartes();
                return marcaPartes._Instance;
            }
        }
        //

        public marcaPartes()
        {
            InitializeComponent();            
        }

        #region [ Metodos]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<MarcaPartesView>("MarcaPartes", Negocio.General.GetListOf<MarcaPartesView>(m => m.MarcaParteID > 0));
                this.IniciarCarga(dt, "Marcas de las Partes");
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
            DetalleMarcaParte m = new DetalleMarcaParte();
            m.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleMarcaParte m = new DetalleMarcaParte(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["MarcaParteID"].Value));
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
