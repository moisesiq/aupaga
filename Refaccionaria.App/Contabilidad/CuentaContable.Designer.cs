namespace Refaccionaria.App
{
    partial class CuentaContable
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCuenta = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCuentaContpaq = new System.Windows.Forms.TextBox();
            this.chkTieneDetalle = new System.Windows.Forms.CheckBox();
            this.chkVisibleEnCaja = new System.Windows.Forms.CheckBox();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.pnlAdicional = new System.Windows.Forms.Panel();
            this.chkSumaGastosFijos = new System.Windows.Forms.CheckBox();
            this.chkAfectaMetas = new System.Windows.Forms.CheckBox();
            this.dtpDejarDeSemanalizar = new System.Windows.Forms.DateTimePicker();
            this.chkDejarDeSemanalizar = new System.Windows.Forms.CheckBox();
            this.txtMeses = new System.Windows.Forms.TextBox();
            this.lblDiasMovimiento = new System.Windows.Forms.Label();
            this.chkCalculoSemanal = new System.Windows.Forms.CheckBox();
            this.dgvDevSuc = new System.Windows.Forms.DataGridView();
            this.SucursalID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sucursal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Porcentaje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkDevengarAutomaticamente = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCuentaSat = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlAdicionalCuentaDeMayor = new System.Windows.Forms.Panel();
            this.chkRestaInversa = new System.Windows.Forms.CheckBox();
            this.chkDevengarEspecial = new System.Windows.Forms.CheckBox();
            this.cmbDevengarEspecial = new System.Windows.Forms.ComboBox();
            this.pnlAdicional.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevSuc)).BeginInit();
            this.pnlAdicionalCuentaDeMayor.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre";
            // 
            // txtCuenta
            // 
            this.txtCuenta.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCuenta.Location = new System.Drawing.Point(102, 6);
            this.txtCuenta.Name = "txtCuenta";
            this.txtCuenta.Size = new System.Drawing.Size(200, 20);
            this.txtCuenta.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Cuenta Contpaq";
            // 
            // txtCuentaContpaq
            // 
            this.txtCuentaContpaq.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCuentaContpaq.Location = new System.Drawing.Point(102, 32);
            this.txtCuentaContpaq.Name = "txtCuentaContpaq";
            this.txtCuentaContpaq.Size = new System.Drawing.Size(200, 20);
            this.txtCuentaContpaq.TabIndex = 1;
            // 
            // chkTieneDetalle
            // 
            this.chkTieneDetalle.AutoSize = true;
            this.chkTieneDetalle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkTieneDetalle.ForeColor = System.Drawing.Color.White;
            this.chkTieneDetalle.Location = new System.Drawing.Point(87, 3);
            this.chkTieneDetalle.Name = "chkTieneDetalle";
            this.chkTieneDetalle.Size = new System.Drawing.Size(12, 11);
            this.chkTieneDetalle.TabIndex = 0;
            this.chkTieneDetalle.UseVisualStyleBackColor = true;
            // 
            // chkVisibleEnCaja
            // 
            this.chkVisibleEnCaja.AutoSize = true;
            this.chkVisibleEnCaja.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkVisibleEnCaja.ForeColor = System.Drawing.Color.White;
            this.chkVisibleEnCaja.Location = new System.Drawing.Point(87, 23);
            this.chkVisibleEnCaja.Name = "chkVisibleEnCaja";
            this.chkVisibleEnCaja.Size = new System.Drawing.Size(12, 11);
            this.chkVisibleEnCaja.TabIndex = 1;
            this.chkVisibleEnCaja.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(225, 366);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(81, 23);
            this.btnCancelar.TabIndex = 4;
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
            this.btnGuardar.Location = new System.Drawing.Point(138, 366);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(81, 23);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // pnlAdicional
            // 
            this.pnlAdicional.Controls.Add(this.cmbDevengarEspecial);
            this.pnlAdicional.Controls.Add(this.chkDevengarEspecial);
            this.pnlAdicional.Controls.Add(this.chkSumaGastosFijos);
            this.pnlAdicional.Controls.Add(this.chkAfectaMetas);
            this.pnlAdicional.Controls.Add(this.dtpDejarDeSemanalizar);
            this.pnlAdicional.Controls.Add(this.chkDejarDeSemanalizar);
            this.pnlAdicional.Controls.Add(this.txtMeses);
            this.pnlAdicional.Controls.Add(this.lblDiasMovimiento);
            this.pnlAdicional.Controls.Add(this.chkCalculoSemanal);
            this.pnlAdicional.Controls.Add(this.dgvDevSuc);
            this.pnlAdicional.Controls.Add(this.chkDevengarAutomaticamente);
            this.pnlAdicional.Controls.Add(this.label3);
            this.pnlAdicional.Controls.Add(this.chkTieneDetalle);
            this.pnlAdicional.Controls.Add(this.label4);
            this.pnlAdicional.Controls.Add(this.chkVisibleEnCaja);
            this.pnlAdicional.Location = new System.Drawing.Point(15, 87);
            this.pnlAdicional.Name = "pnlAdicional";
            this.pnlAdicional.Size = new System.Drawing.Size(287, 273);
            this.pnlAdicional.TabIndex = 2;
            this.pnlAdicional.Visible = false;
            // 
            // chkSumaGastosFijos
            // 
            this.chkSumaGastosFijos.AutoSize = true;
            this.chkSumaGastosFijos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSumaGastosFijos.ForeColor = System.Drawing.Color.White;
            this.chkSumaGastosFijos.Location = new System.Drawing.Point(149, 21);
            this.chkSumaGastosFijos.Name = "chkSumaGastosFijos";
            this.chkSumaGastosFijos.Size = new System.Drawing.Size(125, 17);
            this.chkSumaGastosFijos.TabIndex = 3;
            this.chkSumaGastosFijos.Text = "Suma en Gastos Fijos";
            this.chkSumaGastosFijos.UseVisualStyleBackColor = true;
            // 
            // chkAfectaMetas
            // 
            this.chkAfectaMetas.AutoSize = true;
            this.chkAfectaMetas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAfectaMetas.ForeColor = System.Drawing.Color.White;
            this.chkAfectaMetas.Location = new System.Drawing.Point(149, 1);
            this.chkAfectaMetas.Name = "chkAfectaMetas";
            this.chkAfectaMetas.Size = new System.Drawing.Size(86, 17);
            this.chkAfectaMetas.TabIndex = 2;
            this.chkAfectaMetas.Text = "Afecta Metas";
            this.chkAfectaMetas.UseVisualStyleBackColor = true;
            // 
            // dtpDejarDeSemanalizar
            // 
            this.dtpDejarDeSemanalizar.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDejarDeSemanalizar.Location = new System.Drawing.Point(153, 250);
            this.dtpDejarDeSemanalizar.Name = "dtpDejarDeSemanalizar";
            this.dtpDejarDeSemanalizar.Size = new System.Drawing.Size(124, 20);
            this.dtpDejarDeSemanalizar.TabIndex = 10;
            // 
            // chkDejarDeSemanalizar
            // 
            this.chkDejarDeSemanalizar.AutoSize = true;
            this.chkDejarDeSemanalizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDejarDeSemanalizar.ForeColor = System.Drawing.Color.White;
            this.chkDejarDeSemanalizar.Location = new System.Drawing.Point(23, 250);
            this.chkDejarDeSemanalizar.Name = "chkDejarDeSemanalizar";
            this.chkDejarDeSemanalizar.Size = new System.Drawing.Size(121, 17);
            this.chkDejarDeSemanalizar.TabIndex = 9;
            this.chkDejarDeSemanalizar.Text = "Dejar de semanalizar";
            this.chkDejarDeSemanalizar.UseVisualStyleBackColor = true;
            this.chkDejarDeSemanalizar.CheckedChanged += new System.EventHandler(this.chkDejarDeSemanalizar_CheckedChanged);
            // 
            // txtMeses
            // 
            this.txtMeses.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMeses.Location = new System.Drawing.Point(185, 224);
            this.txtMeses.Name = "txtMeses";
            this.txtMeses.Size = new System.Drawing.Size(92, 20);
            this.txtMeses.TabIndex = 8;
            // 
            // lblDiasMovimiento
            // 
            this.lblDiasMovimiento.AutoSize = true;
            this.lblDiasMovimiento.ForeColor = System.Drawing.Color.White;
            this.lblDiasMovimiento.Location = new System.Drawing.Point(20, 227);
            this.lblDiasMovimiento.Name = "lblDiasMovimiento";
            this.lblDiasMovimiento.Size = new System.Drawing.Size(159, 13);
            this.lblDiasMovimiento.TabIndex = 7;
            this.lblDiasMovimiento.Text = "¿Cada cuántos meses se paga?";
            // 
            // chkCalculoSemanal
            // 
            this.chkCalculoSemanal.AutoSize = true;
            this.chkCalculoSemanal.Checked = true;
            this.chkCalculoSemanal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCalculoSemanal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkCalculoSemanal.ForeColor = System.Drawing.Color.White;
            this.chkCalculoSemanal.Location = new System.Drawing.Point(3, 202);
            this.chkCalculoSemanal.Name = "chkCalculoSemanal";
            this.chkCalculoSemanal.Size = new System.Drawing.Size(134, 17);
            this.chkCalculoSemanal.TabIndex = 6;
            this.chkCalculoSemanal.Text = "Aplicar cálculo semanal";
            this.chkCalculoSemanal.UseVisualStyleBackColor = true;
            this.chkCalculoSemanal.CheckedChanged += new System.EventHandler(this.chkCalculoSemanal_CheckedChanged);
            // 
            // dgvDevSuc
            // 
            this.dgvDevSuc.AllowUserToAddRows = false;
            this.dgvDevSuc.AllowUserToDeleteRows = false;
            this.dgvDevSuc.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvDevSuc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDevSuc.CausesValidation = false;
            this.dgvDevSuc.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvDevSuc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDevSuc.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SucursalID,
            this.Sucursal,
            this.Porcentaje});
            this.dgvDevSuc.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDevSuc.Location = new System.Drawing.Point(23, 64);
            this.dgvDevSuc.MultiSelect = false;
            this.dgvDevSuc.Name = "dgvDevSuc";
            this.dgvDevSuc.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvDevSuc.RowHeadersVisible = false;
            this.dgvDevSuc.RowHeadersWidth = 40;
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            this.dgvDevSuc.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvDevSuc.Size = new System.Drawing.Size(254, 82);
            this.dgvDevSuc.TabIndex = 5;
            // 
            // SucursalID
            // 
            this.SucursalID.HeaderText = "SucursalID";
            this.SucursalID.Name = "SucursalID";
            this.SucursalID.ReadOnly = true;
            this.SucursalID.Visible = false;
            // 
            // Sucursal
            // 
            this.Sucursal.HeaderText = "Sucursal";
            this.Sucursal.Name = "Sucursal";
            this.Sucursal.ReadOnly = true;
            // 
            // Porcentaje
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            dataGridViewCellStyle5.NullValue = null;
            this.Porcentaje.DefaultCellStyle = dataGridViewCellStyle5;
            this.Porcentaje.HeaderText = "Porcentaje %";
            this.Porcentaje.Name = "Porcentaje";
            // 
            // chkDevengarAutomaticamente
            // 
            this.chkDevengarAutomaticamente.AutoSize = true;
            this.chkDevengarAutomaticamente.Checked = true;
            this.chkDevengarAutomaticamente.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDevengarAutomaticamente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDevengarAutomaticamente.ForeColor = System.Drawing.Color.White;
            this.chkDevengarAutomaticamente.Location = new System.Drawing.Point(3, 41);
            this.chkDevengarAutomaticamente.Name = "chkDevengarAutomaticamente";
            this.chkDevengarAutomaticamente.Size = new System.Drawing.Size(154, 17);
            this.chkDevengarAutomaticamente.TabIndex = 4;
            this.chkDevengarAutomaticamente.Text = "Devengar automáticamente";
            this.chkDevengarAutomaticamente.UseVisualStyleBackColor = true;
            this.chkDevengarAutomaticamente.CheckedChanged += new System.EventHandler(this.chkDevengarAutomaticamente_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(-3, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Visible en caja";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(-3, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Tiene detalle";
            // 
            // txtCuentaSat
            // 
            this.txtCuentaSat.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCuentaSat.Location = new System.Drawing.Point(102, 58);
            this.txtCuentaSat.Name = "txtCuentaSat";
            this.txtCuentaSat.Size = new System.Drawing.Size(200, 20);
            this.txtCuentaSat.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(12, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Cuenta Sat";
            // 
            // pnlAdicionalCuentaDeMayor
            // 
            this.pnlAdicionalCuentaDeMayor.Controls.Add(this.chkRestaInversa);
            this.pnlAdicionalCuentaDeMayor.Location = new System.Drawing.Point(15, 365);
            this.pnlAdicionalCuentaDeMayor.Name = "pnlAdicionalCuentaDeMayor";
            this.pnlAdicionalCuentaDeMayor.Size = new System.Drawing.Size(116, 24);
            this.pnlAdicionalCuentaDeMayor.TabIndex = 7;
            this.pnlAdicionalCuentaDeMayor.Visible = false;
            // 
            // chkRestaInversa
            // 
            this.chkRestaInversa.AutoSize = true;
            this.chkRestaInversa.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRestaInversa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkRestaInversa.ForeColor = System.Drawing.Color.White;
            this.chkRestaInversa.Location = new System.Drawing.Point(-5, 0);
            this.chkRestaInversa.Name = "chkRestaInversa";
            this.chkRestaInversa.Size = new System.Drawing.Size(103, 17);
            this.chkRestaInversa.TabIndex = 13;
            this.chkRestaInversa.Text = "Resta inversar    ";
            this.chkRestaInversa.UseVisualStyleBackColor = true;
            // 
            // chkDevengarEspecial
            // 
            this.chkDevengarEspecial.AutoSize = true;
            this.chkDevengarEspecial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDevengarEspecial.ForeColor = System.Drawing.Color.White;
            this.chkDevengarEspecial.Location = new System.Drawing.Point(3, 152);
            this.chkDevengarEspecial.Name = "chkDevengarEspecial";
            this.chkDevengarEspecial.Size = new System.Drawing.Size(112, 17);
            this.chkDevengarEspecial.TabIndex = 13;
            this.chkDevengarEspecial.Text = "Devengar especial";
            this.chkDevengarEspecial.UseVisualStyleBackColor = true;
            this.chkDevengarEspecial.CheckedChanged += new System.EventHandler(this.chkDevengarEspecial_CheckedChanged);
            // 
            // cmbDevengarEspecial
            // 
            this.cmbDevengarEspecial.FormattingEnabled = true;
            this.cmbDevengarEspecial.Location = new System.Drawing.Point(23, 175);
            this.cmbDevengarEspecial.Name = "cmbDevengarEspecial";
            this.cmbDevengarEspecial.Size = new System.Drawing.Size(121, 21);
            this.cmbDevengarEspecial.TabIndex = 14;
            // 
            // CuentaContable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(318, 394);
            this.Controls.Add(this.pnlAdicionalCuentaDeMayor);
            this.Controls.Add(this.txtCuentaSat);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pnlAdicional);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.txtCuentaContpaq);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCuenta);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CuentaContable";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cuenta";
            this.Load += new System.EventHandler(this.CuentaContable_Load);
            this.pnlAdicional.ResumeLayout(false);
            this.pnlAdicional.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevSuc)).EndInit();
            this.pnlAdicionalCuentaDeMayor.ResumeLayout(false);
            this.pnlAdicionalCuentaDeMayor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCuenta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCuentaContpaq;
        private System.Windows.Forms.CheckBox chkTieneDetalle;
        private System.Windows.Forms.CheckBox chkVisibleEnCaja;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Panel pnlAdicional;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDevengarAutomaticamente;
        private System.Windows.Forms.CheckBox chkCalculoSemanal;
        private System.Windows.Forms.DataGridView dgvDevSuc;
        private System.Windows.Forms.Label lblDiasMovimiento;
        private System.Windows.Forms.DataGridViewTextBoxColumn SucursalID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sucursal;
        private System.Windows.Forms.DataGridViewTextBoxColumn Porcentaje;
        private System.Windows.Forms.TextBox txtMeses;
        private System.Windows.Forms.DateTimePicker dtpDejarDeSemanalizar;
        private System.Windows.Forms.CheckBox chkDejarDeSemanalizar;
        private System.Windows.Forms.CheckBox chkSumaGastosFijos;
        private System.Windows.Forms.CheckBox chkAfectaMetas;
        private System.Windows.Forms.TextBox txtCuentaSat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlAdicionalCuentaDeMayor;
        private System.Windows.Forms.CheckBox chkRestaInversa;
        private System.Windows.Forms.ComboBox cmbDevengarEspecial;
        private System.Windows.Forms.CheckBox chkDevengarEspecial;
    }
}