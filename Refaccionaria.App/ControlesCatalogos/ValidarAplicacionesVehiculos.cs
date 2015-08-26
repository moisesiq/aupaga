using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ValidarAplicacionesVehiculos : ListadoSimple
    {
        // Para el Singleton
        private static ValidarAplicacionesVehiculos _Instance;
        public static ValidarAplicacionesVehiculos Instance
        {
            get
            {
                if (ValidarAplicacionesVehiculos._Instance == null || ValidarAplicacionesVehiculos._Instance.IsDisposed)
                    ValidarAplicacionesVehiculos._Instance = new ValidarAplicacionesVehiculos();
                return ValidarAplicacionesVehiculos._Instance;
            }
        }
        //

        public ValidarAplicacionesVehiculos()
        {
            InitializeComponent();
        }

        private void ValidarAplicacionesVehiculos_Load(object sender, EventArgs e)
        {
            var Datos = Helper.newTable<PartesVehiculosBasicoView>("", General.GetListOf<PartesVehiculosBasicoView>(q => q.TipoFuenteID == Cat.TipoDeFuentes.Mostrador));
            Datos.Columns.RemoveAt(Datos.Columns.Count - 1);
            Datos.Columns.RemoveAt(Datos.Columns.Count - 1);
            base.CargarDatos(Datos);
            Helper.OcultarColumnas(this.dgvDatos, "ParteVehiculoID", "ParteID", "TipoFuenteID");
            Helper.ColumnasToHeaderText(this.dgvDatos);
        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.btnValidar_Click(sender, e);
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;

            DetalleValidarAplicacionVehiculo frmValidar = new DetalleValidarAplicacionVehiculo((int)this.dgvDatos.CurrentRow.Cells["ParteVehiculoID"].Value);
            if (frmValidar.ShowDialog() == DialogResult.OK)
            {
                var Fuente = General.GetEntity<TipoFuente>(q => q.TipoFuenteID == frmValidar.ParteV.TipoFuenteID);
                this.dgvDatos.CurrentRow.Cells["Fuente"].Value = (Fuente == null ? "" : Fuente.NombreTipoFuente);
                this.dgvDatos.AutoResizeColumns();
            }
            frmValidar.Dispose();
        }
    }
}
