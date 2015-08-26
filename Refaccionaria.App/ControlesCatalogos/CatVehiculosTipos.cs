using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CatVehiculosTipos : ListadoEditable
    {
        // Para el Singleton
        private static CatVehiculosTipos _Instance;
        public static CatVehiculosTipos Instance
        {
            get
            {
                if (CatVehiculosTipos._Instance == null || CatVehiculosTipos._Instance.IsDisposed)
                    CatVehiculosTipos._Instance = new CatVehiculosTipos();
                return CatVehiculosTipos._Instance;
            }
        }
        //

        public CatVehiculosTipos()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            this.AgregarColumna("VehiculoTipoID", "Id", 40).ReadOnly = true;
            this.AgregarColumna("Tipo", "Tipo", 120);
        }

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = General.GetListOf<VehiculoTipo>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.Rows.Add(oReg.VehiculoTipoID, Cat.TiposDeAfectacion.SinCambios, oReg.VehiculoTipoID, oReg.Tipo);
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            // Se valida
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            VehiculoTipo oReg;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iId = this.dgvDatos.ObtenerId(oFila);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new VehiculoTipo();
                        else
                            oReg = General.GetEntity<VehiculoTipo>(c => c.VehiculoTipoID == iId);

                        // Se llenan los datos
                        oReg.Tipo = Helper.ConvertirCadena(oFila.Cells["Tipo"].Value);
                        //
                        Guardar.Generico<VehiculoTipo>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<VehiculoTipo>(c => c.VehiculoTipoID == iId);
                        Guardar.Eliminar<VehiculoTipo>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            this.CargarDatos();
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();

            // Se valida que no se borren tipos que se estén usando
            bool bErrorGrid = false;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;
                if (this.dgvDatos.ObtenerIdCambio(oFila) == Cat.TiposDeAfectacion.Borrar)
                {
                    if (General.Exists<ClienteFlotilla>(c => c.VehiculoTipoID == this.dgvDatos.ObtenerId(oFila)))
                        oFila.ErrorText = "Este tipo está siendo usado por uno o más vehículos. No se puede borrar.";
                }
                bErrorGrid = (bErrorGrid || (oFila.ErrorText != ""));
            }
            if (bErrorGrid)
                this.ctlError.PonerError(this.btnGuardar, "Existen algunos errores de validación.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }

        #endregion
    }
}
