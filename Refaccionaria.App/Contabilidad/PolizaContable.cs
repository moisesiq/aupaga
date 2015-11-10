using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class PolizaContable : Form
    {
        ControlError ctlError = new ControlError();
        int iPolizaID;

        public PolizaContable()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public int ModPolizaID { get; set; }

        #endregion

        #region [ Eventos ]

        private void PolizaContable_Load(object sender, EventArgs e)
        {
            this.dgvTotales.Rows.Add();
            this.cmbTipoPoliza.CargarDatos("ContaTipoPolizaID", "TipoDePoliza", General.GetListOf<ContaTipoPoliza>());
            this.SucursalID.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.dgvDetalle.Inicializar();

            // Si es modificación, se mandan cargar los datos
            if (this.ModPolizaID > 0)
                this.CargarPoliza(this.ModPolizaID);
        }

        private void dgvDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4:
                    this.AfectarCargoAbonoFaltante();
                    this.dgvDetalle.VerAgregarFilaNueva();
                    break;
                case Keys.F3:
                    var oDatos = Helper.ListaEntityADataTable(General.GetListOf<ContaCuentaAuxiliar>());
                    var frmSel = new SeleccionListado(oDatos);
                    frmSel.dgvListado.MostrarColumnas("CuentaAuxiliar", "CuentaContpaq", "CuentaSat");
                    if (frmSel.ShowDialog(Principal.Instance) == DialogResult.OK)
                    {
                        this.dgvDetalle.CurrentRow.Cells["ContaCuentaAuxiliarID"].Value = frmSel.Seleccion["ContaCuentaAuxiliarID"];
                        this.dgvDetalle.CurrentRow.Cells["CuentaContpaq"].Value = frmSel.Seleccion["CuentaContpaq"];
                        this.dgvDetalle.CurrentRow.Cells["CuentaAuxiliar"].Value = frmSel.Seleccion["CuentaAuxiliar"];
                        this.dgvDetalle.VerAgregarFilaNueva();
                    }
                    frmSel.Dispose();
                    break;
            }
        }
                
        private void dgvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            bool bMayorQueCero = (Helper.ConvertirDecimal(this.dgvDetalle[e.ColumnIndex, e.RowIndex].Value) > 0);
            if (!bMayorQueCero) return;

            if (this.dgvDetalle.Columns[e.ColumnIndex].Name == "Cargo")
                this.dgvDetalle["Abono", e.RowIndex].Value = 0M;
            else if (this.dgvDetalle.Columns[e.ColumnIndex].Name == "Abono")
                this.dgvDetalle["Cargo", e.RowIndex].Value = 0M;

            this.CalcularTotales();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void AfectarCargoAbonoFaltante()
        {
            decimal mCargo = 0, mAbono = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                mCargo += Helper.ConvertirDecimal(oFila.Cells["Cargo"].Value);
                mAbono += Helper.ConvertirDecimal(oFila.Cells["Abono"].Value);
            }

            decimal mDiferencia = (mCargo - mAbono);
            if (mDiferencia > 0)
                this.dgvDetalle.CurrentRow.Cells["Abono"].Value = mDiferencia;
            else if (mDiferencia < 0)
                this.dgvDetalle.CurrentRow.Cells["Cargo"].Value = (mDiferencia * -1);

            this.CalcularTotales();
        }

        private void CalcularTotales()
        {
            decimal mCargo = 0, mAbono = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                mCargo += Helper.ConvertirDecimal(oFila.Cells["Cargo"].Value);
                mAbono += Helper.ConvertirDecimal(oFila.Cells["Abono"].Value);
            }
            this.dgvTotales["tot_Cargo", 0].Value = mCargo;
            this.dgvTotales["tot_Abono", 0].Value = mAbono;
        }

        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            // Se obtiene la póliza
            ContaPoliza oPoliza;
            if (this.iPolizaID > 0)
                oPoliza = General.GetEntity<ContaPoliza>(c => c.ContaPolizaID == iPolizaID);
            else
                oPoliza = new ContaPoliza() { SucursalID = GlobalClass.SucursalID, RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID, FueManual = true };

            // Se guarda la póliza
            oPoliza.Fecha = this.dtpFecha.Value;
            oPoliza.ContaTipoPolizaID = Helper.ConvertirEntero(this.cmbTipoPoliza.SelectedValue);
            oPoliza.Concepto = this.txtConcepto.Text;
            oPoliza.Origen = this.txtOrigen.Text;
            oPoliza.Error = false;
            Guardar.Generico<ContaPoliza>(oPoliza);

            // Se procede a guardar el detalle
            ContaPolizaDetalle oReg = null;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iCuentaAuxID = Helper.ConvertirEntero(oFila.Cells["ContaCuentaAuxiliarID"].Value);
                int iId = this.dgvDetalle.ObtenerId(oFila);
                int iCambio = this.dgvDetalle.ObtenerIdCambio(oFila);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new ContaPolizaDetalle() { ContaPolizaID = oPoliza.ContaPolizaID };
                        else
                            oReg = General.GetEntity<ContaPolizaDetalle>(c => c.ContaPolizaDetalleID == iId);

                        oReg.ContaCuentaAuxiliarID = iCuentaAuxID;
                        oReg.Cargo = Helper.ConvertirDecimal(oFila.Cells["Cargo"].Value);
                        oReg.Abono = Helper.ConvertirDecimal(oFila.Cells["Abono"].Value);
                        oReg.Referencia = Helper.ConvertirCadena(oFila.Cells["Referencia"].Value);
                        oReg.SucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);

                        Guardar.Generico<ContaPolizaDetalle>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<ContaPolizaDetalle>(c => c.ContaPolizaDetalleID == iId);
                        Guardar.Eliminar<ContaPolizaDetalle>(oReg);
                        break;
                }

                // Se verifica si se afecta una cuenta bancaria, en cuyo caso, se crea un movimiento bancario
                if (iCambio == Cat.TiposDeAfectacion.Agregar)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == iCuentaAuxID);
                    if ((oCuentaAux.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Bancos || oCuentaAux.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo)
                        && oCuentaAux.RelacionID.HasValue)
                    {
                        var oMovBanc = new BancoCuentaMovimiento()
                        {
                            BancoCuentaID = oCuentaAux.RelacionID.Valor(),
                            EsIngreso = (oReg.Cargo > 0),
                            Fecha = oPoliza.Fecha,
                            FechaAsignado = oPoliza.Fecha,
                            SucursalID = oPoliza.SucursalID,
                            Importe = (oReg.Cargo > 0 ? oReg.Cargo : oReg.Abono),
                            Concepto = oPoliza.Concepto,
                            Referencia = oReg.Referencia,
                            TipoFormaPagoID = Cat.FormasDePago.Efectivo,
                            RelacionID = oReg.ContaPolizaDetalleID
                        };
                        ContaProc.RegistrarMovimientoBancario(oMovBanc);
                    }
                }
            }

            Cargando.Cerrar();
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();

            if (Helper.ConvertirEntero(this.cmbTipoPoliza.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbTipoPoliza, "Tipo de Póliza inválido.");
            if (this.txtConcepto.Text == "")
                this.ctlError.PonerError(this.txtConcepto, "Debes especificar un Concepto.");

            decimal mCargo = 0, mAbono = 0;
            bool bErrorGrid = false;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                if (oFila.IsNewRow) continue;

                //
                oFila.ErrorText = "";
                if (Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value) == 0)
                {
                    oFila.ErrorText = "Sucursal inválida.";
                    bErrorGrid = true;
                }

                //
                mCargo += Helper.ConvertirDecimal(oFila.Cells["Cargo"].Value);
                mAbono += Helper.ConvertirDecimal(oFila.Cells["Abono"].Value);
            }
            if (bErrorGrid)
                this.ctlError.PonerError(this.btnGuardar, "Existen errores de validación. Verificar.", ErrorIconAlignment.MiddleLeft);
            if (mCargo != mAbono)
                this.ctlError.PonerError(this.btnGuardar, "La suma de los Cargos es diferente a la suma de los Abonos.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion

        #region [ Públicos ]

        public void CargarPoliza(int iPolizaID)
        {
            this.iPolizaID = iPolizaID;

            var oPoliza = General.GetEntity<ContaPoliza>(c => c.ContaPolizaID == iPolizaID);
            this.dtpFecha.Value = oPoliza.Fecha;
            this.cmbTipoPoliza.SelectedValue = oPoliza.ContaTipoPolizaID;
            this.txtConcepto.Text = oPoliza.Concepto;
            this.txtOrigen.Text = oPoliza.Origen;

            var oPolizaDetV = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.ContaPolizaID == iPolizaID);
            this.dgvDetalle.Rows.Clear();
            foreach (var oReg in oPolizaDetV)
                this.dgvDetalle.AgregarFila(oReg.ContaPolizaDetalleID, Cat.TiposDeAfectacion.SinCambios, oReg.ContaCuentaAuxiliarID, oReg.CuentaContpaq, oReg.CuentaAuxiliar
                    , oReg.Cargo, oReg.Abono, oReg.Referencia, oReg.SucursalID);

            this.CalcularTotales();
        }

        public void HacerModificacionMinima()
        {
            // this.dtpFecha.Enabled = false; // Se habilita la edición de la fecha
            this.cmbTipoPoliza.Enabled = false;
            this.txtConcepto.ReadOnly = true;
            this.txtOrigen.ReadOnly = true;
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.PermitirBorrar = false;
            this.dgvDetalle.KeyDown -= this.dgvDetalle_KeyDown;
            this.ActiveControl = this.dgvDetalle;
        }

        #endregion

    }
}
