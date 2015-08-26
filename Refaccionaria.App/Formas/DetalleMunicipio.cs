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
    public partial class DetalleMunicipio : DetalleBase
    {

        Municipio Municipio;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleMunicipio Instance
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

            internal static readonly DetalleMunicipio instance = new DetalleMunicipio();
        }

        public DetalleMunicipio()
        {
            InitializeComponent();
        }

        public DetalleMunicipio(int Id)
        {
            InitializeComponent();
            try
            {
                Municipio = General.GetEntityById<Municipio>("Municipio", "MunicipioID", Id);
                if (Municipio == null)
                    throw new EntityNotFoundException(Id.ToString(), "Municipio");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleMunicipio_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Helper.ClearTextBoxes(this);
                this.cboEstado.Enabled = true;
                this.cboEstado.Focus();
            }
            else
            {
                if (Municipio.MunicipioID > 0)
                {
                    this.Text = "Modificar";
                    this.cboEstado.SelectedValue = Municipio.EstadoID;
                    this.cboEstado.Enabled = false;
                    this.txtNombreMunicipio.Text = Municipio.NombreMunicipio;
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
                    var municipio = new Municipio()
                    {
                        EstadoID = Helper.ConvertirEntero(this.cboEstado.SelectedValue),
                        NombreMunicipio = this.txtNombreMunicipio.Text
                    };
                    Guardar.Generico<Municipio>(municipio);
                }
                else
                {
                    Municipio.EstadoID = Helper.ConvertirEntero(this.cboEstado.SelectedValue);
                    Municipio.NombreMunicipio = this.txtNombreMunicipio.Text;
                    Guardar.Generico<Municipio>(Municipio);
                }
                new Notificacion("Municipio Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                municipios.Instance.CustomInvoke<municipios>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleSistema_KeyDown(object sender, KeyEventArgs e)
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
                this.cboEstado.DataSource = General.GetListOf<Estado>(s => s.Estatus.Equals(true));
                this.cboEstado.DisplayMember = "NombreEstado";
                this.cboEstado.ValueMember = "EstadoID";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {
            try
            {
                int estadoId = Helper.ConvertirEntero(this.cboEstado.SelectedValue);
                var item = General.GetEntity<Municipio>(m => m.EstadoID == estadoId && m.NombreMunicipio.Equals(this.txtNombreMunicipio.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Helper.MensajeError("Ya existe un Municipio en ese Estado con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.MunicipioID != Municipio.MunicipioID)
                {
                    Helper.MensajeError("Ya existe un Municipio en ese Estado con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtNombreMunicipio.Text == "")
                this.cntError.PonerError(this.txtNombreMunicipio, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
