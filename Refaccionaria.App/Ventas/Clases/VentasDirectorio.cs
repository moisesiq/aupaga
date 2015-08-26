using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FastReport;
using System.Text;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    class VentasDirectorio : VentaOpcion
    {
        public DirectorioVentas ctlDirectorio;
        public DirectorioVentasDetalle ctlDirDetalle;
        public DirectorioVentasBusca ctlDirBuscador;

        public VentasDirectorio(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
            this.bExpandirContenido = false;  // true: Panel contenido también ocupa el panel detalle
        }

        #region [ Base ]

        public override void Inicializar()
        {
            base.Inicializar();

            //!!! recuperar lineas
            this.Busqueda.Controls.Add(this.pnlEnBusqueda = new Panel() { Dock = DockStyle.Fill });
            this.pnlEnBusqueda.ControlAdded += new ControlEventHandler((s, e) => { e.Control.Dock = DockStyle.Fill; });

            // Se crean los objetos necesarios
            this.ctlDirDetalle = new DirectorioVentasDetalle();
            this.ctlDirectorio = new DirectorioVentas(this.ctlDirDetalle);
            this.ctlDirBuscador = new DirectorioVentasBusca(this.ctlDirectorio);

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnContenido.Controls.Add(this.ctlDirectorio);
            this.pnlEnEquivalentes.Controls.Add(this.ctlDirDetalle);
            this.pnlEnBusqueda.Controls.Add(this.ctlDirBuscador);
        }


        //!!! recuperar función original -> 
        public override bool Activar()
        {
            // Si ya está activo, se oculta, si no, se muestra
            if (this.Activo)
            {
                this.pnlEnBusqueda.SendToBack();
                this.pnlEnTotales.SendToBack();
                this.pnlEnContenido.SendToBack();
                this.pnlEnEquivalentes.SendToBack();

                // Se expande el área de contenido, para tomar la parte de equivalentes, cuando aplique
                this.ExpandirContenido(false);
            }
            else
            {
                // Si no está creado, se crea
                if (!this.Inicializado)
                {
                    this.Inicializar();
                }

                // Se expande el área de contenido, para tomar la parte de equivalentes, cuando aplique
                this.ExpandirContenido(this.bExpandirContenido);

                // Se muestra
                this.pnlEnBusqueda.BringToFront();
                this.pnlEnTotales.BringToFront();
                this.pnlEnContenido.BringToFront();
                this.pnlEnEquivalentes.BringToFront();

                // Se restaura el cliente, según corresponda
                if (this.Cliente == null || this.oVenta.Cliente.ClienteID != this.Cliente.ClienteID)
                    this.oVenta.Cliente = this.Cliente;

                // Se establece el texto de estado, si hay para la opción especificada
                if (this.AccesosDeTeclado != null)
                    this.oVenta.EstablecerTextoEstado(this.AccesosDeTeclado);
            }

            return this.Activo;
        }

        public override void Limpiar()
        {
            //!!! devolver lineas
            this.Busqueda.Controls.Remove(this.pnlEnBusqueda);
            this.pnlEnBusqueda.Dispose();
            this.pnlEnBusqueda = null; 
            
            base.Limpiar();
        }

        #endregion
    }
}