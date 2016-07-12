using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroMultiple : UserControl
    {
        protected BindingSource oFuente;
        protected class GridFuente
        {
            public const int Principal = 1;
            public const int Grupos = 2;
            public const int Meses = 3;
            public const int Semanas = 4;
            public const int Vendedores = 5;
            public const int Sucursales = 6;
        }

        public CuadroMultiple()
        {
            InitializeComponent();
        }

        #region [ Clases auxiliares ]

        protected class TiposDeReporte
        {
            public const int Proveedores = 1;
            public const int Marcas = 2;
            public const int Lineas = 3;
            public const int Vendedores = 4;
            public const int Clientes = 5;
        }

        protected class EnteroCadenaComp : IEnumerable, IEquatable<EnteroCadenaComp>
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

        protected class AgrupadoPorEnteroCadenaEntero : IEquatable<AgrupadoPorEnteroCadenaEntero>
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

        class TotalesPorFecha
        {
            public DateTime Llave { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }

        protected class TotalesPorEntero
        {
            public int Llave { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }

        protected class TotalesPorEnteroCadena
        {
            public int Llave { get; set; }
            public string Cadena { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }
                
        #endregion

        #region [ Propiedades ]

        public int TipoDeReporte { get; set; }

        #endregion

        #region [ Eventos ]

        private void CuadroMultiple_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            
            // Se llenan los tipos de cálculo
            this.cmbCalculo.Items.AddRange(new object[] { "Utilidad", "Utilidad Desc.", "Precio", "Costo", "Costo Desc.", "Ventas", "Productos" });
            this.cmbCalculo.SelectedIndex = 1;
            // Se llenan las Sucursales
            var oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            oSucursales.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "Todas" });
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", oSucursales);
            this.cmbSucursal.SelectedValue = 0;
            this.chkPagadas.Checked = true;
            this.chkCostoConDescuento.Checked = true;
            // Se mustran u ocunltan los combos especiales de marcas y líneas, según el caso
            if (this.TipoDeReporte == TiposDeReporte.Marcas || this.TipoDeReporte == TiposDeReporte.Lineas)
            {
                var oMarcas = Datos.GetListOf<MarcaParte>(c => c.Estatus).OrderBy(c => c.NombreMarcaParte);
                foreach (var oReg in oMarcas)
                    this.ctlMarcas.AgregarElemento(oReg.MarcaParteID, oReg.NombreMarcaParte);
                var oLineas = Datos.GetListOf<Linea>(c => c.Estatus).OrderBy(c => c.NombreLinea);
                foreach (var oReg in oLineas)
                    this.ctlLineas.AgregarElemento(oReg.LineaID, oReg.NombreLinea);
            }
            // Se llenan las fechas
            this.dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);
            // Se agrega la fila de totales del grid principal
            this.dgvPrincipalTotales.Rows.Add("Totales");
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

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.dgvPrincipal.FiltrarContiene(this.txtBusqueda.Text, "Principal_Nombre");
        }

        protected void dgvPrincipal_CurrentCellChanged(object sender, EventArgs e)
        {
            this.dgvGrupos.Rows.Clear();
            this.dgvSemanas.Rows.Clear();
            this.dgvMeses.Rows.Clear();
            this.dgvVendedor.Rows.Clear();
            this.dgvSucursal.Rows.Clear();
            this.dgvPartes.Rows.Clear();
            if (!this.dgvPrincipal.Focused) return;

            Cargando.Mostrar();

            bool bSelNueva = this.dgvPrincipal.VerSeleccionNueva();
            if (bSelNueva)
            {
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                this.LlenarGrupos(iId);
                this.LlenarSemanas(iId);
                this.LlenarMeses(iId);
                this.LLenarVendedores(iId);
                this.LlenarSucursales(iId);
                // this.dgvPartes.Rows.Clear();
            }

            Cargando.Cerrar();

            this.PrincipalCambioSel(bSelNueva);
        }
                
        private void dgvGrupos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvGrupos.Focused && this.dgvGrupos.VerSeleccionNueva())
            {
                Cargando.Mostrar();

                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iGrupoId = (this.dgvGrupos.CurrentRow == null ? 0 : Util.Entero(this.dgvGrupos.CurrentRow.Cells["Grupos_Id"].Value));

                // Se cargan todos los grids, si es marcas o líneas
                if (this.TipoDeReporte == TiposDeReporte.Marcas || this.TipoDeReporte == TiposDeReporte.Lineas)
                {
                    this.LlenarSemanas(iGrupoId);
                    this.LlenarMeses(iGrupoId);
                    this.LLenarVendedores(iGrupoId);
                    this.LlenarSucursales(iGrupoId);
                }

                this.LlenarPartes(CuadroMultiple.GridFuente.Grupos, iId, iGrupoId);

                Cargando.Cerrar();
            }
        }

        private void dgvSemanas_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvSemanas.Focused && this.dgvSemanas.VerSeleccionNueva())
            {
                Cargando.Mostrar();
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iSemana = (this.dgvSemanas.CurrentRow == null ? 0 : Util.Entero(this.dgvSemanas.CurrentRow.Cells["Semanas_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Semanas, iId, iSemana);
                Cargando.Cerrar();
            }
        }

        private void dgvMeses_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvMeses.Focused && this.dgvMeses.VerSeleccionNueva())
            {
                Cargando.Mostrar();
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iMes = (this.dgvMeses.CurrentRow == null ? 0 : Util.Entero(this.dgvMeses.CurrentRow.Cells["Meses_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Meses, iId, iMes);
                Cargando.Cerrar();
            }
        }

        private void dgvVendedor_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvVendedor.Focused && this.dgvVendedor.VerSeleccionNueva())
            {
                Cargando.Mostrar();
                int iIdPrincipal = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iId = (this.dgvVendedor.CurrentRow == null ? 0 : Util.Entero(this.dgvVendedor.CurrentRow.Cells["Vendedor_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Vendedores, iIdPrincipal, iId);
                Cargando.Cerrar();
            }
        }

        private void dgvSucursal_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvSucursal.Focused && this.dgvSucursal.VerSeleccionNueva())
            {
                Cargando.Mostrar();
                int iIdPrincipal = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iId = (this.dgvSucursal.CurrentRow == null ? 0 : Util.Entero(this.dgvSucursal.CurrentRow.Cells["Sucursal_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Sucursales, iIdPrincipal, iId);
                Cargando.Cerrar();
            }
        }

        #endregion

        #region [ Métodos ]

        private void AplicarFormatoColumnas()
        {
            string sFormato = ((this.cmbCalculo.Text == "Ventas" || this.cmbCalculo.Text == "Productos") ? "N" : "C");
            sFormato += Util.Cadena((int)this.nudDecimales.Value);
            this.dgvPrincipal.Columns["Principal_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvPrincipal.Columns["Principal_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvPrincipalTotales.Columns["PrincipalT_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvPrincipalTotales.Columns["PrincipalT_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvGrupos.Columns["Grupos_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvGrupos.Columns["Grupos_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvMeses.Columns["Meses_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvMeses.Columns["Meses_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanas.Columns["Semanas_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanas.Columns["Semanas_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvSucursal.Columns["Sucursal_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSucursal.Columns["Sucursal_Anterior"].DefaultCellStyle.Format = sFormato;
            this.dgvVendedor.Columns["Vendedor_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvVendedor.Columns["Vendedor_Anterior"].DefaultCellStyle.Format = sFormato;
        }

        #endregion

        #region [ Virtual ]

        protected virtual Dictionary<string, object> ObtenerParametros()
        {
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

            return oParams;
        }

        protected virtual void LlenarDetalle(IEnumerable<pauCuadroDeControlGeneral_Result> oDatos, int? iLineaID)
        {
            List<int> oVentasProc = new List<int>();

            foreach (var oReg in oDatos)
            {
                if (oVentasProc.Contains(oReg.VentaID)) continue;
                oVentasProc.Add(oReg.VentaID);

                var oVentasDetV = Datos.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID && (iLineaID == null || c.LineaID == iLineaID));
                foreach (var oDet in oVentasDetV)
                    this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte
                        , ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
            }
        }

        protected virtual void PrincipalCambioSel(bool bSelNueva)
        {
            
        }

        protected virtual void CargarDatos()
        {
            // Se configuran columnas del grid
            this.AplicarFormatoColumnas();
        }

        protected virtual void LlenarGrupos(int iId) { }

        protected virtual void LlenarSemanas(int iId) { }

        protected virtual void LlenarMeses(int iId) { }

        protected virtual void LLenarVendedores(int iId) { }

        protected virtual void LlenarSucursales(int iId) { }

        protected virtual void LlenarPartes(int iGridFuente, int iIdPrincipal, int iId) { }

        protected virtual IEnumerable<TotalesPorEntero> AgruparPorEntero(IEnumerable<IGrouping<int, pauCuadroDeControlGeneral_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new TotalesPorEntero()
                    {
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
                        Anterior = c.Where(s => s.EsActual != true).Select(s => s.VentaID).Distinct().Count(),
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

        protected virtual IEnumerable<TotalesPorEnteroCadena> AgruparPorEnteroCadena(IEnumerable<IGrouping<EnteroCadenaComp, pauCuadroDeControlGeneral_Result>> oDatos)
        {
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oDatos.Select(c => new TotalesPorEnteroCadena()
                    {
                        Llave = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Actual = c.Sum(s => s.Actual).Valor(),
                        Anterior = c.Sum(s => s.Anterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Utilidad Desc.":
                    return oDatos.Select(c => new TotalesPorEnteroCadena()
                    {
                        Llave = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Actual = c.Sum(s => s.UtilDescActual).Valor(),
                        Anterior = c.Sum(s => s.UtilDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Precio":
                    return oDatos.Select(c => new TotalesPorEnteroCadena()
                    {
                        Llave = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Actual = c.Sum(s => s.PrecioActual).Valor(),
                        Anterior = c.Sum(s => s.PrecioAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo":
                    return oDatos.Select(c => new TotalesPorEnteroCadena()
                    {
                        Llave = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Actual = c.Sum(s => s.CostoActual).Valor(),
                        Anterior = c.Sum(s => s.CostoAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Costo Desc.":
                    return oDatos.Select(c => new TotalesPorEnteroCadena()
                    {
                        Llave = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Actual = c.Sum(s => s.CostoDescActual).Valor(),
                        Anterior = c.Sum(s => s.CostoDescAnterior).Valor()
                    }).OrderBy(o => o.Llave);
                case "Ventas":
                    return oDatos.Select(c => new TotalesPorEnteroCadena()
                    {
                        Llave = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Actual = c.Where(s => s.EsActual == true).Select(s => s.VentaID).Distinct().Count(),
                        Anterior = c.Where(s => s.EsActual != true).Select(s => s.VentaID).Distinct().Count(),
                    }).OrderBy(o => o.Llave);
                case "Productos":
                    return oDatos.Select(c => new TotalesPorEnteroCadena()
                    {
                        Llave = c.Key.Entero,
                        Cadena = c.Key.Cadena,
                        Actual = c.Sum(s => s.ProductosActual).Valor(),
                        Anterior = c.Sum(s => s.ProductosAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }

        protected virtual IEnumerable<AgrupadoPorEnteroCadenaEntero> AgruparPorEnteroCadenaEntero(
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
                        Actual = c.Where(s => s.EsActual == true).Select(s => s.VentaID).Distinct().Count(),
                        Anterior = c.Where(s => s.EsActual != true).Select(s => s.VentaID).Distinct().Count(),
                    }).OrderBy(o => o.Llave);
                case "Productos":
                    return oDatos.Select(c => new AgrupadoPorEnteroCadenaEntero()
                    {
                        Llave = c.Key.Llave,
                        Cadena = c.Key.Cadena,
                        Entero = c.Key.Entero,
                        Actual = c.Sum(s => s.ProductosActual).Valor(),
                        Anterior = c.Sum(s => s.ProductosAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }

        protected virtual IEnumerable<TotalesPorEnteroCadena> FiltrarMarcas(IEnumerable<TotalesPorEnteroCadena> oDatos)
        {
            return oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.Llave));
        }

        protected virtual void FiltrarLineas()
        {

        }
        
        #endregion

        #region [ Públicos ]

        public void MostrarComboMarcas(bool bMostrar)
        {
            this.lblMarcas.Visible = bMostrar;
            this.ctlMarcas.Visible = bMostrar;
        }

        public void MostrarComboLineas(bool bMostrar)
        {
            this.lblLineas.Visible = bMostrar;
            this.ctlLineas.Visible = bMostrar;
            this.lblLineas.Left = this.lblMarcas.Left;
            this.ctlLineas.Left = this.ctlMarcas.Left;
        }

        #endregion
    }
}
