using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class BusquedaReimpresion : BusquedaVenta
    {
        public VentasReimpresion oReimpresion;

        public BusquedaReimpresion()
        {
            InitializeComponent();
        }

        protected override void MostrarDatosDeVenta(int iVentaID)
        {
            base.MostrarDatosDeVenta(iVentaID);

            // Para mostrar el detalle de la venta
            // this.oReimpresion.ctlDetalle.LlenarDetalle(iVentaID);
            
            // Se limpia el grid de detalle
            this.oReimpresion.ctlDetalle.LimpiarDetalle();

            // Se agrega el detalle de la venta
            var oDetalle = General.GetListOf<VentasDetalleView>(q => q.VentaID == iVentaID);
            foreach (var oProducto in oDetalle)
            {
                this.oReimpresion.ctlDetalle.AgregarProducto(new ProductoVenta()
                {
                    ParteID = oProducto.ParteID,
                    NumeroDeParte = oProducto.NumeroParte,
                    NombreDeParte = oProducto.NombreParte,
                    Cantidad = oProducto.Cantidad,
                    PrecioUnitario = oProducto.PrecioUnitario,
                    Iva = oProducto.Iva
                });
            }
            // Se agregan las devoluciones, si hubiera
            var oDevoluciones = General.GetListOf<VentasDevolucionesView>(c => c.VentaID == iVentaID);
            foreach (var oDev in oDevoluciones)
            {
                var oDevDet = General.GetListOf<VentasDevolucionesDetalleView>(q => q.VentaDevolucionID == oDev.VentaDevolucionID);
                foreach (var oProducto in oDevDet)
                {
                    this.oReimpresion.ctlDetalle.AgregarProducto(new ProductoVenta()
                    {
                        ParteID = oProducto.ParteID,
                        NumeroDeParte = oProducto.NumeroParte,
                        NombreDeParte = oProducto.NombreParte,
                        Cantidad = oProducto.Cantidad,
                        PrecioUnitario = oProducto.PrecioUnitario,
                        Iva = oProducto.Iva
                    });
                }
            }
            // Se agregan las garantías, si hubiera
            var oGarantias = General.GetListOf<VentasGarantiasView>(c => c.VentaID == iVentaID);
            foreach (var oGarantia in oGarantias)
            {
                this.oReimpresion.ctlDetalle.AgregarProducto(new ProductoVenta()
                {
                    ParteID = oGarantia.ParteID,
                    NumeroDeParte = oGarantia.NumeroDeParte,
                    NombreDeParte = oGarantia.NombreDeParte,
                    Cantidad = 1,
                    PrecioUnitario = oGarantia.PrecioUnitario,
                    Iva = oGarantia.Iva
                });
            }

            // Se marcan las devoluciones y garantías en rojo
            for (int iFila = oDetalle.Count; iFila < this.oReimpresion.ctlDetalle.dgvProductos.Rows.Count; iFila++)
            {
                this.oReimpresion.ctlDetalle.dgvProductos.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
                this.oReimpresion.ctlDetalle.dgvProductos.Rows[iFila].DefaultCellStyle.SelectionForeColor = Color.Red;
            }

            // Se llenan las observaciones, si aplica
            this.txtObservaciones.Clear();
            // Devoluciones
            foreach (var oDev in oDevoluciones)
            {
                this.txtObservaciones.Text += (
                    oDev.Fecha.ToString(GlobalClass.FormatoFechaHora)
                    + ": " + (oDev.EsCancelacion ? "CANCELACIÓN" : "DEVOLUCIÓN") + " (" + oDev.VentaDevolucionID.ToString() + ")"
                    + " / " + "TIENDA: " + oDev.SucursalID
                    + " / " + "MOTIVO: " + oDev.Motivo
                    + " / " + oDev.Observacion
                    + " / " + oDev.FormaDePago + (oDev.FormaDePagoID == Cat.FormasDePago.Vale ? (" (" + oDev.NotaDeCreditoID.ToString() + ")") : "")
                    + " / " + oDev.Realizo + " > " + oDev.AutorizoUsuario
                    + "\r\n"
                );
            }
            // Garantías
            foreach (var oReg in oGarantias)
            {
                this.txtObservaciones.Text += (
                    oReg.Fecha.ToString(GlobalClass.FormatoFechaHora)
                    + ": " + "GARANTÍA (" + oReg.VentaGarantiaID.ToString() + ")"
                    + " / " + "TIENDA: " + oReg.SucursalID
                    + " / " + "MOTIVO: " + oReg.Motivo
                    + " / " + oReg.MotivoObservacion
                    + " / " + "ACCIÓN: " + oReg.Accion + (oReg.AccionID == Cat.VentasGarantiasAcciones.NotaDeCredito ? (" (" + oReg.NotaDeCreditoID.ToString() + ")") : "")
                    + " / " + (oReg.FechaCompletado.HasValue 
                        ? (oReg.FechaCompletado.Value.ToString(GlobalClass.FormatoFechaHora) + " ") : "") + oReg.ObservacionCompletado
                    + " / " + oReg.Realizo + " > " + oReg.AutorizoUsuario
                    + "\r\n"
                );
            }
            
            // Notas de crédito fiscales
            var oNcFiscales = General.GetListOf<NotaDeCreditoFiscalDetalle>(c => c.VentaID == iVentaID);
            foreach (var oReg in oNcFiscales)
            {
                var oNcfV = General.GetEntity<NotasDeCreditoFiscalesView>(c => c.NotaDeCreditoFiscalID == oReg.NotaDeCreditoFiscalID);
                this.txtObservaciones.Text += (
                    oNcfV.Fecha.ToString(GlobalClass.FormatoFechaHora)
                    + ": NOTA DE CRÉDITO FISCAL (" + oReg.NotaDeCreditoFiscalID.ToString() + ")"
                    + " / ANTES: " + oReg.ImporteAntes.Valor().ToString(GlobalClass.FormatoMoneda)
                    + " / L. P.: " + oReg.ListaDePreciosUsada.Valor().ToString()
                    + " / DESCUENTO: " + (oReg.Descuento + oReg.IvaDescuento).ToString(GlobalClass.FormatoMoneda)
                    + " / USUARIO: " + oNcfV.Usuario
                    + "\r\n"
                );
            }
        }
    }
}
