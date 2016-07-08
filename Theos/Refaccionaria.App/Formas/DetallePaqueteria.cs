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
    public partial class DetallePaqueteria : DetalleBase
    {
        ProveedorPaqueteria ProveedorPaqueteria;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetallePaqueteria Instance
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

            internal static readonly DetallePaqueteria instance = new DetallePaqueteria();
        }

        public DetallePaqueteria()
        {
            InitializeComponent();
        }

        public DetallePaqueteria(int Id)
        {
            InitializeComponent();
            try
            {
                //ProveedorPaqueteria = General.GetEntityById<ProveedorPaqueteria>("ProveedorPaqueteria", "ProveedorPaqueteriaID", Id);
                ProveedorPaqueteria = Datos.GetEntity<ProveedorPaqueteria>(c => c.ProveedorPaqueteriaID == Id && c.Estatus);
                if (ProveedorPaqueteria == null)
                    throw new EntityNotFoundException(Id.ToString(), "ProveedorPaqueteria");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetallePaqueteria_Load(object sender, EventArgs e)
        {
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                UtilLocal.ClearTextBoxes(this);
            }
            else
            {
                if (ProveedorPaqueteria.ProveedorPaqueteriaID > 0)
                {
                    this.Text = "Modificar";
                    var pp = Datos.GetEntity<ProveedorPaqueteria>(p => p.ProveedorPaqueteriaID.Equals(ProveedorPaqueteria.ProveedorPaqueteriaID));
                    if (pp != null)
                    {
                        this.txtNombrePaqueteria.Text = pp.NombrePaqueteria;
                        this.txtCalle.Text = pp.Calle;
                        this.txtColonia.Text = pp.Colonia;
                        this.txtCiudad.Text = pp.Ciudad;
                        this.txtEstado.Text = pp.Estado;
                        this.txtCP.Text = pp.CP;
                        this.txtTelefono.Text = pp.Telefono;
                        this.chkEntregaOcurre.Checked = pp.EntregaOcurre;
                    }
                }
            }
        }

        private void DetallePaqueteria_KeyDown(object sender, KeyEventArgs e)
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
                    var proveedorPaqueteria = new ProveedorPaqueteria()
                    {
                        NombrePaqueteria = this.txtNombrePaqueteria.Text,
                        Calle = this.txtCalle.Text,
                        Colonia = this.txtColonia.Text,
                        Ciudad = this.txtCiudad.Text,
                        Estado = this.txtEstado.Text,
                        CP = this.txtCP.Text,
                        Telefono = this.txtTelefono.Text,
                        EntregaOcurre = this.chkEntregaOcurre.Checked,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Datos.SaveOrUpdate<ProveedorPaqueteria>(proveedorPaqueteria);
                }
                else
                {
                    ProveedorPaqueteria.NombrePaqueteria = this.txtNombrePaqueteria.Text;
                    ProveedorPaqueteria.Calle = this.txtCalle.Text;
                    ProveedorPaqueteria.Colonia = this.txtColonia.Text;
                    ProveedorPaqueteria.Ciudad = this.txtCiudad.Text;
                    ProveedorPaqueteria.Estado = this.txtEstado.Text;
                    ProveedorPaqueteria.CP = this.txtCP.Text;
                    ProveedorPaqueteria.Telefono = this.txtTelefono.Text;
                    ProveedorPaqueteria.EntregaOcurre = this.chkEntregaOcurre.Checked;
                    ProveedorPaqueteria.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ProveedorPaqueteria.FechaModificacion = DateTime.Now;
                    ProveedorPaqueteria.Estatus = true;
                    Datos.SaveOrUpdate<ProveedorPaqueteria>(ProveedorPaqueteria);

                }
                new Notificacion("Paqueteria Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                paqueterias.Instance.CustomInvoke<paqueterias>(m => m.ActualizarListado());
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

        private bool Validaciones()
        {
            try
            {
                var item = Datos.GetEntity<ProveedorPaqueteria>(pp => pp.NombrePaqueteria.Equals(txtNombrePaqueteria.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Util.MensajeError("Ya existe una Paqueteria con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ProveedorPaqueteriaID != ProveedorPaqueteria.ProveedorPaqueteriaID)
                {
                    Util.MensajeError("Ya existe una Paqueteria con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombrePaqueteria.Text == "")
                this.cntError.PonerError(this.txtNombrePaqueteria, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
