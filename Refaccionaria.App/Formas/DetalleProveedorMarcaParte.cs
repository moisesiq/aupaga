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
    public partial class DetalleProveedorMarcaParte : DetalleBase
    {
        public int ProveedorId;
        ProveedorMarcaParte ProveedorMarcaParte;
        ControlError cntError = new ControlError();
        bool EsNuevo = true;

        public static DetalleProveedorMarcaParte Instance
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

            internal static readonly DetalleProveedorMarcaParte instance = new DetalleProveedorMarcaParte();
        }

        public DetalleProveedorMarcaParte()
        {
            InitializeComponent();
        }

        public DetalleProveedorMarcaParte(int Id)
        {
            InitializeComponent();
            try
            {
                ProveedorMarcaParte = Negocio.General.GetEntityById<ProveedorMarcaParte>("ProveedorMarcaParte", "ProveedorMarcaParteID", Id);
                if (ProveedorMarcaParte == null)
                    throw new EntityNotFoundException(Id.ToString(), "ProveedorMarcaParte");
                EsNuevo = false;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ Eventos ]

        private void DetalleProveedorMarcaParte_Load(object sender, EventArgs e)
        {
            CargaInicial();
            if (EsNuevo)
            {
                this.Text = "Nuevo";
                Helper.ClearTextBoxes(this);
                this.cboMarcaParte.Focus();
            }
            else
            {
                if (ProveedorMarcaParte.ProveedorMarcaParteID > 0)
                {
                    this.Text = "Modificar";
                    var pmp = Negocio.General.GetEntity<ProveedorMarcaParte>(p => p.ProveedorMarcaParteID.Equals(ProveedorMarcaParte.ProveedorMarcaParteID));
                    if (pmp != null)
                    {
                        this.cboMarcaParte.SelectedValue = pmp.MarcaParteID;
                        this.txtDescuentoUno.Text = pmp.DescuentoUno.ToString();
                        this.txtDescuentoDos.Text = pmp.DescuentoDos.ToString();
                        this.txtDescuentoTres.Text = pmp.DescuentoTres.ToString();
                        this.txtDescuentoCuatro.Text = pmp.DescuentoCuatro.ToString();
                        this.txtDescuentoCinco.Text = pmp.DescuentoCinco.ToString();
                        if (pmp.ImpactaFactura.Equals(true))
                        {
                            this.chkImpactaFactura.Checked = true;
                            this.chkImpactaArticulo.Checked = false;
                        }

                        if (pmp.ImpactaArticulo.Equals(true))
                        {
                            this.chkImpactaArticulo.Checked = true;
                            this.chkImpactaFactura.Checked = false;
                        }
                    }
                }
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                var marcaParteId = Helper.ConvertirEntero(cboMarcaParte.SelectedValue);
                
                if (EsNuevo)
                {
                    var proveedorMarcaParte = new ProveedorMarcaParte()
                    {
                        ProveedorID = ProveedorId,
                        MarcaParteID = marcaParteId,
                        DescuentoUno = Helper.ConvertirDecimal(txtDescuentoUno.Text),
                        DescuentoDos = Helper.ConvertirDecimal(txtDescuentoDos.Text),
                        DescuentoTres = Helper.ConvertirDecimal(txtDescuentoTres.Text),
                        DescuentoCuatro = Helper.ConvertirDecimal(txtDescuentoCuatro.Text),
                        DescuentoCinco = Helper.ConvertirDecimal(txtDescuentoCinco.Text),
                        ImpactaFactura = this.chkImpactaFactura.Checked,
                        ImpactaArticulo = this.chkImpactaArticulo.Checked,
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true
                    };
                    General.SaveOrUpdate<ProveedorMarcaParte>(proveedorMarcaParte, proveedorMarcaParte);

                    var relaciones = General.GetListOf<LineaMarcaParte>(m => m.MarcaParteID == marcaParteId && m.Estatus);
                    foreach (var relacion in relaciones)
                    {
                        var pg = General.GetEntity<ProveedorGanancia>(p => p.ProveedorID == ProveedorId
                            && p.MarcaParteID == marcaParteId && p.LineaID == relacion.LineaID);

                        if (pg == null) //Inserta, solo si es nuevo
                        {
                            var proveedorGanancia = new ProveedorGanancia()
                            {
                                ProveedorID = ProveedorId,
                                MarcaParteID = marcaParteId,
                                LineaID = relacion.LineaID,
                                PorcentajeUno = Helper.ConvertirDecimal(this.txtGananciaUno.Text),
                                PorcentajeDos = Helper.ConvertirDecimal(this.txtGananciaDos.Text),
                                PorcentajeTres = Helper.ConvertirDecimal(this.txtGananciaTres.Text),
                                PorcentajeCuatro = Helper.ConvertirDecimal(this.txtGananciaCuatro.Text),
                                PorcentajeCinco = Helper.ConvertirDecimal(this.txtGananciaCinco.Text),
                            };
                            Guardar.Generico<ProveedorGanancia>(proveedorGanancia);
                        }
                    }                    
                }
                else
                {
                    //ProveedorMarcaParte.MarcaParteID = marcaParteId;
                    ProveedorMarcaParte.DescuentoUno = Helper.ConvertirDecimal(txtDescuentoUno.Text);
                    ProveedorMarcaParte.DescuentoDos = Helper.ConvertirDecimal(txtDescuentoDos.Text);
                    ProveedorMarcaParte.DescuentoTres = Helper.ConvertirDecimal(txtDescuentoTres.Text);
                    ProveedorMarcaParte.DescuentoCuatro = Helper.ConvertirDecimal(txtDescuentoCuatro.Text);
                    ProveedorMarcaParte.DescuentoCinco = Helper.ConvertirDecimal(txtDescuentoCinco.Text);
                    ProveedorMarcaParte.ImpactaFactura = this.chkImpactaFactura.Checked;
                    ProveedorMarcaParte.ImpactaArticulo = this.chkImpactaArticulo.Checked;
                    ProveedorMarcaParte.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    ProveedorMarcaParte.FechaModificacion = DateTime.Now;
                    ProveedorMarcaParte.Estatus = true;
                    General.SaveOrUpdate<ProveedorMarcaParte>(ProveedorMarcaParte, ProveedorMarcaParte);

                    var relaciones = General.GetListOf<LineaMarcaParte>(m => m.MarcaParteID == marcaParteId && m.Estatus);
                    foreach (var relacion in relaciones)
                    {
                        var pg = General.GetEntity<ProveedorGanancia>(p => p.ProveedorID == ProveedorMarcaParte.ProveedorID
                            && p.MarcaParteID == ProveedorMarcaParte.MarcaParteID && p.LineaID == relacion.LineaID);

                        if (pg == null) //Inserta, solo si es nuevo
                        {
                            var proveedorGanancia = new ProveedorGanancia()
                            {
                                ProveedorID = ProveedorMarcaParte.ProveedorID,
                                MarcaParteID = ProveedorMarcaParte.MarcaParteID,
                                LineaID = relacion.LineaID,
                                PorcentajeUno = Helper.ConvertirDecimal(this.txtGananciaUno.Text),
                                PorcentajeDos = Helper.ConvertirDecimal(this.txtGananciaDos.Text),
                                PorcentajeTres = Helper.ConvertirDecimal(this.txtGananciaTres.Text),
                                PorcentajeCuatro = Helper.ConvertirDecimal(this.txtGananciaCuatro.Text),
                                PorcentajeCinco = Helper.ConvertirDecimal(this.txtGananciaCinco.Text),
                            };
                            Guardar.Generico<ProveedorGanancia>(proveedorGanancia);
                        }
                    }                    
                }
                new Notificacion("Marca Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarMarcas(ProveedorId));
                catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarGanancias(ProveedorId));
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        private void DetalleProveedorMarcaParte_KeyDown(object sender, KeyEventArgs e)
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

        private void chkImpactaFactura_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkImpactaFactura.CheckState == CheckState.Checked)
                this.chkImpactaArticulo.Checked = false;
            else
                this.chkImpactaArticulo.Checked = true;
        }

        private void chkImpactaArticulo_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkImpactaArticulo.CheckState == CheckState.Checked)
                this.chkImpactaFactura.Checked = false;
            else
                this.chkImpactaFactura.Checked = true;
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

        private bool Validaciones()
        {
            try
            {
                int id = Negocio.Helper.ConvertirEntero(cboMarcaParte.SelectedValue);
                bool impactaFactura = this.chkImpactaFactura.Checked;
                bool impactaArticulo = this.chkImpactaArticulo.Checked;

                var item = Negocio.General.GetEntity<ProveedorMarcaParte>(p => p.ProveedorID.Equals(ProveedorId) && p.MarcaParteID.Equals(id)
                    && p.ImpactaArticulo.Equals(impactaArticulo) && p.ImpactaFactura.Equals(impactaFactura));

                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe una Marca con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.MarcaParteID != ProveedorMarcaParte.MarcaParteID)
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
            //if (this.txtNombreMarca.Text == "")
            //    this.cntError.PonerError(this.txtNombreMarca, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            //if (this.clbLineas.CheckedItems.Count < 1)
            //    this.cntError.PonerError(this.clbLineas, "Es necesario seleccionar por lo menos una linea.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
