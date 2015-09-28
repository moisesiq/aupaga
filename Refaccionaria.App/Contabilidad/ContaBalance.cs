using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ContaBalance : UserControl
    {
        public ContaBalance()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ContaBalance_Load(object sender, EventArgs e)
        {
            // Se configuran los controles
            this.cmbSucursal1.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.dtpDesde1.Value = new DateTime(DateTime.Now.Year, 1, 1);
            this.dtpHasta1.Value = this.dtpDesde1.Value.AddYears(1).AddDays(-1);
            this.cmbSucursal2.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.dtpDesde2.Value = this.dtpDesde1.Value.AddYears(-1);
            this.dtpHasta2.Value = this.dtpHasta1.Value.AddYears(-1);
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            var oParams1 = new Dictionary<string, object>();
            oParams1.Add("Desde", this.dtpDesde1.Value);
            oParams1.Add("Hasta", this.dtpHasta1.Value);
            if (this.cmbSucursal1.SelectedValue != null)
                oParams1.Add("SucursalID", Helper.ConvertirEntero(this.cmbSucursal1.SelectedValue));
            var oParams2 = new Dictionary<string, object>();
            oParams2.Add("Desde", this.dtpDesde2.Value);
            oParams2.Add("Hasta", this.dtpHasta2.Value);
            if (this.cmbSucursal2.SelectedValue != null)
                oParams2.Add("SucursalID", Helper.ConvertirEntero(this.cmbSucursal2.SelectedValue));
            var oDatos1 = General.ExecuteProcedure<pauContaCuentasPolizasImportes_Result>("pauContaCuentasPolizasImportes", oParams1);
            var oDatos2 = General.ExecuteProcedure<pauContaCuentasPolizasImportes_Result>("pauContaCuentasPolizasImportes", oParams2);

            // Se sacan las cuentas de mayor que no aplican
            oDatos1 = oDatos1.Where(c => c.ContaCuentaID == Cat.ContaCuentas.Activo || c.ContaCuentaID == Cat.ContaCuentas.Pasivo
                || c.ContaCuentaID == Cat.ContaCuentas.CapitalContable).ToList();
            oDatos2 = oDatos2.Where(c => c.ContaCuentaID == Cat.ContaCuentas.Activo || c.ContaCuentaID == Cat.ContaCuentas.Pasivo
                || c.ContaCuentaID == Cat.ContaCuentas.CapitalContable).ToList();

            var oPorCuenta1 = oDatos1.GroupBy(c => (c.Cuenta + " - " + c.Subcuenta)).Select(c => new { Subcuenta = c.Key, Importe = c.Sum(s => s.Importe) })
                .ToDictionary(c => c.Subcuenta, d => d.Importe);
            var oPorCuentaDemayor1 = oDatos1.GroupBy(c => new { Subcuenta = (c.Cuenta + " - " + c.Subcuenta), c.CuentaDeMayor }).Select(c =>
                new { Subcuenta = c.Key.Subcuenta, CuentaDeMayor = c.Key.CuentaDeMayor, Importe = c.Sum(s => s.Importe) }).ToList();
            var oPorCuenta2 = oDatos2.GroupBy(c => (c.Cuenta + " - " + c.Subcuenta)).Select(c => new { Subcuenta = c.Key, Importe = c.Sum(s => s.Importe) })
                .ToDictionary(c => c.Subcuenta, d => d.Importe);
            var oPorCuentaDemayor2 = oDatos2.GroupBy(c => new { Subcuenta = (c.Cuenta + " - " + c.Subcuenta), c.CuentaDeMayor }).Select(c =>
                new { Subcuenta = c.Key.Subcuenta, CuentaDeMayor = c.Key.CuentaDeMayor, Importe = c.Sum(s => s.Importe) }).ToList();
            
            // Se comienzan a llenar los datos
            this.dgvActivos.Rows.Clear();
            this.dgvPasivos.Rows.Clear();
            decimal mActivosT1 = oPorCuenta1.Where(c => c.Key.ToLower().StartsWith("activo")).Sum(c => c.Value).Valor();
            decimal mPasivosT1 = oPorCuenta1.Where(c => !c.Key.ToLower().StartsWith("activo")).Sum(c => c.Value).Valor();
            decimal mActivosT2 = oPorCuenta2.Where(c => c.Key.ToLower().StartsWith("activo")).Sum(c => c.Value).Valor();
            decimal mPasivosT2 = oPorCuenta2.Where(c => !c.Key.ToLower().StartsWith("activo")).Sum(c => c.Value).Valor();
            DataGridView oGrid;
            string sSubcuentaActivos = "", sSubcuentaPasivos = "";
            for (int i = 0; i < oPorCuentaDemayor1.Count; i++)
            {
                var oReg1 = oPorCuentaDemayor1[i];
                var oReg2 = oPorCuentaDemayor2[i];
                if (oReg1.Subcuenta.ToLower().StartsWith("activo"))
                {
                    oGrid = this.dgvActivos;
                    if (sSubcuentaActivos != oReg1.Subcuenta)
                    {
                        sSubcuentaActivos = oReg1.Subcuenta;
                        int iFila = oGrid.Rows.Add(oReg1.Subcuenta, oPorCuenta1[sSubcuentaActivos], Helper.DividirONull(oPorCuenta1[sSubcuentaActivos], mActivosT1)
                            , oPorCuenta2[sSubcuentaActivos], Helper.DividirONull(oPorCuenta2[sSubcuentaActivos], mActivosT2));
                        oGrid.Rows[iFila].DefaultCellStyle.Font = new Font(oGrid.DefaultCellStyle.Font, FontStyle.Bold);
                    }
                }
                else
                {
                    oGrid = this.dgvPasivos;
                    if (sSubcuentaPasivos != oReg1.Subcuenta)
                    {
                        sSubcuentaPasivos = oReg1.Subcuenta;
                        int iFila = oGrid.Rows.Add(oReg1.Subcuenta, oPorCuenta1[sSubcuentaPasivos], Helper.DividirONull(oPorCuenta1[sSubcuentaPasivos], mPasivosT1)
                            , oPorCuenta2[sSubcuentaPasivos], Helper.DividirONull(oPorCuenta2[sSubcuentaPasivos], mPasivosT2));
                        oGrid.Rows[iFila].DefaultCellStyle.Font = new Font(oGrid.DefaultCellStyle.Font, FontStyle.Bold);
                    }
                }

                oGrid.Rows.Add(oReg1.CuentaDeMayor
                    , oReg1.Importe, Helper.DividirONull(oReg1.Importe, oPorCuenta1[oReg1.Subcuenta])
                    , oReg2.Importe, Helper.DividirONull(oReg2.Importe, oPorCuenta2[oReg2.Subcuenta])
                    , Helper.DividirONull(oReg1.Importe, oReg2.Importe));
            }

            Cargando.Cerrar();
        }
    
        #endregion

    }
}
