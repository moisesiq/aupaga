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

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public static class UtilLocal
    {
        #region [ Constantes ]

        public static Color ColorDeFondo = Color.FromArgb(67, 87, 123);

        #endregion

        public static decimal ObtenerPrecioSinIva(decimal mPrecio, int iDecimales)
        {
            return Math.Round(mPrecio / (1 + (GlobalClass.ConfiguracionGlobal.IVA / 100)), iDecimales);
        }

        public static decimal ObtenerPrecioSinIva(decimal mPrecio)
        {
            return UtilLocal.ObtenerPrecioSinIva(mPrecio, 2);
        }

        public static decimal ObtenerIvaDePrecio(decimal mPrecio, int iDecimales)
        {
            // return Math.Round(mPrecio - UtilLocal.ObtenerPrecioSinIva(mPrecio), 2);
            return (mPrecio - UtilLocal.ObtenerPrecioSinIva(mPrecio, iDecimales));
        }

        public static decimal ObtenerIvaDePrecio(decimal mPrecio)
        {
            return UtilLocal.ObtenerIvaDePrecio(mPrecio, 2);
        }

        public static decimal ObtenerImporteMasIva(decimal mImporte)
        {
            return Math.Round(mImporte * (1 + (GlobalClass.ConfiguracionGlobal.IVA / 100)), 2);
        }

        public static decimal ObtenerIvaDeSubtotal(decimal mImporte)
        {
            return Math.Round(mImporte * (GlobalClass.ConfiguracionGlobal.IVA / 100), 2);
        }

        public static int SemanaSabAVie(DateTime dFecha)
        {
            int iAnio = dFecha.Year;
            int iDia = dFecha.DayOfYear;
            // Se obtiene el día del primer Sábado del año
            int iDiaUno = (6 - (int)(new DateTime(iAnio, 1, 1).DayOfWeek) + 1);
            DateTime dFechaUno = new DateTime(iAnio, 1, iDiaUno);

            int iSemana;
            if (iDia < iDiaUno)
            {  // Es un día antes de la semana uno
                // Se obtiene el primer día del año pasado
                int iDiaUnoPas = (6 - (int)(new DateTime(iAnio - 1, 1, 1).DayOfWeek) + 1);
                DateTime dFechaUnoPas = new DateTime(iAnio - 1, 1, iDiaUnoPas);
                // Se obtiene la semana
                double mDiasDif = (dFecha - dFechaUnoPas).TotalDays;
                if (mDiasDif % 7 == 0) mDiasDif += 0.1;
                iSemana = (int)Math.Ceiling(mDiasDif / 7);
            }
            else
            {
                // Se obtiene la semana
                double mDiasDif = (dFecha - dFechaUno).TotalDays;
                if (mDiasDif % 7 == 0) mDiasDif += 0.1;
                iSemana = (int)Math.Ceiling(mDiasDif / 7);
            }

            return iSemana;
        }

        public static DateTime InicioSemanaSabAVie(int iAnio, int iSemana)
        {
            // Se obtiene el día del primer Sábado del año
            int iDiaUno = (6 - (int)(new DateTime(iAnio, 1, 1).DayOfWeek) + 1);
            DateTime dFechaUno = new DateTime(iAnio, 1, iDiaUno);

            return dFechaUno.AddDays((iSemana - 1) * 7).Date;
        }

        public static DateTime InicioSemanaSabAVie(DateTime dBase)
        {
            DateTime d = (dBase.DayOfWeek == DayOfWeek.Saturday ? dBase : dBase.AddDays((int)dBase.DayOfWeek * -1).AddDays(-1));
            return d.Date;
        }

        public static decimal GastoCalcularImporteDiario(DateTime dFecha, decimal mImporte, int iPeriodicidadMes)
        {
            DateTime dInicioPer = dFecha.DiaPrimero().Date;
            DateTime dFinPer = dInicioPer.AddMonths(iPeriodicidadMes).AddDays(-1);
            decimal mImporteDiario = (mImporte / ((dFinPer - dInicioPer).Days + 1));
            return mImporteDiario;
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

        public static string RutaRecursos()
        {
            return (UtilLocal.RutaAplicacion() + "Recursos\\");
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
            var iClienteID = General.GetEntity<ClientePersonal>(cp => cp.ClientePersonalID == iClientePersonalID).ClienteID;
            //C:\tmp\CRImg\Clientes\[clienteid]\PersonalFirma-009.jpg
            String ClientePersonalIDCadena = iClientePersonalID.ToString("000");
            
            //Helper.MensajeInformacion(String.Format("{0}{1}{2}{3}{4}{5}", GlobalClass.ConfiguracionGlobal.pathImagenes.ToString(), "Clientes\\", iClienteID, "\\PersonalFirma-", ClientePersonalIDCadena, ".jpg"), "");
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
            return Helper.MensajeAdvertencia(sMensaje, GlobalClass.NombreApp);
        }

        public static DialogResult MensajePregunta(string sMensaje)
        {
            return Helper.MensajePregunta(sMensaje, GlobalClass.NombreApp);
        }

        public static DialogResult MensajePreguntaCancelar(string sMensaje)
        {
            return MessageBox.Show(sMensaje, GlobalClass.NombreApp, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
        }

        public static DialogResult MensajeInformacion(string sMensaje)
        {
            return Helper.MensajeInformacion(sMensaje, GlobalClass.NombreApp);
        }
        
        public static DialogResult MensajeError(string sMensaje)
        {
            return Helper.MensajeError(sMensaje, GlobalClass.NombreApp);
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

                Grid.CurrentRow.Cells[sColumnaCheck].Value = !Helper.ConvertirBool(Grid.CurrentRow.Cells[sColumnaCheck].Value);
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

                Grid.CurrentRow.Cells[sColumnaCheck].Value = !Helper.ConvertirBool(Grid.CurrentRow.Cells[sColumnaCheck].Value);
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
            if (!Helper.IsEmailValid(sPara))
                return false;

            SmtpClient Correo = new SmtpClient();
            NetworkCredential Acceso = new NetworkCredential();
            MailMessage Mensaje = new MailMessage();

            var oConfig = Config.ValoresVarios("Correo.");

            Correo.Host = oConfig["Correo.Servidor"];
            Correo.Port = Helper.ConvertirEntero(oConfig["Correo.Puerto"]);
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
    }
}
