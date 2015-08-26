namespace Refaccionaria.Negocio
{
    partial class TextoAutocompletar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtBusqueda = new Refaccionaria.Negocio.TextoMod();
            this.dgvDatos = new System.Windows.Forms.DataGridView();
            this.btnMostrarAutocompletar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBusqueda.Etiqueta = "";
            this.txtBusqueda.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtBusqueda.Location = new System.Drawing.Point(0, 0);
            this.txtBusqueda.Margin = new System.Windows.Forms.Padding(0);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.PasarEnfoqueConEnter = true;
            this.txtBusqueda.SeleccionarTextoAlEnfoque = false;
            this.txtBusqueda.Size = new System.Drawing.Size(182, 20);
            this.txtBusqueda.TabIndex = 0;
            this.txtBusqueda.Click += new System.EventHandler(this.txtBusqueda_Click);
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            this.txtBusqueda.Enter += new System.EventHandler(this.txtBusqueda_Enter);
            this.txtBusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBusqueda_KeyDown);
            // 
            // dgvDatos
            // 
            this.dgvDatos.AllowUserToAddRows = false;
            this.dgvDatos.AllowUserToDeleteRows = false;
            this.dgvDatos.AllowUserToResizeColumns = false;
            this.dgvDatos.AllowUserToResizeRows = false;
            this.dgvDatos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatos.BackgroundColor = System.Drawing.Color.White;
            this.dgvDatos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvDatos.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDatos.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDatos.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDatos.Location = new System.Drawing.Point(0, 21);
            this.dgvDatos.Margin = new System.Windows.Forms.Padding(0);
            this.dgvDatos.MultiSelect = false;
            this.dgvDatos.Name = "dgvDatos";
            this.dgvDatos.ReadOnly = true;
            this.dgvDatos.RowHeadersVisible = false;
            this.dgvDatos.RowTemplate.Height = 16;
            this.dgvDatos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDatos.Size = new System.Drawing.Size(200, 200);
            this.dgvDatos.StandardTab = true;
            this.dgvDatos.TabIndex = 2;
            this.dgvDatos.Visible = false;
            this.dgvDatos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDatos_CellClick);
            this.dgvDatos.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDatos_CellMouseEnter);
            this.dgvDatos.CurrentCellChanged += new System.EventHandler(this.dgvDatos_CurrentCellChanged);
            this.dgvDatos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDatos_KeyDown);
            // 
            // btnMostrarAutocompletar
            // 
            this.btnMostrarAutocompletar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMostrarAutocompletar.BackColor = System.Drawing.Color.White;
            this.btnMostrarAutocompletar.FlatAppearance.BorderSize = 0;
            this.btnMostrarAutocompletar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMostrarAutocompletar.ForeColor = System.Drawing.Color.Black;
            this.btnMostrarAutocompletar.Location = new System.Drawing.Point(183, 1);
            this.btnMostrarAutocompletar.Margin = new System.Windows.Forms.Padding(0);
            this.btnMostrarAutocompletar.Name = "btnMostrarAutocompletar";
            this.btnMostrarAutocompletar.Size = new System.Drawing.Size(16, 18);
            this.btnMostrarAutocompletar.TabIndex = 1;
            this.btnMostrarAutocompletar.TabStop = false;
            this.btnMostrarAutocompletar.Text = "V";
            this.btnMostrarAutocompletar.UseVisualStyleBackColor = false;
            this.btnMostrarAutocompletar.Click += new System.EventHandler(this.btnMostrarAutocompletar_Click);
            // 
            // TextoAutocompletar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.Controls.Add(this.btnMostrarAutocompletar);
            this.Controls.Add(this.dgvDatos);
            this.Controls.Add(this.txtBusqueda);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TextoAutocompletar";
            this.Size = new System.Drawing.Size(200, 221);
            this.Leave += new System.EventHandler(this.TextoAutocompletar_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Refaccionaria.Negocio.TextoMod txtBusqueda;
        private System.Windows.Forms.DataGridView dgvDatos;
        private System.Windows.Forms.Button btnMostrarAutocompletar;
    }
}
