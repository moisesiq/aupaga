namespace Refaccionaria.App
{
    partial class BusquedaVIN
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
            this.gbBrowser = new System.Windows.Forms.GroupBox();
            this.wkbVIN = new WebKit.WebKitBrowser();
            this.gbBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbBrowser
            // 
            this.gbBrowser.Controls.Add(this.wkbVIN);
            this.gbBrowser.Location = new System.Drawing.Point(3, 12);
            this.gbBrowser.Name = "gbBrowser";
            this.gbBrowser.Size = new System.Drawing.Size(1113, 560);
            this.gbBrowser.TabIndex = 0;
            this.gbBrowser.TabStop = false;
            // 
            // wkbVIN
            // 
            this.wkbVIN.BackColor = System.Drawing.Color.White;
            this.wkbVIN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wkbVIN.Location = new System.Drawing.Point(3, 16);
            this.wkbVIN.Name = "wkbVIN";
            this.wkbVIN.Size = new System.Drawing.Size(1107, 541);
            this.wkbVIN.TabIndex = 1;
            this.wkbVIN.Url = null;
            this.wkbVIN.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wkbVIN_DocumentCompleted);
            // 
            // BusquedaVIN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.ClientSize = new System.Drawing.Size(1119, 618);
            this.Controls.Add(this.gbBrowser);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BusquedaVIN";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BusquedaVIN";
            this.gbBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbBrowser;
        private WebKit.WebKitBrowser wkbVIN;
    }
}