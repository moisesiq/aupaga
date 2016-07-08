namespace Refaccionaria.App
{
    partial class CatMetas
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvEspecificas = new System.Windows.Forms.DataGridView();
            this.mesMetaID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mesCambio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mesParteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mesUsuarioID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.mesMarcaID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.mesLineaID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.mesNumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mesNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mesCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mesRutaImagen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSucursal = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEspecificas)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvEspecificas);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(3, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(634, 391);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Metas Específicas";
            // 
            // dgvEspecificas
            // 
            this.dgvEspecificas.AllowDrop = true;
            this.dgvEspecificas.AllowUserToDeleteRows = false;
            this.dgvEspecificas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEspecificas.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvEspecificas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEspecificas.CausesValidation = false;
            this.dgvEspecificas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvEspecificas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEspecificas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mesMetaID,
            this.mesCambio,
            this.mesParteID,
            this.mesUsuarioID,
            this.mesMarcaID,
            this.mesLineaID,
            this.mesNumeroDeParte,
            this.mesNombre,
            this.mesCantidad,
            this.mesRutaImagen});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvEspecificas.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvEspecificas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvEspecificas.Location = new System.Drawing.Point(6, 19);
            this.dgvEspecificas.MultiSelect = false;
            this.dgvEspecificas.Name = "dgvEspecificas";
            this.dgvEspecificas.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEspecificas.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvEspecificas.RowHeadersWidth = 40;
            this.dgvEspecificas.Size = new System.Drawing.Size(622, 366);
            this.dgvEspecificas.TabIndex = 1;
            this.dgvEspecificas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEspecificas_CellDoubleClick);
            this.dgvEspecificas.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEspecificas_CellValueChanged);
            this.dgvEspecificas.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvEspecificas_RowsAdded);
            this.dgvEspecificas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvEspecificas_KeyDown);
            // 
            // mesMetaID
            // 
            this.mesMetaID.HeaderText = "MetaID";
            this.mesMetaID.Name = "mesMetaID";
            this.mesMetaID.ReadOnly = true;
            this.mesMetaID.Visible = false;
            // 
            // mesCambio
            // 
            this.mesCambio.HeaderText = "Cambio";
            this.mesCambio.Name = "mesCambio";
            this.mesCambio.ReadOnly = true;
            this.mesCambio.Visible = false;
            // 
            // mesParteID
            // 
            this.mesParteID.HeaderText = "ParteID";
            this.mesParteID.Name = "mesParteID";
            this.mesParteID.ReadOnly = true;
            this.mesParteID.Visible = false;
            // 
            // mesUsuarioID
            // 
            this.mesUsuarioID.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.mesUsuarioID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mesUsuarioID.HeaderText = "Usuario";
            this.mesUsuarioID.Name = "mesUsuarioID";
            this.mesUsuarioID.Width = 80;
            // 
            // mesMarcaID
            // 
            this.mesMarcaID.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.mesMarcaID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mesMarcaID.HeaderText = "Marca";
            this.mesMarcaID.Name = "mesMarcaID";
            this.mesMarcaID.Width = 140;
            // 
            // mesLineaID
            // 
            this.mesLineaID.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.mesLineaID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mesLineaID.HeaderText = "Línea";
            this.mesLineaID.Name = "mesLineaID";
            this.mesLineaID.Width = 140;
            // 
            // mesNumeroDeParte
            // 
            this.mesNumeroDeParte.HeaderText = "Artículo";
            this.mesNumeroDeParte.Name = "mesNumeroDeParte";
            this.mesNumeroDeParte.Width = 120;
            // 
            // mesNombre
            // 
            this.mesNombre.HeaderText = "Nombre";
            this.mesNombre.Name = "mesNombre";
            this.mesNombre.Width = 120;
            // 
            // mesCantidad
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N2";
            this.mesCantidad.DefaultCellStyle = dataGridViewCellStyle1;
            this.mesCantidad.HeaderText = "Cantidad";
            this.mesCantidad.Name = "mesCantidad";
            this.mesCantidad.Width = 60;
            // 
            // mesRutaImagen
            // 
            this.mesRutaImagen.HeaderText = "Imagen";
            this.mesRutaImagen.Name = "mesRutaImagen";
            this.mesRutaImagen.Width = 200;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(562, 427);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Sucursal";
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSucursal.FormattingEnabled = true;
            this.cmbSucursal.Location = new System.Drawing.Point(60, 3);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.Size = new System.Drawing.Size(140, 21);
            this.cmbSucursal.TabIndex = 0;
            this.cmbSucursal.SelectedIndexChanged += new System.EventHandler(this.cmbSucursal_SelectedIndexChanged);
            // 
            // CatMetas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGuardar);
            this.Name = "CatMetas";
            this.Size = new System.Drawing.Size(640, 453);
            this.Load += new System.EventHandler(this.MetasCat_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEspecificas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvEspecificas;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private System.Windows.Forms.DataGridViewTextBoxColumn mesMetaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn mesCambio;
        private System.Windows.Forms.DataGridViewTextBoxColumn mesParteID;
        private System.Windows.Forms.DataGridViewComboBoxColumn mesUsuarioID;
        private System.Windows.Forms.DataGridViewComboBoxColumn mesMarcaID;
        private System.Windows.Forms.DataGridViewComboBoxColumn mesLineaID;
        private System.Windows.Forms.DataGridViewTextBoxColumn mesNumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn mesNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn mesCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn mesRutaImagen;
    }
}
