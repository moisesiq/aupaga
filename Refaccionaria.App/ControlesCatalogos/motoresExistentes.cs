using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class motoresExistentes : ListadoBase
    {
        // Para el Singleton
        private static motoresExistentes _Instance;
        public static motoresExistentes Instance
        {
            get
            {
                if (motoresExistentes._Instance == null || motoresExistentes._Instance.IsDisposed)
                    motoresExistentes._Instance = new motoresExistentes();
                return motoresExistentes._Instance;
            }
        }
        //

        public motoresExistentes()
        {
            InitializeComponent();            
        }

        #region [ Metodos]

        public void ActualizarListado()
        {
            try
            {
                var dt = Negocio.Helper.newTable<MotoresExistentesView>("Motores", Negocio.General.GetListOf<MotoresExistentesView>(m => m.MotorExistenteID > 0));
                this.IniciarCarga(dt, "Motores");
            }
            catch (Exception ex)
            {
                Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        #endregion

        #region [ Eventos ]

        protected override void btnAgregar_Click(object sender, EventArgs e)
        {
            DetalleMotorExistente m = new DetalleMotorExistente();
            m.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleMotorExistente m = new DetalleMotorExistente(Negocio.Helper.ConvertirEntero(this.dgvDatos.CurrentRow.Cells["MotorExistenteID"].Value));
            m.ShowDialog();
        }

        protected override void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            btnModificar_Click(sender, null);
        }

        protected override void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null) return;
            if (e.KeyCode == Keys.Delete)
            {
                var res = Negocio.Helper.MensajePregunta("¿Está seguro de que desea eliminar este motor?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        var dg = (DataGridView)sender;
                        var fila = dg.SelectedRows[0].Index;
                        //var modeloID = Negocio.Helper.ConvertirEntero(dg.Rows[fila].Cells["ModeloID"].Value);
                        //var modelo = Negocio.General.GetEntity<Modelo.Modelo>(m => m.ModeloID.Equals(modeloID));
                        //if (modelo != null)
                        //{
                        //    var listaMotores = Negocio.General.GetListOf<Motor>(m => m.ModeloID.Equals(modelo.ModeloID));
                        //    if (listaMotores.Count > 0)
                        //    {
                        //        StringBuilder n = new StringBuilder();
                        //        foreach (var motor in listaMotores)
                        //        {
                        //            var nombres = Negocio.General.GetListOf<MotorNombre>(m => m.MotorID.Equals(motor.MotorID));
                        //            foreach (var nombre in nombres)
                        //                n.Append(string.Format("{0},", nombre.NombreMotor));
                        //        }
                        //        var mensaje = "Primero debe eliminar la relación de este modelo con este motor(es): " + n.ToString();
                        //        Negocio.Helper.MensajeAdvertencia(mensaje.Substring(0, mensaje.Length - 1), GlobalClass.NombreApp);
                        //        return;
                        //    }
                        //    else
                        //    {
                        //        Negocio.General.Delete<Modelo.Modelo>(modelo);
                        //        this.ActualizarListado();
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        Negocio.Helper.MensajeError(ex.Message, GlobalClass.NombreApp);
                    }
                }
            }
        }

        #endregion

    }
}
