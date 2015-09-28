using System;
using System.Windows.Forms;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Autorizaciones : UserControl
    {
        // Para el Singleton *
        private static Autorizaciones instance;
        public static Autorizaciones Instance
        {
            get
            {
                if (Autorizaciones.instance == null || Autorizaciones.instance.IsDisposed)
                    Autorizaciones.instance = new Autorizaciones();
                return Autorizaciones.instance;
            }
        }
        //

        public Autorizaciones()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void Autorizaciones_Load(object sender, EventArgs e)
        {
            var oTiposAut = General.GetListOf<AutorizacionProceso>(q => q.Estatus);
            oTiposAut.Insert(0, new AutorizacionProceso() { Descripcion = "(TODAS)" });
            this.cmbTipo.CargarDatos("AutorizacionProcesoID", "Descripcion", oTiposAut);
            this.cmbTipo.SelectedIndex = 0;

            this.AplicarFiltro();
        }

        private void cmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbTipo.Focused)
                this.AplicarFiltro();
        }

        private void dgvAutorizaciones_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            int iAutorizacionID = Helper.ConvertirEntero(e.Row.Cells["AutorizacionID"].Value);
            this.LlenarDetalle(iAutorizacionID);
        }

        private void dgvAutorizaciones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvAutorizaciones.CurrentRow == null) return;
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas autorizar la tarea seleccionada?") == DialogResult.Yes)
            {
                int iAutorizacionID = Helper.ConvertirEntero(this.dgvAutorizaciones.CurrentRow.Cells["AutorizacionID"].Value);
                this.AutorizarAutorizacion(iAutorizacionID);
                this.AplicarFiltro();
            }
        }

        private void dgvAutorizaciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.dgvAutorizaciones_CellDoubleClick(sender, null);
                e.Handled = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas autorizar los registros seleccionados?") == DialogResult.Yes)
            {
                Cargando.Mostrar();

                foreach (DataGridViewRow oFila in this.dgvAutorizaciones.Rows)
                {
                    if (Helper.ConvertirBool(oFila.Cells["Autorizado"].Value))
                    {
                        int iAutorizacionID = Helper.ConvertirEntero(oFila.Cells["AutorizacionID"].Value);
                        this.AutorizarAutorizacion(iAutorizacionID);
                    }
                }
                this.AplicarFiltro();

                Cargando.Cerrar();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            this.AplicarFiltro();
        }

        #endregion

        #region [ Métodos ]

        private void AplicarFiltro()
        {
            int iAutProcesoID = Helper.ConvertirEntero(this.cmbTipo.SelectedValue);
            var oAutorizaciones = General.GetListOf<AutorizacionesView>(q =>
                !q.Autorizado
                && q.SucursalID == GlobalClass.SucursalID
                && (iAutProcesoID == 0 || q.AutorizacionProcesoID == iAutProcesoID)
            );

            // Se llena el grid
            this.dgvAutorizaciones.Rows.Clear();
            this.rtbDetalle.Clear();
            foreach (var oAut in oAutorizaciones)
                this.dgvAutorizaciones.Rows.Add(oAut.AutorizacionID, oAut.Tipo, oAut.Fecha, oAut.Autorizado, oAut.FechaAutorizo, oAut.Autorizo);
        }

        private void LlenarDetalle(int iAutorizacionID)
        {
            this.rtbDetalle.Clear();
            var oAut = General.GetEntity<Autorizacion>(q => q.AutorizacionID == iAutorizacionID && q.Estatus);
            bool bNoEncontrado = false;
            switch (oAut.AutorizacionProcesoID)
            {
                case Cat.AutorizacionesProcesos.c9500PrecioFueraDeRango:
                case Cat.AutorizacionesProcesos.c9500SinAnticipo:
                    var o9500V = General.GetEntity<Cotizaciones9500View>(q => q.Cotizacion9500ID == oAut.TablaRegistroID);
                    if (o9500V == null) { bNoEncontrado = true; break; }
                    var o9500DetalleV = General.GetListOf<Cotizaciones9500DetalleView>(q => q.Cotizacion9500ID == o9500V.Cotizacion9500ID);
                    this.LineaDetalle("Folio 9500: " + o9500V.Cotizacion9500ID.ToString());
                    this.LineaDetalle("Cliente: " + o9500V.Cliente);
                    this.LineaDetalle("Total: " + o9500V.Total.Valor().ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Anticipo: " + o9500V.Anticipo.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Vendedor: " + o9500V.Vendedor);

                    this.LineaDetalle("");
                    this.LineaDetalle("Detalle:");
                    this.LineaDetalle("Parte        Cant. Descripción                      Costo      Precio    ");
                    foreach (var oDet in o9500DetalleV)
                        this.LineaDetalle(
                            oDet.NumeroParte.Izquierda(12).PadRight(12)
                            + " " + oDet.Cantidad.ToString().Izquierda(5).PadRight(5)
                            + " " + oDet.NombreParte.Izquierda(32).PadRight(32)
                            + " " + oDet.Costo.ToString(GlobalClass.FormatoMoneda).Izquierda(10).PadLeft(10)
                            + " " + oDet.PrecioAlCliente.ToString(GlobalClass.FormatoMoneda).Izquierda(10).PadLeft(10)
                        );
                    break;
                case Cat.AutorizacionesProcesos.CorteDeCaja:
                    var oCorte = General.GetEntity<CajaEfectivoPorDiaView>(q => q.CajaEfectivoPorDiaID == oAut.TablaRegistroID);
                    if (oCorte == null) { bNoEncontrado = true; break; }
                    this.LineaDetalle("Día: " + oCorte.Dia.ToLongDateString());
                    this.LineaDetalle("Debía haber: " + oCorte.CierreDebeHaber.Valor().ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Cierre: " + oCorte.Cierre.Valor().ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Diferencia: " + (oCorte.Cierre.Valor() - oCorte.CierreDebeHaber.Valor()).ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Usuario: " + oCorte.UsuarioCierre);
                    break;
                case Cat.AutorizacionesProcesos.CreditoNoAplicable:
                    var oVentaCNA = General.GetEntity<VentasView>(q => q.VentaID == oAut.TablaRegistroID);
                    if (oVentaCNA == null) { bNoEncontrado = true; break; }
                    var oClienteCredito = General.GetEntity<ClientesCreditoView>(q => q.ClienteID == oVentaCNA.ClienteID);
                    this.LineaDetalle("Venta: " + oVentaCNA.Folio);
                    this.LineaDetalle("Cliente: " + oVentaCNA.Cliente);
                    this.LineaDetalle("Límite de crédito: " + oClienteCredito.LimiteDeCredito);
                    this.LineaDetalle("Días de crédito: " + oClienteCredito.DiasDeCredito);
                    this.LineaDetalle("Adeudo: " + oClienteCredito.Adeudo.Valor().ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Adeudo más antiguo: " + oClienteCredito.FechaPrimerAdeudo.Valor().ToString());
                    break;
                case Cat.AutorizacionesProcesos.DevolucionCancelacion:
                    var oDev = General.GetEntity<VentasDevolucionesView>(q => q.VentaDevolucionID == oAut.TablaRegistroID);
                    if (oDev == null) { bNoEncontrado = true; break; }
                    var oVentaDC = General.GetEntity<VentasView>(q => q.VentaID == oDev.VentaID);
                    var oDevDet = General.GetListOf<VentasDevolucionesDetalleView>(q => q.VentaDevolucionID == oDev.VentaDevolucionID);
                    string sTipo = (oDev.EsCancelacion ? "Cancelación" : "Devolución");
                    this.LineaDetalle("Tipo: " + sTipo);
                    this.LineaDetalle("Venta: " + oDev.FolioDeVenta);
                    this.LineaDetalle("Cliente: " + oVentaDC.Cliente);
                    this.LineaDetalle("Motivo: " + oDev.Motivo);
                    this.LineaDetalle("Observación: " + oDev.Observacion);
                    this.LineaDetalle("Usuario: " + oDev.Realizo);

                    this.LineaDetalle("");
                    this.LineaDetalle("Detalle de " + sTipo + ":");
                    this.LineaDetalle("Parte        Cant. Descripción");
                    foreach (var oDet in oDevDet)
                        this.LineaDetalle(
                            oDet.NumeroParte.Izquierda(12).PadRight(12)
                            + " " + oDet.Cantidad.ToString().Izquierda(5).PadRight(5)
                            + " " + oDet.NombreParte.Izquierda(32).PadRight(32)
                        );
                    break;
                case Cat.AutorizacionesProcesos.Garantia:
                    var oGarantiaV = General.GetEntity<VentasGarantiasView>(c => c.VentaGarantiaID == oAut.TablaRegistroID);
                    if (oGarantiaV == null) { bNoEncontrado = true; break; }
                    var oVentaGC = General.GetEntity<VentasView>(q => q.VentaID == oGarantiaV.VentaID);
                    this.LineaDetalle("Venta: " + oGarantiaV.FolioDeVenta);
                    this.LineaDetalle("Cliente: " + oVentaGC.Cliente);
                    this.LineaDetalle("Fecha: " + oGarantiaV.Fecha);
                    this.LineaDetalle("Motivo: " + oGarantiaV.Motivo);
                    this.LineaDetalle("Motivo Obs.: " + oGarantiaV.MotivoObservacion);
                    this.LineaDetalle("Acción: " + oGarantiaV.Accion);
                    this.LineaDetalle("Observación final: " + oGarantiaV.ObservacionCompletado);
                    this.LineaDetalle("Usuario: " + oGarantiaV.Realizo);
                    break;
                case Cat.AutorizacionesProcesos.FondoDeCajaDiferencia:
                    var oFondo = General.GetEntity<CajaEfectivoPorDiaView>(q => q.CajaEfectivoPorDiaID == oAut.TablaRegistroID);
                    if (oFondo == null) { bNoEncontrado = true; break; }
                    // Se obtiene el registro del día anterior, para obtener el importe del cierre
                    var oDias = General.GetListOf<CajaEfectivoPorDia>(q => q.CajaEfectivoPorDiaID < oFondo.CajaEfectivoPorDiaID && q.Estatus);
                    int iAnteriorID = (oDias.Count > 0 ? oDias.Max(r => r.CajaEfectivoPorDiaID) : 0);
                    var oCorteAnt = General.GetEntity<CajaEfectivoPorDiaView>(q => q.CajaEfectivoPorDiaID == iAnteriorID);
                    //
                    this.LineaDetalle("Día: " + oFondo.Dia.ToLongDateString());
                    this.LineaDetalle("Cierre anterior: " + (oCorteAnt == null ? "" : oCorteAnt.Dia.ToLongDateString()));
                    this.LineaDetalle("Importe cierre: " + (oCorteAnt == null ? "" : oCorteAnt.Cierre.Valor().ToString(GlobalClass.FormatoMoneda)));
                    this.LineaDetalle("Fondo de caja: " + oFondo.Inicio.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Diferencia: " + (oFondo.Inicio - (oCorteAnt == null ? 0 : oCorteAnt.Cierre.Valor())).ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Usuario: " + oFondo.UsuarioInicio);
                    break;
                case Cat.AutorizacionesProcesos.Gastos:
                case Cat.AutorizacionesProcesos.Resguardo:
                    var oGastoV = General.GetEntity<CajaEgresosView>(q => q.CajaEgresoID == oAut.TablaRegistroID);
                    if (oGastoV == null) { bNoEncontrado = true; break; }
                    this.LineaDetalle("Tipo: " + oGastoV.Tipo);

                    // Parche para ver poner la cuenta contable, en lugar del tipo de egreso de caja, cuando aplica
                    var oGasto = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == oGastoV.CajaEgresoID);
                    if (oGasto.ContaEgresoID > 0)
                    {
                        var oContaEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == oGasto.ContaEgresoID);
                        var oCuentaAuxV = General.GetEntity<ContaCuentasAuxiliaresView>(c => c.ContaCuentaAuxiliarID == oContaEgreso.ContaCuentaAuxiliarID);
                        this.LineaDetalle("Cuenta Auxiliar: " + oCuentaAuxV.CuentaAuxiliar);
                        this.LineaDetalle("Cuenta de Mayor: " + oCuentaAuxV.CuentaDeMayor);
                    }
                    //

                    this.LineaDetalle("Fecha: " + oGastoV.Fecha.ToString());
                    this.LineaDetalle("Importe: " + oGastoV.Importe.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Concepto: " + oGastoV.Concepto);
                    break;
                case Cat.AutorizacionesProcesos.GastosBorrar:
                    // No se hace nada, pues el registro ya fue borrado
                    bNoEncontrado = true;
                    break;
                case Cat.AutorizacionesProcesos.OtrosIngresos:
                case Cat.AutorizacionesProcesos.Refuerzo:
                    var oIngreso = General.GetEntity<CajaIngresosView>(q => q.CajaIngresoID == oAut.TablaRegistroID);
                    if (oIngreso == null) { bNoEncontrado = true; break; }
                    this.LineaDetalle("Tipo: " + oIngreso.Tipo);
                    this.LineaDetalle("Fecha: " + oIngreso.Fecha.ToString());
                    this.LineaDetalle("Importe: " + oIngreso.Importe.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Concepto: " + oIngreso.Concepto);
                    break;
                case Cat.AutorizacionesProcesos.OtrosIngresosBorrar:
                    // No se hace nada, pues el registro ya fue borrado
                    bNoEncontrado = true;
                    break;
                case Cat.AutorizacionesProcesos.NotaDeCreditoOtroClienteUsar:
                case Cat.AutorizacionesProcesos.NotaDeCreditoCrear:
                    var oNotaV = General.GetEntity<NotasDeCreditoView>(q => q.NotaDeCreditoID == oAut.TablaRegistroID);
                    if (oNotaV == null) { bNoEncontrado = true; break; }
                    this.LineaDetalle("Nota de crédito: " + oNotaV.NotaDeCreditoID.ToString());
                    this.LineaDetalle("Cliente: " + oNotaV.Cliente);
                    this.LineaDetalle("Fecha de emisión: " + oNotaV.FechaDeEmision.ToString());
                    this.LineaDetalle("Venta de origen: " + oNotaV.OrigenVentaFolio);
                    this.LineaDetalle("Importe: " + oNotaV.Importe.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Válida: " + (oNotaV.Valida ? "Sí" : "No"));
                    this.LineaDetalle("Fecha de uso: " + oNotaV.FechaDeUso.ToString());
                    this.LineaDetalle("Venta de uso: " + oNotaV.UsoVentaFolio);
                    this.LineaDetalle("Observación: " + oNotaV.Observacion);
                    break;
                case Cat.AutorizacionesProcesos.VentaCambio:
                    var oVentaCambio = General.GetEntity<VentasCambiosView>(q => q.VentaCambioID == oAut.TablaRegistroID);
                    if (oVentaCambio == null) { bNoEncontrado = true; break; }
                    this.LineaDetalle("Venta: " + oVentaCambio.VentaFolio);
                    this.LineaDetalle("Cliente: " + oVentaCambio.Cliente);
                    this.LineaDetalle("Fecha: " + oVentaCambio.Fecha.ToString());
                    this.LineaDetalle("Cliente: " + oVentaCambio.Cliente);

                    this.LineaDetalle("");
                    this.LineaDetalle("Detalle del cambio: ");
                    this.LineaDetalle("Característica Antes            Después");
                    this.LineaDetalle("Forma de pago".PadRight(14)
                        + " " + oVentaCambio.FormasDePagoAntes.Valor().Izquierda(16).PadRight(16)
                        + " " + oVentaCambio.FormasDePagoDespues.Valor().Izquierda(16).PadRight(16)
                    );
                    this.LineaDetalle("Vendedor".PadRight(14)
                        + " " + oVentaCambio.VendedorAntes.Valor().Izquierda(16).PadRight(16)
                        + " " + oVentaCambio.VendedorDespues.Valor().Izquierda(16).PadRight(16)
                    );
                    this.LineaDetalle("Comisionista".PadRight(14)
                        + " " + oVentaCambio.ComisionistaAntes.Valor().Izquierda(16).PadRight(16)
                        + " " + oVentaCambio.ComisionistaDespues.Valor().Izquierda(16).PadRight(16)
                    );
                    break;
                case Cat.AutorizacionesProcesos.NotaDeCreditoFiscalCrear:
                    var oNotaFiscalV = General.GetEntity<NotasDeCreditoFiscalesView>(c => c.NotaDeCreditoFiscalID == oAut.TablaRegistroID);
                    if (oNotaFiscalV == null) { bNoEncontrado = true; break; }
                    this.LineaDetalle("Nota de crédito: " + oNotaFiscalV.NotaDeCreditoFiscalID.ToString());
                    this.LineaDetalle("Cliente: " + oNotaFiscalV.Cliente);
                    this.LineaDetalle("Fecha: " + oNotaFiscalV.Fecha.ToString());
                    this.LineaDetalle("Sucursal: " + oNotaFiscalV.Sucursal);
                    this.LineaDetalle("Folio: " + (oNotaFiscalV.Serie + oNotaFiscalV.Folio));
                    this.LineaDetalle("Subtotal: " + oNotaFiscalV.Subtotal.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Iva: " + oNotaFiscalV.Iva.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Total: " + oNotaFiscalV.Total.ToString(GlobalClass.FormatoMoneda));
                    this.LineaDetalle("Usuario: " + oNotaFiscalV.Usuario);
                    break;
            }

            if (bNoEncontrado)
                this.LineaDetalle("No se encontró información de la autorización seleccionada.");
        }

        private void LineaDetalle(string sLinea)
        {
            this.rtbDetalle.AppendText(sLinea.ToUpper() + "\n");
        }

        private void AutorizarAutorizacion(int iAutorizacionID)
        {
            var oAut = General.GetEntity<Autorizacion>(q => q.AutorizacionID == iAutorizacionID && q.Estatus);

            // Se valida que se tenga permiso para autorizar
            var oAutPro = General.GetEntity<AutorizacionProceso>(c => c.AutorizacionProcesoID == oAut.AutorizacionProcesoID && c.Estatus);
            if (!UtilDatos.ValidarPermiso(oAutPro.Permiso, true))
                return;
            
            oAut.Autorizado = true;
            oAut.AutorizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
            oAut.FechaAutorizo = DateTime.Now;
            Guardar.Generico<Autorizacion>(oAut);
        }

        #endregion
                
    }
}
