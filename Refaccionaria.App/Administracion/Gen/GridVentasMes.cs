using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;

using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class GridVentasMes : UserControl
    {
        public GridVentasMes()
        {
            InitializeComponent();
        }

        #region [ Eventos ]
        #endregion

        #region [ Métodos ]
        #endregion

        #region [ Públicos ]

        public void LlenarDatos(int iParteID)
        {
            DateTime dDesde = DateTime.Now.AddYears(-1).DiaPrimero();
            DateTime dHasta = DateTime.Now.DiaUltimo();

            var oParams = new Dictionary<string, object>();
            oParams.Add("ParteID", iParteID);
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);
            var oDatos = General.ExecuteProcedure<pauParteVentasPorMes_Result>("pauParteVentasPorMes", oParams);

            // Se configuran las columnas
            DateTime dMes = dDesde;
            for (int iCol = 1; iCol <= 12; iCol++) {
                this.dgvDatos.Columns[iCol].Name = (dMes.Year.ToString() + dMes.Month.ToString());
                this.dgvDatos.Columns[iCol].HeaderText = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames[dMes.Month];
                dMes = dMes.AddMonths(1);
            }

            // Se llenan los datos
            int iSucursalID = 0;
            int iFila = 0;
            decimal mTotal = 0, mTotalNegados = 0;
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                if (iSucursalID != oReg.SucursalID)
                {
                    iSucursalID = oReg.SucursalID;
                    iFila = this.dgvDatos.Rows.Add(oReg.Sucursal);
                    mTotalNegados = mTotal = 0;
                }

                string sMes = (oReg.Anio.ToString() + oReg.Mes.ToString());
                this.dgvDatos[sMes, iFila].Value = string.Format("{0} / {1}", oReg.Cantidad, oReg.Negado);
                mTotal += oReg.Cantidad.Valor();
                mTotalNegados = oReg.Negado.Valor();
                this.dgvDatos["Total", iFila].Value = string.Format("{0} / {1}", mTotal, mTotalNegados);
            }
        }

        public void LimpiarDatos()
        {
            this.dgvDatos.Rows.Clear();
        }

        #endregion
    }
}
