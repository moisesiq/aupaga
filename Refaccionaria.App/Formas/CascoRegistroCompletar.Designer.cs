namespace Refaccionaria.App
{
    partial class CascoRegistroCompletar
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CascoRegistroCompletar));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCascoRecibido = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFolioDeCobro = new System.Windows.Forms.TextBox();
            this.dgvImportesAFavor = new System.Windows.Forms.DataGridView();
            this.Sel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CascoImporteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImporteAUsar = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCerrrar = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTotalAFavor = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportesAFavor)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Casco Recibido";
            // 
            // cmbCascoRecibido
            // 
            this.cmbCascoRecibido.FormattingEnabled = true;
            this.cmbCascoRecibido.Location = new System.Drawing.Point(111, 8);
            this.cmbCascoRecibido.Name = "cmbCascoRecibido";
            this.cmbCascoRecibido.Size = new System.Drawing.Size(341, 21);
            this.cmbCascoRecibido.TabIndex = 3;
            this.cmbCascoRecibido.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cmbCascoRecibido_Format);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Folio Cobro Casco";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFolioDeCobro
            // 
            this.txtFolioDeCobro.Location = new System.Drawing.Point(111, 35);
            this.txtFolioDeCobro.Name = "txtFolioDeCobro";
            this.txtFolioDeCobro.Size = new System.Drawing.Size(124, 20);
            this.txtFolioDeCobro.TabIndex = 5;
            // 
            // dgvImportesAFavor
            // 
            this.dgvImportesAFavor.AllowUserToAddRows = false;
            this.dgvImportesAFavor.AllowUserToDeleteRows = false;
            this.dgvImportesAFavor.AllowUserToResizeRows = false;
            this.dgvImportesAFavor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvImportesAFavor.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvImportesAFavor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvImportesAFavor.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvImportesAFavor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvImportesAFavor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvImportesAFavor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Sel,
            this.CascoImporteID,
            this.Importe,
            this.ImporteAUsar});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvImportesAFavor.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvImportesAFavor.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvImportesAFavor.Location = new System.Drawing.Point(12, 63);
            this.dgvImportesAFavor.MultiSelect = false;
            this.dgvImportesAFavor.Name = "dgvImportesAFavor";
            this.dgvImportesAFavor.RowHeadersVisible = false;
            this.dgvImportesAFavor.RowHeadersWidth = 25;
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            this.dgvImportesAFavor.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvImportesAFavor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvImportesAFavor.Size = new System.Drawing.Size(440, 119);
            this.dgvImportesAFavor.TabIndex = 7;
            this.dgvImportesAFavor.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPendientes_CellValueChanged);
            // 
            // Sel
            // 
            this.Sel.HeaderText = "Sel";
            this.Sel.Name = "Sel";
            this.Sel.Width = 40;
            // 
            // CascoImporteID
            // 
            this.CascoImporteID.HeaderText = "Folio";
            this.CascoImporteID.Name = "CascoImporteID";
            this.CascoImporteID.ReadOnly = true;
            this.CascoImporteID.Width = 40;
            // 
            // Importe
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            this.Importe.DefaultCellStyle = dataGridViewCellStyle2;
            this.Importe.HeaderText = "Importe";
            this.Importe.Name = "Importe";
            this.Importe.ReadOnly = true;
            this.Importe.Width = 80;
            // 
            // ImporteAUsar
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "C2";
            this.ImporteAUsar.DefaultCellStyle = dataGridViewCellStyle3;
            this.ImporteAUsar.HeaderText = "Usar";
            this.ImporteAUsar.Name = "ImporteAUsar";
            this.ImporteAUsar.Width = 80;
            // 
            // btnCerrrar
            // 
            this.btnCerrrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCerrrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCerrrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCerrrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrrar.Image = ((System.Drawing.Image)(resources.GetObject("btnCerrrar.Image")));
            this.btnCerrrar.Location = new System.Drawing.Point(361, 188);
            this.btnCerrrar.Name = "btnCerrrar";
            this.btnCerrrar.Size = new System.Drawing.Size(91, 23);
            this.btnCerrrar.TabIndex = 9;
            this.btnCerrrar.Text = "&Cerrar";
            this.btnCerrrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCerrrar.UseVisualStyleBackColor = false;
            this.btnCerrrar.Click += new System.EventHandler(this.btnCerrrar_Click);
            // 
            // btnAceptar
            // 
            this.btnAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Image = ((System.Drawing.Image)(resources.GetObject("btnAceptar.Image")));
            this.btnAceptar.Location = new System.Drawing.Point(264, 188);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(91, 23);
            this.btnAceptar.TabIndex = 8;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(15, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Disponible a favor: $";
            // 
            // lblTotalAFavor
            // 
            this.lblTotalAFavor.AutoSize = true;
            this.lblTotalAFavor.ForeColor = System.Drawing.Color.White;
            this.lblTotalAFavor.Location = new System.Drawing.Point(136, 193);
            this.lblTotalAFavor.Name = "lblTotalAFavor";
            this.lblTotalAFavor.Size = new System.Drawing.Size(28, 13);
            this.lblTotalAFavor.TabIndex = 11;
            this.lblTotalAFavor.Text = "0.00";
            // 
            // CascoRegistroCompletar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrrar;
            this.ClientSize = new System.Drawing.Size(464, 223);
            this.Controls.Add(this.lblTotalAFavor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCerrrar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.dgvImportesAFavor);
            this.Controls.Add(this.txtFolioDeCobro);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbCascoRecibido);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CascoRegistroCompletar";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Completar registro de Casco";
            this.Load += new System.EventHandler(this.CascoRegistroCompletar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportesAFavor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCascoRecibido;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFolioDeCobro;
        private System.Windows.Forms.DataGridView dgvImportesAFavor;
        public System.Windows.Forms.Button btnCerrrar;
        public System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Sel;
        private System.Windows.Forms.DataGridViewTextBoxColumn CascoImporteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImporteAUsar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTotalAFavor;
    }
}