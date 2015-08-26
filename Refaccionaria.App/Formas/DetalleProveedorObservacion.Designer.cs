namespace Refaccionaria.App
{
    partial class DetalleProveedorObservacion
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
            this.txtObservacion = new System.Windows.Forms.TextBox();
            this.lblObservacion = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtObservacion);
            this.gpoGen.Controls.Add(this.lblObservacion);
            this.gpoGen.Size = new System.Drawing.Size(560, 104);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(490, 122);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(402, 122);
            // 
            // txtObservacion
            // 
            this.txtObservacion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtObservacion.Location = new System.Drawing.Point(79, 13);
            this.txtObservacion.MaxLength = 4000;
            this.txtObservacion.Multiline = true;
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtObservacion.Size = new System.Drawing.Size(459, 85);
            this.txtObservacion.TabIndex = 3;
            // 
            // lblObservacion
            // 
            this.lblObservacion.AutoSize = true;
            this.lblObservacion.ForeColor = System.Drawing.Color.White;
            this.lblObservacion.Location = new System.Drawing.Point(6, 16);
            this.lblObservacion.Name = "lblObservacion";
            this.lblObservacion.Size = new System.Drawing.Size(67, 13);
            this.lblObservacion.TabIndex = 2;
            this.lblObservacion.Text = "Observación";
            // 
            // DetalleProveedorObservacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(584, 153);
            this.KeyPreview = true;
            this.Name = "DetalleProveedorObservacion";
            this.Load += new System.EventHandler(this.DetalleProveedorObservacion_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleProveedorObservacion_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtObservacion;
        private System.Windows.Forms.Label lblObservacion;
    }
}
