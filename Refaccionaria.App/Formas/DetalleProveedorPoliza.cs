using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
using FastReport;
using FastReport.Export;

namespace Refaccionaria.App
{
    public partial class DetalleProveedorPoliza : DetalleBase
    {
        public int ProveedorId = 0;
        ControlError cntError = new ControlError();
        public List<int> FacturasSel;
        Proveedor Proveedor;

        public DetalleProveedorPoliza()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void DetalleProveedorPoliza_Load(object sender, EventArgs e)
        {
            this.cboBanco.DataSource = Negocio.General.GetListOf<Banco>(p => p.Estatus.Equals(true));
            this.cboBanco.DisplayMember = "NombreBanco";
            this.cboBanco.ValueMember = "BancoID";
            this.cboBanco.SelectedValue = 6;

            this.cmbCuentaBancaria.CargarDatos("BancoCuentaID", "NombreDeCuenta", General.GetListOf<BancoCuenta>(c => c.UsoProveedores));
            this.cmbCuentaBancaria.SelectedValue = Cat.CuentasBancarias.Scotiabank;

            this.cboFormaPago.DataSource = Negocio.General.GetListOf<TipoFormaPago>(c => c.Estatus && (c.TipoFormaPagoID == Cat.FormasDePago.Cheque
                || c.TipoFormaPagoID == Cat.FormasDePago.Tarjeta || c.TipoFormaPagoID == Cat.FormasDePago.Transferencia));
            this.cboFormaPago.DisplayMember = "NombreTipoFormaPago";
            this.cboFormaPago.ValueMember = "TipoFormaPagoID";

            Proveedor = Negocio.General.GetEntity<Proveedor>(p => p.ProveedorID.Equals(ProveedorId));
            this.txtBeneficiario.Text = Proveedor.Beneficiario;

            this.Text = "Datos del Pago";
            // this.txtBeneficiario.Clear();
            this.txtImporte.Clear();
            this.txtDocumento.Clear();

            this.dgvDetalle.DefaultCellStyle.ForeColor = Color.Black;
            this.dgvAbonos.DefaultCellStyle.ForeColor = Color.Black;

            this.CargarMovimientosNoPagados(FacturasSel);
            // this.CargarNotasDeCredito(this.Proveedor.ProveedorID);
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
                    this.dgvAbonos.Rows.Remove(this.dgvAbonos.CurrentRow);
                    this.CalcularDescuentos();
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

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar()) {
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
                    saldoToltal += Negocio.Helper.ConvertirDecimal(fila.Cells["Saldo"].Value);
                }

                if (Negocio.Helper.ConvertirDecimal(this.txtImporte.Text) > saldoToltal)
                {
                    var res = Negocio.Helper.MensajePregunta("El Importe ingresado es mayor a la suma de todos los adeudos, desea continuar?", GlobalClass.NombreApp);
                    if (res == DialogResult.No)
                    {
                        return;
                    }
                }

                var poliza = new ProveedorPoliza
                {
                    ProveedorID = ProveedorId,
                    BancoID = Negocio.Helper.ConvertirEntero(this.cboBanco.SelectedValue),
                    TipoFormaPagoID = Negocio.Helper.ConvertirEntero(this.cboFormaPago.SelectedValue),
                    NumeroDocumento = this.txtDocumento.Text,
                    ImporteTotal = Negocio.Helper.ConvertirDecimal(this.txtImporte.Text),
                    Beneficiario = this.txtBeneficiario.Text,
                    FechaPago = this.dtpFechaMovimiento.Value,
                    UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                    FechaRegistro = DateTime.Now,
                    Estatus = true,
                    Actualizar = true
                };
                Negocio.General.SaveOrUpdate<ProveedorPoliza>(poliza, poliza);

                foreach (DataGridViewRow fila in this.dgvDetalle.Rows)
                {
                    var polizaDetalle = new ProveedorPolizaDetalle
                    {
                        ProveedorPolizaID = poliza.ProveedorPolizaID,
                        MovimientoInventarioID = Negocio.Helper.ConvertirEntero(fila.Cells["MovimientoInventarioID"].Value),
                        ProveedorPolizaTipoMovimientoID = Negocio.Helper.ConvertirEntero(fila.Cells["ProveedorPolizaTipoMovimientoID"].Value),
                        ImporteMovimiento = Negocio.Helper.ConvertirDecimal(fila.Cells["Pago"].Value),
                        FechaRegistro = DateTime.Now,
                        Estatus = true
                    };
                    Negocio.General.SaveOrUpdate<ProveedorPolizaDetalle>(polizaDetalle, polizaDetalle);
                }

                //Se validan los pagos registrados de todos los movimientos y si el importe es igual a la suma de los pagos
                //se cambia el estatus de "FueLiquidado" a verdadero
                foreach (DataGridViewRow fila in this.dgvDetalle.Rows)
                {
                    decimal importeTotal = 0;
                    decimal sumaPagos = 0;
                    var movimientoInventarioID = Negocio.Helper.ConvertirEntero(fila.Cells["MovimientoInventarioID"].Value);
                    importeTotal = Negocio.Helper.ConvertirDecimal(fila.Cells["Importe"].Value);
                    sumaPagos = ObtenerSumaPagos(movimientoInventarioID);
                    if (importeTotal == sumaPagos)
                    {
                        var movimientoInventario = Negocio.General.GetEntity<MovimientoInventario>(m => m.MovimientoInventarioID.Equals(movimientoInventarioID));
                        if (movimientoInventario != null)
                        {
                            movimientoInventario.FueLiquidado = true;
                            movimientoInventario.FechaModificacion = DateTime.Now;
                            Negocio.General.SaveOrUpdate<MovimientoInventario>(movimientoInventario, movimientoInventario);
                        }
                    }
                }
                
                //new Notificacion("Poliza Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                var resp = Negocio.Helper.MensajePregunta(string.Format("{0}{1}{2}", "Poliza generada exitosamente con el Número: ", poliza.ProveedorPolizaID, ", desea ver reporte?"), GlobalClass.NombreApp);
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
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
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
            this.dgvDetalle.Rows.Clear();
            foreach (int iId in Ids)
            {
                var oCompra = General.GetEntity<ProveedoresComprasView>(c => c.MovimientoInventarioID == iId);
                this.dgvDetalle.Rows.Add(oCompra.MovimientoInventarioID, oCompra.Factura, oCompra.Fecha, oCompra.ImporteFactura
                    , oCompra.Abonado, oCompra.Saldo.Valor(), 0, oCompra.Saldo.Valor());
            }
            this.CalcularImporteRestante();
        }

        private void CalcularImporteRestante()
        {
            decimal mImporte = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
                mImporte += Helper.ConvertirDecimal(oFila.Cells["fac_Final"].Value);            

            this.txtImporte.Text = mImporte.ToString();
        }
        
        private void CrearDescuento()
        {
            // Se solicita validación de usuario y autorización
            // var oResU = UtilDatos.ValidarObtenerUsuario();
            // if (oResU.Error) return;
            // var oResA = UtilDatos.ValidarObtenerUsuario("", "Autorización");
            
            // Se piden los datos
            var frmDatos = new ObtenerValores("Importe", "Observación");
            if (frmDatos.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                int iMovID = Helper.ConvertirEntero(this.dgvDetalle.CurrentRow.Cells["fac_MovimientoInventarioID"].Value);
                string sFactura = Helper.ConvertirCadena(this.dgvDetalle.CurrentRow.Cells["fac_Factura"].Value);
                decimal mImporteDesc = Helper.ConvertirDecimal(frmDatos.Valor1);
                this.dgvAbonos.Rows.Add(null, iMovID, Cat.OrigenesPagosAProveedores.DescuentoDirecto, null, null, sFactura, "Descuento Directo", null, null, mImporteDesc
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
                if (Helper.ConvertirCadena(oFila.Cells["abo_Origen"].Value) == "Descuento Factura")
                {
                    this.dgvAbonos.Rows.Remove(oFila);
                    iFila--;
                }
            }

            // Se agregan los descuentos las facturas
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                int iMovID = Helper.ConvertirEntero(oFila.Cells["fac_MovimientoInventarioID"].Value);
                var oMov = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovID && c.Estatus);
                decimal mImporteDescuento = (oMov.ImporteFactura - oMov.ImporteTotal);
                if (mImporteDescuento > 0)
                {
                    this.dgvAbonos.Rows.Add(null, oMov.MovimientoInventarioID, Cat.OrigenesPagosAProveedores.DescuentoFactura, null, null, oMov.FolioFactura, "Descuento Factura"
                        , oMov.ImporteFactura, oMov.ImporteTotal, mImporteDescuento, null, "");
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
                if (Helper.ConvertirCadena(oFila.Cells["abo_Origen"].Value) == "Nota de Crédito")
                {
                    int iNotaID = Helper.ConvertirEntero(oFila.Cells["abo_NotaDeCreditoID"].Value);
                    if (!oNotasUsadas.ContainsKey(iNotaID))
                        oNotasUsadas.Add(iNotaID, 0);
                    oNotasUsadas[iNotaID] += Helper.ConvertirDecimal(oFila.Cells["abo_Importe"].Value);
                }
            }

            // Se obtienen las notas de crédito disponibles y se restan los importes ya usados
            var oNotas = General.GetListOf<ProveedoresNotasDeCreditoView>(c => c.ProveedorID == this.Proveedor.ProveedorID && c.Disponible);
            // Se modifica el subtotal de la nota, para ser usada como el total restante
            foreach (var oReg in oNotas)
            {
                // oReg.Subtotal = (oReg.Subtotal + oReg.Iva);
                if (oNotasUsadas.ContainsKey(oReg.ProveedorNotaDeCreditoID))
                    oReg.Restante -= oNotasUsadas[oReg.ProveedorNotaDeCreditoID];
            }
            // Se muestra el formulario con las notas disponibles
            var frmNotas = new EdicionListadoTheos(oNotas) { Text = "Nota de crédito" };
            frmNotas.MostrarColumnas("Folio", "Total", "Restante");
            frmNotas.HabilitarColumnas();
            // frmNotas.dgvListado.Columns["Subtotal"].HeaderText = "Restante";
            frmNotas.AgregarColumnaEdicion("ImporteAUsar", "Usar");
            frmNotas.AgregarColumnaSeleccion("Sel");
            frmNotas.dgvListado.SelectionMode = DataGridViewSelectionMode.CellSelect;
            
            // Se agrega la validación para cuando se le de aceptar
            decimal mImporteFaltante = Helper.ConvertirDecimal(oFactura.Cells["fac_Final"].Value);
            frmNotas.Aceptado += new EventHandler<FormClosingEventArgs>((s, e) =>
            {
                DataGridView oGrid = (s as EdicionListado).dgvListado;
                decimal mTotalNotas = 0;
                foreach (DataGridViewRow oFila in oGrid.Rows)
                {
                    if (!Helper.ConvertirBool(oFila.Cells["Sel"].Value)) continue;

                    decimal mAUsar = Helper.ConvertirDecimal(oFila.Cells["ImporteAUsar"].Value);
                    if (mAUsar > Helper.ConvertirDecimal(oFila.Cells["Restante"].Value))
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
                    if (Helper.ConvertirBool(oFila["Sel"]) && Helper.ConvertirDecimal(oFila["ImporteAUsar"]) > 0)
                    {
                        this.dgvAbonos.Rows.Add(null, oFactura.Cells["fac_MovimientoInventarioID"].Value, Cat.OrigenesPagosAProveedores.NotaDeCredito
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
                int iMovID = Helper.ConvertirEntero(oFila.Cells["abo_MovimientoInventarioID"].Value);
                decimal mImporte = Helper.ConvertirDecimal(oFila.Cells["abo_Importe"].Value);
                if (!oDescuentos.ContainsKey(iMovID))
                    oDescuentos.Add(iMovID, 0);
                oDescuentos[iMovID] += mImporte;
            }

            // Se aplica el descuento
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                int iMovID = Helper.ConvertirEntero(oFila.Cells["fac_MovimientoInventarioID"].Value);
                oFila.Cells["fac_Descuento"].Value = (oDescuentos.ContainsKey(iMovID) ? oDescuentos[iMovID] : 0);
                oFila.Cells["fac_Final"].Value = (Helper.ConvertirDecimal(oFila.Cells["fac_Saldo"].Value) - Helper.ConvertirDecimal(oFila.Cells["fac_Descuento"].Value));
            }

            this.CalcularImporteRestante();
        }

        private void AgregarPagoDeCaja(DataGridViewRow oFactura)
        {
            // Se obtiene la cuenta auxiliar correspondiente al proveedor
            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Proveedores
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
                if (Helper.ConvertirCadena(oFila.Cells["abo_Origen"].Value) == "Pago de Caja")
                {
                    int iEgresoID = Helper.ConvertirEntero(oFila.Cells["abo_CajaEgresoID"].Value);
                    if (!oUsados.ContainsKey(iEgresoID))
                        oUsados.Add(iEgresoID, 0);
                    oUsados[iEgresoID] += Helper.ConvertirDecimal(oFila.Cells["abo_Importe"].Value);
                }
            }

            // Se obtienen las notas de crédito disponibles y se restan los importes ya usados
            var oPagosCaja = General.GetListOf<CajaEgresosProveedoresView>(c => c.ContaCuentaAuxiliarID == oCuentaAux.ContaCuentaAuxiliarID
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
            decimal mImporteFaltante = Helper.ConvertirDecimal(oFactura.Cells["fac_Final"].Value);
            frmLista.Aceptado += new EventHandler<FormClosingEventArgs>((s, e) =>
            {
                DataGridView oGrid = (s as EdicionListado).dgvListado;
                decimal mTotal = 0;
                foreach (DataGridViewRow oFila in oGrid.Rows)
                {
                    if (!Helper.ConvertirBool(oFila.Cells["Sel"].Value)) continue;

                    decimal mAUsar = Helper.ConvertirDecimal(oFila.Cells["ImporteAUsar"].Value);
                    if (mAUsar > Helper.ConvertirDecimal(oFila.Cells["Restante"].Value))
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
                    if (Helper.ConvertirBool(oFila["Sel"]) && Helper.ConvertirDecimal(oFila["ImporteAUsar"]) > 0)
                    {
                        this.dgvAbonos.Rows.Add(null, oFactura.Cells["fac_MovimientoInventarioID"].Value, Cat.OrigenesPagosAProveedores.PagoDeCaja
                            , oFila["CajaEgresoID"], null, oFactura.Cells["fac_Factura"].Value, "Pago de Caja", null, null, oFila["ImporteAUsar"]
                            , null, oFila["Concepto"]);
                    }
                }
            }
            frmLista.Dispose();

            this.CalcularDescuentos();
        }

        private bool AccionGuardar()
        {
            if (!this.Validaciones())
                return false;

            // Se verifican los totales
            decimal mImportePago = Helper.ConvertirDecimal(this.txtImporte.Text);
            decimal mFaltanteTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
                mFaltanteTotal += Helper.ConvertirDecimal(oFila.Cells["fac_Final"].Value);
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

            Cargando.Mostrar();

            // Se comienza a guardar la información
            var poliza = new ProveedorPoliza
            {
                ProveedorID = ProveedorId,
                FechaPago = this.dtpFechaMovimiento.Value,
                ImportePago = mImportePago,
                BancoCuentaID = Helper.ConvertirEntero(this.cmbCuentaBancaria.SelectedValue),
                TipoFormaPagoID = Helper.ConvertirEntero(this.cboFormaPago.SelectedValue),
                FolioDePago = this.txtDocumento.Text,
                UsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
            };
            Guardar.Generico<ProveedorPoliza>(poliza);
            
            // Se guarda el detalle de la poliza
            var oPolizaDetalle = new List<ProveedorPolizaDetalle>();
            var oIdsPagosDeCaja = new List<int>();
            // Se agregan los pagos directos, uno para cada factura
            decimal mTotalRestante = mImportePago;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                int iMovID = Helper.ConvertirEntero(oFila.Cells["fac_MovimientoInventarioID"].Value);
                decimal mImporte = Helper.ConvertirDecimal(oFila.Cells["fac_Final"].Value);
                if (mImporte <= 0) continue;

                // Se determina el importe, considerando lo que resta
                if (mTotalRestante > 0)
                    mImporte = (mImporte < mTotalRestante ? mImporte : mTotalRestante);
                else
                    break;
                
                var polizaDetalle = new ProveedorPolizaDetalle
                {
                    ProveedorPolizaID = poliza.ProveedorPolizaID,
                    MovimientoInventarioID = iMovID,
                    OrigenID = Cat.OrigenesPagosAProveedores.PagoDirecto,
                    Importe = mImporte,
                    Folio = this.txtDocumento.Text
                };
                Guardar.Generico<ProveedorPolizaDetalle>(polizaDetalle);
                oPolizaDetalle.Add(polizaDetalle);

                // Se marca cada factura como liquidada, si ya se pagó en su totalidad
                if (mImporte == Helper.ConvertirDecimal(oFila.Cells["fac_Final"].Value))
                {
                    var oMovFact = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovID && c.Estatus);
                    oMovFact.FueLiquidado = true;
                    Guardar.Generico<MovimientoInventario>(oMovFact);
                }

                mTotalRestante -= mImporte;

                // Se llena el dato del id asignado al registro de abono
                // No se recuerda para qué se había puesto la sig. línea, pero al parecer no se necesita, x eso se comentó.
                // oFila.Cells["abo_ProveedorPolizaDetalleID"].Value = polizaDetalle.ProveedorPolizaDetalleID;
            }
            // Se agregan los descuentos
            foreach (DataGridViewRow oFila in this.dgvAbonos.Rows)
            {
                decimal mImporte = Helper.ConvertirDecimal(oFila.Cells["abo_Importe"].Value);
                int iNotaDeCreditoID = Helper.ConvertirEntero(oFila.Cells["abo_NotaDeCreditoID"].Value);
                int iOrigenID = Helper.ConvertirEntero(oFila.Cells["abo_OrigenID"].Value);
                string sOrigen = Helper.ConvertirCadena(oFila.Cells["abo_Origen"].Value);
                int iCajaEgresoID = Helper.ConvertirEntero(oFila.Cells["abo_CajaEgresoID"].Value);

                var polizaDetalle = new ProveedorPolizaDetalle
                {
                    ProveedorPolizaID = poliza.ProveedorPolizaID,
                    MovimientoInventarioID = Helper.ConvertirEntero(oFila.Cells["abo_MovimientoInventarioID"].Value),
                    OrigenID = iOrigenID,
                    Importe = mImporte,
                    NotaDeCreditoID = (iNotaDeCreditoID > 0 ? (int?)iNotaDeCreditoID : null),
                    CajaEgresoID = (iCajaEgresoID > 0 ? (int?)iCajaEgresoID : null),
                    Observacion = Helper.ConvertirCadena(oFila.Cells["abo_Observacion"].Value),
                    Folio = (iNotaDeCreditoID > 0 ? Helper.ConvertirCadena(oFila.Cells["abo_NotaDeCredito"].Value) : null)
                };
                Guardar.Generico<ProveedorPolizaDetalle>(polizaDetalle);
                oPolizaDetalle.Add(polizaDetalle);

                // Se marcan las notas de crédito como no disponibles, si ya se utilizaron por completo
                if (iNotaDeCreditoID > 0)
                    AdmonProc.VerMarcarDisponibilidadNotaDeCreditoProveedor(iNotaDeCreditoID);

                // Se marca el gasto de caja como afectado en proveedores, si aplica
                if (iCajaEgresoID > 0)
                {
                    // if (iOrigenID == Cat.OrigenesPagosAProveedores.PagoDeCaja)  // No sé x q se hacía está validación. No le vi caso - Moi 2015-08-17
                    // {
                        if (General.Exists<CajaEgresosProveedoresView>(c => c.CajaEgresoID == iCajaEgresoID && c.Restante <= 0))
                        {
                            var oGasto = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iCajaEgresoID && c.Estatus);
                            oGasto.AfectadoEnProveedores = true;
                            Guardar.Generico<CajaEgreso>(oGasto);    
                        }

                        // Se agrega el CajaEgresoID a la lista oIdsPagosDeCaja para después mandarlos a la afectación contable (Pólizas)
                        oIdsPagosDeCaja.Add(iCajaEgresoID);
                    // }
                }

                // Se llena el dato del id asignado al registro de abono
                oFila.Cells["abo_ProveedorPolizaDetalleID"].Value = polizaDetalle.ProveedorPolizaDetalleID;
            }
            
            // Se guarda el movimiento bancario
            if (mImportePago > 0)
            {
                var oProveedor = General.GetEntity<Proveedor>(c => c.ProveedorID == ProveedorId && c.Estatus);
                var oMovBanc = new BancoCuentaMovimiento()
                {
                    BancoCuentaID = Helper.ConvertirEntero(this.cmbCuentaBancaria.SelectedValue),
                    EsIngreso = false,
                    Fecha = poliza.FechaPago,
                    FechaAsignado = poliza.FechaPago,
                    Importe = mImportePago,
                    Concepto = oProveedor.NombreProveedor,
                    Referencia = poliza.ProveedorPolizaID.ToString(),
                    TipoFormaPagoID = Helper.ConvertirEntero(this.cboFormaPago.SelectedValue),
                    DatosDePago = this.txtDocumento.Text,
                    RelacionID = poliza.ProveedorPolizaID
                };
                ContaProc.RegistrarMovimientoBancario(oMovBanc);
            }

            // Se manda a afectar contabilidad (AfeConta)
            foreach (var oReg in oPolizaDetalle)
            {
                var oMovInv = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == oReg.MovimientoInventarioID && c.Estatus);
                switch (oReg.OrigenID)
                {
                    case Cat.OrigenesPagosAProveedores.NotaDeCredito:
                        int iNotaDeCreditoID = oReg.NotaDeCreditoID.Valor();
                        var oNota = General.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iNotaDeCreditoID);
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
                        var oGasto = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iGastoID && c.Estatus);
                        if (oGasto.Facturado.Valor())
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoGastoCajaFacturado, oReg.ProveedorPolizaDetalleID
                                , oGasto.FolioFactura, oGasto.Concepto, poliza.FechaPago);
                        else
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoCompraCreditoGastoCaja, oReg.ProveedorPolizaDetalleID
                                , "PAGO CAJA", oGasto.Concepto, poliza.FechaPago);
                        break;
                    case Cat.OrigenesPagosAProveedores.PagoDirecto:
                        if (poliza.BancoCuentaID == Cat.CuentasBancarias.Banamex || poliza.BancoCuentaID == Cat.CuentasBancarias.Scotiabank)
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
            //new Notificacion("Poliza Guardada exitosamente", 2 * 1000).Mostrar(Principal.Instance);
            var resp = Negocio.Helper.MensajePregunta(string.Format("{0}{1}{2}", "Poliza generada exitosamente con el Número: ", poliza.ProveedorPolizaID, ", desea ver reporte?"), GlobalClass.NombreApp);
            if (resp == DialogResult.Yes)
            {
                var oPoliza = General.GetEntity<ProveedoresPolizasView>(pro => pro.ProveedorPolizaID == poliza.ProveedorPolizaID);
                var oAbonos = General.GetListOf<ProveedoresPolizasDetalleAvanzadoView>(pr => pr.ProveedorPolizaID == poliza.ProveedorPolizaID);
                var oFacturas = new List<ProveedoresComprasView>();
                var oIdsFacturas = oAbonos.Select(c => c.MovimientoInventarioID).Distinct();
                foreach (int iMovId in oIdsFacturas)
                    oFacturas.Add(General.GetEntity<ProveedoresComprasView>(c => c.MovimientoInventarioID == iMovId));

                // Se genera el ticket
                var oRep = new Report();
                oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "ProveedorPoliza.frx");
                oRep.RegisterData(new List<ProveedoresPolizasView>() { oPoliza }, "Poliza");
                oRep.RegisterData(oAbonos, "Abonos");
                oRep.RegisterData(oFacturas, "Facturas");
                //oRep.GetDataSource("Pagos").Enabled = true;
                UtilLocal.EnviarReporteASalida("Reportes.ProveedoresPolizas.Salida", oRep);
            }
            catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarMovimientosNoPagados(ProveedorId));
            // catalogosProveedores.Instance.CustomInvoke<catalogosProveedores>(m => m.CargarPagosParcialesYdevoluciones(0, null));

            return true;
        }

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            if (this.txtImporte.Text == "")
                this.cntError.PonerError(this.txtImporte, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtDocumento.Text == "")
                this.cntError.PonerError(this.txtDocumento, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
            if (this.txtBeneficiario.Text == "")
                this.cntError.PonerError(this.txtBeneficiario, "El campo es necesario.", ErrorIconAlignment.MiddleRight);

            /* foreach (DataGridViewRow fila in this.dgvDetalle.Rows)
            {
                if (Negocio.Helper.ConvertirDecimal(fila.Cells["Pago"].Value) <= 0)
                    this.cntError.PonerError(this.dgvDetalle, "No se puede guardar movimientos con 0.0 como pago", ErrorIconAlignment.MiddleRight);
            }
            */

            return (this.cntError.NumeroDeErrores == 0);
        }

        #endregion

    }
}
