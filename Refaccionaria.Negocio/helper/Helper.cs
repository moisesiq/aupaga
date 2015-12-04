using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace Refaccionaria.Negocio
{
    public static class Helper
    {
        private const string InvalidXmlExpression = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
        private static readonly Regex RFCPattern = new Regex("[A-Z,\x00d1,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]{2}[0-9,A]");
        private static readonly Regex UUIDPattern = new Regex(@"^[0-9A-F]{8}\-[0-9A-F]{4}\-[0-9A-F]{4}\-[0-9A-F]{4}\-[0-9A-F]{12}$");
        private static readonly Regex emailPattern = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

        #region [Conversion DataTable]

        public static DataTable ListaEntityADataTable<T>(List<T> oLista)
        {
            DataTable Datos = new DataTable();

            if (oLista == null || oLista.Count <= 0) return Datos;

            // Se obtienen las propiedades y se generan las columnas de DataTable
            PropertyInfo[] oPropiedades = oLista[0].GetType().GetProperties();
            string sTipo;
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

        public static DataTable LinqToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        public static DataSet ToDataSet<T>(this List<T> list)
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable(elementType.FullName);
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                t.Columns.Add(propInfo.Name, propInfo.PropertyType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();
                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null);
                }
            }
            return ds;
        }

        public static DataTable newTable<T>(string name, IEnumerable<T> list)
        {
            PropertyInfo[] pi = typeof(T).GetProperties();

            DataTable table = Table<T>(name, list, pi);

            IEnumerator<T> e = list.GetEnumerator();

            while (e.MoveNext())
                table.Rows.Add(newRow<T>(table.NewRow(), e.Current, pi));

            return table;
        }

        private static DataTable Table<T>(string name, IEnumerable<T> list, PropertyInfo[] pi)
        {
            DataTable table = new DataTable(name);

            // Loop through each property, and add it as a column to the datatable
            foreach (PropertyInfo p in pi)
            {
                // The the type of the property
                Type columnType = p.PropertyType;

                // We need to check whether the property is NULLABLE
                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    columnType = p.PropertyType.GetGenericArguments()[0];
                }

                // Add the column definition to the datatable.
                table.Columns.Add(new DataColumn(p.Name, columnType));
            }
            return table;
        }

        private static DataRow newRow<T>(DataRow row, T listItem, PropertyInfo[] pi)
        {
            foreach (PropertyInfo p in pi)
            {
                row[p.Name.ToString()] = p.GetValue(listItem, null) == null ? DBNull.Value : p.GetValue(listItem, null);
            }
            return row;
        }

        #endregion

        #region [ Controles de UI ]

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

        public static void FiltrarEntero(this DataGridView oGrid, int iValor, string sColumna)
        {
            foreach (DataGridViewRow oFila in oGrid.Rows)
            {
                oFila.Visible = (Helper.ConvertirEntero(oFila.Cells[sColumna].Value) == iValor);
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
                    if (Helper.ConvertirCadena(oFila.Cells[sCol].Value).ToLower().Contains(sBusqueda))
                    {
                        bCoincide = true;
                        break;
                    }
                }
                oFila.Visible = bCoincide;
            }
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
                    if (Helper.ConvertirCadena(oFila.Cells[sCol].Value).ToLower().Contains(sBusqueda))
                    {
                        oGrid.CurrentCell = oFila.Cells[sCol];
                        return;
                    }
                }
            }
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

        public static void ReadOnlyColumnas(this DataGridView Grid, params string[] Columnas)
        {
            foreach (string sColumna in Columnas)
            {
                if (Grid.Columns.Contains(sColumna))
                    Grid.Columns[sColumna].ReadOnly = true;
            }            
        }

        public static void FormatoDecimalColumnas(this DataGridView Grid, params string[] Columnas)
        {
            foreach (string sColumna in Columnas)
            {
                if (Grid.Columns.Contains(sColumna))
                {
                    Grid.Columns[sColumna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    Grid.Columns[sColumna].DefaultCellStyle.Format = "###,###,###,##0.00";
                }
            }            
        }

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

        public static int BuscarValor(this DataGridView Grid, string sCampo, object Valor)
        {
            for (int iCont = 0; iCont < Grid.Rows.Count; iCont++)
            {
                if (Helper.ConvertirCadena(Grid.Rows[iCont].Cells[sCampo].Value) == Helper.ConvertirCadena(Valor))
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

        public static Dictionary<string, object> ADiccionario(this DataGridViewRow Fila)
        {
            var oDic = new Dictionary<string, object>();
            foreach (DataGridViewCell oCelda in Fila.Cells)
                oDic.Add(oCelda.OwningColumn.Name, oCelda.Value);
            return oDic;
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

        public static int EncontrarIndiceDeValor(this DataGridView oGrid, string sColumna, object oValor)
        {
            for (int iFila = 0; iFila < oGrid.Rows.Count; iFila++)
            {
                if (Helper.ConvertirCadena(oGrid.Rows[iFila].Cells[sColumna].Value) == Helper.ConvertirCadena(oValor))
                    return iFila;
            }
            return -1;
        }

        public static int ContarIncidencias(this DataGridView oGrid, string sColumna, object oValor)
        {
            int iCuenta = 0;
            for (int iFila = 0; iFila < oGrid.Rows.Count; iFila++)
            {
                if (Helper.ConvertirCadena(oGrid.Rows[iFila].Cells[sColumna].Value) == Helper.ConvertirCadena(oValor))
                    iCuenta++;
            }
            return -iCuenta;
        }

        public static void EstablecerSortModeEnColumnas(this DataGridView oGrid, DataGridViewColumnSortMode eSortMode)
        {
            foreach (DataGridViewColumn oCol in oGrid.Columns)
                oCol.SortMode = eSortMode;
        }

        public static void AgregarColumnaCadena(this DataGridView oGrid, string sNombre, string sEncabezado, int iWidth)
        {
            oGrid.Columns.Add(sNombre, sEncabezado);
            oGrid.Columns[sNombre].ValueType = typeof(string);
            oGrid.Columns[sNombre].Width = iWidth;
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

        #endregion

        #region [Utilidades]

        public static decimal AplicarRedondeo(decimal cantidad)
        {
            decimal resultado = cantidad;
            try
            {
                if (cantidad >= 0.01M && cantidad <= 1.00M)
                {
                    string[] param = cantidad.ToString().Split('.');
                    if (param.Length == 2)
                    {
                        var arr = param[1].ToArray();
                        if (arr.Length > 0)
                        {
                            int decimo = 0;
                            int centesimo = 0;

                            int.TryParse(arr[0].ToString(), out decimo);
                            int.TryParse(arr[1].ToString(), out centesimo);

                            //Si los centesimos son iguales a 0, que no redondee
                            if (centesimo == 0 && centesimo == 5)
                                return cantidad;

                            if (decimo != 9)
                            {
                                if (centesimo < 5)
                                {
                                    centesimo = 5;
                                }
                                else
                                {
                                    decimo = decimo + 1;
                                    centesimo = 0;
                                }
                                resultado = ConvertirDecimal(string.Format("{0}.{1}{2}", param[0], decimo, centesimo));
                            }
                            else
                            {
                                resultado = Math.Ceiling(cantidad);
                            }
                        }
                    }
                }
                else if (cantidad >= 1.01M && cantidad <= 30.00M)
                {
                    string[] param = cantidad.ToString().Split('.');
                    if (param.Length == 2)
                    {
                        var arr = param[1].ToArray();
                        if (arr.Length > 0)
                        {
                            int decimo = 0;
                            int centesimo = 0;

                            int.TryParse(arr[0].ToString(), out decimo);
                            int.TryParse(arr[1].ToString(), out centesimo);

                            //Si los centesimos son iguales a 0, que no redondee
                            if (centesimo == 0)
                                return cantidad;

                            if (decimo != 9)
                            {
                                decimo = decimo + 1;
                                centesimo = 0;

                                resultado = ConvertirDecimal(string.Format("{0}.{1}{2}", param[0], decimo, centesimo));
                            }
                            else
                            {
                                resultado = Math.Ceiling(cantidad);
                            }
                        }
                    }
                }
                ////else if (cantidad >= 20.01M && cantidad <= 100.00M)
                ////{
                ////    string[] param = cantidad.ToString().Split('.');
                ////    if (param.Length == 2)
                ////    {
                ////        var unitario = ConvertirDecimal(param[0].ToString());
                ////        resultado = ConvertirDecimal(string.Format("{0}.00", unitario + 1));
                ////    }
                ////}
                //else if (cantidad >= 100.01M && cantidad <= 200.00M)
                //{
                //    string[] param = cantidad.ToString().Split('.');
                //    if (param.Length == 2)
                //    {
                //        var unitario = ConvertirDecimal(param[0].ToString());
                //        resultado = ConvertirDecimal(string.Format("{0}.00", unitario + 2));
                //    }
                //}
                else if (cantidad >= 30.01M && cantidad <= 100000.00M)
                {
                    string[] param = cantidad.ToString().Split('.');
                    if (param.Length == 2)
                    {
                        var arr = param[1].ToArray();
                        if (arr.Length > 0)
                        {
                            int decimo = 0;
                            int centesimo = 0;

                            int.TryParse(arr[0].ToString(), out decimo);
                            int.TryParse(arr[1].ToString(), out centesimo);

                            //Si los centesimos son iguales a 0, que no redondee
                            if (decimo == 0 && centesimo == 0)
                                return cantidad;
                        }
                        var unitario = ConvertirDecimal(param[0].ToString());
                        resultado = ConvertirDecimal(string.Format("{0}.00", unitario + 1));
                    }
                }
                //else if (cantidad >= 2000.01M && cantidad <= 10000.00M)
                //{
                //    string[] param = cantidad.ToString().Split('.');
                //    if (param.Length == 2)
                //    {
                //        var unitario = ConvertirDecimal(param[0].ToString());
                //        resultado = ConvertirDecimal(string.Format("{0}.00", unitario + 1));
                //    }
                //}
            }
            catch (Exception ex)
            {
            }

            return resultado;
        }        

        public static void ClearTextBoxes(System.Windows.Forms.Form form)
        {
            foreach (System.Windows.Forms.Control c in form.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }

                if (c is CheckBox)
                {
                    ((CheckBox)c).Checked = false;
                }

                if (c is RadioButton)
                {
                    ((RadioButton)c).Checked = false;
                }
            }
        }

        public static void ClearControlTextBoxes(System.Windows.Forms.UserControl control)
        {
            foreach (System.Windows.Forms.Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }

                if (c is CheckBox)
                {
                    ((CheckBox)c).Checked = false;
                }

                if (c is RadioButton)
                {
                    ((RadioButton)c).Checked = false;
                }
            }
        }

        public static void cboCharacterCasingUpper(object sender, EventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox cb = (ComboBox)sender;
                int i = cb.SelectionStart;
                cb.Text = cb.Text.ToUpper();
                cb.Select(i, 0);
            }
        }

        public static void ColumnasToHeaderText(object sender)
        {
            if (sender is DataGridView)
            {
                DataGridView dgv = (DataGridView)sender;
                for (int i = 0; i < dgv.Columns.Count - 1; i++)
                {
                    dgv.Columns[i].HeaderText = ToHeaderText(dgv.Columns[i].HeaderText.ToString());
                }
            }
        }

        public static string DecimalToCadenaMoneda(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            decimal mValor;
            decimal.TryParse(sValor, out mValor);
            return String.Format("{0:C2}", mValor).Replace("$", "");
        }
                
        public static int findRowIndex(DataGridView dgv, string cellName, string searchValue)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (Helper.ConvertirCadena(row.Cells[cellName].Value).Equals(searchValue))
                {
                    return row.Index;
                }
            }
            return -1;
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
                iCentenar = ConvertirEntero(sCentenar);
                iLongitudCentenar = sCentenar.Length;
                sAProcesar = sCentenar;
                iUnidadesTercio = ConvertirEntero(sAProcesar.Derecha(1));

                for (int iCont2 = 1; iCont2 <= iLongitudCentenar; iCont2++)
                {
                    iNumeroActual = ConvertirEntero(sAProcesar.Derecha(1));

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
                
        public static string NombreDateTime()
        {
            return DateTime.Now.ToString("u").Replace("-", string.Empty).Replace(":", string.Empty).Replace(" ", string.Empty);
        }
        
        public static bool RevisarConexion()
        {
            WebClient client = new WebClient();
            try
            {
                //using (client.OpenRead("http://www.google.com"))
                using (client.OpenRead("http://cfdiws2012.umbrall.com.mx/Service.asmx"))
                {
                }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        public static string ToHeaderText(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value == string.Empty)
                return string.Empty;

            var headerText = new StringBuilder();
            var space = (char)32;
            var existSpaceBetween = false;
            var lastCharIsUpper = char.IsUpper(value[value.Length - 1]);
            var longitud = lastCharIsUpper ? value.Length - 1 : value.Length;

            for (int x = 1; x < value.Length; x++)
                if (value[x] == space)
                    existSpaceBetween = true;

            if (existSpaceBetween)
                return value.Trim();

            headerText.Append(value[0].ToString().ToUpper());
            if (!existSpaceBetween)
                for (int x = 1; x < longitud; x++)
                {
                    var charIsUpper = char.IsUpper(value[x]);

                    if (charIsUpper)
                    {
                        headerText.Append(" ");
                    }
                    headerText.Append(value[x]);
                }

            if (lastCharIsUpper)
                headerText.Append(value[value.Length - 1]);

            var humanText = headerText.ToString().Trim();
            return humanText;
        }

        public static void OcultarColumnas(object sender, params string[] Columnas)
        {
            if (sender is DataGridView)
            {
                DataGridView dgv = (DataGridView)sender;
                foreach (string sColumna in Columnas)
                {
                    if (dgv.Columns.Contains(sColumna))
                        dgv.Columns[sColumna].Visible = false;
                }
            }
        }

        public static void OnExportGridToCSV(object sender, string pathCsv)
        {
            try
            {
                if (sender is DataGridView)
                {
                    DataGridView dgv = (DataGridView)sender;
                    StreamWriter sw = new StreamWriter(pathCsv, false);

                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        sw.Write(dgv.Columns[i].HeaderText);
                        if (i != dgv.Columns.Count)
                        {
                            sw.Write(",");
                        }
                    }
                    // add new line
                    sw.Write(sw.NewLine);
                    // iterate through all the rows within the gridview
                    foreach (DataGridViewRow dr in dgv.Rows)
                    {
                        // iterate through all colums of specific row
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            // write particular cell to csv file
                            sw.Write(dr.Cells[i].Value.ToString().Replace(",", " "));
                            if (i != dgv.Columns.Count)
                            {
                                sw.Write(",");
                            }
                        }
                        // write new line
                        sw.Write(sw.NewLine);
                    }
                    // flush from the buffers.
                    sw.Flush();
                    // closes the file
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public static string NombreDeArchivoIncremental(string sRutaYPrefijo, int iLongitudNumero, string sSufijo)
        {
            int iIncremento = 0;
            string sNombre = "";
            do {
                sNombre = (sRutaYPrefijo + (++iIncremento).ToString().PadLeft(iLongitudNumero, '0') + sSufijo);
            } while (File.Exists(sNombre));
            return sNombre;
        }

        public static string AgregarSeparadorDeCarpeta(string sRuta)
        {
            if (!sRuta.EndsWith(Path.DirectorySeparatorChar.ToString()))
                sRuta += Path.DirectorySeparatorChar;
            return sRuta;
        }

        #endregion

        #region [ Extensiones string ]

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

        public static string Derecha(this string sCadena, int iCaracteres)
        {
            return sCadena.Substring(((sCadena.Length - iCaracteres) < 0 ? 0 : (sCadena.Length - iCaracteres)));
        }

        public static string Izquierda(this string sCadena, int iCaracteres)
        {
            if (iCaracteres < 0 || sCadena == null)
                return "";
            else if ((sCadena.Length - iCaracteres) < 0)
                return sCadena.Substring(0);
            else
                return sCadena.Substring(0, iCaracteres);
        }

        public static string LimpiarCadenaDeEspaciosBlancos(this string sCadena)
        {
            return System.Text.RegularExpressions.Regex.Replace(sCadena, @"\s+", " ").Trim();
        }

        public static string Repetir(this string sCadena, int iVeces)
        {
            string sRes = "";
            for (int iCont = 0; iCont < iVeces; iCont++)
                sRes += sCadena;
            return sRes;
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

        public static string Truncar(this string sCadena, int iCaracteres)
        {
            return (sCadena.Substring(0, (iCaracteres > sCadena.Length ? sCadena.Length : iCaracteres)));
        }

        #endregion
        
        #region [ Extensiones DateTime ]

        public static DateTime DiaPrimero(this DateTime dFecha)
        {
            return dFecha.AddDays((dFecha.Day - 1) * -1);
        }

        public static DateTime DiaUltimo(this DateTime dFecha)
        {
            return dFecha.AddMonths(1).DiaPrimero().AddDays(-1);
        }

        public static int Semana(this DateTime dFecha)
        {
            var oFechaInfo = DateTimeFormatInfo.CurrentInfo;
            return oFechaInfo.Calendar.GetWeekOfYear(dFecha, oFechaInfo.CalendarWeekRule, oFechaInfo.FirstDayOfWeek);
        }

        public static DateTime InicioAbsoluto(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime FinAbsoluto(this DateTime dateTime)
        {
            return InicioAbsoluto(dateTime).AddDays(1).AddTicks(-1);
        }

        #endregion

        #region [ Extensiones - Corrección Nullables ]

        public static bool Valor(this bool? bVar)
        {
            return Helper.ConvertirBool(bVar);
        }

        public static int Valor(this int? iVar)
        {
            return Helper.ConvertirEntero(iVar);
        }

        public static decimal Valor(this decimal? mVar)
        {
            return Helper.ConvertirDecimal(mVar);
        }

        public static DateTime Valor(this DateTime? dVar)
        {
            return Helper.ConvertirFechaHora(dVar);
        }

        /* public static TimeSpan Valor(this TimeSpan? tVar)
        {
            return (tVar.HasValue ? tVar.Value : TimeSpan.MinValue);
        } */

        public static string Valor(this string sVar)
        {
            return Helper.ConvertirCadena(sVar);
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

        #region [Conversiones]

        public static int ConvertirEntero(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            int iValor;
            int.TryParse(sValor, out iValor);
            return iValor;
        }

        public static decimal ConvertirDecimal(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            decimal mValor;
            decimal.TryParse(sValor, out mValor);
            return mValor;
        }

        public static double ConvertirDoble(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            double mValor;
            double.TryParse(sValor, out mValor);
            return mValor;
        }

        public static string ConvertirCadena(object Valor)
        {
            return (Valor == null ? "" : Valor.ToString());
        }

        public static DateTime ConvertirFechaHora(object Valor)
        {
            bool bExito;
            return ConvertirFechaHora(Valor, out bExito);
        }

        public static DateTime ConvertirFechaHora(object Valor, out bool bExito)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            DateTime dValor;
            bExito = DateTime.TryParse(sValor, CultureInfo.CurrentCulture, DateTimeStyles.None, out dValor);
            return dValor;
        }

        public static bool ConvertirBool(object Valor)
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

        public static int ValorEntero(this string sValor)
        {
            if (sValor == null)
                return 0;

            sValor = sValor.Replace("$", "");
            sValor = sValor.Replace(",", "");

            return Helper.ConvertirEntero(sValor);
        }

        public static decimal ValorDecimal(this string sValor)
        {
            if (sValor == null)
                return 0;

            sValor = sValor.Replace("$", "");
            sValor = sValor.Replace(",", "");

            return Helper.ConvertirDecimal(sValor);
        }

        public static string obtenerNombreMes(int numeroMes)
        {
            try
            {
                DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(numeroMes);
                return nombreMes;
            }
            catch
            {
                return "Desconocido";
            }
        }

        #endregion
                
        #region Error Log

        public static void WriteToErrorLog(string message, string stkTrace, string title)
        {
            var path = Application.StartupPath + "\\Errors\\";
            if (!(System.IO.Directory.Exists(path)))
                System.IO.Directory.CreateDirectory(path);

            FileStream fs = new FileStream(path + "errorslog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter s = new StreamWriter(fs);
            s.Close();
            fs.Close();
            FileStream fs1 = new FileStream(path + "errorslog.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1);
            string nl = System.Environment.NewLine;
            sw.Write("Title: " + title + nl);
            sw.Write("Message: " + message + nl);
            sw.Write("StackTrace: " + stkTrace + nl);
            sw.Write("Fecha: " + DateTime.Now.ToString() + nl);
            sw.WriteLine();
            sw.Close();
            fs1.Close();
        }

        #endregion

        #region [email]

        public static void SendFileByEmail(string address, string subject, string message, string pathArchivo)
        {
            string email = "@gmail.com";
            string password = "";
            var loginInfo = new NetworkCredential(email, password);
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            var mail = new MailMessage();
            mail.From = new MailAddress(email);
            mail.To.Add(new MailAddress(address));
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = false;
            if (!string.IsNullOrEmpty(pathArchivo))
                mail.Attachments.Add(new Attachment(pathArchivo));

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;
            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                WriteToErrorLog(ex.Message, ex.StackTrace, "SendFileByEmail");
            }
        }

        public static void SendFacturaByEmail(string emailLogin, string passEmailLogin, string emailAddressTo, string subject, string message, string pathArchivoPDF, string pathArchivoXML)
        {
            string email = emailLogin;
            string password = passEmailLogin;
            var loginInfo = new NetworkCredential(email, password);
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            var mail = new MailMessage();
            mail.From = new MailAddress(email);
            mail.To.Add(new MailAddress(emailAddressTo));
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = false;
            if (!string.IsNullOrEmpty(pathArchivoPDF))
                mail.Attachments.Add(new Attachment(pathArchivoPDF));
            if (!string.IsNullOrEmpty(pathArchivoXML))
                mail.Attachments.Add(new Attachment(pathArchivoXML));
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;
            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                WriteToErrorLog(ex.Message, ex.StackTrace, "SendFileByEmail");
            }
        }

        #endregion

        #region [Validaciones - Internal]

        public static string BuildExceptionMessage(Exception exception)
        {
            return exception.Message;
        }

        public static string BuildExceptionMessage(XmlException exception, string title)
        {
            string str = string.Concat(new object[] { "Motivo : ", CleanInvalidXmlChars(exception.Message), "Número de linea : ", exception.LineNumber, "\nN\x00famero de columna : ", exception.LinePosition });
            if (!Negocio.Helper.IsEmpty(title))
            {
                str = title + "\n\n" + str;
            }
            return str;
        }

        public static bool IsEmpty(string value)
        {
            return (string.IsNullOrEmpty(value) || (value.Trim() == string.Empty));
        }

        public static string CleanInvalidXmlChars(string xmlText)
        {
            return Regex.Replace(xmlText, @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]", string.Empty);
        }

        public static int GetVersion(string version)
        {
            int num2;
            if (version == null)
            {
                return 0;
            }
            string[] strArray = version.Split(new char[] { '.' });
            version = string.Empty;
            foreach (string str in strArray)
            {
                version = version + str;
            }
            if (!int.TryParse(version, out num2))
            {
                return 0;
            }
            return num2;
        }

        public static bool IsRFCValid(string rfc)
        {
            return RFCPattern.IsMatch(rfc);
        }

        public static bool IsUUIDValid(string uuid)
        {
            return UUIDPattern.IsMatch(uuid);
        }

        public static bool IsEmailValid(string email)
        {
            return emailPattern.IsMatch(email);
        }

        public static DateTime StringToDate(string dateTime)
        {
            char[] separator = new char[] { '-' };
            string[] strArray = dateTime.Split(separator);
            return new DateTime(int.Parse(strArray[0]), int.Parse(strArray[1]), int.Parse(strArray[2]), 0, 0, 0);
        }

        public static DateTime StringToDateTime(string dateTime)
        {
            char[] separator = new char[] { '-', ' ', ':', 'T' };
            string[] strArray = dateTime.Split(separator);
            return new DateTime(int.Parse(strArray[0]), int.Parse(strArray[1]), int.Parse(strArray[2]), int.Parse(strArray[3]), int.Parse(strArray[4]), int.Parse(strArray[5]));
        }

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

        public static void CreatePfxCertFromCerCertAt(string cerLocation, string withPassword)
        {
            var directory = Path.GetDirectoryName(cerLocation);
            var fileName = Path.GetFileNameWithoutExtension(cerLocation);
            X509Certificate cert = new X509Certificate(cerLocation);
            byte[] certData = cert.Export(X509ContentType.Pkcs12, withPassword);
            System.IO.File.WriteAllBytes(directory + @"\" + fileName + ".pfx", certData);
        }

        #region [ Rotate Images ]

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

        #region [ Encriptar MD5]

        public static string Encriptar(string cadena)
        {
            string key = "fon";
            byte[] keyArray;
            byte[] arrCifrar = UTF8Encoding.UTF8.GetBytes(cadena);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();            
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] ArrayResultado = cTransform.TransformFinalBlock(arrCifrar, 0, arrCifrar.Length);
            tdes.Clear();

            return Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
        }

        public static string Desencriptar(string cadenaEncriptada)
        {
            string key = "fon";
            byte[] keyArray;
            byte[] arrDescifrar = Convert.FromBase64String(cadenaEncriptada);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(arrDescifrar, 0, arrDescifrar.Length);
            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
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
    }
}
