using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class cVentaDetalle : UserControl
    {
        protected BindingList<ProductoVenta> oListaVenta = new BindingList<ProductoVenta>();
        
        public cVentaDetalle()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public decimal Total { get { return Helper.ConvertirDecimal(this.lblTotal.Text.SoloNumeric()); } }
                
        #endregion

        #region [ Eventos ]

        private void cVentaDetalle_Load(object sender, EventArgs e)
        {
            this.dgvProductos.AutoGenerateColumns = false;
        }

        private void dgvProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvProductos.CurrentRow == null) return;

            var oFila = this.dgvProductos.CurrentRow;

            switch (e.KeyCode)
            {
                case Keys.Divide:
                    this.EditarDescripcion(oFila);
                    break;
            }
        }
                
        private void dgvProductos_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;
            
            // Si está seleccionada, se ponen las negritas, si no, se quitan
            e.Row.Cells["Descripcion"].Style.Font = new Font(this.dgvProductos.Font, (e.Row.Selected ? FontStyle.Bold : FontStyle.Regular));
        }

        #endregion

        #region [ Métodos ]

        protected void ActualizarListaVenta()
        {
            this.dgvProductos.DataSource = null;
            this.dgvProductos.DataSource = this.oListaVenta;

            this.dgvProductos.AutoResizeRows();
            this.CalcularTotal();
        }

        private void EditarDescripcion(DataGridViewRow oFila)
        {
            if (UtilDatos.ValidarPermiso("Ventas.Ticket.EditarDescripcionPartes"))
            {
                var oProd = (oFila.DataBoundItem as ProductoVenta);
                var oValor = UtilLocal.ObtenerValor("Indica la Descripción a utilizar:", oProd.NombreDeParte, MensajeObtenerValor.Tipo.TextoLargo);
                if (oValor != null)
                {
                    oProd.NombreDeParte = Helper.ConvertirCadena(oValor);
                    oFila.Cells["Descripcion"].Value = oValor;
                }
            }
        }

        protected virtual void CalcularTotal()
        {
            decimal mTotal = 0;
            foreach (ProductoVenta Producto in this.oListaVenta)
                mTotal += Producto.Importe;

            this.lblTotal.Text = mTotal.ToString(GlobalClass.FormatoMoneda);
        }
        
        #endregion

        #region [ Públicos ]

        public virtual void AgregarProducto(ProductoVenta Producto)
        {
            // Se agrega a la lista
            this.oListaVenta.Add(Producto);

            // Se actualiza el Grid
            this.ActualizarListaVenta();

            // Se quitan las negritas de todas las filas, no aparecer seleccionado al cargar datos
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
                Fila.Cells["Descripcion"].Style.Font = new Font(this.dgvProductos.Font, FontStyle.Regular);
        }
        
        public virtual void LimpiarDetalle()
        {
            this.oListaVenta.Clear();
            this.ActualizarListaVenta();
        }

        public virtual void LlenarDetalle(int iVentaID)
        {
            var oDetalle = General.GetListOf<VentasDetalleView>(q => q.VentaID == iVentaID);
            // Se limpia el grid de detalle
            this.LimpiarDetalle();
            // Se agrega el detalle
            foreach (var oProducto in oDetalle)
            {
                this.AgregarProducto(new ProductoVenta()
                {
                    ParteID = oProducto.ParteID,
                    NumeroDeParte = oProducto.NumeroParte,
                    NombreDeParte = oProducto.NombreParte,
                    Costo = oProducto.Costo,
                    CostoConDescuento = oProducto.CostoConDescuento,
                    Cantidad = oProducto.Cantidad,
                    PrecioUnitario = oProducto.PrecioUnitario,
                    Iva = oProducto.Iva,
                    UnidadDeMedida = oProducto.Medida
                });
            }
        }

        public virtual List<ProductoVenta> ObtenerListaVenta()
        {
            var oLista = new List<ProductoVenta>();
            foreach (var oArt in this.oListaVenta)
                oLista.Add(oArt);
            return oLista;
        }

        #endregion
                
    }
}
