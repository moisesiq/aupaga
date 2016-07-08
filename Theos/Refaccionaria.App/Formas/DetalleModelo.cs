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
    public partial class DetalleModelo : DetalleBase
    {
        TheosProc.Modelo Modelo;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleModelo Instance
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

            internal static readonly DetalleModelo instance = new DetalleModelo();
        }

        public DetalleModelo()
        {
            InitializeComponent();
        }

        public DetalleModelo(int Id)
        {
            InitializeComponent();
            try
            {
                Modelo = Datos.GetEntity<TheosProc.Modelo>(c => c.ModeloID == Id && c.Estatus);
                if (Modelo == null)
                    throw new EntityNotFoundException(Id.ToString(), "Modelo");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleModelo_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
                cboMarca.Focus();
            }
            else
            {
                if (Modelo.ModeloID > 0)
                {
                    this.Text = "Modificar";
                    cboMarca.SelectedValue = Modelo.MarcaID;
                    txtNombreModelo.Text = Modelo.NombreModelo;
                }
            }
        }

        private void DetalleModelo_KeyDown(object sender, KeyEventArgs e)
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
                    var modelo = new Modelo()
                    {
                        MarcaID = Util.Entero(cboMarca.SelectedValue),
                        NombreModelo = txtNombreModelo.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<Modelo>(modelo);
                }
                else
                {
                    Modelo.MarcaID = Util.Entero(cboMarca.SelectedValue);
                    Modelo.NombreModelo = txtNombreModelo.Text;
                    Modelo.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Modelo.FechaModificacion = DateTime.Now;
                    Modelo.Estatus = true;
                    Datos.SaveOrUpdate<Modelo>(Modelo);
                }
                new Notificacion("Modelo Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                modelos.Instance.CustomInvoke<modelos>(m => m.ActualizarListado());
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
                cboMarca.DataSource = Datos.GetListOf<MarcasView>(m => m.MarcaID > -1);
                cboMarca.DisplayMember = "NombreMarca";
                cboMarca.ValueMember = "MarcaID";
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
                if (EsNuevo.Equals(true))
                {
                    var marcaId = Util.Entero(cboMarca.SelectedValue);
                    var item = Datos.GetEntity<Modelo>(m => m.MarcaID.Equals(marcaId) && m.NombreModelo.Equals(txtNombreModelo.Text));
                    if (item != null)
                    {
                        Util.MensajeError("Ya existe un Modelo con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
                else if (EsNuevo.Equals(false))
                {
                    var item = Datos.GetEntity<Modelo>(m => m.MarcaID.Equals(Modelo.MarcaID) && m.NombreModelo.Equals(txtNombreModelo.Text));
                    if ((item != null) && item.ModeloID != Modelo.ModeloID)
                    {
                        Util.MensajeError("Ya existe un Modelo con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreModelo.Text == "")
                this.cntError.PonerError(this.txtNombreModelo, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
