using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace Actualizador
{
    public static class UtilLocal
    {
        public static Dictionary<string, string> ProcesarArgumentos(string[] aArgumentos)
        {
            var oArgumentos = new Dictionary<string, string>();
            for (int i = 0; i < aArgumentos.Length; i++)
            {
                if (aArgumentos[i].Contains("-"))
                    oArgumentos.Add(aArgumentos[i].Replace("-", ""), (aArgumentos.Length > (i + 1) ? aArgumentos[++i] : ""));
            }

            return oArgumentos;
        }

        public static string ObtenerValorDeXml(string sArchivoXml, string sXpath)
        {
            var oXml = new XmlDocument();
            oXml.Load(sArchivoXml);
            var oNodo = oXml.SelectSingleNode(sXpath);
            return (oNodo == null ? null : oNodo.InnerText);
        }

        public static Dictionary<string, string> LeerArchivoParametros(string sArchivo)
        {
            if (!File.Exists(sArchivo))
                return null;

            var aParametros = new Dictionary<string, string>();
            var aLineas = File.ReadAllLines(sArchivo);
            foreach (string sLinea in aLineas)
            {
                var aValor = sLinea.Split('=');
                if (aValor.Length > 1)
                    aParametros[aValor[0]] = aValor[1];
            }

            return aParametros;
        }

        public static void GuardarValorArchivoParametros(string sArchivo, string sParametro, string sValor)
        {
            var oParametros = UtilLocal.LeerArchivoParametros(sArchivo);
            if (oParametros == null)
                oParametros = new Dictionary<string, string>();
            oParametros[sParametro] = sValor;
            // Se forman las líneas
            var oLineas = new List<string>();
            foreach (var oReg in oParametros)
                oLineas.Add(oReg.Key + "=" + oReg.Value);
            // Se escriben todos los datos
            File.WriteAllLines(sArchivo, oLineas);
        }

        public static DateTime FechaHora(object oValor)
        {
            string sValor = (oValor == null ? "" : oValor.ToString());
            DateTime dValor;
            bool bExito = DateTime.TryParse(sValor, CultureInfo.CurrentCulture, DateTimeStyles.None, out dValor);
            return dValor;
        }

        public static string AgregarSeparadorDeCarpeta(string sRuta)
        {
            if (!sRuta.EndsWith(Path.DirectorySeparatorChar.ToString()))
                sRuta += Path.DirectorySeparatorChar;
            return sRuta;
        }

        #region [ Ventanas de Mensaje ]

        public static DialogResult MensajeError(string sMensaje, string sTitulo)
        {
            return MessageBox.Show(sMensaje, sTitulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult MensajeAdvertencia(string sMensaje, string sTitulo)
        {
            return MessageBox.Show(sMensaje, sTitulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static DialogResult MensajeInformacion(string sMensaje, string sTitulo)
        {
            return MessageBox.Show(sMensaje, sTitulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult MensajePregunta(string sMensaje, string sTitulo)
        {
            return MessageBox.Show(sMensaje, sTitulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        }

        #endregion
    }
}
