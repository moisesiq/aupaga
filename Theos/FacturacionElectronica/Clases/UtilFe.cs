using System;
using System.IO;

namespace FacturacionElectronica
{
    public static class UtilFe
    {
        public static string RutaFe()
        {
            return (Directory.GetCurrentDirectory() + "\\" + "FacturacionElectronica" + "\\");
        }

        public static string RutaFe(string sArchivo)
        {
            return (UtilFe.RutaFe() + sArchivo);
        }
    }
}
