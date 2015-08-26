namespace Refaccionaria.App
{
    partial class Contabilidad
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Consumibles");
            this.tabContabilidad = new System.Windows.Forms.TabControl();
            this.tbpConfiguracion = new System.Windows.Forms.TabPage();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.trvCatalogos = new System.Windows.Forms.TreeView();
            this.tbpCuentasContables = new System.Windows.Forms.TabPage();
            this.tbpCuentasPolizas = new System.Windows.Forms.TabPage();
            this.tbpCuentasPorSemana = new System.Windows.Forms.TabPage();
            this.tbpConfigAfectaciones = new System.Windows.Forms.TabPage();
            this.tbpBancos = new System.Windows.Forms.TabPage();
            this.tbpGastosParaPolizas = new System.Windows.Forms.TabPage();
            this.tbpReserva = new System.Windows.Forms.TabPage();
            this.tabContabilidad.SuspendLayout();
            this.tbpConfiguracion.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabContabilidad
            // 
            this.tabContabilidad.Controls.Add(this.tbpConfiguracion);
            this.tabContabilidad.Controls.Add(this.tbpCuentasContables);
            this.tabContabilidad.Controls.Add(this.tbpCuentasPolizas);
            this.tabContabilidad.Controls.Add(this.tbpCuentasPorSemana);
            this.tabContabilidad.Controls.Add(this.tbpConfigAfectaciones);
            this.tabContabilidad.Controls.Add(this.tbpBancos);
            this.tabContabilidad.Controls.Add(this.tbpGastosParaPolizas);
            this.tabContabilidad.Controls.Add(this.tbpReserva);
            this.tabContabilidad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabContabilidad.Location = new System.Drawing.Point(0, 0);
            this.tabContabilidad.Name = "tabContabilidad";
            this.tabContabilidad.SelectedIndex = 0;
            this.tabContabilidad.Size = new System.Drawing.Size(752, 438);
            this.tabContabilidad.TabIndex = 11;
            this.tabContabilidad.SelectedIndexChanged += new System.EventHandler(this.tabContabilidad_SelectedIndexChanged);
            // 
            // tbpConfiguracion
            // 
            this.tbpConfiguracion.Controls.Add(this.pnlContenido);
            this.tbpConfiguracion.Controls.Add(this.trvCatalogos);
            this.tbpConfiguracion.Location = new System.Drawing.Point(4, 22);
            this.tbpConfiguracion.Name = "tbpConfiguracion";
            this.tbpConfiguracion.Padding = new System.Windows.Forms.Padding(3);
            this.tbpConfiguracion.Size = new System.Drawing.Size(744, 412);
            this.tbpConfiguracion.TabIndex = 0;
            this.tbpConfiguracion.Text = "Configuración";
            // 
            // pnlContenido
            // 
            this.pnlContenido.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContenido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.pnlContenido.Location = new System.Drawing.Point(242, 0);
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Size = new System.Drawing.Size(502, 412);
            this.pnlContenido.TabIndex = 1;
            // 
            // trvCatalogos
            // 
            this.trvCatalogos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.trvCatalogos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.trvCatalogos.ForeColor = System.Drawing.Color.White;
            this.trvCatalogos.Location = new System.Drawing.Point(0, 0);
            this.trvCatalogos.Name = "trvCatalogos";
            treeNode1.Name = "Consumibles";
            treeNode1.Text = "Consumibles";
            this.trvCatalogos.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.trvCatalogos.Size = new System.Drawing.Size(240, 412);
            this.trvCatalogos.TabIndex = 0;
            this.trvCatalogos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvCatalogos_AfterSelect);
            // 
            // tbpCuentasContables
            // 
            this.tbpCuentasContables.Location = new System.Drawing.Point(4, 22);
            this.tbpCuentasContables.Name = "tbpCuentasContables";
            this.tbpCuentasContables.Padding = new System.Windows.Forms.Padding(3);
            this.tbpCuentasContables.Size = new System.Drawing.Size(744, 412);
            this.tbpCuentasContables.TabIndex = 1;
            this.tbpCuentasContables.Text = "Gastos";
            // 
            // tbpCuentasPolizas
            // 
            this.tbpCuentasPolizas.Location = new System.Drawing.Point(4, 22);
            this.tbpCuentasPolizas.Name = "tbpCuentasPolizas";
            this.tbpCuentasPolizas.Size = new System.Drawing.Size(744, 412);
            this.tbpCuentasPolizas.TabIndex = 6;
            this.tbpCuentasPolizas.Text = "Catálogo";
            this.tbpCuentasPolizas.UseVisualStyleBackColor = true;
            // 
            // tbpCuentasPorSemana
            // 
            this.tbpCuentasPorSemana.Location = new System.Drawing.Point(4, 22);
            this.tbpCuentasPorSemana.Name = "tbpCuentasPorSemana";
            this.tbpCuentasPorSemana.Size = new System.Drawing.Size(744, 412);
            this.tbpCuentasPorSemana.TabIndex = 2;
            this.tbpCuentasPorSemana.Text = "Cuentas x Semana";
            this.tbpCuentasPorSemana.UseVisualStyleBackColor = true;
            // 
            // tbpConfigAfectaciones
            // 
            this.tbpConfigAfectaciones.Location = new System.Drawing.Point(4, 22);
            this.tbpConfigAfectaciones.Name = "tbpConfigAfectaciones";
            this.tbpConfigAfectaciones.Size = new System.Drawing.Size(744, 412);
            this.tbpConfigAfectaciones.TabIndex = 3;
            this.tbpConfigAfectaciones.Text = "Config. Pólizas";
            this.tbpConfigAfectaciones.UseVisualStyleBackColor = true;
            // 
            // tbpBancos
            // 
            this.tbpBancos.Location = new System.Drawing.Point(4, 22);
            this.tbpBancos.Name = "tbpBancos";
            this.tbpBancos.Size = new System.Drawing.Size(744, 412);
            this.tbpBancos.TabIndex = 4;
            this.tbpBancos.Text = "Bancos";
            this.tbpBancos.UseVisualStyleBackColor = true;
            // 
            // tbpGastosParaPolizas
            // 
            this.tbpGastosParaPolizas.Location = new System.Drawing.Point(4, 22);
            this.tbpGastosParaPolizas.Name = "tbpGastosParaPolizas";
            this.tbpGastosParaPolizas.Size = new System.Drawing.Size(744, 412);
            this.tbpGastosParaPolizas.TabIndex = 5;
            this.tbpGastosParaPolizas.Text = "Asignar Gastos";
            this.tbpGastosParaPolizas.UseVisualStyleBackColor = true;
            // 
            // tbpReserva
            // 
            this.tbpReserva.Location = new System.Drawing.Point(4, 22);
            this.tbpReserva.Name = "tbpReserva";
            this.tbpReserva.Size = new System.Drawing.Size(744, 412);
            this.tbpReserva.TabIndex = 7;
            this.tbpReserva.Text = "Reserva";
            this.tbpReserva.UseVisualStyleBackColor = true;
            // 
            // Contabilidad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(87)))), ((int)(((byte)(123)))));
            this.Controls.Add(this.tabContabilidad);
            this.Name = "Contabilidad";
            this.Size = new System.Drawing.Size(752, 438);
            this.Load += new System.EventHandler(this.Contabilidad_Load);
            this.tabContabilidad.ResumeLayout(false);
            this.tbpConfiguracion.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabContabilidad;
        private System.Windows.Forms.TabPage tbpConfiguracion;
        private System.Windows.Forms.TabPage tbpCuentasContables;
        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.TreeView trvCatalogos;
        private System.Windows.Forms.TabPage tbpCuentasPorSemana;
        private System.Windows.Forms.TabPage tbpConfigAfectaciones;
        private System.Windows.Forms.TabPage tbpBancos;
        private System.Windows.Forms.TabPage tbpGastosParaPolizas;
        private System.Windows.Forms.TabPage tbpCuentasPolizas;
        private System.Windows.Forms.TabPage tbpReserva;
    }
}
