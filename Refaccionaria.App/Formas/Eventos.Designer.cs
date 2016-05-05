namespace Refaccionaria.App
{
    partial class Eventos
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
            this.flpEventos = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlMuestra = new System.Windows.Forms.Panel();
            this.lblCambio = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblContacto = new System.Windows.Forms.Label();
            this.lblEtContacto = new System.Windows.Forms.Label();
            this.lblEtAdeudo = new System.Windows.Forms.Label();
            this.lblAdeudo = new System.Windows.Forms.Label();
            this.lblEtVencido = new System.Windows.Forms.Label();
            this.btnRevisado = new System.Windows.Forms.Button();
            this.lblVencido = new System.Windows.Forms.Label();
            this.lblCliente = new System.Windows.Forms.Label();
            this.rdbHoy = new System.Windows.Forms.RadioButton();
            this.rdbManiana = new System.Windows.Forms.RadioButton();
            this.btnReporte = new System.Windows.Forms.Button();
            this.pnlMuestra.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpEventos
            // 
            this.flpEventos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpEventos.AutoScroll = true;
            this.flpEventos.Location = new System.Drawing.Point(12, 59);
            this.flpEventos.Name = "flpEventos";
            this.flpEventos.Size = new System.Drawing.Size(420, 231);
            this.flpEventos.TabIndex = 0;
            // 
            // pnlMuestra
            // 
            this.pnlMuestra.BackColor = System.Drawing.Color.White;
            this.pnlMuestra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMuestra.Controls.Add(this.lblCambio);
            this.pnlMuestra.Controls.Add(this.dtpFecha);
            this.pnlMuestra.Controls.Add(this.lblContacto);
            this.pnlMuestra.Controls.Add(this.lblEtContacto);
            this.pnlMuestra.Controls.Add(this.lblEtAdeudo);
            this.pnlMuestra.Controls.Add(this.lblAdeudo);
            this.pnlMuestra.Controls.Add(this.lblEtVencido);
            this.pnlMuestra.Controls.Add(this.btnRevisado);
            this.pnlMuestra.Controls.Add(this.lblVencido);
            this.pnlMuestra.Controls.Add(this.lblCliente);
            this.pnlMuestra.Location = new System.Drawing.Point(12, 59);
            this.pnlMuestra.Name = "pnlMuestra";
            this.pnlMuestra.Size = new System.Drawing.Size(400, 64);
            this.pnlMuestra.TabIndex = 0;
            this.pnlMuestra.Visible = false;
            // 
            // lblCambio
            // 
            this.lblCambio.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCambio.ForeColor = System.Drawing.Color.Red;
            this.lblCambio.Location = new System.Drawing.Point(372, 15);
            this.lblCambio.Name = "lblCambio";
            this.lblCambio.Size = new System.Drawing.Size(24, 20);
            this.lblCambio.TabIndex = 10;
            this.lblCambio.Text = "*";
            this.lblCambio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCambio.Visible = false;
            // 
            // dtpFecha
            // 
            this.dtpFecha.CustomFormat = "dd/MM/yyyy hh:mm tt";
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.Location = new System.Drawing.Point(0, 0);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(148, 20);
            this.dtpFecha.TabIndex = 9;
            // 
            // lblContacto
            // 
            this.lblContacto.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContacto.Location = new System.Drawing.Point(156, 41);
            this.lblContacto.Name = "lblContacto";
            this.lblContacto.Size = new System.Drawing.Size(209, 19);
            this.lblContacto.TabIndex = 8;
            this.lblContacto.Text = "Nombre del Contacto";
            this.lblContacto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEtContacto
            // 
            this.lblEtContacto.Location = new System.Drawing.Point(153, 20);
            this.lblEtContacto.Name = "lblEtContacto";
            this.lblEtContacto.Size = new System.Drawing.Size(108, 20);
            this.lblEtContacto.TabIndex = 7;
            this.lblEtContacto.Text = "Contacto";
            this.lblEtContacto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEtAdeudo
            // 
            this.lblEtAdeudo.Location = new System.Drawing.Point(0, 40);
            this.lblEtAdeudo.Name = "lblEtAdeudo";
            this.lblEtAdeudo.Size = new System.Drawing.Size(46, 20);
            this.lblEtAdeudo.TabIndex = 6;
            this.lblEtAdeudo.Text = "Adeudo";
            this.lblEtAdeudo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAdeudo
            // 
            this.lblAdeudo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdeudo.Location = new System.Drawing.Point(47, 41);
            this.lblAdeudo.Name = "lblAdeudo";
            this.lblAdeudo.Size = new System.Drawing.Size(100, 19);
            this.lblAdeudo.TabIndex = 5;
            this.lblAdeudo.Text = "$0.00";
            this.lblAdeudo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblEtVencido
            // 
            this.lblEtVencido.Location = new System.Drawing.Point(0, 20);
            this.lblEtVencido.Name = "lblEtVencido";
            this.lblEtVencido.Size = new System.Drawing.Size(46, 20);
            this.lblEtVencido.TabIndex = 4;
            this.lblEtVencido.Text = "Vencido";
            this.lblEtVencido.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRevisado
            // 
            this.btnRevisado.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ok;
            this.btnRevisado.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRevisado.Location = new System.Drawing.Point(371, 35);
            this.btnRevisado.Name = "btnRevisado";
            this.btnRevisado.Size = new System.Drawing.Size(24, 24);
            this.btnRevisado.TabIndex = 3;
            // 
            // lblVencido
            // 
            this.lblVencido.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVencido.Location = new System.Drawing.Point(47, 21);
            this.lblVencido.Name = "lblVencido";
            this.lblVencido.Size = new System.Drawing.Size(100, 19);
            this.lblVencido.TabIndex = 2;
            this.lblVencido.Text = "$0.00";
            this.lblVencido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCliente
            // 
            this.lblCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCliente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.lblCliente.Location = new System.Drawing.Point(153, 0);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(242, 20);
            this.lblCliente.TabIndex = 1;
            this.lblCliente.Text = "Nombre del Cliente";
            this.lblCliente.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rdbHoy
            // 
            this.rdbHoy.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdbHoy.Location = new System.Drawing.Point(12, 12);
            this.rdbHoy.Name = "rdbHoy";
            this.rdbHoy.Size = new System.Drawing.Size(180, 40);
            this.rdbHoy.TabIndex = 1;
            this.rdbHoy.Text = "Hoy";
            this.rdbHoy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdbHoy.UseVisualStyleBackColor = true;
            this.rdbHoy.CheckedChanged += new System.EventHandler(this.rdbHoy_CheckedChanged);
            // 
            // rdbManiana
            // 
            this.rdbManiana.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdbManiana.Location = new System.Drawing.Point(198, 12);
            this.rdbManiana.Name = "rdbManiana";
            this.rdbManiana.Size = new System.Drawing.Size(180, 40);
            this.rdbManiana.TabIndex = 2;
            this.rdbManiana.Text = "Mañana";
            this.rdbManiana.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdbManiana.UseVisualStyleBackColor = true;
            this.rdbManiana.CheckedChanged += new System.EventHandler(this.rdbManiana_CheckedChanged);
            // 
            // btnReporte
            // 
            this.btnReporte.Location = new System.Drawing.Point(384, 12);
            this.btnReporte.Name = "btnReporte";
            this.btnReporte.Size = new System.Drawing.Size(48, 40);
            this.btnReporte.TabIndex = 3;
            this.btnReporte.Text = "Rep";
            this.btnReporte.UseVisualStyleBackColor = true;
            this.btnReporte.Click += new System.EventHandler(this.btnReporte_Click);
            // 
            // Eventos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.ClientSize = new System.Drawing.Size(444, 302);
            this.Controls.Add(this.btnReporte);
            this.Controls.Add(this.rdbManiana);
            this.Controls.Add(this.rdbHoy);
            this.Controls.Add(this.pnlMuestra);
            this.Controls.Add(this.flpEventos);
            this.MaximizeBox = false;
            this.Name = "Eventos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Eventos";
            this.Load += new System.EventHandler(this.Eventos_Load);
            this.pnlMuestra.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpEventos;
        private System.Windows.Forms.Panel pnlMuestra;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Label lblVencido;
        private System.Windows.Forms.Button btnRevisado;
        private System.Windows.Forms.RadioButton rdbHoy;
        private System.Windows.Forms.RadioButton rdbManiana;
        private System.Windows.Forms.Label lblEtVencido;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblContacto;
        private System.Windows.Forms.Label lblEtContacto;
        private System.Windows.Forms.Label lblEtAdeudo;
        private System.Windows.Forms.Label lblAdeudo;
        private System.Windows.Forms.Label lblCambio;
        private System.Windows.Forms.Button btnReporte;
    }
}