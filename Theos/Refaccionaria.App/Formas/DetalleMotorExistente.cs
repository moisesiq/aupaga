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
    public partial class DetalleMotorExistente : DetalleBase
    {
        MotorExistente MotorExistente;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleMotorExistente Instance
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

            internal static readonly DetalleMotorExistente instance = new DetalleMotorExistente();
        }

        public DetalleMotorExistente()
        {
            InitializeComponent();
        }

        public DetalleMotorExistente(int Id)
        {
            InitializeComponent();
            try
            {
                //MotorExistente = General.GetEntityById<MotorExistente>("MotorExistente", "MotorExistenteID", Id);
                MotorExistente = Datos.GetEntity<MotorExistente>(c => c.MotorExistenteID == Id && c.Estatus);
                if (MotorExistente == null)
                    throw new EntityNotFoundException(Id.ToString(), "MotorExistente");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleMotorExistente_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                txtNombreMotor.Focus();
            }
            else
            {
                if (MotorExistente.MotorExistenteID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreMotor.Text = MotorExistente.NombreMotorExistente;
                }
            }
        }

        private void DetalleMotorExistente_KeyDown(object sender, KeyEventArgs e)
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
                    var motor = new MotorExistente()
                    {
                        NombreMotorExistente = txtNombreMotor.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<MotorExistente>(motor);
                }
                else
                {
                    MotorExistente.NombreMotorExistente = txtNombreMotor.Text;
                    MotorExistente.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    MotorExistente.FechaModificacion = DateTime.Now;
                    MotorExistente.Estatus = true;
                    Datos.SaveOrUpdate<MotorExistente>(MotorExistente);
                }
                new Notificacion("Motor Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                motoresExistentes.Instance.CustomInvoke<motoresExistentes>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
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
                var item = Datos.GetEntity<MotorExistente>(m => m.NombreMotorExistente.Equals(txtNombreMotor.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe un Motor con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.MotorExistenteID != MotorExistente.MotorExistenteID)
                {
                    Util.MensajeError("Ya existe un Motor con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreMotor.Text == "")
                this.cntError.PonerError(this.txtNombreMotor, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
