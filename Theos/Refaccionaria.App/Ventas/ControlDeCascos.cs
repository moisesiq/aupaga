using System;
using System.Windows.Forms;
using System.Drawing;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class ControlDeCascos : UserControl
    {
        public ControlDeCascos()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ControlDeCascos_Load(object sender, EventArgs e)
        {
            // Se configuran los controles
            this.cmbHisSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
            this.cmbHisSucursal.SelectedValue = GlobalClass.SucursalID;
            this.dtpHisDesde.Value = DateTime.Now.DiaPrimero();
            this.dtpHisHasta.Value = DateTime.Now.DiaUltimo();
            
            //
            this.CargarDatos();
        }

        private void dgvPendientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iCascoRegistroID = Util.Entero(this.dgvPendientes.CurrentRow.Cells["CascoRegistroID"].Value);
            var frmAccion = new CascoRegistroCompletar(iCascoRegistroID);
            if (frmAccion.ShowDialog(Principal.Instance) == DialogResult.OK)
                this.CargarDatos();
            frmAccion.Dispose();
        }

        private void btnHisActualizar_Click(object sender, EventArgs e)
        {
            Cargando.Mostrar();

            int iSucursalID = Util.Entero(this.cmbHisSucursal.SelectedValue);
            DateTime dDesde = this.dtpHisDesde.Value.Date;
            DateTime dHasta = this.dtpHisHasta.Value.Date.AddDays(1);
            var oDatos = Datos.GetListOf<CascosRegistrosView>(c => c.SucursalID == iSucursalID && (c.Fecha >= dDesde && c.Fecha < dHasta));

            this.dgvHistorico.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                int iFila = this.dgvHistorico.Rows.Add(oReg.CascoRegistroID, oReg.Fecha, oReg.FolioDeVenta, oReg.Cliente, oReg.NumeroDeParte, oReg.Descripcion
                    , oReg.NumeroDeParteDeCasco, oReg.NumeroDeParteRecibido, oReg.FolioDeCobro, oReg.CascoImporte);
                if (oReg.VentaEstatusID == Cat.VentasEstatus.Cancelada || oReg.VentaEstatusID == Cat.VentasEstatus.CanceladaSinPago)
                    this.dgvHistorico.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }

            Cargando.Cerrar();
        }

        private void txtReimpresion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.VerTicketCasco();
            }
        }
        
        private void dgvHistorico_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvHistorico.CurrentRow.Cells["hisNumeroDeParteRecibido"].Value == null && this.dgvHistorico.CurrentRow.Cells["hisFolioDeCobro"].Value != null)
            {
                int iCascoRegistroID = Util.Entero(this.dgvHistorico.CurrentRow.Cells["hisCascoRegistroID"].Value);
                var frmAccion = new CascoRegistroCompletar(iCascoRegistroID);
                if (frmAccion.ShowDialog(Principal.Instance) == DialogResult.OK)
                    this.btnHisActualizar_Click(sender, e);
                frmAccion.Dispose();
            }
        }

        #endregion

        #region [ Métodos ]

        private void VerTicketCasco()
        {
            int iCascoID = Util.Entero(this.txtReimpresion.Text);
            VentasLoc.GenerarTicketCasco(iCascoID);
        }

        #endregion

        #region [ Públicos ]

        public void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = Datos.GetListOf<CascosRegistrosView>(c => c.NumeroDeParteRecibido == null && c.FolioDeCobro == null
                && (c.VentaEstatusID != Cat.VentasEstatus.Cancelada && c.VentaEstatusID != Cat.VentasEstatus.CanceladaSinPago) && c.SucursalID == GlobalClass.SucursalID);
            this.dgvPendientes.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvPendientes.Rows.Add(oReg.CascoRegistroID, oReg.Fecha, oReg.FolioDeVenta, oReg.Cliente, oReg.NumeroDeParte, oReg.Descripcion
                    , oReg.NumeroDeParteDeCasco, oReg.NumeroDeParteRecibido, oReg.FolioDeCobro, oReg.CascoImporte);

            Cargando.Cerrar();
        }

        #endregion

    }
}
