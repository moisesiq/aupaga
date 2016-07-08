namespace Refaccionaria.App
{
    partial class DetalleResolverContingencia
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
            this.lblOperacion = new System.Windows.Forms.Label();
            this.cboTipoOperacion = new System.Windows.Forms.ComboBox();
            this.cboConceptoOperacion = new System.Windows.Forms.ComboBox();
            this.lblConcepto = new System.Windows.Forms.Label();
            this.txtObservacion = new System.Windows.Forms.TextBox();
            this.lblObservacion = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.lblObservacion);
            this.gpoGen.Controls.Add(this.txtObservacion);
            this.gpoGen.Controls.Add(this.cboConceptoOperacion);
            this.gpoGen.Controls.Add(this.lblConcepto);
            this.gpoGen.Controls.Add(this.cboTipoOperacion);
            this.gpoGen.Controls.Add(this.lblOperacion);
            this.gpoGen.Size = new System.Drawing.Size(390, 149);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(320, 167);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(232, 167);
            // 
            // lblOperacion
            // 
            this.lblOperacion.AutoSize = true;
            this.lblOperacion.ForeColor = System.Drawing.Color.White;
            this.lblOperacion.Location = new System.Drawing.Point(6, 22);
            this.lblOperacion.Name = "lblOperacion";
            this.lblOperacion.Size = new System.Drawing.Size(56, 13);
            this.lblOperacion.TabIndex = 5;
            this.lblOperacion.Text = "Operación";
            // 
            // cboTipoOperacion
            // 
            this.cboTipoOperacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoOperacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboTipoOperacion.FormattingEnabled = true;
            this.cboTipoOperacion.Location = new System.Drawing.Point(117, 19);
            this.cboTipoOperacion.Name = "cboTipoOperacion";
            this.cboTipoOperacion.Size = new System.Drawing.Size(250, 21);
            this.cboTipoOperacion.TabIndex = 6;
            this.cboTipoOperacion.SelectedValueChanged += new System.EventHandler(this.cboTipoOperacion_SelectedValueChanged);
            // 
            // cboConceptoOperacion
            // 
            this.cboConceptoOperacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConceptoOperacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboConceptoOperacion.FormattingEnabled = true;
            this.cboConceptoOperacion.Location = new System.Drawing.Point(117, 46);
            this.cboConceptoOperacion.Name = "cboConceptoOperacion";
            this.cboConceptoOperacion.Size = new System.Drawing.Size(250, 21);
            this.cboConceptoOperacion.TabIndex = 8;
            // 
            // lblConcepto
            // 
            this.lblConcepto.AutoSize = true;
            this.lblConcepto.ForeColor = System.Drawing.Color.White;
            this.lblConcepto.Location = new System.Drawing.Point(6, 49);
            this.lblConcepto.Name = "lblConcepto";
            this.lblConcepto.Size = new System.Drawing.Size(105, 13);
            this.lblConcepto.TabIndex = 7;
            this.lblConcepto.Text = "Concepto Operación";
            // 
            // txtObservacion
            // 
            this.txtObservacion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtObservacion.Location = new System.Drawing.Point(117, 74);
            this.txtObservacion.MaxLength = 255;
            this.txtObservacion.Multiline = true;
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.Size = new System.Drawing.Size(250, 60);
            this.txtObservacion.TabIndex = 9;
            // 
            // lblObservacion
            // 
            this.lblObservacion.AutoSize = true;
            this.lblObservacion.ForeColor = System.Drawing.Color.White;
            this.lblObservacion.Location = new System.Drawing.Point(6, 77);
            this.lblObservacion.Name = "lblObservacion";
            this.lblObservacion.Size = new System.Drawing.Size(67, 13);
            this.lblObservacion.TabIndex = 10;
            this.lblObservacion.Text = "Observación";
            // 
            // DetalleResolverContingencia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(414, 198);
            this.Name = "DetalleResolverContingencia";
            this.Load += new System.EventHandler(this.DetalleResolverContingencia_Load);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblOperacion;
        private System.Windows.Forms.ComboBox cboConceptoOperacion;
        private System.Windows.Forms.Label lblConcepto;
        private System.Windows.Forms.ComboBox cboTipoOperacion;
        private System.Windows.Forms.Label lblObservacion;
        private System.Windows.Forms.TextBox txtObservacion;
    }
}
