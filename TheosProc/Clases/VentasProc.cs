using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data.Objects;
using System.Diagnostics;

using LibUtil;

namespace TheosProc
{
    public static class VentasProc
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
                    Datos.Guardar<NotaDeCredito>(oNota);
                    // Se debe imprimir algo aquí ?

                    // Si ya se emparejó la cantidad, se sale
                    if (mImporte == 0)
                        return new ResAcc<int>(true, "");
                }
                else
                // 
                {
                    oNota.Importe += (mImporte * -1);
                    Datos.Guardar<NotaDeCredito>(oNota);
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
            Datos.Guardar<NotaDeCredito>(oNotaNueva);

            // Se imprime el ticket correspondiente


            return new ResAcc<int>(true, oNotaNueva.NotaDeCreditoID);
        }
        */

        #region [ Vales ]

        public static ResAcc<int> GenerarNotaDeCredito(int iClienteID, decimal mImporte, string sObservacion, int iOrigenID, int iRelacionID)
        {
            DateTime dAhora = DateTime.Now;
            
            // Se revisa si existen otras notas de crédito que se puedan usar para afectar ésta
            bool bNegativa = (mImporte < 0);
            var oNotas = Datos.GetListOf<NotaDeCredito>(q => q.ClienteID == iClienteID && q.Valida && q.Estatus).OrderByDescending(q => q.FechaDeEmision);
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
                    Datos.Guardar<NotaDeCredito>(oNota);
                    // Se debe imprimir algo aquí ?

                    // Si ya se emparejó la cantidad, se sale
                    if (mImporte == 0)
                        return new ResAcc<int>(true, "");
                }
                else
                // 
                {
                    oNota.Importe += (mImporte * -1);
                    Datos.Guardar<NotaDeCredito>(oNota);
                    // Se debe imprimir algo aquí ?

                    // Se sale, pues ya no hay importe para afectar
                    return new ResAcc<int>(true, "");
                }
            }

            // Se crea la nota
            var oNotaNueva = new NotaDeCredito()
            {
                FechaDeEmision = DateTime.Now,
                SucursalID = Theos.SucursalID,
                Importe = mImporte,
                ClienteID = iClienteID,
                Valida = true,
                Observacion = sObservacion,
                OrigenID = iOrigenID,
                RelacionID = iRelacionID
            };
            Datos.Guardar<NotaDeCredito>(oNotaNueva);

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
            var oVale = Datos.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iNotaDeCreditoID && c.Estatus);
            oVale.Valida = false;
            oVale.MotivoBaja = sMotivoDeBaja;
            Datos.Guardar<NotaDeCredito>(oVale);

            return new ResAcc<bool>(true);
        }

        public static List<NotaDeCredito> ObtenerValesCreados(List<VentaPagoDetalle> oPagoDetalle)
        {
            var oVales = new List<NotaDeCredito>();
            
            foreach (var oReg in oPagoDetalle)
            {
                if (oReg.TipoFormaPagoID == Cat.FormasDePago.Vale)
                {
                    var oNota = Datos.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == oReg.NotaDeCreditoID && c.Estatus);
                    int iOrigenValeID = (oNota.OrigenID == Cat.OrigenesNotaDeCredito.ImporteRestante ? oNota.RelacionID.Valor() : oNota.NotaDeCreditoID);
                    var oValeN = Datos.GetEntity<NotaDeCredito>(c => c.OrigenID == Cat.OrigenesNotaDeCredito.ImporteRestante
                        && c.RelacionID == iOrigenValeID && c.Valida && c.FechaDeEmision >= oReg.FechaRegistro && c.Estatus);
                    if (oValeN != null)
                        oVales.Add(oValeN);
                }
            }
            return oVales;
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
            Config.EstablecerValor("Ventas.Folio", (Util.Entero(sFolio) + 1).ToString());
            return sFolio;
        }

        public static string GenerarFolioDeCobranza()
        {
            string sFolio = Config.Valor("Cobranza.Folio");
            Config.EstablecerValor("Cobranza.Folio", (Util.Entero(sFolio) + 1).ToString());
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
                SucursalID = Theos.SucursalID
            };
            if (iAutorizoUsuarioID.HasValue && iAutorizoUsuarioID > 0)
            {
                oAutorizacion.Autorizado = true;
                oAutorizacion.AutorizoUsuarioID = iAutorizoUsuarioID;
                oAutorizacion.FechaAutorizo = DateTime.Now;
            };
            Datos.Guardar<Autorizacion>(oAutorizacion);

            return new ResAcc<int>(true);
        }

        public static void EliminarVenta(int iVentaID)
        {
            var oVenta = Datos.GetEntity<Venta>(q => q.VentaID == iVentaID && q.Estatus);

            // De momento sólo están contempladas las ventas que sólo han sido registradas. No cobradas, pagadas, canceladas
            if (oVenta.VentaEstatusID != Cat.VentasEstatus.Realizada) return;

            var oVentaDet = Datos.GetListOf<VentaDetalle>(q => q.VentaID == oVenta.VentaID && q.Estatus);

            // Se regresa la existencia y se borra el detalle de la venta
            foreach (var oParteDet in oVentaDet)
            {
                AdmonProc.AgregarExistencia(oParteDet.ParteID, oVenta.SucursalID, oParteDet.Cantidad, Cat.Tablas.Venta, iVentaID);
                Datos.Eliminar<VentaDetalle>(oParteDet, true);
            }
            // Se borra la venta en sí
            Datos.Eliminar<Venta>(oVenta, true);

            // Se borran los datos del kardex
            var oPartesKardex = Datos.GetListOf<ParteKardex>(c => c.OperacionID == Cat.OperacionesKardex.Venta && c.RelacionTabla == Cat.Tablas.Venta
                && c.RelacionID == iVentaID);
            foreach (var oReg in oVentaDet)
            {
                var oKardex = oPartesKardex.FirstOrDefault(c => c.ParteID == oReg.ParteID);
                if (oKardex == null) continue;
                Datos.Eliminar<ParteKardex>(oKardex);
                // Se verifica si hubo algún otro movimiento en kardex de la misma parte, para hacer el reajuste
                var oDespues = Datos.GetListOf<ParteKardex>(c => c.ParteKardexID > oKardex.ParteKardexID && c.ParteID == oKardex.ParteID);
                foreach (var oRegD in oDespues)
                {
                    oRegD.ExistenciaNueva += (oKardex.Cantidad * -1);
                    Datos.Guardar<ParteKardex>(oRegD);
                }
            }
        }

        #region [ 9500 ]

        public static void Cancelar9500(int i9500ID, string sMotivo, int iUsuarioID)
        {
            var o9500 = Datos.GetEntity<Cotizacion9500>(q => q.Cotizacion9500ID == i9500ID && q.Estatus);
            var o9500Detalle = Datos.GetListOf<Cotizacion9500Detalle>(q => q.Cotizacion9500ID == i9500ID && q.Estatus);

            // Se verifica si se cobró el anticipo, para cancelarlo
            if (o9500.Anticipo > 0)
            {
                var oAnticipo = Datos.GetEntity<Venta>(c => c.VentaID == o9500.AnticipoVentaID && c.Estatus);
                if (oAnticipo.VentaEstatusID == Cat.VentasEstatus.Cobrada || oAnticipo.VentaEstatusID == Cat.VentasEstatus.Completada)
                {
                    // Si se pagó, se genera una devolución
                    var oVentaV = Datos.GetEntity<VentasView>(c => c.VentaID == o9500.AnticipoVentaID);
                    if (oVentaV.Pagado > 0)
                        VentasProc.GenerarDevolucionDeEfectivo(oVentaV.VentaID, oVentaV.Pagado);

                    // Se cancela la venta del anticipo
                    oAnticipo.VentaEstatusID = (oVentaV.Pagado > 0 ? Cat.VentasEstatus.Cancelada : Cat.VentasEstatus.CanceladaSinPago);
                    Datos.Guardar<Venta>(oAnticipo);
                }
            }
            
            // Se borran las partes del 9500, si no han sido usadas
            foreach (var oParte in o9500Detalle)
            {
                // Se valida que la parte no haya sido usada en ventas
                if (Datos.Exists<VentaDetalle>(q => q.ParteID == oParte.ParteID))
                    continue;
                // Se valida que la parte no haya sido usada en almacén
                if (Datos.Exists<MovimientoInventarioDetalle>(q => q.ParteID == oParte.ParteID))
                    continue;
                // Se borra -> Deshabilitado hasta tener las validaciones del Almacén
                Guardar.EliminarParte(oParte.ParteID);
            }

            // Se verifica si hay autorizaciones, para borrarlas
            var oAuts = Datos.GetListOf<Autorizacion>(c => (c.AutorizacionProcesoID == Cat.AutorizacionesProcesos.c9500PrecioFueraDeRango
                || c.AutorizacionProcesoID == Cat.AutorizacionesProcesos.c9500SinAnticipo) && c.Tabla == Cat.Tablas.Tabla9500 && c.TablaRegistroID == i9500ID && c.Estatus);
            foreach (var oReg in oAuts)
            {
                oReg.Estatus = true;
                Datos.Guardar<Autorizacion>(oReg);
            }

            // Se cancela el 9500
            o9500.EstatusGenericoID = Cat.EstatusGenericos.CanceladoAntesDeVender;
            o9500.BajaUsuarioID = iUsuarioID;
            o9500.BajaMotivo = sMotivo;
            Datos.Guardar<Cotizacion9500>(o9500);
        }

        public static void Completar9500(Cotizacion9500 o9500, decimal mSobrante, bool bDevolverEfectivo)
        {
            // Se cancela la venta del anticipo
            var oVentaAnt = Datos.GetEntity<Venta>(q => q.VentaID == o9500.AnticipoVentaID && q.VentaEstatusID != Cat.VentasEstatus.Cancelada && 
                q.VentaEstatusID != Cat.VentasEstatus.CanceladaSinPago && q.Estatus);
            if (oVentaAnt != null)
            {
                oVentaAnt.VentaEstatusID = Cat.VentasEstatus.Cancelada;
                Datos.Guardar<Venta>(oVentaAnt);
                // Se genera una devolución de efectivo (si se realizó un pago) de la venta cancelada, pues se generará una nueva venta con el importe total
                if (Datos.Exists<VentaPago>(q => q.VentaID == o9500.AnticipoVentaID && q.Estatus))
                    VentasProc.GenerarDevolucionDeEfectivo(o9500.AnticipoVentaID.Valor(), o9500.Anticipo - (mSobrante > 0 ? mSobrante : 0));
            }

            // Si hubo un sobrante, se genera nota de crédito o se devuelve efectivo
            if (mSobrante > 0)
            {
                if (bDevolverEfectivo)
                    VentasProc.GenerarDevolucionDeEfectivo(oVentaAnt.VentaID, mSobrante);
                else
                    VentasProc.GenerarNotaDeCredito(o9500.ClienteID, mSobrante, "", Cat.OrigenesNotaDeCredito.Anticipo9500, oVentaAnt.VentaID);
            }

            // Se modifica el Estatus del 9500
            o9500.EstatusGenericoID = Cat.EstatusGenericos.Completada;
            Datos.Guardar<Cotizacion9500>(o9500);
        }

        public static void Regresar9500DeCompletar(Cotizacion9500 o9500)
        {
            // Se restaura el dato de la venta y el estatus del 9500
            o9500.VentaID = null;
            o9500.EstatusGenericoID = Cat.EstatusGenericos.Pendiente;
            Datos.Guardar<Cotizacion9500>(o9500);
        }

        #endregion

        public static void EliminarPagosVenta(int iVentaID)
        {
            var oPagos = Datos.GetListOf<VentaPago>(c => c.VentaID == iVentaID && c.Estatus);
            foreach (var oPago in oPagos)
            {
                var oPagoDet = Datos.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == oPago.VentaPagoID && c.Estatus);
                foreach (var oPagoForma in oPagoDet)
                    Datos.Eliminar<VentaPagoDetalle>(oPagoForma, true);
                Datos.Eliminar<VentaPago>(oPago, true);
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
                RealizoUsuarioID = Theos.UsuarioID
            };
            Datos.Guardar<CajaEgreso>(oCajaEgreso);
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
                RealizoUsuarioID = Theos.UsuarioID
            };
            Datos.Guardar<CajaIngreso>(oCajaIngreso);
            return oCajaIngreso;
        }

        public static bool EsFacturaMultiple(int iVentaID)
        {
            var oVentaFaDe = Datos.GetEntity<VentaFacturaDetalle>(c => c.VentaID == iVentaID && c.Estatus);
            if (oVentaFaDe == null)
                return false;
            else
                return Datos.Exists<VentasFacturasView>(c => c.VentaFacturaID == oVentaFaDe.VentaFacturaID && c.Ventas > 1);
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
                oFormaDePago = Datos.GetEntity<TipoFormaPago>(q => q.TipoFormaPagoID == oForma.TipoFormaPagoID && q.Estatus);
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
            var oVentasP = Datos.GetListOf<VentasPagosDetalleView>(q => q.VentaID == iVentaID);
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

    }
}
