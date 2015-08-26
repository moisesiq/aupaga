using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Refaccionaria.Negocio;
using Refaccionaria.Modelo;

namespace Refaccionaria.App
{
    public partial class DetalleMostrarFirmas : Form
    {
        int iClienteID;

        public DetalleMostrarFirmas()
        {
            InitializeComponent();
        }

        public DetalleMostrarFirmas(int ClienteID)
        {
            InitializeComponent();

            this.iClienteID = ClienteID;
        }
        
        #region[ Eventos ]

        private void DetalleMostrarFirmas_Load(object sender, EventArgs e)
        {
            var personales = General.GetListOf<ClientePersonal>(c => c.ClienteID == iClienteID && c.Estatus);
            this.dgvPersonal.Columns.Add("ClientePersonalID", "ClientePersonalID");
            this.dgvPersonal.Columns.Add("NombrePersonal", "Nombre");
            this.dgvPersonal.Columns.Add("CorreoElectronico", "Correo");
            foreach (var personal in personales)
            {
                this.dgvPersonal.Rows.Add(personal.ClientePersonalID, personal.NombrePersonal, personal.CorreoElectronico);
            }
            this.dgvPersonal.Columns["ClientePersonalID"].Visible = false;
            this.dgvPersonal.DefaultCellStyle.ForeColor = Color.Black;

            if (this.dgvPersonal.Rows.Count > 0)
            {
                this.dgvPersonal.CurrentCell = this.dgvPersonal["NombrePersonal", 0];
                this.dgvPersonal_Click(sender, e);
            }
        }

        private void dgvPersonal_Click(object sender, EventArgs e)
        {
            int iClientePersonalID = Helper.ConvertirEntero(this.dgvPersonal.CurrentRow.Cells["ClientePersonalID"].Value);
            try
            {
                picBoxImagen.Image = new Bitmap(UtilLocal.RutaImagenClientePersonalFirma(iClientePersonalID));
            }
            catch (Exception)
            {
                picBoxImagen.Image = null;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
        
        #region [ Metodos]



        #endregion

    }
}
