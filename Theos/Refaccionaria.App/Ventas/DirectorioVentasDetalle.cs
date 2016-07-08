using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class DirectorioVentasDetalle : UserControl
    {
        public enum eModo : byte { Proveedores = 0, Lineas, Marcas }
        public const string SubCarpeta = "attach\\";
        public int IdLinea { get; set; }
        public int IdMarca { get; set; }
        private DirectorioVentasDetalle.eModo cliente { get; set; }
        
        public DirectorioVentasDetalle()
        {
            InitializeComponent();
            LimpiaDetalle();
        }

        // ////////////////////////////////////

        private void DirectorioVentasDetalle_Load(object sender, EventArgs e)
        {
            this.pnlDetProveedor.Dock = DockStyle.Fill;
            this.pnlDetLinea.Dock = DockStyle.Fill;

            this.pnlDetLinea.Visible = true;
            this.pnlDetLinea.Visible = false;

            this.pnlDetLinea.Dock = DockStyle.Fill;
        }

        private void listArchivos_DoubleClick(object sender, EventArgs e)
        {
            if (this.listArchivos.SelectedItems[0].ImageKey == "nulo") return;

            string ruta = (cliente == eModo.Lineas) ? UtilLocal.RutaImagenesLineas() : UtilLocal.RutaImagenesMarcas();
            ruta += SubCarpeta;
            
            string target = ruta + this.listArchivos.SelectedItems[0].SubItems[3].Text;
            if (target.IndexOf("www.") > -1) target = this.listArchivos.SelectedItems[0].SubItems[3].Text;

            System.Diagnostics.Process.Start(target);
        }

        private void listArchivos_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            System.Diagnostics.Process.Start(target);
        }

        // ////////////////////////////////////////////////

        internal void AddFiles(int LineaMarcaId)
        {
            listArchivos.Items.Clear();

            if (cliente == eModo.Lineas)
            {
                this.IdLinea = LineaMarcaId;
                var Archivo = Datos.GetListOf<LineaAttachFile>(p => p.LineaID == LineaMarcaId).ToList();
                foreach (var oAttach in Archivo)
                {
                    //string archivo = oAttach.NombreArchivo;
                    string tipo = this.tipoArchivo(oAttach.NombreArchivo);
                    //archivo = (tipo == "nulo") ? "#" + archivo : archivo;
                    ListViewItem Objeto = new ListViewItem(new string[] { oAttach.NombreArchivo, oAttach.LineaID.ToString(), oAttach.Descripcion, oAttach.NombreArchivo }, tipo);
                    listArchivos.Items.Add(Objeto);
                }
            }
            else
            {
                this.IdMarca = LineaMarcaId;
                var Archivo = Datos.GetListOf<MarcaAttachFile>(p => p.MarcaID == LineaMarcaId).ToList();
                foreach (var oAttach in Archivo)
                {
                    string tipo = this.tipoArchivo(oAttach.NombreArchivo);
                    ListViewItem Objeto = new ListViewItem(new string[] { oAttach.NombreArchivo, oAttach.MarcaID.ToString(), oAttach.Descripcion, oAttach.NombreArchivo }, tipo);
                    listArchivos.Items.Add(Objeto);
                }
            }
        }

        private int ObtenerIdDeLista(ListViewItem Elemento)
        {
            return Util.Entero(Elemento.SubItems[0].Text);
        }

        public void MuestraPanel(DirectorioVentasDetalle.eModo visible)
        {
            this.cliente = visible;

            this.pnlDetProveedor.Visible = false;
            this.pnlDetLinea.Visible = false;

            this.lblDescripcion.Text = "";
            this.listArchivos.Items.Clear();

            if (visible == DirectorioVentasDetalle.eModo.Proveedores)
                this.pnlDetProveedor.Visible = true;
            else
                this.pnlDetLinea.Visible = true;
        }

        private string tipoArchivo(string NombreArchivo)
        {
            string ruta = (cliente == eModo.Lineas) ? UtilLocal.RutaImagenesLineas() : UtilLocal.RutaImagenesMarcas();
            ruta += SubCarpeta;
            string MiTipo = "nulo";


            if (NombreArchivo.IndexOf("www.") > -1) 
                MiTipo = "htm";
            else
                if (System.IO.File.Exists(ruta + NombreArchivo))
                {
                    MiTipo = NombreArchivo.Substring(NombreArchivo.LastIndexOf('.') + 1).ToLower();
                    if (MiTipo.IndexOf("xls") > -1) MiTipo = "xls";
                    else if (MiTipo.IndexOf("doc") > -1) MiTipo = "doc";
                    else if (MiTipo.IndexOf("ppt") > -1) MiTipo = "ppt";
                    else if (MiTipo.IndexOf("pps") > -1) MiTipo = "ppt";
                    else if (MiTipo.IndexOf("jpg") > -1) MiTipo = "jpg";
                    else if (MiTipo.IndexOf("jpeg") > -1) MiTipo = "jpg";
                    else if (MiTipo.IndexOf("png") > -1) MiTipo = "jpg";
                    else if (MiTipo.IndexOf("gif") > -1) MiTipo = "jpg";
                    else if (MiTipo.IndexOf("bmp") > -1) MiTipo = "jpg";
                    else if (MiTipo.IndexOf("pdf") > -1) MiTipo = "pdf";
                    else if (MiTipo.IndexOf("txt") > -1) MiTipo = "txt";
                }

            return MiTipo;
        }

        public void llenaCampos(int IdProveedor, string NombreProveedor)
        {
            if (IdProveedor <= 0)
                LimpiaDetalle();
            else
            {
                string strLogo = "(" + IdProveedor.ToString() + ").jpg";
                strLogo = UtilLocal.RutaImagenesProveedores() + strLogo;
                ControlPicture.CargaImagen(strLogo, this.picLogo);

                //todo: Cargar proveedores
                this.lblNombre.Text = NombreProveedor;
                listContactos.Items.Clear();
                this.txObserva.Text = "";

                // Busca los contactos del proveedor por Id
                var oProvee = Datos.GetListOf<ProveedorContactosView>(p => p.ProveedorID == IdProveedor);
                foreach (var items in oProvee)
                {
                    string ext = (items.TelExt.Trim() == "")? "": " Ext: " + items.TelExt;
                    listContactos.Items.Add(new ListViewItem(new string[]{items.Contacto, items.TelOficina + ext,items.TelParticular, items.Celular, items.CorreoElectronico}));
                }

                // Busca las observaciones del proveedor por Id
                var Obs = Datos.GetEntity<ProveedorObservacion>(p => p.ProveedorID == IdProveedor);
                if (Obs != null) this.txObserva.Text = Obs.Observacion;
            }
        }

        private void LimpiaDetalle()
        {
            foreach (Control elemento in this.Controls)
                if (elemento is TextBox) elemento.Text = "";
            this.listArchivos.LargeImageList = this.ilArchivos;
            this.listArchivos.Items.Clear();
        }

        private void listArchivos_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int indice = e.Item.Index;
            string Descripcion = this.listArchivos.Items[indice].SubItems[2].Text;   //this.listArchivos.SelectedItems[0].SubItems[2].Text;
            this.lblDescripcion.Text = Descripcion;
        }
    }
}