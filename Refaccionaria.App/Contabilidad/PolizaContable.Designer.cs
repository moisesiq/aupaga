namespace Refaccionaria.App
{
    partial class PolizaContable
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.cmbTipoPoliza = new System.Windows.Forms.ComboBox();
            this.txtConcepto = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvDetalle = new Refaccionaria.Negocio.GridEditable();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOrigen = new System.Windows.Forms.TextBox();
            this.dgvTotales = new System.Windows.Forms.DataGridView();
            this.tot_Titulo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tot_Cargo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tot_Abono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tot_Adicional = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContaCuentaAuxiliarID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CuentaContpaq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CuentaAuxiliar = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cargo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Abono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Referencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SucursalID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotales)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fecha";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(48, 6);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(100, 20);
            this.dtpFecha.TabIndex = 0;
            // 
            // cmbTipoPoliza
            // 
            this.cmbTipoPoliza.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbTipoPoliza.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTipoPoliza.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTipoPoliza.FormattingEnabled = true;
            this.cmbTipoPoliza.Location = new System.Drawing.Point(188, 6);
            this.cmbTipoPoliza.Name = "cmbTipoPoliza";
            this.cmbTipoPoliza.Size = new System.Drawing.Size(121, 21);
            this.cmbTipoPoliza.TabIndex = 1;
            // 
            // txtConcepto
            // 
            this.txtConcepto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtConcepto.Location = new System.Drawing.Point(374, 6);
            this.txtConcepto.Name = "txtConcepto";
            this.txtConcepto.Size = new System.Drawing.Size(293, 20);
            this.txtConcepto.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(154, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tipo";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(315, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Concepto";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalle.ColorBorrado = System.Drawing.Color.Gray;
            this.dgvDetalle.ColorModificado = System.Drawing.Color.Orange;
            this.dgvDetalle.ColorNuevo = System.Drawing.Color.Blue;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ContaCuentaAuxiliarID,
            this.CuentaContpaq,
            this.CuentaAuxiliar,
            this.Cargo,
            this.Abono,
            this.Referencia,
            this.SucursalID});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDetalle.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDetalle.Location = new System.Drawing.Point(8, 33);
            this.dgvDetalle.MultiSelect = false;
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.PermitirBorrar = true;
            this.dgvDetalle.Size = new System.Drawing.Size(689, 259);
            this.dgvDetalle.TabIndex = 3;
            this.dgvDetalle.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetalle_CellValueChanged);
            this.dgvDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDetalle_KeyDown);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(541, 322);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 6;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(622, 322);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(5, 327);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Origen";
            // 
            // txtOrigen
            // 
            this.txtOrigen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOrigen.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtOrigen.Location = new System.Drawing.Point(49, 324);
            this.txtOrigen.Name = "txtOrigen";
            this.txtOrigen.Size = new System.Drawing.Size(320, 20);
            this.txtOrigen.TabIndex = 5;
            // 
            // dgvTotales
            // 
            this.dgvTotales.AllowUserToAddRows = false;
            this.dgvTotales.AllowUserToDeleteRows = false;
            this.dgvTotales.AllowUserToOrderColumns = true;
            this.dgvTotales.AllowUserToResizeColumns = false;
            this.dgvTotales.AllowUserToResizeRows = false;
            this.dgvTotales.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTotales.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvTotales.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTotales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTotales.ColumnHeadersVisible = false;
            this.dgvTotales.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tot_Titulo,
            this.tot_Cargo,
            this.tot_Abono,
            this.tot_Adicional});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTotales.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvTotales.Location = new System.Drawing.Point(8, 298);
            this.dgvTotales.MultiSelect = false;
            this.dgvTotales.Name = "dgvTotales";
            this.dgvTotales.ReadOnly = true;
            this.dgvTotales.RowHeadersVisible = false;
            this.dgvTotales.Size = new System.Drawing.Size(689, 20);
            this.dgvTotales.TabIndex = 4;
            // 
            // tot_Titulo
            // 
            this.tot_Titulo.HeaderText = "Cuenta";
            this.tot_Titulo.Name = "tot_Titulo";
            this.tot_Titulo.ReadOnly = true;
            this.tot_Titulo.Width = 420;
            // 
            // tot_Cargo
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "C2";
            this.tot_Cargo.DefaultCellStyle = dataGridViewCellStyle4;
            this.tot_Cargo.HeaderText = "Cargo";
            this.tot_Cargo.Name = "tot_Cargo";
            this.tot_Cargo.ReadOnly = true;
            this.tot_Cargo.Width = 80;
            // 
            // tot_Abono
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "C2";
            this.tot_Abono.DefaultCellStyle = dataGridViewCellStyle5;
            this.tot_Abono.HeaderText = "Abono";
            this.tot_Abono.Name = "tot_Abono";
            this.tot_Abono.ReadOnly = true;
            this.tot_Abono.Width = 80;
            // 
            // tot_Adicional
            // 
            this.tot_Adicional.HeaderText = "Referencia";
            this.tot_Adicional.Name = "tot_Adicional";
            this.tot_Adicional.ReadOnly = true;
            this.tot_Adicional.Width = 80;
            // 
            // ContaCuentaAuxiliarID
            // 
            this.ContaCuentaAuxiliarID.HeaderText = "ContaCuentaAuxiliarID";
            this.ContaCuentaAuxiliarID.Name = "ContaCuentaAuxiliarID";
            this.ContaCuentaAuxiliarID.Visible = false;
            // 
            // CuentaContpaq
            // 
            this.CuentaContpaq.HeaderText = "Contpaq";
            this.CuentaContpaq.Name = "CuentaContpaq";
            this.CuentaContpaq.ReadOnly = true;
            this.CuentaContpaq.Width = 80;
            // 
            // CuentaAuxiliar
            // 
            this.CuentaAuxiliar.HeaderText = "Cuenta";
            this.CuentaAuxiliar.Name = "CuentaAuxiliar";
            this.CuentaAuxiliar.ReadOnly = true;
            this.CuentaAuxiliar.Width = 300;
            // 
            // Cargo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Cargo.DefaultCellStyle = dataGridViewCellStyle1;
            this.Cargo.HeaderText = "Cargo";
            this.Cargo.Name = "Cargo";
            this.Cargo.Width = 80;
            // 
            // Abono
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            this.Abono.DefaultCellStyle = dataGridViewCellStyle2;
            this.Abono.HeaderText = "Abono";
            this.Abono.Name = "Abono";
            this.Abono.Width = 80;
            // 
            // Referencia
            // 
            this.Referencia.HeaderText = "Referencia";
            this.Referencia.Name = "Referencia";
            this.Referencia.Width = 80;
            // 
            // SucursalID
            // 
            this.SucursalID.HeaderText = "SucursalID";
            this.SucursalID.Name = "SucursalID";
            this.SucursalID.Width = 80;
            // 
            // PolizaContable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(704, 349);
            this.Controls.Add(this.dgvTotales);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOrigen);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtConcepto);
            this.Controls.Add(this.cmbTipoPoliza);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.label1);
            this.MinimizeBox = false;
            this.Name = "PolizaContable";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Póliza Contable";
            this.Load += new System.EventHandler(this.PolizaContable_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotales)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.ComboBox cmbTipoPoliza;
        private System.Windows.Forms.TextBox txtConcepto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        protected Negocio.GridEditable dgvDetalle;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOrigen;
        private System.Windows.Forms.DataGridView dgvTotales;
        private System.Windows.Forms.DataGridViewTextBoxColumn tot_Titulo;
        private System.Windows.Forms.DataGridViewTextBoxColumn tot_Cargo;
        private System.Windows.Forms.DataGridViewTextBoxColumn tot_Abono;
        private System.Windows.Forms.DataGridViewTextBoxColumn tot_Adicional;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContaCuentaAuxiliarID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CuentaContpaq;
        private System.Windows.Forms.DataGridViewTextBoxColumn CuentaAuxiliar;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cargo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Abono;
        private System.Windows.Forms.DataGridViewTextBoxColumn Referencia;
        private System.Windows.Forms.DataGridViewComboBoxColumn SucursalID;
    }
}