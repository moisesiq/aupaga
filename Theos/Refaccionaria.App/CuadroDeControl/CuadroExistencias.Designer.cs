namespace Refaccionaria.App
{
    partial class CuadroExistencias
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDatos = new System.Windows.Forms.DataGridView();
            this.ParteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Proveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Marca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Linea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Costo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Existencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CostoTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UltimaVenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UltimaCompra = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FolioFactura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblExistenciaTotal = new System.Windows.Forms.Label();
            this.lblCostoTotal = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbLinea = new LibUtil.ComboEtiqueta();
            this.cmbProveedor = new LibUtil.ComboEtiqueta();
            this.cmbMarca = new LibUtil.ComboEtiqueta();
            this.chkCostoConDescuento = new System.Windows.Forms.CheckBox();
            this.cmbSucursal = new LibUtil.ComboEtiqueta();
            this.chkSoloConExistencia = new System.Windows.Forms.CheckBox();
            this.btnExcel = new System.Windows.Forms.Button();
            this.vmsExistencia = new Refaccionaria.App.VentasMesSemana();
            this.cmbAgrupar = new LibUtil.ComboEtiqueta();
            this.chkFolioFactura = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDatos
            // 
            this.dgvDatos.AllowUserToAddRows = false;
            this.dgvDatos.AllowUserToDeleteRows = false;
            this.dgvDatos.AllowUserToResizeRows = false;
            this.dgvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDatos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParteID,
            this.NumeroDeParte,
            this.Descripcion,
            this.Proveedor,
            this.Marca,
            this.Linea,
            this.Costo,
            this.Existencia,
            this.CostoTotal,
            this.UltimaVenta,
            this.UltimaCompra,
            this.FolioFactura});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDatos.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDatos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDatos.Location = new System.Drawing.Point(3, 32);
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.ReadOnly = true;
            this.dgvDatos.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDatos.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDatos.RowHeadersVisible = false;
            this.dgvDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDatos.Size = new System.Drawing.Size(1194, 363);
            this.dgvDatos.TabIndex = 9;
            this.dgvDatos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDatos_CellClick);
            // 
            // ParteID
            // 
            this.ParteID.HeaderText = "ParteID";
            this.ParteID.Name = "ParteID";
            this.ParteID.ReadOnly = true;
            this.ParteID.Visible = false;
            // 
            // NumeroDeParte
            // 
            this.NumeroDeParte.HeaderText = "No. Parte";
            this.NumeroDeParte.Name = "NumeroDeParte";
            this.NumeroDeParte.ReadOnly = true;
            this.NumeroDeParte.Width = 120;
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripción";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            this.Descripcion.Width = 320;
            // 
            // Proveedor
            // 
            this.Proveedor.HeaderText = "Proveedor";
            this.Proveedor.Name = "Proveedor";
            this.Proveedor.ReadOnly = true;
            this.Proveedor.Width = 140;
            // 
            // Marca
            // 
            this.Marca.HeaderText = "Marca";
            this.Marca.Name = "Marca";
            this.Marca.ReadOnly = true;
            this.Marca.Width = 140;
            // 
            // Linea
            // 
            this.Linea.HeaderText = "Línea";
            this.Linea.Name = "Linea";
            this.Linea.ReadOnly = true;
            this.Linea.Width = 140;
            // 
            // Costo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Costo.DefaultCellStyle = dataGridViewCellStyle1;
            this.Costo.HeaderText = "Costo";
            this.Costo.Name = "Costo";
            this.Costo.ReadOnly = true;
            this.Costo.Width = 80;
            // 
            // Existencia
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N2";
            this.Existencia.DefaultCellStyle = dataGridViewCellStyle2;
            this.Existencia.HeaderText = "Existencia";
            this.Existencia.Name = "Existencia";
            this.Existencia.ReadOnly = true;
            this.Existencia.Width = 60;
            // 
            // CostoTotal
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "C2";
            this.CostoTotal.DefaultCellStyle = dataGridViewCellStyle3;
            this.CostoTotal.HeaderText = "Costo total";
            this.CostoTotal.Name = "CostoTotal";
            this.CostoTotal.ReadOnly = true;
            this.CostoTotal.Width = 80;
            // 
            // UltimaVenta
            // 
            this.UltimaVenta.HeaderText = "Ult. Venta";
            this.UltimaVenta.Name = "UltimaVenta";
            this.UltimaVenta.ReadOnly = true;
            this.UltimaVenta.Width = 136;
            // 
            // UltimaCompra
            // 
            this.UltimaCompra.HeaderText = "Ult. Compra";
            this.UltimaCompra.Name = "UltimaCompra";
            this.UltimaCompra.ReadOnly = true;
            this.UltimaCompra.Width = 136;
            // 
            // FolioFactura
            // 
            this.FolioFactura.HeaderText = "Folio Com.";
            this.FolioFactura.Name = "FolioFactura";
            this.FolioFactura.ReadOnly = true;
            this.FolioFactura.Width = 80;
            // 
            // btnActualizar
            // 
            this.btnActualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(1041, 5);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(75, 23);
            this.btnActualizar.TabIndex = 7;
            this.btnActualizar.Text = "&Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(884, 398);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Existencia";
            // 
            // lblExistenciaTotal
            // 
            this.lblExistenciaTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExistenciaTotal.ForeColor = System.Drawing.Color.White;
            this.lblExistenciaTotal.Location = new System.Drawing.Point(945, 398);
            this.lblExistenciaTotal.Name = "lblExistenciaTotal";
            this.lblExistenciaTotal.Size = new System.Drawing.Size(80, 13);
            this.lblExistenciaTotal.TabIndex = 3;
            this.lblExistenciaTotal.Text = "0";
            this.lblExistenciaTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblCostoTotal
            // 
            this.lblCostoTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCostoTotal.ForeColor = System.Drawing.Color.White;
            this.lblCostoTotal.Location = new System.Drawing.Point(1094, 398);
            this.lblCostoTotal.Name = "lblCostoTotal";
            this.lblCostoTotal.Size = new System.Drawing.Size(100, 13);
            this.lblCostoTotal.TabIndex = 5;
            this.lblCostoTotal.Text = "$0.00";
            this.lblCostoTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(1031, 398);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Costo total";
            // 
            // cmbLinea
            // 
            this.cmbLinea.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbLinea.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbLinea.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbLinea.DataSource = null;
            this.cmbLinea.DisplayMember = "";
            this.cmbLinea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbLinea.Etiqueta = "Línea";
            this.cmbLinea.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbLinea.Location = new System.Drawing.Point(501, 5);
            this.cmbLinea.Name = "cmbLinea";
            this.cmbLinea.SelectedIndex = -1;
            this.cmbLinea.SelectedItem = null;
            this.cmbLinea.SelectedText = "";
            this.cmbLinea.SelectedValue = null;
            this.cmbLinea.Size = new System.Drawing.Size(223, 21);
            this.cmbLinea.TabIndex = 3;
            this.cmbLinea.ValueMember = "";
            // 
            // cmbProveedor
            // 
            this.cmbProveedor.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbProveedor.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbProveedor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbProveedor.DataSource = null;
            this.cmbProveedor.DisplayMember = "";
            this.cmbProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbProveedor.Etiqueta = "Proveedor";
            this.cmbProveedor.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbProveedor.Location = new System.Drawing.Point(149, 5);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.SelectedIndex = -1;
            this.cmbProveedor.SelectedItem = null;
            this.cmbProveedor.SelectedText = "";
            this.cmbProveedor.SelectedValue = null;
            this.cmbProveedor.Size = new System.Drawing.Size(170, 21);
            this.cmbProveedor.TabIndex = 1;
            this.cmbProveedor.ValueMember = "";
            // 
            // cmbMarca
            // 
            this.cmbMarca.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbMarca.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbMarca.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbMarca.DataSource = null;
            this.cmbMarca.DisplayMember = "";
            this.cmbMarca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbMarca.Etiqueta = "Marca";
            this.cmbMarca.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbMarca.Location = new System.Drawing.Point(325, 5);
            this.cmbMarca.Name = "cmbMarca";
            this.cmbMarca.SelectedIndex = -1;
            this.cmbMarca.SelectedItem = null;
            this.cmbMarca.SelectedText = "";
            this.cmbMarca.SelectedValue = null;
            this.cmbMarca.Size = new System.Drawing.Size(170, 21);
            this.cmbMarca.TabIndex = 2;
            this.cmbMarca.ValueMember = "";
            // 
            // chkCostoConDescuento
            // 
            this.chkCostoConDescuento.AutoSize = true;
            this.chkCostoConDescuento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkCostoConDescuento.ForeColor = System.Drawing.Color.White;
            this.chkCostoConDescuento.Location = new System.Drawing.Point(828, 9);
            this.chkCostoConDescuento.Name = "chkCostoConDescuento";
            this.chkCostoConDescuento.Size = new System.Drawing.Size(102, 17);
            this.chkCostoConDescuento.TabIndex = 5;
            this.chkCostoConDescuento.Text = "Costo con Desc.";
            this.chkCostoConDescuento.UseVisualStyleBackColor = true;
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSucursal.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSucursal.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbSucursal.DataSource = null;
            this.cmbSucursal.DisplayMember = "";
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbSucursal.Etiqueta = "Sucursal";
            this.cmbSucursal.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbSucursal.Location = new System.Drawing.Point(3, 5);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.SelectedIndex = -1;
            this.cmbSucursal.SelectedItem = null;
            this.cmbSucursal.SelectedText = "";
            this.cmbSucursal.SelectedValue = null;
            this.cmbSucursal.Size = new System.Drawing.Size(140, 21);
            this.cmbSucursal.TabIndex = 0;
            this.cmbSucursal.ValueMember = "";
            // 
            // chkSoloConExistencia
            // 
            this.chkSoloConExistencia.AutoSize = true;
            this.chkSoloConExistencia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSoloConExistencia.ForeColor = System.Drawing.Color.White;
            this.chkSoloConExistencia.Location = new System.Drawing.Point(939, 9);
            this.chkSoloConExistencia.Name = "chkSoloConExistencia";
            this.chkSoloConExistencia.Size = new System.Drawing.Size(93, 17);
            this.chkSoloConExistencia.TabIndex = 6;
            this.chkSoloConExistencia.Text = "Sólo con Exist.";
            this.chkSoloConExistencia.UseVisualStyleBackColor = true;
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcel.ForeColor = System.Drawing.Color.White;
            this.btnExcel.Location = new System.Drawing.Point(1122, 5);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(75, 23);
            this.btnExcel.TabIndex = 8;
            this.btnExcel.Text = "&Excel";
            this.btnExcel.UseVisualStyleBackColor = false;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // vmsExistencia
            // 
            this.vmsExistencia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.vmsExistencia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.vmsExistencia.Location = new System.Drawing.Point(0, 414);
            this.vmsExistencia.Name = "vmsExistencia";
            this.vmsExistencia.Size = new System.Drawing.Size(1280, 100);
            this.vmsExistencia.TabIndex = 10;
            // 
            // cmbAgrupar
            // 
            this.cmbAgrupar.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAgrupar.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAgrupar.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbAgrupar.DataSource = null;
            this.cmbAgrupar.DisplayMember = "";
            this.cmbAgrupar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbAgrupar.Etiqueta = "Agrupar";
            this.cmbAgrupar.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbAgrupar.Location = new System.Drawing.Point(730, 5);
            this.cmbAgrupar.Name = "cmbAgrupar";
            this.cmbAgrupar.SelectedIndex = -1;
            this.cmbAgrupar.SelectedItem = null;
            this.cmbAgrupar.SelectedText = "";
            this.cmbAgrupar.SelectedValue = null;
            this.cmbAgrupar.Size = new System.Drawing.Size(92, 21);
            this.cmbAgrupar.TabIndex = 4;
            this.cmbAgrupar.ValueMember = "";
            // 
            // chkFolioFactura
            // 
            this.chkFolioFactura.AutoSize = true;
            this.chkFolioFactura.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkFolioFactura.ForeColor = System.Drawing.Color.White;
            this.chkFolioFactura.Location = new System.Drawing.Point(1038, 9);
            this.chkFolioFactura.Name = "chkFolioFactura";
            this.chkFolioFactura.Size = new System.Drawing.Size(67, 17);
            this.chkFolioFactura.TabIndex = 11;
            this.chkFolioFactura.Text = "Con Folio";
            this.chkFolioFactura.UseVisualStyleBackColor = true;
            // 
            // CuadroExistencias
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.chkFolioFactura);
            this.Controls.Add(this.cmbAgrupar);
            this.Controls.Add(this.vmsExistencia);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.chkSoloConExistencia);
            this.Controls.Add(this.chkCostoConDescuento);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.cmbLinea);
            this.Controls.Add(this.cmbProveedor);
            this.Controls.Add(this.cmbMarca);
            this.Controls.Add(this.lblCostoTotal);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblExistenciaTotal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.dgvDatos);
            this.Name = "CuadroExistencias";
            this.Size = new System.Drawing.Size(1200, 517);
            this.Load += new System.EventHandler(this.CuadroExistencias_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.DataGridView dgvDatos;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblExistenciaTotal;
        private System.Windows.Forms.Label lblCostoTotal;
        private System.Windows.Forms.Label label4;
        private LibUtil.ComboEtiqueta cmbLinea;
        private LibUtil.ComboEtiqueta cmbProveedor;
        private LibUtil.ComboEtiqueta cmbMarca;
        private System.Windows.Forms.CheckBox chkCostoConDescuento;
        private LibUtil.ComboEtiqueta cmbSucursal;
        private System.Windows.Forms.CheckBox chkSoloConExistencia;
        private System.Windows.Forms.Button btnExcel;
        private VentasMesSemana vmsExistencia;
        private LibUtil.ComboEtiqueta cmbAgrupar;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Proveedor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Marca;
        private System.Windows.Forms.DataGridViewTextBoxColumn Linea;
        private System.Windows.Forms.DataGridViewTextBoxColumn Costo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Existencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn CostoTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn UltimaVenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn UltimaCompra;
        private System.Windows.Forms.DataGridViewTextBoxColumn FolioFactura;
        private System.Windows.Forms.CheckBox chkFolioFactura;
    }
}
