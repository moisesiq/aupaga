using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public static class UtilDatos
    {
        public static void LlenarUsuarioGlobal(Usuario oUsuario)
        {
            GlobalClass.UsuarioGlobal = new UsuarioSis();
            GlobalClass.UsuarioGlobal.UsuarioID = oUsuario.UsuarioID;
            GlobalClass.UsuarioGlobal.NombreUsuario = oUsuario.NombreUsuario;
            GlobalClass.UsuarioGlobal.NombrePersona = oUsuario.NombrePersona;
            GlobalClass.UsuarioGlobal.Perfiles = General.GetListOf<UsuarioPerfilesView>(up => up.UsuarioID.Equals(oUsuario.UsuarioID));

            var ppv = General.GetListOf<PerfilPermisosView>().ToList();
            var permisos = new List<PerfilPermisosView>();

            foreach (var perfil in GlobalClass.UsuarioGlobal.Perfiles)
            {
                var items = ppv.Where(p => p.PerfilID == perfil.PerfilID);
                foreach (var item in items)
                    permisos.Add(item);
            }

            GlobalClass.UsuarioGlobal.Permisos = permisos;
        }

        public static string VentaPagoFormasDePago(List<VentaPagoDetalle> oPagos)
        {
            string sCadena = "1-2-3-4-5";
            foreach (var oPago in oPagos)
            {
                // Si es pago negativo (devolución), no se cuenta
                // if (oPago.Importe <= 0) continue;

                switch (oPago.TipoFormaPagoID)
                {
                    case Cat.FormasDePago.Efectivo:
                        sCadena = sCadena.Replace("1", "EF");
                        break;
                    case Cat.FormasDePago.Cheque:
                        sCadena = sCadena.Replace("2", "CH");
                        break;
                    case Cat.FormasDePago.Tarjeta:
                        sCadena = sCadena.Replace("3", "TC");
                        break;
                    case Cat.FormasDePago.Transferencia:
                        sCadena = sCadena.Replace("4", "TR");
                        break;
                    case Cat.FormasDePago.Vale:
                        sCadena = sCadena.Replace("5", "NC");
                        break;
                }
            }

            return Regex.Replace(sCadena, @"\d", "  ");
        }

        public static string VentaPagoFormasDePago(int iVentaPagoID)
        {
            var oPagos = General.GetListOf<VentaPagoDetalle>(q => q.VentaPagoID == iVentaPagoID && q.Estatus);
            return UtilDatos.VentaPagoFormasDePago(oPagos);
        }

        public static string VentaPagoVales(int iVentaPagoID)
        {
            string sVales = "";
            var oDet = General.GetListOf<VentaPagoDetalle>(c => c.VentaPagoID == iVentaPagoID && c.TipoFormaPagoID == Cat.FormasDePago.Vale && c.Estatus);
            foreach (var oReg in oDet)
                sVales += string.Format(", {0}({1})", oReg.NotaDeCreditoID, oReg.Importe.ToString(GlobalClass.FormatoMoneda));
            sVales = (sVales == "" ? "" : sVales.Substring(2));
            return sVales;
        }

        public static string VentaDevolucionFormaDePago(int iFormaDePagoID)
        {
            switch (iFormaDePagoID)
            {
                case Cat.FormasDePago.Efectivo: return "EF-  -  -  ";
                case Cat.FormasDePago.Cheque: return "  -CH-  -  ";
                case Cat.FormasDePago.Tarjeta: return "  -  -TC-  ";
                case Cat.FormasDePago.Vale: return "  -  -  -NC";
            }
            return "";
        }

        public static decimal PartePrecioDeVenta(PartePrecio oPartePrecio, int iListaDePrecios)
        {
            switch (iListaDePrecios)
            {
                case 1:
                    return oPartePrecio.PrecioUno.Valor();
                case 2:
                    return oPartePrecio.PrecioDos.Valor();
                case 3:
                    return oPartePrecio.PrecioTres.Valor();
                case 4:
                    return oPartePrecio.PrecioCuatro.Valor();
                case 5:
                    return oPartePrecio.PrecioCinco.Valor();
            }
            return 0;
        }

        public static decimal VentaComisionCliente(int iVentaID, int iComisionistaID)
        {
            // Se calcula el importe de la comisión
            var oVentaDetalle = General.GetListOf<VentaDetalle>(q => q.VentaID == iVentaID && q.Estatus);
            var oComisionista = General.GetEntity<Cliente>(q => q.ClienteID == iComisionistaID && q.Estatus);
            if (oComisionista == null) return 0;

            decimal mComision = 0;
            PreciosParte oPrecios;
            foreach (var ParteD in oVentaDetalle)
            {
                oPrecios = new PreciosParte(ParteD.ParteID);
                mComision += (((ParteD.PrecioUnitario + ParteD.Iva) - oPrecios.ObtenerPrecio(oComisionista.ListaDePrecios)) * ParteD.Cantidad);
            }

            return mComision;
        }
        
        public static bool VerCierreDeDaja()
        {
            DateTime dHoy = DateTime.Today;
            var oDia = General.GetEntity<CajaEfectivoPorDia>(q => q.SucursalID == GlobalClass.SucursalID && q.Dia == dHoy && q.Estatus);
            return (oDia != null && oDia.Cierre != null);
        }

        public static DosVal<DateTime, DateTime> FechasDeComisiones(DateTime dBase)
        {
            int iDiaIni = Helper.ConvertirEntero(Config.Valor("Comisiones.DiaInicial"));
            int iDiaFin = Helper.ConvertirEntero(Config.Valor("Comisiones.DiaFinal"));
            int iDiaBase = (int)dBase.DayOfWeek;
            
            var oRes = new DosVal<DateTime, DateTime>();
            oRes.Valor1 = dBase.AddDays((double)(iDiaBase <= iDiaIni ? (iDiaIni - 7 - iDiaBase) : (iDiaIni - iDiaBase)));
            oRes.Valor2 = dBase.AddDays((double)(iDiaBase > (iDiaFin + 1) ? (iDiaFin + 7 - iDiaBase) : (iDiaFin - iDiaBase)));
            
            // Se mandan las horas a los extremos, de 00:00 a 23:59, para que no haya problemas con los filtros
            oRes.Valor1 = oRes.Valor1.Date;
            oRes.Valor2 = oRes.Valor2.Date.AddDays(1).AddSeconds(-1);

            return oRes;
        }

        public static string LeyendaDeVehiculo(int iClienteVehiculoID)
        {
            var oVehiculo = General.GetEntity<ClientesFlotillasView>(c => c.ClienteFlotillaID == iClienteVehiculoID);
            return string.Format("NO.ECO. {0}, {1}, {2} MOD. {3}, {4}, {5}, PLACAS {6} KM {7}",
                oVehiculo.NumeroEconomico, oVehiculo.Marca, oVehiculo.Modelo, oVehiculo.Anio, oVehiculo.Motor, oVehiculo.Tipo, oVehiculo.Placa, oVehiculo.Kilometraje);
        }

        public static List<string> InventarioUsuariosConteoPendiente()
        {
            var oLista = new List<string>();
            DateTime dHoy = DateTime.Now.Date;
            var oConteos = General.GetListOf<InventarioConteo>(c => c.Dia == dHoy && !c.Diferencia.HasValue)
                .Select(c => new { UsuarioID = c.ConteoUsuarioID }).Distinct();
            var oUsuariosSuc = General.GetListOf<InventarioUsuario>(c => c.SucursalID == GlobalClass.SucursalID);
            foreach (var oReg in oConteos)
            {
                if (oUsuariosSuc.Any(c => c.InvUsuarioID == oReg.UsuarioID))
                {
                    var oUsuario = General.GetEntity<Usuario>(c => c.UsuarioID == oReg.UsuarioID);
                    oLista.Add(oUsuario.NombreUsuario);
                }
            }
            return oLista;
        }

        public static string GenerarSugerenciaNombreParte(int iLineaID, int iMarcaID, string sNumeroDeParte)
        {
            var oLinea = General.GetEntity<Linea>(c => c.LineaID == iLineaID && c.Estatus);
            var oMarca = General.GetEntity<MarcaParte>(c => c.MarcaParteID == iMarcaID && c.Estatus);

            return string.Format("{0} {1} {2}", oLinea.Abreviacion, oMarca.Abreviacion, sNumeroDeParte).Trim();
        }

        public static DateTime FechaServidorDeDatos()
        {
            var oLista = General.ExecuteQuery<DateTime>("SELECT GETDATE()", null);
            return oLista.First();
        }

        #region [ Almacenamiento temporal en base de datos ]

        public static void AgregarTemporal(string sProceso, int iId, string sValor)
        {
            var oTemp = new Temporal()
            {
                Proceso = sProceso,
                Identificador = iId,
                Valor = sValor
            };
            Guardar.Generico<Temporal>(oTemp);
        }

        public static string ObtenerTemporal(string sProceso, int iId)
        {
            var oTemp = General.GetEntity<Temporal>(c => c.Proceso == sProceso && c.Identificador == iId);
            return (oTemp == null ? null : oTemp.Valor);
        }

        public static string ObtenerQuitarTemporal(string sProceso, int iVentaID)
        {
            string sLeyenda = UtilDatos.ObtenerTemporal(sProceso, iVentaID);
            UtilDatos.BorrarTemporal(sProceso, iVentaID);

            return sLeyenda;
        }

        public static void BorrarTemporal(string sProceso, int iId)
        {
            var oTemp = General.GetEntity<Temporal>(c => c.Proceso == sProceso && c.Identificador == iId);
            if (oTemp != null)
                Guardar.Eliminar<Temporal>(oTemp);
        }

        #endregion

        #region [ Seguridad ]

        public static ResAcc<Usuario> ValidarObtenerUsuario(List<string> oPermisos, bool bCumplirTodosLosPermisos, string sTitulo)
        {
            var Res = new ResAcc<Usuario>();

            var frmValidar = new ValidarUsuario(oPermisos, bCumplirTodosLosPermisos);
            if (sTitulo != null)
                frmValidar.Text = sTitulo;

            if (frmValidar.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                Res.Respuesta = frmValidar.UsuarioSel;
                Res.Exito = true;
            }
            Res.CodigoDeEstatus = (int)frmValidar.DialogResult;
            frmValidar.Dispose();

            return Res;
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario(List<string> oPermisos, bool bCumplirTodosLosPermisos)
        {
            return UtilDatos.ValidarObtenerUsuario(oPermisos, bCumplirTodosLosPermisos, null);
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario(string sPermiso, string sTitulo)
        {
            return UtilDatos.ValidarObtenerUsuario((sPermiso == null ? null : new List<string>() { sPermiso }), true, sTitulo);
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario(string sPermiso)
        {
            return UtilDatos.ValidarObtenerUsuario(new List<string>() { sPermiso }, true, null);
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario()
        {
            return UtilDatos.ValidarObtenerUsuario(null, false, null);
        }

        public static Usuario ObtenerUsuarioDeContrasenia(string sContrasenia)
        {
            string sContraseniaEnc = Helper.Encriptar(sContrasenia);
            var oUsuario = General.GetEntity<Usuario>(q => q.Contrasenia == sContraseniaEnc && q.Estatus);
            // var oUsuario = General.GetEntity<Usuario>(q => q.Contrasenia == sContrasenia && q.Estatus);
            return oUsuario;
        }
        
        public static ResAcc<bool> ValidarUsuarioPermisos(int iUsuarioID, List<string> oPermisos, bool bCumplirTodos)
        {
            var oUsuarioPer = General.GetListOf<UsuariosPermisosView>(c => c.UsuarioID == iUsuarioID);
            bool bCumplido;
            List<string> oNoCumplidos = new List<string>();
            foreach (string sPermiso in oPermisos)
            {
                bCumplido = false;
                // Se busca el permiso en la lista de permisos del usuario
                foreach (var oPermiso in oUsuarioPer)
                {
                    if (oPermiso.Permiso == sPermiso)
                    {
                        bCumplido = true;
                        break;
                    }
                }
                // Se verifica si el permiso actual se cumplió o no
                if (!bCumplido)
                    oNoCumplidos.Add(sPermiso);
            }

            // Se verifica si se cumplió la validación o no
            if (bCumplirTodos && oNoCumplidos.Count == 0)
            {
                return new ResAcc<bool>(true);
            }
            else if (!bCumplirTodos && oNoCumplidos.Count < oPermisos.Count)
            {
                return new ResAcc<bool>(true);
            }
            else
            {
                // Se obtiene el mensaje de error del primer permiso no cumplido
                string sPermiso = oNoCumplidos[0];
                var oPermiso = General.GetEntity<Permiso>(c => c.NombrePermiso == sPermiso && c.Estatus);
                return new ResAcc<bool>(false, oPermiso.MensajeDeError);
            }
        }

        public static Usuario ObtenerUsuarioIDDeNombreUsuario(string sUsuario)
        {
            return General.GetEntity<Usuario>(q => q.Estatus && q.NombreUsuario == sUsuario);
        }

        public static bool ValidarPermiso(int iUsuarioID, string sPermiso, bool bMostrarMensaje)
        {
            bool bValido = General.Exists<UsuariosPermisosView>(c => c.UsuarioID == iUsuarioID && c.Permiso == sPermiso);
            if (bValido)
            {
                return true;
            }
            else
            {
                if (bMostrarMensaje)
                {
                    var oPermiso = General.GetEntity<Permiso>(c => c.NombrePermiso == sPermiso);
                    if (oPermiso == null)
                        UtilLocal.MensajeError("El Permiso especificado ni siquiera existe. ¡Échame la mano!");
                    else
                        UtilLocal.MensajeAdvertencia(oPermiso.MensajeDeError);
                }
                return false;
            }
        }

        public static bool ValidarPermiso(int iUsuarioID, string sPermiso)
        {
            return UtilDatos.ValidarPermiso(iUsuarioID, sPermiso, false);
        }

        public static bool ValidarPermiso(string sPermiso, bool bMostrarMensaje)
        {
            return UtilDatos.ValidarPermiso(GlobalClass.UsuarioGlobal.UsuarioID, sPermiso, bMostrarMensaje);
        }

        public static bool ValidarPermiso(string sPermiso)
        {
            return UtilDatos.ValidarPermiso(GlobalClass.UsuarioGlobal.UsuarioID, sPermiso, false);
        }
        
        #endregion
        
    }
}
