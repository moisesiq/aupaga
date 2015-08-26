using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class catalogosGenerales : UserControl
    {       

        // Para el Singleton
        private static catalogosGenerales _Instance;
        public static catalogosGenerales Instance
        {
            get
            {
                if (catalogosGenerales._Instance == null || catalogosGenerales._Instance.IsDisposed)
                    catalogosGenerales._Instance = new catalogosGenerales();
                return catalogosGenerales._Instance;
            }
        }
        //

        public catalogosGenerales()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void catalogosGenerales_Load(object sender, EventArgs e)
        {            
            treeViewCatalogos.ExpandAll();
        }

        private void treeViewCatalogos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Name.ToString())
            {
                case "NodeMedidas":
                    medidas medidas = medidas.Instance;
                    medidas.txtBusqueda.Clear();
                    medidas.ActualizarListado();
                    this.addControlInPanel(medidas);
                    break;

                case "NodeSucursales":
                    sucursales sucursales = sucursales.Instance;
                    sucursales.txtBusqueda.Clear();
                    sucursales.ActualizarListado();
                    this.addControlInPanel(sucursales);
                    sucursales.txtBusqueda.Focus();
                    break;

                case "NodeTipoMovimientos":
                    tipoMovimientos tipoMovimientos = tipoMovimientos.Instance;
                    tipoMovimientos.txtBusqueda.Clear();
                    tipoMovimientos.ActualizarListado();
                    this.addControlInPanel(tipoMovimientos);
                    break;

                case "NodeTipoOperaciones":
                    tipoOperaciones tipoOperaciones = tipoOperaciones.Instance;
                    tipoOperaciones.txtBusqueda.Clear();
                    tipoOperaciones.ActualizarListado();
                    this.addControlInPanel(tipoOperaciones);
                    break;

                case "NodeTipoPagos":
                    tipoPagos tipoPagos = tipoPagos.Instance;
                    tipoPagos.txtBusqueda.Clear();
                    tipoPagos.ActualizarListado();
                    this.addControlInPanel(tipoPagos);
                    break;

                case "NodeConfiguraciones":
                    this.addControlInPanel(configuraciones.Instance);
                    break;

                case "NodeUsuarios":
                    usuarios usuarios = usuarios.Instance;
                    usuarios.txtBusqueda.Clear();
                    usuarios.ActualizarListado();
                    this.addControlInPanel(usuarios);
                    break;

                case "NodePerfiles":
                    perfiles perfiles = perfiles.Instance;
                    perfiles.txtBusqueda.Clear();
                    perfiles.ActualizarListado();
                    this.addControlInPanel(perfiles);
                    break;

                case "NodePermisos":
                    permisos permisos = permisos.Instance;
                    permisos.txtBusqueda.Clear();
                    permisos.ActualizarListado();
                    this.addControlInPanel(permisos);
                    break;

                case "NodeAsistencias":
                    AsistenciaUsuario asistencia = AsistenciaUsuario.Instance;
                    asistencia.ActualizarListado();
                    this.addControlInPanel(asistencia);
                    break;

                case "NodeNominaOficialCuentas":
                    this.addControlInPanel(CatNominaOficialCuentas.Instance);
                    break;

                case "NodeNominaOficialUsuarios":
                    this.addControlInPanel(CatNominaOficialUsuarios.Instance);
                    break;

                case "NodeMarcas":
                    marcas marcas = marcas.Instance;
                    marcas.txtBusqueda.Clear();
                    marcas.ActualizarListado();
                    this.addControlInPanel(marcas);
                    break;

                case "NodeModelos":
                    modelos modelos = modelos.Instance;
                    modelos.txtBusqueda.Clear();
                    modelos.ActualizarListado();
                    this.addControlInPanel(modelos);
                    break;

                case "NodeMotoresExistentes":
                    motoresExistentes existentes = motoresExistentes.Instance;
                    existentes.txtBusqueda.Clear();
                    existentes.ActualizarListado();
                    this.addControlInPanel(existentes);
                    break;

                case "NodeMotores":
                    motores motores = motores.Instance;
                    motores.txtBusqueda.Clear();
                    motores.ActualizarListado();
                    this.addControlInPanel(motores);
                    break;

                case "NodeVehiculosTipos":
                    this.addControlInPanel(CatVehiculosTipos.Instance);
                    break;

                case "NodeLineas":
                    lineas lineas = lineas.Instance;
                    lineas.txtBusqueda.Clear();
                    lineas.ActualizarListado();
                    this.addControlInPanel(lineas);
                    break;

                case "NodeMarcaPartes":
                    marcaPartes marcaPartes = marcaPartes.Instance;
                    marcaPartes.txtBusqueda.Clear();
                    marcaPartes.ActualizarListado();
                    this.addControlInPanel(marcaPartes);
                    break;

                case "NodeSistemas":
                    sistemas sistemas = sistemas.Instance;
                    sistemas.txtBusqueda.Clear();
                    sistemas.ActualizarListado();
                    this.addControlInPanel(sistemas);
                    break;

                case "NodeSubsistemas":
                    subsistemas subsistemas = subsistemas.Instance;
                    subsistemas.txtBusqueda.Clear();
                    subsistemas.ActualizarListado();
                    this.addControlInPanel(subsistemas);
                    break;
                                    
                case "NodeUbicacion":
                    ubicaciones ubicaciones = ubicaciones.Instance;
                    ubicaciones.txtBusqueda.Clear();
                    ubicaciones.ActualizarListado();
                    addControlInPanel(ubicaciones);
                    break;

                case "NodeCaracteristicasSistemas":
                    sistemasCaracteristicas SistemasC = sistemasCaracteristicas.Instance;
                    SistemasC.txtBusqueda.Clear();
                    SistemasC.ActualizarListado();
                    this.addControlInPanel(SistemasC);
                    break;

                case "NodePaqueterias":
                    paqueterias paq = paqueterias.Instance;
                    paq.txtBusqueda.Clear();
                    paq.ActualizarListado();
                    this.addControlInPanel(paq);
                    break;

                //case "NodeCriteriosAbc": //Se movieron a mantenimiento.cs
                //    //criteriosAbc cri = criteriosAbc.Instance;
                //    PartesAbc cri = PartesAbc.Instance;
                //    this.addControlInPanel(cri);
                //    break;

                //case "NodeAplicaciones":
                //    this.addControlInPanel(ValidarAplicacionesVehiculos.Instance);
                //    break;

                case "NodeBancos":
                    this.addControlInPanel(Bancos.Instance);
                    break;

                case "NodeCuentasBancarias":
                    this.addControlInPanel(CatCuentasBancarias.Instance);
                    break;

                case "NodeVentasEgresos":
                    this.addControlInPanel(VentasEgresos.Instance);
                    break;

                case "NodeVentasIngresos":
                    this.addControlInPanel(VentasIngresos.Instance);
                    break;

                case "NodeMetasSucursales":
                    this.addControlInPanel(CatMetasSucursales.Instance);
                    break;
                    
                case "NodeMetasVendedores":
                    this.addControlInPanel(CatMetasVendedores.Instance);
                    break;

                case "NodeMetas":
                    this.addControlInPanel(CatMetas.Instance);
                    break;

                case "NodeLeyendas":
                    this.addControlInPanel(CatLeyendasVenta.Instance);
                    break;

                case "NodeCiudades":                    
                    this.addControlInPanel(ciudades.Instance);
                    break;

                case "NodeEstados":
                    var est = estados.Instance;
                    est.ActualizarListado();
                    this.addControlInPanel(est);                    
                    break;

                case "NodeMunicipios":
                    var mun = municipios.Instance;
                    mun.ActualizarListado();
                    this.addControlInPanel(mun);                    
                    break;

                case "NodeInventarioUsuario":
                    this.addControlInPanel(CatInventarioUsuarios.Instance);
                    break;

                case "NodeInventarioPeriodicidad":
                    this.addControlInPanel(CatInventarioPeriodicidad.Instance);
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region [ Metodos ]

        private void addControlInPanel(object controlHijo)
        {
            this.panelContenedorCatalogos.Controls.Clear();
            if (this.panelContenedorCatalogos.Controls.Count > 0)
                this.panelContenedorCatalogos.Controls.RemoveAt(0);
            UserControl usc = controlHijo as UserControl;
            usc.Dock = DockStyle.Fill;
            this.panelContenedorCatalogos.Controls.Add(usc);
            this.panelContenedorCatalogos.Tag = usc;
            usc.Show();
        }

        #endregion

    }
}
