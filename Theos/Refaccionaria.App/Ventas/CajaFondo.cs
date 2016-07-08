using System;
using System.Windows.Forms;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CajaFondo : CajaMonedas
    {
        ControlError ctlAdvertencia = new ControlError() { Icon = Properties.Resources._16_Ico_Advertencia };

        public CajaFondo()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        private decimal _Conteo;
        public decimal Conteo
        {
            get { return this._Conteo; }
            set
            {
                this._Conteo = value;
                this.lblConteo.Text = value.ToString(GlobalClass.FormatoDecimal);
                this.CalcularDiferencia();
            }
        }

        private decimal _FondoDeCaja;
        public decimal FondoDeCaja
        {
            get { return this._FondoDeCaja; }
            set
            {
                this._FondoDeCaja = value;
                this.lblFondoDeCaja.Text = value.ToString(GlobalClass.FormatoDecimal);
                this.CalcularDiferencia();
            }
        }

        public decimal Diferencia
        {
            get { return (this._Conteo - this._FondoDeCaja); }
        }

        private bool _FondoRealizado;
        public bool FondoRealizado
        {
            get { return this._FondoRealizado; }
            set { this._FondoRealizado = value; }
        }

        #endregion

        private void CajaFondo_Load(object sender, System.EventArgs e)
        {
            // Se llena el importe del día anterior
            DateTime dHoy = DateTime.Today;
            CajaEfectivoPorDia oEfectivo;
            var oEfectivos = Datos.GetListOf<CajaEfectivoPorDia>(q => q.SucursalID == GlobalClass.SucursalID && q.Dia < dHoy && q.Estatus);
            if (oEfectivos.Count > 0)
            {
                int iEfectivoID = oEfectivos.Max(q => q.CajaEfectivoPorDiaID);
                oEfectivo = Datos.GetEntity<CajaEfectivoPorDia>(q => q.CajaEfectivoPorDiaID == iEfectivoID);
                this.FondoDeCaja = (oEfectivo == null ? 0 : oEfectivo.Cierre.Valor());
            }

            // Se llenan los datos
            this.ActualizarDatos();
        }

        protected override void MonedasCambio()
        {
            this.Conteo = this.MonedasImporteTotal;
        }

        private void CalcularDiferencia()
        {
            this.lblDiferencia.Text = this.Diferencia.ToString(GlobalClass.FormatoDecimal);

            if (this.Diferencia == 0 || this.FondoRealizado)
                this.ctlAdvertencia.QuitarError(this.lblEtDiferencia);
            else
                this.ctlAdvertencia.PonerError(this.lblEtDiferencia, "Se requerirá una autorización mientras haya una diferencia.", ErrorIconAlignment.MiddleLeft);
        }

        public void ActualizarDatos()
        {
            // Se limpian las monedas usadas
            this.LimpiarMonedas();
            // Se verifica si ya se realizó el fondo el día de hoy
            DateTime dHoy = DateTime.Today;
            var oEfectivo = Datos.GetEntity<CajaEfectivoPorDia>(q => q.SucursalID == GlobalClass.SucursalID && q.Dia == dHoy && q.Estatus);
            if (oEfectivo != null)
            {
                this.Conteo = oEfectivo.Inicio;
                this.pnlMonedas.Enabled = false;
                this.FondoRealizado = true;
                this.ctlAdvertencia.LimpiarErrores();
            }
        }
    }
}
