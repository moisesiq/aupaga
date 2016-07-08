using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class DetalleBusquedaDeCriterios : Form
    {

        public string Sel;

        public static DetalleBusquedaDeCriterios Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly DetalleBusquedaDeCriterios instance = new DetalleBusquedaDeCriterios();
        }

        public DetalleBusquedaDeCriterios()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void DetalleBusquedaDeCriterios_Load(object sender, EventArgs e)
        {
            this.cargaInicial();
            try
            {
                var criterio = Datos.GetListOf<MxMnCriterioView>(m => m.MxMnCriterioID>0);
                this.dgvDatos.DataSource = criterio;
                UtilLocal.ColumnasToHeaderText(this.dgvDatos);

                if (!this.dgvDatos.Columns.Contains("X"))
                {
                    DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                    checkColumn.Name = "X";
                    checkColumn.HeaderText = " ";
                    checkColumn.Width = 50;
                    checkColumn.ReadOnly = false;
                    checkColumn.FillWeight = 10;
                    this.dgvDatos.Columns.Add(checkColumn);
                    this.dgvDatos.Columns["X"].DisplayIndex = 0;
                }

                foreach (DataGridViewColumn column in this.dgvDatos.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (!column.Name.Equals("X"))
                    {
                        column.ReadOnly = true;
                    }
                }

                this.dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }
        
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                var sb = new StringBuilder();
                var res = string.Empty;
                foreach (DataGridViewRow row in this.dgvDatos.Rows)
                {
                    if (Util.Logico(row.Cells["X"].Value).Equals(true))
                    {
                        sb.Append(row.Cells["MxMnCriterioID"].Value);
                        sb.Append(",");
                    }
                }
                if (sb.ToString().Length > 0)
                {
                    if (sb.ToString().Substring(sb.ToString().Length - 1, 1) == ",")
                        res = sb.ToString().Substring(0, sb.ToString().Length - 1);
                }
                Sel = res;
                this.Close();
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Metodos ]

        private void cargaInicial()
        {
            this.Sel = string.Empty;
            try
            {
                this.dgvDatos.DataSource = null;
                if (this.dgvDatos.Columns.Count > 0)
                    this.dgvDatos.Columns.Clear();
                if (this.dgvDatos.Rows.Count > 0)
                    this.dgvDatos.Rows.Clear();
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

    }
}
