using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class DetalleBase : Form
    {
        public DetalleBase()
        {
            InitializeComponent();
        }

        protected virtual void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        protected virtual void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
