namespace Refaccionaria.Negocio
{
    partial class Duck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Duck));
            this.wmpDuck = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.wmpDuck)).BeginInit();
            this.SuspendLayout();
            // 
            // wmpDuck
            // 
            this.wmpDuck.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wmpDuck.CausesValidation = false;
            this.wmpDuck.Enabled = true;
            this.wmpDuck.Location = new System.Drawing.Point(-1, 0);
            this.wmpDuck.Name = "wmpDuck";
            this.wmpDuck.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("wmpDuck.OcxState")));
            this.wmpDuck.Size = new System.Drawing.Size(695, 633);
            this.wmpDuck.TabIndex = 0;
            this.wmpDuck.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.wmpDuck_PlayStateChange);
            // 
            // Duck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 632);
            this.Controls.Add(this.wmpDuck);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Duck";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Duck";
            ((System.ComponentModel.ISupportInitialize)(this.wmpDuck)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer wmpDuck;
    }
}