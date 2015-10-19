using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CuentaContable : Form
    {
        public enum Tipo { Cuenta, Subcuenta, CuentaDeMayor, CuentaAuxiliar };

        bool EsMod;
        Tipo TipoCuenta;
        int CuentaID;
        int CuentaPadreID;
        object oCuenta;
        ControlError ctlError = new ControlError();

        public CuentaContable(bool bNuevaCuenta, Tipo eTipo, int iPadreOCuentaID)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;

            this.chkDevengarAutomaticamente.Checked = false;
            this.chkCalculoSemanal.Checked = false;

            this.EsMod = !bNuevaCuenta;
            this.TipoCuenta = eTipo;

            if (this.EsMod)
                this.CuentaID = iPadreOCuentaID;
            else
                this.CuentaPadreID = iPadreOCuentaID;

            // Se ajustan los controles y el alto del formulario
            this.Height = 146;
            switch (this.TipoCuenta)
            {
                case Tipo.CuentaDeMayor:
                    this.AjustarAdicional(this.pnlAdicionalCuentaDeMayor);
                    break;
                case Tipo.CuentaAuxiliar:
                    this.AjustarAdicional(this.pnlAdicional);
                    break;
            }
        }

        #region [ Eventos ]

        private void CuentaContable_Load(object sender, EventArgs e)
        {
            // Se llena el grid de sucursales, para devengado automático
            var oSucursales = General.GetListOf<Sucursal>(c => c.Estatus);
            foreach (var oSucursal in oSucursales)
                this.dgvDevSuc.Rows.Add(oSucursal.SucursalID, oSucursal.NombreSucursal);

            // Se llena el combo de devengado especial
            this.cmbDevengarEspecial.CargarDatos("DuenioID", "Duenio1", General.GetListOf<Duenio>());

            //
            string sTipoCuenta = "";
            switch (this.TipoCuenta)
            {
                case Tipo.Cuenta: sTipoCuenta = "Cuenta"; break;
                case Tipo.Subcuenta: sTipoCuenta = "Subcuenta"; break;
                case Tipo.CuentaDeMayor: sTipoCuenta = "Cuenta de Mayor"; break;
                case Tipo.CuentaAuxiliar: sTipoCuenta = "Cuenta Auxiliar"; break;
            }
            
            // Se llenan los datos, si aplica
            if (this.EsMod)
            {
                this.Text = ("Editar " + sTipoCuenta + " - " + this.CuentaID.ToString());
                this.LlenarDatos(this.CuentaID);
            }
            else
            {
                this.Text = ("Agregar " + sTipoCuenta);
            }
        }

        private void chkDevengarAutomaticamente_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.chkDevengarAutomaticamente.Focused)
                return;

            /*
            // Se valida si ya está visible o no el grid
            if ((this.chkDevengarAutomaticamente.Checked && this.dgvDevSuc.Visible) || (!this.chkDevengarAutomaticamente.Checked && !this.dgvDevSuc.Visible))
                return;
            //
            int iDif = (this.dgvDevSuc.Height + 3);
            if (this.chkDevengarAutomaticamente.Checked)
            {
                this.Height += iDif;
                this.lblDiasMovimiento.Top += iDif;
                this.txtMeses.Top += iDif;
                this.chkCalculoSemanal.Top += iDif;
            }
            else
            {
                this.Height -= iDif;
                this.chkCalculoSemanal.Top -= iDif;
                this.lblDiasMovimiento.Top -= iDif;
                this.txtMeses.Top -= iDif;
            }
            this.dgvDevSuc.Visible = this.chkDevengarAutomaticamente.Checked;
            */

            if (this.chkDevengarAutomaticamente.Checked)
                this.chkDevengarEspecial.Checked = false;
        }

        private void chkDevengarEspecial_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkDevengarEspecial.Focused)
            {
                if (this.chkDevengarEspecial.Checked)
                    this.chkDevengarAutomaticamente.Checked = false;
            }
        }

        private void chkCalculoSemanal_CheckedChanged(object sender, EventArgs e)
        {
            // Se valida si ya está visible o no lo correspondiente
            if ((this.chkCalculoSemanal.Checked && this.txtMeses.Visible) || (!this.chkCalculoSemanal.Checked && !this.txtMeses.Visible))
                return;
            //
            if (this.chkCalculoSemanal.Checked)
            {
                this.Height += (this.txtMeses.Height + this.dtpDejarDeSemanalizar.Height + 3 + 3);
                this.pnlAdicional.Height += (this.txtMeses.Height + this.dtpDejarDeSemanalizar.Height + 3 + 3);
            }
            else
            {
                this.Height -= (this.txtMeses.Height + this.dtpDejarDeSemanalizar.Height + 3 + 3);
                this.pnlAdicional.Height -= (this.txtMeses.Height + this.dtpDejarDeSemanalizar.Height + 3 + 3);
            }
            this.txtMeses.Visible = this.chkCalculoSemanal.Checked;
        }

        private void chkDejarDeSemanalizar_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpDejarDeSemanalizar.Enabled = this.chkDejarDeSemanalizar.Checked;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void AjustarAdicional(Control oAdicional)
        {
            // Se esconden los controles que pudiera haber
            this.pnlAdicional.Visible = false;
            this.pnlAdicionalCuentaDeMayor.Visible = false;
            // Se posiciona el control adicional, si aplica
            if (oAdicional != this.pnlAdicional)
                oAdicional.Location = this.pnlAdicional.Location;
            // Se ajusta el height de la forma
            oAdicional.Visible = true;
            this.Height += (oAdicional.Height + 3);
        }

        private void LlenarDatos(int iCuentaID)
        {
            switch (this.TipoCuenta)
            {
                case Tipo.Cuenta:
                    var oCuenta = General.GetEntity<ContaCuenta>(c => c.ContaCuentaID == iCuentaID);
                    this.txtCuenta.Text = oCuenta.Cuenta;
                    this.txtCuentaContpaq.Text = oCuenta.CuentaContpaq;
                    this.txtCuentaSat.Text = oCuenta.CuentaSat;
                    this.oCuenta = oCuenta;
                    break;
                case Tipo.Subcuenta:
                    var oSubcuenta = General.GetEntity<ContaSubcuenta>(c => c.ContaSubcuentaID == iCuentaID);
                    this.txtCuenta.Text = oSubcuenta.Subcuenta;
                    this.txtCuentaContpaq.Text = oSubcuenta.CuentaContpaq;
                    this.txtCuentaSat.Text = oSubcuenta.CuentaSat;
                    this.oCuenta = oSubcuenta;
                    break;
                case Tipo.CuentaDeMayor:
                    var oCuentaMay = General.GetEntity<ContaCuentaDeMayor>(c => c.ContaCuentaDeMayorID == iCuentaID);
                    this.txtCuenta.Text = oCuentaMay.CuentaDeMayor;
                    this.txtCuentaContpaq.Text = oCuentaMay.CuentaContpaq;
                    this.txtCuentaSat.Text = oCuentaMay.CuentaSat;
                    this.chkRestaInversa.Checked = oCuentaMay.RestaInversa.Valor();
                    this.oCuenta = oCuentaMay;
                    break;
                case Tipo.CuentaAuxiliar:
                    this.oCuenta = this.LlenarDatosCuentaAuxiliar(iCuentaID);
                    break;
            }
        }

        private object LlenarDatosCuentaAuxiliar(int iCuentaID)
        {
            var oCuentaAux = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == iCuentaID);
            this.txtCuenta.Text = oCuentaAux.CuentaAuxiliar;
            this.txtCuentaContpaq.Text = oCuentaAux.CuentaContpaq;
            this.txtCuentaSat.Text = oCuentaAux.CuentaSat;
            this.chkTieneDetalle.Checked = oCuentaAux.Detallable;
            this.chkVisibleEnCaja.Checked = oCuentaAux.VisibleEnCaja;
            // this.chkDevengarAutomaticamente.Checked = oCuentaAux.DevengarAut.Valor();

            // Se verifica si es devengado autómatico y de qué tipo es
            this.chkDevengarAutomaticamente.Checked = false;
            this.chkDevengarEspecial.Checked = false;
            if (oCuentaAux.DevengarAut.Valor())
            {
                this.chkDevengarAutomaticamente.Checked = true;
                var oDevAut = General.GetListOf<ContaCuentaAuxiliarDevengadoAutomatico>(c => c.ContaCuentaAuxiliarID == iCuentaID);
                foreach (var oReg in oDevAut)
                {
                    int iFila = this.dgvDevSuc.EncontrarIndiceDeValor("SucursalID", oReg.SucursalID);
                    this.dgvDevSuc["Porcentaje", iFila].Value = oReg.Porcentaje;
                }
            }
            else if (oCuentaAux.DevengarAutEsp.Valor())
            {
                this.chkDevengarEspecial.Checked = true;
                var oDevEsp = General.GetEntity<ContaCuentaAuxiliarDevengadoEspecial>(c => c.ContaCuentaAuxiliarID == oCuentaAux.ContaCuentaAuxiliarID);
                this.cmbDevengarEspecial.SelectedValue = oDevEsp.DuenioID;
            }

            this.chkCalculoSemanal.Checked = oCuentaAux.CalculoSemanal.Valor();
            this.txtMeses.Text = oCuentaAux.PeriodicidadMes.Valor().ToString();
            this.chkDejarDeSemanalizar.Checked = oCuentaAux.FinSemanalizar.HasValue;
            if (oCuentaAux.FinSemanalizar.HasValue)
                this.dtpDejarDeSemanalizar.Value = oCuentaAux.FinSemanalizar.Value;

            this.chkAfectaMetas.Checked = oCuentaAux.AfectaMetas.Valor();
            this.chkSumaGastosFijos.Checked = oCuentaAux.SumaGastosFijos.Valor();
            
            return oCuentaAux;
        }

        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            switch (this.TipoCuenta)
            {
                case Tipo.Cuenta:
                    var oCuenta = (this.EsMod ? (this.oCuenta as ContaCuenta) : (new ContaCuenta()));
                    oCuenta.Cuenta = this.txtCuenta.Text;
                    oCuenta.CuentaContpaq = this.txtCuentaContpaq.Text;
                    oCuenta.CuentaSat = this.txtCuentaSat.Text;
                    Guardar.Generico<ContaCuenta>(oCuenta);
                    break;
                case Tipo.Subcuenta:
                    var oSubcuenta = (this.EsMod ? (this.oCuenta as ContaSubcuenta) : (new ContaSubcuenta()));
                    oSubcuenta.ContaCuentaID = (this.EsMod ? oSubcuenta.ContaCuentaID : this.CuentaPadreID);
                    oSubcuenta.Subcuenta = this.txtCuenta.Text;
                    oSubcuenta.CuentaContpaq = this.txtCuentaContpaq.Text;
                    oSubcuenta.CuentaSat = this.txtCuentaSat.Text;
                    Guardar.Generico<ContaSubcuenta>(oSubcuenta);
                    break;
                case Tipo.CuentaDeMayor:
                    var oCuentaMay = (this.EsMod ? (this.oCuenta as ContaCuentaDeMayor) : (new ContaCuentaDeMayor()));
                    oCuentaMay.ContaSubcuentaID = (this.EsMod ? oCuentaMay.ContaSubcuentaID : this.CuentaPadreID);
                    oCuentaMay.CuentaDeMayor = this.txtCuenta.Text;
                    oCuentaMay.CuentaContpaq = this.txtCuentaContpaq.Text;
                    oCuentaMay.CuentaSat = this.txtCuentaSat.Text;
                    oCuentaMay.RestaInversa = this.chkRestaInversa.Checked;
                    Guardar.Generico<ContaCuentaDeMayor>(oCuentaMay);
                    break;
                case Tipo.CuentaAuxiliar:
                    this.GuardarCuentaAuxiliar();
                    break;
            }

            // Se muestra una notificación
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");
            Cargando.Cerrar();

            return true;
        }

        private bool GuardarCuentaAuxiliar()
        {
            var oCuentaAux = (this.EsMod ? (this.oCuenta as ContaCuentaAuxiliar) : (new ContaCuentaAuxiliar()));
            oCuentaAux.ContaCuentaDeMayorID = (this.EsMod ? oCuentaAux.ContaCuentaDeMayorID : this.CuentaPadreID);
            oCuentaAux.CuentaAuxiliar = this.txtCuenta.Text;
            oCuentaAux.CuentaContpaq = this.txtCuentaContpaq.Text;
            oCuentaAux.CuentaSat = this.txtCuentaSat.Text;
            oCuentaAux.Detallable = this.chkTieneDetalle.Checked;
            oCuentaAux.VisibleEnCaja = this.chkVisibleEnCaja.Checked;
            oCuentaAux.DevengarAut = this.chkDevengarAutomaticamente.Checked;
            oCuentaAux.DevengarAutEsp = this.chkDevengarEspecial.Checked;
            oCuentaAux.CalculoSemanal = this.chkCalculoSemanal.Checked;
            if (this.chkCalculoSemanal.Checked)
            {
                oCuentaAux.PeriodicidadMes = Helper.ConvertirEntero(this.txtMeses.Text);
                oCuentaAux.FinSemanalizar = (this.chkDejarDeSemanalizar.Checked ? (DateTime?)this.dtpDejarDeSemanalizar.Value : null);
            }

            oCuentaAux.AfectaMetas = this.chkAfectaMetas.Checked;
            oCuentaAux.SumaGastosFijos = this.chkSumaGastosFijos.Checked;

            Guardar.Generico<ContaCuentaAuxiliar>(oCuentaAux);

            // Se borran los registros de SucursalGastoFijo, si aplica
            if (!oCuentaAux.AfectaMetas.Valor() || !oCuentaAux.SumaGastosFijos.Valor())
            {
                var oGastosFijo = General.GetListOf<SucursalGastoFijo>(c => c.ContaCuentaAuxiliarID == oCuentaAux.ContaCuentaAuxiliarID);
                foreach (var oReg in oGastosFijo)
                    Guardar.Eliminar<SucursalGastoFijo>(oReg);
            }

            // Se llenan los datos de devengar automáticamente, si aplica y con el tipo que aplique
            if (this.chkDevengarAutomaticamente.Checked)
            {
                foreach (DataGridViewRow oFila in this.dgvDevSuc.Rows)
                {
                    int iSucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                    var oDevAut = General.GetEntity<ContaCuentaAuxiliarDevengadoAutomatico>(
                        c => c.ContaCuentaAuxiliarID == oCuentaAux.ContaCuentaAuxiliarID && c.SucursalID == iSucursalID);
                    if (oDevAut == null)
                    {
                        oDevAut = new ContaCuentaAuxiliarDevengadoAutomatico();
                        oDevAut.ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID;
                        oDevAut.SucursalID = iSucursalID;
                    }
                    oDevAut.Porcentaje = Helper.ConvertirDecimal(oFila.Cells["Porcentaje"].Value);
                    Guardar.Generico<ContaCuentaAuxiliarDevengadoAutomatico>(oDevAut);
                }
            }
            else if (this.chkDevengarEspecial.Checked)
            {
                var oDevEsp = General.GetEntity<ContaCuentaAuxiliarDevengadoEspecial>(c => c.ContaCuentaAuxiliarID == oCuentaAux.ContaCuentaAuxiliarID);
                if (oDevEsp == null)
                    oDevEsp = new ContaCuentaAuxiliarDevengadoEspecial() { ContaCuentaAuxiliarID = oCuentaAux.ContaCuentaAuxiliarID };
                oDevEsp.DuenioID = Helper.ConvertirEntero(this.cmbDevengarEspecial.SelectedValue);
                Guardar.Generico<ContaCuentaAuxiliarDevengadoEspecial>(oDevEsp);
            }
            //

            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (this.txtCuenta.Text == "")
                this.ctlError.PonerError(this.txtCuenta, "Debes especificar el nombre de la Cuenta.");

            // Validaciones para cuando es una cuenta auxiliar
            if (this.chkDevengarAutomaticamente.Checked)
            {
                decimal mPorTotal = 0;
                foreach (DataGridViewRow oFila in this.dgvDevSuc.Rows)
                    mPorTotal += Helper.ConvertirDecimal(oFila.Cells["Porcentaje"].Value);
                if (mPorTotal == 0)
                    this.ctlError.PonerError(this.chkDevengarAutomaticamente, "Debes especificar el Porcentaje a aplicar en cada Sucursal.");
                if (mPorTotal > 100)
                    this.ctlError.PonerError(this.chkDevengarAutomaticamente, "El Porcentaje total no puede ser mayor a cien.");
            }
            if (this.chkDevengarEspecial.Checked)
            {
                if (Helper.ConvertirEntero(this.cmbDevengarEspecial.SelectedValue) == 0)
                    this.ctlError.PonerError(this.cmbDevengarEspecial, "Debes seleccionar un devengado especial.");
            }
            if (this.chkCalculoSemanal.Checked)
            {
                if (Helper.ConvertirDecimal(this.txtMeses.Text) <= 0)
                    this.ctlError.PonerError(this.chkCalculoSemanal, "El número de días debe ser válido y mayor a cero.");
            }

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
                                       
    }
}
