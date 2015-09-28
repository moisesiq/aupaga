using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class ContaEstadoDeResultados : UserControl
    {
        public ContaEstadoDeResultados()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ContaEstadoDeResultados_Load(object sender, EventArgs e)
        {
            // Se cargan los combos
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));

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

            var oFuenteT = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);

            // Se obtienen los datos para los ingresos
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            DateTime dDesde = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime dHasta = DateTime.Now;
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", true);
            oParams.Add("Cobradas", false);
            oParams.Add("Solo9500", false);
            oParams.Add("OmitirDomingo", false);
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);

            var oDatos = General.ExecuteProcedure<pauCuadroDeControlGeneral_Result>("pauCuadroDeControlGeneral", oParams);
            var oSemanas = oDatos.GroupBy(c => new { Semana = UtilLocal.SemanaSabAVie(c.Fecha), c.Sucursal })
                .Select(c => new { c.Key.Semana, c.Key.Sucursal, Precio = c.Sum(s => s.PrecioActual), Costo = c.Sum(s => s.CostoDescActual) })
                .OrderBy(c => c.Sucursal).ThenBy(c => c.Semana);
            this.dgvDatos.Rows.Clear();

            // Se agrega la fila de ingresos
            int iFilaIngresos = this.dgvDatos.Rows.Add("+ Ingresos");
            this.dgvDatos.Rows[iFilaIngresos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los ingresos
            DateTime dSemana = dDesde;
            string sSucursal = "";
            int iFila = 0;
            foreach (var oReg in oSemanas)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal);
                    dSemana = UtilLocal.InicioSemanaSabAVie(dDesde);
                }

                string sSemana = dSemana.ToShortDateString();
                if (!this.dgvDatos.Columns.Contains(sSemana))
                {
                    this.dgvDatos.Columns.Add(sSemana, sSemana);
                    this.dgvDatos.Columns[sSemana].FormatoMoneda();
                }

                this.dgvDatos[sSemana, iFila].Value = oReg.Precio;
                this.dgvDatos[sSemana, iFilaIngresos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaIngresos].Value) + oReg.Precio);
                dSemana = dSemana.AddDays(7);
            }

            // Se agrega la fila de Costos
            int iFilaCostos = this.dgvDatos.Rows.Add("- Costos");
            this.dgvDatos.Rows[iFilaCostos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los costos
            dSemana = UtilLocal.InicioSemanaSabAVie(dDesde);
            sSucursal = "";
            foreach (var oReg in oSemanas)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal);
                    dSemana = UtilLocal.InicioSemanaSabAVie(dDesde);
                }

                string sSemana = dSemana.ToShortDateString();
                if (!this.dgvDatos.Columns.Contains(sSemana))
                {
                    this.dgvDatos.Columns.Add(sSemana, sSemana);
                    this.dgvDatos.Columns[sSemana].FormatoMoneda();
                }

                this.dgvDatos[sSemana, iFila].Value = oReg.Costo;
                this.dgvDatos[sSemana, iFilaCostos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaCostos].Value) + oReg.Costo);
                dSemana = dSemana.AddDays(7);
            }

            // Se agrega la fila de margen bruto
            int iFilaMargen = this.dgvDatos.Rows.Add("Margen Bruto");
            this.dgvDatos.Rows[iFilaMargen].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaMargen].Value = (Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaIngresos].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaCostos].Value));
            }

            // Se obtienen los datos para los gastos
            oParams.Clear();
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);
            var oGastos = General.ExecuteProcedure<pauContaCuentasPorSemana_Result>("pauContaCuentasPorSemana", oParams);
            var oGastosSem = oGastos.GroupBy(c => new { Semana = UtilLocal.SemanaSabAVie(c.Fecha), c.Sucursal })
                .Select(c => new { c.Key.Semana, c.Key.Sucursal, Importe = c.Sum(s => s.ImporteDev) })
                .OrderBy(c => c.Sucursal).ThenBy(c => c.Semana);

            // Se agrega la fila de los Gastos
            int iFilaGastos = this.dgvDatos.Rows.Add("- Gastos");
            this.dgvDatos.Rows[iFilaGastos].DefaultCellStyle.Font = oFuenteT;
            // Se llenan los ingresos
            dSemana = UtilLocal.InicioSemanaSabAVie(dDesde);
            sSucursal = "";
            foreach (var oReg in oGastosSem)
            {
                if (sSucursal != oReg.Sucursal)
                {
                    sSucursal = oReg.Sucursal;
                    iFila = this.dgvDatos.Rows.Add(sSucursal);
                    dSemana = UtilLocal.InicioSemanaSabAVie(dDesde);
                }

                string sSemana = dSemana.ToShortDateString();
                if (!this.dgvDatos.Columns.Contains(sSemana))
                {
                    this.dgvDatos.Columns.Add(sSemana, sSemana);
                    this.dgvDatos.Columns[sSemana].FormatoMoneda();
                }

                this.dgvDatos[sSemana, iFila].Value = oReg.Importe;
                this.dgvDatos[sSemana, iFilaGastos].Value = (Helper.ConvertirDecimal(this.dgvDatos[sSemana, iFilaGastos].Value) + oReg.Importe);
                dSemana = dSemana.AddDays(7);
            }

            // Se agrega la fila de utilidad neta
            int iFilaUtilidad = this.dgvDatos.Rows.Add("Utilidad neta");
            this.dgvDatos.Rows[iFilaUtilidad].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;
                this.dgvDatos[oCol.Index, iFilaUtilidad].Value = (Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaMargen].Value)
                    - Helper.ConvertirDecimal(this.dgvDatos[oCol.Index, iFilaGastos].Value));
            }

            Cargando.Cerrar();
        }

        #endregion

    }
}
