namespace Refaccionaria.App
{
    partial class GastoContable
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCuentaFinal = new System.Windows.Forms.ComboBox();
            this.cmbCuentaSubcuenta = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCuentaDeMayor = new System.Windows.Forms.ComboBox();
            this.cmbCuentaAuxiliar = new System.Windows.Forms.ComboBox();
            this.cmbDocumento = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.cmbFormaDePago = new System.Windows.Forms.ComboBox();
            this.txtImporte = new System.Windows.Forms.TextBox();
            this.txtFolioDePago = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this._Cambio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContaConsumibleID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Precio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkEsFiscal = new System.Windows.Forms.CheckBox();
            this.txtObservaciones = new Refaccionaria.Negocio.TextoMod();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.cmbCuentaBancaria = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtFolioFactura = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cuenta";
            // 
            // cmbCuentaFinal
            // 
            this.cmbCuentaFinal.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCuentaFinal.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCuentaFinal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCuentaFinal.FormattingEnabled = true;
            this.cmbCuentaFinal.Location = new System.Drawing.Point(6, 16);
            this.cmbCuentaFinal.Name = "cmbCuentaFinal";
            this.cmbCuentaFinal.Size = new System.Drawing.Size(236, 21);
            this.cmbCuentaFinal.TabIndex = 3;
            this.cmbCuentaFinal.SelectedIndexChanged += new System.EventHandler(this.cmbCuentaFinal_SelectedIndexChanged);
            // 
            // cmbCuentaSubcuenta
            // 
            this.cmbCuentaSubcuenta.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCuentaSubcuenta.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCuentaSubcuenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCuentaSubcuenta.FormattingEnabled = true;
            this.cmbCuentaSubcuenta.Location = new System.Drawing.Point(62, 43);
            this.cmbCuentaSubcuenta.Name = "cmbCuentaSubcuenta";
            this.cmbCuentaSubcuenta.Size = new System.Drawing.Size(180, 21);
            this.cmbCuentaSubcuenta.TabIndex = 0;
            this.cmbCuentaSubcuenta.SelectedIndexChanged += new System.EventHandler(this.cmbCuentaSubcuenta_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "C. Mayor";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "C. Auxiliar";
            // 
            // cmbCuentaDeMayor
            // 
            this.cmbCuentaDeMayor.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCuentaDeMayor.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCuentaDeMayor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCuentaDeMayor.FormattingEnabled = true;
            this.cmbCuentaDeMayor.Location = new System.Drawing.Point(62, 70);
            this.cmbCuentaDeMayor.Name = "cmbCuentaDeMayor";
            this.cmbCuentaDeMayor.Size = new System.Drawing.Size(180, 21);
            this.cmbCuentaDeMayor.TabIndex = 1;
            this.cmbCuentaDeMayor.SelectedIndexChanged += new System.EventHandler(this.cmbCuentaDeMayor_SelectedIndexChanged);
            // 
            // cmbCuentaAuxiliar
            // 
            this.cmbCuentaAuxiliar.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCuentaAuxiliar.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCuentaAuxiliar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCuentaAuxiliar.FormattingEnabled = true;
            this.cmbCuentaAuxiliar.Location = new System.Drawing.Point(62, 97);
            this.cmbCuentaAuxiliar.Name = "cmbCuentaAuxiliar";
            this.cmbCuentaAuxiliar.Size = new System.Drawing.Size(180, 21);
            this.cmbCuentaAuxiliar.TabIndex = 2;
            this.cmbCuentaAuxiliar.SelectedIndexChanged += new System.EventHandler(this.cmbCuentaAuxiliar_SelectedIndexChanged);
            // 
            // cmbDocumento
            // 
            this.cmbDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDocumento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDocumento.FormattingEnabled = true;
            this.cmbDocumento.Location = new System.Drawing.Point(328, 37);
            this.cmbDocumento.Name = "cmbDocumento";
            this.cmbDocumento.Size = new System.Drawing.Size(180, 21);
            this.cmbDocumento.TabIndex = 2;
            this.cmbDocumento.SelectedIndexChanged += new System.EventHandler(this.cmbDocumento_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(260, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Documento";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(260, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "F. de Pago";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(506, 6);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(105, 20);
            this.dtpFecha.TabIndex = 0;
            // 
            // cmbFormaDePago
            // 
            this.cmbFormaDePago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormaDePago.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFormaDePago.FormattingEnabled = true;
            this.cmbFormaDePago.Location = new System.Drawing.Point(328, 66);
            this.cmbFormaDePago.Name = "cmbFormaDePago";
            this.cmbFormaDePago.Size = new System.Drawing.Size(114, 21);
            this.cmbFormaDePago.TabIndex = 3;
            this.cmbFormaDePago.SelectedIndexChanged += new System.EventHandler(this.cmbFormaDePago_SelectedIndexChanged);
            // 
            // txtImporte
            // 
            this.txtImporte.Location = new System.Drawing.Point(328, 93);
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.Size = new System.Drawing.Size(100, 20);
            this.txtImporte.TabIndex = 5;
            this.txtImporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtFolioDePago
            // 
            this.txtFolioDePago.Enabled = false;
            this.txtFolioDePago.Location = new System.Drawing.Point(448, 66);
            this.txtFolioDePago.Name = "txtFolioDePago";
            this.txtFolioDePago.Size = new System.Drawing.Size(60, 20);
            this.txtFolioDePago.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(260, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Importe";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(463, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Fecha";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbCuentaFinal);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbCuentaSubcuenta);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbCuentaDeMayor);
            this.groupBox1.Controls.Add(this.cmbCuentaAuxiliar);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(6, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 124);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cuenta Auxiliar";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalle.CausesValidation = false;
            this.dgvDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._Cambio,
            this.ContaConsumibleID,
            this.Cantidad,
            this.Precio,
            this.Importe});
            this.dgvDetalle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDetalle.Location = new System.Drawing.Point(6, 198);
            this.dgvDetalle.MultiSelect = false;
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvDetalle.RowHeadersWidth = 40;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            this.dgvDetalle.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDetalle.Size = new System.Drawing.Size(605, 141);
            this.dgvDetalle.TabIndex = 8;
            this.dgvDetalle.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetalle_CellValueChanged);
            this.dgvDetalle.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvDetalle_RowsAdded);
            this.dgvDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDetalle_KeyDown);
            // 
            // _Cambio
            // 
            this._Cambio.HeaderText = "Cambio";
            this._Cambio.Name = "_Cambio";
            this._Cambio.ReadOnly = true;
            this._Cambio.Visible = false;
            // 
            // ContaConsumibleID
            // 
            this.ContaConsumibleID.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.ContaConsumibleID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ContaConsumibleID.HeaderText = "Consumible";
            this.ContaConsumibleID.Name = "ContaConsumibleID";
            this.ContaConsumibleID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ContaConsumibleID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ContaConsumibleID.Width = 300;
            // 
            // Cantidad
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.Cantidad.DefaultCellStyle = dataGridViewCellStyle1;
            this.Cantidad.HeaderText = "Cantidad";
            this.Cantidad.Name = "Cantidad";
            this.Cantidad.Width = 60;
            // 
            // Precio
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            dataGridViewCellStyle2.NullValue = null;
            this.Precio.DefaultCellStyle = dataGridViewCellStyle2;
            this.Precio.HeaderText = "Precio";
            this.Precio.Name = "Precio";
            this.Precio.Width = 80;
            // 
            // Importe
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "C2";
            this.Importe.DefaultCellStyle = dataGridViewCellStyle3;
            this.Importe.HeaderText = "Importe";
            this.Importe.Name = "Importe";
            this.Importe.ReadOnly = true;
            this.Importe.Width = 80;
            // 
            // chkEsFiscal
            // 
            this.chkEsFiscal.AutoSize = true;
            this.chkEsFiscal.Enabled = false;
            this.chkEsFiscal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEsFiscal.Location = new System.Drawing.Point(328, 119);
            this.chkEsFiscal.Name = "chkEsFiscal";
            this.chkEsFiscal.Size = new System.Drawing.Size(65, 17);
            this.chkEsFiscal.TabIndex = 6;
            this.chkEsFiscal.Text = "Es Fiscal";
            this.chkEsFiscal.UseVisualStyleBackColor = true;
            // 
            // txtObservaciones
            // 
            this.txtObservaciones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObservaciones.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtObservaciones.Etiqueta = "";
            this.txtObservaciones.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtObservaciones.Location = new System.Drawing.Point(93, 148);
            this.txtObservaciones.Multiline = true;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.PasarEnfoqueConEnter = true;
            this.txtObservaciones.SeleccionarTextoAlEnfoque = false;
            this.txtObservaciones.Size = new System.Drawing.Size(518, 44);
            this.txtObservaciones.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Observaciones";
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(530, 345);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(81, 23);
            this.btnCancelar.TabIndex = 21;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(443, 345);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(81, 23);
            this.btnGuardar.TabIndex = 20;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // cmbCuentaBancaria
            // 
            this.cmbCuentaBancaria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCuentaBancaria.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCuentaBancaria.FormattingEnabled = true;
            this.cmbCuentaBancaria.Location = new System.Drawing.Point(497, 115);
            this.cmbCuentaBancaria.Name = "cmbCuentaBancaria";
            this.cmbCuentaBancaria.Size = new System.Drawing.Size(114, 21);
            this.cmbCuentaBancaria.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(429, 118);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "Cuenta";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(443, 96);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Factura";
            // 
            // txtFolioFactura
            // 
            this.txtFolioFactura.Location = new System.Drawing.Point(511, 93);
            this.txtFolioFactura.Name = "txtFolioFactura";
            this.txtFolioFactura.Size = new System.Drawing.Size(100, 20);
            this.txtFolioFactura.TabIndex = 24;
            this.txtFolioFactura.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GastoContable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtFolioFactura);
            this.Controls.Add(this.cmbCuentaBancaria);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtObservaciones);
            this.Controls.Add(this.chkEsFiscal);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtFolioDePago);
            this.Controls.Add(this.txtImporte);
            this.Controls.Add(this.cmbFormaDePago);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbDocumento);
            this.Controls.Add(this.label4);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "GastoContable";
            this.Size = new System.Drawing.Size(616, 373);
            this.Load += new System.EventHandler(this.GastoContable_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbDocumento;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.ComboBox cmbFormaDePago;
        private System.Windows.Forms.TextBox txtImporte;
        private System.Windows.Forms.TextBox txtFolioDePago;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.CheckBox chkEsFiscal;
        private Negocio.TextoMod txtObservaciones;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.DataGridViewTextBoxColumn _Cambio;
        private System.Windows.Forms.DataGridViewComboBoxColumn ContaConsumibleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn Precio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
        private System.Windows.Forms.ComboBox cmbCuentaFinal;
        private System.Windows.Forms.ComboBox cmbCuentaSubcuenta;
        private System.Windows.Forms.ComboBox cmbCuentaDeMayor;
        private System.Windows.Forms.ComboBox cmbCuentaAuxiliar;
        private System.Windows.Forms.ComboBox cmbCuentaBancaria;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtFolioFactura;
    }
}
