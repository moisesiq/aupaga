﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class lineas : ListadoBase
    {
        // Para el Singleton
        private static lineas _Instance;
        public static lineas Instance
        {
            get
            {
                if (lineas._Instance == null || lineas._Instance.IsDisposed)
                    lineas._Instance = new lineas();
                return lineas._Instance;
            }
        }
        //

        public lineas()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<LineasView>("Lineas", Datos.GetListOf<LineasView>(l => l.LineaID > 0));
                this.IniciarCarga(dt, "Lineas");
                Util.OcultarColumnas(this.dgvDatos, new string[] { "Sistema" });
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetalleLinea l = new DetalleLinea();
            l.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleLinea l = new DetalleLinea(Util.Entero(this.dgvDatos.CurrentRow.Cells["LineaID"].Value));
            l.ShowDialog();
        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            this.btnModificar_Click(sender, null);
        }

        #endregion
        
    }
}
