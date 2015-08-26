using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using System.IO;

namespace Refaccionaria.App
{
    public partial class DetalleImagen : DetalleBase
    {
        Parte Parte;
        string nombre;

        public static DetalleImagen Instance
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

            internal static readonly DetalleImagen instance = new DetalleImagen();
        }

        public DetalleImagen()
        {
            InitializeComponent();
        }

        public DetalleImagen(int Id)
        {
            InitializeComponent();
            try
            {
                Parte = Negocio.General.GetEntityById<Parte>("Parte", "ParteID", Id);
                if (Parte == null)
                    throw new EntityNotFoundException(Id.ToString(), "Parte");
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleImagen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                dialog.InitialDirectory = @"C:\";
                dialog.Title = "Seleccione una imagen.";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    picBoxImagen.Image = new Bitmap(dialog.FileName);
                    nombre = Path.GetFileNameWithoutExtension(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (picBoxImagen.Image == null && !string.IsNullOrEmpty(nombre))
                return;

            try
            {
                var nombreArchivo = string.Format("{0}{1}{2}", GlobalClass.ConfiguracionGlobal.pathImagenes, nombre, ".jpg");
                //picBoxImagen.Image.Save(nombreArchivo, ImageFormat.Jpeg);
                SaveJpeg(nombreArchivo, picBoxImagen.Image, 100);

                var imagen = new ParteImagen()
                {
                    ParteID = Parte.ParteID,
                    NombreImagen = string.Format("{0}{1}", nombre, ".jpg"),
                    UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                    FechaRegistro = DateTime.Now,
                    Estatus = true,
                    Actualizar = true
                };
                Negocio.General.SaveOrUpdate<ParteImagen>(imagen, imagen);
                new Notificacion("Imagen Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Metodos]

        public static void SaveJpeg(string path, Image img, int quality)
        {
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            ImageCodecInfo jpegCodec = GetEncoderInfo(@"image/jpeg");

            EncoderParameters encoderParams = new EncoderParameters(1);

            encoderParams.Param[0] = qualityParam;

            MemoryStream mss = new MemoryStream();

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

            img.Save(mss, jpegCodec, encoderParams);
            byte[] matriz = mss.ToArray();
            fs.Write(matriz, 0, matriz.Length);

            mss.Close();
            fs.Close();
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        #endregion
                
    }
}
