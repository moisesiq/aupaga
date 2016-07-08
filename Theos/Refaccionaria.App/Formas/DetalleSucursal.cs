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
    public partial class DetalleSucursal : DetalleBase
    {
        Sucursal Sucursal;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleSucursal Instance
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

            internal static readonly DetalleSucursal instance = new DetalleSucursal();
        }

        public DetalleSucursal()
        {
            InitializeComponent();
        }

        public DetalleSucursal(int Id)
        {
            InitializeComponent();
            try
            {
                //Sucursal = General.GetEntityById<Sucursal>("Sucursal", "SucursalID", Id);
                Sucursal = Datos.GetEntity<Sucursal>(c => c.SucursalID == Id && c.Estatus);
                if (Sucursal == null)
                    throw new EntityNotFoundException(Id.ToString(), "Sucursal");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleSusursal_Load(object sender, EventArgs e)
        {
            this.cmbGerente.CargarDatos("UsuarioID", "NombreUsuario", Datos.GetListOf<Usuario>(c => c.Activo && c.Estatus));

            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                txtNombreSucursal.Focus();
            }
            else
            {
                if (Sucursal.SucursalID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreSucursal.Text = Sucursal.NombreSucursal;
                    txtIP.Text = Sucursal.DireccionIP;
                    this.cmbGerente.SelectedValue = this.Sucursal.GerenteID;
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
                    var sucursal = new Sucursal()
                    {
                        NombreSucursal = txtNombreSucursal.Text,
                        DireccionIP = txtIP.Text,
                        GerenteID = Util.Entero(this.cmbGerente.SelectedValue),
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<Sucursal>(sucursal);
                }
                else
                {
                    Sucursal.NombreSucursal = txtNombreSucursal.Text;
                    Sucursal.DireccionIP = txtIP.Text;
                    this.Sucursal.GerenteID = Util.Entero(this.cmbGerente.SelectedValue);
                    Sucursal.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Sucursal.FechaModificacion = DateTime.Now;
                    Sucursal.Estatus = true;
                    Datos.SaveOrUpdate<Sucursal>(Sucursal);
                }
                new Notificacion("Sucursal Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                sucursales.Instance.CustomInvoke<sucursales>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleSucursal_KeyDown(object sender, KeyEventArgs e)
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
                var item = Datos.GetEntity<Sucursal>(s => s.NombreSucursal.Equals(txtNombreSucursal.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe una Sucursal con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.SucursalID != Sucursal.SucursalID)
                {
                    Util.MensajeError("Ya existe una Sucursal con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreSucursal.Text == "")
                this.cntError.PonerError(this.txtNombreSucursal, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtIP.Text == "")
                this.cntError.PonerError(this.txtIP, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
