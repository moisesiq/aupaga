using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class catalogosPedidos : UserControl
    {
        ControlError cntError = new ControlError();
        // BindingSource fuenteDatos;
        // DataTable dtProveedores = new DataTable();
        // DataTable dtPedidos = new DataTable();
        bool sel = true;
        bool bGridPedidosSel = false;

        public static catalogosPedidos Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly catalogosPedidos instance = new catalogosPedidos();
        }

        public catalogosPedidos()
        {
            InitializeComponent();
        }

        #region [ Metodos ]

        public void CargaInicial()
        {
            // Se validan los permisos
            //if (this.EsNuevo)
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Agregar", true))
            //    {
            //        this.Close();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Modificar", false))
            //        this.btnGuardar.Enabled = false;
            //}

            try
            {
                this.tabPedidos.SelectedIndex = 0;
                this.LimpiarFormulario();

                this.CargarSucursales();

                // Se cargan los combos de marcas y líneas
                var oGuardados = Datos.GetListOf<PedidoFiltro>(c => c.Tabla == Cat.Tablas.Marcas || c.Tabla == Cat.Tablas.Lineas);
                var oParams = new Dictionary<string, object>();
                oParams.Add("Pagadas", true);
                oParams.Add("Cobradas", false);
                oParams.Add("Solo9500", false);
                oParams.Add("OmitirDomingo", false);
                oParams.Add("Desde", DateTime.Now.AddMonths(-6));
                oParams.Add("Hasta", DateTime.Now);
                var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
                // Marcas
                var oMarcas = oDatos.GroupBy(c => new { c.MarcaID, c.Marca })
                    .Select(c => new { MarcaID = c.Key.MarcaID, Marca = c.Key.Marca, UtilDesc = c.Sum(s => s.UtilDescActual) })
                    .OrderByDescending(c => c.UtilDesc);
                decimal mTotal = oMarcas.Sum(c => c.UtilDesc).Valor();
                foreach (var oReg in oMarcas)
                {
                    decimal mPorcentaje = Math.Round((oReg.UtilDesc.Valor() / mTotal) * 100, 2);
                    bool bMarca = oGuardados.Any(c => c.Tabla == Cat.Tablas.Marcas && c.RelacionID == oReg.MarcaID && c.Seleccion == true);
                    this.ctlMarcas.AgregarElemento(oReg.MarcaID.Valor(), (mPorcentaje.ToString() + " " + oReg.Marca), bMarca);
                }
                // Líneas
                var oLineas = oDatos.GroupBy(c => new { c.LineaID, c.Linea })
                    .Select(c => new { LineaID = c.Key.LineaID, Linea = c.Key.Linea, UtilDesc = c.Sum(s => s.UtilDescActual) })
                    .OrderByDescending(c => c.UtilDesc);
                mTotal = oLineas.Sum(c => c.UtilDesc).Valor();
                foreach (var oReg in oLineas)
                {
                    decimal mPorcentaje = Math.Round((oReg.UtilDesc.Valor() / mTotal) * 100, 2);
                    bool bMarca = oGuardados.Any(c => c.Tabla == Cat.Tablas.Lineas && c.RelacionID == oReg.LineaID && c.Seleccion == true);
                    this.ctlLineas.AgregarElemento(oReg.LineaID.Valor(), (mPorcentaje.ToString() + " " + oReg.Linea), bMarca);
                }
                //

                var mesActual = DateTime.Now.Month;
                var anioActual = DateTime.Now.Year;

                var ultimoDiaDelMes = DateTime.DaysInMonth(anioActual, mesActual);
                this.dtpInicial.Value = new DateTime(anioActual, mesActual, 1);
                this.dtpFinal.Value = new DateTime(anioActual, mesActual, ultimoDiaDelMes);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void LimpiarFormulario()
        {
            try
            {
                /* this.dgvSugeridos.DataSource = null;
                if (this.dgvSugeridos.Rows.Count > 0)
                    this.dgvSugeridos.Rows.Clear();
                if (this.dgvSugeridos.Columns.Count > 0)
                    this.dgvSugeridos.Columns.Clear();

                this.dgvProveedores.DataSource = null;
                if (this.dgvProveedores.Rows.Count > 0)
                    this.dgvProveedores.Rows.Clear();
                if (this.dgvProveedores.Columns.Count > 0)
                    this.dgvProveedores.Columns.Clear();
                */
                this.dgvProveedores.Rows.Clear();
                this.dgvSugeridos.Rows.Clear();

                this.dgvExistencias.DataSource = null;

                this.txtImporteTotal.Clear();
                this.txtImporteTotalDos.Clear();

                this.dgvPedidos.DataSource = null;
                if (this.dgvPedidos.Rows.Count > 0)
                    this.dgvPedidos.Rows.Clear();
                if (this.dgvPedidos.Columns.Count > 0)
                    this.dgvPedidos.Columns.Clear();

                this.dgvDetallePedido.DataSource = null;
                if (this.dgvDetallePedido.Rows.Count > 0)
                    this.dgvDetallePedido.Rows.Clear();
                if (this.dgvDetallePedido.Columns.Count > 0)
                    this.dgvDetallePedido.Columns.Clear();

                this.chkMostrarTodos.Checked = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void LimpiarFormularioDos()
        {
            try
            {
                /* this.dgvSugeridos.DataSource = null;
                if (this.dgvSugeridos.Rows.Count > 0)
                    this.dgvSugeridos.Rows.Clear();
                if (this.dgvSugeridos.Columns.Count > 0)
                    this.dgvSugeridos.Columns.Clear();

                this.dgvProveedores.DataSource = null;
                if (this.dgvProveedores.Rows.Count > 0)
                    this.dgvProveedores.Rows.Clear();
                if (this.dgvProveedores.Columns.Count > 0)
                    this.dgvProveedores.Columns.Clear();
                */
                this.dgvProveedores.Rows.Clear();
                this.dgvSugeridos.Rows.Clear();

                this.dgvExistencias.DataSource = null;

                this.txtImporteTotal.Clear();
                this.txtImporteTotalDos.Clear();
                this.progreso.Value = 0;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        
        public void CargarSucursales()
        {
            try
            {
                this.dgvSucursales.DataSource = null;
                if (this.dgvSucursales.Rows.Count > 0)
                    this.dgvSucursales.Rows.Clear();
                if (this.dgvSucursales.Columns.Count > 0)
                    this.dgvSucursales.Columns.Clear();

                var sucursales = UtilLocal.newTable<SucursalesCriterioAbcView>("Sucursales", Datos.GetListOf<SucursalesCriterioAbcView>());

                var colX = new DataColumn();
                colX.DataType = System.Type.GetType("System.Boolean");
                colX.ColumnName = "X";
                sucursales.Columns.Add(colX);

                this.dgvSucursales.DataSource = sucursales;
                this.dgvSucursales.Columns["X"].DisplayIndex = 0;
                this.dgvSucursales.Columns["X"].Width = 20;
                this.dgvSucursales.Columns["X"].HeaderText = "";

                foreach (DataGridViewColumn column in this.dgvSucursales.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (!column.Name.Equals("X"))
                        column.ReadOnly = true;
                }

                foreach (DataGridViewRow row in this.dgvSucursales.Rows)
                {
                    row.Cells["X"].Value = true;
                }

                this.dgvSucursales.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvSucursales.BackgroundColor = Color.FromArgb(188, 199, 216);

                Util.OcultarColumnas(this.dgvSucursales, new string[] { "SucursalID", "ids", "EntityKey", "EntityState" });
                UtilLocal.ColumnasToHeaderText(this.dgvSucursales);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargaExistencias(int parteId)
        {
            try
            {
                this.dgvExistencias.DataSource = null;
                if (dgvExistencias.Columns.Count > 0)
                    dgvExistencias.Columns.Clear();

                if (dgvExistencias.Rows.Count > 0)
                    dgvExistencias.Rows.Clear();

                this.dgvExistencias.DataSource = Datos.GetListOf<ExistenciasView>(ex => ex.ParteID.Equals(parteId));
                this.dgvExistencias.MostrarColumnas("Tienda", "Exist", "Max", "Min");
                this.dgvExistencias.AutoResizeColumns();
                // Util.OcultarColumnas(this.dgvExistencias, new string[] { "ParteExistenciaID", "ParteID", "NumeroParte", "SucursalID" });
                UtilLocal.ColumnasToHeaderText(this.dgvExistencias);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void sacarImporteTotal()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
            {
                if (!oFila.Visible || !Util.Logico(oFila.Cells["sug_Sel"].Value)) continue;
                mTotal += Util.Decimal(oFila.Cells["sug_CostoTotal"].Value);
            }
            this.txtImporteTotal.Text = mTotal.ToString(GlobalClass.FormatoMoneda);

            /* Se modifica la forma de calcular el total, pues se encontraban diferencias al seleccionar y des-seleccionar todos - Moi 2015-09-01
            try
            {
                decimal importeTotal = dgvSugeridos.Rows.OfType<DataGridViewRow>().Where(c => Util.ConvertirCadena(c.Cells["Caracteristica"].Value) != "NP")
                    .Sum(row => Util.ConvertirDecimal(row.Cells["Costo Total"].Value));
                var importeNoSeleccionados = 0.0M;

                /* No se xq se calculaba el importe de lo no seleccionado, se modifica para q sólo se considere el importe de lo seleccionado - Moi 2015-09-01
                foreach (DataGridViewRow fila in this.dgvSugeridos.Rows)
                {
                    if (!fila.Visible) continue;

                    DataGridViewCheckBoxCell cb = (DataGridViewCheckBoxCell)fila.Cells["Sel"];
                    if (cb.Value != null)
                    {
                        if (!Util.ConvertirBool(cb.Value))
                        {
                            importeNoSeleccionados += Util.ConvertirDecimal(fila.Cells["Costo Total"].Value);
                        }
                    }
                }
                * /

                this.txtImporteTotal.Text = Util.DecimalToCadenaMoneda(importeTotal - importeNoSeleccionados);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }

            */
        }

        private void CargarSugeridos()
        {
            // Se obtiene el filtro de sucursales
            var oSucursales = new DataTable();
            oSucursales.Columns.Add("SucursalID", typeof(int));
            foreach (DataGridViewRow oFila in this.dgvSucursales.Rows)
            {
                if (Util.Logico(oFila.Cells["X"].Value))
                    oSucursales.Rows.Add(Util.Entero(oFila.Cells["SucursalID"].Value));
            }
            if (oSucursales.Rows.Count == 0)
            {
                Util.MensajeAdvertencia("Debe seleccionar al menos una sucursal", GlobalClass.NombreApp);
                return;
            }

            Cargando.Mostrar();
            this.btnMostrar.Enabled = false;
            this.tabPedidos.SelectedIndex = 0;
            // var oLog = new Log();
            // oLog.Show();
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Obteniendo datos.");  // dPend

            // Se obtienen los parámetros adicionales
            var dic = new Dictionary<string, object>();
            dic.Add("Sucursales/tpuTablaEnteros", oSucursales);

            // Se agregan los parámetros de línea y marca
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
            {
                var oDataTable = Util.ListaEntityADataTable(this.ctlMarcas.ElementosSeleccionados);
                oDataTable.Columns.Remove("Cadena");
                dic.Add("Marcas/tpuTablaEnteros", oDataTable);
            }
            if (this.ctlLineas.ValoresSeleccionados.Count > 0)
            {
                var oDataTable = Util.ListaEntityADataTable(this.ctlLineas.ElementosSeleccionados);
                oDataTable.Columns.Remove("Cadena");
                dic.Add("Lineas/tpuTablaEnteros", oDataTable);
            }

            // Se obtienen los datos
            var oSugeridos = Datos.ExecuteProcedure<pauPedidosSugeridos_Res>("pauPedidosSugeridos", dic);

            // Se comienzan a llenar los datos
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Llenando sugeridos.");  // dPend

            //     
            var selMatriz = false;
            var selSuc02 = false;
            var selSuc03 = false;
            var nivelMatriz = string.Empty;
            var nivelSuc02 = string.Empty;
            var nivelSuc03 = string.Empty;
            foreach (DataGridViewRow fila in this.dgvSucursales.Rows)
            {
                if (Util.Logico(fila.Cells["X"].Value) == true && Util.Cadena(fila.Cells["NombreSucursal"].Value).ToUpper() == "MATRIZ")
                {
                    selMatriz = true;
                    nivelMatriz = Util.Cadena(fila.Cells["Nivel"].Value);
                }
                if (Util.Logico(fila.Cells["X"].Value) == true && Util.Cadena(fila.Cells["NombreSucursal"].Value).ToUpper() == "SUC02")
                {
                    selSuc02 = true;
                    nivelSuc02 = Util.Cadena(fila.Cells["Nivel"].Value);
                }
                if (Util.Logico(fila.Cells["X"].Value) == true && Util.Cadena(fila.Cells["NombreSucursal"].Value).ToUpper() == "SUC03")
                {
                    selSuc03 = true;
                    nivelSuc03 = Util.Cadena(fila.Cells["Nivel"].Value);
                }
            }

            // Se comienzan a llenar los datos
            this.dgvSugeridos.Rows.Clear();
            foreach (var oReg in oSugeridos)
            {
                // Se verifica si está seleccionada la sucursal
                oReg.NecesidadMatriz = (selMatriz ? oReg.NecesidadMatriz : 0);
                oReg.NecesidadSuc02 = (selSuc02 ? oReg.NecesidadSuc02 : 0);
                oReg.NecesidadSuc03 = (selSuc03 ? oReg.NecesidadSuc03 : 0);

                // Si no se cumple el criterio Abc según el filtro, por sucursal, la necesidad se hace cero
                oReg.NecesidadMatriz = (nivelMatriz.Contains(oReg.CriterioABC) ? oReg.NecesidadMatriz : 0);
                oReg.NecesidadSuc02 = (nivelSuc02.Contains(oReg.CriterioABC) ? oReg.NecesidadSuc02 : 0);
                oReg.NecesidadSuc03 = (nivelSuc02.Contains(oReg.CriterioABC) ? oReg.NecesidadSuc03 : 0);

                if (oReg.UnidadEmpaque > 1)
                {
                    decimal mTotal = (oReg.NecesidadMatriz + oReg.NecesidadSuc02 + oReg.NecesidadSuc03).Valor();
                    oReg.Pedido = this.CalcularPedido(mTotal, oReg.UnidadEmpaque.Valor());
                }
                decimal mCostoTotal = (oReg.Pedido * oReg.CostoConDescuento).Valor();
                oReg.Costo = mCostoTotal;

                int iFila = this.dgvSugeridos.Rows.Add(oReg.ParteID, oReg.ProveedorID, true, oReg.NumeroParte, oReg.NombreParte, oReg.UnidadEmpaque, oReg.CriterioABC
                    , oReg.NecesidadMatriz, oReg.NecesidadSuc02, oReg.NecesidadSuc03, oReg.Total, oReg.Pedido, oReg.CostoConDescuento, mCostoTotal
                    , oReg.Observacion, oReg.Caracteristica);
                var oFila = this.dgvSugeridos.Rows[iFila];

                // Se colorean las filas
                /* switch (oReg.Caracteristica)
                {
                    case "9500": oFila.Cells["sug_Descripcion"].Style.ForeColor = Color.Blue; break;
                    case "RF": oFila.Cells["sug_Descripcion"].Style.ForeColor = Color.Red; break;
                }
                */
            }
            // Se llena el total
            this.txtImporteTotal.Text = oSugeridos.Sum(c => c.Costo).Valor().ToString(GlobalClass.FormatoMoneda);

            // Se obtienen los datos para lo de proveedores
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Llenando proveedores.");  // dPend
            var oProveedores = oSugeridos.GroupBy(c => c.ProveedorID).Select(c => new
            {
                ProveedorID = c.Key,
                Proveedor = c.Max(s => s.NombreProveedor),
                Importe = c.Sum(s => s.Costo),
                Caracteristica = c.Max(s => s.Caracteristica)
            }).OrderByDescending(c => c.Importe);
            decimal mTotalProv = oProveedores.Sum(c => c.Importe).Valor();
            // Se llena el grid de proveedores
            this.dgvProveedores.Rows.Clear();
            foreach (var oReg in oProveedores)
            {
                int iFila = this.dgvProveedores.Rows.Add(oReg.ProveedorID, oReg.Proveedor, oReg.Importe, (oReg.Importe / mTotalProv), oReg.Caracteristica);
                var oFila = this.dgvProveedores.Rows[iFila];
                // Se colorean las filas
                /* switch (oReg.Caracteristica)
                {
                    case "9500": oFila.Cells["pro_Proveedor"].Style.ForeColor = Color.Blue; break;
                    case "RF": oFila.Cells["pro_Proveedor"].Style.ForeColor = Color.Red; break;
                }
                */
            }
            // Se llena el total
            this.txtImporteTotalDos.Text = mTotalProv.ToString(GlobalClass.FormatoMoneda);

            // Se colorean algunas filas, según el caso
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Coloreando.");  // dPend
            this.ColorearSugeridos(true);

            // Se calcula el presupuesto
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Calculando presupuesto.");  // dPend
            this.CalcularPresupuesto();
            
            // Se guardan las líneas y marcas seleccionadas
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Guardando filtro.");  // dPend
            this.GuardarFiltro();

            //
            this.btnMostrar.Enabled = true;
            // oLog.finalizo = true;
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Finalizó.");  // dPend
            Cargando.Cerrar();
        }

        private void ColorearSugeridos(bool bColorearGridProveedores)
        {
            /* Ya no se ocultan las partes NP (partes que no tienen existencia pero tienen equivalentes que sí), pues se notó que esa funcionalidad sí aplica
             * pero sólo para ciertos casos, no para todos. Se revisará más a detalle posteriormente. Moi - 27/07/2015
            this.dgvSugeridos.CurrentCell = null;
            foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
                oFila.Visible = (Util.ConvertirCadena(oFila.Cells["Caracteristica"].Value) != "NP");
            */

            foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
            {
                // Se ocultan las filas de partes que no deben ser pedidas por existencia en sus equivalentes
                // oFila.Visible = (Util.ConvertirCadena(oFila.Cells["Caracteristica"].Value) != "NP");
                /* if (Util.ConvertirCadena(oFila.Cells["Caracteristica"].Value) != "NP")
                {
                    //this.dgvSugeridos.CurrentCell = null;
                    oFila.Visible = false;
                } */
                
                // Coloración
                /* Color oColor = Color.Black;
                switch (Util.ConvertirCadena(oFila.Cells["Caracteristica"].Value))
                {
                    case "9500": oColor = Color.Blue; break;
                    case "RF": oColor = Color.Red; break;
                }

                if (oColor != Color.Black)
                {
                    oFila.Cells["NombreParte"].Style.ForeColor = oColor;
                    // Se colorea la línea del proveedor
                    if (bColorearGridProveedores)
                    {
                        int iFilaP = this.dgvProveedores.EncontrarIndiceDeValor("ProveedorID", Util.ConvertirEntero(oFila.Cells["ProveedorID"].Value));
                        this.dgvProveedores["Nombre Proveedor", iFilaP].Style.ForeColor = oColor;
                    }
                }
                */

                switch (Util.Cadena(oFila.Cells["sug_Caracteristica"].Value))
                {
                    case "9500": oFila.Cells["sug_Descripcion"].Style.ForeColor = Color.Blue; break;
                    case "RF": oFila.Cells["sug_Descripcion"].Style.ForeColor = Color.Red; break;
                }
            }

            // Se colorean las filas de proveedores
            if (bColorearGridProveedores)
            {
                foreach (DataGridViewRow oFila in this.dgvProveedores.Rows)
                {
                    switch (Util.Cadena(oFila.Cells["pro_Caracteristica"].Value))
                    {
                        case "9500": oFila.Cells["pro_Proveedor"].Style.ForeColor = Color.Blue; break;
                        case "RF": oFila.Cells["pro_Proveedor"].Style.ForeColor = Color.Red; break;
                    }
                }
            }
        }

        private void CargarEquivalentes(int parteId)
        {
            var equivalentes = Datos.GetListOf<PartesEquivalentesView>(pe => pe.ParteID == parteId)
                .OrderByDescending(c => c.Matriz).ThenByDescending(c => c.Suc02).ThenByDescending(c => c.Suc03).ThenBy(c => c.CostoConDescuento);
            this.dgvEquivalente.Rows.Clear();   
            foreach (var equivalente in equivalentes)
            {
                this.dgvEquivalente.Rows.Add(equivalente.ParteIDEquivalente, equivalente.NumeroParte, equivalente.Descripcion
                    , equivalente.CostoConDescuento, equivalente.Matriz, equivalente.Suc02, equivalente.Suc03);
            }
            this.dgvEquivalente.CurrentCell = null;
        }

        private void QuitarSugerido(DataGridViewRow oFila)
        {
            int iParteID = Util.Entero(oFila.Cells["sug_ParteID"].Value);
            for (int iCont = 1; iCont < 4; iCont++)
            {
                var oReporteF = Datos.GetEntity<ReporteDeFaltante>(c => c.ParteID == iParteID && c.SucursalID == iCont && !c.Pedido && c.Estatus);
                if (oReporteF != null)
                    Datos.Eliminar<ReporteDeFaltante>(oReporteF, true);
            }
        }

        private void CalcularPresupuesto()
        {
            var oFechasAnt = UtilDatos.FechasDeComisiones(DateTime.Now.AddDays(-7));
            var oFechas = UtilDatos.FechasDeComisiones(DateTime.Now);
            /*
            var oSucursalesSel = new Dictionary<int, bool>();
            bool bSucursal1 = false, bSucursal2 = false, bSucursal3 = false;
            foreach (DataGridViewRow oFila in this.dgvSucursales.Rows)
            {
                switch (Util.ConvertirEntero(oFila.Cells["SucursalID"].Value))
                {
                    case 1: bSucursal1 = Util.ConvertirBool(oFila.Cells["X"].Value); break;
                    case 2: bSucursal2 = Util.ConvertirBool(oFila.Cells["X"].Value); break;
                    case 3: bSucursal3 = Util.ConvertirBool(oFila.Cells["X"].Value); break;
                }
            }
            var oVentasDetV = General.GetListOf<VentasDetalleAvanzadoView>(c => c.VentaEstatusID == Cat.VentasEstatus.Completada
                && c.VentaFecha >= oFechasAnt.Valor1 && c.VentaFecha <= oFechasAnt.Valor2
                && ((bSucursal1 && c.SucursalID == 1) || (bSucursal2 && c.SucursalID == 2) || (bSucursal3 && c.SucursalID == 3))
            );
            decimal mVendido = oVentasDetV.Sum(c => c.Costo * c.Cantidad);
            */

            // Para calcular el presupuesto
            var oParams = new Dictionary<string, object>();
            oParams.Add("Pagadas", true);
            oParams.Add("Cobradas", false);
            oParams.Add("Solo9500", false);
            oParams.Add("OmitirDomingo", false);
            oParams.Add("Desde", oFechasAnt.Valor1);
            oParams.Add("Hasta", oFechasAnt.Valor2);
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            decimal mVendido = oDatos.Sum(c => c.CostoActual).Valor();

            var oPedidos = Datos.GetListOf<Pedido>(c => c.FechaRegistro >= oFechas.Valor1 && c.FechaRegistro <= oFechas.Valor2 && c.Estatus);
            decimal mPedido = (oPedidos.Count > 0 ? oPedidos.Sum(c => c.ImporteTotal) : 0);

            // Se aplica el incremento de configuración, si hay
            decimal mPor = Util.Decimal(Config.Valor("Pedidos.PorcentajeIncrementoPresupuesto"));
            mVendido *= (1 + (mPor / 100));

            this.txtPresupuesto.Text = mVendido.ToString(GlobalClass.FormatoMoneda);
            this.txtGastado.Text = mPedido.ToString(GlobalClass.FormatoMoneda);
            this.txtPorGastar.Text = (mVendido - mPedido).ToString(GlobalClass.FormatoMoneda);
        }

        private void GuardarFiltro()
        {
            // Se guardan las marcas
            var oGuardados = Datos.GetListOf<PedidoFiltro>(c => c.Tabla == Cat.Tablas.Marcas);
            foreach (int iId in this.ctlMarcas.ValoresSeleccionados)
            {
                // Se verifica si ya existe el registro
                var oReg = oGuardados.FirstOrDefault(c => c.RelacionID == iId);
                if (oReg == null)
                {
                    oReg = new PedidoFiltro()
                    {
                        Tabla = Cat.Tablas.Marcas,
                        RelacionID = iId
                    };
                }
                oReg.Seleccion = true;
                Datos.Guardar<PedidoFiltro>(oReg);
                oGuardados.Remove(oReg);
            }
            // Se guardan las que antes estaba y ahora no fueron marcadas
            foreach (var oReg in oGuardados)
            {
                oReg.Seleccion = false;
                Datos.Guardar<PedidoFiltro>(oReg);
            }

            // Se procede a guardar las líneas
            oGuardados = Datos.GetListOf<PedidoFiltro>(c => c.Tabla == Cat.Tablas.Lineas);
            foreach (int iId in this.ctlLineas.ValoresSeleccionados)
            {
                // Se verifica si ya existe el registro
                var oReg = oGuardados.FirstOrDefault(c => c.RelacionID == iId);
                if (oReg == null)
                {
                    oReg = new PedidoFiltro()
                    {
                        Tabla = Cat.Tablas.Lineas,
                        RelacionID = iId
                    };
                }
                oReg.Seleccion = true;
                Datos.Guardar<PedidoFiltro>(oReg);
                oGuardados.Remove(oReg);
            }
            // Se guardan las que antes estaba y ahora no fueron marcadas
            foreach (var oReg in oGuardados)
            {
                oReg.Seleccion = false;
                Datos.Guardar<PedidoFiltro>(oReg);
            }
        }

        private void AplicarCambioNecesidad(DataGridViewRow oFila)
        {
            decimal mTotal = (Util.Decimal(oFila.Cells["sug_NecesidadMatriz"].Value) + Util.Decimal(oFila.Cells["sug_NecesidadSuc02"].Value)
                + Util.Decimal(oFila.Cells["sug_NecesidadSuc03"].Value));
            decimal mEmpaque = Util.Decimal(oFila.Cells["sug_UnidadDeEmpaque"].Value);
            decimal mPedido = this.CalcularPedido(mTotal, mEmpaque);

            oFila.Cells["sug_Total"].Value = mTotal;
            oFila.Cells["sug_Pedido"].Value = mPedido;
            oFila.Cells["sug_CostoTotal"].Value = (mPedido * Util.Decimal(oFila.Cells["sug_CostoConDescuento"].Value));
            
            this.sacarImporteTotal();
        }

        private decimal CalcularPedido(decimal mNecesidad, decimal mUnidadDeEmpaque)
        {
            decimal mFactor = (mNecesidad / mUnidadDeEmpaque);
            mFactor = Math.Round(mFactor, 0);
            if (mFactor == 0)
                mFactor = 1;
            return (mFactor * mUnidadDeEmpaque);
        }

        private void LlenarDescripcionMaxMin(DataGridViewRow oFila)
        {
            this.txtDescripcionMaxMin.Clear();
            if (oFila == null)
                return;
            this.txtDescripcionMaxMin.Text = string.Format("Condición: {0} Procesado: {1}\r\n{2}", Util.Entero(oFila.Cells["ParteMaxMinReglaID"].Value)
                , Util.FechaHora(oFila.Cells["FechaMaxMin"].Value), Util.Cadena(oFila.Cells["DescripcionMaxMin"].Value));
        }

        #endregion

        #region [ Eventos ]

        private void catalogosPedidos_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarSugeridos();
        }
                         
        public void OrganizarGrid(DataGridView Grid)
        {
            int twidth = 0;
            if (Grid.Rows.Count > 0)
            {
                twidth = 70; //(Grid.Width * Grid.Columns.Count) / 210;
                for (int i = 0; i < Grid.Columns.Count; i++)
                {
                    if (i == 2)
                        Grid.Columns[i].Width = twidth;
                    if (i == 3)
                        Grid.Columns[i].Width = twidth * 3;
                    if (i > 3)
                        Grid.Columns[i].Width = 60;
                }
            }
        }

        private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change)
            {
                this.sacarImporteTotal();
                e.Row.AcceptChanges();
            }
        }

        private void dgvProveedores_Sorted(object sender, EventArgs e)
        {
            // this.fuenteDatos.RemoveFilter();
            this.ColorearSugeridos(true);
        }

        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvProveedores.CurrentRow == null || e.RowIndex < 0) return;
            /*string filter = string.Empty;
            fuenteDatos.Filter = filter;
            filter = "NombreProveedor like '%" + Util.ConvertirCadena(this.dgvProveedores.Rows[e.RowIndex].Cells["Nombre Proveedor"].Value) + "%'";
            if (fuenteDatos != null)
                fuenteDatos.Filter = filter;
            this.ColorearSugeridos(false);
            */

            int iProveedorID = Util.Entero(this.dgvProveedores.CurrentRow.Cells["pro_ProveedorID"].Value);
            this.dgvSugeridos.FiltrarEntero(iProveedorID, "sug_ProveedorID");

            this.sacarImporteTotal();
        }

        private void dgvSugeridos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvSugeridos.CurrentRow == null) return;
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                var fila = dgvSugeridos.CurrentRow.Index;
                if (Util.Logico(dgvSugeridos.Rows[fila].Cells["sug_Sel"].Value).Equals(true))
                    dgvSugeridos.Rows[fila].Cells["sug_Sel"].Value = false;
                else
                    dgvSugeridos.Rows[fila].Cells["sug_Sel"].Value = true;
            }
        }

        private void dgvSugeridos_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.dgvSugeridos.CurrentRow != null)
                {
                    if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                    {
                        this.dgvSugeridos_CellClick(sender, new DataGridViewCellEventArgs(0, this.dgvSugeridos.CurrentRow.Index));
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvSugeridos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgvSugeridos.IsCurrentCellDirty)
            {
                this.dgvSugeridos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                this.sacarImporteTotal();
            }
        }

        private void dgvSugeridos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string sCol = this.dgvSugeridos[e.ColumnIndex, e.RowIndex].OwningColumn.Name;
            if (sCol == "sug_NecesidadMatriz" || sCol == "sug_NecesidadSuc02" || sCol == "sug_NecesidadSuc03")
            {
                this.AplicarCambioNecesidad(this.dgvSugeridos.Rows[e.RowIndex]);
            }
        }

        private void dgvSugeridos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvSugeridos.CurrentRow == null)
                return;
            if (e.RowIndex > -1)
            {
                int parteId = Util.Entero(this.dgvSugeridos.Rows[e.RowIndex].Cells["sug_ParteID"].Value);
                this.CargaExistencias(parteId);

                this.CargarEquivalentes(parteId);

                // Se llena la observación, para cuando es un reporte de faltante
                
                this.txtSugerenciaObservacion.Text = Util.Cadena(this.dgvSugeridos["sug_Observacion", e.RowIndex].Value);

                // Se cargan los datos de las ventas para la parte seleccionada
                this.ctlVentasPorMes.LlenarDatos(parteId);

                // Para cambiar el color de fondo de algunos grids
                this.dgvExistencias.CambiarColorDeFondo(Color.FromArgb(188, 199, 216));
                this.ctlVentasPorMes.dgvDatos.CambiarColorDeFondo(Color.FromArgb(188, 199, 216));
                // this.bGridPedidosSel = true;
            }
        }

        private void dgvSugeridos_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {

                if (this.dgvSugeridos.Columns[e.ColumnIndex].Name == "NecesidadMatriz" ||
                    this.dgvSugeridos.Columns[e.ColumnIndex].Name == "NecesidadSuc02" || this.dgvSugeridos.Columns[e.ColumnIndex].Name == "NecesidadSuc03")
                {
                    decimal value;
                    if (!Decimal.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        this.dgvSugeridos.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                    else if (value < 0)
                    {
                        this.dgvSugeridos.Rows[e.RowIndex].ErrorText = "Debe ingresar un valor mayor o igual cero.";
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvSugeridos_Sorted(object sender, EventArgs e)
        {
            this.ColorearSugeridos(false);
        }

        private void cmsSugeridos_Opening(object sender, CancelEventArgs e)
        {
            //this.cmsSugeridos.Items["tmiSugeridosQuitar"].Enabled = false;// (Util.ConvertirCadena(this.dgvSugeridos.CurrentRow.Cells["Caracteristica"].Value) == "RF");
            //this.tmiSugeridosQuitar.Visible = (Util.ConvertirCadena(this.dgvSugeridos.CurrentRow.Cells["Caracteristica"].Value) == "RF");
            if (this.dgvSugeridos.CurrentRow == null || Util.Cadena(this.dgvSugeridos.CurrentRow.Cells["sug_Caracteristica"].Value) != "RF")
                e.Cancel = true;
        }

        private void tmiSugeridosQuitar_Click(object sender, EventArgs e)
        {
            if (this.dgvSugeridos.CurrentRow == null || Util.Cadena(this.dgvSugeridos.CurrentRow.Cells["sug_Caracteristica"].Value) != "RF") return;
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas quitar la sugerencia seleccionada?") == DialogResult.Yes)
            {
                this.QuitarSugerido(this.dgvSugeridos.CurrentRow);
                this.dgvSugeridos.Rows.Remove(this.dgvSugeridos.CurrentRow);
                this.sacarImporteTotal();
            }
        }
                
        private void dgvSucursales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvSucursales.CurrentRow == null || e.RowIndex < 0) return;

            if (this.dgvSucursales.Columns[e.ColumnIndex].Name == "Nivel")
            {
                DetalleNivel n = new DetalleNivel(Util.Entero(this.dgvSucursales.Rows[e.RowIndex].Cells["SucursalID"].Value));
                n.ShowDialog();
                this.CargarSucursales();
            }
        }

        private void btnImprimirSugerido_Click(object sender, EventArgs e)
        {
            try
            {
                // if (this.dgvSugeridos.DataSource == null) return;
                if (this.tabPedidos.SelectedIndex != 0) return;
                
                /*
                DataTable t = new DataTable();
                DataTable dt = new DataTable();

                BindingSource bs = (BindingSource)this.dgvSugeridos.DataSource;
                if (string.IsNullOrEmpty(bs.Filter))
                {
                    dt = (DataTable)bs.DataSource;
                    t = dt.AsEnumerable().Where(x => x.Field<Boolean>("Sel") && x.Field<decimal>("Pedido") > 0).CopyToDataTable();
                }
                else
                {
                    DataView dv = (DataView)bs.List;
                    dt = dv.ToTable();
                    t = dt.AsEnumerable().Where(x => x.Field<Boolean>("Sel") && x.Field<decimal>("Pedido") > 0).CopyToDataTable();
                }
                */

                // Se quitan los que no estén seleccionados y los que no tengan pedido
                var oPartes = new List<pauPedidosSugeridos_Res>();
                foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
                {
                    if (!oFila.Visible || !Util.Logico(oFila.Cells["sug_Sel"].Value) || Util.Decimal(oFila.Cells["sug_Pedido"].Value) <= 0)
                        continue;
                    oPartes.Add(new pauPedidosSugeridos_Res()
                    {
                        ParteID = Util.Entero(oFila.Cells["sug_ParteID"].Value),
                        NumeroParte = Util.Cadena(oFila.Cells["sug_NumeroDeParte"].Value),
                        NombreParte = Util.Cadena(oFila.Cells["sug_Descripcion"].Value),
                        UnidadEmpaque = Util.Decimal(oFila.Cells["sug_UnidadDeEmpaque"].Value),
                        CriterioABC = Util.Cadena(oFila.Cells["sug_AbcDeVentas"].Value),
                        NecesidadMatriz = Util.Decimal(oFila.Cells["sug_NecesidadMatriz"].Value),
                        NecesidadSuc02 = Util.Decimal(oFila.Cells["sug_NecesidadSuc02"].Value),
                        NecesidadSuc03 = Util.Decimal(oFila.Cells["sug_NecesidadSuc03"].Value),
                        Total = Util.Decimal(oFila.Cells["sug_Total"].Value),
                        Pedido = Util.Decimal(oFila.Cells["sug_Pedido"].Value),
                        CostoConDescuento = Util.Decimal(oFila.Cells["sug_CostoConDescuento"].Value),
                        Costo = Util.Decimal(oFila.Cells["sug_CostoTotal"].Value),
                        Observacion = Util.Cadena(oFila.Cells["sug_Observacion"].Value),
                    });
                }

                var oRep = new FastReport.Report();
                oRep.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReportePedidosSugeridos.frx"));
                oRep.RegisterData(oPartes, "PartesSugeridas");
                oRep.SetParameterValue("Proveedor", this.dgvProveedores.CurrentRow.Cells["pro_Proveedor"].Value);
                oRep.SetParameterValue("Usuario", GlobalClass.UsuarioGlobal.NombreUsuario);
                // oRep.GetDataSource("PartesSugeridas").Enabled = true;
                UtilLocal.EnviarReporteASalida("Reportes.Pedidos.Pedido", oRep);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnTodos_Click(object sender, EventArgs e)
        {
            // if (this.dgvSugeridos.DataSource == null) return;
            if (this.tabPedidos.SelectedIndex != 0) return;
            if (sel)
            {
                this.btnTodos.Text = "Sel Todos";
                sel = false;
                this.txtImporteTotal.Text = "0";
            }
            else
            {
                this.btnTodos.Text = "Sel Ninguno";
                sel = true;
            }

            if (this.dgvSugeridos.Columns.Contains("sug_Sel"))
            {
                foreach (DataGridViewRow row in this.dgvSugeridos.Rows)
                    if (sel)
                        row.Cells["sug_Sel"].Value = true;
                    else
                        row.Cells["sug_Sel"].Value = false;
            }
            this.sacarImporteTotal();
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            try
            {
                this.tabPedidos.SelectedIndex = 0; ;
                if (this.dgvProveedores.CurrentRow == null)
                {
                    Util.MensajeError("Debe seleccionar a un Proveedor.", GlobalClass.NombreApp);
                    return;
                }

                // Se valida que haya al menos una parte seleccionada
                bool bError = true;
                foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
                {
                    if (Util.Logico(oFila.Cells["sug_Sel"].Value))
                    {
                        bError = false;
                        break;
                    }
                }
                // if (this.dgvSugeridos.DataSource == null)
                if (bError)
                {
                    Util.MensajeError("Debe seleccionar al menos un número de parte.", GlobalClass.NombreApp);
                    return;
                }

                var proveedorId = Util.Entero(this.dgvProveedores.CurrentRow.Cells["pro_ProveedorID"].Value);
                var proveedor = Datos.GetEntity<Proveedor>(p => p.ProveedorID == proveedorId);
                if (proveedor != null)
                {
                    var msj = string.Format("{0} {1} {2}", "¿Está seguro de que la información del Pedido es correcta?", "Para el Proveedor:", proveedor.NombreProveedor);
                    var res = Util.MensajePregunta(msj, GlobalClass.NombreApp);
                    if (res == DialogResult.No)
                        return;
                }
                else
                {
                    Util.MensajeError("Proveedor Inválido.", GlobalClass.NombreApp);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                SplashScreen.Show(new Splash());
                this.btnProcesar.Enabled = false;

                var pedido = new Pedido()
                {
                    ProveedorID = proveedorId,
                    ImporteTotal = Util.Decimal(this.txtImporteTotal.Text.SoloNumeric()),
                    PedidoEstatusID = 2
                };

                Datos.Guardar<Pedido>(pedido);

                if (pedido.PedidoID <= 0)
                {
                    this.Cursor = Cursors.Default;
                    SplashScreen.Close();
                    Util.MensajeError("Ocurrio un error al guardar el Pedido.", GlobalClass.NombreApp);
                    this.btnProcesar.Enabled = true;
                    return;
                }

                // DataTable dt = new DataTable();
                // BindingSource bs = (BindingSource)this.dgvSugeridos.DataSource;

                //Validación, debe estar seleccionado y el pedido debe ser mayor a 0
                /* if (!string.IsNullOrEmpty(bs.Filter))
                {
                    DataView dv = (DataView)bs.List;
                    dt = dv.ToTable().AsEnumerable().Where(x => x.Field<Boolean>("Sel") && x.Field<decimal>("Pedido") > 0).CopyToDataTable();
                } */

                foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
                {
                    if (!oFila.Visible) continue;

                    int iParteID = Util.Entero(oFila.Cells["sug_ParteID"].Value);

                    // Se verifica si no se debe pedir por existencia en equivalentes
                    var oNoPedir = Datos.GetEntity<ParteCaracteristicaTemporal>(c => c.ParteID == iParteID
                        && c.Caracteristica == Cat.CaracTempPartes.NoPedidosPorEquivalentes);
                    if (Util.Cadena(oFila.Cells["sug_Caracteristica"].Value) == "NP")
                    {
                        if (oNoPedir == null)
                        {
                            oNoPedir = new ParteCaracteristicaTemporal()
                            {
                                ParteID = iParteID,
                                Caracteristica = Cat.CaracTempPartes.NoPedidosPorEquivalentes
                            };
                            Datos.Guardar<ParteCaracteristicaTemporal>(oNoPedir);
                        }
                    }
                    else
                    {
                        if (oNoPedir != null)
                            Datos.Eliminar<ParteCaracteristicaTemporal>(oNoPedir);
                    }

                    // Se verifica si está marcado o tiene pedidos para procesar, si no, se salta
                    if (!Util.Logico(oFila.Cells["sug_Sel"].Value) || Util.Decimal(oFila.Cells["sug_Pedido"].Value) <= 0)
                        continue;

                    if (Util.Entero(oFila.Cells["sug_ProveedorID"].Value) != proveedorId)
                    {
                        this.Cursor = Cursors.Default;
                        SplashScreen.Close();
                        var msj = string.Format("{0} {1} {2} {3}", "El número de Parte:", Util.Cadena(oFila.Cells["sug_NumeroDeParte"].Value)
                            , "No está asignado al Proveedor:", proveedor.NombreProveedor);
                        Util.MensajeError(msj, GlobalClass.NombreApp);
                        this.btnProcesar.Enabled = true;
                        return;
                    }
                                        
                    //
                    var detallePedido = new PedidoDetalle()
                    {
                        PedidoID = pedido.PedidoID,
                        ParteID = iParteID,
                        CantidadPedido = Util.Decimal(oFila.Cells["sug_Pedido"].Value),
                        CostosUnitario = Util.Decimal(oFila.Cells["sug_CostoConDescuento"].Value),
                        PedidoEstatusID = 2
                    };
                    Datos.Guardar<PedidoDetalle>(detallePedido);

                    // Se marca como pedido si es 9500
                    if (Util.Cadena(oFila.Cells["sug_Caracteristica"].Value) == "9500")
                    {
                        var o9500 = Datos.GetListOf<Cotizacion9500Detalle>(c => c.ParteID == iParteID && !c.Pedido && c.Estatus);
                        foreach (var oReg in o9500)
                        {
                            oReg.Pedido = true;
                            Datos.Guardar<Cotizacion9500Detalle>(oReg);
                        }
                    }

                    // Se marca como pedido en reporte de faltante, si aplica
                    if (Util.Cadena(oFila.Cells["sug_Caracteristica"].Value) == "RF")
                    {
                        var oFaltantes = Datos.GetListOf<ReporteDeFaltante>(c => c.ParteID == iParteID && !c.Pedido && c.Estatus);
                        foreach (var oReg in oFaltantes)
                        {
                            oReg.Pedido = true;
                            Datos.Guardar<ReporteDeFaltante>(oReg);
                        }
                    }
                }

                var ped = Datos.GetListOf<PedidosView>(p => p.PedidoID.Equals(pedido.PedidoID));
                var detalle = Datos.GetListOf<PedidosDetalleView>(p => p.PedidoID.Equals(pedido.PedidoID));

                IEnumerable<PedidosView> pedidoE = ped;
                IEnumerable<PedidosDetalleView> detalleE = detalle;

                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReportePedidos.frx"));
                    report.RegisterData(pedidoE, "Pedido");
                    report.GetDataSource("Pedido").Enabled = true;
                    report.RegisterData(detalleE, "DetallePedido");
                    report.GetDataSource("DetallePedido").Enabled = true;
                    report.Show(true);
                }

                this.Cursor = Cursors.Default;
                this.LimpiarFormularioDos();
                SplashScreen.Close();
                this.btnProcesar.Enabled = true;
                new Notificacion("Pedido Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                SplashScreen.Close();
                this.btnProcesar.Enabled = true;
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvExistencias_CurrentCellChanged(object sender, EventArgs e)
        {
            this.LlenarDescripcionMaxMin(this.dgvExistencias.CurrentRow);
        }

        private void dgvEquivalente_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvEquivalente.Focused && this.dgvEquivalente.VerSeleccionNueva())
            {
                if (this.dgvEquivalente.CurrentRow == null)
                    return;
                int iParteID = Util.Entero(this.dgvEquivalente.CurrentRow.Cells["eqParteID"].Value);
                // Se carga el grid de las existencias
                this.CargaExistencias(iParteID);
                // Se carga el grid de ventas por mes
                this.ctlVentasPorMes.LlenarDatos(iParteID);

                // Para cambiar el color de fondo de algunos grids
                this.dgvExistencias.CambiarColorDeFondo(Color.CadetBlue);
                this.ctlVentasPorMes.dgvDatos.CambiarColorDeFondo(Color.CadetBlue);
                // this.bGridPedidosSel = false;
            }
        }

        #endregion

        #region [ Pedidos ]

        #region [ Metodos ]

        private void cargarPedidos()
        {
            try
            {
                this.dgvPedidos.DataSource = null;
                var pedidos = new List<PedidosView>();
                var fechaInicial = this.dtpInicial.Value;
                var fechaFinal = this.dtpFinal.Value;

                if (this.chkMostrarTodos.Checked) //Todos
                {
                    pedidos = Datos.GetListOf<PedidosView>(p => p.Fecha >= fechaInicial && p.Fecha <= fechaFinal).ToList();
                }
                else //No surtidos                
                {
                    pedidos = Datos.GetListOf<PedidosView>(p => p.PedidoEstatusID == 2 && p.Fecha >= fechaInicial && p.Fecha <= fechaFinal).ToList();
                }

                this.dgvPedidos.DataSource = pedidos;
                Util.OcultarColumnas(this.dgvPedidos, new string[] { "ProveedorID", "Beneficiario", "ImporteTotal", "FechaRegistro", "PedidoEstatusID", "NombrePedidoEstatus" });
                UtilLocal.ColumnasToHeaderText(this.dgvPedidos);
                this.dgvPedidos.Columns["PedidoID"].HeaderText = "Pedido";
                this.dgvPedidos.Columns["Abreviacion"].HeaderText = "Estatus";
                this.dgvPedidos.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos ]

        private void tabPedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabPedidos.SelectedIndex == 1)
            {
                this.cargarPedidos();
            }
        }

        private void chkMostrarTodos_CheckedChanged(object sender, EventArgs e)
        {
            this.cargarPedidos();
        }

        private void dtpInicial_ValueChanged(object sender, EventArgs e)
        {
            this.cargarPedidos();
        }

        private void dtpFinal_ValueChanged(object sender, EventArgs e)
        {
            this.cargarPedidos();
        }

        private void dgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvPedidos.CurrentRow == null || e.RowIndex < 0) return;
            try
            {
                this.dgvDetallePedido.DataSource = null;
                var pedidoId = Util.Entero(this.dgvPedidos.Rows[e.RowIndex].Cells["PedidoID"].Value);
                this.dgvDetallePedido.DataSource = Datos.GetListOf<PedidosDetalleView>(p => p.PedidoID == pedidoId).ToList();
                Util.OcultarColumnas(this.dgvDetallePedido, new string[] { "PedidoDetalleID", "PedidoID", "ParteID", "PedidoEstatusID", "NombrePedidoEstatus", "Abreviacion", "CostosUnitario", "FechaRegistro", "Fecha" });
                UtilLocal.ColumnasToHeaderText(this.dgvDetallePedido);
                this.dgvDetallePedido.Columns["NombreParte"].Width = 400;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvPedidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvPedidos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                var res = Util.MensajePregunta("¿Está seguro de que desea Cancelar el Pedido?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        var pedidoId = Util.Entero(this.dgvPedidos.CurrentRow.Cells["PedidoID"].Value);
                        if (pedidoId > 0)
                        {
                            //Actualiza el pedidoEstatusID del pedido a Cancelado = 3
                            var pedido = Datos.GetEntity<Pedido>(p => p.PedidoID == pedidoId);
                            if (pedido != null)
                            {
                                pedido.PedidoEstatusID = 3;
                                Datos.Guardar<Pedido>(pedido);

                                //Actualiza el estatus del detalle a 0
                                var detalle = Datos.GetListOf<PedidoDetalle>(p => p.PedidoID == pedidoId).ToList();
                                foreach (var articulo in detalle)
                                {
                                    Datos.Eliminar<PedidoDetalle>(articulo, true);
                                    //General.SaveOrUpdate<PedidoDetalle>(articulo, articulo);
                                }
                            }
                            this.cargarPedidos();
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        #endregion
            
        #endregion
                
    }
}
