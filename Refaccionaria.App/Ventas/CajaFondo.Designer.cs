namespace Refaccionaria.App
{
    partial class CajaFondo
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
            this.lblEtDiferencia = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblConteo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFondoDeCaja = new System.Windows.Forms.Label();
            this.lblDiferencia = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblEtDiferencia
            // 
            this.lblEtDiferencia.AutoSize = true;
            this.lblEtDiferencia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtDiferencia.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblEtDiferencia.Location = new System.Drawing.Point(420, 305);
            this.lblEtDiferencia.Name = "lblEtDiferencia";
            this.lblEtDiferencia.Size = new System.Drawing.Size(106, 20);
            this.lblEtDiferencia.TabIndex = 70;
            this.lblEtDiferencia.Text = "Diferencia $";
            this.lblEtDiferencia.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(399, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 20);
            this.label3.TabIndex = 71;
            this.label3.Text = "Total Conteo $";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblConteo
            // 
            this.lblConteo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConteo.ForeColor = System.Drawing.Color.White;
            this.lblConteo.Location = new System.Drawing.Point(532, 253);
            this.lblConteo.Name = "lblConteo";
            this.lblConteo.Size = new System.Drawing.Size(100, 20);
            this.lblConteo.TabIndex = 79;
            this.lblConteo.Text = "0.00";
            this.lblConteo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label2.Location = new System.Drawing.Point(385, 279);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 20);
            this.label2.TabIndex = 69;
            this.label2.Text = "Fondo de Caja $";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblFondoDeCaja
            // 
            this.lblFondoDeCaja.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFondoDeCaja.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblFondoDeCaja.Location = new System.Drawing.Point(532, 279);
            this.lblFondoDeCaja.Name = "lblFondoDeCaja";
            this.lblFondoDeCaja.Size = new System.Drawing.Size(100, 20);
            this.lblFondoDeCaja.TabIndex = 80;
            this.lblFondoDeCaja.Text = "0.00";
            this.lblFondoDeCaja.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDiferencia
            // 
            this.lblDiferencia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiferencia.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblDiferencia.Location = new System.Drawing.Point(532, 305);
            this.lblDiferencia.Name = "lblDiferencia";
            this.lblDiferencia.Size = new System.Drawing.Size(100, 20);
            this.lblDiferencia.TabIndex = 81;
            this.lblDiferencia.Text = "0.00";
            this.lblDiferencia.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // CajaFondo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDiferencia);
            this.Controls.Add(this.lblFondoDeCaja);
            this.Controls.Add(this.lblConteo);
            this.Controls.Add(this.lblEtDiferencia);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.MostrarTotal = true;
            this.Name = "CajaFondo";
            this.Load += new System.EventHandler(this.CajaFondo_Load);
            this.Controls.SetChildIndex(this.lblEtTotal, 0);
            this.Controls.SetChildIndex(this.lblTotal, 0);
            this.Controls.SetChildIndex(this.pnlMonedas, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.lblEtDiferencia, 0);
            this.Controls.SetChildIndex(this.lblConteo, 0);
            this.Controls.SetChildIndex(this.lblFondoDeCaja, 0);
            this.Controls.SetChildIndex(this.lblDiferencia, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEtDiferencia;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblConteo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFondoDeCaja;
        private System.Windows.Forms.Label lblDiferencia;
    }
}
