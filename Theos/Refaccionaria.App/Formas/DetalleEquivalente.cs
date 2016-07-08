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
    public partial class DetalleEquivalente : DetalleBase
    {
        Parte Parte;
        BindingSource fuenteDatos;
        ControlError cntError = new ControlError();

        private enum equivalenteOperaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        private static DetalleEquivalente aForm = null;

        public static DetalleEquivalente Instance()
        {
            if (aForm == null)
            {
                aForm = new DetalleEquivalente();
            }
            return aForm;
        }

        public DetalleEquivalente()
        {
            InitializeComponent();
        }

        public DetalleEquivalente(int Id)
        {
            InitializeComponent();
            try
            {
                //Parte = General.GetEntityById<Parte>("Parte", "ParteID", Id);
                Parte = Datos.GetEntity<Parte>(c => c.ParteID == Id && c.Estatus);
                if (Parte == null)
                    throw new EntityNotFoundException(Id.ToString(), "Parte");

                //ParteEquivalente = General.GetEntityById<ParteEquivalente>("ParteEquivalente", "ParteEquivalenteID", Parte.ParteID);                                
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [eventos]

        private void DetalleEquivalente_Load(object sender, EventArgs e)
        {
            CargaInicial();
            this.Text = string.Format("{0}: {1}", "Equivalentes de", Parte.NumeroParte);
        }

        private void DetalleEquivalente_KeyDown(object sender, KeyEventArgs e)
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

            var res = Util.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
            if (res.Equals(DialogResult.No))
                return;

            SplashScreen.Show(new Splash());
            this.btnGuardar.Enabled = false;

            try
            {
                /*
                var listaSeleccion = new List<int>();
                listaSeleccion.Add(Parte.ParteID);
                foreach (var fila in dgvSeleccion.Rows)
                {
                    var equivalente = (DataGridViewRow)fila;
                    listaSeleccion.Add(Util.ConvertirEntero(equivalente.Cells["ParteID"].Value));
                }
                //Almacenar: Todos contra todos
                foreach (var parteId in listaSeleccion)
                {
                    foreach (var parteIdComparar in listaSeleccion)
                    {
                        var equivalente = General.GetEntity<ParteEquivalente>(p => p.ParteID == parteId && p.ParteIDequivalente == parteIdComparar);
                        if (equivalente == null)
                        {
                            var equivalencia = new ParteEquivalente() 
                            { 
                                ParteID = parteId,
                                ParteIDequivalente = parteIdComparar
                            };
                            Datos.Guardar<ParteEquivalente>(equivalencia);
                        }
                    }
                }
                */

                // Nueva modalidad de equivalentes - Moisés
                int iParteID = this.Parte.ParteID;
                foreach (DataGridViewRow oFila in this.dgvSeleccion.Rows)
                {
                    int iParteIDEq = Util.Entero(oFila.Cells["ParteID"].Value);
                    Guardar.ParteEquivalencia(iParteID, iParteIDEq);
                }
                //
                
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                new Notificacion("Equivalentes guardados exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
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
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void cboLinea_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Util.Entero(cboLinea.SelectedValue) > 0)
            {
                CargarEquivalenciasSimilares(Util.Entero(cboLinea.SelectedValue));
                txtBusqueda.Clear();
            }
        }

        private void dgvSimilares_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            try
            {
                var fila = e.RowIndex;
                var parteId = Util.Entero(dgvSimilares.Rows[fila].Cells["ParteID"].Value);
                var numeroParte = dgvSimilares.Rows[fila].Cells["NumeroDeParte"].Value;
                var descripcion = dgvSimilares.Rows[fila].Cells["Descripcion"].Value;

                var dt = (DataTable)dgvSeleccion.DataSource;

                foreach (DataRow numero in dt.Rows)
                {
                    var cadena = numero["NumeroParte"].ToString();
                    if (cadena.Equals(numeroParte))
                        return;
                }

                DataRow row = dt.NewRow();
                row[0] = parteId;
                row[1] = numeroParte;
                row[2] = descripcion;
                dt.Rows.Add(row);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvSimilares_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvSimilares.CurrentRow == null) return;
            if (e.KeyCode == Keys.Enter)
            {
                var dg = (DataGridView)sender;
                var fila = dg.SelectedRows[0].Index;
                DataGridViewCellEventArgs ea = new DataGridViewCellEventArgs(0, fila);
                dgvSimilares_CellDoubleClick(sender, ea);
            }
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtBusqueda.Clear();
            }
        }

        #endregion

        #region [metodos]

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
                var listaLineas = Datos.GetListOf<Linea>(l => l.Estatus.Equals(true));
                cboLinea.DataSource = listaLineas;
                cboLinea.DisplayMember = "NombreLinea";
                cboLinea.ValueMember = "LineaID";
                AutoCompleteStringCollection autLinea = new AutoCompleteStringCollection();
                foreach (var listaLinea in listaLineas) autLinea.Add(listaLinea.NombreLinea);
                cboLinea.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboLinea.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboLinea.AutoCompleteCustomSource = autLinea;
                cboLinea.TextUpdate += new EventHandler(UtilLocal.cboCharacterCasingUpper);
                cboLinea.SelectedValue = Parte.LineaID;

                CargarEquivalenciasSimilares(Parte.LineaID);

                ConfigurarSeleccion();

                CargarEquivalenciasActuales(Parte.ParteID);

            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarEquivalenciasSimilares(int lineaId)
        {
            this.fuenteDatos = new BindingSource();
            try
            {
                var dt = UtilLocal.newTable<PartesView>("Partes", Datos.GetListOf<PartesView>(p => p.LineaID.Equals(lineaId) && !p.ParteID.Equals(Parte.ParteID)));
                this.fuenteDatos.DataSource = dt;
                this.dgvSimilares.DataSource = fuenteDatos;
                Util.OcultarColumnas(this.dgvSimilares, new string[] { "ParteID", "ParteEstatusID", "LineaID", "Aplicacion", "Equivalente", "Busqueda", "EntityState", "EntityKey", "FaltanCaracteristicas" });
                UtilLocal.ColumnasToHeaderText(this.dgvSimilares);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarEquivalenciasActuales(int parteId)
        {
            this.dgvEquivalentes.DataSource = Datos.GetListOf<PartesEquivalentesView>(e => e.ParteID.Equals(parteId));
            Util.OcultarColumnas(this.dgvEquivalentes, new string[] { "ParteEquivalenteID", "ParteID", "ParteIDequivalente", "NombreImagen" });
            UtilLocal.ColumnasToHeaderText(this.dgvEquivalentes);
        }

        private void ConfigurarSeleccion()
        {
            if (dgvSeleccion.Columns.Count > 0)
                dgvSeleccion.Columns.Clear();

            if (dgvSeleccion.Rows.Count > 0)
                dgvSeleccion.Rows.Clear();

            DataTable dt = new DataTable();

            var colParteId = new DataColumn();
            colParteId.DataType = System.Type.GetType("System.Int32");
            colParteId.ColumnName = "ParteID";

            var colNumeroParte = new DataColumn();
            colNumeroParte.DataType = System.Type.GetType("System.String");
            colNumeroParte.ColumnName = "NumeroParte";

            var colDescripcion = new DataColumn();
            colDescripcion.DataType = System.Type.GetType("System.String");
            colDescripcion.ColumnName = "Descripcion";

            dt.Columns.AddRange(new DataColumn[] { colParteId, colNumeroParte, colDescripcion });
            dgvSeleccion.DataSource = dt;

            Util.OcultarColumnas(this.dgvSeleccion, new string[] { "ParteID" });
            UtilLocal.ColumnasToHeaderText(this.dgvSeleccion);
        }

        /* Al parecer ya no se usa - Moisés
        private static void UpdateEquivalentes(int ParteId, IEnumerable<int> values)
        {
            try
            {
                var equivalentesActuales = General.GetListOf<ParteEquivalente>(e => e.ParteID.Equals(ParteId));
                var selectedValues = new Dictionary<int, int>();

                foreach (var item in values)
                {
                    selectedValues.Add(item, (int)equivalenteOperaciones.Add);
                }

                foreach (var item in equivalentesActuales)
                {
                    if (selectedValues.ContainsKey(item.ParteIDequivalente))
                    {
                        selectedValues[item.ParteIDequivalente] = (int)equivalenteOperaciones.None;
                    }
                    else
                    {
                        selectedValues[item.ParteIDequivalente] = (int)equivalenteOperaciones.Delete;
                    }
                }

                foreach (var item in selectedValues)
                {
                    if (item.Value == (int)equivalenteOperaciones.Add) //add new
                    {
                        var parteEquivalente = new ParteEquivalente
                         {
                             ParteID = ParteId,
                             ParteIDequivalente = item.Key,
                             UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                             FechaRegistro = DateTime.Now,
                             Estatus = true,
                             Actualizar = true
                         };
                        General.SaveOrUpdate<ParteEquivalente>(parteEquivalente, parteEquivalente);

                        var existeInverso = General.GetEntity<ParteEquivalente>(p => p.ParteID == item.Key && p.ParteIDequivalente == ParteId);
                        if (existeInverso == null)
                        {
                            var equivalenteInverso = new ParteEquivalente() 
                            {
                                ParteID = item.Key,
                                ParteIDequivalente = ParteId
                            };
                            Datos.Guardar<ParteEquivalente>(equivalenteInverso);
                        }
                    }
                    else if (item.Value == (int)equivalenteOperaciones.Delete) //search and delete
                    {
                        //var parteEquivalente = General.GetEntity<ParteEquivalente>(p => p.ParteID.Equals(ParteId) && p.ParteIDequivalente.Equals(item.Key));
                        //if (parteEquivalente != null)
                        //    General.Delete<ParteEquivalente>(parteEquivalente);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        */

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            if (this.dgvSeleccion.Rows.Count < 1)
                this.cntError.PonerError(this.dgvSeleccion, "Debe seleccionar al menos un equivalente.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
