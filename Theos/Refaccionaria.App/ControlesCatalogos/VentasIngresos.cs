using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class VentasIngresos : ListadoSimpleBotones
    {
        // Para el Singleton
        private static VentasIngresos _Instance;
        public static VentasIngresos Instance
        {
            get
            {
                if (VentasIngresos._Instance == null || VentasIngresos._Instance.IsDisposed)
                    VentasIngresos._Instance = new VentasIngresos();
                return VentasIngresos._Instance;
            }
        }
        //

        public VentasIngresos()
        {
            InitializeComponent();
        }

        // Se cargan los datos por el método "ActualizarDatos", que es llamado desde el "Load" de la base.

        protected override void btnNuevo_Click(object sender, EventArgs e)
        {
            object oValor = UtilLocal.ObtenerValor("Indica el nombre del Ingreso:", "", MensajeObtenerValor.Tipo.Texto);
            string sValor = Util.Cadena(oValor);
            if (sValor != "")
            {
                var oIngreso = new CajaTipoIngreso() { NombreTipoIngreso = sValor, Seleccionable = true };
                Datos.Guardar<CajaTipoIngreso>(oIngreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sValorAnt = Util.Cadena(this.dgvDatos.CurrentRow.Cells["NombreTipoIngreso"].Value);
            object oValor = UtilLocal.ObtenerValor("Indica el nombre del Ingreso:", sValorAnt, MensajeObtenerValor.Tipo.Texto);
            string sValor = Util.Cadena(oValor);
            if (sValor != "" && sValor != sValorAnt)
            {
                int iIngresoID = Util.Entero(this.dgvDatos.CurrentRow.Cells["CajaTipoIngresoID"].Value);
                var oIngreso = Datos.GetEntity<CajaTipoIngreso>(q => q.CajaTipoIngresoID == iIngresoID && q.Estatus);
                oIngreso.NombreTipoIngreso = sValor;
                Datos.Guardar<CajaTipoIngreso>(oIngreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iId = Util.Entero(this.dgvDatos.CurrentRow.Cells["CajaTipoIngresoID"].Value);
            var oObjeto = Datos.GetEntity<CajaTipoIngreso>(q => q.CajaTipoIngresoID == iId && q.Estatus);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "tipo de Ingreso", oObjeto.NombreTipoIngreso);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Datos.Eliminar<CajaTipoIngreso>(oObjeto, true);
            this.ActualizarDatos();
        }

        public override void ActualizarDatos()
        {
            var oDatos = Util.ListaEntityADataTable<CajaTipoIngreso>(Datos.GetListOf<CajaTipoIngreso>(q => q.Seleccionable && q.Estatus));
            base.CargarDatos(oDatos);
            Util.OcultarColumnas(this.dgvDatos, "CajaTipoIngresoID", "Seleccionable", "UsuarioID", "FechaRegistro", "FechaModificacion", "Estatus", "Actualizar");
            UtilLocal.ColumnasToHeaderText(this.dgvDatos);
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
