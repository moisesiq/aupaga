using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class DetalleBusquedaDeMovimientos : Form
    {
        int iProveedorID;
        public bool bGarantias = false;
        public int iGarantiaID = 0;
        public List<Dictionary<string, object>> Sel;
        public MovimientoInventarioDetalle oMovimientoDetalle;

        public DetalleBusquedaDeMovimientos(int iProveedorID)
        {
            InitializeComponent();

            this.iProveedorID = iProveedorID;
        }

        #region [ Eventos ]

        private void DetalleBusquedaDeMovimientos_Load(object sender, EventArgs e)
        {
            // Se hacen ajustes, si es garantía
            if (this.bGarantias)
            {
                this.dgvPartes.Columns["parFolio"].HeaderText = "Venta";
                this.dgvPartes.Columns["parFolio"].Visible = true;
            }

            // Se llenan los datetimepickers
            this.dtpDesde.Value = DateTime.Now.AddMonths(-3);
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {
            this.LlenarPartes(this.iProveedorID);
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvPartes.Focus();
            }
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            this.LlenarPartes(this.iProveedorID);
        }

        private void txtDescripcion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvPartes.Focus();
            }
        }
        
        private void dgvPartes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvPartes.CurrentRow == null)
                return;
            var iParteID = Helper.ConvertirEntero(this.dgvPartes.CurrentRow.Cells["parParteID"].Value);
            this.LlenarFacturas(iParteID);
        }

        private void dgvPartes_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                this.dgvPartes_CellClick(sender, new DataGridViewCellEventArgs(0, this.dgvPartes.CurrentRow.Index));
            }
        }

        private void dgvFacturas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.btnAceptar_Click(sender, e);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.AccionAceptar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #endregion

        #region [ Metodos ]

        private void LlenarPartes(int iProveedorID)
        {
            // Se obtienen los parámetros
            var oParams = new Dictionary<string, object>();
            oParams.Add("ProveedorID", iProveedorID);
            oParams.Add("Desde", this.dtpDesde.Value);
            oParams.Add("Hasta", this.dtpHasta.Value);
            oParams.Add("Garantias", this.bGarantias);

            if (this.txtCodigo.Text == "")
            {
                var aPalabras = this.txtDescripcion.Text.Split(' ');
                for (int iCont = 0; iCont < aPalabras.Length && iCont < 9; iCont++)
                    oParams.Add(string.Format("Descripcion{0}", iCont + 1), aPalabras[iCont]);
            }
            else
            {
                oParams.Add("Codigo", this.txtCodigo.Text);
            }

            // Se llenan las partes
            var oDatos = General.ExecuteProcedure<pauPartesMovimientosDevoluciones_Result>("pauPartesMovimientosDevoluciones", oParams);
            this.dgvPartes.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvPartes.Rows.Add(oReg.ParteID, oReg.Id, oReg.Folio, oReg.NumeroDeParte, oReg.Descripcion, oReg.Marca, oReg.Linea, oReg.Costo);

            this.lblEncontrados.Text = string.Format("Encontrados: {0}", oDatos.Count);
        }

        private void LlenarFacturas(int iParteID)
        {
            DateTime dDesde = this.dtpDesde.Value.Date;
            DateTime dHasta = this.dtpHasta.Value.Date.AddDays(1);
            var oDatos = General.GetListOf<MovimientosInventarioDetalleAvanzadoView>(c => c.TipoOperacionID == Cat.TiposDeOperacionMovimientos.EntradaCompra 
                && c.ParteID == iParteID && (c.Cantidad - c.CantidadDevuelta) > 0 && (c.FechaFactura >= dDesde && c.FechaFactura < dHasta));
            this.dgvFacturas.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvFacturas.Rows.Add(oReg.MovimientoInventarioDetalleID, oReg.FolioFactura, oReg.FechaFactura, oReg.FechaRecepcion, oReg.ImporteTotal, oReg.Usuario);
        }

        private bool AccionAceptar()
        {
            if (this.dgvFacturas.CurrentRow == null)
            {
                UtilLocal.MensajeAdvertencia("No hay ninguna factura seleccionada.");
                return false;
            }

            // Se llena el dato de Id, para garantías
            int iId = Helper.ConvertirEntero(this.dgvPartes.CurrentRow.Cells["parId"].Value);
            if (this.bGarantias)
                this.iGarantiaID = iId;
            // Se llena el movimiento detalle
            int iIdDet = Helper.ConvertirEntero(this.dgvFacturas.CurrentRow.Cells["facMovimientoInventarioDetalleID"].Value);
            this.oMovimientoDetalle = General.GetEntity<MovimientoInventarioDetalle>(c => c.MovimientoInventarioDetalleID == iIdDet && c.Estatus);

            return true;
        }

        #endregion
                        
    }
}
