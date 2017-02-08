using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

using TheosProc;
using LibUtil;
using System.Collections;

namespace Refaccionaria.App
{
    public partial class CuadroVentas : UserControl
    {
        public CuadroVentas()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        //private ArrayList ValidarPermisosCalculo()
        //{

        //    List<UsuariosPermisosView> list = Datos.GetListOf<UsuariosPermisosView>(i => i.UsuarioID == Theos.UsuarioID );
        //    ArrayList per = new ArrayList();

        //    foreach (var x in list)
        //    {
        //        switch (x.Permiso)
        //        {
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Utilidad":
        //                per.Add("Utilidad");
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.UtilidadDesc":
        //                per.Add("Utilidad Desc.");
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Precio":
        //                per.Add("Precio");
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Costo":
        //                per.Add("Costo");
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.CostoDesc":
        //                per.Add("Costo Desc.");
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Ventas":
        //                per.Add("Ventas");
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Productos":
        //                per.Add("Productos");
        //                break;
        //        }

        //    }

        //    return per;
        //}

        //private List<Sucursal> ValidarPermisosTienda()
        //{
        //    List<Sucursal> oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
        //    List<UsuariosPermisosView> listaPermisos = Datos.GetListOf<UsuariosPermisosView>(i => i.UsuarioID == Theos.UsuarioID);
        //    List<Sucursal> listaSucursales = new List<Sucursal>();

        //    foreach (var x in listaPermisos)
        //    {
        //        switch (x.Permiso)
        //        {
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Sucursal1":
        //                listaSucursales.Add(oSucursales.ElementAt(0));
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Sucursal2":
        //                listaSucursales.Add(oSucursales.ElementAt(1));
        //                break;
        //            case "CuadroDeControl.Pestania.Fecha.Ver.Sucursal3":
        //                listaSucursales.Add(oSucursales.ElementAt(2));
        //                break;
        //        }
        //    }
        //    return listaSucursales;
        //}


        private void CuadroVentas_Load(object sender, EventArgs e)
        {
            //CuadroControlPermisos PermisosC = new CuadroControlPermisos();


            // Se llenan los combos
            // this.cmbCalculo.Items.AddRange(new object[] { "Utilidad", "Utilidad Desc.", "Precio", "Costo", "Costo Desc.", "Ventas", "Productos" });
            //this.cmbCalculo.Items.AddRange(CuadroControlPermisos.ValidarPermisosCalculo().ToArray());
            this.cmbCalculo.Items.AddRange(CuadroControlPermisos.ValidarPermisosCalculoCuadroMultiple(CuadroControlPermisos.GetTabPage).ToArray());
            this.cmbCalculo.SelectedIndex = 0;
            //var oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            //List<Sucursal> oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);

            //var oSucursales = CuadroControlPermisos.ValidarPermisosTienda();
            var oSucursales = CuadroControlPermisos.ValidarPermisosTiendaCuadroMultiple(CuadroControlPermisos.GetTabPage);

            if (oSucursales.Count() > 2)
            {
                oSucursales.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "Todas" });
            }

            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", oSucursales);
            this.cmbSucursal.SelectedValue = oSucursales.ElementAt(0).SucursalID;
            //this.cmbSucursal.SelectedValue = 0;
            this.chkPagadas.Checked = true;
            // this.chkCostoConDescuento.Checked = true;
            
            // Se llenan las fechas
            this.dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);

            // Se agrega la línea de totales a los grids correspondientes
            this.dgvPorDiaT.Rows.Add("Promedio");
            this.dgvPorSemanaT.Rows.Add("Promedio");

            // Se obiene el año actual y anterior, para mostrar etiquetas
            int iAnio = DateTime.Now.Year;
            int iAnioAnt = (iAnio - 1);
            foreach (Control oControl in this.Controls)
            {
                if (oControl is DataGridView && oControl != this.dgvPorAnio)
                {
                    (oControl as DataGridView).Columns[1].HeaderText = iAnio.ToString();
                    (oControl as DataGridView).Columns[2].HeaderText = iAnioAnt.ToString();
                }
            }
            this.lblAnioActual.Text = iAnio.ToString();
            this.lblAnioAnterior.Text = iAnioAnt.ToString();

            // Se manda cargar los datos
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
            if (this.nudDecimales.Focused)
                this.AplicarFormatoColumnas();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", this.chkPagadas.Checked);
            oParams.Add("Cobradas", this.chkCobradas.Checked);
            oParams.Add("Solo9500", this.chk9500.Checked);
            oParams.Add("OmitirDomingo", this.chkOmitirDomingos.Checked);
            // oParams.Add("CostoConDescuento", this.chkCostoConDescuento.Checked);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid de por día
            var oPorDia = this.AgruparPorDia(oDatos);
            this.dgvPorDia.Rows.Clear();
            foreach (var oReg in oPorDia)
            {
                this.dgvPorDia.Rows.Add(oReg.Llave, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, oTotales.Actual) * 100));
            }
            this.dgvPorDiaT["DiaT_Actual", 0].Value = (oPorDia.Count() > 0 ? oPorDia.Average(c => c.Actual) : 0);
            this.dgvPorDiaT["DiaT_Anterior", 0].Value = (oPorDia.Count() > 0 ? oPorDia.Average(c => c.Anterior) : 0);

            // Se llena el grid de por semana
            var oPorSemana = this.AgruparPorEntero(oDatos.GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            this.dgvPorSemana.Rows.Clear();
            this.chrPorSemana.Series["Actual"].Points.Clear();
            this.chrPorSemana.Series["Pasado"].Points.Clear();
            foreach (var oReg in oPorSemana)
            {
                this.dgvPorSemana.Rows.Add(oReg.Llave, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, oTotales.Actual) * 100));
                this.chrPorSemana.Series["Actual"].Points.AddXY(oReg.Llave, oReg.Actual);
                this.chrPorSemana.Series["Pasado"].Points.AddXY(oReg.Llave, oReg.Anterior);
            }
            this.dgvPorSemanaT["SemanaT_Actual", 0].Value = (oPorSemana.Count() > 0 ? oPorSemana.Average(c => c.Actual) : 0);
            this.dgvPorSemanaT["SemanaT_Anterior", 0].Value = (oPorSemana.Count() > 0 ? oPorSemana.Average(c => c.Anterior) : 0);

            // Se llena el grid de por mes
            var oPorMes = this.AgruparPorEntero(oDatos.GroupBy(g => g.Fecha.Month));
            this.dgvPorMes.Rows.Clear();
            this.chrPorMes.Series["Actual"].Points.Clear();
            this.chrPorMes.Series["Pasado"].Points.Clear();
            foreach (var oReg in oPorMes)
            {
                this.dgvPorMes.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.Llave).ToUpper(), oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, oTotales.Actual) * 100));
                this.chrPorMes.Series["Actual"].Points.AddXY(oReg.Llave, oReg.Actual);
                this.chrPorMes.Series["Pasado"].Points.AddXY(oReg.Llave, oReg.Anterior);
            }

            // Se llena el grid de días
            oParams["Desde"] = this.dtpDesde.Value;
            oParams["Hasta"] = this.dtpHasta.Value;
            oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oPorDiaSem = oDatos.GroupBy(g => g.Fecha.DayOfWeek).Select(c => new { Dia = c.Key, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) })
                .OrderBy(o => (int)o.Dia);
            decimal mTotalDiaSem = (oDatos.Count > 0 ? oDatos.Sum(c => c.Actual).Valor() : 0);
            this.dgvDias.Rows.Clear();
            this.chrPorDiaSem.Series["Actual"].Points.Clear();
            foreach (var oReg in oPorDiaSem)
            {
                this.dgvDias.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(oReg.Dia).ToUpper(), oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotalDiaSem) * 100));
                int iPunto = this.chrPorDiaSem.Series["Actual"].Points.AddXY((int)oReg.Dia, oReg.Actual);
                this.chrPorDiaSem.Series["Actual"].Points[iPunto].AxisLabel = CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(oReg.Dia).ToUpper();
            }

            // Se llena el grid de horas
            var oPorHora = this.AgruparPorEntero(oDatos.GroupBy(g => g.Fecha.Hour));
            this.dgvHoras.Rows.Clear();
            this.chrPorHora.Series.Clear();
            foreach (var oReg in oPorHora)
            {
                this.dgvHoras.Rows.Add(string.Format("{0:00}:00", oReg.Llave), oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotalDiaSem) * 100));
                this.AgregarSerieCilindro(this.chrPorHora, oReg.Llave, oReg.Actual);
            }

            // Se llena el grid de sucursales
            var oPorSucursal = this.AgruparPorEntero(oDatos.GroupBy(g => g.SucursalID));
            var oSucursales = oDatos.Select(c => new { c.SucursalID, c.Sucursal }).Distinct();
            this.dgvSucursales.Rows.Clear();
            foreach (var oReg in oPorSucursal)
            {
                string sSucursal = oSucursales.FirstOrDefault(c => c.SucursalID == oReg.Llave).Sucursal;
                this.dgvSucursales.Rows.Add(sSucursal, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotalDiaSem) * 100));
            }

            // Se llena el grid de por año
            oParams.Remove("Desde");
            oParams.Remove("Hasta");
            var oPorAnio = Datos.ExecuteProcedure<pauCuadroDeControlPorAnio_Result>("pauCuadroDeControlPorAnio", oParams);
            this.dgvPorAnio.Rows.Clear();
            foreach (var oReg in oPorAnio)
            {
                decimal mDato = this.ObtenerDatoAnio(oReg);
                this.dgvPorAnio.Rows.Add(oReg.Anio, mDato);
                this.chrPorAnio.Series["Actual"].Points.AddXY(oReg.Anio, mDato);
            }

            // Se configuran columnas del grid
            this.AplicarFormatoColumnas();

            // Se llenan los totales
            this.txtAnioActual.Text = oTotales.Actual.ToString(GlobalClass.FormatoMoneda);
            this.txtAnioAnterior.Text = oTotales.Anterior.ToString(GlobalClass.FormatoMoneda);
            this.txtResultado.Text = Util.DividirONull(oTotales.Actual, oTotales.Anterior).Valor().ToString(GlobalClass.FormatoDecimal);

            Cargando.Cerrar();
        }

        private void AplicarFormatoColumnas()
        {
            string sFormato = ((this.cmbCalculo.Text == "Ventas" || this.cmbCalculo.Text == "Productos") ? "N" : "C");
            sFormato += Util.Cadena((int)this.nudDecimales.Value);
            this.dgvPorDia.Columns["Dia_AnioActual"].DefaultCellStyle.Format = sFormato;
            this.dgvPorDia.Columns["Dia_AnioAnterior"].DefaultCellStyle.Format = sFormato;
            this.dgvPorDiaT.Columns["DiaT_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvPorDiaT.Columns["DiaT_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvPorSemana.Columns["Semana_AnioActual"].DefaultCellStyle.Format = sFormato;
            this.dgvPorSemana.Columns["Semana_AnioAnterior"].DefaultCellStyle.Format = sFormato;
            this.dgvPorSemanaT.Columns["SemanaT_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvPorSemanaT.Columns["SemanaT_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvPorMes.Columns["Mes_AnioActual"].DefaultCellStyle.Format = sFormato;
            this.dgvPorMes.Columns["Mes_AnioAnterior"].DefaultCellStyle.Format = sFormato;
            this.dgvDias.Columns["Dias_AnioActual"].DefaultCellStyle.Format = sFormato;
            this.dgvDias.Columns["Dias_AnioAnterior"].DefaultCellStyle.Format = sFormato;
            this.dgvHoras.Columns["Horas_AnioActual"].DefaultCellStyle.Format = sFormato;
            this.dgvHoras.Columns["Horas_AnioAnterior"].DefaultCellStyle.Format = sFormato;
            this.dgvSucursales.Columns["Sucursal_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSucursales.Columns["Sucursal_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvPorAnio.Columns["Anio_Actual"].DefaultCellStyle.Format = sFormato;
        }

        private decimal ObtenerDatoAnio(pauCuadroDeControlPorAnio_Result oReg)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad": return oReg.Utilidad.Valor();
                case "Utilidad Desc.": return oReg.UtilidadConDescuento.Valor();
                case "Precio": return oReg.Precio.Valor();
                case "Costo": return oReg.Costo.Valor();
                case "Costo Desc.": return oReg.CostoConDescuento.Valor();
                case "Ventas": return oReg.Ventas.Valor();
                case "Productos": return oReg.Productos.Valor();
            }

            return 0;
        }

        class TotalesPorFecha {
            public DateTime Llave { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }
        private IEnumerable<TotalesPorFecha> AgruparPorDia(List<pauCuadroDeControlGeneral_Result> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            DateTime dDiaCero = new DateTime(DateTime.Now.Year, 1, 1).AddDays(-1);
            var oPorDia = oDatos.GroupBy(g => g.Fecha.Date.DayOfYear);
            switch (sCalculo)
            {
                case "Utilidad":
                    return oPorDia.Select(c => new TotalesPorFecha() {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.Actual).Valor(),
                        Anterior = c.Sum(s => s.Anterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Utilidad Desc.":
                    return oPorDia.Select(c => new TotalesPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.UtilDescActual).Valor(),
                        Anterior = c.Sum(s => s.UtilDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Precio":
                    return oPorDia.Select(c => new TotalesPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.PrecioActual).Valor(),
                        Anterior = c.Sum(s => s.PrecioAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo":
                    return oPorDia.Select(c => new TotalesPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.CostoActual).Valor(),
                        Anterior = c.Sum(s => s.CostoAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo Desc.":
                    return oPorDia.Select(c => new TotalesPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.CostoDescActual).Valor(),
                        Anterior = c.Sum(s => s.CostoDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Ventas":
                    return oPorDia.Select(c => new TotalesPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Where(s => s.EsActual == true).Select(s => s.VentaID).Distinct().Count(),
                        Anterior = c.Where(s => s.EsActual != true).Select(s => s.VentaID).Distinct().Count()
                    }).OrderBy(o => o.Llave);
                case "Productos":
                    return oPorDia.Select(c => new TotalesPorFecha()
                    {
                        Llave = dDiaCero.AddDays(c.Key),
                        Actual = c.Sum(s => s.ProductosActual).Valor(),
                        Anterior = c.Sum(s => s.ProductosAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }

        class TotalesPorEntero {
            public int Llave { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }
        private IEnumerable<TotalesPorEntero> AgruparPorEntero(IEnumerable<IGrouping<int, pauCuadroDeControlGeneral_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new TotalesPorEntero() {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.Actual).Valor(),
                        Anterior = c.Sum(s => s.Anterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Utilidad Desc.":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.UtilDescActual).Valor(),
                        Anterior = c.Sum(s => s.UtilDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Precio":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.PrecioActual).Valor(),
                        Anterior = c.Sum(s => s.PrecioAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.CostoActual).Valor(),
                        Anterior = c.Sum(s => s.CostoAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo Desc.":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.CostoDescActual).Valor(),
                        Anterior = c.Sum(s => s.CostoDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Ventas":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Where(s => s.EsActual == true).Select(s => s.VentaID).Distinct().Count(),
                        Anterior = c.Where(s => s.EsActual != true).Select(s => s.VentaID).Distinct().Count()
                    }).OrderBy(o => o.Llave);
                case "Productos":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
                        Llave = c.Key,
                        Actual = c.Sum(s => s.ProductosActual).Valor(),
                        Anterior = c.Sum(s => s.ProductosAnterior).Valor()
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
                        Actual = oDatos.Sum(c => c.Actual).Valor(),
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
                        Actual = oDatos.Where(c => c.EsActual == true).Select(c => c.VentaID).Distinct().Count(),
                        Anterior = oDatos.Where(c => c.EsActual != true).Select(c => c.VentaID).Distinct().Count()
                    };
                case "Productos":
                    return new Totales()
                    {
                        Actual = oDatos.Sum(c => c.ProductosActual).Valor(),
                        Anterior = oDatos.Sum(c => c.ProductosAnterior).Valor()
                    };
            }

            return null;
        }

        private void AgregarSerieCilindro(Chart oGrafica, int? iHora, decimal? mValor)
        {
            var oSerie = new Series()
            {
                BackGradientStyle = GradientStyle.VerticalCenter,
                BackSecondaryColor = Color.Transparent,
                BorderColor = Color.White,
                ChartType = SeriesChartType.StackedColumn100,
                Label = "#AXISLABEL",
                LabelForeColor = Color.White,
                CustomProperties = "PixelPointWidth=80"
            };
            oSerie.SmartLabelStyle.Enabled = false;
            int iPunto = oSerie.Points.AddY(mValor);
            oSerie.Points[iPunto].AxisLabel = iHora.ToString();

            oGrafica.Series.Add(oSerie);
        }

        #endregion

    }
}
