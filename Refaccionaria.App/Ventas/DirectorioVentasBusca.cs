using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class DirectorioVentasBusca : UserControl
    {
        
        private DirectorioVentas Directorio;
        
        // Para la búsqueda avanzada
        private int BusquedaRetrasoTecla = 400;
        private int BusquedaLlamada = 0;
        private int BusquedaIntento = 0;

        // Criterio de Búsqueda
        private DirectorioVentasDetalle.eModo BuscarPor;

        public DirectorioVentasBusca(DirectorioVentas Directorio)
        {
            InitializeComponent();
            this.Directorio = Directorio;  // Panel de Contenido
            this.BuscarPor = DirectorioVentasDetalle.eModo.Proveedores;
            this.txFinder.Text = "";

            this.Directorio.WasChangeBuscarPor += this.EtiquetaBuscarPor_Cambio;
        }

        #region [Eventos]
        
        private void txFinder_TextChanged(object sender, EventArgs e)
        {
            if (!this.txFinder.Focused) return;
            if (this.txFinder.Text.IndexOf("#") == 0) return;
            Directorio.ContadorPaginas = 0;

            // Se implementa mecanismo de restraso para teclas, si se presionan demasiado rápido, no se hace la búsqueda
            this.BusquedaLlamada++;
            new System.Threading.Thread(this.IniciarBusquedaAsincrona).Start();
        }

        #endregion

        #region [Métodos Púbicos]

        public void LimpiarBusqueda()
        {
            this.Directorio.IdPartSelect = 0;
            this.Directorio.IdProveedor = 0;
            this.Directorio.IdLinea = 0;
            this.Directorio.IdMarca = 0;

            this.Directorio.FiltroProveedor = "";
            this.Directorio.FiltroLinea = "";
            this.Directorio.FiltroMarca = "";

            this.Directorio.proveedorPorLineaMarca = "";
            this.Directorio.LineaPorProveedorMarca = "";
            this.Directorio.MarcaPorProveedorLinea = "";

            if (this.txFinder.Text == "")
            {
                this.BusquedaLlamada++;
                new System.Threading.Thread(this.IniciarBusquedaAsincrona).Start();
            }
            else
                this.txFinder.Text = "";

            this.txFinder.Focus();
        }

        #endregion

        #region [Métodos Privados]

        private void IniciarBusquedaAsincrona()
        {
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(this.BusquedaRetrasoTecla);
            if (++this.BusquedaIntento == this.BusquedaLlamada)
                this.Invoke(new Action(this.BusquedaAvanzada));
        }

        private void BusquedaAvanzada()
        {
            this.BusquedaLlamada = this.BusquedaIntento = 0;
            this.Directorio.BusquedaFiltro(this.txFinder.Text);          
        }

        private void btnVistaIconos_Click(object sender, EventArgs e)
        {
            Directorio.MostrarTarjetas(true);
            if (Directorio.UsaScroll == true)
            {
                Directorio.ContadorPaginas = 0;
                Directorio.cargaProveedorPorFiltro(Directorio.FiltroProveedor);
            }
        }

        private void btnVistaDetalle_Click(object sender, EventArgs e)
        {
            Directorio.MostrarTarjetas(false);
         
            if (Directorio.UsaScroll == true)
                Directorio.cargaProveedorPorFiltro(Directorio.FiltroProveedor);
        }

        public void EtiquetaBuscarPor_Cambio(object sender, EventArgs e)
        {
            byte seleccion =  Convert.ToByte(((TabControl)sender).SelectedIndex);

            if (seleccion == (byte)DirectorioVentasDetalle.eModo.Proveedores)
            {
                txFinder.Etiqueta = "Buscar Proveedor";
                txFinder.Text = Directorio.FiltroProveedor;
            }
            else if (seleccion == (byte)DirectorioVentasDetalle.eModo.Lineas)
            {
                txFinder.Etiqueta = "Buscar Línea";
                txFinder.Text = Directorio.FiltroLinea;
            }
            else
            {
                txFinder.Etiqueta = "Buscar Marca";
                txFinder.Text = Directorio.FiltroMarca;
            }
        }

        #endregion

    }
}