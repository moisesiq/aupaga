namespace Refaccionaria.App.Administracion.Formas
{
    partial class ImprimirEtiquetas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImprimirEtiquetas));
            this.cmbLinea = new LibUtil.ComboEtiqueta();
            this.cmbSucursales = new LibUtil.ComboEtiqueta();
            this.dgvExistencias = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.lblNombreParte = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExistencias)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbLinea
            // 
            this.cmbLinea.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbLinea.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbLinea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.cmbLinea.DataSource = null;
            this.cmbLinea.DisplayMember = "";
            this.cmbLinea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbLinea.Etiqueta = "Línea";
            this.cmbLinea.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbLinea.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLinea.Location = new System.Drawing.Point(24, 42);
            this.cmbLinea.Name = "cmbLinea";
            this.cmbLinea.SelectedIndex = -1;
            this.cmbLinea.SelectedItem = null;
            this.cmbLinea.SelectedText = "";
            this.cmbLinea.SelectedValue = null;
            this.cmbLinea.Size = new System.Drawing.Size(187, 23);
            this.cmbLinea.TabIndex = 4;
            this.cmbLinea.ValueMember = "";
            // 
            // cmbSucursales
            // 
            this.cmbSucursales.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSucursales.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSucursales.AutoSize = true;
            this.cmbSucursales.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.cmbSucursales.DataSource = null;
            this.cmbSucursales.DisplayMember = "";
            this.cmbSucursales.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbSucursales.Etiqueta = "Sucursal";
            this.cmbSucursales.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbSucursales.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSucursales.Location = new System.Drawing.Point(227, 42);
            this.cmbSucursales.Name = "cmbSucursales";
            this.cmbSucursales.SelectedIndex = -1;
            this.cmbSucursales.SelectedItem = null;
            this.cmbSucursales.SelectedText = "";
            this.cmbSucursales.SelectedValue = null;
            this.cmbSucursales.Size = new System.Drawing.Size(187, 23);
            this.cmbSucursales.TabIndex = 5;
            this.cmbSucursales.ValueMember = "";
            // 
            // dgvExistencias
            // 
            this.dgvExistencias.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.dgvExistencias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExistencias.Location = new System.Drawing.Point(24, 88);
            this.dgvExistencias.Name = "dgvExistencias";
            this.dgvExistencias.Size = new System.Drawing.Size(544, 348);
            this.dgvExistencias.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.Snow;
            this.button1.Location = new System.Drawing.Point(486, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Mostrar";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblNombreParte
            // 
            this.lblNombreParte.AutoSize = true;
            this.lblNombreParte.BackColor = System.Drawing.Color.Transparent;
            this.lblNombreParte.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreParte.ForeColor = System.Drawing.Color.White;
            this.lblNombreParte.Location = new System.Drawing.Point(21, 26);
            this.lblNombreParte.Name = "lblNombreParte";
            this.lblNombreParte.Size = new System.Drawing.Size(33, 13);
            this.lblNombreParte.TabIndex = 147;
            this.lblNombreParte.Text = "Linea";
            this.lblNombreParte.Click += new System.EventHandler(this.lblNombreParte_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(224, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 148;
            this.label1.Text = "Sucursales";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.Snow;
            this.button2.Location = new System.Drawing.Point(486, 458);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 23);
            this.button2.TabIndex = 149;
            this.button2.Text = "Imprimir";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ImprimirEtiquetas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.ClientSize = new System.Drawing.Size(597, 493);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblNombreParte);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgvExistencias);
            this.Controls.Add(this.cmbSucursales);
            this.Controls.Add(this.cmbLinea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImprimirEtiquetas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Imprimir Etiquetas";
            this.Load += new System.EventHandler(this.ImprimirEtiquetas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvExistencias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LibUtil.ComboEtiqueta cmbLinea;
        private LibUtil.ComboEtiqueta cmbSucursales;
        private System.Windows.Forms.DataGridView dgvExistencias;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblNombreParte;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
    }
}