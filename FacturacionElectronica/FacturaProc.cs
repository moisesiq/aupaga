using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacturacionElectronica
{
    static class FacturaProc
    {
        public static string GenerarCadenaCbb(string sFolioFiscal, string sRfcEmisor, string sRfcReceptor, decimal mImporteTotal)
        {
            return string.Format("?re={0}&rr={1}&tt={2}&id={3}",
                sRfcEmisor,
                sRfcReceptor,
                mImporteTotal.ToString("0000000000.000000"),
                sFolioFiscal
            );
        }
    }
}
