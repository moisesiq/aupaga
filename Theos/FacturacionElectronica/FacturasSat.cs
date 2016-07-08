using System;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Xml;

using LibUtil;

namespace FacturacionElectronica
{
    public partial class FacturasSat : Form
    {
        ConSat oSat;

        public FacturasSat()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public string Rfc { get; set; }
        public string ClaveCiec { get; set; }

        /// <summary>
        /// {_RfcEmisor}
        /// {_RfcReceptor}
        /// {_Emisor}
        /// {_Receptor}
        /// {_Anio}
        /// {_Mes}
        /// {_Dia}
        /// {_Serie}
        /// {_Folio}
        /// {_Uuid}
        /// </summary>
        public string RutaGuardar { get; set; }

        #endregion

        #region [ Eventos ]

        private void FacturasSat_Load(object sender, EventArgs e)
        {
            // Se llenan los controles
            int iAnio = 2011;
            DateTime dAhora = DateTime.Now;
            while (iAnio <= dAhora.Year)
                this.cmbAnio.Items.Add(iAnio++);
            this.cmbAnio.Text = dAhora.Year.ToString();
            for (int iMes = 1; iMes <= 12; iMes++)
                this.cmbMes.Items.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(iMes).ToUpper());
            this.cmbMes.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dAhora.Month).ToUpper();
            this.LlenarDias();

            this.lblProceso.Text = "";
            this.lblAvance.Text = "";

            // Se inicializa el control navegador
            this.oSat = new FacturacionElectronica.ConSat(this.Rfc, this.ClaveCiec);
            this.oSat.PasoCompletado += oSat_PasoCompletado;
            this.oSat.RutaGuardar = this.RutaGuardar;
            this.oSat.InicializarNavegador(this.webSat);
        }

        private void cmbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbAnio.Focused)
                this.LlenarDias();
        }

        private void cmbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbMes.Focused)
                this.LlenarDias();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            this.ConsultarRecibidas();
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            // Se configuran los datos de progreso
            this.lblProceso.Text = "Descargando Xmls..";
            this.lblAvance.Text = "";
            this.pgbProceso.Value = 0;

            this.oSat.IniciarObtenerXmls();
        }

        int iVeces = 0;
        void oSat_PasoCompletado(ConSat.ConSatPaso ePaso)
        {
            switch (ePaso)
            {
                case ConSat.ConSatPaso.IniciandoSesion:
                case ConSat.ConSatPaso.SesionIniciada:
                case ConSat.ConSatPaso.BuscandoRecibidas:
                    this.pgbProceso.PerformStep();
                    break;
                case ConSat.ConSatPaso.BusquedaCompletada:
                    this.pgbProceso.PerformStep();
                    this.pgbProceso.Value = this.pgbProceso.Maximum;
                    break;
                case ConSat.ConSatPaso.IniciandoDescarga:
                    if (this.oSat.Xmls == null)
                    {
                        Util.MensajeAdvertencia("No se han obtenidos los xmls. Vuelve a intentarlo.", "");
                        break;
                    }
                    this.lblAvance.Text = ("0 de " + this.oSat.Xmls.Length.ToString());
                    this.lblAvance.Tag = 0;
                    this.pgbProceso.Maximum = this.oSat.Xmls.Length;
                    break;
                case ConSat.ConSatPaso.XmlDescargado:
                    iVeces++;
                    int iXmlDes = (Util.Entero(this.lblAvance.Tag) + 1);
                    this.lblAvance.Text = string.Format("{0} de {1}", iXmlDes, this.pgbProceso.Maximum);
                    this.lblAvance.Tag = iXmlDes;
                    this.pgbProceso.PerformStep();
                    break;
                case ConSat.ConSatPaso.DescargaCompletada:
                    this.lblAvance.Tag = 0;
                    break;

            }
            System.Threading.Thread.CurrentThread.Join(100);
        }
        
        #endregion

        #region [ Métodos ]

        private void LlenarDias()
        {
            this.cmbDia.Items.Clear();
            int iAnio = Util.Entero(this.cmbAnio.Text);
            int iMes = (this.cmbMes.SelectedIndex + 1);
            if (iAnio == 0 || iMes == 0)
                return;

            int iDias = DateTime.DaysInMonth(iAnio, iMes);
            for (int i = 1; i <= iDias; i++)
                this.cmbDia.Items.Add(i);
        }

        private void ConsultarRecibidas()
        {
            // Se configuran los datos de progreso
            this.lblProceso.Text = "Consultando Sat..";
            this.lblAvance.Text = "";
            this.pgbProceso.Value = 0;
            this.pgbProceso.Maximum = 4;

            int iAnio = Util.Entero(this.cmbAnio.Text);
            int iMes = (this.cmbMes.SelectedIndex + 1);
            int? iDia = Util.Entero(this.cmbDia.Text);
            iDia = (iDia > 0 ? iDia : null);
            if (iAnio == 0 || iMes == 0)
                return;

            // Se limpia el navegador, si aplica
            if (this.oSat != null)
                this.oSat.CerrarSesion();

            // 
            this.oSat.IniciarFacturasRecibidas(iAnio, iMes, iDia);
        }

        #endregion

    }
}
