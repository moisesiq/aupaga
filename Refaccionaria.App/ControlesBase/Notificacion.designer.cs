namespace Refaccionaria.App
{
    partial class Notificacion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Notificacion));
            this.pcbIcono = new System.Windows.Forms.PictureBox();
            this.txtMensaje = new System.Windows.Forms.TextBox();
            this.tmrCerrar = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pcbIcono)).BeginInit();
            this.SuspendLayout();
            // 
            // pcbIcono
            // 
            this.pcbIcono.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pcbIcono.ErrorImage")));
            this.pcbIcono.Image = ((System.Drawing.Image)(resources.GetObject("pcbIcono.Image")));
            this.pcbIcono.InitialImage = ((System.Drawing.Image)(resources.GetObject("pcbIcono.InitialImage")));
            this.pcbIcono.Location = new System.Drawing.Point(12, 12);
            this.pcbIcono.Name = "pcbIcono";
            this.pcbIcono.Size = new System.Drawing.Size(48, 48);
            this.pcbIcono.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcbIcono.TabIndex = 1;
            this.pcbIcono.TabStop = false;
            // 
            // txtMensaje
            // 
            this.txtMensaje.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.txtMensaje.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMensaje.Location = new System.Drawing.Point(69, 31);
            this.txtMensaje.Multiline = true;
            this.txtMensaje.Name = "txtMensaje";
            this.txtMensaje.ReadOnly = true;
            this.txtMensaje.Size = new System.Drawing.Size(239, 29);
            this.txtMensaje.TabIndex = 2;
            this.txtMensaje.TabStop = false;
            this.txtMensaje.Text = "Mensaje";
            // 
            // tmrCerrar
            // 
            this.tmrCerrar.Tick += new System.EventHandler(this.tmrCerrar_Tick);
            // 
            // Notificacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(199)))), ((int)(((byte)(216)))));
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(320, 72);
            this.Controls.Add(this.txtMensaje);
            this.Controls.Add(this.pcbIcono);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Notificacion";
            this.ShowInTaskbar = false;
            this.Text = "Notificación";
            this.Load += new System.EventHandler(this.Notificacion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pcbIcono)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pcbIcono;
        public System.Windows.Forms.TextBox txtMensaje;
        private System.Windows.Forms.Timer tmrCerrar;

    }
}