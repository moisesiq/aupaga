using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Master : UserControl
    {
        public Master()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void Master_Load(object sender, EventArgs e)
        {
            // Se cargan los combos
            // this.cmbProveedor.CargarDatos("ProveedorID", "NombreProveedor", General.GetListOf<Proveedor>(q => q.Estatus));
            var oProveedores = General.GetListOf<Proveedor>(c => c.Estatus).OrderBy(c => c.NombreProveedor).ToList();
            foreach (var oReg in oProveedores)
                this.ctlProveedores.AgregarElemento(oReg.ProveedorID, oReg.NombreProveedor);
            // this.cmbMarca.CargarDatos("MarcaParteID", "NombreMarcaParte", General.GetListOf<MarcaParte>(q => q.Estatus));
            var oMarcas = General.GetListOf<MarcaParte>(c => c.Estatus).OrderBy(c => c.NombreMarcaParte).ToList();
            foreach (var oReg in oMarcas)
                this.ctlMarcas.AgregarElemento(oReg.MarcaParteID, oReg.NombreMarcaParte);
            // this.cmbLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(q => q.Estatus));
            var oLineas = General.GetListOf<Linea>(c => c.Estatus).OrderBy(c => c.NombreLinea).ToList();
            foreach (var oReg in oLineas)
                this.ctlLineas.AgregarElemento(oReg.LineaID, oReg.NombreLinea);
            // Se cargan los combos del grid
            this.ProveedorID.ValueMember = "ProveedorID";
            this.ProveedorID.DisplayMember = "NombreProveedor";
            this.ProveedorID.DataSource = oProveedores;
            this.MarcaID.ValueMember = "MarcaParteID";
            this.MarcaID.DisplayMember = "NombreMarcaParte";
            this.MarcaID.DataSource = oMarcas;
            this.LineaID.ValueMember = "LineaID";
            this.LineaID.DisplayMember = "NombreLinea";
            this.LineaID.DataSource = oLineas;
            this.UnidadDeMedidaID.ValueMember = "MedidaID";
            this.UnidadDeMedidaID.DisplayMember = "NombreMedida";
            this.UnidadDeMedidaID.DataSource = General.GetListOf<Medida>(q => q.Estatus);
            // Se ajustan los tipos de datos del grid
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Name == "UnidadDeEmpaque" || oCol.Name == "Costo" || oCol.Name.StartsWith("Por") || oCol.Name.StartsWith("Precio"))
                    oCol.ValueType = typeof(decimal);
            }
        }

        private void txtFiltrarNumeroDeParte_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Cargando.Mostrar();
                this.dgvDatos.FiltrarContiene(this.txtFiltrarNumeroDeParte.Text, "NumeroDeParte");
                Cargando.Cerrar();
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            this.GuardarDatos();
        }

        private void btnEtiquetas_Click(object sender, EventArgs e)
        {
            var frmOpciones = new ObtenerElementoLista();
            frmOpciones.lblMensaje.Text = "¿Cuál Etiqueta deseas utilizar?";
            frmOpciones.dgvDatos.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Reporte", HeaderText = "Reporte" });
            frmOpciones.dgvDatos.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Archivo", HeaderText = "Archivo", Visible = false });

            var oReportes = Directory.GetFiles(GlobalClass.ConfiguracionGlobal.pathReportes, "ReporteEtiquetas*.frx");
            foreach (string sReporte in oReportes)
            {
                string sNombre = Path.GetFileNameWithoutExtension(sReporte);
                sNombre = sNombre.Replace("ReporteEtiquetas", "");
                if (sNombre == "") sNombre = "(Original)";
                frmOpciones.dgvDatos.Rows.Add(sNombre, sReporte);
            }

            if (frmOpciones.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                this.ImprimirEtiquetas(Helper.ConvertirCadena(frmOpciones.Sel["Archivo"]));
            }
            frmOpciones.Dispose();
        }

        private void dgvDatos_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (this.dgvDatos.CurrentCell == null) return;

            // Se desenfoca la celda de "Procesar" (si aplica), para que tome el cambio de esa celda
            if (this.dgvDatos.CurrentCell.OwningColumn.Name == "Procesar" && this.dgvDatos.CurrentCell.IsInEditMode)
                this.dgvDatos.EndEdit();

            // Se procesa una acción, dependiendo de la columna seleccionada
            string sColumna = this.dgvDatos.Columns[e.ColumnIndex].Name;
            switch (sColumna)
            {
                case "Procesar":
                case "Comision":
                case "Servicio":
                case "Etiqueta":
                case "SoloUna":
                case "EsPar":
                    this.AplicarCheck(sColumna);
                    break;
                case "NumeroDeParte":
                case "Descripcion":
                    this.BuscarReemplazar(sColumna);
                    break;
                case "ProveedorID":
                case "MarcaID":
                case "LineaID":
                case "UnidadDeMedidaID":
                    DataGridViewComboBoxColumn oCol;
                    if (sColumna == "ProveedorID")
                        oCol = this.ProveedorID;
                    else if (sColumna == "MarcaID")
                        oCol = this.MarcaID;
                    else if (sColumna == "LineaID")
                        oCol = this.LineaID;
                    else
                        oCol = this.UnidadDeMedidaID;

                    this.AplicarCombo(sColumna, oCol.ValueMember, oCol.DisplayMember, oCol.DataSource);
                    break;
                case "UnidadDeEmpaque":
                case "TiempoDeReposicion":
                    string sMensaje = (sColumna == "UnidadDeEmpaque" ? "Indica la Unidad de Empaque:" : "Indica el tiempo de reposición:");
                    var oValor = UtilLocal.ObtenerValor(sMensaje, "1", MensajeObtenerValor.Tipo.Decimal);
                    if (oValor != null)
                        this.AplicarCambio(sColumna, oValor);
                    break;
                case "Costo":
                case "Precio1":
                case "Precio2":
                case "Precio3":
                case "Precio4":
                case "Precio5":
                    this.AplicarPrecio(sColumna);
                    break;
                case "Por1":
                case "Por2":
                case "Por3":
                case "Por4":
                case "Por5":
                    this.AplicarPorcentajes();
                    break;
            }
        }

        private void dgvDatos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            this.MarcarCambioCelda(this.dgvDatos[e.ColumnIndex, e.RowIndex]);
        }
                
        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            var oParams = new Dictionary<string, object>();
            
            DateTime dHasta = DateTime.Now.Date.DiaPrimero().AddDays(-1);
            oParams.Add("Hasta", dHasta);
            oParams.Add("Desde", dHasta.AddYears(-1).AddDays(1));
            // if (this.cmbProveedor.SelectedValue != null)
            //     oParams.Add("ProveedorID", Helper.ConvertirEntero(this.cmbProveedor.SelectedValue));
            if (this.ctlProveedores.ValoresSeleccionados.Count > 0)
            {
                var oDtProveedores = Helper.ListaEntityADataTable(this.ctlProveedores.ElementosSeleccionados);
                oDtProveedores.Columns.Remove("Cadena");
                oParams.Add("Proveedores/tpuTablaEnteros", oDtProveedores);
            }
            // if (this.cmbMarca.SelectedValue != null)
            //     oParams.Add("MarcaID", Helper.ConvertirEntero(this.cmbMarca.SelectedValue));
            if (this.ctlMarcas.ValoresSeleccionados.Count > 0)
            {
                var oDtMarcas = Helper.ListaEntityADataTable(this.ctlMarcas.ElementosSeleccionados);
                oDtMarcas.Columns.Remove("Cadena");
                oParams.Add("Marcas/tpuTablaEnteros", oDtMarcas);
            }
            // if (this.cmbLinea.SelectedValue != null)
            //     oParams.Add("LineaID", Helper.ConvertirEntero(this.cmbLinea.SelectedValue));
            if (this.ctlLineas.ValoresSeleccionados.Count > 0)
            {
                var oDtLineas = Helper.ListaEntityADataTable(this.ctlLineas.ElementosSeleccionados);
                oDtLineas.Columns.Remove("Cadena");
                oParams.Add("Lineas/tpuTablaEnteros", oDtLineas);
            }
            //
            var oDatos = General.ExecuteProcedure<pauPartesMaster_Result>("pauPartesMaster", oParams);
 
            int iSC = Cat.TiposDeAfectacion.SinCambios;
            this.dgvDatos.Rows.Clear();
            foreach (var oParte in oDatos)
            {
                this.dgvDatos.Rows.Add(oParte.ParteID, iSC, iSC, true
                    , oParte.NumeroDeParte, oParte.Descripcion, oParte.ProveedorID, oParte.LineaID, oParte.MarcaParteID
                    , oParte.MedidaID, oParte.UnidadDeEmpaque, oParte.TiempoDeReposicion
                    , oParte.AplicaComision, oParte.EsServicio, oParte.Etiqueta, oParte.SoloUnaEtiqueta, oParte.EsPar
                    , oParte.CodigoDeBara, oParte.Existencia, oParte.Ventas, oParte.Costo, oParte.CostoConDescuento
                    , oParte.PorcentajeUtilidadUno, oParte.PrecioUno, oParte.PrecioUno
                    , oParte.PorcentajeUtilidadDos, oParte.PrecioDos, oParte.PrecioDos
                    , oParte.PorcentajeUtilidadTres, oParte.PrecioTres, oParte.PrecioTres
                    , oParte.PorcentajeUtilidadCuatro, oParte.PrecioCuatro, oParte.PrecioCuatro
                    , oParte.PorcentajeUtilidadCinco, oParte.PrecioCinco, oParte.PrecioCinco
                );
            }

            Cargando.Cerrar();    
        }

        private void GuardarDatos()
        {
            // Se solicita la autorización
            var oResU = UtilDatos.ValidarObtenerUsuario("Administracion.Master.Modificar");
            if (oResU.Error)
                return;
            int iUsuarioID = oResU.Respuesta.UsuarioID;

            // Se muestra progreso de avance
            this.IniciarTerminarProgreso(true);

            // Se inicia el proceso de guardado
            ModelHelper.StartPersistentContext();
            int iErroresVal = 0;
            var oCambios = new List<ParteCambio>();
            var oVarCostos = new Dictionary<int, decimal>();
            foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
            {
                this.pgrGuardar.EjecutarPaso(true);
                if (!Fila.Visible) continue;
                if (!Helper.ConvertirBool(Fila.Cells["Procesar"].Value)) continue;
                
                int iParteID = Helper.ConvertirEntero(Fila.Cells["ParteID"].Value);
                bool bCambioParte = (Helper.ConvertirEntero(Fila.Cells["CambioParte"].Value) == Cat.TiposDeAfectacion.Modificar);
                bool bCambioPrecio = (Helper.ConvertirEntero(Fila.Cells["CambioPrecio"].Value) == Cat.TiposDeAfectacion.Modificar);

                // Se valida el Proveedor - Marca - Línea
                int iProveedorID = 0, iMarcaID = 0, iLineaID = 0;
                if (bCambioParte || bCambioPrecio)
                {
                    iProveedorID = Helper.ConvertirEntero(Fila.Cells["ProveedorID"].Value);
                    iMarcaID = Helper.ConvertirEntero(Fila.Cells["MarcaID"].Value);
                    iLineaID = Helper.ConvertirEntero(Fila.Cells["LineaID"].Value);
                    if (!General.Exists<ProveedorMarcaParte>(q => q.ProveedorID == iProveedorID && q.MarcaParteID == iMarcaID && q.Estatus)
                        || !General.Exists<LineaMarcaParte>(q => q.MarcaParteID == iMarcaID && q.LineaID == iLineaID && q.Estatus))
                    {
                        Fila.ErrorText = "Proveedor - Marca - Línea inválido.";
                        // Fila.DefaultCellStyle.ForeColor = Color.Red;
                        iErroresVal++;
                        continue;
                    }
                }

                // Se guardan los datos de la parte, si aplica
                if (bCambioParte)
                {
                    var oParte = General.GetEntity<Parte>(q => q.ParteID == iParteID && q.Estatus);
                    string sNumeroDeParte = Helper.ConvertirCadena(Fila.Cells["NumeroDeParte"].Value);
                    string sDescripcion = Helper.ConvertirCadena(Fila.Cells["Descripcion"].Value);
                    int iUnidadDeMedidaID = Helper.ConvertirEntero(Fila.Cells["UnidadDeMedidaID"].Value);
                    decimal mUnidadDeEmpaque = Helper.ConvertirDecimal(Fila.Cells["UnidadDeEmpaque"].Value);
                    decimal mTiempoDeReposicion = Helper.ConvertirDecimal(Fila.Cells["TiempoDeReposicion"].Value);
                    bool bComision = Helper.ConvertirBool(Fila.Cells["Comision"].Value);
                    bool bServicio = Helper.ConvertirBool(Fila.Cells["Servicio"].Value);
                    bool bEtiqueta = Helper.ConvertirBool(Fila.Cells["Etiqueta"].Value);
                    bool bSoloUna = Helper.ConvertirBool(Fila.Cells["SoloUna"].Value);
                    bool bEsPar = Helper.ConvertirBool(Fila.Cells["EsPar"].Value);
                    string sCodigoDeBarras = Helper.ConvertirCadena(Fila.Cells["CodigoDeBarra"].Value);

                    // Para agregar registro de cambios
                    if (Helper.ConvertirCadena(oParte.NumeroParte) != sNumeroDeParte)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.NumeroDeParte, oParte.NumeroParte, sNumeroDeParte);
                        oParte.NumeroParte = sNumeroDeParte;
                    }
                    if (Helper.ConvertirCadena(oParte.NombreParte) != sDescripcion)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Descripcion, oParte.NombreParte, sDescripcion);
                        oParte.NombreParte = sDescripcion;
                    }
                    if (oParte.ProveedorID != iProveedorID)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Proveedor, oParte.ProveedorID.ToString(), iProveedorID.ToString());
                        oParte.ProveedorID = iProveedorID;
                    }
                    if (oParte.LineaID != iLineaID)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Linea, oParte.LineaID.ToString(), iLineaID.ToString());
                        oParte.LineaID = iLineaID;
                    }
                    if (oParte.MarcaParteID != iMarcaID)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Marca, oParte.MarcaParteID.ToString(), iMarcaID.ToString());
                        oParte.MarcaParteID = iMarcaID;
                    }
                    if (oParte.MedidaID.Valor() != iUnidadDeMedidaID)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.UnidadDeMedida, oParte.MedidaID.ToString(), iUnidadDeMedidaID.ToString());
                        oParte.MedidaID = iUnidadDeMedidaID;
                    }
                    if (oParte.UnidadEmpaque != mUnidadDeEmpaque)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.UnidadDeEmpaque, oParte.UnidadEmpaque.ToString(), mUnidadDeEmpaque.ToString());
                        oParte.UnidadEmpaque = mUnidadDeEmpaque;
                    }
                    if (oParte.TiempoReposicion != mTiempoDeReposicion)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.TiempoDeReposicion, oParte.TiempoReposicion.ToString(), mTiempoDeReposicion.ToString());
                        oParte.TiempoReposicion = mTiempoDeReposicion;
                    }
                    if (oParte.AplicaComision.Valor() != bComision)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.AplicaComision, this.CadenaDeBool(oParte.AplicaComision), bComision.ACadena());
                        oParte.AplicaComision = bComision;
                    }
                    if (oParte.EsServicio.Valor() != bServicio)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.EsServicio, this.CadenaDeBool(oParte.EsServicio), bServicio.ACadena());
                        oParte.EsServicio = bServicio;
                    }
                    if (oParte.Etiqueta.Valor() != bEtiqueta)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Etiqueta, this.CadenaDeBool(oParte.Etiqueta), bEtiqueta.ACadena());
                        oParte.Etiqueta = bEtiqueta;
                    }
                    if (oParte.SoloUnaEtiqueta.Valor() != bSoloUna)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.SoloUnaEtiqueta, this.CadenaDeBool(oParte.SoloUnaEtiqueta), bSoloUna.ACadena());
                        oParte.SoloUnaEtiqueta = bSoloUna;
                    }
                    if (oParte.EsPar.Valor() != bEsPar)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.EsPar, this.CadenaDeBool(oParte.EsPar), bEsPar.ACadena());
                        oParte.EsPar = bEsPar;
                    }
                    if (Helper.ConvertirCadena(oParte.CodigoBarra) != sCodigoDeBarras)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.CodigoDeBarras, oParte.CodigoBarra, sCodigoDeBarras);
                        oParte.CodigoBarra = sCodigoDeBarras;
                    }
                    
                    /* oParte.NumeroParte = Helper.ConvertirCadena(Fila.Cells["NumeroDeParte"].Value);
                    oParte.NombreParte = Helper.ConvertirCadena(Fila.Cells["Descripcion"].Value);
                    oParte.ProveedorID = Helper.ConvertirEntero(Fila.Cells["ProveedorID"].Value);
                    oParte.LineaID = Helper.ConvertirEntero(Fila.Cells["LineaID"].Value);
                    oParte.MarcaParteID = Helper.ConvertirEntero(Fila.Cells["MarcaID"].Value);
                    oParte.MedidaID = Helper.ConvertirEntero(Fila.Cells["UnidadDeMedidaID"].Value);
                    oParte.AplicaComision = Helper.ConvertirBool(Fila.Cells["Comision"].Value);
                    oParte.EsServicio = Helper.ConvertirBool(Fila.Cells["Servicio"].Value);
                    oParte.Etiqueta = Helper.ConvertirBool(Fila.Cells["Etiqueta"].Value);
                    oParte.SoloUnaEtiqueta = Helper.ConvertirBool(Fila.Cells["SoloUna"].Value);
                    oParte.CodigoBarra = Helper.ConvertirCadena(Fila.Cells["CodigoDeBarra"].Value);
                    */

                    Guardar.Generico<Parte>(oParte);
                }
                // Se guardan los datos de precios
                if (bCambioPrecio)
                {
                    var oParte = General.GetEntity<PartePrecio>(q => q.ParteID == iParteID && q.Estatus);
                    decimal mCosto = Helper.ConvertirDecimal(Fila.Cells["Costo"].Value);
                    decimal mCostoConDescuento = Helper.ConvertirDecimal(Fila.Cells["CostoConDescuento"].Value);
                    decimal mPorUtil1 = Helper.ConvertirDecimal(Fila.Cells["Por1"].Value);
                    decimal mPorUtil2 = Helper.ConvertirDecimal(Fila.Cells["Por2"].Value);
                    decimal mPorUtil3 = Helper.ConvertirDecimal(Fila.Cells["Por3"].Value);
                    decimal mPorUtil4 = Helper.ConvertirDecimal(Fila.Cells["Por4"].Value);
                    decimal mPorUtil5 = Helper.ConvertirDecimal(Fila.Cells["Por5"].Value);
                    decimal mPrecio1 = Helper.ConvertirDecimal(Fila.Cells["Precio1"].Value);
                    decimal mPrecio2 = Helper.ConvertirDecimal(Fila.Cells["Precio2"].Value);
                    decimal mPrecio3 = Helper.ConvertirDecimal(Fila.Cells["Precio3"].Value);
                    decimal mPrecio4 = Helper.ConvertirDecimal(Fila.Cells["Precio4"].Value);
                    decimal mPrecio5 = Helper.ConvertirDecimal(Fila.Cells["Precio5"].Value);

                    // Para agregar registro de cambios
                    if (oParte.Costo.Valor() != mCosto)
                    {
                        // Para calcular la diferencia total en costo, por sucursal
                        decimal mVarCosto = (mCosto - oParte.Costo.Valor());
                        var oExistencias = General.GetListOf<ParteExistencia>(c => c.ParteID == iParteID && c.Estatus);
                        foreach (var oReg in oExistencias)
                        {
                            if (!oVarCostos.ContainsKey(oReg.SucursalID))
                                oVarCostos.Add(oReg.SucursalID, 0);
                            oVarCostos[oReg.SucursalID] += (mVarCosto * oReg.Existencia.Valor());
                        }
                        
                        //
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Costo, this.CadenaDeDecimal(oParte.Costo), mCosto.ToString());
                        oParte.Costo = mCosto;
                    }
                    if (oParte.CostoConDescuento.Valor() != mCostoConDescuento)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.CostoConDescuento
                            , this.CadenaDeDecimal(oParte.CostoConDescuento), mCostoConDescuento.ToString());
                        oParte.CostoConDescuento = mCostoConDescuento;
                    }
                    if (oParte.PorcentajeUtilidadUno.Valor() != mPorUtil1)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.PorUtil1, this.CadenaDeDecimal(oParte.PorcentajeUtilidadUno), mPorUtil1.ToString());
                        oParte.PorcentajeUtilidadUno = mPorUtil1;
                    }
                    if (oParte.PorcentajeUtilidadDos.Valor() != mPorUtil2)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.PorUtil2, this.CadenaDeDecimal(oParte.PorcentajeUtilidadDos), mPorUtil2.ToString());
                        oParte.PorcentajeUtilidadDos = mPorUtil2;
                    }
                    if (oParte.PorcentajeUtilidadTres.Valor() != mPorUtil3)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.PorUtil3, this.CadenaDeDecimal(oParte.PorcentajeUtilidadTres), mPorUtil3.ToString());
                        oParte.PorcentajeUtilidadTres = mPorUtil3;
                    }
                    if (oParte.PorcentajeUtilidadCuatro.Valor() != mPorUtil4)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.PorUtil4, this.CadenaDeDecimal(oParte.PorcentajeUtilidadCuatro), mPorUtil4.ToString());
                        oParte.PorcentajeUtilidadCuatro = mPorUtil4;
                    }
                    if (oParte.PorcentajeUtilidadCinco.Valor() != mPorUtil5)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.PorUtil5, this.CadenaDeDecimal(oParte.PorcentajeUtilidadCinco), mPorUtil5.ToString());
                        oParte.PorcentajeUtilidadCinco = mPorUtil5;
                    }
                    if (oParte.PrecioUno.Valor() != mPrecio1)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Precio1, this.CadenaDeDecimal(oParte.PrecioUno), mPrecio1.ToString());
                        oParte.PrecioUno = mPrecio1;
                    }
                    if (oParte.PrecioDos.Valor() != mPrecio2)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Precio2, this.CadenaDeDecimal(oParte.PrecioDos), mPrecio2.ToString());
                        oParte.PrecioDos = mPrecio2;
                    }
                    if (oParte.PrecioTres.Valor() != mPrecio3)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Precio3, this.CadenaDeDecimal(oParte.PrecioTres), mPrecio3.ToString());
                        oParte.PrecioTres = mPrecio3;
                    }
                    if (oParte.PrecioCuatro.Valor() != mPrecio4)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Precio4, this.CadenaDeDecimal(oParte.PrecioCuatro), mPrecio4.ToString());
                        oParte.PrecioCuatro = mPrecio4;
                    }
                    if (oParte.PrecioCinco.Valor() != mPrecio5)
                    {
                        this.AgregarCambio(oCambios, oParte.ParteID, Cat.PartesCambios.Precio5, this.CadenaDeDecimal(oParte.PrecioCinco), mPrecio5.ToString());
                        oParte.PrecioCinco = mPrecio5;
                    }

                    /* oParte.Costo = Helper.ConvertirDecimal(Fila.Cells["Costo"].Value);
                    oParte.PorcentajeUtilidadUno = Helper.ConvertirDecimal(Fila.Cells["Por1"].Value);
                    oParte.PrecioUno = Helper.ConvertirDecimal(Fila.Cells["Precio1"].Value);
                    oParte.PorcentajeUtilidadDos = Helper.ConvertirDecimal(Fila.Cells["Por2"].Value);
                    oParte.PrecioDos = Helper.ConvertirDecimal(Fila.Cells["Precio2"].Value);
                    oParte.PorcentajeUtilidadTres = Helper.ConvertirDecimal(Fila.Cells["Por3"].Value);
                    oParte.PrecioTres = Helper.ConvertirDecimal(Fila.Cells["Precio3"].Value);
                    oParte.PorcentajeUtilidadCuatro = Helper.ConvertirDecimal(Fila.Cells["Por4"].Value);
                    oParte.PrecioCuatro = Helper.ConvertirDecimal(Fila.Cells["Precio4"].Value);
                    oParte.PorcentajeUtilidadCinco = Helper.ConvertirDecimal(Fila.Cells["Por5"].Value);
                    oParte.PrecioCinco = Helper.ConvertirDecimal(Fila.Cells["Precio5"].Value);
                    */

                    Guardar.Generico<PartePrecio>(oParte);
                }

                // Se restaura la fila
                if (bCambioParte || bCambioPrecio)
                {
                    Fila.Cells["CambioParte"].Value = Cat.TiposDeAfectacion.SinCambios;
                    Fila.Cells["CambioPrecio"].Value = Cat.TiposDeAfectacion.SinCambios;
                    Fila.ErrorText = "";
                    // Fila.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            // Se generan los pólizas correspondientes por diferencias de costos
            string sObsPoliza = Helper.ConvertirCadena(this.dgvDatos.Columns["Costo"].Tag);
            foreach (var oDif in oVarCostos)
            {
                if (oDif.Value != 0)
                {
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, sObsPoliza, Cat.ContaCuentasAuxiliares.Inventario, Cat.ContaCuentasAuxiliares.CapitalFijo
                        , oDif.Value, "", null, null, oDif.Key);
                }
            }

            // Se guardan los cambios
            UtilLocal.MostrarNotificacion("Guardando registro de cambios...");
            Application.DoEvents();
            this.pgrGuardar.Inicializar(oCambios.Count, 1);
            DateTime dAhora = DateTime.Now;
            foreach (var oCambio in oCambios)
            {
                oCambio.Fecha = dAhora;
                oCambio.UsuarioID = iUsuarioID;
                Guardar.Generico<ParteCambio>(oCambio, false);
                this.pgrGuardar.EjecutarPaso(true);
            }
            
            ModelHelper.EndPersistentContext();

            // Se restaura el color de las celdas, si no hubo error
            foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
            {
                if (Fila.ErrorText == "")
                {
                    foreach (DataGridViewCell Celda in Fila.Cells)
                    {
                        if (Celda.HasStyle)
                            Celda.Style.ForeColor = Color.Black;
                    }
                }
            }

            this.IniciarTerminarProgreso(false);
            UtilLocal.MostrarNotificacion("Proceso completado " + (iErroresVal > 0 ? "con errores." : "correctamente."));
        }

        private void ImprimirEtiquetas(string sReporte)
        {
            // Se muestra progreso de avance
            this.IniciarTerminarProgreso(true);

            // Se inicia el proceso de impresión
            ModelHelper.StartPersistentContext();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                this.pgrGuardar.EjecutarPaso(true);
                if (!Helper.ConvertirBool(oFila.Cells["Procesar"].Value)) continue;

                int iParteID = Helper.ConvertirEntero(oFila.Cells["ParteID"].Value);
                var oParte = General.GetEntity<Parte>(c => c.ParteID == iParteID && c.Estatus);
                var oEtiqueta = new Etiquetas()
                {
                    ParteID = iParteID,
                    NumeroParte = oParte.NumeroParte,
                    NombreParte = oParte.NombreParte,
                    CodigoBarra = oParte.CodigoBarra
                };

                // Se manda imprimir el reporte
                var oRep = new FastReport.Report();
                oRep.Load(sReporte);
                oRep.RegisterData(new List<Etiquetas>() { oEtiqueta }, "etiquetas");
                oRep.Print();
            }
            ModelHelper.EndPersistentContext();

            this.IniciarTerminarProgreso(false);
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");
        }

        private void AgregarCambio(List<ParteCambio> oListaDeCambios, int iParteID, string sCampo, string sAntes, string sDespues)
        {
            oListaDeCambios.Add(new ParteCambio() { ParteID = iParteID, Campo = sCampo, Antes = sAntes, Despues = sDespues });
        }

        private string CadenaDeBool(bool? bValor)
        {
            return (bValor == null ? "" : bValor.Valor().ACadena());
        }

        private string CadenaDeDecimal(decimal? mValor)
        {
            return (mValor == null ? "" : mValor.ToString());
        }

        private void MarcarCambioCelda(DataGridViewCell oCelda)
        {
            string sColumna = oCelda.OwningColumn.Name;
            string sColMarca;
            if (sColumna == "CambioParte" || sColumna == "CambioPrecio" || sColumna == "Procesar")
                return;
            else if (sColumna == "Costo" || sColumna == "Por1" || sColumna == "Por2" || sColumna == "Por3" || sColumna == "Por" || sColumna == "Por5"
                || sColumna == "Precio1" || sColumna == "Precio2" || sColumna == "Precio3" || sColumna == "Precio4" || sColumna == "Precio5")
                sColMarca = "CambioPrecio";
            else
                sColMarca = "CambioParte";

            oCelda.OwningRow.Cells[sColMarca].Value = Cat.TiposDeAfectacion.Modificar;
            oCelda.Style.ForeColor = Color.Orange;

            // Se verifica si se cambió un precio, para colorear las celdas según aplique
            if (sColumna == "Precio1" || sColumna == "Precio2" || sColumna == "Precio3" || sColumna == "Precio4" || sColumna == "Precio5")
            {
                DataGridViewCell oCeldaMod = oCelda.OwningRow.Cells[oCelda.ColumnIndex + 1];
                decimal mDiferencia = (Helper.ConvertirDecimal(oCelda.Value) - Helper.ConvertirDecimal(oCeldaMod.Value));
                if (mDiferencia == 0)
                    oCeldaMod.Style.ForeColor = oCeldaMod.DataGridView.ForeColor;
                else if (mDiferencia > 0)
                    oCeldaMod.Style.ForeColor = Color.Green;
                else
                    oCeldaMod.Style.ForeColor = Color.Red;
            }
        }

        private void AplicarCambio(string sColumna, object oValor)
        {
            foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
            {
                if (!Helper.ConvertirBool(Fila.Cells["Procesar"].Value)) continue;
                Fila.Cells[sColumna].Value = oValor;
            }
        }

        private void AplicarCheck(string sColumna)
        {
            bool? bMarcado = null;
            foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
            {
                if (sColumna != "Procesar" && !Helper.ConvertirBool(Fila.Cells["Procesar"].Value)) continue;
                if (bMarcado == null)
                    bMarcado = !Helper.ConvertirBool(Fila.Cells[sColumna].Value);
                Fila.Cells[sColumna].Value = bMarcado;
            }
        }

        private void BuscarReemplazar(string sColumna)
        {
            // Se obtienen las cadenas de buscar y reemplazar
            string sBuscar = null, sReemplazar = null;
            var frmReemplazar = new BuscarReemplazar();
            if (frmReemplazar.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                sBuscar = frmReemplazar.Buscar.ToUpper();
                sReemplazar = frmReemplazar.Reemplazar.ToUpper();
            }
            frmReemplazar.Dispose();

            if (sBuscar == null || sReemplazar == null) return;

            // Se comienza a hacer el reemplaco masivo
            string sValor;
            foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
            {
                if (!Helper.ConvertirBool(Fila.Cells["Procesar"].Value)) continue;

                sValor = Helper.ConvertirCadena(Fila.Cells[sColumna].Value).ToUpper();
                if (sValor.Contains(sBuscar))
                    Fila.Cells[sColumna].Value = sValor.Replace(sBuscar, sReemplazar);
            }
        }

        private void AplicarCombo(string sColumna, string sCampoValor, string sCampoTexto, object oDatos)
        {
            var frmValor = new MensajeObtenerValor("Selecciona la opción deseada:", -1, MensajeObtenerValor.Tipo.Combo);
            frmValor.CargarCombo(sCampoValor, sCampoTexto, oDatos);
            if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                this.AplicarCambio(sColumna, frmValor.Valor);
            }
            frmValor.Dispose();
        }

        private void AplicarPrecio(string sColumna)
        {
            var frmCalculo = new MasterCostos();
            bool bCosto = (sColumna == "Costo");
            frmCalculo.MostrarActualizarPrecios = bCosto;
            frmCalculo.Text = (bCosto ? "Costo" : ("Precio " + sColumna.Derecha(1)));
            if (frmCalculo.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
                {
                    if (!Helper.ConvertirBool(Fila.Cells["Procesar"].Value)) continue;

                    // Se aplica el nuevo costo / precio, según aplique
                    if (frmCalculo.TipoDePrecio == 1)
                    {
                        Fila.Cells[sColumna].Value = frmCalculo.Importe;
                        if (bCosto)
                        {
                            Fila.Cells["CostoConDescuento"].Value = frmCalculo.Importe;
                            this.dgvDatos.Columns[sColumna].Tag = ("Importe fijo: " + frmCalculo.Importe.ToString(GlobalClass.FormatoMoneda));
                        }
                    }
                    else if (frmCalculo.TipoDePrecio == 2)
                    {
                        Fila.Cells[sColumna].Value = Math.Round(Helper.ConvertirDecimal(Fila.Cells[sColumna].Value) * (1 + (frmCalculo.Porcentaje / 100)), 2);
                        if (bCosto)
                        {
                            Fila.Cells["CostoConDescuento"].Value = 
                                Math.Round(Helper.ConvertirDecimal(Fila.Cells["CostoConDescuento"].Value) * (1 + (frmCalculo.Porcentaje / 100)), 2);
                            this.dgvDatos.Columns[sColumna].Tag = ("Incremento o descuento: " + frmCalculo.Porcentaje.ToString() + "%");
                        }
                    }
                    else
                    {
                        Fila.Cells[sColumna].Value = (Helper.ConvertirDecimal(Fila.Cells[sColumna].Value) + frmCalculo.Importe);
                        if (bCosto)
                        {
                            Fila.Cells["CostoConDescuento"].Value = (Helper.ConvertirDecimal(Fila.Cells["CostoConDescuento"].Value) + frmCalculo.Importe);
                            this.dgvDatos.Columns[sColumna].Tag = ("Incremento o descuento: " + frmCalculo.Importe.ToString(GlobalClass.FormatoMoneda));
                        }
                    }

                    // Si es un precio, se aplica el redondeo
                    if (!bCosto)
                        Fila.Cells[sColumna].Value = Helper.AplicarRedondeo(Helper.ConvertirDecimal(Fila.Cells[sColumna].Value));

                    // Se actualizan los precios, si aplica
                    if (frmCalculo.ActualizarPrecios)
                    {
                        decimal mCosto = Helper.ConvertirDecimal(Fila.Cells["Costo"].Value);
                        Fila.Cells["Precio1"].Value = Helper.AplicarRedondeo(mCosto * Helper.ConvertirDecimal(Fila.Cells["Por1"].Value));
                        Fila.Cells["Precio2"].Value = Helper.AplicarRedondeo(mCosto * Helper.ConvertirDecimal(Fila.Cells["Por2"].Value));
                        Fila.Cells["Precio3"].Value = Helper.AplicarRedondeo(mCosto * Helper.ConvertirDecimal(Fila.Cells["Por3"].Value));
                        Fila.Cells["Precio4"].Value = Helper.AplicarRedondeo(mCosto * Helper.ConvertirDecimal(Fila.Cells["Por4"].Value));
                        Fila.Cells["Precio5"].Value = Helper.AplicarRedondeo(mCosto * Helper.ConvertirDecimal(Fila.Cells["Por5"].Value));
                    }
                }
            }
            frmCalculo.Dispose();
        }

        private void AplicarPorcentajes()
        {
            var frmCalculo = new MasterCostos();
            frmCalculo.Text = "Porcentajes";
            frmCalculo.ModoPrecios = false;
            if (frmCalculo.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
                {
                    if (!Helper.ConvertirBool(Fila.Cells["Procesar"].Value)) continue;

                    // Se afectan los porcentajes
                    Fila.Cells["Por1"].Value = frmCalculo.Porcentajes[0];
                    Fila.Cells["Por2"].Value = frmCalculo.Porcentajes[1];
                    Fila.Cells["Por3"].Value = frmCalculo.Porcentajes[2];
                    Fila.Cells["Por4"].Value = frmCalculo.Porcentajes[3];
                    Fila.Cells["Por5"].Value = frmCalculo.Porcentajes[4];

                    // Se actualizan los precios, si aplica
                    if (frmCalculo.ActualizarPrecios)
                    {
                        decimal mCosto = Helper.ConvertirDecimal(Fila.Cells["Costo"].Value);
                        Fila.Cells["Precio1"].Value = Helper.AplicarRedondeo(mCosto * frmCalculo.Porcentajes[0]);
                        Fila.Cells["Precio2"].Value = Helper.AplicarRedondeo(mCosto * frmCalculo.Porcentajes[1]);
                        Fila.Cells["Precio3"].Value = Helper.AplicarRedondeo(mCosto * frmCalculo.Porcentajes[2]);
                        Fila.Cells["Precio4"].Value = Helper.AplicarRedondeo(mCosto * frmCalculo.Porcentajes[3]);
                        Fila.Cells["Precio5"].Value = Helper.AplicarRedondeo(mCosto * frmCalculo.Porcentajes[4]);
                    }
                }
            }
            frmCalculo.Dispose();
        }

        private void IniciarTerminarProgreso(bool bIniciar)
        {
            if (bIniciar)
            {
                this.dgvDatos.Height -= (this.pgrGuardar.Height + 6);
                Application.DoEvents();
                foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
                    oCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                this.pgrGuardar.Inicializar(this.dgvDatos.Rows.Count, 1);
            }
            else
            {
                this.dgvDatos.Height += (this.pgrGuardar.Height + 6);
                foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
                    oCol.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            this.btnMostrar.Enabled = !bIniciar;
            this.btnGuardar.Enabled = !bIniciar;
            this.pgrGuardar.Visible = bIniciar;
        }

        #endregion

                        
    }
}
