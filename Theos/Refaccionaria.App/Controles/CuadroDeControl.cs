using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroDeControl : UserControl
    {
        // Para el Singleton *
        private static CuadroDeControl instance;
        public static CuadroDeControl Instance
        {
            get
            {
                if (CuadroDeControl.instance == null || CuadroDeControl.instance.IsDisposed)
                    CuadroDeControl.instance = new CuadroDeControl();
                return CuadroDeControl.instance;
            }
        }
        //

        public CuadroDeControl()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroDeControl_Load(object sender, EventArgs e)
        {
            this.tabCuadroDeControl_SelectedIndexChanged(this, null);
        }

        private void tabCuadroDeControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabCuadroDeControl.SelectedTab.Name)
            {


                case "tbpVentas":
                    CuadroControlPermisos.GetTabPage = "Ventas";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Ventas.Ver", true))
                        return;

                    if (this.tbpVentas.Controls.Count <= 0)
                        this.tbpVentas.Controls.Add(new CuadroVentas() { Dock = DockStyle.Fill });
                    break;
                case "tbpClientes":
                    CuadroControlPermisos.GetTabPage = "Clientes";

                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Clientes.Ver", true))
                        return;

                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;
                    
                    if (this.tbpClientes.Controls.Count <= 0)
                        this.tbpClientes.Controls.Add(new CuadroClientes() { Dock = DockStyle.Fill });
                    break;
                case "tbpVendedores":
                    CuadroControlPermisos.GetTabPage = "Vendedores";

                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Vendedores.Ver", true))
                        return;

                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpVendedores.Controls.Count <= 0)
                        this.tbpVendedores.Controls.Add(new CuadroVendedores() { Dock = DockStyle.Fill });
                    break;
                case "tbpProveedores":
                    CuadroControlPermisos.GetTabPage = "Proveedores";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Proveedores.Ver", true))
                        return;

                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpProveedores.Controls.Count <= 0)
                        this.tbpProveedores.Controls.Add(new CuadroProveedores() { Dock = DockStyle.Fill });
                    break;
                case "tbpMarcas":
                    CuadroControlPermisos.GetTabPage = "Marcas";

                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Marcas.Ver", true))
                        return;

                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpMarcas.Controls.Count <= 0)
                        this.tbpMarcas.Controls.Add(new CuadroMarcas() { Dock = DockStyle.Fill });
                    break;
                case "tbpLineas":
                    CuadroControlPermisos.GetTabPage = "Lineas";

                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Lineas.Ver", true))
                        return;

                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpLineas.Controls.Count <= 0)
                        this.tbpLineas.Controls.Add(new CuadroLineas() { Dock = DockStyle.Fill });
                    break;
                case "tbpCancelaciones":
                    CuadroControlPermisos.GetTabPage = "Cancelaciones";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Cancelaciones.Ver", true))
                        return;
                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpCancelaciones.Controls.Count <= 0)
                        this.tbpCancelaciones.Controls.Add(new CuadroCancelaciones() { Dock = DockStyle.Fill });
                    break;
                case "tbpSemanas":
                    CuadroControlPermisos.GetTabPage = "Semana";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Semanas.Ver", true))
                        return;
                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpSemanas.Controls.Count <= 0)
                        this.tbpSemanas.Controls.Add(new CuadroSemanas() { Dock = DockStyle.Fill });
                    break;
                case "tbpMetas":
                    CuadroControlPermisos.GetTabPage = "Metas";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Metas.Ver", true))
                        return;
                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpMetas.Controls.Count <= 0)
                        this.tbpMetas.Controls.Add(new CuadroMetas() { Dock = DockStyle.Fill });
                    break;
                case "tbpExistencias":
                    CuadroControlPermisos.GetTabPage = "Existencias";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Existencias.Ver", true))
                        return;
                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpExistencias.Controls.Count <= 0)
                        this.tbpExistencias.Controls.Add(new CuadroExistencias() { Dock = DockStyle.Fill });
                    break;
                case "tbpSucursales":
                    CuadroControlPermisos.GetTabPage = "Sucursales";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.Sucursales.Ver", true))
                        return;
                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpSucursales.Controls.Count <= 0)
                        this.tbpSucursales.Controls.Add(new CuadroSucursales() { Dock = DockStyle.Fill });
                    break;
                case "tbpCobranza":
                    CuadroControlPermisos.GetTabPage = "CreditoCobranza";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.CreditoCobranza.Ver", true))
                        return;
                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpCobranza.Controls.Count <= 0)
                        this.tbpCobranza.Controls.Add(new CuadroCobranza() { Dock = DockStyle.Fill });
                    break;
                case "tbpVip":
                    CuadroControlPermisos.GetTabPage = "VIP";
                    // Se verifica que el usuario tenga permisos para entrar a esta opción
                    if (!UtilLocal.ValidarPermiso("CuadroDeControl.VIP.Ver", true))
                        return;
                    if (!CuadroControlPermisos.ValidarTodosPermisos(CuadroControlPermisos.GetTabPage))
                        return;

                    if (this.tbpVip.Controls.Count <= 0)
                        this.tbpVip.Controls.Add(new CuadroVip() { Dock = DockStyle.Fill });
                    break;
                default :
                    CuadroControlPermisos.GetTabPage = "";
                    break;
            }
        }

        #endregion


    }
}
