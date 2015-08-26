namespace Refaccionaria.Negocio
{
    partial class ComboMultiSel
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
            this.txtSeleccion = new System.Windows.Forms.TextBox();
            this.btnCombo = new System.Windows.Forms.Button();
            this.clbSeleccion = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // txtSeleccion
            // 
            this.txtSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSeleccion.Location = new System.Drawing.Point(0, 0);
            this.txtSeleccion.Margin = new System.Windows.Forms.Padding(0);
            this.txtSeleccion.Name = "txtSeleccion";
            this.txtSeleccion.Size = new System.Drawing.Size(179, 20);
            this.txtSeleccion.TabIndex = 0;
            this.txtSeleccion.Click += new System.EventHandler(this.txtSeleccion_Click);
            this.txtSeleccion.TextChanged += new System.EventHandler(this.txtSeleccion_TextChanged);
            this.txtSeleccion.Enter += new System.EventHandler(this.txtSeleccion_Enter);
            this.txtSeleccion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSeleccion_KeyDown);
            this.txtSeleccion.Leave += new System.EventHandler(this.txtSeleccion_Leave);
            // 
            // btnCombo
            // 
            this.btnCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCombo.ForeColor = System.Drawing.Color.Black;
            this.btnCombo.Location = new System.Drawing.Point(179, -1);
            this.btnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.btnCombo.Name = "btnCombo";
            this.btnCombo.Size = new System.Drawing.Size(21, 21);
            this.btnCombo.TabIndex = 1;
            this.btnCombo.Text = "V";
            this.btnCombo.UseVisualStyleBackColor = true;
            this.btnCombo.Click += new System.EventHandler(this.btnCombo_Click);
            // 
            // clbSeleccion
            // 
            this.clbSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clbSeleccion.CheckOnClick = true;
            this.clbSeleccion.FormattingEnabled = true;
            this.clbSeleccion.Location = new System.Drawing.Point(0, 19);
            this.clbSeleccion.Margin = new System.Windows.Forms.Padding(0);
            this.clbSeleccion.Name = "clbSeleccion";
            this.clbSeleccion.Size = new System.Drawing.Size(200, 184);
            this.clbSeleccion.TabIndex = 2;
            this.clbSeleccion.Visible = false;
            this.clbSeleccion.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbSeleccion_ItemCheck);
            this.clbSeleccion.SelectedIndexChanged += new System.EventHandler(this.clbSeleccion_SelectedIndexChanged);
            // 
            // ComboMultiSel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.clbSeleccion);
            this.Controls.Add(this.btnCombo);
            this.Controls.Add(this.txtSeleccion);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ComboMultiSel";
            this.Size = new System.Drawing.Size(200, 203);
            this.Load += new System.EventHandler(this.ComboMultiSel_Load);
            this.Leave += new System.EventHandler(this.ComboMultiSel_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSeleccion;
        private System.Windows.Forms.Button btnCombo;
        private System.Windows.Forms.CheckedListBox clbSeleccion;
    }
}
