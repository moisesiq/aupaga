using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ContaBancos : UserControl
    {
        public ContaBancos()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public int ConBancoCuentaID
        {
            get { return Helper.ConvertirEntero(this.cmbBancoCuenta.SelectedValue); }
        }

        #endregion

        #region [ Eventos ]

        private void ContaBancos_Load(object sender, EventArgs e)
        {
            // Se configuran los controles
            this.cmbBancoCuenta.CargarDatos("BancoCuentaID", "NombreDeCuenta", General.GetListOf<BancoCuenta>());
            this.cmbBancoCuenta.SelectedValue = Cat.CuentasBancarias.Scotiabank;
            this.dtpConDesde.Value = DateTime.Now.DiaPrimero();
            this.dtpConHasta.Value = DateTime.Now.DiaUltimo();

            // Se llenan las asignaciones
            this.LlenarAsignaciones();
        }

        private void tabBancos_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabBancos.SelectedTab.Name)
            {
                case "tbpAsignacion":
                    if (this.dgvAsignacion.Rows.Count <= 0)
                        this.LlenarAsignaciones();
                    break;
                case "tbpConciliacion":
                    if (this.dgvConciliacion.Rows.Count <= 0)
                        this.LlenarConciliaciones();
                    break;
            }
        }

        #region [ Asignación ]

        private void dgvAsignacion_KeyDown(object sender, KeyEventArgs e)
        {
            this.dgvAsignacion.VerEspacioCheck(e, "asi_Sel");
        }

        private void dgvAsignacion_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvAsignacion.CurrentRow == null) return;
            this.dgvAsignacion.CurrentRow.Cells["asi_Sel"].Value = true;
            this.Asignar();
        }

        private void chkAsiMostrarTodos_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpAsiDesde.Enabled = this.chkAsiMostrarTodos.Checked;
            this.dtpAsiHasta.Enabled = this.chkAsiMostrarTodos.Checked;
            if (this.chkAsiMostrarTodos.Focused)
                this.LlenarAsignaciones();
        }

        private void dtpAsiDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpAsiDesde.Focused)
                this.LlenarAsignaciones();
        }

        private void dtpAsiHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpAsiHasta.Focused)
                this.LlenarAsignaciones();
        }

        private void btnAsiActualizar_Click(object sender, EventArgs e)
        {
            this.LlenarAsignaciones();
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            this.Asignar();
        }

        #endregion

        #region [ Conciliación ]

        private void cmbBancoCuenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBancoCuenta.Focused)
                this.LlenarConciliaciones();
        }

        private void dtpConDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpConDesde.Focused)
                this.LlenarConciliaciones();
        }

        private void dtpConHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpConHasta.Focused)
                this.LlenarConciliaciones();
        }

        private void btnConMostrar_Click(object sender, EventArgs e)
        {
            // this.LlenarConciliaciones();
        }

        private void txtConBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (this.txtConBusqueda.Focused)
                this.dgvConciliacion.EncontrarContiene(this.txtConBusqueda.Text, "con_Concepto", "con_Referencia", "con_Depositos", "con_Retiros");
        }

        private void dgvConciliacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvConciliacion.CurrentRow == null) return;

            switch (e.KeyCode)
            {
                case Keys.Space:
                    this.dgvConciliacion.VerEspacioCheck(e, "con_Sel");
                    break;
                case Keys.Delete:
                    this.BorrarMovimiento(this.dgvConciliacion.CurrentRow);
                    break;
            }
        }

        private void dgvConciliacion_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvConciliacion.VerSeleccionNueva() && this.dgvConciliacion.CurrentRow != null)
            {
                int iMovID = Helper.ConvertirEntero(this.dgvConciliacion.CurrentRow.Cells["con_BancoCuentaMovimientoID"].Value);
                this.LlenarMovimientosGrupo(iMovID);
            }
        }

        private void dgvConciliacion_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvConciliacion.VerDirtyStateChanged("con_Sel");
        }

        private void dgvConciliacion_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            switch (this.dgvConciliacion.Columns[e.ColumnIndex].Name)
            {
                case "con_Sel":
                    this.CalcularImporteSeleccion();
                    this.dgvConciliacion.CurrentRow.Tag = 1;
                    break;
                case "con_FechaAsignado":
                    this.dgvConciliacion.CurrentRow.Tag = 2;
                    break;
                case "con_Observacion":
                    this.dgvConciliacion.CurrentRow.Tag = 3;
                    break;
            }
        }

        private void dgvConciliacion_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.cmsConciliados.Show(this.dgvConciliacion, new Point(e.X, e.Y));
        }

        private void smiDesasignar_Click(object sender, EventArgs e)
        {
            if (this.dgvConciliacion.CurrentRow == null) return;
            int iMovID = Helper.ConvertirEntero(this.dgvConciliacion.CurrentRow.Cells["con_BancoCuentaMovimientoID"].Value);
            if (ContaProc.DesasignarMovimientoBancario(iMovID))
                this.LlenarConciliaciones();
        }

        private void dgvConciliacionDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvConciliacionDetalle.CurrentRow == null) return;

            this.dgvConciliacionDetalle.VerEspacioCheck(e, "cnd_Sel");
        }

        private void dgvConciliacionDetalle_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvConciliacionDetalle.VerDirtyStateChanged("cnd_Sel");
        }

        private void btnAgrupar_Click(object sender, EventArgs e)
        {
            this.AgruparMovimientos();
        }

        private void btnDesagrupar_Click(object sender, EventArgs e)
        {
            if (this.dgvConciliacion.CurrentRow == null) return;
            int iMovID = Helper.ConvertirEntero(this.dgvConciliacion.CurrentRow.Cells["con_BancoCuentaMovimientoID"].Value);
            this.DesagruparMovimientos(iMovID);
        }

        private void btnConTraspaso_Click(object sender, EventArgs e)
        {
            this.HacerTraspaso();
        }

        private void btnConDepositoEfectivo_Click(object sender, EventArgs e)
        {
            var frmDeposito = new DepositoEfectivo(this.ConBancoCuentaID);
            if (frmDeposito.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.LlenarConciliaciones();
            frmDeposito.Dispose();
        }

        private void btnConAgregarMovimiento_Click(object sender, EventArgs e)
        {
            this.AgregarMovimiento();
        }

        private void btnConGuardar_Click(object sender, EventArgs e)
        {
            if (UtilLocal.MensajePreguntaCancelar("¿Estás seguro que deseas realizar las conciliaciones especificadas?") == DialogResult.Yes)
                this.GuardarConciliacion();
        }

        #endregion

        #endregion

        #region [ Métodos ]

        private void LlenarAsignaciones()
        {
            Cargando.Mostrar();

            bool bTodos = this.chkAsiMostrarTodos.Checked;
            DateTime dDesde = this.dtpAsiDesde.Value.Date;
            DateTime dHasta = this.dtpAsiHasta.Value.Date.AddDays(1);

            var oPendientes = General.GetListOf<BancosCuentasMovimientosView>(c => (c.RelacionTabla == null || c.RelacionTabla != Cat.Tablas.VentaPagoDetalle || c.Resguardado)
                && (!c.BancoCuentaID.HasValue
                || (bTodos && c.Fecha >= dDesde && c.Fecha < dHasta)));
            // Se quitan todos los datos con fecha menor al 31 de Mayo, petición especial
            oPendientes = oPendientes.Where(c => c.Fecha >= new DateTime(2015, 5, 31)).ToList();

            this.dgvAsignacion.Rows.Clear();
            foreach (var oReg in oPendientes)
            {
                int iFila = this.dgvAsignacion.Rows.Add(oReg.BancoCuentaMovimientoID, false, oReg.Fecha, oReg.Sucursal, oReg.Referencia, oReg.Concepto
                    , string.Format("{0}-{1}", (oReg.FormaDePago == null ? "" : oReg.FormaDePago.Substring(0, 2)), oReg.DatosDePago), oReg.Importe);
                if (oReg.BancoCuentaID.HasValue)
                    this.dgvAsignacion.Rows[iFila].DefaultCellStyle.ForeColor = Color.Green;
            }

            Cargando.Cerrar();
        }

        private void Asignar()
        {
            // Se obtiene la cuenta correspondiente
            var frmCuenta = new MensajeObtenerValor("Selecciona la cuenta a la cual se le van a asignar los movimientos seleccionados:", Cat.CuentasBancarias.Scotiabank, MensajeObtenerValor.Tipo.Combo);
            frmCuenta.CargarCombo("BancoCuentaID", "NombreDeCuenta", General.GetListOf<BancoCuenta>(c => c.UsoClientes));
            int iBancoCuentaID = 0;
            if (frmCuenta.ShowDialog(Principal.Instance) == DialogResult.OK)
                iBancoCuentaID = Helper.ConvertirEntero(frmCuenta.Valor);
            else
                return;

            Cargando.Mostrar();
            
            foreach (DataGridViewRow oFila in this.dgvAsignacion.Rows)
            {
                if (Helper.ConvertirBool(oFila.Cells["asi_Sel"].Value))
                {
                    int iMovID = Helper.ConvertirEntero(oFila.Cells["asi_BancoCuentaMovimientoID"].Value);
                    ContaProc.AsignarMovimientoBancario(iMovID, iBancoCuentaID);
                }
            }

            Cargando.Cerrar();

            this.LlenarAsignaciones();
        }

        private void LlenarConciliaciones()
        {
            int iBancoCuentaID = Helper.ConvertirEntero(this.cmbBancoCuenta.SelectedValue);
            if (iBancoCuentaID <= 0)
                return;

            Cargando.Mostrar();

            DateTime dDesde = this.dtpConDesde.Value.Date;
            DateTime dHasta = this.dtpConHasta.Value.Date.AddDays(1);
            var oMovs = General.GetListOf<BancosCuentasMovimientosView>(c => c.BancoCuentaID == iBancoCuentaID && c.FechaAsignado >= dDesde && c.FechaAsignado < dHasta
                && (c.RelacionTabla == null || c.RelacionTabla != Cat.Tablas.VentaPagoDetalle || c.Resguardado) && !c.MovimientoAgrupadorID.HasValue)
                .OrderBy(c => c.FechaAsignado);
            // Se quitan todos los datos con fecha menor al 01 de Junio, petición especial
            oMovs = oMovs.Where(c => c.Fecha >= new DateTime(2015, 5, 29)).ToList().OrderBy(c => c.FechaAsignado);

            decimal mSaldoInicial = (oMovs.Count() > 0 ? oMovs.First().SaldoAcumulado : 0);
            decimal mSaldoOperacion = (oMovs.Count() > 0 ? oMovs.Last().SaldoAcumulado : 0);
            decimal mSaldoConciliado = 0;

            this.dgvConciliacion.Rows.Clear();
            foreach (var oReg in oMovs)
            {
                int iFila = this.dgvConciliacion.Rows.Add(oReg.BancoCuentaMovimientoID, false, oReg.FechaAsignado, oReg.Sucursal, oReg.Concepto, oReg.Referencia
                    , (oReg.EsIngreso ? (decimal?)oReg.Importe : null), (oReg.EsIngreso ? null : (decimal?)oReg.Importe), oReg.SaldoAcumulado
                    , oReg.FechaConciliado, oReg.UsuarioConciliado, oReg.Observacion, oReg.FueManual);
                if (oReg.Conciliado.Valor())
                {
                    mSaldoConciliado += (oReg.Importe * (oReg.EsIngreso ? 1 : -1));
                    this.dgvConciliacion["con_Sel", iFila].ReadOnly = true;
                    if (!oReg.FueManual.Valor())
                        this.dgvConciliacion.Rows[iFila].DefaultCellStyle.ForeColor = Color.Green;
                }
                // Si es un movimiento agrupador, no se puede seleccionar, pues eso se hace desde los agrupados (grid de abajo)
                if (oReg.EsAgrupador.Valor())
                    this.dgvConciliacion["con_Sel", iFila].ReadOnly = true;
            }

            this.lblSaldoInicial.Text = mSaldoInicial.ToString(GlobalClass.FormatoMoneda);
            this.lblSaldoConciliado.Text = mSaldoConciliado.ToString(GlobalClass.FormatoMoneda);
            this.lblSaldoOperacion.Text = mSaldoOperacion.ToString(GlobalClass.FormatoMoneda);

            Cargando.Cerrar();
        }

        private void CalcularImporteSeleccion()
        {
            decimal mDepositos = 0, mRetiros = 0;
            foreach (DataGridViewRow oFila in this.dgvConciliacion.Rows)
            {
                if (Helper.ConvertirBool(oFila.Cells["con_Sel"].Value))
                {
                    mDepositos += Helper.ConvertirDecimal(oFila.Cells["con_Depositos"].Value);
                    mRetiros += Helper.ConvertirDecimal(oFila.Cells["con_Retiros"].Value);
                }
            }
            this.lblImporteSeleccion.Text = (mDepositos + mRetiros).ToString(GlobalClass.FormatoMoneda);
        }

        private void BorrarMovimiento(DataGridViewRow oFila)
        {
            bool bEliminar = false;
            if (Helper.ConvertirBool(oFila.Cells["con_FueManual"].Value))
            {
                if (UtilLocal.MensajePreguntaCancelar("¿Estás seguro que deseas eliminar el movimiento seleccionado?") == DialogResult.Yes)
                {
                    bEliminar = true;
                }
            }
            else
            {
                if (oFila.DefaultCellStyle.ForeColor != Color.Green)
                {
                    // Se valida el permiso
                    var oResU = UtilDatos.ValidarObtenerUsuario("Bancos.Movimientos.Borrar");
                    if (oResU.Error) return;
                    
                    bEliminar = true;
                }
            }

            if (bEliminar)
            {
                int iMovID = Helper.ConvertirEntero(oFila.Cells["con_BancoCuentaMovimientoID"].Value);
                ContaProc.EliminarMovimientoBancario(iMovID);
                this.LlenarConciliaciones();
            }
        }

        private void LlenarMovimientosGrupo(int iMovID)
        {
            var oMovs = General.GetListOf<BancosCuentasMovimientosView>(c => c.MovimientoAgrupadorID == iMovID);
            this.dgvConciliacionDetalle.Rows.Clear();
            foreach (var oReg in oMovs)
            {
                int iFila = this.dgvConciliacionDetalle.Rows.Add(oReg.BancoCuentaMovimientoID, oReg.Conciliado.Valor(), oReg.FechaAsignado, oReg.Sucursal
                    , oReg.Concepto, oReg.Referencia, (oReg.EsIngreso ? oReg.Importe : 0), (oReg.EsIngreso ? 0 : oReg.Importe));
                if (oReg.Conciliado.Valor())
                {
                    this.dgvConciliacionDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Green;
                    this.dgvConciliacionDetalle["cnd_Sel", iFila].ReadOnly = true;
                }
            }
        }

        private void AgruparMovimientos()
        {
            // Se obtienen los movimientos marcados
            var oMovsIds = new List<int>();
            decimal mDeposito = 0, mRetiro = 0;
            foreach (DataGridViewRow oFila in this.dgvConciliacion.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["con_Sel"].Value))
                    continue;
                mDeposito += Helper.ConvertirDecimal(oFila.Cells["con_Depositos"].Value);
                mRetiro += Helper.ConvertirDecimal(oFila.Cells["con_Retiros"].Value);
                oMovsIds.Add(Helper.ConvertirEntero(oFila.Cells["con_BancoCuentaMovimientoID"].Value));
            }

            // Se valida que haya movimientos o importe
            if ((mDeposito + mRetiro) == 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ningún movimiento seleccionado o el importe es igual a cero.");
                return;
            }
            // Se valida que sean puros depósitos o puros retiros
            if (mDeposito > 0 && mRetiro > 0)
            {
                UtilLocal.MensajeAdvertencia("No es posible agrupar movimientos de tipo depósito con movimientos de tipo retiro.");
                return;
            }

            // Se abre forma para guardar los datos
            var frmDatos = new AgruparMovimientosBancarios();
            frmDatos.Deposito = (mDeposito > 0);
            frmDatos.Importe = (mDeposito + mRetiro);
            // Se llenan los datos con el primer movimiento seleccionado
            int iPrimerMovID = oMovsIds[0];
            var oPrimerMov = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iPrimerMovID);
            frmDatos.Fecha = oPrimerMov.FechaAsignado.Valor();
            frmDatos.SucursalID = oPrimerMov.SucursalID;
            frmDatos.Concepto = oPrimerMov.Concepto;
            frmDatos.Referencia = oPrimerMov.Referencia;
            // Se muestra el formulario
            if (frmDatos.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                Cargando.Mostrar();
                // Se obtienen los movimientos a agrupar
                var oMovs = new List<BancoCuentaMovimiento>();
                foreach (int iModID in oMovsIds)
                    oMovs.Add(General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iModID));
                // Se genera y guarda el movimiento agrupador
                var oMovAgrupador = new BancoCuentaMovimiento()
                {
                    BancoCuentaID = oPrimerMov.BancoCuentaID,
                    EsIngreso = (mDeposito > 0),
                    Fecha = DateTime.Now,
                    FechaAsignado = frmDatos.Fecha,
                    SucursalID = frmDatos.SucursalID,
                    Importe = oMovs.Sum(c => c.Importe),
                    Concepto = frmDatos.Concepto,
                    Referencia = frmDatos.Referencia,
                    SaldoAcumulado = oMovs.OrderBy(c=> c.FechaAsignado).Last().SaldoAcumulado
                };
                Guardar.Generico<BancoCuentaMovimiento>(oMovAgrupador);
                // Se agrupan los movimientos
                foreach (var oMov in oMovs)
                {
                    oMov.MovimientoAgrupadorID = oMovAgrupador.BancoCuentaMovimientoID;
                    Guardar.Generico<BancoCuentaMovimiento>(oMov);
                }
                // Se recalcula el acumulado
                this.RecalcularAcumulado(oMovAgrupador.FechaAsignado.Valor());
                //
                Cargando.Cerrar();
                this.LlenarConciliaciones();
            }
            frmDatos.Dispose();
        }

        private void DesagruparMovimientos(int iMovAgrupadorID)
        {
            var oMovsGrupo = General.GetListOf<BancoCuentaMovimiento>(c => c.MovimientoAgrupadorID == iMovAgrupadorID);
            if (oMovsGrupo.Count > 0)
            {
                if (UtilLocal.MensajePreguntaCancelar("¿Estás seguro que deseas desagrupar el movimiento seleccionado?") != DialogResult.Yes)
                    return;
            }
            else
            {
                return;
            }

            Cargando.Mostrar();

            // Se desagrupan los movimientos
            DateTime dMenor = DateTime.MaxValue;
            foreach (var oReg in oMovsGrupo)
            {
                oReg.MovimientoAgrupadorID = null;
                Guardar.Generico<BancoCuentaMovimiento>(oReg);
                if (dMenor > oReg.FechaAsignado)
                    dMenor = oReg.FechaAsignado.Valor();
            }

            // Se borra el movimiento agrupador
            var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iMovAgrupadorID);
            Guardar.Eliminar<BancoCuentaMovimiento>(oMov);
            
            // Se reajustan los saldos
            this.RecalcularAcumulado(dMenor);

            Cargando.Cerrar();

            // Se actualizan los datos
            this.LlenarConciliaciones();
        }

        private void GuardarConciliacion()
        {
            Cargando.Mostrar();

            DateTime dFechaMenor = DateTime.MaxValue;
            foreach (DataGridViewRow oFila in this.dgvConciliacion.Rows)
            {
                int iCambio = Helper.ConvertirEntero(oFila.Tag);
                if (iCambio > 0)  // Hubo una modificación
                {
                    int iMovID = Helper.ConvertirEntero(oFila.Cells["con_BancoCuentaMovimientoID"].Value);
                    var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iMovID);
                    oMov.Observacion = Helper.ConvertirCadena(oFila.Cells["con_Observacion"].Value);
                    
                    // Se verifica si se modificó la fecha de asignación, para saber desde dónde re-hacer el cálculo del saldo acumulado
                    if (iCambio == 2)
                    {
                        if (oMov.FechaAsignado < dFechaMenor)
                            dFechaMenor = oMov.FechaAsignado.Valor();
                        oMov.FechaAsignado = Helper.ConvertirFechaHora(oFila.Cells["con_FechaAsignado"].Value);
                    }

                    // Se guarda el movimiento
                    Guardar.Generico<BancoCuentaMovimiento>(oMov);

                    // Si se marcó el checkbox, se marca como conciliado
                    if (Helper.ConvertirBool(oFila.Cells["con_Sel"].Value))
                        this.ConciliarMovimiento(oMov.BancoCuentaMovimientoID);
                }
            }

            // Se verifica si hubo un cambio de orden (fecha asignado), para recalcular los saldos acumulados
            if (dFechaMenor < DateTime.MaxValue)
            {
                this.RecalcularAcumulado(dFechaMenor);
            }

            // Se mandan a conciliar los agrupados, del grid de abajo, si hubiera | y si es que no se han conciliado ya anteriormente
            foreach (DataGridViewRow oFila in this.dgvConciliacionDetalle.Rows)
            {
                if (!oFila.Cells["cnd_Sel"].ReadOnly && Helper.ConvertirBool(oFila.Cells["cnd_Sel"].Value))
                {
                    int iMovID = Helper.ConvertirEntero(oFila.Cells["cnd_BancoCuentaMovimientoID"].Value);
                    this.ConciliarMovimiento(iMovID);
                }
            }

            // Se actualizan los datos
            this.LlenarConciliaciones();

            Cargando.Cerrar();
        }

        private void ConciliarMovimiento(int iMovID)
        {
            var oMov = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == iMovID);
            oMov.Conciliado = true;
            oMov.FechaConciliado = DateTime.Now;
            oMov.ConciliadoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
            Guardar.Generico<BancoCuentaMovimiento>(oMov);

            // Si es una venta (ingreso), se realiza una póliza para pasar el dinero de Caja a la Cuenta correspondiente
            if (oMov.RelacionTabla == Cat.Tablas.VentaPagoDetalle)
            {
                var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Bancos
                    && c.RelacionID == oMov.BancoCuentaID);
                if (oCuentaAux == null)
                {
                    UtilLocal.MensajeAdvertencia(string.Format("La cuenta bancaria seleccionada no tiene una cuenta auxiliar en contabilidad. No se agregará la póliza."));
                }
                // Se hace la afectación contable (AfeConta)
                DateTime dFechaPoliza = oMov.FechaAsignado.Valor();
                if (oMov.MovimientoAgrupadorID.HasValue)
                {
                    var oMovAgr = General.GetEntity<BancoCuentaMovimiento>(c => c.BancoCuentaMovimientoID == oMov.MovimientoAgrupadorID);
                    dFechaPoliza = oMovAgr.FechaAsignado.Valor();
                }
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DepositoBancario, oMov.BancoCuentaMovimientoID, oMov.Referencia, oMov.Concepto, dFechaPoliza);
            }
        }

        private void RecalcularAcumulado(DateTime dDesde)
        {
            int iCuentaID = Helper.ConvertirEntero(this.cmbBancoCuenta.SelectedValue);
            ContaProc.RecalcularSaldoAcumulado(iCuentaID, dDesde);
        }

        private void HacerTraspaso()
        {
            var frmTraspaso = new MovimientoBancarioGen() { OrigenBancoCuentaID = this.ConBancoCuentaID, Text = "Traspaso entre cuentas" };
            frmTraspaso.LlenarComboCuenta();
            frmTraspaso.lblImporteInfo.Text = this.lblSaldoOperacion.Text;
            frmTraspaso.txtConcepto.Text = "Traspaso entre cuentas";
            // Para concatenar la cuenta destino
            frmTraspaso.cmbBancoCuenta.SelectedIndexChanged += new EventHandler((s, e) =>
            {
                frmTraspaso.txtConcepto.Text = ("Traspaso entre cuentas - " + frmTraspaso.cmbBancoCuenta.Text);
            });
            // Para validar los datos
            frmTraspaso.delValidar += () => {
                frmTraspaso.ctlError.LimpiarErrores();
                if (frmTraspaso.BancoCuentaID <= 0)
                    frmTraspaso.ctlError.PonerError(frmTraspaso.cmbBancoCuenta, "Debes especificar una cuenta.");
                if (frmTraspaso.Importe == 0)
                    frmTraspaso.ctlError.PonerError(frmTraspaso.txtImporte, "El importe especificado es inválido.");
                return frmTraspaso.ctlError.Valido;
            };
            if (frmTraspaso.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                Cargando.Mostrar();

                // Se crea el retiro de la cuenta origen
                var oMovOrigen = new BancoCuentaMovimiento
                {
                    BancoCuentaID = this.ConBancoCuentaID,
                    EsIngreso = false,
                    Fecha = frmTraspaso.dtpFecha.Value,
                    FechaAsignado = frmTraspaso.dtpFecha.Value,
                    SucursalID = GlobalClass.SucursalID,
                    Importe = frmTraspaso.Importe,
                    Concepto = frmTraspaso.txtConcepto.Text,
                    Referencia = GlobalClass.UsuarioGlobal.NombreUsuario
                };
                ContaProc.RegistrarMovimientoBancario(oMovOrigen);
                // Se crea el depósito a la cuenta destino
                var oMovDestino = new BancoCuentaMovimiento
                {
                    BancoCuentaID = frmTraspaso.BancoCuentaID,
                    EsIngreso = true,
                    Fecha = frmTraspaso.dtpFecha.Value,
                    FechaAsignado = frmTraspaso.dtpFecha.Value,
                    SucursalID = GlobalClass.SucursalID,
                    Importe = frmTraspaso.Importe,
                    Concepto = frmTraspaso.txtConcepto.Text,
                    Referencia = GlobalClass.UsuarioGlobal.NombreUsuario
                };
                ContaProc.RegistrarMovimientoBancario(oMovDestino);

                // Se crea la póliza sencilla correspondiente (AfeConta)
                var oCuentaOrigen = General.GetEntity<BancoCuenta>(c => c.BancoCuentaID == oMovOrigen.BancoCuentaID);
                var oCuentaDestino = General.GetEntity<BancoCuenta>(c => c.BancoCuentaID == oMovDestino.BancoCuentaID);
                var oCuentaAuxOrigen = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == 
                    (oCuentaOrigen.EsCpcp ? Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo : Cat.ContaCuentasDeMayor.Bancos) && c.RelacionID == oMovOrigen.BancoCuentaID);
                var oCuentaAuxDestino = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == 
                    (oCuentaDestino.EsCpcp ? Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo : Cat.ContaCuentasDeMayor.Bancos) && c.RelacionID == oMovDestino.BancoCuentaID);
                if (oCuentaAuxOrigen == null || oCuentaAuxDestino == null)
                {
                    Cargando.Cerrar();
                    UtilLocal.MensajeAdvertencia("No se encontró las cuenta auxiliar de alguna de las cuentas bancarias. No se realizará la Póliza.");
                }
                else
                {
                    var oPoliza = ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, oMovOrigen.Concepto, oCuentaAuxDestino.ContaCuentaAuxiliarID,
                        oCuentaAuxOrigen.ContaCuentaAuxiliarID, oMovOrigen.Importe, oMovOrigen.Referencia, Cat.Tablas.BancoCuentaMovimiento, oMovOrigen.BancoCuentaMovimientoID);
                    oPoliza.Fecha = oMovOrigen.Fecha;
                    Guardar.Generico<ContaPoliza>(oPoliza);
                }

                Cargando.Cerrar();
                this.LlenarConciliaciones();
            }
            frmTraspaso.Dispose();
        }

        private void AgregarMovimiento()
        {
            var frmMov = new MovimientoBancarioGen() { Text = "Agregar movimiento" };
            frmMov.LlenarComboCuenta();
            frmMov.cmbBancoCuenta.SelectedValue = this.ConBancoCuentaID;
            frmMov.lblEtImporteInfo.Visible = false;
            frmMov.lblImporteInfo.Visible = false;
            frmMov.ActiveControl = frmMov.txtImporte;
            // Para validar los datos
            frmMov.delValidar += () =>
            {
                frmMov.ctlError.LimpiarErrores();
                if (frmMov.BancoCuentaID <= 0)
                    frmMov.ctlError.PonerError(frmMov.cmbBancoCuenta, "Debes especificar una cuenta.");
                if (frmMov.Importe == 0)
                    frmMov.ctlError.PonerError(frmMov.txtImporte, "El importe especificado es inválido.");
                if (frmMov.txtConcepto.Text == "")
                    frmMov.ctlError.PonerError(frmMov.txtConcepto, "Debes especificar un concepto.", ErrorIconAlignment.BottomLeft);
                return frmMov.ctlError.Valido;
            };
            if (frmMov.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                Cargando.Mostrar();

                // Se crea el movimiento bancario
                var oMov = new BancoCuentaMovimiento
                {
                    BancoCuentaID = frmMov.BancoCuentaID,
                    EsIngreso = (frmMov.Importe >= 0),
                    Fecha = frmMov.dtpFecha.Value,
                    FechaAsignado = frmMov.dtpFecha.Value,
                    SucursalID = GlobalClass.SucursalID,
                    Importe = (frmMov.Importe > 0 ? frmMov.Importe : (frmMov.Importe * -1)),
                    Concepto = frmMov.txtConcepto.Text,
                    Referencia = GlobalClass.UsuarioGlobal.NombreUsuario,
                    FueManual = true
                };
                ContaProc.RegistrarMovimientoBancario(oMov);

                Cargando.Cerrar();
                this.LlenarConciliaciones();
            }
            frmMov.Dispose();
        }

        #endregion

    }
}