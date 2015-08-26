namespace Refaccionaria.App
{
    partial class DetalleLinea
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtNombreLinea = new System.Windows.Forms.TextBox();
            this.lblNombreLinea = new System.Windows.Forms.Label();
            this.txtAbreviacion = new System.Windows.Forms.TextBox();
            this.lblAbreviacion = new System.Windows.Forms.Label();
            this.cboSubsistema = new System.Windows.Forms.ComboBox();
            this.lblSistema = new System.Windows.Forms.Label();
            this.txtMachote = new System.Windows.Forms.TextBox();
            this.lblMachote = new System.Windows.Forms.Label();
            this.gpoCaracteristicas = new System.Windows.Forms.GroupBox();
            this.dgvCaracteristicas = new Refaccionaria.Negocio.GridEditable();
            this.colCaracteristicaIdAnt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCaracteristicaID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colMultipleOpciones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.gpoGen.SuspendLayout();
            this.gpoCaracteristicas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCaracteristicas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.button2);
            this.gpoGen.Controls.Add(this.picLogo);
            this.gpoGen.Controls.Add(this.button1);
            this.gpoGen.Controls.Add(this.gpoCaracteristicas);
            this.gpoGen.Controls.Add(this.txtMachote);
            this.gpoGen.Controls.Add(this.lblMachote);
            this.gpoGen.Controls.Add(this.cboSubsistema);
            this.gpoGen.Controls.Add(this.lblSistema);
            this.gpoGen.Controls.Add(this.txtAbreviacion);
            this.gpoGen.Controls.Add(this.lblAbreviacion);
            this.gpoGen.Controls.Add(this.txtNombreLinea);
            this.gpoGen.Controls.Add(this.lblNombreLinea);
            this.gpoGen.ForeColor = System.Drawing.Color.White;
            this.gpoGen.Size = new System.Drawing.Size(720, 476);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(650, 494);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(562, 494);
            this.btnGuardar.TabIndex = 1;
            // 
            // txtNombreLinea
            // 
            this.txtNombreLinea.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreLinea.Location = new System.Drawing.Point(89, 13);
            this.txtNombreLinea.MaxLength = 100;
            this.txtNombreLinea.Name = "txtNombreLinea";
            this.txtNombreLinea.Size = new System.Drawing.Size(225, 20);
            this.txtNombreLinea.TabIndex = 0;
            // 
            // lblNombreLinea
            // 
            this.lblNombreLinea.AutoSize = true;
            this.lblNombreLinea.Location = new System.Drawing.Point(6, 16);
            this.lblNombreLinea.Name = "lblNombreLinea";
            this.lblNombreLinea.Size = new System.Drawing.Size(73, 13);
            this.lblNombreLinea.TabIndex = 4;
            this.lblNombreLinea.Text = "Nombre Linea";
            // 
            // txtAbreviacion
            // 
            this.txtAbreviacion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAbreviacion.Location = new System.Drawing.Point(89, 39);
            this.txtAbreviacion.MaxLength = 100;
            this.txtAbreviacion.Name = "txtAbreviacion";
            this.txtAbreviacion.Size = new System.Drawing.Size(225, 20);
            this.txtAbreviacion.TabIndex = 1;
            // 
            // lblAbreviacion
            // 
            this.lblAbreviacion.AutoSize = true;
            this.lblAbreviacion.Location = new System.Drawing.Point(6, 42);
            this.lblAbreviacion.Name = "lblAbreviacion";
            this.lblAbreviacion.Size = new System.Drawing.Size(63, 13);
            this.lblAbreviacion.TabIndex = 6;
            this.lblAbreviacion.Text = "Abreviación";
            // 
            // cboSubsistema
            // 
            this.cboSubsistema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubsistema.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboSubsistema.FormattingEnabled = true;
            this.cboSubsistema.Location = new System.Drawing.Point(89, 65);
            this.cboSubsistema.Name = "cboSubsistema";
            this.cboSubsistema.Size = new System.Drawing.Size(225, 21);
            this.cboSubsistema.TabIndex = 2;
            // 
            // lblSistema
            // 
            this.lblSistema.AutoSize = true;
            this.lblSistema.Location = new System.Drawing.Point(6, 68);
            this.lblSistema.Name = "lblSistema";
            this.lblSistema.Size = new System.Drawing.Size(61, 13);
            this.lblSistema.TabIndex = 8;
            this.lblSistema.Text = "Subsistema";
            // 
            // txtMachote
            // 
            this.txtMachote.Location = new System.Drawing.Point(89, 92);
            this.txtMachote.MaxLength = 255;
            this.txtMachote.Name = "txtMachote";
            this.txtMachote.Size = new System.Drawing.Size(625, 20);
            this.txtMachote.TabIndex = 3;
            // 
            // lblMachote
            // 
            this.lblMachote.AutoSize = true;
            this.lblMachote.Location = new System.Drawing.Point(6, 95);
            this.lblMachote.Name = "lblMachote";
            this.lblMachote.Size = new System.Drawing.Size(49, 13);
            this.lblMachote.TabIndex = 10;
            this.lblMachote.Text = "Machote";
            // 
            // gpoCaracteristicas
            // 
            this.gpoCaracteristicas.Controls.Add(this.dgvCaracteristicas);
            this.gpoCaracteristicas.ForeColor = System.Drawing.Color.White;
            this.gpoCaracteristicas.Location = new System.Drawing.Point(9, 118);
            this.gpoCaracteristicas.Name = "gpoCaracteristicas";
            this.gpoCaracteristicas.Size = new System.Drawing.Size(705, 346);
            this.gpoCaracteristicas.TabIndex = 4;
            this.gpoCaracteristicas.TabStop = false;
            this.gpoCaracteristicas.Text = "Características";
            // 
            // dgvCaracteristicas
            // 
            this.dgvCaracteristicas.AllowUserToDeleteRows = false;
            this.dgvCaracteristicas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCaracteristicas.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvCaracteristicas.ColorBorrado = System.Drawing.Color.Gray;
            this.dgvCaracteristicas.ColorModificado = System.Drawing.Color.Orange;
            this.dgvCaracteristicas.ColorNuevo = System.Drawing.Color.Blue;
            this.dgvCaracteristicas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCaracteristicas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCaracteristicaIdAnt,
            this.colCaracteristicaID,
            this.colMultipleOpciones});
            this.dgvCaracteristicas.Location = new System.Drawing.Point(6, 15);
            this.dgvCaracteristicas.MultiSelect = false;
            this.dgvCaracteristicas.Name = "dgvCaracteristicas";
            this.dgvCaracteristicas.PermitirBorrar = true;
            this.dgvCaracteristicas.Size = new System.Drawing.Size(693, 325);
            this.dgvCaracteristicas.TabIndex = 2;
            this.dgvCaracteristicas.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCaracteristicas_CellValueChanged);
            this.dgvCaracteristicas.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvCaracteristicas_CurrentCellDirtyStateChanged);
            // 
            // colCaracteristicaIdAnt
            // 
            this.colCaracteristicaIdAnt.HeaderText = "Id Car.";
            this.colCaracteristicaIdAnt.Name = "colCaracteristicaIdAnt";
            this.colCaracteristicaIdAnt.ReadOnly = true;
            this.colCaracteristicaIdAnt.Visible = false;
            // 
            // colCaracteristicaID
            // 
            this.colCaracteristicaID.HeaderText = "Característica";
            this.colCaracteristicaID.Name = "colCaracteristicaID";
            this.colCaracteristicaID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCaracteristicaID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCaracteristicaID.Width = 120;
            // 
            // colMultipleOpciones
            // 
            this.colMultipleOpciones.HeaderText = "Opciones";
            this.colMultipleOpciones.Name = "colMultipleOpciones";
            this.colMultipleOpciones.Width = 240;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(463, 63);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Cargar Imagen";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(361, 9);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(96, 77);
            this.picLogo.TabIndex = 12;
            this.picLogo.TabStop = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(566, 63);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "Agregar Archivo";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DetalleLinea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(744, 525);
            this.KeyPreview = true;
            this.Name = "DetalleLinea";
            this.Load += new System.EventHandler(this.DetalleLinea_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleLinea_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.gpoCaracteristicas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCaracteristicas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreLinea;
        private System.Windows.Forms.Label lblNombreLinea;
        private System.Windows.Forms.TextBox txtAbreviacion;
        private System.Windows.Forms.Label lblAbreviacion;
        private System.Windows.Forms.TextBox txtMachote;
        private System.Windows.Forms.Label lblMachote;
        private System.Windows.Forms.ComboBox cboSubsistema;
        private System.Windows.Forms.Label lblSistema;
        private System.Windows.Forms.GroupBox gpoCaracteristicas;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button button2;
        protected Negocio.GridEditable dgvCaracteristicas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCaracteristicaIdAnt;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCaracteristicaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMultipleOpciones;
    }
}
