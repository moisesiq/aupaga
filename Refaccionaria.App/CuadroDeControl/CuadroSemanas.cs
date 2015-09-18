using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CuadroSemanas : UserControl
    {
        public CuadroSemanas()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroSemanas_Load(object sender, EventArgs e)
        {
            // Se llenan los tipos de cálculo
            this.cmbCalculo.Items.AddRange(new object[] { "Utilidad", "Utilidad Desc.", "Precio", "Costo", "Costo Desc.", "Ventas" });
            this.cmbCalculo.SelectedIndex = 1;
            // Se llenan las Sucursales
            var oSucursales = General.GetListOf<Sucursal>(c => c.Estatus);
            oSucursales.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "Todas" });
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", oSucursales);
            this.cmbSucursal.SelectedValue = 0;
            this.chkPagadas.Checked = true;
            this.chkCostoConDescuento.Checked = true;
            // Se llenan las fechas
            this.dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);
            // Se agrega la fila de totales del grid principal
            this.dgvSemanaT.Rows.Add("Totales");
            this.dgvVendedorT.Rows.Add("Totales");

            // Se 
            // this.CargarDatos();
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (this.cmbSucursal.Focused)
            //     this.CargarDatos();
        }

        private void chkPagadas_CheckedChanged(object sender, EventArgs e)
        {
            // if (this.chkPagadas.Focused)
            //     this.CargarDatos();
        }

        private void chkCobradas_CheckedChanged(object sender, EventArgs e)
        {
            // if (this.chkCobradas.Focused)
            //     this.CargarDatos();
        }

        private void chk9500_CheckedChanged(object sender, EventArgs e)
        {
            // if (this.chk9500.Focused)
            //     this.CargarDatos();
        }

        private void chkOmitirDomingos_CheckedChanged(object sender, EventArgs e)
        {
            // if (this.chkOmitirDomingos.Focused)
            //     this.CargarDatos();
        }

        private void chkCostoConDescuento_CheckedChanged(object sender, EventArgs e)
        {
            // if (this.chkCostoConDescuento.Focused)
            //     this.CargarDatos();
        }

        private void dtpDesde_ValueChanged(object sender, EventArgs e)
        {
            // if (this.dtpDesde.Focused)
            //     this.CargarDatos();
        }

        private void dtpHasta_ValueChanged(object sender, EventArgs e)
        {
            // if (this.dtpHasta.Focused)
            //     this.CargarDatos();
        }

        private void nudDecimales_ValueChanged(object sender, EventArgs e)
        {
            string sFormato = (this.cmbCalculo.Text == "Ventas" ? "N" : "C");
            sFormato += Helper.ConvertirCadena((int)this.nudDecimales.Value);
            this.dgvSemana.Columns["Semana_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSemana.Columns["Semana_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanaT.Columns["SemanaT_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanaT.Columns["SemanaT_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvVendedor.Columns["Vendedor_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvVendedor.Columns["Vendedor_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvVendedorT.Columns["VendedorT_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvVendedorT.Columns["VendedorT_Anterior"].DefaultCellStyle.Format = sFormato;
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        private void dgvSemana_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvSemana.Focused && this.dgvSemana.VerSeleccionNueva())
            {
                int iSemana = (this.dgvSemana.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvSemana.CurrentRow.Cells["Semana_Id"].Value));
                this.LlenarVendedores(iSemana);
                this.LlenarGraficaSemanas(null);
            }
        }

        private void dgvVendedorSem_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvVendedorSem.Focused && this.dgvVendedorSem.VerSeleccionNueva())
            {
                int iVendedorID = (this.dgvVendedorSem.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvVendedorSem.CurrentRow.Cells["VendedorSem_Id"].Value));
                this.LlenarGraficaSemanas(iVendedorID);
            }
        }

        #endregion

        #region [ Métodos ]

        private Dictionary<string, object> ObtenerParametros()
        {
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", this.chkPagadas.Checked);
            oParams.Add("Cobradas", this.chkCobradas.Checked);
            oParams.Add("Solo9500", this.chk9500.Checked);
            oParams.Add("OmitirDomingo", this.chkOmitirDomingos.Checked);
            // oParams.Add("CostoConDescuento", this.chkCostoConDescuento.Checked);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            return oParams;
        }

        private void CargarDatos()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid de por semana
            var oPorSemana = this.AgruparPorEntero(oDatos.GroupBy(g => UtilLocal.SemanaSabAVie(g.Fecha)));
            this.dgvSemana.Rows.Clear();
            // this.chrSemana.Series["Actual"].Points.Clear();
            // this.chrSemana.Series["Pasado"].Points.Clear();
            foreach (var oReg in oPorSemana)
            {
                this.dgvSemana.Rows.Add(oReg.Llave, oReg.Llave, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, oTotales.Actual) * 100));
                // this.chrSemana.Series["Actual"].Points.AddXY(oReg.Semana, oReg.Actual);
                // this.chrSemana.Series["Pasado"].Points.AddXY(oReg.Semana, oReg.Anterior);
            }
            this.dgvSemanaT["SemanaT_Actual", 0].Value = oTotales.Actual;
            this.dgvSemanaT["SemanaT_Anterior", 0].Value = oTotales.Anterior;
            this.dgvSemanaT["SemanaT_Resultado", 0].Value = Helper.DividirONull(oTotales.Actual, oTotales.Anterior);

            // Se llena el grid de Vendedores con semanas
            int iFila;
            Dictionary<string, decimal> oSemanasT = new Dictionary<string, decimal>();
            var oPorVendedor = this.AgruparPorEnteroCadenaEntero(oDatos.GroupBy(g => new AgrupadoPorEnteroCadenaEntero() {
                Llave = g.VendedorID, Cadena = g.Vendedor, Entero = UtilLocal.SemanaSabAVie(g.Fecha) })).OrderBy(c => c.Entero);
            this.dgvVendedorSem.Rows.Clear();
            foreach (var oReg in oPorVendedor)
            {
                // Se encuentra la fila del vendedor, o se agrega si no existe
                iFila = this.dgvVendedorSem.EncontrarIndiceDeValor("VendedorSem_Id", oReg.Llave);
                if (iFila < 0)
                {
                    var oVendedor = oPorVendedor.Where(c => c.Llave == oReg.Llave);
                    iFila = this.dgvVendedorSem.Rows.Add(oReg.Llave, oReg.Cadena
                        , oVendedor.Average(c => c.Actual), oVendedor.Sum(c => c.Actual));
                }

                // Se verifica si existe la columna de la semana, si no, se agrega
                string sColSem = ("VendedorSem_" + oReg.Entero.ToString());
                if (!this.dgvVendedorSem.Columns.Contains(sColSem))
                {
                    this.dgvVendedorSem.Columns.Add(sColSem, ("Sem " + oReg.Entero.ToString()));
                    this.dgvVendedorSem.Columns[sColSem].FormatoMoneda();
                }
                if (!oSemanasT.ContainsKey(sColSem))
                    oSemanasT.Add(sColSem, 0);

                // Se llenan los datos
                this.dgvVendedorSem[sColSem, iFila].Value = oReg.Actual;
                oSemanasT[sColSem] += oReg.Actual;

                // Para el formato de la columna
                this.dgvVendedorSem.Columns[sColSem].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            }
            // Se agrega la fila de total
            // this.dgvVendedorSem.Rows.Add();
            iFila = this.dgvVendedorSem.Rows.Add(0, "Totales", oPorVendedor.Average(c => c.Actual), oPorVendedor.Sum(c => c.Actual));
            this.dgvVendedorSem.Rows[iFila].DefaultCellStyle.Font = new Font(this.dgvVendedorSem.Font, FontStyle.Bold);
            foreach (DataGridViewColumn oCol in this.dgvVendedorSem.Columns)
            {
                if (oCol.Index < 4) continue;
                if (!oSemanasT.ContainsKey(oCol.Name)) continue;
                this.dgvVendedorSem[oCol.Name, iFila].Value = oSemanasT[oCol.Name];
            }

            // Se configuran columnas del grid
            this.dgvSemana.Columns["Semana_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvSemana.Columns["Semana_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvSemanaT.Columns["SemanaT_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvSemanaT.Columns["SemanaT_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvVendedor.Columns["Vendedor_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvVendedor.Columns["Vendedor_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvVendedorSem.Columns["VendedorSem_Promedio"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N2" : "C2");
            this.dgvVendedorSem.Columns["VendedorSem_Total"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");

            Cargando.Cerrar();
        }

        private void LlenarVendedores(int iSemana)
        {
            this.dgvVendedor.Rows.Clear();
            if (iSemana <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de Vendedor
            var oConsulta = oDatos.Where(c => UtilLocal.SemanaSabAVie(c.Fecha) == iSemana).GroupBy(g => new { g.VendedorID, g.Vendedor }).Select(
                c => new { c.Key.VendedorID, c.Key.Vendedor, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) }).OrderByDescending(c => c.Actual);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual).Valor() : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvVendedor.Rows.Add(oReg.VendedorID, oReg.Vendedor, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
            }
            
            // Se llenan los totales
            decimal mTotalAnt = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Anterior).Valor() : 0);
            this.dgvVendedorT["VendedorT_Actual", 0].Value = mTotal;
            this.dgvVendedorT["VendedorT_Anterior", 0].Value = mTotalAnt;
            this.dgvVendedorT["VendedorT_Resultado", 0].Value = Helper.DividirONull(mTotal, mTotalAnt);
        }

        private void LlenarGraficaSemanas(int? iVendedorID)
        {
            this.chrSemana.Series["Actual"].Points.Clear();
            this.chrSemana.Series["Anterior"].Points.Clear();
            if (iVendedorID <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena la gráfica
            IEnumerable<pauCuadroDeControlGeneral_Result> oFiltro = oDatos;
            if (iVendedorID.HasValue)
                oFiltro = oDatos.Where(c => c.VendedorID == iVendedorID);
            // 
            var oConsulta = oFiltro.GroupBy(g => new { Semana = UtilLocal.SemanaSabAVie(g.Fecha) }).Select(
                c => new { c.Key.Semana, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) }).OrderBy(c => c.Semana);
            foreach (var oReg in oConsulta)
            {
                this.chrSemana.Series["Actual"].Points.AddXY(oReg.Semana, oReg.Actual);
                this.chrSemana.Series["Anterior"].Points.AddXY(oReg.Semana, oReg.Anterior);
            }
        }

        class AgrupadoPorFecha
        {
            public DateTime Llave { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }
        private IEnumerable<AgrupadoPorFecha> AgruparPorDia(List<pauCuadroDeControlGeneral_Result> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            DateTime dDiaCero = new DateTime(DateTime.Now.Year, 1, 1).AddDays(-1);
            var oPorDia = oDatos.GroupBy(g => g.Fecha.Date.DayOfYear);
            switch (sCalculo)
            {
                case "Utilidad":
                    return oPorDia.Select(c => new AgrupadoPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.Actual).Valor(),
                        Anterior = c.Sum(s => s.Anterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Utilidad Desc.":
                    return oPorDia.Select(c => new AgrupadoPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.UtilDescActual).Valor(),
                        Anterior = c.Sum(s => s.UtilDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Precio":
                    return oPorDia.Select(c => new AgrupadoPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.PrecioActual).Valor(),
                        Anterior = c.Sum(s => s.PrecioAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo":
                    return oPorDia.Select(c => new AgrupadoPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.CostoActual).Valor(),
                        Anterior = c.Sum(s => s.CostoAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo Desc.":
                    return oPorDia.Select(c => new AgrupadoPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.CostoDescActual).Valor(),
                        Anterior = c.Sum(s => s.CostoDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Ventas":
                    return oPorDia.Select(c => new AgrupadoPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.VentasActual).Valor(),
                        Anterior = c.Sum(s => s.VentasAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }

        class AgrupadoPorEntero
        {
            public int Llave { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }
        private IEnumerable<AgrupadoPorEntero> AgruparPorEntero(IEnumerable<IGrouping<int, pauCuadroDeControlGeneral_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new AgrupadoPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.Actual).Valor(),
                        Anterior = c.Sum(s => s.Anterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Utilidad Desc.":
                    return oDatos.Select(c => new AgrupadoPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.UtilDescActual).Valor(),
                        Anterior = c.Sum(s => s.UtilDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Precio":
                    return oDatos.Select(c => new AgrupadoPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.PrecioActual).Valor(),
                        Anterior = c.Sum(s => s.PrecioAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo":
                    return oDatos.Select(c => new AgrupadoPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.CostoActual).Valor(),
                        Anterior = c.Sum(s => s.CostoAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo Desc.":
                    return oDatos.Select(c => new AgrupadoPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.CostoDescActual).Valor(),
                        Anterior = c.Sum(s => s.CostoDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Ventas":
                    return oDatos.Select(c => new AgrupadoPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.VentasActual).Valor(),
                        Anterior = c.Sum(s => s.VentasAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }

        class AgrupadoPorEnteroCadenaEntero : IEquatable<AgrupadoPorEnteroCadenaEntero>
        {
            public int Llave { get; set; }
            public string Cadena { get; set; }
            public int Entero { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }

            public bool Equals(AgrupadoPorEnteroCadenaEntero other)
            {
                if (Object.ReferenceEquals(other, null)) return false;
                if (Object.ReferenceEquals(this, other)) return true;
                return (this.Llave.Equals(other.Llave) && this.Entero.Equals(other.Entero) && this.Cadena.Equals(other.Cadena));
            }

            public override int GetHashCode()
            {
                int hashLlave = (this.Llave == null ? 0 : this.Llave.GetHashCode());
                int hashEntero = (this.Entero == null ? 0 : this.Entero.GetHashCode());
                int hashCadena = this.Cadena.GetHashCode();
                return (hashLlave ^ hashEntero ^ hashCadena);
            }
        }
        private IEnumerable<AgrupadoPorEnteroCadenaEntero> AgruparPorEnteroCadenaEntero(
            IEnumerable<IGrouping<AgrupadoPorEnteroCadenaEntero, pauCuadroDeControlGeneral_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new AgrupadoPorEnteroCadenaEntero()
                    {
                        Llave = c.Key.Llave,
                        Cadena = c.Key.Cadena,
                        Entero = c.Key.Entero,
                        Actual = c.Sum(s => s.Actual).Valor(),
                        Anterior = c.Sum(s => s.Anterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Utilidad Desc.":
                    return oDatos.Select(c => new AgrupadoPorEnteroCadenaEntero()
                    {
                        Llave = c.Key.Llave,
                        Cadena = c.Key.Cadena,
                        Entero = c.Key.Entero,
                        Actual = c.Sum(s => s.UtilDescActual).Valor(),
                        Anterior = c.Sum(s => s.UtilDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Precio":
                    return oDatos.Select(c => new AgrupadoPorEnteroCadenaEntero()
                    {
                        Llave = c.Key.Llave,
                        Cadena = c.Key.Cadena,
                        Entero = c.Key.Entero,
                        Actual = c.Sum(s => s.PrecioActual).Valor(),
                        Anterior = c.Sum(s => s.PrecioAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo":
                    return oDatos.Select(c => new AgrupadoPorEnteroCadenaEntero()
                    {
                        Llave = c.Key.Llave,
                        Cadena = c.Key.Cadena,
                        Entero = c.Key.Entero,
                        Actual = c.Sum(s => s.CostoActual).Valor(),
                        Anterior = c.Sum(s => s.CostoAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo Desc.":
                    return oDatos.Select(c => new AgrupadoPorEnteroCadenaEntero()
                    {
                        Llave = c.Key.Llave,
                        Cadena = c.Key.Cadena,
                        Entero = c.Key.Entero,
                        Actual = c.Sum(s => s.CostoDescActual).Valor(),
                        Anterior = c.Sum(s => s.CostoDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Ventas":
                    return oDatos.Select(c => new AgrupadoPorEnteroCadenaEntero()
                    {
                        Llave = c.Key.Llave,
                        Cadena = c.Key.Cadena,
                        Entero = c.Key.Entero,
                        Actual = c.Sum(s => s.VentasActual).Valor(),
                        Anterior = c.Sum(s => s.VentasAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }

        class Totales
        {
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }
        private Totales ObtenerTotales(List<pauCuadroDeControlGeneral_Result> oDatos)
        {
            if (oDatos.Count <= 0)
                return new Totales();

            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return new Totales()
                    {
                        Actual = oDatos.Sum(c => c.Actual).Valor()
                        ,
                        Anterior = oDatos.Sum(c => c.Anterior).Valor()
                    };
                case "Utilidad Desc.":
                    return new Totales()
                    {
                        Actual = oDatos.Sum(c => c.UtilDescActual).Valor(),
                        Anterior = oDatos.Sum(c => c.UtilDescAnterior).Valor()
                    };
                case "Precio":
                    return new Totales()
                    {
                        Actual = oDatos.Sum(c => c.PrecioActual).Valor(),
                        Anterior = oDatos.Sum(c => c.PrecioAnterior).Valor()
                    };
                case "Costo":
                    return new Totales()
                    {
                        Actual = oDatos.Sum(c => c.CostoActual).Valor(),
                        Anterior = oDatos.Sum(c => c.CostoAnterior).Valor()
                    };
                case "Costo Desc.":
                    return new Totales()
                    {
                        Actual = oDatos.Sum(c => c.CostoDescActual).Valor(),
                        Anterior = oDatos.Sum(c => c.CostoDescAnterior).Valor()
                    };
                case "Ventas":
                    return new Totales()
                    {
                        Actual = oDatos.Sum(c => c.VentasActual).Valor(),
                        Anterior = oDatos.Sum(c => c.VentasAnterior).Valor()
                    };
            }

            return null;
        }

        #endregion
                                                       
    }
}
