﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class ContaFlujoDeEfectivo : UserControl
    {
        int iColumnasFijas;

        public ContaFlujoDeEfectivo()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void ContaFlujoDeEfectivo_Load(object sender, EventArgs e)
        {
            // Se inicializan los controles
            this.cmbAnio.Items.AddRange(new object[] { 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020 });
            this.cmbAnio.Text = DateTime.Now.Year.ToString();
            // this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));

            this.iColumnasFijas = this.dgvDatos.Columns.Count;
        }

        private void nudDecimales_ValueChanged(object sender, EventArgs e)
        {
            if (this.nudDecimales.Focused)
                this.FormatoColumnas();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        #endregion
        
        #region [ Privados ]

        private void LlenarColumnasAnio(int iAnio)
        {
            // Se borran las columnas de semanas
            for (int iCol = (this.dgvDatos.Columns.Count - 1); iCol >= this.iColumnasFijas; iCol--)
                this.dgvDatos.Columns.RemoveAt(iCol);

            // Se agregan las nuevas columnas
            DateTime dDia = new DateTime(iAnio, DateTime.Now.Month, DateTime.Now.Day);
            DateTime dIni = UtilTheos.InicioSemanaSabAVie(new DateTime(iAnio, 1, 1));
            int iColSem = 0;
            while (dIni.Year <= iAnio)
            {
                string sEnc = string.Format("{0}\n{1}", dIni.ToString("dd/MMM"), dIni.AddDays(6).ToString("dd/MMM"));
                var oCol = new DataGridViewTextBoxColumn() { Name = dIni.ToShortDateString(), HeaderText = sEnc };
                oCol.FormatoMoneda();
                this.dgvDatos.Columns.Add(oCol);

                if (dDia >= dIni && dDia < dIni.AddDays(7))
                    iColSem = this.dgvDatos.Columns.Count;

                dIni = dIni.AddDays(7);
            }

            this.dgvDatos.FirstDisplayedScrollingColumnIndex = (iColSem - 1);

            this.FormatoColumnas();
        }

        private void FormatoColumnas()
        {
            string sFormato = ("C" + this.nudDecimales.Value.ToString());
            for (int iCol = 1; iCol < this.dgvDatos.Columns.Count; iCol++)
                this.dgvDatos.Columns[iCol].DefaultCellStyle.Format = sFormato;
        }

        class GastoSem
        {
            public DateTime Semana { get; set; }
            public string Grupo { get; set; }
            public decimal Importe { get; set; }
        }
        private void CargarDatos()
        {
            Cargando.Mostrar();

            var oFuenteT = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);

            // Se limpian los datos
            this.dgvDatos.Rows.Clear();
            foreach (var oSerie in this.chrPorSemana.Series)
                oSerie.Points.Clear();

            // Se llenan las columnas del año
            int iAnio = Util.Entero(this.cmbAnio.Text);
            this.LlenarColumnasAnio(iAnio);

            // Se agrega la fila de ingresos
            int iFilaIngresos = this.dgvDatos.Rows.Add("+ Ingresos");
            this.dgvDatos.Rows[iFilaIngresos].DefaultCellStyle.Font = oFuenteT;
            decimal mTotal = 0, mPromedio = 0;

            // Se agrega la fila de saldo inicial
            int iFilaSaldoInicial = this.dgvDatos.Rows.Add("Saldo inicial");

            // Se obtienen los datos para las ventas
            // int iSucursalID = Util.ConvertirEntero(this.cmbSucursal.SelectedValue);
            DateTime dDesde = new DateTime(iAnio, 1, 1);
            DateTime dHasta = new DateTime(iAnio, 12, 31);
            DateTime dDesdeSemUno = UtilTheos.InicioSemanaSabAVie(dDesde);
            var oParams = new Dictionary<string, object>();
            // oParams.Add("SucursalID", (iSucursalID == 0 ? null : (int?)iSucursalID));
            oParams.Add("Pagadas", true);
            oParams.Add("Cobradas", false);
            oParams.Add("Solo9500", false);
            oParams.Add("OmitirDomingo", false);
            oParams.Add("Desde", dDesdeSemUno);
            oParams.Add("Hasta", dHasta);
            var oDatos = Datos.ExecuteProcedure<pauCuadroDeControlGeneralNuevo_Result>("pauCuadroDeControlGeneral", oParams);
            var oSemanas = oDatos.Where(c => c.Fecha >= dDesdeSemUno)
                .GroupBy(c => new { Semana = UtilTheos.InicioSemanaSabAVie(c.Fecha) })
                .Select(c => new { c.Key.Semana, PrecioSinIva = c.Sum(s => s.PrecioSinIvaActual) })
                .OrderBy(c => c.Semana);
            mTotal += oSemanas.Sum(c => c.PrecioSinIva).Valor();
            mPromedio += oSemanas.Average(c => c.PrecioSinIva).Valor();
            // Se agrega la fila de ventas
            int iFila = this.dgvDatos.Rows.Add("Ventas", oSemanas.Sum(c => c.PrecioSinIva), oSemanas.Average(c => c.PrecioSinIva));
            foreach (var oReg in oSemanas)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Util.Decimal(this.dgvDatos[sSemana, iFila].Value) + oReg.PrecioSinIva);
                this.dgvDatos[sSemana, iFilaIngresos].Value = (Util.Decimal(this.dgvDatos[sSemana, iFilaIngresos].Value) + oReg.PrecioSinIva);
            }

            // Se obtienen datos para varios grupos
            DateTime dHastaMas1 = dHasta.AddDays(1);
            // DateTime dDesdeMas1 = dDesde.AddDays(1);
            var oReinversiones = Datos.GetListOf<ContaPolizasDetalleAvanzadoView>(c => c.FechaPoliza >= dDesdeSemUno && c.FechaPoliza < dHastaMas1
                && (
                    c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarLargoPlazo
                    || (c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo && c.ContaCuentaAuxiliarID != Cat.ContaCuentasAuxiliares.TarjetaDeCredito)
                    || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.AcreedoresDiversos
                    || c.ContaSubcuentaID == Cat.ContaSubcuentas.ActivoFijo
                )
            );

            // Se agrega la fila de préstamos
            var oPrestamos = oReinversiones.Where(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarLargoPlazo
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo)
                .GroupBy(c => UtilTheos.InicioSemanaSabAVie(c.FechaPoliza.Valor()))
                .Select(c => new { Semana = c.Key, Importe = c.Sum(s => s.Abono) });
            mTotal += oPrestamos.Sum(c => c.Importe);
            decimal mPromedioAct = (oPrestamos.Count() > 0 ? oPrestamos.Average(c => c.Importe) : 0);
            mPromedio += mPromedioAct;
            iFila = this.dgvDatos.Rows.Add("Préstamos", oPrestamos.Sum(c => c.Importe), mPromedioAct);
            foreach (var oReg in oPrestamos)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Util.Decimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaIngresos].Value = (Util.Decimal(this.dgvDatos[sSemana, iFilaIngresos].Value) + oReg.Importe);
            }

            // Se llenan los totales de ingresos
            this.dgvDatos["Total", iFilaIngresos].Value = mTotal;
            this.dgvDatos["Promedio", iFilaIngresos].Value = (mPromedio / 2);

            // Se agrega la fila de egresos
            int iFilaEgresos = this.dgvDatos.Rows.Add("- Egresos");
            this.dgvDatos.Rows[iFilaEgresos].DefaultCellStyle.Font = oFuenteT;
            mTotal = mPromedio = 0;
            
            // Se obtienen los datos para los gastos
            oParams.Clear();
            oParams.Add("Desde", dDesdeSemUno);
            oParams.Add("Hasta", dHasta);
            var oGastos = Datos.ExecuteProcedure<pauContaCuentasPorSemana_Result>("pauContaCuentasPorSemana", oParams);
            var oGastosSem = ContaProc.GastosSemanalizados(oGastos, Util.FechaHora(this.dgvDatos.Columns[this.dgvDatos.Columns.Count - 1].Name));
            oGastosSem = oGastosSem.Where(c => c.Semana >= dDesdeSemUno)
                .GroupBy(c => new { c.Semana }).Select(c => new ContaProc.GastoSem() { Semana = c.Key.Semana, Importe = c.Sum(s => s.Importe) }).ToList();
            mTotal += oGastosSem.Sum(c => c.Importe);
            mPromedio += oGastosSem.Average(c => c.Importe);
            // Se agrega la fila de los Gastos
            iFila = this.dgvDatos.Rows.Add("Gastos", oGastosSem.Sum(c => c.Importe), oGastosSem.Average(c => c.Importe));
            foreach (var oReg in oGastosSem)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Util.Decimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaEgresos].Value = (Util.Decimal(this.dgvDatos[sSemana, iFilaEgresos].Value) + oReg.Importe);
            }
            
            // Para las compras
            var oCompras = Datos.GetListOf<ProveedoresPolizasDetalleAvanzadoView>(c => c.Fecha >= dDesdeSemUno && c.Fecha < dHastaMas1
                && (c.OrigenID == Cat.OrigenesPagosAProveedores.PagoDirecto || c.OrigenID == Cat.OrigenesPagosAProveedores.PagoDeCaja))
                .GroupBy(c => new { Semana = UtilTheos.InicioSemanaSabAVie(c.Fecha.Valor()) })
                .Select(c => new { c.Key.Semana, Importe = c.Sum(s => s.Subtotal) })
                .OrderBy(c => c.Semana);
            mTotal += oCompras.Sum(c => c.Importe);
            mPromedioAct = (oCompras.Count() > 0 ? oCompras.Average(c => c.Importe) : 0);
            mPromedio += mPromedioAct;
            // Se agrega la fila de compras
            iFila = this.dgvDatos.Rows.Add("Compras", oCompras.Sum(c => c.Importe), mPromedioAct);
            foreach (var oReg in oCompras)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Util.Decimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaEgresos].Value = (Util.Decimal(this.dgvDatos[sSemana, iFilaEgresos].Value) + oReg.Importe);
            }

            // Para las deudas
            var oDeudas = oReinversiones.Where(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarLargoPlazo
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.CuentasPorPagarCortoPlazo || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.AcreedoresDiversos)
                .GroupBy(c => UtilTheos.InicioSemanaSabAVie(c.FechaPoliza.Valor()))
                .Select(c => new { Semana = c.Key, Importe = c.Sum(s => s.Cargo) });
            mTotal += oDeudas.Sum(c => c.Importe);
            mPromedioAct = (oDeudas.Count() > 0 ? oDeudas.Average(c => c.Importe) : 0);
            mPromedio += mPromedioAct;
            // Se agrega la fila de deudas
            iFila = this.dgvDatos.Rows.Add("Deudas", oDeudas.Sum(c => c.Importe), mPromedioAct);
            foreach (var oReg in oDeudas)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Util.Decimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaEgresos].Value = (Util.Decimal(this.dgvDatos[sSemana, iFilaEgresos].Value) + oReg.Importe);
            }

            // Para las inversiones
            var oInversiones = oReinversiones.Where(c => c.ContaSubcuentaID == Cat.ContaSubcuentas.ActivoFijo)
                .GroupBy(c => UtilTheos.InicioSemanaSabAVie(c.FechaPoliza.Valor()))
                .Select(c => new { Semana = c.Key, Importe = c.Sum(s => s.Cargo) });
            mTotal += oInversiones.Sum(c => c.Importe);
            mPromedioAct = (oInversiones.Count() > 0 ? oInversiones.Average(c => c.Importe) : 0);
            mPromedio += mPromedioAct;
            // Se agrega la fila de deudas
            iFila = this.dgvDatos.Rows.Add("Inversiones", oInversiones.Sum(c => c.Importe), mPromedioAct);
            foreach (var oReg in oInversiones)
            {
                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Util.Decimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaEgresos].Value = (Util.Decimal(this.dgvDatos[sSemana, iFilaEgresos].Value) + oReg.Importe);
            }

            // Para lo de Isidro y Don Isidro
            var oGastosSemEsp = ContaProc.GastosSemanalizados(Datos.GetListOf<ContaEgresosDevengadoEspecialCuentasView>(c => c.Fecha >= dDesdeSemUno && c.Fecha < dHastaMas1)
                , Util.FechaHora(this.dgvDatos.Columns[this.dgvDatos.Columns.Count - 1].Name));
            oGastosSemEsp = oGastosSemEsp.Where(c => c.Semana >= dDesdeSemUno).ToList();
            mTotal += oGastosSemEsp.Sum(c => c.Importe);
            mPromedioAct = (oGastosSemEsp.Count() > 0 ? oGastosSemEsp.Average(c => c.Importe) : 0);
            mPromedio += mPromedioAct;
            // Se agregan las filas
            string sDuenio = "";
            foreach (var oReg in oGastosSemEsp)
            {
                if (sDuenio != oReg.Grupo)
                {
                    sDuenio = oReg.Grupo;
                    iFila = this.dgvDatos.Rows.Add(sDuenio, oGastosSemEsp.Where(c => c.Grupo == sDuenio).Sum(c => c.Importe)
                        , oGastosSemEsp.Where(c => c.Grupo == sDuenio).Average(c => c.Importe));
                }

                string sSemana = oReg.Semana.ToShortDateString();
                this.dgvDatos[sSemana, iFila].Value = (Util.Decimal(this.dgvDatos[sSemana, iFila].Value) + oReg.Importe);
                this.dgvDatos[sSemana, iFilaEgresos].Value = (Util.Decimal(this.dgvDatos[sSemana, iFilaEgresos].Value) + oReg.Importe);
            }

            // Se llenan los totales de egresos
            this.dgvDatos["Total", iFilaEgresos].Value = mTotal;
            this.dgvDatos["Promedio", iFilaEgresos].Value = (mPromedio / 2);

            // Ajuste especial para el año 2015, se quitan los datos antes del 01 de Julio
            if (dDesde.Year == 2015)
            {
                DateTime dInicio = new DateTime(2015, 6, 1);
                foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
                {
                    mPromedio = 0;
                    for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
                    {
                        DateTime dSem = Util.FechaHora(this.dgvDatos.Columns[iCol].Name);
                        oFila.Cells["Total"].Value = 0;
                        if (dSem >= dInicio)
                        {
                            mPromedio++;
                            oFila.Cells["Total"].Value = (Util.Decimal(oFila.Cells["Total"].Value) + Util.Decimal(oFila.Cells[iCol].Value));
                        }
                        else
                        {
                            oFila.Cells[iCol].Value = null;
                        }
                    }
                    oFila.Cells["Promedio"].Value = (Util.Decimal(oFila.Cells["Total"].Value) / mPromedio);
                }
            }

            // Se agrega el saldo final
            int iFilaSaldo = this.dgvDatos.Rows.Add("= Saldo final");
            this.dgvDatos.Rows[iFilaSaldo].DefaultCellStyle.Font = oFuenteT;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
            {
                if (oCol.Index == 0) continue;

                // Se modifica el saldo inicial de la semana actual, y se suma a la fila de ingresos
                if (oCol.Index >= this.iColumnasFijas)
                {
                    this.dgvDatos[oCol.Index, iFilaIngresos].Value = (
                        Util.Decimal(this.dgvDatos[oCol.Index, iFilaIngresos].Value)
                        + Util.Decimal(this.dgvDatos[oCol.Index, iFilaSaldoInicial].Value)
                    );
                }

                decimal mSaldo = (
                    Util.Decimal(this.dgvDatos[oCol.Index, iFilaIngresos].Value)
                    - Util.Decimal(this.dgvDatos[oCol.Index, iFilaEgresos].Value)
                );
                this.dgvDatos[oCol.Index, iFilaSaldo].Value = mSaldo;

                // Se llena el saldo inicial de la siguiente semana, si hay
                if (oCol.Index >= this.iColumnasFijas && oCol.Index < (this.dgvDatos.Columns.Count - 1))
                    this.dgvDatos[oCol.Index + 1, iFilaSaldoInicial].Value = mSaldo;
            }

            // Se llena la gráfica, en base al grid ya cargado
            for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
            {
                this.chrPorSemana.Series["Ventas"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaIngresos + 1].Value));
                this.chrPorSemana.Series["Prestamos"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaIngresos + 2].Value));
                this.chrPorSemana.Series["Gastos"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaEgresos + 1].Value));
                this.chrPorSemana.Series["Compras"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaEgresos + 2].Value));
                this.chrPorSemana.Series["Deudas"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaEgresos + 3].Value));
                this.chrPorSemana.Series["Inversiones"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaEgresos + 4].Value));
                if (this.dgvDatos.Rows.Count > 10)
                {
                    this.chrPorSemana.Series["Isidro"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaEgresos + 5].Value));
                    this.chrPorSemana.Series["DonIsidro"].Points.AddY(Util.Decimal(this.dgvDatos[iCol, iFilaEgresos + 6].Value));
                }
            }

            Cargando.Cerrar();
        }

        #endregion
    }
}
