namespace Refaccionaria.App
{
    partial class DetalleEquivalente
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
            this.lblLinea = new System.Windows.Forms.Label();
            this.cboLinea = new System.Windows.Forms.ComboBox();
            this.dgvSimilares = new System.Windows.Forms.DataGridView();
            this.lblMostrar = new System.Windows.Forms.Label();
            this.lblSimilares = new System.Windows.Forms.Label();
            this.lblSeleccionados = new System.Windows.Forms.Label();
            this.dgvSeleccion = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvEquivalentes = new System.Windows.Forms.DataGridView();
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.gpoGen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSimilares)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeleccion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquivalentes)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtBusqueda);
            this.gpoGen.Controls.Add(this.label1);
            this.gpoGen.Controls.Add(this.dgvEquivalentes);
            this.gpoGen.Controls.Add(this.lblSeleccionados);
            this.gpoGen.Controls.Add(this.dgvSeleccion);
            this.gpoGen.Controls.Add(this.lblSimilares);
            this.gpoGen.Controls.Add(this.lblMostrar);
            this.gpoGen.Controls.Add(this.dgvSimilares);
            this.gpoGen.Controls.Add(this.lblLinea);
            this.gpoGen.Controls.Add(this.cboLinea);
            this.gpoGen.Size = new System.Drawing.Size(570, 594);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(500, 612);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(412, 612);
            this.btnGuardar.TabIndex = 1;
            // 
            // lblLinea
            // 
            this.lblLinea.AutoSize = true;
            this.lblLinea.ForeColor = System.Drawing.Color.White;
            this.lblLinea.Location = new System.Drawing.Point(6, 16);
            this.lblLinea.Name = "lblLinea";
            this.lblLinea.Size = new System.Drawing.Size(35, 13);
            this.lblLinea.TabIndex = 10;
            this.lblLinea.Text = "Línea";
            // 
            // cboLinea
            // 
            this.cboLinea.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboLinea.FormattingEnabled = true;
            this.cboLinea.Location = new System.Drawing.Point(54, 13);
            this.cboLinea.Name = "cboLinea";
            this.cboLinea.Size = new System.Drawing.Size(222, 21);
            this.cboLinea.TabIndex = 0;
            this.cboLinea.SelectedValueChanged += new System.EventHandler(this.cboLinea_SelectedValueChanged);
            // 
            // dgvSimilares
            // 
            this.dgvSimilares.AllowUserToAddRows = false;
            this.dgvSimilares.AllowUserToDeleteRows = false;
            this.dgvSimilares.AllowUserToResizeRows = false;
            this.dgvSimilares.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSimilares.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSimilares.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvSimilares.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSimilares.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvSimilares.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSimilares.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvSimilares.Location = new System.Drawing.Point(9, 89);
            this.dgvSimilares.Name = "dgvSimilares";
            this.dgvSimilares.ReadOnly = true;
            this.dgvSimilares.RowHeadersVisible = false;
            this.dgvSimilares.RowHeadersWidth = 25;
            this.dgvSimilares.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSimilares.Size = new System.Drawing.Size(542, 150);
            this.dgvSimilares.TabIndex = 2;
            this.dgvSimilares.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSimilares_CellDoubleClick);
            this.dgvSimilares.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvSimilares_KeyDown);
            // 
            // lblMostrar
            // 
            this.lblMostrar.AutoSize = true;
            this.lblMostrar.ForeColor = System.Drawing.Color.White;
            this.lblMostrar.Location = new System.Drawing.Point(6, 46);
            this.lblMostrar.Name = "lblMostrar";
            this.lblMostrar.Size = new System.Drawing.Size(32, 13);
            this.lblMostrar.TabIndex = 14;
            this.lblMostrar.Text = "Filtrar";
            // 
            // lblSimilares
            // 
            this.lblSimilares.AutoSize = true;
            this.lblSimilares.ForeColor = System.Drawing.Color.White;
            this.lblSimilares.Location = new System.Drawing.Point(6, 73);
            this.lblSimilares.Name = "lblSimilares";
            this.lblSimilares.Size = new System.Drawing.Size(52, 13);
            this.lblSimilares.TabIndex = 15;
            this.lblSimilares.Text = "Opciones";
            // 
            // lblSeleccionados
            // 
            this.lblSeleccionados.AutoSize = true;
            this.lblSeleccionados.ForeColor = System.Drawing.Color.White;
            this.lblSeleccionados.Location = new System.Drawing.Point(6, 245);
            this.lblSeleccionados.Name = "lblSeleccionados";
            this.lblSeleccionados.Size = new System.Drawing.Size(139, 13);
            this.lblSeleccionados.TabIndex = 17;
            this.lblSeleccionados.Text = "Equivalentes seleccionados";
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
            this.dgvSeleccion.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvSeleccion.Location = new System.Drawing.Point(9, 261);
            this.dgvSeleccion.Name = "dgvSeleccion";
            this.dgvSeleccion.ReadOnly = true;
            this.dgvSeleccion.RowHeadersVisible = false;
            this.dgvSeleccion.RowHeadersWidth = 25;
            this.dgvSeleccion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSeleccion.Size = new System.Drawing.Size(542, 150);
            this.dgvSeleccion.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 416);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Equivalentes actuales";
            // 
            // dgvEquivalentes
            // 
            this.dgvEquivalentes.AllowUserToAddRows = false;
            this.dgvEquivalentes.AllowUserToDeleteRows = false;
            this.dgvEquivalentes.AllowUserToResizeRows = false;
            this.dgvEquivalentes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEquivalentes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvEquivalentes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvEquivalentes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEquivalentes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvEquivalentes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEquivalentes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvEquivalentes.Location = new System.Drawing.Point(9, 432);
            this.dgvEquivalentes.Name = "dgvEquivalentes";
            this.dgvEquivalentes.ReadOnly = true;
            this.dgvEquivalentes.RowHeadersVisible = false;
            this.dgvEquivalentes.RowHeadersWidth = 25;
            this.dgvEquivalentes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEquivalentes.Size = new System.Drawing.Size(542, 150);
            this.dgvEquivalentes.TabIndex = 4;
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBusqueda.Location = new System.Drawing.Point(54, 43);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(497, 20);
            this.txtBusqueda.TabIndex = 1;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBusqueda_KeyDown);
            this.txtBusqueda.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBusqueda_KeyPress);
            // 
            // DetalleEquivalente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(594, 643);
            this.KeyPreview = true;
            this.Name = "DetalleEquivalente";
            this.Load += new System.EventHandler(this.DetalleEquivalente_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleEquivalente_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSimilares)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeleccion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquivalentes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblLinea;
        private System.Windows.Forms.ComboBox cboLinea;
        public System.Windows.Forms.DataGridView dgvSimilares;
        private System.Windows.Forms.Label lblSimilares;
        private System.Windows.Forms.Label lblMostrar;
        private System.Windows.Forms.Label lblSeleccionados;
        public System.Windows.Forms.DataGridView dgvSeleccion;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DataGridView dgvEquivalentes;
        private System.Windows.Forms.TextBox txtBusqueda;
    }
}
