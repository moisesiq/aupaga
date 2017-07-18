namespace TheosProc
{
    public static class Cat
    {
        public class TiposDeAfectacion
        {
            public const int NoEspecificado = 0;
            public const int SinCambios = 1;
            public const int Agregar = 2;
            public const int Modificar = 3;
            public const int Borrar = 4;
        }

        public class TipoDeFuentes
        {
            public const int Mostrador = 4;
        }

        public class Clientes
        {
            public const int Mostrador = 1;
        }

        public class VentasEstatus
        {
            public const int Realizada = 1;
            public const int Cobrada = 2;
            public const int Completada = 3;
            public const int Cancelada = 4;
            public const int CanceladaSinPago = 5;
            public const int AGarantia = 6;
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

        public class TiposDePago
        {
            public const int Credito = 1;
            public const int Contado = 2;
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
            public const int TarjetaDeDebito = 8;
        }

        public class PartesEstatus
        {
            public const int Activo = 1;
            public const int Inactivo = 2;
        }

        public class Medidas
        {
            public const int Pieza = 2;
        }

        public class AutorizacionesProcesos
        {
            public const int CreditoNoAplicable = 1;
            public const int c9500SinAnticipo = 2;
            public const int DevolucionCancelacion = 3;
            public const int FondoDeCajaDiferencia = 4;
            public const int Gastos = 5;
            public const int OtrosIngresos = 6;
            public const int VentaCambio = 7;
            public const int Refuerzo = 8;
            public const int Resguardo = 9;
            public const int GastosBorrar = 10;
            public const int OtrosIngresosBorrar = 11;
            public const int c9500PrecioFueraDeRango = 12;
            public const int CorteDeCaja = 13;
            public const int NotaDeCreditoCrear = 14;
            public const int NotaDeCreditoOtroClienteUsar = 15;
            public const int Garantia = 16;
            public const int NotaDeCreditoFiscalCrear = 17;
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
            public const string NotaDeCreditoFiscalDetalle = "NotaDeCreditoFiscalDetalle";
            public const string MovimientoInventario = "MovimientoInventario";
            public const string ProveedorPolizaDetalle = "ProveedorPolizaDetalle";
            public const string BancoCuentaMovimiento = "BancoCuentaMovimiento";
            public const string NominaUsuario = "NominaUsuario";
            public const string Nomina = "Nomina";
            public const string CajaFacturaGlobal = "CajaFacturaGlobal";
            public const string NominaImpuesto = "NominaImpuesto";
            public const string CascoRegistro = "CascoRegistro";
        }

        public class CajaTiposDeIngreso
        {
            public const int Refuerzo = 1;
            public const int Otros = 5;
        }

        public class CajaTiposDeEgreso
        {
            public const int Resguardo = 1;
            public const int CuentaAuxiliar = 12;
        }

        public class Sucursales
        {
            public const int Matriz = 1;
            public const int Sucursal2 = 2;
            public const int Sucursal3 = 3;
        }

        public class TiposDeAbc
        {
            public const string Ventas = "VENTAS";
            public const string Utilidad = "UTILIDAD";
            public const string Negocio = "NEGOCIO";
            public const string Proveedor = "PROVEEDOR";
            public const string Linea = "LINEA";
        }

        public class PartesCambios
        {
            public const string CodigoDeBarras = "Cod. Barras";
            public const string NumeroDeParte = "Num. Parte";
            public const string Descripcion = "Descripción";
            public const string Proveedor = "Proveedor";
            public const string Marca = "Marca";
            public const string Linea = "Línea";
            public const string UnidadDeMedida = "U. Medida";
            public const string UnidadDeEmpaque = "U. Empaque";
            public const string TiempoDeReposicion = "T. Reposición";
            public const string AplicaComision = "Comisión";
            public const string EsServicio = "Servicio";
            public const string Etiqueta = "Etiqueta";
            public const string SoloUnaEtiqueta = "Sólo 1 Et.";
            public const string EsPar = "Par";
            public const string Costo = "Costo";
            public const string CostoConDescuento = "CostoConDescuento";
            public const string PorUtil1 = "% Util 1";
            public const string PorUtil2 = "% Util 2";
            public const string PorUtil3 = "% Util 3";
            public const string PorUtil4 = "% Util 4";
            public const string PorUtil5 = "% Util 5";
            public const string Precio1 = "Precio 1";
            public const string Precio2 = "Precio 2";
            public const string Precio3 = "Precio 3";
            public const string Precio4 = "Precio 4";
            public const string Precio5 = "Precio 5";
        }

        public class TiposDeDocumento
        {
            public const int Factura = 1;
            public const int OtroOficial = 2;
            public const int Nota = 3;
        }

        public class Procesos
        {
            public const string VentasLeyendas = "VentasLeyendas";
        }

        public class VentasGarantiasAcciones
        {
            public const int ArticuloNuevo = 1;
            public const int NotaDeCredito = 2;
            public const int Efectivo = 3;
            public const int RevisionDeProveedor = 4;
            public const int NoProcede = 5;
            public const int Cheque = 6;
            public const int Tarjeta = 7;
            public const int Transferencia = 8;
            public const int TarjetaDeDebito = 9;
        }

        public class VentasGarantiasRespuestas
        {
            public const int ArticuloNuevo = 1;
            public const int NotaDeCredito = 2;
            public const int NoProcedio = 3;
        }

        public class ContaCuentas
        {
            public const int Activo = 1;
            public const int Pasivo = 2;
            public const int CapitalContable = 3;
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
            public const int CuentasPorPagarLargoPlazo = 90;
            public const int ReparteDeUtilidades = 18;
            public const int AcreedoresDiversos = 120;
            public const int Edificios = 118;
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
            public const int CascoIntermedio = 4592;

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
            public const int TarjetaDeCredito = 1344;
        }

        public class OrigenesNotaDeCredito
        {
            public const int Directo = 1;
            public const int Devolucion = 2;
            public const int Comision = 3;
            public const int Anticipo9500 = 4;
            public const int Garantia = 5;
            public const int ImporteRestante = 6;
            public const int CambioComisionista = 7;
            public const int CascoDeMayorValor = 8;
        }

        public class Estados
        {
            public const int Jalisco = 14;
        }

        public class Municipios
        {
            public const int ZapotlanElGrande = 655;
        }

        public class Ciudades
        {
            public const int CiudadGuzman = 132256;
        }

        public class CaracTempPartes
        {
            public const string NoPedidosPorEquivalentes = "NoPedidosPorEquivalentes";
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

        public class PedidosEstatus
        {
            public const int Surtido = 1;
            public const int NoSurtido = 2;
            public const int Cancelado = 3;
        }

        public class TiposDeOperacionMovimientos
        {
            public const int EntradaCompra = 1;
            public const int EntradaInventario = 2;
            public const int SalidaInventario = 3;
            public const int DevolucionAProveedor = 4;
            public const int Traspaso = 5;
            public const int AjusteKardex = 6;
        }

        public class Partes
        {
            public const int AnticipoClientes = 9399;
            public const int DepositoCascoChico = 3397;
            public const int DepositoCascoMediano = 3406;
            public const int DepositoCascoGrande = 3403;
            public const int DepositoCascoExtragrande = 3400;
            public const int DiferenciaDeCascos = 3434;
            public const int DepositoCascoIntermedio = 135819;
            
        }

        public class VentaDevolucionMotivos
        {
            public const int Otro = 4;
        }

        public class MovimientosConceptosDeOperacion
        {
            public const int EntradaGarantia = 10;
            public const int DevolucionGarantia = 12;
        }

        public class ContaAfectaciones
        {
            public const int VentaContadoFacturaDirecta = 1;
            public const int VentaContadoFacturaConvertida = 46;
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

        public class ContaAfectacionesCasosFijos
        {
            public const int GerenteDeSucursal = 1;
        }

        public class CuentasBancarias
        {
            public const int Scotiabank = 1;
            public const int Banamex = 2;
            public const int Serfin = 5;
        }

        public class ContaTiposDePoliza
        {
            public const int Ingreso = 1;
            public const int Egreso = 2;
            public const int Diario = 3;
        }

        public class OrigenesNotasDeCreditoProveedor
        {
            public const int Devolucion = 1;
            public const int Garantia = 2;
            public const int Descuento = 3;
        }

        public class OrigenesPagosAProveedores
        {
            public const int PagoDirecto = 1;
            public const int DescuentoFactura = 2;
            public const int DescuentoDirecto = 3;
            public const int NotaDeCredito = 4;
            public const int PagoDeCaja = 5;
        }

        public class TiposDeUsuario
        {
            public const int Vendedor = 1;
            public const int Gerente = 2;
            public const int Repartidor = 3;
        }

        public class CategoriasCorte
        {
            public const int Ventas = 1;
            public const int CancelacionesDia = 2;
            public const int CancelacionesDiasAnteriores = 3;
            public const int GarantiasDia = 4;
            public const int GarantiasDiasAnteriores = 5;
            public const int Cobranza = 6;
            public const int ValesCreados = 7;
            public const int Gastos = 8;
            public const int Resguardos = 9;
            public const int Refuerzos = 10;
        }

        public class ContaPolizaAsignacionDeSucursales
        {
            public const int Local = 1;
            public const int Matriz = 2;
            public const int DondeSeHizo = 3;
        }

        public class ContaSubcuentas
        {
            public const int ActivoFijo = 2;
        }

        public class TiposDeCliente
        {
            public const int Particular = 3;
        }

        public class TraspasosContingenciasEstatus
        {
            public const int Solucionado = 1;
            public const int NoSolucionado = 2;
        }

        public class TiposDeDescuentoCompras
        {
            public const int AItems = 1;
            public const int AFactura = 2;
            public const int Individual = 3;
            public const int AMarcaArticulo = 4;
            public const int AMarcaFactura = 5;
            public const int IndividualAFactura = 6;
        }

        public class CodigosRes
        {
            // Códigos de lógica común de Theos
            public const int ConflictoEnTraspasos = 1001;
            // Códigos de Theos Windows
            // Códigos de Theos Web
            public const int SesionCaducada = 3001;
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
