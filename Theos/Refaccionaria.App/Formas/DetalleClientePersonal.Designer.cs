namespace Refaccionaria.App
{
    partial class DetalleClientePersonal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetalleClientePersonal));
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtCorreoE = new System.Windows.Forms.TextBox();
            this.lblCorreo = new System.Windows.Forms.Label();
            this.chkCfdi = new System.Windows.Forms.CheckBox();
            this.gbImagen = new System.Windows.Forms.GroupBox();
            this.picBoxImagen = new System.Windows.Forms.PictureBox();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.gpoGen.SuspendLayout();
            this.gbImagen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImagen)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.gbImagen);
            this.gpoGen.Controls.Add(this.chkCfdi);
            this.gpoGen.Controls.Add(this.txtCorreoE);
            this.gpoGen.Controls.Add(this.lblCorreo);
            this.gpoGen.Controls.Add(this.txtNombre);
            this.gpoGen.Controls.Add(this.lblNombre);
            this.gpoGen.Size = new System.Drawing.Size(460, 365);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(390, 383);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(302, 383);
            // 
            // txtNombre
            // 
            this.txtNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombre.Location = new System.Drawing.Point(111, 19);
            this.txtNombre.MaxLength = 50;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(333, 20);
            this.txtNombre.TabIndex = 5;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.ForeColor = System.Drawing.Color.White;
            this.lblNombre.Location = new System.Drawing.Point(11, 22);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(44, 13);
            this.lblNombre.TabIndex = 6;
            this.lblNombre.Text = "Nombre";
            // 
            // txtCorreoE
            // 
            this.txtCorreoE.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtCorreoE.Location = new System.Drawing.Point(111, 45);
            this.txtCorreoE.MaxLength = 50;
            this.txtCorreoE.Name = "txtCorreoE";
            this.txtCorreoE.Size = new System.Drawing.Size(333, 20);
            this.txtCorreoE.TabIndex = 7;
            // 
            // lblCorreo
            // 
            this.lblCorreo.AutoSize = true;
            this.lblCorreo.ForeColor = System.Drawing.Color.White;
            this.lblCorreo.Location = new System.Drawing.Point(11, 48);
            this.lblCorreo.Name = "lblCorreo";
            this.lblCorreo.Size = new System.Drawing.Size(94, 13);
            this.lblCorreo.TabIndex = 8;
            this.lblCorreo.Text = "Correo Electrónico";
            // 
            // chkCfdi
            // 
            this.chkCfdi.AutoSize = true;
            this.chkCfdi.ForeColor = System.Drawing.Color.White;
            this.chkCfdi.Location = new System.Drawing.Point(111, 71);
            this.chkCfdi.Name = "chkCfdi";
            this.chkCfdi.Size = new System.Drawing.Size(83, 17);
            this.chkCfdi.TabIndex = 171;
            this.chkCfdi.Text = "Enviar CFDI";
            this.chkCfdi.UseVisualStyleBackColor = true;
            // 
            // gbImagen
            // 
            this.gbImagen.Controls.Add(this.picBoxImagen);
            this.gbImagen.Controls.Add(this.btnSeleccionar);
            this.gbImagen.Location = new System.Drawing.Point(111, 94);
            this.gbImagen.Name = "gbImagen";
            this.gbImagen.Size = new System.Drawing.Size(270, 253);
            this.gbImagen.TabIndex = 172;
            this.gbImagen.TabStop = false;
            // 
            // picBoxImagen
            // 
            this.picBoxImagen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxImagen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.picBoxImagen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxImagen.Location = new System.Drawing.Point(6, 48);
            this.picBoxImagen.Name = "picBoxImagen";
            this.picBoxImagen.Size = new System.Drawing.Size(258, 199);
            this.picBoxImagen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxImagen.TabIndex = 4;
            this.picBoxImagen.TabStop = false;
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnSeleccionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeleccionar.ForeColor = System.Drawing.Color.White;
            this.btnSeleccionar.Image = ((System.Drawing.Image)(resources.GetObject("btnSeleccionar.Image")));
            this.btnSeleccionar.Location = new System.Drawing.Point(6, 19);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(82, 23);
            this.btnSeleccionar.TabIndex = 3;
            this.btnSeleccionar.Text = "&Imagen";
            this.btnSeleccionar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSeleccionar.UseVisualStyleBackColor = false;
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click_1);
            // 
            // DetalleClientePersonal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(484, 414);
            this.Name = "DetalleClientePersonal";
            this.Text = "Personal";
            this.Load += new System.EventHandler(this.DetalleClientePersonal_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleClientePersonal_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.gbImagen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImagen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtCorreoE;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.CheckBox chkCfdi;
        private System.Windows.Forms.GroupBox gbImagen;
        private System.Windows.Forms.PictureBox picBoxImagen;
        private System.Windows.Forms.Button btnSeleccionar;
    }
}
