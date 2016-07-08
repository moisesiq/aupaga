using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class GastoDevengar : Form
    {
        ContaEgreso oEgreso;
        bool bTieneDetalle;
        // decimal mImporteDev;
        ControlError ctlError = new ControlError();
        List<Sucursal> oSucursales;
        List<int> EgresosDevBorrados = new List<int>();
        List<int> DevEspecialBorrados = new List<int>();
        enum TipoDev { Sucursal, Duenio }

        public GastoDevengar(int iContaEgresoID)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;

            this.oEgreso = Datos.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iContaEgresoID);
        }

        #region [ Eventos ]

        private void GastoDevengar_Load(object sender, EventArgs e)
        {
            this.ActiveControl = this.cmbSucursal;
            this.oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", this.oSucursales);
            this.cmbDuenio.CargarDatos("DuenioID", "Duenio1", Datos.GetListOf<Duenio>().OrderBy(c => c.Duenio1).ToList());
            this.dgvTotales.Rows.Add();

            // Se llenan los datos
            this.LlenarDatos();

            // 
            if (!this.bTieneDetalle)
            {
                this.dgvEgresoDetalle.Visible = false;
                int iDif = (this.dgvEgresoDetalle.Height + 3);
                // this.pnlParteAbajo.Top -= iDif;
                this.Height -= iDif;
            }
        }

        private void cmbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbSucursal.Focused)
                this.cmbDuenio.SelectedIndex = -1;
        }

        private void cmbDuenio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbDuenio.Focused)
                this.cmbSucursal.SelectedIndex = -1;
        }

        private void dgvEgresoDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvEgresoDetalle.Columns[e.ColumnIndex].Name == "CantidadDev")
            {
                this.AplicarCambioCantidad(this.dgvEgresoDetalle.Rows[e.RowIndex]);
            }
        }
        
        private void dgvDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                if (UtilLocal.MensajePregunta("¿Estás seguro que deseas borrar el Movimiento seleccionado?") == DialogResult.Yes)
                    this.BorrarEgresoDevGrid(this.dgvDetalle.CurrentRow);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Util.Decimal(this.txtImporte.Text) != 0)
                return;

            TipoDev eTipoDev = TipoDev.Sucursal;
            int iSelID = Util.Entero(this.cmbSucursal.SelectedValue);
            string sRelacion = this.cmbSucursal.Text;
            if (iSelID == 0)
            {
                eTipoDev = TipoDev.Duenio;
                iSelID = Util.Entero(this.cmbDuenio.SelectedValue);
                sRelacion = this.cmbDuenio.Text;
            }
            
            int iFila = this.dgvDetalle.Rows.Add(eTipoDev, null, iSelID, this.dtpFecha.Value, sRelacion, Util.Decimal(this.txtImporteDev.Text));
            bool bError = false;
            if (this.bTieneDetalle)
            {
                var oDetalleDev = new List<ContaEgresoDetalleDevengado>();
                var oDetalleDevEsp = new List<ContaEgresoDetalleDevengadoEspecial>();
                foreach (DataGridViewRow oFila in this.dgvEgresoDetalle.Rows)
                {
                    // Se valida que no haya errores
                    if (oFila.ErrorText != "")
                    {
                        bError = true;
                        break;
                    }

                    int iCantidadDev = Util.Entero(oFila.Cells["CantidadDev"].Value);
                    if (iCantidadDev > 0)
                    {
                        if (eTipoDev == TipoDev.Sucursal)
                        {
                            oDetalleDev.Add(new ContaEgresoDetalleDevengado()
                            {
                                ContaEgresoDetalleID = Util.Entero(oFila.Cells["ContaEgresoDetalleID"].Value),
                                Cantidad = iCantidadDev
                            });
                        }
                        else
                        {
                            oDetalleDevEsp.Add(new ContaEgresoDetalleDevengadoEspecial()
                            {
                                ContaEgresoDetalleID = Util.Entero(oFila.Cells["ContaEgresoDetalleID"].Value),
                                Cantidad = iCantidadDev
                            });
                        }

                        // Se ajusta el restante y se deja en cero la cantidad devengada, por si se hace otro devengado
                        oFila.Cells["Restante"].Value = (Util.Decimal(oFila.Cells["Restante"].Value) - iCantidadDev);
                        oFila.Cells["CantidadDev"].Value = null;
                    }
                }
                if (eTipoDev == TipoDev.Sucursal)
                    this.dgvDetalle.Rows[iFila].Tag = oDetalleDev;
                else
                    this.dgvDetalle.Rows[iFila].Tag = oDetalleDevEsp;

                // this.dgvEgresoDetalle.Rows.Clear();
            }

            if (bError)
            {
                this.dgvDetalle.Rows.RemoveAt(iFila);
            }
            else
            {
                this.txtImporteDev.Clear();
                this.CalcularTotal();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void LlenarDatos()
        {
            // Se llenan los datos simples
            var oCuenta = Datos.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == this.oEgreso.ContaCuentaAuxiliarID);
            this.txtCuentaAuxiliar.Text = oCuenta.CuentaAuxiliar;
            this.txtConcepto.Text = this.oEgreso.Observaciones;
            this.txtImporte.Text = this.oEgreso.Importe.ToString(GlobalClass.FormatoMoneda);
            this.cmbSucursal.SelectedValue = this.oEgreso.SucursalID;
            // Se llena y configura el detalle, si aplica
            var oDet = Datos.GetListOf<ContaEgresosDetalleView>(c => c.ContaEgresoID == this.oEgreso.ContaEgresoID);
            if (oDet.Count > 0)
            {
                this.dgvEgresoDetalle.Rows.Clear();
                foreach (var oConDev in oDet)
                {
                    int iFila = this.dgvEgresoDetalle.Rows.Add(oConDev.ContaEgresoDetalleID, oConDev.Consumible
                        , (oConDev.Cantidad - oConDev.CantidadDev.Valor() - oConDev.CantidadDevEspecial.Valor()));
                    this.dgvEgresoDetalle.Rows[iFila].Tag = oConDev;
                }
                this.txtImporteDev.Enabled = false;
                this.bTieneDetalle = true;
            }
            // Se llenan las "devengadas" anteriores
            this.LlenarDev();
        }

        private void LlenarDev()
        {
            var oDevs = Datos.GetListOf<ContaEgresoDevengado>(c => c.ContaEgresoID == this.oEgreso.ContaEgresoID);
            this.dgvDetalle.Rows.Clear();
            foreach (var oDev in oDevs)
            {
                string sSucursal = this.oSucursales.FirstOrDefault(c => c.SucursalID == oDev.SucursalID).NombreSucursal;
                this.dgvDetalle.Rows.Add(TipoDev.Sucursal, oDev.ContaEgresoDevengadoID, oDev.SucursalID, oDev.Fecha, sSucursal, oDev.Importe);
            }

            // Se agregan los devengados especiales
            var oDuenios = Datos.GetListOf<Duenio>();
            var oDevEsp = Datos.GetListOf<ContaEgresoDevengadoEspecial>(c => c.ContaEgresoID == this.oEgreso.ContaEgresoID);
            foreach (var oReg in oDevEsp)
            {
                string sDuenio = oDuenios.FirstOrDefault(c => c.DuenioID == oReg.DuenioID).Duenio1;
                this.dgvDetalle.Rows.Add(TipoDev.Duenio, oReg.ContaEgresoDevengadoEspecialID, oReg.DuenioID, oReg.Fecha, sDuenio, oReg.Importe);
            }

            // Se calcula el total
            this.CalcularTotal();
        }

        private void AplicarCambioCantidad(DataGridViewRow Fila)
        {
            var oEgresoDetV = (Fila.Tag as ContaEgresosDetalleView);
            int iCantidad = Util.Entero(Fila.Cells["CantidadDev"].Value);
            int iRestante = (int)Util.Decimal(Fila.Cells["Restante"].Value);

            Fila.Cells["egImporte"].Value = 0;
            if (iCantidad == 0)
            {
                Fila.ErrorText = "";
                Fila.DefaultCellStyle.ForeColor = this.dgvDetalle.DefaultCellStyle.ForeColor;
            }
            else
            {
                if (iCantidad < 0)
                {
                    Fila.ErrorText = "La Cantidad no puede ser negativa.";
                }
                else if (iCantidad > iRestante)
                {
                    Fila.ErrorText = "La Cantidad no puede ser mayor que lo Restante.";
                }
                else
                {
                    Fila.ErrorText = "";
                    Fila.Cells["egImporte"].Value = (oEgresoDetV.Importe * iCantidad);
                }
                Fila.DefaultCellStyle.ForeColor = Color.Orange;
            }

            // Se calcula el importe total
            this.CalcularTotalDetalle();
        }

        private void CalcularTotalDetalle()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow Fila in this.dgvEgresoDetalle.Rows)
                mTotal += Util.Decimal(Fila.Cells["egImporte"].Value);
            this.txtImporteDev.Text = mTotal.ToString();
        }

        private void CalcularTotal()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
                mTotal += Util.Decimal(oFila.Cells["Importe"].Value);
            this.dgvTotales["TotalImporte", 0].Value = mTotal;
        }

        private void BorrarEgresoDevGrid(DataGridViewRow Fila)
        {
            // Se modifican los importes restantes, si hay detalle
            int iRegistroID = Util.Entero(Fila.Cells["RegistroID"].Value);
            if (iRegistroID > 0)
            {
                if (this.bTieneDetalle)
                {
                    var oDevDetalle = Datos.GetListOf<ContaEgresoDetalleDevengado>(c => c.ContaEgresoDevengadoID == iRegistroID);
                    foreach (var oDevDet in oDevDetalle)
                    {
                        int iFila = this.dgvEgresoDetalle.EncontrarIndiceDeValor("ContaEgresoDetalleID", oDevDet.ContaEgresoDetalleID);
                        this.dgvEgresoDetalle["Restante", iFila].Value = (Util.Decimal(this.dgvEgresoDetalle["Restante", iFila].Value) + oDevDet.Cantidad);
                        this.AplicarCambioCantidad(this.dgvEgresoDetalle.Rows[iFila]);
                    }
                }
                // Se mete el EgresoDev a la lista para borrar correspondiente
                if (((TipoDev)Fila.Cells["colTipoDev"].Value) == TipoDev.Sucursal)
                    this.EgresosDevBorrados.Add(iRegistroID);
                else
                    this.DevEspecialBorrados.Add(iRegistroID);
            }
            // Se quita la fila del grid
            this.dgvDetalle.Rows.Remove(Fila);
            // Se actualizan los totales
            this.CalcularTotal();
        }

        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            // Se borran los "devengados", si hay
            foreach (int iDevBorrarID in this.EgresosDevBorrados)
                ContaProc.DevengadoEliminar(iDevBorrarID);
            // Se borran los especials, si hay
            foreach (int iDevID in this.DevEspecialBorrados)
                ContaProc.DevengadoEspecialEliminar(iDevID);
            
            // Se procesan los nuevos
            foreach (DataGridViewRow oFila in this.dgvDetalle.Rows)
            {
                if (oFila.Cells["RegistroID"].Value != null)
                    continue;

                // Se genera el "devengado", según corresponda
                if (((TipoDev)oFila.Cells["colTipoDev"].Value) == TipoDev.Sucursal)
                {
                    var oEgresoDev = new ContaEgresoDevengado()
                    {
                        ContaEgresoID = this.oEgreso.ContaEgresoID,
                        Fecha = Util.FechaHora(oFila.Cells["Fecha"].Value),
                        Importe = Util.Decimal(oFila.Cells["Importe"].Value),
                        SucursalID = Util.Entero(oFila.Cells["SelID"].Value),
                    };
                    // Se obtiene el detalle, si aplica
                    var oDetalleDev = (oFila.Tag as List<ContaEgresoDetalleDevengado>);

                    // Se manda guardar los datos
                    ContaProc.GastoDevengar(oEgresoDev, oDetalleDev);
                }
                else
                {
                    var oDevEsp = new ContaEgresoDevengadoEspecial()
                    {
                        ContaEgresoID = this.oEgreso.ContaEgresoID,
                        Fecha = Util.FechaHora(oFila.Cells["Fecha"].Value),
                        DuenioID = Util.Entero(oFila.Cells["SelID"].Value),
                        Importe = Util.Decimal(oFila.Cells["Importe"].Value)
                    };
                    // Datos.Guardar<ContaEgresoDevengadoEspecial>(oDevEsp);

                    // Se obtiene el detalle, si aplica
                    var oDetalleDev = (oFila.Tag as List<ContaEgresoDetalleDevengadoEspecial>);

                    // Se manda guardar los datos
                    ContaProc.GastoDevengarEspecial(oDevEsp, oDetalleDev);
                }
            }

            // Se muestra una notificación
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");
            
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();

            /* if (Util.ConvertirDecimal(this.txtImporteDev.Text) <= 0 && this.EgresosDevBorrados.Count <= 0)
            {
                this.ctlError.PonerError(this.txtImporteDev, "Importe inválido.", ErrorIconAlignment.MiddleLeft);
            }
            else if (this.txtImporteDev.Enabled)
            {
                if ((Util.ConvertirDecimal(this.txtImporteDev.Text) + this.mImporteDev) > this.oEgreso.Importe)
                    this.ctlError.PonerError(this.txtImporteDev, "El Importe especificado es mayor a lo restante por devengar.", ErrorIconAlignment.MiddleLeft);
            }

            // Se validan datos del grid, si aplica
            bool bDetalleVal = true;
            if (this.bTieneDetalle)
            {
                foreach (DataGridViewRow Fila in this.dgvEgresoDetalle.Rows)
                {
                    if (Fila.ErrorText != "")
                    {
                        bDetalleVal = false;
                        break;
                    }
                }
            }
            */

            if (Util.Decimal(this.dgvTotales["TotalImporte", 0].Value) > this.oEgreso.Importe)
                this.ctlError.PonerError(this.dgvTotales, "El Importe total es mayor a lo restante por devengar.");

            return this.ctlError.Valido;
        }

        #endregion
                                
    }
}
