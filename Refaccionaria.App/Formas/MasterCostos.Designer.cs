namespace Refaccionaria.App
{
    partial class MasterCostos
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
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.gpbPrecios = new System.Windows.Forms.GroupBox();
            this.txtDescuentoFijo = new System.Windows.Forms.TextBox();
            this.txtDescuentoPor = new System.Windows.Forms.TextBox();
            this.rbdDescuento = new System.Windows.Forms.RadioButton();
            this.txtIncrementoFijo = new System.Windows.Forms.TextBox();
            this.txtIncrementoPor = new System.Windows.Forms.TextBox();
            this.rdbIncremento = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtValorFijo = new System.Windows.Forms.TextBox();
            this.rdbValorFijo = new System.Windows.Forms.RadioButton();
            this.chkActualizarPrecios = new System.Windows.Forms.CheckBox();
            this.gpbPorcentajes = new System.Windows.Forms.GroupBox();
            this.txtPorcentaje5 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPorcentaje4 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPorcentaje3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPorcentaje2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPorcentaje1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gpbPrecios.SuspendLayout();
            this.gpbPorcentajes.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Location = new System.Drawing.Point(161, 146);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.Location = new System.Drawing.Point(80, 146);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 3;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // gpbPrecios
            // 
            this.gpbPrecios.Controls.Add(this.txtDescuentoFijo);
            this.gpbPrecios.Controls.Add(this.txtDescuentoPor);
            this.gpbPrecios.Controls.Add(this.rbdDescuento);
            this.gpbPrecios.Controls.Add(this.txtIncrementoFijo);
            this.gpbPrecios.Controls.Add(this.txtIncrementoPor);
            this.gpbPrecios.Controls.Add(this.rdbIncremento);
            this.gpbPrecios.Controls.Add(this.label2);
            this.gpbPrecios.Controls.Add(this.label1);
            this.gpbPrecios.Controls.Add(this.txtValorFijo);
            this.gpbPrecios.Controls.Add(this.rdbValorFijo);
            this.gpbPrecios.Location = new System.Drawing.Point(6, 3);
            this.gpbPrecios.Name = "gpbPrecios";
            this.gpbPrecios.Size = new System.Drawing.Size(235, 137);
            this.gpbPrecios.TabIndex = 0;
            this.gpbPrecios.TabStop = false;
            // 
            // txtDescuentoFijo
            // 
            this.txtDescuentoFijo.Enabled = false;
            this.txtDescuentoFijo.Location = new System.Drawing.Point(155, 84);
            this.txtDescuentoFijo.Name = "txtDescuentoFijo";
            this.txtDescuentoFijo.Size = new System.Drawing.Size(60, 20);
            this.txtDescuentoFijo.TabIndex = 7;
            // 
            // txtDescuentoPor
            // 
            this.txtDescuentoPor.Enabled = false;
            this.txtDescuentoPor.Location = new System.Drawing.Point(89, 84);
            this.txtDescuentoPor.Name = "txtDescuentoPor";
            this.txtDescuentoPor.Size = new System.Drawing.Size(60, 20);
            this.txtDescuentoPor.TabIndex = 6;
            // 
            // rbdDescuento
            // 
            this.rbdDescuento.AutoSize = true;
            this.rbdDescuento.Location = new System.Drawing.Point(6, 85);
            this.rbdDescuento.Name = "rbdDescuento";
            this.rbdDescuento.Size = new System.Drawing.Size(77, 17);
            this.rbdDescuento.TabIndex = 2;
            this.rbdDescuento.Text = "Descuento";
            this.rbdDescuento.UseVisualStyleBackColor = true;
            this.rbdDescuento.CheckedChanged += new System.EventHandler(this.rbdDescuento_CheckedChanged);
            // 
            // txtIncrementoFijo
            // 
            this.txtIncrementoFijo.Enabled = false;
            this.txtIncrementoFijo.Location = new System.Drawing.Point(155, 58);
            this.txtIncrementoFijo.Name = "txtIncrementoFijo";
            this.txtIncrementoFijo.Size = new System.Drawing.Size(60, 20);
            this.txtIncrementoFijo.TabIndex = 5;
            // 
            // txtIncrementoPor
            // 
            this.txtIncrementoPor.Enabled = false;
            this.txtIncrementoPor.Location = new System.Drawing.Point(89, 58);
            this.txtIncrementoPor.Name = "txtIncrementoPor";
            this.txtIncrementoPor.Size = new System.Drawing.Size(60, 20);
            this.txtIncrementoPor.TabIndex = 4;
            // 
            // rdbIncremento
            // 
            this.rdbIncremento.AutoSize = true;
            this.rdbIncremento.Location = new System.Drawing.Point(6, 59);
            this.rdbIncremento.Name = "rdbIncremento";
            this.rdbIncremento.Size = new System.Drawing.Size(78, 17);
            this.rdbIncremento.TabIndex = 1;
            this.rdbIncremento.Text = "Incremento";
            this.rdbIncremento.UseVisualStyleBackColor = true;
            this.rdbIncremento.CheckedChanged += new System.EventHandler(this.rdbIncremento_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "$";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(134, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "%";
            // 
            // txtValorFijo
            // 
            this.txtValorFijo.Location = new System.Drawing.Point(89, 19);
            this.txtValorFijo.Name = "txtValorFijo";
            this.txtValorFijo.Size = new System.Drawing.Size(60, 20);
            this.txtValorFijo.TabIndex = 3;
            // 
            // rdbValorFijo
            // 
            this.rdbValorFijo.AutoSize = true;
            this.rdbValorFijo.Checked = true;
            this.rdbValorFijo.Location = new System.Drawing.Point(6, 19);
            this.rdbValorFijo.Name = "rdbValorFijo";
            this.rdbValorFijo.Size = new System.Drawing.Size(68, 17);
            this.rdbValorFijo.TabIndex = 0;
            this.rdbValorFijo.TabStop = true;
            this.rdbValorFijo.Text = "Valor Fijo";
            this.rdbValorFijo.UseVisualStyleBackColor = true;
            this.rdbValorFijo.CheckedChanged += new System.EventHandler(this.rdbValorFijo_CheckedChanged);
            // 
            // chkActualizarPrecios
            // 
            this.chkActualizarPrecios.AutoSize = true;
            this.chkActualizarPrecios.BackColor = System.Drawing.Color.Transparent;
            this.chkActualizarPrecios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkActualizarPrecios.Location = new System.Drawing.Point(95, 113);
            this.chkActualizarPrecios.Margin = new System.Windows.Forms.Padding(0);
            this.chkActualizarPrecios.Name = "chkActualizarPrecios";
            this.chkActualizarPrecios.Size = new System.Drawing.Size(142, 17);
            this.chkActualizarPrecios.TabIndex = 2;
            this.chkActualizarPrecios.Text = "Actualizar lista de precios";
            this.chkActualizarPrecios.UseVisualStyleBackColor = false;
            // 
            // gpbPorcentajes
            // 
            this.gpbPorcentajes.Controls.Add(this.txtPorcentaje5);
            this.gpbPorcentajes.Controls.Add(this.label7);
            this.gpbPorcentajes.Controls.Add(this.txtPorcentaje4);
            this.gpbPorcentajes.Controls.Add(this.label6);
            this.gpbPorcentajes.Controls.Add(this.txtPorcentaje3);
            this.gpbPorcentajes.Controls.Add(this.label5);
            this.gpbPorcentajes.Controls.Add(this.txtPorcentaje2);
            this.gpbPorcentajes.Controls.Add(this.label4);
            this.gpbPorcentajes.Controls.Add(this.txtPorcentaje1);
            this.gpbPorcentajes.Controls.Add(this.label3);
            this.gpbPorcentajes.Location = new System.Drawing.Point(247, 3);
            this.gpbPorcentajes.Name = "gpbPorcentajes";
            this.gpbPorcentajes.Size = new System.Drawing.Size(235, 137);
            this.gpbPorcentajes.TabIndex = 1;
            this.gpbPorcentajes.TabStop = false;
            this.gpbPorcentajes.Visible = false;
            // 
            // txtPorcentaje5
            // 
            this.txtPorcentaje5.Location = new System.Drawing.Point(157, 44);
            this.txtPorcentaje5.Name = "txtPorcentaje5";
            this.txtPorcentaje5.Size = new System.Drawing.Size(60, 20);
            this.txtPorcentaje5.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(127, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "% 5";
            // 
            // txtPorcentaje4
            // 
            this.txtPorcentaje4.Location = new System.Drawing.Point(157, 18);
            this.txtPorcentaje4.Name = "txtPorcentaje4";
            this.txtPorcentaje4.Size = new System.Drawing.Size(60, 20);
            this.txtPorcentaje4.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(127, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "% 4";
            // 
            // txtPorcentaje3
            // 
            this.txtPorcentaje3.Location = new System.Drawing.Point(47, 70);
            this.txtPorcentaje3.Name = "txtPorcentaje3";
            this.txtPorcentaje3.Size = new System.Drawing.Size(60, 20);
            this.txtPorcentaje3.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "% 3";
            // 
            // txtPorcentaje2
            // 
            this.txtPorcentaje2.Location = new System.Drawing.Point(47, 44);
            this.txtPorcentaje2.Name = "txtPorcentaje2";
            this.txtPorcentaje2.Size = new System.Drawing.Size(60, 20);
            this.txtPorcentaje2.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "% 2";
            // 
            // txtPorcentaje1
            // 
            this.txtPorcentaje1.Location = new System.Drawing.Point(47, 18);
            this.txtPorcentaje1.Name = "txtPorcentaje1";
            this.txtPorcentaje1.Size = new System.Drawing.Size(60, 20);
            this.txtPorcentaje1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "% 1";
            // 
            // MasterCostos
            // 
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(494, 177);
            this.Controls.Add(this.chkActualizarPrecios);
            this.Controls.Add(this.gpbPorcentajes);
            this.Controls.Add(this.gpbPrecios);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MasterCostos";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Costo";
            this.Load += new System.EventHandler(this.MasterCostos_Load);
            this.gpbPrecios.ResumeLayout(false);
            this.gpbPrecios.PerformLayout();
            this.gpbPorcentajes.ResumeLayout(false);
            this.gpbPorcentajes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.GroupBox gpbPrecios;
        private System.Windows.Forms.GroupBox gpbPorcentajes;
        private System.Windows.Forms.TextBox txtPorcentaje5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPorcentaje4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPorcentaje3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPorcentaje2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPorcentaje1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkActualizarPrecios;
        private System.Windows.Forms.TextBox txtDescuentoFijo;
        private System.Windows.Forms.TextBox txtDescuentoPor;
        private System.Windows.Forms.RadioButton rbdDescuento;
        private System.Windows.Forms.TextBox txtIncrementoFijo;
        private System.Windows.Forms.TextBox txtIncrementoPor;
        private System.Windows.Forms.RadioButton rdbIncremento;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtValorFijo;
        private System.Windows.Forms.RadioButton rdbValorFijo;

    }
}