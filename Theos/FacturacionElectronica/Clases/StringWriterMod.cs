using System;
using System.IO;
using System.Text;

namespace FacturacionElectronica
{
    class StringWriterMod : StringWriter
    {
        public StringWriterMod(Encoding oEncoding) : base()
        {
            this._Encoding = oEncoding;
        }

        private Encoding _Encoding;
        public override Encoding Encoding
        {
            get { return this._Encoding; }
        }
    }
}
