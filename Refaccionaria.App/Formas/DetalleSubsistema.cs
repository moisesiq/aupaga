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
    public partial class DetalleSubsistema : DetalleBase
    {
        Subsistema Subsistema;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleSubsistema Instance
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

            internal static readonly DetalleSubsistema instance = new DetalleSubsistema();
        }

        public DetalleSubsistema()
        {
            InitializeComponent();
        }

        public DetalleSubsistema(int Id)
        {
            InitializeComponent();
            try
            {
                Subsistema = Negocio.General.GetEntityById<Subsistema>("Subsistema", "SubsistemaID", Id);
                if (Subsistema == null)
                    throw new EntityNotFoundException(Id.ToString(), "Subsistema");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleSubsistema_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Negocio.Helper.ClearTextBoxes(this);
                cboSistema.Focus();
            }
            else
            {
                if (Subsistema.SubsistemaID > 0)
                {
                    this.Text = "Modificar";
                    cboSistema.SelectedValue = Subsistema.SistemaID;
                    txtNombreSubsistema.Text = Subsistema.NombreSubsistema;
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
                    var subsistema = new Subsistema()
                    {
                        SistemaID = Negocio.Helper.ConvertirEntero(cboSistema.SelectedValue),
                        NombreSubsistema = txtNombreSubsistema.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Negocio.General.SaveOrUpdate<Subsistema>(subsistema, subsistema);
                }
                else
                {
                    Subsistema.SistemaID = Negocio.Helper.ConvertirEntero(cboSistema.SelectedValue);
                    Subsistema.NombreSubsistema = txtNombreSubsistema.Text;
                    Subsistema.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Subsistema.FechaModificacion = DateTime.Now;
                    Subsistema.Estatus = true;
                    Negocio.General.SaveOrUpdate<Subsistema>(Subsistema, Subsistema);
                }
                new Notificacion("Subsistema Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                subsistemas.Instance.CustomInvoke<subsistemas>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleSubsistema_KeyDown(object sender, KeyEventArgs e)
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
                cboSistema.DataSource = Negocio.General.GetListOf<Sistema>(s => s.Estatus.Equals(true));
                cboSistema.DisplayMember = "NombreSistema";
                cboSistema.ValueMember = "SistemaID";
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
                if (EsNuevo.Equals(true))
                {
                    var sistemaId = Negocio.Helper.ConvertirEntero(cboSistema.SelectedValue);
                    var item = Negocio.General.GetEntity<Subsistema>(s => s.SistemaID.Equals(sistemaId) && s.NombreSubsistema.Equals(txtNombreSubsistema.Text));
                    if (item != null)
                    {
                        Negocio.Helper.MensajeError("Ya existe un Subsistema con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
                else if (EsNuevo.Equals(false))
                {
                    var item = Negocio.General.GetEntity<Subsistema>(s => s.SistemaID.Equals(Subsistema.SistemaID) && s.NombreSubsistema.Equals(txtNombreSubsistema.Text));
                    if ((item != null) && item.SubsistemaID != Subsistema.SubsistemaID)
                    {
                        Negocio.Helper.MensajeError("Ya existe un Subsistema con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreSubsistema.Text == "")
                this.cntError.PonerError(this.txtNombreSubsistema, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
