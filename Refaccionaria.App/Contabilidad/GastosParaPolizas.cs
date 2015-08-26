using System;
using System.Windows.Forms;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class GastosParaPolizas : UserControl
    {
        public GastosParaPolizas()
        {
            InitializeComponent();
        }

        private void GastosParaPolizas_Load(object sender, EventArgs e)
        {
            this.LlenarGastos();
        }

        private void dgvGastos_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvGastos.CurrentRow != null)
                this.CrearPoliza(this.dgvGastos.CurrentRow);
        }

        private void CrearPoliza(DataGridViewRow oFilaGasto)
        {
            decimal mImporte = Helper.ConvertirDecimal(oFilaGasto.Cells["Importe"].Value);
            var frmGastoPol = new GastoCajaAPoliza(mImporte);
            if (frmGastoPol.ShowDialog(Principal.Instance) == DialogResult.OK)
            {
                // Se llenan los datos del gasto
                int iEgresoID = Helper.ConvertirEntero(oFilaGasto.Cells["CajaEgresoID"].Value);
                var oGasto = General.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iEgresoID && c.Estatus);
                oGasto.Facturado = frmGastoPol.Facturado;
                oGasto.FolioFactura = frmGastoPol.Folio;
                oGasto.FechaFactura = frmGastoPol.Fecha;
                oGasto.Subtotal = frmGastoPol.Subtotal;
                oGasto.Iva = frmGastoPol.Iva;
                Guardar.Generico<CajaEgreso>(oGasto);

                // Se manda crear la Póliza, si no es de Proveedores
                var oContaEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == oGasto.ContaEgresoID);
                if (oContaEgreso == null || !General.Exists<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oContaEgreso.ContaCuentaAuxiliarID
                    && c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Proveedores))
                {
                    var oUsuario = General.GetEntity<Usuario>(c => c.UsuarioID == oGasto.RealizoUsuarioID && c.Estatus);

                    // Caso especial para reparto de utilidades
                    if (General.Exists<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == oContaEgreso.ContaCuentaAuxiliarID
                        && c.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.ReparteDeUtilidades))
                    {
                        ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GastoReparteUtilidades, oGasto.CajaEgresoID, oUsuario.NombreUsuario, oGasto.Concepto
                            , oGasto.SucursalID, oGasto.Fecha);
                    }
                    else
                    {
                        if (oGasto.Facturado.Valor())
                        {
                            // Se manda a afectar contabilidad (AfeConta)
                            ContaProc.CrearPolizaAfectacion(Cat.ContaAfectaciones.GastoCajaFacturado, iEgresoID, oGasto.FolioFactura, oGasto.Concepto
                                , oGasto.SucursalID, oGasto.Fecha);
                        }
                        else
                        {
                            // Se crea una póliza simple
                            var oPoliza = new ContaPoliza()
                            {
                                Fecha = oGasto.Fecha,
                                ContaTipoPolizaID = Cat.ContaTiposDePoliza.Egreso,
                                Concepto = oGasto.Concepto,
                                SucursalID = oGasto.SucursalID,
                                RelacionTabla = Cat.Tablas.CajaEgreso,
                                RelacionID = iEgresoID
                            };
                            ContaProc.CrearPoliza(oPoliza, Cat.ContaCuentasAuxiliares.GastosNoDeducibles, Cat.ContaCuentasAuxiliares.Caja, oGasto.Importe
                                , oUsuario.NombreUsuario);
                        }
                    }
                }

                // Se marca el gasto como afectado, y se guardan los datos
                oGasto.AfectadoEnPolizas = true;
                Guardar.Generico<CajaEgreso>(oGasto);

                this.LlenarGastos();
            }
            frmGastoPol.Dispose();
        }

        #region [ Públicos ]

        public void LlenarGastos()
        {
            Cargando.Mostrar();

            var oGastos = General.GetListOf<CajaEgresosView>(c => (!c.AfectadoEnPolizas.HasValue || !c.AfectadoEnPolizas.Value) 
                && c.CajaTipoEgresoID != Cat.CajaTiposDeEgreso.Resguardo);
            // Se quitan todos los datos con fecha menor al 01 de Junio, petición especial
            oGastos = oGastos.Where(c => c.Fecha >= new DateTime(2015, 6, 1)).ToList();
            
            this.dgvGastos.Rows.Clear();
            foreach (var oReg in oGastos)
            {
                this.dgvGastos.Rows.Add(oReg.CajaEgresoID, oReg.ContaCuentaAuxiliarID, oReg.SucursalID, oReg.Fecha, oReg.Sucursal, oReg.CuentaAuxiliar
                    , oReg.Importe, oReg.Concepto);
            }

            Cargando.Cerrar();
        }

        #endregion
    }
}
