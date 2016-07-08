namespace Refaccionaria.App
{
    partial class DetalleProveedorProntoPago
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
            this.lblNumeroDias = new System.Windows.Forms.Label();
            this.nudNumeroDias = new System.Windows.Forms.NumericUpDown();
            this.lblPorcentaje = new System.Windows.Forms.Label();
            this.txtPorcentaje = new Refaccionaria.App.textBoxOnlyDouble();
            this.lblComentario = new System.Windows.Forms.Label();
            this.txtComentario = new System.Windows.Forms.TextBox();
            this.gpoGen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumeroDias)).BeginInit();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtComentario);
            this.gpoGen.Controls.Add(this.lblComentario);
            this.gpoGen.Controls.Add(this.txtPorcentaje);
            this.gpoGen.Controls.Add(this.lblPorcentaje);
            this.gpoGen.Controls.Add(this.nudNumeroDias);
            this.gpoGen.Controls.Add(this.lblNumeroDias);
            this.gpoGen.ForeColor = System.Drawing.Color.White;
            this.gpoGen.Size = new System.Drawing.Size(420, 102);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(350, 120);
            this.btnCerrar.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(262, 120);
            this.btnGuardar.TabIndex = 1;
            // 
            // lblNumeroDias
            // 
            this.lblNumeroDias.AutoSize = true;
            this.lblNumeroDias.ForeColor = System.Drawing.Color.White;
            this.lblNumeroDias.Location = new System.Drawing.Point(6, 16);
            this.lblNumeroDias.Name = "lblNumeroDias";
            this.lblNumeroDias.Size = new System.Drawing.Size(83, 13);
            this.lblNumeroDias.TabIndex = 0;
            this.lblNumeroDias.Text = "Número de Dias";
            // 
            // nudNumeroDias
            // 
            this.nudNumeroDias.Location = new System.Drawing.Point(138, 14);
            this.nudNumeroDias.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.nudNumeroDias.Name = "nudNumeroDias";
            this.nudNumeroDias.Size = new System.Drawing.Size(47, 20);
            this.nudNumeroDias.TabIndex = 0;
            // 
            // lblPorcentaje
            // 
            this.lblPorcentaje.AutoSize = true;
            this.lblPorcentaje.Location = new System.Drawing.Point(6, 43);
            this.lblPorcentaje.Name = "lblPorcentaje";
            this.lblPorcentaje.Size = new System.Drawing.Size(126, 13);
            this.lblPorcentaje.TabIndex = 2;
            this.lblPorcentaje.Text = "Porcentaje de descuento";
            // 
            // txtPorcentaje
            // 
            this.txtPorcentaje.Location = new System.Drawing.Point(138, 40);
            this.txtPorcentaje.Name = "txtPorcentaje";
            this.txtPorcentaje.Size = new System.Drawing.Size(47, 20);
            this.txtPorcentaje.TabIndex = 1;
            // 
            // lblComentario
            // 
            this.lblComentario.AutoSize = true;
            this.lblComentario.Location = new System.Drawing.Point(6, 69);
            this.lblComentario.Name = "lblComentario";
            this.lblComentario.Size = new System.Drawing.Size(60, 13);
            this.lblComentario.TabIndex = 4;
            this.lblComentario.Text = "Comentario";
            // 
            // txtComentario
            // 
            this.txtComentario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtComentario.Location = new System.Drawing.Point(138, 66);
            this.txtComentario.MaxLength = 255;
            this.txtComentario.Name = "txtComentario";
            this.txtComentario.Size = new System.Drawing.Size(264, 20);
            this.txtComentario.TabIndex = 2;
            // 
            // DetalleProveedorProntoPago
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(444, 151);
            this.KeyPreview = true;
            this.Name = "DetalleProveedorProntoPago";
            this.Load += new System.EventHandler(this.DetalleProveedorProntoPago_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleProveedorProntoPago_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumeroDias)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtComentario;
        private System.Windows.Forms.Label lblComentario;
        private textBoxOnlyDouble txtPorcentaje;
        private System.Windows.Forms.Label lblPorcentaje;
        private System.Windows.Forms.NumericUpDown nudNumeroDias;
        private System.Windows.Forms.Label lblNumeroDias;
    }
}
