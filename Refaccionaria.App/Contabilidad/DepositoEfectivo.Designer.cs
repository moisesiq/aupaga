namespace Refaccionaria.App
{
    partial class DepositoEfectivo
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAdv)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.Text = "Fecha del Sugerido";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(56, 89);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(69, 63);
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(115, 134);
            this.btnAceptar.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(196, 134);
            this.btnCancelar.UseVisualStyleBackColor = false;
            // 
            // lblEtImporteInfo
            // 
            this.lblEtImporteInfo.Location = new System.Drawing.Point(0, 10);
            this.lblEtImporteInfo.Size = new System.Drawing.Size(111, 13);
            this.lblEtImporteInfo.Text = "Resguardo Disponible";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Location = new System.Drawing.Point(115, 34);
            this.dtpFecha.Size = new System.Drawing.Size(106, 20);
            this.dtpFecha.ValueChanged += new System.EventHandler(this.dtpFecha_ValueChanged);
            // 
            // txtConcepto
            // 
            this.txtConcepto.Location = new System.Drawing.Point(115, 86);
            this.txtConcepto.Size = new System.Drawing.Size(156, 40);
            this.txtConcepto.Text = "DEPÓSITO EFECTIVO";
            // 
            // txtImporte
            // 
            this.txtImporte.Location = new System.Drawing.Point(115, 60);
            this.txtImporte.Size = new System.Drawing.Size(106, 20);
            this.txtImporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblImporteInfo
            // 
            this.lblImporteInfo.Location = new System.Drawing.Point(117, 10);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(242, 26);
            this.label1.Visible = false;
            // 
            // cmbBancoCuenta
            // 
            this.cmbBancoCuenta.Location = new System.Drawing.Point(245, 2);
            this.cmbBancoCuenta.Size = new System.Drawing.Size(38, 21);
            this.cmbBancoCuenta.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(15, 167);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(268, 52);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "Sumatoria de las Facturas pagadas en efectivo menos los tickets pagados con tarje" +
    "ta, cheque o transferencia, más la factura global del día.";
            // 
            // DepositoEfectivo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 222);
            this.Controls.Add(this.textBox1);
            this.Name = "DepositoEfectivo";
            this.Text = "Deposito efectivo";
            this.Load += new System.EventHandler(this.DepositoEfectivo_Load);
            this.Controls.SetChildIndex(this.textBox1, 0);
            this.Controls.SetChildIndex(this.lblEtImporteInfo, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.dtpFecha, 0);
            this.Controls.SetChildIndex(this.txtConcepto, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.txtImporte, 0);
            this.Controls.SetChildIndex(this.btnAceptar, 0);
            this.Controls.SetChildIndex(this.btnCancelar, 0);
            this.Controls.SetChildIndex(this.lblImporteInfo, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.cmbBancoCuenta, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAdv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
    }
}