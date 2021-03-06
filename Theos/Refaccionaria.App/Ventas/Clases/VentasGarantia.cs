﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FastReport;
using System.Text;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public class VentasGarantia : VentaOpcion
    {
        public BusquedaGarantia ctlBusqueda;
        public DetalleDevolucion ctlDetalle;

        public VentasGarantia(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
            this.bExpandirContenido = true;
        }

        #region [ Base ]

        public override void Inicializar()
        {
            base.Inicializar();

            // Se crean los objetos necesarios
            this.ctlDetalle = new DetalleDevolucion();
            this.ctlBusqueda = new BusquedaGarantia();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlBusqueda.oGarantia = this;

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnTotales.Controls.Add(this.ctlDetalle);
            this.pnlEnContenido.Controls.Add(this.ctlBusqueda);

            // Se hace el cambio del cliente, para que se ejecuten los filtros, excepto cuando es "Ventas Mostrador"
            if (this.Cliente != null && this.Cliente.ClienteID != Cat.Clientes.Mostrador)
                this.ctlBusqueda.CambiarCliente(this.Cliente);
        }

        public override bool Ejecutar()
        {
            // Se verifica si ya se hizo el cierre de caja
            if (UtilDatos.VerCierreDeDaja())
            {
                UtilLocal.MensajeAdvertencia("Ya se hizo el Corte de Caja. No se puede continuar.");
                return false;
            }

            // Se realiza la acción según el caso
            bool bResultado;
            if (this.ctlBusqueda.GarantiaNueva)
                bResultado = this.AgregarGarantia();
            else
                bResultado = this.CompletarGarantia();

            if (!bResultado)
                return false;

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Garantía guardada correctamente.");

            // Se limpia después de haberse guardado
            this.Limpiar();

            return true;
        }

        protected override void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;

            this.Cliente = this.oVenta.Cliente;
            this.ctlBusqueda.CambiarCliente(this.Cliente);
        }

        #endregion

        #region [ Uso interno ]

        private bool AgregarGarantia()
        {
            // Se valida la parte de detalle
            if (!this.ctlDetalle.Validar())
                return false;
            // Se valida la parte de búsqueda
            if (!this.ctlBusqueda.Validar())
                return false;
            // Se valida que sólo esté seleccionada una parte
            if (this.ctlDetalle.ProductosSel().Count > 1)
            {
                UtilLocal.MensajeAdvertencia("Debes seleccionar sólo un producto al hacer una garantía.");
                return false;
            }

            // Se verifica si se creará vale, para pedir el cliente en caso de que no haya
            var oGarantia = this.ctlBusqueda.GenerarGarantia();
            int iVentaID = this.ctlBusqueda.VentaID;
            var oVentaV = Datos.GetEntity<VentasView>(q => q.VentaID == iVentaID);
            int iValeClienteID = oVentaV.ClienteID;
            if ((oGarantia.AccionID == Cat.VentasGarantiasAcciones.ArticuloNuevo || oGarantia.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito)
                && iValeClienteID == Cat.Clientes.Mostrador)
            {
                iValeClienteID = VentasLoc.ObtenerClienteID("Selecciona el cliente para crear el Vale:", false);
                if (iValeClienteID == 0)
                    return false;
            }

            // Se pregunta el usuario que realiza la devolución
            int iUsuarioID = 0;
            var ResU = UtilLocal.ValidarObtenerUsuario("Ventas.Garantia.Agregar");
            if (ResU.Error)
                return false;
            iUsuarioID = ResU.Respuesta.UsuarioID;

            // Se solicita la autorización
            int iAutorizoID = 0;
            ResU = UtilLocal.ValidarObtenerUsuario("Autorizaciones.Ventas.Garantia.Agregar", "Autorización");
            if (ResU.Exito)
                iAutorizoID = ResU.Respuesta.UsuarioID;
            
            // Se procede a guardar los cambios
            DateTime dAhora = DateTime.Now;
            
            // Se genera la garantía
            oGarantia.Fecha = dAhora;
            oGarantia.RealizoUsuarioID = iUsuarioID;
            oGarantia.EstatusGenericoID = Cat.EstatusGenericos.Recibido;
            // Se llenan los datos de la parte
            var oProducto = this.ctlDetalle.ProductosSel()[0];
            oGarantia.ParteID = oProducto.ParteID;
            oGarantia.Costo = oProducto.Costo;
            oGarantia.CostoConDescuento = oProducto.CostoConDescuento;
            oGarantia.PrecioUnitario = oProducto.PrecioUnitario;
            oGarantia.Iva = oProducto.Iva;

            // Se guarda la garantía
            Guardar.VentaGarantia(oGarantia);
            
            // Si queda a revisión del proveedor, ya no se hace nada más que mandar el ticket
            if (oGarantia.AccionID != Cat.VentasGarantiasAcciones.RevisionDeProveedor)
            {
                this.CompletarAccionGarantia(oGarantia.VentaGarantiaID, iValeClienteID);
            }

            // Se mete un registro en kárdex, en cero, sólo para mantener historial de la operación
            var oKardex = new ParteKardex()
            {
                ParteID = oGarantia.ParteID,
                OperacionID = Cat.OperacionesKardex.VentaCancelada,
                SucursalID = oGarantia.SucursalID,
                Folio = oVentaV.Folio,
                Fecha = dAhora,
                RealizoUsuarioID = iUsuarioID,
                Entidad = oVentaV.Cliente,
                Origen = oVentaV.Sucursal,
                Destino = "GARANTÍA RECIBIDA",
                Cantidad = 0,
                Importe = (oGarantia.PrecioUnitario + oGarantia.Iva),
                RelacionTabla = Cat.Tablas.VentaGarantia,
                RelacionID = oGarantia.VentaGarantiaID
            };
            AdmonProc.RegistrarKardex(oKardex);

            // Se guarda la autorización, si aplica
            VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.Garantia, Cat.Tablas.VentaGarantia, oGarantia.VentaGarantiaID, iAutorizoID);
            // Se genera el ticket correspondiente
            VentasLoc.GenerarTicketGarantia(oGarantia.VentaGarantiaID);

            return true;
        }

        private bool CompletarGarantia()
        {
            // Se valida la parte de búsqueda
            if (!this.ctlBusqueda.ValidarPendiente())
                return false;

            //
            int iGarantiaID = this.ctlBusqueda.SeleccionGarantiaID;
            var oGarantia = Datos.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iGarantiaID && c.Estatus);
            var oVentaV = Datos.GetEntity<VentasView>(c => c.VentaID == oGarantia.VentaID);

            // Se verifica si se creará vale, para pedir el cliente en caso de que no haya
            int iValeClienteID = oVentaV.ClienteID;
            if ((this.ctlBusqueda.IdAccionPosterior == Cat.VentasGarantiasAcciones.ArticuloNuevo 
                || this.ctlBusqueda.IdAccionPosterior == Cat.VentasGarantiasAcciones.NotaDeCredito) && iValeClienteID == Cat.Clientes.Mostrador)
            {
                iValeClienteID = VentasLoc.ObtenerClienteID("Selecciona el cliente para crear el Vale:", false);
                if (iValeClienteID == 0)
                    return false;
            }

            // Se pregunta el usuario que realiza la devolución
            int iUsuarioID = 0;
            var ResU = UtilLocal.ValidarObtenerUsuario("Ventas.Garantia.Agregar");
            if (ResU.Error)
                return false;
            iUsuarioID = ResU.Respuesta.UsuarioID;

            // Se solicita la autorización
            int iAutorizoID = 0;
            ResU = UtilLocal.ValidarObtenerUsuario("Autorizaciones.Ventas.Garantia.Agregar", "Autorización");
            if (ResU.Exito)
                iAutorizoID = ResU.Respuesta.UsuarioID;
            
            // Se procede a guardar los cambios
            DateTime dAhora = DateTime.Now;

            // Se completan los datos de la garantía
            oGarantia.AccionID = this.ctlBusqueda.IdAccionPosterior;
            oGarantia.FechaCompletado = dAhora;
            oGarantia.EstatusGenericoID = Cat.EstatusGenericos.Completada;
            if (this.ctlBusqueda.AccionObservacion != "")
                oGarantia.ObservacionCompletado += (" - " + this.ctlBusqueda.AccionObservacion);
            // Se guardan los datos
            Datos.Guardar<VentaGarantia>(oGarantia);

            // Si queda a revisión del proveedor, ya no se hace nada más que mandar el ticket
            if (oGarantia.AccionID != Cat.VentasGarantiasAcciones.NoProcede)
            {
                // Se borra la parte de la venta, pues ya se hizo válida la garantia
                var oParteVenta = Datos.GetEntity<VentaDetalle>(q => q.Estatus
                    && q.VentaID == oGarantia.VentaID
                    && q.ParteID == oGarantia.ParteID
                    && q.PrecioUnitario == oGarantia.PrecioUnitario
                    && q.Iva == oGarantia.Iva);
                if (oParteVenta.Cantidad > 1)
                {
                    oParteVenta.Cantidad--;
                    Datos.Guardar<VentaDetalle>(oParteVenta);
                }
                else
                {
                    Datos.Eliminar<VentaDetalle>(oParteVenta, true);
                }
                //
                this.CompletarAccionGarantia(iGarantiaID, iValeClienteID);
            }

            // Se guarda la autorización, si aplica
            VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.Garantia, Cat.Tablas.VentaGarantia, oGarantia.VentaGarantiaID, iAutorizoID);
            // Se genera el ticket correspondiente
            VentasLoc.GenerarTicketGarantia(oGarantia.VentaGarantiaID);

            return true;
        }

        private void CompletarAccionGarantia(int iGarantiaID, int? iValeClienteID)
        {
            var oGarantiaV = Datos.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == iGarantiaID);
            var oVentaV = Datos.GetEntity<VentasView>(c => c.VentaID == oGarantiaV.VentaID);
            int iVentaID = oVentaV.VentaID;
            
            // Se cambia el estatus de la venta, cuando aplique (cuando ya no hay partes en el detalle de la venta)
            if (!Datos.Exists<VentaDetalle>(c => c.VentaID == iVentaID && c.Estatus))
            {
                var oVenta = Datos.GetEntity<Venta>(c => c.VentaID == iVentaID && c.Estatus);
                oVenta.VentaEstatusID = Cat.VentasEstatus.AGarantia;
                Datos.Guardar<Venta>(oVenta);
            }

            // Se obtiene el importe a devolver, por si fue a crédito y no se ha pagado toda la venta
            decimal mImporteDev = (oVentaV.Pagado > oGarantiaV.Total ? oGarantiaV.Total.Valor() : oVentaV.Pagado);

            // Se genera nota de crédito o devolución de efectivo, u otro, según aplique
            if (mImporteDev > 0)
            {
                ResAcc<int> oResPagoNeg = null;
                switch (oGarantiaV.AccionID)
                {
                    case Cat.VentasGarantiasAcciones.ArticuloNuevo:
                    case Cat.VentasGarantiasAcciones.NotaDeCredito:
                        // var oVenta = General.GetEntity<Venta>(q => q.Estatus && q.VentaID == iVentaID);
                        var oResVale = VentasProc.GenerarNotaDeCredito(iValeClienteID.Value, mImporteDev, "", Cat.OrigenesNotaDeCredito.Garantia
                            , oGarantiaV.VentaGarantiaID);
                        // Se genera el pago negativo por la nota de crédito generada
                        oResPagoNeg = VentasProc.GenerarPagoNegativoPorNotaDeCredito(iVentaID, mImporteDev, oResVale.Respuesta);
                        break;
                    case Cat.VentasGarantiasAcciones.Efectivo:
                        oResPagoNeg = VentasProc.GenerarDevolucionDeEfectivo(iVentaID, mImporteDev);
                        break;
                    case Cat.VentasGarantiasAcciones.Cheque:
                    case Cat.VentasGarantiasAcciones.Tarjeta:
                    case Cat.VentasGarantiasAcciones.TarjetaDeDebito:
                    case Cat.VentasGarantiasAcciones.Transferencia:
                        int iFormaDePagoID = UtilDatos.FormaDePagoDeAccionGarantia(oGarantiaV.AccionID);
                        var oVentaPago = Datos.GetEntity<VentaPago>(q => q.VentaID == iVentaID && q.Estatus);
                        var oFormaPago = Datos.GetEntity<VentaPagoDetalle>(q => 
                            q.VentaPagoID == oVentaPago.VentaPagoID && q.TipoFormaPagoID == iFormaDePagoID && q.Estatus);
                        // Se genera un pago negativo con la misma forma del pago a contrarestar
                        oResPagoNeg = VentasProc.GenerarPago(iVentaID, (oFormaPago.Importe * -1), iFormaDePagoID, oFormaPago.BancoID.Valor(), oFormaPago.Folio, oFormaPago.Cuenta);
                        break;
                }

                // Se guarda el dato del pago negativo correspondiente a la devolución, si aplica
                if (oResPagoNeg != null)
                {
                    var oGarantia = Datos.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iGarantiaID && c.Estatus);
                    // Se obtiene el primer registro de VentaPagoDetalle, y ese es el que se relaciona con la garantía, pues se supone que siempre habrá sólo uno
                    var oPagoDet = Datos.GetEntity<VentaPagoDetalle>(c => c.VentaPagoID == oResPagoNeg.Respuesta && c.Estatus);
                    oGarantia.VentaPagoDetalleID = oPagoDet.VentaPagoDetalleID;
                    Datos.Guardar<VentaGarantia>(oGarantia);
                }
            }

            // Se verifica si es factura, en cuyo caso, se genera nota de crédito, según aplique
            if (oVentaV.Facturada)
            {
                var ResFactura = VentasLoc.GenerarFacturaDevolucionPorGarantia(iGarantiaID);
                if (ResFactura.Error)
                    UtilLocal.MensajeAdvertencia("Hubo un error al generar la factura de la devolución.\n\n" + ResFactura.Mensaje);
            }

            // Se crea la póliza contable correspondiente, según el caso (AfeConta)
            if (oVentaV.Facturada)
            {
                if (oVentaV.ACredito)
                {
                    if (oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.ArticuloNuevo || oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito)
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale, oGarantiaV.VentaGarantiaID
                            , oGarantiaV.FolioDeVenta, oGarantiaV.MotivoObservacion);
                    else
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago, oGarantiaV.VentaGarantiaID
                            , oGarantiaV.FolioDeVenta, oGarantiaV.MotivoObservacion);
                }
                else
                {
                    if (oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.ArticuloNuevo || oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito)
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GarantiaVentaValeFactura, oGarantiaV.VentaGarantiaID
                            , oGarantiaV.FolioDeVenta, oGarantiaV.MotivoObservacion);
                    else
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GarantiaVentaPagoFactura, oGarantiaV.VentaGarantiaID
                            , oGarantiaV.FolioDeVenta, oGarantiaV.MotivoObservacion);
                }
            }
            else
            {
                if (oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.ArticuloNuevo || oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito)
                    ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GarantiaVentaValeTicket, oGarantiaV.VentaGarantiaID
                        , oGarantiaV.FolioDeVenta, oGarantiaV.MotivoObservacion);

                // Si es tiecket a crédito, se hace ajuste temporal de pólizas
                if (oVentaV.ACredito)
                {
                    ContaProc.BorrarPolizaTemporalTicketCredito(iVentaID);
                    // Se actualizan los datos de la venta
                    oVentaV = Datos.GetEntity<VentasView>(c => c.VentaID == iVentaID);
                    ContaProc.CrearPolizaTemporalTicketCredito(iVentaID, (oVentaV.Total - oVentaV.Pagado));
                }
            }
        }

        #endregion
    }
}
