using System;
using System.Windows.Forms;
using System.Drawing;

namespace Refaccionaria.Negocio
{
    public partial class Progreso : UserControl
    {
        public const string Formato = "#,###,###";
        public enum PosTexto { Derecha, Izquierda, NoMostrar };

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
                this.pnlTextoAv.ForeColor = value;
            }
        }

        private Progreso.PosTexto _PosicionTexto = PosTexto.Derecha;
        public Progreso.PosTexto PosicionTexto
        {
            get { return this._PosicionTexto; }
            set {
                this._PosicionTexto = value;
                switch (value)
                {
                    case PosTexto.Derecha:
                        this.pgbProgreso.Left = 0;
                        this.pnlTextoAv.Left = (this.pgbProgreso.Left + this.pgbProgreso.Width + 6);
                        this.pnlTextoAv.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right);
                        this.pnlTextoAv.Visible = true;
                        break;
                    case PosTexto.Izquierda:
                        this.pnlTextoAv.Left = 0;
                        this.pgbProgreso.Left = (this.pnlTextoAv.Left + this.pnlTextoAv.Width + 6);
                        this.pnlTextoAv.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
                        this.pnlTextoAv.Visible = true;
                        break;
                    case PosTexto.NoMostrar:
                        this.pgbProgreso.Left = 0;
                        this.pgbProgreso.Width = this.Width;
                        this.pnlTextoAv.Visible = false;
                        break;
                }
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
