using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

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
            oColLinea.CargarDatos("LineaID", "NombreLinea", Datos.GetListOf<Linea>(c => c.Estatus));
            this.AgregarColumna("Periodicidad", "Periodicidad", 80);
        }

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = Datos.GetListOf<InventarioLineaPeriodicidad>();
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

                int iId = this.dgvDatos.ObtenerId(oFila); // Util.ConvertirEntero(oFila.Cells["__Id"].Value);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila); // Util.ConvertirEntero(oFila.Cells["__Cambio"].Value);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new InventarioLineaPeriodicidad();
                        else
                            oReg = Datos.GetEntity<InventarioLineaPeriodicidad>(c => c.InventarioLineaPeriodicidadID == iId);

                        oReg.LineaID = Util.Entero(oFila.Cells["LineaID"].Value);
                        oReg.Periodicidad = Util.Entero(oFila.Cells["Periodicidad"].Value);

                        Datos.Guardar<InventarioLineaPeriodicidad>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = Datos.GetEntity<InventarioLineaPeriodicidad>(c => c.InventarioLineaPeriodicidadID == iId);
                        Datos.Eliminar<InventarioLineaPeriodicidad>(oReg);
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
                if (Util.Entero(oFila.Cells["LineaID"].Value) < 1)
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
