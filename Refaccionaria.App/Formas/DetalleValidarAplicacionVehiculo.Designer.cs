namespace Refaccionaria.App
{
    partial class DetalleValidarAplicacionVehiculo
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFuente = new System.Windows.Forms.ComboBox();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.cmbFuente);
            this.gpoGen.Controls.Add(this.label1);
            this.gpoGen.Size = new System.Drawing.Size(303, 46);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(233, 64);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(145, 64);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fuente";
            // 
            // cmbFuente
            // 
            this.cmbFuente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFuente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFuente.FormattingEnabled = true;
            this.cmbFuente.Location = new System.Drawing.Point(52, 13);
            this.cmbFuente.Name = "cmbFuente";
            this.cmbFuente.Size = new System.Drawing.Size(240, 21);
            this.cmbFuente.TabIndex = 1;
            // 
            // DetalleValidarAplicacionVehiculo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 95);
            this.Name = "DetalleValidarAplicacionVehiculo";
            this.Text = "Validar";
            this.Load += new System.EventHandler(this.DetalleValidarAplicacionVehiculo_Load);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbFuente;
        private System.Windows.Forms.Label label1;
    }
}