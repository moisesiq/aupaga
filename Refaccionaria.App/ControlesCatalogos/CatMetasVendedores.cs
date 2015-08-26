using System;
using System.Windows.Forms;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CatMetasVendedores : ListadoEditable
    {
        // Para el Singleton
        private static CatMetasVendedores _Instance;
        public static CatMetasVendedores Instance
        {
            get
            {
                if (CatMetasVendedores._Instance == null || CatMetasVendedores._Instance.IsDisposed)
                    CatMetasVendedores._Instance = new CatMetasVendedores();
                return CatMetasVendedores._Instance;
            }
        }
        //

        public CatMetasVendedores()
        {
            InitializeComponent();

            // Se agregan las columnas al grid
            var oColUsuario = this.AgregarColumnaCombo("UsuarioID", "Usuario", 120);
            oColUsuario.CargarDatos("UsuarioID", "NombreUsuario", General.GetListOf<Usuario>(c => c.Estatus));
            var oColSucursal = base.AgregarColumnaCombo("SucursalID", "Sucursal", 120);
            oColSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.AgregarColumnaCheck("EsGerente", "Es Gerente", 40);
            this.AgregarColumnaCheck("MetaConsiderar9500", "Considerar 9500", 80);
            this.AgregarColumnaImporte("SueldoFijo", "Sueldo Fijo");
            this.AgregarColumnaImporte("SueldoMeta", "Sueldo Meta");
            this.AgregarColumnaImporte("SueldoMinimo", "Sueldo Mínimo");
            this.AgregarColumnaImporte("IncrementoUtil", "Incremento Util.");
            this.AgregarColumnaImporte("IncrementoFijo", "Incremento Fijo");
            this.AgregarColumna("Porcentaje9500", "% 9500", 40).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        #region [ Eventos ]

        private void CatMetasVendedores_Load(object sender, EventArgs e)
        {
            // Los datos se mandan cargar desde el "Load" la base, "ListadoEditable"
        }
        
        #endregion

        #region [ Métodos ]

        protected override void CargarDatos()
        {
            Cargando.Mostrar();
            
            var oMetasVen = General.GetListOf<MetaVendedor>();
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oMetasVen)
            {
                this.dgvDatos.AgregarFila(oReg.MetaVendedorID, Cat.TiposDeAfectacion.SinCambios, oReg.VendedorID, oReg.SucursalID, oReg.EsGerente
                    , oReg.MetaConsiderar9500, oReg.SueldoFijo, oReg.SueldoMeta, oReg.SueldoMinimo, oReg.IncrementoUtil, oReg.IncrementoFijo, oReg.Porcentaje9500);
            }

            Cargando.Cerrar();
        }
                
        protected override bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            Cargando.Mostrar();

            MetaVendedor oReg;
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
                            oReg = new MetaVendedor();
                        else
                            oReg = General.GetEntity<MetaVendedor>(c => c.MetaVendedorID == iId);

                        oReg.VendedorID = Helper.ConvertirEntero(oFila.Cells["UsuarioID"].Value);
                        oReg.SucursalID = Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value);
                        oReg.EsGerente = Helper.ConvertirBool(oFila.Cells["EsGerente"].Value);
                        oReg.MetaConsiderar9500 = Helper.ConvertirBool(oFila.Cells["MetaConsiderar9500"].Value);
                        oReg.SueldoFijo = Helper.ConvertirDecimal(oFila.Cells["SueldoFijo"].Value);
                        oReg.SueldoMeta = Helper.ConvertirDecimal(oFila.Cells["SueldoMeta"].Value);
                        oReg.SueldoMinimo = Helper.ConvertirDecimal(oFila.Cells["SueldoMinimo"].Value);
                        oReg.IncrementoUtil = Helper.ConvertirDecimal(oFila.Cells["IncrementoUtil"].Value);
                        oReg.IncrementoFijo = Helper.ConvertirDecimal(oFila.Cells["IncrementoFijo"].Value);
                        oReg.Porcentaje9500 = Helper.ConvertirDecimal(oFila.Cells["Porcentaje9500"].Value);

                        Guardar.Generico<MetaVendedor>(oReg);
                        break;
                    case Cat.TiposDeAfectacion.Borrar:
                        oReg = General.GetEntity<MetaVendedor>(c => c.MetaVendedorID == iId);
                        Guardar.Eliminar<MetaVendedor>(oReg);
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
                if (Helper.ConvertirEntero(oFila.Cells["UsuarioID"].Value) < 1)
                {
                    oFila.ErrorText = "Usuario inválido.";
                    bError = true;
                }
                if (Helper.ConvertirEntero(oFila.Cells["SucursalID"].Value) < 1)
                {
                    oFila.ErrorText = "Sucursal inválida.";
                    bError = true;
                }
            }

            return (!bError);
        }

        #endregion
    }
}
