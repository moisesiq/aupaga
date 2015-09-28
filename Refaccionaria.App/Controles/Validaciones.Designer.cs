namespace Refaccionaria.App
{
    partial class Validaciones
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
            this.tabValidaciones = new System.Windows.Forms.TabControl();
            this.tbpAutorizaciones = new System.Windows.Forms.TabPage();
            this.tbpCortes = new System.Windows.Forms.TabPage();
            this.tbpRecepcionCredito = new System.Windows.Forms.TabPage();
            this.tabValidaciones.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabValidaciones
            // 
            this.tabValidaciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabValidaciones.Controls.Add(this.tbpAutorizaciones);
            this.tabValidaciones.Controls.Add(this.tbpCortes);
            this.tabValidaciones.Controls.Add(this.tbpRecepcionCredito);
            this.tabValidaciones.Location = new System.Drawing.Point(3, 3);
            this.tabValidaciones.Name = "tabValidaciones";
            this.tabValidaciones.SelectedIndex = 0;
            this.tabValidaciones.Size = new System.Drawing.Size(860, 437);
            this.tabValidaciones.TabIndex = 0;
            this.tabValidaciones.SelectedIndexChanged += new System.EventHandler(this.tabValidaciones_SelectedIndexChanged);
            // 
            // tbpAutorizaciones
            // 
            this.tbpAutorizaciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.tbpAutorizaciones.Location = new System.Drawing.Point(4, 22);
            this.tbpAutorizaciones.Name = "tbpAutorizaciones";
            this.tbpAutorizaciones.Padding = new System.Windows.Forms.Padding(3);
            this.tbpAutorizaciones.Size = new System.Drawing.Size(852, 411);
            this.tbpAutorizaciones.TabIndex = 0;
            this.tbpAutorizaciones.Text = "Autorizaciones";
            // 
            // tbpCortes
            // 
            this.tbpCortes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.tbpCortes.Location = new System.Drawing.Point(4, 22);
            this.tbpCortes.Name = "tbpCortes";
            this.tbpCortes.Padding = new System.Windows.Forms.Padding(3);
            this.tbpCortes.Size = new System.Drawing.Size(852, 411);
            this.tbpCortes.TabIndex = 1;
            this.tbpCortes.Text = "Comprobación Corte";
            // 
            // tbpRecepcionCredito
            // 
            this.tbpRecepcionCredito.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.tbpRecepcionCredito.Location = new System.Drawing.Point(4, 22);
            this.tbpRecepcionCredito.Name = "tbpRecepcionCredito";
            this.tbpRecepcionCredito.Size = new System.Drawing.Size(852, 411);
            this.tbpRecepcionCredito.TabIndex = 2;
            this.tbpRecepcionCredito.Text = "Recepción crédito";
            // 
            // Validaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.tabValidaciones);
            this.Name = "Validaciones";
            this.Size = new System.Drawing.Size(866, 443);
            this.tabValidaciones.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabValidaciones;
        private System.Windows.Forms.TabPage tbpAutorizaciones;
        private System.Windows.Forms.TabPage tbpCortes;
        private System.Windows.Forms.TabPage tbpRecepcionCredito;
    }
}
