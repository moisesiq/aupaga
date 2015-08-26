using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

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
            string sValor = Helper.ConvertirCadena(oValor);
            if (sValor != "")
            {
                var oIngreso = new CajaTipoIngreso() { NombreTipoIngreso = sValor, Seleccionable = true };
                Guardar.Generico<CajaTipoIngreso>(oIngreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sValorAnt = Helper.ConvertirCadena(this.dgvDatos.CurrentRow.Cells["NombreTipoIngreso"].Value);
            object oValor = UtilLocal.ObtenerValor("Indica el nombre del Ingreso:", sValorAnt, MensajeObtenerValor.Tipo.Texto);
            string sValor = Helper.ConvertirCadena(oValor);
            if (sValor != "" && sValor != sValorAnt)
            {
                int iIngresoID = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["CajaTipoIngresoID"].Value);
                var oIngreso = General.GetEntity<CajaTipoIngreso>(q => q.CajaTipoIngresoID == iIngresoID && q.Estatus);
                oIngreso.NombreTipoIngreso = sValor;
                Guardar.Generico<CajaTipoIngreso>(oIngreso);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iId = Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["CajaTipoIngresoID"].Value);
            var oObjeto = General.GetEntity<CajaTipoIngreso>(q => q.CajaTipoIngresoID == iId && q.Estatus);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "tipo de Ingreso", oObjeto.NombreTipoIngreso);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Guardar.Eliminar<CajaTipoIngreso>(oObjeto, true);
            this.ActualizarDatos();
        }

        public override void ActualizarDatos()
        {
            var Datos = Helper.ListaEntityADataTable<CajaTipoIngreso>(General.GetListOf<CajaTipoIngreso>(q => q.Seleccionable && q.Estatus));
            base.CargarDatos(Datos);
            Helper.OcultarColumnas(this.dgvDatos, "CajaTipoIngresoID", "Seleccionable", "UsuarioID", "FechaRegistro", "FechaModificacion", "Estatus", "Actualizar");
            Helper.ColumnasToHeaderText(this.dgvDatos);
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
