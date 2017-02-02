using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;

using AdvancedDataGridView;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroVip : UserControl
    {
        List<pauCuadroDeControlGeneral_Result> oDatos;
        List<pauCuadroDeControlPorAnio_Result> oDatosPorAnio;
        List<pauCuadroDeControlPartes_Result> oDatosPartes;

        public CuadroVip()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroVip_Load(object sender, EventArgs e)
        {
            CuadroControlPermisos PermisosC = new CuadroControlPermisos();
            // Se llenan los tipos de cálculo
            //this.cmbCalculo.Items.AddRange(new object[] { "Utilidad", "Utilidad Desc.", "Precio", "Costo", "Costo Desc.", "Ventas", "Productos" });
            this.cmbCalculo.Items.AddRange(PermisosC.ValidarPermisosCalculoCuadroMultiple(CuadroControlPermisos.GetTabPage).ToArray());
            //this.cmbCalculo.SelectedIndex = 1;
            this.cmbCalculo.SelectedIndex = 0;


            var oSucursales = PermisosC.ValidarPermisosTiendaCuadroMultiple(CuadroControlPermisos.GetTabPage);


            if (oSucursales.Count() > 2)
            {
                oSucursales.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "Todas" });
            }

            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", oSucursales);
            this.cmbSucursal.SelectedValue = oSucursales.ElementAt(0).SucursalID;

            //// Se llenan las Sucursales
            //var oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            //oSucursales.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "Todas" });
            //this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", oSucursales);
            ////
            //this.cmbSucursal.SelectedValue = 0;

            this.chkPagadas.Checked = true;
            this.dtpDesde.Value = DateTime.Now.AddYears(-1).AddDays(1);
            this.nudAcumuladoMostrar.Value = 80;
        }

        private void nudDecimales_ValueChanged(object sender, EventArgs e)
        {
            if (this.nudDecimales.Focused)
                this.AplicarFormatoColumnas();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarClientes();
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.dgvClientes.EncontrarContiene(this.txtBusqueda.Text, "cli_Cliente");
        }

        private void dgvClientes_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvClientes.Focused && this.dgvClientes.CurrentRow != null && this.dgvClientes.VerSeleccionNueva())
            {
                Cargando.Mostrar();
                int iClienteID = Util.Entero(this.dgvClientes.CurrentRow.Cells["cli_ClienteID"].Value);
                this.CargarSemanas(iClienteID);
                this.CargarMeses(iClienteID);
                this.CargarSucursales(iClienteID);
                this.CargarVendedores(iClienteID);
                this.CargarAnios(iClienteID);
                this.CargarArbolPartes(iClienteID);
                this.AplicarFormatoColumnas();
                Cargando.Cerrar();
            }
        }

        #endregion

        #region [ Métodos ]

        protected virtual Dictionary<string, object> ObtenerParametros()
        {
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", this.chkPagadas.Checked);
            oParams.Add("Cobradas", this.chkCobradas.Checked);
            oParams.Add("Solo9500", this.chk9500.Checked);
            oParams.Add("OmitirDomingo", this.chkOmitirDomingos.Checked);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            return oParams;
        }

        private void AplicarFormatoColumnas()
        {
            string sFormato = ((this.cmbCalculo.Text == "Ventas" || this.cmbCalculo.Text == "Productos") ? "N" : "C");
            sFormato += Util.Cadena((int)this.nudDecimales.Value);
            this.dgvClientes.Columns["cli_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanas.Columns["Semana_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvMeses.Columns["Meses_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSucursales.Columns["Sucursal_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvVendedores.Columns["Vendedor_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvPorAnio.Columns["Anio_Actual"].DefaultCellStyle.Format = sFormato;
            this.txtPromedioSemanas.Text = Util.Decimal(this.txtPromedioSemanas.Text).ToString(sFormato);
            this.txtPromedioMeses.Text = Util.Decimal(this.txtPromedioMeses.Text).ToString(sFormato);
            this.txtPromedioAnios.Text = Util.Decimal(this.txtPromedioAnios.Text).ToString(sFormato);
        }

        private void CargarClientes()
        {
            Cargando.Mostrar();

            // Se cargan los datos base
            var oParams = this.ObtenerParametros();
            this.oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            // Se obtienen los datos para partes
            this.oDatosPartes = Datos.ExecuteProcedure<pauCuadroDeControlPartes_Result>("pauCuadroDeControlPartes", oParams);
            // Se obtienen los datos por año
            oParams.Remove("Desde");
            oParams.Remove("Hasta");
            this.oDatosPorAnio = Datos.ExecuteProcedure<pauCuadroDeControlPorAnio_Result>("pauCuadroDeControlPorAnio", oParams);
            // Se obtienen los datos agrupados por clientes y semanas, para marcar en rojo
            var oCliSem = this.ObtenerClientesSemanas(this.oDatos).OrderByDescending(c => c.Semana).ToList();
            var oCliProm = oCliSem.GroupBy(c => c.ClienteID).Select(c => new { ClienteID = c.Key, Promedio = c.Average(s => s.Actual) }).ToList();

            // Se llena el grid de clientes
            var oPorCliente = this.AgruparPorEnteroCadena(this.oDatos.GroupBy(g => new EnteroCadenaAgrupar() { Entero = g.ClienteID, Cadena = g.Cliente }))
                .OrderByDescending(c => c.Actual);
            decimal mTotal = (oPorCliente.Count() > 0 ? oPorCliente.Sum(c => c.Actual) : 0);
            this.dgvClientes.Rows.Clear();
            foreach (var oReg in oPorCliente)
            {
                int iFila = this.dgvClientes.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, Util.DividirONull(oReg.Actual, oReg.Anterior)
                    , Util.DividirONull(oReg.Actual, mTotal));

                // Se obtiene un el dato de si tiene más de 2 semanas abajo del promedio, para marcarlo en rojo
                decimal mPromedio = oCliProm.FirstOrDefault(c => c.ClienteID == oReg.Llave).Promedio;
                if (oCliSem.Where(c => c.ClienteID == oReg.Llave).Take(3).Any(c => c.Actual < mPromedio))
                    this.dgvClientes.Rows[iFila].DefaultCellStyle.ForeColor = Color.Black;
            }
            int iTotalClientes = this.dgvClientes.Rows.Count;

            // Se llena el dato de acumulado
            decimal mPorAcum = 0;
            foreach (DataGridViewRow oFila in this.dgvClientes.Rows)
            {
                mPorAcum += Util.Decimal(oFila.Cells["cli_Porcentaje"].Value);
                oFila.Cells["cli_Acumulado"].Value = mPorAcum;
                // Se valida si se pasa del máximo, para ocultarlo
                if ((mPorAcum * 100) > this.nudAcumuladoMostrar.Value)
                {
                    oFila.Visible = false;
                    iTotalClientes--;
                }
            }

            // Para configurar las columnas de los grids
            this.lblClientes.Text = iTotalClientes.ToString(Con.Formatos.Entero);
            this.AplicarFormatoColumnas();

            Cargando.Cerrar();
        }

        private void CargarSemanas(int iClienteID)
        {
            if (this.oDatos == null)
                return;

            var oSemanas = this.AgruparPorEntero(this.oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(c => UtilTheos.SemanaSabAVie(c.Fecha)));
            decimal mTotal = (oSemanas.Count() > 0 ? oSemanas.Sum(c => c.Actual) : 0);
            decimal mPromedio = (oSemanas.Count() > 0 ? oSemanas.Average(c => c.Actual) : 0);
            this.dgvSemanas.Rows.Clear();
            this.chrPorSemana.Series["Actual"].Points.Clear();
            this.chrPorSemana.Series["Anterior"].Points.Clear();
            this.chrPorSemana.Series["Promedio"].Points.Clear();
            foreach (var oReg in oSemanas)
            {
                int iFila = this.dgvSemanas.Rows.Add(oReg.Llave, oReg.Llave, oReg.Actual, Util.DividirONull(oReg.Actual, oReg.Anterior)
                    , Util.DividirONull(oReg.Actual, mTotal));
                this.chrPorSemana.Series["Actual"].Points.AddXY(oReg.Llave, oReg.Actual);
                this.chrPorSemana.Series["Anterior"].Points.AddXY(oReg.Llave, oReg.Anterior);
                this.chrPorSemana.Series["Promedio"].Points.AddXY(oReg.Llave, mPromedio);
                if (oReg.Actual < mPromedio)
                    this.dgvSemanas.Rows[iFila].DefaultCellStyle.ForeColor = Color.Black;
            }
            this.txtPromedioSemanas.Text = mPromedio.ToString();
        }

        private void CargarMeses(int iClienteID)
        {
            if (this.oDatos == null)
                return;

            var oMeses = this.AgruparPorEntero(this.oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(c => c.Fecha.Month));
            decimal mTotal = (oMeses.Count() > 0 ? oMeses.Sum(c => c.Actual) : 0);
            decimal mPromedio = (oMeses.Count() > 0 ? oMeses.Average(c => c.Actual) : 0);
            this.dgvMeses.Rows.Clear();
            this.chrPorMes.Series["Actual"].Points.Clear();
            this.chrPorMes.Series["Anterior"].Points.Clear();
            this.chrPorMes.Series["Promedio"].Points.Clear();
            foreach (var oReg in oMeses)
            {
                int iFila = this.dgvMeses.Rows.Add(oReg.Llave, oReg.Llave, oReg.Actual, Util.DividirONull(oReg.Actual, oReg.Anterior)
                    , Util.DividirONull(oReg.Actual, mTotal));
                this.chrPorMes.Series["Actual"].Points.AddXY(oReg.Llave, oReg.Actual);
                this.chrPorMes.Series["Anterior"].Points.AddXY(oReg.Llave, oReg.Anterior);
                this.chrPorMes.Series["Promedio"].Points.AddXY(oReg.Llave, mPromedio);
                if (oReg.Actual < mPromedio)
                    this.dgvMeses.Rows[iFila].DefaultCellStyle.ForeColor = Color.Black;
            }
            this.txtPromedioMeses.Text = mPromedio.ToString();
        }

        private void CargarSucursales(int iClienteID)
        {
            if (this.oDatos == null)
                return;

            var oSucursales = this.AgruparPorEnteroCadena(this.oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(
                c => new EnteroCadenaAgrupar() { Entero = c.SucursalID, Cadena = c.Sucursal }));
            decimal mTotal = (oSucursales.Count() > 0 ? oSucursales.Sum(c => c.Actual) : 0);
            this.dgvSucursales.Rows.Clear();
            foreach (var oReg in oSucursales)
                this.dgvSucursales.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, Util.DividirONull(oReg.Actual, oReg.Anterior), Util.DividirONull(oReg.Actual, mTotal));
        }

        private void CargarVendedores(int iClienteID)
        {
            if (this.oDatos == null)
                return;

            var oVendedores = this.AgruparPorEnteroCadena(this.oDatos.Where(c => c.ClienteID == iClienteID).GroupBy(
                c => new EnteroCadenaAgrupar() { Entero = c.VendedorID, Cadena = c.Vendedor }));
            decimal mTotal = (oVendedores.Count() > 0 ? oVendedores.Sum(c => c.Actual) : 0);
            this.dgvVendedores.Rows.Clear();
            foreach (var oReg in oVendedores)
                this.dgvVendedores.Rows.Add(oReg.Llave, oReg.Cadena, oReg.Actual, Util.DividirONull(oReg.Actual, oReg.Anterior), Util.DividirONull(oReg.Actual, mTotal));
        }

        private void CargarAnios(int iClienteID)
        {
            if (this.oDatosPorAnio == null)
                return;

            decimal mPromedio = this.ObtenerPromedioAnios(this.oDatosPorAnio);
            this.dgvPorAnio.Rows.Clear();
            this.chrPorAnio.Series["Actual"].Points.Clear();
            this.chrPorAnio.Series["Promedio"].Points.Clear();
            foreach (var oReg in this.oDatosPorAnio)
            {
                decimal mDato = this.ObtenerDatoAnio(oReg);
                this.dgvPorAnio.Rows.Add(oReg.Anio, mDato);
                this.chrPorAnio.Series["Actual"].Points.AddXY(oReg.Anio, mDato);
                this.chrPorAnio.Series["Promedio"].Points.AddXY(oReg.Anio, mPromedio);
            }
            this.txtPromedioAnios.Text = mPromedio.ToString();
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

        private decimal ObtenerPromedioAnios(List<pauCuadroDeControlPorAnio_Result> oDatos)
        {
            if (oDatos.Count == 0)
                return 0;

            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad": return oDatos.Average(c => c.Utilidad).Valor();
                case "Utilidad Desc.": return oDatos.Average(c => c.UtilidadConDescuento).Valor();
                case "Precio": return oDatos.Average(c => c.Precio).Valor();
                case "Costo": return oDatos.Average(c => c.Costo).Valor();
                case "Costo Desc.": return oDatos.Average(c => c.CostoConDescuento).Valor();
                case "Ventas": return ((decimal?)oDatos.Average(c => c.Ventas)).Valor();
                case "Productos": return ((decimal?)oDatos.Average(c => c.Productos)).Valor();
            }

            return 0;
        }

        private void CargarArbolPartes(int iClienteID)
        {
            if (this.oDatosPartes.Count == 0)
                return;

            var oFiltro = this.oDatosPartes.Where(c => c.ClienteID == iClienteID);
            var oPartes = this.ObtenerDatosPartes(oFiltro).OrderBy(c => c.Sistema).ThenBy(c => c.Subsistema).ThenBy(c => c.Linea).ThenBy(c => c.Marca).ToList();
            decimal mTotal = (oPartes.Count() > 0 ? oPartes.Sum(c => c.Actual) : 0);

            string sSistema, sSubsistema, sLinea, sMarca;
            TreeGridNode oNodoSistema, oNodoSubsistema, oNodoLinea, oNodoMarca;
            sSistema = sSubsistema = sLinea = sMarca = "";
            oNodoSistema = oNodoSubsistema = oNodoLinea = oNodoMarca = null;
            this.tgvPartes.Nodes.Clear();
            foreach (var oReg in oPartes)
            {
                if (oReg.Sistema != sSistema)
                {
                    sSistema = oReg.Sistema;
                    oNodoSistema = this.tgvPartes.Nodes.Add(sSistema);
                }
                if (oReg.Subsistema != sSubsistema)
                {
                    sSubsistema = oReg.Subsistema;
                    oNodoSubsistema = oNodoSistema.Nodes.Add(sSubsistema);
                }
                if (oReg.Linea != sLinea)
                {
                    sLinea = oReg.Linea;
                    oNodoLinea = oNodoSubsistema.Nodes.Add(sLinea);
                }
                if (oReg.Marca != sMarca)
                {
                    sMarca = oReg.Marca;
                    decimal mPor = Util.DividirONull(oReg.Actual, mTotal).Valor();
                    oNodoMarca = oNodoLinea.Nodes.Add(sMarca, oReg.Actual, mPor);
                    // Se llenan los totales de los niveles superiores
                    oNodoLinea.Cells[1].Value = (Util.Decimal(oNodoLinea.Cells[1].Value) + oReg.Actual);
                    oNodoLinea.Cells[2].Value = (Util.Decimal(oNodoLinea.Cells[2].Value) + mPor);
                    oNodoSubsistema.Cells[1].Value = (Util.Decimal(oNodoSubsistema.Cells[1].Value) + oReg.Actual);
                    oNodoSubsistema.Cells[2].Value = (Util.Decimal(oNodoSubsistema.Cells[2].Value) + mPor);
                    oNodoSistema.Cells[1].Value = (Util.Decimal(oNodoSistema.Cells[1].Value) + oReg.Actual);
                    oNodoSistema.Cells[2].Value = (Util.Decimal(oNodoSistema.Cells[2].Value) + mPor);
                }
            }

            // Se recalculan los porcentajes, en base a cada nodo
            foreach (var oNodSistema in this.tgvPartes.Nodes)
            {
                decimal mTotSis = Util.Decimal(oNodSistema.Cells[1].Value);
                foreach (var oNodSubsistema in oNodSistema.Nodes)
                {
                    decimal mTotSub = Util.Decimal(oNodSubsistema.Cells[1].Value);
                    oNodSubsistema.Cells[2].Value = Util.DividirONull(Util.Decimal(oNodSubsistema.Cells[1].Value), mTotSis);
                    foreach (var oNodLinea in oNodSubsistema.Nodes)
                    {
                        decimal mTotLinea = Util.Decimal(oNodLinea.Cells[1].Value);
                        oNodLinea.Cells[2].Value = Util.DividirONull(Util.Decimal(oNodLinea.Cells[1].Value), mTotSub);
                        foreach (var oNodMarca in oNodLinea.Nodes)
                        {
                            decimal mTotMarca = Util.Decimal(oNodMarca.Cells[1].Value);
                            oNodMarca.Cells[2].Value = Util.DividirONull(Util.Decimal(oNodMarca.Cells[1].Value), mTotLinea);
                        }
                    }
                }
            }
        }

        protected class TotalesPorEntero
        {
            public int Llave { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }

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

        protected class TotalesPorEnteroCadena
        {
            public int Llave { get; set; }
            public string Cadena { get; set; }
            public decimal Actual { get; set; }
            public decimal Anterior { get; set; }
        }
        protected class EnteroCadenaAgrupar : IEnumerable, IEquatable<EnteroCadenaAgrupar>
        {

            public int Entero { get; set; }
            public string Cadena { get; set; }

            public IEnumerator GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public bool Equals(EnteroCadenaAgrupar other)
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
        protected virtual IEnumerable<TotalesPorEnteroCadena> AgruparPorEnteroCadena(IEnumerable<IGrouping<EnteroCadenaAgrupar, pauCuadroDeControlGeneral_Result>> oDatos)
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

        private class TotalesClientesSemanas
        {
            public int ClienteID { get; set; }
            public int Semana { get; set; }
            public decimal Actual { get; set; }
        }
        private IEnumerable<TotalesClientesSemanas> ObtenerClientesSemanas(List<pauCuadroDeControlGeneral_Result> oDatos)
        {
            var oGrupo = oDatos.GroupBy(c => new { c.ClienteID, Semana = UtilTheos.SemanaSabAVie(c.Fecha) });

            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oGrupo.Select(c => new TotalesClientesSemanas()
                    {
                        ClienteID = c.Key.ClienteID,
                        Semana = c.Key.Semana,
                        Actual = c.Sum(s => s.Actual).Valor()
                    });
                case "Utilidad Desc.":
                    return oGrupo.Select(c => new TotalesClientesSemanas()
                    {
                        ClienteID = c.Key.ClienteID,
                        Semana = c.Key.Semana,
                        Actual = c.Sum(s => s.UtilDescActual).Valor()
                    });
                case "Precio":
                    return oGrupo.Select(c => new TotalesClientesSemanas()
                    {
                        ClienteID = c.Key.ClienteID,
                        Semana = c.Key.Semana,
                        Actual = c.Sum(s => s.PrecioActual).Valor()
                    });
                case "Costo":
                    return oGrupo.Select(c => new TotalesClientesSemanas()
                    {
                        ClienteID = c.Key.ClienteID,
                        Semana = c.Key.Semana,
                        Actual = c.Sum(s => s.CostoActual).Valor()
                    });
                case "Costo Desc.":
                    return oGrupo.Select(c => new TotalesClientesSemanas()
                    {
                        ClienteID = c.Key.ClienteID,
                        Semana = c.Key.Semana,
                        Actual = c.Sum(s => s.CostoDescActual).Valor()
                    });
                case "Ventas":
                    return oGrupo.Select(c => new TotalesClientesSemanas()
                    {
                        ClienteID = c.Key.ClienteID,
                        Semana = c.Key.Semana,
                        Actual = c.Where(s => s.EsActual == true).Select(s => s.VentaID).Distinct().Count()
                    });
                case "Productos":
                    return oGrupo.Select(c => new TotalesClientesSemanas()
                    {
                        ClienteID = c.Key.ClienteID,
                        Semana = c.Key.Semana,
                        Actual = c.Sum(s => s.ProductosActual).Valor()
                    });
            }

            return null;
        }

        private class TotalesPartes
        {
            public string Sistema { get; set; }
            public string Subsistema { get; set; }
            public string Linea { get; set; }
            public string Marca { get; set; }
            public decimal Actual { get; set; }
        }
        private IEnumerable<TotalesPartes> ObtenerDatosPartes(IEnumerable<pauCuadroDeControlPartes_Result> oDatos)
        {
            var oGrupo = oDatos.GroupBy(c => new { c.Sistema, c.Subsistema, c.Linea, c.Marca });
            string sCalculo = this.cmbCalculo.Text;
            switch (sCalculo)
            {
                case "Utilidad":
                    return oGrupo.Select(c => new TotalesPartes()
                    {
                        Sistema = c.Key.Sistema,
                        Subsistema = c.Key.Subsistema,
                        Linea = c.Key.Linea,
                        Marca = c.Key.Marca,
                        Actual = c.Sum(s => s.Utilidad).Valor()
                    });
                case "Utilidad Desc.":
                    return oGrupo.Select(c => new TotalesPartes()
                    {
                        Sistema = c.Key.Sistema,
                        Subsistema = c.Key.Subsistema,
                        Linea = c.Key.Linea,
                        Marca = c.Key.Marca,
                        Actual = c.Sum(s => s.UtilidadConDescuento).Valor()
                    });
                case "Precio":
                    return oGrupo.Select(c => new TotalesPartes()
                    {
                        Sistema = c.Key.Sistema,
                        Subsistema = c.Key.Subsistema,
                        Linea = c.Key.Linea,
                        Marca = c.Key.Marca,
                        Actual = c.Sum(s => s.Precio).Valor()
                    });
                case "Costo":
                    return oGrupo.Select(c => new TotalesPartes()
                    {
                        Sistema = c.Key.Sistema,
                        Subsistema = c.Key.Subsistema,
                        Linea = c.Key.Linea,
                        Marca = c.Key.Marca,
                        Actual = c.Sum(s => s.Costo).Valor()
                    });
                case "Costo Desc.":
                    return oGrupo.Select(c => new TotalesPartes()
                    {
                        Sistema = c.Key.Sistema,
                        Subsistema = c.Key.Subsistema,
                        Linea = c.Key.Linea,
                        Marca = c.Key.Marca,
                        Actual = c.Sum(s => s.CostoConDescuento).Valor()
                    });
                case "Ventas":
                    return oGrupo.Select(c => new TotalesPartes()
                    {
                        Sistema = c.Key.Sistema,
                        Subsistema = c.Key.Subsistema,
                        Linea = c.Key.Linea,
                        Marca = c.Key.Marca,
                        Actual = c.Select(s => s.VentaID).Distinct().Count()
                    });
                case "Productos":
                    return oGrupo.Select(c => new TotalesPartes()
                    {
                        Sistema = c.Key.Sistema,
                        Subsistema = c.Key.Subsistema,
                        Linea = c.Key.Linea,
                        Marca = c.Key.Marca,
                        Actual = c.Select(s => s.ParteID).Count()
                    });
            }

            return null;
        }

        #endregion

    }
}
