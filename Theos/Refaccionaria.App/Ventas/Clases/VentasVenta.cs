using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FastReport;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    class VentasVenta
    {
        Ventas oControlVentas;

        public VentasVenta(Ventas oVenta)
        {
            this.oControlVentas = oVenta;
            this.oControlVentas.ClienteCambiado += new Action(oVenta_ClienteCambiado);
        }

        private void oVenta_ClienteCambiado()
        {

        }

        public bool Ejecutar()
        {
            // Se verifica si ya se hizo el cierre de caja
            if (UtilDatos.VerCierreDeDaja())
            {
                UtilLocal.MensajeAdvertencia("Ya se hizo el Corte de Caja. No se puede continuar.");
                return false;
            }

            // Se valida el cliente
            if (this.oControlVentas.Cliente == null)
            {
                UtilLocal.MensajeAdvertencia("No hay ningún cliente seleccionado o cliente inválido.");
                return false;
            }

            // Se validan los productos
            if (!this.oControlVentas.Validar())
                return false;

            // Se validan si hay una parte de cobro de diferencia de casco, sea la única en la venta
            var oVentaDet = this.oControlVentas.GenerarVentaDetalle();
            foreach (var oReg in oVentaDet)
            {
                if (Datos.Exists<Parte>(c => c.ParteID == oReg.ParteID && c.EsCascoPara > 0) || oReg.ParteID == Cat.Partes.DiferenciaDeCascos)
                {
                    if (oVentaDet.Count > 1)
                    {
                        UtilLocal.MensajeAdvertencia("La venta contiene un artículo Casco, por lo cual no puede contener otros artículos.");
                        return false;
                    }
                }
            }

            // Se verifica si se debe mostrar la opción de cobro
            if (!this.oControlVentas.CobroAlFrente)
            {
                if (this.oControlVentas.ctlCobro == null)
                {
                    this.oControlVentas.ctlCobro = new Cobro() { Dock = DockStyle.Fill };
                    this.oControlVentas.pnlContenidoDetalle.Controls.Add(this.oControlVentas.ctlCobro);

                    // Se configura el evento Click para el botón de cotización
                    this.oControlVentas.ctlCobro.Cotizacion_Click += new EventHandler((s, e) =>
                    {
                        this.GenerarCotizacion();
                    });
                }
                this.oControlVentas.ctlCobro.CambiarCliente(this.oControlVentas.Cliente.ClienteID);
                this.oControlVentas.ctlCobro.Total = this.oControlVentas.Total;

                // Sólo se permite cambiar el vendedor, no cobrar
                this.oControlVentas.ctlCobro.HabilitarTipoDePago = false;
                this.oControlVentas.ctlCobro.MostrarFacturar = false;
                this.oControlVentas.ctlCobro.MostrarFacturarDividir = false;
                this.oControlVentas.ctlCobro.HabilitarCotizacion = true;
                this.oControlVentas.ctlCobro.HabilitarFormasDePago = false;
                this.oControlVentas.ctlCobro.Total = 0;

                this.oControlVentas.ctlCobro.BringToFront();
                return false;
            }

            // Se verifica si es cotización
            if (this.oControlVentas.EsCotizacion)
            {
                UtilLocal.MensajeAdvertencia("No se puede realizar la venta porque esta es sólo una cotización.");
                return false;
            }

            // Se intenta completar
            if (!this.oControlVentas.ctlCobro.CompletarCobro())
                return false;
            decimal mImportePato = Util.Decimal(Config.Valor("Ventas.ImportePato"));
            bool esPato = false;
            if (this.oControlVentas.Total >= mImportePato)
            {
                var hunt = new Duck(UtilLocal.RutaRecursos() + "huntduck2.wmv");
                hunt.Show(Principal.Instance);
                esPato = true;
            } else      // Se muestra la ventana de "Cargando.."
                Cargando.Mostrar();
            
            // Se procede a guardar la venta
            DateTime dAhora = DateTime.Now;
            // Se crea el objeto de la nueva venta
            Venta oVenta = new Venta()
            {
                Fecha = dAhora,
                ClienteID = this.oControlVentas.Cliente.ClienteID,
                VentaEstatusID = Cat.VentasEstatus.Realizada,
                RealizoUsuarioID = this.oControlVentas.ctlCobro.VendodorID,
                ComisionistaClienteID = this.oControlVentas.ctlCobro.ComisionistaID,
                ClienteVehiculoID = this.oControlVentas.ctlCobro.ClienteVehiculoID,
                Kilometraje = this.oControlVentas.ctlCobro.Kilometraje
            };
            // Se manda a guardar la venta
            var oVentaDetalle = this.oControlVentas.GenerarVentaDetalle();
            Guardar.Venta(oVenta, oVentaDetalle);

            // Se agrega al Kardex
            // ** Se hace al momento de cobrar :\

            // Se generan los datos de pago (no en la primer versión)
            /* var oPago = this.ctlCobro.GenerarPago();
            oPago.VentaID = oVenta.VentaID;
            oPago.Fecha = dAhora;
            var oPagoDetalle = this.ctlCobro.GenerarPagoDetalle();
            // Se mandan guardar los datos del pago
            Guardar.VentaPago(oPago, oPagoDetalle);
            */
            
            // Se imprimen los tickets correspondientes
            // .. aquí no hay tickets, sino hasta que se paga, creo

            // Se guarda la leyenda, para usarla en el ticket, después de cobrar
            if (this.oControlVentas.ctlCobro.Leyenda != "")
                VentasProc.AgregarLeyenda(oVenta.VentaID, this.oControlVentas.ctlCobro.Leyenda);

            // Se hace verificación para control de cascos
            foreach (var oReg in oVentaDetalle)
            {
                if (Datos.Exists<Parte>(c => c.ParteID == oReg.ParteID && c.RequiereCascoDe > 0 && c.Estatus))
                {
                    // Se agrega el registro para casco, uno por cada casco según la cantidad
                    for (int i = 0; i < oReg.Cantidad; i++)
                    {
                        var oCascoReg = new CascoRegistro()
                        {
                            Fecha = dAhora,
                            VentaID = oVenta.VentaID,
                            ParteID = oReg.ParteID
                        };
                        Datos.Guardar<CascoRegistro>(oCascoReg);
                    }
                }
            }

            // Se guardan los datos de la aplicación, si hubiera
            if (this.oControlVentas.oAplicaciones != null && this.oControlVentas.oAplicaciones.Count > 0)
            {
                foreach (var oReg in this.oControlVentas.oAplicaciones)
                {
                    var oRegVentaD = oVentaDetalle.FirstOrDefault(c => c.ParteID == oReg.ParteID);
                    oReg.VentaID = oVenta.VentaID;
                    oReg.Cantidad = oRegVentaD.Cantidad;
                    Datos.Guardar<VentaParteAplicacion>(oReg);
                }
            }

            // Se cierra la ventana de "Cargando.."
            if(!esPato)
                Cargando.Cerrar();
           
            // Se muestra notificación y se limpia el formulario
            UtilLocal.MostrarNotificacion("La Venta ha sido guardada correctamente.");

            return true;
        }

        public void GenerarCotizacion()
        {
            // Se guarda la cotización en la base de datos
            var dAhora = DateTime.Now;
            var oCot = new VentaCotizacion()
            {
                Fecha = dAhora,
                SucursalID = GlobalClass.SucursalID,
                ClienteID = this.oControlVentas.Cliente.ClienteID,
                VendedorID = this.oControlVentas.ctlCobro.VendodorID
            };
            Datos.Guardar<VentaCotizacion>(oCot);
            // Detalle
            var oVentaDetalle = this.oControlVentas.GenerarVentaDetalle();
            foreach (var oReg in oVentaDetalle)
            {
                var oParteCot = new VentaCotizacionDetalle()
                {
                    VentaCotizacionID = oCot.VentaCotizacionID,
                    ParteID = oReg.ParteID,
                    Cantidad = oReg.Cantidad,
                    PrecioUnitario = oReg.PrecioUnitario,
                    Iva = oReg.Iva
                };
                Datos.Guardar<VentaCotizacionDetalle>(oParteCot);
            }

            // Para generar el ticket de la cotización
            var oVendedor = Datos.GetEntity<Usuario>(q => q.UsuarioID == this.oControlVentas.ctlCobro.VendodorID && q.Estatus);
            var oVentaV = new VentasView()
            {
                Fecha = dAhora,
                Cliente = this.oControlVentas.Cliente.Nombre,
                Vendedor = oVendedor.NombrePersona,
                Total = this.oControlVentas.Total,
                SucursalID = GlobalClass.SucursalID
            };
            List<VentasDetalleView> oVentaDetalleV = new List<VentasDetalleView>();
            Parte oParte;
            foreach (var oDet in oVentaDetalle)
            {
                oParte = Datos.GetEntity<Parte>(q => q.ParteID == oDet.ParteID && q.Estatus);
                oVentaDetalleV.Add(new VentasDetalleView()
                {
                    NumeroParte = oParte.NumeroParte,
                    NombreParte = oParte.NombreParte,
                    Cantidad = oDet.Cantidad,
                    PrecioUnitario = oDet.PrecioUnitario,
                    Iva = oDet.Iva
                });
            }
            VentasLoc.GenerarTicketDeCotizacion(oVentaV, oVentaDetalleV);
        }
    }
}
