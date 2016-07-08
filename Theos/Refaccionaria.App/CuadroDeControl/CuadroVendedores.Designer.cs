namespace Refaccionaria.App
{
    partial class CuadroVendedores
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
            this.chkGrafSemanas = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chrMeses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrSemanas)).BeginInit();
            this.SuspendLayout();
            // 
            // chrMeses
            // 
            this.chrMeses.Location = new System.Drawing.Point(529, 624);
            this.chrMeses.Size = new System.Drawing.Size(400, 149);
            // 
            // chrSemanas
            // 
            this.chrSemanas.Location = new System.Drawing.Point(0, 624);
            this.chrSemanas.Size = new System.Drawing.Size(520, 149);
            // 
            // label6
            // 
            this.label6.Text = "Utilidad por Vendedor";
            // 
            // chkGrafSemanas
            // 
            this.chkGrafSemanas.AutoSize = true;
            this.chkGrafSemanas.ForeColor = System.Drawing.Color.White;
            this.chkGrafSemanas.Location = new System.Drawing.Point(3, 718);
            this.chkGrafSemanas.Name = "chkGrafSemanas";
            this.chkGrafSemanas.Size = new System.Drawing.Size(56, 17);
            this.chkGrafSemanas.TabIndex = 15;
            this.chkGrafSemanas.Text = "Todos";
            this.chkGrafSemanas.UseVisualStyleBackColor = true;
            this.chkGrafSemanas.CheckedChanged += new System.EventHandler(this.chkGrafSemanas_CheckedChanged);
            // 
            // CuadroVendedores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.chkGrafSemanas);
            this.Name = "CuadroVendedores";
            this.Load += new System.EventHandler(this.CuadroVendedores_Load);
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
            this.Controls.SetChildIndex(this.chkGrafSemanas, 0);
            ((System.ComponentModel.ISupportInitialize)(this.chrMeses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrSemanas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkGrafSemanas;
    }
}
