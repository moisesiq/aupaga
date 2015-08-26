namespace Refaccionaria.Negocio
{
    partial class Progreso
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
            this.pgbProgreso = new System.Windows.Forms.ProgressBar();
            this.lblProgreso = new System.Windows.Forms.Label();
            this.lblTextoDe = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pgbProgreso
            // 
            this.pgbProgreso.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbProgreso.Location = new System.Drawing.Point(0, 0);
            this.pgbProgreso.Name = "pgbProgreso";
            this.pgbProgreso.Size = new System.Drawing.Size(213, 20);
            this.pgbProgreso.TabIndex = 0;
            this.pgbProgreso.Value = 32;
            // 
            // lblProgreso
            // 
            this.lblProgreso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgreso.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgreso.Location = new System.Drawing.Point(219, 2);
            this.lblProgreso.Name = "lblProgreso";
            this.lblProgreso.Size = new System.Drawing.Size(72, 15);
            this.lblProgreso.TabIndex = 1;
            this.lblProgreso.Text = "9,999,999";
            this.lblProgreso.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTextoDe
            // 
            this.lblTextoDe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextoDe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTextoDe.Location = new System.Drawing.Point(297, 2);
            this.lblTextoDe.Name = "lblTextoDe";
            this.lblTextoDe.Size = new System.Drawing.Size(23, 15);
            this.lblTextoDe.TabIndex = 2;
            this.lblTextoDe.Text = "de";
            this.lblTextoDe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(326, 2);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(72, 15);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "9,999,999";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Progreso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblTextoDe);
            this.Controls.Add(this.lblProgreso);
            this.Controls.Add(this.pgbProgreso);
            this.Name = "Progreso";
            this.Size = new System.Drawing.Size(400, 20);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ProgressBar pgbProgreso;
        public System.Windows.Forms.Label lblProgreso;
        public System.Windows.Forms.Label lblTextoDe;
        public System.Windows.Forms.Label lblTotal;



    }
}
