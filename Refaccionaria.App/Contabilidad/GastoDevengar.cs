using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class GastoDevengar : Form
    {
        ContaEgreso oEgreso;
        bool bTieneDetalle;
        decimal mImporteDev;
        ControlError ctlError = new ControlError();
        List<Sucursal> oSucursales;
        List<int> EgresosDevBorrados = new List<int>();

        public GastoDevengar(int iContaEgresoID)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria_Ant;

            this.oEgreso = General.GetEntity<ContaEgreso>(c => c.ContaEgresoID == iContaEgresoID);
        }

        #region [ Eventos ]

        private void GastoDevengar_Load(object sender, EventArgs e)
        {
            this.ActiveControl = this.cmbSucursal;
            this.oSucursales = General.GetListOf<Sucursal>(c => c.Estatus);
            this.cmbSucursal.CargarDatos("SucursalID", "NombreSucursal", this.oSucursales);

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
            var oCuenta = General.GetEntity<ContaCuentaAuxiliar>(c => c.ContaCuentaAuxiliarID == this.oEgreso.ContaCuentaAuxiliarID);
            this.txtCuentaAuxiliar.Text = oCuenta.CuentaAuxiliar;
            this.txtConcepto.Text = this.oEgreso.Observaciones;
            this.txtImporte.Text = this.oEgreso.Importe.ToString(GlobalClass.FormatoMoneda);
            this.cmbSucursal.SelectedValue = this.oEgreso.SucursalID;
            // Se llena y configura el detalle, si aplica
            var oDet = General.GetListOf<ContaEgresosDetalleView>(c => c.ContaEgresoID == this.oEgreso.ContaEgresoID);
            if (oDet.Count > 0)
            {
                this.dgvEgresoDetalle.Rows.Clear();
                foreach (var oConDev in oDet)
                {
                    int iFila = this.dgvEgresoDetalle.Rows.Add(oConDev.ContaEgresoDetalleID, oConDev.Consumible, (oConDev.Cantidad - oConDev.CantidadDev.Valor()));
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
            var oDevs = General.GetListOf<ContaEgresoDevengado>(c => c.ContaEgresoID == this.oEgreso.ContaEgresoID);
            this.mImporteDev = 0;
            this.dgvDetalle.Rows.Clear();
            foreach (var oDev in oDevs)
            {
                string sSucursal = this.oSucursales.FirstOrDefault(c => c.SucursalID == oDev.SucursalID).NombreSucursal;
                this.dgvDetalle.Rows.Add(oDev.ContaEgresoDevengadoID, oDev.Fecha, sSucursal, oDev.Importe);
                this.mImporteDev += oDev.Importe;
            }
            // Se agrega el total
            this.dgvTotales.Rows.Clear();
            this.dgvTotales.Rows.Add("", this.mImporteDev);
        }

        private void AplicarCambioCantidad(DataGridViewRow Fila)
        {
            var oEgresoDetV = (Fila.Tag as ContaEgresosDetalleView);
            int iCantidad = Helper.ConvertirEntero(Fila.Cells["CantidadDev"].Value);
            int iRestante = (int)Helper.ConvertirDecimal(Fila.Cells["Restante"].Value);

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
            this.CalcularImporteTotal();
        }

        private void CalcularImporteTotal()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow Fila in this.dgvEgresoDetalle.Rows)
                mTotal += Helper.ConvertirDecimal(Fila.Cells["egImporte"].Value);
            this.txtImporteDev.Text = mTotal.ToString();
        }

        private void BorrarEgresoDevGrid(DataGridViewRow Fila)
        {
            // Se resta el importe de lo devengado
            decimal mImporte = Helper.ConvertirDecimal(Fila.Cells["Importe"].Value);
            this.mImporteDev -= mImporte;
            // Se modifican los importes restantes, si hay detalle
            int iEgresoDevID = Helper.ConvertirEntero(Fila.Cells["ContaEgresoDevengadoID"].Value);
            if (this.bTieneDetalle)
            {
                var oDevDetalle = General.GetListOf<ContaEgresoDetalleDevengado>(c => c.ContaEgresoDevengadoID == iEgresoDevID);
                foreach (var oDevDet in oDevDetalle)
                {
                    int iFila = this.dgvEgresoDetalle.EncontrarIndiceDeValor("ContaEgresoDetalleID", oDevDet.ContaEgresoDetalleID);
                    this.dgvEgresoDetalle["Restante", iFila].Value = (Helper.ConvertirDecimal(this.dgvEgresoDetalle["Restante", iFila].Value) + oDevDet.Cantidad);
                    this.AplicarCambioCantidad(this.dgvEgresoDetalle.Rows[iFila]);
                }
            }
            // Se mete el EgresoDev a la lista para borrar
            this.EgresosDevBorrados.Add(iEgresoDevID);
            // Se quita la fila del grid
            this.dgvDetalle.Rows.Remove(Fila);
            // Se actualizan los totales
            this.dgvTotales["TotalImporte", 0].Value = this.mImporteDev;
        }

        private bool AccionGuardar()
        {
            if (!this.Validar())
                return false;

            // Se borran los "devengados", si hay
            foreach (int iDevBorrarID in this.EgresosDevBorrados)
                ContaProc.DevengadoEliminar(iDevBorrarID);

            if (Helper.ConvertirDecimal(this.txtImporteDev.Text) > 0)
            {
                // Se genera el "devengado"
                var oEgresoDev = new ContaEgresoDevengado()
                {
                    ContaEgresoID = this.oEgreso.ContaEgresoID,
                    Fecha = this.dtpFecha.Value,
                    Importe = Helper.ConvertirDecimal(this.txtImporteDev.Text),
                    SucursalID = Helper.ConvertirEntero(this.cmbSucursal.SelectedValue),
                };
                // Se genera el detalle, si aplica
                var oDetalleDev = new List<ContaEgresoDetalleDevengado>();
                if (this.bTieneDetalle)
                {
                    foreach (DataGridViewRow Fila in this.dgvEgresoDetalle.Rows)
                    {
                        int iCantidadDev = Helper.ConvertirEntero(Fila.Cells["CantidadDev"].Value);
                        if (iCantidadDev > 0)
                        {
                            oDetalleDev.Add(new ContaEgresoDetalleDevengado()
                            {
                                ContaEgresoDetalleID = Helper.ConvertirEntero(Fila.Cells["ContaEgresoDetalleID"].Value),
                                Cantidad = iCantidadDev
                            });
                        }
                    }
                }
                // Se manda guardar los datos
                ContaProc.GastoDevengar(oEgresoDev, oDetalleDev);
            }

            // Se muestra una notificación
            UtilLocal.MostrarNotificacion("Proceso completado correctamente.");
            
            return true;
        }

        private bool Validar()
        {
            this.ctlError.LimpiarErrores();
            if (Helper.ConvertirDecimal(this.txtImporteDev.Text) <= 0 && this.EgresosDevBorrados.Count <= 0)
            {
                this.ctlError.PonerError(this.txtImporteDev, "Importe inválido.", ErrorIconAlignment.MiddleLeft);
            }
            else if (this.txtImporteDev.Enabled)
            {
                if ((Helper.ConvertirDecimal(this.txtImporteDev.Text) + this.mImporteDev) > this.oEgreso.Importe)
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
            return (this.ctlError.NumeroDeErrores == 0 && bDetalleVal);
        }

        #endregion
                
                                                
    }
}
