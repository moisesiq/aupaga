using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using AdvancedDataGridView;

namespace Refaccionaria.App
{
    public partial class CuentasContables : UserControl
    {
        private class ColsCuentas
        {
            public const int Id = 0;
            public const int Cuenta = 1;
            public const int Porcentaje = 2;
            public const int Fiscal = 3;
            public const int Total = 4;
            public const int Matriz = 5;
            public const int Sucursal2 = 6;
            public const int Sucursal3 = 7;
            public const int ImporteDev = 8;
        }

        List<Sucursal> oSucursales;
        int iPosicionBusqueda;
        List<ContaModelos.IndiceCuentasContables> oIndiceCuentas = new List<ContaModelos.IndiceCuentasContables>();

        public CuentasContables()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuentasContables_Load(object sender, EventArgs e)
        {
            this.oSucursales = General.GetListOf<Sucursal>(c => c.Estatus);

            // Se inicializan las fechas
            this.dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);
            this.dtpHasta.Value = new DateTime(DateTime.Now.Year, 12, 31);

            // Se cargan los datos
            this.LlenarCuentasTotales();
        }

        private void dtpDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpDesde.Focused)
                this.LlenarCuentasTotales();
        }

        private void dtpHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpHasta.Focused)
                this.LlenarCuentasTotales();
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (!this.txtBusqueda.Focused) return;

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.iPosicionBusqueda++;
                    this.SeleccionarEncontrarCuenta(this.txtBusqueda.Text);
                    break;
                case Keys.Down:
                    this.dgvGastos.Focus();
                    break;
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.SeleccionarEncontrarCuenta(this.txtBusqueda.Text);
        }

        private void btnGastoAgregar_Click(object sender, EventArgs e)
        {
            // Se pre-selecciona la cuenta según la posición en el Grid
            var oGasto = new GastoContable();
            if (this.tgvCuentas.CurrentNode != null && this.tgvCuentas.CurrentNode.Level == 4)
            {
                int iId = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                oGasto.Load += new EventHandler((s, ea) =>
                {
                    oGasto.SeleccionarCuentaFinal(iId);
                });
            }

            var frmCont = new ContenedorControl("Agregar Gasto", oGasto);
            frmCont.FormBorderStyle = FormBorderStyle.Sizable;
            if (frmCont.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.LlenarCuentasTotales();
            frmCont.Dispose();
        }

        private void tgvCuentas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.tgvCuentas.VerSeleccionNueva())
            {
                if (this.tgvCuentas.CurrentNode != null && this.tgvCuentas.CurrentNode.Level == 4)
                {
                    int iCuentaID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                    this.LlenarMovimientosCuenta(iCuentaID);
                }
                else
                {
                    this.dgvGastos.Rows.Clear();
                    this.dgvGastoDev.Rows.Clear();
                }
            }

            // Para habilitar el botón de agregar Cuenta
            int? iNivel = (this.tgvCuentas.CurrentNode == null ? null : (int?)this.tgvCuentas.CurrentNode.Level);
            this.btnCuentaAgregar.Enabled = (iNivel < 4);
            this.btnCuentaEliminar.Enabled = (iNivel > 2);
            this.btnCuentaMover.Enabled = (iNivel == 4);
        }

        private void tgvCuentas_CurrentCellChanged(object sender, EventArgs e)
        {
            // Se cambió al evento CellClick porque se ejecutaba muchas veces al expandir o contraer el árbol.
        }

        private void tgvCuentas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.tgvCuentas.Columns[e.ColumnIndex].Name == "Cuentas_Cuenta")
                this.btnCuentaEditar_Click(sender, e);
        }

        private void btnCuentaAgregar_Click(object sender, EventArgs e)
        {
            int iNivel = this.tgvCuentas.CurrentNode.Level;
            int iID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
            var frmCuenta = new CuentaContable(true, (CuentaContable.Tipo)(iNivel), iID);
            if (frmCuenta.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.LlenarCuentasTotales();
            frmCuenta.Dispose();
        }

        private void btnCuentaEditar_Click(object sender, EventArgs e)
        {
            if (this.tgvCuentas.CurrentNode == null) return;

            int iNivel = this.tgvCuentas.CurrentNode.Level;
            int iID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
            var frmCuenta = new CuentaContable(false, (CuentaContable.Tipo)(iNivel - 1), iID);
            if (frmCuenta.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.LlenarCuentasTotales();
            frmCuenta.Dispose();
        }

        private void btnCuentaEliminar_Click(object sender, EventArgs e)
        {
            if (UtilLocal.MensajePregunta(string.Format("¿Estás seguro que deseas eliminar la cuenta \"{0}\"?", this.tgvCuentas.CurrentNode.Cells["Cuentas_Cuenta"].Value))
                == DialogResult.Yes)
            {
                int iCuentaID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                if (this.CuentaEliminar(this.tgvCuentas.CurrentNode.Level - 1, iCuentaID))
                    this.LlenarCuentasTotales();
            }
        }

        private void btnCuentaMover_Click(object sender, EventArgs e)
        {
            var oCuentas = General.GetListOf<ContaCuentasAuxiliaresView>().Select(c =>
                new { c.ContaCuentaDeMayorID, Nombre = (c.Cuenta + " - " + c.Subcuenta + " - " + c.CuentaDeMayor) }).Distinct().OrderBy(o => o.Nombre).ToList();
            var frmValor = new MensajeObtenerValor("Indica la Cuenta de Mayor a donde quieres mover la Cuenta seleccionada:", 0, MensajeObtenerValor.Tipo.Combo);
            frmValor.CargarCombo("ContaCuentaDeMayorID", "Nombre", oCuentas);
            frmValor.Width += 100;
            if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                int iCuentaID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == iCuentaID);
                oCuentaAux.ContaCuentaDeMayorID = Helper.ConvertirEntero(frmValor.Valor);
                Guardar.Generico<ContaCuentaAuxiliar>(oCuentaAux);
                this.LlenarCuentasTotales();
            }
            frmValor.Dispose();
        }
        
        private void dgvGastos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Se manda eliminar el gasto
                if (this.dgvGastos.CurrentRow == null) return;
                this.GastoEliminar(this.dgvGastos.CurrentRow);
            }
        }

        private void dgvGastos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvGastos.VerSeleccionNueva())
            {
                if (this.dgvGastos.CurrentRow == null)
                {
                    this.dgvGastoDev.Rows.Clear();
                }
                else
                {
                    int iEgresoID = Helper.ConvertirEntero(this.dgvGastos.CurrentRow.Cells["Gastos_ContaEgresoID"].Value);
                    this.LlenarGastoDev(iEgresoID);
                }
            }
        }

        private void dgvGastos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.btnEgresoDevengar_Click(sender, e);
        }

        private void dgvGastos_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void btnEgresoDevengar_Click(object sender, EventArgs e)
        {
            if (this.dgvGastos.CurrentRow == null) return;
            int iContaEgresoID = Helper.ConvertirEntero(this.dgvGastos.CurrentRow.Cells["Gastos_ContaEgresoID"].Value);

            var frmDev = new GastoDevengar(iContaEgresoID);
            if (frmDev.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                this.LlenarCuentasTotales();
                int iCuentaID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                this.LlenarMovimientosCuenta(iCuentaID);
            }
            frmDev.Dispose();
        }

        private void btnEgresoEditar_Click(object sender, EventArgs e)
        {
            if (this.dgvGastos.CurrentRow == null) return;
            int iContaEgresoID = Helper.ConvertirEntero(this.dgvGastos.CurrentRow.Cells["Gastos_ContaEgresoID"].Value);
            var frmEgreso = new ContenedorControl("Gasto Contable", new GastoContable(iContaEgresoID));
            if (frmEgreso.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                this.LlenarCuentasTotales();
                int iCuentaID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                this.LlenarMovimientosCuenta(iCuentaID);
            }
            frmEgreso.Dispose();
        }

        #endregion

        #region [ Métodos ]

        private void SeleccionarEncontrarCuenta(string sBusqueda)
        {
            // Se hace la búsqueda en el índice
            int iPos;
            TreeGridNode oNodo = null;
            for (iPos = this.iPosicionBusqueda; iPos < this.oIndiceCuentas.Count; iPos++)
            {
                var oIndice = this.oIndiceCuentas[iPos];
                if (oIndice.Cuenta.Contains(sBusqueda.ToLower()))
                {
                    if (oIndice.IndiceCuenta.HasValue)
                    {
                        oNodo = this.tgvCuentas.Nodes[oIndice.IndiceCuenta.Value];
                        oNodo.Expand();
                    }
                    if (oIndice.IndiceSubcuenta.HasValue)
                    {
                        oNodo = oNodo.Nodes[oIndice.IndiceSubcuenta.Value];
                        oNodo.Expand();
                    }
                    if (oIndice.IndiceCuentaDeMayor.HasValue)
                    {
                        oNodo = oNodo.Nodes[oIndice.IndiceCuentaDeMayor.Value];
                        oNodo.Expand();
                    }
                    if (oIndice.IndiceCuentaAuxiliar.HasValue)
                    {
                        oNodo = oNodo.Nodes[oIndice.IndiceCuentaAuxiliar.Value];
                        oNodo.Expand();
                    }
                    break;
                }
            }

            if (iPos >= this.oIndiceCuentas.Count)
            {
                this.iPosicionBusqueda = 0;
            }
            else
            {
                this.tgvCuentas.CurrentCell = this.tgvCuentas["Cuentas_Cuenta", oNodo.RowIndex];
                this.iPosicionBusqueda = iPos;
            }
        }

        private void AplicarColor(TreeGridNode oNodo)
        {
            decimal mTotal = Helper.ConvertirDecimal(oNodo.Cells[ColsCuentas.Total].Value);
            decimal mImporteDev = Helper.ConvertirDecimal(oNodo.Cells[ColsCuentas.ImporteDev].Value);

            // Se determinó que si el importe del gasto es cero, se marca en verde independientemente de si tiene devengado - Moi 12/07/2015
            if (mTotal == 0 || mImporteDev >= mTotal)
                oNodo.DefaultCellStyle.ForeColor = Color.FromArgb(79, 98, 40);
            else if (mImporteDev == 0)
                oNodo.DefaultCellStyle.ForeColor = Color.FromArgb(226, 107, 10);
            else
                oNodo.DefaultCellStyle.ForeColor = Color.FromArgb(58, 79, 109);
        }
                
        private void FormatoColumnasImporte(TreeGridNode oNodo)
        {
            for (int iCol = 2; iCol < this.tgvCuentas.Columns.Count; iCol++)
            {
                string sCol = this.tgvCuentas.Columns[iCol].Name;
                if (sCol != "Fiscal" && sCol != "Total" && sCol != "Matriz" && sCol != "Sucursal2" && sCol != "Sucursal3")
                    continue;

                if (oNodo.Cells[iCol].Value != null)
                    oNodo.Cells[iCol].Value = Helper.ConvertirDecimal(oNodo.Cells[iCol].Value).ToString(GlobalClass.FormatoMoneda);
            }
        }

        private void LlenarMovimientosCuenta(int iCuentaID)
        {
            DateTime dDesde = this.dtpDesde.Value.Date;
            DateTime dHasta = this.dtpHasta.Value.Date.AddDays(1);
            var oMovs = General.GetListOf<ContaEgresosView>(c => c.ContaCuentaAuxiliarID == iCuentaID && (c.Fecha >= dDesde && c.Fecha < dHasta))
                .OrderBy(c => c.Fecha);
            
            this.dgvGastos.Rows.Clear();
            foreach (var oMov in oMovs)
            {
                int iFila = this.dgvGastos.Rows.Add(oMov.ContaEgresoID, oMov.Fecha, oMov.FolioDePago, oMov.Importe, oMov.Sucursal
                    , oMov.FormaDePago, oMov.Usuario, oMov.Observaciones);
                // Se aplica el color
                // Se determinó que si el importe del gasto es cero, se marca en verde independientemente de si tiene devengado - Moi 12/07/2015
                if (oMov.Importe == 0 || oMov.ImporteDev.Valor() >= oMov.Importe)
                    this.dgvGastos.Rows[iFila].DefaultCellStyle.ForeColor = Color.FromArgb(79, 98, 40);
                else if (oMov.ImporteDev.Valor() == 0)
                    this.dgvGastos.Rows[iFila].DefaultCellStyle.ForeColor = Color.FromArgb(226, 107, 10);
                else
                    this.dgvGastos.Rows[iFila].DefaultCellStyle.ForeColor = Color.FromArgb(58, 79, 109);
            }

            /* this.lsvMovimientos.Items.Clear();
            foreach (var oMov in oMovs)
            {
                // Se agrega la fila al listview
                var oFila = this.lsvMovimientos.Items.Add(new ListViewItem(new string[] {
                    oMov.Fecha.ToString(), 
                    oMov.FolioDePago, 
                    oMov.Importe.ToString(GlobalClass.FormatoMoneda), 
                    oMov.Sucursal, 
                    oMov.FormaDePago, 
                    oMov.Usuario, 
                    oMov.Observaciones,
                    oMov.ContaEgresoID.ToString()
                }));
                // Se aplica el color
                if (oMov.ImporteDev.Valor() == 0)
                    oFila.ForeColor = Color.Orange;
                else if (oMov.ImporteDev.Valor() < oMov.Importe)
                    oFila.ForeColor = Color.Blue;
                else
                    oFila.ForeColor = Color.Green;
            }
            */
        }

        private void LlenarGastoDev(int iEgresoID)
        {
            var oDevs = General.GetListOf<ContaEgresoDevengado>(c => c.ContaEgresoID == iEgresoID);
            this.dgvGastoDev.Rows.Clear();
            foreach (var oDev in oDevs)
            {
                string sSucursal = this.oSucursales.FirstOrDefault(c => c.SucursalID == oDev.SucursalID).NombreSucursal;
                this.dgvGastoDev.Rows.Add(oDev.ContaEgresoDevengadoID, oDev.Fecha, sSucursal, oDev.Importe);
            }
        }

        private bool CuentaEliminar(int iNivel, int iCuentaID)
        {
            // Se valida que no se tengan cuentas hijas o movimientos
            switch (iNivel)
            {
                case 2: // Cuenta de Mayor
                    if (General.Exists<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaID))
                    {
                        UtilLocal.MensajeError("La Cuenta seleccionada tiene Subcuentas asignadas. No se puede eliminar.");
                        return false;
                    }
                    // Se procede a eliminar la cuenta
                    var oCuentaMay = General.GetEntity<ContaCuentaDeMayor>(c => c.ContaCuentaDeMayorID == iCuentaID);
                    Guardar.Eliminar<ContaCuentaDeMayor>(oCuentaMay);
                    break;
                case 3: // Cuenta auxiliar
                    if (General.Exists<ContaEgreso>(c => c.ContaCuentaAuxiliarID == iCuentaID))
                    {
                        UtilLocal.MensajeError("La Cuenta seleccionada ha sido utilizado en varios movimientos. No se puede eliminar.");
                        return false;
                    }
                    // Se manda eliminar la cuenta
                    ContaProc.CuentaAuxiliarEliminar(iCuentaID);
                    break;
            }

            return true;
        }

        private bool GastoEliminar(DataGridViewRow oFila)
        {
            // Se verifica si es un gasto desde caja, el cual no se puede borrar
            int iEgresoID = Helper.ConvertirEntero(oFila.Cells["Gastos_ContaEgresoID"].Value);
            if (General.Exists<CajaEgreso>(c => c.ContaEgresoID == iEgresoID))
            {
                UtilLocal.MensajeAdvertencia("El gasto especificado se creó desde caja. No se puede eliminar desde aquí.");
                return false;
            }

            if (UtilLocal.MensajePregunta(string.Format("¿Estás seguro que deseas eliminar el gasto \"{0}\"?", oFila.Cells["Gastos_Observaciones"].Value)) != DialogResult.Yes)
                return false;

            // Se borra el gasto, de manera lógica
            // var oEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iEgresoID && c.Estatus);
            // Guardar.Eliminar<ContaEgreso>(oEgreso, true);
            ContaProc.GastoEliminar(iEgresoID);

            // Se actualizan los datos
            this.dgvGastos.Rows.Clear();
            this.LlenarCuentasTotales();
            
            return true;
        }

        #endregion

        #region [ Públicos ]

        public void SeleccionarCuadroBusqueda()
        {
            this.txtBusqueda.Focus();
        }

        public void LlenarCuentasTotales()
        {
            Cargando.Mostrar();

            // Se guarda la selección actual
            var oRutaNodoAct = new List<int>();
            var oNodo = this.tgvCuentas.CurrentNode;
            while (oNodo != null && oNodo.Level > 0)
            {
                oRutaNodoAct.Insert(0, oNodo.Index);
                oNodo = oNodo.Parent;
            }

            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            // Se llenan los datos
            var oDatos = General.ExecuteProcedure<pauContaCuentasTotales_Result>("pauContaCuentasTotales", oParams);
            TreeGridNode oNodoCuenta = null, oNodoSubcuenta = null, oNodoCuentaDeMayor = null;
            string sCuenta = "", sSubcuenta = "", sCuentaDeMayor = "";
            this.tgvCuentas.Nodes.Clear();
            this.oIndiceCuentas.Clear();
            foreach (var oCuenta in oDatos)
            {
                // Nodo de Cuenta
                if (oCuenta.Cuenta != sCuenta)
                {
                    sCuenta = oCuenta.Cuenta;
                    oNodoCuenta = this.tgvCuentas.Nodes.Add(oCuenta.ContaCuentaID, sCuenta);
                    oNodoCuenta.DefaultCellStyle.Font = new Font(this.tgvCuentas.Font.FontFamily, 10);
                    this.oIndiceCuentas.Add(new ContaModelos.IndiceCuentasContables() { Cuenta = sCuenta.ToLower(), Nivel = 1, IndiceCuenta = oNodoCuenta.Index });
                    sSubcuenta = "";
                }
                // Nodo de Subcuenta
                if (oCuenta.Subcuenta == null)
                    continue;
                else if (oCuenta.Subcuenta != sSubcuenta)
                {
                    sSubcuenta = oCuenta.Subcuenta;
                    oNodoSubcuenta = oNodoCuenta.Nodes.Add(oCuenta.ContaSubcuentaID, sSubcuenta);
                    oNodoSubcuenta.DefaultCellStyle.Font = new Font(this.tgvCuentas.Font.FontFamily, 9);
                    this.oIndiceCuentas.Add(new ContaModelos.IndiceCuentasContables()
                    {
                        Cuenta = sSubcuenta.ToLower(),
                        Nivel = 2
                        ,
                        IndiceCuenta = oNodoCuenta.Index,
                        IndiceSubcuenta = oNodoSubcuenta.Index
                    });
                    sCuentaDeMayor = "";
                }
                // Nodo de Cuenta de mayor
                if (oCuenta.CuentaDeMayor == null)
                    continue;
                else
                    if (oCuenta.CuentaDeMayor != sCuentaDeMayor)
                    {
                        sCuentaDeMayor = oCuenta.CuentaDeMayor;
                        oNodoCuentaDeMayor = oNodoSubcuenta.Nodes.Add(oCuenta.ContaCuentaDeMayorID, sCuentaDeMayor);
                        oNodoCuentaDeMayor.DefaultCellStyle.Font = new Font(this.tgvCuentas.Font, FontStyle.Bold);
                        this.oIndiceCuentas.Add(new ContaModelos.IndiceCuentasContables()
                        {
                            Cuenta = sCuentaDeMayor.ToLower(),
                            Nivel = 3
                            ,
                            IndiceCuenta = oNodoCuenta.Index,
                            IndiceSubcuenta = oNodoSubcuenta.Index,
                            IndiceCuentaDeMayor = oNodoCuentaDeMayor.Index
                        });
                    }
                // Se agrega la cuenta auxiliar
                if (oCuenta.CuentaAuxiliar == null)
                    continue;
                var oNodoCuentaAux = oNodoCuentaDeMayor.Nodes.Add(
                    oCuenta.ContaCuentaAuxiliarID,
                    oCuenta.CuentaAuxiliar,
                    0,
                    oCuenta.Fiscal,
                    oCuenta.Total,
                    oCuenta.Matriz,
                    oCuenta.Suc02,
                    oCuenta.Suc03,
                    oCuenta.ImporteDev
                );
                this.oIndiceCuentas.Add(new ContaModelos.IndiceCuentasContables()
                {
                    Cuenta = oCuenta.CuentaAuxiliar.ToLower(),
                    Nivel = 4,
                    IndiceCuenta = oNodoCuenta.Index,
                    IndiceSubcuenta = oNodoSubcuenta.Index,
                    IndiceCuentaDeMayor = oNodoCuentaDeMayor.Index,
                    IndiceCuentaAuxiliar = oNodoCuentaAux.Index
                });

                // Se agregan totales para los niveles superiores
                decimal mImporte = 0;
                for (int iCol = 2; iCol < (this.tgvCuentas.Columns.Count - 1); iCol++)
                {
                    switch (iCol)
                    {
                        case ColsCuentas.Fiscal: mImporte = oCuenta.Fiscal.Valor(); break;
                        case ColsCuentas.Total: mImporte = oCuenta.Total.Valor(); break;
                        case ColsCuentas.Matriz: mImporte = oCuenta.Matriz.Valor(); break;
                        case ColsCuentas.Sucursal2: mImporte = oCuenta.Suc02.Valor(); break;
                        case ColsCuentas.Sucursal3: mImporte = oCuenta.Suc03.Valor(); break;
                        case ColsCuentas.ImporteDev: mImporte = oCuenta.ImporteDev.Valor(); break;
                    }
                    oNodoCuentaDeMayor.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuentaDeMayor.Cells[iCol].Value) + mImporte);
                    oNodoSubcuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoSubcuenta.Cells[iCol].Value) + mImporte);
                    oNodoCuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuenta.Cells[iCol].Value) + mImporte);
                }
            }
            
            // Se aplica el formato y el color, y se llena el porcentaje
            decimal mCuenta = 0, mSubcuenta = 0, mCuentaDeMayor = 0, mCuentaAux= 0;
            foreach (var oNodCuenta in this.tgvCuentas.Nodes)
            {
                oNodCuenta.Expand();
                mCuenta = Helper.ConvertirDecimal(oNodCuenta.Cells["Cuentas_Total"].Value);
                oNodCuenta.Cells["Cuentas_Porcentaje"].Value = "-";
                foreach (var oNodSubcuenta in oNodCuenta.Nodes)
                {
                    oNodSubcuenta.Expand();
                    mSubcuenta = Helper.ConvertirDecimal(oNodSubcuenta.Cells["Cuentas_Total"].Value);
                    if (mCuenta != 0)
                        oNodSubcuenta.Cells["Cuentas_Porcentaje"].Value = ((mSubcuenta / mCuenta) * 100);
                    foreach (var oNodCuentaDeMayor in oNodSubcuenta.Nodes)
                    {
                        mCuentaDeMayor = Helper.ConvertirDecimal(oNodCuentaDeMayor.Cells["Cuentas_Total"].Value);
                        if (mSubcuenta != 0)
                            oNodCuentaDeMayor.Cells["Cuentas_Porcentaje"].Value = ((mCuentaDeMayor / mSubcuenta) * 100);
                        foreach (var oNodCuentaAuxiliar in oNodCuentaDeMayor.Nodes)
                        {
                            this.AplicarColor(oNodCuentaAuxiliar);
                            this.FormatoColumnasImporte(oNodCuentaAuxiliar);

                            mCuentaAux = Helper.ConvertirDecimal(oNodCuentaAuxiliar.Cells[ColsCuentas.Total].Value);
                            if (mCuentaDeMayor != 0)
                                oNodCuentaAuxiliar.Cells[ColsCuentas.Porcentaje].Value = ((mCuentaAux / mCuentaDeMayor) * 100);
                        }
                        this.AplicarColor(oNodCuentaDeMayor);
                        this.FormatoColumnasImporte(oNodCuentaDeMayor);
                        oNodCuentaDeMayor.Collapse();
                    }
                    this.AplicarColor(oNodSubcuenta);
                    this.FormatoColumnasImporte(oNodSubcuenta);
                }
                this.AplicarColor(oNodCuenta);
                this.FormatoColumnasImporte(oNodCuenta);
            }

            // Se selecciona el nodo previamente seleccionado
            oNodo = (oRutaNodoAct.Count > 0 ? this.tgvCuentas.Nodes[oRutaNodoAct[0]] : null);
            for (int iNodo = 1; iNodo < oRutaNodoAct.Count; iNodo++)
            {
                oNodo.Expand();
                oNodo = oNodo.Nodes[oRutaNodoAct[iNodo]];
                this.tgvCuentas.CurrentCell = oNodo.Cells["Cuentas_Cuenta"];
            }

            Cargando.Cerrar();
        }

        #endregion

    }
}
