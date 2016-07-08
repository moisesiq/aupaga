namespace Refaccionaria.App
{
    partial class DetalleVale
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
            this.gbVale = new System.Windows.Forms.GroupBox();
            this.txtVale = new System.Windows.Forms.TextBox();
            this.lblVale = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.gbVale.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbVale
            // 
            this.gbVale.Controls.Add(this.txtVale);
            this.gbVale.Controls.Add(this.lblVale);
            this.gbVale.Location = new System.Drawing.Point(2, 3);
            this.gbVale.Name = "gbVale";
            this.gbVale.Size = new System.Drawing.Size(279, 130);
            this.gbVale.TabIndex = 0;
            this.gbVale.TabStop = false;
            // 
            // txtVale
            // 
            this.txtVale.Location = new System.Drawing.Point(6, 39);
            this.txtVale.MaxLength = 64;
            this.txtVale.Multiline = true;
            this.txtVale.Name = "txtVale";
            this.txtVale.Size = new System.Drawing.Size(264, 78);
            this.txtVale.TabIndex = 1;
            // 
            // lblVale
            // 
            this.lblVale.AutoSize = true;
            this.lblVale.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblVale.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblVale.Location = new System.Drawing.Point(10, 16);
            this.lblVale.Name = "lblVale";
            this.lblVale.Size = new System.Drawing.Size(131, 13);
            this.lblVale.TabIndex = 0;
            this.lblVale.Text = "Introducir el texto del vale:";
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAceptar.Location = new System.Drawing.Point(125, 143);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 1;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancelar.Location = new System.Drawing.Point(206, 143);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // DetalleVale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(284, 178);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.gbVale);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetalleVale";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Introduce los datos del vale";
            this.gbVale.ResumeLayout(false);
            this.gbVale.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbVale;
        private System.Windows.Forms.TextBox txtVale;
        private System.Windows.Forms.Label lblVale;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
    }
}