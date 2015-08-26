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
    public partial class DetalleMarca : DetalleBase
    {
        Marca Marca;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleMarca Instance
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

            internal static readonly DetalleMarca instance = new DetalleMarca();
        }

        public DetalleMarca()
        {
            InitializeComponent();
        }

        public DetalleMarca(int Id)
        {
            InitializeComponent();
            try
            {
                Marca = Negocio.General.GetEntityById<Marca>("Marca", "MarcaID", Id);
                if (Marca == null)
                    throw new EntityNotFoundException(Id.ToString(), "Marca");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleMarca_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Negocio.Helper.ClearTextBoxes(this);
                txtNombreMarca.Focus();
            }
            else
            {
                if (Marca.MarcaID > 0)
                {
                    this.Text = "Modificar";
                    txtNombreMarca.Text = Marca.NombreMarca;
                }
            }
        }

        private void DetalleMarca_KeyDown(object sender, KeyEventArgs e)
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
                    var marca = new Marca()
                    {
                        NombreMarca = txtNombreMarca.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Negocio.General.SaveOrUpdate<Marca>(marca, marca);
                }
                else
                {
                    Marca.NombreMarca = txtNombreMarca.Text;
                    Marca.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Marca.FechaModificacion = DateTime.Now;
                    Marca.Estatus = true;
                    Negocio.General.SaveOrUpdate<Marca>(Marca, Marca);
                }
                new Notificacion("Marca Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                marcas.Instance.CustomInvoke<marcas>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {
            try
            {
                var item = Negocio.General.GetEntity<Marca>(m => m.NombreMarca.Equals(txtNombreMarca.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe una Marca con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.MarcaID != Marca.MarcaID)
                {
                    Negocio.Helper.MensajeError("Ya existe una Marca con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreMarca.Text == "")
                this.cntError.PonerError(this.txtNombreMarca, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
