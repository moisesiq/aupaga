namespace Refaccionaria.App
{
    partial class DetalleModelo
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
            this.lblMarca = new System.Windows.Forms.Label();
            this.cboMarca = new System.Windows.Forms.ComboBox();
            this.txtNombreModelo = new System.Windows.Forms.TextBox();
            this.lblNombreModelo = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreModelo);
            this.gpoGen.Controls.Add(this.lblNombreModelo);
            this.gpoGen.Controls.Add(this.cboMarca);
            this.gpoGen.Controls.Add(this.lblMarca);
            this.gpoGen.Size = new System.Drawing.Size(334, 72);
            // 
            // btnCerrar
            // 
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Location = new System.Drawing.Point(264, 90);
            this.btnCerrar.Size = new System.Drawing.Size(82, 23);
            // 
            // btnGuardar
            // 
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Location = new System.Drawing.Point(176, 90);
            this.btnGuardar.Size = new System.Drawing.Size(82, 23);
            // 
            // lblMarca
            // 
            this.lblMarca.AutoSize = true;
            this.lblMarca.Location = new System.Drawing.Point(6, 16);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Size = new System.Drawing.Size(37, 13);
            this.lblMarca.TabIndex = 0;
            this.lblMarca.Text = "Marca";
            // 
            // cboMarca
            // 
            this.cboMarca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarca.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMarca.FormattingEnabled = true;
            this.cboMarca.Location = new System.Drawing.Point(94, 13);
            this.cboMarca.Name = "cboMarca";
            this.cboMarca.Size = new System.Drawing.Size(219, 21);
            this.cboMarca.TabIndex = 1;
            // 
            // txtNombreModelo
            // 
            this.txtNombreModelo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreModelo.Location = new System.Drawing.Point(94, 40);
            this.txtNombreModelo.MaxLength = 100;
            this.txtNombreModelo.Name = "txtNombreModelo";
            this.txtNombreModelo.Size = new System.Drawing.Size(219, 20);
            this.txtNombreModelo.TabIndex = 3;
            // 
            // lblNombreModelo
            // 
            this.lblNombreModelo.AutoSize = true;
            this.lblNombreModelo.Location = new System.Drawing.Point(6, 43);
            this.lblNombreModelo.Name = "lblNombreModelo";
            this.lblNombreModelo.Size = new System.Drawing.Size(82, 13);
            this.lblNombreModelo.TabIndex = 2;
            this.lblNombreModelo.Text = "Nombre Modelo";
            // 
            // DetalleModelo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(358, 121);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "DetalleModelo";
            this.Load += new System.EventHandler(this.DetalleModelo_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleModelo_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboMarca;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.TextBox txtNombreModelo;
        private System.Windows.Forms.Label lblNombreModelo;
    }
}
