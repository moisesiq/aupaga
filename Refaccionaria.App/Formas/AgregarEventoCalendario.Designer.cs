namespace Refaccionaria.App
{
    partial class AgregarEventoCalendario
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
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblCobrarCreditoHrs = new System.Windows.Forms.Label();
            this.lblCobrarCredito = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.tbNombre = new System.Windows.Forms.TextBox();
            this.dtpHoraCobro = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaEvento = new System.Windows.Forms.DateTimePicker();
            this.tbDescripcion = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblNombre.Location = new System.Drawing.Point(15, 27);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(47, 13);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre:";
            // 
            // lblCobrarCreditoHrs
            // 
            this.lblCobrarCreditoHrs.AutoSize = true;
            this.lblCobrarCreditoHrs.BackColor = System.Drawing.Color.Transparent;
            this.lblCobrarCreditoHrs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCobrarCreditoHrs.ForeColor = System.Drawing.Color.White;
            this.lblCobrarCreditoHrs.Location = new System.Drawing.Point(18, 80);
            this.lblCobrarCreditoHrs.Name = "lblCobrarCreditoHrs";
            this.lblCobrarCreditoHrs.Size = new System.Drawing.Size(33, 13);
            this.lblCobrarCreditoHrs.TabIndex = 185;
            this.lblCobrarCreditoHrs.Text = "Hora:";
            // 
            // lblCobrarCredito
            // 
            this.lblCobrarCredito.AutoSize = true;
            this.lblCobrarCredito.BackColor = System.Drawing.Color.Transparent;
            this.lblCobrarCredito.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCobrarCredito.ForeColor = System.Drawing.Color.White;
            this.lblCobrarCredito.Location = new System.Drawing.Point(15, 56);
            this.lblCobrarCredito.Name = "lblCobrarCredito";
            this.lblCobrarCredito.Size = new System.Drawing.Size(94, 13);
            this.lblCobrarCredito.TabIndex = 183;
            this.lblCobrarCredito.Text = "Fecha del Evento:";
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnAceptar.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(252, 227);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 23);
            this.btnAceptar.TabIndex = 188;
            this.btnAceptar.Text = "&Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(63)))), ((int)(((byte)(87)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(333, 227);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 189;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // tbNombre
            // 
            this.tbNombre.Enabled = false;
            this.tbNombre.Location = new System.Drawing.Point(109, 19);
            this.tbNombre.Name = "tbNombre";
            this.tbNombre.Size = new System.Drawing.Size(299, 20);
            this.tbNombre.TabIndex = 190;
            // 
            // dtpHoraCobro
            // 
            this.dtpHoraCobro.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraCobro.Location = new System.Drawing.Point(109, 80);
            this.dtpHoraCobro.Name = "dtpHoraCobro";
            this.dtpHoraCobro.ShowUpDown = true;
            this.dtpHoraCobro.Size = new System.Drawing.Size(119, 20);
            this.dtpHoraCobro.TabIndex = 192;
            // 
            // dtpFechaEvento
            // 
            this.dtpFechaEvento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaEvento.Location = new System.Drawing.Point(109, 50);
            this.dtpFechaEvento.Name = "dtpFechaEvento";
            this.dtpFechaEvento.Size = new System.Drawing.Size(120, 20);
            this.dtpFechaEvento.TabIndex = 193;
            // 
            // tbDescripcion
            // 
            this.tbDescripcion.Location = new System.Drawing.Point(21, 131);
            this.tbDescripcion.Multiline = true;
            this.tbDescripcion.Name = "tbDescripcion";
            this.tbDescripcion.Size = new System.Drawing.Size(387, 76);
            this.tbDescripcion.TabIndex = 194;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.BackColor = System.Drawing.Color.Transparent;
            this.lblDescripcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescripcion.ForeColor = System.Drawing.Color.White;
            this.lblDescripcion.Location = new System.Drawing.Point(18, 115);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(116, 13);
            this.lblDescripcion.TabIndex = 195;
            this.lblDescripcion.Text = "Descripción del evento";
            // 
            // AgregarEventoCalendario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(420, 262);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.tbDescripcion);
            this.Controls.Add(this.dtpFechaEvento);
            this.Controls.Add(this.dtpHoraCobro);
            this.Controls.Add(this.tbNombre);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.lblCobrarCreditoHrs);
            this.Controls.Add(this.lblCobrarCredito);
            this.Controls.Add(this.lblNombre);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgregarEventoCalendario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Agregar Evento Calendario";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblCobrarCreditoHrs;
        private System.Windows.Forms.Label lblCobrarCredito;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.TextBox tbNombre;
        private System.Windows.Forms.DateTimePicker dtpHoraCobro;
        private System.Windows.Forms.DateTimePicker dtpFechaEvento;
        private System.Windows.Forms.TextBox tbDescripcion;
        private System.Windows.Forms.Label lblDescripcion;
    }
}