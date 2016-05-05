namespace Refaccionaria.App
{
    partial class DetalleCodigoAlterno
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
            this.txtCodigoAlterno = new System.Windows.Forms.TextBox();
            this.lblCodigoAlterno = new System.Windows.Forms.Label();
            this.cboMarca = new System.Windows.Forms.ComboBox();
            this.lblMarca = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtCodigoAlterno);
            this.gpoGen.Controls.Add(this.lblCodigoAlterno);
            this.gpoGen.Controls.Add(this.cboMarca);
            this.gpoGen.Controls.Add(this.lblMarca);
            this.gpoGen.Size = new System.Drawing.Size(341, 81);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(271, 99);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(183, 99);
            // 
            // txtCodigoAlterno
            // 
            this.txtCodigoAlterno.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCodigoAlterno.Location = new System.Drawing.Point(100, 46);
            this.txtCodigoAlterno.MaxLength = 100;
            this.txtCodigoAlterno.Name = "txtCodigoAlterno";
            this.txtCodigoAlterno.Size = new System.Drawing.Size(219, 20);
            this.txtCodigoAlterno.TabIndex = 7;
            // 
            // lblCodigoAlterno
            // 
            this.lblCodigoAlterno.AutoSize = true;
            this.lblCodigoAlterno.Location = new System.Drawing.Point(12, 49);
            this.lblCodigoAlterno.Name = "lblCodigoAlterno";
            this.lblCodigoAlterno.Size = new System.Drawing.Size(76, 13);
            this.lblCodigoAlterno.TabIndex = 6;
            this.lblCodigoAlterno.Text = "Código Alterno";
            // 
            // cboMarca
            // 
            this.cboMarca.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboMarca.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboMarca.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMarca.FormattingEnabled = true;
            this.cboMarca.Location = new System.Drawing.Point(100, 19);
            this.cboMarca.Name = "cboMarca";
            this.cboMarca.Size = new System.Drawing.Size(219, 21);
            this.cboMarca.TabIndex = 5;
            // 
            // lblMarca
            // 
            this.lblMarca.AutoSize = true;
            this.lblMarca.Location = new System.Drawing.Point(12, 22);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Size = new System.Drawing.Size(37, 13);
            this.lblMarca.TabIndex = 4;
            this.lblMarca.Text = "Marca";
            // 
            // DetalleCodigoAlterno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(365, 130);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "DetalleCodigoAlterno";
            this.Load += new System.EventHandler(this.DetalleCodigoAlterno_Load);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtCodigoAlterno;
        private System.Windows.Forms.Label lblCodigoAlterno;
        private System.Windows.Forms.ComboBox cboMarca;
        private System.Windows.Forms.Label lblMarca;
    }
}
