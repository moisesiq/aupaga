﻿using System;
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
    public partial class modelos : ListadoBase
    {
        // Para el Singleton
        private static modelos _Instance;
        public static modelos Instance
        {
            get
            {
                if (modelos._Instance == null || modelos._Instance.IsDisposed)
                    modelos._Instance = new modelos();
                return modelos._Instance;
            }
        }
        //

        public modelos()
        {
            InitializeComponent();            
        }

        #region [ Metodos ]

        public void ActualizarListado()
        {
            try
            {
                var dt = UtilLocal.newTable<ModelosView>("Modelos", Datos.GetListOf<ModelosView>(m => m.ModeloID > 0));
                this.IniciarCarga(dt, "Modelos");
                Util.OcultarColumnas(this.dgvDatos, new string[] { "MarcaID" });
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
            DetalleModelo m = new DetalleModelo();
            m.ShowDialog();
        }

        protected override void btnModificar_Click(object sender, EventArgs e)
        {
            if (this.dgvDatos.CurrentRow == null)
                return;
            DetalleModelo m = new DetalleModelo(Util.Entero(this.dgvDatos.CurrentRow.Cells["ModeloID"].Value));
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
                var res = Util.MensajePregunta("¿Está seguro de que desea eliminar este modelo?", GlobalClass.NombreApp);
                if (res == DialogResult.Yes)
                {
                    try
                    {

                        var fila = this.dgvDatos.CurrentRow.Index;
                        var modeloID = Util.Entero(this.dgvDatos.Rows[fila].Cells["ModeloID"].Value);
                        var modelo = Datos.GetEntity<Modelo>(m => m.ModeloID.Equals(modeloID));
                        if (modelo != null)
                        {
                            var listaMotores = Datos.GetListOf<Motor>(m => m.ModeloID.Equals(modelo.ModeloID));
                            if (listaMotores.Count > 0)
                            {
                                StringBuilder n = new StringBuilder();
                                foreach (var motor in listaMotores)
                                {
                                    var nombres = Datos.GetListOf<Motor>(m => m.MotorID.Equals(motor.MotorID));
                                    foreach (var nombre in nombres)
                                        n.Append(string.Format("{0},", nombre.NombreMotor));
                                }
                                var mensaje = "Primero debe eliminar la relación de este modelo con este motor(es): " + n.ToString();
                                Util.MensajeAdvertencia(mensaje.Substring(0, mensaje.Length - 1), GlobalClass.NombreApp);
                                return;
                            }
                            else
                            {
                                Datos.Delete<Modelo>(modelo);
                                new Notificacion("Modelo Eliminado exitosamente", 2 * 1000).Mostrar(Principal.Instance);
                                this.ActualizarListado();
                            }
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
