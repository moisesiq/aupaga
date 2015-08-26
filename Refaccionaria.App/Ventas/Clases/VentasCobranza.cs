using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using FastReport;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public class VentasCobranza : VentaOpcion
    {
        public Cobranza ctlCobranza;
        public CobroCobranza ctlCobro;

        public VentasCobranza(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
            this.bExpandirContenido = true;
        }

        #region [ Base ]

        public override bool Activar()
        {
            // Se verifica el permiso para ver la opción de cobranza
            if (!UtilDatos.ValidarPermiso("Ventas.Cobranza.Ver", true))
                return false;

            return base.Activar();
        }

        public override void Inicializar()
        {
            base.Inicializar();

            // Se crean los objetos necesarios
            this.ctlCobranza = new Cobranza();
            this.ctlCobro = new CobroCobranza();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlCobranza.oVentasCobranza = this;
            this.ctlCobranza.CambiarCliente(this.Cliente);
            this.ctlCobro.CambiarCliente(this.Cliente.ClienteID);
            this.ctlCobro.HabilitarTipoDePago = false;
            this.ctlCobro.HabilitarRetroceso = false;

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnTotales.Controls.Add(this.ctlCobro);
            this.pnlEnContenido.Controls.Add(this.ctlCobranza);
        }

        public override bool Ejecutar()
        {
            // Se valida que el importe a pagar sea mayor que cero
            if (this.ctlCobranza.Total <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ningún importe a pagar.");
                return false;
            }

            // Se valida el cobro
            if (!this.ctlCobro.Validar())
                return false;

            // Se valida que el pago sea en la misma sucursal que el primer abono, si hubiera
            var oVentasACobrar = this.ctlCobranza.ObtenerVentasMarcadas();
            string sVentasOt = "";
            foreach (var oReg in oVentasACobrar)
            {
                if (General.Exists<VentaPago>(c => c.VentaID == oReg.VentaID && c.Estatus && c.SucursalID != GlobalClass.SucursalID))
                    sVentasOt += (", " + oReg.Folio);
            }
            if (sVentasOt != "" && oVentasACobrar[0].VentaID != 270157)  // Modificaciòn temporal para no evaluar esa venta - 2015-08-24
            {
                UtilLocal.MensajeAdvertencia("Las siguientes ventas ya fueron abonadas en otra sucursal, por lo tanto aquí no se pueden cobrar.\n\n"
                    + sVentasOt.Substring(2));
                return false;
            }

            // Si se quiere pagar con Vales, se valida que sólo sea una venta
            var oFormasDePago = this.ctlCobro.GenerarPagoDetalle();
            if (oFormasDePago.Any(c => c.TipoFormaPagoID == Cat.FormasDePago.Vale) && oVentasACobrar.Count > 1)
            {
                UtilLocal.MensajeAdvertencia("En selecciones múltiples no se puede usar vales. Es necesario seleccionar sólo una venta.");
                return false;
            }

            // Confirmación
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas realizar el abono indicado?") != DialogResult.Yes)
                return false;

            // Se completa el cobro, por si fue pago en efectivo
            if (!this.ctlCobro.CompletarCobro())
                return false;

            // Se solicita la validación de autorización, si aplica
            int iAutorizoID = 0;
            if (this.ctlCobro.AutorizacionDeNotasDeCreditoRequerida)
            {
                var Res = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.Cobro.NotaDeCreditoOtroCliente", "Autorización");
                iAutorizoID = (Res.Exito ? Res.Respuesta.UsuarioID : 0);
            }

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se generan los pagos para las ventas marcadas, hasta donde alcance el importe
            decimal mPago = this.ctlCobro.Suma;
            var oVentasAfectadas = new List<VentasACreditoView>();
            var oIdsPago = new List<int>();
            foreach (var oVentaACobrar in oVentasACobrar)
            {
                // Si la venta no tiene saldo, se salta
                if (oVentaACobrar.Restante <= 0) continue;

                var oPago = new VentaPago()
                {
                    VentaID = oVentaACobrar.VentaID,
                    Fecha = dAhora,
                };
                var oPagoDetalle = new List<VentaPagoDetalle>();
                decimal mPagoForma = 0;
                oVentaACobrar.Pagado = 0;  // Se hace cero para que sólo sume lo pagado en esta ocasión
                foreach (var oFormaDePago in oFormasDePago)
                {
                    mPagoForma = (oFormaDePago.Importe > oVentaACobrar.Restante ? oVentaACobrar.Restante : oFormaDePago.Importe);
                    if (oFormaDePago.Importe > 0)
                    {
                        oPagoDetalle.Add(new VentaPagoDetalle()
                        {
                            TipoFormaPagoID = oFormaDePago.TipoFormaPagoID,
                            Importe = mPagoForma,
                            BancoID = oFormaDePago.BancoID,
                            Folio = oFormaDePago.Folio,
                            Cuenta = oFormaDePago.Cuenta,
                            NotaDeCreditoID = oFormaDePago.NotaDeCreditoID
                        });

                        mPago -= mPagoForma;
                        oFormaDePago.Importe -= mPagoForma;
                        oVentaACobrar.Pagado += mPagoForma;
                        oVentaACobrar.Restante -= mPagoForma;
                        if (oVentaACobrar.Restante <= 0)
                            break;
                    }
                }
                Guardar.VentaPago(oPago, oPagoDetalle);

                // Se agrega la venta actual a las ventas afectadas
                oVentasAfectadas.Add(oVentaACobrar);
                oIdsPago.Add(oPago.VentaPagoID);

                if (mPago <= 0)
                    break;
            }

            // Se manda a generar la póliza contable (AfeConta)
            foreach (int iPagoID in oIdsPago)
            {
                var oPagoV = General.GetEntity<VentasPagosView>(c => c.VentaPagoID == iPagoID);
                if (oPagoV.Facturada)
                    ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoVentaCredito, iPagoID, oPagoV.Folio, oPagoV.Cliente);
            }

            // Se guardan la autorizaciones aplicables
            if (this.ctlCobro.AutorizacionDeNotasDeCreditoRequerida)
            {
                // Se agrega una autorización por cada nota de otro cliente
                var oNotasOC = this.ctlCobro.NotasDeCreditoOtrosClientes();
                foreach (var oNotaOC in oNotasOC)
                    VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.NotaDeCreditoOtroClienteUsar, Cat.Tablas.NotaDeCredito, oNotaOC, iAutorizoID);
            }

            // Se guardan los datos del cobro, para reimpresión de tickets
            string sFolioCob = VentasProc.GenerarFolioDeCobranza();
            int iCuenta = 0;
            foreach (var oVentaAf in oVentasAfectadas)
            {
                Guardar.Generico<CobranzaTicket>(new CobranzaTicket()
                {
                    Ticket = sFolioCob,
                    VentaID = oVentaAf.VentaID,
                    ClienteID = this.Cliente.ClienteID,
                    Folio = oVentaAf.Folio,
                    Fecha = oVentaAf.Fecha,
                    Vencimiento = oVentaAf.Vencimiento,
                    Total = oVentaAf.Total,
                    Pagado = oVentaAf.Pagado,
                    Restante = oVentaAf.Restante,
                    VentaPagoID = oIdsPago[iCuenta++]
                });
            }

            // Se genera el ticket correspondiente
            VentasProc.GenerarTicketCobranza(sFolioCob);

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            // Se limpia después de haberse guardado
            this.Limpiar();

            return true;
        }

        protected override void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;

            this.Cliente = this.oVenta.Cliente;
            this.ctlCobranza.CambiarCliente(this.Cliente);
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            this.ctlCobro.CambiarCliente(iClienteID);
        }

        #endregion
    }
}
