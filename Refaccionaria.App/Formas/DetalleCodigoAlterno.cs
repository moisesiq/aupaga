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
    public partial class DetalleCodigoAlterno : Refaccionaria.App.DetalleBase
    {

        Parte Parte;        
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        private static DetalleCodigoAlterno aForm = null;

        public static DetalleCodigoAlterno Instance()
        {
            if (aForm == null)
            {
                aForm = new DetalleCodigoAlterno();
            }
            return aForm;
        }

        public DetalleCodigoAlterno()
        {
            InitializeComponent();
        }

        public DetalleCodigoAlterno(int Id)
        {
            InitializeComponent();
            try
            {
                Parte = General.GetEntityById<Parte>("Parte", "ParteID", Id);
                if (Parte == null)
                    throw new EntityNotFoundException(Id.ToString(), "Parte");
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleCodigoAlterno_Load(object sender, EventArgs e)
        {
            CargaInicial();
            this.Text = string.Format("{0}: {1}", "Codigos Alterno de", Parte.NumeroParte);            
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Helper.ClearTextBoxes(this);
                cboMarca.Focus();
            }
            else
            {
                //if (Modelo.ModeloID > 0)
                //{
                //    this.Text = "Modificar";
                //    cboMarca.SelectedValue = Modelo.MarcaID;
                //    txtNombreModelo.Text = Modelo.NombreModelo;
                //}
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
                    var codigos = this.txtCodigoAlterno.Text.Split(',');
                    foreach (var codigo in codigos)
                    {
                        if (!string.IsNullOrEmpty(codigo))
                        {
                            var codigoAlterno = new ParteCodigoAlterno()
                            {
                                ParteID = Parte.ParteID,
                                MarcaParteID = Helper.ConvertirEntero(this.cboMarca.SelectedValue),
                                CodigoAlterno = codigo,
                                RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                            };
                            Guardar.Generico<ParteCodigoAlterno>(codigoAlterno);
                        }
                    }
                }
                else
                {
                    //Modelo.MarcaID = Negocio.Helper.ConvertirEntero(cboMarca.SelectedValue);
                    //Modelo.NombreModelo = txtNombreModelo.Text;
                    //Modelo.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    //Modelo.FechaModificacion = DateTime.Now;
                    //Modelo.Estatus = true;
                    //Negocio.General.SaveOrUpdate<Refaccionaria.Modelo.Modelo>(Modelo, Modelo);
                }
                new Notificacion("Código Alterno Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                //modelos.Instance.CustomInvoke<modelos>(m => m.ActualizarListado());
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
            try
            {
                cboMarca.DataSource = General.GetListOf<MarcaPartesView>(m => m.MarcaParteID > -1);
                cboMarca.DisplayMember = "NombreMarcaParte";
                cboMarca.ValueMember = "MarcaParteID";
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
                var marcaId = Helper.ConvertirEntero(cboMarca.SelectedValue);
                if (EsNuevo.Equals(true))
                {                    
                    var item = General.GetEntity<ParteCodigoAlterno>(m => m.ParteID == Parte.ParteID 
                        && m.MarcaParteID.Equals(marcaId) && m.CodigoAlterno.Equals(txtCodigoAlterno.Text));
                    if (item != null)
                    {
                        Helper.MensajeError("Ya existe un Código Alterno con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
                else if (EsNuevo.Equals(false))
                {
                    var item = General.GetEntity<ParteCodigoAlterno>(m => m.ParteID == Parte.ParteID
                        && m.MarcaParteID.Equals(marcaId) && m.CodigoAlterno.Equals(txtCodigoAlterno.Text));
                    if ((item != null) && item.ParteCodigoAlternoID != Parte.ParteID)
                    {
                        Helper.MensajeError("Ya existe un Código Alterno con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtCodigoAlterno.Text == "")
                this.cntError.PonerError(this.txtCodigoAlterno, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
