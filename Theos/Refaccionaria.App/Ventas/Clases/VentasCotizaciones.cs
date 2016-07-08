using System;
using System.Windows.Forms;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public class VentasCotizaciones : VentaOpcion
    {
        public CotizacionesVentas ctlCotizaciones;
        public cVentaDetalle ctlDetalle;

        public VentasCotizaciones(Ventas oVenta)
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
            this.ctlCotizaciones = new CotizacionesVentas();

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnTotales.Controls.Add(this.ctlDetalle);
            this.pnlEnContenido.Controls.Add(this.ctlCotizaciones);

            // Se hace el cambio del cliente, para que se ejecuten los filtros, excepto cuando es "Ventas Mostrador"
            if (this.Cliente != null && this.Cliente.ClienteID != Cat.Clientes.Mostrador)
                this.ctlCotizaciones.CambiarCliente(this.Cliente);
        }

        public override bool Ejecutar()
        {
            // Se verifica si es 
            

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("No hay acción configurada para esta opción.");

            // Se limpia después de haberse guardado
            // this.Limpiar();

            return false;
        }

        protected override void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;

            this.Cliente = this.oVenta.Cliente;
            this.ctlCotizaciones.CambiarCliente(this.Cliente);
        }

        #endregion
    }
}
