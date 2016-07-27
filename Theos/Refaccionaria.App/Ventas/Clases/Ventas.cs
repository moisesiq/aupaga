using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using FastReport;

using LibUtil;
using TheosProc;
using FacturacionElectronica;

namespace Refaccionaria.App
{
    static class VentasLoc
    {
        #region [ Impresión de tickets ]

        public static void TicketAgregarLeyendas(ref Report oReporte)
        {
            var oLeyendas = TheosProc.Config.ValoresVarios("Tickets.Leyenda");
            oReporte.SetParameterValue("Leyenda1", oLeyendas["Tickets.Leyenda1"]);
            oReporte.SetParameterValue("Leyenda2", oLeyendas["Tickets.Leyenda2"]);
            oReporte.SetParameterValue("Leyenda3", oLeyendas["Tickets.Leyenda3"]);
            oReporte.SetParameterValue("Leyenda4", oLeyendas["Tickets.Leyenda4"]);
            oReporte.SetParameterValue("Leyenda5", oLeyendas["Tickets.Leyenda5"]);
            oReporte.SetParameterValue("Leyenda6", oLeyendas["Tickets.Leyenda6"]);
            oReporte.SetParameterValue("Leyenda7", oLeyendas["Tickets.Leyenda7"]);
        }

        public static void GenerarTicketDeVenta(int iVentaID, List<ProductoVenta> oListaVenta, Dictionary<string, object> oAdicionales)
        {
            var oVentaV = Datos.GetEntity<VentasView>(q => q.VentaID == iVentaID);
            var oVentaDetalle = Datos.GetListOf<VentasDetalleView>(q => q.VentaID == iVentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaTicket.frx");
            VentasLoc.TicketAgregarLeyendas(ref oRep);

            // Se obtienen las formas de pago
            string sFormaDePago = VentasProc.GenerarFormaDePago(iVentaID);

            // Se modifican las descripciones, si aplica
            if (oListaVenta != null)
            {
                foreach (var oDet in oVentaDetalle)
                {
                    var oParteVenta = oListaVenta.FirstOrDefault(c => c.ParteID == oDet.ParteID && c.Cantidad == oDet.Cantidad
                        && c.PrecioUnitario == oDet.PrecioUnitario && c.Iva == oDet.Iva);
                    if (oParteVenta != null)
                        oDet.NombreParte = oParteVenta.NombreDeParte;
                }
            }

            // Se mandan los datos al reporte
            oRep.RegisterData(new List<VentasView>() { oVentaV }, "Venta");
            oRep.RegisterData(oVentaDetalle, "VentaDetalle");
            oRep.SetParameterValue("FormaDePago", sFormaDePago);
            oRep.SetParameterValue("TotalConLetra", Util.ImporteALetra(oVentaV.Total).ToUpper());
            oRep.SetParameterValue("LeyendaDeVenta", VentasProc.ObtenerQuitarLeyenda(iVentaID));
            oRep.SetParameterValue("LeyendaVehiculo", (oVentaV.ClienteVehiculoID.HasValue ? UtilDatos.LeyendaDeVehiculo(oVentaV.ClienteVehiculoID.Value) : ""));
            oRep.SetParameterValue("Precio1", false);
            // Se agregan los adicionales, si hubiera
            if (oAdicionales != null)
            {
                foreach (var oAd in oAdicionales)
                    oRep.SetParameterValue(oAd.Key, oAd.Value);
            }

            UtilLocal.EnviarReporteASalida("Reportes.VentaTicket.Salida", oRep);
        }

        public static void GenerarTicketDeVenta(int iVentaID)
        {
            VentasLoc.GenerarTicketDeVenta(iVentaID, null, null);
        }

        public static void GenerarTicketDeCotizacion(VentasView oVenta, List<VentasDetalleView> oVentaDetalle)
        {
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CotizacionTicket.frx");
            VentasLoc.TicketAgregarLeyendas(ref oRep);

            oRep.RegisterData(new List<VentasView>() { oVenta }, "Venta");
            oRep.RegisterData(oVentaDetalle, "VentaDetalle");
            oRep.SetParameterValue("TotalConLetra", Util.ImporteALetra(oVenta.Total).ToUpper());
            oRep.SetParameterValue("Precio1", false);

            UtilLocal.EnviarReporteASalida("Reportes.VentaCotizacion.Salida", oRep);
        }

        public static void GenerarTicketPrecio1(int VentaID)
        {
            var oVentaDetalles = Datos.GetListOf<VentasDetalleView>(v => v.VentaID == VentaID);
            var oVenta = Datos.GetEntity<VentasView>(v => v.VentaID == VentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaTicket.frx");
            VentasLoc.TicketAgregarLeyendas(ref oRep);

            decimal total = 0;
            var oVentaPrecio1 = new List<VentasDetalleView>();
            foreach (var oVentaDetalle in oVentaDetalles)
            {
                var oParte = Datos.GetEntity<PartePrecio>(p => p.ParteID == oVentaDetalle.ParteID);
                var precioConIva = Util.Decimal(oParte.PrecioUno);
                var precioSinIva = UtilTheos.ObtenerPrecioSinIva(Util.Decimal(oParte.PrecioUno));
                var iva = Util.Decimal(oParte.PrecioUno) - precioSinIva;
                oVentaPrecio1.Add(new VentasDetalleView()
                {
                    VentaDetalleID = oVentaDetalle.VentaDetalleID,
                    VentaID = oVentaDetalle.VentaID,
                    ParteID = oVentaDetalle.ParteID,
                    NumeroParte = oVentaDetalle.NumeroParte,
                    NombreParte = oVentaDetalle.NombreParte,
                    Costo = oVentaDetalle.Costo,
                    CostoConDescuento = oVentaDetalle.CostoConDescuento,
                    Cantidad = oVentaDetalle.Cantidad,
                    Medida = oVentaDetalle.Medida,
                    LineaID = oVentaDetalle.LineaID,
                    PrecioUnitario = precioSinIva,
                    Iva = iva
                });
                total += precioConIva * oVentaDetalle.Cantidad;
            }
            var oVentasViewPrecio1 = new VentasView()
            {
                VentaID = oVenta.VentaID,
                Facturada = oVenta.Facturada,
                Folio = oVenta.Folio,
                ClienteID = oVenta.ClienteID,
                Cliente = oVenta.Cliente,
                SucursalID = oVenta.SucursalID,
                Sucursal = oVenta.Sucursal,
                VentaEstatusID = oVenta.VentaEstatusID,
                Estatus = oVenta.Estatus,
                Subtotal = UtilTheos.ObtenerPrecioSinIva(total),
                Iva = UtilTheos.ObtenerIvaDePrecio(total),
                Total = total,
                Pagado = oVenta.Pagado,
                ACredito = oVenta.ACredito,
                VendedorID = oVenta.VendedorID,
                Vendedor = oVenta.Vendedor,
                VendedorUsuario = oVenta.VendedorUsuario,
                ComisionistaID = oVenta.ComisionistaID,
                ClienteVehiculoID = oVenta.ClienteVehiculoID,
                Kilometraje = oVenta.Kilometraje
            };


            oRep.RegisterData(new List<VentasView>() { oVentasViewPrecio1 }, "Venta");
            oRep.RegisterData(oVentaPrecio1, "VentaDetalle");
            oRep.SetParameterValue("TotalConLetra", Util.ImporteALetra(oVentasViewPrecio1.Total).ToUpper());
            oRep.SetParameterValue("Precio1", true);
            UtilLocal.EnviarReporteASalida("Reportes.VentaTicket.Salida", oRep);
        }

        public static void GenerarTicketDe9500(int i9500ID)
        {
            var o9500V = Datos.GetEntity<Cotizaciones9500View>(q => q.Cotizacion9500ID == i9500ID);
            var o9500DetV = Datos.GetListOf<Cotizaciones9500DetalleView>(q => q.Cotizacion9500ID == i9500ID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "9500Ticket.frx");
            VentasLoc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<Cotizaciones9500View>() { o9500V }, "c9500");
            oRep.RegisterData(o9500DetV, "c9500Detalle");
            oRep.SetParameterValue("AnticipoConLetra", Util.ImporteALetra(o9500V.Anticipo).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.9500Ticket.Salida", oRep);
        }

        public static bool ReimprimirFactura(string sFactura)
        {
            var oFactura = Datos.GetEntity<VentasFacturasView>(q => q.SerieFolio == sFactura);

            string sRutaPdf = TheosProc.Config.Valor("Facturacion.RutaPdf");
            sRutaPdf += ("\\" + oFactura.Fecha.Year.ToString() + "\\" + oFactura.Fecha.Month.ToString() + "\\");
            sRutaPdf += (sFactura + ".pdf");
            if (File.Exists(sRutaPdf))
            {
                Process.Start(sRutaPdf);
                return true;
            }
            else
            {
                UtilLocal.MensajeAdvertencia("No se ha encontrado el archivo Pdf correspondiente a la factura especificada.");
                return false;
            }
        }

        public static void GenerarTicketDevolucion(int iDevolucionID)
        {
            var oDevolucionV = Datos.GetEntity<VentasDevolucionesView>(q => q.VentaDevolucionID == iDevolucionID);
            var oDevolucionDetalleV = Datos.GetListOf<VentasDevolucionesDetalleView>(q => q.VentaDevolucionID == iDevolucionID);
            var oVentaV = Datos.GetEntity<VentasView>(q => q.VentaID == oDevolucionV.VentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaDevolucionTicket.frx");
            VentasLoc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<VentasDevolucionesView>() { oDevolucionV }, "Devolucion");
            oRep.RegisterData(oDevolucionDetalleV, "DevolucionDetalle");
            oRep.RegisterData(new List<VentasView>() { oVentaV }, "Venta");
            oRep.SetParameterValue("NC", (oDevolucionV.FormaDePagoID == Cat.FormasDePago.Vale));
            oRep.SetParameterValue("Accion", oDevolucionV.FormaDePago);
            oRep.SetParameterValue("FolioNC", oDevolucionV.NotaDeCreditoID.Valor());
            oRep.SetParameterValue("TotalConLetra", Util.ImporteALetra(oDevolucionV.Total.Valor()).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.VentaDevolucionTicket.Salida", oRep);
        }

        public static void GenerarTicketGarantia(int iGarantiaID)
        {
            var oGarantiaV = Datos.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == iGarantiaID);
            var oVentaV = Datos.GetEntity<VentasView>(c => c.VentaID == oGarantiaV.VentaID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "VentaGarantiaTicket.frx");
            VentasLoc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<VentasGarantiasView>() { oGarantiaV }, "Garantia");
            oRep.RegisterData(new List<VentasView>() { oVentaV }, "Venta");
            // oRep.SetParameterValue("NC", (oGarantiaV.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito));
            oRep.SetParameterValue("TotalConLetra", Util.ImporteALetra(oGarantiaV.Total.Valor()).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.VentaGarantiaTicket.Salida", oRep);
        }

        public static void GenerarTicketCobranza(string sFolio)
        {
            var oCobradas = Datos.GetListOf<CobranzaTicket>(q => q.Ticket == sFolio);
            if (oCobradas.Count <= 0) return;

            // Se obtiene el total de lo pagado
            decimal mTotalPagado = oCobradas.Sum(c => c.Pagado);
            string sPagadoLetra = Util.ImporteALetra(mTotalPagado).ToUpper();

            int iClienteID = oCobradas[0].ClienteID;
            var oClienteV = Datos.GetEntity<ClientesDatosView>(q => q.ClienteID == iClienteID);

            // Se agrupan las facturas de varios tickets
            var oCobradasAg = oCobradas.GroupBy(c => c.Folio).Select(c => new
            {
                Folio = c.Key,
                Fecha = c.Min(s => s.Fecha),
                Vencimiento = c.Min(s => s.Vencimiento)
                ,
                Total = c.Sum(s => s.Total),
                Pagado = c.Sum(s => s.Pagado),
                Restante = c.Sum(s => s.Restante)
            });

            // Se genera el reporte
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CobranzaTicket.frx");
            VentasLoc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<ClientesDatosView>() { oClienteV }, "Cliente");
            oRep.RegisterData(oCobradasAg, "Ventas");
            oRep.SetParameterValue("TotalPagado", mTotalPagado);
            oRep.SetParameterValue("TotalPagadoLetra", sPagadoLetra);
            //
            UtilLocal.EnviarReporteASalida("Reportes.CobranzaTicket.Salida", oRep);
        }

        public static void GenerarTicketGasto(int iEgresoID)
        {
            var oEgresoV = Datos.GetEntity<CajaEgresosView>(c => c.CajaEgresoID == iEgresoID);
            if (oEgresoV == null) return;

            // Se genera el reporte
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CajaGastoTicket.frx");
            // VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<CajaEgresosView>() { oEgresoV }, "CajaEgreso");
            //
            UtilLocal.EnviarReporteASalida("Reportes.CajaGastoTicket.Salida", oRep);
        }

        public static void GenerarTicketNotaDeCredito(int iNotaDeCreditoID)
        {
            var oNotaDeCreditoV = Datos.GetEntity<NotasDeCreditoView>(c => c.NotaDeCreditoID == iNotaDeCreditoID);
            if (oNotaDeCreditoV == null) return;

            // Se genera el reporte
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "NotaDeCreditoTicket.frx");
            // VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<NotasDeCreditoView>() { oNotaDeCreditoV }, "NotaDeCredito");
            //
            UtilLocal.EnviarReporteASalida("Reportes.NotaDeCreditoTicket.Salida", oRep);
        }

        public static void GenerarTicketCasco(int iCascoID)
        {
            var oCascoRegV = Datos.GetEntity<CascosRegistrosView>(c => c.CascoRegistroID == iCascoID);
            if (oCascoRegV == null) return;

            var oRep = new Report();
            oRep.Load(UtilLocal.RutaReportes("ControlDeCascos.frx"));
            VentasLoc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<CascosRegistrosView>() { oCascoRegV }, "ControlDeCasco");
            UtilLocal.EnviarReporteASalida("Reportes.ControlDeCascos.Completar.Salida", oRep);
        }

        #endregion

        #region [ Facturación electrónica ]

        public static Dictionary<string, string> GenerarFolioDeFactura()
        {
            var Res = new Dictionary<string, string>();
            Res.Add("Serie", TheosProc.Config.Valor("Facturacion.Serie"));
            Res.Add("Folio", TheosProc.Config.Valor("Facturacion.Folio"));
            TheosProc.Config.EstablecerValor("Facturacion.Folio", (Util.Entero(Res["Folio"]) + 1).ToString());
            return Res;
        }

        private static void RegresarFolioDeFacrtura(string sFolio)
        {
            // Se obtiene el último folio de la serie especificada
            string sUltFolio = TheosProc.Config.Valor("Facturacion.Folio");
            int iFolioAnt = (Util.Entero(sUltFolio) - 1);
            int iFolio = Util.Entero(sFolio);
            if (iFolio == iFolioAnt)
                TheosProc.Config.EstablecerValor("Facturacion.Folio", (iFolio).ToString());
        }

        public static Dictionary<string, string> GenerarFolioDeFacturaDevolucion()
        {
            var Res = new Dictionary<string, string>();
            Res.Add("Serie", TheosProc.Config.Valor("Facturacion.Devolucion.Serie"));
            Res.Add("Folio", TheosProc.Config.Valor("Facturacion.Devolucion.Folio"));
            TheosProc.Config.EstablecerValor("Facturacion.Devolucion.Folio", (Util.Entero(Res["Folio"]) + 1).ToString());
            return Res;
        }

        private static void FeLLenarConfiguracion(ref FacturaElectronica oFacturaE, Dictionary<string, string> oConfig)
        {
            oFacturaE.Configuracion = new FacturacionElectronica.Config()
            {
                RutaCertificado = oConfig["Facturacion.RutaCertificado"],
                RutaArchivoKey = oConfig["Facturacion.RutaArchivoKey"],
                ContraseniaArchivoKey = oConfig["Facturacion.ContraseniaArchivoKey"],
                RutaArchivoPfx = oConfig["Facturacion.RutaArchivoPfx"],
                ContraseniaArchivoPfx = oConfig["Facturacion.ContraseniaArchivoPfx"],
                UsuarioPac = oConfig["Facturacion.UsuarioPac"],
                ContraseniaPac = oConfig["Facturacion.ContraseniaPac"]
            };
        }

        private static void FeLlenarDatosComunes(ref FacturaElectronica oFacturaE, Dictionary<string, string> oConfig)
        {
            // Se llenan los valores de configuración
            VentasLoc.FeLLenarConfiguracion(ref oFacturaE, oConfig);

            // Se llenan los datos del Emisor
            var oConfigMatriz = TheosProc.Config.ValoresVarios(Cat.Sucursales.Matriz, "Facturacion.");
            oFacturaE.Emisor = new Emisor()
            {
                RFC = oConfig["Facturacion.Rfc"],
                Nombre = oConfig["Facturacion.RazonSocial"],
                DomicilioFiscal = new Ubicacion()
                {
                    Calle = oConfigMatriz["Facturacion.Calle"],
                    NumeroExterior = oConfigMatriz["Facturacion.NumeroExterior"],
                    NumeroInterior = oConfigMatriz["Facturacion.NumeroInterior"],
                    Referencia = oConfigMatriz["Facturacion.Referencia"],
                    Colonia = oConfigMatriz["Facturacion.Colonia"],
                    CodigoPostal = oConfigMatriz["Facturacion.CodigoPostal"],
                    Localidad = oConfigMatriz["Facturacion.Localidad"],
                    Municipio = oConfigMatriz["Facturacion.Municipio"],
                    Estado = oConfigMatriz["Facturacion.Estado"],
                    Pais = oConfigMatriz["Facturacion.Pais"]
                },
                ExpedidoEn = new Ubicacion()
                {
                    Calle = oConfig["Facturacion.Calle"],
                    NumeroExterior = oConfig["Facturacion.NumeroExterior"],
                    NumeroInterior = oConfig["Facturacion.NumeroInterior"],
                    Referencia = oConfig["Facturacion.Referencia"],
                    Colonia = oConfig["Facturacion.Colonia"],
                    CodigoPostal = oConfig["Facturacion.CodigoPostal"],
                    Localidad = oConfig["Facturacion.Localidad"],
                    Municipio = oConfig["Facturacion.Municipio"],
                    Estado = oConfig["Facturacion.Estado"],
                    Pais = oConfig["Facturacion.Pais"]
                },
                RegimenesFiscales = new List<string>(oConfig["Facturacion.RegimenesFiscales"].Split(','))
            };
        }

        private static ResAcc<bool> FeLlenarDatosReceptor(ref FacturaElectronica oFacturaE, int iClienteID)
        {
            var oClienteF = Datos.GetEntity<ClientesFacturacionView>(q => q.ClienteID == iClienteID);
            if (oClienteF == null)
                return new ResAcc<bool>(false, "El cliente seleccionado no tiene datos de facturación registrados. No se puede generar la factura.");
            oFacturaE.Receptor = new Receptor()
            {
                RFC = oClienteF.Rfc,
                Nombre = oClienteF.RazonSocial,
                DomicilioFiscal = new Ubicacion()
                {
                    Calle = oClienteF.Calle,
                    NumeroExterior = oClienteF.NumeroExterior,
                    NumeroInterior = oClienteF.NumeroInterior,
                    Referencia = oClienteF.Referencia,
                    Colonia = oClienteF.Colonia,
                    CodigoPostal = oClienteF.CodigoPostal,
                    Localidad = oClienteF.Localidad,
                    Municipio = oClienteF.Municipio,
                    Estado = oClienteF.Estado,
                    Pais = oClienteF.Pais
                }
            };

            return new ResAcc<bool>(true);
        }

        private static ResAcc<string> FeEnviarFactura(ref FacturaElectronica oFacturaE)
        {
            // Se genera el Xml inicial, con el formato que pide el Sat
            var ResXml = oFacturaE.GenerarFactura(true);
            if (ResXml.Error)
                return new ResAcc<string>(false, ResXml.Mensaje);
            string sCfdi = ResXml.Respuesta;

            // Se manda a timbrar el Xml con el proveedor Pac
            ResXml = oFacturaE.TimbrarFactura(sCfdi, !GlobalClass.Produccion);
            if (ResXml.Error)
                return new ResAcc<string>(false, ResXml.Mensaje);
            string sCfdiTimbrado = ResXml.Respuesta;

            var Res = new ResAcc<string>(true);
            Res.Respuesta = sCfdiTimbrado;
            return Res;
        }

        private static Report FeGuardarArchivosFactura(ref FacturaElectronica oFacturaE, string sCfdiTimbrado, string sSerieFolio)
        {
            // Se obtienen las rutas a utilizar
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = TheosProc.Config.Valor("Facturacion.RutaPdf");
            string sRutaXml = TheosProc.Config.Valor("Facturacion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += sAnioMes;
            Directory.CreateDirectory(sRutaPdf);
            sRutaXml += sAnioMes;
            Directory.CreateDirectory(sRutaXml);

            // Se guarda el xml
            string sArchivoXml = (sRutaXml + sSerieFolio + ".xml");
            File.WriteAllText(sArchivoXml, sCfdiTimbrado, Encoding.UTF8);

            // Se manda la salida de la factura
            string sArchivoPdf = (sRutaPdf + sSerieFolio + ".pdf");
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "Cfdi.frx");
            var ObjetoCbb = new { Imagen = oFacturaE.GenerarCbb() };
            oRep.RegisterData(new List<FacturaElectronica>() { oFacturaE }, "Factura");
            oRep.RegisterData(new List<object>() { ObjetoCbb }, "Cbb");
            oRep.SetParameterValue("ACredito", false);
            oRep.SetParameterValue("Vendedor", "");
            if (oFacturaE.Adicionales != null)
            {
                if (oFacturaE.Adicionales.ContainsKey("ACredito"))
                    oRep.SetParameterValue("ACredito", bool.Parse(oFacturaE.Adicionales["ACredito"]));
                if (oFacturaE.Adicionales.ContainsKey("Vendedor"))
                    oRep.SetParameterValue("Vendedor", oFacturaE.Adicionales["Vendedor"].ToUpper());
                if (oFacturaE.Adicionales.ContainsKey("Vencimiento"))
                    oRep.SetParameterValue("Vencimiento", oFacturaE.Adicionales["Vencimiento"]);
                if (oFacturaE.Adicionales.ContainsKey("LeyendaDeVenta"))
                    oRep.SetParameterValue("LeyendaDeVenta", oFacturaE.Adicionales["LeyendaDeVenta"]);
                if (oFacturaE.Adicionales.ContainsKey("LeyendaDeVehiculo"))
                    oRep.SetParameterValue("LeyendaDeVehiculo", oFacturaE.Adicionales["LeyendaDeVehiculo"]);
                if (oFacturaE.Adicionales.ContainsKey("EfectivoRecibido"))
                    oRep.SetParameterValue("EfectivoRecibido", Util.Decimal(oFacturaE.Adicionales["EfectivoRecibido"]));
                if (oFacturaE.Adicionales.ContainsKey("Cambio"))
                    oRep.SetParameterValue("Cambio", Util.Decimal(oFacturaE.Adicionales["Cambio"]));
                if (oFacturaE.Adicionales.ContainsKey("LeyendaDePago"))
                    oRep.SetParameterValue("LeyendaDePago", oFacturaE.Adicionales["LeyendaDePago"]);
            }
            oRep.SetParameterValue("TotalConLetra", Util.ImporteALetra(oFacturaE.Total).ToUpper());

            UtilLocal.EnviarReporteASalida("Reportes.VentaFactura.Salida", oRep);

            // Se guarda el pdf
            var oRepPdf = new FastReport.Export.Pdf.PDFExport() { ShowProgress = true };
            oRep.Prepare();
            oRep.Export(oRepPdf, sArchivoPdf);

            return oRep;
        }

        private static Report FeGuardarArchivosFacturaDevolucion(ref FacturaElectronica oFacturaE, string sCfdiTimbrado, string sSerieFolio)
        {
            // Se obtienen las rutas a utilizar
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = TheosProc.Config.Valor("Facturacion.Devolucion.RutaPdf");
            string sRutaXml = TheosProc.Config.Valor("Facturacion.Devolucion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += sAnioMes;
            Directory.CreateDirectory(sRutaPdf);
            sRutaXml += sAnioMes;
            Directory.CreateDirectory(sRutaXml);

            // Se guarda el xml
            string sArchivoXml = (sRutaXml + sSerieFolio + ".xml");
            File.WriteAllText(sArchivoXml, sCfdiTimbrado, Encoding.UTF8);

            // Se manda la salida de la factura
            string sArchivoPdf = (sRutaPdf + sSerieFolio + ".pdf");
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "CfdiNotaDeCredito.frx");
            var ObjetoCbb = new { Imagen = oFacturaE.GenerarCbb() };
            oRep.RegisterData(new List<FacturaElectronica>() { oFacturaE }, "Factura");
            oRep.RegisterData(new List<object>() { ObjetoCbb }, "Cbb");
            oRep.SetParameterValue("Vendedor", oFacturaE.Adicionales["Vendedor"].ToUpper());
            oRep.SetParameterValue("TotalConLetra", Util.ImporteALetra(oFacturaE.Total).ToUpper());
            oRep.SetParameterValue("FacturaOrigen", oFacturaE.Adicionales["FacturaOrigen"]);

            UtilLocal.EnviarReporteASalida("Reportes.VentaFacturaDevolucion.Salida", oRep);

            // Se guarda el pdf
            var oRepPdf = new FastReport.Export.Pdf.PDFExport() { ShowProgress = true };
            oRep.Prepare();
            oRep.Export(oRepPdf, sArchivoPdf);

            return oRep;
        }

        private static bool EnviarFacturaPorCorreo(int iFacturaID)
        {
            // Se verifica si es pruebas, en cuyo caso, no se envía el correo
            if (!GlobalClass.Produccion) return false;

            // Se obtienen los correos para envío
            var oVentaFactura = Datos.GetEntity<VentaFactura>(q => q.VentaFacturaID == iFacturaID && q.Estatus);
            string sPara = VentasLoc.ObtenerCorreosPersonalCliente(oVentaFactura.ClienteID);
            if (sPara == "") return false;

            // Se obtienen las rutas de los archivos
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = TheosProc.Config.Valor("Facturacion.RutaPdf");
            string sRutaXml = TheosProc.Config.Valor("Facturacion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += (sAnioMes + oVentaFactura.Serie + oVentaFactura.Folio + ".pdf");
            sRutaXml += (sAnioMes + oVentaFactura.Serie + oVentaFactura.Folio + ".xml");

            string sRutaFormato = (TheosProc.Config.Valor("Correo.RutaFormatos") + "CorreoFactura.html");

            if (!File.Exists(sRutaPdf) || !File.Exists(sRutaXml) || !File.Exists(sRutaFormato))
                return false;

            // Se carga el formato
            string sMensaje = File.ReadAllText(sRutaFormato);
            string sAsunto = sMensaje.Extraer("<title>", "</title>");
            // Se cargan los archivos como Sreams
            var oPdf = new FileStream(sRutaPdf, FileMode.Open);
            var oXml = new FileStream(sRutaXml, FileMode.Open);
            var oAdjuntos = new Dictionary<string, Stream>();
            oAdjuntos.Add(Path.GetFileName(sRutaPdf), oPdf);
            oAdjuntos.Add(Path.GetFileName(sRutaXml), oXml);

            // Se procesa el envío de correo
            return UtilLocal.EnviarCorreo(sAsunto, sMensaje, sPara, oAdjuntos);
        }

        private static bool EnviarFacturaCancelacionPorCorreo(int iFacturaID)
        {
            // Se obtienen los correos para envío
            var oVentaFactura = Datos.GetEntity<VentaFactura>(q => q.VentaFacturaID == iFacturaID && q.Estatus);
            string sPara = VentasLoc.ObtenerCorreosPersonalCliente(oVentaFactura.ClienteID);
            if (sPara == "") return false;

            // Se obtienen las rutas de los archivos
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = TheosProc.Config.Valor("Facturacion.RutaPdf");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += (sAnioMes + oVentaFactura.Serie + oVentaFactura.Folio + ".pdf");

            string sRutaFormato = (TheosProc.Config.Valor("Correo.RutaFormatos") + "CorreoFacturaCancelacion.html");

            if (!File.Exists(sRutaPdf) || !File.Exists(sRutaFormato))
                return false;

            // Se carga el formato
            string sMensaje = File.ReadAllText(sRutaFormato);
            string sAsunto = sMensaje.Extraer("<title>", "</title>");
            // Se procesan campos
            sMensaje = sMensaje.Replace("{__FolioInterno}", (oVentaFactura.Serie + oVentaFactura.Folio));
            sMensaje = sMensaje.Replace("{__FolioFiscal}", oVentaFactura.FolioFiscal);
            sMensaje = sMensaje.Replace("{__Fecha}", oVentaFactura.Fecha.ToString(GlobalClass.FormatoFechaHora));
            // Se cargan los archivos como Sreams
            var oPdf = new FileStream(sRutaPdf, FileMode.Open);
            var oAdjuntos = new Dictionary<string, Stream>();
            oAdjuntos.Add(Path.GetFileName(sRutaPdf), oPdf);

            // Se procesa el envío de correo
            return UtilLocal.EnviarCorreo(sAsunto, sMensaje, sPara, oAdjuntos);
        }

        private static bool EnviarFacturaDevolucionPorCorreo(int iFacturaDevolucionID)
        {
            // Se obtienen los correos para envío
            var oFacturaDev = Datos.GetEntity<VentaFacturaDevolucion>(q => q.VentaFacturaDevolucionID == iFacturaDevolucionID && q.Estatus);
            var oFactura = Datos.GetEntity<VentaFactura>(q => q.VentaFacturaID == oFacturaDev.VentaFacturaID && q.Estatus);
            string sPara = VentasLoc.ObtenerCorreosPersonalCliente(oFactura.ClienteID);
            if (sPara == "") return false;

            // Se obtienen las rutas de los archivos
            DateTime dAhora = DateTime.Now;
            string sRutaPdf = TheosProc.Config.Valor("Facturacion.Devolucion.RutaPdf");
            string sRutaXml = TheosProc.Config.Valor("Facturacion.Devolucion.RutaXml");
            string sAnioMes = (@"\" + dAhora.Year.ToString() + @"\" + dAhora.Month.ToString() + @"\");
            sRutaPdf += (sAnioMes + oFacturaDev.Serie + oFacturaDev.Folio + ".pdf");
            sRutaXml += (sAnioMes + oFacturaDev.Serie + oFacturaDev.Folio + ".xml");

            string sRutaFormato = (TheosProc.Config.Valor("Correo.RutaFormatos") + "CorreoFacturaDevolucion.html");

            if (!File.Exists(sRutaPdf) || !File.Exists(sRutaXml) || !File.Exists(sRutaFormato))
                return false;

            // Se carga el formato
            string sMensaje = File.ReadAllText(sRutaFormato);
            string sAsunto = sMensaje.Extraer("<title>", "</title>");
            // Se procesan campos
            sMensaje = sMensaje.Replace("{__FolioInterno}", (oFactura.Serie + oFactura.Folio));
            sMensaje = sMensaje.Replace("{__FolioFiscal}", oFactura.FolioFiscal);
            sMensaje = sMensaje.Replace("{__Fecha}", oFactura.Fecha.ToString(GlobalClass.FormatoFechaHora));
            // Se cargan los archivos como Sreams
            var oPdf = new FileStream(sRutaPdf, FileMode.Open);
            var oXml = new FileStream(sRutaXml, FileMode.Open);
            var oAdjuntos = new Dictionary<string, Stream>();
            oAdjuntos.Add(Path.GetFileName(sRutaPdf), oPdf);
            oAdjuntos.Add(Path.GetFileName(sRutaXml), oXml);

            // Se procesa el envío de correo
            return UtilLocal.EnviarCorreo(sAsunto, sMensaje, sPara, oAdjuntos);
        }

        private static string ObtenerCorreosPersonalCliente(int iClienteID)
        {
            var oPersonas = Datos.GetListOf<ClientePersonal>(q => q.ClienteID == iClienteID && q.EnviarCfdi && q.Estatus);
            string sPara = "";
            foreach (var oPersona in oPersonas)
            {
                if (UtilLocal.IsEmailValid(oPersona.CorreoElectronico))
                    sPara += (", " + oPersona.CorreoElectronico);
            }
            // if (sPara == "")
            //     return (oPersonas.Count <= 0);
            // sPara = sPara.Substring(2);
            sPara = (sPara == "" ? "" : sPara.Substring(2));
            return sPara;
        }

        public static ResAcc<int> GenerarFacturaElectronica(List<int> VentasIDs, int iClienteID
            , List<ProductoVenta> oListaVenta, List<VentasPagosDetalleView> oFormasDePago, string sObservacion, Dictionary<string, object> oAdicionales)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = TheosProc.Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasLoc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtienen las ventas a facturar, y los vendedores
            var oVentas = new List<VentasView>();
            string sVendedores = "";
            foreach (int iVentaID in VentasIDs)
            {
                var oVentaV = Datos.GetEntity<VentasView>(q => q.VentaID == iVentaID);
                oVentas.Add(oVentaV);
                sVendedores += (", " + oVentaV.Vendedor);
            }
            sVendedores = (sVendedores == "" ? "" : sVendedores.Substring(2));
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);
            if (oVentas.Count > 0)
            {
                oFacturaE.Adicionales.Add("ACredito", oVentas[0].ACredito.ToString());
                // Se llena el dato de la fecha de vencimiento, sólo de la primer venta
                int iClienteVenID = oVentas[0].ClienteID;
                var oCliente = Datos.GetEntity<Cliente>(q => q.ClienteID == iClienteVenID && q.Estatus);
                oFacturaE.Adicionales.Add("Vencimiento", oVentas[0].Fecha.AddDays(oCliente.DiasDeCredito.Valor()).ToShortDateString());
                // Leyenda de la venta
                oFacturaE.Adicionales.Add("LeyendaDeVenta", VentasProc.ObtenerQuitarLeyenda(oVentas[0].VentaID));
                // Si no hay leyenda, se verifica si hay observación (por tickets convertidos a factura), y si hay, se manda como leyenda
                if (string.IsNullOrEmpty(oFacturaE.Adicionales["LeyendaDeVenta"]) && sObservacion != "")
                    oFacturaE.Adicionales["LeyendaDeVenta"] = sObservacion;
                // Leyenda de vehículo, si aplica
                if (oVentas[0].ClienteVehiculoID.HasValue)
                    oFacturaE.Adicionales.Add("LeyendaDeVehiculo", UtilDatos.LeyendaDeVehiculo(oVentas[0].ClienteVehiculoID.Value));
                // Se agregan los adicionales, si hubiera
                if (oAdicionales != null)
                {
                    if (oAdicionales.ContainsKey("EfectivoRecibido"))
                        oFacturaE.Adicionales.Add("EfectivoRecibido", Util.Cadena(oAdicionales["EfectivoRecibido"]));
                    if (oAdicionales.ContainsKey("Cambio"))
                        oFacturaE.Adicionales.Add("Cambio", Util.Cadena(oAdicionales["Cambio"]));
                }
            }

            // Se procesa la forma de pago con nueva modalidad del sat
            string sFormaDePago = VentasLoc.GenerarMetodoDePagoFactura(oFormasDePago);
            oFacturaE.MetodoDePago = sFormaDePago;
            oFacturaE.CuentaPago = VentasLoc.ObtenerCuentaDePago(oFormasDePago);
            // Se agraga la cadena del método de pago, como adicional
            oFacturaE.Adicionales.Add("LeyendaDePago", VentasLoc.GenerarCadenaMetodoDePagoFactura(oFormasDePago));
            
            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "UNA SOLA EXHIBICIÓN";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Ingreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();

            // Se llenan los datos del receptor
            var ResRec = VentasLoc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            decimal mUnitarioTotal = 0, mIvaTotal = 0;
            oFacturaE.Conceptos = new List<Concepto>();
            if (oListaVenta == null)
            {
                List<VentasDetalleView> oVentaDetalle;
                foreach (var oVentaCon in oVentas)
                {
                    oVentaDetalle = Datos.GetListOf<VentasDetalleView>(q => q.VentaID == oVentaCon.VentaID);
                    foreach (var oConcepto in oVentaDetalle)
                    {
                        oFacturaE.Conceptos.Add(new Concepto()
                        {
                            Identificador = oConcepto.NumeroParte,
                            Cantidad = oConcepto.Cantidad,
                            Unidad = oConcepto.Medida,
                            Descripcion = oConcepto.NombreParte,
                            ValorUnitario = oConcepto.PrecioUnitario,
                            Iva = oConcepto.Iva
                        });

                        mUnitarioTotal += oConcepto.PrecioUnitario;
                        mIvaTotal += oConcepto.Iva;
                    }
                }
            }
            else
            {
                foreach (var oConcepto in oListaVenta)
                {
                    oFacturaE.Conceptos.Add(new Concepto()
                    {
                        Identificador = oConcepto.NumeroDeParte,
                        Cantidad = oConcepto.Cantidad,
                        Unidad = oConcepto.UnidadDeMedida,
                        Descripcion = oConcepto.NombreDeParte,
                        ValorUnitario = oConcepto.PrecioUnitario,
                        Iva = oConcepto.Iva
                    });

                    mUnitarioTotal += oConcepto.PrecioUnitario;
                    mIvaTotal += oConcepto.Iva;
                }
            }

            // Se asigna el folio y la serie de la factura
            var oFolioFactura = VentasLoc.GenerarFolioDeFactura();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasLoc.FeEnviarFactura(ref oFacturaE);
            if (ResXml.Error)
            {
                // Se trata de regresar el folio de facturación apartado
                VentasLoc.RegresarFolioDeFacrtura(oFacturaE.Folio);
                return new ResAcc<int>(false, ResXml.Mensaje);
            }
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oVentaFactura = new VentaFactura()
            {
                Fecha = dAhora,
                FolioFiscal = oFacturaE.Timbre.FolioFiscal,
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                ClienteID = iClienteID,
                Observacion = sObservacion,
                Subtotal = mUnitarioTotal,
                Iva = mIvaTotal
            };
            var oVentaFacturaD = new List<VentaFacturaDetalle>();
            foreach (var oVentaFac in oVentas)
            {
                var oFacturaDet = new VentaFacturaDetalle() { VentaID = oVentaFac.VentaID };

                // Se revisa si es la primera factura para esta venta
                if (!Datos.Exists<VentaFacturaDetalle>(c => c.VentaID == oVentaFac.VentaID && c.Estatus))
                    oFacturaDet.Primera = true;

                oVentaFacturaD.Add(oFacturaDet);
            }
            Guardar.Factura(oVentaFactura, oVentaFacturaD);

            // Se escribe el folio de factura en cada venta
            string sSerieFolio = (oFacturaE.Serie + oFacturaE.Folio);
            foreach (int iVentaID in VentasIDs)
            {
                var oVenta = Datos.GetEntity<Venta>(q => q.VentaID == iVentaID && q.Estatus);
                oVenta.Facturada = true;
                oVenta.Folio = sSerieFolio;
                Datos.Guardar<Venta>(oVenta);
            }

            // Se manda guardar la factura, en pdf y xml. También se manda a salida
            var oRep = VentasLoc.FeGuardarArchivosFactura(ref oFacturaE, sCfdiTimbrado, sSerieFolio);

            // Se manda la factura por correo
            VentasLoc.EnviarFacturaPorCorreo(oVentaFactura.VentaFacturaID);

            return new ResAcc<int>(true, oVentaFactura.VentaFacturaID);
        }

        public static ResAcc<int> GenerarFacturaElectronica(List<int> VentasIDs, int iClienteID, string sObservacion)
        {
            return VentasLoc.GenerarFacturaElectronica(VentasIDs, iClienteID, null, null, sObservacion, null);
        }

        public static ResAcc<int> GenerarFacturaGlobal(string sConceptoVentas, decimal mCostoTotal, decimal mTotalVentas
            , string sConceptoCancelaciones, decimal mTotalCancelaciones, string sConceptoDevoluciones, decimal mTotalDevoluciones
            , List<VentasPagosDetalleView> oFormasDePago)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = TheosProc.Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasLoc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            int iClienteID = Cat.Clientes.Mostrador;
            DateTime dAhora = DateTime.Now;

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Ingreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = VentasLoc.GenerarMetodoDePagoFactura(oFormasDePago);
            // Se agraga la cadena del método de pago, como adicional
            oFacturaE.Adicionales.Add("LeyendaDePago", VentasLoc.GenerarCadenaMetodoDePagoFactura(oFormasDePago));

            // Se llenan los datos del receptor
            var ResRec = VentasLoc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se calcula el Iva
            decimal mVentasValor = UtilTheos.ObtenerPrecioSinIva(mTotalVentas);
            decimal mVentasIva = UtilTheos.ObtenerIvaDePrecio(mTotalVentas);
            decimal mCancelacionesValor = UtilTheos.ObtenerPrecioSinIva(mTotalCancelaciones);
            decimal mCancelacionesIva = UtilTheos.ObtenerIvaDePrecio(mTotalCancelaciones);
            decimal mDevolucionesValor = UtilTheos.ObtenerPrecioSinIva(mTotalDevoluciones);
            decimal mDevolucionesIva = UtilTheos.ObtenerIvaDePrecio(mTotalDevoluciones);

            // Se llenan los conceptos de la factura
            const string sUnidad = "U";
            oFacturaE.Conceptos = new List<Concepto>();
            // Ventas
            oFacturaE.Conceptos.Add(new Concepto()
            {
                Cantidad = 1,
                Unidad = sUnidad,
                Descripcion = sConceptoVentas,
                ValorUnitario = mVentasValor,
                Iva = mVentasIva
            });
            // Cancelaciones
            oFacturaE.Conceptos.Add(new Concepto()
            {
                Cantidad = 1,
                Unidad = sUnidad,
                Descripcion = sConceptoCancelaciones,
                ValorUnitario = (mCancelacionesValor * -1),
                Iva = (mCancelacionesIva * -1)
            });
            // Devoluciones
            oFacturaE.Conceptos.Add(new Concepto()
            {
                Cantidad = 1,
                Unidad = sUnidad,
                Descripcion = sConceptoDevoluciones,
                ValorUnitario = (mDevolucionesValor * -1),
                Iva = (mDevolucionesIva * -1)
            });

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasLoc.FeEnviarFactura(ref oFacturaE);
            if (ResXml.Error)
                return new ResAcc<int>(false, ResXml.Mensaje);
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oFolioFactura = VentasLoc.GenerarFolioDeFactura();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oVentaFactura = new VentaFactura()
            {
                Fecha = dAhora,
                FolioFiscal = oFacturaE.Timbre.FolioFiscal,
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                ClienteID = iClienteID,
                Costo = mCostoTotal,
                Subtotal = (mVentasValor - mCancelacionesValor - mDevolucionesValor),
                Iva = (mVentasIva - mCancelacionesIva - mDevolucionesIva)
            };
            var oVentaFacturaD = new List<VentaFacturaDetalle>();
            // No se agrega ningún detalle, para que las ventas aquí facturadas no aparezcan como facturadas
            // foreach (var oVenta in oVentas)
            //     oVentaFacturaD.Add(new VentaFacturaDetalle() { VentaID = oVenta.VentaID });
            Guardar.Factura(oVentaFactura, oVentaFacturaD);

            // Se manda guardar la factura, en pdf y xml
            var oRep = VentasLoc.FeGuardarArchivosFactura(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda imprimir la factura
            // ..

            return new ResAcc<int>(true, oVentaFactura.VentaFacturaID);
        }

        public static ResAcc<bool> ValidarDatosParaFactura(List<int> VentasIDs, int iClienteID)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = TheosProc.Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasLoc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtienen las ventas a facturar
            var oVentas = new List<VentasView>();

            foreach (int iVentaID in VentasIDs)
                oVentas.Add(Datos.GetEntity<VentasView>(q => q.VentaID == iVentaID));

            // Se llenan un dato "x" para simular el método de pago, pues no se tiene esa información todavía porque no se ha guardado la venta
            oFacturaE.MetodoDePago = "Efectivo";

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Ingreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();

            // Se llenan los datos del receptor
            var ResRec = VentasLoc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<bool>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            oFacturaE.Conceptos = new List<Concepto>();
            List<VentasDetalleView> oVentaDetalle;
            foreach (var oVenta in oVentas)
            {
                oVentaDetalle = Datos.GetListOf<VentasDetalleView>(q => q.VentaID == oVenta.VentaID);
                foreach (var oConcepto in oVentaDetalle)
                {
                    oFacturaE.Conceptos.Add(new Concepto()
                    {
                        Identificador = oConcepto.ParteID.ToString(),
                        Cantidad = oConcepto.Cantidad,
                        Unidad = oConcepto.Medida,
                        Descripcion = oConcepto.NombreParte,
                        ValorUnitario = oConcepto.PrecioUnitario,
                        Iva = oConcepto.Iva
                    });
                }
            }

            // Se genera el Xml inicial, con el formato que pide el Sat
            var ResXml = oFacturaE.GenerarFactura(true);
            if (ResXml.Error)
                return new ResAcc<bool>(false, ResXml.Mensaje);

            return new ResAcc<bool>(true, true);
        }

        public static ResAcc<bool> ValidarDatosParaFactura(int iVentaID, int iClienteID)
        {
            return VentasLoc.ValidarDatosParaFactura(new List<int>() { iVentaID }, iClienteID);
        }

        public static ResAcc<int> GenerarFacturaCancelacion(int iFacturaID, List<int> oIdsDevoluciones)
        {
            // Se obtiene el folio fiscal
            var oFactura = Datos.GetEntity<VentaFactura>(c => c.VentaFacturaID == iFacturaID && c.Estatus);
            string sFolioFiscal = oFactura.FolioFiscal;

            // Se generan los datos de la cancelación
            DateTime dAhora = DateTime.Now;
            var oFacturaDevolucion = new VentaFacturaDevolucion()
            {
                VentaFacturaID = iFacturaID,
                Fecha = dAhora,
                EsCancelacion = true
            };
            // Se genera el detalle de la devolución de factura, con los Ids de las devoluciones incluidas
            var oFacturaDevDet = new List<VentaFacturaDevolucionDetalle>();
            foreach (int iDevolucionID in oIdsDevoluciones)
                oFacturaDevDet.Add(new VentaFacturaDevolucionDetalle() { VentaDevolucionID = iDevolucionID });
            Guardar.FacturaDevolucion(oFacturaDevolucion, oFacturaDevDet);

            // Se manda cancelar la factura, y completar los procesos correspondientes
            var ResCanc = VentasLoc.GenerarFacturaCancelacion(sFolioFiscal, oFacturaDevolucion.VentaFacturaDevolucionID);

            /* Ya no se sale, pues aunque haya error, se deben guardar los datos, ya que la venta sí se canceló
            if (ResC.Error)
                return new ResAcc<bool>(false, ResC.Mensaje);
            */

            var Res = new ResAcc<int>(ResCanc.Exito, ResCanc.Mensaje);
            Res.Respuesta = oFacturaDevolucion.VentaFacturaDevolucionID;

            return Res;
        }

        public static ResAcc<bool> GenerarFacturaCancelacion(string sFolioFiscal, int iVentaFacturaDevolucionID)
        {
            // Se manda hacer la cancelación
            var oFacturaE = new FacturaElectronica();
            var oConfig = TheosProc.Config.ValoresVarios("Facturacion.");
            VentasLoc.FeLLenarConfiguracion(ref oFacturaE, oConfig);
            VentasLoc.FeLlenarDatosComunes(ref oFacturaE, oConfig);
            var ResC = oFacturaE.CancelarFactura(sFolioFiscal, !GlobalClass.Produccion);
            bool bFacturada = ResC.Exito;

            // Se guardan datos del resultado de la cancelación ante Sat, si se canceló
            if (bFacturada)
            {
                // Se obtiene el objeto de la cancelacion
                var oFacturaDevolucion = Datos.GetEntity<VentaFacturaDevolucion>(q => q.VentaFacturaDevolucionID == iVentaFacturaDevolucionID);

                oFacturaDevolucion.Ack = ResC.Respuesta.Izquierda(36);
                oFacturaDevolucion.Procesada = true;
                oFacturaDevolucion.FechaProcesada = DateTime.Now;
                Datos.Guardar<VentaFacturaDevolucion>(oFacturaDevolucion);

                // Se le cambia el estatus a la factura
                var oFactura = Datos.GetEntity<VentaFactura>(q => q.VentaFacturaID == oFacturaDevolucion.VentaFacturaID && q.Estatus);
                oFactura.EstatusGenericoID = Cat.EstatusGenericos.Cancelada;
                Datos.Guardar<VentaFactura>(oFactura);

                // Se manda la notificación por correo, si fue exitosa
                VentasLoc.EnviarFacturaCancelacionPorCorreo(oFacturaDevolucion.VentaFacturaID);
            }

            return new ResAcc<bool>(bFacturada, ResC.Mensaje);
        }
                
        public static string GenerarMetodoDePagoFactura(List<VentasPagosDetalleView> oFormasDePago)
        {
            string sMetodo = "";

            if (oFormasDePago != null && oFormasDePago.Count > 0)
            {
                oFormasDePago = oFormasDePago.OrderByDescending(c => c.Importe).ToList();
                foreach (var oReg in oFormasDePago)
                {
                    switch (oReg.FormaDePagoID)
                    {
                        case Cat.FormasDePago.Efectivo:
                            sMetodo += ("," + CatFe.MetodosDePago.Efectivo);
                            break;
                        case Cat.FormasDePago.Cheque:
                            sMetodo += ("," + CatFe.MetodosDePago.Cheque);
                            break;
                        case Cat.FormasDePago.Tarjeta:
                            sMetodo += ("," + CatFe.MetodosDePago.TarjetaDeCredito);
                            break;
                        case Cat.FormasDePago.TarjetaDeDebito:
                            sMetodo += ("," + CatFe.MetodosDePago.TarjetaDeDebito);
                            break;
                        case Cat.FormasDePago.Transferencia:
                            sMetodo += ("," + CatFe.MetodosDePago.Transferencia);
                            break;
                    }
                }
            }

            sMetodo = (sMetodo == "" ? CatFe.MetodosDePago.Otros : sMetodo.Substring(1));
            return sMetodo;
        }

        public static string GenerarCadenaMetodoDePagoFactura(List<VentasPagosDetalleView> oFormasDePago)
        {
            string sMetodo = "";

            if (oFormasDePago != null && oFormasDePago.Count > 0)
            {
                oFormasDePago = oFormasDePago.OrderByDescending(c => c.Importe).ToList();
                foreach (var oReg in oFormasDePago)
                    sMetodo += ("," + oReg.FormaDePago);
            }

            sMetodo = (sMetodo == "" ? "" : sMetodo.Substring(1));
            return sMetodo;
        }

        public static string ObtenerCuentaDePago(List<VentasPagosDetalleView> oFormasDePago)
        {
            if (oFormasDePago == null)
                return "";

            string sCuenta = "";
            foreach (var oReg in oFormasDePago)
            {
                if (oReg.Folio != "")
                {
                    sCuenta = oReg.Folio;
                    break;
                }
            }
            return sCuenta;
        }

        /*
        public static ResAcc<int> GenerarFacturaDevolucion(int iDevolucionID, int iUsuarioID)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasProc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;
            var oDev = General.GetEntity<VentasDevolucionesView>(q => q.VentaDevolucionID == iDevolucionID);

            // Se obtiene el nombre del vendedor
            var oUsuario = General.GetEntity<Usuario>(q => q.UsuarioID == iUsuarioID && q.Estatus);
            string sVendedores = oUsuario.NombrePersona;
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Egreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = oDev.FormaDePago;

            // Se llenan los datos del receptor
            var oVenta = General.GetEntity<Venta>(q => q.VentaID == oDev.VentaID && q.Estatus);
            int iClienteID = oVenta.ClienteID;
            var ResRec = VentasProc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            var oDevDetalle = General.GetListOf<VentasDevolucionesDetalleView>(q => q.VentaDevolucionID == oDev.VentaDevolucionID);
            oFacturaE.Conceptos = new List<Concepto>();
            foreach (var oDet in oDevDetalle)
            {
                oFacturaE.Conceptos.Add(new Concepto()
                {
                    Identificador = oDet.NumeroParte,
                    Cantidad = oDet.Cantidad,
                    Unidad = oDet.NombreMedida,
                    Descripcion = oDet.NombreParte,
                    ValorUnitario = oDet.PrecioUnitario,
                    Iva = oDet.Iva
                });
            }

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasProc.FeEnviarFactura(ref oFacturaE);
            bool bFacturada = ResXml.Exito;
            // if (ResXml.Error)
            //    return new ResAcc<int>(false, ResXml.Mensaje);
            
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oVentaFactura = General.GetEntity<VentasFacturasDetalleView>(q => q.VentaID == oVenta.VentaID);
            var oFolioFactura = VentasProc.GenerarFolioDeFacturaDevolucion();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oFacturaDevolucion = new VentaFacturaDevolucion()
            {
                VentaFacturaID = oVentaFactura.VentaFacturaID,
                Fecha = dAhora,
                FolioFiscal = (oFacturaE.Timbre == null ? "" : oFacturaE.Timbre.FolioFiscal),
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                EsCancelacion = false,
                Procesada = bFacturada,
                FechaProcesada = (bFacturada ? ((DateTime?)dAhora) : null)
            };
            var oFacturaDevDet = new List<VentaFacturaDevolucionDetalle>();
            oFacturaDevDet.Add(new VentaFacturaDevolucionDetalle() { VentaDevolucionID = iDevolucionID });
            Guardar.FacturaDevolucion(oFacturaDevolucion, oFacturaDevDet);

            //
            oFacturaE.Adicionales.Add("FacturaOrigen", (oVentaFactura.Serie + oVentaFactura.Folio));

            // Se manda guardar la factura, en pdf y xml
            if (bFacturada)
                VentasProc.FeGuardarArchivosFacturaDevolucion(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda la nota de crédito generada, por correo
            VentasProc.EnviarFacturaDevolucionPorCorreo(oFacturaDevolucion.VentaFacturaDevolucionID);

            // Se manda imprimir la factura
            // ..

            return new ResAcc<int>(bFacturada, oFacturaDevolucion.VentaFacturaDevolucionID);
        }
        */

        public static ResAcc<int> GenerarFacturaDevolucion(List<VentasPagosDetalleView> oFormasDePago, int iVentaID, List<ProductoVenta> oDetalle, int iUsuarioID
            , bool bEsDevolucion, int iId)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = TheosProc.Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasLoc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtiene el nombre del vendedor
            var oUsuario = Datos.GetEntity<Usuario>(q => q.UsuarioID == iUsuarioID && q.Estatus);
            string sVendedores = oUsuario.NombrePersona;
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Egreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = VentasLoc.GenerarMetodoDePagoFactura(oFormasDePago);
            // Se agraga la cadena del método de pago, como adicional
            oFacturaE.Adicionales.Add("LeyendaDePago", VentasLoc.GenerarCadenaMetodoDePagoFactura(oFormasDePago));

            // Se llenan los datos del receptor
            // var oVenta = General.GetEntity<Venta>(q => q.VentaID == iVentaID && q.Estatus);
            var oVentaFactV = Datos.GetEntity<VentasFacturasDetalleAvanzadoView>(c => c.VentaID == iVentaID);
            int iClienteID = oVentaFactV.ClienteID.Valor();
            var ResRec = VentasLoc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se llenan los conceptos de la factura
            decimal mUnitarioTotal = 0, mIvaTotal = 0;
            oFacturaE.Conceptos = new List<Concepto>();
            foreach (var oDet in oDetalle)
            {
                oFacturaE.Conceptos.Add(new Concepto()
                {
                    Identificador = oDet.NumeroDeParte,
                    Cantidad = oDet.Cantidad,
                    Unidad = oDet.UnidadDeMedida,
                    Descripcion = oDet.NombreDeParte,
                    ValorUnitario = oDet.PrecioUnitario,
                    Iva = oDet.Iva
                });

                mUnitarioTotal += oDet.PrecioUnitario;
                mIvaTotal += oDet.Iva;
            }

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasLoc.FeEnviarFactura(ref oFacturaE);
            bool bFacturada = ResXml.Exito;
            /* if (ResXml.Error)
                return new ResAcc<int>(false, ResXml.Mensaje);
            */
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            // var oVentaFactura = General.GetEntity<VentasFacturasDetalleAvanzadoView>(q => q.VentaID == iVentaID);
            var oFolioFactura = VentasLoc.GenerarFolioDeFacturaDevolucion();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oFacturaDevolucion = new VentaFacturaDevolucion()
            {
                VentaFacturaID = oVentaFactV.VentaFacturaID.Valor(),
                Fecha = dAhora,
                FolioFiscal = (oFacturaE.Timbre == null ? "" : oFacturaE.Timbre.FolioFiscal),
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                EsCancelacion = false,
                Procesada = bFacturada,
                FechaProcesada = (bFacturada ? ((DateTime?)dAhora) : null),
                Subtotal = mUnitarioTotal,
                Iva = mIvaTotal
            };
            var oFacturaDevDet = new List<VentaFacturaDevolucionDetalle>();
            var oRegFacDevDet = new VentaFacturaDevolucionDetalle();
            if (bEsDevolucion)
                oRegFacDevDet.VentaDevolucionID = iId;
            else
                oRegFacDevDet.VentaGarantiaID = iId;
            oFacturaDevDet.Add(oRegFacDevDet);
            Guardar.FacturaDevolucion(oFacturaDevolucion, oFacturaDevDet);

            // Quizá sea buena opción marcar (de alguna forma) el registro de VentaFacturaDetalle como cancelada, si es que fue una cancelación de la Venta

            //
            oFacturaE.Adicionales.Add("FacturaOrigen", (oVentaFactV.Serie + oVentaFactV.Folio));

            // Se manda guardar la factura, en pdf y xml
            if (bFacturada)
                VentasLoc.FeGuardarArchivosFacturaDevolucion(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda la nota de crédito generada, por correo
            VentasLoc.EnviarFacturaDevolucionPorCorreo(oFacturaDevolucion.VentaFacturaDevolucionID);

            // Se manda imprimir la factura
            // ..

            return new ResAcc<int>(bFacturada, oFacturaDevolucion.VentaFacturaDevolucionID);
        }

        public static ResAcc<int> GenerarFacturaDevolucionPorDevolucion(int iDevolucionID)
        {
            var oDev = Datos.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iDevolucionID);
            var oDevDet = Datos.GetListOf<VentasDevolucionesDetalleView>(c => c.VentaDevolucionID == iDevolucionID);
            var oPartesDev = new List<ProductoVenta>();
            foreach (var oReg in oDevDet)
            {
                oPartesDev.Add(new ProductoVenta()
                {
                    NumeroDeParte = oReg.NumeroParte,
                    NombreDeParte = oReg.NombreParte,
                    UnidadDeMedida = oReg.NombreMedida,
                    Cantidad = oReg.Cantidad,
                    PrecioUnitario = oReg.PrecioUnitario,
                    Iva = oReg.Iva
                });
            }

            // Se genera el objeto con la forma de pago
            var oFormaDePago = new List<VentasPagosDetalleView>() { new VentasPagosDetalleView() { FormaDePagoID = oDev.FormaDePagoID, FormaDePago = oDev.FormaDePago } };
            
            return VentasLoc.GenerarFacturaDevolucion(oFormaDePago, oDev.VentaID, oPartesDev, oDev.RealizoUsuarioID, true, oDev.VentaDevolucionID);
        }

        public static ResAcc<int> GenerarFacturaDevolucionPorGarantia(int iGarantiaID)
        {
            var oGarantia = Datos.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == iGarantiaID);
            var oPartesDev = new List<ProductoVenta>();
            oPartesDev.Add(new ProductoVenta()
            {
                NumeroDeParte = oGarantia.NumeroDeParte,
                NombreDeParte = oGarantia.NombreDeParte,
                UnidadDeMedida = oGarantia.Medida,
                Cantidad = 1,
                PrecioUnitario = oGarantia.PrecioUnitario,
                Iva = oGarantia.Iva
            });

            // Se genera el objeto con la forma de pago
            int iFormaDePagoID = UtilDatos.FormaDePagoDeAccionGarantia(oGarantia.AccionID);
            var oFormaDePago = new List<VentasPagosDetalleView>() { new VentasPagosDetalleView() { FormaDePagoID = iFormaDePagoID, FormaDePago = oGarantia.Accion } };

            return VentasLoc.GenerarFacturaDevolucion(oFormaDePago, oGarantia.VentaID, oPartesDev, oGarantia.RealizoUsuarioID, false, oGarantia.VentaGarantiaID);
        }

        public static ResAcc<int> GenerarNotaDeCreditoFiscal(List<ProductoVenta> oDetalle, int iClienteID, int iUsuarioID)
        {
            // Se crea la instancia de la clase de Facturación Electrónica
            var oFacturaE = new FacturaElectronica();
            var oConfig = TheosProc.Config.ValoresVarios("Facturacion.");
            // Se llenan los valores de configuración y los datos del emisor
            VentasLoc.FeLlenarDatosComunes(ref oFacturaE, oConfig);

            // Se llenan los datos generales de la factura
            DateTime dAhora = DateTime.Now;

            // Se obtiene el nombre del vendedor
            var oUsuario = Datos.GetEntity<Usuario>(q => q.UsuarioID == iUsuarioID && q.Estatus);
            string sVendedores = oUsuario.NombrePersona;
            oFacturaE.Adicionales = new Dictionary<string, string>();
            oFacturaE.Adicionales.Add("Vendedor", sVendedores);

            oFacturaE.Fecha = dAhora;
            oFacturaE.FormaDePago = "Una sola exhibición";
            oFacturaE.LugarDeExpedicion = string.Format("{0}, {1}", oConfig["Facturacion.Municipio"], oConfig["Facturacion.Estado"]);
            oFacturaE.TipoDeComprobante = Enumerados.TiposDeComprobante.Egreso;
            oFacturaE.TasaDeImpuesto = GlobalClass.ConfiguracionGlobal.IVA.ToString();
            oFacturaE.MetodoDePago = CatFe.MetodosDePago.Otros;

            // Se llenan los datos del receptor
            var ResRec = VentasLoc.FeLlenarDatosReceptor(ref oFacturaE, iClienteID);
            if (ResRec.Error)
                return new ResAcc<int>(false, ResRec.Mensaje);

            // Se agregan datos adicionales
            oFacturaE.Adicionales.Add("FacturaOrigen", "");

            // Se llenan los conceptos de la factura
            decimal mSubtotal = 0, mIva = 0;
            oFacturaE.Conceptos = new List<Concepto>();
            foreach (var oDet in oDetalle)
            {
                oFacturaE.Conceptos.Add(new Concepto()
                {
                    Identificador = oDet.NumeroDeParte,
                    Cantidad = oDet.Cantidad,
                    Unidad = oDet.UnidadDeMedida,
                    Descripcion = oDet.NombreDeParte,
                    ValorUnitario = oDet.PrecioUnitario,
                    Iva = oDet.Iva
                });
                mSubtotal += oDet.PrecioUnitario;
                mIva += oDet.Iva;
            }

            // Se comienza a procesar la facturación electrónica

            // Se envía la factura y se obtiene el Xml generado
            var ResXml = VentasLoc.FeEnviarFactura(ref oFacturaE);
            bool bFacturada = ResXml.Exito;
            /* if (ResXml.Error)
                return new ResAcc<int>(false, ResXml.Mensaje);
            */
            string sCfdiTimbrado = ResXml.Respuesta;

            // Se guarda la información
            var oFolioFactura = VentasLoc.GenerarFolioDeFacturaDevolucion();
            oFacturaE.Serie = oFolioFactura["Serie"];
            oFacturaE.Folio = oFolioFactura["Folio"];
            var oNota = new NotaDeCreditoFiscal()
            {
                Fecha = dAhora,
                ClienteID = iClienteID,
                SucursalID = GlobalClass.SucursalID,
                FolioFiscal = (oFacturaE.Timbre == null ? "" : oFacturaE.Timbre.FolioFiscal),
                Serie = oFacturaE.Serie,
                Folio = oFacturaE.Folio,
                Subtotal = mSubtotal,
                Iva = mIva,
                RealizoUsuarioID = iUsuarioID
            };
            Datos.Guardar<NotaDeCreditoFiscal>(oNota);
            // Se guarda el detalle de la Nota de Crédito
            foreach (var oReg in oDetalle)
            {
                int iVentaID = Util.Entero(oReg.NumeroDeParte);
                if (iVentaID <= 0) continue;
                Datos.Guardar<NotaDeCreditoFiscalDetalle>(new NotaDeCreditoFiscalDetalle()
                {
                    NotaDeCreditoFiscalID = oNota.NotaDeCreditoFiscalID,
                    VentaID = iVentaID,
                    Descuento = oReg.PrecioUnitario,
                    IvaDescuento = oReg.Iva
                });
            }

            // Se manda guardar la factura, en pdf y xml
            if (bFacturada)
                VentasLoc.FeGuardarArchivosFacturaDevolucion(ref oFacturaE, sCfdiTimbrado, (oFacturaE.Serie + oFacturaE.Folio));

            // Se manda la nota de crédito generada, por correo
            // VentasProc.EnviarFacturaDevolucionPorCorreo(oFacturaDevolucion.VentaFacturaDevolucionID);

            // Se manda imprimir la factura
            // ..

            var oRes = new ResAcc<int>(bFacturada, oNota.NotaDeCreditoFiscalID);
            if (!bFacturada)
                oRes.Mensaje = ResXml.Mensaje;
            return oRes;
        }

        #endregion

        #region [ Clientes ]

        public static int ObtenerClienteID(string sMensaje, bool bVentasMostrador)
        {
            int iClienteID = 0;
            var frmValor = new MensajeObtenerValor(sMensaje, "", MensajeObtenerValor.Tipo.Combo);
            frmValor.CargarCombo("ClienteID", "Nombre", Datos.GetListOf<Cliente>(c => (bVentasMostrador || c.ClienteID != Cat.Clientes.Mostrador) && c.Estatus));
            if (frmValor.ShowDialog(Principal.Instance) == DialogResult.OK)
                iClienteID = Util.Entero(frmValor.Valor);
            frmValor.Dispose();
            return iClienteID;
        }

        #endregion

        public static void MostrarAvisoDevolucionFacturaCreditoAnt(int iDevolucionID)
        {
            if (Principal.Instance.InvokeRequired)
            {
                Principal.Instance.Invoke(new Action<int>(VentasLoc.MostrarAvisoDevolucionFacturaCreditoAnt), iDevolucionID);
            }
            else
            {
                var oDevV = Datos.GetEntity<VentasDevolucionesView>(c => c.VentaDevolucionID == iDevolucionID);
                AvisoDevolucionFacturaCreditoAnt.Instance.AgregarTexto("Cliente: " + oDevV.Cliente);
                AvisoDevolucionFacturaCreditoAnt.Instance.AgregarTexto("Folio: " + oDevV.FolioDeVenta);
                AvisoDevolucionFacturaCreditoAnt.Instance.AgregarTexto("Motivo: " + oDevV.Motivo);
                AvisoDevolucionFacturaCreditoAnt.Instance.AgregarTexto("Observación: " + oDevV.Observacion);
                AvisoDevolucionFacturaCreditoAnt.Instance.AgregarTexto("Acción: " + oDevV.FormaDePago);
                AvisoDevolucionFacturaCreditoAnt.Instance.AgregarTexto("Usuario: " + oDevV.Realizo);
                AvisoDevolucionFacturaCreditoAnt.Instance.Show();
            }
        }

        #region [ Procesos independientes ]

        public static void ActualizarBarraDeMetas(object state)
        {
            try
            {
                // Se obtiene la utilidad meta
                var oMetaSucursal = Datos.GetEntity<MetaSucursal>(c => c.SucursalID == GlobalClass.SucursalID);
                // Se obtiene la utilidad acumulada
                var oParams = new Dictionary<string, object>();
                var oSemana = UtilDatos.FechasDeComisiones(DateTime.Now);
                oParams.Add("Desde", oSemana.Valor1);
                oParams.Add("Hasta", oSemana.Valor2);
                var oDatos = Datos.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado", oParams);
                decimal mUtilidad = oDatos.Where(c => c.SucursalID == GlobalClass.SucursalID).Sum(c => c.Utilidad).Valor();
                // Se llena la barra
                Ventas.Instance.Invoke(new Action(() =>
                {
                    if (mUtilidad < 0)
                    {
                        UtilLocal.MensajeAdvertencia("La Utilidad es menor que cero.");
                        return;
                    }
                    var oBarra = (Ventas.Instance.Controls["pnlBarraVentas"].Controls["pgbMetas"] as ProgressBar);
                    oBarra.Maximum = (int)oMetaSucursal.UtilSucursal;
                    oBarra.Value = (int)(mUtilidad > oMetaSucursal.UtilSucursal ? oMetaSucursal.UtilSucursal : mUtilidad);
                }));

                // Se programa la siguiente ejecución
                // Se obtiene el tiempo necesario para la llamada
                int iSegundos = Util.Entero(TheosProc.Config.Valor("Ventas.BarraDeMetas.SegundosActualizacion"));
                if (Program.oTimers.ContainsKey("BarraDeMetas"))
                {
                    Program.oTimers["BarraDeMetas"].Change((iSegundos * 1000), System.Threading.Timeout.Infinite);
                }
                else
                {
                    Program.oTimers.Add("BarraDeMetas", new System.Threading.Timer(new System.Threading.TimerCallback(VentasLoc.ActualizarBarraDeMetas), null
                        , (iSegundos * 1000), System.Threading.Timeout.Infinite));
                }
            }
            catch (Exception e)
            {
                UtilLocal.MensajeError("Error no controlado al ejecutar actualización de Barra de Metas.\n\n" + e.Message);
            }
        }

        #endregion

        #region [ 9500 ]

        public static void NotificarCreacion9500()
        {
            var oUsuarios = Datos.GetListOf<Usuario>(c => c.Alerta9500 == true && c.Estatus);
            foreach (var oReg in oUsuarios)
                Proc.EnviarMensajeTcp(oReg.Ip, Proc.MensajesTcp.Alerta9500, (DateTime.Now.ToShortTimeString() + ": Se acaba de crear un 9500. Revisar."));
        }

        #endregion
    }
}
