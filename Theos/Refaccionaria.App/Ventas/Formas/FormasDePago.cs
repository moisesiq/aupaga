using System;
using System.Windows.Forms;
using System.Collections.Generic;

using LibUtil;
using TheosProc;

namespace Refaccionaria.App
{
    public partial class FormasDePago : Form
    {
        private class FormaDePago
        {
            public int Id { get; set; }
            public string Forma { get; set; }
            public override string ToString()
            {
                return this.Forma;
            }
        }

        public FormasDePago()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public List<VentasPagosDetalleView> FormasDePagoSel { get; set; }

        #endregion

        #region [ Eventos ]

        private void FormasDePago_Load(object sender, EventArgs e)
        {
            // Se llenan las formas de pago
            this.LlenarListas();
        }

        private void lsbFormasDePago_DoubleClick(object sender, EventArgs e)
        {
            if (this.lsbFormasDePago.Focused)
                this.PasarElemento(this.lsbFormasDePago, this.lsbSeleccionadas, this.lsbFormasDePago.SelectedItem);
        }

        private void lsbSeleccionadas_DoubleClick(object sender, EventArgs e)
        {
            if (this.lsbSeleccionadas.Focused)
                this.PasarElemento(this.lsbSeleccionadas, this.lsbFormasDePago, this.lsbSeleccionadas.SelectedItem);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.LlenarFormasDePago();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #region [ Privados ]

        private void PasarElemento(ListBox oListaDe, ListBox oListaA, object oElemento)
        {
            if (oElemento != null)
            {
                oListaA.Items.Add(oElemento);
                oListaDe.Items.Remove(oElemento);
            }
        }

        private bool ExisteElemento(ListBox oLista, object oElemento)
        {
            var oForma = (oElemento as FormaDePago);
            int iFormaID = oForma.Id;
            foreach (object oReg in oLista.Items)
            {
                var oFormaReg = (oReg as FormaDePago);
                if (oFormaReg.Id == iFormaID)
                    return true;
            }
            return false;
        }

        private void LlenarFormasDePago()
        {
            if (this.FormasDePagoSel == null)
                this.FormasDePagoSel = new List<VentasPagosDetalleView>();
            this.FormasDePagoSel.Clear();
            foreach (object oReg in this.lsbSeleccionadas.Items)
            {
                var oFormaReg = (oReg as FormaDePago);
                this.FormasDePagoSel.Add(new VentasPagosDetalleView() { FormaDePagoID = oFormaReg.Id, FormaDePago = oFormaReg.Forma });
            }
        }

        #endregion

        #region [ Públicos ]

        public void LlenarListas()
        {
            var oFormas = Datos.GetListOf<TipoFormaPago>(c => c.Estatus);
            this.lsbFormasDePago.Items.Clear();
            this.lsbSeleccionadas.Items.Clear();
            foreach (var oReg in oFormas)
                this.lsbFormasDePago.Items.Add(new FormaDePago { Id = oReg.TipoFormaPagoID, Forma = oReg.NombreTipoFormaPago });

            if (this.FormasDePagoSel == null)
                return;

            foreach (var oReg in this.FormasDePagoSel)
            {
                for (int i = 0; i < this.lsbFormasDePago.Items.Count; i++)
                {
                    var oForma = this.lsbFormasDePago.Items[i];
                    var oFormaReg = (oForma as FormaDePago);
                    if (oFormaReg.Id == oReg.FormaDePagoID)
                    {
                        this.lsbSeleccionadas.Items.Add(oForma);
                        this.lsbFormasDePago.Items.RemoveAt(i--);
                    }
                }
            }
        }

        #endregion
    }
}
