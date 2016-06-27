using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Calendar;
using System.ComponentModel;

using AdvancedDataGridView;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class catalogosProveedores : UserControl
    {
        BindingSource fuenteDatos;
        public Proveedor Proveedor;
        ControlError ctlError = new ControlError();
        ControlError ctlInfo = new ControlError() { Icon = Properties.Resources.Ico_Info };
        public bool EsNuevo = true;
        public int Modo = 0;
        bool sel = true;
        bool unaVez = true;
        bool bCargandoDescuentosGanancias = false;

        /* bool paiting = false;
        decimal sumaImporte = 0;
        decimal sumaPagos = 0;
        decimal sumaSaldo = 0;
        */

        bool tabMarcasOneTime = false; //Solo cargar la tabmarcas una vez
        bool tabDatosProveedoresOneTime = false;//Solo cargar la tabDatosProveedores una vez
        bool tabOperacionesKardexOneTime = false;//Solo cargar la tabOperacionesKardex una vez
        bool tabProductosOneTime = false;//Solo cargar la tabProductos una vez
        bool tabControlOneTime = false;//Solo cargar la tabControl una vez
        List<CalendarItem> oEventosCalendario;

        // Para el Singleton
        private static catalogosProveedores _Instance;
        public static catalogosProveedores Instance
        {
            get
            {
                if (catalogosProveedores._Instance == null || catalogosProveedores._Instance.IsDisposed)
                    catalogosProveedores._Instance = new catalogosProveedores();
                return catalogosProveedores._Instance;
            }
        }
        //

        public catalogosProveedores()
        {
            InitializeComponent();
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Si es la pestaña de Descuentos/Ganancias, se ignora la configuración del Enter
            if (this.tabDatosProveedores.SelectedTab == this.tbpDescGan)
                return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region [ Eventos ]

        private void catalogosProveedores_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
            if (EsNuevo)
            {
                this.LimpiarFormulario();
            }
            else
            {
                if (Proveedor.ProveedorID > 0)
                {
                    try
                    {
                        this.txtNombreProveedor.Text = Proveedor.NombreProveedor;
                        this.txtRFC.Text = Proveedor.RFC;
                        this.txtBeneficiario.Text = Proveedor.Beneficiario;
                        this.txtDireccion.Text = Proveedor.Direccion;
                        this.txtCiudad.Text = Proveedor.Ciudad;
                        this.txtEstado.Text = Proveedor.Estado;
                        this.txtCP.Text = Proveedor.CP;
                        this.dtpFechaRegistro.Value = Proveedor.FechaRegistro;
                        this.txtTelUno.Text = Proveedor.TelUno;
                        this.txtTelDos.Text = Proveedor.TelDos;
                        this.txtTelTres.Text = Proveedor.TelTres;
                        this.txtPaginaWeb.Text = Proveedor.PaginaWeb;
                        this.npdDiasPlazo.Value = Proveedor.DiasPlazo.Equals(null) ? 0 : Negocio.Helper.ConvertirEntero(Proveedor.DiasPlazo);

                        if (Proveedor.PaqueteriaID > 0)
                            this.cboPaqueteria.SelectedValue = Proveedor.PaqueteriaID;

                        this.nudCalendarioPedido.Value = Proveedor.CalendarioPedido == null ? 0 : Negocio.Helper.ConvertirEntero(Proveedor.CalendarioPedido);

                        if (Proveedor.CalendarioPedidoEnDia != null)
                        {
                            this.chkLunes.Checked = false;
                            this.chkMartes.Checked = false;
                            this.chkMiercoles.Checked = false;
                            this.chkJueves.Checked = false;
                            this.chkViernes.Checked = false;
                            this.chkSabado.Checked = false;

                            var dias = Proveedor.CalendarioPedidoEnDia.Split(',');
                            foreach (var dia in dias)
                            {                                
                                switch (dia)
                                { 
                                    case "L":
                                        this.chkLunes.Checked = true;
                                        break;
                                    case "Ma":
                                        this.chkMartes.Checked = true;
                                        break;
                                    case "Mi":
                                        this.chkMiercoles.Checked = true;
                                        break;
                                    case "J":
                                        this.chkJueves.Checked = true;
                                        break;
                                    case "V":
                                        this.chkViernes.Checked = true;
                                        break;
                                    case "S":
                                        this.chkSabado.Checked = true;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            this.chkLunes.Checked = false;
                            this.chkMartes.Checked = false;
                            this.chkMiercoles.Checked = false;
                            this.chkJueves.Checked = false;
                            this.chkViernes.Checked = false;
                            this.chkSabado.Checked = false;
                        }

                        this.dtpHoraTope.Value = DateTime.Now.Date.Add(Proveedor.HoraTope.HasValue ? Proveedor.HoraTope.Value : new TimeSpan());
                        this.dtpHoraMaxima.Value = DateTime.Now.Date.Add(Proveedor.HoraMaxima.HasValue ? Proveedor.HoraMaxima.Value : new TimeSpan());
                        this.chkUsarDescuentosItemFactura.Checked = Proveedor.UsarDescuentosItemFactura == null ? false : Helper.ConvertirBool(Proveedor.UsarDescuentosItemFactura);
                        this.txtDescuentoItemUno.Text = Proveedor.DescuentoItemUno.ToString();
                        this.txtDescuentoItemDos.Text = Proveedor.DescuentoItemDos.ToString();
                        this.txtDescuentoItemTres.Text = Proveedor.DescuentoItemTres.ToString();
                        this.txtDescuentoItemCuatro.Text = Proveedor.DescuentoItemCuatro.ToString();
                        this.txtDescuentoItemCinco.Text = Proveedor.DescuentoItemCinco.ToString();
                        this.txtDescuentoFacturaUno.Text = Proveedor.DescuentoFacturaUno.ToString();
                        this.txtDescuentoFacturaDos.Text = Proveedor.DescuentoFacturaDos.ToString();
                        this.txtDescuentoFacturaTres.Text = Proveedor.DescuentoFacturaTres.ToString();
                        this.txtDescuentoFacturaCuatro.Text = Proveedor.DescuentoFacturaCuatro.ToString();
                        this.txtDescuentoFacturaCinco.Text = Proveedor.DescuentoFacturaCinco.ToString();

                        this.chkCobraSeguro.Checked = Proveedor.CobraSeguro == null ? false : Helper.ConvertirBool(Proveedor.CobraSeguro);
                        this.txtSeguro.Text = Proveedor.Seguro == null ? "0.0" : Proveedor.Seguro.ToString();
                        this.chkAceptaDevolucion.Checked = Proveedor.AceptaDevolucionSeguro == null ? false : Helper.ConvertirBool(Proveedor.AceptaDevolucionSeguro);

                        this.tabDatos_SelectedIndexChanged(sender, null);
                        this.btnAgregarMarca.Enabled = true;
                        this.btnAgregarProntoPago.Enabled = true;
                        this.btnAgregarGanancia.Enabled = true;

                        this.CargarMarcas(Proveedor.ProveedorID);
                        tabMarcasOneTime = true;
                        tabDatosProveedoresOneTime = true;
                        tabOperacionesKardexOneTime = true;
                        tabProductosOneTime = true;
                        tabControlOneTime = true;
                        //this.CargarProntoPago(Proveedor.ProveedorID);
                        //this.CargarGanancias(Proveedor.ProveedorID);
                        //tabDatosProveedoresOneTime = true;

                        //this.CargarMovimientosNoPagados(Proveedor.ProveedorID);
                        //this.CargarDevoluciones(Proveedor.ProveedorID);
                        //this.CargarPagosParcialesYdevoluciones(0, this.dgvDepositos);
                        
                        //this.CargarOperaciones(Proveedor.ProveedorID, dtpInicial.Value, dtpFinal.Value);
                        
                        //this.CargarProductosComprados(Proveedor.ProveedorID, dtpInicial.Value, dtpFinal.Value);

                        this.LimpiarGridsOperaciones();
                    }
                    catch (Exception ex)
                    {
                        Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        private void tabDatosProveedores_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tabDatos.SelectedIndex = 0;
            switch (tabDatosProveedores.SelectedTab.Name)
            {
                case "tabDoctosDepsDevs":
                    if (tabDatosProveedoresOneTime)
                    {
                        this.CargarMovimientosNoPagados(Proveedor.ProveedorID);
                        this.CargarNotasDeCredito(Proveedor.ProveedorID);
                        // this.CargarPagosParcialesYdevoluciones(0, this.dgvDepositos);
                        tabDatosProveedoresOneTime = false;
                    }
                    break;

                case "tabKardex":
                    if (tabOperacionesKardexOneTime)
                    {
                        this.CargarOperaciones(Proveedor.ProveedorID, dtpInicial.Value, dtpFinal.Value);
                        tabOperacionesKardexOneTime = false;
                    }
                    break;

                case "tabProductos":
                    if (tabProductosOneTime)
                    {
                        this.CargarProductosComprados(Proveedor.ProveedorID, dtpInicial.Value, dtpFinal.Value);
                        tabProductosOneTime = false;
                    }
                    break;
                case "tabCalendario":
                    if (this.dtpCppListaInicio.Tag == null)
                    {
                        // this.dtpCppListaInicio.Value = this.ObtenerFechaAdeudoMasViejo(this.Proveedor.ProveedorID);
                        this.dtpCppListaInicio.Value = DateTime.Now.DiaPrimero();
                        this.dtpCppListaInicio.Tag = true;
                        this.CargarListaVencimientos(this.Proveedor.ProveedorID);
                    }
                    break;
                case "tabControl":
                    if (tabControlOneTime)
                    {
                        this.CargarTableroControl(Proveedor.ProveedorID);
                        tabControlOneTime = false;
                    }
                    break;
                case "tbpGarantias":
                    if (this.dgvGarantias.Rows.Count <= 0)
                        this.LlenarGarantias(this.Proveedor.ProveedorID);
                    break;
                case "tbpDevoluciones":
                    if (this.dgvDevoluciones.Rows.Count <= 0)
                        this.CargarDevoluciones(Proveedor.ProveedorID);
                    break;
                case "tbpDescGan":
                    if (this.tgvDescGan.Nodes.Count <= 0)
                    {
                        this.CargarDescuentosGanancias(this.Proveedor.ProveedorID);
                    }
                    break;

                default:
                    break;
            }
            
            
        }

        private void tabDatos_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabDatos.SelectedTab.Name)
            {
                case "tabContactos":
                    proveedorContactos pc = proveedorContactos.Instance;
                    this.addControlInPanel(pc, panelProveedorContactos);
                    pc.txtBusqueda.Clear();
                    pc.ActualizarListado(Proveedor.ProveedorID);
                    break;

                case "tabObservaciones":
                    proveedorObservaciones po = proveedorObservaciones.Instance;
                    this.addControlInPanel(po, panelProveedorObservaciones);
                    po.txtBusqueda.Clear();
                    po.ActualizarListado(Proveedor.ProveedorID);
                    break;

                default:
                    break;
            }
        }
        
        private void dgvDatos_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                this.dgvDatos.Rows[0].Selected = false;
                this.dgvDatos.ClearSelection();
            }
            catch { }
        }

        #endregion

        #region [ Metodos ]

        public void CargaInicial()
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
                this.tabDatosProveedores.SelectedIndex = 0;
                this.gpoDescuentosItems.ForeColor = Color.White;
                this.gpoDescuentosFacturas.ForeColor = Color.White;
                this.LimpiarFormulario();
                var provEstatus = Negocio.General.GetListOf<ProveedorEstatus>(p => p.Estatus.Equals(true));
                this.cboFiltro.DataSource = provEstatus;
                this.cboFiltro.DisplayMember = "NombreProveedorEstatus";
                this.cboFiltro.ValueMember = "ProveedorEstatusID";

                var listaPaqueteria = Negocio.General.GetListOf<ProveedorPaqueteria>(p => p.Estatus.Equals(true));
                this.cboPaqueteria.DataSource = listaPaqueteria;
                this.cboPaqueteria.DisplayMember = "NombrePaqueteria";
                this.cboPaqueteria.ValueMember = "ProveedorPaqueteriaID";
                AutoCompleteStringCollection autPaqueteria = new AutoCompleteStringCollection();
                foreach (var paqueteria in listaPaqueteria) autPaqueteria.Add(paqueteria.NombrePaqueteria);
                this.cboPaqueteria.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboPaqueteria.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboPaqueteria.AutoCompleteCustomSource = autPaqueteria;
                this.cboPaqueteria.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                this.cboEstatusProveedor.DataSource = provEstatus;
                this.cboEstatusProveedor.DisplayMember = "NombreProveedorEstatus";
                this.cboEstatusProveedor.ValueMember = "ProveedorEstatusID";

                this.dgvMovimientosNoPagados.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvAgrupadosNoPagados.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDevoluciones.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvOperaciones.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDetalleOperaciones.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvAbonosKardex.DefaultCellStyle.ForeColor = Color.Black;
                // Para que al cambiar proveedor, se actualicen las pestañas de Garantías y Devolución
                this.dgvGarantias.Rows.Clear();
                this.dgvDevoluciones.Rows.Clear();

                this.tgvDescGan.Nodes.Clear();
                this.tgvDescGan.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                this.tgvDescGan.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

                /*
                var modoList = new List<string>();
                string[] modos = { "SOLO MES ACTUAL", "SOLO AÑO ACTUAL", "DESDE INICIO DE OPERACIONES" };
                modoList.AddRange(modos);
                this.cboModo.DataSource = modoList;
                */
                if (unaVez)
                {
                    this.ActualizarListado();
                    this.txtBusqueda.Clear();
                    this.txtBusqueda.Focus();
                    unaVez = false;
                    this.dgvDatos.Rows[0].Selected = false;
                    this.dtpInicial.Value = DateTime.Now.DiaPrimero();
                    this.dtpFinal.Value = DateTime.Now.DiaUltimo();
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void LimpiarFormulario()
        {
            this.txtNombreProveedor.Text = string.Empty;
            this.txtRFC.Text = string.Empty;
            this.txtBeneficiario.Text = string.Empty;
            this.txtDireccion.Text = string.Empty;
            this.txtCiudad.Text = string.Empty;
            this.txtEstado.Text = string.Empty;
            this.txtCP.Text = string.Empty;
            this.dtpFechaRegistro.Value = DateTime.Now;
            this.txtTelUno.Text = string.Empty;
            this.txtTelDos.Text = string.Empty;
            this.txtTelTres.Text = string.Empty;
            this.txtPaginaWeb.Text = string.Empty;
            this.npdDiasPlazo.Value = 0;
            this.nudCalendarioPedido.Value = 0;
            this.dtpHoraTope.Text = "12:00:00 p.m.";
            this.dtpHoraMaxima.Text = "12:00:00 p.m.";
            this.txtDescuentoItemUno.Text = "0.0";
            this.txtDescuentoItemDos.Text = "0.0";
            this.txtDescuentoItemTres.Text = "0.0";
            this.txtDescuentoItemCuatro.Text = "0.0";
            this.txtDescuentoItemCinco.Text = "0.0";
            this.txtDescuentoFacturaUno.Text = "0.0";
            this.txtDescuentoFacturaDos.Text = "0.0";
            this.txtDescuentoFacturaTres.Text = "0.0";
            this.txtDescuentoFacturaCuatro.Text = "0.0";
            this.txtDescuentoFacturaCinco.Text = "0.0";
            this.chkCobraSeguro.Checked = false;
            this.chkAceptaDevolucion.Checked = false;
            this.txtSeguro.Text = "0.0";
            this.chkLunes.Checked = false;
            this.chkMartes.Checked = false;
            this.chkMiercoles.Checked = false;
            this.chkJueves.Checked = false;
            this.chkViernes.Checked = false;
            this.chkSabado.Checked = false;
            this.btnAgregarMarca.Enabled = false;
            this.btnAgregarProntoPago.Enabled = false;
            this.btnAgregarGanancia.Enabled = false;
            this.dgvMarcas.DataSource = null;
            this.dgvProntoPago.DataSource = null;
            this.dgvGanancias.DataSource = null;
            this.dgvMovimientosNoPagados.DataSource = null;
        }

        public void ActualizarListado()
        {
            try
            {
                int id;
                if (int.TryParse(cboFiltro.SelectedValue.ToString(), out id))
                {
                    this.fuenteDatos = new BindingSource();
                    this.fuenteDatos.DataSource = Negocio.Helper.newTable<ProveedoresView>("Proveedores", Negocio.General.GetListOf<ProveedoresView>(p => p.ProveedorID > 0 && p.ProveedorEstatusID.Equals(id)));
                    this.dgvDatos.DataSource = this.fuenteDatos;
                    this.dgvDatos.AutoResizeColumns();
                    Helper.OcultarColumnas(this.dgvDatos, new string[] { "ProveedorID", "RFC", "Direccion", "Ciudad", "Estado", "CP", "TelUno", "TelDos", "TelTres", "PaginaWeb", "DiasPlazo", "ProveedorEstatusID", "FechaRegistro", "Busqueda", "EntityState", "EntityKey" });
                    Helper.ColumnasToHeaderText(this.dgvDatos);
                    this.dgvDatos.ClearSelection();
                    this.dgvDatos.Rows[0].Selected = false;
                    this.txtBusqueda.Focus();
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void CargarMarcas(int proveedorId)
        {
            try
            {
                if (proveedorId <= 0) return;
                this.dgvMarcas.DataSource = null;
                var marcas = Helper.newTable<ProveedorMarcaPartesView>("Marcas", General.GetListOf<ProveedorMarcaPartesView>(p => p.ProveedorID.Equals(proveedorId)).ToList());
                
                this.dgvMarcas.DataSource = marcas;
                Helper.OcultarColumnas(this.dgvMarcas, new string[] { "ProveedorMarcaParteID", "ProveedorID", "EntityKey", "EntityState" });
                Helper.ColumnasToHeaderText(this.dgvMarcas);
                this.dgvMarcas.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvMarcas.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void CargarProntoPago(int proveedorId)
        {
            try
            {
                this.dgvProntoPago.DataSource = General.GetListOf<ProveedorProntoPagosView>(p => p.ProveedorID.Equals(proveedorId));
                Negocio.Helper.OcultarColumnas(this.dgvProntoPago, new string[] { "ProveedorProntoPagoID", "ProveedorID" });
                Negocio.Helper.ColumnasToHeaderText(this.dgvProntoPago);
                this.dgvProntoPago.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvProntoPago.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void CargarGanancias(int proveedorId)
        {
            try
            {
                if (this.dgvGanancias.Columns.Count > 0)
                    this.dgvGanancias.Columns.Clear();
                if (this.dgvGanancias.Rows.Count > 0)
                    this.dgvGanancias.Rows.Clear();

                var listaGanancias = General.GetListOf<ProveedorGananciasView>(p => p.ProveedorID.Equals(proveedorId)).ToList();
                listaGanancias.Sort((x, y) => x.Marca.CompareTo(y.Marca));
                SortableBindingList<ProveedorGananciasView> det = new SortableBindingList<ProveedorGananciasView>(listaGanancias);
                this.dgvGanancias.DataSource = det;
                Helper.OcultarColumnas(this.dgvGanancias, new string[] { "ProveedorGananciaID", "ProveedorID", "LineaID", "LineaMarca", "MarcaParteID" });
                Helper.ColumnasToHeaderText(this.dgvGanancias);

                if (!this.dgvDatos.Columns.Contains("X"))
                {
                    DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                    checkColumn.Name = "X";
                    checkColumn.HeaderText = "";
                    checkColumn.Width = 50;
                    checkColumn.ReadOnly = false;
                    checkColumn.FillWeight = 10;
                    this.dgvGanancias.Columns.Add(checkColumn);
                    this.dgvGanancias.Columns["X"].DisplayIndex = 0;
                }

                foreach (DataGridViewColumn col in this.dgvGanancias.Columns)
                {
                    if (!col.Name.Equals("X"))
                        col.ReadOnly = true;
                }

                this.dgvGanancias.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvGanancias.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public string CalendarioPedidoEnDias()
        {
            var res = string.Empty;
            var sDias = new StringBuilder();
            if (chkLunes.Checked.Equals(true))
                sDias.Append("L,");
            if (chkMartes.Checked.Equals(true))
                sDias.Append("Ma,");
            if (chkMiercoles.Checked.Equals(true))
                sDias.Append("Mi,");
            if (chkJueves.Checked.Equals(true))
                sDias.Append("J,");
            if (chkViernes.Checked.Equals(true))
                sDias.Append("V,");
            if (chkSabado.Checked.Equals(true))
                sDias.Append("S,");
            if (sDias.Length > 0)
                res = sDias.ToString().Substring(0, sDias.Length - 1);
            return res;
        }

        public void LimpiarGridsOperaciones()
        {
            if (this.dgvDetalleOperaciones.Columns.Count > 0)
                this.dgvDetalleOperaciones.Columns.Clear();
            if (this.dgvDetalleOperaciones.Rows.Count > 0)
                this.dgvDetalleOperaciones.Rows.Clear();

            this.dgvDetalleOperaciones.DataSource = null;

            this.dgvAbonosKardex.Rows.Clear();

            if (this.dgvDetalleDocumentos.Columns.Count > 0)
                this.dgvDetalleDocumentos.Columns.Clear();
            if (this.dgvDetalleDocumentos.Rows.Count > 0)
                this.dgvDetalleDocumentos.Rows.Clear();

            this.dgvDetalleDocumentos.DataSource = null;
        }

        private void addControlInPanel(object controlHijo, Panel panel)
        {
            panel.Controls.Clear();
            if (panel.Controls.Count > 0)
                panel.Controls.RemoveAt(0);
            UserControl usc = controlHijo as UserControl;
            usc.Dock = DockStyle.Fill;
            panel.Controls.Add(usc);
            panel.Tag = usc;
            usc.Show();
        }

        private bool Validaciones()
        {
            try
            {
                var item = Negocio.General.GetEntity<Proveedor>(p => p.NombreProveedor.Equals(txtNombreProveedor.Text));
                if (EsNuevo.Equals(true) && item != null)
                {
                    Negocio.Helper.MensajeError("Ya existe un Proveedor con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
                else if ((EsNuevo.Equals(false) && item != null) && item.ProveedorID != Proveedor.ProveedorID)
                {
                    Negocio.Helper.MensajeError("Ya existe un Proveedor con ese nombre, intente con otro.", GlobalClass.NombreApp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.ctlError.LimpiarErrores();
            if (this.txtNombreProveedor.Text == "")
                this.ctlError.PonerError(this.txtNombreProveedor, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            if (this.chkCobraSeguro.CheckState == CheckState.Checked && this.txtSeguro.Text == "")
                this.ctlError.PonerError(this.txtSeguro, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        private void CargarTableroControl(int proveedorId)
        {
            CuadroProveedores oProvCont;
            if (this.tabControl.Controls.Count <= 0)
            {
                oProvCont = new CuadroProveedores() { Dock = DockStyle.Fill };
                this.tabControl.Controls.Add(oProvCont);
                oProvCont.ReacomodarSinPrincipal();
            } else {
                oProvCont = (this.tabControl.Controls[0] as CuadroProveedores);
            }

            oProvCont.ProveedorFijoID = proveedorId;
            oProvCont.CargarDatos(proveedorId);
        }
        
        #endregion

        #region [ Tab Datos ]

        private void cboFiltro_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ActualizarListado();
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBusqueda.Text.Length > 0)
                {
                    string Value = txtBusqueda.Text; //"M a";
                    string filter = string.Empty;
                    if (Value.Contains(" ")) //revisar si existe espacio en blanco
                    {
                        string[] Values = Value.Split(' '); //separar valores
                        filter += "(Busqueda like '%" + Values[0].Trim() + "%') AND ";
                        for (int i = 1; i < Values.Length; i++)
                        {
                            filter += "(Busqueda like '%" + Values[i].Trim() + "%') AND ";
                        }
                        filter = filter.Substring(0, filter.LastIndexOf("AND ") - 1);
                    }
                    else
                    {
                        filter = "Busqueda like '%" + Value + "%'";
                    }
                    fuenteDatos.Filter = filter;
                }
                else
                {
                    fuenteDatos.Filter = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtBusqueda_Enter(object sender, EventArgs e)
        {
            txtBusqueda.BeginInvoke(new MethodInvoker(txtBusqueda.SelectAll));
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
           
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtBusqueda.Clear();
            }
            if (e.KeyCode == Keys.Down)
            {
                this.dgvDatos.Focus();
                this.dgvDatos.CurrentCell = this.dgvDatos[1, 0];
                e.Handled = true;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            this.dgvDatos.ClearSelection();
            //this.txtBusqueda.Clear();
            this.txtNombreProveedor.Focus();
            this.tabDatosProveedores.SelectedIndex = 0;
            this.tabDatos.SelectedIndex = 0;
            Proveedor = new Proveedor();
            EsNuevo = true;
            this.catalogosProveedores_Load(sender, null);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            try
            {
                var proveedorId = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ProveedorID"].Value);
                this.Proveedor = General.GetEntity<Proveedor>(p => p.ProveedorID.Equals(proveedorId));
                this.tabDatosProveedores.SelectedIndex = 0;
                EsNuevo = false;
                this.ctlError.LimpiarErrores();
                this.catalogosProveedores_Load(sender, null);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnFactuas_Click(object sender, EventArgs e)
        {
            var oProcXml = new System.Threading.Thread((o) =>
            {
                var frmFacturas = new FacturacionElectronica.FacturasSat();
                frmFacturas.Rfc = Config.Valor("Facturacion.Rfc");;
                frmFacturas.ClaveCiec = Config.Valor("Facturacion.ClaveCiec"); ;
                frmFacturas.RutaGuardar = Config.Valor("Facturacion.RutaDescargaXmls");
                frmFacturas.ShowDialog();
            });
            oProcXml.SetApartmentState(System.Threading.ApartmentState.STA);
            oProcXml.Start();
        }

        private void tabProveedorMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMarcasOneTime)
            {
                switch (this.tabProveedorMarca.SelectedTab.Name)
                {
                    case "tabProntoPago":
                        this.CargarProntoPago(Proveedor.ProveedorID);
                        break;
                    case "tabGanancias":
                        this.CargarGanancias(Proveedor.ProveedorID);
                        break;
                }
                tabMarcasOneTime = false;
            }
        }
        
        /*private void cboModo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mesActual = DateTime.Now.Month;
            var anioActual = DateTime.Now.Year;

            switch (cboModo.SelectedIndex)
            {
                case 0:
                    Modo = 0;
                    var ultimoDiaDelMes = DateTime.DaysInMonth(anioActual, mesActual);
                    dtpInicial.Value = new DateTime(anioActual, mesActual, 1);
                    dtpFinal.Value = new DateTime(anioActual, mesActual, ultimoDiaDelMes);
                    break;
                case 1:
                    Modo = 1;
                    var ultimoDiaDelAnio = DateTime.DaysInMonth(anioActual, 12);
                    dtpInicial.Value = new DateTime(anioActual, 1, 1);
                    dtpFinal.Value = new DateTime(anioActual, 12, ultimoDiaDelAnio);
                    break;
                case 2:
                    Modo = 2;
                    dtpInicial.Value = DateTime.Now;
                    dtpFinal.Value = DateTime.Now;
                    break;
            }
        }*/

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                SplashScreen.Show(new Splash());
                this.btnGuardar.Enabled = false;
                if (EsNuevo)
                {
                    var proveedor = new Proveedor()
                    {
                        NombreProveedor = txtNombreProveedor.Text,
                        Beneficiario = txtBeneficiario.Text,
                        RFC = txtRFC.Text,
                        Direccion = txtDireccion.Text,
                        Ciudad = txtCiudad.Text,
                        Estado = txtEstado.Text,
                        CP = txtCP.Text,
                        TelUno = txtTelUno.Text,
                        TelDos = txtTelDos.Text,
                        TelTres = txtTelTres.Text,
                        PaqueteriaID = Negocio.Helper.ConvertirEntero(cboPaqueteria.SelectedValue),
                        CalendarioPedido = Negocio.Helper.ConvertirCadena(this.nudCalendarioPedido.Value),
                        CalendarioPedidoEnDia = this.CalendarioPedidoEnDias(),
                        HoraTope = dtpHoraTope.Value.TimeOfDay,
                        HoraMaxima = dtpHoraMaxima.Value.TimeOfDay,
                        DiasPlazo = Negocio.Helper.ConvertirEntero(npdDiasPlazo.Value),
                        PaginaWeb = txtPaginaWeb.Text,
                        ProveedorEstatusID = Negocio.Helper.ConvertirEntero(cboEstatusProveedor.SelectedValue),
                        UsarDescuentosItemFactura = chkUsarDescuentosItemFactura.Checked,
                        DescuentoItemUno = Negocio.Helper.ConvertirDecimal(txtDescuentoItemUno.Text),
                        DescuentoItemDos = Negocio.Helper.ConvertirDecimal(txtDescuentoItemDos.Text),
                        DescuentoItemTres = Negocio.Helper.ConvertirDecimal(txtDescuentoItemTres.Text),
                        DescuentoItemCuatro = Negocio.Helper.ConvertirDecimal(txtDescuentoItemCuatro.Text),
                        DescuentoItemCinco = Negocio.Helper.ConvertirDecimal(txtDescuentoItemCinco.Text),
                        DescuentoFacturaUno = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaUno.Text),
                        DescuentoFacturaDos = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaDos.Text),
                        DescuentoFacturaTres = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaTres.Text),
                        DescuentoFacturaCuatro = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaCuatro.Text),
                        DescuentoFacturaCinco = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaCinco.Text),
                        CobraSeguro = chkCobraSeguro.Checked,
                        AceptaDevolucionSeguro = chkAceptaDevolucion.Checked,
                        Seguro = Negocio.Helper.ConvertirDecimal(txtSeguro.Text),
                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FechaRegistro = DateTime.Now,
                        Estatus = true,
                        Actualizar = true,
                        MontoPaqueteria = Helper.ConvertirDecimal(this.txtMontoPaqueteria.Text)
                    };
                    General.SaveOrUpdate<Proveedor>(proveedor, proveedor);

                    // Se agrega la cuenta auxiliar correspondiente
                    ContaProc.CrearCuentaAuxiliar(proveedor.NombreProveedor, Cat.ContaCuentasDeMayor.Proveedores, proveedor.ProveedorID);
                }
                else
                {
                    Proveedor.NombreProveedor = txtNombreProveedor.Text;
                    Proveedor.Beneficiario = txtBeneficiario.Text;
                    Proveedor.RFC = txtRFC.Text;
                    Proveedor.Direccion = txtDireccion.Text;
                    Proveedor.Ciudad = txtCiudad.Text;
                    Proveedor.Estado = txtEstado.Text;
                    Proveedor.CP = txtCP.Text;
                    Proveedor.TelUno = txtTelUno.Text;
                    Proveedor.TelDos = txtTelDos.Text;
                    Proveedor.TelTres = txtTelTres.Text;
                    Proveedor.PaqueteriaID = Negocio.Helper.ConvertirEntero(cboPaqueteria.SelectedValue);
                    Proveedor.CalendarioPedido = Negocio.Helper.ConvertirCadena(this.nudCalendarioPedido.Value);
                    Proveedor.CalendarioPedidoEnDia = this.CalendarioPedidoEnDias();
                    Proveedor.HoraTope = dtpHoraTope.Value.TimeOfDay;
                    Proveedor.HoraMaxima = dtpHoraMaxima.Value.TimeOfDay;
                    Proveedor.DiasPlazo = Negocio.Helper.ConvertirEntero(npdDiasPlazo.Value);
                    Proveedor.PaginaWeb = txtPaginaWeb.Text;
                    Proveedor.ProveedorEstatusID = Negocio.Helper.ConvertirEntero(cboEstatusProveedor.SelectedValue);
                    Proveedor.UsarDescuentosItemFactura = chkUsarDescuentosItemFactura.Checked;
                    Proveedor.DescuentoItemUno = Negocio.Helper.ConvertirDecimal(txtDescuentoItemUno.Text);
                    Proveedor.DescuentoItemDos = Negocio.Helper.ConvertirDecimal(txtDescuentoItemDos.Text);
                    Proveedor.DescuentoItemTres = Negocio.Helper.ConvertirDecimal(txtDescuentoItemTres.Text);
                    Proveedor.DescuentoItemCuatro = Negocio.Helper.ConvertirDecimal(txtDescuentoItemCuatro.Text);
                    Proveedor.DescuentoItemCinco = Negocio.Helper.ConvertirDecimal(txtDescuentoItemCinco.Text);
                    Proveedor.DescuentoFacturaUno = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaUno.Text);
                    Proveedor.DescuentoFacturaDos = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaDos.Text);
                    Proveedor.DescuentoFacturaTres = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaTres.Text);
                    Proveedor.DescuentoFacturaCuatro = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaCuatro.Text);
                    Proveedor.DescuentoFacturaCinco = Negocio.Helper.ConvertirDecimal(txtDescuentoFacturaCinco.Text);
                    Proveedor.CobraSeguro = chkCobraSeguro.Checked;
                    Proveedor.AceptaDevolucionSeguro = chkAceptaDevolucion.Checked;
                    Proveedor.Seguro = Negocio.Helper.ConvertirDecimal(txtSeguro.Text);
                    Proveedor.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Proveedor.FechaRegistro = DateTime.Now;
                    Proveedor.Estatus = true;
                    Proveedor.Actualizar = true;
                    Proveedor.MontoPaqueteria = Helper.ConvertirDecimal(this.txtMontoPaqueteria.Text);
                    Negocio.General.SaveOrUpdate<Proveedor>(Proveedor, Proveedor);
                }
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                new Notificacion("Proveedor Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                this.ActualizarListado();
            }
            catch (Exception ex)
            {
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            this.btnModificar_Click(sender, null);
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (this.dgvDatos.CurrentRow != null)
                    this.dgvDatos_CellClick(sender, null);
            }
            if (e.KeyCode == Keys.F5)
            {
                this.txtBusqueda.Clear();
                this.txtBusqueda.Focus();
            }
            if (e.KeyCode == Keys.End)
            {
                this.dgvDatos.Rows[dgvDatos.Rows.Count - 1].Selected = true;
                this.dgvDatos.Rows[dgvDatos.Rows.Count - 1].Cells[0].Selected = true;
            }
        }

        private void dgvDatos_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.btnModificar_Click(sender, null);
            }
        }

        private void tabDatosProveedores_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (EsNuevo)
            {
                if (e.TabPage == tabDoctosDepsDevs)
                    e.Cancel = true;
                if (e.TabPage == tabKardex)
                    e.Cancel = true;
                if (e.TabPage == tabProductos)
                    e.Cancel = true;
            }
        }

        private void tabDatos_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (EsNuevo)
            {
                if (e.TabPage == tabContactos)
                    e.Cancel = true;
                if (e.TabPage == tabObservaciones)
                    e.Cancel = true;
            }
        }

        private void btnAgregarMarca_Click(object sender, EventArgs e)
        {
            DetalleProveedorMarcaParte m = new DetalleProveedorMarcaParte();
            m.ProveedorId = Proveedor.ProveedorID;
            m.ShowDialog(); 
            if (m.DialogResult == DialogResult.OK)
            {
                this.CargarMarcas(Proveedor.ProveedorID);
                this.CargarGanancias(Proveedor.ProveedorID);
            }
        }

        private void dgvMarcas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvMarcas.CurrentRow == null)
                return;
            DetalleProveedorMarcaParte m = new DetalleProveedorMarcaParte(Negocio.Helper.ConvertirEntero(this.dgvMarcas.CurrentRow.Cells["ProveedorMarcaParteID"].Value));
            m.ProveedorId = Proveedor.ProveedorID;
            m.ShowDialog();
            if (m.DialogResult == DialogResult.OK)
            {
                this.CargarMarcas(Proveedor.ProveedorID);
                this.CargarGanancias(Proveedor.ProveedorID);
            }
        }

        private void dgvMarcas_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.dgvMarcas.CurrentRow == null) return;
                if (e.KeyCode == Keys.Delete)
                {
                    var res = Helper.MensajePregunta("¿Está seguro de que desea eliminar la relación con ésta Marca?", GlobalClass.NombreApp);
                    if (res == DialogResult.Yes)
                    {
                        var fila = this.dgvMarcas.CurrentRow.Index;
                        var proveedorMarcaParteID = Negocio.Helper.ConvertirEntero(this.dgvMarcas.Rows[fila].Cells["ProveedorMarcaParteID"].Value);
                        var proveedorMarcaParte = Negocio.General.GetEntity<ProveedorMarcaParte>(p => p.ProveedorMarcaParteID.Equals(proveedorMarcaParteID));
                        if (proveedorMarcaParte != null)
                        {
                            General.Delete<ProveedorMarcaParte>(proveedorMarcaParte);

                            var res2 = Helper.MensajePregunta("¿Desea eliminar la relación de ésta Marca con todas las Lineas registradas en el módulo de Ganancias?", GlobalClass.NombreApp);
                            if (res2 == DialogResult.Yes)
                            {
                                var ganancias = General.GetListOf<ProveedorGanancia>(p => p.ProveedorID == proveedorMarcaParte.ProveedorID
                                    && p.MarcaParteID == proveedorMarcaParte.MarcaParteID
                                    && p.Estatus);
                                foreach (var ganancia in ganancias)
                                {
                                    General.Delete<ProveedorGanancia>(ganancia);
                                }
                            }
                        }

                    }
                    else e.SuppressKeyPress = true;
                    //this.CargarGanancias(Proveedor.ProveedorID);
                    //this.CargarMarcas(Proveedor.ProveedorID);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        
        private void btnAgregarProntoPago_Click(object sender, EventArgs e)
        {
            DetalleProveedorProntoPago m = new DetalleProveedorProntoPago();
            m.ProveedorId = Proveedor.ProveedorID;
            m.ShowDialog();
        }

        private void dgvProntoPago_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvProntoPago.CurrentRow == null)
                return;
            DetalleProveedorProntoPago p = new DetalleProveedorProntoPago(Negocio.Helper.ConvertirEntero(this.dgvProntoPago.CurrentRow.Cells["ProveedorProntoPagoID"].Value));
            p.ProveedorId = Proveedor.ProveedorID;
            p.ShowDialog();
        }

        private void dgvProntoPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvProntoPago.CurrentRow == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                var res = Negocio.Helper.MensajePregunta("¿Está seguro de que desea eliminar la relación con el registro de pronto pago?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        var dg = (DataGridView)sender;
                        var fila = dg.SelectedRows[0].Index;
                        var proveedorProntoPagoID = Negocio.Helper.ConvertirEntero(dg.Rows[fila].Cells["ProveedorProntoPagoID"].Value);
                        var proveedorProntoPago = Negocio.General.GetEntity<ProveedorProntoPago>(p => p.ProveedorProntoPagoID.Equals(proveedorProntoPagoID));
                        if (proveedorProntoPago != null)
                        {
                            Negocio.General.Delete<ProveedorProntoPago>(proveedorProntoPago);
                            this.CargarProntoPago(Proveedor.ProveedorID);
                        }
                    }
                    catch (Exception ex)
                    {
                        Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        private void btnAgregarGanancia_Click(object sender, EventArgs e)
        {
            DetalleProveedorGanancia m = new DetalleProveedorGanancia();
            m.ProveedorId = Proveedor.ProveedorID;
            m.ShowDialog();
        }

        private void dgvGanancias_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            return;
            //if (e == null)
            //    return;
            //if (e.RowIndex == -1)
            //    return;
            //if (this.dgvGanancias.CurrentRow == null)
            //    return;
            //DetalleProveedorGanancia m = new DetalleProveedorGanancia(Negocio.Helper.ConvertirEntero(this.dgvGanancias.CurrentRow.Cells["ProveedorGananciaID"].Value));
            //m.ProveedorId = Proveedor.ProveedorID;
            //m.ShowDialog();
        }

        private void dgvGanancias_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvGanancias.CurrentRow == null) return;

            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                var fila = this.dgvGanancias.CurrentRow.Index;
                if (Convert.ToBoolean(this.dgvGanancias.Rows[fila].Cells["X"].Value).Equals(true))
                    this.dgvGanancias.Rows[fila].Cells["X"].Value = false;
                else
                    this.dgvGanancias.Rows[fila].Cells["X"].Value = true;
            }

            if (e.KeyCode == Keys.Delete)
            {
                var res = Negocio.Helper.MensajePregunta("¿Está seguro de que desea eliminar la relación con esta marca?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        var fila = this.dgvGanancias.CurrentRow.Index;
                        var proveedorGananciaID = Helper.ConvertirEntero(this.dgvGanancias.Rows[fila].Cells["ProveedorGananciaID"].Value);
                        var proveedorGanancia = General.GetEntity<ProveedorGanancia>(p => p.ProveedorGananciaID.Equals(proveedorGananciaID));
                        if (proveedorGanancia != null)
                        {
                            General.Delete<ProveedorMarcaParte>(proveedorGanancia);
                            this.CargarGanancias(Proveedor.ProveedorID);
                        }
                    }
                    catch (Exception ex)
                    {
                        Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        private void dgvGanancias_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (this.dgvGanancias.Columns[e.ColumnIndex].Name == "PCT1"
                    || this.dgvGanancias.Columns[e.ColumnIndex].Name == "PCT2"
                    || this.dgvGanancias.Columns[e.ColumnIndex].Name == "PCT3"
                    || this.dgvGanancias.Columns[e.ColumnIndex].Name == "PCT4"
                    || this.dgvGanancias.Columns[e.ColumnIndex].Name == "PCT5")
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var frmCantidad = new MensajeObtenerValor("Porcentaje", "0", MensajeObtenerValor.Tipo.Decimal);
                        if (frmCantidad.ShowDialog(Principal.Instance) == DialogResult.OK)
                        {
                            var valor = Helper.ConvertirDecimal(frmCantidad.Valor);
                            foreach (DataGridViewRow row in this.dgvGanancias.Rows)
                            {
                                var oCell = row.Cells["X"] as DataGridViewCheckBoxCell;
                                if (Helper.ConvertirBool(oCell.Value).Equals(true))
                                    row.Cells[e.ColumnIndex].Value = valor;
                            }
                        }
                        frmCantidad.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvGanancias_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgvGanancias.IsCurrentCellDirty)
            {
                this.dgvGanancias.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnGuardarGanancias_Click(object sender, EventArgs e)
        {
            try
            {
                var res = Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                foreach (DataGridViewRow row in this.dgvGanancias.Rows)
                {
                    if (Helper.ConvertirBool(row.Cells["X"].Value).Equals(true))
                    {
                        var proveedorGananciaId = Helper.ConvertirEntero(row.Cells["ProveedorGananciaID"].Value);
                        var ganancia = General.GetEntity<ProveedorGanancia>(p => p.ProveedorGananciaID == proveedorGananciaId);
                        if (ganancia != null)
                        {
                            ganancia.PorcentajeUno = Helper.ConvertirDecimal(row.Cells["PCT1"].Value);
                            ganancia.PorcentajeDos = Helper.ConvertirDecimal(row.Cells["PCT2"].Value);
                            ganancia.PorcentajeTres = Helper.ConvertirDecimal(row.Cells["PCT3"].Value);
                            ganancia.PorcentajeCuatro = Helper.ConvertirDecimal(row.Cells["PCT4"].Value);
                            ganancia.PorcentajeCinco = Helper.ConvertirDecimal(row.Cells["PCT5"].Value);
                            Guardar.Generico<ProveedorGanancia>(ganancia);
                        }
                    }
                }

                this.Cursor = Cursors.Default;
                new Notificacion("Información almacenada correctamente.", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnSeleccionarTodos_Click(object sender, EventArgs e)
        {
            if (this.dgvGanancias.DataSource == null) return;
            if (sel)
            {
                this.btnSeleccionarTodos.Text = "Sel Ninguno";
                sel = false;
            }
            else
            {
                this.btnSeleccionarTodos.Text = "Sel Todos";
                sel = true;
            }

            if (this.dgvGanancias.Columns.Contains("X"))
            {
                foreach (DataGridViewRow row in this.dgvGanancias.Rows)
                    if (sel)
                        row.Cells["X"].Value = false;
                    else
                        row.Cells["X"].Value = true;
            }
        }

        private void nudCalendarioPedido_ValueChanged(object sender, EventArgs e)
        {
            if (nudCalendarioPedido.Value > 0)
            {
                if (chkLunes.Checked.Equals(true))
                    chkLunes.Checked = false;
                if (chkMartes.Checked.Equals(true))
                    chkMartes.Checked = false;
                if (chkMiercoles.Checked.Equals(true))
                    chkMiercoles.Checked = false;
                if (chkJueves.Checked.Equals(true))
                    chkJueves.Checked = false;
                if (chkViernes.Checked.Equals(true))
                    chkViernes.Checked = false;
                if (chkSabado.Checked.Equals(true))
                    chkSabado.Checked = false;
            }
        }

        private void chkLunes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLunes.Checked.Equals(true))
                nudCalendarioPedido.Value = 0;
        }

        private void chkMartes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMartes.Checked.Equals(true))
                nudCalendarioPedido.Value = 0;
        }

        private void chkMiercoles_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMiercoles.Checked.Equals(true))
                nudCalendarioPedido.Value = 0;
        }

        private void chkJueves_CheckedChanged(object sender, EventArgs e)
        {
            if (chkJueves.Checked.Equals(true))
                nudCalendarioPedido.Value = 0;
        }

        private void chkViernes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkViernes.Checked.Equals(true))
                nudCalendarioPedido.Value = 0;
        }

        private void chkSabado_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSabado.Checked.Equals(true))
                nudCalendarioPedido.Value = 0;
        }

        private void chkCobraSeguro_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkCobraSeguro.Checked.Equals(true))
            {
                this.txtSeguro.Enabled = true;
                this.chkAceptaDevolucion.Enabled = true;
            }
            else
            {
                this.txtSeguro.Enabled = false;
                this.txtSeguro.Text = "0.0";
                this.chkAceptaDevolucion.Checked = false;
                this.chkAceptaDevolucion.Enabled = false;
            }
        }

        #endregion

        #region [ Tab Doctos-Deptos-Devs ]
                
        private void dgvMovimientosNoPagados_KeyDown(object sender, KeyEventArgs e)
        {
            this.dgvMovimientosNoPagados.VerEspacioCheck(e, "pen_Sel");
        }

        private void dgvMovimientosNoPagados_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvMovimientosNoPagados.VerSeleccionNueva())
            {
                this.dgvAgrupadosNoPagados.Rows.Clear();

                if (this.dgvMovimientosNoPagados.CurrentRow == null) return;
                int iMovID = Helper.ConvertirEntero(this.dgvMovimientosNoPagados.CurrentRow.Cells["pen_MovimientoInventarioID"].Value);
                this.CargarAbonos(this.dgvAbonos, iMovID);

                // Se verifica si es agrupador, para cargar los movimientos correspondientes
                if (Helper.ConvertirBool(this.dgvMovimientosNoPagados.CurrentRow.Cells["pen_EsAgrupador"].Value))
                    this.CargarMovimientosAgrupados(iMovID);
            }
        }

        private void btnAgruparNoPagados_Click(object sender, EventArgs e)
        {
            this.AgruparMovimientos();
        }

        private void btnDesagruparNoPagados_Click(object sender, EventArgs e)
        {
            if (this.dgvMovimientosNoPagados.CurrentRow == null)
                return;
            if (!Helper.ConvertirBool(this.dgvMovimientosNoPagados.CurrentRow.Cells["pen_EsAgrupador"].Value))
            {
                UtilLocal.MensajeAdvertencia("El movimiento seleccionado no es agrupador.");
                return;
            }
            if (UtilLocal.MensajePreguntaCancelar("¿Estás seguro que quieres desagrupar el movimiento seleccionado?") != DialogResult.Yes)
                return;
            int iMovID = Helper.ConvertirEntero(this.dgvMovimientosNoPagados.CurrentRow.Cells["pen_MovimientoInventarioID"].Value);
            this.DesagruparMovimientos(iMovID);
        }

        private void btnAgregarPoliza_Click(object sender, EventArgs e)
        {
            var ids = new List<int>();
            try
            {
                foreach (DataGridViewRow row in dgvMovimientosNoPagados.Rows)
                {
                    if (!Helper.ConvertirBool(row.Cells["pen_Sel"].Value))
                        continue;

                    int iMovID = Helper.ConvertirEntero(row.Cells["pen_MovimientoInventarioID"].Value);
                    if (Helper.ConvertirBool(row.Cells["pen_EsAgrupador"].Value))
                        ids.AddRange(General.GetListOf<MovimientoInventario>(c => c.MovimientoAgrupadorID == iMovID && c.Estatus).Select(c => c.MovimientoInventarioID));
                    else
                        ids.Add(iMovID);
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }

            if (ids.Count < 1) return;

            var poliza = new DetalleProveedorPoliza();
            poliza.FacturasSel = ids;
            poliza.ProveedorId = Proveedor.ProveedorID;
            if (poliza.ShowDialog() == DialogResult.OK)
                this.CargarMovimientosNoPagados(this.Proveedor.ProveedorID);
            poliza.Dispose();
        }

        private void btnAplicarDevolucion_Click(object sender, EventArgs e)
        {
            if (this.dgvMovimientosNoPagados.CurrentRow == null)
                return;

            /*
            var ids = new List<int>();
            try
            {
                foreach (DataGridViewRow row in dgvDevoluciones.Rows)
                    if (Convert.ToBoolean(row.Cells["X"].Value).Equals(true))
                        ids.Add(Negocio.Helper.ConvertirEntero(row.Cells["DevolucionID"].Value));

                if (ids.Count < 1) return;

                var movimientoDestinoID = Negocio.Helper.ConvertirEntero(this.dgvMovimientosNoPagados.CurrentRow.Cells["pen_MovimientoInventarioID"].Value);

                var idsArr = ids.ToArray();
                string idsJoined = string.Join(", ", idsArr);

                var pregunta = string.Format("{0}{1}{2}{3}", idsArr.Length > 1 ? "Está seguro de aplicar las devoluciones: " : "Está seguro de aplicar la devolución: ",
                    idsJoined, " a la Factura: ", this.dgvMovimientosNoPagados.CurrentRow.Cells["Factura"].Value.ToString());

                var res = Negocio.Helper.MensajePregunta(pregunta, GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                //Al aplicar una devolución a cualquier movimiento            
                //Modificar: AplicaEnMovimientoInventarioID = this.dgvMovimientosNoPagados.CurrentRow.Cells["MovimientoInventarioID"].Value
                //Modificar: FueLiquidado = True 
                //Modificar: FechaAplicacion = now
                foreach (var movimientoOrigenID in ids)
                {
                    var devolucion = Negocio.General.GetEntity<MovimientoInventario>(m => m.MovimientoInventarioID == movimientoOrigenID);
                    if (devolucion != null)
                    {
                        devolucion.FechaAplicacion = DateTime.Now;
                        devolucion.AplicaEnMovimientoInventarioID = movimientoDestinoID;
                        devolucion.FueLiquidado = true;
                        devolucion.FechaModificacion = DateTime.Now;
                        devolucion.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                        Negocio.General.SaveOrUpdate<MovimientoInventario>(devolucion, devolucion);
                    }
                }
                //DUDA
                //Si el importe total del movimientoInventarioID al que se le aplico la devolucion = a la suma de los pagos, entonces Modificar: FueLiquidado = True             
                var movimiento = Negocio.General.GetEntity<MovimientoInventario>(m => m.MovimientoInventarioID == movimientoDestinoID);
                decimal pagos = 0;
                var listaPagosParcialesYdevoluciones = Negocio.General.GetListOf<ProveedorPagosYdevolucionesView>(p => p.MovimientoInventarioID == movimientoDestinoID && p.ProveedorID == Proveedor.ProveedorID);
                if (listaPagosParcialesYdevoluciones != null && listaPagosParcialesYdevoluciones.Count > 0)
                {
                    foreach (var pago in listaPagosParcialesYdevoluciones)
                        pagos += pago.Importe;
                }

                if (movimiento != null)
                {
                    if (pagos >= movimiento.ImporteTotal)
                    {
                        movimiento.FueLiquidado = true;
                        movimiento.FechaModificacion = DateTime.Now;
                        movimiento.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                        Negocio.General.SaveOrUpdate<MovimientoInventario>(movimiento, movimiento);
                    }
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            new Notificacion("Devolución Aplicada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            this.CargarMovimientosNoPagados(Proveedor.ProveedorID);
            this.CargarDevoluciones(Proveedor.ProveedorID);
            // this.CargarPagosParcialesYdevoluciones(0, this.dgvDepositos);

            */
        }

        private void dgvNotasDeCredito_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvNotasDeCredito.VerSeleccionNueva() && this.dgvNotasDeCredito.CurrentRow != null)
            {
                this.txtNdcObservacion.Text = Helper.ConvertirCadena(this.dgvNotasDeCredito.CurrentRow.Cells["ndc_Observacion"].Value);
            }
        }

        public void CargarMovimientosNoPagados(int proveedorId)
        {
            var oPendientes = General.GetListOf<ProveedoresComprasView>(m => m.ProveedorID == Proveedor.ProveedorID && m.Saldo > 0 && !m.MovimientoAgrupadorID.HasValue);
            this.dgvMovimientosNoPagados.Rows.Clear();
            foreach (var oReg in oPendientes)
            {
                int iFila = this.dgvMovimientosNoPagados.Rows.Add(oReg.MovimientoInventarioID, false, oReg.Factura, oReg.Fecha
                    , oReg.ImporteFactura, oReg.Abonado, oReg.Saldo, (oReg.Fecha.Value.AddDays(oReg.DiasPlazo.Valor())), oReg.Usuario, oReg.EsAgrupador);
                if (oReg.EsAgrupador)
                    this.dgvMovimientosNoPagados.Rows[iFila].DefaultCellStyle.Font = new Font(this.dgvMovimientosNoPagados.Font, FontStyle.Bold);
            }
            // Se ordena el grid
            this.dgvMovimientosNoPagados.Sort(this.dgvMovimientosNoPagados.Columns["pen_Fecha"], ListSortDirection.Ascending);

            // Se llenan los totales
            this.dgvPendientesTotales.Rows.Clear();
            this.dgvPendientesTotales.Rows.Add("Totales", oPendientes.Sum(c => c.ImporteFactura), oPendientes.Sum(c => c.Abonado), oPendientes.Sum(c => c.Saldo));
        }

        public void CargarAbonos(DataGridView oGrid, int iMovimientoID)
        {
            var oAbonos = General.GetListOf<ProveedoresPolizasDetalleAvanzadoView>(c => c.MovimientoInventarioID == iMovimientoID);
            oGrid.Rows.Clear();
            foreach (var oReg in oAbonos)
            {
                oGrid.Rows.Add(oReg.ProveedorPolizaDetalleID, oReg.Fecha, (oReg.NotaDeCreditoID.HasValue ? oReg.NotaDeCredito : oReg.Folio)
                    , string.Format("{0} - {1}", oReg.Origen, (oReg.NotaDeCreditoID.HasValue ? oReg.NotaDeCreditoOrigen : oReg.FormaDePago)), oReg.Importe);
            }
        }

        private void CargarNotasDeCredito(int iProveedorID)
        {
            var oNotas = General.GetListOf<ProveedoresNotasDeCreditoView>(c => c.ProveedorID == iProveedorID && c.Disponible);
            this.dgvNotasDeCredito.Rows.Clear();
            foreach (var oReg in oNotas)
                this.dgvNotasDeCredito.Rows.Add(oReg.ProveedorNotaDeCreditoID, oReg.Folio, oReg.Origen, oReg.Fecha, oReg.Total, oReg.Usado, oReg.Restante
                        , oReg.Facturas, oReg.Observacion);
        }

        private void CargarMovimientosAgrupados(int iMovID)
        {
            var oMovs = General.GetListOf<ProveedoresComprasView>(c => c.MovimientoAgrupadorID == iMovID);
            this.dgvAgrupadosNoPagados.Rows.Clear();
            foreach (var oReg in oMovs)
                this.dgvAgrupadosNoPagados.Rows.Add(oReg.MovimientoInventarioID, oReg.Factura, oReg.Fecha, oReg.ImporteFactura, oReg.Abonado, oReg.Saldo, oReg.Usuario);
        }

        private void AgruparMovimientos()
        {
            // Se valida que haya un proveedor seleccionado
            if (this.Proveedor.ProveedorID <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ningún proveedor seleccionado. No se puede continuar.");
                return;
            }

            var oMovsSel = new List<int>();
            foreach (DataGridViewRow oFila in this.dgvMovimientosNoPagados.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["pen_Sel"].Value))
                    continue;

                // Se valida que no se seleccione un movimiento agrupador
                if (Helper.ConvertirBool(oFila.Cells["pen_EsAgrupador"].Value))
                {
                    UtilLocal.MensajeAdvertencia("No se puede agrupar un movimiento que ya es agrupador.");
                    return;
                }
                //
                if (Helper.ConvertirBool(oFila.Cells["pen_Sel"].Value))
                    oMovsSel.Add(Helper.ConvertirEntero(oFila.Cells["pen_MovimientoInventarioID"].Value));
            }
            if (oMovsSel.Count <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay movimientos seleccionados para agrupar.");
                return;
            }

            var frmAgrupar = new MovimientosAgrupar(oMovsSel);
            if (frmAgrupar.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                Cargando.Mostrar();
                // Se obtienen los movimientos a agrupar
                var oMovsAgr = new List<MovimientoInventario>();
                foreach (int iMovID in oMovsSel)
                    oMovsAgr.Add(General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovID && c.Estatus));
                // Se crea el movimiento agrupador
                var oMovAgr = new MovimientoInventario()
                {
                    TipoOperacionID = Cat.TiposDeOperacionMovimientos.EntradaCompra,
                    ProveedorID = this.Proveedor.ProveedorID,
                    FechaRecepcion = frmAgrupar.FechaGuardar,
                    Subtotal = oMovsAgr.Sum(c => c.Subtotal),
                    IVA = oMovsAgr.Sum(c => c.IVA),
                    ImporteTotal = oMovsAgr.Sum(c => c.ImporteTotal),
                    FueLiquidado = false,
                    ImporteFactura = oMovsAgr.Sum(c => c.ImporteFactura),
                    EsAgrupador = true
                };
                Guardar.Generico<MovimientoInventario>(oMovAgr);
                // Se hace la agrupación
                foreach (var oReg in oMovsAgr)
                {
                    oReg.MovimientoAgrupadorID = oMovAgr.MovimientoInventarioID;
                    Guardar.Generico<MovimientoInventario>(oReg);
                }
                Cargando.Cerrar();
                this.CargarMovimientosNoPagados(this.Proveedor.ProveedorID);
            }
            frmAgrupar.Dispose();
        }

        private void DesagruparMovimientos(int iMovimientoAgrupadorID)
        {
            Cargando.Mostrar();

            var oMovs = General.GetListOf<MovimientoInventario>(c => c.MovimientoAgrupadorID == iMovimientoAgrupadorID);
            foreach (var oReg in oMovs)
            {
                oReg.MovimientoAgrupadorID = null;
                Guardar.Generico<MovimientoInventario>(oReg);
            }
            var oMovAgr = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovimientoAgrupadorID && c.EsAgrupador && c.Estatus);
            Guardar.Eliminar<MovimientoInventario>(oMovAgr);

            Cargando.Cerrar();
            this.CargarMovimientosNoPagados(this.Proveedor.ProveedorID);
        }

        #endregion

        #region [Tap Operaciones]

        private void dtpInicial_ValueChanged(object sender, EventArgs e)
        {
            if (this.Proveedor != null && this.dtpInicial.Focused)
            {
                this.CargarOperaciones(this.Proveedor.ProveedorID, this.dtpInicial.Value, this.dtpFinal.Value);
                this.CargarProductosComprados(Proveedor.ProveedorID, dtpInicial.Value, dtpFinal.Value);
            }
        }

        private void dtpFinal_ValueChanged(object sender, EventArgs e)
        {
            if (this.Proveedor != null && this.dtpInicial.Focused)
            {
                this.CargarOperaciones(this.Proveedor.ProveedorID, this.dtpInicial.Value, this.dtpFinal.Value);
                this.CargarProductosComprados(Proveedor.ProveedorID, dtpInicial.Value, dtpFinal.Value);
            }
        }

        public void CargarOperaciones(int proveedorId, DateTime fechaIncial, DateTime fechaFinal)
        {
            if (proveedorId < 0)
                return;
            if (Modo != 2)
            {
                if (fechaIncial > fechaFinal)
                {
                    Negocio.Helper.MensajeError("La Fecha Inicial no puede ser mayor a la Fecha Final ó viceversa.", GlobalClass.NombreApp);
                    return;
                }
            }
            
            // Se llenan los datos
            fechaFinal = fechaFinal.Date.AddDays(1);
            var oCompras = General.GetListOf<ProveedoresComprasView>(c => c.ProveedorID == proveedorId && c.Fecha >= fechaIncial && c.Fecha < fechaFinal);
            this.dgvOperaciones.Rows.Clear();
            foreach (var oReg in oCompras)
                this.dgvOperaciones.Rows.Add(oReg.MovimientoInventarioID, oReg.Fecha, oReg.Factura, oReg.ImporteFactura, oReg.Usuario);
        }

        public void CargarDetalleOperacion(int movimientoInventarioId)
        {
            try
            {
                if (this.dgvDetalleOperaciones.Columns.Count > 0)
                    this.dgvDetalleOperaciones.Columns.Clear();
                if (this.dgvDetalleOperaciones.Rows.Count > 0)
                    this.dgvDetalleOperaciones.Rows.Clear();

                var opers = Negocio.General.GetListOf<ProveedorKardexDetalleOperacionesView>(o => o.MovimientoInventarioID.Equals(movimientoInventarioId));
                this.dgvDetalleOperaciones.DataSource = opers;
                Negocio.Helper.OcultarColumnas(this.dgvDetalleOperaciones, new string[] { "MovimientoInventarioDetalleID", "MovimientoInventarioID" });
                Negocio.Helper.ColumnasToHeaderText(this.dgvDetalleOperaciones);

                var colImporte = this.dgvDetalleOperaciones.Columns["Importe"];
                colImporte.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colImporte.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;

                this.dgvDetalleOperaciones.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDetalleOperaciones.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void CargarProductosComprados(int proveedorId, DateTime fechaIncial, DateTime fechaFinal)
        {
            if (proveedorId < 0)
                return;
            if (Modo != 2)
            {
                if (fechaIncial > fechaFinal)
                {
                    Negocio.Helper.MensajeError("La Fecha Inicial no puede ser mayor a la Fecha Final ó viceversa.", GlobalClass.NombreApp);
                    return;
                }
            }
            try
            {
                if (this.dgvProductosComprados.Columns.Count > 0)
                    this.dgvProductosComprados.Columns.Clear();
                if (this.dgvProductosComprados.Rows.Count > 0)
                    this.dgvProductosComprados.Rows.Clear();

                var prods = new List<ProveedorProductosCompradosView>();

                if (Modo == 2)
                {
                    prods = Negocio.General.GetListOf<ProveedorProductosCompradosView>(o => o.ProveedorID == Proveedor.ProveedorID);
                }
                else
                {
                    prods = Negocio.General.GetListOf<ProveedorProductosCompradosView>(o => o.ProveedorID == Proveedor.ProveedorID && (o.FechaRecepcion >= fechaIncial && o.FechaRecepcion <= fechaFinal));
                }

                List<ProductosComprados> result = prods.GroupBy(row => new { row.ParteID, row.NumeroParte, row.Linea, row.Marca, row.Descripcion })
                    .Select(g => new ProductosComprados()
                    {
                        ParteID = g.Key.ParteID,
                        NumeroParte = g.Key.NumeroParte,
                        Linea = g.Key.Linea,
                        Marca = g.Key.Marca,
                        Descripcion = g.Key.Descripcion,
                        UNS = g.Sum(x => x.Cantidad),
                        Importe = g.Sum(x => x.Importe)
                    }).ToList();

                this.dgvProductosComprados.DataSource = new SortableBindingList<ProductosComprados>(result);
                Negocio.Helper.OcultarColumnas(this.dgvProductosComprados, new string[] { "ParteID" });
                Negocio.Helper.ColumnasToHeaderText(this.dgvProductosComprados);

                var colImporte = this.dgvProductosComprados.Columns["Importe"];
                colImporte.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colImporte.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;

                decimal sumaImporte = 0;
                foreach (DataGridViewRow fila in this.dgvProductosComprados.Rows)
                {
                    sumaImporte += Negocio.Helper.ConvertirDecimal(fila.Cells["Importe"].Value);
                }

                //labelDgvTotales.Text = "Totales";
                //labelDgvTotales.Name = "lblTotales";
                //labelDgvTotales.Height = 21;
                //labelDgvTotales.AutoSize = false;
                //labelDgvTotales.BorderStyle = BorderStyle.FixedSingle;
                //labelDgvTotales.TextAlign = ContentAlignment.MiddleLeft;
                //int Xdgv1 = this.dgvOperaciones.GetCellDisplayRectangle(3, -1, true).Location.X;
                //labelDgvTotales.Width = this.dgvOperaciones.Columns[3].Width + Xdgv1;
                //labelDgvTotales.Location = new Point(0, this.dgvOperaciones.Height - 21);
                //this.dgvOperaciones.Controls.Add(labelDgvTotales);

                //labelDgvSumaImporte.Text = Negocio.Helper.DecimalToCadenaMoneda(sumaImporte);
                //labelDgvSumaImporte.Height = 21;
                //labelDgvSumaImporte.AutoSize = false;
                //labelDgvSumaImporte.BorderStyle = BorderStyle.FixedSingle;
                //labelDgvSumaImporte.TextAlign = ContentAlignment.MiddleRight;
                //labelDgvSumaImporte.Width = this.dgvOperaciones.Columns["Importe"].Width;
                //labelDgvSumaImporte.Location = new Point(labelDgvTotales.Width, this.dgvOperaciones.Height - 21);
                //this.dgvOperaciones.Controls.Add(labelDgvSumaImporte);

                this.dgvProductosComprados.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvProductosComprados.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void CargarDetalleDocumentos(int parteId)
        {
            try
            {
                if (this.dgvDetalleDocumentos.Columns.Count > 0)
                    this.dgvDetalleDocumentos.Columns.Clear();
                if (this.dgvDetalleDocumentos.Rows.Count > 0)
                    this.dgvDetalleDocumentos.Rows.Clear();

                var docs = Negocio.General.GetListOf<ProveedorDetalleDocumentosView>(o => o.ProveedorID == Proveedor.ProveedorID && o.ParteID == parteId);
                this.dgvDetalleDocumentos.DataSource = docs;
                Negocio.Helper.OcultarColumnas(this.dgvDetalleDocumentos, new string[] { "MovimientoInventarioDetalleID", "MovimientoInventarioID", "ProveedorID", "ParteID" });
                Negocio.Helper.ColumnasToHeaderText(this.dgvDetalleDocumentos);

                this.dgvDetalleDocumentos.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDetalleDocumentos.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void CargarDetalleDocumentos(int parteId, DateTime fechaInicial, DateTime fechaFinal)
        {
            try
            {
                if (this.dgvDetalleDocumentos.Columns.Count > 0)
                    this.dgvDetalleDocumentos.Columns.Clear();
                if (this.dgvDetalleDocumentos.Rows.Count > 0)
                    this.dgvDetalleDocumentos.Rows.Clear();

                var docs = Negocio.General.GetListOf<ProveedorDetalleDocumentosView>(o => o.ProveedorID == Proveedor.ProveedorID && o.ParteID == parteId && o.Fecha>=fechaInicial && o.Fecha<=fechaFinal);
                this.dgvDetalleDocumentos.DataSource = docs;
                Negocio.Helper.OcultarColumnas(this.dgvDetalleDocumentos, new string[] { "MovimientoInventarioDetalleID", "MovimientoInventarioID", "ProveedorID", "ParteID" });
                Negocio.Helper.ColumnasToHeaderText(this.dgvDetalleDocumentos);

                this.dgvDetalleDocumentos.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDetalleDocumentos.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvOperaciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvOperaciones.CurrentRow == null) return;
            try
            {
                if (e.KeyCode == Keys.F12)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();

                    saveDialog.Filter = "Excel files (*.csv)|*.csv ";
                    saveDialog.FileName = "ConcentradoDeOperaciones";
                    saveDialog.Title = GlobalClass.NombreApp;
                    saveDialog.InitialDirectory = "c:\\";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        Negocio.Helper.OnExportGridToCSV(this.dgvOperaciones, saveDialog.FileName.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvOperaciones_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvOperaciones.VerSeleccionNueva())
            {
                if (this.dgvOperaciones.CurrentRow == null)
                {
                    this.dgvDetalleOperaciones.DataSource = null;
                    this.dgvAbonosKardex.Rows.Clear();
                }
                else
                {
                    int iMovID = Helper.ConvertirEntero(this.dgvOperaciones.CurrentRow.Cells["com_MovimientoInventarioID"].Value);
                    this.CargarDetalleOperacion(iMovID);
                    this.CargarAbonos(this.dgvAbonosKardex, iMovID);
                }
            }
        }

        private void dgvAbonosKardex_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvOperaciones.VerSeleccionNueva())
            {
                if (this.dgvOperaciones.CurrentRow == null)
                {
                    this.dgvDetalleOperaciones.Rows.Clear();
                    this.dgvAbonosKardex.Rows.Clear();
                }
                else
                {
                    int iMovID = Helper.ConvertirEntero(this.dgvOperaciones.CurrentRow.Cells["MovimientoInventarioID"].Value);
                    this.CargarDetalleOperacion(iMovID);
                    this.CargarAbonos(this.dgvAbonosKardex, iMovID);
                }
            }
        }

        private void dgvAbonosKardex_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.dgvAbonosKardex.CurrentRow != null && e.Button == MouseButtons.Right)
            {
                this.cmsAbonos.Show(this.dgvAbonosKardex, new Point(e.X, e.Y));
            }
        }

        private void smiCambiarFolio_Click(object sender, EventArgs e)
        {
            if (this.dgvAbonosKardex.CurrentRow == null) return;
            int iAbonoID = Helper.ConvertirEntero(this.dgvAbonosKardex.CurrentRow.Cells["abk_ProveedorPolizaDetalleID"].Value);
            this.CambiarFolioAbono(iAbonoID);
        }

        private void dgvProductosComprados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvProductosComprados.CurrentRow == null)
                return;
            var parteId = Negocio.Helper.ConvertirEntero(this.dgvProductosComprados.CurrentRow.Cells["ParteID"].Value);
            if (parteId > 0)
            {
                this.CargarDetalleDocumentos(parteId, this.dtpInicial.Value, this.dtpFinal.Value);
            }
        }
        
        private void dgvProductosComprados_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                var ev = new DataGridViewCellEventArgs(this.dgvProductosComprados.CurrentCell.ColumnIndex, this.dgvProductosComprados.CurrentRow.Index);
                this.dgvProductosComprados_CellClick(sender, ev);
            }
        }

        private void dgvProductosComprados_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.dgvProductosComprados.CurrentRow == null) return;
                if (e.KeyCode == Keys.F12)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();

                    saveDialog.Filter = "Excel files (*.csv)|*.csv ";
                    saveDialog.FileName = "ProductosComprados";
                    saveDialog.Title = GlobalClass.NombreApp;
                    saveDialog.InitialDirectory = "c:\\";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        Negocio.Helper.OnExportGridToCSV(this.dgvProductosComprados, saveDialog.FileName.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CambiarFolioAbono(int iAbonoID)
        {
            var oNuevo = UtilLocal.ObtenerValor("Folio nuevo:", "", MensajeObtenerValor.Tipo.Texto);
            if (oNuevo == null) return;
            
            string sFolio = Helper.ConvertirCadena(oNuevo);
            var oAbono = General.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iAbonoID && c.Estatus);
            oAbono.Folio = sFolio;
            Guardar.Generico<ProveedorPolizaDetalle>(oAbono);

            // Se modifica la póliza contable correspondiente
            var oPoliza = General.GetEntity<ContaPoliza>(c => c.RelacionTabla == Cat.Tablas.ProveedorPolizaDetalle && c.RelacionID == oAbono.ProveedorPolizaDetalleID);
            var oPolizaDet = General.GetListOf<ContaPolizaDetalle>(c => c.ContaPolizaID == oPoliza.ContaPolizaID);
            foreach (var oReg in oPolizaDet)
            {
                oReg.Referencia = sFolio;
                Guardar.Generico<ContaPolizaDetalle>(oReg);
            }

            // Se verifica si es de una nota de crédito, para modificar el folio
            if (oAbono.OrigenID == Cat.OrigenesPagosAProveedores.NotaDeCredito)
            {
                var oNotaP = General.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == oAbono.NotaDeCreditoID);
                oNotaP.Folio = sFolio;
                Guardar.Generico<ProveedorNotaDeCredito>(oNotaP);
            }
        }

        #endregion

        #region [ Cuentas por pagar ]
        
        // Cuando se active el control de la tab, cargar el calendario
        private void tabCalendario_Enter(object sender, EventArgs e)
        {
            // CargarCalendario(DateTime.Today);
            this.CargarEventosCalendario();
        }

        private void chkCppSoloProveedorSel_CheckedChanged(object sender, EventArgs e)
        {
            this.CargarEventosCalendario();
        }

        private void mvCalendarioProveedor_SelectionChanged(object sender, EventArgs e)
        {
            // CargarItemsACalendario(mvCalendarioProveedor.SelectionStart, mvCalendarioProveedor.SelectionEnd);
            this.CalendarioEstablecerRangoMostrado(this.mvCalendarioProveedor.SelectionStart, this.mvCalendarioProveedor.SelectionEnd);
        }

        /* private void CargarCalendario(DateTime dt)
        {

            this.ciProv = new List<CalendarItem>();
            CalendarItem citemp;
            var eventos = General.GetListOf<ProveedorEventoCalendario>();
            foreach (var evento in eventos)
            {
                var proveedor = General.GetEntity<Proveedor>(c => c.ProveedorID == evento.ProveedorID && c.Estatus);
                DateTime dtTemp = (DateTime)evento.Fecha;
                citemp = new CalendarItem(this.calProveedor, dtTemp, dtTemp.AddMinutes(30)
                                    , string.Format("{0}\n{1}\n{2}", Proveedor.NombreProveedor, evento.Fecha, evento.Evento));
                this.ciProv.Add(citemp);
            }
        }

        private void CargarItemsACalendario(DateTime dt, DateTime dt2)
        {
            calProveedor.Items.Clear();
            calProveedor.SetViewRange(dt, dt2);
            foreach (var calendarItem in this.ciProv)
            {
                if (calProveedor.ViewIntersects(calendarItem))
                {
                    calProveedor.Items.Add(calendarItem);

                }
            }
        }
        */

        private void calProveedor_DoubleClick(object sender, EventArgs e)
        {
            var frmAgregar = new AgregarEventoCalendario(Proveedor, calProveedor.SelectedElementStart.Date);
            frmAgregar.ShowDialog(Principal.Instance);

            if (frmAgregar.DialogResult == DialogResult.OK)
            {
                this.CargarEventosCalendario();
            }
        }

        private void calProveedor_ItemSelected(object sender, CalendarItemEventArgs e)
        {
            Helper.MensajeInformacion(e.Item.Text, "Informacion del evento");
        }

        private void calProveedor_ItemCreating(object sender, CalendarItemCancelEventArgs e)
        {
            /* e.Cancel = true;
            if (this.mvCalendarioProveedor.SelectionStart > new DateTime(2000, 01, 01))
            {
                CargarItemsACalendario(this.mvCalendarioProveedor.SelectionStart, this.mvCalendarioProveedor.SelectionEnd);
            }
            else
            {
                CargarItemsACalendario(DateTime.Today, DateTime.Today.AddDays(6));
            }
            */
        }

        private void CargarEventosCalendario()
        {
            Cargando.Mostrar();

            // Se obtienen los datos
            List<ProveedoresComprasView> oEventos;
            if (this.chkCppSoloProveedorSel.Checked)
            {
                if (this.Proveedor == null || this.Proveedor.ProveedorID <= 0)
                {
                    UtilLocal.MensajeAdvertencia("No hay ningún Proveedor seleccionado.");
                    Cargando.Cerrar();
                    return;
                }
                oEventos = General.GetListOf<ProveedoresComprasView>(c => c.Saldo > 0 && c.ProveedorID == this.Proveedor.ProveedorID 
                    && !c.MovimientoAgrupadorID.HasValue);
            }
            else
            {
                oEventos = General.GetListOf<ProveedoresComprasView>(c => c.Saldo > 0 && !c.MovimientoAgrupadorID.HasValue);
            }

            this.oEventosCalendario = new List<CalendarItem>();
            foreach (var oReg in oEventos)
            {
                // Se obtiene el día de pago
                DateTime dPago = AdmonProc.SugerirVencimientoCompra(oReg.Fecha.Valor(), oReg.DiasPlazo.Valor());
                
                var citemp = new CalendarItem(this.calProveedor, dPago, dPago.AddMinutes(30)
                    , string.Format("{0} - {1}\n{2}\n{3}", oReg.Proveedor, oReg.Saldo.Valor().ToString(GlobalClass.FormatoMoneda), dPago, "FECHA DE PAGO"));
                citemp.Tag = oReg.Saldo;
                this.oEventosCalendario.Add(citemp);
            }
            this.CalendarioEstablecerRangoMostrado(this.mvCalendarioProveedor.SelectionStart, this.mvCalendarioProveedor.SelectionEnd);

            Cargando.Cerrar();
        }

        private void CalendarioEstablecerRangoMostrado(DateTime dInicio, DateTime dFin)
        {
            if (dInicio == DateTime.MinValue || dFin == DateTime.MinValue)
            {
                dInicio = DateTime.Now.DiaPrimero();
                dFin = DateTime.Now.DiaUltimo();
            }
            
            dInicio = dInicio.Date.AddHours(9);
            dFin = dFin.Date.AddHours(20);
            this.calProveedor.SetViewRange(dInicio, dFin);
            
            // Se vuelven a agregar los Items porque el .SetViewRange los quita
            // Se agregan los eventos
            foreach (var oReg in this.oEventosCalendario)
            {
                if (this.calProveedor.ViewIntersects(oReg))
                    this.calProveedor.Items.Add(oReg);
            }
            // Se agregan los totales
            var oTotales = this.oEventosCalendario.GroupBy(c => c.StartDate.Date).Select(c => new { Fecha = c.Key, Adeudo = c.Sum(s => Helper.ConvertirDecimal(s.Tag)) });
            foreach (var oReg in oTotales)
            {
                DateTime dFecha = oReg.Fecha.AddHours(8.5);
                if (this.calProveedor.ViewIntersects(dFecha, dFecha.AddHours(0.5)))
                {
                    this.calProveedor.Items.Add(new CalendarItem(this.calProveedor, dFecha, dFecha.AddHours(0.5), "TOTAL: " + oReg.Adeudo.ToString(GlobalClass.FormatoMoneda)));
                }
            }

            // Para que se muestre el calendario recorrido hasta las 20 hrs.
            if (this.calProveedor.GetTimeUnit(this.calProveedor.ViewStart.Date.AddHours(18)) != null)
                this.calProveedor.EnsureVisible(this.calProveedor.GetTimeUnit(this.calProveedor.ViewStart.Date.AddHours(20.5)));
        }

        private void tabCuentasPorPagar_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void dtpCppListaInicio_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpCppListaInicio.Focused)
                this.CargarListaVencimientos(this.Proveedor.ProveedorID);
        }

        private DateTime ObtenerFechaAdeudoMasViejo(int iProveedorID)
        {
            var oCompras = General.GetListOf<ProveedoresComprasView>(c => c.Saldo > 0 && !c.MovimientoAgrupadorID.HasValue);
            var oMasViejo = oCompras.OrderBy(c => c.Fecha.Value.AddDays(c.DiasPlazo.Valor())).FirstOrDefault();
            return (oMasViejo == null ? DateTime.Now : oMasViejo.Fecha.Valor().AddDays(oMasViejo.DiasPlazo.Valor()));
        }

        private void CargarListaVencimientos(int iProveedorID)
        {
            Cargando.Mostrar();

            DateTime dInicio = this.dtpCppListaInicio.Value.Date;
            DateTime dFinMas1 = dInicio.AddMonths(1).DiaUltimo().AddDays(1);
            DateTime dFinalMax = DateTime.Now.DiaUltimo().AddMonths(1);
            // La fecha final es máximo 2 meses del día actual
            if (dFinMas1 < dFinalMax)
                dFinMas1 = dFinalMax;
            //
            var oCompras = General.GetListOf<ProveedoresComprasView>(c => c.Saldo > 0 && !c.MovimientoAgrupadorID.HasValue && c.Fecha < dFinMas1);
            var oComprasA = oCompras
                .Where(c => c.Fecha.Value.AddDays(c.DiasPlazo.Valor()) >= dInicio && c.Fecha.Value.AddDays(c.DiasPlazo.Valor()) < dFinMas1)
                .GroupBy(c => new { Dia = c.Fecha.Value.Date.AddDays(c.DiasPlazo.Valor()), c.Proveedor })
                .Select(c => new { c.Key.Dia, c.Key.Proveedor, Saldo = c.Sum(s => s.Saldo) })
                .OrderBy(c => c.Dia).ThenBy(c => c.Proveedor).ToList();
            decimal mSaldoIni = oCompras.Where(c => c.Fecha.Value.AddDays(c.DiasPlazo.Valor()) < dInicio).Sum(c => c.Saldo).Valor();
            this.ctlInfo.QuitarError(this.dtpCppListaInicio);
            if (mSaldoIni > 0)
                this.ctlInfo.PonerError(this.dtpCppListaInicio, "Existen adeudos con fechas anteriores a la seleccionada.");
            decimal mSaldoAcum = mSaldoIni;

            // Se empiezan a llenar los datos
            DateTime dFechaReg, dFecha = DateTime.MinValue;
            TreeGridNode oNodoDia = null;
            this.tgvCuentasPorPagar.Nodes.Clear();
            foreach (var oReg in oComprasA)
            {
                dFechaReg = AdmonProc.SugerirVencimientoCompra(oReg.Dia, 0);
                if (dFechaReg != dFecha)
                {
                    dFecha = dFechaReg;
                    oNodoDia = this.tgvCuentasPorPagar.Nodes.Add(dFecha.ToString("dd-MMM").ToUpper(), 0, 0);
                }
                mSaldoAcum += oReg.Saldo.Valor();
                oNodoDia.Nodes.Add(oReg.Proveedor, oReg.Saldo, mSaldoAcum);
                // Se llena el total
                oNodoDia.Cells["cpp_Importe"].Value = (Helper.ConvertirDecimal(oNodoDia.Cells["cpp_Importe"].Value) + oReg.Saldo.Valor());
            }
            // Se llena el acumulado
            mSaldoAcum = mSaldoIni;
            foreach (TreeGridNode oNodo in this.tgvCuentasPorPagar.Nodes)
            {
                mSaldoAcum += Helper.ConvertirDecimal(oNodo.Cells["cpp_Importe"].Value);
                oNodo.Cells["cpp_Acumulado"].Value = mSaldoAcum;
            }

            Cargando.Cerrar();
        }

        #endregion

        #region [ Tab Garantías ]

        private void chkGarMostrarTodas_CheckedChanged(object sender, EventArgs e)
        {
            this.LlenarGarantias(this.Proveedor.ProveedorID);
        }

        private void btnGarNotaDeCredito_Click(object sender, EventArgs e)
        {
            this.GarantiasNotaDeCredito();
        }
                                
        private void dgvGarantias_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvGarantias.CurrentRow == null)
            {
                this.dgvGarantiasDet.Rows.Clear();
                return;
            }

            if (this.dgvGarantias.VerSeleccionNueva())
            {
                this.LlenarGarantiasDetalle(this.dgvGarantias.CurrentRow);
                this.LlenarGarantiasNotaDeCredito(this.dgvGarantias.CurrentRow);
            }
        }

        private void btnGarNoProcedio_Click(object sender, EventArgs e)
        {
            if (this.dgvGarantias.CurrentRow == null) return;
            
            // Se valida es estatus
            if (Helper.ConvertirEntero(this.dgvGarantias.CurrentRow.Cells["garEstatusGenericoID"].Value) != Cat.EstatusGenericos.EnRevision)
                return;

            var oMotivo = UtilLocal.ObtenerValor("Indica cuál es el motivo por el cual no procedió:", "", MensajeObtenerValor.Tipo.TextoLargo);
            if (oMotivo == null) return;

            int iGarantiaID = Helper.ConvertirEntero(this.dgvGarantias.CurrentRow.Cells["garVentaGarantiaID"].Value);
            this.GarantiasMarcarNoProcedio(iGarantiaID, Helper.ConvertirCadena(oMotivo));
        }

        private void LlenarGarantias(int iProveedorID)
        {
            Cargando.Mostrar();

            bool bMostrarTodas = this.chkGarMostrarTodas.Checked;
            var oGarantias = General.GetListOf<VentasGarantiasView>(c => c.ProveedorID == iProveedorID
                && (bMostrarTodas || c.EstatusGenericoID == Cat.EstatusGenericos.EnRevision));
            this.dgvGarantias.Rows.Clear();
            foreach (var oReg in oGarantias)
            {
                int iFila = this.dgvGarantias.Rows.Add(oReg.VentaGarantiaID, oReg.EstatusGenericoID, false, oReg.NumeroDeParte, oReg.NombreDeParte
                    , oReg.Linea, oReg.Marca, oReg.Costo, oReg.Estatus, oReg.FacturaDeCompra);
                this.dgvGarantias.Rows[iFila].Tag = oReg;
            }

            Cargando.Cerrar();
        }

        private void LlenarGarantiasDetalle(DataGridViewRow oFila)
        {
            var oGarantiaV = (oFila.Tag as VentasGarantiasView);
            this.dgvGarantiasDet.Rows.Clear();

            if (oGarantiaV != null)
            {
                this.dgvGarantiasDet.Rows.Add(oGarantiaV.Fecha, oGarantiaV.Sucursal, oGarantiaV.FolioDeVenta, oGarantiaV.Motivo, oGarantiaV.MotivoObservacion
                    , oGarantiaV.Accion, oGarantiaV.Realizo, oGarantiaV.AutorizoUsuario);
            }
        }

        private void LlenarGarantiasNotaDeCredito(DataGridViewRow oFila)
        {
            var oGarantiaV = (oFila.Tag as VentasGarantiasView);
            this.dgvGarantiasNota.Rows.Clear();

            if (oGarantiaV != null && oGarantiaV.ProveedorNotaDeCreditoID.HasValue)
            {
                var oNota = General.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == oGarantiaV.ProveedorNotaDeCreditoID);
                this.dgvGarantiasNota.Rows.Add(oNota.ProveedorNotaDeCreditoID, oNota.Folio, oNota.Fecha, (oNota.Subtotal + oNota.Iva), oNota.Observacion);
            }
        }

        private void GarantiasNotaDeCredito()
        {
            // Se verifican las filas marcadas, que tengan el estatus correcto
            var oGarantias = new List<int>();
            foreach (DataGridViewRow oFila in this.dgvGarantias.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["garSel"].Value))
                    continue;
                if (Helper.ConvertirEntero(oFila.Cells["garEstatusGenericoID"].Value) == Cat.EstatusGenericos.EnRevision)
                    oGarantias.Add(Helper.ConvertirEntero(oFila.Cells["garVentaGarantiaID"].Value));
                else
                    oFila.Cells["garSel"].Value = false;
            }

            if (oGarantias.Count > 0)
            {
                // Se piden los datos de la nota de crédito
                var frmNotaDeCredito = new ProveedorNotaDeCreditoForma();
                if (frmNotaDeCredito.ShowDialog(Principal.Instance) == DialogResult.OK)
                {
                    Cargando.Mostrar();

                    // Se genera la nota de crédito
                    var oNotaC = new ProveedorNotaDeCredito()
                    {
                        ProveedorID = this.Proveedor.ProveedorID,
                        Folio = frmNotaDeCredito.Folio,
                        Fecha = frmNotaDeCredito.Fecha,
                        Subtotal = frmNotaDeCredito.Subtotal,
                        Iva = frmNotaDeCredito.Iva,
                        Disponible = true,
                        OrigenID = Cat.OrigenesNotasDeCreditoProveedor.Garantia,
                        Observacion = frmNotaDeCredito.Observacion
                    };
                    AdmonProc.CrearNotaDeCreditoProveedor(oNotaC);

                    // Se afectan las garantías correspondientes
                    foreach (int iGarantiaID in oGarantias)
                    {
                        var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iGarantiaID && c.Estatus);
                        oGarantia.RespuestaID = Cat.VentasGarantiasRespuestas.NotaDeCredito;
                        oGarantia.EstatusGenericoID = Cat.EstatusGenericos.Resuelto;
                        Guardar.Generico<VentaGarantia>(oGarantia);

                        // Se guarda el detalle de la Nota de crédito
                        var oNotaDet = new ProveedorNotaDeCreditoDetalle()
                        {
                            ProveedorNotaDeCreditoID = oNotaC.ProveedorNotaDeCreditoID,
                            MovimientoInventarioID = oGarantia.MovimientoInventarioID.Valor()
                        };
                    }

                    Cargando.Cerrar();

                    this.LlenarGarantias(this.Proveedor.ProveedorID);
                }
                frmNotaDeCredito.Dispose();
            }
        }

        private void GarantiasMarcarNoProcedio(int iGarantiaID, string sMotivo)
        {
            var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iGarantiaID && c.Estatus);
            // oGarantia.FechaCompletado = DateTime.Now;
            oGarantia.RespuestaID = Cat.VentasGarantiasRespuestas.NoProcedio;
            oGarantia.ObservacionCompletado = sMotivo;
            oGarantia.EstatusGenericoID = Cat.EstatusGenericos.Resuelto;
            Guardar.Generico<VentaGarantia>(oGarantia);

            this.LlenarGarantias(this.Proveedor.ProveedorID);
        }
                
        #endregion

        #region [ Tab Devoluciones ]

        private void chkDevMostrarTodas_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkDevMostrarTodas.Focused)
                this.CargarDevoluciones(this.Proveedor.ProveedorID);
        }

        private void dgvDevoluciones_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDevoluciones.VerSeleccionNueva())
                this.LlenarDetalleDevolucion();
        }

        private void dgvDevoluciones_KeyDown(object sender, KeyEventArgs e)
        {
            this.dgvDevoluciones.VerEspacioCheck(e, "des_Sel");
        }

        private void btnDevNotaDeCredito_Click(object sender, EventArgs e)
        {
            this.DevolucionesNotasDeCredito();
        }

        public void CargarDevoluciones(int proveedorId)
        {
            Cargando.Mostrar();

            var devs = Negocio.General.GetListOf<MovimientoInventario>(c => c.ProveedorID == Proveedor.ProveedorID
                && c.TipoOperacionID == Cat.TiposDeOperacionMovimientos.DevolucionAProveedor 
                && c.TipoConceptoOperacionID != Cat.MovimientosConceptosDeOperacion.DevolucionGarantia
                && (this.chkDevMostrarTodas.Checked || (!c.FueLiquidado)));
                // Aquí quizá sea necesario agregar un filtro para no mostrar las devoluciones ya usadas, o algo así
                // && (this.chkDevMostrarTodas.Checked || (!c.AplicaEnMovimientoInventarioID.HasValue && !c.FueLiquidado)));
            this.dgvDevoluciones.Rows.Clear();
            foreach (var oReg in devs)
                this.dgvDevoluciones.Rows.Add(oReg.MovimientoInventarioID, false, oReg.MovimientoInventarioID, oReg.FolioFactura, oReg.FechaRegistro
                    , oReg.ImporteTotal, oReg.Observacion);
            // this.dgvDevoluciones.DefaultCellStyle.ForeColor = Color.Black;
            // this.dgvDevoluciones.BackgroundColor = Color.FromArgb(188, 199, 216);

            Cargando.Cerrar();
        }

        private void LlenarDetalleDevolucion()
        {
            this.dgvDevolucionesDetalle.Rows.Clear();
            if (this.dgvDevoluciones.CurrentRow == null)
                return;

            int iDevID = Helper.ConvertirEntero(this.dgvDevoluciones.CurrentRow.Cells["des_MovimientoInventarioID"].Value);
            var oDetalle = General.GetListOf<MovimientosInventarioDetalleAvanzadoView>(c => c.MovimientoInventarioID == iDevID);
            foreach (var oReg in oDetalle)
                this.dgvDevolucionesDetalle.Rows.Add(oReg.MovimientoInventarioDetalleID, oReg.Linea, oReg.Marca, oReg.NumeroDeParte, oReg.Descripcion);
        }

        private void DevolucionesNotasDeCredito()
        {
            // Se verifican las filas marcadas, que tengan el estatus correcto
            var oDevs = new List<int>();
            foreach (DataGridViewRow oFila in this.dgvDevoluciones.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["des_Sel"].Value))
                    continue;
                oDevs.Add(Helper.ConvertirEntero(oFila.Cells["des_MovimientoInventarioID"].Value));
            }

            if (oDevs.Count > 0)
            {
                // Se piden los datos de la nota de crédito
                var frmNotaDeCredito = new ProveedorNotaDeCreditoForma();
                if (frmNotaDeCredito.ShowDialog(Principal.Instance) == DialogResult.OK)
                {
                    Cargando.Mostrar();

                    // Se genera la nota de crédito
                    var oNotaC = new ProveedorNotaDeCredito()
                    {
                        ProveedorID = this.Proveedor.ProveedorID,
                        Folio = frmNotaDeCredito.Folio,
                        Fecha = frmNotaDeCredito.Fecha,
                        Subtotal = frmNotaDeCredito.Subtotal,
                        Iva = frmNotaDeCredito.Iva,
                        Disponible = true,
                        OrigenID = Cat.OrigenesNotasDeCreditoProveedor.Devolucion,
                        Observacion = frmNotaDeCredito.Observacion
                    };
                    AdmonProc.CrearNotaDeCreditoProveedor(oNotaC);

                    // Se marcan los movimientos afectados
                    foreach (int iMovID in oDevs)
                    {
                        var oMovDev = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovID && c.Estatus);
                        oMovDev.FueLiquidado = true;
                        Guardar.Generico<MovimientoInventario>(oMovDev);

                        // Se guarda el detalle de la Nota de crédito
                        var oNotaDet = new ProveedorNotaDeCreditoDetalle()
                        {
                            ProveedorNotaDeCreditoID = oNotaC.ProveedorNotaDeCreditoID,
                            MovimientoInventarioID = iMovID
                        };
                    }

                    Cargando.Cerrar();
                }
                frmNotaDeCredito.Dispose();

                this.CargarDevoluciones(this.Proveedor.ProveedorID);
            }
        }

        #endregion

        #region [ Tab Descuentos Ganancias ]

        private void tgvDescGan_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var oText = (e.Control as TextBox);
            if (oText != null)
                oText.KeyDown += tgvDescGan_TextBox_KeyDown;
        }
        Keys oUlt;
        void tgvDescGan_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            this.oUlt = e.KeyCode;
            if (e.KeyCode == Keys.Enter)
            {
                this.tgvDescGan_KeyDown(sender, e);
            }
        }

        private void tgvDescGan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.tgvDescGan.CurrentCell.ColumnIndex < (this.tgvDescGan.Columns.Count - 1))
                {
                    this.tgvDescGan.CurrentCell = this.tgvDescGan.CurrentCell.OwningRow.Cells[this.tgvDescGan.CurrentCell.ColumnIndex + 1];
                    e.Handled = true;
                }
            }
        }

        private void tgvDescGan_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.oUlt == Keys.Enter)
            {
                if (this.tgvDescGan.CurrentCell.ColumnIndex < (this.tgvDescGan.Columns.Count - 1))
                    this.tgvDescGan.CurrentCell = this.tgvDescGan.CurrentCell.OwningRow.Cells[this.tgvDescGan.CurrentCell.ColumnIndex + 1];
            }

        }

        private void tgvDescGan_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.tgvDescGan.VerSeleccionNueva())
            {
                if (this.tgvDescGan.CurrentNode == null)
                    this.txtDescGanObservacion.Clear();
                else
                    this.txtDescGanObservacion.Text = Helper.ConvertirCadena(this.tgvDescGan.CurrentNode.Cells["dcgObservacion"].Value);
            }
        }

        private void tgvDescGan_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.tgvDescGan.Columns[e.ColumnIndex] == this.dcgNombre)
                this.CargarPartesLinea(this.tgvDescGan.CurrentNode);
        }

        private void tgvDescGan_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.bCargandoDescuentosGanancias || this.tgvDescGan.CurrentNode == null) return;
            this.VerMarcarCambio(this.tgvDescGan.CurrentNode.Cells[e.ColumnIndex]);
        }

        private void btnDescGanGuardar_Click(object sender, EventArgs e)
        {
            this.GuardarDescuentosGanancias();
        }

        private void VerMarcarCambio(DataGridViewCell oCelda)
        {
            // Se verifica si ya se marcó la fila como modificada, en base al tag
            if (oCelda.Tag != null)
                return;

            // Se marca la fila como modificada y se manda afectar los nodos hijos
            oCelda.Style.ForeColor = Color.Orange;
            oCelda.Tag = true;
            oCelda.OwningRow.Tag = true;
            if (oCelda.OwningColumn.Name != "dcgObservacion")
                this.VerCambiosEnCascada(this.tgvDescGan.GetNodeForRow(oCelda.OwningRow), oCelda.ColumnIndex, oCelda.Value);
        }

        private void VerCambiosEnCascada(TreeGridNode oNodo, int iCol, object oValor)
        {
            if (oNodo.Nodes.Count > 0)
            {
                var oCelda = oNodo.Cells[iCol];
                oCelda.Style.ForeColor = Color.Orange;
                oCelda.Tag = true;
                oCelda.OwningRow.Tag = true;
                oCelda.Value = oValor;

                foreach (var oNodoHijo in oNodo.Nodes)
                    this.VerCambiosEnCascada(oNodoHijo, iCol, oValor);
            }
            else
            {
                var oCelda = oNodo.Cells[iCol];
                oCelda.Style.ForeColor = Color.Orange;
                oCelda.Tag = true;
                oCelda.OwningRow.Tag = true;
                oCelda.Value = oValor;
            }
        }

        private void CargarDescuentosGanancias(int iProveedorID)
        {
            Cargando.Mostrar();
            this.bCargandoDescuentosGanancias = true;

            string sProveedor = "", sMarca = "", sLinea = "";
            TreeGridNode oNodoProveedor = null, oNodoMarca = null, oNodoLinea = null;
            var oDatos = General.GetListOf<ProveedoresPartesGananciasLineasView>(c => c.ProveedorID == iProveedorID)
                .OrderBy(c => c.Proveedor).ThenBy(c => c.Marca).ThenBy(c => c.Linea).ThenBy(c => c.Parte);
            this.tgvDescGan.Nodes.Clear();
            foreach (var oReg in oDatos)
            {
                if (oReg.Proveedor != sProveedor)
                {
                    sProveedor = oReg.Proveedor;
                    oNodoProveedor = this.tgvDescGan.Nodes.Add(oReg.ProveedorParteGananciaID, oReg.ProveedorID, sProveedor
                        , oReg.DescuentoFactura1, oReg.DescuentoFactura2, oReg.DescuentoFactura3
                        , oReg.DescuentoArticulo1, oReg.DescuentoArticulo2, oReg.DescuentoArticulo3, oReg.PorcentajeDeGanancia1, oReg.PorcentajeDeGanancia2
                        , oReg.PorcentajeDeGanancia3, oReg.PorcentajeDeGanancia4, oReg.PorcentajeDeGanancia5, oReg.Observacion);
                    oNodoProveedor.Expand();
                    continue;
                }
                if (oReg.Marca != sMarca)
                {
                    sMarca = oReg.Marca;
                    oNodoMarca = oNodoProveedor.Nodes.Add(oReg.ProveedorParteGananciaID, oReg.MarcaID, sMarca
                        , oReg.DescuentoFactura1, oReg.DescuentoFactura2, oReg.DescuentoFactura3
                        , oReg.DescuentoArticulo1, oReg.DescuentoArticulo2, oReg.DescuentoArticulo3, oReg.PorcentajeDeGanancia1, oReg.PorcentajeDeGanancia2
                        , oReg.PorcentajeDeGanancia3, oReg.PorcentajeDeGanancia4, oReg.PorcentajeDeGanancia5, oReg.Observacion);
                    continue;
                }
                if (oReg.Linea != sLinea)
                {
                    sLinea = oReg.Linea;
                    oNodoLinea = oNodoMarca.Nodes.Add(oReg.ProveedorParteGananciaID, oReg.LineaID, sLinea
                        , oReg.DescuentoFactura1, oReg.DescuentoFactura2, oReg.DescuentoFactura3
                        , oReg.DescuentoArticulo1, oReg.DescuentoArticulo2, oReg.DescuentoArticulo3, oReg.PorcentajeDeGanancia1, oReg.PorcentajeDeGanancia2
                        , oReg.PorcentajeDeGanancia3, oReg.PorcentajeDeGanancia4, oReg.PorcentajeDeGanancia5, oReg.Observacion);
                    continue;
                }

                /* Se carga sólo hasta líneas, para que sea már rápido
                oNodoLinea.Nodes.Add(oReg.ProveedorParteGananciaID, oReg.ParteID, oReg.Parte
                    , oReg.DescuentoFactura1, oReg.DescuentoFactura2, oReg.DescuentoFactura3
                    , oReg.DescuentoArticulo1, oReg.DescuentoArticulo2, oReg.DescuentoArticulo3, oReg.PorcentajeDeGanancia1, oReg.PorcentajeDeGanancia2
                    , oReg.PorcentajeDeGanancia3, oReg.PorcentajeDeGanancia4, oReg.PorcentajeDeGanancia5);
                */
            }

            this.bCargandoDescuentosGanancias = false;
            Cargando.Cerrar();
        }

        private void CargarPartesLinea(TreeGridNode oNodo)
        {
            Cargando.Mostrar();

            int iLineaID = Helper.ConvertirEntero(oNodo.Cells["dcgId"].Value);
            int iMarcaID = Helper.ConvertirEntero(oNodo.Parent.Cells["dcgId"].Value);
            int iProveedorID = Helper.ConvertirEntero(oNodo.Parent.Parent.Cells["dcgId"].Value);
            var oDatos = General.GetListOf<ProveedoresPartesGananciasView>(c => c.ProveedorID == iProveedorID && c.MarcaID == iMarcaID && c.LineaID == iLineaID
                && c.ParteID > 0).OrderBy(c => c.ParteID);
            oNodo.Nodes.Clear();
            foreach (var oReg in oDatos)
            {
                oNodo.Nodes.Add(oReg.ProveedorParteGananciaID, oReg.ParteID, oReg.Parte
                    , oReg.DescuentoFactura1, oReg.DescuentoFactura2, oReg.DescuentoFactura3
                    , oReg.DescuentoArticulo1, oReg.DescuentoArticulo2, oReg.DescuentoArticulo3, oReg.PorcentajeDeGanancia1, oReg.PorcentajeDeGanancia2
                    , oReg.PorcentajeDeGanancia3, oReg.PorcentajeDeGanancia4, oReg.PorcentajeDeGanancia5, oReg.Observacion);
            }
            
            Cargando.Cerrar();
        }

        private void GuardarDescuentosGanancias()
        {
            if (UtilLocal.MensajePreguntaCancelar("¿Estás seguro que deseas guardar los cambios realizados?") != DialogResult.Yes)
                return;

            Cargando.Mostrar();

            foreach (DataGridViewRow oFila in this.tgvDescGan.Rows)
            {
                if (oFila.Tag == null) continue;

                var oNodo = this.tgvDescGan.GetNodeForRow(oFila);
                int? iProveedorID = null, iMarcaID = null, iLineaID = null, iParteID = null;
                switch (oNodo.Level)
                {
                    case 1:
                        iProveedorID = (int)oNodo.Cells["dcgId"].Value;
                        break;
                    case 2:
                        iProveedorID = (int)oNodo.Parent.Cells["dcgId"].Value;
                        iMarcaID = (int)oNodo.Cells["dcgId"].Value;
                        break;
                    case 3:
                        iProveedorID = (int)oNodo.Parent.Parent.Cells["dcgId"].Value;
                        iMarcaID = (int)oNodo.Parent.Cells["dcgId"].Value;
                        iLineaID = (int)oNodo.Cells["dcgId"].Value;
                        break;
                    case 4:
                        iProveedorID = (int)oNodo.Parent.Parent.Parent.Cells["dcgId"].Value;
                        iMarcaID = (int)oNodo.Parent.Parent.Cells["dcgId"].Value;
                        iLineaID = (int)oNodo.Parent.Cells["dcgId"].Value;
                        iParteID = (int)oNodo.Cells["dcgId"].Value;
                        break;
                }

                int iParteGananciaID = Helper.ConvertirEntero(oNodo.Cells["dcgProveedorParteGananciaID"].Value);
                ProveedorParteGanancia oParteGanancia;
                if (iParteGananciaID > 0)
                {
                    oParteGanancia = General.GetEntity<ProveedorParteGanancia>(c => c.ProveedorParteGananciaID == iParteGananciaID);
                }
                else
                {
                    oParteGanancia = new ProveedorParteGanancia() { ProveedorID = iProveedorID.Value, MarcaParteID = iMarcaID, LineaID = iLineaID, ParteID = iParteID };
                    oParteGanancia.DescuentoFactura1 = oParteGanancia.DescuentoFactura2 = oParteGanancia.DescuentoFactura3 =
                        oParteGanancia.DescuentoArticulo1 = oParteGanancia.DescuentoArticulo2 = oParteGanancia.DescuentoArticulo3 =
                        oParteGanancia.PorcentajeDeGanancia1 = oParteGanancia.PorcentajeDeGanancia2 = oParteGanancia.PorcentajeDeGanancia3 =
                        oParteGanancia.PorcentajeDeGanancia4 = oParteGanancia.PorcentajeDeGanancia5 = 0;
                }
                
                if (oNodo.Cells["dcgDescuentoFactura1"].Tag != null)
                    oParteGanancia.DescuentoFactura1 = Helper.ConvertirDecimal(oNodo.Cells["dcgDescuentoFactura1"].Value);
                if (oNodo.Cells["dcgDescuentoFactura2"].Tag != null)
                    oParteGanancia.DescuentoFactura2 = Helper.ConvertirDecimal(oNodo.Cells["dcgDescuentoFactura2"].Value);
                if (oNodo.Cells["dcgDescuentoFactura3"].Tag != null)
                    oParteGanancia.DescuentoFactura3 = Helper.ConvertirDecimal(oNodo.Cells["dcgDescuentoFactura3"].Value);
                if (oNodo.Cells["dcgDescuentoArticulo1"].Tag != null)
                    oParteGanancia.DescuentoArticulo1 = Helper.ConvertirDecimal(oNodo.Cells["dcgDescuentoArticulo1"].Value);
                if (oNodo.Cells["dcgDescuentoArticulo2"].Tag != null)
                    oParteGanancia.DescuentoArticulo2 = Helper.ConvertirDecimal(oNodo.Cells["dcgDescuentoArticulo2"].Value);
                if (oNodo.Cells["dcgDescuentoArticulo3"].Tag != null)
                    oParteGanancia.DescuentoArticulo3 = Helper.ConvertirDecimal(oNodo.Cells["dcgDescuentoArticulo3"].Value);
                if (oNodo.Cells["dcgPorcentajeDeGanancia1"].Tag != null)
                    oParteGanancia.PorcentajeDeGanancia1 = Helper.ConvertirDecimal(oNodo.Cells["dcgPorcentajeDeGanancia1"].Value);
                if (oNodo.Cells["dcgPorcentajeDeGanancia2"].Tag != null)
                    oParteGanancia.PorcentajeDeGanancia2 = Helper.ConvertirDecimal(oNodo.Cells["dcgPorcentajeDeGanancia2"].Value);
                if (oNodo.Cells["dcgPorcentajeDeGanancia3"].Tag != null)
                    oParteGanancia.PorcentajeDeGanancia3 = Helper.ConvertirDecimal(oNodo.Cells["dcgPorcentajeDeGanancia3"].Value);
                if (oNodo.Cells["dcgPorcentajeDeGanancia4"].Tag != null)
                    oParteGanancia.PorcentajeDeGanancia4 = Helper.ConvertirDecimal(oNodo.Cells["dcgPorcentajeDeGanancia4"].Value);
                if (oNodo.Cells["dcgPorcentajeDeGanancia5"].Tag != null)
                    oParteGanancia.PorcentajeDeGanancia5 = Helper.ConvertirDecimal(oNodo.Cells["dcgPorcentajeDeGanancia5"].Value);

                // Se guarda la observación
                if (oNodo.Cells["dcgObservacion"].Tag != null)
                {
                    if (!string.IsNullOrEmpty(oParteGanancia.Observacion))
                        oParteGanancia.Observacion += "\r\n";
                    oParteGanancia.Observacion += string.Format("{0} / {1} / {2}", DateTime.Now, GlobalClass.UsuarioGlobal.NombreUsuario
                        , Helper.ConvertirCadena(oNodo.Cells["dcgObservacion"].Value));
                }

                Guardar.Generico<ProveedorParteGanancia>(oParteGanancia);
            }

            Cargando.Cerrar();

            this.CargarDescuentosGanancias(this.Proveedor.ProveedorID);
        }

        #endregion

    }
}
