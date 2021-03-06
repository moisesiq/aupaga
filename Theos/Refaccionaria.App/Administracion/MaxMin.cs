﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Text;
using FastReport;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class MaxMin : UserControl
    {
        DateTime Desde, Hasta;
        Dictionary<int, int> MesesEtiquetas = new Dictionary<int, int>();
        Dictionary<string, bool> CgCambiosCalcular = new Dictionary<string, bool>();
        Dictionary<string, bool> CgCambiosVentasGlobales = new Dictionary<string, bool>();
        DataGridViewRow DetalleUltimaFilaSel;

        public MaxMin()
        {
            InitializeComponent();

            this.dgvEquivalentes.EstablecerSortModeEnColumnas(DataGridViewColumnSortMode.NotSortable);
        }

        #region [ Eventos ]

        private void MaxMin_Load(object sender, EventArgs e)
        {
            // Se cargan los combos
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(q => q.Estatus));
            this.cmbSucursal.SelectedValue = Cat.Sucursales.Matriz;
            // this.cmbProveedor.CargarDatos("ProveedorID", "NombreProveedor", General.GetListOf<Proveedor>(q => q.Estatus));
            var oProveedores = Datos.GetListOf<Proveedor>(c => c.Estatus).OrderBy(c => c.NombreProveedor);
            foreach (var oReg in oProveedores)
                this.ctlProveedores.AgregarElemento(oReg.ProveedorID, oReg.NombreProveedor);
            // this.cmbMarca.CargarDatos("MarcaParteID", "NombreMarcaParte", General.GetListOf<MarcaParte>(q => q.Estatus));
            var oMarcas = Datos.GetListOf<MarcaParte>(c => c.Estatus).OrderBy(c => c.NombreMarcaParte);
            foreach (var oReg in oMarcas)
                this.ctlMarcas.AgregarElemento(oReg.MarcaParteID, oReg.NombreMarcaParte);
            // this.cmbLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(q => q.Estatus));
            var oLineas = Datos.GetListOf<Linea>(c => c.Estatus).OrderBy(c => c.NombreLinea);
            foreach (var oReg in oLineas)
                this.ctlLineas.AgregarElemento(oReg.LineaID, oReg.NombreLinea);
            //
            this.cmbCambios.Items.AddRange(new object[] { "0 a > 0", "> 0 a > Actual", "Actual baja a > 0 ", "Max a 0", "Max igual", "Fijos" });
            this.cmbFiltroDeVentas.Items.AddRange(new object[] { "Todas", "12 meses", "Año ant. 1", "Año ant. 2", "Año ant. 3" });
            this.cmbFiltroDeVentas.Text = "12 Meses";

            // Se configura el grid de detalle
            this.dgvDetalle.Columns["NumeroDeParte"].ValueType = typeof(string);
            this.dgvDetalle.Columns["Descripcion"].ValueType = typeof(string);
            this.dgvDetalle.Columns["Proveedor"].ValueType = typeof(string);
            this.dgvDetalle.Columns["Linea"].ValueType = typeof(string);
            this.dgvDetalle.Columns["Marca"].ValueType = typeof(string);
            //this.dgvDetalle.Columns["Id"].ValueType = typeof(string);

            this.Hasta = DateTime.Now.Date.DiaPrimero().AddDays(-1);
            this.Desde = this.Hasta.AddYears(-1).AddDays(1);
            // Se limpian los datos extra
            this.LimpiarDatosExtra();
            // Se llenan las etiquetas de los meses
            /* for (int iCont = 0; iCont < 12; iCont++)
            {
                DateTime dMes = this.Hasta.AddMonths(iCont * -1);
                string sEtiqueta = string.Format("lblEtMes{0:00}", (12 - iCont));
                this.Controls[sEtiqueta].Text =
                    CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dMes.Month).ToUpper()
                    + dMes.Year.ToString().Derecha(2);
                this.MesesEtiquetas[dMes.Month] = (12 - iCont);
            }
            */

            this.trvVentasGlobales.AfterCheck += new TreeViewEventHandler(this.trvPartesCalcular_AfterCheck);
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabMaxMin.SelectedTab.Name)
            {
                case "tbpDetalle":
                    this.dgvDetalle.Rows.Clear();
                    break;
                case "tbpCriteriosGenerales":
                    this.LlenarCriteriosGenerales();
                    break;
                case "tbpReglas":
                    this.LlenarReglas();
                    break;
            }
        }

        private void cmbCambios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbCambios.Focused)
            {
                if (this.cmbCambios.Text == "Fijos")
                    this.ProcesarFijos();
                else
                    this.FiltroDeCambios();
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            switch (this.tabMaxMin.SelectedTab.Name)
            {
                case "tbpDetalle":
                    this.MostrarDetalle();
                    break;
                case "tbpEquivalentes":
                    this.LlenarEquivalentes();
                    break;
                case "tbpIman":
                    this.LlenarPartesIman();
                    break;
            }
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            switch (this.tabMaxMin.SelectedTab.Name)
            {
                case "tbpDetalle":
                    this.CalcularMaxMin();
                    break;
                case "tbpEquivalentes":
                    this.ProcesarEquivalentes();
                    break;
            }
        }

        private void tabMaxMin_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabMaxMin.SelectedTab.Name)
            {
                case "tbpCriteriosGenerales":
                    if (this.trvPartesCalcular.Nodes.Count <= 0)
                        this.LlenarCriteriosGenerales();
                    break;
                case "tbpReglas":
                    if (this.dgvReglas.Rows.Count <= 0)
                        this.LlenarReglas();
                    break;
                case "tbpSinMaxMin":
                    if (this.dgvSinMaxMin.Rows.Count <= 0)
                        this.LlenarSinMaxMin();
                    break;
            }
        }

        private void dgvDetalle_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.dgvDetalle.Columns[e.ColumnIndex].Name == "colProcesar")
            {
                bool? bMarcado = null;
                foreach (DataGridViewRow Fila in this.dgvDetalle.Rows)
                {
                    if (bMarcado == null)
                        bMarcado = !Util.Logico(Fila.Cells["colProcesar"].Value);
                    Fila.Cells["colProcesar"].Value = bMarcado;
                }
            }
        }

        private void dgvDetalle_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;
            if (this.dgvDetalle.CurrentRow == this.DetalleUltimaFilaSel) return;

            this.DetalleUltimaFilaSel = this.dgvDetalle.CurrentRow;
            int iParteID = Util.Entero(this.dgvDetalle.CurrentRow.Cells["ParteID"].Value);
            this.LlenarDatosExtra(iParteID);
            this.LlenarDescripcionMaxMin(this.dgvDetalle.CurrentRow);
            this.ctlVentasMes.LlenarDatos(iParteID);
            this.ctlExistencias.CargarDatos(iParteID);
        }

        private void dgvDetalle_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvDetalle.VerDirtyStateChanged("Fijo");
        }

        private void dgvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;

            if (this.dgvDetalle[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "Fijo")
            {
                bool bMarcado = Util.Logico(this.dgvDetalle.CurrentRow.Cells["Fijo"].Value);
                this.dgvDetalle.CurrentRow.Cells["Maximo"].ReadOnly = !bMarcado;
                this.dgvDetalle.CurrentRow.Cells["Minimo"].ReadOnly = !bMarcado;
            }

        }

        private void trvPartesCalcular_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 3)
            {
                int iParteID = Util.Entero(e.Node.Name);
                this.LlenarDatosExtra(iParteID);
            }
        }

        private void trvPartesCalcular_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!this.trvPartesCalcular.Focused && !this.trvVentasGlobales.Focused) return;

            this.ColorNodo(e.Node, true);
            this.ColorNodoPadre(e.Node.Parent);
        }

        private void dgvReglas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (this.dgvReglas.Columns[e.ColumnIndex].Name == "Reglas_Cambio") return;

            if (Util.Entero(this.dgvReglas["Reglas_Cambio", e.RowIndex].Value) == Cat.TiposDeAfectacion.SinCambios)
            {
                this.dgvReglas["Reglas_Cambio", e.RowIndex].Value = Cat.TiposDeAfectacion.Modificar;
                this.dgvReglas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Orange;
            }
        }

        private void btnReglasAgregar_Click(object sender, EventArgs e)
        {
            // Se obtiene el orden max.
            int iOrden = 1;
            if (this.dgvReglas.Rows.Count > 0)
                iOrden = (Util.Entero(this.dgvReglas.Rows[this.dgvReglas.Rows.Count - 1].Cells["Reglas_Orden"].Value) + 1);

            int iFila = this.dgvReglas.Rows.Add(0, Cat.TiposDeAfectacion.Agregar, iOrden);
            this.dgvReglas.Rows[iFila].DefaultCellStyle.ForeColor = Color.Blue;
            this.dgvReglas.CurrentCell = this.dgvReglas.Rows[iFila].Cells["Reglas_Regla"];
            this.dgvReglas.Focus();
        }

        private void btnReglasQuitar_Click(object sender, EventArgs e)
        {
            if (this.dgvReglas.CurrentRow == null) return;

            if (Util.Entero(this.dgvReglas.CurrentRow.Cells["Reglas_Cambio"].Value) == Cat.TiposDeAfectacion.Agregar)
            {
                this.dgvReglas.Rows.Remove(this.dgvReglas.CurrentRow);
            }
            else
            {
                this.dgvReglas.CurrentRow.Cells["Reglas_Cambio"].Value = Cat.TiposDeAfectacion.Borrar;
                this.dgvReglas.CurrentRow.DefaultCellStyle.ForeColor = Color.Gray;
            }

            // Se reasigna el orden de las filas siguientes
            for (int iFila = (this.dgvReglas.CurrentRow.Index + 1); iFila < this.dgvReglas.Rows.Count; iFila++)
                this.dgvReglas["Reglas_Orden", iFila].Value = (Util.Entero(this.dgvReglas["Reglas_Orden", iFila].Value) - 1);
        }

        private void btnReglasArriba_Click(object sender, EventArgs e)
        {
            this.SubirBajarRegla(this.dgvReglas.CurrentRow, true);
        }

        private void btnReglasAbajo_Click(object sender, EventArgs e)
        {
            this.SubirBajarRegla(this.dgvReglas.CurrentRow, false);
        }

        private void btnReglasCopiar_Click(object sender, EventArgs e)
        {
            if (this.dgvReglas.CurrentRow == null) return;

            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var frmValor = new MensajeObtenerValor("¿A qué sucursal deseas copiar la regla seleccionada?", 0, MensajeObtenerValor.Tipo.Combo);
            frmValor.CargarCombo("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(q => q.SucursalID != iSucursalID));
            frmValor.Combo.DropDownStyle = ComboBoxStyle.DropDownList;
            frmValor.Combo.SelectedIndex = 0;
            if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                // Se obtiene el orden correspondiente
                int iCopiaSucID = Util.Entero(frmValor.Valor);
                int iOrden = 1;
                var oReglasSuc = Datos.GetListOf<ParteMaxMinRegla>(q => q.SucursalID == iCopiaSucID && q.Estatus);
                if (oReglasSuc.Count > 0)
                    iOrden = (oReglasSuc.Max(q => q.Orden) + 1);
                // Se inserta la nueva regla
                var oRegla = new ParteMaxMinRegla()
                {
                    SucursalID = iCopiaSucID,
                    Orden = iOrden,
                    Regla = Util.Cadena(this.dgvReglas.CurrentRow.Cells["Reglas_Regla"].Value),
                    Condicion = Util.Cadena(this.dgvReglas.CurrentRow.Cells["Reglas_Condicion"].Value),
                    Maximo = Util.Cadena(this.dgvReglas.CurrentRow.Cells["Reglas_Maximo"].Value),
                    Minimo = Util.Cadena(this.dgvReglas.CurrentRow.Cells["Reglas_Minimo"].Value)
                };
                Datos.Guardar<ParteMaxMinRegla>(oRegla);
                UtilLocal.MostrarNotificacion("Regla copiada correctamente.");
            }
            frmValor.Dispose();
        }

        private void btnReglasAyuda_Click(object sender, EventArgs e)
        {
            string sAyuda =
                "Variables"
                + "\n\tIdProveedor            : Identificador interno del Proveedor."
                + "\n\tIdMarca                : Identificador interno de la Marca."
                + "\n\tIdLinea                : Identificador interno de la Línea."
                + "\n\tUDE                    : Unidad de Empaque."
                + "\n\tVentasTotal            : Número total de ventas (tickets) que tuvo el producto en un año."
                + "\n\tCantidadMaxDia         : Número máximo de unidades vendidas en un día."
                + "\n\tCantidadMaxSem         : Número máximo de unidades vendidas en una semana."
                + "\n\tCantidadMaxMes         : Número máximo de unidades vendidas en un mes."
                + "\n\tMaximo                 : Cálculo del Máximo."
                + "\n\tMinimo                 : Cálculo del Mínimo."
                + "\n\tEsPar                  : Indica si la parte requiere una existencia Par."
                + "\n\tAbcDeVentas            : Cálculo del Abc de Ventas."
                + "\n\tAbcDeUtilidad          : Cálculo del Abc de Utilidad."
                + "\n\tAbcDeNegocio           : Cálculo del Abc de "
                + "\n\tAbcDeProveedor         : Cálculo del Abc de Proveedor."
                + "\n\tAbcDeLinea             : Cálculo del Abc de Linea."
                + "\n\tVentasAnioAnt1         : Número total de ventas que tuvo el producto el año anterior al actual."
                + "\n\tVentasAnioAnt2         : Número total de ventas que tuvo el producto el año dos antes del actual."
                + "\n\tVentasAnioAnt3         : Número total de ventas que tuvo el producto el año tres antes del actual."
                + "\n\nFunciones"
                + "\n\tRedondear(n1, n2)      : Función que devulve el número \"n1\" redondeado a las posiciones decimales indicadas por \"n2\"."
                + "\n\tMultiploSup(n1, n2)    : Función que devulve el número \"n1\" redondeado al múltiplo superior de \"n2\"."
                + "\n\tPromedio(n1, n2, ...)  : Función que devulve el promedio de los números especificados (n1, n2, n3, etc.)."
                + "\n\tMaxMes(n1)             : Número de unidades vendidas en el mes con ventas máximas número \"n1\"."
                + "\n\tMaxSem(n1)             : Número de unidades vendidas en la semana con ventas máximas número \"n1\"."
                + "\n\nOperadores aritméticos"
                + "\n\t+                      : Suma."
                + "\n\t-                      : Resta."
                + "\n\t*                      : Multiplicación."
                + "\n\t/                      : División."
                + "\n\t%                      : Módulo (Residuo de la división del número 1 entre el número 2)."
                + "\n\nOperadores lógicos"
                + "\n\t&&                     : Y lógico (La condición se cumple sólo si los dos operandos son verdaderos)."
                + "\n\t||                     : Ó lógico (La condición se cumple si alguno de los dos operandos es verdadero)."
                + "\n\t!                      : No lógico (Si el operando es verdadero, se hace falso y viceversa)."
                + "\n\nOtros"
                + "\n\tM                      : Sufijo M. Utilizado después de un número constante para indicar que éste es decimal."
                + "\n\t( )                    : Paréntesis. Utilizados para dar prioridad de operaciones, según la agrupación."
                + "\n\tC ? V : F              : C -> Condición, V -> Valor si la condición es verdadera, F -> Valor si la condicion es falsa"
                ;
            var oTexto = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                Font = new Font(FontFamily.GenericMonospace, 9),
                Text = sAyuda,
                ReadOnly = true
            };
            var oForma = new Form()
            {
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                Size = new Size(Principal.Instance.Width - 160, Principal.Instance.Height - 160),
                StartPosition = FormStartPosition.CenterParent,
                KeyPreview = true,
                Text = "Ayuda para crear Reglas"
            };
            oForma.Controls.Add(oTexto);
            oForma.KeyPress += new KeyPressEventHandler((s, kpe) => { if (kpe.KeyChar == (char)Keys.Escape) oForma.Close(); });
            oForma.ShowDialog(Principal.Instance);
        }

        private void txtEquBuscar_TextChanged(object sender, EventArgs e)
        {
            this.dgvEquivalentes.EncontrarContiene(this.txtEquBuscar.Text, "equ_NumeroDeParte", "equ_Descripcion", "equ_Proveedor", "equ_Marca");
        }

        private void btnEquCopiarDe_Click(object sender, EventArgs e)
        {
            var frmValor = new MensajeObtenerValor("¿De cuál sucursal quieres copiar?", "", MensajeObtenerValor.Tipo.Combo);
            frmValor.CargarCombo("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
            frmValor.Combo.DropDownStyle = ComboBoxStyle.DropDownList;
            if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                this.CopiarEquivalentesDeSucursal(Util.Entero(frmValor.Valor));
            }
            frmValor.Dispose();
        }

        private void dgvEquivalentes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvEquivalentes_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvEquivalentes.CurrentRow == null) return;
            int iParteID = Util.Entero(this.dgvEquivalentes.CurrentRow.Cells["equ_ParteID"].Value);
            if (iParteID > 0)
                this.LlenarDatosExtra(iParteID);
        }

        private void dgvEquivalentes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string sColumna = this.dgvEquivalentes[e.ColumnIndex, e.RowIndex].OwningColumn.Name;
            if (sColumna == "equ_Sel" || sColumna == "equ_Maximo" || sColumna == "equ_Minimo")
            {
                this.dgvEquivalentes.Rows[e.RowIndex].Tag = true;  // Indica que la fila ha sido modificada
            }
        }

        private void dgvSinMaxMin_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvSinMaxMin.CurrentRow == null) return;
            int iParteID = Util.Entero(this.dgvSinMaxMin.CurrentRow.Cells["SinMaxMin_ParteID"].Value);
            this.LlenarDatosExtra(iParteID);
        }

        private void btnImanImprimir_Click(object sender, EventArgs e)
        {
            this.ImprimirImanes();
        }

        private void dgvIman_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.dgvIman.Columns[e.ColumnIndex].Name == "ima_Sel")
                {
                    bool bSel = true;
                    if (this.dgvIman.Rows.Count > 0)
                        bSel = !Util.Logico(this.dgvIman["ima_Sel", 0].Value);
                    foreach (DataGridViewRow oFila in this.dgvIman.Rows)
                        oFila.Cells["ima_Sel"].Value = bSel;
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (this.tabMaxMin.SelectedTab.Name == "tbpDetalle")
            {
                Cargando.Mostrar();
                string sArchivo = (Path.GetTempFileName() + ".csv");
                this.dgvDetalle.ExportarACsv(sArchivo, true);
                Process.Start("excel", sArchivo);
                Cargando.Cerrar();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            switch (this.tabMaxMin.SelectedTab.Name)
            {
                case "tbpDetalle":
                    this.GuardarMaxMin();
                    break;
                case "tbpCriteriosGenerales":
                    this.GuardarCriteriosGenerales();
                    break;
                case "tbpReglas":
                    this.GuardarReglas();
                    break;
                case "tbpEquivalentes":
                    this.GuardarEquivalentes();
                    break;
            }
        }

        #endregion

        #region [ Métodos ]

        private void MostrarDetalle()
        {
            Cargando.Mostrar();

            // Se agregan los parámetros
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", Util.Entero(this.cmbSucursal.SelectedValue));
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);
            // if (this.cmbProveedor.SelectedValue != null)
            //     oParams.Add("ProveedorID", Util.ConvertirEntero(this.cmbProveedor.SelectedValue));
            if (this.ctlProveedores.ValoresSeleccionados.Count > 0)
            {
                var oDtProveedores = Util.ListaEntityADataTable(this.ctlProveedores.ElementosSeleccionados);
                oDtProveedores.Columns.Remove("Cadena");
                oParams.Add("Proveedores/tpuTablaEnteros", oDtProveedores);
            }
            // if (this.cmbMarca.SelectedValue != null)
            //     oParams.Add("MarcaID", Util.ConvertirEntero(this.cmbMarca.SelectedValue));
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
            {
                var oDtMarcas = Util.ListaEntityADataTable(this.ctlMarcas.ElementosSeleccionados);
                oDtMarcas.Columns.Remove("Cadena");
                oParams.Add("Marcas/tpuTablaEnteros", oDtMarcas);
            }
            // if (this.cmbLinea.SelectedValue != null)
            //     oParams.Add("LineaID", Util.ConvertirEntero(this.cmbLinea.SelectedValue));
            if (this.ctlLineas.ValoresSeleccionados.Count > 0)
            {
                var oDtLineas = Util.ListaEntityADataTable(this.ctlLineas.ElementosSeleccionados);
                oDtLineas.Columns.Remove("Cadena");
                oParams.Add("Lineas/tpuTablaEnteros", oDtLineas);
            }
            //
            var oMaxMin = Datos.ExecuteProcedure<pauPartesMaxMin_Res>("pauPartesMaxMin", oParams);
            DateTime dFechaDeCalc = this.dtpFechaDeCalculo.Value.Date;//.AddDays(1);
            string c2 = dFechaDeCalc.Date.ToString();
            string f = DateTime.Now.Date.ToString();

            
            //if (dFechaDeCalc < DateTime.Now.Date) //se comentó esta linea para evitar filtrar registros que coincidad con fecha de calculo al dia actual
                oMaxMin = oMaxMin.Where(c => c.FechaCalculo.Valor().Date < dFechaDeCalc.Date).ToList();

            int iFila;
            this.dgvDetalle.Rows.Clear();
            int NumFila = 0;
            foreach (var oParte in oMaxMin)
            {
                bool bSel = (oParte.VentasTotal > 0 || oParte.Maximo > 0);
                // Se filtran las partes según sus ventas
                switch (this.cmbFiltroDeVentas.Text)
                {
                    case "12 meses":
                        bSel = (oParte.VentasTotal > 0 || oParte.Maximo > 0);
                        break;
                    case "Año ant. 1":
                        bSel = (oParte.VentasAnt1 > 0 || oParte.Maximo > 0);
                        break;
                    case "Año ant. 2":
                        bSel = (oParte.VentasAnt2 > 0 || oParte.Maximo > 0);
                        break;
                    case "Año ant. 3":
                        bSel = (oParte.VentasAnt3 > 0 || oParte.Maximo > 0);
                        break;
                }
                // Se saltan las partes que no tengan ventas o MaxMin, si aplica
                if (!bSel) continue;

                iFila = this.dgvDetalle.Rows.Add(oParte.ParteID, bSel, oParte.NumeroDeParte, oParte.Descripcion, oParte.Proveedor, oParte.Linea, oParte.Marca
                    , oParte.Existencia, oParte.UnidadEmpaque, oParte.TiempoReposicion, oParte.AbcDeNegocio, oParte.AbcDeVentas, oParte.AbcDeUtilidad
                    , oParte.VentasTotal, oParte.CantidadTotal, oParte.UtilidadTotal, oParte.Fijo, oParte.Minimo, oParte.Maximo, null, null
                    , oParte.ParteMaxMinReglaID, oParte.FechaCalculo);
                //iFila = this.dgvDetalle.Rows.Add(oParte.ParteID, bSel, oParte.NumeroDeParte, oParte.Descripcion, oParte.Proveedor, oParte.Linea, oParte.Marca
                //    , oParte.Existencia, oParte.UnidadEmpaque, oParte.TiempoReposicion, oParte.AbcDeNegocio, oParte.AbcDeVentas, oParte.AbcDeUtilidad
                //    , oParte.VentasTotal, oParte.CantidadTotal, oParte.UtilidadTotal, oParte.Fijo, oParte.Minimo, oParte.Maximo, null, null
                //    , oParte.ParteMaxMinReglaID, oParte.FechaCalculo);
                this.dgvDetalle.Rows[iFila].Tag = oParte;
            }
            // this.dgvDetalle.AutoResizeColumns();

            Cargando.Cerrar();
        }

        private void CalcularMaxMin()
        {
            Cargando.Mostrar();

            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);

            // Se obtiene la lista de reglas
            var oReglas = Datos.GetListOf<ParteMaxMinRegla>(q => q.SucursalID == iSucursalID && q.Estatus).OrderBy(q => q.Orden);
            // Se cargan las reglas a una clase, para ser compiladas
            string sParamsMetodo = (
                "int IdProveedor, int IdMarca, int IdLinea, decimal UDE, int VentasTotal"
                + ", decimal CantidadMaxDia, decimal CantidadMaxSem, decimal CantidadMaxMes, decimal Maximo, decimal Minimo"
                + ", bool EsPar"
                + ", string AbcDeVentas, string AbcDeUtilidad, string AbcDeNegocio, string AbcDeProveedor, string AbcDeLinea"
                + ", int VentasAnioAnt1, int VentasAnioAnt2, int VentasAnioAnt3"
            );

            // Se genera el código dinámico
            var oDin = new CodigoDinamico();
            oDin.Referencias.AddRange(new string[] { "System.dll", "THEOS.exe", "LibUtil.dll" });
            oDin.Usings.AddRange(new string[] { "System", "Refaccionaria.App", "LibUtil" });
            var oCodigo = new StringBuilder();
            foreach (var oRegla in oReglas)
            {
                oCodigo.AppendFormat("public object Regla{0}_Condicion({1}) {{ return ({2}); }}", oRegla.ParteMaxMinReglaID, sParamsMetodo, oRegla.Condicion);
                oCodigo.AppendFormat("public object Regla{0}_Maximo({1}) {{ return ({2}); }}", oRegla.ParteMaxMinReglaID, sParamsMetodo, oRegla.Maximo);
                oCodigo.AppendFormat("public object Regla{0}_Minimo({1}) {{ return ({2}); }}", oRegla.ParteMaxMinReglaID, sParamsMetodo, oRegla.Minimo);
            }
            // Se agrega la función para el múltiplo de la Unidad de Empaque
            oCodigo.AppendLine("public decimal MultiploSup(decimal mNumero, decimal mMultiplo) { if (mMultiplo == 0) return 0; decimal mResiduo = (mNumero % mMultiplo);"
                + " if (mResiduo == 0) return mNumero; else return (mNumero + (mMultiplo - mResiduo)); }");
            // Se agrega función para redondeo
            oCodigo.AppendLine("public decimal Redondear(decimal mNumero, int iDecimales) { return Math.Round(mNumero, iDecimales); }");
            // Se agrega función para promedio
            oCodigo.AppendLine("public decimal Promedio(params decimal[] aNumeros) { return Util.Promedio(aNumeros); }");
            // Se agregan funciones para cantidades máximas por períodos de tiempo
            oCodigo.AppendLine("public decimal MaxSem(int iOrden) { return MaxMinFunciones.MaxSem(iOrden); }");
            oCodigo.AppendLine("public decimal MaxMes(int iOrden) { return MaxMinFunciones.MaxMes(iOrden); }");
            // Se manda generar la instancia con el código
            oDin.GenerarInstancia(oCodigo.ToString());
            if (oDin.Instancia == null) { Cargando.Cerrar(); return; }

            // Se inicia el cálculo, por partes
            int iFilaActual = 0, iTotalFilas = this.dgvDetalle.Rows.Count, iIncremento = 300;
            while (iFilaActual < iTotalFilas)
            {
                if ((iFilaActual + iIncremento) > iTotalFilas)
                    iIncremento = (iTotalFilas - iFilaActual - 1);
                UtilLocal.MostrarNotificacion(string.Format("Procesando filas de la {0} a la {1}..", iFilaActual, iIncremento));
                Application.DoEvents();
                this.CalcularMaxMinFilas(iSucursalID, oDin, oReglas, iFilaActual, iIncremento);
                iFilaActual += (iIncremento + 1);
            }

            // Se hace el filtro de Cambios, si aplica
            this.FiltroDeCambios();

            Cargando.Cerrar();
        }

        private void FiltroDeCambios()
        {
            DataGridView test = new DataGridView();
            test = dgvDetalle;
            Cargando.Mostrar();
            //this.dgvDetalle.Rows.Clear();
            if (this.cmbCambios.SelectedIndex >= 0)
            {
                for (int iFila = 0; iFila < this.dgvDetalle.Rows.Count; iFila++)
                {
                    if (!this.VerFiltroDeCambios(this.dgvDetalle.Rows[iFila]))
                    {
                        // this.dgvDetalle["colProcesar", iFila].Value = false;
                        this.dgvDetalle.Rows[iFila].Visible = false;
                    }
                }
            }

            Cargando.Cerrar();
        }

        private void CalcularMaxMinFilas(int iSucursalID, CodigoDinamico oDin, IEnumerable<ParteMaxMinRegla> oReglas, int iFilaDesde, int iIncremento)
        {
            pauPartesMaxMin_Res oParte;
            DataGridViewRow oFila;
            int iFilaHasta = (iFilaDesde + iIncremento);
            for (int iFila = iFilaDesde; iFila <= iFilaHasta; iFila++)
            {
                oFila = this.dgvDetalle.Rows[iFila];
                // Si no está marcado, no se calcula
                if (!Util.Logico(oFila.Cells["colProcesar"].Value)) continue;
                // Si es fijo, se pasan los datos actuales tal cual
                if (Util.Logico(oFila.Cells["Fijo"].Value))
                {
                    oFila.Cells["Maximo"].Value = oFila.Cells["MaximoActual"].Value;
                    oFila.Cells["Minimo"].Value = oFila.Cells["MinimoActual"].Value;
                    continue;
                }

                oParte = (oFila.Tag as pauPartesMaxMin_Res);

                // Se generan los parámetros
                var oParams = new object[] {
                    oParte.ProveedorID, oParte.MarcaParteID, oParte.LineaID, oParte.UnidadEmpaque, oParte.VentasTotal
                    , oParte.CantidadMaxDia, oParte.CantidadMaxSem, oParte.CantidadMaxMes, oParte.Maximo, oParte.Minimo
                    , oParte.EsPar
                    , oParte.AbcDeVentas, oParte.AbcDeUtilidad, oParte.AbcDeNegocio, oParte.AbcDeProveedor, oParte.AbcDeLinea
                    , oParte.VentasAnt1, oParte.VentasAnt2, oParte.VentasAnt3
                };
                // Se establece el Max y el Min con cero
                oParte.Maximo = 0;
                oParte.Minimo = 0;
                // Se inicializan los datos extra
                MaxMinFunciones.Inicializar(oParte.ParteID, iSucursalID);

                // Se procesan la regla correspondiente, o todas si no se tiene una asignada
                string sReglasAp = this.ProcesarReglas(oDin, oReglas, ref oParte, oParams);

                // Si Maximo es cero, se vuelven a correr las reglas con las ventas globales, si aplica
                if (oParte.VentasGlobales && oParte.Maximo.Valor() == 0 && iSucursalID == Cat.Sucursales.Matriz)
                {
                    // Se traen los datos globales
                    var oParamsProc = new Dictionary<string, object>();
                    oParamsProc.Add("ParteID", oParte.ParteID);
                    oParamsProc.Add("Desde", this.Desde);
                    oParamsProc.Add("Hasta", this.Hasta);
                    var oParteGlobal = Datos.ExecuteProcedure<pauParteMaxMinDatosVentas_Result>("pauParteMaxMinDatosVentas", oParamsProc).FirstOrDefault();

                    oParams = new object[] {
                        oParte.ProveedorID, oParte.MarcaParteID, oParte.LineaID, oParte.UnidadEmpaque, oParteGlobal.Ventas
                        , oParteGlobal.CantidadMaxDia, oParteGlobal.CantidadMaxSem, oParteGlobal.CantidadMaxMes, oParte.Maximo, oParte.Minimo
                        , oParte.EsPar
                        , oParte.AbcDeVentas, oParte.AbcDeUtilidad, oParte.AbcDeNegocio, oParte.AbcDeProveedor, oParte.AbcDeLinea
                        , oParte.VentasAnt1, oParte.VentasAnt2, oParte.VentasAnt3
                    };
                    MaxMinFunciones.Inicializar(oParte.ParteID, null);
                    // Se procesan las reglas
                    sReglasAp = this.ProcesarReglas(oDin, oReglas, ref oParte, oParams);
                }

                // Se escriben los cambios en el grid
                oFila.Cells["Maximo"].Value = oParte.Maximo;
                oFila.Cells["Minimo"].Value = oParte.Minimo;
                if (Util.Decimal(oFila.Cells["MaximoActual"].Value) == oParte.Maximo && Util.Decimal(oFila.Cells["MinimoActual"].Value) == oParte.Minimo)
                {
                    oFila.DefaultCellStyle.ForeColor = this.dgvDetalle.DefaultCellStyle.ForeColor;
                }
                else
                {
                    oFila.DefaultCellStyle.ForeColor = Color.Brown;
                    oFila.DefaultCellStyle.Font = new Font(this.dgvDetalle.DefaultCellStyle.Font, FontStyle.Bold);
                }
                oFila.Cells["Condiciones"].Value = sReglasAp;
            }
        }

        private string ProcesarReglas(CodigoDinamico oDin, IEnumerable<ParteMaxMinRegla> oReglas, ref pauPartesMaxMin_Res oParte, object[] oParams)
        {
            string sReglasAp = "";
            // Se ejecuta la regla correspondiente si ya está asignada, si no, se ejecutan todas las reglas
            int iReglaID = oParte.ParteMaxMinReglaID.Valor();
            if (iReglaID > 0 && Datos.Exists<ParteMaxMinRegla>(c => c.ParteMaxMinReglaID == iReglaID && c.Estatus))
            {
                string sReglaID = oParte.ParteMaxMinReglaID.ToString();
                string sPrefijoMet = ("Regla" + sReglaID + "_");
                oParte.Maximo = Util.Decimal(oDin.Ejecutar((sPrefijoMet + "Maximo"), oParams));
                oParams[8] = oParte.Maximo;
                oParte.Minimo = Util.Decimal(oDin.Ejecutar((sPrefijoMet + "Minimo"), oParams));
                // oParams[9] = oParte.Minimo;
                sReglasAp += (", " + sReglaID);
            }
            else
            {
                foreach (var oRegla in oReglas)
                {
                    string sReglaID = oRegla.ParteMaxMinReglaID.ToString();
                    string sPrefijoMet = ("Regla" + sReglaID + "_");
                    bool bProcesar = Util.Logico(oDin.Ejecutar((sPrefijoMet + "Condicion"), oParams));
                    if (bProcesar)
                    {
                        oParte.Maximo = Util.Decimal(oDin.Ejecutar((sPrefijoMet + "Maximo"), oParams));
                        oParams[8] = oParte.Maximo;
                        oParte.Minimo = Util.Decimal(oDin.Ejecutar((sPrefijoMet + "Minimo"), oParams));
                        oParams[9] = oParte.Minimo;
                        sReglasAp += (", " + sReglaID);
                    }
                }
            }

            // Se ejecutan las reglas básicas finales
            if (oParte.Maximo == 0)
            {
                oParte.Minimo = 0;
            }
            else if (oParte.Minimo > 0)
            {
                if (oParte.Minimo < 1)
                    oParte.Minimo = 1;
                if (oParte.Minimo > oParte.Maximo)
                    oParte.Minimo = (oParte.Maximo - 1);
                if (oParte.Minimo == oParte.Maximo)
                    oParte.Minimo--;
            }

            return (sReglasAp.Length > 1 ? sReglasAp.Substring(2) : "");
        }

        private bool VerFiltroDeCambios(DataGridViewRow oFila)
        {
            decimal mMaximo = Util.Decimal(oFila.Cells["MaximoActual"].Value);
            decimal mMaximoNuevo = Util.Decimal(oFila.Cells["Maximo"].Value);
            bool bFijo = Util.Logico(oFila.Cells["Fijo"].Value);
            // this.cmbCambios.Items.AddRange(new object[] { , , , , ,  });
            switch (this.cmbCambios.SelectedIndex)
            {
                case 0:  // "0 a > 0"
                    return (mMaximo == 0 && mMaximoNuevo > 0);
                case 1:  // "> 0 a > Actual"
                    return (mMaximo > 0 && mMaximoNuevo > mMaximo);
                case 2:  // "Actual baja a > 0 "
                    return (mMaximoNuevo < mMaximo && mMaximoNuevo > 0);
                case 3:  // "Max a 0"
                    return (!bFijo && mMaximoNuevo != mMaximo && mMaximoNuevo == 0);
                case 4:  // "Max igual"
                    return (!bFijo && mMaximoNuevo == mMaximo);
                case 5: // "Fijos"
                    return true;
                default:
                    return true;
            }
        }

        private void ProcesarFijos()
        {
            // Se filtran sólo los fijos
            Cargando.Mostrar();
            for (int iFila = 0; iFila < this.dgvDetalle.Rows.Count; iFila++)
            {
                if (Util.Logico(this.dgvDetalle["Fijo", iFila].Value))
                    this.dgvDetalle["Fijo", iFila].Value = false;
                else
                    this.dgvDetalle.Rows.RemoveAt(iFila--);
            }
            Cargando.Cerrar();
            // Se mandan a procesar los fijos
            this.btnProcesar_Click(this, null);
        }

        private void GuardarMaxMin()
        {
            // Se valida que todas las filas tengan máximo una sola condición
            bool bError = false;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                if (Util.Cadena(oFila.Cells["Condiciones"].Value).Contains(","))
                {
                    bError = true;
                    break;
                }
            }
            if (bError)
            {
                UtilLocal.MensajeAdvertencia("Existe una o más partes que coinciden con más de una regla. Verificar.");
                return;
            }

            // Se valida que si hay fijos en cero, se ponga el motivo
            bError = false;
            int x = 0;
            int linea = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                if (oFila.Visible == true)
                {
                    if ((bool)oFila.Cells[1].Value == true)
                    {
                        bool fijo = Util.Logico(oFila.Cells["Fijo"].Value);
                        decimal max = Util.Decimal(oFila.Cells["Maximo"].Value);
                        decimal min = Util.Decimal(oFila.Cells["Minimo"].Value);
                        bool motivo = String.IsNullOrEmpty(Util.Cadena(oFila.Cells["MotivoFijoCero"].Value));

                        oFila.Cells["MotivoFijoCero"].ErrorText = "";
                        if (Util.Logico(oFila.Cells["Fijo"].Value) && (Util.Decimal(oFila.Cells["Maximo"].Value) == 0) && (Util.Decimal(oFila.Cells["Minimo"].Value) == 0)
                            && String.IsNullOrEmpty(Util.Cadena(oFila.Cells["MotivoFijoCero"].Value)))
                        {
                            oFila.Cells["MotivoFijoCero"].ErrorText = "Debes especificar un motivo";
                            bError = true;
                            if (bError)
                            {
                                linea = oFila.Index;
                                UtilLocal.MensajeAdvertencia("Por favor indica todos los motivos en donde se especificó un Max Min de cero en la linea: " + linea + ", con Num Parte: " + oFila.Cells[3].Value + ".");
                                return;
                            }
                        }

                    }
                }


            }

            if (bError)
            {
                UtilLocal.MensajeAdvertencia("Por favor indica todos los motivos en donde se especificó un Max Min de cero en la linea .");
                return;
            }

            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas guardar los Máximos y Mínimos mostrados?") != DialogResult.Yes)
                return;

            Cargando.Mostrar();

            // Se procede a guardar los máximos y mínimos
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            DateTime dAhora = DateTime.Now;
            foreach (DataGridViewRow Fila in this.dgvDetalle.Rows)
            {
                // Si no está marcado, no se guarda
                if (!Fila.Visible || !Util.Logico(Fila.Cells["colProcesar"].Value)) continue;

                int iParteID = Util.Entero(Fila.Cells["ParteID"].Value);
                var oParteMaxMin = Datos.GetEntity<ParteMaxMin>(q => q.ParteID == iParteID && q.SucursalID == iSucursalID);
                // Se supone que no puede pasar que el registro no exista, pues aquí sólo aparecen los que tengan "ParteMaxMin.Calcular" = 1
                oParteMaxMin.Fijo = Util.Logico(Fila.Cells["Fijo"].Value);
                oParteMaxMin.Maximo = Util.Decimal(Fila.Cells["Maximo"].Value);
                oParteMaxMin.Minimo = Util.Decimal(Fila.Cells["Minimo"].Value);
                oParteMaxMin.FechaCalculo = dAhora;
                oParteMaxMin.ParteMaxMinReglaID = ((oParteMaxMin.Fijo.Valor() || Util.Entero(Fila.Cells["Condiciones"].Value) == 0)
                    ? null : (int?)Util.Entero(Fila.Cells["Condiciones"].Value));
                Datos.Guardar<ParteMaxMin>(oParteMaxMin);

                // Se verifica si aplica para 9500 y se guarda el dato Es9500
                AdmonProc.VerGuardar9500(iParteID);

                // Se guardan los datos de fijos históricos, si aplican
                if (oParteMaxMin.Fijo.Valor() && oParteMaxMin.Maximo == 0 && oParteMaxMin.Minimo == 0)
                {
                    var oFijoHist = new ParteMaxMinFijoHistorico()
                    {
                        ParteID = iParteID,
                        SucursalID = iSucursalID,
                        Fecha = dAhora,
                        Motivo = Util.Cadena(Fila.Cells["MotivoFijoCero"].Value)
                    };
                    Datos.Guardar<ParteMaxMinFijoHistorico>(oFijoHist);
                }
            }

            Cargando.Cerrar();
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");
        }

        private void LlenarDatosExtra(int iParteID)
        {
            this.LimpiarDatosExtra();
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);

            // Se obtiene los datos de Abc
            var oParteAbc = Datos.GetEntity<ParteAbc>(q => q.ParteID == iParteID);
            if (oParteAbc != null)
            {
                this.lblAbcProveedor.Text = oParteAbc.AbcDeProveedor;
                this.lblAbcLinea.Text = oParteAbc.AbcDeLinea;
            }
            // Se llena el costo
            var oPartePrecio = Datos.GetEntity<PartePrecio>(q => q.ParteID == iParteID && q.Estatus);
            this.lblCosto.Text = oPartePrecio.Costo.Valor().ToString(GlobalClass.FormatoMoneda);
            // Se obtiene el dato de si es ventas globales o no
            // var oParteMaxMin = General.GetEntity<ParteMaxMin>(q => q.SucursalID == iSucursalID && q.ParteID == iParteID);
            // this.lblVentasGlobales.Visible = oParteMaxMin.VentasGlobales.Valor();

            // Se llenan los datos calculados
            var oParams = new Dictionary<string, object>();
            oParams.Add("ParteID", iParteID);
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);
            oParams.Add("SucursalID", iSucursalID);

            var oDatos = Datos.ExecuteProcedure<pauParteMaxMinDatosExtra_Result>("pauParteMaxMinDatosExtra", oParams);

            // Se llenan los datos, según la sucursal, si hubo resultados
            if (oDatos.Count > 1)
            {
                this.lblVentaMayorDia.Text = oDatos[0].Cantidad.Valor().ToString();
                this.lblVentaMenorDia.Text = oDatos[1].Cantidad.Valor().ToString();
                this.lblSemanasConVenta.Text = oDatos.Where(q => q.Grupo == 3).Count().ToString();
                // Coloración
                this.lblVentaMayorDia.ForeColor = this.DatosExtraColoracion(oDatos[0].Cantidad, oDatos[0].Negadas);
                this.lblVentaMenorDia.ForeColor = this.DatosExtraColoracion(oDatos[1].Cantidad, oDatos[1].Negadas);
                this.lblSemanasConVenta.ForeColor = this.DatosExtraColoracion(oDatos.Where(q => q.Grupo == 3).Count(), oDatos.Where(q => q.Grupo == 3 && q.Negadas > 0).Count());
                // Fin - Coloración

                // Se llenan los meses
                /* foreach (var oDato in oDatos)
                {
                    if (oDato.Grupo == 4)
                    {
                        this.Controls[string.Format("lblMes{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].Text = oDato.Cantidad.Valor().ToString();
                        this.Controls[string.Format("lblMes{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].ForeColor =
                            this.DatosExtraColoracion(oDato.Cantidad, oDato.Negadas);
                    }
                }
                */
                // Se llenan los indicadores de los meses
                /* var oMeses = oDatos.Where(q => q.Grupo == 4).OrderByDescending(q => q.Cantidad);
                int iCuentaMes = 1;
                foreach (var oMes in oMeses)
                {
                    string sEtiqueta = string.Format("lblInMes{0:00}", this.MesesEtiquetas[oMes.Periodo.Valor()]);
                    this.Controls[sEtiqueta].Text = (iCuentaMes > 6 ? "<" : ">");
                    this.Controls[sEtiqueta].ForeColor = (iCuentaMes > 6 ? Color.Olive : Color.DarkRed);
                    iCuentaMes++;
                }
                */

                // Se llenan las semanas
                var oSemanas = oDatos.Where(q => q.Grupo == 3).OrderByDescending(q => q.Cantidad).ToList();
                for (int iCont = 0; (iCont < 26 && iCont < oSemanas.Count); iCont++)
                {
                    this.Controls[string.Format("lblSem{0:00}", iCont + 1)].Text = oSemanas[iCont].Cantidad.Valor().ToString();
                    this.Controls[string.Format("lblSem{0:00}", iCont + 1)].ForeColor = this.DatosExtraColoracion(oSemanas[iCont].Cantidad, oSemanas[iCont].Negadas);
                }
            }

            // Se llenan los datos globales, si aplica
            /* if (oParteMaxMin.VentasGlobales.Valor())
            {
                oParams.Remove("SucursalID");
                oDatos = General.ExecuteProcedure<pauParteMaxMinDatosExtra_Result>("pauParteMaxMinDatosExtra", oParams);
                foreach (var oDato in oDatos)
                {
                    if (oDato.Grupo == 4)
                    {
                        this.Controls[string.Format("lblMesG{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].Text = oDato.Cantidad.Valor().ToString();
                        this.Controls[string.Format("lblMesG{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].ForeColor =
                            this.DatosExtraColoracion(oDato.Cantidad, oDato.Negadas);
                    }
                }
            }
            */
        }

        private Color DatosExtraColoracion(decimal? mCantidad, decimal? mNegadas)
        {
            decimal? mDiferencia = (mCantidad - mNegadas);

            if (mDiferencia == mCantidad)
                return Color.White;
            else if (mDiferencia == 0)
                return Color.Tomato;
            else
                return Color.LightSkyBlue;
        }

        private void LimpiarDatosExtra()
        {
            this.lblAbcProveedor.Text = "";
            this.lblAbcLinea.Text = "";
            this.lblCosto.Text = "";
            this.lblVentaMayorDia.Text = "";
            this.lblVentaMenorDia.Text = "";
            this.lblSemanasConVenta.Text = "";
            // this.lblVentasGlobales.Visible = false;

            foreach (Control oControl in this.Controls)
            {
                if (oControl.Name.Contains("lblInMes"))
                    oControl.Text = "";
                else if (oControl.Name.Contains("lblMes") || oControl.Name.Contains("lblSem"))
                    oControl.Text = "0";
            }

            // Coloración
            foreach (Control oControl in this.Controls)
            {
                if (oControl.Name.Contains("lblVentaM") || oControl.Name == "lblSemanasConVenta")
                    oControl.ForeColor = Color.Black;
                else if (oControl.Name.Contains("lblMes") || oControl.Name.Contains("lblSem"))
                    oControl.ForeColor = Color.White;
            }
            // Fin - Coloración
        }

        private void LlenarCriteriosGenerales()
        {
            Control oEnfocado = this.ActiveControl;
            this.tbpCriteriosGenerales.Focus();
            Cargando.Mostrar();

            // Se llenan las partes, agrupadas por Proveedor, Marca, Línea
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oPartes = Datos.GetListOf<PartesMaxMinView>(q => q.SucursalID == iSucursalID && q.ParteEstatusID == Cat.PartesEstatus.Activo).OrderBy(q => q.Proveedor)
                .ThenBy(q => q.Marca).ThenBy(q => q.Linea).ThenBy(q => q.Descripcion);
            this.trvPartesCalcular.Nodes.Clear();

            TreeNode oNodoProveedor, oNodoMarca, oNodoLinea, oNodoParte;
            oNodoProveedor = oNodoMarca = oNodoLinea = oNodoParte = null;
            string sProveedor, sMarca, sLinea;
            sProveedor = sMarca = sLinea = "";
            List<TreeNode> oNodosMarc = new List<TreeNode>();
            foreach (var oParte in oPartes)
            {
                // Nodo de proeedor, al cambiar de proveedor
                if (oParte.Proveedor != sProveedor)
                {
                    sProveedor = oParte.Proveedor;
                    oNodoProveedor = this.trvPartesCalcular.Nodes.Add(oParte.ProveedorID.ToString(), sProveedor);
                    sMarca = "";
                }

                // Nodo de marca, al cambiar
                if (oParte.Marca != sMarca)
                {
                    sMarca = oParte.Marca;
                    oNodoMarca = oNodoProveedor.Nodes.Add(oParte.MarcaParteID.ToString(), sMarca);
                    sLinea = "";
                }

                // Nodo de línea
                if (oParte.Linea != sLinea)
                {
                    sLinea = oParte.Linea;
                    oNodoLinea = oNodoMarca.Nodes.Add(oParte.LineaID.ToString(), sLinea);
                }

                // Nodo de parte en sí
                oNodoParte = oNodoLinea.Nodes.Add(oParte.ParteID.ToString(), oParte.Descripcion);

                if (oParte.Calcular.HasValue)
                {
                    oNodoParte.Checked = oParte.Calcular.Value;
                    // oNodoParte.Tag = oParte.CalcularMaxMin.Value;
                    oNodosMarc.Add(oNodoParte);
                }
            }

            // Se hace la coloración
            this.trvPartesCalcular.ForeColor = Color.Orange;
            foreach (var oNodo in oNodosMarc)
            {
                this.ColorNodo(oNodo, false);
                this.ColorNodoPadre(oNodo.Parent);
            }

            // Se replica el código, para el segundo tree view
            this.trvVentasGlobales.Visible = (iSucursalID == Cat.Sucursales.Matriz);
            if (iSucursalID == Cat.Sucursales.Matriz)
            {
                this.trvVentasGlobales.Nodes.Clear();
                oNodoProveedor = oNodoMarca = oNodoLinea = oNodoParte = null;
                sProveedor = sMarca = sLinea = "";
                oNodosMarc.Clear();
                foreach (var oParte in oPartes)
                {
                    // Nodo de proeedor, al cambiar de proveedor
                    if (oParte.Proveedor != sProveedor)
                    {
                        sProveedor = oParte.Proveedor;
                        oNodoProveedor = this.trvVentasGlobales.Nodes.Add(oParte.ProveedorID.ToString(), sProveedor);
                        sMarca = "";
                    }
                    // Nodo de marca, al cambiar
                    if (oParte.Marca != sMarca)
                    {
                        sMarca = oParte.Marca;
                        oNodoMarca = oNodoProveedor.Nodes.Add(oParte.MarcaParteID.ToString(), sMarca);
                        sLinea = "";
                    }
                    // Nodo de línea
                    if (oParte.Linea != sLinea)
                    {
                        sLinea = oParte.Linea;
                        oNodoLinea = oNodoMarca.Nodes.Add(oParte.LineaID.ToString(), sLinea);
                    }
                    // Nodo de parte en sí
                    oNodoParte = oNodoLinea.Nodes.Add(oParte.ParteID.ToString(), oParte.Descripcion);
                    if (oParte.VentasGlobales.HasValue)
                    {
                        oNodoParte.Checked = oParte.VentasGlobales.Value;
                        oNodosMarc.Add(oNodoParte);
                    }
                }
                // Se hace la coloración
                this.trvVentasGlobales.ForeColor = Color.Orange;
                foreach (var oNodo in oNodosMarc)
                {
                    this.ColorNodo(oNodo, false);
                    this.ColorNodoPadre(oNodo.Parent);
                }
            }

            // Se enlazan / desenlazan los TreeViews, según aplique
            if (iSucursalID == Cat.Sucursales.Matriz)
                this.trvPartesCalcular.AddLinkedTreeView(this.trvVentasGlobales);
            else
                this.trvPartesCalcular.RemoveLinkedTreeView(this.trvVentasGlobales);

            Cargando.Cerrar();
            oEnfocado.Focus();
        }

        private void ColorNodo(TreeNode oNodo, bool bRegistrarCambio)
        {
            var oCambios = (oNodo.TreeView == this.trvPartesCalcular ? this.CgCambiosCalcular : this.CgCambiosVentasGlobales);

            oNodo.ForeColor = Color.Green;
            if (bRegistrarCambio && oNodo.Level == 3)
                oCambios[oNodo.Name] = oNodo.Checked;
            foreach (TreeNode oNodoHijo in oNodo.Nodes)
            {
                oNodoHijo.Checked = oNodo.Checked;
                if (bRegistrarCambio && oNodo.Level == 3)
                    oCambios[oNodoHijo.Name] = oNodo.Checked;
            }
        }

        private void ColorNodoPadre(TreeNode oNodoPadre)
        {
            if (oNodoPadre == null) return;

            int iHijo, iDefinidos = 0, iMedioDefin = 0;
            for (iHijo = 0; iHijo < oNodoPadre.Nodes.Count; iHijo++)
            {
                if (oNodoPadre.Nodes[iHijo].ForeColor == Color.Green)
                    iDefinidos++;
                else if (oNodoPadre.Nodes[iHijo].ForeColor == Color.Blue)
                    iMedioDefin++;
            }

            if (iDefinidos == iHijo)
            {
                oNodoPadre.ForeColor = Color.Green;
            }
            else if (iDefinidos > 0 || iMedioDefin > 0)
            {
                oNodoPadre.ForeColor = Color.Blue;
            }
            else
            {
                oNodoPadre.ForeColor = Color.Orange;
            }

            this.ColorNodoPadre(oNodoPadre.Parent);
        }

        private void GuardarCriteriosGenerales()
        {
            Cargando.Mostrar();

            // Se guardan los datos de "calcular"
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            foreach (var oCambio in this.CgCambiosCalcular)
            {
                int iParteID = Util.Entero(oCambio.Key);
                var oParteMaxMin = Datos.GetEntity<ParteMaxMin>(q => q.SucursalID == iSucursalID && q.ParteID == iParteID);
                oParteMaxMin.Calcular = oCambio.Value;
                // Si ya no se calcula, el Max y el Min se hacen 0
                if (!oParteMaxMin.Calcular.Valor())
                {
                    oParteMaxMin.Maximo = 0;
                    oParteMaxMin.Minimo = 0;

                    // Se verifica si aplica para 9500 y se guarda el dato Es9500
                    AdmonProc.VerGuardar9500(iParteID);
                }
                Datos.Guardar<ParteMaxMin>(oParteMaxMin);
            }
            // Se guardan los cambios para criterios predefinidos
            this.GuardarCriteriosGeneralesPredefinidos(this.trvPartesCalcular, this.CgCambiosCalcular);
            // Se limpian los cambios
            this.CgCambiosCalcular.Clear();

            // Se guardan los cambios de ventas globales, sólo en Matriz
            if (Util.Entero(this.cmbSucursal.SelectedValue) == Cat.Sucursales.Matriz)
            {
                foreach (var oCambio in this.CgCambiosVentasGlobales)
                {
                    int iParteID = Util.Entero(oCambio.Key);
                    var oParteMaxMin = Datos.GetEntity<ParteMaxMin>(q => q.SucursalID == Cat.Sucursales.Matriz && q.ParteID == iParteID);
                    oParteMaxMin.VentasGlobales = oCambio.Value;
                    Datos.Guardar<ParteMaxMin>(oParteMaxMin);
                }
                // Se guardan los cambios para criterios predefinidos
                this.GuardarCriteriosGeneralesPredefinidos(this.trvVentasGlobales, this.CgCambiosVentasGlobales);
                // Se limpian los cambios
                this.CgCambiosVentasGlobales.Clear();
            }

            Cargando.Cerrar();
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");
        }

        private void GuardarCriteriosGeneralesPredefinidos(TreeView oTreeView, Dictionary<string, bool> oCambios)
        {
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oYaGuardaros = new List<string>();

            foreach (var oCambio in oCambios)
            {
                // Si ya se guardó el registros correspondiente, se salta
                if (oYaGuardaros.Contains(oCambio.Key)) continue;

                // Se obtiene el nodo correspondiente, del TreeView
                var oNodoParte = this.ObtenerNodoDeParteID(oTreeView, oCambio.Key);

                // Se recorren todos los nodos, para determinar si su "check state" es el mismo
                bool? bComparacion = oNodoParte.Checked;
                foreach (TreeNode oNodo in oNodoParte.Parent.Nodes)
                {
                    if (bComparacion != null && oNodo.Checked != bComparacion)
                        bComparacion = null;

                    // Se agrega la parte a la lista de ya guardados, pues se guardará
                    oYaGuardaros.Add(oNodo.Name);
                }

                // Se procede a guardar
                int iProveedorID = Util.Entero(oNodoParte.Parent.Parent.Parent.Name);
                int iMarcaID = Util.Entero(oNodoParte.Parent.Parent.Name);
                int iLineaID = Util.Entero(oNodoParte.Parent.Name);
                // Se busca el criterio, en base al Proveedor, la Marca y la Línea
                var oCriterio = Datos.GetEntity<ParteMaxMinCriterioPredefinido>(q => q.SucursalID == iSucursalID
                    && q.ProveedorID == iProveedorID && q.MarcaID == iMarcaID && q.LineaID == iLineaID);
                // Si no existe, se crea
                if (oCriterio == null)
                {
                    oCriterio = new ParteMaxMinCriterioPredefinido()
                    {
                        SucursalID = iSucursalID,
                        ProveedorID = iProveedorID,
                        MarcaID = iMarcaID,
                        LineaID = iLineaID
                    };
                }
                //Se registra el cambio
                if (oTreeView == this.trvPartesCalcular)
                    oCriterio.Calcular = bComparacion;
                else
                    oCriterio.VentasGlobales = bComparacion;
                Datos.Guardar<ParteMaxMinCriterioPredefinido>(oCriterio);
            }
        }

        private TreeNode ObtenerNodoDeParteID(TreeView oTreeView, string sParteID)
        {
            foreach (TreeNode oNodoProveedor in oTreeView.Nodes)
            {
                foreach (TreeNode oNodoMarca in oNodoProveedor.Nodes)
                {
                    foreach (TreeNode oNodoLinea in oNodoMarca.Nodes)
                    {
                        foreach (TreeNode oNodoParte in oNodoLinea.Nodes)
                        {
                            if (oNodoParte.Name == sParteID)
                                return oNodoParte;
                        }
                    }
                }
            }
            return null;
        }

        private void LlenarReglas()
        {
            Cargando.Mostrar();
            this.dgvReglas.Rows.Clear();
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oReglas = Datos.GetListOf<ParteMaxMinRegla>(q => q.SucursalID == iSucursalID && q.Estatus).OrderBy(q => q.Orden);
            foreach (var oRegla in oReglas)
                this.dgvReglas.Rows.Add(oRegla.ParteMaxMinReglaID, Cat.TiposDeAfectacion.SinCambios, oRegla.Orden, oRegla.Regla
                    , oRegla.Condicion, oRegla.Maximo, oRegla.Minimo, oRegla.Descripcion);
            this.dgvReglas.AutoResizeRows();
            Cargando.Cerrar();
        }

        private void SubirBajarRegla(DataGridViewRow Fila, bool bSubir)
        {
            if (Fila == null) return;

            // Se obtiene la fila con la cual hacer el cambio, la siguiente o la anterior, según sea el caso
            DataGridViewRow oFilaCambio = null;
            do
            {
                if (bSubir)
                    oFilaCambio = Fila.FilaAnterior();
                else
                    oFilaCambio = Fila.FilaSiguiente();
            } while (oFilaCambio == null || Util.Entero(oFilaCambio.Cells["Reglas_Cambio"].Value) == Cat.TiposDeAfectacion.Borrar);
            if (oFilaCambio == null) return;

            // Se hace el cambio de orden
            Fila.Cells["Reglas_Orden"].Value = (Util.Entero(Fila.Cells["Reglas_Orden"].Value) + (bSubir ? -1 : 1));
            oFilaCambio.Cells["Reglas_Orden"].Value = (Util.Entero(oFilaCambio.Cells["Reglas_Orden"].Value) + (bSubir ? 1 : -1));
            Fila.DataGridView.Sort(this.dgvReglas.Columns["Reglas_Orden"], ListSortDirection.Ascending);
        }

        private void GuardarReglas()
        {
            Cargando.Mostrar();

            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            ParteMaxMinRegla oRegla = null;
            DataGridViewRow Fila = null;
            for (int iFila = 0; iFila < this.dgvReglas.Rows.Count; iFila++)
            {
                Fila = this.dgvReglas.Rows[iFila];
                int iCambio = Util.Entero(Fila.Cells["Reglas_Cambio"].Value);

                if (iCambio == Cat.TiposDeAfectacion.SinCambios)
                    continue;
                else if (iCambio == Cat.TiposDeAfectacion.Agregar)
                {
                    oRegla = new ParteMaxMinRegla();
                    oRegla.SucursalID = iSucursalID;
                }
                else
                {
                    int iReglaID = Util.Entero(Fila.Cells["ParteMaxMinReglaID"].Value);
                    oRegla = Datos.GetEntity<ParteMaxMinRegla>(q => q.ParteMaxMinReglaID == iReglaID && q.Estatus);
                }

                if (iCambio == Cat.TiposDeAfectacion.Borrar)
                {
                    // Se manda hacer el borrado lógico
                    Datos.Eliminar<ParteMaxMinRegla>(oRegla, true);
                    this.dgvReglas.Rows.Remove(Fila);
                    iFila--;
                }
                else
                {
                    // Se llenan los datos
                    oRegla.Orden = Util.Entero(Fila.Cells["Reglas_Orden"].Value);
                    oRegla.Regla = Util.Cadena(Fila.Cells["Reglas_Regla"].Value);
                    oRegla.Condicion = Util.Cadena(Fila.Cells["Reglas_Condicion"].Value);
                    oRegla.Maximo = Util.Cadena(Fila.Cells["Reglas_Maximo"].Value);
                    oRegla.Minimo = Util.Cadena(Fila.Cells["Reglas_Minimo"].Value);
                    oRegla.Descripcion = Util.Cadena(Fila.Cells["Reglas_Descripcion"].Value);
                    // Se valida la regla
                    if (!this.ValidarRegla(oRegla))
                    {
                        Fila.ErrorText = "Error al validar la regla.";
                        Fila.DefaultCellStyle.ForeColor = Color.Red;
                        continue;
                    }
                    // Se guarda
                    Datos.Guardar<ParteMaxMinRegla>(oRegla);
                    Fila.Cells["ParteMaxMinReglaID"].Value = oRegla.ParteMaxMinReglaID;
                    // Se restablece el color
                    Fila.Cells["Reglas_Cambio"].Value = Cat.TiposDeAfectacion.SinCambios;
                    this.dgvReglas.Rows[iFila].Cells["Reglas_Cambio"].Value = Cat.TiposDeAfectacion.SinCambios;
                    Fila.DefaultCellStyle.ForeColor = Color.Black;
                    Fila.ErrorText = "";
                }

            }

            Cargando.Cerrar();
        }

        private bool ValidarRegla(ParteMaxMinRegla oRegla)
        {
            return (
                oRegla.Orden > 0
                && oRegla.Condicion != ""
                && oRegla.Maximo != ""
                && oRegla.Minimo != ""
            );
        }

        private void LlenarEquivalentes()
        {
            Cargando.Mostrar();

            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);
            oParams.Add("SucursalID", Util.Entero(this.cmbSucursal.SelectedValue));
            if (this.cmbMarca.SelectedValue != null)
                oParams.Add("MarcaID", Util.Entero(this.cmbMarca.SelectedValue));
            if (this.cmbLinea.SelectedValue != null)
                oParams.Add("LineaID", Util.Entero(this.cmbLinea.SelectedValue));

            var oDatos = Datos.ExecuteProcedure<pauPartesMaxMinEquivalentes_Result>("pauPartesMaxMinEquivalentes", oParams);
            this.dgvEquivalentes.Rows.Clear();
            int iUltGrupo = (oDatos.Count > 0 ? oDatos[0].GrupoID : 0);
            foreach (var oEquiv in oDatos)
            {
                if (oEquiv.GrupoID != iUltGrupo)
                {
                    int iFila = this.dgvEquivalentes.Rows.Add();
                    this.dgvEquivalentes.Rows[iFila].Cells["equ_Sel"].ReadOnly = true;
                    this.dgvEquivalentes.Rows[iFila].Cells["equ_Sel"] = new DataGridViewTextBoxCell();
                    iUltGrupo = oEquiv.GrupoID;
                }

                this.dgvEquivalentes.Rows.Add(oEquiv.ParteID, oEquiv.GrupoID, oEquiv.Calcular, oEquiv.NumeroDeParte, oEquiv.Descripcion, oEquiv.Proveedor, oEquiv.Marca
                    , oEquiv.Existencia, oEquiv.Ventas, oEquiv.CostoConDescuento
                    , oEquiv.Minimo, oEquiv.Maximo, oEquiv.Minimo, oEquiv.Maximo, oEquiv.FechaModificacion);
            }

            Cargando.Cerrar();
        }

        private void ProcesarEquivalentes()
        {
            Cargando.Mostrar();

            int iFilaCostoMenor = -1;
            decimal mMaximo = 0, mMinimo = 0;
            int x = 0;
            for (int iFila = 0; iFila < this.dgvEquivalentes.Rows.Count; iFila++)
            {
                var oFila = this.dgvEquivalentes.Rows[iFila];

                if (oFila.Cells["equ_ParteID"].Value != null || iFila == (this.dgvEquivalentes.Rows.Count - 1))
                {
                    // Se calculan las variables
                    if (iFilaCostoMenor < 0 || Util.Decimal(oFila.Cells["equ_Costo"].Value) <
                        Util.Decimal(this.dgvEquivalentes["equ_Costo", iFilaCostoMenor].Value))
                        iFilaCostoMenor = iFila;
                    if (Util.Decimal(oFila.Cells["equ_Maximo"].Value) > mMaximo)
                    {
                        mMaximo = Util.Decimal(oFila.Cells["equ_Maximo"].Value);
                        mMinimo = Util.Decimal(oFila.Cells["equ_Minimo"].Value);
                    }
                    // Se ponen los datos como "vacíos" en todas las filas
                    oFila.Cells["equ_Sel"].Value = false;
                    oFila.Cells["equ_Minimo"].Value = 0;
                    oFila.Cells["equ_Maximo"].Value = 0;
                }

                // Si la fila está vacía o ya se acabaron las filas, se calculan los datos del grupo
                if (oFila.Cells["equ_ParteID"].Value == null || iFila == (this.dgvEquivalentes.Rows.Count - 1))
                {
                    this.dgvEquivalentes["equ_Sel", iFilaCostoMenor].Value = true;
                    this.dgvEquivalentes["equ_Minimo", iFilaCostoMenor].Value = mMinimo;
                    this.dgvEquivalentes["equ_Maximo", iFilaCostoMenor].Value = mMaximo;
                    iFilaCostoMenor = -1;
                    mMaximo = mMinimo = 0;
                }

            }

            Cargando.Cerrar();
        }

        private void CopiarEquivalentesDeSucursal(int iSucursalID)
        {
            var oGruposYa = new List<int>();
            foreach (DataGridViewRow oFila in this.dgvEquivalentes.Rows)
            {
                int iGrupo = Util.Entero(oFila.Cells["equ_GrupoID"].Value);
                if (iGrupo <= 0 || oGruposYa.Contains(iGrupo)) continue;
                oGruposYa.Add(iGrupo);

                bool bYaTienenMaxMin = false;
                decimal mMax = 0, mMin = 0, mCosto = 0;
                int iFilaMasEco = 0;
                var oEquivMaxMin = Datos.GetListOf<PartesEquivalentesMaxMinView>(c => c.GrupoID == iGrupo && c.SucursalID == iSucursalID)
                    .OrderByDescending(c => c.Maximo);
                foreach (var oReg in oEquivMaxMin)
                {
                    int iFila = this.dgvEquivalentes.EncontrarIndiceDeValor("equ_ParteID", oReg.ParteID);
                    var oFilaParte = this.dgvEquivalentes.Rows[iFila];
                    oFilaParte.Cells["equ_Sel"].Value = oReg.Calcular.Valor();

                    // Se verifica si uno de los marcados ya tiene MaxMin
                    if (!bYaTienenMaxMin && oReg.Calcular.Valor() && Util.Decimal(oFilaParte.Cells["equ_Maximo"].Value) > 0)
                    {
                        bYaTienenMaxMin = true;
                    }
                    else
                    {
                        // Se obtiene el Max y el Min más altos de una parte que no esté marcada, para usarla en caso de que los marcados no tengan MaxMin
                        if (!oReg.Calcular.Valor() && Util.Decimal(oFilaParte.Cells["equ_Maximo"].Value) > mMax)
                        {
                            mMax = Util.Decimal(oFilaParte.Cells["equ_Maximo"].Value);
                            mMin = Util.Decimal(oFilaParte.Cells["equ_Minimo"].Value);
                        }
                        // Para determinar la parte con menor costo
                        if (oReg.Calcular.Valor() && (mCosto == 0 || Util.Decimal(oFilaParte.Cells["equ_Costo"].Value) < mCosto))
                        {
                            mCosto = Util.Decimal(oFilaParte.Cells["equ_Costo"].Value);
                            iFilaMasEco = iFila;
                        }
                    }
                }

                if (!bYaTienenMaxMin)
                {
                    this.dgvEquivalentes["equ_Maximo", iFilaMasEco].Value = mMax;
                    this.dgvEquivalentes["equ_Minimo", iFilaMasEco].Value = mMin;
                }
            }
        }

        private void GuardarEquivalentes()
        {
            Cargando.Mostrar();

            // var oSucursales = General.GetListOf<Sucursal>(c => c.Estatus);
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            foreach (DataGridViewRow oFila in this.dgvEquivalentes.Rows)
            {
                if (oFila.Tag == null) continue;

                int iParteID = Util.Entero(oFila.Cells["equ_ParteID"].Value);
                bool bCalcular = Util.Logico(oFila.Cells["equ_Sel"].Value);
                decimal mMaximo = Util.Decimal(oFila.Cells["equ_Maximo"].Value);
                decimal mMinimo = Util.Decimal(oFila.Cells["equ_Minimo"].Value);
                // Se verifica si hubo cambios
                var oParte = Datos.GetEntity<ParteMaxMin>(c => c.ParteID == iParteID && c.SucursalID == iSucursalID);
                if (oParte.Calcular == bCalcular && oParte.Maximo == mMaximo && oParte.Minimo == mMinimo)
                    continue;  // No hubo cambios en realidad
                // Se realizan los cambios pertinentes
                oParte.Calcular = bCalcular;
                oParte.Maximo = mMaximo;
                oParte.Minimo = mMinimo;
                oParte.Fijo = bCalcular;
                // Se guardan los cambios
                Datos.Guardar<ParteMaxMin>(oParte);

                /* foreach (var oSucursal in oSucursales)
                {
                    var oParte = General.GetEntity<ParteMaxMin>(c => c.ParteID == iParteID && c.SucursalID == oSucursal.SucursalID);
                    oParte.Calcular = bCalcular;
                    Datos.Guardar<ParteMaxMin>(oParte);
                }
                */
            }

            Cargando.Cerrar();
        }

        private void LlenarSinMaxMin()
        {
            Cargando.Mostrar();

            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);

            var oSinMaxMin = Datos.ExecuteProcedure<pauPartesMaxMinSinCalcular_Result>("pauPartesMaxMinSinCalcular", oParams);
            this.dgvSinMaxMin.Rows.Clear();
            foreach (var oParte in oSinMaxMin)
            {
                this.dgvSinMaxMin.Rows.Add(oParte.ParteID, oParte.NumeroDeParte, oParte.Descripcion, oParte.Sucursal
                    , oParte.Proveedor, oParte.Marca, oParte.Linea, oParte.Cantidad);
            }
            this.dgvSinMaxMin.AutoResizeColumns();

            Cargando.Cerrar();
        }

        private void LlenarDescripcionMaxMin(DataGridViewRow oFila)
        {
            this.txtDescripcionMaxMin.Clear();
            if (oFila == null) return;
            int iRegla = Util.Entero(oFila.Cells["Condiciones"].Value);
            if (iRegla == 0) return;
            var oRegla = Datos.GetEntity<ParteMaxMinRegla>(c => c.ParteMaxMinReglaID == iRegla && c.Estatus);
            if (oRegla != null)
                this.txtDescripcionMaxMin.Text = string.Format("Condición: {0}\r\n{1}", iRegla, oRegla.Descripcion);
        }

        private void LlenarPartesIman()
        {
            Cargando.Mostrar();

            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", Util.Entero(this.cmbSucursal.SelectedValue));
            UtilLocal.AgregarParametroListaEntero(ref oParams, "Proveedores/tpuTablaEnteros", this.ctlProveedores);
            UtilLocal.AgregarParametroListaEntero(ref oParams, "Marcas/tpuTablaEnteros", this.ctlMarcas);
            UtilLocal.AgregarParametroListaEntero(ref oParams, "Lineas/tpuTablaEnteros", this.ctlLineas);
            if (this.txtImanNumeroDeParte.Text != "")
                oParams.Add("NumeroDeParte", this.txtImanNumeroDeParte.Text);
            if (this.txtImanDescripcion.Text != "")
                oParams.Add("Descripcion", this.txtImanDescripcion.Text);

            var oDatos = Datos.ExecuteProcedure<pauPartesBusqueda_Res>("pauPartesBusqueda", oParams);
            if (this.txtImanNumeroDeParte.Text == "")
                oDatos = oDatos.Where(c => c.Maximo > 0).ToList();
            this.dgvIman.Tag = this.cmbSucursal.SelectedValue;

            this.dgvIman.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                int iFila = this.dgvIman.Rows.Add(oReg.ParteID, true, oReg.NumeroDeParte, oReg.Descripcion, oReg.Proveedor, oReg.Marca, oReg.Linea
                    , oReg.Maximo, oReg.Minimo, oReg.FechaMaxMin, oReg.FechaIman);
                this.dgvIman.Rows[iFila].Tag = oReg;
            }

            Cargando.Cerrar();
        }

        private void ImprimirImanes()
        {
            var oDatos = new List<pauPartesBusqueda_Res>();
            foreach (DataGridViewRow oFila in this.dgvIman.Rows)
            {
                if (Util.Logico(oFila.Cells["ima_Sel"].Value))
                    oDatos.Add(oFila.Tag as pauPartesBusqueda_Res);
            }

            var oRep = new Report();
            oRep.Load(UtilLocal.RutaReportes("PartesImanes.frx"));
            oRep.RegisterData(oDatos, "Partes");
            oRep.SetParameterValue("Sucursal", this.cmbSucursal.Text);
            UtilLocal.EnviarReporteASalida("Reportes.Partes.Imanes.Salida", oRep);

            // Se guarda la fecha de impresión del imán, en cada parte
            Cargando.Mostrar();
            int iSucursalID = Util.Entero(this.dgvIman.Tag);
            DateTime dAhora = DateTime.Now;
            foreach (var oReg in oDatos)
            {
                var oParteMaxMin = Datos.GetEntity<ParteMaxMin>(c => c.SucursalID == iSucursalID && c.ParteID == oReg.ParteID);
                oParteMaxMin.FechaIman = dAhora;
                Datos.Guardar<ParteMaxMin>(oParteMaxMin);
            }
            Cargando.Cerrar();
        }

        #endregion

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }

    public static class MaxMinFunciones
    {
        static List<pauParteMaxMinDatosExtra_Result> oDatos;
        static List<pauParteMaxMinDatosExtra_Result> oSemanas;
        static List<pauParteMaxMinDatosExtra_Result> oMeses;

        #region [ Propiedades ]

        public static int ParteID { get; set; }
        public static int? SucursalID { get; set; }

        #endregion

        #region [ Privados ]

        private static void VerIniciarDatos()
        {
            if (MaxMinFunciones.oDatos == null)
            {
                DateTime dHasta = DateTime.Now.Date.DiaPrimero().AddDays(-1);
                var oParams = new Dictionary<string, object>();
                oParams.Add("ParteID", MaxMinFunciones.ParteID);
                oParams.Add("Hasta", dHasta);
                oParams.Add("Desde", dHasta.AddYears(-1).AddDays(1));
                oParams.Add("SucursalID", MaxMinFunciones.SucursalID);
                MaxMinFunciones.oDatos = Datos.ExecuteProcedure<pauParteMaxMinDatosExtra_Result>("pauParteMaxMinDatosExtra", oParams);
                MaxMinFunciones.oSemanas = oDatos.Where(c => c.Grupo == 3).ToList();
                MaxMinFunciones.oMeses = oDatos.Where(c => c.Grupo == 4).ToList();
            }
        }

        #endregion

        #region [ Públicos ]

        public static void Inicializar(int iParteID, int? iSucursalID)
        {
            MaxMinFunciones.ParteID = iParteID;
            MaxMinFunciones.SucursalID = iSucursalID;
            MaxMinFunciones.oDatos = null;
            MaxMinFunciones.oSemanas = null;
            MaxMinFunciones.oMeses = null;
        }

        public static decimal MaxSem(int iOrden)
        {
            MaxMinFunciones.VerIniciarDatos();

            if (MaxMinFunciones.oSemanas.Count >= iOrden)
                return (MaxMinFunciones.oSemanas[iOrden - 1].Cantidad + MaxMinFunciones.oSemanas[iOrden - 1].Negadas).Valor();
            else
                return 0;
        }

        public static decimal MaxMes(int iOrden)
        {
            MaxMinFunciones.VerIniciarDatos();

            if (MaxMinFunciones.oMeses.Count >= iOrden)
                return (MaxMinFunciones.oMeses[iOrden - 1].Cantidad + MaxMinFunciones.oMeses[iOrden - 1].Negadas).Valor();
            else
                return 0;
        }

        #endregion
    }
}
