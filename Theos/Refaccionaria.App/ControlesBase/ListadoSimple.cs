using System;
using System.Windows.Forms;
using System.Text;
using System.Data;

using LibUtil;

namespace Refaccionaria.App
{
    public partial class ListadoSimple : UserControl
    {
        const int BotonesAlto = 23;
        const int BotonesMargenAbajo = 6;

        BindingSource FuenteDatos;

        public ListadoSimple()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        bool _EspacioBotones = false;
        public bool EspacioBotones
        {
            get { return this._EspacioBotones; }
            set
            {
                if (this._EspacioBotones == value) return;

                this._EspacioBotones = value;
                int iEspacio = (BotonesAlto + BotonesMargenAbajo);
                iEspacio = (this._EspacioBotones ? iEspacio : (iEspacio * -1));
                
                this.dgvDatos.Height -= iEspacio;
                this.dgvDatos.Top += iEspacio;
                // this.txtBusqueda.Top += iEspacio;
            }
        }

        #endregion

        #region [ Eventos ]

        private void ListadoSimple_Load(object sender, EventArgs e)
        {
            this.ActualizarDatos();
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (this.FuenteDatos == null)
                return;

            if (this.txtBusqueda.Text == "")
            {
                this.FuenteDatos.Filter = "";
                return;
            }

            this.FuenteDatos.Filter = Util.ObtenerCadenaDeFiltro(this.dgvDatos, this.txtBusqueda.Text);
        }

        protected virtual void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        protected virtual void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {

        }

        #endregion

        #region [ Métodos ]

        public virtual void ActualizarDatos() { }

        public void CargarDatos(object Datos)
        {
            this.FuenteDatos = new BindingSource(Datos, "");
            this.dgvDatos.DataSource = this.FuenteDatos;
            this.dgvDatos.AutoResizeColumns();
        }

        #endregion
                
        
    }
}
