using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CobroTarjeta : Form
    {
        ControlError ctlError = new ControlError();

        public CobroTarjeta()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public int BancoCuentaID { get; set; }
        public int Meses { get; set; }
        public string Celular
        {
            get { return this.txtCelular.Text; }
            set { this.txtCelular.Text = value; }
        }

        #endregion

        #region [ Eventos ]

        private void CobroTarjeta_Load(object sender, EventArgs e)
        {
            // Se carga el combo de bancos
            this.cmbCuenta.CargarDatos("BancoCuentaID", "NombreDeCuenta", Datos.GetListOf<BancoCuenta>(
                c => c.BancoCuentaID == Cat.CuentasBancarias.Banamex || c.BancoCuentaID == Cat.CuentasBancarias.Serfin));
        }

        private void rdbUnaSolaExhibicion_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbUnaSolaExhibicion.Focused)
                this.VerHabilitarControles();
        }

        private void rdbMeses_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdbMeses.Focused)
                this.VerHabilitarControles();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!this.Validar())
                return;

            this.BancoCuentaID = Util.Entero(this.cmbCuenta.SelectedValue);
            if (this.rdbMeses.Checked)
                this.Meses = (int)this.nudMeses.Value;
            // this.Celular = this.txtCelular.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void VerHabilitarControles()
        {
            this.nudMeses.Enabled = this.rdbMeses.Checked;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (this.cmbCuenta.SelectedValue == null)
                this.ctlError.PonerError(this.cmbCuenta, "Debes especificar una cuenta bancaria.");

            return this.ctlError.Valido;
        }

        #endregion

    }
}
