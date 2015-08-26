using System;
using System.Windows.Forms;
using System.Data.Objects;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CajaVentasPorCobrar : UserControl
    {
        public VentasCaja oVentasCaja;

        public Panel pnlParaDetalle;
        public Cobro ctlCobro;
        public cVentaDetalle ctlDetalle;
                
        public CajaVentasPorCobrar()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public int VentaID
        {
            get
            {
                if (this.dgvDatos.CurrentRow == null) return 0;
                return Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["colVentaID"].Value);
            }
        }

        public int ClienteID
        {
            get
            {
                if (this.dgvDatos.CurrentRow == null) return 0;
                return Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["colClienteID"].Value);
            }
        }

        public decimal ImporteVenta
        {
            get
            {
                if (this.dgvDatos.CurrentRow == null) return 0;
                return Helper.ConvertirDecimal(this.dgvDatos.CurrentRow.Cells["colImporte"].Value);
            }
        }

        #endregion

        #region [ Eventos ]

        private void CajaVentasPorCobrar_Load(object sender, EventArgs e)
        {
            // Se carga la parte del cobro
            this.ctlDetalle = new cVentaDetalle() { Dock = DockStyle.Fill };
            this.ctlCobro = new Cobro() { Dock = DockStyle.Fill };
            this.oVentasCaja.pnlEnTotales.Controls.Add(this.pnlParaDetalle = new Panel());
            this.pnlParaDetalle.Controls.Add(this.ctlDetalle);
            this.pnlParaDetalle.Controls.Add(this.ctlCobro);
            
            // Se llenan los datos
            this.ActualizarDatos();
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            if (e.KeyCode == Keys.Delete)
            {
                this.BorrarVenta(this.VentaID);
            }
        }

        private void dgvDatos_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (!e.Row.Selected) return;

            int iVentaID = Helper.ConvertirEntero(e.Row.Cells["colVentaID"].Value);
            this.ctlDetalle.LlenarDetalle(iVentaID);
        }

        #endregion

        #region [ Métodos ]

        private void BorrarVenta(int iVentaID)
        {
            // Para ver si la venta es un anticipo de 9500
            var o9500Ant = General.GetEntity<Cotizacion9500>(q => q.AnticipoVentaID == iVentaID && q.EstatusGenericoID == Cat.EstatusGenericos.Pendiente
                && q.Estatus);
            // Para ver si la venta es de un 9500
            Cotizacion9500 o9500 = null;
            if (o9500Ant == null)
                o9500 = General.GetEntity<Cotizacion9500>(q => q.VentaID == iVentaID && q.Estatus);

            string sPregunta = "¿Estás seguro que deseas eliminar la venta seleccionada?";
            if (o9500Ant != null)
                sPregunta = "La venta seleccionada pertenece a un anticipo de 9500. ¿Estás seguro que deseas eliminarla?";
            else if (o9500 != null)
                sPregunta = "La venta seleccionada pertenece a un 9500. ¿Estás seguro que deseas eliminarla?";

            if (UtilLocal.MensajePregunta(sPregunta) == DialogResult.Yes)
            {
                // Si es un anticipo de 9500, éste se cancela y se borran las partes, si no han sido usadas
                if (o9500Ant != null)
                    VentasProc.Cancelar9500(o9500Ant.Cotizacion9500ID, "POR BORRAR VENTA SIN COBRAR", GlobalClass.UsuarioGlobal.UsuarioID);
                // Si es un 9500, éste se regresa a los 9500 pendientes
                else if (o9500 != null)
                    VentasProc.Regresar9500DeCompletar(o9500);

                // Se elimina la venta
                VentasProc.EliminarVenta(iVentaID);
                this.ActualizarDatos();
            }
        }

        #endregion

        #region [ Públicos ]

        public void ActualizarDatos()
        {
            // Se oculta la ventana de cobro, por si estuviera visible
            this.ctlCobro.SendToBack();
            // Se limpian los datos de detalle
            this.ctlDetalle.LimpiarDetalle();

            // Se limpian los datos del Grid
            this.dgvDatos.Rows.Clear();
            // Se cargan las ventas pendientes en el Grid
            DateTime dHoy = DateTime.Today;
            var oVentas = General.GetListOf<VentasView>(q => q.SucursalID == GlobalClass.SucursalID
                && q.VentaEstatusID == Cat.VentasEstatus.Realizada && EntityFunctions.TruncateTime(q.Fecha) == dHoy);
            foreach (var oVenta in oVentas)
                this.dgvDatos.Rows.Add(oVenta.VentaID, oVenta.ClienteID, oVenta.Facturada, oVenta.Folio, oVenta.Cliente, oVenta.Vendedor, oVenta.Total);

            // Se muestra ventana de recordatorio de cambio de turno, si aplica
            DateTime dHora = (DateTime.Now > dHoy.AddHours(18) ? dHoy.AddHours(18) : (DateTime.Now > dHoy.AddHours(12) ? dHoy.AddHours(12) : dHoy));
            if (dHora > dHoy && !General.Exists<CajaCambioDeTurno>(c => c.SucursalID == GlobalClass.SucursalID && c.Fecha >= dHora))
                UtilLocal.MensajeAdvertencia("Favor de realizar un Cambio de Turno para verificar caja.");
        }

        #endregion
    }
}
