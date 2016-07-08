using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibUtil;
using TheosProc;

namespace Refaccionaria.App
{
    public partial class Migrador : Form
    {
        Parte Parte;
        public MigradorType oTipoMigrador { get; set; }

        public enum MigradorType
        {
            Alternos = 1,
            Equivalentes = 2,
            Aplicaciones = 3
        }

        public static Migrador Instance
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

            internal static readonly Migrador instance = new Migrador();
        }

        public Migrador()
        {
            InitializeComponent();
        }

        public Migrador(int Id)
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

        private void Migrador_Load(object sender, EventArgs e)
        {
            this.txtPath.Clear();
            this.Text = string.Format("{0}{1}", "Migrador de ", oTipoMigrador);
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtPath.Clear();
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.Title = string.Format("{0}{1}", "Migrador de ", oTipoMigrador);
                openfile.Filter = "Archivos de Excel|*.xlsx;*.xls";
                if (openfile.ShowDialog() == DialogResult.OK)
                {
                    this.txtPath.Text = openfile.FileName;
                }

                if (this.txtPath.Text.Length > 0)
                {
                    if (System.IO.File.Exists(this.txtPath.Text))
                    {
                        var cadena = string.Empty;
                        var ext = System.IO.Path.GetExtension(this.txtPath.Text);

                        switch (ext)
                        {
                            case ".xls": //Excel 97-03
                                cadena = string.Format("{0}{1}{2}", "Provider = Microsoft.jet.OLEDB.4.0; Data source=", this.txtPath.Text, ";Extended Properties=\"Excel 8.0;HDR=yes;\";");
                                break;

                            case ".xlsx": //Excel 07-
                                cadena = string.Format("{0}{1}{2}", "Provider = Microsoft.ACE.OLEDB.12.0; Data source=", this.txtPath.Text, ";Extended Properties=\"Excel 8.0;HDR=yes;\";");
                                break;
                        }

                        System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(cadena);

                        conn.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
                        var SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        conn.Close();

                        System.Data.OleDb.OleDbDataAdapter excelAdapter = new System.Data.OleDb.OleDbDataAdapter("Select * from [" + SheetName + "]", conn);
                        DataTable dtDatos = new DataTable();
                        excelAdapter.Fill(dtDatos);
                        this.dgvDatos.DataSource = dtDatos;
                    }

                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                switch (oTipoMigrador)
                {
                    case MigradorType.Alternos:

                        //Config. dt
                        var dtAlternos = new DataTable();

                        var colParteId = new DataColumn();
                        colParteId.DataType = System.Type.GetType("System.Int32");
                        colParteId.ColumnName = "ParteID";

                        var colMarcaParteId = new DataColumn();
                        colMarcaParteId.DataType = System.Type.GetType("System.Int32");
                        colMarcaParteId.ColumnName = "MarcaParteID";

                        var colCodigoAlterno = new DataColumn();
                        colCodigoAlterno.DataType = Type.GetType("System.String");
                        colCodigoAlterno.ColumnName = "CodigoAlterno";

                        dtAlternos.Columns.AddRange(new DataColumn[] { colParteId, colMarcaParteId, colCodigoAlterno });

                        //Obtengo los ids a partir de los nombres de las columnas
                        var listaMarcas = new List<int>();
                        foreach (DataGridViewColumn col in this.dgvDatos.Columns)
                        {
                            var marcaId = Util.Entero(col.Name);
                            if (marcaId > 0)
                            {
                                var existeMarca = Datos.GetEntity<MarcaParte>(m => m.MarcaParteID == marcaId);
                                if (existeMarca != null)
                                    listaMarcas.Add(marcaId);
                                else
                                    Util.MensajeError(string.Format("{0}{1}", "No existe una MarcaParte con el ID: ", marcaId), GlobalClass.NombreApp);
                            }
                        }

                        //Por cada marcaId, agrego un codigo alterno que tienen las celdas
                        DataRow fila;
                        foreach (var marcaId in listaMarcas)
                        {
                            foreach (DataGridViewRow row in this.dgvDatos.Rows)
                            {
                                var parteId = Util.Entero(row.Cells["PARTEID"].Value);
                                var codigos = row.Cells[marcaId.ToString()].Value.ToString().Split(',');
                                //Por cada codigo, se registra una fila en el dt
                                foreach (var codigo in codigos)
                                {
                                    if (!string.IsNullOrEmpty(codigo))
                                    {
                                        fila = dtAlternos.NewRow();
                                        fila["ParteID"] = parteId;
                                        fila["MarcaParteID"] = marcaId;
                                        fila["CodigoAlterno"] = Util.Cadena(codigo.Trim());
                                        dtAlternos.Rows.Add(fila);
                                    }
                                }
                            }
                        }

                        this.dgvProcesados.DataSource = dtAlternos;
                        break;

                    case MigradorType.Equivalentes:

                        //Config. dt
                        var dtEquivalentes = new DataTable();

                        var colParteIdOrigen = new DataColumn();
                        colParteIdOrigen.DataType = System.Type.GetType("System.Int32");
                        colParteIdOrigen.ColumnName = "ParteID";

                        var colNumeroParteOrigen = new DataColumn();
                        colNumeroParteOrigen.DataType = System.Type.GetType("System.String");
                        colNumeroParteOrigen.ColumnName = "NumeroParte";

                        var colNombreParteOrigen = new DataColumn();
                        colNombreParteOrigen.DataType = System.Type.GetType("System.String");
                        colNombreParteOrigen.ColumnName = "NombreParte";

                        var colLineaOrigen = new DataColumn();
                        colLineaOrigen.DataType = System.Type.GetType("System.String");
                        colLineaOrigen.ColumnName = "Linea";

                        var colParteIdDestino = new DataColumn();
                        colParteIdDestino.DataType = System.Type.GetType("System.Int32");
                        colParteIdDestino.ColumnName = "EquivalenteID";

                        var colNumeroParteDestino = new DataColumn();
                        colNumeroParteDestino.DataType = System.Type.GetType("System.String");
                        colNumeroParteDestino.ColumnName = "NumeroParteEquivalente";

                        var colNombreParteDestino = new DataColumn();
                        colNombreParteDestino.DataType = System.Type.GetType("System.String");
                        colNombreParteDestino.ColumnName = "NombreParteEquivalente";

                        var colLineaDestino = new DataColumn();
                        colLineaDestino.DataType = System.Type.GetType("System.String");
                        colLineaDestino.ColumnName = "LineaEquivalente";

                        dtEquivalentes.Columns.AddRange(new DataColumn[] { colParteIdOrigen, colNumeroParteOrigen, colNombreParteOrigen, colLineaOrigen, colParteIdDestino, colNumeroParteDestino, colNombreParteDestino, colLineaDestino });

                        DataRow filae;
                        foreach (DataGridViewRow row in this.dgvDatos.Rows)
                        {
                            filae = dtEquivalentes.NewRow();
                            var parteId = Util.Entero(row.Cells["PARTEID"].Value);
                            filae["ParteID"] = parteId;
                            filae["NumeroParte"] = Util.Cadena(row.Cells["CODIGO"].Value);
                            filae["NombreParte"] = Util.Cadena(row.Cells["DESCRIPCION"].Value);

                            var parteOrigen = Datos.GetEntity<Parte>(p => p.ParteID == parteId);
                            if (parteOrigen != null)
                            {
                                var lineaDestino = Datos.GetEntity<Linea>(l => l.LineaID == parteOrigen.LineaID);
                                filae["Linea"] = lineaDestino.NombreLinea;
                            }
                            else
                            {
                                filae["Linea"] = "";
                            }

                            var parteEquivalente = Util.Cadena(row.Cells["EQUIVALENTE"].Value);
                            if (!string.IsNullOrEmpty(parteEquivalente))
                            {
                                var listaParte = Datos.GetListOf<Parte>(p => p.NumeroParte == parteEquivalente).ToList();
                                if (listaParte != null)
                                {
                                    if (listaParte.Count > 0)
                                    {
                                        if (listaParte.Count == 1)
                                        {
                                            foreach (var parteDestino in listaParte)
                                            {
                                                filae["EquivalenteID"] = parteDestino.ParteID;
                                                filae["NumeroParteEquivalente"] = parteDestino.NumeroParte;
                                                filae["NombreParteEquivalente"] = parteDestino.NombreParte;

                                                var lineaDestino = Datos.GetEntity<Linea>(l => l.LineaID == parteDestino.LineaID);
                                                filae["LineaEquivalente"] = Util.Cadena(lineaDestino.NombreLinea);
                                                dtEquivalentes.Rows.Add(filae);
                                            }
                                        }
                                        else if (listaParte.Count > 1)
                                        {
                                            var seleccion = DetalleEquivalenteSeleccion.Instance;
                                            seleccion.NumeroParte = parteEquivalente;
                                            seleccion.ShowDialog();
                                            if (seleccion.Seleccionado)
                                            {
                                                if (seleccion.Sel != null)
                                                {
                                                    foreach (var val in seleccion.Sel)
                                                    {
                                                        var parte = (PartesView)val.Value;
                                                        filae["EquivalenteID"] = parte.ParteID;
                                                        filae["NumeroParteEquivalente"] = parte.NumeroDeParte;
                                                        filae["NombreParteEquivalente"] = parte.Descripcion;

                                                        var lineaDestino = Datos.GetEntity<Linea>(l => l.LineaID == parte.LineaID);
                                                        filae["LineaEquivalente"] = Util.Cadena(lineaDestino.NombreLinea);
                                                        dtEquivalentes.Rows.Add(filae);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        this.dgvProcesados.DataSource = dtEquivalentes;
                        break;

                    case MigradorType.Aplicaciones:
                        //Config. dt
                        var dtAplicaciones = new DataTable();

                        var colParteAppId = new DataColumn();
                        colParteAppId.DataType = System.Type.GetType("System.Int32");
                        colParteAppId.ColumnName = "ParteID";

                        var colModeloId = new DataColumn();
                        colModeloId.DataType = System.Type.GetType("System.Int32");
                        colModeloId.ColumnName = "ModeloID";

                        var colMotorId = new DataColumn();
                        colMotorId.DataType = Type.GetType("System.String");
                        colMotorId.ColumnName = "MotorID";

                        var colAnio = new DataColumn();
                        colAnio.DataType = Type.GetType("System.String");
                        colAnio.ColumnName = "Anio";

                        dtAplicaciones.Columns.AddRange(new DataColumn[] { colParteAppId, colModeloId, colMotorId, colAnio });

                        foreach (DataGridViewRow row in this.dgvDatos.Rows)
                        {
                            var parteId = Util.Entero(row.Cells["ParteId"].Value);
                            var modeloId = Util.Entero(row.Cells["ModeloId"].Value);
                            var motorId = Util.Entero(row.Cells["MotorId"].Value);
                            var anio = Util.Entero(row.Cells["AÑO"].Value);

                            var parte = Datos.GetEntity<Parte>(p => p.ParteID == parteId);
                            if (parte != null)
                            {
                                var modelo = Datos.GetEntity<Modelo>(m => m.ModeloID == modeloId);
                                if (modelo != null)
                                {
                                    fila = dtAplicaciones.NewRow();
                                    fila["ParteID"] = parteId;
                                    fila["ModeloID"] = modeloId;
                                    fila["MotorId"] = motorId > 0 ? motorId.ToString() : string.Empty;
                                    fila["Anio"] = anio > 0 ? anio.ToString() : string.Empty;
                                    dtAplicaciones.Rows.Add(fila);
                                }
                            }
                        }
                        this.dgvProcesados.DataSource = dtAplicaciones;
                        break;

                    default:
                        break;
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvProcesados.Rows.Count < 1)
                    return;

                var res = Util.MensajePregunta("Esta seguro de que la información a migrar es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                Cursor.Current = Cursors.WaitCursor;

                switch (oTipoMigrador)
                {
                    case MigradorType.Alternos:

                        foreach (DataGridViewRow row in this.dgvProcesados.Rows)
                        {
                            var parteId = Util.Entero(row.Cells["ParteID"].Value);
                            var marcaParteId = Util.Entero(row.Cells["MarcaParteID"].Value);
                            var codigoAlterno = Util.Cadena(row.Cells["CodigoAlterno"].Value);
                            this.AlmacenarCodigoAlterno(parteId, marcaParteId, codigoAlterno);
                        }

                        new Notificacion("El proceso de migración finalizó correctamente.", 2 * 1000).Mostrar(Principal.Instance);
                        this.Close();
                        break;

                    case MigradorType.Equivalentes:

                        foreach (DataGridViewRow row in this.dgvProcesados.Rows)
                        {
                            var parteId = Util.Entero(row.Cells["ParteID"].Value);
                            var parteIdEquivalente = Util.Entero(row.Cells["EquivalenteID"].Value);
                            this.AlmacenarEquivalente(parteId, parteIdEquivalente);
                        }

                        new Notificacion("El proceso de migración finalizo correctamente.", 2 * 1000).Mostrar(Principal.Instance);
                        this.Close();
                        break;

                    case MigradorType.Aplicaciones:
                        foreach (DataGridViewRow row in this.dgvProcesados.Rows)
                        {
                            //Validar si existen ParteID, ModeloID y MotorID
                            var parteId = Util.Entero(row.Cells["ParteID"].Value);
                            var modeloId = Util.Entero(row.Cells["ModeloID"].Value);
                            var motorId = Util.Entero(row.Cells["MotorID"].Value);
                            var anio = Util.Entero(row.Cells["Anio"].Value);
                            
                            //Valida la existencia de motor y anio
                            if (motorId > 0 && anio > 0)
                            {
                                var existeParteVehiculo = Datos.GetEntity<ParteVehiculo>(p => p.ParteID == parteId
                                    && p.ModeloID == modeloId && p.MotorID == motorId && p.Anio == anio);
                                if (existeParteVehiculo == null)
                                {
                                    var parteVehiculo = new ParteVehiculo()
                                    {
                                        ParteID = parteId,
                                        ModeloID = modeloId,
                                        MotorID = motorId,
                                        Anio = anio,
                                        TipoFuenteID = 1,
                                        RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                                    };
                                    Datos.Guardar<ParteVehiculo>(parteVehiculo);
                                }
                            }
                            else if (motorId > 0 && anio <= 0) //Valida solo la existencia de motor
                            {
                                var existeParteVehiculo = Datos.GetEntity<ParteVehiculo>(p => p.ParteID == parteId
                                    && p.ModeloID == modeloId && p.MotorID == motorId);
                                if (existeParteVehiculo == null)
                                {
                                    var parteVehiculo = new ParteVehiculo()
                                    {
                                        ParteID = parteId,
                                        ModeloID = modeloId,
                                        MotorID = motorId,
                                        TipoFuenteID = 1,
                                        RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                                    };
                                    Datos.Guardar<ParteVehiculo>(parteVehiculo);
                                }
                            }
                            else if (motorId <= 0 && anio > 0) //Valida solo la existencia de anio
                            {
                                var existeParteVehiculo = Datos.GetEntity<ParteVehiculo>(p => p.ParteID == parteId
                                    && p.ModeloID == modeloId && p.Anio == anio);
                                if (existeParteVehiculo == null)
                                {
                                    var parteVehiculo = new ParteVehiculo()
                                    {
                                        ParteID = parteId,
                                        ModeloID = modeloId,
                                        Anio = anio,
                                        TipoFuenteID = 1,
                                        RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                                    };
                                    Datos.Guardar<ParteVehiculo>(parteVehiculo);
                                }
                            }
                        }
                        new Notificacion("El proceso de migración finalizo correctamente.", 2 * 1000).Mostrar(Principal.Instance);
                        this.Close();
                        break;

                    default:
                        break;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Metodos ]

        private void AlmacenarCodigoAlterno(int parteId, int marcaParteId, string codigoAlterno)
        {
            try
            {
                var existe = Datos.GetEntity<ParteCodigoAlterno>(p => p.ParteID == parteId && p.MarcaParteID == marcaParteId && p.CodigoAlterno == codigoAlterno);
                if (existe == null)
                {
                    var codigo = new ParteCodigoAlterno()
                    {
                        ParteID = parteId,
                        MarcaParteID = marcaParteId,
                        CodigoAlterno = codigoAlterno,
                        RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                    };
                    Datos.Guardar<ParteCodigoAlterno>(codigo);
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void AlmacenarEquivalente(int parteId, int equivalenteId)
        {
            try
            {
                /* var existe = General.GetEntity<ParteEquivalente>(p => p.ParteID == parteId && p.ParteEquivalenteID == equivalenteId);
                if (existe == null)
                {
                    var equivalente = new ParteEquivalente()
                    {
                        ParteID = parteId,
                        ParteIDequivalente = equivalenteId
                    };
                    Datos.Guardar<ParteEquivalente>(equivalente);
                }

                var existeInverso = General.GetEntity<ParteEquivalente>(p => p.ParteID == equivalenteId && p.ParteEquivalenteID == parteId);
                if (existeInverso == null)
                {
                    var equivalenteInverso = new ParteEquivalente()
                    {
                        ParteID = equivalenteId,
                        ParteIDequivalente = parteId
                    };
                    Datos.Guardar<ParteEquivalente>(equivalenteInverso);
                }
                */

                // Nueva modalidad de equivalentes - Moisés
                Guardar.ParteEquivalencia(parteId, equivalenteId);
                //
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

    }
}

