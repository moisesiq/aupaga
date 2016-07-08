using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class Bancos : ListadoSimpleBotones
    {
        // Para el Singleton
        private static Bancos _Instance;
        public static Bancos Instance
        {
            get {
                if (Bancos._Instance == null || Bancos._Instance.IsDisposed)
                    Bancos._Instance = new Bancos();
                return Bancos._Instance;
            }
        }
        //

        public Bancos()
        {
            InitializeComponent();
        }

        private void Bancos_Load(object sender, EventArgs e)
        {
            // Se cargan los datos por el método "ActualizarDatos", que es llamado desde el "Load" de la base.
        }

        #region [ Eventos ]

        protected override void btnNuevo_Click(object sender, EventArgs e)
        {
            object Banco = UtilLocal.ObtenerValor("Indica el nombre del banco:", "", MensajeObtenerValor.Tipo.Texto);
            if (Util.Cadena(Banco) != "")
            {
                var oBanco = new Banco() { NombreBanco = Util.Cadena(Banco) };
                Datos.Guardar<Banco>(oBanco);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sBancoAnt = Util.Cadena(this.dgvDatos.CurrentRow.Cells["NombreBanco"].Value);
            object Banco = UtilLocal.ObtenerValor("Indica el nombre del banco:", sBancoAnt, MensajeObtenerValor.Tipo.Texto);
            string sBanco = Util.Cadena(Banco);
            if (sBanco != "" && sBanco != sBancoAnt)
            {
                int iBancoID = Util.Entero(this.dgvDatos.CurrentRow.Cells["BancoID"].Value);
                var oBanco = Datos.GetEntity<Banco>(q => q.BancoID == iBancoID && q.Estatus);
                oBanco.NombreBanco = sBanco;
                Datos.Guardar<Banco>(oBanco);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iBancoID = Util.Entero(this.dgvDatos.CurrentRow.Cells["BancoID"].Value);
            var oBanco = Datos.GetEntity<Banco>(q => q.BancoID == iBancoID && q.Estatus);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "Banco", oBanco.NombreBanco);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Datos.Eliminar<Banco>(oBanco, true);
            this.ActualizarDatos();
        }

        #endregion

        public override void ActualizarDatos()
        {
            var oDatos = Util.ListaEntityADataTable<Banco>(Datos.GetListOf<Banco>(q => q.Estatus));
            base.CargarDatos(oDatos);
            Util.OcultarColumnas(this.dgvDatos, "BancoID", "UsuarioID", "FechaRegistro", "FechaModificacion", "Estatus", "Actualizar");
            UtilLocal.ColumnasToHeaderText(this.dgvDatos);
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
