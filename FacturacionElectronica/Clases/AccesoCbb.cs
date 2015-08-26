using System;
using System.Text;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;

namespace FacturacionElectronica
{
    class AccesoCbb
    {
        public Image GenerarCbb(string sCadena, Encoding oCodificacion)
        {
            var oCbb = new QRCodeEncoder();
            return oCbb.Encode(sCadena, oCodificacion);
        }
    }
}
