namespace Refaccionaria.App
{
    partial class CajaGastos
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.dgvConceptos = new System.Windows.Forms.DataGridView();
            this.EsNuevo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CategoriaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Concepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbGastos = new LibUtil.ComboEtiqueta();
            this.cmbOtrosIngresos = new LibUtil.ComboEtiqueta();
            this.txtConcepto = new LibUtil.TextoMod();
            this.txtImporte = new LibUtil.TextoMod();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConceptos)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(525, 56);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(96, 23);
            this.btnAceptar.TabIndex = 4;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // dgvConceptos
            // 
            this.dgvConceptos.AllowUserToAddRows = false;
            this.dgvConceptos.AllowUserToDeleteRows = false;
            this.dgvConceptos.AllowUserToResizeRows = false;
            this.dgvConceptos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvConceptos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvConceptos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvConceptos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvConceptos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvConceptos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConceptos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EsNuevo,
            this.ID,
            this.CategoriaID,
            this.Tipo,
            this.Concepto,
            this.Importe});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvConceptos.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvConceptos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvConceptos.Location = new System.Drawing.Point(3, 85);
            this.dgvConceptos.Name = "dgvConceptos";
            this.dgvConceptos.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvConceptos.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            this.dgvConceptos.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvConceptos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConceptos.Size = new System.Drawing.Size(618, 191);
            this.dgvConceptos.StandardTab = true;
            this.dgvConceptos.TabIndex = 5;
            this.dgvConceptos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvConceptos_KeyDown);
            // 
            // EsNuevo
            // 
            this.EsNuevo.HeaderText = "EsNuevo";
            this.EsNuevo.Name = "EsNuevo";
            this.EsNuevo.ReadOnly = true;
            this.EsNuevo.Visible = false;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // CategoriaID
            // 
            this.CategoriaID.HeaderText = "CategoriaID";
            this.CategoriaID.Name = "CategoriaID";
            this.CategoriaID.ReadOnly = true;
            this.CategoriaID.Visible = false;
            // 
            // Tipo
            // 
            this.Tipo.HeaderText = "Tipo";
            this.Tipo.Name = "Tipo";
            this.Tipo.ReadOnly = true;
            this.Tipo.Width = 80;
            // 
            // Concepto
            // 
            this.Concepto.HeaderText = "Concepto";
            this.Concepto.Name = "Concepto";
            this.Concepto.ReadOnly = true;
            this.Concepto.Width = 400;
            // 
            // Importe
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            dataGridViewCellStyle2.NullValue = null;
            this.Importe.DefaultCellStyle = dataGridViewCellStyle2;
            this.Importe.HeaderText = "Importe";
            this.Importe.Name = "Importe";
            this.Importe.ReadOnly = true;
            this.Importe.Width = 80;
            // 
            // cmbGastos
            // 
            this.cmbGastos.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbGastos.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbGastos.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbGastos.DataSource = null;
            this.cmbGastos.DisplayMember = "";
            this.cmbGastos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbGastos.Etiqueta = "Gastos";
            this.cmbGastos.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbGastos.Location = new System.Drawing.Point(3, 3);
            this.cmbGastos.Name = "cmbGastos";
            this.cmbGastos.SelectedIndex = -1;
            this.cmbGastos.SelectedItem = null;
            this.cmbGastos.SelectedText = "";
            this.cmbGastos.SelectedValue = null;
            this.cmbGastos.Size = new System.Drawing.Size(375, 21);
            this.cmbGastos.TabIndex = 0;
            this.cmbGastos.ValueMember = "";
            this.cmbGastos.SelectedIndexChanged += new System.EventHandler(this.cmbGastos_SelectedIndexChanged);
            // 
            // cmbOtrosIngresos
            // 
            this.cmbOtrosIngresos.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbOtrosIngresos.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbOtrosIngresos.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbOtrosIngresos.DataSource = null;
            this.cmbOtrosIngresos.DisplayMember = "";
            this.cmbOtrosIngresos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbOtrosIngresos.Etiqueta = "Otros Ingresos";
            this.cmbOtrosIngresos.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbOtrosIngresos.Location = new System.Drawing.Point(384, 3);
            this.cmbOtrosIngresos.Name = "cmbOtrosIngresos";
            this.cmbOtrosIngresos.SelectedIndex = -1;
            this.cmbOtrosIngresos.SelectedItem = null;
            this.cmbOtrosIngresos.SelectedText = "";
            this.cmbOtrosIngresos.SelectedValue = null;
            this.cmbOtrosIngresos.Size = new System.Drawing.Size(105, 21);
            this.cmbOtrosIngresos.TabIndex = 1;
            this.cmbOtrosIngresos.ValueMember = "";
            this.cmbOtrosIngresos.SelectedIndexChanged += new System.EventHandler(this.cmbOtrosIngresos_SelectedIndexChanged);
            // 
            // txtConcepto
            // 
            this.txtConcepto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtConcepto.Etiqueta = "Concepto de la Operación";
            this.txtConcepto.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtConcepto.Location = new System.Drawing.Point(3, 30);
            this.txtConcepto.Multiline = true;
            this.txtConcepto.Name = "txtConcepto";
            this.txtConcepto.PasarEnfoqueConEnter = true;
            this.txtConcepto.SeleccionarTextoAlEnfoque = false;
            this.txtConcepto.Size = new System.Drawing.Size(486, 49);
            this.txtConcepto.TabIndex = 2;
            // 
            // txtImporte
            // 
            this.txtImporte.Etiqueta = "Importe";
            this.txtImporte.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtImporte.Location = new System.Drawing.Point(525, 30);
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.PasarEnfoqueConEnter = true;
            this.txtImporte.SeleccionarTextoAlEnfoque = false;
            this.txtImporte.Size = new System.Drawing.Size(96, 20);
            this.txtImporte.TabIndex = 3;
            this.txtImporte.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CajaGastos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.txtImporte);
            this.Controls.Add(this.txtConcepto);
            this.Controls.Add(this.cmbOtrosIngresos);
            this.Controls.Add(this.cmbGastos);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.dgvConceptos);
            this.Name = "CajaGastos";
            this.Size = new System.Drawing.Size(624, 281);
            this.Load += new System.EventHandler(this.CajaGastos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConceptos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.DataGridView dgvConceptos;
        private LibUtil.ComboEtiqueta cmbGastos;
        private LibUtil.ComboEtiqueta cmbOtrosIngresos;
        private LibUtil.TextoMod txtConcepto;
        private LibUtil.TextoMod txtImporte;
        private System.Windows.Forms.DataGridViewTextBoxColumn EsNuevo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CategoriaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Concepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
    }
}
