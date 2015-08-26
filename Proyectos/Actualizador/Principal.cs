using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Actualizador
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void Principal_Load(object sender, EventArgs e)
        {
            // this.IniciarProceso();
        }

        private void Principal_Shown(object sender, EventArgs e)
        {
            // Se hace una breve pausa para dar tiempo a que el ejecutable se cierre completamente
            Application.DoEvents();
            System.Threading.Thread.Sleep(900);

            // Se inicial el proceso de actualización
            this.IniciarProceso();
        }

        private void btnAccion_Click(object sender, EventArgs e)
        {
            if (this.btnAccion.Text == "Reintentar")
            {
                this.btnAccion.Text = "Procesando..";
                this.btnAccion.Enabled = false;
                this.IniciarProceso();
            }
        }

        #endregion

        #region [ Métodos ]

        private void IniciarProceso()
        {
            this.lsvArchivos.Items.Clear();
            this.pgbActualizacion.Value = 0;

            // Se crean los directorios
            var aDirs = Directory.GetDirectories(Inicio.RutaAct, "*", SearchOption.AllDirectories);
            foreach (string sDir in aDirs)
                this.lsvArchivos.Items.Add(sDir).Tag = "D";
            // Se llenan los archivos
            var aArchivos = Directory.GetFiles(Inicio.RutaAct, "*", SearchOption.AllDirectories);
            foreach (string sArchivo in aArchivos)
                this.lsvArchivos.Items.Add(sArchivo);
            
            // Se empieza a realizar la copia
            int iErrores = 0;
            this.pgbActualizacion.Maximum = this.lsvArchivos.Items.Count;
            foreach (ListViewItem oItem in this.lsvArchivos.Items)
            {
                string sArchivo = oItem.SubItems[0].Text;
                string sRutaRelativa = sArchivo.Replace(Inicio.RutaAct, "");

                // Se intenta hacer la copia
                try
                {
                    if (oItem.Tag == null)
                        File.Copy(sArchivo, (Inicio.RutaLocal + sRutaRelativa), true);
                    else
                        Directory.CreateDirectory(Inicio.RutaLocal + sRutaRelativa);
                    oItem.SubItems.Add("Copiado");
                    oItem.ForeColor = Color.Green;
                }
                catch (Exception e)
                {
                    oItem.SubItems.Add("Error");
                    oItem.SubItems.Add(e.Message);
                    oItem.ForeColor = Color.Red;
                    iErrores++;
                }

                this.pgbActualizacion.PerformStep();
                Application.DoEvents();
            }

            if (iErrores > 0)
            {
                this.btnAccion.Enabled = true;
                this.btnAccion.Text = "Reintentar";
            }
            else
            {
                this.VerEjecutarScriptAdicional();
                Inicio.ActualizacionCorrecta = true;
                this.Close();
            }
        }

        private void VerEjecutarScriptAdicional()
        {
            if (File.Exists(Inicio.RutaAct + Inicio.ScriptAd))
                Process.Start(Inicio.RutaLocal + Inicio.ScriptAd);
        }

        #endregion
                                                
    }
}
