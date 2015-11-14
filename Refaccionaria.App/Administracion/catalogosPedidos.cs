﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class catalogosPedidos : UserControl
    {
        ControlError cntError = new ControlError();
        BindingSource fuenteDatos;
        DataTable dtProveedores = new DataTable();
        DataTable dtPedidos = new DataTable();
        bool sel = true;

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
                var oGuardados = General.GetListOf<PedidoFiltro>(c => c.Tabla == Cat.Tablas.Marcas || c.Tabla == Cat.Tablas.Lineas);
                var oParams = new Dictionary<string, object>();
                oParams.Add("Pagadas", true);
                oParams.Add("Cobradas", false);
                oParams.Add("Solo9500", false);
                oParams.Add("OmitirDomingo", false);
                oParams.Add("Desde", DateTime.Now.AddMonths(-6));
                oParams.Add("Hasta", DateTime.Now);
                var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
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
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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

                this.dgvExistencias.DataSource = null;

                this.txtImporteTotal.Clear();
                this.txtImporteTotalDos.Clear();
                this.progreso.Value = 0;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void configurarGridProveedor()
        {
            try
            {
                dtProveedores.Clear();
                this.dgvProveedores.Refresh();
                if (this.dgvProveedores.RowCount > 0)
                    this.dgvProveedores.Rows.Clear();

                if (this.dgvProveedores.Columns.Count > 0)
                    this.dgvProveedores.Columns.Clear();

                this.dgvProveedores.DataSource = null;
                dtProveedores = new DataTable();

                var colProveedorId = new DataColumn();
                colProveedorId.DataType = System.Type.GetType("System.Int32");
                colProveedorId.ColumnName = "ProveedorID";

                var colNombreProveedor = new DataColumn();
                colNombreProveedor.DataType = Type.GetType("System.String");
                colNombreProveedor.ColumnName = "Nombre Proveedor";

                var colImporte = new DataColumn();
                colImporte.DataType = Type.GetType("System.Decimal");
                colImporte.ColumnName = "Importe";

                var colPct = new DataColumn();
                colPct.DataType = Type.GetType("System.Decimal");
                colPct.ColumnName = "PCT";

                var colCaracteristica = new DataColumn();
                colCaracteristica.DataType = Type.GetType("System.String");
                colCaracteristica.ColumnName = "Caracteristica";

                /* var colPctPedido = new DataColumn();
                colPctPedido.DataType = Type.GetType("System.String");
                colPctPedido.ColumnName = "Pct Pedido";
                */

                dtProveedores.Columns.AddRange(new DataColumn[] { colProveedorId, colNombreProveedor, colImporte, colPct, colCaracteristica });

                this.dgvProveedores.DataSource = dtProveedores;
                Helper.OcultarColumnas(this.dgvProveedores, new string[] { "ProveedorID" });
                // Helper.FormatoDecimalColumnas(this.dgvProveedores, new string[] { "Importe" });
                this.dgvProveedores.Columns["Importe"].FormatoMoneda();
                this.dgvProveedores.Columns["PCT"].DefaultCellStyle.Format = GlobalClass.FormatoPorcentaje;
                
                this.dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                foreach (DataGridViewColumn column in this.dgvProveedores.Columns)
                {
                    // column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    column.ReadOnly = true;
                }

                this.dgvProveedores.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvProveedores.BackgroundColor = Color.FromArgb(188, 199, 216);
                this.dgvProveedores.ClearSelection();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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

                var sucursales = Helper.newTable<SucursalesCriterioAbcView>("Sucursales", General.GetListOf<SucursalesCriterioAbcView>());

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

                Helper.OcultarColumnas(this.dgvSucursales, new string[] { "SucursalID", "ids", "EntityKey", "EntityState" });
                Helper.ColumnasToHeaderText(this.dgvSucursales);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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

                this.dgvExistencias.DataSource = General.GetListOf<ExistenciasView>(ex => ex.ParteID.Equals(parteId));
                this.dgvExistencias.AutoResizeColumns();
                Negocio.Helper.OcultarColumnas(this.dgvExistencias, new string[] { "ParteExistenciaID", "ParteID", "NumeroParte", "SucursalID" });
                Negocio.Helper.ColumnasToHeaderText(this.dgvExistencias);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void sacarImporteTotal()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
            {
                if (!oFila.Visible || !Helper.ConvertirBool(oFila.Cells["Sel"].Value)) continue;
                mTotal += Helper.ConvertirDecimal(oFila.Cells["Costo Total"].Value);
            }
            this.txtImporteTotal.Text = mTotal.ToString(GlobalClass.FormatoMoneda);

            /* Se modifica la forma de calcular el total, pues se encontraban diferencias al seleccionar y des-seleccionar todos - Moi 2015-09-01
            try
            {
                decimal importeTotal = dgvSugeridos.Rows.OfType<DataGridViewRow>().Where(c => Helper.ConvertirCadena(c.Cells["Caracteristica"].Value) != "NP")
                    .Sum(row => Helper.ConvertirDecimal(row.Cells["Costo Total"].Value));
                var importeNoSeleccionados = 0.0M;

                /* No se xq se calculaba el importe de lo no seleccionado, se modifica para q sólo se considere el importe de lo seleccionado - Moi 2015-09-01
                foreach (DataGridViewRow fila in this.dgvSugeridos.Rows)
                {
                    if (!fila.Visible) continue;

                    DataGridViewCheckBoxCell cb = (DataGridViewCheckBoxCell)fila.Cells["Sel"];
                    if (cb.Value != null)
                    {
                        if (!Helper.ConvertirBool(cb.Value))
                        {
                            importeNoSeleccionados += Helper.ConvertirDecimal(fila.Cells["Costo Total"].Value);
                        }
                    }
                }
                * /

                this.txtImporteTotal.Text = Helper.DecimalToCadenaMoneda(importeTotal - importeNoSeleccionados);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                if (Helper.ConvertirBool(oFila.Cells["X"].Value))
                    oSucursales.Rows.Add(Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value));
            }
            if (oSucursales.Rows.Count == 0)
            {
                Helper.MensajeAdvertencia("Debe seleccionar al menos una sucursal", GlobalClass.NombreApp);
                return;
            }

            Cargando.Mostrar();
            this.btnMostrar.Enabled = false;
            this.tabPedidos.SelectedIndex = 0;
            var oLog = new Log();
            oLog.Show();
            oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Obteniendo datos.");  // dPend

            // Se obtienen los parámetros adicionales
            var dic = new Dictionary<string, object>();
            dic.Add("Sucursales/tpuTablaEnteros", oSucursales);

            // Se agregan los parámetros de línea y marca
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
            {
                var oDataTable = Helper.ListaEntityADataTable(this.ctlMarcas.ElementosSeleccionados);
                oDataTable.Columns.Remove("Cadena");
                dic.Add("Marcas/tpuTablaEnteros", oDataTable);
            }
            if (this.ctlLineas.ValoresSeleccionados.Count > 0)
            {
                var oDataTable = Helper.ListaEntityADataTable(this.ctlLineas.ElementosSeleccionados);
                oDataTable.Columns.Remove("Cadena");
                dic.Add("Lineas/tpuTablaEnteros", oDataTable);
            }

            // Se obtienen los datos
            var oSugeridos = General.ExecuteProcedure<pauPedidosSugeridos_Result>("pauPedidosSugeridos", dic);

            // Se comienzan a llenar los datos
            oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Llenando sugeridos.");  // dPend

            //     
            var selMatriz = false;
            var selSuc02 = false;
            var selSuc03 = false;
            var nivelMatriz = string.Empty;
            var nivelSuc02 = string.Empty;
            var nivelSuc03 = string.Empty;
            foreach (DataGridViewRow fila in this.dgvSucursales.Rows)
            {
                if (Helper.ConvertirBool(fila.Cells["X"].Value) == true && Helper.ConvertirCadena(fila.Cells["NombreSucursal"].Value).ToUpper() == "MATRIZ")
                {
                    selMatriz = true;
                    nivelMatriz = Helper.ConvertirCadena(fila.Cells["Nivel"].Value);
                }
                if (Helper.ConvertirBool(fila.Cells["X"].Value) == true && Helper.ConvertirCadena(fila.Cells["NombreSucursal"].Value).ToUpper() == "SUC02")
                {
                    selSuc02 = true;
                    nivelSuc02 = Helper.ConvertirCadena(fila.Cells["Nivel"].Value);
                }
                if (Helper.ConvertirBool(fila.Cells["X"].Value) == true && Helper.ConvertirCadena(fila.Cells["NombreSucursal"].Value).ToUpper() == "SUC03")
                {
                    selSuc03 = true;
                    nivelSuc03 = Helper.ConvertirCadena(fila.Cells["Nivel"].Value);
                }
            }

            // Se comienzan a llenar los datos
            foreach (var oReg in oSugeridos)
            {
                oReg.NecesidadMatriz = (selMatriz ? oReg.NecesidadMatriz : 0);
                oReg.NecesidadSuc02 = (selSuc02 ? oReg.NecesidadSuc02 : 0);
                oReg.NecesidadSuc03 = (selSuc03 ? oReg.NecesidadSuc03 : 0);

                //Si la el criterio 'A' no se encuentra en el campo Nivel de cada sucursal, entonces ponener la necesidad de c/sucurasal en 0                
                /* Duda aquí, preguntar
                foreach (DataRow fila in dtPedidos.Rows)
                {
                    var criterio = Helper.ConvertirCadena(fila["CriterioABC"]);
                    if (!string.IsNullOrEmpty(criterio) && !dtPedidos.Columns["CriterioABC"].ReadOnly)
                    {
                        if (!nivelMatriz.Contains(criterio) && !dtPedidos.Columns["NecesidadMatriz"].ReadOnly)
                            fila["NecesidadMatriz"] = 0;

                        if (!nivelSuc02.Contains(criterio) && !dtPedidos.Columns["NecesidadSuc02"].ReadOnly)
                            fila["NecesidadSuc02"] = 0;

                        if (!nivelSuc03.Contains(criterio) && !dtPedidos.Columns["NecesidadSuc03"].ReadOnly)
                            fila["NecesidadSuc03"] = 0;
                    }
                }
                */

                decimal mTotal = (oReg.NecesidadMatriz + oReg.NecesidadSuc02 + oReg.NecesidadSuc03).Valor();
                decimal mPedido = ((int)(((mTotal / oReg.UnidadEmpaque.Valor()) + 0.4M)) * oReg.UnidadEmpaque).Valor();
                oReg.Costo = (mPedido * oReg.CostoConDescuento);

                int iFila = this.dgvSugeridos.Rows.Add(oReg.ParteID, oReg.ProveedorID, true, oReg.NumeroParte, oReg.NombreParte, oReg.UnidadEmpaque, oReg.CriterioABC
                    , oReg.NecesidadMatriz, oReg.NecesidadSuc02, oReg.NecesidadSuc03, oReg.Total, mPedido, oReg.CostoConDescuento, oReg.Costo);
                var oFila = this.dgvSugeridos.Rows[iFila];

                // Se colorean las filas
                switch (oReg.Caracteristica)
                {
                    case "9500": oFila.Cells["sug_Descripcion"].Style.ForeColor = Color.Blue; break;
                    case "RF": oFila.Cells["sug_Descripcion"].Style.ForeColor = Color.Red; break;
                }
            }

            // Se obtienen los datos para lo de proveedores
            oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Llenando proveedores.");  // dPend
            var oProveedores = oSugeridos.GroupBy(c => c.ProveedorID).Select(c => new
            {
                ProveedorID = c.Key,
                Proveedor = c.Max(s => s.NombreProveedor),
                Importe = c.Sum(s => s.Costo),
                Caracteristica = c.Max(s => s.Caracteristica)
            }).OrderByDescending(c => c.Importe);
            decimal mTotalProv = oProveedores.Sum(c => c.Importe).Valor();
            // Se llena el grid de proveedores
            foreach (var oReg in oProveedores)
            {
                int iFila = this.dgvProveedores.Rows.Add(oReg.ProveedorID, oReg.Proveedor, oReg.Importe, (oReg.Importe / mTotalProv), oReg.Caracteristica);
                var oFila = this.dgvProveedores.Rows[iFila];
                // Se colorean las filas
                switch (oReg.Caracteristica)
                {
                    case "9500": oFila.Cells["pro_Proveedor"].Style.ForeColor = Color.Blue; break;
                    case "RF": oFila.Cells["pro_Proveedor"].Style.ForeColor = Color.Red; break;
                }
            }

            // Se colorean algunas filas, según el caso
            // oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Coloreando.");  // dPend
            // this.ColorearSugeridos(true);

            // Se calcula el presupuesto
            oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Calculando presupuesto.");  // dPend
            this.CalcularPresupuesto();
            
            // Se guardan las líneas y marcas seleccionadas
            oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Guardando filtro.");  // dPend
            this.GuardarFiltro();

            //
            this.btnMostrar.Enabled = true;
            oLog.finalizo = true;
            oLog.AppendTextBox(DateTime.Now.ToString(GlobalClass.FormatoFechaHora) + ": Finalizó.");  // dPend
            Cargando.Cerrar();
        }

        private void ColorearSugeridos(bool bColorearGridProveedores)
        {
            /* Ya no se ocultan las partes NP (partes que no tienen existencia pero tienen equivalentes que sí), pues se notó que esa funcionalidad sí aplica
             * pero sólo para ciertos casos, no para todos. Se revisará más a detalle posteriormente. Moi - 27/07/2015
            this.dgvSugeridos.CurrentCell = null;
            foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
                oFila.Visible = (Helper.ConvertirCadena(oFila.Cells["Caracteristica"].Value) != "NP");
            */

            foreach (DataGridViewRow oFila in this.dgvSugeridos.Rows)
            {
                // Se ocultan las filas de partes que no deben ser pedidas por existencia en sus equivalentes
                // oFila.Visible = (Helper.ConvertirCadena(oFila.Cells["Caracteristica"].Value) != "NP");
                /* if (Helper.ConvertirCadena(oFila.Cells["Caracteristica"].Value) != "NP")
                {
                    //this.dgvSugeridos.CurrentCell = null;
                    oFila.Visible = false;
                } */
                
                // Coloración
                /* Color oColor = Color.Black;
                switch (Helper.ConvertirCadena(oFila.Cells["Caracteristica"].Value))
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
                        int iFilaP = this.dgvProveedores.EncontrarIndiceDeValor("ProveedorID", Helper.ConvertirEntero(oFila.Cells["ProveedorID"].Value));
                        this.dgvProveedores["Nombre Proveedor", iFilaP].Style.ForeColor = oColor;
                    }
                }
                */

                switch (Helper.ConvertirCadena(oFila.Cells["Caracteristica"].Value))
                {
                    case "9500": oFila.Cells["NombreParte"].Style.ForeColor = Color.Blue; break;
                    case "RF": oFila.Cells["NombreParte"].Style.ForeColor = Color.Red; break;
                }
            }

            // Se colorean las filas de proveedores
            if (bColorearGridProveedores)
            {
                foreach (DataGridViewRow oFila in this.dgvProveedores.Rows)
                {
                    switch (Helper.ConvertirCadena(oFila.Cells["Caracteristica"].Value))
                    {
                        case "9500": oFila.Cells["Nombre Proveedor"].Style.ForeColor = Color.Blue; break;
                        case "RF": oFila.Cells["Nombre Proveedor"].Style.ForeColor = Color.Red; break;
                    }
                }
            }
        }

        private void CargarEquivalentes(int parteId)
        {
            var equivalentes = General.GetListOf<PartesEquivalentesView>(pe => pe.ParteID == parteId);
            this.dgvEquivalente.Rows.Clear();   
            foreach (var equivalente in equivalentes)
            {
                this.dgvEquivalente.Rows.Add(equivalente.ParteID, equivalente.NumeroParte, equivalente.Descripcion
                    , equivalente.CostoConDescuento, equivalente.Matriz, equivalente.Suc02, equivalente.Suc03);
            }
        }

        private void QuitarSugerido(DataGridViewRow oFila)
        {
            int iParteID = Helper.ConvertirEntero(oFila.Cells["ParteID"].Value);
            for (int iCont = 1; iCont < 4; iCont++)
            {
                var oReporteF = General.GetEntity<ReporteDeFaltante>(c => c.ParteID == iParteID && c.SucursalID == iCont && !c.Pedido && c.Estatus);
                if (oReporteF != null)
                    Guardar.Eliminar<ReporteDeFaltante>(oReporteF, true);
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
                switch (Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value))
                {
                    case 1: bSucursal1 = Helper.ConvertirBool(oFila.Cells["X"].Value); break;
                    case 2: bSucursal2 = Helper.ConvertirBool(oFila.Cells["X"].Value); break;
                    case 3: bSucursal3 = Helper.ConvertirBool(oFila.Cells["X"].Value); break;
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
            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            decimal mVendido = oDatos.Sum(c => c.CostoActual).Valor();

            var oPedidos = General.GetListOf<Pedido>(c => c.FechaRegistro >= oFechas.Valor1 && c.FechaRegistro <= oFechas.Valor2 && c.Estatus);
            decimal mPedido = (oPedidos.Count > 0 ? oPedidos.Sum(c => c.ImporteTotal) : 0);

            // Se aplica el incremento de configuración, si hay
            decimal mPor = Helper.ConvertirDecimal(Config.Valor("Pedidos.PorcentajeIncrementoPresupuesto"));
            mVendido *= (1 + (mPor / 100));

            this.txtPresupuesto.Text = mVendido.ToString(GlobalClass.FormatoMoneda);
            this.txtGastado.Text = mPedido.ToString(GlobalClass.FormatoMoneda);
            this.txtPorGastar.Text = (mVendido - mPedido).ToString(GlobalClass.FormatoMoneda);
        }

        private void GuardarFiltro()
        {
            // Se guardan las marcas
            var oGuardados = General.GetListOf<PedidoFiltro>(c => c.Tabla == Cat.Tablas.Marcas);
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
                Guardar.Generico<PedidoFiltro>(oReg);
                oGuardados.Remove(oReg);
            }
            // Se guardan las que antes estaba y ahora no fueron marcadas
            foreach (var oReg in oGuardados)
            {
                oReg.Seleccion = false;
                Guardar.Generico<PedidoFiltro>(oReg);
            }

            // Se procede a guardar las líneas
            oGuardados = General.GetListOf<PedidoFiltro>(c => c.Tabla == Cat.Tablas.Lineas);
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
                Guardar.Generico<PedidoFiltro>(oReg);
                oGuardados.Remove(oReg);
            }
            // Se guardan las que antes estaba y ahora no fueron marcadas
            foreach (var oReg in oGuardados)
            {
                oReg.Seleccion = false;
                Guardar.Generico<PedidoFiltro>(oReg);
            }
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
            this.fuenteDatos.RemoveFilter();
            this.ColorearSugeridos(true);
        }

        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvProveedores.CurrentRow == null || e.RowIndex < 0) return;
            /*string filter = string.Empty;
            fuenteDatos.Filter = filter;
            filter = "NombreProveedor like '%" + Helper.ConvertirCadena(this.dgvProveedores.Rows[e.RowIndex].Cells["Nombre Proveedor"].Value) + "%'";
            if (fuenteDatos != null)
                fuenteDatos.Filter = filter;
            this.ColorearSugeridos(false);
            */

            int iProveedorID = Helper.ConvertirEntero(this.dgvProveedores.CurrentRow.Cells["pro_ProveedorID"].Value);
            this.dgvSugeridos.FiltrarIgual(iProveedorID, "sug_ProveedorID");

            this.sacarImporteTotal();
        }

        private void dgvSugeridos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvSugeridos.CurrentRow == null) return;
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                var fila = dgvSugeridos.CurrentRow.Index;
                if (Helper.ConvertirBool(dgvSugeridos.Rows[fila].Cells["Sel"].Value).Equals(true))
                    dgvSugeridos.Rows[fila].Cells["Sel"].Value = false;
                else
                    dgvSugeridos.Rows[fila].Cells["Sel"].Value = true;
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
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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

        private void dgvSugeridos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvSugeridos.CurrentRow == null)
                return;
            if (e.RowIndex > -1)
            {
                int parteId = Helper.ConvertirEntero(this.dgvSugeridos.Rows[e.RowIndex].Cells["ParteID"].Value);
                this.CargaExistencias(parteId);

                this.CargarEquivalentes(parteId);

                // Se llena la observación, para cuando es un reporte de faltante
                
                this.txtSugerenciaObservacion.Text = Helper.ConvertirCadena(this.dgvSugeridos["Observacion", e.RowIndex].Value);

                // Se cargan los datos de las ventas para la parte seleccionada
                this.ctlVentasPorMes.LlenarDatos(parteId);
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
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvSugeridos_Sorted(object sender, EventArgs e)
        {
            this.ColorearSugeridos(false);
        }

        private void cmsSugeridos_Opening(object sender, CancelEventArgs e)
        {
            //this.cmsSugeridos.Items["tmiSugeridosQuitar"].Enabled = false;// (Helper.ConvertirCadena(this.dgvSugeridos.CurrentRow.Cells["Caracteristica"].Value) == "RF");
            //this.tmiSugeridosQuitar.Visible = (Helper.ConvertirCadena(this.dgvSugeridos.CurrentRow.Cells["Caracteristica"].Value) == "RF");
            if (this.dgvSugeridos.CurrentRow == null || Helper.ConvertirCadena(this.dgvSugeridos.CurrentRow.Cells["Caracteristica"].Value) != "RF")
                e.Cancel = true;
        }

        private void tmiSugeridosQuitar_Click(object sender, EventArgs e)
        {
            if (this.dgvSugeridos.CurrentRow == null || Helper.ConvertirCadena(this.dgvSugeridos.CurrentRow.Cells["Caracteristica"].Value) != "RF") return;
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
                DetalleNivel n = new DetalleNivel(Helper.ConvertirEntero(this.dgvSucursales.Rows[e.RowIndex].Cells["SucursalID"].Value));
                n.ShowDialog();
                this.CargarSucursales();
            }
        }

        private void btnImprimirSugerido_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvSugeridos.DataSource == null) return;
                if (this.tabPedidos.SelectedIndex != 0) return;
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
                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReportePedidosSugeridos.frx"));
                    report.RegisterData(t, "PartesSugeridas");
                    report.GetDataSource("PartesSugeridas").Enabled = true;
                    report.Show(true);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnTodos_Click(object sender, EventArgs e)
        {
            if (this.dgvSugeridos.DataSource == null) return;
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

            if (this.dgvSugeridos.Columns.Contains("Sel"))
            {
                foreach (DataGridViewRow row in this.dgvSugeridos.Rows)
                    if (sel)
                        row.Cells["Sel"].Value = true;
                    else
                        row.Cells["Sel"].Value = false;
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
                    Helper.MensajeError("Debe seleccionar a un Proveedor.", GlobalClass.NombreApp);
                    return;
                }

                if (this.dgvSugeridos.DataSource == null)
                {
                    Helper.MensajeError("Debe seleccionar al menos un número de parte.", GlobalClass.NombreApp);
                    return;
                }

                var proveedorId = Helper.ConvertirEntero(this.dgvProveedores.CurrentRow.Cells["ProveedorID"].Value);
                var proveedor = General.GetEntity<Proveedor>(p => p.ProveedorID == proveedorId);
                if (proveedor != null)
                {
                    var msj = string.Format("{0} {1} {2}", "¿Está seguro de que la información del Pedido es correcta?", "Para el Proveedor:", proveedor.NombreProveedor);
                    var res = Helper.MensajePregunta(msj, GlobalClass.NombreApp);
                    if (res == DialogResult.No)
                        return;
                }
                else
                {
                    Helper.MensajeError("Proveedor Inválido.", GlobalClass.NombreApp);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                SplashScreen.Show(new Splash());
                this.btnProcesar.Enabled = false;

                var pedido = new Pedido()
                {
                    ProveedorID = proveedorId,
                    ImporteTotal = Helper.ConvertirDecimal(this.txtImporteTotal.Text.SoloNumeric()),
                    PedidoEstatusID = 2
                };

                Guardar.Generico<Pedido>(pedido);

                if (pedido.PedidoID <= 0)
                {
                    this.Cursor = Cursors.Default;
                    SplashScreen.Close();
                    Helper.MensajeError("Ocurrio un error al guardar el Pedido.", GlobalClass.NombreApp);
                    this.btnProcesar.Enabled = true;
                    return;
                }

                // DataTable dt = new DataTable();
                BindingSource bs = (BindingSource)this.dgvSugeridos.DataSource;

                //Validación, debe estar seleccionado y el pedido debe ser mayor a 0
                /* if (!string.IsNullOrEmpty(bs.Filter))
                {
                    DataView dv = (DataView)bs.List;
                    dt = dv.ToTable().AsEnumerable().Where(x => x.Field<Boolean>("Sel") && x.Field<decimal>("Pedido") > 0).CopyToDataTable();
                } */

                var oTabla = this.dgvSugeridos.ADataTable();
                foreach (DataRow fila in oTabla.Rows)
                {
                    int iParteID = Helper.ConvertirEntero(fila["ParteID"]);

                    // Se verifica si no se debe pedir por existencia en equivalentes
                    var oNoPedir = General.GetEntity<ParteCaracteristicaTemporal>(c => c.ParteID == iParteID
                        && c.Caracteristica == Cat.CaracTempPartes.NoPedidosPorEquivalentes);
                    if (Helper.ConvertirCadena(fila["Caracteristica"]) == "NP")
                    {
                        if (oNoPedir == null)
                        {
                            oNoPedir = new ParteCaracteristicaTemporal()
                            {
                                ParteID = iParteID,
                                Caracteristica = Cat.CaracTempPartes.NoPedidosPorEquivalentes
                            };
                            Guardar.Generico<ParteCaracteristicaTemporal>(oNoPedir);
                        }
                    }
                    else
                    {
                        if (oNoPedir != null)
                            Guardar.Eliminar<ParteCaracteristicaTemporal>(oNoPedir);
                    }

                    // Se verifica si está marcado o tiene pedidos para procesar, si no, se salta
                    if (!Helper.ConvertirBool(fila["Sel"]) || Helper.ConvertirDecimal(fila["Pedido"]) <= 0)
                        continue;

                    if (Helper.ConvertirEntero(fila["ProveedorID"]) != proveedorId)
                    {
                        this.Cursor = Cursors.Default;
                        SplashScreen.Close();
                        var msj = string.Format("{0} {1} {2} {3}", "El número de Parte:", Helper.ConvertirCadena(fila["NumeroParte"]), "No está asignado al Proveedor:", proveedor.NombreProveedor);
                        Helper.MensajeError(msj, GlobalClass.NombreApp);
                        this.btnProcesar.Enabled = true;
                        return;
                    }
                                        
                    //
                    var detallePedido = new PedidoDetalle()
                    {
                        PedidoID = pedido.PedidoID,
                        ParteID = iParteID,
                        CantidadPedido = Helper.ConvertirDecimal(fila["Pedido"]),
                        CostosUnitario = Helper.ConvertirDecimal(fila["CostoConDescuento"]),
                        PedidoEstatusID = 2
                    };
                    Guardar.Generico<PedidoDetalle>(detallePedido);

                    // Se marca como pedido en reporte de faltante, si aplica
                    if (Helper.ConvertirCadena(fila["Caracteristica"]) == "RF")
                    {
                        var oFaltantes = General.GetListOf<ReporteDeFaltante>(c => c.ParteID == iParteID && !c.Pedido && c.Estatus);
                        foreach (var oReg in oFaltantes)
                        {
                            oReg.Pedido = true;
                            Guardar.Generico<ReporteDeFaltante>(oReg);
                        }
                    }
                }

                var ped = General.GetListOf<PedidosView>(p => p.PedidoID.Equals(pedido.PedidoID));
                var detalle = General.GetListOf<PedidosDetalleView>(p => p.PedidoID.Equals(pedido.PedidoID));

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
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                    pedidos = General.GetListOf<PedidosView>(p => p.Fecha >= fechaInicial && p.Fecha <= fechaFinal).ToList();
                }
                else //No surtidos                
                {
                    pedidos = General.GetListOf<PedidosView>(p => p.PedidoEstatusID == 2 && p.Fecha >= fechaInicial && p.Fecha <= fechaFinal).ToList();
                }

                this.dgvPedidos.DataSource = pedidos;
                Helper.OcultarColumnas(this.dgvPedidos, new string[] { "ProveedorID", "Beneficiario", "ImporteTotal", "FechaRegistro", "PedidoEstatusID", "NombrePedidoEstatus" });
                Helper.ColumnasToHeaderText(this.dgvPedidos);
                this.dgvPedidos.Columns["PedidoID"].HeaderText = "Pedido";
                this.dgvPedidos.Columns["Abreviacion"].HeaderText = "Estatus";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                var pedidoId = Helper.ConvertirEntero(this.dgvPedidos.Rows[e.RowIndex].Cells["PedidoID"].Value);
                this.dgvDetallePedido.DataSource = General.GetListOf<PedidosDetalleView>(p => p.PedidoID == pedidoId).ToList();
                Helper.OcultarColumnas(this.dgvDetallePedido, new string[] { "PedidoDetalleID", "PedidoID", "ParteID", "PedidoEstatusID", "NombrePedidoEstatus", "Abreviacion", "CostosUnitario", "FechaRegistro", "Fecha" });
                Helper.ColumnasToHeaderText(this.dgvDetallePedido);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvPedidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvPedidos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                var res = Helper.MensajePregunta("¿Está seguro de que desea Cancelar el Pedido?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        var pedidoId = Helper.ConvertirEntero(this.dgvPedidos.CurrentRow.Cells["PedidoID"].Value);
                        if (pedidoId > 0)
                        {
                            //Actualiza el pedidoEstatusID del pedido a Cancelado = 3
                            var pedido = General.GetEntity<Pedido>(p => p.PedidoID == pedidoId);
                            if (pedido != null)
                            {
                                pedido.PedidoEstatusID = 3;
                                Guardar.Generico<Pedido>(pedido);

                                //Actualiza el estatus del detalle a 0
                                var detalle = General.GetListOf<PedidoDetalle>(p => p.PedidoID == pedidoId).ToList();
                                foreach (var articulo in detalle)
                                {
                                    Guardar.Eliminar<PedidoDetalle>(articulo, true);
                                    //General.SaveOrUpdate<PedidoDetalle>(articulo, articulo);
                                }
                            }
                            this.cargarPedidos();
                        }
                    }
                    catch (Exception ex)
                    {
                        Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        #endregion

        private void dgvSugeridos_DragOver(object sender, DragEventArgs e)
        {

        }
                                                
        #endregion
                
    }
}
