using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.Objects;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class BusquedaDevolucion : BusquedaVenta
    {
        public VentasDevolucion oDevolucion;

        public BusquedaDevolucion()
        {
            InitializeComponent();
            this.dgvVentas.Width += 183;
            this.pnlDatosDePago.Left += 183;
        }

        #region [ Propiedades ]

        public bool CancelarTodaLaFactura { get { return this.chkCancelarFactura.Checked; } }

        #endregion

        #region [ Eventos ]

        private void BusquedaDevoluciones_Load(object sender, System.EventArgs e)
        {
            // Se nombran los motivos y se asigna un nombre que hace referencia al Id en la tabla
            var Motivos = General.GetListOf<VentaDevolucionMotivo>();
            string sControl;
            int iCont = 0;
            foreach (var Motivo in Motivos)
            {
                sControl = "rdbMotivo" + (++iCont).ToString();
                this.gpbMotivo.Controls[sControl].Tag = Motivo.VentaDevolucionMotivoID;
                this.gpbMotivo.Controls[sControl].Text = (Motivo.Descripcion.Izquierda(1) + Motivo.Descripcion.Substring(1).ToLower());
            }

            this.tabCancelaciones.SendToBack();
        }

        private void txtReimpresion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.VerTicketDevolucion();
            }
        }

        private void tabCancelaciones_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabCancelaciones.SelectedTab.Name)
            {
                case "tbpCancelaciones":
                    this.tabCancelaciones.SendToBack();
                    break;
                case "tbpFacturasPorCancelar":
                    this.tabCancelaciones.BringToFront();
                    this.ActualizarFacturasPorCancelar();
                    break;
            }
        }

        private void btnCancelarFacPen_Click(object sender, EventArgs e)
        {
            if (this.dgvFacturasPorCancelar.CurrentRow == null)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna factura por cancelar seleccionada.");
                return;
            }

            this.btnCancelarFacPen.Enabled = false;
            Cargando.Mostrar();

            int iVentaFacturaDevolucionID = Helper.ConvertirEntero(this.dgvFacturasPorCancelar.CurrentRow.Cells["VentaFacturaDevolucionID"].Value);
            string sFolioFiscal = Helper.ConvertirCadena(this.dgvFacturasPorCancelar.CurrentRow.Cells["FolioFiscal"].Value);
            var Res = VentasProc.GenerarFacturaCancelacion(sFolioFiscal, iVentaFacturaDevolucionID);
            if (Res.Exito)
            {
                this.ActualizarFacturasPorCancelar();
                Cargando.Cerrar();
            }
            else
            {
                Cargando.Cerrar();
                UtilLocal.MensajeAdvertencia("Hubo un error al cancelar la factura:\n\n" + Res.Mensaje);
            }

            this.btnCancelarFacPen.Enabled = true;
        }

        #endregion

        #region [ Overrides ]

        protected override void AplicarFiltro()
        {
            // Se muestra la ventana de "Cargando.."
            if (this.txtFolio.Text == "")
                Cargando.Mostrar();

            //
            string sFolio = this.txtFolio.Text;
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            bool bFacturadas = this.rdbFactura.Checked;
            DateTime dInicio = this.dtpInicio.Value.Date;
            DateTime dFin = this.dtpFin.Value.Date.AddDays(1);
            bool bFiltroMinimo = (this.chkMostrarTodasLasVentas.Checked);

            List<VentasView> Lista;

            if (sFolio == "")
            {
                Lista = General.GetListOf<VentasView>(q =>
                    (q.VentaEstatusID != Cat.VentasEstatus.Cancelada && q.VentaEstatusID != Cat.VentasEstatus.CanceladaSinPago)
                    && (bFiltroMinimo || iClienteID == 0 || q.ClienteID == iClienteID)
                    && (bFiltroMinimo || iSucursalID == 0 || q.SucursalID == iSucursalID)
                    && q.Fecha >= dInicio
                    && q.Fecha < dFin
                    && (bFiltroMinimo || q.Facturada == bFacturadas)
                    && q.VentaEstatusID != Cat.VentasEstatus.Realizada
                );
            }
            else
            {
                Lista = General.GetListOf<VentasView>(q => q.Folio == sFolio && (iSucursalID == 0 || q.SucursalID == iSucursalID));
            }

            // Se manda llenar el Grid
            this.LlenarGrid(Lista);

            // Se cierra la ventana de "Cargando.."
            if (this.txtFolio.Text == "")
                Cargando.Cerrar();
        }

        protected override void MostrarDatosDeVenta(int iVentaID)
        {
            // Se muestran los datos de la venta seleccionada
            base.MostrarDatosDeVenta(iVentaID);

            // Se configuran las acciones a tomar, según la venta seleccionada
            this.rdbDevolucionCheque.Checked = false;
            this.rdbDevolucionTarjeta.Checked = false;
            this.rdbDevolucionTransferencia.Checked = false;
            DateTime dHoy = DateTime.Today;
            var oVentaPagos = General.GetListOf<VentasPagosDetalleView>(q => q.VentaID == iVentaID && EntityFunctions.TruncateTime(q.Fecha) == dHoy);
            int iPagosCheque = 0, iPagosTarjeta = 0, iPagosTransferencia = 0, iPagosOtro = 0;
            foreach (var oFormaP in oVentaPagos)
            {
                if (oFormaP.FormaDePagoID == Cat.FormasDePago.Cheque)
                    iPagosCheque++;
                else if (oFormaP.FormaDePagoID == Cat.FormasDePago.Tarjeta)
                    iPagosTarjeta++;
                else if (oFormaP.FormaDePagoID == Cat.FormasDePago.Transferencia)
                    iPagosTransferencia++;
                else
                    iPagosOtro++;
            }
            this.rdbDevolucionCheque.Enabled = (iPagosCheque == 1 && iPagosTarjeta == 0 && iPagosTransferencia == 0 && iPagosOtro == 0);
            this.rdbDevolucionTarjeta.Enabled = (iPagosTarjeta == 1 && iPagosCheque == 0 && iPagosTransferencia == 0 && iPagosOtro == 0);
            this.rdbDevolucionTransferencia.Enabled = (iPagosTransferencia == 1 && iPagosCheque == 0 && iPagosTarjeta == 0 && iPagosOtro == 0);

            // Para mostrar el detalle de la venta
            this.oDevolucion.ctlDetalle.LlenarDetalle(iVentaID);

            // Se verifica si es factura múltiple, para mostrar el check de cancelar todas
            this.chkCancelarFactura.Visible = VentasProc.EsFacturaMultiple(iVentaID);

        }

        #endregion

        #region [ Privados ]

        private int ObtenerMotivoSeleccionado()
        {
            int iMotivoID = 0;
            foreach (Control oControl in this.gpbMotivo.Controls)
            {
                if (oControl is RadioButton)
                {
                    if ((oControl as RadioButton).Checked)
                    {
                        iMotivoID = Helper.ConvertirEntero(oControl.Tag);
                    }
                }
            }
            return iMotivoID;
        }

        private void VerTicketDevolucion()
        {
            string sFolio = this.txtReimpresion.Text;
            var oVenta = General.GetEntity<Venta>(c => c.Folio == sFolio && c.Estatus);
            if (oVenta == null)
            {
                UtilLocal.MensajeAdvertencia("La venta especificada no existe.");
                return;
            }

            // Se obtiene la devolución a reimprimir
            var oDevs = General.GetListOf<VentasDevolucionesView>(c => c.VentaID == oVenta.VentaID);
            int iDevID = 0;
            if (oDevs.Count == 0)
            {
                UtilLocal.MensajeAdvertencia("La venta especificada no tiene devoluciones/cancelaciones.");
                return;
            }
            else if (oDevs.Count > 1)
            {
                var frmListado = new SeleccionListado(oDevs);
                frmListado.Text = "Selecciona una Devolución/Cancelación";
                frmListado.MostrarColumnas("Fecha", "VentaDevolucionID", "Total");
                frmListado.dgvListado.Columns["VentaDevolucionID"].HeaderText = "Folio Dev.";
                frmListado.dgvListado.Columns["Total"].FormatoMoneda();
                if (frmListado.ShowDialog(Principal.Instance) == DialogResult.OK)
                    iDevID = Helper.ConvertirEntero(frmListado.Seleccion["VentaDevolucionID"]);
                frmListado.Dispose();
            }
            else
            {
                iDevID = oDevs[0].VentaDevolucionID;
            }

            // Se manda reimprimir
            if (iDevID > 0)
            {
                VentasProc.GenerarTicketDevolucion(iDevID);
                this.txtReimpresion.Clear();
            }
        }

        #endregion

        #region [ Públicos ]

        public int FormaDeDevolucion
        {
            get
            {
                if (this.rdbNotaDeCredito.Checked)
                    return Cat.FormasDePago.Vale;
                else if (this.rdbDevolucionEfectivo.Checked)
                    return Cat.FormasDePago.Efectivo;
                else if (this.rdbDevolucionCheque.Checked)
                    return Cat.FormasDePago.Cheque;
                else if (this.rdbDevolucionTarjeta.Checked)
                    return Cat.FormasDePago.Tarjeta;
                else if (this.rdbDevolucionTransferencia.Checked)
                    return Cat.FormasDePago.Transferencia;
                else
                    return 0;
            }
        }

        public bool Validar()
        {
            this.ctlError.LimpiarErrores();

            // Se valida que se especifique un motivo
            if (this.ObtenerMotivoSeleccionado() == 0)
                this.ctlError.PonerError(this.txtMotivoNota, "Debes especificar un motivo.", ErrorIconAlignment.BottomLeft);
            // Se valida que se especifique una observación
            if (this.txtMotivoNota.Text == "")
                this.ctlError.PonerError(this.txtMotivoNota, "Debes especificar una observación.", ErrorIconAlignment.BottomLeft);
            // Se valida la acción a tomar
            if (this.FormaDeDevolucion == 0)
                this.ctlError.PonerError(this.rdbNotaDeCredito, "Debes especificar una acción a tomar.");

            // Se valida que el tipo de acción a tomar sea válido
            if ((this.rdbDevolucionCheque.Checked || this.rdbDevolucionTarjeta.Checked || this.rdbDevolucionTransferencia.Checked)
                && !this.oDevolucion.ctlDetalle.TodosMarcados())
                this.ctlError.PonerError(this.rdbNotaDeCredito, "No se puede especificar Cheque, Tarjeta o Transferencia si no es una cancelación.");

            return (this.ctlError.NumeroDeErrores == 0);
        }

        public VentaDevolucion GenerarDevolucion()
        {
            // Se obtiene el Id de motivo, según el radio marcado
            int iMotivoID = this.ObtenerMotivoSeleccionado();

            return new VentaDevolucion()
            {
                VentaID = this.VentaID,
                Fecha = DateTime.Now,
                MotivoID = iMotivoID,
                Observacion = this.txtMotivoNota.Text,
                TipoFormaPagoID = this.FormaDeDevolucion
            };
        }

        public void ActualizarFacturasPorCancelar()
        {
            this.dgvFacturasPorCancelar.DataSource = null;
            this.dgvFacturasPorCancelar.DataSource = General.GetListOf<VentasFacturasCancelacionesView>();
            this.dgvFacturasPorCancelar.OcultarColumnas("VentaFacturaDevolucionID", "VentaFacturaID");
            this.dgvFacturasPorCancelar.AutoResizeColumns();
        }

        #endregion

    }
}
