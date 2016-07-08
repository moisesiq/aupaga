namespace Refaccionaria.App
{
    partial class DetalleMotor
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
            this.cboModelo = new System.Windows.Forms.ComboBox();
            this.lblModelo = new System.Windows.Forms.Label();
            this.lblNombreMotor = new System.Windows.Forms.Label();
            this.clbAnios = new System.Windows.Forms.CheckedListBox();
            this.cboMotor = new System.Windows.Forms.ComboBox();
            this.lblAnios = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.lblAnios);
            this.gpoGen.Controls.Add(this.cboMotor);
            this.gpoGen.Controls.Add(this.clbAnios);
            this.gpoGen.Controls.Add(this.lblNombreMotor);
            this.gpoGen.Controls.Add(this.cboModelo);
            this.gpoGen.Controls.Add(this.lblModelo);
            this.gpoGen.Controls.Add(this.cboMarca);
            this.gpoGen.Controls.Add(this.lblMarca);
            this.gpoGen.Size = new System.Drawing.Size(289, 408);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(219, 426);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(131, 426);
            this.btnGuardar.TabIndex = 1;
            // 
            // lblMarca
            // 
            this.lblMarca.AutoSize = true;
            this.lblMarca.Location = new System.Drawing.Point(6, 16);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Size = new System.Drawing.Size(37, 13);
            this.lblMarca.TabIndex = 7;
            this.lblMarca.Text = "Marca";
            // 
            // cboMarca
            // 
            this.cboMarca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarca.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMarca.FormattingEnabled = true;
            this.cboMarca.Location = new System.Drawing.Point(98, 13);
            this.cboMarca.Name = "cboMarca";
            this.cboMarca.Size = new System.Drawing.Size(170, 21);
            this.cboMarca.TabIndex = 0;
            this.cboMarca.SelectedValueChanged += new System.EventHandler(this.cboMarca_SelectedValueChanged);
            // 
            // cboModelo
            // 
            this.cboModelo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModelo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboModelo.FormattingEnabled = true;
            this.cboModelo.Location = new System.Drawing.Point(98, 40);
            this.cboModelo.Name = "cboModelo";
            this.cboModelo.Size = new System.Drawing.Size(170, 21);
            this.cboModelo.TabIndex = 1;
            this.cboModelo.SelectedValueChanged += new System.EventHandler(this.cboModelo_SelectedValueChanged);
            // 
            // lblModelo
            // 
            this.lblModelo.AutoSize = true;
            this.lblModelo.Location = new System.Drawing.Point(6, 43);
            this.lblModelo.Name = "lblModelo";
            this.lblModelo.Size = new System.Drawing.Size(42, 13);
            this.lblModelo.TabIndex = 8;
            this.lblModelo.Text = "Modelo";
            // 
            // lblNombreMotor
            // 
            this.lblNombreMotor.AutoSize = true;
            this.lblNombreMotor.Location = new System.Drawing.Point(6, 70);
            this.lblNombreMotor.Name = "lblNombreMotor";
            this.lblNombreMotor.Size = new System.Drawing.Size(74, 13);
            this.lblNombreMotor.TabIndex = 11;
            this.lblNombreMotor.Text = "Nombre Motor";
            // 
            // clbAnios
            // 
            this.clbAnios.CheckOnClick = true;
            this.clbAnios.FormattingEnabled = true;
            this.clbAnios.Location = new System.Drawing.Point(98, 94);
            this.clbAnios.Name = "clbAnios";
            this.clbAnios.Size = new System.Drawing.Size(170, 304);
            this.clbAnios.TabIndex = 12;
            // 
            // cboMotor
            // 
            this.cboMotor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMotor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMotor.FormattingEnabled = true;
            this.cboMotor.Location = new System.Drawing.Point(98, 67);
            this.cboMotor.Name = "cboMotor";
            this.cboMotor.Size = new System.Drawing.Size(170, 21);
            this.cboMotor.TabIndex = 13;
            // 
            // lblAnios
            // 
            this.lblAnios.AutoSize = true;
            this.lblAnios.Location = new System.Drawing.Point(6, 94);
            this.lblAnios.Name = "lblAnios";
            this.lblAnios.Size = new System.Drawing.Size(74, 13);
            this.lblAnios.TabIndex = 14;
            this.lblAnios.Text = "Nombre Motor";
            // 
            // DetalleMotor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(313, 457);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "DetalleMotor";
            this.Load += new System.EventHandler(this.DetalleMotor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleMotor_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.ComboBox cboMarca;
        private System.Windows.Forms.ComboBox cboModelo;
        private System.Windows.Forms.Label lblModelo;
        private System.Windows.Forms.Label lblNombreMotor;
        private System.Windows.Forms.CheckedListBox clbAnios;
        private System.Windows.Forms.Label lblAnios;
        private System.Windows.Forms.ComboBox cboMotor;
    }
}
