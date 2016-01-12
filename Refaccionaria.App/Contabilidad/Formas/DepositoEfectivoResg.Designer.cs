namespace Refaccionaria.App
{
    partial class DepositoEfectivoResg
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
            this.lblImporteInfo = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.txtImporte = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtConcepto = new System.Windows.Forms.TextBox();
            this.dtpFechaSugerido = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.lblEtImporteInfo = new System.Windows.Forms.Label();
            this.dtpFechaMovimiento = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblImporteSugerido = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblImporteInfo
            // 
            this.lblImporteInfo.AutoSize = true;
            this.lblImporteInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImporteInfo.ForeColor = System.Drawing.Color.White;
            this.lblImporteInfo.Location = new System.Drawing.Point(123, 9);
            this.lblImporteInfo.Name = "lblImporteInfo";
            this.lblImporteInfo.Size = new System.Drawing.Size(39, 13);
            this.lblImporteInfo.TabIndex = 0;
            this.lblImporteInfo.Text = "$0.00";
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(209, 157);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(128, 157);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 6;
            this.btnAceptar.Text = "&Guardar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // txtImporte
            // 
            this.txtImporte.Location = new System.Drawing.Point(126, 59);
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.Size = new System.Drawing.Size(100, 20);
            this.txtImporte.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(75, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Importe";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(64, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Concepto";
            // 
            // txtConcepto
            // 
            this.txtConcepto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtConcepto.Location = new System.Drawing.Point(126, 85);
            this.txtConcepto.Multiline = true;
            this.txtConcepto.Name = "txtConcepto";
            this.txtConcepto.Size = new System.Drawing.Size(160, 40);
            this.txtConcepto.TabIndex = 4;
            this.txtConcepto.Text = "DEPÓSITO EFECTIVO";
            // 
            // dtpFechaSugerido
            // 
            this.dtpFechaSugerido.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaSugerido.Location = new System.Drawing.Point(126, 33);
            this.dtpFechaSugerido.Name = "dtpFechaSugerido";
            this.dtpFechaSugerido.Size = new System.Drawing.Size(100, 20);
            this.dtpFechaSugerido.TabIndex = 1;
            this.dtpFechaSugerido.ValueChanged += new System.EventHandler(this.dtpFechaSugerido_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(20, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Fecha del sugerido";
            // 
            // lblEtImporteInfo
            // 
            this.lblEtImporteInfo.AutoSize = true;
            this.lblEtImporteInfo.ForeColor = System.Drawing.Color.White;
            this.lblEtImporteInfo.Location = new System.Drawing.Point(8, 9);
            this.lblEtImporteInfo.Name = "lblEtImporteInfo";
            this.lblEtImporteInfo.Size = new System.Drawing.Size(109, 13);
            this.lblEtImporteInfo.TabIndex = 9;
            this.lblEtImporteInfo.Text = "Resguardo disponible";
            // 
            // dtpFechaMovimiento
            // 
            this.dtpFechaMovimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaMovimiento.Location = new System.Drawing.Point(126, 131);
            this.dtpFechaMovimiento.Name = "dtpFechaMovimiento";
            this.dtpFechaMovimiento.Size = new System.Drawing.Size(100, 20);
            this.dtpFechaMovimiento.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 134);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Fecha del movimiento";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(18, 186);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(268, 52);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "Sumatoria de las Facturas pagadas en efectivo menos los tickets pagados con tarje" +
    "ta, cheque o transferencia, más la factura global del día.";
            // 
            // lblImporteSugerido
            // 
            this.lblImporteSugerido.AutoSize = true;
            this.lblImporteSugerido.ForeColor = System.Drawing.Color.White;
            this.lblImporteSugerido.Location = new System.Drawing.Point(232, 36);
            this.lblImporteSugerido.Name = "lblImporteSugerido";
            this.lblImporteSugerido.Size = new System.Drawing.Size(34, 13);
            this.lblImporteSugerido.TabIndex = 3;
            this.lblImporteSugerido.Text = "$0.00";
            // 
            // DepositoEfectivoResg
            // 
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(298, 239);
            this.Controls.Add(this.lblImporteSugerido);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dtpFechaMovimiento);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblImporteInfo);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.txtImporte);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtConcepto);
            this.Controls.Add(this.dtpFechaSugerido);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblEtImporteInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DepositoEfectivoResg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Depósito efectivo";
            this.Load += new System.EventHandler(this.DepositoEfectivoResg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblImporteInfo;
        protected System.Windows.Forms.Button btnCancelar;
        protected System.Windows.Forms.Button btnAceptar;
        public System.Windows.Forms.TextBox txtImporte;
        protected System.Windows.Forms.Label label4;
        protected System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtConcepto;
        public System.Windows.Forms.DateTimePicker dtpFechaSugerido;
        protected System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label lblEtImporteInfo;
        public System.Windows.Forms.DateTimePicker dtpFechaMovimiento;
        protected System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblImporteSugerido;
    }
}