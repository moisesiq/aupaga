using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    static class Program
    {
        // Constantes globales del sistema
        public const string NombreActualizador = "Actualizador.exe";

        // Se preparan procesos asíncronos
        public static Dictionary<string, System.Threading.Timer> oTimers = new Dictionary<string, System.Threading.Timer>();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Se muestra la pantalla de iniciando..
            var frmIniciando = new Iniciando();
            frmIniciando.Show();
            Application.DoEvents();
            
            // Se inicializan parámetros de la aplicación
            var oIni = Proc.InicializarAplicacion();
            if (oIni.Error)
            {
                UtilLocal.MensajeAdvertencia(oIni.Mensaje + "\n\nTheos encontró un problema al iniciar la aplicación y por lo tanto se cerrará.");
                frmIniciando.Close();
                return;
            }

            // Se verifica si hay una actualización
            if (Util.Logico(System.Configuration.ConfigurationManager.AppSettings["VerNuevaVersion"]))
            {
                if (Proc.VerNuevaVersion())
                {
                    frmIniciando.Close();
                    return;
                }
            }

            // Se abre el formulario principal
            frmIniciando.Close();
            Application.Run(Principal.Instance);

            Proc.FinalizarAplicacion();
        }
    }

    static class GlobalClass
    {
        public const string RutaImg = "Img";
        
        public const string FormatoEntero = "###,###,###,##0";
        public const string FormatoDecimal = "###,###,###,##0.00";
        public const string FormatoMoneda = "$###,###,###,##0.00";
        public const string FormatoPorcentaje = "##0.00%";

        public const string FormatoFecha = "dd/MM/yyyy";
        public const string FormatoFechaHora = "dd/MM/yyyy HH:mm:ss";

        public const int TiempoNotificacion = (2 * 1000);

        public const int Puerto = 32000;
                
        private static String _NombreApp = Application.ProductName;
        private static String _NombreTienda = string.Empty;
        private static int _SucursalID = Util.Entero(Properties.Settings.Default.SucursalID);
        private static UsuarioSis _UsuarioGlobal = new UsuarioSis();
        private static ConfiguracionSis _ConfiguracionGlobal = new ConfiguracionSis();
        
        public static String NombreApp { get { return _NombreApp; } }
        public static String NombreTienda { set { _NombreTienda = value; } get { return _NombreTienda; } }
        public static int SucursalID { get { return _SucursalID; } set { _SucursalID = value; } }
        public static UsuarioSis UsuarioGlobal { get { return _UsuarioGlobal; } set { _UsuarioGlobal = value; } }
        public static ConfiguracionSis ConfiguracionGlobal { get { return _ConfiguracionGlobal; } set { _ConfiguracionGlobal = value; } }
        public static int ProcesosActivos { get; set; }

        public static string Modo { get; private set; }
        public static bool Produccion { get; private set; }
        public static bool Pruebas { get; private set; }
        public static bool Desarrollo { get; private set; }
        
        static GlobalClass()
        {
            GlobalClass.Modo = Properties.Settings.Default.Modo;
            switch (GlobalClass.Modo.ToLower())
            {
                case "prod": GlobalClass.Produccion = true; break;
                case "pruebas": GlobalClass.Pruebas = true; break;
                default: GlobalClass.Desarrollo = true; break;
            }

            // Se llenan valores predefinidos para la ventana "Cargando"
            Cargando.Animacion = Properties.Resources.Cargando;
        }
    }
}
