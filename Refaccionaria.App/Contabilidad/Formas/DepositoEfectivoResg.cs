using System;
using System.Windows.Forms;
using System.Data.Objects;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class DepositoEfectivoResg : Form
    {
        ControlError ctlError = new ControlError();
        ControlError ctlAdv = new ControlError() { Icon = Properties.Resources._16_Ico_Advertencia };

        public DepositoEfectivoResg(int iBancoCuentaID)
        {
            InitializeComponent();

            this.OrigenBancoCuentaID = iBancoCuentaID;
        }

        #region [ Propiedades ]

        public int OrigenBancoCuentaID { get; set; }

        #endregion

        #region [ Eventos ]

        private void DepositoEfectivoResg_Load(object sender, EventArgs e)
        {
            this.dtpFechaSugerido.Value = DateTime.Now.AddDays(-1);

            // Se llena el importe total de resguardo
            DateTime dInicioAnio = new DateTime(DateTime.Now.Year, 1, 1);
            var oResguardos = General.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.ContaCuentaAuxiliarID == Cat.ContaCuentasAuxiliares.Resguardo
                && c.FechaPoliza >= dInicioAnio);
            this.lblImporteInfo.Text = oResguardos.Sum(c => c.Cargo - c.Abono).ToString(GlobalClass.FormatoMoneda);
            // Se calcula el importe correspondiente
            this.CalcularImporteDia();
        }

        private void dtpFechaSugerido_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpFechaSugerido.Focused)
                this.CalcularImporteDia();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void CalcularImporteDia()
        {
            Cargando.Mostrar();

            DateTime dDia = this.dtpFechaSugerido.Value.Date;
            var oFacturasEfe = General.GetListOf<VentasPagosDetalleAvanzadoView>(c => c.SucursalID == GlobalClass.SucursalID && c.Facturada
                && c.VentaEstatusID == Cat.VentasEstatus.Completada && c.FormaDePagoID == Cat.FormasDePago.Efectivo && EntityFunctions.TruncateTime(c.Fecha) == dDia);
            var oTicketsNoEfe = General.GetListOf<VentasPagosDetalleAvanzadoView>(c => c.SucursalID == GlobalClass.SucursalID && !c.Facturada
                && c.VentaEstatusID == Cat.VentasEstatus.Completada
                && (c.FormaDePagoID == Cat.FormasDePago.Cheque || c.FormaDePagoID == Cat.FormasDePago.Tarjeta || c.FormaDePagoID == Cat.FormasDePago.Transferencia
                || c.FormaDePagoID == Cat.FormasDePago.Vale)
                && EntityFunctions.TruncateTime(c.Fecha) == dDia);
            var oFacturaGlobal = General.GetEntity<CajaFacturaGlobal>(c => c.Dia == dDia);
            var oDevEfe = General.GetListOf<VentasDevolucionesView>(c => c.SucursalID == GlobalClass.SucursalID && c.FormaDePagoID == Cat.FormasDePago.Efectivo
                && EntityFunctions.TruncateTime(c.Fecha) == dDia);

            decimal mFacturasEfe = oFacturasEfe.Sum(c => c.Importe);
            decimal mTicketsNoEfe = oTicketsNoEfe.Sum(c => c.Importe);
            decimal mFacturaGlobal = (oFacturaGlobal == null ? 0 : oFacturaGlobal.Facturado);
            decimal mDevEfe = oDevEfe.Sum(c => c.Total).Valor();
            // this.txtImporte.Text = (mFacturasEfe - mTicketsNoEfe + mFacturaGlobal - mDevEfe).ToString();
            this.lblImporteSugerido.Text = (mFacturasEfe - mTicketsNoEfe + mFacturaGlobal - mDevEfe).ToString(GlobalClass.FormatoMoneda);

            // Se muestra o no el ícono de advertencia
            if (oFacturaGlobal == null)
                this.ctlAdv.PonerError(this.txtImporte, "No se encontró factura global el día especificado.");
            else
                this.ctlAdv.QuitarError(this.txtImporte);

            Cargando.Cerrar();
        }

        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            // Se crea el movimiento bancario
            DateTime dFecha = this.dtpFechaMovimiento.Value;
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
            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.DepositoBancario, oMovBanc.BancoCuentaMovimientoID
                , oMovBanc.Referencia, oMovBanc.Concepto, oMovBanc.Fecha);

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
