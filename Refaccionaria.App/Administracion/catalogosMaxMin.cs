using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Negocio;
using Refaccionaria.Modelo;
using System.Text.RegularExpressions;

namespace Refaccionaria.App
{
    public partial class catalogosMaxMin : UserControl
    {
        BindingSource fuenteDatos;
        ControlError cntError = new ControlError();
        BackgroundWorker bgworker;
        public delegate object MyDelegate();
        bool sel = true;

        private enum operaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        public static catalogosMaxMin Instance
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

            internal static readonly catalogosMaxMin instance = new catalogosMaxMin();
        }

        public catalogosMaxMin()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void cboMarca_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboMarca.SelectedValue == null)
                return;
            int id;
            if (int.TryParse(cboMarca.SelectedValue.ToString(), out id))
            {
                this.CargarLineas(id);
            }
        }

        private void cboLinea_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(cboLinea.SelectedValue.ToString(), out id))
            {
                this.CargarSistemas(id);
            }
        }

        private void cboSistema_SelectedValueChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(cboSistema.SelectedValue.ToString(), out id))
            {
                this.CargarSubsistemas(id);
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            try
            {
                this.IniciarActualizarListado();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvDatos.CurrentRow == null)
                return;
            try
            {
                this.txtMes1.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior1"].Value).ToString();
                this.txtMes2.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior2"].Value).ToString();
                this.txtMes3.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior3"].Value).ToString();
                this.txtMes4.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior4"].Value).ToString();
                this.txtMes5.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior5"].Value).ToString();
                this.txtMes6.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior6"].Value).ToString();
                this.txtMes7.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior7"].Value).ToString();
                this.txtMes8.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior8"].Value).ToString();
                this.txtMes9.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior9"].Value).ToString();
                this.txtMes10.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior10"].Value).ToString();
                this.txtMes11.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior11"].Value).ToString();
                this.txtMes12.Text = Helper.ConvertirDecimal(this.dgvDatos.Rows[e.RowIndex].Cells["MesAnterior12"].Value).ToString();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDatos_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                var dgv = (DataGridView)sender;
                this.dgvDatos_CellClick(sender, new DataGridViewCellEventArgs(0, dgv.CurrentRow.Index));
            }
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                var dgv = (DataGridView)sender;
                var fila = dgv.SelectedRows[0].Index;
                if (Convert.ToBoolean(dgv.Rows[fila].Cells["Sel"].Value).Equals(true))
                    dgv.Rows[fila].Cells["Sel"].Value = false;
                else
                    dgv.Rows[fila].Cells["Sel"].Value = true;
            }
        }

        private void dgvDatos_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (this.dgvDatos.Columns[e.ColumnIndex].Name == "Criterio" && e.Button == MouseButtons.Right)
                {
                    var frmCantidad = new MensajeObtenerValor("Criterio", "0", MensajeObtenerValor.Tipo.Entero);
                    if (frmCantidad.ShowDialog(Principal.Instance) == DialogResult.OK)
                    {
                        var valor = Helper.ConvertirEntero(frmCantidad.Valor);
                        if (valor >= 0 && valor <= 3)
                            foreach (DataGridViewRow row in this.dgvDatos.Rows)
                                if (Helper.ConvertirBool(row.Cells["Sel"].Value).Equals(true))
                                    row.Cells[e.ColumnIndex].Value = valor;
                    }
                    frmCantidad.Dispose();
                }

                if (this.dgvDatos.Columns[e.ColumnIndex].Name == "TiempoReposicion" && e.Button == MouseButtons.Right)
                {
                    var frmCantidad = new MensajeObtenerValor("Tiempo de Reposición", "0", MensajeObtenerValor.Tipo.Entero);
                    if (frmCantidad.ShowDialog(Principal.Instance) == DialogResult.OK)
                    {
                        var valor = Helper.ConvertirEntero(frmCantidad.Valor);
                        if (valor >= 0)
                            foreach (DataGridViewRow row in this.dgvDatos.Rows)
                                if (Helper.ConvertirBool(row.Cells["Sel"].Value).Equals(true))
                                    row.Cells[e.ColumnIndex].Value = valor;
                    }
                    frmCantidad.Dispose();
                }

                if (this.dgvDatos.Columns[e.ColumnIndex].Name == "CriteriosGenerales" && e.Button == MouseButtons.Right)
                {
                    var detCriterio = DetalleBusquedaDeCriterios.Instance;
                    detCriterio.ShowDialog();
                    var valor = detCriterio.Sel;
                    if (!string.IsNullOrEmpty(valor))
                        foreach (DataGridViewRow row in this.dgvDatos.Rows)
                            if (Helper.ConvertirBool(row.Cells["Sel"].Value).Equals(true))
                                row.Cells[e.ColumnIndex].Value = valor;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvDatos.Rows.Count <= 0)
                    return;

                var data = (DataTable)this.fuenteDatos.DataSource;
                foreach (DataRow row in data.Rows)
                {
                    //Primero se ejecuta el criterio
                    if (!string.IsNullOrEmpty(Helper.ConvertirCadena(row["Criterio"])))
                        this.ejecutarCriterio(row);

                    //Segundo: se evaluan los CriteriosGenerales
                    if (!string.IsNullOrEmpty(Helper.ConvertirCadena(row["CriteriosGenerales"])))
                        this.evaluarCriteriosGenerales(row);

                    //Tercero: se aplican los ajustes
                    if (!string.IsNullOrEmpty(Helper.ConvertirCadena(row["AjusteMx"])) || !string.IsNullOrEmpty(Helper.ConvertirCadena(row["AjusteMn"])))
                        this.ejecutarAjuste(row);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var res = Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                SplashScreen.Show(new Splash());
                this.btnGuardar.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                foreach (DataGridViewRow row in this.dgvDatos.Rows)
                {
                    if (Helper.ConvertirBool(row.Cells["Sel"].Value).Equals(true))
                    {
                        var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                        var sucursalId = Helper.ConvertirEntero(row.Cells["SucursalID"].Value);

                        //Actualizar tabla ParteExistencia, campos Maximo y Minimo
                        var parteExistencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId);
                        if (null != parteExistencia)
                        {
                            parteExistencia.Maximo = Helper.ConvertirEntero(row.Cells["MAX"].Value);
                            parteExistencia.Minimo = Helper.ConvertirEntero(row.Cells["MIN"].Value);
                            Guardar.Generico<ParteExistencia>(parteExistencia);
                        }

                        //Actualizar tabla Parte, campo TiempoReposicion
                        var parte = General.GetEntity<Parte>(p => p.ParteID == parteId);
                        if (null != parte)
                        {
                            parte.TiempoReposicion = Helper.ConvertirDecimal(row.Cells["TiempoReposicion"].Value);
                            Guardar.Generico<Parte>(parte);
                        }

                        var mxmn = General.GetEntity<ParteMxMn>(p => p.ParteID == parteId && p.SucursalID == sucursalId && p.Estatus);

                        int? max = null;
                        int? min = null;
                        bool fijo = false;
                        if (Helper.ConvertirBool(row.Cells["Fijo"].Value).Equals(true))
                        {
                            fijo = true;
                            if (!string.IsNullOrEmpty(Helper.ConvertirCadena(row.Cells["MAX"].Value)))
                                max = Helper.ConvertirEntero(row.Cells["MAX"].Value);
                            if (!string.IsNullOrEmpty(Helper.ConvertirCadena(row.Cells["MIN"].Value)))
                                min = Helper.ConvertirEntero(row.Cells["MIN"].Value);
                        }

                        if (null == mxmn) //Nuevo registro
                        {
                            mxmn = new ParteMxMn()
                            {
                                EsFijo = fijo,
                                ParteID = parteId,
                                SucursalID = sucursalId,
                                Criterio = !string.IsNullOrEmpty(Helper.ConvertirCadena(row.Cells["Criterio"].Value)) ? Helper.ConvertirCadena(row.Cells["Criterio"].Value) : null,
                                AjusteMn = !string.IsNullOrEmpty(Helper.ConvertirCadena(row.Cells["AjusteMn"].Value)) ? Helper.ConvertirCadena(row.Cells["AjusteMn"].Value) : null,
                                AjusteMx = !string.IsNullOrEmpty(Helper.ConvertirCadena(row.Cells["AjusteMx"].Value)) ? Helper.ConvertirCadena(row.Cells["AjusteMx"].Value) : null,
                                Maximo = max,
                                Minimo = min
                            };
                        }
                        else
                        {
                            mxmn.EsFijo = fijo;
                            mxmn.Criterio = Helper.ConvertirCadena(row.Cells["Criterio"].Value);
                            mxmn.AjusteMn = Helper.ConvertirCadena(row.Cells["AjusteMn"].Value);
                            mxmn.AjusteMx = Helper.ConvertirCadena(row.Cells["AjusteMx"].Value);
                            mxmn.Maximo = max;
                            mxmn.Minimo = min;
                        }
                        Guardar.Generico<ParteMxMn>(mxmn);
                        this.actualizarParteMxMnCriterio(mxmn.ParteMxMnID, Helper.ConvertirCadena(row.Cells["CriteriosGenerales"].Value));
                    }
                }

                this.Cursor = Cursors.Default;
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                new Notificacion("Información almacenada correctamente.", 2 * 1000).Mostrar(Principal.Instance);
                this.CargaInicial();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnSeleccionarDiferencia_Click(object sender, EventArgs e)
        {
            if (sel)
            {
                this.btnSeleccionarDiferencia.Text = "Sel Ninguno";
                sel = false;
            }
            else
            {
                this.btnSeleccionarDiferencia.Text = "Sel Todos";
                sel = true;
            }

            if (this.dgvDatos.Columns.Contains("Sel"))
            {
                foreach (DataGridViewRow row in this.dgvDatos.Rows)
                    if (sel)
                        row.Cells["Sel"].Value = false;
                    else
                        row.Cells["Sel"].Value = true;
            }
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
                this.LimpiarFormulario();
                this.CargarMarcas(0);

                this.cboSucursal.DataSource = Negocio.General.GetListOf<Sucursal>(s => s.Estatus.Equals(true));
                this.cboSucursal.DisplayMember = "NombreSucursal";
                this.cboSucursal.ValueMember = "SucursalID";

                var listaProveedor = General.GetListOf<Proveedor>(p => p.ProveedorEstatusID == 1 && p.Estatus);
                listaProveedor.Insert(0, new Proveedor() { ProveedorID = 0, NombreProveedor = "TODOS", UsuarioID = 1, FechaRegistro = DateTime.Now, Estatus = true, Actualizar = true });
                this.cboProveedor.DataSource = listaProveedor;
                this.cboProveedor.DisplayMember = "NombreProveedor";
                this.cboProveedor.ValueMember = "ProveedorID";
                AutoCompleteStringCollection autProveedor = new AutoCompleteStringCollection();
                foreach (var proveedor in listaProveedor) autProveedor.Add(proveedor.NombreProveedor);
                this.cboProveedor.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboProveedor.AutoCompleteCustomSource = autProveedor;
                this.cboMarca.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);

                var hoy = DateTime.Now;
                int anioActual = hoy.Year;
                int mes = hoy.Month;

                this.lblMes1.Text = string.Format("{0}{1}", hoy.AddMonths(-1).ToShortMonthName(), hoy.AddMonths(-1).ToString("yy"));
                this.lblMes2.Text = string.Format("{0}{1}", hoy.AddMonths(-2).ToShortMonthName(), hoy.AddMonths(-2).ToString("yy"));
                this.lblMes3.Text = string.Format("{0}{1}", hoy.AddMonths(-3).ToShortMonthName(), hoy.AddMonths(-3).ToString("yy"));
                this.lblMes4.Text = string.Format("{0}{1}", hoy.AddMonths(-4).ToShortMonthName(), hoy.AddMonths(-4).ToString("yy"));
                this.lblMes5.Text = string.Format("{0}{1}", hoy.AddMonths(-5).ToShortMonthName(), hoy.AddMonths(-5).ToString("yy"));
                this.lblMes6.Text = string.Format("{0}{1}", hoy.AddMonths(-6).ToShortMonthName(), hoy.AddMonths(-6).ToString("yy"));
                this.lblMes7.Text = string.Format("{0}{1}", hoy.AddMonths(-7).ToShortMonthName(), hoy.AddMonths(-7).ToString("yy"));
                this.lblMes8.Text = string.Format("{0}{1}", hoy.AddMonths(-8).ToShortMonthName(), hoy.AddMonths(-8).ToString("yy"));
                this.lblMes9.Text = string.Format("{0}{1}", hoy.AddMonths(-9).ToShortMonthName(), hoy.AddMonths(-9).ToString("yy"));
                this.lblMes10.Text = string.Format("{0}{1}", hoy.AddMonths(-10).ToShortMonthName(), hoy.AddMonths(-10).ToString("yy"));
                this.lblMes11.Text = string.Format("{0}{1}", hoy.AddMonths(-11).ToShortMonthName(), hoy.AddMonths(-11).ToString("yy"));
                this.lblMes12.Text = string.Format("{0}{1}", hoy.AddMonths(-12).ToShortMonthName(), hoy.AddMonths(-12).ToString("yy"));

                var criterios = General.GetListOf<MxMnCriterioView>(c => c.MxMnCriterioID > 0);
                this.dgvCriterios.DataSource = criterios;
                //Helper.OcultarColumnas(this.dgvCriterios, new string[] { "MxMnCriterioID" });
                Helper.ColumnasToHeaderText(this.dgvCriterios);
                this.dgvCriterios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void LimpiarFormulario()
        {
            try
            {
                this.tabMaxMin.SelectedIndex = 0;
                this.dgvDatos.DataSource = null;
                if (this.dgvDatos.Rows.Count > 0)
                    this.dgvDatos.Rows.Clear();
                if (this.dgvDatos.Columns.Count > 0)
                    this.dgvDatos.Columns.Clear();

                this.dgvCriterios.DataSource = null;
                if (this.dgvCriterios.Rows.Count > 0)
                    this.dgvCriterios.Rows.Clear();
                if (this.dgvCriterios.Columns.Count > 0)
                    this.dgvCriterios.Columns.Clear();

                this.lblEncontrados.Text = "Encontrados:";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarMarcas(int valor)
        {
            try
            {
                if (valor > 0)
                {
                    var listaMarcaParte = General.GetListOf<LineaMarcaPartesView>(m => m.LineaID == valor);
                    listaMarcaParte.Insert(0, new LineaMarcaPartesView() { MarcaParteID = 0, NombreMarcaParte = "TODOS" });
                    this.cboMarca.DataSource = listaMarcaParte;
                    this.cboMarca.DisplayMember = "NombreMarcaParte";
                    this.cboMarca.ValueMember = "MarcaParteID";
                    AutoCompleteStringCollection autMarcaParte = new AutoCompleteStringCollection();
                    foreach (var marcaParte in listaMarcaParte) autMarcaParte.Add(marcaParte.NombreMarcaParte);
                    this.cboMarca.AutoCompleteMode = AutoCompleteMode.Suggest;
                    this.cboMarca.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    this.cboMarca.AutoCompleteCustomSource = autMarcaParte;
                    this.cboMarca.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
                }
                else
                {
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
                    this.cboMarca.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
                }
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
                if (marcaId > 0)
                {
                    var listaLineas = Negocio.General.GetListOf<LineaMarcaPartesView>(l => l.MarcaParteID.Equals(marcaId));
                    listaLineas.Insert(0, new LineaMarcaPartesView() { LineaID = 0, NombreLinea = "TODOS" });
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
                    var listaLineas = Negocio.General.GetListOf<LineasView>(l => l.LineaID > 0);
                    listaLineas.Insert(0, new LineasView() { LineaID = 0, NombreLinea = "TODOS" });
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

        private void CargarSistemas(int lineaId)
        {
            try
            {
                if (lineaId > 0)
                {
                    var sistemaId = Negocio.General.GetEntity<Linea>(l => l.LineaID.Equals(lineaId)).SistemaID;
                    var listaSistemas = Negocio.General.GetListOf<Sistema>(s => s.SistemaID.Equals(sistemaId));
                    cboSistema.DataSource = listaSistemas;
                    cboSistema.DisplayMember = "NombreSistema";
                    cboSistema.ValueMember = "SistemaID";

                    AutoCompleteStringCollection autSistema = new AutoCompleteStringCollection();
                    foreach (var listaSistema in listaSistemas) autSistema.Add(listaSistema.NombreSistema);
                    cboSistema.AutoCompleteMode = AutoCompleteMode.Suggest;
                    cboSistema.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cboSistema.AutoCompleteCustomSource = autSistema;
                    cboSistema.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
                }
                else
                {
                    var listaSistemas = Negocio.General.GetListOf<Sistema>(s => s.SistemaID.Equals(0));
                    listaSistemas.Insert(0, new Sistema() { SistemaID = 0, NombreSistema = "TODOS", UsuarioID = 1, FechaRegistro = DateTime.Now, Actualizar = true, Estatus = true });
                    cboSistema.DataSource = listaSistemas;
                    cboSistema.DisplayMember = "NombreSistema";
                    cboSistema.ValueMember = "SistemaID";
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarSubsistemas(int sistemaId)
        {
            try
            {
                if (sistemaId > 0)
                {
                    var listaSubsistemas = Negocio.General.GetListOf<Subsistema>(s => s.SistemaID.Equals(sistemaId));
                    cboSubsistema.DataSource = listaSubsistemas;
                    cboSubsistema.DisplayMember = "NombreSubsistema";
                    cboSubsistema.ValueMember = "SubsistemaID";
                    AutoCompleteStringCollection autSubsistema = new AutoCompleteStringCollection();
                    foreach (var listaSubsistema in listaSubsistemas) autSubsistema.Add(listaSubsistema.NombreSubsistema);
                    cboSubsistema.AutoCompleteMode = AutoCompleteMode.Suggest;
                    cboSubsistema.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cboSubsistema.AutoCompleteCustomSource = autSubsistema;
                    cboSubsistema.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);
                }
                else
                {
                    var listaSubsistemas = Negocio.General.GetListOf<Subsistema>(s => s.SistemaID.Equals(0));
                    listaSubsistemas.Insert(0, new Subsistema() { SubsistemaID = 0, SistemaID = 0, NombreSubsistema = "TODOS", FechaRegistro = DateTime.Now, Actualizar = true, Estatus = true });
                    cboSubsistema.DataSource = listaSubsistemas;
                    cboSubsistema.DisplayMember = "NombreSubsistema";
                    cboSubsistema.ValueMember = "SubsistemaID";
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void IniciarActualizarListado()
        {
            this.dgvDatos.DataSource = null;
            if (this.dgvDatos.Rows.Count > 0)
                this.dgvDatos.Rows.Clear();
            if (this.dgvDatos.Columns.Count > 0)
                this.dgvDatos.Columns.Clear();

            this.lblEncontrados.Text = "Encontrados:";
            this.Cursor = Cursors.WaitCursor;
            bgworker = new BackgroundWorker();
            bgworker.DoWork += ActualizarListado;
            bgworker.RunWorkerCompleted += Terminado;
            bgworker.RunWorkerAsync();
            progreso.Value = 10;

            bgworker.WorkerReportsProgress = true;
            bgworker.DoWork += new DoWorkEventHandler(bgworker_DoWork);
            bgworker.ProgressChanged += new ProgressChangedEventHandler(bgworker_ProgressChanged);
        }

        void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 11; i <= 100; i++)
            {
                bgworker.ReportProgress(i);                           
            }
        }

        void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progreso.Value = e.ProgressPercentage;
        }

        public void ActualizarListado(object o, DoWorkEventArgs e)
        {
            try
            {
                var sucursalId = Helper.ConvertirEntero(GetSelectedValue(this.cboSucursal));
                var marcaId = Helper.ConvertirEntero(GetSelectedValue(this.cboMarca));
                var lineaId = Helper.ConvertirEntero(GetSelectedValue(this.cboLinea));
                var sistemaId = Helper.ConvertirEntero(GetSelectedValue(this.cboSistema));
                var subsistemaId = Helper.ConvertirEntero(GetSelectedValue(this.cboSubsistema));
                var proveedorId = Helper.ConvertirEntero(GetSelectedValue(this.cboProveedor));

                DataTable dt = new DataTable();

                //Filtro Todos
                
                this.fuenteDatos = new BindingSource();
                this.fuenteDatos.DataSource = dt;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private object GetSelectedValue(ComboBox cbo)
        {
            object obj = null;
            if (cbo.InvokeRequired)
            {
                obj = cbo.Invoke((MyDelegate)delegate() { return obj = cbo.SelectedValue; });
                return obj;
            }
            else
            {
                return obj = cbo.SelectedValue;
            }
        }

        public void Terminado(object o, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.dgvDatos.DataSource = null;
                if (this.dgvDatos.Rows.Count > 0)
                    this.dgvDatos.Rows.Clear();
                if (this.dgvDatos.Columns.Count > 0)
                    this.dgvDatos.Columns.Clear();

                this.dgvDatos.DataSource = this.fuenteDatos;
                this.lblEncontrados.Text = string.Format("Encontrados: {0}", fuenteDatos.Count);

                Helper.ColumnasToHeaderText(this.dgvDatos);
                Helper.OcultarColumnas(this.dgvDatos, new string[] { "MxMnID", "ParteID", "Cantidad", "SucursalID", "LineaID", "NombreLinea", 
                    "MarcaParteID", "NombreMarcaParte", "SistemaID", "NombreSistema", "SubsistemaID", "NombreSubsistema", "ProveedorID", 
                    "NombreProveedor", "Es9500", "MesAnterior1", "MesAnterior2", "MesAnterior3", "MesAnterior4", "MesAnterior5", "MesAnterior6", 
                    "MesAnterior7", "MesAnterior8", "MesAnterior9", "MesAnterior10", "MesAnterior11", "MesAnterior12", "EntityKey", "EntityState",
                    "PromedioMayor", "PromedioMenor"
                });

                Helper.ReadOnlyColumnas(this.dgvDatos, new string[] { "NumeroParte", "NombreParte", "Existencia", "UnidadEmpaque", "Promedio", "Mx", "Mn" });
                Helper.FormatoDecimalColumnas(this.dgvDatos, new string[] { "Existencia", "UnidadEmpaque", "TiempoReposicion", "Mx", "Mn", "Max", "Min" });
                this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                this.dgvDatos.Columns["Sel"].HeaderText = "";
                progreso.Value = 0;
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                this.Cursor = Cursors.Default;
            }
        }

        private void ejecutarCriterio(DataRow row)
        {
            try
            {
                decimal puntoPedido = 0;
                decimal tiempoRepocion = 0;
                decimal consumoPromedio = 0;
                decimal consumoMaximo = 0;
                decimal consumoMinimo = 0;
                decimal existenciaMaxima = 0;
                decimal existenciaMinima = 0;
                decimal cantidadPedido = 0;
                decimal existenciaActual = 0;

                if (!string.IsNullOrEmpty(Helper.ConvertirCadena(row["Criterio"])))
                {
                    int criterio = Helper.ConvertirEntero(row["Criterio"]);
                    tiempoRepocion = Helper.ConvertirDecimal(row["TiempoReposicion"]);
                    consumoPromedio = Helper.ConvertirDecimal(row["Promedio"]);
                    consumoMaximo = Helper.ConvertirDecimal(row["Mx"]);
                    consumoMinimo = Helper.ConvertirDecimal(row["Mn"]);
                    existenciaActual = Helper.ConvertirDecimal(row["Existencia"]);
                    existenciaMinima = consumoMinimo * tiempoRepocion;
                    existenciaMaxima = consumoMaximo * tiempoRepocion + existenciaMinima;
                    puntoPedido = consumoPromedio * tiempoRepocion + existenciaMinima;
                    cantidadPedido = existenciaMaxima - existenciaActual;

                    switch (criterio)
                    {
                        case 0: //Punto Pedido
                            row["MAX"] = existenciaMaxima;
                            row["MIN"] = puntoPedido;
                            break;

                        case 1: //Promedio Ventas Mayores, Promedio de Ventas Menores
                            row["MAX"] = Helper.ConvertirDecimal(row["PromedioMayor"]);
                            row["MIN"] = Helper.ConvertirDecimal(row["PromedioMenor"]);
                            break;

                        case 2:
                            row["MAX"] = Helper.ConvertirDecimal(row["Mx"]);
                            row["MIN"] = Helper.ConvertirDecimal(row["Mn"]);
                            break;

                        case 3:
                            row["MAX"] = 0;
                            row["MIN"] = 0;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void evaluarCriteriosGenerales(DataRow row)
        {
            try
            {
                string criterios = Helper.ConvertirCadena(row["CriteriosGenerales"]);
                string[] arrCriterios = criterios.Split(',');
                foreach (var criterio in arrCriterios)
                {
                    switch (criterio)
                    {
                        case "1":
                            var Mn = Helper.ConvertirDecimal(row["Mn"]);
                            if (Mn > 0.0M && Mn < 1.00M)
                                row["MIN"] = 1;
                            break;

                        case "2":
                            var Es9500 = Helper.ConvertirBool(row["Es9500"]);
                            if (Es9500)
                            {
                                row["MIN"] = 0;
                                row["MAX"] = 0;
                            }
                            break;

                        case "3":
                            var Min = Helper.ConvertirDecimal(row["Mn"]);
                            var Max = Helper.ConvertirDecimal(row["Mx"]);
                            if (Min > 0 && Min == Max)
                            {
                                row["MIN"] = Min - 1;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void ejecutarAjuste(DataRow row)
        {
            try
            {
                var Max = Helper.ConvertirDecimal(row["MAX"]);
                var Min = Helper.ConvertirDecimal(row["MIN"]);
                var ajMax = Helper.ConvertirCadena(row["AjusteMx"]);
                var ajMin = Helper.ConvertirCadena(row["AjusteMn"]);

                if (!string.IsNullOrEmpty(ajMax))
                {
                    var cadena = ajMax.Replace(" ", "").Trim();
                    var operador = cadena.Substring(0, 1);
                    var cantidad = Helper.ConvertirDecimal(cadena.Substring(1, cadena.Length - 1));
                    switch (operador)
                    {
                        case "+":
                            row["MAX"] = Max + cantidad;
                            break;

                        case "-":
                            var res = Max - cantidad;
                            row["MAX"] = res > 0 ? res : 0;
                            break;

                        case "*":
                            row["MAX"] = Max * cantidad;
                            break;

                        case "/":
                            if (cantidad > 0)
                            {
                                var resd = Min / cantidad;
                                row["MAX"] = resd > 0 ? resd : 0;
                            }
                            else
                            {
                                row["MAX"] = 0;
                            }
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(ajMin))
                {
                    var cadena = ajMin.Replace(" ", "").Trim();
                    var operador = cadena.Substring(0, 1);
                    var cantidad = Helper.ConvertirDecimal(cadena.Substring(1, cadena.Length - 1));
                    switch (operador)
                    {
                        case "+":
                            row["MIN"] = Min + cantidad;
                            break;

                        case "-":
                            var res = Min - cantidad;
                            row["MIN"] = res > 0 ? res : 0;
                            break;

                        case "*":
                            row["MIN"] = Min * cantidad;
                            break;

                        case "/":
                            if (cantidad > 0)
                            {
                                var resd = Min / cantidad;
                                row["MIN"] = resd > 0 ? resd : 0;
                            }
                            else
                            {
                                row["MIN"] = 0;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void actualizarParteMxMnCriterio(int parteMxMnID, string criterios)
        {
            if (string.IsNullOrEmpty(criterios))
                return;

            Match match = Regex.Match(criterios, @"^(\d+,)*\d+$");
            if (!match.Success)
                return;

            var values = criterios.Split(',');
            try
            {
                var actuales = General.GetListOf<ParteMxMnCriterio>(p => p.ParteMxMnID == parteMxMnID);
                var selectedValues = new Dictionary<string, int>();

                foreach (var item in values)
                {
                    selectedValues.Add(item, (int)operaciones.Add);
                }

                foreach (var item in actuales)
                {
                    if (selectedValues.ContainsKey(item.MxMnCriterioID.ToString()))
                    {
                        selectedValues[item.MxMnCriterioID.ToString()] = (int)operaciones.None;
                    }
                    else
                    {
                        selectedValues[item.MxMnCriterioID.ToString()] = (int)operaciones.Delete;
                    }
                }

                foreach (var item in selectedValues)
                {
                    if (item.Value == (int)operaciones.Add) //add new
                    {
                        var mxmnc = new ParteMxMnCriterio
                        {
                            ParteMxMnID = parteMxMnID,
                            MxMnCriterioID = Helper.ConvertirEntero(item.Key)
                        };
                        Guardar.Generico<ParteMxMnCriterio>(mxmnc);
                    }
                    else if (item.Value == (int)operaciones.Delete) //search and delete
                    {
                        var id = Helper.ConvertirEntero(item.Key);
                        var partemxmnc = General.GetEntity<ParteMxMnCriterio>(p => p.ParteMxMnID == parteMxMnID && p.MxMnCriterioID == id);
                        if (partemxmnc != null)
                            General.Delete<ParteMxMnCriterio>(partemxmnc);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void x()
        {
            //AND = $
            //OR = #
            //Afectar a = :

            string sCondicion = "[Mn](>){0}$[Mn](<){2}:[MIN]={1}";
            //string sCondicion2 = "[Es9500](=){1}:[Mn](=){0},[MAX](=){0}";
            //string sCondicion3 = "[Mn](>){0}$[Mn]=[Mx]:[MIN](=)[MIN](-){1}";

            var regla = sCondicion.Split(':');
            if (regla.Count() == 2) //Valida que tenga una condicion y un campo a afectar
            {
                if (regla[0].Contains('$')) //AND //Valida si la regla tiene 2 partes para evaluar
                {
                    var expresiones = regla[0].Split('$');
                    if (expresiones.Count() > 1)
                    {
                        if (expresiones[0].Length > 1) //Valida que la expresion1 tenga valores                         
                        {
                            var value = expresiones[0];
                            var campo = Regex.Matches(value, @"\[(.+?)\]").Cast<Match>().Select(m => m.Groups[1].Value);
                            var operador = Regex.Matches(value, @"\((.+?)\)").Cast<Match>().Select(m => m.Groups[1].Value);
                            var valor = Regex.Matches(value, @"\{(.+?)\}").Cast<Match>().Select(m => m.Groups[1].Value);
                        }
                        if (expresiones[1].Length > 1) //Valida que la expresion1 tenga valores                         
                        {
                            var value = expresiones[0];
                            var campo = Regex.Matches(value, @"\[(.+?)\]").Cast<Match>().Select(m => m.Groups[1].Value);
                            var operador = Regex.Matches(value, @"\((.+?)\)").Cast<Match>().Select(m => m.Groups[1].Value);
                            var valor = Regex.Matches(value, @"\{(.+?)\}").Cast<Match>().Select(m => m.Groups[1].Value);
                        }
                    }
                }
                else //Solo tiene una expresión para evaluar
                {
                    if (regla[0].Length > 1) //Valida que la expresion1 tenga valores                         
                    {
                        var value = regla[0];
                        var campo = Regex.Matches(value, @"\[(.+?)\]").Cast<Match>().Select(m => m.Groups[1].Value);
                        var operador = Regex.Matches(value, @"\((.+?)\)").Cast<Match>().Select(m => m.Groups[1].Value);
                        var valor = Regex.Matches(value, @"\{(.+?)\}").Cast<Match>().Select(m => m.Groups[1].Value);
                    }
                }
            }

            //var conds = sCondicion.Split('#');
            //foreach (var cond in conds)
            //{
            //    var valores = cond.Split('=');
            //    string sCampo = valores[0];
            //    string svalor = valores[1];
            //    var registro = General.GetEntity<MxMnView>(q => q.MxMnID == 1);

            //    var tipoobj = registro.GetType();
            //    var propCampo = tipoobj.GetProperty(sCampo);
            //    var oValor = propCampo.GetValue(registro, null);

            //    //switch (sComparador)
            //    //{
            //    //    case "=":
            //    //        bPasa = (oValor == oValorFijo);
            //    //}

            //}
        }

        #endregion

    }
}
