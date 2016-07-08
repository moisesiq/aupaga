using System;
using System.Windows.Forms;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroLineas : CuadroMultiple
    {
        public CuadroLineas()
        {
            InitializeComponent();
            this.TipoDeReporte = CuadroMultiple.TiposDeReporte.Lineas;
        }

        #region [ Eventos ]

        private void CuadroLineas_Load(object sender, EventArgs e)
        {
            // Se configuran las columnas
            this.dgvPrincipal.Columns["Principal_Nombre"].HeaderText = "Línea";

            // Se manda a cargar los datos
            // this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oDatos = oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.MarcaID.Valor())).ToList();

            // Se llena el grid de líneas
            var oLineas = this.AgruparPorEnteroCadena(oDatos.GroupBy(g => new EnteroCadenaComp() { Entero = g.LineaID.Valor(), Cadena = g.Linea }))
                .OrderByDescending(c => c.Actual);
            
            decimal mTotal = (oLineas.Count() > 0 ? oLineas.Sum(c => c.Actual) : 0);
            this.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oLineas)
            {
                this.dgvPrincipal.Rows.Add(oReg.Llave, oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
            }
            // Se llenan los totales
            decimal mTotalAnt = (oLineas.Count() > 0 ? oLineas.Sum(c => c.Anterior) : 0);
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

            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oDatos = oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.MarcaID.Valor())).ToList();

            // Se llena el grid de clientes
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.LineaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.ClienteID, Cadena = g.Cliente }))
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

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oDatos = oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.MarcaID.Valor())).ToList();

            // Se llena el grid y la gráfica de Semanas
            IEnumerable<TotalesPorEntero> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.LineaID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));
            else
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ClienteID == iId).GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha)));

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

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oDatos = oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.MarcaID.Valor())).ToList();

            // Se llena el grid y la gráfica de Meses
            IEnumerable<TotalesPorEntero> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.LineaID == iId).GroupBy(g => g.Fecha.Month));
            else
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ClienteID == iId).GroupBy(g => g.Fecha.Month));

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

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oDatos = oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.MarcaID.Valor())).ToList();

            // Se llena el grid de Vendedor
            IEnumerable<TotalesPorEnteroCadena> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.LineaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.VendedorID, Cadena = g.Vendedor }));
            else
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ClienteID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.VendedorID, Cadena = g.Vendedor }));
            oConsulta = oConsulta.OrderByDescending(c => c.Actual);

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

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oDatos = oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.MarcaID.Valor())).ToList();

            // Se llena el grid de Sucursales
            IEnumerable<TotalesPorEnteroCadena> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.LineaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.SucursalID, Cadena = g.Sucursal }));
            else
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ClienteID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.SucursalID, Cadena = g.Sucursal }));

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

            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oDatos = oDatos.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.MarcaID.Valor())).ToList();

            var oConsulta = oDatos.Where(c => c.LineaID == iIdPrincipal && c.Fecha >= this.dtpDesde.Value.Date);
            switch (iGridFuente)
            {
                case CuadroMultiple.GridFuente.Grupos:
                    oConsulta = oConsulta.Where(c => c.ClienteID == iId);
                    break;
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
            this.LlenarDetalle(oConsulta, iIdPrincipal);
            /* foreach (var oReg in oConsulta)
            {
                var oVentasDetV = General.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID && c.LineaID == iIdPrincipal);

                foreach (var oDet in oVentasDetV)
                    this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte, ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
            }
            */
        }

        #endregion

    }
}
