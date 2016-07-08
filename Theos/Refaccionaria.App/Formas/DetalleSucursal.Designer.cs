namespace Refaccionaria.App
{
    partial class DetalleSucursal
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
            this.txtNombreSucursal = new System.Windows.Forms.TextBox();
            this.lblNombreSucursal = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbGerente = new System.Windows.Forms.ComboBox();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.cmbGerente);
            this.gpoGen.Controls.Add(this.label1);
            this.gpoGen.Controls.Add(this.txtIP);
            this.gpoGen.Controls.Add(this.lblIP);
            this.gpoGen.Controls.Add(this.txtNombreSucursal);
            this.gpoGen.Controls.Add(this.lblNombreSucursal);
            this.gpoGen.Size = new System.Drawing.Size(334, 93);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(264, 111);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(176, 111);
            // 
            // txtNombreSucursal
            // 
            this.txtNombreSucursal.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreSucursal.Location = new System.Drawing.Point(100, 13);
            this.txtNombreSucursal.MaxLength = 100;
            this.txtNombreSucursal.Name = "txtNombreSucursal";
            this.txtNombreSucursal.Size = new System.Drawing.Size(215, 20);
            this.txtNombreSucursal.TabIndex = 3;
            // 
            // lblNombreSucursal
            // 
            this.lblNombreSucursal.AutoSize = true;
            this.lblNombreSucursal.ForeColor = System.Drawing.Color.White;
            this.lblNombreSucursal.Location = new System.Drawing.Point(6, 16);
            this.lblNombreSucursal.Name = "lblNombreSucursal";
            this.lblNombreSucursal.Size = new System.Drawing.Size(88, 13);
            this.lblNombreSucursal.TabIndex = 2;
            this.lblNombreSucursal.Text = "Nombre Sucursal";
            // 
            // txtIP
            // 
            this.txtIP.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtIP.Location = new System.Drawing.Point(100, 39);
            this.txtIP.MaxLength = 100;
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(215, 20);
            this.txtIP.TabIndex = 5;
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.ForeColor = System.Drawing.Color.White;
            this.lblIP.Location = new System.Drawing.Point(6, 42);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(65, 13);
            this.lblIP.TabIndex = 4;
            this.lblIP.Text = "Dirección IP";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Gerente";
            // 
            // cmbGerente
            // 
            this.cmbGerente.FormattingEnabled = true;
            this.cmbGerente.Location = new System.Drawing.Point(100, 65);
            this.cmbGerente.Name = "cmbGerente";
            this.cmbGerente.Size = new System.Drawing.Size(215, 21);
            this.cmbGerente.TabIndex = 7;
            // 
            // DetalleSucursal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(358, 142);
            this.KeyPreview = true;
            this.Name = "DetalleSucursal";
            this.Load += new System.EventHandler(this.DetalleSusursal_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleSucursal_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtNombreSucursal;
        private System.Windows.Forms.Label lblNombreSucursal;
        private System.Windows.Forms.ComboBox cmbGerente;
        private System.Windows.Forms.Label label1;
    }
}
