namespace Refaccionaria.App
{
    partial class VerImagenesParte
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
            this.pcbParte = new System.Windows.Forms.PictureBox();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnPrimera = new System.Windows.Forms.Button();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.btnSiguiente = new System.Windows.Forms.Button();
            this.btnUltima = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtImagen = new System.Windows.Forms.TextBox();
            this.txtNumeroDeImagenes = new System.Windows.Forms.TextBox();
            this.pnlBotones = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pcbParte)).BeginInit();
            this.pnlBotones.SuspendLayout();
            this.SuspendLayout();
            // 
            // pcbParte
            // 
            this.pcbParte.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pcbParte.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.pcbParte.Location = new System.Drawing.Point(-1, -1);
            this.pcbParte.Name = "pcbParte";
            this.pcbParte.Size = new System.Drawing.Size(588, 441);
            this.pcbParte.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcbParte.TabIndex = 0;
            this.pcbParte.TabStop = false;
            // 
            // lblMensaje
            // 
            this.lblMensaje.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMensaje.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.lblMensaje.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblMensaje.Location = new System.Drawing.Point(12, 12);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(560, 220);
            this.lblMensaje.TabIndex = 8;
            this.lblMensaje.Text = "La parte seleccionada no tiene ninguna imagen.";
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMensaje.Visible = false;
            // 
            // btnPrimera
            // 
            this.btnPrimera.BackColor = System.Drawing.Color.Transparent;
            this.btnPrimera.BackgroundImage = global::Refaccionaria.App.Properties.Resources.BotonPrimera;
            this.btnPrimera.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrimera.FlatAppearance.BorderSize = 0;
            this.btnPrimera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrimera.ForeColor = System.Drawing.Color.Transparent;
            this.btnPrimera.Location = new System.Drawing.Point(0, 0);
            this.btnPrimera.Name = "btnPrimera";
            this.btnPrimera.Size = new System.Drawing.Size(40, 30);
            this.btnPrimera.TabIndex = 0;
            this.btnPrimera.UseVisualStyleBackColor = false;
            this.btnPrimera.Click += new System.EventHandler(this.btnPrimera_Click);
            // 
            // btnAnterior
            // 
            this.btnAnterior.BackColor = System.Drawing.Color.Transparent;
            this.btnAnterior.BackgroundImage = global::Refaccionaria.App.Properties.Resources.BotonAnterior;
            this.btnAnterior.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAnterior.FlatAppearance.BorderSize = 0;
            this.btnAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnterior.ForeColor = System.Drawing.Color.Transparent;
            this.btnAnterior.Location = new System.Drawing.Point(46, 0);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(40, 30);
            this.btnAnterior.TabIndex = 1;
            this.btnAnterior.UseVisualStyleBackColor = false;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            // 
            // btnSiguiente
            // 
            this.btnSiguiente.BackColor = System.Drawing.Color.Transparent;
            this.btnSiguiente.BackgroundImage = global::Refaccionaria.App.Properties.Resources.BotonSiguiente;
            this.btnSiguiente.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSiguiente.FlatAppearance.BorderSize = 0;
            this.btnSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSiguiente.ForeColor = System.Drawing.Color.Transparent;
            this.btnSiguiente.Location = new System.Drawing.Point(204, 0);
            this.btnSiguiente.Name = "btnSiguiente";
            this.btnSiguiente.Size = new System.Drawing.Size(40, 30);
            this.btnSiguiente.TabIndex = 6;
            this.btnSiguiente.UseVisualStyleBackColor = false;
            this.btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            // 
            // btnUltima
            // 
            this.btnUltima.BackColor = System.Drawing.Color.Transparent;
            this.btnUltima.BackgroundImage = global::Refaccionaria.App.Properties.Resources.BotonUltima;
            this.btnUltima.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUltima.FlatAppearance.BorderSize = 0;
            this.btnUltima.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUltima.ForeColor = System.Drawing.Color.Transparent;
            this.btnUltima.Location = new System.Drawing.Point(250, 0);
            this.btnUltima.Name = "btnUltima";
            this.btnUltima.Size = new System.Drawing.Size(40, 30);
            this.btnUltima.TabIndex = 7;
            this.btnUltima.UseVisualStyleBackColor = false;
            this.btnUltima.Click += new System.EventHandler(this.btnUltima_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(154, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "de";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(92, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Imagen";
            // 
            // txtImagen
            // 
            this.txtImagen.BackColor = System.Drawing.SystemColors.Control;
            this.txtImagen.Location = new System.Drawing.Point(135, 6);
            this.txtImagen.Name = "txtImagen";
            this.txtImagen.Size = new System.Drawing.Size(17, 20);
            this.txtImagen.TabIndex = 3;
            this.txtImagen.TextChanged += new System.EventHandler(this.txtImagen_TextChanged);
            this.txtImagen.Enter += new System.EventHandler(this.txtImagen_Enter);
            // 
            // txtNumeroDeImagenes
            // 
            this.txtNumeroDeImagenes.BackColor = System.Drawing.SystemColors.Control;
            this.txtNumeroDeImagenes.Location = new System.Drawing.Point(174, 6);
            this.txtNumeroDeImagenes.Name = "txtNumeroDeImagenes";
            this.txtNumeroDeImagenes.ReadOnly = true;
            this.txtNumeroDeImagenes.Size = new System.Drawing.Size(24, 20);
            this.txtNumeroDeImagenes.TabIndex = 5;
            // 
            // pnlBotones
            // 
            this.pnlBotones.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pnlBotones.BackColor = System.Drawing.Color.Transparent;
            this.pnlBotones.Controls.Add(this.btnPrimera);
            this.pnlBotones.Controls.Add(this.txtNumeroDeImagenes);
            this.pnlBotones.Controls.Add(this.btnAnterior);
            this.pnlBotones.Controls.Add(this.txtImagen);
            this.pnlBotones.Controls.Add(this.btnSiguiente);
            this.pnlBotones.Controls.Add(this.label2);
            this.pnlBotones.Controls.Add(this.btnUltima);
            this.pnlBotones.Controls.Add(this.label1);
            this.pnlBotones.Location = new System.Drawing.Point(146, 399);
            this.pnlBotones.Name = "pnlBotones";
            this.pnlBotones.Size = new System.Drawing.Size(292, 32);
            this.pnlBotones.TabIndex = 9;
            // 
            // VerImagenesParte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 439);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.pcbParte);
            this.MinimizeBox = false;
            this.Name = "VerImagenesParte";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.VerImagenesParte_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pcbParte)).EndInit();
            this.pnlBotones.ResumeLayout(false);
            this.pnlBotones.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pcbParte;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnPrimera;
        private System.Windows.Forms.Button btnAnterior;
        private System.Windows.Forms.Button btnSiguiente;
        private System.Windows.Forms.Button btnUltima;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtImagen;
        private System.Windows.Forms.TextBox txtNumeroDeImagenes;
        private System.Windows.Forms.Panel pnlBotones;
    }
}