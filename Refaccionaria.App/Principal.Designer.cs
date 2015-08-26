namespace Refaccionaria.App
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Principal));
            this.statusStripPrincipal = new System.Windows.Forms.StatusStrip();
            this.sslAcc_Calc = new System.Windows.Forms.ToolStripStatusLabel();
            this.sslAcc_Correo = new System.Windows.Forms.ToolStripStatusLabel();
            this.sslAcc_Internet = new System.Windows.Forms.ToolStripStatusLabel();
            this.sslSeparador01 = new System.Windows.Forms.ToolStripStatusLabel();
            this.sslRestante = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblToolTip = new System.Windows.Forms.ToolStripStatusLabel();
            this.sslSeparador02 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnVentas = new Refaccionaria.App.ButtonStripItem();
            this.btnMantenimiento = new Refaccionaria.App.ButtonStripItem();
            this.btnContabilidad = new Refaccionaria.App.ButtonStripItem();
            this.btnReportes = new Refaccionaria.App.ButtonStripItem();
            this.btnAutorizaciones = new Refaccionaria.App.ButtonStripItem();
            this.btnCambiosSistema = new Refaccionaria.App.ButtonStripItem();
            this.btnSesion = new Refaccionaria.App.ButtonStripItem();
            this.btnConfiguracion = new Refaccionaria.App.ButtonStripItem();
            this.sslSeparador03 = new System.Windows.Forms.ToolStripStatusLabel();
            this.sslFecha = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.tmrHora = new System.Windows.Forms.Timer(this.components);
            this.statusStripPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripPrincipal
            // 
            this.statusStripPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sslAcc_Calc,
            this.sslAcc_Correo,
            this.sslAcc_Internet,
            this.sslSeparador01,
            this.sslRestante,
            this.lblToolTip,
            this.sslSeparador02,
            this.btnVentas,
            this.btnMantenimiento,
            this.btnContabilidad,
            this.btnReportes,
            this.btnAutorizaciones,
            this.btnCambiosSistema,
            this.btnSesion,
            this.btnConfiguracion,
            this.sslSeparador03,
            this.sslFecha});
            this.statusStripPrincipal.Location = new System.Drawing.Point(0, 633);
            this.statusStripPrincipal.Name = "statusStripPrincipal";
            this.statusStripPrincipal.Size = new System.Drawing.Size(1008, 30);
            this.statusStripPrincipal.TabIndex = 0;
            this.statusStripPrincipal.Text = "statusStrip";
            // 
            // sslAcc_Calc
            // 
            this.sslAcc_Calc.AutoSize = false;
            this.sslAcc_Calc.BackColor = System.Drawing.Color.Transparent;
            this.sslAcc_Calc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.sslAcc_Calc.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.sslAcc_Calc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sslAcc_Calc.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.sslAcc_Calc.Name = "sslAcc_Calc";
            this.sslAcc_Calc.Size = new System.Drawing.Size(30, 25);
            this.sslAcc_Calc.Tag = "calc";
            this.sslAcc_Calc.ToolTipText = "Calculadora";
            // 
            // sslAcc_Correo
            // 
            this.sslAcc_Correo.AutoSize = false;
            this.sslAcc_Correo.BackColor = System.Drawing.Color.Transparent;
            this.sslAcc_Correo.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogCorreo;
            this.sslAcc_Correo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.sslAcc_Correo.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.sslAcc_Correo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sslAcc_Correo.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.sslAcc_Correo.Name = "sslAcc_Correo";
            this.sslAcc_Correo.Size = new System.Drawing.Size(30, 25);
            this.sslAcc_Correo.Tag = "iexplore https://www.live.com/";
            this.sslAcc_Correo.ToolTipText = "Correo";
            // 
            // sslAcc_Internet
            // 
            this.sslAcc_Internet.AutoSize = false;
            this.sslAcc_Internet.BackColor = System.Drawing.Color.Transparent;
            this.sslAcc_Internet.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogInternet;
            this.sslAcc_Internet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.sslAcc_Internet.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.sslAcc_Internet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sslAcc_Internet.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.sslAcc_Internet.Name = "sslAcc_Internet";
            this.sslAcc_Internet.Size = new System.Drawing.Size(30, 25);
            this.sslAcc_Internet.Tag = "iexplore";
            this.sslAcc_Internet.ToolTipText = "Internet";
            // 
            // sslSeparador01
            // 
            this.sslSeparador01.BackColor = System.Drawing.SystemColors.Control;
            this.sslSeparador01.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sslSeparador01.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.sslSeparador01.Name = "sslSeparador01";
            this.sslSeparador01.Size = new System.Drawing.Size(4, 25);
            // 
            // sslRestante
            // 
            this.sslRestante.BackColor = System.Drawing.SystemColors.Control;
            this.sslRestante.Name = "sslRestante";
            this.sslRestante.Size = new System.Drawing.Size(460, 25);
            this.sslRestante.Spring = true;
            // 
            // lblToolTip
            // 
            this.lblToolTip.BackColor = System.Drawing.SystemColors.Control;
            this.lblToolTip.Name = "lblToolTip";
            this.lblToolTip.Size = new System.Drawing.Size(51, 25);
            this.lblToolTip.Text = "Leyenda";
            // 
            // sslSeparador02
            // 
            this.sslSeparador02.BackColor = System.Drawing.SystemColors.Control;
            this.sslSeparador02.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sslSeparador02.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.sslSeparador02.Name = "sslSeparador02";
            this.sslSeparador02.Size = new System.Drawing.Size(4, 25);
            // 
            // btnVentas
            // 
            this.btnVentas.AutoSize = false;
            this.btnVentas.BackColor = System.Drawing.SystemColors.Control;
            this.btnVentas.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogVentas;
            this.btnVentas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVentas.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnVentas.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.btnVentas.Name = "btnVentas";
            this.btnVentas.Size = new System.Drawing.Size(30, 25);
            this.btnVentas.ToolTipText = "Ventas";
            this.btnVentas.Click += new System.EventHandler(this.btnVentas_Click);
            // 
            // btnMantenimiento
            // 
            this.btnMantenimiento.AutoSize = false;
            this.btnMantenimiento.AutoToolTip = true;
            this.btnMantenimiento.BackColor = System.Drawing.SystemColors.Control;
            this.btnMantenimiento.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogMantenimiento;
            this.btnMantenimiento.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMantenimiento.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnMantenimiento.Margin = new System.Windows.Forms.Padding(1, 3, 4, 2);
            this.btnMantenimiento.Name = "btnMantenimiento";
            this.btnMantenimiento.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnMantenimiento.Size = new System.Drawing.Size(30, 25);
            this.btnMantenimiento.ToolTipText = "Administración";
            this.btnMantenimiento.Click += new System.EventHandler(this.btnMantenimiento_Click);
            // 
            // btnContabilidad
            // 
            this.btnContabilidad.AutoSize = false;
            this.btnContabilidad.BackColor = System.Drawing.SystemColors.Control;
            this.btnContabilidad.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogContabilidad;
            this.btnContabilidad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnContabilidad.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnContabilidad.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.btnContabilidad.Name = "btnContabilidad";
            this.btnContabilidad.Size = new System.Drawing.Size(30, 25);
            this.btnContabilidad.ToolTipText = "Contabilidad";
            this.btnContabilidad.Click += new System.EventHandler(this.btnContabilidad_Click);
            // 
            // btnReportes
            // 
            this.btnReportes.AutoSize = false;
            this.btnReportes.BackColor = System.Drawing.SystemColors.Control;
            this.btnReportes.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogReportes;
            this.btnReportes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReportes.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnReportes.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(30, 25);
            this.btnReportes.ToolTipText = "Cuadro de Control";
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            // 
            // btnAutorizaciones
            // 
            this.btnAutorizaciones.AutoSize = false;
            this.btnAutorizaciones.BackColor = System.Drawing.SystemColors.Control;
            this.btnAutorizaciones.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogAutorizaciones;
            this.btnAutorizaciones.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAutorizaciones.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnAutorizaciones.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.btnAutorizaciones.Name = "btnAutorizaciones";
            this.btnAutorizaciones.Size = new System.Drawing.Size(30, 25);
            this.btnAutorizaciones.ToolTipText = "Autorizaciones";
            this.btnAutorizaciones.Click += new System.EventHandler(this.btnAutorizaciones_Click);
            // 
            // btnCambiosSistema
            // 
            this.btnCambiosSistema.AutoSize = false;
            this.btnCambiosSistema.BackColor = System.Drawing.SystemColors.Control;
            this.btnCambiosSistema.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogCambiosSistema;
            this.btnCambiosSistema.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCambiosSistema.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnCambiosSistema.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.btnCambiosSistema.Name = "btnCambiosSistema";
            this.btnCambiosSistema.Size = new System.Drawing.Size(30, 25);
            this.btnCambiosSistema.ToolTipText = "Bitácora de Cambios";
            this.btnCambiosSistema.Click += new System.EventHandler(this.btnCambiosSistema_Click);
            // 
            // btnSesion
            // 
            this.btnSesion.AutoSize = false;
            this.btnSesion.BackColor = System.Drawing.SystemColors.Control;
            this.btnSesion.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogSesion;
            this.btnSesion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSesion.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnSesion.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.btnSesion.Name = "btnSesion";
            this.btnSesion.Size = new System.Drawing.Size(30, 25);
            this.btnSesion.ToolTipText = "Bloquear Sesión";
            this.btnSesion.Click += new System.EventHandler(this.btnSesion_Click);
            // 
            // btnConfiguracion
            // 
            this.btnConfiguracion.AutoSize = false;
            this.btnConfiguracion.BackColor = System.Drawing.SystemColors.Control;
            this.btnConfiguracion.BackgroundImage = global::Refaccionaria.App.Properties.Resources.ogConfiguracion;
            this.btnConfiguracion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConfiguracion.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnConfiguracion.Margin = new System.Windows.Forms.Padding(0, 3, 4, 2);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(30, 25);
            this.btnConfiguracion.ToolTipText = "Configuración";
            this.btnConfiguracion.Click += new System.EventHandler(this.btnConfiguracion_Click);
            // 
            // sslSeparador03
            // 
            this.sslSeparador03.BackColor = System.Drawing.SystemColors.Control;
            this.sslSeparador03.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.sslSeparador03.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.sslSeparador03.Name = "sslSeparador03";
            this.sslSeparador03.Size = new System.Drawing.Size(4, 25);
            // 
            // sslFecha
            // 
            this.sslFecha.BackColor = System.Drawing.SystemColors.Control;
            this.sslFecha.Name = "sslFecha";
            this.sslFecha.Size = new System.Drawing.Size(95, 25);
            this.sslFecha.Text = "06/04/2015 11:07";
            this.sslFecha.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelContenedor
            // 
            this.panelContenedor.BackgroundImage = global::Refaccionaria.App.Properties.Resources.Fondo_AutopartesGaribaldi;
            this.panelContenedor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Location = new System.Drawing.Point(0, 0);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(1008, 633);
            this.panelContenedor.TabIndex = 1;
            // 
            // tmrHora
            // 
            this.tmrHora.Interval = 1000;
            this.tmrHora.Tick += new System.EventHandler(this.tmrHora_Tick);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.ClientSize = new System.Drawing.Size(1008, 663);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.statusStripPrincipal);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Principal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Principal_Load);
            this.statusStripPrincipal.ResumeLayout(false);
            this.statusStripPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripPrincipal;
        private System.Windows.Forms.Panel panelContenedor;
        private ButtonStripItem btnMantenimiento;
        private System.Windows.Forms.ToolStripStatusLabel lblToolTip;
        private ButtonStripItem btnVentas;
        private ButtonStripItem btnAutorizaciones;
        private ButtonStripItem btnCambiosSistema;
        private ButtonStripItem btnSesion;
        private ButtonStripItem btnContabilidad;
        private ButtonStripItem btnReportes;
        private ButtonStripItem btnConfiguracion;
        private System.Windows.Forms.ToolStripStatusLabel sslRestante;
        private System.Windows.Forms.ToolStripStatusLabel sslFecha;
        private System.Windows.Forms.ToolStripStatusLabel sslAcc_Internet;
        private System.Windows.Forms.ToolStripStatusLabel sslSeparador01;
        private System.Windows.Forms.ToolStripStatusLabel sslSeparador02;
        private System.Windows.Forms.ToolStripStatusLabel sslSeparador03;
        private System.Windows.Forms.ToolStripStatusLabel sslAcc_Calc;
        private System.Windows.Forms.ToolStripStatusLabel sslAcc_Correo;
        private System.Windows.Forms.Timer tmrHora;

    }
}