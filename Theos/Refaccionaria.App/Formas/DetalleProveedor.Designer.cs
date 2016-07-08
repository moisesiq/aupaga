namespace Refaccionaria.App
{
    partial class DetalleProveedor
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
            this.txtNombreProveedor = new System.Windows.Forms.TextBox();
            this.lblNombreFabricante = new System.Windows.Forms.Label();
            this.gpoGen.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpoGen
            // 
            this.gpoGen.Controls.Add(this.txtNombreProveedor);
            this.gpoGen.Controls.Add(this.lblNombreFabricante);
            this.gpoGen.Size = new System.Drawing.Size(416, 44);
            // 
            // btnCerrar
            // 
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(340, 62);
            this.btnCerrar.Size = new System.Drawing.Size(82, 23);
            // 
            // btnGuardar
            // 
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(254, 62);
            this.btnGuardar.Size = new System.Drawing.Size(82, 23);
            // 
            // txtNombreProveedor
            // 
            this.txtNombreProveedor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombreProveedor.Location = new System.Drawing.Point(68, 13);
            this.txtNombreProveedor.MaxLength = 100;
            this.txtNombreProveedor.Name = "txtNombreProveedor";
            this.txtNombreProveedor.Size = new System.Drawing.Size(342, 20);
            this.txtNombreProveedor.TabIndex = 7;
            // 
            // lblNombreFabricante
            // 
            this.lblNombreFabricante.AutoSize = true;
            this.lblNombreFabricante.ForeColor = System.Drawing.Color.White;
            this.lblNombreFabricante.Location = new System.Drawing.Point(6, 16);
            this.lblNombreFabricante.Name = "lblNombreFabricante";
            this.lblNombreFabricante.Size = new System.Drawing.Size(56, 13);
            this.lblNombreFabricante.TabIndex = 6;
            this.lblNombreFabricante.Text = "Proveedor";
            // 
            // DetalleProveedor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(440, 93);
            this.KeyPreview = true;
            this.Name = "DetalleProveedor";
            this.Load += new System.EventHandler(this.DetalleProveedor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DetalleProveedor_KeyDown);
            this.gpoGen.ResumeLayout(false);
            this.gpoGen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreProveedor;
        private System.Windows.Forms.Label lblNombreFabricante;
    }
}
