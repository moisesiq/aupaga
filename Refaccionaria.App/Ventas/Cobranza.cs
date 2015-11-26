using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using FastReport;
using System.Data;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Cobranza : UserControl
    {
        public VentasCobranza oVentasCobranza;

        bool FiltroDeFechas = false;

        public Cobranza()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public ClientesDatosView Cliente { get; set; }

        public decimal Total
        {
            get
            {
                decimal mTotal = 0;
                foreach (DataGridViewRow Fila in this.dgvVentas.Rows)
                {
                    if (Helper.ConvertirBool(Fila.Cells["vPagar"].Value))
                        mTotal += Helper.ConvertirDecimal(Fila.Cells["vSaldo"].Value);
                }
                return mTotal;
            }
        }

        #endregion

        #region [ Eventos ]

        private void Cobranza_Load(object sender, EventArgs e)
        {
            // Se cargan las ventas por pagar
            this.AplicarFiltro();

            // Se asignan valores predeterminados
            this.dtpDesde.Value = DateTime.Now.DiaPrimero();
            this.dtpHasta.Value = DateTime.Now.DiaUltimo();

            // Se llenan los totales
            // this.CalcularTotales();  // Se manda llamar desde el "CambiarCliente" incial

            // Se cargan valores predeterminados para notas de crédito
            this.dtpNcDesde.Value = this.dtpDesde.Value;
            this.dtpNcHasta.Value = this.dtpHasta.Value;
            
            // Vencimientos
            this.dgvVencimientosTotales.Rows.Add("TOTALES");

            // Notas de crédito fiscales
            this.cmbNcf_Sucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.dtpNcf_Desde.Value = DateTime.Now.DiaPrimero();
            this.dtpNcf_Hasta.Value = DateTime.Now.DiaUltimo();
            this.Ncf_ListaDePrecios.Items.AddRange("1", "2", "3", "4", "5");

            // Se cargan las notas de crédito - Ya no, ahora se cargan al seleccionar esa opción en el tab page
            // this.NcAplicarFiltro();
        }

        private void tabCobranza_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabCobranza.SelectedTab.Name)
            {
                case "tbpVales":
                    this.NcAplicarFiltro();
                    break;
                case "tbpVencimientos":
                    this.LlenarVencimientos();
                    break;
            }

        }

        private void dgvVentas_KeyDown(object sender, KeyEventArgs e)
        {
            this.dgvVentas.VerShift(e, "vPagar");
        }

        private void dgvVentas_CurrentCellChanged(object sender, EventArgs e)
        {
            int iVentaID;
            if (this.dgvVentas.CurrentRow == null)
                iVentaID = 0;
            else
                iVentaID = Helper.ConvertirEntero(this.dgvVentas.CurrentRow.Cells["vVentaID"].Value);
            
            // Se llenan los datos de los pagos
            this.LlenarPagos(iVentaID);
        }

        private void dgvVentas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!this.dgvVentas.IsCurrentCellDirty) return;
            if (this.dgvVentas.CurrentCell.OwningColumn.Name == "vPagar")
                this.dgvVentas.EndEdit();
        }

        private void dgvVentas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvVentas.CurrentRow == null) return;

            if (this.dgvVentas[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "vPagar")
                this.FilaMarcaCambiada(this.dgvVentas.CurrentRow);
        }

        private void chkTodos_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow Fila in this.dgvVentas.Rows)
                Fila.Cells["vPagar"].Value = this.chkTodos.Checked;
        }

        private void chkMostrarTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.chkMostrarTodos.Focused) return;
            this.AplicarFiltro();
        }

        private void dtpDesde_ValueChanged(object sender, EventArgs e)
        {
            if (!this.dtpDesde.Focused) return;
            this.FiltroDeFechas = true;
            this.AplicarFiltro();
        }

        private void dtpHasta_ValueChanged(object sender, EventArgs e)
        {
            if (!this.dtpHasta.Focused) return;
            this.FiltroDeFechas = true;
            this.AplicarFiltro();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            var oVentas = this.ObtenerVentasMarcadas();
            var oClienteV = General.GetEntity<ClientesDatosView>(q => q.ClienteID == this.Cliente.ClienteID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "Cobranza.frx");
            oRep.RegisterData(new List<ClientesDatosView>() { oClienteV }, "Cliente");
            oRep.RegisterData(oVentas, "Ventas");
            UtilLocal.EnviarReporteASalida("Reportes.Cobranza.Salida", oRep);
        }

        private void dgvNotasDeCredito_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvNotasDeCredito.CurrentRow == null) return;

            if (e.KeyCode == Keys.Delete)
            {
                // Se verifica la nota de crédito seleccionada
                int iNotaDeCreditoID = Helper.ConvertirEntero(this.dgvNotasDeCredito.CurrentRow.Cells["ncNotaDeCreditoID"].Value);
                var oNota = General.GetEntity<NotaDeCredito>(q => q.NotaDeCreditoID == iNotaDeCreditoID && q.Estatus);
                if (oNota.Valida == false)
                {
                    UtilLocal.MensajeAdvertencia("La nota de crédito seleccionada ya no es válida. No se puede continuar.");
                    return;
                }

                if (UtilLocal.MensajePregunta("¿Estás seguro que deseas dar de baja la nota de crédito seleccionada?") == DialogResult.Yes)
                {
                    // Se solicita el motivo
                    var oMotivo = UtilLocal.ObtenerValor("Indica el motivo de la baja", "", MensajeObtenerValor.Tipo.TextoLargo);
                    if (oMotivo != null)
                    {
                        // Se solicita la validación de usuario
                        var ResU = UtilDatos.ValidarObtenerUsuario("Ventas.NotasDeCredito.Baja");
                        if (ResU.Exito)
                        {
                            oNota.Valida = false;
                            oNota.MotivoBaja = Helper.ConvertirCadena(oMotivo);
                            Guardar.Generico<NotaDeCredito>(oNota);
                            this.NcAplicarFiltro();
                        }
                        else
                        {
                            if (ResU.Respuesta != null)
                                UtilLocal.MensajeAdvertencia("No tienes permisos para realizar la operación solicitada.");
                        }
                    }
                }
            }
        }

        private void dtpNcDesde_ValueChanged(object sender, EventArgs e)
        {
            if (!this.dtpNcDesde.Focused) return;
            this.NcAplicarFiltro();
        }

        private void dtpNcHasta_ValueChanged(object sender, EventArgs e)
        {
            if (!this.dtpNcHasta.Focused) return;
            this.NcAplicarFiltro();
        }

        private void chkNcMostrarTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.chkNcMostrarTodos.Focused) return;
            this.NcAplicarFiltro();
        }
        
        private void btnNcAgregar_Click(object sender, EventArgs e)
        {
            // Se solicita el concepto e importe de la nota de crédito
            string sConcepto = Helper.ConvertirCadena(UtilLocal.ObtenerValor("Concepto de la Nota de Crédito:", "", MensajeObtenerValor.Tipo.TextoLargo));
            if (sConcepto == "") return;
            decimal mImporte = Helper.ConvertirDecimal(UtilLocal.ObtenerValor("Importe de la Nota de Crédito:", "0.00", MensajeObtenerValor.Tipo.Decimal));
            if (mImporte == 0) return;
            // Se solicita la autorización
            var ResAut = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.NotasDeCredito.Agregar", "Autorización");
            // Se genera la nota, si todo fue bien
            if (ResAut.Exito)
            {
                var ResNC = VentasProc.GenerarNotaDeCredito(this.Cliente.ClienteID, mImporte, sConcepto, Cat.OrigenesNotaDeCredito.Directo, ResAut.Respuesta.UsuarioID);
                // Se manda a crear la póliza contable correspondiente (AfeConta)
                var oVale = General.GetEntity<NotaDeCredito>(c => c.NotaDeCreditoID == ResNC.Respuesta && c.Estatus);
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.ValeDirecto, oVale.NotaDeCreditoID, this.Cliente.Nombre, oVale.Observacion);
                // Se guarda la autorización
                VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.NotaDeCreditoCrear, Cat.Tablas.NotaDeCredito, ResNC.Respuesta, ResAut.Respuesta.UsuarioID);
                // Se manda imprimir el ticket correspondiente
                VentasProc.GenerarTicketNotaDeCredito(ResNC.Respuesta);
                //
                UtilLocal.MostrarNotificacion("Nota de Crédito generada correctamente.");
                this.NcAplicarFiltro();
            }
        }

        private void txtReimpresion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.txtReimpresion.SelectAll();
                string sFolioCob = this.txtReimpresion.Text;
                var oCobranza = General.GetEntity<CobranzaTicket>(q => q.Ticket == sFolioCob);
                if (oCobranza == null)
                {
                    UtilLocal.MensajeAdvertencia("El folio de Cobranza especificado no existe.");
                    return;
                }
                VentasProc.GenerarTicketCobranza(sFolioCob);
            }
        }

        private void dgvVencimientos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvVencimientos.CurrentRow == null) return;
            int iClienteID = Helper.ConvertirEntero(this.dgvVencimientos.CurrentRow.Cells["ClienteID"].Value);
            this.LlenarVencimientoDetalle(iClienteID);
        }

        private void btnVenExportar_Click(object sender, EventArgs e)
        {
            UtilLocal.AbrirEnExcel(this.dgvVencimientos);
        }

        private void btnNcf_Actualizar_Click(object sender, EventArgs e)
        {
            this.NcfLlenarFacturas();
        }

        private void btnNcf_CrearNcf_Click(object sender, EventArgs e)
        {
            this.Ncf_CrearNotaDeCreditoFiscal();
        }

        private void dgvNcf_Facturas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvNcf_Facturas.VerDirtyStateChanged("Ncf_ListaDePrecios");
        }

        private void dgvNcf_Facturas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvNcf_Facturas.Columns[e.ColumnIndex].Name == "Ncf_ListaDePrecios")
            {
                this.Ncf_CalcularImporte();
            }
            else if (dgvNcf_Facturas.Columns[e.ColumnIndex].Name == "Ncf_ImporteNuevo")
            {
                this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_Diferencia"].Value = (Helper.ConvertirDecimal(this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_Importe"].Value)
                    - Helper.ConvertirDecimal(this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_ImporteNuevo"].Value));
            }
        }
                
        #endregion

        #region [ Métodos ]

        private void LlenarPagos(int iVentaID)
        {
            var oPagos = General.GetListOf<VentasPagosView>(q => q.VentaID == iVentaID);
            this.dgvAbonos.Rows.Clear();
            string sFormaDePago, sVales = "";
            foreach (var oPago in oPagos)
            {
                // Se obtiene el folio de cobranza, según el pago
                var oCobranzaT = General.GetEntity<CobranzaTicket>(c => c.VentaPagoID == oPago.VentaPagoID);
                string sTicket = (oCobranzaT == null ? "" : oCobranzaT.Ticket);
                //
                sFormaDePago = UtilDatos.VentaPagoFormasDePago(oPago.VentaPagoID);
                if (sFormaDePago.Contains("NC"))
                    sVales = UtilDatos.VentaPagoVales(oPago.VentaPagoID);
                this.dgvAbonos.Rows.Add(oPago.Fecha, sFormaDePago, sVales, oPago.VentaPagoID, oPago.Importe, sTicket);
            }
        }

        private void AplicarFiltro()
        {
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            DateTime dDesde = this.dtpDesde.Value.Date;
            DateTime dHasta = this.dtpHasta.Value.Date.AddDays(1);

            var oVentasAC = General.GetListOf<VentasACreditoView>(q =>
                q.ClienteID == iClienteID
                && (this.chkMostrarTodos.Checked
                    || (q.Restante > 0
                        && (!this.FiltroDeFechas
                            || (q.Fecha >= dDesde && q.Fecha < dHasta)
                        )
                    )
                )
            ).OrderBy(q => q.Fecha);

            this.dgvVentas.Rows.Clear();
            foreach (var oVentaAC in oVentasAC)
            {
                int iFila = this.dgvVentas.Rows.Add(oVentaAC.VentaID, true, oVentaAC.Folio, oVentaAC.Fecha, oVentaAC.Vencimiento, 
                    oVentaAC.Total, oVentaAC.Pagado, oVentaAC.Restante);
                // Si la venta ya fue pagada, se desmarca y se deshabilita el checkbox
                if (Helper.ConvertirDecimal(this.dgvVentas["vSaldo", iFila].Value) <= 0)
                {
                    this.dgvVentas["vPagar", iFila].Value = false;
                    this.dgvVentas["vPagar", iFila].ReadOnly = true;
                }
            }
            this.chkTodos.Checked = true;

            // Se establece el total a pagar
            this.oVentasCobranza.ctlCobro.Total = this.Total;
            if (this.Cliente == null)
                this.oVentasCobranza.ctlCobro.LimpiarFormasDePago();
            else
                this.oVentasCobranza.ctlCobro.EstablecerFormaDePagoPredeterminada(this.Cliente.TipoFormaPagoID.Valor(),
                    this.Total, this.Cliente.BancoID.Valor(), this.Cliente.CuentaBancaria);
        }

        private void CalcularTotales()
        {
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            var oVentasAC = General.GetListOf<VentasACreditoView>(q => q.ClienteID == iClienteID && q.Restante > 0);

            decimal mTotal = 0, mVencido = 0;
            foreach (var oVentaAC in oVentasAC)
            {
                mTotal += oVentaAC.Restante;
                if (oVentaAC.Vencimiento.Valor().Date > DateTime.Today)
                    continue;
                mVencido += oVentaAC.Restante;
            }
            this.lblTotal.Text = mTotal.ToString(GlobalClass.FormatoMoneda);
            this.lblVencido.Text = mVencido.ToString(GlobalClass.FormatoMoneda);
        }

        private void NcAplicarFiltro()
        {
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            DateTime dDesde = this.dtpNcDesde.Value.Date;
            DateTime dHasta = this.dtpNcHasta.Value.Date.AddDays(1);
            var oNotasDeCredito = General.GetListOf<NotasDeCreditoView>(q =>
                q.ClienteID == iClienteID
                && (this.chkNcMostrarTodos.Checked
                || (q.Valida
                && q.FechaDeEmision >= dDesde
                && q.FechaDeEmision < dHasta))
            );

            this.dgvNotasDeCredito.Rows.Clear();
            foreach (var oNota in oNotasDeCredito)
                this.dgvNotasDeCredito.Rows.Add(oNota.NotaDeCreditoID, oNota.FechaDeEmision, oNota.Origen, 
                    oNota.RelacionID, oNota.Importe, oNota.UsoVentaFolio, oNota.FechaDeUso, oNota.Observacion);
        }

        private void FilaMarcaCambiada(DataGridViewRow Fila)
        {
            this.oVentasCobranza.ctlCobro.Total = this.Total;
            this.oVentasCobranza.ctlCobro.EstablecerFormaDePagoPredeterminada(this.Cliente.TipoFormaPagoID.Valor(),
                this.Total, this.Cliente.BancoID.Valor(), this.Cliente.CuentaBancaria);
        }

        private void LlenarVencimientos()
        {
            Cargando.Mostrar();

            DateTime dAhora = DateTime.Now;
            var oVentasC = General.GetListOf<VentasACreditoView>(c => dAhora > c.Vencimiento && c.Restante > 0).OrderBy(o => o.Cliente);
            this.dgvVencimientos.Rows.Clear();

            int iClienteID = 0;
            string sCliente = "";
            decimal mSem1, mSem2, mSem3, mSem4, mMes;
            mSem1 = mSem2 = mSem3 = mSem4 = mMes = 0;
            bool bPrimeraVuelta = true;
            foreach (var oVenta in oVentasC)
            {
                // Se verifica si es la primera vuelta, para no agregar nada pues falta ver si la siguiente es el mismo cliente
                if (bPrimeraVuelta)
                {
                    iClienteID = oVenta.ClienteID;
                    sCliente = oVenta.Cliente;
                    bPrimeraVuelta = false;
                }
                //
                if (oVenta.ClienteID != iClienteID)
                {
                    // Se agrega el cliente al grid
                    this.dgvVencimientos.Rows.Add(iClienteID, sCliente, (mSem1 + mSem2 + mSem3 + mSem4 + mMes), mSem1, mSem2, mSem3, mSem4, mMes);
                    // Se restauran los valores con el siguiente cliente
                    iClienteID = oVenta.ClienteID;
                    sCliente = oVenta.Cliente;
                    mSem1 = mSem2 = mSem3 = mSem4 = mMes = 0;
                }
                // Se calculan los importes de vencimiento por períodos de tiempo
                int iDiasVencido = (dAhora - oVenta.Vencimiento.Valor()).Days;
                if (iDiasVencido < 8)
                    mSem1 += oVenta.Restante;
                else if (iDiasVencido < 15)
                    mSem2 += oVenta.Restante;
                else if (iDiasVencido < 22)
                    mSem3 += oVenta.Restante;
                else if (iDiasVencido < 29)
                    mSem4 += oVenta.Restante;
                else
                    mMes += oVenta.Restante;
            }
            // Se agrega el último cliente, si aplica
            if (iClienteID > 0)
                this.dgvVencimientos.Rows.Add(iClienteID, sCliente, (mSem1 + mSem2 + mSem3 + mSem4 + mMes), mSem1, mSem2, mSem3, mSem4, mMes);

            // Se ordena según 
            var oListaGrid = new[] { new { ClienteID = 1, Cliente = "uno", Vencido = 1.1
                , UnaSem = 1.1, DosSem = 1.1, TresSem = 1.1, CuatroSem = 1.1, MasMes = 1.1 } }.ToList();
            oListaGrid.Clear();
            foreach (DataGridViewRow oFila in this.dgvVencimientos.Rows)
                oListaGrid.Add(new
                {
                    ClienteID = Helper.ConvertirEntero(oFila.Cells["ClienteID"].Value),
                    Cliente = Helper.ConvertirCadena(oFila.Cells["colCliente"].Value),
                    Vencido = Helper.ConvertirDoble(oFila.Cells["Vencido"].Value),
                    UnaSem = Helper.ConvertirDoble(oFila.Cells["UnaSem"].Value),
                    DosSem = Helper.ConvertirDoble(oFila.Cells["DosSem"].Value),
                    TresSem = Helper.ConvertirDoble(oFila.Cells["TresSem"].Value),
                    CuatroSem = Helper.ConvertirDoble(oFila.Cells["CuatroSem"].Value),
                    MasMes = Helper.ConvertirDoble(oFila.Cells["MasMes"].Value)
                });
            oListaGrid = oListaGrid.OrderByDescending(c => c.MasMes).ThenByDescending(c => c.CuatroSem).ThenByDescending(c => c.TresSem)
                .ThenByDescending(c => c.DosSem).ThenByDescending(c => c.UnaSem).ToList();

            // Se llenan los totales
            for (int iCol = 1; iCol < this.dgvVencimientosTotales.Columns.Count; iCol++)
                this.dgvVencimientosTotales[iCol, 0].Value = 0;
            if (oListaGrid.Count > 0)
            {
                this.dgvVencimientosTotales["tvVencido", 0].Value = oListaGrid.Sum(c => c.Vencido);
                this.dgvVencimientosTotales["tvUnaSem", 0].Value = oListaGrid.Sum(c => c.UnaSem);
                this.dgvVencimientosTotales["tvDosSem", 0].Value = oListaGrid.Sum(c => c.DosSem);
                this.dgvVencimientosTotales["tvTresSem", 0].Value = oListaGrid.Sum(c => c.TresSem);
                this.dgvVencimientosTotales["tvCuatroSem", 0].Value = oListaGrid.Sum(c => c.CuatroSem);
                this.dgvVencimientosTotales["tvMasMes", 0].Value = oListaGrid.Sum(c => c.MasMes);
            }

            // var oDatos = this.dgvVencimientos.ADataTable();
            this.dgvVencimientos.Rows.Clear();
            foreach (var oReg in oListaGrid)
            {
                this.dgvVencimientos.Rows.Add(oReg.ClienteID, oReg.Cliente, oReg.Vencido, oReg.UnaSem, oReg.DosSem, oReg.TresSem, oReg.CuatroSem, oReg.MasMes);
            }

            Cargando.Cerrar();
        }

        private void LlenarVencimientoDetalle(int iClienteID)
        {
            DateTime dAhora = DateTime.Now;
            var oVentasC = General.GetListOf<VentasACreditoView>(c => c.ClienteID == iClienteID && dAhora > c.Vencimiento && c.Restante > 0).OrderBy(o => o.Cliente);
            this.dgvVentasVen.Rows.Clear();
            foreach (var oVenta in oVentasC)
                this.dgvVentasVen.Rows.Add(oVenta.Folio, oVenta.Fecha, oVenta.Vencimiento, oVenta.Total, oVenta.Pagado, oVenta.Restante);
        }

        private void NcfLlenarFacturas()
        {
            Cargando.Mostrar();

            // Se obtienen los datos, en base al filtro
            int iClienteID = (this.Cliente == null ? 0 : this.Cliente.ClienteID);
            int iSucursalID = Helper.ConvertirEntero(this.cmbNcf_Sucursal.SelectedValue);
            DateTime dDesde = this.dtpNcf_Desde.Value.Date;
            DateTime dHasta = this.dtpNcf_Hasta.Value.Date.AddDays(1);
            bool bTodasLasVentas = (this.chkNcf_TodasLasVentas.Checked);

            var oLista = General.GetListOf<VentasView>(c =>
                c.ClienteID == iClienteID
                && c.Facturada
                && c.VentaEstatusID == Cat.VentasEstatus.Cobrada
                && c.Pagado <= 0
                && (bTodasLasVentas || iSucursalID == 0 || c.SucursalID == iSucursalID)
                && c.Fecha >= dDesde
                && c.Fecha < dHasta
            );

            // Se empiezan a llenar los datos
            this.dgvNcf_Facturas.Rows.Clear();
            foreach (var oReg in oLista)
                this.dgvNcf_Facturas.Rows.Add(oReg.VentaID, false, oReg.Fecha, oReg.Folio, oReg.Sucursal, oReg.Total - oReg.Pagado);

            Cargando.Cerrar();
        }

        private void Ncf_CalcularImporte()
        {
            int iListaDeP = Helper.ConvertirEntero(this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_ListaDePrecios"].Value);
            int iVentaID = Helper.ConvertirEntero(this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_VentaID"].Value);
            var oVentaDet = General.GetListOf<VentaDetalle>(c => c.VentaID == iVentaID && c.Estatus);
            decimal mTotal = 0;
            foreach (var oReg in oVentaDet)
            {
                var oPrecio = General.GetEntity<PartePrecio>(c => c.ParteID == oReg.ParteID && c.Estatus);
                switch (iListaDeP)
                {
                    case 1: mTotal += (oPrecio.PrecioUno.Valor() * oReg.Cantidad); break;
                    case 2: mTotal += (oPrecio.PrecioDos.Valor() * oReg.Cantidad); break;
                    case 3: mTotal += (oPrecio.PrecioTres.Valor() * oReg.Cantidad); break;
                    case 4: mTotal += (oPrecio.PrecioCuatro.Valor() * oReg.Cantidad); break;
                    case 5: mTotal += (oPrecio.PrecioCinco.Valor() * oReg.Cantidad); break;
                }
            }

            this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_ImporteNuevo"].Value = mTotal;
            this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_Diferencia"].Value = (Helper.ConvertirDecimal(this.dgvNcf_Facturas.CurrentRow.Cells["Ncf_Importe"].Value) - mTotal);
        }

        private void Ncf_CrearNotaDeCreditoFiscal()
        {
            // Se verifica si hay ventas seleccionadas
            if (this.dgvNcf_Facturas.ContarIncidencias("Ncf_Aplicar", true) == 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna venta seleccionada.");
                return;
            }

            // Se obtiene el concepto
            var oConcepto = UtilLocal.ObtenerValor("Concepto de Nota de Crédito:", "", MensajeObtenerValor.Tipo.TextoLargo);
            if (oConcepto == null) return;

            // Se valida el permiso
            var oResU = UtilDatos.ValidarObtenerUsuario("Ventas.NotasDeCreditoFiscales.Agregar");
            if (oResU.Error)
                return;
            int iUsuarioID = oResU.Respuesta.UsuarioID;
            // Se solicita la validación de autorización
            var oResA = UtilDatos.ValidarObtenerUsuario("Autorizaciones.Ventas.NotasDeCreditoFiscales.Agregar", "Autorización");
            int? iAutorizoID = (oResA.Respuesta == null ? null : (int?)oResA.Respuesta.UsuarioID);

            // Se genera el detalle de la nota de crédito, para mandar a hacer la factura
            decimal mTotal = 0;
            var oNotaDetalle = new List<ProductoVenta>();
            oNotaDetalle.Add(new ProductoVenta() { NombreDeParte = Helper.ConvertirCadena(oConcepto), UnidadDeMedida = "." });
            // Se meten las facturas afectadas
            foreach (DataGridViewRow oFila in this.dgvNcf_Facturas.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["Ncf_Aplicar"].Value)) continue;

                int iVentaID = Helper.ConvertirEntero(oFila.Cells["Ncf_VentaID"].Value);
                decimal mImporte = Helper.ConvertirDecimal(oFila.Cells["Ncf_Diferencia"].Value);
                mTotal += mImporte;

                var oVenta = General.GetEntity<Venta>(c => c.VentaID == iVentaID && c.Estatus);
                decimal mImporteSinIva = UtilLocal.ObtenerPrecioSinIva(mImporte);
                oNotaDetalle.Add(new ProductoVenta()
                {
                    NumeroDeParte = iVentaID.ToString(), // Se usa para mete la VentaID
                    NombreDeParte = string.Format("FACTURA: {0}", oVenta.Folio), // Se usa para mostrar la factura
                    Cantidad = 1,
                    PrecioUnitario = mImporteSinIva,
                    Iva = (mImporte - mImporteSinIva),
                    UnidadDeMedida = "."
                });
            }
            if (mTotal == 0) return;

            Cargando.Mostrar();

            // Se manda hacer la nota de crédito fiscal
            var oRes = VentasProc.GenerarNotaDeCreditoFiscal(oNotaDetalle, this.Cliente.ClienteID, iUsuarioID);
            if (oRes.Error)
            {
                Cargando.Cerrar();
                UtilLocal.MensajeAdvertencia(string.Format("Ocurrió un error al hacer la Nota de Crédito Fiscal\n\n{0}", oRes.Mensaje));
                return;
            }
            
            // Se modifica el detalle de la Nota de Crédito Fiscal, con datos adicionales de las ventas afectadas
            var oNcVentas = General.GetListOf<NotaDeCreditoFiscalDetalle>(c => c.NotaDeCreditoFiscalID == oRes.Respuesta);
            foreach (DataGridViewRow oFila in this.dgvNcf_Facturas.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["Ncf_Aplicar"].Value)) continue;

                int iVentaID = Helper.ConvertirEntero(oFila.Cells["Ncf_VentaID"].Value);
                int iListaPre = Helper.ConvertirEntero(oFila.Cells["Ncf_ListaDePrecios"].Value);
                
                var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == iVentaID);
                var oNcVenta = oNcVentas.FirstOrDefault(c => c.VentaID == iVentaID);
                if (oNcVenta == null) continue;
                oNcVenta.ListaDePreciosUsada = iListaPre;
                oNcVenta.ImporteAntes = oVentaV.Total;
                Guardar.Generico<NotaDeCreditoFiscalDetalle>(oNcVenta);
            }

            // Se descuenta el importe en cada artículo de las ventas afectadas
            foreach (DataGridViewRow oFila in this.dgvNcf_Facturas.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["Ncf_Aplicar"].Value)) continue;

                int iVentaID = Helper.ConvertirEntero(oFila.Cells["Ncf_VentaID"].Value);
                int iListaPre = Helper.ConvertirEntero(oFila.Cells["Ncf_ListaDePrecios"].Value);
                var oPartes = General.GetListOf<VentaDetalle>(c => c.VentaID == iVentaID && c.Estatus);
                decimal mPrecio = 0;
                foreach (var oReg in oPartes)
                {
                    var oPrecio = General.GetEntity<PartePrecio>(c => c.ParteID == oReg.ParteID && c.Estatus);
                    switch (iListaPre)
                    {
                        case 1: mPrecio = oPrecio.PrecioUno.Valor(); break;
                        case 2: mPrecio = oPrecio.PrecioDos.Valor(); break;
                        case 3: mPrecio = oPrecio.PrecioTres.Valor(); break;
                        case 4: mPrecio = oPrecio.PrecioCuatro.Valor(); break;
                        case 5: mPrecio = oPrecio.PrecioCinco.Valor(); break;
                    }
                    // Se calcula el Iva
                    oReg.PrecioUnitario = UtilLocal.ObtenerPrecioSinIva(mPrecio);
                    oReg.Iva = (mPrecio - oReg.PrecioUnitario);
                    Guardar.Generico<VentaDetalle>(oReg);
                }
            }

            // Se crea la póliza contable correspondiente (AfeConta)
            // Una póliza por cada venta afectada
            var oNotaV = General.GetEntity<NotasDeCreditoFiscalesView>(c => c.NotaDeCreditoFiscalID == oRes.Respuesta);
            foreach (var oReg in oNcVentas)
            {
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.NotaDeCreditoDescuentoVenta, oReg.NotaDeCreditoFiscalDetalleID
                    , (oNotaV.Serie + oNotaV.Folio), oNotaV.Cliente);
            }

            // Se guarda la autorización
            VentasProc.GenerarAutorizacion(Cat.AutorizacionesProcesos.NotaDeCreditoFiscalCrear, "NotaDeCreditoFiscal", oRes.Respuesta, iAutorizoID);

            Cargando.Cerrar();
            this.NcfLlenarFacturas();
        }

        #endregion

        #region [ Públicos ]

        public void CambiarCliente(ClientesDatosView oCliente)
        {
            this.Cliente = oCliente;
            this.AplicarFiltro();
            this.CalcularTotales();
            if (this.tabCobranza.SelectedTab.Name == "tbpVales")
                this.NcAplicarFiltro();
        }

        public List<VentasACreditoView> ObtenerVentasMarcadas()
        {
            var oVentas = new List<VentasACreditoView>();
            foreach (DataGridViewRow Fila in this.dgvVentas.Rows)
            {
                if (!Helper.ConvertirBool(Fila.Cells["vPagar"].Value))
                    continue;

                oVentas.Add(new VentasACreditoView()
                {
                    VentaID = Helper.ConvertirEntero(Fila.Cells["vVentaID"].Value),
                    Folio = Helper.ConvertirCadena(Fila.Cells["vFolio"].Value),
                    Fecha = Helper.ConvertirFechaHora(Fila.Cells["vFecha"].Value),
                    Vencimiento = Helper.ConvertirFechaHora(Fila.Cells["vVencimiento"].Value),
                    Total = Helper.ConvertirDecimal(Fila.Cells["vImporte"].Value),
                    Pagado = Helper.ConvertirDecimal(Fila.Cells["vAbono"].Value),
                    Restante = Helper.ConvertirDecimal(Fila.Cells["vSaldo"].Value)
                });
            }
            return oVentas;
        }

        #endregion

                                                                                                                                                        
    }
}
