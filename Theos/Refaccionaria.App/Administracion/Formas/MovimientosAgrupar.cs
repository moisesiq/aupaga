using System;
using System.Windows.Forms;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class MovimientosAgrupar : Form
    {
        bool bEsNuevo = true;
        int iDiasDePlazo = 0;
        int iMovimientoAgrupadorID;
        List<int> oMovimientosAgrupados;
        ControlError ctlError = new ControlError();

        public MovimientosAgrupar(int iMovimientoAgrupadorID)
        {
            InitializeComponent();

            this.bEsNuevo = false;
            this.iMovimientoAgrupadorID = iMovimientoAgrupadorID;
        }

        public MovimientosAgrupar(List<int> oMovimientosAgrupados)
        {
            this.InitializeComponent();

            this.oMovimientosAgrupados = oMovimientosAgrupados;
        }

        #region [ Propiedades ]

        public DateTime Vencimiento { get; private set; }
        public DateTime FechaGuardar { get; private set; }

        #endregion

        #region [ Eventos ]

        private void MovimientoAgrupar_Load(object sender, EventArgs e)
        {
            DateTime dVencimiento = DateTime.MaxValue;
            int iProveedorID = 0;
            if (this.bEsNuevo)
            {
                this.Text = "Agrupar movimientos";
                
                // Se llenan los datos
                decimal mTotal = 0;
                foreach (int iMovID in this.oMovimientosAgrupados)
                {
                    var oReg = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == iMovID && c.Estatus);
                    mTotal += oReg.ImporteFactura;
                    if (oReg.FechaRecepcion < dVencimiento)
                        dVencimiento = oReg.FechaRecepcion.Valor();
                    // Se obtiene el proveedor
                    if (iProveedorID == 0)
                        iProveedorID = oReg.ProveedorID.Valor();
                }
                this.lblMovimientos.Text = this.oMovimientosAgrupados.Count.ToString();
                this.lblImporteFactura.Text = mTotal.ToString(GlobalClass.FormatoMoneda);
            }
            else
            {
                this.Text = "Editar movimiento agrupador";

                // Se llenan los datos
                var oMovAgr = Datos.GetEntity<MovimientoInventario>(c => c.MovimientoInventarioID == this.iMovimientoAgrupadorID && c.Estatus);
                iProveedorID = oMovAgr.ProveedorID.Valor();
                var oMovs = Datos.GetListOf<MovimientoInventario>(c => c.MovimientoAgrupadorID == this.iMovimientoAgrupadorID && c.Estatus);
                this.lblMovimientos.Text = oMovs.Count.ToString();
                this.lblImporteFactura.Text = oMovAgr.ImporteFactura.ToString(GlobalClass.FormatoMoneda);
            }

            // Se sugiere la fecha de vencimiento
            var oProveedor = Datos.GetEntity<Proveedor>(c => c.ProveedorID == iProveedorID && c.Estatus);
            this.iDiasDePlazo = oProveedor.DiasPlazo.Valor();
            dVencimiento = AdmonProc.SugerirVencimientoCompra(dVencimiento, iDiasDePlazo);
            this.dtpVencimiento.Value = dVencimiento;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!this.Validar())
                return;

            this.Vencimiento = this.dtpVencimiento.Value;
            this.FechaGuardar = this.Vencimiento.AddDays(iDiasDePlazo * -1);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Privados ]
        
        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (this.dtpVencimiento.Value.DayOfWeek == DayOfWeek.Saturday || this.dtpVencimiento.Value.DayOfWeek == DayOfWeek.Sunday)
                this.ctlError.PonerError(this.dtpVencimiento, "No se puede seleccionar el día Sábado ni Domingo como fecha de vencimiento.");
            return this.ctlError.Valido;
        }

        #endregion
    }
}
