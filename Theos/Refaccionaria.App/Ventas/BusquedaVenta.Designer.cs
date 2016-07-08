namespace Refaccionaria.App
{
    partial class FacturarVentas
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvVentas = new System.Windows.Forms.DataGridView();
            this.vVentaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vFolio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vImporte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtpFin = new System.Windows.Forms.DateTimePicker();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.label51 = new System.Windows.Forms.Label();
            this.txtFolio = new LibUtil.TextoMod();
            this.cmbSucursal = new LibUtil.ComboEtiqueta();
            this.dgvAFacturar = new System.Windows.Forms.DataGridView();
            this.afVentaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.afFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.afFolio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.afImporte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSeleccionados = new System.Windows.Forms.Label();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnSeleccionarTodas = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.txtObservacion = new LibUtil.TextoMod();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAFacturar)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvVentas
            // 
            this.dgvVentas.AllowUserToAddRows = false;
            this.dgvVentas.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvVentas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvVentas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvVentas.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvVentas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvVentas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvVentas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVentas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.vVentaID,
            this.vFecha,
            this.vFolio,
            this.vImporte});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvVentas.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvVentas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvVentas.Location = new System.Drawing.Point(4, 57);
            this.dgvVentas.Name = "dgvVentas";
            this.dgvVentas.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvVentas.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvVentas.RowHeadersWidth = 24;
            this.dgvVentas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVentas.Size = new System.Drawing.Size(382, 187);
            this.dgvVentas.StandardTab = true;
            this.dgvVentas.TabIndex = 4;
            this.dgvVentas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVentas_CellDoubleClick);
            this.dgvVentas.CurrentCellChanged += new System.EventHandler(this.dgvVentas_CurrentCellChanged);
            // 
            // vVentaID
            // 
            this.vVentaID.HeaderText = "VentaID";
            this.vVentaID.Name = "vVentaID";
            this.vVentaID.ReadOnly = true;
            this.vVentaID.Visible = false;
            // 
            // vFecha
            // 
            this.vFecha.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.vFecha.HeaderText = "Fecha";
            this.vFecha.Name = "vFecha";
            this.vFecha.ReadOnly = true;
            this.vFecha.Width = 120;
            // 
            // vFolio
            // 
            this.vFolio.HeaderText = "Folio";
            this.vFolio.Name = "vFolio";
            this.vFolio.ReadOnly = true;
            this.vFolio.Width = 80;
            // 
            // vImporte
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            dataGridViewCellStyle2.NullValue = null;
            this.vImporte.DefaultCellStyle = dataGridViewCellStyle2;
            this.vImporte.HeaderText = "Importe";
            this.vImporte.Name = "vImporte";
            this.vImporte.ReadOnly = true;
            // 
            // dtpFin
            // 
            this.dtpFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFin.Location = new System.Drawing.Point(285, 7);
            this.dtpFin.Name = "dtpFin";
            this.dtpFin.Size = new System.Drawing.Size(101, 20);
            this.dtpFin.TabIndex = 2;
            this.dtpFin.ValueChanged += new System.EventHandler(this.dtpFin_ValueChanged);
            // 
            // dtpInicio
            // 
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInicio.Location = new System.Drawing.Point(170, 7);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(101, 20);
            this.dtpInicio.TabIndex = 1;
            this.dtpInicio.ValueChanged += new System.EventHandler(this.dtpInicio_ValueChanged);
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.BackColor = System.Drawing.Color.Transparent;
            this.label51.ForeColor = System.Drawing.Color.White;
            this.label51.Location = new System.Drawing.Point(251, 6);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(13, 13);
            this.label51.TabIndex = 91;
            this.label51.Text = "_";
            // 
            // txtFolio
            // 
            this.txtFolio.Etiqueta = "Folio";
            this.txtFolio.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtFolio.Location = new System.Drawing.Point(653, 6);
            this.txtFolio.Name = "txtFolio";
            this.txtFolio.PasarEnfoqueConEnter = true;
            this.txtFolio.SeleccionarTextoAlEnfoque = false;
            this.txtFolio.Size = new System.Drawing.Size(120, 20);
            this.txtFolio.TabIndex = 3;
            this.txtFolio.TextChanged += new System.EventHandler(this.txtFolio_TextChanged);
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cmbSucursal.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cmbSucursal.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbSucursal.DataSource = null;
            this.cmbSucursal.DisplayMember = "";
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbSucursal.Etiqueta = "Tienda";
            this.cmbSucursal.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbSucursal.Location = new System.Drawing.Point(6, 6);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.SelectedIndex = -1;
            this.cmbSucursal.SelectedItem = null;
            this.cmbSucursal.SelectedText = "";
            this.cmbSucursal.SelectedValue = null;
            this.cmbSucursal.Size = new System.Drawing.Size(116, 21);
            this.cmbSucursal.TabIndex = 0;
            this.cmbSucursal.ValueMember = "";
            this.cmbSucursal.SelectedIndexChanged += new System.EventHandler(this.cmbSucursal_SelectedIndexChanged);
            // 
            // dgvAFacturar
            // 
            this.dgvAFacturar.AllowUserToAddRows = false;
            this.dgvAFacturar.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            this.dgvAFacturar.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvAFacturar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvAFacturar.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvAFacturar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAFacturar.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvAFacturar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAFacturar.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.afVentaID,
            this.afFecha,
            this.afFolio,
            this.afImporte});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAFacturar.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvAFacturar.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvAFacturar.Location = new System.Drawing.Point(398, 57);
            this.dgvAFacturar.Name = "dgvAFacturar";
            this.dgvAFacturar.ReadOnly = true;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAFacturar.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvAFacturar.RowHeadersWidth = 24;
            this.dgvAFacturar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAFacturar.Size = new System.Drawing.Size(375, 187);
            this.dgvAFacturar.StandardTab = true;
            this.dgvAFacturar.TabIndex = 6;
            this.dgvAFacturar.CurrentCellChanged += new System.EventHandler(this.dgvAFacturar_CurrentCellChanged);
            // 
            // afVentaID
            // 
            this.afVentaID.HeaderText = "VentaID";
            this.afVentaID.Name = "afVentaID";
            this.afVentaID.ReadOnly = true;
            this.afVentaID.Visible = false;
            // 
            // afFecha
            // 
            this.afFecha.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.afFecha.DataPropertyName = "Fecha";
            this.afFecha.HeaderText = "Fecha";
            this.afFecha.Name = "afFecha";
            this.afFecha.ReadOnly = true;
            this.afFecha.Width = 120;
            // 
            // afFolio
            // 
            this.afFolio.HeaderText = "Folio";
            this.afFolio.Name = "afFolio";
            this.afFolio.ReadOnly = true;
            this.afFolio.Width = 80;
            // 
            // afImporte
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "C2";
            dataGridViewCellStyle6.NullValue = null;
            this.afImporte.DefaultCellStyle = dataGridViewCellStyle6;
            this.afImporte.HeaderText = "Importe";
            this.afImporte.Name = "afImporte";
            this.afImporte.ReadOnly = true;
            // 
            // lblSeleccionados
            // 
            this.lblSeleccionados.AutoSize = true;
            this.lblSeleccionados.BackColor = System.Drawing.Color.Transparent;
            this.lblSeleccionados.ForeColor = System.Drawing.Color.White;
            this.lblSeleccionados.Location = new System.Drawing.Point(398, 41);
            this.lblSeleccionados.Name = "lblSeleccionados";
            this.lblSeleccionados.Size = new System.Drawing.Size(77, 13);
            this.lblSeleccionados.TabIndex = 94;
            this.lblSeleccionados.Text = "Seleccionados";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEliminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.Location = new System.Drawing.Point(398, 250);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(99, 23);
            this.btnEliminar.TabIndex = 7;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnSeleccionarTodas
            // 
            this.btnSeleccionarTodas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSeleccionarTodas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnSeleccionarTodas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeleccionarTodas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeleccionarTodas.Location = new System.Drawing.Point(6, 250);
            this.btnSeleccionarTodas.Name = "btnSeleccionarTodas";
            this.btnSeleccionarTodas.Size = new System.Drawing.Size(99, 23);
            this.btnSeleccionarTodas.TabIndex = 5;
            this.btnSeleccionarTodas.Text = "Seleccionar Todo";
            this.btnSeleccionarTodas.UseVisualStyleBackColor = false;
            this.btnSeleccionarTodas.Click += new System.EventHandler(this.btnSeleccionarTodas_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.BackColor = System.Drawing.Color.Transparent;
            this.label27.ForeColor = System.Drawing.Color.White;
            this.label27.Location = new System.Drawing.Point(3, 41);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(119, 13);
            this.label27.TabIndex = 93;
            this.label27.Text = "Documentos del Cliente";
            // 
            // txtObservacion
            // 
            this.txtObservacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtObservacion.Etiqueta = "Observación";
            this.txtObservacion.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtObservacion.Location = new System.Drawing.Point(4, 279);
            this.txtObservacion.Multiline = true;
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.PasarEnfoqueConEnter = true;
            this.txtObservacion.SeleccionarTextoAlEnfoque = false;
            this.txtObservacion.Size = new System.Drawing.Size(769, 38);
            this.txtObservacion.TabIndex = 8;
            // 
            // FacturarVentas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.dgvAFacturar);
            this.Controls.Add(this.dgvVentas);
            this.Controls.Add(this.txtObservacion);
            this.Controls.Add(this.lblSeleccionados);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnSeleccionarTodas);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.txtFolio);
            this.Controls.Add(this.dtpFin);
            this.Controls.Add(this.dtpInicio);
            this.Controls.Add(this.label51);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FacturarVentas";
            this.Size = new System.Drawing.Size(776, 320);
            this.Load += new System.EventHandler(this.BusquedaVenta_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAFacturar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label51;
        protected System.Windows.Forms.DataGridView dgvVentas;
        protected System.Windows.Forms.DateTimePicker dtpFin;
        protected System.Windows.Forms.DateTimePicker dtpInicio;
        protected LibUtil.TextoMod txtFolio;
        protected LibUtil.ComboEtiqueta cmbSucursal;
        protected System.Windows.Forms.DataGridView dgvAFacturar;
        private System.Windows.Forms.Label lblSeleccionados;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnSeleccionarTodas;
        private System.Windows.Forms.Label label27;
        private LibUtil.TextoMod txtObservacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn vVentaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn vFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn vFolio;
        private System.Windows.Forms.DataGridViewTextBoxColumn vImporte;
        private System.Windows.Forms.DataGridViewTextBoxColumn afVentaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn afFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn afFolio;
        private System.Windows.Forms.DataGridViewTextBoxColumn afImporte;
    }
}
