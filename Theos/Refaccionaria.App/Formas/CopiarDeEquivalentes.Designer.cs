namespace Refaccionaria.App
{
    partial class CopiarDeEquivalentes
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblLinea = new System.Windows.Forms.Label();
            this.chkAplicaciones = new System.Windows.Forms.CheckBox();
            this.chkCodigosAlternos = new System.Windows.Forms.CheckBox();
            this.chkPartesComplementarias = new System.Windows.Forms.CheckBox();
            this.dgvPartes = new System.Windows.Forms.DataGridView();
            this.ParteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.pgvAvance = new LibUtil.Progreso();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Línea";
            // 
            // lblLinea
            // 
            this.lblLinea.AutoSize = true;
            this.lblLinea.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLinea.ForeColor = System.Drawing.Color.White;
            this.lblLinea.Location = new System.Drawing.Point(53, 9);
            this.lblLinea.Name = "lblLinea";
            this.lblLinea.Size = new System.Drawing.Size(40, 13);
            this.lblLinea.TabIndex = 1;
            this.lblLinea.Text = "Línea";
            // 
            // chkAplicaciones
            // 
            this.chkAplicaciones.AutoSize = true;
            this.chkAplicaciones.Checked = true;
            this.chkAplicaciones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAplicaciones.ForeColor = System.Drawing.Color.White;
            this.chkAplicaciones.Location = new System.Drawing.Point(12, 29);
            this.chkAplicaciones.Name = "chkAplicaciones";
            this.chkAplicaciones.Size = new System.Drawing.Size(86, 17);
            this.chkAplicaciones.TabIndex = 2;
            this.chkAplicaciones.Text = "Aplicaciones";
            this.chkAplicaciones.UseVisualStyleBackColor = true;
            // 
            // chkCodigosAlternos
            // 
            this.chkCodigosAlternos.AutoSize = true;
            this.chkCodigosAlternos.Checked = true;
            this.chkCodigosAlternos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCodigosAlternos.ForeColor = System.Drawing.Color.White;
            this.chkCodigosAlternos.Location = new System.Drawing.Point(104, 29);
            this.chkCodigosAlternos.Name = "chkCodigosAlternos";
            this.chkCodigosAlternos.Size = new System.Drawing.Size(105, 17);
            this.chkCodigosAlternos.TabIndex = 3;
            this.chkCodigosAlternos.Text = "Códigos Alternos";
            this.chkCodigosAlternos.UseVisualStyleBackColor = true;
            // 
            // chkPartesComplementarias
            // 
            this.chkPartesComplementarias.AutoSize = true;
            this.chkPartesComplementarias.Checked = true;
            this.chkPartesComplementarias.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPartesComplementarias.ForeColor = System.Drawing.Color.White;
            this.chkPartesComplementarias.Location = new System.Drawing.Point(215, 29);
            this.chkPartesComplementarias.Name = "chkPartesComplementarias";
            this.chkPartesComplementarias.Size = new System.Drawing.Size(139, 17);
            this.chkPartesComplementarias.TabIndex = 4;
            this.chkPartesComplementarias.Text = "Partes Complementarias";
            this.chkPartesComplementarias.UseVisualStyleBackColor = true;
            // 
            // dgvPartes
            // 
            this.dgvPartes.AllowUserToAddRows = false;
            this.dgvPartes.AllowUserToDeleteRows = false;
            this.dgvPartes.AllowUserToResizeRows = false;
            this.dgvPartes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPartes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPartes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPartes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvPartes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPartes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParteID,
            this.NumeroDeParte,
            this.Descripcion});
            this.dgvPartes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPartes.Location = new System.Drawing.Point(3, 52);
            this.dgvPartes.Name = "dgvPartes";
            this.dgvPartes.ReadOnly = true;
            this.dgvPartes.RowHeadersVisible = false;
            this.dgvPartes.RowHeadersWidth = 25;
            this.dgvPartes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPartes.Size = new System.Drawing.Size(578, 275);
            this.dgvPartes.StandardTab = true;
            this.dgvPartes.TabIndex = 20;
            // 
            // ParteID
            // 
            this.ParteID.HeaderText = "ParteID";
            this.ParteID.Name = "ParteID";
            this.ParteID.ReadOnly = true;
            this.ParteID.Visible = false;
            this.ParteID.Width = 49;
            // 
            // NumeroDeParte
            // 
            this.NumeroDeParte.HeaderText = "No. de Parte";
            this.NumeroDeParte.Name = "NumeroDeParte";
            this.NumeroDeParte.ReadOnly = true;
            this.NumeroDeParte.Width = 120;
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripción";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            this.Descripcion.Width = 400;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Image = global::Refaccionaria.App.Properties.Resources._16_Guardar;
            this.btnGuardar.Location = new System.Drawing.Point(496, 333);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(85, 23);
            this.btnGuardar.TabIndex = 21;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // pgvAvance
            // 
            this.pgvAvance.ColorDeTexto = System.Drawing.Color.White;
            this.pgvAvance.Location = new System.Drawing.Point(3, 333);
            this.pgvAvance.Name = "pgvAvance";
            this.pgvAvance.PosicionTexto = LibUtil.Progreso.PosTexto.Izquierda;
            this.pgvAvance.Size = new System.Drawing.Size(487, 20);
            this.pgvAvance.TabIndex = 22;
            // 
            // CopiarDeEquivalentes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Controls.Add(this.pgvAvance);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvPartes);
            this.Controls.Add(this.chkPartesComplementarias);
            this.Controls.Add(this.chkCodigosAlternos);
            this.Controls.Add(this.chkAplicaciones);
            this.Controls.Add(this.lblLinea);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopiarDeEquivalentes";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copiar de Equivalentes";
            this.Load += new System.EventHandler(this.CopiarDeEquivalentes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLinea;
        private System.Windows.Forms.CheckBox chkAplicaciones;
        private System.Windows.Forms.CheckBox chkCodigosAlternos;
        private System.Windows.Forms.CheckBox chkPartesComplementarias;
        private System.Windows.Forms.DataGridView dgvPartes;
        public System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private LibUtil.Progreso pgvAvance;
    }
}