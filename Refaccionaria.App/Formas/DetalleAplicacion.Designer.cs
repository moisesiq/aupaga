namespace Refaccionaria.App
{
    partial class DetalleAplicacion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetalleAplicacion));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblFuente = new System.Windows.Forms.Label();
            this.cboTipoFuente = new System.Windows.Forms.ComboBox();
            this.cboMarca = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gpoModelos = new System.Windows.Forms.GroupBox();
            this.lblActuales = new System.Windows.Forms.Label();
            this.lblNombreMotor = new System.Windows.Forms.Label();
            this.dgvAplicaciones = new System.Windows.Forms.DataGridView();
            this.cboMotor = new System.Windows.Forms.ComboBox();
            this.lblSeleccion = new System.Windows.Forms.Label();
            this.btnNingunoAnios = new System.Windows.Forms.Button();
            this.btnTodosAnios = new System.Windows.Forms.Button();
            this.clbAnios = new System.Windows.Forms.CheckedListBox();
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.lblMostrar = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.dgvSeleccion = new System.Windows.Forms.DataGridView();
            this.dgvModelos = new System.Windows.Forms.DataGridView();
            this.gpoGen.SuspendLayout();
            this.gpoModelos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAplicaciones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeleccion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvModelos)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.gpoModelos);
            this.gpoGen.Controls.Add(this.cboMarca);
            this.gpoGen.Controls.Add(this.label1);
            this.gpoGen.Controls.Add(this.cboTipoFuente);
            this.gpoGen.Controls.Add(this.lblFuente);
            this.gpoGen.Size = new System.Drawing.Size(650, 598);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(580, 616);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(492, 616);
            this.btnGuardar.TabIndex = 1;
            // 
            // lblFuente
            // 
            this.lblFuente.AutoSize = true;
            this.lblFuente.ForeColor = System.Drawing.Color.White;
            this.lblFuente.Location = new System.Drawing.Point(7, 20);
            this.lblFuente.Name = "lblFuente";
            this.lblFuente.Size = new System.Drawing.Size(40, 13);
            this.lblFuente.TabIndex = 0;
            this.lblFuente.Text = "Fuente";
            // 
            // cboTipoFuente
            // 
            this.cboTipoFuente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboTipoFuente.FormattingEnabled = true;
            this.cboTipoFuente.Location = new System.Drawing.Point(53, 17);
            this.cboTipoFuente.Name = "cboTipoFuente";
            this.cboTipoFuente.Size = new System.Drawing.Size(250, 21);
            this.cboTipoFuente.TabIndex = 0;
            // 
            // cboMarca
            // 
            this.cboMarca.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMarca.FormattingEnabled = true;
            this.cboMarca.Location = new System.Drawing.Point(388, 17);
            this.cboMarca.Name = "cboMarca";
            this.cboMarca.Size = new System.Drawing.Size(250, 21);
            this.cboMarca.TabIndex = 1;
            this.cboMarca.SelectedValueChanged += new System.EventHandler(this.cboMarca_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(342, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Marca";
            // 
            // gpoModelos
            // 
            this.gpoModelos.Controls.Add(this.lblActuales);
            this.gpoModelos.Controls.Add(this.lblNombreMotor);
            this.gpoModelos.Controls.Add(this.dgvAplicaciones);
            this.gpoModelos.Controls.Add(this.cboMotor);
            this.gpoModelos.Controls.Add(this.lblSeleccion);
            this.gpoModelos.Controls.Add(this.btnNingunoAnios);
            this.gpoModelos.Controls.Add(this.btnTodosAnios);
            this.gpoModelos.Controls.Add(this.clbAnios);
            this.gpoModelos.Controls.Add(this.txtBusqueda);
            this.gpoModelos.Controls.Add(this.lblMostrar);
            this.gpoModelos.Controls.Add(this.btnAgregar);
            this.gpoModelos.Controls.Add(this.dgvSeleccion);
            this.gpoModelos.Controls.Add(this.dgvModelos);
            this.gpoModelos.ForeColor = System.Drawing.Color.White;
            this.gpoModelos.Location = new System.Drawing.Point(10, 41);
            this.gpoModelos.Name = "gpoModelos";
            this.gpoModelos.Size = new System.Drawing.Size(628, 551);
            this.gpoModelos.TabIndex = 6;
            this.gpoModelos.TabStop = false;
            this.gpoModelos.Text = "Modelos de la marca seleccionada";
            // 
            // lblActuales
            // 
            this.lblActuales.AutoSize = true;
            this.lblActuales.ForeColor = System.Drawing.Color.White;
            this.lblActuales.Location = new System.Drawing.Point(6, 387);
            this.lblActuales.Name = "lblActuales";
            this.lblActuales.Size = new System.Drawing.Size(110, 13);
            this.lblActuales.TabIndex = 19;
            this.lblActuales.Text = "Aplicaciones actuales";
            // 
            // lblNombreMotor
            // 
            this.lblNombreMotor.AutoSize = true;
            this.lblNombreMotor.Location = new System.Drawing.Point(434, 24);
            this.lblNombreMotor.Name = "lblNombreMotor";
            this.lblNombreMotor.Size = new System.Drawing.Size(74, 13);
            this.lblNombreMotor.TabIndex = 21;
            this.lblNombreMotor.Text = "Nombre Motor";
            // 
            // dgvAplicaciones
            // 
            this.dgvAplicaciones.AllowUserToAddRows = false;
            this.dgvAplicaciones.AllowUserToDeleteRows = false;
            this.dgvAplicaciones.AllowUserToResizeRows = false;
            this.dgvAplicaciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAplicaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvAplicaciones.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvAplicaciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAplicaciones.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvAplicaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAplicaciones.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAplicaciones.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvAplicaciones.Location = new System.Drawing.Point(6, 403);
            this.dgvAplicaciones.Name = "dgvAplicaciones";
            this.dgvAplicaciones.ReadOnly = true;
            this.dgvAplicaciones.RowHeadersVisible = false;
            this.dgvAplicaciones.RowHeadersWidth = 25;
            this.dgvAplicaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAplicaciones.Size = new System.Drawing.Size(614, 142);
            this.dgvAplicaciones.TabIndex = 18;
            // 
            // cboMotor
            // 
            this.cboMotor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMotor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMotor.FormattingEnabled = true;
            this.cboMotor.Location = new System.Drawing.Point(514, 21);
            this.cboMotor.Name = "cboMotor";
            this.cboMotor.Size = new System.Drawing.Size(106, 21);
            this.cboMotor.TabIndex = 20;
            this.cboMotor.SelectedValueChanged += new System.EventHandler(this.cboMotor_SelectedValueChanged);
            // 
            // lblSeleccion
            // 
            this.lblSeleccion.AutoSize = true;
            this.lblSeleccion.ForeColor = System.Drawing.Color.White;
            this.lblSeleccion.Location = new System.Drawing.Point(6, 225);
            this.lblSeleccion.Name = "lblSeleccion";
            this.lblSeleccion.Size = new System.Drawing.Size(138, 13);
            this.lblSeleccion.TabIndex = 17;
            this.lblSeleccion.Text = "Aplicaciones seleccionadas";
            // 
            // btnNingunoAnios
            // 
            this.btnNingunoAnios.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnNingunoAnios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNingunoAnios.ForeColor = System.Drawing.Color.White;
            this.btnNingunoAnios.Location = new System.Drawing.Point(563, 48);
            this.btnNingunoAnios.Name = "btnNingunoAnios";
            this.btnNingunoAnios.Size = new System.Drawing.Size(57, 23);
            this.btnNingunoAnios.TabIndex = 19;
            this.btnNingunoAnios.Text = "&Ninguno";
            this.btnNingunoAnios.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNingunoAnios.UseVisualStyleBackColor = false;
            this.btnNingunoAnios.Click += new System.EventHandler(this.btnNingunoAnios_Click);
            // 
            // btnTodosAnios
            // 
            this.btnTodosAnios.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnTodosAnios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTodosAnios.ForeColor = System.Drawing.Color.White;
            this.btnTodosAnios.Location = new System.Drawing.Point(514, 48);
            this.btnTodosAnios.Name = "btnTodosAnios";
            this.btnTodosAnios.Size = new System.Drawing.Size(45, 23);
            this.btnTodosAnios.TabIndex = 18;
            this.btnTodosAnios.Text = "&Todos";
            this.btnTodosAnios.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTodosAnios.UseVisualStyleBackColor = false;
            this.btnTodosAnios.Click += new System.EventHandler(this.btnTodosAnios_Click);
            // 
            // clbAnios
            // 
            this.clbAnios.CheckOnClick = true;
            this.clbAnios.FormattingEnabled = true;
            this.clbAnios.Location = new System.Drawing.Point(514, 79);
            this.clbAnios.Name = "clbAnios";
            this.clbAnios.Size = new System.Drawing.Size(106, 274);
            this.clbAnios.TabIndex = 1;
            this.clbAnios.KeyDown += new System.Windows.Forms.KeyEventHandler(this.clbAnios_KeyDown);
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBusqueda.Location = new System.Drawing.Point(43, 21);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(385, 20);
            this.txtBusqueda.TabIndex = 0;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBusqueda_KeyDown);
            this.txtBusqueda.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBusqueda_KeyPress);
            // 
            // lblMostrar
            // 
            this.lblMostrar.AutoSize = true;
            this.lblMostrar.ForeColor = System.Drawing.Color.White;
            this.lblMostrar.Location = new System.Drawing.Point(3, 24);
            this.lblMostrar.Name = "lblMostrar";
            this.lblMostrar.Size = new System.Drawing.Size(32, 13);
            this.lblMostrar.TabIndex = 17;
            this.lblMostrar.Text = "Filtrar";
            // 
            // btnAgregar
            // 
            this.btnAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregar.Image = ((System.Drawing.Image)(resources.GetObject("btnAgregar.Image")));
            this.btnAgregar.Location = new System.Drawing.Point(514, 361);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(108, 23);
            this.btnAgregar.TabIndex = 5;
            this.btnAgregar.Text = "&Agregar";
            this.btnAgregar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // dgvSeleccion
            // 
            this.dgvSeleccion.AllowUserToAddRows = false;
            this.dgvSeleccion.AllowUserToResizeRows = false;
            this.dgvSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSeleccion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSeleccion.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvSeleccion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSeleccion.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvSeleccion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSeleccion.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSeleccion.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvSeleccion.Location = new System.Drawing.Point(6, 241);
            this.dgvSeleccion.Name = "dgvSeleccion";
            this.dgvSeleccion.ReadOnly = true;
            this.dgvSeleccion.RowHeadersVisible = false;
            this.dgvSeleccion.RowHeadersWidth = 25;
            this.dgvSeleccion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSeleccion.Size = new System.Drawing.Size(502, 143);
            this.dgvSeleccion.TabIndex = 16;
            // 
            // dgvModelos
            // 
            this.dgvModelos.AllowUserToAddRows = false;
            this.dgvModelos.AllowUserToDeleteRows = false;
            this.dgvModelos.AllowUserToResizeRows = false;
            this.dgvModelos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvModelos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvModelos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvModelos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvModelos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvModelos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvModelos.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvModelos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvModelos.Location = new System.Drawing.Point(6, 49);
            this.dgvModelos.Name = "dgvModelos";
            this.dgvModelos.ReadOnly = true;
            this.dgvModelos.RowHeadersVisible = false;
            this.dgvModelos.RowHeadersWidth = 25;
            this.dgvModelos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvModelos.Size = new System.Drawing.Size(502, 173);
            this.dgvModelos.TabIndex = 15;
            this.dgvModelos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModelos_CellClick);
            this.dgvModelos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvModelos_KeyDown);
            // 
            // DetalleAplicacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(674, 647);
            this.KeyPreview = true;
            this.Name = "DetalleAplicacion";
            this.Load += new System.EventHandler(this.DetalleAplicacion_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleAplicacion_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.gpoModelos.ResumeLayout(false);
            this.gpoModelos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAplicaciones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeleccion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvModelos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTipoFuente;
        private System.Windows.Forms.Label lblFuente;
        private System.Windows.Forms.ComboBox cboMarca;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gpoModelos;
        public System.Windows.Forms.DataGridView dgvModelos;
        public System.Windows.Forms.DataGridView dgvSeleccion;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Label lblSeleccion;
        private System.Windows.Forms.Label lblActuales;
        public System.Windows.Forms.DataGridView dgvAplicaciones;
        private System.Windows.Forms.TextBox txtBusqueda;
        private System.Windows.Forms.Label lblMostrar;
        private System.Windows.Forms.CheckedListBox clbAnios;
        private System.Windows.Forms.Button btnNingunoAnios;
        private System.Windows.Forms.Button btnTodosAnios;
        private System.Windows.Forms.ComboBox cboMotor;
        private System.Windows.Forms.Label lblNombreMotor;
    }
}
