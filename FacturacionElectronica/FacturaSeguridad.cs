using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;

namespace FacturacionElectronica
{
    class FacturaSeguridad
    {
        public Dictionary<string, string> DatosCertificado(string sRutaCertificado)
        {
            var Res = new Dictionary<string, string>();
            var oCertificado = X509Certificate.CreateFromCertFile(sRutaCertificado);

            Res.Add("NumeroDeSerie", "");
            string sSerie = oCertificado.GetSerialNumberString();
            for (int iCont = 0; iCont < sSerie.Length; iCont++)
            {
                if ((iCont % 2) != 0)
                    Res["NumeroDeSerie"] += sSerie.Substring(iCont, 1);
            }

            Res.Add("Contenido", oCertificado.GetRawCertDataString());
            Res.Add("ContenidoBase64", Convert.ToBase64String(oCertificado.GetRawCertData()));

            return Res;
        }
                
        public ResAcc<string> GenerarSelloDigital(string sArchivoKey, string sContrasenia, string sCadenaOriginal)
        {
            var Res = new ResAcc<string>(true);
            try
            {
                byte[] encryptedPrivateKeyInfoData = File.ReadAllBytes(sArchivoKey);
                AsymmetricKeyParameter parameter = PrivateKeyFactory.DecryptKey(sContrasenia.ToCharArray(), encryptedPrivateKeyInfoData);
                MemoryStream stream = new MemoryStream();
                new StreamWriter(stream);
                StringWriter writer = new StringWriter();
                new PemWriter(writer).WriteObject(parameter);
                writer.Close();
                ISigner signer = SignerUtilities.GetSigner("SHA1WithRSA");
                byte[] bytes = Encoding.UTF8.GetBytes(sCadenaOriginal);
                signer.Init(true, parameter);
                signer.BlockUpdate(bytes, 0, bytes.Length);
                Res.Respuesta = Convert.ToBase64String(signer.GenerateSignature()).ToString();
            } catch (Exception e) {
                Res.Exito = false;
                Res.Mensaje = "Error al generar el sello digital\n\n";
                Res.Mensaje += (e.InnerException == null ? e.Message : e.InnerException.Message);
            }

            return Res;
        }
    }
}
