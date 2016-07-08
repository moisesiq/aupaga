using System;

namespace Refaccionaria.App
{
    class pauCajaCorte_Result
    {
        public int Orden { get; set; }
        public string Categoria { get; set; }
        public string Concepto { get; set; }
        public decimal? Tickets { get; set; }
        public decimal? Facturas { get; set; }
        public decimal? Total { get; set; }
        public int? Pendientes { get; set; }
    }

    class cdcVentas
    {
        public int RegistroID { get; set; }
        public string Folio { get; set; }
        public string Cliente { get; set; }
        public string FormaDePago { get; set; }
        public string Usuario { get; set; }
        public string Caracteristica { get; set; }
        public decimal Importe { get; set; }
        public DateTime? FechaVistoBueno { get; set; }
        public int Devolucion { get; set; }
        public string CatTabla { get; set; }

        public int Facturada { get; set; }
        public int Orden { get; set; }
    }

    class cdcDevoluciones
    {
        public int VentaDevolucionID { get; set; }
        public string Folio { get; set; }
        public string Tipo { get; set; }
        public string Cliente { get; set; }
        public string Salida { get; set; }
        public string Autorizacion { get; set; }
        public decimal Importe { get; set; }
        public DateTime? FechaVistoBueno { get; set; }
    }

    class cdcGarantias
    {
        public int VentaGarantiaID { get; set; }
        public string Folio { get; set; }
        public string Cliente { get; set; }
        public string Salida { get; set; }
        public string Autorizacion { get; set; }
        public decimal Importe { get; set; }
        public DateTime? FechaVistoBueno { get; set; }
    }

    class cdc9500
    {
        public int Cotizacion9500ID { get; set; }
        public string Folio { get; set; }
        public string Cliente { get; set; }
        public decimal Anticipo { get; set; }
        public DateTime? FechaVistoBueno { get; set; }
    }

    class cdcCobranza
    {
        public int VentaPagoID { get; set; }
        public string Folio { get; set; }
        public string Cliente { get; set; }
        public string FormaDePago { get; set; }
        public decimal Importe { get; set; }
        public DateTime? FechaVistoBueno { get; set; }
    }

    class cdcIngresosEgresos
    {
        public int RegistroID { get; set; }
        public string Concepto { get; set; }
        public string Autorizacion { get; set; }
        public decimal Importe { get; set; }
        public DateTime? FechaVistoBueno { get; set; }
    }

    class cdcCambios
    {
        public int VentaCambioID { get; set; }
        public string Folio { get; set; }
        public string Cliente { get; set; }
        public string FormasDePago { get; set; }
        public string Vendedor { get; set; }
        public string Comisionista { get; set; }
    }
}
