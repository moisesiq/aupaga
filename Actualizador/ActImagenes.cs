using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace Actualizador
{
    public partial class ActImagenes : Form
    {
        const string ArchivoUltimaActualizacion = "UltimaActualizacion";
        const string ParametroUltimaActualizacion = "UltimaActualizacion";

        public string RutaImgLocal { get; set; }
        public bool Procesando { get; set; }

        public ActImagenes()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ActImagenes_Load(object sender, EventArgs e)
        {
            this.Text = ("Actualizador de Imágenes " + Application.ProductVersion);
        }

        private void ActImagenes_Shown(object sender, EventArgs e)
        {
            // Se obtienen las diferencias
            this.ObtenerDiferencias();
        }
        
        private void ActImagenes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Procesando)
                e.Cancel = true;
        }

        private void btnAccion_Click(object sender, EventArgs e)
        {
            if (this.btnAccion.Text == "Reintentar")
            {
                this.btnAccion.Text = "Cerrar";
                this.CopiarImagenes();
            }
            else
            {
                this.Close();
            }
        }

        #endregion
                
        #region [ Métodos ]

        private void ObtenerDiferencias()
        {
            this.Procesando = true;
            this.lblMensaje.Visible = true;
            
            // Se obtiene la fecha de la última actualización
            DateTime dUltAct = DateTime.MinValue;
            if (File.Exists(this.RutaImgLocal + ActImagenes.ArchivoUltimaActualizacion))
            {
                var aInfo = UtilLocal.LeerArchivoParametros(this.RutaImgLocal + ActImagenes.ArchivoUltimaActualizacion);
                if (aInfo.ContainsKey(ActImagenes.ParametroUltimaActualizacion))
                    dUltAct = UtilLocal.FechaHora(aInfo[ActImagenes.ParametroUltimaActualizacion]);
            }

            // var oDiferencias = new List<string>();
            // var oImgAct = new DirectoryInfo(Inicio.RutaAct).GetFiles().Where(c => c.Extension == "jpg" && (c.CreationTime > dUltAct || c.LastWriteTime > dUltAct));
            // var aImgLoc = Directory.GetFiles(this.RutaImgLocal);

            // var oImgAct = new DirectoryInfo(Inicio.RutaAct).GetFiles();
            // var oImgLoc = new DirectoryInfo(this.RutaImgLocal).GetFiles();
            
            /* this.pgbActualizacion.Maximum = aImgAct.Length;

            string sLocal = "";
            foreach (string sActual in aImgAct)
            {
                if (aImgLoc.Contains(this.RutaImgLocal + Path.GetFileName(sActual)))
                {
                    sLocal = aImgLoc.First(c => c == (this.RutaImgLocal + Path.GetFileName(sActual)));
                }
                else
                {
                    oDiferencias.Add(sActual);
                    this.pgbActualizacion.PerformStep();
                    continue;
                }

                if (File.OpenRead(sLocal).Length != File.OpenRead(sActual).Length)
                    oDiferencias.Add(sActual);

                this.pgbActualizacion.PerformStep();
                Application.DoEvents();
            }
 
            // Se listan las diferencias
            foreach (string sImagen in oDiferencias)
                this.lsvArchivos.Items.Add(sImagen);
            */

            // Nueva forma
            Application.DoEvents();
            var oDiferencias = new DirectoryInfo(Inicio.RutaAct).GetFiles("*.jpg").Where(c => (c.CreationTime > dUltAct || c.LastWriteTime > dUltAct)).ToList();
            this.pgbActualizacion.Maximum = oDiferencias.Count;
            // Se listan las diferencias
            foreach (var oReg in oDiferencias)
            {
                this.lsvArchivos.Items.Add(oReg.FullName);
                this.pgbActualizacion.PerformStep();
                Application.DoEvents();
            }

            // 
            this.lblMensaje.Visible = false;

            // Se comienzan a copiar los archivos necesarios
            this.CopiarImagenes();

            this.Procesando = false;
        }

        private void CopiarImagenes()
        {
            this.Procesando = true;

            int iErrores = 0;
            this.pgbActualizacion.Value = 0;
            this.pgbActualizacion.Maximum = this.lsvArchivos.Items.Count;
            foreach (ListViewItem oItem in this.lsvArchivos.Items)
            {
                string sArchivo = oItem.SubItems[0].Text;
                string sRutaRelativa = sArchivo.Replace(Inicio.RutaAct  , "");

                // Se intenta hacer la copia
                try
                {
                    File.Copy(sArchivo, (this.RutaImgLocal + sRutaRelativa), true);
                    oItem.SubItems.Add("Copiado");
                    oItem.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    oItem.SubItems.Add("Error");
                    oItem.SubItems.Add(ex.Message);
                    oItem.ForeColor = Color.Red;
                    iErrores++;
                }

                this.pgbActualizacion.PerformStep();
                Application.DoEvents();
            }

            if (iErrores > 0)
            {
                this.btnAccion.Text = "Reintentar";
            }
            else
            {
                // Se guarda el dato de la última actualización
                UtilLocal.GuardarValorArchivoParametros((this.RutaImgLocal + ActImagenes.ArchivoUltimaActualizacion)
                    , ActImagenes.ParametroUltimaActualizacion, DateTime.Now.ToString());
            }

            this.Procesando = false;
        }        

        #endregion
    }
}
