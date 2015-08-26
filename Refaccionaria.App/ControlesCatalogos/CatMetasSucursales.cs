using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

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
            oColSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
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

            var oDatos = General.GetListOf<MetaSucursal>();
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

                int iId = this.dgvDatos.ObtenerId(oFila); // Helper.ConvertirEntero(oFila.Cells["__Id"].Value);
                int iCambio = this.dgvDatos.ObtenerIdCambio(oFila); // Helper.ConvertirEntero(oFila.Cells["__Cambio"].Value);
                switch (iCambio)
                {
                    case Cat.TiposDeAfectacion.Agregar:
                    case Cat.TiposDeAfectacion.Modificar:
                        if (iCambio == Cat.TiposDeAfectacion.Agregar)
                            oReg = new MetaSucursal();
                        else
                            oReg = General.GetEntity<MetaSucursal>(c => c.MetaSucursalID == iId);

                        // Se llenan los datos
                        oReg.SucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                        oReg.UtilSucursal = Helper.ConvertirDecimal(oFila.Cells["UtilSucursal"].Value);
                        oReg.UtilSucursalMinimo = Helper.ConvertirDecimal(oFila.Cells["UtilSucursalMinimo"].Value);
                        oReg.UtilSucursalLargoPlazo = Helper.ConvertirDecimal(oFila.Cells["UtilSucursalLargoPlazo"].Value);
                        oReg.UtilGerente = Helper.ConvertirDecimal(oFila.Cells["UtilGerente"].Value);
                        oReg.UtilVendedor = Helper.ConvertirDecimal(oFila.Cells["UtilVendedor"].Value);
                        oReg.DiasPorSemana = Helper.ConvertirEntero(oFila.Cells["DiasPorSemana"].Value);

                        Guardar.Generico<MetaSucursal>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<MetaSucursal>(c => c.MetaSucursalID == iId);
                        Guardar.Eliminar<MetaSucursal>(oReg);
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
