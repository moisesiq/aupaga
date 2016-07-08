namespace Refaccionaria.App
{
    partial class ReportarErrorParte
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
            this.chkFoto = new System.Windows.Forms.CheckBox();
            this.chkEquivalente = new System.Windows.Forms.CheckBox();
            this.chkAlterno = new System.Windows.Forms.CheckBox();
            this.chkAplicacion = new System.Windows.Forms.CheckBox();
            this.chkComplemento = new System.Windows.Forms.CheckBox();
            this.chkOtro = new System.Windows.Forms.CheckBox();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.txtComentario = new LibUtil.TextoMod();
            this.SuspendLayout();
            // 
            // chkFoto
            // 
            this.chkFoto.AutoSize = true;
            this.chkFoto.ForeColor = System.Drawing.Color.White;
            this.chkFoto.Location = new System.Drawing.Point(12, 12);
            this.chkFoto.Name = "chkFoto";
            this.chkFoto.Size = new System.Drawing.Size(47, 17);
            this.chkFoto.TabIndex = 0;
            this.chkFoto.Text = "Foto";
            this.chkFoto.UseVisualStyleBackColor = true;
            // 
            // chkEquivalente
            // 
            this.chkEquivalente.AutoSize = true;
            this.chkEquivalente.ForeColor = System.Drawing.Color.White;
            this.chkEquivalente.Location = new System.Drawing.Point(12, 35);
            this.chkEquivalente.Name = "chkEquivalente";
            this.chkEquivalente.Size = new System.Drawing.Size(82, 17);
            this.chkEquivalente.TabIndex = 1;
            this.chkEquivalente.Text = "Equivalente";
            this.chkEquivalente.UseVisualStyleBackColor = true;
            // 
            // chkAlterno
            // 
            this.chkAlterno.AutoSize = true;
            this.chkAlterno.ForeColor = System.Drawing.Color.White;
            this.chkAlterno.Location = new System.Drawing.Point(117, 12);
            this.chkAlterno.Name = "chkAlterno";
            this.chkAlterno.Size = new System.Drawing.Size(94, 17);
            this.chkAlterno.TabIndex = 3;
            this.chkAlterno.Text = "Código alterno";
            this.chkAlterno.UseVisualStyleBackColor = true;
            // 
            // chkAplicacion
            // 
            this.chkAplicacion.AutoSize = true;
            this.chkAplicacion.ForeColor = System.Drawing.Color.White;
            this.chkAplicacion.Location = new System.Drawing.Point(12, 58);
            this.chkAplicacion.Name = "chkAplicacion";
            this.chkAplicacion.Size = new System.Drawing.Size(75, 17);
            this.chkAplicacion.TabIndex = 2;
            this.chkAplicacion.Text = "Aplicación";
            this.chkAplicacion.UseVisualStyleBackColor = true;
            // 
            // chkComplemento
            // 
            this.chkComplemento.AutoSize = true;
            this.chkComplemento.ForeColor = System.Drawing.Color.White;
            this.chkComplemento.Location = new System.Drawing.Point(117, 35);
            this.chkComplemento.Name = "chkComplemento";
            this.chkComplemento.Size = new System.Drawing.Size(90, 17);
            this.chkComplemento.TabIndex = 4;
            this.chkComplemento.Text = "Complemento";
            this.chkComplemento.UseVisualStyleBackColor = true;
            // 
            // chkOtro
            // 
            this.chkOtro.AutoSize = true;
            this.chkOtro.ForeColor = System.Drawing.Color.White;
            this.chkOtro.Location = new System.Drawing.Point(117, 58);
            this.chkOtro.Name = "chkOtro";
            this.chkOtro.Size = new System.Drawing.Size(46, 17);
            this.chkOtro.TabIndex = 5;
            this.chkOtro.Text = "Otro";
            this.chkOtro.UseVisualStyleBackColor = true;
            // 
            // btnAceptar
            // 
            this.btnAceptar.Location = new System.Drawing.Point(56, 147);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 7;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(137, 147);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 8;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // txtComentario
            // 
            this.txtComentario.Etiqueta = "Comentario";
            this.txtComentario.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtComentario.Location = new System.Drawing.Point(12, 81);
            this.txtComentario.Multiline = true;
            this.txtComentario.Name = "txtComentario";
            this.txtComentario.PasarEnfoqueConEnter = true;
            this.txtComentario.SeleccionarTextoAlEnfoque = false;
            this.txtComentario.Size = new System.Drawing.Size(200, 60);
            this.txtComentario.TabIndex = 6;
            // 
            // ReportarErrorParte
            // 
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(224, 181);
            this.Controls.Add(this.txtComentario);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.chkOtro);
            this.Controls.Add(this.chkComplemento);
            this.Controls.Add(this.chkAplicacion);
            this.Controls.Add(this.chkAlterno);
            this.Controls.Add(this.chkEquivalente);
            this.Controls.Add(this.chkFoto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportarErrorParte";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reportar error en artículo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkFoto;
        private System.Windows.Forms.CheckBox chkEquivalente;
        private System.Windows.Forms.CheckBox chkAlterno;
        private System.Windows.Forms.CheckBox chkAplicacion;
        private System.Windows.Forms.CheckBox chkComplemento;
        private System.Windows.Forms.CheckBox chkOtro;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
        private LibUtil.TextoMod txtComentario;
    }
}