using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class Detalle9500Com : cVentaDetalleMod
    {
        public Detalle9500Com()
        {
            InitializeComponent();
            this.dgvProductos.KeyDown += new KeyEventHandler(dgvProductos_KeyDown);
        }

        #region [ Eventos ]

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
                    var frmCantidad = new MensajeObtenerValor("Indica la cantidad que deseas aplicar.", (ProdCantidad.Cantidad + 1).ToString(), MensajeObtenerValor.Tipo.Entero);
                    if (frmCantidad.ShowDialog(Principal.Instance) == DialogResult.OK)
                        this.ModificarCantidad(this.dgvProductos.SelectedRows[0], Util.Entero(frmCantidad.Valor));
                    frmCantidad.Dispose();
                    e.Handled = true;
                    break;
            }
        }

        #endregion

        #region [ Métodos ]

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

            // No se permite dejar en ceros la cantidad
            if (mCantidad <= 0) return;

            // Se valida la existencia
            this.ValidarExistencia(Fila, mCantidad);
            //if (!this.ValidarExistencia(Fila, iCantidad))
            //    return;

            Producto.Cantidad = mCantidad;
            this.dgvProductos.Refresh();
            this.CalcularTotal();
        }

        private bool ValidarExistencia(DataGridViewRow Fila, decimal mCantidad)
        {
            var oProductoV = (Fila.DataBoundItem as ProductoVenta);
            var ParteV = Datos.GetEntity<PartesVentasView>(q => q.ParteID == oProductoV.ParteID);
            if (ParteV == null)
                return false;

            // Se llenan datos necesarios para la validación
            oProductoV.EsServicio = ParteV.EsServicio.Valor();
            oProductoV.Existencias = new decimal[] { ParteV.ExistenciaSuc01.Valor(), ParteV.ExistenciaSuc02.Valor(), ParteV.ExistenciaSuc03.Valor() };
            // Se hace la validación
            bool bExistencia = false;
            if (oProductoV.EsServicio || mCantidad <= oProductoV.Existencias[GlobalClass.SucursalID - 1])
            {
                Fila.DefaultCellStyle.ForeColor = Color.SteelBlue;
                Fila.DefaultCellStyle.SelectionForeColor = Color.SteelBlue;
                bExistencia = true;
            }
            else
            {
                Fila.DefaultCellStyle.ForeColor = Color.Red;
                Fila.DefaultCellStyle.SelectionForeColor = Color.Red;
            }

            return bExistencia;
        }

        #endregion

        #region [ Públicos ]

        public override void AgregarProducto(ProductoVenta Producto)
        {
            base.AgregarProducto(Producto);

            // Se marcan todas las filas como seleccionadas
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
                Fila.Cells["Aplicar"].Value = true;
            this.CalcularTotal();
        }

        public override void LimpiarDetalle()
        {
            base.LimpiarDetalle();
            this.ctlError.LimpiarErrores();
        }

        public bool VerExistenciaLista()
        {
            bool bExistencia = true;
            ProductoVenta Producto;
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
            {
                if (!Util.Logico(Fila.Cells["Aplicar"].Value)) continue;

                Producto = (Fila.DataBoundItem as ProductoVenta);
                // Se valida la existencia
                if (!this.ValidarExistencia(Fila, Producto.Cantidad))
                    bExistencia = false;
            }

            return bExistencia;
        }

        public void QuitarProductoVenta(DataGridViewRow Fila)
        {
            base.oListaVenta.Remove(Fila.DataBoundItem as ProductoVenta);
            base.ActualizarListaVenta();
            this.VerExistenciaLista();
        }

        public bool Validar()
        {
            this.ctlError.LimpiarErrores();

            // Se valida que hayan producto con check
            bool bMarcado = false;
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
            {
                if (Util.Logico(Fila.Cells["Aplicar"].Value))
                {
                    bMarcado = true;
                    break;
                }

            }
            if (!bMarcado)
                this.ctlError.PonerError(this.lblEtTotal, "No hay ningún producto seleccionado.", ErrorIconAlignment.MiddleLeft);
            // Se valida la existencia
            if (!this.VerExistenciaLista())
                this.ctlError.PonerError(this.lblEtTotal, "No hay existencia suficiente.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
    }
}
