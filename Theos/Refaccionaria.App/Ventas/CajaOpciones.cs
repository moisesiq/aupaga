using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class CajaOpciones : UserControl
    {
        public CajaGeneral ctlGeneral;

        public CajaOpciones()
        {
            InitializeComponent();
        }

        private void btnFondoDeCaja_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpFondoDeCaja");
        }

        private void btnGastosIngresos_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpGastos");
        }

        private void btnModificarVenta_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpVentasCambios");
        }

        private void btnCambioDeEfectivo_Click(object sender, System.EventArgs e)
        {

        }

        private void btnCambioDeTurno_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpCambioTurno");
        }

        private void btnPagoDeNomina_Click(object sender, System.EventArgs e)
        {

        }

        private void btnVentasPorCobrar_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpVentasPorCobrar");
        }

        private void btnRefuerzo_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpRefuerzo");
        }

        private void btnResguardo_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpResguardo");
        }

        private void btnCorte_Click(object sender, System.EventArgs e)
        {
            this.ctlGeneral.CambiarOpcion("tbpCorte");
        }
    }
}