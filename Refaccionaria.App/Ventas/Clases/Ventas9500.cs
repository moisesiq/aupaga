using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FastReport;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public class Ventas9500 : VentaOpcion
    {
        public enum eOpcion { Agregar, Completar }

        public Cobro ctlCobro;
        public c9500Partes ctlPartes;
        public Panel pnlCompletar;
        public Cobro ctlComCobro;
        public Detalle9500Com ctlComDetalle;
        public eOpcion Opcion = eOpcion.Agregar;

        public Ventas9500(Ventas oVenta)
        {
            base.CargarReferencias(oVenta);
        }

        #region [ Base ]

        public override void Inicializar()
        {
            base.Inicializar();

            // Se crean los objetos necesarios
            this.ctlCobro = new Cobro();
            this.ctlPartes = new c9500Partes();

            // Se configuran las propiedades necesarias de los objetos
            this.ctlCobro.HabilitarTipoDePago = false;
            this.ctlCobro.MostrarFacturar = false;
            this.ctlCobro.HabilitarRetroceso = false;
            this.ctlCobro.HabilitarFormasDePago = false;
            this.ctlCobro.CambiarCliente(this.Cliente.ClienteID);
            this.ctlPartes.o9500 = this;
            this.ctlPartes.CambiarCliente(this.Cliente);

            // Se agregan a los paneles (esto desencadena el evento "Load" de cada control)
            this.pnlEnTotales.Controls.Add(this.ctlCobro);
            this.pnlEnContenido.Controls.Add(this.ctlPartes);
        }

        public override void Limpiar()
        {
            if (this.pnlCompletar != null)
            {
                this.ctlPartes.ComCliente = null;
                this.pnlEnTotales.Controls.Remove(this.pnlCompletar);
                this.pnlCompletar.Dispose();
                this.pnlCompletar = null;
            }

            this.Opcion = eOpcion.Agregar;

            base.Limpiar();
        }

        public override bool Ejecutar()
        {
            // Se verifica si ya se hizo el cierre de caja
            if (UtilDatos.VerCierreDeDaja())
            {
                UtilLocal.MensajeAdvertencia("Ya se hizo el Corte de Caja. No se puede continuar.");
                return false;
            }

            bool bExito;
            if (this.Opcion == eOpcion.Agregar)
                bExito = this.Agregar9500();
            else
                bExito = this.Completar9500();

            if (!bExito)
                return false;

            // Se limpia después de haberse guardado
            this.Limpiar();
            
            return true;
        }

        protected override void oVenta_ClienteCambiado()
        {
            if (!this.Activo) return;

            this.Cliente = this.oVenta.Cliente;
            this.ctlPartes.CambiarCliente(this.Cliente);
            this.ctlCobro.CambiarCliente(this.Cliente == null ? 0 : this.Cliente.ClienteID);
        }

        #endregion

        #region [ Públicos ]

        public void CambiarOpcion(eOpcion Opcion)
        {
            switch (Opcion)
            {
                case eOpcion.Agregar:
                    if (this.pnlCompletar != null)
                        this.pnlCompletar.SendToBack();
                    this.oVenta.Cliente = this.ctlPartes.Cliente;
                    break;
                case eOpcion.Completar:
                    // Verifica si se ha inicializado, si no, lo hace
                    if (this.pnlCompletar == null)
                    {
                        this.pnlCompletar = new Panel() { Dock = DockStyle.Fill };
                        this.pnlCompletar.ControlAdded += new ControlEventHandler((s, e) => { e.Control.Dock = DockStyle.Fill; });
                        this.ctlComDetalle = new Detalle9500Com();
                        this.pnlCompletar.Controls.Add(this.ctlComDetalle);
                        this.pnlEnTotales.Controls.Add(this.pnlCompletar);
                    }

                    // Se verifica si se debe cambiar el cliente
                    if (this.ctlPartes.ComCliente != null)
                        this.oVenta.Cliente = this.ctlPartes.ComCliente;

                    this.pnlCompletar.BringToFront();
                    break;
            }

            this.Opcion = Opcion;
        }

        public void ClienteCompletar(ClientesDatosView oCliente)
        {
            this.oVenta.Cliente = oCliente;
        }

        #endregion

        #region [ Uso interno ]

        private bool Agregar9500()
        {
            // Se valida la parte de "Partes"
            if (!this.ctlPartes.Validar())
                return false;
            // Se pide el efectivo, si aplica
            if (!this.ctlCobro.CompletarCobro())
                return false;
            // Se valida que exista una Medida genérica, para las nuevas partes
            if (General.GetEntity<Medida>(q => q.MedidaID == Cat.Medidas.Pieza && q.Estatus) == null)
            {
                UtilLocal.MensajeAdvertencia("No existe una Medida genérica para asignarle a las partes nuevas. No se puede continuar.");
                return false;
            }

            // Se solicitan la autorizaciones, si se requiere
            int iAutorizoID = 0;
            if (this.ctlPartes.AutorizacionRequeridaPrecio || this.ctlPartes.AutorizacionRequeridaAnticipo)
            {
                string sPermiso = (this.ctlPartes.AutorizacionRequeridaPrecio ? "Autorizaciones.Ventas.9500.PrecioFueraDeRango" :
                    "Autorizaciones.Ventas.9500.NoAnticipo");
                var Res = UtilDatos.ValidarObtenerUsuario(sPermiso, "Autorización");
                iAutorizoID = (Res.Respuesta == null ? 0 : Res.Respuesta.UsuarioID);
            }

            // Se procede a guardar los datos
            DateTime dAhora = DateTime.Now;
            // Se genera la Cotización 9500
            var o9500 = this.ctlPartes.Generar9500();
            o9500.Fecha = dAhora;
            o9500.RealizoUsuarioID = this.ctlCobro.VendodorID;
            if (this.ctlCobro.ComisionistaID > 0)
                o9500.ComisionistaClienteID = this.ctlCobro.ComisionistaID;
            // Se genera el detalle del 9500
            var oParteGanancia = this.ctlPartes.ObtenerParteGanancia(null);
            var o9500Detalle = new List<Cotizacion9500Detalle>();
            foreach (var Parte9500 in this.ctlPartes.Detalle)
            {
                // Si la parte no existe, se agrega
                if (Parte9500.Value.ParteID <= 0)
                {
                    int iFila = Helper.findRowIndex(this.ctlPartes.dgvPartes, "Llave", Parte9500.Key);
                    string sNumeroDeParte = Helper.ConvertirCadena(this.ctlPartes.dgvPartes["NumeroDeParte", iFila].Value);
                    string sDescripcion = Helper.ConvertirCadena(this.ctlPartes.dgvPartes["Descripcion", iFila].Value);
                    var oLinea = General.GetEntity<Linea>(q => q.LineaID == Parte9500.Value.LineaID && q.Estatus);
                    Parte oParte = new Parte()
                    {
                        NumeroParte = sNumeroDeParte,
                        LineaID = Parte9500.Value.LineaID,
                        MarcaParteID = Parte9500.Value.MarcaParteID,
                        ProveedorID = Parte9500.Value.ProveedorID,
                        NombreParte = sDescripcion,
                        Es9500 = true,
                        SubsistemaID = oLinea.SubsistemaID.Valor()
                    };
                    
                    // Se agregan los precios
                    PartePrecio oPartePrecio = null;
                    if (oParteGanancia != null)
                    {
                        oPartePrecio = new PartePrecio()
                        {
                            Costo = Parte9500.Value.Costo,
                            PorcentajeUtilidadUno = oParteGanancia.PorcentajeDeGanancia1,
                            PorcentajeUtilidadDos = oParteGanancia.PorcentajeDeGanancia2,
                            PorcentajeUtilidadTres = oParteGanancia.PorcentajeDeGanancia3,
                            PorcentajeUtilidadCuatro = oParteGanancia.PorcentajeDeGanancia4,
                            PorcentajeUtilidadCinco = oParteGanancia.PorcentajeDeGanancia5,
                            PrecioUno = Helper.AplicarRedondeo(Parte9500.Value.Costo * oParteGanancia.PorcentajeDeGanancia1),
                            PrecioDos = Helper.AplicarRedondeo(Parte9500.Value.Costo * oParteGanancia.PorcentajeDeGanancia2),
                            PrecioTres = Helper.AplicarRedondeo(Parte9500.Value.Costo * oParteGanancia.PorcentajeDeGanancia3),
                            PrecioCuatro = Helper.AplicarRedondeo(Parte9500.Value.Costo * oParteGanancia.PorcentajeDeGanancia4),
                            PrecioCinco = Helper.AplicarRedondeo(Parte9500.Value.Costo * oParteGanancia.PorcentajeDeGanancia5)
                        };
                    }

                    // Se guarda
                    Guardar.Parte(oParte, oPartePrecio);
                    Parte9500.Value.ParteID = oParte.ParteID;
                }

                // Se agrega la parte al detalle del 9500
                o9500Detalle.Add(Parte9500.Value);
            }

            // Se guardan los datos de 9500
            App.Guardar.c9500(o9500, o9500Detalle);
            
            // Se genera la venta con el anticipo
            var oVenta = new Venta()
            {
                ClienteID = o9500.ClienteID,
                RealizoUsuarioID = o9500.RealizoUsuarioID
            };
            var oPrecioAnticipo = General.GetEntity<PartePrecio>(c => c.ParteID == Cat.Partes.AnticipoClientes && c.Estatus);
            var oVentaDetalle = new VentaDetalle()
            {
                ParteID = Cat.Partes.AnticipoClientes,
                Cantidad = 1,
                PrecioUnitario = o9500.Anticipo,
                Costo = oPrecioAnticipo.Costo.Valor(),
                CostoConDescuento = (oPrecioAnticipo.CostoConDescuento ?? oPrecioAnticipo.Costo.Valor())
            };
            App.Guardar.Venta(oVenta, new List<VentaDetalle>() { oVentaDetalle });

            // Se guarda el dato de la venta con el anticipo en el registro de 9500
            o9500.AnticipoVentaID = oVenta.VentaID;
            App.Guardar.Generico<Cotizacion9500>(o9500);

            // Se guardan las autorizaciones, si hubiera
            if (this.ctlPartes.AutorizacionRequeridaPrecio)
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.c9500PrecioFueraDeRango, Cat.Tablas.Tabla9500, o9500.Cotizacion9500ID, iAutorizoID);
            if (this.ctlPartes.AutorizacionRequeridaAnticipo)
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.c9500SinAnticipo, Cat.Tablas.Tabla9500, o9500.Cotizacion9500ID, iAutorizoID);

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Cotización 9500 guardada correctamente.");

            return true;
        }

        private bool Completar9500()
        {
            // Se validan las partes
            if (!this.ctlComDetalle.Validar())
                return false;
                        
            //if (Helper.ControlAlFrente(this.pnlCompletar) == this.ctlComDetalle)
            //{                  
            //}

            // Se verifica que se haya hecho el pago del anticipo
            Cotizacion9500 o9500 = this.ctlPartes.oCotizacion9500;
            if (!General.Exists<Venta>(c => c.VentaID == o9500.AnticipoVentaID 
                && (c.VentaEstatusID == Cat.VentasEstatus.Completada || c.VentaEstatusID == Cat.VentasEstatus.Cobrada)))
            {
                UtilLocal.MensajeAdvertencia("Al parecer no se ha realizado el pago correspondiente al Anticipo. No se puede continuar.");
                return false;
            }

            // Se confirma la operación
            if (UtilLocal.MensajePregunta(string.Format("¿Estás seguro que deseas completar el 9500 con el folio {0}?\n\n{1}"
                , this.ctlPartes.oCotizacion9500.Cotizacion9500ID, this.ctlPartes.o9500Sel["lisDescripcion"])) != DialogResult.Yes)
                return false;

            // Se guardan los datos
            DateTime dAhora = DateTime.Now;
                        
            // Se cancela la venta del anticipo
            /* Ya no. Ahora todo esto se hace al cobrar la venta final
            oVenta.VentaEstatusID = Cat.VentasEstatus.Cancelada;
            Guardar.Generico<Venta>(oVenta);
            // Se genera una devolución de efectivo (si se realizó un pago) de la venta cancelada, pues se generará una nueva venta con el importe total
            if (oVentaPago != null)
                VentasProc.GenerarDevolucionDeEfectivo(o9500.AnticipoVentaID.Valor(), o9500.Anticipo);
            */

            // Se genera la venta correspondiente al 9500
            // var o9500Detalle = General.GetListOf<Cotizacion9500Detalle>(q => q.Estatus && q.Cotizacion9500ID == oCotizacion9500.Cotizacion9500ID);
            var oCliente = General.GetEntity<Cliente>(q => q.ClienteID == o9500.ClienteID && q.Estatus);
            var oDetalle = this.ctlComDetalle.ProductosSel();
            var oVenta = new Venta()
            {
                Fecha = dAhora,
                ClienteID = o9500.ClienteID,
                VentaEstatusID = Cat.VentasEstatus.Realizada,
                RealizoUsuarioID = o9500.RealizoUsuarioID,
                ComisionistaClienteID = o9500.ComisionistaClienteID
            };
            var oVentaDetalle = new List<VentaDetalle>();
            foreach (var oParte in oDetalle)
            {
                // Se toma el precio de la tabla "PartePrecio", pues pudo haber sido cambiado por el encargado de Compras
                var oPartePrecio = General.GetEntity<PartePrecio>(q => q.ParteID == oParte.ParteID);
                decimal mPrecio = UtilDatos.PartePrecioDeVenta(oPartePrecio, oCliente.ListaDePrecios);
                // Se agrega la parte al detalle de la venta
                oVentaDetalle.Add(new VentaDetalle()
                {
                    ParteID = oParte.ParteID,
                    Costo = oPartePrecio.Costo.Valor(),
                    CostoConDescuento = (oPartePrecio.CostoConDescuento ?? oPartePrecio.Costo.Valor()),
                    Cantidad = oParte.Cantidad,
                    PrecioUnitario = UtilLocal.ObtenerPrecioSinIva(mPrecio, 3),
                    Iva = UtilLocal.ObtenerIvaDePrecio(mPrecio, 3)
                });
            }
            // Se guarda la venta
            Guardar.Venta(oVenta, oVentaDetalle);

            // Se modifica el dato de la venta correspondiente al 9500
            o9500.VentaID = oVenta.VentaID;
            o9500.EstatusGenericoID = Cat.EstatusGenericos.PorCompletar;
            Guardar.Generico<Cotizacion9500>(o9500);

            // Se restaura
            this.ctlPartes.ComCliente = null;
            this.pnlEnTotales.Controls.Remove(this.pnlCompletar);
            this.pnlCompletar.Dispose();
            this.pnlCompletar = null;
            this.CambiarOpcion(eOpcion.Agregar);
            this.ctlPartes.tab9500.SelectedIndex = 0;

            // Se muestra una notifiación con el resultado
            UtilLocal.MostrarNotificacion("Cotización 9500 guardada correctamente.");

            // Se retorna falso para que no se quite la opción de 9500
            return false;
        }

        #endregion
    }
}
