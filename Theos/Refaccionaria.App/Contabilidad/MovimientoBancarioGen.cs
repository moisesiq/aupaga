﻿using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class MovimientoBancarioGen : Form
    {
        public ControlError ctlError = new ControlError();
        public ControlError ctlAdv = new ControlError() { Icon = Properties.Resources._16_Ico_Advertencia };

        public delegate bool DelBool();
        public DelBool delValidar;

        public MovimientoBancarioGen()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public int OrigenBancoCuentaID { get; set; }

        public int BancoCuentaID
        {
            get { return Util.Entero(this.cmbBancoCuenta.SelectedValue); }
        }

        public decimal Importe
        {
            get { return Util.Decimal(this.txtImporte.Text); }
        }

        #endregion

        #region [ Eventos ]

        private void btnAceptar_Click(object sender, EventArgs e)
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

        public void LlenarComboCuenta()
        {
            this.cmbBancoCuenta.CargarDatos("BancoCuentaID", "NombreDeCuenta", Datos.GetListOf<BancoCuenta>());
        }

        protected virtual bool AccionGuardar()
        {
            if (this.delValidar != null)
            {
                if (!this.delValidar())
                    return false;
            }

            return true;
        }

        #endregion

    }
}
