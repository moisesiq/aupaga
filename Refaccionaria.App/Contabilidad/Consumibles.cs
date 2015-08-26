using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Consumibles : ListadoSimpleBotones
    {
        public Consumibles()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        // Se cargan los datos por el método "ActualizarDatos", que es llamado desde el "Load" de la base.

        protected override void btnNuevo_Click(object sender, EventArgs e)
        {
            object oNombre = UtilLocal.ObtenerValor("Indica el nombre del Consumible:", "", MensajeObtenerValor.Tipo.Texto);
            if (Helper.ConvertirCadena(oNombre) != "")
            {
                var oRegistro = new ContaConsumible() { Consumible = Helper.ConvertirCadena(oNombre) };
                Guardar.Generico<ContaConsumible>(oRegistro);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sNombreAnt = Helper.ConvertirCadena(this.dgvDatos.CurrentRow.Cells["Consumible"].Value);
            object oNombre = UtilLocal.ObtenerValor("Indica el nombre del Consumible:", sNombreAnt, MensajeObtenerValor.Tipo.Texto);
            string sNombre = Helper.ConvertirCadena(oNombre);
            if (sNombre != "" && sNombre != sNombreAnt)
            {
                int iRegistroID = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ContaConsumibleID"].Value);
                var oRegistro = General.GetEntity<ContaConsumible>(c => c.ContaConsumibleID == iRegistroID);
                oRegistro.Consumible = sNombre;
                Guardar.Generico<ContaConsumible>(oRegistro);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iRegistroID = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ContaConsumibleID"].Value);
            var oRegistro = General.GetEntity<ContaConsumible>(c => c.ContaConsumibleID == iRegistroID);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "Consumible", oRegistro.Consumible);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Guardar.Eliminar<ContaConsumible>(oRegistro);
            this.ActualizarDatos();
        }

        #endregion

        public override void ActualizarDatos()
        {
            var Datos = Helper.ListaEntityADataTable<ContaConsumible>(General.GetListOf<ContaConsumible>());
            base.CargarDatos(Datos);
            Helper.OcultarColumnas(this.dgvDatos, "ContaConsumibleID");
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
