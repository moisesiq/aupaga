using System;
using System.Windows.Forms;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CuadroDeControl : UserControl
    {
        // Para el Singleton *
        private static CuadroDeControl instance;
        public static CuadroDeControl Instance
        {
            get
            {
                if (CuadroDeControl.instance == null || CuadroDeControl.instance.IsDisposed)
                    CuadroDeControl.instance = new CuadroDeControl();
                return CuadroDeControl.instance;
            }
        }
        //

        public CuadroDeControl()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void CuadroDeControl_Load(object sender, EventArgs e)
        {
            this.tabCuadroDeControl_SelectedIndexChanged(this, null);
        }

        private void tabCuadroDeControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabCuadroDeControl.SelectedTab.Name)
            {
                case "tbpVentas":
                    if (this.tbpVentas.Controls.Count <= 0)
                        this.tbpVentas.Controls.Add(new CuadroVentas() { Dock = DockStyle.Fill });
                    break;
                case "tbpClientes":
                    if (this.tbpClientes.Controls.Count <= 0)
                        this.tbpClientes.Controls.Add(new CuadroClientes() { Dock = DockStyle.Fill });
                    break;
                case "tbpVendedores":
                    if (this.tbpVendedores.Controls.Count <= 0)
                        this.tbpVendedores.Controls.Add(new CuadroVendedores() { Dock = DockStyle.Fill });
                    break;
                case "tbpProveedores":
                    if (this.tbpProveedores.Controls.Count <= 0)
                        this.tbpProveedores.Controls.Add(new CuadroProveedores() { Dock = DockStyle.Fill });
                    break;
                case "tbpMarcas":
                    if (this.tbpMarcas.Controls.Count <= 0)
                        this.tbpMarcas.Controls.Add(new CuadroMarcas() { Dock = DockStyle.Fill });
                    break;
                case "tbpLineas":
                    if (this.tbpLineas.Controls.Count <= 0)
                        this.tbpLineas.Controls.Add(new CuadroLineas() { Dock = DockStyle.Fill });
                    break;
                case "tbpCancelaciones":
                    if (this.tbpCancelaciones.Controls.Count <= 0)
                        this.tbpCancelaciones.Controls.Add(new CuadroCancelaciones() { Dock = DockStyle.Fill });
                    break;
                case "tbpSemanas":
                    if (this.tbpSemanas.Controls.Count <= 0)
                        this.tbpSemanas.Controls.Add(new CuadroSemanas() { Dock = DockStyle.Fill });
                    break;
                case "tbpMetas":
                    if (this.tbpMetas.Controls.Count <= 0)
                        this.tbpMetas.Controls.Add(new CuadroMetas() { Dock = DockStyle.Fill });
                    break;
                case "tbpExistencias":
                    if (this.tbpExistencias.Controls.Count <= 0)
                        this.tbpExistencias.Controls.Add(new CuadroExistencias() { Dock = DockStyle.Fill });
                    break;
                case "tbpSucursales":
                    if (this.tbpSucursales.Controls.Count <= 0)
                        this.tbpSucursales.Controls.Add(new CuadroSucursales() { Dock = DockStyle.Fill });
                    break;
                case "tbpCobranza":
                    if (this.tbpCobranza.Controls.Count <= 0)
                        this.tbpCobranza.Controls.Add(new CuadroCobranza() { Dock = DockStyle.Fill });
                    break;
                case "tbpVip":
                    if (this.tbpVip.Controls.Count <= 0)
                        this.tbpVip.Controls.Add(new CuadroVip() { Dock = DockStyle.Fill });
                    break;
            }
        }

        #endregion


    }
}
