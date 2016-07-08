using System;
using System.Windows.Forms;
using System.Data.Objects;
using System.Linq;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CajaFacturaGlobal : UserControl
    {

        public CajaFacturaGlobal()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public string ConceptoVentas { get { return this.txtConcepto.Text; } }
        public decimal TotalVentas { get; private set; }
        public decimal TotalCancelaciones { get; private set; }
        public decimal TotalDevoluciones { get; private set; }

        #endregion

        #region [ Eventos ]

        private void FacturaGlobal_Load(object sender, EventArgs e)
        {
            // Se llenan los datos de los pagos
            DateTime dHoy = DateTime.Today;
            var oPagosDet = General.GetListOf<VentasPagosDetalleView>(q => q.SucursalID == GlobalClass.SucursalID 
                && EntityFunctions.TruncateTime(q.Fecha) == dHoy && !q.Facturada).OrderBy(q => q.VentaPagoID);
            int iVentaPagoID = 0, iFila = 0;
            string sTipoMov = "", sFolioInicial = "", sFolioFinal = "";
            decimal mEfectivo, mCheque, mTarjeta, mTransferencia, mNotaDeCredito;
            mEfectivo = mCheque = mTarjeta = mTransferencia = mNotaDeCredito = 0;
            // Se llena el detalle
            foreach (var oPagoDet in oPagosDet)
            {
                if (oPagoDet.VentaPagoID != iVentaPagoID)
                {
                    // Si es nota de crédito negativa, se ignora
                    if (oPagoDet.FormaDePagoID == Cat.FormasDePago.Vale && oPagoDet.Importe < 0)
                        continue;

                    // Se obtiene el dato de qué tipo de movimiento es (Venta, Cancelación, Devolución)
                    if (oPagoDet.Importe < 0)
                        sTipoMov = ((oPagoDet.VentaEstatusID == Cat.VentasEstatus.Cancelada || oPagoDet.VentaEstatusID == Cat.VentasEstatus.CanceladaSinPago) 
                            ? "Cancelacion" : "Devolucion");
                    else
                        sTipoMov = "Venta";

                    // iFila = this.dgvPagos.Rows.Add(sTipoMov, oPagoDet.VentaID, true, oPagoDet.VentaFolio, oPagoDet.Cliente);
                    iFila = this.dgvPagos.Rows.Add(sTipoMov, oPagoDet.VentaPagoID, true, oPagoDet.VentaFolio, oPagoDet.Cliente);

                    // Para determinar folio inicial y final
                    sFolioInicial = (sFolioInicial == "" || oPagoDet.VentaID < Helper.ConvertirEntero(sFolioInicial) ? oPagoDet.VentaFolio : sFolioInicial);
                    sFolioFinal = (sFolioFinal == "" || oPagoDet.VentaID > Helper.ConvertirEntero(sFolioFinal) ? oPagoDet.VentaFolio : sFolioFinal);
                }

                // Se llena el importe de la forma de pago correspondiente
                switch (oPagoDet.FormaDePagoID)
                {
                    case Cat.FormasDePago.Efectivo:
                        this.dgvPagos["Efectivo", iFila].Value = oPagoDet.Importe;
                        mEfectivo += oPagoDet.Importe;
                        break;
                    case Cat.FormasDePago.Cheque:
                        this.dgvPagos["Cheque", iFila].Value = oPagoDet.Importe;
                        mCheque += oPagoDet.Importe;
                        break;
                    case Cat.FormasDePago.Tarjeta:
                        this.dgvPagos["Tarjeta", iFila].Value = oPagoDet.Importe;
                        mTarjeta += oPagoDet.Importe;
                        break;
                    case Cat.FormasDePago.Transferencia:
                        this.dgvPagos["Transferencia", iFila].Value = oPagoDet.Importe;
                        mTransferencia += oPagoDet.Importe;
                        break;
                    case Cat.FormasDePago.Vale:
                        this.dgvPagos["NotaDeCredito", iFila].Value = oPagoDet.Importe;
                        mNotaDeCredito += oPagoDet.Importe;
                        break;
                }

                switch (sTipoMov)
                {
                    case "Venta": this.TotalVentas += oPagoDet.ImporteTotal.Valor(); break;
                    case "Cancelacion": this.TotalCancelaciones += oPagoDet.ImporteTotal.Valor(); break;
                    case "Devolucion": this.TotalDevoluciones += oPagoDet.ImporteTotal.Valor(); break;
                }
            }
            // Se llenan los totales
            this.dgvTotales.Rows.Add("Totales", mEfectivo, mCheque, mTarjeta, mTransferencia, mNotaDeCredito);

            // Se llena el concepto y los totales
            this.chkTodos.Checked = true;
            this.txtConcepto.Text = string.Format("VENTAS DEL DIA : {0} DEL FOLIO : {1} AL : {2}", dHoy.ToShortDateString(), sFolioInicial, sFolioFinal);
            this.txtTotal.Text = (mEfectivo + mCheque + mTarjeta + mTransferencia + mNotaDeCredito).ToString(GlobalClass.FormatoMoneda);
            this.txtSeleccionadas.Text = this.txtTotal.Text;

            this.CalcularTotales();
        }

        private void dgvPagos_KeyDown(object sender, KeyEventArgs e)
        {
            this.dgvPagos.VerShift(e, "Contar");
        }

        private void dgvPagos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvPagos.VerDirtyStateChanged("Contar");
        }

        private void dgvPagos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvPagos.CurrentRow == null) return;

            if (this.dgvPagos[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "Contar")
                this.CalcularTotales();
        }

        private void chkTodos_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow Fila in this.dgvPagos.Rows)
                Fila.Cells["Contar"].Value = this.chkTodos.Checked;
        }

        #endregion

        #region [ Métodos ]

        public void CalcularTotales()
        {
            decimal mEfectivo, mCheque, mTarjeta, mTransferencia, mNotaDeCredito;
            mEfectivo = mCheque = mTarjeta = mTransferencia = mNotaDeCredito = 0;

            this.TotalVentas = this.TotalCancelaciones = this.TotalDevoluciones = 0;
            decimal mEfectivoFila, mChequeFila, mTarjetaFila, mTransferenciaFila, mNotaDeCreditoFila, mTotalFila;
            mEfectivoFila = mChequeFila = mTarjetaFila = mTransferenciaFila = mNotaDeCreditoFila = mTotalFila = 0;

            foreach (DataGridViewRow Fila in this.dgvPagos.Rows)
            {
                if (!Helper.ConvertirBool(Fila.Cells["Contar"].Value)) continue;

                mEfectivoFila = Helper.ConvertirDecimal(Fila.Cells["Efectivo"].Value);
                mChequeFila = Helper.ConvertirDecimal(Fila.Cells["Cheque"].Value);
                mTarjetaFila = Helper.ConvertirDecimal(Fila.Cells["Tarjeta"].Value);
                mTransferenciaFila = Helper.ConvertirDecimal(Fila.Cells["Transferencia"].Value);
                mNotaDeCreditoFila = Helper.ConvertirDecimal(Fila.Cells["NotaDeCredito"].Value);

                mEfectivo += mEfectivoFila;
                mCheque += mChequeFila;
                mTarjeta += mTarjetaFila;
                mTransferencia += mTransferenciaFila;
                mNotaDeCredito += mNotaDeCreditoFila;

                mTotalFila = (mEfectivoFila + mChequeFila + mTarjetaFila + mTransferenciaFila + mNotaDeCreditoFila);
                switch (Helper.ConvertirCadena(Fila.Cells["TipoMov"].Value))
                {
                    case "Venta": this.TotalVentas += mTotalFila; break;
                    case "Cancelacion": this.TotalCancelaciones += mTotalFila; break;
                    case "Devolucion": this.TotalDevoluciones += mTotalFila; break;
                }
            }

            this.dgvTotales.Rows[0].SetValues("Totales", mEfectivo, mCheque, mTarjeta, mTransferencia, mNotaDeCredito);
            this.txtSeleccionadas.Text = (mEfectivo + mCheque + mTarjeta + mTransferencia + mNotaDeCredito).ToString(GlobalClass.FormatoMoneda);
        }

        #endregion

        #region [ Públicos ]

        [Obsolete("Este método ya no se usa.")]
        public List<int> GenerarIds(string sTipoMov)
        {
            var Res = new List<int>();
            foreach (DataGridViewRow Fila in this.dgvPagos.Rows)
                if (Helper.ConvertirCadena(Fila.Cells["TipoMov"].Value) == sTipoMov)
                    Res.Add(Helper.ConvertirEntero(Fila.Cells["VentaID"].Value)); //Cambiar llave "VentaID" por "VentaPagoID"
            return Res;
        }

        public string GenerarConceptoCanDev(string sTipoMov)
        {
            string sConcepto = (sTipoMov == "Cancelacion" ? "Cancelaciones" : "Devoluciones");
            string sFolios = "";
            foreach (DataGridViewRow Fila in this.dgvPagos.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["Contar"].Value) && Helper.ConvertirCadena(Fila.Cells["TipoMov"].Value) == sTipoMov)
                    sFolios += (", " + Helper.ConvertirCadena(Fila.Cells["Folio"].Value));
            }
            sFolios = (sFolios == "" ? "" : sFolios.Substring(2));
            sConcepto = (sConcepto + ": " + sFolios).ToUpper();
            return sConcepto;
        }

        #endregion
    }
}
