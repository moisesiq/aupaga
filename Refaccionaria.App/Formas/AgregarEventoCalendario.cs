using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Modelo;
using Refaccionaria.Negocio;
namespace Refaccionaria.App
{
    public partial class AgregarEventoCalendario : Form
    {
        
        Cliente cliente;
        Proveedor prov;
        DateTime dt;
        string nombre;
        public AgregarEventoCalendario(Cliente cliente,DateTime dt)
        {
            InitializeComponent();
            this.cliente = cliente;
            this.dt = dt;
            this.nombre = cliente.Nombre;
            cargaInicial();
        }

        public AgregarEventoCalendario(Proveedor prov, DateTime dt)
        {
            InitializeComponent();
            this.prov = prov;
            this.dt = dt;
            this.nombre = prov.NombreProveedor;
            cargaInicial();
        }

        private void cargaInicial()
        {
            this.tbNombre.Text = this.nombre;
            
            //Cargar dia y hora del evento
            this.dtpFechaEvento.Value = dt;
            this.dtpHoraCobro.Value = dt;

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (cliente != null)
            {
                var Ec = new ClienteEventoCalendario()
                {
                    ClienteID = cliente.ClienteID,
                    Fecha = (this.dtpFechaEvento.Value.Date.Add(this.dtpHoraCobro.Value.TimeOfDay)),
                    Evento = this.tbDescripcion.Text
                };

                Guardar.Generico<ClienteEventoCalendario>(Ec);
            }
            else if (prov != null)
            {
                var Ec = new ProveedorEventoCalendario()
                {
                    ProveedorID = prov.ProveedorID,
                    Fecha = this.dtpFechaEvento.Value.Add(this.dtpHoraCobro.Value.TimeOfDay),
                    Evento = this.tbDescripcion.Text
                };
                Guardar.Generico<ProveedorEventoCalendario>(Ec);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
