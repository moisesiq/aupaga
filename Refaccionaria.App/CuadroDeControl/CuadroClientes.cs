using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CuadroClientes : CuadroMultiple
    {
        public CuadroClientes()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public int ClienteFijoID { get; set; }

        #endregion

        #region [ Eventos ]

        private void CuadroClientes_Load(object sender, EventArgs e)
        {
            // Se agregan y configuran columnas de cliente
            this.dgvPrincipal.Columns["Principal_Nombre"].HeaderText = "Razón Social";
            this.dgvPrincipal.Columns.Add("Principal_Tipo", "Tipo");
            var oVip = new DataGridViewCheckBoxColumn()
            {
                Name = "Principal_Vip",
                HeaderText = "VIP",
                Width = 40
            };
            this.dgvPrincipal.Columns.Add(oVip);

            // Se manda a cargar los datos
            // this.CargarDatos();

            // Se selecciona por predeterminado Ventas Mostrador
            // int iIndice = this.dgvPrincipal.EncontrarIndiceDeValor("Principal_Id", Cat.Clientes.Mostrador);
            // this.dgvPrincipal.CurrentCell = this.dgvPrincipal["Principal_Nombre", iIndice];
        }

        #endregion

        #region [ Métodos ]
        
        protected override void CargarDatos()
        {
            // Se verifica si está fijo ya el cliente
            if (this.ClienteFijoID > 0)
            {
                this.CargarDatos(this.ClienteFijoID);
                return;
            }

            Cargando.Mostrar();

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de clientes
            var oPorCliente = this.AgruparPorEntero(oDatos.GroupBy(g => g.ClienteID)).OrderByDescending(c => c.Actual);
            decimal mTotal = (oPorCliente.Count() > 0 ? oPorCliente.Sum(c => c.Actual) : 0);
            this.dgvPrincipal.Rows.Clear();
            foreach (var oReg in oPorCliente)
            {
                var oClienteV = General.GetEntity<ClientesView>(c => c.ClienteID == oReg.Llave);
                this.dgvPrincipal.Rows.Add(oReg.Llave, oReg.Llave, oClienteV.Nombre, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100)
                    , oClienteV.NombreTipoCliente, oClienteV.Vip);
            }
            
            // Se llenan los totales de clientes
            decimal mTotalAnt = (oPorCliente.Count() > 0 ? oPorCliente.Sum(c => c.Anterior) : 0);
            this.dgvPrincipalTotales["PrincipalT_Actual", 0].Value = mTotal;
            this.dgvPrincipalTotales["PrincipalT_Anterior", 0].Value = mTotalAnt;
            this.dgvPrincipalTotales["PrincipalT_Resultado", 0].Value = Helper.DividirONull(mTotal, mTotalAnt);
            
            // Para configurar las columnas de los grids
            base.CargarDatos();

            Cargando.Cerrar();
        }

        protected override void LlenarGrupos(int iClienteID)
        {
            this.dgvGrupos.Rows.Clear();
            if (iClienteID <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de clientes
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(
                g => new EnteroCadenaComp() { Entero = g.LineaID.Valor(), Cadena = g.Linea }))
                .OrderByDescending(c => c.Actual);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvGrupos.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected override void LlenarSemanas(int iClienteID)
        {
            this.dgvSemanas.Rows.Clear();
            this.chrSemanas.Series["Actual"].Points.Clear();
            this.chrSemanas.Series["Pasado"].Points.Clear();
            if (iClienteID <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica de Semanas
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(g => UtilLocal.SemanaSabAVie(g.Fecha)));
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

        protected override void LlenarMeses(int iClienteID)
        {
            this.dgvMeses.Rows.Clear();
            this.chrMeses.Series["Actual"].Points.Clear();
            this.chrMeses.Series["Pasado"].Points.Clear();
            if (iClienteID <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid y la gráfica de Meses
            var oConsulta = this.AgruparPorEntero(oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(g => g.Fecha.Month));
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
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ClienteID == iId).GroupBy(g => new EnteroCadenaComp() { Entero = g.VendedorID, Cadena = g.Vendedor }))
                .OrderByDescending(c => c.Actual);
            decimal mTotal = (oConsulta.Count() > 0 ? oConsulta.Sum(c => c.Actual) : 0);
            foreach (var oReg in oConsulta)
            {
                this.dgvVendedor.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, oReg.Anterior
                    , Helper.DividirONull(oReg.Actual, oReg.Anterior), (Helper.DividirONull(oReg.Actual, mTotal) * 100));
            }
        }

        protected override void LlenarSucursales(int iClienteID)
        {
            this.dgvSucursal.Rows.Clear();
            if (iClienteID <= 0)
                return;

            var oParams = this.ObtenerParametros();
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);

            // Se llena el grid de Sucursales
            var oConsulta = this.AgruparPorEnteroCadena(oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(
                g => new EnteroCadenaComp() { Entero = g.SucursalID, Cadena = g.Sucursal }));
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

            var oConsulta = oDatos.Where(c => c.ClienteID == iIdPrincipal && c.Fecha >= this.dtpDesde.Value.Date);
            switch (iGridFuente)
            {
                case CuadroMultiple.GridFuente.Grupos:
                    this.LlenarDetalle(oConsulta, iId);
                    /* foreach (var oReg in oConsulta)
                    {
                        var oVentasDetV = General.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID && c.LineaID == iId);
                        foreach (var oDet in oVentasDetV)
                            this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte
                                , ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
                    } */
                    return;
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
                var oVentasDetV = General.GetListOf<VentasDetalleView>(c => c.VentaID == oReg.VentaID);
                foreach (var oDet in oVentasDetV)
                    this.dgvPartes.Rows.Add(oReg.Fecha, oReg.Folio, oDet.NumeroParte, oDet.NombreParte, ((oDet.PrecioUnitario - oDet.CostoConDescuento) * oDet.Cantidad));
            } */
        }

        #endregion

        #region [ Públicos ]

        public void CargarDatos(int iClienteID)
        {
            Cargando.Mostrar();

            this.dgvGrupos.Rows.Clear();

            this.dgvSemanas.Rows.Clear();
            this.dgvMeses.Rows.Clear();
            this.dgvVendedor.Rows.Clear();
            this.dgvSucursal.Rows.Clear();
            this.dgvPartes.Rows.Clear();

            this.LlenarGrupos(iClienteID);
            this.LlenarSemanas(iClienteID);
            this.LlenarMeses(iClienteID);
            this.LLenarVendedores(iClienteID);
            this.LlenarSucursales(iClienteID);

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