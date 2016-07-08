using System;
using System.Windows.Forms;
using System.Drawing;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class configuraciones : UserControl
    {
        BindingSource fuenteDatos;
        // Para el Singleton
        private static configuraciones _Instance;
        public static configuraciones Instance
        {
            get
            {
                if (configuraciones._Instance == null || configuraciones._Instance.IsDisposed)
                    configuraciones._Instance = new configuraciones();
                return configuraciones._Instance;
            }
        }
        //
        
        public configuraciones()
        {
            InitializeComponent();            
        }

        #region [ Eventos ]

        private void configuraciones_Load(object sender, EventArgs e)
        {
            this.dgvDatos.DefaultCellStyle.ForeColor = Color.Black;
            this.ActualizarListado();
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (this.txtBusqueda.Text == "")
                this.fuenteDatos.RemoveFilter();
            else
                this.fuenteDatos.Filter = this.dgvDatos.ObtenerCadenaDeFiltro(this.txtBusqueda.Text);
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleConfiguracion c = new DetalleConfiguracion(Util.Entero(this.dgvDatos.CurrentRow.Cells["ConfiguracionID"].Value));
            c.ShowDialog(Principal.Instance);
        }

        #endregion

        #region [ Públicos ]

        public void ActualizarListado()
        {
            var dt = UtilLocal.newTable<Configuracion>("Configuraciones", Datos.GetListOf<Configuracion>(m => m.ConfiguracionID > 0));
            this.fuenteDatos = new BindingSource();
            this.fuenteDatos.DataSource = dt;
            this.dgvDatos.DataSource = this.fuenteDatos;
            this.dgvDatos.AutoResizeColumns();
            Util.OcultarColumnas(this.dgvDatos, new string[] { "Busqueda", "EntityState", "EntityKey" });
            UtilLocal.ColumnasToHeaderText(this.dgvDatos);
        }

        #endregion

    }
}
