﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

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

        protected class EnteroCadena : IEnumerable, IEquatable<EnteroCadena>
        {

            public int Entero { get; set; }
            public string Cadena { get; set; }

            public IEnumerator GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public bool Equals(EnteroCadena other)
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

        #region [ Eventos ]

        private void CuadroMultiple_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            
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

            bool bSelNueva = this.dgvPrincipal.VerSeleccionNueva();
            if (bSelNueva)
            {
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                this.LlenarGrupos(iId);
                this.LlenarSemanas(iId);
                this.LlenarMeses(iId);
                this.LLenarVendedores(iId);
                this.LlenarSucursales(iId);
                // this.dgvPartes.Rows.Clear();
            }

            this.PrincipalCambioSel(bSelNueva);
        }
                
        private void dgvGrupos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvGrupos.Focused && this.dgvGrupos.VerSeleccionNueva())
            {
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iGrupoId = (this.dgvGrupos.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvGrupos.CurrentRow.Cells["Grupos_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Grupos, iId, iGrupoId);
            }
        }

        private void dgvSemanas_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvSemanas.Focused && this.dgvSemanas.VerSeleccionNueva())
            {
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iSemana = (this.dgvSemanas.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvSemanas.CurrentRow.Cells["Semanas_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Semanas, iId, iSemana);
            }
        }

        private void dgvMeses_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvMeses.Focused && this.dgvMeses.VerSeleccionNueva())
            {
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iMes = (this.dgvMeses.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvMeses.CurrentRow.Cells["Meses_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Meses, iId, iMes);
            }
        }

        private void dgvVendedor_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvVendedor.Focused && this.dgvVendedor.VerSeleccionNueva())
            {
                int iIdPrincipal = (this.dgvPrincipal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iId = (this.dgvVendedor.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvVendedor.CurrentRow.Cells["Vendedor_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Vendedores, iIdPrincipal, iId);
            }
        }

        private void dgvSucursal_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvSucursal.Focused && this.dgvSucursal.VerSeleccionNueva())
            {
                int iIdPrincipal = (this.dgvPrincipal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                int iId = (this.dgvSucursal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvSucursal.CurrentRow.Cells["Sucursal_Id"].Value));
                this.LlenarPartes(CuadroMultiple.GridFuente.Sucursales, iIdPrincipal, iId);
            }
        }

        #endregion

        #region [ Métodos ]

        

        #endregion

        #region [ Virtual ]

        protected virtual Dictionary<string, object> ObtenerParametros()
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

        protected virtual void LlenarDetalle(IEnumerable<pauCuadroDeControlGeneral_Result> oDatos, int? iLineaID)
        {
            List<int> oVentasProc = new List<int>();

            foreach (var oReg in oDatos)
            {
                if (oVentasProc.Contains(oReg.VentaID)) continue;
                oVentasProc.Add(oReg.VentaID);

                var oVentasDetV = General.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID && (iLineaID == null || c.LineaID == iLineaID));
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
            this.dgvPrincipal.Columns["Principal_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvPrincipal.Columns["Principal_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvPrincipalTotales.Columns["PrincipalT_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvPrincipalTotales.Columns["PrincipalT_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvGrupos.Columns["Grupos_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvGrupos.Columns["Grupos_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvMeses.Columns["Meses_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvMeses.Columns["Meses_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvSemanas.Columns["Semanas_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvSemanas.Columns["Semanas_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvSucursal.Columns["Sucursal_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvSucursal.Columns["Sucursal_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvVendedor.Columns["Vendedor_Actual"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
            this.dgvVendedor.Columns["Vendedor_Anterior"].DefaultCellStyle.Format = (this.cmbCalculo.Text == "Ventas" ? "N0" : "C2");
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
                        Actual = c.Sum(s => s.VentasActual).Valor(),
                        Anterior = c.Sum(s => s.VentasAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }

        protected virtual IEnumerable<TotalesPorEnteroCadena> AgruparPorEnteroCadena(IEnumerable<IGrouping<EnteroCadena, pauCuadroDeControlGeneral_Result>> oDatos)
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
                        Actual = c.Sum(s => s.VentasActual).Valor(),
                        Anterior = c.Sum(s => s.VentasAnterior).Valor()
                    }).OrderBy(o => o.Llave);
            }

            return null;
        }
                
        protected IEnumerable<AgrupadoPorEnteroCadenaEntero> AgruparPorEnteroCadenaEntero(
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

        #endregion
                                                
    }
}