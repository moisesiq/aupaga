using System.Windows.Forms;
using System.Drawing;

namespace Refaccionaria.App
{
    public partial class Notificacion : Form
    {
        protected override bool ShowWithoutActivation { get { return true; } }

        public Form Contenedor;

        public Notificacion(string sMensaje, int iIntervalo)
        {
            InitializeComponent();

            this.txtMensaje.Text = sMensaje;
            if (iIntervalo > 0)
            {
                this.tmrCerrar.Interval = iIntervalo;
                this.tmrCerrar.Enabled = true;
            }
        }

        #region [ Eventos ]

        private void Notificacion_Load(object sender, System.EventArgs e)
        {
            // Posición de la ventana
            this.EstablecerPosicion();
        }

        private void tmrCerrar_Tick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Metodos ]

        private void EstablecerPosicion()
        {
            int num;
            int num2;
            if (this.Contenedor == null)
            {
                Screen screen = Screen.FromControl(this);
                num = (int)(((screen.WorkingArea.Left + screen.WorkingArea.Width) - base.Width) - 6);
                num2 = (int)(((screen.WorkingArea.Top + screen.WorkingArea.Height) - base.Height) - 6);
            }
            else
            {
                SetParent((int)base.Handle, (int)this.Contenedor.Handle);
                num = (int)((this.Contenedor.ClientSize.Width - base.Width) - 6);
                num2 = (int)((this.Contenedor.ClientSize.Height - base.Height) - 6);
            }
            base.Location = new Point(num, num2);
        }

        public void Mostrar(string sMensaje)
        {
            this.txtMensaje.Text = sMensaje;
            base.Show();
        }
                
        public void Mostrar(Form Contenedor)
        {
            this.Contenedor = Contenedor;
            base.Show();
        }

        [System.Runtime.InteropServices.DllImport("User32")]
        public static extern int SetParent(int hWndChild, int hWndNewParent);

        #endregion
    }
}
