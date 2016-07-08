using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Data;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public static class Proc
    {
        static ServidorTcp oEscucha;

        #region [ Aplicación ]

        public static ResAcc InicializarAplicacion()
        {
            // Se llena la cadena de conexión
            string sCadenaDeConexion = System.Configuration.ConfigurationManager.ConnectionStrings[GlobalClass.Modo].ConnectionString;
            string sUsuario = sCadenaDeConexion.Extraer("user id=", ";");
            string sContrasenia = sCadenaDeConexion.Extraer("password=", ";");
            if (sUsuario.Length > 0)
                sCadenaDeConexion = sCadenaDeConexion.Replace(sUsuario, UtilLocal.Desencriptar(sUsuario));
            if (sContrasenia.Length > 0)
                sCadenaDeConexion = sCadenaDeConexion.Replace(sContrasenia, UtilLocal.Desencriptar(sContrasenia));
            Datos.CadenaDeConexion = sCadenaDeConexion;

            // Se cargan parámetros de configuración iniciales
            var oResCon = Proc.loadConfiguraciones();
            if (oResCon.Error)
                return oResCon;

            // Se verifica la hora del sistema con la hora de sql
            if (Math.Abs((DateTime.Now - UtilDatos.FechaServidorDeDatos()).Minutes) >= 5)
                return new ResAcc(false, "Favor de actualizar la fecha y hora de la computadora. Informar a Soporte Técnico.");

            return new ResAcc(true);
        }

        public static void FinalizarAplicacion()
        {
            // Se limpian procesos asíncronos, si hubiera
            foreach (var oTimer in Program.oTimers)
            {
                oTimer.Value.Change(Timeout.Infinite, Timeout.Infinite);
            }

            // Se detiene el socket de escucha Tcp
            if (Proc.oEscucha != null)
                Proc.oEscucha.Detener();

            // Se manda cerrar la ventana de cargando, por si quedara alguna abierta
            Cargando.Cerrar();
        }

        #endregion

        #region [ Uso interno ]

        private static ResAcc loadConfiguraciones()
        {
            try
            {
                var configuraciones = Datos.GetListOf<Configuracion>(c => c.ConfiguracionID > 0);
                foreach (var configuracion in configuraciones)
                {
                    switch (configuracion.Nombre)
                    {
                        case "IVA":
                            GlobalClass.ConfiguracionGlobal.IVA = Util.Decimal(configuracion.Valor);
                            break;

                        case "pathImagenes":
                            GlobalClass.ConfiguracionGlobal.pathImagenes = Properties.Settings.Default.RutaImagenes.ToString();
                            break;

                        case "pathImagenesMovimientos":
                            // Este parámetro ya no se usará. Moi 07/05/2015
                            // GlobalClass.ConfiguracionGlobal.pathImagenesMovimientos = Properties.Settings.Default.RutaImagenesMovimientos.ToString();
                            break;

                        default:
                            break;
                    }
                }

                GlobalClass.ConfiguracionGlobal.pathReportes = string.Format("{0}{1}", System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "\\Reportes\\");
            }
            catch (EntityException ex)
            {
                return new ResAcc(false, "Error al conectarse al servidor de datos.\n" + ex.MensajeDeError());
            }
            catch (Exception ex)
            {
                return new ResAcc(false, ex.MensajeDeError());
            }
            return new ResAcc(true);
        }

        #endregion

        /*
        public static Form MostrarPantallaIniciando()
        {
            var frmIniciando = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                Size = new System.Drawing.Size(700, 400),
                StartPosition = FormStartPosition.CenterScreen,
            };
            var lblMensaje = new Label()
            {
                BackColor = System.Drawing.Color.FromArgb(58, 79, 109),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 14),
                Text = "Theos está iniciando..",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            frmIniciando.Controls.Add(lblMensaje);
            frmIniciando.Show();
            Application.DoEvents();
        }
        */

        public static bool VerNuevaVersion()
        {
            string sRutaActualizacion = Util.AgregarSeparadorDeCarpeta(Config.Valor("Actualizacion.RutaArchivos"));
            string sNombreExe = AppDomain.CurrentDomain.FriendlyName;
            string sRutaEjecutable = (sRutaActualizacion + sNombreExe);

            // Se verifica si existe el archivo ejecutable
            if (!File.Exists(sRutaEjecutable))
                return false;

            // Se verifica si la versión es más nueva
            bool bActualizar = false;
            var oInfoVer = FileVersionInfo.GetVersionInfo(sRutaEjecutable);
            if (Application.ProductVersion != oInfoVer.ProductVersion)
            {
                var aVerAct = Application.ProductVersion.Split('.');
                var aVerNueva = oInfoVer.ProductVersion.Split('.');
                for (int i = 0; i < 4; i++)
                {
                    if (Util.Entero(aVerAct[i]) < Util.Entero(aVerNueva[i]))
                    {
                        bActualizar = true;
                        break;
                    }
                }
            }


            if (bActualizar)
            {
                if (UtilLocal.MensajePregunta(string.Format("Hay una versión más nueva de la aplicación ({0})\n¿Deseas cerrar el sistema y aplicar la actualización?"
                    , oInfoVer.ProductVersion)) != DialogResult.Yes)
                    return false;

                // Se abre el actualizador y se cierra el sistema
                string sRutaAp = UtilLocal.RutaAplicacion();
                if (File.Exists(sRutaAp + Program.NombreActualizador))
                {
                    Process.Start(sRutaAp + Program.NombreActualizador, string.Format(" -t ap -r \"{0}\"", sRutaActualizacion));
                    return true;
                }
                else
                {
                    UtilLocal.MensajeAdvertencia("No se encontró el archivo del Actualizador.");
                }
            }

            return false;
        }

        public static void InicializarSesion(int iSucursalID, Usuario oUsuario)
        {
            // Se llenan datos globales
            GlobalClass.SucursalID = iSucursalID;
            GlobalClass.NombreTienda = Datos.GetEntity<Sucursal>(s => s.SucursalID == iSucursalID && s.Estatus).NombreSucursal;
            // Se llenan los datos del Usuario
            UtilLocal.LlenarUsuarioGlobal(oUsuario);
            // Se llena el título
            Principal.Instance.LlenarTitulo();
            // Se llenan los datos en la librería de TheosProc,
            Theos.SucursalID = iSucursalID;
            Theos.UsuarioID = oUsuario.UsuarioID;
            Theos.Iva = GlobalClass.ConfiguracionGlobal.IVA;
            Theos.RutaImagenes = GlobalClass.ConfiguracionGlobal.pathImagenes;
            Theos.Titulo = GlobalClass.NombreApp;

            //Se verifica si el usuario ya se ha logueado este dia para tomar asistencia
            var fechaHora = UtilDatos.FechaServidorDeDatos();
            if (!Datos.Exists<UsuarioAsistencia>(u => u.AccesoUsuarioID == oUsuario.UsuarioID && EntityFunctions.TruncateTime(u.FechaHora) == fechaHora.Date))
            {//Si no, se registra
                UsuarioAsistencia oAsistencia = new UsuarioAsistencia()
                {
                    AccesoUsuarioID = oUsuario.UsuarioID,
                    SucursalID = GlobalClass.SucursalID,
                    FechaHora = fechaHora
                };
                Datos.Guardar<UsuarioAsistencia>(oAsistencia);
            }

            // Se registra la ip acutal del usuario
            string sIp = Util.IpLocal();
            if (oUsuario.Ip != sIp)
            {
                oUsuario.Ip = sIp;
                Datos.Guardar<Usuario>(oUsuario);
            }

            // Se inicializa el socket de escucha, si aplica
            if (oUsuario.Alerta9500.Valor())
            {
                //
                if (Proc.oEscucha != null)
                    Proc.oEscucha.Detener();
                //
                Proc.oEscucha = new ServidorTcp(GlobalClass.Puerto);
                Proc.oEscucha.ConexionRecibida += oEscucha_ConexionRecibida;
                
                Proc.oEscucha.Escuchar();

                // Proc.oEscucha.Probar();
            }
            
            // Se configuran los recordatorios para Pedidos a Proveedores, si aplica
            if (oUsuario.AlertaPedidos.Valor())
            {
                DateTime dManiana = DateTime.Now.Date.AddDays(1);
                var oAlertas = Datos.GetListOf<ProveedorEventoCalendario>(c => c.Fecha < dManiana && !c.Revisado);
                foreach (var oReg in oAlertas)
                {
                    if (oReg.Fecha < DateTime.Now)
                        Admon.MostrarRecordatorioPedidos(oReg.ProveedorEventoCalendarioID);
                    else
                        Program.oTimers.Add("AlertaPedido" + Program.oTimers.Count.ToString(), new System.Threading.Timer(new TimerCallback(Admon.MostrarRecordatorioPedidos)
                            , oReg.ProveedorEventoCalendarioID, (int)(oReg.Fecha - DateTime.Now).TotalMilliseconds, Timeout.Infinite));
                }
            }

            // Se verifica la alterta de traspasos
            if (oUsuario.AlertaTraspasos.Valor())
            {
                if (Datos.Exists<MovimientoInventarioTraspasoContingencia>(
                    c => c.MovimientoInventarioEstatusContingenciaID == Cat.TraspasosContingenciasEstatus.NoSolucionado))
                    UtilLocal.MensajeAdvertencia("Existen conflictos de traspasos sin resolver.");
            }

            // Se configuran los recordatorios para Cobros a Clientes, si aplica
            if (oUsuario.AlertaCalendarioClientes.Valor())
            {
                Eventos.Instance.Show();
            }


            // Se configuran los recordatorios para conteos de inventario, si aplica
            if (Datos.Exists<InventarioUsuario>(c => c.InvUsuarioID == GlobalClass.UsuarioGlobal.UsuarioID))
            {
                Asincrono.ProcesoHora(Principal.Instance.MostrarMensaje, new DosVal<string, string>("Recordatorio de Inventario", "Te recordamos realizar el inventario."), 9, 10);
                Asincrono.ProcesoHora(UtilLocal.VerInventarioConteoPendiente, null, 18, 00);
                // Asincrono.ProcesoHora(UtilLocal.VerInventarioConteoPendiente, null, 13, 16);
            }
        }

        #region [ Mensajes Tcp ]

        public class MensajesTcp
        {
            public const string Alerta9500 = "01";
            public const string DevolucionFacturaCreditoAnt = "02";
        }

        public static void EnviarMensajeTcp(string sEquipo, string sCodigo, string sMensaje)
        {
            try
            {
                int iPuerto = GlobalClass.Puerto;
                var oTcpCli = new TcpClient(sEquipo, iPuerto);
                var oStream = oTcpCli.GetStream();
                var oMensaje = UTF8Encoding.UTF8.GetBytes(sCodigo + sMensaje);
                oStream.Write(oMensaje, 0, oMensaje.Length);
                oStream.Close(1000 * 1);
            }
            catch (Exception oEx)
            {
                UtilLocal.MensajeAdvertencia("Error al mandar un mensaje a través de Ip.\n\n" + oEx.Message + "\n" + Util.Cadena(oEx.InnerException));
            }
        }

        private static void oEscucha_ConexionRecibida(Socket oSocket, string sMensaje)
        {
            if (string.IsNullOrEmpty(sMensaje)) return;

            string sCodigo = sMensaje.Substring(0, 2);
            sMensaje = sMensaje.Substring(2);

            switch (sCodigo)
            {
                case MensajesTcp.Alerta9500:
                    Util.MensajeInformacion(sMensaje, "Notificación");
                    break;
                case MensajesTcp.DevolucionFacturaCreditoAnt:
                    int iDevolucionID = Util.Entero(sMensaje);
                    VentasLoc.MostrarAvisoDevolucionFacturaCreditoAnt(iDevolucionID);
                    break;
            }
        }

        #endregion
    }
}
