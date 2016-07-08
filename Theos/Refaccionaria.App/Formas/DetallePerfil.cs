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
    public partial class DetallePerfil : DetalleBase
    {
        Perfil Perfil;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        private enum PerfilOperaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        public static DetallePerfil Instance
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

            internal static readonly DetallePerfil instance = new DetallePerfil();
        }

        public DetallePerfil()
        {
            InitializeComponent();
        }

        public DetallePerfil(int Id)
        {
            InitializeComponent();
            try
            {
                //Perfil = General.GetEntityById<Perfil>("Perfil", "PerfilID", Id);
                Perfil = Datos.GetEntity<Perfil>(c => c.PerfilID == Id && c.Estatus);
                if (Perfil == null)
                    throw new EntityNotFoundException(Id.ToString(), "Perfil");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetallePerfil_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                txtNombrePerfil.Focus();
            }
            else
            {
                if (Perfil.PerfilID > 0)
                {
                    this.Text = "Modificar";
                    var permisos = Datos.GetListOf<PerfilPermisosView>(p => p.PerfilID.Equals(Perfil.PerfilID));
                    foreach (var permiso in permisos)
                    {
                        for (int i = 0; i < clbPermisos.Items.Count; i++)
                        {
                            var x = (Permiso)clbPermisos.Items[i];
                            if (x.PermisoID == permiso.PermisoID)
                            {
                                clbPermisos.SetItemChecked(i, true);
                            }
                        }
                    }
                    txtNombrePerfil.Text = Perfil.NombrePerfil;
                }
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                var lista = new List<int>();
                foreach (object itemChecked in clbPermisos.CheckedItems)
                {
                    Permiso castedItem = itemChecked as Permiso;
                    lista.Add(castedItem.PermisoID);
                }
                if (EsNuevo)
                {
                    var per = new Perfil()
                    {
                        NombrePerfil = txtNombrePerfil.Text,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<Perfil>(per);
                    UpdateUsuarioPerfiles(per.PerfilID, lista);
                }
                else
                {
                    Perfil.NombrePerfil = txtNombrePerfil.Text;
                    Perfil.FechaModificacion = DateTime.Now;
                    Perfil.Estatus = true;
                    Datos.SaveOrUpdate<Perfil>(Perfil);
                    UpdateUsuarioPerfiles(Perfil.PerfilID, lista);
                }
                new Notificacion("Perfil Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                usuarios.Instance.CustomInvoke<usuarios>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetallePerfil_KeyDown(object sender, KeyEventArgs e)
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
                ((ListBox)clbPermisos).DataSource = Datos.GetListOf<Permiso>(p => p.PermisoID > 0);
                ((ListBox)clbPermisos).DisplayMember = "NombrePermiso";
                ((ListBox)clbPermisos).ValueMember = "PermisoID";
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private static void UpdateUsuarioPerfiles(int perfilId, IEnumerable<int> values)
        {
            var permisosActuales = Datos.GetListOf<PerfilPermisosView>(p => p.PerfilID.Equals(perfilId));
            var selectedValues = new Dictionary<int, int>();

            foreach (var item in values)
            {
                selectedValues.Add(item, (int)PerfilOperaciones.Add);
            }

            foreach (var item in permisosActuales)
            {
                if (selectedValues.ContainsKey(item.PermisoID))
                {
                    selectedValues[item.PermisoID] = (int)PerfilOperaciones.None;
                }
                else
                {
                    selectedValues[item.PermisoID] = (int)PerfilOperaciones.Delete;
                }
            }

            foreach (var item in selectedValues)
            {
                if (item.Value == (int)PerfilOperaciones.Add) //add new
                {
                    var perfilPermiso = new PerfilPermiso
                    {
                        PerfilID = perfilId,
                        PermisoID = Util.Entero(item.Key),
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<PerfilPermiso>(perfilPermiso);
                }
                else if (item.Value == (int)PerfilOperaciones.Delete) //search and delete
                {
                    var perfilPermiso = Datos.GetEntity<PerfilPermiso>(p => p.PerfilID.Equals(perfilId) && p.PermisoID.Equals(item.Key));
                    if (perfilPermiso != null)
                        Datos.Delete<PerfilPermiso>(perfilPermiso);
                }
            }
        }

        private bool Validaciones()
        {
            try
            {
                var item = Datos.GetEntity<Perfil>(m => m.NombrePerfil.Equals(txtNombrePerfil.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe un Perfil con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.PerfilID != Perfil.PerfilID)
                {
                    Util.MensajeError("Ya existe un Perfil con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombrePerfil.Text == "")
                this.cntError.PonerError(this.txtNombrePerfil, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.clbPermisos.CheckedItems.Count < 1)
                this.cntError.PonerError(this.clbPermisos, "Debe seleccionar al menos un Permiso.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
