﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using System.Drawing;
using FastReport;

using TheosProc;
using LibUtil;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

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
            this.cmbVendedor.CargarDatos("UsuarioID", "NombrePersona", Datos.GetListOf<Usuario>(q => q.Estatus));
            // Se llenan los combos de fechas con los datos predeterminados
            var oFechas = UtilDatos.FechasDeComisiones(DateTime.Today);
            this.dtpDe.Value = oFechas.Valor1;
            this.dtpA.Value = oFechas.Valor2;
            // Se agrega una fila al grid de totales
            this.dgvTotales.Rows.Add("TOTALES");
            this.pnlUtVendedores.Visible = false;
            //this.dgvGerentesComisiones.Visible = false;
            

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
            oRep.SetParameterValue("Fijo", Util.Decimal(this.lblFijo.Text.SoloNumeric()));
            oRep.SetParameterValue("Variable", Util.Decimal(this.lblVariable.Text.SoloNumeric()));
            oRep.SetParameterValue("Devolucion", Util.Decimal(this.lblDevoluciones.Text.SoloNumeric()));
            oRep.SetParameterValue("Total", Util.Decimal(this.lblTotal.Text.SoloNumeric()));
            oRep.Design();
        }

        private void dgvVentas_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            int iVentaID;
            if (e.Row.DataGridView.Columns.Contains("colVentaID")) // this.lblVersion
                iVentaID = Util.Entero(e.Row.Cells["colVentaID"].Value);
            else
                iVentaID = Util.Entero(e.Row.Cells["VentaID"].Value);

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
            
            int iVendedorID = Util.Entero(this.cmbVendedor.SelectedValue);
            this.oComisiones.ctlDetalle.LimpiarDetalle();
            //this.dgvGerentesComisiones.Visible = false;
            this.pnlUtVendedores.Visible = false;
            //this.lblUtilSuc.Visible = false;
            // Se muestra u oculta la columna de Utilidad
            this.dgvVentas.Columns["Utilidad"].Visible = this.bVerAdicional;
            this.dgvTotales.Columns["TotalesUtilidad"].Visible = this.bVerAdicional;

            // Se obtienen los datos del vendedor
            this.oMetaVendedor = Datos.GetEntity<MetaVendedor>(c => c.VendedorID == iVendedorID);
            if (this.oMetaVendedor == null)
            {
                Cargando.Cerrar();
                UtilLocal.MensajeInformacion("No se ha especificado una meta al vendedor.");
                return;
            }
            this.oMetaSucursal = Datos.GetEntity<MetaSucursal>(c => c.SucursalID == this.oMetaVendedor.SucursalID);
            this.mMetaSucursal = this.oMetaSucursal.UtilSucursal;
            this.mMetaVendedor = (this.oMetaVendedor.EsGerente ? this.oMetaSucursal.UtilGerente : this.oMetaSucursal.UtilVendedor);
            // Quitar variables globales de metavendedor y metasucursa, sólo deberían usarse las locales. Revisar

            // Se manda llamar el procedimiento para obtener los datos
            var oParams = new Dictionary<string, object>();
            oParams.Add("ModoID", 1);
            oParams.Add("VendedorID", iVendedorID);
            oParams.Add("Desde", this.dtpDe.Value.Date);
            oParams.Add("Hasta", this.dtpA.Value.Date);
            oParams.Add("SucursalID", Theos.SucursalID);
            //var oDatos = Datos.ExecuteProcedure<pauComisiones_Result>("pauComisiones", oParams);
            var oDatos = Datos.ExecuteProcedure<pauComisiones2_Result>("pauComisiones2test", oParams).OrderBy(c => c.Caracteristica == "D");
            //var oDatos = Datos.ExecuteProcedure<pauComisiones2_Result>("pauComisiones", oParams);
            //var oDatos = Datos.ExecuteProcedure<pauComisiones2_Result>("pauComisiones3test5", oParams);

            #region LLenarGrid
            // Se llena el grid
            this.dgvVentas.Rows.Clear();
            foreach (var oReg in oDatos)
            {
                //int iFila = this.dgvVentas.Rows.Add(oReg.VentaID, oReg.Caracteristica, oReg.Fecha, oReg.Cliente, oReg.Folio, oReg.Importe, oReg.Cobranza
                //   , oReg.Utilidad, oReg.Comision, (oReg.Caracteristica.EndsWith("9500") ? "SÍ" : ""));
                int iFila = this.dgvVentas.Rows.Add(oReg.VentaID, oReg.Caracteristica, oReg.Fecha, oReg.Cliente, oReg.Folio, oReg.Importe, oReg.Cobranza
                    , oReg.Utilidad, oReg.Comision, oReg.ComisionFija, (oReg.Es9500.Valor() ? "SÍ" : ""));
                // Se marcan los colores
                switch (oReg.Caracteristica)
                {
                    case "VD9500":
                    case "VD": this.dgvVentas.Rows[iFila].DefaultCellStyle.ForeColor = Color.Gray; break;
                    case "D9500":
                    case "D": this.dgvVentas.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red; break;
                }
            }
            #endregion


            // Se llena la línea de totales, del grid
            decimal mTotalImporte = 0, mTotalCobranza = 0;
            decimal mComision = 0, mUtilidad = 0;
            decimal mComisionVariable = 0, mComisionNegativa = 0;
            decimal mComision9500 = 0, mUtilidad9500 = 0, mComNeg9500 = 0, mUtilNeg9500 = 0;
            decimal mComisionFija = 0;
            decimal mComisionFijaDev = 0;
            decimal mComision9500Dev = 0;

            #region CalculaComisiones
            foreach (DataGridViewRow Fila in this.dgvVentas.Rows)
            {
                mTotalImporte += Util.Decimal(Fila.Cells["Importe"].Value);
                mTotalCobranza += Util.Decimal(Fila.Cells["Cobranza"].Value);
                mUtilidad += Util.Decimal(Fila.Cells["Utilidad"].Value);
                mComision = Util.Decimal(Fila.Cells["Comision"].Value);

                // bool b9500 = Util.Cadena(Fila.Cells["Caracteristica"].Value).Contains("9500");
                bool b9500 = (Util.Cadena(Fila.Cells["Detalle_9500"].Value) == "SÍ");

                //si es 9500 y es devolución se suman la cantidad
                if (b9500)
                {
                    if (Util.Cadena(Fila.Cells["Caracteristica"].Value).Substring(0, 1) == "D")
                    {
                        mComision9500Dev += Util.Decimal(Fila.Cells["Comision"].Value);
                    }
                }


                if (Util.Cadena(Fila.Cells["Caracteristica"].Value).Substring(0, 1) == "V")
                {
                    mComisionVariable += mComision;
                    mComision9500 += (b9500 ? mComision : 0);
                    mUtilidad9500 += (b9500 ? mUtilidad : 0);
                    mComisionFija += Util.Decimal(Fila.Cells["ComisionFija"].Value);
                }
                else
                {
                    //mComisionNegativa += mComision;
                    if (!b9500)
                    {
                        mComisionNegativa += Util.Decimal(Fila.Cells["Comision"].Value);
                    }
                    mComNeg9500 += (b9500 ? mComision : 0);
                    mUtilNeg9500 += (b9500 ? mUtilidad : 0);
                    mComisionFijaDev += Util.Decimal(Fila.Cells["ComisionFija"].Value);
                }

                // Para sumar la comisión de las ventas 9500
                /* if (Util.ConvertirCadena(Fila.Cells["Caracteristica"].Value) == "V9500")
                {
                    mComision9500 += mComision;
                    mUtilidad9500 += mUtilidad;
                } */

            }
            #endregion


            this.dgvTotales["TotalesImporte", 0].Value = mTotalImporte;
            this.dgvTotales["TotalesCobranza", 0].Value = mTotalCobranza;
            this.dgvTotales["TotalesUtilidad", 0].Value = mUtilidad;
            this.dgvTotales["TotalesComision", 0].Value = (mComisionVariable + mComisionNegativa + mComNeg9500); //se cambio a resta, original suma
            this.dgvTotales["TotalFija", 0].Value = mComisionFija;
            

            // Se obtienen los totales de tienda
            this.LlenarUtilidadSuc();



            #region LlenarEtiquetas
            // Se llenan los totales del vendedor

            decimal mFijo = this.oMetaVendedor.SueldoFijo;

            decimal mUtilidadSuc = this.mUtilidadSuc; // (this.mUtilidadSuc - this.mGastoSuc);


            // Se calcula el total
            decimal mUtilMinimo = (this.oMetaVendedor.EsGerente ? this.oMetaSucursal.UtilGerente : this.oMetaSucursal.UtilVendedor);
            decimal mComisionGerente = 0;
            if (mUtilidadSuc >= this.oMetaSucursal.UtilSucursalMinimo && mUtilidad >= mUtilMinimo)
            {
                if (this.oMetaVendedor.EsGerente)
                {
                    /*
                    decimal mExcedente = (mUtilidadSuc - this.mMetaSucursal);
                    int iMultiplicador = (int)(mExcedente / this.oMetaVendedor.IncrementoUtil.Valor());
                    decimal mComisionGerente = (this.oMetaVendedor.IncrementoFijo.Valor() * iMultiplicador);
                    */

                    mComisionGerente = VentasProc.CalcularComisionGerente(this.oMetaSucursal.UtilSucursalMinimo, mUtilidadSuc, this.oMetaVendedor.IncrementoUtil.Valor()
                        , this.oMetaVendedor.IncrementoFijo.Valor());
                    this.lblVariable.Text = mComisionGerente.ToString(GlobalClass.FormatoMoneda);
                    this.lblTotal.Text = (this.oMetaVendedor.SueldoFijo + mComisionGerente + mComision9500 + (mComNeg9500)).ToString(GlobalClass.FormatoMoneda);
                }
            }
            else
            {
                //if (this.oMetaVendedor.EsGerente)
                //    this.lblVariable.Text = 0.ToString(GlobalClass.FormatoMoneda);
                //this.lblTotal.Text = "--";
            }




            if (!this.oMetaVendedor.MetaConsiderar9500)
                mUtilidad -= mUtilidad9500;

            //this.lblUtilSuc.Text = mUtilidadSuc.ToString(GlobalClass.FormatoMoneda);

            //subtotal comisiones y fijo
            this.lblFijo.Text = mFijo.ToString(GlobalClass.FormatoMoneda);

            //subtotal variable
            if (this.oMetaVendedor.EsGerente)
            {
                this.lblVariable.Text = mComisionGerente.ToString(GlobalClass.FormatoMoneda);
                this.lblDevoluciones.Text = 0.ToString(GlobalClass.FormatoMoneda);
                this.lblSubVariable.Text = mComisionGerente.ToString(GlobalClass.FormatoMoneda);
            }
            else
            {
                this.lblVariable.Text = (mComisionVariable - mComision9500).ToString(GlobalClass.FormatoMoneda);
                this.lblDevoluciones.Text = (mComisionNegativa).ToString(GlobalClass.FormatoMoneda);
                this.lblSubVariable.Text = ((mComisionVariable - mComision9500) - Math.Abs(mComisionNegativa)).ToString(GlobalClass.FormatoMoneda);
            }


            //Subtotal 9500
            this.lbl9500.Text = (mComision9500).ToString(GlobalClass.FormatoMoneda);
            this.lblDev9500.Text = (mComision9500Dev).ToString(GlobalClass.FormatoMoneda);
            this.lblSub9500.Text = (mComision9500 - Math.Abs(mComision9500Dev)).ToString(GlobalClass.FormatoMoneda);

            //subtotal comision fija
            //this.lblTotal.Text = (mFijo + mComisionVariable + (mComisionNegativa)).ToString(GlobalClass.FormatoMoneda);
            this.lblComisionFija.Text = (mComisionFija).ToString(GlobalClass.FormatoMoneda);
            this.lblDevFija.Text = (mComisionFijaDev).ToString(GlobalClass.FormatoMoneda);
            this.lblSubFija.Text = (mComisionFija + mComisionFijaDev).ToString(GlobalClass.FormatoMoneda);

            decimal Total = 0;
            //total y meta
            if (this.oMetaVendedor.EsGerente)
            {
                Total = (mFijo + (mComisionGerente) + mComision9500 + mComisionFija + mComision9500Dev);
            }
            else
            {
                Total = (mFijo + (mComisionVariable - mComision9500) + mComision9500 + mComisionFija + mComisionFijaDev + mComisionNegativa + mComision9500Dev);
            }
            
            this.lblTotal.Text = (Total).ToString(GlobalClass.FormatoMoneda);

            if (this.oMetaVendedor.SueldoMeta - Total < 0)
            {
                this.lblMetaRes.Text = (0).ToString(GlobalClass.FormatoMoneda);
            }
            else
            {
                this.lblMetaRes.Text = (this.oMetaVendedor.SueldoMeta - Total).ToString(GlobalClass.FormatoMoneda);
            }


#endregion



            if (this.oMetaVendedor.EsGerente)
            {
                decimal meta = this.oMetaSucursal.UtilSucursalMinimo;
                ComisionesVendedorPorSucursal(mUtilidadSuc, this.oMetaSucursal.UtilSucursalMinimo, mComisionGerente);
            }

            // Se cierra la ventana de "Cargando.."
            Cargando.Cerrar();

            this.ComisionesAct = true;
        }


        private void ComisionesVendedorPorSucursal(decimal UtilSucursal, decimal UtilSucursalMinimo, decimal mComisionGerente)
        {
            if (UtilSucursal < UtilSucursalMinimo)
                return;

            this.dgvGerentesComisiones.Rows.Clear();
            this.pnlUtVendedores.Visible = true;
            var VendedoresSucursal = Datos.GetListOf<MetaVendedor>(c => c.SucursalID == Theos.SucursalID);
            
            Dictionary<string,decimal> dic = new Dictionary<string,decimal>();
            Dictionary<string, decimal> dic2 = new Dictionary<string, decimal>();

            decimal totales = UtilSucursal - UtilSucursalMinimo;

            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.dtpDe.Value.Date);
            oParams.Add("Hasta", this.dtpA.Value.Date);
            oParams.Add("SucursalID", Theos.SucursalID);

            var oDatos = Datos.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado2", oParams);
            foreach (var i in VendedoresSucursal)   
            {
                decimal suma = (decimal)oDatos.ToList().Where(f => f.RealizoUsuarioID == i.VendedorID).Sum(c => c.Utilidad);
                dic.Add(Datos.GetEntity<Usuario>( c => c.UsuarioID == i.VendedorID).NombreUsuario, suma);
            }

            foreach(var i in dic)
            {
                decimal mUtilidadVendedores = dic.Sum(c => c.Value);
                dic2.Add(i.Key, ((i.Value * 100) / mUtilidadVendedores));
            }

            dic.Clear();

            foreach(var i in dic2)
            {
                dic.Add(i.Key, ((i.Value * mComisionGerente) / 100));
            }


            //se llenan las comisiones aportadas por cada vendedor al variable 
            foreach (var oReg in dic)
            {
                this.dgvGerentesComisiones.Rows.Add(oReg.Key, oReg.Value);
            }

        }


        private void LlenarUtilidadSuc()
        {

            int iSucursal = this.oMetaVendedor.SucursalID;
            int iUsuario = this.oMetaVendedor.VendedorID;
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.dtpDe.Value);
            oParams.Add("Hasta", this.dtpA.Value);
            oParams.Add("SucursalID", iSucursal);
            //oParams.Add("VendedorID", iUsuario);
            //oParams.Add("ModoID", 1);

            // Se calcula la utilidad
            var oUtilidad = Datos.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado2", oParams);
            //var oUtilidad = Datos.ExecuteProcedure<pauComisionesAgrupadoTest3_Result>("pauComisionesAgrupadoTest3", oParams);
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
            int iVendedorID = Util.Entero(this.cmbVendedor.SelectedValue);
            this.dgvComisionesNp.DataSource = null;
            this.oComisiones.ctlDetalle.LimpiarDetalle();

            // Se manda llamar el procedimiento para obtener los datos
            var oParams = new Dictionary<string, object>();
            oParams.Add("ModoID", 2);
            oParams.Add("VendedorID", iVendedorID);
            oParams.Add("SucursalID", Theos.SucursalID);
            //var oDatos = Datos.ExecuteProcedure<pauComisiones_Result>("pauComisionesNormal", oParams);
            //var oDatos = Datos.ExecuteProcedure<pauComisiones_Result>("pauComisiones", oParams);
            var oDatos = Datos.ExecuteProcedure<pauComisiones_Result>("pauComisiones2test", oParams);
            //var oDatos = Datos.ExecuteProcedure<pauComisiones_Result>("pauComisiones3test5", oParams);

            // Se llena el grid
            try
            {
                this.dgvComisionesNp.DataSource = new SortableBindingList<pauComisiones_Result>(oDatos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            // int iUsuarioCom = Util.ConvertirEntero(this.cmbVendedor.SelectedValue);
            // this.oMetas.Preparar(iUsuarioCom);
            // Se llenan los vendedores
            // this.cmbVendedor.CargarDatos("UsuarioID", "NombrePersona", General.GetListOf<Usuario>(q => q.Estatus));
            // this.cmbVendedor.SelectedValue = this.oComisiones.UsuarioAcceso.UsuarioID;
            // Se verifica el usuario, y se bloquea o permite la selección de otro vendedor
            this.cmbVendedor.Enabled = UtilLocal.ValidarPermiso(this.oComisiones.UsuarioAcceso.UsuarioID, "Ventas.Comisiones.VerOtrosUsuarios");

            // Se asigna el permiso de usuario avanzado, o para ver datos críticos
            this.bVerAdicional = UtilLocal.ValidarPermiso(this.oComisiones.UsuarioAcceso.UsuarioID, "Ventas.Comisiones.VerAdicional");
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
            this.oMetas.UsuarioID = Util.Entero(this.cmbVendedor.SelectedValue);
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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tbpComisiones_Click(object sender, EventArgs e)
        {

        }

        private void dgvGerentesComisiones_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void dgvVentas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



    }
}