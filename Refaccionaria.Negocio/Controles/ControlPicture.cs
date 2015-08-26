using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Refaccionaria.Negocio.Controles
{
    public static class ControlPicture
    {

        /// <summary>
        /// Asigna una Imagen a un PictureBox mediante una ruta.
        /// </summary>
        /// <param name="RutaImg">Ruta del archivo de imagen.</param>
        /// <param name="Logo">Control que recibe la imagen</param>
        public static void CargaImagen(string RutaImg, PictureBox Logo)
        {
            if (System.IO.File.Exists(RutaImg))
            {
                PictureBox NuevaImagen = new PictureBox();
                NuevaImagen.Image = Bitmap.FromFile(RutaImg);
                Logo.Image = null;
                SetImage(Logo, NuevaImagen.Image);
            }
            else
            {
                Logo.SizeMode = PictureBoxSizeMode.StretchImage;
                Logo.Image = Logo.ErrorImage;
            }
        }

        private static Size GenerateImageDimensions(int currW, int currH, int destW, int destH)
        {
            //double to hold the final multiplier to use when scaling the image
            double multiplier = 0;

            //string for holding layout
            string layout;

            //determine if it's Portrait or Landscape
            if (currH > currW)
                layout = "portrait";
            else
                layout = "landscape";

            switch (layout)
            {
                case "portrait":
                    //calculate multiplier on heights
                    if (destH < currH)
                        multiplier = Convert.ToDouble(destH) / Convert.ToDouble(currH);
                    else
                        multiplier = Convert.ToDouble(currH) / Convert.ToDouble(destH);
                    break;
                case "landscape":
                    //calculate multiplier on widths
                    if (destW < currW)
                        multiplier = Convert.ToDouble(destW) / Convert.ToDouble(currW);
                    else
                        multiplier = Convert.ToDouble(currW) / Convert.ToDouble(destW);
                    break;
            }
            //return the new image dimensions
            return new Size(Convert.ToInt32(currW * multiplier), Convert.ToInt32(currH * multiplier));
        }

        //Resize the image
        private static void SetImage(PictureBox Logo, Image img)
        {
            try
            {
                //calculate the size of the image
                Size imgSize = GenerateImageDimensions(img.Width, img.Height, Logo.Width, Logo.Height);

                //create a new Bitmap with the proper dimensions
                Bitmap finalImg = new Bitmap(img, imgSize.Width, imgSize.Height);

                //create a new Graphics object from the image
                Graphics gfx = Graphics.FromImage(img);

                //clean up the image (take care of any image loss from resizing)
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gfx.DrawImage(finalImg, 0, 0);

                //center the new image
                Logo.SizeMode = PictureBoxSizeMode.CenterImage;

                //set the new image
                Logo.Image = finalImg;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void DibujarSombraLista(ImageList Contenedor, DrawListViewItemEventArgs e)
        {
            int iLeftImg = 30;   // "Left" de la imagen en relación al "Item"
            int iTopImg = 2;     // "Top" de la imagen en relación al "Item"
            int iWidthImg = Contenedor.ImageSize.Width;
            int iHeightImg = Contenedor.ImageSize.Height;
            int iSeparacionSombra = 4;

            // Se crean los rectángulos de las partes a dibujar
            var oRecImagen = new Rectangle(e.Bounds.Left + iLeftImg, e.Bounds.Top + iTopImg, iWidthImg, iHeightImg);
            var oRecTexto = new Rectangle(e.Bounds.Left, oRecImagen.Bottom + iSeparacionSombra, e.Bounds.Width, 40);

            // Si está seleccionado, se muestra diferente
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, oRecTexto);
                e.Item.ForeColor = Color.White;
            }
            else
            {
                e.Item.ForeColor = e.Item.ListView.ForeColor;
            }

            // Se dibuja la imagen y el texto
            e.Graphics.DrawImage(Contenedor.Images[e.Item.ImageKey], oRecImagen);
            e.Graphics.DrawString(e.Item.Text, e.Item.Font, new SolidBrush(e.Item.ForeColor), oRecTexto, new StringFormat() { Alignment = StringAlignment.Center });

            // Se crean los rectángulos correspondientes a las sombras            
            var RecHorizontal = new Rectangle(oRecImagen.Left + iSeparacionSombra, oRecImagen.Bottom, iWidthImg - (iSeparacionSombra / 2), iSeparacionSombra);
            var RecVertical = new Rectangle(oRecImagen.Right, oRecImagen.Top + iSeparacionSombra, iSeparacionSombra, iHeightImg);
            var RecCorreccion = new Rectangle(
                RecVertical.X - 1, RecHorizontal.Y - 1,
                iSeparacionSombra + 1, iSeparacionSombra + 1//(iSeparacionSombra / 2)
            );

            var oColor = Color.Gray;
            var BrochaHorizontal = new LinearGradientBrush(RecHorizontal, oColor, Color.White, LinearGradientMode.Vertical);
            var BrochaVertical = new LinearGradientBrush(RecVertical, oColor, Color.White, LinearGradientMode.Horizontal);
            //var BrochaCorreccion = new LinearGradientBrush(RecCorreccion, oColor, Color.White, LinearGradientMode.ForwardDiagonal);
            // Se dibuja la sombra
            //e.Graphics.FillRectangle(BrochaCorreccion, RecCorreccion.X, RecCorreccion.Y, RecCorreccion.Width, RecCorreccion.Height);
            e.Graphics.FillRectangle(BrochaVertical, RecVertical);
            e.Graphics.FillRectangle(BrochaHorizontal, RecHorizontal);

            // Para cuando está seleccionado
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)), oRecImagen);
            }
        }

        public static string ImagenEnLista(ImageList Lista, string LlaveImg, string ArchivoImg, string Ruta)
        {
            if (!Lista.Images.ContainsKey(LlaveImg))
            {
                Ruta = Ruta + ArchivoImg;
                if (System.IO.File.Exists(Ruta))
                    Lista.Images.Add(LlaveImg, Image.FromFile(Ruta));
                else
                    LlaveImg = "0";
            }
            return LlaveImg;
        }

    }

    public class CargaLogos
    {

        public void CargaLogo(string LlaveImg, PictureBox Contenedor, string Ruta)
        {
            Contenedor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            // string Ruta = RutaGlobal; // GlobalClass.ConfiguracionGlobal.pathImagenes;
            Ruta = Ruta + "(" + LlaveImg + ").jpg";
            if (System.IO.File.Exists(Ruta))
                Contenedor.Image = Image.FromFile(Ruta);
        }

        public string AbrirGuardarImg(string LlaveImg, PictureBox Contenedor, string Ruta)
        {
            if (!(Contenedor.Image == null)) Contenedor.Image.Dispose();

            String Archivo = "";
            string NombreArchivo = "";

            var fdAttach = new OpenFileDialog();
            fdAttach.Filter = "Imagen|*.jpg;*.jpeg;*.gif;*.png;*.bmp;";
            //fdAttach.Filter = "Tipos de archivo|*.doc?;*.xls?;*.ppt?;*.pps?;*.pdf;*.txt;*.jpg;*.jpeg;*.gif;*.png;*.bmp;";
            fdAttach.Title = "Ruta del archivo a importar";
            if (fdAttach.ShowDialog() == DialogResult.OK)
                //if (fdAttach.ShowDialog(Principal.Instance) == DialogResult.OK)
                Archivo = fdAttach.FileName;
            fdAttach.Dispose();

            if (Archivo != "")
            {
                Bitmap Img;
                // string Ruta = RutaGlobal; //GlobalClass.ConfiguracionGlobal.pathImagenes;
                Ruta = Ruta + "\\(" + LlaveImg + ").jpg";
                var Cuadro = new System.Drawing.Size(130, 100);
                Img = (Bitmap)resizeImage(Image.FromFile(Archivo, true), Cuadro);
                saveJpeg(Ruta, Img, 150);
                NombreArchivo = fdAttach.FileName;
                NombreArchivo = NombreArchivo.Substring(NombreArchivo.LastIndexOf("\\") + 1).ToLower();
            }

            return NombreArchivo;
        }


        private void saveJpeg(string path, Bitmap img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            try
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                img.Save(path, jpegCodec, encoderParams);
            }
            catch (System.IO.IOException e)
            {
                MessageBox.Show("Error: " + e.Message);
            }

        }

        private ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        private Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

    }

}
