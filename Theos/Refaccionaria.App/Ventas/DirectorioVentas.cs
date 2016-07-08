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
    public partial class DirectorioVentas : UserControl
    {
        /////////////////////////////////////////////////////////////////////////////////////
        /// CONFIGURABLES
        /////////////////////////////////////////////////////////////////////////////////////

        bool SinLogoEnG = false;    // En true: Utiliza la G de Garibaldi cuando el proveedor no tiene Imágen


        //Variables para el manejo de búsqueda por páginas
        private int ItemsPorPagina { get; set; }   // Numero de Items que se muestran por página
        internal int ContadorPaginas { get; set; }  // Página que se esta visualizando
        internal bool UsaScroll { get; set; }
        
        public DirectorioVentasDetalle Detalles; // Panel de detalles del directorio
        public DirectorioVentasDetalle.eModo BuscarPor; // Indica el criterio de bpusqueda
        private bool Tarjetas;  // Vista de tarjetas o Lista

        // Filtros de Proveedor
        public int IdProveedor { get; set; } // Id de proveedor seleccionado
        public int IdPartSelect { get; set; }    // Id de parte seleccionada
        internal string FiltroProveedor { get; set; }
        internal string proveedorPorLineaMarca { get; set; }

        // Filtros de Linea
        public int IdLinea { get; set; }
        private int IdProveedorLinea { get; set; }
        internal string FiltroLinea { get; set; }
        internal string LineaPorProveedorMarca { get; set; }

        // Filtros de Marca
        public int IdMarca { get; set; }
        private int IdProveedorMarca { get; set; }
        internal string FiltroMarca { get; set; }
        internal string MarcaPorProveedorLinea { get; set; }
                
        // Previenen que las listas se rellenen cuando estan mostrando el total de registros y no se han hecho cambios
        private int TotalDeLineas = 0; 
        private int TotalDeProvee = 0;
        private int TotalDeMarcas = 0;

        // Contenedores de Imagen para las listas
        ImageList ImagenesLineas;
        ImageList ImagenesProvee;
        ImageList ImagenesMarca;

        public event EventHandler<EventArgs> WasChangeBuscarPor;
        private ScrollEventType mLastScroll = ScrollEventType.EndScroll;

        public DirectorioVentas(DirectorioVentasDetalle Detalles)
        {
            InitializeComponent();

            this.Detalles = Detalles;  // Panel de detalles (Parte Inferior de la pantalla)

            //búsqueda por páginas
            this.ItemsPorPagina = 30;
            this.ContadorPaginas = 0;
            this.UsaScroll = false;

            // Inicializa los contenedores de Imagen
            //Proveedores
            this.ImagenesProvee = new ImageList() { ImageSize = new Size(86, 62) };
            if (SinLogoEnG) this.ImagenesProvee.Images.Add("0", Properties.Resources.ParteSinImagen);

            //Líneas
            this.ImagenesLineas = new ImageList() { ImageSize = new Size(100, 75) };
            this.ImagenesLineas.Images.Add("0", Properties.Resources.ParteSinImagen);
            this.listLinea.LargeImageList = this.ImagenesLineas;

            //Marcas
            this.ImagenesMarca = new ImageList() { ImageSize = new Size(100, 75) };
            this.ImagenesMarca.Images.Add("0", Properties.Resources.ParteSinImagen);
            this.listMarca.LargeImageList = this.ImagenesMarca;

            // Marcadores de estado de los filtros
            this.FiltroProveedor = "";
            this.FiltroLinea = "";
            this.FiltroMarca = "";

            this.proveedorPorLineaMarca = "";
            this.LineaPorProveedorMarca = "";
            this.MarcaPorProveedorLinea = "";

            this.IdProveedor = 0;
            this.IdProveedorLinea = 0;
            this.IdProveedorMarca = 0;
            this.IdLinea = 0;
            this.IdMarca = 0;
            this.IdPartSelect = 0;
        }

        #region [Eventos]
        private void DirectorioVentas_Load(object sender, EventArgs e)
        {
            this.tjContactos.Dock = DockStyle.Fill;
            this.tjContactos.Detalles = this.listProvee;
            this.tjContactos.tabs = this.tabDirectorio;
            this.tjContactos.Accion = "";
            this.MostrarTarjetas(true);
        }

        private void tabDirectorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Control de tarjetas vinculado a lineas y marcas
            if (tjContactos.Accion != "")
            {
                string accion = tjContactos.Accion;
                tjContactos.Accion = "";
                if (accion == "linea")
                {
                    this.LineaPorProveedorMarca = "proveedor";
                    this.IdProveedorLinea = tjContactos.ProveedorId;
                    this.FiltroLinea = "#Proveedor: " + tjContactos.Proveedor;
                }
                else if (accion == "marca")
                {
                    this.MarcaPorProveedorLinea = "proveedor";
                    this.IdProveedorMarca = tjContactos.ProveedorId;
                    this.FiltroMarca = "#Proveedor: " + tjContactos.Proveedor;
                }
            }

            // Genera la llamada al evento WasChangeBuscarPor
            var WasCahnge = WasChangeBuscarPor;
            if (WasCahnge != null) WasChangeBuscarPor(sender, e);

            this.UsaScroll = false;

            // PROVEEDORES
            if (tabDirectorio.SelectedIndex == (int)DirectorioVentasDetalle.eModo.Proveedores)
            {
                this.BuscarPor = DirectorioVentasDetalle.eModo.Proveedores;
                // Filtra por Línea o por Marca
                if (this.proveedorPorLineaMarca != "")
                    this.cargaProveedorPorLineaMarca(this.proveedorPorLineaMarca);
                // Filtra por Id de proveedor seleccionado o Id Parte seleccionada
                else if (this.IdProveedor != 0)
                {
                    int IdProveedor = this.IdProveedor;
                    IdProveedor = (IdProveedor == 0) ? this.BuscaProveedorConIdParte(this.IdPartSelect) : IdProveedor;
                    this.cargaProveedorUnicoOPorParte(IdProveedor);
                }
                // Sin filtro ó Filtro por Nombre de proveedor
                else
                    this.cargaProveedorPorFiltro(this.FiltroProveedor);

                if (this.tjContactos.ItemTPSelected() <= 0) this.tjContactos.IndexTPSelect();
                if (this.listProvee.Items.Count > 0) this.listProvee.Items[0].Selected = true;
            }
            // LINEAS
            else if (tabDirectorio.SelectedIndex == (int)DirectorioVentasDetalle.eModo.Lineas)
            {
                this.BuscarPor = DirectorioVentasDetalle.eModo.Lineas;
                // Filtra por Proveedor o por Marca
                if (this.LineaPorProveedorMarca != "")
                    this.cargaLineaPorProveedorMarca(this.LineaPorProveedorMarca);
                // Filtra por Id Parte seleccionada
                else if (this.IdPartSelect != 0)
                    this.cargaLineaPorParte(this.IdPartSelect);
                // Sin filtro ó Filtro por Nombre de proveedor
                else
                    this.cargaLineaPorFiltro(this.FiltroLinea);
            }
            // MARCAS
            else
            {
                this.BuscarPor = DirectorioVentasDetalle.eModo.Marcas;
                // Filtra por Proveedor o por Marca
                if (this.MarcaPorProveedorLinea != "")
                    this.cargaMarcaPorProveedorLinea(this.MarcaPorProveedorLinea);
                // Filtra por Id Parte seleccionada
                else if (this.IdPartSelect != 0)
                    this.cargaMarcaPorParte(this.IdPartSelect);
                // Sin filtro ó Filtro por Nombre de proveedor
                else
                    this.cargaMarcaPorFiltro(this.FiltroMarca);
            }

            // Cambia el panel de detalle correspondiente a la selección
            this.Detalles.MuestraPanel(this.BuscarPor);
        }

        // Llena el la descipción del proveedor de acuerdo a la selección en el listProvee
        private void listProvee_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int index = e.Item.Index;
            if (index < 0) return;

            int IdProveedor = Convert.ToInt32(listProvee.Items[index].SubItems[0].Text);
            string Nombre = listProvee.Items[index].SubItems[1].Text;
            this.Detalles.llenaCampos(IdProveedor, Nombre);
        }

        private void listLinea_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int index = e.Item.Index;
            if (index < 0) return;

            int LineaId = this.IdLinea = Convert.ToInt32(listLinea.Items[index].SubItems[1].Text);
            Detalles.AddFiles(LineaId);
        }

        private void listLinea_DoubleClick(object sender, EventArgs e)
        {
            this.proveedorPorLineaMarca = "linea";
            this.FiltroProveedor = "#Línea: " + this.listLinea.SelectedItems[0].SubItems[0].Text;
            this.IdLinea = this.ObtenerIdDeLista(this.listLinea.SelectedItems[0]);
            MuestraTab(DirectorioVentasDetalle.eModo.Proveedores);
        }

        private void listLinea_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (this.listLinea.View == View.Details)
            {
                e.DrawDefault = true;
                if (e.Item.Selected)
                    e.Item.BackColor = SystemColors.Highlight;
                else
                {
                    e.Item.ForeColor = e.Item.ListView.ForeColor;
                    e.Item.BackColor = e.Item.ListView.BackColor;
                }
                return;
            }
            ControlPicture.DibujarSombraLista(this.ImagenesLineas, e);
        }

        private void listMarca_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int index = e.Item.Index;
            if (index < 0) return;

            int MarcaId = this.IdMarca = Convert.ToInt32(listMarca.Items[index].SubItems[1].Text);
            Detalles.AddFiles(MarcaId);
        }

        private void listMarca_DoubleClick(object sender, EventArgs e)
        {
            this.proveedorPorLineaMarca = "marca";
            this.FiltroProveedor = "#Marca: " + this.listMarca.SelectedItems[0].SubItems[0].Text;
            this.IdMarca = this.ObtenerIdDeLista(this.listMarca.SelectedItems[0]);

            MuestraTab(DirectorioVentasDetalle.eModo.Proveedores);
        }

        private void listMarca_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (this.listMarca.View == View.Details)
            {
                e.DrawDefault = true;
                if (e.Item.Selected)
                    e.Item.BackColor = SystemColors.Highlight;
                else
                {
                    e.Item.ForeColor = e.Item.ListView.ForeColor;
                    e.Item.BackColor = e.Item.ListView.BackColor;
                }
                return;
            }
            ControlPicture.DibujarSombraLista(this.ImagenesMarca, e);
        }

        private void listas_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listLinea_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && this.listLinea.SelectedIndices.Count > 0)
            {
                this.StripMenuItemLineas.Visible = false;
                this.StripMenuItemMarcas.Visible = true;

                Point p = new Point(e.X, e.Y);  // posición del puntero
                this.ctMnu.Show(listLinea, p);
            }
        }

        private void listMarca_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && this.listMarca.SelectedIndices.Count > 0)
            {
                this.StripMenuItemLineas.Visible = true;
                this.StripMenuItemMarcas.Visible = false;

                Point p = new Point(e.X, e.Y);  // posición del puntero
                this.ctMnu.Show(listMarca, p);
            }
        }

        private void StripMenuItemLineas_Click(object sender, EventArgs e)
        {
            this.LineaPorProveedorMarca = "marca";
            this.FiltroLinea = "#Marca: " + this.listMarca.SelectedItems[0].SubItems[0].Text;
            this.IdMarca = this.ObtenerIdDeLista(this.listMarca.SelectedItems[0]);

            MuestraTab(DirectorioVentasDetalle.eModo.Lineas);
        }

        private void StripMenuItemMarcas_Click(object sender, EventArgs e)
        {
            this.MarcaPorProveedorLinea = "linea";
            this.FiltroMarca = "#Línea: " + this.listLinea.SelectedItems[0].SubItems[0].Text;
            this.IdLinea = this.ObtenerIdDeLista(this.listLinea.SelectedItems[0]);

            MuestraTab(DirectorioVentasDetalle.eModo.Marcas);
        }

        private void StripMenuItemProveedores_Click(object sender, EventArgs e)
        {
            if (this.BuscarPor == DirectorioVentasDetalle.eModo.Lineas)
            {
                this.proveedorPorLineaMarca = "linea";
                this.FiltroProveedor = "#Línea: " + this.listLinea.SelectedItems[0].SubItems[0].Text;
                this.IdLinea = this.ObtenerIdDeLista(this.listLinea.SelectedItems[0]);
            }
            else
            {
                this.proveedorPorLineaMarca = "marca";
                this.FiltroProveedor = "#Marca: " + this.listMarca.SelectedItems[0].SubItems[0].Text;
                this.IdMarca = this.ObtenerIdDeLista(this.listMarca.SelectedItems[0]);
            }

            MuestraTab(DirectorioVentasDetalle.eModo.Proveedores);
        }

        #endregion

        #region [Métodos]

        // Forza la selección de una prestaña en el TabControl
        public void MuestraTab(DirectorioVentasDetalle.eModo visible, int IdSelected = 0)
        {
            this.BuscarPor = visible;
            if (IdSelected != 0) this.IdPartSelect = IdSelected;

            if (visible == DirectorioVentasDetalle.eModo.Proveedores)
            {
                //if (IdSelected != 0) this.IdProveedor = IdSelected;
                tabDirectorio.SelectedTab = tabDirectorio.TabPages[0];
            }
            else if (visible == DirectorioVentasDetalle.eModo.Lineas)
            {
                //if (IdSelected != 0) this.IdLinea = IdSelected;
                tabDirectorio.SelectedTab = tabDirectorio.TabPages[1];
            }
            else
            {
                //if (IdSelected != 0) this.IdMarca = IdSelected;
                tabDirectorio.SelectedTab = tabDirectorio.TabPages[2];
            }
        }

        public void primerSeleccionProvedor(int IdSelectedPart)
        {
            if (IdSelectedPart != 0)
            {
                int IdProveedor = this.BuscaProveedorConIdParte(IdSelectedPart, true);
                this.cargaProveedorUnicoOPorParte(IdProveedor);
            }
            else
                this.cargaProveedorPorFiltro("");

            if (this.tjContactos.ItemTPSelected() <= 0) this.tjContactos.IndexTPSelect();
            if (this.listProvee.Items.Count > 0) this.listProvee.Items[0].Selected = true;
        }

        // Swith entre Lista Detalle e Iconos
        internal void MostrarTarjetas(bool valor)
        {
            this.tjContactos.Visible = valor;
            this.listProvee.Visible = !valor;
            if (valor)
            {
                this.listLinea.View = View.LargeIcon;
                this.listMarca.View = View.LargeIcon;
            }
            else
            {
                this.listLinea.View = View.Details;
                this.listMarca.View = View.Details;
            }
            this.Tarjetas = valor;
        }

        // Provoca la actualización de datos dependiendo de la pestaña selecciónada
        internal void BusquedaFiltro(string Filtro)
        {
            if (BuscarPor == DirectorioVentasDetalle.eModo.Proveedores)
            {
                this.IdPartSelect = this.IdProveedor = 0;
                this.FiltroProveedor = Filtro;
                if (this.proveedorPorLineaMarca == "proveedor") this.proveedorPorLineaMarca = "";
                if (this.LineaPorProveedorMarca == "proveedor") this.LineaPorProveedorMarca = "";
                if (this.MarcaPorProveedorLinea == "proveedor") this.MarcaPorProveedorLinea = "";
                this.cargaProveedorPorFiltro(Filtro);
            }
            else if (BuscarPor == DirectorioVentasDetalle.eModo.Lineas)
            {
                this.IdLinea = 0;
                this.FiltroLinea = Filtro;
                if (this.proveedorPorLineaMarca == "linea") this.proveedorPorLineaMarca = "";
                if (this.LineaPorProveedorMarca == "linea") this.LineaPorProveedorMarca = "";
                if (this.MarcaPorProveedorLinea == "linea") this.MarcaPorProveedorLinea = "";
                this.cargaLineaPorFiltro(Filtro);
            }
            else
            {
                this.IdMarca = 0;
                this.FiltroMarca = Filtro;
                if (this.proveedorPorLineaMarca == "marca") this.proveedorPorLineaMarca = "";
                if (this.LineaPorProveedorMarca == "marca") this.LineaPorProveedorMarca = "";
                if (this.MarcaPorProveedorLinea == "marca") this.MarcaPorProveedorLinea = "";
                this.cargaMarcaPorFiltro(Filtro);
            }
        }

        // Resuelve el Id del elemento seleccionado tanto en Lista como Marca
        private int ObtenerIdDeLista(ListViewItem Elemento)
        {
            return Util.Entero(Elemento.SubItems[1].Text);
        }

        #endregion

        #region [ Filtro para Proveedor Linea Marca]

        ////////// // PROVEEDORES - MÉTODOS FLTROS - //////////
        // Devuelve el Id de proveedor mediante un IdParte
        internal int BuscaProveedorConIdParte(int IdParte = 0, bool TraeParte = false)
        {
            if (IdParte <= 0) return 0;

            this.ContadorPaginas = 0;
            this.IdPartSelect = IdParte;
            var oParte = Datos.GetEntity<Parte>(p => p.ParteID == IdParte);
            if (TraeParte) FiltrosConDescipParte(oParte.NombreParte);
            return oParte.ProveedorID;
        }

        //  Llena el listView y tjContactos de proveedores filtrado por Id de Parte
        private void cargaProveedorUnicoOPorParte(int IdProveedor)
        {
            this.listProvee.Items.Clear();
            this.tjContactos.ClearTPresenta();
            this.ContadorPaginas = 0;
            this.IdProveedor = IdProveedor;

            var Pro = Datos.GetEntity<ProveedorContactoPrincipalView>(p => p.ProveedorID == IdProveedor);

            string Imagen = Pro.ProveedorID.ToString();
            string Direccion = Pro.Direccion + ", " + Pro.Ciudad + ", C.P. " + Pro.CP + ", " + Pro.Estado;
            var iLst = FilaProveedor(Pro.ProveedorID.ToString(), Pro.NombreProveedor, Pro.NombreContacto, Pro.ProveedorContactoID.ToString(), Imagen, Pro.TelProCUno, Pro.TelProCDos, Pro.TelProCTres, Pro.TelProCCuatro, Pro.TelProUno, Pro.TelProDos, Pro.TelProTres, Direccion, Pro.PaginaWeb);
            this.listProvee.Items.Add(iLst);

            if (this.IdPartSelect > 0)
            {
                // Para mostrar las partes equivalentes
                var PartesEq = Datos.GetListOf<ProveedorEquivalContactoPpalView>(q => q.ParteIDEquivalente == this.IdPartSelect);
                foreach (var ProEq in PartesEq)
                {
                    if (IdProveedor != ProEq.ProveedorID)
                    {
                        Imagen = ProEq.ProveedorID.ToString();
                        Direccion = ProEq.Direccion + ", " + ProEq.Ciudad + ", C.P. " + ProEq.CP + ", " + ProEq.Estado;
                        var EqLst = FilaProveedor(ProEq.ProveedorID.ToString(), ProEq.NombreProveedor, ProEq.NombreContacto, ProEq.ProveedorContactoID.ToString(), Imagen, ProEq.TelProCUno, ProEq.TelProCDos, ProEq.TelProCTres, ProEq.TelProCCuatro, ProEq.TelProUno, ProEq.TelProDos, ProEq.TelProTres, Direccion, ProEq.PaginaWeb, true);
                        this.listProvee.Items.Add(EqLst);
                    }
                }
            }
        }

        // Lena el listView de proveedores filtrado por Linea o Marca
        private void cargaProveedorPorLineaMarca(string PorLineaMarca)
        {
            this.listProvee.Items.Clear();
            this.tjContactos.ClearTPresenta();
            this.ContadorPaginas = 0;

            // Filtro por linea
            if (this.proveedorPorLineaMarca == "linea")
            {
                var oProveeLinea = Datos.GetListOf<ProveedorLineaView>(p => p.LineaID == this.IdLinea).ToList();
                foreach (var ProLin in oProveeLinea)
                {
                    var Pro = Datos.GetEntity<ProveedorContactoPrincipalView>(p => p.ProveedorID == ProLin.ProveedorID);
                    string Imagen = Pro.ProveedorID.ToString();
                    string Direccion = Pro.Direccion + ", " + Pro.Ciudad + ", C.P. " + Pro.CP + ", " + Pro.Estado;
                    this.listProvee.Items.Add(FilaProveedor(Pro.ProveedorID.ToString(), Pro.NombreProveedor, Pro.NombreContacto, Pro.ProveedorContactoID.ToString(), Imagen, Pro.TelProCUno, Pro.TelProCDos, Pro.TelProCTres, Pro.TelProCCuatro, Pro.TelProUno, Pro.TelProDos, Pro.TelProTres, Direccion, Pro.PaginaWeb));
                }
            }
            else
            {
                var oProveeLinea = Datos.GetListOf<ProveedorMarcaParteView>(p => p.MarcaParteID == this.IdMarca).Distinct().ToList();
                foreach (var ProLin in oProveeLinea)
                {
                    var Pro = Datos.GetEntity<ProveedorContactoPrincipalView>(p => p.ProveedorID == ProLin.ProveedorID);
                    string Imagen = Pro.ProveedorID.ToString();
                    string Direccion = Pro.Direccion + ", " + Pro.Ciudad + ", C.P. " + Pro.CP + ", " + Pro.Estado;
                    this.listProvee.Items.Add(FilaProveedor(Pro.ProveedorID.ToString(), Pro.NombreProveedor, Pro.NombreContacto, Pro.ProveedorContactoID.ToString(), Imagen, Pro.TelProCUno, Pro.TelProCDos, Pro.TelProCTres, Pro.TelProCCuatro, Pro.TelProUno, Pro.TelProDos, Pro.TelProTres, Direccion, Pro.PaginaWeb));
                }
            }

        }

        // Llena el listView de proveedores filtrando texto
        internal void cargaProveedorPorFiltro(string Filtro)
        {
            if (this.TotalDeProvee != listProvee.Items.Count || this.TotalDeProvee == 0 || Filtro != "" || this.ItemsPorPagina == listProvee.Items.Count)
            {
                List<ProveedorContactoPrincipalView> oProveedor;
                this.UsaScroll = true;

                if (this.ContadorPaginas == 0)
                {
                    this.tjContactos.ClearTPresenta();
                    this.listProvee.Items.Clear();
                    if (Filtro.Length == 0 && this.TotalDeProvee == 0)
                    {
                        oProveedor = Datos.GetListOf<ProveedorContactoPrincipalView>(p => p.NombreProveedor.Contains(Filtro)).ToList();
                        this.TotalDeProvee = oProveedor.Count();
                    }
                }

                if (tjContactos.Visible == true)
                {
                    //búsqueda por páginas
                    int Inicia = this.ContadorPaginas * this.ItemsPorPagina;
                    oProveedor = Datos.GetListOf<ProveedorContactoPrincipalView>(p => p.NombreProveedor.Contains(Filtro)).Skip(Inicia).Take(this.ItemsPorPagina).ToList();
                    ++this.ContadorPaginas;

                    foreach (var Pro in oProveedor)
                    {
                        string Imagen = Pro.ProveedorID.ToString();
                        string Direccion = Pro.Direccion + ", " + Pro.Ciudad + ", C.P. " + Pro.CP + ", " + Pro.Estado;
                        var iLst = FilaProveedor(Pro.ProveedorID.ToString(), Pro.NombreProveedor, Pro.NombreContacto, Pro.ProveedorContactoID.ToString(), Imagen, Pro.TelProCUno, Pro.TelProCDos, Pro.TelProCTres, Pro.TelProCCuatro, Pro.TelProUno, Pro.TelProDos, Pro.TelProTres, Direccion, Pro.PaginaWeb);
                        this.listProvee.Items.Add(iLst);
                    }
                }
                else
                {
                    this.listProvee.Items.Clear();
                    oProveedor = Datos.GetListOf<ProveedorContactoPrincipalView>(p => p.NombreProveedor.Contains(Filtro)).ToList();

                    foreach (var Pro in oProveedor)
                    {
                        string Imagen = Pro.ProveedorID.ToString();
                        string Direccion = Pro.Direccion + ", " + Pro.Ciudad + ", C.P. " + Pro.CP + ", " + Pro.Estado;
                        var iLst = FilaProveedor(Pro.ProveedorID.ToString(), Pro.NombreProveedor, Pro.NombreContacto, Pro.ProveedorContactoID.ToString(), Imagen, Pro.TelProCUno, Pro.TelProCDos, Pro.TelProCTres, Pro.TelProCCuatro, Pro.TelProUno, Pro.TelProDos, Pro.TelProTres, Direccion, Pro.PaginaWeb, false, true);
                        this.listProvee.Items.Add(iLst);
                    }
                }
            }
        }

        // Texto para identificar el filtro por parte seleccionada
        private void FiltrosConDescipParte(string NombreParteFiltro)
        {
            NombreParteFiltro = "#Parte: " + NombreParteFiltro;
            this.FiltroProveedor = this.FiltroLinea = this.FiltroMarca = NombreParteFiltro;
        }

        ////////// // LINEAS - MÉTODOS FLTROS - //////////

        //  Llena el listView de lineas filtrado por Id de Parte
        private int BuscaLineasConIdParte(int IdParte = 0)
        {
            return 0;
        }
        
        private void cargaLineaPorParte(int IdParte)
        {
            listLinea.Items.Clear();
            Parte oParte = Datos.GetEntity<Parte>(p => p.ParteID == IdParte);
            var oLinea = Datos.GetListOf<LineasView>(l => l.LineaID == oParte.LineaID).ToList();
            foreach (var item in oLinea)
                this.listLinea.Items.Add(CargaLineas(item.LineaID.ToString(), item.NombreLinea));
        }

        // Lena el listView de proveedores filtrado por Linea o Marca
        private void cargaLineaPorProveedorMarca(string PorProveedorMarca)
        {
            listLinea.Items.Clear();
            // Solo mestr las lineas del proveedor seleccionado
            if (PorProveedorMarca == "proveedor")
            {
                var oLinea = Datos.GetListOf<ProveedorLineaView>(l => l.ProveedorID == this.IdProveedorLinea).ToList();
                foreach (var item in oLinea)
                    this.listLinea.Items.Add(CargaLineas(item.LineaID.ToString(), item.NombreLinea));
            }
            // Solo muestra las lineas de la marca seleccionada
            else 
            {
                var oLinea = Datos.GetListOf<LineaMarcaPartesView>(l => l.MarcaParteID == this.IdMarca).ToList();
                foreach (var item in oLinea)
                    this.listLinea.Items.Add(CargaLineas(item.LineaID  .ToString(), item.NombreLinea));
            }
        }

        // Llena el listView de lineas filtrando texto
        private void cargaLineaPorFiltro(string Filtro)
        {
            if (this.TotalDeLineas != listProvee.Items.Count || this.TotalDeLineas == 0 || Filtro != "")
            {
                this.listLinea.Items.Clear();

                var oLineas = Datos.GetListOf<LineasView>(p => p.NombreLinea.Contains(Filtro)).ToList();
                foreach (var Lin in oLineas)
                    this.listLinea.Items.Add(CargaLineas(Lin.LineaID.ToString(), Lin.NombreLinea));

                if (Filtro.Length == 0) this.TotalDeLineas = this.listLinea.Items.Count;
            }
        }

        ////////// // MARCAS - MÉTODOS FLTROS - //////////

        // Lena
        private void cargaMarcaPorParte(int IdParte)
        {
            listMarca.Items.Clear();
            Parte oParte = Datos.GetEntity<Parte>(p => p.ParteID == IdParte);
            var oMarcas = Datos.GetListOf<MarcaPartesView>(l => l.MarcaParteID == oParte.MarcaParteID).ToList();
            foreach (var Mar in oMarcas)
                this.listMarca.Items.Add(CargaMarcas(Mar.MarcaParteID.ToString(), Mar.NombreMarcaParte));
        }

        // Llena 
        private void cargaMarcaPorProveedorLinea(string PorProveedorLinea)
        {
            listMarca.Items.Clear();
            // Solo mestr las Marcas del proveedor seleccionado
            if (PorProveedorLinea == "proveedor")
            {
                var oMarca = Datos.GetListOf<ProveedorMarcaParteView>(l => l.ProveedorID == this.IdProveedorMarca).Distinct().ToList();
                foreach (var item in oMarca)
                    this.listMarca.Items.Add(CargaMarcas(item.MarcaParteID.ToString(), item.NombreMarcaParte));
            }
            // Solo muestra las Marcas de la marca seleccionada
            else
            {
                var oMarca = Datos.GetListOf<LineaMarcaPartesView>(l => l.LineaID == this.IdLinea).ToList();
                foreach (var item in oMarca)
                    this.listMarca.Items.Add(CargaMarcas(item.MarcaParteID.ToString(), item.NombreMarcaParte));
            }
        }

        // Llena el listView de Marcas filtrando texto
        private void cargaMarcaPorFiltro(string Filtro)
        {
            if (this.TotalDeMarcas != listProvee.Items.Count || this.TotalDeMarcas == 0 || Filtro != "")
            {
                this.listMarca.Items.Clear();

                var oMarcas = Datos.GetListOf<MarcaPartesView>(p => p.NombreMarcaParte.Contains(Filtro)).ToList();
                foreach (var Mar in oMarcas)
                    this.listMarca.Items.Add(CargaMarcas(Mar.MarcaParteID.ToString(), Mar.NombreMarcaParte));

                if (Filtro.Length == 0) this.TotalDeMarcas = this.listMarca.Items.Count;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Carga los datos para los Item de los Proveedores, Lineas y Marcas
        ////////////////////////////////////////////////////////////////////////////////
        
        ////////// // PROVEEDORES - SUB FUNCIONES
        private void CargaProveedores(string IdProveedor, string Proveedor, string Nombre, string IdContacto, string TelUno, string TelDos, string TelTres, string Direccion, string PaginaWeb, bool generico = false)
        {
            string sLlaveImg = IdProveedor;

            string sRutaImagen = UtilLocal.RutaImagenesProveedores();
            sLlaveImg = ControlPicture.ImagenEnLista(this.ImagenesProvee, sLlaveImg, "(" + sLlaveImg + ").jpg", sRutaImagen);

            tjPresentaItem ItemtjContacto = this.tjContactos.AddTPresenta();
            ItemtjContacto.IdProvee = Convert.ToInt32(IdProveedor);
            ItemtjContacto.Titulo(Proveedor);
            ItemtjContacto.logo(this.ImagenesProvee.Images[sLlaveImg], sLlaveImg);
            ItemtjContacto.DatosContacto(Nombre, TelUno, TelDos, TelTres, Direccion, PaginaWeb, generico);
        }

        // estructura el contenido de las tarjetas y la líneas del listview
        private ListViewItem FilaProveedor(string Id, string wProveedor, string wNombreContacto, string wIdContacto, string wImagen, string wTC1, string wTC2, string wTC3, string wTC4, string wT1, string wT2, string wT3, string Direccion, string PaginaWeb, bool generico = false, bool evitarTj = false)
        {
            string Contacto, IdContacto, TelUno, TelDos, TelTres;

            if (wNombreContacto != null)
            {
                Contacto = wNombreContacto;
                IdContacto = wIdContacto;
                TelUno = "Part: " + wTC2;
                TelDos = "Cel: " + wTC4;
                wT3 = (wTC3 == "") ? "" : " Ext: " + wT3;
                TelTres = " Ofna: " + wTC1 + wTC3;
            }
            else
            {
                Contacto = "Contacto general";
                IdContacto = "0";
                TelUno = "Ofna: " + wT1;
                TelDos = "Fax: " + wT2;
                TelTres = "Lada 800: " + wT3;
            }

            ListViewItem iList = new ListViewItem(new string[] { 
                Id, wProveedor,
                IdContacto, wImagen, Contacto,
                TelUno, TelDos, TelTres, PaginaWeb });

            if (!evitarTj)
                CargaProveedores(Id, wProveedor, Contacto, IdContacto, TelUno, TelDos, TelTres, Direccion, PaginaWeb, generico);

            return iList;
        }

        // Si hay datos muestra los detalles del primer registro encontrado
        private void DetallElPrimero()
        {
            if (this.listProvee.Items.Count == 0)
            {
                this.Detalles.llenaCampos(0, "");
                this.lblMensajeBusqueda.Visible = true;
            }
            else
            {
                this.listProvee.Items[0].Selected = true;
                this.lblMensajeBusqueda.Visible = false;
            }
        }

        ////////// // LINEAS - SUB FUNCIONES
        private ListViewItem CargaLineas(string LineaId, string NombreLinea)
        {
            string sLlaveImg = LineaId, sRutaImagen;

            sRutaImagen = UtilLocal.RutaImagenesLineas();
            sLlaveImg = ControlPicture.ImagenEnLista(this.ImagenesLineas, sLlaveImg, "(" + sLlaveImg + ").jpg", sRutaImagen);

            ListViewItem oElemento = new ListViewItem(new string[] { NombreLinea, LineaId }, sLlaveImg);
            return oElemento;
        }

        ////////// // MARCAS - SUB FUNCIONES
        private ListViewItem CargaMarcas(string MarcaId, string NombreMarca)
        {
            string sLlaveImg = MarcaId, sRutaImagen;

            sRutaImagen = UtilLocal.RutaImagenesMarcas();
            sLlaveImg = ControlPicture.ImagenEnLista(this.ImagenesMarca, sLlaveImg, "(" + sLlaveImg + ").jpg", sRutaImagen);

            ListViewItem oElemento = new ListViewItem(new string[] { NombreMarca, MarcaId }, sLlaveImg);
            return oElemento;
        }

        #endregion

        private void tjContactos_Scroll(object sender, ScrollEventArgs e)
        {
            //MessageBox.Show(ScrollEventType.ThumbPosition.ToString());

            if (e.Type == ScrollEventType.EndScroll && this.UsaScroll == true)
                this.cargaProveedorPorFiltro(this.FiltroProveedor);
            mLastScroll = e.Type;
        }
    }


    class Tarjetas : tjPresenta
    {

        public ListView Detalles { get; set; }  // this.tjContactos.Detalles
        public TabControl tabs { get; set; }
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; }
        public string Accion { get; set; }

        public event ScrollEventHandler Scroll;

        public override void wasCliked(object sender)
        {
            int NuevoIndex = this.ItemTPSelected();
             this.Detalles.Items[--NuevoIndex].Selected = true;

            // Obtiene el Id del proveedor correspondiente a la tarjeta
            int IdProveedor = this.IdMaestro(NuevoIndex);
            // Obtiene el Nombre del proveedor correspondiente a la tarjeta
            this.Proveedor = this.NombreProveedor(NuevoIndex);

            // Acciona el tabcontrol con base en el nombre del objeto clickeado
            string ObjClickeado = ((tjPresentaItem)sender).ClickSobre();
            if (ObjClickeado == "lnkLine")
            {
                this.ProveedorId = IdProveedor;
                this.Accion = "linea";
                tabs.SelectedTab = tabs.TabPages[1];
            }
            else if (ObjClickeado == "lnkMarca")
            {
                this.ProveedorId = IdProveedor;
                this.Accion = "marca";
                tabs.SelectedTab = tabs.TabPages[2];
            }
            else
                this.Accion = "";
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            ScrollEventHandler handler = this.Scroll;
            if (handler != null) handler(this, e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x115)
            { // Trap WM_VSCROLL
                OnScroll(new ScrollEventArgs((ScrollEventType)(m.WParam.ToInt32() & 0xffff), 0));
            }
        }

    }
}