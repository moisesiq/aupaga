using System;
using System.Windows.Forms;

using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CuadroMultipleSucursales : UserControl
    {
        public CuadroMultipleSucursales()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            this.dgvPrincipal.FiltrarContiene(this.txtBusqueda.Text, "pri_Nombre");
        }

        private void dgvPrincipal_CurrentCellChanged(object sender, EventArgs e)
        {
            /*
            this.dgvSemanas.Rows.Clear();
            this.dgvMeses.Rows.Clear();
            if (!this.dgvPrincipal.Focused) return;

            bool bSelNueva = this.dgvPrincipal.VerSeleccionNueva();
            if (bSelNueva)
            {
                int iId = (this.dgvPrincipal.CurrentRow == null ? 0 : Helper.ConvertirEntero(this.dgvPrincipal.CurrentRow.Cells["pri_Id"].Value));
                this.LlenarSemanas(iId);
                this.LlenarMeses(iId);
            }

            this.PrincipalCambioSel(bSelNueva);
            */
        }

        #endregion

        #region [ Públicos ]

        public void FormatoColumnas(string sCalculo, int iDecimales)
        {
            string sFormato = (sCalculo == "Ventas" ? "N" : "C");
            sFormato += Helper.ConvertirCadena(iDecimales);

            this.dgvPrincipal.Columns["pri_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvPrincipal.Columns["pri_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvPrincipal.Columns["pri_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvMeses.Columns["mes_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvMeses.Columns["mes_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvMeses.Columns["mes_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanas.Columns["sem_Suc01_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanas.Columns["sem_Suc02_Actual"].DefaultCellStyle.Format = sFormato;
            this.dgvSemanas.Columns["sem_Suc03_Actual"].DefaultCellStyle.Format = sFormato;
        }

        #endregion

        #region [ Virtual ]

        protected virtual void LlenarSemanas(int iId) { }

        protected virtual void LlenarMeses(int iId) { }

        protected virtual void PrincipalCambioSel(bool bSelNueva) { }

        #endregion

    }
}
