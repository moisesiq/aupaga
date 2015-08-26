using System;
using System.Windows.Forms;
using System.Drawing;

namespace Refaccionaria.Negocio
{
    public partial class Progreso : UserControl
    {
        public const string Formato = "#,###,###";

        public Progreso()
        {
            InitializeComponent();
            this.RestaurarProgreso();
        }

        #region [ Propiedades ]

        private Color _ColorDeTexto = Color.Black;
        public Color ColorDeTexto
        {
            get { return this._ColorDeTexto; }
            set
            {
                this._ColorDeTexto = value;
                this.lblProgreso.ForeColor = value;
                this.lblTextoDe.ForeColor = value;
                this.lblTotal.ForeColor = value;
            }
        }

        #endregion

        #region [ Públicos ]

        public void RestaurarProgreso()
        {
            this.pgbProgreso.Value = 0;
            this.lblProgreso.Text = "0";
            this.lblTotal.Text = "0";
        }

        public void Inicializar(int iTotal, int iPaso)
        {
            this.RestaurarProgreso();
            this.pgbProgreso.Maximum = iTotal;
            this.pgbProgreso.Step = iPaso;
            this.lblTotal.Text = iTotal.ToString(Progreso.Formato);
        }

        public void EjecutarPaso(bool bEjecutarEventos)
        {
            // this.pgbProgreso.Value += this.pgbProgreso.Step;
            this.pgbProgreso.PerformStep();
            
            int iProgreso = this.pgbProgreso.Value;
            this.lblProgreso.Text = iProgreso.ToString(Progreso.Formato);

            if (bEjecutarEventos)
                Application.DoEvents();
        }

        public void Finalizar()
        {
            this.pgbProgreso.Value = this.pgbProgreso.Maximum;
            // this.lblProgreso.Text = 
        }

        #endregion
    }
}
