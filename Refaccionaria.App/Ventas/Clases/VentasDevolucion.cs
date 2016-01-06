using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FastReport;
using System.Text;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public class VentasDevolucion : VentaOpcion
    {
        public BusquedaDevolucion ctlBusqueda;
        public DetalleDevolucion ctlDetalle;

        public VentasDevolucion(Ventas oVenta)
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
            this.ctlBusqueda = new BusquedaDevolucion();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlBusqueda.oDevolucion = this;

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

            // Se valida la parte de detalle
            if (!this.ctlDetalle.Validar())
                return false;
            // Se valida la parte de búsqueda
            if (!this.ctlBusqueda.Validar())
                return false;
           
            int iVentaID = this.ctlBusqueda.VentaID;

            // Se valida que no sea una venta usada para cobro de Control de Cascos
            if (General.Exists<CascosRegistrosView>(c => c.CobroVentaID == iVentaID 
                && (c.VentaEstatusID != Cat.VentasEstatus.Cancelada && c.VentaEstatusID != Cat.VentasEstatus.CanceladaSinPago)))
            {
                UtilLocal.MensajeAdvertencia("La venta seleccionado fue utilizada para un cobro de Control de Cascos. No se puede cancelar.");
                return false;
            }

            // 
            bool bCancelacion = this.ctlDetalle.TodosMarcados();
            var oVentaV = General.GetEntity<VentasView>(q => q.VentaID == iVentaID);

            // Se verifica si es una cancelación de otra sucursal
            if (oVentaV.SucursalID != GlobalClass.SucursalID)
            {
                if (UtilLocal.MensajePregunta("La Venta seleccionada es de otra Sucursal. ¿Deseas continuar?") != DialogResult.Yes)
                    return false;
            }
            
            // Se verifica si es una cancelación de factura de varios tickets
            int iVentaFacturaID = 0;
            bool bFacturaMultiple = false;
            List<VentaFacturaDetalle> oVentasFactura = null;
            if (bCancelacion && oVentaV.Facturada)
            {
                iVentaFacturaID = General.GetEntity<VentaFacturaDetalle>(q => q.VentaID == oVentaV.VentaID && q.Estatus).VentaFacturaID;
                oVentasFactura = General.GetListOf<VentaFacturaDetalle>(q => q.VentaFacturaID == iVentaFacturaID && q.Estatus);
                bFacturaMultiple = (oVentasFactura.Count > 1);

                // Se muestran los datos de detalle de todas las ventas de la factura
                if (bFacturaMultiple)
                {
                    if (!this.MostrarDetalleVentasFactura(oVentasFactura))
                        return false;
                }
            }

            // Se verifica si se creará vale, para pedir el cliente en caso de que no haya
            int iValeClienteID = oVentaV.ClienteID;
            if (this.ctlBusqueda.FormaDeDevolucion == Cat.FormasDePago.Vale && iValeClienteID == Cat.Clientes.Mostrador)
            {
                var frmValor = new MensajeObtenerValor("Selecciona el cliente para crear el Vale:", "", MensajeObtenerValor.Tipo.Combo);
                frmValor.CargarCombo("ClienteID", "Nombre", General.GetListOf<Cliente>(q => q.ClienteID != Cat.Clientes.Mostrador && q.Estatus));
                if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
                    iValeClienteID = Helper.ConvertirEntero(frmValor.Valor);
                frmValor.Dispose();
                if (iValeClienteID == 0)
                    return false;
            }

            // Se pregunta el usuario que realiza la devolución
            int iUsuarioID = 0;
            var ResU = UtilDatos.ValidarObtenerUsuario("Ventas.Devolucion.Agregar");
            if (ResU.Error)
                return false;
            iUsuarioID = ResU.Respuesta.UsuarioID;

            // Se solicita la autorización
            int iAutorizoID = 0;
            ResU = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.Devoluciones.Agregar", "Autorización");
            if (ResU.Exito)
                iAutorizoID = ResU.Respuesta.UsuarioID;

            // Se procede a guardar los cambios
            DateTime dAhora = DateTime.Now;

            // Si es factura múltiple, se cancelan todas las ventas de la factura, si no, sólo la venta seleccionada
            var oIdsDev = new List<int>();
            var oIdsCascos = new List<int>();
            if (bFacturaMultiple)
            {
                var oDevGeneral = this.ctlBusqueda.GenerarDevolucion();
                foreach (var oVentaFac in oVentasFactura)
                {
                    // Se genera la devolución
                    var oDev = new VentaDevolucion()
                    {
                        VentaID = oVentaFac.VentaID,
                        Fecha = dAhora,
                        RealizoUsuarioID = iUsuarioID,
                        EsCancelacion = true,
                        MotivoID = oDevGeneral.MotivoID,
                        Observacion = oDevGeneral.Observacion,
                        TipoFormaPagoID = oDevGeneral.TipoFormaPagoID
                    };
                    // Se genera el detalle de la devolución
                    var oDevDet = new List<VentaDevolucionDetalle>();
                    var oVentaDetalle = General.GetListOf<VentaDetalle>(c => c.VentaID == oVentaFac.VentaID && c.Estatus);
                    foreach (var oParte in oVentaDetalle) {
                        oDevDet.Add(new VentaDevolucionDetalle()
                        {
                            ParteID = oParte.ParteID,
                            Costo = oParte.Costo,
                            CostoConDescuento = oParte.CostoConDescuento,
                            Cantidad = oParte.Cantidad,
                            PrecioUnitario = oParte.PrecioUnitario,
                            Iva = oParte.Iva
                        });
                    }
                    // Se manda guardar la devolución
                    this.GuardarDevolucion(oDev, oDevDet, iValeClienteID);
                    // Se agrega a la lisa de devoluciones
                    oIdsDev.Add(oDev.VentaDevolucionID);

                    // Se verifica si requiere un casco
                    foreach (var oReg in oDevDet) {
                        if (General.Exists<Parte>(c => c.ParteID == oReg.ParteID && c.RequiereCascoDe > 0 && c.Estatus))
                            oIdsCascos.Add(oReg.VentaDevolucionDetalleID);
                    }
                }
            }
            else
            {
                // Se genera la devolución
                var oDevolucion = this.ctlBusqueda.GenerarDevolucion();
                oDevolucion.Fecha = dAhora;
                oDevolucion.EsCancelacion = bCancelacion;
                oDevolucion.RealizoUsuarioID = iUsuarioID;
                // Se genera el detalle de la devolución
                var oDevDetalle = new List<VentaDevolucionDetalle>();
                var oProductos = this.ctlDetalle.ProductosSel();
                foreach (var oProducto in oProductos)
                {
                    oDevDetalle.Add(new VentaDevolucionDetalle()
                    {
                        ParteID = oProducto.ParteID,
                        Costo = oProducto.Costo,
                        CostoConDescuento = oProducto.CostoConDescuento,
                        Cantidad = oProducto.Cantidad,
                        PrecioUnitario = oProducto.PrecioUnitario,
                        Iva = oProducto.Iva
                    });
                }
                // Se guarda la devolución
                this.GuardarDevolucion(oDevolucion, oDevDetalle, iValeClienteID);
                //
                oIdsDev.Add(oDevolucion.VentaDevolucionID);

                // Se verifica si requiere un casco
                foreach (var oReg in oDevDetalle) {
                    if (General.Exists<Parte>(c => c.ParteID == oReg.ParteID && c.RequiereCascoDe > 0 && c.Estatus))
                        oIdsCascos.Add(oReg.VentaDevolucionDetalleID);
                }
            }

            // Se verifica si es factura, en cuyo caso, se cancela la factura o se genera nota de crédito, según aplique
            if (oVentaV.Facturada)
            {
                if (bCancelacion)
                {
                    var ResFactura = VentasProc.GenerarFacturaCancelacion(iVentaFacturaID, oIdsDev);
                    if (ResFactura.Error)
                        UtilLocal.MensajeAdvertencia("Hubo un error al cancelar la factura.\n\n" + ResFactura.Mensaje);
                }
                else
                {
                    var ResFactura = VentasProc.GenerarFacturaDevolucionPorDevolucion(oIdsDev[0]);
                    if (ResFactura.Error)
                        UtilLocal.MensajeAdvertencia("Hubo un error al generar la factura de la devolución.\n\n" + ResFactura.Mensaje);
                }
            }

            // Se verifica si hay una cancelación de control de cascos
            foreach (int iDevDetID in oIdsCascos)
            {
                this.DevolverControlCasco(iDevDetID, iUsuarioID);
            }

            // Se manda a afectar contabilidad (AfeConta)
            foreach (int iDevID in oIdsDev)
            {
                var oDevV = General.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iDevID);
                if (oDevV.Facturada)
                {
                    if (oDevV.VentaACredito)
                    {
                        if (oDevV.FormaDePagoID == Cat.FormasDePago.Vale)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale, iDevID, oDevV.FolioDeVenta, oDevV.Observacion);
                        else
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago, iDevID, oDevV.FolioDeVenta, oDevV.Observacion);
                    }
                    else
                    {
                        if (oDevV.FormaDePagoID == Cat.FormasDePago.Vale)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DevolucionVentaValeFactura, iDevID, oDevV.FolioDeVenta, oDevV.Observacion);
                        else
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DevolucionVentaPago, iDevID, oDevV.FolioDeVenta, oDevV.Observacion);
                    }
                }
                else
                {
                    if (oDevV.FormaDePagoID == Cat.FormasDePago.Vale)
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DevolucionVentaValeTicket, iDevID, oDevV.FolioDeVenta, oDevV.Observacion);

                    // Si es tiecket a crédito, se hace ajuste temporal de pólizas
                    if (oDevV.VentaACredito)
                    {
                        ContaProc.BorrarPolizaTemporalTicketCredito(oDevV.VentaID);
                        if (!bCancelacion)
                        {
                            var oVentaVi = General.GetEntity<VentasView>(c => c.VentaID == oDevV.VentaID);
                            ContaProc.CrearPolizaTemporalTicketCredito(oDevV.VentaID, (oVentaVi.Total - oVentaVi.Pagado));
                        }
                    }
                }
            }

            //
            foreach (int iDevID in oIdsDev)
            {
                // Se guarda la autorización, si aplica
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.DevolucionCancelacion, Cat.Tablas.VentaDevolucion, iDevID, iAutorizoID);
                // Se genera el ticket correspondiente
                VentasProc.GenerarTicketDevolucion(iDevID);

                // Se agrega al Kardex
                var oDevV = General.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iDevID);
                var oDet = General.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iDevID && c.Estatus);
                foreach (var oReg in oDet)
                {
                    AdmonProc.RegistrarKardex(new ParteKardex()
                    {
                        ParteID = oReg.ParteID,
                        OperacionID = Cat.OperacionesKardex.VentaCancelada,
                        SucursalID = oDevV.SucursalID,
                        Folio = oDevV.FolioDeVenta,
                        Fecha = DateTime.Now,
                        RealizoUsuarioID = oDevV.RealizoUsuarioID,
                        Entidad = oVentaV.Cliente,
                        Origen = oVentaV.ClienteID.ToString(),
                        Destino = oDevV.Sucursal,
                        Cantidad = oReg.Cantidad,
                        Importe = (oReg.PrecioUnitario + oReg.Iva),
                        RelacionTabla = Cat.Tablas.VentaDevolucion,
                        RelacionID = oDevV.VentaDevolucionID
                    });
                }
            }
            
            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion((bCancelacion ? "Cancelación" : "Devolución") + " guardada correctamente.");

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

        private bool MostrarDetalleVentasFactura(List<VentaFacturaDetalle> oVentasFactura)
        {
            // Se forma la cadena con el texto a mostrar
            var oCadena = new StringBuilder();
            oCadena.AppendLine("Estás a punto de cancelar una Factura Múltiple (una factura que se compone de varios tickets). A continuación se presenta "
                + "el detalle de las partes que serán canceladas:\n");
            oCadena.AppendLine("NÚMERO DE PARTE".PadRight(16) + " " + "DESCRIPCIÓN");
            oCadena.AppendLine("-".Repetir(81));
            foreach (var oVenta in oVentasFactura)
            {
                var oDetalle = General.GetListOf<VentasDetalleView>(c => c.VentaID == oVenta.VentaID);
                foreach (var oParte in oDetalle)
                    oCadena.AppendLine(oParte.NumeroParte.RellenarCortarDerecha(16) + " " + oParte.NombreParte);
            }

            // Se muestra el diálogo
            var frmTexto = new MensajeTexto("Detalle de Ventas", oCadena.ToString());
            var oRes = frmTexto.ShowDialog(Principal.Instance);

            return (oRes == DialogResult.OK);
        }

        private void GuardarDevolucion(VentaDevolucion oDevolucion, List<VentaDevolucionDetalle> oDetalle, int? iValeClienteID)
        {
            // Se obtiene el importe total de la venta antes de la devolución
            var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == oDevolucion.VentaID);
            // decimal mTotalAntes = oVentaV.Total;

            // Se calcula el total de lo devuelto
            decimal mImporteDev = 0;
            foreach (var oParte in oDetalle)
                mImporteDev += ((oParte.PrecioUnitario + oParte.Iva) * oParte.Cantidad);
            // Se guarda la devolución
            Guardar.VentaDevolucion(oDevolucion, oDetalle);

            // Se verifica cuánto se ha pagado de la venta, por si fue a crédito
            if (oVentaV.Pagado < mImporteDev)
                mImporteDev = oVentaV.Pagado;

            // Se genera nota de crédito o devolución de efectivo, u otro, según aplique
            int iVentaID = oDevolucion.VentaID;
            if (mImporteDev > 0)
            {
                ResAcc<int> oResPagoNeg = null;
                switch (this.ctlBusqueda.FormaDeDevolucion)
                {
                    case Cat.FormasDePago.Efectivo:
                        oResPagoNeg = VentasProc.GenerarDevolucionDeEfectivo(iVentaID, mImporteDev);
                        break;
                    case Cat.FormasDePago.Vale:
                        // var oVenta = General.GetEntity<Venta>(q => q.Estatus && q.VentaID == iVentaID);
                        var oResVale = VentasProc.GenerarNotaDeCredito(iValeClienteID.Value, mImporteDev, "", Cat.OrigenesNotaDeCredito.Devolucion
                            , oDevolucion.VentaDevolucionID);
                        // Se genera el pago negativo por la nota de crédito generada
                        oResPagoNeg = VentasProc.GenerarPagoNegativoPorNotaDeCredito(iVentaID, mImporteDev, oResVale.Respuesta);
                        break;
                    case Cat.FormasDePago.Cheque:
                    case Cat.FormasDePago.Tarjeta:
                    case Cat.FormasDePago.Transferencia:
                        int iFormaDePagoID = this.ctlBusqueda.FormaDeDevolucion;
                        var oVentaPago = General.GetEntity<VentaPago>(q => q.VentaID == iVentaID && q.Estatus);
                        var oFormaPago = General.GetEntity<VentaPagoDetalle>(q => q.VentaPagoID == oVentaPago.VentaPagoID
                            && q.TipoFormaPagoID == iFormaDePagoID && q.Estatus);
                        // Se genera un pago negativo con la misma forma del pago a contrarestar
                        oResPagoNeg = VentasProc.GenerarPago(iVentaID, (oFormaPago.Importe * -1), iFormaDePagoID, oFormaPago.BancoID.Valor(), oFormaPago.Folio, oFormaPago.Cuenta);
                        break;
                }

                // Se guarda el dato del pago negativo correspondiente a la devolución, si aplica
                if (oResPagoNeg != null)
                {
                    // Se obtiene el primer registro de VentaPagoDetalle, y ese es el que se relaciona con la devolución, pues se supone que siempre habrá sólo uno
                    var oPagoDet = General.GetEntity<VentaPagoDetalle>(c => c.VentaPagoID == oResPagoNeg.Respuesta && c.Estatus);
                    oDevolucion.VentaPagoDetalleID = oPagoDet.VentaPagoDetalleID;
                    Guardar.Generico<VentaDevolucion>(oDevolucion);
                }
            }

            // Se revisa si la venta pertenece a un 9500, para realizar operaciones especiales
            var o9500 = General.GetEntity<Cotizacion9500>(c => c.VentaID == iVentaID && c.Estatus);
            // Si es 9500, se cambia el estatus del 9500
            if (o9500 != null)
            {
                o9500.EstatusGenericoID = Cat.EstatusGenericos.CanceladoDespuesDeVendido;
                Guardar.Generico<Cotizacion9500>(o9500);
            }

            // Se verifica si la venta generó un movimiento bancario, para borrarlo si éste no ha sido asignado
            if (oDevolucion.EsCancelacion)
            {
                var oPagoDetV = General.GetListOf<VentasPagosDetalleView>(c => c.VentaID == iVentaID);
                foreach (var oReg in oPagoDetV)
                {
                    if (oReg.FormaDePagoID == Cat.FormasDePago.Cheque || oReg.FormaDePagoID == Cat.FormasDePago.Tarjeta || oReg.FormaDePagoID == Cat.FormasDePago.Transferencia)
                    {
                        var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.RelacionTabla == Cat.Tablas.VentaPagoDetalle && c.RelacionID == oReg.VentaPagoDetalleID);
                        if (oMov != null && !oMov.FechaAsignado.HasValue)
                            Guardar.Eliminar<BancoCuentaMovimiento>(oMov);
                    }
                }
            }
        }

        private void DevolverControlCasco(int iDevolucionDetalleID, int iUsuarioID)
        {
            // Se obtiene el registro del control de casco
            var oDevDet = General.GetEntity<VentaDevolucionDetalle>(c => c.VentaDevolucionDetalleID == iDevolucionDetalleID && c.Estatus);
            var oDev = General.GetEntity<VentaDevolucion>(c => c.VentaDevolucionID == oDevDet.VentaDevolucionID && c.Estatus);
            for (int i = 0; i < oDevDet.Cantidad; i++)
            {
                int iCancelarVentaID = 0;
                var oCascoReg = General.GetEntity<CascoRegistro>(c => c.VentaID == oDev.VentaID && c.ParteID == oDevDet.ParteID);
                // Acción si no se recibió ningún casco
                if (oCascoReg.RecibidoCascoID == null)
                {
                    iCancelarVentaID = oCascoReg.CobroVentaID.Valor();
                }
                else
                {
                    var oParte = General.GetEntity<Parte>(c => c.ParteID == oCascoReg.ParteID && c.Estatus);

                    // Se registra la salida del inventario
                    var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == oDev.VentaID);
                    var oCascoRecibidoPrecio = General.GetEntity<PartePrecio>(c => c.ParteID == oCascoReg.RecibidoCascoID.Value && c.Estatus);
                    AdmonProc.AfectarExistenciaYKardex(oCascoReg.RecibidoCascoID.Valor(), GlobalClass.SucursalID, Cat.OperacionesKardex.SalidaInventario
                        , oCascoReg.CascoRegistroID.ToString(), iUsuarioID, oVentaV.Cliente, "CONTROL DE CASCOS", oVentaV.Sucursal, 1, oCascoRecibidoPrecio.Costo.Valor()
                        , Cat.Tablas.CascoRegistro, oCascoReg.CascoRegistroID);

                    // Acciones si no se recibió el casco adecuado
                    if (oCascoReg.RecibidoCascoID != oParte.RequiereCascoDe)
                    {
                        // Acción si hubo diferencia de importe entre el casco recibido y el esperado
                        iCancelarVentaID = oCascoReg.CobroVentaID.Valor();  // Por si se realizó un cobro
                        // Por si se generó un vale
                        var oVale = General.GetEntity<NotaDeCredito>(c => c.OrigenID == Cat.OrigenesNotaDeCredito.CascoDeMayorValor
                            && c.RelacionID == oCascoReg.CascoRegistroID);
                        if (oVale != null)
                        {
                            VentasProc.CancelarNotaDeCredito(oVale.NotaDeCreditoID, "POR CANCELACIÓN DE VENTA CON CASCO");
                        }
                        // Si se utilizarn importes de caso a favor, se liberan
                        var oUsados = General.GetListOf<CascoRegistroImporte>(c => c.CascoRegistroID == oCascoReg.CascoRegistroID);
                        foreach (var oReg in oUsados)
                        {
                            var oCascoImp = General.GetEntity<CascoImporte>(c => c.CascoImporteID == oReg.CascoImporteID);
                            oCascoImp.ImporteUsado -= oReg.Importe.Valor();
                            Guardar.Generico<CascoImporte>(oCascoImp);
                            Guardar.Eliminar<CascoRegistroImporte>(oReg);
                        }
                        // Si se creó un importe de caso a favor, se cancela
                        var oCascoImporte = General.GetEntity<CascoImporte>(c => c.OrigenID == oCascoReg.CascoRegistroID && c.Importe > 0);
                        if (oCascoImporte != null)
                        {
                            // Se verifica si ha sido usado
                            if (General.Exists<CascoRegistroImporte>(c => c.CascoImporteID == oCascoImporte.CascoImporteID))
                            {
                                // Se mete un Casco Importe negativo, porque ya fue usado
                                Guardar.Generico<CascoImporte>(new CascoImporte()
                                {
                                    Fecha = DateTime.Now,
                                    OrigenID = oCascoReg.CascoRegistroID,
                                    Importe = (oCascoImporte.Importe * -1)
                                });
                            }
                            else
                            {
                                // Se elimina el importe a favor
                                Guardar.Eliminar<CascoImporte>(oCascoImporte);
                            }
                        }
                    }
                }

                // Se cancela la venta del cobro de la diferencia (sólo lo correspondiente al cobro), si aplica
                if (iCancelarVentaID > 0)
                {
                    int iDevParteID = Cat.Partes.DiferenciaDeCascos;
                    if (oCascoReg.RecibidoCascoID == null)
                    {
                        var oParte = General.GetEntity<Parte>(c => c.ParteID == oCascoReg.ParteID && c.Estatus);
                        iDevParteID = oParte.RequiereDepositoDe.Valor();
                        if (iDevParteID <= 0)
                        {
                            UtilLocal.MensajeError("El Artículo registrado en Control de Cascos no tiene un depósito asignado.");
                            break;
                        }
                    }

                    var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == iCancelarVentaID && c.Estatus);
                    if (oVentaDet.Count > 0)
                    {
                        var oParteDet = oVentaDet.FirstOrDefault(c => c.ParteID == iDevParteID);
                        // Se obtiene la forma de pago
                        var oPagoDetV = General.GetEntity<VentasPagosDetalleView>(c => c.VentaID == iCancelarVentaID);
                        int iFormaDePagoID = (oPagoDetV.Fecha.Valor().Date == DateTime.Now.Date ? oPagoDetV.FormaDePagoID : Cat.FormasDePago.Efectivo);
                        // Se general el registro de devolución
                        var oDevolucion = new VentaDevolucion()
                        {
                            VentaID = iCancelarVentaID,
                            Fecha = DateTime.Now,
                            SucursalID = GlobalClass.SucursalID,
                            MotivoID = Cat.VentaDevolucionMotivos.Otro,
                            Observacion = "POR CANCELACIÓN DE VENTA CON CASCO",
                            RealizoUsuarioID = iUsuarioID,
                            TipoFormaPagoID = iFormaDePagoID,
                            EsCancelacion = (oVentaDet.Count == 1)
                        };
                        // Se genera el detalle de la devolución
                        var oDetDev = new VentaDevolucionDetalle()
                        {
                            ParteID = iDevParteID,
                            Cantidad = oParteDet.Cantidad,
                            PrecioUnitario = oParteDet.PrecioUnitario,
                            Iva = oParteDet.Iva,
                            Costo = oParteDet.Costo,
                            CostoConDescuento = oParteDet.CostoConDescuento
                        };
                        // Se guarda la devolución
                        int iValeClienteID = 0;
                        if (iFormaDePagoID == Cat.FormasDePago.Vale)
                        {
                            var oVale = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == oPagoDetV.NotaDeCreditoID && c.Estatus);
                            iValeClienteID = oVale.ClienteID;
                        }
                        this.GuardarDevolucion(oDevolucion, new List<VentaDevolucionDetalle>() { oDetDev }, iValeClienteID);
                    }
                }
            }
        }

        #endregion
    }
}
