using System;
using System.Collections.Generic;
using System.Linq;

using LibUtil;

namespace TheosProc
{
    static class ContaProc
    {
        public static ResAcc<int> CrearCuentaAuxiliar(string sCuenta, int iCuentaDeMayorID, int? iRelacionID)
        {
            var oCuentaAux = new ContaCuentaAuxiliar()
            {
                CuentaAuxiliar = sCuenta,
                ContaCuentaDeMayorID = iCuentaDeMayorID,
                RelacionID = iRelacionID
            };
            Datos.Guardar<ContaCuentaAuxiliar>(oCuentaAux);

            return new ResAcc<int>(true, oCuentaAux.ContaCuentaAuxiliarID);
        }

        #region [ Pólizas ]

        public static void BorrarPoliza(int iPolizaID)
        {
            var oPoliza = Datos.GetEntity<ContaPoliza>(c => c.ContaPolizaID == iPolizaID);
            var oPolizaDet = Datos.GetListOf<ContaPolizaDetalle>(c => c.ContaPolizaID == iPolizaID);
            foreach (var oReg in oPolizaDet)
                Datos.Eliminar<ContaPolizaDetalle>(oReg);
            Datos.Eliminar<ContaPoliza>(oPoliza);
        }

        public static ContaPoliza CrearPoliza(int iTipoPolizaID, string sConcepto, int iCuentaCargo, int iCuentaAbono, decimal mImporte, string sReferencia
            , string sTabla, int? iRelacionID, int iSucursalID)
        {
            var oPoliza = new ContaPoliza()
            {
                Fecha = DateTime.Now,
                ContaTipoPolizaID = iTipoPolizaID,
                Concepto = sConcepto,
                RealizoUsuarioID = Theos.UsuarioID,
                SucursalID = iSucursalID,
                RelacionID = iRelacionID
            };
            Datos.Guardar<ContaPoliza>(oPoliza);

            var oDetCargo = new ContaPolizaDetalle()
            {
                ContaPolizaID = oPoliza.ContaPolizaID,
                ContaCuentaAuxiliarID = iCuentaCargo,
                SucursalID = oPoliza.SucursalID,
                Cargo = mImporte,
                Abono = 0,
                Referencia = sReferencia
            };
            var oDetAbono = new ContaPolizaDetalle()
            {
                ContaPolizaID = oPoliza.ContaPolizaID,
                ContaCuentaAuxiliarID = iCuentaAbono,
                SucursalID = oPoliza.SucursalID,
                Cargo = 0,
                Abono = mImporte,
                Referencia = sReferencia
            };
			if (iCuentaCargo > 0)
	            Datos.Guardar<ContaPolizaDetalle>(oDetCargo);
			if (iCuentaAbono > 0)            
				Datos.Guardar<ContaPolizaDetalle>(oDetAbono);
			

            return oPoliza;
        }

        public static ContaPoliza CrearPoliza(int iTipoPolizaID, string sConcepto, int iCuentaCargo, int iCuentaAbono, decimal mImporte, string sReferencia
            , string sTabla, int? iRelacionID)
        {
            return ContaProc.CrearPoliza(iTipoPolizaID, sConcepto, iCuentaCargo, iCuentaAbono, mImporte, sReferencia, sTabla, iRelacionID, Theos.SucursalID);
        }

        public static ContaPoliza CrearPolizaAfectacion(int iAfectacionID, int iId, string sReferencia, string sConcepto, int iSucursalID)
        {
            // Se genera el detalle de la poliza
            ContaPolizaDetalle oRegPoliza;
            var oPolizaDet = new List<ContaPolizaDetalle>();

            decimal mCargo = 0, mAbono = 0;
            var oAfectacion = Datos.GetEntity<ContaConfigAfectacion>(c => c.ContaConfigAfectacionID == iAfectacionID);
            var oAfectacionDet = Datos.GetListOf<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionID == iAfectacionID);
            foreach (var oReg in oAfectacionDet)
            {
                oRegPoliza = null;

                if (oReg.EsCasoFijo)
                {
                    // Actualmente sólo hay un caso fijo y por tanto no se verifica más ni se hacen casos
                    oRegPoliza = ContaProc.GafClientes(iAfectacionID, iId);
                    switch (Theos.SucursalID)
                    {
                        case Cat.Sucursales.Matriz: oRegPoliza.ContaCuentaAuxiliarID = Cat.ContaCuentasAuxiliares.ClientesHectorRicardo; break;
                        case Cat.Sucursales.Sucursal2: oRegPoliza.ContaCuentaAuxiliarID = Cat.ContaCuentasAuxiliares.ClientesEdgarAron; break;
                        case Cat.Sucursales.Sucursal3: oRegPoliza.ContaCuentaAuxiliarID = Cat.ContaCuentasAuxiliares.ClientesJoseManuel; break;
                    }
                }
                else if (oReg.EsCuentaDeMayor)
                {
                    switch (oReg.CuentaID)
                    {
                        case Cat.ContaCuentasDeMayor.Clientes:
                            oRegPoliza = ContaProc.GafClientes(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerClienteCuentaAuxiliarID(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasDeMayor.Proveedores:
                            oRegPoliza = ContaProc.GafProveedores(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerProveedorCuentaAuxiliarID(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasDeMayor.Bancos:
                            oRegPoliza = ContaProc.GafBancos(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerBancoCuentaAuxiliarID(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasDeMayor.Agua:  // Se usa como si fuera *Gastos
                            oRegPoliza = ContaProc.GafGastos(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerGastoCuentaAuxiliarID(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasDeMayor.InteresesBancarios:
                            oRegPoliza = ContaProc.GafInteresesBancarios(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerGastoCuentaAuxiliarID(iAfectacionID, iId);  // Se parte de un ContaEgresoID
                            break;
                        case Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo:
                            oRegPoliza = ContaProc.GafCuentasPorPagarCortoPlazo(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerCpcpCuentaAuxiliarID(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasDeMayor.ReparteDeUtilidades:
                            oRegPoliza = ContaProc.GafRepartoDeUtilidades(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerGastoCuentaAuxiliarID(iAfectacionID, iId);
                            break;

                        case Cat.ContaCuentasDeMayor.Salarios:
                            oRegPoliza = ContaProc.GafSalarios(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Salarios);
                            break;
                        case Cat.ContaCuentasDeMayor.TiempoExtra:
                            oRegPoliza = ContaProc.GafTiempoExtra(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.TiempoExtra);
                            break;
                        case Cat.ContaCuentasDeMayor.PremioDeAsistencia:
                            oRegPoliza = ContaProc.GafPremioDeAsistencia(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.PremioDeAsistencia);
                            break;
                        case Cat.ContaCuentasDeMayor.PremioDePuntualidad:
                            oRegPoliza = ContaProc.GafPremioDePuntualidad(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.PremioDePuntualidad);
                            break;
                        case Cat.ContaCuentasDeMayor.Vacaciones:
                            oRegPoliza = ContaProc.GafVacaciones(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Vacaciones);
                            break;
                        case Cat.ContaCuentasDeMayor.PrimaVacacional:
                            oRegPoliza = ContaProc.GafPrimaVacacional(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.PrimaVacacional);
                            break;
                        case Cat.ContaCuentasDeMayor.Aguinaldo:
                            oRegPoliza = ContaProc.GafAguinaldo(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Aguinaldo);
                            break;
                        case Cat.ContaCuentasDeMayor.Ptu:
                            oRegPoliza = ContaProc.GafPtu(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Ptu);
                            break;
                        case Cat.ContaCuentasDeMayor.Imss:
                            oRegPoliza = ContaProc.GafImss(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Imss);
                            break;
                        case Cat.ContaCuentasDeMayor.Ispt:
                            oRegPoliza = ContaProc.GafIspt(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Ispt);
                            break;
                        case Cat.ContaCuentasDeMayor.Infonavit:
                            oRegPoliza = ContaProc.GafInfonavit(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Infonavit);
                            break;
                        case Cat.ContaCuentasDeMayor.RetencionImss:
                            oRegPoliza = ContaProc.GafRetencionImss(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.RetencionImss);
                            break;
                        case Cat.ContaCuentasDeMayor.SubsidioAlEmpleo:
                            oRegPoliza = ContaProc.GafSubsidioAlEmpleo(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.SubsidioAlEmpleo);
                            break;
                        case Cat.ContaCuentasDeMayor.RetencionInfonavit:
                            oRegPoliza = ContaProc.GafRetencionInfonavit(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaOficialCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.RetencionInfonavit);
                            break;

                        case Cat.ContaCuentasDeMayor.Nomina2Por:
                            oRegPoliza = ContaProc.GafNomina2Por(iAfectacionID, iId);
                            oRegPoliza.ContaCuentaAuxiliarID = ContaProc.ObtenerNominaImpuestoUsuarioCuentaAuxiliarID(iAfectacionID, iId, Cat.ContaCuentasDeMayor.Nomina2Por);
                            break;
                    }
                }
                else
                {
                    switch (oReg.CuentaID)
                    {
                        case Cat.ContaCuentasAuxiliares.Inventario:
                            oRegPoliza = ContaProc.GafInventario(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.InventarioGarantias:
                            oRegPoliza = ContaProc.GafInventarioGarantias(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.CostoVenta:
                            oRegPoliza = ContaProc.GafCostoVenta(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.VentasContado:
                            oRegPoliza = ContaProc.GafVentasContado(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.VentasCredito:
                            oRegPoliza = ContaProc.GafVentasCredito(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.IvaTrasladadoCobrado:
                            oRegPoliza = ContaProc.GafIvaTrasladadoCobrado(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.IvaTrasladadoNoCobrado:
                            oRegPoliza = ContaProc.GafIvaTrasladadoNoCobrado(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.Caja:
                            oRegPoliza = ContaProc.GafCaja(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.IvaAcreditablePorPagar:
                            oRegPoliza = ContaProc.GafIvaAcreditablePorPagar(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.IvaAcreditablePagado:
                            oRegPoliza = ContaProc.GafIvaAcreditablePagado(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.CapitalFijo:
                            oRegPoliza = ContaProc.GafCapitalFijo(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.Resguardo:
                            oRegPoliza = ContaProc.GafResguardo(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.GastosNoDeducibles:
                            oRegPoliza = ContaProc.GafResguardo(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.AnticipoClientes:
                            oRegPoliza = ContaProc.GafAnticipoClientes(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.DescuentoSobreVentaClientes:
                            oRegPoliza = ContaProc.GafDescuentoSobreVentaClientes(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.DevolucionSobreVentaClientes:
                            oRegPoliza = ContaProc.GafDevolucionesSobreVentaClientes(iAfectacionID, iId);
                            break;
                        case Cat.ContaCuentasAuxiliares.ReservaNomina:
                            oRegPoliza = ContaProc.GafReservaNomina(iAfectacionID, iId);
                            break;
                    }

                    if (oRegPoliza != null)
                        oRegPoliza.ContaCuentaAuxiliarID = oReg.CuentaID;
                }

                if (oRegPoliza != null)
                {
                    // oRegPoliza.ContaCuentaAuxiliarID = oReg.ContaCuentaAuxiliarID;
                    oRegPoliza.Cargo = Math.Round(oRegPoliza.Cargo, 2);
                    if (!oReg.EsCargo)
                    {
                        oRegPoliza.Abono = oRegPoliza.Cargo;
                        oRegPoliza.Cargo = 0;
                    }
                    oRegPoliza.Referencia = sReferencia;
                    // oRegPoliza.RelacionID = iId;
                    oPolizaDet.Add(oRegPoliza);

                    // Para la validación
                    mCargo += oRegPoliza.Cargo;
                    mAbono += oRegPoliza.Abono;
                }
            }

            // Se validan que los movimientos den cero como resultado
            DateTime dAhora = DateTime.Now;
            bool bError = (mCargo != mAbono);
            /* if (bError)
            {
                // Se registra la diferencia y se sale, pues no coincide
                var oError = new ContaPolizaError()
                {
                    Fecha = dAhora,
                    ContaTipoPolizaID = oAfectacion.ContaTipoPolizaID,
                    Concepto = (sReferencia + " / " + sConcepto),
                    RealizoUsuarioID = Theos.UsuarioID,
                    SucursalID = iSucursalID,
                    Detalle = ""
                };
                foreach (var oReg in oPolizaDet)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oReg.ContaCuentaAuxiliarID);
                    oError.Detalle += string.Format("{0}\t{1}\t{2}\t{3}\n", oReg.ContaCuentaAuxiliarID, oCuentaAux.CuentaAuxiliar.RellenarCortarDerecha(32)
                        , oReg.Cargo.ToString(Util.FormatoMoneda), oReg.Abono.ToString(Util.FormatoMoneda));
                }
                Datos.Guardar<ContaPolizaError>(oError);
                // Se muestra un mensaje de error
                UtilLocal.MensajeAdvertencia(string.Format("Se encontró una diferencia al tratar de crear la Póliza Contable.\n\n\t\t\t\tCargo\tAbono\n{0}\t\t\t\t{1}\t{2}"
                    , oError.Detalle, mCargo.ToString(GlobalClass.FormatoMoneda), mAbono.ToString(GlobalClass.FormatoMoneda)));
                // Ya no se sale, se guarda la póliza y se marca como error
                // return;
            }
            */

            // Se genera la Póliza
            var oPoliza = new ContaPoliza()
            {
                Fecha = dAhora,
                ContaTipoPolizaID = oAfectacion.ContaTipoPolizaID,
                Concepto = (sReferencia + " / " + sConcepto),
                RealizoUsuarioID = Theos.UsuarioID,
                SucursalID = iSucursalID,
                RelacionTabla = ContaProc.ObtenerTablaDeAfectacion(iAfectacionID),
                RelacionID = iId,
                Origen = oAfectacion.Operacion,
                Error = bError
            };

            // Se guardan los datos
            Datos.Guardar<ContaPoliza>(oPoliza);
            foreach (var oReg in oPolizaDet)
            {
                oReg.ContaPolizaID = oPoliza.ContaPolizaID;
                Datos.Guardar<ContaPolizaDetalle>(oReg);
            }

            return oPoliza;
        }

        public static ContaPoliza CrearPolizaAfectacion(int iAfectacionID, int iId, string sReferencia, string sConcepto)
        {
            return ContaProc.CrearPolizaAfectacion(iAfectacionID, iId, sReferencia, sConcepto, Theos.SucursalID);
        }

        public static string ObtenerTablaDeAfectacion(int iAfectacionID)
        {
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                case Cat.ContaAfectaciones.VentaContadoVale:
                case Cat.ContaAfectaciones.VentaCredito:
                    return Cat.Tablas.Venta;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    return Cat.Tablas.VentaFactura;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                case Cat.ContaAfectaciones.DevolucionVentaValeTicket:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    return Cat.Tablas.VentaDevolucion;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    return Cat.Tablas.VentaPago;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    return Cat.Tablas.NotaDeCreditoFiscal;
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                case Cat.ContaAfectaciones.GarantiaVentaValeTicket:
                    return Cat.Tablas.VentaGarantia;
                case Cat.ContaAfectaciones.CompraCreditoFactura:
                case Cat.ContaAfectaciones.CompraCreditoNota:
                case Cat.ContaAfectaciones.EntradaInventario:
                case Cat.ContaAfectaciones.SalidaInventario:
                    return Cat.Tablas.MovimientoInventario;
                case Cat.ContaAfectaciones.PagoCompraCredito:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoGarantia:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoDevolucion:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoDirecto:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCaja:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoFactura:
                case Cat.ContaAfectaciones.GastoCajaFacturado:
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    return Cat.Tablas.ProveedorPolizaDetalle;
                case Cat.ContaAfectaciones.Resguardo:
                case Cat.ContaAfectaciones.GastoReparteUtilidades:
                    return Cat.Tablas.CajaEgreso;
                case Cat.ContaAfectaciones.Refuerzo:
                    return Cat.Tablas.CajaIngreso;
                case Cat.ContaAfectaciones.DepositoBancario:
                    return Cat.Tablas.BancoCuentaMovimiento;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.GastoNotaEfectivo:
                case Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp:
                case Cat.ContaAfectaciones.InteresesBancarios:
                    return Cat.Tablas.ContaEgreso;
                case Cat.ContaAfectaciones.ValeDirecto:
                    return Cat.Tablas.NotaDeCredito;
                case Cat.ContaAfectaciones.NominaOficial:
                    return Cat.Tablas.NominaUsuario;
            }

            return "";
        }

        public static ContaPolizaDetalle GafClientes(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                case Cat.ContaAfectaciones.VentaCredito:
                    ContaProc.AfectarConPrecioVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    ContaProc.AfectarConPago(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConFactura(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConCostoMasIvaParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                    ContaProc.AfectarConPrecioDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                    ContaProc.AfectarConPrecioGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    ContaProc.AfectarConImporteNotaDeCreditoFiscal(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                    ContaProc.AfectarConDevolucionMenosLoAbonado(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConGarantiaMenosLoAbonado(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafProveedores(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                // case Cat.ContaAfectaciones.CompraContado:
                case Cat.ContaAfectaciones.CompraCreditoFactura:
                    // case Cat.ContaAfectaciones.DevolucionCompraPago:
                    // case Cat.ContaAfectaciones.DevolucionCompraNotaDeCredito:
                    ContaProc.AfectarConPrecioInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.CompraCreditoNota:
                    ContaProc.AfectarConImporteFacturaCompra(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoCompraCredito:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoGarantia:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoDevolucion:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCaja:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoDirecto:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoFactura:
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    ContaProc.AfectarConAbonoProveedor(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafBancos(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.PagoCompraCredito:
                    ContaProc.AfectarConAbonoProveedor(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.InteresesBancarios:
                    ContaProc.AfectarConGastoContable(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DepositoBancario:
                    ContaProc.AfectarConMovimientoBancario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConNominaOficial(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.Pago2Por:
                case Cat.ContaAfectaciones.PagoImss:
                case Cat.ContaAfectaciones.PagoInfonavit:
                    ContaProc.AfectarConTotalPagoImpuestoUsuario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafGastos(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.GastoCajaFacturado:
                    ContaProc.AfectarConSubtotalCajaEgreso(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp:
                    ContaProc.AfectarConSubtotalGastoContable(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafSalarios(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.Salarios);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafTiempoExtra(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.TiempoExtra);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafPremioDeAsistencia(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.PremioDeAsistencia);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafPremioDePuntualidad(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.PremioDePuntualidad);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafVacaciones(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.Vacaciones);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafPrimaVacacional(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.PrimaVacacional);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafAguinaldo(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.Aguinaldo);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafPtu(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.Ptu);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafImss(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.Imss);
                    break;
                case Cat.ContaAfectaciones.PagoImss:
                    ContaProc.AfectarConGastoPagoImpuestoUsuario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafIspt(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.Ispt);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafInfonavit(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.Infonavit);
                    break;
                case Cat.ContaAfectaciones.PagoInfonavit:
                    ContaProc.AfectarConGastoPagoImpuestoUsuario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafRetencionImss(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.RetencionImss);
                    break;
                case Cat.ContaAfectaciones.PagoImss:
                    ContaProc.AfectarConRetenidoPagoImpuestoUsuario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafSubsidioAlEmpleo(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.SubsidioAlEmpleo);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafRetencionInfonavit(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    ContaProc.AfectarConCuentaNominaOficial(ref oDetalle, iId, Cat.ContaCuentasDeMayor.RetencionInfonavit);
                    break;
                case Cat.ContaAfectaciones.PagoInfonavit:
                    ContaProc.AfectarConRetenidoPagoImpuestoUsuario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafInventario(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                case Cat.ContaAfectaciones.VentaCredito:
                    ContaProc.AfectarConCostoVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConCostoFactura(ref oDetalle, iId);
                    break;
                // case Cat.ContaAfectaciones.CompraContado:
                case Cat.ContaAfectaciones.EntradaInventario:
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.CompraCreditoFactura:
                case Cat.ContaAfectaciones.DevolucionCompraPago:
                case Cat.ContaAfectaciones.DevolucionCompraNotaDeCredito:
                    ContaProc.AfectarConCostoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.CompraCreditoNota:
                    ContaProc.AfectarConImporteFacturaCompra(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    ContaProc.AfectarConCostoDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                case Cat.ContaAfectaciones.GarantiaVentaValeTicket:
                    ContaProc.AfectarConCostoGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoDevolucion:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoDirecto:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoFactura:
                    ContaProc.AfectarConSubtotalAbonoProveedor(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafInventarioGarantias(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                case Cat.ContaAfectaciones.GarantiaVentaValeTicket:
                    ContaProc.AfectarConCostoGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoGarantia:
                    ContaProc.AfectarConSubtotalAbonoProveedor(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafCostoVenta(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                case Cat.ContaAfectaciones.VentaCredito:
                    ContaProc.AfectarConCostoVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConCostoFactura(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    ContaProc.AfectarConCostoDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConCostoGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                    ContaProc.AfectarConCostoGarantia(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafVentasContado(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                    // case Cat.ContaAfectaciones.NotaDeCreditoDevolucionVenta:
                    ContaProc.AfectarConPrecioSinIvaVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConSubtotalFactura(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                    ContaProc.AfectarConPrecioSinIvaDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                    ContaProc.AfectarConPrecioSinIvaGarantia(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafVentasCredito(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaCredito:
                    ContaProc.AfectarConPrecioSinIvaVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    ContaProc.AfectarConImporteSivIvaNotaDeCreditoFiscal(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    ContaProc.AfectarConPrecioSinIvaDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConPrecioSinIvaGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafIvaTrasladadoCobrado(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                    ContaProc.AfectarConIvaVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    ContaProc.AfectarConIvaPago(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConIvaFactura(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConIvaCostoParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                    ContaProc.AfectarConIvaDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    // Se debe afectar con el iva del importe devuelto al cliente, lo cual depende de cuanto haya abonado el cliente, para cuando la venta es a crédito
                    ContaProc.AfectarConIvaImporteDevueltoDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConIvaImporteDevueltoGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                    ContaProc.AfectarConIvaGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.ValeDirecto:
                    ContaProc.AfectarConIvaVale(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafIvaTrasladadoNoCobrado(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaCredito:
                    ContaProc.AfectarConIvaVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    ContaProc.AfectarConIvaPago(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    ContaProc.AfectarConIvaNotaDeCreditoFiscal(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    // Se debe afectar con el iva de (el importe de la devolución menos el importe de lo abonado)
                    ContaProc.AfectarConIvaDevolucionMenosLoAbonado(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConIvaGarantiaMenosLoAbonado(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConIvaCostoParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafCaja(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                    ContaProc.AfectarConPrecioVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    ContaProc.AfectarConPagoNoVale(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoVale:
                    ContaProc.AfectarConPagoValeDeVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConFactura(ref oDetalle, iId);
                    break;
                // case Cat.ContaAfectaciones.CompraContado:
                case Cat.ContaAfectaciones.DevolucionCompraPago:
                    ContaProc.AfectarConPrecioInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                    ContaProc.AfectarConPrecioDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                    ContaProc.AfectarConPrecioGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.Resguardo:
                case Cat.ContaAfectaciones.GastoCajaFacturado:
                case Cat.ContaAfectaciones.GastoReparteUtilidades:
                    ContaProc.AfectarConCajaEgreso(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCaja:
                    ContaProc.AfectarConAbonoProveedor(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.Refuerzo:
                    ContaProc.AfectarConCajaIngreso(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeTicket:
                    ContaProc.AfectarConImporteDevueltoDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaValeTicket:
                    ContaProc.AfectarConImporteDevueltoGarantia(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafIvaAcreditablePorPagar(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.CompraCreditoFactura:
                case Cat.ContaAfectaciones.DevolucionCompraNotaDeCredito:
                    ContaProc.AfectarConIvaInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoCompraCredito:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoGarantia:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoDevolucion:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoDirecto:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoFactura:
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    ContaProc.AfectarConIvaAbonoProveedor(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafIvaAcreditablePagado(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.DevolucionCompraPago:
                    ContaProc.AfectarConIvaInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoCompraCredito:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado:
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    ContaProc.AfectarConIvaAbonoProveedor(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GastoCajaFacturado:
                    ContaProc.AfectarConIvaCajaEgreso(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.InteresesBancarios:
                case Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp:
                    ContaProc.AfectarConIvaGastoContable(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafCapitalFijo(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.EntradaInventario:
                    ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafResguardo(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.Resguardo:
                case Cat.ContaAfectaciones.GastoReparteUtilidades:
                    ContaProc.AfectarConCajaEgreso(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.Refuerzo:
                    ContaProc.AfectarConCajaIngreso(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.GastoNotaEfectivo:
                    ContaProc.AfectarConGastoContable(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DepositoBancario:
                    ContaProc.AfectarConMovimientoBancario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafGastosNoDeducibles(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.GastoNotaEfectivo:
                    ContaProc.AfectarConGastoContable(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafAnticipoClientes(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                    ContaProc.AfectarConPrecioDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoVale:
                    ContaProc.AfectarConPagoValeDeVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                case Cat.ContaAfectaciones.DevolucionVentaValeTicket:
                    ContaProc.AfectarConImporteDevueltoDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.ValeDirecto:
                    ContaProc.AfectarConVale(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    ContaProc.AfectarConPagoVale(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                    ContaProc.AfectarConPrecioGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                case Cat.ContaAfectaciones.GarantiaVentaValeTicket:
                    ContaProc.AfectarConImporteDevueltoGarantia(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static ContaPolizaDetalle GafDescuentoSobreVentaClientes(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.ValeDirecto:
                    ContaProc.AfectarConSubtotalVale(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    ContaProc.AfectarConImporteSivIvaNotaDeCreditoFiscal(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static ContaPolizaDetalle GafDevolucionesSobreVentaClientes(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    ContaProc.AfectarConPrecioSinIvaDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConPrecioSinIvaGarantia(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static ContaPolizaDetalle GafInteresesBancarios(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.InteresesBancarios:
                    ContaProc.AfectarConSubtotalGastoContable(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static ContaPolizaDetalle GafCuentasPorPagarCortoPlazo(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    ContaProc.AfectarConAbonoProveedor(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp:
                    ContaProc.AfectarConGastoContable(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static ContaPolizaDetalle GafReservaNomina(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.GastoReparteUtilidades:
                    ContaProc.AfectarConCajaEgreso(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static ContaPolizaDetalle GafRepartoDeUtilidades(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.GastoReparteUtilidades:
                    ContaProc.AfectarConCajaEgreso(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static ContaPolizaDetalle GafNomina2Por(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.Pago2Por:
                    ContaProc.AfectarConTotalPagoImpuestoUsuario(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        private static int ObtenerClienteCuentaAuxiliarID(int iAfectacionID, int iId)
        {
            int iClienteID = 0;
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoPago:
                case Cat.ContaAfectaciones.VentaCredito:
                    var oVenta = Datos.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
                    iClienteID = oVenta.ClienteID;
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    var oPagoV = Datos.GetEntity<VentasPagosView>(c => c.VentaPagoID == iId);
                    iClienteID = oPagoV.ClienteID.Valor();
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    iClienteID = Cat.Clientes.VentasMostrador;
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeTicket:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    var oDevV = Datos.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iId);
                    iClienteID = oDevV.ClienteID.Valor();
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    var oGarantiaV = Datos.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == iId);
                    iClienteID = oGarantiaV.ClienteID.Valor();
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    var oNotaCF = Datos.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == iId);
                    iClienteID = oNotaCF.ClienteID;
                    break;
            }

            var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Clientes && c.RelacionID == iClienteID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oCliente = Datos.GetEntity<Cliente>(c => c.ClienteID == iClienteID && c.Estatus);
                var oRes = ContaProc.CrearCuentaAuxiliar(oCliente.Nombre, Cat.ContaCuentasDeMayor.Clientes, iClienteID);
                iCuentaAuxID = oRes.Respuesta;
            }
            else
            {
                iCuentaAuxID = oCuentaAux.ContaCuentaAuxiliarID;
            }

            return iCuentaAuxID;
        }

        private static int ObtenerProveedorCuentaAuxiliarID(int iAfectacionID, int iId)
        {
            int iProveedorID = 0;
            switch (iAfectacionID)
            {
                // case Cat.ContaAfectaciones.CompraContado:
                case Cat.ContaAfectaciones.CompraCreditoFactura:
                case Cat.ContaAfectaciones.DevolucionCompraPago:
                case Cat.ContaAfectaciones.DevolucionCompraNotaDeCredito:
                case Cat.ContaAfectaciones.CompraCreditoNota:
                    var oMov = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
                    iProveedorID = oMov.ProveedorID.Valor();
                    break;
                case Cat.ContaAfectaciones.PagoCompraCredito:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoDirecto:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoDevolucion:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCaja:
                case Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado:
                case Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoGarantia:
                case Cat.ContaAfectaciones.PagoCompraCreditoDescuentoFactura:
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    var oAbonoProv = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId && c.Estatus);
                    var oProveedorPol = Datos.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == oAbonoProv.ProveedorPolizaID && c.Estatus);
                    iProveedorID = oProveedorPol.ProveedorID;
                    break;
            }

            var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Proveedores && c.RelacionID == iProveedorID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oProveedor = Datos.GetEntity<Proveedor>(c => c.ProveedorID == iProveedorID && c.Estatus);
                var oRes = ContaProc.CrearCuentaAuxiliar(oProveedor.NombreProveedor, Cat.ContaCuentasDeMayor.Proveedores, iProveedorID);
                iCuentaAuxID = oRes.Respuesta;
            }
            else
            {
                iCuentaAuxID = oCuentaAux.ContaCuentaAuxiliarID;
            }

            return iCuentaAuxID;
        }

        private static int ObtenerBancoCuentaAuxiliarID(int iAfectacionID, int iId)
        {
            int iBancoCuentaID = 0;
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.PagoCompraCredito:
                    var oMov = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId && c.Estatus);
                    var oPago = Datos.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == oMov.ProveedorPolizaID && c.Estatus);
                    iBancoCuentaID = oPago.BancoCuentaID;
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.GastoNotaEfectivo:
                case Cat.ContaAfectaciones.InteresesBancarios:
                    var oContaEgreso = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
                    iBancoCuentaID = oContaEgreso.BancoCuentaID.Valor();
                    break;
                case Cat.ContaAfectaciones.DepositoBancario:
                    var oMovBanco = Datos.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iId);
                    iBancoCuentaID = oMovBanco.BancoCuentaID.Valor();
                    break;
                case Cat.ContaAfectaciones.NominaOficial:
                    var oNominaUs = Datos.GetEntity<NominaUsuario>(c => c.NominaUsuarioID == iId);
                    var oNomina = Datos.GetEntity<Nomina>(c => c.NominaID == oNominaUs.NominaID);
                    iBancoCuentaID = oNomina.BancoCuentaID;
                    break;
                case Cat.ContaAfectaciones.Pago2Por:
                case Cat.ContaAfectaciones.PagoImss:
                case Cat.ContaAfectaciones.PagoInfonavit:
                    var oImpUsuario = Datos.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
                    var oImpuesto = Datos.GetEntity<NominaImpuesto>(c => c.NominaImpuestoID == oImpUsuario.NominaImpuestoID);
                    iBancoCuentaID = oImpuesto.BancoCuentaID;
                    break;
            }

            var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Bancos && c.RelacionID == iBancoCuentaID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oCuentaBanco = Datos.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iBancoCuentaID);
                var oRes = ContaProc.CrearCuentaAuxiliar(oCuentaBanco.NombreDeCuenta, Cat.ContaCuentasDeMayor.Bancos, iBancoCuentaID);
                iCuentaAuxID = oRes.Respuesta;
            }
            else
            {
                iCuentaAuxID = oCuentaAux.ContaCuentaAuxiliarID;
            }

            return iCuentaAuxID;
        }

        private static int ObtenerCpcpCuentaAuxiliarID(int iAfectacionID, int iId)
        {
            int iBancoCuentaID = 0;
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    var oMov = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId && c.Estatus);
                    var oPago = Datos.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == oMov.ProveedorPolizaID && c.Estatus);
                    iBancoCuentaID = oPago.BancoCuentaID;
                    break;
                case Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp:
                    var oContaEgreso = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
                    iBancoCuentaID = oContaEgreso.BancoCuentaID.Valor();
                    break;
            }

            var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo && c.RelacionID == iBancoCuentaID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oCuentaBanco = Datos.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iBancoCuentaID);
                var oRes = ContaProc.CrearCuentaAuxiliar(oCuentaBanco.NombreDeCuenta, Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo, iBancoCuentaID);
                iCuentaAuxID = oRes.Respuesta;
            }
            else
            {
                iCuentaAuxID = oCuentaAux.ContaCuentaAuxiliarID;
            }

            return iCuentaAuxID;
        }

        private static int ObtenerGastoCuentaAuxiliarID(int iAfectacionID, int iId)
        {
            int iCuentaAuxID = 0;
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.GastoCajaFacturado:
                case Cat.ContaAfectaciones.GastoReparteUtilidades:
                    var oGasto = Datos.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
                    var oContaEgreso = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == oGasto.ContaEgresoID);
                    if (oContaEgreso != null)
                        iCuentaAuxID = oContaEgreso.ContaCuentaAuxiliarID;
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.GastoNotaEfectivo:
                case Cat.ContaAfectaciones.InteresesBancarios:
                    var oGastoCont = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
                    if (oGastoCont != null)
                        iCuentaAuxID = oGastoCont.ContaCuentaAuxiliarID;
                    break;
            }

            return iCuentaAuxID;
        }

        private static int ObtenerNominaOficialCuentaAuxiliarID(int iAfectacionID, int iId, int iCuentaDeMayorID)
        {
            int iUsuarioID = 0;
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.NominaOficial:
                    var oNominaUs = Datos.GetEntity<NominaUsuario>(c => c.NominaUsuarioID == iId);
                    iUsuarioID = oNominaUs.IdUsuario;
                    break;
                case Cat.ContaAfectaciones.Pago2Por:
                case Cat.ContaAfectaciones.PagoImss:
                case Cat.ContaAfectaciones.PagoInfonavit:
                    var oImpUsuario = Datos.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
                    iUsuarioID = oImpUsuario.IdUsuario;
                    break;
            }

            var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaDeMayorID && c.RelacionID == iUsuarioID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oUsuario = Datos.GetEntity<Usuario>(c => c.UsuarioID == iUsuarioID && c.Estatus);
                var oRes = ContaProc.CrearCuentaAuxiliar(oUsuario.NombrePersona, iCuentaDeMayorID, iUsuarioID);
                iCuentaAuxID = oRes.Respuesta;
            }
            else
            {
                iCuentaAuxID = oCuentaAux.ContaCuentaAuxiliarID;
            }

            return iCuentaAuxID;
        }

        private static int ObtenerNominaImpuestoUsuarioCuentaAuxiliarID(int iAfectacionID, int iId, int iCuentaDeMayorID)
        {
            int iUsuarioID = 0;
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.Pago2Por:
                    var oPago = Datos.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
                    iUsuarioID = oPago.IdUsuario;
                    break;
            }

            var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaDeMayorID && c.RelacionID == iUsuarioID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oUsuario = Datos.GetEntity<Usuario>(c => c.UsuarioID == iUsuarioID && c.Estatus);
                var oRes = ContaProc.CrearCuentaAuxiliar(oUsuario.NombrePersona, iCuentaDeMayorID, iUsuarioID);
                iCuentaAuxID = oRes.Respuesta;
            }
            else
            {
                iCuentaAuxID = oCuentaAux.ContaCuentaAuxiliarID;
            }

            return iCuentaAuxID;
        }

        private static void AfectarConCostoVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVenta = Datos.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = Datos.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => c.Costo * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConPrecioSinIvaVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVenta = Datos.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = Datos.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => c.PrecioUnitario * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConIvaVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVenta = Datos.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = Datos.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => c.Iva * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConPrecioVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVenta = Datos.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = Datos.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => (c.PrecioUnitario + c.Iva) * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConCostoFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = Datos.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = oFactura.Costo;
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConSubtotalFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = Datos.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = oFactura.Subtotal;
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConIvaFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = Datos.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = oFactura.Iva;
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = Datos.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = (oFactura.Subtotal + oFactura.Iva);
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConPago(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = Datos.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConIvaPago(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = Datos.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConPagoVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = Datos.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId && c.TipoFormaPagoID == Cat.FormasDePago.Vale);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConPagoNoVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = Datos.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId && c.TipoFormaPagoID != Cat.FormasDePago.Vale);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConPagoValeDeVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = Datos.GetListOf<VentasPagosDetalleView>(c => c.VentaID == iId && c.FormaDePagoID == Cat.FormasDePago.Vale);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConCostoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            var oCompraDet = Datos.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            oDetalle.Cargo = (oCompraDet.Count > 0 ? oCompraDet.Sum(c => c.PrecioUnitario * c.Cantidad) : 0);
            oDetalle.Referencia = oCompra.FolioFactura;
        }

        private static void AfectarConCostoParteDeMovimientoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            decimal mCosto = 0;
            var oPartesMov = Datos.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            foreach (var oReg in oPartesMov)
            {
                var oPartePrecio = Datos.GetEntity<PartePrecio>(c => c.ParteID == oReg.ParteID && c.Estatus);
                mCosto += (oPartePrecio.Costo.Valor() * oReg.Cantidad);
            }
            oDetalle.Cargo = mCosto;
        }

        private static void AfectarConIvaCostoParteDeMovimientoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDeImporte(oDetalle.Cargo);
        }

        private static void AfectarConCostoMasIvaParteDeMovimientoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
            oDetalle.Cargo = UtilTheos.ObtenerImporteMasIva(oDetalle.Cargo);
        }

        private static void AfectarConIvaInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            var oCompraDet = Datos.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            decimal mMultIva = (Theos.Iva / 100);
            oDetalle.Cargo = (oCompraDet.Count > 0 ? oCompraDet.Sum(c => (c.PrecioUnitario * mMultIva) * c.Cantidad) : 0);
            oDetalle.Referencia = oCompra.FolioFactura;
        }

        private static void AfectarConPrecioInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            var oCompraDet = Datos.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            decimal mMultIva = (1 + (Theos.Iva / 100));
            oDetalle.Cargo = (oCompraDet.Count > 0 ? oCompraDet.Sum(c => (c.PrecioUnitario * mMultIva) * c.Cantidad) : 0);
            oDetalle.Referencia = oCompra.FolioFactura;
        }

        private static void AfectarConCostoDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = Datos.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => c.Costo * c.Cantidad) : 0);
        }

        private static void AfectarConPrecioSinIvaDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = Datos.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => c.PrecioUnitario * c.Cantidad) : 0);
        }

        private static void AfectarConIvaDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = Datos.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => c.Iva * c.Cantidad) : 0);
        }

        private static void AfectarConPrecioDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = Datos.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => (c.PrecioUnitario + c.Iva) * c.Cantidad) : 0);
        }

        private static void AfectarConCostoGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = Datos.GetListOf<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => c.Costo) : 0);
        }

        private static void AfectarConPrecioSinIvaGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = Datos.GetListOf<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => c.PrecioUnitario) : 0);
        }

        private static void AfectarConIvaGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = Datos.GetListOf<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => c.Iva) : 0);
        }

        private static void AfectarConPrecioGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = Datos.GetListOf<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => (c.PrecioUnitario + c.Iva)) : 0);
        }

        private static void AfectarConCajaEgreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConSubtotalCajaEgreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Subtotal.Valor();
        }

        private static void AfectarConIvaCajaEgreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Iva.Valor();
        }

        private static void AfectarConGastoContable(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConSubtotalGastoContable(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
            oDetalle.Cargo = UtilTheos.ObtenerPrecioSinIva(oReg.Importe);
        }

        private static void AfectarConIvaGastoContable(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDePrecio(oReg.Importe);
        }

        private static void AfectarConCajaIngreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<CajaIngreso>(c => c.CajaIngresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConImporteNotaDeCreditoFiscal(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == iId);
            oDetalle.Cargo = (oReg.Subtotal + oReg.Iva);
        }

        private static void AfectarConImporteSivIvaNotaDeCreditoFiscal(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == iId);
            oDetalle.Cargo = oReg.Subtotal;
        }

        private static void AfectarConIvaNotaDeCreditoFiscal(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == iId);
            oDetalle.Cargo = oReg.Iva;
        }

        private static void AfectarConDiferenciaCorteDeCaja(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<CajaEfectivoPorDia>(c => c.CajaEfectivoPorDiaID == iId && c.Estatus);
            oDetalle.Cargo = (oReg.Cierre - oReg.CierreDebeHaber).Valor();
            oDetalle.Cargo *= (oDetalle.Cargo < 0 ? -1 : 1);
        }

        private static void AfectarConPagoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == iId && c.Estatus);
            var oAbonos = Datos.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && c.Estatus);
            oDetalle.Cargo = oAbonos.Sum(c => c.Subtotal + c.Iva);
        }

        private static void AfectarConIvaPagoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == iId && c.Estatus);
            var oAbonos = Datos.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && c.Estatus);
            oDetalle.Cargo = oAbonos.Sum(c => c.Iva);
        }

        private static void AfectarConSubtotalNotaDeCreditoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iId);
            oDetalle.Cargo = oReg.Subtotal;
        }

        private static void AfectarConIvaNotaDeCreditoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iId);
            oDetalle.Cargo = oReg.Iva;
        }

        private static void AfectarConNotaDeCreditoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iId);
            oDetalle.Cargo = (oReg.Subtotal + oReg.Iva);
        }

        private static void AfectarConAbonoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            // var oAbonos = General.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && (c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoDirecto
            //    || c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoFactura) && c.Estatus);
            // oDetalle.Cargo = (oAbonos.Count > 0 ? oAbonos.Sum(c => c.Importe) : 0);
            var oReg = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId);
            oDetalle.Cargo = (oReg.Subtotal + oReg.Iva);
        }

        private static void AfectarConSubtotalAbonoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            // var oAbonos = General.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && (c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoDirecto
            //    || c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoFactura) && c.Estatus);
            // oDetalle.Cargo = (oAbonos.Count > 0 ? UtilLocal.ObtenerPrecioSinIva(oAbonos.Sum(c => c.Importe)) : 0);
            var oReg = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId);
            oDetalle.Cargo = oReg.Subtotal;
        }

        private static void AfectarConIvaAbonoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            // var oAbonos = General.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && (c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoDirecto
            //    || c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoFactura) && c.Estatus);
            // oDetalle.Cargo = (oAbonos.Count > 0 ? UtilLocal.ObtenerPrecioSinIva(oAbonos.Sum(c => c.Importe)) : 0);
            var oReg = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId);
            oDetalle.Cargo = oReg.Iva;
        }

        private static void AfectarConDevolucionMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevV = Datos.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iId);
            oDetalle.Cargo = oDevV.Total.Valor();

            var oPagoNeg = Datos.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oDevV.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo += (oPagoNeg == null ? 0 : oPagoNeg.Importe);  // Se suma con el pago negativo, por tanto queda como resta
            oDetalle.Cargo = (oDetalle.Cargo < 0 ? 0 : oDetalle.Cargo);
        }

        private static void AfectarConIvaDevolucionMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConDevolucionMenosLoAbonado(ref oDetalle, iId);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConImporteDevueltoDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDev = Datos.GetEntity<VentaDevolucion>(c => c.VentaDevolucionID == iId && c.Estatus);
            var oPagoNeg = Datos.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oDev.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo = (oPagoNeg == null ? 0 : (oPagoNeg.Importe * -1));
        }

        private static void AfectarConIvaImporteDevueltoDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConImporteDevueltoDevolucion(ref oDetalle, iId);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConImporteDevueltoGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            var oPagoNeg = Datos.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oReg.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo = (oPagoNeg == null ? 0 : (oPagoNeg.Importe * -1));
        }

        private static void AfectarConIvaImporteDevueltoGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConImporteDevueltoGarantia(ref oDetalle, iId);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConGarantiaMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            oDetalle.Cargo = (oReg.PrecioUnitario + oReg.Iva);

            var oPagoNeg = Datos.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oReg.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo += (oPagoNeg == null ? 0 : oPagoNeg.Importe);  // Se suma con el pago negativo, por tanto queda como resta
            oDetalle.Cargo = (oDetalle.Cargo < 0 ? 0 : oDetalle.Cargo);
        }

        private static void AfectarConIvaGarantiaMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConGarantiaMenosLoAbonado(ref oDetalle, iId);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConSubtotalVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iId && c.Estatus);
            oDetalle.Cargo = UtilTheos.ObtenerPrecioSinIva(oReg.Importe);
        }

        private static void AfectarConIvaVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iId && c.Estatus);
            oDetalle.Cargo = UtilTheos.ObtenerIvaDePrecio(oReg.Importe);
        }

        private static void AfectarConMovimientoBancario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iId);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConImporteFacturaCompra(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            oDetalle.Cargo = oReg.ImporteFactura;
        }

        private static void AfectarConCuentaNominaOficial(ref ContaPolizaDetalle oDetalle, int iId, int iCuentaDeMayorID)
        {
            var oNominaUs = Datos.GetEntity<NominaUsuario>(c => c.NominaUsuarioID == iId);
            var oNominaUsOf = Datos.GetEntity<NominaUsuarioOficial>(c => c.IdUsuario == oNominaUs.IdUsuario && c.NominaID == oNominaUs.NominaID
                && c.ContaCuentaDeMayorID == iCuentaDeMayorID);
            oDetalle.Cargo = oNominaUsOf.Importe;
        }

        private static void AfectarConNominaOficial(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oNominaUsV = Datos.GetEntity<NominaUsuariosView>(c => c.NominaUsuarioID == iId);
            oDetalle.Cargo = oNominaUsV.TotalOficial.Valor();
        }

        private static void AfectarConTotalPagoImpuestoUsuario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
            oDetalle.Cargo = oReg.Total;
        }

        private static void AfectarConGastoPagoImpuestoUsuario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
            oDetalle.Cargo = (oReg.Total - oReg.Retenido).Valor();
        }

        private static void AfectarConRetenidoPagoImpuestoUsuario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = Datos.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
            oDetalle.Cargo = oReg.Retenido.Valor();
        }

        #endregion
    }
}
