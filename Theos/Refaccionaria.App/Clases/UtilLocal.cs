using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Xml;
using System.Threading;
using FastReport;
using AdvancedDataGridView;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public static class UtilLocal
    {
        #region [ Constantes ]

        public static Color ColorDeFondo = Color.FromArgb(67, 87, 123);

        #endregion

        #region [ Datos ]

        public static void LlenarUsuarioGlobal(Usuario oUsuario)
        {
            GlobalClass.UsuarioGlobal = new UsuarioSis();
            GlobalClass.UsuarioGlobal.UsuarioID = oUsuario.UsuarioID;
            GlobalClass.UsuarioGlobal.NombreUsuario = oUsuario.NombreUsuario;
            GlobalClass.UsuarioGlobal.NombrePersona = oUsuario.NombrePersona;
            GlobalClass.UsuarioGlobal.Perfiles = Datos.GetListOf<UsuarioPerfilesView>(up => up.UsuarioID.Equals(oUsuario.UsuarioID));

            var ppv = Datos.GetListOf<PerfilPermisosView>().ToList();
            var permisos = new List<PerfilPermisosView>();

            foreach (var perfil in GlobalClass.UsuarioGlobal.Perfiles)
            {
                var items = ppv.Where(p => p.PerfilID == perfil.PerfilID);
                foreach (var item in items)
                    permisos.Add(item);
            }

            GlobalClass.UsuarioGlobal.Permisos = permisos;
        }

        public static bool AgregarParametroListaEntero(ref Dictionary<string, object> oParams, string sParam, ComboMultiSel oCombo)
        {
            if (oCombo.ValoresSeleccionados.Count > 0)
            {
                var oDt = Util.ListaEntityADataTable(oCombo.ElementosSeleccionados);
                oDt.Columns.Remove("Cadena");
                oParams.Add(sParam, oDt);
                return true;
            }
            return false;
        }

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
            Res.Codigo = (int)frmValidar.DialogResult;
            frmValidar.Dispose();

            return Res;
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario(List<string> oPermisos, bool bCumplirTodosLosPermisos)
        {
            return UtilLocal.ValidarObtenerUsuario(oPermisos, bCumplirTodosLosPermisos, null);
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario(string sPermiso, string sTitulo)
        {
            return UtilLocal.ValidarObtenerUsuario((sPermiso == null ? null : new List<string>() { sPermiso }), true, sTitulo);
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario(string sPermiso)
        {
            return UtilLocal.ValidarObtenerUsuario(new List<string>() { sPermiso }, true, null);
        }

        public static ResAcc<Usuario> ValidarObtenerUsuario()
        {
            return UtilLocal.ValidarObtenerUsuario(null, false, null);
        }

        public static bool ValidarPermiso(int iUsuarioID, string sPermiso, bool bMostrarMensaje)
        {
            bool bValido = Datos.Exists<UsuariosPermisosView>(c => c.UsuarioID == iUsuarioID && c.Permiso == sPermiso);
            if (bValido)
            {
                return true;
            }
            else
            {
                if (bMostrarMensaje)
                {
                    var oPermiso = Datos.GetEntity<Permiso>(c => c.NombrePermiso == sPermiso);
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
            return UtilLocal.ValidarPermiso(iUsuarioID, sPermiso, false);
        }

        public static bool ValidarPermiso(string sPermiso, bool bMostrarMensaje)
        {
            return UtilLocal.ValidarPermiso(Theos.UsuarioID, sPermiso, bMostrarMensaje);
        }

        public static bool ValidarPermiso(string sPermiso)
        {
            return UtilLocal.ValidarPermiso(Theos.UsuarioID, sPermiso, false);
        }

        #endregion

        public static string DecimalToCadenaMoneda(object Valor)
        {
            string sValor = (Valor == null ? "" : Valor.ToString());
            decimal mValor;
            decimal.TryParse(sValor, out mValor);
            return String.Format("{0:C2}", mValor).Replace("$", "");
        }

        #region [ Rutas ]

        public static string RutaAplicacion()
        {
            return (Directory.GetCurrentDirectory() + "\\");
        }

        public static string RutaImagenes()
        {
            return (UtilLocal.RutaAplicacion() + GlobalClass.RutaImg + "\\");
        }

        public static string RutaImagenesProveedores()
        {
            return (UtilLocal.RutaImagenes() + "Proveedores\\");
        }

        public static string RutaImagenesMarcas()
        {
            return (UtilLocal.RutaImagenes() + "Marcas\\");
        }

        public static string RutaImagenesLineas()
        {
            return (UtilLocal.RutaImagenes() + "Lineas\\");
        }

        public static string RutaRecursos(string sArchivo)
        {
            return (UtilLocal.RutaAplicacion() + "Recursos\\" + sArchivo);
        }

        public static string RutaRecursos()
        {
            return UtilLocal.RutaRecursos("");
        }

        public static string RutaReportes(string sArchivo)
        {
            return (UtilLocal.RutaAplicacion() + "Reportes\\" + sArchivo);
        }

        public static string RutaReportes()
        {
            return UtilLocal.RutaReportes("");
        }

        public static string RutaImagenClientePersonalFirma(int iClientePersonalID)
        {
            var iClienteID = Datos.GetEntity<ClientePersonal>(cp => cp.ClientePersonalID == iClientePersonalID).ClienteID;
            //C:\tmp\CRImg\Clientes\[clienteid]\PersonalFirma-009.jpg
            String ClientePersonalIDCadena = iClientePersonalID.ToString("000");
            
            //Util.MensajeInformacion(String.Format("{0}{1}{2}{3}{4}{5}", GlobalClass.ConfiguracionGlobal.pathImagenes.ToString(), "Clientes\\", iClienteID, "\\PersonalFirma-", ClientePersonalIDCadena, ".jpg"), "");
            return (String.Format("{0}{1}{2}{3}{4}{5}", GlobalClass.ConfiguracionGlobal.pathImagenes.ToString(), "Clientes\\", iClienteID, "\\PersonalFirma-", ClientePersonalIDCadena, ".jpg"));
        }

        #endregion

        #region [ Cuadros de mensaje ]

        public static object ObtenerValor(string sMensaje, string sValorPredefinido, MensajeObtenerValor.Tipo Tipo)
        {
            object Res = null;
            var frmValor = new MensajeObtenerValor(sMensaje, sValorPredefinido, Tipo);
            frmValor.CapitalizacionDeTexto = CharacterCasing.Upper;
            frmValor.ShowDialog(Principal.Instance);
            if (frmValor.DialogResult == DialogResult.OK)
                Res = frmValor.Valor;
            frmValor.Dispose();
            return Res;
        }

        public static void MostrarNotificacion(string sMensaje)
        {
            new Notificacion(sMensaje, GlobalClass.TiempoNotificacion).Mostrar(Principal.Instance);
        }

        public static DialogResult MensajeAdvertencia(string sMensaje)
        {
            return Util.MensajeAdvertencia(sMensaje, GlobalClass.NombreApp);
        }

        public static DialogResult MensajePregunta(string sMensaje)
        {
            return Util.MensajePregunta(sMensaje, GlobalClass.NombreApp);
        }

        public static DialogResult MensajePreguntaCancelar(string sMensaje)
        {
            return MessageBox.Show(sMensaje, GlobalClass.NombreApp, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
        }

        public static DialogResult MensajeInformacion(string sMensaje)
        {
            return Util.MensajeInformacion(sMensaje, GlobalClass.NombreApp);
        }
        
        public static DialogResult MensajeError(string sMensaje)
        {
            return Util.MensajeError(sMensaje, GlobalClass.NombreApp);
        }
        #endregion

        #region [ Controles de UI ]

        public static void VerShift(this DataGridView Grid, KeyEventArgs e, string sColumnaCheck)
        {
            if (Grid.CurrentRow == null) return;

            if (e.KeyCode == Keys.ShiftKey)
            {
                // Si la celda es de sólo lectura, no se hace nada
                if (Grid.CurrentRow.Cells[sColumnaCheck].ReadOnly) return;

                // Para corregir extraño detalle de que si está seleccionada la celda de "Aplicar", no se cambia el valor
                if (Grid.CurrentCell.OwningColumn.Name == sColumnaCheck)
                    Grid.EndEdit();

                Grid.CurrentRow.Cells[sColumnaCheck].Value = !Util.Logico(Grid.CurrentRow.Cells[sColumnaCheck].Value);
            }
        }

        public static void VerEspacioCheck(this DataGridView Grid, KeyEventArgs e, string sColumnaCheck)
        {
            if (Grid.CurrentRow == null) return;

            if (e.KeyCode == Keys.Space)
            {
                // Si la celda es de sólo lectura, no se hace nada
                if (Grid.CurrentRow.Cells[sColumnaCheck].ReadOnly) return;

                // Para corregir extraño detalle de que si está seleccionada la celda de "Aplicar", no se cambia el valor
                if (Grid.CurrentCell.OwningColumn.Name == sColumnaCheck)
                    Grid.EndEdit();

                Grid.CurrentRow.Cells[sColumnaCheck].Value = !Util.Logico(Grid.CurrentRow.Cells[sColumnaCheck].Value);
            }
        }

        public static void VerDirtyStateChanged(this DataGridView Grid, string sColumnaVer)
        {
            if (!Grid.IsCurrentCellDirty) return;
            if (Grid.CurrentCell.OwningColumn.Name == sColumnaVer)
            {
                Grid.EndEdit();
            }
        }

        public static bool VerSeleccionNueva(this DataGridView oGrid)
        {
            bool bCambio = (oGrid.CurrentRow != (oGrid.Tag as DataGridViewRow));
            oGrid.Tag = oGrid.CurrentRow;
            return bCambio;
        }

        public static void FormatoDecimal(this DataGridViewColumn oColumna)
        {
            oColumna.DefaultCellStyle.Format = "N2";
            oColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            oColumna.Width = 60;
        }

        public static void FormatoMoneda(this DataGridViewColumn Columna)
        {
            Columna.DefaultCellStyle.Format = "C2";
            Columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            Columna.Width = 80;
        }

        public static DataGridViewColumn AgregarColumna(this DataGridView oGrid, string sNombre, string sEncabezado, int iAncho)
        {
            oGrid.Columns.Add(sNombre, sEncabezado);
            oGrid.Columns[sNombre].Width = iAncho;

            return oGrid.Columns[sNombre];
        }

        public static DataGridViewColumn AgregarColumnaDecimal(this DataGridView oGrid, string sNombre, string sEncabezado)
        {
            var oColumna = oGrid.AgregarColumna(sNombre, sEncabezado, 60);
            oColumna.FormatoDecimal();

            return oColumna;
        }

        public static DataGridViewColumn AgregarColumnaImporte(this DataGridView oGrid, string sNombre, string sEncabezado)
        {
            var oColumna = oGrid.AgregarColumna(sNombre, sEncabezado, 80);
            oColumna.FormatoMoneda();

            return oColumna;
        }

        public static DataGridViewComboBoxColumn AgregarColumnaCombo(this DataGridView oGrid, string sNombre, string sEncabezado, int iAncho)
        {
            var oColumna = new DataGridViewComboBoxColumn()
            {
                Name = sNombre,
                HeaderText = sEncabezado,
                Width = iAncho,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                FlatStyle = FlatStyle.Flat
            };
            oGrid.Columns.Add(oColumna);

            return oColumna;
        }

        public static DataGridViewCheckBoxColumn AgregarColumnaCheck(this DataGridView oGrid, string sNombre, string sEncabezado, int iAncho)
        {
            var oColumna = new DataGridViewCheckBoxColumn()
            {
                Name = sNombre,
                HeaderText = sEncabezado,
                Width = iAncho,
            };
            oGrid.Columns.Add(oColumna);

            return oColumna;
        }

        public static void Refrescar(this System.Windows.Forms.DataVisualization.Charting.Chart oGrafico)
        {
            oGrafico.Invalidate();
            oGrafico.Update();
        }
        
        #endregion
                
        #region [ Reportes ]

        public static void EnviarReporteASalida(string sParametro, Report oReporte, bool bUsarPreparado)
        {
            // Se abre el xml correspondiente al reporte
            var oXmlRep = new XmlDocument();
            oXmlRep.Load(oReporte.FileName);
            
            // Se obtiene el tipo de salida, primero del xml del reporte, si no, de la base de datos
            string sTipoDeSalida = "";
            if (oXmlRep.DocumentElement.Attributes["Cr_Salida"] == null)
                sTipoDeSalida = Config.Valor(sParametro);
            else
                sTipoDeSalida = oXmlRep.DocumentElement.Attributes["Cr_Salida"].Value;

            switch (sTipoDeSalida.ToLower())
            {
                case "d":
                    oReporte.Design();
                    break;
                case "p":
                    if (bUsarPreparado)
                        oReporte.ShowPrepared();
                    else
                        oReporte.Show();
                    break;
                case "i":
                    bool bDialogo = (oReporte.PrintSettings.Printer == "");
                    oReporte.PrintSettings.ShowDialog = bDialogo;

                    // Se intenta mandar a imprimir
                    try
                    {
                        if (bUsarPreparado)
                            oReporte.PrintPrepared();
                        else
                            oReporte.Print();

                        if (bDialogo)
                        {
                            if (UtilLocal.MensajePregunta("¿Deseas guardar los datos de impresión especificados como predeterminados para este reporte?") == DialogResult.Yes)
                            {
                                oReporte.PrintSettings.SavePrinterWithReport = true;
                                oReporte.Save(oReporte.FileName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        UtilLocal.MensajeError(e.Message);
                    }

                    break;
            }
        }

        public static void EnviarReporteASalida(string sParametro, Report oReporte)
        {
            UtilLocal.EnviarReporteASalida(sParametro, oReporte, false);
        }

        public static void AbrirEnExcel(DataGridView oGrid)
        {
            Cargando.Mostrar();
            string sArchivo = (Path.GetTempFileName() + ".csv");
            oGrid.ExportarACsv(sArchivo, true);
            System.Diagnostics.Process.Start("excel", ("\"" + sArchivo + "\""));
            Cargando.Cerrar();
        }

        #endregion

        #region [ Ayuda Eventos ]

        public static void TextLeaveFormatoMoneda(object sender, EventArgs e)
        {
            var oTexto = (sender as TextBox);
            if (oTexto == null)
                return;

            oTexto.Text = oTexto.Text.ValorDecimal().ToString(GlobalClass.FormatoMoneda);
        }

        #endregion

        #region [ Otros ]

        public static bool EnviarCorreo(string sAsunto, string sMensaje, string sPara, Dictionary<string, Stream> Adjuntos)
        {
            if (!UtilLocal.IsEmailValid(sPara))
                return false;

            SmtpClient Correo = new SmtpClient();
            NetworkCredential Acceso = new NetworkCredential();
            MailMessage Mensaje = new MailMessage();

            var oConfig = Config.ValoresVarios("Correo.");

            Correo.Host = oConfig["Correo.Servidor"];
            Correo.Port = Util.Entero(oConfig["Correo.Puerto"]);
            Correo.EnableSsl = (oConfig["Correo.Ssl"].ToLower() == "v");
            Acceso.UserName = oConfig["Correo.Usuario"];
            Acceso.Password = oConfig["Correo.Contrasenia"];
            Correo.Credentials = Acceso;

            Mensaje.From = new MailAddress(Acceso.UserName, oConfig["Correo.Nombre"]);
            Mensaje.To.Add(sPara);
            Mensaje.Subject = sAsunto;
            Mensaje.Body = sMensaje;
            Mensaje.IsBodyHtml = true;
            if (Adjuntos != null && Adjuntos.Count > 0)
            {
                foreach (var oAdjunto in Adjuntos)
                    Mensaje.Attachments.Add(new Attachment(oAdjunto.Value, oAdjunto.Key));
            }

            try
            {
                Correo.SendCompleted += new SendCompletedEventHandler((s, e) =>
                {
                    var oMensaje = (e.UserState as MailMessage);
                    if (oMensaje != null)
                    {
                        foreach (var oAdjunto in oMensaje.Attachments)
                            oAdjunto.ContentStream.Close();
                    }
                });
                Correo.SendAsync(Mensaje, Mensaje);
            }
            catch { return false; }

            return true;
        }
        
        public static bool EnviarCorreo(string sAsunto, string sMensaje, string sPara)
        {
            return UtilLocal.EnviarCorreo(sAsunto, sMensaje, sPara, null);
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

        #region [ Formularios especiales ]

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

        public static DateTime InicioAbsoluto(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime FinAbsoluto(this DateTime dateTime)
        {
            return InicioAbsoluto(dateTime).AddDays(1).AddTicks(-1);
        }

        public static int findRowIndex(DataGridView dgv, string cellName, string searchValue)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (Util.Cadena(row.Cells[cellName].Value).Equals(searchValue))
                {
                    return row.Index;
                }
            }
            return -1;
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

        public static bool IsEmailValid(string email)
        {
            var emailPattern = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            return emailPattern.IsMatch(email);
        }

        public static string LimpiarCadenaDeEspaciosBlancos(this string sCadena)
        {
            return System.Text.RegularExpressions.Regex.Replace(sCadena, @"\s+", " ").Trim();
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

        public static DataTable newTable<T>(string name, IEnumerable<T> list)
        {
            PropertyInfo[] pi = typeof(T).GetProperties();

            DataTable table = Table<T>(name, list, pi);

            IEnumerator<T> e = list.GetEnumerator();

            while (e.MoveNext())
                table.Rows.Add(UtilLocal.newRow<T>(table.NewRow(), e.Current, pi));

            return table;
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

        public static string ToMonthName(this DateTime dateTime)
        {
            DateTimeFormatInfo c = new CultureInfo("es-MX", false).DateTimeFormat;
            return c.GetMonthName(dateTime.Month);
        }

        #endregion

        #region [ Avisos ]

        public static void VerInventarioConteoPendiente(object o)
        {
            // Se comprueba si hay conteos pendientes en la sucursal, cuando aplique
            string sMensaje = "";
            var oConteosPen = UtilDatos.InventarioUsuariosConteoPendiente();
            foreach (string sUsuario in oConteosPen)
                sMensaje += string.Format("El usuario {0} no ha concluido su conteo.\n", sUsuario);
            if (sMensaje != "")
                Principal.Instance.MostrarMensaje(new DosVal<string, string>("Recordatorio de Invenario", sMensaje));
        }

        #endregion
    }
}
