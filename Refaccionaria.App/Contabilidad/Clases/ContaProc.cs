using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    static class ContaProc
    {
        #region [ Procesos ]

        public static void GastoVerDevengarAutomaticamente(ContaEgreso oEgreso)
        {
            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oEgreso.ContaCuentaAuxiliarID);
            if (oCuentaAux.DevengarAut.Valor())
            {
                decimal mPorTotal = 0, mImporteDev = 0;
                var oDevAut = General.GetListOf<ContaCuentaAuxiliarDevengadoAutomatico>(c => c.ContaCuentaAuxiliarID == oEgreso.ContaCuentaAuxiliarID);
                foreach (var oReg in oDevAut)
                {
                    mPorTotal += oReg.Porcentaje;
                    decimal mImporte = Math.Round(oEgreso.Importe * (oReg.Porcentaje / 100), 2);
                    if (mPorTotal == 100)
                        mImporte = (oEgreso.Importe - mImporteDev);

                    ContaProc.GastoDevengar(oEgreso.ContaEgresoID, oReg.SucursalID, mImporte, oEgreso.Fecha);
                    mImporteDev += mImporte;
                }
            }
            // Se verifica si se debe hacer un devengado especial
            else if (oCuentaAux.DevengarAutEsp.Valor())
            {
                var oDevEsp = General.GetEntity<ContaCuentaAuxiliarDevengadoEspecial>(c => c.ContaCuentaAuxiliarID == oEgreso.ContaCuentaAuxiliarID);
                if (oDevEsp != null)
                    ContaProc.GastoDevengarEspecial(oEgreso.ContaEgresoID, oDevEsp.DuenioID, oEgreso.Importe, oEgreso.Fecha);
            }
        }

        #endregion

        #region [ Crear ]

        public static ResAcc<int> GastoCrear(ContaEgreso oGasto)
        {
            Guardar.Generico<ContaEgreso>(oGasto);
            // Se manda devengar, sólo se afecta si aplica, si no no
            ContaProc.GastoVerDevengarAutomaticamente(oGasto);

            return new ResAcc<int>(true, oGasto.ContaEgresoID);
        }

        public static ResAcc<int> GastoDevengar(ContaEgresoDevengado oDev, List<ContaEgresoDetalleDevengado> oDetalleDev)
        {
            // Se llenan datos calculables
            oDev.RealizoUsuarioID = (oDev.RealizoUsuarioID > 0 ? oDev.RealizoUsuarioID : GlobalClass.UsuarioGlobal.UsuarioID);
            //
            Guardar.Generico<ContaEgresoDevengado>(oDev);
            
            // Se llena el detalle, si hay
            if (oDetalleDev != null && oDetalleDev.Count > 0)
            {
                foreach (var oReg in oDetalleDev)
                {
                    oReg.ContaEgresoDevengadoID = oDev.ContaEgresoDevengadoID;
                    Guardar.Generico<ContaEgresoDetalleDevengado>(oReg);
                }

                // Se verifica si ya se completaron las cantidades del detalle, en cuyo caso, se hace que el importe cuadre con el total
                var oEgresoV = General.GetEntity<ContaEgresosView>(c => c.ContaEgresoID == oDev.ContaEgresoID);
                if (oEgresoV.Importe != oEgresoV.ImporteDev) {
                    var oEgresosDetV = General.GetListOf<ContaEgresosDetalleView>(c => c.ContaEgresoID == oDev.ContaEgresoID);
                    if (oEgresosDetV.Sum(c => c.Cantidad) == oEgresosDetV.Sum(c => c.CantidadDev))
                    {
                        oDev.Importe += (oEgresoV.Importe - oEgresoV.ImporteDev.Valor());
                        Guardar.Generico<ContaEgresoDevengado>(oDev);
                    }
                }
            }

            // Se editan los gastos fijos, si aplican
            var oEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == oDev.ContaEgresoID);
            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oEgreso.ContaCuentaAuxiliarID);
            if (oCuentaAux.AfectaMetas.Valor() && oCuentaAux.SumaGastosFijos.Valor())
            {
                var oGastoFijo = General.GetEntity<SucursalGastoFijo>(c => c.SucursalID == oDev.SucursalID && c.ContaCuentaAuxiliarID == oCuentaAux.ContaCuentaAuxiliarID);
                if (oGastoFijo == null)
                {
                    oGastoFijo = new SucursalGastoFijo();
                    oGastoFijo.SucursalID = oDev.SucursalID;
                    oGastoFijo.ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID;
                }

                // Se calcula el importe según el caso
                // decimal mImporte = ((oDev.Importe / oCuentaAux.DivisorDia.Valor()) * 7);
                decimal mImporte = 0;
                if (oCuentaAux.CalculoSemanal.Valor())
                {
                    mImporte = UtilLocal.GastoCalcularImporteDiario(oDev.Fecha, oDev.Importe, oCuentaAux.PeriodicidadMes.Valor());
                    oGastoFijo.Importe = (mImporte * 7);
                }
                else
                {
                    oGastoFijo.Importe = oDev.Importe;
                }
                
                Guardar.Generico<SucursalGastoFijo>(oGastoFijo);
            }

            return new ResAcc<int>(true, oDev.ContaEgresoDevengadoID);
        }

        public static ResAcc<int> GastoDevengar(int iEgresoID, int iSucursalID, decimal mImporte, DateTime dFecha)
        {
            var oDev = General.GetEntity<ContaEgresoDevengado>(c => c.ContaEgresoID == iEgresoID && c.SucursalID == iSucursalID);
            if (oDev == null)
            {
                oDev = new ContaEgresoDevengado()
                {
                    ContaEgresoID = iEgresoID,
                    SucursalID = iSucursalID,
                    Fecha = dFecha
                };
            }
            oDev.Importe = mImporte;

            return ContaProc.GastoDevengar(oDev, null);
        }

        public static ResAcc<int> GastoDevengarEspecial(ContaEgresoDevengadoEspecial oDev, List<ContaEgresoDetalleDevengadoEspecial> oDetalleDev)
        {
            // Se llenan datos calculables
            oDev.RealizoUsuarioID = (oDev.RealizoUsuarioID > 0 ? oDev.RealizoUsuarioID : GlobalClass.UsuarioGlobal.UsuarioID);
            //
            Guardar.Generico<ContaEgresoDevengadoEspecial>(oDev);

            // Se llena el detalle, si hay
            if (oDetalleDev != null && oDetalleDev.Count > 0)
            {
                foreach (var oReg in oDetalleDev)
                {
                    oReg.ContaEgresoDevengadoEspecialID = oDev.ContaEgresoDevengadoEspecialID;
                    Guardar.Generico<ContaEgresoDetalleDevengadoEspecial>(oReg);
                }
            }
                    
            return new ResAcc<int>(true, oDev.ContaEgresoDevengadoEspecialID);
        }

        public static ResAcc<int> GastoDevengarEspecial(int iEgresoID, int iDuenioID, decimal mImporte, DateTime dFecha)
        {
            var oDev = new ContaEgresoDevengadoEspecial()
            {
                ContaEgresoID = iEgresoID,
                DuenioID = iDuenioID,
                Fecha = dFecha,
                Importe = mImporte
            };

            return ContaProc.GastoDevengarEspecial(oDev, null);
        }

        public static ResAcc<int> CrearCuentaAuxiliar(string sCuenta, int iCuentaDeMayorID, int? iRelacionID)
        {
            var oCuentaAux = new ContaCuentaAuxiliar()
            {
                CuentaAuxiliar = sCuenta,
                ContaCuentaDeMayorID = iCuentaDeMayorID,
                RelacionID = iRelacionID
            };
            Guardar.Generico<ContaCuentaAuxiliar>(oCuentaAux);

            return new ResAcc<int>(true, oCuentaAux.ContaCuentaAuxiliarID);
        }

        #endregion

        #region [ Eliminar ]

        public static ResAcc<bool> CuentaAuxiliarEliminar(int iCuentaID)
        {
            // Se verifica si existe un gasto contable usando esta cuenta
            if (General.Exists<ContaEgreso>(c => c.ContaCuentaAuxiliarID == iCuentaID))
            {
                UtilLocal.MensajeError("La Cuenta seleccionada ha sido utilizada en varios movimientos. No se puede eliminar.");
                return new ResAcc<bool>(false);
            }
            // Se verifica si existe una poliza contable usando esta cuenta
            if (General.Exists<ContaPolizaDetalle>(c => c.ContaCuentaAuxiliarID == iCuentaID))
            {
                UtilLocal.MensajeError("La Cuenta seleccionada ha sido utilizada en una o varias Pólizas. No se puede eliminar.");
                return new ResAcc<bool>(false);
            }

            // Se borran los registros ligados de Devengados automáticos
            var oDevsAut = General.GetListOf<ContaCuentaAuxiliarDevengadoAutomatico>(c => c.ContaCuentaAuxiliarID == iCuentaID);
            foreach (var oDevAut in oDevsAut)
                Guardar.Eliminar<ContaCuentaAuxiliarDevengadoAutomatico>(oDevAut);
            // Se borran los registros de SucursalGastoFijo, si hubiera
            var oGastosFijos = General.GetListOf<SucursalGastoFijo>(c => c.ContaCuentaAuxiliarID == iCuentaID);
            foreach (var oReg in oGastosFijos)
                Guardar.Eliminar<SucursalGastoFijo>(oReg);
            // Se procede a eliminar la cuenta
            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == iCuentaID);
            Guardar.Eliminar<ContaCuentaAuxiliar>(oCuentaAux);

            return new ResAcc<bool>(true);
        }

        public static ResAcc<bool> GastoEliminar(int iEgresoID)
        {
            // Se borran los datos de devengado, si hubiera
            var oDevs = General.GetListOf<ContaEgresoDevengado>(c => c.ContaEgresoID == iEgresoID);
            foreach (var oReg in oDevs)
                ContaProc.DevengadoEliminar(oReg.ContaEgresoDevengadoID);

            // Se borran los datos de detalle, si hubiera
            var oDetalle = General.GetListOf<ContaEgresoDetalle>(c => c.ContaEgresoID == iEgresoID);
            foreach (var oReg in oDetalle)
                Guardar.Eliminar<ContaEgresoDetalle>(oReg);

            // Se borran los datos del devengado especial, si hubiera
            var oDevsEsp = General.GetListOf<ContaEgresoDevengadoEspecial>(c => c.ContaEgresoID == iEgresoID);
            foreach (var oReg in oDevsEsp)
                ContaProc.DevengadoEspecialEliminar(oReg.ContaEgresoDevengadoEspecialID);

            // Se borra el gasto en sí
            var oGasto = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iEgresoID);
            Guardar.Eliminar<ContaEgreso>(oGasto);

            // Se borra el movimiento bancario, si no ha sido conciliado
            bool bConciliado = false;
            var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.RelacionTabla == Cat.Tablas.ContaEgreso && c.RelacionID == iEgresoID);
            if (oMov != null)
            {
                bConciliado = oMov.Conciliado;
                if (!bConciliado)
                    Guardar.Eliminar<BancoCuentaMovimiento>(oMov);
            }

            // Se borra la póliza, si el movimiento bancario no ha sido conciliado
            if (!bConciliado)
            {
                var oPoliza = General.GetEntity<ContaPoliza>(c => c.RelacionTabla == Cat.Tablas.ContaEgreso && c.RelacionID == iEgresoID);
                if (oPoliza != null)
                    ContaProc.BorrarPoliza(oPoliza.ContaPolizaID);
            }

            return new ResAcc<bool>(true);
        }
        
        public static ResAcc<bool> DevengadoEliminar(int iDevID)
        {
            // Se borra el detalle, si tiene
            var oDetalleDev = General.GetListOf<ContaEgresoDetalleDevengado>(c => c.ContaEgresoDevengadoID == iDevID);
            foreach (var oReg in oDetalleDev)
                Guardar.Eliminar<ContaEgresoDetalleDevengado>(oReg);
            // Se borra el "devengado"
            var oDev = General.GetEntity<ContaEgresoDevengado>(c => c.ContaEgresoDevengadoID == iDevID);
            Guardar.Eliminar<ContaEgresoDevengado>(oDev);

            return new ResAcc<bool>(true);
        }

        public static ResAcc<bool> DevengadoEspecialEliminar(int iDevID)
        {
            // Se borra el detalle, si tiene
            var oDetalleDev = General.GetListOf<ContaEgresoDetalleDevengadoEspecial>(c => c.ContaEgresoDevengadoEspecialID == iDevID);
            foreach (var oReg in oDetalleDev)
                Guardar.Eliminar<ContaEgresoDetalleDevengadoEspecial>(oReg);
            // Se borra el "devengado"
            var oDev = General.GetEntity<ContaEgresoDevengadoEspecial>(c => c.ContaEgresoDevengadoEspecialID == iDevID);
            Guardar.Eliminar<ContaEgresoDevengadoEspecial>(oDev);

            return new ResAcc<bool>(true);
        }

        #endregion

        #region [ Consultas ]

        public static decimal GastoImporteSemana(pauContaCuentasPorSemana_Result oEgresoPorSem)
        {
            decimal mImporte = 0;
            if (oEgresoPorSem.PeriodicidadMes.HasValue)
            {
                DateTime dInicioPer = oEgresoPorSem.Fecha.DiaPrimero().Date;
                DateTime dFinPer = dInicioPer.AddMonths(oEgresoPorSem.PeriodicidadMes.Valor()).AddDays(-1);
                decimal mImporteDiario = (oEgresoPorSem.ImporteDev.Valor() / ((dFinPer - dInicioPer).Days + 1));
                int iDias = 7;
                DateTime dIniSem = UtilLocal.InicioSemanaSabAVie(dInicioPer).Date;
                DateTime dFinSem = dIniSem.AddDays(6);

                // Se calcula el importe correspondiente
                if (dIniSem < dInicioPer)
                {
                    // Se calcula el importe del gasto anterior
                    DateTime dInicioPerAnt = dInicioPer.AddMonths(oEgresoPorSem.PeriodicidadMes.Valor() * -1);
                    var oDevAnt = General.GetEntity<ContaEgresoDevengado>(c => c.ContaEgresoID == oEgresoPorSem.ContaEgresoID && c.SucursalID == oEgresoPorSem.SucursalID
                        && c.Fecha >= dInicioPerAnt && c.Fecha < dInicioPer);
                    if (oDevAnt != null)
                    {
                        decimal mImporteDiaAnt = (oDevAnt.Importe / (dInicioPer - dInicioPerAnt).Days);
                        mImporte += ((dInicioPer - dIniSem).Days * mImporteDiaAnt);

                        // Días del mes actual a contar
                        iDias = dFinSem.Day;
                    }
                }
                else if (dFinSem > dFinPer)
                {
                    // Se calcula el importe del gasto posterior, si hubiera
                    DateTime dFinPerPos = dFinPer.AddMonths(oEgresoPorSem.PeriodicidadMes.Valor());
                    DateTime dFinPerPosC = dFinPerPos.AddDays(1);
                    var oDevPos = General.GetEntity<ContaEgresoDevengado>(c => c.ContaEgresoID == oEgresoPorSem.ContaEgresoID && c.SucursalID == oEgresoPorSem.SucursalID
                        && c.Fecha > dFinPer && c.Fecha < dFinPerPosC);
                    if (oDevPos != null)
                    {
                        decimal mImporteDiaPos = (oDevPos.Importe / (dFinPerPos - dFinPer).Days);
                        mImporte += ((dFinSem - dFinPer).Days * mImporteDiaPos);

                        // Días del mes actual a contar
                        iDias = (dFinPer - dIniSem).Days;
                    }
                }
                // int iDias = (dIniSem < dInicioPer ? dFinSem.Day : (dFinSem > dFinPer ? ((dIniSem.DiaUltimo().Day - dIniSem.Day) + 1) : 7));
                mImporte += (mImporteDiario * iDias);
            }
            return mImporte;
        }

        public class GastoSem
        {
            public DateTime Semana { get; set; }
            public string Grupo { get; set; }
            public decimal Importe { get; set; }
        }
        public static List<GastoSem> GastosSemanalizados(List<pauContaCuentasPorSemana_Result> oDatos, DateTime dUltSem)
        {
            var oGastosSem = new List<GastoSem>();

            foreach (var oReg in oDatos)
            {
                if (oReg.PeriodicidadMes.HasValue)
                {
                    DateTime dInicioPer = oReg.Fecha.DiaPrimero().Date;
                    DateTime dFinPer = dInicioPer.AddMonths(oReg.PeriodicidadMes.Valor()).AddDays(-1);
                    decimal mImporteDiario = (oReg.ImporteDev.Valor() / ((dFinPer - dInicioPer).Days + 1));
                    decimal mImporte; int iDias;
                    DateTime dIniSem = UtilLocal.InicioSemanaSabAVie(dInicioPer).Date;

                    while (dIniSem <= dUltSem)
                    {
                        // Se verifica si ya existe la semana actual
                        var oSem = oGastosSem.Find(c => c.Semana == dIniSem && c.Grupo == oReg.Sucursal);
                        if (oSem == null)
                            oGastosSem.Add(oSem = new GastoSem() { Semana = dIniSem, Grupo = oReg.Sucursal });

                        // Se verifica si se debe de seguir semanalizando
                        if (oReg.FinSemanalizar.HasValue && oReg.FinSemanalizar <= dIniSem)
                            break;
                        // Se verifica la fecha final, 
                        if (dIniSem > dFinPer)
                            break;

                        // Se calcula el importe correspondiente
                        DateTime dFinSem = dIniSem.AddDays(6);
                        if (dIniSem < dInicioPer)
                            iDias = dFinSem.Day;
                        else if (dIniSem <= dFinPer && dFinSem > dFinPer)
                            iDias = ((dIniSem.DiaUltimo().Day - dIniSem.Day) + 1);
                        else if (dIniSem > dFinPer && (dIniSem - dFinPer).Days < 7)
                        {
                            iDias = (dIniSem.Day - 1);
                        }
                        else
                        {
                            iDias = 7;
                        }
                        mImporte = (mImporteDiario * iDias);
                        dIniSem = dIniSem.AddDays(7);

                        // Se va sumando el importe en la semana correspondiente
                        oSem.Importe += mImporte;
                    }
                }
                else
                {
                    // Se verifica si ya existe la semana actual
                    DateTime dSem = UtilLocal.InicioSemanaSabAVie(oReg.Fecha).Date;
                    var oSem = oGastosSem.Find(c => c.Semana == dSem && c.Grupo == oReg.Sucursal);
                    if (oSem == null)
                        oGastosSem.Add(oSem = new GastoSem() { Semana = dSem, Grupo = oReg.Sucursal });
                    // Se va sumando el importe en la semana correspondiente
                    oSem.Importe += oReg.ImporteDev.Valor();
                }
            }

            return oGastosSem.OrderBy(c => c.Grupo).ThenBy(c => c.Semana).ToList();
        }

        public static List<GastoSem> GastosSemanalizados(List<ContaEgresosDevengadoEspecialCuentasView> oDatos, DateTime dUltSem)
        {
            var oListaSem = new List<pauContaCuentasPorSemana_Result>();
            foreach (var oReg in oDatos)
            {
                oListaSem.Add(new pauContaCuentasPorSemana_Result()
                {
                    Fecha = oReg.Fecha,
                    Sucursal = oReg.Duenio,
                    ImporteDev = oReg.ImporteDev,
                    PeriodicidadMes = oReg.PeriodicidadMes,
                    FinSemanalizar = oReg.FinSemanalizar
                });
            }

            return ContaProc.GastosSemanalizados(oListaSem, dUltSem);
        }

        #endregion

        #region [ Pólizas ]

        public static void BorrarPoliza(int iPolizaID)
        {
            var oPoliza = General.GetEntity<ContaPoliza>(c => c.ContaPolizaID == iPolizaID);
            var oPolizaDet = General.GetListOf<ContaPolizaDetalle>(c => c.ContaPolizaID == iPolizaID);
            foreach (var oReg in oPolizaDet)
                Guardar.Eliminar<ContaPolizaDetalle>(oReg);
            Guardar.Eliminar<ContaPoliza>(oPoliza);
        }

        public static ContaPoliza CrearPoliza(ContaPoliza oPoliza, int iCuentaCargo, int iCuentaAbono, decimal mImporte, string sReferencia)
        {
            if (oPoliza.Fecha == DateTime.MinValue)
                oPoliza.Fecha = DateTime.Now;
            if (oPoliza.RealizoUsuarioID <= 0)
                oPoliza.RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
            if (oPoliza.SucursalID <= 0)
                oPoliza.SucursalID = GlobalClass.SucursalID;
                        
            Guardar.Generico<ContaPoliza>(oPoliza);

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
                Guardar.Generico<ContaPolizaDetalle>(oDetCargo);
            if (iCuentaAbono > 0)
                Guardar.Generico<ContaPolizaDetalle>(oDetAbono);

            return oPoliza;
        }

        public static ContaPoliza CrearPoliza(int iTipoPolizaID, string sConcepto, int iCuentaCargo, int iCuentaAbono, decimal mImporte, string sReferencia
            , string sTabla, int? iRelacionID, int iSucursalID)
        {
            var oPoliza = new ContaPoliza()
            {
                Fecha = DateTime.Now,
                ContaTipoPolizaID = iTipoPolizaID,
                Concepto = sConcepto,
                RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                SucursalID = iSucursalID,
                RelacionTabla = sTabla,
                RelacionID = iRelacionID
            };

            return ContaProc.CrearPoliza(oPoliza, iCuentaCargo, iCuentaAbono, mImporte, sReferencia);
        }

        public static ContaPoliza CrearPoliza(int iTipoPolizaID, string sConcepto, int iCuentaCargo, int iCuentaAbono, decimal mImporte, string sReferencia
            , string sTabla, int? iRelacionID)
        {
            return ContaProc.CrearPoliza(iTipoPolizaID, sConcepto, iCuentaCargo, iCuentaAbono, mImporte, sReferencia, sTabla, iRelacionID, GlobalClass.SucursalID);
        }

        public static ContaPoliza CrearPolizaAfectacion(int iAfectacionID, int iId, string sReferencia, string sConcepto, int iSucursalID, DateTime dFecha)
        {
            // Se genera el detalle de la poliza
            ContaPolizaDetalle oRegPoliza;
            var oPolizaDet = new List<ContaPolizaDetalle>();

            decimal mCargo = 0, mAbono = 0;
            var oAfectacion = General.GetEntity<ContaConfigAfectacion>(c => c.ContaConfigAfectacionID == iAfectacionID);
            var oAfectacionDet = General.GetListOf<ContaConfigAfectacionDetalle>(c => c.ContaConfigAfectacionID == iAfectacionID);
            foreach (var oReg in oAfectacionDet)
            {
                oRegPoliza = null;
                bool bFacturaGlobalAnticipoClientes = false;

                if (oReg.EsCasoFijo)
                {
                    // Actualmente sólo hay un caso fijo y por tanto no se verifica más ni se hacen casos
                    oRegPoliza = ContaProc.GafClientes(iAfectacionID, iId);
                    switch (GlobalClass.SucursalID)
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
                            if (iAfectacionID == Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal)
                                bFacturaGlobalAnticipoClientes = true;
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

                    // Se verifica y ajusta el dato de la sucursal, si se debe modificar
                    switch (oReg.ContaPolizaAsigSucursalID)
                    {
                        case Cat.ContaPolizaAsignacionDeSucursales.Local: oRegPoliza.SucursalID = iSucursalID; break;
                        case Cat.ContaPolizaAsignacionDeSucursales.Matriz: oRegPoliza.SucursalID = Cat.Sucursales.Matriz; break;
                        case Cat.ContaPolizaAsignacionDeSucursales.DondeSeHizo: break;  // La sucursal se llena en la afectación del importe
                    }
                    // Si no se llenó la sucursal, porque no se dió el caso, se llena con la sucursal local
                    if (oRegPoliza.SucursalID == 0)
                        oRegPoliza.SucursalID = iSucursalID;

                    oPolizaDet.Add(oRegPoliza);

                    // Para la validación
                    mCargo += oRegPoliza.Cargo;
                    mAbono += oRegPoliza.Abono;

                    // Se verifica si es la afectación de Anticipo Clientes en la Factura Global, para hacer un ajuste especial
                    if (bFacturaGlobalAnticipoClientes)
                    {
                        DateTime dHoy = DateTime.Now;
                        var oPagosVales = General.GetListOf<VentasPagosDetalleAvanzadoView>(c => c.SucursalID == GlobalClass.SucursalID 
                            && EntityFunctions.TruncateTime(c.Fecha) == dHoy && !c.Facturada && c.Importe > 0 && c.FormaDePagoID == Cat.FormasDePago.Vale);
                        foreach (var oPagoVale in oPagosVales)
                        {
                            var oVale = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == oPagoVale.NotaDeCreditoID && c.Estatus);
                            var oRegAntC = new ContaPolizaDetalle()
                            {
                                ContaCuentaAuxiliarID = Cat.ContaCuentasAuxiliares.AnticipoClientes,
                                Referencia = sReferencia,
                                Cargo = (oReg.EsCargo ? oPagoVale.Importe : 0),
                                Abono = (oReg.EsCargo ? 0 : oPagoVale.Importe),
                                SucursalID = oVale.SucursalID.Valor()
                            };
                            oPolizaDet.Add(oRegAntC);
                            // Para la validación
                            mCargo += oRegAntC.Cargo;
                            mAbono += oRegAntC.Abono;
                        }

                        // Se borra la PolizaDetalle creada por el proceso normal
                        if (oPagosVales.Count > 0)
                            oPolizaDet.Remove(oRegPoliza);
                    }
                }
            }

            // Se validan que los movimientos den cero como resultado
            bool bError = (mCargo != mAbono);
            if (bError)
            {
                // Se registra la diferencia y se sale, pues no coincide
                var oError = new ContaPolizaError()
                {
                    Fecha = dFecha,
                    ContaTipoPolizaID = oAfectacion.ContaTipoPolizaID,
                    Concepto = (sReferencia + " / " + sConcepto),
                    RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                    SucursalID = iSucursalID,
                    Detalle = ""
                };
                foreach (var oReg in oPolizaDet)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oReg.ContaCuentaAuxiliarID);
                    oError.Detalle += string.Format("{0}\t{1}\t{2}\t{3}\n", oReg.ContaCuentaAuxiliarID, oCuentaAux.CuentaAuxiliar.RellenarCortarDerecha(32)
                        , oReg.Cargo.ToString(GlobalClass.FormatoMoneda), oReg.Abono.ToString(GlobalClass.FormatoMoneda));
                }
                // Se deja de llenar el registro de ContaPolizaError por incompatibilidad con los nuevos cambios de ContaPoliza - Moi 2015-10-26
                // Guardar.Generico<ContaPolizaError>(oError);
                // Se muestra un mensaje de error
                UtilLocal.MensajeAdvertencia(string.Format("Se encontró una diferencia al tratar de crear la Póliza Contable.\n\n\t\t\t\tCargo\tAbono\n{0}\t\t\t\t{1}\t{2}"
                    , oError.Detalle, mCargo.ToString(GlobalClass.FormatoMoneda), mAbono.ToString(GlobalClass.FormatoMoneda)));
                // Ya no se sale, se guarda la póliza y se marca como error
                // return;
            }

            // Se genera la Póliza
            var oPoliza = new ContaPoliza()
            {
                Fecha = dFecha,
                ContaTipoPolizaID = oAfectacion.ContaTipoPolizaID,
                Concepto = (sReferencia + " / " + sConcepto),
                RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                SucursalID = iSucursalID,
                RelacionTabla = ContaProc.ObtenerTablaDeAfectacion(iAfectacionID),
                RelacionID = iId,
                Origen = oAfectacion.Operacion,
                Error = bError
            };

            // Se guardan los datos
            Guardar.Generico<ContaPoliza>(oPoliza);
            foreach (var oReg in oPolizaDet)
            {
                oReg.ContaPolizaID = oPoliza.ContaPolizaID;
                Guardar.Generico<ContaPolizaDetalle>(oReg);
            }

            return oPoliza;
        }

        public static ContaPoliza CrearPolizaAfectacion(int iAfectacionID, int iId, string sReferencia, string sConcepto, int iSucursalID)
        {
            return ContaProc.CrearPolizaAfectacion(iAfectacionID, iId, sReferencia, sConcepto, iSucursalID, DateTime.Now);
        }

        public static ContaPoliza CrearPolizaAfectacion(int iAfectacionID, int iId, string sReferencia, string sConcepto, DateTime dFecha)
        {
            return ContaProc.CrearPolizaAfectacion(iAfectacionID, iId, sReferencia, sConcepto, GlobalClass.SucursalID, dFecha);
        }

        public static ContaPoliza CrearPolizaAfectacion(int iAfectacionID, int iId, string sReferencia, string sConcepto)
        {
            return ContaProc.CrearPolizaAfectacion(iAfectacionID, iId, sReferencia, sConcepto, GlobalClass.SucursalID, DateTime.Now);
        }

        public static string ObtenerTablaDeAfectacion(int iAfectacionID)
        {
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
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
                    return Cat.Tablas.NotaDeCreditoFiscalDetalle;
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
                case Cat.ContaAfectaciones.PagoProveedorDirectoCpcp:
                    return Cat.Tablas.ProveedorPolizaDetalle;
                case Cat.ContaAfectaciones.Resguardo:
                case Cat.ContaAfectaciones.GastoReparteUtilidades:
                case Cat.ContaAfectaciones.GastoCajaFacturado:
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
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
                case Cat.ContaAfectaciones.VentaCredito:
                    ContaProc.AfectarConPrecioVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    ContaProc.AfectarConPago(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDePago(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConFactura(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    ContaProc.AfectarConCostoMasIvaParteDeMovimientoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                    ContaProc.AfectarConPrecioDevolucion(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                    ContaProc.AfectarConPrecioGarantia(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    ContaProc.AfectarConImporteNotaDeCreditoFiscalDetalle(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeNotaDeCreditoFiscalDetalle(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                    ContaProc.AfectarConDevolucionMenosLoAbonado(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConGarantiaMenosLoAbonado(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeGarantia(ref oDetalle, iId);
                    break;
            }
            return oDetalle;
        }

        public static ContaPolizaDetalle GafProveedores(int iAfectacionID, int iId)
        {
            var oDetalle = new ContaPolizaDetalle();
            switch (iAfectacionID)
            {
                case Cat.ContaAfectaciones.CompraCreditoFactura:
                    ContaProc.AfectarConTotalImporteFacturaInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.CompraCreditoNota:
                    ContaProc.AfectarConTotalImporteFacturaInventario(ref oDetalle, iId);
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
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
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
                case Cat.ContaAfectaciones.DevolucionCompraPago:
                case Cat.ContaAfectaciones.DevolucionCompraNotaDeCredito:
                    ContaProc.AfectarConCostoInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.CompraCreditoFactura:
                    ContaProc.AfectarConSubtotalImporteFacturaInventario(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.CompraCreditoNota:
                    ContaProc.AfectarConTotalImporteFacturaInventario(ref oDetalle, iId);
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
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
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
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
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
                    ContaProc.AfectarConImporteSivIvaNotaDeCreditoFiscalDetalle(ref oDetalle, iId);
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
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
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
                    ContaProc.AfectarSucursalConVentaDePago(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    ContaProc.AfectarConIvaNotaDeCreditoFiscalDetalle(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeNotaDeCreditoFiscalDetalle(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    // Se debe afectar con el iva de (el importe de la devolución menos el importe de lo abonado)
                    ContaProc.AfectarConIvaDevolucionMenosLoAbonado(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeDevolucion(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    ContaProc.AfectarConIvaGarantiaMenosLoAbonado(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConVentaDeGarantia(ref oDetalle, iId);
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
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                    ContaProc.AfectarConPagoNoValeDeVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
                    // ContaProc.AfectarConPrecioVenta(ref oDetalle, iId);
                    // Se verifica si la venta es del día o de días anteriores
                    if (General.Exists<Venta>(c => c.VentaID == iId && c.Fecha < DateTime.Today && c.Estatus))
                        ContaProc.AfectarConPagoDeVenta(ref oDetalle, iId);
                    else
                        ContaProc.AfectarConPagoNoValeDeVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    ContaProc.AfectarConPagoNoVale(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoVale:
                    ContaProc.AfectarConPagoValeDeVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConFacturaGlobalNoValesDeFactura(ref oDetalle, iId);
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
                    ContaProc.AfectarConIvaImporteFacturaInventario(ref oDetalle, iId);
                    break;
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
                    ContaProc.AfectarSucursalConValeDePago(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                    ContaProc.AfectarConPrecioGarantia(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConValeDeGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                case Cat.ContaAfectaciones.GarantiaVentaValeTicket:
                    ContaProc.AfectarConImporteDevueltoGarantia(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal:
                    ContaProc.AfectarConFacturaGlobalValesDeFactura(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                    ContaProc.AfectarConPagoValeDeVenta(ref oDetalle, iId);
                    ContaProc.AfectarSucursalConValeDeVenta(ref oDetalle, iId);
                    break;
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
                    // Se verifica si la venta es del día o de días anteriores
                    if (General.Exists<Venta>(c => c.VentaID == iId && EntityFunctions.TruncateTime(c.Fecha) == DateTime.Today && c.Estatus))
                        ContaProc.AfectarConPagoValeDeVenta(ref oDetalle, iId);
                    // Para lo de la sucursal
                    ContaProc.AfectarSucursalConValeDeVenta(ref oDetalle, iId);
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
                    ContaProc.AfectarConImporteSivIvaNotaDeCreditoFiscalDetalle(ref oDetalle, iId);
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
                case Cat.ContaAfectaciones.VentaContadoFacturaDirecta:
                case Cat.ContaAfectaciones.VentaContadoFacturaConvertida:
                case Cat.ContaAfectaciones.VentaCredito:
                    var oVenta = General.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
                    iClienteID = oVenta.ClienteID;
                    break;
                case Cat.ContaAfectaciones.PagoVentaCredito:
                    var oPagoV = General.GetEntity<VentasPagosView>(c => c.VentaPagoID == iId);
                    iClienteID = oPagoV.ClienteID.Valor();
                    break;
                case Cat.ContaAfectaciones.SalidaInventario:
                    iClienteID = Cat.Clientes.Mostrador;
                    break;
                case Cat.ContaAfectaciones.DevolucionVentaPago:
                case Cat.ContaAfectaciones.DevolucionVentaValeTicket:
                case Cat.ContaAfectaciones.DevolucionVentaValeFactura:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.DevolucionVentaCreditoFacturadaVale:
                    var oDevV = General.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iId);
                    iClienteID = oDevV.ClienteID.Valor();
                    break;
                case Cat.ContaAfectaciones.GarantiaVentaPagoFactura:
                case Cat.ContaAfectaciones.GarantiaVentaValeFactura:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturadaPago:
                case Cat.ContaAfectaciones.GarantiaVentaCreditoFacturaVale:
                    var oGarantiaV = General.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == iId);
                    iClienteID = oGarantiaV.ClienteID.Valor();
                    break;
                case Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta:
                    var oNotaCFDet = General.GetEntity<NotaDeCreditoFiscalDetalle>(c => c.NotaDeCreditoFiscalDetalleID == iId);
                    var oNotaCF = General.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == oNotaCFDet.NotaDeCreditoFiscalID);
                    iClienteID = oNotaCF.ClienteID;
                    break;
            }

            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Clientes && c.RelacionID == iClienteID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oCliente = General.GetEntity<Cliente>(c => c.ClienteID == iClienteID && c.Estatus);
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
                    var oMov = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
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
                    var oAbonoProv = General.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId && c.Estatus);
                    var oProveedorPol = General.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == oAbonoProv.ProveedorPolizaID && c.Estatus);
                    iProveedorID = oProveedorPol.ProveedorID;
                    break;
            }

            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Proveedores && c.RelacionID == iProveedorID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oProveedor = General.GetEntity<Proveedor>(c => c.ProveedorID == iProveedorID && c.Estatus);
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
                    var oMov = General.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId && c.Estatus);
                    var oPago = General.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == oMov.ProveedorPolizaID && c.Estatus);
                    iBancoCuentaID = oPago.BancoCuentaID;
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.GastoNotaEfectivo:
                case Cat.ContaAfectaciones.InteresesBancarios:
                    var oContaEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
                    iBancoCuentaID = oContaEgreso.BancoCuentaID.Valor();
                    break;
                case Cat.ContaAfectaciones.DepositoBancario:
                    var oMovBanco = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iId);
                    iBancoCuentaID = oMovBanco.BancoCuentaID.Valor();
                    break;
                case Cat.ContaAfectaciones.NominaOficial:
                    var oNominaUs = General.GetEntity<NominaUsuario>(c => c.NominaUsuarioID == iId);
                    var oNomina = General.GetEntity<Nomina>(c => c.NominaID == oNominaUs.NominaID);
                    iBancoCuentaID = oNomina.BancoCuentaID;
                    break;
                case Cat.ContaAfectaciones.Pago2Por:
                case Cat.ContaAfectaciones.PagoImss:
                case Cat.ContaAfectaciones.PagoInfonavit:
                    var oImpUsuario = General.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
                    var oImpuesto = General.GetEntity<NominaImpuesto>(c => c.NominaImpuestoID == oImpUsuario.NominaImpuestoID);
                    iBancoCuentaID = oImpuesto.BancoCuentaID;
                    break;
            }

            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Bancos && c.RelacionID == iBancoCuentaID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oCuentaBanco = General.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iBancoCuentaID);
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
                    var oMov = General.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId && c.Estatus);
                    var oPago = General.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == oMov.ProveedorPolizaID && c.Estatus);
                    iBancoCuentaID = oPago.BancoCuentaID;
                    break;
                case Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp:
                    var oContaEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
                    iBancoCuentaID = oContaEgreso.BancoCuentaID.Valor();
                    break;
            }

            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo && c.RelacionID == iBancoCuentaID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oCuentaBanco = General.GetEntity<BancoCuenta>(c => c.BancoCuentaID == iBancoCuentaID);
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
                    var oGasto = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
                    var oContaEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == oGasto.ContaEgresoID);
                    if (oContaEgreso != null)
                        iCuentaAuxID = oContaEgreso.ContaCuentaAuxiliarID;
                    break;
                case Cat.ContaAfectaciones.GastoFacturadoBanco:
                case Cat.ContaAfectaciones.GastoFacturadoEfectivo:
                case Cat.ContaAfectaciones.GastoNotaEfectivo:
                case Cat.ContaAfectaciones.InteresesBancarios:
                case Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp:
                    var oGastoCont = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
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
                    var oNominaUs = General.GetEntity<NominaUsuario>(c => c.NominaUsuarioID == iId);
                    iUsuarioID = oNominaUs.IdUsuario;
                    break;
                case Cat.ContaAfectaciones.Pago2Por:
                case Cat.ContaAfectaciones.PagoImss:
                case Cat.ContaAfectaciones.PagoInfonavit:
                    var oImpUsuario = General.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
                    iUsuarioID = oImpUsuario.IdUsuario;
                    break;
            }

            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaDeMayorID && c.RelacionID == iUsuarioID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oUsuario = General.GetEntity<Usuario>(c => c.UsuarioID == iUsuarioID && c.Estatus);
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
                    var oPago = General.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
                    iUsuarioID = oPago.IdUsuario;
                    break;
            }

            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaDeMayorID && c.RelacionID == iUsuarioID);

            // Si no existe la cuenta auxiliar, se crea
            int iCuentaAuxID;
            if (oCuentaAux == null)
            {
                var oUsuario = General.GetEntity<Usuario>(c => c.UsuarioID == iUsuarioID && c.Estatus);
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
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => c.Costo * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConPrecioSinIvaVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => c.PrecioUnitario * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConIvaVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => c.Iva * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConPrecioVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == iId && c.Estatus);
            var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == iId && c.Estatus);
            oDetalle.Cargo = (oVentaDet.Count > 0 ? oVentaDet.Sum(c => (c.PrecioUnitario + c.Iva) * c.Cantidad) : 0);
            oDetalle.Referencia = oVenta.Folio;
        }

        private static void AfectarConCostoFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = General.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = oFactura.Costo;
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConSubtotalFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = General.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = oFactura.Subtotal;
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConIvaFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = General.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = oFactura.Iva;
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFactura = General.GetEntity<VentaFactura>(c => c.VentaFacturaID == iId && c.Estatus);
            oDetalle.Cargo = (oFactura.Subtotal + oFactura.Iva);
            oDetalle.Referencia = (oFactura.Serie + oFactura.Folio);
        }

        private static void AfectarConFacturaGlobalNoValesDeFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFacturaGlobal = General.GetEntity<CajaFacturaGlobal>(c => c.VentaFacturaID == iId);
            oDetalle.Cargo = (oFacturaGlobal.Facturado - oFacturaGlobal.FacturadoVales).Valor();
        }

        private static void AfectarConFacturaGlobalValesDeFactura(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oFacturaGlobal = General.GetEntity<CajaFacturaGlobal>(c => c.VentaFacturaID == iId);
            oDetalle.Cargo = oFacturaGlobal.FacturadoVales.Valor();
        }

        private static void AfectarConPago(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = General.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConIvaPago(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = General.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConPagoVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = General.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId && c.TipoFormaPagoID == Cat.FormasDePago.Vale);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConPagoNoVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = General.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iId && c.TipoFormaPagoID != Cat.FormasDePago.Vale);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConPagoValeDeVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = General.GetListOf<VentasPagosDetalleView>(c => c.VentaID == iId && c.FormaDePagoID == Cat.FormasDePago.Vale);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConPagoNoValeDeVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = General.GetListOf<VentasPagosDetalleView>(c => c.VentaID == iId && c.FormaDePagoID != Cat.FormasDePago.Vale);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }

        private static void AfectarConPagoDeVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagos = General.GetListOf<VentasPagosView>(c => c.VentaID == iId);
            oDetalle.Cargo = (oPagos.Count > 0 ? oPagos.Sum(c => c.Importe) : 0);
        }
        
        private static void AfectarConCostoParteDeMovimientoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            decimal mCosto = 0;
            var oPartesMov = General.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            foreach (var oReg in oPartesMov)
            {
                var oPartePrecio = General.GetEntity<PartePrecio>(c => c.ParteID == oReg.ParteID && c.Estatus);
                mCosto += (oPartePrecio.Costo.Valor() * oReg.Cantidad);
            }
            oDetalle.Cargo = mCosto;
        }

        private static void AfectarConIvaCostoParteDeMovimientoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDeImporte(oDetalle.Cargo);
        }

        private static void AfectarConCostoMasIvaParteDeMovimientoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConCostoParteDeMovimientoInventario(ref oDetalle, iId);
            oDetalle.Cargo = UtilLocal.ObtenerImporteMasIva(oDetalle.Cargo);
        }

        private static void AfectarConCostoInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            var oCompraDet = General.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            oDetalle.Cargo = (oCompraDet.Count > 0 ? oCompraDet.Sum(c => c.PrecioUnitario * c.Cantidad) : 0);
            // oDetalle.Referencia = oCompra.FolioFactura;
        }

        private static void AfectarConIvaInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            var oCompraDet = General.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            decimal mMultIva = (GlobalClass.ConfiguracionGlobal.IVA / 100);
            oDetalle.Cargo = (oCompraDet.Count > 0 ? oCompraDet.Sum(c => (c.PrecioUnitario * mMultIva) * c.Cantidad) : 0);
            // oDetalle.Referencia = oCompra.FolioFactura;
        }

        private static void AfectarConPrecioInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            var oCompraDet = General.GetListOf<MovimientoInventarioDetalle>(c => c.MovimientoInventarioID == iId && c.Estatus);
            decimal mMultIva = (1 + (GlobalClass.ConfiguracionGlobal.IVA / 100));
            oDetalle.Cargo = (oCompraDet.Count > 0 ? oCompraDet.Sum(c => (c.PrecioUnitario * mMultIva) * c.Cantidad) : 0);
            // oDetalle.Referencia = oCompra.FolioFactura;
        }

        private static void AfectarConSubtotalImporteFacturaInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            oDetalle.Cargo = UtilLocal.ObtenerPrecioSinIva(oCompra.ImporteFactura);
        }

        private static void AfectarConIvaImporteFacturaInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oCompra.ImporteFactura);
        }

        private static void AfectarConTotalImporteFacturaInventario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oCompra = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iId && c.Estatus);
            oDetalle.Cargo = oCompra.ImporteFactura;
        }

        private static void AfectarConCostoDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = General.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => c.Costo * c.Cantidad) : 0);
        }

        private static void AfectarConPrecioSinIvaDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = General.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => c.PrecioUnitario * c.Cantidad) : 0);
        }

        private static void AfectarConIvaDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = General.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => c.Iva * c.Cantidad) : 0);
        }

        private static void AfectarConPrecioDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevDet = General.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == iId && c.Estatus);
            oDetalle.Cargo = (oDevDet.Count > 0 ? oDevDet.Sum(c => (c.PrecioUnitario + c.Iva) * c.Cantidad) : 0);
        }

        private static void AfectarConCostoGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            // oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => c.Costo) : 0);
            oDetalle.Cargo = oGarantia.Costo;
        }

        private static void AfectarConPrecioSinIvaGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            // oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => c.PrecioUnitario) : 0);
            oDetalle.Cargo = oGarantia.PrecioUnitario;
        }

        private static void AfectarConIvaGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            // oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => c.Iva) : 0);
            oDetalle.Cargo = oGarantia.Iva;
        }

        private static void AfectarConPrecioGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            // oDetalle.Cargo = (oGarantia.Count > 0 ? oGarantia.Sum(c => (c.PrecioUnitario + c.Iva)) : 0);
            oDetalle.Cargo = (oGarantia.PrecioUnitario + oGarantia.Iva);
        }

        private static void AfectarConCajaEgreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConSubtotalCajaEgreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Subtotal.Valor();
        }

        private static void AfectarConIvaCajaEgreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Iva.Valor();
        }

        private static void AfectarConGastoContable(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConSubtotalGastoContable(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
            oDetalle.Cargo = UtilLocal.ObtenerPrecioSinIva(oReg.Importe);
        }

        private static void AfectarConIvaGastoContable(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iId);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oReg.Importe);
        }

        private static void AfectarConCajaIngreso(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<CajaIngreso>(c => c.CajaIngresoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConImporteNotaDeCreditoFiscal(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == iId);
            oDetalle.Cargo = (oReg.Subtotal + oReg.Iva);
        }

        private static void AfectarConImporteSivIvaNotaDeCreditoFiscal(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == iId);
            oDetalle.Cargo = oReg.Subtotal;
        }

        private static void AfectarConIvaNotaDeCreditoFiscal(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCreditoFiscal>(c => c.NotaDeCreditoFiscalID == iId);
            oDetalle.Cargo = oReg.Iva;
        }

        private static void AfectarConImporteNotaDeCreditoFiscalDetalle(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCreditoFiscalDetalle>(c => c.NotaDeCreditoFiscalDetalleID == iId);
            oDetalle.Cargo = (oReg.Descuento + oReg.IvaDescuento);
        }

        private static void AfectarConImporteSivIvaNotaDeCreditoFiscalDetalle(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCreditoFiscalDetalle>(c => c.NotaDeCreditoFiscalDetalleID == iId);
            oDetalle.Cargo = oReg.Descuento;
        }

        private static void AfectarConIvaNotaDeCreditoFiscalDetalle(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCreditoFiscalDetalle>(c => c.NotaDeCreditoFiscalDetalleID == iId);
            oDetalle.Cargo = oReg.IvaDescuento;
        }

        private static void AfectarConDiferenciaCorteDeCaja(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<CajaEfectivoPorDia>(c => c.CajaEfectivoPorDiaID == iId && c.Estatus);
            oDetalle.Cargo = (oReg.Cierre - oReg.CierreDebeHaber).Valor();
            oDetalle.Cargo *= (oDetalle.Cargo < 0 ? -1 : 1);
        }

        private static void AfectarConPagoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == iId && c.Estatus);
            oDetalle.Cargo = oReg.ImportePago;
        }

        private static void AfectarConIvaPagoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == iId && c.Estatus);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oReg.ImportePago);
        }

        private static void AfectarConSubtotalNotaDeCreditoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iId);
            oDetalle.Cargo = oReg.Subtotal;
        }

        private static void AfectarConIvaNotaDeCreditoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iId);
            oDetalle.Cargo = oReg.Iva;
        }

        private static void AfectarConNotaDeCreditoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iId);
            oDetalle.Cargo = (oReg.Subtotal + oReg.Iva);
        }

        private static void AfectarConAbonoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            // var oAbonos = General.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && (c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoDirecto
            //    || c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoFactura) && c.Estatus);
            // oDetalle.Cargo = (oAbonos.Count > 0 ? oAbonos.Sum(c => c.Importe) : 0);
            var oReg = General.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConSubtotalAbonoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            // var oAbonos = General.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && (c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoDirecto
            //    || c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoFactura) && c.Estatus);
            // oDetalle.Cargo = (oAbonos.Count > 0 ? UtilLocal.ObtenerPrecioSinIva(oAbonos.Sum(c => c.Importe)) : 0);
            var oReg = General.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId);
            oDetalle.Cargo = UtilLocal.ObtenerPrecioSinIva(oReg.Importe);
        }

        private static void AfectarConIvaAbonoProveedor(ref ContaPolizaDetalle oDetalle, int iId)
        {
            // var oAbonos = General.GetListOf<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == iId && (c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoDirecto
            //    || c.OrigenID == Cat.OrigenesPagosAProveedores.DescuentoFactura) && c.Estatus);
            // oDetalle.Cargo = (oAbonos.Count > 0 ? UtilLocal.ObtenerPrecioSinIva(oAbonos.Sum(c => c.Importe)) : 0);
            var oReg = General.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iId);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oReg.Importe);
        }

        private static void AfectarConDevolucionMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDevV = General.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iId);
            oDetalle.Cargo = oDevV.Total.Valor();

            var oPagoNeg = General.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oDevV.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo += (oPagoNeg == null ? 0 : oPagoNeg.Importe);  // Se suma con el pago negativo, por tanto queda como resta
            oDetalle.Cargo = (oDetalle.Cargo < 0 ? 0 : oDetalle.Cargo);
        }

        private static void AfectarConIvaDevolucionMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConDevolucionMenosLoAbonado(ref oDetalle, iId);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConImporteDevueltoDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oDev = General.GetEntity<VentaDevolucion>(c => c.VentaDevolucionID == iId && c.Estatus);
            var oPagoNeg = General.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oDev.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo = (oPagoNeg == null ? 0 : (oPagoNeg.Importe * -1));
        }
        
        private static void AfectarConIvaImporteDevueltoDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConImporteDevueltoDevolucion(ref oDetalle, iId);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConImporteDevueltoGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            var oPagoNeg = General.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oReg.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo = (oPagoNeg == null ? 0 : (oPagoNeg.Importe * -1));
        }

        private static void AfectarConIvaImporteDevueltoGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConImporteDevueltoGarantia(ref oDetalle, iId);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConGarantiaMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            oDetalle.Cargo = (oReg.PrecioUnitario + oReg.Iva);

            var oPagoNeg = General.GetEntity<VentaPagoDetalle>(c => c.VentaPagoDetalleID == oReg.VentaPagoDetalleID && c.Estatus);
            oDetalle.Cargo += (oPagoNeg == null ? 0 : oPagoNeg.Importe);  // Se suma con el pago negativo, por tanto queda como resta
            oDetalle.Cargo = (oDetalle.Cargo < 0 ? 0 : oDetalle.Cargo);
        }

        private static void AfectarConIvaGarantiaMenosLoAbonado(ref ContaPolizaDetalle oDetalle, int iId)
        {
            ContaProc.AfectarConGarantiaMenosLoAbonado(ref oDetalle, iId);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oDetalle.Cargo);
        }

        private static void AfectarConVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iId && c.Estatus);
            oDetalle.Cargo = oReg.Importe;
        }

        private static void AfectarConSubtotalVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iId && c.Estatus);
            oDetalle.Cargo = UtilLocal.ObtenerPrecioSinIva(oReg.Importe);
        }

        private static void AfectarConIvaVale(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == iId && c.Estatus);
            oDetalle.Cargo = UtilLocal.ObtenerIvaDePrecio(oReg.Importe);
        }

        private static void AfectarConMovimientoBancario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iId);
            oDetalle.Cargo = oReg.Importe;
        }
        
        private static void AfectarConCuentaNominaOficial(ref ContaPolizaDetalle oDetalle, int iId, int iCuentaDeMayorID)
        {
            var oNominaUs = General.GetEntity<NominaUsuario>(c => c.NominaUsuarioID == iId);
            var oNominaUsOf = General.GetEntity<NominaUsuarioOficial>(c => c.IdUsuario == oNominaUs.IdUsuario && c.NominaID == oNominaUs.NominaID
                && c.ContaCuentaDeMayorID == iCuentaDeMayorID);
            oDetalle.Cargo = oNominaUsOf.Importe;
        }

        private static void AfectarConNominaOficial(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oNominaUsV = General.GetEntity<NominaUsuariosView>(c => c.NominaUsuarioID == iId);
            oDetalle.Cargo = oNominaUsV.TotalOficial.Valor();
        }

        private static void AfectarConTotalPagoImpuestoUsuario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
            oDetalle.Cargo = oReg.Total;
        }

        private static void AfectarConGastoPagoImpuestoUsuario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
            oDetalle.Cargo = (oReg.Total - oReg.Retenido).Valor();
        }

        private static void AfectarConRetenidoPagoImpuestoUsuario(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NominaImpuestoUsuario>(c => c.NominaImpuestoUsuarioID == iId);
            oDetalle.Cargo = oReg.Retenido.Valor();
        }

        private static void AfectarSucursalConVentaDeDevolucion(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oVentaDev = General.GetEntity<VentaDevolucion>(c => c.VentaDevolucionID == iId && c.Estatus);
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == oVentaDev.VentaID && c.Estatus);
            oDetalle.SucursalID = oVenta.SucursalID;
        }

        private static void AfectarSucursalConVentaDeGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == oGarantia.VentaID && c.Estatus);
            oDetalle.SucursalID = oVenta.SucursalID;
        }

        private static void AfectarSucursalConValeDeGarantia(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iId && c.Estatus);
            oDetalle.SucursalID = oGarantia.SucursalID;
        }

        private static void AfectarSucursalConVentaDeNotaDeCreditoFiscalDetalle(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oReg = General.GetEntity<NotaDeCreditoFiscalDetalle>(c => c.NotaDeCreditoFiscalDetalleID == iId);
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == oReg.VentaID && c.Estatus);
            oDetalle.SucursalID = oVenta.SucursalID;
        }

        private static void AfectarSucursalConVentaDePago(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPago = General.GetEntity<VentaPago>(c => c.VentaPagoID == iId && c.Estatus);
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == oPago.VentaID && c.Estatus);
            oDetalle.SucursalID = oVenta.SucursalID;
        }

        private static void AfectarSucursalConValeDePago(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagoDetV = General.GetEntity<VentasPagosDetalleAvanzadoView>(c => c.VentaPagoID == iId && c.FormaDePagoID == Cat.FormasDePago.Vale);
            if (oPagoDetV != null)
            {
                var oVale = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == oPagoDetV.NotaDeCreditoID && c.Estatus);
                oDetalle.SucursalID = oVale.SucursalID.Valor();
            }
        }

        private static void AfectarSucursalConValeDeVenta(ref ContaPolizaDetalle oDetalle, int iId)
        {
            var oPagoDetV = General.GetEntity<VentasPagosDetalleAvanzadoView>(c => c.VentaID == iId && c.FormaDePagoID == Cat.FormasDePago.Vale);
            if (oPagoDetV != null)
                oDetalle.SucursalID = oPagoDetV.SucursalID.Valor();
        }

        #endregion

        #region [ Bancos ]

        public static void RecalcularSaldoAcumulado(int iBancoCuentaID, DateTime dDesde)
        {
            var oMovAnterior = General.GetListOf<BancosCuentasMovimientosView>(c => c.BancoCuentaID == iBancoCuentaID && c.FechaAsignado < dDesde
                && !c.MovimientoAgrupadorID.HasValue && (c.RelacionTabla == null || c.RelacionTabla != Cat.Tablas.VentaPagoDetalle || c.Resguardado))
                .OrderByDescending(c => c.FechaAsignado).FirstOrDefault();
            decimal mSaldo = (oMovAnterior == null ? 0 : oMovAnterior.SaldoAcumulado);

            var oMovsSig = General.GetListOf<BancoCuentaMovimiento>(c => c.BancoCuentaID == iBancoCuentaID && c.FechaAsignado >= dDesde && !c.MovimientoAgrupadorID.HasValue)
                .OrderBy(c => c.FechaAsignado);
            foreach (var oReg in oMovsSig)
            {
                // Si es un movimiento que proviene de una venta, se busca si ya está resguardado, si no, se ignora
                if (oReg.RelacionTabla == Cat.Tablas.VentaPagoDetalle && !General.Exists<VentaPagoDetalleResguardo>(c => c.VentaPagoDetalleID == oReg.RelacionID))
                    continue;
                //
                decimal mImporte = (oReg.Importe * (oReg.EsIngreso ? 1 : -1));
                oReg.SaldoAcumulado = (mSaldo + mImporte);
                mSaldo = oReg.SaldoAcumulado;
                Guardar.Generico<BancoCuentaMovimiento>(oReg);
            }
        }

        public static void RegistrarMovimientoBancario(BancoCuentaMovimiento oMov)
        {
            // Se llanan algunos datos, si no han sido llenados
            oMov.Fecha = (oMov.Fecha == DateTime.MinValue ? DateTime.Now : oMov.Fecha);
            oMov.SucursalID = (oMov.SucursalID == 0 ? GlobalClass.SucursalID : oMov.SucursalID);

            // Se calcula el nuevo saldo acumulado, si ya está asignado
            if (oMov.FechaAsignado.HasValue)
            {
                ContaProc.RecalcularSaldoAcumulado(oMov.BancoCuentaID.Valor(), oMov.FechaAsignado.Value);
            }

            Guardar.Generico<BancoCuentaMovimiento>(oMov);
        }

        public static void EliminarMovimientoBancario(int iBancoCuentaMovimientoID)
        {
            var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iBancoCuentaMovimientoID);
            Guardar.Eliminar<BancoCuentaMovimiento>(oMov);

            // Se calcula el nuevo saldo acumulado, si ya estaba asignado
            if (oMov.FechaAsignado.HasValue)
                ContaProc.RecalcularSaldoAcumulado(oMov.BancoCuentaID.Valor(), oMov.FechaAsignado.Value);
        }

        public static void AsignarMovimientoBancario(int iBancoCuentaMovimientoID, int iBancoCuentaID)
        {
            var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iBancoCuentaMovimientoID);
            oMov.BancoCuentaID = iBancoCuentaID;
            oMov.FechaAsignado = DateTime.Now;
            // Se calcula el acumulado
            var oMovAnt = General.GetListOf<BancoCuentaMovimiento>(c => c.BancoCuentaID == oMov.BancoCuentaID && c.FechaAsignado.HasValue)
                .OrderByDescending(c => c.FechaAsignado).FirstOrDefault();
            decimal mSaldo = (oMovAnt == null ? 0 : oMovAnt.SaldoAcumulado);
            decimal mImporte = (oMov.Importe * (oMov.EsIngreso ? 1 : -1));
            oMov.SaldoAcumulado = (mSaldo + mImporte);
            // Se guarda el movimiento
            Guardar.Generico<BancoCuentaMovimiento>(oMov);
        }

        public static bool DesasignarMovimientoBancario(int iBancoCuentaMovimientoID)
        {
            var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iBancoCuentaMovimientoID);
            // Se verifica que no esté conciliado
            if (oMov.Conciliado)
            {
                UtilLocal.MensajeAdvertencia("El movimiento seleccionado ya fue conciliado. No se puede continuar.");
                return false;
            }
            // Se verifica que no sea un movimiento agrupador
            if (General.Exists<BancoCuentaMovimiento>(c => c.MovimientoAgrupadorID == iBancoCuentaMovimientoID))
            {
                UtilLocal.MensajeAdvertencia("El movimiento seleccionado es un movimiento agrupador. No se puede continuar.");
                return false;
            }
            // 
            var oMovAnt = General.GetListOf<BancoCuentaMovimiento>(c => c.BancoCuentaID == oMov.BancoCuentaID && c.FechaAsignado.HasValue
                && c.FechaAsignado < oMov.FechaAsignado).OrderByDescending(c => c.FechaAsignado).FirstOrDefault();
            // Se desasigna
            oMov.BancoCuentaID = null;
            oMov.FechaAsignado = null;
            Guardar.Generico<BancoCuentaMovimiento>(oMov);
            // Se recalcula el acumulado
            ContaProc.RecalcularSaldoAcumulado(oMovAnt.BancoCuentaID.Valor(), oMovAnt.FechaAsignado.Valor());
            //
            return true;
        }

        #endregion
    }
}
