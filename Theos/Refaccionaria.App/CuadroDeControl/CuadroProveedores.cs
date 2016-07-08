using System;
using System.Windows.Forms;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroProveedores : CuadroMultiple
    {
        public CuadroProveedores()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public int ProveedorFijoID { get; set; }

        #endregion

        #region [ Eventos ]

        private void CuadroProveedores_Load(object sender, EventArgs e)
        {
            // Se configura el combo de tipo
            this.cmbTipo.Items.AddRange(new object[] { "Ventas", "Compras" });
            this.cmbTipo.SelectedIndex = 0;

            // Se configuran las columnas
            this.dgvPrincipal.Columns["Principal_Nombre"].HeaderText = "Proveedor";

            // Se manda a cargar los datos
            // this.CargarDatos();
        }

        private void cmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (this.cmbTipo.Focused)
            //     this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            // Se verifica si está fijo ya el cliente
            if (this.ProveedorFijoID > 0)
            {
                this.CargarDatos(this.ProveedorFijoID);
                return;
            }

            // Si es tipo compras, se va a otro método
            if (this.cmbTipo.SelectedIndex == 1)
            {
                this.cmbCalculo.Text = "Utilidad Desc.";
                this.CargarDatosCompras();
                return;
            }

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de proveedores
            var oProveedores = this.AgruparPorEnteroCadena(oDatos.GroupBy(g => new EnteroCadenaComp() { Entero = g.ProveedorID.Valor(), Cadena = g.Proveedor }))
                .OrderByDescending(c => c.Actual);
            decimal mTotal = (oProveedores.Count() > 0 ? oProveedores.Sum(c => c.Actual) : 0);
            this.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oProveedores)
            {
                this.dgvPrincipal.Rows.Add(oReg.Llave, oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
            // Se llenan los totales
            decimal mTotalAnt = (oProveedores.Count() > 0 ? oProveedores.Sum(c => c.Anterior) : 0);
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

            // Si es tipo compras, se va a otro método
            if (this.cmbTipo.SelectedIndex == 1)
            {
                this.LlenarGruposCompras(iId);
                return;
            }

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de clientes
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.LineaID.Valor(), Cadena = g.Linea }))
                .OrderByDescending(c => c.Actual);
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
            this.chrSemanas.Series["Actual"].Points.Clear();
            this.chrSemanas.Series["Pasado"].Points.Clear();
            if (iId <= 0)
                return;

            // Si es tipo compras, se va a otro método
            if (this.cmbTipo.SelectedIndex == 1)
            {
                this.LlenarSemanasCompras(iId);
                return;
            }

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica de Semanas
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvSemanas.Rows.Add(oReg.Llave, oReg.Llave, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                // Para la gráfica
                this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Llave, oReg.Actual);
                this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Llave, oReg.Anterior);
            }
        }

        protected override void LlenarMeses(int iId)
        {
            this.dgvMeses.Rows.Clear();
            this.chrMeses.Series["Actual"].Points.Clear();
            this.chrMeses.Series["Pasado"].Points.Clear();
            if (iId <= 0)
                return;

            // Si es tipo compras, se va a otro método
            if (this.cmbTipo.SelectedIndex == 1)
            {
                this.LlenarMesesCompras(iId);
                return;
            }

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica de Meses
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => g.Fecha.Month));
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

        protected override void LLenarVendedores(int iId)
        {
            this.dgvVendedor.Rows.Clear();
            if (iId <= 0)
                return;

            // Si es tipo compras, se sale, pues no aplica
            if (this.cmbTipo.SelectedIndex == 1)
            {
                return;
            }

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de Vendedor
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.VendedorID, Cadena = g.Vendedor }))
                .OrderByDescending(c => c.Actual);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvVendedor.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected override void LlenarSucursales(int iId)
        {
            this.dgvSucursal.Rows.Clear();
            if (iId <= 0)
                return;

            // Si es tipo compras, se sale, pues no aplica
            if (this.cmbTipo.SelectedIndex == 1)
            {
                return;
            }

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de Sucursales
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.SucursalID, Cadena = g.Sucursal }));
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

            // Si es tipo compras, se va a otro método
            if (this.cmbTipo.SelectedIndex == 1)
            {
                this.LlenarPartesCompras(iGridFuente, iIdPrincipal, iId);
                return;
            }

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            var oConsulta = oDatos.Where(c => c.ProveedorID == iIdPrincipal && c.Fecha >= this.dtpDesde.Value.Date);
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
                case CuadroMultiple.GridFuente.Vendedores:
                    oConsulta = oConsulta.Where(c => c.VendedorID == iId);
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
            } */
        }

        #endregion

        #region [ Tipo Compras ]

        protected Dictionary<string, object> ObtenerParametrosCompras()
        {
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            return oParams;
        }

        protected void CargarDatosCompras()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametrosCompras();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCompras_Result>("pauCuadroDeControlCompras", oParams);

            // Se llena el grid principal
            var oConsulta = oDatos.GroupBy(g => new { g.ProveedorID, g.Proveedor })
                .Select(c => new { c.Key.ProveedorID, c.Key.Proveedor, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) }).OrderByDescending(c => c.Actual);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual).Valor() : 0);
            this.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oConsulta)
            {
                this.dgvPrincipal.Rows.Add(oReg.ProveedorID, oReg.ProveedorID, oReg.Proveedor, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
            // Se llenan los totales
            decimal mTotalAnt = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Anterior).Valor() : 0);
            this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotal;
            this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalAnt;
            this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(mTotal, mTotalAnt);

            Cargando.Cerrar();
        }

        protected void LlenarGruposCompras(int iId)
        {
            this.dgvGrupos.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametrosCompras();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCompras_Result>("pauCuadroDeControlCompras", oParams);

            // Se llena el grid de clientes
            var oConsulta = oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => new { g.LineaID, g.Linea }).Select(
                c => new { c.Key.LineaID, c.Key.Linea, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) }).OrderByDescending(c => c.Actual);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual).Valor() : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvGrupos.Rows.Add(oReg.LineaID, oReg.Linea, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected void LlenarSemanasCompras(int iId)
        {
            this.dgvSemanas.Rows.Clear();
            this.chrSemanas.Series["Actual"].Points.Clear();
            this.chrSemanas.Series["Pasado"].Points.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametrosCompras();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCompras_Result>("pauCuadroDeControlCompras", oParams);

            // Se llena el grid y la gráfica de Semanas
            var oConsulta = oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha.Valor())).Select(
                c => new { Semana = c.Key, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) }).OrderBy(c => c.Semana);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual).Valor() : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvSemanas.Rows.Add(oReg.Semana, oReg.Semana, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                // Para la gráfica
                this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Semana, oReg.Actual);
                this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Semana, oReg.Anterior);
            }
        }

        protected void LlenarMesesCompras(int iId)
        {
            this.dgvMeses.Rows.Clear();
            this.chrMeses.Series["Actual"].Points.Clear();
            this.chrMeses.Series["Pasado"].Points.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametrosCompras();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCompras_Result>("pauCuadroDeControlCompras", oParams);

            // Se llena el grid y la gráfica de Meses
            var oConsulta = oDatos.Where(c => c.ProveedorID == iId).GroupBy(g => g.Fecha.Valor().Month).Select(
                c => new { Mes = c.Key, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) }).OrderBy(c => c.Mes);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual).Valor() : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvMeses.Rows.Add(oReg.Mes, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.Mes).ToUpper(), oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                // Para la gráfica
                this.chrMeses.Series["Actual"].Points.AddXY(oReg.Mes, oReg.Actual);
                this.chrMeses.Series["Pasado"].Points.AddXY(oReg.Mes, oReg.Anterior);
            }
        }

        protected void LlenarPartesCompras(int iGridFuente, int iIdPrincipal, int iId)
        {
            this.dgvPartes.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametrosCompras();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCompras_Result>("pauCuadroDeControlCompras", oParams);

            var oConsulta = oDatos.Where(c => c.ProveedorID == iId);
            switch (iGridFuente)
            {
                case CuadroMultiple.GridFuente.Grupos:
                    foreach (var oReg in oConsulta)
                    {
                        var oDetV = Datos.GetListOf<MovimientoInventarioDetalleView>(c => c.MovimientoInventarioID == oReg.MovimientoInventarioID && c.LineaID == iId);
                        foreach (var oRegDet in oDetV)
                            this.dgvPartes.Rows.Add(oReg.Fecha, oReg.FolioFactura, oRegDet.NumeroParte, oRegDet.NombreParte
                                , (oRegDet.PrecioUnitarioConDescuento * oRegDet.Cantidad));
                    }
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
            }
                        
            // Se llena el grid
            foreach (var oReg in oConsulta)
            {
                var oDetV = Datos.GetListOf<MovimientoInventarioDetalleView>(c => c.MovimientoInventarioID == oReg.MovimientoInventarioID);
                foreach (var oRegDet in oDetV)
                    this.dgvPartes.Rows.Add(oReg.Fecha, oReg.FolioFactura, oRegDet.NumeroParte, oRegDet.NombreParte
                        , (oRegDet.PrecioUnitarioConDescuento * oRegDet.Cantidad));
            }
        }

        #endregion

        #region [Publicos]

        public void CargarDatos(int iProveedorID)
        {
            Cargando.Mostrar();

            this.dgvGrupos.Rows.Clear();
            this.dgvSemanas.Rows.Clear();
            this.dgvMeses.Rows.Clear();
            this.dgvVendedor.Rows.Clear();
            this.dgvSucursal.Rows.Clear();
            this.dgvPartes.Rows.Clear();

            this.LlenarGrupos(iProveedorID);
            this.LlenarSemanas(iProveedorID);
            this.LlenarMeses(iProveedorID);
            this.LLenarVendedores(iProveedorID);
            this.LlenarSucursales(iProveedorID);

            Cargando.Cerrar();
        }

        public void ReacomodarSinPrincipal()
        {
            this.txtBusqueda.Visible = false;
            this.dgvPrincipal.Visible = false;
            this.dgvPrincipalTotales.Visible = false;
            this.dgvVendedor.Top = 54;
            this.chrSemanas.Location = new Point(3, 360);
            this.chrSemanas.Width = 500;
            this.chrMeses.Location = new Point(3, 520);
            this.chrMeses.Width = 500;
            this.dgvPartes.Height = 125;
        }


        #endregion
    }
}
