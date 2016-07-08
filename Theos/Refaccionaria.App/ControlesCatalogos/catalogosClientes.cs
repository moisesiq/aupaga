using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class catalogosClientes : UserControl
    {
        BindingSource datosClientes;
        ControlError cntError = new ControlError();
        BackgroundWorker bgworker;

        public static catalogosClientes Instance
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

            internal static readonly catalogosClientes instance = new catalogosClientes();
        }

        public catalogosClientes()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void catalogosClientes_Load(object sender, EventArgs e)
        {
            this.CargaInicial();
            this.LimpiarFormulario();
            var cliente = clientes.Instance;
            cliente.ClienteID = 0;
            cliente.ModificaCredito = UtilLocal.ValidarPermiso("Administracion.Clientes.Credito.Modificar");
            this.addControlInPanel(cliente, panelClientes);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            this.LimpiarFormulario();
            clientes.Instance.CustomInvoke<clientes>(m => m.CargarCliente(0));
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var bus = DetalleBusquedaDeClientes.Instance;
            bus.ShowDialog();

            if (bus.Seleccionado)
            {
                var clienteId = bus.Sel;
                if (clienteId > 0)
                {
                    this.cboCliente.SelectedValue = clienteId;
                    this.cboCliente_SelectionChangeCommitted(sender, null);
                }
            }
        }

        public void seleccionarCliente(int clienteId)
        {
            try
            {                
                this.datosClientes = new BindingSource();
                this.datosClientes.DataSource = Datos.GetListOf<ClientesDatosView>(c => c.Nombre.Length > 2).ToList();                
                var listaClientes = (List<ClientesDatosView>)datosClientes.DataSource;
                listaClientes.Sort((x, y) => x.Nombre.CompareTo(y.Nombre));
                this.cboCliente.DisplayMember = "Nombre";
                this.cboCliente.DataSource = listaClientes;
                this.cboCliente.ValueMember = "ClienteID";
                AutoCompleteStringCollection autCliente = new AutoCompleteStringCollection();
                foreach (var cliente in listaClientes) autCliente.Add(cliente.Nombre);
                this.cboCliente.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboCliente.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboCliente.AutoCompleteCustomSource = autCliente;
                this.cboCliente.TextUpdate += new EventHandler(UtilLocal.cboCharacterCasingUpper);
                this.cboCliente.SelectedValue = clienteId;
                this.cboCliente_SelectionChangeCommitted(null, null);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void cboCliente_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCliente.SelectedValue == null)
                    return;
                int id;
                if (int.TryParse(this.cboCliente.SelectedValue.ToString(), out id))
                {
                    clientes.Instance.CustomInvoke<clientes>(m => m.CargarCliente(id));
                }
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void cboCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.cboCliente.SelectedValue == null) return;

            if (e.KeyCode == Keys.Enter && this.cboCliente.Text != "")
            {
                int id;
                if (int.TryParse(this.cboCliente.SelectedValue.ToString(), out id))
                {
                    clientes.Instance.CustomInvoke<clientes>(m => m.CargarCliente(id));
                }
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
                bgworker = new BackgroundWorker();
                bgworker.DoWork += ActualizarListados;
                bgworker.RunWorkerCompleted += Terminado;
                bgworker.RunWorkerAsync();
                bgworker.WorkerReportsProgress = true;
                bgworker.DoWork += new DoWorkEventHandler(bgworker_DoWork);
                bgworker.ProgressChanged += new ProgressChangedEventHandler(bgworker_ProgressChanged);
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void LimpiarFormulario()
        {
            this.cboCliente.Text = "";
        }

        public void ActualizarListados(object o, DoWorkEventArgs e)
        {
            try
            {
                //SplashScreen.Show(new Splash());
                this.datosClientes = new BindingSource();
                this.datosClientes.DataSource = Datos.GetListOf<ClientesDatosView>(c => c.Nombre.Length > 2).OrderBy(c => c.Nombre).ToList();
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
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
            progreso.Value = e.ProgressPercentage;
        }

        public void Terminado(object o, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.cboCliente.DataSource = null;
                // var listaClientes = (List<ClientesDatosView>)datosClientes.DataSource;
                // listaClientes.Sort((x, y) => x.Nombre.CompareTo(y.Nombre));
                this.cboCliente.DisplayMember = "Nombre";
                this.cboCliente.DataSource = this.datosClientes.DataSource;
                this.cboCliente.ValueMember = "ClienteID";
                // this.cboCliente.TextUpdate += new EventHandler(Util.cboCharacterCasingUpper);
                this.LimpiarFormulario();
                //SplashScreen.Close();
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void addControlInPanel(object controlHijo, Panel panel)
        {
            panel.Controls.Clear();
            if (panel.Controls.Count > 0)
                panel.Controls.RemoveAt(0);
            UserControl usc = controlHijo as UserControl;
            usc.Dock = DockStyle.Fill;
            panel.Controls.Add(usc);
            panel.Tag = usc;
            usc.Show();
        }

        #endregion

    }
}
