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
using System.IO;

namespace Refaccionaria.App
{
    public partial class catalogosKardex : UserControl
    {

        BindingSource fuenteDatos;
        BindingSource fuenteDatosKardex;
        public static catalogosKardex Instance
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

            internal static readonly catalogosKardex instance = new catalogosKardex();
        }

        public catalogosKardex()
        {
            InitializeComponent();
        }

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
                this.txtCodigo.Clear();
                this.txtDescripcion.Clear();

                this.dtpFechaDesde.Value = DateTime.Now.AddMonths(-3);
                this.dtpFechaHasta.Value = DateTime.Now;

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

                var listaLineas = General.GetListOf<Linea>(l => l.Estatus);
                listaLineas.Insert(0, new Linea() { LineaID = 0, NombreLinea = "TODOS" });
                cboLinea.DataSource = listaLineas;
                cboLinea.DisplayMember = "NombreLinea";
                cboLinea.ValueMember = "LineaID";
                AutoCompleteStringCollection autLinea = new AutoCompleteStringCollection();
                foreach (var listaLinea in listaLineas) autLinea.Add(listaLinea.NombreLinea);
                cboLinea.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboLinea.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboLinea.AutoCompleteCustomSource = autLinea;
                cboLinea.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);

                var listaSuc = General.GetListOf<Sucursal>(s => s.Estatus).ToList();
                listaSuc.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "TODOS", UsuarioID = 1, FechaRegistro = DateTime.Now, Estatus = true, Actualizar = true });
                this.cboUbicacionOrigen.DataSource = listaSuc;
                this.cboUbicacionOrigen.DisplayMember = "NombreSucursal";
                this.cboUbicacionOrigen.ValueMember = "SucursalID";

                var listaProveedor = General.GetListOf<Proveedor>(p => p.Estatus).ToList();
                listaProveedor.Insert(0, new Proveedor() { ProveedorID = 0, NombreProveedor = "TODOS", UsuarioID = 1, FechaRegistro = DateTime.Now, Estatus = true, Actualizar = true });
                this.cboProveedor.DataSource = listaProveedor;
                this.cboProveedor.DisplayMember = "NombreProveedor";
                this.cboProveedor.ValueMember = "ProveedorID";
                AutoCompleteStringCollection autProveedor = new AutoCompleteStringCollection();
                foreach (var proveedor in listaProveedor) autProveedor.Add(proveedor.NombreProveedor);
                this.cboProveedor.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboProveedor.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboProveedor.AutoCompleteCustomSource = autProveedor;
                this.cboProveedor.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);
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
                this.dgvDatos.DataSource = null;
                this.dgvOperaciones.DataSource = null;
                this.dgvDetalle.DataSource = null;
                this.dgvExistencias.DataSource = null;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                    cboLinea.TextUpdate += new EventHandler(Helper.cboCharacterCasingUpper);
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
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                //this.dgvExistencias.AutoResizeColumns();
                Helper.OcultarColumnas(this.dgvExistencias, new string[] { "ParteExistenciaID", "ParteID", "NumeroParte", "SucursalID" });
                Helper.ColumnasToHeaderText(this.dgvExistencias);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarKardex(int parteId)
        {
            try
            {
                var seleccionSucursal = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue);
                var sucursal = string.Empty;
                if (seleccionSucursal == 0)
                {
                    var sucursales = General.GetListOf<Sucursal>(s => s.Estatus).ToList();
                    var ids = new StringBuilder();
                    foreach (var sucursalId in sucursales)
                    {
                        ids.Append(string.Format("{0},", sucursalId.SucursalID));
                    }
                    if (ids.ToString().Length > 0)
                        sucursal = ids.ToString().Substring(0, ids.ToString().Length - 1);
                }
                else
                {
                    sucursal = Helper.ConvertirCadena(this.cboUbicacionOrigen.SelectedValue);
                }

                var dic = new Dictionary<string, object>();
                dic.Add("ParteID", parteId);
                dic.Add("SucursalID", sucursal);
                dic.Add("FechaInicial", this.dtpFechaDesde.Value.Date);
                dic.Add("FechaFinal", this.dtpFechaHasta.Value.Date);

                Cursor.Current = Cursors.WaitCursor;

                var kardex = General.ExecuteProcedure<pauKardex_Result>("pauKardex", dic);
                if (kardex != null)
                {
                    //Detalle
                    //this.dgvDetalle.DataSource = null;
                    //this.dgvDetalle.DataSource = kardex;

                    this.dgvDetalle.DataSource = null;
                    this.fuenteDatosKardex = new BindingSource();
                    var dt = Helper.newTable<pauKardex_Result>("Kardex", kardex);
                    this.fuenteDatosKardex.DataSource = dt;
                    this.dgvDetalle.DataSource = this.fuenteDatosKardex;

                    Helper.ColumnasToHeaderText(this.dgvDetalle);
                    Helper.FormatoDecimalColumnas(this.dgvDetalle, new string[] { "Unitario", "Cantidad", "ExistenciaNueva" });

                    //Operaciones
                    var x = kardex.ToList();
                    var summary = from p in x
                                  group p by p.Operacion into g
                                  select new { Operacion = g.Key, Cantidad = g.Sum(z => z.Cantidad), Importe = g.Sum(z => z.Unitario) };

                    this.dgvOperaciones.DataSource = null;
                    this.dgvOperaciones.DataSource = summary.ToList();
                    Helper.FormatoDecimalColumnas(this.dgvOperaciones, new string[] { "Cantidad", "Importe" });

                    //Existencia Nueva
                    var existencia = 0M;
                    var existencias = new List<ParteExistencia>();
                    if (seleccionSucursal == 0)
                    {
                        existencias = General.GetListOf<ParteExistencia>(p => p.ParteID == parteId).ToList();
                        foreach (var exist in existencias)
                            existencia += Helper.ConvertirDecimal(exist.ExistenciaInicial);

                        this.txtExistenciaInicial.Text = existencia.ToString();
                    }
                    else
                    {
                        var sucursalId = Helper.ConvertirEntero(sucursal);
                        var exist = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId);
                        existencia = Helper.ConvertirDecimal(exist.ExistenciaInicial);
                        this.txtExistenciaInicial.Text = existencia.ToString();
                    }

                    if (existencia != null)
                    {
                        var cantidad = 0M;
                        foreach (DataGridViewRow row in this.dgvDetalle.Rows)
                        {
                            cantidad = Helper.ConvertirDecimal(row.Cells["Cantidad"].Value);
                            if (Helper.ConvertirCadena(row.Cells["Tipo"].Value) == "S")
                                existencia = existencia - cantidad;
                            else
                                existencia = existencia + cantidad;
                            row.Cells["ExistenciaNueva"].Value = existencia;
                        }
                    }

                    this.fuenteDatosKardex.Filter = String.Format("Fecha >= '{0:yyyy-MM-dd}' AND Fecha < '{1:yyyy-MM-dd}'", dtpFechaDesde.Value, dtpFechaHasta.Value.AddDays(1));
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos ]

        private void catalogosKardex_Load(object sender, EventArgs e)
        {
            this.dgvDatos.DataSource = null;
            this.dgvDetalle.DataSource = null;
            this.dgvExistencias.DataSource = null;
            this.dgvOperaciones.DataSource = null;
            this.CargaInicial();
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string input = this.txtDescripcion.Text.Replace("'", "");
                string[] matches = Regex.Matches(input, @""".*?""|[^\s]+").Cast<Match>().Select(m => m.Value).ToArray();

                if (matches.Length > 0 && matches.Length < 10)
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("Codigo", null);
                    for (int x = 0; x < matches.Length; x++)
                    {
                        dic.Add(string.Format("{0}{1}", "Descripcion", x + 1), matches[x].ToString());
                    }

                    var lst = General.ExecuteProcedure<pauParteBusquedaAvanzadaEnKardex_Result>("pauParteBusquedaAvanzadaEnKardex", dic);

                    if (lst != null)
                    {
                        this.dgvDatos.DataSource = null;
                        this.fuenteDatos = new BindingSource();
                        var dt = Helper.newTable<pauParteBusquedaAvanzadaEnKardex_Result>("Partes", lst);
                        this.fuenteDatos.DataSource = dt;
                        this.dgvDatos.DataSource = this.fuenteDatos;
                        Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "MarcaParteID", "LineaID", "ProveedorID", "Busqueda" });
                        Helper.ColumnasToHeaderText(this.dgvDatos);
                        this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";
                        this.dgvDatos.Columns["NombreParte"].Width = 400;
                        //this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    }
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
                string input = this.txtCodigo.Text.Replace("'", "");
                var dic = new Dictionary<string, object>();
                dic.Add("Codigo", input);
                for (int x = 0; x < 9; x++)
                {
                    dic.Add(string.Format("{0}{1}", "Descripcion", x + 1), null);
                }
                var lst = General.ExecuteProcedure<pauParteBusquedaAvanzadaEnKardex_Result>("pauParteBusquedaAvanzadaEnKardex", dic);

                if (lst != null)
                {
                    this.dgvDatos.DataSource = null;
                    this.fuenteDatos = new BindingSource();
                    var dt = Helper.newTable<pauParteBusquedaAvanzadaEnKardex_Result>("Partes", lst);
                    this.fuenteDatos.DataSource = dt;
                    this.dgvDatos.DataSource = this.fuenteDatos;
                    Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "MarcaParteID", "LineaID", "ProveedorID", "Busqueda" });
                    Helper.ColumnasToHeaderText(this.dgvDatos);
                    this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";
                    this.dgvDatos.Columns["NombreParte"].Width = 400;
                    //this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void cboMarca_SelectedValueChanged(object sender, EventArgs e)
        {
            //if (cboMarca.SelectedValue == null)
            //    return;
            //int id;
            //if (int.TryParse(cboMarca.SelectedValue.ToString(), out id))
            //{
            //    this.CargarLineas(id);
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

                var dic = new Dictionary<string, object>();
                dic.Add("MarcaParteID", marcaparteId);
                dic.Add("LineaID", lineaId);
                dic.Add("ProveedorID", proveedorId);
                var lst = General.ExecuteProcedure<pauParteBusquedaEnKardex_Result>("pauParteBusquedaEnKardex", dic);

                if (lst != null)
                {
                    this.dgvDatos.DataSource = null;
                    this.dgvDatos.DataSource = new SortableBindingList<pauParteBusquedaEnKardex_Result>(lst);
                    Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "MarcaParteID", "LineaID", "ProveedorID", "Busqueda" });
                    Helper.ColumnasToHeaderText(this.dgvDatos);
                    this.dgvDatos.Columns["NombreParte"].HeaderText = "Descripcion";
                    this.dgvDatos.Columns["NombreParte"].Width = 400;
                    //this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
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
            var parteId = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ParteID"].Value);
            if (parteId > 0)
            {
                this.CargaExistencias(parteId);
                this.CargarKardex(parteId);
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

        private void cboUbicacionOrigen_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow != null)
                this.dgvDatos_CellClick(sender, new DataGridViewCellEventArgs(0, this.dgvDatos.CurrentRow.Index));
        }

        private void dtpFechaDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow != null)
                this.fuenteDatosKardex.Filter = String.Format("Fecha >= '{0:yyyy-MM-dd}' AND Fecha < '{1:yyyy-MM-dd}'", dtpFechaDesde.Value, dtpFechaHasta.Value.AddDays(1));
        }

        private void dtpFechaHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow != null)
                this.fuenteDatosKardex.Filter = String.Format("Fecha >= '{0:yyyy-MM-dd}' AND Fecha < '{1:yyyy-MM-dd}'", dtpFechaDesde.Value, dtpFechaHasta.Value.AddDays(1));
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvDetalle.DataSource != null)
                {
                    var path = string.Format("{0}{1}{2}{3}", GlobalClass.ConfiguracionGlobal.pathReportes, "Kardex_", DateTime.Now.ToString("s").Replace(":", "-"), ".csv");
                    Helper.OnExportGridToCSV(this.dgvDetalle, path);
                    var fileInfo = new FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
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
                    if (fuenteDatos != null)
                        fuenteDatos.Filter = filter;
                }
                else
                {
                    fuenteDatos.Filter = string.Empty;
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
