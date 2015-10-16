using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ContaEstadoDeResultados : UserControl
    {
        public ContaEstadoDeResultados()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ContaEstadoDeResultados_Load(object sender, EventArgs e)
        {
            // Se inicializan los controles
            this.cmbAnio.Items.AddRange(new object[] { 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020 });
            this.cmbAnio.Text = DateTime.Now.Year.ToString();
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
        }

        private void nudDecimales_ValueChanged(object sender, EventArgs e)
        {
            if (this.nudDecimales.Focused)
                this.FormatoColumnas();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        private void LlenarColumnasAnio(int iAnio)
        {
            // Se borran las columnas de semanas
            for (int iCol = (this.dgvDatos.Columns.Count - 1); iCol > 0; iCol--)
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

            // this.dgvDatos.FirstDisplayedScrollingColumnIndex = (iColSem - 1);

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
            this.dgvDatos.Rows.Clear();

            // Se agrega la fila de ingresos
            int iFilaIngresos = this.dgvDatos.Rows.Add("+ Ingresos");
            this.dgvDatos.Rows[iFilaIngresos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los ingresos
            string sSucursal = "";
            int iFila = 0;
            foreach (var oReg in oSemanas)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal);
                }

                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFila].Value) + oReg.PrecioSinIva);
                this.dgvDatos[sSemana, iFilaIngresos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaIngresos].Value) + oReg.PrecioSinIva);
            }

            // Se agrega la fila de Costos
            int iFilaCostos = this.dgvDatos.Rows.Add("- Costos");
            this.dgvDatos.Rows[iFilaCostos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los costos
            sSucursal = "";
            foreach (var oReg in oSemanas)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal);
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
            var oGastosSem = oGastos.GroupBy(c => new { Semana = UtilLocal.InicioSemanaSabAVie(c.Fecha), c.Sucursal })
                .Select(c => new { c.Key.Semana, c.Key.Sucursal, Importe = c.Sum(s => s.ImporteDev) })
                .OrderBy(c => c.Sucursal).ThenBy(c => c.Semana);

            // Se agrega la fila de los Gastos
            int iFilaGastos = this.dgvDatos.Rows.Add("- Gastos");
            this.dgvDatos.Rows[iFilaGastos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los gastos
            sSucursal = "";
            foreach (var oReg in oGastosSem)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal);
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
                for (int i = 1; i < oSucursales.Count; i++)
                    this.dgvDatos[oCol.Index, iFilaUtilidad + i].Value = (
                        Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaIngresos + i].Value)
                        - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaCostos + i].Value)
                        - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos + i].Value)
                    );
            }

            // Se obtienen los datos de gastos especiales
            DateTime dHastaMas1 = dHasta.AddDays(1);
            var oDevEsp = General.GetListOf<ContaEgresosDevengadoEspecialCuentasView>(c => c.Fecha >= dDesde && c.Fecha < dHastaMas1)
                .GroupBy(c => new { c.Duenio, Semana = UtilLocal.InicioSemanaSabAVie(c.Fecha) })
                .Select(c => new { c.Key.Duenio, c.Key.Semana, Importe = c.Sum(s => s.ImporteDev) })
                .OrderBy(c => c.Duenio).ThenBy(c => c.Semana);

            // Se agrega la fila de Especiales
            int iFilaEsp = this.dgvDatos.Rows.Add("- Especiales");
            this.dgvDatos.Rows[iFilaEsp].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los gastos
            string sDuenio = "";
            foreach (var oReg in oDevEsp)
            {
                if (sDuenio != oReg.Duenio)
                {
                    sDuenio = oReg.Duenio;
                    iFila = this.dgvDatos.Rows.Add(sDuenio);
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

            Cargando.Cerrar();
        }

        #endregion

    }
}
