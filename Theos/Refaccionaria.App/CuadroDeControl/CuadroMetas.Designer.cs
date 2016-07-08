namespace Refaccionaria.App
{
    partial class CuadroMetas
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
            this.cmbSucursal = new System.Windows.Forms.ComboBox();
            this.cmbVendedor = new LibUtil.ComboEtiqueta();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.metasMetas = new Refaccionaria.App.Metas();
            this.btnMostrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSucursal.FormattingEnabled = true;
            this.cmbSucursal.Location = new System.Drawing.Point(3, 3);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.Size = new System.Drawing.Size(160, 21);
            this.cmbSucursal.TabIndex = 0;
            this.cmbSucursal.SelectedIndexChanged += new System.EventHandler(this.cmbSucursal_SelectedIndexChanged);
            // 
            // cmbVendedor
            // 
            this.cmbVendedor.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbVendedor.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbVendedor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cmbVendedor.DataSource = null;
            this.cmbVendedor.DisplayMember = "";
            this.cmbVendedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbVendedor.Etiqueta = "Vendedor";
            this.cmbVendedor.EtiquetaColor = System.Drawing.Color.Gray;
            this.cmbVendedor.Location = new System.Drawing.Point(381, 3);
            this.cmbVendedor.Name = "cmbVendedor";
            this.cmbVendedor.SelectedIndex = -1;
            this.cmbVendedor.SelectedItem = null;
            this.cmbVendedor.SelectedText = "";
            this.cmbVendedor.SelectedValue = null;
            this.cmbVendedor.Size = new System.Drawing.Size(240, 21);
            this.cmbVendedor.TabIndex = 3;
            this.cmbVendedor.ValueMember = "";
            this.cmbVendedor.SelectedIndexChanged += new System.EventHandler(this.cmbVendedor_SelectedIndexChanged);
            // 
            // dtpHasta
            // 
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(275, 3);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta.TabIndex = 2;
            this.dtpHasta.ValueChanged += new System.EventHandler(this.dtpHasta_ValueChanged);
            // 
            // dtpDesde
            // 
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(169, 3);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde.TabIndex = 1;
            this.dtpDesde.ValueChanged += new System.EventHandler(this.dtpDesde_ValueChanged);
            // 
            // metasMetas
            // 
            this.metasMetas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.metasMetas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.metasMetas.bVerAdicional = false;
            this.metasMetas.Desde = new System.DateTime(((long)(0)));
            this.metasMetas.Hasta = new System.DateTime(((long)(0)));
            this.metasMetas.Location = new System.Drawing.Point(3, 30);
            this.metasMetas.MostrarMinimizar = false;
            this.metasMetas.Name = "metasMetas";
            this.metasMetas.Size = new System.Drawing.Size(714, 425);
            this.metasMetas.SucursalID = 0;
            this.metasMetas.TabIndex = 5;
            this.metasMetas.UsuarioID = 0;
            // 
            // btnMostrar
            // 
            this.btnMostrar.Location = new System.Drawing.Point(627, 2);
            this.btnMostrar.Name = "btnMostrar";
            this.btnMostrar.Size = new System.Drawing.Size(75, 23);
            this.btnMostrar.TabIndex = 4;
            this.btnMostrar.Text = "&Mostrar";
            this.btnMostrar.UseVisualStyleBackColor = true;
            this.btnMostrar.Click += new System.EventHandler(this.btnMostrar_Click);
            // 
            // CuadroMetas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMostrar);
            this.Controls.Add(this.metasMetas);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.cmbVendedor);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.dtpDesde);
            this.Name = "CuadroMetas";
            this.Size = new System.Drawing.Size(720, 458);
            this.Load += new System.EventHandler(this.CuadroMetas_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Metas metasMetas;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private LibUtil.ComboEtiqueta cmbVendedor;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Button btnMostrar;
    }
}
