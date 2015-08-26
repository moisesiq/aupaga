namespace Refaccionaria.App
{
    partial class BusquedaVenta
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvVentas = new System.Windows.Forms.DataGridView();
            this.colVentaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFolio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colImporte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtpFin = new System.Windows.Forms.DateTimePicker();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.rdbFactura = new System.Windows.Forms.RadioButton();
            this.rdbTicket = new System.Windows.Forms.RadioButton();
            this.label51 = new System.Windows.Forms.Label();
            this.txtFolio = new Refaccionaria.Negocio.TextoMod();
            this.cmbSucursal = new Refaccionaria.Negocio.ComboEtiqueta();
            this.pnlDatosDePago = new System.Windows.Forms.Panel();
            this.txtVendedor = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txtAbonos = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtNotaDeCredito = new System.Windows.Forms.TextBox();
            this.txtNoIdentificado = new System.Windows.Forms.TextBox();
            this.txtEPoints = new System.Windows.Forms.TextBox();
            this.txtTransferencia = new System.Windows.Forms.TextBox();
            this.txtTarjeta = new System.Windows.Forms.TextBox();
            this.txtCheque = new System.Windows.Forms.TextBox();
            this.txtEfectivo = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.rdbContado = new System.Windows.Forms.RadioButton();
            this.rdbCredito = new System.Windows.Forms.RadioButton();
            this.label56 = new System.Windows.Forms.Label();
            this.chkMostrarTodasLasVentas = new Refaccionaria.Negocio.CheckBoxMod();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).BeginInit();
            this.pnlDatosDePago.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvVentas
            // 
            this.dgvVentas.AllowUserToAddRows = false;
            this.dgvVentas.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.SkyBlue;
            this.dgvVentas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvVentas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvVentas.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvVentas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvVentas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvVentas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVentas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVentaID,
            this.colFecha,
            this.colCliente,
            this.colFolio,
            this.colImporte});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvVentas.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvVentas.GridColor = System.Drawing.Color.SkyBlue;
            this.dgvVentas.Location = new System.Drawing.Point(6, 56);
            this.dgvVentas.Name = "dgvVentas";
            this.dgvVentas.ReadOnly = true;
            this.dgvVentas.RowHeadersWidth = 24;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            this.dgvVentas.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvVentas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVentas.Size = new System.Drawing.Size(370, 255);
            this.dgvVentas.StandardTab = true;
            this.dgvVentas.TabIndex = 8;
            this.dgvVentas.CurrentCellChanged += new System.EventHandler(this.dgvVentas_CurrentCellChanged);
            // 
            // colVentaID
            // 
            this.colVentaID.HeaderText = "VentaID";
            this.colVentaID.Name = "colVentaID";
            this.colVentaID.ReadOnly = true;
            this.colVentaID.Visible = false;
            // 
            // colFecha
            // 
            this.colFecha.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colFecha.DataPropertyName = "Fecha";
            this.colFecha.HeaderText = "Fecha";
            this.colFecha.Name = "colFecha";
            this.colFecha.ReadOnly = true;
            this.colFecha.Width = 140;
            // 
            // colCliente
            // 
            this.colCliente.HeaderText = "Cliente";
            this.colCliente.Name = "colCliente";
            this.colCliente.ReadOnly = true;
            this.colCliente.Width = 120;
            // 
            // colFolio
            // 
            this.colFolio.HeaderText = "Folio";
            this.colFolio.Name = "colFolio";
            this.colFolio.ReadOnly = true;
            this.colFolio.Width = 80;
            // 
            // colImporte
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            dataGridViewCellStyle2.NullValue = null;
            this.colImporte.DefaultCellStyle = dataGridViewCellStyle2;
            this.colImporte.HeaderText = "Importe";
            this.colImporte.Name = "colImporte";
            this.colImporte.ReadOnly = true;
            // 
            // dtpFin
            // 
            this.dtpFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFin.Location = new System.Drawing.Point(204, 7);
            this.dtpFin.Name = "dtpFin";
            this.dtpFin.Size = new System.Drawing.Size(89, 20);
            this.dtpFin.TabIndex = 2;
            this.dtpFin.ValueChanged += new System.EventHandler(this.dtpFin_ValueChanged);
            // 
            // dtpInicio
            // 
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInicio.Location = new System.Drawing.Point(101, 7);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(89, 20);
            this.dtpInicio.TabIndex = 1;
            this.dtpInicio.ValueChanged += new System.EventHandler(this.dtpInicio_ValueChanged);
            // 
            // rdbFactura
            // 
            this.rdbFactura.AutoSize = true;
            this.rdbFactura.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdbFactura.ForeColor = System.Drawing.Color.Black;
            this.rdbFactura.Location = new System.Drawing.Point(363, 8);
            this.rdbFactura.Name = "rdbFactura";
            this.rdbFactura.Size = new System.Drawing.Size(60, 17);
            this.rdbFactura.TabIndex = 4;
            this.rdbFactura.Text = "Factura";
            this.rdbFactura.UseVisualStyleBackColor = true;
            this.rdbFactura.CheckedChanged += new System.EventHandler(this.rdbFactura_CheckedChanged);
            // 
            // rdbTicket
            // 
            this.rdbTicket.AutoSize = true;
            this.rdbTicket.Checked = true;
            this.rdbTicket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdbTicket.ForeColor = System.Drawing.Color.Black;
            this.rdbTicket.Location = new System.Drawing.Point(304, 8);
            this.rdbTicket.Name = "rdbTicket";
            this.rdbTicket.Size = new System.Drawing.Size(54, 17);
            this.rdbTicket.TabIndex = 3;
            this.rdbTicket.TabStop = true;
            this.rdbTicket.Text = "Ticket";
            this.rdbTicket.UseVisualStyleBackColor = true;
            this.rdbTicket.CheckedChanged += new System.EventHandler(this.rdbTicket_CheckedChanged);
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label51.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label51.Location = new System.Drawing.Point(190, 6);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(13, 13);
            this.label51.TabIndex = 91;
            this.label51.Text = "_";
            // 
            // txtFolio
            // 
            this.txtFolio.Etiqueta = "Folio";
            this.txtFolio.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtFolio.Location = new System.Drawing.Point(430, 7);
            this.txtFolio.Name = "txtFolio";
            this.txtFolio.PasarEnfoqueConEnter = true;
            this.txtFolio.SeleccionarTextoAlEnfoque = false;
            this.txtFolio.Size = new System.Drawing.Size(60, 20);
            this.txtFolio.TabIndex = 5;
            this.txtFolio.TextChanged += new System.EventHandler(this.txtFolio_TextChanged);
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSucursal.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSucursal.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbSucursal.DataSource = null;
            this.cmbSucursal.DisplayMember = "";
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbSucursal.Etiqueta = "Tienda";
            this.cmbSucursal.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbSucursal.Location = new System.Drawing.Point(6, 6);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.SelectedIndex = -1;
            this.cmbSucursal.SelectedItem = null;
            this.cmbSucursal.SelectedText = "";
            this.cmbSucursal.SelectedValue = null;
            this.cmbSucursal.Size = new System.Drawing.Size(89, 21);
            this.cmbSucursal.TabIndex = 0;
            this.cmbSucursal.ValueMember = "";
            this.cmbSucursal.SelectedIndexChanged += new System.EventHandler(this.cmbSucursal_SelectedIndexChanged);
            // 
            // pnlDatosDePago
            // 
            this.pnlDatosDePago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDatosDePago.Controls.Add(this.txtVendedor);
            this.pnlDatosDePago.Controls.Add(this.label26);
            this.pnlDatosDePago.Controls.Add(this.txtAbonos);
            this.pnlDatosDePago.Controls.Add(this.groupBox1);
            this.pnlDatosDePago.Controls.Add(this.rdbContado);
            this.pnlDatosDePago.Controls.Add(this.rdbCredito);
            this.pnlDatosDePago.Controls.Add(this.label56);
            this.pnlDatosDePago.ForeColor = System.Drawing.Color.Black;
            this.pnlDatosDePago.Location = new System.Drawing.Point(382, 32);
            this.pnlDatosDePago.Name = "pnlDatosDePago";
            this.pnlDatosDePago.Size = new System.Drawing.Size(205, 279);
            this.pnlDatosDePago.TabIndex = 9;
            // 
            // txtVendedor
            // 
            this.txtVendedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtVendedor.Location = new System.Drawing.Point(94, 258);
            this.txtVendedor.Name = "txtVendedor";
            this.txtVendedor.ReadOnly = true;
            this.txtVendedor.Size = new System.Drawing.Size(100, 20);
            this.txtVendedor.TabIndex = 4;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label26.ForeColor = System.Drawing.Color.Black;
            this.label26.Location = new System.Drawing.Point(44, 30);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(43, 13);
            this.label26.TabIndex = 105;
            this.label26.Text = "Abonos";
            // 
            // txtAbonos
            // 
            this.txtAbonos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtAbonos.Location = new System.Drawing.Point(94, 27);
            this.txtAbonos.Name = "txtAbonos";
            this.txtAbonos.ReadOnly = true;
            this.txtAbonos.Size = new System.Drawing.Size(100, 20);
            this.txtAbonos.TabIndex = 2;
            this.txtAbonos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtNotaDeCredito);
            this.groupBox1.Controls.Add(this.txtNoIdentificado);
            this.groupBox1.Controls.Add(this.txtEPoints);
            this.groupBox1.Controls.Add(this.txtTransferencia);
            this.groupBox1.Controls.Add(this.txtTarjeta);
            this.groupBox1.Controls.Add(this.txtCheque);
            this.groupBox1.Controls.Add(this.txtEfectivo);
            this.groupBox1.Controls.Add(this.label53);
            this.groupBox1.Controls.Add(this.label47);
            this.groupBox1.Controls.Add(this.label52);
            this.groupBox1.Controls.Add(this.label46);
            this.groupBox1.Controls.Add(this.label45);
            this.groupBox1.Controls.Add(this.label39);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Location = new System.Drawing.Point(1, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(202, 199);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Forma de Pago";
            // 
            // txtNotaDeCredito
            // 
            this.txtNotaDeCredito.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtNotaDeCredito.Location = new System.Drawing.Point(93, 170);
            this.txtNotaDeCredito.Name = "txtNotaDeCredito";
            this.txtNotaDeCredito.ReadOnly = true;
            this.txtNotaDeCredito.Size = new System.Drawing.Size(100, 20);
            this.txtNotaDeCredito.TabIndex = 6;
            this.txtNotaDeCredito.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtNoIdentificado
            // 
            this.txtNoIdentificado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtNoIdentificado.Location = new System.Drawing.Point(93, 144);
            this.txtNoIdentificado.Name = "txtNoIdentificado";
            this.txtNoIdentificado.ReadOnly = true;
            this.txtNoIdentificado.Size = new System.Drawing.Size(100, 20);
            this.txtNoIdentificado.TabIndex = 5;
            this.txtNoIdentificado.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtEPoints
            // 
            this.txtEPoints.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtEPoints.Location = new System.Drawing.Point(93, 119);
            this.txtEPoints.Name = "txtEPoints";
            this.txtEPoints.ReadOnly = true;
            this.txtEPoints.Size = new System.Drawing.Size(100, 20);
            this.txtEPoints.TabIndex = 4;
            this.txtEPoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTransferencia
            // 
            this.txtTransferencia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtTransferencia.Location = new System.Drawing.Point(93, 93);
            this.txtTransferencia.Name = "txtTransferencia";
            this.txtTransferencia.ReadOnly = true;
            this.txtTransferencia.Size = new System.Drawing.Size(100, 20);
            this.txtTransferencia.TabIndex = 3;
            this.txtTransferencia.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTarjeta
            // 
            this.txtTarjeta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtTarjeta.Location = new System.Drawing.Point(93, 67);
            this.txtTarjeta.Name = "txtTarjeta";
            this.txtTarjeta.ReadOnly = true;
            this.txtTarjeta.Size = new System.Drawing.Size(100, 20);
            this.txtTarjeta.TabIndex = 2;
            this.txtTarjeta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtCheque
            // 
            this.txtCheque.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtCheque.Location = new System.Drawing.Point(93, 40);
            this.txtCheque.Name = "txtCheque";
            this.txtCheque.ReadOnly = true;
            this.txtCheque.Size = new System.Drawing.Size(100, 20);
            this.txtCheque.TabIndex = 1;
            this.txtCheque.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtEfectivo
            // 
            this.txtEfectivo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtEfectivo.Location = new System.Drawing.Point(93, 14);
            this.txtEfectivo.Name = "txtEfectivo";
            this.txtEfectivo.ReadOnly = true;
            this.txtEfectivo.Size = new System.Drawing.Size(100, 20);
            this.txtEfectivo.TabIndex = 0;
            this.txtEfectivo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label53.ForeColor = System.Drawing.Color.Black;
            this.label53.Location = new System.Drawing.Point(21, 173);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(66, 13);
            this.label53.TabIndex = 0;
            this.label53.Text = "Nota Crédito";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label47.ForeColor = System.Drawing.Color.Black;
            this.label47.Location = new System.Drawing.Point(45, 122);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(42, 13);
            this.label47.TabIndex = 0;
            this.label47.Text = "ePoints";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label52.ForeColor = System.Drawing.Color.Black;
            this.label52.Location = new System.Drawing.Point(8, 147);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(79, 13);
            this.label52.TabIndex = 0;
            this.label52.Text = "No Identificado";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label46.ForeColor = System.Drawing.Color.Black;
            this.label46.Location = new System.Drawing.Point(15, 96);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(72, 13);
            this.label46.TabIndex = 0;
            this.label46.Text = "Transferencia";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label45.ForeColor = System.Drawing.Color.Black;
            this.label45.Location = new System.Drawing.Point(47, 70);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(40, 13);
            this.label45.TabIndex = 0;
            this.label45.Text = "Tarjeta";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label39.ForeColor = System.Drawing.Color.Black;
            this.label39.Location = new System.Drawing.Point(43, 43);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(44, 13);
            this.label39.TabIndex = 0;
            this.label39.Text = "Cheque";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label27.ForeColor = System.Drawing.Color.Black;
            this.label27.Location = new System.Drawing.Point(41, 17);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(46, 13);
            this.label27.TabIndex = 0;
            this.label27.Text = "Efectivo";
            // 
            // rdbContado
            // 
            this.rdbContado.AutoSize = true;
            this.rdbContado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.rdbContado.Enabled = false;
            this.rdbContado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdbContado.ForeColor = System.Drawing.Color.Black;
            this.rdbContado.Location = new System.Drawing.Point(15, 3);
            this.rdbContado.Name = "rdbContado";
            this.rdbContado.Size = new System.Drawing.Size(64, 17);
            this.rdbContado.TabIndex = 0;
            this.rdbContado.TabStop = true;
            this.rdbContado.Text = "Contado";
            this.rdbContado.UseVisualStyleBackColor = false;
            // 
            // rdbCredito
            // 
            this.rdbCredito.AutoSize = true;
            this.rdbCredito.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.rdbCredito.Enabled = false;
            this.rdbCredito.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdbCredito.ForeColor = System.Drawing.Color.Black;
            this.rdbCredito.Location = new System.Drawing.Point(104, 3);
            this.rdbCredito.Name = "rdbCredito";
            this.rdbCredito.Size = new System.Drawing.Size(57, 17);
            this.rdbCredito.TabIndex = 1;
            this.rdbCredito.TabStop = true;
            this.rdbCredito.Text = "Crédito";
            this.rdbCredito.UseVisualStyleBackColor = false;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.label56.ForeColor = System.Drawing.Color.Black;
            this.label56.Location = new System.Drawing.Point(34, 261);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(53, 13);
            this.label56.TabIndex = 104;
            this.label56.Text = "Vendedor";
            // 
            // chkMostrarTodasLasVentas
            // 
            this.chkMostrarTodasLasVentas.AutoSize = true;
            this.chkMostrarTodasLasVentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkMostrarTodasLasVentas.ForeColor = System.Drawing.Color.Black;
            this.chkMostrarTodasLasVentas.Location = new System.Drawing.Point(6, 33);
            this.chkMostrarTodasLasVentas.Name = "chkMostrarTodasLasVentas";
            this.chkMostrarTodasLasVentas.Size = new System.Drawing.Size(139, 17);
            this.chkMostrarTodasLasVentas.TabIndex = 7;
            this.chkMostrarTodasLasVentas.Text = "Mostrar todas las Ventas";
            this.chkMostrarTodasLasVentas.UseVisualStyleBackColor = true;
            this.chkMostrarTodasLasVentas.CheckedChanged += new System.EventHandler(this.chkMostrarTodasLasVentas_CheckedChanged);
            // 
            // BusquedaVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.Controls.Add(this.chkMostrarTodasLasVentas);
            this.Controls.Add(this.pnlDatosDePago);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.txtFolio);
            this.Controls.Add(this.dgvVentas);
            this.Controls.Add(this.dtpFin);
            this.Controls.Add(this.dtpInicio);
            this.Controls.Add(this.rdbFactura);
            this.Controls.Add(this.rdbTicket);
            this.Controls.Add(this.label51);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "BusquedaVenta";
            this.Size = new System.Drawing.Size(592, 320);
            this.Load += new System.EventHandler(this.BusquedaVenta_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).EndInit();
            this.pnlDatosDePago.ResumeLayout(false);
            this.pnlDatosDePago.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TextBox txtVendedor;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtAbonos;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtNotaDeCredito;
        private System.Windows.Forms.TextBox txtNoIdentificado;
        private System.Windows.Forms.TextBox txtEPoints;
        private System.Windows.Forms.TextBox txtTransferencia;
        private System.Windows.Forms.TextBox txtTarjeta;
        private System.Windows.Forms.TextBox txtCheque;
        private System.Windows.Forms.TextBox txtEfectivo;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.RadioButton rdbContado;
        private System.Windows.Forms.RadioButton rdbCredito;
        private System.Windows.Forms.Label label56;
        protected System.Windows.Forms.DataGridView dgvVentas;
        protected System.Windows.Forms.DateTimePicker dtpFin;
        protected System.Windows.Forms.DateTimePicker dtpInicio;
        protected System.Windows.Forms.RadioButton rdbFactura;
        protected System.Windows.Forms.RadioButton rdbTicket;
        protected Negocio.TextoMod txtFolio;
        protected Negocio.ComboEtiqueta cmbSucursal;
        protected System.Windows.Forms.Panel pnlDatosDePago;
        protected Negocio.CheckBoxMod chkMostrarTodasLasVentas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVentaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFolio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colImporte;
    }
}
