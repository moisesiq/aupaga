namespace Refaccionaria.App
{
    partial class PartesAbc
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDatos = new System.Windows.Forms.DataGridView();
            this.ParteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UtilidadP1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Utilidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbcDeVentas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbcDeUtilidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbcDeNegocio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbcDeProveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbcDeLinea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnMostrar = new System.Windows.Forms.Button();
            this.btnProcesar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnPausar = new System.Windows.Forms.Button();
            this.lblGuardadas = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotalPartes = new System.Windows.Forms.Label();
            this.pgbGuardado = new System.Windows.Forms.ProgressBar();
            this.pnlGuardado = new System.Windows.Forms.Panel();
            this.btnExportar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            this.pnlGuardado.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDatos
            // 
            this.dgvDatos.AllowUserToAddRows = false;
            this.dgvDatos.AllowUserToDeleteRows = false;
            this.dgvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDatos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDatos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDatos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParteID,
            this.NumeroDeParte,
            this.Descripcion,
            this.UtilidadP1,
            this.Cantidad,
            this.Utilidad,
            this.AbcDeVentas,
            this.AbcDeUtilidad,
            this.AbcDeNegocio,
            this.AbcDeProveedor,
            this.AbcDeLinea});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDatos.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDatos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvDatos.Location = new System.Drawing.Point(3, 30);
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDatos.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvDatos.RowHeadersVisible = false;
            this.dgvDatos.RowHeadersWidth = 25;
            this.dgvDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDatos.Size = new System.Drawing.Size(617, 444);
            this.dgvDatos.TabIndex = 5;
            // 
            // ParteID
            // 
            this.ParteID.HeaderText = "ParteID";
            this.ParteID.Name = "ParteID";
            this.ParteID.ReadOnly = true;
            this.ParteID.Visible = false;
            // 
            // NumeroDeParte
            // 
            this.NumeroDeParte.HeaderText = "No. Parte";
            this.NumeroDeParte.Name = "NumeroDeParte";
            this.NumeroDeParte.ReadOnly = true;
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripción";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            this.Descripcion.Width = 240;
            // 
            // UtilidadP1
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "C2";
            this.UtilidadP1.DefaultCellStyle = dataGridViewCellStyle2;
            this.UtilidadP1.HeaderText = "Util. Uni.";
            this.UtilidadP1.Name = "UtilidadP1";
            this.UtilidadP1.ReadOnly = true;
            this.UtilidadP1.Width = 80;
            // 
            // Cantidad
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = null;
            this.Cantidad.DefaultCellStyle = dataGridViewCellStyle3;
            this.Cantidad.HeaderText = "Cantidad";
            this.Cantidad.Name = "Cantidad";
            this.Cantidad.ReadOnly = true;
            this.Cantidad.Width = 80;
            // 
            // Utilidad
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "C2";
            this.Utilidad.DefaultCellStyle = dataGridViewCellStyle4;
            this.Utilidad.HeaderText = "Util. Total";
            this.Utilidad.Name = "Utilidad";
            this.Utilidad.ReadOnly = true;
            this.Utilidad.Width = 80;
            // 
            // AbcDeVentas
            // 
            this.AbcDeVentas.HeaderText = "Ventas";
            this.AbcDeVentas.Name = "AbcDeVentas";
            this.AbcDeVentas.ReadOnly = true;
            this.AbcDeVentas.Width = 60;
            // 
            // AbcDeUtilidad
            // 
            this.AbcDeUtilidad.HeaderText = "Utilidad";
            this.AbcDeUtilidad.Name = "AbcDeUtilidad";
            this.AbcDeUtilidad.ReadOnly = true;
            this.AbcDeUtilidad.Width = 60;
            // 
            // AbcDeNegocio
            // 
            this.AbcDeNegocio.HeaderText = "Negocio";
            this.AbcDeNegocio.Name = "AbcDeNegocio";
            this.AbcDeNegocio.ReadOnly = true;
            this.AbcDeNegocio.Width = 60;
            // 
            // AbcDeProveedor
            // 
            this.AbcDeProveedor.HeaderText = "Proveedor";
            this.AbcDeProveedor.Name = "AbcDeProveedor";
            this.AbcDeProveedor.ReadOnly = true;
            this.AbcDeProveedor.Width = 60;
            // 
            // AbcDeLinea
            // 
            this.AbcDeLinea.HeaderText = "Línea";
            this.AbcDeLinea.Name = "AbcDeLinea";
            this.AbcDeLinea.ReadOnly = true;
            this.AbcDeLinea.Width = 60;
            // 
            // btnMostrar
            // 
            this.btnMostrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnMostrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMostrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMostrar.ForeColor = System.Drawing.Color.White;
            this.btnMostrar.Location = new System.Drawing.Point(3, 3);
            this.btnMostrar.Name = "btnMostrar";
            this.btnMostrar.Size = new System.Drawing.Size(65, 21);
            this.btnMostrar.TabIndex = 0;
            this.btnMostrar.Text = "&Mostrar";
            this.btnMostrar.UseVisualStyleBackColor = false;
            this.btnMostrar.Click += new System.EventHandler(this.btnMostrar_Click);
            // 
            // btnProcesar
            // 
            this.btnProcesar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnProcesar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcesar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcesar.ForeColor = System.Drawing.Color.White;
            this.btnProcesar.Location = new System.Drawing.Point(74, 3);
            this.btnProcesar.Name = "btnProcesar";
            this.btnProcesar.Size = new System.Drawing.Size(65, 21);
            this.btnProcesar.TabIndex = 1;
            this.btnProcesar.Text = "&Procesar";
            this.btnProcesar.UseVisualStyleBackColor = false;
            this.btnProcesar.Click += new System.EventHandler(this.btnProcesar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(145, 3);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(65, 21);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnPausar
            // 
            this.btnPausar.Location = new System.Drawing.Point(423, 0);
            this.btnPausar.Name = "btnPausar";
            this.btnPausar.Size = new System.Drawing.Size(32, 23);
            this.btnPausar.TabIndex = 4;
            this.btnPausar.Text = "||";
            this.btnPausar.UseVisualStyleBackColor = true;
            this.btnPausar.Click += new System.EventHandler(this.btnPausar_Click);
            // 
            // lblGuardadas
            // 
            this.lblGuardadas.AutoSize = true;
            this.lblGuardadas.ForeColor = System.Drawing.Color.White;
            this.lblGuardadas.Location = new System.Drawing.Point(308, 5);
            this.lblGuardadas.Name = "lblGuardadas";
            this.lblGuardadas.Size = new System.Drawing.Size(40, 13);
            this.lblGuardadas.TabIndex = 1;
            this.lblGuardadas.Text = "12,367";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(354, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "de";
            // 
            // lblTotalPartes
            // 
            this.lblTotalPartes.AutoSize = true;
            this.lblTotalPartes.ForeColor = System.Drawing.Color.White;
            this.lblTotalPartes.Location = new System.Drawing.Point(379, 5);
            this.lblTotalPartes.Name = "lblTotalPartes";
            this.lblTotalPartes.Size = new System.Drawing.Size(40, 13);
            this.lblTotalPartes.TabIndex = 3;
            this.lblTotalPartes.Text = "15,932";
            // 
            // pgbGuardado
            // 
            this.pgbGuardado.Location = new System.Drawing.Point(2, 2);
            this.pgbGuardado.Name = "pgbGuardado";
            this.pgbGuardado.Size = new System.Drawing.Size(300, 21);
            this.pgbGuardado.Step = 1;
            this.pgbGuardado.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgbGuardado.TabIndex = 0;
            this.pgbGuardado.Value = 42;
            // 
            // pnlGuardado
            // 
            this.pnlGuardado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGuardado.Controls.Add(this.pgbGuardado);
            this.pnlGuardado.Controls.Add(this.btnPausar);
            this.pnlGuardado.Controls.Add(this.lblTotalPartes);
            this.pnlGuardado.Controls.Add(this.lblGuardadas);
            this.pnlGuardado.Controls.Add(this.label2);
            this.pnlGuardado.Location = new System.Drawing.Point(304, 3);
            this.pnlGuardado.Name = "pnlGuardado";
            this.pnlGuardado.Size = new System.Drawing.Size(458, 23);
            this.pnlGuardado.TabIndex = 4;
            this.pnlGuardado.Visible = false;
            // 
            // btnExportar
            // 
            this.btnExportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(216, 3);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(65, 21);
            this.btnExportar.TabIndex = 3;
            this.btnExportar.Text = "&Exportar";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(626, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "ABC Ventas";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(626, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 91);
            this.label3.TabIndex = 6;
            this.label3.Text = "A\r\nB \r\nC\r\nD\r\nE\r\nN\r\nZ";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(642, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 91);
            this.label4.TabIndex = 6;
            this.label4.Text = ">20\r\n12 - 19\r\n  8 - 11\r\n  3 - 7\r\n  2\r\n  1\r\n  0";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(626, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "ABC Utilidad";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(626, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 91);
            this.label6.TabIndex = 6;
            this.label6.Text = "A\r\nB \r\nC\r\nD\r\nE\r\nN\r\nZ";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(642, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 91);
            this.label7.TabIndex = 6;
            this.label7.Text = "250.00 >>\r\n100.00 - 249.99\r\n  50.00 -   99.99\r\n  25.00 -   49.99\r\n  15.00 -   24." +
    "99\r\n    7.50 -   14.99\r\n    0.01 -     7.49";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(626, 254);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "ABC Negocio";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(626, 267);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 91);
            this.label9.TabIndex = 6;
            this.label9.Text = "A\r\nB \r\nC\r\nD\r\nE\r\nN\r\nZ";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(642, 267);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 91);
            this.label10.TabIndex = 6;
            this.label10.Text = "  20%\r\n  40%\r\n  60%\r\n  80%\r\n  90%\r\n  95%\r\n100%";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(626, 363);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(132, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "ABC Proveedor  Linea";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.Control;
            this.label12.Location = new System.Drawing.Point(626, 376);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(19, 91);
            this.label12.TabIndex = 6;
            this.label12.Text = "A\r\nB \r\nC\r\nD\r\nE\r\nN\r\nZ";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.Control;
            this.label13.Location = new System.Drawing.Point(664, 376);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 91);
            this.label13.TabIndex = 6;
            this.label13.Text = "  80 %          50 %\r\n  90 %          60 %\r\n  95 %          80 %\r\n  98 %         " +
    " 90 %\r\n99.5%          95 %\r\n99.9%          99 %\r\n 100%        100 %";
            // 
            // PartesAbc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.pnlGuardado);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnProcesar);
            this.Controls.Add(this.btnMostrar);
            this.Controls.Add(this.dgvDatos);
            this.Name = "PartesAbc";
            this.Size = new System.Drawing.Size(765, 477);
            this.Load += new System.EventHandler(this.PartesAbc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            this.pnlGuardado.ResumeLayout(false);
            this.pnlGuardado.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvDatos;
        private System.Windows.Forms.Button btnMostrar;
        private System.Windows.Forms.Button btnProcesar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnPausar;
        private System.Windows.Forms.Label lblGuardadas;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalPartes;
        private System.Windows.Forms.ProgressBar pgbGuardado;
        private System.Windows.Forms.Panel pnlGuardado;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn UtilidadP1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn Utilidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbcDeVentas;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbcDeUtilidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbcDeNegocio;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbcDeProveedor;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbcDeLinea;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}
