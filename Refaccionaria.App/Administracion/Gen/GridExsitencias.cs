using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

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
            var oExistencias = General.GetListOf<ExistenciasView>(c => c.ParteID == iParteID);
            this.dgvExistencias.Rows.Clear();
            foreach (var oReg in oExistencias)
                this.dgvExistencias.Rows.Add(oReg.Tienda, oReg.Exist, oReg.Max, oReg.Min);
        }

        public void LimpiarDatos()
        {
            this.dgvExistencias.Rows.Clear();
        }

        #endregion
    }
}
