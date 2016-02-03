using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ContaEstadoDeResultados : UserControl
    {
        int iColumnasFijas;

        public ContaEstadoDeResultados()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ContaEstadoDeResultados_Load(object sender, EventArgs e)
        {
            // Se inicializan los controles
            this.cmbTipo.Items.AddRange(new object[] { "Semanal", "Mensual", "Trimestral", "Semestral" });
            this.cmbTipo.Text = "Semanal";
            this.cmbAnio.Items.AddRange(new object[] { 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020 });
            this.cmbAnio.Text = DateTime.Now.Year.ToString();
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));

            this.iColumnasFijas = this.dgvDatos.Columns.Count;
        }

        private void nudDecimales_ValueChanged(object sender, EventArgs e)
        {
            if (this.nudDecimales.Focused)
                this.FormatoColumnas();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            if (this.cmbTipo.Text == "Semanal")
                this.CargarDatos();
            else
                this.CargarDatosPorTipo();
        }

        #endregion

        #region [ Métodos ]

        private void LlenarColumnasAnio(int iAnio)
        {
            // Se borran las columnas de semanas
            for (int iCol = (this.dgvDatos.Columns.Count - 1); iCol >= this.iColumnasFijas; iCol--)
                this.dgvDatos.Columns.RemoveAt(iCol);

            // Se agregan las nuevas columnas
            DateTime dDia = new DateTime(iAnio, DateTime.Now.Month, DateTime.Now.Day);
            DateTime dIni = UtilLocal.InicioSemanaSabAVie(new DateTime(iAnio, 1, 1));
            int iColSem = 0;
            while (dIni.Year <= iAnio)
            {
                string sEnc = string.Format("{0}\n{1}", dIni.ToString("dd/MMM"), dIni.AddDays(6).ToString("dd/MMM"));
                var oCol = new DataGridViewTextBoxColumn() { Name = dIni.ToShortDateString(), HeaderText = sEnc };
                oCol.FormatoMoneda();
                this.dgvDatos.Columns.Add(oCol);

                if (dDia >= dIni && dDia < dIni.AddDays(7))
                    iColSem = this.dgvDatos.Columns.Count;

                dIni = dIni.AddDays(7);
            }

            this.dgvDatos.FirstDisplayedScrollingColumnIndex = (iColSem - 1);

            this.FormatoColumnas();
        }

        private void FormatoColumnas()
        {
            string sFormato = ("C" + this.nudDecimales.Value.ToString());
            for (int iCol = 1; iCol < this.dgvDatos.Columns.Count; iCol++)
                this.dgvDatos.Columns[iCol].DefaultCellStyle.Format = sFormato;
        }
                
        private void CargarDatos()
        {
            Cargando.Mostrar();

            var oFuenteT = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);

            // Se limpian los datos
            this.dgvDatos.Rows.Clear();
            foreach (var oSerie in this.chrPorSemana.Series)
                oSerie.Points.Clear();

            // Se llenan las columnas del año
            int iAnio = Helper.ConvertirEntero(this.cmbAnio.Text);
            this.LlenarColumnasAnio(iAnio);

            // Se obtienen los datos para los ingresos
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            DateTime dDesde = new DateTime(iAnio, 1, 1);
            DateTime dHasta = new DateTime(iAnio, 12, 31);
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", true);
            oParams.Add("Cobradas", false);
            oParams.Add("Solo9500", false);
            oParams.Add("OmitirDomingo", false);
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);

            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oSemanas = oDatos.Where(c => c.Fecha >= dDesde)
                .GroupBy(c => new { Semana = UtilLocal.InicioSemanaSabAVie(c.Fecha), c.Sucursal })
                .Select(c => new { c.Key.Semana, c.Key.Sucursal, PrecioSinIva = c.Sum(s => s.PrecioSinIvaActual), Costo = c.Sum(s => s.CostoDescActual) })
                .OrderBy(c => c.Sucursal).ThenBy(c => c.Semana);

            // Se agrega la fila de ingresos
            int iFilaIngresos = this.dgvDatos.Rows.Add("+ Ingresos", oSemanas.Sum(c => c.PrecioSinIva), oSemanas.Average(c => c.PrecioSinIva));
            this.dgvDatos.Rows[iFilaIngresos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los ingresos
            string sSucursal = "";
            int iFila = 0;
            foreach (var oReg in oSemanas)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal, oSemanas.Where(c=> c.Sucursal == sSucursal).Sum(c => c.PrecioSinIva)
                        , oSemanas.Where(c => c.Sucursal == sSucursal).Average(c => c.PrecioSinIva));
                }

                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFila].Value) + oReg.PrecioSinIva);
                this.dgvDatos[sSemana, iFilaIngresos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaIngresos].Value) + oReg.PrecioSinIva);
            }

            // Se agrega la fila de Costos
            int iFilaCostos = this.dgvDatos.Rows.Add("- Costos", oSemanas.Sum(c => c.Costo), oSemanas.Average(c => c.Costo));
            this.dgvDatos.Rows[iFilaCostos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los costos
            sSucursal = "";
            foreach (var oReg in oSemanas)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal, oSemanas.Where(c => c.Sucursal == sSucursal).Sum(c => c.Costo)
                        , oSemanas.Where(c => c.Sucursal == sSucursal).Average(c => c.Costo));
                }

                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Costo);
                this.dgvDatos[sSemana, iFilaCostos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaCostos].Value) + oReg.Costo);
            }

            // Se agrega la fila de margen bruto
            int iFilaMargen = this.dgvDatos.Rows.Add("= Margen Bruto");
            this.dgvDatos.Rows[iFilaMargen].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaMargen].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaIngresos].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaCostos].Value)
                );
            }

            // Se obtienen los datos para los gastos
            oParams.Clear();
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);
            var oGastos = General.ExecuteProcedure<pauContaCuentasPorSemana_Result>("pauContaCuentasPorSemana", oParams);
            var oGastosSemFijo = oGastos.GroupBy(c => new { Semana = UtilLocal.InicioSemanaSabAVie(c.Fecha), c.Sucursal })
                .Select(c => new ContaProc.GastoSem() { Semana = c.Key.Semana, Grupo = c.Key.Sucursal, Importe = c.Sum(s => s.ImporteDev).Valor() });
            // Se obtiene los datos según el tipo de semanalización
            List<ContaProc.GastoSem> oGastosSem;
            if (this.rdbSemanalizar.Checked)
            {
                oGastosSem = ContaProc.GastosSemanalizados(oGastos, Helper.ConvertirFechaHora(this.dgvDatos.Columns[this.dgvDatos.Columns.Count - 1].Name));
            }
            else
            {
                oGastosSem = oGastosSemFijo.OrderBy(c => c.Grupo).ThenBy(c => c.Semana).ToList();
            }

            // Se agrega la fila de los Gastos
            int iFilaGastos = this.dgvDatos.Rows.Add("- Gastos", oGastosSem.Sum(c => c.Importe), oGastosSem.Average(c => c.Importe));
            this.dgvDatos.Rows[iFilaGastos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los gastos
            sSucursal = "";
            foreach (var oReg in oGastosSem)
            {
                if (sSucursal != oReg.Grupo)
                {
                    sSucursal = oReg.Grupo;
                    iFila = this.dgvDatos.Rows.Add(sSucursal, oGastosSem.Where(c => c.Grupo == sSucursal).Sum(c => c.Importe)
                        , oGastosSem.Where(c => c.Grupo == sSucursal).Average(c => c.Importe));
                }

                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaGastos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaGastos].Value) + oReg.Importe);
            }

            // Se agrega la fila de utilidad, con cada una de las sucursales
            int iFilaUtilidad = this.dgvDatos.Rows.Add("= Utilidad");
            this.dgvDatos.Rows[iFilaUtilidad].DefaultCellStyle.Font = oFuenteT;
            // Sucursales
            var oSucursales = General.GetListOf<Sucursal>(c => c.Estatus).OrderBy(c => c.NombreSucursal).ToList();
            foreach (var oReg in oSucursales)
                this.dgvDatos.Rows.Add(oReg.NombreSucursal);
            // Se llenan los datos                        
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                // Utilidad total
                this.dgvDatos[oCol.Index, iFilaUtilidad].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaMargen].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos].Value)
                );
                // Sucursales
                for (int i = 1; i <= oSucursales.Count; i++)
                    this.dgvDatos[oCol.Index, iFilaUtilidad + i].Value = (
                        Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaIngresos + i].Value)
                        - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaCostos + i].Value)
                        - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos + i].Value)
                    );
            }

            // Se obtienen los datos de gastos especiales
            DateTime dHastaMas1 = dHasta.AddDays(1);
            // Se obtiene los datos según el tipo de semanalización
            List<ContaProc.GastoSem> oGastosSemEsp;
            var oGastosSemEspFijo = General.GetListOf<ContaEgresosDevengadoEspecialCuentasView>(c => c.Fecha >= dDesde && c.Fecha < dHastaMas1)
                .GroupBy(c => new { c.Duenio, Semana = UtilLocal.InicioSemanaSabAVie(c.Fecha) })
                .Select(c => new ContaProc.GastoSem() { Semana = c.Key.Semana, Grupo = c.Key.Duenio, Importe = c.Sum(s => s.ImporteDev) });
            if (this.rdbSemanalizar.Checked)
            {
                oGastosSemEsp = ContaProc.GastosSemanalizados(General.GetListOf<ContaEgresosDevengadoEspecialCuentasView>(c => c.Fecha >= dDesde && c.Fecha < dHastaMas1)
                    , Helper.ConvertirFechaHora(this.dgvDatos.Columns[this.dgvDatos.Columns.Count - 1].Name));
            }
            else
            {
                oGastosSemEsp = oGastosSemEspFijo.OrderBy(c => c.Grupo).ThenBy(c => c.Semana).ToList();
            }

            // Se agrega la fila de Especiales
            bool bHay = (oGastosSemEspFijo.Count() > 0);
            int iFilaEsp = this.dgvDatos.Rows.Add("- Especiales", (bHay ? oGastosSemEspFijo.Sum(c => c.Importe) : 0)
                , (bHay ? oGastosSemEspFijo.Average(c => c.Importe) : 0));
            this.dgvDatos.Rows[iFilaEsp].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los gastos
            string sDuenio = "";
            foreach (var oReg in oGastosSemEsp)
            {
                if (sDuenio != oReg.Grupo)
                {
                    sDuenio = oReg.Grupo;
                    iFila = this.dgvDatos.Rows.Add(sDuenio, oGastosSemEspFijo.Where(c => c.Grupo == sDuenio).Sum(c => c.Importe)
                        , oGastosSemEspFijo.Where(c => c.Grupo == sDuenio).Average(c => c.Importe));
                }

                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaEsp].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaEsp].Value) + oReg.Importe);
            }

            // Se agrega la fila de utilidad neta
            int iFilaDividendos = this.dgvDatos.Rows.Add("= Dividendos");
            this.dgvDatos.Rows[iFilaDividendos].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaDividendos].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaMargen].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaEsp].Value)
                );
            }

            // Se obtienen los datos para lo de reinversión
            var oReinversiones = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.FechaPoliza >= dDesde && c.FechaPoliza < dHastaMas1
                && (
                c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarLargoPlazo
                || (c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo && c.ContaCuentaAuxiliarID != Cat.ContaCuentasAuxiliares.TarjetaDeCredito)
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.AcreedoresDiversos
                || (c.ContaSubcuentaID == Cat.ContaSubcuentas.ActivoFijo && c.ContaCuentaDeMayorID != Cat.ContaCuentasDeMayor.Edificios)
                ));
            var oDeudas = oReinversiones.Where(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarLargoPlazo
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.AcreedoresDiversos)
                .GroupBy(c => UtilLocal.InicioSemanaSabAVie(c.FechaPoliza.Valor()))
                .Select(c => new { Semana = c.Key, Importe = c.Sum(s => s.Cargo) });
            var oInversiones = oReinversiones.Where(c => c.ContaSubcuentaID == Cat.ContaSubcuentas.ActivoFijo)
                .GroupBy(c => UtilLocal.InicioSemanaSabAVie(c.FechaPoliza.Valor())).Select(c => new { Semana = c.Key, Importe = c.Sum(s => s.Cargo) });
            // Se agrega la fila de Reinversión
            decimal mDeudas = (oDeudas.Count() > 0 ? oDeudas.Sum(c => c.Importe) : 0);
            decimal mDeudasProm = (oDeudas.Count() > 0 ? oDeudas.Average(c => c.Importe) : 0);
            decimal mInversiones = (oInversiones.Count() > 0 ? oInversiones.Sum(c => c.Importe) : 0);
            decimal mInversionesProm = (oInversiones.Count() > 0 ? oInversiones.Average(c => c.Importe) : 0);
            int iFilaReinversion = this.dgvDatos.Rows.Add("- Reinversión", (mDeudas + mInversiones), ((mDeudasProm + mInversionesProm) / 2));
            this.dgvDatos.Rows[iFilaReinversion].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los datos de reinversión
            // Deudas
            iFila = this.dgvDatos.Rows.Add("Deudas", mDeudas, mDeudasProm);
            foreach (var oReg in oDeudas)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaReinversion].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaReinversion].Value) + oReg.Importe);
            }
            // Inversión
            iFila = this.dgvDatos.Rows.Add("Inversiones", mInversiones, mInversionesProm);
            foreach (var oReg in oInversiones)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaReinversion].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaReinversion].Value) + oReg.Importe);
            }

            // Se agrega el saldo final
            int iFilaSaldo = this.dgvDatos.Rows.Add("= Saldo final");
            this.dgvDatos.Rows[iFilaSaldo].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaSaldo].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaDividendos].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaReinversion].Value)
                );
            }

            // Se llena la gráfica, en base al grid ya cargado
            for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
            {
                this.chrPorSemana.Series["Ingresos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaIngresos].Value));
                this.chrPorSemana.Series["Costos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaCostos].Value));
                this.chrPorSemana.Series["Margen"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaMargen].Value));
                this.chrPorSemana.Series["Gastos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaGastos].Value));
                this.chrPorSemana.Series["Utilidad"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaUtilidad].Value));
                this.chrPorSemana.Series["Especiales"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaEsp].Value));
                this.chrPorSemana.Series["Dividendos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaDividendos].Value));
            }

            Cargando.Cerrar();
        }

        class ConsultaEdr
        {
            public int Dia { get; set; }
            public int Mes { get; set; }
            public string Sucursal { get; set; }
            public decimal Importe { get; set; }
            public decimal PrecioSinIva { get; set; }
            public decimal Costo { get; set; }
        }
        private void CargarDatosPorTipo()
        {
            Cargando.Mostrar();

            var oFuenteT = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);

            // Se limpian los datos
            this.dgvDatos.Rows.Clear();
            foreach (var oSerie in this.chrPorSemana.Series)
                oSerie.Points.Clear();

            // Se borran las columnas de semanas
            for (int iCol = (this.dgvDatos.Columns.Count - 1); iCol >= this.iColumnasFijas; iCol--)
                this.dgvDatos.Columns.RemoveAt(iCol);
            // Se llenan las columnas
            string sPrefijo = "";
            switch (this.cmbTipo.Text)
            {
                case "Mensual":
                    sPrefijo = "Mes";
                    for (int iMes = 1; iMes <= 12; iMes++)
                        this.dgvDatos.Columns.Add((sPrefijo + iMes.ToString()), DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(iMes).PrimeraMayus());
                    break;
                case "Trimestral":
                    sPrefijo = "Tri";
                    for (int iMes = 1; iMes <= 12; iMes += 3)
                        this.dgvDatos.Columns.Add((sPrefijo + iMes.ToString()), DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(iMes).PrimeraMayus());
                    break;
                case "Semestral":
                    sPrefijo = "Sem";
                    for (int iMes = 1; iMes <= 12; iMes += 6)
                        this.dgvDatos.Columns.Add((sPrefijo + iMes.ToString()), DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(iMes).PrimeraMayus());
                    break;
            }
            this.FormatoColumnas();

            // Se obtienen los datos para los ingresos
            int iAnio = Helper.ConvertirEntero(this.cmbAnio.Text);
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            DateTime dDesde = new DateTime(iAnio, 1, 1);
            DateTime dHasta = new DateTime(iAnio, 12, 31);
            DateTime dHastaMas1 = dHasta.AddDays(1);
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", true);
            oParams.Add("Cobradas", false);
            oParams.Add("Solo9500", false);
            oParams.Add("OmitirDomingo", false);
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);

            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            oDatos = oDatos.Where(c => c.Fecha >= dDesde).ToList();
            
            List<ConsultaEdr> oConsulta = null;
            switch (this.cmbTipo.Text)
            {
                case "Mensual":
                    oConsulta = oDatos.GroupBy(c => new { Mes = c.Fecha.Month, c.Sucursal })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Sucursal
                            , PrecioSinIva = c.Sum(s => s.PrecioSinIvaActual).Valor(), Costo = c.Sum(s => s.CostoDescActual).Valor() })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
                case "Trimestral":
                    oConsulta = oDatos.GroupBy(c => new { Mes = UtilLocal.ObtenerTrimestre(c.Fecha), Sucursal = c.Sucursal })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Sucursal
                            , PrecioSinIva = c.Sum(s => s.PrecioSinIvaActual).Valor(), Costo = c.Sum(s => s.CostoDescActual).Valor() })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
                case "Semestral":
                    oConsulta = oDatos.GroupBy(c => new { Mes = UtilLocal.ObtenerSemestre(c.Fecha), Sucursal = c.Sucursal })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Sucursal
                            , PrecioSinIva = c.Sum(s => s.PrecioSinIvaActual).Valor(), Costo = c.Sum(s => s.CostoDescActual).Valor() })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
            }

            // Se agrega la fila de ingresos
            int iFilaIngresos = this.dgvDatos.Rows.Add("+ Ingresos", oConsulta.Sum(c => c.PrecioSinIva), oConsulta.Average(c => c.PrecioSinIva));
            this.dgvDatos.Rows[iFilaIngresos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los ingresos
            string sSucursal = "";
            int iFila = 0;
            foreach (var oReg in oConsulta)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal, oConsulta.Where(c => c.Sucursal == sSucursal).Sum(c => c.PrecioSinIva)
                        , oConsulta.Where(c => c.Sucursal == sSucursal).Average(c => c.PrecioSinIva));
                }

                string sColumna = (sPrefijo + oReg.Mes.ToString());
                this.dgvDatos[sColumna, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFila].Value) + oReg.PrecioSinIva);
                this.dgvDatos[sColumna, iFilaIngresos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFilaIngresos].Value) + oReg.PrecioSinIva);
            }

            // Se agrega la fila de Costos
            int iFilaCostos = this.dgvDatos.Rows.Add("- Costos", oConsulta.Sum(c => c.Costo), oConsulta.Average(c => c.Costo));
            this.dgvDatos.Rows[iFilaCostos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los costos
            sSucursal = "";
            foreach (var oReg in oConsulta)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal, oConsulta.Where(c => c.Sucursal == sSucursal).Sum(c => c.Costo)
                        , oConsulta.Where(c => c.Sucursal == sSucursal).Average(c => c.Costo));
                }

                string sColumna = (sPrefijo + oReg.Mes.ToString());
                this.dgvDatos[sColumna, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFila].Value) + oReg.Costo);
                this.dgvDatos[sColumna, iFilaCostos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFilaCostos].Value) + oReg.Costo);
            }

            // Se agrega la fila de margen bruto
            int iFilaMargen = this.dgvDatos.Rows.Add("= Margen Bruto");
            this.dgvDatos.Rows[iFilaMargen].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaMargen].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaIngresos].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaCostos].Value)
                );
            }

            // Se obtienen los datos para los gastos
            oParams.Clear();
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);
            var oGastos = General.GetListOf<ContaEgresosView>(c => c.Fecha >= dDesde && c.Fecha < dHastaMas1);

            oConsulta = null;
            switch (this.cmbTipo.Text)
            {
                case "Mensual":
                    oConsulta = oGastos.GroupBy(c => new { Mes = c.Fecha.Month, c.Sucursal })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Sucursal, Importe = c.Sum(s => s.ImporteDev).Valor() })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
                case "Trimestral":
                    oConsulta = oGastos.GroupBy(c => new { Mes = UtilLocal.ObtenerTrimestre(c.Fecha), c.Sucursal })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Sucursal, Importe = c.Sum(s => s.ImporteDev).Valor() })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
                case "Semestral":
                    oConsulta = oGastos.GroupBy(c => new { Mes = UtilLocal.ObtenerSemestre(c.Fecha), c.Sucursal })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Sucursal, Importe = c.Sum(s => s.ImporteDev).Valor() })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
            }

            // Se agrega la fila de los Gastos
            int iFilaGastos = this.dgvDatos.Rows.Add("- Gastos", oConsulta.Sum(c => c.Importe), oConsulta.Average(c => c.Importe));
            this.dgvDatos.Rows[iFilaGastos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los gastos
            sSucursal = "";
            foreach (var oReg in oConsulta)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal, oConsulta.Where(c => c.Sucursal == sSucursal).Sum(c => c.Importe)
                        , oConsulta.Where(c => c.Sucursal == sSucursal).Average(c => c.Importe));
                }

                string sColumna = (sPrefijo + oReg.Mes.ToString());
                this.dgvDatos[sColumna, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFila].Value) + oReg.Importe);
                this.dgvDatos[sColumna, iFilaGastos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFilaGastos].Value) + oReg.Importe);
            }

            // Se agrega la fila de utilidad, con cada una de las sucursales
            int iFilaUtilidad = this.dgvDatos.Rows.Add("= Utilidad");
            this.dgvDatos.Rows[iFilaUtilidad].DefaultCellStyle.Font = oFuenteT;
            // Sucursales
            var oSucursales = General.GetListOf<Sucursal>(c => c.Estatus).OrderBy(c => c.NombreSucursal).ToList();
            foreach (var oReg in oSucursales)
                this.dgvDatos.Rows.Add(oReg.NombreSucursal);
            // Se llenan los datos                        
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                // Utilidad total
                this.dgvDatos[oCol.Index, iFilaUtilidad].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaMargen].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos].Value)
                );
                // Sucursales
                for (int i = 1; i <= oSucursales.Count; i++)
                    this.dgvDatos[oCol.Index, iFilaUtilidad + i].Value = (
                        Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaIngresos + i].Value)
                        - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaCostos + i].Value)
                        - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos + i].Value)
                    );
            }

            // Se obtienen los datos de gastos especiales
            var oGastosEsp = General.GetListOf<ContaEgresosDevengadoEspecialCuentasView>(c => c.Fecha >= dDesde && c.Fecha < dHastaMas1);

            oConsulta = null;
            switch (this.cmbTipo.Text)
            {
                case "Mensual":
                    oConsulta = oGastosEsp.GroupBy(c => new { Mes = c.Fecha.Month, c.Duenio })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Duenio, Importe = c.Sum(s => s.ImporteDev) })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
                case "Trimestral":
                    oConsulta = oGastosEsp.GroupBy(c => new { Mes = UtilLocal.ObtenerTrimestre(c.Fecha), c.Duenio })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Duenio, Importe = c.Sum(s => s.ImporteDev) })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
                case "Semestral":
                    oConsulta = oGastosEsp.GroupBy(c => new { Mes = UtilLocal.ObtenerSemestre(c.Fecha), c.Duenio })
                        .Select(c => new ConsultaEdr() { Mes = c.Key.Mes, Sucursal = c.Key.Duenio, Importe = c.Sum(s => s.ImporteDev) })
                        .OrderBy(c => c.Sucursal).ThenBy(c => c.Mes).ToList();
                    break;
            }

            // Se agrega la fila de Especiales
            bool bHay = (oConsulta.Count() > 0);
            int iFilaEsp = this.dgvDatos.Rows.Add("- Especiales", (bHay ? oConsulta.Sum(c => c.Importe) : 0), (bHay ? oConsulta.Average(c => c.Importe) : 0));
            this.dgvDatos.Rows[iFilaEsp].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los gastos
            string sDuenio = "";
            foreach (var oReg in oConsulta)
            {
                if (sDuenio != oReg.Sucursal)
                {
                    sDuenio = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sDuenio, oConsulta.Where(c => c.Sucursal == sDuenio).Sum(c => c.Importe)
                        , oConsulta.Where(c => c.Sucursal == sDuenio).Average(c => c.Importe));
                }

                string sColumna = (sPrefijo + oReg.Mes.ToString());
                this.dgvDatos[sColumna, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFila].Value) + oReg.Importe);
                this.dgvDatos[sColumna, iFilaEsp].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFilaEsp].Value) + oReg.Importe);
            }

            // Se agrega la fila de utilidad neta
            int iFilaDividendos = this.dgvDatos.Rows.Add("= Dividendos");
            this.dgvDatos.Rows[iFilaDividendos].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaDividendos].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaMargen].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaEsp].Value)
                );
            }

            // Se obtienen los datos para lo de reinversión
            var oReinversiones = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.FechaPoliza >= dDesde && c.FechaPoliza < dHastaMas1
                && (
                c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarLargoPlazo
                || (c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo && c.ContaCuentaAuxiliarID != Cat.ContaCuentasAuxiliares.TarjetaDeCredito)
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.AcreedoresDiversos
                || (c.ContaSubcuentaID == Cat.ContaSubcuentas.ActivoFijo && c.ContaCuentaDeMayorID != Cat.ContaCuentasDeMayor.Edificios)
                ));
            var oDeudas = oReinversiones.Where(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarLargoPlazo
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.AcreedoresDiversos);
            var oInversiones = oReinversiones.Where(c => c.ContaSubcuentaID == Cat.ContaSubcuentas.ActivoFijo);

            List<ConsultaEdr> oDeudasGr = null;
            List<ConsultaEdr> oInversionesGr = null;
            switch (this.cmbTipo.Text)
            {
                case "Mensual":
                    oDeudasGr = oDeudas.GroupBy(c => c.FechaPoliza.Valor().Month).Select(c => new ConsultaEdr() { Mes = c.Key, Importe = c.Sum(s => s.Cargo) }).ToList();
                    oInversionesGr = oInversiones.GroupBy(c => c.FechaPoliza.Valor().Month).Select(c => new ConsultaEdr() { Mes = c.Key, Importe = c.Sum(s => s.Cargo) }).ToList();
                    break;
                case "Trimestral":
                    oDeudasGr = oDeudas.GroupBy(c => UtilLocal.ObtenerTrimestre(c.FechaPoliza.Valor()))
                        .Select(c => new ConsultaEdr() { Mes = c.Key, Importe = c.Sum(s => s.Cargo) }).ToList();
                    oInversionesGr = oInversiones.GroupBy(c => UtilLocal.ObtenerTrimestre(c.FechaPoliza.Valor()))
                        .Select(c => new ConsultaEdr() { Mes = c.Key, Importe = c.Sum(s => s.Cargo) }).ToList();
                    break;
                case "Semestral":
                    oDeudasGr = oDeudas.GroupBy(c => UtilLocal.ObtenerSemestre(c.FechaPoliza.Valor()))
                        .Select(c => new ConsultaEdr() { Mes = c.Key, Importe = c.Sum(s => s.Cargo) }).ToList();
                    oInversionesGr = oInversiones.GroupBy(c => UtilLocal.ObtenerSemestre(c.FechaPoliza.Valor()))
                        .Select(c => new ConsultaEdr() { Mes = c.Key, Importe = c.Sum(s => s.Cargo) }).ToList();
                    break;
            }

            // Se agrega la fila de Reinversión
            decimal mDeudas = (oDeudasGr.Count() > 0 ? oDeudasGr.Sum(c => c.Importe) : 0);
            decimal mDeudasProm = (oDeudasGr.Count() > 0 ? oDeudasGr.Average(c => c.Importe) : 0);
            decimal mInversiones = (oInversionesGr.Count() > 0 ? oInversionesGr.Sum(c => c.Importe) : 0);
            decimal mInversionesProm = (oInversionesGr.Count() > 0 ? oInversionesGr.Average(c => c.Importe) : 0);
            int iFilaReinversion = this.dgvDatos.Rows.Add("- Reinversión", (mDeudas + mInversiones), ((mDeudasProm + mInversionesProm) / 2));
            this.dgvDatos.Rows[iFilaReinversion].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los datos de reinversión
            // Deudas
            iFila = this.dgvDatos.Rows.Add("Deudas", mDeudas, mDeudasProm);
            foreach (var oReg in oDeudasGr)
            {
                string sColumna = (sPrefijo + oReg.Mes.ToString());
                this.dgvDatos[sColumna, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFila].Value) + oReg.Importe);
                this.dgvDatos[sColumna, iFilaReinversion].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFilaReinversion].Value) + oReg.Importe);
            }
            // Inversión
            iFila = this.dgvDatos.Rows.Add("Inversiones", mInversiones, mInversionesProm);
            foreach (var oReg in oInversionesGr)
            {
                string sColumna = (sPrefijo + oReg.Mes.ToString());
                this.dgvDatos[sColumna, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFila].Value) + oReg.Importe);
                this.dgvDatos[sColumna, iFilaReinversion].Value = (Helper.ConvertirDecimal(this.dgvDatos[sColumna, iFilaReinversion].Value) + oReg.Importe);
            }

            // Se agrega el saldo final
            int iFilaSaldo = this.dgvDatos.Rows.Add("= Saldo final");
            this.dgvDatos.Rows[iFilaSaldo].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaSaldo].Value = (
                    Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaDividendos].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaReinversion].Value)
                );
            }

            // Se llena la gráfica, en base al grid ya cargado
            for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
            {
                this.chrPorSemana.Series["Ingresos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaIngresos].Value));
                this.chrPorSemana.Series["Costos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaCostos].Value));
                this.chrPorSemana.Series["Margen"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaMargen].Value));
                this.chrPorSemana.Series["Gastos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaGastos].Value));
                this.chrPorSemana.Series["Utilidad"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaUtilidad].Value));
                this.chrPorSemana.Series["Especiales"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaEsp].Value));
                this.chrPorSemana.Series["Dividendos"].Points.AddY(Helper.ConvertirDecimal(this.dgvDatos[iCol, iFilaDividendos].Value));
            }

            Cargando.Cerrar();
        }

        #endregion

    }
}
