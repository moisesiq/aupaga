using System.Windows.Forms;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ValidarUsuario : Form
    {
        List<string> Permisos;
        bool CumplirTodosLosPermisos;

        #region [ Propiedades ]

        public int SucursalInicialID { get; set; }
        public Usuario UsuarioSel { get; private set; }
        public bool Valido { get; private set; }

        private bool _MostrarSeleccionDeSucursal = true;
        public bool MostrarSeleccionDeSucursal
        {
            get { return this._MostrarSeleccionDeSucursal; }
            set
            {
                this._MostrarSeleccionDeSucursal = value;
                this.lblSucursal.Visible = value;
                this.cmbSucursal.Visible = value;
                if (value)
                    this.Height = 116;
                else
                    this.Height = 88;

            }
        }

        public int SucursalID
        {
            get { return Helper.ConvertirEntero(this.cmbSucursal.SelectedValue); }
            set { this.cmbSucursal.SelectedValue = value; }
        }

        #endregion

        public ValidarUsuario()
        {
            this.InitializeComponent();
        }

        public ValidarUsuario(List<string> oPermisos, bool bCumplirTodosLosPermisos)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;

            this.MostrarSeleccionDeSucursal = false;

            this.Permisos = oPermisos;
            this.CumplirTodosLosPermisos = bCumplirTodosLosPermisos;
        }

        #region [ Eventos ]

        private void ValidarUsuario_Load(object sender, System.EventArgs e)
        {
            // Se cargan los datos de los usuarios
            // this.cmbUsuario.CargarDatos("UsuarioID", "NombreUsuario", General.GetListOf<Usuario>(q => q.Estatus));
            // this.cmbUsuario.SelectedIndex = -1;
            //this.cmbUsuario.SelectedValue = this.UsuarioSel.UsuarioID;
            
            // Se cargan los datos de las sucursales
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.cmbSucursal.SelectedValue = this.SucursalInicialID;

            this.ActiveControl = this.txtContrasenia;
        }

        private void btnAceptar_Click(object sender, System.EventArgs e)
        {
            // Se valida el usuario
            this.UsuarioSel = UtilDatos.ObtenerUsuarioDeContrasenia(this.txtContrasenia.Text);
            this.Valido = (this.UsuarioSel != null);
            if (!this.Valido)
            {
                UtilLocal.MensajeAdvertencia("Usuario o contraseña inválidos.");
                this.RegresarAContrasenia();
                return;
            }
            // Se valida que el usuario esté activo
            if (!this.UsuarioSel.Activo)
            {
                UtilLocal.MensajeAdvertencia("El Usuario especificado no está activo en el sistema.");
                this.RegresarAContrasenia();
                return;
            }

            // Se valida la sucursal, si aplica
            if (this.MostrarSeleccionDeSucursal && this.SucursalID != GlobalClass.SucursalID)
            {
                bool bPermiso = UtilDatos.ValidarPermiso(this.UsuarioSel.UsuarioID, "Sistema.VerOtrasSucursales", true);
                if (!bPermiso)
                {
                    this.RegresarAContrasenia();
                    return;
                }
            }

            // Se validan los permisos, si hay
            if (this.Permisos != null)
            {
                var oResV = UtilDatos.ValidarUsuarioPermisos(this.UsuarioSel.UsuarioID, this.Permisos, this.CumplirTodosLosPermisos);
                if (oResV.Error)
                {
                    UtilLocal.MensajeAdvertencia(oResV.Mensaje);
                    this.RegresarAContrasenia();
                    return;
                }
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void RegresarAContrasenia()
        {
            this.txtContrasenia.Focus();
            this.txtContrasenia.SelectAll();
        }

        #endregion

        private void button1_Click(object sender, System.EventArgs e)
        {
            var fu = Cargando.Mostrar();
            //fu.Show(Principal.Instance);
            System.Threading.Thread.Sleep(2500);
        }
    }
}
