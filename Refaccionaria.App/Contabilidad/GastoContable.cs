﻿using System;
using System.Windows.Forms;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class GastoContable : UserControl
    {
        ControlError ctlError = new ControlError();
        ContaEgreso oEgreso;
        bool EsMod = false;

        public GastoContable()
        {
            InitializeComponent();
        }

        public GastoContable(int iEgresoID)
        {
            this.InitializeComponent();
            
            this.oEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iEgresoID);
            this.EsMod = true;
        }

        #region [ Eventos ]

        private void GastoContable_Load(object sender, EventArgs e)
        {
            // Se cargan los Combos
            this.cmbCuentaFinal.CargarDatos("ContaCuentaAuxiliarID", "CuentaAuxiliar", General.GetListOf<ContaCuentaAuxiliar>());
            var oCuentasSubs = General.GetListOf<ContaCuentasAuxiliaresView>().Select(c =>
                new { c.ContaSubcuentaID, CuentaSubcuenta = (c.Cuenta + " - " + c.Subcuenta) }).Distinct().ToList();
            this.cmbCuentaSubcuenta.CargarDatos("ContaSubcuentaID", "CuentaSubcuenta", oCuentasSubs);
            this.cmbCuentaDeMayor.ValueMember = "ContaCuentaDeMayorID";
            this.cmbCuentaDeMayor.DisplayMember = "CuentaDeMayor";
            this.cmbCuentaAuxiliar.ValueMember = "ContaCuentaAuxiliarID";
            this.cmbCuentaAuxiliar.DisplayMember = "CuentaAuxiliar";
            // Se cargan los tipos de documentos
            var oTiposDoc = General.GetListOf<TipoDocumento>(c => c.TipoDocumentoID == Cat.TiposDeDocumento.Factura || c.TipoDocumentoID == Cat.TiposDeDocumento.Nota);
            // oTiposDoc.Add(new TipoDocumento() { Documento = "NO APLICA" });
            this.cmbDocumento.CargarDatos("TipoDocumentoID", "Documento", oTiposDoc);
            //
            this.cmbFormaDePago.CargarDatos("TipoFormaPagoID", "NombreTipoFormaPago", General.GetListOf<TipoFormaPago>(c =>
                c.TipoFormaPagoID == Cat.FormasDePago.Efectivo || c.TipoFormaPagoID == Cat.FormasDePago.Cheque ||
                c.TipoFormaPagoID == Cat.FormasDePago.Tarjeta || c.TipoFormaPagoID == Cat.FormasDePago.Transferencia));
            this.cmbCuentaBancaria.CargarDatos("BancoCuentaID", "NombreDeCuenta", General.GetListOf<BancoCuenta>());
            // Se cargan los consumibles, para el grid
            this.ContaConsumibleID.ValueMember = "ContaConsumibleID";
            this.ContaConsumibleID.DisplayMember = "Consumible";
            this.ContaConsumibleID.DataSource = General.GetListOf<ContaConsumible>();

            this.ActiveControl = this.cmbCuentaFinal;

            // Si es modificación, se llenan los datos
            if (this.EsMod)
            {
                this.dtpFecha.Value = this.oEgreso.Fecha;
                this.cmbCuentaFinal.SelectedValue = this.oEgreso.ContaCuentaAuxiliarID;
                this.cmbCuentaFinal_SelectedIndexChanged(null, null);
                if (this.oEgreso.TipoDocumentoID.HasValue)
                    this.cmbDocumento.SelectedValue = this.oEgreso.TipoDocumentoID;
                this.cmbFormaDePago.SelectedValue = this.oEgreso.TipoFormaPagoID;
                this.cmbFormaDePago_SelectedIndexChanged(null, null);
                this.txtFolioDePago.Text = this.oEgreso.FolioDePago;
                this.txtFolioFactura.Text = this.oEgreso.FolioFactura;
                this.txtImporte.Text = this.oEgreso.Importe.ToString();
                this.chkEsFiscal.Checked = this.oEgreso.EsFiscal;
                this.txtObservaciones.Text = this.oEgreso.Observaciones;
                this.txtFolioFactura.Text = this.oEgreso.FolioFactura;
                if (this.oEgreso.BancoCuentaID.HasValue)
                    this.cmbCuentaBancaria.SelectedValue = this.oEgreso.BancoCuentaID;
                // Se llenan los datos del detalle, si hay
                var oEgresoDet = General.GetListOf<ContaEgresoDetalle>(c => c.ContaEgresoID == this.oEgreso.ContaEgresoID);
                foreach (var oDet in oEgresoDet)
                {
                    int iFila = this.dgvDetalle.Rows.Add(Cat.TiposDeAfectacion.SinCambios, oDet.ContaConsumibleID, oDet.Cantidad, oDet.Importe, 
                        (oDet.Cantidad * oDet.Importe));
                    this.dgvDetalle.Rows[iFila].Tag = oDet;
                }
            }
        }

        private void cmbCuentaFinal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbCuentaFinal.Focused || sender == null)
            {
                var oCuentaAux = (this.cmbCuentaFinal.SelectedItem as ContaCuentaAuxiliar);
                var oCuentaM = General.GetEntity<ContaCuentaDeMayor>(c => c.ContaCuentaDeMayorID == oCuentaAux.ContaCuentaDeMayorID);
                this.cmbCuentaSubcuenta.SelectedValue = oCuentaM.ContaSubcuentaID;
                this.cmbCuentaDeMayor.SelectedValue = oCuentaM.ContaCuentaDeMayorID;
                this.cmbCuentaAuxiliar.SelectedValue = oCuentaAux.ContaCuentaAuxiliarID;

                this.VerDetallable();
            }
        }

        private void cmbCuentaSubcuenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSubcuentaID = Helper.ConvertirEntero(this.cmbCuentaSubcuenta.SelectedValue);
            this.cmbCuentaDeMayor.DataSource = General.GetListOf<ContaCuentaDeMayor>(c => c.ContaSubcuentaID == iSubcuentaID);
        }

        private void cmbCuentaDeMayor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iCuentaMaID = Helper.ConvertirEntero(this.cmbCuentaDeMayor.SelectedValue);
            this.cmbCuentaAuxiliar.DataSource = General.GetListOf<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaMaID);
        }

        private void cmbCuentaAuxiliar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbCuentaFinal.Focused) return;

            int iCuentaFinal = Helper.ConvertirEntero(this.cmbCuentaFinal.SelectedValue);
            int iCuentaAux = Helper.ConvertirEntero(this.cmbCuentaAuxiliar.SelectedValue);
            if (iCuentaFinal != iCuentaAux)
            {
                this.cmbCuentaFinal.SelectedValue = iCuentaAux;
                this.VerDetallable();
            }
        }

        private void cmbDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbDocumento.Focused)
                this.VerEsFiscal();
        }

        private void cmbFormaDePago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbFormaDePago.Focused || sender == null)
            {
                this.txtFolioDePago.Enabled = (Helper.ConvertirEntero(this.cmbFormaDePago.SelectedValue) != Cat.FormasDePago.Efectivo);
                this.cmbCuentaBancaria.Enabled = (Helper.ConvertirEntero(this.cmbFormaDePago.SelectedValue) != Cat.FormasDePago.Efectivo);
                this.VerEsFiscal();
            }
        }

        private void dgvDetalle_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (this.dgvDetalle["_Cambio", e.RowIndex].Value == null)
                this.dgvDetalle["_Cambio", e.RowIndex].Value = Cat.TiposDeAfectacion.Agregar;
        }

        private void dgvDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;

            if (e.KeyCode == Keys.Delete)
            {
                if (UtilLocal.MensajePregunta("¿Estás seguro que deseas borrar el Concepto seleccionado?") == DialogResult.Yes)
                {
                    if (Helper.ConvertirEntero(this.dgvDetalle.CurrentRow.Cells["_Cambio"].Value) == Cat.TiposDeAfectacion.Agregar)
                    {
                        this.dgvDetalle.Rows.Remove(this.dgvDetalle.CurrentRow);
                    }
                    else
                    {
                        this.dgvDetalle.CurrentRow.Cells["_Cambio"].Value = Cat.TiposDeAfectacion.Borrar;
                        this.dgvDetalle.CurrentRow.Visible = false;
                    }
                }
            }
        }

        private void dgvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDetalle.Columns[e.ColumnIndex].Name == "Cantidad" || this.dgvDetalle.Columns[e.ColumnIndex].Name == "Precio")
            {
                var Fila = this.dgvDetalle.Rows[e.RowIndex];
                Fila.Cells["Importe"].Value = (Helper.ConvertirDecimal(Fila.Cells["Cantidad"].Value) * Helper.ConvertirDecimal(Fila.Cells["Precio"].Value));

                if (Helper.ConvertirEntero(Fila.Cells["_Cambio"].Value) == Cat.TiposDeAfectacion.SinCambios)
                    Fila.Cells["_Cambio"].Value = Cat.TiposDeAfectacion.Modificar;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.ParentForm.DialogResult = DialogResult.OK;
                this.ParentForm.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }

        #endregion

        #region [ Métodos ]

        private void VerEsFiscal()
        {
            int iFormaDePagoID = Helper.ConvertirEntero(this.cmbFormaDePago.SelectedValue);
            if (iFormaDePagoID == Cat.FormasDePago.Efectivo)
            {
                int iDocumentoID = Helper.ConvertirEntero(this.cmbDocumento.SelectedValue);
                this.chkEsFiscal.Checked = (iDocumentoID == Cat.TiposDeDocumento.Factura || iDocumentoID == Cat.TiposDeDocumento.OtroOficial);
                this.chkEsFiscal.Enabled = (iDocumentoID == Cat.TiposDeDocumento.Factura || iDocumentoID == Cat.TiposDeDocumento.OtroOficial);
            }
            else
            {
                this.chkEsFiscal.Checked = true;
                this.chkEsFiscal.Enabled = false;
            }
        }

        private void VerDetallable()
        {
            var oCuentaAux = (this.cmbCuentaAuxiliar.SelectedItem as ContaCuentaAuxiliar);
            this.dgvDetalle.Rows.Clear();
            this.dgvDetalle.AllowUserToAddRows = (oCuentaAux.Detallable);
            //this.dgvDetalle.AllowUserToDeleteRows = (oCuentaAux.Detallable);
        }

        private void RestaurarControles()
        {
            this.dtpFecha.Value = DateTime.Now;
            this.cmbCuentaFinal.SelectedIndex = -1;
            this.cmbCuentaSubcuenta.SelectedIndex = -1;
            this.cmbCuentaDeMayor.SelectedIndex = -1;
            this.cmbCuentaAuxiliar.SelectedIndex = -1;
            this.cmbDocumento.SelectedIndex = -1;
            this.cmbFormaDePago.SelectedIndex = -1;
            this.txtFolioDePago.Clear();
            this.txtFolioFactura.Clear();
            this.txtImporte.Clear();
            this.chkEsFiscal.Checked = false;
            this.txtObservaciones.Clear();
            this.dgvDetalle.Rows.Clear();
            this.cmbCuentaFinal.Focus();
        }

        #endregion

        #region [ Públicos ]

        public void SeleccionarCuentaFinal(int iIdCuenta)
        {
            this.cmbCuentaFinal.SelectedValue = iIdCuenta;
            this.cmbCuentaFinal_SelectedIndexChanged(null, null);
        }

        public bool AccionGuardar()
        {
            // Se valida
            if (!this.Validar()) return false;

            Cargando.Mostrar();

            // Se guardan los datos
            // DateTime dAhora = DateTime.Now;
            int? iDocID = Helper.ConvertirEntero(this.cmbDocumento.SelectedValue);
            // Se guarda el gasto
            ContaEgreso oGasto = (this.EsMod ? this.oEgreso : new ContaEgreso());
            oGasto.ContaCuentaAuxiliarID = Helper.ConvertirEntero(this.cmbCuentaAuxiliar.SelectedValue);
            oGasto.Fecha = this.dtpFecha.Value;
            oGasto.Importe = Helper.ConvertirDecimal(this.txtImporte.Text);
            oGasto.TipoFormaPagoID = Helper.ConvertirEntero(this.cmbFormaDePago.SelectedValue);
            oGasto.FolioDePago = this.txtFolioDePago.Text;
            oGasto.FolioFactura = this.txtFolioFactura.Text;
            oGasto.BancoCuentaID = (int?)this.cmbCuentaBancaria.SelectedValue;
            oGasto.TipoDocumentoID = (iDocID > 0 ? iDocID : null);
            oGasto.EsFiscal = this.chkEsFiscal.Checked;
            oGasto.Observaciones = this.txtObservaciones.Text;
            oGasto.RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
            oGasto.SucursalID = GlobalClass.SucursalID;
            Guardar.Generico<ContaEgreso>(oGasto);
            // Se guarda el detalle, si aplica
            foreach (DataGridViewRow Fila in this.dgvDetalle.Rows)
            {
                if (Fila.IsNewRow) continue;
                
                int iCambioID = Helper.ConvertirEntero(Fila.Cells["_Cambio"].Value);
                if (iCambioID == Cat.TiposDeAfectacion.SinCambios) continue;

                ContaEgresoDetalle oGastoDet;
                if (iCambioID == Cat.TiposDeAfectacion.Agregar)
                {
                    oGastoDet = new ContaEgresoDetalle();
                    oGastoDet.ContaEgresoID = oGasto.ContaEgresoID;
                }
                else
                {
                    oGastoDet = (Fila.Tag as ContaEgresoDetalle);
                    if (iCambioID == Cat.TiposDeAfectacion.Borrar)
                    {
                        Guardar.Eliminar<ContaEgresoDetalle>(oGastoDet, true);
                        continue;
                    }
                }
                oGastoDet.ContaConsumibleID = Helper.ConvertirEntero(Fila.Cells["ContaConsumibleID"].Value);
                oGastoDet.Cantidad = Helper.ConvertirDecimal(Fila.Cells["Cantidad"].Value);
                oGastoDet.Importe = Helper.ConvertirDecimal(Fila.Cells["Precio"].Value);
                Guardar.Generico<ContaEgresoDetalle>(oGastoDet);
            }
            // Se manda devengar automáticamente, si aplica
            ContaProc.GastoVerDevengarAutomaticamente(oGasto);
            
            // Se crea la póliza contable correspondiente (AfeConta), sólo cuando es nuevo
            if (!this.EsMod)
            {
                ContaPoliza oPoliza = null;
                var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oGasto.ContaCuentaAuxiliarID);

                if (oGasto.TipoDocumentoID == Cat.TiposDeDocumento.Factura)
                {
                    if (oGasto.TipoFormaPagoID == Cat.FormasDePago.Efectivo)
                    {
                        oPoliza = ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GastoFacturadoEfectivo, oGasto.ContaEgresoID, oGasto.FolioFactura, oGasto.Observaciones);
                    }
                    else
                    {
                        if (oGasto.BancoCuentaID == Cat.CuentasBancarias.Banamex || oGasto.BancoCuentaID == Cat.CuentasBancarias.Scotiabank)
                            oPoliza = ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GastoFacturadoBanco, oGasto.ContaEgresoID, oGasto.FolioFactura, oGasto.Observaciones);
                        else
                            oPoliza = ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GastoContableFacturadoBancoCpcp, oGasto.ContaEgresoID
                                , oGasto.FolioFactura, oGasto.Observaciones);
                    }
                }
                else
                {
                    oPoliza = ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GastoNotaEfectivo, oGasto.ContaEgresoID, oGasto.FolioFactura
                        , (oCuentaAux.CuentaAuxiliar + " / " + oGasto.Observaciones));
                }

                // Se modifica la fecha de la póliza creada, para que conincida con la fecha especificada para el gasto
                oPoliza.Fecha = oGasto.Fecha;
                Guardar.Generico<ContaPoliza>(oPoliza);

                // Se crea el movimiento bancario correspondiente, si aplica
                // Como se afecta la cuenta de bancos, se crea el movimiento bancario para mandarlo a conciliación y así llevar el control de todos los
                // movimientos bancarios
                if (oGasto.TipoFormaPagoID == Cat.FormasDePago.Cheque || oGasto.TipoFormaPagoID == Cat.FormasDePago.Tarjeta
                    || oGasto.TipoFormaPagoID == Cat.FormasDePago.Transferencia)
                {
                    var oMovBanc = new BancoCuentaMovimiento()
                    {
                        BancoCuentaID = oGasto.BancoCuentaID,
                        EsIngreso = false,
                        Fecha = oGasto.Fecha,
                        FechaAsignado = oGasto.Fecha,
                        SucursalID = oGasto.SucursalID,
                        Importe = oGasto.Importe,
                        Concepto = oGasto.Observaciones,
                        Referencia = oGasto.FolioFactura,
                        TipoFormaPagoID = oGasto.TipoFormaPagoID,
                        RelacionTabla = Cat.Tablas.ContaEgreso,
                        RelacionID = oGasto.ContaEgresoID
                    };
                    ContaProc.RegistrarMovimientoBancario(oMovBanc);
                }
            }

            Cargando.Cerrar();

            // Se muestra una notificación
            this.RestaurarControles();
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");

            return true;
        }

        public bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Helper.ConvertirEntero(this.cmbCuentaAuxiliar.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbCuentaAuxiliar, "Cuenta auxiliar inválida.", ErrorIconAlignment.MiddleLeft);
            int iFormaDePagoID = Helper.ConvertirEntero(this.cmbFormaDePago.SelectedValue);
            if (iFormaDePagoID <= 0)
                this.ctlError.PonerError(this.cmbFormaDePago, "Forma de pago inválida.", ErrorIconAlignment.MiddleLeft);
            if (iFormaDePagoID > 0 && iFormaDePagoID != Cat.FormasDePago.Efectivo && this.txtFolioDePago.Text == "")
                this.ctlError.PonerError(this.txtFolioDePago, "Debes especificar el Folio de Pago.", ErrorIconAlignment.BottomRight);
            if (Helper.ConvertirDecimal(this.txtImporte.Text) <= 0)
                this.ctlError.PonerError(this.txtImporte, "Debes especificar un Importe válido y mayor que cero.");
            if (this.txtFolioFactura.Text == "")
                this.ctlError.PonerError(this.txtFolioFactura, "Debes especificar el Folio / Factura.", ErrorIconAlignment.MiddleLeft);
            if (Helper.ConvertirEntero(this.cmbFormaDePago.SelectedValue) != Cat.FormasDePago.Efectivo && this.cmbCuentaBancaria.SelectedValue == null)
                this.ctlError.PonerError(this.cmbCuentaBancaria, "Debes especificar la cuenta bancaria.", ErrorIconAlignment.MiddleLeft);
            if (this.txtObservaciones.Text == "")
                this.ctlError.PonerError(this.txtObservaciones, "Debes especificar una Observación.", ErrorIconAlignment.BottomLeft);

            decimal mTotal = 0;
            bool bErrorGrid = false;
            bool bErrorBorrarDet = false;
            foreach (DataGridViewRow Fila in this.dgvDetalle.Rows)
            {
                if (Fila.IsNewRow) continue;
                                
                Fila.ErrorText = "";
                if (Helper.ConvertirEntero(Fila.Cells["ContaConsumibleID"].Value) <= 0)
                    Fila.ErrorText = "Consumible inválido.";
                if (Helper.ConvertirDecimal(Fila.Cells["Cantidad"].Value) <= 0)
                    Fila.ErrorText = "Cantidad inválida.";
                if (Helper.ConvertirDecimal(Fila.Cells["Precio"].Value) <= 0)
                    Fila.ErrorText = "Precio inválido.";

                int iCambioID = Helper.ConvertirEntero(Fila.Cells["_Cambio"].Value);
                if (iCambioID == Cat.TiposDeAfectacion.Modificar || iCambioID == Cat.TiposDeAfectacion.Borrar)
                {
                    var oGastoDet = (Fila.Tag as ContaEgresoDetalle);
                    var oGastoDetDev = General.GetListOf<ContaEgresoDetalleDevengado>(c => c.ContaEgresoDetalleID == oGastoDet.ContaEgresoDetalleID);
                    if (iCambioID == Cat.TiposDeAfectacion.Modificar)
                    {
                        if (Helper.ConvertirDecimal(Fila.Cells["Cantidad"].Value) < oGastoDetDev.Sum(c => c.Cantidad))
                            Fila.ErrorText = "La Cantidad especificada es menor a lo que ya se ha devengado para este Concepto.";
                    }
                    else
                    {
                        if (oGastoDetDev.Count > 0)
                        {
                            Fila.ErrorText = "El Concepto que se quiere borrar ya ha sido devengado. Para borrarlo, debes borrar primero los movimientos en donde se devengó.";
                            bErrorBorrarDet = true;
                        }
                        mTotal -= Helper.ConvertirDecimal(Fila.Cells["Importe"].Value);
                    }
                }

                bErrorGrid = (bErrorGrid || Fila.ErrorText != "");
                mTotal += Helper.ConvertirDecimal(Fila.Cells["Importe"].Value);
            }

            if (bErrorBorrarDet)
                this.ctlError.PonerError(this.btnGuardar,
                    "Hay uno o más Conceptos a borrar que ya han sido devengados. Para borrarlos, se deben borrar primero los movimientos en donde se devengaron.",
                    ErrorIconAlignment.MiddleLeft);

            // Se valida que el total del grid coincida con el importe del textbox, si hay detalle
            if (this.dgvDetalle.Rows.Count > 1)
            {
                // Se valida sólo la parte entera
                if (Math.Round(mTotal, 0) != Math.Round(Helper.ConvertirDecimal(this.txtImporte.Text), 0))
                    this.ctlError.PonerError(this.txtImporte, "El Importe no coincide con el total del detalle.");
            }

            return (this.ctlError.NumeroDeErrores == 0 && !bErrorGrid);
        }

        #endregion
                       
    }
}
