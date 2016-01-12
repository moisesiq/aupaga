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
    public partial class CuentasPolizas : UserControl
    {
        int iPosicionBusqueda;
        List<ContaModelos.IndiceCuentasContables> oIndiceCuentas = new List<ContaModelos.IndiceCuentasContables>();
        BindingSource bdsDetalle;
        class ColumnasArbol
        {
            public const int Id = 0;
            public const int Cuenta = 1;
            public const int Total = 1;
        }

        public CuentasPolizas()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuentasPolizas_Load(object sender, EventArgs e)
        {
            // Se inicializan las fechas
            this.dtpDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);
            this.dtpHasta.Value = new DateTime(DateTime.Now.Year, 12, 31);

            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));

            // Se cargan los datos
            this.LlenarArbol();
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
                    this.dgvDetalle.Focus();
                    break;
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.SeleccionarEncontrarCuenta(this.txtBusqueda.Text);
        }

        private void dtpDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpDesde.Focused)
                this.LlenarArbol();
        }

        private void dtpHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpHasta.Focused)
                this.LlenarArbol();
        }

        private void btnCuentaAgregar_Click(object sender, EventArgs e)
        {
            int iNivel = this.tgvCuentas.CurrentNode.Level;
            int iID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
            var frmCuenta = new CuentaContable(true, (CuentaContable.Tipo)(iNivel), iID);
            if (frmCuenta.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.LlenarArbol();
            frmCuenta.Dispose();
        }

        private void btnCuentaEditar_Click(object sender, EventArgs e)
        {
            if (this.tgvCuentas.CurrentNode == null) return;

            int iNivel = this.tgvCuentas.CurrentNode.Level;
            int iID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
            var frmCuenta = new CuentaContable(false, (CuentaContable.Tipo)(iNivel - 1), iID);
            if (frmCuenta.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.LlenarArbol();
            frmCuenta.Dispose();
        }

        private void btnCuentaEliminar_Click(object sender, EventArgs e)
        {
            if (UtilLocal.MensajePregunta(string.Format("¿Estás seguro que deseas eliminar la cuenta \"{0}\"?", this.tgvCuentas.CurrentNode.Cells["Cuentas_Cuenta"].Value))
                == DialogResult.Yes)
            {
                int iCuentaID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                if (this.CuentaEliminar(this.tgvCuentas.CurrentNode.Level - 1, iCuentaID))
                    this.LlenarArbol();
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
                this.LlenarArbol();
            }
            frmValor.Dispose();
        }

        private void btnPolizaAgregar_Click(object sender, EventArgs e)
        {
            var frmPoliza = new PolizaContable();
            frmPoliza.ShowDialog(Principal.Instance);
            frmPoliza.Dispose();
        }

        private void btnRecibirResguardo_Click(object sender, EventArgs e)
        {
            this.RecibirResguardo();
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
                    // this.dgvDetalle.Rows.Clear();
                    this.dgvDetalle.DataSource = null;
                    this.dgvPoliza.Rows.Clear();
                    this.txtConceptoPoliza.Clear();
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

        private void txtBusquedaPolizaDet_TextChanged(object sender, EventArgs e)
        {
            if (this.txtBusquedaPolizaDet.Text == "")
                this.bdsDetalle.RemoveFilter();
            else
                this.bdsDetalle.Filter = this.dgvDetalle.ObtenerCadenaDeFiltro(this.txtBusquedaPolizaDet.Text);

            this.MovimientosCuentaFormatoAdicional();
        }

        private void cmbSucursal_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused && this.cmbSucursal.Text == "")
                this.cmbSucursal_SelectedIndexChanged(sender, e);
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused)
            {
                int iCuentaID = Helper.ConvertirEntero(this.tgvCuentas.CurrentNode.Cells["Cuentas_Id"].Value);
                this.LlenarMovimientosCuenta(iCuentaID);
            }
        }

        private void dgvDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                int iPolizaID = Helper.ConvertirEntero(this.dgvDetalle.CurrentRow.Cells["ContaPolizaID"].Value);
                this.BorrarPoliza(iPolizaID);
            }
        }

        private void dgvDetalle_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDetalle.VerSeleccionNueva())
            {
                if (this.dgvDetalle.CurrentRow == null)
                {
                    this.dgvPoliza.Rows.Clear();
                    this.txtConceptoPoliza.Clear();
                }
                else
                {
                    int iPolizaID = Helper.ConvertirEntero(this.dgvDetalle.CurrentRow.Cells["ContaPolizaID"].Value);
                    this.LlenarPoliza(iPolizaID);
                }
            }
        }

        private void dgvDetalle_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;
            int iPolizaID = Helper.ConvertirEntero(this.dgvDetalle.CurrentRow.Cells["ContaPolizaID"].Value);
            var frmPoliza = new PolizaContable();
            frmPoliza.ModPolizaID = iPolizaID;
            frmPoliza.HacerModificacionMinima();
            if (frmPoliza.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.LlenarArbol();
            frmPoliza.Dispose();
        }

        private void dgvDetalle_Sorted(object sender, EventArgs e)
        {
            this.MovimientosCuentaFormatoAdicional();
        }

        private void btnPolizaCambiarSucursal_Click(object sender, EventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;

            // Se pide la nueva sucursal
            var frmValor = new MensajeObtenerValor("Selecciona la nueva sucursal:", "", MensajeObtenerValor.Tipo.Combo);
            frmValor.CargarCombo("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                int iPolizaID = Helper.ConvertirEntero(this.dgvDetalle.CurrentRow.Cells["ContaPolizaID"].Value);
                var oPoliza = General.GetEntity<ContaPoliza>(c => c.ContaPolizaID == iPolizaID);
                oPoliza.SucursalID = Helper.ConvertirEntero(frmValor.Valor);
                Guardar.Generico<ContaPoliza>(oPoliza);
                this.LlenarArbol();
            }
            frmValor.Dispose();
        }

        private void btnPolizasCambiarFecha_Click(object sender, EventArgs e)
        {
            this.PolizasCambiarFecha();
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
                    if (oIndice.IndiceSubcuenta.HasValue) {
                        oNodo = oNodo.Nodes[oIndice.IndiceSubcuenta.Value];
                        oNodo.Expand();
                    }
                    if (oIndice.IndiceCuentaDeMayor.HasValue) {
                        oNodo = oNodo.Nodes[oIndice.IndiceCuentaDeMayor.Value];
                        oNodo.Expand();
                    }
                    if (oIndice.IndiceCuentaAuxiliar.HasValue) {
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
            Cargando.Mostrar();

            DateTime dDesde = this.dtpDesde.Value.Date;
            DateTime dHasta = this.dtpHasta.Value.Date;
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            // var oMovs = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => 
            //     c.ContaCuentaAuxiliarID == iCuentaID && (c.FechaPoliza >= dDesde && c.FechaPoliza < dHasta) && (iSucursalID == 0 || c.SucursalID == iSucursalID))
            //     .OrderBy(c => c.FechaPoliza).ToList();
            var oParams = new Dictionary<string, object>();
            oParams.Add("CuentaAuxiliarID", iCuentaID);
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);
            if (iSucursalID > 0)
                oParams.Add("SucursalID", iSucursalID);
            var oMovs = General.ExecuteProcedure<pauContaCuentaAuxiliarPolizas_Result>("pauContaCuentaAuxiliarPolizas", oParams);

            // Se llena a partir de un DataTable
            this.bdsDetalle = new BindingSource();
            this.bdsDetalle.DataSource = Helper.ListaEntityADataTable<pauContaCuentaAuxiliarPolizas_Result>(oMovs);
            this.dgvDetalle.Columns.Clear();
            this.dgvDetalle.DataSource = null;
            this.dgvDetalle.DataSource = this.bdsDetalle;
            if (oMovs.Count <= 0) return;
            // Se configuran las columnas
            this.dgvDetalle.OcultarColumnas("ContaPolizaDetalleID", "FueManual", "Error");
            this.dgvDetalle.Columns["ContaPolizaID"].HeaderText = "Póliza";
            this.dgvDetalle.Columns["Concepto"].HeaderText = "Observación";
            // Se agrega la columna de "Manual"
            this.dgvDetalle.Columns.Add("Manual", "Manual");
            //
            // this.dgvDetalle.AutoResizeColumns();
            this.dgvDetalle.Columns["Fecha"].Width = 136;
            this.dgvDetalle.Columns["ContaPolizaID"].Width = 50;
            this.dgvDetalle.Columns["Referencia"].Width = 80;
            this.dgvDetalle.Columns["Sucursal"].Width = 80;
            this.dgvDetalle.Columns["Concepto"].Width = 280;
            this.dgvDetalle.Columns["Manual"].Width = 50;
            this.dgvDetalle.Columns["Cargo"].FormatoMoneda();
            this.dgvDetalle.Columns["Abono"].FormatoMoneda();
            // Se marcan en rojo las que tengan error
            this.MovimientosCuentaFormatoAdicional();

            /* this.dgvDetalle.Rows.Clear();
            foreach (var oMov in oMovs)
            {
                int iFila = this.dgvDetalle.Rows.Add(oMov.ContaPolizaDetalleID, oMov.FechaPoliza, oMov.ContaPolizaID.ToString(), oMov.Referencia
                    , oMov.Cargo, oMov.Abono, oMov.Sucursal, oMov.ConceptoPoliza);
                // Se aplica el color de error, cuando hay
                if (oMov.Error.Valor())
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }
            */

            // Si existían un filtro previamente, se ejecuta nuevamente
            if (this.txtBusquedaPolizaDet.Text != "")
                this.txtBusquedaPolizaDet_TextChanged(this, null);

            Cargando.Cerrar();
        }

        private void MovimientosCuentaFormatoAdicional()
        {
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                if (Helper.ConvertirBool(oFila.Cells["Error"].Value))
                    oFila.DefaultCellStyle.ForeColor = Color.Red;

                if (Helper.ConvertirBool(oFila.Cells["FueManual"].Value))
                {
                    oFila.Cells["Manual"].Value = "Sí";
                    oFila.DefaultCellStyle.Font = new Font(this.dgvDetalle.Font, FontStyle.Bold);
                }
            }
        }

        private void BorrarPoliza(int iPolizaID)
        {
            // Se valida el permiso
            var oResU = UtilDatos.ValidarObtenerUsuario("Contabilidad.Polizas.Borrar");
            if (oResU.Error) return;
            // Se borra la póliza
            ContaProc.BorrarPoliza(iPolizaID);
            this.LlenarArbol();
        }

        private void LlenarPoliza(int iPolizaID)
        {
            var oDetalle = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.ContaPolizaID == iPolizaID);
            this.dgvPoliza.Rows.Clear();
            foreach (var oReg in oDetalle)
                this.dgvPoliza.Rows.Add(oReg.ContaPolizaDetalleID, oReg.CuentaAuxiliar, oReg.Cargo, oReg.Abono, oReg.Sucursal, oReg.Referencia);
            this.lblOrigenPoliza.Text = oDetalle[0].OrigenPoliza;
            this.txtConceptoPoliza.Text = (oDetalle.Count > 0 ? oDetalle[0].ConceptoPoliza : "");
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
                    // Se manda eliminar la cuenta
                    if (!ContaProc.CuentaAuxiliarEliminar(iCuentaID).Respuesta)
                        return false;
                    break;
            }

            return true;
        }

        private void RecibirResguardo()
        {
            // Se obtiene la lista de los resugardos posibles para traspaso
            var oResg = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.Resguardo
                && c.SucursalID != Cat.Sucursales.Matriz && c.Cargo > 0).OrderBy(c => c.SucursalID).ToList();
            // Se quintan los resguardos marcados para ocultar manualmente, y también los no mover
            for (int i = 0; i < oResg.Count; i++)
            {
                int iPolDetID = oResg[i].ContaPolizaDetalleID;
                if (General.Exists<ContaPolizaResguardoOcultar>(c => c.ContaPolizaDetalleID == iPolDetID))
                {
                    oResg.RemoveAt(i--);
                    continue;
                }

                // Para quitar los resguardos de "NO MOVER"
                if (oResg[i].Referencia == "NO MOVER")
                    oResg.RemoveAt(i--);
            }
            // Se crea el formulario para el listado
            var frmListado = new ObtenerElementoLista("Selecciona el resguardo a recibir", oResg);
            frmListado.MostrarColumnas("Sucursal", "FechaPoliza", "Cargo");
            frmListado.dgvDatos.Columns["Sucursal"].DisplayIndex = 0;
            frmListado.dgvDatos.Columns["FechaPoliza"].HeaderText = "Fecha";
            frmListado.dgvDatos.Columns["Cargo"].HeaderText = "Importe";
            frmListado.dgvDatos.Columns["Cargo"].FormatoMoneda();
            // Se agrega opción (con menú contextual) para ocultar resguardos
            var cmsMenu = new ContextMenuStrip();
            cmsMenu.Items.Add("Ocultar");
            frmListado.dgvDatos.MouseClick += new MouseEventHandler((s, e) =>
            {
                if (e.Button != MouseButtons.Right) return;
                if (frmListado.dgvDatos.CurrentRow == null) return;
                cmsMenu.Show(frmListado.dgvDatos, new Point(e.X, e.Y));
            });
            cmsMenu.Items[0].Click += new EventHandler((s, e) =>
            {
                // if (frmListado.dgvDatos.CurrentRow == null) return;
                int iPolizaDetID = Helper.ConvertirEntero(frmListado.dgvDatos.CurrentRow.Cells["ContaPolizaDetalleID"].Value);
                var oOcultar = new ContaPolizaResguardoOcultar() { ContaPolizaDetalleID = iPolizaDetID };
                Guardar.Generico<ContaPolizaResguardoOcultar>(oOcultar);
                int iFila = frmListado.dgvDatos.CurrentRow.Index;
                frmListado.dgvDatos.CurrentCell = null;
                frmListado.dgvDatos.Rows[iFila].Visible = false;
            });
            // Se muestra el formulario con el listado
            if (frmListado.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                // Se pide el usuario
                var oResU = UtilDatos.ValidarObtenerUsuario();
                if (oResU.Exito)
                {
                    // Se procede a guardar la póliza
                    var oPolizaV = (frmListado.Seleccion as ContaPolizasDetalleAvanzadoView);
                    string sFecha = DateTime.Now.ToString(GlobalClass.FormatoFecha);
                    // Se modifica la póliza original, para quitar el resguardo
                    var oPolResguardo = General.GetEntity<ContaPolizaDetalle>(c => c.ContaPolizaID == oPolizaV.ContaPolizaID
                        && c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.Resguardo);
                    oPolResguardo.Cargo = 0;
                    Guardar.Generico<ContaPolizaDetalle>(oPolResguardo);
                    var oPoliza = General.GetEntity<ContaPoliza>(c => c.ContaPolizaID == oPolizaV.ContaPolizaID);
                    oPoliza.Concepto = string.Format("{0} / {1} / {2} / {3}", oPolizaV.Sucursal, sFecha, oResU.Respuesta.NombreUsuario, oPoliza.Concepto);
                    Guardar.Generico<ContaPoliza>(oPoliza);
                    // Ahora se crea una nueva póliza, para meter el resguadro a matriz
                    oPoliza = ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, string.Format("RESGUARDO / {1} / {2} / {3}", DateTime.Now, oPolizaV.Sucursal
                        , sFecha, oResU.Respuesta.NombreUsuario), Cat.ContaCuentasAuxiliares.Resguardo, Cat.ContaCuentasAuxiliares.Caja, oPolizaV.Cargo, "RESGUARDO"
                        , null, null, Cat.Sucursales.Matriz);
                    var oPolCaja = General.GetEntity<ContaPolizaDetalle>(c => c.ContaPolizaID == oPoliza.ContaPolizaID
                        && c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.Caja);
                    oPolCaja.Abono = 0;
                    Guardar.Generico<ContaPolizaDetalle>(oPolCaja);

                    //
                    this.LlenarArbol();
                }
            }
            frmListado.Dispose();
        }

        private void PolizasCambiarFecha()
        {
            if (this.dgvDetalle.SelectedRows.Count <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna póliza seleccionada.");
                return;
            }

            if (UtilLocal.MensajePregunta(string.Format("Se han seleccionado {0} pólizas. ¿Estás seguro que deseas cambiar la fecha de todas esas pólizas?"
                , this.dgvDetalle.SelectedRows.Count)) != DialogResult.Yes)
                return;

            // Se pide la nueva sucursal
            var frmValor = new MensajeObtenerValor("Selecciona la nueva fecha:", DateTime.Now, MensajeObtenerValor.Tipo.Fecha);
            if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                DateTime dNueva = Helper.ConvertirFechaHora(frmValor.Valor).Date;
                foreach (DataGridViewRow oFila in this.dgvDetalle.SelectedRows)
                {
                    int iPolizaID = Helper.ConvertirEntero(oFila.Cells["ContaPolizaID"].Value);
                    var oPoliza = General.GetEntity<ContaPoliza>(c => c.ContaPolizaID == iPolizaID);
                    oPoliza.Fecha = (dNueva.Add(oPoliza.Fecha - oPoliza.Fecha.Date));
                    Guardar.Generico<ContaPoliza>(oPoliza);
                }
                this.LlenarArbol();
            }
            frmValor.Dispose();
        }

        #endregion

        #region [ Públicos ]

        public void SeleccionarCuadroBusqueda()
        {
            this.txtBusqueda.Focus();
        }

        public void LlenarArbol()
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
            
            // Se configuran los parámetros
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);

            // Se llenan los datos
            var oDatos = General.ExecuteProcedure<pauContaCuentasPolizas_Result>("pauContaCuentasPolizas", oParams);
            TreeGridNode oNodoCuenta = null, oNodoSubcuenta = null, oNodoCuentaDeMayor = null;
            string sCuenta = "", sSubcuenta = "", sCuentaDeMayor = "";
            TreeGridNode oNodoClientes = null;
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
                    this.oIndiceCuentas.Add(new ContaModelos.IndiceCuentasContables() { Cuenta = sSubcuenta.ToLower(), Nivel = 2
                        , IndiceCuenta = oNodoCuenta.Index, IndiceSubcuenta = oNodoSubcuenta.Index });
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
                        this.oIndiceCuentas.Add(new ContaModelos.IndiceCuentasContables() { Cuenta = sCuentaDeMayor.ToLower(), Nivel = 3
                            , IndiceCuenta = oNodoCuenta.Index, IndiceSubcuenta = oNodoSubcuenta.Index, IndiceCuentaDeMayor = oNodoCuentaDeMayor.Index });

                        if (oCuenta.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Clientes)
                            oNodoClientes = oNodoCuentaDeMayor;
                    }
                // Se agrega la cuenta auxiliar
                if (oCuenta.CuentaAuxiliar == null)
                    continue;
                var oNodoCuentaAux = oNodoCuentaDeMayor.Nodes.Add(
                    oCuenta.ContaCuentaAuxiliarID,
                    oCuenta.CuentaAuxiliar,
                    oCuenta.Total,
                    oCuenta.Matriz,
                    oCuenta.Suc02,
                    oCuenta.Suc03
                );
                this.oIndiceCuentas.Add(new ContaModelos.IndiceCuentasContables() { Cuenta = oCuenta.CuentaAuxiliar.ToLower(), Nivel = 4
                    , IndiceCuenta = oNodoCuenta.Index, IndiceSubcuenta = oNodoSubcuenta.Index, IndiceCuentaDeMayor = oNodoCuentaDeMayor.Index
                    , IndiceCuentaAuxiliar = oNodoCuentaAux.Index });

                // Se marca con rojo si hay error
                if (oCuenta.Error)
                    oNodoCuentaAux.DefaultCellStyle.ForeColor = Color.Red;

                // Se agregan totales para los niveles superiores
                decimal mImporte = 0;
                for (int iCol = 2; iCol < this.tgvCuentas.Columns.Count; iCol++)
                {
                    switch (iCol)
                    {
                        case 2: mImporte = oCuenta.Total.Valor(); break;
                        case 3: mImporte = oCuenta.Matriz.Valor(); break;
                        case 4: mImporte = oCuenta.Suc02.Valor(); break;
                        case 5: mImporte = oCuenta.Suc03.Valor(); break;
                    }
                    oNodoCuentaDeMayor.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuentaDeMayor.Cells[iCol].Value) + mImporte);
                    oNodoSubcuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoSubcuenta.Cells[iCol].Value) + mImporte);
                    oNodoCuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuenta.Cells[iCol].Value) + mImporte);
                }
            }

            // Se aplica el formato y el color
            foreach (var oNodCuenta in this.tgvCuentas.Nodes)
            {
                oNodCuenta.Expand();
                foreach (var oNodSubcuenta in oNodCuenta.Nodes)
                {
                    oNodSubcuenta.Expand();
                    foreach (var oNodCuentaDeMayor in oNodSubcuenta.Nodes)
                    {
                        foreach (var oNodCuentaAuxiliar in oNodCuentaDeMayor.Nodes)
                        {
                            // this.AplicarColor(oNodCuentaAuxiliar);
                            this.FormatoColumnasImporte(oNodCuentaAuxiliar);

                            // Se marcan los niveles superiores con error, si aplica
                            if (oNodCuentaAuxiliar.DefaultCellStyle.ForeColor == Color.Red)
                            {
                                oNodCuentaDeMayor.DefaultCellStyle.ForeColor = Color.Red;
                                oNodSubcuenta.DefaultCellStyle.ForeColor = Color.Red;
                                oNodCuenta.DefaultCellStyle.ForeColor = Color.Red;
                            }
                        }
                        // this.AplicarColor(oNodCuentaDeMayor);
                        this.FormatoColumnasImporte(oNodCuentaDeMayor);
                        oNodCuentaDeMayor.Collapse();
                    }
                    // this.AplicarColor(oNodSubcuenta);
                    this.FormatoColumnasImporte(oNodSubcuenta);
                }
                // this.AplicarColor(oNodCuenta);
                this.FormatoColumnasImporte(oNodCuenta);
            }

            // Se marcan las diferencias en adeudos, en la cuenta de Clientes,
            /* if (oNodoClientes != null)
            {
                foreach (var oNodoCliente in oNodoClientes.Nodes)
                {
                    int iCuentaAuxID = Helper.ConvertirEntero(oNodoCliente.Cells[ColumnasArbol.Id].Value);
                    var oCuentaAuxCliente = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == iCuentaAuxID);
                    if (oCuentaAuxCliente == null || !oCuentaAuxCliente.RelacionID.HasValue)
                    {
                        oNodoCliente.DefaultCellStyle.ForeColor = Color.Red;
                    }
                    else
                    {
                        int iClienteID = oCuentaAuxCliente.RelacionID.Valor();
                        var oClienteAd = General.GetEntity<ClientesCreditoView>(c => c.ClienteID == iClienteID);
                        decimal mTotal = Helper.ConvertirDecimal(oNodoCliente.Cells[ColumnasArbol.Total].Value);
                        if (mTotal != oClienteAd.AdeudoVencido.Valor())
                            oNodoCliente.DefaultCellStyle.ForeColor = Color.Blue;
                    }
                }
            }
            */

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
