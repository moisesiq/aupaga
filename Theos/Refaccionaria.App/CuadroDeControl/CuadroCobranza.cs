using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.ComponentModel;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroCobranza : UserControl
    {
        public CuadroCobranza()
        {
            InitializeComponent();
        }

        #region [ Eventos ]
        private void CuadroCobranza_Load(object sender, EventArgs e)
        {
            // Se llenan los combos
            this.cmbCalculo.Items.AddRange(new object[] { "Utilidad", "Utilidad Desc.", "Precio", "Costo", "Costo Desc.", "Ventas", "Productos" });
            this.cmbCalculo.SelectedIndex = 1;
            var oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            oSucursales.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "Todas" });
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", oSucursales);
            this.cmbSucursal.SelectedValue = 0;
            this.chkPagadas.Checked = true;
            this.chkCobradas.Checked = true;

            // Se llenan las fechas
            this.dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);
            this.dtpHasta.Value = this.dtpDesde.Value.AddYears(1).AddDays(-1);
        }

        private void nudDecimales_ValueChanged(object sender, EventArgs e)
        {
            if (this.nudDecimales.Focused)
                this.AplicarFormatoColumnas();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        private void dgvClientes_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvClientes.CurrentRow != null && this.dgvClientes.VerSeleccionNueva())
            {
                int iClienteID = Util.Entero(this.dgvClientes.CurrentRow.Cells["cli_ClienteID"].Value);
                this.CargarDatosCliente(iClienteID);
            }
        }

        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            // Se obtienen los datos base
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", this.chkPagadas.Checked);
            oParams.Add("Cobradas", this.chkCobradas.Checked);
            oParams.Add("Solo9500", this.chk9500.Checked);
            oParams.Add("OmitirDomingo", this.chkOmitirDomingos.Checked);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            var oClientes = Datos.ExecuteProcedure<pauCuadroDeControlClientesCredito_Result>("pauCuadroDeControlClientesCredito", oParams);
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCobranza_Result>("pauCuadroDeControlCobranza", oParams);
            
            // Se carga el grid de Clientes
            this.dgvClientes.Rows.Clear();
            var oTotalesCli = this.ObtenerTotales(oClientes);
            foreach (var oReg in oClientes)
            {
                decimal mAdeudo = this.ObtenerValor(oReg, true);
                decimal mVencido = this.ObtenerValor(oReg, false);
                this.dgvClientes.Rows.Add(oReg.ClienteID, oReg.Cliente, oReg.DiasDeCredito, mAdeudo, Util.DividirONull(mAdeudo, oTotalesCli.Adeudo)
                    , mVencido, Util.DividirONull(mVencido, oTotalesCli.Vencido), oReg.PromedioDePagoAnual, oReg.PromedioDePago3Meses);
            }
            // Se orden el grid
            this.dgvClientes.Sort(this.dgvClientes.Columns["cli_Vencido"], ListSortDirection.Descending);

            // Se llenan los totales
            this.dgvClientesT.Rows.Clear();
            this.dgvClientesT.Rows.Add("TOTALES", oTotalesCli.Adeudo, null, oTotalesCli.Vencido);

            // Se llena el grid de por semana
            this.CargarPorSemana(oDatos);

            // Se llena el grid de por mes
            this.CargarPorMes(oDatos);
            
            // Se configuran columnas del grid
            this.AplicarFormatoColumnas();
            
            Cargando.Cerrar();
        }

        private void CargarPorSemana(List<pauCuadroDeControlCobranza_Result> oDatos)
        {
            var oPorSemana = this.AgruparPorEntero(oDatos.GroupBy(g => UtilTheos.SemanaSabAVie(g.Vencimiento.Valor())));
            this.dgvPorSemana.Rows.Clear();
            this.chrPorSemana.Series["Expectativa"].Points.Clear();
            this.chrPorSemana.Series["Cobrado"].Points.Clear();
            foreach (var oReg in oPorSemana)
            {
                this.dgvPorSemana.Rows.Add(oReg.Llave, oReg.Esperado, oReg.Pagado
                    , Util.DividirONull(oReg.Pagado, oReg.Esperado), Util.DividirONull(oReg.Pagado, oReg.PagadoAnt));
                this.chrPorSemana.Series["Expectativa"].Points.AddXY(oReg.Llave, oReg.Esperado);
                this.chrPorSemana.Series["Cobrado"].Points.AddXY(oReg.Llave, oReg.Pagado);
            }
            // this.dgvPorSemanaT["SemanaT_Actual", 0].Value = (oPorSemana.Count() > 0 ? oPorSemana.Average(c => c.Actual) : 0);
            // this.dgvPorSemanaT["SemanaT_Anterior", 0].Value = (oPorSemana.Count() > 0 ? oPorSemana.Average(c => c.Anterior) : 0);
        }

        private void CargarPorMes(List<pauCuadroDeControlCobranza_Result> oDatos)
        {
            var oPorMes = this.AgruparPorEntero(oDatos.GroupBy(g => g.Vencimiento.Valor().Month));
            this.dgvPorMes.Rows.Clear();
            this.chrPorMes.Series["Expectativa"].Points.Clear();
            this.chrPorMes.Series["Cobrado"].Points.Clear();
            foreach (var oReg in oPorMes)
            {
                this.dgvPorMes.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.Llave).ToUpper(), oReg.Esperado, oReg.Pagado
                    , Util.DividirONull(oReg.Pagado, oReg.Esperado), Util.DividirONull(oReg.Pagado, oReg.PagadoAnt));
                this.chrPorMes.Series["Expectativa"].Points.AddXY(oReg.Llave, oReg.Esperado);
                this.chrPorMes.Series["Cobrado"].Points.AddXY(oReg.Llave, oReg.Pagado);
            }
        }

        private void CargarDatosCliente(int iClienteID)
        {
            Cargando.Mostrar();

            // Se obtienen los datos base
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("ClienteID", iClienteID);
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", this.chkPagadas.Checked);
            oParams.Add("Cobradas", this.chkCobradas.Checked);
            oParams.Add("Solo9500", this.chk9500.Checked);
            oParams.Add("OmitirDomingo", this.chkOmitirDomingos.Checked);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCobranza_Result>("pauCuadroDeControlCobranza", oParams);

            // Se llenan los grids
            this.CargarPorSemana(oDatos);
            this.CargarPorMes(oDatos);
            this.AplicarFormatoColumnas();

            Cargando.Cerrar();
        }

        private void AplicarFormatoColumnas()
        {
            string sFormato = ((this.cmbCalculo.Text == "Ventas" || this.cmbCalculo.Text == "Productos") ? "N" : "C");
            sFormato += Util.Cadena((int)this.nudDecimales.Value);
            this.dgvClientes.Columns["cli_Adeudo"].DefaultCellStyle.Format = sFormato;
            this.dgvClientes.Columns["cli_Vencido"].DefaultCellStyle.Format = sFormato;
            this.dgvPorSemana.Columns["sem_Expectativa"].DefaultCellStyle.Format = sFormato;
            this.dgvPorSemana.Columns["sem_Cobrado"].DefaultCellStyle.Format = sFormato;
            // this.dgvPorSemanaT.Columns["SemanaT_Actual"].DefaultCellStyle.Format = sFormato;
            // this.dgvPorSemanaT.Columns["SemanaT_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvPorMes.Columns["mes_Expectativa"].DefaultCellStyle.Format = sFormato;
            this.dgvPorMes.Columns["mes_Cobrado"].DefaultCellStyle.Format = sFormato;            
        }

        private decimal ObtenerValor(pauCuadroDeControlClientesCredito_Result oReg, bool bAdeudo)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad": return (bAdeudo ? oReg.AdeudoUtilidad : oReg.VencidoUtilidad).Valor();
                case "Utilidad Desc.": return (bAdeudo ? oReg.AdeudoUtilidadConDescuento : oReg.VencidoUtilidadConDescuento).Valor();
                case "Precio": return (bAdeudo ? oReg.Adeudo : oReg.Vencido).Valor();
                case "Costo": return (bAdeudo ? oReg.AdeudoCosto : oReg.VencidoCosto).Valor();
                case "Costo Desc.": return (bAdeudo ? oReg.AdeudoCostoConDescuento : oReg.VencidoCostoConDescuento).Valor();
                case "Ventas": return (bAdeudo ? oReg.AdeudoVentas : oReg.VencidoVentas).Valor();
                case "Productos": return (bAdeudo ? oReg.AdeudoProductos : oReg.VencidoProductos).Valor();
            }

            return 0;
        }

        class Totales
        {
            public decimal Adeudo { get; set; }
            public decimal Vencido { get; set; }
        }
        private Totales ObtenerTotales(List<pauCuadroDeControlClientesCredito_Result> oDatos)
        {
            if (oDatos.Count <= 0)
                return new Totales();

            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return new Totales()
                    {
                        Adeudo = oDatos.Sum(c => c.AdeudoUtilidad).Valor(),
                        Vencido = oDatos.Sum(c => c.VencidoUtilidad).Valor()
                    };
                case "Utilidad Desc.":
                    return new Totales()
                    {
                        Adeudo = oDatos.Sum(c => c.AdeudoUtilidadConDescuento).Valor(),
                        Vencido = oDatos.Sum(c => c.VencidoUtilidadConDescuento).Valor()
                    };
                case "Precio":
                    return new Totales()
                    {
                        Adeudo = oDatos.Sum(c => c.Adeudo).Valor(),
                        Vencido = oDatos.Sum(c => c.Vencido).Valor()
                    };
                case "Costo":
                    return new Totales()
                    {
                        Adeudo = oDatos.Sum(c => c.AdeudoCosto).Valor(),
                        Vencido = oDatos.Sum(c => c.VencidoCosto).Valor()
                    };
                case "Costo Desc.":
                    return new Totales()
                    {
                        Adeudo = oDatos.Sum(c => c.AdeudoCostoConDescuento).Valor(),
                        Vencido = oDatos.Sum(c => c.VencidoCostoConDescuento).Valor()
                    };
                case "Ventas":
                    return new Totales()
                    {
                        Adeudo = oDatos.Sum(c => c.AdeudoVentas).Valor(),
                        Vencido = oDatos.Sum(c => c.VencidoVentas).Valor()
                    };
                case "Productos":
                    return new Totales()
                    {
                        Adeudo = oDatos.Sum(c => c.AdeudoProductos).Valor(),
                        Vencido = oDatos.Sum(c => c.VencidoProductos).Valor()
                    };
            }

            return null;
        }

        class TotalesPorEntero
        {
            public int Llave { get; set; }
            public decimal Esperado { get; set; }
            public decimal Pagado { get; set; }
            public decimal PagadoAnt { get; set; }
        }
        private IEnumerable<TotalesPorEntero> AgruparPorEntero(IEnumerable<IGrouping<int, pauCuadroDeControlCobranza_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Esperado = c.Sum(s => s.EsperadoUtilidad).Valor(),
                        Pagado = c.Sum(s => s.EsperadoUtilidad * s.Porcentaje).Valor(),
                        PagadoAnt = c.Sum(s => s.EsperadoUtilidadAnt * s.PorcentajeAnt).Valor()
                    }).OrderBy(o => o.Llave);
                case "Utilidad Desc.":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Esperado = c.Sum(s => s.EsperadoUtilidadConDescuento).Valor(),
                        Pagado = c.Sum(s => s.EsperadoUtilidadConDescuento * s.Porcentaje).Valor(),
                        PagadoAnt = c.Sum(s => s.EsperadoUtilidadConDescuentoAnt * s.PorcentajeAnt).Valor()
                    }).OrderBy(o => o.Llave);
                case "Precio":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Esperado = c.Sum(s => s.Esperado).Valor(),
                        Pagado = c.Sum(s => s.Pagado).Valor(),
                        PagadoAnt = c.Sum(s => s.PagadoAnt).Valor(),
                    }).OrderBy(o => o.Llave);
                case "Costo":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Esperado = c.Sum(s => s.EsperadoCosto).Valor(),
                        Pagado = c.Sum(s => s.EsperadoCosto * s.Porcentaje).Valor(),
                        PagadoAnt = c.Sum(s => s.EsperadoCostoAnt * s.PorcentajeAnt).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo Desc.":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Esperado = c.Sum(s => s.EsperadoCostoConDescuento).Valor(),
                        Pagado = c.Sum(s => s.EsperadoCostoConDescuento * s.Porcentaje).Valor(),
                        PagadoAnt = c.Sum(s => s.EsperadoCostoConDescuentoAnt * s.PorcentajeAnt).Valor()
                    }).OrderBy(o => o.Llave);
                case "Ventas":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Esperado = c.Where(s => s.EsActual == true).Select(s => s.VentaID).Distinct().Count(),
                        Pagado = c.Where(s => s.EsActual == true && s.VentaEstatusID == Cat.VentasEstatus.Completada).Select(s => s.VentaID).Distinct().Count(),
                        PagadoAnt = c.Where(s => s.EsActual != true && s.VentaEstatusID == Cat.VentasEstatus.Completada).Select(s => s.VentaID).Distinct().Count()
                    }).OrderBy(o => o.Llave);
                case "Productos":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Esperado = c.Sum(s => s.Productos).Valor(),
                        Pagado = c.Sum(s => s.Productos * s.Porcentaje).Valor(),
                        PagadoAnt = c.Sum(s => s.ProductosAnt * s.PorcentajeAnt).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }


        #endregion

    }
}
