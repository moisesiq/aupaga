using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using System.Threading;

namespace Refaccionaria.App
{
    partial class LogIn : Form
    {
        private static bool existe = false;
        private static int intentos = 0;

        public static LogIn Instance
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

            internal static readonly LogIn instance = new LogIn();
        }

        public LogIn()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void txtContrasena_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.btnAceptar_Click(sender, null);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                SplashScreen.Show(new Splash());
                if (!loadConfiguraciones())
                {
                    Negocio.Helper.MensajeError("No se pudo cargar la configuración inicial.", GlobalClass.NombreApp);
                    return;
                }

                if (txtUsuario.TextLength.Equals(0) || txtContrasena.TextLength.Equals(0))
                {
                    intentos += 1;
                    Negocio.Helper.MensajeAdvertencia("Los datos proporcionados son incorrectos." + " Intentos restantes: " + (3 - intentos).ToString(), GlobalClass.NombreApp);
                    if (intentos > 2)
                    {
                        this.Close();
                    }
                    return;
                }
                var usuario = General.GetEntity<Usuario>(u => u.NombreUsuario.Equals(txtUsuario.Text.ToLower()) && u.Contrasenia.Equals(txtContrasena.Text.ToLower()) && u.Estatus.Equals(true));
                if (usuario != null)
                {
                    var usuarioGlobal = new UsuarioSis();
                    usuarioGlobal.UsuarioID = usuario.UsuarioID;
                    usuarioGlobal.NombreUsuario = usuario.NombreUsuario;
                    usuarioGlobal.NombrePersona = usuario.NombrePersona;
                    usuarioGlobal.Perfiles = General.GetListOf<UsuarioPerfilesView>(up => up.UsuarioID.Equals(usuario.UsuarioID));

                    var ppv = General.GetListOf<PerfilPermisosView>().ToList();
                    var permisos = new List<PerfilPermisosView>();

                    foreach (var perfil in usuarioGlobal.Perfiles)
                    {
                        var items = ppv.Where(p => p.PerfilID == perfil.PerfilID);
                        foreach (var item in items)
                            permisos.Add(item);
                    }

                    usuarioGlobal.Permisos = permisos;
                    GlobalClass.UsuarioGlobal = usuarioGlobal;
                    existe = true;
                    this.Close();
                }
                if (!existe)
                {
                    intentos += 1;
                    Negocio.Helper.MensajeAdvertencia("Los datos proporcionados son incorrectos." + " Intentos restantes: " + (3 - intentos).ToString(), GlobalClass.NombreApp);
                    if (intentos > 2)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string msj = string.Empty;
                if (ex.InnerException == null)
                    msj = string.Format("msj:{0}.", ex.Message);
                else
                    msj = string.Format("msj:{0}. inner:{1}.", ex.Message, ex.InnerException);
                Helper.MensajeError(msj, GlobalClass.NombreApp);
            }
            SplashScreen.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Principal principal = Principal.Instance;
            principal.Salir();
        }

        private void LogIn_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!existe)
            {
                btnCancelar_Click(sender, e);
            }
        }

        private void LogIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        private void LogIn_Load(object sender, EventArgs e)
        {
#if DEBUG
            try
            {
                var usuario = General.GetEntity<Usuario>(u => u.NombreUsuario.Equals("admin") && u.Contrasenia.Equals("toor"));
                var usuarioGlobal = new UsuarioSis();
                usuarioGlobal.UsuarioID = usuario.UsuarioID;
                usuarioGlobal.NombreUsuario = usuario.NombreUsuario;
                usuarioGlobal.NombrePersona = usuario.NombrePersona;
                usuarioGlobal.Perfiles = Negocio.General.GetListOf<UsuarioPerfilesView>(up => up.UsuarioID.Equals(usuario.UsuarioID));                
                var ppv = General.GetListOf<PerfilPermisosView>().ToList();
                var permisos = new List<PerfilPermisosView>();

                foreach (var perfil in usuarioGlobal.Perfiles)
                {
                    var items = ppv.Where(p => p.PerfilID == perfil.PerfilID);
                    foreach (var item in items)
                        permisos.Add(item);
                }
                usuarioGlobal.Permisos = permisos;
                GlobalClass.UsuarioGlobal = usuarioGlobal;
                existe = true;
                loadConfiguraciones();
                this.Close();
            }
            catch (Exception ex)
            {
                string msj = string.Empty;
                if (ex.InnerException == null)
                    msj = string.Format("msj:{0}.", ex.Message);
                else
                    msj = string.Format("msj:{0}. inner:{1}.", ex.Message, ex.InnerException);
                Helper.MensajeError(msj, GlobalClass.NombreApp);
                this.btnCancelar_Click(sender, null);
            }
#endif
        }

        #endregion

        #region [ Metodos ]

        public void TryImproveEntity()
        {
            Thread backgroundThread = new Thread(new ThreadStart(this.improveThread));
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
        }

        private void improveThread()
        {
            try
            {
                SplashScreen.Show(new Splash());
            }
            catch (Exception ex)
            {
                Helper.MensajeError(string.Format("msj:{0}. inner:{1}.", ex.Message, ex.InnerException), GlobalClass.NombreApp);
            }
        }

        private bool loadConfiguraciones()
        {
            try
            {
                var configuraciones = Negocio.General.GetListOf<Configuracion>(c => c.ConfiguracionID > 0);
                foreach (var configuracion in configuraciones)
                {
                    switch (configuracion.Nombre)
                    {
                        case "IVA":
                            GlobalClass.ConfiguracionGlobal.IVA = Negocio.Helper.ConvertirDecimal(configuracion.Valor);
                            break;

                        case "pathImagenes":
                            GlobalClass.ConfiguracionGlobal.pathImagenes = Properties.Settings.Default.RutaImagenes.ToString();
                            break;

                        case "pathImagenesMovimientos":
                            GlobalClass.ConfiguracionGlobal.pathImagenesMovimientos = Properties.Settings.Default.RutaImagenesMovimientos.ToString();
                            break;

                        default:
                            break;
                    }
                }

                GlobalClass.ConfiguracionGlobal.pathReportes = string.Format("{0}{1}", System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "\\Reportes\\");
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                return false;
            }
            return true;
        }

        public static bool VerPermiso(string sPermiso, bool bMostrarMensaje)
        {
            if (GlobalClass.UsuarioGlobal.VerPermiso(sPermiso))
            {
                return true;
            }
            else
            {
                var Permiso = Negocio.General.GetEntity<Permiso>(p => p.NombrePermiso.Equals(sPermiso));
                if (bMostrarMensaje && Permiso != null)
                    Negocio.Helper.MensajeAdvertencia(Permiso.MensajeDeError, GlobalClass.NombreApp);
                return false;
            }
        }

        #endregion

    }
}
