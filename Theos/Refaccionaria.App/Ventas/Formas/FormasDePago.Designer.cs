namespace Refaccionaria.App
{
    partial class FormasDePago
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
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.lsbFormasDePago = new System.Windows.Forms.ListBox();
            this.lsbSeleccionadas = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(168, 126);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 13;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(87, 126);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 12;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // lsbFormasDePago
            // 
            this.lsbFormasDePago.AllowDrop = true;
            this.lsbFormasDePago.FormattingEnabled = true;
            this.lsbFormasDePago.Location = new System.Drawing.Point(12, 12);
            this.lsbFormasDePago.Name = "lsbFormasDePago";
            this.lsbFormasDePago.Size = new System.Drawing.Size(150, 108);
            this.lsbFormasDePago.TabIndex = 14;
            this.lsbFormasDePago.DoubleClick += new System.EventHandler(this.lsbFormasDePago_DoubleClick);
            // 
            // lsbSeleccionadas
            // 
            this.lsbSeleccionadas.AllowDrop = true;
            this.lsbSeleccionadas.FormattingEnabled = true;
            this.lsbSeleccionadas.Location = new System.Drawing.Point(168, 12);
            this.lsbSeleccionadas.Name = "lsbSeleccionadas";
            this.lsbSeleccionadas.Size = new System.Drawing.Size(150, 108);
            this.lsbSeleccionadas.TabIndex = 15;
            this.lsbSeleccionadas.DoubleClick += new System.EventHandler(this.lsbSeleccionadas_DoubleClick);
            // 
            // FormasDePago
            // 
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.ClientSize = new System.Drawing.Size(330, 160);
            this.Controls.Add(this.lsbSeleccionadas);
            this.Controls.Add(this.lsbFormasDePago);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormasDePago";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selección de formas de pago";
            this.Load += new System.EventHandler(this.FormasDePago_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.ListBox lsbFormasDePago;
        private System.Windows.Forms.ListBox lsbSeleccionadas;
    }
}