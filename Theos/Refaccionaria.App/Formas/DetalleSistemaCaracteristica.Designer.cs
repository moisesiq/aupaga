namespace Refaccionaria.App
{
    partial class DetalleSistemaCaracteristica
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
            this.txtNombreSistema = new System.Windows.Forms.TextBox();
            this.lblNombreMarca = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreSistema);
            this.gpoGen.Controls.Add(this.lblNombreMarca);
            this.gpoGen.Size = new System.Drawing.Size(334, 44);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(264, 62);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(176, 62);
            // 
            // txtNombreSistema
            // 
            this.txtNombreSistema.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreSistema.Location = new System.Drawing.Point(96, 12);
            this.txtNombreSistema.MaxLength = 100;
            this.txtNombreSistema.Name = "txtNombreSistema";
            this.txtNombreSistema.Size = new System.Drawing.Size(225, 20);
            this.txtNombreSistema.TabIndex = 7;
            // 
            // lblNombreMarca
            // 
            this.lblNombreMarca.AutoSize = true;
            this.lblNombreMarca.ForeColor = System.Drawing.Color.White;
            this.lblNombreMarca.Location = new System.Drawing.Point(6, 15);
            this.lblNombreMarca.Name = "lblNombreMarca";
            this.lblNombreMarca.Size = new System.Drawing.Size(84, 13);
            this.lblNombreMarca.TabIndex = 6;
            this.lblNombreMarca.Text = "Nombre Sistema";
            // 
            // DetalleSistemaCaracteristica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(358, 93);
            this.KeyPreview = true;
            this.Name = "DetalleSistemaCaracteristica";
            this.Load += new System.EventHandler(this.DetalleSistemaCaracteristica_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleSistemaCaracteristica_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreSistema;
        private System.Windows.Forms.Label lblNombreMarca;
    }
}
