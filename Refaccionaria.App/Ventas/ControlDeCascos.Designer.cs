namespace Refaccionaria.App
{
    partial class ControlDeCascos
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlDeCaja = new System.Windows.Forms.TabControl();
            this.tbpPendientes = new System.Windows.Forms.TabPage();
            this.dgvPendientes = new System.Windows.Forms.DataGridView();
            this.CascoRegistroID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FolioDeVenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroDeParteDeCasco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroDeParteRecibido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FolioDeCobro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CascoImporte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbpHistorico = new System.Windows.Forms.TabPage();
            this.dgvHistorico = new System.Windows.Forms.DataGridView();
            this.hisCascoRegistroID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisFolioDeVenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisCliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisNumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisNumeroDeParteCasco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisNumeroDeParteRecibido = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisFolioDeCobro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hisCascoImporte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnHisActualizar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpHisHasta = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpHisDesde = new System.Windows.Forms.DateTimePicker();
            this.cmbHisSucursal = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReimpresion = new Refaccionaria.Negocio.TextoMod();
            this.tabControlDeCaja.SuspendLayout();
            this.tbpPendientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendientes)).BeginInit();
            this.tbpHistorico.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorico)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlDeCaja
            // 
            this.tabControlDeCaja.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlDeCaja.Controls.Add(this.tbpPendientes);
            this.tabControlDeCaja.Controls.Add(this.tbpHistorico);
            this.tabControlDeCaja.Location = new System.Drawing.Point(3, 3);
            this.tabControlDeCaja.Name = "tabControlDeCaja";
            this.tabControlDeCaja.SelectedIndex = 0;
            this.tabControlDeCaja.Size = new System.Drawing.Size(919, 358);
            this.tabControlDeCaja.TabIndex = 0;
            // 
            // tbpPendientes
            // 
            this.tbpPendientes.Controls.Add(this.dgvPendientes);
            this.tbpPendientes.Location = new System.Drawing.Point(4, 22);
            this.tbpPendientes.Name = "tbpPendientes";
            this.tbpPendientes.Padding = new System.Windows.Forms.Padding(3);
            this.tbpPendientes.Size = new System.Drawing.Size(911, 332);
            this.tbpPendientes.TabIndex = 0;
            this.tbpPendientes.Text = "Pendientes";
            this.tbpPendientes.UseVisualStyleBackColor = true;
            // 
            // dgvPendientes
            // 
            this.dgvPendientes.AllowUserToAddRows = false;
            this.dgvPendientes.AllowUserToDeleteRows = false;
            this.dgvPendientes.AllowUserToResizeRows = false;
            this.dgvPendientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPendientes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPendientes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPendientes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPendientes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvPendientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPendientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CascoRegistroID,
            this.Fecha,
            this.FolioDeVenta,
            this.Cliente,
            this.NumeroDeParte,
            this.Descripcion,
            this.NumeroDeParteDeCasco,
            this.NumeroDeParteRecibido,
            this.FolioDeCobro,
            this.CascoImporte});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPendientes.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgvPendientes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPendientes.Location = new System.Drawing.Point(3, 3);
            this.dgvPendientes.MultiSelect = false;
            this.dgvPendientes.Name = "dgvPendientes";
            this.dgvPendientes.ReadOnly = true;
            this.dgvPendientes.RowHeadersVisible = false;
            this.dgvPendientes.RowHeadersWidth = 25;
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black;
            this.dgvPendientes.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvPendientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPendientes.Size = new System.Drawing.Size(905, 326);
            this.dgvPendientes.TabIndex = 3;
            this.dgvPendientes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPendientes_CellDoubleClick);
            // 
            // CascoRegistroID
            // 
            this.CascoRegistroID.HeaderText = "Folio";
            this.CascoRegistroID.Name = "CascoRegistroID";
            this.CascoRegistroID.ReadOnly = true;
            this.CascoRegistroID.Width = 40;
            // 
            // Fecha
            // 
            this.Fecha.HeaderText = "Fecha";
            this.Fecha.Name = "Fecha";
            this.Fecha.ReadOnly = true;
            this.Fecha.Width = 136;
            // 
            // FolioDeVenta
            // 
            this.FolioDeVenta.HeaderText = "Venta";
            this.FolioDeVenta.Name = "FolioDeVenta";
            this.FolioDeVenta.ReadOnly = true;
            this.FolioDeVenta.Width = 40;
            // 
            // Cliente
            // 
            this.Cliente.HeaderText = "Cliente";
            this.Cliente.Name = "Cliente";
            this.Cliente.ReadOnly = true;
            this.Cliente.Width = 120;
            // 
            // NumeroDeParte
            // 
            this.NumeroDeParte.HeaderText = "No. Parte";
            this.NumeroDeParte.Name = "NumeroDeParte";
            this.NumeroDeParte.ReadOnly = true;
            this.NumeroDeParte.Width = 80;
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripción";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            this.Descripcion.Width = 200;
            // 
            // NumeroDeParteDeCasco
            // 
            this.NumeroDeParteDeCasco.HeaderText = "Casco";
            this.NumeroDeParteDeCasco.Name = "NumeroDeParteDeCasco";
            this.NumeroDeParteDeCasco.ReadOnly = true;
            this.NumeroDeParteDeCasco.Width = 80;
            // 
            // NumeroDeParteRecibido
            // 
            this.NumeroDeParteRecibido.HeaderText = "Recibido";
            this.NumeroDeParteRecibido.Name = "NumeroDeParteRecibido";
            this.NumeroDeParteRecibido.ReadOnly = true;
            this.NumeroDeParteRecibido.Width = 80;
            // 
            // FolioDeCobro
            // 
            this.FolioDeCobro.HeaderText = "Cobro";
            this.FolioDeCobro.Name = "FolioDeCobro";
            this.FolioDeCobro.ReadOnly = true;
            this.FolioDeCobro.Width = 40;
            // 
            // CascoImporte
            // 
            this.CascoImporte.HeaderText = "A favor";
            this.CascoImporte.Name = "CascoImporte";
            this.CascoImporte.ReadOnly = true;
            this.CascoImporte.Width = 80;
            // 
            // tbpHistorico
            // 
            this.tbpHistorico.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.tbpHistorico.Controls.Add(this.txtReimpresion);
            this.tbpHistorico.Controls.Add(this.dgvHistorico);
            this.tbpHistorico.Controls.Add(this.btnHisActualizar);
            this.tbpHistorico.Controls.Add(this.label3);
            this.tbpHistorico.Controls.Add(this.dtpHisHasta);
            this.tbpHistorico.Controls.Add(this.label2);
            this.tbpHistorico.Controls.Add(this.dtpHisDesde);
            this.tbpHistorico.Controls.Add(this.cmbHisSucursal);
            this.tbpHistorico.Controls.Add(this.label1);
            this.tbpHistorico.Location = new System.Drawing.Point(4, 22);
            this.tbpHistorico.Name = "tbpHistorico";
            this.tbpHistorico.Padding = new System.Windows.Forms.Padding(3);
            this.tbpHistorico.Size = new System.Drawing.Size(911, 332);
            this.tbpHistorico.TabIndex = 1;
            this.tbpHistorico.Text = "Histórico";
            // 
            // dgvHistorico
            // 
            this.dgvHistorico.AllowUserToAddRows = false;
            this.dgvHistorico.AllowUserToDeleteRows = false;
            this.dgvHistorico.AllowUserToResizeRows = false;
            this.dgvHistorico.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHistorico.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvHistorico.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvHistorico.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHistorico.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvHistorico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorico.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.hisCascoRegistroID,
            this.hisFecha,
            this.hisFolioDeVenta,
            this.hisCliente,
            this.hisNumeroDeParte,
            this.hisDescripcion,
            this.hisNumeroDeParteCasco,
            this.hisNumeroDeParteRecibido,
            this.hisFolioDeCobro,
            this.hisCascoImporte});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHistorico.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvHistorico.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvHistorico.Location = new System.Drawing.Point(3, 33);
            this.dgvHistorico.MultiSelect = false;
            this.dgvHistorico.Name = "dgvHistorico";
            this.dgvHistorico.ReadOnly = true;
            this.dgvHistorico.RowHeadersVisible = false;
            this.dgvHistorico.RowHeadersWidth = 25;
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            this.dgvHistorico.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvHistorico.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorico.Size = new System.Drawing.Size(902, 296);
            this.dgvHistorico.TabIndex = 5;
            this.dgvHistorico.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistorico_CellDoubleClick);
            // 
            // hisCascoRegistroID
            // 
            this.hisCascoRegistroID.HeaderText = "Folio";
            this.hisCascoRegistroID.Name = "hisCascoRegistroID";
            this.hisCascoRegistroID.ReadOnly = true;
            this.hisCascoRegistroID.Width = 40;
            // 
            // hisFecha
            // 
            this.hisFecha.HeaderText = "Fecha";
            this.hisFecha.Name = "hisFecha";
            this.hisFecha.ReadOnly = true;
            this.hisFecha.Width = 136;
            // 
            // hisFolioDeVenta
            // 
            this.hisFolioDeVenta.HeaderText = "Venta";
            this.hisFolioDeVenta.Name = "hisFolioDeVenta";
            this.hisFolioDeVenta.ReadOnly = true;
            this.hisFolioDeVenta.Width = 55;
            // 
            // hisCliente
            // 
            this.hisCliente.HeaderText = "Cliente";
            this.hisCliente.Name = "hisCliente";
            this.hisCliente.ReadOnly = true;
            this.hisCliente.Width = 120;
            // 
            // hisNumeroDeParte
            // 
            this.hisNumeroDeParte.HeaderText = "No. Parte";
            this.hisNumeroDeParte.Name = "hisNumeroDeParte";
            this.hisNumeroDeParte.ReadOnly = true;
            this.hisNumeroDeParte.Width = 80;
            // 
            // hisDescripcion
            // 
            this.hisDescripcion.HeaderText = "Descripción";
            this.hisDescripcion.Name = "hisDescripcion";
            this.hisDescripcion.ReadOnly = true;
            this.hisDescripcion.Width = 200;
            // 
            // hisNumeroDeParteCasco
            // 
            this.hisNumeroDeParteCasco.HeaderText = "Casco";
            this.hisNumeroDeParteCasco.Name = "hisNumeroDeParteCasco";
            this.hisNumeroDeParteCasco.ReadOnly = true;
            this.hisNumeroDeParteCasco.Width = 70;
            // 
            // hisNumeroDeParteRecibido
            // 
            this.hisNumeroDeParteRecibido.HeaderText = "Recibido";
            this.hisNumeroDeParteRecibido.Name = "hisNumeroDeParteRecibido";
            this.hisNumeroDeParteRecibido.ReadOnly = true;
            this.hisNumeroDeParteRecibido.Width = 70;
            // 
            // hisFolioDeCobro
            // 
            this.hisFolioDeCobro.HeaderText = "Cobro";
            this.hisFolioDeCobro.Name = "hisFolioDeCobro";
            this.hisFolioDeCobro.ReadOnly = true;
            this.hisFolioDeCobro.Width = 65;
            // 
            // hisCascoImporte
            // 
            this.hisCascoImporte.HeaderText = "A favor";
            this.hisCascoImporte.Name = "hisCascoImporte";
            this.hisCascoImporte.ReadOnly = true;
            this.hisCascoImporte.Width = 65;
            // 
            // btnHisActualizar
            // 
            this.btnHisActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnHisActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHisActualizar.ForeColor = System.Drawing.Color.White;
            this.btnHisActualizar.Location = new System.Drawing.Point(458, 4);
            this.btnHisActualizar.Name = "btnHisActualizar";
            this.btnHisActualizar.Size = new System.Drawing.Size(75, 23);
            this.btnHisActualizar.TabIndex = 3;
            this.btnHisActualizar.Text = "&Mostrar";
            this.btnHisActualizar.UseVisualStyleBackColor = false;
            this.btnHisActualizar.Click += new System.EventHandler(this.btnHisActualizar_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(321, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "a";
            // 
            // dtpHisHasta
            // 
            this.dtpHisHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHisHasta.Location = new System.Drawing.Point(340, 7);
            this.dtpHisHasta.Name = "dtpHisHasta";
            this.dtpHisHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpHisHasta.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(188, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "De";
            // 
            // dtpHisDesde
            // 
            this.dtpHisDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHisDesde.Location = new System.Drawing.Point(215, 7);
            this.dtpHisDesde.Name = "dtpHisDesde";
            this.dtpHisDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpHisDesde.TabIndex = 1;
            // 
            // cmbHisSucursal
            // 
            this.cmbHisSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHisSucursal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbHisSucursal.FormattingEnabled = true;
            this.cmbHisSucursal.Location = new System.Drawing.Point(61, 6);
            this.cmbHisSucursal.Name = "cmbHisSucursal";
            this.cmbHisSucursal.Size = new System.Drawing.Size(121, 21);
            this.cmbHisSucursal.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Sucursal";
            // 
            // txtReimpresion
            // 
            this.txtReimpresion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReimpresion.Etiqueta = "Reimprimir";
            this.txtReimpresion.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtReimpresion.Location = new System.Drawing.Point(820, 7);
            this.txtReimpresion.Name = "txtReimpresion";
            this.txtReimpresion.PasarEnfoqueConEnter = false;
            this.txtReimpresion.SeleccionarTextoAlEnfoque = true;
            this.txtReimpresion.Size = new System.Drawing.Size(85, 20);
            this.txtReimpresion.TabIndex = 4;
            this.txtReimpresion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReimpresion_KeyPress);
            // 
            // ControlDeCascos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlDeCaja);
            this.Name = "ControlDeCascos";
            this.Size = new System.Drawing.Size(925, 364);
            this.Load += new System.EventHandler(this.ControlDeCascos_Load);
            this.tabControlDeCaja.ResumeLayout(false);
            this.tbpPendientes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendientes)).EndInit();
            this.tbpHistorico.ResumeLayout(false);
            this.tbpHistorico.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorico)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlDeCaja;
        private System.Windows.Forms.TabPage tbpPendientes;
        private System.Windows.Forms.TabPage tbpHistorico;
        private System.Windows.Forms.DataGridView dgvPendientes;
        private System.Windows.Forms.DataGridViewTextBoxColumn CascoRegistroID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn FolioDeVenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroDeParteDeCasco;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroDeParteRecibido;
        private System.Windows.Forms.DataGridViewTextBoxColumn FolioDeCobro;
        private System.Windows.Forms.DataGridViewTextBoxColumn CascoImporte;
        private System.Windows.Forms.Button btnHisActualizar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpHisHasta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpHisDesde;
        private System.Windows.Forms.ComboBox cmbHisSucursal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvHistorico;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisCascoRegistroID;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisFolioDeVenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisCliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisNumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisNumeroDeParteCasco;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisNumeroDeParteRecibido;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisFolioDeCobro;
        private System.Windows.Forms.DataGridViewTextBoxColumn hisCascoImporte;
        protected Negocio.TextoMod txtReimpresion;
    }
}
