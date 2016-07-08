namespace Refaccionaria.App
{
    partial class DirectorioVentas
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
            this.listProvee = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblMensajeBusqueda = new System.Windows.Forms.Label();
            this.tabDirectorio = new System.Windows.Forms.TabControl();
            this.PageProvee = new System.Windows.Forms.TabPage();
            this.tjContactos = new Refaccionaria.App.Tarjetas();
            this.PageLinea = new System.Windows.Forms.TabPage();
            this.listLinea = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PageMarcas = new System.Windows.Forms.TabPage();
            this.listMarca = new System.Windows.Forms.ListView();
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.StripMenuItemProveedores = new System.Windows.Forms.ToolStripMenuItem();
            this.StripMenuItemLineas = new System.Windows.Forms.ToolStripMenuItem();
            this.StripMenuItemMarcas = new System.Windows.Forms.ToolStripMenuItem();
            this.tabDirectorio.SuspendLayout();
            this.PageProvee.SuspendLayout();
            this.PageLinea.SuspendLayout();
            this.PageMarcas.SuspendLayout();
            this.ctMnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // listProvee
            // 
            this.listProvee.BackColor = System.Drawing.Color.Ivory;
            this.listProvee.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listProvee.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader10,
            this.columnHeader11});
            this.listProvee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProvee.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listProvee.ForeColor = System.Drawing.Color.SteelBlue;
            this.listProvee.FullRowSelect = true;
            this.listProvee.Location = new System.Drawing.Point(3, 3);
            this.listProvee.MultiSelect = false;
            this.listProvee.Name = "listProvee";
            this.listProvee.Size = new System.Drawing.Size(639, 496);
            this.listProvee.TabIndex = 1;
            this.listProvee.UseCompatibleStateImageBehavior = false;
            this.listProvee.View = System.Windows.Forms.View.Details;
            this.listProvee.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listProvee_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Proveedor";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Contacto";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Teléfono";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Teléfono";
            this.columnHeader5.Width = 120;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Teléfono";
            this.columnHeader10.Width = 120;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Página Web";
            this.columnHeader11.Width = 260;
            // 
            // lblMensajeBusqueda
            // 
            this.lblMensajeBusqueda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMensajeBusqueda.BackColor = System.Drawing.Color.Ivory;
            this.lblMensajeBusqueda.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensajeBusqueda.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblMensajeBusqueda.Location = new System.Drawing.Point(3, 3);
            this.lblMensajeBusqueda.Name = "lblMensajeBusqueda";
            this.lblMensajeBusqueda.Size = new System.Drawing.Size(1068, 49);
            this.lblMensajeBusqueda.TabIndex = 45;
            this.lblMensajeBusqueda.Text = "La búsqueda especificada no regresó ningún resultado.";
            this.lblMensajeBusqueda.Visible = false;
            // 
            // tabDirectorio
            // 
            this.tabDirectorio.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabDirectorio.Controls.Add(this.PageProvee);
            this.tabDirectorio.Controls.Add(this.PageLinea);
            this.tabDirectorio.Controls.Add(this.PageMarcas);
            this.tabDirectorio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDirectorio.Location = new System.Drawing.Point(0, 0);
            this.tabDirectorio.Name = "tabDirectorio";
            this.tabDirectorio.SelectedIndex = 0;
            this.tabDirectorio.Size = new System.Drawing.Size(653, 531);
            this.tabDirectorio.TabIndex = 47;
            this.tabDirectorio.SelectedIndexChanged += new System.EventHandler(this.tabDirectorio_SelectedIndexChanged);
            // 
            // PageProvee
            // 
            this.PageProvee.BackColor = System.Drawing.Color.Ivory;
            this.PageProvee.Controls.Add(this.lblMensajeBusqueda);
            this.PageProvee.Controls.Add(this.tjContactos);
            this.PageProvee.Controls.Add(this.listProvee);
            this.PageProvee.Location = new System.Drawing.Point(4, 25);
            this.PageProvee.Name = "PageProvee";
            this.PageProvee.Padding = new System.Windows.Forms.Padding(3);
            this.PageProvee.Size = new System.Drawing.Size(645, 502);
            this.PageProvee.TabIndex = 0;
            this.PageProvee.Text = "Proveedores";
            // 
            // tjContactos
            // 
            this.tjContactos.Accion = null;
            this.tjContactos.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tjContactos.AutoScroll = true;
            this.tjContactos.Detalles = null;
            this.tjContactos.Location = new System.Drawing.Point(212, 150);
            this.tjContactos.Name = "tjContactos";
            this.tjContactos.Proveedor = null;
            this.tjContactos.ProveedorId = 0;
            this.tjContactos.Size = new System.Drawing.Size(427, 346);
            this.tjContactos.TabIndex = 46;
            this.tjContactos.tabs = null;
            this.tjContactos.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tjContactos_Scroll);
            // 
            // PageLinea
            // 
            this.PageLinea.Controls.Add(this.listLinea);
            this.PageLinea.Location = new System.Drawing.Point(4, 25);
            this.PageLinea.Name = "PageLinea";
            this.PageLinea.Padding = new System.Windows.Forms.Padding(3);
            this.PageLinea.Size = new System.Drawing.Size(645, 502);
            this.PageLinea.TabIndex = 1;
            this.PageLinea.Text = "Líneas";
            this.PageLinea.UseVisualStyleBackColor = true;
            // 
            // listLinea
            // 
            this.listLinea.BackColor = System.Drawing.Color.Ivory;
            this.listLinea.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listLinea.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7});
            this.listLinea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLinea.Font = new System.Drawing.Font("Arial Narrow", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listLinea.ForeColor = System.Drawing.Color.SteelBlue;
            this.listLinea.FullRowSelect = true;
            this.listLinea.GridLines = true;
            this.listLinea.Location = new System.Drawing.Point(3, 3);
            this.listLinea.MultiSelect = false;
            this.listLinea.Name = "listLinea";
            this.listLinea.OwnerDraw = true;
            this.listLinea.Size = new System.Drawing.Size(639, 496);
            this.listLinea.TabIndex = 0;
            this.listLinea.UseCompatibleStateImageBehavior = false;
            this.listLinea.View = System.Windows.Forms.View.SmallIcon;
            this.listLinea.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listas_DrawColumnHeader);
            this.listLinea.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listLinea_DrawItem);
            this.listLinea.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listLinea_ItemSelectionChanged);
            this.listLinea.DoubleClick += new System.EventHandler(this.listLinea_DoubleClick);
            this.listLinea.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listLinea_MouseUp);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Línea";
            this.columnHeader6.Width = 260;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Id";
            this.columnHeader7.Width = 0;
            // 
            // PageMarcas
            // 
            this.PageMarcas.Controls.Add(this.listMarca);
            this.PageMarcas.Location = new System.Drawing.Point(4, 25);
            this.PageMarcas.Name = "PageMarcas";
            this.PageMarcas.Padding = new System.Windows.Forms.Padding(3);
            this.PageMarcas.Size = new System.Drawing.Size(645, 502);
            this.PageMarcas.TabIndex = 2;
            this.PageMarcas.Text = "Marcas";
            this.PageMarcas.UseVisualStyleBackColor = true;
            // 
            // listMarca
            // 
            this.listMarca.BackColor = System.Drawing.Color.Ivory;
            this.listMarca.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listMarca.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader12,
            this.columnHeader13});
            this.listMarca.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMarca.Font = new System.Drawing.Font("Arial Narrow", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listMarca.ForeColor = System.Drawing.Color.SteelBlue;
            this.listMarca.FullRowSelect = true;
            this.listMarca.Location = new System.Drawing.Point(3, 3);
            this.listMarca.MultiSelect = false;
            this.listMarca.Name = "listMarca";
            this.listMarca.OwnerDraw = true;
            this.listMarca.Size = new System.Drawing.Size(639, 496);
            this.listMarca.TabIndex = 0;
            this.listMarca.UseCompatibleStateImageBehavior = false;
            this.listMarca.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listas_DrawColumnHeader);
            this.listMarca.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listMarca_DrawItem);
            this.listMarca.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listMarca_ItemSelectionChanged);
            this.listMarca.DoubleClick += new System.EventHandler(this.listMarca_DoubleClick);
            this.listMarca.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listMarca_MouseUp);
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Marca";
            this.columnHeader12.Width = 260;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Id";
            this.columnHeader13.Width = 0;
            // 
            // ctMnu
            // 
            this.ctMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StripMenuItemProveedores,
            this.StripMenuItemLineas,
            this.StripMenuItemMarcas});
            this.ctMnu.Name = "ctMnu";
            this.ctMnu.Size = new System.Drawing.Size(184, 70);
            // 
            // StripMenuItemProveedores
            // 
            this.StripMenuItemProveedores.Name = "StripMenuItemProveedores";
            this.StripMenuItemProveedores.Size = new System.Drawing.Size(183, 22);
            this.StripMenuItemProveedores.Text = "Mostrar Proveedores";
            this.StripMenuItemProveedores.Click += new System.EventHandler(this.StripMenuItemProveedores_Click);
            // 
            // StripMenuItemLineas
            // 
            this.StripMenuItemLineas.Name = "StripMenuItemLineas";
            this.StripMenuItemLineas.Size = new System.Drawing.Size(183, 22);
            this.StripMenuItemLineas.Text = "Mostrar Líneas";
            this.StripMenuItemLineas.Click += new System.EventHandler(this.StripMenuItemLineas_Click);
            // 
            // StripMenuItemMarcas
            // 
            this.StripMenuItemMarcas.Name = "StripMenuItemMarcas";
            this.StripMenuItemMarcas.Size = new System.Drawing.Size(183, 22);
            this.StripMenuItemMarcas.Text = "Mostrar Marcas";
            this.StripMenuItemMarcas.Click += new System.EventHandler(this.StripMenuItemMarcas_Click);
            // 
            // DirectorioVentas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Ivory;
            this.Controls.Add(this.tabDirectorio);
            this.Name = "DirectorioVentas";
            this.Size = new System.Drawing.Size(653, 531);
            this.Load += new System.EventHandler(this.DirectorioVentas_Load);
            this.tabDirectorio.ResumeLayout(false);
            this.PageProvee.ResumeLayout(false);
            this.PageLinea.ResumeLayout(false);
            this.PageMarcas.ResumeLayout(false);
            this.ctMnu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listProvee;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label lblMensajeBusqueda;
        private Tarjetas tjContactos;
        public System.Windows.Forms.TabControl tabDirectorio;
        private System.Windows.Forms.TabPage PageLinea;
        private System.Windows.Forms.TabPage PageProvee;
        private System.Windows.Forms.TabPage PageMarcas;
        private System.Windows.Forms.ListView listLinea;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ListView listMarca;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ContextMenuStrip ctMnu;
        private System.Windows.Forms.ToolStripMenuItem StripMenuItemProveedores;
        private System.Windows.Forms.ToolStripMenuItem StripMenuItemLineas;
        private System.Windows.Forms.ToolStripMenuItem StripMenuItemMarcas;
    }
}
