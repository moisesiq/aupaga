using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class PartesAbc : UserControl
    {
        // Para el Singleton
        private static PartesAbc _Instance;
        public static PartesAbc Instance
        {
            get
            {
                if (PartesAbc._Instance == null || PartesAbc._Instance.IsDisposed)
                    PartesAbc._Instance = new PartesAbc();
                return PartesAbc._Instance;
            }
        }
        //

        // Variables locales
        List<pauPartesAbc_Result> oPartesAbc;
        int IndiceGuardado = 0;
        bool PausaSolicitada = false;

        public PartesAbc()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void PartesAbc_Load(object sender, EventArgs e)
        {
            this.dgvDatos.Columns["NumeroDeParte"].ValueType = typeof(string);
            this.dgvDatos.Columns["Descripcion"].ValueType = typeof(string);
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.MostrarDatos();
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            this.ProcesarDatos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas guardar los ABCs mostrados? Toma en cuenta que este proceso puede durar varios minutos.")
                != DialogResult.Yes)
                return;

            this.GuardarDatos();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();
            string sArchivo = (Path.GetTempFileName() + ".csv");
            this.dgvDatos.ExportarACsv(sArchivo, true);
            System.Diagnostics.Process.Start("excel", sArchivo);
            Cargando.Cerrar();
        }

        private void btnPausar_Click(object sender, EventArgs e)
        {
            if (Util.Logico(this.Tag))
            {
                this.PausaSolicitada = false;
                this.Tag = false;
                this.btnPausar.Text = "||";
                this.GuardarDatos();
            }
            else
            {
                this.PausaSolicitada = true;
                this.Tag = true;
                this.btnPausar.Text = ">";
            }
        }

        #endregion

        #region [ Métodos ]

        private void MostrarDatos()
        {
            Cargando.Mostrar();

            DateTime dHasta = DateTime.Now.Date.DiaPrimero().AddDays(-1);
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", dHasta.AddYears(-1).AddDays(1));
            oParams.Add("Hasta", dHasta);
            this.oPartesAbc = Datos.ExecuteProcedure<pauPartesAbc_Result>("pauPartesAbc", oParams);

            // Se llena el grid
            this.dgvDatos.Rows.Clear();
            int iFila;
            foreach (var oParte in this.oPartesAbc)
            {
                iFila = this.dgvDatos.Rows.Add(oParte.ParteID, oParte.NumeroDeParte, oParte.Descripcion
                    , oParte.UtilidadUniP1, oParte.Cantidad, oParte.Utilidad
                    , oParte.AbcDeVentas, oParte.AbcDeUtilidad, oParte.AbcDeNegocio, oParte.AbcDeProveedor, oParte.AbcDeLinea);
                this.dgvDatos.Rows[iFila].Tag = oParte;
            }

            Cargando.Cerrar();
        }

        private void ProcesarDatos()
        {
            if (this.dgvDatos.Rows.Count <= 0) return;
            Cargando.Mostrar();
            
            // Se obtienen los rangos
            var oRangos = Datos.GetListOf<ParteAbcRango>();

            // Se obtiene el listado de partes, para sacar los porcentajes de utilidad
            var oPorUtilAcum = new Dictionary<int, decimal>();
            var oPartesUtil = this.oPartesAbc.OrderByDescending(q => q.Utilidad);
            decimal mTotalUtil = oPartesUtil.Sum(q => q.Utilidad);
            decimal mPorUtilAcum = 0;
            foreach (var oParteU in oPartesUtil)
            {
                mPorUtilAcum += ((oParteU.Utilidad / mTotalUtil) * 100);
                oPorUtilAcum.Add(oParteU.ParteID, mPorUtilAcum);
            }
            // Se obtiene el listado de utilidades por proveedor
            var oPorUtilAcumProv = new Dictionary<int, decimal>();
            var oProvUtil = this.oPartesAbc.GroupBy(g => g.ProveedorID).Select(o => new { ProveedorID = o.Key, Utilidad = o.Sum(q => q.Utilidad) })
                .OrderByDescending(q => q.Utilidad);
            mPorUtilAcum = 0;
            foreach (var oProv in oProvUtil)
            {
                mPorUtilAcum += ((oProv.Utilidad / mTotalUtil) * 100);
                oPorUtilAcumProv.Add(oProv.ProveedorID, mPorUtilAcum);
            }
            // Se obtiene el listado de utilidades por línea
            var oPorUtilAcumLinea = new Dictionary<int, decimal>();
            var oLineaUtil = this.oPartesAbc.GroupBy(g => g.LineaID).Select(o => new { LineaID = o.Key, Utilidad = o.Sum(q => q.Utilidad) })
                .OrderByDescending(q => q.Utilidad);
            mPorUtilAcum = 0;
            foreach (var oLinea in oLineaUtil)
            {
                mPorUtilAcum += ((oLinea.Utilidad / mTotalUtil) * 100);
                oPorUtilAcumLinea.Add(oLinea.LineaID, mPorUtilAcum);
            }

            // Se comienzan a procesar las partes
            pauPartesAbc_Result oParte;
            bool bYaVentas, bYaUtilidad, bYaNegocio, bYaProveedor, bYaLinea;
            decimal mCantidad, mUtilidadUni;
            mPorUtilAcum = 0;
            decimal mPorUtilAcumProv = 0, mPorUtilAcumLinea = 0;
            foreach (DataGridViewRow Fila in this.dgvDatos.Rows)
            {
                // Se limpian los Abc actuales
                Fila.Cells["AbcDeVentas"].Value = "";
                Fila.Cells["AbcDeUtilidad"].Value = "";
                Fila.Cells["AbcDeNegocio"].Value = "";
                Fila.Cells["AbcDeProveedor"].Value = "";
                Fila.Cells["AbcDeLinea"].Value = "";
                
                //
                oParte = (Fila.Tag as pauPartesAbc_Result);
                bYaVentas = bYaUtilidad = bYaNegocio = bYaProveedor = bYaLinea = false;
                mCantidad = Math.Round(oParte.Cantidad, 0);
                mUtilidadUni = Math.Round(oParte.UtilidadUniP1, 2);
                mPorUtilAcum = Math.Round(oPorUtilAcum[oParte.ParteID], 0);
                mPorUtilAcumProv = Math.Round(oPorUtilAcumProv[oParte.ProveedorID], 1);
                mPorUtilAcumLinea = Math.Round(oPorUtilAcumLinea[oParte.LineaID], 0);

                // Se recorren los rangos en busca del que coincida con el valor de la parte
                foreach (var oRango in oRangos)
                {
                    switch (oRango.TipoDeRango)
                    {
                        // Se calcula el Abc de Ventas
                        case Cat.TiposDeAbc.Ventas:
                            if (bYaVentas) break;
                            if ((oRango.Inicial == null || mCantidad >= oRango.Inicial) && (oRango.Final == null || mCantidad <= oRango.Final))
                            {
                                oParte.AbcDeVentas = oRango.Abc;
                                bYaVentas = true;
                            }
                            break;
                        // Se calcula el Abc de Utilidad
                        case Cat.TiposDeAbc.Utilidad:
                            if (bYaUtilidad) break;
                            if ((oRango.Inicial == null || mUtilidadUni >= oRango.Inicial) && (oRango.Final == null || mUtilidadUni <= oRango.Final))
                            {
                                oParte.AbcDeUtilidad = oRango.Abc;
                                bYaUtilidad = true;
                            }
                            break;
                        // Se calcula el Abc de Negocio
                        case Cat.TiposDeAbc.Negocio:
                            if (bYaNegocio) break;
                            if ((oRango.Inicial == null || mPorUtilAcum >= oRango.Inicial) && (oRango.Final == null || mPorUtilAcum <= oRango.Final))
                            {
                                oParte.AbcDeNegocio = oRango.Abc;
                                bYaNegocio = true;
                            }
                            break;
                        // Se calcula el Abc de Proveedor
                        case Cat.TiposDeAbc.Proveedor:
                            if (bYaProveedor) break;
                            if ((oRango.Inicial == null || mPorUtilAcumProv >= oRango.Inicial) && (oRango.Final == null || mPorUtilAcumProv <= oRango.Final))
                            {
                                oParte.AbcDeProveedor = oRango.Abc;
                                bYaProveedor = true;
                            }
                            break;
                        // Se calcula el Abc de Línea
                        case Cat.TiposDeAbc.Linea:
                            if (bYaLinea) break;
                            if ((oRango.Inicial == null || mPorUtilAcumLinea >= oRango.Inicial) && (oRango.Final == null || mPorUtilAcumLinea <= oRango.Final))
                            {
                                oParte.AbcDeLinea = oRango.Abc;
                                bYaLinea = true;
                            }
                            break;
                    }
                }

                // Se registran los Abc
                Fila.Cells["AbcDeVentas"].Value = oParte.AbcDeVentas;
                Fila.Cells["AbcDeUtilidad"].Value = oParte.AbcDeUtilidad;
                Fila.Cells["AbcDeNegocio"].Value = oParte.AbcDeNegocio;
                Fila.Cells["AbcDeProveedor"].Value = oParte.AbcDeProveedor;
                Fila.Cells["AbcDeLinea"].Value = oParte.AbcDeLinea;
            }

            Cargando.Cerrar();
        }

        private void GuardarDatos()
        {
            // Se inicializa el proceso de guardado, si aplica
            if (this.IndiceGuardado == 0)
            {
                if (this.dgvDatos.Rows.Count <= 0) return;

                this.btnMostrar.Enabled = false;
                this.btnProcesar.Enabled = false;
                this.btnGuardar.Enabled = false;
                // 
                this.pgbGuardado.Maximum = this.dgvDatos.Rows.Count ;
                this.pgbGuardado.Value = 0;
                this.lblGuardadas.Text = "0";
                this.lblTotalPartes.Text = this.dgvDatos.Rows.Count.ToString(GlobalClass.FormatoEntero);
                this.btnPausar.Enabled = true;
                this.btnPausar.Tag = false;
                this.pnlGuardado.Visible = true;
                // Se deshabilita el "Sort" de las columnas
                foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
                    oCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Se inicia el proceso
            Datos.StartPersistentContext();
            DataGridViewRow Fila;
            int iParteID;
            ParteAbc oParte;
            int iFilas = this.dgvDatos.Rows.Count;
            for (int iFila = this.IndiceGuardado; iFila < iFilas; iFila++)
            {
                Fila = this.dgvDatos.Rows[iFila];
                iParteID = Util.Entero(Fila.Cells["ParteID"].Value);
                oParte = Datos.GetEntity<ParteAbc>(q => q.ParteID == iParteID);
                // Si no existe, se crea
                if (oParte == null)
                    oParte = new ParteAbc() { ParteID = iParteID };
                //
                oParte.AbcDeVentas = (string)Fila.Cells["AbcDeVentas"].Value;
                oParte.AbcDeUtilidad = (string)Fila.Cells["AbcDeUtilidad"].Value;
                oParte.AbcDeNegocio = (string)Fila.Cells["AbcDeNegocio"].Value;
                oParte.AbcDeProveedor = (string)Fila.Cells["AbcDeProveedor"].Value;
                oParte.AbcDeLinea = (string)Fila.Cells["AbcDeLinea"].Value;
                Datos.Guardar<ParteAbc>(oParte);

                // Se reporta el progreso y se ejecutan los eventos (para que la aplicación no se trabe)
                this.IndiceGuardado = iFila;
                this.pgbGuardado.Value = (iFila + 1);
                this.lblGuardadas.Text = (iFila + 1).ToString(GlobalClass.FormatoEntero);
                Application.DoEvents();
                if (this.PausaSolicitada)
                    return;
            }
            Datos.EndPersistentContext();

            // Se finaliza
            this.IndiceGuardado = 0;
            this.btnMostrar.Enabled = true;
            this.btnProcesar.Enabled = true;
            this.btnGuardar.Enabled = true;
            this.btnPausar.Enabled = false;
            //this.pnlGuardado.Visible = false;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
                oCol.SortMode = DataGridViewColumnSortMode.Automatic;
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");
        }

        #endregion
                                
    }
}
