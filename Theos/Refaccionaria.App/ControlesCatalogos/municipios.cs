using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class municipios : ListadoBase
    {
        // Para el Singleton
        private static municipios _Instance;
        public static municipios Instance
        {
            get
            {
                if (municipios._Instance == null || municipios._Instance.IsDisposed)
                    municipios._Instance = new municipios();
                return municipios._Instance;
            }
        }
        //

        public municipios()
        {
            InitializeComponent();
        }


        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<MunicipiosView>("Municipios", Datos.GetListOf<MunicipiosView>().ToList());
                this.IniciarCarga(dt, "Municipios");
                Util.OcultarColumnas(base.dgvDatos, new string[] { "EstadoID" });
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
            DetalleMunicipio p = new DetalleMunicipio();
            p.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleMunicipio p = new DetalleMunicipio(Util.Entero(this.dgvDatos.CurrentRow.Cells["MunicipioID"].Value));
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
