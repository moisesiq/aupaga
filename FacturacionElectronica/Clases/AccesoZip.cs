using System;
using System.IO;

using Ionic.Zip;

namespace FacturacionElectronica
{
    class AccesoZip
    {
        ZipFile ArchivoZip { get; set; }

        public AccesoZip(byte[] Archivo)
        {
            this.ArchivoZip = ZipFile.Read(new MemoryStream(Archivo));
        }

        public byte[] ObtenerArchivo(string sArchivo)
        {
            var oStream = new MemoryStream();
            this.ArchivoZip[sArchivo].Extract(oStream);
            return oStream.ToArray();
        }
    }
}
