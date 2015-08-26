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
    public partial class DetalleProveedorObservacion : DetalleBase
    {
        public int ProveedorId = 0;
        ProveedorObservacion ProveedorObservacion;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleProveedorObservacion Instance
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

            internal static readonly DetalleProveedorObservacion instance = new DetalleProveedorObservacion();
        }

        public DetalleProveedorObservacion()
        {
            InitializeComponent();
        }

        public DetalleProveedorObservacion(int Id)
        {
            InitializeComponent();
            try
            {
                ProveedorObservacion = Negocio.General.GetEntityById<ProveedorObservacion>("ProveedorObservacion", "ProveedorObservacionID", Id);
                if (ProveedorObservacion == null)
                    throw new EntityNotFoundException(Id.ToString(), "ProveedorObservacion");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleProveedorObservacion_Load(object sender, EventArgs e)
        {
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Negocio.Helper.ClearTextBoxes(this);
                txtObservacion.Focus();
            }
            else
            {
                if (ProveedorObservacion.ProveedorObservacionID > 0)
                {
                    this.Text = "Modificar";
                    txtObservacion.Text = ProveedorObservacion.Observacion;
                }
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                int Id = 0;
                if (EsNuevo)
                {
                    var observacion = new ProveedorObservacion()
                    {
                        ProveedorID = ProveedorId,
                        Observacion = txtObservacion.Text,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Negocio.General.SaveOrUpdate<ProveedorObservacion>(observacion, observacion);
                    Id = observacion.ProveedorID;
                }
                else
                {
                    ProveedorObservacion.Observacion = txtObservacion.Text;
                    ProveedorObservacion.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ProveedorObservacion.FechaModificacion = DateTime.Now;
                    ProveedorObservacion.Estatus = true;
                    Negocio.General.SaveOrUpdate<ProveedorObservacion>(ProveedorObservacion, ProveedorObservacion);
                    Id = ProveedorObservacion.ProveedorID;
                }
                new Notificacion("Observación Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                proveedorObservaciones.Instance.CustomInvoke<proveedorObservaciones>(m => m.ActualizarListado(Id));
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleProveedorObservacion_KeyDown(object sender, KeyEventArgs e)
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

        #region [ Metodos]

        private bool Validaciones()
        {
            try
            {
                var item = Negocio.General.GetEntity<ProveedorObservacion>(o => o.Observacion.Equals(txtObservacion.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe una Observación con esa descripción, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ProveedorObservacionID != ProveedorObservacion.ProveedorObservacionID)
                {
                    Negocio.Helper.MensajeError("Ya existe una Observación con esa descripción, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtObservacion.Text == "")
                this.cntError.PonerError(this.txtObservacion, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
