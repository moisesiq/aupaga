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
    public partial class DetalleAplicacion : DetalleBase
    {
        Parte Parte;
        BindingSource fuenteDatos;
        ControlError cntError = new ControlError();

        private enum aplicacionOperaciones
        {
            None = 0,
            Add = 1,
            Change = 2,
            Delete = 3
        }

        public static DetalleAplicacion Instance
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

            internal static readonly DetalleAplicacion instance = new DetalleAplicacion();
        }

        public DetalleAplicacion()
        {
            InitializeComponent();
        }

        public DetalleAplicacion(int Id)
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

        private void DetalleAplicacion_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
            this.Text = string.Format("{0}: {1}", "Aplicaciones de", Parte.NumeroParte);
        }

        private void DetalleAplicacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        private void cboMarca_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Helper.ConvertirEntero(cboMarca.SelectedValue) >= 0)
            {
                this.CargarModelos(Helper.ConvertirEntero(cboMarca.SelectedValue));
            }
        }

        private void cboMotor_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (int.TryParse(this.cboMotor.SelectedValue.ToString(), out id))
                {
                    this.CargarAnios(id);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvModelos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvModelos.SelectedRows.Count > 0)
            {
                this.CargarMotores(Helper.ConvertirEntero(this.dgvModelos.Rows[e.RowIndex].Cells["ModeloID"].Value));
                this.CargarAnios(this.dgvModelos.Rows[e.RowIndex].Cells["Anios"].Value.ToString());
            }
        }

        private void dgvModelos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvModelos.CurrentRow == null) return;
            var fila = this.dgvModelos.CurrentRow.Index;
            DataGridViewCellEventArgs ea = new DataGridViewCellEventArgs(0, fila);
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                dgvModelos_CellClick(sender, ea);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row;
                if (dgvModelos.SelectedRows.Count > 0)
                {
                    var dt = (DataTable)dgvSeleccion.DataSource;
                    int rowIndex = dgvModelos.SelectedCells[0].RowIndex;
                    
                    row = dt.NewRow();
                    row[0] = Parte.ParteID; //ParteID
                    row[1] = dgvModelos.Rows[rowIndex].Cells[0].Value; //ModeloID
                    row[2] = dgvModelos.Rows[rowIndex].Cells[3].Value; //NombreModelo
                    if (clbAnios.CheckedItems.Count > 0)
                    {
                        var anios = new List<string>();
                        foreach (object itemChecked in clbAnios.CheckedItems)
                        {
                            anios.Add(itemChecked.ToString());
                        }
                        row[4] = string.Join(", ", anios.ToArray()); //Anios
                    }

                    if (Helper.ConvertirEntero(this.cboMotor.SelectedValue) > 0)
                    {
                        row[3] = this.cboMotor.Text; //NombreMotor
                        row[5] = this.cboMotor.SelectedValue; //MotoresIDs
                    }

                    //Validar que no exista un valor identico en el grid, con las diferentes configuraciones posibles
                    foreach (DataRow fila in dt.Rows)
                    {
                        if (Helper.ConvertirCadena(fila["ModeloID"]) == Helper.ConvertirCadena(row[1])
                            && Helper.ConvertirCadena(fila["Anios"]) == Helper.ConvertirCadena(row[4])
                            && Helper.ConvertirCadena(fila["MotoresIDs"]) == Helper.ConvertirCadena(row[5]))
                        {
                            Helper.MensajeInformacion("Ya existe una fila con esa información.", GlobalClass.NombreApp);
                            return;
                        }
                    }

                    dt.Rows.Add(row);
                    this.btnNingunoAnios_Click(sender, null);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            var res = Negocio.Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
            if (res.Equals(DialogResult.No))
                return;

            SplashScreen.Show(new Splash());
            this.btnGuardar.Enabled = false;

            try
            {
                var listaSeleccion = new List<RelacionParteModelo>();
                foreach (var fila in dgvSeleccion.Rows)
                {
                    var aplicacion = (DataGridViewRow)fila;
                    var ids = new List<int>();
                    var anios = new List<int>();

                    if (!string.IsNullOrEmpty(aplicacion.Cells["MotoresIDs"].Value.ToString()))
                    {
                        var elements = aplicacion.Cells["MotoresIDs"].Value.ToString().Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string item in elements)
                        {
                            ids.Add(Helper.ConvertirEntero(item));
                        }
                    }

                    if (!string.IsNullOrEmpty(aplicacion.Cells["Anios"].Value.ToString()))
                    {
                        var elements = aplicacion.Cells["Anios"].Value.ToString().Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string item in elements)
                        {
                            anios.Add(Helper.ConvertirEntero(item));
                        }
                    }

                    var relacion = new RelacionParteModelo()
                    {
                        ParteID = Helper.ConvertirEntero(aplicacion.Cells["ParteID"].Value),
                        ModeloID = Helper.ConvertirEntero(aplicacion.Cells["ModeloID"].Value),
                        TipoFuenteID = Helper.ConvertirEntero(this.cboTipoFuente.SelectedValue),
                        MotorIDs = ids,
                        Anios = anios
                    };
                    listaSeleccion.Add(relacion);
                }

                //var aplicacionesActuales = General.GetListOf<ParteVehiculo>(p => p.ParteID.Equals(Parte.ParteID));
                foreach (var item in listaSeleccion)
                {
                    //Valida que MotorIDs y Anios tengan al menos un valor
                    if (item.Anios.Count > 0 && item.MotorIDs.Count > 0)
                    {
                        foreach (var motorId in item.MotorIDs)
                        {
                            foreach (var anio in item.Anios)
                            {
                                var parteVehiculo = new ParteVehiculo();

                                parteVehiculo = General.GetEntity<ParteVehiculo>(p => p.ParteID == Parte.ParteID
                                    && p.MotorID == motorId && p.Anio == anio && p.ModeloID == item.ModeloID);

                                if (parteVehiculo == null)
                                {
                                    parteVehiculo = new ParteVehiculo()
                                    {
                                        ParteID = item.ParteID,
                                        TipoFuenteID = item.TipoFuenteID,
                                        MotorID = motorId,
                                        ModeloID = item.ModeloID,
                                        Anio = anio,
                                        RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                        UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                        FechaRegistro = DateTime.Now,
                                        Estatus = true,
                                        Actualizar = true
                                    };
                                }
                                else
                                {
                                    parteVehiculo.ParteID = item.ParteID;
                                    parteVehiculo.TipoFuenteID = item.TipoFuenteID;
                                    parteVehiculo.MotorID = motorId;
                                    parteVehiculo.ModeloID = item.ModeloID;
                                    parteVehiculo.Anio = anio;
                                    parteVehiculo.RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                                    parteVehiculo.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                                    parteVehiculo.FechaRegistro = DateTime.Now;
                                    parteVehiculo.Estatus = true;
                                    parteVehiculo.Actualizar = true;
                                }
                                General.SaveOrUpdate<ParteVehiculo>(parteVehiculo, parteVehiculo);
                            }
                        }
                    }

                    //Valida que solo MotorIDs tenga valores
                    if (item.Anios.Count <= 0 && item.MotorIDs.Count > 0)
                    {
                        foreach (var motorId in item.MotorIDs)
                        {
                            var parteVehiculo = new ParteVehiculo();
                            parteVehiculo = General.GetEntity<ParteVehiculo>(p => p.ParteID == Parte.ParteID
                                && p.MotorID == motorId && p.Anio == null && p.ModeloID == item.ModeloID);

                            if (parteVehiculo == null)
                            {
                                parteVehiculo = new ParteVehiculo()
                                {
                                    ParteID = item.ParteID,
                                    TipoFuenteID = item.TipoFuenteID,
                                    MotorID = motorId,
                                    ModeloID = item.ModeloID,
                                    Anio = null,
                                    RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                    UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                    FechaRegistro = DateTime.Now,
                                    Estatus = true,
                                    Actualizar = true
                                };
                            }
                            else
                            {
                                parteVehiculo.ParteID = item.ParteID;
                                parteVehiculo.TipoFuenteID = item.TipoFuenteID;
                                parteVehiculo.MotorID = motorId;
                                parteVehiculo.ModeloID = item.ModeloID;
                                parteVehiculo.Anio = null;
                                parteVehiculo.RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                                parteVehiculo.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                                parteVehiculo.FechaRegistro = DateTime.Now;
                                parteVehiculo.Estatus = true;
                                parteVehiculo.Actualizar = true;
                            }
                            General.SaveOrUpdate<ParteVehiculo>(parteVehiculo, parteVehiculo);
                        }
                    }

                    //Valida que solo Anios tenga valores
                    if (item.Anios.Count > 0 && item.MotorIDs.Count <= 0)
                    {
                        foreach (var anio in item.Anios)
                        {
                            var parteVehiculo = new ParteVehiculo();

                            parteVehiculo = General.GetEntity<ParteVehiculo>(p => p.ParteID == Parte.ParteID
                                && p.MotorID == null && p.Anio == anio && p.ModeloID == item.ModeloID);

                            if (parteVehiculo == null)
                            {
                                parteVehiculo = new ParteVehiculo()
                                {
                                    ParteID = item.ParteID,
                                    TipoFuenteID = item.TipoFuenteID,
                                    MotorID = null,
                                    ModeloID = item.ModeloID,
                                    Anio = anio,
                                    RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                    UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                    FechaRegistro = DateTime.Now,
                                    Estatus = true,
                                    Actualizar = true
                                };
                            }
                            else
                            {
                                parteVehiculo.ParteID = item.ParteID;
                                parteVehiculo.TipoFuenteID = item.TipoFuenteID;
                                parteVehiculo.MotorID = null;
                                parteVehiculo.ModeloID = item.ModeloID;
                                parteVehiculo.Anio = anio;
                                parteVehiculo.RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                                parteVehiculo.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                                parteVehiculo.FechaRegistro = DateTime.Now;
                                parteVehiculo.Estatus = true;
                                parteVehiculo.Actualizar = true;
                            }
                            General.SaveOrUpdate<ParteVehiculo>(parteVehiculo, parteVehiculo);
                        }
                    }

                    //Valida que ninguno tenga valores
                    if (item.Anios.Count <= 0 && item.MotorIDs.Count <= 0)
                    {
                        var parteVehiculo = new ParteVehiculo();
                        parteVehiculo = General.GetEntity<ParteVehiculo>(p => p.ParteID == Parte.ParteID
                            && p.MotorID == null && p.Anio == null && p.ModeloID == item.ModeloID);

                        if (parteVehiculo == null)
                        {
                            parteVehiculo = new ParteVehiculo()
                            {
                                ParteID = item.ParteID,
                                TipoFuenteID = item.TipoFuenteID,
                                MotorID = null,
                                ModeloID = item.ModeloID,
                                Anio = null,
                                RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                                FechaRegistro = DateTime.Now,
                                Estatus = true,
                                Actualizar = true
                            };
                        }
                        else
                        {
                            parteVehiculo.ParteID = item.ParteID;
                            parteVehiculo.TipoFuenteID = item.TipoFuenteID;
                            parteVehiculo.MotorID = null;
                            parteVehiculo.ModeloID = item.ModeloID;
                            parteVehiculo.Anio = null;
                            parteVehiculo.RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                            parteVehiculo.UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                            parteVehiculo.FechaRegistro = DateTime.Now;
                            parteVehiculo.Estatus = true;
                            parteVehiculo.Actualizar = true;
                        }
                        General.SaveOrUpdate<ParteVehiculo>(parteVehiculo, parteVehiculo);
                    }
                }
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                new Notificacion("Aplicaciones Guardadas exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            }
            catch (Exception ex)
            {
                SplashScreen.Close();
                this.btnGuardar.Enabled = true;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
                    fuenteDatos.Filter = filter;
                }
                else
                {
                    fuenteDatos.Filter = string.Empty;                    
                }

                this.clbAnios.Items.Clear();
                this.cboMotor.Text = "";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39) //Simple Comma
            {
                e.Handled = true;
            }
        }

        private void btnTodosAnios_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbAnios.Items.Count; i++)
            {
                clbAnios.SetItemCheckState(i, CheckState.Checked);
            }
        }

        private void btnNingunoAnios_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbAnios.Items.Count; i++)
            {
                clbAnios.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void clbAnios_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                var valor = (CheckedListBox)sender;
                if (valor.GetItemCheckState(valor.SelectedIndex).Equals(CheckState.Unchecked))
                    valor.SetItemChecked(valor.SelectedIndex, true);
                else
                    valor.SetItemChecked(valor.SelectedIndex, false);
            }
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                this.txtBusqueda.Clear();
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvModelos.Focus();
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
                var listaFuentes = Negocio.General.GetListOf<TipoFuente>(t => t.Estatus.Equals(true));
                cboTipoFuente.DataSource = listaFuentes;
                cboTipoFuente.DisplayMember = "NombreTipoFuente";
                cboTipoFuente.ValueMember = "TipoFuenteID";
                AutoCompleteStringCollection autFuente = new AutoCompleteStringCollection();
                foreach (var listaFuente in listaFuentes) autFuente.Add(listaFuente.NombreTipoFuente);
                cboTipoFuente.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboTipoFuente.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboTipoFuente.AutoCompleteCustomSource = autFuente;
                cboTipoFuente.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                Marca iMarca = new Marca();
                iMarca.MarcaID = 0;
                iMarca.NombreMarca = "TODOS";
                iMarca.UsuarioID = 1;
                iMarca.FechaRegistro = DateTime.Now;
                iMarca.Estatus = true;
                iMarca.Actualizar = true;
                List<Marca> listaMarcas = new List<Marca>();

                listaMarcas.Add(iMarca);
                var listaDeMarcas = Negocio.General.GetListOf<Marca>(m => m.Estatus.Equals(true));
                foreach (var marca in listaDeMarcas)
                {
                    listaMarcas.Add(marca);
                }

                cboMarca.DisplayMember = "NombreMarca";
                cboMarca.ValueMember = "MarcaID";
                cboMarca.DataSource = listaMarcas;

                AutoCompleteStringCollection autMarca = new AutoCompleteStringCollection();
                foreach (var listaMarca in listaMarcas) autMarca.Add(listaMarca.NombreMarca);
                cboMarca.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboMarca.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cboMarca.AutoCompleteCustomSource = autMarca;
                cboMarca.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                this.ConfigurarGrid();

                this.CargarAplicacionesActuales(Parte.ParteID);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarModelos(int marcaId)
        {
            this.fuenteDatos = new BindingSource();
            var dt = new DataTable();
            try
            {
                if (marcaId.Equals(0))
                {
                    dt = Helper.newTable<ModelosView>("Modelos", Negocio.General.GetListOf<ModelosView>(m => m.ModeloID > 0));
                }
                else
                {
                    dt = Helper.newTable<ModelosView>("Modelos", Negocio.General.GetListOf<ModelosView>(m => m.MarcaID.Equals(marcaId)));
                }
                this.fuenteDatos.DataSource = dt;
                this.dgvModelos.DataSource = fuenteDatos;
                Helper.OcultarColumnas(this.dgvModelos, new string[] { "ModeloID", "MarcaID", "Busqueda", "EntityKey", "EntityState" });
                Helper.ColumnasToHeaderText(this.dgvModelos);
                this.dgvModelos.DefaultCellStyle.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarMotores(int modeloId)
        {
            try
            {
                var listaMotores = General.GetListOf<PartesVehiculosMotoresView>(m => m.ModeloID.Equals(modeloId));
                listaMotores.Insert(0, new PartesVehiculosMotoresView() { MotorID = 0, ModeloID = 0, MarcaID = 0, NombreMotor = "" });
                this.cboMotor.DataSource = listaMotores;
                this.cboMotor.DisplayMember = "NombreMotor";
                this.cboMotor.ValueMember = "MotorID";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarAnios(int motorId)
        {
            try
            {
                if (clbAnios.Items.Count > 0)
                    clbAnios.Items.Clear();

                if (motorId == 0)
                {                    
                    if (this.dgvModelos.CurrentRow != null)
                        this.CargarAnios(this.dgvModelos.Rows[dgvModelos.CurrentRow.Index].Cells["Anios"].Value.ToString());
                }
                else
                {
                    var anios = General.GetListOf<MotorAnio>(m => m.MotorID == motorId);
                    if (anios != null)
                    {
                        anios.Sort((x, y) => x.Anio.CompareTo(y.Anio));
                        anios.Reverse();
                        foreach (var anio in anios)
                        {
                            ((ListBox)clbAnios).Items.Add(anio.Anio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarAnios(string anios)
        {
            try
            {
                if (clbAnios.Items.Count > 0)
                    clbAnios.Items.Clear();

                if (string.IsNullOrEmpty(anios))
                    return;

                var arr = anios.Split('-');
                var anioInicial = Negocio.Helper.ConvertirEntero(arr[0].TrimEnd());
                var anioFinal = Negocio.Helper.ConvertirEntero(arr[1].Trim());

                if (anioInicial <= anioFinal)
                {
                    for (int i = anioFinal; i > anioInicial - 1; i--)
                    {
                        ((ListBox)clbAnios).Items.Add(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void ConfigurarGrid()
        {
            try
            {
                dgvSeleccion.DataSource = null;
                if (dgvSeleccion.Columns.Count > 0)
                    dgvSeleccion.Columns.Clear();

                if (dgvSeleccion.Rows.Count > 0)
                    dgvSeleccion.Rows.Clear();

                DataTable dt = new DataTable();

                var colParteId = new DataColumn();
                colParteId.DataType = System.Type.GetType("System.Int32");
                colParteId.ColumnName = "ParteID";

                var colModeloId = new DataColumn();
                colModeloId.DataType = System.Type.GetType("System.Int32");
                colModeloId.ColumnName = "ModeloID";

                var colModelo = new DataColumn();
                colModelo.DataType = Type.GetType("System.String");
                colModelo.ColumnName = "Nombre Modelo";

                var colAnios = new DataColumn();
                colAnios.DataType = Type.GetType("System.String");
                colAnios.ColumnName = "Anios";

                var colMotores = new DataColumn();
                colMotores.DataType = Type.GetType("System.String");
                colMotores.ColumnName = "Motores";

                var colMotoresIDs = new DataColumn();
                colMotoresIDs.DataType = Type.GetType("System.String");
                colMotoresIDs.ColumnName = "MotoresIDs";

                dt.Columns.AddRange(new DataColumn[] { colParteId, colModeloId, colModelo, colMotores, colAnios, colMotoresIDs });
                dgvSeleccion.DataSource = dt;

                //dgvSeleccion.Columns["ParteID"].Visible = false;
                //dgvSeleccion.Columns["ModeloID"].Visible = false;
                //dgvSeleccion.Columns["MotoresIDs"].Visible = false;
                Helper.OcultarColumnas(this.dgvSeleccion, new string[] { "ParteID", "ModeloID", "MotoresIDs" });

                this.dgvAplicaciones.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvAplicaciones.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void CargarAplicacionesActuales(int parteId)
        {
            try
            {
                this.dgvAplicaciones.DataSource = General.GetListOf<PartesVehiculosView>(e => e.ParteID.Equals(parteId));
                Helper.OcultarColumnas(this.dgvAplicaciones, new string[] { "GenericoID", "ParteID", "MotorID", "ModeloID" });
                Helper.ColumnasToHeaderText(this.dgvAplicaciones);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            if (this.dgvSeleccion.Rows.Count < 1)
                this.cntError.PonerError(this.dgvSeleccion, "Debe seleccionar al menos una aplicación.", ErrorIconAlignment.MiddleRight);
            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
