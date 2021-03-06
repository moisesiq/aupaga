﻿using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class Consumibles : ListadoSimpleBotones
    {
        public Consumibles()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        // Se cargan los datos por el método "ActualizarDatos", que es llamado desde el "Load" de la base.

        protected override void btnNuevo_Click(object sender, EventArgs e)
        {
            object oNombre = UtilLocal.ObtenerValor("Indica el nombre del Consumible:", "", MensajeObtenerValor.Tipo.Texto);
            if (Util.Cadena(oNombre) != "")
            {
                var oRegistro = new ContaConsumible() { Consumible = Util.Cadena(oNombre) };
                Datos.Guardar<ContaConsumible>(oRegistro);
                this.ActualizarDatos();
            }
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;

            string sNombreAnt = Util.Cadena(this.dgvDatos.CurrentRow.Cells["Consumible"].Value);
            object oNombre = UtilLocal.ObtenerValor("Indica el nombre del Consumible:", sNombreAnt, MensajeObtenerValor.Tipo.Texto);
            string sNombre = Util.Cadena(oNombre);
            if (sNombre != "" && sNombre != sNombreAnt)
            {
                int iRegistroID = Util.Entero(this.dgvDatos.CurrentRow.Cells["ContaConsumibleID"].Value);
                var oRegistro = Datos.GetEntity<ContaConsumible>(c => c.ContaConsumibleID == iRegistroID);
                oRegistro.Consumible = sNombre;
                Datos.Guardar<ContaConsumible>(oRegistro);
                this.ActualizarDatos();
            }
        }

        protected override void btnEliminar_Click(object sender, EventArgs e)
        {
            int iRegistroID = Util.Entero(this.dgvDatos.CurrentRow.Cells["ContaConsumibleID"].Value);
            var oRegistro = Datos.GetEntity<ContaConsumible>(c => c.ContaConsumibleID == iRegistroID);

            string sPregunta = string.Format("¿Estás seguro que deseas eliminar el {0}: \"{1}\"?", "Consumible", oRegistro.Consumible);
            if (UtilLocal.MensajePregunta(sPregunta) != DialogResult.Yes)
                return;

            Datos.Eliminar<ContaConsumible>(oRegistro);
            this.ActualizarDatos();
        }

        #endregion

        public override void ActualizarDatos()
        {
            var oDatos = Util.ListaEntityADataTable<ContaConsumible>(Datos.GetListOf<ContaConsumible>());
            base.CargarDatos(oDatos);
            Util.OcultarColumnas(this.dgvDatos, "ContaConsumibleID");
            this.dgvDatos.AutoResizeColumns();
        }
    }
}
