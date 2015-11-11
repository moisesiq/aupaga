using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.IO;
using System.Text;
using FastReport;
using FastReport.Export;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public class VentasCaja : VentaOpcion
    {
        public CajaGeneral ctlCajaGeneral;
        public CajaOpciones ctlOpciones;

        protected override string AccesosDeTeclado { get { return "F6 - Actualizar datos | F9 - Ventas por cobrar"; } }

        public VentasCaja(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
        }

        #region [ Base ]

        public override void Inicializar()
        {
            base.Inicializar();

            // Se crean los objetos necesarios
            this.ctlCajaGeneral = new CajaGeneral();
            this.ctlCajaGeneral.oVentasCaja = this;
            this.ctlOpciones = new CajaOpciones();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlOpciones.ctlGeneral = this.ctlCajaGeneral;

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnContenido.Controls.Add(this.ctlCajaGeneral);
            this.pnlEnEquivalentes.Controls.Add(this.ctlOpciones);
        }

        public override bool Ejecutar()
        {
            // Se verifica si ya se hizo el cierre de caja
            if (UtilDatos.VerCierreDeDaja())
            {
                UtilLocal.MensajeAdvertencia("Ya se hizo el Corte de Caja. No se puede continuar.");
                return false;
            }

            // Se ejecuta cierta acción dependiendo del tab seleccionado
            bool bExito = false, bCierre = false;
            switch (this.ctlCajaGeneral.tabOpciones.SelectedTab.Name)
            {
                case "tbpFondoDeCaja":
                    bExito = this.FondoDeCaja();
                    break;
                case "tbpGastos":
                    bExito = this.Gastos();
                    break;
                case "tbpRefuerzo":
                    bExito = this.Refuerzo();
                    break;
                case "tbpResguardo":
                    bExito = this.Resguardo();
                    break;
                case "tbpVentasPorCobrar":
                    bExito = this.VentasPorCobrar();
                    break;
                case "tbpVentasCambios":
                    bExito = this.VentasCambios();
                    break;
                case "tbpCambioTurno":
                    bExito = this.CambioTurno();
                    break;
                case "tbpCorte":
                    bExito = this.Corte();
                    bCierre = bExito;
                    break;
            }

            if (!bExito)
                return false;
                        
            // Si se llega a este punto, la operación fue exitosa pero no se sale de la opción de caja, al menos que se haya realizado el cierre de caja
            if (bCierre)
            {
                this.Limpiar();
                return true;
            }
            else
            {
                // Si no se hizo el cierre, se permanece en caja y se actualiza la opción actual
                this.ctlCajaGeneral.ActualizarCaja();
            }

            return false;
        }

        protected override void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;

            this.Cliente = this.oVenta.Cliente;
            // this.ctlBusqueda.CambiarCliente(this.Cliente);
        }

        #endregion

        #region [ Opciones ]

        private bool FondoDeCaja()
        {
            // Se verifica si ya se hizo el fondo
            CajaFondo oFondo = this.ctlCajaGeneral.ctlFondeDeCaja;
            if (oFondo.FondoRealizado)
            {
                UtilLocal.MensajeInformacion("El Fondo de Caja del día de hoy ya se realizó.");
                return false;
            }

            // Se confirma la operación
            string sMensaje = string.Format("¿Estás seguro que deseas guardar el Fondo de Caja con un importe de {0}?", oFondo.Conteo.ToString(GlobalClass.FormatoMoneda));
            if (oFondo.Diferencia != 0)
                sMensaje += string.Format("\n\nToma en cuenta que hay una diferencia de {0} y por tanto se requerirá una autorización para continuar.", 
                    oFondo.Diferencia.ToString(GlobalClass.FormatoMoneda));

            if (UtilLocal.MensajePregunta(sMensaje) != DialogResult.Yes)
                return false;

            // Se solicita la validación del usuario
            var Res = UtilDatos.ValidarObtenerUsuario("Ventas.FondoDeCaja.Agregar");
            if (Res.Error)
                return false;
            var oUsuario = Res.Respuesta;

            // Se solicita la validación de autorización, si aplica
            Usuario oAutorizo = null;
            if (oFondo.Diferencia != 0)
            {
                Res = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.FondoDeCaja.Diferencia", "Autorización");
                //if (Res.Exito)
                    oAutorizo = Res.Respuesta;
            }

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se guarda el dato de inicio
            var oEfectivo = new CajaEfectivoPorDia()
            {
                SucursalID = GlobalClass.SucursalID,
                Dia = dAhora,
                Inicio = oFondo.Conteo,
                InicioUsuarioID = oUsuario.UsuarioID
            };
            Guardar.Generico<CajaEfectivoPorDia>(oEfectivo);

            // Se registra la póliza de la diferencia, si hubo
            if (oFondo.Diferencia != 0)
            {
                if (oFondo.Diferencia > 0)
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, "DIFERENCIA EN FONDO DE CAJA", Cat.ContaCuentasAuxiliares.Caja, Cat.ContaCuentasAuxiliares.FondoDeCaja
                        , oFondo.Diferencia, DateTime.Now.ToShortDateString(), Cat.Tablas.CajaEfectivoPorDia, oEfectivo.CajaEfectivoPorDiaID);
                else
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, "DIFERENCIA EN FONDO DE CAJA", Cat.ContaCuentasAuxiliares.FondoDeCaja, Cat.ContaCuentasAuxiliares.Caja
                        , (oFondo.Diferencia * -1), DateTime.Now.ToShortDateString(), Cat.Tablas.CajaEfectivoPorDia, oEfectivo.CajaEfectivoPorDiaID);
            }

            // Se genera y guarda la autorización, si aplica
            if (oFondo.Diferencia != 0)
            {
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.FondoDeCajaDiferencia, Cat.Tablas.CajaEfectivoPorDia, oEfectivo.CajaEfectivoPorDiaID
                    , (oAutorizo == null ? 0 : oAutorizo.UsuarioID));
            }

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        private bool Gastos()
        {
            CajaGastos oGastos = this.ctlCajaGeneral.ctlGastos;

            // Se obtiene la lista de los cambios necesarios
            var oIngresos = oGastos.GenerarListaDeIngresos();
            var oEgresos = oGastos.GenerarListaDeEgresos();
            var oIngresosBorrados = oGastos.GenerarListaDeIngresosBorrados();
            var oEgresosBorrados = oGastos.GenerarListaDeEgresosBorrados();
            
            // Se verifica si hay cambios necesarios
            bool bCambios = (oIngresos.Count > 0 || oEgresos.Count > 0 || oIngresosBorrados.Count > 0 || oEgresosBorrados.Count > 0);
            if (!bCambios)
                return false;

            // Se solicita la validación del usuario
            var oResUs = UtilDatos.ValidarObtenerUsuario(new List<string>() { "Ventas.Ingresos.Agregar", "Ventas.Ingresos.Borrar", 
                "Ventas.Egresos.Agregar", "Ventas.Egresos.Borrar" }, true);
            if (oResUs.Error)
                return false;
            int iRealizoUsuarioID = oResUs.Respuesta.UsuarioID;
            
            // Se solicita la autorización
            var ResAut = UtilDatos.ValidarObtenerUsuario(new List<string>() { "Autorizaciones.Ventas.Ingresos.Agregar", "Autorizaciones.Ventas.Ingresos.Borrar", 
                "Autorizaciones.Ventas.Egresos.Agregar", "Autorizaciones.Ventas.Egresos.Borrar" }, true, "Autorización");
            int iAutorizoID = (ResAut.Respuesta == null ? 0 : ResAut.Respuesta.UsuarioID);

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se guardan los Ingresos
            foreach (var oMov in oIngresos)
            {
                oMov.RealizoUsuarioID = iRealizoUsuarioID;
                Guardar.Generico<CajaIngreso>(oMov);

                // Si es un refuerzo, se elimina la póliza correspondiente
                if (oMov.CajaTipoIngresoID == Cat.CajaTiposDeIngreso.Refuerzo)
                {
                    var oPoliza = General.GetEntity<ContaPoliza>(c => c.RelacionTabla == Cat.Tablas.CajaIngreso && c.RelacionID == oMov.CajaIngresoID);
                    ContaProc.BorrarPoliza(oPoliza.ContaPolizaID);
                }

                // Se genera la autorización
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.OtrosIngresos, Cat.Tablas.CajaIngreso, oMov.CajaIngresoID, iAutorizoID);
            }
            // Se guardan los ingresos borrados
            foreach (var oMov in oIngresosBorrados)
            {
                oMov.Estatus = false;
                Guardar.Generico<CajaIngreso>(oMov);
                // Se genera la autorización
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.OtrosIngresosBorrar, Cat.Tablas.CajaIngreso, oMov.CajaIngresoID, iAutorizoID);
            }

            // Se guardan los Egresos
            bool bGastoCaja = false;
            var oGastosCaja = new List<int>();
            foreach (var oMov in oEgresos)
            {
                // Referente a lo contable
                
                // Validación de importe máximo (5000) para gastos de sueldos de Isidro padre e hijo (durazo)
                int iCuentaAuxiliarID = Helper.ConvertirEntero(oMov["CategoriaID"]);
                if (iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.SueldoIsidroPadre || iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.SueldoIsidroHijo)
                {
                    var oFechas = UtilDatos.FechasDeComisiones(DateTime.Now);
                    var oContaEgresos = General.GetListOf<ContaEgreso>(c => c.ContaCuentaAuxiliarID == iCuentaAuxiliarID
                        && (c.Fecha >= oFechas.Valor1 && c.Fecha <= oFechas.Valor2));
                    if (oContaEgresos.Count > 0 && oContaEgresos.Sum(c => c.Importe) >= 5000)
                        iCuentaAuxiliarID = (iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.SueldoIsidroPadre
                            ? Cat.ContaCuentasAuxiliares.UtilidadesIsidroPadre : Cat.ContaCuentasAuxiliares.UtilidadesIsidroHijo);
                }
                //

                // Se genera el Gasto Contable
                var oGastoConta = new ContaEgreso()
                {
                    ContaCuentaAuxiliarID = iCuentaAuxiliarID,
                    Fecha = dAhora,
                    Importe = Helper.ConvertirDecimal(oMov["Importe"]),
                    TipoFormaPagoID = Cat.FormasDePago.Efectivo,
                    Observaciones = Helper.ConvertirCadena(oMov["Concepto"]),
                    RealizoUsuarioID = iRealizoUsuarioID,
                    SucursalID = Helper.ConvertirEntero(oMov["SucursalID"])
                };
                Guardar.Generico<ContaEgreso>(oGastoConta);
                // Se manda devengar automáticamente, si aplica
                ContaProc.GastoVerDevengarAutomaticamente(oGastoConta);
                // Se genera el gasto de caja (CajaEgreso)
                var oEgreso = new CajaEgreso()
                {
                    CajaTipoEgresoID = Cat.CajaTiposDeEgreso.CuentaAuxiliar,
                    Concepto = oGastoConta.Observaciones,
                    Importe = oGastoConta.Importe,
                    Fecha = dAhora,
                    SucursalID = oGastoConta.SucursalID,
                    RealizoUsuarioID = iRealizoUsuarioID,
                    ContaEgresoID = oGastoConta.ContaEgresoID
                };
                Guardar.Generico<CajaEgreso>(oEgreso);
                
                // Si es un gasto de la cuenta deudores diversos, se genera la póliza correspondiente (AfeConta)
                if (General.Exists<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == iCuentaAuxiliarID && c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.DeudoresDiversos))
                {
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "PRÉSTAMO", iCuentaAuxiliarID, Cat.ContaCuentasAuxiliares.Caja, oGastoConta.Importe
                        , oResUs.Respuesta.NombreUsuario, Cat.Tablas.CajaEgreso, oEgreso.CajaEgresoID);
                    // Se marca el gasto como ya afectado en pólizas, para que no aparezca en gastos pendientes
                    oEgreso.AfectadoEnPolizas = true;
                    Guardar.Generico<CajaEgreso>(oEgreso);
                }

                // Se verifica si es un gasto de casco, para afectar la existencia
                if (iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoChico || iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoMediano
                    || iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoGrande || iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoExtragrande)
                {
                    int iCascoParteID = 0;
                    switch (iCuentaAuxiliarID)
                    {
                        case Cat.ContaCuentasAuxiliares.CascoChico: iCascoParteID = Cat.Partes.DepositoCascoChico; break;
                        case Cat.ContaCuentasAuxiliares.CascoMediano: iCascoParteID = Cat.Partes.DepositoCascoMediano; break;
                        case Cat.ContaCuentasAuxiliares.CascoGrande: iCascoParteID = Cat.Partes.DepositoCascoGrande; break;
                        case Cat.ContaCuentasAuxiliares.CascoExtragrande: iCascoParteID = Cat.Partes.DepositoCascoExtragrande; break;
                    }
                    var oSucursal = General.GetEntity<Sucursal>(c => c.SucursalID == GlobalClass.SucursalID && c.Estatus);
                    AdmonProc.AfectarExistenciaYKardex(iCascoParteID, GlobalClass.SucursalID, Cat.OperacionesKardex.EntradaInventario, oEgreso.CajaEgresoID.ToString()
                        , iRealizoUsuarioID, "----", "Gasto por Casco", oSucursal.NombreSucursal, 1, oGastoConta.Importe, Cat.Tablas.CajaEgreso, oEgreso.CajaEgresoID);
                }

                // Se genera la autorización
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.Gastos, Cat.Tablas.CajaEgreso, oEgreso.CajaEgresoID, iAutorizoID);

                // Bandera para saber si se realizó un gasto
                oGastosCaja.Add(oEgreso.CajaEgresoID);
                bGastoCaja = true;
            }
            // Se guardan los egresos borrados
            foreach (var oMov in oEgresosBorrados)
            {
                // Se guarda el borrado del gasto de caja (CajaEgreso)
                int iEgresoID = oMov.ContaEgresoID.Valor();
                Guardar.Eliminar<CajaEgreso>(oMov);  // Ahora se borra definitivamente, pues el gasto contable no tiene campo de "Estatus"
                // Se guarda el borrado del Gasto Contable, si hubiera
                int iCuentaAuxiliarID = 0;
                if (iEgresoID > 0)
                {
                    var oContaEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iEgresoID);
                    iCuentaAuxiliarID = oContaEgreso.ContaCuentaAuxiliarID;
                    ContaProc.GastoEliminar(iEgresoID);
                }

                // Si es un resguardo, se elimina la póliza correspondiente
                if (oMov.CajaTipoEgresoID == Cat.CajaTiposDeEgreso.Resguardo)
                {
                    var oPoliza = General.GetEntity<ContaPoliza>(c => c.RelacionTabla == Cat.Tablas.CajaEgreso && c.RelacionID == oMov.CajaEgresoID);
                    ContaProc.BorrarPoliza(oPoliza.ContaPolizaID);
                }
                // Si es un gasto de la cuenta deudores diversos, se verifica y elimina la póliza (AfeConta)
                if (General.Exists<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == iCuentaAuxiliarID && c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.DeudoresDiversos))
                {
                    var oPoliza = General.GetEntity<ContaPoliza>(c => c.RelacionTabla == Cat.Tablas.CajaEgreso && c.RelacionID == oMov.CajaEgresoID);
                    ContaProc.BorrarPoliza(oPoliza.ContaPolizaID);
                }

                // Se verifica si es un gasto de casco, para afectar la existencia
                if (iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoChico || iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoMediano
                    || iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoGrande || iCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CascoExtragrande)
                {
                    int iCascoParteID = 0;
                    switch (iCuentaAuxiliarID)
                    {
                        case Cat.ContaCuentasAuxiliares.CascoChico: iCascoParteID = Cat.Partes.DepositoCascoChico; break;
                        case Cat.ContaCuentasAuxiliares.CascoMediano: iCascoParteID = Cat.Partes.DepositoCascoMediano; break;
                        case Cat.ContaCuentasAuxiliares.CascoGrande: iCascoParteID = Cat.Partes.DepositoCascoGrande; break;
                        case Cat.ContaCuentasAuxiliares.CascoExtragrande: iCascoParteID = Cat.Partes.DepositoCascoExtragrande; break;
                    }
                    var oSucursal = General.GetEntity<Sucursal>(c => c.SucursalID == GlobalClass.SucursalID && c.Estatus);
                    AdmonProc.AfectarExistenciaYKardex(iCascoParteID, GlobalClass.SucursalID, Cat.OperacionesKardex.SalidaInventario, oMov.CajaEgresoID.ToString()
                        , iRealizoUsuarioID, "----", "Gasto por Casco", oSucursal.NombreSucursal, 1, oMov.Importe, Cat.Tablas.CajaEgreso, oMov.CajaEgresoID);
                }
                // Se genera la autorización
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.GastosBorrar, Cat.Tablas.CajaEgreso, oMov.CajaEgresoID, iAutorizoID);
            }
            
            // Se pregunta si se debe mostrar el ticket, cuando hubo gastos
            if (bGastoCaja)
            {
                if (UtilLocal.MensajePregunta("¿Deseas imprimir los Tickets de Gasto correspondientes?") == DialogResult.Yes)
                {
                    foreach (int iEgresoID in oGastosCaja)
                        VentasProc.GenerarTicketGasto(iEgresoID);
                }
            }

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        private bool Refuerzo()
        {
            // Se valida
            CajaRefuerzo oRefuerzo = this.ctlCajaGeneral.ctlRefuerzo;
            if (!oRefuerzo.Validar())
                return false;

            // Se confirma la operación
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas hacer el Refuerzo de " + oRefuerzo.MonedasImporteTotal.ToString(GlobalClass.FormatoMoneda))
                != DialogResult.Yes)
                return false;

            // Se solicita la validación del usuario
            var Res = UtilDatos.ValidarObtenerUsuario("Ventas.Refuerzo.Agregar");
            if (Res.Error)
                return false;
            var oUsuario = Res.Respuesta;

            // Se solicita la validación de autorización
            Res = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.Refuerzo.Agregar", "Autorización");
            var oAutorizo = Res.Respuesta;
            
            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se guarda el refuerzo (como un ingreso)
            var oIngreso = new CajaIngreso()
            {
                CajaTipoIngresoID = Cat.CajaTiposDeIngreso.Refuerzo,
                Importe = oRefuerzo.MonedasImporteTotal,
                Fecha = dAhora,
                SucursalID = GlobalClass.SucursalID,
                Concepto = "REFUERZO",
                RealizoUsuarioID = oUsuario.UsuarioID
            };
            Guardar.Generico<CajaIngreso>(oIngreso);

            // Se crea la Poliza correspondiente (AfeConta)
            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.Refuerzo, oIngreso.CajaIngresoID, oUsuario.NombreUsuario, "REFUERZO");

            // Se guarda la autorización
            VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.Refuerzo, Cat.Tablas.CajaIngreso, oIngreso.CajaIngresoID
                , (oAutorizo == null ? 0 : oAutorizo.UsuarioID));

            // Se genera el ticket
            var oConteo = oRefuerzo.GenerarConteo();
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CajaRefuerzoTicket.frx");
            oRep.RegisterData(new List<ConteoCaja>() { oConteo }, "Conteo");
            UtilLocal.EnviarReporteASalida("Reportes.CajaRefuerzoTicket.Salida", oRep);

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        private bool Resguardo()
        {
            // Se valida
            CajaResguardo oResguardo = (this.ctlCajaGeneral.ctlResguardo);
            if (!oResguardo.Validar())
                return false;

            // Se confirma la operación
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas hacer el Resguardo de " + oResguardo.Total.ToString(GlobalClass.FormatoMoneda))
                != DialogResult.Yes)
                return false;

            // Se solicita la validación del usuario
            var Res = UtilDatos.ValidarObtenerUsuario("Ventas.Resguardo.Agregar");
            if (Res.Error)
                return false;
            var oUsuario = Res.Respuesta;

            // Se solicita la validación de autorización
            Res = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.Resguardo.Agregar", "Autorización");
            var oAutorizo = Res.Respuesta;

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se guarda el Resguardo (como un Egreso)
            var oEgreso = new CajaEgreso()
            {
                CajaTipoEgresoID = Cat.CajaTiposDeEgreso.Resguardo,
                Importe = oResguardo.Total,
                Fecha = dAhora,
                SucursalID = GlobalClass.SucursalID,
                Concepto = "RESGUARDO",
                RealizoUsuarioID = oUsuario.UsuarioID
            };
            Guardar.Generico<CajaEgreso>(oEgreso);

            // Se registran los pagos bancarios resguardados
            var oPagosBan = oResguardo.GenerarPagosBancariosResguardados();
            foreach (var oPagoBan in oPagosBan)
                Guardar.Generico<VentaPagoDetalleResguardo>(oPagoBan);

            // Se crea la Poliza correspondiente (AfeConta)
            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.Resguardo, oEgreso.CajaEgresoID, oUsuario.NombreUsuario, "RESGUARDO");

            // Se guarda la autorización
            VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.Resguardo, Cat.Tablas.CajaEgreso, oEgreso.CajaEgresoID
                , (oAutorizo == null ? 0 : oAutorizo.UsuarioID));

            // Se genera el ticket
            var oConteo = oResguardo.GenerarConteo();
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CajaResguardoTicket.frx");
            oRep.RegisterData(new List<ConteoCaja>() { oConteo }, "Conteo");
            UtilLocal.EnviarReporteASalida("Reportes.CajaResguardoTicket.Salida", oRep);

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        private bool VentasPorCobrar()
        {
            // Se valida que haya una venta seleccionada
            CajaVentasPorCobrar oPorCobrar = this.ctlCajaGeneral.ctlVentasPorCobrar;
            int iVentaID = oPorCobrar.VentaID;
            if (iVentaID <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna venta seleccionada.");
                return false;
            }
                        
            // Se verifica si se debe cobrar o mostrar la pantalla de cobro
            var oCliente = General.GetEntity<Cliente>(q => q.ClienteID == oPorCobrar.ClienteID && q.Estatus);
            if (Helper.ControlAlFrente(oPorCobrar.pnlParaDetalle) == oPorCobrar.ctlDetalle)
            {
                oPorCobrar.ctlCobro.LlenarDatosGenerales(oPorCobrar.VentaID);
                oPorCobrar.ctlCobro.Total = oPorCobrar.ImporteVenta;
                oPorCobrar.ctlCobro.EstablecerFormaDePagoPredeterminada(oCliente.TipoFormaPagoID.Valor(),
                    oPorCobrar.ctlCobro.Total, oCliente.BancoID.Valor(), oCliente.CuentaBancaria);
                oPorCobrar.ctlCobro.Leyenda = VentasProc.ObtenerLeyenda(iVentaID);
                oPorCobrar.ctlCobro.BringToFront();
                return false;
            }
            else
            {
                // Se pide el efectivo, si aplica
                if (!oPorCobrar.ctlCobro.CompletarCobro())
                    return false;                
            }
            
            // Si la venta es a crédito y el cliente tiene personal, se muestran las firmas
            if (oPorCobrar.ctlCobro.ACredito)
            {
                var personal = General.GetListOf<ClientePersonal>(c => c.ClienteID == oPorCobrar.ClienteID );
                if (personal.Count > 0)
                {
                    var frmMostrarFirmas = new DetalleMostrarFirmas(oPorCobrar.ClienteID);
                    frmMostrarFirmas.ShowDialog(Principal.Instance);
                    if (frmMostrarFirmas.DialogResult != DialogResult.OK)
                        return false;
                    
                    //Helper.MensajeInformacion("Revisar la firma", "");
                }
            }
            
            // Se pide la orden de compra, si aplica
            var oVenta = General.GetEntity<Venta>(q => q.VentaID == oPorCobrar.VentaID && q.Estatus);
            if (oPorCobrar.ctlCobro.ACredito && oCliente.SiempreVale.Valor())
            {
                var oOrdenDeCompra = UtilLocal.ObtenerValor("Indica la Orden de Compra:", "", MensajeObtenerValor.Tipo.TextoLargo);
                if (oOrdenDeCompra == null)
                    return false;
                oVenta.OrdenDeCompra = Helper.ConvertirCadena(oOrdenDeCompra);
            }

            // Si hay que facturar, se pregunta si se debe facturar al mismo cliente de las ventas o a otro
            int iAFClienteID = oPorCobrar.ClienteID;
            if (oPorCobrar.ctlCobro.Facturar)
            {
                // Se valida que la venta sólo tenga una artículo, en caso de que se quiera dividir
                if (oPorCobrar.ctlCobro.DividirFactura) {
                    if (oPorCobrar.ctlDetalle.ObtenerListaVenta().Count > 1) {
                        UtilLocal.MensajeAdvertencia("No se puede dividir la factura si la venta tiene más de un artículo.");
                        return false;
                    }
                }

                // Se obtiene el cliente al cual se va a facturar
                if (UtilLocal.MensajePregunta("¿Deseas hacer la factura a nombre del cliente que realizó la compra?") == DialogResult.No)
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

                // Se muestra la ventana de "Cargando.."
                Cargando.Mostrar();

                // Se validan los datos para realizar la facturación
                var ResVal = VentasProc.ValidarDatosParaFactura(iVentaID, iAFClienteID);

                // Se cierra la ventana de "Cargando.."
                Cargando.Cerrar();

                //
                if (!ResVal.Respuesta)
                {
                    UtilLocal.MensajeAdvertencia("Hubo un error al validar la factura.\n\n" + ResVal.Mensaje);
                    return false;
                }
            }

            // Se verifica si la venta es de un anticipo de 9500
            if (General.Exists<Cotizacion9500>(c => c.AnticipoVentaID == iVentaID && c.Estatus))
            {
                VentasProc.NotificarCreacion9500();
            }
            // Se verifica si la venta pertenece a un 9500, para tomar las acciones necesarias
            var o9500 = General.GetEntity<Cotizacion9500>(q => q.VentaID == iVentaID && q.Estatus);
            bool b9500 = (o9500 != null);
            bool bDevolverEfectivo = false;
            decimal mSobrante = 0;
            if (b9500)
            {
                var oVentaV9500 = General.GetEntity<VentasView>(q => q.VentaID == o9500.VentaID);
                mSobrante = (o9500.Anticipo - oVentaV9500.Total);

                // Si el total es menor que el anticipo, se pregunta por la acción a tomar
                if (mSobrante > 0)
                {
                    var Res9500 = MessageBox.Show("El importe total es menor al anticipo que dejó el Cliente. ¿Deseas generar una notá de crédito por la diferencia?\n\n"
                        + " Si seleccionas \"No\", deberás devolver el efectivo al Cliente.", "Saldo a favor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (Res9500 == DialogResult.Cancel)
                        return false;
                    bDevolverEfectivo = (Res9500 == DialogResult.No);
                }
            }

            // Se verifica si la venta ya fue cobrada
            if (oVenta.VentaEstatusID != Cat.VentasEstatus.Realizada)
            {
                UtilLocal.MensajeAdvertencia("La venta seleccionada ya fue cobrada o ya no es posible realizar el cobro.");
                oPorCobrar.ActualizarDatos();
                return false;
            }

            // Se solicita y la validación de autorización, si aplica
            int iAutorizoID = 0;
            if (oPorCobrar.ctlCobro.AutorizacionDeCreditoRequerida || oPorCobrar.ctlCobro.AutorizacionDeNotasDeCreditoRequerida)
            {
                // Aquí falta considerar que las dos autorizaciones pueden ser requeridas, se debería pedir dos veces la autorización, para validar en cada caso
                string sPermiso = (oPorCobrar.ctlCobro.AutorizacionDeCreditoRequerida ? "Autorizaciones.Ventas.Cobro.CreditoNoAplicable" :
                    "Autorizaciones.Ventas.Cobro.NotaDeCreditoOtroCliente");
                var Res = UtilDatos.ValidarObtenerUsuario(sPermiso, "Autorización");
                iAutorizoID = (Res.Exito ? Res.Respuesta.UsuarioID : 0);
            }

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se cambia el estatus de la venta
            oVenta.VentaEstatusID = Cat.VentasEstatus.Cobrada;
            Guardar.Generico<Venta>(oVenta);

            // Si la venta es a crédito, se guarda el dato
            // si no, se generan los datos del pago
            if (oPorCobrar.ctlCobro.ACredito)
            {
                oVenta.ACredito = true;
                Guardar.Generico<Venta>(oVenta);
            }
            else
            {
                var oPago = new VentaPago()
                {
                    VentaID = oPorCobrar.VentaID,
                    Fecha = dAhora,
                };
                var oPagoDetalle = oPorCobrar.ctlCobro.GenerarPagoDetalle();
                // Se mandan guardar los datos del pago
                Guardar.VentaPago(oPago, oPagoDetalle);
                // Se actualiza la venta, pues pudo haber cambiado en el proceso anterior
                oVenta = General.GetEntity<Venta>(q => q.VentaID == oPorCobrar.VentaID && q.Estatus);
            }
            // Se muestra la ventana de "Cargando.."
            Cargando.Mostrar();
            // Si es de un 9500, se completa el 9500
            if (b9500)
                VentasProc.Completar9500(o9500, mSobrante, bDevolverEfectivo);

            // Se guardan la autorizaciones aplicables
            if (oPorCobrar.ctlCobro.AutorizacionDeCreditoRequerida)
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.CreditoNoAplicable, Cat.Tablas.Venta, oPorCobrar.VentaID, iAutorizoID);
            if (oPorCobrar.ctlCobro.AutorizacionDeNotasDeCreditoRequerida)
            {
                // Se agrega una autorización por cada nota de otro cliente
                var oNotasOC = oPorCobrar.ctlCobro.NotasDeCreditoOtrosClientes();
                foreach (var oNotaOC in oNotasOC)
                    VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.NotaDeCreditoOtroClienteUsar, Cat.Tablas.NotaDeCredito, oNotaOC, iAutorizoID);
            }

            // Se realiza la facturación, si aplica
            ResAcc<int> ResFactura = null;
            bool bGenerarFolio = true;
            if (oPorCobrar.ctlCobro.Facturar)
            {
                string sFormaDePago = oPorCobrar.ctlCobro.FormaDePagoLibre;
                if (sFormaDePago == "")
                    sFormaDePago = VentasProc.GenerarFormaDePago(oPorCobrar.ctlCobro.GenerarPagoDetalle());

                // Se verifica si se debe dividir la factura
                var oProductos = oPorCobrar.ctlDetalle.ObtenerListaVenta();
                if (oPorCobrar.ctlCobro.DividirFactura)
                {
                    decimal mImporteMax = Helper.ConvertirDecimal(Config.Valor("Facturacion.ImporteDividir"));
                    if (oPorCobrar.ImporteVenta > mImporteMax)
                    {
                        decimal mImporteFac = (mImporteMax - 60);
                        decimal mRestante = oPorCobrar.ImporteVenta;
                        int iFactura = 0;
                        string sDescripcion = oProductos[0].NombreDeParte;
                        while (mRestante > 0)
                        {
                            if (mRestante < mImporteFac)
                                mImporteFac = mRestante;
                            // Se ajustan los datos del producto
                            oProductos[0].PrecioUnitario = UtilLocal.ObtenerPrecioSinIva(mImporteFac);
                            oProductos[0].Iva = UtilLocal.ObtenerIvaDePrecio(mImporteFac);
                            if (++iFactura > 1)
                                oProductos[0].NombreDeParte = string.Format("{0}\n(Complemento {1})", sDescripcion, (iFactura - 1));
                            // Se manda hacer la factura
                            ResFactura = VentasProc.GenerarFacturaElectronica(new List<int>() { iVentaID }, iAFClienteID, oProductos, sFormaDePago, "");
                            if (ResFactura.Error)
                                break;
                            //
                            mRestante -= mImporteFac;
                        }

                        // Se marca la venta como facturas divididas
                        oVenta = General.GetEntity<Venta>(q => q.VentaID == oPorCobrar.VentaID && q.Estatus);
                        oVenta.FacturaDividida = true;
                        Guardar.Generico<Venta>(oVenta);
                    }
                }
                
                if (ResFactura == null)
                    ResFactura = VentasProc.GenerarFacturaElectronica(new List<int>() { iVentaID }, iAFClienteID, oProductos, sFormaDePago, "");

                // Se obtiene la forma de pago
                /* if (oPorCobrar.ctlCobro.FormaDePagoLibre == "")
                {
                    ResFactura = VentasProc.GenerarFacturaElectronica(iVentaID, iAFClienteID
                        , oPorCobrar.ctlDetalle.ObtenerListaVenta(), oPorCobrar.ctlCobro.GenerarPagoDetalle(), "");
                }
                else
                {
                    ResFactura = VentasProc.GenerarFacturaElectronica(new List<int>() { iVentaID }, iAFClienteID
                        , oPorCobrar.ctlDetalle.ObtenerListaVenta(), oPorCobrar.ctlCobro.FormaDePagoLibre, "");
                }

                // Se manda hacer la factura electrónica
                if (oVenta.ACredito)
                    ResFactura = VentasProc.GenerarFacturaElectronica(iVentaID, iAFClienteID
                        , oPorCobrar.ctlDetalle.ObtenerListaVenta(), oPorCobrar.ctlCobro.GenerarPagoDetalle(), "");
                else
                    ResFactura = VentasProc.GenerarFacturaElectronica(iVentaID, iAFClienteID, oPorCobrar.ctlDetalle.ObtenerListaVenta(), null, "");
                */

                if (ResFactura.Exito)
                {
                    // Se escribe el folio en la venta
                    /* Siempre no, se hace dentro del procedimiento "GenerarFacturaElectronica"
                    var oFactura = General.GetEntity<VentaFactura>(q => q.VentaFacturaID == ResFactura.Respuesta);
                    oVenta.Folio = (oFactura.Serie + oFactura.Folio);
                    Guardar.Generico<Venta>(oVenta);
                    */
                    bGenerarFolio = false;
                    
                    // Se manda a afectar contabilidad (AfeConta)
                    var oFactura = General.GetEntity<VentaFactura>(c => c.VentaFacturaID == ResFactura.Respuesta && c.Estatus);
                    if (oVenta.ACredito)
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.VentaCredito, iVentaID, (oFactura.Serie + oFactura.Folio), oCliente.Nombre);
                    else
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.VentaContadoFacturaDirecta, iVentaID, (oFactura.Serie + oFactura.Folio), oCliente.Nombre);
                }
                else
                {
                    UtilLocal.MensajeAdvertencia("Hubo un error al generar la factura.\n\n" + ResFactura.Mensaje);
                    // return true;  // Igual se sale, porque ya se guardaron todos los datos del pago.
                }
            }
            
            // Se genera el folio de la venta, si no se hizo factura o hubo un error al generar la factura
            if (bGenerarFolio) {
                // Se genera el folio de venta
                string sFolio = VentasProc.GenerarFolioDeVenta();
                oVenta.Folio = sFolio;
                oVenta.FolioIni = sFolio;
                Guardar.Generico<Venta>(oVenta);

                // Se imprimen el ticket correpondiente
                // Se verifica si es una venta por Anticipo de 9500
                var o9500Ant = General.GetEntity<Cotizacion9500>(q => q.EstatusGenericoID == Cat.EstatusGenericos.Pendiente
                    && q.AnticipoVentaID.Value == oVenta.VentaID && q.Estatus);
                if (o9500Ant == null) {
                    VentasProc.GenerarTicketDeVenta(oVenta.VentaID, oPorCobrar.ctlDetalle.ObtenerListaVenta());
					// Se verifica si se debe imprimir ticket precio 1
					if(General.Exists<Cliente>(c => c.ClienteID == oPorCobrar.ClienteID && c.TicketPrecio1.HasValue && c.TicketPrecio1.Value))
	                    VentasProc.GenerarTicketPrecio1(oVenta.VentaID);
                }
                else
                    VentasProc.GenerarTicketDe9500(o9500Ant.Cotizacion9500ID);
            }

            // Se obtiene la vista de la venta actualizada, por el cambio de folio y otros cambios que pudo haber tenido
            var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == oVenta.VentaID);

            // Se verifica si se crearon movimientos bancarios (por pagos de banco), en cuyo caso, se completan con el folio de venta asignado
            var oPagoDet = General.GetListOf<VentasPagosDetalleView>(c => c.VentaID == iVentaID);
            foreach (var oReg in oPagoDet)
            {
                if (oReg.FormaDePagoID == Cat.FormasDePago.Cheque || oReg.FormaDePagoID == Cat.FormasDePago.Tarjeta || oReg.FormaDePagoID == Cat.FormasDePago.Transferencia)
                {
                    var oMovBanco = General.GetEntity<BancoCuentaMovimiento>(c => c.RelacionTabla == Cat.Tablas.VentaPagoDetalle && c.RelacionID == oReg.VentaPagoDetalleID);
                    oMovBanco.Referencia = oVentaV.Folio;
                    Guardar.Generico<BancoCuentaMovimiento>(oMovBanco);
                }
            }

            // Se manda a afectar contabilidad (AfeConta), si es vale
            // Ya no se manda hacer esta póliza porque este movimiento ya lo contempla la Factura global del Día o la Factura al 
            // momento del Pago, sea crédito o contado. Si se quiciera reactivar esta póliza verificar la Función porque Moy la modificó 2015-08-26
            // if (oPagoDet.Any(c => c.FormaDePagoID == Cat.FormasDePago.Vale))
            //     ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.VentaContadoVale, iVentaID, oVentaV.Folio, oCliente.Nombre);

            // Se agrega al Kardex
            var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == iVentaID && c.Estatus);
            foreach (var oReg in oVentaDet)
            {
                AdmonProc.RegistrarKardex(new ParteKardex()
                {
                    ParteID = oReg.ParteID,
                    OperacionID = Cat.OperacionesKardex.Venta,
                    SucursalID = oVentaV.SucursalID,
                    Folio = oVentaV.Folio,
                    Fecha = DateTime.Now,
                    RealizoUsuarioID = oVenta.RealizoUsuarioID,
                    Entidad = oVentaV.Cliente,
                    Origen = oVentaV.Sucursal,
                    Destino = oVentaV.Cliente,
                    Cantidad = oReg.Cantidad,
                    Importe = (oReg.PrecioUnitario + oReg.Iva),
                    RelacionTabla = Cat.Tablas.Venta,
                    RelacionID = iVentaID
                });
            }

            // Se cierra la ventana de "Cargando.."
            Cargando.Cerrar();

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        private bool VentasCambios()
        {
            // Se validan los datos
            CajaVentasCambios oVentasCambios = (this.ctlCajaGeneral.ctlVentasCambios);
            if (!oVentasCambios.ctlCobro.Validar())
                return false;

            // Se verifican los cambios que se tienen que hacer
            int iVentaID = oVentasCambios.VentaID;
            var oPagoDetActual = General.GetListOf<VentaPagoDetalle>(q => q.VentaPagoID == oVentasCambios.VentaPagoID && q.Estatus);
            var oPagoDetNuevo = oVentasCambios.ctlCobro.GenerarPagoDetalle();
            var oCambios = this.CambiosPagoDetalle(oPagoDetActual, oPagoDetNuevo);
            bool bCambioFP = (oCambios.Count > 0);
            var oVenta = General.GetEntity<Venta>(q => q.VentaID == iVentaID);
            bool bCambioGen = (oVentasCambios.ctlCobro.VendodorID != oVenta.RealizoUsuarioID || oVentasCambios.ctlCobro.ComisionistaID != oVenta.ComisionistaClienteID);
            
            if (!bCambioFP && !bCambioGen)
            {
                UtilLocal.MensajeAdvertencia("No se ha realizado ningún cambio.");
                return false;
            }

            // Se confirma la operación
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas realizar el cambio especificado?") != DialogResult.Yes)
                return false;

            // Se solicita la validación del usuario
            var Res = UtilDatos.ValidarObtenerUsuario("Ventas.Cambios.Agregar");
            if (Res.Error)
                return false;
            var oUsuario = Res.Respuesta;

            // Se solicita la validación de autorización
            Res = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.Cambios.Agregar", "Autorización");
            var oAutorizo = Res.Respuesta;

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se crea el objeto para guardar los cambios
            var oRegCambios = new VentaCambio();
            oRegCambios.SucursalID = GlobalClass.SucursalID;
            oRegCambios.VentaPagoID = oVentasCambios.VentaPagoID;
            oRegCambios.Fecha = dAhora;

            // Se guardan los cambios en la venta
            if (bCambioGen)
            {
                // Cambio de vendedor
                if (oVentasCambios.ctlCobro.VendodorID != oVenta.RealizoUsuarioID)
                {
                    // Se registra el cambio
                    oRegCambios.RealizoIDAntes = oVenta.RealizoUsuarioID;
                    oRegCambios.RealizoIDDespues = oVentasCambios.ctlCobro.VendodorID;
                    //
                    oVenta.RealizoUsuarioID = oVentasCambios.ctlCobro.VendodorID;
                }

                // Cambio de comisionista
                if (oVentasCambios.ctlCobro.ComisionistaID != oVenta.ComisionistaClienteID.Valor())
                {
                    // Se registra el cambio
                    oRegCambios.ComisionistaIDAntes = oVenta.ComisionistaClienteID;
                    oRegCambios.ComisionistaIDDespues = oVentasCambios.ctlCobro.ComisionistaID;
                    
                    // Se hacen los cambios de notas de crédito aplicables
                    decimal mComisionAntes = UtilDatos.VentaComisionCliente(iVentaID, oVenta.ComisionistaClienteID.Valor());
                    if (mComisionAntes > 0)
                        VentasProc.GenerarNotaDeCredito(oVenta.ComisionistaClienteID.Valor(), (mComisionAntes * -1), ""
                            , Cat.OrigenesNotaDeCredito.CambioComisionista, iVentaID.ToString());
                    decimal mComisionDespues = UtilDatos.VentaComisionCliente(iVentaID, oVentasCambios.ctlCobro.ComisionistaID);
                    if (mComisionDespues > 0)
                        VentasProc.GenerarNotaDeCredito(oVentasCambios.ctlCobro.ComisionistaID, mComisionDespues, ""
                            , Cat.OrigenesNotaDeCredito.CambioComisionista, iVentaID.ToString());

                    // Se registra el cambio en la venta
                    oVenta.ComisionistaClienteID = oVentasCambios.ctlCobro.ComisionistaID;
                }

                oVenta.ComisionistaClienteID = (oVenta.ComisionistaClienteID > 0 ? oVenta.ComisionistaClienteID : null);
                Guardar.Generico<Venta>(oVenta);
            }
            // Se guardan los cambios en la forma de pago
            var oVentaPago = General.GetEntity<VentaPago>(q => q.VentaPagoID == oVentasCambios.VentaPagoID && q.Estatus);
            var oPagoDetalle = oCambios.ObtenerLista();
            List<VentaPagoDetalle> oPagosValidos = new List<VentaPagoDetalle>();
            if (oPagoDetalle.Count > 0)
            {
                foreach (var oCambio in oCambios)
                {
                    if (oCambio.IdEstatus == Cat.TiposDeAfectacion.Borrar)
                        oCambio.Objeto.Estatus = false;
                    else
                        oPagosValidos.Add(oCambio.Objeto);
                }

                // Se registran los cambios en las formas de pago
                oRegCambios.FormasDePagoAntes = UtilDatos.VentaPagoFormasDePago(oVentaPago.VentaPagoID);
                oRegCambios.FormasDePagoDespues = UtilDatos.VentaPagoFormasDePago(oPagosValidos);
                //
                Guardar.VentaPago(oVentaPago, oPagoDetalle);
            }

            // Se guardan los datos de los cambios
            //? oRegCambios.ComisionistaIDAntes = (oRegCambios.ComisionistaIDAntes > 0 ? oRegCambios.ComisionistaIDAntes : null);
            //? oRegCambios.ComisionistaIDDespues = (oRegCambios.ComisionistaIDDespues > 0 ? oRegCambios.ComisionistaIDDespues : null);
            Guardar.Generico<VentaCambio>(oRegCambios);

            // Se guarda la autorización
            VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.VentaCambio, Cat.Tablas.VentaCambio, oRegCambios.VentaCambioID
                , (oAutorizo == null ? 0 : oAutorizo.UsuarioID));
                        
            // Se genera el ticket
            // ..

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        private bool CambioTurno()
        {
            // Se ejecuta una acción dependiendo del "paso" en el que vaya el proceso
            //   Paso 0: Corte mostrado
            //   Paso 1: Pantalla para conteo mostrada
            //   Paso 2: Corte mostrado con conteo realizado
            CajaCambioTurno oCambioTurno = this.ctlCajaGeneral.ctlCambioTurno;
            switch (oCambioTurno.Paso)
            {
                case 0:  // De momento no aplica
                    return false;
                case 1:
                    oCambioTurno.RegistrarConteo(oCambioTurno.ctlConteo.Total);
                    oCambioTurno.Paso = 2;
                    return false;
            }
            
            // Se piden las contraseñas
            var oResU1 = UtilDatos.ValidarObtenerUsuario(null, false, "Engrega");
            if (oResU1.Error)
                return false;
            var oResU2 = UtilDatos.ValidarObtenerUsuario(null, false, "Recibe");
            if (oResU2.Error)
                return false;

            // Se genera un pdf con la información del corte
            var Rep = new Report();
            Rep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CajaCorte.frx");
            Rep.RegisterData(oCambioTurno.GenerarDatosCorte(), "Corte");
            Rep.RegisterData(this.ctlCajaGeneral.ctlDetalleCorte.GenerarDatosDetalle(), "Detalle");
            Rep.RegisterData(oCambioTurno.GenerarDatosCambios(), "Cambios");
            Rep.RegisterData(new List<ConteoCaja>() { oCambioTurno.ctlConteo.GenerarConteo() }, "Conteo");
            Rep.SetParameterValue("Sucursal", GlobalClass.NombreTienda);
            Rep.SetParameterValue("Entrega", oResU1.Respuesta.NombreUsuario);
            Rep.SetParameterValue("Recibe", oResU2.Respuesta.NombreUsuario);
            // Se obtiene el nombre del archivo
            int iConsecutivo = 0;
            string sRuta = Config.Valor("Caja.CambioTurno.RutaArchivos");
            string sFormato = (sRuta + DateTime.Now.ToString("yyyyMMdd") + "-{0}.pdf");
            string sArchivo;
            do {
                sArchivo = string.Format(sFormato, ++iConsecutivo);
            } while (File.Exists(sArchivo));
            // Se genera el Pdf
            var oRepPdf = new FastReport.Export.Pdf.PDFExport() { ShowProgress = true };
            Rep.Prepare();
            Rep.Export(oRepPdf, sArchivo);
                        
            // Se manda la impresion
            UtilLocal.EnviarReporteASalida("Reportes.CajaCorte.Salida", Rep, true);

            // Se regresa el paso del proceso
            oCambioTurno.Paso = 0;

            // Se registra el cambio de turno en la base de datos
            Guardar.Generico<CajaCambioDeTurno>(new CajaCambioDeTurno()
            {
                SucursalID = GlobalClass.SucursalID,
                Fecha = DateTime.Now,
                EntregaUsuarioID = oResU1.Respuesta.UsuarioID,
                RecibeUsuarioID = oResU2.Respuesta.UsuarioID,
                Total = oCambioTurno.Total,
                Conteo = oCambioTurno.Conteo
            });

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        private bool Corte()
        {
            // Se ejecuta una acción dependiendo del "paso" en el que vaya el proceso
            //   Paso 0: Corte mostrado
            //   Paso 1: Pantalla para conteo mostrada
            //   Paso 2: Corte mostrado con conteo realizado
            //   Paso 3: Pantalla de tickets del día (para Factura Global)
            CajaCorte oCorte = this.ctlCajaGeneral.ctlCorte;
            switch (oCorte.Paso)
            {
                case 0:  // De momento no aplica
                    return false;
                case 1:
                    oCorte.RegistrarConteo(oCorte.ctlConteo.Total);
                    oCorte.Paso = 2;
                    return false;
                /* case 2:
                    oCorte.ctlFacturaGlobal = new CajaFacturaGlobal() { Dock = DockStyle.Fill };
                    oCorte.Parent.Controls.Add(oCorte.ctlFacturaGlobal);
                    oCorte.ctlFacturaGlobal.BringToFront();
                    oCorte.Paso = 3;
                    return false;
                */
            }

            // Se valida que no haya ventas por cobrar
            DateTime dHoy = DateTime.Today;
            if (General.Exists<Venta>(c => c.VentaEstatusID == Cat.VentasEstatus.Realizada && EntityFunctions.TruncateTime(c.Fecha) == dHoy && c.Estatus))
            {
                UtilLocal.MensajeAdvertencia("Existen una o más ventas por cobrar. No se puede continuar.");
                return false;
            }

            // Se valida que se haya hecho el fondo de caja
            var oDia = General.GetEntity<CajaEfectivoPorDia>(q => q.SucursalID == GlobalClass.SucursalID && q.Dia == dHoy && q.Estatus);
            if (oDia == null)
            {
                UtilLocal.MensajeAdvertencia("No se ha realizado el Fondo de Caja, por lo tanto no se puede continuar.");
                return false;
            }

            // Se valida que no haya autorizaciones pendientes
            if (General.Exists<Autorizacion>(c => c.SucursalID == GlobalClass.SucursalID && EntityFunctions.TruncateTime(c.FechaRegistro) == dHoy && !c.Autorizado))
            {
                UtilLocal.MensajeAdvertencia("No se han completado todas las Autorizaciones del día. No se puede continuar.");
                return false;
            }

            // Se valida que se hayan recibido todos los traspasos marcados
            if (General.Exists<MovimientoInventario>(c => c.TipoOperacionID == Cat.TiposDeOperacionMovimientos.Traspaso && c.SucursalDestinoID == GlobalClass.SucursalID
                && c.TraspasoEntregado.HasValue && c.TraspasoEntregado.Value && !c.FechaRecepcion.HasValue))
            {
                UtilLocal.MensajeAdvertencia("No se han registrado todos los traspasos que ya se marcaron como entregados.");
                return false;
            }
            
            // Se valida que no haya conteos de inventario pendientes
            if (Helper.ConvertirBool(Config.Valor("Inventario.Conteo.RevisarEnCorte")))
            {
                var oConteosPen = UtilDatos.InventarioUsuariosConteoPendiente();
                if (oConteosPen.Count > 0)
                {
                    UtilLocal.MensajeAdvertencia("No se han realizado todos los conteos de inventario. No se puede continuar.");
                    return false;
                }
            }

            // Se valida que no haya Control de Cascos pendientes por completar
            if (General.Exists<CascosRegistrosView>(c => c.NumeroDeParteRecibido == null && c.FolioDeCobro == null
                && (c.VentaEstatusID != Cat.VentasEstatus.Cancelada && c.VentaEstatusID != Cat.VentasEstatus.CanceladaSinPago)))
            {
                UtilLocal.MensajeAdvertencia("No se han completado todos los registros de cascos. No se puede continuar.");
                return false;
            }

            // Se confirma la operación
            string sMensaje = string.Format("¿Estás seguro que deseas realizar el Cierre de Caja con un importe de {0}?", oCorte.Conteo.ToString(GlobalClass.FormatoMoneda));
            if (oCorte.Diferencia != 0)
                sMensaje += string.Format("\n\nToma en cuenta que hay una diferencia de {0} y por tanto se requerirá una autorización para continuar.", 
                    oCorte.Diferencia.ToString(GlobalClass.FormatoMoneda));

            if (UtilLocal.MensajePregunta(sMensaje) != DialogResult.Yes)
                return false;

            // Se solicita la validación del usuario
            var Res = UtilDatos.ValidarObtenerUsuario("Ventas.CorteDeCaja.Agregar");
            if (Res.Error)
                return false;
            var oUsuario = Res.Respuesta;

            // Se solicita la validación de autorización, si aplica
            Usuario oAutorizo = null;
            if (oCorte.Diferencia != 0)
            {
                Res = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.CorteDeCaja.Diferencia", "Autorización");
                //if (Res.Exito)
                    oAutorizo = Res.Respuesta;
            }

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;

            // Se manda a generar la Factura Global
            bool bFacturaGlobal = this.FacturaGlobal(oDia);
            if (!bFacturaGlobal)
                return false;

            // Se manda guardar el histórico del corte
            bool bSeguir = this.GuardarHistoricoCorte();
            if (!bSeguir)
                return false;

            // Se registra el cierre de caja
            oDia.CierreDebeHaber = oCorte.Total;
            oDia.Cierre = oCorte.Conteo;
            oDia.CierreUsuarioID = oUsuario.UsuarioID;
            Guardar.Generico<CajaEfectivoPorDia>(oDia);

            // Se registra la póliza de la diferencia, si hubo
            if (oCorte.Diferencia != 0)
            {
                if (oCorte.Diferencia > 0)
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, "DIFERENCIA EN CORTE", Cat.ContaCuentasAuxiliares.Caja, Cat.ContaCuentasAuxiliares.FondoDeCaja
                        , oCorte.Diferencia, DateTime.Now.ToShortDateString(), Cat.Tablas.CajaEfectivoPorDia, oDia.CajaEfectivoPorDiaID);
                else
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, "DIFERENCIA EN CORTE", Cat.ContaCuentasAuxiliares.FondoDeCaja, Cat.ContaCuentasAuxiliares.Caja
                        , (oCorte.Diferencia * -1), DateTime.Now.ToShortDateString(), Cat.Tablas.CajaEfectivoPorDia, oDia.CajaEfectivoPorDiaID);
            }

            // Se genera y guarda la autorización, si aplica
            if (oCorte.Diferencia != 0)
            {
                int iAutorizoID = (oAutorizo == null ? 0 : oAutorizo.UsuarioID);
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.CorteDeCaja, Cat.Tablas.CajaEfectivoPorDia, oDia.CajaEfectivoPorDiaID, iAutorizoID);
            }

            // Se genera un pdf con la información del corte
            var Rep = new Report();
            Rep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CajaCorte.frx");
            Rep.RegisterData(oCorte.GenerarDatosCorte(), "Corte");
            Rep.RegisterData(this.ctlCajaGeneral.ctlDetalleCorte.GenerarDatosDetalle(), "Detalle");
            Rep.RegisterData(oCorte.GenerarDatosCambios(), "Cambios");
            Rep.RegisterData(new List<ConteoCaja>() { oCorte.ctlConteo.GenerarConteo() }, "Conteo");
            Rep.SetParameterValue("Sucursal", GlobalClass.NombreTienda);
            // Se obtiene el nombre del archivo
            string sRuta = Config.Valor("Caja.Corte.RutaArchivos");
            string sArchivo = (sRuta + DateTime.Now.ToString("yyyyMMdd") + "_" + GlobalClass.NombreTienda + ".pdf");
            // Se genera el Pdf
            var oRepPdf = new FastReport.Export.Pdf.PDFExport() { ShowProgress = true };
            Rep.Prepare();
            Rep.Export(oRepPdf, sArchivo);

            // Se manda la impresion
            UtilLocal.EnviarReporteASalida("Reportes.CajaCorte.Salida", Rep, true);

            // Se regresa el paso del proceso
            oCorte.Paso = 0;

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento completado correctamente.");

            return true;
        }

        #endregion

        #region [ Uso interno ]

        private bool FacturaGlobal(CajaEfectivoPorDia oDia)
        {
            // Se obtiene el importe a restar, excepto si es domingo
            decimal mRestar = 0;
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                mRestar = Helper.ConvertirDecimal(Config.Valor("Ventas.FacturaGlobal.Restar"));

            // Se obtienen el total de los tickets del día
            DateTime dHoy = DateTime.Today;
            var oPagosDet = General.GetListOf<VentasPagosDetalleAvanzadoView>(c => c.SucursalID == GlobalClass.SucursalID && EntityFunctions.TruncateTime(c.Fecha) == dHoy
                && !c.Facturada);
            var oDevsV = General.GetListOf<VentasDevolucionesView>(c => c.SucursalID == GlobalClass.SucursalID && EntityFunctions.TruncateTime(c.Fecha) == dHoy);
            decimal mTickets = 0, mNegativos = 0, mCancelaciones = 0, mDevoluciones = 0;
            decimal mDevolucionesDia = 0, mDevolucionesDiasAnt = 0, mGarantiasDia = 0, mGarantiasDiasAnt = 0, mCobranza = 0;
            decimal mFacturarVales = 0, mCostoVales = 0;
            string sCancelaciones = "", sDevoluciones = "";
            decimal mCostoTotal = 0;
            // var oVentasProc = new List<int>();
            var oPagosProc = new List<int>();
            foreach (var oReg in oPagosDet)
            {
                // Si es nota de crédito negativa, se ignora
                // Se determinó que las devoluciones y las garantías con vale (las cuales generan un pago negativo) sí deben restar en la 
                // factura global porque en otro momento, cuando se use el vale, ese importe sí va a sumar en la factura de ese día.
                // Se llegó a esta conclusión entre Isidro y Santa Haidé. 17/04/2015
                // if (oReg.FormaDePagoID == Cat.FormasDePago.Vale && oReg.Importe < 0)
                //     continue;

                // Se suma el importe a la variable correspondiente
                if (oReg.Importe >= 0)
                {
                    mTickets += oReg.Importe;

                    // Para diferenciar vales
                    if (oReg.FormaDePagoID == Cat.FormasDePago.Vale)
                        mFacturarVales += oReg.Importe;

                    // Para diferencia la cobranza
                    if (oReg.ACredito.Valor())
                        mCobranza += oReg.Importe;

                    // Se suma el costo, si es venta a crédito, se calcula un proporcional
                    if (!oPagosProc.Contains(oReg.VentaPagoID.Valor()))
                    {
                        var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == oReg.VentaID && c.Estatus);
                        decimal mCosto = oVentaDet.Sum(c => c.Costo * c.Cantidad);
                        decimal mPrecio = oVentaDet.Sum(c => (c.PrecioUnitario + c.Iva) * c.Cantidad);
                        // Si la venta no está pagada, se calcula un proporcional
                        if (oReg.VentaEstatusID != Cat.VentasEstatus.Completada && mPrecio > 0)
                            mCosto = ((oReg.ImportePago.Valor() / mPrecio) * mCosto);
                        mCostoTotal += mCosto;
                        oPagosProc.Add(oReg.VentaPagoID.Valor());

                        // Para diferenciar vales
                        var oPagoVales = General.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == oReg.VentaPagoID && c.Importe > 0
                            && c.TipoFormaPagoID == Cat.FormasDePago.Vale && c.Estatus);
                        if (oPagoVales.Count > 0)
                            mCostoVales += oPagoVales.Sum(c => c.Importe);
                    }
                }
                else
                {
                    // Se verifica a qué tipo de movimiento corresponde el pago negativo, Devolución / Garantía / 9500
                    decimal mImporteNeg = (oReg.Importe * -1);

                    // Se verifica si es una Devolución/Cancelación
                    var oDev = General.GetEntity<VentaDevolucion>(c => c.VentaPagoDetalleID == oReg.VentaPagoDetalleID && c.Estatus);
                    if (oDev == null)
                    {
                        // Se verifica si es de un 9500
                        var o9500 = General.GetEntity<Cotizacion9500>(c => c.AnticipoVentaID == oReg.VentaID && c.Estatus);
                        if (o9500 == null)
                        {
                            // Se verifica si es de una garantía
                            var oGar = General.GetEntity<VentaGarantia>(c => c.VentaPagoDetalleID == oReg.VentaPagoDetalleID && c.Estatus);
                            if (oGar == null)
                            {
                                // Si se llega aquí quiere decir que hay un pago negativo que no es ni de devolución/cancelación ni de 9500 ni de garantía. Es importante
                                // revisar de qué es para hacer los ajustes necesarios - Moi 16/06/2015
                                UtilLocal.MensajeAdvertencia("Se encontró un pago negativo sin origen aparente. Número: " + oReg.VentaPagoDetalleID.ToString()
                                    + ". Por favor toma nota de este número y avísale al administrador del sistema.");
                            }
                            else
                            {
                                // Se suma el importe correspondiente de venta y de costo
                                if (oReg.FechaVenta < dHoy)
                                {
                                    mGarantiasDiasAnt += mImporteNeg;

                                    mCostoTotal -= oGar.Costo;
                                    // Para diferenciar vales
                                    if (oReg.FormaDePagoID == Cat.FormasDePago.Vale)
                                        mCostoVales -= oGar.Costo;
                                }
                                else
                                {
                                    mGarantiasDia += mImporteNeg;
                                    
                                    // No se resta el costo, por lo mismo que los pagos negativos de devoluciones (abajo mencionado).
                                }
                            }
                        }
                        else
                        {
                            // Se suma el importe correspondiente
                            mNegativos += mImporteNeg;

                            // Se suma el costo, se toma el importe proporcional según el pago
                            var o9500Det = General.GetListOf<Cotizacion9500Detalle>(c => c.Cotizacion9500ID == o9500.Cotizacion9500ID && c.Estatus);
                            decimal mCosto = o9500Det.Sum(c => c.Costo * c.Cantidad);
                            decimal mPrecio = o9500Det.Sum(c => c.PrecioAlCliente * c.Cantidad);
                            mCosto = ((oReg.Importe / mPrecio) * mCosto);
                            mCostoTotal += mCosto;

                            // Para diferenciar vales
                            if (oReg.FormaDePagoID == Cat.FormasDePago.Vale)
                                mCostoVales += mCosto;
                        }
                    }
                    else
                    {
                        // Se suma el importe correspondiente de venta y de costo
                        if (oReg.FechaVenta < dHoy)
                        {
                            mDevolucionesDiasAnt += mImporteNeg;

                            // Para la factura impresa
                            string sFolioVenta = ("|" + oReg.FolioVenta + "|");
                            if (oDev.EsCancelacion)
                            {
                                mCancelaciones += (oReg.Importe * -1);
                                sCancelaciones += (sCancelaciones.Contains(sFolioVenta) ? "" : (", " + sFolioVenta));
                            }
                            else
                            {
                                mDevoluciones += (oReg.Importe * -1);
                                sDevoluciones += (sCancelaciones.Contains(sFolioVenta) ? "" : (", " + sFolioVenta));
                            }

                            // Para diferenciar vales (como es negativo, se resta)
                            // Siempre no, si es un pago negativo no se debe considerar para la suma del importe de vales (Haid{e e Isidro detectamos que los vales
                            // creados son negativos y estaban apareciendo en la FGD en negativo y esto no es correcto. Cuando se crea un vale ya existe una póliza
                            // que hace la operación de afectar Caja y Anticipo Clientes. La FGD sólo debe afectarse cuando los vales se usan)- Moi 08/08/2015
                            // if (oReg.FormaDePagoID == Cat.FormasDePago.Vale)
                            //     mFacturarVales += oReg.Importe;

                            // Se resta el costo, (cada pago negativo corresponde sólo a una devolución)
                            var oDevDet = General.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == oDev.VentaDevolucionID && c.Estatus);
                            mCostoTotal -= (oDevDet.Count > 0 ? oDevDet.Sum(c => c.Costo * c.Cantidad) : 0);

                            // Para diferenciar vales
                            if (oReg.FormaDePagoID == Cat.FormasDePago.Vale)
                                mCostoVales -= oDevDet.Sum(c => c.Costo * c.Cantidad);
                        }
                        else
                        {
                            mDevolucionesDia += mImporteNeg;

                            // No se resta el costo en pagos negativos del día, pues tampoco se suma el costo de su pago positivo equivalente ya que si la venta está devuelta
                            // ya no tiene detalle - Moi 16/06/2015
                            // var oDevDet = General.GetListOf<VentaDevolucionDetalle>(c => c.VentaDevolucionID == oDev.VentaDevolucionID && c.Estatus);
                            // mCostoTotal -= (oDevDet.Count > 0 ? oDevDet.Sum(c => c.Costo * c.Cantidad) : 0);
                        }
                    }
                }
            }

            // Se obtiene el total de los tickets de días anteriores facturados el día de hoy
            var oDatos = General.GetListOf<VentasFacturasDetalleAvanzadoView>(c => EntityFunctions.TruncateTime(c.Fecha) == dHoy
                && c.FechaVenta < dHoy && c.EstatusGenericoID == Cat.EstatusGenericos.Completada)
                .Select(c => new { c.VentaID }).Distinct();
            decimal mFacturadoDiasAnt = 0;
            foreach (var oReg in oDatos)
            {
                // Se verifica si el pago es de la sucursal actual, si no, no se cuenta
                if (General.Exists<VentasPagosView>(c => c.VentaID == oReg.VentaID && c.Importe > 0 && c.SucursalID != GlobalClass.SucursalID))
                    continue;
                
                // Se obtiene el importe de lo abonado sólo los días anteriores a hoy
                var oAbonosAnt = General.GetListOf<VentasPagosView>(c => c.VentaID == oReg.VentaID && c.Fecha < dHoy);
                decimal mAbonosAnt = oAbonosAnt.Sum(c => c.Importe);
                mFacturadoDiasAnt += mAbonosAnt;

                // Para diferenciar vales
                /* Ya no se consideran los vales de lo facturado de tickets de días anteriores porque ya fueron contemplados en la factura global del día de la venta
                   - Moi 2015-08-26
                var oAbonosAntVale = General.GetListOf<VentasPagosDetalleAvanzadoView>(c => c.VentaID == oReg.VentaID && c.Fecha < dHoy
                    && c.FormaDePagoID == Cat.FormasDePago.Vale);
                decimal mAbonosAntVale = oAbonosAntVale.Sum(c => c.Importe);
                mFacturarVales += mAbonosAntVale;
                */

                // Se resta el costo, proporcional a lo abonado en días anteriores
                var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == oReg.VentaID && c.Estatus);
                decimal mCosto = oVentaDet.Sum(c => c.Costo * c.Cantidad);
                decimal mPrecio = oVentaDet.Sum(c => (c.PrecioUnitario + c.Iva) * c.Cantidad);
                if (mPrecio == 0)
                    continue;  // No estoy seguro por qué el precio podría ser cero, sería bueno analizar con más calma :D
                mCosto = ((mAbonosAnt / mPrecio) * mCosto);
                mCostoTotal -= mCosto;

                // Para diferenciar vales
                // mCostoVales -= ((mAbonosAntVale / mPrecio) * mCosto);
            }

            // Se hace el cálculo final
            decimal mCostoMinimo = (UtilLocal.ObtenerImporteMasIva(mCostoTotal) * 1.1M);
            // decimal mOficial = (mTickets - mNegativos - mDevoluciones - mCancelaciones - mFacturadoDiasAnt);
            decimal mOficial = (mTickets - mNegativos - mDevolucionesDia - mDevolucionesDiasAnt - mGarantiasDia - mGarantiasDiasAnt - mFacturadoDiasAnt);
            decimal mFacturar = (mOficial - mRestar);
            decimal mRestante = 0;
            var oFacturaGlobalAnt = General.GetListOf<CajaFacturaGlobal>(c => c.SucursalID == GlobalClass.SucursalID).OrderBy(c => c.Dia).LastOrDefault();
            if (mFacturar > mCostoMinimo)
            {
                // Se verifica si hay saldo restante, para abonar la diferencia
                if (oFacturaGlobalAnt.SaldoRestante > 0)
                {
                    mRestante = (mFacturar - mCostoMinimo);
                    mRestante = (mRestante > oFacturaGlobalAnt.SaldoRestante ? oFacturaGlobalAnt.SaldoRestante : mRestante);
                    mFacturar -= mRestante;
                    mRestante *= -1;
                }
            }
            else
            {
                mRestante = (mCostoMinimo - mFacturar);
                mFacturar = mCostoMinimo;
            }

            // Se genera cuadro informativo de los cálculos realizados
            /* var oTexto = new StringBuilder();
            oTexto.AppendLine("Tickets:\t\t(+)\t" + mTickets.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("Negativos:\t\t(-)\t" + mNegativos.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("Devoluciones:\t\t(-)\t" + mDevoluciones.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("Cancelaciones:\t(-)\t" + mCancelaciones.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("Facturado días ant.:\t(-)\t" + mFacturadoDiasAnt.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("Restar:\t\t(-)\t" + mRestar.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("Costo Min.:\t\t(i)\t" + mCostoMinimo.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("Restante ant.:\t(-)\t" + mRestante.ToString(GlobalClass.FormatoMoneda));
            oTexto.AppendLine("------------------------------------------------------");
            oTexto.AppendLine("Facturar:\t\t(=)\t" + mFacturar.ToString(GlobalClass.FormatoMoneda));
            new MensajeTexto("Factura Global del Día", oTexto.ToString()).ShowDialog(Principal.Instance);
            */

            // Se manda hacer la factura
            int iFacturaID = 0;
            if (mFacturar > 0)
            {
                string sVentas = ("VENTAS PÚBLICO GENERAL: " + dHoy.ToString("d"));
                sCancelaciones = ("CANCELACIONES: " + (sCancelaciones.Length > 0 ? sCancelaciones.Substring(2).Replace("|", "") : ""));
                sDevoluciones = ("DEVOLUCIONES: " + (sDevoluciones.Length > 0 ? sDevoluciones.Substring(2).Replace("|", "") : ""));
                var oResFactura = VentasProc.GenerarFacturaGlobal(sVentas, mCostoTotal, (mFacturar + mCancelaciones + mDevoluciones)
                    , sCancelaciones, mCancelaciones, sDevoluciones, mDevoluciones);
                if (oResFactura.Error)
                {
                    UtilLocal.MensajeAdvertencia("Hubo un error al generar la factura.\n\n" + oResFactura.Mensaje);
                    return false;
                }
                iFacturaID = oResFactura.Respuesta;
            }
            else
            {
                mFacturar = 0;
                mRestante = (mOficial - mRestar);
                if (mRestante < 0)
                    mRestante *= -1;
            }
            

            // Se guardan los datos de la factura global
            var oFacturaGlobal = new CajaFacturaGlobal()
            {
                Dia = oDia.Dia,
                SucursalID = GlobalClass.SucursalID,
                Tickets = mTickets,
                FacturadoDeDiasAnt = mFacturadoDiasAnt,
                Negativos = mNegativos,
                DevolucionesDia = mDevolucionesDia,
                DevolucionesDiasAnt = mDevolucionesDiasAnt,
                GarantiasDia = mGarantiasDia,
                GarantiasDiasAnt = mGarantiasDiasAnt,
                Cobranza = mCobranza,
                Restar = mRestar,
                CostoMinimo = mCostoMinimo,
                Restante = mRestante,
                SaldoRestante = (oFacturaGlobalAnt.SaldoRestante + mRestante),
                Facturado = mFacturar,
                FacturadoVales = mFacturarVales,
                VentaFacturaID = (iFacturaID > 0 ? (int?)iFacturaID : null)
            };
            Guardar.Generico<CajaFacturaGlobal>(oFacturaGlobal);

            // Se crea la Poliza correspondiente (AfeConta)
            if (iFacturaID > 0)
            {
                var oFactura = General.GetEntity<VentaFactura>(c => c.VentaFacturaID == iFacturaID && c.Estatus);
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.VentaContadoPagoFacturaGlobal, oFactura.VentaFacturaID, (oFactura.Serie + oFactura.Folio)
                    , "FACTURA GLOBAL DEL DÍA");
                // Se hace una póliza sencilla por la diferencia de, si hay
                if (mOficial != mFacturar)
                {
                    decimal mReserva = (mOficial - mFacturar);
                    mReserva = (mReserva > 0 ? mReserva : 0);
                    var oSucursal = General.GetEntity<Sucursal>(c => c.SucursalID == GlobalClass.SucursalID && c.Estatus);
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, "RESERVA NÓMINA", Cat.ContaCuentasAuxiliares.ReservaNomina, Cat.ContaCuentasAuxiliares.Resguardo
                        , mReserva, oSucursal.NombreSucursal, Cat.Tablas.CajaFacturaGlobal, oFacturaGlobal.CajaFacturaGlobalID);
                    // Se crea una póliza nueva cargando a Caja y la otra cuenta en cero, para equilibrar caja (caso raro), sólo si la reserva fue mayor a cero
                    if (mReserva > 0)
                    {
                        var oPoliza = ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, "RESERVA NÓMINA AJUSTE", Cat.ContaCuentasAuxiliares.Caja
                            , Cat.ContaCuentasAuxiliares.CapitalFijo, mReserva, "RN AJUSTE", null, null);
                        var oPolizaDet = General.GetEntity<ContaPolizaDetalle>(c => c.ContaPolizaID == oPoliza.ContaPolizaID && c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.CapitalFijo);
                        oPolizaDet.Abono = 0;
                        Guardar.Generico<ContaPolizaDetalle>(oPolizaDet);
                    }
                }
            }
            
            return true;
        }

        private bool GuardarHistoricoCorte()
        {
            int iSucursalID = GlobalClass.SucursalID;
            DateTime dHoy = DateTime.Today;
            DateTime dManiana = dHoy.AddDays(1);

            // Se guardan las ventas
            var oPagos = General.GetListOf<VentasPagosFormasDePagoView>(c => c.SucursalID == iSucursalID && c.Fecha >= dHoy && c.Fecha < dManiana
                && c.Importe > 0 && c.ACredito == false);
            foreach (var oReg in oPagos)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = Cat.CategoriasCorte.Ventas,
                    RelacionTabla = Cat.Tablas.Venta,
                    RelacionID = oReg.VentaID,
                    Concepto = oReg.FolioDeVenta,
                    Importe = oReg.Importe.Valor(),
                    Efectivo = oReg.Efectivo,
                    Cheque = oReg.Cheque,
                    Tarjeta = oReg.Tarjeta,
                    Transferencia = oReg.Transferencia,
                    Vale = oReg.Vale,
                    Factura = oReg.Facturada
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }

            // Se registran las cancelaciones
            var oCancelaciones = General.GetListOf<VentasDevolucionesView>(c => c.SucursalID == iSucursalID && c.Fecha >= dHoy && c.Fecha < dManiana);
            foreach (var oReg in oCancelaciones)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = (oReg.FechaDeVenta < dHoy ? Cat.CategoriasCorte.CancelacionesDiasAnteriores : Cat.CategoriasCorte.CancelacionesDia),
                    RelacionTabla = Cat.Tablas.VentaDevolucion,
                    RelacionID = oReg.VentaDevolucionID,
                    Concepto = oReg.FolioDeVenta,
                    Importe = oReg.Total.Valor(),
                    Efectivo = (oReg.FormaDePagoID == Cat.FormasDePago.Efectivo ? oReg.Total : 0),
                    Cheque = (oReg.FormaDePagoID == Cat.FormasDePago.Cheque ? oReg.Total : 0),
                    Tarjeta = (oReg.FormaDePagoID == Cat.FormasDePago.Tarjeta ? oReg.Total : 0),
                    Transferencia = (oReg.FormaDePagoID == Cat.FormasDePago.Transferencia ? oReg.Total : 0),
                    Vale = (oReg.FormaDePagoID == Cat.FormasDePago.Vale ? oReg.Total : 0),
                    Factura = oReg.Facturada
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }

            // Se registran las garantías
            var oGarantias = General.GetListOf<VentasGarantiasView>(c => c.SucursalID == iSucursalID && c.Fecha >= dHoy && c.Fecha < dManiana);
            foreach (var oReg in oGarantias)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = (oReg.FechaDeVenta < dHoy ? Cat.CategoriasCorte.GarantiasDiasAnteriores : Cat.CategoriasCorte.GarantiasDia),
                    RelacionTabla = Cat.Tablas.VentaGarantia,
                    RelacionID = oReg.VentaGarantiaID,
                    Concepto = oReg.FolioDeVenta,
                    Importe = oReg.Total.Valor(),
                    Efectivo = (oReg.AccionID == Cat.VentasGarantiasAcciones.Efectivo ? oReg.Total : 0),
                    Cheque = (oReg.AccionID == Cat.VentasGarantiasAcciones.Cheque ? oReg.Total : 0),
                    Tarjeta = (oReg.AccionID == Cat.VentasGarantiasAcciones.Tarjeta ? oReg.Total : 0),
                    Transferencia = (oReg.AccionID == Cat.VentasGarantiasAcciones.Transferencia ? oReg.Total : 0),
                    Vale = ((oReg.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito || oReg.AccionID == Cat.VentasGarantiasAcciones.ArticuloNuevo) ? oReg.Total : 0),
                    Factura = oReg.Facturada
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }

            // Se guarda la cobranza
            var oCobranza = General.GetListOf<VentasPagosFormasDePagoView>(c => c.SucursalID == iSucursalID && c.Fecha >= dHoy && c.Fecha < dManiana
                && c.Importe > 0 && c.ACredito == true);
            foreach (var oReg in oCobranza)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = Cat.CategoriasCorte.Cobranza,
                    RelacionTabla = Cat.Tablas.VentaPago,
                    RelacionID = oReg.VentaPagoID,
                    Concepto = oReg.FolioDeVenta,
                    Importe = oReg.Importe.Valor(),
                    Efectivo = oReg.Efectivo,
                    Cheque = oReg.Cheque,
                    Tarjeta = oReg.Tarjeta,
                    Transferencia = oReg.Transferencia,
                    Vale = oReg.Vale,
                    Factura = oReg.Facturada
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }

            // Se guardan los Vales creados
            var oVales = General.GetListOf<NotaDeCredito>(c => c.FechaDeEmision >= dHoy && c.FechaDeEmision < dManiana && c.Estatus);
            foreach (var oReg in oVales)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = Cat.CategoriasCorte.ValesCreados,
                    RelacionTabla = Cat.Tablas.NotaDeCredito,
                    RelacionID = oReg.NotaDeCreditoID,
                    Concepto = oReg.Observacion,
                    Importe = oReg.Importe
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }

            // Se guardan los gastos
            var oGastos = General.GetListOf<CajaEgreso>(c => c.CajaTipoEgresoID != Cat.CajaTiposDeEgreso.Resguardo && c.Fecha >= dHoy && c.Fecha < dManiana && c.Estatus);
            foreach (var oReg in oGastos)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = Cat.CategoriasCorte.Gastos,
                    RelacionTabla = Cat.Tablas.CajaEgreso,
                    RelacionID = oReg.CajaEgresoID,
                    Concepto = oReg.Concepto,
                    Importe = oReg.Importe
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }

            // Se guardan los resguardos
            var oResguardos = General.GetListOf<CajaEgreso>(c => c.CajaTipoEgresoID == Cat.CajaTiposDeEgreso.Resguardo && c.Fecha >= dHoy && c.Fecha < dManiana && c.Estatus);
            foreach (var oReg in oResguardos)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = Cat.CategoriasCorte.Resguardos,
                    RelacionTabla = Cat.Tablas.CajaEgreso,
                    RelacionID = oReg.CajaEgresoID,
                    Concepto = oReg.Concepto,
                    Importe = oReg.Importe
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }

            // Se guardan los refuerzos
            var oRefuerzos = General.GetListOf<CajaIngreso>(c => c.CajaTipoIngresoID == Cat.CajaTiposDeIngreso.Refuerzo && c.Fecha >= dHoy && c.Fecha < dManiana && c.Estatus);
            foreach (var oReg in oRefuerzos)
            {
                var oRegCorte = new CorteDetalleHistorico()
                {
                    Dia = dHoy,
                    SucursalID = iSucursalID,
                    CorteCategoriaID = Cat.CategoriasCorte.Refuerzos,
                    RelacionTabla = Cat.Tablas.CajaIngreso,
                    RelacionID = oReg.CajaIngresoID,
                    Concepto = oReg.Concepto,
                    Importe = oReg.Importe
                };
                Guardar.Generico<CorteDetalleHistorico>(oRegCorte);
            }


            // Si aplica, se retorna falso, para no hacer el corte, útil en caso de pruebas
            if (!GlobalClass.Produccion)
            {
                if (System.Configuration.ConfigurationManager.AppSettings["CorteDePrueba"] != null)
                    return !Helper.ConvertirBool(System.Configuration.ConfigurationManager.AppSettings["CorteDePrueba"]);
            }

            return true;
        }

        private ListaEstatus<VentaPagoDetalle> CambiosPagoDetalle(List<VentaPagoDetalle> oPagoDetActual, List<VentaPagoDetalle> oPagoDetNuevo)
        {
            var oCambios = new ListaEstatus<VentaPagoDetalle>();

            // Se comienza la búsqueda de cambios, partiendo de la lista actual
            bool bCambio;
            VentaPagoDetalle oNuevoExistente;
            foreach (var oActual in oPagoDetActual)
            {
                // Se restauran las variables
                oNuevoExistente = null;
                bCambio = false;

                // Se busca la forma de pago actual en la lista nueva
                foreach (var oNuevo in oPagoDetNuevo)
                {
                    if (oNuevo.TipoFormaPagoID == oActual.TipoFormaPagoID && oNuevo.NotaDeCreditoID == oActual.NotaDeCreditoID)
                    {
                        // Se verifica si hubo un cambio en el importe
                        if (oNuevo.Importe != oActual.Importe)
                        {
                            oActual.Importe = oNuevo.Importe;
                            bCambio = true;
                        }

                        // Se verifica si hubo un cambio en los datos bancarios, si aplica
                        if (oActual.TipoFormaPagoID == Cat.FormasDePago.Cheque
                            || oActual.TipoFormaPagoID == Cat.FormasDePago.Tarjeta
                            || oActual.TipoFormaPagoID == Cat.FormasDePago.Transferencia)
                        {
                            if (oNuevo.BancoID != oActual.BancoID || oNuevo.Folio != oActual.Folio || oNuevo.Cuenta != oActual.Cuenta)
                            {
                                oActual.BancoID = oNuevo.BancoID;
                                oActual.Folio = oNuevo.Folio;
                                oActual.Cuenta = oNuevo.Cuenta;
                                bCambio = true;
                            }
                        }

                        oNuevoExistente = oNuevo;
                        break;
                    }
                }

                // Se procesan los cambios
                if (oNuevoExistente == null)  // No se encontró en la nueva lista, por lo tanto, fue borrado
                {
                    oCambios.Add(new ObjetoEstatus<VentaPagoDetalle>("", Cat.TiposDeAfectacion.Borrar, oActual));
                }
                else  // Se encontró en la nueva lista
                {
                    if (bCambio)  // Se detectó un cambio en la forma de pago de la nueva lista, modificación
                        oCambios.Add(new ObjetoEstatus<VentaPagoDetalle>("", Cat.TiposDeAfectacion.Modificar, oActual));
                    oPagoDetNuevo.Remove(oNuevoExistente);
                }
            }

            // Todas las formas de pago que quedaron en la lista nueva, son nuevas y por tanto se agregan al listado de cambios
            foreach (var oNuevo in oPagoDetNuevo)
                oCambios.Add(new ObjetoEstatus<VentaPagoDetalle>("", Cat.TiposDeAfectacion.Agregar, oNuevo));

            return oCambios;
        }

        #endregion

        #region [ Públicos ]

        public void EjecutarAccesoDeTeclado(Keys eTecla)
        {
            switch (eTecla)
            {
                case Keys.F6:
                    this.ctlCajaGeneral.ActualizarCaja();
                    break;
                case Keys.F9:
                    this.ctlCajaGeneral.CambiarOpcion("tbpVentasPorCobrar");
                    break;
            }
        }

        #endregion
    }
}
