namespace Refaccionaria.App
{
    partial class CargaImgMarcaLinea
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
            this.components = new System.ComponentModel.Container();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnImportar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.txtRuta = new Refaccionaria.Negocio.TextoMod();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblArchivo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FaltanDatos = new System.Windows.Forms.ErrorProvider(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.pEdicion = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.pMostrar = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.FaltanDatos)).BeginInit();
            this.pEdicion.SuspendLayout();
            this.pMostrar.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(439, 217);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 21;
            this.btnCancelar.Text = "&Cerrar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnImportar
            // 
            this.btnImportar.Location = new System.Drawing.Point(325, 138);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(83, 23);
            this.btnImportar.TabIndex = 20;
            this.btnImportar.Text = "Agregar";
            this.btnImportar.UseVisualStyleBackColor = true;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click_1);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(0, 0);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(104, 23);
            this.btnBuscar.TabIndex = 18;
            this.btnBuscar.Text = "Incluir Archivo...";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // txtRuta
            // 
            this.txtRuta.Etiqueta = "vínculo web";
            this.txtRuta.EtiquetaColor = System.Drawing.Color.Gray;
            this.txtRuta.Location = new System.Drawing.Point(181, 32);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.PasarEnfoqueConEnter = true;
            this.txtRuta.SeleccionarTextoAlEnfoque = false;
            this.txtRuta.Size = new System.Drawing.Size(333, 20);
            this.txtRuta.TabIndex = 17;
            this.txtRuta.TextChanged += new System.EventHandler(this.txtRuta_TextChanged);
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDescripcion.Location = new System.Drawing.Point(69, 61);
            this.txtDescripcion.MaxLength = 350;
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(445, 71);
            this.txtDescripcion.TabIndex = 22;
            this.txtDescripcion.TextChanged += new System.EventHandler(this.txtDescripcion_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Descripción:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "Página web";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblArchivo
            // 
            this.lblArchivo.AutoSize = true;
            this.lblArchivo.ForeColor = System.Drawing.Color.White;
            this.lblArchivo.Location = new System.Drawing.Point(110, 5);
            this.lblArchivo.Name = "lblArchivo";
            this.lblArchivo.Size = new System.Drawing.Size(49, 13);
            this.lblArchivo.TabIndex = 25;
            this.lblArchivo.Text = "_______";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(110, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 16);
            this.label3.TabIndex = 25;
            this.label3.Text = "http://www.";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(514, 211);
            this.listView1.TabIndex = 26;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Nombre de Archivo";
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Descripción";
            this.columnHeader3.Width = 325;
            // 
            // FaltanDatos
            // 
            this.FaltanDatos.ContainerControl = this;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(271, 217);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "&Nuevo";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pEdicion
            // 
            this.pEdicion.Controls.Add(this.button3);
            this.pEdicion.Controls.Add(this.txtDescripcion);
            this.pEdicion.Controls.Add(this.txtRuta);
            this.pEdicion.Controls.Add(this.btnImportar);
            this.pEdicion.Controls.Add(this.button1);
            this.pEdicion.Controls.Add(this.label3);
            this.pEdicion.Controls.Add(this.lblArchivo);
            this.pEdicion.Controls.Add(this.label1);
            this.pEdicion.Controls.Add(this.btnBuscar);
            this.pEdicion.Location = new System.Drawing.Point(11, 13);
            this.pEdicion.Name = "pEdicion";
            this.pEdicion.Size = new System.Drawing.Size(514, 168);
            this.pEdicion.TabIndex = 27;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(439, 138);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 26;
            this.button3.Text = "Cancelar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pMostrar
            // 
            this.pMostrar.Controls.Add(this.button4);
            this.pMostrar.Controls.Add(this.listView1);
            this.pMostrar.Controls.Add(this.btnCancelar);
            this.pMostrar.Controls.Add(this.button2);
            this.pMostrar.Location = new System.Drawing.Point(11, 187);
            this.pMostrar.Name = "pMostrar";
            this.pMostrar.Size = new System.Drawing.Size(514, 248);
            this.pMostrar.TabIndex = 0;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(355, 217);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 27;
            this.button4.Text = "Editar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // CargaImgMarcaLinea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.Controls.Add(this.pMostrar);
            this.Controls.Add(this.pEdicion);
            this.Name = "CargaImgMarcaLinea";
            this.Size = new System.Drawing.Size(524, 258);
            this.Load += new System.EventHandler(this.CargaImgMarcaLinea_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FaltanDatos)).EndInit();
            this.pEdicion.ResumeLayout(false);
            this.pEdicion.PerformLayout();
            this.pMostrar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnImportar;
        private System.Windows.Forms.Button btnBuscar;
        private Negocio.TextoMod txtRuta;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblArchivo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ErrorProvider FaltanDatos;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel pMostrar;
        private System.Windows.Forms.Panel pEdicion;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}
