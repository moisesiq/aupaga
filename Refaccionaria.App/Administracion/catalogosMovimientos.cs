using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class catalogosMovimientos : UserControl
    {
        delegate void SetColumnNameFocusCaptura(string columnName, int rowIndex);
        delegate void SetColumnNameFocusDiferencia(string columnName, int rowIndex);

        Proveedor proveedor;
        MovimientoInventario movimientoInventario;
        ControlError cntError = new ControlError();
        DataTable dtDetalleConceptos = new DataTable();
        DataTable dtDetalleDescuentos = new DataTable();
        DataTable dtHistorialDescuentos = new DataTable();
        DataTable dtDiferencia = new DataTable();
        DataTable dtProntoPago = new DataTable();
        bool sel = true;
        bool finalizo = false;
        CamHelper camDevice = null;
        bool bRecalcularPreciosPorcentajes = true;
        int iCompraDePedidoID = 0;

        public static catalogosMovimientos Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly catalogosMovimientos instance = new catalogosMovimientos();
        }

        public catalogosMovimientos()
        {
            InitializeComponent();
            this.tabDetalleOperacion.Appearance = TabAppearance.FlatButtons;
            this.tabDetalleOperacion.ItemSize = new Size(0, 1);
            this.tabDetalleOperacion.SizeMode = TabSizeMode.Fixed;
        }

        private void catalogosMovimientos_Load(object sender, EventArgs e)
        {
            this.cargaInicial();
            this.ActiveControl = this.txtFolioFactura;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (this.ActiveControl != this.txtNumeroPedido)
                {
                    this.SelectNextControl(this.ActiveControl, true, true, true, true);
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void tabDetalleOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Entrada Inventario
            if (this.cboTipoOperacion.SelectedIndex == 0)
            {
                this.btnBack.Enabled = true;
                this.btnNext.Enabled = true;
                this.btnFinish.Enabled = true;

                switch (this.tabDetalleOperacion.SelectedIndex)
                {
                    case 0:
                        this.btnBack.Enabled = false;
                        this.btnNext.Enabled = true;
                        this.btnFinish.Enabled = false;
                        break;

                    case 1:
                        this.btnBack.Enabled = true;
                        this.btnNext.Enabled = true;
                        this.btnFinish.Enabled = false;
                        this.cboProveedor_SelectedValueChanged(sender, null);
                        break;

                    case 2:
                        this.btnBack.Enabled = true;
                        this.btnNext.Enabled = true;
                        this.btnFinish.Enabled = false;
                        break;

                    case 3:
                        this.btnBack.Enabled = true;
                        this.btnNext.Enabled = false;
                        this.btnFinish.Enabled = true;

                        foreach (Negocio.CamHelper device in Negocio.CamHelper.GetDevices())
                            camDevice = device;
                        if (camDevice != null)
                        {
                            this.lblEstatusCamara.Text = string.Format("{0}{1}", "Estatus:", " Ok");
                            this.btnIniciarCamara.Enabled = true;
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        private void txtNumeroPedido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.btnBuscarPedido_Click(sender, e);
        }

        private void btnBuscarPedido_Click(object sender, EventArgs e)
        {
            int iPedidoID = Helper.ConvertirEntero(this.txtNumeroPedido.Text);

            // Se valida que el pedido sea válido y que el proveedor corresponda
            var oPedido = General.GetEntity<Pedido>(c => c.PedidoID == iPedidoID && c.Estatus);
            if (oPedido == null)
            {
                UtilLocal.MensajeAdvertencia("El Pedido especificado no existe o es inválido.");
                return;
            }
            if (oPedido.ProveedorID != Helper.ConvertirEntero(this.cboProveedor.SelectedValue))
            {
                UtilLocal.MensajeAdvertencia("El Pedido no corresponde al Proveedor seleccionado.");
                return;
            }

            var frmLista = new CompraDePedido(iPedidoID);
            if (frmLista.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.PonerPedido(frmLista.Seleccion);
            frmLista.Dispose();
            this.txtNumeroPedido.Clear();
        }

        private void PonerPedido(List<object> oPedido)
        {
            this.dtDetalleConceptos.Rows.Clear();
            this.dtDetalleDescuentos.Rows.Clear();
            this.dtDiferencia.Rows.Clear();
            foreach (PedidosDetalleView oReg in oPedido)
            {
                int iFila = this.AgregarParteDetalleCaptura(oReg.ParteID);
                if (iFila >= 0)
                {
                    this.dtDetalleConceptos.Rows[iFila]["UNS"] = oReg.CantidadPedido;
                    bool bCostoDif = (Helper.ConvertirDecimal(this.dtDetalleConceptos.Rows[iFila]["Costo Inicial"]) != oReg.CostosUnitario);
                    this.dtDetalleConceptos.Rows[iFila]["Costo Inicial"] = oReg.CostosUnitario;
                    // this.dgvDetalleCaptura["UNS", iFila].Value = oReg.CantidadPedido;
                    // this.dgvDetalleCaptura["Costo Inicial", iFila].Value = oReg.CostosUnitario;
                    this.dgvDetalleCaptura_CellValueChanged(null, new DataGridViewCellEventArgs(this.dgvDetalleCaptura.Columns["UNS"].Index, iFila));
                    // this.dgvDetalleCaptura.RefreshEdit();
                }
            }
            // No se usa el tag del grid pues ese se modifica con la función "VerSeleccionNueva()" en el evento "CurrentCellChanged".
            // this.dgvDetalleCaptura.Tag = this.txtNumeroPedido.Text;
            this.iCompraDePedidoID = Helper.ConvertirEntero(this.txtNumeroPedido.Text);
        }

        private int AgregarParteDetalleCaptura(int parteId)
        {
            int ri = -1;
            var parte = Negocio.General.GetEntity<PartesBusquedaEnMovimientosView>(p => p.ParteID.Equals(parteId));

            if (!this.validarProveedorMarca(proveedor, parte.MarcaParteID) && Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) == 1)
            {
                var msj = string.Format("{0} {1} {2} {3} {4} {5}", "El Número de Parte:", parte.NumeroParte, "No puede ser agregado. La Marca:", parte.Marca, "no tiene relación con el Proveedor:", proveedor.NombreProveedor);
                Helper.MensajeError(msj, GlobalClass.NombreApp);
            }
            else
            {
                var rowIndex = Negocio.Helper.findRowIndex(this.dgvDetalleCaptura, "ParteID", parte.ParteID.ToString());
                if (rowIndex < 0)
                {
                    if (parte != null)
                    {
                        this.agregarDetalleCaptura(parte);
                        this.agregarDetalleDescuentos(parte);
                        this.agregarDetalleDiferencia(parte);
                    }
                }

                ri = Helper.findRowIndex(this.dgvDetalleCaptura, "ParteID", parte.ParteID.ToString());
                if (ri >= 0)
                {
                    var ci = this.dgvDetalleCaptura.Columns["Costo Inicial"].Index;
                    this.dgvDetalleCaptura_CellValueChanged(null, new DataGridViewCellEventArgs(ci, ri));
                }
            }

            return ri;
        }

        private void establecerInfoCaptura(int parteId)
        {
            try
            {
                if (parteId > 0)
                {
                    var info = Negocio.General.GetEntity<PartesBusquedaEnMovimientosView>(p => p.ParteID.Equals(parteId));
                    if (info != null)
                    {
                        this.lblNombreLinea.Text = info.Linea;
                        this.lblNombreMarca.Text = info.Marca;
                    }

                    var existencias = Negocio.General.GetListOf<ExistenciasView>(p => p.ParteID.Equals(parteId));
                    decimal existenciaLocal = 0;
                    decimal existenciaGlobal = 0;
                    if (existencias != null)
                    {

                        foreach (var existencia in existencias)
                        {
                            if (existencia.SucursalID.Equals(GlobalClass.SucursalID))
                            {
                                existenciaLocal += Negocio.Helper.ConvertirDecimal(existencia.Exist);
                            }
                            else
                            {
                                existenciaGlobal += Negocio.Helper.ConvertirDecimal(existencia.Exist);
                            }
                        }
                    }
                    this.lblExistencias.Text = string.Format("{0} / {1}", existenciaLocal, existenciaGlobal);
                }
                else
                {
                    this.lblNombreLinea.Text = string.Empty;
                    this.lblNombreMarca.Text = string.Empty;
                    this.lblExistencias.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void establecerTotalPorFilas(DataGridView dgv)
        {
            decimal sumUns = 0;
            decimal totalSinDesc = 0;
            var iva = 1 + (GlobalClass.ConfiguracionGlobal.IVA / 100);
            if (dgv.Name == "dgvDetalleCaptura")
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    sumUns += Negocio.Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                    totalSinDesc += Helper.ConvertirDecimal(row.Cells["Importe"].Value);
                }

                this.txtUnidades.Text = sumUns.ToString();
                this.txtArticulos.Text = this.dgvDetalleCaptura.Rows.Count.ToString();
                this.lblTotalSinDescuento.Text = Helper.DecimalToCadenaMoneda(totalSinDesc * iva);
            }

            decimal porcentajeSeguro = 0;
            decimal subtotal = 0;
            decimal importeSeguro = 0;
            decimal total = 0;

            subtotal = this.sacarImporteTotalPorFilas(dgv);

            if (proveedor != null)
            {
                porcentajeSeguro = (Helper.ConvertirDecimal(proveedor.Seguro) / 100);
            }

            if (porcentajeSeguro > 0)
            {
                importeSeguro = subtotal * porcentajeSeguro;
                total = (subtotal + importeSeguro) * iva;
            }
            else
            {
                total = subtotal * iva;
            }

            this.txtSubtotal.Text = Helper.DecimalToCadenaMoneda(subtotal);
            this.txtSeguro.Text = Helper.DecimalToCadenaMoneda(importeSeguro);
            this.lblTotal.Text = Helper.DecimalToCadenaMoneda(total);
            if (!this.chkImporteFactura.Checked)
                this.lblImporteFactura.Text = this.lblTotal.Text;
        }

        private decimal sacarImporteTotalPorFilas(DataGridView dgv)
        {
            decimal sumaImp = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                sumaImp += Negocio.Helper.ConvertirDecimal(row.Cells["Importe"].Value);
            }
            return sumaImp;
        }

        private void recorrerHistorialYCalcularTotal()
        {
            var iva = 1 + (GlobalClass.ConfiguracionGlobal.IVA / 100);
            this.establecerTotalPorFilas(this.dgvDetalleDescuentos);
            foreach (DataGridViewRow row in this.dgvHistorialDescuentos.Rows)
                if (Negocio.Helper.ConvertirEntero(row.Cells["TipoDescuento"].Value) > 0) //A Item, Individual y Marca Articulos
                    this.establecerTotalPorFilas(this.dgvDetalleDescuentos);

            this.txtTotalDescuentos.Text = "0.0";
            decimal ImporteConDescuento = 0;
            decimal total = Helper.ConvertirDecimal(this.lblTotal.Text);
            foreach (DataGridViewRow row in this.dgvHistorialDescuentos.Rows)
            {
                if (Negocio.Helper.ConvertirEntero(row.Cells["TipoDescuento"].Value) == -2) //Marca Factura
                {
                    //Primero: Se identifica el ParteID y su Importe del grid dgvDetalleDescuentos, se le aplica el descuento registrado
                    var parteId = row.Cells["ParteID"].Value.ToString();
                    foreach (DataGridViewRow fila in this.dgvDetalleDescuentos.Rows)
                    {
                        if (fila.Cells["ParteID"].Value.ToString().Equals(parteId))
                        {
                            var importe = Helper.ConvertirDecimal(fila.Cells["Importe"].Value);
                            var desUno = Helper.ConvertirDecimal(row.Cells["DescuentoUno"].Value);
                            var desDos = Helper.ConvertirDecimal(row.Cells["DescuentoDos"].Value);
                            var desTres = Helper.ConvertirDecimal(row.Cells["DescuentoTres"].Value);
                            var desCuatro = Helper.ConvertirDecimal(row.Cells["DescuentoCuatro"].Value);
                            var desCinco = Helper.ConvertirDecimal(row.Cells["DescuentoCinco"].Value);

                            var resultado = this.calcularDescuento(importe, desUno, desDos, desTres, desCuatro, desCinco);
                            resultado = importe - resultado;
                            //Segundo: Se suma el importe de descuento y se resta en el total de la factura

                            ImporteConDescuento += (resultado * iva);
                        }
                    }
                    //Resultado en el total de la factura (ImporteConDescuento - Total)                    
                    this.txtTotalDescuentos.Text = Helper.DecimalToCadenaMoneda(ImporteConDescuento);
                    this.lblTotal.Text = Helper.DecimalToCadenaMoneda(total - ImporteConDescuento);
                    if (!this.chkImporteFactura.Checked)
                        this.lblImporteFactura.Text = this.lblTotal.Text;
                    this.txtSubtotal.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.lblTotal.Text) / iva);
                }
            }

            foreach (DataGridViewRow row in this.dgvHistorialDescuentos.Rows)
                if (Negocio.Helper.ConvertirEntero(row.Cells["TipoDescuento"].Value) == -1) //A Factura
                {
                    var importe = Negocio.Helper.ConvertirDecimal(this.lblTotal.Text);
                    var desUno = Negocio.Helper.ConvertirDecimal(row.Cells["DescuentoUno"].Value);
                    var desDos = Negocio.Helper.ConvertirDecimal(row.Cells["DescuentoDos"].Value);
                    var desTres = Negocio.Helper.ConvertirDecimal(row.Cells["DescuentoTres"].Value);
                    var desCuatro = Negocio.Helper.ConvertirDecimal(row.Cells["DescuentoCuatro"].Value);
                    var desCinco = Negocio.Helper.ConvertirDecimal(row.Cells["DescuentoCinco"].Value);

                    var resultado = this.calcularDescuento(importe, desUno, desDos, desTres, desCuatro, desCinco);
                    var oper = Helper.ConvertirDecimal(this.lblTotal.Text) - resultado;
                    this.txtTotalDescuentos.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.txtTotalDescuentos.Text) + oper);
                    this.lblTotal.Text = Negocio.Helper.DecimalToCadenaMoneda(resultado);
                    if (!this.chkImporteFactura.Checked)
                        this.lblImporteFactura.Text = this.lblTotal.Text;
                    this.txtSubtotal.Text = Helper.DecimalToCadenaMoneda(Helper.ConvertirDecimal(this.lblTotal.Text) / iva);
                }

            this.sacarImporteProntoPago();
        }

        private void sacarImporteProntoPago()
        {
            try
            {
                if (proveedor != null)
                {
                    DataRow row;
                    var datosProntoPago = General.GetListOf<ProveedorProntoPagosView>(p => p.ProveedorID.Equals(proveedor.ProveedorID));
                    if (datosProntoPago != null)
                    {
                        dtProntoPago.Clear();
                        this.dgvProntoPago.Refresh();
                        if (this.dgvProntoPago.RowCount > 0)
                        {
                            if (this.dgvProntoPago.Rows.Count > 0)
                                this.dgvProntoPago.Rows.Clear();

                            if (this.dgvProntoPago.Columns.Count > 0)
                                this.dgvProntoPago.Columns.Clear();
                        }

                        foreach (var prontoPago in datosProntoPago)
                        {
                            row = dtProntoPago.NewRow();
                            row["Dias"] = prontoPago.NumeroDias;
                            row["%"] = prontoPago.PorcentajeDescuento;
                            var total = Helper.ConvertirDecimal(this.lblTotal.Text);
                            var oper = total * (prontoPago.PorcentajeDescuento / 100);
                            var res = total - oper;
                            row["Importe"] = Helper.DecimalToCadenaMoneda(res);

                            dtProntoPago.Rows.Add(row);
                        }
                        this.dgvProntoPago.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private bool Validaciones()
        {
            this.cntError.LimpiarErrores();
            if (Negocio.Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) == 1 && this.txtFolioFactura.Text == "")
            {
                this.cntError.PonerError(this.txtFolioFactura, "El campo es necesario.", ErrorIconAlignment.MiddleRight);
                this.txtFolioFactura.Focus();
            }
            int iTipoOp = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue);
            if ((iTipoOp == Cat.TiposDeOperacionMovimientos.EntradaCompra || iTipoOp == Cat.TiposDeOperacionMovimientos.DevolucionAProveedor) 
                && Helper.ConvertirEntero(this.cboProveedor.SelectedValue) < 1)
            {
                this.txtNumeroParte.Clear();
                this.cntError.PonerError(this.cboProveedor);
            }
            return (this.cntError.NumeroDeErrores == 0);
        }

        private void finalizar()
        {
            finalizo = true;
            this.cargaInicial();
            this.establecerTotalPorFilas(this.dgvDetalleCaptura);
            //this.establecerInfoCaptura(-1);
            this.establecerInfoDescuentos(-1);
            this.picBoxImagen.Image = null;
            this.btnDetenerCamara_Click(null, null);
            finalizo = false;
        }

        #region [ Wizard ]

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var res = Negocio.Helper.MensajePregunta("Esta seguro de que desea cancelar?", GlobalClass.NombreApp);
            if (res.Equals(DialogResult.Yes))
            {
                this.finalizar();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.chkImporteFactura.Enabled = false;

            //Entrada Inventario
            if (this.cboTipoOperacion.SelectedIndex == 0)
            {
                switch (this.tabDetalleOperacion.SelectedIndex)
                {
                    case 0:

                        break;

                    case 1:
                        this.tabDetalleOperacion.SelectedIndex = 0;
                        break;

                    case 2:
                        this.chkImporteFactura.Enabled = true;
                        this.tabDetalleOperacion.SelectedIndex = 1;
                        break;

                    case 3:
                        this.tabDetalleOperacion.SelectedIndex = 2;
                        break;

                    default:
                        break;
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.chkImporteFactura.Enabled = (this.tabDetalleOperacion.SelectedIndex == 1);

            if (this.cboTipoOperacion.SelectedIndex == 0) //Entrada Inventario
            {
                switch (this.tabDetalleOperacion.SelectedIndex)
                {
                    case 0:
                        if (this.ValidarCaptura())
                        {
                            this.tabDetalleOperacion.SelectedIndex = 1;
                            this.chkImporteFactura.Enabled = true;
                        }
                        break;

                    case 1:
                        if (this.ValidarDescuentos())
                            this.tabDetalleOperacion.SelectedIndex = 2;
                        break;

                    case 2:
                        // Se validan los precios
                        if (!this.ValidarPrecios())
                            return;
                        this.tabDetalleOperacion.SelectedIndex = 3;
                        break;

                    default:
                        break;
                }
            }
        }
                
        private bool ValidarCaptura()
        {
            // Se valida si la cantidad sobrepasa el MaxMin
            bool bValido = true;
            foreach (DataGridViewRow oFila in this.dgvDetalleCaptura.Rows)
            {
                oFila.DefaultCellStyle.ForeColor = Color.Black;

                int iParteID = Helper.ConvertirEntero(oFila.Cells["ParteID"].Value);
                // Si es un 9500, no se valida
                if (General.Exists<Parte>(c => c.ParteID == iParteID && c.Es9500 == true && c.Estatus))
                    continue;

                // int iSucursalID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue);
                decimal mCantidad = Helper.ConvertirDecimal(oFila.Cells["UNS"].Value);
                decimal mNecesidad = General.GetListOf<PartesExistenciasMaxMinView>(c => c.ParteID == iParteID).Sum(c => c.Maximo - c.Existencia).Valor();
                if (mCantidad > mNecesidad)
                {
                    oFila.ErrorText = "La Cantidad especificada supera el Máximo del artículo, considerando todas las sucursales. Verificar.";
                    oFila.DefaultCellStyle.ForeColor = Color.Red;
                    bValido = false;
                }
            }

            //
            if (!bValido)
            {
                bValido = 
                    (UtilLocal.MensajePregunta("Existe uno o más artículos que quedarían con una Cantidad mayor a su Máximo según el movimiento específicado. ¿Deseas continuar?")
                    == DialogResult.Yes);
            }

            return bValido;
        }

        private bool ValidarDescuentos()
        {
            this.cntError.LimpiarErrores();

            // Se valida que se haya cuadrado el importe con la factura en papel
            if (!this.chkImporteFactura.Checked)
                this.cntError.PonerError(this.chkImporteFactura, "Debes cuadrar el importe de la factura.");

            return (this.cntError.Valido);
        }

        private bool ValidarFinal()
        {
            this.cntError.LimpiarErrores();
            if (Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) != Cat.TiposDeOperacionMovimientos.EntradaCompra && this.txtObservaciones.Text == "")
                this.cntError.PonerError(this.txtObservaciones, "Es necesario poner una observación.", ErrorIconAlignment.MiddleLeft);

            return this.cntError.Valido;
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.ValidarFinal())
                    return;

                int iAutorizoID = 0;
                var ResU = UtilDatos.ValidarObtenerUsuario(null, "Autorización");
                if (ResU.Exito)
                    iAutorizoID = ResU.Respuesta.UsuarioID;
                else
                {
                    Helper.MensajeError("Error al validar el usuario.", GlobalClass.NombreApp);
                    return;
                }

                var res = Helper.MensajePregunta("¿Está seguro de que la información es correcta?", GlobalClass.NombreApp);
                if (res == DialogResult.No)
                    return;

                SplashScreen.Show(new Splash());
                this.btnFinish.Enabled = false;

                var proveedorId = Helper.ConvertirEntero(this.cboProveedor.SelectedValue);
                var sucursalId = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue);
                var iva = 1 + (GlobalClass.ConfiguracionGlobal.IVA / 100);

                switch (Helper.ConvertirEntero(cboTipoOperacion.SelectedValue))
                {
                    case 1:

                        #region [ Entrada Compra ]

                        // Se valida que no se haya utilizado ese folio de factura
                        var folio = General.GetEntity<MovimientoInventario>(m => m.ProveedorID == proveedorId
                            && m.FolioFactura == this.txtFolioFactura.Text && m.Estatus.Equals(true));
                        if (folio != null)
                        {
                            this.Cursor = Cursors.Default;
                            SplashScreen.Close();
                            var msj = string.Format("{0} {1} {2}", "El Folio Factura", this.txtFolioFactura.Text, "Ya fue utilizado con el proveedor seleccionado.");
                            Helper.MensajeError(msj, GlobalClass.NombreApp);
                            return;
                        }

                        //Insertar Movimiento
                        var subtotal = Helper.ConvertirDecimal(this.txtSubtotal.Text);
                        var importeTotal = Helper.ConvertirDecimal(this.lblTotal.Text);
                        var movimientoEntrada = new MovimientoInventario()
                        {
                            TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue),
                            TipoPagoID = Helper.ConvertirEntero(this.cboTipoPago.SelectedValue),
                            ProveedorID = proveedorId,
                            SucursalOrigenID = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue),
                            SucursalDestinoID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue),
                            FechaFactura = this.dtpFechaFacturacion.Value,
                            FechaRecepcion = this.dtpFechaRecepcion.Value,
                            FolioFactura = this.txtFolioFactura.Text,

                            AplicaEnMovimientoInventarioID = null,
                            FechaAplicacion = null,

                            Subtotal = subtotal,
                            IVA = importeTotal - subtotal,
                            ImporteTotal = importeTotal,
                            FueLiquidado = false,
                            TipoConceptoOperacionID = null,
                            Observacion = this.txtObservaciones.Text,
                            Articulos = Helper.ConvertirDecimal(this.txtArticulos.Text),
                            Unidades = Helper.ConvertirDecimal(this.txtUnidades.Text),
                            Seguro = Helper.ConvertirDecimal(this.txtSeguro.Text),
                            ImporteTotalSinDescuento = Helper.ConvertirDecimal(this.lblTotalSinDescuento.Text),
                            ImporteFactura = Helper.ConvertirDecimal(this.lblImporteFactura.Text)
                        };
                        Guardar.Generico<MovimientoInventario>(movimientoEntrada);

                        if (movimientoEntrada.MovimientoInventarioID < 1)
                        {
                            this.Cursor = Cursors.Default;
                            SplashScreen.Close();
                            new EntityNotFoundException("MovimientoInventarioID", "MovimientoInventario");
                            return;
                        }

                        //Insertar MovimientoDetalle
                        foreach (DataGridViewRow row in this.dgvDetalleDescuentos.Rows)
                        {
                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var cantidad = Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                            var detalleMovimiento = new MovimientoInventarioDetalle()
                            {
                                MovimientoInventarioID = movimientoEntrada.MovimientoInventarioID,
                                ParteID = parteId,
                                Cantidad = cantidad,
                                PrecioUnitario = Helper.ConvertirDecimal(row.Cells["Unitario"].Value),
                                Importe = Helper.ConvertirDecimal(row.Cells["Importe"].Value),
                                FueDevolucion = false,
                                FechaRegistro = DateTime.Now,
                                Estatus = true
                            };
                            General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimiento, detalleMovimiento);

                            //Actualizar ParteExistencia
                            var oParte = General.GetEntity<Parte>(c => c.ParteID == parteId && c.Estatus);
                            if (!oParte.EsServicio.Valor())
                            {
                                var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId);
                                if (existencia != null)
                                {
                                    var inicial = existencia.Existencia;
                                    existencia.Existencia += cantidad;
                                    Guardar.Generico<ParteExistencia>(existencia);

                                    var historial = new MovimientoInventarioHistorial()
                                    {
                                        MovmientoInventarioID = movimientoEntrada.MovimientoInventarioID,
                                        ParteID = parteId,
                                        ExistenciaInicial = Helper.ConvertirDecimal(inicial),
                                        ExistenciaFinal = Helper.ConvertirDecimal(existencia.Existencia),
                                        SucursalID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue)
                                    };
                                    Guardar.Generico<MovimientoInventarioHistorial>(historial);
                                }
                            }

                            // Se afectan los datos del pedido, si aplica
                            // if (this.dgvDetalleCaptura.Tag != null)
                            if (this.iCompraDePedidoID > 0)
                            {
                                int iPedidoID = this.iCompraDePedidoID; // Helper.ConvertirEntero(this.dgvDetalleCaptura.Tag);
                                var oPartesPed = General.GetListOf<PedidoDetalle>(c => c.PedidoID == iPedidoID && c.PedidoEstatusID == Cat.PedidosEstatus.NoSurtido
                                    && c.Estatus);
                                var oPartePed = oPartesPed.FirstOrDefault(c => c.ParteID == parteId);
                                if (oPartePed != null)
                                {
                                    oPartePed.PedidoEstatusID = Cat.PedidosEstatus.Surtido;
                                    Guardar.Generico<PedidoDetalle>(oPartePed);
                                    // Se verifica si ya no quedan pedidos pendientes, para marcar como Surtido
                                    if (oPartesPed.Count == 1)
                                    {
                                        var oPedido = General.GetEntity<Pedido>(c => c.PedidoID == oPartePed.PedidoID && c.Estatus);
                                        oPedido.PedidoEstatusID = Cat.PedidosEstatus.Surtido;
                                        Guardar.Generico<Pedido>(oPedido);
                                    }

                                }
                            }

                            // Se agrega al Kardex
                            int iFilaDif = this.dgvDiferencia.EncontrarIndiceDeValor("ParteID", parteId);
                            AdmonProc.RegistrarKardex(new ParteKardex()
                            {
                                ParteID = parteId,
                                OperacionID = Cat.OperacionesKardex.EntradaCompra,
                                SucursalID = sucursalId,
                                Folio = movimientoEntrada.FolioFactura,
                                Fecha = DateTime.Now,
                                RealizoUsuarioID = iAutorizoID,
                                Entidad = this.cboProveedor.Text,
                                Origen = proveedorId.ToString(),
                                Destino = this.cboUbicacionDestino.Text,
                                Cantidad = cantidad,
                                Importe = Helper.ConvertirDecimal(this.dgvDiferencia["Costo Nuevo", iFilaDif].Value)
                            });
                            
                        }

                        // Para limpiar el estatus de relación con un pedido
                        // this.dgvDetalleCaptura.Tag = null;
                        this.iCompraDePedidoID = 0;

                        //Insertar MovimientoDescuentos
                        foreach (DataGridViewRow row in this.dgvHistorialDescuentos.Rows)
                        {
                            int tipoDescuento = Helper.ConvertirEntero(row.Cells["TipoDescuento"].Value);
                            int tipoDescuentoId = 1;
                            switch (tipoDescuento)
                            {
                                case -1:
                                    tipoDescuentoId = 2;
                                    break;

                                case 2:
                                    tipoDescuentoId = 3;
                                    break;

                                case 3:
                                    tipoDescuentoId = 4;
                                    break;

                                case -2:
                                    tipoDescuentoId = 5;
                                    break;
                            }

                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var hisDesc = new MovimientoInventarioDescuento()
                            {
                                MovimientoInventarioID = movimientoEntrada.MovimientoInventarioID,
                                ParteID = parteId,
                                TipoDescuentoID = tipoDescuentoId,
                                DescuentoUno = Helper.ConvertirDecimal(row.Cells["DescuentoUno"].Value),
                                DescuentoDos = Helper.ConvertirDecimal(row.Cells["DescuentoDos"].Value),
                                DescuentoTres = Helper.ConvertirDecimal(row.Cells["DescuentoTres"].Value),
                                DescuentoCuatro = Helper.ConvertirDecimal(row.Cells["DescuentoCuatro"].Value),
                                DescuentoCinco = Helper.ConvertirDecimal(row.Cells["DescuentoCinco"].Value)
                            };
                            Guardar.Generico<MovimientoInventarioDescuento>(hisDesc);
                        }

                        //Actualizar PartePrecio
                        foreach (DataGridViewRow row in this.dgvDiferencia.Rows)
                        {
                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var partePrecio = General.GetEntity<PartePrecio>(p => p.ParteID.Equals(parteId));
                            var costoNuevo = Helper.ConvertirDecimal(row.Cells["Costo Nuevo"].Value);
                            var costoActual = Helper.ConvertirDecimal(row.Cells["Costo Actual"].Value);
                            decimal costoConDescuento = 0;
                            if (partePrecio != null)
                            {
                                if (Convert.ToBoolean(row.Cells["X"].Value).Equals(true))
                                    partePrecio.Costo = costoNuevo;
                                else
                                    partePrecio.Costo = costoActual;

                                partePrecio.PorcentajeUtilidadUno = Helper.ConvertirDecimal(row.Cells["%1"].Value);
                                partePrecio.PorcentajeUtilidadDos = Helper.ConvertirDecimal(row.Cells["%2"].Value);
                                partePrecio.PorcentajeUtilidadTres = Helper.ConvertirDecimal(row.Cells["%3"].Value);
                                partePrecio.PorcentajeUtilidadCuatro = Helper.ConvertirDecimal(row.Cells["%4"].Value);
                                partePrecio.PorcentajeUtilidadCinco = Helper.ConvertirDecimal(row.Cells["%5"].Value);

                                partePrecio.PrecioUno = Helper.ConvertirDecimal(row.Cells["Precio 1"].Value);
                                partePrecio.PrecioDos = Helper.ConvertirDecimal(row.Cells["Precio 2"].Value);
                                partePrecio.PrecioTres = Helper.ConvertirDecimal(row.Cells["Precio 3"].Value);
                                partePrecio.PrecioCuatro = Helper.ConvertirDecimal(row.Cells["Precio 4"].Value);
                                partePrecio.PrecioCinco = Helper.ConvertirDecimal(row.Cells["Precio 5"].Value);

                                //CostoConDescuento, calcular el descuento por fila, si existen descuentos a Factura y a Marca Factura
                                var rowIndex = Helper.findRowIndex(this.dgvDetalleDescuentos, "ParteID", parteId.ToString());
                                if (rowIndex != -1)
                                {
                                    decimal precioConDescuento = Helper.ConvertirDecimal(this.dgvDetalleDescuentos.Rows[rowIndex].Cells["Unitario"].Value);

                                    var rowIndexHisAFactura = Helper.findRowIndex(this.dgvHistorialDescuentos, "ParteID", "-1");
                                    if (rowIndexHisAFactura != -1)
                                    {
                                        var desUno = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAFactura].Cells["DescuentoUno"].Value);
                                        var desDos = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAFactura].Cells["DescuentoDos"].Value);
                                        var desTres = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAFactura].Cells["DescuentoTres"].Value);
                                        var desCuatro = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAFactura].Cells["DescuentoCuatro"].Value);
                                        var desCinco = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAFactura].Cells["DescuentoCinco"].Value);
                                        precioConDescuento = this.calcularDescuento(precioConDescuento, desUno, desDos, desTres, desCuatro, desCinco);
                                    }

                                    var rowIndexHisAMarcaF = -1;
                                    foreach (DataGridViewRow fila in this.dgvHistorialDescuentos.Rows)
                                    {
                                        if (Helper.ConvertirEntero(fila.Cells["ParteID"].Value).Equals(parteId) && Helper.ConvertirEntero(fila.Cells["TipoDescuento"].Value).Equals(-2))
                                        {
                                            rowIndexHisAMarcaF = fila.Index;
                                        }
                                    }
                                    if (rowIndexHisAMarcaF != -1)
                                    {
                                        var desUno = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAMarcaF].Cells["DescuentoUno"].Value);
                                        var desDos = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAMarcaF].Cells["DescuentoDos"].Value);
                                        var desTres = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAMarcaF].Cells["DescuentoTres"].Value);
                                        var desCuatro = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAMarcaF].Cells["DescuentoCuatro"].Value);
                                        var desCinco = Helper.ConvertirDecimal(this.dgvHistorialDescuentos.Rows[rowIndexHisAMarcaF].Cells["DescuentoCinco"].Value);
                                        precioConDescuento = this.calcularDescuento(precioConDescuento, desUno, desDos, desTres, desCuatro, desCinco);
                                    }

                                    partePrecio.CostoConDescuento = precioConDescuento;
                                }

                                Guardar.Generico<PartePrecio>(partePrecio);

                                //Si cambia de el valor del precio entonces
                                //Actualizar PartePrecioHistorico
                                if (costoActual != costoNuevo)
                                {
                                    var precioHis = new PartePrecioHistorico()
                                    {
                                        ParteID = parteId,
                                        MovimientoInventarioID = movimientoEntrada.MovimientoInventarioID,
                                        CostoNuevo = costoNuevo,
                                        CostoActual = costoActual,
                                        CostoConDescuento = costoConDescuento,
                                        PorcentajeUtilidadUno = Helper.ConvertirDecimal(row.Cells["%1"].Value),
                                        PorcentajeUtilidadDos = Helper.ConvertirDecimal(row.Cells["%2"].Value),
                                        PorcentajeUtilidadTres = Helper.ConvertirDecimal(row.Cells["%3"].Value),
                                        PorcentajeUtilidadCuatro = Helper.ConvertirDecimal(row.Cells["%4"].Value),
                                        PorcentajeUtilidadCinco = Helper.ConvertirDecimal(row.Cells["%5"].Value),
                                        PrecioUno = Helper.ConvertirDecimal(row.Cells["Precio 1"].Value),
                                        PrecioDos = Helper.ConvertirDecimal(row.Cells["Precio 2"].Value),
                                        PrecioTres = Helper.ConvertirDecimal(row.Cells["Precio 3"].Value),
                                        PrecioCuatro = Helper.ConvertirDecimal(row.Cells["Precio 4"].Value),
                                        PrecioCinco = Helper.ConvertirDecimal(row.Cells["Precio 5"].Value)
                                    };
                                    Guardar.Generico<PartePrecioHistorico>(precioHis);
                                }
                            }

                            //Actualizar MovimientoInventarioEtiqueta
                            var etiqueta = new MovimientoInventarioEtiqueta()
                            {
                                MovimientoInventarioID = movimientoEntrada.MovimientoInventarioID,
                                ParteID = parteId,
                                NumeroEtiquetas = Helper.ConvertirEntero(row.Cells["Etiqueta"].Value),
                            };
                            Guardar.Generico<MovimientoInventarioEtiqueta>(etiqueta);
                        }

                        //Guardar Imagen en Directorio
                        //Nombre Imagen = MovimientoID_FolioFactura
                        /* De momento no se usa. Se quitó por incompatiblidad con el parámetro "pathImagenesMovimientos". Moi 07/05/2015
                        if (this.picBoxImagen.Image != null)
                        {
                            var pathProveedor = System.IO.Path.Combine(GlobalClass.ConfiguracionGlobal.pathImagenesMovimientos, "Compras", this.cboProveedor.Text, DateTime.Now.Year.ToString());
                            if (!System.IO.Directory.Exists(pathProveedor))
                                System.IO.Directory.CreateDirectory(pathProveedor);

                            //Guardar Imagen en la tabla
                            movimientoEntrada.NombreImagen = string.Format("{0}_{1}{2}", movimientoEntrada.MovimientoInventarioID, movimientoEntrada.FolioFactura, ".jpg");
                            Guardar.Generico<MovimientoInventario>(movimientoEntrada);

                            var path = System.IO.Path.Combine(pathProveedor, movimientoEntrada.NombreImagen);
                            picBoxImagen.Image.Save(path, ImageFormat.Jpeg);
                        } */

                        // Se manda a afectar contabilidad (AfeConta)
                        if (this.chkEsNota.Checked)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.CompraCreditoNota, movimientoEntrada.MovimientoInventarioID
                                , movimientoEntrada.FolioFactura, this.cboProveedor.Text);
                        else
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.CompraCreditoFactura, movimientoEntrada.MovimientoInventarioID
                                , movimientoEntrada.FolioFactura, this.cboProveedor.Text);

                        SplashScreen.Close();
                        this.btnFinish.Enabled = true;
                        this.Cursor = Cursors.Default;

                        //Visor de ticket de entrada compra
                        ReporteadorMovimientos visor = ReporteadorMovimientos.Instance;
                        visor.oID = movimientoEntrada.MovimientoInventarioID;
                        visor.oTipoReporte = 3;
                        visor.Load();

                        var resEtiqueta = Helper.MensajePregunta("Desea imprimir las etiquetas?", GlobalClass.NombreApp);
                        if (resEtiqueta == DialogResult.Yes)
                        {
                            //VisorDeReportes visor = VisorDeReportes.Instance;
                            ReporteadorMovimientos visorEt = ReporteadorMovimientos.Instance;
                            visorEt.oID = movimientoEntrada.MovimientoInventarioID;
                            visorEt.oTipoReporte = 2;
                            visorEt.Load();
                        }

                        #endregion

                        break;

                    case 2:

                        #region [ Entrada Inventario ]

                        //Insertar Movimiento
                        var movimientoEntradaI = new MovimientoInventario()
                        {
                            TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue),
                            TipoPagoID = Helper.ConvertirEntero(this.cboTipoPago.SelectedValue),
                            ProveedorID = proveedorId,
                            SucursalOrigenID = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue),
                            SucursalDestinoID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue),
                            FechaFactura = this.dtpFechaFacturacion.Value,
                            FechaRecepcion = this.dtpFechaRecepcion.Value,
                            FolioFactura = null,

                            AplicaEnMovimientoInventarioID = null,
                            FechaAplicacion = null,

                            Subtotal = null,
                            IVA = null,
                            ImporteTotal = 0,
                            FueLiquidado = false,
                            TipoConceptoOperacionID = Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue),
                            Observacion = this.txtObservaciones.Text,
                            Articulos = null,
                            Unidades = null,
                            Seguro = null,
                            ImporteTotalSinDescuento = null
                        };
                        Guardar.Generico<MovimientoInventario>(movimientoEntradaI);

                        if (movimientoEntradaI.MovimientoInventarioID < 1)
                        {
                            this.Cursor = Cursors.Default;
                            SplashScreen.Close();
                            new EntityNotFoundException("MovimientoInventarioID", "MovimientoInventario");                            
                            return;
                        }

                        //Insertar MovimientoDetalle
                        foreach (DataGridViewRow row in this.dgvDetalleCaptura.Rows)
                        {
                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var cantidad = Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                            var detalleMovimiento = new MovimientoInventarioDetalle()
                            {
                                MovimientoInventarioID = movimientoEntradaI.MovimientoInventarioID,
                                ParteID = parteId,
                                Cantidad = cantidad,
                                PrecioUnitario = 0,
                                Importe = 0,
                                FueDevolucion = false,
                                FechaRegistro = DateTime.Now,
                                Estatus = true
                            };
                            General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimiento, detalleMovimiento);

                            //Actualizar ParteExistencia
                            var oParte = General.GetEntity<Parte>(c => c.ParteID == parteId && c.Estatus);
                            if (!oParte.EsServicio.Valor())
                            {
                                var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId);
                                if (existencia != null)
                                {
                                    var inicial = existencia.Existencia;
                                    existencia.Existencia += cantidad;
                                    Guardar.Generico<ParteExistencia>(existencia);

                                    var historial = new MovimientoInventarioHistorial()
                                    {
                                        MovmientoInventarioID = movimientoEntradaI.MovimientoInventarioID,
                                        ParteID = parteId,
                                        ExistenciaInicial = Helper.ConvertirDecimal(inicial),
                                        ExistenciaFinal = Helper.ConvertirDecimal(existencia.Existencia),
                                        SucursalID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue)
                                    };
                                    Guardar.Generico<MovimientoInventarioHistorial>(historial);
                                }
                            }

                            // Se verifica si es una garantía, para cambiar el estatus correspondiente
                            bool bEntradaGarantia = (Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue) == Cat.MovimientosConceptosDeOperacion.EntradaGarantia);
                            if (bEntradaGarantia)
                            {
                                int iGarantiaID = Helper.ConvertirEntero(row.Tag);
                                var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iGarantiaID && c.Estatus);
                                if (oGarantia != null)
                                {
                                    oGarantia.RespuestaID = Cat.VentasGarantiasRespuestas.ArticuloNuevo;
                                    if (oGarantia.AccionID == Cat.VentasGarantiasAcciones.RevisionDeProveedor)
                                    {
                                        oGarantia.EstatusGenericoID = Cat.EstatusGenericos.Resuelto;
                                    }
                                    else
                                    {
                                        oGarantia.FechaCompletado = DateTime.Now;
                                        oGarantia.EstatusGenericoID = Cat.EstatusGenericos.Completada;
                                    }
                                    Guardar.Generico<VentaGarantia>(oGarantia);
                                }
                            }

                            // Se agrega al Kardex
                            var oPartePrecio = General.GetEntity<PartePrecio>(c => c.ParteID == parteId && c.Estatus);
                            AdmonProc.RegistrarKardex(new ParteKardex()
                            {
                                ParteID = parteId,
                                OperacionID = Cat.OperacionesKardex.EntradaInventario,
                                SucursalID = sucursalId,
                                Folio = movimientoEntradaI.MovimientoInventarioID.ToString(),
                                Fecha = DateTime.Now,
                                RealizoUsuarioID = iAutorizoID,
                                Entidad = "----",
                                Origen = (bEntradaGarantia ? "GARANTÍA" : "----"),
                                Destino = this.cboUbicacionDestino.Text,
                                Cantidad = cantidad,
                                Importe = oPartePrecio.Costo.Valor()
                            });
                        }

                        // Se manda a afectar contabilidad (AfeConta)
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.EntradaInventario, movimientoEntradaI.MovimientoInventarioID
                            , ResU.Respuesta.NombreUsuario, movimientoEntradaI.Observacion, sucursalId);

                        SplashScreen.Close();
                        this.btnFinish.Enabled = true;
                        this.Cursor = Cursors.Default;

                        //Visor de ticket de entrada compra
                        ReporteadorMovimientos visorE = ReporteadorMovimientos.Instance;
                        visorE.oID = movimientoEntradaI.MovimientoInventarioID;
                        visorE.oTipoReporte = 4;
                        visorE.Load();

                        #endregion

                        break;

                    case 3:

                        #region [ Salida Inventario ]

                        //Valida la existencia antes  de dar de baja del inventario
                        bool bError = false;
                        foreach (DataGridViewRow row in this.dgvDetalleCaptura.Rows)
                        {
                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var cantidad = Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                            var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId && p.Estatus);
                            if (existencia.Existencia < cantidad)
                            {
                                row.ErrorText = string.Format("No hay existencia suficiente. Existencia: {0}, Salida: {1}", existencia.Existencia, cantidad);
                                row.DefaultCellStyle.ForeColor = Color.Red;
                                bError = true;
                            }
                        }
                        if (bError)
                        {
                            this.Cursor = Cursors.Default;
                            SplashScreen.Close();
                            UtilLocal.MensajeAdvertencia("Uno o más artículos no cuentan con existencia suficiente para la salida. No se puede continuar.");
                            this.btnFinish.Enabled = true;
                            return;
                        }

                        //Insertar Movimiento
                        var movimientoSalida = new MovimientoInventario()
                        {
                            TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue),
                            TipoPagoID = Helper.ConvertirEntero(this.cboTipoPago.SelectedValue),
                            ProveedorID = proveedorId,
                            SucursalOrigenID = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue),
                            SucursalDestinoID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue),
                            FechaFactura = this.dtpFechaFacturacion.Value,
                            FechaRecepcion = this.dtpFechaRecepcion.Value,
                            FolioFactura = null,

                            AplicaEnMovimientoInventarioID = null,
                            FechaAplicacion = null,

                            Subtotal = null,
                            IVA = null,
                            ImporteTotal = 0,
                            FueLiquidado = false,
                            TipoConceptoOperacionID = Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue),
                            Observacion = this.txtObservaciones.Text,
                            Articulos = null,
                            Unidades = null,
                            Seguro = null,
                            ImporteTotalSinDescuento = null
                        };
                        Guardar.Generico<MovimientoInventario>(movimientoSalida);

                        if (movimientoSalida.MovimientoInventarioID < 1)
                        {
                            this.Cursor = Cursors.Default;
                            SplashScreen.Close();
                            new EntityNotFoundException("MovimientoInventarioID", "MovimientoInventario");                            
                            return;
                        }

                        //Insertar MovimientoDetalle
                        foreach (DataGridViewRow row in this.dgvDetalleCaptura.Rows)
                        {
                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var cantidad = Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                            var detalleMovimiento = new MovimientoInventarioDetalle()
                            {
                                MovimientoInventarioID = movimientoSalida.MovimientoInventarioID,
                                ParteID = parteId,
                                Cantidad = cantidad,
                                PrecioUnitario = 0,
                                Importe = 0,
                                FueDevolucion = false,
                                FechaRegistro = DateTime.Now,
                                Estatus = true
                            };
                            General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimiento, detalleMovimiento);

                            //Actualizar ParteExistencia
                            var oParte = General.GetEntity<Parte>(c => c.ParteID == parteId && c.Estatus);
                            if (!oParte.EsServicio.Valor())
                            {
                                var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId);
                                if (existencia != null)
                                {
                                    var inicial = existencia.Existencia;
                                    existencia.Existencia -= cantidad;
                                    Guardar.Generico<ParteExistencia>(existencia);

                                    var historial = new MovimientoInventarioHistorial()
                                    {
                                        MovmientoInventarioID = movimientoSalida.MovimientoInventarioID,
                                        ParteID = parteId,
                                        ExistenciaInicial = Helper.ConvertirDecimal(inicial),
                                        ExistenciaFinal = Helper.ConvertirDecimal(existencia.Existencia),
                                        SucursalID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue)
                                    };
                                    Guardar.Generico<MovimientoInventarioHistorial>(historial);
                                }
                            }

                            // Se agrega al Kardex
                            var oPartePrecio = General.GetEntity<PartePrecio>(c => c.ParteID == parteId && c.Estatus);
                            AdmonProc.RegistrarKardex(new ParteKardex()
                            {
                                ParteID = parteId,
                                OperacionID = Cat.OperacionesKardex.SalidaInventario,
                                SucursalID = sucursalId,
                                Folio = movimientoSalida.MovimientoInventarioID.ToString(),
                                Fecha = DateTime.Now,
                                RealizoUsuarioID = iAutorizoID,
                                Entidad = "----",
                                Origen = "----",
                                Destino = this.cboUbicacionDestino.Text,
                                Cantidad = cantidad,
                                Importe = oPartePrecio.Costo.Valor()
                            });
                        }

                        // Se manda a afectar contabilidad (AfeConta)
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.SalidaInventario, movimientoSalida.MovimientoInventarioID
                            , ResU.Respuesta.NombreUsuario, movimientoSalida.Observacion, sucursalId);

                        SplashScreen.Close();
                        this.btnFinish.Enabled = true;
                        this.Cursor = Cursors.Default;

                        //Visor de ticket de entrada compra
                        ReporteadorMovimientos visorS = ReporteadorMovimientos.Instance;
                        visorS.oID = movimientoSalida.MovimientoInventarioID;
                        visorS.oTipoReporte = 5;
                        visorS.Load();

                        #endregion

                        break;

                    case 4:

                        #region [ Devolucion ]

                        //Validaciones
                        foreach (DataGridViewRow row in this.dgvDetalleCaptura.Rows)
                        {
                            if (Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue) != Cat.MovimientosConceptosDeOperacion.DevolucionGarantia)
                            {
                                //Valida existencias antes de continuar
                                var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                                var sucursal = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue);
                                var unidades = Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                                var existencias = General.GetEntity<ParteExistencia>(p => p.ParteID.Equals(parteId) && p.SucursalID.Equals(sucursal) && p.Estatus);
                                if (existencias.Existencia < unidades)
                                {
                                    this.Cursor = Cursors.Default;
                                    SplashScreen.Close();
                                    Helper.MensajeError(string.Format("{0} {1} {2}", "El articulo", row.Cells["Numero Parte"].Value, "no cuenta con la existencia suficiente para realizar la operación"), GlobalClass.NombreApp);
                                    return;
                                }
                            }

                            //Valida que la cantidad a devolver sea igual o menor a la del movimiento
                            //if (movimientoInventario.MovimientoInventarioID > 0)
                            //{
                            //    var detalle = General.GetEntity<MovimientoInventarioDetalle>(m => m.MovimientoInventarioID.Equals(movimientoInventario.MovimientoInventarioID)
                            //        && m.ParteID.Equals(parteId) && m.Estatus);
                            //    if (unidades > detalle.Cantidad)
                            //    {
                            //        this.Cursor = Cursors.Default;
                            //        SplashScreen.Close();
                            //        Helper.MensajeError(string.Format("{0} {1} {2}", "El articulo", row.Cells["Numero Parte"].Value, "tiene registrada una cantidad menor a la que intenta devolver."), GlobalClass.NombreApp);                                    
                            //        return;
                            //    }
                            //}

                            //Validar que LA CANTIDAD del movimiento de compra, sea menor o igual que la suma de las devoluciones

                            /*
                            var mov = General.GetEntity<MovimientoInventarioView>(m => m.MovimientoInventarioID == movimientoInventario.MovimientoInventarioID && m.FolioFactura == m.FolioFactura);
                            if (mov != null)
                            {
                                var det = General.GetEntity<MovimientoInventarioDetalleView>(m => m.MovimientoInventarioID == mov.MovimientoInventarioID && m.ParteID == parteId);
                                if (det != null)
                                {
                                    var dev = General.GetEntity<MovimientosInventarioContadorDevolucionesView>(m => m.MovimientoInventarioID == movimientoInventario.MovimientoInventarioID
                                        && m.ParteID == parteId);
                                    if (dev != null)
                                    {
                                        if (det.Cantidad > dev.Cantidad)
                                        {
                                            this.Cursor = Cursors.Default;
                                            SplashScreen.Close();
                                            Helper.MensajeError(string.Format("{0} {1} {2}", "El articulo", row.Cells["Numero Parte"].Value, "tiene registrada una cantidad menor en la Factura a la que intenta devolver."), GlobalClass.NombreApp);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    this.Cursor = Cursors.Default;
                                    SplashScreen.Close();
                                    Helper.MensajeError("Error al tratar de validar la cantidad comprada.", GlobalClass.NombreApp);
                                    return;
                                }
                            }
                            else
                            {
                                this.Cursor = Cursors.Default;
                                SplashScreen.Close();
                                Helper.MensajeError("Error al tratar de validar el movmiento de Compra.", GlobalClass.NombreApp);
                                return;
                            }
                            */

                            int iDetMovCompra = Helper.ConvertirEntero(row.Cells["FuenteMovimientoInventarioDetalleID"].Value);
                            decimal mCantidad = Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                            var oDetMovCompra = General.GetEntity<MovimientoInventarioDetalle>(c => c.MovimientoInventarioDetalleID == iDetMovCompra && c.Estatus);
                            if (oDetMovCompra != null && (oDetMovCompra.Cantidad - oDetMovCompra.CantidadDevuelta) < mCantidad)
                            {
                                this.Cursor = Cursors.Default;
                                SplashScreen.Close();
                                UtilLocal.MensajeAdvertencia(string.Format("El artículo {0} tiene registrada una cantidad menor en la factura a la que intenta devolver."
                                    , row.Cells["Numero Parte"].Value));
                                return;
                            }

                        }

                        // Se obtiene el folio factura del primer artículo a devolver
                        if (this.dgvDetalleCaptura.Rows.Count > 0)
                        {
                            int iMovDetID = Helper.ConvertirEntero(this.dgvDetalleCaptura["FuenteMovimientoInventarioDetalleID", 0].Value);
                            var oMovDet = General.GetEntity<MovimientoInventarioDetalle>(c => c.MovimientoInventarioDetalleID == iMovDetID && c.Estatus);
                            var oMov = General.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == oMovDet.MovimientoInventarioID && c.Estatus);
                            this.txtFolioFactura.Text = oMov.FolioFactura;
                        }

                        //Insertar Movimiento
                        var movimientoDevolucion = new MovimientoInventario()
                        {
                            TipoOperacionID = Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue),
                            TipoPagoID = Helper.ConvertirEntero(this.cboTipoPago.SelectedValue),
                            ProveedorID = proveedorId,
                            SucursalOrigenID = Helper.ConvertirEntero(this.cboUbicacionOrigen.SelectedValue),
                            SucursalDestinoID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue),
                            FechaFactura = this.dtpFechaFacturacion.Value,
                            FechaRecepcion = this.dtpFechaRecepcion.Value,
                            FolioFactura = this.txtFolioFactura.Text,

                            AplicaEnMovimientoInventarioID = null,
                            FechaAplicacion = null,

                            Subtotal = null,
                            IVA = null,
                            ImporteTotal = Helper.ConvertirDecimal(this.lblTotal.Text),
                            FueLiquidado = false,
                            TipoConceptoOperacionID = Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue),
                            Observacion = this.txtObservaciones.Text,
                            Articulos = Helper.ConvertirDecimal(this.txtArticulos.Text),
                            Unidades = Helper.ConvertirDecimal(this.lblUnidades.Text),
                            Seguro = null,
                            ImporteTotalSinDescuento = null,
                        };
                        Guardar.Generico<MovimientoInventario>(movimientoDevolucion);

                        if (movimientoDevolucion.MovimientoInventarioID < 1)
                        {
                            this.Cursor = Cursors.Default;
                            SplashScreen.Close();
                            new EntityNotFoundException("MovimientoInventarioID", "MovimientoInventario");                            
                            return;
                        }

                        //Insertar MovimientoDetalle
                        foreach (DataGridViewRow row in this.dgvDetalleCaptura.Rows)
                        {
                            var parteId = Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                            var cantidad = Helper.ConvertirEntero(row.Cells["UNS"].Value);
                            var detalleMovimiento = new MovimientoInventarioDetalle()
                            {
                                MovimientoInventarioID = movimientoDevolucion.MovimientoInventarioID,
                                ParteID = parteId,
                                Cantidad = cantidad,
                                PrecioUnitario = Helper.ConvertirDecimal(row.Cells["Costo Inicial"].Value),
                                Importe = Helper.ConvertirDecimal(row.Cells["Importe"].Value),
                                FueDevolucion = true,
                                FechaRegistro = DateTime.Now,
                                Estatus = true
                            };
                            General.SaveOrUpdate<MovimientoInventarioDetalle>(detalleMovimiento, detalleMovimiento);

                            //Actualizar ParteExistencia, si no es garantía y no es servicio
                            bool bDevGarantia = (Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue) == Cat.MovimientosConceptosDeOperacion.DevolucionGarantia);
                            var oParte = General.GetEntity<Parte>(c => c.ParteID == parteId && c.Estatus);
                            if (!oParte.EsServicio.Valor() && !bDevGarantia) {
                                var existencia = General.GetEntity<ParteExistencia>(p => p.ParteID == parteId && p.SucursalID == sucursalId);
                                if (existencia != null)
                                {
                                    var inicial = existencia.Existencia;
                                    existencia.Existencia -= cantidad;
                                    Guardar.Generico<ParteExistencia>(existencia);

                                    var historial = new MovimientoInventarioHistorial()
                                    {
                                        MovmientoInventarioID = movimientoDevolucion.MovimientoInventarioID,
                                        ParteID = parteId,
                                        ExistenciaInicial = Helper.ConvertirDecimal(inicial),
                                        ExistenciaFinal = Helper.ConvertirDecimal(existencia.Existencia),
                                        SucursalID = Helper.ConvertirEntero(this.cboUbicacionDestino.SelectedValue)
                                    };
                                    Guardar.Generico<MovimientoInventarioHistorial>(historial);
                                }
                            }

                            // Se modifica la cantidad devuelta, en el detalle de la compra utilizada
                            int iDetMovCompra = Helper.ConvertirEntero(row.Cells["FuenteMovimientoInventarioDetalleID"].Value);
                            var oDetMovCompra = General.GetEntity<MovimientoInventarioDetalle>(c => c.MovimientoInventarioDetalleID == iDetMovCompra && c.Estatus);
                            if (oDetMovCompra != null)
                            {
                                oDetMovCompra.CantidadDevuelta += cantidad;
                                Guardar.Generico<MovimientoInventarioDetalle>(oDetMovCompra);
                            }

                            // Se verifica si es una garantía, para cambiar el estatus correspondiente, y relacionar la garantía con el Mov
                            if (Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue) == Cat.MovimientosConceptosDeOperacion.DevolucionGarantia)
                            {
                                int iGarantiaID = Helper.ConvertirEntero(row.Tag);
                                var oGarantia = General.GetEntity<VentaGarantia>(c => c.VentaGarantiaID == iGarantiaID && c.Estatus);
                                if (oGarantia != null)
                                {
                                    oGarantia.EstatusGenericoID = Cat.EstatusGenericos.EnRevision;
                                    oGarantia.MovimientoInventarioDetalleID = iDetMovCompra;
                                    Guardar.Generico<VentaGarantia>(oGarantia);
                                }
                            }

                            // Se agrega al Kardex
                            AdmonProc.RegistrarKardex(new ParteKardex()
                            {
                                ParteID = parteId,
                                OperacionID = Cat.OperacionesKardex.DevolucionAProveedor,
                                SucursalID = sucursalId,
                                Folio = movimientoDevolucion.MovimientoInventarioID.ToString(),
                                Fecha = DateTime.Now,
                                RealizoUsuarioID = iAutorizoID,
                                Entidad = this.cboProveedor.Text,
                                Origen = this.cboUbicacionDestino.Text,
                                Destino = (bDevGarantia ? "GARANTÍA" : proveedorId.ToString()),
                                Cantidad = (bDevGarantia ? 0 : cantidad),
                                Importe = Helper.ConvertirDecimal(row.Cells["Costo Inicial"].Value)
                            });
                        }

                        SplashScreen.Close();
                        this.btnFinish.Enabled = true;
                        this.Cursor = Cursors.Default;

                        //Visor de ticket de devolucion
                        ReporteadorMovimientos visorD = ReporteadorMovimientos.Instance;
                        visorD.oID = movimientoDevolucion.MovimientoInventarioID;
                        visorD.oTipoReporte = 6;
                        visorD.Load();
                        #endregion

                        break;

                    default:
                        break;
                }
                
                new Notificacion(string.Format("{0} {1}", this.cboTipoOperacion.Text, "Guardado exitosamente"), 2 * 1000).Mostrar(Principal.Instance);
                this.finalizar();
            }
            catch (Exception ex)
            {
                SplashScreen.Close();
                this.btnFinish.Enabled = true;
                this.Cursor = Cursors.Default;
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [Tab Detalle Captura ]

        #region [ Eventos ]

        private void btnHabilitar_Click(object sender, EventArgs e)
        {
            this.habilitarControles();
        }

        private void btnBuscarNoParte_Click(object sender, EventArgs e)
        {
            if (!Validaciones())
                return;

            try
            {
                if (Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) <= 3)
                {
                    if (Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue) == Cat.MovimientosConceptosDeOperacion.EntradaGarantia)
                    {
                        int iProveedorID = Helper.ConvertirEntero(this.cboProveedor.SelectedValue);
                        var oGarantias = General.GetListOf<VentasGarantiasView>(c => c.ProveedorID == iProveedorID && c.EstatusGenericoID == Cat.EstatusGenericos.EnRevision);
                        var frmSel = new ObtenerElementoLista("Selecciona el artículo repuesto:", oGarantias);
                        frmSel.Size = new Size(800, 400);
                        frmSel.MostrarColumnas("NumeroDeParte", "NombreDeParte", "Linea", "Marca");
                        if (frmSel.ShowDialog(Principal.Instance) == DialogResult.OK)
                        {
                            var oGarantia = (frmSel.Seleccion as VentasGarantiasView);
                            int iFila = this.AgregarParteDetalleCaptura(oGarantia.ParteID);
                            // Se agrega el id de la garantía
                            this.dgvDetalleCaptura.Rows[iFila].Tag = oGarantia.VentaGarantiaID;
                        }
                        frmSel.Dispose();
                    }
                    else
                    {
                        var bus = DetalleBusquedaDesdeMovimientos.Instance;
                        bus.BusquedaPredefinida = this.txtNumeroParte.Text;
                        bus.ShowDialog();

                        if (bus.Seleccionado)
                        {
                            this.txtNumeroParte.Clear();
                            var lista = bus.Sel;
                            foreach (var dic in lista)
                            {
                                var parteId = Negocio.Helper.ConvertirEntero(dic["ParteID"]);
                                int iFila = this.AgregarParteDetalleCaptura(parteId);
                                if (iFila >= 0)
                                {
                                    bool bCostoDif = (Helper.ConvertirDecimal(this.dtDetalleConceptos.Rows[iFila]["Costo Inicial"]) != Helper.ConvertirDecimal(dic["Costo"]));
                                    this.dtDetalleConceptos.Rows[iFila]["Costo Inicial"] = Helper.ConvertirDecimal(dic["Costo"]);
                                    this.dtDetalleConceptos.Rows[iFila]["UNS"] = Helper.ConvertirDecimal(dic["Cantidad"]);
                                    this.dgvDetalleCaptura_CellValueChanged(null, new DataGridViewCellEventArgs(this.dgvDetalleCaptura.Columns["UNS"].Index, iFila));
                                }
                            }
                            this.dgvDetalleCaptura.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                        }
                    }
                }
                if (Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) == 4) //Dev a proveedor
                {
                    DetalleBusquedaDeMovimientos bus = new DetalleBusquedaDeMovimientos(Helper.ConvertirEntero(this.cboProveedor.SelectedValue));
                    bus.bGarantias = (Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue) == Cat.MovimientosConceptosDeOperacion.DevolucionGarantia);
                    if (bus.ShowDialog(Principal.Instance) == DialogResult.OK) {
                        this.txtNumeroParte.Clear();

                        /* if (this.txtFolioFactura.Text == string.Empty)
                        {
                            movimientoInventario = bus.movimiento;
                            this.txtFolioFactura.Text = movimientoInventario.FolioFactura;
                        } */

                        // Se agrega
                        int iFila = this.AgregarParteDetalleCaptura(bus.oMovimientoDetalle.ParteID);
                        // Se guarda el MovimientoInventarioDetalleID,
                        this.dgvDetalleCaptura["FuenteMovimientoInventarioDetalleID", iFila].Value = bus.oMovimientoDetalle.MovimientoInventarioDetalleID;
                        // Se agrega el id de la garantía, si aplica
                        if (bus.bGarantias)
                            this.dgvDetalleCaptura.Rows[iFila].Tag = bus.iGarantiaID;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void txtNumeroParte_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Validaciones())
                return;
            if (e.KeyCode == Keys.F2)
                this.btnBuscarNoParte_Click(sender, null);
        }

        private void cboTipoOperacion_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.habilitarControles();
            this.btnHabilitar.Enabled = true;
            this.txtFolioFactura.ReadOnly = false;
            this.chkEsNota.Enabled = false;
            var tipoOperacion = Helper.ConvertirEntero(cboTipoOperacion.SelectedValue);
            switch (tipoOperacion)
            {
                case 1:
                    //Entrada Compra
                    this.cboConceptoOperacion.Enabled = false;
                    this.cboUbicacionOrigen.Enabled = false;
                    this.tabDetalleOperacion.SelectedIndex = 0;
                    this.dtpFechaFacturacion.Value = DateTime.Now.AddDays(-1);
                    this.dtpFechaFacturacion.Enabled = true;
                    this.txtFolioFactura.Text = string.Empty;
                    this.btnBack.Enabled = false;
                    this.btnNext.Enabled = true;
                    this.chkEsNota.Enabled = true;
                    break;

                case 2:
                    //Entrada Inventario
                    this.cboTipoPago.Enabled = false;
                    this.cboUbicacionOrigen.Enabled = false;
                    this.dtpFechaFacturacion.Value = DateTime.Now;
                    this.dtpFechaFacturacion.Enabled = false;
                    this.dtpFechaRecepcion.Enabled = false;
                    this.cboProveedor.Enabled = false;
                    this.txtFolioFactura.Enabled = false;
                    this.txtFolioFactura.Text = string.Empty;
                    this.tabDetalleOperacion.SelectedIndex = 0;
                    this.btnBack.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnFinish.Enabled = true;
                    break;

                case 3:
                    //Salida Inventario
                    this.cboTipoPago.Enabled = false;
                    this.cboUbicacionOrigen.Enabled = false;
                    this.dtpFechaFacturacion.Value = DateTime.Now;
                    this.dtpFechaFacturacion.Enabled = false;
                    this.dtpFechaRecepcion.Enabled = false;
                    this.cboProveedor.Enabled = false;
                    this.txtFolioFactura.Enabled = false;
                    this.txtFolioFactura.Text = string.Empty;
                    this.tabDetalleOperacion.SelectedIndex = 0;
                    this.btnBack.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnFinish.Enabled = true;
                    break;

                case 4:
                    //Devolucion Proveedor
                    this.cboTipoPago.Enabled = false;
                    this.cboUbicacionOrigen.Enabled = false;
                    this.dtpFechaFacturacion.Value = DateTime.Now;
                    this.dtpFechaFacturacion.Enabled = false;
                    this.dtpFechaRecepcion.Enabled = false;
                    this.txtFolioFactura.Enabled = false;
                    this.txtFolioFactura.Text = string.Empty;
                    this.tabDetalleOperacion.SelectedIndex = 0;
                    this.txtNumeroParte.Enabled = false;
                    this.txtNumeroPedido.Enabled = false;
                    this.btnBuscarPedido.Enabled = false;
                    this.btnBack.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnFinish.Enabled = true;
                    this.txtFolioFactura.ReadOnly = true;
                    break;

                case 5:
                    //Traspaso
                    this.cboTipoPago.Enabled = false;
                    this.dtpFechaRecepcion.Enabled = false;
                    this.cboProveedor.Enabled = false;
                    this.txtFolioFactura.Enabled = false;
                    this.txtFolioFactura.Text = string.Empty;
                    this.cboConceptoOperacion.Enabled = false;
                    this.dtpFechaFacturacion.Enabled = false;
                    this.dtpFechaRecepcion.Enabled = false;
                    this.tabDetalleOperacion.SelectedIndex = 0;
                    this.btnBack.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnFinish.Enabled = true;
                    break;

                default:
                    break;
            }

            this.cargarConceptoOperacion(Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue));

            //Borra las filas del grid detalleCaptura
            if (tipoOperacion > 0 && this.dgvDetalleCaptura.Rows.Count > 0)
            {
                var numeroFilas = this.dgvDetalleCaptura.Rows.Count - 1;
                for (int x = numeroFilas; x >= 0; --x)
                {
                    var parteId = this.dgvDetalleCaptura.Rows[x].Cells["ParteID"].Value.ToString();
                    this.dgvDetalleCaptura.Rows.RemoveAt(x);
                    this.eliminarFilaEnGridsSiguientes(parteId);
                }
            }
        }

        private void cboConceptoOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) == Cat.TiposDeOperacionMovimientos.EntradaInventario)
            {
                this.txtNumeroParte.Enabled = true;
                this.txtNumeroPedido.Enabled = true;
                this.btnBuscarPedido.Enabled = true;
                this.cboProveedor.Enabled = false;
            }

            switch (Helper.ConvertirEntero(this.cboConceptoOperacion.SelectedValue))
            {
                case Cat.MovimientosConceptosDeOperacion.EntradaGarantia:
                    this.txtNumeroParte.Enabled = false;
                    this.txtNumeroPedido.Enabled = false;
                    this.btnBuscarPedido.Enabled = false;
                    this.cboProveedor.Enabled = true;
                    break;
            }
        }

        private void cboProveedor_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                var proveedorId = Negocio.Helper.ConvertirEntero(this.cboProveedor.SelectedValue);
                if (proveedorId > 0)
                {
                    proveedor = Negocio.General.GetEntity<Proveedor>(p => p.ProveedorID.Equals(proveedorId));
                    this.txtAitemsUno.Text = proveedor.DescuentoItemUno != null ? proveedor.DescuentoItemUno.ToString() : "0.0";
                    this.txtAitemsDos.Text = proveedor.DescuentoItemDos != null ? proveedor.DescuentoItemDos.ToString() : "0.0";
                    this.txtAitemsTres.Text = proveedor.DescuentoItemTres != null ? proveedor.DescuentoItemTres.ToString() : "0.0";
                    this.txtAitemsCuatro.Text = proveedor.DescuentoItemCuatro != null ? proveedor.DescuentoItemCuatro.ToString() : "0.0";
                    this.txtAitemsCinco.Text = proveedor.DescuentoItemCinco != null ? proveedor.DescuentoItemCinco.ToString() : "0.0";

                    this.txtAfacturaUno.Text = proveedor.DescuentoFacturaUno != null ? proveedor.DescuentoFacturaUno.ToString() : "0.0";
                    this.txtAfacturaDos.Text = proveedor.DescuentoFacturaDos != null ? proveedor.DescuentoFacturaDos.ToString() : "0.0";
                    this.txtAfacturaTres.Text = proveedor.DescuentoFacturaTres != null ? proveedor.DescuentoFacturaTres.ToString() : "0.0";
                    this.txtAfacturaCuatro.Text = proveedor.DescuentoFacturaCuatro != null ? proveedor.DescuentoFacturaCuatro.ToString() : "0.0";
                    this.txtAfacturaCinco.Text = proveedor.DescuentoFacturaCinco != null ? proveedor.DescuentoFacturaCinco.ToString() : "0.0";

                    this.validarProveedorMarcaFilas(proveedor, this.dgvDetalleCaptura.Rows.Count);
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDetalleCaptura_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Se verifica si la parte es AGranel o no, para quitar decimales
                int iParteID = Helper.ConvertirEntero(this.dgvDetalleCaptura["ParteID", e.RowIndex].Value);
                decimal mCantidad = Helper.ConvertirDecimal(this.dgvDetalleCaptura["UNS", e.RowIndex].Value);
                if ((mCantidad - (int)mCantidad) != 0)
                {
                    if (!General.Exists<Parte>(c => c.ParteID == iParteID && c.AGranel && c.Estatus))
                        this.dgvDetalleCaptura["UNS", e.RowIndex].Value = (int)mCantidad;
                }

                SetColumnNameFocusCaptura method = new SetColumnNameFocusCaptura(dgvDetalleCapturaFocus);
                this.dgvDetalleCaptura.BeginInvoke(method, this.dgvDetalleCaptura.Columns[e.ColumnIndex].Name, e.RowIndex);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDetalleCaptura_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (this.dgvDetalleCaptura.Columns[e.ColumnIndex].Name == "Costo Inicial")
                {
                    decimal value;
                    if (!Decimal.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                    else
                    {
                        this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = null;
                    }
                }
                if (this.dgvDetalleCaptura.Columns[e.ColumnIndex].Name == "UNS")
                {
                    decimal value;
                    if (!Decimal.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                    else if (value <= 0)
                    {
                        this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = "Debe ingresar un valor mayor a cero.";
                        e.Cancel = true;
                    }
                    else
                    {
                        this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDetalleCaptura_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (this.dgvDetalleCaptura.Columns[e.ColumnIndex].Name == "Costo Inicial")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDetalleCaptura.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal importe = 0;
                        importe = Negocio.Helper.ConvertirDecimal(this.dgvDetalleCaptura.Rows[e.RowIndex].Cells["Costo Inicial"].Value) * Negocio.Helper.ConvertirDecimal(this.dgvDetalleCaptura.Rows[e.RowIndex].Cells["UNS"].Value);
                        this.dgvDetalleCaptura.Rows[e.RowIndex].Cells["Importe"].Value = importe;
                    }
                }

                if (this.dgvDetalleCaptura.Columns[e.ColumnIndex].Name == "UNS")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDetalleCaptura.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal importe = 0;
                        importe = Negocio.Helper.ConvertirDecimal(this.dgvDetalleCaptura.Rows[e.RowIndex].Cells["Costo Inicial"].Value) * Negocio.Helper.ConvertirDecimal(this.dgvDetalleCaptura.Rows[e.RowIndex].Cells["UNS"].Value);
                        this.dgvDetalleCaptura.Rows[e.RowIndex].Cells["Importe"].Value = importe;
                    }
                }

                //Actualizar valores en grids siguientes
                for (var fila = 0; fila <= this.dgvDetalleCaptura.Rows.Count - 1; fila++)
                {
                    //Limpiar celdas//
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Costo Inicial"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Descuento Marca"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Resultado Marca"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Descuento Items"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Resultado Items"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Descuento Individual"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Resultado Individual"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Unitario"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["UNS"].Value = 0;
                    this.dgvDetalleDescuentos.Rows[fila].Cells["Importe"].Value = 0;
                    this.txtTotalDescuentos.Text = "0.0";

                    //Grid Descuentos                
                    var rowIndex = Negocio.Helper.findRowIndex(this.dgvDetalleDescuentos, "ParteID", this.dgvDetalleCaptura.Rows[fila].Cells["ParteID"].Value.ToString());
                    if (rowIndex >= 0)
                    {
                        decimal imp = 0, uns = 0;
                        imp = Negocio.Helper.ConvertirDecimal(this.dgvDetalleCaptura["Importe", fila].Value);
                        uns = Negocio.Helper.ConvertirDecimal(this.dgvDetalleCaptura["UNS", fila].Value);
                        this.dgvDetalleDescuentos.Rows[rowIndex].Cells["Costo Inicial"].Value = this.dgvDetalleCaptura["Costo Inicial", fila].Value;
                        this.dgvDetalleDescuentos.Rows[rowIndex].Cells["UNS"].Value = uns;
                        this.dgvDetalleDescuentos.Rows[rowIndex].Cells["Importe"].Value = imp;
                        this.dgvDetalleDescuentos.Rows[rowIndex].Cells["Unitario"].Value = imp > 0 ? imp / uns : 0;
                    }

                    //Grid Diferencias
                    this.establecerValoresEnDiferencias(Helper.ConvertirEntero(this.dgvDetalleCaptura.Rows[fila].Cells["ParteID"].Value),
                        Helper.ConvertirDecimal(this.dgvDetalleDescuentos.Rows[rowIndex].Cells["Unitario"].Value),
                        Helper.ConvertirDecimal(this.dgvDetalleDescuentos.Rows[rowIndex].Cells["UNS"].Value));
                }

                //Grid Historial Descuentos                                
                if (this.dgvHistorialDescuentos.Rows.Count > 0)
                {
                    var dt = (DataTable)this.dgvHistorialDescuentos.DataSource;
                    dt.Clear();
                }

                //Establecer Totales de Detalle Captura
                this.establecerTotalPorFilas(this.dgvDetalleCaptura);

                this.dgvDetalleCaptura.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                this.dgvDetalleDescuentos.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                this.dgvDiferencia.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDetalleCaptura_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalleCaptura.SelectedCells.Count < 1)
                return;

            var parteId = this.dgvDetalleCaptura.Rows[this.dgvDetalleCaptura.CurrentCell.RowIndex].Cells["ParteID"].Value.ToString();

            if (e.KeyCode == Keys.Delete)
            {
                var res = Negocio.Helper.MensajePregunta("¿Está seguro de que desea eliminar este Número de Parte?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        this.dgvDetalleCaptura.Rows.RemoveAt(this.dgvDetalleCaptura.CurrentCell.RowIndex);
                        //Eliminar Fila en grids siguientes                        
                        this.eliminarFilaEnGridsSiguientes(parteId);
                    }
                    catch (Exception ex)
                    {
                        Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        private void dgvDetalleCaptura_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvDetalleCaptura.CurrentRow == null)
                return;
            var parteId = Negocio.Helper.ConvertirEntero(this.dgvDetalleCaptura.CurrentRow.Cells["ParteID"].Value);
            this.establecerInfoCaptura(parteId);
        }

        private void dgvDetalleCaptura_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalleCaptura.SelectedCells.Count < 1)
                return;

            var parteId = this.dgvDetalleCaptura.Rows[this.dgvDetalleCaptura.CurrentCell.RowIndex].Cells["ParteID"].Value.ToString();
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.establecerInfoCaptura(Negocio.Helper.ConvertirEntero(parteId));
            }
        }

        private void dgvDetalleCaptura_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            this.establecerTotalPorFilas(this.dgvDetalleCaptura);
        }

        private void dgvDetalleCaptura_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.establecerTotalPorFilas(this.dgvDetalleCaptura);
        }

        private void txtNumeroParte_Enter(object sender, EventArgs e)
        {
            this.txtNumeroParte.SelectAll();
        }

        private void txtNumeroParte_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtNumeroParte.Text))
                    return;

                if (Validaciones())
                {
                    if (Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) <= 3)
                    {
                        var listaParte = General.GetListOf<PartesBusquedaEnMovimientosView>(p => p.NumeroParte.Equals(txtNumeroParte.Text));
                        if (listaParte.Count == 0)
                        {
                            // Helper.MensajeInformacion("No se encontro ningún resultado", GlobalClass.NombreApp);
                            this.btnBuscarNoParte_Click(sender, e);
                            // this.txtNumeroParte.Focus();
                            return;
                        }

                        if (listaParte.Count < 2)
                        {
                            foreach (var parte in listaParte)
                            {
                                // Se valida el estatus
                                if (parte.ParteEstatusID == Cat.PartesEstatus.Inactivo)
                                {
                                    UtilLocal.MensajeAdvertencia("El artículo seleccionado está inactivo. No se puede agregar.");
                                    return;
                                }
                                //
                                if (!this.validarProveedorMarca(proveedor, parte.MarcaParteID) && Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) == 1)
                                {
                                    var msj = string.Format("{0} {1} {2} {3} {4} {5}", "El Número de Parte:", parte.NumeroParte, "No puede ser agregado. La Marca:", parte.Marca, "no tiene relación con el Proveedor:", proveedor.NombreProveedor);
                                    Helper.MensajeError(msj, GlobalClass.NombreApp);
                                    return;
                                }

                                var rowIndex = Helper.findRowIndex(this.dgvDetalleCaptura, "ParteID", parte.ParteID.ToString());
                                if (rowIndex >= 0)
                                {
                                    return;
                                }
                                this.agregarDetalleCaptura(parte);
                                this.agregarDetalleDescuentos(parte);
                                this.agregarDetalleDiferencia(parte);

                                var ri = Helper.findRowIndex(this.dgvDetalleCaptura, "ParteID", parte.ParteID.ToString());
                                if (ri >= 0)
                                {
                                    var ci = this.dgvDetalleCaptura.Columns["Costo Inicial"].Index;
                                    this.dgvDetalleCaptura_CellValueChanged(sender, new DataGridViewCellEventArgs(ci, ri));
                                }
                            }
                            this.txtNumeroParte.Clear();
                        }
                        else
                        {
                            Busqueda bus = Busqueda.Instance;
                            var dt = Negocio.Helper.newTable<PartesBusquedaEnMovimientosView>("Partes", General.GetListOf<PartesBusquedaEnMovimientosView>(
                                p => p.NumeroParte.Equals(txtNumeroParte.Text) && p.ParteEstatusID == Cat.PartesEstatus.Activo));
                            bus.IniciarCarga(dt, string.Format("{0} {1}", "Resultados encontrados con Número de Parte: ", txtNumeroParte.Text));
                            Negocio.Helper.OcultarColumnas(bus.dgvDatos, new string[] { "ParteID", "MarcaParteID", "LineaID", "PartePrecioID", "Costo","PorcentajeUtilidadUno","PorcentajeUtilidadDos",
                                "PorcentajeUtilidadTres","PorcentajeUtilidadCuatro","PorcentajeUtilidadCinco","PrecioUno","PrecioDos","PrecioTres","PrecioCuatro",
                                "PrecioCinco","Busqueda", "EntityState", "EntityKey" });
                            Negocio.Helper.ColumnasToHeaderText(bus.dgvDatos);
                            bus.ShowDialog();

                            if (bus.Seleccionado)
                            {
                                this.txtNumeroParte.Clear();
                                var dic = bus.Sel;
                                foreach (var val in dic)
                                {
                                    if (val.Key.Equals("ParteID"))
                                    {
                                        var parteId = Negocio.Helper.ConvertirEntero(val.Value);
                                        var parte = Negocio.General.GetEntity<PartesBusquedaEnMovimientosView>(p => p.ParteID.Equals(parteId));

                                        if (!this.validarProveedorMarca(proveedor, parte.MarcaParteID) && Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) == 1)
                                        {
                                            var msj = string.Format("{0} {1} {2} {3} {4} {5}", "El Número de Parte:", parte.NumeroParte, "No puede ser agregado. La Marca:", parte.Marca, "no tiene relación con el Proveedor:", proveedor.NombreProveedor);
                                            Helper.MensajeError(msj, GlobalClass.NombreApp);
                                            return;
                                        }

                                        var rowIndex = Negocio.Helper.findRowIndex(this.dgvDetalleCaptura, "ParteID", parte.ParteID.ToString());
                                        if (rowIndex < 0)
                                        {
                                            if (parte != null)
                                            {
                                                this.agregarDetalleCaptura(parte);
                                                this.agregarDetalleDescuentos(parte);
                                                this.agregarDetalleDiferencia(parte);

                                                var ri = Helper.findRowIndex(this.dgvDetalleCaptura, "ParteID", parte.ParteID.ToString());
                                                if (ri >= 0)
                                                {
                                                    var ci = this.dgvDetalleCaptura.Columns["Costo Inicial"].Index;
                                                    this.dgvDetalleCaptura_CellValueChanged(sender, new DataGridViewCellEventArgs(ci, ri));
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Helper.ConvertirEntero(this.cboTipoOperacion.SelectedValue) == 4)
                {
                    this.btnBuscarNoParte_Click(sender, null);
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDetalleCaptura_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDetalleCaptura.VerSeleccionNueva())
            {
                if (this.dgvDetalleCaptura.CurrentRow == null)
                {
                    this.dgvExistencias.LimpiarDatos();
                }
                else
                {
                    int iParteID = Helper.ConvertirEntero(this.dgvDetalleCaptura.CurrentRow.Cells["ParteID"].Value);
                    this.dgvExistencias.CargarDatos(iParteID);
                }
            }
        }
        
        private void dgvDiferencia_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDiferencia.VerSeleccionNueva())
            {
                if (this.dgvDiferencia.CurrentRow == null)
                {
                    this.dgvExistencias.LimpiarDatos();
                }
                else
                {
                    int iParteID = Helper.ConvertirEntero(this.dgvDiferencia.CurrentRow.Cells["ParteID"].Value);
                    this.dgvExistencias.CargarDatos(iParteID);
                }
            }
        }

        #endregion

        #region[ Metodos ]

        private void agregarDetalleCaptura(PartesBusquedaEnMovimientosView parte)
        {
            DataRow row;
            try
            {
                if (parte != null)
                {
                    //var costo = General.GetEntity<PartePrecio>(p => p.ParteID == parte.ParteID);
                    row = dtDetalleConceptos.NewRow();
                    row["ParteID"] = parte.ParteID;
                    row["MarcaParteID"] = parte.MarcaParteID;
                    row["Numero Parte"] = parte.NumeroParte;
                    row["Descripcion"] = parte.NombreParte;
                    row["Costo Inicial"] = parte.Costo;
                    row["UNS"] = 1;
                    row["Importe"] = parte.Costo;

                    dtDetalleConceptos.Rows.Add(row);
                    if (this.dgvDetalleCaptura.Rows.Count > 0)
                    {
                        this.dgvDetalleCaptura.ClearSelection();
                        this.dgvDetalleCaptura.CurrentCell = this.dgvDetalleCaptura["Costo Inicial", this.dgvDetalleCaptura.Rows.Count - 1];
                        this.dgvDetalleCaptura.BeginEdit(true);
                    }
                }
            }
            catch
            {

            }
        }

        public void cargaInicial()
        {
            // Se validan los permisos
            //if (this.EsNuevo)
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Agregar", true))
            //    {
            //        this.Close();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (!LogIn.VerPermiso("Administracion.Catalagos.Modificar", false))
            //        this.btnGuardar.Enabled = false;
            //}

            proveedor = null;

            try
            {
                this.tabDetalleOperacion.SelectedIndex = 0;
                this.limpiarFormulario();

                this.cboTipoOperacion.DataSource = Negocio.General.GetListOf<TipoOperacionesView>(p => p.TipoOperacionID > 0 && !p.NombreTipoOperacion.Equals("TRASPASO")
                    && p.TipoOperacionID != Cat.TiposDeOperacionMovimientos.AjusteKardex);
                this.cboTipoOperacion.DisplayMember = "NombreTipoOperacion";
                this.cboTipoOperacion.ValueMember = "TipoOperacionID";

                this.cboTipoPago.DataSource = Negocio.General.GetListOf<TipoPago>(t => t.Estatus.Equals(true));
                this.cboTipoPago.DisplayMember = "NombreTipoPago";
                this.cboTipoPago.ValueMember = "TipoPagoID";

                this.cboUbicacionOrigen.DataSource = Negocio.General.GetListOf<Sucursal>(s => s.Estatus.Equals(true));
                this.cboUbicacionOrigen.DisplayMember = "NombreSucursal";
                this.cboUbicacionOrigen.ValueMember = "SucursalID";

                this.cboUbicacionDestino.DataSource = Negocio.General.GetListOf<Sucursal>(s => s.Estatus.Equals(true));
                this.cboUbicacionDestino.DisplayMember = "NombreSucursal";
                this.cboUbicacionDestino.ValueMember = "SucursalID";

                this.cboConceptoOperacion.DataSource = Negocio.General.GetListOf<TipoConceptoOperacion>(t => t.Estatus.Equals(true));
                this.cboConceptoOperacion.DisplayMember = "NombreConceptoOperacion";
                this.cboConceptoOperacion.ValueMember = "TipoConceptoOperacionID";

                var listaProveedor = Negocio.General.GetListOf<Proveedor>(p => p.Estatus.Equals(true));
                this.cboProveedor.DataSource = listaProveedor;
                this.cboProveedor.DisplayMember = "NombreProveedor";
                this.cboProveedor.ValueMember = "ProveedorID";
                this.cboProveedor.SelectedIndex = -1;
                AutoCompleteStringCollection autProveedor = new AutoCompleteStringCollection();
                foreach (var prov in listaProveedor) autProveedor.Add(prov.NombreProveedor);
                this.cboProveedor.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboProveedor.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cboProveedor.AutoCompleteCustomSource = autProveedor;
                this.cboProveedor.TextUpdate += new EventHandler(Negocio.Helper.cboCharacterCasingUpper);

                this.dtpFechaFacturacion.Value = DateTime.Now.AddDays(-1);

                //Configuraciones de grids
                this.configurarGridDetalleCaptura();
                this.configurarGridDetalleDescuentos();
                this.configurarGridDiferencia();

                this.habilitarControles();
                this.establecerInfoCaptura(-1);

                this.txtFolioFactura.Focus();

                this.cboTipoOperacion_SelectionChangeCommitted(null, null);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void cargarConceptoOperacion(int tipoOperacionId)
        {
            try
            {
                var conceptos = Negocio.General.GetListOf<TipoConceptoOperacion>(t => t.TipoOperacionID == tipoOperacionId && t.Estatus.Equals(true));
                this.cboConceptoOperacion.DataSource = conceptos;
                this.cboConceptoOperacion.ValueMember = "TipoConceptoOperacionID";
                this.cboConceptoOperacion.DisplayMember = "NombreConceptoOperacion";
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        public void configurarGridDetalleCaptura()
        {
            try
            {
                dtDetalleConceptos.Clear();
                this.dgvDetalleCaptura.Refresh();
                if (this.dgvDetalleCaptura.RowCount > 0)
                {
                    if (this.dgvDetalleCaptura.Rows.Count > 0)
                        this.dgvDetalleCaptura.Rows.Clear();

                    if (this.dgvDetalleCaptura.Columns.Count > 0)
                        this.dgvDetalleCaptura.Columns.Clear();
                }

                this.dgvDetalleCaptura.DataSource = null;

                dtDetalleConceptos = new DataTable();

                var colParteId = new DataColumn();
                colParteId.DataType = System.Type.GetType("System.Int32");
                colParteId.ColumnName = "ParteID";

                var colMarcaParteId = new DataColumn();
                colMarcaParteId.DataType = System.Type.GetType("System.Int32");
                colMarcaParteId.ColumnName = "MarcaParteID";

                var colNumeroParte = new DataColumn();
                colNumeroParte.DataType = Type.GetType("System.String");
                colNumeroParte.ColumnName = "Numero Parte";

                var colNombreParte = new DataColumn();
                colNombreParte.DataType = Type.GetType("System.String");
                colNombreParte.ColumnName = "Descripcion";

                var colPrecioLista = new DataColumn();
                colPrecioLista.DataType = Type.GetType("System.Decimal");
                colPrecioLista.ColumnName = "Costo Inicial";

                var colUnidades = new DataColumn();
                colUnidades.DataType = Type.GetType("System.Decimal");
                colUnidades.ColumnName = "UNS";

                var colImporte = new DataColumn();
                colImporte.DataType = Type.GetType("System.Decimal");
                colImporte.ColumnName = "Importe";

                // Se agrega una columna para saber cuál es el detalle movimiento fuente
                var oColFuente = new DataColumn() { DataType = typeof(int), ColumnName = "FuenteMovimientoInventarioDetalleID" };

                dtDetalleConceptos.Columns.AddRange(new DataColumn[] { colParteId, colMarcaParteId, colNumeroParte, colNombreParte, colPrecioLista, colUnidades, colImporte
                    , oColFuente });

                this.dgvDetalleCaptura.DataSource = dtDetalleConceptos;
                Negocio.Helper.OcultarColumnas(this.dgvDetalleCaptura, new string[] { "ParteID", "MarcaParteID", "FuenteMovimientoInventarioDetalleID" });
                foreach (DataGridViewColumn column in this.dgvDetalleCaptura.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (column.Name != "Costo Inicial" && column.Name != "UNS")
                        column.ReadOnly = true;

                    if (column.Name == "Costo Inicial" || column.Name == "UNS" || column.Name == "Importe")
                    {
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                    }
                }

                this.dgvDetalleCaptura.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDetalleCaptura.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {

            }
        }

        private void dgvDetalleCapturaFocus(string columnName, int rowIndex)
        {
            try
            {
                if (columnName == "Costo Inicial")
                {
                    this.dgvDetalleCaptura.ClearSelection();
                    this.dgvDetalleCaptura.CurrentCell = this.dgvDetalleCaptura["UNS", rowIndex];
                    this.dgvDetalleCaptura.BeginEdit(true);
                }
                if (columnName == "UNS")
                {
                    this.dgvDetalleCaptura.ClearSelection();
                    this.txtNumeroParte.Focus();
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void eliminarFilaEnGridsSiguientes(string parteId)
        {
            var rowIndex = Negocio.Helper.findRowIndex(this.dgvDetalleDescuentos, "ParteID", parteId);
            if (rowIndex >= 0)
                this.dgvDetalleDescuentos.Rows.RemoveAt(rowIndex);

            var rowIndexh = Negocio.Helper.findRowIndex(this.dgvHistorialDescuentos, "ParteID", parteId);
            if (rowIndexh >= 0)
                this.dgvHistorialDescuentos.Rows.RemoveAt(rowIndexh);

            var rowIndexDif = Negocio.Helper.findRowIndex(this.dgvDiferencia, "ParteID", parteId);
            if (rowIndexDif >= 0)
                this.dgvDiferencia.Rows.RemoveAt(rowIndexDif);

            //Establecer totales
            this.establecerTotalPorFilas(this.dgvDetalleCaptura);

            //Borrar datos Info
            this.establecerInfoCaptura(-1);
        }

        private void habilitarControles()
        {
            this.cboTipoOperacion.Enabled = true;
            this.cboTipoPago.Enabled = true;
            this.cboUbicacionOrigen.Enabled = true;
            this.cboUbicacionDestino.Enabled = true;
            this.cboConceptoOperacion.Enabled = true;
            this.dtpFechaFacturacion.Enabled = true;
            this.dtpFechaRecepcion.Enabled = true;
            this.txtFolioFactura.Enabled = true;
            this.cboProveedor.Enabled = true;
            this.btnHabilitar.Enabled = false;
            this.txtNumeroParte.Enabled = true;
            this.txtNumeroPedido.Enabled = true;
            this.btnBuscarPedido.Enabled = true;
        }

        private void limpiarFormulario()
        {
            this.txtUnidades.Clear();
            this.txtArticulos.Clear();
            this.lblTotal.Text = "0.0";
            this.lblImporteFactura.Text = "0.0";
            this.chkImporteFactura.Checked = false;
            this.lblTotalSinDescuento.Text = "0.0";
            this.chkEsNota.Checked = false;

            //Tab Captura
            this.txtFolioPoliza.Clear();
            this.txtFolioFactura.Clear();
            this.txtNumeroParte.Clear();
            this.txtNumeroPedido.Clear();
            this.txtArticulos.Clear();
            this.txtUnidades.Clear();
            this.txtObservaciones.Clear();

            //Tab Descuentos
            this.txtAitemsUno.Text = "0.0";
            this.txtAitemsDos.Text = "0.0";
            this.txtAitemsTres.Text = "0.0";
            this.txtAitemsCuatro.Text = "0.0";
            this.txtAitemsCinco.Text = "0.0";

            this.txtAfacturaUno.Text = "0.0";
            this.txtAfacturaDos.Text = "0.0";
            this.txtAfacturaTres.Text = "0.0";
            this.txtAfacturaCuatro.Text = "0.0";
            this.txtAfacturaCinco.Text = "0.0";

            this.txtIndividualUno.Text = "0.0";
            this.txtIndividualDos.Text = "0.0";
            this.txtIndividualTres.Text = "0.0";
            this.txtIndividualCuatro.Text = "0.0";
            this.txtIndividualCinco.Text = "0.0";

            //this.lblMarcaArticulos.Enabled = false;
            //this.btnAplicarMarcaArticulos.Enabled = false;

            //this.lblMarcaFactura.Enabled = false;
            //this.btnAplicarMarcaFactura.Enabled = false;

            this.txtAfacturaUno.Text = "0.0";
            this.txtAfacturaDos.Text = "0.0";
            this.txtAfacturaTres.Text = "0.0";
            this.txtAfacturaCuatro.Text = "0.0";
            this.txtAfacturaCinco.Text = "0.0";
            this.txtMarcaSelId.Clear();
            this.txtDescuentoMarcaArticuloUno.Text = "0.0";
            this.txtDescuentoMarcaArticuloDos.Text = "0.0";
            this.txtDescuentoMarcaArticuloTres.Text = "0.0";
            this.txtDescuentoMarcaArticuloCuatro.Text = "0.0";
            this.txtDescuentoMarcaArticuloCinco.Text = "0.0";
            this.txtDescuentoMarcaFacturaUno.Text = "0.0";
            this.txtDescuentoMarcaFacturaDos.Text = "0.0";
            this.txtDescuentoMarcaFacturaTres.Text = "0.0";
            this.txtDescuentoMarcaFacturaCuatro.Text = "0.0";
            this.txtDescuentoMarcaFacturaCinco.Text = "0.0";
            this.txtTotalDescuentos.Text = "0.0";

            //Tab Diferencia
            this.lblDiferencia.Text = "0.0";
        }

        private bool validarProveedorMarca(Proveedor proveedor, int marcaParteID)
        {
            if (proveedor == null)
                return true;

            var existe = false;
            try
            {
                var listaMarcas = General.GetListOf<ProveedorMarcaParte>(p => p.ProveedorID.Equals(proveedor.ProveedorID) && p.Estatus.Equals(true));
                if (listaMarcas != null)
                {
                    foreach (var marca in listaMarcas)
                        if (marcaParteID == marca.MarcaParteID)
                            existe = true;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            return existe;
        }

        private void validarProveedorMarcaFilas(Proveedor proveedor, int numeroFilas)
        {
            if (proveedor == null)
                return;
            if (numeroFilas == 0)
                return;
            numeroFilas = numeroFilas - 1;
            for (int x = numeroFilas; x >= 0; --x)
            {
                if (finalizo == false)
                {
                    if (!this.validarProveedorMarca(proveedor, Helper.ConvertirEntero(this.dgvDetalleCaptura.Rows[x].Cells["MarcaParteID"].Value)))
                    {
                        var msj = string.Format("{0} {1} {2} {3}", "El Número de Parte:", this.dgvDetalleCaptura.Rows[x].Cells["Numero Parte"].Value, "No puede ser agregado. La Marca no tiene relación con el Proveedor:", proveedor.NombreProveedor);
                        Helper.MensajeError(msj, GlobalClass.NombreApp);
                        var parteId = this.dgvDetalleCaptura.Rows[x].Cells["ParteID"].Value.ToString();
                        this.dgvDetalleCaptura.Rows.RemoveAt(x);
                        this.eliminarFilaEnGridsSiguientes(parteId);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region [Tab Detalle Descuentos ]

        private void agregarDetalleDescuentos(PartesBusquedaEnMovimientosView parte)
        {
            DataRow row;
            try
            {
                if (parte != null)
                {
                    row = dtDetalleDescuentos.NewRow();
                    row["ParteID"] = parte.ParteID;
                    row["MarcaParteID"] = parte.MarcaParteID;
                    row["Numero Parte"] = parte.NumeroParte;
                    row["Descripcion"] = parte.NombreParte;
                    row["Costo Inicial"] = 0;           //Valor Precio de Lista del grid inicial                    
                    row["Descuento Marca"] = 0;
                    row["Resultado Marca"] = 0;
                    row["Descuento Items"] = 0;
                    row["Resultado Items"] = 0;
                    row["Descuento Individual"] = 0;
                    row["Resultado Individual"] = 0;
                    row["Unitario"] = 0;                //=(Importe / UNS)
                    row["UNS"] = 0;                     //Valor UNS del grid incial
                    row["Importe"] = 0;                 //Valor Importe del grid inicial
                    dtDetalleDescuentos.Rows.Add(row);
                }
            }
            catch
            {

            }
        }

        public void configurarGridDetalleDescuentos()
        {
            try
            {
                dtDetalleDescuentos.Clear();
                this.dgvDetalleDescuentos.Refresh();
                if (this.dgvDetalleDescuentos.RowCount > 0)
                {
                    if (this.dgvDetalleDescuentos.Columns.Count > 0)
                        this.dgvDetalleDescuentos.Columns.Clear();

                    if (this.dgvDetalleDescuentos.Rows.Count > 0)
                        this.dgvDetalleDescuentos.Rows.Clear();
                }

                this.dgvDetalleDescuentos.DataSource = null;

                dtDetalleDescuentos = new DataTable();

                var colParteId = new DataColumn();
                colParteId.DataType = System.Type.GetType("System.Int32");
                colParteId.ColumnName = "ParteID";

                var colMarcaParteId = new DataColumn();
                colMarcaParteId.DataType = System.Type.GetType("System.Int32");
                colMarcaParteId.ColumnName = "MarcaParteID";

                var colNumeroParte = new DataColumn();
                colNumeroParte.DataType = Type.GetType("System.String");
                colNumeroParte.ColumnName = "Numero Parte";

                var colDescripcion = new DataColumn();
                colDescripcion.DataType = Type.GetType("System.String");
                colDescripcion.ColumnName = "Descripcion";

                var colCostoInicial = new DataColumn();
                colCostoInicial.DataType = Type.GetType("System.Decimal");
                colCostoInicial.ColumnName = "Costo Inicial";

                var colDescuentoMarca = new DataColumn();
                colDescuentoMarca.DataType = Type.GetType("System.Decimal");
                colDescuentoMarca.ColumnName = "Descuento Marca";

                var colResultadoMarca = new DataColumn();
                colResultadoMarca.DataType = Type.GetType("System.Decimal");
                colResultadoMarca.ColumnName = "Resultado Marca";

                var colDescuentoItems = new DataColumn();
                colDescuentoItems.DataType = Type.GetType("System.Decimal");
                colDescuentoItems.ColumnName = "Descuento Items";

                var colResultadoItems = new DataColumn();
                colResultadoItems.DataType = Type.GetType("System.Decimal");
                colResultadoItems.ColumnName = "Resultado Items";

                var colDescuentoIndividual = new DataColumn();
                colDescuentoIndividual.DataType = Type.GetType("System.Decimal");
                colDescuentoIndividual.ColumnName = "Descuento Individual";

                var colResultadoIndividual = new DataColumn();
                colResultadoIndividual.DataType = Type.GetType("System.Decimal");
                colResultadoIndividual.ColumnName = "Resultado Individual";

                var colUnitarios = new DataColumn();
                colUnitarios.DataType = Type.GetType("System.Decimal");
                colUnitarios.ColumnName = "Unitario";

                var colUnidades = new DataColumn();
                colUnidades.DataType = Type.GetType("System.Decimal");
                colUnidades.ColumnName = "UNS";

                var colImporte = new DataColumn();
                colImporte.DataType = Type.GetType("System.Decimal");
                colImporte.ColumnName = "Importe";

                dtDetalleDescuentos.Columns.AddRange(new DataColumn[] { colParteId, colMarcaParteId, colNumeroParte, colDescripcion, colCostoInicial, 
                    colDescuentoMarca, colResultadoMarca, colDescuentoItems, colResultadoItems, colDescuentoIndividual, colResultadoIndividual, 
                    colUnitarios, colUnidades, colImporte });

                this.dgvDetalleDescuentos.DataSource = dtDetalleDescuentos;
                Negocio.Helper.OcultarColumnas(this.dgvDetalleDescuentos, new string[] { "ParteID", "MarcaParteID" });

                if (!this.dgvDetalleDescuentos.Columns.Contains("X"))
                {
                    DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                    checkColumn.Name = "X";
                    checkColumn.HeaderText = "";
                    checkColumn.Width = 50;
                    checkColumn.ReadOnly = false;
                    checkColumn.FillWeight = 10;
                    this.dgvDetalleDescuentos.Columns.Add(checkColumn);
                    this.dgvDetalleDescuentos.Columns["X"].DisplayIndex = 0;
                }

                foreach (DataGridViewColumn column in this.dgvDetalleDescuentos.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (!column.Name.Equals("X"))
                    {
                        column.ReadOnly = true;
                    }
                    if (column.Name != "ParteID" && column.Name != "Numero Parte" && column.Name != "Descripcion")
                    {
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                    }
                }
                this.dgvDetalleDescuentos.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDetalleDescuentos.BackgroundColor = Color.FromArgb(188, 199, 216);

                //Grid Historial de descuentos
                dtHistorialDescuentos.Clear();
                this.dgvHistorialDescuentos.Refresh();
                if (this.dgvHistorialDescuentos.RowCount > 0)
                {
                    if (this.dgvHistorialDescuentos.Rows.Count > 0)
                        this.dgvHistorialDescuentos.Rows.Clear();

                    if (this.dgvHistorialDescuentos.Columns.Count > 0)
                        this.dgvHistorialDescuentos.Columns.Clear();
                }

                dtHistorialDescuentos = new DataTable();

                var colParteID = new DataColumn();
                colParteID.DataType = System.Type.GetType("System.Int32");
                colParteID.ColumnName = "ParteID";

                var colTipoDescuento = new DataColumn();

                colTipoDescuento.DataType = Type.GetType("System.Int32");
                colTipoDescuento.ColumnName = "TipoDescuento";

                var colDescuentoUno = new DataColumn();
                colDescuentoUno.DataType = Type.GetType("System.Decimal");
                colDescuentoUno.ColumnName = "DescuentoUno";

                var colDescuentoDos = new DataColumn();
                colDescuentoDos.DataType = Type.GetType("System.Decimal");
                colDescuentoDos.ColumnName = "DescuentoDos";

                var colDescuentoTres = new DataColumn();
                colDescuentoTres.DataType = Type.GetType("System.Decimal");
                colDescuentoTres.ColumnName = "DescuentoTres";

                var colDescuentoCuatro = new DataColumn();
                colDescuentoCuatro.DataType = Type.GetType("System.Decimal");
                colDescuentoCuatro.ColumnName = "DescuentoCuatro";

                var colDescuentoCinco = new DataColumn();
                colDescuentoCinco.DataType = Type.GetType("System.Decimal");
                colDescuentoCinco.ColumnName = "DescuentoCinco";

                dtHistorialDescuentos.Columns.AddRange(new DataColumn[] { colParteID, colTipoDescuento, colDescuentoUno, colDescuentoDos, 
                    colDescuentoTres, colDescuentoCuatro, colDescuentoCinco });

                this.dgvHistorialDescuentos.DataSource = dtHistorialDescuentos;

                this.dgvHistorialDescuentos.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvHistorialDescuentos.BackgroundColor = Color.FromArgb(188, 199, 216);

                //Grid Pronto Pago
                dtProntoPago.Clear();
                this.dgvProntoPago.Refresh();
                if (this.dgvProntoPago.RowCount > 0)
                {
                    if (this.dgvProntoPago.Rows.Count > 0)
                        this.dgvProntoPago.Rows.Clear();

                    if (this.dgvProntoPago.Columns.Count > 0)
                        this.dgvProntoPago.Columns.Clear();
                }

                dtProntoPago = new DataTable();

                var colDias = new DataColumn();
                colDias.DataType = Type.GetType("System.Int32");
                colDias.ColumnName = "Dias";

                var colDescuento = new DataColumn();
                colDescuento.DataType = Type.GetType("System.Decimal");
                colDescuento.ColumnName = "%";

                var colImportePronto = new DataColumn();
                colImportePronto.DataType = Type.GetType("System.Decimal");
                colImportePronto.ColumnName = "Importe";

                dtProntoPago.Columns.AddRange(new DataColumn[] { colDias, colDescuento, colImportePronto });

                this.dgvProntoPago.DataSource = dtProntoPago;

                foreach (DataGridViewColumn column in this.dgvProntoPago.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    column.ReadOnly = true;
                    if (column.Name == "Importe")
                    {
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                    }
                }

                this.dgvProntoPago.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvProntoPago.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch (Exception ex)
            {

            }
        }

        private void establecerInfoDescuentos(int parteId)
        {
            try
            {
                if (parteId > 0)
                {
                    var info = Negocio.General.GetEntity<PartesBusquedaEnMovimientosView>(p => p.ParteID.Equals(parteId));
                    if (info != null)
                    {
                        this.lblNombreLineaDescuentos.Text = info.Linea;
                        this.lblNombreMarcaDescuentos.Text = info.Marca;
                    }

                    var existencias = Negocio.General.GetListOf<ExistenciasView>(p => p.ParteID.Equals(parteId));
                    decimal existenciaLocal = 0;
                    decimal existenciaGlobal = 0;
                    if (existencias != null)
                    {

                        foreach (var existencia in existencias)
                        {
                            if (existencia.SucursalID.Equals(GlobalClass.SucursalID))
                            {
                                existenciaLocal += Negocio.Helper.ConvertirDecimal(existencia.Exist);
                            }
                            else
                            {
                                existenciaGlobal += Negocio.Helper.ConvertirDecimal(existencia.Exist);
                            }
                        }
                    }
                    this.lblExistenciasDescuentos.Text = string.Format("{0} / {1}", existenciaLocal, existenciaGlobal);
                }
                else
                {
                    this.lblNombreLineaDescuentos.Text = string.Empty;
                    this.lblNombreMarcaDescuentos.Text = string.Empty;
                    this.lblExistenciasDescuentos.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void establecerInfoDescuentosMarca(int parteId, int marcaParteId)
        {
            if (proveedor == null)
                return;

            if (parteId < 1)
                return;

            if (marcaParteId < 1)
                return;

            this.txtDescuentoMarcaArticuloUno.Text = "0.0";
            this.txtDescuentoMarcaArticuloDos.Text = "0.0";
            this.txtDescuentoMarcaArticuloTres.Text = "0.0";
            this.txtDescuentoMarcaArticuloCuatro.Text = "0.0";
            this.txtDescuentoMarcaArticuloCinco.Text = "0.0";
            this.txtDescuentoMarcaFacturaUno.Text = "0.0";
            this.txtDescuentoMarcaFacturaDos.Text = "0.0";
            this.txtDescuentoMarcaFacturaTres.Text = "0.0";
            this.txtDescuentoMarcaFacturaCuatro.Text = "0.0";
            this.txtDescuentoMarcaFacturaCinco.Text = "0.0";

            try
            {
                this.txtMarcaSelId.Text = marcaParteId.ToString();
                /* Método anterior, se modifica por el de la nueva talba ProveedorParteGanancia
                var listaDescuentos = Negocio.General.GetListOf<ProveedorMarcaParte>(p => p.ProveedorID.Equals(proveedor.ProveedorID) && p.MarcaParteID.Equals(marcaParteId));
                foreach (var desc in listaDescuentos)
                {
                    if (desc.ImpactaArticulo.Equals(true) && desc.ImpactaFactura.Equals(false))
                    {
                        this.txtDescuentoMarcaArticuloUno.Text = desc.DescuentoUno.ToString();
                        this.txtDescuentoMarcaArticuloDos.Text = desc.DescuentoDos.ToString();
                        this.txtDescuentoMarcaArticuloTres.Text = desc.DescuentoTres.ToString();
                        this.txtDescuentoMarcaArticuloCuatro.Text = desc.DescuentoCuatro.ToString();
                        this.txtDescuentoMarcaArticuloCinco.Text = desc.DescuentoCinco.ToString();
                    }
                    if (desc.ImpactaFactura.Equals(true) && desc.ImpactaArticulo.Equals(false))
                    {
                        this.txtDescuentoMarcaFacturaUno.Text = desc.DescuentoUno.ToString();
                        this.txtDescuentoMarcaFacturaDos.Text = desc.DescuentoDos.ToString();
                        this.txtDescuentoMarcaFacturaTres.Text = desc.DescuentoTres.ToString();
                        this.txtDescuentoMarcaFacturaCuatro.Text = desc.DescuentoCuatro.ToString();
                        this.txtDescuentoMarcaFacturaCinco.Text = desc.DescuentoCinco.ToString();
                    }
                }
                */

                var oParteGan = AdmonProc.ObtenerParteDescuentoGanancia(this.proveedor.ProveedorID, marcaParteId, null, parteId, true);
                if (oParteGan != null)
                {                    
                    this.txtDescuentoMarcaArticuloUno.Text = oParteGan.DescuentoArticulo1.ToString();
                    this.txtDescuentoMarcaArticuloDos.Text = oParteGan.DescuentoArticulo1.ToString();
                    this.txtDescuentoMarcaArticuloTres.Text = oParteGan.DescuentoArticulo1.ToString();
                    this.txtDescuentoMarcaFacturaUno.Text = oParteGan.DescuentoFactura1.ToString();
                    this.txtDescuentoMarcaFacturaDos.Text = oParteGan.DescuentoFactura1.ToString();
                    this.txtDescuentoMarcaFacturaTres.Text = oParteGan.DescuentoFactura1.ToString();
                }

            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void establecerValoresEnDiferencias(int parteId, decimal unitario, decimal uns)
        {
            try
            {
                var rowIndex = Negocio.Helper.findRowIndex(this.dgvDiferencia, "ParteID", parteId.ToString());
                if (rowIndex >= 0)
                {
                    this.dgvDiferencia.Rows[rowIndex].Cells["Costo Nuevo"].Value = unitario;
                    this.dgvDiferencia.Rows[rowIndex].Cells["Precio 1"].Value = 
                        Helper.AplicarRedondeo(unitario * Helper.ConvertirDecimal(this.dgvDiferencia.Rows[rowIndex].Cells["%1"].Value));
                    this.dgvDiferencia.Rows[rowIndex].Cells["Precio 2"].Value = 
                        Helper.AplicarRedondeo(unitario * Helper.ConvertirDecimal(this.dgvDiferencia.Rows[rowIndex].Cells["%2"].Value));
                    this.dgvDiferencia.Rows[rowIndex].Cells["Precio 3"].Value = 
                        Helper.AplicarRedondeo(unitario * Helper.ConvertirDecimal(this.dgvDiferencia.Rows[rowIndex].Cells["%3"].Value));
                    this.dgvDiferencia.Rows[rowIndex].Cells["Precio 4"].Value = 
                        Helper.AplicarRedondeo(unitario * Helper.ConvertirDecimal(this.dgvDiferencia.Rows[rowIndex].Cells["%4"].Value));
                    this.dgvDiferencia.Rows[rowIndex].Cells["Precio 5"].Value = 
                        Helper.AplicarRedondeo(unitario * Helper.ConvertirDecimal(this.dgvDiferencia.Rows[rowIndex].Cells["%5"].Value));

                    // Columna Cantidad
                    this.dgvDiferencia["Cantidad", rowIndex].Value = uns;

                    //Columna Etiqueta
                    var parte = General.GetEntity<PartesBusquedaEnMovimientosView>(p => p.ParteID.Equals(parteId));
                    if (parte != null)
                    {
                        if (parte.Etiqueta != null)
                            if (Helper.ConvertirBool(parte.Etiqueta) == true)
                                if (parte.SoloUnaEtiqueta != null)
                                    if (Helper.ConvertirBool(parte.SoloUnaEtiqueta) == true)
                                        this.dgvDiferencia.Rows[rowIndex].Cells["Etiqueta"].Value = 1;
                                    else
                                        this.dgvDiferencia.Rows[rowIndex].Cells["Etiqueta"].Value = uns;
                                else
                                    this.dgvDiferencia.Rows[rowIndex].Cells["Etiqueta"].Value = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private decimal calcularDescuento(decimal unitario, decimal desUno, decimal desDos, decimal desTres, decimal desCuatro, decimal desCinco)
        {
            decimal oper = 0;
            decimal resultado = 0;
            if (desUno > 0)
            {
                oper = unitario * desUno;
                resultado = unitario - oper;
                if (desDos > 0)
                {
                    oper = resultado * desDos;
                    resultado = resultado - oper;
                    if (desTres > 0)
                    {
                        oper = resultado * desTres;
                        resultado = resultado - oper;
                        if (desCuatro > 0)
                        {
                            oper = resultado * desCuatro;
                            resultado = resultado - oper;
                            if (desCinco > 0)
                            {
                                oper = resultado * desCinco;
                                resultado = resultado - oper;
                            }
                        }
                    }
                }
            }
            return resultado;
        }

        private decimal calcularDescuento(decimal unitario, List<decimal> listaDescuentos)
        {
            decimal diferencia = 0;
            decimal resultado = unitario;

            foreach (var desc in listaDescuentos)
            {
                diferencia = resultado * desc;
                resultado = resultado - diferencia;
            }

            return resultado;
        }

        private void establecerHistoriaDescuentos(decimal resultado, int tipoDescuento, int parteId, decimal desUno, decimal desDos, decimal desTres, decimal desCuatro, decimal desCinco)
        {
            if (resultado > 0) //agregar
            {
                var his = new HistorialDescuentos()
                {
                    ParteID = parteId,
                    TipoDescuento = tipoDescuento,
                    DescuentoUno = desUno,
                    DescuentoDos = desDos,
                    DescuentoTres = desTres,
                    DescuentoCuatro = desCuatro,
                    DescuentoCinco = desCinco
                };
                this.agregarHistorialDescuentos(his);
            }
            else //buscar y eliminar
            {
                //var rowIndex = Negocio.Helper.findRowIndex(this.dgvHistorialDescuentos, "ParteID", parteId.ToString());
                int rowIndex = -1;
                foreach (DataGridViewRow row in this.dgvHistorialDescuentos.Rows)
                {
                    if (row.Cells["ParteID"].Value.ToString().Equals(parteId.ToString())
                        && row.Cells["TipoDescuento"].Value.ToString().Equals(tipoDescuento.ToString()))
                    {
                        rowIndex = row.Index;
                    }
                }
                if (rowIndex >= 0)
                {
                    this.dgvHistorialDescuentos.Rows.RemoveAt(rowIndex);
                }
            }
        }

        private void agregarHistorialDescuentos(HistorialDescuentos historial)
        {
            try
            {
                int rowIndex = -1;
                foreach (DataGridViewRow row in this.dgvHistorialDescuentos.Rows)
                {
                    if (row.Cells["ParteID"].Value.ToString().Equals(historial.ParteID.ToString())
                        && row.Cells["TipoDescuento"].Value.ToString().Equals(historial.TipoDescuento.ToString()))
                    {
                        rowIndex = row.Index;
                    }
                }

                if (rowIndex >= 0)
                {
                    //existe:. actualizar
                    this.dgvHistorialDescuentos.Rows[rowIndex].Cells["DescuentoUno"].Value = historial.DescuentoUno;
                    this.dgvHistorialDescuentos.Rows[rowIndex].Cells["DescuentoDos"].Value = historial.DescuentoDos;
                    this.dgvHistorialDescuentos.Rows[rowIndex].Cells["DescuentoTres"].Value = historial.DescuentoTres;
                    this.dgvHistorialDescuentos.Rows[rowIndex].Cells["DescuentoCuatro"].Value = historial.DescuentoCuatro;
                    this.dgvHistorialDescuentos.Rows[rowIndex].Cells["DescuentoCinco"].Value = historial.DescuentoCinco;
                }
                else
                {
                    //es nuevo
                    DataRow row;
                    if (historial != null)
                    {
                        row = dtHistorialDescuentos.NewRow();
                        row["ParteID"] = historial.ParteID;
                        row["TipoDescuento"] = historial.TipoDescuento;
                        row["DescuentoUno"] = historial.DescuentoUno;
                        row["DescuentoDos"] = historial.DescuentoDos;
                        row["DescuentoTres"] = historial.DescuentoTres;
                        row["DescuentoCuatro"] = historial.DescuentoCuatro;
                        row["DescuentoCinco"] = historial.DescuentoCinco;
                        dtHistorialDescuentos.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private List<decimal> listaDescuentosAplicados(int parteId, int tipoDescuento)
        {
            var lst = new List<decimal>();
            foreach (DataGridViewRow row in this.dgvHistorialDescuentos.Rows)
            {
                if (Negocio.Helper.ConvertirEntero(row.Cells["ParteID"].Value) == parteId && Negocio.Helper.ConvertirEntero(row.Cells["TipoDescuento"].Value) == tipoDescuento)
                {
                    for (var x = 0; x <= this.dgvHistorialDescuentos.Columns.Count - 1; x++)
                    {
                        switch (this.dgvHistorialDescuentos.Columns[x].Name)
                        {
                            case "DescuentoUno":
                                if (Negocio.Helper.ConvertirDecimal(row.Cells[x].Value) > 0)
                                    lst.Add(Negocio.Helper.ConvertirDecimal(row.Cells[x].Value));
                                break;

                            case "DescuentoDos":
                                if (Negocio.Helper.ConvertirDecimal(row.Cells[x].Value) > 0)
                                    lst.Add(Negocio.Helper.ConvertirDecimal(row.Cells[x].Value));
                                break;

                            case "DescuentoTres":
                                if (Negocio.Helper.ConvertirDecimal(row.Cells[x].Value) > 0)
                                    lst.Add(Negocio.Helper.ConvertirDecimal(row.Cells[x].Value));
                                break;

                            case "DescuentoCuatro":
                                if (Negocio.Helper.ConvertirDecimal(row.Cells[x].Value) > 0)
                                    lst.Add(Negocio.Helper.ConvertirDecimal(row.Cells[x].Value));
                                break;

                            case "DescuentoCinco":
                                if (Negocio.Helper.ConvertirDecimal(row.Cells[x].Value) > 0)
                                    lst.Add(Negocio.Helper.ConvertirDecimal(row.Cells[x].Value));
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            return lst;
        }

        private void btnAplicarAitems_Click(object sender, EventArgs e)
        {
            try
            {
                var desUno = Negocio.Helper.ConvertirDecimal(this.txtAitemsUno.Text) / 100;
                var desDos = Negocio.Helper.ConvertirDecimal(this.txtAitemsDos.Text) / 100;
                var desTres = Negocio.Helper.ConvertirDecimal(this.txtAitemsTres.Text) / 100;
                var desCuatro = Negocio.Helper.ConvertirDecimal(this.txtAitemsCuatro.Text) / 100;
                var desCinco = Negocio.Helper.ConvertirDecimal(this.txtAitemsCinco.Text) / 100;

                foreach (DataGridViewRow row in this.dgvDetalleDescuentos.Rows)
                {
                    row.Cells["X"].Value = false; //Limpiar seleccionados

                    var parteId = Negocio.Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                    var costoInicial = Negocio.Helper.ConvertirDecimal(row.Cells["Costo Inicial"].Value);
                    var unitario = costoInicial - (Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Marca"].Value) + Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Individual"].Value));

                    //Actualizar historial de descuentos
                    this.establecerHistoriaDescuentos(desUno, 1, parteId, desUno, desDos, desTres, desCuatro, desCinco);

                    var listaDescItems = this.listaDescuentosAplicados(parteId, 1);
                    if (listaDescItems.Count > 0)
                    {
                        var resultado = this.calcularDescuento(unitario, listaDescItems);
                        row.Cells["Descuento Items"].Value = unitario - resultado;
                        row.Cells["Resultado Items"].Value = resultado;
                        row.Cells["Unitario"].Value = unitario - Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Items"].Value);
                        row.Cells["Importe"].Value = Negocio.Helper.ConvertirDecimal(row.Cells["Unitario"].Value) * Negocio.Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                    }
                    else
                    {
                        row.Cells["Descuento Items"].Value = "0.0";
                        row.Cells["Resultado Items"].Value = "0.0";
                        row.Cells["Unitario"].Value = unitario - Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Items"].Value);
                        row.Cells["Importe"].Value = Negocio.Helper.ConvertirDecimal(row.Cells["Unitario"].Value) * Negocio.Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                    }

                    //Actualizar Diferencias
                    this.establecerValoresEnDiferencias(parteId, Helper.ConvertirDecimal(row.Cells["Unitario"].Value), Helper.ConvertirDecimal(row.Cells["UNS"].Value));
                }

                if (desUno == 0)
                {
                    this.txtAitemsUno.Text = "0.0";
                    this.txtAitemsDos.Text = "0.0";
                    this.txtAitemsTres.Text = "0.0";
                    this.txtAitemsCuatro.Text = "0.0";
                    this.txtAitemsCinco.Text = "0.0";
                }

                this.recorrerHistorialYCalcularTotal();
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnAplicarAfactura_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in this.dgvDetalleDescuentos.Rows)
                    row.Cells["X"].Value = false;

                var desUno = Negocio.Helper.ConvertirDecimal(this.txtAfacturaUno.Text) / 100;
                var desDos = Negocio.Helper.ConvertirDecimal(this.txtAfacturaDos.Text) / 100;
                var desTres = Negocio.Helper.ConvertirDecimal(this.txtAfacturaTres.Text) / 100;
                var desCuatro = Negocio.Helper.ConvertirDecimal(this.txtAfacturaCuatro.Text) / 100;
                var desCinco = Negocio.Helper.ConvertirDecimal(this.txtAfacturaCinco.Text) / 100;

                //Primero: sacar el importe total
                var importe = sacarImporteTotalPorFilas(this.dgvDetalleDescuentos);
                if (importe > 0)
                {
                    var resultado = this.calcularDescuento(importe, desUno, desDos, desTres, desCuatro, desCinco);

                    var totalDescuentos = Negocio.Helper.ConvertirDecimal(this.txtTotalDescuentos.Text);
                    this.txtTotalDescuentos.Text = resultado > 0 ? Negocio.Helper.DecimalToCadenaMoneda(totalDescuentos + (importe - resultado)) : "0.0";

                    //Segundo: Registrar un descuento de tipo Factura
                    this.establecerHistoriaDescuentos(resultado, -1, -1, desUno, desDos, desTres, desCuatro, desCinco);

                    //Tercero: recorrer dgvHistorial y aplicar los descuentos a factura en cascada, junto con el ingresado en el paso 2
                    this.recorrerHistorialYCalcularTotal();
                }

                if (desUno == 0)
                {
                    this.txtAfacturaUno.Text = "0.0";
                    this.txtAfacturaDos.Text = "0.0";
                    this.txtAfacturaTres.Text = "0.0";
                    this.txtAfacturaCuatro.Text = "0.0";
                    this.txtAfacturaCinco.Text = "0.0";
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnAplicarIndividual_Click(object sender, EventArgs e)
        {
            try
            {
                var desUno = Negocio.Helper.ConvertirDecimal(this.txtIndividualUno.Text) / 100;
                var desDos = Negocio.Helper.ConvertirDecimal(this.txtIndividualDos.Text) / 100;
                var desTres = Negocio.Helper.ConvertirDecimal(this.txtIndividualTres.Text) / 100;
                var desCuatro = Negocio.Helper.ConvertirDecimal(this.txtIndividualCuatro.Text) / 100;
                var desCinco = Negocio.Helper.ConvertirDecimal(this.txtIndividualCinco.Text) / 100;
                var selCount = 0;

                foreach (DataGridViewRow row in this.dgvDetalleDescuentos.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["X"].Value).Equals(true))
                    {
                        selCount += 1;

                        var parteId = Negocio.Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                        var costoInicial = Negocio.Helper.ConvertirDecimal(row.Cells["Costo Inicial"].Value);
                        var unitario = costoInicial - (Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Marca"].Value) + Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Items"].Value));

                        //Actualizar historial de descuentos
                        this.establecerHistoriaDescuentos(desUno, 2, parteId, desUno, desDos, desTres, desCuatro, desCinco);

                        var listaDescItems = this.listaDescuentosAplicados(parteId, 2);
                        if (listaDescItems.Count > 0)
                        {
                            var resultado = this.calcularDescuento(unitario, listaDescItems);
                            row.Cells["Descuento Individual"].Value = unitario - resultado;
                            row.Cells["Resultado Individual"].Value = resultado;
                            row.Cells["Unitario"].Value = unitario - Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Individual"].Value);
                            row.Cells["Importe"].Value = Negocio.Helper.ConvertirDecimal(row.Cells["Unitario"].Value) * Negocio.Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                        }
                        else
                        {
                            row.Cells["Descuento Individual"].Value = "0.0";
                            row.Cells["Resultado Individual"].Value = "0.0";
                            row.Cells["Unitario"].Value = unitario - Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Individual"].Value);
                            row.Cells["Importe"].Value = Negocio.Helper.ConvertirDecimal(row.Cells["Unitario"].Value) * Negocio.Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                        }

                        //Actualizar Diferencias
                        this.establecerValoresEnDiferencias(parteId, Helper.ConvertirDecimal(row.Cells["Unitario"].Value), Helper.ConvertirDecimal(row.Cells["UNS"].Value));
                    }
                }

                if (selCount == 0)
                    Negocio.Helper.MensajeAdvertencia("Es necesario seleccionar al menos un Número de Parte.", GlobalClass.NombreApp);
                else
                {
                    foreach (DataGridViewRow row in this.dgvDetalleDescuentos.Rows)
                        row.Cells["X"].Value = false; //Limpiar seleccionados

                    if (desUno == 0)
                    {
                        this.txtIndividualUno.Text = "0.0";
                        this.txtIndividualDos.Text = "0.0";
                        this.txtIndividualTres.Text = "0.0";
                        this.txtIndividualCuatro.Text = "0.0";
                        this.txtIndividualCinco.Text = "0.0";
                    }
                    this.recorrerHistorialYCalcularTotal();
                }

            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnAplicarMarcaFactura_Click(object sender, EventArgs e)
        {
            try
            {
                var listaMarcas = Negocio.General.GetListOf<ProveedorMarcaParte>(p => p.ProveedorID.Equals(proveedor.ProveedorID)
                    && p.ImpactaFactura.Equals(true) && p.Estatus.Equals(true));

                if (listaMarcas == null)
                {
                    Negocio.Helper.MensajeInformacion("El proveedor seleccionado no tiene ninguna Marca registrada con descuentos para Aplicar al Total de la Factura.", GlobalClass.NombreApp);
                    return;
                }

                var totalDescuentos = Negocio.Helper.ConvertirDecimal(this.txtTotalDescuentos.Text);

                foreach (var marca in listaMarcas)
                {
                    var desUno = Negocio.Helper.ConvertirDecimal(marca.DescuentoUno) / 100;
                    var desDos = Negocio.Helper.ConvertirDecimal(marca.DescuentoDos) / 100;
                    var desTres = Negocio.Helper.ConvertirDecimal(marca.DescuentoTres) / 100;
                    var desCuatro = Negocio.Helper.ConvertirDecimal(marca.DescuentoCuatro) / 100;
                    var desCinco = Negocio.Helper.ConvertirDecimal(marca.DescuentoCinco) / 100;

                    foreach (DataGridViewRow row in this.dgvDetalleDescuentos.Rows)
                    {
                        row.Cells["X"].Value = false; //Limpiar seleccionados

                        var parteId = Negocio.Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                        var marcaParteId = Negocio.Helper.ConvertirEntero(row.Cells["MarcaParteID"].Value);
                        if (marcaParteId.Equals(marca.MarcaParteID))
                        {
                            var importe = Negocio.Helper.ConvertirDecimal(row.Cells["Importe"].Value);
                            this.establecerHistoriaDescuentos(importe, -2, parteId, desUno, desDos, desTres, desCuatro, desCinco);
                        }
                    }
                }
                this.recorrerHistorialYCalcularTotal();
                Negocio.Helper.MensajeInformacion("Los descuentos de las Marcas registradas fueron aplicados satisfactoriamente al Total de la Factura.", GlobalClass.NombreApp);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnAplicarMarcaArticulos_Click(object sender, EventArgs e)
        {
            try
            {
                var listaMarcas = Negocio.General.GetListOf<ProveedorMarcaParte>(p => p.ProveedorID.Equals(proveedor.ProveedorID)
                    && p.ImpactaArticulo.Equals(true) && p.Estatus.Equals(true));

                if (listaMarcas == null)
                {
                    Negocio.Helper.MensajeInformacion("El proveedor seleccionado no tiene ninguna Marca registrada con descuentos para Aplicar a los Articulos.", GlobalClass.NombreApp);
                    return;
                }

                foreach (var marca in listaMarcas)
                {
                    var desUno = Negocio.Helper.ConvertirDecimal(marca.DescuentoUno) / 100;
                    var desDos = Negocio.Helper.ConvertirDecimal(marca.DescuentoDos) / 100;
                    var desTres = Negocio.Helper.ConvertirDecimal(marca.DescuentoTres) / 100;
                    var desCuatro = Negocio.Helper.ConvertirDecimal(marca.DescuentoCuatro) / 100;
                    var desCinco = Negocio.Helper.ConvertirDecimal(marca.DescuentoCinco) / 100;

                    foreach (DataGridViewRow row in this.dgvDetalleDescuentos.Rows)
                    {
                        row.Cells["X"].Value = false; //Limpiar seleccionados

                        var parteId = Negocio.Helper.ConvertirEntero(row.Cells["ParteID"].Value);
                        var marcaParteId = Negocio.Helper.ConvertirEntero(row.Cells["MarcaParteID"].Value);
                        if (marcaParteId.Equals(marca.MarcaParteID))
                        {
                            var costoInicial = Negocio.Helper.ConvertirDecimal(row.Cells["Costo Inicial"].Value);
                            var unitario = costoInicial - (Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Items"].Value) + Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Individual"].Value));

                            //Actualizar historial de descuentos
                            this.establecerHistoriaDescuentos(desUno, 3, parteId, desUno, desDos, desTres, desCuatro, desCinco);

                            var listaDescItems = this.listaDescuentosAplicados(parteId, 3);
                            if (listaDescItems.Count > 0)
                            {
                                var resultado = this.calcularDescuento(unitario, listaDescItems);
                                row.Cells["Descuento Marca"].Value = unitario - resultado;
                                row.Cells["Resultado Marca"].Value = resultado;
                                row.Cells["Unitario"].Value = unitario - Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Marca"].Value);
                                row.Cells["Importe"].Value = Negocio.Helper.ConvertirDecimal(row.Cells["Unitario"].Value) * Negocio.Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                            }
                            else
                            {
                                row.Cells["Descuento Marca"].Value = "0.0";
                                row.Cells["Resultado Marca"].Value = "0.0";
                                row.Cells["Unitario"].Value = unitario - Negocio.Helper.ConvertirDecimal(row.Cells["Descuento Marca"].Value);
                                row.Cells["Importe"].Value = Negocio.Helper.ConvertirDecimal(row.Cells["Unitario"].Value) * Negocio.Helper.ConvertirDecimal(row.Cells["UNS"].Value);
                            }

                            //Actualizar Diferencias
                            this.establecerValoresEnDiferencias(parteId, Helper.ConvertirDecimal(row.Cells["Unitario"].Value), Helper.ConvertirDecimal(row.Cells["UNS"].Value));
                        }
                    }
                }

                this.recorrerHistorialYCalcularTotal();
                Negocio.Helper.MensajeInformacion("Los descuentos de las Marcas registradas fueron aplicados satisfactoriamente a todos los Articulos.", GlobalClass.NombreApp);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }


        private void dgvDetalleDescuentos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvDetalleDescuentos.CurrentRow == null)
                return;
            var parteId = Negocio.Helper.ConvertirEntero(this.dgvDetalleDescuentos.CurrentRow.Cells["ParteID"].Value);
            var marcaParteId = Negocio.Helper.ConvertirEntero(this.dgvDetalleDescuentos.CurrentRow.Cells["MarcaParteID"].Value);
            this.establecerInfoDescuentos(parteId);
            this.establecerInfoDescuentosMarca(parteId, marcaParteId);
        }

        private void dgvDetalleDescuentos_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalleDescuentos.CurrentRow == null)
                return;

            var parteId = this.dgvDetalleDescuentos.Rows[this.dgvDetalleDescuentos.CurrentCell.RowIndex].Cells["ParteID"].Value.ToString();
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.establecerInfoDescuentos(Negocio.Helper.ConvertirEntero(parteId));
            }
        }

        private void dgvDetalleDescuentos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalleDescuentos.CurrentRow == null) return;
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                var dgv = (DataGridView)sender;
                var selectedrowindex = dgv.SelectedCells[0].RowIndex;
                var fila = dgv.Rows[selectedrowindex];
                if (Convert.ToBoolean(fila.Cells["X"].Value).Equals(true))
                    fila.Cells["X"].Value = false;
                else
                    fila.Cells["X"].Value = true;
            }
        }

        private void dgvDetalleDescuentos_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvDetalleDescuentos.VerSeleccionNueva())
            {
                if (this.dgvDetalleDescuentos.CurrentRow == null)
                {
                    this.dgvExistencias.LimpiarDatos();
                }
                else
                {
                    int iParteID = Helper.ConvertirEntero(this.dgvDetalleDescuentos.CurrentRow.Cells["ParteID"].Value);
                    this.dgvExistencias.CargarDatos(iParteID);
                }
            }
        }

        private void dgvDetalleDescuentos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewColumn column in this.dgvDetalleDescuentos.Columns)
                if (column.Name != "ParteID" && column.Name != "Numero Parte" && column.Name != "Descripcion" && column.Name != "Costo Inicial"
                    && column.Name != "Unitario" && column.Name != "UNS" && column.Name != "Importe")
                    if (Negocio.Helper.ConvertirDecimal(this.dgvDetalleDescuentos.Rows[e.RowIndex].Cells[column.Name].Value) == 0)
                        this.dgvDetalleDescuentos.Columns[column.Name].DefaultCellStyle.ForeColor = Color.Gray;
                    else
                        this.dgvDetalleDescuentos.Columns[column.Name].DefaultCellStyle.ForeColor = Color.Black;
        }

        #endregion

        #region [ Tab Diferencia de Costos ]

        public void configurarGridDiferencia()
        {
            try
            {
                dtDiferencia.Clear();
                this.dgvDiferencia.Refresh();
                if (this.dgvDiferencia.RowCount > 0)
                {
                    if (this.dgvDiferencia.Rows.Count > 0)
                        this.dgvDiferencia.Rows.Clear();

                    if (this.dgvDiferencia.Columns.Count > 0)
                        this.dgvDiferencia.Columns.Clear();
                }

                dtDiferencia = new DataTable();

                var colParteId = new DataColumn();
                colParteId.DataType = System.Type.GetType("System.Int32");
                colParteId.ColumnName = "ParteID";

                var colMarcaParteId = new DataColumn();
                colMarcaParteId.DataType = System.Type.GetType("System.Int32");
                colMarcaParteId.ColumnName = "MarcaParteID";

                var colNumeroParte = new DataColumn();
                colNumeroParte.DataType = Type.GetType("System.String");
                colNumeroParte.ColumnName = "Numero Parte";

                var colDescripcion = new DataColumn();
                colDescripcion.DataType = Type.GetType("System.String");
                colDescripcion.ColumnName = "Descripcion";

                var colCantidad = new DataColumn();
                colCantidad.DataType = Type.GetType("System.Decimal");
                colCantidad.ColumnName = "Cantidad";

                var colCostoActual = new DataColumn();
                colCostoActual.DataType = Type.GetType("System.Decimal");
                colCostoActual.ColumnName = "Costo Actual";

                var colCostoNuevo = new DataColumn();
                colCostoNuevo.DataType = Type.GetType("System.Decimal");
                colCostoNuevo.ColumnName = "Costo Nuevo";

                var colPorcentajeUno = new DataColumn();
                colPorcentajeUno.DataType = Type.GetType("System.Decimal");
                colPorcentajeUno.ColumnName = "%1";

                var colPorcentajeDos = new DataColumn();
                colPorcentajeDos.DataType = Type.GetType("System.Decimal");
                colPorcentajeDos.ColumnName = "%2";

                var colPorcentajeTres = new DataColumn();
                colPorcentajeTres.DataType = Type.GetType("System.Decimal");
                colPorcentajeTres.ColumnName = "%3";

                var colPorcentajeCuatro = new DataColumn();
                colPorcentajeCuatro.DataType = Type.GetType("System.Decimal");
                colPorcentajeCuatro.ColumnName = "%4";

                var colPorcentajeCinco = new DataColumn();
                colPorcentajeCinco.DataType = Type.GetType("System.Decimal");
                colPorcentajeCinco.ColumnName = "%5";

                var colPrecioUno = new DataColumn();
                colPrecioUno.DataType = Type.GetType("System.Decimal");
                colPrecioUno.ColumnName = "Precio 1";

                var colPrecioDos = new DataColumn();
                colPrecioDos.DataType = Type.GetType("System.Decimal");
                colPrecioDos.ColumnName = "Precio 2";

                var colPrecioTres = new DataColumn();
                colPrecioTres.DataType = Type.GetType("System.Decimal");
                colPrecioTres.ColumnName = "Precio 3";

                var colPrecioCuatro = new DataColumn();
                colPrecioCuatro.DataType = Type.GetType("System.Decimal");
                colPrecioCuatro.ColumnName = "Precio 4";

                var colPrecioCinco = new DataColumn();
                colPrecioCinco.DataType = Type.GetType("System.Decimal");
                colPrecioCinco.ColumnName = "Precio 5";

                var colEtiqueta = new DataColumn();
                colEtiqueta.DataType = Type.GetType("System.Int32");
                colEtiqueta.ColumnName = "Etiqueta";

                dtDiferencia.Columns.AddRange(new DataColumn[] { colParteId, colMarcaParteId, colNumeroParte, colDescripcion, colCantidad, colCostoActual, colCostoNuevo, 
                    colPorcentajeUno, colPorcentajeDos, colPorcentajeTres, colPorcentajeCuatro, colPorcentajeCinco, colPrecioUno, colPrecioDos,
                    colPrecioTres, colPrecioCuatro, colPrecioCinco, colEtiqueta });

                this.dgvDiferencia.DataSource = dtDiferencia;
                Negocio.Helper.OcultarColumnas(this.dgvDiferencia, new string[] { "ParteID", "MarcaParteID" });

                if (!this.dgvDiferencia.Columns.Contains("X"))
                {
                    DataGridViewCheckBoxColumn colAplicarCosto = new DataGridViewCheckBoxColumn();
                    colAplicarCosto.Name = "X";
                    colAplicarCosto.HeaderText = "";
                    colAplicarCosto.Width = 50;
                    colAplicarCosto.ReadOnly = false;
                    colAplicarCosto.FillWeight = 10;
                    this.dgvDiferencia.Columns.Add(colAplicarCosto);
                    this.dgvDiferencia.Columns["X"].DisplayIndex = 4;
                }

                foreach (DataGridViewColumn column in this.dgvDiferencia.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (!column.Name.Equals("X") && !column.Name.Equals("%1") && !column.Name.Equals("%2") && !column.Name.Equals("%3")
                        && !column.Name.Equals("%4") && !column.Name.Equals("%5") && !column.Name.Equals("Precio 1") && !column.Name.Equals("Precio 2")
                        && !column.Name.Equals("Precio 3") && !column.Name.Equals("Precio 4") && !column.Name.Equals("Precio 5") && !column.Name.Equals("Etiqueta"))
                    {
                        column.ReadOnly = true;
                    }
                    if (column.Name != "ParteID" && column.Name != "Numero Parte" && column.Name != "Descripcion" && column.Name != "Etiqueta")
                    {
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        column.DefaultCellStyle.Format = GlobalClass.FormatoDecimal;
                    }
                    if (column.Name == "Etiqueta")
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                this.dgvDiferencia.DefaultCellStyle.ForeColor = Color.Black;
                this.dgvDiferencia.BackgroundColor = Color.FromArgb(188, 199, 216);
            }
            catch
            {

            }
        }

        private void agregarDetalleDiferencia(PartesBusquedaEnMovimientosView parte)
        {
            DataRow row;
            try
            {
                if (parte != null)
                {
                    row = dtDiferencia.NewRow();
                    row["ParteID"] = parte.ParteID;
                    row["MarcaParteID"] = parte.MarcaParteID;
                    row["Numero Parte"] = parte.NumeroParte;
                    row["Descripcion"] = parte.NombreParte;
                    row["Cantidad"] = 1M;
                    row["Costo Actual"] = parte.Costo;

                    //Obtener los valores de % de la tabla 'proveedorGanancia', de lo contrario de la tabla 'PartePrecio'
                    /* Método anterior
                    var ganancias = Negocio.General.GetEntity<ProveedorGanancia>(p => p.ProveedorID.Equals(proveedor.ProveedorID)
                        && p.MarcaParteID.Equals(parte.MarcaParteID) && p.LineaID.Equals(parte.LineaID));
                    if (ganancias != null)
                    {
                        row["%1"] = ganancias.PorcentajeUno;
                        row["%2"] = ganancias.PorcentajeDos;
                        row["%3"] = ganancias.PorcentajeTres;
                        row["%4"] = ganancias.PorcentajeCuatro;
                        row["%5"] = ganancias.PorcentajeCinco;
                    }
                    else
                    {
                        row["%1"] = parte.PorcentajeUtilidadUno;
                        row["%2"] = parte.PorcentajeUtilidadDos;
                        row["%3"] = parte.PorcentajeUtilidadTres;
                        row["%4"] = parte.PorcentajeUtilidadCuatro;
                        row["%5"] = parte.PorcentajeUtilidadCinco;
                    }
                    */
                    var oParteGan = AdmonProc.ObtenerParteDescuentoGanancia(this.proveedor.ProveedorID, parte.MarcaParteID, parte.LineaID, parte.ParteID);
                    if (oParteGan != null)
                    {
                        row["%1"] = oParteGan.PorcentajeDeGanancia1;
                        row["%2"] = oParteGan.PorcentajeDeGanancia2;
                        row["%3"] = oParteGan.PorcentajeDeGanancia3;
                        row["%4"] = oParteGan.PorcentajeDeGanancia4;
                        row["%5"] = oParteGan.PorcentajeDeGanancia5;
                    }
                    // Si es 9500, se pone el precio ya establecido
                    if (General.Exists<Parte>(c => c.ParteID == parte.ParteID && c.Es9500 == true && c.Estatus))
                    {
                        var oReg9500 = General.GetEntity<Cotizaciones9500DetalleAvanzadoView>(c => c.ParteID == parte.ParteID
                            && c.Cotizacion9500EstatusID == Cat.EstatusGenericos.Pendiente);
                        if (oReg9500 != null)
                        {
                            decimal mPorcentaje = Math.Round(oReg9500.PrecioAlCliente / parte.Costo.Valor(), 2);
                            // Se verifica si el porcentaje del 9500 está dentro del rango preestablecido
                            if (mPorcentaje < Helper.ConvertirDecimal(row["%5"]) || mPorcentaje > Helper.ConvertirDecimal(row["%1"]))
                                row.SetColumnError("Precio 1", "El precio del 9500 no está dentro del rango de los precios pre-establecidos.");
                            row["%1"] = mPorcentaje;
                            row["%2"] = mPorcentaje;
                            row["%3"] = mPorcentaje;
                            row["%4"] = mPorcentaje;
                            row["%5"] = mPorcentaje;
                        }
                    }
                    else
                    {
                        
                    }

                    //Columna Etiqueta
                    var rowIndex = Helper.findRowIndex(this.dgvDetalleCaptura, "ParteID", parte.ParteID.ToString());
                    if (parte.Etiqueta != null)
                    {
                        if (Helper.ConvertirBool(parte.Etiqueta) == true)
                            if (parte.SoloUnaEtiqueta != null)
                                if (Helper.ConvertirBool(parte.SoloUnaEtiqueta) == true)
                                    row["Etiqueta"] = 1;
                                else
                                    if (rowIndex >= 0)
                                        row["Etiqueta"] = Helper.ConvertirEntero(this.dgvDetalleCaptura.Rows[rowIndex].Cells["UNS"].Value);
                    }

                    dtDiferencia.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        
        private void establecerInfoDiferencia(int rowIndex)
        {
            try
            {
                var costoActual = Helper.ConvertirDecimal(this.dgvDiferencia.Rows[rowIndex].Cells["Costo Actual"].Value);
                var costoNuevo = Helper.ConvertirDecimal(this.dgvDiferencia.Rows[rowIndex].Cells["Costo Nuevo"].Value);
                var porcentaje = ((costoNuevo - costoActual) / costoActual) * 100;
                this.lblDiferencia.Text = Helper.DecimalToCadenaMoneda(porcentaje);
                if (costoNuevo > costoActual)
                    this.lblDiferencia.ForeColor = Color.Red;
                if (costoNuevo < costoActual)
                    this.lblDiferencia.ForeColor = Color.Green;
                if (costoNuevo == costoActual)
                    this.lblDiferencia.ForeColor = Color.Black;

                var parteId = Helper.ConvertirEntero(this.dgvDiferencia.Rows[rowIndex].Cells["ParteID"].Value);
                var parte = General.GetEntity<PartesBusquedaEnMovimientosView>(p => p.ParteID.Equals(parteId));
                if (parte != null)
                {
                    this.txtPorcentajeActualUno.Text = Helper.DecimalToCadenaMoneda(parte.PorcentajeUtilidadUno);
                    this.txtPorcentajeActualDos.Text = Helper.DecimalToCadenaMoneda(parte.PorcentajeUtilidadDos);
                    this.txtPorcentajeActualTres.Text = Helper.DecimalToCadenaMoneda(parte.PorcentajeUtilidadTres);
                    this.txtPorcentajeActualCuatro.Text = Helper.DecimalToCadenaMoneda(parte.PorcentajeUtilidadCuatro);
                    this.txtPorcentajeActualCinco.Text = Helper.DecimalToCadenaMoneda(parte.PorcentajeUtilidadCinco);

                    this.txtPrecioActualUno.Text = Helper.DecimalToCadenaMoneda(parte.PrecioUno);
                    this.txtPrecioActualDos.Text = Helper.DecimalToCadenaMoneda(parte.PrecioDos);
                    this.txtPrecioActualTres.Text = Helper.DecimalToCadenaMoneda(parte.PrecioTres);
                    this.txtPrecioActualCuatro.Text = Helper.DecimalToCadenaMoneda(parte.PrecioCuatro);
                    this.txtPrecioActualCinco.Text = Helper.DecimalToCadenaMoneda(parte.PrecioCinco);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void GridDiferenciaCambiarValor(DataGridViewCell oCelda, object oValor)
        {
            this.bRecalcularPreciosPorcentajes = false;
            oCelda.Value = oValor;
            this.bRecalcularPreciosPorcentajes = true;
        }

        private void btnSeleccionarDiferencia_Click(object sender, EventArgs e)
        {
            if (sel)
            {
                this.btnSeleccionarDiferencia.Text = "Sel Todos";
                sel = false;
            }
            else
            {
                this.btnSeleccionarDiferencia.Text = "Sel Ninguno";
                sel = true;
            }

            if (this.dgvDiferencia.Columns.Contains("X"))
            {
                foreach (DataGridViewRow row in this.dgvDiferencia.Rows)
                    if (sel)
                        row.Cells["X"].Value = true;
                    else
                        row.Cells["X"].Value = false;
            }
        }

        private void dgvDiferenciaFocus(string columnName, int rowIndex)
        {
            try
            {
                if (columnName == "%1")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["%2", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "%2")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["%3", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "%3")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["%4", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "%4")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["%5", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "%5")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["Precio 1", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "Precio 1")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["Precio 2", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "Precio 2")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["Precio 3", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "Precio 3")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["Precio 4", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "Precio 4")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["Precio 5", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
                if (columnName == "Precio 5")
                {
                    this.dgvDiferencia.ClearSelection();
                    this.dgvDiferencia.CurrentCell = this.dgvDiferencia["Etiqueta", rowIndex];
                    this.dgvDiferencia.BeginEdit(true);
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDiferencia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvDetalleCaptura.CurrentRow == null)
                return;
            var parteId = Negocio.Helper.ConvertirEntero(this.dgvDiferencia.CurrentRow.Cells["ParteID"].Value);
            this.establecerInfoDiferencia(e.RowIndex);
        }

        private void dgvDiferencia_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.dgvDiferencia.SelectedCells.Count < 1)
                return;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.establecerInfoDiferencia(this.dgvDiferencia.CurrentCell.RowIndex);
            }
        }

        private void dgvDiferencia_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (this.dgvDiferencia.Columns.Contains("X"))
            {
                foreach (DataGridViewRow row in this.dgvDiferencia.Rows)
                    if (Convert.ToBoolean(row.Cells["X"].Value).Equals(false))
                        row.Cells["X"].Value = true;
            }
        }

        private void dgvDiferencia_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                SetColumnNameFocusDiferencia method = new SetColumnNameFocusDiferencia(dgvDiferenciaFocus);
                this.dgvDiferencia.BeginInvoke(method, this.dgvDiferencia.Columns[e.ColumnIndex].Name, e.RowIndex);
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDiferencia_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%1" || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%2"
                    || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%3" || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%4"
                    || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%5" || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 1"
                    || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 2" || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 3"
                    || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 4" || this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 5")
                {
                    decimal value;
                    if (!Decimal.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                }
                else
                {
                    this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = null;
                }

                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Etiqueta")
                {
                    int value;
                    if (!int.TryParse(e.FormattedValue.ToString(), out value))
                    {
                        this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = "Debe ingresar una cantidad valida.";
                        e.Cancel = true;
                    }
                }
                else
                {
                    this.dgvDetalleCaptura.Rows[e.RowIndex].ErrorText = null;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDiferencia_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!this.bRecalcularPreciosPorcentajes) return;

            try
            {
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 1")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal porcentaje = 0;
                        porcentaje = value <= 0 ? 0 : value / Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["%1"].Value = porcentaje;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["%1", e.RowIndex], porcentaje);
                    }
                }
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%1")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal precio = 0;
                        precio = value * Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["Precio 1"].Value = precio;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["Precio 1", e.RowIndex], precio);
                    }
                }
                ////
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 2")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal porcentaje = 0;
                        porcentaje = value <= 0 ? 0 : value / Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["%2"].Value = porcentaje;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["%2", e.RowIndex], porcentaje);
                    }
                }
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%2")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal precio = 0;
                        precio = value * Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["Precio 2"].Value = precio;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["Precio 2", e.RowIndex], precio);
                    }
                }
                ////
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 3")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal porcentaje = 0;
                        porcentaje = value <= 0 ? 0 : value / Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["%3"].Value = porcentaje;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["%3", e.RowIndex], porcentaje);
                    }
                }
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%3")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal precio = 0;
                        precio = value * Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["Precio 3"].Value = precio;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["Precio 3", e.RowIndex], precio);
                    }
                }
                ////
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 4")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal porcentaje = 0;
                        porcentaje = value <= 0 ? 0 : value / Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["%4"].Value = porcentaje;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["%4", e.RowIndex], porcentaje);
                    }
                }
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%4")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal precio = 0;
                        precio = value * Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["Precio 4"].Value = precio;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["Precio 4", e.RowIndex], precio);
                    }
                }
                ////
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "Precio 5")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal porcentaje = 0;
                        porcentaje = value <= 0 ? 0 : value / Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["%5"].Value = porcentaje;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["%5", e.RowIndex], porcentaje);
                    }
                }
                if (this.dgvDiferencia.Columns[e.ColumnIndex].Name == "%5")
                {
                    decimal value;
                    if (Decimal.TryParse(this.dgvDiferencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out value))
                    {
                        decimal precio = 0;
                        precio = value * Negocio.Helper.ConvertirDecimal(this.dgvDiferencia.Rows[e.RowIndex].Cells["Costo Nuevo"].Value);
                        // this.dgvDiferencia.Rows[e.RowIndex].Cells["Precio 5"].Value = precio;
                        this.GridDiferenciaCambiarValor(this.dgvDiferencia["Precio 5", e.RowIndex], precio);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDiferencia_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                //Porcentajes y Precios
                if (e.ColumnIndex > 6 && e.ColumnIndex < 17 && e.Button == MouseButtons.Right)
                {
                    var msj = string.Empty;
                    if (e.ColumnIndex > 6 && e.ColumnIndex < 12)
                        msj = "Indica la cantidad que deseas aplicar a %";
                    else
                        msj = "Indica la cantidad que deseas aplicar a Precios.";
                    var frmCantidad = new MensajeObtenerValor(msj, "0.0", MensajeObtenerValor.Tipo.Decimal);
                    if (frmCantidad.ShowDialog(Principal.Instance) == DialogResult.OK)
                    {
                        var valor = Helper.ConvertirDecimal(frmCantidad.Valor);
                        if (valor > 0)
                            foreach (DataGridViewRow row in this.dgvDiferencia.Rows)
                                row.Cells[e.ColumnIndex].Value = valor;
                    }
                    frmCantidad.Dispose();
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Tab Imagen ]

        private void btnIniciarCamara_Click(object sender, EventArgs e)
        {
            try
            {
                if (camDevice != null)
                {
                    camDevice.Attach(this.picBoxCamaraOrigen);
                    this.btnIniciarCamara.Enabled = false;
                    this.btnDetenerCamara.Enabled = true;
                    this.btnConfiguracionCamara.Enabled = true;
                    this.btnCapturar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnDetenerCamara_Click(object sender, EventArgs e)
        {
            if (camDevice != null)
            {
                camDevice.Detach();
                this.btnIniciarCamara.Enabled = true;
                this.btnDetenerCamara.Enabled = false;
                this.btnConfiguracionCamara.Enabled = false;
                this.btnCapturar.Enabled = false;
                this.btnRotar.Enabled = false;
            }
        }

        private void btnConfiguracionCamara_Click(object sender, EventArgs e)
        {
            camDevice.ShowVideoSourceDialog();
        }

        private void btnCapturar_Click(object sender, EventArgs e)
        {
            try
            {
                this.picBoxImagen.Image = camDevice.Capture();
                if (picBoxImagen.Image != null)
                    this.btnRotar.Enabled = true;
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void btnRotar_Click(object sender, EventArgs e)
        {
            if (this.picBoxImagen.Image == null)
                return;
            try
            {
                Image image = this.picBoxImagen.Image;
                this.picBoxImagen.Image = (Bitmap)image.Clone();
                Image oldImage = this.picBoxImagen.Image;
                this.picBoxImagen.Image = Helper.RotateImage(image, 90);
                if (oldImage != null)
                    oldImage.Dispose();
            }
            catch (Exception ex)
            {
                Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        private bool ValidarPrecios()
        {
            DataGridViewCell oCeldaError = null;
            var oPrecios = new decimal[5];
            foreach (DataGridViewRow oFila in this.dgvDiferencia.Rows)
            {
                oPrecios[0] = Helper.ConvertirDecimal(oFila.Cells["Precio 1"].Value);
                oPrecios[1] = Helper.ConvertirDecimal(oFila.Cells["Precio 2"].Value);
                oPrecios[2] = Helper.ConvertirDecimal(oFila.Cells["Precio 3"].Value);
                oPrecios[3] = Helper.ConvertirDecimal(oFila.Cells["Precio 4"].Value);
                oPrecios[4] = Helper.ConvertirDecimal(oFila.Cells["Precio 5"].Value);

                oCeldaError = null;
                for (int i = 0; i < oPrecios.Length; i++)
                {
                    int iMas = (i + 1);
                    decimal mPrecioAnt = oPrecios[i];
                    decimal mPrecioDes = (iMas >= oPrecios.Length ? mPrecioAnt : oPrecios[iMas]);
                    oFila.Cells["Precio " + iMas.ToString()].ErrorText = "";

                    //
                    if (mPrecioAnt == 0)
                    {
                        oCeldaError = oFila.Cells["Precio " + iMas.ToString()];
                        oCeldaError.ErrorText = string.Format("El Precio {0} no puede ser cero.", iMas);
                        break;
                    }
                    // Precio 1 no puede ser mayor a Precio 2
                    if (mPrecioDes > mPrecioAnt)
                    {
                        oCeldaError = oFila.Cells["Precio " + iMas.ToString()];
                        oCeldaError.ErrorText = string.Format("El Precio {0} no puede ser mayor que el Precio {1}.", i + 2, iMas);
                        break;
                    }
                    // No puede haber una diferencia mayor al 15% entre cada precio
                    if ((mPrecioDes / mPrecioAnt) < 0.85M)
                    {
                        oCeldaError = oFila.Cells["Precio " + iMas.ToString()];
                        oCeldaError.ErrorText = string.Format("No puede haber una diferencia mayor al 15% entre el Precio {0} y el Precio {1}.", i + 2, iMas);
                        break;
                    }
                    // Ningún precio puedes ser igual o menor al Costo + Iva
                    if (mPrecioAnt <= UtilLocal.ObtenerImporteMasIva(Helper.ConvertirDecimal(oFila.Cells["Costo Nuevo"].Value)))
                    {
                        oCeldaError = oFila.Cells["Precio " + iMas.ToString()];
                        oCeldaError.ErrorText = string.Format("El Precio {0} no puede ser menor o igual que el Costo más Iva.", iMas);
                        break;
                    }
                }

                if (oCeldaError != null)
                    UtilLocal.MensajeAdvertencia(oCeldaError.ErrorText);

                if (oCeldaError != null)
                    break;
            }

            return (oCeldaError == null);
        }
                
    }
}
