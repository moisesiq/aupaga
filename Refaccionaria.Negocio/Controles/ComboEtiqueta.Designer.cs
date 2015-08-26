namespace Refaccionaria.Negocio
{
    partial class ComboEtiqueta
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
            this.cmbCombo = new System.Windows.Forms.ComboBox();
            this.lblEtiqueta = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbCombo
            // 
            this.cmbCombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCombo.FormattingEnabled = true;
            this.cmbCombo.Location = new System.Drawing.Point(0, 0);
            this.cmbCombo.Name = "cmbCombo";
            this.cmbCombo.Size = new System.Drawing.Size(121, 21);
            this.cmbCombo.TabIndex = 0;
            this.cmbCombo.SelectedIndexChanged += new System.EventHandler(this.cmbCombo_SelectedIndexChanged);
            this.cmbCombo.DropDownStyleChanged += new System.EventHandler(this.cmbCombo_DropDownStyleChanged);
            this.cmbCombo.TextChanged += new System.EventHandler(this.cmbCombo_TextChanged);
            this.cmbCombo.Enter += new System.EventHandler(this.cmbCombo_Enter);
            this.cmbCombo.Leave += new System.EventHandler(this.cmbCombo_Leave);
            // 
            // lblEtiqueta
            // 
            this.lblEtiqueta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEtiqueta.BackColor = System.Drawing.SystemColors.Window;
            this.lblEtiqueta.ForeColor = System.Drawing.Color.Gray;
            this.lblEtiqueta.Location = new System.Drawing.Point(3, 3);
            this.lblEtiqueta.Name = "lblEtiqueta";
            this.lblEtiqueta.Size = new System.Drawing.Size(97, 16);
            this.lblEtiqueta.TabIndex = 1;
            this.lblEtiqueta.Text = "Etiqueta";
            this.lblEtiqueta.Click += new System.EventHandler(this.lblEtiqueta_Click);
            // 
            // ComboEtiqueta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.lblEtiqueta);
            this.Controls.Add(this.cmbCombo);
            this.Name = "ComboEtiqueta";
            this.Size = new System.Drawing.Size(121, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbCombo;
        private System.Windows.Forms.Label lblEtiqueta;
    }
}
