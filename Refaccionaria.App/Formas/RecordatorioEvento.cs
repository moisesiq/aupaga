using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class RecordatorioEvento : Form
    {
        public RecordatorioEvento(ProveedorEventoCalendario oEvento)
        {
            InitializeComponent();

            this.oEvento = oEvento;
            var oProveedor = General.GetEntity<Proveedor>(c => c.ProveedorID == oEvento.ProveedorID && c.Estatus);

            this.Entidad = oProveedor.NombreProveedor;
            this.Fecha = oEvento.Fecha;
            this.Evento = oEvento.Evento;
        }

        #region [ Propiedades ]

        public ProveedorEventoCalendario oEvento { get; set; }

        public string Entidad
        {
            get { return this.lblProveedor.Text; }
            set { this.lblProveedor.Text = value; }
        }

        public DateTime Fecha
        {
            get { return this.dtpFechaHora.Value; }
            set { this.dtpFechaHora.Value = value; }
        }

        public string Evento
        {
            get { return this.txtEvento.Text; }
            set { this.txtEvento.Text = value; }
        }

        #endregion

        #region [ Eventos ]

        private void RecordatorioEvento_Load(object sender, EventArgs e)
        {

        }

        private void btnReagendar_Click(object sender, EventArgs e)
        {
            if (this.dtpFechaHora.Value <= DateTime.Now)
            {
                UtilLocal.MensajeAdvertencia("No puedes agendar un evento para el pasado.");
                return;
            }

            this.oEvento.Fecha = this.dtpFechaHora.Value;
            this.oEvento.Evento = this.txtEvento.Text;
            Guardar.Generico<ProveedorEventoCalendario>(this.oEvento);
            this.Close();
        }

        private void btnDescartar_Click(object sender, EventArgs e)
        {
            this.oEvento.Revisado = true;
            Guardar.Generico<ProveedorEventoCalendario>(this.oEvento);
            this.Close();
        }

        #endregion
                
    }
}
