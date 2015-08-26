﻿using System;
using System.Windows.Forms;
using System.Data.Objects;
using System.Linq;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CajaResguardo : CajaConteoPagos
    {
        ControlError ctlError = new ControlError();

        public CajaResguardo()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CajaResguardo_Load(object sender, EventArgs e)
        {
            // Se llenan los datos
            if (!this.DesignMode)
                this.ActualizarDatos();
        }

        #endregion

        #region [ Métodos ]

        /* Código para cálculos de importe restante e ideal
        protected override void MonedasCambio()
        {
            this.lblRestante.Text = (
                this.Efectivo
                - this.MonedasImporteTotal
                - Helper.ConvertirDecimal(this.lblImporteIdeal.Text.SoloNumeric())
            ).ToString(GlobalClass.FormatoDecimal);

            this.lblEfectivo.Text = (this.Efectivo - this.MonedasImporteTotal).ToString(GlobalClass.FormatoDecimal);
            this.CalcularTotal();
        }

        protected override void FilaMarcaCambiada(DataGridViewRow Fila)
        {
            bool bMarcado = Helper.ConvertirBool(this.dgvPagosBancarios.CurrentRow.Cells["Resguardar"].Value);
            decimal mImporte = Helper.ConvertirDecimal(this.dgvPagosBancarios.CurrentRow.Cells["Importe"].Value);
            switch (Helper.ConvertirEntero(this.dgvPagosBancarios.CurrentRow.Cells["FormaDePagoID"].Value))
            {
                case Cat.FormasDePago.Cheque:
                    this.lblCheques.Text = (this.Cheques - (mImporte * (bMarcado ? 1 : 0))).ToString(GlobalClass.FormatoDecimal);
                    break;
                case Cat.FormasDePago.Tarjeta:
                    this.lblTarjetas.Text = (this.Tarjetas - (mImporte * (bMarcado ? 1 : 0))).ToString(GlobalClass.FormatoDecimal);
                    break;
                case Cat.FormasDePago.Transferencia:
                    this.lblTransferencias.Text = (this.Transferencias - (mImporte * (bMarcado ? 1 : 0))).ToString(GlobalClass.FormatoDecimal);
                    break;
            }
            this.CalcularTotal();
        }

        private void CalcularTotal()
        {
            this.lblTotal.Text = (
                Helper.ConvertirDecimal(this.lblEfectivo.Text.SoloNumeric())
                + Helper.ConvertirDecimal(this.lblTransferencias.Text.SoloNumeric())
                + Helper.ConvertirDecimal(this.lblCheques.Text.SoloNumeric())
                + Helper.ConvertirDecimal(this.lblTarjetas.Text.SoloNumeric())
            ).ToString(GlobalClass.FormatoMoneda);
        }
        */

        #endregion

        #region [ Propiedades ]

        /*
        public decimal TotalMarcadao
        {
            get
            {
                decimal mTotal = this.MonedasImporteTotal;
                foreach (DataGridViewRow Fila in this.dgvPagosBancarios.Rows)
                {
                    if (Helper.ConvertirBool(Fila.Cells["Resguardar"].Value))
                        mTotal += Helper.ConvertirDecimal(Fila.Cells["Importe"].Value);
                }
                return mTotal;
            }
        }
        */

        #endregion

        #region [ Públicos ]

        public List<VentaPagoDetalleResguardo> GenerarPagosBancariosResguardados()
        {
            var oPagosB = new List<VentaPagoDetalleResguardo>();
            foreach (DataGridViewRow Fila in this.dgvPagosBancarios.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["Resguardar"].Value))
                {
                    oPagosB.Add(new VentaPagoDetalleResguardo()
                    {
                        VentaPagoDetalleID = Helper.ConvertirEntero(Fila.Cells["VentaPagoDetalleID"].Value),
                        Resguardado = true
                    });
                }
            }
            return oPagosB;
        }
                
        public override bool Validar()
        {
            this.ctlError.LimpiarErrores();

            bool bFilaMarcada = false;
            foreach (DataGridViewRow Fila in this.dgvPagosBancarios.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["Resguardar"].Value))
                {
                    bFilaMarcada = true;
                    break;
                }
            }
            if (this.MonedasImporteTotal == 0 && !bFilaMarcada)
                this.ctlError.PonerError(this.Controls["lblEtTotal"], "No se ha especificado ningún importe.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
    }
}
