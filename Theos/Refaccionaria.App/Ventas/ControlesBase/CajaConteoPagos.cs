using System;
using System.Windows.Forms;
using System.Data.Objects;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CajaConteoPagos : CajaMonedas
    {
        public CajaConteoPagos()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public decimal Efectivo { get; set; }
        public decimal Cheques { get; set; }
        public decimal Tarjetas { get; set; }
        public decimal Transferencias { get; set; }

        public decimal Total { get { return (this.Efectivo + this.Cheques + this.Tarjetas + this.Transferencias); } }

        #endregion

        #region [ Eventos ]

        private void CajaConteoPagos_Load(object sender, EventArgs e)
        {
            this.ActualizarDatos();
        }

        private void dgvPagosBancarios_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!this.dgvPagosBancarios.IsCurrentCellDirty) return;
            if (this.dgvPagosBancarios.CurrentCell.OwningColumn.Name == "Resguardar")
            {
                this.dgvPagosBancarios.EndEdit();
            }
        }

        private void dgvPagosBancarios_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvPagosBancarios.Columns[e.ColumnIndex].Name == "Resguardar")
            {
                this.FilaMarcaCambiada(this.dgvPagosBancarios.CurrentRow);
            }
        }
        
        #endregion

        #region [ Métodos ]

        protected override void MonedasCambio()
        {
            this.Efectivo = this.MonedasImporteTotal;
            this.EstablecerEtiquetas();
        }

        protected virtual void FilaMarcaCambiada(DataGridViewRow Fila)
        {
            bool bMarcado = Util.Logico(Fila.Cells["Resguardar"].Value);
            decimal mImporte = Util.Decimal(Fila.Cells["Importe"].Value);
            switch (Util.Entero(Fila.Cells["FormaDePagoID"].Value))
            {
                case Cat.FormasDePago.Cheque:
                    this.Cheques += (mImporte * (bMarcado ? 1 : -1));
                    break;
                case Cat.FormasDePago.Tarjeta:
                case Cat.FormasDePago.TarjetaDeDebito:
                    this.Tarjetas += (mImporte * (bMarcado ? 1 : -1));
                    break;
                case Cat.FormasDePago.Transferencia:
                    this.Transferencias += (mImporte * (bMarcado ? 1 : -1));
                    break;
            }
            this.EstablecerEtiquetas();
        }

        protected virtual void EstablecerEtiquetas()
        {
            this.lblEfectivo.Text = this.Efectivo.ToString(GlobalClass.FormatoDecimal);
            this.lblCheques.Text = this.Cheques.ToString(GlobalClass.FormatoDecimal);
            this.lblTarjetas.Text = this.Tarjetas.ToString(GlobalClass.FormatoDecimal);
            this.lblTransferencias.Text = this.Transferencias.ToString(GlobalClass.FormatoDecimal);
            this.lblTotal.Text = this.Total.ToString(GlobalClass.FormatoDecimal);
        }

        #endregion

        #region [ Públicos ]

        public void ActualizarDatos()
        {
            // Se limpian las monedas
            this.LimpiarMonedas();

            // Se llenan los importes
            int iSucursalID = GlobalClass.SucursalID;
            DateTime dHoy = DateTime.Today;
            var oPagos = Datos.GetListOf<VentasPagosDetalleView>(q => q.SucursalID == iSucursalID && EntityFunctions.TruncateTime(q.Fecha) == dHoy);

            // Se llanan los pagos con comprobantes bancarios
            this.dgvPagosBancarios.Rows.Clear();
            //VentaPagoDetalle oPagoBNegativo;
            foreach (var oPago in oPagos)
            {
                // Solo se cuenta los pagos de tipo bancario
                if (oPago.FormaDePagoID != Cat.FormasDePago.Cheque && oPago.FormaDePagoID != Cat.FormasDePago.Tarjeta
                    && oPago.FormaDePagoID != Cat.FormasDePago.TarjetaDeDebito && oPago.FormaDePagoID != Cat.FormasDePago.Transferencia)
                    continue;
                // Se verifica si existe un pago contrario, lo cual indica que hubo una devolución de Cheque o Tarjeta
                var oPagoCont = oPagos.FirstOrDefault(q => q.VentaPagoDetalleID != oPago.VentaPagoDetalleID && q.VentaID == oPago.VentaID
                    && q.BancoID == oPago.BancoID && q.Folio == oPago.Folio && q.Cuenta == oPago.Cuenta && q.Importe == (oPago.Importe * -1));
                if (oPagoCont != null) continue;
                // Si ya fue resguardado, se omite
                if (oPago.Resguardado) continue;

                this.dgvPagosBancarios.Rows.Add(oPago.VentaPagoDetalleID, oPago.FormaDePagoID, oPago.FormaDePago, oPago.Importe, oPago.Banco, oPago.Folio, oPago.Cuenta);
            }

            // Se resetean los totales
            this.Cheques = this.Tarjetas = this.Transferencias = 0;

            /*
            // Se calcula el importe en efectivo
            var oDia = General.GetEntity<CajaEfectivoPorDia>(q => q.SucursalID == iSucursalID && q.Dia == dHoy && q.Estatus);
            this.Efectivo = (oDia == null ? 0 : oDia.Inicio);
            this.Efectivo += oPagos.Where(q => q.FormaDePagoID == Cat.FormasDePago.Efectivo).Sum(q => q.Importe);
            this.Efectivo += General.GetListOf<CajaIngreso>(q => q.SucursalID == iSucursalID && q.CajaTipoIngresoID == Cat.CajaTiposDeIngreso.Refuerzo
                && EntityFunctions.TruncateTime(q.Fecha) == dHoy && q.Estatus).Sum(q => q.Importe);
            this.Efectivo -= General.GetListOf<CajaEgreso>(q => q.SucursalID == iSucursalID && q.CajaTipoEgresoID == Cat.CajaTiposDeEgreso.Resguardo
                && EntityFunctions.TruncateTime(q.Fecha) == dHoy && q.Estatus).Sum(q => q.Importe);
            // Se calcula el resto de importes
            this.Transferencias = oPagos.Where(q => q.FormaDePagoID == Cat.FormasDePago.Transferencia && !q.Resguardado).Sum(q => q.Importe);
            this.Cheques = oPagos.Where(q => q.FormaDePagoID == Cat.FormasDePago.Cheque && !q.Resguardado).Sum(q => q.Importe);
            this.Tarjetas = oPagos.Where(q => q.FormaDePagoID == Cat.FormasDePago.Tarjeta && !q.Resguardado).Sum(q => q.Importe);
            // Se llenan las etiquetas
            this.lblEfectivo.Text = this.Efectivo.ToString(GlobalClass.FormatoDecimal);
            this.lblTransferencias.Text = this.Transferencias.ToString(GlobalClass.FormatoDecimal);
            this.lblCheques.Text = this.Cheques.ToString(GlobalClass.FormatoDecimal);
            this.lblTarjetas.Text = this.Tarjetas.ToString(GlobalClass.FormatoDecimal);
            this.lblTotal.Text = (this.Efectivo + this.Transferencias + this.Cheques + this.Tarjetas).ToString(GlobalClass.FormatoDecimal);

            decimal mTruncado = (((int)(this.Efectivo / 100)) * 100);
            decimal mImporteIdeal = Util.ConvertirDecimal(Config.Valor("Caja.Resguardo.Ideal"));
            mImporteIdeal += (this.Efectivo - mTruncado);
            this.lblImporteIdeal.Text = mImporteIdeal.ToString(GlobalClass.FormatoDecimal);
            */

            this.MonedasCambio();
        }


        public override ConteoCaja GenerarConteo()
        {
            var oConteo = base.GenerarConteo();
            foreach (DataGridViewRow Fila in this.dgvPagosBancarios.Rows)
            {
                if (Util.Logico(Fila.Cells["Resguardar"].Value))
                    oConteo.Comprobantes.Add(new ConteoComprobante()
                    {
                        Tipo = Util.Cadena(Fila.Cells["Tipo"].Value),
                        Importe = Util.Decimal(Fila.Cells["Importe"].Value),
                        Banco = Util.Cadena(Fila.Cells["Banco"].Value),
                        Cuenta = Util.Cadena(Fila.Cells["Cuenta"].Value),
                        Folio = Util.Cadena(Fila.Cells["Folio"].Value)
                    });
            }

            return oConteo;
        }

        #endregion
    }
}
