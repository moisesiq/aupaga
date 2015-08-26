namespace Refaccionaria.App
{
    partial class BusquedaReimpresion
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
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).BeginInit();
            this.SuspendLayout();
            // 
            // txtObservaciones
            // 
            this.txtObservaciones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObservaciones.Location = new System.Drawing.Point(6, 317);
            this.txtObservaciones.Multiline = true;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.ReadOnly = true;
            this.txtObservaciones.Size = new System.Drawing.Size(579, 55);
            this.txtObservaciones.TabIndex = 20;
            // 
            // BusquedaReimpresion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtObservaciones);
            this.Name = "BusquedaReimpresion";
            this.Size = new System.Drawing.Size(592, 380);
            this.Controls.SetChildIndex(this.txtObservaciones, 0);
            this.Controls.SetChildIndex(this.rdbTicket, 0);
            this.Controls.SetChildIndex(this.rdbFactura, 0);
            this.Controls.SetChildIndex(this.dtpInicio, 0);
            this.Controls.SetChildIndex(this.dtpFin, 0);
            this.Controls.SetChildIndex(this.txtFolio, 0);
            this.Controls.SetChildIndex(this.cmbSucursal, 0);
            this.Controls.SetChildIndex(this.pnlDatosDePago, 0);
            this.Controls.SetChildIndex(this.chkMostrarTodasLasVentas, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ctlError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtObservaciones;

    }
}
