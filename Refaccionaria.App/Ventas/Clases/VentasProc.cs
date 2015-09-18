using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using FastReport;
using System.Text;
using System.Data.Objects;
using System.Diagnostics;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using FacturacionElectronica;

namespace Refaccionaria.App
{
    static class VentasProc
    {
        /*
        public static ResAcc<int> GenerarNotaDeCredito(int iClienteID, decimal mImporte, string sObservacion)
        {
            return VentasProc.GenerarNotaDeCredito(iClienteID, mImporte, sObservacion, null);
        }

        public static ResAcc<int> GenerarNotaDeCredito(int iClienteID, decimal mImporte, string sObservacion, int? iOrigenVentaID)
        {
            DateTime dAhora = DateTime.Now;
            
            // Se revisa si existen otras notas de crédito que se puedan usar para afectar ésta
            bool bNegativa = (mImporte < 0);
            var oNotas = General.GetListOf<NotaDeCredito>(q => q.ClienteID == iClienteID && q.Valida && q.Estatus).OrderByDescending(q => q.FechaDeEmision);
            foreach (var oNota in oNotas)
            {
                if ((bNegativa && oNota.Importe < 0) || (!bNegativa && oNota.Importe > 0))
                    continue;

                mImporte += oNota.Importe;

                if ((bNegativa && mImporte <= 0) || (!bNegativa && mImporte >= 0))
                // 
                {
                    oNota.Valida = false;
                    oNota.FechaDeUso = dAhora;
                    oNota.Observacion += ("\n\n" + DateTime.Now.ToString() + "Nota cancelada por ser descontada al agregar otra nota con un importe mayor.");
                    Guardar.Generico<NotaDeCredito>(oNota);
                    // Se debe imprimir algo aquí ?

                    // Si ya se emparejó la cantidad, se sale
                    if (mImporte == 0)
                        return new ResAcc<int>(true, "");
                }
                else
                // 
                {
                    oNota.Importe += (mImporte * -1);
                    Guardar.Generico<NotaDeCredito>(oNota);
                    // Se debe imprimir algo aquí ?
                    
                    // Se sale, pues ya no hay importe para afectar
                    return new ResAcc<int>(true, "");
                }
            }

            // Se crea la nota
            var oNotaNueva = new NotaDeCredito()
            {
                FechaDeEmision = DateTime.Now,
                Importe = mImporte,
                ClienteID = iClienteID,
                Valida = true,
                Observacion = sObservacion,
                OrigenVentaID = iOrigenVentaID
            };
            Guardar.Generico<NotaDeCredito>(oNotaNueva);

            // Se imprime el ticket correspondiente


            return new ResAcc<int>(true, oNotaNueva.NotaDeCreditoID);
        }
        */

        #region [ Vales ]

        public static ResAcc<int> GenerarNotaDeCredito(int iClienteID, decimal mImporte, string sObservacion, int iOrigenID, string sReferencia)
        {
            DateTime dAhora = DateTime.Now;
            
            // Se revisa si existen otras notas de crédito que se puedan usar para afectar ésta
            bool bNegativa = (mImporte < 0);
            var oNotas = General.GetListOf<NotaDeCredito>(q => q.ClienteID == iClienteID && q.Valida && q.Estatus).OrderByDescending(q => q.FechaDeEmision);
            foreach (var oNota in oNotas)
            {
                if ((bNegativa && oNota.Importe < 0) || (!bNegativa && oNota.Importe > 0))
                    continue;

                mImporte += oNota.Importe;

                if ((bNegativa && mImporte <= 0) || (!bNegativa && mImporte >= 0))
                // 
                {
                    oNota.Valida = false;
                    oNota.FechaDeUso = dAhora;
                    oNota.Observacion += ("\n\n" + DateTime.Now.ToString() + "Nota cancelada por ser descontada al agregar otra nota con un importe mayor.");
                    Guardar.Generico<NotaDeCredito>(oNota);
                    // Se debe imprimir algo aquí ?

                    // Si ya se emparejó la cantidad, se sale
                    if (mImporte == 0)
                        return new ResAcc<int>(true, "");
                }
                else
                // 
                {
                    oNota.Importe += (mImporte * -1);
                    Guardar.Generico<NotaDeCredito>(oNota);
                    // Se debe imprimir algo aquí ?

                    // Se sale, pues ya no hay importe para afectar
                    return new ResAcc<int>(true, "");
                }
            }

            // Se crea la nota
            var oNotaNueva = new NotaDeCredito()
            {
                FechaDeEmision = DateTime.Now,
                Importe = mImporte,
                ClienteID = iClienteID,
                Valida = true,
                Observacion = sObservacion,
                OrigenID = iOrigenID,
                Referencia = sReferencia
            };
            Guardar.Generico<NotaDeCredito>(oNotaNueva);

            // Se imprime el ticket correspondiente


            return new ResAcc<int>(true, oNotaNueva.NotaDeCreditoID);
        }

        public static ResAcc<int> GenerarPagoNegativoPorNotaDeCredito(int iVentaID, decimal mImporte, int iNotaDeCreditoID)
        {
            // Se generan los datos del pago
            var oPago = new VentaPago()
            {
                VentaID = iVentaID,
                // TipoPagoID = Cat.TiposDePago.Contado,
                Fecha = DateTime.Now
            };
            var oPagoDetalle = new List<VentaPagoDetalle>();
            oPagoDetalle.Add(new VentaPagoDetalle()
            {
                TipoFormaPagoID = Cat.FormasDePago.Vale,
                Importe = (mImporte * -1),
                NotaDeCreditoID = iNotaDeCreditoID
            });

            // Se guarda el pago
            Guardar.VentaPago(oPago, oPagoDetalle);

            return new ResAcc<int>(true, oPago.VentaPagoID);
        }

        public static ResAcc<bool> CancelarNotaDeCredito(int iNotaDeCreditoID, string sMotivoDeBaja)
        {
            var oVale = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iNotaDeCreditoID && c.Estatus);
            oVale.Valida = false;
            oVale.MotivoBaja = sMotivoDeBaja;
            Guardar.Generico<NotaDeCredito>(oVale);

            return new ResAcc<bool>(true);
        }

        #endregion

        public static ResAcc<int> GenerarDevolucionDeEfectivo(int iVentaID, decimal mImporte)
        {
            return VentasProc.GenerarPago(iVentaID, (mImporte * -1), Cat.FormasDePago.Efectivo);
        }

        public static ResAcc<int> GenerarPago(int iVentaID, decimal mImporte, int iFormaDePagoID, int iBancoID, string sFolio, string sCuenta)
        {
            // Se generan los datos del pago
            var oPago = new VentaPago()
            {
                VentaID = iVentaID,
                Fecha = DateTime.Now
            };
            var oPagoDetalle = new List<VentaPagoDetalle>();
            oPagoDetalle.Add(new VentaPagoDetalle()
            {
                TipoFormaPagoID = iFormaDePagoID,
                Importe = mImporte
            });
            if (iBancoID > 0)
            {
                oPagoDetalle[0].BancoID = iBancoID;
                oPagoDetalle[0].Folio = sFolio;
                oPagoDetalle[0].Cuenta = sCuenta;
            }

            // Se guarda el pago
            Guardar.VentaPago(oPago, oPagoDetalle);

            return new ResAcc<int>(true, oPago.VentaPagoID);
        }

        public static ResAcc<int> GenerarPago(int iVentaID, decimal mImporte, int iFormaDePagoID)
        {
            return VentasProc.GenerarPago(iVentaID, mImporte, iFormaDePagoID, 0, "", "");
        }

        public static string GenerarFolioDeVenta()
        {
            string sFolio = Config.Valor("Ventas.Folio");
            Config.EstablecerValor("Ventas.Folio", (Helper.ConvertirEntero(sFolio) + 1).ToString());
            return sFolio;
        }

        public static string GenerarFolioDeCobranza()
        {
            string sFolio = Config.Valor("Cobranza.Folio");
            Config.EstablecerValor("Cobranza.Folio", (Helper.ConvertirEntero(sFolio) + 1).ToString());
            return sFolio;
        }

        public static ResAcc<int> GenerarAutorizacion(int iProcesoID, string sTabla, int iRegistroID, int? iAutorizoUsuarioID)
        {
            // Se guarda la autorización
            var oAutorizacion = new Autorizacion()
            {
                AutorizacionProcesoID = iProcesoID,
                Tabla = sTabla,
                TablaRegistroID = iRegistroID,
                SucursalID = GlobalClass.SucursalID
            };
            if (iAutorizoUsuarioID.HasValue && iAutorizoUsuarioID > 0)
            {
                oAutorizacion.Autorizado = true;
                oAutorizacion.AutorizoUsuarioID = iAutorizoUsuarioID;
                oAutorizacion.FechaAutorizo = DateTime.Now;
            };
            Guardar.Generico<Autorizacion>(oAutorizacion);

            return new ResAcc<int>(true);
        }

        public static void AgregarExistencia(int iParteID, int iSucursalID, decimal mAgregar)
        {
            var oParte = General.GetEntity<Parte>(q => q.ParteID == iParteID);
            if (!oParte.EsServicio.Valor())
            {
                var oParteEx = General.GetEntity<ParteExistencia>(q => q.SucursalID == iSucursalID && q.ParteID == iParteID && q.Estatus);
                oParteEx.Existencia += mAgregar;
                Guardar.Generico<ParteExistencia>(oParteEx);
            }
        }

        public static void EliminarVenta(int iVentaID)
        {
            var oVenta = General.GetEntity<Venta>(q => q.VentaID == iVentaID && q.Estatus);

            // De momento sólo están contempladas las ventas que sólo han sido registradas. No cobradas, pagadas, canceladas
            if (oVenta.VentaEstatusID != Cat.VentasEstatus.Realizada) return;

            var oVentaDet = General.GetListOf<VentaDetalle>(q => q.VentaID == oVenta.VentaID && q.Estatus);

            // Se regresa la existencia y se borra el detalle de la venta
            foreach (var oParteDet in oVentaDet)
            {
                VentasProc.AgregarExistencia(oParteDet.ParteID, oVenta.SucursalID, oParteDet.Cantidad);
                Guardar.Eliminar<VentaDetalle>(oParteDet, true);
            }
            // Se borra la venta en sí
            Guardar.Eliminar<Venta>(oVenta, true);
        }

        public static void Cancelar9500(int i9500ID, string sMotivo, int iUsuarioID)
        {
            var o9500 = General.GetEntity<Cotizacion9500>(q => q.Cotizacion9500ID == i9500ID && q.Estatus);
            var o9500Detalle = General.GetListOf<Cotizacion9500Detalle>(q => q.Cotizacion9500ID == i9500ID && q.Estatus);

            // Se verifica si se cobró el anticipo, para cancelarlo
            if (o9500.Anticipo > 0)
            {
                var oAnticipo = General.GetEntity<Venta>(c => c.VentaID == o9500.AnticipoVentaID && c.Estatus);
                if (oAnticipo.VentaEstatusID == Cat.VentasEstatus.Cobrada || oAnticipo.VentaEstatusID == Cat.VentasEstatus.Completada)
                {
                    // Si se pagó, se genera una devolución
                    var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == o9500.AnticipoVentaID);
                    if (oVentaV.Pagado > 0)
                        VentasProc.GenerarDevolucionDeEfectivo(oVentaV.VentaID, oVentaV.Pagado);

                    // Se cancela la venta del anticipo
                    oAnticipo.VentaEstatusID = (oVentaV.Pagado > 0 ? Cat.VentasEstatus.Cancelada : Cat.VentasEstatus.CanceladaSinPago);
                    Guardar.Generico<Venta>(oAnticipo);
                }
            }
            
            // Se borran las partes del 9500, si no han sido usadas
            foreach (var oParte in o9500Detalle)
            {
                // Se valida que la parte no haya sido usada en ventas
                if (General.Exists<VentaDetalle>(q => q.ParteID == oParte.ParteID))
                    continue;
                // Se valida que la parte no haya sido usada en almacén
                if (General.Exists<MovimientoInventarioDetalle>(q => q.ParteID == oParte.ParteID))
                    continue;
                // Se borra -> Deshabilitado hasta tener las validaciones del Almacén
                Guardar.EliminarParte(oParte.ParteID);
            }

            // Se verifica si hay autorizaciones, para borrarlas
            var oAuts = General.GetListOf<Autorizacion>(c => (c.AutorizacionProcesoID == Cat.AutorizacionesProcesos.c9500PrecioFueraDeRango
                || c.AutorizacionProcesoID == Cat.AutorizacionesProcesos.c9500SinAnticipo) && c.Tabla == Cat.Tablas.Tabla9500 && c.TablaRegistroID == i9500ID && c.Estatus);
            foreach (var oReg in oAuts)
            {
                oReg.Estatus = true;
                Guardar.Generico<Autorizacion>(oReg);
            }

            // Se cancela el 9500
            o9500.EstatusGenericoID = Cat.EstatusGenericos.CanceladoAntesDeVender;
            o9500.BajaUsuarioID = iUsuarioID;
            o9500.BajaMotivo = sMotivo;
            Guardar.Generico<Cotizacion9500>(o9500);
        }

        public static void Completar9500(Cotizacion9500 o9500, decimal mSobrante, bool bDevolverEfectivo)
        {
            // Se cancela la venta del anticipo
            var oVentaAnt = General.GetEntity<Venta>(q => q.VentaID == o9500.AnticipoVentaID && q.VentaEstatusID != Cat.VentasEstatus.Cancelada && 
                q.VentaEstatusID != Cat.VentasEstatus.CanceladaSinPago && q.Estatus);
            if (oVentaAnt != null)
            {
                oVentaAnt.VentaEstatusID = Cat.VentasEstatus.Cancelada;
                Guardar.Generico<Venta>(oVentaAnt);
                // Se genera una devolución de efectivo (si se realizó un pago) de la venta cancelada, pues se generará una nueva venta con el importe total
                if (General.Exists<VentaPago>(q => q.VentaID == o9500.AnticipoVentaID && q.Estatus))
                    VentasProc.GenerarDevolucionDeEfectivo(o9500.AnticipoVentaID.Valor(), o9500.Anticipo - (mSobrante > 0 ? mSobrante : 0));
            }

            // Si hubo un sobrante, se genera nota de crédito o se devuelve efectivo
            if (mSobrante > 0)
            {
                if (bDevolverEfectivo)
                    VentasProc.GenerarDevolucionDeEfectivo(oVentaAnt.VentaID, mSobrante);
                else
                    VentasProc.GenerarNotaDeCredito(o9500.ClienteID, mSobrante, "", Cat.OrigenesNotaDeCredito.Anticipo9500, oVentaAnt.VentaID.ToString());
            }

            // Se modifica el Estatus del 9500
            o9500.EstatusGenericoID = Cat.EstatusGenericos.Completada;
            Guardar.Generico<Cotizacion9500>(o9500);
        }

        public static void Regresar9500DeCompletar(Cotizacion9500 o9500)
        {
            // Se restaura el dato de la venta y el estatus del 9500
            o9500.VentaID = null;
            o9500.EstatusGenericoID = Cat.EstatusGenericos.Pendiente;
            Guardar.Generico<Cotizacion9500>(o9500);
        }

        public static void EliminarPagosVenta(int iVentaID)
        {
            var oPagos = General.GetListOf<VentaPago>(c => c.VentaID == iVentaID && c.Estatus);
            foreach (var oPago in oPagos)
            {
                var oPagoDet = General.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == oPago.VentaPagoID && c.Estatus);
                foreach (var oPagoForma in oPagoDet)
                    Guardar.Eliminar<VentaPagoDetalle>(oPagoForma, true);
                Guardar.Eliminar<VentaPago>(oPago, true);
            }
        }

        public static decimal CalcularComisionGerente(decimal mUtilSucursalMinimo, decimal mUtilSucursal, decimal mIncrementoUtil, decimal mIncrementoFijo)
        {
            decimal mExcedente = (mUtilSucursal - mUtilSucursalMinimo);
            int iMultiplicador = (int)(mExcedente / mIncrementoUtil);
            decimal mComisionGerente = (mIncrementoFijo * iMultiplicador);
            return mComisionGerente;
        }

        public static CajaEgreso GenerarResguardo(decimal mImporte, int iSucursalID)
        {
            var oCajaEgreso = new CajaEgreso()
            {
                CajaTipoEgresoID = Cat.CajaTiposDeEgreso.Resguardo,
                Concepto = "RESGUARDO",
                Importe = mImporte,
                Fecha = DateTime.Now,
                SucursalID = iSucursalID,
                RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
            };
            Guardar.Generico<CajaEgreso>(oCajaEgreso);
            return oCajaEgreso;
        }

        public static CajaIngreso GenerarRefuerzo(decimal mImporte, int iSucursalID)
        {
            var oCajaIngreso = new CajaIngreso()
            {
                CajaTipoIngresoID = Cat.CajaTiposDeIngreso.Refuerzo,
                Concepto = "REFUERZO",
                Importe = mImporte,
                Fecha = DateTime.Now,
                SucursalID = iSucursalID,
                RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
            };
            Guardar.Generico<CajaIngreso>(oCajaIngreso);
            return oCajaIngreso;
        }

        #region [ Auxiliares ]

        public static string GenerarFormaDePago(List<VentasPagosDetalleView> oPagoDetalle)
        {
            string sFormaDePago = "", sNotasDeCredito = "";
            foreach (var oReg in oPagoDetalle)
            {
                if (oReg.FormaDePagoID == Cat.FormasDePago.Vale)
                {
                    oReg.FormaDePago += " ({0})";
                    sNotasDeCredito += (", " + oReg.NotaDeCreditoID.Valor().ToString());
                }
                sFormaDePago += (sFormaDePago.Contains(oReg.FormaDePago) ? "" : (", " + oReg.FormaDePago));

                if (oReg.FormaDePagoID == Cat.FormasDePago.Cheque ||
                    oReg.FormaDePagoID == Cat.FormasDePago.Tarjeta || oReg.FormaDePagoID == Cat.FormasDePago.Transferencia)
                    sFormaDePago += (" (" + oReg.Cuenta + ")");
            }
            if (sNotasDeCredito != "")
                sFormaDePago = string.Format(sFormaDePago, sNotasDeCredito.Substring(2));
            sFormaDePago = (sFormaDePago == "" ? "" : sFormaDePago.Substring(2));

            return sFormaDePago;
        }

        public static string GenerarFormaDePago(List<VentaPagoDetalle> oPagoDetalle)
        {
            List<VentasPagosDetalleView> oPagoD = null;

            oPagoD = new List<VentasPagosDetalleView>();
            TipoFormaPago oFormaDePago;
            foreach (var oForma in oPagoDetalle)
            {
                oFormaDePago = General.GetEntity<TipoFormaPago>(q => q.TipoFormaPagoID == oForma.TipoFormaPagoID && q.Estatus);
                oPagoD.Add(new VentasPagosDetalleView()
                {
                    FormaDePagoID = oForma.TipoFormaPagoID,
                    FormaDePago = oFormaDePago.NombreTipoFormaPago,
                    Cuenta = oForma.Cuenta
                });
            }

            return VentasProc.GenerarFormaDePago(oPagoD);
        }

        public static string GenerarFormaDePago(int iVentaID)
        {
            var oVentasP = General.GetListOf<VentasPagosDetalleView>(q => q.VentaID == iVentaID);
            return VentasProc.GenerarFormaDePago(oVentasP);
        }

        #endregion

        #region [ Leyendas ]

        public static void AgregarLeyenda(int iVentaID, string sLeyenda)
        {
            UtilDatos.AgregarTemporal(Cat.Procesos.VentasLeyendas, iVentaID, sLeyenda);
        }

        public static string ObtenerLeyenda(int iVentaID)
        {
            return UtilDatos.ObtenerTemporal(Cat.Procesos.VentasLeyendas, iVentaID);
        }

        public static string ObtenerQuitarLeyenda(int iVentaID)
        {
            return UtilDatos.ObtenerQuitarTemporal(Cat.Procesos.VentasLeyendas, iVentaID);
        }

        #endregion

        #region [ Impresión de tickets ]

        public static void TicketAgregarLeyendas(ref Report oReporte)
        {
            var oLeyendas = Config.ValoresVarios("Tickets.Leyenda");
            oReporte.SetParameterValue("Leyenda1", oLeyendas["Tickets.Leyenda1"]);
            oReporte.SetParameterValue("Leyenda2", oLeyendas["Tickets.Leyenda2"]);
            oReporte.SetParameterValue("Leyenda3", oLeyendas["Tickets.Leyenda3"]);
            oReporte.SetParameterValue("Leyenda4", oLeyendas["Tickets.Leyenda4"]);
            oReporte.SetParameterValue("Leyenda5", oLeyendas["Tickets.Leyenda5"]);
            oReporte.SetParameterValue("Leyenda6", oLeyendas["Tickets.Leyenda6"]);
            oReporte.SetParameterValue("Leyenda7", oLeyendas["Tickets.Leyenda7"]);
        }

        public static void GenerarTicketDeVenta(int iVentaID, List<ProductoVenta> oListaVenta)
        {
            var oVentaV = General.GetEntity<VentasView>(q => q.VentaID == iVentaID);
            var oVentaDetalle = General.GetListOf<VentasDetalleView>(q => q.VentaID == iVentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaTicket.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);

            // Se obtienen las formas de pago
            string sFormaDePago = VentasProc.GenerarFormaDePago(iVentaID);

            // Se modifican las descripciones, si aplica
            if (oListaVenta != null)
            {
                foreach (var oDet in oVentaDetalle)
                {
                    var oParteVenta = oListaVenta.FirstOrDefault(c => c.ParteID == oDet.ParteID && c.Cantidad == oDet.Cantidad
                        && c.PrecioUnitario == oDet.PrecioUnitario && c.Iva == oDet.Iva);
                    if (oParteVenta != null)
                        oDet.NombreParte = oParteVenta.NombreDeParte;
                }
            }

            // Se mandan los datos al reporte
            oRep.RegisterData(new List<VentasView>() { oVentaV }, "Venta");
            oRep.RegisterData(oVentaDetalle, "VentaDetalle");
            oRep.SetParameterValue("FormaDePago", sFormaDePago);
            oRep.SetParameterValue("TotalConLetra", Helper.ImporteALetra(oVentaV.Total).ToUpper());
            oRep.SetParameterValue("LeyendaDeVenta", VentasProc.ObtenerQuitarLeyenda(iVentaID));
            oRep.SetParameterValue("LeyendaVehiculo", (oVentaV.ClienteVehiculoID.HasValue ? UtilDatos.LeyendaDeVehiculo(oVentaV.ClienteVehiculoID.Value) : ""));
            oRep.SetParameterValue("Precio1", false);

            UtilLocal.EnviarReporteASalida("Reportes.VentaTicket.Salida", oRep);
        }

        public static void GenerarTicketDeVenta(int iVentaID)
        {
            VentasProc.GenerarTicketDeVenta(iVentaID, null);
        }

        public static void GenerarTicketDeCotizacion(VentasView oVenta, List<VentasDetalleView> oVentaDetalle)
        {
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaTicket.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);

            oRep.RegisterData(new List<VentasView>() { oVenta }, "Venta");
            oRep.RegisterData(oVentaDetalle, "VentaDetalle");
            oRep.SetParameterValue("TotalConLetra", Helper.ImporteALetra(oVenta.Total).ToUpper());
            oRep.SetParameterValue("Precio1", false);

            UtilLocal.EnviarReporteASalida("Reportes.VentaCotizacion.Salida", oRep);
        }

        public static void GenerarTicketPrecio1(int VentaID)
        {
            var oVentaDetalles = General.GetListOf<VentasDetalleView>(v => v.VentaID == VentaID);
            var oVenta = General.GetEntity<VentasView>(v => v.VentaID == VentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaTicket.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);
            
            decimal total=0;
            var oVentaPrecio1=new List<VentasDetalleView>();
            foreach (var oVentaDetalle in oVentaDetalles){
                var oParte=General.GetEntity<PartePrecio>(p => p.ParteID == oVentaDetalle.ParteID);
                var precioConIva = Helper.ConvertirDecimal(oParte.PrecioUno);
                var precioSinIva = UtilLocal.ObtenerPrecioSinIva(Helper.ConvertirDecimal(oParte.PrecioUno));
                var iva = Helper.ConvertirDecimal(oParte.PrecioUno) - precioSinIva;
                oVentaPrecio1.Add(new VentasDetalleView()
                {
                    VentaDetalleID = oVentaDetalle.VentaDetalleID,
                    VentaID = oVentaDetalle.VentaID,
                    ParteID = oVentaDetalle.ParteID,
                    NumeroParte = oVentaDetalle.NumeroParte,
                    NombreParte = oVentaDetalle.NombreParte,
                    Costo = oVentaDetalle.Costo,
                    CostoConDescuento = oVentaDetalle.CostoConDescuento,
                    Cantidad = oVentaDetalle.Cantidad,
                    Medida = oVentaDetalle.Medida,
                    LineaID = oVentaDetalle.LineaID,
                    PrecioUnitario = precioSinIva,
                    Iva = iva
                });
                total +=  precioConIva* oVentaDetalle.Cantidad;
            }
            var oVentasViewPrecio1 = new VentasView()
            {
                VentaID = oVenta.VentaID,
                Facturada = oVenta.Facturada,
                Folio = oVenta.Folio,
                ClienteID = oVenta.ClienteID,
                Cliente = oVenta.Cliente,
                SucursalID = oVenta.SucursalID,
                Sucursal = oVenta.Sucursal,
                VentaEstatusID = oVenta.VentaEstatusID,
                Estatus = oVenta.Estatus,
                Subtotal= UtilLocal.ObtenerPrecioSinIva(total),
                Iva = UtilLocal.ObtenerIvaDePrecio(total),
                Total = total,
                Pagado =oVenta.Pagado,
                ACredito = oVenta.ACredito,
                VendedorID = oVenta.VendedorID,
                Vendedor = oVenta.Vendedor,
                VendedorUsuario = oVenta.VendedorUsuario,
                ComisionistaID = oVenta.ComisionistaID,
                ClienteVehiculoID = oVenta.ClienteVehiculoID, 
                Kilometraje = oVenta.Kilometraje
            };
            

            oRep.RegisterData(new List<VentasView>() { oVentasViewPrecio1 }, "Venta");
            oRep.RegisterData(oVentaPrecio1, "VentaDetalle");
            oRep.SetParameterValue("TotalConLetra", Helper.ImporteALetra(oVentasViewPrecio1.Total).ToUpper());
            oRep.SetParameterValue("Precio1", true);
            UtilLocal.EnviarReporteASalida("Reportes.VentaTicket.Salida", oRep);
        }

        public static void GenerarTicketDe9500(int i9500ID)
        {
            var o9500V = General.GetEntity<Cotizaciones9500View>(q => q.Cotizacion9500ID == i9500ID);
            var o9500DetV = General.GetListOf<Cotizaciones9500DetalleView>(q => q.Cotizacion9500ID == i9500ID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "9500Ticket.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<Cotizaciones9500View>() { o9500V }, "c9500");
            oRep.RegisterData(o9500DetV, "c9500Detalle");
            oRep.SetParameterValue("AnticipoConLetra", Helper.ImporteALetra(o9500V.Anticipo).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.9500Ticket.Salida", oRep);
        }

        public static bool ReimprimirFactura(string sFactura)
        {
            var oFactura = General.GetEntity<VentasFacturasView>(q => q.SerieFolio == sFactura);

            string sRutaPdf = Config.Valor("Facturacion.RutaPdf");
            sRutaPdf += ("\\" + oFactura.Fecha.Year.ToString() + "\\" + oFactura.Fecha.Month.ToString() + "\\");
            sRutaPdf += (sFactura + ".pdf");
            if (File.Exists(sRutaPdf))
            {
                Process.Start(sRutaPdf);
                return true;
            }
            else
            {
                UtilLocal.MensajeAdvertencia("No se ha encontrado el archivo Pdf correspondiente a la factura especificada.");
                return false;
            }
        }

        public static void GenerarTicketDevolucion(int iDevolucionID)
        {
            var oDevolucionV = General.GetEntity<VentasDevolucionesView>(q => q.VentaDevolucionID == iDevolucionID);
            var oDevolucionDetalleV = General.GetListOf<VentasDevolucionesDetalleView>(q => q.VentaDevolucionID == iDevolucionID);
            var oVentaV = General.GetEntity<VentasView>(q => q.VentaID == oDevolucionV.VentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaDevolucionTicket.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<VentasDevolucionesView>() { oDevolucionV }, "Devolucion");
            oRep.RegisterData(oDevolucionDetalleV, "DevolucionDetalle");
            oRep.RegisterData(new List<VentasView>() { oVentaV }, "Venta");
            oRep.SetParameterValue("NC", (oDevolucionV.FormaDePagoID == Cat.FormasDePago.Vale));
            oRep.SetParameterValue("Accion", oDevolucionV.FormaDePago);
            oRep.SetParameterValue("FolioNC", oDevolucionV.NotaDeCreditoID.Valor());
            oRep.SetParameterValue("TotalConLetra", Helper.ImporteALetra(oDevolucionV.Total.Valor()).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.VentaDevolucionTicket.Salida", oRep);
        }

        public static void GenerarTicketGarantia(int iGarantiaID)
        {
            var oGarantiaV = General.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == iGarantiaID);
            var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == oGarantiaV.VentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaGarantiaTicket.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<VentasGarantiasView>() { oGarantiaV }, "Garantia");
            oRep.RegisterData(new List<VentasView>() { oVentaV }, "Venta");
            // oRep.SetParameterValue("NC", (oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito));
            oRep.SetParameterValue("TotalConLetra", Helper.ImporteALetra(oGarantiaV.Total.Valor()).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.VentaGarantiaTicket.Salida", oRep);
        }

        public static void GenerarTicketCobranza(string sFolio)
        {
            var oCobradas = General.GetListOf<CobranzaTicket>(q => q.Ticket == sFolio);
            if (oCobradas.Count <= 0) return;

            // Se obtiene el total de lo pagado
            decimal mTotalPagado = oCobradas.Sum(c => c.Pagado);
            string sPagadoLetra = Helper.ImporteALetra(mTotalPagado).ToUpper();

            int iClienteID = oCobradas[0].ClienteID;
            var oClienteV = General.GetEntity<ClientesDatosView>(q => q.ClienteID == iClienteID);

            // Se genera el reporte
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CobranzaTicket.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<ClientesDatosView>() { oClienteV }, "Cliente");
            oRep.RegisterData(oCobradas, "Ventas");
            oRep.SetParameterValue("TotalPagado", mTotalPagado);
            oRep.SetParameterValue("TotalPagadoLetra", sPagadoLetra);
            //
            UtilLocal.EnviarReporteASalida("Reportes.CobranzaTicket.Salida", oRep);
        }

        public static void GenerarTicketGasto(int iEgresoID)
        {
            var oEgresoV = General.GetEntity<CajaEgresosView>(c => c.CajaEgresoID == iEgresoID);
            if (oEgresoV == null) return;
            
            // Se genera el reporte
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CajaGastoTicket.frx");
            // VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<CajaEgresosView>() { oEgresoV }, "CajaEgreso");
            //
            UtilLocal.EnviarReporteASalida("Reportes.CajaGastoTicket.Salida", oRep);
        }

        public static void GenerarTicketNotaDeCredito(int iNotaDeCreditoID)
        {
            var oNotaDeCreditoV = General.GetEntity<NotasDeCreditoView>(c => c.NotaDeCreditoID == iNotaDeCreditoID);
            if (oNotaDeCreditoV == null) return;

            // Se genera el reporte
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "NotaDeCreditoTicket.frx");
            // VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<NotasDeCreditoView>() { oNotaDeCreditoV }, "NotaDeCredito");
            //
            UtilLocal.EnviarReporteASalida("Reportes.NotaDeCreditoTicket.Salida", oRep);
        }

        #endregion
        
        #region [ Facturación electrónica ]

        public static Dictionary<string, string> GenerarFolioDeFactura()
        {
            var Res = new Dictionary<string, string>();
            Res.Add("Serie", Config.Valor("Facturacion.Serie"));
            Res.Add("Folio", Config.Valor("Facturacion.Folio"));
            Config.EstablecerValor("Facturacion.Folio", (Helper.ConvertirEntero(Res["Folio"]) + 1).ToString());
            return Res;
        }

        public static Dictionary<string, string> GenerarFolioDeFacturaDevolucion()
        {
            var Res = new Dictionary<string, string>();
            Res.Add("Serie", Config.Valor("Facturacion.Devolucion.Serie"));
            Res.Add("Folio", Config.Valor("Facturacion.Devolucion.Folio"));
            Config.EstablecerValor("Facturacion.Devolucion.Folio", (Helper.ConvertirEntero(Res["Folio"]) + 1).ToString());
            return Res;
        }

        private static void FeLLenarConfiguracion(ref FacturaElectronica oFacturaE, Dictionary<string, string> oConfig)
        {
            oFacturaE.Configuracion = new FacturacionElectronica.Config()
            {
                RutaCertificado = oConfig["Facturacion.RutaCertificado"],
                RutaArchivoKey = oConfig["Facturacion.RutaArchivoKey"],
                ContraseniaArchivoKey = oConfig["Facturacion.ContraseniaArchivoKey"],
                RutaArchivoPfx = oConfig["Facturacion.RutaArchivoPfx"],
                ContraseniaArchivoPfx = oConfig["Facturacion.ContraseniaArchivoPfx"],
                UsuarioPac = oConfig["Facturacion.UsuarioPac"],
                ContraseniaPac = oConfig["Facturacion.ContraseniaPac"]
            };
        }

        private static void FeLlenarDatosComunes(ref FacturaElectronica oFacturaE, Dictionary<string, string> oConfig)
        {
            // Se llenan los valores de configuración
            VentasProc.FeLLenarConfiguracion(ref oFacturaE, oConfig);

            // Se llenan los datos del Emisor
            var oConfigMatriz = Config.ValoresVarios(Cat.Sucursales.Matriz, "Facturacion.");
            oFacturaE.Emisor = new Emisor()
            {
                RFC = oConfig["Facturacion.Rfc"],
                Nombre = oConfig["Facturacion.RazonSocial"],
                DomicilioFiscal = new Ubicacion()
                {
                    Calle = oConfigMatriz["Facturacion.Calle"],
                    NumeroExterior = oConfigMatriz["Facturacion.NumeroExterior"],
                    NumeroInterior = oConfigMatriz["Facturacion.NumeroInterior"],
                    Referencia = oConfigMatriz["Facturacion.Referencia"],
                    Colonia = oConfigMatriz["Facturacion.Colonia"],
                    CodigoPostal = oConfigMatriz["Facturacion.CodigoPostal"],
                    Localidad = oConfigMatriz["Facturacion.Localidad"],
                    Municipio = oConfigMatriz["Facturacion.Municipio"],
                    Estado = oConfigMatriz["Facturacion.Estado"],
                    Pais = oConfigMatriz["Facturacion.Pais"]
                },
                ExpedidoEn = new Ubicacion()
                {
                    Calle = oConfig["Facturacion.Calle"],
                    NumeroExterior = oConfig["Facturacion.NumeroExterior"],
                    NumeroInterior = oConfig["Facturacion.NumeroInterior"],
                    Referencia = oConfig["Facturacion.Referencia"],
                    Colonia = oConfig["Facturacion.Colonia"],
                    CodigoPostal = oConfig["Facturacion.CodigoPostal"],
                    Localidad = oConfig["Facturacion.Localidad"],
                    Municipio = oConfig["Facturacion.Municipio"],
                    Estado = oConfig["Facturacion.Estado"],
                    Pais = oConfig["Facturacion.Pais"]
                },
                RegimenesFiscales = new List<string>(oConfig["Facturacion.RegimenesFiscales"].Split(','))
            };
        }

        private static ResAcc<bool> FeLlenarDatosReceptor(ref FacturaElectronica oFacturaE, int iClienteID)
        {
            var oClienteF = General.GetEntity<ClientesFacturacionView>(q => q.ClienteID == iClienteID);
            if (oClienteF == null)
                return new ResAcc<bool>(false, "El cliente seleccionado no tiene datos de facturación registrados. No se puede generar la factura.");
            oFacturaE.Receptor = new Receptor()
            {
                RFC = oClienteF.Rfc,
                Nombre = oClienteF.RazonSocial,
                DomicilioFiscal = new Ubicacion()
                {
                    Calle = oClienteF.Calle,
                    NumeroExterior = oClienteF.NumeroExterior,
                    NumeroInterior = oClienteF.NumeroInterior,
                    Referencia = oClienteF.Referencia,
                    Colonia = oClienteF.Colonia,
                    CodigoPostal = oClienteF.CodigoPostal,
                    Localidad = oClienteF.Localidad,
                    Municipio = oClienteF.Municipio,
                    Estado = oClienteF.Estado,
                    Pais = oClienteF.Pais
                }
            };

            return new ResAcc<bool>(true);
        }

        private static ResAcc<string> FeEnviarFactura(ref FacturaElectronica oFacturaE)
        {
            // Se genera el Xml inicial, con el formato que pide el Sat
            var ResXml = oFacturaE.GenerarFactura(true);
            if (ResXml.Error)
                return new ResAcc<string>(false, ResXml.Mensaje);
            string sCfdi = ResXml.Respuesta;

            // Se manda a timbrar el Xml con el proveedor Pac
            ResXml = oFacturaE.TimbrarFactura(sCfdi, !GlobalClass.Produccion);
            if (ResXml.Error)
                return new ResAcc<string>(false, ResXml.Mensaje);
            string sCfdiTimbrado = ResXml.Respuesta;

            var Res = new ResAcc<string>(true);
            Res.Respuesta = sCfdiTimbrado;
            return Res;
        }

        private static Report FeGuardarArchivosFactura(ref FacturaElectronica oFacturaE, string sCfdiTimbrado, string sSerieFolio)
        {
            // Se obtienen las rutas a utilizar
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = Config.Valor("Facturacion.RutaPdf");
            string sRutaXml = Config.Valor("Facturacion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += sAnioMes;
            Directory.CreateDirectory(sRutaPdf);
            sRutaXml += sAnioMes;
            Directory.CreateDirectory(sRutaXml);

            // Se guarda el xml
            string sArchivoXml = (sRutaXml + sSerieFolio + ".xml");
            File.WriteAllText(sArchivoXml, sCfdiTimbrado, Encoding.UTF8);

            // Se manda la salida de la factura
            string sArchivoPdf = (sRutaPdf + sSerieFolio + ".pdf");
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "Cfdi.frx");
            var ObjetoCbb = new { Imagen = oFacturaE.GenerarCbb() };
            oRep.RegisterData(new List<FacturaElectronica>() { oFacturaE }, "Factura");
            oRep.RegisterData(new List<object>() { ObjetoCbb }, "Cbb");
            oRep.SetParameterValue("ACredito", false);
            oRep.SetParameterValue("Vendedor", "");
            if (oFacturaE.Adicionales != null) {
                if (oFacturaE.Adicionales.ContainsKey("ACredito"))
                    oRep.SetParameterValue("ACredito", bool.Parse(oFacturaE.Adicionales["ACredito"]));
                if (oFacturaE.Adicionales.ContainsKey("Vendedor"))
                    oRep.SetParameterValue("Vendedor", oFacturaE.Adicionales["Vendedor"].ToUpper());
                if (oFacturaE.Adicionales.ContainsKey("Vencimiento"))
                    oRep.SetParameterValue("Vencimiento", oFacturaE.Adicionales["Vencimiento"]);
                if (oFacturaE.Adicionales.ContainsKey("LeyendaDeVenta"))
                    oRep.SetParameterValue("LeyendaDeVenta", oFacturaE.Adicionales["LeyendaDeVenta"]);
                if (oFacturaE.Adicionales.ContainsKey("LeyendaDeVehiculo"))
                    oRep.SetParameterValue("LeyendaDeVehiculo", oFacturaE.Adicionales["LeyendaDeVehiculo"]);
            }
            oRep.SetParameterValue("TotalConLetra", Helper.ImporteALetra(oFacturaE.Total).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.VentaFactura.Salida", oRep);

            // Se guarda el pdf
            var oRepPdf = new FastReport.Export.Pdf.PDFExport() { ShowProgress = true };
            oRep.Prepare();
            oRep.Export(oRepPdf, sArchivoPdf);

            return oRep;
        }

        private static Report FeGuardarArchivosFacturaDevolucion(ref FacturaElectronica oFacturaE, string sCfdiTimbrado, string sSerieFolio)
        {
            // Se obtienen las rutas a utilizar
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = Config.Valor("Facturacion.Devolucion.RutaPdf");
            string sRutaXml = Config.Valor("Facturacion.Devolucion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += sAnioMes;
            Directory.CreateDirectory(sRutaPdf);
            sRutaXml += sAnioMes;
            Directory.CreateDirectory(sRutaXml);

            // Se guarda el xml
            string sArchivoXml = (sRutaXml + sSerieFolio + ".xml");
            File.WriteAllText(sArchivoXml, sCfdiTimbrado, Encoding.UTF8);

            // Se manda la salida de la factura
            string sArchivoPdf = (sRutaPdf + sSerieFolio + ".pdf");
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CfdiNotaDeCredito.frx");
            var ObjetoCbb = new { Imagen = oFacturaE.GenerarCbb() };
            oRep.RegisterData(new List<FacturaElectronica>() { oFacturaE }, "Factura");
            oRep.RegisterData(new List<object>() { ObjetoCbb }, "Cbb");
            oRep.SetParameterValue("Vendedor", oFacturaE.Adicionales["Vendedor"].ToUpper());
            oRep.SetParameterValue("TotalConLetra", Helper.ImporteALetra(oFacturaE.Total).ToUpper());
            oRep.SetParameterValue("FacturaOrigen", oFacturaE.Adicionales["FacturaOrigen"]);

            UtilLocal.EnviarReporteASalida("Reportes.VentaFacturaDevolucion.Salida", oRep);

            // Se guarda el pdf
            var oRepPdf = new FastReport.Export.Pdf.PDFExport() { ShowProgress = true };
            oRep.Prepare();
            oRep.Export(oRepPdf, sArchivoPdf);

            return oRep;
        }

        private static bool EnviarFacturaPorCorreo(int iFacturaID)
        {
            // Se verifica si es pruebas, en cuyo caso, no se envía el correo
            if (!GlobalClass.Produccion) return false;

            // Se obtienen los correos para envío
            var oVentaFactura = General.GetEntity<VentaFactura>(q => q.VentaFacturaID == iFacturaID && q.Estatus);
            string sPara = VentasProc.ObtenerCorreosPersonalCliente(oVentaFactura.ClienteID);
            if (sPara == "") return false;

            // Se obtienen las rutas de los archivos
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = Config.Valor("Facturacion.RutaPdf");
            string sRutaXml = Config.Valor("Facturacion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += (sAnioMes + oVentaFactura.Serie + oVentaFactura.Folio + ".pdf");
            sRutaXml += (sAnioMes + oVentaFactura.Serie + oVentaFactura.Folio + ".xml");

            string sRutaFormato = (Config.Valor("Correo.RutaFormatos") + "CorreoFactura.html");

            if (!File.Exists(sRutaPdf) || !File.Exists(sRutaXml) || !File.Exists(sRutaFormato))
                return false;

            // Se carga el formato
            string sMensaje = File.ReadAllText(sRutaFormato);
            string sAsunto = sMensaje.Extraer("<title>", "</title>");
            // Se cargan los archivos como Sreams
            var oPdf = new FileStream(sRutaPdf, FileMode.Open);
            var oXml = new FileStream(sRutaXml, FileMode.Open);
            var oAdjuntos = new Dictionary<string, Stream>();
            oAdjuntos.Add(Path.GetFileName(sRutaPdf), oPdf);
            oAdjuntos.Add(Path.GetFileName(sRutaXml), oXml);
                        
            // Se procesa el envío de correo
            return UtilLocal.EnviarCorreo(sAsunto, sMensaje, sPara, oAdjuntos);
        }

        private static bool EnviarFacturaCancelacionPorCorreo(int iFacturaID)
        {
            // Se obtienen los correos para envío
            var oVentaFactura = General.GetEntity<VentaFactura>(q => q.VentaFacturaID == iFacturaID && q.Estatus);
            string sPara = VentasProc.ObtenerCorreosPersonalCliente(oVentaFactura.ClienteID);
            if (sPara == "") return false;

            // Se obtienen las rutas de los archivos
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = Config.Valor("Facturacion.RutaPdf");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += (sAnioMes + oVentaFactura.Serie + oVentaFactura.Folio + ".pdf");

            string sRutaFormato = (Config.Valor("Correo.RutaFormatos") + "CorreoFacturaCancelacion.html");

            if (!File.Exists(sRutaPdf) || !File.Exists(sRutaFormato))
                return false;

            // Se carga el formato
            string sMensaje = File.ReadAllText(sRutaFormato);
            string sAsunto = sMensaje.Extraer("<title>", "</title>");
            // Se procesan campos
            sMensaje = sMensaje.Replace("{__FolioInterno}", (oVentaFactura.Serie + oVentaFactura.Folio));
            sMensaje = sMensaje.Replace("{__FolioFiscal}", oVentaFactura.FolioFiscal);
            sMensaje = sMensaje.Replace("{__Fecha}", oVentaFactura.Fecha.ToString(GlobalClass.FormatoFechaHora));
            // Se cargan los archivos como Sreams
            var oPdf = new FileStream(sRutaPdf, FileMode.Open);
            var oAdjuntos = new Dictionary<string, Stream>();
            oAdjuntos.Add(Path.GetFileName(sRutaPdf), oPdf);

            // Se procesa el envío de correo
            return UtilLocal.EnviarCorreo(sAsunto, sMensaje, sPara, oAdjuntos);
        }

        private static bool EnviarFacturaDevolucionPorCorreo(int iFacturaDevolucionID)
        {
            // Se obtienen los correos para envío
            var oFacturaDev = General.GetEntity<VentaFacturaDevolucion>(q => q.VentaFacturaDevolucionID == iFacturaDevolucionID && q.Estatus);
            var oFactura = General.GetEntity<VentaFactura>(q => q.VentaFacturaID == oFacturaDev.VentaFacturaID && q.Estatus);
            string sPara = VentasProc.ObtenerCorreosPersonalCliente(oFactura.ClienteID);
            if (sPara == "") return false;

            // Se obtienen las rutas de los archivos
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = Config.Valor("Facturacion.Devolucion.RutaPdf");
            string sRutaXml = Config.Valor("Facturacion.Devolucion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += (sAnioMes + oFacturaDev.Serie + oFacturaDev.Folio + ".pdf");
            sRutaXml += (sAnioMes + oFacturaDev.Serie + oFacturaDev.Folio + ".xml");

            string sRutaFormato = (Config.Valor("Correo.RutaFormatos") + "CorreoFacturaDevolucion.html");

            if (!File.Exists(sRutaPdf) || !File.Exists(sRutaXml) || !File.Exists(sRutaFormato))
                return false;

            // Se carga el formato
            string sMensaje = File.ReadAllText(sRutaFormato);
            string sAsunto = sMensaje.Extraer("<title>", "</title>");
            // Se procesan campos
            sMensaje = sMensaje.Replace("{__FolioInterno}", (oFactura.Serie + oFactura.Folio));
            sMensaje = sMensaje.Replace("{__FolioFiscal}", oFactura.FolioFiscal);
            sMensaje = sMensaje.Replace("{__Fecha}", oFactura.Fecha.ToString(GlobalClass.FormatoFechaHora));
            // Se cargan los archivos como Sreams
            var oPdf = new FileStream(sRutaPdf, FileMode.Open);
            var oXml = new FileStream(sRutaXml, FileMode.Open);
            var oAdjuntos = new Dictionary<string, Stream>();
            oAdjuntos.Add(Path.GetFileName(sRutaPdf), oPdf);
            oAdjuntos.Add(Path.GetFileName(sRutaXml), oXml);

            // Se procesa el envío de correo
            return UtilLocal.EnviarCorreo(sAsunto, sMensaje, sPara, oAdjuntos);
        }

        private static string ObtenerCorreosPersonalCliente(int iClienteID)
        {
            var oPersonas = General.GetListOf<ClientePersonal>(q => q.ClienteID == iClienteID && q.EnviarCfdi && q.Estatus);
            string sPara = "";
            foreach (var oPersona in oPersonas)
            {
                if (Helper.IsEmailValid(oPersona.CorreoElectronico))
                    sPara += (", " + oPersona.CorreoElectronico);
            }
            // if (sPara == "")
            //     return (oPersonas.Count <= 0);
            // sPara = sPara.Substring(2);
            sPara = (sPara == "" ? "" : sPara.Substring(2));
            return sPara;
        }

        public static ResAcc<int> GenerarFacturaElectronica(List<int> VentasIDs, int iClienteID
            , List<ProductoVenta> oListaVenta, string sFormaDePago, string sObservacion)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);            

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtienen las ventas a facturar, y los vendedores
            var oVentas = new List<VentasView>();
            string sVendedores = "";
            foreach (int iVentaID in VentasIDs)
            {
                var oVentaV = General.GetEntity<VentasView>(q => q.VentaID == iVentaID);
                oVentas.Add(oVentaV);
                sVendedores += (", " + oVentaV.Vendedor);
            }
            sVendedores = (sVendedores == "" ? "" : sVendedores.Substring(2));
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);
            if (oVentas.Count > 0)
            {
                oFacturaE.Adicionales.Add("ACredito", oVentas[0].ACredito.ToString());
                // Se llena el dato de la fecha de vencimiento, sólo de la primer venta
                int iClienteVenID = oVentas[0].ClienteID;
                var oCliente = General.GetEntity<Cliente>(q => q.ClienteID == iClienteVenID && q.Estatus);
                oFacturaE.Adicionales.Add("Vencimiento", oVentas[0].Fecha.AddDays(oCliente.DiasDeCredito.Valor()).ToShortDateString());
                // Leyenda de la venta
                oFacturaE.Adicionales.Add("LeyendaDeVenta", VentasProc.ObtenerQuitarLeyenda(oVentas[0].VentaID));
                // Leyenda de vehículo, si aplica
                if (oVentas[0].ClienteVehiculoID.HasValue)
                    oFacturaE.Adicionales.Add("LeyendaDeVehiculo", UtilDatos.LeyendaDeVehiculo(oVentas[0].ClienteVehiculoID.Value));
            }
            
            // Se llenan los datos referentes a la forma de pago
            oFacturaE.MetodoDePago = sFormaDePago;
            oFacturaE.MetodoDePago = (string.IsNullOrEmpty(oFacturaE.MetodoDePago) ? "No identificado" : oFacturaE.MetodoDePago);

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Ingreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();

            // Se llenan los datos del receptor
            var ResRec = VentasProc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            decimal mUnitarioTotal = 0, mIvaTotal = 0;
            oFacturaE.Conceptos = new List<Concepto>();
            if (oListaVenta == null)
            {
                List<VentasDetalleView> oVentaDetalle;
                foreach (var oVentaCon in oVentas)
                {
                    oVentaDetalle = General.GetListOf<VentasDetalleView>(q => q.VentaID == oVentaCon.VentaID);
                    foreach (var oConcepto in oVentaDetalle)
                    {
                        oFacturaE.Conceptos.Add(new Concepto()
                        {
                            Identificador = oConcepto.NumeroParte,
                            Cantidad = oConcepto.Cantidad,
                            Unidad = oConcepto.Medida,
                            Descripcion = oConcepto.NombreParte,
                            ValorUnitario = oConcepto.PrecioUnitario,
                            Iva = oConcepto.Iva
                        });

                        mUnitarioTotal += oConcepto.PrecioUnitario;
                        mIvaTotal += oConcepto.Iva;
                    }
                }
            }
            else
            {
                foreach (var oConcepto in oListaVenta)
                {
                    oFacturaE.Conceptos.Add(new Concepto()
                    {
                        Identificador = oConcepto.NumeroDeParte,
                        Cantidad = oConcepto.Cantidad,
                        Unidad = oConcepto.UnidadDeMedida,
                        Descripcion = oConcepto.NombreDeParte,
                        ValorUnitario = oConcepto.PrecioUnitario,
                        Iva = oConcepto.Iva
                    });

                    mUnitarioTotal += oConcepto.PrecioUnitario;
                    mIvaTotal += oConcepto.Iva;
                }
            }

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasProc.FeEnviarFactura(ref oFacturaE);
            if (ResXml.Error)
                return new ResAcc<int>(false, ResXml.Mensaje);
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oFolioFactura = VentasProc.GenerarFolioDeFactura();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oVentaFactura = new VentaFactura()
            {
                Fecha = dAhora,
                FolioFiscal = oFacturaE.Timbre.FolioFiscal,
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                ClienteID = iClienteID,
                Observacion = sObservacion,
                Subtotal = mUnitarioTotal,
                Iva = mIvaTotal
            };
            var oVentaFacturaD = new List<VentaFacturaDetalle>();
            foreach (var oVentaFac in oVentas)
            {
                var oFacturaDet = new VentaFacturaDetalle() { VentaID = oVentaFac.VentaID };

                // Se revisa si es la primera factura para esta venta
                if (!General.Exists<VentaFacturaDetalle>(c => c.VentaID == oVentaFac.VentaID && c.Estatus))
                    oFacturaDet.Primera = true;

                oVentaFacturaD.Add(oFacturaDet);
            }
            Guardar.Factura(oVentaFactura, oVentaFacturaD);

            // Se escribe el folio de factura en cada venta
            string sSerieFolio = (oFacturaE.Serie + oFacturaE.Folio);
            foreach (int iVentaID in VentasIDs)
            {
                var oVenta = General.GetEntity<Venta>(q => q.VentaID == iVentaID && q.Estatus);
                oVenta.Facturada = true;
                oVenta.Folio = sSerieFolio;
                Guardar.Generico<Venta>(oVenta);
            }

            // Se manda guardar la factura, en pdf y xml. También se manda a salida
            var oRep = VentasProc.FeGuardarArchivosFactura(ref oFacturaE, sCfdiTimbrado, sSerieFolio);
            
            // Se manda la factura por correo
            VentasProc.EnviarFacturaPorCorreo(oVentaFactura.VentaFacturaID);

            return new ResAcc<int>(true, oVentaFactura.VentaFacturaID);
        }
        /*
        public static ResAcc<int> GenerarFacturaElectronica(List<int> VentasIDs, int iClienteID, List<VentasPagosDetalleView> oFormasDePago, string sObservacion)
        {
            return VentasProc.GenerarFacturaElectronica(VentasIDs, iClienteID, null, oFormasDePago, sObservacion);
        }
        */
        public static ResAcc<int> GenerarFacturaElectronica(List<int> VentasIDs, int iClienteID, string sObservacion)
        {
            return VentasProc.GenerarFacturaElectronica(VentasIDs, iClienteID, null, null, sObservacion);
        }

        public static ResAcc<int> GenerarFacturaElectronica(int iVentaID, int iClienteID, string sObservacion)
        {
            return VentasProc.GenerarFacturaElectronica(new List<int>() { iVentaID }, iClienteID, null, null, sObservacion);
        }

        public static ResAcc<int> GenerarFacturaGlobal(string sConceptoVentas, decimal mCostoTotal, decimal mTotalVentas
            , string sConceptoCancelaciones, decimal mTotalCancelaciones, string sConceptoDevoluciones, decimal mTotalDevoluciones)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            int iClienteID = Cat.Clientes.Mostrador;
            DateTime dAhora = DateTime.Now;
                        
            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Ingreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = "Varios";

            // Se llenan los datos del receptor
            var ResRec = VentasProc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);
                        
            // Se calcula el Iva
            decimal mVentasValor = UtilLocal.ObtenerPrecioSinIva(mTotalVentas);
            decimal mVentasIva = UtilLocal.ObtenerIvaDePrecio(mTotalVentas);
            decimal mCancelacionesValor = UtilLocal.ObtenerPrecioSinIva(mTotalCancelaciones);
            decimal mCancelacionesIva = UtilLocal.ObtenerIvaDePrecio(mTotalCancelaciones);
            decimal mDevolucionesValor = UtilLocal.ObtenerPrecioSinIva(mTotalDevoluciones);
            decimal mDevolucionesIva = UtilLocal.ObtenerIvaDePrecio(mTotalDevoluciones);

            // Se llenan los conceptos de la factura
            const string sUnidad = "U";
            oFacturaE.Conceptos = new List<Concepto>();
            // Ventas
            oFacturaE.Conceptos.Add(new Concepto()
            {
                Cantidad = 1,
                Unidad = sUnidad,
                Descripcion = sConceptoVentas,
                ValorUnitario = mVentasValor,
                Iva = mVentasIva
            });
            // Cancelaciones
            oFacturaE.Conceptos.Add(new Concepto()
            {
                Cantidad = 1,
                Unidad = sUnidad,
                Descripcion = sConceptoCancelaciones,
                ValorUnitario = (mCancelacionesValor * -1),
                Iva = (mCancelacionesIva * -1)
            });
            // Devoluciones
            oFacturaE.Conceptos.Add(new Concepto()
            {
                Cantidad = 1,
                Unidad = sUnidad,
                Descripcion = sConceptoDevoluciones,
                ValorUnitario = (mDevolucionesValor * -1),
                Iva = (mDevolucionesIva * -1)
            });

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasProc.FeEnviarFactura(ref oFacturaE);
            if (ResXml.Error)
                return new ResAcc<int>(false, ResXml.Mensaje);
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oFolioFactura = VentasProc.GenerarFolioDeFactura();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oVentaFactura = new VentaFactura()
            {
                Fecha = dAhora,
                FolioFiscal = oFacturaE.Timbre.FolioFiscal,
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                ClienteID = iClienteID,
                Costo = mCostoTotal,
                Subtotal = (mVentasValor - mCancelacionesValor - mDevolucionesValor),
                Iva = (mVentasIva - mCancelacionesIva - mDevolucionesIva)
            };
            var oVentaFacturaD = new List<VentaFacturaDetalle>();
            // No se agrega ningún detalle, para que las ventas aquí facturadas no aparezcan como facturadas
            // foreach (var oVenta in oVentas)
            //     oVentaFacturaD.Add(new VentaFacturaDetalle() { VentaID = oVenta.VentaID });
            Guardar.Factura(oVentaFactura, oVentaFacturaD);

            // Se manda guardar la factura, en pdf y xml
            var oRep = VentasProc.FeGuardarArchivosFactura(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda imprimir la factura
            // ..

            return new ResAcc<int>(true, oVentaFactura.VentaFacturaID);
        }
                
        public static ResAcc<bool> ValidarDatosParaFactura(List<int> VentasIDs, int iClienteID)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtienen las ventas a facturar
            var oVentas = new List<VentasView>();

            foreach (int iVentaID in VentasIDs)
                oVentas.Add(General.GetEntity<VentasView>(q => q.VentaID == iVentaID));

            // Se llenan un dato "x" para simular el método de pago, pues no se tiene esa información todavía porque no se ha guardado la venta
            oFacturaE.MetodoDePago = "Efectivo";

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Ingreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();

            // Se llenan los datos del receptor
            var ResRec = VentasProc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<bool>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            oFacturaE.Conceptos = new List<Concepto>();
            List<VentasDetalleView> oVentaDetalle;
            foreach (var oVenta in oVentas)
            {
                oVentaDetalle = General.GetListOf<VentasDetalleView>(q => q.VentaID == oVenta.VentaID);
                foreach (var oConcepto in oVentaDetalle)
                {
                    oFacturaE.Conceptos.Add(new Concepto()
                    {
                        Identificador = oConcepto.ParteID.ToString(),
                        Cantidad = oConcepto.Cantidad,
                        Unidad = oConcepto.Medida,
                        Descripcion = oConcepto.NombreParte,
                        ValorUnitario = oConcepto.PrecioUnitario,
                        Iva = oConcepto.Iva
                    });
                }
            }

            // Se genera el Xml inicial, con el formato que pide el Sat
            var ResXml = oFacturaE.GenerarFactura(true);
            if (ResXml.Error)
                return new ResAcc<bool>(false, ResXml.Mensaje);

            return new ResAcc<bool>(true, true);
        }

        public static ResAcc<bool> ValidarDatosParaFactura(int iVentaID, int iClienteID)
        {
            return VentasProc.ValidarDatosParaFactura(new List<int>() { iVentaID }, iClienteID);
        }

        public static ResAcc<int> GenerarFacturaCancelacion(int iFacturaID, List<int> oIdsDevoluciones)
        {
            // Se obtiene el folio fiscal
            var oFactura = General.GetEntity<VentaFactura>(c => c.VentaFacturaID == iFacturaID && c.Estatus);
            string sFolioFiscal = oFactura.FolioFiscal;

            // Se generan los datos de la cancelación
            DateTime dAhora = DateTime.Now;
            var oFacturaDevolucion = new VentaFacturaDevolucion()
            {
                VentaFacturaID = iFacturaID,
                Fecha = dAhora,
                EsCancelacion = true
            };
            // Se genera el detalle de la devolución de factura, con los Ids de las devoluciones incluidas
            var oFacturaDevDet = new List<VentaFacturaDevolucionDetalle>();
            foreach (int iDevolucionID in oIdsDevoluciones)
                oFacturaDevDet.Add(new VentaFacturaDevolucionDetalle() { VentaDevolucionID = iDevolucionID });
            Guardar.FacturaDevolucion(oFacturaDevolucion, oFacturaDevDet);

            // Se manda cancelar la factura, y completar los procesos correspondientes
            var ResCanc = VentasProc.GenerarFacturaCancelacion(sFolioFiscal, oFacturaDevolucion.VentaFacturaDevolucionID);

            /* Ya no se sale, pues aunque haya error, se deben guardar los datos, ya que la venta sí se canceló
            if (ResC.Error)
                return new ResAcc<bool>(false, ResC.Mensaje);
            */

            var Res = new ResAcc<int>(ResCanc.Exito, ResCanc.Mensaje);
            Res.Respuesta = oFacturaDevolucion.VentaFacturaDevolucionID;

            return Res;
        }

        public static ResAcc<bool> GenerarFacturaCancelacion(string sFolioFiscal, int iVentaFacturaDevolucionID)
        {
            // Se manda hacer la cancelación
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            VentasProc.FeLLenarConfiguracion(ref oFacturaE, oConfig);
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);
            var ResC = oFacturaE.CancelarFactura(sFolioFiscal, !GlobalClass.Produccion);
            bool bFacturada = ResC.Exito;

            // Se guardan datos del resultado de la cancelación ante Sat, si se canceló
            if (bFacturada)
            {
                // Se obtiene el objeto de la cancelacion
                var oFacturaDevolucion = General.GetEntity<VentaFacturaDevolucion>(q => q.VentaFacturaDevolucionID == iVentaFacturaDevolucionID);

                oFacturaDevolucion.Ack = ResC.Respuesta.Izquierda(36);
                oFacturaDevolucion.Procesada = true;
                oFacturaDevolucion.FechaProcesada = DateTime.Now;
                Guardar.Generico<VentaFacturaDevolucion>(oFacturaDevolucion);

                // Se le cambia el estatus a la factura
                var oFactura = General.GetEntity<VentaFactura>(q => q.VentaFacturaID == oFacturaDevolucion.VentaFacturaID && q.Estatus);
                oFactura.EstatusGenericoID = Cat.EstatusGenericos.Cancelada;
                Guardar.Generico<VentaFactura>(oFactura);

                // Se manda la notificación por correo, si fue exitosa
                VentasProc.EnviarFacturaCancelacionPorCorreo(oFacturaDevolucion.VentaFacturaID);
            }

            return new ResAcc<bool>(bFacturada, ResC.Mensaje);
        }

        /*
        public static ResAcc<int> GenerarFacturaDevolucion(int iDevolucionID, int iUsuarioID)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;
            var oDev = General.GetEntity<VentasDevolucionesView>(q => q.VentaDevolucionID == iDevolucionID);

            // Se obtiene el nombre del vendedor
            var oUsuario = General.GetEntity<Usuario>(q => q.UsuarioID == iUsuarioID && q.Estatus);
            string sVendedores = oUsuario.NombrePersona;
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Egreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = oDev.FormaDePago;

            // Se llenan los datos del receptor
            var oVenta = General.GetEntity<Venta>(q => q.VentaID == oDev.VentaID && q.Estatus);
            int iClienteID = oVenta.ClienteID;
            var ResRec = VentasProc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            var oDevDetalle = General.GetListOf<VentasDevolucionesDetalleView>(q => q.VentaDevolucionID == oDev.VentaDevolucionID);
            oFacturaE.Conceptos = new List<Concepto>();
            foreach (var oDet in oDevDetalle)
            {
                oFacturaE.Conceptos.Add(new Concepto()
                {
                    Identificador = oDet.NumeroParte,
                    Cantidad = oDet.Cantidad,
                    Unidad = oDet.NombreMedida,
                    Descripcion = oDet.NombreParte,
                    ValorUnitario = oDet.PrecioUnitario,
                    Iva = oDet.Iva
                });
            }

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasProc.FeEnviarFactura(ref oFacturaE);
            bool bFacturada = ResXml.Exito;
            // if (ResXml.Error)
            //    return new ResAcc<int>(false, ResXml.Mensaje);
            
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oVentaFactura = General.GetEntity<VentasFacturasDetalleView>(q => q.VentaID == oVenta.VentaID);
            var oFolioFactura = VentasProc.GenerarFolioDeFacturaDevolucion();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oFacturaDevolucion = new VentaFacturaDevolucion()
            {
                VentaFacturaID = oVentaFactura.VentaFacturaID,
                Fecha = dAhora,
                FolioFiscal = (oFacturaE.Timbre == null ? "" : oFacturaE.Timbre.FolioFiscal),
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                EsCancelacion = false,
                Procesada = bFacturada,
                FechaProcesada = (bFacturada ? ((DateTime?)dAhora) : null)
            };
            var oFacturaDevDet = new List<VentaFacturaDevolucionDetalle>();
            oFacturaDevDet.Add(new VentaFacturaDevolucionDetalle() { VentaDevolucionID = iDevolucionID });
            Guardar.FacturaDevolucion(oFacturaDevolucion, oFacturaDevDet);

            //
            oFacturaE.Adicionales.Add("FacturaOrigen", (oVentaFactura.Serie + oVentaFactura.Folio));

            // Se manda guardar la factura, en pdf y xml
            if (bFacturada)
                VentasProc.FeGuardarArchivosFacturaDevolucion(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda la nota de crédito generada, por correo
            VentasProc.EnviarFacturaDevolucionPorCorreo(oFacturaDevolucion.VentaFacturaDevolucionID);

            // Se manda imprimir la factura
            // ..

            return new ResAcc<int>(bFacturada, oFacturaDevolucion.VentaFacturaDevolucionID);
        }
        */

        public static ResAcc<int> GenerarFacturaDevolucion(string sFormaDePago, int iVentaID, List<ProductoVenta> oDetalle, int iUsuarioID, bool bEsDevolucion, int iId)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtiene el nombre del vendedor
            var oUsuario = General.GetEntity<Usuario>(q => q.UsuarioID == iUsuarioID && q.Estatus);
            string sVendedores = oUsuario.NombrePersona;
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Egreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = sFormaDePago;

            // Se llenan los datos del receptor
            var oVenta = General.GetEntity<Venta>(q => q.VentaID == iVentaID && q.Estatus);
            int iClienteID = oVenta.ClienteID;
            var ResRec = VentasProc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            decimal mUnitarioTotal = 0, mIvaTotal = 0;
            oFacturaE.Conceptos = new List<Concepto>();
            foreach (var oDet in oDetalle)
            {
                oFacturaE.Conceptos.Add(new Concepto()
                {
                    Identificador = oDet.NumeroDeParte,
                    Cantidad = oDet.Cantidad,
                    Unidad = oDet.UnidadDeMedida,
                    Descripcion = oDet.NombreDeParte,
                    ValorUnitario = oDet.PrecioUnitario,
                    Iva = oDet.Iva
                });

                mUnitarioTotal += oDet.PrecioUnitario;
                mIvaTotal += oDet.Iva;
            }

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasProc.FeEnviarFactura(ref oFacturaE);
            bool bFacturada = ResXml.Exito;
            /* if (ResXml.Error)
                return new ResAcc<int>(false, ResXml.Mensaje);
            */
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oVentaFactura = General.GetEntity<VentasFacturasDetalleAvanzadoView>(q => q.VentaID == iVentaID);
            var oFolioFactura = VentasProc.GenerarFolioDeFacturaDevolucion();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oFacturaDevolucion = new VentaFacturaDevolucion()
            {
                VentaFacturaID = oVentaFactura.VentaFacturaID.Valor(),
                Fecha = dAhora,
                FolioFiscal = (oFacturaE.Timbre == null ? "" : oFacturaE.Timbre.FolioFiscal),
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                EsCancelacion = false,
                Procesada = bFacturada,
                FechaProcesada = (bFacturada ? ((DateTime?)dAhora) : null),
                Subtotal = mUnitarioTotal,
                Iva = mIvaTotal
            };
            var oFacturaDevDet = new List<VentaFacturaDevolucionDetalle>();
            var oRegFacDevDet = new VentaFacturaDevolucionDetalle();
            if (bEsDevolucion)
                oRegFacDevDet.VentaDevolucionID = iId;
            else
                oRegFacDevDet.VentaGarantiaID = iId;
            oFacturaDevDet.Add(oRegFacDevDet);
            Guardar.FacturaDevolucion(oFacturaDevolucion, oFacturaDevDet);

            //
            oFacturaE.Adicionales.Add("FacturaOrigen", (oVentaFactura.Serie + oVentaFactura.Folio));

            // Se manda guardar la factura, en pdf y xml
            if (bFacturada)
                VentasProc.FeGuardarArchivosFacturaDevolucion(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda la nota de crédito generada, por correo
            VentasProc.EnviarFacturaDevolucionPorCorreo(oFacturaDevolucion.VentaFacturaDevolucionID);

            // Se manda imprimir la factura
            // ..

            return new ResAcc<int>(bFacturada, oFacturaDevolucion.VentaFacturaDevolucionID);
        }

        public static ResAcc<int> GenerarFacturaDevolucionPorDevolucion(int iDevolucionID)
        {
            var oDev = General.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iDevolucionID);
            var oDevDet = General.GetListOf<VentasDevolucionesDetalleView>(c => c.VentaDevolucionID == iDevolucionID);
            var oPartesDev = new List<ProductoVenta>();
            foreach (var oReg in oDevDet)
            {
                oPartesDev.Add(new ProductoVenta()
                {
                    NumeroDeParte = oReg.NumeroParte,
                    NombreDeParte = oReg.NombreParte,
                    UnidadDeMedida = oReg.NombreMedida,
                    Cantidad = oReg.Cantidad,
                    PrecioUnitario = oReg.PrecioUnitario,
                    Iva = oReg.Iva
                });
            }

            return VentasProc.GenerarFacturaDevolucion(oDev.FormaDePago, oDev.VentaID, oPartesDev, oDev.RealizoUsuarioID, true, oDev.VentaDevolucionID);
        }

        public static ResAcc<int> GenerarFacturaDevolucionPorGarantia(int iGarantiaID)
        {
            var oGarantia = General.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == iGarantiaID);
            var oPartesDev = new List<ProductoVenta>();
            oPartesDev.Add(new ProductoVenta()
            {
                NumeroDeParte = oGarantia.NumeroDeParte,
                NombreDeParte = oGarantia.NombreDeParte,
                UnidadDeMedida = oGarantia.Medida,
                Cantidad = 1,
                PrecioUnitario = oGarantia.PrecioUnitario,
                Iva = oGarantia.Iva
            });

            return VentasProc.GenerarFacturaDevolucion(oGarantia.Accion, oGarantia.VentaID, oPartesDev, oGarantia.RealizoUsuarioID, false, oGarantia.VentaGarantiaID);
        }

        public static ResAcc<int> GenerarNotaDeCreditoFiscal(List<ProductoVenta> oDetalle, int iClienteID, int iUsuarioID)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtiene el nombre del vendedor
            var oUsuario = General.GetEntity<Usuario>(q => q.UsuarioID == iUsuarioID && q.Estatus);
            string sVendedores = oUsuario.NombrePersona;
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Egreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = "NOTA DE CRÉDITO FISCAL";

            // Se llenan los datos del receptor
            var ResRec = VentasProc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se agregan datos adicionales
            oFacturaE.Adicionales.Add("FacturaOrigen", "");

            // Se llenan los conceptos de la factura
            decimal mSubtotal = 0, mIva = 0;
            oFacturaE.Conceptos = new List<Concepto>();
            foreach (var oDet in oDetalle)
            {
                oFacturaE.Conceptos.Add(new Concepto()
                {
                    Identificador = oDet.NumeroDeParte,
                    Cantidad = oDet.Cantidad,
                    Unidad = oDet.UnidadDeMedida,
                    Descripcion = oDet.NombreDeParte,
                    ValorUnitario = oDet.PrecioUnitario,
                    Iva = oDet.Iva
                });
                mSubtotal += oDet.PrecioUnitario;
                mIva += oDet.Iva;
            }

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasProc.FeEnviarFactura(ref oFacturaE);
            bool bFacturada = ResXml.Exito;
            /* if (ResXml.Error)
                return new ResAcc<int>(false, ResXml.Mensaje);
            */
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oFolioFactura = VentasProc.GenerarFolioDeFacturaDevolucion();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oNota = new NotaDeCreditoFiscal()
            {
                Fecha = dAhora,
                ClienteID = iClienteID,
                SucursalID = GlobalClass.SucursalID,
                FolioFiscal = (oFacturaE.Timbre == null ? "" : oFacturaE.Timbre.FolioFiscal),
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                Subtotal = mSubtotal,
                Iva = mIva,
                RealizoUsuarioID = iUsuarioID
            };
            Guardar.Generico<NotaDeCreditoFiscal>(oNota);
            // Se guarda el detalle de la Nota de Crédito
            foreach (var oReg in oDetalle)
            {
                int iVentaID = Helper.ConvertirEntero(oReg.NumeroDeParte);
                if (iVentaID <= 0) continue;
                Guardar.Generico<NotaDeCreditoFiscalDetalle>(new NotaDeCreditoFiscalDetalle()
                {
                    NotaDeCreditoFiscalID = oNota.NotaDeCreditoFiscalID,
                    VentaID = iVentaID,
                    Descuento = oReg.PrecioUnitario,
                    IvaDescuento = oReg.Iva
                });
            }

            // Se manda guardar la factura, en pdf y xml
            if (bFacturada)
                VentasProc.FeGuardarArchivosFacturaDevolucion(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda la nota de crédito generada, por correo
            // VentasProc.EnviarFacturaDevolucionPorCorreo(oFacturaDevolucion.VentaFacturaDevolucionID);

            // Se manda imprimir la factura
            // ..

            var oRes = new ResAcc<int>(bFacturada, oNota.NotaDeCreditoFiscalID);
            if (!bFacturada)
                oRes.Mensaje = ResXml.Mensaje;
            return oRes;
        }

        #endregion

        #region [ Procesos independientes ]

        public static void ActualizarBarraDeMetas(object state)
        {
            try
            {
                // Se obtiene la utilidad meta
                var oMetaSucursal = General.GetEntity<MetaSucursal>(c => c.SucursalID == GlobalClass.SucursalID);
                // Se obtiene la utilidad acumulada
                var oParams = new Dictionary<string, object>();
                var oSemana = UtilDatos.FechasDeComisiones(DateTime.Now);
                oParams.Add("Desde", oSemana.Valor1);
                oParams.Add("Hasta", oSemana.Valor2);
                var oDatos = General.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado", oParams);
                decimal mUtilidad = oDatos.Where(c => c.SucursalID == GlobalClass.SucursalID).Sum(c => c.Utilidad).Valor();
                // Se llena la barra
                Ventas.Instance.Invoke(new Action(() =>
                {
                    if (mUtilidad < 0)
                    {
                        UtilLocal.MensajeAdvertencia("La Utilidad es menor que cero.");
                        return;
                    }
                    var oBarra = (Ventas.Instance.Controls["pnlBarraVentas"].Controls["pgbMetas"] as ProgressBar);
                    oBarra.Maximum = (int)oMetaSucursal.UtilSucursal;
                    oBarra.Value = (int)(mUtilidad > oMetaSucursal.UtilSucursal ? oMetaSucursal.UtilSucursal : mUtilidad);
                }));

                // Se programa la siguiente ejecución
                // Se obtiene el tiempo necesario para la llamada
                int iSegundos = Helper.ConvertirEntero(Config.Valor("Ventas.BarraDeMetas.SegundosActualizacion"));
                if (Program.oTimers.ContainsKey("BarraDeMetas"))
                {
                    Program.oTimers["BarraDeMetas"].Change((iSegundos * 1000), System.Threading.Timeout.Infinite);
                }
                else
                {
                    Program.oTimers.Add("BarraDeMetas", new System.Threading.Timer(new System.Threading.TimerCallback(VentasProc.ActualizarBarraDeMetas), null
                        , (iSegundos * 1000), System.Threading.Timeout.Infinite));
                }
            }
            catch (Exception e)
            {
                UtilLocal.MensajeError("Error no controlado al ejecutar actualización de Barra de Metas.\n\n" + e.Message);
            }
        }

        #endregion

    }
}
