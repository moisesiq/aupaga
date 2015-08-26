namespace Actualizador
{
    partial class ActImagenes
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
            this.lsvArchivos = new System.Windows.Forms.ListView();
            this.colArchivo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEstatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMensaje = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAccion = new System.Windows.Forms.Button();
            this.pgbActualizacion = new System.Windows.Forms.ProgressBar();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lsvArchivos
            // 
            this.lsvArchivos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvArchivos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colArchivo,
            this.colEstatus,
            this.colMensaje});
            this.lsvArchivos.FullRowSelect = true;
            this.lsvArchivos.Location = new System.Drawing.Point(3, 3);
            this.lsvArchivos.Name = "lsvArchivos";
            this.lsvArchivos.Size = new System.Drawing.Size(664, 355);
            this.lsvArchivos.TabIndex = 6;
            this.lsvArchivos.UseCompatibleStateImageBehavior = false;
            this.lsvArchivos.View = System.Windows.Forms.View.Details;
            // 
            // colArchivo
            // 
            this.colArchivo.Text = "Archivo";
            this.colArchivo.Width = 400;
            // 
            // colEstatus
            // 
            this.colEstatus.Text = "Estatus";
            this.colEstatus.Width = 80;
            // 
            // colMensaje
            // 
            this.colMensaje.Text = "Mensaje";
            this.colMensaje.Width = 160;
            // 
            // btnAccion
            // 
            this.btnAccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccion.Location = new System.Drawing.Point(587, 364);
            this.btnAccion.Name = "btnAccion";
            this.btnAccion.Size = new System.Drawing.Size(80, 28);
            this.btnAccion.TabIndex = 8;
            this.btnAccion.Text = "Cerrar";
            this.btnAccion.UseVisualStyleBackColor = true;
            this.btnAccion.Click += new System.EventHandler(this.btnAccion_Click);
            // 
            // pgbActualizacion
            // 
            this.pgbActualizacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbActualizacion.Location = new System.Drawing.Point(3, 364);
            this.pgbActualizacion.Name = "pgbActualizacion";
            this.pgbActualizacion.Size = new System.Drawing.Size(578, 28);
            this.pgbActualizacion.Step = 1;
            this.pgbActualizacion.TabIndex = 7;
            // 
            // lblMensaje
            // 
            this.lblMensaje.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje.Location = new System.Drawing.Point(255, 155);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(160, 40);
            this.lblMensaje.TabIndex = 9;
            this.lblMensaje.Text = "Comparando..";
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMensaje.Visible = false;
            // 
            // ActImagenes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 395);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.lsvArchivos);
            this.Controls.Add(this.btnAccion);
            this.Controls.Add(this.pgbActualizacion);
            this.Icon = Properties.Resources.G;
            this.Name = "ActImagenes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Actualizador de Imágenes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ActImagenes_FormClosing);
            this.Load += new System.EventHandler(this.ActImagenes_Load);
            this.Shown += new System.EventHandler(this.ActImagenes_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lsvArchivos;
        private System.Windows.Forms.ColumnHeader colArchivo;
        private System.Windows.Forms.ColumnHeader colEstatus;
        private System.Windows.Forms.ColumnHeader colMensaje;
        private System.Windows.Forms.Button btnAccion;
        private System.Windows.Forms.ProgressBar pgbActualizacion;
        private System.Windows.Forms.Label lblMensaje;


    }
}