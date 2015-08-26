namespace Refaccionaria.App
{
    partial class GastoCajaAPoliza
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
            this.rdbFactura = new System.Windows.Forms.RadioButton();
            this.rdbNota = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtImporte = new System.Windows.Forms.TextBox();
            this.gpbFactura = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIva = new System.Windows.Forms.TextBox();
            this.txtSubtotal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFolio = new System.Windows.Forms.TextBox();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.gpbFactura.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdbFactura
            // 
            this.rdbFactura.AutoSize = true;
            this.rdbFactura.ForeColor = System.Drawing.Color.White;
            this.rdbFactura.Location = new System.Drawing.Point(6, 19);
            this.rdbFactura.Name = "rdbFactura";
            this.rdbFactura.Size = new System.Drawing.Size(61, 17);
            this.rdbFactura.TabIndex = 0;
            this.rdbFactura.TabStop = true;
            this.rdbFactura.Text = "Factura";
            this.rdbFactura.UseVisualStyleBackColor = true;
            this.rdbFactura.CheckedChanged += new System.EventHandler(this.rdbFactura_CheckedChanged);
            // 
            // rdbNota
            // 
            this.rdbNota.AutoSize = true;
            this.rdbNota.ForeColor = System.Drawing.Color.White;
            this.rdbNota.Location = new System.Drawing.Point(73, 19);
            this.rdbNota.Name = "rdbNota";
            this.rdbNota.Size = new System.Drawing.Size(48, 17);
            this.rdbNota.TabIndex = 1;
            this.rdbNota.TabStop = true;
            this.rdbNota.Text = "Nota";
            this.rdbNota.UseVisualStyleBackColor = true;
            this.rdbNota.CheckedChanged += new System.EventHandler(this.rdbNota_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.rdbFactura);
            this.groupBox1.Controls.Add(this.txtImporte);
            this.groupBox1.Controls.Add(this.rdbNota);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(171, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Importe";
            // 
            // txtImporte
            // 
            this.txtImporte.Location = new System.Drawing.Point(219, 16);
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.ReadOnly = true;
            this.txtImporte.Size = new System.Drawing.Size(100, 20);
            this.txtImporte.TabIndex = 2;
            this.txtImporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // gpbFactura
            // 
            this.gpbFactura.Controls.Add(this.label3);
            this.gpbFactura.Controls.Add(this.label4);
            this.gpbFactura.Controls.Add(this.txtIva);
            this.gpbFactura.Controls.Add(this.txtSubtotal);
            this.gpbFactura.Controls.Add(this.label2);
            this.gpbFactura.Controls.Add(this.txtFolio);
            this.gpbFactura.Controls.Add(this.dtpFecha);
            this.gpbFactura.Controls.Add(this.label1);
            this.gpbFactura.ForeColor = System.Drawing.Color.White;
            this.gpbFactura.Location = new System.Drawing.Point(3, 56);
            this.gpbFactura.Name = "gpbFactura";
            this.gpbFactura.Size = new System.Drawing.Size(332, 77);
            this.gpbFactura.TabIndex = 1;
            this.gpbFactura.TabStop = false;
            this.gpbFactura.Text = "Datos de la Factura";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(167, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Iva";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(167, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Subtotal";
            // 
            // txtIva
            // 
            this.txtIva.Location = new System.Drawing.Point(219, 45);
            this.txtIva.Name = "txtIva";
            this.txtIva.Size = new System.Drawing.Size(100, 20);
            this.txtIva.TabIndex = 3;
            this.txtIva.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Location = new System.Drawing.Point(219, 19);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.Size = new System.Drawing.Size(100, 20);
            this.txtSubtotal.TabIndex = 2;
            this.txtSubtotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(9, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Folio";
            // 
            // txtFolio
            // 
            this.txtFolio.Location = new System.Drawing.Point(44, 19);
            this.txtFolio.Name = "txtFolio";
            this.txtFolio.Size = new System.Drawing.Size(100, 20);
            this.txtFolio.TabIndex = 0;
            // 
            // dtpFecha
            // 
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(44, 45);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(100, 20);
            this.dtpFecha.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Folio";
            // 
            // btnAceptar
            // 
            this.btnAceptar.Location = new System.Drawing.Point(166, 139);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 2;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(247, 139);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // GastoCajaAPoliza
            // 
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(339, 169);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.gpbFactura);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GastoCajaAPoliza";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Crear Póliza de Gasto";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gpbFactura.ResumeLayout(false);
            this.gpbFactura.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbFactura;
        private System.Windows.Forms.RadioButton rdbNota;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtImporte;
        private System.Windows.Forms.GroupBox gpbFactura;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtIva;
        private System.Windows.Forms.TextBox txtSubtotal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFolio;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
    }
}