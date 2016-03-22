using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CotizacionesVentas : UserControl
    {
        public ClientesDatosView oCliente;

        public CotizacionesVentas()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CotizacionesVentas_Load(object sender, EventArgs e)
        {
            // Se cargan los combos
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.dtpDesde.Value = DateTime.Now.DiaPrimero();
            this.dtpHasta.Value = DateTime.Now.DiaUltimo();
            this.cmbVendedor.CargarDatos("UsuarioID", "NombreUsuario", General.GetListOf<Usuario>(c => c.Estatus).OrderBy(c => c.NombreUsuario).ToList());
            this.cmbLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(c => c.Estatus).OrderBy(c => c.NombreLinea).ToList());
            this.cmbMarca.CargarDatos("MarcaParteID", "NombreMarcaParte", General.GetListOf<MarcaParte>(c => c.Estatus).OrderBy(c => c.NombreMarcaParte).ToList());
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.AplicarFiltro();
        }

        #endregion

        #region [ Privados ]

        protected virtual void AplicarFiltro()
        {
            Cargando.Mostrar();

            //
            int iClienteID = (this.oCliente == null ? 0 : this.oCliente.ClienteID);
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            DateTime dDesde = this.dtpDesde.Value.Date;
            DateTime dHastaMas1 = this.dtpHasta.Value.Date.AddDays(1);
            int iVendedorID = Helper.ConvertirEntero(this.cmbVendedor.SelectedValue);
            int iLineaID = Helper.ConvertirEntero(this.cmbLinea.SelectedValue);
            int iMarcaID = Helper.ConvertirEntero(this.cmbMarca.SelectedValue);
            string sDescripcion = this.txtDescripcion.Text;

            var oLista = General.GetListOf<VentasCotizacionesDetalleAvanzadoView>(c =>
                (iClienteID == 0 || c.ClienteID == iClienteID)
                && (iSucursalID == 0 || c.SucursalID == iSucursalID)
                && (c.Fecha >= dDesde && c.Fecha < dHastaMas1)
                && (iVendedorID == 0 || c.VendedorID == iVendedorID)
                && (iLineaID == 0 || c.LineaID == iLineaID)
                && (iMarcaID == 0 || c.MarcaID == iMarcaID)
                && (sDescripcion == "" || c.Descripcion.Contains(sDescripcion))
            );

            // Se llena el grid
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oLista)
            {
                this.dgvDatos.Rows.Add(oReg.VentaCotizacionID, oReg.Fecha, oReg.Sucursal, oReg.Vendedor, oReg.Cliente, oReg.Cantidad
                    , oReg.NumeroDeParte, oReg.Descripcion, oReg.Linea, oReg.Marca, oReg.Importe);
            }
            
            Cargando.Cerrar();
        }

        #endregion

        #region [ Públicos ]

        public void CambiarCliente(ClientesDatosView oCliente)
        {
            this.oCliente = oCliente;
            this.AplicarFiltro();
        }

        #endregion
    }
}
