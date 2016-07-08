using System;
using System.Windows.Forms;
using System.Linq;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroVendedores : CuadroMultiple
    {
        public CuadroVendedores()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroVendedores_Load(object sender, EventArgs e)
        {
            // Se configuran las columnas
            this.dgvPrincipal.Columns["Principal_Nombre"].HeaderText = "Vendedor";

            // Se manda a cargar los datos
            // this.CargarDatos();
        }

        private void chkGrafSemanas_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkGrafSemanas.Checked)
            {
                this.LlenarGraficaSemanasTodos();
            }
            else
            {
                int iVendedorID = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                this.LlenarSemanas(iVendedorID);
            }
        }

        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid principal
            var oVendedores = this.AgruparPorEnteroCadena(oDatos.GroupBy(g => new EnteroCadenaComp() { Entero = g.VendedorID, Cadena = g.Vendedor }))
                .OrderByDescending(c => c.Actual);
            decimal mTotal = (oVendedores.Count() > 0 ? oVendedores.Sum(c => c.Actual) : 0);
            this.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oVendedores)
            {
                this.dgvPrincipal.Rows.Add(oReg.Llave, oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
            // Se llenan los totales
            decimal mTotalAnt = (oVendedores.Count() > 0 ? oVendedores.Sum(c => c.Anterior) : 0);
            this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotal;
            this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalAnt;
            this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(mTotal, mTotalAnt);

            // Para configurar las columnas de los grids
            base.CargarDatos();

            Cargando.Cerrar();
        }

        protected override void LlenarGrupos(int iId)
        {
            this.dgvGrupos.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de líneas
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.VendedorID == iId).GroupBy(
                g => new EnteroCadenaComp() { Entero = g.LineaID.Valor(), Cadena = g.Linea })).OrderByDescending(c => c.Actual);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvGrupos.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected override void LlenarSemanas(int iId)
        {
            this.dgvSemanas.Rows.Clear();

            // Se limpian los puntos de las series
            bool bGraficar = !this.chkGrafSemanas.Checked;
            if (bGraficar)
            {
                foreach (Series oSerie in this.chrSemanas.Series)
                    oSerie.Points.Clear();
            }
            
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica de Semanas
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.VendedorID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvSemanas.Rows.Add(oReg.Llave, oReg.Llave, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                // Para la gráfica
                if (bGraficar)
                {
                    this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Llave, oReg.Actual);
                    this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Llave, oReg.Anterior);
                }
            }
        }

        protected void LlenarGraficaSemanasTodos()
        {
            // Se limpian los puntos de las series
            foreach (Series oSerie in this.chrSemanas.Series)
                oSerie.Points.Clear();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se empieza a llenar el gráfico
            var oConsulta = this.AgruparPorEnteroCadenaEntero(oDatos.GroupBy(g => new AgrupadoPorEnteroCadenaEntero() {
                Llave = g.VendedorID, Cadena = g.Vendedor, Entero = UtilTheos.SemanaSabAVie(g.Fecha) }));
            foreach (var oReg in oConsulta)
            {
                string sSerie = oReg.Llave.ToString();
                // Si la serie no existe, se crea
                if (this.chrSemanas.Series.Any(c => c.Name == sSerie))
                {
                    this.chrSemanas.Series[sSerie].Points.AddXY(oReg.Entero, oReg.Actual);
                }
                else
                {
                    var oSerie = new Series(sSerie)
                    {
                        ChartType = SeriesChartType.Line,
                        ToolTip = oReg.Cadena,
                        Label = "o",
                        LabelForeColor = Color.Transparent,
                        LabelToolTip = "Semana: #VALX\nImporte: $#VAL",
                        CustomProperties = "LabelStyle=Center",
                    };
                    oSerie.Points.AddXY(oReg.Entero, oReg.Actual);
                    this.chrSemanas.Series.Add(oSerie);
                }
            }
        }

        protected override void LlenarMeses(int iId)
        {
            this.dgvMeses.Rows.Clear();
            this.chrMeses.Series["Actual"].Points.Clear();
            this.chrMeses.Series["Pasado"].Points.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica de Meses
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.VendedorID == iId).GroupBy(g => g.Fecha.Month));
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvMeses.Rows.Add(oReg.Llave, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.Llave).ToUpper(), oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                // Para la gráfica
                this.chrMeses.Series["Actual"].Points.AddXY(oReg.Llave, oReg.Actual);
                this.chrMeses.Series["Pasado"].Points.AddXY(oReg.Llave, oReg.Anterior);
            }
        }

        protected override void LlenarSucursales(int iId)
        {
            this.dgvSucursal.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de Sucursales
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.VendedorID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.SucursalID, Cadena = g.Sucursal }));
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvSucursal.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected override void LlenarPartes(int iGridFuente, int iIdPrincipal, int iId)
        {
            this.dgvPartes.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            var oConsulta = oDatos.Where(c => c.VendedorID == iIdPrincipal && c.Fecha >= this.dtpDesde.Value.Date);
            switch (iGridFuente)
            {
                case CuadroMultiple.GridFuente.Grupos:
                    this.LlenarDetalle(oConsulta, iId);
                    /* foreach (var oReg in oConsulta)
                    {
                        var oVentasDetV = General.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID && c.LineaID == iId);
                        foreach (var oDet in oVentasDetV)
                            this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte, 
                                ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
                    } */
                    return;
                case CuadroMultiple.GridFuente.Meses:
                case CuadroMultiple.GridFuente.Semanas:
                    DateTime dIni, dFin;
                    if (iGridFuente == CuadroMultiple.GridFuente.Semanas)
                    {
                        dIni = UtilTheos.InicioSemanaSabAVie(this.dtpDesde.Value.Year, iId);
                        dFin = dIni.AddDays(6).AddDays(1);
                    }
                    else
                    {
                        dIni = new DateTime(this.dtpDesde.Value.Year, iId, 1);
                        dFin = dIni.DiaUltimo().AddDays(1);
                    }

                    oConsulta = oConsulta.Where(c => (c.Fecha >= dIni && c.Fecha < dFin));
                    break;
                case CuadroMultiple.GridFuente.Sucursales:
                    oConsulta = oConsulta.Where(c => c.SucursalID == iId);
                    break;
            }

            // Se llena el grid
            this.LlenarDetalle(oConsulta, null);
            /* foreach (var oReg in oConsulta)
            {
                var oVentasDetV = General.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID);
                foreach (var oDet in oVentasDetV)
                    this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte, ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
            }*/ 
        }

        #endregion


    }
}
