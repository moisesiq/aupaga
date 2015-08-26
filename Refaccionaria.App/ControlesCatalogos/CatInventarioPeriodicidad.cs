using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CatInventarioPeriodicidad : ListadoEditable
    {
        // Para el Singleton
        private static readonly CatInventarioPeriodicidad instance = new CatInventarioPeriodicidad();
        public static CatInventarioPeriodicidad Instance
        {
            get { return CatInventarioPeriodicidad.instance; }
        }
        //

        public CatInventarioPeriodicidad()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            var oColLinea = this.AgregarColumnaCombo("LineaID", "Línea", 120);
            oColLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(c => c.Estatus));
            this.AgregarColumna("Periodicidad", "Periodicidad", 80);
        }

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = General.GetListOf<InventarioLineaPeriodicidad>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.AgregarFila(oReg.InventarioLineaPeriodicidadID, Cat.TiposDeAfectacion.SinCambios, oReg.LineaID, oReg.Periodicidad);
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            InventarioLineaPeriodicidad oReg;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;

                int iId = this.dgvDatos.ObtenerId(oFila); // Helper.ConvertirEntero(oFila.Cells["__Id"].Value);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila); // Helper.ConvertirEntero(oFila.Cells["__Cambio"].Value);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new InventarioLineaPeriodicidad();
                        else
                            oReg = General.GetEntity<InventarioLineaPeriodicidad>(c => c.InventarioLineaPeriodicidadID == iId);

                        oReg.LineaID = Helper.ConvertirEntero(oFila.Cells["LineaID"].Value);
                        oReg.Periodicidad = Helper.ConvertirEntero(oFila.Cells["Periodicidad"].Value);

                        Guardar.Generico<InventarioLineaPeriodicidad>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<InventarioLineaPeriodicidad>(c => c.InventarioLineaPeriodicidadID == iId);
                        Guardar.Eliminar<InventarioLineaPeriodicidad>(oReg);
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

            bool bError = false;
            foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
            {
                if (oFila.IsNewRow) continue;
                if (Helper.ConvertirEntero(oFila.Cells["LineaID"].Value) < 1)
                {
                    oFila.ErrorText = "Línea inválida.";
                    bError = true;
                }
            }

            return (!bError);
        }

        #endregion

    }
}
