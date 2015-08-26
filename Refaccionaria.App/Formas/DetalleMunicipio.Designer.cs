namespace Refaccionaria.App
{
    partial class DetalleMunicipio
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
            this.txtNombreMunicipio = new System.Windows.Forms.TextBox();
            this.lblNombreMunicipio = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.lblSistema = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreMunicipio);
            this.gpoGen.Controls.Add(this.lblNombreMunicipio);
            this.gpoGen.Controls.Add(this.cboEstado);
            this.gpoGen.Controls.Add(this.lblSistema);
            this.gpoGen.Size = new System.Drawing.Size(370, 78);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(300, 96);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(212, 96);
            // 
            // txtNombreMunicipio
            // 
            this.txtNombreMunicipio.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreMunicipio.Location = new System.Drawing.Point(109, 46);
            this.txtNombreMunicipio.MaxLength = 255;
            this.txtNombreMunicipio.Name = "txtNombreMunicipio";
            this.txtNombreMunicipio.Size = new System.Drawing.Size(242, 20);
            this.txtNombreMunicipio.TabIndex = 11;
            // 
            // lblNombreMunicipio
            // 
            this.lblNombreMunicipio.AutoSize = true;
            this.lblNombreMunicipio.ForeColor = System.Drawing.Color.White;
            this.lblNombreMunicipio.Location = new System.Drawing.Point(11, 49);
            this.lblNombreMunicipio.Name = "lblNombreMunicipio";
            this.lblNombreMunicipio.Size = new System.Drawing.Size(92, 13);
            this.lblNombreMunicipio.TabIndex = 10;
            this.lblNombreMunicipio.Text = "Nombre Municipio";
            // 
            // cboEstado
            // 
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEstado.FormattingEnabled = true;
            this.cboEstado.Location = new System.Drawing.Point(109, 19);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(242, 21);
            this.cboEstado.TabIndex = 9;
            // 
            // lblSistema
            // 
            this.lblSistema.AutoSize = true;
            this.lblSistema.ForeColor = System.Drawing.Color.White;
            this.lblSistema.Location = new System.Drawing.Point(11, 22);
            this.lblSistema.Name = "lblSistema";
            this.lblSistema.Size = new System.Drawing.Size(40, 13);
            this.lblSistema.TabIndex = 8;
            this.lblSistema.Text = "Estado";
            // 
            // DetalleMunicipio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(394, 127);
            this.Name = "DetalleMunicipio";
            this.Load += new System.EventHandler(this.DetalleMunicipio_Load);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreMunicipio;
        private System.Windows.Forms.Label lblNombreMunicipio;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Label lblSistema;
    }
}
