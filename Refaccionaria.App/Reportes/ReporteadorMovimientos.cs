using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ReporteadorMovimientos
    {
        public static ReporteadorMovimientos Instance
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

            internal static readonly ReporteadorMovimientos instance = new ReporteadorMovimientos();
        }

        public ReporteadorMovimientos()
        {

        }

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
            Traspaso = 7,
        }

        #region [ Metodos ]

        public void Load()
        {
            if (string.IsNullOrEmpty(oID.ToString()))
                return;

            if (string.IsNullOrEmpty(oTipoReporte.ToString()))
                return;

            var reportType = (ReportType)oTipoReporte;

            switch (reportType)
            {
                case ReportType.Poliza:
                    //this.DisplayReportePoliza();
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

                case ReportType.Traspaso:
                    this.DisplayReporteTraspaso();
                    break;

                default:
                    return;
            }
        }

        private void DisplayReporteEntradaCompra()
        {
            try
            {
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

                foreach (var descuentoGen in detalleDescuentosInd)
                {
                    detalleDescuentosGen.Add(descuentoGen);
                }

                IEnumerable<MovimientoInventarioView> movimientoE = movimiento;
                IEnumerable<MovimientoInventarioDetalleView> detalleE = detalle;
                IEnumerable<MovimientoInventarioDescuentoView> descuentosGen = detalleDescuentosGen;
                //IEnumerable<MovimientoInventarioDescuentoView> descuentosInd = detalleDescuentosInd;

                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteEntradaCompra.frx"));
                    report.RegisterData(movimientoE, "movimiento", 3);
                    report.RegisterData(detalleE, "detalle", 3);
                    report.RegisterData(descuentosGen, "descuentosGenerales", 3);
                    //report.RegisterData(descuentosInd, "descuentosIndividuales", 3);
                    report.GetDataSource("movimiento").Enabled = true;
                    report.GetDataSource("detalle").Enabled = true;
                    report.GetDataSource("descuentosGenerales").Enabled = true;
                    //report.GetDataSource("descuentosIndividuales").Enabled = true;
                    report.Show(true);
                }

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
                var movimiento = General.GetListOf<MovimientoInventarioView>(m => m.MovimientoInventarioID.Equals(oID));
                var movimientoDetalle = General.GetListOf<MovimientoInventarioDetalleView>(d => d.MovimientoInventarioID.Equals(oID));

                IEnumerable<MovimientoInventarioView> movimientoE = movimiento;
                IEnumerable<MovimientoInventarioDetalleView> detalleE = movimientoDetalle;

                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteEntradaSalida.frx"));
                    report.RegisterData(movimientoE, "movimiento", 3);
                    report.RegisterData(detalleE, "detalle", 3);
                    report.GetDataSource("movimiento").Enabled = true;
                    report.GetDataSource("detalle").Enabled = true;
                    report.Show(true);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void DisplayReporteEtiquetas()
        {
            try
            {
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
                            NumeroEtiquetas = 1,
                            NombreParte = etiqueta.NombreParte
                        };
                        listaEtiquetas.Add(e);
                    }
                }

                IEnumerable<Etiquetas> detalleE = listaEtiquetas;
                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteEtiquetas.frx"));
                    report.RegisterData(detalleE, "etiquetas", 3);
                    report.GetDataSource("etiquetas").Enabled = true;
                    report.Show(true);
                    //report.Design(true);
                }

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
                var movimiento = General.GetListOf<MovimientoInventarioView>(m => m.MovimientoInventarioID.Equals(oID));
                var detalle = General.GetListOf<MovimientoInventarioDetalleView>(d => d.MovimientoInventarioID.Equals(oID));

                IEnumerable<MovimientoInventarioView> movimientoE = movimiento;
                IEnumerable<MovimientoInventarioDetalleView> detalleE = detalle;

                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteSalidaDevolucion.frx"));
                    report.RegisterData(movimientoE, "movimiento", 3);
                    report.RegisterData(detalleE, "detalle", 3);
                    report.GetDataSource("movimiento").Enabled = true;
                    report.GetDataSource("detalle").Enabled = true;
                    //report.Show(true);
                    UtilLocal.EnviarReporteASalida("Reportes.DevolucionAProveedor.Salida", report);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void DisplayReporteTraspaso()
        {
            try
            {
                var movimiento = General.GetListOf<MovimientoInventarioView>(m => m.MovimientoInventarioID.Equals(oID));
                var detalle = General.GetListOf<MovimientoInventarioDetalleView>(d => d.MovimientoInventarioID.Equals(oID));

                IEnumerable<MovimientoInventarioView> movimientoE = movimiento;
                IEnumerable<MovimientoInventarioDetalleView> detalleE = detalle;

                using (FastReport.Report report = new FastReport.Report())
                {
                    if (detalle.Count < 10)
                        report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteTraspaso.frx"));
                    else
                        report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteTraspasosMasDeDiez.frx"));

                    report.RegisterData(movimientoE, "movimiento", 3);
                    report.RegisterData(detalleE, "detalle", 3);
                    report.GetDataSource("movimiento").Enabled = true;
                    report.GetDataSource("detalle").Enabled = true;
                    report.Show(true);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

    }
}
