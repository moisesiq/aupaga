namespace Refaccionaria.App
{
    partial class CajaConteoPagos
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
            this.dgvPagosBancarios = new System.Windows.Forms.DataGridView();
            this.VentaPagoDetalleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FormaDePagoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Importe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Banco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Folio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cuenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Resguardar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lblEfectivo = new System.Windows.Forms.Label();
            this.lblTransferencias = new System.Windows.Forms.Label();
            this.lblCheques = new System.Windows.Forms.Label();
            this.lblTarjetas = new System.Windows.Forms.Label();
            this.label276 = new System.Windows.Forms.Label();
            this.label277 = new System.Windows.Forms.Label();
            this.label278 = new System.Windows.Forms.Label();
            this.label279 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagosBancarios)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTotal
            // 
            this.lblTotal.Location = new System.Drawing.Point(551, 347);
            // 
            // lblEtTotal
            // 
            this.lblEtTotal.Location = new System.Drawing.Point(472, 347);
            // 
            // dgvPagosBancarios
            // 
            this.dgvPagosBancarios.AllowUserToAddRows = false;
            this.dgvPagosBancarios.AllowUserToDeleteRows = false;
            this.dgvPagosBancarios.AllowUserToResizeRows = false;
            this.dgvPagosBancarios.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.dgvPagosBancarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPagosBancarios.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvPagosBancarios.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvPagosBancarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPagosBancarios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VentaPagoDetalleID,
            this.FormaDePagoID,
            this.Tipo,
            this.Importe,
            this.Banco,
            this.Folio,
            this.Cuenta,
            this.Resguardar});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPagosBancarios.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPagosBancarios.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPagosBancarios.Location = new System.Drawing.Point(311, 3);
            this.dgvPagosBancarios.Name = "dgvPagosBancarios";
            this.dgvPagosBancarios.RowHeadersVisible = false;
            this.dgvPagosBancarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPagosBancarios.Size = new System.Drawing.Size(395, 244);
            this.dgvPagosBancarios.TabIndex = 89;
            this.dgvPagosBancarios.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPagosBancarios_CellValueChanged);
            this.dgvPagosBancarios.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvPagosBancarios_CurrentCellDirtyStateChanged);
            // 
            // VentaPagoDetalleID
            // 
            this.VentaPagoDetalleID.HeaderText = "VentaPagoDetalleID";
            this.VentaPagoDetalleID.Name = "VentaPagoDetalleID";
            this.VentaPagoDetalleID.ReadOnly = true;
            this.VentaPagoDetalleID.Visible = false;
            // 
            // FormaDePagoID
            // 
            this.FormaDePagoID.HeaderText = "FormaDePagoID";
            this.FormaDePagoID.Name = "FormaDePagoID";
            this.FormaDePagoID.ReadOnly = true;
            this.FormaDePagoID.Visible = false;
            // 
            // Tipo
            // 
            this.Tipo.HeaderText = "Tipo";
            this.Tipo.Name = "Tipo";
            this.Tipo.ReadOnly = true;
            this.Tipo.Width = 60;
            // 
            // Importe
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "C2";
            this.Importe.DefaultCellStyle = dataGridViewCellStyle1;
            this.Importe.HeaderText = "Importe";
            this.Importe.Name = "Importe";
            this.Importe.ReadOnly = true;
            this.Importe.Width = 60;
            // 
            // Banco
            // 
            this.Banco.HeaderText = "Banco";
            this.Banco.Name = "Banco";
            this.Banco.ReadOnly = true;
            this.Banco.Width = 80;
            // 
            // Folio
            // 
            this.Folio.HeaderText = "Folio";
            this.Folio.Name = "Folio";
            this.Folio.ReadOnly = true;
            this.Folio.Width = 80;
            // 
            // Cuenta
            // 
            this.Cuenta.HeaderText = "Cuenta";
            this.Cuenta.Name = "Cuenta";
            this.Cuenta.ReadOnly = true;
            this.Cuenta.Width = 50;
            // 
            // Resguardar
            // 
            this.Resguardar.HeaderText = "R";
            this.Resguardar.Name = "Resguardar";
            this.Resguardar.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Resguardar.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Resguardar.Width = 20;
            // 
            // lblEfectivo
            // 
            this.lblEfectivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEfectivo.ForeColor = System.Drawing.Color.White;
            this.lblEfectivo.Location = new System.Drawing.Point(551, 267);
            this.lblEfectivo.Name = "lblEfectivo";
            this.lblEfectivo.Size = new System.Drawing.Size(100, 20);
            this.lblEfectivo.TabIndex = 88;
            this.lblEfectivo.Text = "0.00";
            this.lblEfectivo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblTransferencias
            // 
            this.lblTransferencias.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransferencias.ForeColor = System.Drawing.Color.White;
            this.lblTransferencias.Location = new System.Drawing.Point(551, 287);
            this.lblTransferencias.Name = "lblTransferencias";
            this.lblTransferencias.Size = new System.Drawing.Size(100, 20);
            this.lblTransferencias.TabIndex = 87;
            this.lblTransferencias.Text = "0.00";
            this.lblTransferencias.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblCheques
            // 
            this.lblCheques.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCheques.ForeColor = System.Drawing.Color.White;
            this.lblCheques.Location = new System.Drawing.Point(551, 307);
            this.lblCheques.Name = "lblCheques";
            this.lblCheques.Size = new System.Drawing.Size(100, 20);
            this.lblCheques.TabIndex = 86;
            this.lblCheques.Text = "0.00";
            this.lblCheques.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblTarjetas
            // 
            this.lblTarjetas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTarjetas.ForeColor = System.Drawing.Color.White;
            this.lblTarjetas.Location = new System.Drawing.Point(551, 327);
            this.lblTarjetas.Name = "lblTarjetas";
            this.lblTarjetas.Size = new System.Drawing.Size(100, 20);
            this.lblTarjetas.TabIndex = 85;
            this.lblTarjetas.Text = "0.00";
            this.lblTarjetas.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label276
            // 
            this.label276.AutoSize = true;
            this.label276.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label276.ForeColor = System.Drawing.Color.White;
            this.label276.Location = new System.Drawing.Point(401, 327);
            this.label276.Name = "label276";
            this.label276.Size = new System.Drawing.Size(144, 20);
            this.label276.TabIndex = 81;
            this.label276.Text = "Tarj. Bancarias $";
            this.label276.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label277
            // 
            this.label277.AutoSize = true;
            this.label277.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label277.ForeColor = System.Drawing.Color.White;
            this.label277.Location = new System.Drawing.Point(450, 307);
            this.label277.Name = "label277";
            this.label277.Size = new System.Drawing.Size(95, 20);
            this.label277.TabIndex = 83;
            this.label277.Text = "Cheques $";
            this.label277.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label278
            // 
            this.label278.AutoSize = true;
            this.label278.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label278.ForeColor = System.Drawing.Color.White;
            this.label278.Location = new System.Drawing.Point(402, 287);
            this.label278.Name = "label278";
            this.label278.Size = new System.Drawing.Size(143, 20);
            this.label278.TabIndex = 82;
            this.label278.Text = "Transferencias $";
            this.label278.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label279
            // 
            this.label279.AutoSize = true;
            this.label279.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label279.ForeColor = System.Drawing.Color.White;
            this.label279.Location = new System.Drawing.Point(456, 267);
            this.label279.Name = "label279";
            this.label279.Size = new System.Drawing.Size(89, 20);
            this.label279.TabIndex = 84;
            this.label279.Text = "Efectivo $";
            this.label279.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // CajaConteoPagos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.dgvPagosBancarios);
            this.Controls.Add(this.lblEfectivo);
            this.Controls.Add(this.lblTransferencias);
            this.Controls.Add(this.lblCheques);
            this.Controls.Add(this.lblTarjetas);
            this.Controls.Add(this.label276);
            this.Controls.Add(this.label277);
            this.Controls.Add(this.label278);
            this.Controls.Add(this.label279);
            this.MostrarTotal = true;
            this.Name = "CajaConteoPagos";
            this.Size = new System.Drawing.Size(749, 381);
            this.Load += new System.EventHandler(this.CajaConteoPagos_Load);
            this.Controls.SetChildIndex(this.lblEtTotal, 0);
            this.Controls.SetChildIndex(this.lblTotal, 0);
            this.Controls.SetChildIndex(this.pnlMonedas, 0);
            this.Controls.SetChildIndex(this.label279, 0);
            this.Controls.SetChildIndex(this.label278, 0);
            this.Controls.SetChildIndex(this.label277, 0);
            this.Controls.SetChildIndex(this.label276, 0);
            this.Controls.SetChildIndex(this.lblTarjetas, 0);
            this.Controls.SetChildIndex(this.lblCheques, 0);
            this.Controls.SetChildIndex(this.lblTransferencias, 0);
            this.Controls.SetChildIndex(this.lblEfectivo, 0);
            this.Controls.SetChildIndex(this.dgvPagosBancarios, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagosBancarios)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridViewTextBoxColumn VentaPagoDetalleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn FormaDePagoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Importe;
        private System.Windows.Forms.DataGridViewTextBoxColumn Banco;
        private System.Windows.Forms.DataGridViewTextBoxColumn Folio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cuenta;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Resguardar;
        protected System.Windows.Forms.Label lblEfectivo;
        protected System.Windows.Forms.Label lblTransferencias;
        protected System.Windows.Forms.Label lblCheques;
        protected System.Windows.Forms.Label lblTarjetas;
        protected System.Windows.Forms.Label label276;
        protected System.Windows.Forms.Label label277;
        protected System.Windows.Forms.Label label278;
        protected System.Windows.Forms.Label label279;
        protected System.Windows.Forms.DataGridView dgvPagosBancarios;
    }
}
