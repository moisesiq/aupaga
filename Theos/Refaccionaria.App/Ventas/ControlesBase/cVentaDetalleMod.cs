using System;
using System.Windows.Forms;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class cVentaDetalleMod : cVentaDetalle
    {
        protected ControlError ctlError = new ControlError();

        public cVentaDetalleMod()
        {
            InitializeComponent();
            
            // Se quita el ReadOnly
            this.dgvProductos.ReadOnly = false;
            foreach (DataGridViewColumn oCol in this.dgvProductos.Columns)
                oCol.ReadOnly = true;

            // Se ajusta el grid con el checkbox
            this.dgvProductos.Columns.Add(new DataGridViewCheckBoxColumn() {
                Name = "Aplicar",
                DisplayIndex = 0,
                Width = 20,
            });
            this.dgvProductos.Columns["Descripcion"].Width -= 20;

            // Se agrega evento para recalcular los totales, cuando se marque / desmarque algún producto
            this.dgvProductos.CellValueChanged += new DataGridViewCellEventHandler(dgvProductos_CellValueChanged);
            // Se agrega un evento para cuando se seleccione el CheckBox
            this.dgvProductos.CurrentCellDirtyStateChanged += new System.EventHandler(dgvProductos_CurrentCellDirtyStateChanged);
            // Se agrega un evento para darle marcar el checkbox con la tecla Shift
            this.dgvProductos.KeyDown += new KeyEventHandler(dgvProductos_KeyDown);
        }

        private void dgvProductos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvProductos.CurrentRow.Cells[e.ColumnIndex].OwningColumn.Name == "Aplicar")
            {
                this.FilaMarcaCambiada(this.dgvProductos.CurrentRow);
            }
        }

        private void dgvProductos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!this.dgvProductos.IsCurrentCellDirty) return;
            if (this.dgvProductos.CurrentCell.OwningColumn.Name == "Aplicar")
            {
                this.dgvProductos.EndEdit();
            }
        }

        private void dgvProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvProductos.CurrentRow == null) return;

            if (e.KeyCode == Keys.ShiftKey)
            {
                // Se finaliza el modo edición de la celda, para que pueda aplicar el cambio de valor
                if (this.dgvProductos.CurrentCell.OwningColumn.Name == "Aplicar")
                    this.dgvProductos.EndEdit();

                this.dgvProductos.CurrentRow.Cells["Aplicar"].Value = !Util.Logico(this.dgvProductos.CurrentRow.Cells["Aplicar"].Value);
            }
        }

        protected virtual void FilaMarcaCambiada(DataGridViewRow Fila)
        {
            this.CalcularTotal();
        }

        protected override void CalcularTotal()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
            {
                if (!Util.Logico(Fila.Cells["Aplicar"].Value)) continue;
                mTotal += Util.Decimal(Fila.Cells["Importe"].Value);
            }

            this.lblTotal.Text = mTotal.ToString(GlobalClass.FormatoMoneda);
        }
                
        public List<ProductoVenta> ProductosSel()
        {
            var Lista = new List<ProductoVenta>();
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
            {
                if (!Util.Logico(Fila.Cells["Aplicar"].Value)) continue;
                Lista.Add(Fila.DataBoundItem as ProductoVenta);
            }
            return Lista;
        }
    }
}
