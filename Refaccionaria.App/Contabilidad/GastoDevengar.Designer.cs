namespace Refaccionaria.App
{
    partial class GastoDevengar
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtCuentaAuxiliar = new System.Windows.Forms.TextBox();
            this.txtConcepto = new System.Windows.Forms.TextBox();
            this.txtImporte = new System.Windows.Forms.TextBox();
            this.cmbSucursal = new System.Windows.Forms.ComboBox();
            this.txtImporteDev = new System.Windows.Forms.TextBox();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.ContaEgresoDevengadoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tienda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvTotales = new System.Windows.Forms.DataGridView();
            this.TotalTotales = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalImporte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.dgvEgresoDetalle = new System.Windows.Forms.DataGridView();
            this.ContaEgresoDetalleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContaConsumible = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Restante = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CantidadDev = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.egImporte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlParteAbajo = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEgresoDetalle)).BeginInit();
            this.pnlParteAbajo.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCuentaAuxiliar
            // 
            this.txtCuentaAuxiliar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCuentaAuxiliar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.txtCuentaAuxiliar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCuentaAuxiliar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCuentaAuxiliar.ForeColor = System.Drawing.Color.White;
            this.txtCuentaAuxiliar.Location = new System.Drawing.Point(12, 12);
            this.txtCuentaAuxiliar.Name = "txtCuentaAuxiliar";
            this.txtCuentaAuxiliar.ReadOnly = true;
            this.txtCuentaAuxiliar.Size = new System.Drawing.Size(486, 16);
            this.txtCuentaAuxiliar.TabIndex = 0;
            this.txtCuentaAuxiliar.Text = "Cuenta Auxiliar";
            // 
            // txtConcepto
            // 
            this.txtConcepto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConcepto.Location = new System.Drawing.Point(12, 34);
            this.txtConcepto.Name = "txtConcepto";
            this.txtConcepto.ReadOnly = true;
            this.txtConcepto.Size = new System.Drawing.Size(380, 20);
            this.txtConcepto.TabIndex = 1;
            // 
            // txtImporte
            // 
            this.txtImporte.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImporte.Location = new System.Drawing.Point(398, 34);
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.ReadOnly = true;
            this.txtImporte.Size = new System.Drawing.Size(100, 20);
            this.txtImporte.TabIndex = 2;
            this.txtImporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSucursal.FormattingEnabled = true;
            this.cmbSucursal.Location = new System.Drawing.Point(12, 60);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.Size = new System.Drawing.Size(121, 21);
            this.cmbSucursal.TabIndex = 3;
            // 
            // txtImporteDev
            // 
            this.txtImporteDev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImporteDev.Location = new System.Drawing.Point(312, 60);
            this.txtImporteDev.Name = "txtImporteDev";
            this.txtImporteDev.Size = new System.Drawing.Size(80, 20);
            this.txtImporteDev.TabIndex = 4;
            this.txtImporteDev.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // dtpFecha
            // 
            this.dtpFecha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(398, 60);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(100, 20);
            this.dtpFecha.TabIndex = 5;
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.AllowUserToResizeRows = false;
            this.dgvDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvDetalle.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ContaEgresoDevengadoID,
            this.Fecha,
            this.Tienda,
            this.Importe});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDetalle.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDetalle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDetalle.Location = new System.Drawing.Point(0, 0);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetalle.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(380, 105);
            this.dgvDetalle.StandardTab = true;
            this.dgvDetalle.TabIndex = 8;
            this.dgvDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDetalle_KeyDown);
            // 
            // ContaEgresoDevengadoID
            // 
            this.ContaEgresoDevengadoID.HeaderText = "ContaEgresoDevengadoID";
            this.ContaEgresoDevengadoID.Name = "ContaEgresoDevengadoID";
            this.ContaEgresoDevengadoID.ReadOnly = true;
            this.ContaEgresoDevengadoID.Visible = false;
            // 
            // Fecha
            // 
            this.Fecha.HeaderText = "Fecha";
            this.Fecha.Name = "Fecha";
            this.Fecha.ReadOnly = true;
            this.Fecha.Width = 136;
            // 
            // Tienda
            // 
            this.Tienda.HeaderText = "Tienda";
            this.Tienda.Name = "Tienda";
            this.Tienda.ReadOnly = true;
            this.Tienda.Width = 120;
            // 
            // Importe
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Importe.DefaultCellStyle = dataGridViewCellStyle1;
            this.Importe.HeaderText = "Importe";
            this.Importe.Name = "Importe";
            this.Importe.ReadOnly = true;
            // 
            // dgvTotales
            // 
            this.dgvTotales.AllowUserToAddRows = false;
            this.dgvTotales.AllowUserToDeleteRows = false;
            this.dgvTotales.AllowUserToResizeRows = false;
            this.dgvTotales.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTotales.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvTotales.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvTotales.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvTotales.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvTotales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTotales.ColumnHeadersVisible = false;
            this.dgvTotales.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TotalTotales,
            this.TotalImporte});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTotales.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvTotales.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvTotales.Location = new System.Drawing.Point(0, 106);
            this.dgvTotales.Name = "dgvTotales";
            this.dgvTotales.ReadOnly = true;
            this.dgvTotales.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTotales.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvTotales.RowHeadersVisible = false;
            this.dgvTotales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTotales.Size = new System.Drawing.Size(380, 20);
            this.dgvTotales.StandardTab = true;
            this.dgvTotales.TabIndex = 9;
            // 
            // TotalTotales
            // 
            this.TotalTotales.HeaderText = "TotalTotales";
            this.TotalTotales.Name = "TotalTotales";
            this.TotalTotales.ReadOnly = true;
            this.TotalTotales.Width = 256;
            // 
            // TotalImporte
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "C2";
            this.TotalImporte.DefaultCellStyle = dataGridViewCellStyle4;
            this.TotalImporte.HeaderText = "TotalImporte";
            this.TotalImporte.Name = "TotalImporte";
            this.TotalImporte.ReadOnly = true;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(396, 0);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(81, 23);
            this.btnGuardar.TabIndex = 6;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(396, 29);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(81, 23);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // dgvEgresoDetalle
            // 
            this.dgvEgresoDetalle.AllowUserToAddRows = false;
            this.dgvEgresoDetalle.AllowUserToDeleteRows = false;
            this.dgvEgresoDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEgresoDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvEgresoDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEgresoDetalle.CausesValidation = false;
            this.dgvEgresoDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvEgresoDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEgresoDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ContaEgresoDetalleID,
            this.ContaConsumible,
            this.Restante,
            this.CantidadDev,
            this.egImporte});
            this.dgvEgresoDetalle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvEgresoDetalle.Location = new System.Drawing.Point(12, 87);
            this.dgvEgresoDetalle.MultiSelect = false;
            this.dgvEgresoDetalle.Name = "dgvEgresoDetalle";
            this.dgvEgresoDetalle.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvEgresoDetalle.RowHeadersWidth = 40;
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            this.dgvEgresoDetalle.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvEgresoDetalle.Size = new System.Drawing.Size(486, 120);
            this.dgvEgresoDetalle.TabIndex = 10;
            this.dgvEgresoDetalle.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEgresoDetalle_CellValueChanged);
            // 
            // ContaEgresoDetalleID
            // 
            this.ContaEgresoDetalleID.HeaderText = "ContaEgresoDetalleID";
            this.ContaEgresoDetalleID.Name = "ContaEgresoDetalleID";
            this.ContaEgresoDetalleID.ReadOnly = true;
            this.ContaEgresoDetalleID.Visible = false;
            // 
            // ContaConsumible
            // 
            this.ContaConsumible.HeaderText = "Consumible";
            this.ContaConsumible.Name = "ContaConsumible";
            this.ContaConsumible.ReadOnly = true;
            this.ContaConsumible.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ContaConsumible.Width = 240;
            // 
            // Restante
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N0";
            this.Restante.DefaultCellStyle = dataGridViewCellStyle7;
            this.Restante.HeaderText = "Restante";
            this.Restante.Name = "Restante";
            this.Restante.ReadOnly = true;
            this.Restante.Width = 80;
            // 
            // CantidadDev
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N0";
            dataGridViewCellStyle8.NullValue = null;
            this.CantidadDev.DefaultCellStyle = dataGridViewCellStyle8;
            this.CantidadDev.HeaderText = "Cantidad";
            this.CantidadDev.Name = "CantidadDev";
            this.CantidadDev.Width = 80;
            // 
            // egImporte
            // 
            this.egImporte.HeaderText = "Importe";
            this.egImporte.Name = "egImporte";
            this.egImporte.ReadOnly = true;
            this.egImporte.Visible = false;
            // 
            // pnlParteAbajo
            // 
            this.pnlParteAbajo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlParteAbajo.Controls.Add(this.dgvDetalle);
            this.pnlParteAbajo.Controls.Add(this.dgvTotales);
            this.pnlParteAbajo.Controls.Add(this.btnCancelar);
            this.pnlParteAbajo.Controls.Add(this.btnGuardar);
            this.pnlParteAbajo.Location = new System.Drawing.Point(12, 213);
            this.pnlParteAbajo.Name = "pnlParteAbajo";
            this.pnlParteAbajo.Size = new System.Drawing.Size(486, 127);
            this.pnlParteAbajo.TabIndex = 11;
            // 
            // GastoDevengar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(511, 352);
            this.Controls.Add(this.pnlParteAbajo);
            this.Controls.Add(this.dgvEgresoDetalle);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.txtImporteDev);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.txtImporte);
            this.Controls.Add(this.txtConcepto);
            this.Controls.Add(this.txtCuentaAuxiliar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GastoDevengar";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Devengar Gasto";
            this.Load += new System.EventHandler(this.GastoDevengar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEgresoDetalle)).EndInit();
            this.pnlParteAbajo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCuentaAuxiliar;
        private System.Windows.Forms.TextBox txtConcepto;
        private System.Windows.Forms.TextBox txtImporte;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private System.Windows.Forms.TextBox txtImporteDev;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.DataGridView dgvTotales;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalTotales;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalImporte;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.DataGridView dgvEgresoDetalle;
        private System.Windows.Forms.Panel pnlParteAbajo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContaEgresoDevengadoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tienda;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContaEgresoDetalleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContaConsumible;
        private System.Windows.Forms.DataGridViewTextBoxColumn Restante;
        private System.Windows.Forms.DataGridViewTextBoxColumn CantidadDev;
        private System.Windows.Forms.DataGridViewTextBoxColumn egImporte;
    }
}