using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class SeleccionarNotasDeCredito : UserControl
    {
        bool Cargado = false;
        int ClienteID;
        int VentaID;

        public SeleccionarNotasDeCredito(int iClienteID, int iVentaID)
        {
            InitializeComponent();
            
            this.ClienteID = iClienteID;
            this.VentaID = iVentaID;
        }

        #region [ Eventos ]

        private void SeleccionarNotasDeCredito_Load(object sender, EventArgs e)
        {
            if (this.Cargado) return;

            // Se obtiene la venta, para filtrar por sucursal
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == this.VentaID && c.Estatus);
            int iSucursalID = (oVenta == null ? 0 : oVenta.SucursalID);
            // Se cargan las notas de crédito del cliente
            var oNotas = General.GetListOf<NotaDeCredito>(q => q.ClienteID == this.ClienteID && (iSucursalID == 0 || q.SucursalID == iSucursalID) && q.Valida && q.Estatus);
            foreach (var oNota in oNotas)
                this.dgvNotas.Rows.Add(oNota.ClienteID, false, oNota.NotaDeCreditoID, oNota.Importe, 0.00);

            this.Cargado = true;
        }

        private void dgvNotas_KeyDown(object sender, KeyEventArgs e)
        {
            this.dgvNotas.VerShift(e, "Usar");
        }

        private void dgvNotas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvNotas.VerDirtyStateChanged("Usar");
        }

        private void dgvNotas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            switch (this.dgvNotas.Columns[e.ColumnIndex].Name)
            {
                case "Usar": 
                    this.FilaCambioUsar(this.dgvNotas.CurrentRow);
                    break;
                case "NotaDeCreditoID":
                    this.FilaCambioId(this.dgvNotas.CurrentRow);
                    break;
                case "ImporteAUsar": 
                    this.FilaCambioImporteAUsar(this.dgvNotas.CurrentRow);
                    break;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.Parent is Form)
                (this.Parent as Form).DialogResult = DialogResult.OK;
        }

        #endregion

        #region [ Uso interno ]

        private void FilaCambioUsar(DataGridViewRow Fila)
        {
            this.CalcularTotal();
        }

        private void FilaCambioId(DataGridViewRow Fila)
        {
            int iNotaID = Helper.ConvertirEntero(Fila.Cells["NotaDeCreditoID"].Value);
            var oNota = General.GetEntity<NotaDeCredito>(q => q.NotaDeCreditoID == iNotaID && q.Valida && q.Estatus);

            if (oNota == null)
                UtilLocal.MensajeAdvertencia("La nota de crédito especificada no es válida.");

            Fila.Cells["colClienteID"].Value = (oNota == null ? 0 : oNota.ClienteID);
            Fila.Cells["Usar"].Value = (oNota != null);
            Fila.Cells["Importe"].Value = (oNota == null ? 0 : oNota.Importe);
            Fila.Cells["ImporteAUsar"].Value = (oNota == null ? 0 : oNota.Importe);
            
            Fila.Cells["Usar"].ReadOnly = (oNota == null);
            Fila.Cells["ImporteAUsar"].ReadOnly = (oNota == null);

            this.CalcularTotal();
        }

        private void FilaCambioImporteAUsar(DataGridViewRow Fila)
        {
            if (Helper.ConvertirDecimal(Fila.Cells["ImporteAUsar"].Value) > Helper.ConvertirDecimal(Fila.Cells["Importe"].Value))
            {
                UtilLocal.MensajeAdvertencia("El importe a usar es mayor que el importe total de la nota de crédito.");
                Fila.Cells["ImporteAUsar"].Value = 0;
            }
            this.CalcularTotal();
        }

        private void CalcularTotal()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow Fila in this.dgvNotas.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["Usar"].Value))
                    mTotal += Helper.ConvertirDecimal(Fila.Cells["ImporteAUsar"].Value);
            }
            this.lblTotal.Text = mTotal.ToString(GlobalClass.FormatoMoneda);
        }

        #endregion

        #region [ Propiedades ]

        public bool HayNotasDeOtrosClientes
        {
            get
            {
                foreach (DataGridViewRow Fila in this.dgvNotas.Rows)
                {
                    if (Helper.ConvertirBool(Fila.Cells["Usar"].Value))
                    {
                        if (Helper.ConvertirEntero(Fila.Cells["colClienteID"].Value) != this.ClienteID)
                            return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region [ Públicos ]

        public Dictionary<int, decimal> GenerarNotasDeCredito()
        {
            var oNotas = new Dictionary<int, decimal>();
            foreach (DataGridViewRow Fila in this.dgvNotas.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["Usar"].Value) && Helper.ConvertirDecimal(Fila.Cells["ImporteAUsar"].Value) > 0)
                    oNotas.Add(Helper.ConvertirEntero(Fila.Cells["NotaDeCreditoID"].Value), Helper.ConvertirDecimal(Fila.Cells["ImporteAUsar"].Value));
            }
            return oNotas;
        }

        public List<int> NotasDeCreditoOtrosClientes()
        {
            var oNotas = new List<int>();
            foreach (DataGridViewRow Fila in this.dgvNotas.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["Usar"].Value) && Helper.ConvertirEntero(Fila.Cells["colClienteID"].Value) != this.ClienteID)
                    oNotas.Add(Helper.ConvertirEntero(Fila.Cells["NotaDeCreditoID"].Value));
            }
            return oNotas;
        }

        #endregion
                
    }
}
