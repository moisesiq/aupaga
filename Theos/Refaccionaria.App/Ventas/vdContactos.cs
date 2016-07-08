using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;


namespace Refaccionaria.App
{
    public partial class vdContactos : UserControl
    {
        public vdContactos(int Id)
        {
            InitializeComponent();

            var oContactos = Datos.GetListOf<ProveedorContactosView>(p => p.ProveedorID == Id);
            foreach(var items in oContactos) 
            {
                var oElemento = new ListViewItem(new string[] {
                    items.Contacto,
                    items.Departamento,
                    items.TelParticular,
                    items.Celular,
                    items.TelOficina,
                    items.TelExt,
                    items.CorreoElectronico
                });
                
                this.listContactos.Items.Add(oElemento);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
