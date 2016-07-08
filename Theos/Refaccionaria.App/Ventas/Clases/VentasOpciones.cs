using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    class VentasOpciones
    {
        public enum eOpcion { Venta, c9500, Reimpresion, Devolucion, Caja, FacturarVenta, Cobranza, Comisiones, Garantia, Directorio, Cotizaciones }
        public eOpcion Opcion = eOpcion.Venta;

        Ventas oVenta;
        ClientesDatosView ClienteVentas;
        public Ventas9500 o9500;
        public VentasReimpresion oReimpresion;
        public VentasDevolucion oDevolucion;
        public VentasCaja oCaja;
        public VentasFacturar oFacturar;
        public VentasCobranza oCobranza;
        public VentasComisiones oComisiones;
        public VentasGarantia oGarantia;
        public VentasDirectorio oDirectorio;
        public VentasCotizaciones oCotizaciones;

        public VentasOpciones(Ventas oVenta)
        {
            this.oVenta = oVenta;
        }

        public void Activar(VentasOpciones.eOpcion Operacion)
        {
            //Envía al frente 
            //!!! recuperar linea
            this.oVenta.pnlGenBuscador.Controls["pnlBuscador"].BringToFront();

            // Se guarda el cliente de Ventas
            if (this.Opcion == eOpcion.Venta)
                this.ClienteVentas = this.oVenta.Cliente;

            // Se activa la opción correspondiente
            bool bActivado = this.ObtenerOpcion(Operacion).Activar();

            // Se establece la opción activa
            if (bActivado)
                this.Opcion = Operacion;
            else
                this.RegresarAVenta();  // Se regresa a la pantalla de venta
        }

        public void Limpiar()
        {
            this.ObtenerOpcion(this.Opcion).Limpiar();
            // Se regresa a la pantalla de venta
            this.RegresarAVenta();
        }

        public void Ejecutar()
        {
            if (this.ObtenerOpcion(this.Opcion).Ejecutar())
                this.RegresarAVenta();
        }

        private void RegresarAVenta()
        {
            // Se muestra la venta normal al frente
            //!!! recuperar linea
            this.oVenta.pnlGenBuscador.Controls["pnlBuscador"].BringToFront();
            this.oVenta.pnlContenidoDetalle.Controls["pnlDetalle"].BringToFront();
            this.oVenta.pnlContenido.Controls["pnlPartes"].BringToFront();
            this.oVenta.pnlContenidoEquivalentes.Controls["pnlEquivalentes"].BringToFront();
                
            this.oVenta.Cliente = this.ClienteVentas;
            this.oVenta.EstablecerTextoEstado(Ventas.AccesosDeTeclado);
            this.oVenta.ActiveControl = this.oVenta.Controls["pnlGenBuscador"].Controls["txtCodigo"];
            this.Opcion = eOpcion.Venta;
        }

        private VentaOpcion ObtenerOpcion(eOpcion Operacion)
        {
            switch (Operacion)
            {
                case eOpcion.c9500:
                    if (this.o9500 == null)
                        this.o9500 = new Ventas9500(this.oVenta);
                    return this.o9500;
                case eOpcion.Reimpresion:
                    if (this.oReimpresion == null)
                        this.oReimpresion = new VentasReimpresion(this.oVenta);
                    return this.oReimpresion;
                case eOpcion.Devolucion:
                    if (this.oDevolucion == null)
                        this.oDevolucion = new VentasDevolucion(this.oVenta);
                    return this.oDevolucion;
                case eOpcion.Caja:
                    if (this.oCaja == null)
                        this.oCaja = new VentasCaja(this.oVenta);
                    return this.oCaja;
                case eOpcion.FacturarVenta:
                    if (this.oFacturar == null)
                        this.oFacturar = new VentasFacturar(this.oVenta);
                    return this.oFacturar;
                case eOpcion.Cobranza:
                    if (this.oCobranza == null)
                        this.oCobranza = new VentasCobranza(this.oVenta);
                    return this.oCobranza;
                case eOpcion.Comisiones:
                    if (this.oComisiones == null)
                        this.oComisiones = new VentasComisiones(this.oVenta);
                    return this.oComisiones;
                case eOpcion.Garantia:
                    if (this.oGarantia == null)
                        this.oGarantia = new VentasGarantia(this.oVenta);
                    return this.oGarantia;
                case eOpcion.Directorio:
                    if (this.oDirectorio == null)
                        this.oDirectorio = new VentasDirectorio(this.oVenta);
                    return this.oDirectorio;
                case eOpcion.Cotizaciones:
                    if (this.oCotizaciones == null)
                        this.oCotizaciones = new VentasCotizaciones(this.oVenta);
                    return this.oCotizaciones;
            }

            return null;
        }
    }

    public class VentaOpcion
    {
        const int SeparacionContenidoYEquivalentes = 6;

        protected Ventas oVenta;
        public Panel pnlEnTotales, pnlEnContenido, pnlEnEquivalentes, pnlEnBusqueda;
        protected Panel Totales, Contenido, Equivalentes, Busqueda;
        protected ClientesDatosView Cliente;

        protected bool bExpandirContenido;
        protected virtual string AccesosDeTeclado { get { return null; } }

        public void CargarReferencias(Ventas oVenta)
        {
            this.oVenta = oVenta;
            //!!! recuperar linea
            this.Busqueda = this.oVenta.pnlGenBuscador;
            this.Totales = this.oVenta.pnlContenidoDetalle;
            this.Contenido = this.oVenta.pnlContenido;
            this.Equivalentes = this.oVenta.pnlContenidoEquivalentes;
        }

        public virtual bool Activo
        {
            get
            {
                if (this.pnlEnContenido == null) return false;
                return (this.Contenido.Controls.GetChildIndex(this.pnlEnContenido) == 0);
            }
        }

        public virtual bool Inicializado
        {
            get { return (this.pnlEnContenido != null); }
        }

        public virtual bool Activar()
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

                // Se restaura el cliente, según corresponda
                if (this.Cliente == null || this.oVenta.Cliente.ClienteID != this.Cliente.ClienteID)
                    this.oVenta.Cliente = this.Cliente;

                // Se establece el texto de estado, si hay para la opción especificada
                if (this.AccesosDeTeclado != null)
                    this.oVenta.EstablecerTextoEstado(this.AccesosDeTeclado);
            }

            return this.Activo;
        }

        public virtual void Inicializar()
        {
            // Se inicializa la parte del Cliente
            this.oVenta.ClienteCambiado += new Action(oVenta_ClienteCambiado);
            this.Cliente = this.oVenta.Cliente;

            // Se crean los páneles a usar
            this.Totales.Controls.Add(this.pnlEnTotales = new Panel() { Dock = DockStyle.Fill });
            this.Contenido.Controls.Add(this.pnlEnContenido = new Panel() { Dock = DockStyle.Fill });
            this.Equivalentes.Controls.Add(this.pnlEnEquivalentes = new Panel() { Dock = DockStyle.Fill });

            this.pnlEnTotales.ControlAdded += new ControlEventHandler((s, e) => { e.Control.Dock = DockStyle.Fill; });
            this.pnlEnContenido.ControlAdded += new ControlEventHandler((s, e) => { e.Control.Dock = DockStyle.Fill; });
            this.pnlEnEquivalentes.ControlAdded += new ControlEventHandler((s, e) => { e.Control.Dock = DockStyle.Fill; });
        }
        
        public virtual void Limpiar()
        {
            this.Totales.Controls.Remove(this.pnlEnTotales);
            this.pnlEnTotales.Dispose();
            this.pnlEnTotales = null;

            this.Contenido.Controls.Remove(this.pnlEnContenido);
            this.pnlEnContenido.Dispose();
            this.pnlEnContenido = null;

            this.Equivalentes.Controls.Remove(this.pnlEnEquivalentes);
            this.pnlEnEquivalentes.Dispose();
            this.pnlEnEquivalentes = null;

            this.ExpandirContenido(false);

            this.oVenta.ClienteCambiado -= this.oVenta_ClienteCambiado;
            this.Cliente = null;
        }

        public virtual bool Ejecutar() { return true; }

        protected virtual void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;
            this.Cliente = this.oVenta.Cliente;
        }

        public virtual void VentaCambiarCliente(ClientesDatosView oCliente)
        {
            if (this.oVenta.Cliente != oCliente)
                this.oVenta.Cliente = oCliente;
        }

        #region [ Métodos ]

        protected void ExpandirContenido(bool bExpandir)
        {
            // Se determina si el contenido está expandido o no
            bool bExpandido = (this.Contenido.Bottom > this.Equivalentes.Top);
            //

            if (bExpandir)
            {
                if (!bExpandido)
                    this.Contenido.Height += (VentaOpcion.SeparacionContenidoYEquivalentes + this.Equivalentes.Height);
            }
            else
            {
                if (bExpandido)
                    this.Contenido.Height -= (VentaOpcion.SeparacionContenidoYEquivalentes + this.Equivalentes.Height);
            }
        }

        #endregion
    }
}
