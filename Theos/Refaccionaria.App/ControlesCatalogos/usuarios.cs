using System;
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
    public partial class usuarios : ListadoBase
    {
        // Para el Singleton
        private static usuarios _Instance;
        public static usuarios Instance
        {
            get
            {
                if (usuarios._Instance == null || usuarios._Instance.IsDisposed)
                    usuarios._Instance = new usuarios();
                return usuarios._Instance;
            }
        }
        //

        public usuarios()
        {
            InitializeComponent();            
        }

        #region [ Metodos ] 

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<UsuariosView>("Usuarios", Datos.GetListOf<UsuariosView>(u => u.UsuarioID > 0));
                this.IniciarCarga(dt, "Usuarios");
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
            DetalleUsuario u = new DetalleUsuario();
            u.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleUsuario u = new DetalleUsuario(Util.Entero(this.dgvDatos.CurrentRow.Cells["UsuarioID"].Value));
            u.ShowDialog();
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
