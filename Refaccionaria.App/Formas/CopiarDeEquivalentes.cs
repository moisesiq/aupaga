using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CopiarDeEquivalentes : Form
    {        
        public CopiarDeEquivalentes(int iLineaID)
        {
            InitializeComponent();

            this.LineaID = iLineaID;

            var oCerrar = new Button();
            oCerrar.Click += new EventHandler((sender, e) => { this.Close(); });
            this.CancelButton = oCerrar;
        }

        #region [ Propiedades ]

        public int LineaID { get; set; }

        #endregion

        #region [ Eventos ]

        private void CopiarDeEquivalentes_Load(object sender, EventArgs e)
        {
            // Se llenan los datos
            var oLinea = General.GetEntity<Linea>(c => c.LineaID == this.LineaID && c.Estatus);
            this.lblLinea.Text = oLinea.NombreLinea;

            var oPartes = General.GetListOf<Parte>(c => c.LineaID == this.LineaID && c.Estatus);
            foreach (var oReg in oPartes)
                this.dgvPartes.Rows.Add(oReg.ParteID, oReg.NumeroParte, oReg.NombreParte);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (this.AccionGuardar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #endregion

        #region [ Métodos ]

        private bool AccionGuardar()
        {
            // Se inicializa la barra de progreso
            this.pgvAvance.Inicializar(this.dgvPartes.Rows.Count, 1);

            foreach (DataGridViewRow oFila in this.dgvPartes.Rows)
            {
                int iParteID = Helper.ConvertirEntero(oFila.Cells["ParteID"].Value);

                if (this.chkAplicaciones.Checked)
                    AdmonProc.CopiarAplicacionesDeEquivalentes(iParteID);
                if (this.chkCodigosAlternos.Checked)
                    AdmonProc.CopiarCodigosAlternosDeEquivalentes(iParteID);
                if (this.chkPartesComplementarias.Checked)
                    AdmonProc.CopiarPartesComplementariasDeEquivalentes(iParteID);

                this.pgvAvance.EjecutarPaso(true);
            }

            this.pgvAvance.Finalizar();

            return true;
        }

        #endregion
                
    }
}
