namespace Refaccionaria.App
{
    partial class CuentasPorSemana
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tgvDatos = new AdvancedDataGridView.TreeGridView();
            this.Cuentas = new AdvancedDataGridView.TreeGridColumn();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.Gastos_Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Tienda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_FormaDePago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Usuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Detalle_Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Folio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Detalle_Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gastos_Observaciones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkAfectaMetas = new System.Windows.Forms.CheckBox();
            this.cmbAnio = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.tgvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            // 
            // tgvDatos
            // 
            this.tgvDatos.AllowUserToAddRows = false;
            this.tgvDatos.AllowUserToDeleteRows = false;
            this.tgvDatos.AllowUserToResizeRows = false;
            this.tgvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tgvDatos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.tgvDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tgvDatos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.tgvDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Cuentas,
            this.Total});
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tgvDatos.DefaultCellStyle = dataGridViewCellStyle15;
            this.tgvDatos.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tgvDatos.GridColor = System.Drawing.Color.SteelBlue;
            this.tgvDatos.ImageList = null;
            this.tgvDatos.Location = new System.Drawing.Point(3, 3);
            this.tgvDatos.Name = "tgvDatos";
            this.tgvDatos.RowHeadersVisible = false;
            this.tgvDatos.Size = new System.Drawing.Size(835, 294);
            this.tgvDatos.TabIndex = 2;
            this.tgvDatos.CurrentCellChanged += new System.EventHandler(this.tgvDatos_CurrentCellChanged);
            // 
            // Cuentas
            // 
            this.Cuentas.DefaultNodeImage = null;
            this.Cuentas.Frozen = true;
            this.Cuentas.HeaderText = "Cuentas";
            this.Cuentas.Name = "Cuentas";
            this.Cuentas.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Cuentas.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cuentas.Width = 200;
            // 
            // Total
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle14.Format = "C2";
            this.Total.DefaultCellStyle = dataGridViewCellStyle14;
            this.Total.Frozen = true;
            this.Total.HeaderText = "Total";
            this.Total.Name = "Total";
            this.Total.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column1
            // 
            this.Column1.Frozen = true;
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.AllowUserToResizeRows = false;
            this.dgvDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalle.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.dgvDetalle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvDetalle.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvDetalle.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Gastos_Fecha,
            this.Gastos_Tienda,
            this.Gastos_FormaDePago,
            this.Gastos_Usuario,
            this.Detalle_Total,
            this.Gastos_Folio,
            this.Detalle_Importe,
            this.Gastos_Observaciones});
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDetalle.DefaultCellStyle = dataGridViewCellStyle18;
            this.dgvDetalle.GridColor = System.Drawing.Color.SteelBlue;
            this.dgvDetalle.Location = new System.Drawing.Point(3, 303);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDetalle.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(743, 153);
            this.dgvDetalle.StandardTab = true;
            this.dgvDetalle.TabIndex = 9;
            // 
            // Gastos_Fecha
            // 
            this.Gastos_Fecha.HeaderText = "Fecha dev.";
            this.Gastos_Fecha.Name = "Gastos_Fecha";
            this.Gastos_Fecha.ReadOnly = true;
            this.Gastos_Fecha.Width = 136;
            // 
            // Gastos_Tienda
            // 
            this.Gastos_Tienda.HeaderText = "Tienda";
            this.Gastos_Tienda.Name = "Gastos_Tienda";
            this.Gastos_Tienda.ReadOnly = true;
            // 
            // Gastos_FormaDePago
            // 
            this.Gastos_FormaDePago.HeaderText = "F. de Pago";
            this.Gastos_FormaDePago.Name = "Gastos_FormaDePago";
            this.Gastos_FormaDePago.ReadOnly = true;
            // 
            // Gastos_Usuario
            // 
            this.Gastos_Usuario.HeaderText = "Usuario";
            this.Gastos_Usuario.Name = "Gastos_Usuario";
            this.Gastos_Usuario.ReadOnly = true;
            // 
            // Detalle_Total
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "C2";
            this.Detalle_Total.DefaultCellStyle = dataGridViewCellStyle9;
            this.Detalle_Total.HeaderText = "TotalGasto";
            this.Detalle_Total.Name = "Detalle_Total";
            this.Detalle_Total.ReadOnly = true;
            this.Detalle_Total.Width = 80;
            // 
            // Gastos_Folio
            // 
            this.Gastos_Folio.DataPropertyName = "Fecha";
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle16.Format = "N2";
            this.Gastos_Folio.DefaultCellStyle = dataGridViewCellStyle16;
            this.Gastos_Folio.HeaderText = "Porcentaje";
            this.Gastos_Folio.Name = "Gastos_Folio";
            this.Gastos_Folio.ReadOnly = true;
            this.Gastos_Folio.Width = 60;
            // 
            // Detalle_Importe
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle17.Format = "C2";
            this.Detalle_Importe.DefaultCellStyle = dataGridViewCellStyle17;
            this.Detalle_Importe.HeaderText = "Devengado";
            this.Detalle_Importe.Name = "Detalle_Importe";
            this.Detalle_Importe.ReadOnly = true;
            this.Detalle_Importe.Width = 80;
            // 
            // Gastos_Observaciones
            // 
            this.Gastos_Observaciones.HeaderText = "Observaciones";
            this.Gastos_Observaciones.Name = "Gastos_Observaciones";
            this.Gastos_Observaciones.ReadOnly = true;
            this.Gastos_Observaciones.Width = 300;
            // 
            // chkAfectaMetas
            // 
            this.chkAfectaMetas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAfectaMetas.AutoSize = true;
            this.chkAfectaMetas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAfectaMetas.ForeColor = System.Drawing.Color.White;
            this.chkAfectaMetas.Location = new System.Drawing.Point(752, 303);
            this.chkAfectaMetas.Name = "chkAfectaMetas";
            this.chkAfectaMetas.Size = new System.Drawing.Size(86, 17);
            this.chkAfectaMetas.TabIndex = 10;
            this.chkAfectaMetas.Text = "Afecta Metas";
            this.chkAfectaMetas.UseVisualStyleBackColor = true;
            this.chkAfectaMetas.CheckedChanged += new System.EventHandler(this.chkAfectaMetas_CheckedChanged);
            // 
            // cmbAnio
            // 
            this.cmbAnio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAnio.FormattingEnabled = true;
            this.cmbAnio.Location = new System.Drawing.Point(752, 326);
            this.cmbAnio.Name = "cmbAnio";
            this.cmbAnio.Size = new System.Drawing.Size(86, 21);
            this.cmbAnio.TabIndex = 11;
            this.cmbAnio.SelectedIndexChanged += new System.EventHandler(this.cmbAnio_SelectedIndexChanged);
            // 
            // CuentasPorSemana
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.Controls.Add(this.cmbAnio);
            this.Controls.Add(this.chkAfectaMetas);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.tgvDatos);
            this.Name = "CuentasPorSemana";
            this.Size = new System.Drawing.Size(841, 459);
            this.Load += new System.EventHandler(this.CuentasPorSemana_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tgvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private AdvancedDataGridView.TreeGridView tgvDatos;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private AdvancedDataGridView.TreeGridColumn Cuentas;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Tienda;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_FormaDePago;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Usuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn Detalle_Total;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Folio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Detalle_Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gastos_Observaciones;
        private System.Windows.Forms.CheckBox chkAfectaMetas;
        private System.Windows.Forms.ComboBox cmbAnio;
    }
}
