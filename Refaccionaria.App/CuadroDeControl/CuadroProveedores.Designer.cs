namespace Refaccionaria.App
{
    partial class CuadroProveedores
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
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.chrMeses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrSemanas)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.Text = "Utilidad y Compras por Proveedor";
            // 
            // cmbTipo
            // 
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.FormattingEnabled = true;
            this.cmbTipo.Location = new System.Drawing.Point(966, 26);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(135, 21);
            this.cmbTipo.TabIndex = 6;
            this.cmbTipo.SelectedIndexChanged += new System.EventHandler(this.cmbTipo_SelectedIndexChanged);
            // 
            // CuadroProveedores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.cmbTipo);
            this.Name = "CuadroProveedores";
            this.Load += new System.EventHandler(this.CuadroProveedores_Load);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.chkOmitirDomingos, 0);
            this.Controls.SetChildIndex(this.chkCostoConDescuento, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.cmbSucursal, 0);
            this.Controls.SetChildIndex(this.chkPagadas, 0);
            this.Controls.SetChildIndex(this.chkCobradas, 0);
            this.Controls.SetChildIndex(this.dtpDesde, 0);
            this.Controls.SetChildIndex(this.dtpHasta, 0);
            this.Controls.SetChildIndex(this.chk9500, 0);
            this.Controls.SetChildIndex(this.chrSemanas, 0);
            this.Controls.SetChildIndex(this.chrMeses, 0);
            this.Controls.SetChildIndex(this.cmbTipo, 0);
            ((System.ComponentModel.ISupportInitialize)(this.chrMeses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrSemanas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTipo;
    }
}
