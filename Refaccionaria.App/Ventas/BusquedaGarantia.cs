using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.Objects;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class BusquedaGarantia : BusquedaVenta
    {
        public VentasGarantia oGarantia;

        public BusquedaGarantia()
        {
            InitializeComponent();
            this.dgvVentas.Width += (183 + 40);
            this.pnlDatosDePago.Left += (183 + 40);
        }

        #region [ Propiedades ]

        public bool GarantiaNueva { get { return (this.tabGarantia.SelectedTab.Name == "tbpGarantia"); } }

        public int SeleccionGarantiaID
        {
            get
            {
                if (this.dgvPendientes.CurrentRow == null)
                    return 0;
                else
                    return Helper.ConvertirEntero(this.dgvPendientes.CurrentRow.Cells["penVentaGarantiaID"].Value);
            }
        }

        public int IdAccionPosterior { get { return this.ObtenerIdDeRadioSeleccionado(this.gpbAccionPosterior); } }

        public string AccionObservacion { get { return this.txtAccionObservacion.Text; } }

        #endregion

        #region [ Eventos ]

        private void BusquedaGarantia_Load(object sender, EventArgs e)
        {
            // Se nombran los motivos y se asigna un nombre que hace referencia al Id en la tabla
            var oMotivos = General.GetListOf<VentaGarantiaMotivo>();
            int iCont = 0;
            foreach (var oMotivo in oMotivos)
            {
                string sControl = ("rdbMotivo" + (++iCont).ToString());
                this.gpbMotivo.Controls[sControl].Tag = oMotivo.VentaGarantiaMotivoID;
                this.gpbMotivo.Controls[sControl].Text = (oMotivo.Motivo.Izquierda(1) + oMotivo.Motivo.Substring(1).ToLower());
            }
            
            // Se configuran las opciones de Acciones
            this.rdbAcArticuloNuevo.Tag = Cat.VentasGarantiasAcciones.ArticuloNuevo;
            this.rdbAcNotaDeCredito.Tag = Cat.VentasGarantiasAcciones.NotaDeCredito;
            this.rdbAcEfectivo.Tag = Cat.VentasGarantiasAcciones.Efectivo;
            this.rdbAcCheque.Tag = Cat.VentasGarantiasAcciones.Cheque;
            this.rdbAcTarjeta.Tag = Cat.VentasGarantiasAcciones.Tarjeta;
            this.rdbAcTransferencia.Tag = Cat.VentasGarantiasAcciones.Transferencia;
            this.rdbAcRevision.Tag = Cat.VentasGarantiasAcciones.RevisionDeProveedor;
            // Pendientes
            this.dtpPenDesde.Value = DateTime.Now.DiaPrimero();
            this.dtpPenHasta.Value = DateTime.Now.DiaUltimo();
            // Se configuran las opciones de Acciones posteriores
            this.rdbApArticuloNuevo.Tag = Cat.VentasGarantiasAcciones.ArticuloNuevo;
            this.rdbApNotaDeCredito.Tag = Cat.VentasGarantiasAcciones.NotaDeCredito;
            this.rdbApEfectivo.Tag = Cat.VentasGarantiasAcciones.Efectivo;
            this.rdbApCheque.Tag = Cat.VentasGarantiasAcciones.Cheque;
            this.rdbApTarjeta.Tag = Cat.VentasGarantiasAcciones.Tarjeta;
            this.rdbApTransferencia.Tag = Cat.VentasGarantiasAcciones.Transferencia;
            this.rdbApNoProcede.Tag = Cat.VentasGarantiasAcciones.NoProcede;

            this.tabGarantia.SendToBack();
        }

        private void txtReimpresion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.VerTicketGarantia();
            }
        }

        private void tabGarantia_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabGarantia.SelectedTab.Name)
            {
                case "tbpGarantia":
                    this.tabGarantia.SendToBack();
                    break;
                case "tbpPendientes":
                    this.tabGarantia.BringToFront();
                    this.ActualizarGarantiasPendientes();
                    break;
            }
        }

        private void chkPenMostrarTodas_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpPenDesde.Enabled = this.chkPenMostrarTodas.Checked;
            this.dtpPenHasta.Enabled = this.chkPenMostrarTodas.Checked;
            if (this.chkPenMostrarTodas.Focused)
                this.ActualizarGarantiasPendientes();
        }
                
        private void dtpPenDesde_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.ActualizarGarantiasPendientes();
        }

        private void dtpPenHasta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.ActualizarGarantiasPendientes();
        }

        private void dgvPendientes_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvPendientes.CurrentRow == null) return;

            if (this.dgvPendientes.VerSeleccionNueva())
            {
                int iGarantiaID = Helper.ConvertirEntero(this.dgvPendientes.CurrentRow.Cells["penVentaGarantiaID"].Value);
                var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iGarantiaID && c.Estatus);
                bool bRevision = (oGarantia.AccionID == Cat.VentasGarantiasAcciones.RevisionDeProveedor);
                foreach (Control oControl in this.gpbAccionPosterior.Controls)
                {
                    if (oControl is RadioButton)
                    {
                        oControl.Enabled = bRevision;
                        if (Helper.ConvertirEntero(oControl.Tag) == oGarantia.AccionID)
                            (oControl as RadioButton).Checked = true;
                    }
                }
                if (bRevision)
                    this.HabilitarDeshabilitarAcciones(false, oGarantia.VentaID);
            }
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
            this.HabilitarDeshabilitarAcciones(true, iVentaID);

            // Para mostrar el detalle de la venta
            this.oGarantia.ctlDetalle.LlenarDetalle(iVentaID);
        }

        #endregion

        #region [ Privados ]

        private void HabilitarDeshabilitarAcciones(bool bAccionesNormales, int iVentaID)
        {
            RadioButton rdbCheque, rdbTarjeta, rdbTransferencia;
            if (bAccionesNormales)
            {
                rdbCheque = this.rdbAcCheque;
                rdbTarjeta = this.rdbAcTarjeta;
                rdbTransferencia = this.rdbAcTransferencia;
            }
            else
            {
                rdbCheque = this.rdbApCheque;
                rdbTarjeta = this.rdbApTarjeta;
                rdbTransferencia = this.rdbApTransferencia;
            }

            rdbCheque.Checked = false;
            rdbTarjeta.Checked = false;
            rdbTransferencia.Checked = false;

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
            rdbCheque.Enabled = (iPagosCheque == 1 && iPagosTarjeta == 0 && iPagosTransferencia == 0 && iPagosOtro == 0);
            rdbTarjeta.Enabled = (iPagosTarjeta == 1 && iPagosCheque == 0 && iPagosTransferencia == 0 && iPagosOtro == 0);
            rdbTransferencia.Enabled = (iPagosTransferencia == 1 && iPagosCheque == 0 && iPagosTarjeta == 0 && iPagosOtro == 0);
        }

        private int ObtenerIdDeRadioSeleccionado(Control oContenedor)
        {
            foreach (Control oControl in oContenedor.Controls)
            {
                if (oControl is RadioButton)
                {
                    if ((oControl as RadioButton).Checked)
                    {
                        return Helper.ConvertirEntero(oControl.Tag);
                    }
                }
            }
            return 0;
        }

        private void VerTicketGarantia()
        {
            string sFolio = this.txtReimpresion.Text;
            var oVenta = General.GetEntity<Venta>(c => c.Folio == sFolio && c.Estatus);
            if (oVenta == null)
            {
                UtilLocal.MensajeAdvertencia("La venta especificada no existe.");
                return;
            }

            // Se obtiene la garantía a reimprimir
            var oGarantias = General.GetListOf<VentasGarantiasView>(c => c.VentaID == oVenta.VentaID);
            int iGarantiaID = 0;
            if (oGarantias.Count == 0)
            {
                UtilLocal.MensajeAdvertencia("La venta especificada no tiene garantías.");
                return;
            }
            else if (oGarantias.Count > 1)
            {
                var frmListado = new SeleccionListado(oGarantias);
                frmListado.Text = "Selecciona una Garantía";
                frmListado.MostrarColumnas("Fecha", "VentaGarantiaID", "Total");
                frmListado.dgvListado.Columns["VentaGarantiaID"].HeaderText = "Folio Gar.";
                frmListado.dgvListado.Columns["Total"].FormatoMoneda();
                if (frmListado.ShowDialog(Principal.Instance) == DialogResult.OK)
                    iGarantiaID = Helper.ConvertirEntero(frmListado.Seleccion["VentaGarantiaID"]);
                frmListado.Dispose();
            }
            else
            {
                iGarantiaID = oGarantias[0].VentaGarantiaID;
            }

            // Se manda reimprimir
            if (iGarantiaID > 0)
            {
                VentasProc.GenerarTicketGarantia(iGarantiaID);
                this.txtReimpresion.Clear();
            }
        }

        #endregion

        #region [ Públicos ]

        public bool Validar()
        {
            this.ctlError.LimpiarErrores();

            // Se valida que se especifique un motivo
            if (this.ObtenerIdDeRadioSeleccionado(this.gpbMotivo) == 0)
                this.ctlError.PonerError(this.txtMotivoNota, "Debes especificar un motivo.", ErrorIconAlignment.BottomLeft);
            // Se valida que se especifique una observación
            if (this.txtMotivoNota.Text == "")
                this.ctlError.PonerError(this.txtMotivoNota, "Debes especificar una observación.", ErrorIconAlignment.BottomLeft);
            // Se valida la acción a tomar
            if (this.ObtenerIdDeRadioSeleccionado(this.gpbAccion) == 0)
                this.ctlError.PonerError(this.rdbAcArticuloNuevo, "Debes especificar una acción a tomar.");

            // Se valida que el tipo de acción a tomar sea válido
            if (this.rdbAcCheque.Checked || this.rdbAcTarjeta.Checked || this.rdbAcTransferencia.Checked)
            {
                var oSeleccion = this.oGarantia.ctlDetalle.ProductosSel();
                if (oSeleccion.Count == 1 && oSeleccion[0].Cantidad > 1)
                    this.ctlError.PonerError(this.rdbAcArticuloNuevo, "No se puede especificar Cheque, Tarjeta o Transferencia si la cantidad es mayor que uno.");
            }

            return (this.ctlError.NumeroDeErrores == 0);
        }

        public VentaGarantia GenerarGarantia()
        {
            // Se obtiene el Id de motivo, según el radio marcado
            int iMotivoID = this.ObtenerIdDeRadioSeleccionado(this.gpbMotivo);
            int iAccionID = this.ObtenerIdDeRadioSeleccionado(this.gpbAccion);

            return new VentaGarantia()
            {
                VentaID = this.VentaID,
                Fecha = DateTime.Now,
                SucursalID = GlobalClass.SucursalID,
                MotivoID = iMotivoID,
                MotivoObservacion = this.txtMotivoNota.Text,
                AccionID = iAccionID
            };
        }

        public void ActualizarGarantiasPendientes()
        {
            List<VentasGarantiasView> oDatos;
            if (this.chkPenMostrarTodas.Checked)
            {
                DateTime dHasta = this.dtpPenHasta.Value.Date.AddDays(1);
                oDatos = General.GetListOf<VentasGarantiasView>(c => c.Fecha >= this.dtpPenDesde.Value.Date && c.Fecha < dHasta);
            }
            else
            {
                oDatos = General.GetListOf<VentasGarantiasView>(c => c.AccionID == Cat.VentasGarantiasAcciones.RevisionDeProveedor && c.EstatusGenericoID != Cat.EstatusGenericos.Completada);
            }

            this.dgvPendientes.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvPendientes.Rows.Add(oReg.VentaGarantiaID, oReg.EstatusGenericoID, oReg.NumeroDeParte, oReg.NombreDeParte, oReg.Proveedor, oReg.FolioDeVenta
                    , oReg.Sucursal, oReg.Fecha, oReg.Motivo, oReg.MotivoObservacion, oReg.Estatus, oReg.Accion, oReg.ObservacionCompletado);
        }

        public bool ValidarPendiente()
        {
            this.ctlError.LimpiarErrores();
            
            //
            if (this.SeleccionGarantiaID == 0)
                this.ctlError.PonerError(this.tbpPendientes, "No hay ninguna garantía seleccionada.", ErrorIconAlignment.BottomRight);
            // Se valida que la garantía seleccionada tenga estatus "A revisión"
            if (Helper.ConvertirEntero(this.dgvPendientes.CurrentRow.Cells["penEstatusGenericoID"].Value) != Cat.EstatusGenericos.Resuelto)
                this.ctlError.PonerError(this.tbpPendientes, "La garantía seleccionada no tiene estatus \"Resuelto\".", ErrorIconAlignment.BottomRight);
            // Se valida la acción a tomar
            if (this.ObtenerIdDeRadioSeleccionado(this.gpbAccionPosterior) == 0)
                this.ctlError.PonerError(this.rdbApArticuloNuevo, "Debes especificar una acción a tomar.");
            // Se valida que se especifique una observación
            if (this.txtAccionObservacion.Text == "")
                this.ctlError.PonerError(this.txtAccionObservacion, "Debes especificar una observación.", ErrorIconAlignment.BottomLeft);

            // Se valida que el tipo de acción a tomar sea válido
            if (this.rdbApCheque.Checked || this.rdbApTarjeta.Checked)
            {
                var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == this.SeleccionGarantiaID && c.Estatus);
                if (oGarantia != null)
                {
                    // var oGarantiaDet = General.GetListOf<VentaGarantiaDetalle>(c => c.VentaGarantiaID == oGarantia.VentaGarantiaID && c.Estatus);
                    var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == oGarantia.VentaID && c.Estatus);
                    // if (oGarantiaDet.Count != oVentaDet.Count)
                       //  this.ctlError.PonerError(this.rdbApArticuloNuevo, "No se puede especificar Cheque o Tarjeta si no se mandan todos los productos a garantía.");
                }
            }

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
                                                                        
    }
}
