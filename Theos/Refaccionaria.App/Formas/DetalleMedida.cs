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
    public partial class DetalleMedida : DetalleBase
    {
        Medida Medida;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleMedida Instance
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

            internal static readonly DetalleMedida instance = new DetalleMedida();
        }

        public DetalleMedida()
        {
            InitializeComponent();
        }

        public DetalleMedida(int Id)
        {
            InitializeComponent();
            try
            {
                //Medida = General.GetEntityById<Medida>("Medida", "MedidaID", Id);
                Medida = Datos.GetEntity<Medida>(c => c.MedidaID == Id && c.Estatus);
                if (Medida == null)
                    throw new EntityNotFoundException(Id.ToString(), "Medida");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleMedida_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                txtNombreMedida.Focus();
            }
            else
            {
                if (Medida.MedidaID > 0)
                {
                    this.Text = "Modificar";                    
                    txtNombreMedida.Text = Medida.NombreMedida;
                }
            }
        }

        private void DetalleMedida_KeyDown(object sender, KeyEventArgs e)
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
                    var medida = new Medida()
                    {
                        NombreMedida = txtNombreMedida.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<Medida>(medida);
                }
                else
                {
                    Medida.NombreMedida = txtNombreMedida.Text;
                    Medida.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Medida.FechaModificacion = DateTime.Now;
                    Medida.Estatus = true;
                    Datos.SaveOrUpdate<Medida>(Medida);
                }
                new Notificacion("Medida Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                medidas.Instance.CustomInvoke<medidas>(m => m.ActualizarListado());                                
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

        #region [ Metodos]

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
                var item = Datos.GetEntity<Medida>(m => m.NombreMedida.Equals(txtNombreMedida.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe una Medida con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.MedidaID != Medida.MedidaID)
                {
                    Util.MensajeError("Ya existe una Medida con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }            
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreMedida.Text == "")
                this.cntError.PonerError(this.txtNombreMedida, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion
        
    }
}
