using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class catalogosTraspasos : UserControl
    {
        ControlError cntError = new ControlError();
        DataTable dtDetalleTraspasos = new DataTable();
        public static catalogosTraspasos Instance
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

            internal static readonly catalogosTraspasos instance = new catalogosTraspasos();
        }

        public catalogosTraspasos()
        {
            InitializeComponent();
        }

        #region [ Metodos g ]

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            /*if (keyData == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
                return true;
            }*/
            return base.ProcessCmdKey(ref msg, keyData);
        }

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
                //this.tabTraspasos.SelectedIndex = 0;
                //this.LimpiarFormulario();
                //this.txtCodigo.Clear();
                //this.txtDescripcion.Clear();
                this.configurarGridTraspasos();

                //this.dtpFechaDesde.Value = DateTime.Now;
                //this.dtpFechaHasta.Value = DateTime.Now;

                var listaMarcaParte = General.GetListOf<MarcaParte>(m => m.Estatus.Equals(true));
                listaMarcaParte.Insert(0, new MarcaParte() { MarcaParteID = 0, NombreMarcaParte = "TODOS", UsuarioID = 1, FechaRegistro = DateTime.Now, Estatus = true, Actualizar = true });
                this.cboMarca.DataSource = listaMarcaParte;
                this.cboMarca.DisplayMember = "NombreMarcaParte";
                this.cboMarca.ValueMember = "MarcaParteID";
                AutoCompleteStringCollection autMarcaParte = new AutoCompleteStringCollection();
                foreach (var marcaParte in listaMarcaParte) autMarcaParte.Add(marcaParte.NombreMarcaParte);
                this.cboMarca.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboMarca.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboMarca.AutoCompleteCustomSource = autMarcaParte;
                this.cboMarca.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);

                var listaLineas = General.GetListOf<LineasView>(l => l.LineaID > 0);
                listaLineas.Insert(0, new LineasView() { LineaID = 0, NombreLinea = "TODOS" });
                this.cboLinea.DataSource = listaLineas;
                this.cboLinea.DisplayMember = "NombreLinea";
                this.cboLinea.ValueMember = "LineaID";
                AutoCompleteStringCollection autLinea = new AutoCompleteStringCollection();
                foreach (var linea in listaLineas) autLinea.Add(linea.NombreLinea);
                this.cboLinea.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboLinea.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboLinea.AutoCompleteCustomSource = autLinea;
                this.cboLinea.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);

                var listaProveedores = General.GetListOf<ProveedoresView>(p => p.ProveedorID > 0).OrderBy(p=>p.NombreProveedor).ToList();
                listaProveedores.Insert(0, new ProveedoresView() { ProveedorID = 0, NombreProveedor = "TODOS" });
                this.cboProveedor.DataSource = listaProveedores;
                this.cboProveedor.DisplayMember = "NombreProveedor";
                this.cboProveedor.ValueMember = "ProveedorID";
                AutoCompleteStringCollection autProveedor = new AutoCompleteStringCollection();
                foreach (var proveedor in listaProveedores) autProveedor.Add(proveedor.NombreProveedor);
                this.cboProveedor.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboProveedor.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboProveedor.AutoCompleteCustomSource = autProveedor;
                this.cboProveedor.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);

                this.cboUbicacionOrigen.DataSource = General.GetListOf<Sucursal>(s => s.Estatus.Equals(true) && s.SucursalID == GlobalClass.SucursalID);
                this.cboUbicacionOrigen.DisplayMember = "NombreSucursal";
                this.cboUbicacionOrigen.ValueMember = "SucursalID";

                this.cboUbicacionDestino.DataSource = General.GetListOf<Sucursal>(s => s.Estatus.Equals(true) && s.SucursalID != GlobalClass.SucursalID);
                this.cboUbicacionDestino.DisplayMember = "NombreSucursal";
                this.cboUbicacionDestino.ValueMember = "SucursalID";
                this.cboUbicacionDestino.SelectedIndex = -1;

                this.cboSolicito.DataSource = General.GetListOf<Usuario>(u => u.Estatus.Equals(true) && u.Activo);
                this.cboSolicito.DisplayMember = "NombreUsuario";
                this.cboSolicito.ValueMember = "UsuarioID";

                this.cboSucursalRpt.DataSource = General.GetListOf<Sucursal>(s => s.Estatus.Equals(true));
                this.cboSucursalRpt.DisplayMember = "NombreSucursal";
                this.cboSucursalRpt.ValueMember = "SucursalID";

                //var abcs = General.GetListOf<ParteAbc>(p => p.ParteAbcID > 0).GroupBy(p => p.AbcDeVentas).OrderBy(x => x.Key).ToList();
                //var cadena = new StringBuilder();

                //foreach (var abc in abcs)                
                //    cadena.Append(string.Format("{0}{1}", abc.Key, ","));
                
                //if (cadena.Length > 0)                                    
                //    this.txtAbcs.Text = cadena.ToString().Substring(0, cadena.Length - 1);

                //if (this.clbAbc.Items.Count > 0)
                //{
                //    ((ListBox)clbAbc).DataSource = null;
                //    this.clbAbc.Items.Clear();
                //}

                var criterios = General.GetListOf<CriterioABC>(c => c.Estatus);
                criterios.Sort((x, y) => x.Clasificacion.CompareTo(y.Clasificacion));
                ((ListBox)clbAbc).DataSource = criterios;
                ((ListBox)clbAbc).DisplayMember = "Clasificacion";
                ((ListBox)clbAbc).ValueMember = "CriterioAbcID";
                
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void LimpiarFormulario()
        {
            try
            {
                this.tabTraspasos.SelectedIndex = 0;
                this.lblEncontrados.Text = "Encontrados:";

                this.dgvDatos.DataSource = null;
                if (this.dgvDatos.Rows.Count > 0)
                    this.dgvDatos.Rows.Clear();
                if (this.dgvDatos.Columns.Count > 0)

                    this.dgvTraspasos.DataSource = null;
                if (this.dgvTraspasos.Rows.Count > 0)
                    this.dgvTraspasos.Rows.Clear();
                if (this.dgvTraspasos.Columns.Count > 0)
                    this.dgvTraspasos.Columns.Clear();

                this.dgvExistencias.DataSource = null;
                if (this.dgvExistencias.Rows.Count > 0)
                    this.dgvExistencias.Rows.Clear();
                if (this.dgvExistencias.Columns.Count > 0)
                    this.dgvExistencias.Columns.Clear();

                this.LimpiarTabRecibir();
                this.LimpiarTabHistorico();
                this.LimpiarTabConflictos();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void LimpiarTabRecibir()
        {
            this.lblEncontradosRecibir.Text = "Encontrados:";

            this.dgvRecibir.DataSource = null;
            if (this.dgvRecibir.RowCount > 0)
            {
                if (this.dgvRecibir.Rows.Count > 0)
                    this.dgvRecibir.Rows.Clear();
                if (this.dgvRecibir.Columns.Count > 0)
                    this.dgvRecibir.Columns.Clear();
            }

            this.dgvRecibirDetalle.DataSource = null;
            if (this.dgvRecibirDetalle.Rows.Count > 0)
                this.dgvRecibirDetalle.Rows.Clear();
            if (this.dgvRecibirDetalle.Columns.Count > 0)
                this.dgvRecibirDetalle.Columns.Clear();

            this.txtMotivo.Clear();
        }

        private void LimpiarTabHistorico()
        {
            this.lblEncontradosHis.Text = "Encontrados:";
            this.txtDetalleConflicto.Clear();

            this.dgvHistorico.DataSource = null;
            if (this.dgvHistorico.RowCount > 0)
            {
                if (this.dgvHistorico.Rows.Count > 0)
                    this.dgvHistorico.Rows.Clear();
                if (this.dgvHistorico.Columns.Count > 0)
                    this.dgvHistorico.Columns.Clear();
            }

            this.dgvHistoricoDetalle.DataSource = null;
            if (this.dgvHistoricoDetalle.Rows.Count > 0)
                this.dgvHistoricoDetalle.Rows.Clear();
            if (this.dgvHistoricoDetalle.Columns.Count > 0)
                this.dgvHistoricoDetalle.Columns.Clear();
        }

        private void LimpiarTabConflictos()
        {
            this.lblEncontradosCon.Text = "Encontrados:";

            this.dgvConflictos.DataSource = null;
            if (this.dgvConflictos.RowCount > 0)
            {
                if (this.dgvConflictos.Rows.Count > 0)
                    this.dgvConflictos.Rows.Clear();
                if (this.dgvConflictos.Columns.Count > 0)
                    this.dgvConflictos.Columns.Clear();
            }
        }

        #endregion

        #region [ Registrar Traspaso ]

        #region [ Eventos ]

        private void catalogosTraspasos_Load(object sender, EventArgs e)
        {

        }

        private void cboUbicacionDestino_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.LimpiarFormulario();
            this.configurarGridTraspasos();
        }

        private void cboUbicacionOrigen_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.LimpiarFormulario();
        }

        private void cboMarca_SelectedValueChanged(object sender, EventArgs e)
        {
            //if (cboMarca.SelectedValue == null)
            //    return;
            //int id;
            //if (int.TryParse(cboMarca.SelectedValue.ToString(), out id))
            //{
            //    CargarLineas(id);
            //}
        }

        private void cboLinea_SelectedValueChanged(object sender, EventArgs e)
        {
            //int id;
            //if (int.TryParse(cboLinea.SelectedValue.ToString(), out id))
            //{
            //    this.CargarSistemas(id);
            //}
        }
        
        private void btnMostrar_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtCodigo.Clear();
                this.txtDescripcion.Clear();
                int marcaparteId = Helper.ConvertirEntero(this.cboMarca.SelectedValue);
                int lineaId = Helper.ConvertirEntero(this.cboLinea.SelectedValue);
                int proveedorId = Helper.ConvertirEntero(this.cboProveedor.SelectedValue);
                //int sistemaId = Helper.ConvertirEntero(this.cboSistema.SelectedValue);
                //int subsistemaId = Helper.ConvertirEntero(this.cboSubsistema.SelectedValue);
                int sucursalOrigenId = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue);
                int sucursalDestinoId = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue);

                var abcs = new StringBuilder();

                foreach (object itemChecked in this.clbAbc.CheckedItems)
                {
                    var castedItem = itemChecked as CriterioABC;
                    abcs.Append(string.Format("{0}{1}", castedItem.Clasificacion, ","));
                }

                if (this.clbAbc.CheckedItems.Count <= 0)
                {
                    Helper.MensajeError("Debe seleccionar un criterio ABC", GlobalClass.NombreApp);
                    return;
                }

                var dic = new Dictionary<string, object>();
                dic.Add("MarcaParteID", marcaparteId);
                dic.Add("LineaID", lineaId);
                //dic.Add("SistemaID", sistemaId);
                //dic.Add("SubsistemaID", subsistemaId);
                dic.Add("ProveedorID", proveedorId);
                dic.Add("SucursalOrigenID", sucursalOrigenId);
                dic.Add("SucursalDestinoID", sucursalDestinoId);
                dic.Add("Abcs", abcs.ToString().Substring(0, abcs.ToString().Length -1));
                var lst = General.ExecuteProcedure<pauParteBusquedaEnTraspasos_Result>("pauParteBusquedaEnTraspasos", dic);

                if (lst != null)
                {
                    this.dgvDatos.DataSource = null;
                    this.dgvDatos.Columns.Clear();
                    var det = new SortableBindingList<pauParteBusquedaEnTraspasos_Result>(lst);
                    this.dgvDatos.DataSource = det;
                    this.lblEncontrados.Text = string.Format("Encontrados: {0}", lst.Count);
                    Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "MarcaParteID", "LineaID", "SistemaID", "SubsistemaID", 
                        "SucursalID", "Maximo", "Minimo", "SucursalDestinoID", "DestinoExistencia", "DestinoMaximo", "DestinoMinimo", "Busqueda" });
                    Helper.ColumnasToHeaderText(this.dgvDatos);
                                        
                    this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";
                    if (!this.dgvDatos.Columns.Contains("X"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "X";
                        checkColumn.HeaderText = "";
                        checkColumn.Width = 50;
                        checkColumn.ReadOnly = false;
                        checkColumn.FillWeight = 10;
                        this.dgvDatos.Columns.Add(checkColumn);
                        this.dgvDatos.Columns["X"].DisplayIndex = 0;
                    }

                    foreach (DataGridViewColumn column in this.dgvDatos.Columns)
                    {
                        if (!column.Name.Equals("X"))
                        {
                            column.ReadOnly = true;
                            dgvDatos.AutoResizeColumn(column.Index, DataGridViewAutoSizeColumnMode.AllCells);
                        }
                        if (column.Name.Equals("Existencia") || column.Name.Equals("Sugerencia"))
                        {
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                        }
                    }                    
                    this.btnImprimirSugerido.Enabled = true;
                    this.dgvDatos.Columns[2].Width = 250;
                }
                else
                {
                    this.lblEncontrados.Text = "Encontrados:";
                    this.btnImprimirSugerido.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnImprimirSugerido_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvDatos.DataSource == null)
                    return;

                //(System.Collections.Generic.List<pauParteBusquedaEnTraspasos_Result>)
                var lista = new List<pauParteBusquedaEnTraspasos_Result>();
                foreach (DataGridViewRow fila in this.dgvDatos.Rows)
                {
                    var cb = (DataGridViewCheckBoxCell)fila.Cells["X"];
                    if (cb.Value != null)
                    {
                        if (Helper.ConvertirBool(cb.Value))
                        {
                            var sugerido = new pauParteBusquedaEnTraspasos_Result()
                            {
                                ParteID = Helper.ConvertirEntero(fila.Cells["ParteID"].Value),
                                NumeroParte = Helper.ConvertirCadena(fila.Cells["NumeroParte"].Value),
                                NombreParte = Helper.ConvertirCadena(fila.Cells["NombreParte"].Value),
                                MarcaParteID = Helper.ConvertirEntero(fila.Cells["MarcaParteID"].Value),
                                LineaID = Helper.ConvertirEntero(fila.Cells["LineaID"].Value),
                                SistemaID = Helper.ConvertirEntero(fila.Cells["SistemaID"].Value),
                                SubsistemaID = Helper.ConvertirEntero(fila.Cells["SubsistemaID"].Value),
                                SucursalID = Helper.ConvertirEntero(fila.Cells["SucursalID"].Value),
                                Existencia = Helper.ConvertirDecimal(fila.Cells["Existencia"].Value),
                                Maximo = Helper.ConvertirDecimal(fila.Cells["Maximo"].Value),
                                Minimo = Helper.ConvertirDecimal(fila.Cells["Minimo"].Value),
                                SucursalDestinoID = Helper.ConvertirEntero(fila.Cells["SucursalDestinoID"].Value),
                                DestinoExistencia = Helper.ConvertirDecimal(fila.Cells["DestinoExistencia"].Value),
                                DestinoMaximo = Helper.ConvertirDecimal(fila.Cells["DestinoMaximo"].Value),
                                DestinoMinimo = Helper.ConvertirDecimal(fila.Cells["DestinoMinimo"].Value),
                                Sugerencia = Helper.ConvertirDecimal(fila.Cells["Sugerencia"].Value)
                            };
                            lista.Add(sugerido);
                        }
                    }
                }
                IEnumerable<pauParteBusquedaEnTraspasos_Result> listaE = lista;

                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteSugeridoTraspaso.frx"));
                    report.SetParameterValue("Origen", this.cboUbicacionOrigen.Text);
                    report.SetParameterValue("Destino", this.cboUbicacionDestino.Text);
                    report.SetParameterValue("FechaHora", DateTime.Now);
                    report.RegisterData(listaE, "sugerido", 3);
                    report.GetDataSource("sugerido").Enabled = true;
                    report.Show(true);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var dic = new Dictionary<string, object>();
                dic.Add("Codigo", this.txtCodigo.Text.Replace("'", ""));
                dic.Add("SucursalOrigenID", Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue));
                dic.Add("SucursalDestinoID", Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue));
                for (int x = 0; x < 9; x++)
                {
                    dic.Add(string.Format("{0}{1}", "Descripcion", x + 1), null);
                }

                var lst = Negocio.General.ExecuteProcedure<pauParteBusquedaAvanzadaEnTraspasos_Result>("pauParteBusquedaAvanzadaEnTraspasos", dic);
                if (lst.Count > 0)
                {
                    this.dgvDatos.DataSource = null;
                    this.dgvDatos.Columns.Clear();
                    this.dgvDatos.DataSource = Helper.LinqToDataTable < pauParteBusquedaAvanzadaEnTraspasos_Result >( lst);
                    this.lblEncontrados.Text = string.Format("Encontrados: {0}", lst.Count);
                    Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "MarcaParteID", "LineaID", "SistemaID", "SubsistemaID", 
                        "SucursalID", "Maximo", "Minimo", "SucursalDestinoID", "DestinoExistencia", "DestinoMaximo", "DestinoMinimo" });
                    Helper.ColumnasToHeaderText(this.dgvDatos);

                    this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";
                    if (!this.dgvDatos.Columns.Contains("X"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "X";
                        checkColumn.HeaderText = "";
                        checkColumn.Width = 50;
                        checkColumn.ReadOnly = false;
                        checkColumn.FillWeight = 10;
                        this.dgvDatos.Columns.Add(checkColumn);
                        this.dgvDatos.Columns["X"].DisplayIndex = 0;
                    }

                    foreach (DataGridViewColumn column in this.dgvDatos.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.Automatic;
                        if (!column.Name.Equals("X"))
                        {
                            column.ReadOnly = true;
                        }
                    }
                    this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
                else
                {
                    dgvDatos.DataSource = null;
                    lblEncontrados.Text = "Encontrados:";
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            if (this.txtDescripcion.TextLength < 4)
                return;

            try
            {
                string input = this.txtDescripcion.Text.Replace("'", "");
                string[] matches = Regex.Matches(input, @""".*?""|[^\s]+").Cast<Match>().Select(m => m.Value).ToArray();

                if (matches.Length > 0 && matches.Length < 10)
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("Codigo", null);
                    dic.Add("SucursalOrigenID", Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue));
                    dic.Add("SucursalDestinoID", Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue));
                    for (int x = 0; x < matches.Length; x++)
                    {
                        dic.Add(string.Format("{0}{1}", "Descripcion", x + 1), matches[x].ToString());
                    }

                    var lst = Negocio.General.ExecuteProcedure<pauParteBusquedaAvanzadaEnTraspasos_Result>("pauParteBusquedaAvanzadaEnTraspasos", dic);
                    if (lst != null)
                    {
                        this.dgvDatos.DataSource = null;
                        this.dgvDatos.Columns.Clear();
                        this.dgvDatos.DataSource = Helper.LinqToDataTable<pauParteBusquedaAvanzadaEnTraspasos_Result>(lst);
                        this.lblEncontrados.Text = string.Format("Encontrados: {0}", lst.Count);
                        Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "MarcaParteID", "LineaID", "SistemaID", "SubsistemaID", 
                        "SucursalID", "Maximo", "Minimo", "SucursalDestinoID", "DestinoExistencia", "DestinoMaximo", "DestinoMinimo" });
                        Helper.ColumnasToHeaderText(this.dgvDatos);
                        if (this.dgvDatos.Columns.Contains("NombreParte"))
                            this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";
                        if (!this.dgvDatos.Columns.Contains("X"))
                        {
                            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                            checkColumn.Name = "X";
                            checkColumn.HeaderText = "";
                            checkColumn.Width = 50;
                            checkColumn.ReadOnly = false;
                            checkColumn.FillWeight = 10;
                            this.dgvDatos.Columns.Add(checkColumn);
                            this.dgvDatos.Columns["X"].DisplayIndex = 0;
                        }

                        foreach (DataGridViewColumn column in this.dgvDatos.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.Automatic;
                            if (!column.Name.Equals("X"))
                            {
                                column.ReadOnly = true;
                            }
                        }
                        this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    }
                    else
                    {
                        lblEncontrados.Text = "Encontrados:";
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                try
                {
                    int iParteID = 0;
                    if (this.dgvDatos.Rows.Count == 0)
                    {
                        UtilLocal.MensajeAdvertencia("No se encontró ninguna parte con el código seleccionado.");
                        return;
                    }
                    else if (this.dgvDatos.Rows.Count == 1)
                    {
                        iParteID = Helper.ConvertirEntero(this.dgvDatos["ParteID", 0].Value);
                    }
                    else
                    {
                        var frmLista = new ObtenerElementoLista("Selecciona la parte que deseas agregar:", this.dgvDatos.DataSource);
                        frmLista.Text = "Número de parte o código repetido";
                        frmLista.MostrarColumnas("NumeroParte", "NombreParte");
                        frmLista.dgvDatos.Columns["NombreParte"].DisplayIndex = 0;
                        frmLista.dgvDatos.Columns["NumeroParte"].DisplayIndex = 1;
                        frmLista.ShowDialog(Principal.Instance);
                        if (frmLista.DialogResult == DialogResult.OK)
                        {
                            iParteID = (frmLista.Seleccion as pauParteBusquedaAvanzadaEnTraspasos_Result).ParteID;
                        }
                        frmLista.Dispose();
                    }

                    //se manda a la derecha.
                    var rowIndex = this.dgvDatos.EncontrarIndiceDeValor("ParteID", iParteID);
                    if ((decimal)this.dgvDatos.Rows[rowIndex].Cells["Existencia"].Value <= 0)
                    {
                        Helper.MensajeError("No hay suficientes existencias del articulo", "Error");
                        return;
                    }
                    // Se verifica si ya hay un traspaso para la parte indicada
                    int iSucDestID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue);
                    if (General.Exists<MovimientoInventarioDetalleView>(c => c.TipoOperacionID == Cat.TiposDeOperacionMovimientos.Traspaso && c.SucursalDestinoID == iSucDestID
                        && !c.FechaRecepcion.HasValue && c.ParteID == iParteID))
                    {
                        if (UtilLocal.MensajePregunta("El artículo seleccionado ya tiene un traspaso pendiente. ¿Aún así deseas continuar?") != DialogResult.Yes)
                            return;
                    }

                    this.dgvDatos.CurrentCell = this.dgvDatos.Rows[rowIndex].Cells["X"];
                    dgvDatos.Rows[rowIndex].Cells["X"].Value = true;
                    this.dgvDatos_CurrentCellDirtyStateChanged(sender, null);

                    this.dgvTraspasos.Select();
                    rowIndex = this.dgvTraspasos.EncontrarIndiceDeValor("ParteID", iParteID);
                    this.dgvTraspasos.CurrentCell = this.dgvTraspasos.Rows[rowIndex].Cells["Cantidad"];
                    this.dgvTraspasos.BeginEdit(true);
                    //this.txtCodigo.Clear();
                }
                catch (Exception ex) {
                    UtilLocal.MensajeError(ex.Message);
                }
            }
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtCodigo.Clear();
                this.txtDescripcion.Clear();
                this.LimpiarFormulario();
            }
           
        }

        private void txtDescripcion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtCodigo.Clear();
                this.txtDescripcion.Clear();
                this.LimpiarFormulario();
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvDatos.Focus();
            }
        }

        private void txtCodigo_Enter(object sender, EventArgs e)
        {
            if (this.cboUbicacionDestino.SelectedIndex == -1)
            {
                Helper.MensajeError("Elige primero un destino", "Error");
                this.cboUbicacionDestino.Select();
            }
        }

        private void dgvTraspasos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.txtCodigo.Select();
            this.txtCodigo.SelectAll();
        }

        private void dgvDatos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (null == this.dgvDatos.CurrentRow)
                return;
            try
            {
                if (this.dgvDatos.IsCurrentCellDirty)
                {
                    this.dgvDatos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    dgvDatos.EndEdit();
                    return;
                }

                var cb = (DataGridViewCheckBoxCell)dgvDatos.CurrentRow.Cells["X"];
                if (cb.Value != null)
                {
                    if (Helper.ConvertirBool(cb.Value))
                    {
                        int parteId = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ParteID"].Value);
                        string numeroParte = Helper.ConvertirCadena(this.dgvDatos.CurrentRow.Cells["NumeroParte"].Value);
                        string descripcion = Helper.ConvertirCadena(this.dgvDatos.CurrentRow.Cells["NombreParte"].Value);

                        var existencia = Helper.ConvertirDecimal(this.dgvDatos.CurrentRow.Cells["Existencia"].Value);
                        var sugerencia = Helper.ConvertirDecimal(this.dgvDatos.CurrentRow.Cells["Sugerencia"].Value);

                        if (sugerencia > existencia)
                            sugerencia = existencia;

                        if (this.validaExistencia(parteId, 1))
                        {
                            var rowIndex = Helper.findRowIndex(this.dgvTraspasos, "ParteID", parteId.ToString());
                            if (rowIndex >= 0) return;

                            var parte = new PartesView()
                            {
                                ParteID = parteId,
                                NumeroDeParte = numeroParte,
                                Descripcion = descripcion,
                                Marca = "",
                                Linea = "",
                                ParteEstatusID = 1,
                                LineaID = 0,
                                Busqueda = sugerencia.ToString()
                            };
                            
                            // Se verifica si ya hay un traspaso para la parte indicada
                            int iSucDestID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue);
                            if (General.Exists<MovimientoInventarioDetalleView>(c => c.TipoOperacionID == Cat.TiposDeOperacionMovimientos.Traspaso && c.SucursalDestinoID == iSucDestID
                                && !c.FechaRecepcion.HasValue && c.ParteID == parteId))
                            {
                                if (UtilLocal.MensajePregunta("El artículo seleccionado ya tiene un traspaso pendiente. ¿Aún así deseas continuar?") != DialogResult.Yes)
                                    return;
                            }

                            this.agregarDetalleTraspasos(parte);
                        }
                        else
                        {
                            Helper.MensajeError("No hay existencia suficiente para realizar la operación", GlobalClass.NombreApp);
                            cb.Value = cb.FalseValue;
                        }
                    }
                }
                //dgvDatos.EndEdit();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnNinguno_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                foreach (DataGridViewRow row in this.dgvDatos.Rows)
                {
                    DataGridViewCheckBoxCell cb = (DataGridViewCheckBoxCell)row.Cells["X"];
                    if (cb.Value == null)
                    {
                        if (Helper.ConvertirDecimal(row.Cells["Existencia"].Value) > 0)
                        {
                            cb.Value = true;
                            //
                            int parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            string numeroParte = Helper.ConvertirCadena(row.Cells["NumeroParte"].Value);
                            string descripcion = Helper.ConvertirCadena(row.Cells["NombreParte"].Value);

                            var existencia = Helper.ConvertirDecimal(row.Cells["Existencia"].Value);
                            var sugerencia = Helper.ConvertirDecimal(row.Cells["Sugerencia"].Value);

                            if (sugerencia > existencia)
                                sugerencia = existencia;

                            if (this.validaExistencia(parteId, 1))
                            {
                                var rowIndex = Helper.findRowIndex(this.dgvTraspasos, "ParteID", parteId.ToString());
                                if (rowIndex >= 0) return;

                                var parte = new PartesView()
                                {
                                    ParteID = parteId,
                                    NumeroDeParte = numeroParte,
                                    Descripcion = descripcion,
                                    Marca = "",
                                    Linea = "",
                                    ParteEstatusID = 1,
                                    LineaID = 0,
                                    Busqueda = sugerencia.ToString()
                                };

                                this.agregarDetalleTraspasos(parte);
                            }
                            //
                        }
                        else
                        {
                            cb.Value = false;
                        }
                    }
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                int parteId = Helper.ConvertirEntero(this.dgvDatos.Rows[e.RowIndex].Cells["ParteID"].Value);
                this.CargaExistencias(parteId);
            }
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                var dgv = (DataGridView)sender;
                var fila = dgv.SelectedRows[0].Index;
                if (Convert.ToBoolean(dgv.Rows[fila].Cells["X"].Value).Equals(true))
                {
                    dgv.Rows[fila].Cells["X"].Value = false;
                }
                else
                {
                    dgv.Rows[fila].Cells["X"].Value = true;
                    this.dgvDatos_CurrentCellDirtyStateChanged(sender, null);
                }
            }
        }

        private void dgvDatos_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvDatos_CellClick(sender, new DataGridViewCellEventArgs(0, this.dgvDatos.CurrentRow.Index));
            }
        }

        private void dgvTraspasos_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (this.dgvTraspasos.Columns[e.ColumnIndex].Name == "Cantidad")
                {
                    int parteid=Helper.ConvertirEntero(this.dgvTraspasos.Rows[e.RowIndex].Cells["ParteID"].Value);
                    var oParte= General.GetEntity<Parte>(p=>p.ParteID==parteid );
                    decimal value;
                    if (!decimal.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        this.dgvTraspasos.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                    else
                    {   
                        if (value <= 0)
                        {
                            this.dgvTraspasos.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida mayor a 0.";
                            e.Cancel = true;
                        }
                        else if(!oParte.AGranel && (value-Math.Round(value))!=0){
                            this.dgvTraspasos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Math.Round(value);
                            this.dgvTraspasos.Rows[e.RowIndex].ErrorText = null;
                        }
                        else
                        {
                            this.dgvTraspasos.Rows[e.RowIndex].ErrorText = null;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvTraspasos_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            this.dgvTraspasos.Rows[e.RowIndex].Cells["X"].Value = true;
        }

        private void dgvTraspasos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvTraspasos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                var res = Negocio.Helper.MensajePregunta("¿Está seguro de que desea eliminar esté articulo?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        if (null != this.dgvTraspasos.CurrentRow)
                            this.dgvTraspasos.Rows.RemoveAt(this.dgvTraspasos.CurrentRow.Index);
                    }
                    catch (Exception ex)
                    {
                        Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            try
            {
                int iAutorizoID = 0;
                var ResU = UtilDatos.ValidarObtenerUsuario(null, "Autorización");
                if (ResU.Exito)
                    iAutorizoID = ResU.Respuesta.UsuarioID;
                else
                {
                    Helper.MensajeError("Error al validar el usuario.", GlobalClass.NombreApp);
                    return;
                }

                var res = Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                if (this.dgvTraspasos.Rows.Count <= 0)
                {
                    Helper.MensajeError("Debe seleccionar al menos un articulo.", GlobalClass.NombreApp);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                SplashScreen.Show(new Splash());
                this.btnProcesar.Enabled = false;

                var sucursalId = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue);
                decimal unidades = 0;
                //Validar que la cantidad de cualquier fila sea mayor a 0
                foreach (DataGridViewRow row in this.dgvTraspasos.Rows)
                {
                    var cantidad = Helper.ConvertirDecimal(row.Cells["Cantidad"].Value);
                    if (cantidad <= 0)
                    {
                        Helper.MensajeError(string.Format("{0} {1} {2}", "No puede traspasar el Número de Parte:", Helper.ConvertirCadena(row.Cells["Numero Parte"].Value), "con cantidad en 0."), GlobalClass.NombreApp);
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }

                //Validar existencias de cada articulo en la sucursal origen
                foreach (DataGridViewRow row in this.dgvTraspasos.Rows)
                {
                    if (Helper.ConvertirBool(row.Cells["X"].Value).Equals(true))
                    {
                        var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                        var cantidad = Helper.ConvertirDecimal(row.Cells["Cantidad"].Value);
                        unidades += cantidad;
                        var parteExistencia = General.GetEntity<ParteExistencia>(p => p.ParteID.Equals(parteId) && p.SucursalID.Equals(sucursalId) && p.Estatus);
                        if (null != parteExistencia)
                        {
                            if (parteExistencia.Existencia <= 0 || cantidad > parteExistencia.Existencia)
                            {
                                Helper.MensajeError(string.Format("{0}{1}", row.Cells["Numero Parte"].Value, " No cuenta con la existencia suficiente en este momento."), GlobalClass.NombreApp);
                                this.Cursor = Cursors.Default;
                                SplashScreen.Close();
                                this.btnProcesar.Enabled = true;
                                this.dgvTraspasos.Select();
                                row.Cells["Cantidad"].Selected = true;
                                return;
                            }
                        }
                    }
                }

                //Almacenar traspaso
                var traspaso = new MovimientoInventario()
                {
                    TipoOperacionID = 5,
                    SucursalOrigenID = sucursalId,
                    SucursalDestinoID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue),
                    Unidades = unidades,
                    ImporteTotal = 0,
                    FueLiquidado = false,
                    UsuarioSolicitoTraspasoID = Helper.ConvertirEntero(this.cboSolicito.SelectedValue),
                    ExisteContingencia = false,
                    UsuarioID = iAutorizoID,
                    FechaRegistro = DateTime.Now,
                    Estatus = true,
                    Actualizar = true
                };
                General.SaveOrUpdate<MovimientoInventario>(traspaso, traspaso);

                //Almacenar el detalle del traspaso
                if (traspaso.MovimientoInventarioID > 0)
                {
                    decimal mCostoTotal = 0;
                    foreach (DataGridViewRow row in this.dgvTraspasos.Rows)
                    {
                        if (Helper.ConvertirBool(row.Cells["X"].Value).Equals(true))
                        {
                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var cantidad = Helper.ConvertirDecimal(row.Cells["Cantidad"].Value);
                            var detalleTraspaso = new MovimientoInventarioDetalle()
                            {
                                MovimientoInventarioID = traspaso.MovimientoInventarioID,
                                ParteID = parteId,
                                Cantidad = cantidad,
                                PrecioUnitario = 0,
                                Importe = 0,
                                FueDevolucion = false,
                                FechaRegistro = DateTime.Now,
                                Estatus = true
                            };
                            General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleTraspaso, detalleTraspaso);

                            //Descontar la existencia actual de la sucursal origen
                            var oParte = General.GetEntity<Parte>(c => c.ParteID == parteId && c.Estatus);
                            if (!oParte.EsServicio.Valor())
                            {
                                var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId);
                                if (existencia != null)
                                {
                                    var inicial = existencia.Existencia;
                                    existencia.Existencia -= cantidad;
                                    existencia.UsuarioID = iAutorizoID;
                                    existencia.FechaModificacion = DateTime.Now;
                                    General.SaveOrUpdate<ParteExistencia>(existencia, existencia);

                                    var historial = new MovimientoInventarioHistorial()
                                    {
                                        MovmientoInventarioID = traspaso.MovimientoInventarioID,
                                        ParteID = parteId,
                                        ExistenciaInicial = Helper.ConvertirDecimal(inicial),
                                        ExistenciaFinal = Helper.ConvertirDecimal(existencia.Existencia),
                                        SucursalID = sucursalId,
                                        UsuarioID = iAutorizoID,
                                        FechaRegistro = DateTime.Now,
                                        Estatus = true,
                                        Actualizar = true
                                    };
                                    General.SaveOrUpdate<MovimientoInventarioHistorial>(historial, historial);
                                }
                            }

                            // Se agrega al Kardex
                            var oPartePrecio = General.GetEntity<PartePrecio>(c => c.ParteID == parteId && c.Estatus);
                            AdmonProc.RegistrarKardex(new ParteKardex()
                            {
                                ParteID = parteId,
                                OperacionID = Cat.OperacionesKardex.SalidaTraspaso,
                                SucursalID = sucursalId,
                                Folio = traspaso.MovimientoInventarioID.ToString(),
                                Fecha = DateTime.Now,
                                RealizoUsuarioID = iAutorizoID,
                                Entidad = this.cboProveedor.Text,
                                Origen = this.cboUbicacionOrigen.Text,
                                Destino = this.cboUbicacionDestino.Text,
                                Cantidad = cantidad,
                                Importe = oPartePrecio.Costo.Valor()
                            });

                            // Se suma el importe de cada parte, para crear la póliza
                            mCostoTotal += oPartePrecio.Costo.Valor();
                        }
                    }

                    // Se genera la póliza especial correspondiente (AfeConta)
                    var oPoliza = ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, string.Format("TRASPASO ORIGEN {0:00} DESTINO {1:00}",
                        this.cboUbicacionOrigen.SelectedValue, this.cboUbicacionDestino.SelectedValue), Cat.ContaCuentasAuxiliares.Inventario
                        , Cat.ContaCuentasAuxiliares.Inventario, mCostoTotal, ResU.Respuesta.NombreUsuario, Cat.Tablas.MovimientoInventario, traspaso.MovimientoInventarioID
                        , Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue));
                    var oCuentaQuitar = General.GetEntity<ContaPolizaDetalle>(c => c.ContaPolizaID == oPoliza.ContaPolizaID && c.Cargo > 0);
                    oCuentaQuitar.Cargo = 0;
                    Guardar.Generico<ContaPolizaDetalle>(oCuentaQuitar);
                }

                //Visor de ticket de traspaso
                ReporteadorMovimientos visor = ReporteadorMovimientos.Instance;
                visor.oID = traspaso.MovimientoInventarioID;
                visor.oTipoReporte = 7;
                visor.Load();

                this.LimpiarFormulario();
                this.Cursor = Cursors.Default;
                SplashScreen.Close();
                this.btnProcesar.Enabled = true;
                new Notificacion("Traspaso Guardado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                SplashScreen.Close();
                this.btnProcesar.Enabled = true;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Metodos ]

        private void agregarDetalleTraspasos(PartesView parte)
        {
            DataRow row;
            try
            {
                if (parte != null)
                {
                    // Se valida si ya existe un traspaso de la misma parte, para mostrar la advertencia
                    if (General.Exists<MovimientoInventarioDetalleView>(c => c.TipoOperacionID == Cat.TiposDeOperacionMovimientos.Traspaso && c.ParteID == parte.ParteID
                        && !c.FechaRecepcion.HasValue && c.SucursalDestinoID == GlobalClass.SucursalID))
                    {
                        if (UtilLocal.MensajePregunta("Ya existe un Traspaso en curso para esta Parte. ¿Aún así deseas continuar?") != DialogResult.Yes)
                            return;
                    }
                    
                    var proveedorid = General.GetEntity<Parte>(p => p.ParteID == parte.ParteID);
                    var proveedor = General.GetEntity<Proveedor>(pr=>pr.ProveedorID==proveedorid.ProveedorID);
                    row = dtDetalleTraspasos.NewRow();
                    row["ParteID"] = parte.ParteID;
                    row["Numero Parte"] = parte.NumeroDeParte;
                    row["Descripcion"] = parte.Descripcion;
                    row["Cantidad"] = parte.Busqueda;
                    row["Proveedor"] = proveedor.NombreProveedor;
                    dtDetalleTraspasos.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void configurarGridTraspasos()
        {
            try
            {
                dtDetalleTraspasos.Clear();
                this.dgvTraspasos.Refresh();
                if (this.dgvTraspasos.RowCount > 0)
                {
                    if (this.dgvTraspasos.Columns.Count > 0)
                        this.dgvTraspasos.Columns.Clear();

                    if (this.dgvTraspasos.Rows.Count > 0)
                        this.dgvTraspasos.Rows.Clear();
                }

                this.dgvTraspasos.DataSource = null;

                dtDetalleTraspasos = new DataTable();

                var colParteId = new DataColumn();
                colParteId.DataType = System.Type.GetType("System.Int32");
                colParteId.ColumnName = "ParteID";

                var colNumeroParte = new DataColumn();
                colNumeroParte.DataType = Type.GetType("System.String");
                colNumeroParte.ColumnName = "Numero Parte";

                var colDescripcion = new DataColumn();
                colDescripcion.DataType = Type.GetType("System.String");
                colDescripcion.ColumnName = "Descripcion";

                var colCantidad = new DataColumn();
                colCantidad.DataType = Type.GetType("System.Decimal");
                colCantidad.ColumnName = "Cantidad";

                var colProveedor = new DataColumn();
                colProveedor.DataType = Type.GetType("System.String");
                colProveedor.ColumnName = "Proveedor";

                dtDetalleTraspasos.Columns.AddRange(new DataColumn[] { colParteId, colNumeroParte, colDescripcion, colCantidad, colProveedor });

                this.dgvTraspasos.DataSource = dtDetalleTraspasos;
                Negocio.Helper.OcultarColumnas(this.dgvTraspasos, new string[] { "ParteID" });

                if (!this.dgvTraspasos.Columns.Contains("X"))
                {
                    DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                    checkColumn.Name = "X";
                    checkColumn.HeaderText = "";
                    checkColumn.Width = 50;
                    checkColumn.ReadOnly = false;
                    checkColumn.FillWeight = 10;
                    this.dgvTraspasos.Columns.Add(checkColumn);
                    this.dgvTraspasos.Columns["X"].DisplayIndex = 0;
                }

                foreach (DataGridViewColumn column in this.dgvTraspasos.Columns)
                {
                    if (column.Name.Equals("Descripcion") || column.Name.Equals("Numero Parte"))
                    {
                        column.ReadOnly = true;
                    }
                    if (column.Name.Equals("Cantidad"))
                    {
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                    }
                    if (!column.Name.Equals("X"))
                    {                        
                        dgvDatos.AutoResizeColumn(column.Index, DataGridViewAutoSizeColumnMode.ColumnHeader);
                    }
                }

                //this.dgvTraspasos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            catch (Exception ex)
            {

            }
        }

        private void CargarLineas(int marcaId)
        {
            try
            {
                if (marcaId > 0)
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
                else
                {
                    var listaLineas = Negocio.General.GetListOf<LineaMarcaPartesView>(l => l.MarcaParteID.Equals(marcaId));
                    listaLineas.Insert(0, new LineaMarcaPartesView() { LineaMarcaParteID = 0, LineaID = 0, NombreLinea = "TODOS", MarcaParteID = 0, LineaMarca = "TODOS" });
                    cboLinea.DataSource = listaLineas;
                    cboLinea.DisplayMember = "NombreLinea";
                    cboLinea.ValueMember = "LineaID";
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
                
        private void CargaExistencias(int parteId)
        {
            try
            {
                this.dgvExistencias.DataSource = null;
                if (dgvExistencias.Columns.Count > 0)
                    dgvExistencias.Columns.Clear();

                if (dgvExistencias.Rows.Count > 0)
                    dgvExistencias.Rows.Clear();

                this.dgvExistencias.DataSource = General.GetListOf<ExistenciasView>(ex => ex.ParteID.Equals(parteId));
                this.dgvExistencias.AutoResizeColumns();
                Negocio.Helper.OcultarColumnas(this.dgvExistencias, new string[] { "ParteExistenciaID", "ParteID", "NumeroParte", "SucursalID" });
                Negocio.Helper.ColumnasToHeaderText(this.dgvExistencias);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool validaExistencia(int parteId, int cantidad)
        {
            bool existe = false;
            try
            {
                var sucursalId = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue);
                var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID.Equals(parteId) && p.SucursalID.Equals(sucursalId));
                if (existencia != null)
                {
                    if (existencia.Existencia >= cantidad)
                    {
                        existe = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            return existe;
        }

        #endregion

        #endregion

        #region [ Recibir Traspaso ]

        private void txtNumeroDeCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.TraspasoDeEntradaCompra(Helper.ConvertirEntero(this.txtNumeroDeCompra.Text));
            }
        }

        private void TraspasoDeEntradaCompra(int iEntradaCompraID)
        {
            this.dgvDatos.DataSource = null;
            this.dgvDatos.Columns.Clear();
            if (iEntradaCompraID <= 0) return;

            int iSucursalOriID = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue);
            int iSucursalDesID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue);
            if (iSucursalOriID <= 0 || iSucursalDesID <= 0)
            {
                UtilLocal.MensajeAdvertencia("Debes seleccionar las sucursales de Origen y Destino.");
                return;
            }

            var oCompra = General.GetEntity<MovimientoInventarioView>(c => c.MovimientoInventarioID == iEntradaCompraID);
            if (oCompra.TipoOperacionID != Cat.TiposDeOperacionMovimientos.EntradaCompra)
            {
                UtilLocal.MensajeAdvertencia("El folio especificado no pertenece a una Compra. No se puede continuar.");
                return;
            }
            var oCompraDet = General.GetListOf<MovimientoInventarioDetalleView>(c => c.MovimientoInventarioID == iEntradaCompraID);
            
            // Se agregan las columnas correspondientes
            this.dgvDatos.AgregarColumna("ParteID", "ParteID", 40).Visible = false;
            this.dgvDatos.AgregarColumnaCheck("X", "", 20);
            this.dgvDatos.AgregarColumna("NumeroParte", "No. de Parte", 100);
            this.dgvDatos.AgregarColumna("NombreParte", "Descripción", 200);
            this.dgvDatos.AgregarColumna("Proveedor", "Proveedor", 50);
            this.dgvDatos.AgregarColumna("Existencia", "Existencia", 50);
            this.dgvDatos.AgregarColumna("Sugerencia", "Sugerencia", 50);

            int iFila;
            foreach (var oReg in oCompraDet)
            {
                var oSugerir = General.GetEntity<PartesExistenciasMaxMinView>(c => c.ParteID == oReg.ParteID && c.SucursalID == iSucursalDesID);
                if ((oSugerir.Maximo - oSugerir.Existencia) > 0 && (oSugerir.Existencia <= oSugerir.Minimo))
                {
                    var oSugerirOrigen = General.GetEntity<ParteExistencia>(c => c.ParteID == oReg.ParteID && c.SucursalID == iSucursalOriID && c.Estatus);
                    iFila = this.dgvDatos.Rows.Add(oReg.ParteID, false, oReg.NumeroParte, oReg.NombreParte, oCompra.NombreProveedor
                        , oSugerirOrigen.Existencia, (oSugerir.Maximo - oSugerir.Existencia));
                    this.dgvDatos.CurrentCell = this.dgvDatos["X", iFila];

                    // Se marca la filas y se pasan al grid de la derecha
                    this.dgvDatos["X", iFila].Value = true;
                    this.dgvDatos_CurrentCellDirtyStateChanged(this.txtNumeroDeCompra, new EventArgs());
                }
            }
        }

        private void btnMostrarRecibir_Click(object sender, EventArgs e)
        {
            this.mostrarTraspasosArecibir(new DateTime(2000, 1, 1), new DateTime(DateTime.Now.Year + 5, 12, 31)); 
        }

        private void dgvRecibir_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvRecibir.CurrentRow == null)
                return;
            var movimientoId = Negocio.Helper.ConvertirEntero(this.dgvRecibir.CurrentRow.Cells["MovimientoInventarioID"].Value);
            if (movimientoId > 0)
            {
                this.CargarDetalleTraspaso(movimientoId);
            }
        }

        private void dgvRecibir_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvRecibir_CellClick(sender, new DataGridViewCellEventArgs(0, this.dgvRecibir.CurrentRow.Index));
            }
        }

        private void dgvRecibirDetalle_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                if (this.dgvRecibirDetalle.Columns[e.ColumnIndex].Name == "Recibido")
                {
                    decimal value;
                    if (!decimal.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        this.dgvRecibirDetalle.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                    else
                    {
                        if (value < 0)
                        {
                            this.dgvRecibirDetalle.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida mayor a 0.";
                            e.Cancel = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvRecibirDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;
                if (this.dgvRecibirDetalle.Columns[e.ColumnIndex].Name == "Recibido")
                {
                    var enviado = Helper.ConvertirDecimal(this.dgvRecibirDetalle.Rows[e.RowIndex].Cells["Cantidad"].Value);
                    var recibido = Helper.ConvertirDecimal(this.dgvRecibirDetalle.Rows[e.RowIndex].Cells["Recibido"].Value);
                    if (recibido < enviado)
                    {
                        this.dgvRecibirDetalle.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                        this.dgvRecibirDetalle.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        this.dgvRecibirDetalle.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(188, 199, 216);
                        this.dgvRecibirDetalle.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                    }
                    //Cambiar de foco a la siguiente celda hacia abajo
                    if (null != this.dgvRecibirDetalle.CurrentRow)
                    {
                        var fila = this.dgvRecibirDetalle.CurrentRow.Index + 1;
                        if (fila < this.dgvRecibirDetalle.Rows.Count)
                        {
                            //this.dgvRecibirDetalle.CurrentCell = this.dgvRecibirDetalle.Rows[fila].Cells[e.ColumnIndex];
                            this.dgvRecibirDetalle.BeginEdit(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnNingunoRecibir_Click(object sender, EventArgs e)
        {
            if (this.dgvRecibirDetalle.Columns.Contains("X"))
            {
                foreach (DataGridViewRow row in this.dgvRecibirDetalle.Rows)
                    row.Cells["X"].Value = false;
            }
        }

        private void btnProcesarRecibir_Click(object sender, EventArgs e)
        {
            var log = new Log();
            try
            {
                int iAutorizoID = 0;
                var ResU = UtilDatos.ValidarObtenerUsuario(null, "Autorización");
                if (ResU.Exito)
                    iAutorizoID = ResU.Respuesta.UsuarioID;
                else
                {
                    Helper.MensajeError("Error al validar el usuario.", GlobalClass.NombreApp);
                    return;
                }

                bool existeContingencia = false;

                var res = Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                if (this.dgvRecibir.SelectedRows.Count <= 0)
                {
                    Helper.MensajeError("Debe seleccionar al menos un traspaso.", GlobalClass.NombreApp);
                    return;
                }

                //Validar que lo recibido sea menor o igual que lo enviado
                foreach (DataGridViewRow row in this.dgvRecibirDetalle.Rows)
                {
                    var enviado = Helper.ConvertirDecimal(row.Cells["Cantidad"].Value);
                    var recibido = Helper.ConvertirDecimal(row.Cells["Recibido"].Value);
                    if (recibido > enviado)
                    {
                        Helper.MensajeError(string.Format("{0}{1}{2}", "El articulo ", row.Cells["NumeroParte"].Value, " No tiene una cantidad valida en la columna Recibido."), GlobalClass.NombreApp);
                        return;
                    }
                }

                //SplashScreen.Show(new Splash());
                log.Text = "Ingresando Traspaso, espere un momento...";
                log.Show();
                this.btnProcesarRecibir.Enabled = false;

                foreach (DataGridViewRow row in this.dgvRecibirDetalle.Rows)
                {
                    var enviado = Helper.ConvertirDecimal(row.Cells["Cantidad"].Value);
                    var recibido = Helper.ConvertirDecimal(row.Cells["Recibido"].Value);
                    if (recibido < enviado)
                    {
                        existeContingencia = true;
                    }
                }

                if (existeContingencia)
                {
                    var resp = Helper.MensajePregunta("El Traspaso tiene uno o mas conflictos. ¿Está seguro de continuar y recibir esté Traspaso?", GlobalClass.NombreApp);
                    if (resp == DialogResult.No)
                        return;
                }

                if (!this.ValidacionesAlRecibir(existeContingencia))
                    return;

                var movimientoId = Helper.ConvertirEntero(this.dgvRecibir.Rows[this.dgvRecibir.SelectedRows[0].Index].Cells["MovimientoInventarioID"].Value);
                if (movimientoId <= 0)
                {
                    Helper.MensajeError("Ocurrio un error al intentar recibir el traspaso.", GlobalClass.NombreApp);
                    return;
                }
                this.Cursor = Cursors.WaitCursor;

                // Se obtienen la Sucursal Origen y Destino para el traspaso seleccionado
                string sOrigen = Helper.ConvertirCadena(this.dgvRecibir.CurrentRow.Cells["Origen"].Value);
                string sDestino = Helper.ConvertirCadena(this.dgvRecibir.CurrentRow.Cells["Destino"].Value);

                //
                decimal mCostoTotal = 0;
                foreach (DataGridViewRow row in this.dgvRecibirDetalle.Rows)
                {
                    var enviado = Helper.ConvertirDecimal(row.Cells["Cantidad"].Value);
                    var recibido = Helper.ConvertirDecimal(row.Cells["Recibido"].Value);
                    var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                    var numeroParte = Helper.ConvertirCadena(row.Cells["NumeroParte"].Value);
                    // Si la cantidad recibida es menor a lo enviado, almacenar la contingencia
                    if (recibido < enviado)
                    {
                        var contingencia = new MovimientoInventarioTraspasoContingencia()
                        {
                            MovimientoInventarioID = movimientoId,
                            MovimientoInventarioDetalleID = Helper.ConvertirEntero(row.Cells["MovimientoInventarioDetalleID"].Value),
                            ParteID = parteId,
                            CantidadEnviada = enviado,
                            CantidadRecibida = recibido,
                            CantidadDiferencia = enviado - recibido,
                            Comentario = this.txtMotivo.Text,
                            UsuarioID = iAutorizoID,
                            FechaRegistro = DateTime.Now,
                            MovimientoInventarioEstatusContingenciaID = 2,
                            Estatus = true,
                            Actualizar = true
                        };
                        log.AppendTextBox("Almacenando Contingencia...");
                        General.SaveOrUpdate<MovimientoInventarioTraspasoContingencia>(contingencia, contingencia);
                    }

                    //Aumentar la existencia actual de la sucursal destino
                    var oParte = General.GetEntity<Parte>(c => c.ParteID == parteId && c.Estatus);
                    if (!oParte.EsServicio.Valor())
                    {
                        var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == GlobalClass.SucursalID);
                        if (existencia != null)
                        {
                            var inicial = existencia.Existencia;
                            existencia.Existencia += recibido;
                            existencia.UsuarioID = iAutorizoID;
                            existencia.FechaModificacion = DateTime.Now;
                            log.AppendTextBox(string.Format("{0} {1}", "Actualizando existencia de:", numeroParte));
                            General.SaveOrUpdate<ParteExistencia>(existencia, existencia);

                            var historial = new MovimientoInventarioHistorial()
                            {
                                MovmientoInventarioID = movimientoId,
                                ParteID = parteId,
                                ExistenciaInicial = Helper.ConvertirDecimal(inicial),
                                ExistenciaFinal = Helper.ConvertirDecimal(existencia.Existencia),
                                SucursalID = GlobalClass.SucursalID,
                                UsuarioID = iAutorizoID,
                                FechaRegistro = DateTime.Now,
                                Estatus = true,
                                Actualizar = true
                            };
                            log.AppendTextBox("Almacenando en Historial...");
                            General.SaveOrUpdate<MovimientoInventarioHistorial>(historial, historial);
                        }
                    }

                    // Se agrega al Kardex
                    var oPartePrecio = General.GetEntity<PartePrecio>(c => c.ParteID == parteId && c.Estatus);
                    AdmonProc.RegistrarKardex(new ParteKardex()
                    {
                        ParteID = parteId,
                        OperacionID = Cat.OperacionesKardex.EntradaTraspaso,
                        SucursalID = GlobalClass.SucursalID,
                        Folio = movimientoId.ToString(),
                        Fecha = DateTime.Now,
                        RealizoUsuarioID = iAutorizoID,
                        Entidad = this.cboProveedor.Text,
                        Origen = sOrigen,
                        Destino = sDestino,
                        Cantidad = recibido,
                        Importe = oPartePrecio.Costo.Valor()
                    });

                    // Se suma el importe de cada parte, para crear la póliza
                    mCostoTotal += oPartePrecio.Costo.Valor();
                }

                // Se genera la póliza especial correspondiente (AfeConta)
                int iSucursalOrigenID = Helper.ConvertirEntero(this.dgvRecibir.CurrentRow.Cells["SucursalOrigenID"].Value);
                int iSucursalDestinoID = Helper.ConvertirEntero(this.dgvRecibir.CurrentRow.Cells["SucursalDestinoID"].Value);
                var oPoliza = ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, string.Format("TRASPASO ORIGEN {0:00} DESTINO {1:00}", iSucursalOrigenID, iSucursalDestinoID)
                    , Cat.ContaCuentasAuxiliares.Inventario, Cat.ContaCuentasAuxiliares.Inventario, mCostoTotal, ResU.Respuesta.NombreUsuario
                    , Cat.Tablas.MovimientoInventario, movimientoId, iSucursalDestinoID);
                var oCuentaQuitar = General.GetEntity<ContaPolizaDetalle>(c => c.ContaPolizaID == oPoliza.ContaPolizaID && c.Abono > 0);
                oCuentaQuitar.Abono = 0;
                Guardar.Generico<ContaPolizaDetalle>(oCuentaQuitar);

                //Actualizar el movimiento con los datos (fecha y usuario que recibio)
                var movimiento = General.GetEntity<MovimientoInventario>(m => m.MovimientoInventarioID == movimientoId);
                if (null != movimiento)
                {
                    movimiento.ExisteContingencia = existeContingencia;
                    movimiento.UsuarioRecibioTraspasoID = iAutorizoID;
                    movimiento.FechaRecepcion = DateTime.Now;
                    movimiento.FechaModificacion = DateTime.Now;
                    log.AppendTextBox("Finalizando...");
                    General.SaveOrUpdate<MovimientoInventario>(movimiento, movimiento);
                }

                this.LimpiarFormulario();
                this.Cursor = Cursors.Default;
                //SplashScreen.Close();
                log.finalizo = true;
                log.Close();
                this.btnProcesarRecibir.Enabled = true;
                new Notificacion("Traspaso Recibido exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                //SplashScreen.Close();
                log.finalizo = true;
                log.Close();
                this.btnProcesarRecibir.Enabled = true;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dtpFechaDesde_ValueChanged(object sender, EventArgs e)
        {
            this.mostrarTraspasosArecibir(this.dtpFechaDesde.Value, this.dtpFechaHasta.Value);
        }

        private void dtpFechaHasta_ValueChanged(object sender, EventArgs e)
        {
            this.mostrarTraspasosArecibir(this.dtpFechaDesde.Value, this.dtpFechaHasta.Value);
        }

        private void dtpFechaDesde_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.mostrarTraspasosArecibir(this.dtpFechaDesde.Value, this.dtpFechaHasta.Value);
        }

        private void dtpFechaHasta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.mostrarTraspasosArecibir(this.dtpFechaDesde.Value, this.dtpFechaHasta.Value);
        }

        private void CargarDetalleTraspaso(int movimientoInventarioId)
        {
            try
            {
                var detalle = General.GetListOf<MovimientoInventarioDetalleView>(m => m.MovimientoInventarioID.Equals(movimientoInventarioId)).ToList();
                SortableBindingList<MovimientoInventarioDetalleView> det = new SortableBindingList<MovimientoInventarioDetalleView>(detalle);
                this.dgvRecibirDetalle.DataSource = det;
                Helper.ColumnasToHeaderText(this.dgvRecibirDetalle);
                this.dgvRecibirDetalle.Columns["NombreParte"].HeaderText = "Descripcion";
                Helper.OcultarColumnas(this.dgvRecibirDetalle, new string[] { "MovimientoInventarioDetalleID", "MovimientoInventarioID", "ParteID", 
                    "FueDevolucion", "FechaRegistro", "PrecioUnitario", "Importe", "Cantidad", "CantidadRecibida", "TipoOperacionID", "FechaRecepcion" });

                if (!this.dgvRecibirDetalle.Columns.Contains("Recibido"))
                {
                    DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
                    textColumn.Name = "Recibido";
                    textColumn.HeaderText = "Recibido";
                    textColumn.Width = 50;
                    this.dgvRecibirDetalle.Columns.Add(textColumn);
                }
                else
                {
                    this.dgvRecibirDetalle.Columns["Recibido"].DisplayIndex = this.dgvRecibirDetalle.Columns.Count - 1;
                }

                foreach (DataGridViewColumn column in this.dgvRecibirDetalle.Columns)
                {
                    if (!column.Name.Equals("Recibido"))
                        column.ReadOnly = true;
                    if (column.Name.Equals("Cantidad") || column.Name.Equals("Recibido"))
                    {
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                    }
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                }

                foreach (DataGridViewRow row in this.dgvRecibirDetalle.Rows)
                {
                    row.Cells["Recibido"].Value = 0;
                }

                this.dgvRecibirDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool ValidacionesAlRecibir(bool existeContingencia)
        {
            this.cntError.LimpiarErrores();
            if (this.txtMotivo.Text == "" && existeContingencia)
                this.cntError.PonerError(this.txtMotivo, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        private void mostrarTraspasosArecibir(DateTime FechaInicial, DateTime FechaFinal)
        {
            this.LimpiarTabRecibir();
            try
            {
                //var desde = Helper.InicioAbsoluto(this.dtpFechaDesde.Value);
                //var hasta = Helper.FinAbsoluto(this.dtpFechaHasta.Value);

                //var traspasos = General.GetListOf<MovimientoInventarioTraspasosView>(t => t.TipoOperacionID == 5
                //    && t.SucursalDestinoID == GlobalClass.SucursalID
                //    && t.FechaRecepcion.Equals(null)
                //    && t.FechaRegistro >= desde && t.FechaRegistro <= hasta);

                FechaInicial = Helper.InicioAbsoluto(FechaInicial);
                FechaFinal = Helper.FinAbsoluto(FechaFinal);

                var traspasos = General.GetListOf<MovimientoInventarioTraspasosView>(t => t.TipoOperacionID == 5
                    && t.SucursalDestinoID == GlobalClass.SucursalID
                    && t.FechaRecepcion.Equals(null)
                    && t.FechaRegistro >= FechaInicial && t.FechaRegistro <= FechaFinal);

                if (traspasos != null)
                {
                    this.dgvRecibir.DataSource = null;
                    this.dgvRecibir.DataSource = traspasos;
                    this.lblEncontradosRecibir.Text = string.Format("Encontrados: {0}", traspasos.Count);
                    Helper.OcultarColumnas(this.dgvRecibir, new string[] { "TipoOperacionID", "SucursalOrigenID", "SucursalDestinoID"
                        , "FechaRecepcion", "Recibio", "TraspasoEntregado" });
                    Helper.ColumnasToHeaderText(this.dgvRecibir);
                    //this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";                    
                    this.dgvRecibir.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
                else
                {
                    lblEncontradosRecibir.Text = "Encontrados:";
                }

                this.ColorearMarcadosParaEntrega();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnMarcarEntrega_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();
            foreach (DataGridViewRow oFila in this.dgvRecibir.SelectedRows)
            {
                int iMovimientoID = Helper.ConvertirEntero(oFila.Cells["MovimientoInventarioID"].Value);
                this.MarcarEntrega(iMovimientoID);
            }
            this.mostrarTraspasosArecibir(this.dtpFechaDesde.Value, this.dtpFechaHasta.Value);
            Cargando.Cerrar();
            UtilLocal.MostrarNotificacion("Traspaso marcado correctamente.");
        }

        private void MarcarEntrega(int iMovimientoID)
        {
            var oTraspaso = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovimientoID && c.Estatus);
            oTraspaso.TraspasoEntregado = true;
            Guardar.Generico<MovimientoInventario>(oTraspaso);
        }

        private void ColorearMarcadosParaEntrega()
        {
            foreach (DataGridViewRow oFila in this.dgvRecibir.Rows)
            {
                if (Helper.ConvertirBool(oFila.Cells["TraspasoEntregado"].Value))
                {
                    oFila.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    if ((DateTime.Now - Helper.ConvertirFechaHora(oFila.Cells["FechaRegistro"].Value)).Days >= 5)
                    {
                        int iMovID = Helper.ConvertirEntero(oFila.Cells["MovimientoInventarioID"].Value);
                        this.MarcarEntrega(iMovID);
                    }
                }
            }
        }

        #endregion

        #region [ Historico ]

        private void btnMostrarHis_Click(object sender, EventArgs e)
        {
            this.LimpiarTabHistorico();
            try
            {
                var desde = Helper.InicioAbsoluto(this.dtpFechaDesdeHis.Value);
                var hasta = Helper.FinAbsoluto(this.dtpFechaHastaHis.Value);

                var traspasos = General.GetListOf<MovimientoInventarioTraspasosView>(t => t.TipoOperacionID == 5
                    && !t.FechaRecepcion.Equals(null)
                    && t.FechaRegistro >= desde && t.FechaRegistro <= hasta);

                if (traspasos != null)
                {
                    this.dgvHistorico.DataSource = null;
                    this.dgvHistorico.DataSource = traspasos;
                    this.lblEncontradosHis.Text = string.Format("Encontrados: {0}", traspasos.Count);
                    Helper.OcultarColumnas(this.dgvHistorico, new string[] { "TipoOperacionID", "SucursalOrigenID", "SucursalDestinoID" });
                    Helper.ColumnasToHeaderText(this.dgvHistorico);
                    //this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";                    
                    this.dgvHistorico.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
                else
                {
                    lblEncontradosHis.Text = "Encontrados:";
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvHistorico_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvHistorico.CurrentRow == null)
                return;
            var movimientoId = Helper.ConvertirEntero(this.dgvHistorico.CurrentRow.Cells["MovimientoInventarioID"].Value);
            if (movimientoId > 0)
            {
                this.CargarDetalleHistorico(movimientoId);
            }
        }

        private void dgvHistoricoDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvHistorico.CurrentRow == null)
                return;

            var movimientoId = Helper.ConvertirEntero(this.dgvHistoricoDetalle.Rows[e.RowIndex].Cells["MovimientoInventarioID"].Value);
            var detalleId = Helper.ConvertirEntero(this.dgvHistoricoDetalle.Rows[e.RowIndex].Cells["MovimientoInventarioDetalleID"].Value);

            var contingencia = General.GetEntity<MovimientoInventarioContingenciasView>(c => c.MovimientoInventarioID == movimientoId
                && c.MovimientoInventarioDetalleID == detalleId);

            var cadena = new StringBuilder();
            if (null != contingencia)
            {
                cadena.Append(string.Format("{0} {1} ", "Conflicto:", contingencia.Comentario));
                if (contingencia.MovimientoInventarioEstatusContingenciaID == 1) //Osea SOLUCIONADO
                {
                    cadena.Append(string.Format(" {0} {1} ", "Solución:", contingencia.NombreTipoOperacion));
                    cadena.Append(string.Format(" {0} {1} ", "Fecha Solución:", contingencia.FechaSoluciono.Value.ToShortDateString()));
                    if (contingencia.TipoConceptoOperacionID == 2) //ENTRADA INVENTARIO
                    {
                        cadena.Append(string.Format(" {0} {1} ", "Sucursal:", contingencia.Origen));
                    }
                    if (contingencia.TipoConceptoOperacionID == 3) //SALIDA INVENTARIO
                    {
                        cadena.Append(string.Format(" {0} {1} ", "Sucursal:", contingencia.Origen));
                    }
                    if (contingencia.TipoConceptoOperacionID == 5) //TRASPASO
                    {
                        cadena.Append(string.Format(" {0} {1} {2} {3} ", "De:", contingencia.Destino, " A:", contingencia.Destino));
                    }
                    cadena.Append(string.Format(" {0} {1} ", "Concepto:", contingencia.NombreConceptoOperacion));
                    cadena.Append(string.Format(" {0} {1} ", "Observación:", contingencia.ObservacionSolucion));
                }
            }
            this.txtDetalleConflicto.Text = cadena.ToString();
        }

        private void dgvHistoricoDetalle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvHistoricoDetalle_CellClick(sender, null);
            }
        }

        private void CargarDetalleHistorico(int movimientoInventarioId)
        {
            try
            {
                this.txtDetalleConflicto.Clear();
                var detalle = General.GetListOf<MovimientoInventarioTraspasosHisView>(m => m.MovimientoInventarioID.Equals(movimientoInventarioId));
                this.dgvHistoricoDetalle.DataSource = detalle;
                Helper.ColumnasToHeaderText(this.dgvHistoricoDetalle);
                this.dgvHistoricoDetalle.Columns["NombreParte"].HeaderText = "Descripcion";
                Helper.OcultarColumnas(this.dgvHistoricoDetalle, new string[] { "MovimientoInventarioDetalleID", "MovimientoInventarioID", "ParteID" });

                foreach (DataGridViewColumn column in this.dgvHistoricoDetalle.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    if (column.Name.Equals("Enviado") || column.Name.Equals("Recibido") || column.Name.Equals("Diferencia"))
                    {
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                    }
                }

                this.dgvHistoricoDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Conflictos ]

        public void ActualizarListadoConflictos()
        {
            this.btnMostrarConflictos_Click(null, null);
        }

        private void btnMostrarConflictos_Click(object sender, EventArgs e)
        {
            this.LimpiarTabConflictos();
            try
            {
                var conflictos = General.GetListOf<MovimientoInventarioContingenciasView>(c => c.MovimientoInventarioEstatusContingenciaID == 2);

                if (conflictos != null)
                {
                    this.dgvConflictos.DataSource = null;
                    this.dgvConflictos.DataSource = conflictos;
                    this.lblEncontradosCon.Text = string.Format("Encontrados: {0}", conflictos.Count);
                    Helper.OcultarColumnas(this.dgvConflictos, new string[] { "MovimientoInventarioTraspasoContingenciaID",
                        "MovimientoInventarioDetalleID", "SucursalOrigenID", "SucursalDestinoID",
                        "UsuarioID", "UsuarioRecibioTraspasoID", "ParteID", "MovimientoInventarioEstatusContingenciaID",
                        "UsuarioSolucionoID", "Soluciono", "FechaSoluciono", "TipoOperacionID", "NombreTipoOperacion",
                        "TipoConceptoOperacionID", "NombreConceptoOperacion", "ObservacionSolucion"
                    });
                    Helper.ColumnasToHeaderText(this.dgvConflictos);
                    this.dgvConflictos.Columns["NombreParte"].HeaderText = "Descripcion";
                    this.dgvConflictos.Columns["NombreEstatusContingencia"].HeaderText = "Estatus";
                    this.dgvConflictos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
                else
                {
                    lblEncontradosCon.Text = "Encontrados:";
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnResolver_Click(object sender, EventArgs e)
        {
            if (this.dgvConflictos.CurrentRow == null)
                return;

            try
            {
                var contingenciaId = Helper.ConvertirEntero(this.dgvConflictos.CurrentRow.Cells["MovimientoInventarioTraspasoContingenciaID"].Value);
                DetalleResolverContingencia r = new DetalleResolverContingencia(contingenciaId);
                r.ShowDialog();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnVolverTraspasar_Click(object sender, EventArgs e)
        {
            try
            {
                var res = Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;


            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Reporte de Ventas ]

        private void btnMostrarRpt_Click(object sender, EventArgs e)
        {
            try
            {
                int sucursalId = Helper.ConvertirEntero(this.cboSucursalRpt.SelectedValue);
                var fechaInicial = this.dtpDesdeRpt.Value;
                var fechaFinal = this.dtpHastaRpt.Value;
                if (sucursalId < 1)
                    return;

                var dic = new Dictionary<string, object>();
                dic.Add("SucursalID", sucursalId);
                dic.Add("FechaInicial", fechaInicial);
                dic.Add("FechaFinal", fechaFinal);

                var lst = General.ExecuteProcedure<pauVentasCancelaciones_Result>("pauVentasCancelaciones", dic);

                if (lst != null)
                {
                    this.dgvReporteVentas.DataSource = null;
                    this.dgvReporteVentas.DataSource = new SortableBindingList<pauVentasCancelaciones_Result>(lst);
                    Helper.OcultarColumnas(this.dgvReporteVentas, new string[] { "ParteID", "CantidadVendida", "Cancelaciones" });
                    Helper.ColumnasToHeaderText(this.dgvReporteVentas);
                    this.dgvReporteVentas.Columns["NombreParte"].HeaderText = "Descripcion";
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }

        }

        private void btnImprimirRpt_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvReporteVentas.DataSource == null)
                    return;

                var lista = (System.Collections.Generic.List<pauVentasCancelaciones_Result>)this.dgvReporteVentas.DataSource;
                IEnumerable<pauVentasCancelaciones_Result> listaE = lista;

                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteVentasCancelaciones.frx"));
                    report.SetParameterValue("Origen", this.cboSucursalRpt.Text);
                    report.SetParameterValue("Desde", this.dtpDesdeRpt.Value);
                    report.SetParameterValue("Hasta", this.dtpHastaRpt.Value);
                    report.RegisterData(listaE, "ventasCancelaciones", 3);
                    report.GetDataSource("ventasCancelaciones").Enabled = true;
                    report.Show(true);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion
                                                                
    }
}
