using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.IO;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class VisorDeReportes : Form
    {
        public int oID { get; set; }
        public int oTipoReporte { get; set; }

        public enum ReportType
        {
            Poliza = 1,
            Etiquetas = 2,
            EntradaCompra = 3,
            EntradaInventario = 4,
            SalidaInventario = 5,
            Devolucion = 6,
        }

        public static VisorDeReportes Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly VisorDeReportes instance = new VisorDeReportes();
        }

        public VisorDeReportes()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void VisorDeReportes_Load(object sender, EventArgs e)
        {
            this.reportViewer.RefreshReport();
            if (string.IsNullOrEmpty(oID.ToString()))
                return;

            if (string.IsNullOrEmpty(oTipoReporte.ToString()))
                return;

            var reportType = (ReportType)oTipoReporte;

            switch (reportType)
            {
                case ReportType.Poliza:
                    this.DisplayReportePoliza();
                    break;

                case ReportType.Etiquetas:
                    this.DisplayReporteEtiquetas();
                    break;

                case ReportType.EntradaCompra:
                    this.DisplayReporteEntradaCompra();
                    break;

                case ReportType.EntradaInventario:
                    this.DisplayReporteEntradaSalidaInventario();
                    break;

                case ReportType.SalidaInventario:
                    this.DisplayReporteEntradaSalidaInventario();
                    break;

                case ReportType.Devolucion:
                    this.DisplayReporteDevolucion();
                    break;

                default:
                    return;
            }
        }

        private void VisorDeReportes_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                reportViewer.LocalReport.ReleaseSandboxAppDomain();
            }
            catch
            { }
        }

        private void reportViewer_PrintingBegin(object sender, ReportPrintEventArgs e)
        {
            try
            {
                reportViewer.PrinterSettings.PrinterName = e.PrinterSettings.PrinterName;
                reportViewer.PrinterSettings.FromPage = e.PrinterSettings.FromPage;
                reportViewer.PrinterSettings.ToPage = e.PrinterSettings.ToPage;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Metodos ]

        static private System.IO.Stream RetriveReportDefinition(string reportResourceName)
        {
            var assembly = typeof(string).Assembly;
            var resources = assembly.GetManifestResourceNames();
            var reportResource = resources.Where(c => c.EndsWith(reportResourceName)).FirstOrDefault();
            var reportDefinitionStream = assembly.GetManifestResourceStream(reportResource);
            return reportDefinitionStream;
        }

        private void DisplayReportePoliza()
        {
            try
            {
                var rutaReportes = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                reportViewer.LocalReport.ReportPath = rutaReportes + @"\Reportes\ReportePoliza.rdlc";
                reportViewer.LocalReport.DisplayName = string.Format("{0}{1}", "Poliza: ", oID.ToString());
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.EnableExternalImages = true;
                var cabeceraPoliza = Negocio.General.GetListOf<ProveedorReportePolizasView>(pp => pp.ProveedorPolizaID.Equals(oID));
                var detallePoliza = Negocio.General.GetListOf<ProveedorReportePolizasDetalleView>(pd => pd.ProveedorPolizaID.Equals(oID));

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsPoliza", Value = cabeceraPoliza });
                reportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource() { Name = "dsPolizaDetalle", Value = detallePoliza });

                this.reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(MySubreportEventHandler);

                this.reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        void MySubreportEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                switch (e.ReportPath)
                {
                    case "ReporteDevolucion":
                        var movimientoInventarioId = Negocio.Helper.ConvertirEntero(e.Parameters["AplicaEnMovimientoInventarioID"].Values[0].ToString());
                        var proveedorPoliza = Negocio.General.GetEntity<ProveedorPoliza>(p => p.ProveedorPolizaID.Equals(oID));
                        var detalleDevolucion = Negocio.General.GetListOf<ProveedorReporteDevolucionDetalleView>(p => p.ProveedorID == proveedorPoliza.ProveedorID && p.AplicaEnMovimientoInventarioID == movimientoInventarioId);
                        e.DataSources.Add(new ReportDataSource("dsDevolucion", detalleDevolucion));
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void DisplayReporteEtiquetas()
        {
            try
            {
                var rutaReportes = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                reportViewer.LocalReport.ReportPath = rutaReportes + @"\Reportes\ReporteEtiquetas.rdlc";
                reportViewer.LocalReport.DisplayName = string.Empty;
                reportViewer.LocalReport.DataSources.Clear();

                var movimientoEtiquetas = General.GetListOf<MovimientoInventarioEtiquetasView>(l => l.MovimientoInventarioID.Equals(oID));
                var listaEtiquetas = new List<Etiquetas>();

                var cont = 0;
                foreach (var etiqueta in movimientoEtiquetas)
                {
                    for (var x = 0; x < etiqueta.NumeroEtiquetas; x++)
                    {
                        cont += 1;
                        var e = new Etiquetas()
                        {
                            MovimientoInventarioEtiquetaID = cont,
                            MovimientoInventarioID = oID,
                            ParteID = etiqueta.ParteID,
                            NumeroParte = etiqueta.NumeroParte,
                            CodigoBarra = etiqueta.CodigoBarra,
                            NumeroEtiquetas = 1
                        };
                        listaEtiquetas.Add(e);
                    }
                }

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsEtiquetas", Value = listaEtiquetas });

                //this.reportViewer.ShowZoomControl = true;
                //this.reportViewer.ZoomMode = ZoomMode.PageWidth;
                this.reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void DisplayReporteEntradaCompra()
        {
            try
            {
                var rutaReportes = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                reportViewer.LocalReport.ReportPath = rutaReportes + @"\Reportes\ReporteEntradaCompra.rdlc";
                reportViewer.LocalReport.DisplayName = string.Empty;
                reportViewer.LocalReport.DataSources.Clear();

                var movimiento = General.GetListOf<MovimientoInventarioView>(m => m.MovimientoInventarioID.Equals(oID));
                var detalle = General.GetListOf<MovimientoInventarioDetalleView>(d => d.MovimientoInventarioID.Equals(oID));

                var detalleDescuentosGen = new List<MovimientoInventarioDescuentoView>();
                var detalleDescuentosGenerales = General.GetListOf<MovimientoInventarioDescuentoView>(m => m.MovimientoInventarioID.Equals(oID)
                    && (m.TipoDescuentoID.Equals(1) || m.TipoDescuentoID.Equals(4) || m.TipoDescuentoID.Equals(2) || m.TipoDescuentoID.Equals(5)));

                bool uno = false;
                bool cuatro = false;
                bool dos = false;
                bool cinco = false;
                foreach (var descuentoGen in detalleDescuentosGenerales)
                {
                    if (descuentoGen.TipoDescuentoID.Equals(1) && uno.Equals(false))
                    {
                        uno = true;
                        detalleDescuentosGen.Add(descuentoGen);
                    }
                    if (descuentoGen.TipoDescuentoID.Equals(4) && cuatro.Equals(false))
                    {
                        cuatro = true;
                        detalleDescuentosGen.Add(descuentoGen);
                    }
                    if (descuentoGen.TipoDescuentoID.Equals(2) && dos.Equals(false))
                    {
                        dos = true;
                        detalleDescuentosGen.Add(descuentoGen);
                    }
                    if (descuentoGen.TipoDescuentoID.Equals(5) && cinco.Equals(false))
                    {
                        cinco = true;
                        detalleDescuentosGen.Add(descuentoGen);
                    }
                }

                var detalleDescuentosInd = General.GetListOf<MovimientoInventarioDescuentoView>(m => m.MovimientoInventarioID.Equals(oID)
                    && m.TipoDescuentoID.Equals(3));

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsMovimientoInventario", Value = movimiento });
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsMovimientoInventarioDetalle", Value = detalle });
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsDescuentosGen", Value = detalleDescuentosGen });
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsDescuentosInd", Value = detalleDescuentosInd });

                //this.reportViewer.ShowZoomControl = true;
                //this.reportViewer.ZoomMode = ZoomMode.PageWidth;
                this.reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void DisplayReporteEntradaSalidaInventario()
        {
            try
            {
                var rutaReportes = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                reportViewer.LocalReport.ReportPath = rutaReportes + @"\Reportes\ReporteEntradaSalida.rdlc";
                reportViewer.LocalReport.DisplayName = string.Empty;
                reportViewer.LocalReport.DataSources.Clear();

                var movimiento = General.GetListOf<MovimientoInventarioView>(m => m.MovimientoInventarioID.Equals(oID));
                var detalle = General.GetListOf<MovimientoInventarioDetalleView>(d => d.MovimientoInventarioID.Equals(oID));

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsMovimientoInventario", Value = movimiento });
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsMovimientoInventarioDetalle", Value = detalle });

                this.reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer.RefreshReport();                            
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void DisplayReporteDevolucion()
        {
            try
            {
                var rutaReportes = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                reportViewer.LocalReport.ReportPath = rutaReportes + @"\Reportes\ReporteSalidaDevolucion.rdlc";
                reportViewer.LocalReport.DisplayName = string.Empty;
                reportViewer.LocalReport.DataSources.Clear();

                var movimiento = General.GetListOf<MovimientoInventarioView>(m => m.MovimientoInventarioID.Equals(oID));
                var detalle = General.GetListOf<MovimientoInventarioDetalleView>(d => d.MovimientoInventarioID.Equals(oID));
                                
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsMovimientoInventario", Value = movimiento });
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource() { Name = "dsMovimientoInventarioDetalle", Value = detalle });
               
                //this.reportViewer.ShowZoomControl = true;
                //this.reportViewer.ZoomMode = ZoomMode.PageWidth;
                this.reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
                this.reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void ExportReportViewer2Pdf(ReportViewer rpt, string pathPDF)
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;
                string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
                byte[] bytes = rpt.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                FileStream fs = new FileStream(pathPDF, FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                Negocio.Helper.WriteToErrorLog(ex.Message, ex.StackTrace, this.Name);
            }
        }

        #endregion

    }
}
