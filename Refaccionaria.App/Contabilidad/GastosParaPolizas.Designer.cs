namespace Refaccionaria.App
{
    partial class GastosParaPolizas
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
            this.dgvGastos = new System.Windows.Forms.DataGridView();
            this.CajaEgresoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CuentaAuxiliarID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SucursalID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sucursal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CuentaAuxiliar = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Concepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGastos)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvGastos
            // 
            this.dgvGastos.AllowUserToAddRows = false;
            this.dgvGastos.AllowUserToDeleteRows = false;
            this.dgvGastos.AllowUserToResizeRows = false;
            this.dgvGastos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvGastos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvGastos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvGastos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGastos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CajaEgresoID,
            this.CuentaAuxiliarID,
            this.SucursalID,
            this.Fecha,
            this.Sucursal,
            this.CuentaAuxiliar,
            this.Importe,
            this.Concepto});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvGastos.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvGastos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGastos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvGastos.Location = new System.Drawing.Point(0, 0);
            this.dgvGastos.Name = "dgvGastos";
            this.dgvGastos.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGastos.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvGastos.RowHeadersVisible = false;
            this.dgvGastos.RowHeadersWidth = 25;
            this.dgvGastos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGastos.Size = new System.Drawing.Size(856, 403);
            this.dgvGastos.TabIndex = 9;
            this.dgvGastos.DoubleClick += new System.EventHandler(this.dgvGastos_DoubleClick);
            // 
            // CajaEgresoID
            // 
            this.CajaEgresoID.HeaderText = "CajaEgresoID";
            this.CajaEgresoID.Name = "CajaEgresoID";
            this.CajaEgresoID.ReadOnly = true;
            this.CajaEgresoID.Visible = false;
            // 
            // CuentaAuxiliarID
            // 
            this.CuentaAuxiliarID.HeaderText = "CuentaAuxiliarID";
            this.CuentaAuxiliarID.Name = "CuentaAuxiliarID";
            this.CuentaAuxiliarID.ReadOnly = true;
            this.CuentaAuxiliarID.Visible = false;
            // 
            // SucursalID
            // 
            this.SucursalID.HeaderText = "SucursalID";
            this.SucursalID.Name = "SucursalID";
            this.SucursalID.ReadOnly = true;
            this.SucursalID.Visible = false;
            // 
            // Fecha
            // 
            this.Fecha.HeaderText = "Fecha";
            this.Fecha.Name = "Fecha";
            this.Fecha.ReadOnly = true;
            this.Fecha.Width = 136;
            // 
            // Sucursal
            // 
            this.Sucursal.HeaderText = "Sucursal";
            this.Sucursal.Name = "Sucursal";
            this.Sucursal.ReadOnly = true;
            // 
            // CuentaAuxiliar
            // 
            this.CuentaAuxiliar.HeaderText = "Cuenta Auxiliar";
            this.CuentaAuxiliar.Name = "CuentaAuxiliar";
            this.CuentaAuxiliar.ReadOnly = true;
            this.CuentaAuxiliar.Width = 120;
            // 
            // Importe
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Importe.DefaultCellStyle = dataGridViewCellStyle1;
            this.Importe.HeaderText = "Importe";
            this.Importe.Name = "Importe";
            this.Importe.ReadOnly = true;
            this.Importe.Width = 80;
            // 
            // Concepto
            // 
            this.Concepto.HeaderText = "Concepto";
            this.Concepto.Name = "Concepto";
            this.Concepto.ReadOnly = true;
            this.Concepto.Width = 400;
            // 
            // GastosParaPolizas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvGastos);
            this.Name = "GastosParaPolizas";
            this.Size = new System.Drawing.Size(856, 403);
            this.Load += new System.EventHandler(this.GastosParaPolizas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGastos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvGastos;
        private System.Windows.Forms.DataGridViewTextBoxColumn CajaEgresoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CuentaAuxiliarID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SucursalID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sucursal;
        private System.Windows.Forms.DataGridViewTextBoxColumn CuentaAuxiliar;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn Concepto;
    }
}
