using System;
using System.Windows.Forms;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroCancelaciones : CuadroMultiple
    {
        class Mostrar
        {
            public const int Importe = 0;
            public const int Documentos = 1;
            public const int Piezas = 2;
            public const int Indice = 3;
        }

        class Tipo
        {
            public const int Clientes = 0;
            public const int Vendedores = 1;
        }

        public CuadroCancelaciones()
        {
            InitializeComponent();
        }
                
        #region [ Eventos ]

        private void CuadroCancelaciones_Load(object sender, EventArgs e)
        {
            // Se configuran los 
            this.chkCostoConDescuento.Visible = true;
            this.lblCalculo.Visible = false;
            this.cmbCalculo.Visible = false;
            this.cmbMostrar.Items.AddRange(new object[] { "Importe", "Documentos", "Piezas", "Índice" });
            this.cmbMostrar.SelectedIndex = Mostrar.Importe;
            this.cmbTipo.Items.AddRange(new object[] { "Clientes", "Vendedores" });
            this.cmbTipo.SelectedIndex = Tipo.Clientes;

            // Se limpian las gráficas de motivos y tipos
            foreach (var oPunto in this.chrMotivo.Series["Motivo"].Points)
                oPunto.SetValueY(0);
            this.chrTipoDev.Series["NotaDeCredito"].Points[0].SetValueY(0);
            this.chrTipoDev.Series["Devolucion"].Points[0].SetValueY(0);

            // Se manda a cargar los datos
            // this.CargarDatos();
        }

        private void chkTodos_CheckedChanged(object sender, EventArgs e)
        {
            /* if (this.chkTodos.Focused)
            {
                this.dgvPrincipal.Focus();
                this.CargarDatos();
            }
            */
        }

        private void cmbMostrar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (this.cmbMostrar.Focused)
            //     this.CargarDatos();
        }

        private void cmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (this.cmbTipo.Focused)
            //     this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        protected override Dictionary<string, object> ObtenerParametros()
        {
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", this.chkPagadas.Checked);
            oParams.Add("Cobradas", this.chkCobradas.Checked);
            oParams.Add("Solo9500", this.chk9500.Checked);
            oParams.Add("OmitirDomingo", this.chkOmitirDomingos.Checked);
            oParams.Add("CostoConDescuento", this.chkCostoConDescuento.Checked);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            return oParams;
        }

        protected override void PrincipalCambioSel(bool bSelNueva)
        {
            if (bSelNueva)
            {
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Util.Entero(this.dgvPrincipal.CurrentRow.Cells["Principal_Id"].Value));
                this.LlenarGraficaMotivos(iId);
                this.LlenarGraficaTiposDev(iId);
            }
        }

        protected override void CargarDatos()
        {
            // Si es tipo Vendedores, se va a otro método
            if (this.cmbTipo.SelectedIndex == Tipo.Vendedores)
            {
                this.CargarDatosVendedores();
                return;
            }

            this.dgvPrincipal.Rows.Clear();

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);
            var oConsulta = oDatos.GroupBy(g => new { g.ClienteID, g.Cliente });

            // Se llena el grid principal, según el caso
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    var oImportes = oConsulta.Select(c => new { c.Key.ClienteID, c.Key.Cliente, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior)
                        , CantidadActual = c.Sum(s => s.CantidadActual), CantidadAnterior = c.Sum(s => s.CantidadAnterior) });

                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
                    decimal mTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Actual).Valor() : 0);
                    decimal mCantidadTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);

                    // Se llenan los datos
                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                            this.dgvPrincipal.Rows.Add(oReg.ClienteID, oReg.ClienteID, oReg.Cliente, oReg.Actual, oReg.Anterior
                                , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                        else
                            this.dgvPrincipal.Rows.Add(oReg.ClienteID, oReg.ClienteID, oReg.Cliente, oReg.CantidadActual, oReg.CantidadAnterior
                                , Util.DividirONull(oReg.CantidadActual, oReg.CantidadAnterior), (Util.DividirONull(oReg.CantidadActual, mCantidadTotal) * 100));
                    }
                    // Se llenan los totales
                    decimal mTotalAnt = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Anterior).Valor() : 0);
                    decimal mCantidadTotalAnt = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);
                    this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = (bPorImporte ? mTotal : mTotalAnt);
                    this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = (bPorImporte ? mTotalAnt : mCantidadTotal);
                    this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = (bPorImporte ? Util.DividirONull(mTotal, mTotalAnt) 
                        : Util.DividirONull(mCantidadTotal, mCantidadTotalAnt));
                    break;

                case Mostrar.Documentos:
                    // Se obtienen los datos
                    var oDoc = oConsulta.Select(c => new { c.Key.ClienteID, c.Key.Cliente
                            , VentasActual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1)
                            , VentasAnterior = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 2)
                    });
                    int iTotal = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasActual) : 0);
                    // Se llenan los datos
                    foreach (var oReg in oDoc)
                        this.dgvPrincipal.Rows.Add(oReg.ClienteID, oReg.ClienteID, oReg.Cliente, oReg.VentasActual, oReg.VentasAnterior
                            , Util.DividirONull(oReg.VentasActual, oReg.VentasAnterior), (Util.DividirONull(oReg.VentasActual, iTotal) * 100));
                    // Se llenan los totales
                    int iTotalAnt = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasAnterior) : 0);
                    this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = iTotal;
                    this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = iTotalAnt;
                    this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(iTotal, iTotalAnt);
                    break;

                case Mostrar.Indice:
                    // Se obtiene los datos, de ventas y de cancelaciones
                    var oDatosVentas = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
                    var oVentas = oDatosVentas.GroupBy(g => new { g.ClienteID, g.Cliente })
                        .Select(c => new { c.Key.ClienteID, c.Key.Cliente, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });
                    var oDevs = oConsulta.Select(c => new { c.Key.ClienteID, c.Key.Cliente, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });

                    // Se llenan los datos
                    decimal? mIndice, mIndiceAnt;
                    decimal? mTotalIndice = 0, mTotalIndiceAnt = 0;
                    foreach (var oReg in oDevs)
                    {
                        var oRegVenta = oVentas.SingleOrDefault(c => c.ClienteID == oReg.ClienteID);
                        if (oRegVenta == null) {
                            this.dgvPrincipal.Rows.Add(oReg.ClienteID, oReg.ClienteID, oReg.Cliente);
                        } else {
                            mIndice = Util.DividirONull(oReg.Actual, oRegVenta.Actual);
                            mIndiceAnt = Util.DividirONull(oReg.Anterior, oRegVenta.Anterior);
                            this.dgvPrincipal.Rows.Add(oReg.ClienteID, oReg.ClienteID, oReg.Cliente, mIndice, mIndiceAnt, Util.DividirONull(mIndice, mIndiceAnt));

                            mTotalIndice += mIndice;
                            mTotalIndiceAnt += mIndiceAnt;
                        }
                    }
                    // Se llenan los totales
                    this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotalIndice;
                    this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalIndiceAnt;
                    this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(mTotalIndice, mTotalIndiceAnt);
                    break;
            }

            // Se ajustan las columnas del los grids
            bool bColImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
            this.dgvPrincipal.Columns["Principal_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvPrincipal.Columns["Principal_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            // Se ordena el grid
            this.dgvPrincipal.Sort(this.dgvPrincipal.Columns["Principal_Actual"], ListSortDirection.Descending);
            // Se llenan los totales
            this.dgvPrincipalTotales.Columns["PrincipalT_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvPrincipalTotales.Columns["PrincipalT_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");

            Cargando.Cerrar();
        }

        private void LlenarGraficaMotivos(int iId)
        {
            // Se restablecen los valores de los puntos
            foreach (var oPunto in this.chrMotivo.Series["Motivo"].Points)
                oPunto.SetValueY(0);

            //
            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);

            // Se obtiene el grupo de datos, según el caso
            IEnumerable<pauCuadroDeControlCancelaciones_Result> oFiltro = oDatos;
            if (!this.chkTodos.Checked)
            {
                if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                    oFiltro = oDatos.Where(c => c.ClienteID == iId);
                else
                    oFiltro = oDatos.Where(c => c.VendedorID == iId);
            }
            var oConsulta = oFiltro.GroupBy(g => g.MotivoID);

            //
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);      
                    var oImportes = oConsulta.Select(c => new { MotivoID = c.Key, Importe = c.Sum(s => s.Actual), Cantidad = c.Sum(s => s.CantidadActual) });
                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                            this.chrMotivo.Series["Motivo"].Points[oReg.MotivoID - 1].SetValueY(oReg.Importe);
                        else
                            this.chrMotivo.Series["Motivo"].Points[oReg.MotivoID - 1].SetValueY(oReg.Cantidad);
                    }
                    break;
                case Mostrar.Documentos:
                    var oDoc = oConsulta.Select(c => new { MotivoID = c.Key, Actual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1) });
                    foreach (var oReg in oDoc)
                        this.chrMotivo.Series["Motivo"].Points[oReg.MotivoID - 1].SetValueY(oReg.Actual);
                    break;
            }

            this.chrMotivo.Refrescar();
        }

        private void LlenarGraficaTiposDev(int iId)
        {
            // Se restablecen los valores de los puntos
            this.chrTipoDev.Series["Devolucion"].Points[0].SetValueY(0);
            this.chrTipoDev.Series["NotaDeCredito"].Points[0].SetValueY(0);

            //
            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);

            // Se obtiene el grupo de datos, según el caso
            IEnumerable<pauCuadroDeControlCancelaciones_Result> oFiltro = oDatos;
            if (!this.chkTodos.Checked)
            {
                if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                    oFiltro = oDatos.Where(c => c.ClienteID == iId);
                else
                    oFiltro = oDatos.Where(c => c.VendedorID == iId);
            }
            var oConsulta = oFiltro.GroupBy(g => g.TipoFormaPagoID);

            //
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
                    var oImportes = oConsulta.Select(c => new { TipoID = c.Key, Importe = c.Sum(s => s.Actual), Cantidad = c.Sum(s => s.CantidadActual) });
                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                        {
                            if (oReg.TipoID == Cat.FormasDePago.Vale)
                                this.chrTipoDev.Series["NotaDeCredito"].Points[0].SetValueY(oReg.Importe);
                            else
                                this.chrTipoDev.Series["Devolucion"].Points[0].YValues[0] += (double)oReg.Importe;
                        }
                        else
                        {
                            if (oReg.TipoID == Cat.FormasDePago.Vale)
                                this.chrTipoDev.Series["NotaDeCredito"].Points[0].SetValueY(oReg.Cantidad);
                            else
                                this.chrTipoDev.Series["Devolucion"].Points[0].YValues[0] += (double)oReg.Cantidad;
                        }
                    }
                    break;
                case Mostrar.Documentos:
                    var oDoc = oConsulta.Select(c => new { TipoID = c.Key, Actual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1) });
                    foreach (var oReg in oDoc)
                    {
                        if (oReg.TipoID == Cat.FormasDePago.Vale)
                            this.chrTipoDev.Series["NotaDeCredito"].Points[0].SetValueY(oReg.Actual);
                        else
                            this.chrTipoDev.Series["Devolucion"].Points[0].YValues[0] += (double)oReg.Actual;
                    }
                    break;
            }

            // Se establecen los porcentajes, en la propiedad "AxisLabel"
            double mTotal = (this.chrTipoDev.Series["NotaDeCredito"].Points[0].YValues[0] + this.chrTipoDev.Series["Devolucion"].Points[0].YValues[0]);
            this.chrTipoDev.Series["NotaDeCredito"].Points[0].AxisLabel = string.Format("{0:N0}%", ((this.chrTipoDev.Series["NotaDeCredito"].Points[0].YValues[0] / mTotal) * 100));
            this.chrTipoDev.Series["Devolucion"].Points[0].AxisLabel = string.Format("{0:N0}%", ((this.chrTipoDev.Series["Devolucion"].Points[0].YValues[0] / mTotal) * 100));

            this.chrTipoDev.Refrescar();
        }

        protected override void LlenarGrupos(int iId)
        {
            this.dgvGrupos.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);

            // Se obtiene el grupo de datos, según el caso
            IEnumerable<pauCuadroDeControlCancelaciones_Result> oFiltro = oDatos;
            if (!this.chkTodos.Checked)
            {
                if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                    oFiltro = oDatos.Where(c => c.ClienteID == iId);
                else
                    oFiltro = oDatos.Where(c => c.VendedorID == iId);
            }
            var oConsulta = oFiltro.GroupBy(g => new { g.LineaID, g.Linea });

            // Se llena el grid principal, según el caso
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    var oImportes = oConsulta.Select(c => new { c.Key.LineaID, c.Key.Linea, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior)
                        , CantidadActual = c.Sum(s => s.CantidadActual), CantidadAnterior = c.Sum(s => s.CantidadAnterior) });

                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
                    decimal mTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Actual).Valor() : 0);
                    decimal mCantidadTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);

                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                            this.dgvGrupos.Rows.Add(oReg.LineaID, oReg.Linea, oReg.Actual, oReg.Anterior
                                , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                        else
                            this.dgvGrupos.Rows.Add(oReg.LineaID, oReg.Linea, oReg.CantidadActual, oReg.CantidadAnterior
                                , Util.DividirONull(oReg.CantidadActual, oReg.CantidadAnterior), (Util.DividirONull(oReg.CantidadActual, mCantidadTotal) * 100));
                    }
                    break;

                case Mostrar.Documentos:
                    // Se obtienen los datos
                    var oDoc = oConsulta.Select(c => new
                    {
                        c.Key.LineaID,
                        c.Key.Linea,
                        VentasActual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1),
                        VentasAnterior = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 2)
                    });
                    int iTotal = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasActual) : 0);
                    // Se llenan los datos
                    foreach (var oReg in oDoc)
                        this.dgvGrupos.Rows.Add(oReg.LineaID, oReg.Linea, oReg.VentasActual, oReg.VentasAnterior
                            , Util.DividirONull(oReg.VentasActual, oReg.VentasAnterior), (Util.DividirONull(oReg.VentasActual, iTotal) * 100));
                    break;

                case Mostrar.Indice:
                    // Se obtiene los datos, de ventas y de cancelaciones
                    var oDatosVentas = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
                    IEnumerable<pauCuadroDeControlGeneral_Result> oFiltroVen;
                    if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                        oFiltroVen = oDatosVentas.Where(c => c.ClienteID == iId);
                    else
                        oFiltroVen = oDatosVentas.Where(c => c.VendedorID == iId);
                    var oVentas = oFiltroVen.GroupBy(g => new { g.LineaID, g.Linea })
                        .Select(c => new { c.Key.LineaID, c.Key.Linea, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });
                    var oDevs = oConsulta.Select(c => new { c.Key.LineaID, c.Key.Linea, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });

                    // Se llenan los datos
                    decimal? mIndice, mIndiceAnt;
                    foreach (var oReg in oDevs)
                    {
                        var oRegVenta = oVentas.SingleOrDefault(c => c.LineaID == oReg.LineaID);
                        if (oRegVenta == null)
                        {
                            this.dgvGrupos.Rows.Add(oReg.LineaID, oReg.Linea);
                        }
                        else
                        {
                            mIndice = Util.DividirONull(oReg.Actual, oRegVenta.Actual);
                            mIndiceAnt = Util.DividirONull(oReg.Anterior, oRegVenta.Anterior);
                            this.dgvGrupos.Rows.Add(oReg.LineaID, oReg.Linea, mIndice, mIndiceAnt, Util.DividirONull(mIndice, mIndiceAnt));
                        }
                    }
                    break;
            }

            // Se ajustan las columnas del los grids
            bool bColImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
            this.dgvGrupos.Columns["Grupos_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvGrupos.Columns["Grupos_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            // Se ordena el grid
            this.dgvGrupos.Sort(this.dgvGrupos.Columns["Grupos_Actual"], ListSortDirection.Descending);
        }

        protected override void LlenarSemanas(int iId)
        {
            this.dgvSemanas.Rows.Clear();
            this.chrSemanas.Series["Actual"].Points.Clear();
            this.chrSemanas.Series["Pasado"].Points.Clear();
            if (iId <= 0)
                return;
                        
            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);

            // Se obtiene el grupo de datos, según el caso
            IEnumerable<pauCuadroDeControlCancelaciones_Result> oFiltro = oDatos;
            if (!this.chkTodos.Checked)
            {
                if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                    oFiltro = oDatos.Where(c => c.ClienteID == iId);
                else
                    oFiltro = oDatos.Where(c => c.VendedorID == iId);
            }
            var oConsulta = oFiltro.GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha));

            // Se llena el grid principal, según el caso
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    var oImportes = oConsulta.Select(c => new
                    {
                        Semana = c.Key,
                        Actual = c.Sum(s => s.Actual),
                        Anterior = c.Sum(s => s.Anterior),
                        CantidadActual = c.Sum(s => s.CantidadActual),
                        CantidadAnterior = c.Sum(s => s.CantidadAnterior)
                    });

                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
                    decimal mTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Actual).Valor() : 0);
                    decimal mCantidadTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);

                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                        {
                            this.dgvSemanas.Rows.Add(oReg.Semana, oReg.Semana, oReg.Actual, oReg.Anterior
                                , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                            // Para la gráfica
                            this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Semana, oReg.Actual);
                            this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Semana, oReg.Anterior);
                        }
                        else
                        {
                            this.dgvSemanas.Rows.Add(oReg.Semana, oReg.Semana, oReg.CantidadActual, oReg.CantidadAnterior
                                , Util.DividirONull(oReg.CantidadActual, oReg.CantidadAnterior), (Util.DividirONull(oReg.CantidadActual, mCantidadTotal) * 100));
                            // Para la gráfica
                            this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Semana, oReg.CantidadActual);
                            this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Semana, oReg.CantidadAnterior);
                        }
                    }
                    break;

                case Mostrar.Documentos:
                    // Se obtienen los datos
                    var oDoc = oConsulta.Select(c => new
                    {
                        Semana = c.Key,
                        VentasActual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1),
                        VentasAnterior = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 2)
                    });
                    int iTotal = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasActual) : 0);
                    // Se llenan los datos
                    foreach (var oReg in oDoc)
                    {
                        this.dgvSemanas.Rows.Add(oReg.Semana, oReg.Semana, oReg.VentasActual, oReg.VentasAnterior
                            , Util.DividirONull(oReg.VentasActual, oReg.VentasAnterior), (Util.DividirONull(oReg.VentasActual, iTotal) * 100));
                        // Para la gráfica
                        this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Semana, oReg.VentasActual);
                        this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Semana, oReg.VentasAnterior);
                    }
                    break;

                case Mostrar.Indice:
                    // Se obtiene los datos, de ventas y de cancelaciones
                    var oDatosVentas = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
                    IEnumerable<pauCuadroDeControlGeneral_Result> oFiltroVen;
                    if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                        oFiltroVen = oDatosVentas.Where(c => c.ClienteID == iId);
                    else
                        oFiltroVen = oDatosVentas.Where(c => c.VendedorID == iId);
                    var oVentas = oFiltroVen.GroupBy(g => UtilTheos.SemanaSabAVie(g.Fecha))
                        .Select(c => new { Semana = c.Key, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });
                    var oDevs = oConsulta.Select(c => new { Semana = c.Key, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });
                    
                    // Se llenan los datos
                    decimal? mIndice, mIndiceAnt;
                    foreach (var oReg in oDevs)
                    {
                        var oRegVenta = oVentas.SingleOrDefault(c => c.Semana == oReg.Semana);
                        if (oRegVenta == null)
                        {
                            this.dgvSemanas.Rows.Add(oReg.Semana, oReg.Semana);
                            // Para la gráfica
                            this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Semana, 0);
                            this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Semana, 0);
                        }
                        else
                        {
                            mIndice = Util.DividirONull(oReg.Actual, oRegVenta.Actual);
                            mIndiceAnt = Util.DividirONull(oReg.Anterior, oRegVenta.Anterior);
                            this.dgvSemanas.Rows.Add(oReg.Semana, oReg.Semana, mIndice, mIndiceAnt, Util.DividirONull(mIndice, mIndiceAnt));
                            // Para la gráfica
                            this.chrSemanas.Series["Actual"].Points.AddXY(oReg.Semana, mIndice);
                            this.chrSemanas.Series["Pasado"].Points.AddXY(oReg.Semana, mIndiceAnt);
                        }
                    }
                    break;
            }

            // Se ajustan las columnas del los grids
            bool bColImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
            this.dgvSemanas.Columns["Semanas_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvSemanas.Columns["Semanas_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            // Se ordena el grid
            this.dgvSemanas.Sort(this.dgvSemanas.Columns["Semanas_Id"], ListSortDirection.Ascending);
        }

        protected override void LlenarMeses(int iId)
        {
            this.dgvMeses.Rows.Clear();
            this.chrMeses.Series["Actual"].Points.Clear();
            this.chrMeses.Series["Pasado"].Points.Clear();
            if (iId <= 0)
                return;
 
            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);

            // Se obtiene el grupo de datos, según el caso
            IEnumerable<pauCuadroDeControlCancelaciones_Result> oFiltro = oDatos;
            if (!this.chkTodos.Checked)
            {
                if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                    oFiltro = oDatos.Where(c => c.ClienteID == iId);
                else
                    oFiltro = oDatos.Where(c => c.VendedorID == iId);
            }
            var oConsulta = oFiltro.GroupBy(g => g.Fecha.Month);

            // Se llena el grid principal, según el caso
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    var oImportes = oConsulta.Select(c => new
                    {
                        Mes = c.Key,
                        Actual = c.Sum(s => s.Actual),
                        Anterior = c.Sum(s => s.Anterior),
                        CantidadActual = c.Sum(s => s.CantidadActual),
                        CantidadAnterior = c.Sum(s => s.CantidadAnterior)
                    });

                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
                    decimal mTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Actual).Valor() : 0);
                    decimal mCantidadTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);

                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                        {
                            this.dgvMeses.Rows.Add(oReg.Mes, oReg.Mes, oReg.Actual, oReg.Anterior
                                , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                            // Para la gráfica
                            this.chrMeses.Series["Actual"].Points.AddXY(oReg.Mes, oReg.Actual);
                            this.chrMeses.Series["Pasado"].Points.AddXY(oReg.Mes, oReg.Anterior);
                        }
                        else
                        {
                            this.dgvMeses.Rows.Add(oReg.Mes, oReg.Mes, oReg.CantidadActual, oReg.CantidadAnterior
                                , Util.DividirONull(oReg.CantidadActual, oReg.CantidadAnterior), (Util.DividirONull(oReg.CantidadActual, mCantidadTotal) * 100));
                            // Para la gráfica
                            this.chrMeses.Series["Actual"].Points.AddXY(oReg.Mes, oReg.CantidadActual);
                            this.chrMeses.Series["Pasado"].Points.AddXY(oReg.Mes, oReg.CantidadAnterior);
                        }
                    }
                    break;

                case Mostrar.Documentos:
                    // Se obtienen los datos
                    var oDoc = oConsulta.Select(c => new
                    {
                        Mes = c.Key,
                        VentasActual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1),
                        VentasAnterior = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 2)
                    });
                    int iTotal = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasActual) : 0);
                    // Se llenan los datos
                    foreach (var oReg in oDoc)
                    {
                        this.dgvMeses.Rows.Add(oReg.Mes, oReg.Mes, oReg.VentasActual, oReg.VentasAnterior
                            , Util.DividirONull(oReg.VentasActual, oReg.VentasAnterior), (Util.DividirONull(oReg.VentasActual, iTotal) * 100));
                        // Para la gráfica
                        this.chrMeses.Series["Actual"].Points.AddXY(oReg.Mes, oReg.VentasActual);
                        this.chrMeses.Series["Pasado"].Points.AddXY(oReg.Mes, oReg.VentasAnterior);
                    }
                    break;

                case Mostrar.Indice:
                    // Se obtiene los datos, de ventas y de cancelaciones
                    var oDatosVentas = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
                    IEnumerable<pauCuadroDeControlGeneral_Result> oFiltroVen;
                    if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                        oFiltroVen = oDatosVentas.Where(c => c.ClienteID == iId);
                    else
                        oFiltroVen = oDatosVentas.Where(c => c.VendedorID == iId);
                    var oVentas = oFiltroVen.GroupBy(g => g.Fecha.Month)
                        .Select(c => new { Mes = c.Key, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });
                    var oDevs = oConsulta.Select(c => new { Mes = c.Key, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });

                    // Se llenan los datos
                    decimal? mIndice, mIndiceAnt;
                    foreach (var oReg in oDevs)
                    {
                        var oRegVenta = oVentas.SingleOrDefault(c => c.Mes == oReg.Mes);
                        if (oRegVenta == null)
                        {
                            this.dgvMeses.Rows.Add(oReg.Mes, oReg.Mes);
                            // Para la gráfica
                            this.chrMeses.Series["Actual"].Points.AddXY(oReg.Mes, 0);
                            this.chrMeses.Series["Pasado"].Points.AddXY(oReg.Mes, 0);
                        }
                        else
                        {
                            mIndice = Util.DividirONull(oReg.Actual, oRegVenta.Actual);
                            mIndiceAnt = Util.DividirONull(oReg.Anterior, oRegVenta.Anterior);
                            this.dgvMeses.Rows.Add(oReg.Mes, oReg.Mes, mIndice, mIndiceAnt, Util.DividirONull(mIndice, mIndiceAnt));
                            // Para la gráfica
                            this.chrMeses.Series["Actual"].Points.AddXY(oReg.Mes, mIndice);
                            this.chrMeses.Series["Pasado"].Points.AddXY(oReg.Mes, mIndiceAnt);
                        }
                    }
                    break;
            }

            // Se ajustan las columnas del los grids
            bool bColImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
            this.dgvMeses.Columns["Meses_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvMeses.Columns["Meses_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            // Se ordena el grid
            this.dgvMeses.Sort(this.dgvMeses.Columns["Meses_Id"], ListSortDirection.Ascending);
        }

        protected override void LlenarSucursales(int iId)
        {
            this.dgvSucursal.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);

            // Se obtiene el grupo de datos, según el caso
            IEnumerable<pauCuadroDeControlCancelaciones_Result> oFiltro = oDatos;
            if (!this.chkTodos.Checked)
            {
                if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                    oFiltro = oDatos.Where(c => c.ClienteID == iId);
                else
                    oFiltro = oDatos.Where(c => c.VendedorID == iId);
            }
            var oConsulta = oFiltro.GroupBy(g => new { g.SucursalID, g.Sucursal });

            // Se llena el grid principal, según el caso
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    var oImportes = oConsulta.Select(c => new
                    {
                        c.Key.SucursalID,
                        c.Key.Sucursal,
                        Actual = c.Sum(s => s.Actual),
                        Anterior = c.Sum(s => s.Anterior),
                        CantidadActual = c.Sum(s => s.CantidadActual),
                        CantidadAnterior = c.Sum(s => s.CantidadAnterior)
                    });

                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
                    decimal mTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Actual).Valor() : 0);
                    decimal mCantidadTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);

                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                            this.dgvSucursal.Rows.Add(oReg.SucursalID, oReg.Sucursal, oReg.Actual, oReg.Anterior
                                , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                        else
                            this.dgvSucursal.Rows.Add(oReg.SucursalID, oReg.Sucursal, oReg.CantidadActual, oReg.CantidadAnterior
                                , Util.DividirONull(oReg.CantidadActual, oReg.CantidadAnterior), (Util.DividirONull(oReg.CantidadActual, mCantidadTotal) * 100));
                    }
                    break;

                case Mostrar.Documentos:
                    // Se obtienen los datos
                    var oDoc = oConsulta.Select(c => new
                    {
                        c.Key.SucursalID,
                        c.Key.Sucursal,
                        VentasActual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1),
                        VentasAnterior = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 2)
                    });
                    int iTotal = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasActual) : 0);
                    // Se llenan los datos
                    foreach (var oReg in oDoc)
                        this.dgvSucursal.Rows.Add(oReg.SucursalID, oReg.Sucursal, oReg.VentasActual, oReg.VentasAnterior
                            , Util.DividirONull(oReg.VentasActual, oReg.VentasAnterior), (Util.DividirONull(oReg.VentasActual, iTotal) * 100));
                    break;

                case Mostrar.Indice:
                    // Se obtiene los datos, de ventas y de cancelaciones
                    var oDatosVentas = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
                    IEnumerable<pauCuadroDeControlGeneral_Result> oFiltroVen;
                    if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                        oFiltroVen = oDatosVentas.Where(c => c.ClienteID == iId);
                    else
                        oFiltroVen = oDatosVentas.Where(c => c.VendedorID == iId);
                    var oVentas = oFiltroVen.GroupBy(g => new { g.SucursalID, g.Sucursal })
                        .Select(c => new { c.Key.SucursalID, c.Key.Sucursal, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });
                    var oDevs = oConsulta.Select(c => new { c.Key.SucursalID, c.Key.Sucursal, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });

                    // Se llenan los datos
                    decimal? mIndice, mIndiceAnt;
                    foreach (var oReg in oDevs)
                    {
                        var oRegVenta = oVentas.SingleOrDefault(c => c.SucursalID == oReg.SucursalID);
                        if (oRegVenta == null)
                        {
                            this.dgvSucursal.Rows.Add(oReg.SucursalID, oReg.Sucursal);
                        }
                        else
                        {
                            mIndice = Util.DividirONull(oReg.Actual, oRegVenta.Actual);
                            mIndiceAnt = Util.DividirONull(oReg.Anterior, oRegVenta.Anterior);
                            this.dgvSucursal.Rows.Add(oReg.SucursalID, oReg.Sucursal, mIndice, mIndiceAnt, Util.DividirONull(mIndice, mIndiceAnt));
                        }
                    }
                    break;
            }

            // Se ajustan las columnas del los grids
            bool bColImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
            this.dgvSucursal.Columns["Sucursal_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvSucursal.Columns["Sucursal_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            // Se ordena el grid
            this.dgvSucursal.Sort(this.dgvSucursal.Columns["Sucursal_Id"], ListSortDirection.Ascending);
        }

        protected override void LlenarPartes(int iGridFuente, int iIdPrincipal, int iId)
        {
            this.dgvPartes.Rows.Clear();
            if (iId <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);

            // Se obtiene el grupo de datos, según el caso
            IEnumerable<pauCuadroDeControlCancelaciones_Result> oConsulta = oDatos;
            if (!this.chkTodos.Checked)
            {
                if (this.cmbTipo.SelectedIndex == Tipo.Clientes)
                    oConsulta = oDatos.Where(c => c.ClienteID == iIdPrincipal);
                else
                    oConsulta = oDatos.Where(c => c.VendedorID == iIdPrincipal);
            }
            
            //
            bool bLlenarGrid = false;
            switch (iGridFuente)
            {
                case CuadroMultiple.GridFuente.Grupos:
                    this.LlenarDetalle(oConsulta, iId);
                    /* foreach (var oReg in oConsulta)
                    {
                        var oVentasDetV = (General.GetListOf<VentasDevolucionesDetalleView>(c => c.VentaDevolucionID == oReg.VentaDevolucionID && c.LineaID == iId));
                        foreach (var oDet in oVentasDetV)
                            this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte
                                , ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
                    }
                    */
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

                    // Se llenan los datos
                    oConsulta = oConsulta.Where(c => (c.Fecha >= dIni && c.Fecha < dFin));
                    bLlenarGrid = true;
                    break;
                case CuadroMultiple.GridFuente.Vendedores:
                    oConsulta = oConsulta.Where(c => c.VendedorID == iId);
                    bLlenarGrid = true;
                    break;
                case CuadroMultiple.GridFuente.Sucursales:
                    oConsulta = oConsulta.Where(c => c.SucursalID == iId);
                    bLlenarGrid = true;
                    break;
            }

            // Se llena el grid, si aplica
            if (bLlenarGrid)
                this.LlenarDetalle(oConsulta, null);
            /* if (bLlenarGrid)
            {
                foreach (var oReg in oConsulta)
                {
                    var oVentasDetV = General.GetListOf<VentasDevolucionesDetalleView>(c => c.VentaDevolucionID == oReg.VentaDevolucionID);
                    foreach (var oDet in oVentasDetV)
                        this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte, ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
                }
            }
            */
        }

        private void LlenarDetalle(IEnumerable<pauCuadroDeControlCancelaciones_Result> oDatos, int? iLineaID)
        {
            List<int> oVentasProc = new List<int>();

            foreach (var oReg in oDatos)
            {
                if (oVentasProc.Contains(oReg.VentaDevolucionID)) continue;
                oVentasProc.Add(oReg.VentaDevolucionID);

                var oVentasDetV = Datos.GetListOf<VentasDevolucionesDetalleView>(c => c.VentaDevolucionID == oReg.VentaDevolucionID && (iLineaID == null || c.LineaID == iLineaID));
                foreach (var oDet in oVentasDetV)
                    this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte
                        , ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
            }
        }

        #endregion

        #region [ Tipo Vendedores ]

        protected void CargarDatosVendedores()
        {
            this.dgvPrincipal.Rows.Clear();

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlCancelaciones_Result>("pauCuadroDeControlCancelaciones", oParams);
            var oConsulta = oDatos.GroupBy(g => new { g.VendedorID, g.Vendedor });

            // Se llena el grid principal, según el caso
            switch (this.cmbMostrar.SelectedIndex)
            {
                case Mostrar.Importe:
                case Mostrar.Piezas:
                    var oImportes = oConsulta.Select(c => new
                    {
                        c.Key.VendedorID,
                        c.Key.Vendedor,
                        Actual = c.Sum(s => s.Actual),
                        Anterior = c.Sum(s => s.Anterior),
                        CantidadActual = c.Sum(s => s.CantidadActual),
                        CantidadAnterior = c.Sum(s => s.CantidadAnterior)
                    });

                    bool bPorImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
                    decimal mTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Actual).Valor() : 0);
                    decimal mCantidadTotal = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);

                    foreach (var oReg in oImportes)
                    {
                        if (bPorImporte)
                            this.dgvPrincipal.Rows.Add(oReg.VendedorID, oReg.VendedorID, oReg.Vendedor, oReg.Actual, oReg.Anterior
                                , Util.DividirONull(oReg.Actual, oReg.Anterior), (Util.DividirONull(oReg.Actual, mTotal) * 100));
                        else
                            this.dgvPrincipal.Rows.Add(oReg.VendedorID, oReg.VendedorID, oReg.Vendedor, oReg.CantidadActual, oReg.CantidadAnterior
                                , Util.DividirONull(oReg.CantidadActual, oReg.CantidadAnterior), (Util.DividirONull(oReg.CantidadActual, mCantidadTotal) * 100));
                    }
                    // Se llenan los totales
                    decimal mTotalAnt = (oImportes.Count() > 0 ? oImportes.Sum(c => c.Anterior).Valor() : 0);
                    decimal mCantidadTotalAnt = (oImportes.Count() > 0 ? oImportes.Sum(c => c.CantidadActual).Valor() : 0);
                    this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = (bPorImporte ? mTotal : mTotalAnt);
                    this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = (bPorImporte ? mTotalAnt : mCantidadTotal);
                    this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = (bPorImporte ? Util.DividirONull(mTotal, mTotalAnt)
                        : Util.DividirONull(mCantidadTotal, mCantidadTotalAnt));
                    break;

                case Mostrar.Documentos:
                    // Se obtienen los datos
                    var oDoc = oConsulta.Select(c => new
                    {
                        c.Key.VendedorID,
                        c.Key.Vendedor,
                        VentasActual = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 1),
                        VentasAnterior = c.Select(s => new { s.VentaID, s.ActualAnt }).Distinct().Count(s => s.ActualAnt == 2)
                    });
                    int iTotal = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasActual) : 0);
                    // Se llenan los datos
                    foreach (var oReg in oDoc)
                        this.dgvPrincipal.Rows.Add(oReg.VendedorID, oReg.VendedorID, oReg.Vendedor, oReg.VentasActual, oReg.VentasAnterior
                            , Util.DividirONull(oReg.VentasActual, oReg.VentasAnterior), (Util.DividirONull(oReg.VentasActual, iTotal) * 100));
                    // Se llenan los totales
                    int iTotalAnt = (oDoc.Count() > 0 ? oDoc.Sum(c => c.VentasAnterior) : 0);
                    this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = iTotal;
                    this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = iTotalAnt;
                    this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(iTotal, iTotalAnt);
                    break;

                case Mostrar.Indice:
                    // Se obtiene los datos, de ventas y de cancelaciones
                    var oDatosVentas = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
                    var oVentas = oDatosVentas.GroupBy(g => new { g.VendedorID, g.Vendedor })
                        .Select(c => new { c.Key.VendedorID, c.Key.Vendedor, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });
                    var oDevs = oConsulta.Select(c => new { c.Key.VendedorID, c.Key.Vendedor, Actual = c.Sum(s => s.Actual), Anterior = c.Sum(s => s.Anterior) });

                    // Se llenan los datos
                    decimal? mIndice, mIndiceAnt;
                    decimal? mTotalIndice = 0, mTotalIndiceAnt = 0;
                    foreach (var oReg in oDevs)
                    {
                        var oRegVenta = oVentas.SingleOrDefault(c => c.VendedorID == oReg.VendedorID);
                        if (oRegVenta == null)
                        {
                            this.dgvPrincipal.Rows.Add(oReg.VendedorID, oReg.VendedorID, oReg.Vendedor);
                        }
                        else
                        {
                            mIndice = Util.DividirONull(oReg.Actual, oRegVenta.Actual);
                            mIndiceAnt = Util.DividirONull(oReg.Anterior, oRegVenta.Anterior);
                            this.dgvPrincipal.Rows.Add(oReg.VendedorID, oReg.VendedorID, oReg.Vendedor, mIndice, mIndiceAnt, Util.DividirONull(mIndice, mIndiceAnt));

                            mTotalIndice += mIndice;
                            mTotalIndiceAnt += mIndiceAnt;
                        }
                    }
                    // Se llenan los totales
                    this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotalIndice;
                    this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalIndiceAnt;
                    this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Util.DividirONull(mTotalIndice, mTotalIndiceAnt);
                    break;
            }

            // Se ajustan las columnas del los grids
            bool bColImporte = (this.cmbMostrar.SelectedIndex == Mostrar.Importe);
            this.dgvPrincipal.Columns["Principal_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvPrincipal.Columns["Principal_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvPrincipalTotales.Columns["PrincipalT_Actual"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");
            this.dgvPrincipalTotales.Columns["PrincipalT_Anterior"].DefaultCellStyle.Format = (bColImporte ? "C2" : "N2");

            Cargando.Cerrar();
        }

        #endregion

    }
}
