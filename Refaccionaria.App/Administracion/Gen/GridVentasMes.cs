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
            
            int iAnio = DateTime.Now.Year;
            int iAnio3 = iAnio - 3;
            int iAnio2 = iAnio - 2;
            int iAnio1 = iAnio - 1;

            DateTime dDesde = new DateTime(iAnio3, 1, 1);
            DateTime dHasta = DateTime.Now.AddMonths(-1).DiaUltimo();         
                     
            var oParams = new Dictionary<string, object>();
            oParams.Add("ParteID", iParteID);
            oParams.Add("Desde", dDesde);
            oParams.Add("Hasta", dHasta);
            var oDatos = General.ExecuteProcedure<pauParteVentasPorMes_Result>("pauParteVentasPorMes", oParams);           
                        
            // Se configuran las columnas
            string sYear = Convert.ToString(iAnio3);
            for (int iCol = 1; iCol <= 3; iCol++)
            {
                this.dgvDatos.Columns[iCol].Name = (sYear);
                this.dgvDatos.Columns[iCol].HeaderText = (sYear);

                int sumaYear = int.Parse(sYear)+1;
                sYear = Convert.ToString(sumaYear);
            }

            DateTime dMes = DateTime.Now.AddMonths(-12).DiaPrimero();;
            for (int iCol = 4; iCol <= 15; iCol++)
            {
                this.dgvDatos.Columns[iCol].Name = (dMes.Year.ToString() + dMes.Month.ToString());
                this.dgvDatos.Columns[iCol].HeaderText = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames[dMes.Month - 1];               

                dMes = dMes.AddMonths(1);
            }

            // Se llenan los datos          
            int iSucursalID = 0;
            int iFila = 0;
            int tFila = dgvDatos.Rows.Add();
            decimal mTotal = 0, mTotalNegados = 0;
            this.dgvDatos.Rows.Clear();
            string sMes;
           

            foreach (var oReg in oDatos)
            {
                if (iSucursalID != oReg.SucursalID)
                {
                    iSucursalID = oReg.SucursalID;
                    iFila = this.dgvDatos.Rows.Add(oReg.Sucursal);

                    mTotalNegados = mTotal = 0;
                }

                if (oReg.Anio < iAnio)
                {
                    string syear = oReg.Anio.Valor().ToString();
                    this.dgvDatos[syear, iFila].Value = (Helper.ConvertirDecimal(this.dgvDatos[syear, iFila].Value)
                        + oReg.Cantidad.Valor());
                }

                string sCol = oReg.Anio.ToString() + oReg.Mes.ToString();
                if (!this.dgvDatos.Columns.Contains(sCol))
                    continue;                
                   
                sMes = (oReg.Anio.ToString() + oReg.Mes.ToString());
                this.dgvDatos[sMes, iFila].Value = string.Format("{0:N0}/{1:N0}", oReg.Cantidad, oReg.Negado);

                mTotal += oReg.Cantidad.Valor();
                mTotalNegados = oReg.Negado.Valor();
                this.dgvDatos["Total", iFila].Value = string.Format("{0:N0}/{1:N0}", mTotal, mTotalNegados);
            }

            iFila = this.dgvDatos.Rows.Add("Total");         
            for (int iCol = 1; iCol < this.dgvDatos.Columns.Count; iCol++)
            {
                decimal mCantidad = 0, mNegado = 0;
                int iColumna = Helper.ConvertirEntero(this.dgvDatos.Columns[iCol].Name);
                
                foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
                {                                              
					string sValor = Helper.ConvertirCadena(oFila.Cells[iCol].Value);                        
					var aValores = sValor.Split('/');
					if (aValores.Length > 0)
					{
						mCantidad += Helper.ConvertirDecimal(aValores[0]);
					}
					if (aValores.Length > 1)
					{
						mNegado += Helper.ConvertirDecimal(aValores[1]);
					}
                }
                if (iColumna.Equals(iAnio1) || iColumna.Equals(iAnio2) || iColumna.Equals(iAnio3))
                {
                    this.dgvDatos[iCol, iFila].Value = string.Format("{0:N0}", mCantidad);
                }else{
                    this.dgvDatos[iCol, iFila].Value = string.Format("{0:N0}/{1:N0}", mCantidad, mNegado);
                }
            }

        }

        public void LimpiarDatos()
        {
            this.dgvDatos.Rows.Clear();
        }

        #endregion
    }
}
