using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Calculadora : DetalleBase
    {
        Parte Parte;
        PartePrecio PartePrecio;
        public PreciosCalculadora PreciosCalculadora;
        public bool EsDescuento = true;

        public static Calculadora Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly Calculadora instance = new Calculadora();
        }

        public Calculadora()
        {
            InitializeComponent();
        }

        public Calculadora(int Id)
        {
            InitializeComponent();
            try
            {
                Parte = Negocio.General.GetEntityById<Parte>("Parte", "ParteID", Id);
                if (Parte == null)
                    throw new EntityNotFoundException(Id.ToString(), "Parte");
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #region [ eventos ]

        private void Calculadora_Load(object sender, EventArgs e)
        {
            this.ClearTextBoxes(this);
            this.cboOperacion.SelectedIndex = 0;
            if (Parte != null)
            {
                var precio = Negocio.General.GetEntity<PartePrecio>(p => p.ParteID.Equals(Parte.ParteID));
                if (precio != null)
                {
                    PartePrecio = precio;
                    this.txtCosto.Text = precio.Costo.ToString();
                    this.txtGananciaUno.Text = precio.PorcentajeUtilidadUno.ToString();
                    this.txtGananciaDos.Text = precio.PorcentajeUtilidadDos.ToString();
                    this.txtGananciaTres.Text = precio.PorcentajeUtilidadTres.ToString();
                    this.txtGananciaCuatro.Text = precio.PorcentajeUtilidadCuatro.ToString();
                    this.txtGananciaCinco.Text = precio.PorcentajeUtilidadCinco.ToString();
                    this.txtPrecioUno.Text = precio.PrecioUno.ToString();
                    this.txtPrecioDos.Text = precio.PrecioDos.ToString();
                    this.txtPrecioTres.Text = precio.PrecioTres.ToString();
                    this.txtPrecioCuatro.Text = precio.PrecioCuatro.ToString();
                    this.txtPrecioCinco.Text = precio.PrecioCinco.ToString();
                }
            }
        }

        private void Calculadora_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
            }
        }

        private void UltimoProceso(bool fueModificado)
        {
            catalogosPartes pa = catalogosPartes.Instance;
            var p = new PreciosCalculadora()
            {
                FueModificado = fueModificado,
                Costo = Negocio.Helper.ConvertirDecimal(txtCosto.Text),
                DescuentoUno = Negocio.Helper.ConvertirDecimal(txtDescuentoUno.Text),
                DescuentoDos = Negocio.Helper.ConvertirDecimal(txtDescuentoDos.Text),
                DescuentoTres = Negocio.Helper.ConvertirDecimal(txtDescuentoTres.Text),
                DescuentoCuatro = Negocio.Helper.ConvertirDecimal(txtDescuentoCuatro.Text),
                DescuentoCinco = Negocio.Helper.ConvertirDecimal(txtDescuentoCinco.Text),
                GananciaUno = Negocio.Helper.ConvertirDecimal(txtGananciaUno.Text),
                GananciaDos = Negocio.Helper.ConvertirDecimal(txtGananciaDos.Text),
                GananciaTres = Negocio.Helper.ConvertirDecimal(txtGananciaTres.Text),
                GananciaCuatro = Negocio.Helper.ConvertirDecimal(txtGananciaCuatro.Text),
                GananciaCinco = Negocio.Helper.ConvertirDecimal(txtGananciaCinco.Text),
                PrecioUno = Negocio.Helper.ConvertirDecimal(txtPrecioUno.Text),
                PrecioDos = Negocio.Helper.ConvertirDecimal(txtPrecioDos.Text),
                PrecioTres = Negocio.Helper.ConvertirDecimal(txtPrecioTres.Text),
                PrecioCuatro = Negocio.Helper.ConvertirDecimal(txtPrecioCuatro.Text),
                PrecioCinco = Negocio.Helper.ConvertirDecimal(txtPrecioCinco.Text)
            };
            this.PreciosCalculadora = p;
        }

        protected override void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                UltimoProceso(true);
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
            this.Close();
        }

        protected override void btnCerrar_Click(object sender, EventArgs e)
        {
            UltimoProceso(false);
            this.Close();
        }

        private void cboOperacion_SelectedValueChanged(object sender, EventArgs e)
        {
            this.txtGananciaUno.Enabled = cboOperacion.SelectedIndex == 0 ? false : true;
            this.txtGananciaDos.Enabled = cboOperacion.SelectedIndex == 0 ? false : true;
            this.txtGananciaTres.Enabled = cboOperacion.SelectedIndex == 0 ? false : true;
            this.txtGananciaCuatro.Enabled = cboOperacion.SelectedIndex == 0 ? false : true;
            this.txtGananciaCinco.Enabled = cboOperacion.SelectedIndex == 0 ? false : true;
            if (this.cboOperacion.SelectedIndex == 0)
            {
                EsDescuento = true;
            }
            else
            {
                EsDescuento = false;
            }
        }

        private void txtDescuentoUno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (PartePrecio == null) return;
                if (EsDescuento)
                {
                    this.txtPrecioUno.Text = String.Format("{0:0.00}", operacionDescuento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoUno.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioUno)));
                }
                else
                {
                    this.txtPrecioUno.Text = String.Format("{0:0.00}", operacionIncremento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoUno.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioUno)));
                }
            }
        }

        private void txtDescuentoDos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (PartePrecio == null) return;
                if (EsDescuento)
                {
                    this.txtPrecioDos.Text = String.Format("{0:0.00}", operacionDescuento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoDos.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioDos)));
                }
                else
                {
                    this.txtPrecioDos.Text = String.Format("{0:0.00}", operacionIncremento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoDos.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioDos)));
                }
            }
        }

        private void txtDescuentoTres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (PartePrecio == null) return;
                if (EsDescuento)
                {
                    this.txtPrecioTres.Text = String.Format("{0:0.00}", operacionDescuento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoTres.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioTres)));
                }
                else
                {
                    this.txtPrecioTres.Text = String.Format("{0:0.00}", operacionIncremento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoTres.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioTres)));
                }
            }
        }

        private void txtDescuentoCuatro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (PartePrecio == null) return;
                if (EsDescuento)
                {
                    this.txtPrecioCuatro.Text = String.Format("{0:0.00}", operacionDescuento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoCuatro.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioCuatro)));
                }
                else
                {
                    this.txtPrecioCuatro.Text = String.Format("{0:0.00}", operacionIncremento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoCuatro.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioCuatro)));
                }
            }
        }

        private void txtDescuentoCinco_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (PartePrecio == null) return;
                if (EsDescuento)
                {
                    this.txtPrecioCinco.Text = String.Format("{0:0.00}", operacionDescuento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoCinco.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioCinco)));
                }
                else
                {
                    this.txtPrecioCinco.Text = String.Format("{0:0.00}", operacionIncremento(Negocio.Helper.ConvertirDecimal(this.txtDescuentoCinco.Text), Negocio.Helper.ConvertirDecimal(PartePrecio.PrecioCinco)));
                }
            }
        }

        private void txtGananciaUno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }

        private void txtGananciaDos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }

        private void txtGananciaTres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }

        private void txtGananciaCuatro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }

        private void txtGananciaCinco_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }

        #endregion

        #region [ metodos]

        private void chkUsar_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsar.Checked.Equals(true))
                cboListaPrecios.Enabled = true;
            else
                cboListaPrecios.Enabled = false;
        }

        private void ClearTextBoxes(System.Windows.Forms.Form form)
        {
            txtCosto.Text = "0.0";
            txtDescuentoUno.Text = "0.0";
            txtDescuentoDos.Text = "0.0";
            txtDescuentoTres.Text = "0.0";
            txtDescuentoCuatro.Text = "0.0";
            txtDescuentoCinco.Text = "0.0";
            txtGananciaUno.Text = "0.0";
            txtGananciaDos.Text = "0.0";
            txtGananciaTres.Text = "0.0";
            txtGananciaCuatro.Text = "0.0";
            txtGananciaCinco.Text = "0.0";
        }
        
        private decimal operacionDescuento(decimal valor, decimal precio)
        {
            var porcentaje = precio * (valor / 100);
            var resultado = precio - porcentaje;            
            return resultado;
        }

        private decimal operacionIncremento(decimal valor, decimal precio)
        {
            var porcentaje = precio * (valor / 100);
            var resultado = precio + porcentaje;
            return resultado;
        }

        #endregion
        
    }
}
