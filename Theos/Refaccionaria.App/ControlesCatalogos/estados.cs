﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class estados : ListadoBase
    {
        // Para el Singleton
        private static estados _Instance;
        public static estados Instance
        {
            get
            {
                if (estados._Instance == null || estados._Instance.IsDisposed)
                    estados._Instance = new estados();
                return estados._Instance;
            }
        }
        //

        public estados()
        {
            InitializeComponent();
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<EstadosView>("Estados", Datos.GetListOf<EstadosView>().ToList());
                this.IniciarCarga(dt, "Estados");
                Util.OcultarColumnas(base.dgvDatos, new string[] { "Busqueda" });
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos ]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            //DetallePaqueteria p = new DetallePaqueteria();
            //p.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            //DetallePaqueteria p = new DetallePaqueteria(Util.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["ProveedorPaqueteriaID"].Value));
            //p.ShowDialog();
        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            btnModificar_Click(sender, null);
        }

        #endregion

    }
}
