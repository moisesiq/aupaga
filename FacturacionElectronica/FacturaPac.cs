using System;
using System.Text;
using System.IO;

using FacturacionElectronica.Edicom;

namespace FacturacionElectronica
{
    class FacturaPac
    {
        const string CfdiNombre = "SIGN_XML_COMPROBANTE_3_0.xml";

        public bool Prueba { get; set; }

        public string Usuario { get; set; }
        public string Contrasenia { get; set; }

        public FacturaPac(string sUsuario, string sContrasenia)
        {
            this.Usuario = sUsuario;
            this.Contrasenia = sContrasenia;
        }

        public ResAcc<string> TimbrarFactura(byte[] XmlFactura)
        {
            var Res = new ResAcc<string>();
            try
            {
                var oServicio = new Edicom.CFDiService();
                byte[] oCfdiZip;

                // Se llama el servico de Edicom, prueba o normal
                if (this.Prueba)
                    oCfdiZip = oServicio.getCfdiTest(this.Usuario, this.Contrasenia, XmlFactura);
                else
                    oCfdiZip = oServicio.getCfdi(this.Usuario, this.Contrasenia, XmlFactura);

                var oZip = new AccesoZip(oCfdiZip);
                var oCfdi = oZip.ObtenerArchivo(FacturaPac.CfdiNombre);
                Res.Respuesta = Encoding.UTF8.GetString(oCfdi);
                Res.Exito = true;
            }
            catch (Exception e)
            {
                Res.Mensaje = e.Message;
            }

            return Res;
        }

        public ResAcc<string> CancelarFactura(string sUuid, string sRfc, byte[] ArchivoPfx, string sContraseniaPfx)
        {
            var Res = new ResAcc<string>();
            try
            {
                var oServicio = new Edicom.CFDiService();
                
                // Se manda cancelar la factura, prueba o normal
                if (this.Prueba)
                {
                    Res.Respuesta = "Resultado de prueba. No hay mecanismo de cancelación en Edicom.";
                }
                else
                {
                    CancelaResponse oCancel = oServicio.cancelaCFDi(this.Usuario, this.Contrasenia, sRfc, new string[] { sUuid }, ArchivoPfx, sContraseniaPfx);
                    Res.Respuesta = oCancel.ack;
                }
                Res.Exito = true;
            }
            catch (Exception e)
            {
                Res.Mensaje = e.Message;
            }
            return Res;
        }
    }
}
