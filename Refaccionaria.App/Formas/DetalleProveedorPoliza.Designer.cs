namespace Refaccionaria.App
{
    partial class DetalleProveedorPoliza
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFechaMovimiento = new System.Windows.Forms.DateTimePicker();
            this.cboBanco = new System.Windows.Forms.ComboBox();
            this.lblBanco = new System.Windows.Forms.Label();
            this.txtBeneficiario = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblImporte = new System.Windows.Forms.Label();
            this.txtImporte = new Refaccionaria.App.textBoxOnlyDouble();
            this.txtDocumento = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gpoDetalle = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.fac_MovimientoInventarioID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fac_Factura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fac_Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fac_Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fac_Abonado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fac_Saldo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fac_Descuento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fac_Final = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblFormaPago = new System.Windows.Forms.Label();
            this.cboFormaPago = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvAbonos = new System.Windows.Forms.DataGridView();
            this.btnCrearDescuentosFacturas = new System.Windows.Forms.Button();
            this.btnCrearDescuento = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCuentaBancaria = new System.Windows.Forms.ComboBox();
            this.btnNotaDeCredito = new System.Windows.Forms.Button();
            this.btnPagoDeCaja = new System.Windows.Forms.Button();
            this.abo_ProveedorPolizaDetalleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_MovimientoInventarioID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_OrigenID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_CajaEgresoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_NotaDeCreditoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_Factura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_Origen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_ImporteFactura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_ImporteFinal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_NotaDeCredito = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.abo_Observacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gpoGen.SuspendLayout();
            this.gpoDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonos)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.btnPagoDeCaja);
            this.gpoGen.Controls.Add(this.btnNotaDeCredito);
            this.gpoGen.Controls.Add(this.label3);
            this.gpoGen.Controls.Add(this.btnCrearDescuento);
            this.gpoGen.Controls.Add(this.btnCrearDescuentosFacturas);
            this.gpoGen.Controls.Add(this.cmbCuentaBancaria);
            this.gpoGen.Controls.Add(this.groupBox1);
            this.gpoGen.Controls.Add(this.cboFormaPago);
            this.gpoGen.Controls.Add(this.lblFormaPago);
            this.gpoGen.Controls.Add(this.gpoDetalle);
            this.gpoGen.Controls.Add(this.label2);
            this.gpoGen.Controls.Add(this.txtDocumento);
            this.gpoGen.Controls.Add(this.txtImporte);
            this.gpoGen.Controls.Add(this.lblImporte);
            this.gpoGen.Controls.Add(this.label1);
            this.gpoGen.Controls.Add(this.txtBeneficiario);
            this.gpoGen.Controls.Add(this.lblBanco);
            this.gpoGen.Controls.Add(this.cboBanco);
            this.gpoGen.Controls.Add(this.dtpFechaMovimiento);
            this.gpoGen.Controls.Add(this.lblFecha);
            this.gpoGen.ForeColor = System.Drawing.Color.White;
            this.gpoGen.Location = new System.Drawing.Point(12, 2);
            this.gpoGen.Size = new System.Drawing.Size(782, 573);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(712, 581);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(551, 581);
            this.btnGuardar.Size = new System.Drawing.Size(155, 23);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "&Agregar y Generar Poliza";
            // 
            // lblFecha
            // 
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(12, 41);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(37, 13);
            this.lblFecha.TabIndex = 0;
            this.lblFecha.Text = "Fecha";
            // 
            // dtpFechaMovimiento
            // 
            this.dtpFechaMovimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaMovimiento.Location = new System.Drawing.Point(79, 39);
            this.dtpFechaMovimiento.Name = "dtpFechaMovimiento";
            this.dtpFechaMovimiento.Size = new System.Drawing.Size(117, 20);
            this.dtpFechaMovimiento.TabIndex = 3;
            // 
            // cboBanco
            // 
            this.cboBanco.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBanco.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboBanco.FormattingEnabled = true;
            this.cboBanco.Location = new System.Drawing.Point(433, 13);
            this.cboBanco.Name = "cboBanco";
            this.cboBanco.Size = new System.Drawing.Size(189, 21);
            this.cboBanco.TabIndex = 0;
            this.cboBanco.Visible = false;
            this.cboBanco.SelectedIndexChanged += new System.EventHandler(this.cboBanco_SelectedIndexChanged);
            // 
            // lblBanco
            // 
            this.lblBanco.AutoSize = true;
            this.lblBanco.Location = new System.Drawing.Point(389, 16);
            this.lblBanco.Name = "lblBanco";
            this.lblBanco.Size = new System.Drawing.Size(38, 13);
            this.lblBanco.TabIndex = 3;
            this.lblBanco.Text = "Banco";
            // 
            // txtBeneficiario
            // 
            this.txtBeneficiario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBeneficiario.Location = new System.Drawing.Point(79, 13);
            this.txtBeneficiario.MaxLength = 100;
            this.txtBeneficiario.Name = "txtBeneficiario";
            this.txtBeneficiario.ReadOnly = true;
            this.txtBeneficiario.Size = new System.Drawing.Size(273, 20);
            this.txtBeneficiario.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Beneficiario";
            // 
            // lblImporte
            // 
            this.lblImporte.AutoSize = true;
            this.lblImporte.Location = new System.Drawing.Point(628, 14);
            this.lblImporte.Name = "lblImporte";
            this.lblImporte.Size = new System.Drawing.Size(42, 13);
            this.lblImporte.TabIndex = 6;
            this.lblImporte.Text = "Importe";
            // 
            // txtImporte
            // 
            this.txtImporte.Location = new System.Drawing.Point(676, 13);
            this.txtImporte.MaxLength = 50;
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.Size = new System.Drawing.Size(100, 20);
            this.txtImporte.TabIndex = 2;
            this.txtImporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDocumento
            // 
            this.txtDocumento.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDocumento.Location = new System.Drawing.Point(676, 38);
            this.txtDocumento.MaxLength = 50;
            this.txtDocumento.Name = "txtDocumento";
            this.txtDocumento.Size = new System.Drawing.Size(100, 20);
            this.txtDocumento.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(641, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Folio";
            // 
            // gpoDetalle
            // 
            this.gpoDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpoDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.gpoDetalle.Controls.Add(this.dgvDetalle);
            this.gpoDetalle.ForeColor = System.Drawing.Color.White;
            this.gpoDetalle.Location = new System.Drawing.Point(9, 65);
            this.gpoDetalle.Name = "gpoDetalle";
            this.gpoDetalle.Size = new System.Drawing.Size(646, 274);
            this.gpoDetalle.TabIndex = 6;
            this.gpoDetalle.TabStop = false;
            this.gpoDetalle.Text = "Detalle de las Facturas seleccionadas";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvDetalle.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fac_MovimientoInventarioID,
            this.fac_Factura,
            this.fac_Fecha,
            this.fac_Importe,
            this.fac_Abonado,
            this.fac_Saldo,
            this.fac_Descuento,
            this.fac_Final});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDetalle.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgvDetalle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDetalle.Location = new System.Drawing.Point(6, 16);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.RowHeadersWidth = 25;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(634, 252);
            this.dgvDetalle.TabIndex = 0;
            this.dgvDetalle.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetalle_CellValueChanged);
            // 
            // fac_MovimientoInventarioID
            // 
            this.fac_MovimientoInventarioID.HeaderText = "MovimientoInventarioID";
            this.fac_MovimientoInventarioID.Name = "fac_MovimientoInventarioID";
            this.fac_MovimientoInventarioID.ReadOnly = true;
            this.fac_MovimientoInventarioID.Visible = false;
            // 
            // fac_Factura
            // 
            this.fac_Factura.HeaderText = "Factura";
            this.fac_Factura.Name = "fac_Factura";
            this.fac_Factura.ReadOnly = true;
            this.fac_Factura.Width = 80;
            // 
            // fac_Fecha
            // 
            this.fac_Fecha.HeaderText = "Fecha";
            this.fac_Fecha.Name = "fac_Fecha";
            this.fac_Fecha.ReadOnly = true;
            this.fac_Fecha.Width = 80;
            // 
            // fac_Importe
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "C2";
            this.fac_Importe.DefaultCellStyle = dataGridViewCellStyle6;
            this.fac_Importe.HeaderText = "Importe";
            this.fac_Importe.Name = "fac_Importe";
            this.fac_Importe.ReadOnly = true;
            this.fac_Importe.Width = 80;
            // 
            // fac_Abonado
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "C2";
            this.fac_Abonado.DefaultCellStyle = dataGridViewCellStyle7;
            this.fac_Abonado.HeaderText = "Abonado";
            this.fac_Abonado.Name = "fac_Abonado";
            this.fac_Abonado.ReadOnly = true;
            this.fac_Abonado.Width = 80;
            // 
            // fac_Saldo
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "C2";
            this.fac_Saldo.DefaultCellStyle = dataGridViewCellStyle8;
            this.fac_Saldo.HeaderText = "Saldo";
            this.fac_Saldo.Name = "fac_Saldo";
            this.fac_Saldo.ReadOnly = true;
            this.fac_Saldo.Width = 80;
            // 
            // fac_Descuento
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "C2";
            this.fac_Descuento.DefaultCellStyle = dataGridViewCellStyle9;
            this.fac_Descuento.HeaderText = "Descuento";
            this.fac_Descuento.Name = "fac_Descuento";
            this.fac_Descuento.ReadOnly = true;
            this.fac_Descuento.Width = 80;
            // 
            // fac_Final
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.Format = "C2";
            this.fac_Final.DefaultCellStyle = dataGridViewCellStyle10;
            this.fac_Final.HeaderText = "Final";
            this.fac_Final.Name = "fac_Final";
            this.fac_Final.Width = 80;
            // 
            // lblFormaPago
            // 
            this.lblFormaPago.AutoSize = true;
            this.lblFormaPago.Location = new System.Drawing.Point(348, 41);
            this.lblFormaPago.Name = "lblFormaPago";
            this.lblFormaPago.Size = new System.Drawing.Size(79, 13);
            this.lblFormaPago.TabIndex = 11;
            this.lblFormaPago.Text = "Forma de Pago";
            // 
            // cboFormaPago
            // 
            this.cboFormaPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFormaPago.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboFormaPago.FormattingEnabled = true;
            this.cboFormaPago.Location = new System.Drawing.Point(433, 38);
            this.cboFormaPago.Name = "cboFormaPago";
            this.cboFormaPago.Size = new System.Drawing.Size(189, 21);
            this.cboFormaPago.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.groupBox1.Controls.Add(this.dgvAbonos);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(9, 345);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(767, 222);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Detalle de Abonos";
            // 
            // dgvAbonos
            // 
            this.dgvAbonos.AllowUserToAddRows = false;
            this.dgvAbonos.AllowUserToDeleteRows = false;
            this.dgvAbonos.AllowUserToResizeRows = false;
            this.dgvAbonos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvAbonos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAbonos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvAbonos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAbonos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.abo_ProveedorPolizaDetalleID,
            this.abo_MovimientoInventarioID,
            this.abo_OrigenID,
            this.abo_CajaEgresoID,
            this.abo_NotaDeCreditoID,
            this.abo_Factura,
            this.abo_Origen,
            this.abo_ImporteFactura,
            this.abo_ImporteFinal,
            this.abo_Importe,
            this.abo_NotaDeCredito,
            this.abo_Observacion});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAbonos.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvAbonos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvAbonos.Location = new System.Drawing.Point(6, 19);
            this.dgvAbonos.Name = "dgvAbonos";
            this.dgvAbonos.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAbonos.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvAbonos.RowHeadersVisible = false;
            this.dgvAbonos.RowHeadersWidth = 25;
            this.dgvAbonos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAbonos.Size = new System.Drawing.Size(755, 197);
            this.dgvAbonos.TabIndex = 0;
            this.dgvAbonos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvAbonos_KeyDown);
            // 
            // btnCrearDescuentosFacturas
            // 
            this.btnCrearDescuentosFacturas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCrearDescuentosFacturas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrearDescuentosFacturas.ForeColor = System.Drawing.Color.White;
            this.btnCrearDescuentosFacturas.Location = new System.Drawing.Point(661, 81);
            this.btnCrearDescuentosFacturas.Name = "btnCrearDescuentosFacturas";
            this.btnCrearDescuentosFacturas.Size = new System.Drawing.Size(115, 23);
            this.btnCrearDescuentosFacturas.TabIndex = 6;
            this.btnCrearDescuentosFacturas.Text = "Descuento Factura";
            this.btnCrearDescuentosFacturas.UseVisualStyleBackColor = false;
            this.btnCrearDescuentosFacturas.Click += new System.EventHandler(this.btnCrearDescuentosFacturas_Click);
            // 
            // btnCrearDescuento
            // 
            this.btnCrearDescuento.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCrearDescuento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrearDescuento.ForeColor = System.Drawing.Color.White;
            this.btnCrearDescuento.Location = new System.Drawing.Point(661, 110);
            this.btnCrearDescuento.Name = "btnCrearDescuento";
            this.btnCrearDescuento.Size = new System.Drawing.Size(115, 23);
            this.btnCrearDescuento.TabIndex = 7;
            this.btnCrearDescuento.Text = "Descuento Directo";
            this.btnCrearDescuento.UseVisualStyleBackColor = false;
            this.btnCrearDescuento.Click += new System.EventHandler(this.btnCrearDescuento_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(389, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Cuenta";
            // 
            // cmbCuentaBancaria
            // 
            this.cmbCuentaBancaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCuentaBancaria.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCuentaBancaria.FormattingEnabled = true;
            this.cmbCuentaBancaria.Location = new System.Drawing.Point(433, 14);
            this.cmbCuentaBancaria.Name = "cmbCuentaBancaria";
            this.cmbCuentaBancaria.Size = new System.Drawing.Size(189, 21);
            this.cmbCuentaBancaria.TabIndex = 1;
            // 
            // btnNotaDeCredito
            // 
            this.btnNotaDeCredito.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnNotaDeCredito.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotaDeCredito.ForeColor = System.Drawing.Color.White;
            this.btnNotaDeCredito.Location = new System.Drawing.Point(661, 139);
            this.btnNotaDeCredito.Name = "btnNotaDeCredito";
            this.btnNotaDeCredito.Size = new System.Drawing.Size(115, 23);
            this.btnNotaDeCredito.TabIndex = 8;
            this.btnNotaDeCredito.Text = "Nota de Crédito";
            this.btnNotaDeCredito.UseVisualStyleBackColor = false;
            this.btnNotaDeCredito.Click += new System.EventHandler(this.btnNotaDeCredito_Click);
            // 
            // btnPagoDeCaja
            // 
            this.btnPagoDeCaja.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnPagoDeCaja.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPagoDeCaja.ForeColor = System.Drawing.Color.White;
            this.btnPagoDeCaja.Location = new System.Drawing.Point(661, 168);
            this.btnPagoDeCaja.Name = "btnPagoDeCaja";
            this.btnPagoDeCaja.Size = new System.Drawing.Size(115, 23);
            this.btnPagoDeCaja.TabIndex = 14;
            this.btnPagoDeCaja.Text = "Pago de Caja";
            this.btnPagoDeCaja.UseVisualStyleBackColor = false;
            this.btnPagoDeCaja.Click += new System.EventHandler(this.btnPagoDeCaja_Click);
            // 
            // abo_ProveedorPolizaDetalleID
            // 
            this.abo_ProveedorPolizaDetalleID.HeaderText = "ProveedorPolizaDetalleID";
            this.abo_ProveedorPolizaDetalleID.Name = "abo_ProveedorPolizaDetalleID";
            this.abo_ProveedorPolizaDetalleID.ReadOnly = true;
            this.abo_ProveedorPolizaDetalleID.Visible = false;
            // 
            // abo_MovimientoInventarioID
            // 
            this.abo_MovimientoInventarioID.HeaderText = "MovimientoInventarioID";
            this.abo_MovimientoInventarioID.Name = "abo_MovimientoInventarioID";
            this.abo_MovimientoInventarioID.ReadOnly = true;
            this.abo_MovimientoInventarioID.Visible = false;
            // 
            // abo_OrigenID
            // 
            this.abo_OrigenID.HeaderText = "OrigenID";
            this.abo_OrigenID.Name = "abo_OrigenID";
            this.abo_OrigenID.ReadOnly = true;
            this.abo_OrigenID.Visible = false;
            // 
            // abo_CajaEgresoID
            // 
            this.abo_CajaEgresoID.HeaderText = "CajaEgresoID";
            this.abo_CajaEgresoID.Name = "abo_CajaEgresoID";
            this.abo_CajaEgresoID.ReadOnly = true;
            this.abo_CajaEgresoID.Visible = false;
            // 
            // abo_NotaDeCreditoID
            // 
            this.abo_NotaDeCreditoID.HeaderText = "NotaDeCreditoID";
            this.abo_NotaDeCreditoID.Name = "abo_NotaDeCreditoID";
            this.abo_NotaDeCreditoID.ReadOnly = true;
            this.abo_NotaDeCreditoID.Visible = false;
            // 
            // abo_Factura
            // 
            this.abo_Factura.HeaderText = "Factura";
            this.abo_Factura.Name = "abo_Factura";
            this.abo_Factura.ReadOnly = true;
            this.abo_Factura.Width = 80;
            // 
            // abo_Origen
            // 
            this.abo_Origen.HeaderText = "Origen";
            this.abo_Origen.Name = "abo_Origen";
            this.abo_Origen.ReadOnly = true;
            // 
            // abo_ImporteFactura
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.abo_ImporteFactura.DefaultCellStyle = dataGridViewCellStyle1;
            this.abo_ImporteFactura.HeaderText = "Imp. Fact.";
            this.abo_ImporteFactura.Name = "abo_ImporteFactura";
            this.abo_ImporteFactura.ReadOnly = true;
            this.abo_ImporteFactura.Width = 80;
            // 
            // abo_ImporteFinal
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            this.abo_ImporteFinal.DefaultCellStyle = dataGridViewCellStyle2;
            this.abo_ImporteFinal.HeaderText = "Imp. Final";
            this.abo_ImporteFinal.Name = "abo_ImporteFinal";
            this.abo_ImporteFinal.ReadOnly = true;
            this.abo_ImporteFinal.Width = 80;
            // 
            // abo_Importe
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "C2";
            this.abo_Importe.DefaultCellStyle = dataGridViewCellStyle3;
            this.abo_Importe.HeaderText = "Importe";
            this.abo_Importe.Name = "abo_Importe";
            this.abo_Importe.ReadOnly = true;
            this.abo_Importe.Width = 80;
            // 
            // abo_NotaDeCredito
            // 
            this.abo_NotaDeCredito.HeaderText = "N. C.";
            this.abo_NotaDeCredito.Name = "abo_NotaDeCredito";
            this.abo_NotaDeCredito.ReadOnly = true;
            // 
            // abo_Observacion
            // 
            this.abo_Observacion.HeaderText = "Observación";
            this.abo_Observacion.Name = "abo_Observacion";
            this.abo_Observacion.ReadOnly = true;
            this.abo_Observacion.Width = 200;
            // 
            // DetalleProveedorPoliza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(806, 612);
            this.KeyPreview = true;
            this.Name = "DetalleProveedorPoliza";
            this.Load += new System.EventHandler(this.DetalleProveedorPoliza_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleProveedorPoliza_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.gpoDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpFechaMovimiento;
        private System.Windows.Forms.Label lblFecha;
        private textBoxOnlyDouble txtImporte;
        private System.Windows.Forms.Label lblImporte;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBeneficiario;
        private System.Windows.Forms.Label lblBanco;
        private System.Windows.Forms.ComboBox cboBanco;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDocumento;
        private System.Windows.Forms.GroupBox gpoDetalle;
        private System.Windows.Forms.ComboBox cboFormaPago;
        private System.Windows.Forms.Label lblFormaPago;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvAbonos;
        private System.Windows.Forms.Button btnCrearDescuentosFacturas;
        private System.Windows.Forms.Button btnCrearDescuento;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbCuentaBancaria;
        private System.Windows.Forms.Button btnNotaDeCredito;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_MovimientoInventarioID;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_Factura;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_Abonado;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_Saldo;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_Descuento;
        private System.Windows.Forms.DataGridViewTextBoxColumn fac_Final;
        private System.Windows.Forms.Button btnPagoDeCaja;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_ProveedorPolizaDetalleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_MovimientoInventarioID;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_OrigenID;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_CajaEgresoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_NotaDeCreditoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_Factura;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_Origen;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_ImporteFactura;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_ImporteFinal;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_NotaDeCredito;
        private System.Windows.Forms.DataGridViewTextBoxColumn abo_Observacion;
    }
}
