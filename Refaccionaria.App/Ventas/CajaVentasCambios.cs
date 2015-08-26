using System;
using System.Windows.Forms;
using System.Data.Objects;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CajaVentasCambios : UserControl
    {
        public VentasCaja oVentasCaja;

        public Cobro ctlCobro;

        public CajaVentasCambios()
        {
            InitializeComponent();
        }

        public int VentaID;
        public int VentaPagoID;

        #region [ Eventos ]

        private void CajaVentasCambios_Load(object sender, EventArgs e)
        {
            // Se carga la parte del cobro
            this.ctlCobro = new Cobro();
            this.ctlCobro.HabilitarTipoDePago = false;
            this.ctlCobro.MostrarFacturar = false;
            this.ctlCobro.HabilitarRetroceso = false;
            this.oVentasCaja.pnlEnTotales.Controls.Add(this.ctlCobro);
            
            // Se llenan los datos
            this.ActualizarDatos();
        }
        
        private void dgvDatos_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (!e.Row.Selected) return;
            
            // Se mandan llenar los datos de la ventana de cobro.
            this.VentaID = Helper.ConvertirEntero(e.Row.Cells["colVentaID"].Value);
            this.VentaPagoID = Helper.ConvertirEntero(e.Row.Cells["colVentaPagoID"].Value);
            this.ctlCobro.LlenarDatosGenerales(this.VentaID);
            this.ctlCobro.LlenarDatosFormasDePago(this.VentaPagoID);

            // Se verifica si es factura, en cuyo caso, se deshabilitan las formas de pago
            this.ctlCobro.HabilitarFormasDePago = (!Helper.ConvertirBool(e.Row.Cells["EsFactura"].Value));
        }

        #endregion

        public void ActualizarDatos() {
            // Se cargan las ventas pendientes en el Grid
            DateTime dHoy = DateTime.Today;
            var oPagos = General.GetListOf<VentasPagosView>(q => q.SucursalID == GlobalClass.SucursalID 
                && (q.VentaEstatusID != Cat.VentasEstatus.Cancelada && q.VentaEstatusID != Cat.VentasEstatus.CanceladaSinPago)
                && EntityFunctions.TruncateTime(q.Fecha) == dHoy && q.Importe > 0).OrderBy(q => q.Folio);

            this.dgvDatos.Rows.Clear();
            foreach (var oPago in oPagos)
                this.dgvDatos.Rows.Add(oPago.VentaPagoID, oPago.VentaID, oPago.ClienteID, oPago.Facturada, oPago.Folio, oPago.Cliente, oPago.Vendedor, oPago.Importe);
        }
    }
}
