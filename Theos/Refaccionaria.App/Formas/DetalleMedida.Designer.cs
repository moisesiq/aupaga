namespace Refaccionaria.App
{
    partial class DetalleMedida
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
            this.lblNombreMedida = new System.Windows.Forms.Label();
            this.txtNombreMedida = new System.Windows.Forms.TextBox();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreMedida);
            this.gpoGen.Controls.Add(this.lblNombreMedida);
            this.gpoGen.Size = new System.Drawing.Size(294, 43);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(224, 61);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(136, 61);
            // 
            // lblNombreMedida
            // 
            this.lblNombreMedida.AutoSize = true;
            this.lblNombreMedida.Location = new System.Drawing.Point(6, 16);
            this.lblNombreMedida.Name = "lblNombreMedida";
            this.lblNombreMedida.Size = new System.Drawing.Size(82, 13);
            this.lblNombreMedida.TabIndex = 0;
            this.lblNombreMedida.Text = "Nombre Medida";
            // 
            // txtNombreMedida
            // 
            this.txtNombreMedida.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreMedida.Location = new System.Drawing.Point(94, 13);
            this.txtNombreMedida.MaxLength = 100;
            this.txtNombreMedida.Name = "txtNombreMedida";
            this.txtNombreMedida.Size = new System.Drawing.Size(194, 20);
            this.txtNombreMedida.TabIndex = 1;
            // 
            // DetalleMedida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(318, 92);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "DetalleMedida";
            this.Load += new System.EventHandler(this.DetalleMedida_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleMedida_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNombreMedida;
        private System.Windows.Forms.TextBox txtNombreMedida;
    }
}
