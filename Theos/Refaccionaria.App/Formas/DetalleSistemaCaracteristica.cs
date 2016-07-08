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
    public partial class DetalleSistemaCaracteristica : DetalleBase
    {
        ParteSistema ParteSistema;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleSistemaCaracteristica Instance
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

            internal static readonly DetalleSistemaCaracteristica instance = new DetalleSistemaCaracteristica();
        }

        public DetalleSistemaCaracteristica()
        {
            InitializeComponent();
        }

        public DetalleSistemaCaracteristica(int Id)
        {
            InitializeComponent();
            try
            {
                //ParteSistema = General.GetEntityById<ParteSistema>("ParteSistema", "ParteSistemaID", Id);
                ParteSistema = Datos.GetEntity<ParteSistema>(c => c.ParteSistemaID == Id && c.Estatus);
                if (ParteSistema == null)
                    throw new EntityNotFoundException(Id.ToString(), "ParteSistema");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]


        private void DetalleSistemaCaracteristica_Load(object sender, EventArgs e)
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
                if (ParteSistema.ParteSistemaID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreSistema.Text = ParteSistema.NombreParteSistema;
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
                    var ParteSistema = new ParteSistema()
                    {
                        NombreParteSistema = txtNombreSistema.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<ParteSistema>(ParteSistema);
                }
                else
                {
                    ParteSistema.NombreParteSistema = txtNombreSistema.Text;
                    ParteSistema.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ParteSistema.FechaModificacion = DateTime.Now;
                    ParteSistema.Estatus = true;
                    Datos.SaveOrUpdate<ParteSistema>(ParteSistema);
                }
                new Notificacion("Sistema Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                sistemasCaracteristicas.Instance.CustomInvoke<sistemasCaracteristicas>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleSistemaCaracteristica_KeyDown(object sender, KeyEventArgs e)
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
                var item = Datos.GetEntity<ParteSistema>(m => m.NombreParteSistema.Equals(txtNombreSistema.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe un Sistema con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ParteSistemaID != ParteSistema.ParteSistemaID)
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
