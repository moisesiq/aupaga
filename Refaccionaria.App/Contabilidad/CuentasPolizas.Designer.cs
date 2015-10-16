namespace Refaccionaria.App
{
    partial class CuentasPolizas
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnPolizaAgregar = new System.Windows.Forms.Button();
            this.tgvCuentas = new AdvancedDataGridView.TreeGridView();
            this.Cuentas_Id = new AdvancedDataGridView.TreeGridColumn();
            this.Cuentas_Cuenta = new AdvancedDataGridView.TreeGridColumn();
            this.Cuentas_Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Matriz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Sucursal2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuentas_Sucursal3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvPoliza = new System.Windows.Forms.DataGridView();
            this.pol_PolizaDetalleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pol_Cuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pol_Cargo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pol_Abono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pol_Referencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCuentaMover = new System.Windows.Forms.Button();
            this.btnCuentaEliminar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.det_ContaPolizaDetalleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.det_Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.det_ContaPolizaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.det_Referencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.det_Cargo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.det_Abono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.det_Sucursal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.det_Observacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnCuentaEditar = new System.Windows.Forms.Button();
            this.btnCuentaAgregar = new System.Windows.Forms.Button();
            this.txtConceptoPoliza = new System.Windows.Forms.TextBox();
            this.lblOrigenPoliza = new System.Windows.Forms.Label();
            this.btnRecibirResguardo = new System.Windows.Forms.Button();
            this.btnPolizaCambiarSucursal = new System.Windows.Forms.Button();
            this.txtBusquedaPolizaDet = new Refaccionaria.Negocio.TextoMod();
            this.txtBusqueda = new Refaccionaria.Negocio.TextoMod();
            this.cmbSucursal = new Refaccionaria.Negocio.ComboEtiqueta();
            ((System.ComponentModel.ISupportInitialize)(this.tgvCuentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoliza)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPolizaAgregar
            // 
            this.btnPolizaAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPolizaAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnPolizaAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPolizaAgregar.ForeColor = System.Drawing.Color.White;
            this.btnPolizaAgregar.Location = new System.Drawing.Point(1302, 4);
            this.btnPolizaAgregar.Name = "btnPolizaAgregar";
            this.btnPolizaAgregar.Size = new System.Drawing.Size(55, 23);
            this.btnPolizaAgregar.TabIndex = 10;
            this.btnPolizaAgregar.Text = "+ &Póliza";
            this.btnPolizaAgregar.UseVisualStyleBackColor = false;
            this.btnPolizaAgregar.Click += new System.EventHandler(this.btnPolizaAgregar_Click);
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
            this.Cuentas_Total,
            this.Cuentas_Matriz,
            this.Cuentas_Sucursal2,
            this.Cuentas_Sucursal3});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tgvCuentas.DefaultCellStyle = dataGridViewCellStyle5;
            this.tgvCuentas.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tgvCuentas.ImageList = null;
            this.tgvCuentas.Location = new System.Drawing.Point(3, 30);
            this.tgvCuentas.Name = "tgvCuentas";
            this.tgvCuentas.ReadOnly = true;
            this.tgvCuentas.RowHeadersVisible = false;
            this.tgvCuentas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tgvCuentas.Size = new System.Drawing.Size(720, 430);
            this.tgvCuentas.TabIndex = 1;
            this.tgvCuentas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tgvCuentas_CellClick);
            this.tgvCuentas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tgvCuentas_CellDoubleClick);
            this.tgvCuentas.CurrentCellChanged += new System.EventHandler(this.tgvCuentas_CurrentCellChanged);
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
            this.Cuentas_Cuenta.Width = 300;
            // 
            // Cuentas_Total
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Cuentas_Total.DefaultCellStyle = dataGridViewCellStyle1;
            this.Cuentas_Total.HeaderText = "Total";
            this.Cuentas_Total.Name = "Cuentas_Total";
            this.Cuentas_Total.ReadOnly = true;
            this.Cuentas_Total.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cuentas_Matriz
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            this.Cuentas_Matriz.DefaultCellStyle = dataGridViewCellStyle2;
            this.Cuentas_Matriz.HeaderText = "Matriz";
            this.Cuentas_Matriz.Name = "Cuentas_Matriz";
            this.Cuentas_Matriz.ReadOnly = true;
            this.Cuentas_Matriz.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cuentas_Sucursal2
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "C2";
            this.Cuentas_Sucursal2.DefaultCellStyle = dataGridViewCellStyle3;
            this.Cuentas_Sucursal2.HeaderText = "Suc 02";
            this.Cuentas_Sucursal2.Name = "Cuentas_Sucursal2";
            this.Cuentas_Sucursal2.ReadOnly = true;
            this.Cuentas_Sucursal2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Cuentas_Sucursal3
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "C2";
            this.Cuentas_Sucursal3.DefaultCellStyle = dataGridViewCellStyle4;
            this.Cuentas_Sucursal3.HeaderText = "Suc 03";
            this.Cuentas_Sucursal3.Name = "Cuentas_Sucursal3";
            this.Cuentas_Sucursal3.ReadOnly = true;
            this.Cuentas_Sucursal3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvPoliza
            // 
            this.dgvPoliza.AllowUserToAddRows = false;
            this.dgvPoliza.AllowUserToDeleteRows = false;
            this.dgvPoliza.AllowUserToResizeRows = false;
            this.dgvPoliza.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPoliza.BackgroundColor = System.Drawing.Color.White;
            this.dgvPoliza.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPoliza.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvPoliza.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvPoliza.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPoliza.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pol_PolizaDetalleID,
            this.pol_Cuenta,
            this.pol_Cargo,
            this.pol_Abono,
            this.pol_Referencia});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPoliza.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvPoliza.GridColor = System.Drawing.Color.White;
            this.dgvPoliza.Location = new System.Drawing.Point(729, 248);
            this.dgvPoliza.Name = "dgvPoliza";
            this.dgvPoliza.ReadOnly = true;
            this.dgvPoliza.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPoliza.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvPoliza.RowHeadersVisible = false;
            this.dgvPoliza.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPoliza.Size = new System.Drawing.Size(628, 177);
            this.dgvPoliza.StandardTab = true;
            this.dgvPoliza.TabIndex = 15;
            // 
            // pol_PolizaDetalleID
            // 
            this.pol_PolizaDetalleID.HeaderText = "PolizaID";
            this.pol_PolizaDetalleID.Name = "pol_PolizaDetalleID";
            this.pol_PolizaDetalleID.ReadOnly = true;
            this.pol_PolizaDetalleID.Visible = false;
            // 
            // pol_Cuenta
            // 
            this.pol_Cuenta.HeaderText = "Cuenta";
            this.pol_Cuenta.Name = "pol_Cuenta";
            this.pol_Cuenta.ReadOnly = true;
            this.pol_Cuenta.Width = 120;
            // 
            // pol_Cargo
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "C2";
            this.pol_Cargo.DefaultCellStyle = dataGridViewCellStyle6;
            this.pol_Cargo.HeaderText = "Cargo";
            this.pol_Cargo.Name = "pol_Cargo";
            this.pol_Cargo.ReadOnly = true;
            this.pol_Cargo.Width = 80;
            // 
            // pol_Abono
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "C2";
            this.pol_Abono.DefaultCellStyle = dataGridViewCellStyle7;
            this.pol_Abono.HeaderText = "Abono";
            this.pol_Abono.Name = "pol_Abono";
            this.pol_Abono.ReadOnly = true;
            this.pol_Abono.Width = 80;
            // 
            // pol_Referencia
            // 
            this.pol_Referencia.HeaderText = "Referencia";
            this.pol_Referencia.Name = "pol_Referencia";
            this.pol_Referencia.ReadOnly = true;
            this.pol_Referencia.Width = 200;
            // 
            // btnCuentaMover
            // 
            this.btnCuentaMover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaMover.Enabled = false;
            this.btnCuentaMover.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaMover.ForeColor = System.Drawing.Color.White;
            this.btnCuentaMover.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaMover.Location = new System.Drawing.Point(986, 4);
            this.btnCuentaMover.Name = "btnCuentaMover";
            this.btnCuentaMover.Size = new System.Drawing.Size(55, 22);
            this.btnCuentaMover.TabIndex = 4;
            this.btnCuentaMover.Text = "Cambiar";
            this.btnCuentaMover.UseVisualStyleBackColor = false;
            this.btnCuentaMover.Click += new System.EventHandler(this.btnCuentaMover_Click);
            // 
            // btnCuentaEliminar
            // 
            this.btnCuentaEliminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaEliminar.Enabled = false;
            this.btnCuentaEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaEliminar.ForeColor = System.Drawing.Color.White;
            this.btnCuentaEliminar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaEliminar.Location = new System.Drawing.Point(1107, 4);
            this.btnCuentaEliminar.Name = "btnCuentaEliminar";
            this.btnCuentaEliminar.Size = new System.Drawing.Size(16, 22);
            this.btnCuentaEliminar.TabIndex = 7;
            this.btnCuentaEliminar.Text = "-";
            this.btnCuentaEliminar.UseVisualStyleBackColor = false;
            this.btnCuentaEliminar.Click += new System.EventHandler(this.btnCuentaEliminar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(726, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Fechas";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.AllowUserToResizeRows = false;
            this.dgvDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDetalle.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.det_ContaPolizaDetalleID,
            this.det_Fecha,
            this.det_ContaPolizaID,
            this.det_Referencia,
            this.det_Cargo,
            this.det_Abono,
            this.det_Sucursal,
            this.det_Observacion});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDetalle.DefaultCellStyle = dataGridViewCellStyle12;
            this.dgvDetalle.GridColor = System.Drawing.Color.White;
            this.dgvDetalle.Location = new System.Drawing.Point(729, 59);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetalle.RowHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(628, 166);
            this.dgvDetalle.StandardTab = true;
            this.dgvDetalle.TabIndex = 13;
            this.dgvDetalle.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetalle_CellDoubleClick);
            this.dgvDetalle.CurrentCellChanged += new System.EventHandler(this.dgvDetalle_CurrentCellChanged);
            this.dgvDetalle.Sorted += new System.EventHandler(this.dgvDetalle_Sorted);
            this.dgvDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDetalle_KeyDown);
            // 
            // det_ContaPolizaDetalleID
            // 
            this.det_ContaPolizaDetalleID.HeaderText = "PolizaDetalleID";
            this.det_ContaPolizaDetalleID.Name = "det_ContaPolizaDetalleID";
            this.det_ContaPolizaDetalleID.ReadOnly = true;
            this.det_ContaPolizaDetalleID.Visible = false;
            // 
            // det_Fecha
            // 
            this.det_Fecha.HeaderText = "Fecha";
            this.det_Fecha.Name = "det_Fecha";
            this.det_Fecha.ReadOnly = true;
            this.det_Fecha.Width = 136;
            // 
            // det_ContaPolizaID
            // 
            this.det_ContaPolizaID.DataPropertyName = "Fecha";
            this.det_ContaPolizaID.HeaderText = "Póliza";
            this.det_ContaPolizaID.Name = "det_ContaPolizaID";
            this.det_ContaPolizaID.ReadOnly = true;
            this.det_ContaPolizaID.Width = 50;
            // 
            // det_Referencia
            // 
            this.det_Referencia.HeaderText = "Referencia";
            this.det_Referencia.Name = "det_Referencia";
            this.det_Referencia.ReadOnly = true;
            // 
            // det_Cargo
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.Format = "C2";
            this.det_Cargo.DefaultCellStyle = dataGridViewCellStyle10;
            this.det_Cargo.HeaderText = "Cargo";
            this.det_Cargo.Name = "det_Cargo";
            this.det_Cargo.ReadOnly = true;
            this.det_Cargo.Width = 80;
            // 
            // det_Abono
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle11.Format = "C2";
            this.det_Abono.DefaultCellStyle = dataGridViewCellStyle11;
            this.det_Abono.HeaderText = "Abono";
            this.det_Abono.Name = "det_Abono";
            this.det_Abono.ReadOnly = true;
            this.det_Abono.Width = 80;
            // 
            // det_Sucursal
            // 
            this.det_Sucursal.HeaderText = "Sucursal";
            this.det_Sucursal.Name = "det_Sucursal";
            this.det_Sucursal.ReadOnly = true;
            this.det_Sucursal.Width = 80;
            // 
            // det_Observacion
            // 
            this.det_Observacion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.det_Observacion.HeaderText = "Observación";
            this.det_Observacion.Name = "det_Observacion";
            this.det_Observacion.ReadOnly = true;
            this.det_Observacion.Width = 92;
            // 
            // dtpDesde
            // 
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(774, 5);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde.TabIndex = 2;
            this.dtpDesde.ValueChanged += new System.EventHandler(this.dtpDesde_ValueChanged);
            // 
            // dtpHasta
            // 
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(880, 5);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta.TabIndex = 3;
            this.dtpHasta.ValueChanged += new System.EventHandler(this.dtpHasta_ValueChanged);
            // 
            // btnCuentaEditar
            // 
            this.btnCuentaEditar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaEditar.ForeColor = System.Drawing.Color.White;
            this.btnCuentaEditar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaEditar.Location = new System.Drawing.Point(1061, 4);
            this.btnCuentaEditar.Name = "btnCuentaEditar";
            this.btnCuentaEditar.Size = new System.Drawing.Size(44, 22);
            this.btnCuentaEditar.TabIndex = 6;
            this.btnCuentaEditar.Text = "Editar";
            this.btnCuentaEditar.UseVisualStyleBackColor = false;
            this.btnCuentaEditar.Click += new System.EventHandler(this.btnCuentaEditar_Click);
            // 
            // btnCuentaAgregar
            // 
            this.btnCuentaAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCuentaAgregar.Enabled = false;
            this.btnCuentaAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCuentaAgregar.ForeColor = System.Drawing.Color.White;
            this.btnCuentaAgregar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCuentaAgregar.Location = new System.Drawing.Point(1043, 4);
            this.btnCuentaAgregar.Name = "btnCuentaAgregar";
            this.btnCuentaAgregar.Size = new System.Drawing.Size(16, 22);
            this.btnCuentaAgregar.TabIndex = 5;
            this.btnCuentaAgregar.Text = "+";
            this.btnCuentaAgregar.UseVisualStyleBackColor = false;
            this.btnCuentaAgregar.Click += new System.EventHandler(this.btnCuentaAgregar_Click);
            // 
            // txtConceptoPoliza
            // 
            this.txtConceptoPoliza.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConceptoPoliza.Location = new System.Drawing.Point(729, 431);
            this.txtConceptoPoliza.Multiline = true;
            this.txtConceptoPoliza.Name = "txtConceptoPoliza";
            this.txtConceptoPoliza.ReadOnly = true;
            this.txtConceptoPoliza.Size = new System.Drawing.Size(628, 29);
            this.txtConceptoPoliza.TabIndex = 16;
            // 
            // lblOrigenPoliza
            // 
            this.lblOrigenPoliza.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOrigenPoliza.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrigenPoliza.ForeColor = System.Drawing.Color.White;
            this.lblOrigenPoliza.Location = new System.Drawing.Point(732, 228);
            this.lblOrigenPoliza.Name = "lblOrigenPoliza";
            this.lblOrigenPoliza.Size = new System.Drawing.Size(544, 17);
            this.lblOrigenPoliza.TabIndex = 14;
            this.lblOrigenPoliza.Text = "d";
            this.lblOrigenPoliza.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnRecibirResguardo
            // 
            this.btnRecibirResguardo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnRecibirResguardo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecibirResguardo.ForeColor = System.Drawing.Color.White;
            this.btnRecibirResguardo.Location = new System.Drawing.Point(1125, 4);
            this.btnRecibirResguardo.Name = "btnRecibirResguardo";
            this.btnRecibirResguardo.Size = new System.Drawing.Size(74, 22);
            this.btnRecibirResguardo.TabIndex = 8;
            this.btnRecibirResguardo.Text = "Resguardo";
            this.btnRecibirResguardo.UseVisualStyleBackColor = false;
            this.btnRecibirResguardo.Click += new System.EventHandler(this.btnRecibirResguardo_Click);
            // 
            // btnPolizaCambiarSucursal
            // 
            this.btnPolizaCambiarSucursal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnPolizaCambiarSucursal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPolizaCambiarSucursal.ForeColor = System.Drawing.Color.White;
            this.btnPolizaCambiarSucursal.Location = new System.Drawing.Point(1201, 4);
            this.btnPolizaCambiarSucursal.Name = "btnPolizaCambiarSucursal";
            this.btnPolizaCambiarSucursal.Size = new System.Drawing.Size(67, 22);
            this.btnPolizaCambiarSucursal.TabIndex = 9;
            this.btnPolizaCambiarSucursal.Text = "/ Sucursal";
            this.btnPolizaCambiarSucursal.UseVisualStyleBackColor = false;
            this.btnPolizaCambiarSucursal.Click += new System.EventHandler(this.btnPolizaCambiarSucursal_Click);
            // 
            // txtBusquedaPolizaDet
            // 
            this.txtBusquedaPolizaDet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBusquedaPolizaDet.Etiqueta = "Filtro";
            this.txtBusquedaPolizaDet.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtBusquedaPolizaDet.Location = new System.Drawing.Point(835, 33);
            this.txtBusquedaPolizaDet.Name = "txtBusquedaPolizaDet";
            this.txtBusquedaPolizaDet.PasarEnfoqueConEnter = false;
            this.txtBusquedaPolizaDet.SeleccionarTextoAlEnfoque = true;
            this.txtBusquedaPolizaDet.Size = new System.Drawing.Size(522, 20);
            this.txtBusquedaPolizaDet.TabIndex = 12;
            this.txtBusquedaPolizaDet.TextChanged += new System.EventHandler(this.txtBusquedaPolizaDet_TextChanged);
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Etiqueta = "Búsqueda";
            this.txtBusqueda.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtBusqueda.Location = new System.Drawing.Point(3, 6);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.PasarEnfoqueConEnter = false;
            this.txtBusqueda.SeleccionarTextoAlEnfoque = true;
            this.txtBusqueda.Size = new System.Drawing.Size(720, 20);
            this.txtBusqueda.TabIndex = 0;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBusqueda_KeyDown);
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
            this.cmbSucursal.Location = new System.Drawing.Point(729, 33);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.SelectedIndex = -1;
            this.cmbSucursal.SelectedItem = null;
            this.cmbSucursal.SelectedText = "";
            this.cmbSucursal.SelectedValue = null;
            this.cmbSucursal.Size = new System.Drawing.Size(100, 21);
            this.cmbSucursal.TabIndex = 11;
            this.cmbSucursal.ValueMember = "";
            this.cmbSucursal.SelectedIndexChanged += new System.EventHandler(this.cmbSucursal_SelectedIndexChanged);
            this.cmbSucursal.TextChanged += new System.EventHandler(this.cmbSucursal_TextChanged);
            // 
            // CuentasPolizas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.txtBusquedaPolizaDet);
            this.Controls.Add(this.btnPolizaCambiarSucursal);
            this.Controls.Add(this.btnRecibirResguardo);
            this.Controls.Add(this.txtConceptoPoliza);
            this.Controls.Add(this.txtBusqueda);
            this.Controls.Add(this.btnPolizaAgregar);
            this.Controls.Add(this.tgvCuentas);
            this.Controls.Add(this.dgvPoliza);
            this.Controls.Add(this.btnCuentaMover);
            this.Controls.Add(this.btnCuentaEliminar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.btnCuentaEditar);
            this.Controls.Add(this.btnCuentaAgregar);
            this.Controls.Add(this.lblOrigenPoliza);
            this.Name = "CuentasPolizas";
            this.Size = new System.Drawing.Size(1360, 463);
            this.Load += new System.EventHandler(this.CuentasPolizas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tgvCuentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoliza)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private System.Windows.Forms.Button btnPolizaAgregar;
        private AdvancedDataGridView.TreeGridView tgvCuentas;
        private System.Windows.Forms.DataGridView dgvPoliza;
        private System.Windows.Forms.Button btnCuentaMover;
        private System.Windows.Forms.Button btnCuentaEliminar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnCuentaEditar;
        private System.Windows.Forms.Button btnCuentaAgregar;
        private AdvancedDataGridView.TreeGridColumn Cuentas_Id;
        private AdvancedDataGridView.TreeGridColumn Cuentas_Cuenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Total;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Matriz;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Sucursal2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuentas_Sucursal3;
        private Negocio.TextoMod txtBusqueda;
        private System.Windows.Forms.TextBox txtConceptoPoliza;
        private System.Windows.Forms.DataGridViewTextBoxColumn pol_PolizaDetalleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn pol_Cuenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn pol_Cargo;
        private System.Windows.Forms.DataGridViewTextBoxColumn pol_Abono;
        private System.Windows.Forms.DataGridViewTextBoxColumn pol_Referencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_ContaPolizaDetalleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_ContaPolizaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_Referencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_Cargo;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_Abono;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_Sucursal;
        private System.Windows.Forms.DataGridViewTextBoxColumn det_Observacion;
        private System.Windows.Forms.Label lblOrigenPoliza;
        private System.Windows.Forms.Button btnRecibirResguardo;
        private System.Windows.Forms.Button btnPolizaCambiarSucursal;
        private Negocio.TextoMod txtBusquedaPolizaDet;
        private Negocio.ComboEtiqueta cmbSucursal;
    }
}
