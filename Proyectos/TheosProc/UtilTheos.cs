using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TheosProc
{
    public static class UtilTheos
    {
        public static decimal ObtenerPrecioSinIva(decimal mPrecio)
        {
            return Math.Round(mPrecio / (1 + (Theos.Iva / 100)), 3);
        }

        public static decimal ObtenerIvaDePrecio(decimal mPrecio)
        {
            // return Math.Round(mPrecio - UtilLocal.ObtenerPrecioSinIva(mPrecio), 2);
            return (mPrecio - UtilTheos.ObtenerPrecioSinIva(mPrecio));
        }

        public static decimal ObtenerImporteMasIva(decimal mImporte)
        {
            return Math.Round(mImporte * (1 + (Theos.Iva / 100)), 3);
        }

        public static decimal ObtenerIvaDeImporte(decimal mImporte)
        {
            return Math.Round(mImporte * (Theos.Iva / 100), 3);
        }

        public static string Encriptar(string cadena)
        {
            string key = "fon";
            byte[] keyArray;
            byte[] arrCifrar = UTF8Encoding.UTF8.GetBytes(cadena);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] ArrayResultado = cTransform.TransformFinalBlock(arrCifrar, 0, arrCifrar.Length);
            tdes.Clear();

            return Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
        }

        public static string Desencriptar(string cadenaEncriptada)
        {
            string key = "fon";
            byte[] keyArray;
            byte[] arrDescifrar = Convert.FromBase64String(cadenaEncriptada);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(arrDescifrar, 0, arrDescifrar.Length);
            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
