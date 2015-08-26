using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using AdvancedDataGridView;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CuentasPorSemana : UserControl
    {
        private class DetalleSem
        {
            public string Observacion { get; set; }
            public decimal Total { get; set; }
            public string Sucursal { get; set; }
            public string FormaDePago { get; set; }
            public string Usuario { get; set; }
            public DateTime FechaDev { get; set; }
            public decimal Porcentaje { get; set; }
            public decimal Importe { get; set; }
        }

        public CuentasPorSemana()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuentasPorSemana_Load(object sender, EventArgs e)
        {
            // Se agregan las columnas variables
            DateTime dAhora = DateTime.Now;
            int iAnio = dAhora.Year;
            var oFechas = UtilDatos.FechasDeComisiones(new DateTime(iAnio, 1, 1));
            DateTime dIni = oFechas.Valor1;
            int iColSem = 0;
            while (dIni.Year <= iAnio)
            {
                string sEnc = string.Format("{0}\n{1}", dIni.ToString("dd/MMM"), dIni.AddDays(6).ToString("dd/MMM"));
                var oCol = new DataGridViewTextBoxColumn() { Name = ("Sem" + dIni.ToString("d")), HeaderText = sEnc, Width = 80 };
                oCol.FormatoMoneda();
                this.tgvDatos.Columns.Add(oCol);

                if (dAhora >= dIni && dAhora < dIni.AddDays(7))
                    iColSem = this.tgvDatos.Columns.Count;

                dIni = dIni.AddDays(7);
            }

            // this.tgvDatos.HorizontalScrollingOffset = this.tgvDatos.Columns[10].HeaderCell.ContentBounds.Left;
            this.tgvDatos.FirstDisplayedScrollingColumnIndex = (iColSem - 1);
            
            // Se cargan los datos
            this.CargarDatos();
        }

        private void tgvDatos_CurrentCellChanged(object sender, EventArgs e)
        {
            this.LlenarDetalleSemana(this.tgvDatos.CurrentCell);
        }

        private void chkAfectaMetas_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkAfectaMetas.Focused)
                this.CargarDatos();
        }

        #endregion

        #region [ Métodos ]

        private void LlenarDetalleSemana(DataGridViewCell oCelda)
        {
            this.dgvDetalle.Rows.Clear();

            if (oCelda == null || this.dgvDetalle.Columns.Count <= 0) return;
            var oLista = (oCelda.Tag as List<int>);
            if (oLista == null) return;
            
            foreach (int iId in oLista)
            {
                var oEgresoDev = General.GetEntity<ContaEgresoDevengado>(c => c.ContaEgresoDevengadoID == iId);
                var oEgresoV = General.GetEntity<ContaEgresosView>(c => c.ContaEgresoID == oEgresoDev.ContaEgresoID);
                if (oEgresoV != null)
                {
                    // this.dgvDetalle.Rows.Add(oEgresoV.Observaciones, oEgresoV.Importe, oEgresoV.Sucursal, oEgresoV.FormaDePago, oEgresoV.Usuario
                    //    , oEgresoDev.Fecha, ((oEgresoDev.Importe / oEgresoV.Importe) * 100), oEgresoDev.Importe);
                    this.dgvDetalle.Rows.Add(oEgresoDev.Fecha, oEgresoV.Sucursal, oEgresoV.FormaDePago, oEgresoV.Usuario, oEgresoV.Importe
                        , ((oEgresoDev.Importe / oEgresoV.Importe) * 100), oEgresoDev.Importe, oEgresoV.Observaciones);
                }
            }
        }

        private void PonerGuionCeldas(TreeGridNode oNodo)
        {
            foreach (DataGridViewCell oCelda in oNodo.Cells)
            {
                if (oCelda.Value != null && oCelda.Value.ToString() == "0")
                    oCelda.Value = "-";
            }
        }

        #endregion

        #region [ Públicos ]
        
        public void CargarDatos()
        {
            Cargando.Mostrar();

            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", new DateTime(DateTime.Today.Year, 1, 1));
            oParams.Add("Hasta", new DateTime(DateTime.Today.Year, 12, 31));
            oParams.Add("AfectaMetas", this.chkAfectaMetas.Checked);

            // Se llenan los datos
            var oDatos = General.ExecuteProcedure<pauContaCuentasPorSemana_Result>("pauContaCuentasPorSemana", oParams);
            this.tgvDatos.Nodes.Clear();
            TreeGridNode oNodoSucursal = null, oNodoCuenta = null, oNodoSubcuenta = null, oNodoCuentaDeMayor = null, oNodoCuentaAuxiliar = null;
            string sSucursal = "", sCuenta = "", sSubcuenta = "", sCuentaDeMayor = "", sCuentaAuxiliar = "";
            foreach (var oReg in oDatos)
            {
                // Nodo de Sucursal
                if (oReg.Sucursal != sSucursal)
                {
                    sSucursal = oReg.Sucursal;
                    oNodoSucursal = this.tgvDatos.Nodes.Add(sSucursal);
                    sCuenta = "";
                }
                // Nodo de Cuenta
                if (oReg.Cuenta != sCuenta)
                {
                    sCuenta = oReg.Cuenta;
                    oNodoCuenta = oNodoSucursal.Nodes.Add(sCuenta);
                    sSubcuenta = "";
                }
                // Nodo de Subcuenta
                if (oReg.Subcuenta != sSubcuenta)
                {
                    sSubcuenta = oReg.Subcuenta;
                    oNodoSubcuenta = oNodoCuenta.Nodes.Add(sSubcuenta);
                    sCuentaDeMayor = "";
                }
                // Nodo de Cuenta de mayor
                if (oReg.CuentaDeMayor != sCuentaDeMayor)
                {
                    sCuentaDeMayor = oReg.CuentaDeMayor;
                    oNodoCuentaDeMayor = oNodoSubcuenta.Nodes.Add(sCuentaDeMayor);
                    sCuentaAuxiliar = "";
                }
                // Se agrega la cuenta auxiliar
                if (oReg.CuentaAuxiliar != sCuentaAuxiliar)
                {
                    sCuentaAuxiliar = oReg.CuentaAuxiliar;
                    oNodoCuentaAuxiliar = oNodoCuentaDeMayor.Nodes.Add(oReg.CuentaAuxiliar);
                }
                // Se meten los valores de las semanas, y los totales
                if (oReg.PeriodicidadMes.HasValue)
                {
                    DateTime dInicioPer = oReg.Fecha.DiaPrimero().Date;
                    DateTime dFinPer = dInicioPer.AddMonths(oReg.PeriodicidadMes.Valor()).AddDays(-1);
                    decimal mImporteDiario = (oReg.ImporteDev.Valor() / ((dFinPer - dInicioPer).Days + 1));
                    decimal mImporte; int iDias;
                    DateTime dIniSem = UtilLocal.InicioSemanaSabAVie(dInicioPer).Date;
                    for (int iCol = (this.tgvDatos.Columns["Sem" + dIniSem.ToString("d")].Index); iCol < this.tgvDatos.Columns.Count; iCol++)
                    {
                        // Se verifica si se debe de seguir semanalizando
                        if (oReg.FinSemanalizar.HasValue && oReg.FinSemanalizar <= dIniSem)
                            break;
                        // Se verifica la fecha final, 
                        if (oNodoCuentaAuxiliar.Tag != null && dIniSem > dFinPer)
                            break;

                        // Se calcula el importe correspondiente
                        DateTime dFinSem = dIniSem.AddDays(6);
                        if (dIniSem < dInicioPer)
                            iDias = dFinSem.Day;
                        else if (dIniSem <= dFinPer && dFinSem > dFinPer)
                            iDias = ((dIniSem.DiaUltimo().Day - dIniSem.Day) + 1);
                        else if (dIniSem > dFinPer && (dIniSem - dFinPer).Days < 7)
                        {
                            iDias = (dIniSem.Day - 1);
                            iCol--;
                        }
                        else
                        {
                            iDias = 7;
                            // Se verifica si es 
                            /* if (oReg.CuentaAuxiliar == sCuentaAuxiliar)
                            {
                                oNodoCuentaAuxiliar.Cells[iCol].Value = 0;
                                oNodoCuentaAuxiliar.Cells[iCol].Tag = null;
                            } */
                        }
                        // int iDias = (dIniSem < dInicioPer ? dFinSem.Day : (dFinSem > dFinPer ? ((dIniSem.DiaUltimo().Day - dIniSem.Day) + 1) : 7));
                        mImporte = (mImporteDiario * iDias);
                        dIniSem = dIniSem.AddDays(7);

                        // Para guardar los datos relacionados
                        if (oNodoCuentaAuxiliar.Cells[iCol].Tag == null)
                            oNodoCuentaAuxiliar.Cells[iCol].Tag = new List<int>();
                        (oNodoCuentaAuxiliar.Cells[iCol].Tag as List<int>).Add(oReg.ContaEgresoDevengadoID);

                        // Para llenar las celdas
                        oNodoCuentaAuxiliar.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuentaAuxiliar.Cells[iCol].Value) + mImporte); // mImporte;

                        /*
                        // Para los niveles superiores
                        oNodoCuentaDeMayor.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuentaDeMayor.Cells[iCol].Value) + mImporte);
                        oNodoSubcuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoSubcuenta.Cells[iCol].Value) + mImporte);
                        oNodoCuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuenta.Cells[iCol].Value) + mImporte);
                        oNodoSucursal.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoSucursal.Cells[iCol].Value) + mImporte);

                        // Para la columna de totales
                        oNodoCuentaAuxiliar.Cells[1].Value = (Helper.ConvertirDecimal(oNodoCuentaAuxiliar.Cells[1].Value) + mImporte); // mImporteTotal;
                        oNodoCuentaDeMayor.Cells[1].Value = (Helper.ConvertirDecimal(oNodoCuentaDeMayor.Cells[1].Value) + mImporte);
                        oNodoSubcuenta.Cells[1].Value = (Helper.ConvertirDecimal(oNodoSubcuenta.Cells[1].Value) + mImporte);
                        oNodoCuenta.Cells[1].Value = (Helper.ConvertirDecimal(oNodoCuenta.Cells[1].Value) + mImporte);
                        oNodoSucursal.Cells[1].Value = (Helper.ConvertirDecimal(oNodoSucursal.Cells[1].Value) + mImporte);
                        */
                    }

                    // Se marca la cuenta, para que ya no se semanalice hasta el final
                    if (oNodoCuentaAuxiliar.Tag == null)
                        oNodoCuentaAuxiliar.Tag = true;
                }
                else
                {
                    DateTime dIniSem = UtilLocal.InicioSemanaSabAVie(oReg.Fecha).Date;
                    int iCol = this.tgvDatos.Columns["Sem" + dIniSem.ToString("d")].Index;
                    // Para guardar los datos relacionados
                    if (oNodoCuentaAuxiliar.Cells[iCol].Tag == null)
                        oNodoCuentaAuxiliar.Cells[iCol].Tag = new List<int>();
                    (oNodoCuentaAuxiliar.Cells[iCol].Tag as List<int>).Add(oReg.ContaEgresoDevengadoID);
                    // Para llenar el importe
                    oNodoCuentaAuxiliar.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodoCuentaAuxiliar.Cells[iCol].Value) + oReg.ImporteDev);
                }
            }

            // Se llenan los totales
            foreach (var oNodSucursal in this.tgvDatos.Nodes)
            {
                foreach (var oNodCuenta in oNodSucursal.Nodes)
                {
                    foreach (var oNodSubcuenta in oNodCuenta.Nodes)
                    {
                        foreach (var oNodCuentaDeMayor in oNodSubcuenta.Nodes)
                        {
                            foreach (var oNodCuentaAuxiliar in oNodCuentaDeMayor.Nodes)
                            {
                                for (int iCol = 2; iCol < this.tgvDatos.Columns.Count; iCol++)
                                {
                                    decimal mImporte = Helper.ConvertirDecimal(oNodCuentaAuxiliar.Cells[iCol].Value);
                                    // Para los niveles superiores
                                    oNodCuentaDeMayor.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodCuentaDeMayor.Cells[iCol].Value) + mImporte);
                                    oNodSubcuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodSubcuenta.Cells[iCol].Value) + mImporte);
                                    oNodCuenta.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodCuenta.Cells[iCol].Value) + mImporte);
                                    oNodSucursal.Cells[iCol].Value = (Helper.ConvertirDecimal(oNodSucursal.Cells[iCol].Value) + mImporte);
                                    // Para la columna de totales
                                    oNodCuentaAuxiliar.Cells[1].Value = (Helper.ConvertirDecimal(oNodCuentaAuxiliar.Cells[1].Value) + mImporte); // mImporteTotal;
                                    oNodCuentaDeMayor.Cells[1].Value = (Helper.ConvertirDecimal(oNodCuentaDeMayor.Cells[1].Value) + mImporte);
                                    oNodSubcuenta.Cells[1].Value = (Helper.ConvertirDecimal(oNodSubcuenta.Cells[1].Value) + mImporte);
                                    oNodCuenta.Cells[1].Value = (Helper.ConvertirDecimal(oNodCuenta.Cells[1].Value) + mImporte);
                                    oNodSucursal.Cells[1].Value = (Helper.ConvertirDecimal(oNodSucursal.Cells[1].Value) + mImporte);
                                }
                            }
                        }
                    }
                }
            }

            // Se aplica el formato y el color
            foreach (var oNodSucursal in this.tgvDatos.Nodes)
            {
                // Se aplica el formato
                oNodSucursal.DefaultCellStyle.BackColor = Color.FromArgb(58, 79, 109);
                oNodSucursal.DefaultCellStyle.ForeColor = Color.White;
                oNodSucursal.DefaultCellStyle.Font = new Font(this.tgvDatos.Font, FontStyle.Bold);
                //
                oNodSucursal.Expand();
                // Si su valor es cero, se pone un guión
                this.PonerGuionCeldas(oNodSucursal);
                foreach (var oNodCuenta in oNodSucursal.Nodes)
                {
                    // Se aplica el formato
                    oNodCuenta.DefaultCellStyle.BackColor = Color.FromArgb(67, 87, 123);
                    oNodCuenta.DefaultCellStyle.ForeColor = Color.White;
                    oNodCuenta.DefaultCellStyle.Font = new Font(this.tgvDatos.Font, FontStyle.Bold);
                    oNodCuenta.DefaultCellStyle.Font = new Font(this.tgvDatos.Font.FontFamily, (float)7.75);
                    //
                    oNodCuenta.Expand();
                    // Si su valor es cero, se pone un guión
                    this.PonerGuionCeldas(oNodCuenta);
                    foreach (var oNodSubcuenta in oNodCuenta.Nodes)
                    {
                        // Se aplica el formato
                        oNodSubcuenta.DefaultCellStyle.BackColor = Color.FromArgb(0, 112, 192);
                        oNodSubcuenta.DefaultCellStyle.ForeColor = Color.White;
                        //
                        oNodSubcuenta.Expand();
                        // Si su valor es cero, se pone un guión
                        this.PonerGuionCeldas(oNodSubcuenta);
                        foreach (var oNodCuentaDeMayor in oNodSubcuenta.Nodes)
                        {
                            // Se aplica el formato
                            oNodCuentaDeMayor.DefaultCellStyle.ForeColor = Color.FromArgb(58, 79, 109);
                            oNodCuentaDeMayor.DefaultCellStyle.Font = new Font(this.tgvDatos.Font, FontStyle.Bold);
                            oNodCuentaDeMayor.DefaultCellStyle.Font = new Font(this.tgvDatos.Font.FontFamily, (float)7.75);
                            // Si su valor es cero, se pone un guión
                            this.PonerGuionCeldas(oNodCuentaDeMayor);
                            foreach (var oNodCuentaAuxiliar in oNodCuentaDeMayor.Nodes)
                            {
                                oNodCuentaAuxiliar.DefaultCellStyle.ForeColor = Color.FromArgb(58, 79, 109);
                                // Si su valor es cero, se pone un guión
                                this.PonerGuionCeldas(oNodCuentaAuxiliar);
                            }
                        }
                    }
                }
            }

            Cargando.Cerrar();
        }
        
        #endregion
                                
    }
}
