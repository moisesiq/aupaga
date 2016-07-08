namespace Refaccionaria.App
{
    partial class DetalleUbicacion
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
            this.txtNombreUbicacion = new System.Windows.Forms.TextBox();
            this.lblNombreMarca = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreUbicacion);
            this.gpoGen.Controls.Add(this.lblNombreMarca);
            this.gpoGen.Size = new System.Drawing.Size(334, 44);
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Location = new System.Drawing.Point(264, 62);
            this.btnCerrar.Size = new System.Drawing.Size(82, 23);
            this.btnCerrar.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Location = new System.Drawing.Point(176, 62);
            this.btnGuardar.Size = new System.Drawing.Size(82, 23);
            this.btnGuardar.UseVisualStyleBackColor = false;
            // 
            // txtNombreUbicacion
            // 
            this.txtNombreUbicacion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreUbicacion.Location = new System.Drawing.Point(115, 12);
            this.txtNombreUbicacion.MaxLength = 100;
            this.txtNombreUbicacion.Name = "txtNombreUbicacion";
            this.txtNombreUbicacion.Size = new System.Drawing.Size(206, 20);
            this.txtNombreUbicacion.TabIndex = 5;
            // 
            // lblNombreMarca
            // 
            this.lblNombreMarca.AutoSize = true;
            this.lblNombreMarca.Location = new System.Drawing.Point(14, 15);
            this.lblNombreMarca.Name = "lblNombreMarca";
            this.lblNombreMarca.Size = new System.Drawing.Size(95, 13);
            this.lblNombreMarca.TabIndex = 4;
            this.lblNombreMarca.Text = "Nombre Ubicación";
            // 
            // DetalleUbicacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(358, 93);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "DetalleUbicacion";
            this.Load += new System.EventHandler(this.DetalleUbicacion_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleUbicacion_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreUbicacion;
        private System.Windows.Forms.Label lblNombreMarca;
    }
}
