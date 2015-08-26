namespace Refaccionaria.App
{
    partial class DetalleCiudad
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
            this.txtCiudad = new System.Windows.Forms.TextBox();
            this.lblCiudad = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.lblSistema = new System.Windows.Forms.Label();
            this.cboMunicipio = new System.Windows.Forms.ComboBox();
            this.lblMunicipio = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.cboMunicipio);
            this.gpoGen.Controls.Add(this.lblMunicipio);
            this.gpoGen.Controls.Add(this.txtCiudad);
            this.gpoGen.Controls.Add(this.lblCiudad);
            this.gpoGen.Controls.Add(this.cboEstado);
            this.gpoGen.Controls.Add(this.lblSistema);
            this.gpoGen.Size = new System.Drawing.Size(370, 105);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(300, 123);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(212, 123);
            this.btnGuardar.TabIndex = 1;
            // 
            // txtCiudad
            // 
            this.txtCiudad.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCiudad.Location = new System.Drawing.Point(109, 73);
            this.txtCiudad.MaxLength = 255;
            this.txtCiudad.Name = "txtCiudad";
            this.txtCiudad.Size = new System.Drawing.Size(242, 20);
            this.txtCiudad.TabIndex = 2;
            // 
            // lblCiudad
            // 
            this.lblCiudad.AutoSize = true;
            this.lblCiudad.ForeColor = System.Drawing.Color.White;
            this.lblCiudad.Location = new System.Drawing.Point(11, 76);
            this.lblCiudad.Name = "lblCiudad";
            this.lblCiudad.Size = new System.Drawing.Size(80, 13);
            this.lblCiudad.TabIndex = 14;
            this.lblCiudad.Text = "Nombre Ciudad";
            // 
            // cboEstado
            // 
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEstado.FormattingEnabled = true;
            this.cboEstado.Location = new System.Drawing.Point(109, 19);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(242, 21);
            this.cboEstado.TabIndex = 0;
            this.cboEstado.SelectedValueChanged += new System.EventHandler(this.cboEstado_SelectedValueChanged);
            // 
            // lblSistema
            // 
            this.lblSistema.AutoSize = true;
            this.lblSistema.ForeColor = System.Drawing.Color.White;
            this.lblSistema.Location = new System.Drawing.Point(11, 22);
            this.lblSistema.Name = "lblSistema";
            this.lblSistema.Size = new System.Drawing.Size(40, 13);
            this.lblSistema.TabIndex = 12;
            this.lblSistema.Text = "Estado";
            // 
            // cboMunicipio
            // 
            this.cboMunicipio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMunicipio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMunicipio.FormattingEnabled = true;
            this.cboMunicipio.Location = new System.Drawing.Point(109, 46);
            this.cboMunicipio.Name = "cboMunicipio";
            this.cboMunicipio.Size = new System.Drawing.Size(242, 21);
            this.cboMunicipio.TabIndex = 1;
            // 
            // lblMunicipio
            // 
            this.lblMunicipio.AutoSize = true;
            this.lblMunicipio.ForeColor = System.Drawing.Color.White;
            this.lblMunicipio.Location = new System.Drawing.Point(11, 49);
            this.lblMunicipio.Name = "lblMunicipio";
            this.lblMunicipio.Size = new System.Drawing.Size(52, 13);
            this.lblMunicipio.TabIndex = 16;
            this.lblMunicipio.Text = "Municipio";
            // 
            // DetalleCiudad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(394, 154);
            this.Name = "DetalleCiudad";
            this.Load += new System.EventHandler(this.DetalleCiudad_Load);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboMunicipio;
        private System.Windows.Forms.Label lblMunicipio;
        private System.Windows.Forms.TextBox txtCiudad;
        private System.Windows.Forms.Label lblCiudad;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Label lblSistema;
    }
}
