using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class VerImagenesParte : Form
    {
        enum Direccion { Izquierda, Derecha, Inicio, Fin }
        List<string> oImagenes;
        int ImagenActual;

        public VerImagenesParte(int iParteID)
        {
            InitializeComponent();

            this.pcbParte.Controls.Add(this.pnlBotones);

            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;

            var oParte = General.GetEntity<Parte>(q => q.Estatus && q.ParteID == iParteID);
            if (oParte != null)
                this.Text = (oParte.NumeroParte + " - " + oParte.NombreParte);

            this.oImagenes = AdmonProc.ObtenerImagenesParte(iParteID);
            // this.oImagenes = General.GetListOf<ParteImagen>(q => q.Estatus && q.ParteID == iParteID).OrderBy(q => q.Orden).ToList();

            // Se configuran los botones
            foreach (Control oControl in this.pnlBotones.Controls)
            {
                if (oControl is Button)
                {
                    oControl.MouseLeave += new EventHandler((s, e) =>
                    {
                        var btn = (s as Button);
                        // btn.Image = (Properties.Resources.ResourceManager.GetObject("Boton" + btn.Name.Replace("btn", "")) as Image);
                        btn.BackgroundImage = (Properties.Resources.ResourceManager.GetObject("Boton" + btn.Name.Replace("btn", "")) as Image);
                    });
                    oControl.MouseDown += new MouseEventHandler((s, e) =>
                    {
                        var btn = (s as Button);
                        // btn.Image = (Properties.Resources.ResourceManager.GetObject("Boton" + btn.Name.Replace("btn", "") + "Clic") as Image);
                        btn.BackgroundImage = (Properties.Resources.ResourceManager.GetObject("Boton" + btn.Name.Replace("btn", "") + "Clic") as Image);
                    });
                    oControl.MouseUp += new MouseEventHandler((s, e) =>
                    {
                        var btn = (s as Button);
                        // btn.Image = (Properties.Resources.ResourceManager.GetObject("Boton" + btn.Name.Replace("btn", "")) as Image);
                        btn.BackgroundImage = (Properties.Resources.ResourceManager.GetObject("Boton" + btn.Name.Replace("btn", "")) as Image);
                    });
                }
            }
        }

        #region [ Eventos ]
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    this.btnAnterior_Click(this, null);
                    return true;
                case Keys.Right:
                    this.btnSiguiente_Click(this, null);
                    return true;
                case Keys.Up:
                    this.btnPrimera_Click(this, null);
                    return true;
                case Keys.Down:
                    this.btnUltima_Click(this, null);
                    return true;
                case Keys.Escape:
                    this.Close();
                    break;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void VerImagenesParte_Load(object sender, System.EventArgs e)
        {
            // Se establecen los estados iniciales
            this.ActiveControl = this.lblMensaje;
            this.txtNumeroDeImagenes.Text = this.oImagenes.Count.ToString();
            this.HabilitarBotones();

            //
            if (this.oImagenes.Count > 0)
            {
                this.txtImagen.Text = "1";
            }
            else
            {
                this.btnAnterior.Enabled = false;
                this.txtImagen.Text = "0";
                this.txtImagen.ReadOnly = true;
                this.btnSiguiente.Enabled = false;
                this.lblMensaje.Visible = true;
            }
        }

        private void btnPrimera_Click(object sender, System.EventArgs e)
        {
            this.CambiarImagen(Direccion.Inicio);
        }

        private void btnAnterior_Click(object sender, System.EventArgs e)
        {
            this.CambiarImagen(Direccion.Izquierda);
        }

        private void txtImagen_Enter(object sender, System.EventArgs e)
        {
            this.txtImagen.SelectAll();
        }

        private void txtImagen_TextChanged(object sender, System.EventArgs e)
        {
            int iImagen = (Helper.ConvertirEntero(this.txtImagen.Text) - 1);
            if (iImagen >= 0 && iImagen < this.oImagenes.Count)
            {
                this.ImagenActual = iImagen;
                this.PonerImagen(this.oImagenes[this.ImagenActual]);
                this.HabilitarBotones();
            }
        }

        private void btnSiguiente_Click(object sender, System.EventArgs e)
        {
            this.CambiarImagen(Direccion.Derecha);
        }

        private void btnUltima_Click(object sender, System.EventArgs e)
        {
            this.CambiarImagen(Direccion.Fin);
        }

        #endregion

        private void HabilitarBotones()
        {
            this.btnPrimera.Enabled = (this.ImagenActual > 0);
            // this.btnAnterior.Enabled = (this.ImagenActual > 0);
            // this.btnSiguiente.Enabled = (this.ImagenesParte.Count > 1);
            this.btnUltima.Enabled = (this.ImagenActual < (this.oImagenes.Count - 1));
        }

        private void PonerImagen(string sRutaImagen)
        {
            if (File.Exists(sRutaImagen))
                this.pcbParte.Image = Image.FromFile(sRutaImagen);
            else
                this.pcbParte.Image = null;
        }

        private void CambiarImagen(Direccion eDireccion)
        {
            if (this.oImagenes.Count <= 0) return;

            int iUltImagen = (this.oImagenes.Count - 1);

            // Se determina la imagen que se visualizará
            switch (eDireccion)
            {
                case Direccion.Izquierda:
                    this.ImagenActual--;
                    break;
                case Direccion.Derecha:
                    this.ImagenActual++;
                    break;
                case Direccion.Inicio:
                    this.ImagenActual = 0;
                    break;
                case Direccion.Fin:
                    this.ImagenActual = iUltImagen;
                    break;
            }

            // Se verifica que esté dentro del rango
            if (this.ImagenActual < 0)
                this.ImagenActual = iUltImagen;
            if (this.ImagenActual > iUltImagen)
                this.ImagenActual = 0;

            // Se pone la imagen
            this.PonerImagen(this.oImagenes[this.ImagenActual]);

            // Se habilitan / desabilitan los botones según corresponda
            this.txtImagen.Text = (this.ImagenActual + 1).ToString();
            this.HabilitarBotones();
        }
                
    }
}
