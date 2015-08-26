namespace Refaccionaria.App
{
    partial class CajaCorte
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.Formato = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pendientes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Contenido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Totales = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCambios = new System.Windows.Forms.DataGridView();
            this.CambiosFormato = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CambiosContenido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambios)).BeginInit();
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
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.White;
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.ColumnHeadersVisible = false;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Formato,
            this.Pendientes,
            this.Contenido,
            this.Totales});
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
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(656, 359);
            this.dgvDetalle.TabIndex = 5;
            this.dgvDetalle.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dgvDetalle_RowStateChanged);
            this.dgvDetalle.DoubleClick += new System.EventHandler(this.dgvDetalle_DoubleClick);
            this.dgvDetalle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDetalle_KeyDown);
            // 
            // Formato
            // 
            this.Formato.HeaderText = "Formato";
            this.Formato.Name = "Formato";
            this.Formato.ReadOnly = true;
            this.Formato.Visible = false;
            // 
            // Pendientes
            // 
            this.Pendientes.HeaderText = "Pendientes";
            this.Pendientes.Name = "Pendientes";
            this.Pendientes.ReadOnly = true;
            this.Pendientes.Width = 30;
            // 
            // Contenido
            // 
            this.Contenido.HeaderText = "Contenido";
            this.Contenido.Name = "Contenido";
            this.Contenido.ReadOnly = true;
            this.Contenido.Width = 494;
            // 
            // Totales
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.SteelBlue;
            this.Totales.DefaultCellStyle = dataGridViewCellStyle1;
            this.Totales.HeaderText = "Totales";
            this.Totales.Name = "Totales";
            this.Totales.ReadOnly = true;
            // 
            // dgvCambios
            // 
            this.dgvCambios.AllowUserToAddRows = false;
            this.dgvCambios.AllowUserToDeleteRows = false;
            this.dgvCambios.AllowUserToResizeColumns = false;
            this.dgvCambios.AllowUserToResizeRows = false;
            this.dgvCambios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCambios.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvCambios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCambios.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvCambios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCambios.ColumnHeadersVisible = false;
            this.dgvCambios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CambiosFormato,
            this.CambiosContenido});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightCyan;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCambios.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCambios.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvCambios.Location = new System.Drawing.Point(289, 29);
            this.dgvCambios.Name = "dgvCambios";
            this.dgvCambios.ReadOnly = true;
            this.dgvCambios.RowHeadersVisible = false;
            this.dgvCambios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCambios.Size = new System.Drawing.Size(333, 204);
            this.dgvCambios.TabIndex = 7;
            this.dgvCambios.Visible = false;
            // 
            // CambiosFormato
            // 
            this.CambiosFormato.HeaderText = "CambiosFormato";
            this.CambiosFormato.Name = "CambiosFormato";
            this.CambiosFormato.ReadOnly = true;
            this.CambiosFormato.Visible = false;
            // 
            // CambiosContenido
            // 
            this.CambiosContenido.HeaderText = "Contenido";
            this.CambiosContenido.Name = "CambiosContenido";
            this.CambiosContenido.ReadOnly = true;
            this.CambiosContenido.Width = 624;
            // 
            // CajaCorte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.dgvCambios);
            this.Controls.Add(this.dgvDetalle);
            this.Name = "CajaCorte";
            this.Size = new System.Drawing.Size(662, 365);
            this.Load += new System.EventHandler(this.CajaCorte_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCambios)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvDetalle;
        public System.Windows.Forms.DataGridView dgvCambios;
        private System.Windows.Forms.DataGridViewTextBoxColumn Formato;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pendientes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contenido;
        private System.Windows.Forms.DataGridViewTextBoxColumn Totales;
        private System.Windows.Forms.DataGridViewTextBoxColumn CambiosFormato;
        private System.Windows.Forms.DataGridViewTextBoxColumn CambiosContenido;


    }
}
