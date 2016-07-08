namespace Refaccionaria.App
{
    partial class DetalleBusquedaDeMovimientos
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetalleBusquedaDeMovimientos));
            this.dgvPartes = new System.Windows.Forms.DataGridView();
            this.parParteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parFolio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parNumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parMarca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parLinea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parCosto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFacturas = new System.Windows.Forms.DataGridView();
            this.facMovimientoInventarioDetalleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.facFolioFactura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.facFechaFactura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.facFechaRecepcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.facImporteTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.facUsuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.btnCerrrar = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.lblEncontrados = new System.Windows.Forms.Label();
            this.txtDescripcion = new LibUtil.TextoMod();
            this.txtCodigo = new LibUtil.TextoMod();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPartes
            // 
            this.dgvPartes.AllowDrop = true;
            this.dgvPartes.AllowUserToAddRows = false;
            this.dgvPartes.AllowUserToDeleteRows = false;
            this.dgvPartes.AllowUserToResizeRows = false;
            this.dgvPartes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPartes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPartes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPartes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPartes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPartes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPartes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.parParteID,
            this.parId,
            this.parFolio,
            this.parNumeroDeParte,
            this.parDescripcion,
            this.parMarca,
            this.parLinea,
            this.parCosto});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPartes.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPartes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPartes.Location = new System.Drawing.Point(12, 39);
            this.dgvPartes.Name = "dgvPartes";
            this.dgvPartes.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPartes.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPartes.RowHeadersVisible = false;
            this.dgvPartes.RowHeadersWidth = 25;
            this.dgvPartes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPartes.Size = new System.Drawing.Size(770, 190);
            this.dgvPartes.TabIndex = 3;
            this.dgvPartes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPartes_CellClick);
            this.dgvPartes.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvPartes_KeyUp);
            // 
            // parParteID
            // 
            this.parParteID.HeaderText = "ParteID";
            this.parParteID.Name = "parParteID";
            this.parParteID.ReadOnly = true;
            this.parParteID.Visible = false;
            // 
            // parId
            // 
            this.parId.HeaderText = "Id";
            this.parId.Name = "parId";
            this.parId.ReadOnly = true;
            this.parId.Visible = false;
            // 
            // parFolio
            // 
            this.parFolio.HeaderText = "Folio";
            this.parFolio.Name = "parFolio";
            this.parFolio.ReadOnly = true;
            this.parFolio.Visible = false;
            // 
            // parNumeroDeParte
            // 
            this.parNumeroDeParte.HeaderText = "No. de Parte";
            this.parNumeroDeParte.Name = "parNumeroDeParte";
            this.parNumeroDeParte.ReadOnly = true;
            // 
            // parDescripcion
            // 
            this.parDescripcion.HeaderText = "Descripción";
            this.parDescripcion.Name = "parDescripcion";
            this.parDescripcion.ReadOnly = true;
            this.parDescripcion.Width = 300;
            // 
            // parMarca
            // 
            this.parMarca.HeaderText = "Marca";
            this.parMarca.Name = "parMarca";
            this.parMarca.ReadOnly = true;
            this.parMarca.Width = 120;
            // 
            // parLinea
            // 
            this.parLinea.HeaderText = "Línea";
            this.parLinea.Name = "parLinea";
            this.parLinea.ReadOnly = true;
            this.parLinea.Width = 120;
            // 
            // parCosto
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            this.parCosto.DefaultCellStyle = dataGridViewCellStyle2;
            this.parCosto.HeaderText = "Costo";
            this.parCosto.Name = "parCosto";
            this.parCosto.ReadOnly = true;
            this.parCosto.Width = 80;
            // 
            // dgvFacturas
            // 
            this.dgvFacturas.AllowDrop = true;
            this.dgvFacturas.AllowUserToAddRows = false;
            this.dgvFacturas.AllowUserToDeleteRows = false;
            this.dgvFacturas.AllowUserToResizeRows = false;
            this.dgvFacturas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFacturas.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvFacturas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFacturas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFacturas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvFacturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFacturas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.facMovimientoInventarioDetalleID,
            this.facFolioFactura,
            this.facFechaFactura,
            this.facFechaRecepcion,
            this.facImporteTotal,
            this.facUsuario});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFacturas.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvFacturas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvFacturas.Location = new System.Drawing.Point(12, 240);
            this.dgvFacturas.Name = "dgvFacturas";
            this.dgvFacturas.ReadOnly = true;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFacturas.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvFacturas.RowHeadersVisible = false;
            this.dgvFacturas.RowHeadersWidth = 25;
            this.dgvFacturas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFacturas.Size = new System.Drawing.Size(770, 190);
            this.dgvFacturas.TabIndex = 4;
            this.dgvFacturas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFacturas_CellDoubleClick);
            // 
            // facMovimientoInventarioDetalleID
            // 
            this.facMovimientoInventarioDetalleID.HeaderText = "Id";
            this.facMovimientoInventarioDetalleID.Name = "facMovimientoInventarioDetalleID";
            this.facMovimientoInventarioDetalleID.ReadOnly = true;
            this.facMovimientoInventarioDetalleID.Visible = false;
            // 
            // facFolioFactura
            // 
            this.facFolioFactura.HeaderText = "Factura";
            this.facFolioFactura.Name = "facFolioFactura";
            this.facFolioFactura.ReadOnly = true;
            // 
            // facFechaFactura
            // 
            this.facFechaFactura.HeaderText = "Fecha Fac.";
            this.facFechaFactura.Name = "facFechaFactura";
            this.facFechaFactura.ReadOnly = true;
            this.facFechaFactura.Width = 140;
            // 
            // facFechaRecepcion
            // 
            this.facFechaRecepcion.HeaderText = "Fecha Rec.";
            this.facFechaRecepcion.Name = "facFechaRecepcion";
            this.facFechaRecepcion.ReadOnly = true;
            this.facFechaRecepcion.Width = 140;
            // 
            // facImporteTotal
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "C2";
            this.facImporteTotal.DefaultCellStyle = dataGridViewCellStyle6;
            this.facImporteTotal.HeaderText = "Total";
            this.facImporteTotal.Name = "facImporteTotal";
            this.facImporteTotal.ReadOnly = true;
            // 
            // facUsuario
            // 
            this.facUsuario.HeaderText = "Usuario";
            this.facUsuario.Name = "facUsuario";
            this.facUsuario.ReadOnly = true;
            // 
            // lblBuscar
            // 
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.BackColor = System.Drawing.Color.Transparent;
            this.lblBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuscar.ForeColor = System.Drawing.Color.White;
            this.lblBuscar.Location = new System.Drawing.Point(9, 10);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(40, 13);
            this.lblBuscar.TabIndex = 0;
            this.lblBuscar.Text = "Buscar";
            // 
            // btnCerrrar
            // 
            this.btnCerrrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCerrrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCerrrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCerrrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrrar.Image = ((System.Drawing.Image)(resources.GetObject("btnCerrrar.Image")));
            this.btnCerrrar.Location = new System.Drawing.Point(691, 437);
            this.btnCerrrar.Name = "btnCerrrar";
            this.btnCerrrar.Size = new System.Drawing.Size(91, 23);
            this.btnCerrrar.TabIndex = 7;
            this.btnCerrrar.Text = "&Cerrar";
            this.btnCerrrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCerrrar.UseVisualStyleBackColor = false;
            // 
            // btnAceptar
            // 
            this.btnAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Image = ((System.Drawing.Image)(resources.GetObject("btnAceptar.Image")));
            this.btnAceptar.Location = new System.Drawing.Point(594, 437);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(91, 23);
            this.btnAceptar.TabIndex = 6;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // lblEncontrados
            // 
            this.lblEncontrados.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEncontrados.AutoSize = true;
            this.lblEncontrados.ForeColor = System.Drawing.Color.White;
            this.lblEncontrados.Location = new System.Drawing.Point(9, 442);
            this.lblEncontrados.Name = "lblEncontrados";
            this.lblEncontrados.Size = new System.Drawing.Size(70, 13);
            this.lblEncontrados.TabIndex = 5;
            this.lblEncontrados.Text = "Encontrados:";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescripcion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDescripcion.Etiqueta = "Descripción";
            this.txtDescripcion.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtDescripcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescripcion.Location = new System.Drawing.Point(179, 7);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.PasarEnfoqueConEnter = true;
            this.txtDescripcion.SeleccionarTextoAlEnfoque = false;
            this.txtDescripcion.Size = new System.Drawing.Size(603, 21);
            this.txtDescripcion.TabIndex = 2;
            this.txtDescripcion.TextChanged += new System.EventHandler(this.txtDescripcion_TextChanged);
            this.txtDescripcion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescripcion_KeyDown);
            // 
            // txtCodigo
            // 
            this.txtCodigo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCodigo.Etiqueta = "Código";
            this.txtCodigo.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.Location = new System.Drawing.Point(55, 7);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.PasarEnfoqueConEnter = false;
            this.txtCodigo.SeleccionarTextoAlEnfoque = false;
            this.txtCodigo.Size = new System.Drawing.Size(118, 21);
            this.txtCodigo.TabIndex = 1;
            this.txtCodigo.TextChanged += new System.EventHandler(this.txtCodigo_TextChanged);
            this.txtCodigo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCodigo_KeyDown);
            // 
            // dtpDesde
            // 
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(184, 440);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(140, 442);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Desde";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(290, 442);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Hasta";
            // 
            // dtpHasta
            // 
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(331, 440);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta.TabIndex = 10;
            // 
            // DetalleBusquedaDeMovimientos
            // 
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrrar;
            this.ClientSize = new System.Drawing.Size(794, 472);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblEncontrados);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.btnCerrrar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.dgvFacturas);
            this.Controls.Add(this.dgvPartes);
            this.Controls.Add(this.lblBuscar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetalleBusquedaDeMovimientos";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.DetalleBusquedaDeMovimientos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvPartes;
        public System.Windows.Forms.DataGridView dgvFacturas;
        private System.Windows.Forms.Label lblBuscar;
        public System.Windows.Forms.Button btnCerrrar;
        public System.Windows.Forms.Button btnAceptar;
        private LibUtil.TextoMod txtDescripcion;
        private LibUtil.TextoMod txtCodigo;
        private System.Windows.Forms.Label lblEncontrados;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.DataGridViewTextBoxColumn facMovimientoInventarioDetalleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn facFolioFactura;
        private System.Windows.Forms.DataGridViewTextBoxColumn facFechaFactura;
        private System.Windows.Forms.DataGridViewTextBoxColumn facFechaRecepcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn facImporteTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn facUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn parParteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn parId;
        private System.Windows.Forms.DataGridViewTextBoxColumn parFolio;
        private System.Windows.Forms.DataGridViewTextBoxColumn parNumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn parDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn parMarca;
        private System.Windows.Forms.DataGridViewTextBoxColumn parLinea;
        private System.Windows.Forms.DataGridViewTextBoxColumn parCosto;
    }
}