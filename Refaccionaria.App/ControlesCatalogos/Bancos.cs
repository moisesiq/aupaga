using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

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
            if (Helper.ConvertirCadena(Banco) != "")
            {
                var oBanco = new Banco() { NombreBanco = Helper.ConvertirCadena(Banco) };
                Guardar.Generico<Banco>(oBanco);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sBancoAnt = Helper.ConvertirCadena(this.dgvDatos.CurrentRow.Cells["NombreBanco"].Value);
            object Banco = UtilLocal.ObtenerValor("Indica el nombre del banco:", sBancoAnt, MensajeObtenerValor.Tipo.Texto);
            string sBanco = Helper.ConvertirCadena(Banco);
            if (sBanco != "" && sBanco != sBancoAnt)
            {
                int iBancoID = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["BancoID"].Value);
                var oBanco = General.GetEntity<Banco>(q => q.BancoID == iBancoID && q.Estatus);
                oBanco.NombreBanco = sBanco;
                Guardar.Generico<Banco>(oBanco);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iBancoID = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["BancoID"].Value);
            var oBanco = General.GetEntity<Banco>(q => q.BancoID == iBancoID && q.Estatus);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "Banco", oBanco.NombreBanco);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Guardar.Eliminar<Banco>(oBanco, true);
            this.ActualizarDatos();
        }

        #endregion

        public override void ActualizarDatos()
        {
            var Datos = Helper.ListaEntityADataTable<Banco>(General.GetListOf<Banco>(q => q.Estatus));
            base.CargarDatos(Datos);
            Helper.OcultarColumnas(this.dgvDatos, "BancoID", "UsuarioID", "FechaRegistro", "FechaModificacion", "Estatus", "Actualizar");
            Helper.ColumnasToHeaderText(this.dgvDatos);
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
