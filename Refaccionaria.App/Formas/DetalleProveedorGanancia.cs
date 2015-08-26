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
    public partial class DetalleProveedorGanancia : DetalleBase
    {
        public int ProveedorId;
        ProveedorGanancia ProveedorGanancia;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleProveedorGanancia Instance
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

            internal static readonly DetalleProveedorGanancia instance = new DetalleProveedorGanancia();
        }

        public DetalleProveedorGanancia()
        {
            InitializeComponent();
        }

        public DetalleProveedorGanancia(int Id)
        {
            InitializeComponent();
            try
            {
                ProveedorGanancia = Negocio.General.GetEntityById<ProveedorGanancia>("ProveedorGanancia", "ProveedorGananciaID", Id);
                if (ProveedorGanancia == null)
                    throw new EntityNotFoundException(Id.ToString(), "ProveedorGanancia");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos]

        private void DetalleProveedorGanancia_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Negocio.Helper.ClearTextBoxes(this);
                this.cboMarcaParte.Focus();
            }
            else
            {
                if (ProveedorGanancia.ProveedorGananciaID > 0)
                {
                    this.Text = "Modificar";
                    var pg = General.GetEntity<ProveedorGanancia>(p => p.ProveedorGananciaID.Equals(ProveedorGanancia.ProveedorGananciaID));
                    if (pg != null)
                    {
                        this.cboMarcaParte.SelectedValue = pg.MarcaParteID;
                        this.cboLinea.SelectedValue = pg.LineaID;
                        this.txtPorcentajeUno.Text = pg.PorcentajeUno.ToString();
                        this.txtPorcentajeDos.Text = pg.PorcentajeDos.ToString();
                        this.txtPorcentajeTres.Text = pg.PorcentajeTres.ToString();
                        this.txtPorcentajeCuatro.Text = pg.PorcentajeCuatro.ToString();
                        this.txtPorcentajeCinco.Text = pg.PorcentajeCinco.ToString();
                    }
                }
            }
        }

        private void DetalleProveedorGanancia_KeyDown(object sender, KeyEventArgs e)
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
                    var proveedorGanancia = new ProveedorGanancia()
                    {
                        ProveedorID = ProveedorId,
                        MarcaParteID = Negocio.Helper.ConvertirEntero(cboMarcaParte.SelectedValue),
                        LineaID = Negocio.Helper.ConvertirEntero(cboLinea.SelectedValue),
                        PorcentajeUno = Negocio.Helper.ConvertirDecimal(txtPorcentajeUno.Text),
                        PorcentajeDos = Negocio.Helper.ConvertirDecimal(txtPorcentajeDos.Text),
                        PorcentajeTres = Negocio.Helper.ConvertirDecimal(txtPorcentajeTres.Text),
                        PorcentajeCuatro = Negocio.Helper.ConvertirDecimal(txtPorcentajeCuatro.Text),
                        PorcentajeCinco = Negocio.Helper.ConvertirDecimal(txtPorcentajeCinco.Text),
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    Negocio.General.SaveOrUpdate<ProveedorGanancia>(proveedorGanancia, proveedorGanancia);
                }
                else
                {
                    ProveedorGanancia.MarcaParteID = Negocio.Helper.ConvertirEntero(cboMarcaParte.SelectedValue);
                    ProveedorGanancia.LineaID = Negocio.Helper.ConvertirEntero(cboLinea.SelectedValue);
                    ProveedorGanancia.PorcentajeUno = Negocio.Helper.ConvertirDecimal(txtPorcentajeUno.Text);
                    ProveedorGanancia.PorcentajeDos = Negocio.Helper.ConvertirDecimal(txtPorcentajeDos.Text);
                    ProveedorGanancia.PorcentajeTres = Negocio.Helper.ConvertirDecimal(txtPorcentajeTres.Text);
                    ProveedorGanancia.PorcentajeCuatro = Negocio.Helper.ConvertirDecimal(txtPorcentajeCuatro.Text);
                    ProveedorGanancia.PorcentajeCinco = Negocio.Helper.ConvertirDecimal(txtPorcentajeCinco.Text);
                    ProveedorGanancia.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ProveedorGanancia.FechaModificacion = DateTime.Now;
                    ProveedorGanancia.Estatus = true;
                    Negocio.General.SaveOrUpdate<ProveedorGanancia>(ProveedorGanancia, ProveedorGanancia);
                }
                new Notificacion("Ganancia Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarGanancias(ProveedorId));
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

        private void cboMarcaParte_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(this.cboMarcaParte.SelectedValue.ToString(), out id))
            {
                this.CargarLineas(id);

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
                var listaMarcaParte = Negocio.General.GetListOf<MarcaParte>(m => m.Estatus.Equals(true));
                this.cboMarcaParte.DataSource = listaMarcaParte;
                this.cboMarcaParte.DisplayMember = "NombreMarcaParte";
                this.cboMarcaParte.ValueMember = "MarcaParteID";
                AutoCompleteStringCollection autMarcaParte = new AutoCompleteStringCollection();
                foreach (var mp in listaMarcaParte) autMarcaParte.Add(mp.NombreMarcaParte);
                this.cboMarcaParte.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboMarcaParte.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboMarcaParte.AutoCompleteCustomSource = autMarcaParte;
                this.cboMarcaParte.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarLineas(int marcaId)
        {
            try
            {
                var listaLineas = Negocio.General.GetListOf<LineaMarcaPartesView>(l => l.MarcaParteID.Equals(marcaId));
                cboLinea.DataSource = listaLineas;
                cboLinea.DisplayMember = "NombreLinea";
                cboLinea.ValueMember = "LineaID";
                AutoCompleteStringCollection autLinea = new AutoCompleteStringCollection();
                foreach (var listaLinea in listaLineas) autLinea.Add(listaLinea.NombreLinea);
                cboLinea.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboLinea.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboLinea.AutoCompleteCustomSource = autLinea;
                cboLinea.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
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
                int marcaId = Negocio.Helper.ConvertirEntero(cboMarcaParte.SelectedValue);
                int lineaId = Negocio.Helper.ConvertirEntero(cboLinea.SelectedValue);
                var item = Negocio.General.GetEntity<ProveedorGanancia>(p => p.ProveedorID.Equals(ProveedorId) && p.MarcaParteID.Equals(marcaId) && p.LineaID.Equals(lineaId));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe un Registro, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.MarcaParteID != ProveedorGanancia.MarcaParteID && item.LineaID != ProveedorGanancia.LineaID)
                {
                    Negocio.Helper.MensajeError("Ya existe un Registro, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.cntError.LimpiarErrores();
            //if (this.txtNombreMarca.Text == "")
            //    this.cntError.PonerError(this.txtNombreMarca, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            //if (this.clbLineas.CheckedItems.Count < 1)
            //    this.cntError.PonerError(this.clbLineas, "Es necesario seleccionar por lo menos una linea.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
