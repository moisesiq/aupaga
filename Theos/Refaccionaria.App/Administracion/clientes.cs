using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Calendar;
using System.IO;
using System.Diagnostics;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class clientes : UserControl
    {
        List<CalendarItem> oEventosCalendario;
        public VentasCobranza oCobranza;
        BindingSource estados, tipoPagos, bancos, tipoClientes, comisionistas, compras;
        ControlError cntError = new ControlError();
        BackgroundWorker bgworker;
        Cliente Cliente;
        ClienteFacturacion ClienteFacturacion;
        bool EsNuevo = true;
        bool modificoCredito;

        // Variables públicas
        public bool Guardado = false;
        public bool VieneDeVentas;

        // Para el Singleton
        private static clientes _Instance;
        public static clientes Instance
        {
            get
            {
                if (clientes._Instance == null || clientes._Instance.IsDisposed)
                    clientes._Instance = new clientes();
                return clientes._Instance;
            }
        }
        //

        public clientes()
        {
            InitializeComponent();

            this.CargaInicial();
        }

        #region [ Propiedades ]

        private int clienteId;
        public int ClienteID
        {
            get { return clienteId; }
            set { clienteId = value; }
        }

        public bool ModificaCredito { get { return this.gpoCredito.Enabled; } set { this.gpoCredito.Enabled = value; } }

        public bool ModificaCaracteristicas { get { return this.gpoCaracteristicas.Enabled; } set { this.gpoCaracteristicas.Enabled = value; } }

        #endregion

        #region [ Eventos ]

        private void clientes_Load(object sender, EventArgs e)
        {
            this.modificoCredito = false;

            // Para el grid de los vehículos
            this.dgvVehiculos.Inicializar();
            this.dgvVehiculos.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
        }

        private void tabClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabClientes.SelectedTab.Name)
            {
                case "tabClientesCalendario":
                    // Se llenan los datos del calendario, si no se ha hecho antes
                    if (this.calendar1.Items.Count <= 0)
                    {
                        this.CargarEventosCalendario();
                    }
                    break;
                case "tabControl":
                    this.CargarTableroControl(this.ClienteID);
                    break;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            
            this.Guardado = false;

            if (!Validaciones())
                return;

            var res = Util.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
            if (res == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;

            int iClienteID;
            try
            {
                SplashScreen.Show(new Splash());
                this.btnGuardar.Enabled = false;
                if (EsNuevo)
                {
                    var cliente = new Cliente()
                    {
                        Nombre = this.txtNombreCliente.Text,
                        Calle = this.txtCalle.Text,
                        NumeroExterior = this.txtNumeroExt.Text,
                        NumeroInterior = this.txtNumeroInt.Text,
                        Colonia = this.txtColonia.Text,
                        EstadoID = Util.Entero(this.cboEstado.SelectedValue),
                        MunicipioID = Util.Entero(this.cboMunicipio.SelectedValue),
                        CiudadID = Util.Entero(this.cboCiudad.SelectedValue),
                        CodigoPostal = this.txtCp.Text,
                        Alias = this.txtAlias.Text,
                        Telefono = this.txtTelOficina.Text,
                        Particular = this.txtTelParticular.Text,
                        Celular = this.txtTelCelular.Text,
                        Nextel = this.txtNextel.Text,

                        TieneCredito = this.chkTieneCredito.Checked,
                        Tolerancia = this.chkTolerancia.Checked,
                        DiasDeCredito = Util.Entero(this.txtPlazo.Text),
                        LimiteCredito = Util.Decimal(this.txtLimite.Text),
                        CobranzaContacto = this.txtCreditoContacto.Text,
                        CobranzaObservacion = this.txtCreditoObservacion.Text,

                        MenorQue2000Efectivo = this.chkMenor2000Efectivo.Checked,
                        TipoFormaPagoID = Util.Entero(this.cboMetodoPago.SelectedValue) > 0 ? Util.Entero(this.cboMetodoPago.SelectedValue) : default(int?),
                        BancoID = Util.Entero(this.cboBanco.SelectedValue) > 0 ? Util.Entero(this.cboBanco.SelectedValue) : default(int?),
                        CuentaBancaria = this.txtCuenta.Text,

                        ListaDePrecios = Util.Entero(this.nudListaPrecio.Value),
                        TipoClienteID = Util.Entero(this.cboTipoCliente.SelectedValue) > 0 ? Util.Entero(this.cboTipoCliente.SelectedValue) : default(int?),
                        ClienteComisionistaID = Util.Entero(this.cboClienteComisionista.SelectedValue) > 0 ? Util.Entero(this.cboClienteComisionista.SelectedValue) : default(int?),
                        EsClienteComisionista = this.chkClienteComisionista.Checked,
                        EsTallerElectrico = this.chkTallerElectrico.Checked,
                        EsTallerMecanico = this.chkTallerMecanico.Checked,
                        EsTallerDiesel = this.chkTallerDiesel.Checked,

                        Vip = this.chkVip.Checked,
                        CobroPorEnvio = this.chkCobroPorEnvio.Checked,
                        ImporteParaCobroPorEnvio = Util.Decimal(this.txtImporteParaCobroPorEnvio.Text),
                        ImporteCobroPorEnvio = Util.Decimal(this.txtImporteCobroPorEnvio.Text),
                        
                        DiaDeCobro = this.cbDiaCobro.SelectedIndex+1,
                        //HoraDeCobro = TimeSpan.Parse(this.cbCobrarCredito.Text);
                        //var hora= DateTime.Parse(this.dtpHoraCobro.Value);
                       // HoraDeCobro = TimeSpan.Parse(this.dtpHoraCobro.Value.TimeOfDay.ToString())
                       
                       //Factura
                       SiempreFactura = this.chkSiempreFactura.Checked,
                       SiempreTicket = this.chkSiempreTicket.Checked,
                       //Vale
                       SiempreVale = this.chkSiempreVale.Checked,
                       //Ticket precio 1
                       TicketPrecio1 = this.chkTicket1.Checked
                    };
                    Datos.Guardar<Cliente>(cliente);
                    iClienteID = cliente.ClienteID;

                    if (this.txtRfc.Text != "")
                    {
                        var clf = Datos.GetEntity<ClienteFacturacion>(c => c.ClienteID == cliente.ClienteID);
                        if (clf == null)
                        {
                            var clientef = new ClienteFacturacion()
                            {
                                ClienteID = cliente.ClienteID,
                                Rfc = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtRfc.Text),
                                RazonSocial = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNombreCliente.Text),
                                Calle = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtCalle.Text),
                                NumeroExterior = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNumeroExt.Text),
                                NumeroInterior = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNumeroInt.Text),
                                Colonia = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtColonia.Text),
                                CodigoPostal = this.txtCp.Text != "" ? UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtCp.Text) : "00000",
                                Pais = "MEXICO",
                                EstadoID = Util.Entero(this.cboEstado.SelectedValue),
                                MunicipioID = Util.Entero(this.cboMunicipio.SelectedValue),
                                CiudadID = Util.Entero(this.cboCiudad.SelectedValue)
                            };
                            Datos.Guardar<ClienteFacturacion>(clientef);
                        }
                        else
                        {
                            Datos.Guardar<ClienteFacturacion>(clf);
                        }
                    }
                    if (this.ModificaCredito)
                    {
                        if (Util.Decimal(this.txtPlazo.Text) > 0
                        || Util.Decimal(this.txtLimite.Text) > 0
                        || this.chkTieneCredito.Checked || this.chkTolerancia.Checked)
                        {
                            if (cliente.ClienteID > 0)
                            {
                                var r = this.chkTieneCredito.Checked == true ? "SI" : "NO";
                                var t = this.chkTolerancia.Checked == true ? "SI" : "NO";
                                var credito = new ClienteCredito()
                                {
                                    ClienteID = cliente.ClienteID,
                                    Accion = string.Format("Plazo:{0}, Limite:{1}, Restringir:{2}, Tolerancia:{3}", txtPlazo.Text, txtLimite.Text, r, t),
                                    Comentario = this.txtComentario.Text
                                };
                                Datos.Guardar<ClienteCredito>(credito);
                            }
                        }
                    }

                    catalogosClientes.Instance.CustomInvoke<catalogosClientes>(m => m.seleccionarCliente(cliente.ClienteID));
                }
                else
                {
                    //Modificar Cliente
                    Cliente.Nombre = this.txtNombreCliente.Text;
                    Cliente.Calle = this.txtCalle.Text;
                    Cliente.NumeroExterior = this.txtNumeroExt.Text;
                    Cliente.NumeroInterior = this.txtNumeroInt.Text;
                    Cliente.Colonia = this.txtColonia.Text;
                    Cliente.EstadoID = Util.Entero(this.cboEstado.SelectedValue);
                    Cliente.MunicipioID = Util.Entero(this.cboMunicipio.SelectedValue);
                    Cliente.CiudadID = Util.Entero(this.cboCiudad.SelectedValue);
                    Cliente.CodigoPostal = this.txtCp.Text;
                    Cliente.Alias = this.txtAlias.Text;
                    Cliente.Telefono = this.txtTelOficina.Text;
                    Cliente.Particular = this.txtTelParticular.Text;
                    Cliente.Celular = this.txtTelCelular.Text;
                    Cliente.Nextel = this.txtNextel.Text;

                    Cliente.TieneCredito = this.chkTieneCredito.Checked;
                    Cliente.Tolerancia = this.chkTolerancia.Checked;
                    Cliente.DiasDeCredito = Util.Entero(this.txtPlazo.Text);
                    Cliente.LimiteCredito = Util.Decimal(this.txtLimite.Text);
                    Cliente.CobranzaContacto = this.txtCreditoContacto.Text;
                    Cliente.CobranzaObservacion = this.txtCreditoObservacion.Text;

                    Cliente.MenorQue2000Efectivo = this.chkMenor2000Efectivo.Checked;
                    Cliente.TipoFormaPagoID = Util.Entero(this.cboMetodoPago.SelectedValue);
                    Cliente.BancoID = Util.Entero(this.cboBanco.SelectedValue);
                    Cliente.CuentaBancaria = this.txtCuenta.Text;

                    Cliente.ListaDePrecios = Util.Entero(this.nudListaPrecio.Value);
                    Cliente.TipoClienteID = Util.Entero(this.cboTipoCliente.SelectedValue) > 0 ? Util.Entero(this.cboTipoCliente.SelectedValue) : default(int?);
                    Cliente.ClienteComisionistaID = Util.Entero(this.cboClienteComisionista.SelectedValue) > 0 ? Util.Entero(this.cboClienteComisionista.SelectedValue) : default(int?);
                    Cliente.EsClienteComisionista = this.chkClienteComisionista.Checked;
                    Cliente.EsTallerElectrico = this.chkTallerElectrico.Checked;
                    Cliente.EsTallerMecanico = this.chkTallerMecanico.Checked;
                    Cliente.EsTallerDiesel = this.chkTallerDiesel.Checked;

                    Cliente.Vip = this.chkVip.Checked;
                    Cliente.CobroPorEnvio = this.chkCobroPorEnvio.Checked;
                    Cliente.ImporteParaCobroPorEnvio = Util.Decimal(this.txtImporteParaCobroPorEnvio.Text);
                    Cliente.ImporteCobroPorEnvio = Util.Decimal(this.txtImporteCobroPorEnvio.Text);

                    //Guardar dia y hora de preferencia de cobro
                    Cliente.DiaDeCobro = this.cbDiaCobro.SelectedIndex+1;
                    Cliente.HoraDeCobro = TimeSpan.Parse(this.dtpHoraCobro.Value.TimeOfDay.ToString());
                    Cliente.SiempreFactura = this.chkSiempreFactura.Checked;
                    Cliente.SiempreTicket = this.chkSiempreTicket.Checked;
                    Cliente.SiempreVale = this.chkSiempreVale.Checked;
                    Cliente.TicketPrecio1 = this.chkTicket1.Checked;
                    Datos.Guardar<Cliente>(Cliente);
                    iClienteID = Cliente.ClienteID;

                    //Modificar Cliente Facturacion                    
                    if (ClienteFacturacion != null)
                    {
                        ClienteFacturacion.Rfc = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtRfc.Text);
                        ClienteFacturacion.RazonSocial = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNombreCliente.Text);
                        ClienteFacturacion.Calle = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtCalle.Text);
                        ClienteFacturacion.NumeroExterior = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNumeroExt.Text);
                        ClienteFacturacion.NumeroInterior = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNumeroInt.Text);
                        ClienteFacturacion.Colonia = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtColonia.Text);
                        ClienteFacturacion.CodigoPostal = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtCp.Text);
                        ClienteFacturacion.Pais = "MEXICO";
                        ClienteFacturacion.EstadoID = Util.Entero(this.cboEstado.SelectedValue);
                        ClienteFacturacion.MunicipioID = Util.Entero(this.cboMunicipio.SelectedValue);
                        ClienteFacturacion.CiudadID = Util.Entero(this.cboCiudad.SelectedValue);
                        Datos.Guardar<ClienteFacturacion>(ClienteFacturacion);
                    }
                    else
                    {
                        if (this.txtRfc.Text != "")
                        {
                            var clientef = new ClienteFacturacion()
                            {
                                ClienteID = Cliente.ClienteID,
                                Rfc = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtRfc.Text),
                                RazonSocial = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNombreCliente.Text),
                                Calle = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtCalle.Text),
                                NumeroExterior = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNumeroExt.Text),
                                NumeroInterior = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtNumeroInt.Text),
                                Colonia = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtColonia.Text),
                                CodigoPostal = UtilLocal.LimpiarCadenaDeEspaciosBlancos(this.txtCp.Text),
                                Pais = "MEXICO",
                                EstadoID = Util.Entero(this.cboEstado.SelectedValue),
                                MunicipioID = Util.Entero(this.cboMunicipio.SelectedValue),
                                CiudadID = Util.Entero(this.cboCiudad.SelectedValue)
                            };
                            Datos.Guardar<ClienteFacturacion>(clientef);
                        }
                    }

                    if (modificoCredito && this.ModificaCredito)
                    {
                        var r = this.chkTieneCredito.Checked == true ? "SI" : "NO";
                        var t = this.chkTolerancia.Checked == true ? "SI" : "NO";
                        var credito = new ClienteCredito()
                        {
                            ClienteID = Cliente.ClienteID,
                            Accion = string.Format("Plazo:{0}, Limite:{1}, Restringir:{2}, Tolerancia:{3}", txtPlazo.Text, txtLimite.Text, r, t),
                            Comentario = this.txtComentario.Text
                        };
                        Datos.Guardar<ClienteCredito>(credito);
                        txtComentario.Text = "";
                        cargarClienteCredito(Cliente.ClienteID);
                    }
                }

                // Se guardan los datos de los vehículos
                ClienteFlotilla oReg;
                foreach (DataGridViewRow oFila in this.dgvVehiculos.Rows)
                {
                    if (oFila.IsNewRow) continue;

                    int iId = this.dgvVehiculos.ObtenerId(oFila);
                    int iCambio = this.dgvVehiculos.ObtenerIdCambio(oFila);
                    switch (iCambio)
                    {
                        case Cat.TiposDeAfectacion.Agregar:
                        case Cat.TiposDeAfectacion.Modificar:
                            if (iCambio == Cat.TiposDeAfectacion.Agregar)
                                oReg = new ClienteFlotilla() { ClienteID = iClienteID };
                            else
                                oReg = Datos.GetEntity<ClienteFlotilla>(c => c.ClienteFlotillaID == iId && c.Estatus);

                            oReg.NumeroEconomico = Util.Cadena(oFila.Cells["NumeroEconomico"].Value);
                            oReg.Anio = (Util.Entero(oFila.Cells["Anio"].Value) > 0 ? (int?)Util.Entero(oFila.Cells["Anio"].Value) : null);
                            oReg.MotorID = (Util.Entero(oFila.Cells["MotorID"].Value) > 0
                                ? (int?)Util.Entero(oFila.Cells["MotorID"].Value) : null);
                            oReg.VehiculoTipoID = (Util.Entero(oFila.Cells["TipoID"].Value) > 0
                                ? (int?)Util.Entero(oFila.Cells["TipoID"].Value) : null);
                            oReg.Vin = Util.Cadena(oFila.Cells["Vin"].Value);
                            oReg.Color = Util.Cadena(oFila.Cells["Color"].Value);
                            oReg.Placa = Util.Cadena(oFila.Cells["Placa"].Value);
                            oReg.Kilometraje = Util.Entero(oFila.Cells["Kilometraje"].Value);

                            Datos.Guardar<ClienteFlotilla>(oReg);
                            break;
                        case Cat.TiposDeAfectacion.Borrar:
                            oReg = Datos.GetEntity<ClienteFlotilla>(c => c.ClienteFlotillaID == iId && c.Estatus);
                            Datos.Eliminar<ClienteFlotilla>(oReg, true);
                            break;
                    }
                }
                this.CargarClienteVehiculos(iClienteID);

                new Notificacion("Cliente Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                this.modificoCredito = false;
                this.Cursor = Cursors.Default;
                this.Guardado = true;
                this.clienteId = iClienteID;

                this.txtComentario.Clear();

                //refrescar calendario
                this.CargarEventosCalendario();

                SplashScreen.Close();
                this.btnGuardar.Enabled = true;


            }
            catch (Exception ex)
            {
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
                this.Cursor = Cursors.Default;
            }
        }

        private void btnAgregarPersonal_Click(object sender, EventArgs e)
        {
            if (!EsNuevo && Cliente != null)
            {
                var personal = new DetalleClientePersonal();
                personal.ClienteId = Cliente.ClienteID;
                personal.ShowDialog();
            }
        }

        private void cboEstado_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(this.cboEstado.SelectedValue.ToString(), out id))
            {
                CargarMunicipios(id);
            }
        }

        private void cboMunicipio_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(this.cboMunicipio.SelectedValue.ToString(), out id))
            {
                CargarCiudades(id);
            }
        }

        private void txtCp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar != '\b'))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void chkSiempreFactura_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkSiempreFactura.Focused && this.chkSiempreTicket.Checked)
                this.chkSiempreTicket.Checked = false;
        }

        private void chkSiempreTicket_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkSiempreTicket.Focused && this.chkSiempreFactura.Checked)
                this.chkSiempreFactura.Checked = false;
        }

        private void chkCobroPorEnvio_CheckedChanged(object sender, EventArgs e)
        {
            this.txtImporteCobroPorEnvio.Enabled = this.chkCobroPorEnvio.Checked;
            this.txtImporteParaCobroPorEnvio.Enabled = this.chkCobroPorEnvio.Checked;
        }

        private void dgvDatosPersonal_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDatosPersonal.CurrentRow == null) return;
            int iPersonaID = Util.Entero(this.dgvDatosPersonal.CurrentRow.Cells["ClientePersonalID"].Value);
            var frmPersona = new DetalleClientePersonal(iPersonaID) { ClienteId = this.Cliente.ClienteID };
            frmPersona.ShowDialog(Principal.Instance);
            frmPersona.Dispose();
        }

        private void dgvDatosPersonal_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatosPersonal.CurrentRow == null) return;

            if (e.KeyCode == Keys.Delete)
            {
                if (UtilLocal.MensajePregunta("¿Estás seguro que deseas eliminar el registro de persona seleccionado?") == DialogResult.Yes)
                {
                    int iPersonaID = Util.Entero(this.dgvDatosPersonal.CurrentRow.Cells["ClientePersonalID"].Value);
                    var oPersona = Datos.GetEntity<ClientePersonal>(c => c.ClientePersonalID == iPersonaID && c.Estatus);
                    Datos.Eliminar<ClientePersonal>(oPersona, true);
                    this.cargarClientePersonal(this.Cliente.ClienteID);
                }
            }
        }

        private void dgvVehiculos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var oFila = this.dgvVehiculos.Rows[e.RowIndex];
            string sColumna = this.dgvVehiculos.Columns[e.ColumnIndex].Name;
            int iId = Util.Entero(oFila.Cells[sColumna].Value);
            switch (sColumna)
            {
                case "MarcaID":
                    oFila.Cells["ModeloID"].Value = null;
                    (oFila.Cells["ModeloID"] as DataGridViewComboBoxCell).CargarDatos("ModeloID", "NombreModelo", Datos.GetListOf<Modelo>(
                        c => c.MarcaID == iId && c.Estatus).OrderBy(o => o.NombreModelo).ToList());
                    break;
                case "ModeloID":
                    oFila.Cells["Anio"].Value = null;
                    (oFila.Cells["Anio"] as DataGridViewComboBoxCell).CargarDatos("Anio", "Anio", Datos.GetListOf<ModelosAniosView>(
                        c => c.ModeloID == iId).OrderBy(o => o.Anio).ToList());
                    break;
                case "Anio":
                    oFila.Cells["MotorID"].Value = null;
                    int iModeloId = Util.Entero(oFila.Cells["ModeloID"].Value);
                    (oFila.Cells["MotorID"] as DataGridViewComboBoxCell).CargarDatos("MotorID", "NombreMotor", Datos.GetListOf<MotoresAniosView>(
                        c => c.ModeloID == iModeloId && c.Anio == iId).OrderBy(o => o.NombreMotor).ToList());
                    break;
            }
        }

        private void btnAvisos_Click(object sender, EventArgs e)
        {
            Eventos.Instance.Show();
        }

        #endregion

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

            //Validar permiso exportar catalogo clientes
            //if (LogIn.VerPermiso("Administracion.Clientes.ExportarListado", false))

            // No sé para qué estaba esta línea, se quitó. Moi 2016-03-15
            // tabControl.Show();

            if(!UtilLocal.ValidarPermiso("Administracion.Clientes.ExportarListado"))
                this.btnExportar.Visible = false;
            if (!UtilLocal.ValidarPermiso("Administracion.Clientes.Calendario.Ver"))
                this.tabClientes.TabPages.Remove(tabClientesCalendario);
            try
            {
                bgworker = new BackgroundWorker();
                bgworker.DoWork += ActualizarListados;
                bgworker.RunWorkerCompleted += Terminado;
                bgworker.RunWorkerAsync();

                var listaSuc = Datos.GetListOf<Sucursal>(s => s.Estatus == true).ToList();
                // listaSuc.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "TODOS" });
                this.cboSucursal.DataSource = listaSuc;
                this.cboSucursal.DisplayMember = "NombreSucursal";
                this.cboSucursal.ValueMember = "SucursalID";

                this.cboSucursal.SelectedIndex = -1;

                this.dtpFechaDesde.Value = DateTime.Now.AddMonths(-3);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
                
        public void LimpiarFormulario()
        {
            this.txtNombreCliente.Clear();
            this.txtCalle.Clear();
            this.txtColonia.Clear();
            this.txtCp.Clear();
            this.txtRfc.Clear();
            this.txtNumeroExt.Clear();
            this.txtNumeroInt.Clear();
            this.txtAlias.Clear();
            this.txtTelOficina.Clear();
            this.txtTelParticular.Clear();
            this.txtTelCelular.Clear();
            this.txtNextel.Clear();
            this.txtFechaIngreso.Clear();

            this.txtPlazo.Clear();
            this.txtPromedioPagoTotal.Clear();
            this.txtPromedioPago3meses.Clear();
            this.txtLimite.Clear();
            this.txtDeudaActual.Clear();
            this.txtVencido.Clear();
            this.chkTieneCredito.Checked = false;
            this.chkTolerancia.Checked = false;
            this.txtCreditoContacto.Clear();
            this.txtCreditoObservacion.Clear();

            this.chkSiempreFactura.Checked = false;
            this.chkSiempreTicket.Checked = false;
            this.chkSiempreVale.Checked = false;
            this.chkTicket1.Checked = false;
            this.txtComentario.Clear();
            this.txtCuenta.Clear();
            this.nudListaPrecio.Value = 1;
            this.txtAcumulado.Clear();
            this.chkCobroPorEnvio.Checked = false;
            this.txtImporteParaCobroPorEnvio.Clear();
            this.txtImporteCobroPorEnvio.Clear();
            this.chkClienteComisionista.Checked = false;
            this.chkTallerElectrico.Checked = false;
            this.chkTallerMecanico.Checked = false;
            this.chkTallerDiesel.Checked = false;

            this.chkVip.Checked = false;
            
            this.dtpHoraCobro.Value = DateTime.Now;
            this.cbDiaCobro.SelectedIndex = (int)DateTime.Today.DayOfWeek - 1;

           this.dgvDatosPersonal.DataSource = null;
            this.dgvCredito.DataSource = null;

            this.dgvVehiculos.Rows.Clear();

            this.txtBusqueda.Clear();
            this.dgvDatos.DataSource = null;
            this.dgvDetalle.DataSource = null;

            this.LlenarUbicacionPredeterminada();
            this.cboTipoCliente.SelectedValue = Cat.TiposDeCliente.Particular;
        }

        public void ActualizarListados(object o, DoWorkEventArgs e)
        {
            try
            {
                this.estados = new BindingSource();
                this.tipoPagos = new BindingSource();
                this.bancos = new BindingSource();
                this.tipoClientes = new BindingSource();
                this.comisionistas = new BindingSource();
                this.estados.DataSource = Datos.GetListOf<Estado>().ToList();
                this.tipoClientes.DataSource = Datos.GetListOf<TipoCliente>(t => t.Estatus).ToList();
                this.comisionistas.DataSource = Datos.GetListOf<Cliente>(c => c.EsClienteComisionista == true && c.Estatus).ToList();

                var tipoPagos = Datos.GetListOf<TipoFormaPago>(t => t.Estatus).ToList();
                tipoPagos.Insert(0, new TipoFormaPago() { TipoFormaPagoID = 0, NombreTipoFormaPago = "" });
                this.tipoPagos.DataSource = tipoPagos;

                var bancos = Datos.GetListOf<Banco>().ToList();
                bancos.Insert(0, new Banco() { BancoID = 0, NombreBanco = "" });
                this.bancos.DataSource = bancos;

                // Se llenan los combos del grid de vehículos
                this.MarcaID.CargarDatos("MarcaID", "NombreMarca", Datos.GetListOf<Marca>(c => c.Estatus).OrderBy(c => c.NombreMarca).ToList());
                this.ModeloID.CargarDatos("ModeloID", "NombreModelo", Datos.GetListOf<Modelo>(c => c.Estatus).OrderBy(c => c.NombreModelo).ToList());
                this.Anio.CargarDatos("Anio", "Anio", Datos.GetListOf<MotorAnio>(c => c.Estatus).Select(c => new { Anio = c.Anio }).Distinct().ToList());
                this.MotorID.CargarDatos("MotorID", "NombreMotor", Datos.GetListOf<Motor>(c => c.Estatus).OrderBy(c => c.NombreMotor).ToList());
                this.TipoID.CargarDatos("VehiculoTipoID", "Tipo", Datos.GetListOf<VehiculoTipo>().OrderBy(c => c.Tipo).ToList());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void Terminado(object o, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var listaBancos = (List<Banco>)bancos.DataSource;
                this.cboBanco.DisplayMember = "NombreBanco";
                this.cboBanco.DataSource = listaBancos;
                this.cboBanco.ValueMember = "BancoID";
                // this.cboBanco.TextUpdate += new EventHandler(Util.cboCharacterCasingUpper);

                this.cboMetodoPago.DisplayMember = "NombreTipoFormaPago";
                this.cboMetodoPago.DataSource = (List<TipoFormaPago>)tipoPagos.DataSource;
                this.cboMetodoPago.ValueMember = "TipoFormaPagoID";

                var listaEstados = (List<Estado>)estados.DataSource;
                this.cboEstado.DisplayMember = "NombreEstado";
                this.cboEstado.DataSource = listaEstados;
                this.cboEstado.ValueMember = "EstadoID";
                // this.cboEstado.TextUpdate += new EventHandler(Util.cboCharacterCasingUpper);

                var listadoTipoCliente = (List<TipoCliente>)tipoClientes.DataSource;
                this.cboTipoCliente.DisplayMember = "NombreTipoCliente";
                this.cboTipoCliente.DataSource = listadoTipoCliente;
                this.cboTipoCliente.ValueMember = "TipoClienteID";

                var listadoComisionistas = (List<Cliente>)comisionistas.DataSource;
                listadoComisionistas.Insert(0, new Cliente() { ClienteID = 0, Nombre = "", EsClienteComisionista = true });
                this.cboClienteComisionista.DisplayMember = "Nombre";
                this.cboClienteComisionista.DataSource = listadoComisionistas;
                this.cboClienteComisionista.ValueMember = "ClienteID";
                // this.cboClienteComisionista.TextUpdate += new EventHandler(Util.cboCharacterCasingUpper);

                // Se quita esta línea porque si se queda, como es una llamada asíncrona, al cargar datos de un cliente y mostrar se alcanza a ejecutar este
                // código y eso ocasionaba que se mostrará el formulario sin datos, cuando se abre para editar desde ventas. - Moi 2015/10/16
                // this.LimpiarFormulario();

                if (this.EsNuevo)
                {
                    this.LlenarUbicacionPredeterminada();
                    this.cboTipoCliente.SelectedValue = Cat.TiposDeCliente.Particular;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarMunicipios(int estadoId)
        {
            try
            {
                var listaMunicipios = Datos.GetListOf<Municipio>(m => m.EstadoID.Equals(estadoId));
                this.cboMunicipio.DataSource = listaMunicipios;
                this.cboMunicipio.DisplayMember = "NombreMunicipio";
                this.cboMunicipio.ValueMember = "MunicipioID";
                AutoCompleteStringCollection autMunicipio = new AutoCompleteStringCollection();
                foreach (var municipio in listaMunicipios) autMunicipio.Add(municipio.NombreMunicipio);
                this.cboMunicipio.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboMunicipio.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboMunicipio.AutoCompleteCustomSource = autMunicipio;
                this.cboMunicipio.TextUpdate += new EventHandler(UtilLocal.cboCharacterCasingUpper);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarCiudades(int municipioId)
        {
            try
            {
                var listaCiudades = Datos.GetListOf<Ciudad>(m => m.MunicipioID.Equals(municipioId));
                this.cboCiudad.DataSource = listaCiudades;
                this.cboCiudad.DisplayMember = "NombreCiudad";
                this.cboCiudad.ValueMember = "CiudadID";
                AutoCompleteStringCollection autCiudad = new AutoCompleteStringCollection();
                foreach (var ciudad in listaCiudades) autCiudad.Add(ciudad.NombreCiudad);
                this.cboCiudad.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboCiudad.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboCiudad.AutoCompleteCustomSource = autCiudad;
                this.cboCiudad.TextUpdate += new EventHandler(UtilLocal.cboCharacterCasingUpper);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void LlenarUbicacionPredeterminada()
        {
            this.cboEstado.SelectedValue = Cat.Estados.Jalisco;
            this.cboMunicipio.SelectedValue = Cat.Municipios.ZapotlanElGrande;
            this.cboCiudad.SelectedValue = Cat.Ciudades.CiudadGuzman;
        }

        public void CargarCliente(int clienteId)
        {
            this.LimpiarFormulario();
            //Si viene de Ventas Ocultar  los contenedores de ""abajo"" excepto Personal y vehículos
            if (VieneDeVentas)
            {
                this.tabClientes.TabPages.Remove(this.tabClientesCalendario);
                this.tabClientes.TabPages.Remove(this.tabControl);
                this.gpoCredito.Visible = false;
                this.gpoMetodoPago.Visible = false;
                this.gpoCaracteristicas.Visible = false;
                this.btnCobranza.Visible = false;
            }
            try
            {
                if (clienteId <= 0)
                {
                    EsNuevo = true;
                    // this.LimpiarFormulario()
                    this.modificoCredito = false;
                    this.gpoDatosUsuarios.Enabled = false;
                    return;
                }

                var cliente = Datos.GetEntity<ClientesView>(c => c.ClienteID.Equals(clienteId));
                if (cliente != null)
                {
                    EsNuevo = false;
                    this.gpoDatosUsuarios.Enabled = true;
                    Cliente = Datos.GetEntity<Cliente>(c => c.ClienteID == cliente.ClienteID);
                    this.ClienteID = clienteId;
                    ClienteFacturacion = Datos.GetEntity<ClienteFacturacion>(c => c.ClienteID == cliente.ClienteID);

                    if (this.cboEstado.DataSource == null)
                    {
                        this.bancos = new BindingSource();
                        var bancos = Datos.GetListOf<Banco>().ToList();
                        bancos.Insert(0, new Banco() { BancoID = 0, NombreBanco = "" });
                        this.bancos.DataSource = bancos;

                        var listaEstados = (List<Estado>)estados.DataSource;
                        this.cboEstado.DisplayMember = "NombreEstado";
                        this.cboEstado.DataSource = listaEstados;
                        this.cboEstado.ValueMember = "EstadoID";
                    }

                    this.txtNombreCliente.Text = cliente.Nombre;
                    this.txtRfc.Text = cliente.Rfc;
                    this.txtCalle.Text = cliente.Calle;
                    this.txtNumeroExt.Text = cliente.NumeroExterior;
                    this.txtNumeroInt.Text = cliente.NumeroInterior;
                    this.txtColonia.Text = cliente.Colonia;

                    this.txtPais.Text = "MEXICO";
                    this.txtCp.Text = cliente.CodigoPostal;
                    this.txtFechaIngreso.Text = Util.Cadena(cliente.FechaRegistro.ToShortDateString());
                    this.txtAlias.Text = cliente.Alias;
                    this.txtTelOficina.Text = cliente.Telefono;
                    this.txtTelParticular.Text = cliente.Particular;
                    this.txtTelCelular.Text = cliente.Celular;
                    this.txtNextel.Text = cliente.Nextel;
                    
                    this.txtPlazo.Text = Util.Cadena(cliente.DiasDeCredito);
                    this.txtLimite.Text = UtilLocal.DecimalToCadenaMoneda(cliente.LimiteCredito);
                    this.chkTieneCredito.Checked = Util.Logico(cliente.TieneCredito);
                    this.chkTolerancia.Checked = Util.Logico(cliente.Tolerancia);
                    this.txtCreditoContacto.Text = cliente.CobranzaContacto;
                    this.txtCreditoObservacion.Text = cliente.CobranzaObservacion;

                    this.chkMenor2000Efectivo.Checked = cliente.MenorQue2000Efectivo.Valor();
                    if (cliente.TipoFormaPagoID != null)
                        this.cboMetodoPago.SelectedValue = cliente.TipoFormaPagoID;
                    if (cliente.BancoID != null)
                        this.cboBanco.SelectedValue = cliente.BancoID;
                    this.txtCuenta.Text = cliente.CuentaBancaria;
                    this.nudListaPrecio.Value = cliente.ListaDePrecios < 1 ? 1 : cliente.ListaDePrecios;
                    if (cliente.TipoClienteID != null)
                        this.cboTipoCliente.SelectedValue = cliente.TipoClienteID;
                    this.chkClienteComisionista.Checked = Util.Logico(cliente.EsClienteComisionista);
                    this.chkTallerMecanico.Checked = Util.Logico(cliente.EsTallerMecanico);
                    this.chkTallerElectrico.Checked = Util.Logico(cliente.EsTallerElectrico);
                    this.chkTallerDiesel.Checked = Util.Logico(cliente.EsTallerDiesel);

                    this.chkVip.Checked = Util.Logico(cliente.Vip);
                    this.chkCobroPorEnvio.Checked = Cliente.CobroPorEnvio.Valor();
                    this.txtImporteCobroPorEnvio.Enabled = this.chkCobroPorEnvio.Checked;
                    this.txtImporteParaCobroPorEnvio.Enabled = this.chkCobroPorEnvio.Checked;
                    if (Cliente.CobroPorEnvio.Valor())
                    {
                        this.txtImporteParaCobroPorEnvio.Text = Cliente.ImporteParaCobroPorEnvio.Valor().ToString();
                        this.txtImporteCobroPorEnvio.Text = Cliente.ImporteCobroPorEnvio.Valor().ToString();
                    }
                    
                    if (cliente.ClienteComisionistaID != null)
                        this.cboClienteComisionista.SelectedValue = cliente.ClienteComisionistaID;
                    else
                        this.cboClienteComisionista.Text = "";

                    //this.txtPromedioPagoTotal.Text = Util.ConvertirCadena(cliente.PromedioPagoTotal);
                    //this.txtPromedioPago3meses.Text = Util.ConvertirCadena(cliente.PromedioPagoTresMeses);

                    //this.txtDeudaActual.Text = Util.DecimalToCadenaMoneda(cliente.DeudaActual);
                    //this.txtVencido.Text = Util.DecimalToCadenaMoneda(cliente.DeudaVencido);

                    //this.txtAcumulado.Text = Util.DecimalToCadenaMoneda(cliente.Acumulado);
                    //this.txtAcumuladoH.Text = Util.DecimalToCadenaMoneda(cliente.AcumuladoH);
                    // Otros acumulados
                    var oVentas = Datos.GetListOf<VentasView>(c => c.ClienteID == clienteId && c.VentaEstatusID == Cat.VentasEstatus.Completada);
                    this.txtAcumulado.Text = (cliente.AcumuladoH.Valor() + (oVentas.Count > 0 ? oVentas.Sum(c => c.Pagado) : 0)).ToString(GlobalClass.FormatoMoneda);

                    this.cboEstado.SelectedValue = cliente.EstadoID;
                    this.cboMunicipio.SelectedValue = cliente.MunicipioID;
                    this.cboCiudad.SelectedValue = cliente.CiudadID;

                    this.cargarClientePersonal(clienteId);
                    this.cargarClienteCredito(clienteId);

                    // Se cargan los vehículos del cliente
                    this.CargarClienteVehiculos(clienteId);

                    // Si es ventas mostrador, se bloquea
                    this.gpoDatosUsuarios.Enabled = (clienteId != Cat.Clientes.Mostrador);

                    // Para las cargas de la pestaña de productos vendidos
                    this.CambiarClienteProductosVendidos(clienteId);

                    // Se carga la tab Tablero de Control
                    if (this.tabClientes.SelectedTab == this.tabControl)
                        this.CargarTableroControl(clienteId);
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarTableroControl(int clienteId)
        {
            CuadroClientes oCliCont;
            if (this.tabControl.Controls.Count <= 0)
            {
                oCliCont = new CuadroClientes() { Dock = DockStyle.Fill };
                this.tabControl.Controls.Add(oCliCont);
                oCliCont.ReacomodarSinPrincipal();
            } else {
                oCliCont = (this.tabControl.Controls[0] as CuadroClientes);
            }

            oCliCont.ClienteFijoID = clienteId;
            oCliCont.CargarDatos(clienteId);
        }

        public void cargarClientePersonal(int clienteId)
        {
            this.dgvDatosPersonal.DataSource = null;
            this.dgvDatosPersonal.DataSource = Datos.GetListOf<ClientePersonalView>(c => c.ClienteID == clienteId).ToList();
            UtilLocal.ColumnasToHeaderText(this.dgvDatosPersonal);
            Util.OcultarColumnas(this.dgvDatosPersonal, new string[] { "ClientePersonalID", "ClienteID" });
        }

        public void cargarClienteCredito(int clienteId)
        {
            // Se llenan datos de crédito
            var oClienteAd = Datos.GetEntity<ClientesCreditoView>(c => c.ClienteID == clienteId);
            this.txtPromedioPagoTotal.Text = oClienteAd.PromedioDePagoAnual.Valor().ToString(GlobalClass.FormatoEntero);
            this.txtPromedioPago3meses.Text = oClienteAd.PromedioDePago3Meses.Valor().ToString(GlobalClass.FormatoEntero);
            this.txtDeudaActual.Text = oClienteAd.Adeudo.Valor().ToString(GlobalClass.FormatoMoneda);
            this.txtVencido.Text = oClienteAd.AdeudoVencido.Valor().ToString(GlobalClass.FormatoMoneda);

            //Cargar dia y hora de cobrar credito
            this.cbDiaCobro.SelectedIndex = oClienteAd.DiaDeCobro.Valor()-1;
            //this.cbCobrarCredito.SelectedItem = "07:00";
            // this.cbCobrarCredito.SelectedItem = ((TimeSpan)oClienteAd.HoraDeCobro).ToString();
            this.dtpHoraCobro.Value = DateTime.Now.Date.Add(oClienteAd.HoraDeCobro.HasValue ? oClienteAd.HoraDeCobro.Value : new TimeSpan(0, 0, 0));

            this.txtComentario.Clear();
           
            //Desea facturar?
            this.chkSiempreFactura.Checked = Util.Logico(oClienteAd.SiempreFactura);
            this.chkSiempreTicket.Checked = oClienteAd.SiempreTicket.Valor();
            //Desea Vale
            this.chkSiempreVale.Checked = Util.Logico(oClienteAd.SiempreVale);
            //Ticket precio 1
            this.chkTicket1.Checked = Util.Logico(oClienteAd.TicketPrecio1);
            
            //
            this.dgvCredito.DataSource = null;
            this.dgvCredito.DataSource = Datos.GetListOf<ClienteCreditoView>(c => c.ClienteID == clienteId)
                .OrderByDescending(c => c.Fecha).ThenByDescending(c => c.ClienteCreditoID).ToList();
            UtilLocal.ColumnasToHeaderText(this.dgvCredito);
            Util.OcultarColumnas(this.dgvCredito, new string[] { "ClienteCreditoID", "ClienteID", "UsuarioID" });
            this.dgvCredito.Columns["Comentario"].DisplayIndex = 3;
            this.dgvCredito.Columns["Comentario"].Width = 260;
        }

        private void CargarClienteVehiculos(int iClienteId)
        {
            var oDatos = Datos.GetListOf<ClientesFlotillasView>(c => c.ClienteID == iClienteId);
            this.dgvVehiculos.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvVehiculos.AgregarFila(oReg.ClienteFlotillaID, Cat.TiposDeAfectacion.SinCambios, oReg.NumeroEconomico
                    , oReg.MarcaID, oReg.ModeloID, oReg.Anio, oReg.MotorID, oReg.VehiculoTipoID, oReg.Vin, oReg.Color, oReg.Placa, oReg.Kilometraje);
        }

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            
            // Se valida que no sea cliente mostrador
            if (this.Cliente != null && this.Cliente.ClienteID == Cat.Clientes.Mostrador && !UtilLocal.ValidarPermiso("Administracion.Clientes.EditarVentasMostrador"))
                this.cntError.PonerError(this.btnGuardar, "El Cliente de Mostrador no se puede modificar.", ErrorIconAlignment.MiddleLeft);

            if (this.txtNombreCliente.Text == "")
                this.cntError.PonerError(this.txtNombreCliente, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtPais.Text == "")
                this.cntError.PonerError(this.txtPais, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Util.Entero(this.cboEstado.SelectedValue) < 1)
                this.cntError.PonerError(this.cboEstado, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Util.Entero(this.cboCiudad.SelectedValue) < 1)
                this.cntError.PonerError(this.cboCiudad, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (Util.Entero(this.cboMunicipio.SelectedValue) < 1)
                this.cntError.PonerError(this.cboMunicipio, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtCp.Text == "")
                this.cntError.PonerError(this.txtCp, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            var item = Datos.GetEntity<Cliente>(m => m.Nombre.Equals(this.txtNombreCliente.Text));
            if (EsNuevo.Equals(true) && item != null)
            {
                Util.MensajeError("Ya existe un Cliente con ese nombre, intente con otro.", GlobalClass.NombreApp);
                return false;
            }
            else if ((EsNuevo.Equals(false) && item != null) && item.ClienteID != Cliente.ClienteID)
            {
                Util.MensajeError("Ya existe un Cliente con ese nombre, intente con otro.", GlobalClass.NombreApp);
                return false;
            }

            if (this.ModificaCredito)
            {
                /* if (!EsNuevo)
                {
                    if (Cliente.DiasDeCredito != Util.ConvertirEntero(this.txtPlazo.Text) || Cliente.LimiteCredito != Util.ConvertirDecimal(this.txtLimite.Text)
                        || Cliente.TieneCredito != this.chkTieneCredito.Checked || Cliente.Tolerancia != this.chkTolerancia.Checked)
                    {
                        this.modificoCredito = true;
                        if (this.txtComentario.Text == "")
                            this.cntError.PonerError(this.txtComentario, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
                    }
                }
                else
                {
                    if (this.ModificaCredito)
                    {
                        if (this.txtComentario.Text == "")
                            this.cntError.PonerError(this.txtComentario, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
                    }
                }
                */

                // Se verifica si hubo cambios en lo relacionado con Crédito
                if (!this.EsNuevo)
                {
                    if (Cliente.DiasDeCredito.Valor() != Util.Entero(this.txtPlazo.Text) || Cliente.LimiteCredito.Valor() != Util.Decimal(this.txtLimite.Text)
                            || Cliente.TieneCredito != this.chkTieneCredito.Checked || Cliente.Tolerancia.Valor() != this.chkTolerancia.Checked)
                    {
                        this.modificoCredito = true;
                        if (this.txtComentario.Text == "")
                            this.cntError.PonerError(this.txtComentario, "Debes poner un comentario.", ErrorIconAlignment.BottomRight);
                    }
                }
            }

            // Validaciones de Vehículos
            bool bErrorGrid = false;
            foreach (DataGridViewRow oFila in this.dgvVehiculos.Rows)
            {
                if (oFila.IsNewRow) continue;
                if (this.dgvVehiculos.ObtenerIdCambio(oFila) == Cat.TiposDeAfectacion.Borrar)
                {
                    // Se valida que no se haya usado el vehículo en otra tabla
                    int iClienteFlotillaID = this.dgvVehiculos.ObtenerId(oFila);
                    if (Datos.Exists<Venta>(c => c.ClienteVehiculoID == iClienteFlotillaID && c.Estatus))
                        oFila.ErrorText = "No se puede borrar este vehículo porque ya ha sido usado en alguna venta.";
                }
                else
                {
                    if (Util.Cadena(oFila.Cells["NumeroEconomico"].Value) == "")
                        oFila.ErrorText = "Debes especificar el número económico.";
                }

                bErrorGrid = (bErrorGrid || (oFila.ErrorText != ""));
            }
            if (bErrorGrid)
                this.cntError.PonerError(this.btnGuardar, "Existen errores de valicación en los Vehículos.", ErrorIconAlignment.MiddleLeft);

            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

        #region [ Kardex Compras ]

        private void CambiarClienteProductosVendidos(int iClienteID)
        {
            this.cmbVehiculo.CargarDatos("ClienteFlotillaID", "Etiqueta", Datos.GetListOf<ClientesFlotillasView>(c => c.ClienteID == iClienteID));
        }

        private void btnMostrarCompras_Click(object sender, EventArgs e)
        {
            if (Cliente == null)
            {
                Util.MensajeAdvertencia("Debe seleccionar a un cliente.", GlobalClass.NombreApp);
                return;
            }
            try
            {
                if (Cliente.ClienteID > 0)
                {
                    var sucursalId = (this.cboSucursal.SelectedValue == null ? null : (int?)this.cboSucursal.SelectedValue);
                    var fechaInicial = UtilLocal.InicioAbsoluto(this.dtpFechaDesde.Value);
                    var fechaFinal = UtilLocal.FinAbsoluto(this.dtpFechaHasta.Value);
                    int? iVehiculoID = (this.cmbVehiculo.SelectedValue == null ? null : (int?)this.cmbVehiculo.SelectedValue);

                    var dic = new Dictionary<string, object>();
                    dic.Add("ClienteID", Cliente.ClienteID);
                    dic.Add("SucursalID", sucursalId);
                    dic.Add("FechaInicial", fechaInicial);
                    dic.Add("FechaFinal", fechaFinal);
                    dic.Add("VehiculoID", iVehiculoID);
                    DataTable dtCompras = new DataTable();
                    dtCompras = UtilLocal.newTable<pauClienteKardex_Result>("compras", Datos.ExecuteProcedure<pauClienteKardex_Result>("pauClienteKardex", dic));

                    if (dtCompras != null)
                    {
                        this.dgvDatos.DataSource = null;
                        this.compras = new BindingSource();
                        this.compras.DataSource = dtCompras;
                        this.dgvDatos.DataSource = this.compras;

                        Util.OcultarColumnas(this.dgvDatos, new string[] { "VentaID", "Fecha", "Folio", "Sucursal", "ParteID", "PrecioUnitario", "Iva", "Importe", "EstatusActual", "Busqueda" });
                        UtilLocal.ColumnasToHeaderText(this.dgvDatos);
                        UtilLocal.FormatoDecimalColumnas(this.dgvDatos, new string[] { "Cantidad" });
                        foreach (DataGridViewColumn column in this.dgvDatos.Columns)
                        {
                            dgvDatos.AutoResizeColumn(column.Index, DataGridViewAutoSizeColumnMode.ColumnHeader);
                            column.ReadOnly = true;
                        }
                        this.dgvDatos.Columns["Descripcion"].Width = 350;
                        this.dgvDatos.Columns["Proveedor"].Width = 150;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBusqueda.Text.Length > 0)
                {
                    string Value = txtBusqueda.Text; //"M a";
                    string filter = string.Empty;
                    if (Value.Contains(" ")) //revisar si existe espacio en blanco
                    {
                        string[] Values = Value.Split(' '); //separar valores
                        filter += "(Busqueda like '%" + Values[0].Trim() + "%') AND ";
                        for (int i = 1; i < Values.Length; i++)
                        {
                            filter += "(Busqueda like '%" + Values[i].Trim() + "%') AND ";
                        }
                        filter = filter.Substring(0, filter.LastIndexOf("AND ") - 1);
                    }
                    else
                    {
                        filter = "Busqueda like '%" + Value + "%'";
                    }
                    if (compras != null)
                        compras.Filter = filter;
                }
                else
                {
                    if (compras != null)
                        compras.Filter = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDatos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDatos.VerSeleccionNueva())
            {
                if (this.dgvDatos.CurrentRow == null)
                {
                    this.dgvDetalle.DataSource = null;
                    this.dgvExistencias.DataSource = null;
                }
                else
                {
                    int iParteID = Util.Entero(this.dgvDatos.CurrentRow.Cells["ParteID"].Value);
                    this.LlenarPartesCompradasDetalle(iParteID);
                    this.LlenarExistencias(iParteID);
                }
            }
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtBusqueda.Clear();
            }
            if (e.KeyCode == Keys.Down)
            {
                this.dgvDatos.Focus();
            }
        }

        private void LlenarPartesCompradasDetalle(int parteId)
        {
            try
            {
                // var parteId = Util.ConvertirEntero(this.dgvDatos["ParteID", e.RowIndex].Value);
                if (parteId > 0 && Cliente != null)
                {
                    var fechaInicial = UtilLocal.InicioAbsoluto(this.dtpFechaDesde.Value);
                    var fechaFinal = UtilLocal.FinAbsoluto(this.dtpFechaHasta.Value);
                    int iSucursalID = Util.Entero(this.cboSucursal.SelectedValue);

                    var detalleCompras = Datos.GetListOf<ClienteKardexDetalleView>(
                        c => c.ParteID == parteId && c.ClienteID == Cliente.ClienteID && (c.Fecha >= fechaInicial && c.Fecha <= fechaFinal)
                            && (iSucursalID == 0 || c.SucursalID == iSucursalID)).ToList();

                    this.dgvDetalle.DataSource = null;
                    this.dgvDetalle.DataSource = new SortableBindingList<ClienteKardexDetalleView>(detalleCompras);
                    Util.OcultarColumnas(this.dgvDetalle, new string[] { "VentaID", "ParteID", "ClienteID", "NumeroParte", "Descripcion", "Marca", "Linea", "Proveedor", "PrecioUnitario", "Iva", "Busqueda" });
                    UtilLocal.FormatoDecimalColumnas(this.dgvDetalle, new string[] { "Importe" });
                    UtilLocal.ColumnasToHeaderText(this.dgvDetalle);

                    foreach (DataGridViewColumn column in this.dgvDetalle.Columns)
                    {
                        dgvDetalle.AutoResizeColumn(column.Index, DataGridViewAutoSizeColumnMode.ColumnHeader);
                        column.ReadOnly = true;
                    }

                    this.dgvDetalle.Columns["Fecha"].Width = 120;
                    this.dgvDetalle.Columns["Folio"].Width = 100;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();
            var oClientes = Datos.GetListOf<ClientesView>();
            var dgvClientes = new DataGridView();
            dgvClientes.Columns.Clear();
            dgvClientes.AgregarColumnaCadena("Nombre", "Nombre", 100);
            dgvClientes.Columns.Add("Rfc", "RFC");
            dgvClientes.Columns.Add("Alias", "Alias");
            dgvClientes.AgregarColumnaCadena("Calle", "Calle", 100);
            dgvClientes.AgregarColumnaCadena("NumeroExterior", "Número Exterior", 100);
            dgvClientes.AgregarColumnaCadena("NumeroInterior", "Número Interior", 100);
            dgvClientes.AgregarColumnaCadena("Colonia", "Colonia", 100);
            dgvClientes.AgregarColumnaCadena("Colonia", "Colonia", 100);
            dgvClientes.Columns.Add("CodigoPostal", "Código Postal");
            dgvClientes.Columns.Add("Municipio", "Municipio");
            dgvClientes.Columns.Add("Localidad", "Localidad");
            dgvClientes.Columns.Add("Estado", "Estado");
            dgvClientes.Columns.Add("Telefono", "Télefono Oficina");
            dgvClientes.Columns.Add("Particular", "Télefono Particular");
            dgvClientes.Columns.Add("Celular", "Celular");
            dgvClientes.Columns.Add("Nextel", "Nextel");
            dgvClientes.Columns.Add("TieneCredito", "Tiene Credito");
            dgvClientes.Columns.Add("DiasDeCredito", "Plazo");
            dgvClientes.Columns.Add("Limite", "Limite");
            dgvClientes.Columns.Add("ListaDePrecios", "Lista de Precios");
            dgvClientes.Columns.Add("Tipo", "Tipo de Cliente");
            dgvClientes.Columns.Add("esTallerMecanico", "Taller Mécanico");
            dgvClientes.Columns.Add("esTallerElectrico", "Taller Eléctrico");
            dgvClientes.Columns.Add("esTallerDiesel", "Taller Diesel");

            foreach (var cliente in oClientes)
            {
                dgvClientes.Rows.Add(cliente.Nombre, cliente.Rfc, cliente.Alias, cliente.Calle, cliente.NumeroExterior, cliente.NumeroInterior, cliente.Colonia,
                    cliente.CodigoPostal, cliente.NombreMunicipio, cliente.NombreCiudad, cliente.NombreEstado, cliente.Telefono, cliente.Particular, cliente.Celular,
                    cliente.Nextel, cliente.TieneCredito.ACadena(), cliente.DiasDeCredito,
                     cliente.LimiteCredito, cliente.ListaDePrecios, cliente.NombreTipoCliente,
                     Util.Logico(cliente.EsTallerMecanico).ACadena(), Util.Logico(cliente.EsTallerElectrico).ACadena(),
                     Util.Logico(cliente.EsTallerDiesel).ACadena());
            }
            Cargando.Cerrar();

            //Se exporta a csv
            UtilLocal.AbrirEnExcel(dgvClientes);
            
        }

        private void btnCredito_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();
            var oClientes = Datos.GetListOf<ClientesCreditoView>(c => c.TieneCredito || c.Adeudo > 0);
            var dgvClientes = new DataGridView();
            dgvClientes.Columns.Clear();
            dgvClientes.Columns.Add("Nombre", "Razón Social");
            dgvClientes.Columns.Add("DiasDeCredito", "Plazo");
            dgvClientes.Columns.Add("LimiteDeCredito", "Limite");
            dgvClientes.Columns.Add("Tolerancia", "Tolerancia");
            dgvClientes.Columns.Add("Adeudo", "Deuda Actual");
            dgvClientes.Columns.Add("AdeudoVencido", "Vencido");
            dgvClientes.Columns.Add("PromedioAnual", "Promedio Anual");
            dgvClientes.Columns.Add("Promedio3Meses", "Promedio 3 Meses");
            dgvClientes.Columns["Nombre"].ValueType = typeof(string);

            foreach (var cliente in oClientes)
            {
                dgvClientes.Rows.Add(cliente.Nombre, cliente.DiasDeCredito, cliente.LimiteDeCredito,
                    Util.Logico(cliente.Tolerancia).ACadena(), cliente.Adeudo, cliente.AdeudoVencido,
                    cliente.PromedioDePagoAnual,cliente.PromedioDePago3Meses);
            }
            Cargando.Cerrar();

            //Se exporta a csv
            UtilLocal.AbrirEnExcel(dgvClientes);
            
        }

        private void LlenarExistencias(int iParteID)
        {
            var oPartes = Datos.GetListOf<ExistenciasView>(c => c.ParteID == iParteID);
            this.dgvExistencias.DataSource = null;
            this.dgvExistencias.DataSource = oPartes;
            this.dgvExistencias.OcultarColumnas("ParteExistenciaID", "ParteID", "NumeroParte", "SucursalID", "UEmp");
            this.dgvExistencias.Columns["Exist"].Width = 80;
            this.dgvExistencias.Columns["Max"].Width = 60;
            this.dgvExistencias.Columns["Min"].Width = 60;
        }

        #endregion
        
        #region [ Calendario ]

        private void mVCobrarCredito_SelectionChanged(object sender, EventArgs e)
        {
            // CargarClientesaCalendario(mVCobrarCredito.SelectionStart, mVCobrarCredito.SelectionEnd);
            this.CalendarioEstablecerRangoMostrado(this.mVCobrarCredito.SelectionStart, this.mVCobrarCredito.SelectionEnd);
        }

        private void calendar1_DoubleClick(object sender, EventArgs e)
        {
            if (UtilLocal.ValidarPermiso("Administracion.Clientes.Calendario.Agregar", true))
            {
                if (Cliente != null)
                {
                    var frmAgregar = new AgregarEventoCalendario(Cliente, calendar1.SelectedElementStart.Date);
                    frmAgregar.ShowDialog(Principal.Instance);

                    if (frmAgregar.DialogResult == DialogResult.OK)
                    {
                        this.CargarCliente(Cliente.ClienteID);
                        this.CargarEventosCalendario();
                    }
                }
                else
                {
                    Util.MensajeError("Para agendar elija un Cliente primero.", GlobalClass.NombreApp);
                }
            }
        }

        private void calendar1_ItemSelected(object sender, CalendarItemEventArgs e)
        {
            Util.MensajeInformacion(e.Item.Text, "Informacion del evento");
        }

        private void calendar1_ItemCreating(object sender, CalendarItemCancelEventArgs e)
        {
            /* 
            e.Cancel=true;
            if (this.mVCobrarCredito.SelectionStart > new DateTime(2000, 01, 01))
            {
                CargarClientesaCalendario(this.mVCobrarCredito.SelectionStart, this.mVCobrarCredito.SelectionEnd);
            }
            else
            {
                CargarClientesaCalendario(DateTime.Today, DateTime.Today.AddDays(6));
            }
            */
        }

        private void calendar1_ItemTextEditing(object sender, CalendarItemCancelEventArgs e)
        {
            
        }

        private void calendar1_ItemDeleting(object sender, CalendarItemCancelEventArgs e)
        {
            e.Cancel=true;

        }

        private void CargarEventosCalendario()
        {
            DateTime dHoy = DateTime.Now.Date;
            var eventos = Datos.GetListOf<ClienteEventoCalendario>(c => c.Fecha >= dHoy);
            this.oEventosCalendario = new List<CalendarItem>();
            foreach (var evento in eventos)
            {
                var oClienteCV = Datos.GetEntity<ClientesCreditoView>(c => c.ClienteID == evento.ClienteID);
                var citemp = new CalendarItem(this.calendar1, evento.Fecha, evento.Fecha.AddMinutes(30)
                    , string.Format("{0} - {1}\n{2}\n{3}", oClienteCV.Nombre, oClienteCV.AdeudoVencido.Valor().ToString(GlobalClass.FormatoMoneda), evento.Fecha, evento.Evento));
                citemp.Tag = oClienteCV.AdeudoVencido;
                this.oEventosCalendario.Add(citemp);
            }
            this.CalendarioEstablecerRangoMostrado(this.mVCobrarCredito.SelectionStart, this.mVCobrarCredito.SelectionEnd);
        }

        private void CalendarioEstablecerRangoMostrado(DateTime dInicio, DateTime dFin)
        {
            if (dInicio == DateTime.MinValue || dFin == DateTime.MinValue)
            {
                dInicio = DateTime.Now.DiaPrimero();
                dFin = DateTime.Now.DiaUltimo();
            }

            dInicio = dInicio.Date.AddHours(9);
            dFin = dFin.Date.AddHours(20);
            this.calendar1.SetViewRange(dInicio, dFin);
            
            // Se vuelven a agregar los Items porque el .SetViewRange los quita
            // Se agregan los eventos
            foreach (var oReg in this.oEventosCalendario)
            {

                if (this.calendar1.ViewIntersects(oReg))
                    this.calendar1.Items.Add(oReg);
            }
            // Se agregan los totales
            var oTotales = this.oEventosCalendario.GroupBy(c => c.StartDate.Date).Select(c => new { Fecha = c.Key, Adeudo = c.Sum(s => Util.Decimal(s.Tag)) });
            foreach (var oReg in oTotales)
            {
                DateTime dFecha = oReg.Fecha.AddHours(8.5);
                if (this.calendar1.ViewIntersects(dFecha, dFecha.AddHours(0.5)))
                {
                    //this.calendar1.Items.Add(new CalendarItem(this.calendar1, dFecha, dFecha.AddHours(0.5), "TOTAL: " + oReg.Adeudo.ToString(GlobalClass.FormatoMoneda)));
                }
            }

            // Para que se muestre el calendario recorrido hasta las 20 hrs.
            if (this.calendar1.GetTimeUnit(this.calendar1.ViewStart.Date.AddHours(18)) != null)
                this.calendar1.EnsureVisible(this.calendar1.GetTimeUnit(this.calendar1.ViewStart.Date.AddHours(20.5)));
        }

        #endregion
        
        private void btnCobranza_Click(object sender, EventArgs e)
        {
            Control oVentas = Ventas.Instance;
            Principal.Instance.CargarControl(oVentas);
            oVentas.BringToFront();
            oVentas.Show();
            //if (this.oCobranza == null)
              //              this.oCobranza = new VentasCobranza(this.oVenta);
            //this.VOp.Activar(VentasOpciones.eOpcion.Cobranza);
            /*var cobranza = new VentasCobranza(Ventas.Instance);

            cobranza.Activar();*/
            Ventas.Instance.CargarCobranza();

            if (Cliente!= null) //si hay un cliente elegido, se carga ese cliente en Ventas
                Ventas.Instance.CargarCliente(Cliente.ClienteID);
            
        }

        private void btnMaps_Click(object sender, EventArgs e)
        {
            if (Cliente == null)
            {
                UtilLocal.MensajeAdvertencia("No hay ningún Cliente seleccionado.");
                return;
            }
            //ClearFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Cookies)));
            //ClearFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)));
            
            bool esDireccion;
            StringBuilder query = new StringBuilder();
            if (Cliente.LatLong == null)
            {
                query.Append(this.txtCalle + " ");
                query.Append(this.txtNumeroExt + ", ");
                query.Append(this.txtColonia + ", ");
                query.Append(this.txtCp + " ");
                query.Append(this.cboCiudad.Text + ", ");
                query.Append(this.cboEstado.Text);
                esDireccion = true;
            }
            else
            {
                query.Append(Cliente.LatLong);
                esDireccion = false;
            }
            
            var frmMapa=new Maps(query.ToString(),esDireccion);
            frmMapa.ShowDialog(Principal.Instance);
            if (frmMapa.DialogResult == DialogResult.OK)
            {
                Cliente.LatLong = frmMapa.latlong;
                Datos.Guardar<Cliente>(Cliente);
                new Notificacion("Nueva Localización guardada Exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                
            }
            frmMapa.Dispose();
            frmMapa = null;
        }
        void ClearFolder(DirectoryInfo diPath)
        {
            foreach (FileInfo fiCurrFile in diPath.GetFiles("*.txt"))
            {
                try
                {
                    fiCurrFile.Delete();
                }
                catch (Exception ex) 
                {
                    Util.MensajeError(ex.ToString(), "");
                }
            }
            foreach (DirectoryInfo diSubFolder in diPath.GetDirectories())
            {
                ClearFolder(diSubFolder); // Call recursively for all subfolders
            }
        }



    }
}