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

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public static class Proc
    {
        static ServidorTcp oEscucha;

        #region [ Aplicación ]

        public static void InicializarAplicacion()
        {
            // Se llena la cadena de conexión
            string sCadenaDeConexion = System.Configuration.ConfigurationManager.ConnectionStrings[GlobalClass.Modo].ConnectionString;
            string sUsuario = sCadenaDeConexion.Extraer("user id=", ";");
            string sContrasenia = sCadenaDeConexion.Extraer("password=", ";");
            if (sUsuario.Length > 0)
                sCadenaDeConexion = sCadenaDeConexion.Replace(sUsuario, Helper.Desencriptar(sUsuario));
            if (sContrasenia.Length > 0)
                sCadenaDeConexion = sCadenaDeConexion.Replace(sContrasenia, Helper.Desencriptar(sContrasenia));
            ModelHelper.CadenaDeConexion = sCadenaDeConexion;
        }

        public static void FinalizarAplicacion()
        {
            // Se limpian procesos asíncronos, si hubiera
            foreach (var oTimer in Program.oTimers)
            {
                oTimer.Value.Change(Timeout.Infinite, Timeout.Infinite);
            }

            // Se detiene el socket de escucha Tcp
            Proc.oEscucha.Detener();
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
            string sRutaActualizacion = Helper.AgregarSeparadorDeCarpeta(Config.Valor("Actualizacion.RutaArchivos"));
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
                    if (Helper.ConvertirEntero(aVerAct[i]) < Helper.ConvertirEntero(aVerNueva[i]))
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
            GlobalClass.NombreTienda = General.GetEntity<Sucursal>(s => s.SucursalID == iSucursalID && s.Estatus).NombreSucursal;
            // Se llenan los datos del Usuario
            UtilDatos.LlenarUsuarioGlobal(oUsuario);
            // Se llena el título
            Principal.Instance.LlenarTitulo();

            //Se verifica si el usuario ya se ha logueado este dia para tomar asistencia
            var fechaHora = UtilDatos.FechaServidorDeDatos();
            if (!General.Exists<UsuarioAsistencia>(u => u.AccesoUsuarioID == oUsuario.UsuarioID && EntityFunctions.TruncateTime(u.FechaHora) == fechaHora.Date))
            {//Si no, se registra
                UsuarioAsistencia oAsistencia = new UsuarioAsistencia()
                {
                    AccesoUsuarioID = oUsuario.UsuarioID,
                    SucursalID = GlobalClass.SucursalID,
                    FechaHora = fechaHora
                };
                Guardar.Generico<UsuarioAsistencia>(oAsistencia);
            }

            // Se registra la ip acutal del usuario
            string sIp = Helper.IpLocal();
            if (oUsuario.Ip != sIp)
            {
                oUsuario.Ip = sIp;
                Guardar.Generico<Usuario>(oUsuario);
            }

            // Se inicializa el socket de escucha, si aplica
            if (oUsuario.Alerta9500.Valor())
            {
                Proc.oEscucha = new ServidorTcp(GlobalClass.Puerto);
                Proc.oEscucha.Escuchar();
                Proc.oEscucha.ConexionRecibida += oEscucha_ConexionRecibida;
            }
            
            // Se configuran los recordatorios para Pedidos a Proveedores, si aplica
            if (oUsuario.AlertaPedidos.Valor())
            {
                DateTime dManiana = DateTime.Now.Date.AddDays(1);
                var oAlertas = General.GetListOf<ProveedorEventoCalendario>(c => c.Fecha < dManiana && !c.Revisado);
                foreach (var oReg in oAlertas)
                {
                    if (oReg.Fecha < DateTime.Now)
                        AdmonProc.MostrarRecordatorioPedidos(oReg.ProveedorEventoCalendarioID);
                    else
                        Program.oTimers.Add("AlertaPedido" + Program.oTimers.Count.ToString(), new System.Threading.Timer(new TimerCallback(AdmonProc.MostrarRecordatorioPedidos)
                            , oReg.ProveedorEventoCalendarioID, (int)(oReg.Fecha - DateTime.Now).TotalMilliseconds, Timeout.Infinite));
                }
            }

            // Se configuran los recordatorios para Cobros a Clientes, si aplica
            if (oUsuario.AlertaCalendarioClientes.Valor())
            {
                DateTime dManiana = DateTime.Now.Date.AddDays(1);
                var oAlertas = General.GetListOf<ClienteEventoCalendario>(c => c.Fecha < dManiana && !c.Revisado);
                foreach (var oReg in oAlertas)
                {
                    if (oReg.Fecha < DateTime.Now)
                        AdmonProc.MostrarRecordatorioClientes(oReg.ClienteEventoCalendarioID);
                    else
                        Program.oTimers.Add("AlertaPedido" + Program.oTimers.Count.ToString(), new System.Threading.Timer(new TimerCallback(AdmonProc.MostrarRecordatorioClientes)
                            , oReg.ClienteEventoCalendarioID, (int)(oReg.Fecha - DateTime.Now).TotalMilliseconds, Timeout.Infinite));
                }
            }


            // Se configuran los recordatorios para conteos de inventario, si aplica
            /*
            if (General.Exists<InventarioUsuario>(c => c.InvUsuarioID == GlobalClass.UsuarioGlobal.UsuarioID))
            {
                Asincrono.ProcesoHora(Principal.Instance.MostrarMensaje, new DosVal<string, string>("Recordatorio de Inventario", "Te recordamos realizar el inventario."), 9, 10);
                // Asincrono.RecordatorioMensaje("Recordatorio de Inventario", "Te recordamos realizar el inventario.", 9, 10);
                Asincrono.ProcesoHora(Principal.Instance.VerInventarioConteoPendiente, null, 11, 47);
            }
            */
        }

        #region [ Mensajes Tcp ]

        public class MensajesTcp
        {
            public const string Alerta9500 = "01";
        }

        public static void EnviarMensajeTcp(string sEquipo, string sCodigo, string sMensaje)
        {
            try
            {
                var oTcpCli = new TcpClient(sEquipo, GlobalClass.Puerto);
                var oStream = oTcpCli.GetStream();
                var oMensaje = UTF8Encoding.UTF8.GetBytes(sCodigo + sMensaje);
                oStream.Write(oMensaje, 0, oMensaje.Length);
                oStream.Close(1000 * 1);
            }
            catch
            {

            }
        }

        private static void oEscucha_ConexionRecibida(Socket oSocket, string sMensaje)
        {
            string sCodigo = sMensaje.Substring(0, 2);
            sMensaje = sMensaje.Substring(2);

            switch (sCodigo)
            {
                case MensajesTcp.Alerta9500:
                    Helper.MensajeInformacion(sMensaje, "Notificación");
                    break;
            }
        }

        #endregion
    }
}
