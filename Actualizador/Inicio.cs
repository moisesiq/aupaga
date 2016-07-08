using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Actualizador
{
    static class Inicio
    {
        public const string ScriptAd = "Act.cmd";

        public static string RutaAct { get; set; }
        public static string RutaLocal { get; set; }
        public static string Ejecutable { get; set; }
        public static bool ActualizacionCorrecta { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] aArgumentos)
        {
            // Se procesan los parámetros
            var oArgs = UtilLocal.ProcesarArgumentos(aArgumentos);
            if (oArgs.ContainsKey("h"))
            {
                string sAyuda = @"
                        Programa utilizado para actualizar la aplicación Theos

                        Parámetros:
                        -h: Mostrar esta ventana de ayuda
                        -t: Tipo de apertura
                            ap: Actualizar aplicación Theos
                            img: Actualizar imágenes
                        -r: Ruta donde se encuentran los archivos de actualización, tanto de 'ap' como de 'img'
                        -exe: Nombre del archivo ejecutable de Theos
                    ";
                MessageBox.Show(sAyuda, "Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Se verifican los parámetros
            if (!oArgs.ContainsKey("t") || !oArgs.ContainsKey("r"))
            {
                MessageBox.Show("No se ha especificado la ruta de la Actualización.", "Parámetros incorrectos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sTipoAc = oArgs["t"].ToLower();
            // Se corrige un caso extraño que se da al obtener el argumento
            if (oArgs["r"].EndsWith("\""))
                oArgs["r"] = oArgs["r"].Substring(0, oArgs["r"].Length - 1);
            Inicio.RutaAct = UtilLocal.AgregarSeparadorDeCarpeta(oArgs["r"]);

            Inicio.RutaLocal = UtilLocal.AgregarSeparadorDeCarpeta(Directory.GetCurrentDirectory());
            // Inicio.RutaLocal = @"C:\tmp\CR\Ejec\";  // Para pruebas
            Inicio.Ejecutable = (oArgs.ContainsKey("exe") ? oArgs["exe"] : "THEOS.exe");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Se ejecuta la acción, según el parámetros específicado
            if (sTipoAc == "ap")
            {
                Application.Run(new Principal());

                // Se intenta abrir la aplicación principal (Theos)
                if (Inicio.ActualizacionCorrecta)
                    Process.Start(Inicio.RutaLocal + Inicio.Ejecutable);
            }
            else
            {
                // Se obtiene la ruta local de las imágenes
                string sConfig = (Inicio.Ejecutable + ".config");
                if (!File.Exists(sConfig))
                {
                    UtilLocal.MensajeError("Error al abrir el archivo de configuración.", "Error");
                    return;
                }

                string sRutaImg = UtilLocal.ObtenerValorDeXml(sConfig, "//configuration/applicationSettings/Refaccionaria.App.Properties.Settings/setting[@name='RutaImagenes']");
                if (sRutaImg == null)
                {
                    UtilLocal.MensajeError("Error al acceder al parámetro de 'RutaImagenes' del archivo de configuración.", "Error");
                    return;
                }
                sRutaImg = UtilLocal.AgregarSeparadorDeCarpeta(sRutaImg);

                // sRutaImg = new System.Configuration.AppSettingsReader().GetValue("RutaImagenes", typeof(string)).ToString();
                var frmActImg = new ActImagenes() { RutaImgLocal = sRutaImg };
                Application.Run(frmActImg);
            }
        }
    }
}
