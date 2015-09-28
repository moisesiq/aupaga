using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Contabilidad : UserControl
    {
        // Para el Singleton *
        private static Contabilidad instance;
        public static Contabilidad Instance
        {
            get
            {
                if (Contabilidad.instance == null || Contabilidad.instance.IsDisposed)
                    Contabilidad.instance = new Contabilidad();
                return Contabilidad.instance;
            }
        }
        //

        public Contabilidad()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void Contabilidad_Load(object sender, EventArgs e)
        {
            this.tabContabilidad.SelectedTab = this.tbpCuentasPolizas;

            // Se valida el permiso para mostrar o no la pestaña de reserva
            if (!UtilDatos.ValidarPermiso("Contabilidad.Reserva.Ver"))
                this.tabContabilidad.TabPages.Remove(this.tbpReserva);
            
            // Para que se actualicen* los cambios antes de iniciar la carga predeterminada
            Application.DoEvents();
        }

        private void tabContabilidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabContabilidad.SelectedTab.Name)
            {
                case "tbpConfiguracion":
                    break;
                case "tbpCuentasContables":
                    if (this.tbpCuentasContables.Controls.Count <= 0)
                        this.tbpCuentasContables.Controls.Add(new CuentasContables() { Dock = DockStyle.Fill });
                    break;
                case "tbpCuentasPolizas":
                    if (this.tbpCuentasPolizas.Controls.Count <= 0)
                        this.tbpCuentasPolizas.Controls.Add(new CuentasPolizas() { Dock = DockStyle.Fill });
                    break;
                case "tbpCuentasPorSemana":
                    if (this.tbpCuentasPorSemana.Controls.Count <= 0)
                        this.tbpCuentasPorSemana.Controls.Add(new CuentasPorSemana() { Dock = DockStyle.Fill });
                    break;
                case "tbpConfigAfectaciones":
                    if (this.tbpConfigAfectaciones.Controls.Count <= 0)
                        this.tbpConfigAfectaciones.Controls.Add(new ConfigAfectaciones() { Dock = DockStyle.Fill });
                    break;
                case "tbpBancos":
                    if (this.tbpBancos.Controls.Count <= 0)
                        this.tbpBancos.Controls.Add(new ContaBancos() { Dock = DockStyle.Fill });
                    break;
                case "tbpGastosParaPolizas":
                    if (this.tbpGastosParaPolizas.Controls.Count <= 0)
                        this.tbpGastosParaPolizas.Controls.Add(new GastosParaPolizas() { Dock = DockStyle.Fill });
                    break;
                case "tbpReserva":
                    if (this.tbpReserva.Controls.Count <= 0)
                        this.tbpReserva.Controls.Add(new ContaReserva() { Dock = DockStyle.Fill });
                    break;
                case "tbpBalance":
                    if (this.tbpBalance.Controls.Count <= 0)
                        this.tbpBalance.Controls.Add(new ContaBalance() { Dock = DockStyle.Fill });
                    break;
                case "tbpEstadoDeResultados":
                    if (this.tbpEstadoDeResultados.Controls.Count <= 0)
                        this.tbpEstadoDeResultados.Controls.Add(new ContaEstadoDeResultados() { Dock = DockStyle.Fill });
                    break;
            }
        }

        private void trvCatalogos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "Consumibles":
                    Control oControl = (e.Node.Tag == null ? new Consumibles() : (e.Node.Tag as Consumibles));
                    this.MostrarControl(e.Node, oControl);
                    break;
            }
        }

        #endregion
                                
        #region [ Métodos ]

        private void MostrarControl(TreeNode oNodo, Control oControl)
        {
            this.pnlContenido.Controls.Clear();
            oControl.Dock = DockStyle.Fill;
            this.pnlContenido.Controls.Add(oControl);
            if (oNodo.Tag != null)
                oNodo.Tag = oControl;
        }

        #endregion

        #region [ Públicos ]

        public void EjecutarAccesoDeTeclado(Keys eTecla)
        {
            switch (this.tabContabilidad.SelectedTab.Name)
            {
                case "tbpCuentasContables":
                    var oCuentasContables = (this.tbpCuentasContables.Controls[0] as CuentasContables);
                    switch (eTecla)
                    {
                        case Keys.F2:
                            oCuentasContables.SeleccionarCuadroBusqueda();
                            break;
                        case Keys.F5:
                            oCuentasContables.LlenarCuentasTotales();
                            break;
                    }
                    break;
                case "tbpCuentasPolizas":
                    var oCuentasPolizas = (this.tbpCuentasPolizas.Controls[0] as CuentasPolizas);
                    switch (eTecla)
                    {
                        case Keys.F2:
                            oCuentasPolizas.SeleccionarCuadroBusqueda();
                            break;
                        case Keys.F5:
                            oCuentasPolizas.LlenarArbol();
                            break;
                    }
                    break;
                case "tbpCuentasPorSemana":
                    switch (eTecla)
                    {
                        case Keys.F5:
                            var oCuentasPorSem = (this.tbpCuentasPorSemana.Controls[0] as CuentasPorSemana);
                            oCuentasPorSem.CargarDatos();
                            break;
                    }
                    break;
                case "tbpGastosParaPolizas":
                    switch (eTecla)
                    {
                        case Keys.F5:
                            var oGastosPol = (this.tbpGastosParaPolizas.Controls[0] as GastosParaPolizas);
                            oGastosPol.LlenarGastos();
                            break;
                    }
                    break;
            }
        }

        #endregion

    }
}
