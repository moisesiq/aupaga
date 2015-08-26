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
    public partial class DetalleCiudad : DetalleBase
    {

        Ciudad Ciudad;
        Municipio Municipio;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleCiudad Instance
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

            internal static readonly DetalleCiudad instance = new DetalleCiudad();
        }

        public DetalleCiudad()
        {
            InitializeComponent();
        }

        public DetalleCiudad(int Id)
        {
            InitializeComponent();
            try
            {
                Ciudad = General.GetEntityById<Ciudad>("Ciudad", "CiudadID", Id);
                if (Ciudad == null)
                    throw new EntityNotFoundException(Id.ToString(), "Ciudad");
                Municipio = General.GetEntity<Municipio>(m => m.MunicipioID == Ciudad.MunicipioID);
                if (Municipio == null)
                    throw new EntityNotFoundException(Ciudad.MunicipioID.ToString(), "Municipio");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleCiudad_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Helper.ClearTextBoxes(this);
                this.cboEstado.Enabled = true;
                this.cboMunicipio.Enabled = true;
                this.cboEstado.Focus();
            }
            else
            {
                if (Ciudad.CiudadID > 0 && Municipio.MunicipioID > 0)
                {
                    this.Text = "Modificar";
                    this.cboEstado.SelectedValue = Municipio.EstadoID;
                    this.cboEstado.Enabled = false;
                    this.cboMunicipio.SelectedValue = Ciudad.MunicipioID;
                    this.cboMunicipio.Enabled = false;
                    this.txtCiudad.Text = Ciudad.NombreCiudad;
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
                    var ciudad = new Ciudad()
                    {
                        MunicipioID = Helper.ConvertirEntero(this.cboMunicipio.SelectedValue),
                        NombreCiudad = this.txtCiudad.Text
                    };
                    Guardar.Generico<Ciudad>(ciudad);
                }
                else
                {
                    Ciudad.MunicipioID = Helper.ConvertirEntero(this.cboMunicipio.SelectedValue);
                    Ciudad.NombreCiudad = this.txtCiudad.Text;
                    Guardar.Generico<Ciudad>(Ciudad);
                }
                new Notificacion("Ciudad Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                //ciudades.Instance.CustomInvoke<ciudades>(m => m.ActualizarListado());
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
        
        private void cboEstado_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (int.TryParse(this.cboEstado.SelectedValue.ToString(), out id))
                {
                    var listaMunicipios = General.GetListOf<Municipio>(m => m.EstadoID.Equals(id));
                    this.cboMunicipio.DataSource = listaMunicipios;
                    this.cboMunicipio.DisplayMember = "NombreMunicipio";
                    this.cboMunicipio.ValueMember = "MunicipioID";
                    //AutoCompleteStringCollection autMunicipio = new AutoCompleteStringCollection();
                    //foreach (var municipio in listaMunicipios) autMunicipio.Add(municipio.NombreMunicipio);
                    //this.cboMunicipio.AutoCompleteMode = AutoCompleteMode.Suggest;
                    //this.cboMunicipio.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    //this.cboMunicipio.AutoCompleteCustomSource = autMunicipio;
                    this.cboMunicipio.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
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
                int municipioId = Helper.ConvertirEntero(this.cboMunicipio.SelectedValue);
                var item = General.GetEntity<Ciudad>(c => c.MunicipioID == municipioId && c.NombreCiudad.Equals(this.txtCiudad.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Helper.MensajeError("Ya existe una Ciudad en ese Municipio con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.CiudadID != Ciudad.CiudadID)
                {
                    Helper.MensajeError("Ya existe una Ciudad en ese Municipio con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtCiudad.Text == "")
                this.cntError.PonerError(this.txtCiudad, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion
        
    }
}
