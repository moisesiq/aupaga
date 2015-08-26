namespace Refaccionaria.App
{
    partial class CajaDetalle
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.Formato = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Contenido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VistoBueno = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TextoCheck = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tabla = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegistroID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Caracteristica = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.AllowUserToResizeColumns = false;
            this.dgvDetalle.AllowUserToResizeRows = false;
            this.dgvDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.ColumnHeadersVisible = false;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Formato,
            this.Contenido,
            this.VistoBueno,
            this.TextoCheck,
            this.Tabla,
            this.RegistroID,
            this.Caracteristica});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightCyan;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDetalle.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDetalle.Location = new System.Drawing.Point(3, 3);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(659, 359);
            this.dgvDetalle.TabIndex = 4;
            this.dgvDetalle.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetalle_CellValueChanged);
            this.dgvDetalle.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvDetalle_CurrentCellDirtyStateChanged);
            this.dgvDetalle.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvDetalle_DataError);
            this.dgvDetalle.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dgvDetalle_RowStateChanged);
            this.dgvDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDetalle_KeyDown);
            // 
            // Formato
            // 
            this.Formato.HeaderText = "Formato";
            this.Formato.Name = "Formato";
            this.Formato.Visible = false;
            // 
            // Contenido
            // 
            this.Contenido.HeaderText = "Contenido";
            this.Contenido.Name = "Contenido";
            this.Contenido.ReadOnly = true;
            this.Contenido.Width = 600;
            // 
            // VistoBueno
            // 
            this.VistoBueno.HeaderText = "VistoBueno";
            this.VistoBueno.Name = "VistoBueno";
            this.VistoBueno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.VistoBueno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.VistoBueno.Width = 17;
            // 
            // TextoCheck
            // 
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Gray;
            this.TextoCheck.DefaultCellStyle = dataGridViewCellStyle1;
            this.TextoCheck.HeaderText = "TextoCheck";
            this.TextoCheck.Name = "TextoCheck";
            this.TextoCheck.Width = 42;
            // 
            // Tabla
            // 
            this.Tabla.HeaderText = "Tabla";
            this.Tabla.Name = "Tabla";
            this.Tabla.Visible = false;
            // 
            // RegistroID
            // 
            this.RegistroID.HeaderText = "RegistroID";
            this.RegistroID.Name = "RegistroID";
            this.RegistroID.Visible = false;
            // 
            // Caracteristica
            // 
            this.Caracteristica.HeaderText = "Característica";
            this.Caracteristica.Name = "Caracteristica";
            this.Caracteristica.Visible = false;
            // 
            // CajaDetalle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.Controls.Add(this.dgvDetalle);
            this.Name = "CajaDetalle";
            this.Size = new System.Drawing.Size(662, 365);
            this.Load += new System.EventHandler(this.CajaDetalle_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.DataGridViewTextBoxColumn Formato;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contenido;
        private System.Windows.Forms.DataGridViewCheckBoxColumn VistoBueno;
        private System.Windows.Forms.DataGridViewTextBoxColumn TextoCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tabla;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegistroID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Caracteristica;
    }
}
