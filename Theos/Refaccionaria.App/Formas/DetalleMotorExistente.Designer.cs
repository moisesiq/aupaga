namespace Refaccionaria.App
{
    partial class DetalleMotorExistente
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
            this.txtNombreMotor = new System.Windows.Forms.TextBox();
            this.lblNombreMarca = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreMotor);
            this.gpoGen.Controls.Add(this.lblNombreMarca);
            this.gpoGen.Size = new System.Drawing.Size(334, 44);
            // 
            // btnCerrar
            // 
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Location = new System.Drawing.Point(264, 62);
            this.btnCerrar.Size = new System.Drawing.Size(82, 23);
            // 
            // btnGuardar
            // 
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Location = new System.Drawing.Point(176, 62);
            this.btnGuardar.Size = new System.Drawing.Size(82, 23);
            // 
            // txtNombreMotor
            // 
            this.txtNombreMotor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreMotor.Location = new System.Drawing.Point(94, 13);
            this.txtNombreMotor.MaxLength = 100;
            this.txtNombreMotor.Name = "txtNombreMotor";
            this.txtNombreMotor.Size = new System.Drawing.Size(234, 20);
            this.txtNombreMotor.TabIndex = 5;
            // 
            // lblNombreMarca
            // 
            this.lblNombreMarca.AutoSize = true;
            this.lblNombreMarca.Location = new System.Drawing.Point(6, 16);
            this.lblNombreMarca.Name = "lblNombreMarca";
            this.lblNombreMarca.Size = new System.Drawing.Size(74, 13);
            this.lblNombreMarca.TabIndex = 4;
            this.lblNombreMarca.Text = "Nombre Motor";
            // 
            // DetalleMotorExistente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(358, 93);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "DetalleMotorExistente";
            this.Load += new System.EventHandler(this.DetalleMotorExistente_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleMotorExistente_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreMotor;
        private System.Windows.Forms.Label lblNombreMarca;
    }
}
