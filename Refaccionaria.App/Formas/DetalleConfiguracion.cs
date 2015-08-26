using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using System.Configuration;

namespace Refaccionaria.App
{
    public partial class DetalleConfiguracion : DetalleBase
    {
        Configuracion Configuracion;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleConfiguracion Instance
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

            internal static readonly DetalleConfiguracion instance = new DetalleConfiguracion();
        }

        public DetalleConfiguracion()
        {
            InitializeComponent();
        }

        public DetalleConfiguracion(int Id)
        {
            InitializeComponent();
            try
            {
                Configuracion = Negocio.General.GetEntityById<Configuracion>("Configuracion", "ConfiguracionID", Id);
                if (Configuracion == null)
                    throw new EntityNotFoundException(Id.ToString(), "Configuracion");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleConfiguracion_Load(object sender, EventArgs e)
        {
            if (Configuracion.ConfiguracionID > 0)
            {
                this.Text = "Modificar";
                if (Configuracion.Nombre == "pathImagenes")
                {
                    txtNombre.Text = Configuracion.Nombre;
                    txtValor.Text = Properties.Settings.Default.RutaImagenes.ToString();
                    txtDescripcion.Text = Configuracion.Descripcion;                    
                }
                else
                {
                    txtNombre.Text = Configuracion.Nombre;
                    txtValor.Text = Configuracion.Valor;
                    txtDescripcion.Text = Configuracion.Descripcion;
                }
            }
        }

        private void DetalleConfiguracion_KeyDown(object sender, KeyEventArgs e)
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

            try
            {
                if (EsNuevo)
                {

                }
                else
                {
                    /* Se previene que se edite desde el sistema, los parámetros de imágenes
                    if (Configuracion.Nombre == "pathImagenes")
                    {
                        if (!System.IO.Directory.Exists(this.txtValor.Text))
                            System.IO.Directory.CreateDirectory(this.txtValor.Text);

                        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                        Properties.Settings.Default.RutaImagenes = txtValor.Text;
                        Properties.Settings.Default.Save();

                        GlobalClass.ConfiguracionGlobal.pathImagenes = Properties.Settings.Default.RutaImagenes.ToString();

                    }
                    if (Configuracion.Nombre == "pathImagenesMovimientos")
                    {
                        if (!System.IO.Directory.Exists(this.txtValor.Text))
                            System.IO.Directory.CreateDirectory(this.txtValor.Text);

                        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        
                        Properties.Settings.Default.RutaImagenesMovimientos = txtValor.Text;
                        Properties.Settings.Default.Save();

                        GlobalClass.ConfiguracionGlobal.pathImagenesMovimientos = Properties.Settings.Default.RutaImagenesMovimientos.ToString();
                    }
                    else
                    { */
                        // Configuracion.Nombre = txtNombre.Text;
                        Configuracion.Valor = txtValor.Text;
                        Configuracion.Descripcion = txtDescripcion.Text;
                        Negocio.General.SaveOrUpdate<Configuracion>(Configuracion, Configuracion);
                    // }
                }
                new Notificacion("Configuracion Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                configuraciones.Instance.CustomInvoke<configuraciones>(m => m.ActualizarListado());
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

        #region [ Metodos ]

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            //if (this.txtNombre.Text == "")
            //    this.cntError.PonerError(this.txtNombre, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtValor.Text == "")
                this.cntError.PonerError(this.txtValor, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            //if (this.txtDescripcion.Text == "")
            //    this.cntError.PonerError(this.txtDescripcion, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion
                
    }
}
