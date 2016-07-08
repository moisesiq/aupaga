using System;
using System.Windows.Forms;

using LibUtil;

namespace Refaccionaria.App
{
    public partial class MasterCostos : Form
    {        
        public MasterCostos()
        {
            InitializeComponent();
            this.Width = 254;
            this.gpbPorcentajes.Left = this.gpbPrecios.Left;
        }

        #region [ Propiedades ]

        public bool ModoPrecios
        {
            get { return this.gpbPrecios.Visible; }
            set
            {
                this.gpbPrecios.Visible = value;
                this.gpbPorcentajes.Visible = !value;
            }
        }

        public bool MostrarActualizarPrecios
        {
            get { return this.chkActualizarPrecios.Visible; }
            set { this.chkActualizarPrecios.Visible = value; }
        }

        public int TipoDePrecio { get; private set; }
        public decimal Importe { get; private set; }
        public decimal Porcentaje { get; private set; }
        public bool ActualizarPrecios { get { return this.chkActualizarPrecios.Checked; } }
        public decimal[] Porcentajes { get; private set; }

        #endregion

        #region [ Eventos ]

        private void MasterCostos_Load(object sender, EventArgs e)
        {
            /* if (this.ModoPrecios)
                this.ActiveControl = this.txtValorFijo;
            else
                this.ActiveControl = this.txtPorcentaje1;
            */
        }

        private void rdbValorFijo_CheckedChanged(object sender, EventArgs e)
        {
            this.txtValorFijo.Enabled = this.rdbValorFijo.Checked;
        }

        private void rdbIncremento_CheckedChanged(object sender, EventArgs e)
        {
            this.txtIncrementoPor.Enabled = this.rdbIncremento.Checked;
            this.txtIncrementoFijo.Enabled = this.rdbIncremento.Checked;
        }

        private void rbdDescuento_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDescuentoPor.Enabled = this.rbdDescuento.Checked;
            this.txtDescuentoFijo.Enabled = this.rbdDescuento.Checked;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.ModoPrecios)
            {
                if (this.rdbValorFijo.Checked)
                {
                    this.TipoDePrecio = 1;
                    this.Importe = Util.Decimal(this.txtValorFijo.Text);
                }
                else if (this.rdbIncremento.Checked)
                {
                    this.TipoDePrecio = (this.txtIncrementoPor.Text == "" ? 3 : 2);
                    this.Importe = Util.Decimal(this.txtIncrementoFijo.Text);
                    this.Porcentaje = Util.Decimal(this.txtIncrementoPor.Text);
                }
                else
                {
                    this.TipoDePrecio = (this.txtDescuentoPor.Text == "" ? 3 : 2);
                    this.Importe = (Util.Decimal(this.txtDescuentoFijo.Text) * -1);
                    this.Porcentaje = (Util.Decimal(this.txtDescuentoPor.Text) * -1);
                }
            }
            else
            {
                this.Porcentajes = new decimal[] {
                    Util.Decimal(this.txtPorcentaje1.Text),
                    Util.Decimal(this.txtPorcentaje2.Text),
                    Util.Decimal(this.txtPorcentaje3.Text),
                    Util.Decimal(this.txtPorcentaje4.Text),
                    Util.Decimal(this.txtPorcentaje5.Text)
                };
            }

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
