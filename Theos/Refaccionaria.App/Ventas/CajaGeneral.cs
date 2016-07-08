using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class CajaGeneral : UserControl
    {
        public VentasCaja oVentasCaja;

        public CajaDetalleCorte ctlDetalleCorte;
        public CajaFondo ctlFondeDeCaja;
        public CajaGastos ctlGastos;
        public CajaRefuerzo ctlRefuerzo;
        public CajaResguardo ctlResguardo;
        public CajaVentasPorCobrar ctlVentasPorCobrar;
        public CajaVentasCambios ctlVentasCambios;
        public CajaCambioTurno ctlCambioTurno;
        public CajaCorte ctlCorte;

        public CajaGeneral()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CajaGeneral_Load(object sender, System.EventArgs e)
        {
            this.tabOpciones_SelectedIndexChanged(sender, e);

            this.CambiarOpcion("tbpCorteDetalle");
        }

        private void tabOpciones_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region [ Públicos ]

        public void CambiarOpcion(string sOpcion)
        {
            // Se oculatan todos los tabs
            foreach (TabPage oTab in this.tabOpciones.TabPages)
                this.tabOpciones.TabPages.Remove(oTab);

            // Se ocultan las los controles de detalle que se agregan sólo en algunas opciones
            if (this.ctlVentasPorCobrar != null)
                this.ctlVentasPorCobrar.pnlParaDetalle.Hide();
            if (this.ctlVentasCambios != null)
                this.ctlVentasCambios.ctlCobro.Hide();

            // Se ejecuta la acción correspondiente
            switch (sOpcion)
            {
                case "tbpCorteDetalle":
                case "tbpCorte":
                    this.tabOpciones.TabPages.Add(this.tbpCorteDetalle);
                    if (this.ctlDetalleCorte == null)
                    {
                        this.ctlDetalleCorte = new CajaDetalleCorte() { Dock = DockStyle.Fill };
                        this.tbpCorteDetalle.Controls.Add(this.ctlDetalleCorte);
                    }
                    this.tabOpciones.TabPages.Add(this.tbpCorte);
                    if (this.ctlCorte == null)
                    {
                        this.ctlCorte = new CajaCorte() { Dock = DockStyle.Fill };
                        this.tbpCorte.Controls.Add(this.ctlCorte);
                    }

                    if (sOpcion == "tbpCorte")
                        this.tabOpciones.SelectedTab = this.tbpCorte;

                    break;
                case "tbpFondoDeCaja":
                    this.tabOpciones.TabPages.Add(this.tbpFondoDeCaja);
                    if (this.ctlFondeDeCaja == null)
                    {
                        this.ctlFondeDeCaja = new CajaFondo() { Dock = DockStyle.Fill };
                        this.tbpFondoDeCaja.Controls.Add(this.ctlFondeDeCaja);
                    }
                    break;
                case "tbpGastos":
                    this.tabOpciones.TabPages.Add(this.tbpGastos);
                    if (this.ctlGastos == null)
                    {
                        this.ctlGastos = new CajaGastos() { Dock = DockStyle.Fill };
                        this.tbpGastos.Controls.Add(this.ctlGastos);
                    }
                    break;
                case "tbpRefuerzo":
                    this.tabOpciones.TabPages.Add(this.tbpRefuerzo);
                    if (this.ctlRefuerzo == null)
                    {
                        this.ctlRefuerzo = new CajaRefuerzo() { Dock = DockStyle.Fill, MostrarTotal = true };
                        this.tbpRefuerzo.Controls.Add(this.ctlRefuerzo);
                    }
                    break;
                case "tbpResguardo":
                    this.tabOpciones.TabPages.Add(this.tbpResguardo);
                    if (this.ctlResguardo == null)
                    {
                        this.ctlResguardo = new CajaResguardo() { Dock = DockStyle.Fill };
                        this.tbpResguardo.Controls.Add(this.ctlResguardo);
                    }
                    break;
                case "tbpVentasPorCobrar":
                    this.tabOpciones.TabPages.Add(this.tbpVentasPorCobrar);
                    if (this.ctlVentasPorCobrar == null)
                    {
                        this.ctlVentasPorCobrar = new CajaVentasPorCobrar() { Dock = DockStyle.Fill };
                        this.ctlVentasPorCobrar.oVentasCaja = this.oVentasCaja;
                        this.tbpVentasPorCobrar.Controls.Add(this.ctlVentasPorCobrar);
                    }
                    else
                    {
                        this.ctlVentasPorCobrar.pnlParaDetalle.BringToFront();
                        this.ctlVentasPorCobrar.pnlParaDetalle.Show();
                    }

                    // Se agrega lo de Control de Cascos
                    this.tabOpciones.TabPages.Add(this.tbpControlDeCascos);
                    if (this.tbpControlDeCascos.Controls.Count <= 0)
                        this.tbpControlDeCascos.Controls.Add(new ControlDeCascos() { Dock = DockStyle.Fill });
                    
                    break;
                case "tbpVentasCambios":
                    this.tabOpciones.TabPages.Add(this.tbpVentasCambios);
                    if (this.ctlVentasCambios == null)
                    {
                        this.ctlVentasCambios = new CajaVentasCambios() { Dock = DockStyle.Fill };
                        this.ctlVentasCambios.oVentasCaja = this.oVentasCaja;
                        this.tbpVentasCambios.Controls.Add(this.ctlVentasCambios);
                    }
                    else
                    {
                        this.ctlVentasCambios.ctlCobro.BringToFront();
                        this.ctlVentasCambios.ctlCobro.Show();
                    }
                    break;
                case "tbpCambioTurno":
                    this.tabOpciones.TabPages.Add(this.tbpCambioTurno);
                    if (this.ctlCambioTurno == null)
                    {
                        this.ctlCambioTurno = new CajaCambioTurno() { Dock = DockStyle.Fill };
                        this.tbpCambioTurno.Controls.Add(this.ctlCambioTurno);
                    }
                    break;
            }
        }

        public void ActualizarCaja()
        {
            string sOpcionActual = this.tabOpciones.SelectedTab.Name;
            switch (sOpcionActual)
            {
                case "tbpCorteDetalle":
                case "tbpCorte":
                    this.ctlDetalleCorte.ActualizarDatos();
                    this.ctlCorte.ActualizarDatos();
                    break;
                case "tbpFondoDeCaja":
                    this.ctlFondeDeCaja.ActualizarDatos();
                    break;
                case "tbpGastos":
                    this.ctlGastos.ActualizarDatos();
                    break;
                case "tbpRefuerzo":
                    this.ctlRefuerzo.ActualizarDatos();
                    break;
                case "tbpResguardo":
                    this.ctlResguardo.ActualizarDatos();
                    break;
                case "tbpVentasPorCobrar":
                    this.ctlVentasPorCobrar.ActualizarDatos();
                    break;
                case "tbpControlDeCascos":
                    (this.tbpControlDeCascos.Controls[0] as ControlDeCascos).CargarDatos();
                    break;
                case "tbpVentasCambios":
                    this.ctlVentasCambios.ActualizarDatos();
                    break;
                case "tbpCambioTurno":
                    this.ctlCambioTurno.ActualizarDatos();
                    break;
            }
        }

        #endregion
    }
}
