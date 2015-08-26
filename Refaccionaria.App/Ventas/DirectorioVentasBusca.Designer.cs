namespace Refaccionaria.App
{
    partial class DirectorioVentasBusca
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
            this.btnVistaDetalle = new System.Windows.Forms.Button();
            this.btnVistaIconos = new System.Windows.Forms.Button();
            this.txFinder = new Refaccionaria.Negocio.TextoMod();
            this.pnlBusqueda = new System.Windows.Forms.Panel();
            this.txtCodigo = new Refaccionaria.Negocio.TextoMod();
            this.pnlBusqueda.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnVistaDetalle
            // 
            this.btnVistaDetalle.FlatAppearance.BorderSize = 0;
            this.btnVistaDetalle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVistaDetalle.Image = global::Refaccionaria.App.Properties.Resources.VistaDetalle;
            this.btnVistaDetalle.Location = new System.Drawing.Point(633, 6);
            this.btnVistaDetalle.Name = "btnVistaDetalle";
            this.btnVistaDetalle.Size = new System.Drawing.Size(21, 21);
            this.btnVistaDetalle.TabIndex = 5;
            this.btnVistaDetalle.UseVisualStyleBackColor = true;
            this.btnVistaDetalle.Click += new System.EventHandler(this.btnVistaDetalle_Click);
            // 
            // btnVistaIconos
            // 
            this.btnVistaIconos.FlatAppearance.BorderSize = 0;
            this.btnVistaIconos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVistaIconos.Image = global::Refaccionaria.App.Properties.Resources.VistaIconos;
            this.btnVistaIconos.Location = new System.Drawing.Point(610, 6);
            this.btnVistaIconos.Name = "btnVistaIconos";
            this.btnVistaIconos.Size = new System.Drawing.Size(21, 21);
            this.btnVistaIconos.TabIndex = 2;
            this.btnVistaIconos.UseVisualStyleBackColor = true;
            this.btnVistaIconos.Click += new System.EventHandler(this.btnVistaIconos_Click);
            // 
            // txFinder
            // 
            this.txFinder.Etiqueta = "";
            this.txFinder.EtiquetaColor = System.Drawing.Color.Gray;
            this.txFinder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txFinder.Location = new System.Drawing.Point(142, 7);
            this.txFinder.Name = "txFinder";
            this.txFinder.PasarEnfoqueConEnter = false;
            this.txFinder.SeleccionarTextoAlEnfoque = false;
            this.txFinder.Size = new System.Drawing.Size(462, 21);
            this.txFinder.TabIndex = 1;
            this.txFinder.TextChanged += new System.EventHandler(this.txFinder_TextChanged);
            // 
            // pnlBusqueda
            // 
            this.pnlBusqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBusqueda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(188)))), ((int)(((byte)(216)))));
            this.pnlBusqueda.Controls.Add(this.txtCodigo);
            this.pnlBusqueda.Controls.Add(this.txFinder);
            this.pnlBusqueda.Controls.Add(this.btnVistaDetalle);
            this.pnlBusqueda.Controls.Add(this.btnVistaIconos);
            this.pnlBusqueda.Location = new System.Drawing.Point(0, 0);
            this.pnlBusqueda.Name = "pnlBusqueda";
            this.pnlBusqueda.Size = new System.Drawing.Size(918, 34);
            this.pnlBusqueda.TabIndex = 10;
            // 
            // txtCodigo
            // 
            this.txtCodigo.Enabled = false;
            this.txtCodigo.Etiqueta = "Código";
            this.txtCodigo.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.Location = new System.Drawing.Point(6, 7);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.PasarEnfoqueConEnter = false;
            this.txtCodigo.SeleccionarTextoAlEnfoque = false;
            this.txtCodigo.Size = new System.Drawing.Size(130, 21);
            this.txtCodigo.TabIndex = 9;
            // 
            // DirectorioVentasBusca
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.Controls.Add(this.pnlBusqueda);
            this.Name = "DirectorioVentasBusca";
            this.Size = new System.Drawing.Size(670, 129);
            this.pnlBusqueda.ResumeLayout(false);
            this.pnlBusqueda.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Negocio.TextoMod txFinder;
        private System.Windows.Forms.Button btnVistaDetalle;
        private System.Windows.Forms.Button btnVistaIconos;
        private System.Windows.Forms.Panel pnlBusqueda;
        private Negocio.TextoMod txtCodigo;

    }
}
