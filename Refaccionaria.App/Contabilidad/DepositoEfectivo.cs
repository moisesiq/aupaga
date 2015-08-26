using System;
using System.Windows.Forms;
using System.Linq;
using System.Data.Objects;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class DepositoEfectivo : MovimientoBancarioGen
    {
        public DepositoEfectivo(int iBancoCuentaID)
        {
            InitializeComponent();

            this.OrigenBancoCuentaID = iBancoCuentaID;
        }

        #region [ Eventos ]

        private void DepositoEfectivo_Load(object sender, EventArgs e)
        {
            this.dtpFecha.Value = DateTime.Now.AddDays(-1);

            // Se llena el importe total de resguardo matriz
            var oResguardos = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.Resguardo
                && c.Cargo > 0 && c.SucursalID == GlobalClass.SucursalID);
            this.lblImporteInfo.Text = oResguardos.Sum(c => c.Cargo).ToString(GlobalClass.FormatoMoneda);
            // Se calcula el importe correspondiente
            this.CalcularImporteDia();
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpFecha.Focused)
                this.CalcularImporteDia();
        }

        #endregion

        #region [ Métodos ]

        private void CalcularImporteDia()
        {
            DateTime dDia = this.dtpFecha.Value.Date;
            var oFacturasEfe = General.GetListOf<VentasPagosDetalleAvanzadoView>(c => c.SucursalID == GlobalClass.SucursalID && c.Facturada
                && c.VentaEstatusID == Cat.VentasEstatus.Completada && c.FormaDePagoID == Cat.FormasDePago.Efectivo && EntityFunctions.TruncateTime(c.Fecha) == dDia);
            var oTicketsBanco = General.GetListOf<VentasPagosDetalleAvanzadoView>(c => c.SucursalID == GlobalClass.SucursalID && !c.Facturada
                && c.VentaEstatusID == Cat.VentasEstatus.Completada
                && (c.FormaDePagoID == Cat.FormasDePago.Cheque || c.FormaDePagoID == Cat.FormasDePago.Tarjeta || c.FormaDePagoID == Cat.FormasDePago.Transferencia)
                && EntityFunctions.TruncateTime(c.Fecha) == dDia);
            var oFacturaGlobal = General.GetEntity<CajaFacturaGlobal>(c => c.Dia == dDia);
            decimal mFacturasEfe = oFacturasEfe.Sum(c => c.Importe);
            decimal mTicketsBanco = oTicketsBanco.Sum(c => c.Importe);
            decimal mFacturaGlobal = (oFacturaGlobal == null ? 0 : oFacturaGlobal.Facturado);
            this.txtImporte.Text = (mFacturasEfe - mTicketsBanco + mFacturaGlobal).ToString();

            // Se muestra o no el ícono de advertencia
            if (oFacturaGlobal == null)
                this.ctlAdv.PonerError(this.txtImporte, "No se encontró factura global el día especificado.");
            else
                this.ctlAdv.QuitarError(this.txtImporte);
        }

        protected override bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            // Se crea el movimiento bancario
            DateTime dFecha = DateTime.Now;  // Se toma la fecha de hoy
            var oMovBanc = new BancoCuentaMovimiento()
            {
                BancoCuentaID = this.OrigenBancoCuentaID,
                EsIngreso = true,
                Fecha = dFecha,
                FechaAsignado = dFecha,
                SucursalID = GlobalClass.SucursalID,
                Importe = Helper.ConvertirDecimal(this.txtImporte.Text),
                Concepto = this.txtConcepto.Text,
                Referencia = GlobalClass.UsuarioGlobal.NombreUsuario,
                TipoFormaPagoID = Cat.FormasDePago.Efectivo
            };
            ContaProc.RegistrarMovimientoBancario(oMovBanc);

            // Se crea la póliza correspondiente (AfeConta)
            var oPoliza = ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DepositoBancario, oMovBanc.BancoCuentaMovimientoID, oMovBanc.Referencia, oMovBanc.Concepto);
            oPoliza.Fecha = oMovBanc.Fecha;
            Guardar.Generico<ContaPoliza>(oPoliza);

            Cargando.Cerrar();

            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Helper.ConvertirDecimal(this.txtImporte.Text) <= 0)
                this.ctlError.PonerError(this.txtImporte, "Debes especificar un importe mayor a cero.");
            if (this.txtConcepto.Text == "")
                this.ctlError.PonerError(this.txtConcepto, "Debes especificar un concepto.", ErrorIconAlignment.BottomLeft);
            return this.ctlError.Valido;
        }

        #endregion

    }
}
