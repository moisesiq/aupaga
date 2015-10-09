using System;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ValCortes : UserControl
    {
        public ValCortes()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ValCortes_Load(object sender, EventArgs e)
        {
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.dtpDia.Value = DateTime.Now.AddDays(-1);
        }

        private void dtpDia_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            if (this.cmbSucursal.SelectedValue == null)
            {
                UtilLocal.MensajeAdvertencia("Debes seleccionar una sucursal.");
                return;
            }

            this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            DateTime dDia = this.dtpDia.Value.Date;
            var oCierre = General.GetEntity<CajaEfectivoPorDia>(c => c.Dia == dDia && c.SucursalID == iSucursalID && c.Estatus);
            var oDatos = General.GetListOf<CorteValidacionView>(c => c.Dia == dDia && c.SucursalID == iSucursalID).OrderBy(c => c.Concepto);
            var oFacturaGlobal = General.GetEntity<CajaFacturaGlobal>(c => c.Dia == dDia && c.SucursalID == iSucursalID);

            if (oCierre == null || oFacturaGlobal == null)
            {
                Cargando.Cerrar();
                UtilLocal.MensajeAdvertencia("No hay datos del cierre o factura global del día especificado.");
                return;
            }

            // Se llenan los datos del cierre
            this.lblCorteDeCaja.Text = oCierre.CierreDebeHaber.Valor().ToString(GlobalClass.FormatoMoneda);
            this.lblHuboEnCaja.Text = oCierre.Cierre.Valor().ToString(GlobalClass.FormatoMoneda);
            this.lblDiferencia.Text = (oCierre.Cierre - oCierre.CierreDebeHaber).Valor().ToString(GlobalClass.FormatoMoneda);
            this.lblDiferencia.ForeColor = ((oCierre.Cierre - oCierre.CierreDebeHaber) < 0 ? Color.Red : Color.White);

            // Se llenan las ventas
            this.dgvVentas.Rows.Clear();
            var oVentas = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.Ventas);
            var oFuenteT = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
            this.dgvVentas.Rows.Add("Ventas");
            this.dgvVentas.Rows[this.dgvVentas.Rows.Count - 1].DefaultCellStyle.Font = oFuenteT;
            var oTickets = new CorteValidacionView() { 
                Efectivo = 0, Cheque = 0, Tarjeta = 0, Transferencia = 0, Vale = 0, KardexImporte = 0, KardexCantidad = 0, ExistenciaCantidad = 0 };
            foreach (var oReg in oVentas)
            {
                if (oReg.Factura.Valor())
                {
                    this.dgvVentas.Rows.Add(oReg.Concepto, oReg.Efectivo, oReg.Cheque, oReg.Tarjeta, oReg.Transferencia, oReg.Vale, oReg.Importe
                        , oReg.ContaPolizaID, oReg.CuentaCaja, oReg.CuentaAnticipoClientes, (oReg.CuentaCaja + oReg.CuentaAnticipoClientes), oReg.CuentaInventario
                        , oReg.KardexImporte, oReg.KardexCantidad, Math.Abs(oReg.ExistenciaCantidad.Valor()), null);
                }
                else
                {
                    oTickets.Efectivo += oReg.Efectivo;
                    oTickets.Cheque += oReg.Cheque;
                    oTickets.Tarjeta += oReg.Tarjeta;
                    oTickets.Transferencia += oReg.Transferencia;
                    oTickets.Vale += oReg.Vale;
                    oTickets.Importe += oReg.Importe;
                    oTickets.KardexImporte += oReg.KardexImporte;
                    oTickets.KardexCantidad += oReg.KardexCantidad;
                    oTickets.ExistenciaCantidad += Math.Abs(oReg.ExistenciaCantidad.Valor());
                }
            }
            this.dgvVentas.Rows.Add("Tickets", oTickets.Efectivo, oTickets.Cheque, oTickets.Tarjeta, oTickets.Transferencia, oTickets.Vale, oTickets.Importe
                , null, null, null, null, null, oTickets.KardexImporte, oTickets.KardexCantidad, oTickets.ExistenciaCantidad, oFacturaGlobal.Tickets);
            // Se llenan las devoluciones del día
            var oDevolucionesDia = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.CancelacionesDia);
            this.dgvVentas.Rows.Add("Canc. día");
            this.dgvVentas.Rows[this.dgvVentas.Rows.Count - 1].DefaultCellStyle.Font = oFuenteT;
            oTickets = new CorteValidacionView() { 
                Efectivo = 0, Cheque = 0, Tarjeta = 0, Transferencia = 0, Vale = 0, KardexImporte = 0, KardexCantidad = 0, ExistenciaCantidad = 0 };
            foreach (var oReg in oDevolucionesDia)
            {
                if (oReg.Factura.Valor())
                {
                this.dgvVentas.Rows.Add(oReg.Concepto, oReg.Efectivo, oReg.Cheque, oReg.Tarjeta, oReg.Transferencia, oReg.Vale, oReg.Importe
                    , oReg.ContaPolizaID, oReg.CuentaCaja, oReg.CuentaAnticipoClientes, (oReg.CuentaCaja + oReg.CuentaAnticipoClientes), oReg.CuentaInventario
                    , oReg.KardexImporte, oReg.KardexCantidad, oReg.ExistenciaCantidad, null);
                }
                else
                {
                    oTickets.Efectivo += oReg.Efectivo;
                    oTickets.Cheque += oReg.Cheque;
                    oTickets.Tarjeta += oReg.Tarjeta;
                    oTickets.Transferencia += oReg.Transferencia;
                    oTickets.Vale += oReg.Vale;
                    oTickets.Importe += oReg.Importe;
                    oTickets.KardexImporte += oReg.KardexImporte;
                    oTickets.KardexCantidad += oReg.KardexCantidad;
                    oTickets.ExistenciaCantidad += Math.Abs(oReg.ExistenciaCantidad.Valor());
                }
            }
            this.dgvVentas.Rows.Add("Tickets", oTickets.Efectivo, oTickets.Cheque, oTickets.Tarjeta, oTickets.Transferencia, oTickets.Vale, oTickets.Importe
                , null, null, null, null, null, oTickets.KardexImporte, oTickets.KardexCantidad, oTickets.ExistenciaCantidad, oFacturaGlobal.DevolucionesDia);
            // Se llenan las devoluciones de días anteriores
            var oDevolucionesDiasAnt = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.CancelacionesDiasAnteriores);
            this.dgvVentas.Rows.Add("Canc. ant.");
            this.dgvVentas.Rows[this.dgvVentas.Rows.Count - 1].DefaultCellStyle.Font = oFuenteT;
            oTickets = new CorteValidacionView() { 
                Efectivo = 0, Cheque = 0, Tarjeta = 0, Transferencia = 0, Vale = 0, KardexImporte = 0, KardexCantidad = 0, ExistenciaCantidad = 0 };
            foreach (var oReg in oDevolucionesDiasAnt)
            {
                if (oReg.Factura.Valor())
                {
                this.dgvVentas.Rows.Add(oReg.Concepto, oReg.Efectivo, oReg.Cheque, oReg.Tarjeta, oReg.Transferencia, oReg.Vale, oReg.Importe
                    , oReg.ContaPolizaID, oReg.CuentaCaja, oReg.CuentaAnticipoClientes, (oReg.CuentaCaja + oReg.CuentaAnticipoClientes), oReg.CuentaInventario
                    , oReg.KardexImporte, oReg.KardexCantidad, oReg.ExistenciaCantidad, null);
                }
                else
                {
                    oTickets.Efectivo += oReg.Efectivo;
                    oTickets.Cheque += oReg.Cheque;
                    oTickets.Tarjeta += oReg.Tarjeta;
                    oTickets.Transferencia += oReg.Transferencia;
                    oTickets.Vale += oReg.Vale;
                    oTickets.Importe += oReg.Importe;
                    oTickets.KardexImporte += oReg.KardexImporte;
                    oTickets.KardexCantidad += oReg.KardexCantidad;
                    oTickets.ExistenciaCantidad += Math.Abs(oReg.ExistenciaCantidad.Valor());
                }
            }
            this.dgvVentas.Rows.Add("Tickets", oTickets.Efectivo, oTickets.Cheque, oTickets.Tarjeta, oTickets.Transferencia, oTickets.Vale, oTickets.Importe
                , null, null, null, null, null, oTickets.KardexImporte, oTickets.KardexCantidad, oTickets.ExistenciaCantidad, oFacturaGlobal.DevolucionesDiasAnt);
            // Se llenan las garantías del día
            var oGarantiasDia = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.GarantiasDia);
            this.dgvVentas.Rows.Add("Gar. día");
            this.dgvVentas.Rows[this.dgvVentas.Rows.Count - 1].DefaultCellStyle.Font = oFuenteT;
            oTickets = new CorteValidacionView() { 
                Efectivo = 0, Cheque = 0, Tarjeta = 0, Transferencia = 0, Vale = 0, KardexImporte = 0, KardexCantidad = 0, ExistenciaCantidad = 0 };
            foreach (var oReg in oGarantiasDia)
            {
                if (oReg.Factura.Valor())
                {
                this.dgvVentas.Rows.Add(oReg.Concepto, oReg.Efectivo, oReg.Cheque, oReg.Tarjeta, oReg.Transferencia, oReg.Vale, oReg.Importe
                    , oReg.ContaPolizaID, oReg.CuentaCaja, oReg.CuentaAnticipoClientes, (oReg.CuentaCaja + oReg.CuentaAnticipoClientes), oReg.CuentaInventario
                    , oReg.KardexImporte, oReg.KardexCantidad, oReg.ExistenciaCantidad, null);
                }
                else
                {
                    oTickets.Efectivo += oReg.Efectivo;
                    oTickets.Cheque += oReg.Cheque;
                    oTickets.Tarjeta += oReg.Tarjeta;
                    oTickets.Transferencia += oReg.Transferencia;
                    oTickets.Vale += oReg.Vale;
                    oTickets.Importe += oReg.Importe;
                    oTickets.KardexImporte += oReg.KardexImporte;
                    oTickets.KardexCantidad += oReg.KardexCantidad;
                    oTickets.ExistenciaCantidad += Math.Abs(oReg.ExistenciaCantidad.Valor());
                }
            }
            this.dgvVentas.Rows.Add("Tickets", oTickets.Efectivo, oTickets.Cheque, oTickets.Tarjeta, oTickets.Transferencia, oTickets.Vale, oTickets.Importe
                , null, null, null, null, null, oTickets.KardexImporte, oTickets.KardexCantidad, oTickets.ExistenciaCantidad, oFacturaGlobal.GarantiasDia);
            // Se llenan las garantías de días anteriores
            var oGarantiasDiasAnt = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.GarantiasDiasAnteriores);
            this.dgvVentas.Rows.Add("Gar. ant.");
            this.dgvVentas.Rows[this.dgvVentas.Rows.Count - 1].DefaultCellStyle.Font = oFuenteT;
            oTickets = new CorteValidacionView() { 
                Efectivo = 0, Cheque = 0, Tarjeta = 0, Transferencia = 0, Vale = 0, KardexImporte = 0, KardexCantidad = 0, ExistenciaCantidad = 0 };
            foreach (var oReg in oGarantiasDiasAnt)
            {
                if (oReg.Factura.Valor())
                {
                this.dgvVentas.Rows.Add(oReg.Concepto, oReg.Efectivo, oReg.Cheque, oReg.Tarjeta, oReg.Transferencia, oReg.Vale, oReg.Importe
                    , oReg.ContaPolizaID, oReg.CuentaCaja, oReg.CuentaAnticipoClientes, (oReg.CuentaCaja + oReg.CuentaAnticipoClientes), oReg.CuentaInventario
                    , oReg.KardexImporte, oReg.KardexCantidad, oReg.ExistenciaCantidad, null);
                }
                else
                {
                    oTickets.Efectivo += oReg.Efectivo;
                    oTickets.Cheque += oReg.Cheque;
                    oTickets.Tarjeta += oReg.Tarjeta;
                    oTickets.Transferencia += oReg.Transferencia;
                    oTickets.Vale += oReg.Vale;
                    oTickets.Importe += oReg.Importe;
                    oTickets.KardexImporte += oReg.KardexImporte;
                    oTickets.KardexCantidad += oReg.KardexCantidad;
                    oTickets.ExistenciaCantidad += Math.Abs(oReg.ExistenciaCantidad.Valor());
                }
            }
            this.dgvVentas.Rows.Add("Tickets", oTickets.Efectivo, oTickets.Cheque, oTickets.Tarjeta, oTickets.Transferencia, oTickets.Vale, oTickets.Importe
                , null, null, null, null, null, oTickets.KardexImporte, oTickets.KardexCantidad, oTickets.ExistenciaCantidad, oFacturaGlobal.GarantiasDiasAnt);

            // Se llena la cobranza
            this.dgvCobranza.Rows.Clear();
            var oCobranza = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.Cobranza);
            oTickets = new CorteValidacionView() { 
                Efectivo = 0, Cheque = 0, Tarjeta = 0, Transferencia = 0, Vale = 0, KardexImporte = 0, KardexCantidad = 0, ExistenciaCantidad = 0 };
            foreach (var oReg in oCobranza)
            {
                if (oReg.Factura.Valor())
                {
                this.dgvCobranza.Rows.Add(oReg.Concepto, oReg.Efectivo, oReg.Cheque, oReg.Tarjeta, oReg.Transferencia, oReg.Vale, oReg.Importe
                    , oReg.ContaPolizaID, oReg.CuentaCaja, oReg.CuentaAnticipoClientes, (oReg.CuentaCaja + oReg.CuentaAnticipoClientes), oReg.CuentaInventario
                    , null);
                }
                else
                {
                    oTickets.Efectivo += oReg.Efectivo;
                    oTickets.Cheque += oReg.Cheque;
                    oTickets.Tarjeta += oReg.Tarjeta;
                    oTickets.Transferencia += oReg.Transferencia;
                    oTickets.Vale += oReg.Vale;
                    oTickets.Importe += oReg.Importe;
                }
            }
            this.dgvCobranza.Rows.Add("Tickets", oTickets.Efectivo, oTickets.Cheque, oTickets.Tarjeta, oTickets.Transferencia, oTickets.Vale, oTickets.Importe
                , null, null, null, null, null, oFacturaGlobal.Cobranza);

            // Se llenan los gastos
            this.dgvGastos.Rows.Clear();
            var oGastos = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.Gastos);
            foreach (var oReg in oGastos)
            {
                // Se obtiene la cuenta auxiliar correspondiente,
                var oEgresoV = General.GetEntity<CajaEgresosView>(c => c.CajaEgresoID == oReg.RelacionID);

                this.dgvGastos.Rows.Add(oReg.RelacionID, oReg.Importe, oEgresoV.CuentaAuxiliar, (oEgresoV.AfectadoEnPolizas == true ? null : (decimal?)oReg.Importe)
                    , oReg.ContaPolizaID, oReg.CuentaCaja);
            }

            // Se llenan los vales creados
            this.dgvVales.Rows.Clear();
            var oVales = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.ValesCreados);
            foreach (var oReg in oVales)
            {
                int iFila = this.dgvVales.Rows.Add(oReg.RelacionID, oReg.Importe, oReg.ContaPolizaID, oReg.CuentaAnticipoClientes);
                // Si no hay póliza, se revisa si fue por devolución o garantía
                if (!oReg.ContaPolizaID.HasValue)
                {
                    var oVale = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == oReg.RelacionID && c.Estatus);
                    ContaPolizasDetalleAvanzadoView oPolizaCuenta = null;
                    switch (oVale.OrigenID)
                    {
                        case Cat.OrigenesNotaDeCredito.Devolucion:
                            var oDevV = General.GetEntity<VentasDevolucionesView>(c => c.NotaDeCreditoID == oVale.NotaDeCreditoID);
                            oPolizaCuenta = General.GetEntity<ContaPolizasDetalleAvanzadoView>(c => c.RelacionTabla == Cat.Tablas.VentaDevolucion 
                                && c.RelacionID == oDevV.VentaDevolucionID && c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.AnticipoClientes);
                            break;
                        case Cat.OrigenesNotaDeCredito.Garantia:
                            var oGarV = General.GetEntity<VentasGarantiasView>(c => c.NotaDeCreditoID == oVale.NotaDeCreditoID);
                            oPolizaCuenta = General.GetEntity<ContaPolizasDetalleAvanzadoView>(c => c.RelacionTabla == Cat.Tablas.VentaGarantia 
                                && c.RelacionID == oGarV.VentaGarantiaID && c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.AnticipoClientes);
                            break;
                    }
                    if (oPolizaCuenta != null)
                    {
                        this.dgvVales["val_Poliza", iFila].Value = oPolizaCuenta.ContaPolizaID;
                        this.dgvVales["val_CuentaAnticipoClientes", iFila].Value = oPolizaCuenta.Abono;
                    }
                }
            }

            // Se llenan los refuerzos
            this.dgvRefuerzos.Rows.Clear();
            var oRefuerzos = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.Refuerzos);
            foreach (var oReg in oRefuerzos)
                this.dgvRefuerzos.Rows.Add(oReg.RelacionID, oReg.Importe, oReg.ContaPolizaID, oReg.CuentaCaja);

            // Se llenan los resguardos
            this.dgvResguardos.Rows.Clear();
            var oResguardos = oDatos.Where(c => c.CorteCategoriaID == Cat.CategoriasCorte.Resguardos);
            foreach (var oReg in oResguardos)
                this.dgvResguardos.Rows.Add(oReg.RelacionID, oReg.Importe, oReg.ContaPolizaID, oReg.CuentaCaja);

            Cargando.Cerrar();
        }

        #endregion

    }
}
