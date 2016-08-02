using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class Cobro : UserControl
    {
        ControlError ctlAdvertencia = new ControlError() { Icon = Properties.Resources._16_Ico_Advertencia };
        int ClienteID;
        Dictionary<int, decimal> NotasDeCredito = new Dictionary<int, decimal>();
        ContenedorControl frmNotasDeCredito;
        SeleccionarNotasDeCredito ctlNotasDeCredito;

        protected ControlError ctlError = new ControlError();

        public event EventHandler Cotizacion_Click;

        public Cobro()
        {
            InitializeComponent();

            // Se asignan los eventos a los textos de moneda
            this.txtEfectivo.Leave += UtilLocal.TextLeaveFormatoMoneda;
            this.txtCheque.Leave += UtilLocal.TextLeaveFormatoMoneda;
            this.txtTarjetaDeCredito.Leave += UtilLocal.TextLeaveFormatoMoneda;
            this.txtTransferencia.Leave += UtilLocal.TextLeaveFormatoMoneda;
            this.txtNoIdentificado.Leave += UtilLocal.TextLeaveFormatoMoneda;
        }

        void txtEfectivo_Leave(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #region [ Propiedadees ]

        //private bool _AfectarEfectivo = true;
        private decimal _Total;
        public decimal Total
        {
            get { return this._Total; }
            set
            {
                this._Total = value;
                this.lblTotal.Text = value.ToString(GlobalClass.FormatoDecimal);

                // Para actualizar automáticamente el importe en efectivo (ya que es lo más común)
                /* if (this._AfectarEfectivo)
                {
                    this.chkEfectivo.Checked = true;
                    this.txtEfectivo.Text = this._Total.ToString();
                }
                else
                { */
                    this.CalcularTotales();
                //}
            }
        }

        public decimal Suma { get { return Util.Decimal(this.lblSuma.Text.SoloNumeric()); } }

        public bool HabilitarRetroceso
        {
            get { return this.btnAtras.Enabled; }
            set { this.btnAtras.Enabled = value; }
        }

        private bool _HabilitarTipoDePago = true;
        public bool HabilitarTipoDePago
        {
            get { return this._HabilitarTipoDePago; }
            set
            {
                this._HabilitarTipoDePago = value;
                this.gpbTipoDePago.Enabled = value;
            }
        }

        public bool HabilitarFormasDePago
        {
            get { return this.gpbFormasDePago.Enabled; }
            set { this.gpbFormasDePago.Enabled = value; }
        }

        public bool MostrarFacturar
        {
            get { return this.chkFacturar.Visible; }
            set { this.chkFacturar.Visible = value; }
        }

        public bool HabilitarFacturar
        {
            get { return this.chkFacturar.Enabled; }
            set { this.chkFacturar.Enabled = value; }
        }

        public bool MostrarFacturarDividir
        {
            get { return this.chkFacturarDividir.Visible; }
            set { this.chkFacturarDividir.Visible = value; }
        }

        public bool HabilitarCotizacion
        {
            get { return this.btnCotizacion.Visible; }
            set { this.btnCotizacion.Visible = value; }
        }

        public bool AutorizacionDeCreditoRequerida { get { return this.ctlAdvertencia.TieneError(this.rdbCredito); } }

        public bool AutorizacionDeNotasDeCreditoRequerida { get { return this.ctlAdvertencia.TieneError(this.txtNotaDeCredito); } }

        public int VendodorID { get { return Util.Entero(this.cmbVendedor.SelectedValue); } }

        public int RepartidorID { get { return Util.Entero(this.cmbRepartidor.SelectedValue); } }

        public int ComisionistaID { get { return Util.Entero(this.cmbClienteComisionista.SelectedValue); } }

        public bool ACredito { get { return this.rdbCredito.Checked; } }

        public bool Facturar { get { return this.chkFacturar.Checked; } }

        public bool DividirFactura { get { return this.chkFacturarDividir.Checked; } }

        public int ClienteVehiculoID { get { return Util.Entero(this.cmbVehiculo.SelectedValue); } }

        public int? Kilometraje { get { return (this.txtKilometraje.Text == "" ? null : (int?)Util.Entero(this.txtKilometraje.Text)); } }

        public string Leyenda { get { return this.txtLeyenda.Text; } set { this.txtLeyenda.Text = value; } }

        public List<VentasPagosDetalleView> FormasDePagoLibre { get; set; }

        public int VentaID { get; set; }

        public decimal? EfectivoRecibido { get; set; }

        public int BancoCuentaID { get; set; }
        public int? MesesSinIntereses { get; set; }
        public string CelularTarjeta { get; set; }

        #endregion

        #region [ Eventos ]

        private void Cobro_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            // Se llenan los Combos
            this.cmbBanco.CargarDatos("BancoID", "NombreBanco", Datos.GetListOf<Banco>(q => q.Estatus).OrderBy(q => q.NombreBanco).ToList());
            this.cmbVendedor.CargarDatos("UsuarioID", "NombreUsuario", Datos.GetListOf<Usuario>(q => q.Activo && q.Estatus).OrderBy(q => q.NombreUsuario).ToList());
            this.cmbRepartidor.CargarDatos("UsuarioID", "NombreUsuario", Datos.GetListOf<Usuario>(c => c.TipoUsuarioID == Cat.TiposDeUsuario.Repartidor
                && c.Activo && c.Estatus));
            this.cmbClienteComisionista.CargarDatos("ClienteID", "Nombre", 
                Datos.GetListOf<Cliente>(q => q.EsClienteComisionista.Value && q.Estatus).OrderBy(q => q.Nombre).ToList());
            
            this.cmbVehiculo.CargarDatos("ClienteFlotillaID", "NombreModelo", Datos.GetListOf<ClientesFlotillasView>(q => q.ClienteID == this.ClienteID));
            
            // Se llena el combo de las leyendas
            this.cmbLeyanda.CargarDatos("VentaTicketLeyendaID", "NombreLeyenda", Datos.GetListOf<VentaTicketLeyenda>());

            // Se llenan los datos que deben tener valor
            this.cmbVendedor.SelectedValue = GlobalClass.UsuarioGlobal.UsuarioID;
        }

        private void rdbContado_CheckedChanged(object sender, EventArgs e)
        {
            // Si no está habilitada la selección de tipo de cobro, se sale
            if (!this.HabilitarTipoDePago) return;

            this.chkNoIdentificado.Enabled = this.rdbCredito.Checked;
            this.chkNoIdentificado.Checked = this.rdbCredito.Checked;

            // Si es Contado
            if (this.rdbContado.Checked)
            {
                // Se quita la advertencia de no crédito, si hubiera
                this.ctlAdvertencia.QuitarError(this.rdbCredito);
                this.ctlError.QuitarError(this.rdbCredito);
            }

            // Si es Crédito
            else
            {
                // Se verifica si el cliente tiene forma de pago predeterminada, para dejar las formas de pago y que así se facture
                var oCliente = Datos.GetEntity<Cliente>(q => q.ClienteID == this.ClienteID);
                if (oCliente.TipoFormaPagoID.HasValue && oCliente.TipoFormaPagoID.Value > 0)
                {
                    this.chkNoIdentificado.Checked = false;
                }
                else
                {
                    // Se desmarcan las formas de pago
                    this.chkEfectivo.Checked = false;
                    this.chkCheque.Checked = false;
                    this.chkTarjetaDeCredito.Checked = false;
                    this.chkTarjetaDeDebito.Checked = false;
                    this.chkTransferencia.Checked = false;
                    // this.chkNoIdentificado.Checked = false;
                    this.chkNotaDeCredito.Checked = false;
                }

                // Se verifica si el cliente tiene crédito permitido
                bool bTolerancia = oCliente.Tolerancia.Valor();
                string sNoCredito = "No se puede continuar.";
                string sAutorizacion = "Se requerirá autorización para continuar con la Venta.";
                string sError;

                if (!oCliente.TieneCredito)
                {
                    this.ctlAdvertencia.PonerError(this.rdbCredito, "El cliente especificado no tiene Crédito. " + sAutorizacion);
                    return;
                }
                if (this.Total > oCliente.LimiteCredito)
                {
                    sError = "El Importe de la Venta es mayor al Crédito que tiene el Cliente. ";
                    if (bTolerancia)
                        this.ctlAdvertencia.PonerError(this.rdbCredito, sError + sAutorizacion);
                    else
                        this.ctlError.PonerError(this.rdbCredito, sError + sNoCredito);
                    return;
                }
                var AdeudosCliente = Datos.GetListOf<ClientesAdeudosView>(q => q.ClienteID == oCliente.ClienteID);
                if (AdeudosCliente.Count > 0)
                {
                    DateTime dAdeudoIni = AdeudosCliente.Min(q => q.Fecha);
                    if (dAdeudoIni >= DateTime.Now.AddDays(oCliente.DiasDeCredito.Valor()))
                    {
                        sError = "El Cliente ha sobrepasado su fecha límite de pago. ";
                        if (bTolerancia)
                            this.ctlAdvertencia.PonerError(this.rdbCredito, sError + sAutorizacion);
                        else
                            this.ctlError.PonerError(this.rdbCredito, sError + sNoCredito);
                        return;
                    }
                    decimal mAdeudo = AdeudosCliente.Sum(q => q.Restante.Valor());
                    if ((mAdeudo + this.Total) > oCliente.LimiteCredito)
                    {
                        sError = "El Cliente ha sobrepasado su límite de crédito. ";
                        if (bTolerancia)
                            this.ctlAdvertencia.PonerError(this.rdbCredito, sError + sAutorizacion);
                        else
                            this.ctlError.PonerError(this.rdbCredito, sError + sNoCredito);
                        return;
                    }
                }
            }
        }

        private void rdbCredito_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void chkFacturar_CheckedChanged(object sender, EventArgs e)
        {
            this.chkFacturarDividir.Enabled = this.chkFacturar.Checked;
        }

        private void btnCotizacion_Click(object sender, EventArgs e)
        {
            if (this.Cotizacion_Click != null)
                this.Cotizacion_Click.Invoke(sender, e);
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            this.Cancelar();
        }

        private void chkEfectivo_CheckedChanged(object sender, EventArgs e)
        {
            this.HabilitarTextosFP();

            if (this.chkEfectivo.Checked && this.chkEfectivo.Focused)
            {
                this.txtEfectivo.Focus();
                // this._AfectarEfectivo = false;
            }
        }

        private void txtEfectivo_TextChanged(object sender, EventArgs e)
        {
            this.CalcularTotales();

            // if (this.txtEfectivo.Focused)
                // this._AfectarEfectivo = false;
        }

        private void chkCheque_CheckedChanged(object sender, EventArgs e)
        {
            this.HabilitarTextosFP();
            if (this.chkCheque.Checked && this.chkCheque.Focused)
                this.txtCheque.Focus();
        }

        private void chkTarjetaDeCredito_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkTarjetaDeCredito.Focused)
            {
                this.HabilitarTextosFP();
                if (this.chkTarjetaDeCredito.Checked)
                {
                    this.chkTarjetaDeDebito.Checked = false;
                    this.txtTarjetaDeCredito.Focus();
                }
            }
        }

        private void chkTarjetaDeDebito_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkTarjetaDeDebito.Focused)
            {
                this.HabilitarTextosFP();
                if (this.chkTarjetaDeDebito.Checked)
                {
                    this.chkTarjetaDeCredito.Checked = false;
                    this.txtTarjetaDeCredito.Focus();
                }
            }
        }

        private void chkTransferencia_CheckedChanged(object sender, EventArgs e)
        {
            this.HabilitarTextosFP();
            if (this.chkTransferencia.Checked && this.chkTransferencia.Focused)
                this.txtTransferencia.Focus();
        }

        private void chkNoIdentificado_CheckedChanged(object sender, EventArgs e)
        {
            this.HabilitarTextosFP();
            if (this.chkNoIdentificado.Checked && this.chkNoIdentificado.Focused)
                this.txtNoIdentificado.Focus();
        }

        private void btnFormaDePagoLibre_Click(object sender, EventArgs e)
        {
            var frmForma = new FormasDePago();
            frmForma.FormasDePagoSel = this.FormasDePagoLibre;
            if (frmForma.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.FormasDePagoLibre = frmForma.FormasDePagoSel;
            frmForma.Dispose();
        }

        private void chkNotaDeCredito_CheckedChanged(object sender, EventArgs e)
        {
            this.HabilitarTextosFP();
            if (this.chkNotaDeCredito.Checked && this.chkNotaDeCredito.Focused)
                this.btnEditarNotasDeCredito.Focus();
        }
                
        private void txtCheque_TextChanged(object sender, EventArgs e)
        {
            this.CalcularTotales();
        }

        private void txtTarjetaDeCredito_TextChanged(object sender, EventArgs e)
        {
            this.CalcularTotales();
        }

        private void txtTransferencia_TextChanged(object sender, EventArgs e)
        {
            this.CalcularTotales();
        }

        private void txtNoIdentificado_TextChanged(object sender, EventArgs e)
        {
            this.CalcularTotales();
        }

        private void txtNotaDeCredito_TextChanged(object sender, EventArgs e)
        {
            this.CalcularTotales();
        }

        private void btnEditarNotasDeCredito_Click(object sender, EventArgs e)
        {
            if (this.ctlNotasDeCredito == null)
            {
                this.ctlNotasDeCredito = new SeleccionarNotasDeCredito(this.ClienteID, this.VentaID);
                this.frmNotasDeCredito = new ContenedorControl("Vales", this.ctlNotasDeCredito);
            }

            this.frmNotasDeCredito.ShowDialog(Principal.Instance);
            if (this.frmNotasDeCredito.DialogResult == DialogResult.OK)
            {
                // Si se agregaron notas de otros clientes, se requerirá autorización
                if (this.ctlNotasDeCredito.HayNotasDeOtrosClientes)
                    this.ctlAdvertencia.PonerError(this.txtNotaDeCredito,
                        "Uno o más Vales no pertenecen al Cliente seleccionado. Se requerirá autorización para continuar.", ErrorIconAlignment.MiddleLeft);
                // Se agregan las notas de crédito al texto
                this.NotasDeCredito = this.ctlNotasDeCredito.GenerarNotasDeCredito();
                this.txtNotaDeCredito.Clear();
                foreach (var oNota in this.NotasDeCredito)
                    this.txtNotaDeCredito.Text += (", " + oNota.Key.ToString());
                this.txtNotaDeCredito.Text = (this.txtNotaDeCredito.Text == "" ? "" : this.txtNotaDeCredito.Text.Substring(2));
                
                this.CalcularTotales();
            }
        }

        private void btnAgregarVehiculo_Click(object sender, EventArgs e)
        {

        }

        private void cmbLeyanda_SelectedIndexChanged(object sender, EventArgs e)
        {
            var oLeyenda = (this.cmbLeyanda.SelectedItem as VentaTicketLeyenda);
            this.txtLeyenda.Text = (oLeyenda == null ? "" : oLeyenda.Leyenda);
        }

        #endregion

        #region [ Métodos ]

        private void HabilitarTextosFP()
        {
            this.txtEfectivo.Enabled = this.chkEfectivo.Checked;
            this.txtCheque.Enabled = this.chkCheque.Checked;
            this.txtTarjetaDeCredito.Enabled = (this.chkTarjetaDeCredito.Checked || this.chkTarjetaDeDebito.Checked);
            this.txtTransferencia.Enabled = this.chkTransferencia.Checked;
            this.cmbBanco.Enabled = (this.chkCheque.Checked || this.chkTarjetaDeCredito.Checked || this.chkTarjetaDeDebito.Checked || this.chkTransferencia.Checked);
            this.txtFolio.Enabled = (this.chkCheque.Checked || this.chkTarjetaDeCredito.Checked || this.chkTarjetaDeDebito.Checked || this.chkTransferencia.Checked);
            this.txtCuenta.Enabled = (this.chkCheque.Checked || this.chkTarjetaDeCredito.Checked || this.chkTarjetaDeDebito.Checked || this.chkTransferencia.Checked);
            this.txtNoIdentificado.Enabled = this.chkNoIdentificado.Checked;
            this.txtNotaDeCredito.Enabled = this.chkNotaDeCredito.Checked;
            this.btnEditarNotasDeCredito.Enabled = this.chkNotaDeCredito.Checked;

            if (!this.chkEfectivo.Checked) this.txtEfectivo.Clear();
            if (!this.chkCheque.Checked) this.txtCheque.Clear();
            if (!this.chkTarjetaDeCredito.Checked && !this.chkTarjetaDeDebito.Checked) this.txtTarjetaDeCredito.Clear();
            if (!this.chkTransferencia.Checked) this.txtTransferencia.Clear();
            if (!this.chkCheque.Checked && !this.chkTarjetaDeCredito.Checked && !this.chkTarjetaDeDebito.Checked && !this.chkTransferencia.Checked)
            {
                this.cmbBanco.SelectedIndex = -1;
                this.txtFolio.Clear();
                this.txtCuenta.Clear();
            }
            if (!this.chkNoIdentificado.Checked) this.txtNoIdentificado.Clear();
            if (!this.chkNotaDeCredito.Checked)
            {
                this.txtNotaDeCredito.Clear();
                this.NotasDeCredito.Clear();
                if (this.ctlNotasDeCredito != null)
                {
                    this.ctlNotasDeCredito.Dispose();
                    this.ctlNotasDeCredito = null;
                }
                if (this.frmNotasDeCredito != null)
                {
                    this.frmNotasDeCredito.Dispose();
                    this.frmNotasDeCredito = null;
                }
            }
        }
                
        private void CalcularTotales()
        {
            decimal mSuma = 0M;
            mSuma += (this.chkEfectivo.Checked ? this.txtEfectivo.Text.ValorDecimal() : 0);
            mSuma += (this.chkCheque.Checked ? this.txtCheque.Text.ValorDecimal() : 0);
            mSuma += ((this.chkTarjetaDeCredito.Checked || this.chkTarjetaDeDebito.Checked) ? this.txtTarjetaDeCredito.Text.ValorDecimal() : 0);
            mSuma += (this.chkTransferencia.Checked ? this.txtTransferencia.Text.ValorDecimal() : 0);
            mSuma += (this.chkNoIdentificado.Checked ? this.txtNoIdentificado.Text.ValorDecimal() : 0);
            mSuma += (this.chkNotaDeCredito.Checked ? this.ImporteDeNotasDeCredito() : 0);

            this.lblSuma.Text = mSuma.ToString(GlobalClass.FormatoDecimal);
            this.lblRestante.Text = (this.Total - mSuma).ToString(GlobalClass.FormatoDecimal);

            // Se colorea el importe restante, según sea el caso
            this.lblRestante.ForeColor = (this.lblRestante.Text == "0.00" ? Color.SteelBlue : Color.OrangeRed);
        }

        // Ya no se usa
        private void AgregarNotaDeCredito(int iNotaID)
        {
            if (this.NotasDeCredito.ContainsKey(iNotaID)) return;

            var Nota = Datos.GetEntity<NotaDeCredito>(q => q.NotaDeCreditoID == iNotaID);
            
            // Se verifica si existe la nota de crédito
            if (Nota == null)
            {
                UtilLocal.MensajeAdvertencia("La Nota de Crédito especificada no existe.");
                return;
            }

            // Se verifica si es del cliente seleccionado
            if (Nota.ClienteID != this.ClienteID)
                this.ctlAdvertencia.PonerError(this.txtNotaDeCredito,
                    "Una o más Notas de Crédito no pertenecen al Cliente seleccionado. Se requerirá autorización para continuar.", ErrorIconAlignment.MiddleLeft);
            
            // Se agrea la nota de crédito
            this.txtNotaDeCredito.Text += (", " + iNotaID.ToString());
            this.NotasDeCredito.Add(iNotaID, Nota.Importe);

            this.CalcularTotales();
        }

        private decimal ImporteDeNotasDeCredito()
        {
            decimal mTotal = 0M;
            foreach (var Nota in this.NotasDeCredito)
                mTotal += Nota.Value;
            return mTotal;
        }

        #endregion

        #region [ Públicos ]

        public void CambiarCliente(int iClienteID)
        {
            if (iClienteID == 0) return;

            this.ClienteID = iClienteID;

            var oCliente = Datos.GetEntity<Cliente>(q => q.ClienteID == iClienteID && q.Estatus);
            if (this.HabilitarTipoDePago)
                this.gpbTipoDePago.Enabled = oCliente.TieneCredito;
            // Si el cliente va a requerir siempre factura se checkea facturar y se deshabilita
            this.chkFacturar.Checked = oCliente.SiempreFactura.Valor();
            this.chkFacturar.Enabled = (!oCliente.SiempreFactura.Valor() && !oCliente.SiempreTicket.Valor());
            
            this.NotasDeCredito.Clear();
            this.txtNotaDeCredito.Clear();

            // Si tiene cliente comisionista, se llena y deshabilita el combo de comisionista
            this.cmbClienteComisionista.SelectedIndex = -1;
            if (oCliente.ClienteComisionistaID.Valor() > 0)
                this.cmbClienteComisionista.SelectedValue = oCliente.ClienteComisionistaID.Valor();
            this.cmbClienteComisionista.Enabled = (this.cmbClienteComisionista.SelectedIndex == -1);

            this.cmbVehiculo.CargarDatos("ClienteFlotillaID", "Etiqueta", Datos.GetListOf<ClientesFlotillasView>(c => c.ClienteID == this.ClienteID));
        }

        public virtual bool Validar()
        {
            // Se verifica si hay error por cobro a crédito y no tolerancia
            if (this.ctlError.TieneError(this.rdbCredito))
                return false;

            this.ctlError.LimpiarErrores();
            if (this.chkCheque.Checked || this.chkTarjetaDeCredito.Checked || this.chkTarjetaDeDebito.Checked || this.chkTransferencia.Checked)
            {
                if (this.cmbBanco.SelectedValue == null)
                    this.ctlError.PonerError(this.cmbBanco, "Debes especificar el Banco.", ErrorIconAlignment.MiddleLeft);
                if (this.txtFolio.Text == "")
                    this.ctlError.PonerError(this.lblFolio, "Debes especificar el Folio.", ErrorIconAlignment.MiddleLeft);
                if (this.txtCuenta.Text.Length != 4 || !Util.ValidarEntero(this.txtCuenta.Text))
                    this.ctlError.PonerError(this.lblCuenta, "Debes especificar los últimos 4 dígitos de la Cuenta.", ErrorIconAlignment.MiddleLeft);
            }
            if (this.cmbVendedor.SelectedValue == null)
                this.ctlError.PonerError(this.cmbVendedor, "Debes especificar el Vendedor.");
            if (this.rdbContado.Checked && this.lblRestante.Text != "0.00")
                this.ctlError.PonerError(this.lblEtRestante, "La Suma de la Venta no coincide con el Total.");

            return (this.ctlError.NumeroDeErrores == 0);
        }

        public bool CompletarCobro()
        {
            // Se valian los campos
            if (!this.Validar())
                return false;

            // Se pide el efectivo, si aplica
            bool bExito = true;
            decimal mEfectivo = this.txtEfectivo.Text.ValorDecimal();
            if (mEfectivo > 0 && !this.ACredito)
            {
                var frmEfectivo = new Efectivo(mEfectivo);
                bExito = (frmEfectivo.ShowDialog(Principal.Instance) == DialogResult.OK);
                if (bExito)
                    this.EfectivoRecibido = frmEfectivo.Recibido;
                frmEfectivo.Dispose();
            }

            if (!bExito)
                return false;

            // Se piden los datos de cobro con tarjeta, si aplica
            bExito = true;
            if (this.txtTarjetaDeCredito.Text.ValorDecimal() > 0)
            {
                bool bClienteMostrador = (this.ClienteID == Cat.Clientes.Mostrador);

                var frmTarjeta = new CobroTarjeta();
                if (!bClienteMostrador)
                {
                    var oCliente = Datos.GetEntity<Cliente>(c => c.ClienteID == this.ClienteID && c.Estatus);
                    frmTarjeta.Celular = oCliente.Celular;
                }
                bExito = (frmTarjeta.ShowDialog(Principal.Instance) == DialogResult.OK);
                if (bExito)
                {
                    this.BancoCuentaID = frmTarjeta.BancoCuentaID;
                    this.MesesSinIntereses = frmTarjeta.Meses;
                    this.CelularTarjeta = frmTarjeta.Celular;
                }
                frmTarjeta.Dispose();
            }

            return bExito;
        }

        public List<VentaPagoDetalle> GenerarPagoDetalle()
        {
            var PagoDetalle = new List<VentaPagoDetalle>();
            decimal mImporte;

            // Efectivo
            if (this.chkEfectivo.Checked && (mImporte = this.txtEfectivo.Text.ValorDecimal()) > 0)
                PagoDetalle.Add(new VentaPagoDetalle() { TipoFormaPagoID = Cat.FormasDePago.Efectivo, Importe = mImporte });
            // Cheque
            if (this.chkCheque.Checked && (mImporte = this.txtCheque.Text.ValorDecimal()) > 0)
                PagoDetalle.Add(new VentaPagoDetalle() {
                    TipoFormaPagoID = Cat.FormasDePago.Cheque, Importe = mImporte,
                    BancoID = Util.Entero(this.cmbBanco.SelectedValue), Folio = this.txtFolio.Text, Cuenta = this.txtCuenta.Text
                });
            // Tarjeta
            if (this.chkTarjetaDeCredito.Checked && (mImporte = this.txtTarjetaDeCredito.Text.ValorDecimal()) > 0)
                PagoDetalle.Add(new VentaPagoDetalle()
                {
                    TipoFormaPagoID = Cat.FormasDePago.Tarjeta, Importe = mImporte,
                    BancoID = Util.Entero(this.cmbBanco.SelectedValue), Folio = this.txtFolio.Text, Cuenta = this.txtCuenta.Text
                });
            // Tarjeta de Débito
            if (this.chkTarjetaDeDebito.Checked && (mImporte = this.txtTarjetaDeCredito.Text.ValorDecimal()) > 0)
                PagoDetalle.Add(new VentaPagoDetalle()
                {
                    TipoFormaPagoID = Cat.FormasDePago.TarjetaDeDebito, Importe = mImporte,
                    BancoID = Util.Entero(this.cmbBanco.SelectedValue), Folio = this.txtFolio.Text, Cuenta = this.txtCuenta.Text
                });
            // Transferencia
            if (this.chkTransferencia.Checked && (mImporte = this.txtTransferencia.Text.ValorDecimal()) > 0)
                PagoDetalle.Add(new VentaPagoDetalle()
                {
                    TipoFormaPagoID = Cat.FormasDePago.Transferencia, Importe = mImporte,
                    BancoID = Util.Entero(this.cmbBanco.SelectedValue), Folio = this.txtFolio.Text, Cuenta = this.txtCuenta.Text
                });
            // No identificado
            if (this.chkNoIdentificado.Checked && (mImporte = this.txtNoIdentificado.Text.ValorDecimal()) > 0)
                PagoDetalle.Add(new VentaPagoDetalle() { TipoFormaPagoID = Cat.FormasDePago.NoIdentificado, Importe = mImporte });
            // Notas de crédito
            if (this.chkNotaDeCredito.Checked)
            {
                foreach (var Nota in this.NotasDeCredito)
                    PagoDetalle.Add(new VentaPagoDetalle() { TipoFormaPagoID = Cat.FormasDePago.Vale, Importe = Nota.Value, NotaDeCreditoID = Nota.Key });
            }

            return PagoDetalle;
        }

        public void LimpiarDatos()
        {
            this.rdbContado.Checked = true;
            this.chkFacturar.Checked = false;
            this.chkFacturarDividir.Checked = false;
            this.LimpiarFormasDePago();
            if (this.FormasDePagoLibre != null)
                this.FormasDePagoLibre.Clear();
            // this.txtFormaDePagoLibre.Clear();
            this.cmbVendedor.SelectedIndex = -1;
            this.cmbRepartidor.SelectedIndex = -1;
            this.cmbClienteComisionista.SelectedIndex = -1;
            this.cmbVehiculo.SelectedIndex = -1;
            this.txtKilometraje.Clear();

            this.EfectivoRecibido = null;
            this.BancoCuentaID = 0;
        }

        public void LimpiarFormasDePago()
        {
            this.chkEfectivo.Checked = false;
            this.chkCheque.Checked = false;
            this.chkTarjetaDeCredito.Checked = false;
            this.chkTarjetaDeDebito.Checked = false;
            this.chkTransferencia.Checked = false;
            this.chkNoIdentificado.Checked = false;
            this.chkNotaDeCredito.Checked = false;
        }

        public void LlenarDatosGenerales(int iVentaID)
        {
            // Se limpian datos
            this.LimpiarDatos();
            // Se llenan los datos según la venta especificada
            var oVenta = Datos.GetEntity<Venta>(c => c.VentaID == iVentaID && c.Estatus);
            this.VentaID = iVentaID;
            // this.ClienteID = oVenta.ClienteID;
            this.CambiarCliente(oVenta.ClienteID);
            this.rdbCredito.Checked = oVenta.ACredito;

            // Se quita la marca de Factura, si aplica. Para cuando es un anticipo clientes
            if (Datos.Exists<VentaDetalle>(c => c.VentaID == iVentaID && c.ParteID == Cat.Partes.AnticipoClientes && c.Estatus))
            {
                this.chkFacturar.Checked = false;
                this.chkFacturar.Enabled = true;
            }

            this.cmbVendedor.SelectedValue = oVenta.RealizoUsuarioID;
            this.cmbRepartidor.SelectedValue = oVenta.RepartidorID.Valor();
            this.cmbClienteComisionista.SelectedValue = oVenta.ComisionistaClienteID.Valor();
            this.cmbVehiculo.SelectedValue = oVenta.ClienteVehiculoID.Valor();
            this.txtKilometraje.Text = Util.Cadena(oVenta.Kilometraje);
            //this.Total = oVenta.Total;
        }

        public void LlenarDatosFormasDePago(int iVentaPagoID)
        {
            // Se desmarcan los checks
            this.chkEfectivo.Checked = false;
            this.chkCheque.Checked = false;
            this.chkTarjetaDeCredito.Checked = false;
            this.chkTarjetaDeDebito.Checked = false;
            this.chkTransferencia.Checked = false;
            this.chkNoIdentificado.Checked = false;
            this.chkNotaDeCredito.Checked = false;

            // Se llenan los datos
            var oPagoDetalle = Datos.GetListOf<VentasPagosDetalleView>(q => q.VentaPagoID == iVentaPagoID);
            // this.rdbCredito.Checked = (oPagoDetalle.Count > 0 && oPagoDetalle[0].TipoDePagoID.Valor() == Cat.TiposDePago.Credito);
            decimal mPago = 0;
            foreach (var oFormaDePago in oPagoDetalle)
            {
                switch (oFormaDePago.FormaDePagoID)
                {
                    case Cat.FormasDePago.Efectivo:
                        this.chkEfectivo.Checked = true;
                        this.txtEfectivo.Text = oFormaDePago.Importe.ToString(GlobalClass.FormatoMoneda);
                        break;
                    case Cat.FormasDePago.Cheque:
                    case Cat.FormasDePago.Tarjeta:
                    case Cat.FormasDePago.TarjetaDeDebito:
                    case Cat.FormasDePago.Transferencia:
                        switch (oFormaDePago.FormaDePagoID)
                        {
                            case Cat.FormasDePago.Cheque:
                                this.chkCheque.Checked = true;
                                this.txtCheque.Text = oFormaDePago.Importe.ToString(GlobalClass.FormatoMoneda);
                                break;
                            case Cat.FormasDePago.Tarjeta:
                                this.chkTarjetaDeCredito.Checked = true;
                                this.txtTarjetaDeCredito.Text = oFormaDePago.Importe.ToString(GlobalClass.FormatoMoneda);
                                break;
                            case Cat.FormasDePago.TarjetaDeDebito:
                                this.chkTarjetaDeDebito.Checked = true;
                                this.txtTarjetaDeCredito.Text = oFormaDePago.Importe.ToString(GlobalClass.FormatoMoneda);
                                break;
                            case Cat.FormasDePago.Transferencia:
                                this.chkTransferencia.Checked = true;
                                this.txtTransferencia.Text = oFormaDePago.Importe.ToString(GlobalClass.FormatoMoneda);
                                break;
                        }
                        this.cmbBanco.Text = oFormaDePago.Banco;
                        this.txtFolio.Text = oFormaDePago.Folio;
                        this.txtCuenta.Text = oFormaDePago.Cuenta;
                        break;
                    case Cat.FormasDePago.NoIdentificado:
                        this.chkNoIdentificado.Checked = true;
                        this.txtNoIdentificado.Text = oFormaDePago.Importe.ToString(GlobalClass.FormatoMoneda);
                        break;
                    case Cat.FormasDePago.Vale:
                        this.chkNotaDeCredito.Checked = true;
                        // Sólo se agregan en el TextBox de forma informativa, no se agregan en el listado de notas (this.NotasDeCredito)
                        this.txtNotaDeCredito.Text += ((this.txtNotaDeCredito.Text == "" ? "" : ", ") + oFormaDePago.NotaDeCreditoID.Valor().ToString());
                        break;
                }
                mPago += oFormaDePago.Importe;
            }
            this.Total = mPago;
        }

        public void EstablecerFormaDePagoPredeterminada(int iFormaDePagoID, decimal mImporte, int iBancoID, string sCuenta, bool bMenorQue2000Efectivo)
        {
            // Se limpian las formas de pago
            this.LimpiarFormasDePago();
            //
            string sImporte = mImporte.ToString(GlobalClass.FormatoMoneda);
            if (bMenorQue2000Efectivo)
                iFormaDePagoID = Cat.FormasDePago.Efectivo;
            // Se asigna la forma de pago especificada
            switch (iFormaDePagoID)
            {
                default:
                case Cat.FormasDePago.Efectivo:
                    this.chkEfectivo.Checked = true;
                    this.txtEfectivo.Text = sImporte;
                    break;
                case Cat.FormasDePago.Cheque:
                    this.chkCheque.Checked = true;
                    this.txtCheque.Text = sImporte;
                    break;
                case Cat.FormasDePago.Tarjeta:
                    this.chkTarjetaDeCredito.Checked = true;
                    this.txtTarjetaDeCredito.Text = sImporte;
                    break;
                case Cat.FormasDePago.TarjetaDeDebito:
                    this.chkTarjetaDeDebito.Checked = true;
                    this.txtTarjetaDeCredito.Text = sImporte;
                    break;
                case Cat.FormasDePago.Transferencia:
                    this.chkTransferencia.Checked = true;
                    this.txtTransferencia.Text = sImporte;
                    break;
                case Cat.FormasDePago.NoIdentificado:
                    // Ahora si está como predeterminado No Identificado, sólo se llena la leyenda "NO IDENTIFICADO", para que así se vea en la factura,
                    // pero no se marcar para que se obligue a seleccionar otro método de pago
                    this.chkEfectivo.Checked = true;
                    this.txtEfectivo.Text = sImporte;
                    // this.txtFormaDePagoLibre.Text = "NO IDENTIFICADO";
                    // this.chkNoIdentificado.Checked = true;
                    // this.txtNoIdentificado.Text = sImporte;
                    break;
            }

            if (iFormaDePagoID == Cat.FormasDePago.Cheque || iFormaDePagoID == Cat.FormasDePago.Tarjeta || iFormaDePagoID == Cat.FormasDePago.TarjetaDeDebito
                || iFormaDePagoID == Cat.FormasDePago.Transferencia)
            {
                this.cmbBanco.SelectedValue = iBancoID;
                this.txtCuenta.Text = sCuenta;
            }
        }

        public void EstablecerFormaDePagoPredeterminada(int iFormaDePagoID, decimal mImporte)
        {
            this.EstablecerFormaDePagoPredeterminada(iFormaDePagoID, mImporte, 0, "", false);
        }

        public List<int> NotasDeCreditoOtrosClientes()
        {
            if (this.ctlNotasDeCredito == null)
                return new List<int>();
            return this.ctlNotasDeCredito.NotasDeCreditoOtrosClientes();
        }

        public void Cancelar()
        {
            this.SendToBack();
        }

        #endregion
                
    }
}
