﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class c9500Partes : UserControl
    {
        private ControlError ctlError = new ControlError();
        private ControlError ctlAdvertencia = new ControlError() { Icon = global::Refaccionaria.App.Properties.Resources._16_Ico_Advertencia };

        public Ventas9500 o9500;
        public Dictionary<string, Cotizacion9500Detalle> Detalle = new Dictionary<string, Cotizacion9500Detalle>();
        public ClientesDatosView Cliente;
        private int ParteIDSel;
        private decimal CostoPaqueteria;
        
        public ClientesDatosView ComCliente;
        public decimal ComAnticipoSel;
        public Cotizacion9500 oCotizacion9500;
        
        public c9500Partes()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public bool AutorizacionRequeridaPrecio
        {
            get
            {
                foreach (DataGridViewRow Fila in this.dgvPartes.Rows)
                {
                    if (Fila.ErrorText != "")
                        return true;
                }
                return false;
            }
        }

        public bool AutorizacionRequeridaAnticipo { get { return (Util.Decimal(this.txtAnticipo.Text.SoloNumeric()) <= 0); } }

        public Dictionary<string, object> o9500Sel { get { return (this.dgvDatos.CurrentRow == null ? null : this.dgvDatos.CurrentRow.ADiccionario()); } }

        #endregion

        #region [ Eventos ]

        private void c9500Partes_Load(object sender, System.EventArgs e)
        {
            // this.cntError.Icon = 
            this.cmbProveedor.CargarDatos("ProveedorID", "NombreProveedor", Datos.GetListOf<Proveedor>(q => q.Estatus));

            // Buscar
            this.cmbBuscarSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
            this.cmbBuscarVendedor.CargarDatos("UsuarioID", "NombreUsuario", Datos.GetListOf<Usuario>(c => c.Activo && c.Estatus).OrderBy(c => c.NombreUsuario).ToList());
            this.dtpBuscarDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);

            // Histórico
            this.dtpHisDesde.Value = DateTime.Now.DiaPrimero();
            this.dtpHisHasta.Value = DateTime.Now.DiaUltimo();
            // Combos
            this.cmbHisSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
            this.cmbHisEstatus.CargarDatos("EstatusGenericoID", "Descripcion", Datos.GetListOf<EstatusGenerico>(c => c.EstatusGenericoID == Cat.EstatusGenericos.Completada
                || c.EstatusGenericoID == Cat.EstatusGenericos.Cancelada || c.EstatusGenericoID == Cat.EstatusGenericos.CanceladoAntesDeVender 
                || c.EstatusGenericoID == Cat.EstatusGenericos.Pendiente || c.EstatusGenericoID == Cat.EstatusGenericos.CanceladoDespuesDeVendido));
            this.cmbHisUsuario.CargarDatos("UsuarioID", "NombreUsuario", Datos.GetListOf<Usuario>(c => c.Estatus));
        }

        private void cmbProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbProveedor.Focused) return;

            ProveedorPaqueteria Paq = null;

            // Se cargan las línes - marcas, en base al proveedor seleccionado
            int iProveedorID = Util.Entero(this.cmbProveedor.SelectedValue);
            this.cmbLineaMarca.CargarDatos("LineaMarcaParteID", "LineaMarca", Datos.GetListOf<ProveedoresMarcasLineasView>(q => q.ProveedorID == iProveedorID));

            // Se muestra la paquetería
            var Proveedor = (this.cmbProveedor.SelectedItem as Proveedor);
            if (Proveedor != null)
            {
                int iPaqueteriaID = Proveedor.PaqueteriaID.Valor();
                Paq = Datos.GetEntity<ProveedorPaqueteria>(q => q.Estatus && q.ProveedorPaqueteriaID == iPaqueteriaID);

                //Se muestra la Hora maxima de pedidos
                this.lblHoraMaximaVal.Text = Proveedor.HoraMaxima.ToString();
            }

            this.chkPaqueteria.Text = (Paq == null ? "[Paquetería]" : Paq.NombrePaqueteria);
            this.CostoPaqueteria = (Paq == null ? 0.00M : Paq.CostoPaqueteria.Valor());

            // Se calculan los precios
            this.CalcularPrecios();

        }

        private void cmbLineaMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbLineaMarca.Focused) return;

            this.CalcularPrecios();
        }

        private void txtCosto_Leave(object sender, EventArgs e)
        {
            this.CalcularPrecios();
        }

        private void txtNumeroDeParte_Leave(object sender, EventArgs e)
        {
            this.FijarDescripcion(this.txtNumeroDeParte.Text);
        }
                
        private void chkPaqueteria_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCostoPaqueteria.Visible = this.chkPaqueteria.Checked;
            if (this.chkPaqueteria.Checked)
            {
                this.txtCostoPaqueteria.Text = this.CostoPaqueteria.ToString();
                this.txtCostoPaqueteria.Focus();
            }
            else
            {
                this.txtCostoPaqueteria.Clear();
            }
            this.txtCostoPaqueteria_Leave(this, null);
        }

        private void txtCostoPaqueteria_Leave(object sender, EventArgs e)
        {
            this.CalcularPrecios();
        }

        private void chkPrecioAutomatico_CheckedChanged(object sender, EventArgs e)
        {
            this.CalcularPrecio();
            this.txtPrecio.Enabled = !this.chkPrecioAutomatico.Checked;
        }

        private void txtPrecio_Leave(object sender, EventArgs e)
        {
            if (this.txtPrecio.Text == "") return;

            // Se verifica si el precio está fuera de los rangos calculados
            decimal mPrecio = Util.Decimal(this.txtPrecio.Text);
            if (mPrecio > Util.Decimal(this.lblPrecio1.Text.SoloNumeric()) || mPrecio < Util.Decimal(this.lblPrecio5.Text.SoloNumeric()))
                this.ctlAdvertencia.PonerError(this.txtPrecio, "El Precio específicado está fuera de rango. Se requerirá una autorización para proceder con el pedido."
                    , ErrorIconAlignment.MiddleLeft);
            else
                this.ctlAdvertencia.QuitarError(this.txtPrecio);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!this.ValidarParte()) return;

            // Se valida si ya existe la clave
            string sLlave = (this.txtNumeroDeParte.Text + this.txtDescripcion.Text);
            if (this.Detalle.ContainsKey(sLlave))
            {
                this.Detalle[sLlave].Cantidad += Util.Decimal(this.txtCantidad.Text);
            }
            else
            {
                var LineaMarca = (this.cmbLineaMarca.SelectedItem as ProveedoresMarcasLineasView);
                this.Detalle.Add(sLlave, new Cotizacion9500Detalle()
                {
                    ProveedorID = Util.Entero(this.cmbProveedor.SelectedValue),
                    LineaID = LineaMarca.LineaID,
                    MarcaParteID = LineaMarca.MarcaID,
                    ParteID = this.ParteIDSel,
                    Cantidad = Util.Decimal(this.txtCantidad.Text),
                    Costo = Util.Decimal(this.txtCosto.Text),
                    PrecioAlCliente = Util.Decimal(this.txtPrecio.Text)
                });
            }

            decimal mPrecio = (Util.Decimal(this.txtPrecio.Text) * Util.Decimal(this.txtCantidad.Text));
            int iFila = this.dgvPartes.Rows.Add(sLlave, this.txtNumeroDeParte.Text, this.txtCantidad.Text, this.txtDescripcion.Text, mPrecio);

            // Se agrega advertencia de autorización, si se requiere
            if (this.ctlAdvertencia.GetError(this.txtPrecio) != "")
                this.dgvPartes.Rows[iFila].ErrorText = this.ctlAdvertencia.GetError(this.txtPrecio);

            // Se calcula el anticipo
            decimal mTotal = 0.00M;
            foreach (DataGridViewRow Fila in this.dgvPartes.Rows)
                mTotal += Util.Decimal(Fila.Cells["Precio"].Value);
            this.txtAnticipo.Text = Math.Round(mTotal / 2, 0).ToString(GlobalClass.FormatoMoneda);

            // Se limpian los datos
            this.txtCosto.Clear();
            this.txtCantidad.Clear();
            this.txtNumeroDeParte.Clear();
            this.txtDescripcion.Clear();
            this.chkPaqueteria.Checked = false;
            this.chkPrecioAutomatico.Checked = false;
            this.txtPrecio.Clear();
            this.lblPrecio1.Text = "";
            this.lblPrecio2.Text = "";
            this.lblPrecio3.Text = "";
            this.lblPrecio4.Text = "";
            this.lblPrecio5.Text = "";
            this.ctlAdvertencia.QuitarError(this.txtPrecio);
            this.txtCosto.Focus();
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (this.dgvPartes.CurrentRow == null) return;

            string sLlave = Util.Cadena(this.dgvPartes.CurrentRow.Cells["Llave"].Value);

            this.Detalle.Remove(sLlave);
            this.dgvPartes.Rows.Remove(this.dgvPartes.CurrentRow);
        }

        private void dgvPartes_CurrentCellChanged(object sender, EventArgs e)
        {
            // this.btnQuitar.Enabled = (this.dgvPartes.SelectedRows.Count > 0);
        }

        private void dgvPartes_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            this.btnQuitar.Enabled = (this.dgvPartes.CurrentRow != null);
        }

        private void txtAnticipo_TextChanged(object sender, EventArgs e)
        {
            decimal mAnticipo = Util.Decimal(this.txtAnticipo.Text.SoloNumeric());
            // this.o9500.ctlCobro.Total = mAnticipo;
        }

        private void txtAnticipo_Leave(object sender, EventArgs e)
        {
            decimal mAnticipo = Util.Decimal(this.txtAnticipo.Text.SoloNumeric());
            if (mAnticipo > 0)
                this.ctlAdvertencia.QuitarError(this.txtAnticipo);
            else
                this.ctlAdvertencia.PonerError(this.txtAnticipo, "No se está dejando ningún anticipo. Se requerirá una autorización para proceder con el pedido.", true);
        }

        private void tab9500_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tab9500.SelectedTab == this.tbpNuevo)
            {
                this.o9500.CambiarOpcion(Ventas9500.eOpcion.Agregar);
            }
            else
            {
                this.o9500.CambiarOpcion(Ventas9500.eOpcion.Completar);
                this.CargarLista9500();
            }
        }

        private void cmbBuscarSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBuscarSucursal.Focused)
                this.CargarLista9500();
        }

        private void cmbBuscarVendedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBuscarVendedor.Focused)
                this.CargarLista9500();
        }

        private void dtpBuscarDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpBuscarDesde.Focused)
                this.CargarLista9500();
        }

        private void dtpBuscarHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpBuscarHasta.Focused)
                this.CargarLista9500();
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.dgvDatos.FiltrarContiene(this.txtBusqueda.Text, "lisNumeroDeParte", "lisDescripcion");
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            if (e.KeyCode == Keys.Enter)
                this.MostrarDetalle(Util.Entero(this.dgvDatos.CurrentRow.Cells["Folio"].Value));
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            int i9500ID = Util.Entero(this.dgvDatos.CurrentRow.Cells["lisCotizacion9500ID"].Value);
            /* if (e.KeyCode == Keys.Enter)
            {
                this.MostrarDetalle(i9500ID);
                e.Handled = true;
            }
            else */ if (e.KeyCode == Keys.Delete)
            {
                this.Cancelar9500(i9500ID);
            }
        }

        private void dgvDatos_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected || this.tab9500.SelectedTab.Name != "tbpBuscar") return;
            int i9500ID = Util.Entero(e.Row.Cells["lisCotizacion9500ID"].Value);
            this.MostrarDetalle(i9500ID);
        }

        private void btnHisMostrar_Click(object sender, EventArgs e)
        {
            this.CargarHistorico();
        }

        private void dgvHistorico_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgvHistorico.CurrentRow == null) return;

            if (this.dgvHistorico.VerSeleccionNueva())
            {
                int iParteID = Util.Entero(this.dgvHistorico.CurrentRow.Cells["hisParteID"].Value);
                this.ctlExistencias.CargarDatos(iParteID);
            }
        }

        #endregion

        #region [ Métodos ]

        private void FijarDescripcion(string sNumeroDeParte)
        {
            var Partes = Datos.GetListOf<PartesView>(q => q.NumeroDeParte == sNumeroDeParte);
            this.ParteIDSel = 0;
            bool bSel = false;
            if (Partes.Count > 0)  // Se tiene que mostrar un listado
            {
                var Lista = new ObtenerElementoLista("Selecciona la parte que deseas utilizar o presiona el botón \"Ninguna\" si no es alguna de las listadas."
                    , Partes);
                Lista.MostrarColumnas("Descripcion", "Marca", "Linea");
                Lista.btnAceptar.Text = "&Seleccionar";
                Lista.btnCerrar.Text = "&Ninguna";
                if (Lista.ShowDialog(Principal.Instance) == DialogResult.OK)
                {
                    // Se verifica si la parte tiene su máximo mayor a cero, lo cual indica que no puede ser 9500
                    int iParteID = Util.Entero(Lista.Sel["ParteID"]);
                    if (Datos.Exists<ParteMaxMin>(c => c.ParteID == iParteID && c.Maximo > 0))
                    {
                        UtilLocal.MensajeAdvertencia("Este artículo no puede ser 9500. Si no hay existencia, favor de reportarlo al Departamento de Compras.");
                        this.txtDescripcion.Text = "";
                        this.txtDescripcion.ReadOnly = true;
                        return;
                    }

                    this.txtDescripcion.Text = Util.Cadena(Lista.Sel["Descripcion"]);
                    this.ParteIDSel = iParteID;
                    // this.chkPaqueteria.Focus();
                    bSel = true;
                }
            }
            else
            {
                // Se sugiere el nombre
                var oProvGanV = (this.cmbLineaMarca.SelectedItem as ProveedoresMarcasLineasView);
                if (oProvGanV != null)
                    this.txtDescripcion.Text = UtilDatos.GenerarSugerenciaNombreParte(oProvGanV.LineaID, oProvGanV.MarcaID, sNumeroDeParte);
            }

            this.txtDescripcion.ReadOnly = bSel;
        }

        private void CalcularPrecio()
        {
            decimal mPrecio;
            if (this.chkPrecioAutomatico.Checked)
                mPrecio = Util.Decimal(this.tbpNuevo.Controls["lblPrecio" + this.Cliente.ListaDePrecios.ToString()].Text.SoloNumeric());
            else
                mPrecio = Util.Decimal(this.txtPrecio.Text);

            // Se agrega el valor de la papelería
            //if (this.chkPaqueteria.Checked)
            //    mPrecio += this.CostoPaqueteria;

            this.txtPrecio.Text = mPrecio.ToString();
        }

        private void CalcularPrecios()
        {
            // Se obtiene el registro correspondiente para los porcentajes
            var oParteGan = this.ObtenerParteGanancia(this.ParteIDSel);

            decimal mCosto = Util.Decimal(this.txtCosto.Text);

            if (oParteGan == null)
            {
                if (this.chkPrecioAutomatico.Checked)
                    this.txtPrecio.Text = "";
                this.lblPrecio1.Text = "";
                this.lblPrecio2.Text = "";
                this.lblPrecio3.Text = "";
                this.lblPrecio4.Text = "";
                this.lblPrecio5.Text = "";
            }
            else
            {
                decimal[] Precios = new decimal[] {
                    UtilTheos.AplicarRedondeo(mCosto * oParteGan.PorcentajeDeGanancia1)
                    , UtilTheos.AplicarRedondeo(mCosto * oParteGan.PorcentajeDeGanancia2)
                    , UtilTheos.AplicarRedondeo(mCosto * oParteGan.PorcentajeDeGanancia3)
                    , UtilTheos.AplicarRedondeo(mCosto * oParteGan.PorcentajeDeGanancia4)
                    , UtilTheos.AplicarRedondeo(mCosto * oParteGan.PorcentajeDeGanancia5)
                };

                // Se contempla el costo de la paquetería
                if (this.chkPaqueteria.Checked)
                {
                    decimal mPaqueteria = Util.Decimal(this.txtCostoPaqueteria.Text);
                    for (int iCont = 0; iCont < Precios.Length; iCont++)
                        Precios[iCont] += mPaqueteria;
                }

                // Se asignan los precios
                this.lblPrecio1.Text = Precios[0].ToString(GlobalClass.FormatoMoneda);
                this.lblPrecio2.Text = Precios[1].ToString(GlobalClass.FormatoMoneda);
                this.lblPrecio3.Text = Precios[2].ToString(GlobalClass.FormatoMoneda);
                this.lblPrecio4.Text = Precios[3].ToString(GlobalClass.FormatoMoneda);
                this.lblPrecio5.Text = Precios[4].ToString(GlobalClass.FormatoMoneda);

                // Se asigna el precio automático, si aplica
                if (this.chkPrecioAutomatico.Checked)
                    this.txtPrecio.Text = Precios[this.Cliente.ListaDePrecios - 1].ToString();
            }
        }

        private bool ValidarParte()
        {
            this.ctlError.LimpiarErrores();
            if (this.cmbProveedor.SelectedValue == null)
                this.ErrorAgregar();
            if (this.cmbLineaMarca.SelectedValue == null)
                this.ErrorAgregar();
            if (!Util.ValidarDecimal(this.txtCosto.Text))
                this.ErrorAgregar();
            if (!Util.ValidarDecimal(this.txtCantidad.Text))
                this.ErrorAgregar();
            if (this.txtNumeroDeParte.Text == "")
                this.ErrorAgregar();
            if (this.txtDescripcion.Text == "")
                this.ErrorAgregar();
            if (!Util.ValidarDecimal(this.txtPrecio.Text))
                this.ErrorAgregar();

            return (this.ctlError.NumeroDeErrores <= 0);
        }

        private void ErrorAgregar()
        {
            this.ctlError.PonerError(this.btnAgregar, "Debes llenar todos los datos.", ErrorIconAlignment.MiddleLeft);
        }

        private void CargarLista9500()
        {
            Cargando.Mostrar();

            int iSucursalID = Util.Entero(this.cmbBuscarSucursal.SelectedValue);
            int iVendedorID = Util.Entero(this.cmbBuscarVendedor.SelectedValue);
            DateTime dDesde = this.dtpBuscarDesde.Value.Date;
            DateTime dHasta = this.dtpBuscarHasta.Value.Date.AddDays(1);

            var oDatos = Datos.GetListOf<Lista9500View>(c => c.EstatusGenericoID == Cat.EstatusGenericos.Pendiente
                && (iSucursalID == 0 || c.SucursalID == iSucursalID)
                && (iVendedorID == 0 || c.VendedorID == iVendedorID)
                && (c.Fecha >= dDesde && c.Fecha < dHasta)
            ).OrderBy(c => c.Cotizacion9500ID);
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvDatos.Rows.Add(oReg.Cotizacion9500ID, oReg.Fecha, oReg.Vendedor, oReg.NumeroDeParte, oReg.Descripcion
                    , oReg.Costo, oReg.PrecioAlCliente, oReg.Anticipo, oReg.Cliente, oReg.Sucursal, oReg.Proveedor, oReg.LineaMarca);

            if (this.txtBusqueda.Text != "")
                this.txtBusqueda_TextChanged(this, null);

            Cargando.Cerrar();
        }

        private void MostrarDetalle(int iCotizacion9500ID)
        {
            this.o9500.ctlComDetalle.LimpiarDetalle();
            ProductoVenta oProductoV;
            var Detalle = Datos.GetListOf<Cotizaciones9500DetalleView>(q => q.Cotizacion9500ID == iCotizacion9500ID);
            foreach (var Producto in Detalle)
            {
                oProductoV = new ProductoVenta()
                {
                    ParteID = Producto.ParteID,
                    NumeroDeParte = Producto.NumeroParte,
                    NombreDeParte = Producto.NombreParte,
                    Cantidad = Producto.Cantidad,
                    PrecioUnitario = UtilTheos.ObtenerPrecioSinIva(Producto.PrecioAlCliente, 3),
                    Iva = UtilTheos.ObtenerIvaDePrecio(Producto.PrecioAlCliente, 3)
                    // PrecioConIva = Producto.PrecioAlCliente
                };
                this.o9500.ctlComDetalle.AgregarProducto(oProductoV);
            }
            this.o9500.ctlComDetalle.VerExistenciaLista();

            // Se actualiza el Cliente
            this.oCotizacion9500 = Datos.GetEntity<Cotizacion9500>(q => q.Cotizacion9500ID == iCotizacion9500ID && q.Estatus);
            this.ComCliente = Datos.GetEntity<ClientesDatosView>(q => q.ClienteID == oCotizacion9500.ClienteID);
            this.o9500.ClienteCompletar(this.ComCliente);
            // Se guarda el anticipo
            this.ComAnticipoSel = oCotizacion9500.Anticipo;
        }

        private void Cancelar9500(int i9500ID)
        {
            var o9500 = Datos.GetEntity<Cotizacion9500>(q => q.Cotizacion9500ID == i9500ID && q.Estatus);
            
            // Se valida que ya se haya cobrado la venta del anticipo
            var oVenta = Datos.GetEntity<Venta>(q => q.VentaID == o9500.AnticipoVentaID && q.Estatus);
            if (oVenta.VentaEstatusID == Cat.VentasEstatus.Realizada)
            {
                UtilLocal.MensajeAdvertencia("El 9500 seleccionado no ha sido cobrado. Para cancelarlo, cancela la Venta del anticipo desde Ventas por Cobrar.");
                return;
            }

            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas cancelar el 9500 seleccionado?") == DialogResult.Yes)
            {
                var oMotivo = UtilLocal.ObtenerValor("¿Cuál es el motivo de la baja?", "", MensajeObtenerValor.Tipo.TextoLargo);
                if (oMotivo == null) return;
                var oResU = UtilLocal.ValidarObtenerUsuario();
                if (oResU.Error) return;
                
                Cargando.Mostrar();
                // Se cancela el 9500
                VentasProc.Cancelar9500(i9500ID, Util.Cadena(oMotivo), oResU.Respuesta.UsuarioID);
                Cargando.Cerrar();
                this.CargarLista9500();
            }
        }
        
        private void CargarHistorico()
        {
            Cargando.Mostrar();
            DateTime dDesde = this.dtpHisDesde.Value.Date;
            DateTime dHasta = this.dtpHisHasta.Value.Date.AddDays(1);
            int iSucursalID = Util.Entero(this.cmbHisSucursal.SelectedValue);
            int iEstatusID = Util.Entero(this.cmbHisEstatus.SelectedValue);
            int iUsuarioID = Util.Entero(this.cmbHisUsuario.SelectedValue);
            var oDatos = Datos.GetListOf<Lista9500View>(c => 
                (c.Fecha >= dDesde && c.Fecha < dHasta)
                && (iSucursalID == 0 || c.SucursalID == iSucursalID)
                && (iEstatusID == 0 || c.EstatusGenericoID == iEstatusID)
                && (iUsuarioID == 0 || c.VendedorID == iUsuarioID)
            ).OrderBy(c => c.Cotizacion9500ID);
            this.dgvHistorico.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvHistorico.Rows.Add(oReg.ParteID, oReg.Cotizacion9500ID, oReg.Fecha, oReg.NumeroDeParte, oReg.Descripcion, oReg.Anticipo, oReg.Sucursal
                    , oReg.Estatus, oReg.Cliente, oReg.Vendedor, oReg.BajaMotivo, oReg.BajaUsuario);
            Cargando.Cerrar();
        }

        #endregion

        #region [ Públicos ]
                
        public void CambiarCliente(ClientesDatosView oCliente)
        {
            if (this.o9500.Opcion == Ventas9500.eOpcion.Agregar)
            {
                this.Cliente = oCliente;

                // Se recalcula el precio, si aplica
                if (this.chkPrecioAutomatico.Checked)
                    this.CalcularPrecio();

                // Se actualizan los datos del cliente
                if (this.Cliente == null || this.Cliente.ClienteID == Cat.Clientes.Mostrador)
                {
                    this.txtCliente.Clear();
                    this.txtCelular.Clear();
                }
                else
                {
                    this.txtCliente.Text = this.Cliente.Nombre;
                    this.txtCelular.Text = this.Cliente.Celular;
                }
            }
        }

        public bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (this.Detalle.Count <= 0)
                this.ctlError.PonerError(this.lblPosicionErrorProductos, "Debes especificar al menos un Producto.");
            if (this.txtCliente.Text == "" || this.txtCelular.Text == "")
                this.ctlError.PonerError(this.lblPosicionErrorProductos, "Debes especificar un Cliente y su celular.");
            this.txtAnticipo_Leave(this, null);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        public Cotizacion9500 Generar9500()
        {
            var o9500 = new Cotizacion9500()
            {
                EstatusGenericoID = Cat.EstatusGenericos.Pendiente,
                Fecha = DateTime.Now,
                ClienteID = this.Cliente.ClienteID,
                SucursalID = GlobalClass.SucursalID,
                Anticipo = Util.Decimal(this.txtAnticipo.Text.SoloNumeric())
            };
            return o9500;
        }

        public List<Cotizacion9500Detalle> Generar9500Detalle()
        {
            List<Cotizacion9500Detalle> o9500Detalle = new List<Cotizacion9500Detalle>();
            foreach (var Parte in this.Detalle)
            {
                // Se agrega la parte a la lista de detalle de 9500
                o9500Detalle.Add(Parte.Value);
            }

            return o9500Detalle;
        }

        public ProveedorParteGanancia ObtenerParteGanancia(int? iParteID)
        {
            if (iParteID <= 0)
                iParteID = null;
            int iProveedorID = Util.Entero(this.cmbProveedor.SelectedValue);
            var oMarcaLinea = (this.cmbLineaMarca.SelectedItem as ProveedoresMarcasLineasView);
            if (this.ParteIDSel > 0)
                iParteID = this.ParteIDSel;
            if (oMarcaLinea == null)
                return AdmonProc.ObtenerParteDescuentoGanancia(iProveedorID, null, null, iParteID);
            else
                return AdmonProc.ObtenerParteDescuentoGanancia(iProveedorID, oMarcaLinea.MarcaID, oMarcaLinea.LineaID, iParteID);
        }

        #endregion
                                
    }
}