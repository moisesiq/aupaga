namespace FacturacionElectronica
{
    partial class FacturasSat
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMes = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDescargar = new System.Windows.Forms.Button();
            this.cmbAnio = new System.Windows.Forms.ComboBox();
            this.cmbDia = new System.Windows.Forms.ComboBox();
            this.pgbProceso = new System.Windows.Forms.ProgressBar();
            this.lblProceso = new System.Windows.Forms.Label();
            this.lblAvance = new System.Windows.Forms.Label();
            this.webSat = new System.Windows.Forms.WebBrowser();
            this.btnConsultar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Año";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(110, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Mes";
            // 
            // cmbMes
            // 
            this.cmbMes.FormattingEnabled = true;
            this.cmbMes.Location = new System.Drawing.Point(143, 6);
            this.cmbMes.Name = "cmbMes";
            this.cmbMes.Size = new System.Drawing.Size(121, 21);
            this.cmbMes.TabIndex = 1;
            this.cmbMes.SelectedIndexChanged += new System.EventHandler(this.cmbMes_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(270, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Día";
            // 
            // btnDescargar
            // 
            this.btnDescargar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnDescargar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDescargar.ForeColor = System.Drawing.Color.White;
            this.btnDescargar.Location = new System.Drawing.Point(1033, 6);
            this.btnDescargar.Name = "btnDescargar";
            this.btnDescargar.Size = new System.Drawing.Size(75, 23);
            this.btnDescargar.TabIndex = 4;
            this.btnDescargar.Text = "&Descargar";
            this.btnDescargar.UseVisualStyleBackColor = false;
            this.btnDescargar.Click += new System.EventHandler(this.btnDescargar_Click);
            // 
            // cmbAnio
            // 
            this.cmbAnio.FormattingEnabled = true;
            this.cmbAnio.Location = new System.Drawing.Point(44, 6);
            this.cmbAnio.Name = "cmbAnio";
            this.cmbAnio.Size = new System.Drawing.Size(60, 21);
            this.cmbAnio.TabIndex = 0;
            this.cmbAnio.SelectedIndexChanged += new System.EventHandler(this.cmbAnio_SelectedIndexChanged);
            // 
            // cmbDia
            // 
            this.cmbDia.FormattingEnabled = true;
            this.cmbDia.Location = new System.Drawing.Point(301, 6);
            this.cmbDia.Name = "cmbDia";
            this.cmbDia.Size = new System.Drawing.Size(60, 21);
            this.cmbDia.TabIndex = 2;
            // 
            // pgbProceso
            // 
            this.pgbProceso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbProceso.Location = new System.Drawing.Point(143, 35);
            this.pgbProceso.Name = "pgbProceso";
            this.pgbProceso.Size = new System.Drawing.Size(965, 23);
            this.pgbProceso.Step = 1;
            this.pgbProceso.TabIndex = 13;
            // 
            // lblProceso
            // 
            this.lblProceso.AutoSize = true;
            this.lblProceso.ForeColor = System.Drawing.Color.White;
            this.lblProceso.Location = new System.Drawing.Point(12, 44);
            this.lblProceso.Name = "lblProceso";
            this.lblProceso.Size = new System.Drawing.Size(46, 13);
            this.lblProceso.TabIndex = 14;
            this.lblProceso.Text = "Proceso";
            // 
            // lblAvance
            // 
            this.lblAvance.AutoSize = true;
            this.lblAvance.ForeColor = System.Drawing.Color.White;
            this.lblAvance.Location = new System.Drawing.Point(88, 44);
            this.lblAvance.Name = "lblAvance";
            this.lblAvance.Size = new System.Drawing.Size(49, 13);
            this.lblAvance.TabIndex = 15;
            this.lblAvance.Text = "10 de 25";
            // 
            // webSat
            // 
            this.webSat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webSat.Location = new System.Drawing.Point(12, 64);
            this.webSat.MinimumSize = new System.Drawing.Size(20, 20);
            this.webSat.Name = "webSat";
            this.webSat.Size = new System.Drawing.Size(1096, 551);
            this.webSat.TabIndex = 5;
            // 
            // btnConsultar
            // 
            this.btnConsultar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnConsultar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConsultar.ForeColor = System.Drawing.Color.White;
            this.btnConsultar.Location = new System.Drawing.Point(952, 6);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(75, 23);
            this.btnConsultar.TabIndex = 3;
            this.btnConsultar.Text = "&Consultar";
            this.btnConsultar.UseVisualStyleBackColor = false;
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
            // 
            // FacturasSat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.ClientSize = new System.Drawing.Size(1120, 627);
            this.Controls.Add(this.btnConsultar);
            this.Controls.Add(this.webSat);
            this.Controls.Add(this.lblAvance);
            this.Controls.Add(this.lblProceso);
            this.Controls.Add(this.pgbProceso);
            this.Controls.Add(this.cmbDia);
            this.Controls.Add(this.cmbAnio);
            this.Controls.Add(this.btnDescargar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbMes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FacturasSat";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FacturasSat";
            this.Load += new System.EventHandler(this.FacturasSat_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbMes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDescargar;
        private System.Windows.Forms.ComboBox cmbAnio;
        private System.Windows.Forms.ComboBox cmbDia;
        private System.Windows.Forms.ProgressBar pgbProceso;
        private System.Windows.Forms.Label lblProceso;
        private System.Windows.Forms.Label lblAvance;
        private System.Windows.Forms.WebBrowser webSat;
        private System.Windows.Forms.Button btnConsultar;
    }
}