namespace Refaccionaria.App
{
    partial class DetalleUsuario
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
            this.lblNombrePersona = new System.Windows.Forms.Label();
            this.txtNombrePersona = new System.Windows.Forms.TextBox();
            this.txtNombreUsuario = new System.Windows.Forms.TextBox();
            this.lblNombreUsuario = new System.Windows.Forms.Label();
            this.txtContrasenia = new System.Windows.Forms.TextBox();
            this.lblContrasenia = new System.Windows.Forms.Label();
            this.lblEstatus = new System.Windows.Forms.Label();
            this.cboEstatus = new System.Windows.Forms.ComboBox();
            this.lblPerfiles = new System.Windows.Forms.Label();
            this.clbPerfiles = new System.Windows.Forms.CheckedListBox();
            this.lblAlertas = new System.Windows.Forms.Label();
            this.clbAlertas = new System.Windows.Forms.CheckedListBox();
            this.cmbTipoDeUsuario = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.cmbTipoDeUsuario);
            this.gpoGen.Controls.Add(this.label1);
            this.gpoGen.Controls.Add(this.clbAlertas);
            this.gpoGen.Controls.Add(this.lblAlertas);
            this.gpoGen.Controls.Add(this.clbPerfiles);
            this.gpoGen.Controls.Add(this.lblPerfiles);
            this.gpoGen.Controls.Add(this.cboEstatus);
            this.gpoGen.Controls.Add(this.lblEstatus);
            this.gpoGen.Controls.Add(this.txtContrasenia);
            this.gpoGen.Controls.Add(this.lblContrasenia);
            this.gpoGen.Controls.Add(this.txtNombreUsuario);
            this.gpoGen.Controls.Add(this.lblNombreUsuario);
            this.gpoGen.Controls.Add(this.txtNombrePersona);
            this.gpoGen.Controls.Add(this.lblNombrePersona);
            // 
            // btnCerrar
            // 
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.TabIndex = 1;
            // 
            // lblNombrePersona
            // 
            this.lblNombrePersona.AutoSize = true;
            this.lblNombrePersona.ForeColor = System.Drawing.Color.White;
            this.lblNombrePersona.Location = new System.Drawing.Point(6, 16);
            this.lblNombrePersona.Name = "lblNombrePersona";
            this.lblNombrePersona.Size = new System.Drawing.Size(86, 13);
            this.lblNombrePersona.TabIndex = 0;
            this.lblNombrePersona.Text = "Nombre Persona";
            // 
            // txtNombrePersona
            // 
            this.txtNombrePersona.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombrePersona.Location = new System.Drawing.Point(98, 13);
            this.txtNombrePersona.MaxLength = 50;
            this.txtNombrePersona.Name = "txtNombrePersona";
            this.txtNombrePersona.Size = new System.Drawing.Size(340, 20);
            this.txtNombrePersona.TabIndex = 0;
            // 
            // txtNombreUsuario
            // 
            this.txtNombreUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreUsuario.Location = new System.Drawing.Point(98, 39);
            this.txtNombreUsuario.MaxLength = 50;
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new System.Drawing.Size(170, 20);
            this.txtNombreUsuario.TabIndex = 1;
            // 
            // lblNombreUsuario
            // 
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.ForeColor = System.Drawing.Color.White;
            this.lblNombreUsuario.Location = new System.Drawing.Point(6, 42);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Size = new System.Drawing.Size(83, 13);
            this.lblNombreUsuario.TabIndex = 2;
            this.lblNombreUsuario.Text = "Nombre Usuario";
            // 
            // txtContrasenia
            // 
            this.txtContrasenia.Location = new System.Drawing.Point(98, 65);
            this.txtContrasenia.MaxLength = 50;
            this.txtContrasenia.Name = "txtContrasenia";
            this.txtContrasenia.Size = new System.Drawing.Size(170, 20);
            this.txtContrasenia.TabIndex = 2;
            this.txtContrasenia.UseSystemPasswordChar = true;
            // 
            // lblContrasenia
            // 
            this.lblContrasenia.AutoSize = true;
            this.lblContrasenia.ForeColor = System.Drawing.Color.White;
            this.lblContrasenia.Location = new System.Drawing.Point(6, 68);
            this.lblContrasenia.Name = "lblContrasenia";
            this.lblContrasenia.Size = new System.Drawing.Size(61, 13);
            this.lblContrasenia.TabIndex = 4;
            this.lblContrasenia.Text = "Contraseña";
            // 
            // lblEstatus
            // 
            this.lblEstatus.AutoSize = true;
            this.lblEstatus.ForeColor = System.Drawing.Color.White;
            this.lblEstatus.Location = new System.Drawing.Point(6, 94);
            this.lblEstatus.Name = "lblEstatus";
            this.lblEstatus.Size = new System.Drawing.Size(42, 13);
            this.lblEstatus.TabIndex = 6;
            this.lblEstatus.Text = "Estatus";
            // 
            // cboEstatus
            // 
            this.cboEstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEstatus.FormattingEnabled = true;
            this.cboEstatus.Location = new System.Drawing.Point(98, 91);
            this.cboEstatus.Name = "cboEstatus";
            this.cboEstatus.Size = new System.Drawing.Size(170, 21);
            this.cboEstatus.TabIndex = 3;
            // 
            // lblPerfiles
            // 
            this.lblPerfiles.AutoSize = true;
            this.lblPerfiles.ForeColor = System.Drawing.Color.White;
            this.lblPerfiles.Location = new System.Drawing.Point(6, 148);
            this.lblPerfiles.Name = "lblPerfiles";
            this.lblPerfiles.Size = new System.Drawing.Size(41, 13);
            this.lblPerfiles.TabIndex = 8;
            this.lblPerfiles.Text = "Perfiles";
            // 
            // clbPerfiles
            // 
            this.clbPerfiles.CheckOnClick = true;
            this.clbPerfiles.FormattingEnabled = true;
            this.clbPerfiles.Location = new System.Drawing.Point(98, 148);
            this.clbPerfiles.Name = "clbPerfiles";
            this.clbPerfiles.Size = new System.Drawing.Size(340, 169);
            this.clbPerfiles.TabIndex = 5;
            // 
            // lblAlertas
            // 
            this.lblAlertas.AutoSize = true;
            this.lblAlertas.ForeColor = System.Drawing.Color.White;
            this.lblAlertas.Location = new System.Drawing.Point(6, 336);
            this.lblAlertas.Name = "lblAlertas";
            this.lblAlertas.Size = new System.Drawing.Size(39, 13);
            this.lblAlertas.TabIndex = 10;
            this.lblAlertas.Text = "Alertas";
            // 
            // clbAlertas
            // 
            this.clbAlertas.CheckOnClick = true;
            this.clbAlertas.FormattingEnabled = true;
            this.clbAlertas.Location = new System.Drawing.Point(98, 336);
            this.clbAlertas.Name = "clbAlertas";
            this.clbAlertas.Size = new System.Drawing.Size(340, 49);
            this.clbAlertas.TabIndex = 6;
            // 
            // cmbTipoDeUsuario
            // 
            this.cmbTipoDeUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTipoDeUsuario.FormattingEnabled = true;
            this.cmbTipoDeUsuario.Location = new System.Drawing.Point(98, 118);
            this.cmbTipoDeUsuario.Name = "cmbTipoDeUsuario";
            this.cmbTipoDeUsuario.Size = new System.Drawing.Size(170, 21);
            this.cmbTipoDeUsuario.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Tipo";
            // 
            // DetalleUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(484, 455);
            this.KeyPreview = true;
            this.Name = "DetalleUsuario";
            this.Load += new System.EventHandler(this.DetalleUsuario_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleUsuario_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombrePersona;
        private System.Windows.Forms.Label lblNombrePersona;
        private System.Windows.Forms.CheckedListBox clbPerfiles;
        private System.Windows.Forms.Label lblPerfiles;
        private System.Windows.Forms.ComboBox cboEstatus;
        private System.Windows.Forms.Label lblEstatus;
        private System.Windows.Forms.TextBox txtContrasenia;
        private System.Windows.Forms.Label lblContrasenia;
        private System.Windows.Forms.TextBox txtNombreUsuario;
        private System.Windows.Forms.Label lblNombreUsuario;
        private System.Windows.Forms.CheckedListBox clbAlertas;
        private System.Windows.Forms.Label lblAlertas;
        private System.Windows.Forms.ComboBox cmbTipoDeUsuario;
        private System.Windows.Forms.Label label1;
    }
}
