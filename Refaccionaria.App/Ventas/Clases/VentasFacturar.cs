using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public class VentasFacturar : VentaOpcion
    {
        public FacturarVentas ctlFacturar;
        public cVentaDetalle ctlDetalle;

        public VentasFacturar(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
        }

        #region [ Base ]

        public override void Inicializar()
        {
            base.Inicializar();

            // Se crean los objetos necesarios
            this.ctlDetalle = new cVentaDetalle();
            this.ctlFacturar = new FacturarVentas();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlFacturar.oManejador = this;

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnTotales.Controls.Add(this.ctlDetalle);
            this.pnlEnContenido.Controls.Add(this.ctlFacturar);

            // Se hace el cambio del cliente, para que se ejecuten los filtros, excepto cuando es "Ventas Mostrador"
            if (this.Cliente != null && this.Cliente.ClienteID != Cat.Clientes.Mostrador)
                this.ctlFacturar.CambiarCliente(this.Cliente);
        }

        public override bool Ejecutar()
        {
            // Se valida la opción
            if (!this.ctlFacturar.Validar())
                return false;

            // Se pregunta si se debe facturar al mismo cliente de las ventas o a otro
            int iAFClienteID = this.Cliente.ClienteID;
            if (UtilLocal.MensajePregunta("¿Deseas hacer la factura a nombre del cliente seleccionado?") == DialogResult.No)
            {
                iAFClienteID = 0;
                var frmValor = new MensajeObtenerValor("Selecciona el cliente para facturar:", "", MensajeObtenerValor.Tipo.Combo);
                frmValor.CargarCombo("ClienteID", "Nombre", General.GetListOf<Cliente>(q => q.ClienteID != Cat.Clientes.Mostrador && q.Estatus));
                if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
                    iAFClienteID = Helper.ConvertirEntero(frmValor.Valor);
                frmValor.Dispose();
            }
            if (iAFClienteID == 0)
                return false;

            // Se solicita el usuario que realiza el proceso
            int iUsuarioID = 0;
            var Res = UtilDatos.ValidarObtenerUsuario("Ventas.FacturarTickets.Agregar");
            if (Res.Error)
                return false;
            iUsuarioID = Res.Respuesta.UsuarioID;

            // Se muestra la ventana de "Cargando.."
            Cargando.Mostrar();
            
            // 
            DateTime dAhora = DateTime.Now;

            // Se obtienen los datos de los productos, por si se cambió la descripción
            List<ProductoVenta> oPartes = null;
            if (this.ctlFacturar.MostrarTodasLasPartes)
                oPartes = this.ctlDetalle.ObtenerListaVenta();

            // Se procede a generar la factura
            var oVentasAF = this.ctlFacturar.GenerarListaDeVentas();
            var ResFe = VentasProc.GenerarFacturaElectronica(oVentasAF, iAFClienteID, oPartes, null, this.ctlFacturar.Observacion, null);
            if (ResFe.Error)
            {
                UtilLocal.MensajeAdvertencia(ResFe.Mensaje);
                return false;
            }

            // Se guarda el dato de que fue una factura de ventas

            var oFactura = General.GetEntity<VentaFactura>(q => q.VentaFacturaID == ResFe.Respuesta && q.Estatus);
            oFactura.Convertida = true;
            oFactura.RealizoUsuarioID = iUsuarioID;
            Guardar.Generico<VentaFactura>(oFactura);

            // Se manda a afectar contabilidad (AfeConta)
            foreach (int iVentaID in oVentasAF)
            {
                var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == iVentaID);
                if (oVentaV.ACredito)
                {
                    ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.VentaCredito, iVentaID, (oFactura.Serie + oFactura.Folio), oVentaV.Cliente);
                    // Se verifica si el ticket ya está pagado, para hacer la póliza correspondiente a dicho pago
                    if (oVentaV.Pagado > 0)
                    {
                        var oPagos = General.GetListOf<VentaPago>(c => c.VentaID == iVentaID && c.Estatus);
                        foreach (var oReg in oPagos)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoVentaCredito, oReg.VentaPagoID
                                , ((oFactura.Serie + oFactura.Folio) + " / " + UtilDatos.VentaPagoFormasDePago(oReg.VentaPagoID))
                                , oVentaV.Cliente, oReg.SucursalID);
                    }

                    // Se borra la póliza temporal creada por ser ticket a crédito
                    ContaProc.BorrarPolizaTemporalTicketCredito(iVentaID);
                }
                else
                {
                    ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.VentaContadoFacturaConvertida, iVentaID
                        , (oFactura.Serie + oFactura.Folio), oVentaV.Cliente, oVentaV.SucursalID);
                }
            }

            // Se guarda el dato de pendiente por descontar en la factura global, si aplica
            // Se guardan datos cuando se están facturando tickets abonados en otras sucursales
            foreach (int iVentaID in oVentasAF)
            {
                // Se obtienen los abonos de otras sucursales
                var oAbonos = General.GetListOf<VentasPagosView>(c => c.VentaID == iVentaID && c.SucursalID != GlobalClass.SucursalID)
                    .GroupBy(c => new { c.VentaID, c.SucursalID }).Select(c => new { c.Key.VentaID, c.Key.SucursalID, Abonado = c.Sum(s => s.Importe) });
                if (oAbonos != null && oAbonos.Count() > 0)
                {
                    foreach (var oReg in oAbonos)
                    {
                        var oPendiente = new FacturaGlobalPendientePorDescontar()
                        {
                            VentaID = oReg.VentaID,
                            Fecha = dAhora,
                            SucursalID = oReg.SucursalID,
                            Importe = oReg.Abonado
                        };
                        Guardar.Generico<FacturaGlobalPendientePorDescontar>(oPendiente);
                    }
                }
            }

            // Se cierra la ventana de "Cargando.."
            Cargando.Cerrar();

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento ejecutado correctamente.");

            // Se limpia después de haberse guardado
            this.Limpiar();

            return true;
        }

        protected override void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;

            this.Cliente = this.oVenta.Cliente;
            this.ctlFacturar.CambiarCliente(this.Cliente);
        }

        #endregion
    }
}
