﻿namespace Refaccionaria.App
{
    partial class CuadroClientes
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
            ((System.ComponentModel.ISupportInitialize)(this.chrMeses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrSemanas)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.Text = "Utilidad por Cliente";
            // 
            // cmbCalculo
            // 
            this.cmbCalculo.SelectedIndexChanged += new System.EventHandler(this.cmbCalculo_SelectedIndexChanged);
            // 
            // lblCalculo
            // 
            this.lblCalculo.Click += new System.EventHandler(this.lblCalculo_Click);
            // 
            // CuadroClientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Name = "CuadroClientes";
            this.Load += new System.EventHandler(this.CuadroClientes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chrMeses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chrSemanas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}
