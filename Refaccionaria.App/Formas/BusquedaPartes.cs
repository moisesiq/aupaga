using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class BusquedaPartes : Form
    {
        bool bSeleccionMultiple;
        public Dictionary<string, object> Seleccion;
        public List<object> ListaSeleccion;
        
        // Para la búsqueda avanzada
        const int BusquedaRetrasoTecla = 400;
        int BusquedaLlamada = 0;
        int BusquedaIntento = 0;
        
        public BusquedaPartes()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void BusquedaPartes_Load(object sender, EventArgs e)
        {

        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (!this.txtBusqueda.Focused)
                return;

            // Se implementa mecanismo de restraso para teclas, si se presionan demasiado rápido, no se hace la búsqueda
            this.BusquedaLlamada++;
            new System.Threading.Thread(this.IniciarBusquedaAsincrona).Start();
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!this.bSeleccionMultiple)
                this.btnAceptar_Click(sender, e);
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (!this.bSeleccionMultiple)
                        this.btnAceptar_Click(sender, e);
                    break;
                case Keys.Space:
                    if (this.dgvDatos.CurrentRow != null)
                        this.dgvDatos.CurrentRow.Cells["Sel"].Value = !Helper.ConvertirBool(this.dgvDatos.CurrentRow.Cells["Sel"].Value);
                    break;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.AccionAceptar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
                
        private void btnCerrrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void IniciarBusquedaAsincrona()
        {
            System.Threading.Thread.Sleep(BusquedaRetrasoTecla);
            if (++BusquedaIntento == this.BusquedaLlamada)
                this.Invoke(new Action(this.BusquedaAvanzada));
        }

        private void BusquedaAvanzada()
        {
            var oFiltros = new Dictionary<string, object>();
            var Cadenas = this.txtBusqueda.Text.Split(' ');
            for (int iCont = 0; iCont < Cadenas.Length && iCont < 9; iCont++)
                oFiltros.Add(string.Format("Descripcion{0}", iCont + 1), Cadenas[iCont]);
            
            var lst = General.ExecuteProcedure<pauParteBusquedaGeneral_Result>("pauParteBusquedaGeneral", oFiltros);
            if (lst != null)
            {
                this.dgvDatos.Rows.Clear();
                foreach (var oReg in lst)
                {
                    int iFila = this.dgvDatos.Rows.Add(oReg.ParteID, false, oReg.NumeroDeParte, oReg.Descripcion, oReg.Marca, oReg.Linea, oReg.Proveedor);
                    this.dgvDatos.Rows[iFila].Tag = oReg;
                }
            }
        }

        private bool AccionAceptar()
        {
            if (this.bSeleccionMultiple)
            {
                this.ListaSeleccion = new List<object>();
                foreach (DataGridViewRow oFila in this.dgvDatos.Rows)
                {
                    if (Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                        this.ListaSeleccion.Add(oFila.Tag);
                }
            }
            else
            {
                this.Seleccion = this.dgvDatos.CurrentRow.ADiccionario();
            }

            return true;
        }

        #endregion

        #region [ Públicos ]

        public void HacerMultiple()
        {
            this.dgvDatos.ReadOnly = false;
            foreach (DataGridViewColumn oCol in this.dgvDatos.Columns)
                oCol.ReadOnly = true;
            this.dgvDatos.Columns["Sel"].Visible = true;
            this.dgvDatos.Columns["Sel"].ReadOnly = false;
            this.bSeleccionMultiple = true;
        }

        #endregion

    }
}
