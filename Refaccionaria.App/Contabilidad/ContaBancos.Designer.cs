namespace Refaccionaria.App
{
    partial class ContaBancos
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabBancos = new System.Windows.Forms.TabControl();
            this.tbpAsignacion = new System.Windows.Forms.TabPage();
            this.dtpAsiHasta = new System.Windows.Forms.DateTimePicker();
            this.dtpAsiDesde = new System.Windows.Forms.DateTimePicker();
            this.chkAsiMostrarTodos = new System.Windows.Forms.CheckBox();
            this.btnAsiActualizar = new System.Windows.Forms.Button();
            this.btnAsignar = new System.Windows.Forms.Button();
            this.dgvAsignacion = new System.Windows.Forms.DataGridView();
            this.asi_BancoCuentaMovimientoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asi_Sel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.asi_Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asi_Sucursal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asi_Referencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asi_Concepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asi_DatosDePago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asi_Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbpConciliacion = new System.Windows.Forms.TabPage();
            this.btnDesagrupar = new System.Windows.Forms.Button();
            this.btnConAgregarMovimiento = new System.Windows.Forms.Button();
            this.btnConTraspaso = new System.Windows.Forms.Button();
            this.btnConDepositoEfectivo = new System.Windows.Forms.Button();
            this.btnAgrupar = new System.Windows.Forms.Button();
            this.dgvConciliacionDetalle = new System.Windows.Forms.DataGridView();
            this.cnd_BancoCuentaMovimientoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cnd_Sel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnConGuardar = new System.Windows.Forms.Button();
            this.lblImporteSeleccion = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblSaldoOperacion = new System.Windows.Forms.Label();
            this.lblSaldoConciliado = new System.Windows.Forms.Label();
            this.lblSaldoInicial = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtConBusqueda = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvConciliacion = new System.Windows.Forms.DataGridView();
            this.con_BancoCuentaMovimientoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Sel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.con_FechaAsignado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Sucursal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Concepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Referencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Depositos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Retiros = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Saldo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_FechaConciliado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_UsuarioConciliado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_Observacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.con_FueManual = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnConMostrar = new System.Windows.Forms.Button();
            this.dtpConHasta = new System.Windows.Forms.DateTimePicker();
            this.dtpConDesde = new System.Windows.Forms.DateTimePicker();
            this.cmbBancoCuenta = new System.Windows.Forms.ComboBox();
            this.lblCuenta = new System.Windows.Forms.Label();
            this.cmsConciliados = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.smiDesasignar = new System.Windows.Forms.ToolStripMenuItem();
            this.tabBancos.SuspendLayout();
            this.tbpAsignacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignacion)).BeginInit();
            this.tbpConciliacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConciliacionDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConciliacion)).BeginInit();
            this.cmsConciliados.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabBancos
            // 
            this.tabBancos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabBancos.Controls.Add(this.tbpAsignacion);
            this.tabBancos.Controls.Add(this.tbpConciliacion);
            this.tabBancos.Location = new System.Drawing.Point(3, 3);
            this.tabBancos.Name = "tabBancos";
            this.tabBancos.SelectedIndex = 0;
            this.tabBancos.Size = new System.Drawing.Size(894, 454);
            this.tabBancos.TabIndex = 0;
            this.tabBancos.SelectedIndexChanged += new System.EventHandler(this.tabBancos_SelectedIndexChanged);
            // 
            // tbpAsignacion
            // 
            this.tbpAsignacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.tbpAsignacion.Controls.Add(this.dtpAsiHasta);
            this.tbpAsignacion.Controls.Add(this.dtpAsiDesde);
            this.tbpAsignacion.Controls.Add(this.chkAsiMostrarTodos);
            this.tbpAsignacion.Controls.Add(this.btnAsiActualizar);
            this.tbpAsignacion.Controls.Add(this.btnAsignar);
            this.tbpAsignacion.Controls.Add(this.dgvAsignacion);
            this.tbpAsignacion.Location = new System.Drawing.Point(4, 22);
            this.tbpAsignacion.Name = "tbpAsignacion";
            this.tbpAsignacion.Padding = new System.Windows.Forms.Padding(3);
            this.tbpAsignacion.Size = new System.Drawing.Size(886, 428);
            this.tbpAsignacion.TabIndex = 0;
            this.tbpAsignacion.Text = "Asignación";
            // 
            // dtpAsiHasta
            // 
            this.dtpAsiHasta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpAsiHasta.Enabled = false;
            this.dtpAsiHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAsiHasta.Location = new System.Drawing.Point(171, 402);
            this.dtpAsiHasta.Name = "dtpAsiHasta";
            this.dtpAsiHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpAsiHasta.TabIndex = 3;
            this.dtpAsiHasta.ValueChanged += new System.EventHandler(this.dtpAsiHasta_ValueChanged);
            // 
            // dtpAsiDesde
            // 
            this.dtpAsiDesde.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpAsiDesde.Enabled = false;
            this.dtpAsiDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAsiDesde.Location = new System.Drawing.Point(65, 402);
            this.dtpAsiDesde.Name = "dtpAsiDesde";
            this.dtpAsiDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpAsiDesde.TabIndex = 2;
            this.dtpAsiDesde.ValueChanged += new System.EventHandler(this.dtpAsiDesde_ValueChanged);
            // 
            // chkAsiMostrarTodos
            // 
            this.chkAsiMostrarTodos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAsiMostrarTodos.AutoSize = true;
            this.chkAsiMostrarTodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAsiMostrarTodos.ForeColor = System.Drawing.Color.White;
            this.chkAsiMostrarTodos.Location = new System.Drawing.Point(3, 405);
            this.chkAsiMostrarTodos.Name = "chkAsiMostrarTodos";
            this.chkAsiMostrarTodos.Size = new System.Drawing.Size(53, 17);
            this.chkAsiMostrarTodos.TabIndex = 1;
            this.chkAsiMostrarTodos.Text = "Todos";
            this.chkAsiMostrarTodos.UseVisualStyleBackColor = true;
            this.chkAsiMostrarTodos.CheckedChanged += new System.EventHandler(this.chkAsiMostrarTodos_CheckedChanged);
            // 
            // btnAsiActualizar
            // 
            this.btnAsiActualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAsiActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAsiActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsiActualizar.ForeColor = System.Drawing.Color.White;
            this.btnAsiActualizar.Location = new System.Drawing.Point(727, 402);
            this.btnAsiActualizar.Name = "btnAsiActualizar";
            this.btnAsiActualizar.Size = new System.Drawing.Size(75, 23);
            this.btnAsiActualizar.TabIndex = 4;
            this.btnAsiActualizar.Text = "Ac&tualizar";
            this.btnAsiActualizar.UseVisualStyleBackColor = false;
            this.btnAsiActualizar.Click += new System.EventHandler(this.btnAsiActualizar_Click);
            // 
            // btnAsignar
            // 
            this.btnAsignar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAsignar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAsignar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsignar.ForeColor = System.Drawing.Color.White;
            this.btnAsignar.Location = new System.Drawing.Point(808, 402);
            this.btnAsignar.Name = "btnAsignar";
            this.btnAsignar.Size = new System.Drawing.Size(75, 23);
            this.btnAsignar.TabIndex = 5;
            this.btnAsignar.Text = "&Asignar";
            this.btnAsignar.UseVisualStyleBackColor = false;
            this.btnAsignar.Click += new System.EventHandler(this.btnAsignar_Click);
            // 
            // dgvAsignacion
            // 
            this.dgvAsignacion.AllowUserToAddRows = false;
            this.dgvAsignacion.AllowUserToDeleteRows = false;
            this.dgvAsignacion.AllowUserToResizeRows = false;
            this.dgvAsignacion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAsignacion.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvAsignacion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAsignacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAsignacion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.asi_BancoCuentaMovimientoID,
            this.asi_Sel,
            this.asi_Fecha,
            this.asi_Sucursal,
            this.asi_Referencia,
            this.asi_Concepto,
            this.asi_DatosDePago,
            this.asi_Importe});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAsignacion.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAsignacion.Location = new System.Drawing.Point(3, 3);
            this.dgvAsignacion.Name = "dgvAsignacion";
            this.dgvAsignacion.RowHeadersVisible = false;
            this.dgvAsignacion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAsignacion.Size = new System.Drawing.Size(880, 393);
            this.dgvAsignacion.TabIndex = 0;
            this.dgvAsignacion.DoubleClick += new System.EventHandler(this.dgvAsignacion_DoubleClick);
            this.dgvAsignacion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvAsignacion_KeyDown);
            // 
            // asi_BancoCuentaMovimientoID
            // 
            this.asi_BancoCuentaMovimientoID.HeaderText = "BancoCuentaMovimientoID";
            this.asi_BancoCuentaMovimientoID.Name = "asi_BancoCuentaMovimientoID";
            this.asi_BancoCuentaMovimientoID.ReadOnly = true;
            this.asi_BancoCuentaMovimientoID.Visible = false;
            // 
            // asi_Sel
            // 
            this.asi_Sel.HeaderText = "Sel";
            this.asi_Sel.Name = "asi_Sel";
            this.asi_Sel.Width = 40;
            // 
            // asi_Fecha
            // 
            this.asi_Fecha.HeaderText = "Fecha";
            this.asi_Fecha.Name = "asi_Fecha";
            this.asi_Fecha.ReadOnly = true;
            this.asi_Fecha.Width = 136;
            // 
            // asi_Sucursal
            // 
            this.asi_Sucursal.HeaderText = "Sucursal";
            this.asi_Sucursal.Name = "asi_Sucursal";
            this.asi_Sucursal.ReadOnly = true;
            // 
            // asi_Referencia
            // 
            this.asi_Referencia.HeaderText = "Folio";
            this.asi_Referencia.Name = "asi_Referencia";
            this.asi_Referencia.Width = 80;
            // 
            // asi_Concepto
            // 
            this.asi_Concepto.HeaderText = "Cliente";
            this.asi_Concepto.Name = "asi_Concepto";
            this.asi_Concepto.ReadOnly = true;
            this.asi_Concepto.Width = 200;
            // 
            // asi_DatosDePago
            // 
            this.asi_DatosDePago.HeaderText = "Forma de Pago";
            this.asi_DatosDePago.Name = "asi_DatosDePago";
            this.asi_DatosDePago.ReadOnly = true;
            this.asi_DatosDePago.Width = 200;
            // 
            // asi_Importe
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.asi_Importe.DefaultCellStyle = dataGridViewCellStyle1;
            this.asi_Importe.HeaderText = "Importe";
            this.asi_Importe.Name = "asi_Importe";
            this.asi_Importe.ReadOnly = true;
            this.asi_Importe.Width = 80;
            // 
            // tbpConciliacion
            // 
            this.tbpConciliacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.tbpConciliacion.Controls.Add(this.btnDesagrupar);
            this.tbpConciliacion.Controls.Add(this.btnConAgregarMovimiento);
            this.tbpConciliacion.Controls.Add(this.btnConTraspaso);
            this.tbpConciliacion.Controls.Add(this.btnConDepositoEfectivo);
            this.tbpConciliacion.Controls.Add(this.btnAgrupar);
            this.tbpConciliacion.Controls.Add(this.dgvConciliacionDetalle);
            this.tbpConciliacion.Controls.Add(this.btnConGuardar);
            this.tbpConciliacion.Controls.Add(this.lblImporteSeleccion);
            this.tbpConciliacion.Controls.Add(this.label6);
            this.tbpConciliacion.Controls.Add(this.lblSaldoOperacion);
            this.tbpConciliacion.Controls.Add(this.lblSaldoConciliado);
            this.tbpConciliacion.Controls.Add(this.lblSaldoInicial);
            this.tbpConciliacion.Controls.Add(this.label4);
            this.tbpConciliacion.Controls.Add(this.label3);
            this.tbpConciliacion.Controls.Add(this.label2);
            this.tbpConciliacion.Controls.Add(this.txtConBusqueda);
            this.tbpConciliacion.Controls.Add(this.label1);
            this.tbpConciliacion.Controls.Add(this.dgvConciliacion);
            this.tbpConciliacion.Controls.Add(this.btnConMostrar);
            this.tbpConciliacion.Controls.Add(this.dtpConHasta);
            this.tbpConciliacion.Controls.Add(this.dtpConDesde);
            this.tbpConciliacion.Controls.Add(this.cmbBancoCuenta);
            this.tbpConciliacion.Controls.Add(this.lblCuenta);
            this.tbpConciliacion.Location = new System.Drawing.Point(4, 22);
            this.tbpConciliacion.Name = "tbpConciliacion";
            this.tbpConciliacion.Padding = new System.Windows.Forms.Padding(3);
            this.tbpConciliacion.Size = new System.Drawing.Size(886, 428);
            this.tbpConciliacion.TabIndex = 1;
            this.tbpConciliacion.Text = "Conciliación";
            // 
            // btnDesagrupar
            // 
            this.btnDesagrupar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDesagrupar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnDesagrupar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesagrupar.ForeColor = System.Drawing.Color.White;
            this.btnDesagrupar.Location = new System.Drawing.Point(227, 402);
            this.btnDesagrupar.Name = "btnDesagrupar";
            this.btnDesagrupar.Size = new System.Drawing.Size(75, 23);
            this.btnDesagrupar.TabIndex = 8;
            this.btnDesagrupar.Text = "&Desagrupar";
            this.btnDesagrupar.UseVisualStyleBackColor = false;
            this.btnDesagrupar.Click += new System.EventHandler(this.btnDesagrupar_Click);
            // 
            // btnConAgregarMovimiento
            // 
            this.btnConAgregarMovimiento.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnConAgregarMovimiento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConAgregarMovimiento.ForeColor = System.Drawing.Color.White;
            this.btnConAgregarMovimiento.Location = new System.Drawing.Point(549, 6);
            this.btnConAgregarMovimiento.Name = "btnConAgregarMovimiento";
            this.btnConAgregarMovimiento.Size = new System.Drawing.Size(104, 23);
            this.btnConAgregarMovimiento.TabIndex = 182;
            this.btnConAgregarMovimiento.Text = "&Agregar Mov.";
            this.btnConAgregarMovimiento.UseVisualStyleBackColor = false;
            this.btnConAgregarMovimiento.Click += new System.EventHandler(this.btnConAgregarMovimiento_Click);
            // 
            // btnConTraspaso
            // 
            this.btnConTraspaso.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnConTraspaso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConTraspaso.ForeColor = System.Drawing.Color.White;
            this.btnConTraspaso.Location = new System.Drawing.Point(439, 33);
            this.btnConTraspaso.Name = "btnConTraspaso";
            this.btnConTraspaso.Size = new System.Drawing.Size(104, 23);
            this.btnConTraspaso.TabIndex = 181;
            this.btnConTraspaso.Text = "&Traspaso Cuentas";
            this.btnConTraspaso.UseVisualStyleBackColor = false;
            this.btnConTraspaso.Click += new System.EventHandler(this.btnConTraspaso_Click);
            // 
            // btnConDepositoEfectivo
            // 
            this.btnConDepositoEfectivo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnConDepositoEfectivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConDepositoEfectivo.ForeColor = System.Drawing.Color.White;
            this.btnConDepositoEfectivo.Location = new System.Drawing.Point(549, 33);
            this.btnConDepositoEfectivo.Name = "btnConDepositoEfectivo";
            this.btnConDepositoEfectivo.Size = new System.Drawing.Size(104, 23);
            this.btnConDepositoEfectivo.TabIndex = 180;
            this.btnConDepositoEfectivo.Text = "&Depósito Efectivo";
            this.btnConDepositoEfectivo.UseVisualStyleBackColor = false;
            this.btnConDepositoEfectivo.Click += new System.EventHandler(this.btnConDepositoEfectivo_Click);
            // 
            // btnAgrupar
            // 
            this.btnAgrupar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgrupar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAgrupar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgrupar.ForeColor = System.Drawing.Color.White;
            this.btnAgrupar.Location = new System.Drawing.Point(146, 402);
            this.btnAgrupar.Name = "btnAgrupar";
            this.btnAgrupar.Size = new System.Drawing.Size(75, 23);
            this.btnAgrupar.TabIndex = 7;
            this.btnAgrupar.Text = "&Agrupar";
            this.btnAgrupar.UseVisualStyleBackColor = false;
            this.btnAgrupar.Click += new System.EventHandler(this.btnAgrupar_Click);
            // 
            // dgvConciliacionDetalle
            // 
            this.dgvConciliacionDetalle.AllowUserToAddRows = false;
            this.dgvConciliacionDetalle.AllowUserToDeleteRows = false;
            this.dgvConciliacionDetalle.AllowUserToResizeRows = false;
            this.dgvConciliacionDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConciliacionDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvConciliacionDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvConciliacionDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConciliacionDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cnd_BancoCuentaMovimientoID,
            this.cnd_Sel,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7});
            this.dgvConciliacionDetalle.Location = new System.Drawing.Point(3, 276);
            this.dgvConciliacionDetalle.Name = "dgvConciliacionDetalle";
            this.dgvConciliacionDetalle.RowHeadersVisible = false;
            this.dgvConciliacionDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConciliacionDetalle.Size = new System.Drawing.Size(880, 120);
            this.dgvConciliacionDetalle.TabIndex = 6;
            this.dgvConciliacionDetalle.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvConciliacionDetalle_CurrentCellDirtyStateChanged);
            this.dgvConciliacionDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvConciliacionDetalle_KeyDown);
            // 
            // cnd_BancoCuentaMovimientoID
            // 
            this.cnd_BancoCuentaMovimientoID.HeaderText = "BancoCuentaMovimientoID";
            this.cnd_BancoCuentaMovimientoID.Name = "cnd_BancoCuentaMovimientoID";
            this.cnd_BancoCuentaMovimientoID.ReadOnly = true;
            this.cnd_BancoCuentaMovimientoID.Visible = false;
            // 
            // cnd_Sel
            // 
            this.cnd_Sel.HeaderText = "Sel";
            this.cnd_Sel.Name = "cnd_Sel";
            this.cnd_Sel.Width = 40;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Fecha asignado";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 136;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Sucursal";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Concepto";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 200;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Referencia";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 120;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "C2";
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn6.HeaderText = "Depósitos";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 80;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "C2";
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn7.HeaderText = "Retiros";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 80;
            // 
            // btnConGuardar
            // 
            this.btnConGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnConGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConGuardar.ForeColor = System.Drawing.Color.White;
            this.btnConGuardar.Location = new System.Drawing.Point(808, 402);
            this.btnConGuardar.Name = "btnConGuardar";
            this.btnConGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnConGuardar.TabIndex = 9;
            this.btnConGuardar.Text = "&Guardar";
            this.btnConGuardar.UseVisualStyleBackColor = false;
            this.btnConGuardar.Click += new System.EventHandler(this.btnConGuardar_Click);
            // 
            // lblImporteSeleccion
            // 
            this.lblImporteSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblImporteSeleccion.AutoSize = true;
            this.lblImporteSeleccion.BackColor = System.Drawing.Color.Transparent;
            this.lblImporteSeleccion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImporteSeleccion.ForeColor = System.Drawing.Color.White;
            this.lblImporteSeleccion.Location = new System.Drawing.Point(66, 407);
            this.lblImporteSeleccion.Name = "lblImporteSeleccion";
            this.lblImporteSeleccion.Size = new System.Drawing.Size(39, 13);
            this.lblImporteSeleccion.TabIndex = 9;
            this.lblImporteSeleccion.Text = "$0.00";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(6, 407);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 179;
            this.label6.Text = "Selección";
            // 
            // lblSaldoOperacion
            // 
            this.lblSaldoOperacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSaldoOperacion.BackColor = System.Drawing.Color.Transparent;
            this.lblSaldoOperacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldoOperacion.ForeColor = System.Drawing.Color.White;
            this.lblSaldoOperacion.Location = new System.Drawing.Point(782, 24);
            this.lblSaldoOperacion.Name = "lblSaldoOperacion";
            this.lblSaldoOperacion.Size = new System.Drawing.Size(100, 13);
            this.lblSaldoOperacion.TabIndex = 7;
            this.lblSaldoOperacion.Text = "$0.00";
            this.lblSaldoOperacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSaldoConciliado
            // 
            this.lblSaldoConciliado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSaldoConciliado.BackColor = System.Drawing.Color.Transparent;
            this.lblSaldoConciliado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldoConciliado.ForeColor = System.Drawing.Color.White;
            this.lblSaldoConciliado.Location = new System.Drawing.Point(782, 38);
            this.lblSaldoConciliado.Name = "lblSaldoConciliado";
            this.lblSaldoConciliado.Size = new System.Drawing.Size(100, 13);
            this.lblSaldoConciliado.TabIndex = 6;
            this.lblSaldoConciliado.Text = "$0.00";
            this.lblSaldoConciliado.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSaldoInicial
            // 
            this.lblSaldoInicial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSaldoInicial.BackColor = System.Drawing.Color.Transparent;
            this.lblSaldoInicial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldoInicial.ForeColor = System.Drawing.Color.White;
            this.lblSaldoInicial.Location = new System.Drawing.Point(782, 10);
            this.lblSaldoInicial.Name = "lblSaldoInicial";
            this.lblSaldoInicial.Size = new System.Drawing.Size(100, 13);
            this.lblSaldoInicial.TabIndex = 5;
            this.lblSaldoInicial.Text = "$0.00";
            this.lblSaldoInicial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(692, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 175;
            this.label4.Text = "Saldo operación";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(691, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 174;
            this.label3.Text = "Saldo conciliado";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(713, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 173;
            this.label2.Text = "Saldo inicial";
            // 
            // txtConBusqueda
            // 
            this.txtConBusqueda.Location = new System.Drawing.Point(67, 35);
            this.txtConBusqueda.Name = "txtConBusqueda";
            this.txtConBusqueda.Size = new System.Drawing.Size(366, 20);
            this.txtConBusqueda.TabIndex = 4;
            this.txtConBusqueda.TextChanged += new System.EventHandler(this.txtConBusqueda_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 171;
            this.label1.Text = "Búsqueda";
            // 
            // dgvConciliacion
            // 
            this.dgvConciliacion.AllowUserToAddRows = false;
            this.dgvConciliacion.AllowUserToDeleteRows = false;
            this.dgvConciliacion.AllowUserToResizeRows = false;
            this.dgvConciliacion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConciliacion.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvConciliacion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvConciliacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConciliacion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.con_BancoCuentaMovimientoID,
            this.con_Sel,
            this.con_FechaAsignado,
            this.con_Sucursal,
            this.con_Concepto,
            this.con_Referencia,
            this.con_Depositos,
            this.con_Retiros,
            this.con_Saldo,
            this.con_FechaConciliado,
            this.con_UsuarioConciliado,
            this.con_Observacion,
            this.con_FueManual});
            this.dgvConciliacion.Location = new System.Drawing.Point(3, 61);
            this.dgvConciliacion.Name = "dgvConciliacion";
            this.dgvConciliacion.RowHeadersVisible = false;
            this.dgvConciliacion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvConciliacion.Size = new System.Drawing.Size(880, 209);
            this.dgvConciliacion.TabIndex = 5;
            this.dgvConciliacion.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConciliacion_CellValueChanged);
            this.dgvConciliacion.CurrentCellChanged += new System.EventHandler(this.dgvConciliacion_CurrentCellChanged);
            this.dgvConciliacion.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvConciliacion_CurrentCellDirtyStateChanged);
            this.dgvConciliacion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvConciliacion_KeyDown);
            this.dgvConciliacion.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvConciliacion_MouseClick);
            // 
            // con_BancoCuentaMovimientoID
            // 
            this.con_BancoCuentaMovimientoID.HeaderText = "BancoCuentaMovimientoID";
            this.con_BancoCuentaMovimientoID.Name = "con_BancoCuentaMovimientoID";
            this.con_BancoCuentaMovimientoID.ReadOnly = true;
            this.con_BancoCuentaMovimientoID.Visible = false;
            // 
            // con_Sel
            // 
            this.con_Sel.HeaderText = "Sel";
            this.con_Sel.Name = "con_Sel";
            this.con_Sel.Width = 40;
            // 
            // con_FechaAsignado
            // 
            this.con_FechaAsignado.HeaderText = "Fecha asignado";
            this.con_FechaAsignado.Name = "con_FechaAsignado";
            this.con_FechaAsignado.Width = 136;
            // 
            // con_Sucursal
            // 
            this.con_Sucursal.HeaderText = "Sucursal";
            this.con_Sucursal.Name = "con_Sucursal";
            this.con_Sucursal.ReadOnly = true;
            // 
            // con_Concepto
            // 
            this.con_Concepto.HeaderText = "Concepto";
            this.con_Concepto.Name = "con_Concepto";
            this.con_Concepto.ReadOnly = true;
            this.con_Concepto.Width = 200;
            // 
            // con_Referencia
            // 
            this.con_Referencia.HeaderText = "Referencia";
            this.con_Referencia.Name = "con_Referencia";
            this.con_Referencia.ReadOnly = true;
            this.con_Referencia.Width = 120;
            // 
            // con_Depositos
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "C2";
            this.con_Depositos.DefaultCellStyle = dataGridViewCellStyle5;
            this.con_Depositos.HeaderText = "Depósitos";
            this.con_Depositos.Name = "con_Depositos";
            this.con_Depositos.ReadOnly = true;
            this.con_Depositos.Width = 80;
            // 
            // con_Retiros
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "C2";
            this.con_Retiros.DefaultCellStyle = dataGridViewCellStyle6;
            this.con_Retiros.HeaderText = "Retiros";
            this.con_Retiros.Name = "con_Retiros";
            this.con_Retiros.ReadOnly = true;
            this.con_Retiros.Width = 80;
            // 
            // con_Saldo
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "C2";
            this.con_Saldo.DefaultCellStyle = dataGridViewCellStyle7;
            this.con_Saldo.HeaderText = "Saldo";
            this.con_Saldo.Name = "con_Saldo";
            this.con_Saldo.ReadOnly = true;
            this.con_Saldo.Width = 80;
            // 
            // con_FechaConciliado
            // 
            this.con_FechaConciliado.HeaderText = "Fecha con.";
            this.con_FechaConciliado.Name = "con_FechaConciliado";
            this.con_FechaConciliado.ReadOnly = true;
            this.con_FechaConciliado.Width = 136;
            // 
            // con_UsuarioConciliado
            // 
            this.con_UsuarioConciliado.HeaderText = "Usuario con.";
            this.con_UsuarioConciliado.Name = "con_UsuarioConciliado";
            this.con_UsuarioConciliado.ReadOnly = true;
            // 
            // con_Observacion
            // 
            this.con_Observacion.HeaderText = "Observación";
            this.con_Observacion.Name = "con_Observacion";
            this.con_Observacion.Width = 160;
            // 
            // con_FueManual
            // 
            this.con_FueManual.HeaderText = "FueManual";
            this.con_FueManual.Name = "con_FueManual";
            this.con_FueManual.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.con_FueManual.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.con_FueManual.Visible = false;
            // 
            // btnConMostrar
            // 
            this.btnConMostrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnConMostrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConMostrar.ForeColor = System.Drawing.Color.White;
            this.btnConMostrar.Location = new System.Drawing.Point(439, 6);
            this.btnConMostrar.Name = "btnConMostrar";
            this.btnConMostrar.Size = new System.Drawing.Size(75, 23);
            this.btnConMostrar.TabIndex = 3;
            this.btnConMostrar.Text = "&Mostrar";
            this.btnConMostrar.UseVisualStyleBackColor = false;
            this.btnConMostrar.Visible = false;
            this.btnConMostrar.Click += new System.EventHandler(this.btnConMostrar_Click);
            // 
            // dtpConHasta
            // 
            this.dtpConHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpConHasta.Location = new System.Drawing.Point(333, 9);
            this.dtpConHasta.Name = "dtpConHasta";
            this.dtpConHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpConHasta.TabIndex = 2;
            this.dtpConHasta.ValueChanged += new System.EventHandler(this.dtpConHasta_ValueChanged);
            // 
            // dtpConDesde
            // 
            this.dtpConDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpConDesde.Location = new System.Drawing.Point(227, 9);
            this.dtpConDesde.Name = "dtpConDesde";
            this.dtpConDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpConDesde.TabIndex = 1;
            this.dtpConDesde.ValueChanged += new System.EventHandler(this.dtpConDesde_ValueChanged);
            // 
            // cmbBancoCuenta
            // 
            this.cmbBancoCuenta.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbBancoCuenta.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbBancoCuenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBancoCuenta.FormattingEnabled = true;
            this.cmbBancoCuenta.Location = new System.Drawing.Point(67, 8);
            this.cmbBancoCuenta.Name = "cmbBancoCuenta";
            this.cmbBancoCuenta.Size = new System.Drawing.Size(154, 21);
            this.cmbBancoCuenta.TabIndex = 0;
            this.cmbBancoCuenta.SelectedIndexChanged += new System.EventHandler(this.cmbBancoCuenta_SelectedIndexChanged);
            // 
            // lblCuenta
            // 
            this.lblCuenta.AutoSize = true;
            this.lblCuenta.BackColor = System.Drawing.Color.Transparent;
            this.lblCuenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCuenta.ForeColor = System.Drawing.Color.White;
            this.lblCuenta.Location = new System.Drawing.Point(6, 11);
            this.lblCuenta.Name = "lblCuenta";
            this.lblCuenta.Size = new System.Drawing.Size(41, 13);
            this.lblCuenta.TabIndex = 166;
            this.lblCuenta.Text = "Cuenta";
            // 
            // cmsConciliados
            // 
            this.cmsConciliados.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smiDesasignar});
            this.cmsConciliados.Name = "cmsConciliados";
            this.cmsConciliados.Size = new System.Drawing.Size(132, 26);
            // 
            // smiDesasignar
            // 
            this.smiDesasignar.Name = "smiDesasignar";
            this.smiDesasignar.Size = new System.Drawing.Size(131, 22);
            this.smiDesasignar.Text = "&Desasignar";
            this.smiDesasignar.Click += new System.EventHandler(this.smiDesasignar_Click);
            // 
            // ContaBancos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.tabBancos);
            this.Name = "ContaBancos";
            this.Size = new System.Drawing.Size(900, 460);
            this.Load += new System.EventHandler(this.ContaBancos_Load);
            this.tabBancos.ResumeLayout(false);
            this.tbpAsignacion.ResumeLayout(false);
            this.tbpAsignacion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignacion)).EndInit();
            this.tbpConciliacion.ResumeLayout(false);
            this.tbpConciliacion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConciliacionDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConciliacion)).EndInit();
            this.cmsConciliados.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabBancos;
        private System.Windows.Forms.TabPage tbpAsignacion;
        private System.Windows.Forms.TabPage tbpConciliacion;
        private System.Windows.Forms.DataGridView dgvAsignacion;
        private System.Windows.Forms.Button btnAsignar;
        private System.Windows.Forms.Button btnAsiActualizar;
        private System.Windows.Forms.DateTimePicker dtpConHasta;
        private System.Windows.Forms.DateTimePicker dtpConDesde;
        private System.Windows.Forms.ComboBox cmbBancoCuenta;
        private System.Windows.Forms.Label lblCuenta;
        private System.Windows.Forms.DataGridView dgvConciliacion;
        private System.Windows.Forms.Button btnConMostrar;
        private System.Windows.Forms.DateTimePicker dtpAsiHasta;
        private System.Windows.Forms.DateTimePicker dtpAsiDesde;
        private System.Windows.Forms.CheckBox chkAsiMostrarTodos;
        private System.Windows.Forms.Button btnConGuardar;
        private System.Windows.Forms.Label lblImporteSeleccion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblSaldoOperacion;
        private System.Windows.Forms.Label lblSaldoConciliado;
        private System.Windows.Forms.Label lblSaldoInicial;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtConBusqueda;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn asi_BancoCuentaMovimientoID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn asi_Sel;
        private System.Windows.Forms.DataGridViewTextBoxColumn asi_Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn asi_Sucursal;
        private System.Windows.Forms.DataGridViewTextBoxColumn asi_Referencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn asi_Concepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn asi_DatosDePago;
        private System.Windows.Forms.DataGridViewTextBoxColumn asi_Importe;
        private System.Windows.Forms.Button btnAgrupar;
        private System.Windows.Forms.DataGridView dgvConciliacionDetalle;
        private System.Windows.Forms.Button btnConDepositoEfectivo;
        private System.Windows.Forms.Button btnConTraspaso;
        private System.Windows.Forms.Button btnConAgregarMovimiento;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_BancoCuentaMovimientoID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn con_Sel;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_FechaAsignado;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_Sucursal;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_Concepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_Referencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_Depositos;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_Retiros;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_Saldo;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_FechaConciliado;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_UsuarioConciliado;
        private System.Windows.Forms.DataGridViewTextBoxColumn con_Observacion;
        private System.Windows.Forms.DataGridViewCheckBoxColumn con_FueManual;
        private System.Windows.Forms.DataGridViewTextBoxColumn cnd_BancoCuentaMovimientoID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cnd_Sel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.Button btnDesagrupar;
        private System.Windows.Forms.ContextMenuStrip cmsConciliados;
        private System.Windows.Forms.ToolStripMenuItem smiDesasignar;
    }
}
