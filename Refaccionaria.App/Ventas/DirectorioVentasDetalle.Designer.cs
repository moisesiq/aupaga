namespace Refaccionaria.App
{
    partial class DirectorioVentasDetalle
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectorioVentasDetalle));
            this.lblNombre = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnlDetProveedor = new System.Windows.Forms.Panel();
            this.txObserva = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listContactos = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.pnlDetLinea = new System.Windows.Forms.Panel();
            this.gpoArchDescrio = new System.Windows.Forms.GroupBox();
            this.listArchivos = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilArchivos = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlDetProveedor.SuspendLayout();
            this.pnlDetLinea.SuspendLayout();
            this.gpoArchDescrio.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombre.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblNombre.Location = new System.Drawing.Point(7, 108);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(157, 29);
            this.lblNombre.TabIndex = 100;
            this.lblNombre.Text = "Nombre";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picLogo
            // 
            this.picLogo.ErrorImage = ((System.Drawing.Image)(resources.GetObject("picLogo.ErrorImage")));
            this.picLogo.Location = new System.Drawing.Point(25, 5);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(120, 100);
            this.picLogo.TabIndex = 111;
            this.picLogo.TabStop = false;
            // 
            // pnlDetProveedor
            // 
            this.pnlDetProveedor.Controls.Add(this.txObserva);
            this.pnlDetProveedor.Controls.Add(this.label1);
            this.pnlDetProveedor.Controls.Add(this.listContactos);
            this.pnlDetProveedor.Controls.Add(this.picLogo);
            this.pnlDetProveedor.Controls.Add(this.lblNombre);
            this.pnlDetProveedor.Location = new System.Drawing.Point(42, 0);
            this.pnlDetProveedor.Name = "pnlDetProveedor";
            this.pnlDetProveedor.Size = new System.Drawing.Size(810, 142);
            this.pnlDetProveedor.TabIndex = 112;
            // 
            // txObserva
            // 
            this.txObserva.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txObserva.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txObserva.Location = new System.Drawing.Point(481, 21);
            this.txObserva.Name = "txObserva";
            this.txObserva.Size = new System.Drawing.Size(326, 116);
            this.txObserva.TabIndex = 114;
            this.txObserva.Text = "";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(481, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 113;
            this.label1.Text = "Observaciones:";
            // 
            // listContactos
            // 
            this.listContactos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listContactos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listContactos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listContactos.GridLines = true;
            this.listContactos.Location = new System.Drawing.Point(170, 5);
            this.listContactos.Name = "listContactos";
            this.listContactos.Size = new System.Drawing.Size(305, 132);
            this.listContactos.TabIndex = 112;
            this.listContactos.UseCompatibleStateImageBehavior = false;
            this.listContactos.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Proveedor";
            this.columnHeader5.Width = 150;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Tel Oficina";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Tel Particular";
            this.columnHeader7.Width = 90;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Celular";
            this.columnHeader8.Width = 90;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Correo electrónico";
            this.columnHeader9.Width = 160;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescripcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescripcion.Location = new System.Drawing.Point(6, 11);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(292, 85);
            this.lblDescripcion.TabIndex = 0;
            // 
            // pnlDetLinea
            // 
            this.pnlDetLinea.Controls.Add(this.gpoArchDescrio);
            this.pnlDetLinea.Controls.Add(this.listArchivos);
            this.pnlDetLinea.Location = new System.Drawing.Point(9, 66);
            this.pnlDetLinea.Name = "pnlDetLinea";
            this.pnlDetLinea.Size = new System.Drawing.Size(754, 100);
            this.pnlDetLinea.TabIndex = 112;
            // 
            // gpoArchDescrio
            // 
            this.gpoArchDescrio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpoArchDescrio.Controls.Add(this.lblDescripcion);
            this.gpoArchDescrio.Location = new System.Drawing.Point(453, 0);
            this.gpoArchDescrio.Name = "gpoArchDescrio";
            this.gpoArchDescrio.Size = new System.Drawing.Size(298, 100);
            this.gpoArchDescrio.TabIndex = 1;
            this.gpoArchDescrio.TabStop = false;
            // 
            // listArchivos
            // 
            this.listArchivos.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listArchivos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listArchivos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listArchivos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listArchivos.LargeImageList = this.ilArchivos;
            this.listArchivos.Location = new System.Drawing.Point(3, 16);
            this.listArchivos.MultiSelect = false;
            this.listArchivos.Name = "listArchivos";
            this.listArchivos.Size = new System.Drawing.Size(444, 80);
            this.listArchivos.TabIndex = 0;
            this.listArchivos.UseCompatibleStateImageBehavior = false;
            this.listArchivos.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listArchivos_ItemSelectionChanged);
            this.listArchivos.SelectedIndexChanged += new System.EventHandler(this.listArchivos_SelectedIndexChanged);
            this.listArchivos.DoubleClick += new System.EventHandler(this.listArchivos_DoubleClick);
            // 
            // ilArchivos
            // 
            this.ilArchivos.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilArchivos.ImageStream")));
            this.ilArchivos.TransparentColor = System.Drawing.Color.Transparent;
            this.ilArchivos.Images.SetKeyName(0, "doc");
            this.ilArchivos.Images.SetKeyName(1, "xls");
            this.ilArchivos.Images.SetKeyName(2, "ppt");
            this.ilArchivos.Images.SetKeyName(3, "htm");
            this.ilArchivos.Images.SetKeyName(4, "nulo");
            this.ilArchivos.Images.SetKeyName(5, "pdf");
            this.ilArchivos.Images.SetKeyName(6, "txt");
            this.ilArchivos.Images.SetKeyName(7, "jpg");
            // 
            // DirectorioVentasDetalle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlDetProveedor);
            this.Controls.Add(this.pnlDetLinea);
            this.Name = "DirectorioVentasDetalle";
            this.Size = new System.Drawing.Size(875, 142);
            this.Load += new System.EventHandler(this.DirectorioVentasDetalle_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlDetProveedor.ResumeLayout(false);
            this.pnlDetProveedor.PerformLayout();
            this.pnlDetLinea.ResumeLayout(false);
            this.gpoArchDescrio.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Panel pnlDetProveedor;
        private System.Windows.Forms.Panel pnlDetLinea;
        private System.Windows.Forms.ImageList ilArchivos;
        private System.Windows.Forms.ListView listArchivos;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.GroupBox gpoArchDescrio;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ListView listContactos;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.RichTextBox txObserva;
        private System.Windows.Forms.Label label1;
    }
}