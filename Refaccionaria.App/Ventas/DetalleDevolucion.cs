using System.Windows.Forms;
using System.Drawing;

using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class DetalleDevolucion : cVentaDetalleMod
    {
        ControlError ctlAdvertencia = new ControlError() { Icon = App.Properties.Resources._16_Ico_Advertencia };

        public DetalleDevolucion()
        {
            InitializeComponent();
        }

        private void DetalleDevolucion_Load(object sender, System.EventArgs e)
        {
            // Se agrega la advertencia de autorización requerida
            Label lblPosicionError = new Label() { Location = new Point(6, 3), Width = 0 };
            this.pnlTotal.Controls.Add(lblPosicionError);
            this.ctlAdvertencia.PonerError(lblPosicionError, "Se requiere una autorización para completar esta operación.");
        }

        protected override void FilaMarcaCambiada(DataGridViewRow Fila)
        {
            base.FilaMarcaCambiada(Fila);

            // Se cambia el color del texto de la fila, según aplique
            bool bMarcada = (Helper.ConvertirBool(Fila.Cells["Aplicar"].Value));
            foreach (DataGridViewCell Celda in Fila.Cells)
            {
                Celda.Style.ForeColor = (bMarcada ? Color.Red : Color.SteelBlue);
                Celda.Style.SelectionForeColor = (bMarcada ? Color.Red : Color.SteelBlue);
            }
        }

        #region [ Públicos ]

        public bool TodosMarcados()
        {
            bool bTodosMarcados = true;
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
            {
                if (!Helper.ConvertirBool(Fila.Cells["Aplicar"].Value))
                {
                    bTodosMarcados = false;
                    break;
                }
            }
            return bTodosMarcados;

        }

        public bool Validar()
        {
            this.ctlError.LimpiarErrores();

            // Se valida que hayan producto con check
            bool bMarcado = false;
            foreach (DataGridViewRow Fila in this.dgvProductos.Rows)
            {
                if (Helper.ConvertirBool(Fila.Cells["Aplicar"].Value))
                {
                    bMarcado = true;
                    break;
                }

            }
            if (!bMarcado)
                this.ctlError.PonerError(this.lblEtTotal, "No hay ningún producto seleccionado.", ErrorIconAlignment.MiddleLeft);

            return (this.ctlError.NumeroDeErrores == 0);
        }
        
        #endregion
    }
}
