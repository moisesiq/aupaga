namespace Refaccionaria.App
{
    partial class AplicarPartesAVehiculo
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVehiculo = new System.Windows.Forms.Label();
            this.dgvPartes = new System.Windows.Forms.DataGridView();
            this.Aplicar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ParteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroDeParte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAplicar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selecciona las partes que deseas aplicar al vehículo:";
            // 
            // lblVehiculo
            // 
            this.lblVehiculo.AutoSize = true;
            this.lblVehiculo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVehiculo.ForeColor = System.Drawing.Color.White;
            this.lblVehiculo.Location = new System.Drawing.Point(12, 28);
            this.lblVehiculo.Name = "lblVehiculo";
            this.lblVehiculo.Size = new System.Drawing.Size(58, 13);
            this.lblVehiculo.TabIndex = 1;
            this.lblVehiculo.Text = "Vehículo";
            // 
            // dgvPartes
            // 
            this.dgvPartes.AllowUserToAddRows = false;
            this.dgvPartes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPartes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPartes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
            this.dgvPartes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dgvPartes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPartes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Aplicar,
            this.ParteID,
            this.NumeroDeParte,
            this.Descripcion});
            this.dgvPartes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvPartes.Location = new System.Drawing.Point(12, 48);
            this.dgvPartes.Name = "dgvPartes";
            this.dgvPartes.RowHeadersVisible = false;
            this.dgvPartes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPartes.Size = new System.Drawing.Size(361, 160);
            this.dgvPartes.TabIndex = 0;
            this.dgvPartes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvPartes_KeyDown);
            // 
            // Aplicar
            // 
            this.Aplicar.HeaderText = "Aplicar";
            this.Aplicar.Name = "Aplicar";
            this.Aplicar.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Aplicar.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Aplicar.Width = 50;
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
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Descripcion.DefaultCellStyle = dataGridViewCellStyle1;
            this.Descripcion.HeaderText = "Descripción";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            this.Descripcion.Width = 210;
            // 
            // btnAplicar
            // 
            this.btnAplicar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAplicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicar.ForeColor = System.Drawing.Color.White;
            this.btnAplicar.Location = new System.Drawing.Point(203, 214);
            this.btnAplicar.Name = "btnAplicar";
            this.btnAplicar.Size = new System.Drawing.Size(82, 23);
            this.btnAplicar.TabIndex = 1;
            this.btnAplicar.Text = "&Aplicar";
            this.btnAplicar.UseVisualStyleBackColor = false;
            this.btnAplicar.Click += new System.EventHandler(this.btnAplicar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(291, 214);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(82, 23);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // AplicarPartesAVehiculo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(385, 245);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAplicar);
            this.Controls.Add(this.dgvPartes);
            this.Controls.Add(this.lblVehiculo);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AplicarPartesAVehiculo";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Aplicar a Vehículo";
            this.Load += new System.EventHandler(this.AplicarPartesAVehiculo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVehiculo;
        private System.Windows.Forms.DataGridView dgvPartes;
        private System.Windows.Forms.Button btnAplicar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Aplicar;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroDeParte;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
    }
}