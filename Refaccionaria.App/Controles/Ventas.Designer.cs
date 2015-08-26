namespace Refaccionaria.App
{
    partial class Ventas
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader LiParteID;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ventas));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlSupIz = new System.Windows.Forms.Panel();
            this.btnActualizarClientes = new System.Windows.Forms.Button();
            this.txtVIN = new Refaccionaria.Negocio.TextoMod();
            this.btnBuscarCliente = new System.Windows.Forms.Button();
            this.cmbCliente = new System.Windows.Forms.ComboBox();
            this.lblClienteTieneCredito = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblClienteListaDePrecios = new System.Windows.Forms.Label();
            this.cmbVehiculo = new Refaccionaria.Negocio.ComboEtiqueta();
            this.cmbMotor = new Refaccionaria.Negocio.ComboEtiqueta();
            this.cmbAnio = new Refaccionaria.Negocio.ComboEtiqueta();
            this.cmbModelo = new Refaccionaria.Negocio.ComboEtiqueta();
            this.cmbMarca = new Refaccionaria.Negocio.ComboEtiqueta();
            this.btnAplicarVehiculo = new System.Windows.Forms.Button();
            this.btnAgregarCliente = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblClienteDatos = new System.Windows.Forms.Label();
            this.btnEditarCliente = new System.Windows.Forms.Button();
            this.pnlEquivalentes = new System.Windows.Forms.Panel();
            this.lblTextoEstado = new System.Windows.Forms.Label();
            this.dgvAplicaciones = new System.Windows.Forms.DataGridView();
            this.ap_ParteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ap_Modelo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ap_Anio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ap_Motor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCaracteristicas = new System.Windows.Forms.DataGridView();
            this.cr_ParteCaracteristicaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cr_Caracteristica = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cr_Valor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCodigosAlternos = new System.Windows.Forms.DataGridView();
            this.ca_ParteCodigoAlternoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ca_Marca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ca_CodigoAlterno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblPartesMarca = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblPartesExistSuc3 = new System.Windows.Forms.Label();
            this.lblPartesExistSuc2 = new System.Windows.Forms.Label();
            this.lblPartesExistSuc1 = new System.Windows.Forms.Label();
            this.lblPartesPrecio5 = new System.Windows.Forms.Label();
            this.lblPartesPrecio4 = new System.Windows.Forms.Label();
            this.lblPartesPrecio3 = new System.Windows.Forms.Label();
            this.lblPartesPrecio2 = new System.Windows.Forms.Label();
            this.lblPartesPrecio1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.chkPartesPrecios = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lsvPartesEquivalentes = new System.Windows.Forms.ListView();
            this.label30 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.pnlBusqueda = new System.Windows.Forms.Panel();
            this.chkCotizacion = new Refaccionaria.Negocio.CheckBoxMod();
            this.btnVistaDetalle = new System.Windows.Forms.Button();
            this.btnVistaIconos = new System.Windows.Forms.Button();
            this.txtDescripcion = new Refaccionaria.Negocio.TextoMod();
            this.txtCodigo = new Refaccionaria.Negocio.TextoMod();
            this.gpbBusquedaAv = new System.Windows.Forms.GroupBox();
            this.cmbMarcaParte = new Refaccionaria.Negocio.ComboEtiqueta();
            this.cmbCar01 = new Refaccionaria.Negocio.ComboEtiqueta();
            this.txtCodigoAlterno = new Refaccionaria.Negocio.TextoMod();
            this.cmbSistema = new Refaccionaria.Negocio.ComboEtiqueta();
            this.cmbLinea = new Refaccionaria.Negocio.ComboEtiqueta();
            this.chkEquivalentes = new System.Windows.Forms.CheckBox();
            this.chkBusquedaPorAplicacion = new System.Windows.Forms.CheckBox();
            this.flpCaracteristicas = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlDetalle = new System.Windows.Forms.Panel();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pnlContenidoDetalle = new System.Windows.Forms.Panel();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.pnlPartes = new System.Windows.Forms.Panel();
            this.lsvPartesComplementarias = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMensajeBusqueda = new System.Windows.Forms.Label();
            this.lsvPartes = new System.Windows.Forms.ListView();
            this.LiTexto = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LiNumeroDeParte = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LiDescripcion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LiLinea = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LiMarca = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlOpciones = new System.Windows.Forms.Panel();
            this.btnEjecutar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btn9500 = new System.Windows.Forms.Button();
            this.btnFacturarVentas = new System.Windows.Forms.Button();
            this.btnCobranza = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGarantia = new System.Windows.Forms.Button();
            this.btnReporteDeFaltante = new System.Windows.Forms.Button();
            this.btnDirectorio = new System.Windows.Forms.Button();
            this.btnReimpresion = new System.Windows.Forms.Button();
            this.btnMoto = new System.Windows.Forms.Button();
            this.btnCaja = new System.Windows.Forms.Button();
            this.btnGanancias = new System.Windows.Forms.Button();
            this.lblComisionEstimada = new System.Windows.Forms.Label();
            this.pnlContenidoEquivalentes = new System.Windows.Forms.Panel();
            this.tltBotones = new System.Windows.Forms.ToolTip(this.components);
            this.ctmStripSel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reportarFaltanteDeExistenciaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smiReportarErrorParte = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.smiPar_Kardex = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.buscarProveedorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mostrarLíneaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mostrarProveedoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlGenBuscador = new System.Windows.Forms.Panel();
            this.pnlBuscador = new System.Windows.Forms.Panel();
            this.tacCliente = new Refaccionaria.Negocio.TextoAutocompletar();
            this.tacVechiculo = new Refaccionaria.Negocio.TextoAutocompletar();
            this.pgbMetas = new System.Windows.Forms.ProgressBar();
            this.lblDescripcionOpcion = new System.Windows.Forms.Label();
            this.pcbMetasRegla = new System.Windows.Forms.PictureBox();
            this.pnlBarraVentas = new System.Windows.Forms.Panel();
            LiParteID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlSupIz.SuspendLayout();
            this.pnlEquivalentes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAplicaciones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCaracteristicas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodigosAlternos)).BeginInit();
            this.panel4.SuspendLayout();
            this.pnlBusqueda.SuspendLayout();
            this.gpbBusquedaAv.SuspendLayout();
            this.pnlDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.panel6.SuspendLayout();
            this.pnlContenidoDetalle.SuspendLayout();
            this.pnlContenido.SuspendLayout();
            this.pnlPartes.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlOpciones.SuspendLayout();
            this.pnlContenidoEquivalentes.SuspendLayout();
            this.ctmStripSel.SuspendLayout();
            this.pnlGenBuscador.SuspendLayout();
            this.pnlBuscador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbMetasRegla)).BeginInit();
            this.pnlBarraVentas.SuspendLayout();
            this.SuspendLayout();
            // 
            // LiParteID
            // 
            LiParteID.Text = "ParteID";
            LiParteID.Width = 0;
            // 
            // pnlSupIz
            // 
            this.pnlSupIz.BackColor = System.Drawing.Color.White;
            this.pnlSupIz.Controls.Add(this.btnActualizarClientes);
            this.pnlSupIz.Controls.Add(this.txtVIN);
            this.pnlSupIz.Controls.Add(this.btnBuscarCliente);
            this.pnlSupIz.Controls.Add(this.cmbCliente);
            this.pnlSupIz.Controls.Add(this.lblClienteTieneCredito);
            this.pnlSupIz.Controls.Add(this.label3);
            this.pnlSupIz.Controls.Add(this.lblClienteListaDePrecios);
            this.pnlSupIz.Controls.Add(this.cmbVehiculo);
            this.pnlSupIz.Controls.Add(this.cmbMotor);
            this.pnlSupIz.Controls.Add(this.cmbAnio);
            this.pnlSupIz.Controls.Add(this.cmbModelo);
            this.pnlSupIz.Controls.Add(this.cmbMarca);
            this.pnlSupIz.Controls.Add(this.btnAplicarVehiculo);
            this.pnlSupIz.Controls.Add(this.btnAgregarCliente);
            this.pnlSupIz.Controls.Add(this.label8);
            this.pnlSupIz.Controls.Add(this.label7);
            this.pnlSupIz.Controls.Add(this.lblClienteDatos);
            this.pnlSupIz.Controls.Add(this.btnEditarCliente);
            this.pnlSupIz.Location = new System.Drawing.Point(3, 3);
            this.pnlSupIz.Name = "pnlSupIz";
            this.pnlSupIz.Size = new System.Drawing.Size(339, 185);
            this.pnlSupIz.TabIndex = 0;
            // 
            // btnActualizarClientes
            // 
            this.btnActualizarClientes.BackColor = System.Drawing.Color.Transparent;
            this.btnActualizarClientes.BackgroundImage = global::Refaccionaria.App.Properties.Resources.BlueRefresh;
            this.btnActualizarClientes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnActualizarClientes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizarClientes.ForeColor = System.Drawing.Color.White;
            this.btnActualizarClientes.Location = new System.Drawing.Point(263, 7);
            this.btnActualizarClientes.Name = "btnActualizarClientes";
            this.btnActualizarClientes.Size = new System.Drawing.Size(18, 18);
            this.btnActualizarClientes.TabIndex = 9;
            this.tltBotones.SetToolTip(this.btnActualizarClientes, "Actualizar datos de los clientes");
            this.btnActualizarClientes.UseVisualStyleBackColor = false;
            this.btnActualizarClientes.Click += new System.EventHandler(this.btnActualizarClientes_Click);
            // 
            // txtVIN
            // 
            this.txtVIN.Etiqueta = "VIN";
            this.txtVIN.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtVIN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVIN.Location = new System.Drawing.Point(215, 109);
            this.txtVIN.Name = "txtVIN";
            this.txtVIN.PasarEnfoqueConEnter = true;
            this.txtVIN.SeleccionarTextoAlEnfoque = false;
            this.txtVIN.Size = new System.Drawing.Size(121, 21);
            this.txtVIN.TabIndex = 3;
            this.txtVIN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVIN_KeyDown);
            // 
            // btnBuscarCliente
            // 
            this.btnBuscarCliente.BackColor = System.Drawing.Color.Transparent;
            this.btnBuscarCliente.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBuscarCliente.BackgroundImage")));
            this.btnBuscarCliente.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBuscarCliente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarCliente.ForeColor = System.Drawing.Color.White;
            this.btnBuscarCliente.Location = new System.Drawing.Point(314, 31);
            this.btnBuscarCliente.Name = "btnBuscarCliente";
            this.btnBuscarCliente.Size = new System.Drawing.Size(18, 18);
            this.btnBuscarCliente.TabIndex = 0;
            this.tltBotones.SetToolTip(this.btnBuscarCliente, "Busqueda avanzada de clientes");
            this.btnBuscarCliente.UseVisualStyleBackColor = false;
            this.btnBuscarCliente.Visible = false;
            this.btnBuscarCliente.Click += new System.EventHandler(this.btnBuscarCliente_Click);
            // 
            // cmbCliente
            // 
            this.cmbCliente.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCliente.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCliente.Location = new System.Drawing.Point(3, 6);
            this.cmbCliente.Name = "cmbCliente";
            this.cmbCliente.Size = new System.Drawing.Size(254, 21);
            this.cmbCliente.TabIndex = 0;
            this.cmbCliente.Visible = false;
            this.cmbCliente.SelectedIndexChanged += new System.EventHandler(this.cmbCliente_SelectedIndexChanged);
            // 
            // lblClienteTieneCredito
            // 
            this.lblClienteTieneCredito.AutoSize = true;
            this.lblClienteTieneCredito.BackColor = System.Drawing.Color.Transparent;
            this.lblClienteTieneCredito.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClienteTieneCredito.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblClienteTieneCredito.Location = new System.Drawing.Point(224, 73);
            this.lblClienteTieneCredito.Name = "lblClienteTieneCredito";
            this.lblClienteTieneCredito.Size = new System.Drawing.Size(112, 13);
            this.lblClienteTieneCredito.TabIndex = 8;
            this.lblClienteTieneCredito.Text = "SÍ (SUSPENDIDO)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Location = new System.Drawing.Point(109, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "N. Crédito";
            // 
            // lblClienteListaDePrecios
            // 
            this.lblClienteListaDePrecios.AutoSize = true;
            this.lblClienteListaDePrecios.BackColor = System.Drawing.Color.Transparent;
            this.lblClienteListaDePrecios.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClienteListaDePrecios.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblClienteListaDePrecios.Location = new System.Drawing.Point(35, 73);
            this.lblClienteListaDePrecios.Name = "lblClienteListaDePrecios";
            this.lblClienteListaDePrecios.Size = new System.Drawing.Size(14, 13);
            this.lblClienteListaDePrecios.TabIndex = 6;
            this.lblClienteListaDePrecios.Text = "0";
            // 
            // cmbVehiculo
            // 
            this.cmbVehiculo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbVehiculo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbVehiculo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbVehiculo.DataSource = null;
            this.cmbVehiculo.DisplayMember = "";
            this.cmbVehiculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbVehiculo.Etiqueta = "Vehículo";
            this.cmbVehiculo.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbVehiculo.Location = new System.Drawing.Point(3, 109);
            this.cmbVehiculo.Name = "cmbVehiculo";
            this.cmbVehiculo.SelectedIndex = -1;
            this.cmbVehiculo.SelectedItem = null;
            this.cmbVehiculo.SelectedText = "";
            this.cmbVehiculo.SelectedValue = null;
            this.cmbVehiculo.Size = new System.Drawing.Size(206, 21);
            this.cmbVehiculo.TabIndex = 4;
            this.cmbVehiculo.ValueMember = "";
            this.cmbVehiculo.Visible = false;
            this.cmbVehiculo.SelectedIndexChanged += new System.EventHandler(this.cmbVehiculo_SelectedIndexChanged);
            this.cmbVehiculo.TextChanged += new System.EventHandler(this.cmbVehiculo_TextChanged);
            // 
            // cmbMotor
            // 
            this.cmbMotor.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbMotor.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbMotor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbMotor.DataSource = null;
            this.cmbMotor.DisplayMember = "";
            this.cmbMotor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbMotor.Etiqueta = "Motor";
            this.cmbMotor.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbMotor.Location = new System.Drawing.Point(169, 158);
            this.cmbMotor.Name = "cmbMotor";
            this.cmbMotor.SelectedIndex = -1;
            this.cmbMotor.SelectedItem = null;
            this.cmbMotor.SelectedText = "";
            this.cmbMotor.SelectedValue = null;
            this.cmbMotor.Size = new System.Drawing.Size(101, 21);
            this.cmbMotor.TabIndex = 7;
            this.cmbMotor.ValueMember = "";
            this.cmbMotor.SelectedIndexChanged += new System.EventHandler(this.cmbMotor_SelectedIndexChanged);
            this.cmbMotor.TextChanged += new System.EventHandler(this.cmbMotor_TextChanged);
            // 
            // cmbAnio
            // 
            this.cmbAnio.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAnio.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAnio.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbAnio.DataSource = null;
            this.cmbAnio.DisplayMember = "";
            this.cmbAnio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbAnio.Etiqueta = "Año";
            this.cmbAnio.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbAnio.Location = new System.Drawing.Point(3, 159);
            this.cmbAnio.Name = "cmbAnio";
            this.cmbAnio.SelectedIndex = -1;
            this.cmbAnio.SelectedItem = null;
            this.cmbAnio.SelectedText = "";
            this.cmbAnio.SelectedValue = null;
            this.cmbAnio.Size = new System.Drawing.Size(160, 21);
            this.cmbAnio.TabIndex = 6;
            this.cmbAnio.ValueMember = "";
            this.cmbAnio.SelectedIndexChanged += new System.EventHandler(this.cmbAnio_SelectedIndexChanged);
            this.cmbAnio.TextChanged += new System.EventHandler(this.cmbAnio_TextChanged);
            // 
            // cmbModelo
            // 
            this.cmbModelo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbModelo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbModelo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbModelo.DataSource = null;
            this.cmbModelo.DisplayMember = "";
            this.cmbModelo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbModelo.Etiqueta = "Modelo";
            this.cmbModelo.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbModelo.Location = new System.Drawing.Point(169, 134);
            this.cmbModelo.Name = "cmbModelo";
            this.cmbModelo.SelectedIndex = -1;
            this.cmbModelo.SelectedItem = null;
            this.cmbModelo.SelectedText = "";
            this.cmbModelo.SelectedValue = null;
            this.cmbModelo.Size = new System.Drawing.Size(167, 21);
            this.cmbModelo.TabIndex = 5;
            this.cmbModelo.ValueMember = "";
            this.cmbModelo.SelectedIndexChanged += new System.EventHandler(this.cmbModelo_SelectedIndexChanged);
            // 
            // cmbMarca
            // 
            this.cmbMarca.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbMarca.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbMarca.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbMarca.DataSource = null;
            this.cmbMarca.DisplayMember = "";
            this.cmbMarca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbMarca.Etiqueta = "Marca";
            this.cmbMarca.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbMarca.Location = new System.Drawing.Point(3, 134);
            this.cmbMarca.Name = "cmbMarca";
            this.cmbMarca.SelectedIndex = -1;
            this.cmbMarca.SelectedItem = null;
            this.cmbMarca.SelectedText = "";
            this.cmbMarca.SelectedValue = null;
            this.cmbMarca.Size = new System.Drawing.Size(160, 21);
            this.cmbMarca.TabIndex = 4;
            this.cmbMarca.ValueMember = "";
            this.cmbMarca.SelectedIndexChanged += new System.EventHandler(this.cmbMarca_SelectedIndexChanged);
            // 
            // btnAplicarVehiculo
            // 
            this.btnAplicarVehiculo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnAplicarVehiculo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicarVehiculo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAplicarVehiculo.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnAplicarVehiculo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAplicarVehiculo.Location = new System.Drawing.Point(276, 159);
            this.btnAplicarVehiculo.Margin = new System.Windows.Forms.Padding(0);
            this.btnAplicarVehiculo.Name = "btnAplicarVehiculo";
            this.btnAplicarVehiculo.Size = new System.Drawing.Size(60, 22);
            this.btnAplicarVehiculo.TabIndex = 8;
            this.btnAplicarVehiculo.Text = "Aplicar";
            this.btnAplicarVehiculo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tltBotones.SetToolTip(this.btnAplicarVehiculo, "Registrar aplicación con el Vehículo seleccionado.");
            this.btnAplicarVehiculo.UseVisualStyleBackColor = false;
            this.btnAplicarVehiculo.Click += new System.EventHandler(this.btnAplicarVehiculo_Click);
            // 
            // btnAgregarCliente
            // 
            this.btnAgregarCliente.BackgroundImage = global::Refaccionaria.App.Properties.Resources.BlueAdd;
            this.btnAgregarCliente.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAgregarCliente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregarCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarCliente.ForeColor = System.Drawing.Color.White;
            this.btnAgregarCliente.Location = new System.Drawing.Point(287, 7);
            this.btnAgregarCliente.Name = "btnAgregarCliente";
            this.btnAgregarCliente.Size = new System.Drawing.Size(18, 18);
            this.btnAgregarCliente.TabIndex = 1;
            this.tltBotones.SetToolTip(this.btnAgregarCliente, "Agregar un cliente");
            this.btnAgregarCliente.UseVisualStyleBackColor = true;
            this.btnAgregarCliente.Click += new System.EventHandler(this.btnAgregarCliente_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.SteelBlue;
            this.label8.Location = new System.Drawing.Point(169, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "CRÉDITO:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.ForeColor = System.Drawing.Color.SteelBlue;
            this.label7.Location = new System.Drawing.Point(3, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "L.P.";
            // 
            // lblClienteDatos
            // 
            this.lblClienteDatos.AllowDrop = true;
            this.lblClienteDatos.BackColor = System.Drawing.Color.Transparent;
            this.lblClienteDatos.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblClienteDatos.Location = new System.Drawing.Point(3, 30);
            this.lblClienteDatos.Name = "lblClienteDatos";
            this.lblClienteDatos.Size = new System.Drawing.Size(266, 41);
            this.lblClienteDatos.TabIndex = 0;
            this.lblClienteDatos.Text = "I. Comonfort No. 251, Zapotlán el Grande, Cd. Guzmán";
            // 
            // btnEditarCliente
            // 
            this.btnEditarCliente.BackColor = System.Drawing.Color.Transparent;
            this.btnEditarCliente.BackgroundImage = global::Refaccionaria.App.Properties.Resources.BlueEdit;
            this.btnEditarCliente.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEditarCliente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditarCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditarCliente.ForeColor = System.Drawing.Color.White;
            this.btnEditarCliente.Location = new System.Drawing.Point(314, 7);
            this.btnEditarCliente.Name = "btnEditarCliente";
            this.btnEditarCliente.Size = new System.Drawing.Size(18, 18);
            this.btnEditarCliente.TabIndex = 2;
            this.tltBotones.SetToolTip(this.btnEditarCliente, "Editar los datos del cliente seleccionado");
            this.btnEditarCliente.UseVisualStyleBackColor = false;
            this.btnEditarCliente.Click += new System.EventHandler(this.btnEditarCliente_Click);
            // 
            // pnlEquivalentes
            // 
            this.pnlEquivalentes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlEquivalentes.BackColor = System.Drawing.Color.Linen;
            this.pnlEquivalentes.Controls.Add(this.lblTextoEstado);
            this.pnlEquivalentes.Controls.Add(this.dgvAplicaciones);
            this.pnlEquivalentes.Controls.Add(this.dgvCaracteristicas);
            this.pnlEquivalentes.Controls.Add(this.dgvCodigosAlternos);
            this.pnlEquivalentes.Controls.Add(this.panel4);
            this.pnlEquivalentes.Controls.Add(this.lsvPartesEquivalentes);
            this.pnlEquivalentes.Controls.Add(this.label30);
            this.pnlEquivalentes.Location = new System.Drawing.Point(0, 0);
            this.pnlEquivalentes.Name = "pnlEquivalentes";
            this.pnlEquivalentes.Size = new System.Drawing.Size(998, 140);
            this.pnlEquivalentes.TabIndex = 7;
            // 
            // lblTextoEstado
            // 
            this.lblTextoEstado.AllowDrop = true;
            this.lblTextoEstado.AutoSize = true;
            this.lblTextoEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTextoEstado.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTextoEstado.Location = new System.Drawing.Point(-13, 126);
            this.lblTextoEstado.Name = "lblTextoEstado";
            this.lblTextoEstado.Size = new System.Drawing.Size(164, 12);
            this.lblTextoEstado.TabIndex = 14;
            this.lblTextoEstado.Text = "1 = Ticket    2 = Factura    3 = Cotización";
            this.lblTextoEstado.Visible = false;
            // 
            // dgvAplicaciones
            // 
            this.dgvAplicaciones.AllowUserToAddRows = false;
            this.dgvAplicaciones.AllowUserToDeleteRows = false;
            this.dgvAplicaciones.AllowUserToResizeRows = false;
            this.dgvAplicaciones.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.dgvAplicaciones.BackgroundColor = System.Drawing.Color.Linen;
            this.dgvAplicaciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAplicaciones.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAplicaciones.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAplicaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAplicaciones.ColumnHeadersVisible = false;
            this.dgvAplicaciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ap_ParteID,
            this.ap_Modelo,
            this.ap_Anio,
            this.ap_Motor});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Linen;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Linen;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAplicaciones.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAplicaciones.Location = new System.Drawing.Point(588, 3);
            this.dgvAplicaciones.Name = "dgvAplicaciones";
            this.dgvAplicaciones.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAplicaciones.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvAplicaciones.RowHeadersVisible = false;
            this.dgvAplicaciones.RowTemplate.Height = 14;
            this.dgvAplicaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAplicaciones.Size = new System.Drawing.Size(156, 135);
            this.dgvAplicaciones.TabIndex = 4;
            // 
            // ap_ParteID
            // 
            this.ap_ParteID.HeaderText = "ParteID";
            this.ap_ParteID.Name = "ap_ParteID";
            this.ap_ParteID.ReadOnly = true;
            this.ap_ParteID.Visible = false;
            // 
            // ap_Modelo
            // 
            this.ap_Modelo.HeaderText = "Modelo";
            this.ap_Modelo.Name = "ap_Modelo";
            this.ap_Modelo.ReadOnly = true;
            this.ap_Modelo.Width = 75;
            // 
            // ap_Anio
            // 
            dataGridViewCellStyle2.NullValue = null;
            this.ap_Anio.DefaultCellStyle = dataGridViewCellStyle2;
            this.ap_Anio.HeaderText = "Año";
            this.ap_Anio.Name = "ap_Anio";
            this.ap_Anio.ReadOnly = true;
            this.ap_Anio.Width = 20;
            // 
            // ap_Motor
            // 
            this.ap_Motor.HeaderText = "Motor";
            this.ap_Motor.Name = "ap_Motor";
            this.ap_Motor.ReadOnly = true;
            this.ap_Motor.Width = 43;
            // 
            // dgvCaracteristicas
            // 
            this.dgvCaracteristicas.AllowUserToAddRows = false;
            this.dgvCaracteristicas.AllowUserToDeleteRows = false;
            this.dgvCaracteristicas.AllowUserToResizeRows = false;
            this.dgvCaracteristicas.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.dgvCaracteristicas.BackgroundColor = System.Drawing.Color.Linen;
            this.dgvCaracteristicas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCaracteristicas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCaracteristicas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCaracteristicas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCaracteristicas.ColumnHeadersVisible = false;
            this.dgvCaracteristicas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cr_ParteCaracteristicaID,
            this.cr_Caracteristica,
            this.cr_Valor});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.Linen;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Linen;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.MediumPurple;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCaracteristicas.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvCaracteristicas.Location = new System.Drawing.Point(745, 65);
            this.dgvCaracteristicas.Name = "dgvCaracteristicas";
            this.dgvCaracteristicas.ReadOnly = true;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCaracteristicas.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvCaracteristicas.RowHeadersVisible = false;
            this.dgvCaracteristicas.RowTemplate.Height = 14;
            this.dgvCaracteristicas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCaracteristicas.Size = new System.Drawing.Size(155, 73);
            this.dgvCaracteristicas.TabIndex = 1;
            // 
            // cr_ParteCaracteristicaID
            // 
            this.cr_ParteCaracteristicaID.HeaderText = "ParteCaracteristicaID";
            this.cr_ParteCaracteristicaID.Name = "cr_ParteCaracteristicaID";
            this.cr_ParteCaracteristicaID.ReadOnly = true;
            this.cr_ParteCaracteristicaID.Visible = false;
            // 
            // cr_Caracteristica
            // 
            this.cr_Caracteristica.HeaderText = "Car.";
            this.cr_Caracteristica.Name = "cr_Caracteristica";
            this.cr_Caracteristica.ReadOnly = true;
            this.cr_Caracteristica.Width = 55;
            // 
            // cr_Valor
            // 
            this.cr_Valor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cr_Valor.HeaderText = "Valor";
            this.cr_Valor.Name = "cr_Valor";
            this.cr_Valor.ReadOnly = true;
            this.cr_Valor.Width = 80;
            // 
            // dgvCodigosAlternos
            // 
            this.dgvCodigosAlternos.AllowUserToAddRows = false;
            this.dgvCodigosAlternos.AllowUserToDeleteRows = false;
            this.dgvCodigosAlternos.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.dgvCodigosAlternos.BackgroundColor = System.Drawing.Color.Linen;
            this.dgvCodigosAlternos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCodigosAlternos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCodigosAlternos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvCodigosAlternos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCodigosAlternos.ColumnHeadersVisible = false;
            this.dgvCodigosAlternos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ca_ParteCodigoAlternoID,
            this.ca_Marca,
            this.ca_CodigoAlterno});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.Linen;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Green;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.Linen;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Green;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCodigosAlternos.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvCodigosAlternos.Location = new System.Drawing.Point(745, 3);
            this.dgvCodigosAlternos.Name = "dgvCodigosAlternos";
            this.dgvCodigosAlternos.ReadOnly = true;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCodigosAlternos.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvCodigosAlternos.RowHeadersVisible = false;
            this.dgvCodigosAlternos.RowTemplate.Height = 14;
            this.dgvCodigosAlternos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCodigosAlternos.Size = new System.Drawing.Size(155, 60);
            this.dgvCodigosAlternos.TabIndex = 2;
            // 
            // ca_ParteCodigoAlternoID
            // 
            this.ca_ParteCodigoAlternoID.HeaderText = "ParteCodigoAlternoID";
            this.ca_ParteCodigoAlternoID.Name = "ca_ParteCodigoAlternoID";
            this.ca_ParteCodigoAlternoID.ReadOnly = true;
            this.ca_ParteCodigoAlternoID.Visible = false;
            // 
            // ca_Marca
            // 
            this.ca_Marca.HeaderText = "Marca";
            this.ca_Marca.Name = "ca_Marca";
            this.ca_Marca.ReadOnly = true;
            this.ca_Marca.Width = 55;
            // 
            // ca_CodigoAlterno
            // 
            this.ca_CodigoAlterno.HeaderText = "Código";
            this.ca_CodigoAlterno.Name = "ca_CodigoAlterno";
            this.ca_CodigoAlterno.ReadOnly = true;
            this.ca_CodigoAlterno.Width = 80;
            // 
            // panel4
            // 
            this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.panel4.Controls.Add(this.lblPartesMarca);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.label12);
            this.panel4.Controls.Add(this.lblPartesExistSuc3);
            this.panel4.Controls.Add(this.lblPartesExistSuc2);
            this.panel4.Controls.Add(this.lblPartesExistSuc1);
            this.panel4.Controls.Add(this.lblPartesPrecio5);
            this.panel4.Controls.Add(this.lblPartesPrecio4);
            this.panel4.Controls.Add(this.lblPartesPrecio3);
            this.panel4.Controls.Add(this.lblPartesPrecio2);
            this.panel4.Controls.Add(this.lblPartesPrecio1);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.chkPartesPrecios);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Location = new System.Drawing.Point(901, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(94, 139);
            this.panel4.TabIndex = 3;
            // 
            // lblPartesMarca
            // 
            this.lblPartesMarca.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPartesMarca.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPartesMarca.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesMarca.Location = new System.Drawing.Point(2, 125);
            this.lblPartesMarca.Name = "lblPartesMarca";
            this.lblPartesMarca.Size = new System.Drawing.Size(73, 12);
            this.lblPartesMarca.TabIndex = 15;
            this.lblPartesMarca.Text = "Marca";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.SteelBlue;
            this.label10.Location = new System.Drawing.Point(2, 110);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "SUC03";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.SteelBlue;
            this.label11.Location = new System.Drawing.Point(2, 97);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "SUC02";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.SteelBlue;
            this.label12.Location = new System.Drawing.Point(2, 84);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "MATRIZ";
            // 
            // lblPartesExistSuc3
            // 
            this.lblPartesExistSuc3.AutoSize = true;
            this.lblPartesExistSuc3.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesExistSuc3.Location = new System.Drawing.Point(56, 110);
            this.lblPartesExistSuc3.Name = "lblPartesExistSuc3";
            this.lblPartesExistSuc3.Size = new System.Drawing.Size(13, 13);
            this.lblPartesExistSuc3.TabIndex = 9;
            this.lblPartesExistSuc3.Text = "0";
            // 
            // lblPartesExistSuc2
            // 
            this.lblPartesExistSuc2.AutoSize = true;
            this.lblPartesExistSuc2.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesExistSuc2.Location = new System.Drawing.Point(56, 97);
            this.lblPartesExistSuc2.Name = "lblPartesExistSuc2";
            this.lblPartesExistSuc2.Size = new System.Drawing.Size(13, 13);
            this.lblPartesExistSuc2.TabIndex = 8;
            this.lblPartesExistSuc2.Text = "0";
            // 
            // lblPartesExistSuc1
            // 
            this.lblPartesExistSuc1.AutoSize = true;
            this.lblPartesExistSuc1.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesExistSuc1.Location = new System.Drawing.Point(56, 84);
            this.lblPartesExistSuc1.Name = "lblPartesExistSuc1";
            this.lblPartesExistSuc1.Size = new System.Drawing.Size(13, 13);
            this.lblPartesExistSuc1.TabIndex = 7;
            this.lblPartesExistSuc1.Text = "0";
            // 
            // lblPartesPrecio5
            // 
            this.lblPartesPrecio5.AutoSize = true;
            this.lblPartesPrecio5.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesPrecio5.Location = new System.Drawing.Point(2, 68);
            this.lblPartesPrecio5.Name = "lblPartesPrecio5";
            this.lblPartesPrecio5.Size = new System.Drawing.Size(20, 13);
            this.lblPartesPrecio5.TabIndex = 6;
            this.lblPartesPrecio5.Text = "P5";
            // 
            // lblPartesPrecio4
            // 
            this.lblPartesPrecio4.AutoSize = true;
            this.lblPartesPrecio4.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesPrecio4.Location = new System.Drawing.Point(2, 55);
            this.lblPartesPrecio4.Name = "lblPartesPrecio4";
            this.lblPartesPrecio4.Size = new System.Drawing.Size(20, 13);
            this.lblPartesPrecio4.TabIndex = 5;
            this.lblPartesPrecio4.Text = "P4";
            // 
            // lblPartesPrecio3
            // 
            this.lblPartesPrecio3.AutoSize = true;
            this.lblPartesPrecio3.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesPrecio3.Location = new System.Drawing.Point(2, 42);
            this.lblPartesPrecio3.Name = "lblPartesPrecio3";
            this.lblPartesPrecio3.Size = new System.Drawing.Size(20, 13);
            this.lblPartesPrecio3.TabIndex = 4;
            this.lblPartesPrecio3.Text = "P3";
            // 
            // lblPartesPrecio2
            // 
            this.lblPartesPrecio2.AutoSize = true;
            this.lblPartesPrecio2.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesPrecio2.Location = new System.Drawing.Point(2, 29);
            this.lblPartesPrecio2.Name = "lblPartesPrecio2";
            this.lblPartesPrecio2.Size = new System.Drawing.Size(20, 13);
            this.lblPartesPrecio2.TabIndex = 3;
            this.lblPartesPrecio2.Text = "P2";
            // 
            // lblPartesPrecio1
            // 
            this.lblPartesPrecio1.AutoSize = true;
            this.lblPartesPrecio1.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblPartesPrecio1.Location = new System.Drawing.Point(2, 16);
            this.lblPartesPrecio1.Name = "lblPartesPrecio1";
            this.lblPartesPrecio1.Size = new System.Drawing.Size(20, 13);
            this.lblPartesPrecio1.TabIndex = 2;
            this.lblPartesPrecio1.Text = "P1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.SteelBlue;
            this.label6.Location = new System.Drawing.Point(76, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Existencias";
            this.label6.Visible = false;
            // 
            // chkPartesPrecios
            // 
            this.chkPartesPrecios.AutoSize = true;
            this.chkPartesPrecios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkPartesPrecios.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPartesPrecios.ForeColor = System.Drawing.Color.SteelBlue;
            this.chkPartesPrecios.Location = new System.Drawing.Point(5, 1);
            this.chkPartesPrecios.Name = "chkPartesPrecios";
            this.chkPartesPrecios.Size = new System.Drawing.Size(65, 17);
            this.chkPartesPrecios.TabIndex = 0;
            this.chkPartesPrecios.Text = "Precios";
            this.chkPartesPrecios.UseVisualStyleBackColor = true;
            this.chkPartesPrecios.CheckedChanged += new System.EventHandler(this.chkPartesPrecios_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(88, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Marca";
            this.label9.Visible = false;
            // 
            // lsvPartesEquivalentes
            // 
            this.lsvPartesEquivalentes.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.lsvPartesEquivalentes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvPartesEquivalentes.BackColor = System.Drawing.Color.Linen;
            this.lsvPartesEquivalentes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsvPartesEquivalentes.Font = new System.Drawing.Font("Arial Narrow", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsvPartesEquivalentes.ForeColor = System.Drawing.Color.SteelBlue;
            this.lsvPartesEquivalentes.Location = new System.Drawing.Point(18, 2);
            this.lsvPartesEquivalentes.MultiSelect = false;
            this.lsvPartesEquivalentes.Name = "lsvPartesEquivalentes";
            this.lsvPartesEquivalentes.OwnerDraw = true;
            this.lsvPartesEquivalentes.Size = new System.Drawing.Size(570, 135);
            this.lsvPartesEquivalentes.TabIndex = 0;
            this.lsvPartesEquivalentes.UseCompatibleStateImageBehavior = false;
            this.lsvPartesEquivalentes.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lsvPartesEquivalentes_DrawItem);
            this.lsvPartesEquivalentes.SelectedIndexChanged += new System.EventHandler(this.lsvPartesEquivalentes_SelectedIndexChanged);
            this.lsvPartesEquivalentes.DoubleClick += new System.EventHandler(this.lsvPartesEquivalentes_DoubleClick);
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.Color.Linen;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label30.Image = global::Refaccionaria.App.Properties.Resources.Equivalentes;
            this.label30.Location = new System.Drawing.Point(3, 9);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(14, 112);
            this.label30.TabIndex = 0;
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblTotal.Location = new System.Drawing.Point(245, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(91, 24);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "$0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTotal.DoubleClick += new System.EventHandler(this.lblTotal_DoubleClick);
            // 
            // label35
            // 
            this.label35.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ForeColor = System.Drawing.Color.SteelBlue;
            this.label35.Location = new System.Drawing.Point(188, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(51, 24);
            this.label35.TabIndex = 0;
            this.label35.Text = "Total";
            // 
            // pnlBusqueda
            // 
            this.pnlBusqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBusqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(188)))), ((int)(((byte)(216)))));
            this.pnlBusqueda.Controls.Add(this.chkCotizacion);
            this.pnlBusqueda.Controls.Add(this.btnVistaDetalle);
            this.pnlBusqueda.Controls.Add(this.btnVistaIconos);
            this.pnlBusqueda.Controls.Add(this.txtDescripcion);
            this.pnlBusqueda.Controls.Add(this.txtCodigo);
            this.pnlBusqueda.Location = new System.Drawing.Point(0, 0);
            this.pnlBusqueda.Name = "pnlBusqueda";
            this.pnlBusqueda.Size = new System.Drawing.Size(998, 34);
            this.pnlBusqueda.TabIndex = 1;
            // 
            // chkCotizacion
            // 
            this.chkCotizacion.AutoSize = true;
            this.chkCotizacion.BackColor = System.Drawing.Color.White;
            this.chkCotizacion.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.chkCotizacion.FlatAppearance.BorderSize = 0;
            this.chkCotizacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkCotizacion.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.chkCotizacion.Location = new System.Drawing.Point(121, 13);
            this.chkCotizacion.Name = "chkCotizacion";
            this.chkCotizacion.Size = new System.Drawing.Size(12, 11);
            this.chkCotizacion.TabIndex = 4;
            this.tltBotones.SetToolTip(this.chkCotizacion, "Indica si se quiere hacer sólo una cotización.");
            this.chkCotizacion.UseVisualStyleBackColor = false;
            this.chkCotizacion.CheckedChanged += new System.EventHandler(this.chkCotizacion_CheckedChanged);
            // 
            // btnVistaDetalle
            // 
            this.btnVistaDetalle.FlatAppearance.BorderSize = 0;
            this.btnVistaDetalle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVistaDetalle.Image = global::Refaccionaria.App.Properties.Resources.VistaDetalle;
            this.btnVistaDetalle.Location = new System.Drawing.Point(633, 6);
            this.btnVistaDetalle.Name = "btnVistaDetalle";
            this.btnVistaDetalle.Size = new System.Drawing.Size(21, 21);
            this.btnVistaDetalle.TabIndex = 3;
            this.tltBotones.SetToolTip(this.btnVistaDetalle, "Ver resultados a manera de detalle.");
            this.btnVistaDetalle.UseVisualStyleBackColor = true;
            this.btnVistaDetalle.Click += new System.EventHandler(this.btnVistaDetalle_Click);
            // 
            // btnVistaIconos
            // 
            this.btnVistaIconos.FlatAppearance.BorderSize = 0;
            this.btnVistaIconos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVistaIconos.Image = global::Refaccionaria.App.Properties.Resources.VistaIconos;
            this.btnVistaIconos.Location = new System.Drawing.Point(610, 6);
            this.btnVistaIconos.Name = "btnVistaIconos";
            this.btnVistaIconos.Size = new System.Drawing.Size(21, 21);
            this.btnVistaIconos.TabIndex = 2;
            this.tltBotones.SetToolTip(this.btnVistaIconos, "Ver resultados con imágenes.");
            this.btnVistaIconos.UseVisualStyleBackColor = true;
            this.btnVistaIconos.Click += new System.EventHandler(this.btnVistaIconos_Click);
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Etiqueta = "Descripción";
            this.txtDescripcion.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtDescripcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescripcion.Location = new System.Drawing.Point(142, 7);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.PasarEnfoqueConEnter = true;
            this.txtDescripcion.SeleccionarTextoAlEnfoque = false;
            this.txtDescripcion.Size = new System.Drawing.Size(462, 21);
            this.txtDescripcion.TabIndex = 1;
            this.txtDescripcion.TextChanged += new System.EventHandler(this.txtDescripcion_TextChanged);
            // 
            // txtCodigo
            // 
            this.txtCodigo.Etiqueta = "Código";
            this.txtCodigo.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.Location = new System.Drawing.Point(6, 7);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.PasarEnfoqueConEnter = false;
            this.txtCodigo.SeleccionarTextoAlEnfoque = false;
            this.txtCodigo.Size = new System.Drawing.Size(130, 21);
            this.txtCodigo.TabIndex = 0;
            this.txtCodigo.TextChanged += new System.EventHandler(this.txtCodigo_TextChanged);
            this.txtCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            // 
            // gpbBusquedaAv
            // 
            this.gpbBusquedaAv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbBusquedaAv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.gpbBusquedaAv.Controls.Add(this.cmbMarcaParte);
            this.gpbBusquedaAv.Controls.Add(this.cmbCar01);
            this.gpbBusquedaAv.Controls.Add(this.txtCodigoAlterno);
            this.gpbBusquedaAv.Controls.Add(this.cmbSistema);
            this.gpbBusquedaAv.Controls.Add(this.cmbLinea);
            this.gpbBusquedaAv.Controls.Add(this.chkEquivalentes);
            this.gpbBusquedaAv.Controls.Add(this.chkBusquedaPorAplicacion);
            this.gpbBusquedaAv.Controls.Add(this.flpCaracteristicas);
            this.gpbBusquedaAv.ForeColor = System.Drawing.Color.White;
            this.gpbBusquedaAv.Location = new System.Drawing.Point(357, 39);
            this.gpbBusquedaAv.Name = "gpbBusquedaAv";
            this.gpbBusquedaAv.Size = new System.Drawing.Size(998, 64);
            this.gpbBusquedaAv.TabIndex = 2;
            this.gpbBusquedaAv.TabStop = false;
            this.gpbBusquedaAv.Text = "Búsqueda";
            // 
            // cmbMarcaParte
            // 
            this.cmbMarcaParte.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbMarcaParte.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbMarcaParte.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbMarcaParte.DataSource = null;
            this.cmbMarcaParte.DisplayMember = "";
            this.cmbMarcaParte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbMarcaParte.Etiqueta = "Marca";
            this.cmbMarcaParte.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbMarcaParte.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMarcaParte.Location = new System.Drawing.Point(191, 40);
            this.cmbMarcaParte.Name = "cmbMarcaParte";
            this.cmbMarcaParte.SelectedIndex = -1;
            this.cmbMarcaParte.SelectedItem = null;
            this.cmbMarcaParte.SelectedText = "";
            this.cmbMarcaParte.SelectedValue = null;
            this.cmbMarcaParte.Size = new System.Drawing.Size(143, 18);
            this.cmbMarcaParte.TabIndex = 4;
            this.cmbMarcaParte.ValueMember = "";
            this.cmbMarcaParte.SelectedIndexChanged += new System.EventHandler(this.cmbMarcaParte_SelectedIndexChanged);
            this.cmbMarcaParte.TextChanged += new System.EventHandler(this.cmbMarcaParte_TextChanged);
            // 
            // cmbCar01
            // 
            this.cmbCar01.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCar01.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCar01.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbCar01.DataSource = null;
            this.cmbCar01.DisplayMember = "";
            this.cmbCar01.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbCar01.Enabled = false;
            this.cmbCar01.Etiqueta = "CarC01";
            this.cmbCar01.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbCar01.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCar01.Location = new System.Drawing.Point(200, 40);
            this.cmbCar01.Name = "cmbCar01";
            this.cmbCar01.SelectedIndex = -1;
            this.cmbCar01.SelectedItem = null;
            this.cmbCar01.SelectedText = "";
            this.cmbCar01.SelectedValue = null;
            this.cmbCar01.Size = new System.Drawing.Size(100, 18);
            this.cmbCar01.TabIndex = 6;
            this.cmbCar01.ValueMember = "";
            this.cmbCar01.Visible = false;
            // 
            // txtCodigoAlterno
            // 
            this.txtCodigoAlterno.Etiqueta = "Código Alterno";
            this.txtCodigoAlterno.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtCodigoAlterno.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigoAlterno.Location = new System.Drawing.Point(254, 13);
            this.txtCodigoAlterno.Name = "txtCodigoAlterno";
            this.txtCodigoAlterno.PasarEnfoqueConEnter = true;
            this.txtCodigoAlterno.SeleccionarTextoAlEnfoque = false;
            this.txtCodigoAlterno.Size = new System.Drawing.Size(80, 18);
            this.txtCodigoAlterno.TabIndex = 2;
            this.txtCodigoAlterno.TextChanged += new System.EventHandler(this.txtCodigoAlterno_TextChanged);
            // 
            // cmbSistema
            // 
            this.cmbSistema.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSistema.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSistema.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbSistema.DataSource = null;
            this.cmbSistema.DisplayMember = "";
            this.cmbSistema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbSistema.Etiqueta = "Sistema";
            this.cmbSistema.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbSistema.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSistema.Location = new System.Drawing.Point(79, 40);
            this.cmbSistema.Name = "cmbSistema";
            this.cmbSistema.SelectedIndex = -1;
            this.cmbSistema.SelectedItem = null;
            this.cmbSistema.SelectedText = "";
            this.cmbSistema.SelectedValue = null;
            this.cmbSistema.Size = new System.Drawing.Size(106, 18);
            this.cmbSistema.TabIndex = 5;
            this.cmbSistema.ValueMember = "";
            this.cmbSistema.SelectedIndexChanged += new System.EventHandler(this.cmbSistema_SelectedIndexChanged);
            this.cmbSistema.TextChanged += new System.EventHandler(this.cmbSistema_TextChanged);
            // 
            // cmbLinea
            // 
            this.cmbLinea.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbLinea.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbLinea.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbLinea.DataSource = null;
            this.cmbLinea.DisplayMember = "";
            this.cmbLinea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbLinea.Etiqueta = "Línea";
            this.cmbLinea.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbLinea.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLinea.Location = new System.Drawing.Point(79, 13);
            this.cmbLinea.Name = "cmbLinea";
            this.cmbLinea.SelectedIndex = -1;
            this.cmbLinea.SelectedItem = null;
            this.cmbLinea.SelectedText = "";
            this.cmbLinea.SelectedValue = null;
            this.cmbLinea.Size = new System.Drawing.Size(172, 18);
            this.cmbLinea.TabIndex = 3;
            this.cmbLinea.ValueMember = "";
            this.cmbLinea.SelectedIndexChanged += new System.EventHandler(this.cmbLinea_SelectedIndexChanged);
            this.cmbLinea.TextChanged += new System.EventHandler(this.cmbLinea_TextChanged);
            // 
            // chkEquivalentes
            // 
            this.chkEquivalentes.AutoSize = true;
            this.chkEquivalentes.Checked = true;
            this.chkEquivalentes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEquivalentes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEquivalentes.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEquivalentes.Location = new System.Drawing.Point(4, 40);
            this.chkEquivalentes.Name = "chkEquivalentes";
            this.chkEquivalentes.Size = new System.Drawing.Size(78, 20);
            this.chkEquivalentes.TabIndex = 1;
            this.chkEquivalentes.Text = "Equivalentes";
            this.tltBotones.SetToolTip(this.chkEquivalentes, "Indica si se deben filtrar partes equivalentes.");
            this.chkEquivalentes.UseVisualStyleBackColor = true;
            this.chkEquivalentes.CheckedChanged += new System.EventHandler(this.chkEquivalentes_CheckedChanged);
            // 
            // chkBusquedaPorAplicacion
            // 
            this.chkBusquedaPorAplicacion.AutoSize = true;
            this.chkBusquedaPorAplicacion.Checked = true;
            this.chkBusquedaPorAplicacion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBusquedaPorAplicacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkBusquedaPorAplicacion.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkBusquedaPorAplicacion.Location = new System.Drawing.Point(4, 12);
            this.chkBusquedaPorAplicacion.Name = "chkBusquedaPorAplicacion";
            this.chkBusquedaPorAplicacion.Size = new System.Drawing.Size(77, 20);
            this.chkBusquedaPorAplicacion.TabIndex = 0;
            this.chkBusquedaPorAplicacion.Text = "Aplicaciones";
            this.tltBotones.SetToolTip(this.chkBusquedaPorAplicacion, "Indica si se debe realizar la búsqueda según el Vehículo seleccionado.");
            this.chkBusquedaPorAplicacion.UseVisualStyleBackColor = true;
            this.chkBusquedaPorAplicacion.CheckedChanged += new System.EventHandler(this.chkBusquedaPorAplicacion_CheckedChanged);
            // 
            // flpCaracteristicas
            // 
            this.flpCaracteristicas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCaracteristicas.Location = new System.Drawing.Point(335, 8);
            this.flpCaracteristicas.Name = "flpCaracteristicas";
            this.flpCaracteristicas.Size = new System.Drawing.Size(657, 54);
            this.flpCaracteristicas.TabIndex = 199;
            // 
            // pnlDetalle
            // 
            this.pnlDetalle.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pnlDetalle.Controls.Add(this.dgvProductos);
            this.pnlDetalle.Controls.Add(this.panel6);
            this.pnlDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetalle.Location = new System.Drawing.Point(0, 0);
            this.pnlDetalle.Name = "pnlDetalle";
            this.pnlDetalle.Size = new System.Drawing.Size(339, 446);
            this.pnlDetalle.TabIndex = 5;
            // 
            // dgvProductos
            // 
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.AllowUserToDeleteRows = false;
            this.dgvProductos.AllowUserToResizeColumns = false;
            this.dgvProductos.AllowUserToResizeRows = false;
            this.dgvProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProductos.BackgroundColor = System.Drawing.Color.White;
            this.dgvProductos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProductos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProductos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvProductos.ColumnHeadersVisible = false;
            this.dgvProductos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Descripcion,
            this.Importe});
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProductos.DefaultCellStyle = dataGridViewCellStyle14;
            this.dgvProductos.Location = new System.Drawing.Point(3, 3);
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.ReadOnly = true;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProductos.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dgvProductos.RowHeadersVisible = false;
            this.dgvProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductos.Size = new System.Drawing.Size(333, 409);
            this.dgvProductos.TabIndex = 1;
            this.dgvProductos.CurrentCellChanged += new System.EventHandler(this.dgvProductos_CurrentCellChanged);
            this.dgvProductos.Enter += new System.EventHandler(this.dgvProductos_Enter);
            this.dgvProductos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvProductos_KeyDown);
            // 
            // Descripcion
            // 
            this.Descripcion.DataPropertyName = "Descripcion";
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Descripcion.DefaultCellStyle = dataGridViewCellStyle12;
            this.Descripcion.HeaderText = "Descripción";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            this.Descripcion.Width = 233;
            // 
            // Importe
            // 
            this.Importe.DataPropertyName = "Importe";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.Format = "C2";
            dataGridViewCellStyle13.NullValue = null;
            this.Importe.DefaultCellStyle = dataGridViewCellStyle13;
            this.Importe.HeaderText = "Importe";
            this.Importe.Name = "Importe";
            this.Importe.ReadOnly = true;
            this.Importe.Width = 80;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.lblTotal);
            this.panel6.Controls.Add(this.label35);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 418);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(339, 28);
            this.panel6.TabIndex = 0;
            // 
            // pnlContenidoDetalle
            // 
            this.pnlContenidoDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlContenidoDetalle.BackColor = System.Drawing.Color.White;
            this.pnlContenidoDetalle.Controls.Add(this.pnlDetalle);
            this.pnlContenidoDetalle.Location = new System.Drawing.Point(3, 194);
            this.pnlContenidoDetalle.Name = "pnlContenidoDetalle";
            this.pnlContenidoDetalle.Size = new System.Drawing.Size(339, 446);
            this.pnlContenidoDetalle.TabIndex = 9;
            // 
            // pnlContenido
            // 
            this.pnlContenido.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContenido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.pnlContenido.Controls.Add(this.pnlPartes);
            this.pnlContenido.Location = new System.Drawing.Point(357, 109);
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Size = new System.Drawing.Size(998, 510);
            this.pnlContenido.TabIndex = 8;
            // 
            // pnlPartes
            // 
            this.pnlPartes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.pnlPartes.Controls.Add(this.lsvPartesComplementarias);
            this.pnlPartes.Controls.Add(this.panel1);
            this.pnlPartes.Controls.Add(this.lblMensajeBusqueda);
            this.pnlPartes.Controls.Add(this.lsvPartes);
            this.pnlPartes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPartes.Location = new System.Drawing.Point(0, 0);
            this.pnlPartes.Name = "pnlPartes";
            this.pnlPartes.Size = new System.Drawing.Size(998, 510);
            this.pnlPartes.TabIndex = 45;
            // 
            // lsvPartesComplementarias
            // 
            this.lsvPartesComplementarias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvPartesComplementarias.BackColor = System.Drawing.Color.LightYellow;
            this.lsvPartesComplementarias.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsvPartesComplementarias.Font = new System.Drawing.Font("Arial Narrow", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsvPartesComplementarias.ForeColor = System.Drawing.Color.SteelBlue;
            this.lsvPartesComplementarias.Location = new System.Drawing.Point(818, 17);
            this.lsvPartesComplementarias.MultiSelect = false;
            this.lsvPartesComplementarias.Name = "lsvPartesComplementarias";
            this.lsvPartesComplementarias.OwnerDraw = true;
            this.lsvPartesComplementarias.Size = new System.Drawing.Size(180, 493);
            this.lsvPartesComplementarias.TabIndex = 45;
            this.lsvPartesComplementarias.UseCompatibleStateImageBehavior = false;
            this.lsvPartesComplementarias.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lsvPartesComplementarias_DrawItem);
            this.lsvPartesComplementarias.SelectedIndexChanged += new System.EventHandler(this.lsvPartesComplementarias_SelectedIndexChanged);
            this.lsvPartesComplementarias.DoubleClick += new System.EventHandler(this.lsvPartesComplementarias_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.LightYellow;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(818, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 24);
            this.panel1.TabIndex = 46;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DarkGray;
            this.label4.Location = new System.Drawing.Point(19, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "COMPLEMENTARIOS";
            // 
            // lblMensajeBusqueda
            // 
            this.lblMensajeBusqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMensajeBusqueda.BackColor = System.Drawing.Color.Ivory;
            this.lblMensajeBusqueda.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensajeBusqueda.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblMensajeBusqueda.Location = new System.Drawing.Point(2, 3);
            this.lblMensajeBusqueda.Name = "lblMensajeBusqueda";
            this.lblMensajeBusqueda.Size = new System.Drawing.Size(813, 49);
            this.lblMensajeBusqueda.TabIndex = 44;
            this.lblMensajeBusqueda.Text = "Mensaje";
            this.lblMensajeBusqueda.Visible = false;
            // 
            // lsvPartes
            // 
            this.lsvPartes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvPartes.BackColor = System.Drawing.Color.Ivory;
            this.lsvPartes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsvPartes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LiTexto,
            this.LiNumeroDeParte,
            this.LiDescripcion,
            this.LiLinea,
            this.LiMarca,
            LiParteID});
            this.lsvPartes.Font = new System.Drawing.Font("Arial Narrow", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsvPartes.ForeColor = System.Drawing.Color.SteelBlue;
            this.lsvPartes.FullRowSelect = true;
            this.lsvPartes.Location = new System.Drawing.Point(0, 0);
            this.lsvPartes.MultiSelect = false;
            this.lsvPartes.Name = "lsvPartes";
            this.lsvPartes.OwnerDraw = true;
            this.lsvPartes.Size = new System.Drawing.Size(816, 513);
            this.lsvPartes.TabIndex = 3;
            this.lsvPartes.UseCompatibleStateImageBehavior = false;
            this.lsvPartes.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lsvPartes_ColumnClick);
            this.lsvPartes.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lsvPartes_DrawColumnHeader);
            this.lsvPartes.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lsvPartes_DrawItem);
            this.lsvPartes.SelectedIndexChanged += new System.EventHandler(this.lsvPartes_SelectedIndexChanged);
            this.lsvPartes.DoubleClick += new System.EventHandler(this.lsvPartes_DoubleClick);
            this.lsvPartes.Enter += new System.EventHandler(this.lsvPartes_Enter);
            this.lsvPartes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lsvPartes_MouseUp);
            // 
            // LiTexto
            // 
            this.LiTexto.Width = 0;
            // 
            // LiNumeroDeParte
            // 
            this.LiNumeroDeParte.Text = "Parte";
            // 
            // LiDescripcion
            // 
            this.LiDescripcion.Text = "Descripción";
            this.LiDescripcion.Width = 432;
            // 
            // LiLinea
            // 
            this.LiLinea.Text = "Línea";
            this.LiLinea.Width = 100;
            // 
            // LiMarca
            // 
            this.LiMarca.Text = "Marca";
            this.LiMarca.Width = 100;
            // 
            // pnlOpciones
            // 
            this.pnlOpciones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlOpciones.Controls.Add(this.btnEjecutar);
            this.pnlOpciones.Controls.Add(this.btnLimpiar);
            this.pnlOpciones.Controls.Add(this.btn9500);
            this.pnlOpciones.Controls.Add(this.btnFacturarVentas);
            this.pnlOpciones.Controls.Add(this.btnCobranza);
            this.pnlOpciones.Controls.Add(this.btnCancelar);
            this.pnlOpciones.Controls.Add(this.btnGarantia);
            this.pnlOpciones.Controls.Add(this.btnReporteDeFaltante);
            this.pnlOpciones.Controls.Add(this.btnDirectorio);
            this.pnlOpciones.Controls.Add(this.btnReimpresion);
            this.pnlOpciones.Controls.Add(this.btnMoto);
            this.pnlOpciones.Controls.Add(this.btnCaja);
            this.pnlOpciones.Controls.Add(this.btnGanancias);
            this.pnlOpciones.Controls.Add(this.lblComisionEstimada);
            this.pnlOpciones.Location = new System.Drawing.Point(3, 645);
            this.pnlOpciones.Name = "pnlOpciones";
            this.pnlOpciones.Size = new System.Drawing.Size(339, 94);
            this.pnlOpciones.TabIndex = 6;
            // 
            // btnEjecutar
            // 
            this.btnEjecutar.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voEjecutar;
            this.btnEjecutar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEjecutar.FlatAppearance.BorderSize = 0;
            this.btnEjecutar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnEjecutar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnEjecutar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEjecutar.Location = new System.Drawing.Point(279, 11);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(60, 60);
            this.btnEjecutar.TabIndex = 12;
            this.tltBotones.SetToolTip(this.btnEjecutar, "Ejecutar la acción seleccionada (F4)");
            this.btnEjecutar.UseVisualStyleBackColor = true;
            this.btnEjecutar.Click += new System.EventHandler(this.btnEjecutar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voLimpiar;
            this.btnLimpiar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLimpiar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Location = new System.Drawing.Point(0, 11);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(37, 35);
            this.btnLimpiar.TabIndex = 0;
            this.tltBotones.SetToolTip(this.btnLimpiar, "Limpiar opción actual y regresar a ventas (Ctrl+L)");
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btn9500
            // 
            this.btn9500.BackgroundImage = global::Refaccionaria.App.Properties.Resources.vo9500;
            this.btn9500.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn9500.FlatAppearance.BorderSize = 0;
            this.btn9500.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn9500.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn9500.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn9500.Location = new System.Drawing.Point(0, 48);
            this.btn9500.Name = "btn9500";
            this.btn9500.Size = new System.Drawing.Size(37, 35);
            this.btn9500.TabIndex = 6;
            this.tltBotones.SetToolTip(this.btn9500, "Hacer o completar un 9500 (Ctrl+T)");
            this.btn9500.UseVisualStyleBackColor = true;
            this.btn9500.Click += new System.EventHandler(this.btn9500_Click);
            // 
            // btnFacturarVentas
            // 
            this.btnFacturarVentas.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voFacturarVentas;
            this.btnFacturarVentas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFacturarVentas.FlatAppearance.BorderSize = 0;
            this.btnFacturarVentas.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnFacturarVentas.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnFacturarVentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFacturarVentas.Location = new System.Drawing.Point(43, 11);
            this.btnFacturarVentas.Name = "btnFacturarVentas";
            this.btnFacturarVentas.Size = new System.Drawing.Size(37, 35);
            this.btnFacturarVentas.TabIndex = 1;
            this.tltBotones.SetToolTip(this.btnFacturarVentas, "Convertir tickets a factura (Ctrl+F)");
            this.btnFacturarVentas.UseVisualStyleBackColor = true;
            this.btnFacturarVentas.Click += new System.EventHandler(this.btnFacturarVentas_Click);
            // 
            // btnCobranza
            // 
            this.btnCobranza.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voCobranza;
            this.btnCobranza.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCobranza.FlatAppearance.BorderSize = 0;
            this.btnCobranza.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCobranza.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCobranza.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCobranza.Location = new System.Drawing.Point(43, 48);
            this.btnCobranza.Name = "btnCobranza";
            this.btnCobranza.Size = new System.Drawing.Size(37, 35);
            this.btnCobranza.TabIndex = 7;
            this.tltBotones.SetToolTip(this.btnCobranza, "Cobranza (Ctrl+B)");
            this.btnCobranza.UseVisualStyleBackColor = true;
            this.btnCobranza.Click += new System.EventHandler(this.btnCobranza_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voCancelar;
            this.btnCancelar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCancelar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Location = new System.Drawing.Point(86, 11);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(37, 35);
            this.btnCancelar.TabIndex = 2;
            this.tltBotones.SetToolTip(this.btnCancelar, "Cancelar ventas (Ctrl+K)");
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGarantia
            // 
            this.btnGarantia.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voGarantia;
            this.btnGarantia.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnGarantia.FlatAppearance.BorderSize = 0;
            this.btnGarantia.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnGarantia.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnGarantia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGarantia.Location = new System.Drawing.Point(86, 48);
            this.btnGarantia.Name = "btnGarantia";
            this.btnGarantia.Size = new System.Drawing.Size(37, 35);
            this.btnGarantia.TabIndex = 8;
            this.tltBotones.SetToolTip(this.btnGarantia, "Garantía (Ctrl+G)");
            this.btnGarantia.UseVisualStyleBackColor = true;
            this.btnGarantia.Click += new System.EventHandler(this.btnGarantia_Click);
            // 
            // btnReporteDeFaltante
            // 
            this.btnReporteDeFaltante.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voReporteDeFaltante;
            this.btnReporteDeFaltante.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReporteDeFaltante.FlatAppearance.BorderSize = 0;
            this.btnReporteDeFaltante.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnReporteDeFaltante.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnReporteDeFaltante.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReporteDeFaltante.Location = new System.Drawing.Point(129, 11);
            this.btnReporteDeFaltante.Name = "btnReporteDeFaltante";
            this.btnReporteDeFaltante.Size = new System.Drawing.Size(37, 35);
            this.btnReporteDeFaltante.TabIndex = 3;
            this.tltBotones.SetToolTip(this.btnReporteDeFaltante, "Reportar faltante de existencia del producto seleccionado (Ctrl+R)");
            this.btnReporteDeFaltante.UseVisualStyleBackColor = true;
            this.btnReporteDeFaltante.Click += new System.EventHandler(this.btnReporteDeFaltante_Click);
            // 
            // btnDirectorio
            // 
            this.btnDirectorio.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voDirectorio;
            this.btnDirectorio.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDirectorio.FlatAppearance.BorderSize = 0;
            this.btnDirectorio.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnDirectorio.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnDirectorio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDirectorio.Location = new System.Drawing.Point(129, 48);
            this.btnDirectorio.Name = "btnDirectorio";
            this.btnDirectorio.Size = new System.Drawing.Size(37, 35);
            this.btnDirectorio.TabIndex = 9;
            this.tltBotones.SetToolTip(this.btnDirectorio, "Directorio (Ctrl+D)");
            this.btnDirectorio.UseVisualStyleBackColor = true;
            this.btnDirectorio.Click += new System.EventHandler(this.btnDirectorio_Click);
            // 
            // btnReimpresion
            // 
            this.btnReimpresion.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voReimpresion;
            this.btnReimpresion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReimpresion.FlatAppearance.BorderSize = 0;
            this.btnReimpresion.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnReimpresion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnReimpresion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReimpresion.Location = new System.Drawing.Point(172, 11);
            this.btnReimpresion.Name = "btnReimpresion";
            this.btnReimpresion.Size = new System.Drawing.Size(37, 35);
            this.btnReimpresion.TabIndex = 4;
            this.tltBotones.SetToolTip(this.btnReimpresion, "Reimprimir ventas (Ctrl+I)");
            this.btnReimpresion.UseVisualStyleBackColor = true;
            this.btnReimpresion.Click += new System.EventHandler(this.btnReimpresion_Click);
            // 
            // btnMoto
            // 
            this.btnMoto.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voMoto;
            this.btnMoto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMoto.FlatAppearance.BorderSize = 0;
            this.btnMoto.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnMoto.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnMoto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoto.Location = new System.Drawing.Point(172, 48);
            this.btnMoto.Name = "btnMoto";
            this.btnMoto.Size = new System.Drawing.Size(37, 35);
            this.btnMoto.TabIndex = 10;
            this.btnMoto.UseVisualStyleBackColor = true;
            // 
            // btnCaja
            // 
            this.btnCaja.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voCaja;
            this.btnCaja.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCaja.FlatAppearance.BorderSize = 0;
            this.btnCaja.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCaja.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCaja.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCaja.Location = new System.Drawing.Point(215, 11);
            this.btnCaja.Name = "btnCaja";
            this.btnCaja.Size = new System.Drawing.Size(37, 35);
            this.btnCaja.TabIndex = 5;
            this.tltBotones.SetToolTip(this.btnCaja, "Opciones de Caja (Ctrl+J)");
            this.btnCaja.UseVisualStyleBackColor = true;
            this.btnCaja.Click += new System.EventHandler(this.btnCaja_Click);
            // 
            // btnGanancias
            // 
            this.btnGanancias.BackgroundImage = global::Refaccionaria.App.Properties.Resources.voGanancias;
            this.btnGanancias.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnGanancias.FlatAppearance.BorderSize = 0;
            this.btnGanancias.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnGanancias.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnGanancias.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGanancias.Location = new System.Drawing.Point(215, 48);
            this.btnGanancias.Name = "btnGanancias";
            this.btnGanancias.Size = new System.Drawing.Size(37, 35);
            this.btnGanancias.TabIndex = 11;
            this.tltBotones.SetToolTip(this.btnGanancias, "Ver comisiones (Ctrl+M)");
            this.btnGanancias.UseVisualStyleBackColor = true;
            this.btnGanancias.Click += new System.EventHandler(this.btnGanancias_Click);
            // 
            // lblComisionEstimada
            // 
            this.lblComisionEstimada.AllowDrop = true;
            this.lblComisionEstimada.AutoSize = true;
            this.lblComisionEstimada.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComisionEstimada.ForeColor = System.Drawing.Color.DarkGray;
            this.lblComisionEstimada.Location = new System.Drawing.Point(258, 71);
            this.lblComisionEstimada.Name = "lblComisionEstimada";
            this.lblComisionEstimada.Size = new System.Drawing.Size(28, 12);
            this.lblComisionEstimada.TabIndex = 37;
            this.lblComisionEstimada.Text = "$0.00";
            // 
            // pnlContenidoEquivalentes
            // 
            this.pnlContenidoEquivalentes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContenidoEquivalentes.BackColor = System.Drawing.Color.White;
            this.pnlContenidoEquivalentes.Controls.Add(this.pnlEquivalentes);
            this.pnlContenidoEquivalentes.Location = new System.Drawing.Point(357, 625);
            this.pnlContenidoEquivalentes.Name = "pnlContenidoEquivalentes";
            this.pnlContenidoEquivalentes.Size = new System.Drawing.Size(998, 140);
            this.pnlContenidoEquivalentes.TabIndex = 10;
            // 
            // tltBotones
            // 
            this.tltBotones.BackColor = System.Drawing.Color.White;
            this.tltBotones.IsBalloon = true;
            // 
            // ctmStripSel
            // 
            this.ctmStripSel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportarFaltanteDeExistenciaToolStripMenuItem,
            this.smiReportarErrorParte,
            this.toolStripSeparator2,
            this.smiPar_Kardex,
            this.toolStripMenuItem1,
            this.buscarProveedorToolStripMenuItem,
            this.toolStripSeparator1,
            this.mostrarLíneaToolStripMenuItem,
            this.mostrarProveedoresToolStripMenuItem});
            this.ctmStripSel.Name = "ctmStripSel";
            this.ctmStripSel.Size = new System.Drawing.Size(242, 154);
            // 
            // reportarFaltanteDeExistenciaToolStripMenuItem
            // 
            this.reportarFaltanteDeExistenciaToolStripMenuItem.Name = "reportarFaltanteDeExistenciaToolStripMenuItem";
            this.reportarFaltanteDeExistenciaToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.reportarFaltanteDeExistenciaToolStripMenuItem.Text = "Reportar faltante de existencia...";
            this.reportarFaltanteDeExistenciaToolStripMenuItem.Click += new System.EventHandler(this.reportarFaltanteDeExistenciaToolStripMenuItem_Click);
            // 
            // smiReportarErrorParte
            // 
            this.smiReportarErrorParte.Name = "smiReportarErrorParte";
            this.smiReportarErrorParte.Size = new System.Drawing.Size(241, 22);
            this.smiReportarErrorParte.Text = "Reportar error";
            this.smiReportarErrorParte.Click += new System.EventHandler(this.smiReportarErrorParte_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(238, 6);
            // 
            // smiPar_Kardex
            // 
            this.smiPar_Kardex.Name = "smiPar_Kardex";
            this.smiPar_Kardex.Size = new System.Drawing.Size(241, 22);
            this.smiPar_Kardex.Text = "Kárdex";
            this.smiPar_Kardex.Click += new System.EventHandler(this.smiPar_Kardex_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(238, 6);
            // 
            // buscarProveedorToolStripMenuItem
            // 
            this.buscarProveedorToolStripMenuItem.Name = "buscarProveedorToolStripMenuItem";
            this.buscarProveedorToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.buscarProveedorToolStripMenuItem.Text = "Buscar Proveedor";
            this.buscarProveedorToolStripMenuItem.Click += new System.EventHandler(this.buscarProveedorToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(238, 6);
            // 
            // mostrarLíneaToolStripMenuItem
            // 
            this.mostrarLíneaToolStripMenuItem.Name = "mostrarLíneaToolStripMenuItem";
            this.mostrarLíneaToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.mostrarLíneaToolStripMenuItem.Text = "Mostrar Línea";
            this.mostrarLíneaToolStripMenuItem.Click += new System.EventHandler(this.mostrarLíneaToolStripMenuItem_Click);
            // 
            // mostrarProveedoresToolStripMenuItem
            // 
            this.mostrarProveedoresToolStripMenuItem.Name = "mostrarProveedoresToolStripMenuItem";
            this.mostrarProveedoresToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.mostrarProveedoresToolStripMenuItem.Text = "Mostrar Marca";
            this.mostrarProveedoresToolStripMenuItem.Click += new System.EventHandler(this.mostrarProveedoresToolStripMenuItem_Click);
            // 
            // pnlGenBuscador
            // 
            this.pnlGenBuscador.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGenBuscador.Controls.Add(this.pnlBuscador);
            this.pnlGenBuscador.Location = new System.Drawing.Point(357, 3);
            this.pnlGenBuscador.Name = "pnlGenBuscador";
            this.pnlGenBuscador.Size = new System.Drawing.Size(998, 40);
            this.pnlGenBuscador.TabIndex = 12;
            // 
            // pnlBuscador
            // 
            this.pnlBuscador.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBuscador.Controls.Add(this.pnlBusqueda);
            this.pnlBuscador.Location = new System.Drawing.Point(0, 0);
            this.pnlBuscador.Name = "pnlBuscador";
            this.pnlBuscador.Size = new System.Drawing.Size(998, 40);
            this.pnlBuscador.TabIndex = 12;
            // 
            // tacCliente
            // 
            this.tacCliente.AltoLista = 320;
            this.tacCliente.AutoSize = true;
            this.tacCliente.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tacCliente.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tacCliente.CampoMostrar = null;
            this.tacCliente.CampoValor = null;
            this.tacCliente.ColorDeFondo = System.Drawing.SystemColors.Window;
            this.tacCliente.ColorDeLetra = System.Drawing.SystemColors.WindowText;
            this.tacCliente.Etiqueta = "";
            this.tacCliente.FuenteDeDatos = null;
            this.tacCliente.Location = new System.Drawing.Point(6, 9);
            this.tacCliente.Margin = new System.Windows.Forms.Padding(0);
            this.tacCliente.Name = "tacCliente";
            this.tacCliente.Size = new System.Drawing.Size(254, 20);
            this.tacCliente.TabIndex = 0;
            this.tacCliente.Texto = "";
            this.tacCliente.ValorSel = null;
            this.tacCliente.SeleccionCambiada += new System.EventHandler(this.tacCliente_SeleccionCambiada);
            // 
            // tacVechiculo
            // 
            this.tacVechiculo.AltoLista = 320;
            this.tacVechiculo.AutoSize = true;
            this.tacVechiculo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tacVechiculo.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tacVechiculo.CampoMostrar = null;
            this.tacVechiculo.CampoValor = null;
            this.tacVechiculo.ColorDeFondo = System.Drawing.SystemColors.Window;
            this.tacVechiculo.ColorDeLetra = System.Drawing.SystemColors.WindowText;
            this.tacVechiculo.Etiqueta = "Vehículo";
            this.tacVechiculo.FuenteDeDatos = null;
            this.tacVechiculo.Location = new System.Drawing.Point(6, 112);
            this.tacVechiculo.Margin = new System.Windows.Forms.Padding(0);
            this.tacVechiculo.Name = "tacVechiculo";
            this.tacVechiculo.Size = new System.Drawing.Size(206, 20);
            this.tacVechiculo.TabIndex = 13;
            this.tacVechiculo.Texto = "";
            this.tacVechiculo.ValorSel = null;
            this.tacVechiculo.SeleccionCambiada += new System.EventHandler(this.tacVehiculo_SeleccionCambiada);
            this.tacVechiculo.TextoCambiado += new System.EventHandler(this.tacVehiculo_TextoCambiado);
            // 
            // pgbMetas
            // 
            this.pgbMetas.ForeColor = System.Drawing.Color.Black;
            this.pgbMetas.Location = new System.Drawing.Point(1, 3);
            this.pgbMetas.MarqueeAnimationSpeed = 0;
            this.pgbMetas.Name = "pgbMetas";
            this.pgbMetas.Size = new System.Drawing.Size(333, 15);
            this.pgbMetas.TabIndex = 2;
            // 
            // lblDescripcionOpcion
            // 
            this.lblDescripcionOpcion.AllowDrop = true;
            this.lblDescripcionOpcion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescripcionOpcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescripcionOpcion.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDescripcionOpcion.Location = new System.Drawing.Point(127, 738);
            this.lblDescripcionOpcion.Name = "lblDescripcionOpcion";
            this.lblDescripcionOpcion.Size = new System.Drawing.Size(210, 12);
            this.lblDescripcionOpcion.TabIndex = 1;
            this.lblDescripcionOpcion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblDescripcionOpcion.Visible = false;
            // 
            // pcbMetasRegla
            // 
            this.pcbMetasRegla.Image = global::Refaccionaria.App.Properties.Resources.MetasMarcasBarraVentas7;
            this.pcbMetasRegla.Location = new System.Drawing.Point(1, 1);
            this.pcbMetasRegla.Name = "pcbMetasRegla";
            this.pcbMetasRegla.Size = new System.Drawing.Size(333, 5);
            this.pcbMetasRegla.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcbMetasRegla.TabIndex = 3;
            this.pcbMetasRegla.TabStop = false;
            // 
            // pnlBarraVentas
            // 
            this.pnlBarraVentas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlBarraVentas.BackColor = System.Drawing.Color.White;
            this.pnlBarraVentas.Controls.Add(this.pcbMetasRegla);
            this.pnlBarraVentas.Controls.Add(this.pgbMetas);
            this.pnlBarraVentas.Location = new System.Drawing.Point(6, 749);
            this.pnlBarraVentas.Name = "pnlBarraVentas";
            this.pnlBarraVentas.Size = new System.Drawing.Size(333, 15);
            this.pnlBarraVentas.TabIndex = 14;
            // 
            // Ventas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.pnlBarraVentas);
            this.Controls.Add(this.lblDescripcionOpcion);
            this.Controls.Add(this.gpbBusquedaAv);
            this.Controls.Add(this.tacCliente);
            this.Controls.Add(this.tacVechiculo);
            this.Controls.Add(this.pnlGenBuscador);
            this.Controls.Add(this.pnlOpciones);
            this.Controls.Add(this.pnlSupIz);
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.pnlContenidoDetalle);
            this.Controls.Add(this.pnlContenidoEquivalentes);
            this.Name = "Ventas";
            this.Size = new System.Drawing.Size(1360, 768);
            this.Load += new System.EventHandler(this.Ventas_Load);
            this.pnlSupIz.ResumeLayout(false);
            this.pnlSupIz.PerformLayout();
            this.pnlEquivalentes.ResumeLayout(false);
            this.pnlEquivalentes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAplicaciones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCaracteristicas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCodigosAlternos)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.pnlBusqueda.ResumeLayout(false);
            this.pnlBusqueda.PerformLayout();
            this.gpbBusquedaAv.ResumeLayout(false);
            this.gpbBusquedaAv.PerformLayout();
            this.pnlDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.pnlContenidoDetalle.ResumeLayout(false);
            this.pnlContenido.ResumeLayout(false);
            this.pnlPartes.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlOpciones.ResumeLayout(false);
            this.pnlOpciones.PerformLayout();
            this.pnlContenidoEquivalentes.ResumeLayout(false);
            this.ctmStripSel.ResumeLayout(false);
            this.pnlGenBuscador.ResumeLayout(false);
            this.pnlBuscador.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbMetasRegla)).EndInit();
            this.pnlBarraVentas.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlSupIz;
        private Refaccionaria.Negocio.ComboEtiqueta cmbMotor;
        private Refaccionaria.Negocio.ComboEtiqueta cmbAnio;
        private Refaccionaria.Negocio.ComboEtiqueta cmbModelo;
        private Refaccionaria.Negocio.ComboEtiqueta cmbMarca;
        private System.Windows.Forms.Button btnAplicarVehiculo;
        private System.Windows.Forms.Button btnEditarCliente;
        private System.Windows.Forms.Button btnAgregarCliente;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblClienteDatos;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Panel pnlEquivalentes;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Panel pnlBusqueda;
        private Refaccionaria.Negocio.ComboEtiqueta cmbVehiculo;
        private System.Windows.Forms.GroupBox gpbBusquedaAv;
        private Negocio.TextoMod txtCodigo;
        private Negocio.TextoMod txtDescripcion;
        private System.Windows.Forms.ListView lsvPartesEquivalentes;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblPartesExistSuc3;
        private System.Windows.Forms.Label lblPartesExistSuc2;
        private System.Windows.Forms.Label lblPartesExistSuc1;
        private System.Windows.Forms.Label lblPartesPrecio5;
        private System.Windows.Forms.Label lblPartesPrecio4;
        private System.Windows.Forms.Label lblPartesPrecio3;
        private System.Windows.Forms.Label lblPartesPrecio2;
        private System.Windows.Forms.Label lblPartesPrecio1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkPartesPrecios;
        private System.Windows.Forms.Label lblPartesMarca;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblClienteTieneCredito;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblClienteListaDePrecios;
        private System.Windows.Forms.Panel pnlDetalle;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.Label lblMensajeBusqueda;
        private System.Windows.Forms.ListView lsvPartes;
        private System.Windows.Forms.ComboBox cmbCliente;
        private Negocio.ComboEtiqueta cmbMarcaParte;
        private Negocio.ComboEtiqueta cmbLinea;
        private Negocio.ComboEtiqueta cmbSistema;
        private System.Windows.Forms.CheckBox chkBusquedaPorAplicacion;
        private System.Windows.Forms.Panel pnlOpciones;
        private System.Windows.Forms.Label lblComisionEstimada;
        private System.Windows.Forms.Panel pnlPartes;
        public System.Windows.Forms.Panel pnlContenido;
        public System.Windows.Forms.Panel pnlContenidoDetalle;
        private System.Windows.Forms.Button btnVistaDetalle;
        private System.Windows.Forms.Button btnVistaIconos;
        private System.Windows.Forms.ColumnHeader LiNumeroDeParte;
        private System.Windows.Forms.ColumnHeader LiDescripcion;
        private System.Windows.Forms.ColumnHeader LiTexto;
        private System.Windows.Forms.ColumnHeader LiLinea;
        private Negocio.TextoMod txtCodigoAlterno;
        public System.Windows.Forms.Panel pnlContenidoEquivalentes;
        private System.Windows.Forms.ColumnHeader LiMarca;
        private System.Windows.Forms.Button btnBuscarCliente;
        private Negocio.CheckBoxMod chkCotizacion;
        private System.Windows.Forms.Button btnGanancias;
        private System.Windows.Forms.Button btnEjecutar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btn9500;
        private System.Windows.Forms.Button btnFacturarVentas;
        private System.Windows.Forms.Button btnCobranza;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGarantia;
        private System.Windows.Forms.Button btnReporteDeFaltante;
        private System.Windows.Forms.Button btnDirectorio;
        private System.Windows.Forms.Button btnReimpresion;
        private System.Windows.Forms.Button btnMoto;
        private System.Windows.Forms.Button btnCaja;
        private System.Windows.Forms.ToolTip tltBotones;
        private System.Windows.Forms.CheckBox chkEquivalentes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
        private System.Windows.Forms.ContextMenuStrip ctmStripSel;
        private System.Windows.Forms.ToolStripMenuItem reportarFaltanteDeExistenciaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buscarProveedorToolStripMenuItem;
        private System.Windows.Forms.DataGridView dgvCodigosAlternos;
        private System.Windows.Forms.ListView lsvPartesComplementarias;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Panel pnlGenBuscador;
        private System.Windows.Forms.Panel pnlBuscador;
        private System.Windows.Forms.ToolStripMenuItem mostrarLíneaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mostrarProveedoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Negocio.ComboEtiqueta cmbCar01;
        private System.Windows.Forms.DataGridView dgvCaracteristicas;
        private Negocio.TextoMod txtVIN;
        private System.Windows.Forms.DataGridView dgvAplicaciones;
        private Negocio.TextoAutocompletar tacVechiculo;
        private Negocio.TextoAutocompletar tacCliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn cr_ParteCaracteristicaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn cr_Caracteristica;
        private System.Windows.Forms.DataGridViewTextBoxColumn cr_Valor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ca_ParteCodigoAlternoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ca_Marca;
        private System.Windows.Forms.DataGridViewTextBoxColumn ca_CodigoAlterno;
        private System.Windows.Forms.ToolStripMenuItem smiPar_Kardex;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ap_ParteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ap_Modelo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ap_Anio;
        private System.Windows.Forms.DataGridViewTextBoxColumn ap_Motor;
        private System.Windows.Forms.FlowLayoutPanel flpCaracteristicas;
        private System.Windows.Forms.ToolStripMenuItem smiReportarErrorParte;
        private System.Windows.Forms.Button btnActualizarClientes;
        private System.Windows.Forms.ProgressBar pgbMetas;
        private System.Windows.Forms.Label lblDescripcionOpcion;
        private System.Windows.Forms.PictureBox pcbMetasRegla;
        private System.Windows.Forms.Label lblTextoEstado;
        private System.Windows.Forms.Panel pnlBarraVentas;
    }
}
