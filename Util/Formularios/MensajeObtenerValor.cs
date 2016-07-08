using System;
using System.Windows.Forms;

namespace LibUtil
{
    public partial class MensajeObtenerValor : Form
    {
        const int Separacion = 6;
        const int AltoPorLinea = 13;
        const int PixelesPorLetra = 6;
        public enum Tipo { Texto, TextoLargo, Entero, Decimal, Fecha, Combo };
                
        Tipo TipoDeValor = Tipo.Texto;
        object ValorPredeterminado;
        NumericUpDown nudValor;
        DateTimePicker dtpValor;
        ComboBox cmbValor;

        public object Valor { get; set; }
        public TextBox CuadroDeTexto { get { return this.txtValor; } }
        public ComboBox Combo { get { return this.cmbValor; } }

        public CharacterCasing CapitalizacionDeTexto { get { return this.txtValor.CharacterCasing; } set { this.txtValor.CharacterCasing = value; } }

        public MensajeObtenerValor(string sMensaje, object oValorPredeterminado, Tipo TipoDeValor)
        {
            InitializeComponent();

            this.TipoDeValor = TipoDeValor;
            this.ValorPredeterminado = oValorPredeterminado;
            this.lblMensaje.Text = sMensaje;
            
            // Se ajusta el tamaño de la etiqueta de mensaje, según se requiera
            this.AjustarMensaje();

            // Se pone el control correspondiente
            switch (this.TipoDeValor)
            {
                case Tipo.Texto:
                case Tipo.TextoLargo:
                    this.txtValor.Text = Util.Cadena(oValorPredeterminado);
                    this.txtValor.SelectAll();
                    if (this.TipoDeValor == Tipo.TextoLargo)
                    {
                        int iAltoAnt = this.txtValor.Height;
                        this.txtValor.Multiline = true;
                        this.txtValor.Height = iAltoAnt;
                        this.Height += 60;
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                    }
                    break;
                case Tipo.Entero:
                    this.nudValor = new NumericUpDown();
                    this.nudValor.Maximum = 9999999;
                    this.nudValor.Value = Util.Entero(oValorPredeterminado);
                    this.AgregarControl(this.nudValor);
                    break;
                case Tipo.Decimal:
                    this.nudValor = new NumericUpDown();
                    this.nudValor.DecimalPlaces = 2;
                    this.nudValor.Maximum = 9999999;
                    this.nudValor.Value = Util.Decimal(oValorPredeterminado);
                    this.AgregarControl(this.nudValor);
                    break;
                case Tipo.Fecha:
                    this.dtpValor = new DateTimePicker();
                    this.dtpValor.Format = DateTimePickerFormat.Short;
                    this.dtpValor.Value = Util.FechaHora(oValorPredeterminado);
                    this.AgregarControl(this.dtpValor);
                    break;
                case Tipo.Combo:
                    this.cmbValor = new ComboBox() { AutoCompleteMode = AutoCompleteMode.SuggestAppend, AutoCompleteSource = AutoCompleteSource.ListItems };
                    this.AgregarControl(this.cmbValor);
                    break;
            }
        }

        public MensajeObtenerValor(string sMensaje, string sValorPredeterminado)
        {
            InitializeComponent();

            this.lblMensaje.Text = sMensaje;
            this.txtValor.Text = sValorPredeterminado;
            this.txtValor.SelectAll();

            // Se ajusta el tamaño de la etiqueta de mensaje, según se requiera
            this.AjustarMensaje();
        }

        public void CargarCombo(string sCampoValor, string sCampoTexto, object Datos)
        {
            if (this.cmbValor == null) return;
            this.cmbValor.CargarDatos(sCampoValor, sCampoTexto, Datos);
            this.cmbValor.SelectedValue = this.ValorPredeterminado;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            switch (this.TipoDeValor)
            {
                case Tipo.Entero:
                case Tipo.Decimal:
                    this.Valor = this.nudValor.Value.ToString();
                    break;
                case Tipo.Fecha:
                    this.Valor = this.dtpValor.Value.ToShortDateString();
                    break;
                case Tipo.Combo:
                    this.Valor = this.cmbValor.SelectedValue;
                    break;
                default:
                    this.Valor = this.txtValor.Text;
                    break;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AjustarMensaje()
        {
            string sMensaje = this.lblMensaje.Text;
            int iLetrasPorLinea = (int)Math.Ceiling((decimal)(this.lblMensaje.Width / MensajeObtenerValor.PixelesPorLetra));

            if (sMensaje.Length <= iLetrasPorLinea)
                return;

            int iLineas = 1;
            int iCuenta = 0;
            for (int iCont = 0; iCont < sMensaje.Length; iCont++)
            {
                if (++iCuenta > iLetrasPorLinea || sMensaje[iCont] == '\n')
                {
                    iLineas++;
                    iCuenta = 0;
                }
            }

            int iIncremento = (MensajeObtenerValor.AltoPorLinea * (iLineas - 1));
            this.Height += iIncremento;
            this.lblMensaje.Height = (MensajeObtenerValor.AltoPorLinea * iLineas);
            this.txtValor.Top += iIncremento;
        }

        private void AgregarControl(Control ControlValor)
        {
            this.txtValor.Hide();

            ControlValor.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            ControlValor.Location = this.txtValor.Location;
            ControlValor.Size = this.txtValor.Size;

            if (ControlValor is NumericUpDown)
                (ControlValor as NumericUpDown).Select(0, (ControlValor as NumericUpDown).Controls[1].Text.Length);

            this.Controls.Add(ControlValor);
            this.ActiveControl = ControlValor;
        }

    }

}
