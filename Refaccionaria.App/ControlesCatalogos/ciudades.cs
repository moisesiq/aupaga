using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class ciudades : ListadoBase
    {
        BindingSource fuenteDatos;
        BackgroundWorker bgworker;
        public delegate object MyDelegate();

        // Para el Singleton
        private static ciudades _Instance;
        public static ciudades Instance
        {
            get
            {
                if (ciudades._Instance == null || ciudades._Instance.IsDisposed)
                    ciudades._Instance = new ciudades();
                return ciudades._Instance;
            }
        }
        //

        public ciudades()
        {
            InitializeComponent();
            this.Load += new EventHandler(HandleFormLoad);
        }

        #region [ Metodos ]

        private void HandleFormLoad(object sender, EventArgs e)
        {
            this.CargaInicial();            
            this.IniciarActualizarListado();            
        }

        public void CargaInicial()
        {
            try
            {
                if (this.cboEstado.DataSource == null)
                {
                    this.cboEstado.DataSource = General.GetListOf<Estado>(s => s.Estatus).ToList();
                    this.cboEstado.ValueMember = "EstadoID";
                    this.cboEstado.DisplayMember = "NombreEstado";
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void IniciarActualizarListado()
        {
            SplashScreen.Show(new Splash());
            bgworker = new BackgroundWorker();
            bgworker.DoWork += ActualizarListado;
            bgworker.RunWorkerCompleted += Terminado;
            bgworker.RunWorkerAsync();

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
            //SplashScreen.UpdateProgress(e.ProgressPercentage);            
        }

        public void ActualizarListado(object o, DoWorkEventArgs e)
        {
            try
            {
                var estadoId = Helper.ConvertirEntero(GetSelectedValue(this.cboEstado));
                this.fuenteDatos = new BindingSource();
                this.fuenteDatos.DataSource = Helper.newTable<CiudadesView>("Ciudades", General.GetListOf<CiudadesView>(c => c.EstadoID == estadoId).ToList());
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void Terminado(object o, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.IniciarCarga((DataTable)this.fuenteDatos.DataSource, "Ciudades");
                Helper.OcultarColumnas(base.dgvDatos, new string[] { "EstadoID", "MunicipioID" });                
                SplashScreen.Close();
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
        
        #endregion

        #region [ Eventos ]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetalleCiudad c = new DetalleCiudad();
            c.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleCiudad p = new DetalleCiudad(Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["CiudadID"].Value));
            p.ShowDialog();
        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            btnModificar_Click(sender, null);
        }

        private void cboEstado_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.IniciarActualizarListado();  
        }

        #endregion

    }
}
