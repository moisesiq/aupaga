using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Data.Objects;
using System.Diagnostics;

using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class Principal : Form
    {
        public Principal()
        {
            this.InitializeComponent();
            // Se hace pantalla completa
            // this.EntrarPantallaCompleta();
            // Se asignan eventos para imágenes a los botones de las opciones
            this.AsignarDisenioBotones();
            this.AsignarDisenioEtiquetasBotones();
            // Se asignan eventos clic para los accesos directos
            this.AsignarAccesosDirectos();
            // Se inicia el timer de la hora
            this.tmrHora.Enabled = true;
        }

        public static Principal Instance
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

            internal static readonly Principal instance = new Principal();
        }

        Usuario oUsuario;
        int SucursalID;

        #region [ Eventos ]

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.NumPad0):
                case (Keys.Control | Keys.D0):
                    this.statusStripPrincipal.Focus();
                    break;
                
                // Para mandarlo a alguna de las opciones, si aplica
                default:
                    var oControl = Helper.ControlAlFrente(this.panelContenedor);
                    if (oControl == null) break;
                    switch (oControl.Name)
                    {
                        // Para la opción de Ventas
                        case "Ventas":
                            Ventas.Instance.EjecutarAccesoDeTeclado(keyData);
                            return false;
                        // Para la opción de Contabilidad
                        case "Contabilidad":
                            Contabilidad.Instance.EjecutarAccesoDeTeclado(keyData);
                            return false;
                        // Para la opción de Mantenimiento
                        case "mantenimiento":
                            mantenimiento.Instance.EjecutarAccesoDeTeclado(keyData);
                            return false;
                    }
                    break;
            }
                        
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Principal_Load(object sender, EventArgs e)
        {
            this.lblToolTip.Text = GlobalClass.NombreApp;
            this.ActiveControl = this.statusStripPrincipal;
                        
            //
            //this.TryImproveEntity();

            //this.TryUpdate();

            //var usuarios = General.GetListOf<Modelo.Usuario>(u => u.Estatus);
            //if (usuarios != null)
            //{
            //    foreach (var usuarioC in usuarios)
            //    {
            //        usuarioC.Contrasenia = Helper.Encriptar(usuarioC.Contrasenia.ToLower());
            //        General.SaveOrUpdate<Modelo.Usuario>(usuarioC, usuarioC);
            //    }
            //}
            
            Application.DoEvents();
            
            // Se cargan parámetros de configuración
            if (!loadConfiguraciones())
            {
                Cargando.Cerrar();
                Helper.MensajeError("No se pudo cargar la configuración inicial.", GlobalClass.NombreApp);
                // SplashScreen.Close();
                this.Close();
                return;
            }

            // Se obtiene el usuario, pidiendo la contraseña
            bool bInicio = this.MostrarInicioDeSesion();
            if (!bInicio)
            {
                this.Close();
                return;
            }

            // Se inicializa la sesión
            Proc.InicializarSesion(this.SucursalID, this.oUsuario);

            // Se verfica el modo de ejecución, para avisar si no es producción
            if (!GlobalClass.Produccion)
            {
                //this.lblModo.Text = ("** Modo de " + GlobalClass.Modo + " **");
                this.Text += string.Format(" ** (Modo de {0}) **", GlobalClass.Modo);
                this.statusStripPrincipal.BackColor = System.Drawing.Color.DarkGray;
                //this.lblModo.BackColor = this.statusStripPrincipal.BackColor;
                //this.lblModo.Visible = true;
            }
        }

        private void tmrHora_Tick(object sender, EventArgs e)
        {
            DateTime dMinuto = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).AddMinutes(1);

            int iDif = (dMinuto - DateTime.Now).Seconds;
            if (iDif > 0)
            {
                this.tmrHora.Interval = (iDif * 1000);
                this.sslFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            }
            else
            {
                this.tmrHora.Interval = (60 * 1000);
                this.sslFecha.Text = DateTime.Now.AddSeconds(1).ToString("dd/MM/yyyy HH:mm");
            }
        }

        private void btnMantenimiento_Click(object sender, EventArgs e)
        {
            //addControlInPanel(mantenimiento.Instance);
            //this.CargarControl("mantenimiento");
            this.CargarControl(mantenimiento.Instance);
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            if (!UtilDatos.ValidarPermiso("Administracion.CatalagosGenerales.Ver", true))
                return;
            this.CargarControl(catalogosGenerales.Instance);
            
            
        }

        private void btnContabilidad_Click(object sender, EventArgs e)
        {
            // Se verifica que el usuario tenga permisos para entrar a esta opción
            if (!UtilDatos.ValidarPermiso("Contabilidad.Acceso", true))
                return;
            //
            this.CargarControl(Contabilidad.Instance);
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            //this.addControlInPanel(Ventas.Instance);
            //this.CargarControl("Ventas");
            this.CargarControl(Ventas.Instance);
        }

        private void btnAutorizaciones_Click(object sender, EventArgs e)
        {
            //this.addControlInPanel(Autorizaciones.Instance);
            //this.CargarControl("Autorizaciones");
            this.CargarControl(Autorizaciones.Instance);
        }

        private void btnCambiosSistema_Click(object sender, EventArgs e)
        {
            // Se solicita la validación de Usuario
            var ResU = UtilDatos.ValidarObtenerUsuario();
            if (ResU.Error)
                return;

            if (UtilDatos.ValidarPermiso(ResU.Respuesta.UsuarioID, "CambiosSistema.Agregar"))
                CambiosSistema.Instance.PermisoAgregar = "";
            else
                CambiosSistema.Instance.PermisoAgregar = "No tienes permisos para agregar cambios.";
            if (UtilDatos.ValidarPermiso(ResU.Respuesta.UsuarioID, "CambiosSistema.Modificar"))
                CambiosSistema.Instance.PermisoModificar = "";
            else
                CambiosSistema.Instance.PermisoModificar = "No tienes permisos para modificar cambios.";

            //this.addControlInPanel(CambiosSistema.Instance);
            //this.CargarControl("CambiosSistema");
            this.CargarControl(CambiosSistema.Instance);
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            // Se verifica que el usuario tenga permisos para entrar a esta opción
            if (!UtilDatos.ValidarPermiso("CuadroDeControl.Acceso", true))
                return;
            //
            this.CargarControl(CuadroDeControl.Instance);
        }

        private void btnSesion_Click(object sender, EventArgs e)
        {
            this.CambioSesion();
        }

        #endregion

        #region [ Metodos ]

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
                            // Este parámetro ya no se usará. Moi 07/05/2015
                            // GlobalClass.ConfiguracionGlobal.pathImagenesMovimientos = Properties.Settings.Default.RutaImagenesMovimientos.ToString();
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
                var init = General.GetEntity<Modelo.ParteEstatus>(p => p.Estatus);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(string.Format("msj:{0}. inner:{1}.", ex.Message, ex.InnerException), GlobalClass.NombreApp);
            }
        }

        private void TryUpdate()
        {
            try
            {
                UpdateConfig config = new UpdateConfig();
                //if (config.LoadConfig(Properties.Settings.Default.ActualizadorConfigURL, string.Empty, string.Empty, string.Empty, false))
                //{
                //    if (config != null)
                //    {
                //        var versionRet = new Version(config.AvailableVersion);
                //        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                //        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;

                //        //Revisa versión
                //        if (versionRet > currentVersion)
                //        {
                //            var act = string.Format("{0}{1}", path, "Updater.exe");
                //            if (File.Exists(act))
                //                Process.Start(act);

                //            Process[] processlist = Process.GetProcesses();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void Salir()
        {
            this.Close();
        }

        private void addControlInPanel(object controlHijo)
        {
            if (this.panelContenedor.Controls.Count > 0)
                this.panelContenedor.Controls.RemoveAt(0);
            UserControl usc = controlHijo as UserControl;
            usc.Dock = DockStyle.Fill;
            this.panelContenedor.Controls.Add(usc);
            this.panelContenedor.Tag = usc;
            usc.Show();
        }

        public void CargarControl(Control oControl)
        {
            if (!this.panelContenedor.Controls.Contains(oControl))
            {
                oControl.Dock = DockStyle.Fill;
                this.panelContenedor.Controls.Add(oControl);
            }
            this.panelContenedor.Tag = oControl;
            oControl.BringToFront();
            oControl.Show();
        }

        private void CargarControl(string sControl)
        {
            Control oControl;
            if (this.panelContenedor.Controls.ContainsKey(sControl))
            {
                oControl = this.panelContenedor.Controls[sControl];
            }
            else
            {
                //
                var oTipo = this.GetType();
                oControl = (oTipo.Assembly.CreateInstance(oTipo.Namespace + "." + sControl) as Control);
                oControl.Name = sControl;
                //
                oControl.Dock = DockStyle.Fill;
                this.panelContenedor.Controls.Add(oControl);
            }
            this.panelContenedor.Tag = oControl;
            oControl.BringToFront();
            oControl.Show();
        }

        private void AsignarDisenioBotones()
        {
            // Se asignan eventos para imágenes a los botones de las opciones
            string sPrefijo = "og";
            foreach (ToolStripItem oItem in this.statusStripPrincipal.Items)
            {
                if (oItem is ButtonStripItem)
                {
                    var oBoton = (oItem as ButtonStripItem);
                    string sNombre = oItem.Name.Replace("btn", "");

                    oBoton.FlatStyle = FlatStyle.Flat;
                    oBoton.FlatAppearance.BorderSize = 0;

                    oItem.MouseEnter += new EventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre + "Sobre") as Image);
                        this.lblToolTip.Text = oBoton.ToolTipText;
                    });

                    oItem.MouseLeave += new EventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre) as Image);
                        this.lblToolTip.Text = "";
                    });

                    oItem.MouseDown += new MouseEventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre + "Clic") as Image);
                    });

                    oItem.MouseUp += new MouseEventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre + "Sobre") as Image);
                    });
                }
            }
        }

        private void AsignarDisenioEtiquetasBotones()
        {
            // Se asignan eventos para imágenes a los botones de las opciones
            string sPrefijo = "og";
            foreach (ToolStripItem oItem in this.statusStripPrincipal.Items)
            {
                if (oItem is ToolStripStatusLabel && oItem.Name.StartsWith("sslAcc_"))
                {
                    var oBoton = (oItem as ToolStripStatusLabel);  // Se usa para que el evento quede en el botón actual. No tocar :s
                    string sNombre = oItem.Name.Replace("sslAcc_", "");

                    oItem.MouseEnter += new EventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre + "Sobre") as Image);
                        this.lblToolTip.Text = oBoton.ToolTipText;
                    });

                    oItem.MouseLeave += new EventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre) as Image);
                        this.lblToolTip.Text = "";
                    });

                    oItem.MouseDown += new MouseEventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre + "Clic") as Image);
                    });

                    oItem.MouseUp += new MouseEventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject(sPrefijo + sNombre + "Sobre") as Image);
                    });
                }
            }
        }

        private void AsignarAccesosDirectos()
        {
            foreach (ToolStripItem oItem in this.statusStripPrincipal.Items)
            {
                if (oItem is ToolStripStatusLabel)
                {
                    if (oItem.Name.StartsWith("sslAcc_"))
                    {
                        var oBoton = (oItem as ToolStripStatusLabel);  // Se usa para que el evento quede en el botón actual. No tocar :s
                        oItem.Click += new EventHandler((s, e) =>
                        {
                            if (oBoton.Tag != null)
                            {
                                string sComando = Helper.ConvertirCadena(oBoton.Tag);
                                string sEjec = sComando, sArgumentos = "";
                                if (sComando.Contains('\"'))
                                    sEjec = sComando.Extraer("\"", "\"");
                                else if (sComando.Contains(' '))
                                    sEjec = sComando.Izquierda(sComando.IndexOf(' '));
                                sArgumentos = sComando.Replace(sEjec, "");
                                // Se manda ejecutar el acceso
                                Process.Start(sEjec, sArgumentos);
                            }
                        });
                    }
                }
            }
        }

        private bool MostrarInicioDeSesion()
        {
            var frmInicio = new ValidarUsuario();
            frmInicio.SucursalInicialID = GlobalClass.SucursalID;
            frmInicio.Text = "Inicio de Sesión";
            if (frmInicio.ShowDialog(this) == DialogResult.OK)
            {
                this.oUsuario = frmInicio.UsuarioSel;
                this.SucursalID = frmInicio.SucursalID;
            }
            else
            {
                this.oUsuario = null;
                this.SucursalID = 0;
            }
            frmInicio.Dispose();

            return (this.SucursalID > 0);
        }
                        
        private void CambioSesion()
        {
            Control oControlAc = Helper.ControlAlFrente(this.panelContenedor);
            // Se ocultan todos los controles del formulario principal
            foreach (Control oControl in this.panelContenedor.Controls)
                oControl.Hide();
            this.panelContenedor.Tag = null;

            //
            bool bSesionNueva = false;
            while (true)
            {
                // Se muestra la ventana de inicio de sesion
                bool bInicio = this.MostrarInicioDeSesion();
                if (!bInicio)
                {
                    if (UtilLocal.MensajePregunta("Existe una sesión abierta. Si continúas, el programa se cerrará y la información se perderá. ¿Estás seguro que deseas continuar?")
                        == DialogResult.Yes)
                    {
                        this.Close();
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }
                // Se verifica si es otro usuario
                if (this.oUsuario.UsuarioID != GlobalClass.UsuarioGlobal.UsuarioID)
                {
                    if (UtilLocal.MensajePregunta("Existe una sesión abierta con otro Usuario. Si continúas, la información se perderá. ¿Estás seguro que deseas continuar?")
                        == DialogResult.Yes)
                        bSesionNueva = true;
                    else
                        continue;
                }
                // Se verifica si se inicia sesión con otra sucursal
                if (this.SucursalID != GlobalClass.SucursalID)
                {
                    if (UtilLocal.MensajePregunta("Existe una sesión abierta con otra Sucursal. Si continúas, la información se perderá. ¿Estás seguro que deseas continuar?")
                        == DialogResult.Yes)
                        bSesionNueva = true;
                    else
                        continue;
                }
                //
                break;
            }

            // Se inicia o continúa la sesión
            if (bSesionNueva)
            {
                // Se cierran las opciones abiertas
                while (this.panelContenedor.Controls.Count > 0)
                {
                    Control oControl = this.panelContenedor.Controls[0];
                    this.panelContenedor.Controls.Remove(oControl);
                    oControl.Dispose();
                    oControl = null;
                }
                //
                Proc.InicializarSesion(this.SucursalID, this.oUsuario);
            }
            else
            {
                if (oControlAc != null)
                    this.CargarControl(oControlAc.Name);
            }
        }

        #endregion

        #region [ Públicos ]

        public void LlenarTitulo()
        {
            this.Text = string.Format("{0} v{1} - {2} - {3}", Application.ProductName, Application.ProductVersion,
                GlobalClass.NombreTienda, GlobalClass.UsuarioGlobal.NombreUsuario);
        }

        #endregion

        #region [ Ayuda Asincrono ]

        public void MostrarMensaje(object o)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object>(this.MostrarMensaje), o);
            }
            else
            {
                var oMensaje = (o as DosVal<string, string>);
                Helper.MensajeInformacion(oMensaje.Valor2, oMensaje.Valor1);
            }
        }

        public void VerInventarioConteoPendiente(object o)
        {
            // Se comprueba si hay conteos pendientes en la sucursal, cuando aplique
            if (this.InvokeRequired)
            {
                string sMensaje = "";
                var oConteosPen = UtilDatos.InventarioUsuariosConteoPendiente();
                foreach (string sUsuario in oConteosPen)
                    sMensaje += string.Format("El usuario {0} no ha concluido su conteo.\n", sUsuario);
                if (sMensaje != "")
                    this.Invoke(new Action<object>(this.MostrarMensaje), new DosVal<string, string>("Recordatorio de Invenario", sMensaje));
            }
        }

        #endregion

    }
}
