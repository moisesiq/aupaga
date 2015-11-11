using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using FastReport;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CapitalHumano : UserControl
    {
        ControlError ctlError = new ControlError();
        int iColumnasFijas = 0;
        bool bDomingo;

        List<EnteroCadena> oMeses;
        List<EnteroCadena> oBimestres;

        public CapitalHumano()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CapitalHumano_Load(object sender, EventArgs e)
        {
            this.iColumnasFijas = this.dgvDatos.Columns.Count;

            int iAnio = DateTime.Now.Year;
            this.nudAnio.Value = iAnio;
            // Se llenan los datos de las semanas
            this.LlenarSemanas();
            this.cmbSemana.SelectedValue = UtilLocal.InicioSemanaSabAVie(DateTime.Now).Date;
            // Se selecciona por predeterminado la semana anterior
            if (this.cmbSemana.SelectedIndex > 0 && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                this.cmbSemana.SelectedIndex -= 1;

            // Se llenan las cuentas bancarias
            this.cmbCuentaBancaria.CargarDatos("BancoCuentaID", "NombreDeCuenta", General.GetListOf<BancoCuenta>().OrderBy(c => c.NombreDeCuenta).ToList());
            this.cmbCuentaBancaria.SelectedValue = Cat.CuentasBancarias.Banamex;

            // Se llenan los controles de la pestaña impuestos
            this.cmbImpTipo.CargarDatos("ContaCuentaDeMayorID", "CuentaDeMayor", General.GetListOf<ContaCuentaDeMayor>(c => 
                c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Nomina2Por || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Imss
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Infonavit).OrderBy(c => c.CuentaDeMayor).ToList());
            this.cmbImpPeriodo.ValueMember = "Entero";
            this.cmbImpPeriodo.DisplayMember = "Cadena";
            this.cmbImpCuenta.CargarDatos("BancoCuentaID", "NombreDeCuenta", General.GetListOf<BancoCuenta>().OrderBy(c => c.NombreDeCuenta).ToList());
            this.cmbImpCuenta.SelectedValue = Cat.CuentasBancarias.Scotiabank;
            this.cmbImpFormaDePago.CargarDatos("TipoFormaPagoID", "NombreTipoFormaPago", General.GetListOf<TipoFormaPago>(c =>
                c.TipoFormaPagoID == Cat.FormasDePago.Cheque || c.TipoFormaPagoID == Cat.FormasDePago.Transferencia));
            // 
            this.oMeses = new List<EnteroCadena>();
            var oCadMeses = DateTimeFormatInfo.CurrentInfo.MonthNames;
            for (int iMes = 1; iMes <= 12; iMes++)
                this.oMeses.Add(new EnteroCadena(iMes, oCadMeses[iMes - 1].ToUpper()));
            this.oBimestres = new List<EnteroCadena>();
            oCadMeses = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames;
            for (int iMes = 1; iMes <= 12; iMes += 2)
                this.oBimestres.Add(new EnteroCadena(iMes, string.Format("{0}-{1}", oCadMeses[iMes - 1], oCadMeses[iMes]).ToUpper()));
        }

        #region [ Nómina ]

        private void nudAnio_ValueChanged(object sender, EventArgs e)
        {
            if (this.nudAnio.Focused)
                this.LlenarSemanas();
        }

        private void btnMostar_Click(object sender, EventArgs e)
        {
            this.dgvDatos.Columns["Bono"].ReadOnly = false;
            this.LlenarNomina();
        }

        private void btnDomingo_Click(object sender, EventArgs e)
        {
            // Se llenan los usuarios
            var oUsuarios = General.GetListOf<UsuariosNominaView>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oUsuarios)
                this.dgvDatos.Rows.Add(oReg.UsuarioID, oReg.SucursalID, false, oReg.Usuario, oReg.Sucursal);
            // Se deshabilita la escritura en columnas no editables para este modo
            this.dgvDatos.ReadOnly = false;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
                oCol.ReadOnly = true;
            this.dgvDatos.Columns["Sel"].ReadOnly = false;
            this.dgvDatos.Columns["Adicional"].ReadOnly = false;
            this.dgvDatos.Columns["Tickets"].ReadOnly = false;
            this.dgvDatos.Columns["Adelanto"].ReadOnly = false;
            this.dgvDatos.Columns["MinutosTarde"].ReadOnly = false;
            this.dgvDatos.Columns["Otros"].ReadOnly = false;
            this.dgvDatos.MostrarColumnas("Sel", "Usuario", "Sucursal", "Adicional", "Tickets", "Adelanto", "MinutosTarde", "Otros", "Liquido");
            this.btnGuardar.Enabled = true;
            // 
            this.bDomingo = true;
        }

        private void dgvDatos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.CellStyle.Format == "C2")
            {
                if (Helper.ConvertirDecimal(e.Value) == 0)
                    e.Value = null;
            }
        }

        private void dgvDatos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvDatos.VerDirtyStateChanged("Sel");
        }

        private void dgvDatos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            string sCol = this.dgvDatos.Columns[e.ColumnIndex].Name;
            if (sCol == "Sel" || sCol == "Bono" || sCol == "Adicional" || sCol == "Tickets" || sCol == "Adelanto" || sCol == "MinutosTarde" || sCol == "Otros")
            {
                this.CalcularTotales(this.dgvDatos.Rows[e.RowIndex]);
                this.CalcularReserva();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (UtilLocal.MensajePregunta("¿Estás seguro que deseas guardar la nómina seleccionada?") == DialogResult.Yes)
                this.AccionGuardar();
        }

        private void btnDesgloseLiquido_Click(object sender, EventArgs e)
        {
            this.GenerarDesgloseLiquido();
        }

        #endregion

        #region [ Impuestos ]

        private void cmbImpTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbImpTipo.Focused)
            {
                int iCuentaDeMayorID = Helper.ConvertirEntero(this.cmbImpTipo.SelectedValue);
                this.ImpCambiarTipo(iCuentaDeMayorID);
            }
        }

        private void btnImpMostrar_Click(object sender, EventArgs e)
        {
            this.LlenarImpuestos();
        }

        private void dgvImpDatos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            if (this.dgvImpDatos.Columns[e.ColumnIndex].Name == "imp_Total")
            {
                this.dgvImpDatos["imp_Gasto", e.RowIndex].Value = (Helper.ConvertirDecimal(this.dgvImpDatos["imp_Total", e.RowIndex].Value)
                    - Helper.ConvertirDecimal(this.dgvImpDatos["imp_Retenido", e.RowIndex].Value));
                this.ImpCalcularTotales();
            }
        }

        private void btnImpGuardar_Click(object sender, EventArgs e)
        {
            this.ImpAccionGuardar();
        }

        #endregion

        #endregion

        #region [ Métodos ]

        #region [ Nómina ]

        private void LlenarSemanas()
        {
            int iAnio = (int)this.nudAnio.Value;

            int iDiaIni = Helper.ConvertirEntero(Config.Valor("Comisiones.DiaInicial"));
            DateTime oFecha = new DateTime(iAnio, 1, 1).Date;
            int iDif = (iDiaIni - (int)oFecha.DayOfWeek);
            iDif += (iDif < 0 ? 8 : 0);
            oFecha = oFecha.AddDays(iDif);
            var oSemanas = new List<DosVal<DateTime, string>>();
            while (oFecha.Year == iAnio)
            {
                var oSem = new DosVal<DateTime, string>(oFecha, string.Format("{0:00} {1} al ", oFecha.Day, oFecha.ToString("MMM")));
                oFecha = oFecha.AddDays(6);
                oSem.Valor2 += string.Format("{0:00} {1}", oFecha.Day, oFecha.ToString("MMM"));
                oSem.Valor2 = oSem.Valor2.ToUpper();
                oSemanas.Add(oSem);
                oFecha = oFecha.AddDays(1);
            }
            this.cmbSemana.CargarDatos("Valor1", "Valor2", oSemanas);
        }

        private void BorrarColumnasDinamicas()
        {
            while (this.dgvDatos.Columns.Count > this.iColumnasFijas)
                this.dgvDatos.Columns.RemoveAt(this.iColumnasFijas);
        }

        private void LlenarNomina()
        {
            // Se verifica si ya se había guardado, para mostrar el histórico
            DateTime dSemana = Helper.ConvertirFechaHora(this.cmbSemana.SelectedValue);
            bool bHistorico = General.Exists<Nomina>(c => c.Semana == dSemana && (!c.Domingo.HasValue || !c.Domingo.Value));
            this.dgvDatos.ReadOnly = bHistorico;
            this.cmbCuentaBancaria.Enabled = !bHistorico;
            this.btnGuardar.Enabled = !bHistorico;
            if (bHistorico)
            {
                this.LlenarNominaHistorico();
                return;
            }
            this.cmbCuentaBancaria.SelectedValue = Cat.CuentasBancarias.Banamex;

            Cargando.Mostrar();

            // Se restauran las columnas de nómina
            this.RestaurarColumnasNomina();
            // Se llenan los usuarios
            var oUsuarios = General.GetListOf<UsuariosNominaView>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oUsuarios)
            {
                this.dgvDatos.Rows.Add(oReg.UsuarioID, oReg.SucursalID, true, oReg.Usuario, oReg.Sucursal, null, oReg.SueldoFijo);
            }

            // Se agregan las columnas de la nómina oficial
            var oCuentasNomina = General.GetListOf<NominaOficialCuentasView>();
            this.BorrarColumnasDinamicas();
            foreach (var oReg in oCuentasNomina)
            {
                var oCol = this.dgvDatos.AgregarColumnaImporte(oReg.CuentaDeMayor, oReg.CuentaDeMayor);
                oCol.Width = 60;
                oCol.ReadOnly = true;
                oCol.Tag = oReg;
                oCol.DisplayIndex = this.dgvDatos.Columns["TotalOficial"].DisplayIndex;
            }

            // Se llenan los datos de la nómina oficial
            var oNominaOficial = General.GetListOf<UsuarioNominaOficial>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                int iUsuarioID = Helper.ConvertirEntero(oFila.Cells["UsuarioID"].Value);
                decimal mTotalOf = 0;
                foreach (var oCuenta in oCuentasNomina)
                {
                    var oReg = oNominaOficial.FirstOrDefault(c => c.IdUsuario == iUsuarioID && c.ContaCuentaDeMayorID == oCuenta.ContaCuentaDeMayorID);
                    oFila.Cells[oCuenta.CuentaDeMayor].Value = (oReg == null ? 0 : oReg.Importe);
                    mTotalOf += (oReg == null ? 0 : (oReg.Importe * ((this.dgvDatos.Columns[oCuenta.CuentaDeMayor].Tag as NominaOficialCuentasView).Suma ? 1 : -1)));
                }
                oFila.Cells["TotalOficial"].Value = mTotalOf;
            }

            // Para saber si se superaron las metas de cada sucursal
            DateTime dInicio = Helper.ConvertirFechaHora(this.cmbSemana.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", dInicio);
            oParams.Add("Hasta", dInicio.AddDays(6));
            var oComisiones = General.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado", oParams);
            var oMetasSucursal = General.GetListOf<MetaSucursal>();
            decimal mComisionVen = (Helper.ConvertirDecimal(Config.Valor("Comisiones.Vendedor.Porcentaje")) / 100);
            // Se llenan los datos de los sueldos de comisiones, diferenciando a los vendedores
            var oMetas = General.GetListOf<MetaVendedor>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                int iUsuarioID = Helper.ConvertirEntero(oFila.Cells["UsuarioID"].Value);
                oFila.Cells["SueldoMinimo"].Style.ForeColor = Color.Gray;

                // Se obtienen las comisiones, sólo para vendedores y gerentes
                var oMetaVen = oMetas.FirstOrDefault(c => c.VendedorID == iUsuarioID);
                if (oMetaVen != null && General.Exists<Usuario>(c => c.UsuarioID == iUsuarioID 
                    && (c.TipoUsuarioID == Cat.TiposDeUsuario.Vendedor || c.TipoUsuarioID == Cat.TiposDeUsuario.Gerente) && c.Estatus))
                {
                    int iSucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                    // Se obtienen las comisiones
                    decimal m9500 = oComisiones.Where(c => c.RealizoUsuarioID == iUsuarioID && c.Es9500 == true).Sum(c => c.Comision).Valor();
                    decimal mVariable = 0, mMinimo = 0;

                    // Para sacar el sueldo mínimo
                    var oMetaSuc = oMetasSucursal.FirstOrDefault(c => c.SucursalID == iSucursalID);
                    decimal mUtilidadSuc = oComisiones.Where(c => c.SucursalID == iSucursalID).Sum(c => c.Utilidad).Valor();

                    // Se determina si se superó el mínimo o no
                    bool bMinimo = false;
                    if (oMetaVen.EsGerente)
                    {
                        decimal mUtilidad = oComisiones.Where(c => c.RealizoUsuarioID == iUsuarioID && c.Es9500.HasValue && !c.Es9500.Value).Sum(c => c.Utilidad).Valor();
                        bMinimo = (mUtilidad < oMetaSuc.UtilGerente || mUtilidadSuc < oMetaSuc.UtilSucursalMinimo);

                        mVariable = VentasProc.CalcularComisionGerente(oMetaSuc.UtilSucursalMinimo, mUtilidadSuc
                            , oMetaVen.IncrementoUtil.Valor(), oMetaVen.IncrementoFijo.Valor());
                    }
                    else
                    {
                        decimal mSueldoFijo = Helper.ConvertirDecimal(oFila.Cells["SueldoFijo"].Value);
                        decimal mSueldoVen = (mSueldoFijo + mVariable + (oMetaVen.MetaConsiderar9500 ? m9500 : 0));
                        decimal mSueldoMinimo = oMetaSuc.UtilVendedor;
                        mSueldoMinimo *= mComisionVen;
                        mSueldoMinimo += mSueldoFijo;
                        bMinimo = (mSueldoVen < mSueldoMinimo || mUtilidadSuc < oMetaSuc.UtilSucursalMinimo);

                        mVariable = oComisiones.Where(c => c.RealizoUsuarioID == iUsuarioID && c.Es9500.HasValue && !c.Es9500.Value).Sum(c => c.Comision).Valor();
                    }
                    if (bMinimo)
                    {
                        mMinimo = oMetaVen.SueldoMinimo;
                        oFila.Cells["SueldoFijo"].Style.ForeColor = Color.Gray;
                        oFila.Cells["SueldoVariable"].Style.ForeColor = Color.Gray;
                        oFila.Cells["SueldoMinimo"].Style.ForeColor = Color.Blue;
                    }

                    //
                    oFila.Cells["SueldoVariable"].Value = mVariable;
                    oFila.Cells["Sueldo9500"].Value = m9500;
                    oFila.Cells["SueldoMinimo"].Value = mMinimo;
                }
            }

            // Se llenan los totales
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                this.CalcularTotales(oFila);
            }
            this.CalcularReserva();

            this.bDomingo = false;

            Cargando.Cerrar();
        }

        private void RestaurarColumnasNomina()
        {
            for (int i = 2; i < this.dgvDatos.Columns.Count; i++)
                this.dgvDatos.Columns[i].Visible = true;
        }

        private void LlenarNominaHistorico()
        {
            Cargando.Mostrar();

            DateTime dSemana = Helper.ConvertirFechaHora(this.cmbSemana.SelectedValue);
            var oNomina = General.GetEntity<Nomina>(c => c.Semana == dSemana && (!c.Domingo.HasValue || !c.Domingo.Value));
            var oNomUsuariosV = General.GetListOf<NominaUsuariosView>(c => c.NominaID == oNomina.NominaID);
            var oNomUsuariosOficial = General.GetListOf<NominaUsuarioOficial>(c => c.NominaID == oNomina.NominaID);
            this.BorrarColumnasDinamicas();
            this.dgvDatos.Rows.Clear();

            // Se llena el banco
            this.cmbCuentaBancaria.SelectedValue = oNomina.BancoCuentaID;

            // Se llena el detalle de la nómina
            foreach (var oReg in oNomUsuariosV)
            {
                var oNominaOf = oNomUsuariosOficial.Where(c => c.IdUsuario == oReg.UsuarioID);
                // Se llenan los datos fijos
                this.dgvDatos.Rows.Add(
                    oReg.UsuarioID, 
                    oReg.SucursalID,
                    true,
                    oReg.Usuario, 
                    oReg.Sucursal,
                    oReg.TotalOficial,
                    oReg.SueldoFijo, 
                    oReg.SueldoVariable, 
                    oReg.Sueldo9500, 
                    oReg.SueldoMinimo,
                    oReg.Bono, 
                    oReg.Adicional, 
                    oReg.TotalSueldo,
                    oReg.Diferencia,
                    oReg.Tickets, 
                    oReg.Adelanto, 
                    oReg.MinutosTarde, 
                    oReg.Otros, 
                    oReg.TotalDescuentos,
                    oReg.Liquido
                );
                var oFila = this.dgvDatos.Rows[this.dgvDatos.Rows.Count - 1];
                // Se colorean las celdas, según se haya superado la meta o no
                if (!oReg.SuperoMinimo)
                {
                    oFila.Cells["SueldoFijo"].Style.ForeColor = Color.Gray;
                    oFila.Cells["SueldoVariable"].Style.ForeColor = Color.Gray;
                    oFila.Cells["SueldoMinimo"].Style.ForeColor = Color.Blue;
                }
                // Se llenan los datos dinámicos
                decimal mTotalOficial = 0;
                foreach (var oImporteC in oNominaOf)
                {
                    var oCuentaDeMayor = General.GetEntity<ContaCuentaDeMayor>(c => c.ContaCuentaDeMayorID == oImporteC.ContaCuentaDeMayorID);
                    if (!this.dgvDatos.Columns.Contains(oCuentaDeMayor.CuentaDeMayor))
                    {
                        var oCol = this.dgvDatos.AgregarColumnaImporte(oCuentaDeMayor.CuentaDeMayor, oCuentaDeMayor.CuentaDeMayor);
                        oCol.Width = 60;
                        oCol.DisplayIndex = this.dgvDatos.Columns["TotalOficial"].DisplayIndex;
                    }
                    oFila.Cells[oCuentaDeMayor.CuentaDeMayor].Value = oImporteC.Importe;
                    mTotalOficial += (oImporteC.Importe * (oImporteC.Suma ? 1 : -1));
                }
                // Se llenan los totales
                // oFila.Cells["TotalOficial"].Value = mTotalOficial;
                // oFila.Cells["Diferencia"].Value = (Helper.ConvertirDecimal(oFila.Cells["TotalSueldo"].Value) - mTotalOficial);
                // oFila.Cells["Liquido"].Value = (Helper.ConvertirDecimal(oFila.Cells["Diferencia"].Value) - Helper.ConvertirDecimal(oFila.Cells["TotalDescuentos"].Value));
            }

            Cargando.Cerrar();
        }

        private void CalcularTotales(DataGridViewRow oFila)
        {
            decimal mOficial = Helper.ConvertirDecimal(oFila.Cells["TotalOficial"].Value);
            decimal mSueldo = (
                (oFila.Cells["SueldoFijo"].Style.ForeColor == Color.Gray ? 0 : Helper.ConvertirDecimal(oFila.Cells["SueldoFijo"].Value))
                + (oFila.Cells["SueldoVariable"].Style.ForeColor == Color.Gray ? 0 : Helper.ConvertirDecimal(oFila.Cells["SueldoVariable"].Value))
                + Helper.ConvertirDecimal(oFila.Cells["Sueldo9500"].Value)
                + (oFila.Cells["SueldoMinimo"].Style.ForeColor == Color.Gray ? 0 : Helper.ConvertirDecimal(oFila.Cells["SueldoMinimo"].Value))
                + Helper.ConvertirDecimal(oFila.Cells["Bono"].Value)
                + Helper.ConvertirDecimal(oFila.Cells["Adicional"].Value)
            );
            oFila.Cells["TotalSueldo"].Value = mSueldo;
            decimal mDiferencia = (mSueldo - mOficial);
            oFila.Cells["Diferencia"].Value = mDiferencia;
            decimal mDescuentos = (
                Helper.ConvertirDecimal(oFila.Cells["Tickets"].Value)
                + Helper.ConvertirDecimal(oFila.Cells["Adelanto"].Value)
                + Helper.ConvertirDecimal(oFila.Cells["MinutosTarde"].Value)
                + Helper.ConvertirDecimal(oFila.Cells["Otros"].Value)
            );
            oFila.Cells["TotalDescuentos"].Value = mDescuentos;
            decimal mLiquido = (mDiferencia - mDescuentos);
            oFila.Cells["Liquido"].Value = mLiquido;
        }

        private void CalcularReserva()
        {
            // decimal mDiferencia = 0, mMinutos = 0, mOtros = 0;
            var oSucursales = new Dictionary<int, decimal>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                    continue;

                int iSucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                if (!oSucursales.ContainsKey(iSucursalID))
                    oSucursales.Add(iSucursalID, 0);

                oSucursales[iSucursalID] += Helper.ConvertirDecimal(oFila.Cells["Diferencia"].Value);
                oSucursales[iSucursalID] -= Helper.ConvertirDecimal(oFila.Cells["MinutosTarde"].Value);
                oSucursales[iSucursalID] -= Helper.ConvertirDecimal(oFila.Cells["Otros"].Value);
                // oSucursales[iSucursalID] += (mDiferencia - mMinutos - mOtros);
            }
            // this.lblReserva.Text = (mDiferencia - mMinutos - mOtros).ToString(GlobalClass.FormatoMoneda);
            string sTexto = "";
            foreach (var oReg in oSucursales)
            {
                sTexto += string.Format("Suc {0:00}: {1} | ", oReg.Key, oReg.Value.ToString(GlobalClass.FormatoMoneda));
            }
            this.lblReserva.Text = sTexto;
        }

        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            if (this.bDomingo)
                return this.GuardarDomingo();
             
            Cargando.Mostrar();
            
            // Se guarda la nómina
            int iBancoCuentaID = Helper.ConvertirEntero(this.cmbCuentaBancaria.SelectedValue);
            string sDia = DateTime.Now.ToString("yyMMdd");
            DateTime dSemana = Helper.ConvertirFechaHora(this.cmbSemana.SelectedValue);
            var oNomina = new Nomina()
            {
                Semana = dSemana,
                Fecha = DateTime.Now,
                BancoCuentaID = iBancoCuentaID
            };
            Guardar.Generico<Nomina>(oNomina);

            // Se guarda el detalle
            decimal mTotalOficial = 0;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                    continue;

                int iUsuarioID = Helper.ConvertirEntero(oFila.Cells["UsuarioID"].Value);
                int iSucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                // Se llenan los datos de la nómina oficial (dinámico)
                for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
                {
                    var oNominaOfCuentaV = (this.dgvDatos.Columns[iCol].Tag as NominaOficialCuentasView);
                    var oNominaOf = new NominaUsuarioOficial()
                    {
                        NominaID = oNomina.NominaID,
                        IdUsuario = iUsuarioID,
                        ContaCuentaDeMayorID = oNominaOfCuentaV.ContaCuentaDeMayorID,
                        Importe = Helper.ConvertirDecimal(oFila.Cells[iCol].Value),
                        Suma = oNominaOfCuentaV.Suma
                    };
                    Guardar.Generico<NominaUsuarioOficial>(oNominaOf);
                }
                // Se guardan los datos no dinámicos
                var oNominaFijo = new NominaUsuario()
                {
                    NominaID = oNomina.NominaID,
                    IdUsuario = iUsuarioID,
                    SucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value),
                    SuperoMinimo = (oFila.Cells["SueldoMinimo"].Style.ForeColor == Color.Gray),
                    SueldoFijo = Helper.ConvertirDecimal(oFila.Cells["SueldoFijo"].Value),
                    SueldoVariable = Helper.ConvertirDecimal(oFila.Cells["SueldoVariable"].Value),
                    Sueldo9500 = Helper.ConvertirDecimal(oFila.Cells["Sueldo9500"].Value),
                    SueldoMinimo = Helper.ConvertirDecimal(oFila.Cells["SueldoMinimo"].Value),
                    Bono = Helper.ConvertirDecimal(oFila.Cells["Bono"].Value),
                    Adicional = Helper.ConvertirDecimal(oFila.Cells["Adicional"].Value),
                    Tickets = Helper.ConvertirDecimal(oFila.Cells["Tickets"].Value),
                    Adelanto = Helper.ConvertirDecimal(oFila.Cells["Adelanto"].Value),
                    MinutosTarde = Helper.ConvertirDecimal(oFila.Cells["MinutosTarde"].Value),
                    Otros = Helper.ConvertirDecimal(oFila.Cells["Otros"].Value)
                };
                Guardar.Generico<NominaUsuario>(oNominaFijo);

                // Se generan los gastos contables correspondientes, de lo oficial
                for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
                {
                    decimal mImporte = Helper.ConvertirDecimal(oFila.Cells[iCol].Value);
                    var oNominaOfCuentaV = (this.dgvDatos.Columns[iCol].Tag as NominaOficialCuentasView);
                    int iCuentaDeMayorID = oNominaOfCuentaV.ContaCuentaDeMayorID;
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaDeMayorID && c.RelacionID == iUsuarioID);
                    if (oCuentaAux == null)
                        continue;
                    var oGasto = new ContaEgreso()
                    {
                        ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID,
                        Fecha = DateTime.Now,
                        Importe = mImporte,
                        TipoFormaPagoID = Cat.FormasDePago.Transferencia,
                        FolioDePago = sDia,
                        TipoDocumentoID = Cat.TiposDeDocumento.Factura,
                        EsFiscal = true,
                        Observaciones = ("NÓMINA " + this.cmbSemana.Text),
                        SucursalID = iSucursalID,
                        RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FolioFactura = sDia,
                        BancoCuentaID = iBancoCuentaID
                    };
                    ContaProc.GastoCrear(oGasto);
                }
                // Se genera el gasto contable por la diferencia, si aplica
                decimal mDiferencia = Helper.ConvertirDecimal(oFila.Cells["Diferencia"].Value);
                if (mDiferencia != 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == iUsuarioID);
                    if (oCuentaAux != null)
                    {
                        var oGasto = new ContaEgreso()
                        {
                            ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID,
                            Fecha = DateTime.Now,
                            Importe = mDiferencia,
                            TipoFormaPagoID = Cat.FormasDePago.Efectivo,
                            TipoDocumentoID = Cat.TiposDeDocumento.Nota,
                            EsFiscal = false,
                            Observaciones = ("CN " + this.cmbSemana.Text),
                            SucursalID = iSucursalID,
                            RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                            FolioFactura = sDia,
                        };
                        ContaProc.GastoCrear(oGasto);
                    }
                }

                mTotalOficial += Helper.ConvertirDecimal(oFila.Cells["TotalOficial"].Value);
            }

            // Se genera el moviemiento bancario, con lo oficial
            DateTime dAhora = DateTime.Now;
            var oMov = new BancoCuentaMovimiento()
            {
                BancoCuentaID = iBancoCuentaID,
                EsIngreso = false,
                Fecha = dAhora,
                FechaAsignado = dAhora,
                SucursalID = GlobalClass.SucursalID,
                Importe = mTotalOficial,
                Concepto = "NÓMINA BANCARIA",
                Referencia = GlobalClass.UsuarioGlobal.NombreUsuario,
                TipoFormaPagoID = Cat.FormasDePago.Transferencia,
                RelacionTabla = Cat.Tablas.Nomina,
                RelacionID = oNomina.NominaID
            };
            ContaProc.RegistrarMovimientoBancario(oMov);

            // Se generan las pólizas contables correspondientes (AfeConta)
            var oNominaUsuariosV = General.GetListOf<NominaUsuariosView>(c => c.NominaID == oNomina.NominaID);
            foreach (var oReg in oNominaUsuariosV)
            {
                // Se crea la póliza de lo oficial
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.NominaOficial, oReg.NominaUsuarioID, this.cmbSemana.Text, ("NÓMINA " + this.cmbSemana.Text));
                // Se crea la póliza de la diferencia, si aplica
                if (oReg.Diferencia != 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux == null)
                    {
                        UtilLocal.MensajeAdvertencia("No se encontró la cuenta auxiliar: Salarios -> " + oReg.Usuario + ". No se creará la póliza de Complemento Nómina.");
                        continue;
                    }
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, ("COMPLEMENTO NÓMINA " + this.cmbSemana.Text)
                        , oCuentaAux.ContaCuentaAuxiliarID, Cat.ContaCuentasAuxiliares.ReservaNomina, oReg.Diferencia.Valor(), oReg.Usuario
                        , Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                }

                // Se crea la póliza del adelanto, si aplica
                if (oReg.Adelanto > 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.DeudoresDiversos
                        && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux != null)
                        ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "ADELANTO", Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID
                            , oReg.Adelanto, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor(), oReg.SucursalID);
                }
                // Se crea la póliza de minutos tarde y otros, si aplica
                if (oReg.MinutosTarde > 0 || oReg.Otros > 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios
                        && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux != null)
                    {
                        if (oReg.MinutosTarde > 0)
                            ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "MINUTOS TARDE", Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID
                                , oReg.MinutosTarde, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                        if (oReg.Otros > 0)
                            ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "OTROS DESCUENTOS", Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID
                                , oReg.Otros, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                    }
                }
            }

            // Se genera el resguardo y refuerzo especiales
            var oResguardos = new Dictionary<int, decimal>();
            var oRefuerzos = new Dictionary<int, decimal>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                    continue;

                int iSucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                if (!oResguardos.ContainsKey(iSucursalID))
                    oResguardos.Add(iSucursalID, 0);
                if (!oRefuerzos.ContainsKey(iSucursalID))
                    oRefuerzos.Add(iSucursalID, 0);

                decimal mDiferencia = Helper.ConvertirDecimal(oFila.Cells["Diferencia"].Value);
                decimal mTickets = Helper.ConvertirDecimal(oFila.Cells["Tickets"].Value);
                decimal mMinutosTarde = Helper.ConvertirDecimal(oFila.Cells["MinutosTarde"].Value);
                decimal mOtros = Helper.ConvertirDecimal(oFila.Cells["Otros"].Value);
                decimal mAdelanto = Helper.ConvertirDecimal(oFila.Cells["Adelanto"].Value);
                oRefuerzos[iSucursalID] += (mDiferencia - mMinutosTarde - mOtros);
                // Se quita el adelanto a los resguardos de acuerdo a una nueva petición - Moi 03/08/2015
                // oResguardos[iSucursalID] += (mDiferencia - mMinutosTarde - mOtros + mAdelanto);
                // Se suman ahora los tickets a los resguardos porque no están seguros y están haciendo pruebas - Moi 08/08/2015
                oResguardos[iSucursalID] += (mDiferencia - mMinutosTarde - mOtros + mTickets);
            }
            // Se crean los resguardos / refuerzos y las pólizas correspondientes (AfeConta)
            foreach (var oReg in oResguardos)
            {
                var oResguardo = VentasProc.GenerarResguardo(oReg.Value, oReg.Key);
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.Resguardo, oResguardo.CajaEgresoID, "NO MOVER", "NO MOVER DE TIENDA / RESGUARDO", oReg.Key);
            }
            foreach (var oReg in oRefuerzos)
            {
                var oRefuerzo = VentasProc.GenerarRefuerzo(oReg.Value, oReg.Key);
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.Refuerzo, oRefuerzo.CajaIngresoID, "REFUERZO", "REFUERZO NÓMINA", oReg.Key);
            }

            // Se manda a imprimir la nómina de cada usuario
            var oNominaUsuariosOfV = General.GetListOf<NominaUsuariosOficialView>(c => c.NominaID == oNomina.NominaID);
            var oNominaOficialTotales = oNominaUsuariosOfV.GroupBy(c => c.UsuarioID).Select(c => new
            {
                UsuarioID = c.Key,
                Ingreso = c.Where(s => s.Suma).Sum(s => s.Importe),
                Egreso = c.Where(s => !s.Suma).Sum(s => s.Importe)
            });
            var oRep = new Report();
            oRep.Load(UtilLocal.RutaReportes() + "Nomina.frx");
            oRep.RegisterData(new List<Nomina>() { oNomina }, "Nomina");
            oRep.RegisterData(oNominaUsuariosV, "Usuarios");
            oRep.RegisterData(oNominaUsuariosOfV, "UsuariosOficial");
            oRep.RegisterData(oNominaOficialTotales, "UsuariosOficialTotales");

            Cargando.Cerrar();
            UtilLocal.EnviarReporteASalida("Reportes.Nomina.Salida", oRep);

            this.LlenarNomina();
            return true;
        }

        private bool GuardarDomingo()
        {
            // Se guarda la nómina
            int iBancoCuentaID = Helper.ConvertirEntero(this.cmbCuentaBancaria.SelectedValue);
            string sDia = DateTime.Now.ToString("yyMMdd");
            var oNomina = new Nomina()
            {
                Semana = DateTime.Now,
                Fecha = DateTime.Now,
                BancoCuentaID = iBancoCuentaID,
                Domingo = true
            };
            Guardar.Generico<Nomina>(oNomina);

            // Se guarda el detalle
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                    continue;

                int iUsuarioID = Helper.ConvertirEntero(oFila.Cells["UsuarioID"].Value);
                int iSucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                // Se guardan los datos
                var oNominaFijo = new NominaUsuario()
                {
                    NominaID = oNomina.NominaID,
                    IdUsuario = iUsuarioID,
                    SucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value),
                    Adicional = Helper.ConvertirDecimal(oFila.Cells["Adicional"].Value),
                    Tickets = Helper.ConvertirDecimal(oFila.Cells["Tickets"].Value),
                    Adelanto = Helper.ConvertirDecimal(oFila.Cells["Adelanto"].Value),
                    MinutosTarde = Helper.ConvertirDecimal(oFila.Cells["MinutosTarde"].Value),
                    Otros = Helper.ConvertirDecimal(oFila.Cells["Otros"].Value)
                };
                Guardar.Generico<NominaUsuario>(oNominaFijo);

                // Se genera el gasto contable por la diferencia, si aplica
                decimal mDiferencia = Helper.ConvertirDecimal(oFila.Cells["Diferencia"].Value);
                if (mDiferencia != 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == iUsuarioID);
                    if (oCuentaAux != null)
                    {
                        var oGasto = new ContaEgreso()
                        {
                            ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID,
                            Fecha = DateTime.Now,
                            Importe = mDiferencia,
                            TipoFormaPagoID = Cat.FormasDePago.Efectivo,
                            TipoDocumentoID = Cat.TiposDeDocumento.Nota,
                            EsFiscal = false,
                            Observaciones = "NÓMINA DOMINGO",
                            SucursalID = iSucursalID,
                            RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                            FolioFactura = sDia,
                        };
                        ContaProc.GastoCrear(oGasto);
                    }
                }
            }
                        
            // Se generan las pólizas contables correspondientes (AfeConta)
            var oNominaUsuariosV = General.GetListOf<NominaUsuariosView>(c => c.NominaID == oNomina.NominaID);
            foreach (var oReg in oNominaUsuariosV)
            {
                // Se crea la póliza de la diferencia, si aplica
                if (oReg.Diferencia != 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux == null)
                    {
                        UtilLocal.MensajeAdvertencia("No se encontró la cuenta auxiliar: Salarios -> " + oReg.Usuario + ". No se creará la póliza de Complemento Nómina.");
                        continue;
                    }
                    ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "NÓMINA DOMINGO"
                        , oCuentaAux.ContaCuentaAuxiliarID, Cat.ContaCuentasAuxiliares.ReservaNomina, oReg.Diferencia.Valor(), oReg.Usuario
                        , Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                }

                // Se crea la póliza del adelanto, si aplica
                if (oReg.Adelanto > 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.DeudoresDiversos
                        && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux != null)
                        ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "ADELANTO", Cat.ContaCuentasAuxiliares.Caja, oCuentaAux.ContaCuentaAuxiliarID
                            , oReg.Adelanto, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor(), oReg.SucursalID);
                }
                // Se crea la póliza de minutos tarde y otros, si aplica
                if (oReg.MinutosTarde > 0 || oReg.Otros > 0)
                {
                    var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios
                        && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux != null)
                    {
                        if (oReg.MinutosTarde > 0)
                            ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "MINUTOS TARDE", Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID
                                , oReg.MinutosTarde, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                        if (oReg.Otros > 0)
                            ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "OTROS DESCUENTOS", Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID
                                , oReg.Otros, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                    }
                }
            }
            
            UtilLocal.MostrarNotificacion("Proceso completado.");
            this.btnDomingo_Click(this, null);

            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Helper.ConvertirEntero(this.cmbCuentaBancaria.SelectedValue) == 0)
                this.ctlError.PonerError(this.cmbCuentaBancaria, "Debes especificar la cuenta bancaria.");
            return this.ctlError.Valido;
        }

        private void GenerarDesgloseLiquido()
        {
            var oDesglose = new List<ConteoMonedasId>();
            var oMonedas = new Dictionary<decimal, int>();
            var oListaMon = new List<decimal>() { 500, 200, 100, 50, 20, 10, 5, 2, 1, 0.5M, 0.2M, 0.1M };
            foreach (decimal mMoneda in oListaMon)
                oMonedas.Add(mMoneda, 0);
            
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                decimal mLiquido = Helper.ConvertirDecimal(oFila.Cells["Liquido"].Value);
                // Se calculan las moendas del usuario
                foreach (decimal mMon in oListaMon)
                {
                    oMonedas[mMon] = (int)(mLiquido / mMon);
                    mLiquido -= (mMon * oMonedas[mMon]);
                    if (mLiquido <= 0)
                        break;
                }

                // Se agrega el usuario a la lista
                oDesglose.Add(new ConteoMonedasId()
                {
                    Id = Helper.ConvertirEntero(oFila.Cells["UsuarioID"].Value),
                    Monedas500 = oMonedas[500],
                    Monedas200 = oMonedas[200],
                    Monedas100 = oMonedas[100],
                    Monedas50 = oMonedas[50],
                    Monedas20 = oMonedas[20],
                    Monedas10 = oMonedas[10],
                    Monedas5 = oMonedas[5],
                    Monedas2 = oMonedas[2],
                    Monedas1 = oMonedas[1],
                    Monedas05 = oMonedas[0.5M],
                    Monedas02 = oMonedas[0.2M],
                    Monedas01 = oMonedas[0.1M]
                });
            }

            // Se manda imprimir el reporte
            var oRep = new Report();
            oRep.Load(UtilLocal.RutaReportes("DesgloseNomina.frx"));
            oRep.RegisterData(oDesglose, "Desglose");
            UtilLocal.EnviarReporteASalida("Reportes.Nomina.Desglose", oRep);
        }

        #endregion

        #region [ Impuestos ]

        private void ImpCambiarTipo(int iCuentaDeMayorID)
        {
            // Se ajusta el combo de período
            this.cmbImpPeriodo.DataSource = this.oMeses;
            // Se muestran todas las columnas del grid
            this.dgvImpDatos.Columns["imp_Retenido"].Visible = true;
            this.dgvImpDatos.Columns["imp_Gasto"].Visible = true;
            this.dgvImpTotales.Columns["imp_TotalRetenido"].Visible = true;
            this.dgvImpTotales.Columns["imp_TotalGasto"].Visible = true;

            // Se configuran los controles, según el tipo seleccionado
            switch (iCuentaDeMayorID)
            {
                case Cat.ContaCuentasDeMayor.Nomina2Por:
                    this.dgvImpDatos.Columns["imp_Retenido"].Visible = false;
                    this.dgvImpDatos.Columns["imp_Gasto"].Visible = false;
                    this.dgvImpTotales.Columns["imp_TotalRetenido"].Visible = false;
                    this.dgvImpTotales.Columns["imp_TotalGasto"].Visible = false;
                    break;
                case Cat.ContaCuentasDeMayor.Imss:
                    break;
                case Cat.ContaCuentasDeMayor.Infonavit:
                    // Se ajusta el combo de período por bimestre
                    this.cmbImpPeriodo.DataSource = this.oBimestres;
                    break;
            }
        }

        private void LlenarImpuestos()
        {
            Cargando.Mostrar();

            var oUsuarios = General.GetListOf<UsuariosNominaView>();
            int iTipo = Helper.ConvertirEntero(this.cmbImpTipo.SelectedValue);
            DateTime dDesde = new DateTime(DateTime.Now.Year, Helper.ConvertirEntero(this.cmbImpPeriodo.SelectedValue), 1);
            DateTime dHasta = (iTipo == Cat.ContaCuentasDeMayor.Imss ? dDesde.DiaUltimo() : dDesde.AddMonths(1).DiaUltimo()).AddDays(1);
            var oNominasPer = General.GetListOf<NominaUsuariosOficialView>(c => c.Semana >= dDesde && c.Semana <= dHasta);
            this.dgvImpDatos.Rows.Clear();
            foreach (var oReg in oUsuarios)
            {
                decimal mRetenido = 0;
                if (iTipo != Cat.ContaCuentasDeMayor.Nomina2Por)
                {
                    int iCuentaDeMayorID = (iTipo == Cat.ContaCuentasDeMayor.Imss ? Cat.ContaCuentasDeMayor.RetencionImss : Cat.ContaCuentasDeMayor.RetencionInfonavit);
                    mRetenido = oNominasPer.Where(c => c.UsuarioID == oReg.UsuarioID && c.ContaCuentaDeMayorID == iCuentaDeMayorID).Sum(c => c.Importe);
                }

                this.dgvImpDatos.Rows.Add(oReg.UsuarioID, oReg.SucursalID, true, oReg.Usuario, oReg.Sucursal, mRetenido, (mRetenido * -1), 0);
            }

            // Se llenan los totales
            this.ImpCalcularTotales();

            Cargando.Cerrar();
        }

        private void ImpCalcularTotales()
        {
            decimal mRetenido = 0, mGasto = 0, mTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvImpDatos.Rows)
            {
                mRetenido += Helper.ConvertirDecimal(oFila.Cells["imp_Retenido"].Value);
                mGasto += Helper.ConvertirDecimal(oFila.Cells["imp_Gasto"].Value);
                mTotal += Helper.ConvertirDecimal(oFila.Cells["imp_Total"].Value);
            }

            this.dgvImpTotales.Rows.Clear();
            this.dgvImpTotales.Rows.Add("Totales", mRetenido, mGasto, mTotal);
        }

        private void ImpAccionGuardar()
        {
            if (!this.ImpValidar())
                return;

            Cargando.Mostrar();
                        
            // Se inserta el pago de impuesto
            var oImpuesto = new NominaImpuesto()
            {
                ContaCuentaDeMayorID = Helper.ConvertirEntero(this.cmbImpTipo.SelectedValue),
                Fecha = this.dtpImpFecha.Value,
                Periodo = Helper.ConvertirEntero(this.cmbImpPeriodo.SelectedValue),
                BancoCuentaID = Helper.ConvertirEntero(this.cmbImpCuenta.SelectedValue),
                TipoFormaPagoID = Helper.ConvertirEntero(this.cmbImpFormaDePago.SelectedValue),
                FolioDePago = this.txtImpFolioDePago.Text
            };
            Guardar.Generico<NominaImpuesto>(oImpuesto);

            string sPeriodo = this.cmbImpPeriodo.Text;
            decimal mImporteTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvImpDatos.Rows)
            {
                if (!Helper.ConvertirBool(oFila.Cells["imp_Sel"].Value))
                    continue;

                // Se inserta el impuesto usuario
                var oImpUsuario = new NominaImpuestoUsuario()
                {
                    NominaImpuestoID = oImpuesto.NominaImpuestoID,
                    IdUsuario = Helper.ConvertirEntero(oFila.Cells["imp_UsuarioID"].Value),
                    SucursalID = Helper.ConvertirEntero(oFila.Cells["imp_SucursalID"].Value),
                    Retenido = Helper.ConvertirDecimal(oFila.Cells["imp_Retenido"].Value),
                    Total = Helper.ConvertirDecimal(oFila.Cells["imp_Total"].Value)
                };
                Guardar.Generico<NominaImpuestoUsuario>(oImpUsuario);
                mImporteTotal += oImpUsuario.Total;

                // Se inserta el gasto contable
                var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == oImpuesto.ContaCuentaDeMayorID && c.RelacionID == oImpUsuario.IdUsuario);
                if (oCuentaAux == null)
                {
                    UtilLocal.MensajeAdvertencia("El usuario " + Helper.ConvertirCadena(oFila.Cells["imp_Usuario"].Value)
                        + " no tiene una cuenta auxiliar relacionada. No se generará el gasto contable.");
                }
                else
                {
                    var oEgreso = new ContaEgreso()
                    {
                        ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID,
                        Fecha = oImpuesto.Fecha,
                        Importe = oImpUsuario.Total,
                        TipoFormaPagoID = oImpuesto.TipoFormaPagoID,
                        FolioDePago = oImpuesto.FolioDePago,
                        TipoDocumentoID = Cat.TiposDeDocumento.Factura,
                        EsFiscal = true,
                        Observaciones = ("PAGO " + this.cmbImpTipo.Text + " " + sPeriodo),
                        SucursalID = oImpUsuario.SucursalID,
                        RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FolioFactura = sPeriodo,
                        BancoCuentaID = oImpuesto.BancoCuentaID
                    };
                    ContaProc.GastoCrear(oEgreso);
                }

                // Se registra la póliza correspondiente (AfeConta)
                switch (oImpuesto.ContaCuentaDeMayorID)
                {
                    case Cat.ContaCuentasDeMayor.Nomina2Por:
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.Pago2Por, oImpUsuario.NominaImpuestoUsuarioID, oImpuesto.FolioDePago
                            , ("PAGO 2% " + sPeriodo));
                        break;
                    case Cat.ContaCuentasDeMayor.Imss:
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoImss, oImpUsuario.NominaImpuestoUsuarioID, oImpuesto.FolioDePago
                            , ("PAGO IMSS " + sPeriodo));
                        break;
                    case Cat.ContaCuentasDeMayor.Infonavit:
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoInfonavit, oImpUsuario.NominaImpuestoUsuarioID, oImpuesto.FolioDePago
                            , ("PAGO INFONAVIT " + sPeriodo));
                        break;
                }
            }

            // Se inserta el movimiento bancario
            var oMov = new BancoCuentaMovimiento()
            {
                BancoCuentaID = oImpuesto.BancoCuentaID,
                EsIngreso = false,
                Fecha = oImpuesto.Fecha,
                FechaAsignado = oImpuesto.Fecha,
                SucursalID = Cat.Sucursales.Matriz,
                Importe = mImporteTotal,
                Concepto = ("PAGO IMPUESTO " + this.cmbImpTipo.Text + " " + sPeriodo),
                Referencia = GlobalClass.UsuarioGlobal.NombreUsuario,
                TipoFormaPagoID = oImpuesto.TipoFormaPagoID,
                DatosDePago = oImpuesto.FolioDePago,
                RelacionTabla = Cat.Tablas.NominaImpuesto,
                RelacionID = oImpuesto.NominaImpuestoID
            };
            ContaProc.RegistrarMovimientoBancario(oMov);

            Cargando.Cerrar();
            UtilLocal.MensajeInformacion("Proceso completado exitosamente.");
        }

        private bool ImpValidar()
        {
            this.ctlError.LimpiarErrores();
            if (Helper.ConvertirEntero(this.cmbImpTipo.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpTipo, "Debes especificar el tipo de impuesto.");
            if (Helper.ConvertirEntero(this.cmbImpPeriodo.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpPeriodo, "Debes especificar el período.");
            if (Helper.ConvertirEntero(this.cmbImpCuenta.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpCuenta, "Debes especificar la cuenta bancaria.");
            if (Helper.ConvertirEntero(this.cmbImpFormaDePago.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpFormaDePago, "Debes especificar la forma de pago.");
            if (this.txtImpFolioDePago.Text == "")
                this.ctlError.PonerError(this.txtImpFolioDePago, "Debes especificar un folio de pago.");
            // Se valida que haya al menos un usuario seleccionado
            bool bSel = false;
            foreach (DataGridViewRow oFila in this.dgvImpDatos.Rows)
            {
                if (Helper.ConvertirBool(oFila.Cells["imp_Sel"].Value))
                {
                    bSel = true;
                    break;
                }
            }
            if (!bSel)
                this.ctlError.PonerError(this.btnImpGuardar, "No hay ningún usuario seleccionado.", ErrorIconAlignment.MiddleLeft);

            return this.ctlError.Valido;
        }

        #endregion

        #endregion

    }
}