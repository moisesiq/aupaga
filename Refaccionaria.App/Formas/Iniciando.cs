using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class Iniciando : PerPixelAlphaForm
    {
        public Iniciando()
        {
            InitializeComponent();

            // this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            // this.BackColor = System.Drawing.Color.Transparent;

            // this.BackColor = System.Drawing.Color.Blue;
            // this.TransparencyKey = System.Drawing.Color.Blue;

            // this.BackgroundImage = Properties.Resources.theos;
        }

        // protected override void OnPaintBackground(PaintEventArgs e) { }

        private void Iniciando_Load(object sender, EventArgs e)
        {
            /*this.Controls.Add(new PictureBox()
            {
                Image = Properties.Resources.theos,
                Size = new System.Drawing.Size(700, 400)
            });*/
            

            /* this.BackgroundImage = (System.Drawing.Image.FromFile(@"C:\tmp\CR\Fuente\Refaccionaria.App\Resources\theos.png") as System.Drawing.Bitmap);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = System.Drawing.Color.Red;
            this.TransparencyKey = System.Drawing.Color.Red;
            */
                        
            // this.TransparencyKey = Color.Empty;
            // this.SetBitmap(System.Drawing.Image.FromFile(@"C:\tmp\CR\Fuente\Refaccionaria.App\Resources\theos.png") as Bitmap);
            this.SetBitmap(Properties.Resources.theos);
        }
    }
}