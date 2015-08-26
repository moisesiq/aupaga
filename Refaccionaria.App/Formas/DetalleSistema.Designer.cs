namespace Refaccionaria.App
{
    partial class DetalleSistema
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
            this.lblNombreLinea = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreSistema);
            this.gpoGen.Controls.Add(this.lblNombreLinea);
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
            this.txtNombreSistema.Location = new System.Drawing.Point(96, 13);
            this.txtNombreSistema.MaxLength = 100;
            this.txtNombreSistema.Name = "txtNombreSistema";
            this.txtNombreSistema.Size = new System.Drawing.Size(232, 20);
            this.txtNombreSistema.TabIndex = 7;
            // 
            // lblNombreLinea
            // 
            this.lblNombreLinea.AutoSize = true;
            this.lblNombreLinea.ForeColor = System.Drawing.Color.White;
            this.lblNombreLinea.Location = new System.Drawing.Point(6, 16);
            this.lblNombreLinea.Name = "lblNombreLinea";
            this.lblNombreLinea.Size = new System.Drawing.Size(84, 13);
            this.lblNombreLinea.TabIndex = 6;
            this.lblNombreLinea.Text = "Nombre Sistema";
            // 
            // DetalleSistema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(358, 93);
            this.KeyPreview = true;
            this.Name = "DetalleSistema";
            this.Load += new System.EventHandler(this.DetalleSistema_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleSistema_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreSistema;
        private System.Windows.Forms.Label lblNombreLinea;
    }
}
