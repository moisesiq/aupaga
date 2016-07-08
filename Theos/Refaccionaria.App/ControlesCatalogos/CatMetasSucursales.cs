using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CatMetasSucursales : ListadoEditable
    {
        // Para el Singleton
        private static CatMetasSucursales _Instance;
        public static CatMetasSucursales Instance
        {
            get
            {
                if (CatMetasSucursales._Instance == null || CatMetasSucursales._Instance.IsDisposed)
                    CatMetasSucursales._Instance = new CatMetasSucursales();
                return CatMetasSucursales._Instance;
            }
        }
        //

        public CatMetasSucursales()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            var oColSucursal = base.AgregarColumnaCombo("SucursalID", "Sucursal", 120);
            oColSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
            base.AgregarColumnaImporte("UtilSucursal", "Util. Sucursal C. P.");
            base.AgregarColumnaImporte("UtilSucursalLargoPlazo", "Util. Sucursal L. P.");
            base.AgregarColumnaImporte("UtilSucursalMinimo", "Util. Sucursal Mínimo");
            base.AgregarColumnaImporte("UtilGerente", "Util. Gerente");
            base.AgregarColumnaImporte("UtilVendedor", "Util. Vendedor");
            base.AgregarColumna("DiasPorSemana", "Días por Sem.", 60).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        #region [ Eventos ]



        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();

            var oDatos = Datos.GetListOf<MetaSucursal>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.AgregarFila(oReg.MetaSucursalID, Cat.TiposDeAfectacion.SinCambios, oReg.SucursalID, oReg.UtilSucursal
                    , oReg.UtilSucursalLargoPlazo, oReg.UtilSucursalMinimo, oReg.UtilGerente, oReg.UtilVendedor, oReg.DiasPorSemana);
            }

            Cargando.Cerrar();
        }

        protected override bool AccionGuardar()
        {
            // Se valida
            /* if (!this.Validar())
                return false;
            */

            Cargando.Mostrar();

            MetaSucursal oReg;
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
                            oReg = new MetaSucursal();
                        else
                            oReg = Datos.GetEntity<MetaSucursal>(c => c.MetaSucursalID == iId);

                        // Se llenan los datos
                        oReg.SucursalID = Util.Entero(oFila.Cells["SucursalID"].Value);
                        oReg.UtilSucursal = Util.Decimal(oFila.Cells["UtilSucursal"].Value);
                        oReg.UtilSucursalMinimo = Util.Decimal(oFila.Cells["UtilSucursalMinimo"].Value);
                        oReg.UtilSucursalLargoPlazo = Util.Decimal(oFila.Cells["UtilSucursalLargoPlazo"].Value);
                        oReg.UtilGerente = Util.Decimal(oFila.Cells["UtilGerente"].Value);
                        oReg.UtilVendedor = Util.Decimal(oFila.Cells["UtilVendedor"].Value);
                        oReg.DiasPorSemana = Util.Entero(oFila.Cells["DiasPorSemana"].Value);

                        Datos.Guardar<MetaSucursal>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = Datos.GetEntity<MetaSucursal>(c => c.MetaSucursalID == iId);
                        Datos.Eliminar<MetaSucursal>(oReg);
                        break;
                }
            }

            Cargando.Cerrar();
            this.CargarDatos();
            return true;
        }

        #endregion
    }
}
