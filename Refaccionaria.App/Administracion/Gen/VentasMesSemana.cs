using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class VentasMesSemana : UserControl
    {
        DateTime Desde, Hasta;
        Dictionary<int, int> MesesEtiquetas = new Dictionary<int, int>();
        public VentasMesSemana()
        {
            InitializeComponent();
            this.Hasta = DateTime.Now.Date.DiaPrimero().AddDays(-1);
            this.Desde = this.Hasta.AddYears(-1).AddDays(1);
            LimpiarDatosExtra();
            // Se llenan las etiquetas de los meses
            for (int iCont = 0; iCont < 12; iCont++)
            {
                DateTime dMes = this.Hasta.AddMonths(iCont * -1);
                string sEtiqueta = string.Format("lblEtMes{0:00}", (12 - iCont));
                this.Controls[sEtiqueta].Text =
                    CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dMes.Month).ToUpper()
                    + dMes.Year.ToString().Derecha(2);
                this.MesesEtiquetas[dMes.Month] = (12 - iCont);
            }

        }

        public void LlenarDatosExtra(int iParteID, int iSucursalID)
        {
            this.LimpiarDatosExtra();
            //int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            
            if (iParteID <= 0) return;

            // Se obtiene los datos de Abc
            var oParteAbc = General.GetEntity<ParteAbc>(q => q.ParteID == iParteID);
            if (oParteAbc != null)
            {
                this.lblAbcProveedor.Text = oParteAbc.AbcDeProveedor;
                this.lblAbcLinea.Text = oParteAbc.AbcDeLinea;
            }
            // Se llena el costo
            var oPartePrecio = General.GetEntity<PartePrecio>(q => q.ParteID == iParteID && q.Estatus);
            this.lblCosto.Text = oPartePrecio.Costo.Valor().ToString(GlobalClass.FormatoMoneda);
            // Se obtiene el dato de si es ventas globales o no
            var oParteMaxMin = General.GetEntity<ParteMaxMin>(q => q.SucursalID == iSucursalID && q.ParteID == iParteID);
            bool bVentasGlobales = (oParteMaxMin != null && oParteMaxMin.VentasGlobales.Valor());
            this.lblVentasGlobales.Visible = bVentasGlobales;

            // Se llenan los datos calculados
            var oParams = new Dictionary<string, object>();
            oParams.Add("ParteID", iParteID);
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);
            oParams.Add("SucursalID", iSucursalID);

            var oDatos = General.ExecuteProcedure<pauParteMaxMinDatosExtra_Result>("pauParteMaxMinDatosExtra", oParams);

            // Se llenan los datos, según la sucursal, si hubo resultados
            if (oDatos.Count > 1)
            {
                this.lblVentaMayorDia.Text = oDatos[0].Cantidad.Valor().ToString();
                this.lblVentaMenorDia.Text = oDatos[1].Cantidad.Valor().ToString();
                this.lblSemanasConVenta.Text = oDatos.Where(q => q.Grupo == 3).Count().ToString();
                // Coloración
                this.lblVentaMayorDia.ForeColor = this.DatosExtraColoracion(oDatos[0].Cantidad, oDatos[0].Negadas);
                this.lblVentaMenorDia.ForeColor = this.DatosExtraColoracion(oDatos[1].Cantidad, oDatos[1].Negadas);
                this.lblSemanasConVenta.ForeColor = this.DatosExtraColoracion(oDatos.Where(q => q.Grupo == 3).Count(), oDatos.Where(q => q.Grupo == 3 && q.Negadas > 0).Count());
                // Fin - Coloración

                // Se llenan los meses
                foreach (var oDato in oDatos)
                {
                    if (oDato.Grupo == 4)
                    {
                        this.Controls[string.Format("lblMes{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].Text = oDato.Cantidad.Valor().ToString();
                        this.Controls[string.Format("lblMes{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].ForeColor =
                            this.DatosExtraColoracion(oDato.Cantidad, oDato.Negadas);
                    }
                }
                // Se llenan los indicadores de los meses
                var oMeses = oDatos.Where(q => q.Grupo == 4).OrderByDescending(q => q.Cantidad);
                int iCuentaMes = 1;
                foreach (var oMes in oMeses)
                {
                    string sEtiqueta = string.Format("lblInMes{0:00}", this.MesesEtiquetas[oMes.Periodo.Valor()]);
                    this.Controls[sEtiqueta].Text = (iCuentaMes > 6 ? "<" : ">");
                    this.Controls[sEtiqueta].ForeColor = (iCuentaMes > 6 ? Color.Olive : Color.DarkRed);
                    iCuentaMes++;
                }

                // Se llenan las semanas
                var oSemanas = oDatos.Where(q => q.Grupo == 3).OrderByDescending(q => q.Cantidad).ToList();
                for (int iCont = 0; (iCont < 26 && iCont < oSemanas.Count); iCont++)
                {
                    this.Controls[string.Format("lblSem{0:00}", iCont + 1)].Text = oSemanas[iCont].Cantidad.Valor().ToString();
                    this.Controls[string.Format("lblSem{0:00}", iCont + 1)].ForeColor = this.DatosExtraColoracion(oSemanas[iCont].Cantidad, oSemanas[iCont].Negadas);
                }
            }

            // Se llenan los datos globales, si aplica
            if (bVentasGlobales)
            {
                oParams.Remove("SucursalID");
                oDatos = General.ExecuteProcedure<pauParteMaxMinDatosExtra_Result>("pauParteMaxMinDatosExtra", oParams);
                foreach (var oDato in oDatos)
                {
                    if (oDato.Grupo == 4)
                    {
                        this.Controls[string.Format("lblMesG{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].Text = oDato.Cantidad.Valor().ToString();
                        this.Controls[string.Format("lblMesG{0:00}", this.MesesEtiquetas[oDato.Periodo.Valor()])].ForeColor =
                            this.DatosExtraColoracion(oDato.Cantidad, oDato.Negadas);
                    }
                }
            }
        }

        private Color DatosExtraColoracion(decimal? mCantidad, decimal? mNegadas)
        {
            decimal? mDiferencia = (mCantidad - mNegadas);

            if (mDiferencia == mCantidad)
                return Color.White;
            else if (mDiferencia == 0)
                return Color.Tomato;
            else
                return Color.LightSkyBlue;
        }

        private void LimpiarDatosExtra()
        {
            this.lblAbcProveedor.Text = "";
            this.lblAbcLinea.Text = "";
            this.lblCosto.Text = "";
            this.lblVentaMayorDia.Text = "";
            this.lblVentaMenorDia.Text = "";
            this.lblSemanasConVenta.Text = "";
            this.lblVentasGlobales.Visible = false;

            foreach (Control oControl in this.Controls)
            {
                if (oControl.Name.Contains("lblInMes"))
                    oControl.Text = "";
                else if (oControl.Name.Contains("lblMes") || oControl.Name.Contains("lblSem"))
                    oControl.Text = "0";
            }

            // Coloración
            foreach (Control oControl in this.Controls)
            {
                if (oControl.Name.Contains("lblVentaM") || oControl.Name == "lblSemanasConVenta")
                    oControl.ForeColor = Color.Black;
                else if (oControl.Name.Contains("lblMes") || oControl.Name.Contains("lblSem"))
                    oControl.ForeColor = Color.White;
            }
            // Fin - Coloración
        }
    }
}
