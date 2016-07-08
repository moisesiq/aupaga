using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using System.IO;

using LibUtil;

namespace Refaccionaria.App
{
    public partial class VisorImagenes : Form
    {
        public Image imagen { get;  set; }
        public int parteId { get; set; }

        protected string[] listaImagenes;        
        protected int currentImage = -1;
       
        public static VisorImagenes Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly VisorImagenes instance = new VisorImagenes();
        }

        public VisorImagenes()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void VisorImagenes_Load(object sender, EventArgs e)
        {
            try
            {
                if (imagen != null)
                {
                    this.picBoxImagen.Image = imagen;
                    this.StartPosition = FormStartPosition.CenterScreen;
                }

                if (parteId > 0)
                {                     
                    var imagenes = Datos.GetListOf<ParteImagen>(p => p.ParteID.Equals(parteId));
                    listaImagenes = new string[imagenes.Count];
                    var x = 0;
                    foreach (var img in imagenes)
                    {
                        if (Directory.Exists(GlobalClass.ConfiguracionGlobal.pathImagenes))
                        {                            
                            listaImagenes[x] = string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathImagenes, img.NombreImagen);
                            x++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        
        private void VisorImagenes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
        
        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (listaImagenes.Length > 0)
            {
                currentImage = currentImage == 0 ? listaImagenes.Length - 1 : --currentImage;
                ShowCurrentImage();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (listaImagenes.Length > 0)
            {
                currentImage = currentImage == listaImagenes.Length - 1 ? 0 : ++currentImage;
                ShowCurrentImage();
            }
        }

        #endregion

        #region [ Metodos ]

        protected void ShowCurrentImage()
        {
            if (currentImage >= 0 && currentImage <= listaImagenes.Length - 1)
            {
                //picBoxImagen.Image = new Bitmap(listaImagenes[cont]);
                picBoxImagen.Image = Bitmap.FromFile(listaImagenes[currentImage]);
            }
        }

        #endregion
    }
}
