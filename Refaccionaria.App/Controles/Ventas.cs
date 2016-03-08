using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Drawing.Drawing2D;

using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class Ventas : UserControl
    {
        // Para el Singleton *
        private static Ventas instance;
        public static Ventas Instance
        {
            get
            {
                if (Ventas.instance == null || Ventas.instance.IsDisposed)
                    Ventas.instance = new Ventas();
                return Ventas.instance;
            }
        }
        //

        public const string AccesosDeTeclado = "F2 - Cliente | F3 - Búsqueda Código | F5 - Búsqueda Descripción";
        const string MsjMuchosRes = "{__NumPartes} resultados encontrados. Debes ser más específico en tu búsqueda para poder mostrar los productos.";
        const string LlaveImgNoExiste = "0";
        const int MaximoElementosBusquedaImagen = 200;
        const int MaximoElementosBusquedaTexto = 1000;

        ControlError ctlAdvertencia = new ControlError() { Icon = Properties.Resources._16_Ico_Advertencia };
        ControlError ctlError = new ControlError();
        int ParteIDSel;
        BindingList<ProductoVenta> ListaVenta;
        ImageList ImagenesPartes;
        ListViewColumnSorter PartesOrden;
        decimal mPorComision;
        Dictionary<int, int> oCantImagenes;

        // Clase para manejar las opciones
        VentasVenta VVenta;
        VentasOpciones VOp;
        public Cobro ctlCobro;

        // Para la búsqueda avanzada
        const int BusquedaRetrasoTecla = 700;
        int BusquedaLlamada = 0;
        int BusquedaIntento = 0;
        // Para prevenir doble ejecución al dar click en ejecutar
        const int RetrasoClicEjecutar = 400;
        bool EnEjecucion = false;
        //

        public Ventas()
        {
            InitializeComponent();

            this.GotFocus += new EventHandler((s, e) =>
            {
                this.EstablecerTextoEstado(Ventas.AccesosDeTeclado);
            });

            //
            this.VVenta = new VentasVenta(this);
            this.VOp = new VentasOpciones(this);
            // Se asignan eventos para imágenes a los botones de las opciones
            this.AsignarDisenioOpciones();

            // Para poder ordenar las partes en el ListView
            this.lsvPartes.ListViewItemSorter = (this.PartesOrden = new ListViewColumnSorter());
        }

        #region [ Propiedades ]

        // public delegate void ClienteCambiadoEventHandler(ClientesDatosView o);
        public event Action ClienteCambiado;
        private ClientesDatosView _Cliente;
        public ClientesDatosView Cliente
        {
            get { return (this.cmbCliente.SelectedItem as ClientesDatosView); }
            set
            {
                // Si el cambio se da por una fuente externa, se cambia el valor del combo, y se sale, pues el cambio del combo hará que se entre aquí otra vez
                /* if ((this.cmbCliente.SelectedItem as ClientesDatosView) != value)
                {
                    this.cmbCliente.SelectedValue = (value == null ? 0 : value.ClienteID);
                    return;
                }
                */

                if (this.Cliente != null && Helper.ConvertirEntero(this.tacCliente.ValorSel) != this.Cliente.ClienteID)
                    this.tacCliente.ValorSel = (value == null ? 0 : value.ClienteID);
                
                // Se manda ejecutar el evento "ClienteCambiado"
                if (this.ClienteCambiado != null)
                    this.ClienteCambiado.Invoke();
            }
        }

        public bool PermitirCambiarCliente
        {
            get { return this.cmbCliente.Enabled; }
            set
            {
                this.cmbCliente.Enabled = value;
                this.btnAgregarCliente.Enabled = value;
            }
        }

        public decimal Total { get { return Helper.ConvertirDecimal(this.lblTotal.Text.SoloNumeric()); } }

        public bool CobroAlFrente { get { return (Helper.ControlAlFrente(this.pnlContenidoDetalle) == this.ctlCobro); } }

        public bool EsCotizacion { get { return this.chkCotizacion.Checked; } }

        public List<VentaParteAplicacion> oAplicaciones { get; set; }

        #endregion

        #region [ Eventos ]

        private void Ventas_Load(object sender, EventArgs e)
        {
            // Para la comisión
            this.mPorComision = (Helper.ConvertirDecimal(Config.Valor("Comisiones.Vendedor.Porcentaje")) / 100);

            // Se cargan los datos de los clientes
            this.cmbCliente.CargarDatos("ClienteID", "Nombre", General.GetListOf<ClientesDatosView>(q => q.ListaDePrecios > 0).OrderBy(q => q.Nombre).ToList());
            this.tacCliente.CargarDatos("ClienteID", "Nombre", General.GetListOf<Cliente>().OrderBy(o => o.Nombre).ToList());
            this.tacCliente.ValorSel = Cat.Clientes.Mostrador;
            // this.cmbCliente.SelectedValue = Cat.Clientes.Mostrador;
            
            // Se configura lo de la venta
            this.ListaVenta = new BindingList<ProductoVenta>();
            this.dgvProductos.AutoGenerateColumns = false;
            this.oAplicaciones = new List<VentaParteAplicacion>();
            
            // Se cargan los datos de los combos para el filtro por Vehículos
            // this.cmbVehiculo.CargarDatos("ModeloID", "NombreModelo", General.GetListOf<Modelo.Modelo>(q => q.Estatus).OrderBy(o => o.NombreModelo).ToList());
            this.tacVechiculo.CargarDatos("ModeloID", "NombreModelo", General.GetListOf<ModelosView>().OrderBy(c => c.NombreModelo).ToList());
            this.tacVechiculo.MostrarVariasColumnas("NombreModelo", "NombreMarca");
            this.cmbMarca.CargarDatos("MarcaID", "NombreMarca", General.GetListOf<Modelo.Marca>(q => q.Estatus).OrderBy(o => o.NombreMarca).ToList());
            this.cmbModelo.ValueMember = "ModeloID";
            this.cmbModelo.DisplayMember = "NombreModelo";
            // this.cmbModelo.SelectedIndex = -1;
            this.cmbAnio.ValueMember = "Anio";
            this.cmbAnio.DisplayMember = "Anio";
            // this.cmbAnio.SelectedIndex = -1;
            this.cmbMotor.ValueMember = "MotorID";
            this.cmbMotor.DisplayMember = "NombreMotor";
            // this.cmbMotor.SelectedIndex = -1;

            this.cmbVehiculo.Text = "";

            // Se cargan los datos de los combos para el filtro avanzado
            this.cmbLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Modelo.Linea>(q => q.Estatus).OrderBy(o => o.NombreLinea).ToList());
            this.cmbSistema.CargarDatos("SistemaID", "NombreSistema", General.GetListOf<Modelo.Sistema>(q => q.Estatus).OrderBy(o => o.NombreSistema).ToList());
            this.cmbMarcaParte.CargarDatos("MarcaParteID", "NombreMarcaParte", General.GetListOf<Modelo.MarcaParte>(q => q.Estatus)
                .OrderBy(o => o.NombreMarcaParte).ToList());
            this.RestaurarCaracteristicas();

            // Para la lista de productos
            this.ImagenesPartes = new ImageList() { ImageSize = new Size(100, 75) };
            this.ImagenesPartes.Images.Add(Ventas.LlaveImgNoExiste, Properties.Resources.ParteSinImagen);

            this.lsvPartes.LargeImageList = this.ImagenesPartes;
            this.lsvPartesComplementarias.LargeImageList = this.ImagenesPartes;
            this.lsvPartesEquivalentes.LargeImageList = this.ImagenesPartes;
            //
            LibExt.Especiales.ListViewItem_SetSpacing(this.lsvPartes, 158, 120);

            // Se obtiene el número de imágenes que tiene cada parte
            this.oCantImagenes = AdmonProc.ObtenerCantidadDeImagenesPorParte();

            // Texto de estado
            this.EstablecerTextoEstado(Ventas.AccesosDeTeclado);

            // Se asigna la imagen correspondiente a la barra de metas
            var oMetasSuc = General.GetEntity<MetaSucursal>(c => c.SucursalID == GlobalClass.SucursalID);
            if (oMetasSuc.DiasPorSemana == 6)
            {
                // this.pcbMetasRegla.Image = null;
                this.pcbMetasRegla.Image = Properties.Resources.MetasMarcasBarraVentas6;
            }
            // Se manda actualizar la barra de metas
            VentasProc.ActualizarBarraDeMetas(null);
        }

        private void cmbCliente_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbCliente.Text == "")
                this.cmbCliente_SelectedIndexChanged(sender, e);
        }

        private void cmbCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cliente = (this.cmbCliente.SelectedItem as ClientesDatosView);
            if (this.Cliente == null)
            {
                this.LimpiarDatosCliente();
                return;
            }

            this.lblClienteDatos.Text = (this.Cliente.Direccion + ", " + this.Cliente.Colonia + ", " + this.Cliente.Ciudad);
            this.lblClienteDatos.Text += "\n";
            this.lblClienteDatos.Text += ("RFC: " + this.Cliente.Rfc + " C.P. " + this.Cliente.CodigoPostal + " TEL. " + this.Cliente.Telefono);
            //this.lblClienteAdicional.Text = "RFC: " + this.Cliente.Rfc + " C.P. " + this.Cliente.CodigoPostal + " Tel. " + this.Cliente.Telefono;
            this.lblClienteListaDePrecios.Text = this.Cliente.ListaDePrecios.ToString();
            this.lblClienteTieneCredito.Text = (this.Cliente.TieneCredito ? "SÍ" : "NO");
            this.lblClienteTieneCredito.ForeColor = Color.SteelBlue;

            // Se obtiene información sobre el crédito
            try
            { // dpend Quitar el try después de lo del cambio del servidor
                var oClienteCredito = General.GetEntity<ClientesCreditoView>(q => q.ClienteID == this.Cliente.ClienteID);
                if (oClienteCredito.AdeudoVencido > 0)
                {
                    this.lblClienteTieneCredito.Text += " (SUSPENDIDO)";
                    this.lblClienteTieneCredito.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                this.lblClienteTieneCredito.Text += " (ERROR)";
            }

            // Se actualizan los datos de la parte seleccionada
            this.chkPartesPrecios_CheckedChanged(this, null);

            // Se actualizan los datos de la venta
            this.AplicarListaDePrecios(this.Cliente.ListaDePrecios);
        }

        private void tacCliente_SeleccionCambiada(object sender, EventArgs e)
        {
            // Se cambia el cliente del combo anterior, lo cual desencadena el evento "cmbCliente_SelectedIndexChanged"..
            this.cmbCliente.SelectedValue = Helper.ConvertirEntero(this.tacCliente.ValorSel);
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            var bus = DetalleBusquedaDeClientes.Instance;
            bus.ShowDialog();
            int iClienteID = bus.Sel;

            if (iClienteID > 0)
                this.cmbCliente.SelectedValue = iClienteID;
        }

        private void btnActualizarClientes_Click(object sender, EventArgs e)
        {
            this.ActualizarComboClientes();
            this.tacCliente.Focus();
        }

        private void btnAgregarCliente_Click(object sender, EventArgs e)
        {
            var ctlClientes = new clientes();
            ctlClientes.ModificaCredito = false;
            ContenedorControl frmCliente = new ContenedorControl("Agregar Cliente", ctlClientes);            
            ctlClientes.CargarCliente(0);
            var btnGuardar = (ctlClientes.Controls[0].Controls[0].Controls["btnGuardar"] as Button);
            btnGuardar.Click += new EventHandler((oS, oE) => { 
                if (ctlClientes.Guardado)
                    frmCliente.Close();
            });

            frmCliente.ShowDialog(Principal.Instance);
            if (ctlClientes.Guardado)
            {
                this.ActualizarComboClientes();
                this.tacCliente.ValorSel = ctlClientes.ClienteID;
            }
            frmCliente.Dispose();
        }

        private void btnEditarCliente_Click(object sender, EventArgs e)
        {
            if (this.Cliente == null) return;

            // Se valida que el cliente no sea "Ventas Mostrador"
            if (this.Cliente.ClienteID == Cat.Clientes.Mostrador)
            {
                UtilLocal.MensajeAdvertencia("El cliente especificado no se puede editar.");
                return;
            }

            var ctlClientes = new clientes();
            ctlClientes.ModificaCredito = false;
            ctlClientes.ModificaCaracteristicas = false;
            ctlClientes.VieneDeVentas = true;
            ContenedorControl frmCliente = new ContenedorControl("Editar Cliente", ctlClientes);
            ctlClientes.CargarCliente(this.Cliente.ClienteID);
            var btnGuardar = (ctlClientes.Controls[0].Controls[0].Controls["btnGuardar"] as Button);
            btnGuardar.Click += new EventHandler((oS, oE) =>
            {
                if (ctlClientes.Guardado)
                    frmCliente.Close();
            });

            frmCliente.ShowDialog(Principal.Instance);
            if (ctlClientes.Guardado)
            {
                this.ActualizarComboClientes();
            }
            frmCliente.Dispose();
        }

        private void cmbVehiculo_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbVehiculo.Focused && this.cmbVehiculo.Text == "")
            {
                this.cmbMarca.Text = "";
                this.cmbModelo.Text = "";
                this.cmbAnio.Text = "";
                this.cmbMotor.Text = "";
                this.cmbMarca.SelectedIndex = -1;
                this.cmbModelo.SelectedIndex = -1;
                this.cmbAnio.SelectedIndex = -1;
                this.cmbMotor.SelectedIndex = -1;
            }
        }

        private void cmbVehiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbVehiculo.Focused)
                return;

            var VehiculoSel = (this.cmbVehiculo.SelectedItem as Modelo.Modelo);
            if (VehiculoSel == null)
            {
                // this.cmbMarca.SelectedIndex = -1;
            }
            else
            {
                var oMarca = General.GetEntity<Marca>(q => q.MarcaID == VehiculoSel.MarcaID);
                this.cmbMarca.Text = oMarca.NombreMarca;
                this.cmbModelo.Text = VehiculoSel.NombreModelo;
            }

            if (this.chkBusquedaPorAplicacion.Checked)
                this.BusquedaAvanzada();
        }

        private void tacVehiculo_TextoCambiado(object sender, EventArgs e)
        {
            if (this.tacVechiculo.Texto == "")
            {
                this.cmbMarca.Text = "";
                this.cmbModelo.Text = "";
                this.cmbAnio.Text = "";
                this.cmbMotor.Text = "";
                this.cmbMarca.SelectedIndex = -1;
                this.cmbModelo.SelectedIndex = -1;
                this.cmbAnio.SelectedIndex = -1;
                this.cmbMotor.SelectedIndex = -1;
            }
        }

        private void tacVehiculo_SeleccionCambiada(object sender, EventArgs e)
        {
            //if (!this.tacVechiculo.Focused)
            //    return;

            int iModeloID = Helper.ConvertirEntero(this.tacVechiculo.ValorSel);
            var oSel = General.GetEntity<Modelo.Modelo>(c => c.ModeloID == iModeloID && c.Estatus);
            if (oSel == null)
            {
                // this.cmbMarca.SelectedIndex = -1;
            }
            else
            {
                var oMarca = General.GetEntity<Marca>(q => q.MarcaID == oSel.MarcaID);
                this.cmbMarca.Text = oMarca.NombreMarca;
                this.cmbModelo.Text = oSel.NombreModelo;
            }

            // Ya no se realiza la búsqueda al filtrar un vehículo
            // if (this.chkBusquedaPorAplicacion.Checked)
            //     this.BusquedaAvanzada();
        }

        private void cmbMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            var iMarcaID = Helper.ConvertirEntero(this.cmbMarca.SelectedValue);
            this.cmbModelo.DataSource = General.GetListOf<Modelo.Modelo>(q => q.Estatus && q.MarcaID == iMarcaID).OrderBy(o => o.NombreModelo).ToList();

            // if (this.cmbMarca.Focused && this.chkBusquedaPorAplicacion.Checked)
            //     this.BusquedaAvanzada();
        }

        private void cmbModelo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var iModeloID = Helper.ConvertirEntero(this.cmbModelo.SelectedValue);
            this.cmbAnio.DataSource = General.GetListOf<Modelo.ModelosAniosView>(q => q.ModeloID == iModeloID).OrderBy(o => o.Anio).ToList();
            this.cmbAnio.SelectedIndex = -1;

            if (!this.cmbModelo.Focused) return;

            this.cmbVehiculo.Text = this.cmbModelo.Text;
            this.tacVechiculo.Texto = "";

            // if (this.chkBusquedaPorAplicacion.Checked)
            //     this.BusquedaAvanzada();
        }

        private void cmbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            var iModeloID = Helper.ConvertirEntero(this.cmbModelo.SelectedValue);
            var iAnio = Helper.ConvertirEntero(this.cmbAnio.SelectedValue);
            this.cmbMotor.DataSource = General.GetListOf<MotoresAniosView>(q => q.ModeloID == iModeloID && q.Anio == iAnio).OrderBy(o => o.NombreMotor).ToList();
            this.cmbMotor.SelectedIndex = -1;

            // if (this.cmbAnio.Focused && this.chkBusquedaPorAplicacion.Checked)
            //     this.BusquedaAvanzada();
        }

        private void cmbAnio_TextChanged(object sender, EventArgs e)
        {
            // if (this.cmbAnio.Focused && this.cmbAnio.Text == "")
            //     this.BusquedaAvanzada();
        }

        private void cmbMotor_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (this.cmbMotor.Focused && this.chkBusquedaPorAplicacion.Checked)
            //     this.BusquedaAvanzada();
        }

        private void cmbMotor_TextChanged(object sender, EventArgs e)
        {
            // if (this.cmbMotor.Focused && this.cmbMotor.Text == "")
            //     this.BusquedaAvanzada();
        }

        private void btnAplicarVehiculo_Click(object sender, EventArgs e)
        {
            if (this.cmbModelo.SelectedValue == null)
            {
                UtilLocal.MensajeAdvertencia("Debes seleccionar un Modelo para poder aplicar.");
                return;
            }

            AplicarPartesAVehiculo frmAplicar = new AplicarPartesAVehiculo(new Vehiculo()
            {
                Marca = this.cmbMarca.Text
                , Modelo = this.cmbModelo.Text
                , ModeloID = Helper.ConvertirEntero(this.cmbModelo.SelectedValue)
                , Anio = Helper.ConvertirEntero(this.cmbAnio.SelectedValue)
                , MotorID = Helper.ConvertirEntero(this.cmbMotor.SelectedValue)
                , Motor = this.cmbMotor.Text
            }, this.ListaVenta);

            frmAplicar.ShowDialog(Principal.Instance);
            frmAplicar.Dispose();
        }

        private void txtVIN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var frmBusquedaVIN = new BusquedaVIN(this.txtVIN.Text);
                frmBusquedaVIN.ShowDialog(Principal.Instance);
                frmBusquedaVIN.Dispose();
            }
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvProductos.CurrentRow == null) return;

            var Producto = (this.dgvProductos.CurrentRow.DataBoundItem as ProductoVenta);
            if (Producto.ParteID != this.ParteIDSel)
            {
                this.MostrarEquivalentes(Producto.ParteID);
                // Se actualizan los datos adicionales
                this.MostrarDatosParteSeleccionada(Producto.ParteID);
            }
        }

        private void dgvProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvProductos.CurrentRow == null) return;

            var Fila = this.dgvProductos.CurrentRow;

            switch (e.KeyCode)
            {
                case Keys.Add:
                    this.AgregarQuitarCantidad(Fila, 1);
                    break;
                case Keys.Subtract:
                    this.AgregarQuitarCantidad(Fila, -1);
                    break;
                case Keys.Enter:
                    var ProdCantidad = (Fila.DataBoundItem as ProductoVenta);
                    var frmCantidad = new MensajeObtenerValor("Indica la cantidad que deseas aplicar.",
                        (ProdCantidad.Cantidad + 1).ToString(), MensajeObtenerValor.Tipo.Decimal);
                    if (frmCantidad.ShowDialog(Principal.Instance) == DialogResult.OK)
                        this.ModificarCantidad(this.dgvProductos.SelectedRows[0], Helper.ConvertirDecimal(frmCantidad.Valor));

                    e.Handled = true;

                    frmCantidad.Dispose();
                    break;
                case Keys.Multiply:
                    this.CambiarPrecio(Fila.DataBoundItem as ProductoVenta);
                    break;
                case Keys.Delete:
                    this.QuitarProductoVenta(Fila);
                    break;
            }
        }

        private void dgvProductos_CurrentCellChanged(object sender, EventArgs e)
        {
            /* if (this.dgvProductos.CurrentRow == null) return;

            var Producto = (this.dgvProductos.CurrentRow.DataBoundItem as ProductoVenta);
            this.MostrarEquivalentes(Producto.ParteID);
            // Se actualizan los datos adicionales
            this.MostrarDatosParteSeleccionada(Producto.ParteID);
            */
        }

        private void dgvProductos_Enter(object sender, EventArgs e)
        {
            /* if (this.dgvProductos.CurrentRow == null) return;
            
            var Producto = (this.dgvProductos.CurrentRow.DataBoundItem as ProductoVenta);
            if (Producto.ParteID != this.ParteIDSel)
                this.dgvProductos_CurrentCellChanged(sender, e);
            */
        }
                
        private void lblTotal_DoubleClick(object sender, EventArgs e)
        {
            if (this.ListaVenta.Count <= 0) return;

            var frmPrecio = new MensajeObtenerValor("Indica el importe total a cobrar:", this.lblTotal.Text.SoloNumeric(), MensajeObtenerValor.Tipo.Decimal);
            if (frmPrecio.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.AplicarImporteTotal(Helper.ConvertirDecimal(frmPrecio.Valor));
            frmPrecio.Dispose();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            if (this.VOp.Opcion == VentasOpciones.eOpcion.Venta)
                this.LimpiarVenta();
            else
                this.VOp.Limpiar();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Devolucion);
        }

        private void btnReporteDeFaltante_Click(object sender, EventArgs e)
        {
            this.ReporteDeFaltante(this.ParteIDSel);
        }

        private void btnReimpresion_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Reimpresion);
        }

        private void btnFacturarVentas_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.FacturarVenta);
        }

        private void btnCaja_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Caja);
        }

        private void btn9500_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.c9500);
        }

        private void btnCobranza_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Cobranza);
        }

        private void btnGarantia_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Garantia);
        }

        private void btnDirectorio_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Directorio);
            this.VOp.oDirectorio.ctlDirectorio.MuestraTab(DirectorioVentasDetalle.eModo.Proveedores);
            this.VOp.oDirectorio.ctlDirectorio.primerSeleccionProvedor(0);
        }

        private void btnGanancias_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Comisiones);
        }

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            if (this.EnEjecucion)
                return;

            this.Enabled = false;
            this.EnEjecucion = true;

            if (this.VOp.Opcion == VentasOpciones.eOpcion.Venta)
                this.EjecutarVenta();
            else
                this.VOp.Ejecutar();

            new System.Threading.Thread(() => {
                System.Threading.Thread.Sleep(Ventas.RetrasoClicEjecutar);
                this.EnEjecucion = false;
            }).Start();
            this.Enabled = true; 
        }

        private void chkCotizacion_CheckedChanged(object sender, EventArgs e)
        {
            this.chkCotizacion.Enabled = false;
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(this.txtCodigo.Text)) return;

            if (this.Cliente == null)
            {
                UtilLocal.MensajeAdvertencia("No hay ningún cliente seleccionado o selección incorrecta.");
                return;
            }

            PartesVentasView oPartePrecio = null;
            var Precios = General.GetListOf<PartesVentasView>(q => (q.NumeroParte == this.txtCodigo.Text || q.CodigoBarra == this.txtCodigo.Text)
                && q.ParteEstatusID == Cat.PartesEstatus.Activo);
            switch (Precios.Count)
            {
                case 0: return;
                case 1: oPartePrecio = Precios[0]; break;
                default:  // Hay más de una parte que concuerda con la búsqueda, se muestra un listado para elegir
                    var frmLista = new ObtenerElementoLista("Selecciona la parte que deseas agregar:", Precios);
                    frmLista.Text = "Número de parte o código repetido";
                    frmLista.MostrarColumnas("CodigoBarra", "NumeroParte", "NombreParte");
                    frmLista.dgvDatos.Columns["NombreParte"].DisplayIndex = 0;
                    frmLista.dgvDatos.Columns["NumeroParte"].DisplayIndex = 1;
                    frmLista.ShowDialog(Principal.Instance);
                    if (frmLista.DialogResult == DialogResult.OK)
                        oPartePrecio = (frmLista.Seleccion as PartesVentasView);
                    frmLista.Dispose();
                    if (oPartePrecio == null)
                        return;
                    break;
            }

            this.AgregarProductoVenta(oPartePrecio);
            this.txtCodigo.Text = "";
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {
            /* if (this.txtCodigo.Focused)
                this.BusquedaAvanzada();
            */
        }
                
        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            if (!this.txtDescripcion.Focused)
                return;
            if (this.txtDescripcion.TextLength > 0 && this.txtDescripcion.TextLength < 4)
                return;

            // Se implementa mecanismo de restraso para teclas, si se presionan demasiado rápido, no se hace la búsqueda
            this.BusquedaLlamada++;
            new System.Threading.Thread(this.IniciarBusquedaAsincrona).Start();

            // Se manda hacer el filtro
            //this.BusquedaAvanzada();
        }
                
        private void btnVistaIconos_Click(object sender, EventArgs e)
        {
            this.lsvPartes.View = View.LargeIcon;

            /* Ya no se requieres con el OwnerDraw
            // Se requiere hacer esto (es ríduculo pero..) para que se muestren las imágenes cuando los elementos fueron cargados en vista de detalle
            foreach (ListViewItem Elemento in this.lsvPartes.Items)
                Elemento.ImageKey = Elemento.ImageKey;  // o.O ??
            */
        }

        private void btnVistaDetalle_Click(object sender, EventArgs e)
        {
            this.lsvPartes.View = View.Details;
        }

        private void chkBusquedaPorAplicacion_CheckedChanged(object sender, EventArgs e)
        {
            this.BusquedaAvanzada();
        }

        private void chkEquivalentes_CheckedChanged(object sender, EventArgs e)
        {
            this.BusquedaAvanzada();
        }

        private void txtCodigoAlterno_TextChanged(object sender, EventArgs e)
        {
            if (this.txtCodigoAlterno.Focused)
                this.BusquedaAvanzada();
        }

        private void cmbSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSistema.Focused)
                this.BusquedaAvanzada();
        }

        private void cmbSistema_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbSistema.Focused && this.cmbSistema.Text == "")
            {
                // this.RestaurarCaracteristicas();
                this.BusquedaAvanzada();
            }
        }
                
        private void cmbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbLinea.Focused)
            {
                // Se filtran las marcas por la línea seleccionada
                var oMarcasPartes = General.GetListOf<MarcaParte>(c => c.Estatus).OrderBy(o => o.NombreMarcaParte).ToList();
                int iLineaID = Helper.ConvertirEntero(this.cmbLinea.SelectedValue);
                var oLineasMarcas = General.GetListOf<LineaMarcaParte>(c => c.Estatus && c.LineaID == iLineaID).Select(c => c.MarcaParteID);
                for (int iCont = 0; iCont < oMarcasPartes.Count; iCont++)
                {
                    if (!oLineasMarcas.Contains(oMarcasPartes[iCont].MarcaParteID))
                    {
                        oMarcasPartes.RemoveAt(iCont);
                        iCont--;
                    }
                }
                var oSel = this.cmbMarcaParte.SelectedValue;
                this.cmbMarcaParte.CargarDatos("MarcaParteID", "NombreMarcaParte", oMarcasPartes);
                if (oSel != null)
                    this.cmbMarcaParte.SelectedValue = oSel;
            
                // Se muestran las características correspondientes
                this.CargarCaracteristicas(iLineaID);

                // 
                this.BusquedaAvanzada();
            }
        }

        private void cmbLinea_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbLinea.Focused && this.cmbLinea.Text == "")
            {
                // Se quitan los filtros (si hubiera) de la línea y de la marca
                this.cmbMarcaParte.Focus();
                this.cmbLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Modelo.Linea>(q => q.Estatus).OrderBy(o => o.NombreLinea).ToList());
                this.cmbLinea.Focus();
                var oSel = this.cmbMarcaParte.SelectedValue;
                this.cmbMarcaParte.CargarDatos("MarcaParteID", "NombreMarcaParte", General.GetListOf<MarcaParte>(q => q.Estatus)
                    .OrderBy(o => o.NombreMarcaParte).ToList());
                if (oSel != null)
                    this.cmbMarcaParte.SelectedValue = oSel;
                // Se restauran las características
                this.RestaurarCaracteristicas();
                // 
                this.BusquedaAvanzada();
            }
        }

        private void cmbMarcaParte_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbMarcaParte.Focused)
            {
                // Se filtran las líneas por la marca seleccionada
                var oLineas = General.GetListOf<Linea>(c => c.Estatus).OrderBy(o => o.NombreLinea).ToList();
                int iMarcaID = Helper.ConvertirEntero(this.cmbMarcaParte.SelectedValue);
                var oLineasMarcas = General.GetListOf<LineaMarcaParte>(c => c.Estatus && c.MarcaParteID == iMarcaID).Select(c => c.LineaID);
                for (int iCont = 0; iCont < oLineas.Count; iCont++)
                {
                    if (!oLineasMarcas.Contains(oLineas[iCont].LineaID))
                    {
                        oLineas.RemoveAt(iCont);
                        iCont--;
                    }
                }
                var oSel = this.cmbLinea.SelectedValue;
                this.cmbLinea.CargarDatos("LineaID", "NombreLinea", oLineas);
                if (oSel != null)
                    this.cmbLinea.SelectedValue = oSel;
                // 
                this.BusquedaAvanzada();
            }
        }

        private void cmbMarcaParte_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbMarcaParte.Focused && this.cmbMarcaParte.Text == "")
            {
                // Se quitan los filtros (si hubiera) de la línea y de la marca
                this.cmbLinea.Focus();
                this.cmbMarcaParte.CargarDatos("MarcaParteID", "NombreMarcaParte", General.GetListOf<Modelo.MarcaParte>(q => q.Estatus)
                    .OrderBy(o => o.NombreMarcaParte).ToList());
                this.cmbMarcaParte.Focus();
                var oSel = this.cmbLinea.SelectedValue;
                this.cmbLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(q => q.Estatus).OrderBy(o => o.NombreLinea).ToList());
                if (oSel != null)
                    this.cmbLinea.SelectedValue = oSel;
                // 
                this.BusquedaAvanzada();
            }
        }

        private void lsvPartes_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void lsvPartes_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (this.lsvPartes.View == View.Details)
            {
                e.DrawDefault = true;
                if (e.Item.Selected)
                {
                    e.Item.BackColor = SystemColors.Highlight;
                }
                else
                {
                    e.Item.ForeColor = e.Item.ListView.ForeColor;
                    e.Item.BackColor = e.Item.ListView.BackColor;
                }
                return;
            }

            // Se manda dibujar la parte
            this.DibujarParteLista(e);
        }

        private void lsvPartes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lsvPartes.SelectedItems.Count <= 0)
                return;

            // Se actualizan los datos adicionales
            int iParteID = this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]);
            this.MostrarDatosParteSeleccionada(iParteID);
        }

        private void lsvPartes_Enter(object sender, EventArgs e)
        {
            if (this.lsvPartes.SelectedItems.Count <= 0) return;

            int iParteID = this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]);
            if (iParteID != this.ParteIDSel)
                this.lsvPartes_SelectedIndexChanged(sender, e);
        }

        private void lsvPartes_DoubleClick(object sender, EventArgs e)
        {
            if (this.lsvPartes.SelectedItems.Count <= 0) return;

            VerImagenesParte frmImagenes = new VerImagenesParte(this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]));
            frmImagenes.ShowDialog(Principal.Instance);
            frmImagenes.Dispose();
        }

        private void lsvPartes_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == this.PartesOrden.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (this.PartesOrden.Order == SortOrder.Ascending)
                    this.PartesOrden.Order = SortOrder.Descending;
                else
                    this.PartesOrden.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                this.PartesOrden.SortColumn = e.Column;
                this.PartesOrden.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lsvPartes.Sort();
        }

        private void smiPar_Kardex_Click(object sender, EventArgs e)
        {
            Principal.Instance.CargarControl(mantenimiento.Instance);
            mantenimiento.Instance.SeleccionarOpcion("tabPartes");
            int iParteID = this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]);
            catalogosPartes.Instance.VerKardex(iParteID);
        }

        private void smiReportarErrorParte_Click(object sender, EventArgs e)
        {
            int iParteID = this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]);
            var frmError = new ReportarErrorParte(iParteID);
            frmError.ShowDialog(Principal.Instance);
            frmError.Dispose();
        }

        private void lsvPartesComplementarias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lsvPartesComplementarias.SelectedItems.Count <= 0)
                return;

            // Para mostrar los datos de la parte seleccionada
            int iParteID = this.ObtenerParteIDDeLista(this.lsvPartesComplementarias.SelectedItems[0]);
            this.MostrarDatosParteSeleccionada(iParteID);
        }

        private void lsvPartesComplementarias_DoubleClick(object sender, EventArgs e)
        {
            if (this.lsvPartesComplementarias.SelectedItems.Count <= 0) return;

            VerImagenesParte frmImagenes = new VerImagenesParte(this.ObtenerParteIDDeLista(this.lsvPartesComplementarias.SelectedItems[0]));
            frmImagenes.ShowDialog(Principal.Instance);
            frmImagenes.Dispose();
        }

        private void lsvPartesComplementarias_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Se manda dibujar la parte
            this.DibujarParteLista(e);
        }

        private void lsvPartesEquivalentes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lsvPartesEquivalentes.SelectedItems.Count <= 0)
                return;

            // Para mostrar los datos de la parte seleccionada
            int iParteID = this.ObtenerParteIDDeLista(this.lsvPartesEquivalentes.SelectedItems[0]);
            this.MostrarDatosParteSeleccionada(iParteID);
        }

        private void lsvPartesEquivalentes_DoubleClick(object sender, EventArgs e)
        {
            if (this.lsvPartesEquivalentes.SelectedItems.Count <= 0) return;

            VerImagenesParte frmImagenes = new VerImagenesParte(this.ObtenerParteIDDeLista(this.lsvPartesEquivalentes.SelectedItems[0]));
            frmImagenes.ShowDialog(Principal.Instance);
            frmImagenes.Dispose();
        }

        private void lsvPartesEquivalentes_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Se manda dibujar la parte
            this.DibujarParteLista(e);
        }
                
        private void chkPartesPrecios_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Cliente == null) return;

            this.lblPartesPrecio2.Visible = (this.chkPartesPrecios.Checked || this.Cliente.ListaDePrecios >= 2);
            this.lblPartesPrecio3.Visible = (this.chkPartesPrecios.Checked || this.Cliente.ListaDePrecios >= 3);
            this.lblPartesPrecio4.Visible = (this.chkPartesPrecios.Checked || this.Cliente.ListaDePrecios >= 4);
            this.lblPartesPrecio5.Visible = (this.chkPartesPrecios.Checked || this.Cliente.ListaDePrecios >= 5);
        }
                
        #endregion

        #region [ Métodos ]

        private void LimpiarDatosCliente()
        {
            this.lblClienteDatos.Text = "";
            this.lblClienteListaDePrecios.Text = "";
            this.lblClienteTieneCredito.Text = "";
        }

        private void AgregarProductoVenta(PartesVentasView ParteV)
        {
            // Se valida que no sea un producto casco
            if (General.Exists<Parte>(c => c.ParteID == ParteV.ParteID && c.EsCascoPara > 0 && c.Estatus))
            {
                UtilLocal.MensajeAdvertencia("El artículo seleccionado está marcado como Casco, por lo cual no se puede vender.");
                return;
            }

            ProductoVenta Producto = null;

            // Se verifica si el producto ya existe
            foreach (ProductoVenta ProductoV in this.ListaVenta) {
                if (ProductoV.ParteID == ParteV.ParteID)
                {
                    Producto = ProductoV;
                    Producto.Cantidad++;
                }
            }

            // Se agrega
            if (Producto == null)
            {
                Producto = new ProductoVenta()
                {
                    ParteID = ParteV.ParteID,
                    NumeroDeParte = ParteV.NumeroParte,
                    NombreDeParte = ParteV.NombreParte,
                    Precios = new decimal[] {
                        ParteV.PrecioUno.Valor()
                        , ParteV.PrecioDos.Valor()
                        , ParteV.PrecioTres.Valor()
                        , ParteV.PrecioCuatro.Valor()
                        , ParteV.PrecioCinco.Valor()
                    },
                    Existencias = new decimal[] {
                        ParteV.ExistenciaSuc01.Valor()
                        , ParteV.ExistenciaSuc02.Valor()
                        , ParteV.ExistenciaSuc03.Valor()
                    },
                    Costo = ParteV.Costo.Valor(),
                    CostoConDescuento = (ParteV.CostoConDescuento ?? ParteV.Costo.Valor()),
                    Cantidad = 1,
                    EsServicio = ParteV.EsServicio.Valor(),
                    Es9500 = ParteV.Es9500.Valor(),
                    AGranel = ParteV.AGranel
                };

                // Si es a granel y la existencia es menor a 1, se ajusta para que pase la validación
                if (Producto.AGranel && Producto.Existencias[GlobalClass.SucursalID - 1] > 0 && Producto.Existencias[GlobalClass.SucursalID - 1] < 1)
                    Producto.Cantidad = Producto.Existencias[GlobalClass.SucursalID - 1];

                // Se agrega a la lista, si tiene existencia
                if (this.EsCotizacion || this.ValidarExistencia(Producto, Producto.Cantidad))
                    this.ListaVenta.Add(Producto);
                else
                    return;
            }
            else
            {
                // Se valida la existencia
                if (!this.EsCotizacion && !this.ValidarExistencia(Producto, Producto.Cantidad))
                {
                    Producto.Cantidad--;
                    return;
                }
            }

            // Se muenstran los cambios
            this.AplicarCambioProducto(Producto);

            // Se agrega para aplicación, si fuera el caso
            this.VerAplicacionProducto(Producto);
        }

        private void AplicarCambioProducto(ProductoVenta Producto)
        {
            if (this.Cliente == null) return;

            // Se obtiene el precio correspondiente
            decimal mPrecio = Producto.Precios[this.Cliente.ListaDePrecios - 1];
            // Se evalúa si se debe cobrar el envío
            if (this.Cliente.CobroPorEnvio.Valor())
            {
                if (mPrecio >= this.Cliente.ImporteParaCobroPorEnvio.Valor())
                    mPrecio += this.Cliente.ImporteCobroPorEnvio.Valor();
            }

            this.AplicarPrecioProducto(Producto, mPrecio);
        }

        private void AgregarQuitarCantidad(DataGridViewRow Fila, int iIncremento)
        {
            var Producto = (Fila.DataBoundItem as ProductoVenta);

            // Si ya sólo queda uno, no se sigue decrementando
            if (iIncremento < 0 && Producto.Cantidad == 1)
                return;

            // Se manda afectar el producto, con el nuevo incremento
            this.ModificarCantidad(Fila, Producto.Cantidad + iIncremento);
        }

        private void ModificarCantidad(DataGridViewRow Fila, decimal mCantidad)
        {
            var Producto = (Fila.DataBoundItem as ProductoVenta);

            if (mCantidad < 0) return;
            if (mCantidad == 0) this.QuitarProductoVenta(Fila);

            if (mCantidad != (int)mCantidad && !Producto.AGranel)
            {
                UtilLocal.MensajeAdvertencia("La parte seleccionada no se puede vender a granel.");
                return;
            }

            // Se valida la existencia
            if (!this.EsCotizacion && !this.ValidarExistencia(Producto, mCantidad))
                return;

            Producto.Cantidad = mCantidad;
            this.AplicarCambioProducto(Producto);
        }

        private void QuitarProductoVenta(DataGridViewRow Fila)
        {
            var oParte = (Fila.DataBoundItem as ProductoVenta);
            this.ListaVenta.Remove(oParte);
            this.ActualizarListaVenta();

            // Se verifica si hay una aplicación y se quita
            var oApli = this.oAplicaciones.FirstOrDefault(c => c.ParteID == oParte.ParteID);
            if (oApli != null)
                this.oAplicaciones.Remove(oApli);
        }

        private bool ValidarExistencia(ProductoVenta Producto, decimal mCantidad)
        {
            if (!Producto.EsServicio && mCantidad > Producto.Existencias[GlobalClass.SucursalID - 1])
            {
                if (MessageBox.Show("No hay suficiente existencia para agregar el producto seleccionado. ¿Deseas reportar el incidente a Compras?",
                    "Existencia insuficiente", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    this.ReporteDeFaltante(Producto.ParteID);
                return false;
            }

            return true;
        }

        private string DescripcionParteVenta(ProductoVenta Producto)
        {
            return (Producto.NumeroDeParte + "    " + Producto.NombreDeParte + "\n" +
                Producto.Cantidad.ToString() + " PIEZA" + (Producto.Cantidad == 1 ? "" : "S") + "        P.U. " + 
                (Producto.PrecioUnitario + Producto.Iva).ToString(GlobalClass.FormatoMoneda));
        }

        private void ActualizarListaVenta()
        {
            this.dgvProductos.DataSource = null;
            this.dgvProductos.DataSource = this.ListaVenta;

            this.dgvProductos.AutoResizeRows();
            this.CalcularTotal();
        }

        private void CalcularTotal()
        {
            decimal mTotal = 0;
            decimal mComision = 0;
            foreach (ProductoVenta Producto in this.ListaVenta)
            {
                mTotal += Producto.Importe;
                
                // Se calcula la comisión estimada
                mComision += (((Producto.PrecioUnitario - Producto.Costo) * Producto.Cantidad) * this.mPorComision);
            }

            this.lblTotal.Text = mTotal.ToString(GlobalClass.FormatoMoneda);
            this.lblComisionEstimada.Text = mComision.ToString(GlobalClass.FormatoMoneda);
        }

        private void AplicarListaDePrecios(int iLista)
        {
            if (this.ListaVenta == null) return;

            decimal mPrecio;
            foreach (var Producto in this.ListaVenta)
            {
                mPrecio = Producto.Precios[iLista - 1];
                this.AplicarPrecioProducto(Producto, mPrecio);
            }

            this.ActualizarListaVenta();
        }

        private void AplicarPrecioProducto(ProductoVenta Producto, decimal mPrecio)
        {
            Producto.PrecioUnitario = UtilLocal.ObtenerPrecioSinIva(mPrecio, 3);
            Producto.Iva = UtilLocal.ObtenerIvaDePrecio(mPrecio, 3);
            /* Producto.PrecioConIva = mPrecio;
            Producto.Descripcion = this.DescripcionParteVenta(Producto);
            Producto.Importe = (mPrecio * Producto.Cantidad);
            */

            // Se actualiza el Grid
            this.ActualizarListaVenta();
        }

        private void AplicarImporteTotal(decimal mTotal)
        {
            decimal mTotalActual = Helper.ConvertirDecimal(this.lblTotal.Text.SoloNumeric());
            decimal mIncremento = ((mTotal / mTotalActual) - 1);
            foreach (var Producto in this.ListaVenta)
                this.AplicarPrecioProducto(Producto, (Producto.PrecioConIva + (Producto.PrecioConIva * mIncremento)));
        }

        private void CambiarPrecio(ProductoVenta oParteVenta)
        {
            string sMensaje = string.Format("Lista de precios:\n\nPrecio 1: {0}\nPrecio 2: {1}\nPrecio 3: {2}\nPrecio 4: {3}\nPrecio 5: {4}\n"
                , oParteVenta.Precios[0].ToString(GlobalClass.FormatoMoneda)
                , oParteVenta.Precios[1].ToString(GlobalClass.FormatoMoneda)
                , oParteVenta.Precios[2].ToString(GlobalClass.FormatoMoneda)
                , oParteVenta.Precios[3].ToString(GlobalClass.FormatoMoneda)
                , oParteVenta.Precios[4].ToString(GlobalClass.FormatoMoneda));
            var frmPrecio = new MensajeObtenerValor(sMensaje, oParteVenta.PrecioConIva.ToString(), MensajeObtenerValor.Tipo.Decimal);
            if (frmPrecio.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                decimal mPrecio = Helper.ConvertirDecimal(frmPrecio.Valor);
                // Se valida el precio especificado, cuando aplique
                if (UtilDatos.ValidarPermiso("Ventas.Venta.EditarPreciosLibre"))
                {
                    this.AplicarPrecioProducto(oParteVenta, mPrecio);
                }
                else
                {
                    if (mPrecio > oParteVenta.Precios[0])
                        UtilLocal.MensajeAdvertencia("El Precio especificado no puede ser mayor que el Precio 1.");
                    else if (mPrecio < oParteVenta.Precios[this.Cliente.ListaDePrecios - 1])
                        UtilLocal.MensajeAdvertencia("El Precio especificado no puede ser menor que el precio asignado al Cliente.");
                    else
                        this.AplicarPrecioProducto(oParteVenta, mPrecio);
                }
            }
            frmPrecio.Dispose();
        }

        private void VerAplicacionProducto(ProductoVenta oParte)
        {
            // Se verifica si fue búsqueda por aplicación
            if (!this.chkBusquedaPorAplicacion.Checked || (this.cmbModelo.SelectedValue == null && this.cmbAnio.SelectedValue == null && this.cmbMotor.SelectedValue == null))
                return;

            // Se verifica si ya existe la parte
            var oAplicacion = this.oAplicaciones.FirstOrDefault(c => c.ParteID == oParte.ParteID);
            if (oAplicacion == null)
                oAplicacion = new VentaParteAplicacion() { ParteID = oParte.ParteID };
            // Se restauran los valores para mantener el último filtro
            oAplicacion.ModeloID = null;
            oAplicacion.Anio = null;
            oAplicacion.MotorID = null;

            // Se agrega la aplicación
            if (this.cmbModelo.SelectedValue != null)
                oAplicacion.ModeloID = Helper.ConvertirEntero(this.cmbModelo.SelectedValue);
            if (this.cmbAnio.SelectedValue != null)
                oAplicacion.Anio = Helper.ConvertirEntero(this.cmbAnio.SelectedValue);
            if (this.cmbMotor.SelectedValue != null)
                oAplicacion.MotorID = Helper.ConvertirEntero(this.cmbMotor.SelectedValue);

            this.oAplicaciones.Add(oAplicacion);
        }

        private int ObtenerParteIDDeLista(ListViewItem Elemento)
        {
            return Helper.ConvertirEntero(Elemento.SubItems[5].Text);
        }

        private void IniciarBusquedaAsincrona()
        {
            System.Threading.Thread.Sleep(Ventas.BusquedaRetrasoTecla);
            if (++this.BusquedaIntento == this.BusquedaLlamada)
                this.Invoke(new Action(this.BusquedaAvanzada));
        }

        private void BusquedaAvanzada()
        {
            // Experimental. Se restauran los valores de variables para retraso en búsqueda por descripción
            DateTime dInicio = DateTime.Now;
            this.BusquedaLlamada = this.BusquedaIntento = 0;
            //

            var Filtros = new Dictionary<string, object>();
            Filtros.Add("SucursalID", GlobalClass.SucursalID);
            List<pauVentasPartesBusqueda_Result> Partes;

            // Si es filtro por código, se ignoran los demás filtros
            if (this.txtCodigo.Text != "")
            {
                Filtros.Add("Codigo", this.txtCodigo.Text);
                Partes = General.ExecuteProcedure<pauVentasPartesBusqueda_Result>("pauVentasPartesBusqueda", Filtros);
                if (Partes.Count != 1)
                    return;
            }
            else
            {
                // Si es filtro avanzado
                if (this.txtDescripcion.Text != "")
                {
                    var Cadenas = this.txtDescripcion.Text.Split(' ');
                    for (int iCont = 0; iCont < Cadenas.Length && iCont < 9; iCont++)
                        Filtros.Add(string.Format("Descripcion{0}", iCont + 1), Cadenas[iCont]);
                }

                if (this.txtCodigoAlterno.Text.Length >= 3)
                    Filtros.Add("CodigoAlterno", this.txtCodigoAlterno.Text);

                if (this.cmbSistema.SelectedIndex >= 0)
                    Filtros.Add("SistemaID", Helper.ConvertirEntero(this.cmbSistema.SelectedValue));
                if (this.cmbLinea.SelectedIndex >= 0)
                    Filtros.Add("LineaID", Helper.ConvertirEntero(this.cmbLinea.SelectedValue));
                if (this.cmbMarcaParte.SelectedIndex >= 0)
                    Filtros.Add("MarcaID", Helper.ConvertirEntero(this.cmbMarcaParte.SelectedValue));

                /* if (this.txtCar01.Text != "")
                    Filtros.Add("Largo", this.txtCar01.Text);
                if (this.txtCar02.Text != "")
                    Filtros.Add("Alto", this.txtCar02.Text);
                if (this.txtCar03.Text != "")
                    Filtros.Add("Dientes", this.txtCar03.Text);
                if (this.txtCar04.Text != "")
                    Filtros.Add("Amperes", this.txtCar04.Text);
                if (this.txtWatts.Text != "")
                    Filtros.Add("Watts", this.txtWatts.Text);
                if (this.txtCar05.Text != "")
                    Filtros.Add("Diametro", this.txtCar05.Text);
                if (this.txtCar07.Text != "")
                    Filtros.Add("Astrias", this.txtCar07.Text);
                if (this.txtCar06.Text != "")
                    Filtros.Add("Terminales", this.txtCar06.Text);
                if (this.txtCar08.Text != "")
                    Filtros.Add("Voltios", this.txtCar08.Text);
                */

                // Se agregan los parámetros de características
                int iCaract = 0;
                foreach (Control oControl in this.flpCaracteristicas.Controls)
                {
                    if (oControl.Tag != null && oControl.Text != "")
                    {
                        // Filtros.Add(Helper.ConvertirCadena(oControl.Tag), oControl.Text);
                        Filtros.Add(string.Format("Car{0:00}ID", ++iCaract), oControl.Tag);
                        Filtros.Add(string.Format("Car{0:00}", iCaract), oControl.Text);
                    }
                }
                if (iCaract > 0)
                    Filtros.Add("Caracteristicas", true);

                // Si hay filtros en este punto, se marca para hacer la búsqueda
                bool bFiltrar = (Filtros.Count > 1);

                if (this.chkBusquedaPorAplicacion.Checked)
                {
                    if (this.cmbModelo.SelectedIndex >= 0)
                        Filtros.Add("VehiculoModeloID", Helper.ConvertirEntero(this.cmbModelo.SelectedValue));
                    if (this.cmbAnio.SelectedIndex >= 0)
                        Filtros.Add("VehiculoAnio", Helper.ConvertirEntero(this.cmbAnio.SelectedValue));
                    if (this.cmbMotor.SelectedIndex >= 0)
                        Filtros.Add("VehiculoMotorID", Helper.ConvertirEntero(this.cmbMotor.SelectedValue));
                }
                if (this.chkEquivalentes.Checked)
                    Filtros.Add("Equivalentes", true);

                // Si no hay ningún filtro, se limpian los resultados y se sale
                if (!bFiltrar)
                {
                    this.lsvPartes.Items.Clear();
                    this.lsvPartesComplementarias.Items.Clear();
                    this.lsvPartesEquivalentes.Items.Clear();
                    return;
                }

                Cargando.Mostrar();

                // Se manda hacer el filtro a un procedimiento almacenado de sql
                Partes = General.ExecuteProcedure<pauVentasPartesBusqueda_Result>("pauVentasPartesBusqueda", Filtros);
            }

            // Se llena la lista con las partes que coinciden con el filtro
            this.LlenarPartesFiltro(Partes);

            Cargando.Cerrar();
            this.lblBusquedaDuracion.Text = (DateTime.Now - dInicio).TotalSeconds.ToString();
        }

        private void LlenarPartesFiltro(List<pauVentasPartesBusqueda_Result> Partes)
        {
            // Se limpia la lista
            this.lsvPartes.Items.Clear();
            this.lsvPartesComplementarias.Items.Clear();
            this.lsvPartesEquivalentes.Items.Clear();
            this.lblMensajeBusqueda.Visible = false;

            // Si no hay resultados, se muestra un mensaje
            if (Partes.Count == 0)
            {
                this.lblMensajeBusqueda.Text = "La búsqueda especificada no regresó ningún resultado.";
                this.lblMensajeBusqueda.Visible = true;
                return;
            }
            // Si son muchos resultados, no se muestra
            /* int iMaximo = (this.lsvPartes.View == View.LargeIcon ? Ventas.MaximoElementosBusquedaImagen : Ventas.MaximoElementosBusquedaTexto);
            if (Partes.Count > iMaximo)
            {
                this.lblMensajeBusqueda.Text = MsjMuchosRes.Replace("{__NumPartes}", Partes.Count.ToString(GlobalClass.FormatoEntero));
                this.lblMensajeBusqueda.Visible = true;
                return;
            }
            */
            // Ahora el máximo ya viene desde sql, si trae un registro vacío, quiere decir que se excedió el máximo
            if (Partes.Count == 1 && Partes[0].ParteID == 0)
            {
                this.lblMensajeBusqueda.Text = "* Se encontraron más de 200 resultados. Debes ser más específico en tu búsqueda para poder mostrar los productos.";
                this.lblMensajeBusqueda.Visible = true;
                return;
            }

            string sLlaveImg;
            string sRutaImagen;
            ListViewItem oElemento;
            foreach (var Parte in Partes)
            {
                // Busca la imagen a usar, si no está en la lista de imágenes, la agrega
                sLlaveImg = Parte.ParteID.ToString();
                if (!this.ImagenesPartes.Images.ContainsKey(sLlaveImg))
                {
                    sRutaImagen = (GlobalClass.ConfiguracionGlobal.pathImagenes + sLlaveImg + "#1.jpg");
                    if (File.Exists(sRutaImagen))
                        this.ImagenesPartes.Images.Add(sLlaveImg, Image.FromFile(sRutaImagen));
                    else
                        sLlaveImg = Ventas.LlaveImgNoExiste;
                }
                oElemento = new ListViewItem(new string[] {
                    Parte.NumeroDeParte + " - " + Parte.Descripcion,
                    Parte.NumeroDeParte,
                    Parte.Descripcion,  
                    Parte.Linea,
                    Parte.Marca,
                    Parte.ParteID.ToString()
                }, sLlaveImg);
                oElemento.Tag = Parte;
                this.lsvPartes.Items.Add(oElemento);
            }
        }

        private void LimpiarBusquedaAvanzada()
        {
            this.txtCodigo.Clear();
            this.txtDescripcion.Clear();
            
            this.chkBusquedaPorAplicacion.Checked = true;
            this.cmbLinea.Text = "";
            this.cmbLinea.SelectedIndex = -1;
            this.cmbSistema.Text = "";
            this.cmbSistema.SelectedIndex = -1;
            this.cmbMarcaParte.Text = "";
            this.cmbMarcaParte.SelectedIndex = -1;
            this.txtCodigoAlterno.Clear();

            this.RestaurarCaracteristicas();

            this.lsvPartes.Items.Clear();
            this.lsvPartesComplementarias.Items.Clear();
            this.lsvPartesEquivalentes.Items.Clear();
        }

        private void MostrarComplementarios(int iParteID)
        {
            // Para mostrar las partes complementarias
            var oPartesCom = General.GetListOf<PartesComplementariasView>(c => c.ParteID == iParteID);
            this.lsvPartesComplementarias.Clear();
            string sLlaveImg, sRutaImagen;
            foreach (var Parte in oPartesCom)
            {
                // Busca la imagen a usar, si no está en la lista de imágenes, la agrega
                sLlaveImg = Parte.ParteIDComplementaria.ToString();
                if (!this.ImagenesPartes.Images.ContainsKey(sLlaveImg))
                {
                    sRutaImagen = (GlobalClass.ConfiguracionGlobal.pathImagenes + sLlaveImg + "#1.jpg");
                    if (File.Exists(sRutaImagen))
                        this.ImagenesPartes.Images.Add(sLlaveImg, Image.FromFile(sRutaImagen));
                    else
                        sLlaveImg = Ventas.LlaveImgNoExiste;
                }

                //this.lsvPartesEquivalentes.Items.Add(Parte.NumeroParte + " - " + Parte.Descripcion, sLlaveImg);
                var oElemento = new ListViewItem(new string[] {
                    Parte.NumeroDeParte + " - " + Parte.Descripcion,
                    Parte.NumeroDeParte,
                    Parte.Descripcion,
                    "",
                    "",
                    Parte.ParteIDComplementaria.ToString()
                }, sLlaveImg);
                //
                var oParteEx = General.GetListOf<ParteExistencia>(c => c.ParteID == Parte.ParteIDComplementaria && c.Estatus);
                var oParteBusqueda = new pauVentasPartesBusqueda_Result()
                {
                    ParteID = Parte.ParteIDComplementaria,
                    Existencia = oParteEx.Sum(c => c.Existencia),
                    ExistenciaLocal = oParteEx.Where(c => c.SucursalID == GlobalClass.SucursalID).Sum(c => c.Existencia)
                };
                oElemento.Tag = oParteBusqueda;
                //
                this.lsvPartesComplementarias.Items.Add(oElemento);
            }
        }

        private void MostrarEquivalentes(int iParteID)
        {
            // Para mostrar las partes equivalentes
            var PartesEq = General.GetListOf<PartesEquivalentesView>(q => q.ParteID == iParteID);
            this.lsvPartesEquivalentes.Clear();
            string sLlaveImg, sRutaImagen;
            foreach (var Parte in PartesEq)
            {
                // Busca la imagen a usar, si no está en la lista de imágenes, la agrega
                sLlaveImg = Parte.ParteIDEquivalente.ToString();
                if (!this.ImagenesPartes.Images.ContainsKey(sLlaveImg))
                {
                    sRutaImagen = (GlobalClass.ConfiguracionGlobal.pathImagenes + sLlaveImg + "#1.jpg");
                    if (File.Exists(sRutaImagen))
                        this.ImagenesPartes.Images.Add(sLlaveImg, Image.FromFile(sRutaImagen));
                    else
                        sLlaveImg = Ventas.LlaveImgNoExiste;
                }

                //this.lsvPartesEquivalentes.Items.Add(Parte.NumeroParte + " - " + Parte.Descripcion, sLlaveImg);
                var oElemento = new ListViewItem(new string[] {
                    Parte.NumeroParte + " - " + Parte.Descripcion,
                    Parte.NumeroParte,
                    Parte.Descripcion,
                    "",
                    "",
                    Parte.ParteIDEquivalente.ToString()
                }, sLlaveImg);
                //
                var oParteEx = General.GetListOf<ParteExistencia>(c => c.ParteID == Parte.ParteIDEquivalente && c.Estatus);
                var oParteBusqueda = new pauVentasPartesBusqueda_Result()
                {
                    ParteID = Parte.ParteIDEquivalente,
                    Existencia = oParteEx.Sum(c => c.Existencia),
                    ExistenciaLocal = oParteEx.Where(c => c.SucursalID == GlobalClass.SucursalID).Sum(c => c.Existencia)
                };
                oElemento.Tag = oParteBusqueda;
                //
                this.lsvPartesEquivalentes.Items.Add(oElemento);
            }
        }

        private void MostrarDatosParteSeleccionada(int iParteID)
        {
            // Se determina
            if (this.ActiveControl != this.lsvPartesComplementarias)
                this.MostrarComplementarios(iParteID);
            if (this.ActiveControl != this.lsvPartesEquivalentes)
                this.MostrarEquivalentes(iParteID);
            this.MostrarCaracteristicas(iParteID);
            this.MostrarCodigosAlternos(iParteID);
            this.MostrarAplicaciones(iParteID);

            //
            var oParte = General.GetEntity<PartesVentasView>(q => q.ParteID == iParteID);
            
            // Se evalúa si se debe cobrar el envío
            if (this.Cliente.CobroPorEnvio.Valor())
            {
                if (oParte.PrecioUno.Valor() >= this.Cliente.ImporteParaCobroPorEnvio.Valor())
                    oParte.PrecioUno += this.Cliente.ImporteCobroPorEnvio.Valor();
                if (oParte.PrecioDos.Valor() >= this.Cliente.ImporteParaCobroPorEnvio.Valor())
                    oParte.PrecioDos += this.Cliente.ImporteCobroPorEnvio.Valor();
                if (oParte.PrecioTres.Valor() >= this.Cliente.ImporteParaCobroPorEnvio.Valor())
                    oParte.PrecioTres += this.Cliente.ImporteCobroPorEnvio.Valor();
                if (oParte.PrecioCuatro.Valor() >= this.Cliente.ImporteParaCobroPorEnvio.Valor())
                    oParte.PrecioCuatro += this.Cliente.ImporteCobroPorEnvio.Valor();
                if (oParte.PrecioCinco.Valor() >= this.Cliente.ImporteParaCobroPorEnvio.Valor())
                    oParte.PrecioCinco += this.Cliente.ImporteCobroPorEnvio.Valor();
            }
            // Para mostrar los precios
            this.lblPartesPrecio1.Text = "P1   " + oParte.PrecioUno.Valor().ToString(GlobalClass.FormatoMoneda);
            this.lblPartesPrecio2.Text = "P2   " + oParte.PrecioDos.Valor().ToString(GlobalClass.FormatoMoneda);
            this.lblPartesPrecio3.Text = "P3   " + oParte.PrecioTres.Valor().ToString(GlobalClass.FormatoMoneda);
            this.lblPartesPrecio4.Text = "P4   " + oParte.PrecioCuatro.Valor().ToString(GlobalClass.FormatoMoneda);
            this.lblPartesPrecio5.Text = "P5   " + oParte.PrecioCinco.Valor().ToString(GlobalClass.FormatoMoneda);
            this.chkPartesPrecios_CheckedChanged(this, null);

            // Para mostrar las existencias por sucursal
            this.lblPartesExistSuc1.Text = oParte.ExistenciaSuc01.Valor().ToString(GlobalClass.FormatoDecimal);
            this.lblPartesExistSuc2.Text = oParte.ExistenciaSuc02.Valor().ToString(GlobalClass.FormatoDecimal);
            this.lblPartesExistSuc3.Text = oParte.ExistenciaSuc03.Valor().ToString(GlobalClass.FormatoDecimal);

            // Para la marca
            this.lblPartesMarca.Text = oParte.Marca;

            // Se actualiza el dato del último producto seleccionado
            this.ParteIDSel = iParteID;

            // Se comprueba que el precio esté actualizado
            ComprobarPrecioCaducado(iParteID);
        }

        //si un articulo no tiene mxmn y el precio tiene mas de un mes, pone una advertencia
        private void ComprobarPrecioCaducado(int iparteID)
        {
            this.ctlAdvertencia.QuitarError(this.chkPartesPrecios);
            DateTime fechaMesPasado = DateTime.Now.AddMonths(-1);
            if (General.Exists<ParteMaxMin>(c => c.ParteID == iparteID && (!c.Maximo.HasValue || c.Maximo == 0) && (!c.Minimo.HasValue || c.Minimo == 0)))
            {
                if (General.Exists<Parte>(pp => pp.ParteID == iparteID && pp.FechaRegistro < fechaMesPasado && pp.Estatus))
                {
                    this.ctlAdvertencia.PonerError(this.chkPartesPrecios, "Verificar el precio.");                    
                }
            }
        }

        private void MostrarCaracteristicas(int iParteID)
        {
            var oCaracteristicasV = General.GetListOf<PartesCaracteristicasView>(c => c.ParteID == iParteID);
            this.dgvCaracteristicas.Rows.Clear();
            foreach (var oReg in oCaracteristicasV)
                this.dgvCaracteristicas.Rows.Add(oReg.ParteCaracteristicaID, oReg.Caracteristica, oReg.Valor);
        }

        private void MostrarCodigosAlternos(int iParteID)
        { 
            var oCodigos = General.GetListOf<PartesCodigosAlternosView>(c => c.ParteID == iParteID);
            this.dgvCodigosAlternos.Rows.Clear();
            foreach (var oReg in oCodigos)
                this.dgvCodigosAlternos.Rows.Add(oReg.ParteCodigoAlternoID, oReg.MarcaAbreviacion, oReg.CodigoAlterno);
        }

        private void MostrarAplicaciones(int iParteID)
        {
            var oDatos = General.GetListOf<PartesVehiculosView>(c => c.ParteID == iParteID);
            this.dgvAplicaciones.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvAplicaciones.Rows.Add(oReg.ParteID, oReg.NombreModelo, Helper.ConvertirCadena(oReg.Anio).Derecha(2), oReg.NombreMotor);
        }

        private void ReporteDeFaltante(int iParteID)
        {
            if (iParteID <= 0)
            {
                UtilLocal.MensajeAdvertencia("No hay ningún Producto seleccionado.");
                return;
            }

            ContenedorControl frmContenedor = new ContenedorControl("Reportar Parte faltante", new ReportarParteFaltante(iParteID), new Size(400 + 6, 280 + 28));
            frmContenedor.ShowDialog();
            frmContenedor.Dispose();
        }

        private void EjecutarVenta()
        {
            bool bExito = this.VVenta.Ejecutar();

            if (bExito)
                this.LimpiarVenta();
        }

        public List<VentaDetalle> GenerarVentaDetalle()
        {
            var Lista = new List<VentaDetalle>();
            foreach (var Producto in this.ListaVenta)
            {
                Lista.Add(new VentaDetalle()
                {
                    ParteID = Producto.ParteID,
                    Costo = Producto.Costo,
                    CostoConDescuento = Producto.CostoConDescuento,
                    Cantidad = Producto.Cantidad,
                    PrecioUnitario = Producto.PrecioUnitario,
                    Iva = Producto.Iva
                });
            }

            return Lista;
        }

        private void LimpiarVenta()
        {
            // Se restaura el cliente
            this.tacCliente.ValorSel = Cat.Clientes.Mostrador;
            this.ctlError.LimpiarErrores();
            // this.cmbCliente.SelectedValue = Cat.Clientes.Mostrador;

            // Se limpian los datos del vehículo
            Control oControlAct = this.ActiveControl;
            this.cmbVehiculo.Focus();
            this.cmbVehiculo.SelectedIndex = -1;
            if (oControlAct != null)
                oControlAct.Focus();
            this.tacVechiculo.Texto = "";
            this.cmbMarca.SelectedIndex = -1;
            this.cmbModelo.SelectedIndex = -1;
            this.cmbModelo.Text = "";
            this.cmbAnio.SelectedIndex = -1;
            this.cmbMotor.SelectedIndex = -1;
            this.cmbMotor.Text = "";

            this.txtVIN.Clear();

            // Se limpia la parte del cobro
            if (this.ctlCobro != null)
            {
                this.pnlContenidoDetalle.Controls.Remove(this.ctlCobro);
                this.ctlCobro.Dispose();
                this.ctlCobro = null;
            }

            // Se limpia lo de cotización
            this.chkCotizacion.Checked = false;
            this.chkCotizacion.Enabled = true;

            // Se limpia la parte de la venta
            this.ListaVenta.Clear();
            this.ActualizarListaVenta();
            if (this.oAplicaciones != null)
                this.oAplicaciones.Clear();
        
            // Se restaura la búsqueda avanzada
            this.LimpiarBusquedaAvanzada();

            //
            this.dgvCodigosAlternos.Rows.Clear();
            this.dgvCaracteristicas.Rows.Clear();
            this.dgvAplicaciones.Rows.Clear();
        }
        
        private void AsignarDisenioOpciones()
        {
            // Se asignan eventos para imágenes a los botones de las opciones
            foreach (Control oControl in this.pnlOpciones.Controls)
            {
                if (oControl is Button)
                {
                    string sNombre = oControl.Name.Replace("btn", "");
                    var oBoton = (oControl as Button);

                    oControl.MouseEnter += new EventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject("vo" + sNombre + "Sobre") as Image);
                        this.lblDescripcionOpcion.Text = this.tltBotones.GetToolTip(oBoton);
                    });

                    oControl.MouseLeave += new EventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject("vo" + sNombre) as Image);
                        this.lblDescripcionOpcion.Text = "";
                    });

                    oControl.MouseDown += new MouseEventHandler((s, e) =>
                    {
                        oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject("vo" + sNombre + "Clic") as Image);
                    });

                    oControl.MouseUp += new MouseEventHandler((s, e) =>
                    {
                        if (oBoton.ClientRectangle.Contains(oBoton.PointToClient(Control.MousePosition)))
                            oBoton.BackgroundImage = (Properties.Resources.ResourceManager.GetObject("vo" + sNombre + "Sobre") as Image);
                    });
                }
            }
        }

        private void DibujarParteLista(DrawListViewItemEventArgs e)
        {
            // Se obtiene la parte correspondiente
            var oParte = (e.Item.Tag as pauVentasPartesBusqueda_Result);
            //
            int iLeftImg = 29;   // "Left" de la imagen en relación al "Item" // antes era 30
            int iTopImg = 2;     // "Top" de la imagen en relación al "Item"
            int iWidthImg = this.ImagenesPartes.ImageSize.Width;
            int iHeightImg = this.ImagenesPartes.ImageSize.Height;
            int iSeparacionSombra = 4;

            // Se crean los rectángulos de las partes a dibujar
            var oRecImagen = new Rectangle(e.Bounds.Left + iLeftImg, e.Bounds.Top + iTopImg, iWidthImg, iHeightImg);
            var oRecTexto = new Rectangle(e.Bounds.Left, oRecImagen.Bottom + iSeparacionSombra, e.Bounds.Width, 40);
            
            // Si está seleccionado, se muestra diferente
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, oRecTexto);
                e.Item.ForeColor = Color.White;
            }
            else
            {
                e.Item.ForeColor = e.Item.ListView.ForeColor;
            }

            // Se dibuja la imagen y el texto
            e.Graphics.DrawImage(this.ImagenesPartes.Images[e.Item.ImageKey], oRecImagen);
            e.Graphics.DrawString(e.Item.Text, e.Item.Font, new SolidBrush(e.Item.ForeColor), oRecTexto, new StringFormat() { Alignment = StringAlignment.Center });

            // Se crean los rectángulos correspondientes a las sombras            
            var RecHorizontal = new Rectangle(oRecImagen.Left + iSeparacionSombra, oRecImagen.Bottom, iWidthImg - (iSeparacionSombra / 2), iSeparacionSombra);
            var RecVertical = new Rectangle(oRecImagen.Right, oRecImagen.Top + iSeparacionSombra, iSeparacionSombra, iHeightImg);
            var RecCorreccion = new Rectangle(
                RecVertical.X - 1, RecHorizontal.Y - 1,
                iSeparacionSombra + 1, iSeparacionSombra + 1//(iSeparacionSombra / 2)
            );
            // Si la parte no tiene existencia, la sombra es roja
            var oColor = (oParte.Existencia.Valor() > 0 ? (oParte.ExistenciaLocal.Valor() > 0 ? Color.Gray : Color.CornflowerBlue) : Color.Red);
            var BrochaHorizontal = new LinearGradientBrush(RecHorizontal, oColor, Color.White, LinearGradientMode.Vertical);
            var BrochaVertical = new LinearGradientBrush(RecVertical, oColor, Color.White, LinearGradientMode.Horizontal);
            //var BrochaCorreccion = new LinearGradientBrush(RecCorreccion, oColor, Color.White, LinearGradientMode.ForwardDiagonal);
            // Se dibuja la sombra
            //e.Graphics.FillRectangle(BrochaCorreccion, RecCorreccion.X, RecCorreccion.Y, RecCorreccion.Width, RecCorreccion.Height);
            e.Graphics.FillRectangle(BrochaVertical, RecVertical);
            e.Graphics.FillRectangle(BrochaHorizontal, RecHorizontal);

            // Para cuando está seleccionado
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)), oRecImagen);
            }

            // Si la parte tiene más de una imagen, se agrega ícono
            if (this.oCantImagenes.ContainsKey(oParte.ParteID) && this.oCantImagenes[oParte.ParteID] > 1)
                e.Graphics.DrawImage(Properties.Resources.ParteMasUnaImagen, oRecImagen.Left - 12, e.Item.Bounds.Top);
        }

        private void ActualizarComboClientes()
        {
            int iClienteID = Helper.ConvertirEntero(this.tacCliente.ValorSel);
            // Se cargan los datos otra vez
            this.cmbCliente.CargarDatos("ClienteID", "Nombre", General.GetListOf<ClientesDatosView>(q => q.ListaDePrecios > 0).OrderBy(q => q.Nombre).ToList());
            this.tacCliente.CargarDatos("ClienteID", "Nombre", General.GetListOf<Cliente>().OrderBy(o => o.Nombre).ToList());
            // Se restablece el elemento previamente seleccionado
            this.tacCliente.ValorSel = iClienteID;
        }

        private void AsignarEventosCaracteristicas()
        {
            foreach (Control oControl in this.flpCaracteristicas.Controls)
            {
                if (oControl is TextoMod)
                {
                    var oTexto = (oControl as TextoMod);
                    (oControl as TextoMod).TextChanged += new EventHandler((sender, e) =>
                    {
                        if (oTexto.Focused)
                            this.BusquedaAvanzada();
                    });
                }
                else if (oControl is ComboEtiqueta)
                {
                    var oComboE = (oControl as ComboEtiqueta);
                    (oControl as ComboEtiqueta).TextChanged += new EventHandler((sender, e) =>
                    {
                        if (oComboE.Focused && oComboE.Text == "")
                            this.BusquedaAvanzada();
                    });
                    (oControl as ComboEtiqueta).SelectedIndexChanged += new EventHandler((sender, e) =>
                    {
                        if (oComboE.Focused)
                            this.BusquedaAvanzada();
                    });
                }
            }
        }

        private void RestaurarCaracteristicas()
        {
            this.flpCaracteristicas.Controls.Clear();
        }

        private void CargarCaracteristicas(int iLineaID)
        {
            // Se limpian las características actuales
            this.RestaurarCaracteristicas();

            // 
            var oLineaCarsV = General.GetListOf<LineasCaracteristicasView>(c => c.LineaID == iLineaID).OrderBy(c => c.CaracteristicaID);
            foreach (var oReg in oLineaCarsV)
            {
                if (oReg.Multiple.Valor())
                {
                    var oCombo = new ComboEtiqueta() { Etiqueta = oReg.Caracteristica, Width = 100 };
                    oCombo.Items.AddRange(oReg.MultipleOpciones.Split(','));
                    this.flpCaracteristicas.Controls.Add(oCombo);
                }
                else
                {
                    this.flpCaracteristicas.Controls.Add(new TextoMod() { Etiqueta = oReg.Caracteristica, Width = 80 });
                }
                int iCuenta = this.flpCaracteristicas.Controls.Count;
                // this.flpCaracteristicas.Controls[iCuenta - 1].Tag = ("Car" + iCuenta.ToString("00"));
                this.flpCaracteristicas.Controls[iCuenta - 1].Tag = oReg.CaracteristicaID;
            }

            // Se asignan los eventos para las características
            this.AsignarEventosCaracteristicas();
        }

        #endregion

        #region [ Públicos ]

        public bool Validar()
        {
            if (this.ListaVenta.Count <= 0)
            {
                Helper.MensajeAdvertencia("No hay ningún producto seleccionado.", GlobalClass.NombreApp);
                return false;
            }

            return true;
        }

        public void EjecutarAccesoDeTeclado(Keys eTecla)
        {
            switch (eTecla)
            {
                //case (Keys.LButton | Keys.ShiftKey | Keys.Control):
                case Keys.F4:
                    this.btnEjecutar_Click(this, null);
                    break;
                case Keys.F2:
                    // this.cmbCliente.Focus();
                    // this.ActualizarComboClientes();
                    this.tacCliente.Focus();
                    break;
                case Keys.F3:
                    this.txtCodigo.Focus();
                    break;
                case Keys.F5:
                    if (this.VOp.Opcion == VentasOpciones.eOpcion.Directorio)
                        this.VOp.oDirectorio.ctlDirBuscador.LimpiarBusqueda();
                    else
                    {
                        this.LimpiarBusquedaAvanzada();
                        this.txtDescripcion.Focus();
                    }
                    break;

                case Keys.F6:
                case Keys.F9:
                    if (this.VOp.Opcion == VentasOpciones.eOpcion.Caja)
                        this.VOp.oCaja.EjecutarAccesoDeTeclado(eTecla);
                    break;

                case (Keys.Control | Keys.NumPad1):
                case (Keys.Control | Keys.D1):
                    this.pnlSupIz.Focus();
                    break;
                case (Keys.Control | Keys.NumPad2):
                case (Keys.Control | Keys.D2):
                    this.pnlBusqueda.Focus();
                    break;
                case (Keys.Control | Keys.NumPad3):
                case (Keys.Control | Keys.D3):
                    this.pnlContenido.Focus();
                    break;
                case (Keys.Control | Keys.NumPad4):
                case (Keys.Control | Keys.D4):
                    this.pnlContenidoDetalle.Focus();
                    break;
                case (Keys.Control | Keys.NumPad5):
                case (Keys.Control | Keys.D5):
                    this.pnlOpciones.Focus();
                    break;
                case (Keys.Control | Keys.NumPad6):
                case (Keys.Control | Keys.D6):
                    this.pnlContenidoEquivalentes.Focus();
                    break;

                case (Keys.Control | Keys.L):
                    this.btnLimpiar_Click(this, null);
                    break;
                case (Keys.Control | Keys.F):
                    this.btnFacturarVentas_Click(this, null);
                    break;
                case (Keys.Control | Keys.K):
                    this.btnCancelar_Click(this, null);
                    break;
                case (Keys.Control | Keys.R):
                    this.btnReporteDeFaltante_Click(this, null);
                    break;
                case (Keys.Control | Keys.I):
                    this.btnReimpresion_Click(this, null);
                    break;
                case (Keys.Control | Keys.J):
                    this.btnCaja_Click(this, null);
                    break;
                case (Keys.Control | Keys.T):
                    this.btn9500_Click(this, null);
                    break;
                case (Keys.Control | Keys.B):
                    this.btnCobranza_Click(this, null);
                    break;
                case (Keys.Control | Keys.M):
                    this.btnGanancias_Click(this, null);
                    break;
                case (Keys.Control | Keys.G):
                    this.btnGarantia_Click(this, null);
                    break;
                case (Keys.Control | Keys.D):
                    this.btnDirectorio_Click(this, null);
                    break;
            }

        }

        public void EstablecerTextoEstado(string sTexto)
        {
            this.lblTextoEstado.Text = sTexto;
        }

        public void CargarCobranza() // para cargar cobranza desde el catalogo de clientes.
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Cobranza);
        }

        public void CargarCliente(int ClienteID)
        {
            this.cmbCliente.SelectedValue = ClienteID;
            this.Cliente = (this.cmbCliente.SelectedItem as ClientesDatosView);
            if (this.Cliente == null)
            {
                this.LimpiarDatosCliente();
                return;
            }

            // Se obtiene información sobre el crédito
            var oClienteCredito = General.GetEntity<ClientesCreditoView>(q => q.ClienteID == this.Cliente.ClienteID);

            this.lblClienteDatos.Text = (this.Cliente.Direccion + ", " + this.Cliente.Colonia + ", " + this.Cliente.Ciudad);
            this.lblClienteDatos.Text += "\n";
            this.lblClienteDatos.Text += ("RFC: " + this.Cliente.Rfc + " C.P. " + this.Cliente.CodigoPostal + " TEL. " + this.Cliente.Telefono);
            //this.lblClienteAdicional.Text = "RFC: " + this.Cliente.Rfc + " C.P. " + this.Cliente.CodigoPostal + " Tel. " + this.Cliente.Telefono;
            this.lblClienteListaDePrecios.Text = this.Cliente.ListaDePrecios.ToString();
            this.lblClienteTieneCredito.Text = (this.Cliente.TieneCredito ? "SÍ" : "NO");
            this.lblClienteTieneCredito.ForeColor = Color.SteelBlue;
            if (oClienteCredito.AdeudoVencido > 0)
            {
                this.lblClienteTieneCredito.Text += " (SUSPENDIDO)";
                this.lblClienteTieneCredito.ForeColor = Color.Red;
            }

            // Se actualizan los datos de la parte seleccionada
            this.chkPartesPrecios_CheckedChanged(this, null);

            // Se actualizan los datos de la venta
            this.AplicarListaDePrecios(this.Cliente.ListaDePrecios);
        }

        //public VentasOpciones ObtenerVentasOpciones()
        //{
        //    return this.VOp;
        //}

        public void PonerErrorCliente(string sError)
        {
            this.ctlError.PonerError(this.tacCliente, sError);
        }

        #endregion

        private void lsvPartes_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && this.lsvPartes.SelectedIndices.Count > 0)
            {
                // posición del puntero
                Point p = new Point(e.X, e.Y);
                this.ctmStripSel.Show(lsvPartes, p);
            }
        }

        private void reportarFaltanteDeExistenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ReporteDeFaltante(this.ParteIDSel);
        }

        private void buscarProveedorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Directorio);
            int IdParteSelct = 0;
            
            if (this.lsvPartes.SelectedItems.Count > 0)
            {
                IdParteSelct = this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]);
                this.VOp.oDirectorio.ctlDirectorio.MuestraTab(DirectorioVentasDetalle.eModo.Proveedores, IdParteSelct);
                this.VOp.oDirectorio.ctlDirectorio.primerSeleccionProvedor(IdParteSelct);
            }
        }

        private void mostrarLíneaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Directorio);
            int IdParteSelct = 0;

            if (this.lsvPartes.SelectedItems.Count > 0)
            {
                IdParteSelct = this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]);
                this.VOp.oDirectorio.ctlDirectorio.MuestraTab(DirectorioVentasDetalle.eModo.Lineas, IdParteSelct);
                this.VOp.oDirectorio.ctlDirectorio.BuscaProveedorConIdParte(IdParteSelct, true);
            }
        }

        private void mostrarProveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.VOp.Activar(VentasOpciones.eOpcion.Directorio);
            int IdParteSelct = 0;

            if (this.lsvPartes.SelectedItems.Count > 0)
            {
                IdParteSelct = this.ObtenerParteIDDeLista(this.lsvPartes.SelectedItems[0]);
                this.VOp.oDirectorio.ctlDirectorio.MuestraTab(DirectorioVentasDetalle.eModo.Marcas, IdParteSelct);
                this.VOp.oDirectorio.ctlDirectorio.BuscaProveedorConIdParte(IdParteSelct, true);
            }
        }

    }
}
