using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class DetalleUsuario : DetalleBase
    {
        Usuario Usuario;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        private enum UsuarioOperaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        public static DetalleUsuario Instance
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

            internal static readonly DetalleUsuario instance = new DetalleUsuario();
        }

        public DetalleUsuario()
        {
            InitializeComponent();
        }

        public DetalleUsuario(int Id)
        {
            InitializeComponent();
            try
            {
                Usuario = Negocio.General.GetEntityById<Usuario>("Usuario", "UsuarioID", Id);
                if (Usuario == null)
                    throw new EntityNotFoundException(Id.ToString(), "Usuario");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleUsuario_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Negocio.Helper.ClearTextBoxes(this);
                txtNombrePersona.Focus();
            }
            else
            {
                if (Usuario.UsuarioID > 0)
                {
                    this.Text = "Modificar";
                    var perfiles = Negocio.General.GetListOf<UsuarioPerfilesView>(u => u.UsuarioID.Equals(Usuario.UsuarioID));
                    foreach (var perfil in perfiles)
                    {
                        for (int i = 0; i < clbPerfiles.Items.Count; i++)
                        {
                            var x = (Perfil)clbPerfiles.Items[i];
                            if (x.PerfilID == perfil.PerfilID)
                            {
                                clbPerfiles.SetItemChecked(i, true);
                            }
                        }
                    }
                    txtNombrePersona.Text = Usuario.NombrePersona;
                    txtNombreUsuario.Text = Usuario.NombreUsuario;
                    //txtContrasenia.Text = Usuario.Contrasenia;
                    this.txtContrasenia.Text = Helper.Desencriptar(Usuario.Contrasenia);
                    cboEstatus.SelectedValue = Usuario.Activo.Equals(true) ? 1 : 0;
                    if (Usuario.TipoUsuarioID.HasValue)
                        this.cmbTipoDeUsuario.SelectedValue = Usuario.TipoUsuarioID;
                    else
                        this.cmbTipoDeUsuario.SelectedIndex = -1;

                    //checar alertas

                    this.clbAlertas.SetItemChecked(0, Helper.ConvertirBool(Usuario.AlertaCalendarioClientes));
                    this.clbAlertas.SetItemChecked(1, Helper.ConvertirBool(Usuario.AlertaPedidos));
                    this.clbAlertas.SetItemChecked(2, Helper.ConvertirBool(Usuario.Alerta9500));
                    this.clbAlertas.SetItemChecked(3, Helper.ConvertirBool(Usuario.AlertaTraspasos));
                    this.clbAlertas.SetItemChecked(4, Helper.ConvertirBool(Usuario.AlertaDevFacturaCreditoAnt));

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
                foreach (object itemChecked in clbPerfiles.CheckedItems)
                {
                    Perfil castedItem = itemChecked as Perfil;
                    lista.Add(castedItem.PerfilID);
                }
                if (EsNuevo)
                {
                    var usr = new Usuario()
                    {
                        NombrePersona = txtNombrePersona.Text,
                        NombreUsuario = txtNombreUsuario.Text,
                        Contrasenia = Helper.Encriptar(txtContrasenia.Text),
                        FechaRegistro = DateTime.Now,
                        Activo = cboEstatus.SelectedValue.Equals(1),
                        TipoUsuarioID = (int?)this.cmbTipoDeUsuario.SelectedValue,
                        Estatus = true,
                        Actualizar = true,
                        AlertaCalendarioClientes = this.clbAlertas.GetItemChecked(0),
                        AlertaPedidos = this.clbAlertas.GetItemChecked(1),
                        Alerta9500 = this.clbAlertas.GetItemChecked(2),
                        AlertaTraspasos = this.clbAlertas.GetItemChecked(3),
                        AlertaDevFacturaCreditoAnt = this.clbAlertas.GetItemChecked(4)
                    };
                    Negocio.General.SaveOrUpdate<Usuario>(usr, usr);
                    UpdateUsuarioPerfiles(usr.UsuarioID, lista);

                    // Se crean las cuentas auxiliares indispensables
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Salarios, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.TiempoExtra, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.PremioDeAsistencia, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.PremioDePuntualidad, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Vacaciones, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.PrimaVacacional, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Aguinaldo, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Ptu, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Imss, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Ispt, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Infonavit, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.RetencionImss, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.SubsidioAlEmpleo, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.RetencionInfonavit, usr.UsuarioID);

                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.DeudoresDiversos, usr.UsuarioID);
                    ContaProc.CrearCuentaAuxiliar(usr.NombrePersona, Cat.ContaCuentasDeMayor.Nomina2Por, usr.UsuarioID);
                }
                else
                {
                    Usuario.NombrePersona = txtNombrePersona.Text;
                    Usuario.NombreUsuario = txtNombreUsuario.Text;
                    Usuario.Contrasenia = Helper.Encriptar(txtContrasenia.Text);
                    Usuario.FechaModificacion = DateTime.Now;
                    Usuario.Activo = cboEstatus.SelectedValue.Equals(1);
                    Usuario.TipoUsuarioID = (int?)this.cmbTipoDeUsuario.SelectedValue;
                    //alertas
                    Usuario.AlertaCalendarioClientes = this.clbAlertas.GetItemChecked(0);
                    Usuario.AlertaPedidos = this.clbAlertas.GetItemChecked(1);
                    Usuario.Alerta9500 = this.clbAlertas.GetItemChecked(2);
                    Usuario.AlertaTraspasos = this.clbAlertas.GetItemChecked(3);
                    Usuario.AlertaDevFacturaCreditoAnt = this.clbAlertas.GetItemChecked(4);

                    Negocio.General.SaveOrUpdate<Usuario>(Usuario, Usuario);
                    UpdateUsuarioPerfiles(Usuario.UsuarioID, lista);

                }
                new Notificacion("Usuario Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                usuarios.Instance.CustomInvoke<usuarios>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleUsuario_KeyDown(object sender, KeyEventArgs e)
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
            //Se validan los permisos
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
                cboEstatus.DataSource = Negocio.General.GetListOf<EstatusGeneralesView>(e => e.EstatusID > -1);
                cboEstatus.DisplayMember = "NombreEstatus";
                cboEstatus.ValueMember = "EstatusID";

                this.cmbTipoDeUsuario.CargarDatos("TipoUsuarioID", "TipoDeUsuario", General.GetListOf<TipoUsuario>());

                ((ListBox)clbPerfiles).DataSource = Negocio.General.GetListOf<Perfil>(p => p.PerfilID > 0);
                ((ListBox)clbPerfiles).DisplayMember = "NombrePerfil";
                ((ListBox)clbPerfiles).ValueMember = "PerfilID";

                ((ListBox)clbAlertas).Items.Add("Alerta de Calendario de Clientes");
                ((ListBox)clbAlertas).Items.Add("Alerta de Pedidos");
                ((ListBox)clbAlertas).Items.Add("Alerta de 9500");
                ((ListBox)clbAlertas).Items.Add("Alerta de Traspasos");
                ((ListBox)clbAlertas).Items.Add("Alerta de Devolución factura crédito de días ant.");
                
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private static void UpdateUsuarioPerfiles(int usuarioId, IEnumerable<int> values)
        {
            var perfilesActuales = Negocio.General.GetListOf<UsuarioPerfilesView>(u => u.UsuarioID.Equals(usuarioId));
            var selectedValues = new Dictionary<int, int>();

            foreach (var item in values)
            {
                selectedValues.Add(item, (int)UsuarioOperaciones.Add);
            }

            foreach (var item in perfilesActuales)
            {
                if (selectedValues.ContainsKey(item.PerfilID))
                {
                    selectedValues[item.PerfilID] = (int)UsuarioOperaciones.None;
                }
                else
                {
                    selectedValues[item.PerfilID] = (int)UsuarioOperaciones.Delete;
                }
            }

            foreach (var item in selectedValues)
            {
                if (item.Value == (int)UsuarioOperaciones.Add) //add new
                {
                    var usuarioPerfil = new UsuarioPerfil
                    {
                        UsuarioID = usuarioId,
                        PerfilID = Negocio.Helper.ConvertirEntero(item.Key),
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Negocio.General.SaveOrUpdate<UsuarioPerfil>(usuarioPerfil, usuarioPerfil);
                }
                else if (item.Value == (int)UsuarioOperaciones.Delete) //search and delete
                {
                    var usuarioPerfil = Negocio.General.GetEntity<UsuarioPerfil>(u => u.UsuarioID.Equals(usuarioId) && u.PerfilID.Equals(item.Key));
                    if (usuarioPerfil != null)
                        Negocio.General.Delete<UsuarioPerfil>(usuarioPerfil);
                }
            }
        }

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            try
            {
                var item = Negocio.General.GetEntity<Usuario>(m => m.NombreUsuario.Equals(txtNombreUsuario.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe un Usuario con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.UsuarioID != Usuario.UsuarioID)
                {
                    Negocio.Helper.MensajeError("Ya existe un Usuario con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }

                // Se valida que la contraseña no se repita
                string sContraseniaEnc = Helper.Encriptar(txtContrasenia.Text);
                item = Negocio.General.GetEntity<Usuario>(m => m.Contrasenia.Equals(sContraseniaEnc));
                if (EsNuevo.Equals(true) && item != null)
                {
                    this.cntError.PonerError(this.txtContrasenia, "Error al validar la contraseña. Intenta agregando o modificando algún caracter.");
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.UsuarioID != Usuario.UsuarioID)
                {
                    this.cntError.PonerError(this.txtContrasenia, "Error al validar la contraseña. Intenta agregando o modificando algún caracter.");
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }

            if (this.txtNombrePersona.Text == "")
                this.cntError.PonerError(this.txtNombrePersona, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtNombreUsuario.Text == "")
                this.cntError.PonerError(this.txtNombreUsuario, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtContrasenia.Text == "")
                this.cntError.PonerError(this.txtContrasenia, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.clbPerfiles.CheckedItems.Count < 1)
                this.cntError.PonerError(this.clbPerfiles, "Debe seleccionar al menos un Perfil.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion


    }
}
