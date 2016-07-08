using System;
using System.Windows.Forms;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public class VentasReimpresion : VentaOpcion
    {
        public BusquedaReimpresion ctlBusqueda;
        public cVentaDetalle ctlDetalle;

        public VentasReimpresion(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
            this.bExpandirContenido = true;
        }

        #region [ Base ]

        public override void Inicializar()
        {
            base.Inicializar();

            // Se crean los objetos necesarios
            this.ctlDetalle = new cVentaDetalle();
            this.ctlBusqueda = new BusquedaReimpresion();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlBusqueda.oReimpresion = this;
            
            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnTotales.Controls.Add(this.ctlDetalle);
            this.pnlEnContenido.Controls.Add(this.ctlBusqueda);

            // Se hace el cambio del cliente, para que se ejecuten los filtros, excepto cuando es "Ventas Mostrador"
            if (this.Cliente != null && this.Cliente.ClienteID != Cat.Clientes.Mostrador)
                this.ctlBusqueda.CambiarCliente(this.Cliente);
        }

        public override bool Ejecutar()
        {
            // Se verifica si es factura
            int iVentaID = this.ctlBusqueda.VentaID;
            var oVenta = Datos.GetEntity<VentasView>(q => q.VentaID == iVentaID);
            if (oVenta.Facturada)
            {
                bool bImpresa = VentasLoc.ReimprimirFactura(oVenta.Folio);
                if (!bImpresa)
                    return false;
            }
            else
            {
                // Se manda a re-imprimir la venta seleccionada
                var oAdicionales = new Dictionary<string, object>();
                oAdicionales.Add("Cambio", 0);
                VentasLoc.GenerarTicketDeVenta(oVenta.VentaID, null, oAdicionales);
            }

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Procedimiento de guardado ejecutado..");

            // Se limpia después de haberse guardado
            this.Limpiar();

            return true;
        }

        protected override void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;

            this.Cliente = this.oVenta.Cliente;
            this.ctlBusqueda.CambiarCliente(this.Cliente);
        }

        #endregion
    }
}
