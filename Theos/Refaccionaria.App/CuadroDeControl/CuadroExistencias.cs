using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

using TheosProc;
using LibUtil;

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
            CuadroControlPermisos PermisosC = new CuadroControlPermisos();
            // Se cargan los datos de los combos
            //this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", Datos.GetListOf<Sucursal>(c => c.Estatus));
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", PermisosC.ValidarPermisosTiendaCuadroMultiple(CuadroControlPermisos.GetTabPage));
            this.cmbProveedor.CargarDatos("ProveedorID", "NombreProveedor", Datos.GetListOf<Proveedor>(c => c.Estatus));
            this.cmbMarca.CargarDatos("MarcaParteID", "NombreMarcaParte", Datos.GetListOf<MarcaParte>(c => c.Estatus));
            this.cmbLinea.CargarDatos("LineaID", "NombreLinea", Datos.GetListOf<Linea>(c => c.Estatus));
            this.cmbAgrupar.Items.AddRange(new object[] { "Proveedor", "Marca", "Línea", "Marca-Línea" });
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
            int iParteID = Util.Entero(this.dgvDatos["ParteID", e.RowIndex].Value);
            this.vmsExistencia.LlenarDatosExtra(iParteID, Util.Entero(this.cmbSucursal.SelectedValue));
        }


        #endregion

        #region [ Métodos ]

        private void CargarDatos()
        {
            Cargando.Mostrar();

            // Se obtiene los datos
            int iSucursalID = Util.Entero(this.cmbSucursal.SelectedValue);
            int iProveedorID = Util.Entero(this.cmbProveedor.SelectedValue);
            int iMarcaID = Util.Entero(this.cmbMarca.SelectedValue);
            int iLineaID = Util.Entero(this.cmbLinea.SelectedValue);
            bool bCostoDesc = this.chkCostoConDescuento.Checked;
            bool bSoloConExist = this.chkSoloConExistencia.Checked;
            var oDatos = Datos.GetListOf<PartesExistenciasView>(c => 
                (iSucursalID == 0 || c.SucursalID == iSucursalID)
                && (iProveedorID == 0 || c.ProveedorID == iProveedorID)
                && (iMarcaID == 0 || c.MarcaID == iMarcaID)
                && (iLineaID == 0 || c.LineaID == iLineaID)
                && (!bSoloConExist || c.Existencia > 0)
            ).OrderBy(c => c.NumeroDeParte).ToList();
            List<PartesExistenciasView> oDatosOr = null;
            // Si no hay sucursal, se agrupan los datos
            if (iSucursalID == 0)
            {
                // Se guarda una copia de los datos originales, para obtener el folio posteriormente, sólo si aplica
                if (this.chkFolioFactura.Checked)
                    oDatosOr = oDatos;
                //
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
                    UltimaVenta = c.Max(m => m.UltimaVenta),
                    UltimaCompra = c.Max(m => m.UltimaCompra)
                }).ToList();
            }

            // Se verifica si se deben agrupar los datos, y se agrupan
            switch (this.cmbAgrupar.Text)
            {
                case "Proveedor":
                    oDatos = oDatos.GroupBy(g => new { g.ProveedorID, g.Proveedor }).Select(c => new PartesExistenciasView
                    {
                        Proveedor = c.Key.Proveedor, 
                        Marca = "",
                        Linea = "",
                        Costo = c.Sum(s => s.Costo * s.Existencia),
                        CostoConDescuento = c.Sum(s => s.CostoConDescuento * s.Existencia),
                        Existencia = c.Sum(s => s.Existencia)
                    }).ToList();
                    break;
                case "Marca":
                    oDatos = oDatos.GroupBy(g => new { g.MarcaID, g.Marca }).Select(c => new PartesExistenciasView
                    {
                        Proveedor = "",
                        Marca = c.Key.Marca,
                        Linea = "",
                        Costo = c.Sum(s => s.Costo * s.Existencia),
                        CostoConDescuento = c.Sum(s => s.CostoConDescuento * s.Existencia),
                        Existencia = c.Sum(s => s.Existencia)
                    }).ToList();
                    break;
                case "Línea":
                    oDatos = oDatos.GroupBy(g => new { g.LineaID, g.Linea }).Select(c => new PartesExistenciasView 
                    {
                        Proveedor = "",
                        Marca = "",
                        Linea = c.Key.Linea,
                        Costo = c.Sum(s => s.Costo * s.Existencia),
                        CostoConDescuento = c.Sum(s => s.CostoConDescuento * s.Existencia),
                        Existencia = c.Sum(s => s.Existencia)
                    }).ToList();
                    break;
                case "Marca-Línea":
                    oDatos = oDatos.GroupBy(g => new { g.MarcaID, g.Marca, g.LineaID, g.Linea }).Select(c => new PartesExistenciasView
                    {
                        Proveedor = "",
                        Marca = c.Key.Marca,
                        Linea = c.Key.Linea,
                        Costo = c.Sum(s => s.Costo * s.Existencia),
                        CostoConDescuento = c.Sum(s => s.CostoConDescuento * s.Existencia),
                        Existencia = c.Sum(s => s.Existencia)
                    }).ToList();
                    break;
            }

            // Se llena el grid
            decimal mExistenciaT = 0, mCostoT = 0;
            this.dgvDatos.Rows.Clear();
            if (this.cmbAgrupar.Text == "")
            {
                foreach (var oReg in oDatos)
                {
                    // Si se agruparon los datos, por no seleccionar sucursal, se llena la última copra
                    if (iSucursalID == 0 && this.chkFolioFactura.Checked)
                    {
                        var oRegMax = oDatosOr.Where(c => c.ParteID == oReg.ParteID).OrderByDescending(c => c.UltimaCompra).FirstOrDefault();
                        // oReg.UltimaCompra = oRegMax.UltimaCompra;
                        oReg.FolioFactura = oRegMax.FolioFactura;
                    }
                    //
                    this.dgvDatos.Rows.Add(oReg.ParteID, oReg.NumeroDeParte, oReg.Descripcion, oReg.Proveedor, oReg.Marca, oReg.Linea
                        , (bCostoDesc ? oReg.CostoConDescuento : oReg.Costo), oReg.Existencia, ((bCostoDesc ? oReg.CostoConDescuento : oReg.Costo) * oReg.Existencia)
                        , oReg.UltimaVenta, oReg.UltimaCompra, oReg.FolioFactura);
                    mExistenciaT += oReg.Existencia.Valor();
                    mCostoT += ((bCostoDesc ? oReg.CostoConDescuento : oReg.Costo) * oReg.Existencia).Valor();
                }
            }
            else
            {
                foreach (var oReg in oDatos)
                {
                    this.dgvDatos.Rows.Add(null, null, null, oReg.Proveedor, oReg.Marca, oReg.Linea, null, oReg.Existencia, (bCostoDesc ? oReg.CostoConDescuento : oReg.Costo));
                    mExistenciaT += oReg.Existencia.Valor();
                    mCostoT += (bCostoDesc ? oReg.CostoConDescuento : oReg.Costo).Valor();
                }
            }
            
            // Se agregan los totales
            this.lblExistenciaTotal.Text = mExistenciaT.ToString(GlobalClass.FormatoDecimal);
            this.lblCostoTotal.Text = mCostoT.ToString(GlobalClass.FormatoMoneda);

            Cargando.Cerrar();
        }

        #endregion

    }
}
