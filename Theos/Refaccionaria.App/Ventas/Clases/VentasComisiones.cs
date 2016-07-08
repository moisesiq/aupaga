using System;
using System.Windows.Forms;

using TheosProc;

namespace Refaccionaria.App
{
    public class VentasComisiones : VentaOpcion
    {
        public Comisiones ctlComisiones;
        public cVentaDetalle ctlDetalle;
        public Usuario UsuarioAcceso;

        public VentasComisiones(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
            this.bExpandirContenido = true;
        }

        #region [ Base ]

        public override bool Activar()
        {
            // Si ya está activo, se oculta, si no, se muestra
            if (this.Activo)
            {
                this.pnlEnTotales.SendToBack();
                this.pnlEnContenido.SendToBack();
                this.pnlEnEquivalentes.SendToBack();

                // Se expande el área de contenido, para tomar la parte de equivalentes, cuando aplique
                this.ExpandirContenido(false);
            }
            else
            {
                // Se pide la contraseña del usuario
                /* var ResUsuario = UtilLocal.ValidarObtenerUsuario("Ventas.Comisiones.Ver");
                if (ResUsuario.Error)
                    return false;
                this.UsuarioAcceso = ResUsuario.Respuesta;
                */

                // Si no está creado, se crea
                if (!this.Inicializado)
                {
                    this.Inicializar();
                }

                // Se expande el área de contenido, para tomar la parte de equivalentes, cuando aplique
                this.ExpandirContenido(this.bExpandirContenido);

                // Se muestra
                this.pnlEnTotales.BringToFront();
                this.pnlEnContenido.BringToFront();
                this.pnlEnEquivalentes.BringToFront();

                // Se llenan los datos
                this.ctlComisiones.ActivarMetas(false);

                // Se restaura el cliente, según corresponda
                if (this.oVenta.Cliente.ClienteID != this.Cliente.ClienteID)
                    this.oVenta.Cliente = this.Cliente;
            }

            return this.Activo;
        }

        public override void Inicializar()
        {
            base.Inicializar();

            // Se crean los objetos necesarios
            this.ctlDetalle = new cVentaDetalle();
            this.ctlComisiones = new Comisiones();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlComisiones.oComisiones = this;
            //this.ctlBusqueda.CambiarCliente(this.Cliente);

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnTotales.Controls.Add(this.ctlDetalle);
            this.pnlEnContenido.Controls.Add(this.ctlComisiones);
        }

        public override bool Ejecutar()
        {
            // Se limpia después de haberse guardado
            this.Limpiar();

            return true;
        }

        #endregion
    }
}
