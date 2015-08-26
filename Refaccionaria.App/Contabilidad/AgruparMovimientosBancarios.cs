using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class AgruparMovimientosBancarios : Form
    {
        public AgruparMovimientosBancarios()
        {
            InitializeComponent();

            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
        }

        #region [ Propiedades ]

        public bool Deposito
        {
            get { return this.rdbDeposito.Checked; }
            set
            {
                this.rdbDeposito.Checked = value;
                this.rdbRetiro.Checked = !value;
            }
        }

        public decimal Importe
        {
            get { return Helper.ConvertirDecimal(this.lblImporte.Text.SoloNumeric()); }
            set { this.lblImporte.Text = value.ToString(GlobalClass.FormatoDecimal); }
        }

        public DateTime Fecha
        {
            get { return this.dtpFechaDeAsignacion.Value; }
            set { this.dtpFechaDeAsignacion.Value = value; }
        }

        public int SucursalID
        {
            get { return Helper.ConvertirEntero(this.cmbSucursal.SelectedValue); }
            set { this.cmbSucursal.SelectedValue = value; }
        }

        public string Concepto
        {
            get { return this.txtConcepto.Text; }
            set { this.txtConcepto.Text = value; }
        }

        public string Referencia
        {
            get { return this.txtReferencia.Text; }
            set { this.txtReferencia.Text = value; }
        }
        
        #endregion

        #region [ Eventos ]

        private void AgruparMovimientosBancarios_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
