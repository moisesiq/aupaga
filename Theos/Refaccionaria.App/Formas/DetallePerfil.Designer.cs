namespace Refaccionaria.App
{
    partial class DetallePerfil
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
            this.clbPermisos = new System.Windows.Forms.CheckedListBox();
            this.lblPermisos = new System.Windows.Forms.Label();
            this.txtNombrePerfil = new System.Windows.Forms.TextBox();
            this.lblNombrePerfil = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.clbPermisos);
            this.gpoGen.Controls.Add(this.lblPermisos);
            this.gpoGen.Controls.Add(this.txtNombrePerfil);
            this.gpoGen.Controls.Add(this.lblNombrePerfil);
            // 
            // btnCerrar
            // 
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Location = new System.Drawing.Point(390, 426);
            this.btnCerrar.Size = new System.Drawing.Size(82, 23);
            // 
            // btnGuardar
            // 
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Location = new System.Drawing.Point(302, 426);
            this.btnGuardar.Size = new System.Drawing.Size(82, 23);
            // 
            // clbPermisos
            // 
            this.clbPermisos.CheckOnClick = true;
            this.clbPermisos.FormattingEnabled = true;
            this.clbPermisos.Location = new System.Drawing.Point(106, 41);
            this.clbPermisos.Name = "clbPermisos";
            this.clbPermisos.Size = new System.Drawing.Size(340, 349);
            this.clbPermisos.TabIndex = 13;
            // 
            // lblPermisos
            // 
            this.lblPermisos.AutoSize = true;
            this.lblPermisos.Location = new System.Drawing.Point(14, 41);
            this.lblPermisos.Name = "lblPermisos";
            this.lblPermisos.Size = new System.Drawing.Size(41, 13);
            this.lblPermisos.TabIndex = 12;
            this.lblPermisos.Text = "Perfiles";
            // 
            // txtNombrePerfil
            // 
            this.txtNombrePerfil.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombrePerfil.Location = new System.Drawing.Point(106, 15);
            this.txtNombrePerfil.MaxLength = 100;
            this.txtNombrePerfil.Name = "txtNombrePerfil";
            this.txtNombrePerfil.Size = new System.Drawing.Size(340, 20);
            this.txtNombrePerfil.TabIndex = 11;
            // 
            // lblNombrePerfil
            // 
            this.lblNombrePerfil.AutoSize = true;
            this.lblNombrePerfil.Location = new System.Drawing.Point(14, 18);
            this.lblNombrePerfil.Name = "lblNombrePerfil";
            this.lblNombrePerfil.Size = new System.Drawing.Size(70, 13);
            this.lblNombrePerfil.TabIndex = 10;
            this.lblNombrePerfil.Text = "Nombre Perfil";
            // 
            // DetallePerfil
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(484, 457);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "DetallePerfil";
            this.Load += new System.EventHandler(this.DetallePerfil_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetallePerfil_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbPermisos;
        private System.Windows.Forms.Label lblPermisos;
        private System.Windows.Forms.TextBox txtNombrePerfil;
        private System.Windows.Forms.Label lblNombrePerfil;
    }
}
