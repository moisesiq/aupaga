namespace Refaccionaria.App
{
    partial class BusquedaDevolucion
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdbDevolucionTarjeta = new System.Windows.Forms.RadioButton();
            this.rdbDevolucionCheque = new System.Windows.Forms.RadioButton();
            this.rdbDevolucionEfectivo = new System.Windows.Forms.RadioButton();
            this.rdbNotaDeCredito = new System.Windows.Forms.RadioButton();
            this.gpbMotivo = new System.Windows.Forms.GroupBox();
            this.txtMotivoNota = new Refaccionaria.Negocio.TextoMod();
            this.rdbMotivo3 = new System.Windows.Forms.RadioButton();
            this.rdbMotivo4 = new System.Windows.Forms.RadioButton();
            this.rdbMotivo2 = new System.Windows.Forms.RadioButton();
            this.rdbMotivo1 = new System.Windows.Forms.RadioButton();
            this.txtReimpresion = new Refaccionaria.Negocio.TextoMod();
            this.tabCancelaciones = new System.Windows.Forms.TabControl();
            this.tbpCancelaciones = new System.Windows.Forms.TabPage();
            this.tbpFacturasPorCancelar = new System.Windows.Forms.TabPage();
            this.btnCancelarFacPen = new System.Windows.Forms.Button();
            this.dgvFacturasPorCancelar = new System.Windows.Forms.DataGridView();
            this.rdbDevolucionTransferencia = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.gpbMotivo.SuspendLayout();
            this.tabCancelaciones.SuspendLayout();
            this.tbpFacturasPorCancelar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturasPorCancelar)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.rdbDevolucionTransferencia);
            this.groupBox3.Controls.Add(this.rdbDevolucionTarjeta);
            this.groupBox3.Controls.Add(this.rdbDevolucionCheque);
            this.groupBox3.Controls.Add(this.rdbDevolucionEfectivo);
            this.groupBox3.Controls.Add(this.rdbNotaDeCredito);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox3.Location = new System.Drawing.Point(393, 316);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(317, 54);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Acción a Tomar";
            // 
            // rdbDevolucionTarjeta
            // 
            this.rdbDevolucionTarjeta.AutoSize = true;
            this.rdbDevolucionTarjeta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbDevolucionTarjeta.Location = new System.Drawing.Point(133, 32);
            this.rdbDevolucionTarjeta.Name = "rdbDevolucionTarjeta";
            this.rdbDevolucionTarjeta.Size = new System.Drawing.Size(87, 17);
            this.rdbDevolucionTarjeta.TabIndex = 3;
            this.rdbDevolucionTarjeta.Text = "Dev.  Tarjeta";
            this.rdbDevolucionTarjeta.UseVisualStyleBackColor = true;
            // 
            // rdbDevolucionCheque
            // 
            this.rdbDevolucionCheque.AutoSize = true;
            this.rdbDevolucionCheque.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbDevolucionCheque.Location = new System.Drawing.Point(133, 14);
            this.rdbDevolucionCheque.Name = "rdbDevolucionCheque";
            this.rdbDevolucionCheque.Size = new System.Drawing.Size(88, 17);
            this.rdbDevolucionCheque.TabIndex = 2;
            this.rdbDevolucionCheque.Text = "Dev. Cheque";
            this.rdbDevolucionCheque.UseVisualStyleBackColor = true;
            // 
            // rdbDevolucionEfectivo
            // 
            this.rdbDevolucionEfectivo.AutoSize = true;
            this.rdbDevolucionEfectivo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbDevolucionEfectivo.Location = new System.Drawing.Point(6, 32);
            this.rdbDevolucionEfectivo.Name = "rdbDevolucionEfectivo";
            this.rdbDevolucionEfectivo.Size = new System.Drawing.Size(121, 17);
            this.rdbDevolucionEfectivo.TabIndex = 1;
            this.rdbDevolucionEfectivo.Text = "Devolución Efectivo";
            this.rdbDevolucionEfectivo.UseVisualStyleBackColor = true;
            // 
            // rdbNotaDeCredito
            // 
            this.rdbNotaDeCredito.AutoSize = true;
            this.rdbNotaDeCredito.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbNotaDeCredito.Location = new System.Drawing.Point(6, 14);
            this.rdbNotaDeCredito.Name = "rdbNotaDeCredito";
            this.rdbNotaDeCredito.Size = new System.Drawing.Size(74, 17);
            this.rdbNotaDeCredito.TabIndex = 0;
            this.rdbNotaDeCredito.Text = "Crear Vale";
            this.rdbNotaDeCredito.UseVisualStyleBackColor = true;
            // 
            // gpbMotivo
            // 
            this.gpbMotivo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gpbMotivo.Controls.Add(this.txtMotivoNota);
            this.gpbMotivo.Controls.Add(this.rdbMotivo3);
            this.gpbMotivo.Controls.Add(this.rdbMotivo4);
            this.gpbMotivo.Controls.Add(this.rdbMotivo2);
            this.gpbMotivo.Controls.Add(this.rdbMotivo1);
            this.gpbMotivo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gpbMotivo.Location = new System.Drawing.Point(7, 316);
            this.gpbMotivo.Name = "gpbMotivo";
            this.gpbMotivo.Size = new System.Drawing.Size(380, 54);
            this.gpbMotivo.TabIndex = 10;
            this.gpbMotivo.TabStop = false;
            this.gpbMotivo.Text = "Motivo";
            // 
            // txtMotivoNota
            // 
            this.txtMotivoNota.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMotivoNota.Etiqueta = "Causa";
            this.txtMotivoNota.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtMotivoNota.Location = new System.Drawing.Point(188, 13);
            this.txtMotivoNota.Multiline = true;
            this.txtMotivoNota.Name = "txtMotivoNota";
            this.txtMotivoNota.PasarEnfoqueConEnter = true;
            this.txtMotivoNota.SeleccionarTextoAlEnfoque = false;
            this.txtMotivoNota.Size = new System.Drawing.Size(186, 35);
            this.txtMotivoNota.TabIndex = 4;
            this.txtMotivoNota.WordWrap = false;
            // 
            // rdbMotivo3
            // 
            this.rdbMotivo3.AutoSize = true;
            this.rdbMotivo3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo3.Location = new System.Drawing.Point(102, 14);
            this.rdbMotivo3.Name = "rdbMotivo3";
            this.rdbMotivo3.Size = new System.Drawing.Size(57, 17);
            this.rdbMotivo3.TabIndex = 2;
            this.rdbMotivo3.Text = "Motivo";
            this.rdbMotivo3.UseVisualStyleBackColor = true;
            // 
            // rdbMotivo4
            // 
            this.rdbMotivo4.AutoSize = true;
            this.rdbMotivo4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo4.Location = new System.Drawing.Point(102, 32);
            this.rdbMotivo4.Name = "rdbMotivo4";
            this.rdbMotivo4.Size = new System.Drawing.Size(57, 17);
            this.rdbMotivo4.TabIndex = 3;
            this.rdbMotivo4.Text = "Motivo";
            this.rdbMotivo4.UseVisualStyleBackColor = true;
            // 
            // rdbMotivo2
            // 
            this.rdbMotivo2.AutoSize = true;
            this.rdbMotivo2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo2.Location = new System.Drawing.Point(7, 32);
            this.rdbMotivo2.Name = "rdbMotivo2";
            this.rdbMotivo2.Size = new System.Drawing.Size(57, 17);
            this.rdbMotivo2.TabIndex = 1;
            this.rdbMotivo2.Text = "Motivo";
            this.rdbMotivo2.UseVisualStyleBackColor = true;
            // 
            // rdbMotivo1
            // 
            this.rdbMotivo1.AutoSize = true;
            this.rdbMotivo1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo1.Location = new System.Drawing.Point(7, 14);
            this.rdbMotivo1.Name = "rdbMotivo1";
            this.rdbMotivo1.Size = new System.Drawing.Size(57, 17);
            this.rdbMotivo1.TabIndex = 0;
            this.rdbMotivo1.Text = "Motivo";
            this.rdbMotivo1.UseVisualStyleBackColor = true;
            // 
            // txtReimpresion
            // 
            this.txtReimpresion.Etiqueta = "Reimprimir";
            this.txtReimpresion.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtReimpresion.Location = new System.Drawing.Point(543, 7);
            this.txtReimpresion.Name = "txtReimpresion";
            this.txtReimpresion.PasarEnfoqueConEnter = false;
            this.txtReimpresion.SeleccionarTextoAlEnfoque = false;
            this.txtReimpresion.Size = new System.Drawing.Size(85, 20);
            this.txtReimpresion.TabIndex = 6;
            this.txtReimpresion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReimpresion_KeyPress);
            // 
            // tabCancelaciones
            // 
            this.tabCancelaciones.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabCancelaciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCancelaciones.Controls.Add(this.tbpCancelaciones);
            this.tabCancelaciones.Controls.Add(this.tbpFacturasPorCancelar);
            this.tabCancelaciones.Location = new System.Drawing.Point(0, 0);
            this.tabCancelaciones.Multiline = true;
            this.tabCancelaciones.Name = "tabCancelaciones";
            this.tabCancelaciones.SelectedIndex = 0;
            this.tabCancelaciones.Size = new System.Drawing.Size(797, 377);
            this.tabCancelaciones.TabIndex = 107;
            this.tabCancelaciones.SelectedIndexChanged += new System.EventHandler(this.tabCancelaciones_SelectedIndexChanged);
            // 
            // tbpCancelaciones
            // 
            this.tbpCancelaciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.tbpCancelaciones.Location = new System.Drawing.Point(4, 4);
            this.tbpCancelaciones.Name = "tbpCancelaciones";
            this.tbpCancelaciones.Padding = new System.Windows.Forms.Padding(3);
            this.tbpCancelaciones.Size = new System.Drawing.Size(770, 369);
            this.tbpCancelaciones.TabIndex = 0;
            this.tbpCancelaciones.Text = "Cancelación";
            // 
            // tbpFacturasPorCancelar
            // 
            this.tbpFacturasPorCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.tbpFacturasPorCancelar.Controls.Add(this.btnCancelarFacPen);
            this.tbpFacturasPorCancelar.Controls.Add(this.dgvFacturasPorCancelar);
            this.tbpFacturasPorCancelar.Location = new System.Drawing.Point(4, 4);
            this.tbpFacturasPorCancelar.Name = "tbpFacturasPorCancelar";
            this.tbpFacturasPorCancelar.Padding = new System.Windows.Forms.Padding(3);
            this.tbpFacturasPorCancelar.Size = new System.Drawing.Size(770, 369);
            this.tbpFacturasPorCancelar.TabIndex = 1;
            this.tbpFacturasPorCancelar.Text = "Facturas por cancelar";
            // 
            // btnCancelarFacPen
            // 
            this.btnCancelarFacPen.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancelarFacPen.ForeColor = System.Drawing.Color.Black;
            this.btnCancelarFacPen.Location = new System.Drawing.Point(348, 337);
            this.btnCancelarFacPen.Name = "btnCancelarFacPen";
            this.btnCancelarFacPen.Size = new System.Drawing.Size(75, 23);
            this.btnCancelarFacPen.TabIndex = 10;
            this.btnCancelarFacPen.Text = "&Cancelar";
            this.btnCancelarFacPen.UseVisualStyleBackColor = true;
            this.btnCancelarFacPen.Click += new System.EventHandler(this.btnCancelarFacPen_Click);
            // 
            // dgvFacturasPorCancelar
            // 
            this.dgvFacturasPorCancelar.AllowUserToAddRows = false;
            this.dgvFacturasPorCancelar.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.SkyBlue;
            this.dgvFacturasPorCancelar.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFacturasPorCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFacturasPorCancelar.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvFacturasPorCancelar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvFacturasPorCancelar.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvFacturasPorCancelar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFacturasPorCancelar.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFacturasPorCancelar.GridColor = System.Drawing.Color.SkyBlue;
            this.dgvFacturasPorCancelar.Location = new System.Drawing.Point(6, 6);
            this.dgvFacturasPorCancelar.Name = "dgvFacturasPorCancelar";
            this.dgvFacturasPorCancelar.ReadOnly = true;
            this.dgvFacturasPorCancelar.RowHeadersWidth = 24;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.dgvFacturasPorCancelar.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFacturasPorCancelar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFacturasPorCancelar.Size = new System.Drawing.Size(761, 325);
            this.dgvFacturasPorCancelar.StandardTab = true;
            this.dgvFacturasPorCancelar.TabIndex = 9;
            // 
            // rdbDevolucionTransferencia
            // 
            this.rdbDevolucionTransferencia.AutoSize = true;
            this.rdbDevolucionTransferencia.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbDevolucionTransferencia.Location = new System.Drawing.Point(227, 14);
            this.rdbDevolucionTransferencia.Name = "rdbDevolucionTransferencia";
            this.rdbDevolucionTransferencia.Size = new System.Drawing.Size(81, 17);
            this.rdbDevolucionTransferencia.TabIndex = 4;
            this.rdbDevolucionTransferencia.Text = "Dev. Trans.";
            this.rdbDevolucionTransferencia.UseVisualStyleBackColor = true;
            // 
            // BusquedaDevolucion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gpbMotivo);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.txtReimpresion);
            this.Controls.Add(this.tabCancelaciones);
            this.Name = "BusquedaDevolucion";
            this.Size = new System.Drawing.Size(800, 380);
            this.Load += new System.EventHandler(this.BusquedaDevoluciones_Load);
            this.Controls.SetChildIndex(this.tabCancelaciones, 0);
            this.Controls.SetChildIndex(this.txtReimpresion, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.gpbMotivo, 0);
            this.Controls.SetChildIndex(this.chkMostrarTodasLasVentas, 0);
            this.Controls.SetChildIndex(this.rdbTicket, 0);
            this.Controls.SetChildIndex(this.rdbFactura, 0);
            this.Controls.SetChildIndex(this.dtpInicio, 0);
            this.Controls.SetChildIndex(this.dtpFin, 0);
            this.Controls.SetChildIndex(this.txtFolio, 0);
            this.Controls.SetChildIndex(this.cmbSucursal, 0);
            this.Controls.SetChildIndex(this.pnlDatosDePago, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gpbMotivo.ResumeLayout(false);
            this.gpbMotivo.PerformLayout();
            this.tabCancelaciones.ResumeLayout(false);
            this.tbpFacturasPorCancelar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturasPorCancelar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdbDevolucionEfectivo;
        private System.Windows.Forms.RadioButton rdbNotaDeCredito;
        private System.Windows.Forms.GroupBox gpbMotivo;
        private Negocio.TextoMod txtMotivoNota;
        private System.Windows.Forms.RadioButton rdbMotivo3;
        private System.Windows.Forms.RadioButton rdbMotivo4;
        private System.Windows.Forms.RadioButton rdbMotivo2;
        private System.Windows.Forms.RadioButton rdbMotivo1;
        private System.Windows.Forms.RadioButton rdbDevolucionTarjeta;
        private System.Windows.Forms.RadioButton rdbDevolucionCheque;
        protected Negocio.TextoMod txtReimpresion;
        private System.Windows.Forms.TabControl tabCancelaciones;
        private System.Windows.Forms.TabPage tbpCancelaciones;
        private System.Windows.Forms.TabPage tbpFacturasPorCancelar;
        protected System.Windows.Forms.DataGridView dgvFacturasPorCancelar;
        private System.Windows.Forms.Button btnCancelarFacPen;
        private System.Windows.Forms.RadioButton rdbDevolucionTransferencia;
    }
}
