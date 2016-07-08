namespace Refaccionaria.App
{
    partial class Navegador
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
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnIr = new System.Windows.Forms.Button();
            this.webPagina = new System.Windows.Forms.WebBrowser();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(3, 3);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(641, 20);
            this.txtUrl.TabIndex = 0;
            // 
            // btnIr
            // 
            this.btnIr.Location = new System.Drawing.Point(650, 1);
            this.btnIr.Name = "btnIr";
            this.btnIr.Size = new System.Drawing.Size(40, 23);
            this.btnIr.TabIndex = 1;
            this.btnIr.Text = "Ir";
            this.btnIr.UseVisualStyleBackColor = true;
            // 
            // webPagina
            // 
            this.webPagina.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webPagina.Location = new System.Drawing.Point(3, 29);
            this.webPagina.MinimumSize = new System.Drawing.Size(20, 20);
            this.webPagina.Name = "webPagina";
            this.webPagina.Size = new System.Drawing.Size(687, 239);
            this.webPagina.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(569, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Navegador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 272);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.webPagina);
            this.Controls.Add(this.btnIr);
            this.Controls.Add(this.txtUrl);
            this.Name = "Navegador";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Navegador";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnIr;
        private System.Windows.Forms.WebBrowser webPagina;
        private System.Windows.Forms.Button button1;

    }
}