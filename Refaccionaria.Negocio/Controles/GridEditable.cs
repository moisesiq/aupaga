using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using Refaccionaria.Modelo;

namespace Refaccionaria.Negocio
{
    public class GridEditable : DataGridView
    {
        private bool Inicializado = false;

        public GridEditable()
        {
            // Se modifican propiedades
            this.AllowUserToDeleteRows = false;
            this.MultiSelect = false;

            /* if (!this.DesignMode)
            {
                // Se agregan las columnas de control
                this.Columns.Add("__Id", "Id");
                this.Columns.Add("__Cambio", "Cambio");
                foreach (DataGridViewColumn oCol in this.Columns)
                {
                    oCol.ReadOnly = true;
                    oCol.Visible = false;
                }
            } */

            // Se agregan los eventos
            this.RowsAdded += this.Evento_RowsAdded;
            this.KeyDown += this.Evento_KeyDown;
            this.CellValueChanged += this.Evento_CellValueChanged;
        }

        #region [ Propiedades ]

        private Color _ColorNuevo = Color.Blue;
        public Color ColorNuevo { get { return this._ColorNuevo; } set { this._ColorNuevo = value; } }
        private Color _ColorModificado = Color.Orange;
        public Color ColorModificado { get { return this._ColorModificado; } set { this._ColorModificado = value; } }
        private Color _ColorBorrado = Color.Gray;
        public Color ColorBorrado { get { return this._ColorBorrado; } set { this._ColorBorrado = value; } }

        private bool _PermitirBorrar = true;
        public bool PermitirBorrar { get { return this._PermitirBorrar; } set { this._PermitirBorrar = value; } }

        #endregion

        #region [ Eventos ]

        protected virtual void Evento_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!this.Inicializado) return;

            var oFila = this.Rows[e.RowIndex];
            if (Helper.ConvertirEntero(oFila.Cells["__Cambio"].Value) == Cat.TiposDeAfectacion.NoEspecificado)
            {
                oFila.Cells["__Cambio"].Value = Cat.TiposDeAfectacion.Agregar;
                oFila.DefaultCellStyle.ForeColor = this.ColorNuevo;
            }
        }

        protected virtual void Evento_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.CurrentRow == null || this.CurrentRow.IsNewRow) return;

            if (e.Shift && e.KeyCode == Keys.Delete && this.PermitirBorrar)
            {
                if (Helper.ConvertirEntero(this.CurrentRow.Cells["__Cambio"].Value) == Cat.TiposDeAfectacion.Agregar)
                {
                    this.Rows.Remove(this.CurrentRow);
                }
                else if (Helper.ConvertirEntero(this.CurrentRow.Cells["__Cambio"].Value) == Cat.TiposDeAfectacion.Borrar)
                {
                    this.CurrentRow.Cells["__Cambio"].Value = Cat.TiposDeAfectacion.Modificar;
                    this.CurrentRow.DefaultCellStyle.ForeColor = this.ColorModificado;
                }
                else
                {
                    this.CurrentRow.Cells["__Cambio"].Value = Cat.TiposDeAfectacion.Borrar;
                    this.CurrentRow.DefaultCellStyle.ForeColor = this.ColorBorrado;
                }
            }
        }

        protected virtual void Evento_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var oFila = this.Rows[e.RowIndex];

            if (Helper.ConvertirEntero(oFila.Cells["__Cambio"].Value) == Cat.TiposDeAfectacion.SinCambios)
            {
                oFila.Cells["__Cambio"].Value = Cat.TiposDeAfectacion.Modificar;
                oFila.DefaultCellStyle.ForeColor = this.ColorModificado;
            }
        }

        #endregion

        #region [ Públicos ]

        public void Inicializar()
        {
            if (this.DesignMode) return;

            // Se verifica si ya se han agregado columnas
            if (this.Columns.Count == 0)
                Helper.MensajeAdvertencia("El Grid Editable no tiene ninguna columna al momento de inicializar. Verificar.", "Advertencia");

            // Se agregan las columnas de control
            // this.Columns.Insert(0, new DataGridViewColumn() { Name = "__Id", HeaderText = "Id", ReadOnly = true, Visible = false });
            // this.Columns.Insert(1, new DataGridViewColumn() { Name = "__Cambio", HeaderText = "Cambio", ReadOnly = true, Visible = false });
            this.Columns.Add("__Id", "Id");
            this.Columns.Add("__Cambio", "Cambio");
            for (int iCol = (this.Columns.Count - 2); iCol < this.Columns.Count; iCol++)
            {
                this.Columns[iCol].ReadOnly = true;
                this.Columns[iCol].Visible = false;
            }

            // Se marca la primer fila como nueva, si hay
            foreach (DataGridViewRow oFila in this.Rows)
            {
                if (oFila.IsNewRow)
                {
                    oFila.Cells["__Cambio"].Value = Cat.TiposDeAfectacion.Agregar;
                    oFila.DefaultCellStyle.ForeColor = this.ColorNuevo;
                }
            }
            
            this.Inicializado = true;
        }

        public DataGridViewRow AgregarFila(int iId, int iIdCambio, params object[] aColumnas)
        {
            var aValores = new object[aColumnas.Length + 2];
            aColumnas.CopyTo(aValores, 0);
            aValores[aColumnas.Length] = iId;
            aValores[aColumnas.Length + 1] = iIdCambio;

            int iFila = this.Rows.Add(aValores);
            var oFila = this.Rows[iFila];
            // oFila.Cells["__Id"].Value = iId;
            // oFila.Cells["__Cambio"].Value = iIdCambio;

            return oFila;
        }

        public int ObtenerId(DataGridViewRow oFila)
        {
            return Helper.ConvertirEntero(oFila.Cells["__Id"].Value);
        }

        public void EstablecerId(DataGridViewRow oFila, int iId)
        {
            oFila.Cells["__Id"].Value = iId;
        }

        public int ObtenerIdCambio(DataGridViewRow oFila)
        {
            return Helper.ConvertirEntero(oFila.Cells["__Cambio"].Value);
        }

        public void EstablecerIdCambio(DataGridViewRow oFila, int iIdCambio)
        {
            oFila.Cells["__Cambio"].Value = iIdCambio;
        }

        public void VerAgregarFilaNueva()
        {
            this.NotifyCurrentCellDirty(true);
            this.NotifyCurrentCellDirty(false);
        }

        #endregion
    }
}
