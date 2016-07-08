namespace Refaccionaria.App
{
    partial class CajaResguardo
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
            this.lblRestante = new System.Windows.Forms.Label();
            this.lblEtRestante = new System.Windows.Forms.Label();
            this.lblImporteIdeal = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblRestante
            // 
            this.lblRestante.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRestante.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblRestante.Location = new System.Drawing.Point(535, 212);
            this.lblRestante.Name = "lblRestante";
            this.lblRestante.Size = new System.Drawing.Size(100, 20);
            this.lblRestante.TabIndex = 54;
            this.lblRestante.Text = "0.00";
            this.lblRestante.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblRestante.Visible = false;
            // 
            // lblEtRestante
            // 
            this.lblEtRestante.AutoSize = true;
            this.lblEtRestante.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtRestante.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblEtRestante.Location = new System.Drawing.Point(431, 212);
            this.lblEtRestante.Name = "lblEtRestante";
            this.lblEtRestante.Size = new System.Drawing.Size(98, 20);
            this.lblEtRestante.TabIndex = 53;
            this.lblEtRestante.Text = "Restante $";
            this.lblEtRestante.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblEtRestante.Visible = false;
            // 
            // lblImporteIdeal
            // 
            this.lblImporteIdeal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImporteIdeal.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblImporteIdeal.Location = new System.Drawing.Point(535, 192);
            this.lblImporteIdeal.Name = "lblImporteIdeal";
            this.lblImporteIdeal.Size = new System.Drawing.Size(100, 20);
            this.lblImporteIdeal.TabIndex = 56;
            this.lblImporteIdeal.Text = "0.00";
            this.lblImporteIdeal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblImporteIdeal.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.SteelBlue;
            this.label4.Location = new System.Drawing.Point(400, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 20);
            this.label4.TabIndex = 55;
            this.label4.Text = "Importe ideal $";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label4.Visible = false;
            // 
            // CajaResguardo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblImporteIdeal);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblRestante);
            this.Controls.Add(this.lblEtRestante);
            this.Name = "CajaResguardo";
            this.Load += new System.EventHandler(this.CajaResguardo_Load);
            this.Controls.SetChildIndex(this.label279, 0);
            this.Controls.SetChildIndex(this.label278, 0);
            this.Controls.SetChildIndex(this.label277, 0);
            this.Controls.SetChildIndex(this.label276, 0);
            this.Controls.SetChildIndex(this.lblTarjetas, 0);
            this.Controls.SetChildIndex(this.lblCheques, 0);
            this.Controls.SetChildIndex(this.lblTransferencias, 0);
            this.Controls.SetChildIndex(this.lblEfectivo, 0);
            this.Controls.SetChildIndex(this.lblTotal, 0);
            this.Controls.SetChildIndex(this.lblEtTotal, 0);
            this.Controls.SetChildIndex(this.pnlMonedas, 0);
            this.Controls.SetChildIndex(this.lblEtRestante, 0);
            this.Controls.SetChildIndex(this.lblRestante, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.lblImporteIdeal, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRestante;
        private System.Windows.Forms.Label lblEtRestante;
        private System.Windows.Forms.Label lblImporteIdeal;
        private System.Windows.Forms.Label label4;
    }
}
