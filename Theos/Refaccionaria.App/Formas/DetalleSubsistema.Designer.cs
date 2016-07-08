namespace Refaccionaria.App
{
    partial class DetalleSubsistema
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
            this.txtNombreSubsistema = new System.Windows.Forms.TextBox();
            this.lblNombreSubsistema = new System.Windows.Forms.Label();
            this.cboSistema = new System.Windows.Forms.ComboBox();
            this.lblSistema = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreSubsistema);
            this.gpoGen.Controls.Add(this.lblNombreSubsistema);
            this.gpoGen.Controls.Add(this.cboSistema);
            this.gpoGen.Controls.Add(this.lblSistema);
            this.gpoGen.Size = new System.Drawing.Size(334, 72);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(264, 90);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(176, 90);
            // 
            // txtNombreSubsistema
            // 
            this.txtNombreSubsistema.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreSubsistema.Location = new System.Drawing.Point(102, 40);
            this.txtNombreSubsistema.MaxLength = 100;
            this.txtNombreSubsistema.Name = "txtNombreSubsistema";
            this.txtNombreSubsistema.Size = new System.Drawing.Size(219, 20);
            this.txtNombreSubsistema.TabIndex = 7;
            // 
            // lblNombreSubsistema
            // 
            this.lblNombreSubsistema.AutoSize = true;
            this.lblNombreSubsistema.ForeColor = System.Drawing.Color.White;
            this.lblNombreSubsistema.Location = new System.Drawing.Point(14, 43);
            this.lblNombreSubsistema.Name = "lblNombreSubsistema";
            this.lblNombreSubsistema.Size = new System.Drawing.Size(82, 13);
            this.lblNombreSubsistema.TabIndex = 6;
            this.lblNombreSubsistema.Text = "Nombre Modelo";
            // 
            // cboSistema
            // 
            this.cboSistema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSistema.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboSistema.FormattingEnabled = true;
            this.cboSistema.Location = new System.Drawing.Point(102, 13);
            this.cboSistema.Name = "cboSistema";
            this.cboSistema.Size = new System.Drawing.Size(219, 21);
            this.cboSistema.TabIndex = 5;
            // 
            // lblSistema
            // 
            this.lblSistema.AutoSize = true;
            this.lblSistema.ForeColor = System.Drawing.Color.White;
            this.lblSistema.Location = new System.Drawing.Point(14, 16);
            this.lblSistema.Name = "lblSistema";
            this.lblSistema.Size = new System.Drawing.Size(44, 13);
            this.lblSistema.TabIndex = 4;
            this.lblSistema.Text = "Sistema";
            // 
            // DetalleSubsistema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(358, 121);
            this.KeyPreview = true;
            this.Name = "DetalleSubsistema";
            this.Load += new System.EventHandler(this.DetalleSubsistema_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleSubsistema_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreSubsistema;
        private System.Windows.Forms.Label lblNombreSubsistema;
        private System.Windows.Forms.ComboBox cboSistema;
        private System.Windows.Forms.Label lblSistema;
    }
}
