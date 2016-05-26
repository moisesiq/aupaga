using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using System.Globalization;

namespace Refaccionaria.App
{
    public partial class CuadroMarcas : CuadroMultiple
    {
        public CuadroMarcas()
        {
            InitializeComponent();
            this.TipoDeReporte = CuadroMultiple.TiposDeReporte.Marcas;
        }
                
        #region [ Eventos ]

        private void CuadroMarcas_Load(object sender, EventArgs e)
        {
            // Se configuran las columnas
            this.dgvPrincipal.Columns["Principal_Nombre"].HeaderText = "Marca";
        }

        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid principal
            var oMarcas = this.AgruparPorEnteroCadena(oDatos.GroupBy(g => new EnteroCadenaComp() { Entero = g.MarcaID.Valor(), Cadena = g.Marca }))
                .OrderByDescending(c => c.Actual);
            // Se filtran según el combo de marcas
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
                oMarcas = oMarcas.Where(c => this.ctlMarcas.ValoresSeleccionados.Contains(c.Llave)).OrderBy(c => c.Llave);
            //
            decimal mTotal = (oMarcas.Count() > 0 ? oMarcas.Sum(c => c.Actual) : 0);
            this.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oMarcas)
            {
                this.dgvPrincipal.Rows.Add(oReg.Llave, oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
            }
            // Se llenan los totales
            decimal mTotalAnt = (oMarcas.Count() > 0 ? oMarcas.Sum(c => c.Anterior) : 0);
            this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotal;
            this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalAnt;
            this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Helper.DividirONull(mTotal, mTotalAnt);

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
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de líneas
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.MarcaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.LineaID.Valor(), Cadena = g.Linea }))
                .OrderByDescending(c => c.Actual);
            // Se filtran según el combo de líneas
            if (this.ctlLineas.ValoresSeleccionados.Count > 0)
                oConsulta = oConsulta.Where(c => this.ctlLineas.ValoresSeleccionados.Contains(c.Llave)).OrderBy(c => c.Llave);
            //
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvGrupos.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
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
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica de Semanas
            IEnumerable<TotalesPorEntero> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.MarcaID == iId).GroupBy(g => UtilLocal.SemanaSabAVie(g.Fecha)));
            else
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.LineaID == iId).GroupBy(g => UtilLocal.SemanaSabAVie(g.Fecha)));


            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvSemanas.Rows.Add(oReg.Llave, oReg.Llave, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
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
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica 
            IEnumerable<TotalesPorEntero> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.MarcaID == iId).GroupBy(g => g.Fecha.Month));
            else
                oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.LineaID == iId).GroupBy(g => g.Fecha.Month));

            //
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvMeses.Rows.Add(oReg.Llave, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(oReg.Llave).ToUpper(), oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
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
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de Vendedor
            IEnumerable<TotalesPorEnteroCadena> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.MarcaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.VendedorID, Cadena = g.Vendedor }));
            else
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.LineaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.VendedorID, Cadena = g.Vendedor }));
            oConsulta = oConsulta.OrderByDescending(c => c.Actual);
            
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvVendedor.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected override void LlenarSucursales(int iId)
        {
            this.dgvSucursal.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de Sucursales
            IEnumerable<TotalesPorEnteroCadena> oConsulta;
            if (this.ActiveControl == this.dgvPrincipal)
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.MarcaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.SucursalID, Cadena = g.Sucursal }));
            else
                oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.LineaID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.SucursalID, Cadena = g.Sucursal }));
            
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvSucursal.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected override void LlenarPartes(int iGridFuente, int iIdPrincipal, int iId)
        {
            this.dgvPartes.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            var oConsulta = oDatos.Where(c => c.MarcaID == iIdPrincipal && c.Fecha >= this.dtpDesde.Value.Date);
            switch (iGridFuente)
            {
                case CuadroMultiple.GridFuente.Grupos:
                    oConsulta = oConsulta.Where(c => c.LineaID == iId);
                    break;
                case CuadroMultiple.GridFuente.Meses:
                case CuadroMultiple.GridFuente.Semanas:
                    DateTime dIni, dFin;
                    if (iGridFuente == CuadroMultiple.GridFuente.Semanas)
                    {
                        dIni = UtilLocal.InicioSemanaSabAVie(this.dtpDesde.Value.Year, iId);
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
                var oVentasDetV = General.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID && c.LineaID == iIdPrincipal);

                foreach (var oDet in oVentasDetV)
                    this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte, ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
            }
            */
        }

        #endregion
    }
}
