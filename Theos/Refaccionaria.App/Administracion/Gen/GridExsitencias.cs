﻿using System;
using System.Windows.Forms;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class GridExsitencias : UserControl
    {
        public GridExsitencias()
        {
            InitializeComponent();
        }

        #region [ Eventos ]
        #endregion

        #region [ Métodos ]
        #endregion

        #region [ Públicos ]

        public void CargarDatos(int iParteID)
        {
            var oExistencias = Datos.GetListOf<ExistenciasView>(c => c.ParteID == iParteID);
            this.dgvExistencias.Rows.Clear();
            foreach (var oReg in oExistencias)
            {
                int iFila = this.dgvExistencias.Rows.Add(oReg.Tienda, oReg.Exist, oReg.Max, oReg.Min);
                this.dgvExistencias.Rows[iFila].Tag = oReg;
            }
        }

        public void LimpiarDatos()
        {
            this.dgvExistencias.Rows.Clear();
        }

        #endregion
    }
}
