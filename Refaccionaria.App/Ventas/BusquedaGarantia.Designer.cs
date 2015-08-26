namespace Refaccionaria.App
{
    partial class BusquedaGarantia
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
            this.tabGarantia = new System.Windows.Forms.TabControl();
            this.tbpGarantia = new System.Windows.Forms.TabPage();
            this.txtReimpresion = new Refaccionaria.Negocio.TextoMod();
            this.gpbMotivo = new System.Windows.Forms.GroupBox();
            this.txtMotivoNota = new Refaccionaria.Negocio.TextoMod();
            this.rdbMotivo3 = new System.Windows.Forms.RadioButton();
            this.rdbMotivo4 = new System.Windows.Forms.RadioButton();
            this.rdbMotivo2 = new System.Windows.Forms.RadioButton();
            this.rdbMotivo1 = new System.Windows.Forms.RadioButton();
            this.gpbAccion = new System.Windows.Forms.GroupBox();
            this.rdbAcTransferencia = new System.Windows.Forms.RadioButton();
            this.rdbAcRevision = new System.Windows.Forms.RadioButton();
            this.rdbAcTarjeta = new System.Windows.Forms.RadioButton();
            this.rdbAcCheque = new System.Windows.Forms.RadioButton();
            this.rdbAcEfectivo = new System.Windows.Forms.RadioButton();
            this.rdbAcNotaDeCredito = new System.Windows.Forms.RadioButton();
            this.rdbAcArticuloNuevo = new System.Windows.Forms.RadioButton();
            this.tbpPendientes = new System.Windows.Forms.TabPage();
            this.dtpPenHasta = new System.Windows.Forms.DateTimePicker();
            this.dtpPenDesde = new System.Windows.Forms.DateTimePicker();
            this.chkPenMostrarTodas = new System.Windows.Forms.CheckBox();
            this.gpbAccionPosterior = new System.Windows.Forms.GroupBox();
            this.rdbApTransferencia = new System.Windows.Forms.RadioButton();
            this.rdbApNoProcede = new System.Windows.Forms.RadioButton();
            this.txtAccionObservacion = new Refaccionaria.Negocio.TextoMod();
            this.rdbApTarjeta = new System.Windows.Forms.RadioButton();
            this.rdbApCheque = new System.Windows.Forms.RadioButton();
            this.rdbApEfectivo = new System.Windows.Forms.RadioButton();
            this.rdbApNotaDeCredito = new System.Windows.Forms.RadioButton();
            this.rdbApArticuloNuevo = new System.Windows.Forms.RadioButton();
            this.dgvPendientes = new System.Windows.Forms.DataGridView();
            this.penVentaGarantiaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penEstatusGenericoID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penNumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penNombreParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penProveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penFolioDeVenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penSucursal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penMotivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penMotivoObservacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penEstatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penAccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.penObservacionCompletado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).BeginInit();
            this.tabGarantia.SuspendLayout();
            this.tbpGarantia.SuspendLayout();
            this.gpbMotivo.SuspendLayout();
            this.gpbAccion.SuspendLayout();
            this.tbpPendientes.SuspendLayout();
            this.gpbAccionPosterior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendientes)).BeginInit();
            this.SuspendLayout();
            // 
            // tabGarantia
            // 
            this.tabGarantia.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabGarantia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabGarantia.Controls.Add(this.tbpGarantia);
            this.tabGarantia.Controls.Add(this.tbpPendientes);
            this.tabGarantia.Location = new System.Drawing.Point(0, 0);
            this.tabGarantia.Multiline = true;
            this.tabGarantia.Name = "tabGarantia";
            this.tabGarantia.SelectedIndex = 0;
            this.tabGarantia.Size = new System.Drawing.Size(840, 380);
            this.tabGarantia.TabIndex = 107;
            this.tabGarantia.SelectedIndexChanged += new System.EventHandler(this.tabGarantia_SelectedIndexChanged);
            // 
            // tbpGarantia
            // 
            this.tbpGarantia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.tbpGarantia.Controls.Add(this.txtReimpresion);
            this.tbpGarantia.Controls.Add(this.gpbMotivo);
            this.tbpGarantia.Controls.Add(this.gpbAccion);
            this.tbpGarantia.Location = new System.Drawing.Point(4, 4);
            this.tbpGarantia.Name = "tbpGarantia";
            this.tbpGarantia.Padding = new System.Windows.Forms.Padding(3);
            this.tbpGarantia.Size = new System.Drawing.Size(813, 372);
            this.tbpGarantia.TabIndex = 0;
            this.tbpGarantia.Text = "Garantía";
            // 
            // txtReimpresion
            // 
            this.txtReimpresion.Etiqueta = "Reimprimir";
            this.txtReimpresion.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtReimpresion.Location = new System.Drawing.Point(543, 3);
            this.txtReimpresion.Name = "txtReimpresion";
            this.txtReimpresion.PasarEnfoqueConEnter = false;
            this.txtReimpresion.SeleccionarTextoAlEnfoque = false;
            this.txtReimpresion.Size = new System.Drawing.Size(85, 20);
            this.txtReimpresion.TabIndex = 6;
            this.txtReimpresion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReimpresion_KeyPress);
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
            this.gpbMotivo.Location = new System.Drawing.Point(6, 312);
            this.gpbMotivo.Name = "gpbMotivo";
            this.gpbMotivo.Size = new System.Drawing.Size(450, 54);
            this.gpbMotivo.TabIndex = 10;
            this.gpbMotivo.TabStop = false;
            this.gpbMotivo.Text = "Motivo";
            // 
            // txtMotivoNota
            // 
            this.txtMotivoNota.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtMotivoNota.Etiqueta = "Causa";
            this.txtMotivoNota.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtMotivoNota.Location = new System.Drawing.Point(259, 14);
            this.txtMotivoNota.MaxLength = 512;
            this.txtMotivoNota.Multiline = true;
            this.txtMotivoNota.Name = "txtMotivoNota";
            this.txtMotivoNota.PasarEnfoqueConEnter = true;
            this.txtMotivoNota.SeleccionarTextoAlEnfoque = false;
            this.txtMotivoNota.Size = new System.Drawing.Size(185, 35);
            this.txtMotivoNota.TabIndex = 4;
            this.txtMotivoNota.WordWrap = false;
            // 
            // rdbMotivo3
            // 
            this.rdbMotivo3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo3.Location = new System.Drawing.Point(133, 14);
            this.rdbMotivo3.Name = "rdbMotivo3";
            this.rdbMotivo3.Size = new System.Drawing.Size(120, 17);
            this.rdbMotivo3.TabIndex = 2;
            this.rdbMotivo3.Text = "Motivo";
            this.rdbMotivo3.UseVisualStyleBackColor = true;
            // 
            // rdbMotivo4
            // 
            this.rdbMotivo4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo4.Location = new System.Drawing.Point(133, 32);
            this.rdbMotivo4.Name = "rdbMotivo4";
            this.rdbMotivo4.Size = new System.Drawing.Size(120, 17);
            this.rdbMotivo4.TabIndex = 3;
            this.rdbMotivo4.Text = "Motivo";
            this.rdbMotivo4.UseVisualStyleBackColor = true;
            // 
            // rdbMotivo2
            // 
            this.rdbMotivo2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo2.Location = new System.Drawing.Point(7, 32);
            this.rdbMotivo2.Name = "rdbMotivo2";
            this.rdbMotivo2.Size = new System.Drawing.Size(120, 17);
            this.rdbMotivo2.TabIndex = 1;
            this.rdbMotivo2.Text = "Motivo";
            this.rdbMotivo2.UseVisualStyleBackColor = true;
            // 
            // rdbMotivo1
            // 
            this.rdbMotivo1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbMotivo1.Location = new System.Drawing.Point(7, 14);
            this.rdbMotivo1.Name = "rdbMotivo1";
            this.rdbMotivo1.Size = new System.Drawing.Size(120, 17);
            this.rdbMotivo1.TabIndex = 0;
            this.rdbMotivo1.Text = "Motivo";
            this.rdbMotivo1.UseVisualStyleBackColor = true;
            // 
            // gpbAccion
            // 
            this.gpbAccion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbAccion.Controls.Add(this.rdbAcTransferencia);
            this.gpbAccion.Controls.Add(this.rdbAcRevision);
            this.gpbAccion.Controls.Add(this.rdbAcTarjeta);
            this.gpbAccion.Controls.Add(this.rdbAcCheque);
            this.gpbAccion.Controls.Add(this.rdbAcEfectivo);
            this.gpbAccion.Controls.Add(this.rdbAcNotaDeCredito);
            this.gpbAccion.Controls.Add(this.rdbAcArticuloNuevo);
            this.gpbAccion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gpbAccion.Location = new System.Drawing.Point(462, 312);
            this.gpbAccion.Name = "gpbAccion";
            this.gpbAccion.Size = new System.Drawing.Size(345, 54);
            this.gpbAccion.TabIndex = 11;
            this.gpbAccion.TabStop = false;
            this.gpbAccion.Text = "Acción a tomar";
            // 
            // rdbAcTransferencia
            // 
            this.rdbAcTransferencia.AutoSize = true;
            this.rdbAcTransferencia.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbAcTransferencia.Location = new System.Drawing.Point(203, 32);
            this.rdbAcTransferencia.Name = "rdbAcTransferencia";
            this.rdbAcTransferencia.Size = new System.Drawing.Size(81, 17);
            this.rdbAcTransferencia.TabIndex = 5;
            this.rdbAcTransferencia.Text = "Dev. Trans.";
            this.rdbAcTransferencia.UseVisualStyleBackColor = true;
            // 
            // rdbAcRevision
            // 
            this.rdbAcRevision.AutoSize = true;
            this.rdbAcRevision.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbAcRevision.Location = new System.Drawing.Point(293, 15);
            this.rdbAcRevision.Name = "rdbAcRevision";
            this.rdbAcRevision.Size = new System.Drawing.Size(118, 17);
            this.rdbAcRevision.TabIndex = 6;
            this.rdbAcRevision.Text = "Revisión Proveedor";
            this.rdbAcRevision.UseVisualStyleBackColor = true;
            // 
            // rdbAcTarjeta
            // 
            this.rdbAcTarjeta.AutoSize = true;
            this.rdbAcTarjeta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbAcTarjeta.Location = new System.Drawing.Point(203, 14);
            this.rdbAcTarjeta.Name = "rdbAcTarjeta";
            this.rdbAcTarjeta.Size = new System.Drawing.Size(84, 17);
            this.rdbAcTarjeta.TabIndex = 4;
            this.rdbAcTarjeta.Text = "Dev. Tarjeta";
            this.rdbAcTarjeta.UseVisualStyleBackColor = true;
            // 
            // rdbAcCheque
            // 
            this.rdbAcCheque.AutoSize = true;
            this.rdbAcCheque.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbAcCheque.Location = new System.Drawing.Point(107, 32);
            this.rdbAcCheque.Name = "rdbAcCheque";
            this.rdbAcCheque.Size = new System.Drawing.Size(88, 17);
            this.rdbAcCheque.TabIndex = 3;
            this.rdbAcCheque.Text = "Dev. Cheque";
            this.rdbAcCheque.UseVisualStyleBackColor = true;
            // 
            // rdbAcEfectivo
            // 
            this.rdbAcEfectivo.AutoSize = true;
            this.rdbAcEfectivo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbAcEfectivo.Location = new System.Drawing.Point(107, 14);
            this.rdbAcEfectivo.Name = "rdbAcEfectivo";
            this.rdbAcEfectivo.Size = new System.Drawing.Size(90, 17);
            this.rdbAcEfectivo.TabIndex = 2;
            this.rdbAcEfectivo.Text = "Dev. Efectivo";
            this.rdbAcEfectivo.UseVisualStyleBackColor = true;
            // 
            // rdbAcNotaDeCredito
            // 
            this.rdbAcNotaDeCredito.AutoSize = true;
            this.rdbAcNotaDeCredito.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbAcNotaDeCredito.Location = new System.Drawing.Point(6, 32);
            this.rdbAcNotaDeCredito.Name = "rdbAcNotaDeCredito";
            this.rdbAcNotaDeCredito.Size = new System.Drawing.Size(74, 17);
            this.rdbAcNotaDeCredito.TabIndex = 1;
            this.rdbAcNotaDeCredito.Text = "Crear Vale";
            this.rdbAcNotaDeCredito.UseVisualStyleBackColor = true;
            // 
            // rdbAcArticuloNuevo
            // 
            this.rdbAcArticuloNuevo.AutoSize = true;
            this.rdbAcArticuloNuevo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbAcArticuloNuevo.Location = new System.Drawing.Point(6, 14);
            this.rdbAcArticuloNuevo.Name = "rdbAcArticuloNuevo";
            this.rdbAcArticuloNuevo.Size = new System.Drawing.Size(95, 17);
            this.rdbAcArticuloNuevo.TabIndex = 0;
            this.rdbAcArticuloNuevo.Text = "Artículo nuevo";
            this.rdbAcArticuloNuevo.UseVisualStyleBackColor = true;
            // 
            // tbpPendientes
            // 
            this.tbpPendientes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.tbpPendientes.Controls.Add(this.dtpPenHasta);
            this.tbpPendientes.Controls.Add(this.dtpPenDesde);
            this.tbpPendientes.Controls.Add(this.chkPenMostrarTodas);
            this.tbpPendientes.Controls.Add(this.gpbAccionPosterior);
            this.tbpPendientes.Controls.Add(this.dgvPendientes);
            this.tbpPendientes.Location = new System.Drawing.Point(4, 4);
            this.tbpPendientes.Name = "tbpPendientes";
            this.tbpPendientes.Padding = new System.Windows.Forms.Padding(3);
            this.tbpPendientes.Size = new System.Drawing.Size(813, 372);
            this.tbpPendientes.TabIndex = 1;
            this.tbpPendientes.Text = "Pendientes";
            // 
            // dtpPenHasta
            // 
            this.dtpPenHasta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpPenHasta.Enabled = false;
            this.dtpPenHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPenHasta.Location = new System.Drawing.Point(600, 6);
            this.dtpPenHasta.Name = "dtpPenHasta";
            this.dtpPenHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpPenHasta.TabIndex = 16;
            this.dtpPenHasta.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpPenHasta_KeyDown);
            // 
            // dtpPenDesde
            // 
            this.dtpPenDesde.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpPenDesde.Enabled = false;
            this.dtpPenDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPenDesde.Location = new System.Drawing.Point(494, 6);
            this.dtpPenDesde.Name = "dtpPenDesde";
            this.dtpPenDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpPenDesde.TabIndex = 15;
            this.dtpPenDesde.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpPenDesde_KeyDown);
            // 
            // chkPenMostrarTodas
            // 
            this.chkPenMostrarTodas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPenMostrarTodas.AutoSize = true;
            this.chkPenMostrarTodas.ForeColor = System.Drawing.Color.Black;
            this.chkPenMostrarTodas.Location = new System.Drawing.Point(432, 8);
            this.chkPenMostrarTodas.Name = "chkPenMostrarTodas";
            this.chkPenMostrarTodas.Size = new System.Drawing.Size(56, 17);
            this.chkPenMostrarTodas.TabIndex = 14;
            this.chkPenMostrarTodas.Text = "Todas";
            this.chkPenMostrarTodas.UseVisualStyleBackColor = true;
            this.chkPenMostrarTodas.CheckedChanged += new System.EventHandler(this.chkPenMostrarTodas_CheckedChanged);
            // 
            // gpbAccionPosterior
            // 
            this.gpbAccionPosterior.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbAccionPosterior.Controls.Add(this.rdbApTransferencia);
            this.gpbAccionPosterior.Controls.Add(this.rdbApNoProcede);
            this.gpbAccionPosterior.Controls.Add(this.txtAccionObservacion);
            this.gpbAccionPosterior.Controls.Add(this.rdbApTarjeta);
            this.gpbAccionPosterior.Controls.Add(this.rdbApCheque);
            this.gpbAccionPosterior.Controls.Add(this.rdbApEfectivo);
            this.gpbAccionPosterior.Controls.Add(this.rdbApNotaDeCredito);
            this.gpbAccionPosterior.Controls.Add(this.rdbApArticuloNuevo);
            this.gpbAccionPosterior.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gpbAccionPosterior.Location = new System.Drawing.Point(6, 312);
            this.gpbAccionPosterior.Name = "gpbAccionPosterior";
            this.gpbAccionPosterior.Size = new System.Drawing.Size(801, 54);
            this.gpbAccionPosterior.TabIndex = 13;
            this.gpbAccionPosterior.TabStop = false;
            this.gpbAccionPosterior.Text = "Acción a tomar";
            // 
            // rdbApTransferencia
            // 
            this.rdbApTransferencia.AutoSize = true;
            this.rdbApTransferencia.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbApTransferencia.Location = new System.Drawing.Point(215, 32);
            this.rdbApTransferencia.Name = "rdbApTransferencia";
            this.rdbApTransferencia.Size = new System.Drawing.Size(81, 17);
            this.rdbApTransferencia.TabIndex = 5;
            this.rdbApTransferencia.Text = "Dev. Trans.";
            this.rdbApTransferencia.UseVisualStyleBackColor = true;
            // 
            // rdbApNoProcede
            // 
            this.rdbApNoProcede.AutoSize = true;
            this.rdbApNoProcede.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbApNoProcede.Location = new System.Drawing.Point(305, 14);
            this.rdbApNoProcede.Name = "rdbApNoProcede";
            this.rdbApNoProcede.Size = new System.Drawing.Size(83, 17);
            this.rdbApNoProcede.TabIndex = 6;
            this.rdbApNoProcede.Text = "No procedió";
            this.rdbApNoProcede.UseVisualStyleBackColor = true;
            // 
            // txtAccionObservacion
            // 
            this.txtAccionObservacion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccionObservacion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAccionObservacion.Etiqueta = "Observación";
            this.txtAccionObservacion.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtAccionObservacion.Location = new System.Drawing.Point(394, 13);
            this.txtAccionObservacion.MaxLength = 512;
            this.txtAccionObservacion.Multiline = true;
            this.txtAccionObservacion.Name = "txtAccionObservacion";
            this.txtAccionObservacion.PasarEnfoqueConEnter = true;
            this.txtAccionObservacion.SeleccionarTextoAlEnfoque = false;
            this.txtAccionObservacion.Size = new System.Drawing.Size(401, 35);
            this.txtAccionObservacion.TabIndex = 7;
            this.txtAccionObservacion.WordWrap = false;
            // 
            // rdbApTarjeta
            // 
            this.rdbApTarjeta.AutoSize = true;
            this.rdbApTarjeta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbApTarjeta.Location = new System.Drawing.Point(215, 14);
            this.rdbApTarjeta.Name = "rdbApTarjeta";
            this.rdbApTarjeta.Size = new System.Drawing.Size(84, 17);
            this.rdbApTarjeta.TabIndex = 4;
            this.rdbApTarjeta.Text = "Dev. Tarjeta";
            this.rdbApTarjeta.UseVisualStyleBackColor = true;
            // 
            // rdbApCheque
            // 
            this.rdbApCheque.AutoSize = true;
            this.rdbApCheque.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbApCheque.Location = new System.Drawing.Point(119, 32);
            this.rdbApCheque.Name = "rdbApCheque";
            this.rdbApCheque.Size = new System.Drawing.Size(88, 17);
            this.rdbApCheque.TabIndex = 3;
            this.rdbApCheque.Text = "Dev. Cheque";
            this.rdbApCheque.UseVisualStyleBackColor = true;
            // 
            // rdbApEfectivo
            // 
            this.rdbApEfectivo.AutoSize = true;
            this.rdbApEfectivo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbApEfectivo.Location = new System.Drawing.Point(119, 14);
            this.rdbApEfectivo.Name = "rdbApEfectivo";
            this.rdbApEfectivo.Size = new System.Drawing.Size(90, 17);
            this.rdbApEfectivo.TabIndex = 2;
            this.rdbApEfectivo.Text = "Dev. Efectivo";
            this.rdbApEfectivo.UseVisualStyleBackColor = true;
            // 
            // rdbApNotaDeCredito
            // 
            this.rdbApNotaDeCredito.AutoSize = true;
            this.rdbApNotaDeCredito.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbApNotaDeCredito.Location = new System.Drawing.Point(6, 32);
            this.rdbApNotaDeCredito.Name = "rdbApNotaDeCredito";
            this.rdbApNotaDeCredito.Size = new System.Drawing.Size(74, 17);
            this.rdbApNotaDeCredito.TabIndex = 1;
            this.rdbApNotaDeCredito.Text = "Crear Vale";
            this.rdbApNotaDeCredito.UseVisualStyleBackColor = true;
            // 
            // rdbApArticuloNuevo
            // 
            this.rdbApArticuloNuevo.AutoSize = true;
            this.rdbApArticuloNuevo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdbApArticuloNuevo.Location = new System.Drawing.Point(6, 14);
            this.rdbApArticuloNuevo.Name = "rdbApArticuloNuevo";
            this.rdbApArticuloNuevo.Size = new System.Drawing.Size(95, 17);
            this.rdbApArticuloNuevo.TabIndex = 0;
            this.rdbApArticuloNuevo.Text = "Artículo nuevo";
            this.rdbApArticuloNuevo.UseVisualStyleBackColor = true;
            // 
            // dgvPendientes
            // 
            this.dgvPendientes.AllowUserToAddRows = false;
            this.dgvPendientes.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.SkyBlue;
            this.dgvPendientes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPendientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPendientes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPendientes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvPendientes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvPendientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPendientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.penVentaGarantiaID,
            this.penEstatusGenericoID,
            this.penNumeroDeParte,
            this.penNombreParte,
            this.penProveedor,
            this.penFolioDeVenta,
            this.penSucursal,
            this.penFecha,
            this.penMotivo,
            this.penMotivoObservacion,
            this.penEstatus,
            this.penAccion,
            this.penObservacionCompletado});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPendientes.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPendientes.GridColor = System.Drawing.Color.SkyBlue;
            this.dgvPendientes.Location = new System.Drawing.Point(3, 29);
            this.dgvPendientes.Name = "dgvPendientes";
            this.dgvPendientes.ReadOnly = true;
            this.dgvPendientes.RowHeadersWidth = 24;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.dgvPendientes.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPendientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPendientes.Size = new System.Drawing.Size(804, 277);
            this.dgvPendientes.StandardTab = true;
            this.dgvPendientes.TabIndex = 10;
            this.dgvPendientes.CurrentCellChanged += new System.EventHandler(this.dgvPendientes_CurrentCellChanged);
            // 
            // penVentaGarantiaID
            // 
            this.penVentaGarantiaID.HeaderText = "VentaGarantiaID";
            this.penVentaGarantiaID.Name = "penVentaGarantiaID";
            this.penVentaGarantiaID.ReadOnly = true;
            this.penVentaGarantiaID.Visible = false;
            // 
            // penEstatusGenericoID
            // 
            this.penEstatusGenericoID.HeaderText = "EstatusGenericoID";
            this.penEstatusGenericoID.Name = "penEstatusGenericoID";
            this.penEstatusGenericoID.ReadOnly = true;
            this.penEstatusGenericoID.Visible = false;
            // 
            // penNumeroDeParte
            // 
            this.penNumeroDeParte.HeaderText = "No. Parte";
            this.penNumeroDeParte.Name = "penNumeroDeParte";
            this.penNumeroDeParte.ReadOnly = true;
            // 
            // penNombreParte
            // 
            this.penNombreParte.HeaderText = "Descripción";
            this.penNombreParte.Name = "penNombreParte";
            this.penNombreParte.ReadOnly = true;
            this.penNombreParte.Width = 200;
            // 
            // penProveedor
            // 
            this.penProveedor.HeaderText = "Proveedor";
            this.penProveedor.Name = "penProveedor";
            this.penProveedor.ReadOnly = true;
            this.penProveedor.Width = 80;
            // 
            // penFolioDeVenta
            // 
            this.penFolioDeVenta.HeaderText = "Venta";
            this.penFolioDeVenta.Name = "penFolioDeVenta";
            this.penFolioDeVenta.ReadOnly = true;
            this.penFolioDeVenta.Width = 60;
            // 
            // penSucursal
            // 
            this.penSucursal.HeaderText = "Sucursal";
            this.penSucursal.Name = "penSucursal";
            this.penSucursal.ReadOnly = true;
            this.penSucursal.Width = 80;
            // 
            // penFecha
            // 
            this.penFecha.HeaderText = "Fecha";
            this.penFecha.Name = "penFecha";
            this.penFecha.ReadOnly = true;
            this.penFecha.Width = 80;
            // 
            // penMotivo
            // 
            this.penMotivo.HeaderText = "Motivo";
            this.penMotivo.Name = "penMotivo";
            this.penMotivo.ReadOnly = true;
            this.penMotivo.Width = 80;
            // 
            // penMotivoObservacion
            // 
            this.penMotivoObservacion.HeaderText = "Observación";
            this.penMotivoObservacion.Name = "penMotivoObservacion";
            this.penMotivoObservacion.ReadOnly = true;
            this.penMotivoObservacion.Width = 200;
            // 
            // penEstatus
            // 
            this.penEstatus.HeaderText = "Estatus";
            this.penEstatus.Name = "penEstatus";
            this.penEstatus.ReadOnly = true;
            this.penEstatus.Width = 80;
            // 
            // penAccion
            // 
            this.penAccion.HeaderText = "Acción";
            this.penAccion.Name = "penAccion";
            this.penAccion.ReadOnly = true;
            this.penAccion.Width = 80;
            // 
            // penObservacionCompletado
            // 
            this.penObservacionCompletado.HeaderText = "Obs. Proveedor";
            this.penObservacionCompletado.Name = "penObservacionCompletado";
            this.penObservacionCompletado.ReadOnly = true;
            this.penObservacionCompletado.Width = 200;
            // 
            // BusquedaGarantia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabGarantia);
            this.Name = "BusquedaGarantia";
            this.Size = new System.Drawing.Size(840, 380);
            this.Load += new System.EventHandler(this.BusquedaGarantia_Load);
            this.Controls.SetChildIndex(this.tabGarantia, 0);
            this.Controls.SetChildIndex(this.rdbTicket, 0);
            this.Controls.SetChildIndex(this.rdbFactura, 0);
            this.Controls.SetChildIndex(this.dtpInicio, 0);
            this.Controls.SetChildIndex(this.dtpFin, 0);
            this.Controls.SetChildIndex(this.txtFolio, 0);
            this.Controls.SetChildIndex(this.cmbSucursal, 0);
            this.Controls.SetChildIndex(this.pnlDatosDePago, 0);
            this.Controls.SetChildIndex(this.chkMostrarTodasLasVentas, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).EndInit();
            this.tabGarantia.ResumeLayout(false);
            this.tbpGarantia.ResumeLayout(false);
            this.tbpGarantia.PerformLayout();
            this.gpbMotivo.ResumeLayout(false);
            this.gpbMotivo.PerformLayout();
            this.gpbAccion.ResumeLayout(false);
            this.gpbAccion.PerformLayout();
            this.tbpPendientes.ResumeLayout(false);
            this.tbpPendientes.PerformLayout();
            this.gpbAccionPosterior.ResumeLayout(false);
            this.gpbAccionPosterior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendientes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabGarantia;
        private System.Windows.Forms.TabPage tbpGarantia;
        private System.Windows.Forms.TabPage tbpPendientes;
        private System.Windows.Forms.GroupBox gpbMotivo;
        private Negocio.TextoMod txtMotivoNota;
        private System.Windows.Forms.RadioButton rdbMotivo3;
        private System.Windows.Forms.RadioButton rdbMotivo4;
        private System.Windows.Forms.RadioButton rdbMotivo2;
        private System.Windows.Forms.RadioButton rdbMotivo1;
        private System.Windows.Forms.GroupBox gpbAccion;
        private System.Windows.Forms.RadioButton rdbAcCheque;
        private System.Windows.Forms.RadioButton rdbAcEfectivo;
        private System.Windows.Forms.RadioButton rdbAcNotaDeCredito;
        private System.Windows.Forms.RadioButton rdbAcArticuloNuevo;
        protected Negocio.TextoMod txtReimpresion;
        private Negocio.TextoMod txtAccionObservacion;
        protected System.Windows.Forms.DataGridView dgvPendientes;
        private System.Windows.Forms.RadioButton rdbAcRevision;
        private System.Windows.Forms.RadioButton rdbAcTarjeta;
        private System.Windows.Forms.GroupBox gpbAccionPosterior;
        private System.Windows.Forms.RadioButton rdbApNoProcede;
        private System.Windows.Forms.RadioButton rdbApTarjeta;
        private System.Windows.Forms.RadioButton rdbApCheque;
        private System.Windows.Forms.RadioButton rdbApEfectivo;
        private System.Windows.Forms.RadioButton rdbApNotaDeCredito;
        private System.Windows.Forms.RadioButton rdbApArticuloNuevo;
        private System.Windows.Forms.DateTimePicker dtpPenHasta;
        private System.Windows.Forms.DateTimePicker dtpPenDesde;
        private System.Windows.Forms.CheckBox chkPenMostrarTodas;
        private System.Windows.Forms.RadioButton rdbAcTransferencia;
        private System.Windows.Forms.RadioButton rdbApTransferencia;
        private System.Windows.Forms.DataGridViewTextBoxColumn penVentaGarantiaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn penEstatusGenericoID;
        private System.Windows.Forms.DataGridViewTextBoxColumn penNumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn penNombreParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn penProveedor;
        private System.Windows.Forms.DataGridViewTextBoxColumn penFolioDeVenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn penSucursal;
        private System.Windows.Forms.DataGridViewTextBoxColumn penFecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn penMotivo;
        private System.Windows.Forms.DataGridViewTextBoxColumn penMotivoObservacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn penEstatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn penAccion;
        private System.Windows.Forms.DataGridViewTextBoxColumn penObservacionCompletado;
    }
}
