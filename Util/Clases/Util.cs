using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

namespace LibUtil
{
    public static class Util
    {
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

        public static double Doble(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            double mValor;
            double.TryParse(sValor, out mValor);
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

        public static string ACadena(this bool bValor)
        {
            return (bValor ? "Sí" : "No");
        }

        public static decimal ValorDecimal(this string sValor)
        {
            if (sValor == null)
                return 0;

            sValor = sValor.Replace("$", "");
            sValor = sValor.Replace(",", "");

            return Util.Decimal(sValor);
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

        public static string Subcadena(this string sCadena, int iPosicionInicial, int iPosicionFinal)
        {
            return sCadena.Substring(iPosicionInicial, iPosicionFinal - iPosicionInicial);
        }

        public static string Extraer(this string sCadena, string sDelimitadorInicio, string sDelimitadorFin)
        {
            int iPosIni = sCadena.IndexOf(sDelimitadorInicio, 0);
            if (iPosIni < 0)
                return "";
            iPosIni += sDelimitadorInicio.Length;
            int iPosFin = sCadena.IndexOf(sDelimitadorFin, iPosIni);

            if (iPosFin > iPosIni)
                return sCadena.Subcadena(iPosIni, iPosFin);
            else
                return "";
        }

        public static string PrimeraMayus(this string sCadena)
        {
            return (sCadena.Substring(0, 1).ToUpper() + sCadena.Substring(1).ToLower());
        }

        public static string Repetir(this string sCadena, int iVeces)
        {
            string sRes = "";
            for (int iCont = 0; iCont < iVeces; iCont++)
                sRes += sCadena;
            return sRes;
        }

        public static string SoloNumeric(this string sCadena)
        {
            return Regex.Replace(sCadena, @"[^0-9.-]", "");  // ** Hacer pruebas para comprobar que el RegExp funcione en todos los casos
            string sNueva = "";
            foreach (char sCar in sCadena)
            {
                if (Char.IsDigit(sCar))
                    sNueva += sCar;
            }
            return sNueva;
        }

        public static string Truncar(this string sCadena, int iCaracteres)
        {
            return (sCadena.Substring(0, (iCaracteres > sCadena.Length ? sCadena.Length : iCaracteres)));
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

        #region [ Fecha ]

        public static DateTime DiaPrimero(this DateTime dFecha)
        {
            return dFecha.AddDays((dFecha.Day - 1) * -1);
        }

        public static DateTime DiaUltimo(this DateTime dFecha)
        {
            return dFecha.AddMonths(1).DiaPrimero().AddDays(-1);
        }

        #endregion

        #region [ Numéricos ]

        public static decimal Promedio(params decimal[] aParametros)
        {
            return aParametros.Average();
        }

        public static decimal? DividirONull(decimal? mDividendo, decimal? mDivisor)
        {
            if (mDivisor == 0)
                return null;
            else
                return (mDividendo / mDivisor);
        }

        public static string ImporteALetra(decimal mNumero)
        {
            if ((mNumero / 1000000000000) > 1)
                return "**Desbordamiento numérico**";

            string sLetra = "";

            long lPesos = (long)mNumero;
            long lCentavos = (long)((mNumero - lPesos) * 100);
            string sPesos = lPesos.ToString();
            string sCentavos = lCentavos.ToString();
            sCentavos = "0".Repetir(2 - sCentavos.Length) + sCentavos;
            const string MONEDA = "/100 M. N.";

            string[] aUnidades = new string[9];
            string[] aDieces = new string[9];
            string[] aDecenas = new string[9];
            string[] aCentenas = new string[9];

            /* -------------------------------------------------------------------------- */
            aUnidades[0] = "Uno";
            aUnidades[1] = "Dos";
            aUnidades[2] = "Tres";
            aUnidades[3] = "Cuatro";
            aUnidades[4] = "Cinco";
            aUnidades[5] = "Seis";
            aUnidades[6] = "Siete";
            aUnidades[7] = "Ocho";
            aUnidades[8] = "Nueve";
            /* -------------------------------------------------------------------------- */
            aDieces[0] = "Once";
            aDieces[1] = "Doce";
            aDieces[2] = "Trece";
            aDieces[3] = "Catorce";
            aDieces[4] = "Quince";
            aDieces[5] = "Dieciseis";
            aDieces[6] = "Diecisiete";
            aDieces[7] = "Dieciocho";
            aDieces[8] = "Diecinueve";
            /* -------------------------------------------------------------------------- */
            aDecenas[0] = "Diez";
            aDecenas[1] = "Veinte";
            aDecenas[2] = "Treinta";
            aDecenas[3] = "Cuarenta";
            aDecenas[4] = "Cincuenta";
            aDecenas[5] = "Sesenta";
            aDecenas[6] = "Setenta";
            aDecenas[7] = "Ochenta";
            aDecenas[8] = "Noventa";
            /* -------------------------------------------------------------------------- */
            aCentenas[0] = "Cien";
            aCentenas[1] = "Doscientos";
            aCentenas[2] = "Trecientos";
            aCentenas[3] = "Cuatrocientos";
            aCentenas[4] = "Quinientos";
            aCentenas[5] = "Seiscientos";
            aCentenas[6] = "Setecientos";
            aCentenas[7] = "Ochocientos";
            aCentenas[8] = "Novecientos";
            /* -------------------------------------------------------------------------- */

            int iDigitos = sPesos.Length;
            int iTercions = (int)(iDigitos / 3) + ((iDigitos % 3) > 0 ? 1 : 0);
            string sCantPorPro = sPesos;

            string sLetraTercio, sCentenar, sAProcesar;
            int iCentenar, iLongitudCentenar, iUnidadesTercio, iNumeroActual;
            for (int iCont = 1; iCont <= iTercions; iCont++)
            {
                sLetraTercio = "";
                sCentenar = sCantPorPro.Derecha(3);
                sCantPorPro = sCantPorPro.Izquierda(sCantPorPro.Length - (sCantPorPro.Length > 3 ? 3 : 0));
                iCentenar = Entero(sCentenar);
                iLongitudCentenar = sCentenar.Length;
                sAProcesar = sCentenar;
                iUnidadesTercio = Entero(sAProcesar.Derecha(1));

                for (int iCont2 = 1; iCont2 <= iLongitudCentenar; iCont2++)
                {
                    iNumeroActual = Entero(sAProcesar.Derecha(1));

                    if (iNumeroActual != 0)
                    {
                        switch (iCont2)
                        {
                            case 1:  // Unidades
                                if (iNumeroActual == 1)
                                    sLetraTercio = "Un";
                                else
                                    sLetraTercio = aUnidades[iNumeroActual - 1];
                                break;
                            case 2:  // Decenas
                                if (iUnidadesTercio == 0)
                                {
                                    sLetraTercio = aDecenas[iNumeroActual - 1];
                                }
                                else
                                {
                                    if (iNumeroActual == 1)  // Entre 10 y 19
                                        sLetraTercio = aDieces[iUnidadesTercio - 1];
                                    else if (iNumeroActual == 2)  // Entre 20 y 29
                                        sLetraTercio = "Veinti" + sLetraTercio;
                                    else if (iNumeroActual > 2)  //Entre 30 y 99
                                        sLetraTercio = aDecenas[iNumeroActual - 1] + " y " + sLetraTercio;
                                }
                                break;
                            case 3:  // Centenas
                                if (iNumeroActual == 1 && sCentenar.Derecha(2) != "00")
                                    sLetraTercio = "Ciento " + sLetraTercio;
                                else
                                    sLetraTercio = aCentenas[iNumeroActual - 1] + " " + sLetraTercio;
                                break;
                        }
                    }

                    sAProcesar = sAProcesar.Izquierda(sAProcesar.Length - 1);
                }

                // Se escribe el nombre del Tercio
                if (iCont == 1)
                    sLetraTercio += (sLetraTercio.Izquierda(2) == "Un" ? " Peso " : " Pesos ");
                if ((iCont % 2) == 0)
                    sLetraTercio += " Mil ";
                if ((iCont % 3) == 0)
                    sLetraTercio += (sLetraTercio.Izquierda(2) == "Un" ? " Millón " : " Millones ");

                sLetra = sLetraTercio + sLetra;
            }

            sLetra += sCentavos + MONEDA;

            return sLetra;
        }

        #endregion

        #region [ Validacioines ]

        public static bool ValidarDecimal(string sCadena)
        {
            sCadena += "";
            return Regex.IsMatch(sCadena, @"(^\d+$|^\d+\.\d+$|^\d+\.$|^\.\d+$)");
        }

        public static bool ValidarEntero(string sCadena)
        {
            sCadena += "";
            return Regex.IsMatch(sCadena, @"^\d+$");
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

        public static T CrearCopia<T>(T oOriginal)
        {
            var oCopia = Activator.CreateInstance<T>();
            PropertyInfo[] oPropiedades = oOriginal.GetType().GetProperties();
            // string sTipo;
            foreach (PropertyInfo oPropiedad in oPropiedades)
            {
                Type oTipo = oPropiedad.PropertyType;
                if (!oTipo.EsTipoSimple())
                    continue;
                oPropiedad.SetValue(oCopia, oPropiedad.GetValue(oOriginal, null), null);
            }

            return oCopia;
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

        public static string MensajeDeError(this Exception oEx)
        {
            return (oEx.Message + (oEx.InnerException == null ? "" : (oEx.InnerException.Message == oEx.Message ? "" : ("\n" + oEx.InnerException.Message))));
        }

        #endregion

        #region [ Formularios ]

        public static DataTable ADataTable(this DataGridView Grid)
        {
            var oDatos = new DataTable();
            DataRow oFilaT;
            foreach (DataGridViewRow Fila in Grid.Rows)
            {
                oFilaT = oDatos.NewRow();
                foreach (DataGridViewCell Celda in Fila.Cells)
                {
                    if (!oDatos.Columns.Contains(Celda.OwningColumn.Name))
                        oDatos.Columns.Add(Celda.OwningColumn.Name);
                    oFilaT[Celda.OwningColumn.Name] = Celda.Value;
                }
                oDatos.Rows.Add(oFilaT);
            }

            return oDatos;
        }

        public static void MostrarColumnas(this DataGridView Grid, params string[] Columnas)
        {
            foreach (DataGridViewColumn oCol in Grid.Columns)
            {
                oCol.Visible = (Columnas.Contains(oCol.Name));
            }
        }

        public static void OcultarColumnas(this DataGridView Grid, params string[] Columnas)
        {
            foreach (string sColumna in Columnas)
            {
                if (Grid.Columns.Contains(sColumna))
                    Grid.Columns[sColumna].Visible = false;
            }
        }

        public static void CargarDatos(this ComboBox Combo, string sCampoValor, string sCampoTexto, object Datos)
        {
            Combo.ValueMember = sCampoValor;
            Combo.DisplayMember = sCampoTexto;
            Combo.DataSource = Datos;
            Combo.SelectedIndex = -1;
        }

        public static void CargarDatos(this ComboEtiqueta Combo, string sCampoValor, string sCampoTexto, object Datos)
        {
            Combo.ValueMember = sCampoValor;
            Combo.DisplayMember = sCampoTexto;
            Combo.DataSource = Datos;
            Combo.SelectedIndex = -1;
        }

        public static void CargarDatos(this DataGridViewComboBoxColumn oCombo, string sCampoValor, string sCampoTexto, object oDatos)
        {
            oCombo.ValueMember = sCampoValor;
            oCombo.DisplayMember = sCampoTexto;
            oCombo.DataSource = oDatos;
        }

        public static void CargarDatos(this DataGridViewComboBoxCell oCombo, string sCampoValor, string sCampoTexto, object oDatos)
        {
            oCombo.ValueMember = sCampoValor;
            oCombo.DisplayMember = sCampoTexto;
            oCombo.DataSource = oDatos;
        }

        public static void EstablecerSortModeEnColumnas(this DataGridView oGrid, DataGridViewColumnSortMode eSortMode)
        {
            foreach (DataGridViewColumn oCol in oGrid.Columns)
                oCol.SortMode = eSortMode;
        }

        public static string ObtenerCadenaDeFiltro(this DataGridView Datos, string sBusqueda)
        {
            StringBuilder Filtro = new StringBuilder();
            string sTipoDato, sCampo, sFiltroCampo;
            for (int iCont = 0; iCont < Datos.Columns.Count; iCont++)
            {
                if (!Datos.Columns[iCont].Visible || !Datos.Columns[iCont].IsDataBound)
                    continue;

                sTipoDato = (Datos.Columns[iCont].ValueType == null ? "" : Datos.Columns[iCont].ValueType.ToString());
                if (sTipoDato.Contains("String"))
                    sCampo = string.Format("[{0}]", Datos.Columns[iCont].Name);
                else
                    sCampo = string.Format("Convert([{0}], 'System.String')", Datos.Columns[iCont].Name);

                sFiltroCampo = string.Format("{0} LIKE '%{1}%'", sCampo, sBusqueda);
                Filtro.Append(" OR " + sFiltroCampo);
            }
            if (Filtro.Length > 3)
                Filtro.Remove(0, 4);

            return Filtro.ToString();
        }

        public static int EncontrarIndiceDeValor(this DataGridView oGrid, string sColumna, object oValor)
        {
            for (int iFila = 0; iFila < oGrid.Rows.Count; iFila++)
            {
                if (Util.Cadena(oGrid.Rows[iFila].Cells[sColumna].Value) == Util.Cadena(oValor))
                    return iFila;
            }
            return -1;
        }

        public static Control ControlAlFrente(Control Contenedor)
        {
            foreach (Control oControl in Contenedor.Controls)
            {
                if (Contenedor.Controls.GetChildIndex(oControl) == 0)
                    return oControl;
            }

            return null;
        }

        public static void EntrarPantallaCompleta(this Form oForm)
        {
            oForm.WindowState = FormWindowState.Normal;
            oForm.FormBorderStyle = FormBorderStyle.None;
            oForm.WindowState = FormWindowState.Maximized;
        }

        public static void SalirrPantallaCompleta(this Form oForm)
        {
            oForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            oForm.WindowState = FormWindowState.Normal;
        }

        public static void CopiarPropiedades(Control oFuente, Control oDestino, params string[] aExcluir)
        {
            // Se hacen todas las propiedades minúsculas, para evitar problemas de cómo lo manden
            for (int i = 0; i < aExcluir.Length; i++)
                aExcluir[i] = aExcluir[i].ToLower();

            if (!aExcluir.Contains("anchor"))
                oDestino.Anchor = oFuente.Anchor;
            if (!aExcluir.Contains("autosize"))
                oDestino.AutoSize = oFuente.AutoSize;
            if (!aExcluir.Contains("backcolor"))
                oDestino.BackColor = oFuente.BackColor;
            if (!aExcluir.Contains("backgroundimage"))
                oDestino.BackgroundImage = oFuente.BackgroundImage;
            if (!aExcluir.Contains("backgroundimagelayout"))
                oDestino.BackgroundImageLayout = oFuente.BackgroundImageLayout;
            if (!aExcluir.Contains("dock"))
                oDestino.Dock = oFuente.Dock;
            if (!aExcluir.Contains("enabled"))
                oDestino.Enabled = oFuente.Enabled;
            if (!aExcluir.Contains("font"))
                oDestino.Font = oFuente.Font;
            if (!aExcluir.Contains("forecolor"))
                oDestino.ForeColor = oFuente.ForeColor;
            if (!aExcluir.Contains("height"))
                oDestino.Height = oFuente.Height;
            if (!aExcluir.Contains("left"))
                oDestino.Left = oFuente.Left;
            if (!aExcluir.Contains("text"))
                oDestino.Text = oFuente.Text;
            if (!aExcluir.Contains("top"))
                oDestino.Top = oFuente.Top;
            if (!aExcluir.Contains("visible"))
                oDestino.Visible = oFuente.Visible;
            if (!aExcluir.Contains("width"))
                oDestino.Width = oFuente.Width;
        }

        public static void AgregarColumnaCadena(this DataGridView oGrid, string sNombre, string sEncabezado, int iWidth)
        {
            oGrid.Columns.Add(sNombre, sEncabezado);
            oGrid.Columns[sNombre].ValueType = typeof(string);
            oGrid.Columns[sNombre].Width = iWidth;
        }

        public static int BuscarValor(this DataGridView Grid, string sCampo, object Valor)
        {
            for (int iCont = 0; iCont < Grid.Rows.Count; iCont++)
            {
                if (Util.Cadena(Grid.Rows[iCont].Cells[sCampo].Value) == Util.Cadena(Valor))
                    return iCont;
            }
            return -1;
        }

        public static object[] ObtenerValores(this DataGridViewRow Fila)
        {
            object[] Valores = new object[Fila.Cells.Count];
            for (int iCol = 0; iCol < Fila.Cells.Count; iCol++)
                Valores[iCol] = Fila.Cells[iCol].Value;

            return Valores;
        }

        public static void CambiarColorDeFondo(this DataGridView oGrid, Color oColor)
        {
            oGrid.BackgroundColor = oColor;
            oGrid.DefaultCellStyle.BackColor = oColor;
        }

        public static int ContarIncidencias(this DataGridView oGrid, string sColumna, object oValor)
        {
            int iCuenta = 0;
            for (int iFila = 0; iFila < oGrid.Rows.Count; iFila++)
            {
                if (Util.Cadena(oGrid.Rows[iFila].Cells[sColumna].Value) == Util.Cadena(oValor))
                    iCuenta++;
            }
            return -iCuenta;
        }

        public static void EncontrarContiene(this DataGridView oGrid, string sBusqueda, params string[] oColumnas)
        {
            if (string.IsNullOrEmpty(sBusqueda))
                return;

            sBusqueda = sBusqueda.ToLower();
            foreach (DataGridViewRow oFila in oGrid.Rows)
            {
                foreach (string sCol in oColumnas)
                {
                    if (Util.Cadena(oFila.Cells[sCol].Value).ToLower().Contains(sBusqueda) && oFila.Visible)
                    {
                        oGrid.CurrentCell = oFila.Cells[sCol];
                        return;
                    }
                }
            }
        }

        public static void ExportarACsv(this DataGridView oGrid, string sRutaCsv, bool bSoloColumnasVisibiles)
        {
            var oCsv = new StreamWriter(sRutaCsv, false, Encoding.Default);

            // Se escriben los encabezados
            string sEncabezado = "";
            int iUltCol = 0;
            var oTiposDeDato = new Dictionary<int, string>();
            foreach (DataGridViewColumn oCol in oGrid.Columns)
            {
                if (bSoloColumnasVisibiles && !oCol.Visible) continue;
                sEncabezado += (oCol.HeaderText + ",");
                iUltCol = oCol.Index;

                // Se registra el tipo de dato
                oTiposDeDato.Add(oCol.Index, (oCol.ValueType == null ? "null" : oCol.ValueType.Name));
            }
            sEncabezado = (sEncabezado == "" ? "" : sEncabezado.Substring(0, sEncabezado.Length - 1));
            oCsv.WriteLine(sEncabezado);

            // Se escriben los datos
            foreach (DataGridViewRow oFila in oGrid.Rows)
            {
                foreach (DataGridViewColumn oCol in oGrid.Columns)
                {
                    if (bSoloColumnasVisibiles && !oCol.Visible) continue;

                    if (oTiposDeDato[oCol.Index] == "String")
                    {
                        oCsv.Write("\"");
                        oCsv.Write(oFila.Cells[oCol.Index].Value);
                        oCsv.Write("\"");
                    }
                    else
                    {
                        oCsv.Write(oFila.Cells[oCol.Index].Value);
                    }

                    if (oCol.Index < iUltCol)
                        oCsv.Write(",");
                }
                oCsv.WriteLine();
            }

            // Se cierra el archivo
            oCsv.Flush();
            oCsv.Close();
        }

        public static void FiltrarEntero(this DataGridView oGrid, int iValor, string sColumna)
        {
            foreach (DataGridViewRow oFila in oGrid.Rows)
            {
                oFila.Visible = (Util.Entero(oFila.Cells[sColumna].Value) == iValor);
            }
        }

        public static void FiltrarCadena(this DataGridView oGrid, string sValor, string sColumna)
        {
            foreach (DataGridViewRow oFila in oGrid.Rows)
            {
                oFila.Visible = (Util.Cadena(oFila.Cells[sColumna].Value).ToLower() == sValor.ToLower());
            }
        }

        public static void FiltrarContiene(this DataGridView oGrid, string sBusqueda, params string[] oColumnas)
        {
            sBusqueda = sBusqueda.ToLower();
            foreach (DataGridViewRow oFila in oGrid.Rows)
            {
                if (string.IsNullOrEmpty(sBusqueda))
                {
                    oFila.Visible = true;
                    continue;
                }

                bool bCoincide = false;
                foreach (string sCol in oColumnas)
                {
                    if (Util.Cadena(oFila.Cells[sCol].Value).ToLower().Contains(sBusqueda))
                    {
                        bCoincide = true;
                        break;
                    }
                }
                oFila.Visible = bCoincide;
            }
        }

        public static void QuitarFiltro(this DataGridView oGrid)
        {
            foreach (DataGridViewRow oFila in oGrid.Rows)
                oFila.Visible = true;
        }

        public static Dictionary<string, object> ADiccionario(this DataGridViewRow Fila)
        {
            var oDic = new Dictionary<string, object>();
            foreach (DataGridViewCell oCelda in Fila.Cells)
                oDic.Add(oCelda.OwningColumn.Name, oCelda.Value);
            return oDic;
        }

        public static DataGridViewRow FilaSiguiente(this DataGridViewRow Fila)
        {
            if (Fila.DataGridView.Rows.Count > (Fila.Index + 1))
                return Fila.DataGridView.Rows[Fila.Index + 1];
            else
                return null;
        }

        public static DataGridViewRow FilaAnterior(this DataGridViewRow Fila)
        {
            if (Fila.Index > 0)
                return Fila.DataGridView.Rows[Fila.Index - 1];
            else
                return null;
        }

        public static DataTable LeerCsv(string sRutaCsv)
        {
            var oDatos = new DataTable();

            var oLector = new StreamReader(sRutaCsv, Encoding.Default);
            // Se generan las columnas, con el encabezado
            string sLinea = oLector.ReadLine();
            var oCampos = sLinea.Split(',');
            foreach (string sCampo in oCampos)
                oDatos.Columns.Add(sCampo);
            // Se llenan los datos
            DataRow oFila;
            string sValor;
            while (!oLector.EndOfStream)
            {
                sLinea = oLector.ReadLine();
                oCampos = sLinea.Split(',');
                oFila = oDatos.Rows.Add();
                for (int iCol = 0; iCol < oCampos.Length; iCol++)
                {
                    sValor = oCampos[iCol];
                    if (sValor.StartsWith("\""))
                        sValor = sValor.Substring(1, sValor.Length - 2);
                    oFila[iCol] = sValor;
                }
            }

            return oDatos;
        }

        #endregion

        #region [ Entity ]

        public static bool EsTipoSimple(this Type oTipo)
        {
            string sTipo = oTipo.Name.ToLower();
            return (
                sTipo == "char"
                || sTipo == "string"
                || sTipo == "int32"
                || sTipo == "decimal"
                || sTipo == "double"
                || sTipo == "boolean"
                || sTipo == "datetime"
            );
        }

        public static DataTable ListaEntityADataTable<T>(List<T> oLista)
        {
            DataTable Datos = new DataTable();

            if (oLista == null || oLista.Count <= 0) return Datos;

            // Se obtienen las propiedades y se generan las columnas de DataTable
            PropertyInfo[] oPropiedades = oLista[0].GetType().GetProperties();
            // string sTipo;
            foreach (PropertyInfo oPropiedad in oPropiedades)
            {
                Type oTipo = oPropiedad.PropertyType;
                // Si es tipo "Nullable", se obtiene el tipo interno*
                if (oTipo.IsGenericType && (oTipo.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    oTipo = oTipo.GetGenericArguments()[0];
                // Si no es tipo por valor, no se considera
                if (!oTipo.EsTipoSimple())
                    continue;
                // Se agrega la columna al DataTable
                Datos.Columns.Add(new DataColumn(oPropiedad.Name, oTipo));
            }

            foreach (T oElemento in oLista)
            {
                DataRow Fila = Datos.NewRow();
                object oValor;
                foreach (PropertyInfo oPropiedad in oPropiedades)
                {
                    // Si no se agregó como columna al DataTable, se ignora
                    if (!Datos.Columns.Contains(oPropiedad.Name))
                        continue;
                    // Se agrega el valor al DataRow
                    oValor = oPropiedad.GetValue(oElemento, null);
                    Fila[oPropiedad.Name] = (oValor == null ? DBNull.Value : oValor);
                }

                Datos.Rows.Add(Fila);
            }

            return Datos;
        }

        #endregion

        #region [ Images ]

        public static Bitmap RotateImage(Image image, float angle)
        {
            // center of the image
            float rotateAtX = image.Width / 2;
            float rotateAtY = image.Height / 2;
            bool bNoClip = false;
            return RotateImage(image, rotateAtX, rotateAtY, angle, bNoClip);
        }

        public static Bitmap RotateImage(Image image, float angle, bool bNoClip)
        {
            // center of the image
            float rotateAtX = image.Width / 2;
            float rotateAtY = image.Height / 2;
            return RotateImage(image, rotateAtX, rotateAtY, angle, bNoClip);
        }

        public static Bitmap RotateImage(Image image, float rotateAtX, float rotateAtY, float angle, bool bNoClip)
        {
            int W, H, X, Y;
            if (bNoClip)
            {
                double dW = (double)image.Width;
                double dH = (double)image.Height;

                double degrees = Math.Abs(angle);
                if (degrees <= 90)
                {
                    double radians = 0.0174532925 * degrees;
                    double dSin = Math.Sin(radians);
                    double dCos = Math.Cos(radians);
                    W = (int)(dH * dSin + dW * dCos);
                    H = (int)(dW * dSin + dH * dCos);
                    X = (W - image.Width) / 2;
                    Y = (H - image.Height) / 2;
                }
                else
                {
                    degrees -= 90;
                    double radians = 0.0174532925 * degrees;
                    double dSin = Math.Sin(radians);
                    double dCos = Math.Cos(radians);
                    W = (int)(dW * dSin + dH * dCos);
                    H = (int)(dH * dSin + dW * dCos);
                    X = (W - image.Width) / 2;
                    Y = (H - image.Height) / 2;
                }
            }
            else
            {
                W = image.Width;
                H = image.Height;
                X = 0;
                Y = 0;
            }

            //create a new empty bitmap to hold rotated image
            Bitmap bmpRet = new Bitmap(W, H);
            bmpRet.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(bmpRet);

            //Put the rotation point in the "center" of the image
            g.TranslateTransform(rotateAtX + X, rotateAtY + Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-rotateAtX - X, -rotateAtY - Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0 + X, 0 + Y));

            return bmpRet;
        }

        /// <summary>
        /// Creates a new Image containing the same image only rotated
        /// </summary>
        /// <param name=""image"">The <see cref=""System.Drawing.Image"/"> to rotate
        /// <param name=""offset"">The position to rotate from.
        /// <param name=""angle"">The amount to rotate the image, clockwise, in degrees
        /// <returns>A new <see cref=""System.Drawing.Bitmap"/"> of the same size rotated.</see>
        /// <exception cref=""System.ArgumentNullException"">Thrown if <see cref=""image"/"> 
        /// is null.</see>
        public static Bitmap RotateImageX(Image image, PointF offset, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

        #endregion

        #region [ Red ]

        public static string IpLocal()
        {
            var oHost = Dns.GetHostEntry(Dns.GetHostName());
            var oIp = oHost.AddressList.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            return (oIp == null ? null : oIp.ToString());
        }

        #endregion

        #region [ General ]

        public static string AgregarSeparadorDeCarpeta(string sRuta)
        {
            if (!sRuta.EndsWith(Path.DirectorySeparatorChar.ToString()))
                sRuta += Path.DirectorySeparatorChar;
            return sRuta;
        }

        #endregion

    }
}
