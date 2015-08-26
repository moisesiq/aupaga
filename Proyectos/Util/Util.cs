using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Globalization;

namespace LibUtil
{
    public static class Util
    {
        public const string FormatoEntero = "###,###,###,##0";
        public const string FormatoDecimal = "###,###,###,##0.00";
        public const string FormatoMoneda = "$###,###,###,##0.00";
        public const string FormatoPorcentaje = "##0.00%";
        public const string FormatoFecha = "dd/MM/yyyy";
        public const string FormatoFechaHora = "dd/MM/yyyy HH:mm:ss";

        #region [ Conversiones ]

        public static int Entero(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            int iValor;
            int.TryParse(sValor, out iValor);
            return iValor;
        }

        public static decimal Decimal(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            decimal mValor;
            decimal.TryParse(sValor, out mValor);
            return mValor;
        }

        public static string Cadena(object Valor)
        {
            return (Valor == null ? "" : Valor.ToString());
        }

        public static DateTime FechaHora(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            DateTime dValor;
            bool bExito = DateTime.TryParse(sValor, CultureInfo.CurrentCulture, DateTimeStyles.None, out dValor);
            return dValor;
        }

        public static bool Logico(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            bool iValor;
            bool.TryParse(sValor, out iValor);
            return iValor;
        }

        #endregion

        #region [ Extensiones Cadena ]

        public static string Derecha(this string sCadena, int iCaracteres)
        {
            return sCadena.Substring(((sCadena.Length - iCaracteres) < 0 ? 0 : (sCadena.Length - iCaracteres)));
        }

        public static string Izquierda(this string sCadena, int iCaracteres)
        {
            if ((sCadena.Length - iCaracteres) < 0)
                return sCadena.Substring(0);
            else
                return sCadena.Substring(0, iCaracteres);
        }

        public static string RellenarCortarIzquierda(this string sCadena, int iPosiciones)
        {
            if (sCadena == null) return null;
            return sCadena.PadLeft(iPosiciones).Substring(0, iPosiciones);
        }

        public static string RellenarCortarDerecha(this string sCadena, int iPosiciones)
        {
            if (sCadena == null) return null;
            return sCadena.PadRight(iPosiciones).Substring(0, iPosiciones);
        }

        #endregion

        #region [ Extensiones - Corrección Nullables ]

        public static bool Valor(this bool? bVar)
        {
            return Util.Logico(bVar);
        }

        public static int Valor(this int? iVar)
        {
            return Util.Entero(iVar);
        }

        public static decimal Valor(this decimal? mVar)
        {
            return Util.Decimal(mVar);
        }

        public static DateTime Valor(this DateTime? dVar)
        {
            return Util.FechaHora(dVar);
        }
        
        public static string Valor(this string sVar)
        {
            return Util.Cadena(sVar);
        }

        #endregion

        #region [ Reflexion ]

        public static string JsonSimple(object oObjeto)
        {
            // Se obtienen las propiedades 
            PropertyInfo[] oPropiedades = oObjeto.GetType().GetProperties();
            string sTipo;
            var oJson = new StringBuilder();
            foreach (PropertyInfo oPropiedad in oPropiedades)
            {
                Type oTipo = oPropiedad.PropertyType;
                // Si es tipo "Nullable", se obtiene el tipo interno*
                if (oTipo.IsGenericType && (oTipo.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    oTipo = oTipo.GetGenericArguments()[0];
                // Si no es tipo por valor, no se considera
                sTipo = oTipo.Name.ToLower();
                if (!(
                    sTipo == "string"
                    || sTipo == "int32"
                    || sTipo == "decimal"
                    || sTipo == "boolean"
                    || sTipo == "datetime"
                    )) continue;
                // Se agrega a la cadena Json
                var oValor = oPropiedad.GetValue(oObjeto, null);
                if (sTipo == "string" || sTipo == "datetime")
                    oValor = ("\"" + Util.Cadena(oValor) + "\"");
                else if (sTipo == "boolean")
                    oValor = (Util.Logico(oValor) ? "true" : "false");
                oJson.AppendFormat("\"{0}\": {1},", oPropiedad.Name, oValor);
            }
            oJson.Remove(oJson.Length - 1, 1);

            return ("{" + oJson.ToString() + "}");
        }

        public static string ListaJsonSimple(IEnumerable oLista)
        {
            var oJson = new StringBuilder();
            foreach (var oReg in oLista)
            {
                oJson.AppendFormat("{0},", Util.JsonSimple(oReg));
            }
            oJson.Remove(oJson.Length - 1, 1);

            return ("[" + oJson.ToString() + "]");
        }

        #endregion

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
