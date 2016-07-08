using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class DetalleUbicacion : DetalleBase
    {
        ParteUbicacion ParteUbicacion;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleUbicacion Instance
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

            internal static readonly DetalleUbicacion instance = new DetalleUbicacion();
        }

        public DetalleUbicacion()
        {
            InitializeComponent();
        }

        public DetalleUbicacion(int Id)
        {
            InitializeComponent();
            try
            {
                //ParteUbicacion = General.GetEntityById<ParteUbicacion>("ParteUbicacion", "ParteUbicacionID", Id);
                ParteUbicacion = Datos.GetEntity<ParteUbicacion>(c => c.ParteUbicacionID == Id && c.Estatus);
                if (ParteUbicacion == null)
                    throw new EntityNotFoundException(Id.ToString(), "ParteUbicacion");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleUbicacion_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                txtNombreUbicacion.Focus();
            }
            else
            {
                if (ParteUbicacion.ParteUbicacionID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreUbicacion.Text = ParteUbicacion.NombreParteUbicacion;
                }
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
                    var ParteUbicacion = new ParteUbicacion()
                    {
                        NombreParteUbicacion = txtNombreUbicacion.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<ParteUbicacion>(ParteUbicacion);
                }
                else
                {
                    ParteUbicacion.NombreParteUbicacion = txtNombreUbicacion.Text;
                    ParteUbicacion.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ParteUbicacion.FechaModificacion = DateTime.Now;
                    ParteUbicacion.Estatus = true;
                    Datos.SaveOrUpdate<ParteUbicacion>(ParteUbicacion);
                }
                new Notificacion("Ubicación Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                ubicaciones.Instance.CustomInvoke<ubicaciones>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleUbicacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
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
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {
            try
            {
                var item = Datos.GetEntity<ParteUbicacion>(m => m.NombreParteUbicacion.Equals(txtNombreUbicacion.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe una Ubicación con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ParteUbicacionID != ParteUbicacion.ParteUbicacionID)
                {
                    Util.MensajeError("Ya existe una Ubicación con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreUbicacion.Text == "")
                this.cntError.PonerError(this.txtNombreUbicacion, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
