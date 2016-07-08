namespace Refaccionaria.App
{
    partial class DetalleBusquedaDeClientes
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
            this.listadoSimple = new Refaccionaria.App.ListadoSimple();
            this.SuspendLayout();
            // 
            // listadoSimple
            // 
            this.listadoSimple.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listadoSimple.EspacioBotones = false;
            this.listadoSimple.Location = new System.Drawing.Point(0, 0);
            this.listadoSimple.Name = "listadoSimple";
            this.listadoSimple.Size = new System.Drawing.Size(625, 416);
            this.listadoSimple.TabIndex = 0;
            // 
            // DetalleBusquedaDeClientes
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(79)))), ((int)(((byte)(109)))));
            this.ClientSize = new System.Drawing.Size(625, 416);
            this.Controls.Add(this.listadoSimple);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetalleBusquedaDeClientes";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.DetalleBusquedaDeClientes_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ListadoSimple listadoSimple;

    }
}
