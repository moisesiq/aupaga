using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml;
using System.Threading;

using LibUtil;

namespace FacturacionElectronica
{
    public class ConSat
    {
        const string UrlInicio = "https://cfdiau.sat.gob.mx/nidp/app/login?id=SATUPCFDiCon&sid=0&option=credential&sid=0";
        const string UrlMed = "https://cfdiau.sat.gob.mx/nidp/app?sid=0";
        const string UrlErrorIn = "https://cfdiau.sat.gob.mx/nidp/app/login?sid=0&sid=0";
        const string UrlEmitidas = "https://portalcfdi.facturaelectronica.sat.gob.mx/ConsultaEmisor.aspx";
        const string UrlRecibidas = "https://portalcfdi.facturaelectronica.sat.gob.mx/ConsultaReceptor.aspx";
        const string UrlFinBusqueda = "about:blank?paso=b";
        const string UrlFinXmls = "about:blank?paso=x";
        const string PaginaDescarga = "RecuperaCfdi.aspx";

        enum TipoDeConsulta { Emitidas, Recibidas };
        TipoDeConsulta eTipoDeConsulta;
        WebBrowser webSat;
        DateTime dDesde;
        DateTime dHasta;
        int iAnio;
        int iMes;
        int? iDia;
        Thread oHiloSat;
        bool bDescargando;
        int iXmlProc;

        public ConSat(string sRfc, string sClaveCiec)
        {
            this.Rfc = sRfc;
            this.ClaveCiec = sClaveCiec;

            this.PasoCompletado += ConSat_PasoCompletado;
            
            this.oHiloSat = new Thread(this.DescargaFacturaAsinc);
        }

        #region [ Dll imports ]

        /// <summary>
        /// The URLMON library contains this function, URLDownloadToFile, which is a way
        /// to download files without user prompts.  The ExecWB( _SAVEAS ) function always
        /// prompts the user, even if _DONTPROMPTUSER parameter is specified, for "internet
        /// security reasons".  This function gets around those reasons.
        /// </summary>
        /// <param name="callerPointer">Pointer to caller object (AX).</param>
        /// <param name="url">String of the URL.</param>
        /// <param name="filePathWithName">String of the destination filename/path.</param>
        /// <param name="reserved">[reserved].</param>
        /// <param name="callBack">A callback function to monitor progress or abort.</param>
        /// <returns>0 for okay.</returns>
        /// source: http://www.pinvoke.net/default.aspx/urlmon/URLDownloadToFile%20.html
        [DllImport("urlmon.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Int32 URLDownloadToFile(
            [MarshalAs(UnmanagedType.IUnknown)] object callerPointer,
            [MarshalAs(UnmanagedType.LPWStr)] string url,
            [MarshalAs(UnmanagedType.LPWStr)] string filePathWithName,
            Int32 reserved,
            IntPtr callBack);


        /// <summary>
        /// Download a file from the webpage and save it to the destination without promting the user
        /// </summary>
        /// <param name="url">the url with the file</param>
        /// <param name="destinationFullPathWithName">the absolut full path with the filename as destination</param>
        /// <returns></returns>
        public FileInfo DownloadFile(string url, string destinationFullPathWithName)
        {
            URLDownloadToFile(null, url, destinationFullPathWithName, 0, IntPtr.Zero);
            return new FileInfo(destinationFullPathWithName);
        }

        #endregion

        #region [ Propiedades / Eventos ]

        // Propiedades
        public string Rfc { get; set; }
        public string ClaveCiec { get; set; }
        public string[] Xmls { get; set; }

        public enum ConSatPaso { IniciandoSesion, SesionIniciada, BuscandoEmitidas, BuscandoRecibidas, BusquedaCompletada
            , ObteniendoXmls, IniciandoDescarga, XmlDescargado, DescargaCompletada };

        /// <summary>
        /// {_RfcEmisor}
        /// {_RfcReceptor}
        /// {_Emisor}
        /// {_Receptor}
        /// {_Anio}
        /// {_Mes}
        /// {_Dia}
        /// {_Serie}
        /// {_Folio}
        /// {_Uuid}
        /// </summary>
        public string RutaGuardar { get; set; }

        // Eventos
        public event Action<ConSatPaso> PasoCompletado;

        #endregion

        #region [ Eventos ]

        private void webSat_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().Contains(ConSat.PaginaDescarga))
            {
                e.Cancel = true;
                // Se manda descargar la factura, de forma asíncrona
                // this.oHiloDescarga.Start();
                this.DescargarFactura(e.Url.ToString());
            }
        }

        private void webSat_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Si se está descargando, no se mete a este código
            if (this.bDescargando)
                return;

            string sUrl = e.Url.ToString();
            switch (sUrl)
            {
                case ConSat.UrlInicio:
                    string sJs = File.ReadAllText(this.RutaScriptConSat());
                    sJs += string.Format("\n\nwindow.IniciarSesion('{0}', '{1}');", this.Rfc, this.ClaveCiec);
                    this.InsertarScript(sJs);
                    this.ReportarPaso(ConSat.ConSatPaso.IniciandoSesion);
                    break;
                case ConSat.UrlErrorIn:

                    break;
                case ConSat.UrlMed:
                    switch (this.eTipoDeConsulta)
                    {
                        case ConSat.TipoDeConsulta.Emitidas:
                            this.webSat.Navigate(ConSat.UrlEmitidas);
                            break;
                        case ConSat.TipoDeConsulta.Recibidas:
                            this.webSat.Navigate(ConSat.UrlRecibidas);
                            break;
                    }
                    this.ReportarPaso(ConSat.ConSatPaso.SesionIniciada);
                    break;
                case ConSat.UrlEmitidas:
                    string sJs2 = File.ReadAllText(this.RutaScriptConSat());
                    sJs2 += string.Format("\n\nwindow.BuscarEmitidasPorFecha('{0}', '{1}');", this.dDesde.ToString("dd/MM/yyyy"), this.dHasta.ToString("dd/MM/yyyy"));
                    this.InsertarScript(sJs2);
                    this.ReportarPaso(ConSat.ConSatPaso.BuscandoEmitidas);
                    break;
                case ConSat.UrlRecibidas:
                    string sJs3 = File.ReadAllText(this.RutaScriptConSat());
                    sJs3 += string.Format("\n\nwindow.BuscarRecibidasPorFecha({0}, {1}, {2});", this.iAnio, this.iMes, (this.iDia.HasValue ? this.iDia.ToString() : "null"));
                    this.InsertarScript(sJs3);
                    this.ReportarPaso(ConSat.ConSatPaso.BuscandoRecibidas);
                    break;
            }
        }

        private void webSat_NewWindow(object sender, CancelEventArgs e)
        {
            switch (this.webSat.DocumentTitle)
            {
                case "b":
                    this.ReportarPaso(ConSat.ConSatPaso.BusquedaCompletada);
                    break;
                case "x":
                    // Checar el document, a ver si si tiene las facturas
                    string sXmls = this.webSat.Document.GetElementById("preXmls").InnerText;
                    this.Xmls = sXmls.Split('\n');
                    this.IniciarDescarga();
                    this.ReportarPaso(ConSat.ConSatPaso.IniciandoDescarga);
                    // Se manda cerrar la sesión
                    // this.InsertarScript("window.CerrarSesion();");
                    break;
            }
            e.Cancel = true;
        }

        private void ConSat_PasoCompletado(ConSat.ConSatPaso ePaso)
        {
            if (ePaso == ConSat.ConSatPaso.XmlDescargado)
                this.IniciarDescarga();
        }

        #endregion

        #region [ Privados ]

        private string RutaScriptConSat()
        {
            return UtilFe.RutaFe("ConSat.js");
        }

        private void InsertarScript(string sScript)
        {
            var oJs = this.webSat.Document.CreateElement("script");
            oJs.SetAttribute("type", "text/javascript");
            oJs.SetAttribute("text", sScript);
            this.webSat.Document.GetElementsByTagName("head")[0].AppendChild(oJs);
        }

        private void ReportarPaso(ConSat.ConSatPaso ePaso)
        {
            if (this.PasoCompletado != null)
                this.PasoCompletado.Invoke(ePaso);
        }

        private void IniciarDescarga()
        {
            if (this.iXmlProc >= this.Xmls.Length)
            {
                // Se espera un breve tiempo, para dejar que se terminen de bajar todas las facturas
                Thread.Sleep(1000);
                //
                this.bDescargando = false;
                this.iXmlProc = 0;
                this.ReportarPaso(ConSat.ConSatPaso.DescargaCompletada);
            }
            else
            {
                this.bDescargando = true;
                this.webSat.Navigate(this.Xmls[this.iXmlProc++]);
            }
        }

        private void DescargaFacturaAsinc(object oUrlXml)
        {
            this.DescargarFactura(Util.Cadena(oUrlXml));
        }

        private void DescargarFactura(string sUrlXml)
        {
            // Se descarga la factura a una archivo temporal
            string sTemp = Path.GetTempFileName();
            this.DownloadFile(sUrlXml, sTemp);

            // Se leen los datos principales del xml
            var oXml = new XmlDocument();
            oXml.Load(sTemp);
            var oXmlNs = new XmlNamespaceManager(oXml.NameTable);
            oXmlNs.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
            oXmlNs.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            string sRfcEmisor = this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante/cfdi:Emisor", "rfc");
            string sEmisor = this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante/cfdi:Emisor", "nombre");
            string sRfcReceptor = this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante/cfdi:Receptor", "rfc");
            string sReceptor = this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante/cfdi:Receptor", "nombre");
            DateTime dFecha = Util.FechaHora(this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante", "fecha"));
            string sSerie = this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante", "serie");
            string sFolio = this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante", "folio");
            string sUuid = this.XmlValorDeAtributo(oXml, oXmlNs, "/cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital", "UUID");

            // Se obtiene la ruta donde guardar
            string sGuardar = this.RutaGuardar;
            sGuardar = sGuardar.Replace("{_RfcEmisor}", sRfcEmisor);
            sGuardar = sGuardar.Replace("{_Emisor}", sEmisor);
            sGuardar = sGuardar.Replace("{_RfcReceptor}", sRfcReceptor);
            sGuardar = sGuardar.Replace("{_Receptor}", sReceptor);
            sGuardar = sGuardar.Replace("{_Anio}", dFecha.Year.ToString());
            sGuardar = sGuardar.Replace("{_Mes}", dFecha.Month.ToString());
            sGuardar = sGuardar.Replace("{_Dia}", dFecha.Day.ToString());
            sGuardar = sGuardar.Replace("{_Serie}", sSerie);
            sGuardar = sGuardar.Replace("{_Folio}", sFolio);
            sGuardar = sGuardar.Replace("{_Uuid}", sUuid);

            // Se manda guardar el archivo (se mueve)
            Directory.CreateDirectory(Path.GetDirectoryName(sGuardar));
            File.Delete(sGuardar);
            File.Move(sTemp, sGuardar);

            // Se manda notificar la descarga, para reportar progreso y seguir bajando lo que falte
            this.PasoCompletado.Invoke(ConSat.ConSatPaso.XmlDescargado);
        }

        private string XmlValorDeAtributo(XmlDocument oXml, XmlNamespaceManager oXmlNs, string sXpath, string sAtributo)
        {
            var oNodo = oXml.SelectSingleNode(sXpath, oXmlNs);
            if (oNodo == null)
                return "";
            var oAtributo = oNodo.Attributes[sAtributo];
            if (oAtributo == null)
                return "";

            return oAtributo.Value;
        }

        #endregion

        #region [ Públicos ]

        public void IniciarFacturasEmitidas(DateTime dDesde, DateTime dHasta)
        {
            this.dDesde = dDesde;
            this.dHasta = dHasta;
            this.eTipoDeConsulta = ConSat.TipoDeConsulta.Emitidas;
            this.webSat.Navigate(ConSat.UrlInicio);
        }

        public void IniciarFacturasRecibidas(int iAnio, int iMes, int? iDia)
        {
            this.iAnio = iAnio;
            this.iMes = iMes;
            this.iDia = iDia;
            this.eTipoDeConsulta = ConSat.TipoDeConsulta.Recibidas;
            this.webSat.Navigate(ConSat.UrlInicio);
        }

        public void InicializarNavegador(WebBrowser oWeb)
        {
            if (this.webSat != null)
            {
                this.webSat.Dispose();
                this.webSat = null;
            }
            this.webSat = oWeb;
            this.webSat.Navigating += webSat_Navigating;
            this.webSat.DocumentCompleted += webSat_DocumentCompleted;
            this.webSat.NewWindow += webSat_NewWindow;
        }

        public void InicializarNavegador()
        {
            this.InicializarNavegador(new WebBrowser());
        }

        public void IniciarObtenerXmls()
        {
            this.InsertarScript("window.ObtenerXmls();");
            this.ReportarPaso(ConSat.ConSatPaso.ObteniendoXmls);
        }

        #endregion
    }
}
