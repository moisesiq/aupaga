using System;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Drawing;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CargaImgMarcaLinea : UserControl
    {
        int IdLineaMarca { get; set; }
        bool EsMarca { get; set; }
        int AttachId = 0;
        int index = 0;
        private const string SubCarpeta = "attach\\";

        public CargaImgMarcaLinea(int IdLineaMarca, bool EsMarca)
        {
            InitializeComponent();
            this.IdLineaMarca = IdLineaMarca;
            this.EsMarca = EsMarca;

            this.limpiar();
        }

        public void limpiar()
        {
            listView1.Items.Clear();
            this.index = -1;
            this.AttachId = 0;
            this.lblArchivo.Text = "";
            this.txtRuta.Text = "";
            this.txtDescripcion.Text = "";
            this.txtRuta.Enabled = false;

            this.CargaLineasMarca();
        }

        private void CargaLineasMarca()
        {
            if (this.EsMarca)
            {
                var lins = General.GetListOf<MarcaAttachFile>(p => p.MarcaID == this.IdLineaMarca);
                foreach (var items in lins)
                    listView1.Items.Add(new ListViewItem(new string[] { items.MarcaAttachFileID.ToString(), items.NombreArchivo, items.Descripcion }));
            }
            else
            {
                var lins = General.GetListOf<LineaAttachFile>(p => p.LineaID == this.IdLineaMarca);
                foreach (var items in lins)
                    listView1.Items.Add(new ListViewItem(new string[] { items.LineaAttachFileID.ToString(), items.NombreArchivo, items.Descripcion }));
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            FaltanDatos.SetError(btnImportar, string.Empty);
            txtRuta.Text = "";
            txtRuta.Enabled = false;
            string carpeta = (this.EsMarca) ? UtilLocal.RutaImagenesMarcas() : UtilLocal.RutaImagenesLineas();
            lblArchivo.Text = BuscarGuardarArchivo(carpeta + SubCarpeta);
        }

        private void btnImportar_Click_1(object sender, EventArgs e)
        {
            if (this.txtRuta.Text + lblArchivo.Text == "")
            {
                FaltanDatos.SetError(btnImportar, "Es requerido el nombre de un archivo o vínculo web");
                return;
            }
            else if (this.txtDescripcion.Text == "")
            {
                FaltanDatos.SetError(btnImportar, "Es requerido la descripción del archivo");
                return;
            }

            string NombreArchivo;

            if (txtRuta.Text != "")
            {
                if (txtRuta.Text.LastIndexOf("www.") != -1)
                    txtRuta.Text = txtRuta.Text.Substring(txtRuta.Text.LastIndexOf("www.") + 4).ToLower();

                NombreArchivo = "www." + txtRuta.Text;
            }
            else
                NombreArchivo = lblArchivo.Text;

            if (this.EsMarca)
            {
                if (this.AttachId != 0)
                {  // Actualiza
                    var Registro = General.GetEntity<MarcaAttachFile>(l => l.MarcaAttachFileID == this.AttachId);
                    Registro.MarcaID = this.IdLineaMarca;
                    Registro.NombreArchivo = NombreArchivo;
                    Registro.Descripcion = this.txtDescripcion.Text;
                    General.SaveOrUpdate<MarcaAttachFile>(Registro, Registro);
                }
                else
                {  // Nuevo
                    MarcaAttachFile Registro = new MarcaAttachFile();
                    Registro.MarcaID = this.IdLineaMarca;
                    Registro.NombreArchivo = NombreArchivo;
                    Registro.Descripcion = this.txtDescripcion.Text;

                    Guardar.Generico<MarcaAttachFile>(Registro);
                }
            }
            else
            {
                if (this.AttachId != 0)
                {  // Actualiza
                    var Registro = General.GetEntity<LineaAttachFile>(l => l.LineaAttachFileID == this.AttachId);
                        Registro.LineaID = this.IdLineaMarca;
                        Registro.NombreArchivo = NombreArchivo;
                        Registro.Descripcion = this.txtDescripcion.Text;
                    General.SaveOrUpdate<LineaAttachFile>(Registro, Registro);
                }
                else
                {  // Nuevo
                    LineaAttachFile Registro = new LineaAttachFile();
                    Registro.LineaID = this.IdLineaMarca;
                    Registro.NombreArchivo = NombreArchivo;
                    Registro.Descripcion = this.txtDescripcion.Text;

                    Guardar.Generico<LineaAttachFile>(Registro);
                }
            }

            this.limpiar();

            pEdicion.Visible = false;
            pMostrar.Visible = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblArchivo.Text = "";
            txtRuta.Enabled = true;
            txtRuta.Focus();
        }

        private string BuscarGuardarArchivo(string Carpeta)
        {
            String Archivo = "";
            string NombreArchivo = "";

            var fdAttach = new OpenFileDialog();
            fdAttach.Filter = "Tipos de archivo|*.doc?;*.xls?;*.ppt?;*.pps?;*.pdf;*.txt;*.jpg;*.jpeg;*.gif;*.png;*.bmp;";
            fdAttach.Title = "Ruta del archivo a importar";
            if (fdAttach.ShowDialog(Principal.Instance) == DialogResult.OK)
                Archivo = fdAttach.FileName;
            fdAttach.Dispose();

            if (Archivo != "")
            {
                // string Ruta = UtilLocal.RutaImagenes();
                NombreArchivo = fdAttach.FileName;
                NombreArchivo = NombreArchivo.Substring(NombreArchivo.LastIndexOf("\\") + 1).ToLower();
                if (NombreArchivo.Length > 50)
                {
                    string ext = NombreArchivo.Substring(NombreArchivo.LastIndexOf("."));
                    string nombre = NombreArchivo.Substring(0,46);
                    NombreArchivo = nombre + ext;
                }

                string Ruta = Carpeta + NombreArchivo;

                System.IO.File.Copy(fdAttach.FileName, Ruta, true);
            }

            return NombreArchivo;
        }

        private void txtRuta_TextChanged(object sender, EventArgs e)
        {
            FaltanDatos.SetError(btnImportar, string.Empty);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.index = e.Item.Index;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            this.AttachId = Convert.ToInt32(listView1.Items[index].SubItems[0].Text);
            
            string NombreArchivo = listView1.Items[index].SubItems[1].Text;

            if (NombreArchivo.LastIndexOf("www.") != -1)
                txtRuta.Text = NombreArchivo.Substring(NombreArchivo.LastIndexOf("www.") + 4).ToLower();
            else
                lblArchivo.Text = NombreArchivo;

            this.txtDescripcion.Text = listView1.Items[index].SubItems[2].Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.limpiar();
            pEdicion.Visible = true;
            pMostrar.Visible = false;
            btnImportar.Text = "Agregar";
        }

        private void CargaImgMarcaLinea_Load(object sender, EventArgs e)
        {
            pEdicion.Location = pMostrar.Location = new System.Drawing.Point(5, 5);

            pEdicion.Visible = false;
            pMostrar.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.limpiar();
            pEdicion.Visible = false;
            pMostrar.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.index < 0) return;
            pEdicion.Visible = true;
            pMostrar.Visible = false;
            btnImportar.Text = "Modificar";
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            FaltanDatos.SetError(btnImportar, string.Empty);
        }

    }
}
