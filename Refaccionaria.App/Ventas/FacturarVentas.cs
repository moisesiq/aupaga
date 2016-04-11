using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class FacturarVentas : UserControl
    {
        const string MostrarTodas = "TODOS";

        ControlError ctlError = new ControlError();

        public VentasFacturar oManejador;
        protected ClientesDatosView Cliente;

        public int VentaID { get; set; }

        public FacturarVentas()
        {
            InitializeComponent();
            this.cmbSucursal.TextChanged += new EventHandler(cmbSucursal_TextChanged);
        }

        #region [ Propiedades ]

        public string Observacion { get { return this.txtObservacion.Text; } }

        public bool MostrarTodasLasPartes { get { return (this.dgvAFacturar.CurrentRow.Index == 0); } }

        #endregion

        #region [ Eventos ]

        private void BusquedaVenta_Load(object sender, System.EventArgs e)
        {
            // Se cargan los datos de las Sucursales
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(q => q.Estatus));
            this.cmbSucursal.SelectedValue = GlobalClass.SucursalID;
            // Se especifican las fechas predeterminadas
            this.dtpInicio.Value = DateTime.Now.DiaPrimero();
            this.dtpFin.Value = DateTime.Now.DiaUltimo();

            // Se aplica el filtro al cargar el control
            //this.AplicarFiltro();

            // Se agrega el concepto de "TODOS" en el grid de las ventas a facturar
            this.dgvAFacturar.Rows.Add(0, FacturarVentas.MostrarTodas);
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused)
                this.AplicarFiltro();
        }

        private void cmbSucursal_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused && this.cmbSucursal.Text == "")
                this.AplicarFiltro();
        }

        private void dtpInicio_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpInicio.Focused)
                this.AplicarFiltro();
        }

        private void dtpFin_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpFin.Focused)
                this.AplicarFiltro();
        }

        private void txtFolio_TextChanged(object sender, EventArgs e)
        {
            this.AplicarFiltro();
        }

        private void dgvVentas_CurrentCellChanged(object sender, EventArgs e)
        {
            // Se actualizan los datos
            this.VentaID = (this.dgvVentas.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvVentas.CurrentRow.Cells["vVentaID"].Value));
            this.MostrarDatosDeVenta(this.VentaID);
        }
                
        private void dgvVentas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvVentas.CurrentRow == null) return;

            // Se pasa la venta al grid de AFacturar
            this.PasarVentaAFacturar(this.dgvVentas.CurrentRow);
        }

        private void btnSeleccionarTodas_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow Fila in this.dgvVentas.Rows)
                this.PasarVentaAFacturar(Fila);
        }

        private void dgvAFacturar_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvAFacturar.CurrentRow == null)
            {
                this.MostrarDatosDeVenta(0);
                return;
            }

            if (Helper.ConvertirCadena(this.dgvAFacturar.CurrentRow.Cells["afFecha"].Value) == FacturarVentas.MostrarTodas)
            {
                var Ventas = new List<int>();
                foreach (DataGridViewRow Fila in this.dgvAFacturar.Rows)
                    Ventas.Add(Helper.ConvertirEntero(Fila.Cells["afVentaID"].Value));
                this.MostrarDatosDeVentasVarias(Ventas);
            }
            else
            {
                int iVentaID = Helper.ConvertirEntero(this.dgvAFacturar.CurrentRow.Cells["afVentaID"].Value);
                this.MostrarDatosDeVenta(iVentaID);
            }
        }
                
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (this.dgvAFacturar.CurrentRow == null) return;
            if (Helper.ConvertirCadena(this.dgvAFacturar.CurrentRow.Cells["afFecha"].Value) == FacturarVentas.MostrarTodas) return;
            this.dgvAFacturar.Rows.Remove(this.dgvAFacturar.CurrentRow);
        }
        
        #endregion

        #region [ Métodos ]

        private void AplicarFiltro()
        {
            string sFolio = this.txtFolio.Text;
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            DateTime dInicio = this.dtpInicio.Value.Date;
            DateTime dFin = this.dtpFin.Value.Date.AddDays(1);
            
            List<VentasView> Lista;

            if (sFolio == "")
            {
                Lista = General.GetListOf<VentasView>(q =>
                    (q.VentaEstatusID == Cat.VentasEstatus.Completada || q.VentaEstatusID == Cat.VentasEstatus.Cobrada)
                    && !q.Facturada
                    && (iClienteID == 0 || q.ClienteID == iClienteID)
                    && (iSucursalID == 0 || q.SucursalID == iSucursalID)
                    && q.Fecha >= dInicio
                    && q.Fecha < dFin
                );
            }
            else
            {
                Lista = General.GetListOf<VentasView>(q => 
                    q.Folio == sFolio
                    && (q.VentaEstatusID == Cat.VentasEstatus.Completada || q.VentaEstatusID == Cat.VentasEstatus.Cobrada)
                    && (iSucursalID == 0 || q.SucursalID == iSucursalID)
                );
            }

            // Se manda llenar el Grid
            this.LlenarGrid(Lista);
        }

        private void LlenarGrid(List<VentasView> oVentas)
        {
            this.dgvVentas.Rows.Clear();
            foreach (var oVenta in oVentas)
                this.dgvVentas.Rows.Add(oVenta.VentaID, oVenta.Fecha, oVenta.Folio, oVenta.Total);
        }

        private void MostrarDatosDeVenta(int iVentaID)
        {
            // Para mostrar el detalle de la venta
            this.oManejador.ctlDetalle.LlenarDetalle(iVentaID);
        }

        private void MostrarDatosDeVentasVarias(List<int> Ventas)
        {
            // Se limpia el grid de detalle
            this.oManejador.ctlDetalle.LimpiarDetalle();

            // Se empieza a llenar con los datos de las ventas
            List<VentasDetalleView> oDetalle;
            foreach (int iVentaID in Ventas)
            {
                oDetalle = General.GetListOf<VentasDetalleView>(q => q.VentaID == iVentaID);
                // Se agrega el detalle
                foreach (var oProducto in oDetalle)
                {
                    this.oManejador.ctlDetalle.AgregarProducto(new ProductoVenta()
                    {
                        ParteID = oProducto.ParteID,
                        NumeroDeParte = oProducto.NumeroParte,
                        NombreDeParte = oProducto.NombreParte,
                        Cantidad = oProducto.Cantidad,
                        PrecioUnitario = oProducto.PrecioUnitario,
                        Iva = oProducto.Iva,
                        // PrecioConIva = (oProducto.PrecioUnitario + oProducto.Iva)
                        UnidadDeMedida = oProducto.Medida
                    });
                }
            }
        }

        private void PasarVentaAFacturar(DataGridViewRow Fila)
        {
            if (this.dgvAFacturar.BuscarValor("afVentaID", Fila.Cells["vVentaID"].Value) < 0)
                this.dgvAFacturar.Rows.Add(Fila.ObtenerValores());
        }

        #endregion

        #region [ Públicos ]

        public void CambiarCliente(ClientesDatosView oCliente)
        {
            this.Cliente = oCliente;
            this.AplicarFiltro();
        }

        public List<int> GenerarListaDeVentas()
        {
            var Ventas = new List<int>();
            foreach (DataGridViewRow Fila in this.dgvAFacturar.Rows)
            {
                if (Helper.ConvertirCadena(Fila.Cells["afFecha"].Value) == FacturarVentas.MostrarTodas)
                    continue;
                Ventas.Add(Helper.ConvertirEntero(Fila.Cells["afVentaID"].Value));
            }
            return Ventas;
        }

        public bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (this.dgvAFacturar.Rows.Count <= 1)
                this.ctlError.PonerError(this.lblSeleccionados, "No hay ninguna venta seleccionada para facturar.");
            
            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
