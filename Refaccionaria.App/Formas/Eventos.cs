using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using FastReport;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Eventos : Form
    {
        // Para el Singleton *
        private static Eventos instance;
        public static Eventos Instance
        {
            get
            {
                if (Eventos.instance == null || Eventos.instance.IsDisposed)
                    Eventos.instance = new Eventos();
                return Eventos.instance;
            }
        }
        //

        public Eventos()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria;
            this.Height = (Principal.Instance.Height - 16 - 2);
            this.Top = (Principal.Instance.Top + 8);
            this.Left = (Principal.Instance.Width - this.Width - 16);
        }

        #region [ Eventos ]

        private void Eventos_Load(object sender, EventArgs e)
        {
            this.ActiveControl = this.rdbHoy;
            this.rdbHoy.Checked = true;
        }

        private void rdbHoy_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbHoy.Focused && this.rdbHoy.Checked)
                this.LlenarEventos(true);
        }

        private void rdbManiana_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbManiana.Focused && this.rdbManiana.Checked)
                this.LlenarEventos(false);
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();

            var oDatos = new List<CobroCliente>();
            foreach (Control oControl in this.flpEventos.Controls)
            {
                oDatos.Add(new CobroCliente()
                {
                    Fecha = (oControl.Controls["dtpFecha"] as DateTimePicker).Value,
                    Cliente = oControl.Controls["lblCliente"].Text,
                    Adeudo = Helper.ConvertirDecimal(oControl.Controls["lblAdeudo"].Text),
                    Vencido = Helper.ConvertirDecimal(oControl.Controls["lblVencido"].Text),
                    Contacto = oControl.Controls["lblContacto"].Text
                });
            }

            var oRep = new Report();
            oRep.Load(UtilLocal.RutaReportes("ClientesAvisos.frx"));
            oRep.RegisterData(oDatos, "Avisos");
            Cargando.Cerrar();
            UtilLocal.EnviarReporteASalida("Reportes.Clientes.Avisos", oRep);
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            var dtp = (sender as DateTimePicker);
            var pnl = (dtp.Parent as Panel);

            dtp.Tag = true;
            pnl.Controls["lblCambio"].Visible = true;
        }

        private void btnBien_Click(object sender, EventArgs e)
        {
            var pnl = ((sender as Button).Parent as Panel);

            int iId = Helper.ConvertirEntero(pnl.Tag);
            if (iId > 0)
            {
                // Se verifica si se debe marcar revisado o cambiar la fecha
                if (pnl.Controls["dtpFecha"].Tag == null)
                {
                    this.MarcarEventoRevisado(iId);
                }
                else
                {
                    DateTime dFecha = (pnl.Controls["dtpFecha"] as DateTimePicker).Value;
                    this.CambiarFechaEvento(iId, dFecha);
                }
                // Se quita el control del listado
                this.flpEventos.Controls.Remove((sender as Button).Parent);
            }
        }

        #endregion

        #region [ Privados ]

        private void MarcarEventoRevisado(int iEventoID)
        {
            var oEvento = General.GetEntity<ClienteEventoCalendario>(c => c.ClienteEventoCalendarioID == iEventoID);
            oEvento.Revisado = true;
            Guardar.Generico<ClienteEventoCalendario>(oEvento);
        }

        private void CambiarFechaEvento(int iEventoID, DateTime dFecha)
        {
            var oEvento = General.GetEntity<ClienteEventoCalendario>(c => c.ClienteEventoCalendarioID == iEventoID);
            oEvento.Fecha = dFecha;
            Guardar.Generico<ClienteEventoCalendario>(oEvento);
        }

        #endregion

        #region [ Públicos ]

        public void LlenarEventos(bool bHoy)
        {
            Cargando.Mostrar();

            DateTime dHoy = DateTime.Now.Date;
            DateTime dManiana = dHoy.AddDays(1);
            DateTime dPasadoManiana = dManiana.AddDays(1);
            List<ClientesEventosCalendarioView> oAlertas;
            if (bHoy)
                oAlertas = General.GetListOf<ClientesEventosCalendarioView>(c => c.Fecha >= dHoy && c.Fecha < dManiana && !c.Revisado);
            else
                oAlertas = General.GetListOf<ClientesEventosCalendarioView>(c => c.Fecha >= dManiana && c.Fecha < dPasadoManiana && !c.Revisado);
            oAlertas = oAlertas.OrderBy(c => c.Fecha).ToList();

            this.LimpiarEventos();
            foreach (var oReg in oAlertas)
            {
                /* if (oReg.Fecha < DateTime.Now)
                    AdmonProc.MostrarRecordatorioClientes(oReg.ClienteEventoCalendarioID);
                else
                    Program.oTimers.Add("AlertaPedido" + Program.oTimers.Count.ToString(), new System.Threading.Timer(new TimerCallback(AdmonProc.MostrarRecordatorioClientes)
                        , oReg.ClienteEventoCalendarioID, (int)(oReg.Fecha - DateTime.Now).TotalMilliseconds, Timeout.Infinite));
                */

                this.AgregarEvento(oReg.ClienteEventoCalendarioID, oReg.Fecha, oReg.Cliente, oReg.Evento);
            }

            Cargando.Cerrar();
        }

        public void AgregarEvento(int iId, DateTime dFecha, string sTitulo, string sEvento)
        {
            var oEvento = General.GetEntity<ClienteEventoCalendario>(c => c.ClienteEventoCalendarioID == iId);
            var oAdeudo = General.GetEntity<ClientesCreditoView>(c => c.ClienteID == oEvento.ClienteID);

            var pnlEvento = new Panel();
            pnlEvento.Tag = iId;
            this.flpEventos.Controls.Add(pnlEvento);
            Helper.CopiarPropiedades(this.pnlMuestra, pnlEvento, "Visible");

            foreach (Control oControl in this.pnlMuestra.Controls)
            {
                // Se crea el control nuevo
                Control oNuevo = null;
                if (oControl is DateTimePicker)
                    oNuevo = new DateTimePicker() { Name = "dtp" };
                else if (oControl is Button)
                    oNuevo = new Button();
                else if (oControl is Label)
                    oNuevo = new Label() { TextAlign = (oControl as Label).TextAlign };

                // Se copian las propiedades de la base/muestra
                Helper.CopiarPropiedades(oControl, oNuevo, "Visible");
                oNuevo.Name = oControl.Name;

                // Se configuran los eventos para los controles especiales
                if (oControl == this.btnRevisado)
                {
                    oNuevo.Click += this.btnBien_Click;
                }
                else if (oControl == this.dtpFecha)
                {
                    var dtp = (oNuevo as DateTimePicker);
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = "dd/MM/yyy hh:mm tt";
                    dtp.Checked = true;
                    dtp.Value = oEvento.Fecha;
                    dtp.ValueChanged += dtpFecha_ValueChanged;
                }
                else if (oControl == this.lblCambio)
                {
                    oNuevo.Visible = false;
                }
                else if (oControl == this.lblCliente)
                {
                    oNuevo.Text = oAdeudo.Nombre;
                }
                else if (oControl == this.lblVencido || oControl == this.lblAdeudo)
                {
                    oNuevo.Text = (oControl == this.lblVencido ? oAdeudo.AdeudoVencido : oAdeudo.Adeudo).Valor().ToString(GlobalClass.FormatoMoneda);
                }
                else if (oControl == this.lblContacto)
                {
                    oNuevo.Text = oAdeudo.CobranzaContacto;
                }
                
                // Se agrega al panel
                pnlEvento.Controls.Add(oNuevo);
                pnlEvento.Refresh();
                pnlEvento.Update();
            }
        }

        public void LimpiarEventos()
        {
            this.flpEventos.Controls.Clear();
        }
        
        #endregion

    }

    class CobroCliente
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public decimal Adeudo { get; set; }
        public decimal Vencido { get; set; }
        public string Contacto { get; set; }
    }
}