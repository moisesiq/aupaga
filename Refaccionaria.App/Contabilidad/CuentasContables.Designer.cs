namespace Refaccionaria.App
{
    partial class CuentasContables
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvGastos = new System.Windows.Forms.DataGridView();
            this.Gastos_ContaEgresoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Folio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Tienda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_FormaDePago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Usuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Observaciones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnEgresoEditar = new System.Windows.Forms.Button();
            this.btnCuentaEditar = new System.Windows.Forms.Button();
            this.btnEgresoDevengar = new System.Windows.Forms.Button();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnCuentaAgregar = new System.Windows.Forms.Button();
            this.btnGastoAgregar = new System.Windows.Forms.Button();
            this.btnCuentaEliminar = new System.Windows.Forms.Button();
            this.btnCuentaMover = new System.Windows.Forms.Button();
            this.dgvGastoDev = new System.Windows.Forms.DataGridView();
            this.ContaEgresoDevengadoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tgvCuentas = new AdvancedDataGridView.TreeGridView();
            this.txtBusqueda = new Refaccionaria.Negocio.TextoMod();
            this.Cuentas_Id = new AdvancedDataGridView.TreeGridColumn();
            this.Cuentas_Cuenta = new AdvancedDataGridView.TreeGridColumn();
            this.Cuentas_Porcentaje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Fiscal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Matriz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Sucursal2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Sucursal3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_ImporteDev = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGastos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGastoDev)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tgvCuentas)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Fechas";
            // 
            // dgvGastos
            // 
            this.dgvGastos.AllowUserToAddRows = false;
            this.dgvGastos.AllowUserToDeleteRows = false;
            this.dgvGastos.AllowUserToResizeRows = false;
            this.dgvGastos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvGastos.BackgroundColor = System.Drawing.Color.White;
            this.dgvGastos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvGastos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvGastos.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvGastos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGastos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Gastos_ContaEgresoID,
            this.Gastos_Fecha,
            this.Gastos_Folio,
            this.Gastos_Importe,
            this.Gastos_Tienda,
            this.Gastos_FormaDePago,
            this.Gastos_Usuario,
            this.Gastos_Observaciones});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvGastos.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvGastos.GridColor = System.Drawing.Color.White;
            this.dgvGastos.Location = new System.Drawing.Point(834, 29);
            this.dgvGastos.Name = "dgvGastos";
            this.dgvGastos.ReadOnly = true;
            this.dgvGastos.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGastos.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvGastos.RowHeadersVisible = false;
            this.dgvGastos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGastos.Size = new System.Drawing.Size(602, 160);
            this.dgvGastos.StandardTab = true;
            this.dgvGastos.TabIndex = 9;
            this.dgvGastos.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGastos_CellDoubleClick);
            this.dgvGastos.CurrentCellChanged += new System.EventHandler(this.dgvGastos_CurrentCellChanged);
            this.dgvGastos.DoubleClick += new System.EventHandler(this.dgvGastos_DoubleClick);
            this.dgvGastos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvGastos_KeyDown);
            // 
            // Gastos_ContaEgresoID
            // 
            this.Gastos_ContaEgresoID.HeaderText = "ContaEgresoID";
            this.Gastos_ContaEgresoID.Name = "Gastos_ContaEgresoID";
            this.Gastos_ContaEgresoID.ReadOnly = true;
            this.Gastos_ContaEgresoID.Visible = false;
            // 
            // Gastos_Fecha
            // 
            this.Gastos_Fecha.HeaderText = "Fecha";
            this.Gastos_Fecha.Name = "Gastos_Fecha";
            this.Gastos_Fecha.ReadOnly = true;
            this.Gastos_Fecha.Width = 136;
            // 
            // Gastos_Folio
            // 
            this.Gastos_Folio.DataPropertyName = "Fecha";
            this.Gastos_Folio.HeaderText = "Folio";
            this.Gastos_Folio.Name = "Gastos_Folio";
            this.Gastos_Folio.ReadOnly = true;
            this.Gastos_Folio.Width = 50;
            // 
            // Gastos_Importe
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Gastos_Importe.DefaultCellStyle = dataGridViewCellStyle1;
            this.Gastos_Importe.HeaderText = "Importe";
            this.Gastos_Importe.Name = "Gastos_Importe";
            this.Gastos_Importe.ReadOnly = true;
            this.Gastos_Importe.Width = 70;
            // 
            // Gastos_Tienda
            // 
            this.Gastos_Tienda.HeaderText = "Tienda";
            this.Gastos_Tienda.Name = "Gastos_Tienda";
            this.Gastos_Tienda.ReadOnly = true;
            this.Gastos_Tienda.Width = 60;
            // 
            // Gastos_FormaDePago
            // 
            this.Gastos_FormaDePago.HeaderText = "F.Pago";
            this.Gastos_FormaDePago.Name = "Gastos_FormaDePago";
            this.Gastos_FormaDePago.ReadOnly = true;
            this.Gastos_FormaDePago.Width = 70;
            // 
            // Gastos_Usuario
            // 
            this.Gastos_Usuario.HeaderText = "Usuario";
            this.Gastos_Usuario.Name = "Gastos_Usuario";
            this.Gastos_Usuario.ReadOnly = true;
            this.Gastos_Usuario.Width = 60;
            // 
            // Gastos_Observaciones
            // 
            this.Gastos_Observaciones.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Gastos_Observaciones.HeaderText = "Observaciones";
            this.Gastos_Observaciones.Name = "Gastos_Observaciones";
            this.Gastos_Observaciones.ReadOnly = true;
            this.Gastos_Observaciones.Width = 103;
            // 
            // btnEgresoEditar
            // 
            this.btnEgresoEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEgresoEditar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnEgresoEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEgresoEditar.ForeColor = System.Drawing.Color.White;
            this.btnEgresoEditar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnEgresoEditar.Location = new System.Drawing.Point(1361, 226);
            this.btnEgresoEditar.Name = "btnEgresoEditar";
            this.btnEgresoEditar.Size = new System.Drawing.Size(75, 25);
            this.btnEgresoEditar.TabIndex = 12;
            this.btnEgresoEditar.Text = "Editar";
            this.btnEgresoEditar.UseVisualStyleBackColor = false;
            this.btnEgresoEditar.Click += new System.EventHandler(this.btnEgresoEditar_Click);
            // 
            // btnCuentaEditar
            // 
            this.btnCuentaEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCuentaEditar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaEditar.ForeColor = System.Drawing.Color.White;
            this.btnCuentaEditar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaEditar.Location = new System.Drawing.Point(1268, 3);
            this.btnCuentaEditar.Name = "btnCuentaEditar";
            this.btnCuentaEditar.Size = new System.Drawing.Size(49, 22);
            this.btnCuentaEditar.TabIndex = 6;
            this.btnCuentaEditar.Text = "Editar";
            this.btnCuentaEditar.UseVisualStyleBackColor = false;
            this.btnCuentaEditar.Visible = false;
            this.btnCuentaEditar.Click += new System.EventHandler(this.btnCuentaEditar_Click);
            // 
            // btnEgresoDevengar
            // 
            this.btnEgresoDevengar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEgresoDevengar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnEgresoDevengar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEgresoDevengar.ForeColor = System.Drawing.Color.White;
            this.btnEgresoDevengar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnEgresoDevengar.Location = new System.Drawing.Point(1361, 195);
            this.btnEgresoDevengar.Name = "btnEgresoDevengar";
            this.btnEgresoDevengar.Size = new System.Drawing.Size(75, 25);
            this.btnEgresoDevengar.TabIndex = 11;
            this.btnEgresoDevengar.Text = "Devengar";
            this.btnEgresoDevengar.UseVisualStyleBackColor = false;
            this.btnEgresoDevengar.Click += new System.EventHandler(this.btnEgresoDevengar_Click);
            // 
            // dtpDesde
            // 
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(54, 3);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde.TabIndex = 0;
            this.dtpDesde.ValueChanged += new System.EventHandler(this.dtpDesde_ValueChanged);
            // 
            // dtpHasta
            // 
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(160, 3);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta.TabIndex = 1;
            this.dtpHasta.ValueChanged += new System.EventHandler(this.dtpHasta_ValueChanged);
            // 
            // btnCuentaAgregar
            // 
            this.btnCuentaAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCuentaAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaAgregar.Enabled = false;
            this.btnCuentaAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaAgregar.ForeColor = System.Drawing.Color.White;
            this.btnCuentaAgregar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaAgregar.Location = new System.Drawing.Point(1230, 3);
            this.btnCuentaAgregar.Name = "btnCuentaAgregar";
            this.btnCuentaAgregar.Size = new System.Drawing.Size(32, 22);
            this.btnCuentaAgregar.TabIndex = 5;
            this.btnCuentaAgregar.Text = "+";
            this.btnCuentaAgregar.UseVisualStyleBackColor = false;
            this.btnCuentaAgregar.Visible = false;
            this.btnCuentaAgregar.Click += new System.EventHandler(this.btnCuentaAgregar_Click);
            // 
            // btnGastoAgregar
            // 
            this.btnGastoAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGastoAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGastoAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGastoAgregar.ForeColor = System.Drawing.Color.White;
            this.btnGastoAgregar.Location = new System.Drawing.Point(1361, 3);
            this.btnGastoAgregar.Name = "btnGastoAgregar";
            this.btnGastoAgregar.Size = new System.Drawing.Size(75, 23);
            this.btnGastoAgregar.TabIndex = 8;
            this.btnGastoAgregar.Text = "+ &Gasto";
            this.btnGastoAgregar.UseVisualStyleBackColor = false;
            this.btnGastoAgregar.Click += new System.EventHandler(this.btnGastoAgregar_Click);
            // 
            // btnCuentaEliminar
            // 
            this.btnCuentaEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCuentaEliminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaEliminar.Enabled = false;
            this.btnCuentaEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaEliminar.ForeColor = System.Drawing.Color.White;
            this.btnCuentaEliminar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaEliminar.Location = new System.Drawing.Point(1323, 3);
            this.btnCuentaEliminar.Name = "btnCuentaEliminar";
            this.btnCuentaEliminar.Size = new System.Drawing.Size(32, 22);
            this.btnCuentaEliminar.TabIndex = 7;
            this.btnCuentaEliminar.Text = "-";
            this.btnCuentaEliminar.UseVisualStyleBackColor = false;
            this.btnCuentaEliminar.Visible = false;
            this.btnCuentaEliminar.Click += new System.EventHandler(this.btnCuentaEliminar_Click);
            // 
            // btnCuentaMover
            // 
            this.btnCuentaMover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCuentaMover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaMover.Enabled = false;
            this.btnCuentaMover.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaMover.ForeColor = System.Drawing.Color.White;
            this.btnCuentaMover.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaMover.Location = new System.Drawing.Point(1167, 3);
            this.btnCuentaMover.Name = "btnCuentaMover";
            this.btnCuentaMover.Size = new System.Drawing.Size(57, 22);
            this.btnCuentaMover.TabIndex = 4;
            this.btnCuentaMover.Text = "Cambiar";
            this.btnCuentaMover.UseVisualStyleBackColor = false;
            this.btnCuentaMover.Visible = false;
            this.btnCuentaMover.Click += new System.EventHandler(this.btnCuentaMover_Click);
            // 
            // dgvGastoDev
            // 
            this.dgvGastoDev.AllowUserToAddRows = false;
            this.dgvGastoDev.AllowUserToDeleteRows = false;
            this.dgvGastoDev.AllowUserToResizeRows = false;
            this.dgvGastoDev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvGastoDev.BackgroundColor = System.Drawing.Color.White;
            this.dgvGastoDev.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvGastoDev.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvGastoDev.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvGastoDev.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGastoDev.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ContaEgresoDevengadoID,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvGastoDev.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvGastoDev.GridColor = System.Drawing.Color.White;
            this.dgvGastoDev.Location = new System.Drawing.Point(834, 195);
            this.dgvGastoDev.Name = "dgvGastoDev";
            this.dgvGastoDev.ReadOnly = true;
            this.dgvGastoDev.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGastoDev.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvGastoDev.RowHeadersVisible = false;
            this.dgvGastoDev.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGastoDev.Size = new System.Drawing.Size(383, 244);
            this.dgvGastoDev.StandardTab = true;
            this.dgvGastoDev.TabIndex = 10;
            // 
            // ContaEgresoDevengadoID
            // 
            this.ContaEgresoDevengadoID.HeaderText = "ContaEgresoDevengadoID";
            this.ContaEgresoDevengadoID.Name = "ContaEgresoDevengadoID";
            this.ContaEgresoDevengadoID.ReadOnly = true;
            this.ContaEgresoDevengadoID.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Fecha";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 136;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Tienda";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 120;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "C2";
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn3.HeaderText = "Importe";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // tgvCuentas
            // 
            this.tgvCuentas.AllowUserToAddRows = false;
            this.tgvCuentas.AllowUserToDeleteRows = false;
            this.tgvCuentas.AllowUserToResizeRows = false;
            this.tgvCuentas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tgvCuentas.BackgroundColor = System.Drawing.Color.White;
            this.tgvCuentas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tgvCuentas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.tgvCuentas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Cuentas_Id,
            this.Cuentas_Cuenta,
            this.Cuentas_Porcentaje,
            this.Cuentas_Fiscal,
            this.Cuentas_Total,
            this.Cuentas_Matriz,
            this.Cuentas_Sucursal2,
            this.Cuentas_Sucursal3,
            this.Cuentas_ImporteDev});
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tgvCuentas.DefaultCellStyle = dataGridViewCellStyle13;
            this.tgvCuentas.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tgvCuentas.ImageList = null;
            this.tgvCuentas.Location = new System.Drawing.Point(4, 29);
            this.tgvCuentas.Name = "tgvCuentas";
            this.tgvCuentas.ReadOnly = true;
            this.tgvCuentas.RowHeadersVisible = false;
            this.tgvCuentas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tgvCuentas.Size = new System.Drawing.Size(824, 410);
            this.tgvCuentas.TabIndex = 3;
            this.tgvCuentas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tgvCuentas_CellClick);
            this.tgvCuentas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tgvCuentas_CellDoubleClick);
            this.tgvCuentas.CurrentCellChanged += new System.EventHandler(this.tgvCuentas_CurrentCellChanged);
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Etiqueta = "Búsqueda";
            this.txtBusqueda.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtBusqueda.Location = new System.Drawing.Point(266, 3);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.PasarEnfoqueConEnter = false;
            this.txtBusqueda.SeleccionarTextoAlEnfoque = true;
            this.txtBusqueda.Size = new System.Drawing.Size(562, 20);
            this.txtBusqueda.TabIndex = 2;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBusqueda_KeyDown);
            // 
            // Cuentas_Id
            // 
            this.Cuentas_Id.DefaultNodeImage = null;
            this.Cuentas_Id.Frozen = true;
            this.Cuentas_Id.HeaderText = "Id";
            this.Cuentas_Id.Name = "Cuentas_Id";
            this.Cuentas_Id.ReadOnly = true;
            this.Cuentas_Id.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Cuentas_Id.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cuentas_Id.Visible = false;
            // 
            // Cuentas_Cuenta
            // 
            this.Cuentas_Cuenta.DefaultNodeImage = null;
            this.Cuentas_Cuenta.Frozen = true;
            this.Cuentas_Cuenta.HeaderText = "Cuenta";
            this.Cuentas_Cuenta.Name = "Cuentas_Cuenta";
            this.Cuentas_Cuenta.ReadOnly = true;
            this.Cuentas_Cuenta.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cuentas_Cuenta.Width = 360;
            // 
            // Cuentas_Porcentaje
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "0\\%";
            dataGridViewCellStyle7.NullValue = null;
            this.Cuentas_Porcentaje.DefaultCellStyle = dataGridViewCellStyle7;
            this.Cuentas_Porcentaje.HeaderText = "%";
            this.Cuentas_Porcentaje.Name = "Cuentas_Porcentaje";
            this.Cuentas_Porcentaje.ReadOnly = true;
            this.Cuentas_Porcentaje.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cuentas_Porcentaje.Width = 40;
            // 
            // Cuentas_Fiscal
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "C2";
            this.Cuentas_Fiscal.DefaultCellStyle = dataGridViewCellStyle8;
            this.Cuentas_Fiscal.HeaderText = "Fiscal";
            this.Cuentas_Fiscal.Name = "Cuentas_Fiscal";
            this.Cuentas_Fiscal.ReadOnly = true;
            this.Cuentas_Fiscal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cuentas_Fiscal.Visible = false;
            // 
            // Cuentas_Total
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "C2";
            this.Cuentas_Total.DefaultCellStyle = dataGridViewCellStyle9;
            this.Cuentas_Total.HeaderText = "Total";
            this.Cuentas_Total.Name = "Cuentas_Total";
            this.Cuentas_Total.ReadOnly = true;
            this.Cuentas_Total.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cuentas_Matriz
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.Format = "C2";
            this.Cuentas_Matriz.DefaultCellStyle = dataGridViewCellStyle10;
            this.Cuentas_Matriz.HeaderText = "Matriz";
            this.Cuentas_Matriz.Name = "Cuentas_Matriz";
            this.Cuentas_Matriz.ReadOnly = true;
            this.Cuentas_Matriz.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cuentas_Sucursal2
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle11.Format = "C2";
            this.Cuentas_Sucursal2.DefaultCellStyle = dataGridViewCellStyle11;
            this.Cuentas_Sucursal2.HeaderText = "Suc 02";
            this.Cuentas_Sucursal2.Name = "Cuentas_Sucursal2";
            this.Cuentas_Sucursal2.ReadOnly = true;
            this.Cuentas_Sucursal2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cuentas_Sucursal3
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle12.Format = "C2";
            this.Cuentas_Sucursal3.DefaultCellStyle = dataGridViewCellStyle12;
            this.Cuentas_Sucursal3.HeaderText = "Suc 03";
            this.Cuentas_Sucursal3.Name = "Cuentas_Sucursal3";
            this.Cuentas_Sucursal3.ReadOnly = true;
            this.Cuentas_Sucursal3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cuentas_ImporteDev
            // 
            this.Cuentas_ImporteDev.HeaderText = "Importe Dev";
            this.Cuentas_ImporteDev.Name = "Cuentas_ImporteDev";
            this.Cuentas_ImporteDev.ReadOnly = true;
            this.Cuentas_ImporteDev.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cuentas_ImporteDev.Visible = false;
            // 
            // CuentasContables
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.txtBusqueda);
            this.Controls.Add(this.tgvCuentas);
            this.Controls.Add(this.dgvGastoDev);
            this.Controls.Add(this.btnCuentaMover);
            this.Controls.Add(this.btnCuentaEliminar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvGastos);
            this.Controls.Add(this.btnEgresoEditar);
            this.Controls.Add(this.btnCuentaEditar);
            this.Controls.Add(this.btnEgresoDevengar);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.btnCuentaAgregar);
            this.Controls.Add(this.btnGastoAgregar);
            this.Name = "CuentasContables";
            this.Size = new System.Drawing.Size(1439, 442);
            this.Load += new System.EventHandler(this.CuentasContables_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGastos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGastoDev)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tgvCuentas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvGastos;
        private System.Windows.Forms.Button btnEgresoEditar;
        private System.Windows.Forms.Button btnCuentaEditar;
        private System.Windows.Forms.Button btnEgresoDevengar;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnCuentaAgregar;
        private System.Windows.Forms.Button btnGastoAgregar;
        private System.Windows.Forms.Button btnCuentaEliminar;
        private System.Windows.Forms.Button btnCuentaMover;
        private System.Windows.Forms.DataGridView dgvGastoDev;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContaEgresoDevengadoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private AdvancedDataGridView.TreeGridView tgvCuentas;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_ContaEgresoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Folio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Tienda;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_FormaDePago;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Usuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Observaciones;
        private Negocio.TextoMod txtBusqueda;
        private AdvancedDataGridView.TreeGridColumn Cuentas_Id;
        private AdvancedDataGridView.TreeGridColumn Cuentas_Cuenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Porcentaje;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Fiscal;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Total;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Matriz;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Sucursal2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Sucursal3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_ImporteDev;

    }
}
