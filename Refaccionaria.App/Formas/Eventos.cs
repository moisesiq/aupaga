using System;
using System.Windows.Forms;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Eventos : Form
    {
        // Para el Singleton *
        private static Eventos instance;
        public static Eventos Instance
        {
            get
            {
                if (Eventos.instance == null || Eventos.instance.IsDisposed)
                    Eventos.instance = new Eventos();
                return Eventos.instance;
            }
        }
        //

        public Eventos()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria;
            this.Height = (Principal.Instance.Height - 16 - 2);
            this.Top = (Principal.Instance.Top + 8);
            this.Left = (Principal.Instance.Width - this.Width - 16);
        }

        #region [ Eventos ]

        private void Eventos_Load(object sender, EventArgs e)
        {
            this.flpEventos.Controls.Remove(this.pnlMuestra);
            this.pnlMuestra.Dispose();
        }

        private void btnBien_Click(object sender, EventArgs e)
        {
            var pnl = ((sender as Button).Parent as Panel);
            int iId = Helper.ConvertirEntero(pnl.Tag);
            if (iId > 0)
            {
                this.MarcarEventoRevisado(iId);
                this.flpEventos.Controls.Remove((sender as Button).Parent);
            }
        }

        #endregion

        #region [ Privados ]

        private void MarcarEventoRevisado(int iEventoID)
        {
            var oEvento = General.GetEntity<ClienteEventoCalendario>(c => c.ClienteEventoCalendarioID == iEventoID);
            oEvento.Revisado = true;
            Guardar.Generico<ClienteEventoCalendario>(oEvento);
        }

        #endregion

        #region [ Públicos ]

        public void AgregarEvento(int iId, DateTime dFecha, string sTitulo, string sEvento)
        {
            var pnlEvento = new Panel() { Width = (this.flpEventos.Width - 24), Height = 64, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle
                , Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right)};
            pnlEvento.Tag = iId;
            this.flpEventos.Controls.Add(pnlEvento);

            var lblFecha = new Label() { Text = dFecha.ToString(), AutoSize = false, Width = 140, Height = 20, TextAlign = ContentAlignment.MiddleLeft };
            var lblTitulo = new Label() { Text = sTitulo, AutoSize = false, Width = 260, Height = 20, TextAlign = ContentAlignment.MiddleLeft, Left = 146 };
            var lblEvento = new Label() { Text = sEvento, AutoSize = false, Width = (pnlEvento.Width - 38), Height = 40, Top = 20
                , Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom), Font = new Font(this.Font, FontStyle.Bold) };
            var btnBien = new Button() { Text = "", BackgroundImage = Properties.Resources.ok, BackgroundImageLayout = ImageLayout.Stretch, Width = 24, Height = 36
                , Left = (pnlEvento.Width - 27), Top = 26 };
            btnBien.Click += btnBien_Click;

            pnlEvento.Controls.Add(lblFecha);
            pnlEvento.Controls.Add(lblTitulo);
            pnlEvento.Controls.Add(lblEvento);
            pnlEvento.Controls.Add(btnBien);
        }
        
        #endregion
    }
}
