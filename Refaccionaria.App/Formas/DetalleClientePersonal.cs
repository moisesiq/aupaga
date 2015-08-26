using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using System.IO;
using System.Drawing.Imaging;

namespace Refaccionaria.App
{
    public partial class DetalleClientePersonal : DetalleBase
    {
        string nombre;
        ControlError cntError = new ControlError();

        public int ClienteId { get; set; }
        public ClientePersonal oClientePersonal { get; set; }

        public DetalleClientePersonal()
        {
            InitializeComponent();
        }

        public DetalleClientePersonal(int iClientePersonalID)
        {
            InitializeComponent();
            this.oClientePersonal = General.GetEntity<ClientePersonal>(c => c.ClientePersonalID == iClientePersonalID && c.Estatus);
            this.txtNombre.Text = this.oClientePersonal.NombrePersonal;
            this.txtCorreoE.Text = this.oClientePersonal.CorreoElectronico;
            this.chkCfdi.Checked = this.oClientePersonal.EnviarCfdi;
            try
            {
                picBoxImagen.Image = new Bitmap(UtilLocal.RutaImagenClientePersonalFirma(iClientePersonalID));
            }
            catch (Exception)
            {
                picBoxImagen.Image = null;
            }
        }

        #region [ Eventos ]  

        private void DetalleClientePersonal_Load(object sender, EventArgs e)
        {

        }

        private void DetalleClientePersonal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;
            if (picBoxImagen.Image == null && !string.IsNullOrEmpty(nombre))
                return;
            try
            {
                if (this.oClientePersonal == null)
                {
                    if (ClienteId > 0)
                    {
                        this.oClientePersonal = new ClientePersonal() { ClienteID = ClienteId };
                    }
                }

                this.oClientePersonal.NombrePersonal = this.txtNombre.Text;
                this.oClientePersonal.CorreoElectronico = this.txtCorreoE.Text;
                this.oClientePersonal.EnviarCfdi = this.chkCfdi.Checked;

                Guardar.Generico<ClientePersonal>(this.oClientePersonal);
                new Notificacion("Personal Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                clientes.Instance.CustomInvoke<clientes>(m => m.cargarClientePersonal(ClienteId));

                if (this.picBoxImagen.Image != null)
                {
                    var nombreArchivo = UtilLocal.RutaImagenClientePersonalFirma(oClientePersonal.ClientePersonalID);
                    Directory.CreateDirectory(nombreArchivo.Substring(0, nombreArchivo.Length - 21));
                    SaveJpeg(nombreArchivo, picBoxImagen.Image, 100);
                }

                new Notificacion("Firma guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }

            

            this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DetalleImagenFirma_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        
        private void btnSeleccionar_Click_1(object sender, EventArgs e)
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

        #endregion 

        #region [ Metodos ]

        private void CargaInicial()
        {
            // Se validan los permisos
            //if (this.EsNuevo)
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Agregar", true))
            //    {
            //        this.Close();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Modificar", false))
            //        this.btnGuardar.Enabled = false;
            //}

            try
            {
                
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {            
            this.cntError.LimpiarErrores();
            if (this.txtNombre.Text == "")
                this.cntError.PonerError(this.txtNombre, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if(this.chkCfdi.Checked && this.txtCorreoE.Text == "")
                this.cntError.PonerError(this.txtCorreoE, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

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
