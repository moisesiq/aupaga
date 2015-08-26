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

namespace Refaccionaria.App
{
    public partial class criteriosAbc : UserControl
    {
        BindingSource fuenteDatos;
        ControlError cntError = new ControlError();
        BackgroundWorker bgworker;
        public delegate object MyDelegate();
        
        public static criteriosAbc Instance
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

            internal static readonly criteriosAbc instance = new criteriosAbc();
        }

        public criteriosAbc()
        {
            InitializeComponent();
        }

        #region [ Eventos ] 
                
        private void criteriosAbc_Load(object sender, EventArgs e)
        {
            try
            {
                this.dgvDatos.DataSource = null;
                if (this.dgvDatos.Rows.Count > 0)
                    this.dgvDatos.Rows.Clear();
                if (this.dgvDatos.Columns.Count > 0)
                    this.dgvDatos.Columns.Clear();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvDatos.DataSource == null)
                    return;
                                
                BindingSource bs = (BindingSource)this.dgvDatos.DataSource;                
                DataTable dt = (DataTable)bs.DataSource;

                using (FastReport.Report report = new FastReport.Report())
                {
                    report.Load(string.Format("{0}{1}", GlobalClass.ConfiguracionGlobal.pathReportes, "ReportePartesClasificacionAbc.frx"));                                        
                    report.RegisterData(dt, "PartesClasificacionAbc");
                    report.GetDataSource("PartesClasificacionAbc").Enabled = true;
                    report.Show(true);                    
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
                if (this.dgvDatos.DataSource == null)
                    return;
                             
                var res = Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                this.Cursor = Cursors.WaitCursor;

                General.ExecuteProcedure<List<int>>("pauParteActualizaCriterioAbc", new Dictionary<string, object>());

                this.Cursor = Cursors.Default;
                new Notificacion("Información almacenada correctamente.", 2 * 1000).Mostrar(Principal.Instance);
                this.criteriosAbc_Load(sender, null);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Metodos ]

        public void IniciarActualizarListado()
        {
            this.dgvDatos.DataSource = null;
            if (this.dgvDatos.Rows.Count > 0)
                this.dgvDatos.Rows.Clear();
            if (this.dgvDatos.Columns.Count > 0)
                this.dgvDatos.Columns.Clear();

            //this.lblEncontrados.Text = "Encontrados:";
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
            // Your background task goes here
            for (int i = 11; i <= 100; i++)
            {
                // Report progress to 'UI' thread
                bgworker.ReportProgress(i);
            }
        }

        void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // The progress percentage is a property of e
            progreso.Value = e.ProgressPercentage;
        }

        public void ActualizarListado(object o, DoWorkEventArgs e)
        {
            try
            {          
                this.fuenteDatos = new BindingSource();
                this.fuenteDatos.DataSource = Helper.newTable<PartesClasificacionAbcView>("Abc", General.GetListOf<PartesClasificacionAbcView>().ToList());             
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
                //this.lblEncontrados.Text = string.Format("Encontrados: {0}", fuenteDatos.Count);

                Helper.ColumnasToHeaderText(this.dgvDatos);
                Helper.OcultarColumnas(this.dgvDatos, new string[] { "ParteID", "EntityKey", "EntityState" });
                Helper.FormatoDecimalColumnas(this.dgvDatos, new string[] { "Cantidad" });
                this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;                
                progreso.Value = 0;
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                this.Cursor = Cursors.Default;
            }
        }

        #endregion
                
    }
}
