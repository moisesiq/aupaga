namespace Refaccionaria.App
{
    partial class DetalleImagen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetalleImagen));
            this.picBoxImagen = new System.Windows.Forms.PictureBox();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.gpoGen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImagen)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.btnSeleccionar);
            this.gpoGen.Controls.Add(this.picBoxImagen);
            this.gpoGen.Size = new System.Drawing.Size(270, 245);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(200, 286);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(112, 286);
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
            this.picBoxImagen.Size = new System.Drawing.Size(255, 191);
            this.picBoxImagen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxImagen.TabIndex = 1;
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
            this.btnSeleccionar.TabIndex = 2;
            this.btnSeleccionar.Text = "&Imagen";
            this.btnSeleccionar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSeleccionar.UseVisualStyleBackColor = false;
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click);
            // 
            // DetalleImagen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(294, 317);
            this.KeyPreview = true;
            this.Name = "DetalleImagen";
            this.Text = "Seleccionar Imagen";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleImagen_KeyDown);
            this.gpoGen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImagen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBoxImagen;
        private System.Windows.Forms.Button btnSeleccionar;
    }
}
