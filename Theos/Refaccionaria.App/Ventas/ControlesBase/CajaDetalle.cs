using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CajaDetalle : UserControl
    {
        public CajaDetalle()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CajaDetalle_Load(object sender, System.EventArgs e)
        {
            this.dgvDetalle.ClearSelection();
        }

        private void dgvDetalle_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dgvDetalle_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;
            foreach (DataGridViewCell Celda in e.Row.Cells)
                Celda.Style.SelectionForeColor = Celda.InheritedStyle.ForeColor;
        }

        private void dgvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;
            if (this.dgvDetalle.CurrentRow.Cells[e.ColumnIndex].OwningColumn.Name == "VistoBueno")
            {
                this.FilaVistoBuenoCambiado(this.dgvDetalle.CurrentRow, Util.Logico(this.dgvDetalle.CurrentRow.Cells["VistoBueno"].Value));
            }
        }

        private void dgvDetalle_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!this.dgvDetalle.IsCurrentCellDirty) return;
            if (this.dgvDetalle.CurrentCell.OwningColumn.Name == "VistoBueno")
            {
                this.dgvDetalle.EndEdit();
            }
        }

        private void dgvDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;
            if (e.KeyCode == Keys.ShiftKey)
            {
                // Se verifica si la celda es de tipo checkbox
                if (!(this.dgvDetalle.CurrentRow.Cells["VistoBueno"] is DataGridViewCheckBoxCell)) return;

                // Se finaliza el modo edición de la celda, para que pueda aplicar el cambio de valor
                if (this.dgvDetalle.CurrentCell.OwningColumn.Name == "VistoBueno")
                    this.dgvDetalle.EndEdit();

                this.dgvDetalle.CurrentRow.Cells["VistoBueno"].Value = !Util.Logico(this.dgvDetalle.CurrentRow.Cells["VistoBueno"].Value);
            }
        }
        
        #endregion

        #region [ Métodos ]

        public virtual void FilaVistoBuenoCambiado(DataGridViewRow Fila, bool bVistoBueno)
        {

        }

        public virtual int AgregarLineaTitulo(string sTexto)
        {
            sTexto = sTexto.ToUpper();
            int iFila = this.dgvDetalle.Rows.Add("Titulo", sTexto, null);
            this.dgvDetalle.Rows[iFila].DefaultCellStyle.Font = new Font(this.dgvDetalle.DefaultCellStyle.Font.FontFamily, (float)10, FontStyle.Bold);
            this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.CadetBlue;
            this.dgvDetalle.Rows[iFila].Cells["VistoBueno"] = new DataGridViewTextBoxCell();
            this.dgvDetalle.Rows[iFila].Cells["VistoBueno"].ReadOnly = true;
            return iFila;
        }

        public virtual int AgregarLineaEncabezado(string sTexto)
        {
            sTexto = sTexto.ToUpper();
            int iFila = this.dgvDetalle.Rows.Add("Encabezado", sTexto, null);
            this.dgvDetalle.Rows[iFila].DefaultCellStyle.Font = new Font(this.dgvDetalle.DefaultCellStyle.Font, FontStyle.Bold);
            this.dgvDetalle.Rows[iFila].Cells["VistoBueno"] = new DataGridViewTextBoxCell();
            this.dgvDetalle.Rows[iFila].Cells["VistoBueno"].ReadOnly = true;
            return iFila;
        }

        public virtual int AgregarLineaDetalle(string sTexto, bool bMostrarCheck, string sTextoCheck, string sTabla, int iTablaRegistroID)
        {
            sTexto = sTexto.ToUpper();
            int iFila = this.dgvDetalle.Rows.Add("Detalle", sTexto, null, sTextoCheck, sTabla, iTablaRegistroID);
            this.dgvDetalle.Rows[iFila].Height = 17;
            if (bMostrarCheck)
            {
                this.dgvDetalle.Rows[iFila].Cells["VistoBueno"].Value = (sTextoCheck != "");
            } else {
                this.dgvDetalle.Rows[iFila].Cells["VistoBueno"] = new DataGridViewTextBoxCell();
                this.dgvDetalle.Rows[iFila].Cells["VistoBueno"].ReadOnly = true;
            }
            return iFila;
        }

        public virtual int AgregarLineaEspacio()
        {
            int iFila = this.dgvDetalle.Rows.Add("Espacio");
            this.dgvDetalle.Rows[iFila].Height = 14;
            this.dgvDetalle.Rows[iFila].Cells["VistoBueno"] = new DataGridViewTextBoxCell();
            this.dgvDetalle.Rows[iFila].Cells["VistoBueno"].ReadOnly = true;
            return iFila;
        }

        protected virtual string VentaFormasDePago(int iVentaID)
        {
            string sCadena = "1-2-3-4-5";
            var oPagos = Datos.GetListOf<VentasPagosDetalleView>(q => q.VentaID == iVentaID);
            foreach (var oPago in oPagos)
            {
                // Si es pago negativo (devolución), no se cuenta
                if (oPago.Importe <= 0) continue;

                switch (oPago.FormaDePagoID)
                {
                    case Cat.FormasDePago.Efectivo:
                        sCadena = sCadena.Replace("1", "EF");
                        break;
                    case Cat.FormasDePago.Cheque:
                        sCadena = sCadena.Replace("2", "CH");
                        break;
                    case Cat.FormasDePago.Tarjeta:
                        sCadena = sCadena.Replace("3", "TC");
                        break;
                    case Cat.FormasDePago.Transferencia:
                        sCadena = sCadena.Replace("4", "TR");
                        break;
                    case Cat.FormasDePago.Vale:
                        sCadena = sCadena.Replace("5", "NC");
                        break;
                }
            }

            return Regex.Replace(sCadena, @"\d", "  ");
        }

        #endregion

                
    }
}