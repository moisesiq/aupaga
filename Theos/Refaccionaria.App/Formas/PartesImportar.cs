using System;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Drawing;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class PartesImportar : Form
    {
        const string CadenaVerdadero = "verdadero";

        public PartesImportar()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;
        }

        #region [ Eventos ]

        private void PartesImportar_Load(object sender, EventArgs e)
        {
            this.ActiveControl = this.btnBuscarCsv;
        }

        private void btnBuscarCsv_Click(object sender, EventArgs e)
        {
            var frmCsv = new OpenFileDialog();
            frmCsv.Filter = "(*.csv)|*.csv";
            frmCsv.Title = "Ruta del archivo Csv a importar";
            if (frmCsv.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.txtRutaCsv.Text = frmCsv.FileName;
            frmCsv.Dispose();
        }

        private void btnCargarCsv_Click(object sender, EventArgs e)
        {
            this.CargarCsv(this.txtRutaCsv.Text);
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            this.Importar();
            // this.DialogResult = DialogResult.OK;
            // this.Close();
        }
                
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void CargarCsv(string sRutaCsv)
        {
            // Verifica que el archivo exista
            if (!File.Exists(sRutaCsv)) {
                UtilLocal.MensajeAdvertencia("El archivo especificado no existe.");
                return;
            }

            // Se lee el archivo csv
            var oDatos = Util.LeerCsv(sRutaCsv);

            // Se generan las columnas
            this.dgvProceso.Columns.Clear();
            foreach (DataColumn oCol in oDatos.Columns)
            {
                int iCol = this.dgvProceso.Columns.Add(oCol.ColumnName, oCol.ColumnName);
                // this.dgvProceso.Columns[iCol].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // Se insertan los registros
            this.dgvProceso.Rows.Clear();
            foreach (DataRow oFila in oDatos.Rows)
            {
                this.dgvProceso.Rows.Add(oFila.ItemArray);
                // int iFila = this.dgvProceso.Rows.Add();
                // for (int iCol = 0; iCol < oFila.ItemArray.Length; iCol++)
                //     this.dgvProceso[iCol, iFila].Value = oFila[iCol];
            }

            this.prgProgreso.Inicializar(this.dgvProceso.Rows.Count, 1);
        }

        private void Importar()
        {
            string sColumnasFalt = this.ValidarColumnas();
            if (sColumnasFalt != "")
            {
                UtilLocal.MensajeAdvertencia("El archivo especificado no tiene el formato correcto. Falta especificar las siguientes columnas:\n\n" + sColumnasFalt);
                return;
            }

            this.HabilitarControlesProceso(false);
            this.prgProgreso.Inicializar(this.dgvProceso.Rows.Count, 1);

            foreach (DataGridViewRow oFila in this.dgvProceso.Rows)
            {
                this.prgProgreso.EjecutarPaso(true);

                if (oFila.DefaultCellStyle.ForeColor == Color.Green)
                    continue;
                
                // Se valida que no se repita el número de parte
                string sNumeroDeParte = Util.Cadena(oFila.Cells["NumeroDeParte"].Value);
                if (Datos.Exists<Parte>(c => c.NumeroParte == sNumeroDeParte && c.Estatus))
                {
                    if (UtilLocal.MensajePregunta(string.Format("El Número de Parte especificado ya existe. Aún así deseas importar el registro: {0} - {1}"
                        , oFila.Cells["NumeroDeParte"].Value, oFila.Cells["Nombre"].Value)) != DialogResult.Yes)
                    {
                        this.PonerErrorFila(oFila, "El número de parte especificado ya existe en la base de datos. Revisar.");
                        continue;
                    }
                }
                // Se obtiene los Ids (Proveedor, Marca, ..)
                string sProveedor = Util.Cadena(oFila.Cells["Proveedor"].Value);
                var oProveedor = Datos.GetEntity<Proveedor>(c => c.NombreProveedor == sProveedor && c.Estatus);
                if (oProveedor == null)
                {
                    this.PonerErrorFila(oFila, "El Proveedor especificado es inválido.");
                    continue;   
                }
                string sMarca = Util.Cadena(oFila.Cells["Marca"].Value);
                var oMarca = Datos.GetEntity<MarcaParte>(c => c.NombreMarcaParte == sMarca && c.Estatus);
                if (oMarca == null)
                {
                    this.PonerErrorFila(oFila, "La Marca especificada es inválida.");
                    continue;
                }
                string sLinea = Util.Cadena(oFila.Cells["Linea"].Value);
                var oLinea = Datos.GetEntity<Linea>(c => c.NombreLinea == sLinea && c.Estatus);
                if (oLinea == null)
                {
                    this.PonerErrorFila(oFila, "La Línea especificada es inválida.");
                    continue;
                }
                string sSubsistema = Util.Cadena(oFila.Cells["Subsistema"].Value);
                var oSubsistema = Datos.GetEntity<Subsistema>(c => c.NombreSubsistema == sSubsistema && c.Estatus);
                if (oSubsistema == null)
                {
                    this.PonerErrorFila(oFila, "El Subsistema especificado es inválido.");
                    continue;
                }
                string sUnidadDeMedida = Util.Cadena(oFila.Cells["UnidadDeMedida"].Value);
                var oUnidadDeMedida = Datos.GetEntity<Medida>(c => c.NombreMedida == sUnidadDeMedida && c.Estatus);
                if (oUnidadDeMedida == null)
                {
                    this.PonerErrorFila(oFila, "La Unidad de Medida es inválida.");
                    continue;
                }

                // Se genera el registro de la parte
                var oParte = new Parte()
                {
                    NumeroParte = Util.Cadena(oFila.Cells["NumeroDeParte"].Value),
                    NombreParte = Util.Cadena(oFila.Cells["Nombre"].Value),
                    ProveedorID = oProveedor.ProveedorID,
                    MarcaParteID = oMarca.MarcaParteID,
                    LineaID = oLinea.LineaID,
                    SubsistemaID = oSubsistema.SubsistemaID,
                    MedidaID = oUnidadDeMedida.MedidaID,
                    UnidadEmpaque = Util.Entero(oFila.Cells["UnidadDeEmpaque"].Value),
                    AplicaComision = (Util.Cadena(oFila.Cells["AplicaComision"].Value).ToLower() == PartesImportar.CadenaVerdadero),
                    Etiqueta = (Util.Cadena(oFila.Cells["Etiqueta"].Value).ToLower() == PartesImportar.CadenaVerdadero),
                    Es9500 = true
                };
                // Se genera el registro del precio
                var oPartePrecio = new PartePrecio()
                {
                    Costo = Util.Decimal(oFila.Cells["Costo"].Value),
                    PorcentajeUtilidadUno = Util.Decimal(oFila.Cells["PorUtil1"].Value),
                    PorcentajeUtilidadDos = Util.Decimal(oFila.Cells["PorUtil2"].Value),
                    PorcentajeUtilidadTres = Util.Decimal(oFila.Cells["PorUtil3"].Value),
                    PorcentajeUtilidadCuatro = Util.Decimal(oFila.Cells["PorUtil4"].Value),
                    PorcentajeUtilidadCinco = Util.Decimal(oFila.Cells["PorUtil5"].Value),
                    PrecioUno = Util.Decimal(oFila.Cells["Precio1"].Value),
                    PrecioDos = Util.Decimal(oFila.Cells["Precio2"].Value),
                    PrecioTres = Util.Decimal(oFila.Cells["Precio3"].Value),
                    PrecioCuatro = Util.Decimal(oFila.Cells["Precio4"].Value),
                    PrecioCinco = Util.Decimal(oFila.Cells["Precio5"].Value),
                };
                // Se guardan los datos
                Guardar.Parte(oParte, oPartePrecio);
                // Se colorea la fila
                oFila.ErrorText = "";
                oFila.DefaultCellStyle.ForeColor = Color.Green;
            }

            this.prgProgreso.EjecutarPaso(false);
            this.HabilitarControlesProceso(true);
        }

        private string ValidarColumnas()
        {
            var oRequeridos = new List<string>() {
                "NumeroDeParte"
                , "Nombre"
                , "Proveedor"
                , "Marca"
                , "Linea"
                , "Subsistema"
                , "UnidadDeMedida"
                , "UnidadDeEmpaque"
                , "AplicaComision"
                , "Etiqueta"
                , "Costo"
                , "PorUtil1"
                , "PorUtil2"
                , "PorUtil3"
                , "PorUtil4"
                , "PorUtil5"
                , "Precio1"
                , "Precio2"
                , "Precio3"
                , "Precio4"
                , "Precio5"
            };
            string sFaltantes = "";
            foreach (string sCampo in oRequeridos)
            {
                if (!this.dgvProceso.Columns.Contains(sCampo))
                    sFaltantes += ("\n" + sCampo);
            }
            sFaltantes = (sFaltantes == "" ? "" : sFaltantes.Substring(1));

            return sFaltantes;
        }

        private void HabilitarControlesProceso(bool bHabilitar)
        {
            this.btnCargarCsv.Enabled = bHabilitar;
            this.btnImportar.Enabled = bHabilitar;
        }

        private void PonerErrorFila(DataGridViewRow oFila, string sError)
        {
            oFila.ErrorText = sError;
            oFila.DefaultCellStyle.ForeColor = Color.Red;
        }

        #endregion
                
    }
}
