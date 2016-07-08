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
    public partial class DetalleProveedor : DetalleBase
    {
        Proveedor Proveedor;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleProveedor Instance
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

            internal static readonly DetalleProveedor instance = new DetalleProveedor();
        }

        public DetalleProveedor()
        {
            InitializeComponent();
        }

        public DetalleProveedor(int Id)
        {
            InitializeComponent();
            try
            {
                //Proveedor = General.GetEntityById<Proveedor>("Proveedor", "ProveedorID", Id);
                Proveedor = Datos.GetEntity<Proveedor>(c => c.ProveedorID == Id && c.Estatus);
                if (Proveedor == null)
                    throw new EntityNotFoundException(Id.ToString(), "Proveedor");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleProveedor_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                txtNombreProveedor.Focus();
            }
            else
            {
                if (Proveedor.ProveedorID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreProveedor.Text = Proveedor.NombreProveedor;
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
                    var proveedor = new Proveedor()
                    {
                        NombreProveedor = txtNombreProveedor.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<Proveedor>(proveedor);
                }
                else
                {
                    Proveedor.NombreProveedor = txtNombreProveedor.Text;
                    Proveedor.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Proveedor.FechaModificacion = DateTime.Now;
                    Proveedor.Estatus = true;
                    Datos.SaveOrUpdate<Proveedor>(Proveedor);
                }
                new Notificacion("Proveedor Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                proveedores.Instance.CustomInvoke<proveedores>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleProveedor_KeyDown(object sender, KeyEventArgs e)
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
                var item = Datos.GetEntity<Proveedor>(p => p.NombreProveedor.Equals(txtNombreProveedor.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe un Proveedor con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ProveedorID != Proveedor.ProveedorID)
                {
                    Util.MensajeError("Ya existe un Proveedor con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreProveedor.Text == "")
                this.cntError.PonerError(this.txtNombreProveedor, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
