namespace Refaccionaria.App
{
    partial class DetalleNivel
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
            this.clbCriterioAbc = new System.Windows.Forms.CheckedListBox();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.clbCriterioAbc);
            this.gpoGen.Size = new System.Drawing.Size(171, 252);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(101, 270);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(13, 270);
            // 
            // clbCriterioAbc
            // 
            this.clbCriterioAbc.CheckOnClick = true;
            this.clbCriterioAbc.FormattingEnabled = true;
            this.clbCriterioAbc.Location = new System.Drawing.Point(6, 19);
            this.clbCriterioAbc.Name = "clbCriterioAbc";
            this.clbCriterioAbc.Size = new System.Drawing.Size(159, 214);
            this.clbCriterioAbc.TabIndex = 0;
            this.clbCriterioAbc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.clbCriterioAbc_KeyDown);
            // 
            // DetalleNivel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 301);
            this.Name = "DetalleNivel";
            this.Text = "DetalleNivel";
            this.Load += new System.EventHandler(this.DetalleNivel_Load);
            this.gpoGen.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbCriterioAbc;
    }
}