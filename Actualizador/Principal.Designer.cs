namespace Actualizador
{
    partial class Principal
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
            this.pgbActualizacion = new System.Windows.Forms.ProgressBar();
            this.btnAccion = new System.Windows.Forms.Button();
            this.lsvArchivos = new System.Windows.Forms.ListView();
            this.colArchivo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEstatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMensaje = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pgbActualizacion
            // 
            this.pgbActualizacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbActualizacion.Location = new System.Drawing.Point(3, 364);
            this.pgbActualizacion.Name = "pgbActualizacion";
            this.pgbActualizacion.Size = new System.Drawing.Size(578, 28);
            this.pgbActualizacion.Step = 1;
            this.pgbActualizacion.TabIndex = 1;
            // 
            // btnAccion
            // 
            this.btnAccion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccion.Enabled = false;
            this.btnAccion.Location = new System.Drawing.Point(587, 364);
            this.btnAccion.Name = "btnAccion";
            this.btnAccion.Size = new System.Drawing.Size(80, 28);
            this.btnAccion.TabIndex = 2;
            this.btnAccion.Text = "Procesando..";
            this.btnAccion.UseVisualStyleBackColor = true;
            this.btnAccion.Click += new System.EventHandler(this.btnAccion_Click);
            // 
            // lsvArchivos
            // 
            this.lsvArchivos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvArchivos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.lsvArchivos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colArchivo,
            this.colEstatus,
            this.colMensaje});
            this.lsvArchivos.FullRowSelect = true;
            this.lsvArchivos.Location = new System.Drawing.Point(3, 3);
            this.lsvArchivos.Name = "lsvArchivos";
            this.lsvArchivos.Size = new System.Drawing.Size(664, 301);
            this.lsvArchivos.TabIndex = 0;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(4, 317);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(654, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Existen sesiones de Theos abiertas. Para actualizar es necesario que se cierren.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(4, 337);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(300, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Posteriormente presiona Reintentar.";
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.ClientSize = new System.Drawing.Size(670, 395);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsvArchivos);
            this.Controls.Add(this.btnAccion);
            this.Controls.Add(this.pgbActualizacion);
            this.Icon = global::Actualizador.Properties.Resources.G;
            this.Name = "Principal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Actualizador";
            this.Load += new System.EventHandler(this.Principal_Load);
            this.Shown += new System.EventHandler(this.Principal_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgbActualizacion;
        private System.Windows.Forms.Button btnAccion;
        private System.Windows.Forms.ListView lsvArchivos;
        private System.Windows.Forms.ColumnHeader colArchivo;
        private System.Windows.Forms.ColumnHeader colEstatus;
        private System.Windows.Forms.ColumnHeader colMensaje;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

