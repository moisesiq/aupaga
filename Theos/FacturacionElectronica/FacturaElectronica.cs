using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using LibUtil;

namespace FacturacionElectronica
{
    #region [ Clase para uso tipo enumeraciones ]

    public class Enumerados
    {
        public class Versiones
        {
            public const string v3_2 = "3.2";
        }

        public class TiposDeComprobante
        {
            public const string Ingreso = "ingreso";
            public const string Egreso = "egreso";
            public const string Traslado = "traslado";
        }

        public class TiposDeImpuesto
        {
            /// <summary>
            /// Impuesto al Valor Agregado
            /// </summary>
            public const string Iva = "IVA";
            /// <summary>
            /// Impuesto sobre la renta
            /// </summary>
            public const string Isr = "ISR";
            /// <summary>
            /// Impuesto especial sobre productos y servicios
            /// </summary>
            public const string Ieps = "IEPS";
        }

        public class RegimenesFiscales
        {
            
        }

        
    }

    #endregion

    [Serializable]
    public class FacturaElectronica
    {
        public FacturaElectronica()
        {
            this.Version = Enumerados.Versiones.v3_2;
        }

        #region [ Propiedades ]

        /// <summary>
        /// Para contener el sello digital del comprobante fiscal, al que hacen referencia las reglas de resolución miscelánea aplicable. El sello deberá ser expresado cómo una cadena de texto en formato Base 64.
        /// </summary>
        public string Sello { get; set; }

        #region [ Propiedades fijas con valor preestablecido ]

        /// <summary>
        /// Valor prefijado a 3.2 que indica la versión del estándar bajo el que se encuentra expresado el comprobante.
        /// </summary>
        public string Version { get; set; }

        #endregion

        #region [ Propiedades fijas ]

        /// <summary>
        /// Para expresar el número de serie del certificado de sello digital que ampara al comprobante, de acuerdo al acuse correspondiente a 20 posiciones otorgado por el sistema del SAT.
        /// </summary>
        public string NumeroDeCertificado { get; set; }
        /// <summary>
        /// Para expresar el certificado de sello digital que ampara al comprobante como texto, en formato base 64.
        /// </summary>
        public string Certificado { get; set; }

        public string TipoDeImpuesto { get; set; }
        public string TasaDeImpuesto { get; set; }
                
        #endregion

        #region [ Propiedades variables obligatorias ]

        /// <summary>
        /// Para expresar el efecto del comprobante fiscal para el contribuyente emisor.
        /// </summary>
        public string TipoDeComprobante { get; set; }
        /// <summary>
        /// Para la expresión de la fecha y hora de expedición  del comprobante fiscal.
        /// </summary>
        public DateTime Fecha { get; set; }
        /// <summary>
        /// Para incorporar el lugar de expedición del comprobante.
        /// </summary>
        public string LugarDeExpedicion { get; set; }
        /// <summary>
        /// Para precisar la forma de pago que aplica para este comprobnante fiscal digital a través de Internet. Se utiliza para expresar Pago en una sola exhibición o número de parcialidad pagada contra el total de parcialidades, Parcialidad 1 de X.
        /// </summary>
        public string FormaDePago { get; set; }        
        /// <summary>
        /// Texto libre para expresar el método de pago de los bienes o servicios amparados por el comprobante. Se entiende como método de pago leyendas tales como: cheque, tarjeta de crédito o debito, depósito en cuenta, etc.
        /// </summary>
        public string MetodoDePago { get; set; }

        #endregion

        #region [ Propiedades variables no obligatorias ]

        /// <summary>
        /// Para precisar la serie para control interno del contribuyente. Este atributo acepta una cadena de caracteres alfabéticos de 1 a 25 caracteres sin incluir caracteres acentuados.
        /// </summary>
        public string Serie { get; set; }
        /// <summary>
        /// Para control interno del contribuyente que acepta un valor numérico entero superior a 0 que expresa el folio del comprobante.
        /// </summary>
        public string Folio { get; set; }
        /// <summary>
        /// Para expresar las condiciones comerciales aplicables para el pago del comprobante fiscal digital a través de Internet.
        /// </summary>
        public string CondicionesDePago { get; set; }
        /// <summary>
        /// Para representar el tipo de cambio conforme a la moneda usada.
        /// </summary>
        public string TipoDeCambio { get; set; }
        /// <summary>
        /// Para expresar la moneda utilizada para expresar los montos.
        /// </summary>
        public string Moneda { get; set; }
        /// <summary>
        /// Para incorporar al menos los cuatro últimos digitos del número de cuenta con la que se realizó el pago.
        /// </summary>
        public string CuentaPago { get; set; }
        /// <summary>
        /// Para expresar el motivo del descuento aplicable.
        /// </summary>
        public string MotivoDeDescuento { get; set; }
        /// <summary>
        /// Para señalar el número de folio fiscal del comprobante que se hubiese expedido por el valor total del comprobante, tratándose del pago en parcialidades.
        /// </summary>
        public string FolioFiscalOriginal { get; set; }
        /// <summary>
        /// Para señalar la serie del folio del comprobante que se hubiese expedido por el valor total del comprobante, tratándose del pago en parcialidades.
        /// </summary>
        public string SerieFolioFiscalOriginal { get; set; }
        /// <summary>
        /// Para señalar la fecha de expedición del comprobante que se hubiese emitido por el valor total del comprobante, tratándose del pago en parcialidades.
        /// </summary>
        public DateTime FechaFolioFiscalOriginal { get; set; }
        /// <summary>
        /// Para señalar el total del comprobante que se hubiese expedido por el valor total de la operación, tratándose del pago en parcialidades
        /// </summary>
        public decimal MontoFolioFiscalOriginal { get; set; }

        #endregion

        #region [ Propiedades calculables (no requieren llenarse explícitamente) ]

        /// <summary>
        /// Para representar la suma de los importes antes de descuentos e impuestos.
        /// </summary>
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        /// <summary>
        /// Para representar el importe total de los descuentos aplicables antes de impuestos.
        /// </summary>
        public decimal Descuento { get; set; }
        /// <summary>
        /// Para representar la suma del subtotal, menos los descuentos aplicables, más los impuestos trasladados, menos los impuestos retenidos.
        /// </summary>
        public decimal Total { get; set; }

        #endregion

        #region [ Propiedades complejas ]

        /// <summary>
        /// Para expresar la información del contribuyente emisor del comprobante.
        /// </summary>
        public Emisor Emisor { get; set; }
        /// <summary>
        /// Para precisar la información del contribuyente receptor del comprobante.
        /// </summary>
        public Receptor Receptor { get; set; }
        /// <summary>
        /// Para enlistar los conceptos cubiertos por el comprobante.
        /// </summary>
        public List<Concepto> Conceptos { get; set; }
        /// <summary>
        /// Para capturar los impuestos aplicables.
        /// </summary>
        public Impuesto Impuestos { get; set; }

        #endregion

        #region [ Propiedades generadas con el proceso ]

        [XmlIgnore]
        public Config Configuracion { get; set; }

        [XmlIgnore]
        public TimbreXml Timbre { get; private set; }

        [XmlIgnore]
        public string XmlFactura { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> Adicionales { get; set; }

        #endregion

        #endregion

        #region [ Métodos ]

        private void ProcesarCamposCalculados()
        {
            // Se le quita un minuto a la fecha para evitar problemas de sincronización, con Edicom
            this.Fecha = this.Fecha.AddMinutes(-1);

            //
            this.TipoDeImpuesto = Enumerados.TiposDeImpuesto.Iva;

            this.Subtotal = 0;
            this.Iva = 0;
            this.Descuento = 0;
            foreach (var oConcepto in this.Conceptos)
            {
                this.Subtotal += (oConcepto.ValorUnitario * oConcepto.Cantidad);
                this.Iva += (oConcepto.Iva * oConcepto.Cantidad);
                this.Descuento += (oConcepto.Descuento * oConcepto.Cantidad);
            }
            this.Total = (this.Subtotal + this.Iva - this.Descuento);
        }

        #endregion

        #region [ Públicos ]

        public ResAcc<string> GenerarFactura()
        {
            // Se genera el Xml
            var ResXml = FacturaXml.GenerarXml(this);
            if (ResXml.Error)
                return new ResAcc<string>(false, ResXml.Mensaje);
            // Se llenan algunos datos generados
            /* this.NumeroDeCertificado = ResXml.Respuesta.NumeroDeCertificado;
            this.Certificado = ResXml.Respuesta.Certificado;
            this.Sello = ResXml.Respuesta.Sello;
            this.XmlFactura = ResXml.Respuesta.XmlFactura;
            */

            var Res = new ResAcc<string>(true);
            Res.Respuesta = this.XmlFactura;
            return Res;
        }

        public ResAcc<string> GenerarFactura(bool bProcesarCamposCalculados)
        {
            if (bProcesarCamposCalculados)
                this.ProcesarCamposCalculados();
            return this.GenerarFactura();
        }
        
        public ResAcc<string> TimbrarFactura(string sXmlFactura, bool bPrueba)
        {
            var oPac = new FacturaPac(this.Configuracion.UsuarioPac, this.Configuracion.ContraseniaPac);
            oPac.Prueba = bPrueba;
            var Res = oPac.TimbrarFactura(Encoding.UTF8.GetBytes(sXmlFactura));
            if (Res.Error)
                return Res;
            
            // Se llenan los datos del timbrado
            var ResTimbre = this.DatosFacturaTimbrada(Res.Respuesta);
            if (ResTimbre.Error) {
                Res.Exito = false;
                Res.Mensaje = ResTimbre.Mensaje;
                return Res;
            }
            this.Timbre = ResTimbre.Respuesta;

            return Res;
        }

        public ResAcc<string> TimbrarFactura(string sXmlFactura)
        {
            return this.TimbrarFactura(sXmlFactura, false);
        }
        
        public ResAcc<string> CancelarFactura(string sFolioFiscal, bool bPrueba)
        {
            var oPac = new FacturaPac(this.Configuracion.UsuarioPac, this.Configuracion.ContraseniaPac);
            oPac.Prueba = bPrueba;
            var oPfx = File.ReadAllBytes(this.Configuracion.RutaArchivoPfx);
            var Res = oPac.CancelarFactura(sFolioFiscal, this.Emisor.RFC, oPfx, this.Configuracion.ContraseniaArchivoPfx);
            return Res;
        }

        public ResAcc<string> CancelarFactura(string sFolioFiscal)
        {
            return this.CancelarFactura(sFolioFiscal, false);
        }

        public ResAcc<TimbreXml> DatosFacturaTimbrada(string sXmlFactura)
        {
            return FacturaXml.LeerXmlTimbrado(sXmlFactura);
        }

        public System.Drawing.Image GenerarCbb()
        {
            string sCadena = FacturaProc.GenerarCadenaCbb(this.Timbre.FolioFiscal, this.Emisor.RFC, this.Receptor.RFC, this.Total);
            var oGenCbb = new AccesoCbb();
            return oGenCbb.GenerarCbb(sCadena, Encoding.UTF8);
        }

        #endregion
    }

    #region [ Clases auxiliares ]

    /// <summary>
    /// Tipo definido para introducir la información detallada de un bien o servicio amparado en el comprobante.
    /// </summary>
    public class Concepto
    {
        /// <summary>
        /// Sólo lectura. Para precisar el importe total de los bienes o servicios del presente concepto. Debe ser equivalente al resultado de multiplicar la cantidad por el valor unitario expresado en el concepto.
        /// </summary>
        // public decimal Importe { get { return (this.ValorUnitario * this.Cantidad); } }

        /// <summary>
        /// Para expresar el número de serie del bien o identificador del servicio amparado por el presente concepto.
        /// </summary>
        public string Identificador { get; set; }
        /// <summary>
        /// Para precisar la cantidad de bienes o servicios del tipo particular definido por el presente concepto.
        /// </summary>
        public decimal Cantidad { get; set; }
        /// <summary>
        /// Para precisar la unidad de medida aplicable para la cantidad expresada en el concepto.
        /// </summary>
        public string Unidad { get; set; }
        /// <summary>
        /// Para precisar la descripción del bien o servicio cubierto por el presente concepto.
        /// </summary>
        public string Descripcion { get; set; }
        /// <summary>
        /// Para precisar el valor o precio unitario del bien o servicio cubierto por el presente concepto.
        /// </summary>
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Iva del concepto seleccionado
        /// </summary>
        public decimal Iva { get; set; }
        /// <summary>
        /// Posible descuento aplicado al concepto
        /// </summary>
        public decimal Descuento { get; set; }
    }

    /// <summary>
    /// Tipo definido para expresar la información del contribuyente emisor del comprobante.
    /// </summary>
    public class Emisor
    {
        /// <summary>
        /// Para expresar claves del Registro Federal de Contribuyentes, sin guiones o espacios.
        /// </summary>
        public string RFC { get; set; }
        /// <summary>
        /// Para el nombre, denominación o razón social del contribuyente emisor del comprobante.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Para precisar la información de ubicación del domicilio fiscal del contribuyente emisor
        /// </summary>
        public Ubicacion DomicilioFiscal { get; set; }
        /// <summary>
        /// Para precisar la información de ubicación del domicilio en donde es emitido el comprobante fiscal en caso de que sea distinto del domicilio fiscal del contribuyente emisor.
        /// </summary>
        public Ubicacion ExpedidoEn { get; set; }
        
        /// <summary>
        /// Para incorporar los regímenes en los que tributa el contribuyente emisor. Puede contener más de un régimen.
        /// </summary>
        public List<string> RegimenesFiscales { get; set; }
    }

    /// <summary>
    /// Tipo definido para precisar la información del contribuyente receptor del comprobante.
    /// </summary>
    public class Receptor
    {
        /// <summary>
        /// Para expresar claves del Registro Federal de Contribuyentes, sin guiones o espacios.
        /// </summary>
        public string RFC { get; set; }
        /// <summary>
        /// Para el nombre, denominación o razón social del contribuyente receptor del comprobante.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Para precisar la información de ubicación del domicilio fiscal del contribuyente receptor
        /// </summary>
        public Ubicacion DomicilioFiscal { get; set; }
    }

    /// <summary>
    /// Tipo definido para expresar domicilios o direcciones.
    /// </summary>
    public class Ubicacion
    {
        /// <summary>
        /// Para precisar la avenida, calle, camino o carretera donde se da la ubicación.
        /// </summary>
        public string Calle { get; set; }
        /// <summary>
        /// Para expresar el número particular en donde se da la ubicación sobre una calle dada.
        /// </summary>
        public string NumeroExterior { get; set; }
        /// <summary>
        /// Para expresar información adicional para especificar la ubicación cuando calle y número exterior (noExterior) no resulten suficientes para determinar la ubicación de forma precisa.
        /// </summary>
        public string NumeroInterior { get; set; }
        /// <summary>
        /// Para expresar una referencia de ubicación adicional.
        /// </summary>
        public string Referencia { get; set; }
        /// <summary>
        /// Para precisar la colonia en donde se da la ubicación cuando se desea ser más específico en casos de ubicaciones urbanas.
        /// </summary>
        public string Colonia { get; set; }
        /// <summary>
        /// Para asentar el código postal en donde se da la ubicación.
        /// </summary>
        public string CodigoPostal { get; set; }
        /// <summary>
        /// Para precisar la ciudad o población donde se da la ubicación.
        /// </summary>
        public string Localidad { get; set; }
        /// <summary>
        /// Para precisar el municipio o delegación (en el caso del Distrito Federal) en donde se da la ubicación.
        /// </summary>
        public string Municipio { get; set; }
        /// <summary>
        /// Para precisar el estado o entidad federativa donde se da la ubicación.
        /// </summary>
        public string Estado { get; set; }
        /// <summary>
        /// Para precisar el país donde se da la ubicación.
        /// </summary>
        public string Pais { get; set; }
    }

    /// <summary>
    /// Tipo definido para capturar los impuestos aplicables.
    /// </summary>
    public class Impuesto
    {
        /// <summary>
        /// Para expresar el total de los impuestos retenidos que se desprenden de los conceptos expresados en el comprobante fiscal digital a través de Internet.
        /// </summary>
        public decimal TotalRetenciones
        {
            get
            {
                decimal mTotal = 0;
                foreach (var oRetencion in this.Retenciones)
                    mTotal += oRetencion.Importe;
                return mTotal;
            }
        }
        /// <summary>
        /// Para expresar el total de los impuestos trasladados que se desprenden de los conceptos expresados en el comprobante fiscal digital a través de Internet.
        /// </summary>
        public decimal TotalTraslados
        {
            get
            {
                decimal mTotal = 0;
                foreach (var oTraslado in this.Traslados)
                    mTotal += oTraslado.Importe;
                return mTotal;
            }
        }

        /// <summary>
        /// Para capturar los impuestos retenidos aplicables.
        /// </summary>
        public List<Retencion> Retenciones { get; set; }
        /// <summary>
        /// Para asentar o referir los impuestos trasladados aplicables.
        /// </summary>
        public List<Traslado> Traslados { get; set; }
    }

    /// <summary>
    /// Tipo definido para la información detallada de una retención de impuesto específico.
    /// </summary>
    public class Retencion
    {
        /// <summary>
        /// Para señalar el tipo de impuesto retenido.
        /// </summary>
        public string Impuesto { get; set; }
        /// <summary>
        /// Para señalar el importe o monto del impuesto retenido.
        /// </summary>
        public decimal Importe { get; set; }
    }

    /// <summary>
    /// Tipo definido para la información detallada de un traslado de impuesto específico.
    /// </summary>
    public class Traslado
    {
        /// <summary>
        /// Para señalar el tipo de impuesto trasladado.
        /// </summary>
        public string Impuesto { get; set; }
        /// <summary>
        /// Para señalar el importe del impuesto trasladado.
        /// </summary>
        public decimal Importe { get; set; }
        /// <summary>
        /// Para señalar la tasa del impuesto que se traslada por cada concepto amparado en el comprobante.
        /// </summary>
        public decimal Tasa { get; set; }
    }

    public class Config
    {
        public string RutaCertificado { get; set; }
        public string RutaArchivoKey { get; set; }
        public string ContraseniaArchivoKey { get; set; }
        public string RutaArchivoPfx { get; set; }
        public string ContraseniaArchivoPfx { get; set; }
        public string UsuarioPac { get; set; }
        public string ContraseniaPac { get; set; }
    }

    #endregion

    public class TimbreXml
    {
        public string FolioFiscal { get; set; }
        // public string Certificado { get; set; }
        // public string Sello { get; set; }
        public string SelloSat { get; set; }
        public string CertificadoSat { get; set; }
        public DateTime FechaCertificacion { get; set; }
        public string CadenaOriginal { get; set; }
    }
}
