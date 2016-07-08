using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class DetalleBusquedaDeClientes : Form
    {
        public bool Seleccionado = false;
        public int Sel;

        public static DetalleBusquedaDeClientes Instance
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

            internal static readonly DetalleBusquedaDeClientes instance = new DetalleBusquedaDeClientes();
        }

        public DetalleBusquedaDeClientes()
        {
            InitializeComponent();
            this.listadoSimple.dgvDatos.CellClick += new DataGridViewCellEventHandler(dgvDatos_CellClick);
        }

        private void DetalleBusquedaDeClientes_Load(object sender, EventArgs e)
        {
            try
            {
                var clientes = Datos.GetListOf<ClientesDatosView>(c => c.Nombre.Length > 2).ToList();                
                var t = new DataTable();
                t.Columns.Add("ClienteID", typeof(int));
                t.Columns.Add("Nombre", typeof(string));
                foreach (var cliente in clientes)
                    t.Rows.Add(cliente.ClienteID, cliente.Nombre);

                this.listadoSimple.lblTitulo.Text = "Clientes";
                this.listadoSimple.CargarDatos(t);
                Util.OcultarColumnas(this.listadoSimple.dgvDatos, new string[] { "ClienteID" });
                this.listadoSimple.dgvDatos.RowHeadersVisible = false;
                this.listadoSimple.dgvDatos.BackgroundColor = Color.FromArgb(188, 199, 216);
                this.listadoSimple.dgvDatos.GridColor = Color.FromArgb(188, 199, 216);
                this.listadoSimple.Focus();
            }
            catch (Exception ex)
            {
                Util.MensajeError(ex.Message, GlobalClass.NombreApp);
            }
        }

        private void dgvDatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null)
                return;
            if (e.RowIndex == -1)
                return;
            if (this.listadoSimple.dgvDatos.CurrentRow == null)
                return;
            Sel = Util.Entero(this.listadoSimple.dgvDatos.CurrentRow.Cells["ClienteID"].Value);
            Seleccionado = true;
            this.Close();
            //this.listadoSimple.();
        }

    }
}
