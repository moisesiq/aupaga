using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class ContaReserva : UserControl
    {
        public ContaReserva()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ContaReserva_Load(object sender, EventArgs e)
        {
            this.dtpDesde.Value = DateTime.Now.DiaPrimero();
            this.dtpHasta.Value = DateTime.Now.DiaUltimo();
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));

            // Se cargan los datos
            this.LlenarReserva();
        }

        private void dtpDesde_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpDesde.Focused)
                this.LlenarReserva();
        }

        private void dtpHasta_ValueChanged(object sender, EventArgs e)
        {
            if (this.dtpHasta.Focused)
                this.LlenarReserva();
        }

        private void cmbSucursal_TextChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused && this.cmbSucursal.Text == "")
                this.LlenarReserva();
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused)
                this.LlenarReserva();
        }

        #endregion

        #region [ Métodos ]

        private void LlenarReserva()
        {
            Cargando.Mostrar();

            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            var oDatos = Datos.GetListOf<CajaFacturasGlobalesView>(c => c.Dia >= this.dtpDesde.Value.Date && c.Dia <= this.dtpHasta.Value.Date
                && (iSucursalID == 0 || c.SucursalID == iSucursalID));
            this.dgvReserva.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvReserva.Rows.Add(oReg.CajaFacturaGlobalID, oReg.Dia, oReg.Sucursal, oReg.Tickets, oReg.FacturadoDeDiasAnt, oReg.Negativos
                    , oReg.DevolucionesDia, oReg.DevolucionesDiasAnt, oReg.GarantiasDia, oReg.GarantiasDiasAnt, oReg.Cobranza
                    , oReg.Oficial, oReg.Restar, oReg.Supuesto, oReg.CostoMinimo, oReg.Restante
                    , oReg.SaldoRestante, oReg.PreFacturar, oReg.Facturado
                    , ((oReg.Oficial - oReg.Facturado) > 0 ? (oReg.Oficial - oReg.Facturado) : 0));

            Cargando.Cerrar();
        }

        #endregion



    }
}
