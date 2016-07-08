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
    public partial class DetalleSistema : DetalleBase
    {
        Sistema Sistema;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleSistema Instance
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

            internal static readonly DetalleSistema instance = new DetalleSistema();
        }

        public DetalleSistema()
        {
            InitializeComponent();
        }

        public DetalleSistema(int Id)
        {
            InitializeComponent();
            try
            {
                // Sistema = General.GetEntityById<Sistema>("Sistema", "SistemaID", Id);
                Sistema = Datos.GetEntity<Sistema>(c => c.SistemaID == Id && c.Estatus);
                if (Sistema == null)
                    throw new EntityNotFoundException(Id.ToString(), "Sistema");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleSistema_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                txtNombreSistema.Focus();
            }
            else
            {
                if (Sistema.SistemaID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreSistema.Text = Sistema.NombreSistema;
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
                    var sistema = new Sistema()
                    {
                        NombreSistema = txtNombreSistema.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<Sistema>(sistema);
                }
                else
                {
                    Sistema.NombreSistema = txtNombreSistema.Text;
                    Sistema.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Sistema.FechaModificacion = DateTime.Now;
                    Sistema.Estatus = true;
                    Datos.SaveOrUpdate<Sistema>(Sistema);
                }
                new Notificacion("Sistema Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                sistemas.Instance.CustomInvoke<sistemas>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleSistema_KeyDown(object sender, KeyEventArgs e)
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
                var item = Datos.GetEntity<Sistema>(s => s.NombreSistema.Equals(txtNombreSistema.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe un Sistema con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.SistemaID != Sistema.SistemaID)
                {
                    Util.MensajeError("Ya existe un Sistema con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreSistema.Text == "")
                this.cntError.PonerError(this.txtNombreSistema, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
