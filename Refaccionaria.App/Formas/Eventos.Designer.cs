namespace Refaccionaria.App
{
    partial class Eventos
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
            this.flpEventos = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlMuestra = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.pnlMuestra.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpEventos
            // 
            this.flpEventos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpEventos.AutoScroll = true;
            this.flpEventos.Location = new System.Drawing.Point(12, 12);
            this.flpEventos.Name = "flpEventos";
            this.flpEventos.Size = new System.Drawing.Size(420, 278);
            this.flpEventos.TabIndex = 0;
            // 
            // pnlMuestra
            // 
            this.pnlMuestra.BackColor = System.Drawing.Color.White;
            this.pnlMuestra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMuestra.Controls.Add(this.button1);
            this.pnlMuestra.Controls.Add(this.label2);
            this.pnlMuestra.Controls.Add(this.label1);
            this.pnlMuestra.Controls.Add(this.lblFecha);
            this.pnlMuestra.Location = new System.Drawing.Point(12, 12);
            this.pnlMuestra.Name = "pnlMuestra";
            this.pnlMuestra.Size = new System.Drawing.Size(420, 64);
            this.pnlMuestra.TabIndex = 0;
            this.pnlMuestra.Visible = false;
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ok;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Location = new System.Drawing.Point(391, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 32);
            this.button1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(382, 40);
            this.label2.TabIndex = 2;
            this.label2.Text = "una descripción";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(127, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(288, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre del Cliente";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFecha
            // 
            this.lblFecha.Location = new System.Drawing.Point(0, 0);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(121, 20);
            this.lblFecha.TabIndex = 0;
            this.lblFecha.Text = "10/10/2052 10:34 p. m.";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Eventos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.ClientSize = new System.Drawing.Size(444, 302);
            this.Controls.Add(this.pnlMuestra);
            this.Controls.Add(this.flpEventos);
            this.Name = "Eventos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Eventos";
            this.Load += new System.EventHandler(this.Eventos_Load);
            this.pnlMuestra.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpEventos;
        private System.Windows.Forms.Panel pnlMuestra;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}