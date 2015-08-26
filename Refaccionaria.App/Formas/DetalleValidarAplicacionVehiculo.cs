using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class DetalleValidarAplicacionVehiculo : DetalleBase
    {
        public ParteVehiculo ParteV;

        public DetalleValidarAplicacionVehiculo(int iParteVehiculoID)
        {
            InitializeComponent();

            this.ParteV = General.GetEntity<ParteVehiculo>(q => q.ParteVehiculoID == iParteVehiculoID);
        }

        private void DetalleValidarAplicacionVehiculo_Load(object sender, EventArgs e)
        {
            this.cmbFuente.CargarDatos("TipoFuenteID", "NombreTipoFuente", General.GetListOf<TipoFuente>(q => q.Estatus && q.TipoFuenteID != Cat.TipoDeFuentes.Mostrador));
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            this.ParteV.TipoFuenteID = Helper.ConvertirEntero(this.cmbFuente.SelectedValue);
            Guardar.ParteVehiculo(this.ParteV);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
