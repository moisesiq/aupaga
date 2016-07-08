using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using System.Reflection;

namespace Refaccionaria.App
{
    public partial class mantenimiento : UserControl
    {
        // Para el Singleton *
        private static mantenimiento instance;
        public static mantenimiento Instance
        {
            get
            {
                if (mantenimiento.instance == null || mantenimiento.instance.IsDisposed)
                    mantenimiento.instance = new mantenimiento();
                return mantenimiento.instance;
            }
        }
        //

        /* public static mantenimiento Instance
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

            internal static readonly mantenimiento instance = new mantenimiento();
        }
        */

        public mantenimiento()
        {
            InitializeComponent();
        }
              
        private void mantenimiento_Load(object sender, EventArgs e)
        {
            // Se quita el tab de MaxMin, pues ya no se usa
            this.tabControlMovimientos.TabPages.Remove(this.tabKardex);

            this.tabControlMovimientos.SelectedTab = this.tabPartes;
            // tabControlMovimientos_SelectedIndexChanged(sender, e);
        }

        #region [ Eventos ]

        private void tabControlMovimientos_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControlMovimientos.SelectedTab.Name)
            {
                /*case "tabPageCatalogos": //se pasó a Configuracion

                    //if (!LogIn.VerPermiso("Administracion.Catalagos.Ver", true))
                    //{
                    //    return;
                    //}

                    if (UtilLocal.ValidarPermiso("Administracion.CatalagosGenerales.Ver", true))
                    {
                        //catalogosGenerales ca = catalogosGenerales.Instance;
                        //addControlInPanel(ca, panelCatalogosGenerales);
                        if (this.panelCatalogosGenerales.Controls.Count <= 0)
                            this.CargarControl(this.panelCatalogosGenerales, new catalogosGenerales());
                    }
                    break;
                */
                case "tabPageMovimientos":
                    //catalogosMovimientos mo = catalogosMovimientos.Instance;
                    //this.addControlInPanel(mo, panelMovimientos);
                    if (this.panelMovimientos.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.Movimientos.Ver", true))
                            return;
                        //
                        this.CargarControl(this.panelMovimientos, new catalogosMovimientos());
                    }

                    break;

                case "tabPartes":
                    // Se valida el permiso correspondiente
                    if (!UtilLocal.ValidarPermiso("Administracion.CatalogosPartes.Ver", true))
                        return;
                    //

                    catalogosPartes pa = catalogosPartes.Instance;                                        
                    addControlInPanel(pa, panelCatalogosPartes);
                    break;

                case "tabProveedores":
                    // Se valida el permiso correspondiente
                    if (!UtilLocal.ValidarPermiso("Administracion.Proveedores.Ver", true))
                        return;
                    //

                    catalogosProveedores pro = catalogosProveedores.Instance;
                    addControlInPanel(pro, panelProveedores);

                    pro.Proveedor = new Proveedor { ProveedorID = -1 };
                    pro.EsNuevo = true;                                      
                    break;

                case "tabTraspasos":
                    //catalogosTraspasos tras = catalogosTraspasos.Instance;
                    //addControlInPanel(tras, panelTraspasos);
                    if (this.panelTraspasos.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.Traspasos.Ver", true))
                            return;
                        //
                        var tras = new catalogosTraspasos();
                        this.CargarControl(this.panelTraspasos, tras);
                        tras.CargaInicial();
                    }
                    break;

                case "tabMxMn": // Máximos y mínimos
                    /* catalogosMaxMin mxmn = catalogosMaxMin.Instance;
                    this.addControlInPanel(mxmn, panelMxMn);
                    mxmn.CargaInicial();
                    */

                    // Si sólo hay un control, quiere decir que no se ha cargado la nueva opción de MaxMin
                    if (this.tabMxMn.Controls.Count <= 1)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.MaximosyMinimos.Ver", true))
                            return;
                        //
                        this.CargarControl(this.tabMxMn, new MaxMin());
                    }
                    break;

                case "tabPedidos":
                    //catalogosPedidos pedidos = catalogosPedidos.Instance;
                    //this.addControlInPanel(pedidos, panelPedidos);
                    if (this.panelPedidos.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.Pedidos.Ver", true))
                            return;
                        //
                        this.CargarControl(this.panelPedidos, new catalogosPedidos());
                    }
                    break;

                case "tabClientes":
                    //catalogosClientes clientes = catalogosClientes.Instance;                    
                    //this.addControlInPanel(clientes, panelClientes);
                    if (this.panelClientes.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.Clientes.Ver", true))
                            return;
                        //
                        this.CargarControl(this.panelClientes, new catalogosClientes());
                    }
                    break;

                case "tabKardex":
                    //catalogosKardex kardex = catalogosKardex.Instance;
                    //this.addControlInPanel(kardex, panelKardex);
                    if (this.panelKardex.Controls.Count <= 0)
                        this.CargarControl(this.panelKardex, new catalogosKardex());
                    //kardex.CargaInicial();
                    break;

                case "tbpMaster":
                    // Si no hay un control, quiere decir que no se ha cargado la opción
                    if (this.tbpMaster.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.Master.Ver", true))
                            return;
                        //
                        this.CargarControl(this.tbpMaster, new Master());
                    }
                    break;

                case "tbpInventario":
                    // Si no hay un control, quiere decir que no se ha cargado la opción
                    if (this.tbpInventario.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.Inventario.Ver", true))
                            return;
                        //
                        this.CargarControl(this.tbpInventario, new Inventario());
                    }
                    break;

                case "tbpCriteriosABC":
                    if (this.tbpCriteriosABC.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.CriteriosAbc.Ver", true))
                            return;
                        //
                        this.CargarControl(this.tbpCriteriosABC, new PartesAbc());
                    }
                    break;
                case "tbpAplicaciones":
                    if (this.tbpAplicaciones.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.Aplicaciones.Ver", true))
                            return;
                        //
                        this.CargarControl(this.tbpAplicaciones, new ValidarAplicacionesVehiculos());
                    }
                    break;
                case "tbpCapitalHumano":
                    if (this.tbpCapitalHumano.Controls.Count <= 0)
                    {
                        // Se valida el permiso correspondiente
                        if (!UtilLocal.ValidarPermiso("Administracion.CapitalHumano.Ver", true))
                            return;
                        //
                        this.CargarControl(this.tbpCapitalHumano, new CapitalHumano());
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion
                
        #region [ Metodos ]
        
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

        private void CargarControl(Control oContenedor, Control oHijo)
        {
            oHijo.Dock = DockStyle.Fill;
            oContenedor.Controls.Add(oHijo);
            oContenedor.Tag = oHijo;
            oHijo.BringToFront();
            oHijo.Show();
        }

        #endregion

        #region [ Públicos ]

        public void EjecutarAccesoDeTeclado(Keys oTecla)
        {
            switch (this.tabControlMovimientos.SelectedTab.Name)
            {
                case "tabClientes":
                    if (oTecla == Keys.F5)
                        clientes.Instance.CargarCliente(clientes.Instance.ClienteID);
                    break;
            }
        }

        public void SeleccionarOpcion(string sNombreTab)
        {
            this.tabControlMovimientos.SelectedTab = this.tabControlMovimientos.TabPages[sNombreTab];
        }

        #endregion

    }
}
