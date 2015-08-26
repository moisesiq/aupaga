using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class VentasEgresos : ListadoSimpleBotones
    {
        // Para el Singleton
        private static VentasEgresos _Instance;
        public static VentasEgresos Instance
        {
            get
            {
                if (VentasEgresos._Instance == null || VentasEgresos._Instance.IsDisposed)
                    VentasEgresos._Instance = new VentasEgresos();
                return VentasEgresos._Instance;
            }
        }
        //

        public VentasEgresos()
        {
            InitializeComponent();
        }

        // Se cargan los datos por el método "ActualizarDatos", que es llamado desde el "Load" de la base.

        protected override void btnNuevo_Click(object sender, EventArgs e)
        {
            object oValor = UtilLocal.ObtenerValor("Indica el nombre del Gasto:", "", MensajeObtenerValor.Tipo.Texto);
            string sValor = Helper.ConvertirCadena(oValor);
            if (sValor != "")
            {
                var oEgreso = new CajaTipoEgreso() { NombreTipoEgreso = sValor, Seleccionable = true };
                Guardar.Generico<CajaTipoEgreso>(oEgreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sValorAnt = Helper.ConvertirCadena(this.dgvDatos.CurrentRow.Cells["NombreTipoEgreso"].Value);
            object oValor = UtilLocal.ObtenerValor("Indica el nombre del Gasto:", sValorAnt, MensajeObtenerValor.Tipo.Texto);
            string sValor = Helper.ConvertirCadena(oValor);
            if (sValor != "" && sValor != sValorAnt)
            {
                int iEgresoID = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["CajaTipoEgresoID"].Value);
                var oEgreso = General.GetEntity<CajaTipoEgreso>(q => q.CajaTipoEgresoID == iEgresoID && q.Estatus);
                oEgreso.NombreTipoEgreso = sValor;
                Guardar.Generico<CajaTipoEgreso>(oEgreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iId = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["CajaTipoEgresoID"].Value);
            var oObjeto = General.GetEntity<CajaTipoEgreso>(q => q.CajaTipoEgresoID == iId && q.Estatus);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "tipo de Gasto", oObjeto.NombreTipoEgreso);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Guardar.Eliminar<CajaTipoEgreso>(oObjeto, true);
            this.ActualizarDatos();
        }

        public override void ActualizarDatos()
        {
            var Datos = Helper.ListaEntityADataTable<CajaTipoEgreso>(General.GetListOf<CajaTipoEgreso>(q => q.Seleccionable && q.Estatus));
            base.CargarDatos(Datos);
            Helper.OcultarColumnas(this.dgvDatos, "CajaTipoEgresoID", "Seleccionable", "UsuarioID", "FechaRegistro", "FechaModificacion", "Estatus", "Actualizar");
            Helper.ColumnasToHeaderText(this.dgvDatos);
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
