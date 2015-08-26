using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class AsistenciaUsuario : UserControl
    {
        // Singleton
        private static AsistenciaUsuario _Instance;
        public static AsistenciaUsuario Instance
        {
            get
            {
                if (AsistenciaUsuario._Instance == null || AsistenciaUsuario._Instance.IsDisposed)
                    AsistenciaUsuario._Instance = new AsistenciaUsuario();
                return AsistenciaUsuario._Instance;
            }
        }
        //
        public AsistenciaUsuario()
        {
            InitializeComponent();            
        }

        public void ActualizarListado()
        {
            configurarAsistencias();
            var fechaActual = UtilDatos.FechasDeComisiones(DateTime.Now);
            dtpFechaDesde.Value = fechaActual.Valor1;
            dtpFechaHasta.Value = fechaActual.Valor2;
            
            var oAsistencias = General.GetListOf<UsuariosAsistenciasView>(u=>u.FechaHora>=fechaActual.Valor1 && u.FechaHora<=fechaActual.Valor2).OrderBy(u => u.AccesoUsuarioID);
            foreach (var asistencia in oAsistencias)
            {
                var retardo = asistencia.FechaHora.TimeOfDay-new TimeSpan(09,00,00)<new TimeSpan(0)?new TimeSpan(0):asistencia.FechaHora.TimeOfDay-new TimeSpan(09,00,00);
                dgvAsistencias.Rows.Add(asistencia.NombreSucursal, asistencia.FechaHora, asistencia.NombreUsuario,retardo);
            }
        }

        
        private void btnMostrarAsistencias_Click(object sender, EventArgs e)
        {
            configurarAsistencias();
            var fechaInicio = Helper.InicioAbsoluto(dtpFechaDesde.Value);
            var fechaFin = Helper.FinAbsoluto(dtpFechaHasta.Value);
            var oAsistencias = General.GetListOf<UsuariosAsistenciasView>(u => u.FechaHora >= fechaInicio && u.FechaHora <= fechaFin).OrderBy(u => u.AccesoUsuarioID);
            foreach (var asistencia in oAsistencias)
            {
                var retardo = asistencia.FechaHora.TimeOfDay - new TimeSpan(09, 00, 00) < new TimeSpan(0) ? new TimeSpan(0) : asistencia.FechaHora.TimeOfDay - new TimeSpan(09, 00, 00);
                dgvAsistencias.Rows.Add(asistencia.NombreSucursal, asistencia.FechaHora, asistencia.NombreUsuario, retardo);
            }
        }

        private void configurarAsistencias()
        {
            dgvAsistencias.Columns.Clear();
            dgvAsistencias.Rows.Clear();
            dgvAsistencias.Columns.Add("Tienda", "Tienda");
            dgvAsistencias.Columns.Add("FechaHora", "Fecha/Hora");
            dgvAsistencias.Columns.Add("Usuario", "Usuario");
            dgvAsistencias.Columns.Add("Retardo", "Retardo");
            dgvAsistencias.Columns["Tienda"].Width = 120;
            dgvAsistencias.Columns["FechaHora"].Width = 200;
            dgvAsistencias.Columns["Usuario"].Width = 200;
            dgvAsistencias.Columns["Retardo"].Width = 120;

        }
        

    }


}
