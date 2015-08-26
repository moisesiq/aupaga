using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.Xsl;
using System;

namespace FacturacionElectronica
{
    static class FacturaXml
    {
        static XmlWriterSettings ConfigXml = new XmlWriterSettings()
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            ConformanceLevel = ConformanceLevel.Auto
        };
        static string ResultadoValidacion;

        public static ResAcc<FacturaElectronica> GenerarXml(FacturaElectronica oFactura)
        {
            // Se genera el xml con los datos de la factura
            var ResProcs = FacturaXml.GenerarXmlDeObjeto(oFactura);
            string sFacturaXml = ResProcs.Respuesta;

            // Se transforma al Xml requerido por el Sat v3.2
            ResProcs = FacturaXml.TransformarXml(sFacturaXml, Properties.Resources.Cfdi3_2);
            if (ResProcs.Error)
                return new ResAcc<FacturaElectronica>(false, ResProcs.Mensaje);
            string sCfdiXml = ResProcs.Respuesta;

            // Se valida el Xml generado
            ResProcs = FacturaXml.ValidarXml(sCfdiXml, Properties.Resources.cfdv32);
            if (ResProcs.Error)
                return new ResAcc<FacturaElectronica>(false, ResProcs.Mensaje);

            // Se genera la cadena original
            ResProcs = FacturaXml.TransformarXml(sCfdiXml, Properties.Resources.cadenaoriginal_3_2);
            if (ResProcs.Error)
                return new ResAcc<FacturaElectronica>(false, ResProcs.Mensaje);
            string sCadenaOriginal = System.Web.HttpUtility.HtmlDecode(ResProcs.Respuesta);

            // Se genera la cadena del sello digital
            var Seguridad = new FacturaSeguridad();
            ResProcs = Seguridad.GenerarSelloDigital(oFactura.Configuracion.RutaArchivoKey, oFactura.Configuracion.ContraseniaArchivoKey, sCadenaOriginal);
            if (ResProcs.Error)
                return new ResAcc<FacturaElectronica>(false, ResProcs.Mensaje);
            string sSelloDigital = ResProcs.Respuesta;

            // Se agregan los nuevos datos al xml a generar
            var oCertificado = Seguridad.DatosCertificado(oFactura.Configuracion.RutaCertificado);
            XmlDocument oXml = new XmlDocument();
            oXml.LoadXml(sCfdiXml);
            oXml.DocumentElement.Attributes["noCertificado"].Value = oCertificado["NumeroDeSerie"];
            oXml.DocumentElement.Attributes["certificado"].Value = oCertificado["ContenidoBase64"];
            oXml.DocumentElement.Attributes["sello"].Value = sSelloDigital;
            // Se guarda el xml con los cambios
            var oTexto = new StringWriterMod(Encoding.UTF8);
            var oEscXml = XmlWriter.Create(oTexto, FacturaXml.ConfigXml);
            oXml.Save(oEscXml);
            ResProcs.Exito = true;
            ResProcs.Respuesta = oTexto.ToString();

            oFactura.NumeroDeCertificado = oCertificado["NumeroDeSerie"];
            oFactura.Certificado = oCertificado["ContenidoBase64"];
            oFactura.Sello = sSelloDigital;
            oFactura.XmlFactura = ResProcs.Respuesta;

            return new ResAcc<FacturaElectronica>(true, oFactura);
        }

        public static ResAcc<TimbreXml> LeerXmlTimbrado(string sXmlFactura)
        {
            var Res = new ResAcc<TimbreXml>();
            
            XmlDocument oXml = new XmlDocument();
            oXml.LoadXml(sXmlFactura);
            var oTfds = oXml.GetElementsByTagName("tfd:TimbreFiscalDigital");
            if (oTfds.Count <= 0)
            {
                Res.Mensaje = "El Xml no contiene la información de timbrado.";
                return Res;
            }
            try
            {
                var oTimbreXml = new TimbreXml();
                var oTfd = oTfds[0];
                oTimbreXml.FolioFiscal = oTfd.Attributes["UUID"].Value;
                oTimbreXml.SelloSat = oTfd.Attributes["selloSAT"].Value;
                oTimbreXml.CertificadoSat = oTfd.Attributes["noCertificadoSAT"].Value;
                oTimbreXml.FechaCertificacion = Convert.ToDateTime(oTfd.Attributes["FechaTimbrado"].Value);
                
                // Se genera la cadena original del timbre fiscal
                var oFacturaComps = oXml.GetElementsByTagName("cfdi:Comprobante");
                if (oFacturaComps.Count <= 0) {
                    Res.Mensaje = "El Xml no contiene la información del comprobante o está mal formado.";
                    return Res;
                }
                var oFacturaComp = oFacturaComps[0];
                string sVersion = oFacturaComp.Attributes["version"].Value;
                string sSelloFactura = oFacturaComp.Attributes["sello"].Value;
                oTimbreXml.CadenaOriginal = FacturaXml.GenerarCadenaOriginalTimbre(sVersion, sSelloFactura, oTimbreXml);

                Res.Respuesta = oTimbreXml;
            }
            catch (Exception e)
            {
                Res.Mensaje = e.Message;
                return Res;
            }

            Res.Exito = true;
            return Res;
        }

        public static ResAcc<string> GenerarXmlDeObjeto(object Objeto)
        {
            var oTexto = new StringWriterMod(Encoding.UTF8);
            var oXml = XmlWriter.Create(oTexto, FacturaXml.ConfigXml);
            var oSer = new XmlSerializer(Objeto.GetType());
            oSer.Serialize(oXml, Objeto);
            string sXml = oTexto.ToString();
            oXml.Close();
            oTexto.Close();

            return new ResAcc<string>(true) { Respuesta = sXml };
        }

        public static ResAcc<string> TransformarXml(string sXml, string sXsl)
        {
            var Res = new ResAcc<string>(true);

            var oTexto = new StringWriterMod(Encoding.UTF8);
            var oXml = XmlWriter.Create(oTexto, FacturaXml.ConfigXml);
            XmlReader oXsl = XmlReader.Create(new StringReader(sXsl));
            XmlReader oFactura = XmlReader.Create(new StringReader(sXml));
            XslCompiledTransform TransXsl = new XslCompiledTransform();
            string sXmlRes = "";
            try
            {
                TransXsl.Load(oXsl);
                TransXsl.Transform(oFactura, oXml);
                sXmlRes = oTexto.ToString();
            }
            catch (Exception e)
            {
                Res.Exito = false;
                Res.Mensaje = "Error en transformación de Xml\n\n";
                Res.Mensaje += (e.InnerException == null ? e.Message : e.InnerException.Message);
            }
            oXsl.Close();
            oFactura.Close();
            oXml.Close();
            oTexto.Close();

            Res.Respuesta = sXmlRes;
            return Res;
        }

        public static ResAcc<string> ValidarXml(string sCadenaXml, string sCadenaXsd)
        {
            var Res = new ResAcc<string>();// { Mensaje = "" };

            var EsquemaXsd = XmlSchema.Read(new StringReader(sCadenaXsd), null);
            XmlReaderSettings Config = new XmlReaderSettings();
            Config.Schemas.Add(EsquemaXsd);
            Config.ValidationType = ValidationType.Schema;
            Config.ValidationEventHandler += new ValidationEventHandler((o, e) =>
            {
                if (e.Severity == XmlSeverityType.Warning)
                {
                    Res.Mensaje += ("Warning: " + e.Message + "\n");
                }
                else if (e.Severity == XmlSeverityType.Error)
                {
                    Res.Mensaje += ("Error: " + e.Message + "\n");
                }
            });

            XmlReader LectorXml = XmlReader.Create(new StringReader(sCadenaXml), Config);

            FacturaXml.ResultadoValidacion = "";
            while (LectorXml.Read()) { }

            Res.Exito = (Res.Mensaje == null);

            return Res;
        }

        private static string GenerarCadenaOriginalTimbre(string sVersion, string sSelloFactura, TimbreXml oTimbre)
        {
            return
                "||"
                + sVersion
                + "|" + oTimbre.FolioFiscal
                + "|" + oTimbre.FechaCertificacion.ToString("yyyy-MM-ddTHH:mm:ss")
                + "|" + sSelloFactura
                + "|" + oTimbre.CertificadoSat
                + "||";
        }
    }
}
