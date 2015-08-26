using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using System.Drawing;
using FastReport;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class Comisiones : UserControl
    {
        public VentasComisiones oComisiones;

        private bool ComisionesAct, ComisionesNpAct;
        decimal mMetaVendedor, mMetaSucursal;
        decimal mUtilidadSuc;//, mGastoSuc;
        MetaVendedor oMetaVendedor;
        MetaSucursal oMetaSucursal;
        Metas oMetas;

        bool bVerAdicional = false;

        public Comisiones()
        {
            InitializeComponent();
        }

        #region [ Eventos ]

        private void Comisiones_Load(object sender, EventArgs eo)
        {
            // Se oculta el tab de logros. Se implementará en siguiente versión
            this.tabComisiones.TabPages.Remove(this.tbpLogros);

            // Se llenan los vendedores
            this.cmbVendedor.CargarDatos("UsuarioID", "NombrePersona", General.GetListOf<Usuario>(q => q.Estatus));
            // Se llenan los combos de fechas con los datos predeterminados
            var oFechas = UtilDatos.FechasDeComisiones(DateTime.Today);
            this.dtpDe.Value = oFechas.Valor1;
            this.dtpA.Value = oFechas.Valor2;
            // Se agrega una fila al grid de totales
            this.dgvTotales.Rows.Add("TOTALES");

            // Los datos se mandan llenar desde el manejador (VentasComisiones) al Activar
        }

        private void cmbVendedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbVendedor.Focused)
            {
                this.ComisionesAct = this.ComisionesNpAct = false;
                this.ActualizarComisiones();
            }
        }

        private void dtpDe_ValueChanged(object sender, EventArgs e)
        {
            /*
            if (this.dtpDe.Focused)
            {
                this.ComisionesAct = this.ComisionesNpAct = false;
                this.ActualizarComisiones();
            }
            */
        }

        private void dtpA_ValueChanged(object sender, EventArgs e)
        {
            /*
            if (this.dtpA.Focused)
            {
                this.ComisionesAct = this.ComisionesNpAct = false;
                this.ActualizarComisiones();
            }
            */
        }

        private void dtpDe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                this.ComisionesAct = this.ComisionesNpAct = false;
                this.ActualizarComisiones();
            }
        }

        private void dtpA_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                this.ComisionesAct = this.ComisionesNpAct = false;
                this.ActualizarComisiones();
            }
        }

        private void btnGraficas_Click(object sender, EventArgs e)
        {
            this.ActivarMetas(sender != null);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {   
            var oVendedor = (this.cmbVendedor.SelectedItem as Usuario);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "Comisiones.frx");
            oRep.RegisterData(this.dgvVentas.ADataTable(), "Ventas");
            oRep.RegisterData(this.dgvTotales.ADataTable(), "Totales");
            oRep.SetParameterValue("Vendedor", (oVendedor == null ? "" : oVendedor.NombrePersona));
            oRep.SetParameterValue("FechaDe", this.dtpDe.Value);
            oRep.SetParameterValue("FechaA", this.dtpA.Value);
            oRep.SetParameterValue("Fijo", Helper.ConvertirDecimal(this.lblFijo.Text.SoloNumeric()));
            oRep.SetParameterValue("Variable", Helper.ConvertirDecimal(this.lblVariable.Text.SoloNumeric()));
            oRep.SetParameterValue("Devolucion", Helper.ConvertirDecimal(this.lblDevoluciones.Text.SoloNumeric()));
            oRep.SetParameterValue("Total", Helper.ConvertirDecimal(this.lblTotal.Text.SoloNumeric()));
            oRep.Design();
        }

        private void dgvVentas_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            int iVentaID;
            if (e.Row.DataGridView.Columns.Contains("colVentaID")) // this.lblVersion
                iVentaID = Helper.ConvertirEntero(e.Row.Cells["colVentaID"].Value);
            else
                iVentaID = Helper.ConvertirEntero(e.Row.Cells["VentaID"].Value);

            this.oComisiones.ctlDetalle.LlenarDetalle(iVentaID);
        }

        private void tabComisiones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.tabComisiones.Focused) return;


            switch (this.tabComisiones.SelectedTab.Name)
            {
                case "tbpComisiones":
                    this.pnlTotalesNp.Visible = false;
                    if (!this.ComisionesAct)
                        this.ActualizarComisiones();
                    break;
                case "tbpComisionesNp":
                    this.pnlTotalesNp.Visible = true;
                    if (!this.ComisionesNpAct)
                        this.ActualizarComisiones();
                    break;
            }
        }
        
        #endregion

        #region [ Métodos ]

        /*
        private decimal CalcularComision(int iParteID, decimal mPrecioUnitario, decimal mCantidad, int? ComisionistaID)
        {
            var oPartePrecio = General.GetEntity<PartePrecio>(q => q.ParteID == iParteID && q.Estatus);
            var oPrecios = new decimal[] {
                oPartePrecio.PrecioUno.Valor(),
                oPartePrecio.PrecioDos.Valor(),
                oPartePrecio.PrecioTres.Valor(),
                oPartePrecio.PrecioCuatro.Valor(),
                oPartePrecio.PrecioCinco.Valor()
            };

            decimal mCosto = oPartePrecio.Costo.Valor();

            if (ComisionistaID.Valor() > 0)
            {
                int iComisionistaID = ComisionistaID.Valor();
                var oComisionista = General.GetEntity<Cliente>(q => q.ClienteID == iComisionistaID && q.Estatus);
                mPrecioUnitario = UtilLocal.ObtenerPrecioSinIva(oPrecios[oComisionista.ListaDePrecios - 1]);
            }

            decimal mUtilidad = ((mPrecioUnitario - mCosto) * mCantidad);
            return (mUtilidad * this.PorcentajeDeComision);
        }
        */

        private void ActualizarComisiones()
        {
            if (this.tabComisiones.SelectedTab.Name == "tbpComisiones")
                this.LlenarVentasProc();
            else
                this.LlenarVentasNp();
        }

        private void LlenarVentasProc()
        {            
            // Se muestra la ventana de "Cargando.."
            Cargando.Mostrar();
            
            //
            int iVendedorID = Helper.ConvertirEntero(this.cmbVendedor.SelectedValue);
            // this.dgvVentas.Columns.Clear();
            // this.dgvVentas.DataSource = null;
            this.oComisiones.ctlDetalle.LimpiarDetalle();

            // Se muestra u oculta la columna de Utilidad
            this.dgvVentas.Columns["Utilidad"].Visible = this.bVerAdicional;
            this.dgvTotales.Columns["TotalesUtilidad"].Visible = this.bVerAdicional;

            // Se obtienen los datos del vendedor
            this.oMetaVendedor = General.GetEntity<MetaVendedor>(c => c.VendedorID == iVendedorID);
            this.oMetaSucursal = General.GetEntity<MetaSucursal>(c => c.SucursalID == this.oMetaVendedor.SucursalID);
            this.mMetaSucursal = this.oMetaSucursal.UtilSucursal;
            this.mMetaVendedor = (this.oMetaVendedor.EsGerente ? this.oMetaSucursal.UtilGerente : this.oMetaSucursal.UtilVendedor);
            // Quitar variables globales de metavendedor y metasucursa, sólo deberían usarse las locales. Revisar

            // Se manda llamar el procedimiento para obtener los datos
            var oParams = new Dictionary<string, object>();
            oParams.Add("ModoID", 1);
            oParams.Add("VendedorID", iVendedorID);
            oParams.Add("Desde", this.dtpDe.Value.Date);
            oParams.Add("Hasta", this.dtpA.Value.Date);
            var oDatos = General.ExecuteProcedure<pauComisiones_Result>("pauComisiones", oParams);

            // Se llena el grid
            this.dgvVentas.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                int iFila = this.dgvVentas.Rows.Add(oReg.VentaID, oReg.Caracteristica, oReg.Fecha, oReg.Cliente, oReg.Folio, oReg.Importe, oReg.Cobranza
                    , oReg.Utilidad, oReg.Comision, (oReg.Caracteristica.EndsWith("9500") ? "SÍ" : ""));
                // Se marcan los colores
                switch (oReg.Caracteristica)
                {
                    case "VD": this.dgvVentas.Rows[iFila].DefaultCellStyle.ForeColor = Color.Gray; break;
                    case "D9500":
                    case "D": this.dgvVentas.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red; break;
                }
            }

            // Se llena la línea de totales, del grid
            decimal mTotalImporte = 0, mTotalCobranza = 0;
            decimal mComision = 0, mUtilidad = 0;
            decimal mComisionVariable = 0, mComisionNegativa = 0;
            decimal mComision9500 = 0, mUtilidad9500 = 0, mComNeg9500 = 0, mUtilNeg9500 = 0;
            foreach (DataGridViewRow Fila in this.dgvVentas.Rows)
            {
                mTotalImporte += Helper.ConvertirDecimal(Fila.Cells["Importe"].Value);
                mTotalCobranza += Helper.ConvertirDecimal(Fila.Cells["Cobranza"].Value);
                mUtilidad += Helper.ConvertirDecimal(Fila.Cells["Utilidad"].Value);
                mComision = Helper.ConvertirDecimal(Fila.Cells["Comision"].Value);

                bool b9500 = Helper.ConvertirCadena(Fila.Cells["Caracteristica"].Value).Contains("9500");
                if (Helper.ConvertirCadena(Fila.Cells["Caracteristica"].Value).Substring(0, 1) == "V")
                {
                    mComisionVariable += mComision;
                    mComision9500 += (b9500 ? mComision : 0);
                    mUtilidad9500 += (b9500 ? mUtilidad : 0);
                }
                else
                {
                    mComisionNegativa += mComision;
                    mComNeg9500 += (b9500 ? mComision : 0);
                    mUtilNeg9500 += (b9500 ? mUtilidad : 0);
                }

                // Para sumar la comisión de las ventas 9500
                /* if (Helper.ConvertirCadena(Fila.Cells["Caracteristica"].Value) == "V9500")
                {
                    mComision9500 += mComision;
                    mUtilidad9500 += mUtilidad;
                } */
            }
            this.dgvTotales["TotalesImporte", 0].Value = mTotalImporte;
            this.dgvTotales["TotalesCobranza", 0].Value = mTotalCobranza;
            this.dgvTotales["TotalesUtilidad", 0].Value = mUtilidad;
            this.dgvTotales["TotalesComision", 0].Value = (mComisionVariable + mComisionNegativa);

            // Se obtienen los totales de tienda
            this.LlenarUtilidadSuc();

            // Se llenan los totales del vendedor
            decimal mFijo = this.oMetaVendedor.SueldoFijo;
            decimal mUtilidadSuc = this.mUtilidadSuc; // (this.mUtilidadSuc - this.mGastoSuc);
            if (!this.oMetaVendedor.MetaConsiderar9500)
                mUtilidad -= mUtilidad9500;

            this.lblFijo.Text = mFijo.ToString(GlobalClass.FormatoMoneda);
            this.lblVariable.Text = (mComisionVariable - mComision9500).ToString(GlobalClass.FormatoMoneda);
            this.lbl9500.Text = (mComision9500).ToString(GlobalClass.FormatoMoneda);
            this.lblDevoluciones.Text = (mComisionNegativa).ToString(GlobalClass.FormatoMoneda);
            this.lblTotal.Text = (mFijo + mComisionVariable + (mComisionNegativa)).ToString(GlobalClass.FormatoMoneda);

            // Se calcula el total
            decimal mUtilMinimo = (this.oMetaVendedor.EsGerente ? this.oMetaSucursal.UtilGerente : this.oMetaSucursal.UtilVendedor);
            if (mUtilidadSuc >= this.oMetaSucursal.UtilSucursalMinimo && mUtilidad >= mUtilMinimo)
            {
                if (this.oMetaVendedor.EsGerente)
                {
                    /*
                    decimal mExcedente = (mUtilidadSuc - this.mMetaSucursal);
                    int iMultiplicador = (int)(mExcedente / this.oMetaVendedor.IncrementoUtil.Valor());
                    decimal mComisionGerente = (this.oMetaVendedor.IncrementoFijo.Valor() * iMultiplicador);
                    */
                    decimal mComisionGerente = VentasProc.CalcularComisionGerente(this.oMetaSucursal.UtilSucursalMinimo, mUtilidadSuc, this.oMetaVendedor.IncrementoUtil.Valor()
                        , this.oMetaVendedor.IncrementoFijo.Valor());
                    this.lblVariable.Text = mComisionGerente.ToString(GlobalClass.FormatoMoneda);
                    this.lblTotal.Text = (this.oMetaVendedor.SueldoFijo + mComisionGerente + mComision9500 + (mComNeg9500)).ToString(GlobalClass.FormatoMoneda);
                }
            }
            else
            {
                if (this.oMetaVendedor.EsGerente)
                    this.lblVariable.Text = 0.ToString(GlobalClass.FormatoMoneda);
                this.lblTotal.Text = "--";
            }
            
            // Se cierra la ventana de "Cargando.."
            Cargando.Cerrar();

            this.ComisionesAct = true;
        }

        private void LlenarUtilidadSuc()
        {
            int iSucursal = this.oMetaVendedor.SucursalID;
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.dtpDe.Value);
            oParams.Add("Hasta", this.dtpA.Value);

            // Se calcula la utilidad
            var oUtilidad = General.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado", oParams);
            this.mUtilidadSuc = oUtilidad.Where(c => c.SucursalID == iSucursal).Sum(c => c.Utilidad).Valor();
            // Se calculan los gastos
            /* oParams.Add("SucursalID", iSucursal);
            var oGastos = General.ExecuteProcedure<pauContaCuentasMovimientosTotales_Result>("pauContaCuentasMovimientosTotales", oParams);
            this.mGastoSuc = oGastos.Sum(c => c.Importe).Valor();
            */
        }

        private void LlenarVentasNp()
        {
            // Se muestra la ventana de "Cargando.."
            Cargando.Mostrar();

            //
            int iVendedorID = Helper.ConvertirEntero(this.cmbVendedor.SelectedValue);
            this.dgvComisionesNp.DataSource = null;
            this.oComisiones.ctlDetalle.LimpiarDetalle();

            // Se manda llamar el procedimiento para obtener los datos
            var oParams = new Dictionary<string, object>();
            oParams.Add("ModoID", 2);
            oParams.Add("VendedorID", iVendedorID);
            var oDatos = General.ExecuteProcedure<pauComisiones_Result>("pauComisiones", oParams);

            // Se llena el grid
            this.dgvComisionesNp.DataSource = new SortableBindingList<pauComisiones_Result>(oDatos);
            // Se configuran las columnas
            this.dgvComisionesNp.OcultarColumnas("VentaID", "Cobranza", "ACredito", "Caracteristica", "Orden", "Utilidad");
            this.dgvComisionesNp.Columns["Importe"].FormatoMoneda();
            this.dgvComisionesNp.Columns["Comision"].FormatoMoneda();
            this.dgvComisionesNp.AutoResizeColumns();

            // Se llenan los totales del vendedor
            this.lblTotalNp.Text = (oDatos.Sum(q => q.Comision).Value.ToString(GlobalClass.FormatoMoneda));

            // Se cierra la ventana de "Cargando.."
            Cargando.Cerrar();

            this.ComisionesNpAct = true;
        }

        #endregion

        #region [ Públicos ]

        public void CambiarUsuario()
        {
            this.cmbVendedor.SelectedValue = this.oComisiones.UsuarioAcceso.UsuarioID;
            // int iUsuarioCom = Helper.ConvertirEntero(this.cmbVendedor.SelectedValue);
            // this.oMetas.Preparar(iUsuarioCom);
            // Se llenan los vendedores
            // this.cmbVendedor.CargarDatos("UsuarioID", "NombrePersona", General.GetListOf<Usuario>(q => q.Estatus));
            // this.cmbVendedor.SelectedValue = this.oComisiones.UsuarioAcceso.UsuarioID;
            // Se verifica el usuario, y se bloquea o permite la selección de otro vendedor
            this.cmbVendedor.Enabled = UtilDatos.ValidarPermiso(this.oComisiones.UsuarioAcceso.UsuarioID, "Ventas.Comisiones.VerOtrosUsuarios");

            // Se asigna el permiso de usuario avanzado, o para ver datos críticos
            this.bVerAdicional = UtilDatos.ValidarPermiso(this.oComisiones.UsuarioAcceso.UsuarioID, "Ventas.Comisiones.VerAdicional");
            this.oMetas.bVerAdicional = this.bVerAdicional;

            //
            this.ActualizarComisiones();
        }

        public void ActivarMetas(bool bModoNormal)
        {
            if (this.oMetas == null)
            {
                this.oMetas = new Metas() { Dock = DockStyle.Fill };
                this.Controls.Add(this.oMetas);
                // this.oMetas.Preparar(0);
            }

            this.oMetas.SucursalID = GlobalClass.SucursalID;
            this.oMetas.UsuarioID = Helper.ConvertirEntero(this.cmbVendedor.SelectedValue);
            this.oMetas.Desde = this.dtpDe.Value;
            this.oMetas.Hasta = this.dtpA.Value;
            // this.oMetas.bVerAdicional = this.bVerAdicional;

            this.oMetas.Preparar(this.oMetas.UsuarioID);

            this.oMetas.BringToFront();
            this.oMetas.Show();

            if (bModoNormal)
                this.oMetas.CargarDatos();
            else
                this.oMetas.CargarDatosGeneral();
        }

        #endregion

    }
}