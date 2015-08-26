namespace Refaccionaria.App
{
    partial class DetalleMarcaParte
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
            this.txtNombreMarca = new System.Windows.Forms.TextBox();
            this.lblNombreMarca = new System.Windows.Forms.Label();
            this.clbLineas = new System.Windows.Forms.CheckedListBox();
            this.lblPermisos = new System.Windows.Forms.Label();
            this.txtAbreviacion = new System.Windows.Forms.TextBox();
            this.lblAbreviacion = new System.Windows.Forms.Label();
            this.btnAddLogo = new System.Windows.Forms.Button();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.gpoGen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.btnAddLogo);
            this.gpoGen.Controls.Add(this.txtAbreviacion);
            this.gpoGen.Controls.Add(this.btnAddFile);
            this.gpoGen.Controls.Add(this.lblAbreviacion);
            this.gpoGen.Controls.Add(this.picLogo);
            this.gpoGen.Controls.Add(this.clbLineas);
            this.gpoGen.Controls.Add(this.lblPermisos);
            this.gpoGen.Controls.Add(this.txtNombreMarca);
            this.gpoGen.Controls.Add(this.lblNombreMarca);
            this.gpoGen.Size = new System.Drawing.Size(460, 501);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(390, 519);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(302, 519);
            this.btnGuardar.TabIndex = 1;
            // 
            // txtNombreMarca
            // 
            this.txtNombreMarca.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreMarca.Location = new System.Drawing.Point(110, 13);
            this.txtNombreMarca.MaxLength = 100;
            this.txtNombreMarca.Name = "txtNombreMarca";
            this.txtNombreMarca.Size = new System.Drawing.Size(344, 20);
            this.txtNombreMarca.TabIndex = 1;
            // 
            // lblNombreMarca
            // 
            this.lblNombreMarca.AutoSize = true;
            this.lblNombreMarca.Location = new System.Drawing.Point(15, 16);
            this.lblNombreMarca.Name = "lblNombreMarca";
            this.lblNombreMarca.Size = new System.Drawing.Size(77, 13);
            this.lblNombreMarca.TabIndex = 0;
            this.lblNombreMarca.Text = "Nombre Marca";
            // 
            // clbLineas
            // 
            this.clbLineas.CheckOnClick = true;
            this.clbLineas.FormattingEnabled = true;
            this.clbLineas.Location = new System.Drawing.Point(110, 65);
            this.clbLineas.Name = "clbLineas";
            this.clbLineas.Size = new System.Drawing.Size(344, 349);
            this.clbLineas.TabIndex = 5;
            // 
            // lblPermisos
            // 
            this.lblPermisos.AutoSize = true;
            this.lblPermisos.Location = new System.Drawing.Point(15, 65);
            this.lblPermisos.Name = "lblPermisos";
            this.lblPermisos.Size = new System.Drawing.Size(38, 13);
            this.lblPermisos.TabIndex = 4;
            this.lblPermisos.Text = "Lineas";
            // 
            // txtAbreviacion
            // 
            this.txtAbreviacion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAbreviacion.Location = new System.Drawing.Point(110, 39);
            this.txtAbreviacion.MaxLength = 100;
            this.txtAbreviacion.Name = "txtAbreviacion";
            this.txtAbreviacion.Size = new System.Drawing.Size(213, 20);
            this.txtAbreviacion.TabIndex = 3;
            // 
            // lblAbreviacion
            // 
            this.lblAbreviacion.AutoSize = true;
            this.lblAbreviacion.Location = new System.Drawing.Point(15, 42);
            this.lblAbreviacion.Name = "lblAbreviacion";
            this.lblAbreviacion.Size = new System.Drawing.Size(63, 13);
            this.lblAbreviacion.TabIndex = 2;
            this.lblAbreviacion.Text = "Abreviación";
            // 
            // btnAddLogo
            // 
            this.btnAddLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAddLogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLogo.ForeColor = System.Drawing.Color.White;
            this.btnAddLogo.Location = new System.Drawing.Point(6, 420);
            this.btnAddLogo.Name = "btnAddLogo";
            this.btnAddLogo.Size = new System.Drawing.Size(94, 23);
            this.btnAddLogo.TabIndex = 11;
            this.btnAddLogo.Text = "Cargar Imagen";
            this.btnAddLogo.UseVisualStyleBackColor = false;
            this.btnAddLogo.Click += new System.EventHandler(this.btnAddLogo_Click);
            // 
            // btnAddFile
            // 
            this.btnAddFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAddFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFile.ForeColor = System.Drawing.Color.White;
            this.btnAddFile.Location = new System.Drawing.Point(323, 472);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(131, 23);
            this.btnAddFile.TabIndex = 10;
            this.btnAddFile.Text = "Agregar Archivos";
            this.btnAddFile.UseVisualStyleBackColor = false;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(110, 420);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(99, 74);
            this.picLogo.TabIndex = 9;
            this.picLogo.TabStop = false;
            // 
            // DetalleMarcaParte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(484, 550);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "DetalleMarcaParte";
            this.Load += new System.EventHandler(this.DetalleMarcaParte_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleMarcaParte_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreMarca;
        private System.Windows.Forms.Label lblNombreMarca;
        private System.Windows.Forms.CheckedListBox clbLineas;
        private System.Windows.Forms.Label lblPermisos;
        private System.Windows.Forms.TextBox txtAbreviacion;
        private System.Windows.Forms.Label lblAbreviacion;
        private System.Windows.Forms.Button btnAddLogo;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.PictureBox picLogo;
    }
}
