namespace Refaccionaria.App
{
    partial class PartesImportar
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
            this.btnBuscarCsv = new System.Windows.Forms.Button();
            this.btnCargarCsv = new System.Windows.Forms.Button();
            this.dgvProceso = new System.Windows.Forms.DataGridView();
            this.btnImportar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.prgProgreso = new LibUtil.Progreso();
            this.txtRutaCsv = new LibUtil.TextoMod();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProceso)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBuscarCsv
            // 
            this.btnBuscarCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarCsv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnBuscarCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarCsv.ForeColor = System.Drawing.Color.White;
            this.btnBuscarCsv.Location = new System.Drawing.Point(456, 11);
            this.btnBuscarCsv.Name = "btnBuscarCsv";
            this.btnBuscarCsv.Size = new System.Drawing.Size(24, 23);
            this.btnBuscarCsv.TabIndex = 1;
            this.btnBuscarCsv.Text = "...";
            this.btnBuscarCsv.UseVisualStyleBackColor = false;
            this.btnBuscarCsv.Click += new System.EventHandler(this.btnBuscarCsv_Click);
            // 
            // btnCargarCsv
            // 
            this.btnCargarCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCargarCsv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCargarCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarCsv.ForeColor = System.Drawing.Color.White;
            this.btnCargarCsv.Location = new System.Drawing.Point(487, 11);
            this.btnCargarCsv.Name = "btnCargarCsv";
            this.btnCargarCsv.Size = new System.Drawing.Size(75, 23);
            this.btnCargarCsv.TabIndex = 3;
            this.btnCargarCsv.Text = "Car&gar";
            this.btnCargarCsv.UseVisualStyleBackColor = false;
            this.btnCargarCsv.Click += new System.EventHandler(this.btnCargarCsv_Click);
            // 
            // dgvProceso
            // 
            this.dgvProceso.AllowUserToAddRows = false;
            this.dgvProceso.AllowUserToDeleteRows = false;
            this.dgvProceso.AllowUserToResizeRows = false;
            this.dgvProceso.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProceso.BackgroundColor = System.Drawing.Color.White;
            this.dgvProceso.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProceso.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvProceso.GridColor = System.Drawing.Color.White;
            this.dgvProceso.Location = new System.Drawing.Point(12, 92);
            this.dgvProceso.Name = "dgvProceso";
            this.dgvProceso.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvProceso.RowHeadersWidth = 40;
            this.dgvProceso.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvProceso.Size = new System.Drawing.Size(550, 183);
            this.dgvProceso.TabIndex = 14;
            // 
            // btnImportar
            // 
            this.btnImportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnImportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportar.ForeColor = System.Drawing.Color.White;
            this.btnImportar.Location = new System.Drawing.Point(406, 281);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(75, 23);
            this.btnImportar.TabIndex = 15;
            this.btnImportar.Text = "&Importar";
            this.btnImportar.UseVisualStyleBackColor = false;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(487, 281);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 16;
            this.btnCancelar.Text = "&Cerrar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.prgProgreso);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(550, 48);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Progreso";
            // 
            // prgProgreso
            // 
            this.prgProgreso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgProgreso.ColorDeTexto = System.Drawing.Color.White;
            this.prgProgreso.ForeColor = System.Drawing.Color.White;
            this.prgProgreso.Location = new System.Drawing.Point(9, 19);
            this.prgProgreso.Name = "prgProgreso";
            this.prgProgreso.Size = new System.Drawing.Size(535, 20);
            this.prgProgreso.TabIndex = 17;
            // 
            // txtRutaCsv
            // 
            this.txtRutaCsv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRutaCsv.Etiqueta = "Ruta del archivo .csv";
            this.txtRutaCsv.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtRutaCsv.Location = new System.Drawing.Point(12, 12);
            this.txtRutaCsv.Name = "txtRutaCsv";
            this.txtRutaCsv.PasarEnfoqueConEnter = true;
            this.txtRutaCsv.SeleccionarTextoAlEnfoque = false;
            this.txtRutaCsv.Size = new System.Drawing.Size(441, 20);
            this.txtRutaCsv.TabIndex = 0;
            // 
            // PartesImportar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.ClientSize = new System.Drawing.Size(574, 316);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnImportar);
            this.Controls.Add(this.dgvProceso);
            this.Controls.Add(this.btnCargarCsv);
            this.Controls.Add(this.btnBuscarCsv);
            this.Controls.Add(this.txtRutaCsv);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PartesImportar";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PartesImportar";
            this.Load += new System.EventHandler(this.PartesImportar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProceso)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LibUtil.TextoMod txtRutaCsv;
        private System.Windows.Forms.Button btnBuscarCsv;
        private System.Windows.Forms.Button btnCargarCsv;
        public System.Windows.Forms.DataGridView dgvProceso;
        private System.Windows.Forms.Button btnImportar;
        private System.Windows.Forms.Button btnCancelar;
        private LibUtil.Progreso prgProgreso;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}