namespace TheosProc
{
    public class Cat
    {
        public class CodigosRes
        {
            public const int ConflictoEnTraspasos = 1;
        }

        public class TipoOperacionMovimiento
        {
            public const int Traspaso = 5;
        }

        public class TraspasoEstatusContingencias
        {
            public const int Solucionado = 1;
            public const int NoSolucionado = 2;
        }

        public class OperacionesKardex
        {
            public const int Venta = 1;
            public const int VentaCancelada = 2;
            public const int EntradaCompra = 3;
            public const int DevolucionAProveedor = 4;
            public const int EntradaInventario = 5;
            public const int SalidaInventario = 6;
            public const int EntradaTraspaso = 7;
            public const int SalidaTraspaso = 8;
        }

        public class ContaAfectaciones
        {
            public const int VentaContadoPago = 1;
            public const int VentaContadoPagoFacturaGlobal = 28;
            public const int VentaContadoVale = 2;
            public const int VentaCredito = 3;
            public const int DevolucionVentaPago = 4;
            public const int DevolucionVentaValeFactura = 5;
            public const int DevolucionVentaValeTicket = 27;
            public const int PagoVentaCredito = 6;

            public const int NotaDeCreditoDescuentoVenta = 7;
            public const int DevolucionVentaCreditoFacturadaPago = 8;
            public const int DevolucionVentaCreditoFacturadaVale = 9;

            public const int GarantiaVentaValeFactura = 10;
            public const int GarantiaVentaPagoFactura = 11;
            public const int GarantiaVentaCreditoFacturadaPago = 33;
            public const int GarantiaVentaCreditoFacturaVale = 34;
            public const int GarantiaVentaValeTicket = 35;

            // public const int CompraContado = 12;
            public const int CompraCreditoFactura = 13;
            public const int CompraCreditoNota = 37;
            public const int PagoCompraCredito = 22;
            public const int PagoCompraCreditoNotaDeCreditoGarantia = 23;
            public const int PagoCompraCreditoNotaDeCreditoDevolucion = 24;
            public const int PagoCompraCreditoDescuentoDirecto = 25;
            public const int PagoCompraCreditoGastoCaja = 14;
            public const int PagoCompraCreditoGastoCajaFacturado = 26;
            public const int PagoCompraCreditoDescuentoFactura = 36;

            // Estas ya no se usarán, creo, pero se mantiene por casos que ya hay en el código
            public const int DevolucionCompraPago = -1;
            public const int DevolucionCompraNotaDeCredito = -2;

            public const int EntradaInventario = 15;
            public const int SalidaInventario = 16;

            public const int Resguardo = 17;
            public const int Refuerzo = 18;
            public const int GastoCajaFacturado = 19;
            public const int DepositoBancario = 20;
            public const int GastoFacturadoBanco = 30;
            public const int GastoFacturadoEfectivo = 29;
            public const int GastoNotaEfectivo = 31;
            public const int PagoProveedorDirectoCpcp = 40;
            public const int GastoContableFacturadoBancoCpcp = 41;

            public const int ValeDirecto = 32;

            public const int NominaOficial = 38;
            public const int InteresesBancarios = 39;
            public const int GastoReparteUtilidades = 42;
            public const int Pago2Por = 43;
            public const int PagoImss = 44;
            public const int PagoInfonavit = 45;
        }

        public class ContaCuentasDeMayor
        {
            public const int Salarios = 4;
            public const int Imss = 46;
            public const int Infonavit = 44;
            public const int Nomina2Por = 5;
            public const int TiempoExtra = 104;
            public const int Vacaciones = 105;
            public const int PrimaVacacional = 106;
            public const int Aguinaldo = 37;
            public const int Ptu = 108;
            public const int PremioDeAsistencia = 109;
            public const int PremioDePuntualidad = 111;
            public const int Ispt = 112;
            public const int RetencionImss = 113;
            public const int SubsidioAlEmpleo = 114;
            public const int RetencionInfonavit = 115;
            public const int DeudoresDiversos = 73;

            // public const int Usuarios = -1;
            public const int Clientes = 57;
            public const int Proveedores = 26;
            public const int Bancos = 3;
            public const int Agua = 94;
            public const int InteresesBancarios = 33;
            public const int CuentasPorPagarCortoPlazo = 76;
            public const int ReparteDeUtilidades = 18;
        }

        public class ContaCuentasAuxiliares
        {
            public const int SueldoIsidroPadre = 7;
            public const int SueldoIsidroHijo = 8;
            public const int UtilidadesIsidroPadre = 156;
            public const int UtilidadesIsidroHijo = 162;
            public const int CascoChico = 28;
            public const int CascoMediano = 29;
            public const int CascoGrande = 30;
            public const int CascoExtragrande = 64;

            public const int Inventario = 353;
            public const int CostoVenta = 421;
            public const int VentasContado = 434;
            public const int VentasCredito = 435;
            public const int IvaTrasladadoCobrado = 355;
            public const int IvaTrasladadoNoCobrado = 356;
            public const int Caja = 2;
            public const int IvaAFavor = 352;
            public const int IvaAcreditablePorPagar = 415;
            public const int IvaAcreditablePagado = 414;
            public const int CapitalFijo = 436;
            public const int InventarioGarantias = 573;
            public const int Resguardo = 3;
            public const int FondoDeCaja = 438;
            public const int GastosNoDeducibles = 578;
            public const int AnticipoClientes = 571;
            public const int DescuentoSobreVentaClientes = 31;
            public const int DevolucionSobreVentaClientes = 572;

            public const int ClientesHectorRicardo = 575;
            public const int ClientesJoseManuel = 576;
            public const int ClientesEdgarAron = 577;

            public const int ReservaNomina = 1076;
        }

        public class Tablas
        {
            public const string CajaEfectivoPorDia = "CajaEfectivoPorDia";
            public const string CajaIngreso = "CajaIngreso";
            public const string CajaEgreso = "CajaEgreso";
            public const string Venta = "Venta";
            public const string VentaPago = "VentaPago";
            public const string VentaPagoDetalle = "VentaPagoDetalle";
            public const string VentaDevolucion = "VentaDevolucion";
            public const string Tabla9500 = "Cotizacion9500";
            public const string NotaDeCredito = "NotaDeCredito";
            public const string VentaCambio = "VentaCambio";
            public const string VentaGarantia = "VentaGarantia";
            public const string VentaFactura = "VentaFactura";
            public const string ContaEgreso = "ContaEgreso";

            public const string Marcas = "MarcaParte";
            public const string Lineas = "Linea";

            public const string NotaDeCreditoFiscal = "NotaDeCreditoFiscal";
            public const string MovimientoInventario = "MovimientoInventario";
            public const string ProveedorPolizaDetalle = "ProveedorPolizaDetalle";
            public const string BancoCuentaMovimiento = "BancoCuentaMovimiento";
            public const string NominaUsuario = "NominaUsuario";
            public const string Nomina = "Nomina";
            public const string CajaFacturaGlobal = "CajaFacturaGlobal";
            public const string NominaImpuesto = "NominaImpuesto";
        }

        public class Clientes
        {
            public const int VentasMostrador = 1;
        }

        public class Sucursales
        {
            public const int Matriz = 1;
            public const int Sucursal2 = 2;
            public const int Sucursal3 = 3;
        }

        public class FormasDePago
        {
            public const int Cheque = 1;
            public const int Efectivo = 2;
            public const int Transferencia = 3;
            public const int Tarjeta = 4;
            public const int NoIdentificado = 5;
            public const int Vale = 6;
            public const int NotaDeCreditoFiscal = 7;
        }

        public class ContaTiposDePoliza
        {
            public const int Ingreso = 1;
            public const int Egreso = 2;
            public const int Diario = 3;
        }

        public class EstatusGenericos
        {
            public const int Realizada = 1;
            public const int Pendiente = 2;
            public const int Completada = 3;
            public const int Cancelada = 4;
            public const int PorCompletar = 5;
            public const int EnCurso = 6;
            public const int Contado = 7;
            public const int EnRevision = 8;
            public const int Recibido = 9;
            public const int Resuelto = 10;
            public const int Vendido = 11;
            public const int CanceladoAntesDeVender = 12;
            public const int CanceladoDespuesDeVendido = 13;
        }

        public class InventarioConteosRevisiones
        {
            public const int SinRevision = 0;
            public const int Confirmacion = 1;
            public const int ConfirmacionGerente = 2;
            public const int OtraSucursal = 3;
        }
    }
}
