namespace Refaccionaria.App
{
    partial class ConfigAfectaciones
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbOperacion = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbTipoDePoliza = new System.Windows.Forms.ComboBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.dgvAfectaciones = new Refaccionaria.Negocio.GridEditable();
            this.TipoDeCuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CuentaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CargoAbono = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AsigSucursalID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Observacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAfectaciones)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tipo de Operación";
            // 
            // cmbOperacion
            // 
            this.cmbOperacion.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbOperacion.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbOperacion.FormattingEnabled = true;
            this.cmbOperacion.Location = new System.Drawing.Point(104, 6);
            this.cmbOperacion.Name = "cmbOperacion";
            this.cmbOperacion.Size = new System.Drawing.Size(320, 21);
            this.cmbOperacion.TabIndex = 1;
            this.cmbOperacion.SelectedIndexChanged += new System.EventHandler(this.cmbOperacion_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(430, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tipo de Póliza";
            // 
            // cmbTipoDePoliza
            // 
            this.cmbTipoDePoliza.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbTipoDePoliza.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTipoDePoliza.FormattingEnabled = true;
            this.cmbTipoDePoliza.Location = new System.Drawing.Point(510, 6);
            this.cmbTipoDePoliza.Name = "cmbTipoDePoliza";
            this.cmbTipoDePoliza.Size = new System.Drawing.Size(100, 21);
            this.cmbTipoDePoliza.TabIndex = 3;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(1009, 319);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // dgvAfectaciones
            // 
            this.dgvAfectaciones.AllowUserToDeleteRows = false;
            this.dgvAfectaciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAfectaciones.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvAfectaciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAfectaciones.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvAfectaciones.ColorBorrado = System.Drawing.Color.Gray;
            this.dgvAfectaciones.ColorModificado = System.Drawing.Color.Orange;
            this.dgvAfectaciones.ColorNuevo = System.Drawing.Color.Blue;
            this.dgvAfectaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAfectaciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TipoDeCuenta,
            this.CuentaID,
            this.Cuenta,
            this.CargoAbono,
            this.AsigSucursalID,
            this.Observacion});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAfectaciones.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAfectaciones.Location = new System.Drawing.Point(3, 33);
            this.dgvAfectaciones.Name = "dgvAfectaciones";
            this.dgvAfectaciones.PermitirBorrar = true;
            this.dgvAfectaciones.Size = new System.Drawing.Size(1081, 280);
            this.dgvAfectaciones.TabIndex = 4;
            this.dgvAfectaciones.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvAfectaciones_KeyDown);
            // 
            // TipoDeCuenta
            // 
            this.TipoDeCuenta.HeaderText = "TipoDeCuenta";
            this.TipoDeCuenta.Name = "TipoDeCuenta";
            this.TipoDeCuenta.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TipoDeCuenta.Visible = false;
            this.TipoDeCuenta.Width = 80;
            // 
            // CuentaID
            // 
            this.CuentaID.HeaderText = "CuentaID";
            this.CuentaID.Name = "CuentaID";
            this.CuentaID.Visible = false;
            // 
            // Cuenta
            // 
            this.Cuenta.HeaderText = "Cuenta";
            this.Cuenta.Name = "Cuenta";
            this.Cuenta.ReadOnly = true;
            this.Cuenta.Width = 200;
            // 
            // CargoAbono
            // 
            this.CargoAbono.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.CargoAbono.HeaderText = "Cargo / Abono";
            this.CargoAbono.Name = "CargoAbono";
            // 
            // AsigSucursalID
            // 
            this.AsigSucursalID.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.AsigSucursalID.HeaderText = "Sucursal";
            this.AsigSucursalID.Name = "AsigSucursalID";
            // 
            // Observacion
            // 
            this.Observacion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Observacion.HeaderText = "Observación";
            this.Observacion.Name = "Observacion";
            this.Observacion.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Observacion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Observacion.Width = 73;
            // 
            // ConfigAfectaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvAfectaciones);
            this.Controls.Add(this.cmbTipoDePoliza);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbOperacion);
            this.Controls.Add(this.label1);
            this.Name = "ConfigAfectaciones";
            this.Size = new System.Drawing.Size(1088, 347);
            this.Load += new System.EventHandler(this.ConfigAfectaciones_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAfectaciones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbOperacion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTipoDePoliza;
        private Negocio.GridEditable dgvAfectaciones;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.DataGridViewTextBoxColumn TipoDeCuenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn CuentaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuenta;
        private System.Windows.Forms.DataGridViewComboBoxColumn CargoAbono;
        private System.Windows.Forms.DataGridViewComboBoxColumn AsigSucursalID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Observacion;
    }
}
