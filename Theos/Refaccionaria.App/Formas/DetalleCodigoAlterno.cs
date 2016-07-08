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
                //Parte = General.GetEntityById<Parte>("Parte", "ParteID", Id);
                Parte = Datos.GetEntity<Parte>(c => c.ParteID == Id && c.Estatus);
                if (Parte == null)
                    throw new EntityNotFoundException(Id.ToString(), "Parte");
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                UtilLocal.ClearTextBoxes(this);
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
                                MarcaParteID = Util.Entero(this.cboMarca.SelectedValue),
                                CodigoAlterno = codigo,
                                RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                            };
                            Datos.Guardar<ParteCodigoAlterno>(codigoAlterno);
                        }
                    }
                }
                else
                {
                    //Modelo.MarcaID = Util.ConvertirEntero(cboMarca.SelectedValue);
                    //Modelo.NombreModelo = txtNombreModelo.Text;
                    //Modelo.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    //Modelo.FechaModificacion = DateTime.Now;
                    //Modelo.Estatus = true;
                    //General.SaveOrUpdate<Refaccionaria.Modelo.Modelo>(Modelo, Modelo);
                }
                new Notificacion("Código Alterno Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                //modelos.Instance.CustomInvoke<modelos>(m => m.ActualizarListado());
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
            try
            {
                cboMarca.DataSource = Datos.GetListOf<MarcaPartesView>(m => m.MarcaParteID > -1);
                cboMarca.DisplayMember = "NombreMarcaParte";
                cboMarca.ValueMember = "MarcaParteID";
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
                var marcaId = Util.Entero(cboMarca.SelectedValue);
                if (EsNuevo.Equals(true))
                {                    
                    var item = Datos.GetEntity<ParteCodigoAlterno>(m => m.ParteID == Parte.ParteID 
                        && m.MarcaParteID.Equals(marcaId) && m.CodigoAlterno.Equals(txtCodigoAlterno.Text));
                    if (item != null)
                    {
                        Util.MensajeError("Ya existe un Código Alterno con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
                else if (EsNuevo.Equals(false))
                {
                    var item = Datos.GetEntity<ParteCodigoAlterno>(m => m.ParteID == Parte.ParteID
                        && m.MarcaParteID.Equals(marcaId) && m.CodigoAlterno.Equals(txtCodigoAlterno.Text));
                    if ((item != null) && item.ParteCodigoAlternoID != Parte.ParteID)
                    {
                        Util.MensajeError("Ya existe un Código Alterno con ese nombre, intente con otro.", GlobalClass.NombreApp);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            if (this.txtCodigoAlterno.Text == "")
                this.cntError.PonerError(this.txtCodigoAlterno, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
