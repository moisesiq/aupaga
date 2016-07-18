using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using FastReport;
using FastReport.Export;

using LibUtil;

namespace Refaccionaria.App
{
    public partial class DetalleProveedorPoliza : DetalleBase
    {
        public int ProveedorId = 0;
        ControlError ctlError = new ControlError();
        ControlError ctlInfo = new ControlError() { Icon = Properties.Resources.Ico_Info };
        public List<int> FacturasSel;
        Proveedor Proveedor;

        public DetalleProveedorPoliza()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void DetalleProveedorPoliza_Load(object sender, EventArgs e)
        {
            this.cboBanco.DataSource = Datos.GetListOf<Banco>(p => p.Estatus.Equals(true));
            this.cboBanco.DisplayMember = "NombreBanco";
            this.cboBanco.ValueMember = "BancoID";
            this.cboBanco.SelectedValue = 6;

            this.cmbCuentaBancaria.CargarDatos("BancoCuentaID", "NombreDeCuenta", Datos.GetListOf<BancoCuenta>(c => c.UsoProveedores));
            this.cmbCuentaBancaria.SelectedValue = Cat.CuentasBancarias.Scotiabank;

            this.cboFormaPago.DataSource = Datos.GetListOf<TipoFormaPago>(c => c.Estatus && (c.TipoFormaPagoID == Cat.FormasDePago.Cheque
                || c.TipoFormaPagoID == Cat.FormasDePago.Tarjeta || c.TipoFormaPagoID == Cat.FormasDePago.Transferencia));
            this.cboFormaPago.DisplayMember = "NombreTipoFormaPago";
            this.cboFormaPago.ValueMember = "TipoFormaPagoID";

            Proveedor = Datos.GetEntity<Proveedor>(p => p.ProveedorID.Equals(ProveedorId));
            this.txtBeneficiario.Text = Proveedor.Beneficiario;

            this.Text = "Datos del Pago";
            // this.txtBeneficiario.Clear();
            this.txtImporte.Clear();
            this.txtDocumento.Clear();

            this.dgvDetalle.DefaultCellStyle.ForeColor = Color.Black;
            this.dgvAbonos.DefaultCellStyle.ForeColor = Color.Black;

            this.CargarMovimientosNoPagados(this.FacturasSel);
            // this.CargarNotasDeCredito(this.Proveedor.ProveedorID);

            this.AnalizarNotasDeCredito();
        }

        private void DetalleProveedorPoliza_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        private void cboBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboBanco.Text == "EFECTIVO")
                this.cboFormaPago.SelectedValue = 2;
            else
                this.cboFormaPago.SelectedValue = 1;
        }

        private void dgvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDetalle.Columns[e.ColumnIndex].Name == "fac_Final")
            {
                this.CalcularImporteRestante();
            }
        }

        private void dgvAbonos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.dgvAbonos.CurrentRow == null) return;
                if (UtilLocal.MensajePregunta("¿Estás seguro que deseas eliminar el descuento seleccionado?") == DialogResult.Yes)
                {
                    this.EliminarAbono(this.dgvAbonos.CurrentRow);
                }
            }
        }

        private void btnCrearDescuento_Click(object sender, EventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null)
            {
                UtilLocal.MensajeAdvertencia("Debes seleccionar la factura a la cual se le va a aplicar el descuento.");
                return;
            }

            this.CrearDescuento();
        }

        private void btnCrearDescuentosFacturas_Click(object sender, EventArgs e)
        {
            this.CrearDescuentosFacturas();
        }

        private void btnNotaDeCredito_Click(object sender, EventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null)
            {
                UtilLocal.MensajeAdvertencia("Debes seleccionar la factura a la cual se le va a aplicar la nota de crédito.");
                return;
            }

            this.UsarNotaDeCredito(this.dgvDetalle.CurrentRow);
        }

        private void btnPagoDeCaja_Click(object sender, EventArgs e)
        {
            this.AgregarPagoDeCaja(this.dgvDetalle.CurrentRow);
        }

        private void btnSoloGuardar_Click(object sender, EventArgs e)
        {
            if (this.SoloGuardar(true))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            /*
            this.txtImporte_Leave(sender, null);
            if (!Validaciones())
                return;
            try
            {                
                decimal saldoToltal = 0;
                foreach (DataGridViewRow fila in this.dgvDetalle.Rows)
                {
                    saldoToltal += Util.ConvertirDecimal(fila.Cells["Saldo"].Value);
                }

                if (Util.ConvertirDecimal(this.txtImporte.Text) > saldoToltal)
                {
                    var res = Util.MensajePregunta("El Importe ingresado es mayor a la suma de todos los adeudos, desea continuar?", GlobalClass.NombreApp);
                    if (res == DialogResult.No)
                    {
                        return;
                    }
                }

                var poliza = new ProveedorPoliza
                {
                    ProveedorID = ProveedorId,
                    BancoID = Util.ConvertirEntero(this.cboBanco.SelectedValue),
                    TipoFormaPagoID = Util.ConvertirEntero(this.cboFormaPago.SelectedValue),
                    NumeroDocumento = this.txtDocumento.Text,
                    ImporteTotal = Util.ConvertirDecimal(this.txtImporte.Text),
                    Beneficiario = this.txtBeneficiario.Text,
                    FechaPago = this.dtpFechaMovimiento.Value,
                    UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                    FechaRegistro = DateTime.Now,
                    Estatus = true,
                    Actualizar = true
                };
                General.SaveOrUpdate<ProveedorPoliza>(poliza, poliza);

                foreach (DataGridViewRow fila in this.dgvDetalle.Rows)
                {
                    var polizaDetalle = new ProveedorPolizaDetalle
                    {
                        ProveedorPolizaID = poliza.ProveedorPolizaID,
                        MovimientoInventarioID = Util.ConvertirEntero(fila.Cells["MovimientoInventarioID"].Value),
                        ProveedorPolizaTipoMovimientoID = Util.ConvertirEntero(fila.Cells["ProveedorPolizaTipoMovimientoID"].Value),
                        ImporteMovimiento = Util.ConvertirDecimal(fila.Cells["Pago"].Value),
                        FechaRegistro = DateTime.Now,
                        Estatus = true
                    };
                    General.SaveOrUpdate<ProveedorPolizaDetalle>(polizaDetalle, polizaDetalle);
                }

                //Se validan los pagos registrados de todos los movimientos y si el importe es igual a la suma de los pagos
                //se cambia el estatus de "FueLiquidado" a verdadero
                foreach (DataGridViewRow fila in this.dgvDetalle.Rows)
                {
                    decimal importeTotal = 0;
                    decimal sumaPagos = 0;
                    var movimientoInventarioID = Util.ConvertirEntero(fila.Cells["MovimientoInventarioID"].Value);
                    importeTotal = Util.ConvertirDecimal(fila.Cells["Importe"].Value);
                    sumaPagos = ObtenerSumaPagos(movimientoInventarioID);
                    if (importeTotal == sumaPagos)
                    {
                        var movimientoInventario = General.GetEntity<MovimientoInventario>(m => m.MovimientoInventarioID.Equals(movimientoInventarioID));
                        if (movimientoInventario != null)
                        {
                            movimientoInventario.FueLiquidado = true;
                            movimientoInventario.FechaModificacion = DateTime.Now;
                            General.SaveOrUpdate<MovimientoInventario>(movimientoInventario, movimientoInventario);
                        }
                    }
                }
                
                //new Notificacion("Poliza Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                var resp = Util.MensajePregunta(string.Format("{0}{1}{2}", "Poliza generada exitosamente con el Número: ", poliza.ProveedorPolizaID, ", desea ver reporte?"), GlobalClass.NombreApp);
                if (resp == DialogResult.Yes)
                {
                    var oPoliza = General.GetEntity<ProveedorReportePolizasView>(pro => pro.ProveedorPolizaID == poliza.ProveedorPolizaID);
                    var lista = General.GetListOf<ProveedorReportePolizasDetalleView>(pr => pr.ProveedorPolizaID == poliza.ProveedorPolizaID);
                    IEnumerable<ProveedorReportePolizasDetalleView> listaE = lista;

                    // Se genera el ticket
                    var oRep = new Report();
                    oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "ProveedorPoliza.frx");
                    oRep.RegisterData(listaE, "Pagos");
                    //oRep.GetDataSource("Pagos").Enabled = true;
                    oRep.RegisterData(new List<ProveedorReportePolizasView>() { oPoliza }, "Poliza");
                    oRep.SetParameterValue("documento", this.txtDocumento.Text);
                    UtilLocal.EnviarReporteASalida("Reportes.ProveedoresPolizas.Salida", oRep);

                    
                    /*VisorDeReportes visor = VisorDeReportes.Instance;
                    visor.oID = poliza.ProveedorPolizaID;
                    visor.oTipoReporte = 1;
                    visor.ShowDialog();
                    this.Close();*
                }
                catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarMovimientosNoPagados(ProveedorId));
                catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarPagosParcialesYdevoluciones(0, null));
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
            */
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Metodos ]

        private void CargarMovimientosNoPagados(List<int> Ids)
        {
            // Se llena el grid de las facturas y el grid de los descuentos ya pagados y pendientes
            this.dgvDetalle.Rows.Clear();
            this.dgvAbonos.Rows.Clear();
            foreach (int iId in Ids)
            {
                // Facturas
                var oCompra = Datos.GetEntity<ProveedoresComprasView>(c => c.MovimientoInventarioID == iId);
                this.dgvDetalle.Rows.Add(oCompra.MovimientoInventarioID, oCompra.Factura, oCompra.Fecha, oCompra.ImporteFactura
                    , oCompra.Abonado, oCompra.Saldo, oCompra.Descuento, oCompra.Final);

                // Descuentos / Abonos
                var oAbonos = Datos.GetListOf<ProveedoresPolizasDetalleAvanzadoView>(c => c.MovimientoInventarioID == iId && !c.Pagado);
                foreach (var oReg in oAbonos)
                    this.dgvAbonos.Rows.Add(false, oReg.ProveedorPolizaDetalleID, oReg.MovimientoInventarioID, oReg.OrigenID, oReg.CajaEgresoID, oReg.NotaDeCreditoID
                        , oReg.FolioFactura, oReg.Origen, null, null, oReg.Importe, oReg.Folio, oReg.Observacion);
            }
            
            this.CalcularDescuentos();
        }

        private void CalcularImporteRestante()
        {
            decimal mImporte = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
                mImporte += Util.Decimal(oFila.Cells["fac_Final"].Value);            

            this.txtImporte.Text = mImporte.ToString();
        }

        private void AnalizarNotasDeCredito()
        {
            // Se obtienen las facturas a pagar
            var oMovsAPagar = new List<int>();
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
                oMovsAPagar.Add(Util.Entero(oFila.Cells["fac_MovimientoInventarioID"].Value));

            var oNotas = Datos.GetListOf<ProveedorNotaDeCredito>(c => c.ProveedorID == this.Proveedor.ProveedorID && c.Disponible);
            var oMovsNotas = new List<int>();
            foreach (var oReg in oNotas)
            {
                oMovsNotas.AddRange(Datos.GetListOf<ProveedorNotaDeCreditoDetalle>(c => c.ProveedorNotaDeCreditoID == oReg.ProveedorNotaDeCreditoID)
                    .Select(c => c.MovimientoInventarioID));
            }

            bool bNotasAct = false, bNotasPag = false;
            foreach (var iMov in oMovsNotas)
            {
                // Se revisa si hay notas disponibles para las facturas a pagar
                if (!bNotasAct)
                {
                    if (oMovsAPagar.Contains(iMov))
                    {
                        bNotasAct = true;
                        break;
                    }
                }
                // Se revisa si hay notas disponibles para facturas ya pagadas
                if (!bNotasPag)
                {
                    if (Datos.Exists<MovimientoInventario>(c => c.MovimientoInventarioID == iMov && c.FueLiquidado))
                    {
                        bNotasPag = true;
                    }
                }
            }

            // Se muestra la notificación correspondiente, si hubiera
            string sInfo = "";
            if (bNotasAct)
                sInfo = "Hay una o más notas de crédito disponibles para las facturas seleccionadas.";
            if (bNotasAct)
                sInfo = ((sInfo == "" ? "" : "\n") + "Hay una o más notas de crédito correspondientes a facturas ya pagadas.");
            this.ctlInfo.PonerError(this.lblInfoNotasDeCredito, sInfo);
        }

        private void CrearDescuento()
        {
            // Se solicita validación de usuario y autorización
            // var oResU = UtilLocal.ValidarObtenerUsuario();
            // if (oResU.Error) return;
            // var oResA = UtilLocal.ValidarObtenerUsuario("", "Autorización");
            
            // Se piden los datos
            var frmDatos = new ObtenerValores("Importe", "Observación");
            if (frmDatos.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                int iMovID = Util.Entero(this.dgvDetalle.CurrentRow.Cells["fac_MovimientoInventarioID"].Value);
                string sFactura = Util.Cadena(this.dgvDetalle.CurrentRow.Cells["fac_Factura"].Value);
                decimal mImporteDesc = Util.Decimal(frmDatos.Valor1);
                this.dgvAbonos.Rows.Add(true, null, iMovID, Cat.OrigenesPagosAProveedores.DescuentoDirecto, null, null, sFactura, "Descuento Directo", null, null, mImporteDesc
                    , null, frmDatos.Valor2);
            }
            frmDatos.Dispose();
            this.CalcularDescuentos();
        }

        private void CrearDescuentosFacturas()
        {
            // Se quitan los descuentos previos, si hubiera
            for (int iFila = 0; iFila < this.dgvAbonos.Rows.Count; iFila++)
            {
                var oFila = this.dgvAbonos.Rows[iFila];
                if (Util.Cadena(oFila.Cells["abo_Origen"].Value) == "Descuento Factura")
                {
                    this.dgvAbonos.Rows.Remove(oFila);
                    iFila--;
                }
            }

            // Se agregan los descuentos las facturas
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                int iMovID = Util.Entero(oFila.Cells["fac_MovimientoInventarioID"].Value);
                var oMov = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovID && c.Estatus);
                decimal mPorcentajeDesc = (1 - (oMov.ImporteTotal / oMov.ImporteFactura));
                // decimal mImporteDescuento = (oMov.ImporteFactura - oMov.ImporteTotal);
                decimal mFinal = Util.Decimal(oFila.Cells["fac_Final"].Value);
                decimal mImporteDescuento = (mFinal * mPorcentajeDesc);
                if (mImporteDescuento > 0)
                {
                    this.dgvAbonos.Rows.Add(true, null, oMov.MovimientoInventarioID, Cat.OrigenesPagosAProveedores.DescuentoFactura, null, null, oMov.FolioFactura
                        , "Descuento Factura", oMov.ImporteFactura, oMov.ImporteTotal, mImporteDescuento, null, "");
                }
            }
            this.CalcularDescuentos();
        }

        private void UsarNotaDeCredito(DataGridViewRow oFactura)
        {
            // Se obtienen las notas de créditos ya aplicadas aquí, con sus importes correspondientes
            var oNotasUsadas = new Dictionary<int, decimal>();
            foreach (DataGridViewRow oFila in this.dgvAbonos.Rows)
            {
                if (Util.Cadena(oFila.Cells["abo_Origen"].Value) == "Nota de Crédito")
                {
                    int iNotaID = Util.Entero(oFila.Cells["abo_NotaDeCreditoID"].Value);
                    if (!oNotasUsadas.ContainsKey(iNotaID))
                        oNotasUsadas.Add(iNotaID, 0);
                    oNotasUsadas[iNotaID] += Util.Decimal(oFila.Cells["abo_Importe"].Value);
                }
            }

            // Se obtienen las notas de crédito disponibles y se restan los importes ya usados
            var oNotas = Datos.GetListOf<ProveedoresNotasDeCreditoView>(c => c.ProveedorID == this.Proveedor.ProveedorID && c.Disponible);
            // Se modifica el subtotal de la nota, para ser usada como el total restante
            foreach (var oReg in oNotas)
            {
                // oReg.Subtotal = (oReg.Subtotal + oReg.Iva);
                if (oNotasUsadas.ContainsKey(oReg.ProveedorNotaDeCreditoID))
                    oReg.Restante -= oNotasUsadas[oReg.ProveedorNotaDeCreditoID];
            }
            // Se muestra el formulario con las notas disponibles
            var frmNotas = new EdicionListadoTheos(oNotas) { Text = "Nota de crédito" };
            frmNotas.MostrarColumnas("Folio", "Facturas", "Total", "Restante");
            frmNotas.HabilitarColumnas();
            // frmNotas.dgvListado.Columns["Subtotal"].HeaderText = "Restante";
            frmNotas.AgregarColumnaEdicion("ImporteAUsar", "Usar");
            frmNotas.AgregarColumnaSeleccion("Sel");
            frmNotas.dgvListado.SelectionMode = DataGridViewSelectionMode.CellSelect;
            
            // Se agrega la validación para cuando se le de aceptar
            decimal mImporteFaltante = Util.Decimal(oFactura.Cells["fac_Final"].Value);
            frmNotas.Aceptado += new EventHandler<FormClosingEventArgs>((s, e) =>
            {
                DataGridView oGrid = (s as EdicionListado).dgvListado;
                decimal mTotalNotas = 0;
                foreach (DataGridViewRow oFila in oGrid.Rows)
                {
                    if (!Util.Logico(oFila.Cells["Sel"].Value)) continue;

                    decimal mAUsar = Util.Decimal(oFila.Cells["ImporteAUsar"].Value);
                    if (mAUsar > Util.Decimal(oFila.Cells["Restante"].Value))
                    {
                        oFila.Cells["ImporteAUsar"].ErrorText = "El importe a usar no puede ser mayor que el importe restante.";
                        e.Cancel = true;
                    }

                    mTotalNotas += mAUsar;
                }

                if (mTotalNotas > mImporteFaltante)
                {
                    UtilLocal.MensajeAdvertencia("El importe total indicado es mayor al restante de de la factura seleccionada.");
                    e.Cancel = true;
                }
            });

            if (frmNotas.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                var oDatos = frmNotas.ObtenerResultado();
                foreach (DataRow oFila in oDatos.Rows)
                {
                    if (Util.Logico(oFila["Sel"]) && Util.Decimal(oFila["ImporteAUsar"]) > 0)
                    {
                        this.dgvAbonos.Rows.Add(true, null, oFactura.Cells["fac_MovimientoInventarioID"].Value, Cat.OrigenesPagosAProveedores.NotaDeCredito
                            , null, oFila["ProveedorNotaDeCreditoID"], oFactura.Cells["fac_Factura"].Value, "Nota de Crédito", null, null, oFila["ImporteAUsar"]
                            , oFila["Folio"], null);
                    }
                }
            }
            frmNotas.Dispose();

            this.CalcularDescuentos();
        }

        private void CalcularDescuentos()
        {
            // Se obtiene el descuento total de cada factura
            var oDescuentos = new Dictionary<int, decimal>();
            foreach (DataGridViewRow oFila in this.dgvAbonos.Rows)
            {
                int iMovID = Util.Entero(oFila.Cells["abo_MovimientoInventarioID"].Value);
                decimal mImporte = Util.Decimal(oFila.Cells["abo_Importe"].Value);
                if (!oDescuentos.ContainsKey(iMovID))
                    oDescuentos.Add(iMovID, 0);
                oDescuentos[iMovID] += mImporte;
            }

            // Se aplica el descuento
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                int iMovID = Util.Entero(oFila.Cells["fac_MovimientoInventarioID"].Value);
                oFila.Cells["fac_Descuento"].Value = (oDescuentos.ContainsKey(iMovID) ? oDescuentos[iMovID] : 0);
                oFila.Cells["fac_Final"].Value = (Util.Decimal(oFila.Cells["fac_Saldo"].Value) - Util.Decimal(oFila.Cells["fac_Descuento"].Value));
            }

            this.CalcularImporteRestante();
        }

        private void AgregarPagoDeCaja(DataGridViewRow oFactura)
        {
            // Se obtiene la cuenta auxiliar correspondiente al proveedor
            var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Proveedores
                && c.RelacionID == this.Proveedor.ProveedorID);
            if (oCuentaAux == null)
            {
                UtilLocal.MensajeAdvertencia("El proveedor actual no tiene una cuenta contable asignada. No se puede enlazar con los gastos de caja.");
                return;
            }

            // Se obtienen los pagos de caja ya aplicados aquí, con sus importes correspondientes
            var oUsados = new Dictionary<int, decimal>();
            foreach (DataGridViewRow oFila in this.dgvAbonos.Rows)
            {
                if (Util.Cadena(oFila.Cells["abo_Origen"].Value) == "Pago de Caja")
                {
                    int iEgresoID = Util.Entero(oFila.Cells["abo_CajaEgresoID"].Value);
                    if (!oUsados.ContainsKey(iEgresoID))
                        oUsados.Add(iEgresoID, 0);
                    oUsados[iEgresoID] += Util.Decimal(oFila.Cells["abo_Importe"].Value);
                }
            }

            // Se obtienen las notas de crédito disponibles y se restan los importes ya usados
            var oPagosCaja = Datos.GetListOf<CajaEgresosProveedoresView>(c => c.ContaCuentaAuxiliarID == oCuentaAux.ContaCuentaAuxiliarID
                && c.Facturado.HasValue && (!c.AfectadoEnProveedores.HasValue || !c.AfectadoEnProveedores.Value));
            // Se modifica el subtotal de la nota, para ser usada como el total restante
            foreach (var oReg in oPagosCaja)
            {
                // oReg.Subtotal = (oReg.Subtotal + oReg.Iva);
                if (oUsados.ContainsKey(oReg.CajaEgresoID))
                    oReg.Restante -= oUsados[oReg.CajaEgresoID];
            }
            // Se muestra el formulario con las notas disponibles
            var frmLista = new EdicionListadoTheos(oPagosCaja) { Text = "Agregar pago de caja" };
            frmLista.MostrarColumnas("Fecha", "Concepto", "Total", "Restante");
            frmLista.HabilitarColumnas();
            frmLista.AgregarColumnaEdicion("ImporteAUsar", "Usar");
            frmLista.AgregarColumnaSeleccion("Sel");
            frmLista.dgvListado.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // Se agrega la validación para cuando se le de aceptar
            decimal mImporteFaltante = Util.Decimal(oFactura.Cells["fac_Final"].Value);
            frmLista.Aceptado += new EventHandler<FormClosingEventArgs>((s, e) =>
            {
                DataGridView oGrid = (s as EdicionListado).dgvListado;
                decimal mTotal = 0;
                foreach (DataGridViewRow oFila in oGrid.Rows)
                {
                    if (!Util.Logico(oFila.Cells["Sel"].Value)) continue;

                    decimal mAUsar = Util.Decimal(oFila.Cells["ImporteAUsar"].Value);
                    if (mAUsar > Util.Decimal(oFila.Cells["Restante"].Value))
                    {
                        oFila.Cells["ImporteAUsar"].ErrorText = "El importe a usar no puede ser mayor que el importe restante.";
                        e.Cancel = true;
                    }

                    mTotal += mAUsar;
                }

                if (mTotal > mImporteFaltante)
                {
                    UtilLocal.MensajeAdvertencia("El importe total indicado es mayor al restante de de la factura seleccionada.");
                    e.Cancel = true;
                }
            });

            // Se muestra el formulario
            if (frmLista.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                var oDatos = frmLista.ObtenerResultado();
                foreach (DataRow oFila in oDatos.Rows)
                {
                    if (Util.Logico(oFila["Sel"]) && Util.Decimal(oFila["ImporteAUsar"]) > 0)
                    {
                        this.dgvAbonos.Rows.Add(true, null, oFactura.Cells["fac_MovimientoInventarioID"].Value, Cat.OrigenesPagosAProveedores.PagoDeCaja
                            , oFila["CajaEgresoID"], null, oFactura.Cells["fac_Factura"].Value, "Pago de Caja", null, null, Util.Decimal(oFila["ImporteAUsar"])
                            , null, oFila["Concepto"]);
                    }
                }
            }
            frmLista.Dispose();

            this.CalcularDescuentos();
        }

        private void EliminarAbono(DataGridViewRow oFila)
        {
            int iAbonoID = Util.Entero(oFila.Cells["abo_ProveedorPolizaDetalleID"].Value);
            if (iAbonoID > 0)
            {
                var oAbono = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iAbonoID && c.Estatus);
                int iNotaDeCreditoID = oAbono.NotaDeCreditoID.Valor();
                int iCajaEgresoID = oAbono.CajaEgresoID.Valor();
                // Se borra el registro de la base de datos
                Datos.Eliminar<ProveedorPolizaDetalle>(oAbono);
                // Se borra el registro de ProveedorPoliza, si ya no tiene más abonos
                if (!Datos.Exists<ProveedorPolizaDetalle>(c => c.ProveedorPolizaID == oAbono.ProveedorPolizaID))
                {
                    var oPago = Datos.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == oAbono.ProveedorPolizaID);
                    Datos.Eliminar<ProveedorPoliza>(oPago);
                }

                // Se restablecen las notas de crédito o pagos de caja, para que vuelvan a estar disponibles, si aplica
                switch (oAbono.OrigenID)
                {
                    case Cat.OrigenesPagosAProveedores.NotaDeCredito:
                        AdmonProc.VerMarcarDisponibilidadNotaDeCreditoProveedor(iNotaDeCreditoID);
                        break;
                    case Cat.OrigenesPagosAProveedores.PagoDeCaja:
                        AdmonProc.VerMarcarDisponibilidadGastoDeCajaParaProveedor(iCajaEgresoID);
                        break;
                }
            }
            this.dgvAbonos.Rows.Remove(oFila);

            this.CalcularDescuentos();
        }

        private bool SoloGuardar(bool bReporte)
        {
            // Se verifican los totales
            if (!this.Validaciones() || !this.ValidarTotales())
                return false;

            Cargando.Mostrar();
            
            // Se comienza a guardar la información
            /*
            var poliza = new ProveedorPoliza
            {
                ProveedorID = ProveedorId,
                FechaPago = this.dtpFechaMovimiento.Value,
                // ImportePago = mImportePago,
                // BancoCuentaID = Util.ConvertirEntero(this.cmbCuentaBancaria.SelectedValue),
                // TipoFormaPagoID = Util.ConvertirEntero(this.cboFormaPago.SelectedValue),
                // FolioDePago = this.txtDocumento.Text,
                UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
            };
            Datos.Guardar<ProveedorPoliza>(poliza);
            */

            // Se guarda el detalle de la poliza
            DateTime dAhora = DateTime.Now;
            var oPolizaDetalle = new List<ProveedorPolizaDetalle>();
            // Se agregan los pagos directos, uno para cada factura
            decimal mTotalRestante = Util.Decimal(this.txtImporte.Text);
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                int iMovID = Util.Entero(oFila.Cells["fac_MovimientoInventarioID"].Value);
                decimal mImporte = Util.Decimal(oFila.Cells["fac_Final"].Value);
                if (mImporte <= 0) continue;

                // Se determina el importe, considerando lo que resta
                if (mTotalRestante > 0)
                    mImporte = (mImporte < mTotalRestante ? mImporte : mTotalRestante);
                else
                    break;

                /*
                var polizaDetalle = new ProveedorPolizaDetalle
                {
                    // ProveedorPolizaID = poliza.ProveedorPolizaID,
                    MovimientoInventarioID = iMovID,
                    FechaPago = dAhora,
                    OrigenID = Cat.OrigenesPagosAProveedores.PagoDirecto,
                    Subtotal = UtilLocal.ObtenerPrecioSinIva(mImporte),
                    Iva = UtilLocal.ObtenerIvaDePrecio(mImporte),
                    Folio = this.txtDocumento.Text,
                    Pagado = false
                };
                // Datos.Guardar<ProveedorPolizaDetalle>(polizaDetalle);
                oPolizaDetalle.Add(polizaDetalle);
                */

                // Se agrega el movimiento al grid de abonos, por si es usado para guardar ya bien
                this.dgvAbonos.Rows.Add(true, null, iMovID, Cat.OrigenesPagosAProveedores.PagoDirecto, null, null, "", "PAGO DIRECTO", null, null
                    , mImporte, this.txtDocumento.Text, null);

                mTotalRestante -= mImporte;
            }
            // Se agregan los descuentos
            foreach (DataGridViewRow oFila in this.dgvAbonos.Rows)
            {
                if (Util.Entero(oFila.Cells["abo_ProveedorPolizaDetalleID"].Value) > 0)
                    continue;

                decimal mImporte = Util.Decimal(oFila.Cells["abo_Importe"].Value);
                int iNotaDeCreditoID = Util.Entero(oFila.Cells["abo_NotaDeCreditoID"].Value);
                int iOrigenID = Util.Entero(oFila.Cells["abo_OrigenID"].Value);
                int iCajaEgresoID = Util.Entero(oFila.Cells["abo_CajaEgresoID"].Value);

                var polizaDetalle = new ProveedorPolizaDetalle
                {
                    // ProveedorPolizaID = poliza.ProveedorPolizaID,
                    MovimientoInventarioID = Util.Entero(oFila.Cells["abo_MovimientoInventarioID"].Value),
                    FechaPago = dAhora,
                    OrigenID = iOrigenID,
                    Subtotal = UtilTheos.ObtenerPrecioSinIva(mImporte),
                    Iva = UtilTheos.ObtenerIvaDePrecio(mImporte),
                    NotaDeCreditoID = (iNotaDeCreditoID > 0 ? (int?)iNotaDeCreditoID : null),
                    CajaEgresoID = (iCajaEgresoID > 0 ? (int?)iCajaEgresoID : null),
                    Observacion = Util.Cadena(oFila.Cells["abo_Observacion"].Value),
                    Folio = Util.Cadena(oFila.Cells["abo_NotaDeCredito"].Value),
                    Pagado = false
                };
                // Datos.Guardar<ProveedorPolizaDetalle>(polizaDetalle);
                oPolizaDetalle.Add(polizaDetalle);

                // Se llena el dato del id asignado al registro de abono
                // oFila.Cells["abo_ProveedorPolizaDetalleID"].Value = polizaDetalle.ProveedorPolizaDetalleID;
            }

            // Se verifica si se debe de generar un nuevo registro de ProveedorPoliza
            ProveedorPoliza poliza = null;
            if (oPolizaDetalle.Any(c => c.ProveedorPolizaID == 0))
            {
                poliza = new ProveedorPoliza
                {
                    ProveedorID = ProveedorId,
                    FechaPago = this.dtpFechaMovimiento.Value
                };
                Datos.Guardar<ProveedorPoliza>(poliza);
                
                // Se guardan los abonos
                foreach (var oReg in oPolizaDetalle)
                {
                    oReg.ProveedorPolizaID = poliza.ProveedorPolizaID;
                    Datos.Guardar<ProveedorPolizaDetalle>(oReg);

                    // Se marcan las notas de crédito como no disponibles, si ya se utilizaron por completo
                    if (oReg.NotaDeCreditoID > 0)
                        AdmonProc.VerMarcarDisponibilidadNotaDeCreditoProveedor(oReg.NotaDeCreditoID.Valor());

                    // Se marca el gasto de caja como afectado en proveedores, si aplica
                    if (oReg.CajaEgresoID > 0)
                    {
                        var oGasto = Datos.GetEntity<CajaEgreso>(c => c.CajaEgresoID == oReg.CajaEgresoID && c.Estatus);

                        AdmonProc.VerMarcarDisponibilidadGastoDeCajaParaProveedor(oReg.CajaEgresoID.Valor());
                        
                        // Si es un gasto de caja y es nota, se modifica el pago, para incluir el iva en el subtotal y dejar el iva en cero
                        if (oGasto.Facturado.HasValue && !oGasto.Facturado.Value)
                        {
                            oReg.Subtotal += oReg.Iva;
                            oReg.Iva = 0;
                            Datos.Guardar<ProveedorPolizaDetalle>(oReg);
                        }
                    }

                    // Se busca el registro en el grid y se llena el dato del id que se le asignó
                    foreach (DataGridViewRow oFila in this.dgvAbonos.Rows)
                    {
                        if (Util.Entero(oFila.Cells["abo_ProveedorPolizaDetalleID"].Value) == 0
                            && Util.Entero(oFila.Cells["abo_MovimientoInventarioID"].Value) == oReg.MovimientoInventarioID
                            && Util.Entero(oFila.Cells["abo_OrigenID"].Value) == oReg.OrigenID
                            && Util.Decimal(oFila.Cells["abo_Importe"].Value) == (oReg.Subtotal + oReg.Iva))
                        {
                            oFila.Cells["abo_ProveedorPolizaDetalleID"].Value = oReg.ProveedorPolizaDetalleID;
                            break;
                        }
                    }
                }
            }
            
            Cargando.Cerrar();
            
            // Se manda imprimir el reporte, si aplica
            if (poliza != null && bReporte)
            {
                var resp = UtilLocal.MensajePregunta("Guardado exitosamente. ¿Deseas ver el reporte?");
                if (resp == DialogResult.Yes)
                    this.MostrarReporte(poliza.ProveedorPolizaID);
            }

            return true;
        }

        private bool AccionGuardar()
        {
            if (!this.Validaciones() || !this.ValidarTotales())
                return false;
                        
            // Se manda guardar la información
            if (!this.SoloGuardar(false))
                return false;
            
            // 
            DateTime dPago = this.dtpFechaMovimiento.Value;
            int iCuentaID = Util.Entero(this.cmbCuentaBancaria.SelectedValue);
            int iFormaDePagoID = Util.Entero(this.cboFormaPago.SelectedValue);
            string sFolioDePago = this.txtDocumento.Text;

            // Se marcan los abonos como ya pagados
            int iPolizaNuevaID = 0, iPolizaAntID = 0, iPolizaGeneralID = 0;
            var oPolizaDetalle = new List<ProveedorPolizaDetalle>();
            var oIdsPagosDeCaja = new List<int>();
            decimal mImportePago = 0;
            foreach (DataGridViewRow oFila in this.dgvAbonos.Rows)
            {
                int iAbonoID = Util.Entero(oFila.Cells["abo_ProveedorPolizaDetalleID"].Value);
                var oAbono = Datos.GetEntity<ProveedorPolizaDetalle>(c => c.ProveedorPolizaDetalleID == iAbonoID && c.Estatus);
                
                if (oAbono.OrigenID == Cat.OrigenesPagosAProveedores.PagoDirecto)
                {
                    // Se marca el primer pago directo que se encuentre como principal
                    if (Util.Logico(oFila.Cells["abo_EsNuevo"].Value))
                    {
                        if (iPolizaNuevaID == 0)
                            iPolizaNuevaID = oAbono.ProveedorPolizaID;
                    }
                    else
                    {
                        if (iPolizaAntID == 0)
                            iPolizaAntID = oAbono.ProveedorPolizaID;
                    }

                    //
                    mImportePago += (oAbono.Subtotal + oAbono.Iva);
                    oAbono.TipoFormaPagoID = iFormaDePagoID;
                    oAbono.Folio = sFolioDePago;
                    oAbono.BancoCuentaID = iCuentaID;
                }
                if (iPolizaGeneralID == 0)
                    iPolizaGeneralID = oAbono.ProveedorPolizaID;
                
                // Se guarda si es un pago de caja, para uso posterior al hacer las pólizas
                if (oAbono.OrigenID == Cat.OrigenesPagosAProveedores.PagoDeCaja)
                    oIdsPagosDeCaja.Add(oAbono.CajaEgresoID.Valor());

                // Se guarda
                oAbono.FechaPago = dPago;
                oAbono.Pagado = true;
                Datos.Guardar<ProveedorPolizaDetalle>(oAbono);
                oPolizaDetalle.Add(oAbono);
            }
            int iPolizaID = (iPolizaNuevaID > 0 ? iPolizaNuevaID : iPolizaAntID);
            iPolizaID = (iPolizaID > 0 ? iPolizaID : iPolizaGeneralID);
            var poliza = Datos.GetEntity<ProveedorPoliza>(c => c.ProveedorPolizaID == iPolizaID && c.Estatus);

            // Se guarda el movimiento bancario
            if (mImportePago > 0)
            {
                var oProveedor = Datos.GetEntity<Proveedor>(c => c.ProveedorID == ProveedorId && c.Estatus);
                var oMovBanc = new BancoCuentaMovimiento()
                {
                    BancoCuentaID = Util.Entero(this.cmbCuentaBancaria.SelectedValue),
                    EsIngreso = false,
                    Fecha = poliza.FechaPago,
                    FechaAsignado = poliza.FechaPago,
                    Importe = mImportePago,
                    Concepto = oProveedor.NombreProveedor,
                    Referencia = iPolizaID.ToString(),
                    TipoFormaPagoID = iFormaDePagoID,
                    DatosDePago = sFolioDePago,
                    RelacionID = iPolizaID
                };
                ContaProc.RegistrarMovimientoBancario(oMovBanc);
            }

            // Se manda a afectar contabilidad (AfeConta)
            foreach (var oReg in oPolizaDetalle)
            {
                var oMovInv = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == oReg.MovimientoInventarioID && c.Estatus);
                switch (oReg.OrigenID)
                {
                    case Cat.OrigenesPagosAProveedores.NotaDeCredito:
                        int iNotaDeCreditoID = oReg.NotaDeCreditoID.Valor();
                        var oNota = Datos.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iNotaDeCreditoID);
                        if (oNota.OrigenID == Cat.OrigenesNotasDeCreditoProveedor.Garantia)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoGarantia, oReg.ProveedorPolizaDetalleID
                                , oNota.Folio, ("GARANTÍA / " + oMovInv.FolioFactura), poliza.FechaPago);
                        else if (oNota.OrigenID == Cat.OrigenesNotasDeCreditoProveedor.Devolucion)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoNotaDeCreditoDevolucion, oReg.ProveedorPolizaDetalleID
                                , oNota.Folio, ("DEVOLUCIÓN / " + oMovInv.FolioFactura), poliza.FechaPago);
                        break;
                    case Cat.OrigenesPagosAProveedores.DescuentoFactura:
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoDescuentoFactura, oReg.ProveedorPolizaDetalleID
                            , oMovInv.FolioFactura, "DESCUENTO FACTURA", poliza.FechaPago);
                        break;
                    case Cat.OrigenesPagosAProveedores.DescuentoDirecto:
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoDescuentoDirecto, oReg.ProveedorPolizaDetalleID
                            , oMovInv.FolioFactura, "DESCUENTO DIRECTO", poliza.FechaPago);
                        break;
                    case Cat.OrigenesPagosAProveedores.PagoDeCaja:
                        int iGastoID = oIdsPagosDeCaja[0];
                        oIdsPagosDeCaja.RemoveAt(0);
                        var oGasto = Datos.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iGastoID && c.Estatus);
                        if (oGasto.Facturado.Valor())
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado, oReg.ProveedorPolizaDetalleID
                                , oGasto.FolioFactura, oGasto.Concepto, oGasto.SucursalID, poliza.FechaPago);
                        else
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoGastoCaja, oReg.ProveedorPolizaDetalleID
                                , "PAGO CAJA", oGasto.Concepto, oGasto.SucursalID, poliza.FechaPago);
                        break;
                    case Cat.OrigenesPagosAProveedores.PagoDirecto:
                        if (oReg.BancoCuentaID == Cat.CuentasBancarias.Banamex || oReg.BancoCuentaID == Cat.CuentasBancarias.Scotiabank)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCredito, oReg.ProveedorPolizaDetalleID, this.txtDocumento.Text
                                , ("PAGO DIRECTO / " + this.cmbCuentaBancaria.Text), poliza.FechaPago);
                        else
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoProveedorDirectoCpcp, oReg.ProveedorPolizaDetalleID, this.txtDocumento.Text
                                , ("PAGO DIRECTO / " + this.cmbCuentaBancaria.Text), poliza.FechaPago);
                        break;
                }
            }
            //

            Cargando.Cerrar();

            // Se manda imprimir el reporte,
            var resp = UtilLocal.MensajePregunta("Guardado exitosamente. ¿Deseas ver el reporte?");
            if (resp == DialogResult.Yes)
                this.MostrarReporte(poliza.ProveedorPolizaID);

            return true;
        }

        private void MostrarReporte(int iPolizaID)
        {
            var oPoliza = Datos.GetEntity<ProveedoresPolizasView>(pro => pro.ProveedorPolizaID == iPolizaID);
            var oAbonos = Datos.GetListOf<ProveedoresPolizasDetalleAvanzadoView>(pr => pr.ProveedorPolizaID == iPolizaID);
            var oFacturas = new List<ProveedoresPagosView>();
            var oIdsFacturas = oAbonos.Select(c => c.MovimientoInventarioID).Distinct();
            foreach (int iMovId in oIdsFacturas)
                oFacturas.Add(Datos.GetEntity<ProveedoresPagosView>(c => c.ProveedorPolizaID == iPolizaID && c.MovimientoInventarioID == iMovId));

            // Se genera el ticket
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "ProveedorPoliza.frx");
            oRep.RegisterData(new List<ProveedoresPolizasView>() { oPoliza }, "Poliza");
            oRep.RegisterData(oAbonos, "Abonos");
            oRep.RegisterData(oFacturas, "Facturas");
            UtilLocal.EnviarReporteASalida("Reportes.ProveedoresPolizas.Salida", oRep);
        }

        private bool ValidarTotales()
        {
            decimal mImportePago = Util.Decimal(this.txtImporte.Text);
            decimal mFaltanteTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
                mFaltanteTotal += Util.Decimal(oFila.Cells["fac_Final"].Value);
            if (mImportePago > mFaltanteTotal)
            {
                UtilLocal.MensajeAdvertencia("El Importe indicado es mayor al total por pagar. No se puede continuar.");
                return false;
            }
            else if (mImportePago < mFaltanteTotal)
            {
                if (UtilLocal.MensajePregunta("El Importe indicado es menor al total por pagar. ¿Deseas continuar?") != DialogResult.Yes)
                    return false;
            }

            return true;
        }


        private bool Validaciones()
        {
            this.ctlError.LimpiarErrores();
            if (this.txtImporte.Text == "")
                this.ctlError.PonerError(this.txtImporte, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtDocumento.Text == "")
                this.ctlError.PonerError(this.txtDocumento, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtBeneficiario.Text == "")
                this.ctlError.PonerError(this.txtBeneficiario, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            /* foreach (DataGridViewRow fila in this.dgvDetalle.Rows)
            {
                if (Util.ConvertirDecimal(fila.Cells["Pago"].Value) <= 0)
                    this.cntError.PonerError(this.dgvDetalle, "No se puede guardar movimientos con 0.0 como pago", ErrorIconAlignment.MiddleRight);
            }
            */

            return (this.ctlError.NumeroDeErrores == 0);
        }
        
        #endregion

    }
}
