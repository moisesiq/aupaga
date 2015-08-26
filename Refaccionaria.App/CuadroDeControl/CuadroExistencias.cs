using System;
using System.Windows.Forms;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CuadroExistencias : UserControl
    {
        public CuadroExistencias()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroExistencias_Load(object sender, EventArgs e)
        {
            // Se cargan los datos de los combos
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", General.GetListOf<Sucursal>(c => c.Estatus));
            this.cmbProveedor.CargarDatos("ProveedorID", "NombreProveedor", General.GetListOf<Proveedor>(c => c.Estatus));
            this.cmbMarca.CargarDatos("MarcaParteID", "NombreMarcaParte", General.GetListOf<MarcaParte>(c => c.Estatus));
            this.cmbLinea.CargarDatos("LineaID", "NombreLinea", General.GetListOf<Linea>(c => c.Estatus));
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            this.CargarDatos();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            UtilLocal.AbrirEnExcel(this.dgvDatos);
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int iParteID = Helper.ConvertirEntero(this.dgvDatos["ParteID", e.RowIndex].Value);
            this.vmsExistencia.LlenarDatosExtra(iParteID, Helper.ConvertirEntero(this.cmbSucursal.SelectedValue));
        }


        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            // Se obtiene los datos
            int iSucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue);
            int iProveedorID = Helper.ConvertirEntero(this.cmbProveedor.SelectedValue);
            int iMarcaID = Helper.ConvertirEntero(this.cmbMarca.SelectedValue);
            int iLineaID = Helper.ConvertirEntero(this.cmbLinea.SelectedValue);
            bool bCostoDesc = this.chkCostoConDescuento.Checked;
            bool bSoloConExist = this.chkSoloConExistencia.Checked;
            var oDatos = General.GetListOf<PartesExistenciasView>(c => 
                (iSucursalID == 0 || c.SucursalID == iSucursalID)
                && (iProveedorID == 0 || c.ProveedorID == iProveedorID)
                && (iMarcaID == 0 || c.MarcaID == iMarcaID)
                && (iLineaID == 0 || c.LineaID == iLineaID)
                && (!bSoloConExist || c.Existencia > 0)
            ).OrderBy(c => c.NumeroDeParte).ToList();
            // Si no hay sucursal, se agrupan los datos
            if (iSucursalID == 0)
            {
                oDatos = oDatos.GroupBy(g => new { g.ParteID, g.NumeroDeParte, g.Descripcion, g.ProveedorID, g.Proveedor, g.MarcaID, g.Marca, g.LineaID, g.Linea
                    , g.Costo, g.CostoConDescuento}).Select(c => new PartesExistenciasView
                {
                    ParteID = c.Key.ParteID,
                    NumeroDeParte = c.Key.NumeroDeParte,
                    Descripcion = c.Key.Descripcion,
                    ProveedorID = c.Key.ProveedorID,
                    Proveedor = c.Key.Proveedor,
                    MarcaID = c.Key.MarcaID,
                    Marca = c.Key.Marca,
                    LineaID = c.Key.LineaID,
                    Linea = c.Key.Linea,
                    Costo = c.Key.Costo,
                    CostoConDescuento = c.Key.CostoConDescuento,
                    Existencia = c.Sum(s => s.Existencia),
                    UltimaVenta = c.Max(m => m.UltimaVenta)
                }).ToList();
            }

            // Se llena el grid
            decimal mExistenciaT = 0, mCostoT = 0;
            this.dgvDatos.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                this.dgvDatos.Rows.Add(oReg.ParteID, oReg.NumeroDeParte, oReg.Descripcion, oReg.Proveedor, oReg.Marca, oReg.Linea
                    , (bCostoDesc ? oReg.CostoConDescuento : oReg.Costo), oReg.Existencia, ((bCostoDesc ? oReg.CostoConDescuento : oReg.Costo) * oReg.Existencia)
                    , oReg.UltimaVenta);
                mExistenciaT += oReg.Existencia.Valor();
                mCostoT += ((bCostoDesc ? oReg.CostoConDescuento : oReg.Costo) * oReg.Existencia).Valor();
            }

            // Se agregan los totales
            this.lblExistenciaTotal.Text = mExistenciaT.ToString(GlobalClass.FormatoDecimal);
            this.lblCostoTotal.Text = mCostoT.ToString(GlobalClass.FormatoMoneda);

            Cargando.Cerrar();
        }

        #endregion

        
                                        
    }
}
