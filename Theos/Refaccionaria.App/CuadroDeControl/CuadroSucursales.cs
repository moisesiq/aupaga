using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroSucursales : UserControl
    {
        public CuadroSucursales()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroSucursales_Load(object sender, EventArgs e)
        {
            //CuadroControlPermisos PermisosC = new CuadroControlPermisos();
            // Se llenan los combos
            //this.cmbCalculo.Items.AddRange(new object[] { "Utilidad", "Utilidad Desc.", "Precio", "Costo", "Costo Desc.", "Ventas", "Productos" });
            this.cmbCalculo.Items.AddRange(CuadroControlPermisos.ValidarPermisosCalculoCuadroMultiple(CuadroControlPermisos.GetTabPage).ToArray());
            //this.cmbCalculo.SelectedIndex = 1;
            this.cmbCalculo.SelectedIndex = 0;
            this.chkPagadas.Checked = true;
            // this.chkCostoConDescuento.Checked = true;

            // Se llenan las fechas
            this.dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);

            // Se configuran eventos para pestaña de Proveedores
            this.ctlPorProveedor.dgvPrincipal.CurrentCellChanged += this.ctlPorProveedor_dgvPrincipal_CurrentCellChanged;
            this.ctlPorMarca.dgvPrincipal.CurrentCellChanged += this.ctlPorMarca_dgvPrincipal_CurrentCellChanged;
            this.ctlPorLinea.dgvPrincipal.CurrentCellChanged += this.ctlPorLinea_dgvPrincipal_CurrentCellChanged;
        }

        private void nudDecimales_ValueChanged(object sender, EventArgs e)
        {
            if (this.nudDecimales.Focused)
                this.FormatoColumnas();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            switch (this.tabSucursales.SelectedTab.Name)
            {
                case "tbpPorFecha":
                    this.CargarPorFecha();
                    break;
                case "tbpPorDiaYHora":
                    this.CargarPorDiaSemYHora();
                    break;
                case "tbpPorProveedor":
                    this.CargarPorProveedor();
                    break;
                case "tbpPorMarca":
                    this.CargarPorMarca();
                    break;
                case "tbpPorLinea":
                    this.CargarPorLinea();
                    break;
            }
            
        }

        #endregion

        #region [ Métodos ]

        private Dictionary<string, object> ObtenerParametros()
        {
            var oParams = new Dictionary<string, object>();
            oParams.Add("Pagadas", this.chkPagadas.Checked);
            oParams.Add("Cobradas", this.chkCobradas.Checked);
            oParams.Add("Solo9500", this.chk9500.Checked);
            oParams.Add("OmitirDomingo", this.chkOmitirDomingos.Checked);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);
            return oParams;
        }
        
        class AgrupadoPorSucursal
        {
            public int LlaveEntero { get; set; }
            public DateTime LlaveFecha { get; set; }
            public string Cadena { get; set; }
            public decimal Suc01_Actual { get; set; }
            public decimal Suc01_Anterior { get; set; }
            public decimal Suc02_Actual { get; set; }
            public decimal Suc02_Anterior { get; set; }
            public decimal Suc03_Actual { get; set; }
            public decimal Suc03_Anterior { get; set; }
        }
        private AgrupadoPorSucursal ObtenerTotales(List<pauCuadroDeControlGeneral_Result> oDatos)
        {
            if (oDatos.Count <= 0)
                return new AgrupadoPorSucursal();

            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return new AgrupadoPorSucursal()
                    {
                        Suc01_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Actual : null)).Valor(),
                        Suc01_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Anterior : null)).Valor(),
                        Suc02_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Actual : null)).Valor(),
                        Suc02_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Anterior : null)).Valor(),
                        Suc03_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Actual : null)).Valor(),
                        Suc03_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Anterior : null)).Valor(),
                    };
                case "Utilidad Desc.":
                    return new AgrupadoPorSucursal()
                    {
                        Suc01_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescActual : null)).Valor(),
                        Suc01_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescAnterior : null)).Valor(),
                        Suc02_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescActual : null)).Valor(),
                        Suc02_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescAnterior : null)).Valor(),
                        Suc03_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescActual : null)).Valor(),
                        Suc03_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescAnterior : null)).Valor(),
                    };
                case "Precio":
                    return new AgrupadoPorSucursal()
                    {
                        Suc01_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioActual : null)).Valor(),
                        Suc01_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioAnterior : null)).Valor(),
                        Suc02_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioActual : null)).Valor(),
                        Suc02_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioAnterior : null)).Valor(),
                        Suc03_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioActual : null)).Valor(),
                        Suc03_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioAnterior : null)).Valor(),
                    };
                case "Costo":
                    return new AgrupadoPorSucursal()
                    {
                        Suc01_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoActual : null)).Valor(),
                        Suc01_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoAnterior : null)).Valor(),
                        Suc02_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoActual : null)).Valor(),
                        Suc02_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoAnterior : null)).Valor(),
                        Suc03_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoActual : null)).Valor(),
                        Suc03_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoAnterior : null)).Valor(),
                    };
                case "Costo Desc.":
                    return new AgrupadoPorSucursal()
                    {
                        Suc01_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescActual : null)).Valor(),
                        Suc01_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescAnterior : null)).Valor(),
                        Suc02_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescActual : null)).Valor(),
                        Suc02_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescAnterior : null)).Valor(),
                        Suc03_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescActual : null)).Valor(),
                        Suc03_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescAnterior : null)).Valor(),
                    };
                case "Ventas":
                    return new AgrupadoPorSucursal()
                    {
                        Suc01_Actual = oDatos.Where(c => c.EsActual == true && c.SucursalID == Cat.Sucursales.Matriz).Select(c => c.VentaID).Distinct().Count(),
                        Suc01_Anterior = oDatos.Where(c => c.EsActual != true && c.SucursalID == Cat.Sucursales.Matriz).Select(c => c.VentaID).Distinct().Count(),
                        Suc02_Actual = oDatos.Where(c => c.EsActual == true && c.SucursalID == Cat.Sucursales.Sucursal2).Select(c => c.VentaID).Distinct().Count(),
                        Suc02_Anterior = oDatos.Where(c => c.EsActual != true && c.SucursalID == Cat.Sucursales.Sucursal2).Select(c => c.VentaID).Distinct().Count(),
                        Suc03_Actual = oDatos.Where(c => c.EsActual == true && c.SucursalID == Cat.Sucursales.Sucursal3).Select(c => c.VentaID).Distinct().Count(),
                        Suc03_Anterior = oDatos.Where(c => c.EsActual != true && c.SucursalID == Cat.Sucursales.Sucursal3).Select(c => c.VentaID).Distinct().Count()
                    };
                case "Productos":
                    return new AgrupadoPorSucursal()
                    {
                        Suc01_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosActual : null)).Valor(),
                        Suc01_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosAnterior : null)).Valor(),
                        Suc02_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosActual : null)).Valor(),
                        Suc02_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosAnterior : null)).Valor(),
                        Suc03_Actual = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosActual : null)).Valor(),
                        Suc03_Anterior = oDatos.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosAnterior : null)).Valor(),
                    };
            }

            return null;
        }

        private IEnumerable<AgrupadoPorSucursal> AgruparPorDia(List<pauCuadroDeControlGeneral_Result> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            DateTime dDiaCero = new DateTime(DateTime.Now.Year, 1, 1).AddDays(-1);
            var oPorDia = oDatos.GroupBy(g => new { g.Fecha.Date.DayOfYear });
            switch (sCalculo)
            {
                case "Utilidad":
                    return oPorDia.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveFecha = dDiaCero.AddDays(c.Key.DayOfYear),
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Actual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Anterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Actual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Anterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Actual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Anterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveFecha);
                case "Utilidad Desc.":
                    return oPorDia.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveFecha = dDiaCero.AddDays(c.Key.DayOfYear),
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveFecha);
                case "Precio":
                    return oPorDia.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveFecha = dDiaCero.AddDays(c.Key.DayOfYear),
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveFecha);
                case "Costo":
                    return oPorDia.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveFecha = dDiaCero.AddDays(c.Key.DayOfYear),
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveFecha);
                case "Costo Desc.":
                    return oPorDia.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveFecha = dDiaCero.AddDays(c.Key.DayOfYear),
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveFecha);
                case "Ventas":
                    return oPorDia.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveFecha = dDiaCero.AddDays(c.Key.DayOfYear),
                        Suc01_Actual = oDatos.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Matriz).Select(s => s.VentaID).Distinct().Count(),
                        Suc01_Anterior = oDatos.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Matriz).Select(s => s.VentaID).Distinct().Count(),
                        Suc02_Actual = oDatos.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Sucursal2).Select(s => s.VentaID).Distinct().Count(),
                        Suc02_Anterior = oDatos.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Sucursal2).Select(s => s.VentaID).Distinct().Count(),
                        Suc03_Actual = oDatos.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Sucursal3).Select(s => s.VentaID).Distinct().Count(),
                        Suc03_Anterior = oDatos.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Sucursal3).Select(s => s.VentaID).Distinct().Count()
                    }).OrderBy(o => o.LlaveFecha);
                case "Productos":
                    return oPorDia.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveFecha = dDiaCero.AddDays(c.Key.DayOfYear),
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveFecha);
            }

            return null;
        }

        private IEnumerable<AgrupadoPorSucursal> AgruparPorEntero(IEnumerable<IGrouping<int, pauCuadroDeControlGeneral_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Actual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Anterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Actual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Anterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Actual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Anterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveEntero);
                case "Utilidad Desc.":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveEntero);
                case "Precio":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveEntero);
                case "Costo":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveEntero);
                case "Costo Desc.":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveEntero);
                case "Ventas":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key,
                        Suc01_Actual = c.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Matriz).Select(s => s.VentaID).Distinct().Count(),
                        Suc01_Anterior = c.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Matriz).Select(s => s.VentaID).Distinct().Count(),
                        Suc02_Actual = c.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Sucursal2).Select(s => s.VentaID).Distinct().Count(),
                        Suc02_Anterior = c.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Sucursal2).Select(s => s.VentaID).Distinct().Count(),
                        Suc03_Actual = c.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Sucursal3).Select(s => s.VentaID).Distinct().Count(),
                        Suc03_Anterior = c.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Sucursal3).Select(s => s.VentaID).Distinct().Count()
                    }).OrderBy(o => o.LlaveEntero);
                case "Productos":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveEntero);
            }

            return null;
        }

        private IEnumerable<AgrupadoPorSucursal> AgruparPorEnteroCadena(IEnumerable<IGrouping<EnteroCadenaComp, pauCuadroDeControlGeneral_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Actual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.Anterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Actual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.Anterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Actual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.Anterior : null)).Valor(),
                    }).OrderBy(o => o.Cadena);
                case "Utilidad Desc.":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.UtilDescAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.UtilDescAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.UtilDescAnterior : null)).Valor(),
                    }).OrderBy(o => o.LlaveEntero);
                case "Precio":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.PrecioAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.PrecioAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.PrecioAnterior : null)).Valor(),
                    }).OrderBy(o => o.Cadena);
                case "Costo":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoAnterior : null)).Valor(),
                    }).OrderBy(o => o.Cadena);
                case "Costo Desc.":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.CostoDescAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.CostoDescAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.CostoDescAnterior : null)).Valor(),
                    }).OrderBy(o => o.Cadena);
                case "Ventas":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Suc01_Actual = c.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Matriz).Select(s => s.VentaID).Distinct().Count(),
                        Suc01_Anterior = c.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Matriz).Select(s => s.VentaID).Distinct().Count(),
                        Suc02_Actual = c.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Sucursal2).Select(s => s.VentaID).Distinct().Count(),
                        Suc02_Anterior = c.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Sucursal2).Select(s => s.VentaID).Distinct().Count(),
                        Suc03_Actual = c.Where(s => s.EsActual == true && s.SucursalID == Cat.Sucursales.Sucursal3).Select(s => s.VentaID).Distinct().Count(),
                        Suc03_Anterior = c.Where(s => s.EsActual != true && s.SucursalID == Cat.Sucursales.Sucursal3).Select(s => s.VentaID).Distinct().Count()
                    }).OrderBy(o => o.Cadena);
                case "Productos":
                    return oDatos.Select(c => new AgrupadoPorSucursal()
                    {
                        LlaveEntero = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Suc01_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosActual : null)).Valor(),
                        Suc01_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Matriz ? s.ProductosAnterior : null)).Valor(),
                        Suc02_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosActual : null)).Valor(),
                        Suc02_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal2 ? s.ProductosAnterior : null)).Valor(),
                        Suc03_Actual = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosActual : null)).Valor(),
                        Suc03_Anterior = c.Sum(s => (s.SucursalID == Cat.Sucursales.Sucursal3 ? s.ProductosAnterior : null)).Valor(),
                    }).OrderBy(o => o.Cadena);
            }

            return null;
        }

        private void FormatoColumnas()
        {
            string sFormato = ((this.cmbCalculo.Text == "Ventas" || this.cmbCalculo.Text == "Productos") ? "N" : "C");
            sFormato += Util.Cadena((int)this.nudDecimales.Value);

            switch (this.tabSucursales.SelectedTab.Name)
            {
                case "tbpPorFecha":
                    this.dgvPorDia.Columns["dia_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorDia.Columns["dia_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorDia.Columns["dia_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    // this.dgvPorDiaT.Columns["DiaT_Actual"].DefaultCellStyle.Format = sFormato;
                    // this.dgvPorDiaT.Columns["DiaT_Anterior"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorSemana.Columns["sem_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorSemana.Columns["sem_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorSemana.Columns["sem_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    // this.dgvPorSemanaT.Columns["SemanaT_Actual"].DefaultCellStyle.Format = sFormato;
                    // this.dgvPorSemanaT.Columns["SemanaT_Anterior"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorMes.Columns["mes_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorMes.Columns["mes_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorMes.Columns["mes_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    // this.dgvDias.Columns["Dias_AnioActual"].DefaultCellStyle.Format = sFormato;
                    // ths.dgvDias.Columns["Dias_AnioAnterior"].DefaultCellStyle.Format = sFormato;
                    // this.dgvHoras.Columns["Horas_AnioActual"].DefaultCellStyle.Format = sFormato;
                    // this.dgvHoras.Columns["Horas_AnioAnterior"].DefaultCellStyle.Format = sFormato;
                    break;
                case "tbpPorDiaYHora":
                    this.dgvPorDiaSem.Columns["dse_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorDiaSem.Columns["dse_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorDiaSem.Columns["dse_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorHora.Columns["hor_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorHora.Columns["hor_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    this.dgvPorHora.Columns["hor_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    break;
                case "tbpPorProveedor":
                case "tbpPorMarca":
                case "tbpPorLinea":
                    string sControl = ("ctl" + this.tabSucursales.SelectedTab.Name.Substring(3));
                    var oControl = (this.tabSucursales.SelectedTab.Controls[sControl] as CuadroMultipleSucursales);
                    oControl.dgvPrincipal.Columns["pri_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvPrincipal.Columns["pri_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvPrincipal.Columns["pri_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvMeses.Columns["mes_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvMeses.Columns["mes_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvMeses.Columns["mes_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvSemanas.Columns["sem_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvSemanas.Columns["sem_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
                    oControl.dgvSemanas.Columns["sem_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
                    break;
            }
        }

        #endregion

        #region [ Por fecha ]

        private void CargarPorFecha()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid de por día
            var oPorDia = this.AgruparPorDia(oDatos);
            this.dgvPorDia.Rows.Clear();
            foreach (var oReg in oPorDia)
            {
                this.dgvPorDia.Rows.Add(oReg.LlaveFecha
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
            }
            // this.dgvPorDiaT["DiaT_Actual", 0].Value = (oPorDia.Count() > 0 ? oPorDia.Average(c => c.Actual) : 0);
            // this.dgvPorDiaT["DiaT_Anterior", 0].Value = (oPorDia.Count() > 0 ? oPorDia.Average(c => c.Anterior) : 0);
            // Se llena el grid de por semana
            var oPorSemana = this.AgruparPorEntero(oDatos.GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            this.dgvPorSemana.Rows.Clear();
            this.chrPorSemana.Series["Suc01"].Points.Clear();
            this.chrPorSemana.Series["Suc02"].Points.Clear();
            this.chrPorSemana.Series["Suc03"].Points.Clear();
            foreach (var oReg in oPorSemana)
            {
                this.dgvPorSemana.Rows.Add(oReg.LlaveEntero
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                this.chrPorSemana.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.chrPorSemana.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.chrPorSemana.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }
            // this.dgvPorSemanaT["SemanaT_Actual", 0].Value = (oPorSemana.Count() > 0 ? oPorSemana.Average(c => c.Actual) : 0);
            // this.dgvPorSemanaT["SemanaT_Anterior", 0].Value = (oPorSemana.Count() > 0 ? oPorSemana.Average(c => c.Anterior) : 0);
            // Se llena el grid de por mes
            var oPorMes = this.AgruparPorEntero(oDatos.GroupBy(g => g.Fecha.Month));
            this.dgvPorMes.Rows.Clear();
            this.chrPorMes.Series["Suc01"].Points.Clear();
            this.chrPorMes.Series["Suc02"].Points.Clear();
            this.chrPorMes.Series["Suc03"].Points.Clear();
            foreach (var oReg in oPorMes)
            {
                this.dgvPorMes.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.LlaveEntero).ToUpper()
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                this.chrPorMes.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.chrPorMes.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.chrPorMes.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            // Se configuran columnas del grid
            this.FormatoColumnas();

            // Se llenan los totales
            // this.txtAnioActual.Text = oTotales.Actual.ToString(GlobalClass.FormatoMoneda);
            // this.txtAnioAnterior.Text = oTotales.Anterior.ToString(GlobalClass.FormatoMoneda);
            // this.txtResultado.Text = Util.DividirONull(oTotales.Actual, oTotales.Anterior).Valor().ToString(GlobalClass.FormatoDecimal);

            Cargando.Cerrar();
        }

        #endregion

        #region [ Día y horario ]

        private void CargarPorDiaSemYHora()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid de días
            var oPorDiaSem = this.AgruparPorEntero(oDatos.GroupBy(g => (int)g.Fecha.DayOfWeek));
            this.dgvPorDiaSem.Rows.Clear();
            this.chrPorDiaSem1.Series["Actual"].Points.Clear();
            this.chrPorDiaSem2.Series["Actual"].Points.Clear();
            this.chrPorDiaSem3.Series["Actual"].Points.Clear();
            foreach (var oReg in oPorDiaSem)
            {
                this.dgvPorDiaSem.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)oReg.LlaveEntero).ToUpper()
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                string sDia = CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName((DayOfWeek)oReg.LlaveEntero).ToUpper();
                int iPunto = this.chrPorDiaSem1.Series["Actual"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.chrPorDiaSem1.Series["Actual"].Points[iPunto].AxisLabel = sDia;
                iPunto = this.chrPorDiaSem2.Series["Actual"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.chrPorDiaSem1.Series["Actual"].Points[iPunto].AxisLabel = sDia;
                iPunto = this.chrPorDiaSem3.Series["Actual"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
                this.chrPorDiaSem1.Series["Actual"].Points[iPunto].AxisLabel = sDia;
            }
            // Se llena el grid de horas
            var oPorHora = this.AgruparPorEntero(oDatos.GroupBy(g => g.Fecha.Hour));
            this.dgvPorHora.Rows.Clear();
            this.chrPorHora1.Series.Clear();
            this.chrPorHora2.Series.Clear();
            this.chrPorHora3.Series.Clear();
            foreach (var oReg in oPorHora)
            {
                this.dgvPorHora.Rows.Add(string.Format("{0:00}:00", oReg.LlaveEntero)
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                this.AgregarSerieCilindro(this.chrPorHora1, oReg.LlaveEntero, oReg.Suc01_Actual);
                this.AgregarSerieCilindro(this.chrPorHora2, oReg.LlaveEntero, oReg.Suc02_Actual);
                this.AgregarSerieCilindro(this.chrPorHora3, oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            // Se configuran columnas del grid
            this.FormatoColumnas();

            Cargando.Cerrar();
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

        #region [ Proveedores ]

        private void ctlPorProveedor_dgvPrincipal_CurrentCellChanged(object sender, EventArgs e)
        {
            this.ctlPorProveedor.dgvSemanas.Rows.Clear();
            this.ctlPorProveedor.dgvMeses.Rows.Clear();
            if (!this.ctlPorProveedor.dgvPrincipal.Focused) return;

            bool bSelNueva = this.ctlPorProveedor.dgvPrincipal.VerSeleccionNueva();
            if (bSelNueva)
            {
                int iId = (this.ctlPorProveedor.dgvPrincipal.CurrentRow == null ? 0 : 
                    Util.Entero(this.ctlPorProveedor.dgvPrincipal.CurrentRow.Cells["pri_Id"].Value));
                this.PorProveedorLlenarSemanas(iId);
                this.PorProveedorLlenarMeses(iId);
            }
        }

        private void CargarPorProveedor()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid de proveedores
            var oProveedores = this.AgruparPorEnteroCadena(oDatos.GroupBy(g => new EnteroCadenaComp { Entero = g.ProveedorID.Valor(), Cadena = g.Proveedor }));
            this.ctlPorProveedor.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oProveedores)
            {
                this.ctlPorProveedor.dgvPrincipal.Rows.Add(oReg.LlaveEntero, oReg.Cadena
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
            }
            // Se llenan los totales
            // decimal mTotalAnt = (oProveedores.Count() > 0 ? oProveedores.Sum(c => c.Anterior) : 0);
            // this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotal;
            // this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalAnt;
            // this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(mTotal, mTotalAnt);

            // Para configurar las columnas de los grids
            this.ctlPorProveedor.FormatoColumnas(this.cmbCalculo.Text, (int)this.nudDecimales.Value);
            // Orden
            this.ctlPorProveedor.dgvPrincipal.Sort(this.ctlPorProveedor.dgvPrincipal.Columns["pri_Suc01_Actual"], ListSortDirection.Descending);

            Cargando.Cerrar();
        }

        private void PorProveedorLlenarSemanas(int iId)
        {
            this.ctlPorProveedor.dgvSemanas.Rows.Clear();
            this.ctlPorProveedor.chrSemanas.Series["Suc01"].Points.Clear();
            this.ctlPorProveedor.chrSemanas.Series["Suc02"].Points.Clear();
            this.ctlPorProveedor.chrSemanas.Series["Suc03"].Points.Clear();
            if (iId <= 0)
                return;

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid y la gráfica de Semanas
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            foreach (var oReg in oConsulta)
            {
                this.ctlPorProveedor.dgvSemanas.Rows.Add(oReg.LlaveEntero
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                // Para la gráfica
                this.ctlPorProveedor.chrSemanas.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.ctlPorProveedor.chrSemanas.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.ctlPorProveedor.chrSemanas.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            Cargando.Cerrar();
        }

        private void PorProveedorLlenarMeses(int iId)
        {
            this.ctlPorProveedor.dgvMeses.Rows.Clear();
            this.ctlPorProveedor.chrMeses.Series["Suc01"].Points.Clear();
            this.ctlPorProveedor.chrMeses.Series["Suc02"].Points.Clear();
            this.ctlPorProveedor.chrMeses.Series["Suc03"].Points.Clear();
            if (iId <= 0)
                return;

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid y la gráfica de Meses
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => g.Fecha.Month));
            foreach (var oReg in oConsulta)
            {
                this.ctlPorProveedor.dgvMeses.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.LlaveEntero).ToUpper()
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                // Para la gráfica
                this.ctlPorProveedor.chrMeses.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.ctlPorProveedor.chrMeses.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.ctlPorProveedor.chrMeses.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            Cargando.Cerrar();
        }

        #endregion

        #region [ Marcas ]

        private void ctlPorMarca_dgvPrincipal_CurrentCellChanged(object sender, EventArgs e)
        {
            this.ctlPorMarca.dgvSemanas.Rows.Clear();
            this.ctlPorMarca.dgvMeses.Rows.Clear();
            if (!this.ctlPorMarca.dgvPrincipal.Focused) return;

            bool bSelNueva = this.ctlPorMarca.dgvPrincipal.VerSeleccionNueva();
            if (bSelNueva)
            {
                int iId = (this.ctlPorMarca.dgvPrincipal.CurrentRow == null ? 0 :
                    Util.Entero(this.ctlPorMarca.dgvPrincipal.CurrentRow.Cells["pri_Id"].Value));
                this.PorMarcaLlenarSemanas(iId);
                this.PorMarcaLlenarMeses(iId);
            }
        }

        private void CargarPorMarca()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid de proveedores
            var oMarcas = this.AgruparPorEnteroCadena(oDatos.GroupBy(g => new EnteroCadenaComp { Entero = g.MarcaID.Valor(), Cadena = g.Marca }));
            this.ctlPorMarca.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oMarcas)
            {
                this.ctlPorMarca.dgvPrincipal.Rows.Add(oReg.LlaveEntero, oReg.Cadena
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
            }
            // Se llenan los totales
            // decimal mTotalAnt = (oProveedores.Count() > 0 ? oProveedores.Sum(c => c.Anterior) : 0);
            // this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotal;
            // this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalAnt;
            // this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(mTotal, mTotalAnt);

            // Para configurar las columnas de los grids
            this.ctlPorMarca.FormatoColumnas(this.cmbCalculo.Text, (int)this.nudDecimales.Value);
            // Orden
            this.ctlPorMarca.dgvPrincipal.Sort(this.ctlPorMarca.dgvPrincipal.Columns["pri_Suc01_Actual"], ListSortDirection.Descending);

            Cargando.Cerrar();
        }

        private void PorMarcaLlenarSemanas(int iId)
        {
            this.ctlPorMarca.dgvSemanas.Rows.Clear();
            this.ctlPorMarca.chrSemanas.Series["Suc01"].Points.Clear();
            this.ctlPorMarca.chrSemanas.Series["Suc02"].Points.Clear();
            this.ctlPorMarca.chrSemanas.Series["Suc03"].Points.Clear();
            if (iId <= 0)
                return;

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid y la gráfica de Semanas
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.MarcaID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            foreach (var oReg in oConsulta)
            {
                this.ctlPorMarca.dgvSemanas.Rows.Add(oReg.LlaveEntero
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                // Para la gráfica
                this.ctlPorMarca.chrSemanas.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.ctlPorMarca.chrSemanas.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.ctlPorMarca.chrSemanas.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            Cargando.Cerrar();
        }

        private void PorMarcaLlenarMeses(int iId)
        {
            this.ctlPorMarca.dgvMeses.Rows.Clear();
            this.ctlPorMarca.chrMeses.Series["Suc01"].Points.Clear();
            this.ctlPorMarca.chrMeses.Series["Suc02"].Points.Clear();
            this.ctlPorMarca.chrMeses.Series["Suc03"].Points.Clear();
            if (iId <= 0)
                return;

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid y la gráfica de Meses
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.MarcaID == iId).GroupBy(g => g.Fecha.Month));
            foreach (var oReg in oConsulta)
            {
                this.ctlPorMarca.dgvMeses.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.LlaveEntero).ToUpper()
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                // Para la gráfica
                this.ctlPorMarca.chrMeses.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.ctlPorMarca.chrMeses.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.ctlPorMarca.chrMeses.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            Cargando.Cerrar();
        }

        #endregion

        #region [ Líneas ]

        private void ctlPorLinea_dgvPrincipal_CurrentCellChanged(object sender, EventArgs e)
        {
            this.ctlPorLinea.dgvSemanas.Rows.Clear();
            this.ctlPorLinea.dgvMeses.Rows.Clear();
            if (!this.ctlPorLinea.dgvPrincipal.Focused) return;

            bool bSelNueva = this.ctlPorLinea.dgvPrincipal.VerSeleccionNueva();
            if (bSelNueva)
            {
                int iId = (this.ctlPorLinea.dgvPrincipal.CurrentRow == null ? 0 :
                    Util.Entero(this.ctlPorLinea.dgvPrincipal.CurrentRow.Cells["pri_Id"].Value));
                this.PorLineaLlenarSemanas(iId);
                this.PorLineaLlenarMeses(iId);
            }
        }

        private void CargarPorLinea()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid de proveedores
            var oLineas = this.AgruparPorEnteroCadena(oDatos.GroupBy(g => new EnteroCadenaComp { Entero = g.LineaID.Valor(), Cadena = g.Linea }));
            this.ctlPorLinea.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oLineas)
            {
                this.ctlPorLinea.dgvPrincipal.Rows.Add(oReg.LlaveEntero, oReg.Cadena
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
            }
            // Se llenan los totales
            // decimal mTotalAnt = (oProveedores.Count() > 0 ? oProveedores.Sum(c => c.Anterior) : 0);
            // this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotal;
            // this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalAnt;
            // this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(mTotal, mTotalAnt);

            // Para configurar las columnas de los grids
            this.ctlPorLinea.FormatoColumnas(this.cmbCalculo.Text, (int)this.nudDecimales.Value);
            // Orden
            this.ctlPorLinea.dgvPrincipal.Sort(this.ctlPorLinea.dgvPrincipal.Columns["pri_Suc01_Actual"], ListSortDirection.Descending);

            Cargando.Cerrar();
        }

        private void PorLineaLlenarSemanas(int iId)
        {
            this.ctlPorLinea.dgvSemanas.Rows.Clear();
            this.ctlPorLinea.chrSemanas.Series["Suc01"].Points.Clear();
            this.ctlPorLinea.chrSemanas.Series["Suc02"].Points.Clear();
            this.ctlPorLinea.chrSemanas.Series["Suc03"].Points.Clear();
            if (iId <= 0)
                return;

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid y la gráfica de Semanas
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.LineaID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            foreach (var oReg in oConsulta)
            {
                this.ctlPorLinea.dgvSemanas.Rows.Add(oReg.LlaveEntero
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                // Para la gráfica
                this.ctlPorLinea.chrSemanas.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.ctlPorLinea.chrSemanas.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.ctlPorLinea.chrSemanas.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            Cargando.Cerrar();
        }

        private void PorLineaLlenarMeses(int iId)
        {
            this.ctlPorLinea.dgvMeses.Rows.Clear();
            this.ctlPorLinea.chrMeses.Series["Suc01"].Points.Clear();
            this.ctlPorLinea.chrMeses.Series["Suc02"].Points.Clear();
            this.ctlPorLinea.chrMeses.Series["Suc03"].Points.Clear();
            if (iId <= 0)
                return;

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oTotales = this.ObtenerTotales(oDatos);

            // Se llena el grid y la gráfica de Meses
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.LineaID == iId).GroupBy(g => g.Fecha.Month));
            foreach (var oReg in oConsulta)
            {
                this.ctlPorLinea.dgvMeses.Rows.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.LlaveEntero).ToUpper()
                    , oReg.Suc01_Actual, Util.DividirONull(oReg.Suc01_Actual, oReg.Suc01_Anterior), (Util.DividirONull(oReg.Suc01_Actual, oTotales.Suc01_Actual) * 100)
                    , oReg.Suc02_Actual, Util.DividirONull(oReg.Suc02_Actual, oReg.Suc02_Anterior), (Util.DividirONull(oReg.Suc02_Actual, oTotales.Suc02_Actual) * 100)
                    , oReg.Suc03_Actual, Util.DividirONull(oReg.Suc03_Actual, oReg.Suc03_Anterior), (Util.DividirONull(oReg.Suc03_Actual, oTotales.Suc03_Actual) * 100)
                );
                // Para la gráfica
                this.ctlPorLinea.chrMeses.Series["Suc01"].Points.AddXY(oReg.LlaveEntero, oReg.Suc01_Actual);
                this.ctlPorLinea.chrMeses.Series["Suc02"].Points.AddXY(oReg.LlaveEntero, oReg.Suc02_Actual);
                this.ctlPorLinea.chrMeses.Series["Suc03"].Points.AddXY(oReg.LlaveEntero, oReg.Suc03_Actual);
            }

            Cargando.Cerrar();
        }

        #endregion

        #region [ Clases auxiliares ]

        internal class EnteroCadenaComp : IEnumerable, IEquatable<EnteroCadenaComp>
        {

            public int Entero { get; set; }
            public string Cadena { get; set; }

            public IEnumerator GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public bool Equals(EnteroCadenaComp other)
            {
                if (Object.ReferenceEquals(other, null)) return false;
                if (Object.ReferenceEquals(this, other)) return true;
                return (this.Entero.Equals(other.Entero) && this.Cadena.Equals(other.Cadena));
            }

            public override int GetHashCode()
            {
                int hashEntero = (this.Entero == null ? 0 : this.Entero.GetHashCode());
                int hashCadena = this.Cadena.GetHashCode();
                return (hashEntero ^ hashCadena);
            }
        }

        #endregion

    }
}
