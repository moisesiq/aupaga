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
    public partial class motores : ListadoBase
    {
        // Para el Singleton
        private static motores _Instance;
        public static motores Instance
        {
            get
            {
                if (motores._Instance == null || motores._Instance.IsDisposed)
                    motores._Instance = new motores();
                return motores._Instance;
            }
        }
        //

        public motores()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<MotoresView>("Motores", Datos.GetListOf<MotoresView>(m => m.MotorID > 0));
                this.IniciarCarga(dt, "Relaciones");
                Util.OcultarColumnas(this.dgvDatos, new string[] { "GenericoID", "ModeloID", "MarcaID", "MotorID" });
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
            DetalleMotor m = new DetalleMotor();
            m.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleMotor m = new DetalleMotor(Util.Entero(this.dgvDatos.CurrentRow.Cells["MotorID"].Value));
            m.ShowDialog();
        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            this.btnModificar_Click(sender, null);
        }

        protected override void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                this.dgvDatos_CellDoubleClick(sender, null);
            }
            if (e.KeyCode == Keys.Delete)
            {
                var res = Util.MensajePregunta("¿Está seguro de que desea eliminar esta relación?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        var fila = this.dgvDatos.CurrentRow.Index;
                        var motorId = Util.Entero(dgvDatos.Rows[fila].Cells["MotorID"].Value);
                        var motor = Datos.GetEntity<Motor>(m => m.MotorID.Equals(motorId));
                        if (motor != null)
                        {
                            var listaAnio = Datos.GetListOf<MotorAnio>(m => m.MotorID == motorId);
                            foreach(var anio in listaAnio)
                                Datos.Delete<MotorAnio>(anio);

                            Datos.Delete<Motor>(motor);

                            this.ActualizarListado();
                            this.txtBusqueda.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        #endregion

    }
}
