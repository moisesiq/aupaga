namespace Refaccionaria.App
{
    partial class ContaBalance
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cmbSucursal1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpHasta1 = new System.Windows.Forms.DateTimePicker();
            this.dtpDesde1 = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSucursal2 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpHasta2 = new System.Windows.Forms.DateTimePicker();
            this.dtpDesde2 = new System.Windows.Forms.DateTimePicker();
            this.dgvActivos = new System.Windows.Forms.DataGridView();
            this.act_Cuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.act_Importe1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.act_Porcentaje1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.act_Importe2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.act_Porcentaje2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.act_Resultado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvPasivos = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnMostrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvActivos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPasivos)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbSucursal1
            // 
            this.cmbSucursal1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSucursal1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSucursal1.FormattingEnabled = true;
            this.cmbSucursal1.Location = new System.Drawing.Point(79, 3);
            this.cmbSucursal1.Name = "cmbSucursal1";
            this.cmbSucursal1.Size = new System.Drawing.Size(121, 21);
            this.cmbSucursal1.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(25, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Sucursal";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(356, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Hasta";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(206, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Desde";
            // 
            // dtpHasta1
            // 
            this.dtpHasta1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta1.Location = new System.Drawing.Point(397, 4);
            this.dtpHasta1.Name = "dtpHasta1";
            this.dtpHasta1.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta1.TabIndex = 11;
            // 
            // dtpDesde1
            // 
            this.dtpDesde1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde1.Location = new System.Drawing.Point(250, 4);
            this.dtpDesde1.Name = "dtpDesde1";
            this.dtpDesde1.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde1.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(633, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 15);
            this.label5.TabIndex = 23;
            this.label5.Text = "2";
            // 
            // cmbSucursal2
            // 
            this.cmbSucursal2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSucursal2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSucursal2.FormattingEnabled = true;
            this.cmbSucursal2.Location = new System.Drawing.Point(709, 3);
            this.cmbSucursal2.Name = "cmbSucursal2";
            this.cmbSucursal2.Size = new System.Drawing.Size(121, 21);
            this.cmbSucursal2.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(655, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Sucursal";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(986, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Hasta";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(836, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Desde";
            // 
            // dtpHasta2
            // 
            this.dtpHasta2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta2.Location = new System.Drawing.Point(1027, 4);
            this.dtpHasta2.Name = "dtpHasta2";
            this.dtpHasta2.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta2.TabIndex = 18;
            // 
            // dtpDesde2
            // 
            this.dtpDesde2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde2.Location = new System.Drawing.Point(880, 4);
            this.dtpDesde2.Name = "dtpDesde2";
            this.dtpDesde2.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde2.TabIndex = 17;
            // 
            // dgvActivos
            // 
            this.dgvActivos.AllowUserToAddRows = false;
            this.dgvActivos.AllowUserToDeleteRows = false;
            this.dgvActivos.AllowUserToResizeRows = false;
            this.dgvActivos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvActivos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvActivos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvActivos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvActivos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.act_Cuenta,
            this.act_Importe1,
            this.act_Porcentaje1,
            this.act_Importe2,
            this.act_Porcentaje2,
            this.act_Resultado});
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvActivos.DefaultCellStyle = dataGridViewCellStyle18;
            this.dgvActivos.Location = new System.Drawing.Point(3, 30);
            this.dgvActivos.Name = "dgvActivos";
            this.dgvActivos.ReadOnly = true;
            this.dgvActivos.RowHeadersVisible = false;
            this.dgvActivos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvActivos.Size = new System.Drawing.Size(624, 424);
            this.dgvActivos.TabIndex = 24;
            // 
            // act_Cuenta
            // 
            this.act_Cuenta.HeaderText = "Cuenta";
            this.act_Cuenta.Name = "act_Cuenta";
            this.act_Cuenta.ReadOnly = true;
            this.act_Cuenta.Width = 320;
            // 
            // act_Importe1
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle13.Format = "C2";
            this.act_Importe1.DefaultCellStyle = dataGridViewCellStyle13;
            this.act_Importe1.HeaderText = "Importe";
            this.act_Importe1.Name = "act_Importe1";
            this.act_Importe1.ReadOnly = true;
            this.act_Importe1.Width = 80;
            // 
            // act_Porcentaje1
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle14.Format = "N2";
            this.act_Porcentaje1.DefaultCellStyle = dataGridViewCellStyle14;
            this.act_Porcentaje1.HeaderText = "%";
            this.act_Porcentaje1.Name = "act_Porcentaje1";
            this.act_Porcentaje1.ReadOnly = true;
            this.act_Porcentaje1.Width = 40;
            // 
            // act_Importe2
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle15.Format = "C2";
            this.act_Importe2.DefaultCellStyle = dataGridViewCellStyle15;
            this.act_Importe2.HeaderText = "Importe";
            this.act_Importe2.Name = "act_Importe2";
            this.act_Importe2.ReadOnly = true;
            this.act_Importe2.Width = 80;
            // 
            // act_Porcentaje2
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle16.Format = "N2";
            this.act_Porcentaje2.DefaultCellStyle = dataGridViewCellStyle16;
            this.act_Porcentaje2.HeaderText = "%";
            this.act_Porcentaje2.Name = "act_Porcentaje2";
            this.act_Porcentaje2.ReadOnly = true;
            this.act_Porcentaje2.Width = 40;
            // 
            // act_Resultado
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle17.Format = "N2";
            this.act_Resultado.DefaultCellStyle = dataGridViewCellStyle17;
            this.act_Resultado.HeaderText = "R°";
            this.act_Resultado.Name = "act_Resultado";
            this.act_Resultado.ReadOnly = true;
            this.act_Resultado.Width = 40;
            // 
            // dgvPasivos
            // 
            this.dgvPasivos.AllowUserToAddRows = false;
            this.dgvPasivos.AllowUserToDeleteRows = false;
            this.dgvPasivos.AllowUserToResizeRows = false;
            this.dgvPasivos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvPasivos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvPasivos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPasivos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPasivos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPasivos.DefaultCellStyle = dataGridViewCellStyle24;
            this.dgvPasivos.Location = new System.Drawing.Point(633, 30);
            this.dgvPasivos.Name = "dgvPasivos";
            this.dgvPasivos.ReadOnly = true;
            this.dgvPasivos.RowHeadersVisible = false;
            this.dgvPasivos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPasivos.Size = new System.Drawing.Size(624, 424);
            this.dgvPasivos.TabIndex = 25;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Cuenta";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 320;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle19.Format = "C2";
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle19;
            this.dataGridViewTextBoxColumn2.HeaderText = "Importe";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 80;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle20.Format = "N2";
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle20;
            this.dataGridViewTextBoxColumn3.HeaderText = "%";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 40;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle21.Format = "C2";
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle21;
            this.dataGridViewTextBoxColumn4.HeaderText = "Importe";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 80;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle22.Format = "N2";
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridViewTextBoxColumn5.HeaderText = "%";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 40;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle23.Format = "N2";
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle23;
            this.dataGridViewTextBoxColumn6.HeaderText = "R°";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 40;
            // 
            // btnMostrar
            // 
            this.btnMostrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnMostrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMostrar.ForeColor = System.Drawing.Color.White;
            this.btnMostrar.Location = new System.Drawing.Point(560, 3);
            this.btnMostrar.Name = "btnMostrar";
            this.btnMostrar.Size = new System.Drawing.Size(67, 22);
            this.btnMostrar.TabIndex = 26;
            this.btnMostrar.Text = "&Mostrar";
            this.btnMostrar.UseVisualStyleBackColor = false;
            this.btnMostrar.Click += new System.EventHandler(this.btnMostrar_Click);
            // 
            // ContaBalance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.btnMostrar);
            this.Controls.Add(this.dgvPasivos);
            this.Controls.Add(this.dgvActivos);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSucursal2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dtpHasta2);
            this.Controls.Add(this.dtpDesde2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbSucursal1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpHasta1);
            this.Controls.Add(this.dtpDesde1);
            this.Name = "ContaBalance";
            this.Size = new System.Drawing.Size(1264, 457);
            this.Load += new System.EventHandler(this.ContaBalance_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvActivos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPasivos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSucursal1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpHasta1;
        private System.Windows.Forms.DateTimePicker dtpDesde1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSucursal2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtpHasta2;
        private System.Windows.Forms.DateTimePicker dtpDesde2;
        private System.Windows.Forms.DataGridView dgvActivos;
        private System.Windows.Forms.DataGridViewTextBoxColumn act_Cuenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn act_Importe1;
        private System.Windows.Forms.DataGridViewTextBoxColumn act_Porcentaje1;
        private System.Windows.Forms.DataGridViewTextBoxColumn act_Importe2;
        private System.Windows.Forms.DataGridViewTextBoxColumn act_Porcentaje2;
        private System.Windows.Forms.DataGridViewTextBoxColumn act_Resultado;
        private System.Windows.Forms.DataGridView dgvPasivos;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.Button btnMostrar;
    }
}
