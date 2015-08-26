using System;
using System.Windows.Forms;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CajaGastos : UserControl
    {
        ControlError ctlError = new ControlError();

        List<int> IngresosBorrados = new List<int>();
        List<int> EgresosBorrados = new List<int>();

        public CajaGastos()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CajaGastos_Load(object sender, EventArgs e)
        {
            // Se llenan los datos
            this.ActualizarDatos();
        }

        private void cmbGastos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbGastos.Focused) return;

            if (this.cmbGastos.SelectedIndex >= 0)
                this.cmbOtrosIngresos.SelectedIndex = -1;
        }

        private void cmbOtrosIngresos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.cmbOtrosIngresos.Focused) return;

            if (this.cmbOtrosIngresos.SelectedIndex >= 0)
                this.cmbGastos.SelectedIndex = -1;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Se validan los datos
            if (!this.Validar())
                return;

            // Se agregan los datos
            string sTipo = (this.cmbGastos.SelectedValue == null ? "INGRESO" : "GASTO");
            int iCategoriaID = Helper.ConvertirEntero(this.cmbGastos.SelectedValue == null ? this.cmbOtrosIngresos.SelectedValue : this.cmbGastos.SelectedValue);
            this.dgvConceptos.Rows.Add(true, 0, iCategoriaID, sTipo, this.txtConcepto.Text, Helper.ConvertirDecimal(this.txtImporte.Text));

            // Se limpian los controles
            this.cmbGastos.SelectedIndex = -1;
            this.cmbOtrosIngresos.SelectedIndex = -1;
            this.txtConcepto.Clear();
            this.txtImporte.Clear();
        }

        private void dgvConceptos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvConceptos.CurrentRow == null) return;

            if (e.KeyCode == Keys.Delete)
            {
                if (UtilLocal.MensajePregunta("¿Estás seguro que deseas eliminar el concepto seleccionado?") == DialogResult.Yes)
                {
                    if (!Helper.ConvertirBool(this.dgvConceptos.CurrentRow.Cells["EsNuevo"].Value))
                    {
                        if (Helper.ConvertirCadena(this.dgvConceptos.CurrentRow.Cells["Tipo"].Value).ToLower() == "ingreso")
                            this.IngresosBorrados.Add(Helper.ConvertirEntero(this.dgvConceptos.CurrentRow.Cells["ID"].Value));
                        else
                            this.EgresosBorrados.Add(Helper.ConvertirEntero(this.dgvConceptos.CurrentRow.Cells["ID"].Value));
                    }
                    this.dgvConceptos.Rows.Remove(this.dgvConceptos.CurrentRow);
                }
            }
        }

        #endregion

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (this.cmbGastos.SelectedValue == null && this.cmbOtrosIngresos.SelectedValue == null)
                this.ctlError.PonerError(this.cmbOtrosIngresos, "Debes especificar un tipo de operación.");
            if (this.txtConcepto.Text == "")
                this.ctlError.PonerError(this.txtConcepto, "Debes especificar un concepto.", ErrorIconAlignment.BottomRight);
            if (!Helper.ValidarDecimal(this.txtImporte.Text))
                this.ctlError.PonerError(this.txtImporte, "Importe no especificado o inválido.", ErrorIconAlignment.MiddleLeft);
            return (this.ctlError.NumeroDeErrores == 0);
        }

        #region [ Públicos ]

        public List<CajaIngreso> GenerarListaDeIngresos()
        {
            var oLista = new List<CajaIngreso>();
            DateTime dAhora = DateTime.Now;
            foreach (DataGridViewRow Fila in this.dgvConceptos.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["EsNuevo"].Value) && Helper.ConvertirCadena(Fila.Cells["Tipo"].Value).ToLower() == "ingreso")
                    oLista.Add(new CajaIngreso()
                    {
                        CajaTipoIngresoID = Helper.ConvertirEntero(Fila.Cells["CategoriaID"].Value),
                        Concepto = Helper.ConvertirCadena(Fila.Cells["Concepto"].Value),
                        Importe = Helper.ConvertirDecimal(Fila.Cells["Importe"].Value),
                        Fecha = dAhora,
                        SucursalID = GlobalClass.SucursalID
                    });
            }

            return oLista;
        }

        public List<Dictionary<string, object>> GenerarListaDeEgresos()
        {
            /* var oLista = new List<CajaEgreso>();
            DateTime dAhora = DateTime.Now;
            foreach (DataGridViewRow Fila in this.dgvConceptos.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["EsNuevo"].Value) && Helper.ConvertirCadena(Fila.Cells["Tipo"].Value).ToLower() == "gasto")
                    oLista.Add(new CajaEgreso()
                    {
                        CajaTipoEgresoID = Helper.ConvertirEntero(Fila.Cells["CategoriaID"].Value),
                        Concepto = Helper.ConvertirCadena(Fila.Cells["Concepto"].Value),
                        Importe = Helper.ConvertirDecimal(Fila.Cells["Importe"].Value),
                        Fecha = dAhora,
                        SucursalID = GlobalClass.SucursalID
                    });
            }
            */

            DateTime dAhora = DateTime.Now;
            var oLista = new List<Dictionary<string, object>>();
            foreach (DataGridViewRow Fila in this.dgvConceptos.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["EsNuevo"].Value) && Helper.ConvertirCadena(Fila.Cells["Tipo"].Value).ToLower() == "gasto")
                {
                    var oGasto = new Dictionary<string, object>();
                    oGasto.Add("CategoriaID", Fila.Cells["CategoriaID"].Value);
                    oGasto.Add("Concepto", Fila.Cells["Concepto"].Value);
                    oGasto.Add("Importe", Fila.Cells["Importe"].Value);
                    oGasto.Add("SucursalID", GlobalClass.SucursalID);
                    oLista.Add(oGasto);
                }
            }

            return oLista;
        }

        public List<CajaIngreso> GenerarListaDeIngresosBorrados()
        {
            var oLista = new List<CajaIngreso>();
            foreach (int iIngresoID in this.IngresosBorrados)
                oLista.Add(General.GetEntity<CajaIngreso>(q => q.CajaIngresoID == iIngresoID && q.Estatus));
            return oLista;
        }

        public List<CajaEgreso> GenerarListaDeEgresosBorrados()
        {
            var oLista = new List<CajaEgreso>();
            foreach (int iEgresoID in this.EgresosBorrados)
                oLista.Add(General.GetEntity<CajaEgreso>(q => q.CajaEgresoID == iEgresoID && q.Estatus));
            return oLista;
        }

        public void ActualizarDatos()
        {
            // Se llenan los combos
            var oCuentasAux = General.GetListOf<ContaCuentasAuxiliaresView>().Where(c => c.VisibleEnCaja).Select(c =>
                new { c.ContaCuentaAuxiliarID, CuentaDeMayorCuentaAuxiliar = (c.CuentaDeMayor + " - " + c.CuentaAuxiliar) })
                .OrderBy(c => c.CuentaDeMayorCuentaAuxiliar).ToList();
            this.cmbGastos.CargarDatos("ContaCuentaAuxiliarID", "CuentaDeMayorCuentaAuxiliar", oCuentasAux);
            this.cmbOtrosIngresos.CargarDatos("CajaTipoIngresoID", "NombreTipoIngreso"
                , General.GetListOf<CajaTipoIngreso>(q => q.Seleccionable && q.Estatus).OrderBy(c => c.NombreTipoIngreso).ToList());

            // Se llenan los movimientos del día
            DateTime dHoy = DateTime.Today;
            var oEgresos = General.GetListOf<CajaEgreso>(q => q.SucursalID == GlobalClass.SucursalID && EntityFunctions.TruncateTime(q.Fecha) == dHoy && q.Estatus);
            var oIngresos = General.GetListOf<CajaIngreso>(q => q.SucursalID == GlobalClass.SucursalID && EntityFunctions.TruncateTime(q.Fecha) == dHoy && q.Estatus);

            this.dgvConceptos.Rows.Clear();
            foreach (var oMov in oEgresos)
                this.dgvConceptos.Rows.Add(false, oMov.CajaEgresoID, oMov.CajaTipoEgresoID, "GASTO", oMov.Concepto, oMov.Importe);
            foreach (var oMov in oIngresos)
                this.dgvConceptos.Rows.Add(false, oMov.CajaIngresoID, oMov.CajaTipoIngresoID, "INGRESO", oMov.Concepto, oMov.Importe);

            // Se ordenan
            this.dgvConceptos.Sort(this.dgvConceptos.Columns["Concepto"], ListSortDirection.Ascending);
        }

        #endregion
                
    }
}
