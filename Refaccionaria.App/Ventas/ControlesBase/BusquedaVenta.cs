using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class BusquedaVenta : UserControl
    {
        protected ControlError ctlError = new ControlError();
        protected ClientesDatosView Cliente;

        public int VentaID { get; set; }

        public BusquedaVenta()
        {
            InitializeComponent();
            this.cmbSucursal.TextChanged += new EventHandler(cmbSucursal_TextChanged);
        }

        #region [ Eventos ]

        private void BusquedaVenta_Load(object sender, System.EventArgs e)
        {
            if (this.DesignMode) return;

            // Se cargan los datos de las Sucursales
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(q => q.Estatus));
            this.cmbSucursal.SelectedValue = GlobalClass.SucursalID;
            // Se especifican las fechas predeterminadas
            this.dtpInicio.Value = DateTime.Now.DiaPrimero();
            this.dtpFin.Value = DateTime.Now.DiaUltimo();

            // Se aplica el filtro al cargar el control
            //this.AplicarFiltro();
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused)
                this.AplicarFiltro();
        }

        private void cmbSucursal_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused && this.cmbSucursal.Text == "")
                this.AplicarFiltro();
        }

        private void dtpInicio_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpInicio.Focused)
                this.AplicarFiltro();
        }

        private void dtpFin_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpFin.Focused)
                this.AplicarFiltro();
        }

        private void rdbTicket_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbTicket.Checked)
                this.AplicarFiltro();
        }

        private void rdbFactura_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbFactura.Checked)
                this.AplicarFiltro();
        }

        private void txtFolio_TextChanged(object sender, EventArgs e)
        {
            this.AplicarFiltro();
        }
        
        private void chkMostrarTodasLasVentas_CheckedChanged(object sender, EventArgs e)
        {
            this.AplicarFiltro();
        }

        private void dgvVentas_CurrentCellChanged(object sender, EventArgs e)
        {
            // Se actualizan los datos
            this.VentaID = (this.dgvVentas.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvVentas.CurrentRow.Cells["colVentaID"].Value));
            this.MostrarDatosDeVenta(this.VentaID);
        }

        #endregion

        #region [ Métodos ]

        protected virtual void AplicarFiltro()
        {
            // Se muestra la ventana de "Cargando.."
            if (this.txtFolio.Text == "")
                Cargando.Mostrar();

            //
            string sFolio = this.txtFolio.Text;
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            bool bFacturadas = this.rdbFactura.Checked;
            DateTime dInicio = this.dtpInicio.Value.Date;
            DateTime dFin = this.dtpFin.Value.Date.AddDays(1);
            bool bFiltroMinimo = (this.chkMostrarTodasLasVentas.Checked);

            List<VentasView> Lista;

            if (sFolio == "")
            {
                Lista = General.GetListOf<VentasView>(q =>
                    (bFiltroMinimo || iClienteID == 0 || q.ClienteID == iClienteID)
                    && (bFiltroMinimo || iSucursalID == 0 || q.SucursalID == iSucursalID)
                    && q.Fecha >= dInicio
                    && q.Fecha < dFin
                    && (bFiltroMinimo || q.Facturada == bFacturadas)
                    && q.VentaEstatusID != Cat.VentasEstatus.Realizada
                );
            }
            else
            {
                Lista = General.GetListOf<VentasView>(q => (q.Folio == sFolio || q.FolioIni == sFolio) && (iSucursalID == 0 || q.SucursalID == iSucursalID));
            }

            // Se manda llenar el Grid
            this.LlenarGrid(Lista);

            // Se cierra la ventana de "Cargando.."
            if (this.txtFolio.Text == "")
                Cargando.Cerrar();
        }

        protected virtual void LlenarGrid(List<VentasView> oVentas)
        {
            this.dgvVentas.Rows.Clear();
            foreach (var oVenta in oVentas)
                this.dgvVentas.Rows.Add(oVenta.VentaID, oVenta.Fecha, oVenta.Cliente, oVenta.Folio, oVenta.Total);
        }

        protected virtual void MostrarDatosDeVenta(int iVentaID)
        {
            // Se limpian los datos primero
            this.rdbContado.Checked = false;
            this.rdbCredito.Checked = false;
            // this.rdbContado.Checked = false;
            this.txtAbonos.Clear();
            this.txtEfectivo.Clear();
            this.txtCheque.Clear();
            this.txtTarjeta.Clear();
            this.txtTransferencia.Clear();
            this.txtNoIdentificado.Clear();
            this.txtNotaDeCredito.Clear();
            this.txtVendedor.Clear();

            // Se muestran los datos de la venta seleccionada
            var oVentaV = General.GetEntity<VentasView>(q => q.VentaID == iVentaID);
            var oVentaPagos = General.GetListOf<VentasPagosDetalleAvanzadoView>(q => q.VentaID == iVentaID);
            if (oVentaV == null)
                return;

            // Para mostrar los detalles de pago
            this.rdbContado.Checked = !oVentaV.ACredito;
            this.rdbCredito.Checked = oVentaV.ACredito;
            if (oVentaPagos.Count > 0)
            {
                if (this.rdbCredito.Checked)
                    this.txtAbonos.Text = (oVentaPagos.Count > 0 ? oVentaPagos.Sum(c => c.Importe) : 0).ToString(GlobalClass.FormatoMoneda);
                        //oVentaPagos[0].ImporteTotal.Valor().ToString(GlobalClass.FormatoMoneda);
                foreach (var oFormaPago in oVentaPagos)
                {
                    switch (oFormaPago.FormaDePagoID)
                    {
                        case Cat.FormasDePago.Efectivo:
                            this.txtEfectivo.Text = oFormaPago.Importe.ToString(GlobalClass.FormatoMoneda);
                            break;
                        case Cat.FormasDePago.Cheque:
                            this.txtCheque.Text = oFormaPago.Importe.ToString(GlobalClass.FormatoMoneda);
                            break;
                        case Cat.FormasDePago.Tarjeta:
                            this.txtTarjeta.Text = oFormaPago.Importe.ToString(GlobalClass.FormatoMoneda);
                            break;
                        case Cat.FormasDePago.Transferencia:
                            this.txtTransferencia.Text = oFormaPago.Importe.ToString(GlobalClass.FormatoMoneda);
                            break;
                        case Cat.FormasDePago.NoIdentificado:
                            this.txtNoIdentificado.Text = oFormaPago.Importe.ToString(GlobalClass.FormatoMoneda);
                            break;
                        case Cat.FormasDePago.Vale:
                            if (this.txtNotaDeCredito.Text == "")
                                this.txtNotaDeCredito.Text = (oFormaPago.NotaDeCreditoID.Valor().ToString() + oFormaPago.Importe.ToString(GlobalClass.FormatoMoneda));
                            else
                                this.txtNotaDeCredito.Text += (", " + oFormaPago.NotaDeCreditoID.Valor().ToString() 
                                    + oFormaPago.Importe.ToString(GlobalClass.FormatoMoneda));
                            break;
                    }
                }
                this.txtVendedor.Text = oVentaPagos[0].Vendedor;
            }
        }

        #endregion

        #region [ Públicos ]

        public void CambiarCliente(ClientesDatosView oCliente)
        {
            this.Cliente = oCliente;
            this.AplicarFiltro();
        }
        
        #endregion
               
    }
}
