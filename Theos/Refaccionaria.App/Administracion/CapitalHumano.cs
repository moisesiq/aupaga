using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using FastReport;
using AdvancedDataGridView;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CapitalHumano : UserControl
    {
        const int iNivelLinea = 3;
        const int iNivelParte = 4;
        const int iCol_com_Id = 1;

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
            this.cmbSemana.SelectedValue = UtilTheos.InicioSemanaSabAVie(DateTime.Now).Date;
            // Se selecciona por predeterminado la semana anterior
            if (this.cmbSemana.SelectedIndex > 0 && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                this.cmbSemana.SelectedIndex -= 1;

            // Se configuran las fechas predeterminadas
            this.dtpOficial.Value = DateTime.Now.AddDays(-1);

            // Se llenan las cuentas bancarias
            this.cmbCuentaBancaria.CargarDatos("BancoCuentaID", "NombreDeCuenta", Datos.GetListOf<BancoCuenta>().OrderBy(c => c.NombreDeCuenta).ToList());
            this.cmbCuentaBancaria.SelectedValue = Cat.CuentasBancarias.Banamex;

            // Se llenan los controles de la pestaña impuestos
            this.cmbImpTipo.CargarDatos("ContaCuentaDeMayorID", "CuentaDeMayor", Datos.GetListOf<ContaCuentaDeMayor>(c =>
                c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Nomina2Por || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Imss
                || c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Infonavit).OrderBy(c => c.CuentaDeMayor).ToList());
            this.cmbImpPeriodo.ValueMember = "Entero";
            this.cmbImpPeriodo.DisplayMember = "Cadena";
            this.cmbImpCuenta.CargarDatos("BancoCuentaID", "NombreDeCuenta", Datos.GetListOf<BancoCuenta>().OrderBy(c => c.NombreDeCuenta).ToList());
            this.cmbImpCuenta.SelectedValue = Cat.CuentasBancarias.Scotiabank;
            this.cmbImpFormaDePago.CargarDatos("TipoFormaPagoID", "NombreTipoFormaPago", Datos.GetListOf<TipoFormaPago>(c =>
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

            // Para la pestaña de comisiones
            this.tgvComisiones.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.tgvComisiones.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
        }

        private void tabCapitalHumano_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabCapitalHumano.SelectedTab.Name)
            {
                case "tbpComisiones":
                    if (this.tgvComisiones.Nodes.Count <= 0)
                        this.CargarArbolDeComisiones();
                    break;
            }
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
            var oUsuarios = Datos.GetListOf<UsuariosNominaView>();
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
                if (Util.Decimal(e.Value) == 0)
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
                int iCuentaDeMayorID = Util.Entero(this.cmbImpTipo.SelectedValue);
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
                this.dgvImpDatos["imp_Gasto", e.RowIndex].Value = (Util.Decimal(this.dgvImpDatos["imp_Total", e.RowIndex].Value)
                    - Util.Decimal(this.dgvImpDatos["imp_Retenido", e.RowIndex].Value));
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

            int iDiaIni = Util.Entero(Config.Valor("Comisiones.DiaInicial"));
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
            // Se agrega la semana especial de aguinaldo
            oSemanas.Add(new DosVal<DateTime, string>(DateTime.Now, "AGUINALDO"));

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
            // Se verifica si es aguinaldo o no
            Nomina oNomina = null;
            if (this.cmbSemana.Text == "AGUINALDO")
            {
                int iAnio = (int)this.nudAnio.Value;
                oNomina = Datos.GetEntity<Nomina>(c => c.Semana.Year == iAnio && c.EsAguinaldo == true);
            }
            else
            {
                DateTime dSemana = Util.FechaHora(this.cmbSemana.SelectedValue);
                oNomina = Datos.GetEntity<Nomina>(c => c.Semana == dSemana && (!c.Domingo.HasValue || !c.Domingo.Value)
                    && (!c.EsAguinaldo.HasValue || !c.EsAguinaldo.Value));
            }
            bool bHistorico = (oNomina != null);
            this.dgvDatos.ReadOnly = bHistorico;
            this.cmbCuentaBancaria.Enabled = !bHistorico;
            this.btnGuardar.Enabled = !bHistorico;
            if (bHistorico)
            {
                this.LlenarNominaHistorico(oNomina.NominaID);
                return;
            }
            this.cmbCuentaBancaria.SelectedValue = Cat.CuentasBancarias.Banamex;

            Cargando.Mostrar();

            // Se restauran las columnas de nómina
            this.RestaurarColumnasNomina();
            // Se llenan los usuarios
            var oUsuarios = Datos.GetListOf<UsuariosNominaView>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oUsuarios)
            {
                this.dgvDatos.Rows.Add(oReg.UsuarioID, oReg.SucursalID, true, oReg.Usuario, oReg.Sucursal, null, oReg.SueldoFijo);
            }

            // Se agregan las columnas de la nómina oficial
            var oCuentasNomina = Datos.GetListOf<NominaOficialCuentasView>();
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
            var oNominaOficial = Datos.GetListOf<UsuarioNominaOficial>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                int iUsuarioID = Util.Entero(oFila.Cells["UsuarioID"].Value);
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
            DateTime dInicio = Util.FechaHora(this.cmbSemana.SelectedValue);
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", dInicio);
            oParams.Add("Hasta", dInicio.AddDays(6));
            oParams.Add("SucursalID", 0);
            oParams.Add("VendedorID", 0);
            var oComisiones = Datos.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado", oParams);
            var oMetasSucursal = Datos.GetListOf<MetaSucursal>();
            decimal mComisionVen = (Util.Decimal(Config.Valor("Comisiones.Vendedor.Porcentaje")) / 100);
            // Se llenan los datos de los sueldos de comisiones, diferenciando a los vendedores
            var oMetas = Datos.GetListOf<MetaVendedor>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                int iUsuarioID = Util.Entero(oFila.Cells["UsuarioID"].Value);
                oFila.Cells["SueldoMinimo"].Style.ForeColor = Color.Gray;

                // Se obtienen las comisiones, sólo para vendedores y gerentes
                var oMetaVen = oMetas.FirstOrDefault(c => c.VendedorID == iUsuarioID);
                if (oMetaVen != null && Datos.Exists<Usuario>(c => c.UsuarioID == iUsuarioID
                    && (c.TipoUsuarioID == Cat.TiposDeUsuario.Vendedor || c.TipoUsuarioID == Cat.TiposDeUsuario.Gerente) && c.Estatus))
                {
                    int iSucursalID = Util.Entero(oFila.Cells["SucursalID"].Value);
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
                        decimal mSueldoFijo = Util.Decimal(oFila.Cells["SueldoFijo"].Value);
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

        private void LlenarNominaHistorico(int iNominaID)
        {
            Cargando.Mostrar();

            var oNomina = Datos.GetEntity<Nomina>(c => c.NominaID == iNominaID);
            var oNomUsuariosV = Datos.GetListOf<NominaUsuariosView>(c => c.NominaID == oNomina.NominaID);
            var oNomUsuariosOficial = Datos.GetListOf<NominaUsuarioOficial>(c => c.NominaID == oNomina.NominaID);
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
                    var oCuentaDeMayor = Datos.GetEntity<ContaCuentaDeMayor>(c => c.ContaCuentaDeMayorID == oImporteC.ContaCuentaDeMayorID);
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
                // oFila.Cells["Diferencia"].Value = (Util.ConvertirDecimal(oFila.Cells["TotalSueldo"].Value) - mTotalOficial);
                // oFila.Cells["Liquido"].Value = (Util.ConvertirDecimal(oFila.Cells["Diferencia"].Value) - Util.ConvertirDecimal(oFila.Cells["TotalDescuentos"].Value));
            }

            Cargando.Cerrar();
        }

        private void CalcularTotales(DataGridViewRow oFila)
        {
            decimal mOficial = Util.Decimal(oFila.Cells["TotalOficial"].Value);
            decimal mSueldo = (
                (oFila.Cells["SueldoFijo"].Style.ForeColor == Color.Gray ? 0 : Util.Decimal(oFila.Cells["SueldoFijo"].Value))
                + (oFila.Cells["SueldoVariable"].Style.ForeColor == Color.Gray ? 0 : Util.Decimal(oFila.Cells["SueldoVariable"].Value))
                + Util.Decimal(oFila.Cells["Sueldo9500"].Value)
                + (oFila.Cells["SueldoMinimo"].Style.ForeColor == Color.Gray ? 0 : Util.Decimal(oFila.Cells["SueldoMinimo"].Value))
                + Util.Decimal(oFila.Cells["Bono"].Value)
                + Util.Decimal(oFila.Cells["Adicional"].Value)
            );
            oFila.Cells["TotalSueldo"].Value = mSueldo;
            decimal mDiferencia = (mSueldo - mOficial);
            oFila.Cells["Diferencia"].Value = mDiferencia;
            decimal mDescuentos = (
                Util.Decimal(oFila.Cells["Tickets"].Value)
                + Util.Decimal(oFila.Cells["Adelanto"].Value)
                + Util.Decimal(oFila.Cells["MinutosTarde"].Value)
                + Util.Decimal(oFila.Cells["Otros"].Value)
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
                if (!Util.Logico(oFila.Cells["Sel"].Value))
                    continue;

                int iSucursalID = Util.Entero(oFila.Cells["SucursalID"].Value);
                if (!oSucursales.ContainsKey(iSucursalID))
                    oSucursales.Add(iSucursalID, 0);

                oSucursales[iSucursalID] += Util.Decimal(oFila.Cells["Diferencia"].Value);
                oSucursales[iSucursalID] -= Util.Decimal(oFila.Cells["MinutosTarde"].Value);
                oSucursales[iSucursalID] -= Util.Decimal(oFila.Cells["Otros"].Value);
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
            int iBancoCuentaID = Util.Entero(this.cmbCuentaBancaria.SelectedValue);
            DateTime dAhora = DateTime.Now;
            DateTime dOficial = this.dtpOficial.Value;
            DateTime dComplementaria = this.dtpComplementaria.Value;
            string sDiaOficial = dOficial.ToString("yyMMdd");
            string sDiaComplementaria = dComplementaria.ToString("yyMMdd");
            DateTime dSemana = Util.FechaHora(this.cmbSemana.SelectedValue);
            bool bEsAguinaldo = (this.cmbSemana.Text == "AGUINALDO");
            var oNomina = new Nomina()
            {
                Semana = dSemana,
                Fecha = dAhora,
                BancoCuentaID = iBancoCuentaID,
                EsAguinaldo = bEsAguinaldo
            };
            Datos.Guardar<Nomina>(oNomina);

            // Se guarda el detalle
            decimal mTotalOficial = 0;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Util.Logico(oFila.Cells["Sel"].Value))
                    continue;

                int iUsuarioID = Util.Entero(oFila.Cells["UsuarioID"].Value);
                int iSucursalID = Util.Entero(oFila.Cells["SucursalID"].Value);
                // Se llenan los datos de la nómina oficial (dinámico)
                for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
                {
                    var oNominaOfCuentaV = (this.dgvDatos.Columns[iCol].Tag as NominaOficialCuentasView);
                    var oNominaOf = new NominaUsuarioOficial()
                    {
                        NominaID = oNomina.NominaID,
                        IdUsuario = iUsuarioID,
                        ContaCuentaDeMayorID = oNominaOfCuentaV.ContaCuentaDeMayorID,
                        Importe = Util.Decimal(oFila.Cells[iCol].Value),
                        Suma = oNominaOfCuentaV.Suma
                    };
                    Datos.Guardar<NominaUsuarioOficial>(oNominaOf);
                }
                // Se guardan los datos no dinámicos
                var oNominaFijo = new NominaUsuario()
                {
                    NominaID = oNomina.NominaID,
                    IdUsuario = iUsuarioID,
                    SucursalID = Util.Entero(oFila.Cells["SucursalID"].Value),
                    SuperoMinimo = (oFila.Cells["SueldoMinimo"].Style.ForeColor == Color.Gray),
                    SueldoFijo = Util.Decimal(oFila.Cells["SueldoFijo"].Value),
                    SueldoVariable = Util.Decimal(oFila.Cells["SueldoVariable"].Value),
                    Sueldo9500 = Util.Decimal(oFila.Cells["Sueldo9500"].Value),
                    SueldoMinimo = Util.Decimal(oFila.Cells["SueldoMinimo"].Value),
                    Bono = Util.Decimal(oFila.Cells["Bono"].Value),
                    Adicional = Util.Decimal(oFila.Cells["Adicional"].Value),
                    Tickets = Util.Decimal(oFila.Cells["Tickets"].Value),
                    Adelanto = Util.Decimal(oFila.Cells["Adelanto"].Value),
                    MinutosTarde = Util.Decimal(oFila.Cells["MinutosTarde"].Value),
                    Otros = Util.Decimal(oFila.Cells["Otros"].Value)
                };
                Datos.Guardar<NominaUsuario>(oNominaFijo);

                // Se generan los gastos contables correspondientes, de lo oficial
                for (int iCol = this.iColumnasFijas; iCol < this.dgvDatos.Columns.Count; iCol++)
                {
                    decimal mImporte = Util.Decimal(oFila.Cells[iCol].Value);
                    var oNominaOfCuentaV = (this.dgvDatos.Columns[iCol].Tag as NominaOficialCuentasView);
                    int iCuentaDeMayorID = oNominaOfCuentaV.ContaCuentaDeMayorID;
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == iCuentaDeMayorID && c.RelacionID == iUsuarioID);
                    if (oCuentaAux == null)
                        continue;
                    var oGasto = new ContaEgreso()
                    {
                        ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID,
                        Fecha = dOficial,
                        Importe = mImporte,
                        TipoFormaPagoID = Cat.FormasDePago.Transferencia,
                        FolioDePago = sDiaOficial,
                        TipoDocumentoID = Cat.TiposDeDocumento.Factura,
                        EsFiscal = true,
                        Observaciones = ("NÓMINA " + this.cmbSemana.Text),
                        SucursalID = iSucursalID,
                        RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                        FolioFactura = sDiaOficial,
                        BancoCuentaID = iBancoCuentaID
                    };
                    ContaProc.GastoCrear(oGasto);
                }
                // Se genera el gasto contable por la diferencia, si aplica
                decimal mDiferencia = Util.Decimal(oFila.Cells["Diferencia"].Value);
                if (mDiferencia != 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == iUsuarioID);
                    if (oCuentaAux != null)
                    {
                        var oGasto = new ContaEgreso()
                        {
                            ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID,
                            Fecha = dComplementaria,
                            Importe = mDiferencia,
                            TipoFormaPagoID = Cat.FormasDePago.Efectivo,
                            TipoDocumentoID = Cat.TiposDeDocumento.Nota,
                            EsFiscal = false,
                            Observaciones = ("CN " + this.cmbSemana.Text),
                            SucursalID = iSucursalID,
                            RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                            FolioFactura = sDiaComplementaria,
                        };
                        ContaProc.GastoCrear(oGasto);
                    }
                }

                mTotalOficial += Util.Decimal(oFila.Cells["TotalOficial"].Value);
            }

            // Se genera el moviemiento bancario, con lo oficial
            var oMov = new BancoCuentaMovimiento()
            {
                BancoCuentaID = iBancoCuentaID,
                EsIngreso = false,
                Fecha = dOficial,
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
            var oNominaUsuariosV = Datos.GetListOf<NominaUsuariosView>(c => c.NominaID == oNomina.NominaID);
            var oPolizaBase = new ContaPoliza()
            {
                ContaTipoPolizaID = Cat.ContaTiposDePoliza.Egreso,
                Fecha = dComplementaria,
                Concepto = "",
                RelacionTabla = Cat.Tablas.NominaUsuario,
                RelacionID = oNomina.NominaID
            };
            foreach (var oReg in oNominaUsuariosV)
            {
                // Se crea la póliza de lo oficial
                ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.NominaOficial, oReg.NominaUsuarioID, this.cmbSemana.Text, ("NÓMINA " + this.cmbSemana.Text)
                    , oReg.SucursalID, dOficial);
                // Se crea la póliza de la diferencia, si aplica
                if (oReg.Diferencia != 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux == null)
                    {
                        UtilLocal.MensajeAdvertencia("No se encontró la cuenta auxiliar: Salarios -> " + oReg.Usuario + ". No se creará la póliza de Complemento Nómina.");
                        continue;
                    }
                    var oPoliza = Util.CrearCopia<ContaPoliza>(oPolizaBase);
                    oPoliza.Concepto = ("COMPLEMENTO NÓMINA " + this.cmbSemana.Text);
                    ContaProc.CrearPoliza(oPoliza, oCuentaAux.ContaCuentaAuxiliarID, Cat.ContaCuentasAuxiliares.ReservaNomina, oReg.Diferencia.Valor(), oReg.Usuario
                        , oReg.SucursalID, Cat.Sucursales.Matriz);
                    /* ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, ("COMPLEMENTO NÓMINA " + this.cmbSemana.Text)
                        , oCuentaAux.ContaCuentaAuxiliarID, Cat.ContaCuentasAuxiliares.ReservaNomina, oReg.Diferencia.Valor(), oReg.Usuario
                        , Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                    */
                }

                // Se crea la póliza del adelanto, si aplica
                if (oReg.Adelanto > 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.DeudoresDiversos
                        && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux != null)
                    {
                        // ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Ingreso, "ADELANTO", Cat.ContaCuentasAuxiliares.Caja, oCuentaAux.ContaCuentaAuxiliarID
                        //    , oReg.Adelanto, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor(), oReg.SucursalID);
                        var oPoliza = Util.CrearCopia<ContaPoliza>(oPolizaBase);
                        oPoliza.ContaTipoPolizaID = Cat.ContaTiposDePoliza.Ingreso;
                        oPoliza.Concepto = "ADELANTO";
                        oPoliza.SucursalID = oReg.SucursalID;
                        ContaProc.CrearPoliza(oPoliza, Cat.ContaCuentasAuxiliares.Caja, oCuentaAux.ContaCuentaAuxiliarID, oReg.Adelanto, oReg.Usuario
                            , Cat.Sucursales.Matriz, oReg.SucursalID);

                        // Se crea adicionalmente, un ingreso de caja por el importe del adelanto
                        var oIngreso = new CajaIngreso()
                        {
                            CajaTipoIngresoID = Cat.CajaTiposDeIngreso.Otros,
                            Concepto = ("PAGO ADELANTO " + oReg.Usuario),
                            Importe = oReg.Adelanto,
                            Fecha = dAhora,
                            SucursalID = oReg.SucursalID,
                            RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                        };
                        Datos.Guardar<CajaIngreso>(oIngreso);
                    }
                }
                // Se crea la póliza de minutos tarde y otros, si aplica
                if (oReg.MinutosTarde > 0 || oReg.Otros > 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios
                        && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux != null)
                    {
                        if (oReg.MinutosTarde > 0)
                        {
                            // ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "MINUTOS TARDE", Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID
                            //     , oReg.MinutosTarde, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                            var oPoliza = Util.CrearCopia<ContaPoliza>(oPolizaBase);
                            oPoliza.Concepto = "MINUTOS TARDE";
                            ContaProc.CrearPoliza(oPoliza, Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID, oReg.MinutosTarde, oReg.Usuario
                                , Cat.Sucursales.Matriz, oReg.SucursalID);
                        }
                        if (oReg.Otros > 0)
                        {
                            // ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "OTROS DESCUENTOS", Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID
                            //     , oReg.Otros, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor());
                            var oPoliza = Util.CrearCopia<ContaPoliza>(oPolizaBase);
                            oPoliza.Concepto = "OTROS DESCUENTOS";
                            ContaProc.CrearPoliza(oPoliza, Cat.ContaCuentasAuxiliares.ReservaNomina, oCuentaAux.ContaCuentaAuxiliarID, oReg.Otros, oReg.Usuario
                                , Cat.Sucursales.Matriz, oReg.SucursalID);
                        }
                    }
                }
            }

            // Se genera el resguardo y refuerzo especiales
            /* Este procedimiento hacía crecer el Resguardo, porque el dinero no se tomaba de ReservaNómina y se 
             * mandaba a Resguardo cuando en realidad quedaba en el salario de los usuarios 17/11/2015
            var oResguardos = new Dictionary<int, decimal>();
            var oRefuerzos = new Dictionary<int, decimal>();
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Util.ConvertirBool(oFila.Cells["Sel"].Value))
                    continue;

                int iSucursalID = Util.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                if (!oResguardos.ContainsKey(iSucursalID))
                    oResguardos.Add(iSucursalID, 0);
                if (!oRefuerzos.ContainsKey(iSucursalID))
                    oRefuerzos.Add(iSucursalID, 0);

                decimal mDiferencia = Util.ConvertirDecimal(oFila.Cells["Diferencia"].Value);
                decimal mTickets = Util.ConvertirDecimal(oFila.Cells["Tickets"].Value);
                decimal mMinutosTarde = Util.ConvertirDecimal(oFila.Cells["MinutosTarde"].Value);
                decimal mOtros = Util.ConvertirDecimal(oFila.Cells["Otros"].Value);
                decimal mAdelanto = Util.ConvertirDecimal(oFila.Cells["Adelanto"].Value);
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
            */

            // Se manda a imprimir la nómina de cada usuario
            var oNominaUsuariosOfV = Datos.GetListOf<NominaUsuariosOficialView>(c => c.NominaID == oNomina.NominaID);
            var oNominaOficialTotales = oNominaUsuariosOfV.GroupBy(c => c.UsuarioID).Select(c => new
            {
                UsuarioID = c.Key,
                Ingreso = c.Where(s => s.Suma).Sum(s => s.Importe),
                Egreso = c.Where(s => !s.Suma).Sum(s => s.Importe)
            });
            var oRep = new Report();
            oRep.Load(UtilLocal.RutaReportes("Nomina.frx"));
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
            Cargando.Mostrar();

            // Se guarda la nómina
            int iBancoCuentaID = Util.Entero(this.cmbCuentaBancaria.SelectedValue);
            string sDia = DateTime.Now.ToString("yyMMdd");
            var oNomina = new Nomina()
            {
                Semana = DateTime.Now,
                Fecha = DateTime.Now,
                BancoCuentaID = iBancoCuentaID,
                Domingo = true
            };
            Datos.Guardar<Nomina>(oNomina);

            // Se guarda el detalle
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Util.Logico(oFila.Cells["Sel"].Value))
                    continue;

                int iUsuarioID = Util.Entero(oFila.Cells["UsuarioID"].Value);
                int iSucursalID = Util.Entero(oFila.Cells["SucursalID"].Value);
                // Se guardan los datos
                var oNominaFijo = new NominaUsuario()
                {
                    NominaID = oNomina.NominaID,
                    IdUsuario = iUsuarioID,
                    SucursalID = Util.Entero(oFila.Cells["SucursalID"].Value),
                    Adicional = Util.Decimal(oFila.Cells["Adicional"].Value),
                    Tickets = Util.Decimal(oFila.Cells["Tickets"].Value),
                    Adelanto = Util.Decimal(oFila.Cells["Adelanto"].Value),
                    MinutosTarde = Util.Decimal(oFila.Cells["MinutosTarde"].Value),
                    Otros = Util.Decimal(oFila.Cells["Otros"].Value)
                };
                Datos.Guardar<NominaUsuario>(oNominaFijo);

                // Se genera el gasto contable por la diferencia, si aplica
                decimal mDiferencia = Util.Decimal(oFila.Cells["Diferencia"].Value);
                if (mDiferencia != 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == iUsuarioID);
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
            var oNominaUsuariosV = Datos.GetListOf<NominaUsuariosView>(c => c.NominaID == oNomina.NominaID);
            foreach (var oReg in oNominaUsuariosV)
            {
                // Se crea la póliza de la diferencia, si aplica
                if (oReg.Diferencia != 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios && c.RelacionID == oReg.UsuarioID);
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
                DateTime dAhora = DateTime.Now;
                if (oReg.Adelanto > 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.DeudoresDiversos
                        && c.RelacionID == oReg.UsuarioID);
                    if (oCuentaAux != null)
                    {
                        ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Egreso, "ADELANTO", Cat.ContaCuentasAuxiliares.Caja, oCuentaAux.ContaCuentaAuxiliarID
                            , oReg.Adelanto, oReg.Usuario, Cat.Tablas.NominaUsuario, oReg.NominaID.Valor(), oReg.SucursalID);
                        // Se crea adicionalmente, un ingreso de caja por el importe del adelanto
                        var oIngreso = new CajaIngreso()
                        {
                            CajaTipoIngresoID = Cat.CajaTiposDeIngreso.Otros,
                            Concepto = ("PAGO ADELANTO " + oReg.Usuario),
                            Importe = oReg.Adelanto,
                            Fecha = dAhora,
                            SucursalID = oReg.SucursalID,
                            RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                        };
                        Datos.Guardar<CajaIngreso>(oIngreso);
                    }
                }
                // Se crea la póliza de minutos tarde y otros, si aplica
                if (oReg.MinutosTarde > 0 || oReg.Otros > 0)
                {
                    var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios
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

            // Se manda a imprimir la nómina de cada usuario
            var oRep = new Report();
            oRep.Load(UtilLocal.RutaReportes("NominaDomingo.frx"));
            oRep.RegisterData(new List<Nomina>() { oNomina }, "Nomina");
            oRep.RegisterData(oNominaUsuariosV, "Usuarios");

            Cargando.Cerrar();
            UtilLocal.EnviarReporteASalida("Reportes.NominaDomingo.Salida", oRep);


            UtilLocal.MostrarNotificacion("Proceso completado.");
            this.btnDomingo_Click(this, null);

            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Util.Entero(this.cmbCuentaBancaria.SelectedValue) == 0)
                this.ctlError.PonerError(this.cmbCuentaBancaria, "Debes especificar la cuenta bancaria.");
            return this.ctlError.Valido;
        }

        private void GenerarDesgloseLiquido()
        {
            var oDesglose = new List<ConteoMonedasUsuario>();
            var oMonedas = new Dictionary<decimal, int>();
            var oListaMon = new List<decimal>() { 500, 200, 100, 50, 20, 10, 5, 2, 1, 0.5M, 0.2M, 0.1M };

            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (!Util.Logico(oFila.Cells["Sel"].Value))
                    continue;

                // Se limpian los datos de monedas
                foreach (decimal mMoneda in oListaMon)
                    oMonedas[mMoneda] = 0;
                //
                decimal mLiquido = Util.Decimal(oFila.Cells["Liquido"].Value);
                mLiquido = Math.Round(mLiquido, 1);
                // Se calculan las moendas del usuario
                foreach (decimal mMon in oListaMon)
                {
                    oMonedas[mMon] = (int)(mLiquido / mMon);
                    mLiquido -= (mMon * oMonedas[mMon]);
                    if (mLiquido <= 0)
                        break;
                }

                // Se agrega el usuario a la lista
                oDesglose.Add(new ConteoMonedasUsuario()
                {
                    UsuarioID = Util.Entero(oFila.Cells["UsuarioID"].Value),
                    Usuario = Util.Cadena(oFila.Cells["Usuario"].Value),
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

            var oUsuarios = Datos.GetListOf<UsuariosNominaView>();
            int iTipo = Util.Entero(this.cmbImpTipo.SelectedValue);

            //si se selecciona diciembre, se muestra diciembre del año anterior y no del año actual
            int oAño = 0;
            if (Util.Entero(this.cmbImpPeriodo.SelectedValue) == 12 || (iTipo == 44 && Util.Entero(this.cmbImpPeriodo.SelectedValue) == 11) || (iTipo == 5 && Util.Entero(this.cmbImpPeriodo.SelectedValue) == 12))
            {
                oAño = DateTime.Now.AddYears(-1).Year;
            }
            else
            {
                oAño = DateTime.Now.Year;
            }
            //DateTime dDesde = new DateTime(DateTime.Now.Year, Util.Entero(this.cmbImpPeriodo.SelectedValue), 1);
            DateTime dDesde = new DateTime(oAño, Util.Entero(this.cmbImpPeriodo.SelectedValue), 1);
            DateTime dHasta = (iTipo == Cat.ContaCuentasDeMayor.Imss ? dDesde.DiaUltimo() : dDesde.AddMonths(1).DiaUltimo()).AddDays(1);
            var oNominasPer = Datos.GetListOf<NominaUsuariosOficialView>(c => c.Semana >= dDesde && c.Semana <= dHasta);
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
                mRetenido += Util.Decimal(oFila.Cells["imp_Retenido"].Value);
                mGasto += Util.Decimal(oFila.Cells["imp_Gasto"].Value);
                mTotal += Util.Decimal(oFila.Cells["imp_Total"].Value);
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
                ContaCuentaDeMayorID = Util.Entero(this.cmbImpTipo.SelectedValue),
                Fecha = this.dtpImpFecha.Value,
                Periodo = Util.Entero(this.cmbImpPeriodo.SelectedValue),
                BancoCuentaID = Util.Entero(this.cmbImpCuenta.SelectedValue),
                TipoFormaPagoID = Util.Entero(this.cmbImpFormaDePago.SelectedValue),
                FolioDePago = this.txtImpFolioDePago.Text
            };
            Datos.Guardar<NominaImpuesto>(oImpuesto);

            string sPeriodo = this.cmbImpPeriodo.Text;
            decimal mImporteTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvImpDatos.Rows)
            {
                if (!Util.Logico(oFila.Cells["imp_Sel"].Value))
                    continue;

                // Se inserta el impuesto usuario
                var oImpUsuario = new NominaImpuestoUsuario()
                {
                    NominaImpuestoID = oImpuesto.NominaImpuestoID,
                    IdUsuario = Util.Entero(oFila.Cells["imp_UsuarioID"].Value),
                    SucursalID = Util.Entero(oFila.Cells["imp_SucursalID"].Value),
                    Retenido = Util.Decimal(oFila.Cells["imp_Retenido"].Value),
                    Total = Util.Decimal(oFila.Cells["imp_Total"].Value)
                };
                Datos.Guardar<NominaImpuestoUsuario>(oImpUsuario);
                mImporteTotal += oImpUsuario.Total;

                // Se inserta el gasto contable
                var oCuentaAux = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaDeMayorID == oImpuesto.ContaCuentaDeMayorID && c.RelacionID == oImpUsuario.IdUsuario);
                if (oCuentaAux == null)
                {
                    UtilLocal.MensajeAdvertencia("El usuario " + Util.Cadena(oFila.Cells["imp_Usuario"].Value)
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
                            , ("PAGO 2% " + sPeriodo), oImpuesto.Fecha);
                        break;
                    case Cat.ContaCuentasDeMayor.Imss:
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoImss, oImpUsuario.NominaImpuestoUsuarioID, oImpuesto.FolioDePago
                            , ("PAGO IMSS " + sPeriodo), oImpuesto.Fecha);
                        break;
                    case Cat.ContaCuentasDeMayor.Infonavit:
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.PagoInfonavit, oImpUsuario.NominaImpuestoUsuarioID, oImpuesto.FolioDePago
                            , ("PAGO INFONAVIT " + sPeriodo), oImpuesto.Fecha);
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
            if (Util.Entero(this.cmbImpTipo.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpTipo, "Debes especificar el tipo de impuesto.");
            if (Util.Entero(this.cmbImpPeriodo.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpPeriodo, "Debes especificar el período.");
            if (Util.Entero(this.cmbImpCuenta.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpCuenta, "Debes especificar la cuenta bancaria.");
            if (Util.Entero(this.cmbImpFormaDePago.SelectedValue) <= 0)
                this.ctlError.PonerError(this.cmbImpFormaDePago, "Debes especificar la forma de pago.");
            if (this.txtImpFolioDePago.Text == "")
                this.ctlError.PonerError(this.txtImpFolioDePago, "Debes especificar un folio de pago.");
            // Se valida que haya al menos un usuario seleccionado
            bool bSel = false;
            foreach (DataGridViewRow oFila in this.dgvImpDatos.Rows)
            {
                if (Util.Logico(oFila.Cells["imp_Sel"].Value))
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

        #region [ Comisiones ]

        bool bCargandoComisiones;

        private void tgvComisiones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.tgvComisiones.Columns[e.ColumnIndex] == this.com_Entidad && this.tgvComisiones.CurrentNode.Level == CapitalHumano.iNivelLinea)
            {
                this.CargarPartesLinea(this.tgvComisiones.CurrentNode, true);
            }
        }

        private void tgvComisiones_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.bCargandoComisiones || this.tgvComisiones.CurrentNode == null) return;
            this.VerMarcarCambio(this.tgvComisiones.CurrentNode.Cells[e.ColumnIndex]);
        }

        private void btnGuardarCom_Click(object sender, EventArgs e)
        {
            this.GuardarDatosComisiones();
        }

        private void CargarArbolDeComisiones()
        {
            Cargando.Mostrar();
            this.bCargandoComisiones = true;


            string sProveedor = "", sMarca = "", sLinea = "";
            TreeGridNode oNodoProveedor = null, oNodoMarca = null, oNodoLinea = null, oNodoArticulo = null;
            var oDatos = Datos.GetListOf<PartesComisionesPrevioView>().OrderBy(c => c.Proveedor).ThenBy(c => c.Marca).ThenBy(c => c.Linea).ThenBy(c => c.Parte);
            this.tgvComisiones.Nodes.Clear();
            foreach (var oReg in oDatos)
            {

                if (oReg.Proveedor != sProveedor)
                {
                    sProveedor = oReg.Proveedor;
                    oNodoProveedor = this.tgvComisiones.Nodes.Add(oReg.ParteComisionID, oReg.ProveedorID, sProveedor, oReg.PorcentajeNormal, oReg.ComisionFija
                        , oReg.PorcentajeUnArticulo, oReg.ArticulosEspecial, oReg.PorcentajeArticulosEspecial, oReg.PorcentajeComplementarios
                        , oReg.PorcentajeReduccionPorRepartidor, oReg.PorcentajeRepartidor, oReg.ComisionFijaRepartidor);
                    oNodoProveedor.Expand();
                    continue;
                }
                if (oReg.Marca != sMarca)
                {
                    sMarca = oReg.Marca;
                    oNodoMarca = oNodoProveedor.Nodes.Add(oReg.ParteComisionID, oReg.MarcaID, sMarca, oReg.PorcentajeNormal, oReg.ComisionFija
                        , oReg.PorcentajeUnArticulo, oReg.ArticulosEspecial, oReg.PorcentajeArticulosEspecial, oReg.PorcentajeComplementarios
                        , oReg.PorcentajeReduccionPorRepartidor, oReg.PorcentajeRepartidor, oReg.ComisionFijaRepartidor);
                    //oscar 201216
                    //oNodoMarca.Expand();
                    continue;
                }
                if (oReg.Linea != sLinea)
                {
                    sLinea = oReg.Linea;

                    oNodoLinea = oNodoMarca.Nodes.Add(oReg.ParteComisionID, oReg.LineaID, sLinea, oReg.PorcentajeNormal, oReg.ComisionFija
                        , oReg.PorcentajeUnArticulo, oReg.ArticulosEspecial, oReg.PorcentajeArticulosEspecial, oReg.PorcentajeComplementarios
                        , oReg.PorcentajeReduccionPorRepartidor, oReg.PorcentajeRepartidor, oReg.ComisionFijaRepartidor);

                    //oscar201216
                    //oNodoLinea.Expand();

                    //var oDatos2 = Datos.GetListOf<PartesComisionesView>(c => c.LineaID == oReg.LineaID && c.ProveedorID == oReg.ProveedorID && c.MarcaParteID == oReg.MarcaID && c.ParteID > 0).OrderBy(c => c.ParteID);

                    //////oNodo2.Nodes.Clear();

                    //////comentado oscar 141216
                    //foreach (var oReg2 in oDatos2)
                    ////--
                    ////foreach (var oReg in hs)
                    //{
                    //    oNodoArticulo = oNodoLinea.Nodes.Add(oReg2.ParteComisionID, oReg2.ParteID, oReg2.Parte
                    //        , oReg2.PorcentajeNormal, oReg2.ComisionFija
                    //        , oReg2.PorcentajeUnArticulo, oReg2.ArticulosEspecial, oReg2.PorcentajeArticulosEspecial, oReg2.PorcentajeComplementarios
                    //        , oReg2.PorcentajeReduccionPorRepartidor, oReg2.PorcentajeRepartidor, oReg2.ComisionFijaRepartidor);
                    //}


                    continue;
                }


                //int iLineaID = Util.Entero(oNodoProveedor.Cells[CapitalHumano.iCol_com_Id].Value);
                //int iMarcaID = Util.Entero(oNodoProveedor.Parent.Cells["com_Id"].Value);
                //int iProveedorID = Util.Entero(oNodoProveedor.Parent.Parent.Cells["com_Id"].Value);



                ////if (bExpandir)
                ////    oNodo.Expand();


                // Se carga sólo hasta líneas, para que sea más rápido
                /*
                oNodoLinea.Nodes.Add(oReg.ParteComisionID, oReg.ParteID, oReg.Parte, oReg.PorcentajeNormal, oReg.ComisionFija
                    , oReg.PorcentajeUnArticulo, oReg.ArticulosEspecial, oReg.PorcentajeArticulosEspecial, oReg.PorcentajeComplementarios
                    , oReg.PorcentajeReduccionPorRepartidor, oReg.PorcentajeRepartidor, oReg.ComisionFijaRepartidor);
                */
            }

            //CargarPartesLinea(oNodoProveedor, true);

            this.bCargandoComisiones = false;
            Cargando.Cerrar();
        }

        private void CargarPartesLinea(TreeGridNode oNodo, bool bExpandir)
        {
            Cargando.Mostrar();

            int iLineaID = Util.Entero(oNodo.Cells[CapitalHumano.iCol_com_Id].Value);

            //editado oscar 131216
            int iMarcaID = Util.Entero(oNodo.Parent.Cells["com_Id"].Value);
            int iProveedorID = Util.Entero(oNodo.Parent.Parent.Cells["com_Id"].Value);

            //editado oscar 121216
            //var oDatos = Datos.GetListOf<PartesComisionesView>(c => c.LineaID == iLineaID && c.ParteID > 0).OrderBy(c => c.ParteID);

            var oDatos = Datos.GetListOf<PartesComisionesView>(c => c.LineaID == iLineaID && c.ProveedorID == iProveedorID && c.MarcaParteID == iMarcaID && c.ParteID > 0).OrderBy(c => c.ParteID);

            //agregado oscar141216
            //HashSet<PartesComisionesView> hs = new HashSet<PartesComisionesView>();
            //foreach (var i in oDatos)
            //{
            //    hs.Add(i);
            //}
            //141216


            oNodo.Nodes.Clear();

            //comentado oscar 141216
            foreach (var oReg in oDatos)
            //--
            //foreach (var oReg in hs)
            {
                oNodo.Nodes.Add(oReg.ParteComisionID, oReg.ParteID, oReg.Parte
                    , oReg.PorcentajeNormal, oReg.ComisionFija
                    , oReg.PorcentajeUnArticulo, oReg.ArticulosEspecial, oReg.PorcentajeArticulosEspecial, oReg.PorcentajeComplementarios
                    , oReg.PorcentajeReduccionPorRepartidor, oReg.PorcentajeRepartidor, oReg.ComisionFijaRepartidor);
            }

            if (bExpandir)
                oNodo.Expand();

            Cargando.Cerrar();
        }

        private void VerMarcarCambio(DataGridViewCell oCelda)
        {
            var oNodo = this.tgvComisiones.GetNodeForRow(oCelda.OwningRow);

            // Se verifica si ya se marcó la fila como modificada, en base al tag
            if (oNodo.Level == CapitalHumano.iNivelParte && oCelda.Tag != null)
                return;

            // Se mandan cargar las partes de cada línea encontrada en la selección
            this.CargarPartesLineasEnCascada(oNodo);
            // Se manda afectar los nodos hijos
            this.VerCambiosEnCascada(oNodo, oCelda.ColumnIndex, oCelda.Value);
            // Se marca la fila como modificada
            oCelda.Style.ForeColor = Color.Orange;
            oCelda.Tag = true;
            oCelda.OwningRow.Tag = true;
        }

        private void CargarPartesLineasEnCascada(TreeGridNode oNodo)
        {
            if (oNodo.Level == CapitalHumano.iNivelLinea)
            {
                if (oNodo.Nodes.Count == 0)
                    this.CargarPartesLinea(oNodo, false);
            }
            else
            {
                /*
                if (oNodo.Nodes.Count > 0)
                {
                    foreach (var oNodoHijo in oNodo.Nodes)
                        this.CargarPartesLineasEnCascada(oNodoHijo);
                }
                */
            }
        }

        private void VerCargarPartesLineas(TreeGridNode oNodo)
        {
            if (oNodo.Level == CapitalHumano.iNivelLinea && oNodo.Nodes.Count == 0)
                this.CargarPartesLinea(oNodo, false);
        }

        private void VerCambiosEnCascada(TreeGridNode oNodo, int iCol, object oValor)
        {
            if (oNodo.Nodes.Count > 0)
            {
                var oCelda = oNodo.Cells[iCol];
                oCelda.Style.ForeColor = Color.Orange;
                oCelda.Tag = true;
                oCelda.OwningRow.Tag = true;
                oCelda.Value = oValor;

                foreach (var oNodoHijo in oNodo.Nodes)
                    this.VerCambiosEnCascada(oNodoHijo, iCol, oValor);
            }
            else
            {
                var oCelda = oNodo.Cells[iCol];
                oCelda.Style.ForeColor = Color.Orange;
                oCelda.Tag = true;
                oCelda.OwningRow.Tag = true;
                oCelda.Value = oValor;
            }
        }

        private void GuardarDatosComisiones()
        {
            //if (UtilLocal.MensajePreguntaCancelar("¿Estás seguro que deseas guardar los cambios realizados?") != DialogResult.Yes)
            //    return;

            //Cargando.Mostrar();

            foreach (DataGridViewRow oFila in this.tgvComisiones.Rows)
            {
                if (oFila.Tag == null) continue;

                var oNodo = this.tgvComisiones.GetNodeForRow(oFila);
                if (oNodo.Nodes.Count > 0)
                    oNodo.Expand();
                int? iProveedorID = null, iMarcaID = null, iLineaID = null, iParteID = null;
                switch (oNodo.Level)
                {
                    case 1:
                        iProveedorID = (int)oNodo.Cells["com_Id"].Value;
                        break;
                    case 2:
                        iProveedorID = (int)oNodo.Parent.Cells["com_Id"].Value;
                        iMarcaID = (int)oNodo.Cells["com_Id"].Value;
                        break;
                    case 3:
                        iProveedorID = (int)oNodo.Parent.Parent.Cells["com_Id"].Value;
                        iMarcaID = (int)oNodo.Parent.Cells["com_Id"].Value;
                        iLineaID = (int)oNodo.Cells["com_Id"].Value;
                        break;
                    case 4:
                        iProveedorID = (int)oNodo.Parent.Parent.Parent.Cells["com_Id"].Value;
                        iMarcaID = (int)oNodo.Parent.Parent.Cells["com_Id"].Value;
                        iLineaID = (int)oNodo.Parent.Cells["com_Id"].Value;
                        iParteID = (int)oNodo.Cells["com_Id"].Value;
                        break;
                }

                int iParteComisionID = Util.Entero(oNodo.Cells["com_ParteComisionID"].Value);
                ParteComision oParteComision;
                if (iParteComisionID > 0)
                {
                    oParteComision = Datos.GetEntity<ParteComision>(c => c.ParteComisionID == iParteComisionID);
                }
                else
                {
                    oParteComision = new ParteComision() { ProveedorID = iProveedorID.Value, MarcaParteID = iMarcaID, LineaID = iLineaID, ParteID = iParteID };
                    oParteComision.ArticulosEspecial = 0;
                    oParteComision.ComisionFija = oParteComision.PorcentajeNormal = oParteComision.PorcentajeUnArticulo =
                        oParteComision.PorcentajeArticulosEspecial = oParteComision.PorcentajeComplementarios =
                        oParteComision.PorcentajeReduccionPorRepartidor = oParteComision.PorcentajeRepartidor = oParteComision.ComisionFijaRepartidor = 0;
                }

                if (oNodo.Cells["com_ComisionFija"].Tag != null)
                    oParteComision.ComisionFija = Util.Decimal(oNodo.Cells["com_ComisionFija"].Value);
                if (oNodo.Cells["com_PorcentajeNormal"].Tag != null)
                    oParteComision.PorcentajeNormal = Util.Decimal(oNodo.Cells["com_PorcentajeNormal"].Value);
                if (oNodo.Cells["com_PorcentajeUnArticulo"].Tag != null)
                    oParteComision.PorcentajeUnArticulo = Util.Decimal(oNodo.Cells["com_PorcentajeUnArticulo"].Value);
                if (oNodo.Cells["com_PorcentajeArticulosEspecial"].Tag != null)
                    oParteComision.PorcentajeArticulosEspecial = Util.Decimal(oNodo.Cells["com_PorcentajeArticulosEspecial"].Value);
                if (oNodo.Cells["com_PorcentajeComplementarios"].Tag != null)
                    oParteComision.PorcentajeComplementarios = Util.Decimal(oNodo.Cells["com_PorcentajeComplementarios"].Value);
                if (oNodo.Cells["com_PorcentajeReduccionPorRepartidor"].Tag != null)
                    oParteComision.PorcentajeReduccionPorRepartidor = Util.Decimal(oNodo.Cells["com_PorcentajeReduccionPorRepartidor"].Value);
                if (oNodo.Cells["com_PorcentajeRepartidor"].Tag != null)
                    oParteComision.PorcentajeRepartidor = Util.Decimal(oNodo.Cells["com_PorcentajeRepartidor"].Value);
                if (oNodo.Cells["com_ComisionFijaRepartidor"].Tag != null)
                    oParteComision.ComisionFijaRepartidor = Util.Decimal(oNodo.Cells["com_ComisionFijaRepartidor"].Value);
                if (oNodo.Cells["com_ArticulosEspecial"].Tag != null)
                    oParteComision.ArticulosEspecial = Util.Entero(oNodo.Cells["com_ArticulosEspecial"].Value);

                Datos.Guardar<ParteComision>(oParteComision);
            }

            //Cargando.Cerrar();

            this.CargarArbolDeComisiones();
        }

        #endregion

    }
}