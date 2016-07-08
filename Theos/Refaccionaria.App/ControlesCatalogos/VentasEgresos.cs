using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

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
            string sValor = Util.Cadena(oValor);
            if (sValor != "")
            {
                var oEgreso = new CajaTipoEgreso() { NombreTipoEgreso = sValor, Seleccionable = true };
                Datos.Guardar<CajaTipoEgreso>(oEgreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sValorAnt = Util.Cadena(this.dgvDatos.CurrentRow.Cells["NombreTipoEgreso"].Value);
            object oValor = UtilLocal.ObtenerValor("Indica el nombre del Gasto:", sValorAnt, MensajeObtenerValor.Tipo.Texto);
            string sValor = Util.Cadena(oValor);
            if (sValor != "" && sValor != sValorAnt)
            {
                int iEgresoID = Util.Entero(this.dgvDatos.CurrentRow.Cells["CajaTipoEgresoID"].Value);
                var oEgreso = Datos.GetEntity<CajaTipoEgreso>(q => q.CajaTipoEgresoID == iEgresoID && q.Estatus);
                oEgreso.NombreTipoEgreso = sValor;
                Datos.Guardar<CajaTipoEgreso>(oEgreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iId = Util.Entero(this.dgvDatos.CurrentRow.Cells["CajaTipoEgresoID"].Value);
            var oObjeto = Datos.GetEntity<CajaTipoEgreso>(q => q.CajaTipoEgresoID == iId && q.Estatus);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "tipo de Gasto", oObjeto.NombreTipoEgreso);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Datos.Eliminar<CajaTipoEgreso>(oObjeto, true);
            this.ActualizarDatos();
        }

        public override void ActualizarDatos()
        {
            var oDatos = Util.ListaEntityADataTable<CajaTipoEgreso>(Datos.GetListOf<CajaTipoEgreso>(q => q.Seleccionable && q.Estatus));
            base.CargarDatos(oDatos);
            Util.OcultarColumnas(this.dgvDatos, "CajaTipoEgresoID", "Seleccionable", "UsuarioID", "FechaRegistro", "FechaModificacion", "Estatus", "Actualizar");
            UtilLocal.ColumnasToHeaderText(this.dgvDatos);
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
