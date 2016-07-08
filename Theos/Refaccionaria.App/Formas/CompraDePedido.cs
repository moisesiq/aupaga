using System;
using System.Windows.Forms;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CompraDePedido : Form
    {
        public List<object> Seleccion;

        public CompraDePedido(int iPedidoID)
        {
            InitializeComponent();

            this.CargarPedido(iPedidoID);
        }

        #region [ Eventos ]

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.dgvDatos.FiltrarContiene(this.txtBusqueda.Text, "NumeroDeParte", "Descripcion");
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentCell == null) return;

            switch (e.KeyCode)
            {
                case Keys.Space:
                    this.dgvDatos.CurrentRow.Cells["Sel"].Value = !Util.Logico(this.dgvDatos.CurrentRow.Cells["Sel"].Value);
                    break;
            }
        }

        private void dgvDatos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDatos.Columns[e.ColumnIndex].Name == "Costo")
            {
                this.dgvDatos.CurrentCell = this.dgvDatos["Cantidad", e.RowIndex];
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.AccionAceptar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
                
        private void btnCerrrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void CargarPedido(int iPedidoID)
        {
            var oPedido = Datos.GetListOf<PedidosDetalleView>(c => c.PedidoID == iPedidoID && c.PedidoEstatusID == Cat.PedidosEstatus.NoSurtido);
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oPedido)
            {
                var oPartePrecio = Datos.GetEntity<PartePrecio>(c => c.ParteID == oReg.ParteID && c.Estatus);
                int iFila = this.dgvDatos.Rows.Add(oReg.ParteID, false, oReg.NumeroParte, oReg.NombreParte, oPartePrecio.Costo, oReg.CantidadPedido);
                this.dgvDatos.Rows[iFila].Tag = oReg;
            }
        }

        private bool AccionAceptar()
        {
            this.Seleccion = new List<object>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (Util.Logico(oFila.Cells["Sel"].Value))
                {
                    var oReg = (oFila.Tag as PedidosDetalleView);
                    oReg.CantidadPedido = Util.Decimal(oFila.Cells["Cantidad"].Value);
                    oReg.CostosUnitario = Util.Decimal(oFila.Cells["Costo"].Value);
                    this.Seleccion.Add(oReg);
                }
            }
            
            return true;
        }

        #endregion
                                                                        
    }
}
