namespace Refaccionaria.App
{
    partial class ContaEstadoDeResultados
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.dgvDatos = new System.Windows.Forms.DataGridView();
            this.Concepto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Promedio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbSucursal = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnMostrar = new System.Windows.Forms.Button();
            this.cmbAnio = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudDecimales = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.rdbSemanalizar = new System.Windows.Forms.RadioButton();
            this.rdbFecha = new System.Windows.Forms.RadioButton();
            this.chrPorSemana = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrPorSemana)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDatos
            // 
            this.dgvDatos.AllowUserToAddRows = false;
            this.dgvDatos.AllowUserToDeleteRows = false;
            this.dgvDatos.AllowUserToResizeRows = false;
            this.dgvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Concepto,
            this.Total,
            this.Promedio});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDatos.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDatos.Location = new System.Drawing.Point(3, 30);
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.ReadOnly = true;
            this.dgvDatos.RowHeadersVisible = false;
            this.dgvDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDatos.Size = new System.Drawing.Size(824, 233);
            this.dgvDatos.TabIndex = 4;
            // 
            // Concepto
            // 
            this.Concepto.Frozen = true;
            this.Concepto.HeaderText = "Cuenta";
            this.Concepto.Name = "Concepto";
            this.Concepto.ReadOnly = true;
            this.Concepto.Width = 120;
            // 
            // Total
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Total.DefaultCellStyle = dataGridViewCellStyle1;
            this.Total.Frozen = true;
            this.Total.HeaderText = "Total";
            this.Total.Name = "Total";
            this.Total.ReadOnly = true;
            this.Total.Width = 80;
            // 
            // Promedio
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            this.Promedio.DefaultCellStyle = dataGridViewCellStyle2;
            this.Promedio.Frozen = true;
            this.Promedio.HeaderText = "Promedio";
            this.Promedio.Name = "Promedio";
            this.Promedio.ReadOnly = true;
            this.Promedio.Width = 80;
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSucursal.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSucursal.FormattingEnabled = true;
            this.cmbSucursal.Location = new System.Drawing.Point(155, 3);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.Size = new System.Drawing.Size(121, 21);
            this.cmbSucursal.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(101, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Sucursal";
            // 
            // btnMostrar
            // 
            this.btnMostrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnMostrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMostrar.ForeColor = System.Drawing.Color.White;
            this.btnMostrar.Location = new System.Drawing.Point(549, 4);
            this.btnMostrar.Name = "btnMostrar";
            this.btnMostrar.Size = new System.Drawing.Size(67, 22);
            this.btnMostrar.TabIndex = 3;
            this.btnMostrar.Text = "&Mostrar";
            this.btnMostrar.UseVisualStyleBackColor = false;
            this.btnMostrar.Click += new System.EventHandler(this.btnMostrar_Click);
            // 
            // cmbAnio
            // 
            this.cmbAnio.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAnio.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAnio.FormattingEnabled = true;
            this.cmbAnio.Location = new System.Drawing.Point(35, 3);
            this.cmbAnio.Name = "cmbAnio";
            this.cmbAnio.Size = new System.Drawing.Size(60, 21);
            this.cmbAnio.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Año";
            // 
            // nudDecimales
            // 
            this.nudDecimales.Location = new System.Drawing.Point(493, 5);
            this.nudDecimales.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudDecimales.Name = "nudDecimales";
            this.nudDecimales.Size = new System.Drawing.Size(40, 20);
            this.nudDecimales.TabIndex = 2;
            this.nudDecimales.ValueChanged += new System.EventHandler(this.nudDecimales_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(431, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 184;
            this.label9.Text = "Decimales";
            // 
            // rdbSemanalizar
            // 
            this.rdbSemanalizar.AutoSize = true;
            this.rdbSemanalizar.Checked = true;
            this.rdbSemanalizar.ForeColor = System.Drawing.Color.White;
            this.rdbSemanalizar.Location = new System.Drawing.Point(282, 5);
            this.rdbSemanalizar.Name = "rdbSemanalizar";
            this.rdbSemanalizar.Size = new System.Drawing.Size(82, 17);
            this.rdbSemanalizar.TabIndex = 185;
            this.rdbSemanalizar.TabStop = true;
            this.rdbSemanalizar.Text = "Semanalizar";
            this.rdbSemanalizar.UseVisualStyleBackColor = true;
            // 
            // rdbFecha
            // 
            this.rdbFecha.AutoSize = true;
            this.rdbFecha.ForeColor = System.Drawing.Color.White;
            this.rdbFecha.Location = new System.Drawing.Point(370, 5);
            this.rdbFecha.Name = "rdbFecha";
            this.rdbFecha.Size = new System.Drawing.Size(55, 17);
            this.rdbFecha.TabIndex = 186;
            this.rdbFecha.TabStop = true;
            this.rdbFecha.Text = "Fecha";
            this.rdbFecha.UseVisualStyleBackColor = true;
            // 
            // chrPorSemana
            // 
            this.chrPorSemana.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chrPorSemana.BackColor = System.Drawing.Color.Transparent;
            chartArea1.AlignmentOrientation = ((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations)((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Vertical | System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Horizontal)));
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.IntervalOffset = 1D;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Empty;
            chartArea1.AxisX.MajorTickMark.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisX.ScaleBreakStyle.LineColor = System.Drawing.Color.Maroon;
            chartArea1.AxisX.ScaleView.Size = 52D;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.Empty;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 82.57899F;
            chartArea1.Position.Width = 98F;
            chartArea1.Position.Y = 3F;
            this.chrPorSemana.ChartAreas.Add(chartArea1);
            this.chrPorSemana.Location = new System.Drawing.Point(3, 269);
            this.chrPorSemana.Margin = new System.Windows.Forms.Padding(0);
            this.chrPorSemana.Name = "chrPorSemana";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "Ingresos";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Name = "Costos";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Name = "Margen";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Name = "Gastos";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Name = "Utilidad";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Name = "Especiales";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Name = "Dividendos";
            this.chrPorSemana.Series.Add(series1);
            this.chrPorSemana.Series.Add(series2);
            this.chrPorSemana.Series.Add(series3);
            this.chrPorSemana.Series.Add(series4);
            this.chrPorSemana.Series.Add(series5);
            this.chrPorSemana.Series.Add(series6);
            this.chrPorSemana.Series.Add(series7);
            this.chrPorSemana.Size = new System.Drawing.Size(824, 200);
            this.chrPorSemana.TabIndex = 187;
            this.chrPorSemana.Text = "chart1";
            title1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            title1.ForeColor = System.Drawing.Color.White;
            title1.IsDockedInsideChartArea = false;
            title1.Name = "Title1";
            title1.ShadowOffset = 2;
            title1.Text = "Gráfica semanal";
            this.chrPorSemana.Titles.Add(title1);
            // 
            // ContaEstadoDeResultados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.chrPorSemana);
            this.Controls.Add(this.rdbFecha);
            this.Controls.Add(this.rdbSemanalizar);
            this.Controls.Add(this.nudDecimales);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbAnio);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMostrar);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvDatos);
            this.Name = "ContaEstadoDeResultados";
            this.Size = new System.Drawing.Size(830, 472);
            this.Load += new System.EventHandler(this.ContaEstadoDeResultados_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrPorSemana)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDatos;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnMostrar;
        private System.Windows.Forms.ComboBox cmbAnio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudDecimales;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rdbSemanalizar;
        private System.Windows.Forms.RadioButton rdbFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Concepto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private System.Windows.Forms.DataGridViewTextBoxColumn Promedio;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrPorSemana;
    }
}
