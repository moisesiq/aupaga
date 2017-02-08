using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CuadroMetas : UserControl
    {
        public CuadroMetas()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroMetas_Load(object sender, EventArgs e)
        {
            //CuadroControlPermisos PermisosC = new CuadroControlPermisos();
            // Se llenan los controles iniciales
            //var oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            var oSucursales = CuadroControlPermisos.ValidarPermisosTiendaCuadroMultiple(CuadroControlPermisos.GetTabPage);

            if (oSucursales.Count > 2)
            {
                //oSucursales.Insert(0, new Sucursal() { SucursalID = 0, NombreSucursal = "Todas" });
                oSucursales.Insert(0, new Sucursal() { NombreSucursal = "(TODAS)" });
            }

            //oSucursales.Insert(0, new Sucursal() { NombreSucursal = "(TODAS)" });
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", oSucursales);
            // this.cmbSucursal.SelectedValue = GlobalClass.SucursalID;
            var oFechas = UtilDatos.FechasDeComisiones(DateTime.Now);
            this.dtpDesde.Value = oFechas.Valor1;
            this.dtpHasta.Value = oFechas.Valor2;
            this.cmbVendedor.CargarDatos("UsuarioID", "NombreUsuario", Datos.GetListOf<Usuario>(c => c.Estatus));
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtpDesde_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void dtpHasta_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void cmbVendedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            // Se llenan los datos de metas
            this.metasMetas.Desde = this.dtpDesde.Value;
            this.metasMetas.Hasta = this.dtpHasta.Value;
            this.metasMetas.SucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            this.metasMetas.UsuarioID = Util.Entero(this.cmbVendedor.SelectedValue);
            this.metasMetas.Preparar(this.metasMetas.UsuarioID);

            // Se llenan los datos
            if (this.metasMetas.SucursalID <= 0)
            {
                this.metasMetas.CargarDatosTodasLasSucursales();
            } else if (this.metasMetas.UsuarioID <= 0)
            {
                this.metasMetas.CargarDatosGeneral();
            }
            else
            {
                this.metasMetas.CargarDatos();
            }
        }

        #endregion
                
    }
}
