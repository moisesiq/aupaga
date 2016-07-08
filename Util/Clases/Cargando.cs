using System;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace LibUtil
{
    public static class Cargando
    {
        //static Form oForma;
        static CargandoForma oForma;
        static PictureBox oImagen;
        static Thread oProceso;
        delegate void delForma(Form oForma);

        //public static IWin32Window Contenedor { get; set; }
        public static Image Animacion { get; set; }

        static Cargando()
        {
            
        }

        private static void ProcesoMostrar()
        {
            // Se crea el formulario
            /* Cargando.oForma = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,
                Size = new Size(64, 64),
                Opacity = 85,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterScreen
            };
            // Se crea el "PictureBox"
            Cargando.oImagen = new PictureBox()
            {
                Dock = DockStyle.Fill,
                Image = Cargando.Animacion,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            // Se agrega el "Picture" a la forma
            Cargando.oForma.Controls.Add(oImagen);
            */

            Cargando.oForma = new CargandoForma();
            // if (oParam != null)
                // Cargando.oForma.Owner = (oParam as Form);

            // Se muestra el formulario
            //Cargando.oForma.Show();
            //Cargando.oForma.Size = new Size(64, 64);

            // Se usa un try porque al llamar el "Abort()" para terminar el proceso, se genera un excepción
            //try { Application.Run(Cargando.oForma); }
            //catch (Exception e) { MessageBox.Show(e.Message + "\n\n" + e.InnerException.Message); }
            Application.Run(Cargando.oForma);
            
            /*Cargando.oForma.Close();
            Cargando.oForma.Dispose();*/
            Cargando.oForma.Dispose();
            Cargando.oForma = null;
        }
                
        public static Form Mostrar()
        {
            if (Cargando.oProceso != null && Cargando.oProceso.IsAlive)
            {
                //MessageBox.Show("Ya existe un proceso en ejecución.", "Error", MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return null;
            }

            Cargando.oProceso = new Thread(Cargando.ProcesoMostrar);
            Cargando.oProceso.Start();

            return null;
        }

        public static void Cerrar()
        {
            if (Cargando.oProceso == null || !Cargando.oProceso.IsAlive || Cargando.oForma == null)
                return;

            Cargando.oForma.Cerrar();
            // Cargando.oForma.Close();
            // Cargando.oForma.Dispose();
            // Cargando.oProceso = null;
        }
    }

    class CargandoForma : Form
    {
        bool FormCargado = false;
        bool MandarCerrar = false;

        public CargandoForma()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(128, 128);
            // this.Opacity = 85;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TransparencyKey = Color.Black;
            this.BackColor = Color.Black;
            
            // Se crea el "PictureBox"
            var oImagen = new PictureBox()
            {
                Dock = DockStyle.Fill,
                Image = Cargando.Animacion,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            // Se agrega el "Picture" a la forma
            this.Controls.Add(oImagen);

            // Eventos
            this.Load += new EventHandler(CargandoForma_Load);
            this.Shown += new EventHandler(CargandoForma_Shown);
            this.Disposed += new EventHandler(CargandoForma_Disposed);
        }

        void CargandoForma_Load(object sender, EventArgs e)
        {
            this.Size = new Size(128, 128);
        }

        void CargandoForma_Shown(object sender, EventArgs e)
        {
            if (this.MandarCerrar)
                this.Close();

            this.FormCargado = true;
        }

        void CargandoForma_Disposed(object sender, EventArgs e)
        {
            // var x = "jaja";
            // Util.MensajeAdvertencia("disposing", x);
        }

        public void Cerrar()
        {
            if (this.IsHandleCreated && this.FormCargado)
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(this.Close));
                else
                    this.Close();
            }
            else
            {
                this.MandarCerrar = true;
            }
        }
    }
}
