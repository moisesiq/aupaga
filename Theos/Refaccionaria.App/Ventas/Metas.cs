using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.IO;
using System.Data.Objects;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class Metas : UserControl
    {
        const int BarraMarcaInicio = 333;//331;
        const int BarraMarcaFin = 8;//8;
        const int RotacionMinima = -45;
        const int RotacionMaxima = 180;

        CommonTools.TreeListView tlvGastos;
        MetaSucursal oMetaSucursal;
        MetaVendedor oMetaVendedor;
        decimal mUtilidad;
        decimal mUtilidadFinal;
        decimal mComision, mComision9500, mComisionGerente, mComisionTotal;
        decimal mGastosTotal;
        decimal mGastosFijos;
        decimal mGastosSueldos;
        
        public Metas()
        {
            InitializeComponent();

            // Se arreglan imágenes para la barra
            this.AgregarControlContenedor(this.pcbBarraMarcaMinimoAtras, this.chrComisionActual);
            this.AgregarControlContenedor(this.pcbBarraMarca, this.chrComisionActual);
            this.AgregarControlContenedor(this.pcbBarraMarcaMinimo, this.pcbBarraMarca);
            this.AgregarControlContenedor(this.lblBarraMarca, this.pcbBarraMarca);
            this.lblBarraMarca.Location = new Point(0, 20);
            this.pcbBarraMarcaMinimo.Left = 0;

            // Se agregan las marcas del tacómetro
            this.AgregarControlContenedor(this.lblFecha, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblUtilidadTotal, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca01, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca02, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca03, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca04, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca05, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca06, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca07, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca08, this.pcbTacometroMarca);
            this.AgregarControlContenedor(this.lblMarca09, this.pcbTacometroMarca);
            // Se arreglan las imágenes para el tacómetro
            this.pcbTacometro.Controls.Add(this.pcbTacometroMarca);
            this.pcbTacometroMarca.Location = new Point(0, 0);
            this.pcbTacometroMarca.Size = this.pcbTacometro.Size;// new Size(400, 400);
            // Para la marca mínima
            this.pcbTacometroMarca.Controls.Add(this.pcbTacometroMinimo);
            this.pcbTacometroMinimo.Location = new Point(0, 0);
            this.pcbTacometroMinimo.Size = this.pcbTacometro.Size;// new Size(400, 400);

            // this.AgregarControlContenedor(this.pcbTacometroMinimo, this.pcbTacometroMarca);
            // this.pcbTacometroMinimo.Size = this.pcbTacometroMarca.Size;
            //
            this.AgregarControlContenedor(this.chrUtilidadSem, this.pcbTacometro);

            // Se agrega el TreeListView
            this.tlvGastos = new CommonTools.TreeListView();
            this.tlvGastos.BackColor = this.BackColor;
            this.tlvGastos.ForeColor = Color.White;
            this.tlvGastos.ColumnsOptions.HeaderHeight = 0;
            this.tlvGastos.RowOptions.ShowHeader = false;
            this.tlvGastos.HideSelection = true;
            this.tlvGastos.Size = new Size(this.pnlReferenciaGastos.Width + 17, this.pnlReferenciaGastos.Height);
            this.tlvGastos.Name = "tlvDatos";
            this.tlvGastos.ViewOptions.ShowGridLines = false;
            this.pnlReferenciaGastos.Controls.Add(this.tlvGastos);
        }

        #region [ Propiedades ]

        public int SucursalID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public bool bVerAdicional { get; set; }

        private bool _MostrarMinimizar = true;
        public bool MostrarMinimizar
        {
            get { return this._MostrarMinimizar; }
            set {
                this._MostrarMinimizar = value;
                this.btnOcultar.Visible = value;
            }
        }

        #endregion

        #region [ Eventos ]

        private void Metas_Load(object sender, EventArgs e)
        {
            //
            /*
            this.SucursalID = GlobalClass.SucursalID;
            // Se llenan las variables de las fechas
            int iDiaIni = Util.ConvertirEntero(Config.Valor("Comisiones.DiaInicial"));
            int iDiaFin = Util.ConvertirEntero(Config.Valor("Comisiones.DiaFinal"));
            int iDiaHoy = (int)DateTime.Today.DayOfWeek;
            this.Desde = DateTime.Now.AddDays((double)(iDiaHoy <= iDiaIni ? (iDiaIni - 7 - iDiaHoy) : (iDiaIni - iDiaHoy)));
            this.Hasta = DateTime.Now.AddDays((double)(iDiaHoy > (iDiaFin + 1) ? (iDiaFin + 7 - iDiaHoy) : (iDiaFin - iDiaHoy)));
            this.Desde = this.Desde.Date;
            this.Hasta = this.Hasta.Date.AddDays(1).AddSeconds(-1);
            */ 
            
            // Se configuran etiquetas del tacómetro
            this.lblFecha.Text = string.Format("{0:00}\n{1}", DateTime.Now.Day, DateTime.Now.ToMonthName().Substring(0, 3).ToUpper());
            this.lblUtilidadTotal.Text = "$0.00";

            // Se configuran las columnas de los gastos
            this.tlvGastos.Columns.Add(new CommonTools.TreeListColumn("Movimiento", "Movimiento", 120));
            this.tlvGastos.Columns.Add(new CommonTools.TreeListColumn("Importe", "Importe", 80));
            this.tlvGastos.Columns["Importe"].CellFormat.TextAlignment = ContentAlignment.MiddleRight;
            
            this.tlvGastos.Columns["Movimiento"].CellFormat.ForeColor = Color.White;
            this.tlvGastos.Columns["Importe"].CellFormat.ForeColor = Color.White;
                        
            // Se mandan cargar los datos
            // this.CargarDatos();
        }

        private void btnOcultar_Click(object sender, EventArgs e)
        {
            if (this.chrComisionActual.Visible)
            {
                this.Hide();
                // (this.Parent as Comisiones).CambiarUsuario();
            }
            else
            {
                var ResUsuario = UtilLocal.ValidarObtenerUsuario("Ventas.Comisiones.Ver");
                if (ResUsuario.Error)
                    return;
                (this.Parent as Comisiones).oComisiones.UsuarioAcceso = ResUsuario.Respuesta;
                this.Preparar(ResUsuario.Respuesta.UsuarioID);
                (this.Parent as Comisiones).CambiarUsuario();

                // Se verifica si el usuario es tipo repartidor, para no mostrar gráficas
                if (ResUsuario.Respuesta.TipoUsuarioID == Cat.TiposDeUsuario.Repartidor)
                    this.Hide();
                else
                    this.CargarDatos();
            }
        }

        #endregion

        #region [ Métodos ]

        private void AgregarControlContenedor(Control oHijo, Control oPadre)
        {
            oPadre.Controls.Add(oHijo);
            oHijo.Left -= oPadre.Left;
            oHijo.Top -= oPadre.Top;
            oHijo.BringToFront();
        }
                
        private void CalcularUtilidadComision()
        {
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);
            oParams.Add("ModoID", 1);
            oParams.Add("VendedorID", Theos.UsuarioID);
            oParams.Add("SucursalID", Theos.SucursalID);
            //var oDatos = Datos.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupado", oParams);
            var oDatos = Datos.ExecuteProcedure<pauComisionesAgrupado_Result>("pauComisionesAgrupadoTest2", oParams);

            this.mUtilidad = 0;
            this.mComision = 0;
            if (oDatos.Count > 0)
            {
                if (this.SucursalID > 0)
                    this.mUtilidad = oDatos.Where(c => c.SucursalID == this.SucursalID).Sum(c => c.Utilidad).Valor();
                else
                    this.mUtilidad = oDatos.Sum(c => c.Utilidad).Valor();
                this.mComision = oDatos.Where(c => c.RealizoUsuarioID == this.UsuarioID && !c.Es9500.Valor()).Sum(c => c.Comision).Valor();
                this.mComision9500 = oDatos.Where(c => c.RealizoUsuarioID == this.UsuarioID && c.Es9500.Valor()).Sum(c => c.Comision).Valor();

                // Cálculo por si es gerente
                if (this.oMetaVendedor != null && this.oMetaVendedor.EsGerente)
                {
                    decimal mUtilVen = oDatos.Where(c => c.RealizoUsuarioID == this.UsuarioID).Sum(c => c.Utilidad).Valor();
                    if (mUtilidad >= this.oMetaSucursal.UtilSucursalMinimo && mUtilVen >= this.oMetaSucursal.UtilGerente)
                        this.mComisionGerente = VentasProc.CalcularComisionGerente(this.oMetaSucursal.UtilSucursalMinimo, mUtilidad
                            , this.oMetaVendedor.IncrementoUtil.Valor(), this.oMetaVendedor.IncrementoFijo.Valor());
                    else
                        this.mComisionGerente = 0;
                }

                this.mComisionTotal = (this.mComision + this.mComision9500);
            }
            this.mUtilidadFinal = (this.mUtilidad - this.mGastosTotal);
        }

        private void CargarArbolDeGastos()
        {
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);
            oParams.Add("SucursalID", (this.SucursalID > 0 ? (int?)this.SucursalID : null));

            this.mGastosTotal = this.mGastosFijos = this.mGastosSueldos = 0;
            var oDatos = Datos.ExecuteProcedure<pauContaCuentasMovimientosTotales_Result>("pauContaCuentasMovimientosTotales", oParams);
            oDatos = oDatos.Where(c => c.AfectaMetas.Valor()).OrderBy(o => o.SumaGastosFijos).ThenBy(o => o.Cuenta).ThenBy(o => o.Subcuenta)
                .ThenBy(o => o.CuentaDeMayor).ThenBy(o => o.CuentaAuxiliar).ThenBy(o => o.Egreso).ToList();

            this.tlvGastos.Nodes.Clear();
            var oGastosFijos = this.tlvGastos.Nodes.Add("GASTOS FIJOS");
            CommonTools.Node oCuentaDeMayor = null, oCuentaAuxiliar = null, oEgreso = null;
            string sCuentaDeMayor = "", sCuentaAuxiliar = "";
            decimal mImporte;
            foreach (var oReg in oDatos)
            {
                mImporte = oReg.Importe.Valor();

                // Si tiene bandera de suma Gastos Fijos, se agrupa en un concepto
                if (oReg.SumaGastosFijos.Valor())
                {
                    if (oReg.CuentaDeMayor != sCuentaDeMayor)
                    {
                        sCuentaDeMayor = oReg.CuentaDeMayor;
                        oCuentaDeMayor = oGastosFijos.Nodes.Add(oReg.CuentaDeMayor);
                    }
                }
                else
                {

                    // Nodo de Cuenta de mayor
                    if (oReg.CuentaDeMayor != sCuentaDeMayor)
                    {
                        sCuentaDeMayor = oReg.CuentaDeMayor;
                        oCuentaDeMayor = this.tlvGastos.Nodes.Add(sCuentaDeMayor);
                    }

                }
                // Se agrega la cuenta auxiliar
                if (oReg.CuentaAuxiliar != sCuentaAuxiliar)
                {
                    sCuentaAuxiliar = oReg.CuentaAuxiliar;
                    oCuentaAuxiliar = oCuentaDeMayor.Nodes.Add(sCuentaAuxiliar);
                }
                // Se agrega el egreso, si hay
                if (oReg.Egreso != null)
                {
                    oEgreso = oCuentaAuxiliar.Nodes.Add(oReg.Egreso);
                    oEgreso["Importe"] = mImporte;
                }

                // Se van llenando los totales
                oCuentaAuxiliar["Importe"] = (Util.Decimal(oCuentaAuxiliar["Importe"]) + mImporte);
                oCuentaDeMayor["Importe"] = (Util.Decimal(oCuentaDeMayor["Importe"]) + mImporte);
                if (oReg.SumaGastosFijos.Valor())
                    oGastosFijos["Importe"] = (Util.Decimal(oGastosFijos["Importe"]) + mImporte);
                
                // Se llenan las variables de gastos
                this.mGastosTotal += mImporte;
                if (oReg.GastoFijo.Valor())
                    this.mGastosFijos += mImporte;
                if (oReg.ContaCuentaDeMayorID == Cat.ContaCuentasDeMayor.Salarios)
                    this.mGastosSueldos += mImporte;
            }

            // Se ocultan los gastos fijos, si aplica
            if (!this.bVerAdicional)
                oGastosFijos.Nodes.Clear();
        }

        private void CargarComisionSemanalMensual()
        {
            var oPorSemAnt = Datos.GetListOf<MetasComisionVendedoresView>(c => c.Anio == (this.Hasta.Year - 1) && c.VendedorID == this.UsuarioID);
            var oPorSemAct = Datos.GetListOf<MetasComisionVendedoresView>(c => c.Anio == this.Hasta.Year && c.VendedorID == this.UsuarioID);
            var oPorMesAnt = oPorSemAnt.GroupBy(g => g.Mes).Select(o => new { Mes = o.Key, Comision = o.Sum(c => c.Comision) });
            var oPorMesAct = oPorSemAct.GroupBy(g => g.Mes).Select(o => new { Mes = o.Key, Comision = o.Sum(c => c.Comision) });

            // Se llenan las semanas
            decimal mComision;
            this.chrComisionSem.Series["Pasado"].Points.Clear();
            this.chrComisionSem.Series["Actual"].Points.Clear();
            for (int iSem = 1; iSem <= 52; iSem++)
            {
                // Para el año anterior
                var oRegistro = oPorSemAnt.Where(c => c.Semana == iSem).FirstOrDefault();
                mComision = (oRegistro == null ? 0 : oRegistro.Comision.Valor());
                this.chrComisionSem.Series["Pasado"].Points.Add(new DataPoint((double)iSem, (double)mComision));
                // Para el año actual
                oRegistro = oPorSemAct.Where(c => c.Semana == iSem).FirstOrDefault();
                mComision = (oRegistro == null ? 0 : oRegistro.Comision.Valor());
                this.chrComisionSem.Series["Actual"].Points.Add(new DataPoint((double)iSem, (double)mComision));
            }

            // Se llenan los meses
            this.chrComisionMensual.Series["Pasado"].Points.Clear();
            this.chrComisionMensual.Series["Actual"].Points.Clear();
            for (int iMes = 1; iMes <= 12; iMes++)
            {
                // Para el año anterior
                var oRegistro = oPorMesAnt.Where(c => c.Mes == iMes).FirstOrDefault();
                mComision = (oRegistro == null ? 0 : oRegistro.Comision.Valor());
                this.chrComisionMensual.Series["Pasado"].Points.Add(new DataPoint((double)iMes, (double)mComision));
                // Para el año actual
                oRegistro = oPorMesAct.Where(c => c.Mes == iMes).FirstOrDefault();
                mComision = (oRegistro == null ? 0 : oRegistro.Comision.Valor());
                this.chrComisionMensual.Series["Actual"].Points.Add(new DataPoint((double)iMes, (double)mComision));
            }
        }
        
        private void CargarUtilidadSem()
        {
            var oUtilAnt = Datos.GetListOf<MetasUtilidadSucursalesView>(c => c.Anio == (this.Hasta.Year - 1) && c.SucursalID == this.SucursalID);
            var oUtilAct = Datos.GetListOf<MetasUtilidadSucursalesView>(c => c.Anio == this.Hasta.Year && c.SucursalID == this.SucursalID);

            // Se llenan las semanas
            decimal mUtilidad;
            this.chrUtilidadSem.Series["Pasado"].Points.Clear();
            this.chrUtilidadSem.Series["Actual"].Points.Clear();
            for (int iSem = 1; iSem <= 52; iSem++)
            {
                // Para el año anterior
                var oRegistro = oUtilAnt.FirstOrDefault(c => c.Semana == iSem);
                mUtilidad = (oRegistro == null ? 0 : oRegistro.Utilidad.Valor());
                this.chrUtilidadSem.Series["Pasado"].Points.Add(new DataPoint((double)iSem, (double)mUtilidad));
                this.chrUtilidadSem.Series["Pasado"].Points[iSem - 1].AxisLabel = 
                    ((mUtilidad / this.oMetaSucursal.UtilSucursalLargoPlazo) * 100).ToString(GlobalClass.FormatoDecimal);
                // Para el año actual
                oRegistro = oUtilAct.FirstOrDefault(c => c.Semana == iSem);
                mUtilidad = (oRegistro == null ? 0 : oRegistro.Utilidad.Valor());
                this.chrUtilidadSem.Series["Actual"].Points.Add(new DataPoint((double)iSem, (double)mUtilidad));
                this.chrUtilidadSem.Series["Actual"].Points[iSem - 1].AxisLabel = 
                    ((mUtilidad / this.oMetaSucursal.UtilSucursalLargoPlazo) * 100).ToString(GlobalClass.FormatoDecimal);
            }
        }

        private void CargarComisionActual()
        {
            /* var oMetaVen = General.GetEntity<MetaVendedor>(c => c.VendedorID == this.UsuarioID);
            // var oMetaSuc = General.GetEntity<MetaSucursal>(c => c.SucursalID == this.SucursalID);
            // if (oMetaVen == null || oMetaSuc == null) return;
            if (oMetaVen == null) return;
            */

            this.chrComisionActual.Series["Fijo"].Points.Clear();
            this.chrComisionActual.Series["Variable"].Points.Clear();
            this.chrComisionActual.Series["9500"].Points.Clear();
            this.chrComisionActual.Series["Meta"].Points.Clear();
            this.chrComisionActual.Series["Adicional"].Points.Clear();

            this.chrComisionActual.Series["Fijo"].Points.AddY(this.oMetaVendedor.SueldoFijo);
            //this.chrComisionActual.Series["Fijo"].Points.AddY(0);
            decimal mAdicional;
            if (this.oMetaVendedor.EsGerente)
            {
                this.chrComisionActual.Series["Variable"].Points.AddY(this.mComisionGerente);
                this.chrComisionActual.Series["9500"].Points.AddY(this.mComision9500);
                this.chrComisionActual.Series["Meta"].Points.AddY(this.oMetaVendedor.SueldoMeta - this.oMetaVendedor.SueldoFijo - this.mComisionGerente - this.mComision9500);
                mAdicional = (this.mComisionGerente + this.mComision9500 + this.oMetaVendedor.SueldoFijo - this.oMetaVendedor.SueldoMeta);
            }
            else
            {
                this.chrComisionActual.Series["Variable"].Points.AddY(this.mComision);
                this.chrComisionActual.Series["9500"].Points.AddY(this.mComision9500);
                this.chrComisionActual.Series["Meta"].Points.AddY(this.oMetaVendedor.SueldoMeta - this.oMetaVendedor.SueldoFijo - this.mComision - this.mComision9500);
                mAdicional = (this.mComision + this.mComision9500 + this.oMetaVendedor.SueldoFijo - this.oMetaVendedor.SueldoMeta);
            }
            
            //Se quita el adicional
            if (mAdicional > 0)
                //this.chrComisionActual.Series["Adicional"].Points.AddY(mAdicional);
                this.chrComisionActual.Series["Adicional"].Enabled = false;
            else
                this.chrComisionActual.Series["Adicional"].Enabled = false;

            /* decimal mAdicional = (this.mComision + oMetaVen.SueldoMeta - oMetaVen.SueldoMinimo);
            if (mAdicional > 0)
                this.chrComisionActual.Series["Adicional"].Points.AddY(mAdicional);
            else
                this.chrComisionActual.Series["Adicional"].Enabled = false;
            */
        }

        private void CargarComisionActualMarca()
        {
            /* decimal mFijo = (decimal)this.chrComisionActual.Series["Fijo"].Points[0].YValues[0];
            decimal mMeta = (decimal)this.chrComisionActual.Series["Meta"].Points[0].YValues[0];
            decimal mAdicional = (this.chrComisionActual.Series["Adicional"].Points.Count > 0 ?
                (decimal)this.chrComisionActual.Series["Adicional"].Points[0].YValues[0] : 0);
            decimal mTotalBarra = (mFijo + mMeta + mAdicional);
            */

            // Se calcula el total de la barra
            decimal mTotalBarra = 0;
            foreach (var oSerie in this.chrComisionActual.Series)
            {
                if (!oSerie.Enabled) continue;
                mTotalBarra += (decimal)oSerie.Points[0].YValues[0];
            }

            // Se calcula la posición (Top) donde debe quedar la marca, según el caso
            decimal mAcumulado, mPasos;
            decimal mFijo = (decimal)this.chrComisionActual.Series["Fijo"].Points[0].YValues[0];
            mAcumulado = (mFijo + this.mComision);
            if (this.oMetaVendedor.MetaConsiderar9500)
                mAcumulado += this.mComision9500;

            // Para cuando es gerente
            if (this.oMetaVendedor.EsGerente)
            {
                decimal mComNormal = (mAcumulado - mFijo);

                decimal mPorUtil = Util.Decimal(Config.Valor("Comisiones.Vendedor.Porcentaje"));
                decimal mMetaMinima = (this.oMetaSucursal.UtilGerente * (mPorUtil / 100));
                if (this.mUtilidad < this.oMetaSucursal.UtilSucursalMinimo || mComNormal < mMetaMinima)
                    mAcumulado -= mFijo;
                else
                    mAcumulado = (mFijo + this.mComisionGerente + (this.oMetaVendedor.MetaConsiderar9500 ? this.mComision9500 : 0));
            }

            mPasos = ((mAcumulado / mTotalBarra) * (Metas.BarraMarcaInicio - Metas.BarraMarcaFin));
            mPasos = Math.Round(Metas.BarraMarcaInicio - mPasos, 0);

            // Si el top (mPasos) es menor que el final, se corrige
            if (mPasos < Metas.BarraMarcaFin)
                mPasos = Metas.BarraMarcaFin;
            // 
            int iDif = (Metas.BarraMarcaInicio - (int)mPasos);

            this.pcbBarraMarca.Top = (int)mPasos;
            this.pcbBarraMarca.Height = (28 + iDif);
            this.lblBarraMarca.Text = mAcumulado.ToString(GlobalClass.FormatoMoneda);
        }

        private void CargarComisionActualMarcaMinimo()
        {
            // Se calcula el total de la barra
            decimal mTotalBarra = 0;
            foreach (var oSerie in this.chrComisionActual.Series)
            {
                if (!oSerie.Enabled) continue;
                mTotalBarra += (decimal)oSerie.Points[0].YValues[0];
            }

            // Se calcula la posición (Top) donde debe quedar la marca, según el caso
            decimal mPorUtil = Util.Decimal(Config.Valor("Comisiones.Vendedor.Porcentaje"));
            decimal mMetaMinima = (this.oMetaVendedor.EsGerente ? this.oMetaSucursal.UtilGerente : this.oMetaSucursal.UtilVendedor);
            mMetaMinima = (mMetaMinima * (mPorUtil / 100));
            mMetaMinima += this.oMetaVendedor.SueldoFijo;  // Se le agrega el sueldo fijo

            // Para cuando es gerente
            // if (this.oMetaVendedor.EsGerente && (this.mUtilidad < this.oMetaSucursal.UtilSucursalMinimo || this.mComision < (mMetaMinima - this.oMetaVendedor.SueldoFijo)))
            if (this.oMetaVendedor.EsGerente)
            {
                mMetaMinima -= this.oMetaVendedor.SueldoFijo;  // Se quita el sueldo fijo
            }

            decimal mPasos = ((mMetaMinima / mTotalBarra) * (Metas.BarraMarcaInicio - Metas.BarraMarcaFin));
            mPasos = Math.Round(Metas.BarraMarcaInicio - mPasos, 0);

            this.pcbBarraMarcaMinimoAtras.Top = (int)mPasos;
            this.pcbBarraMarcaMinimo.Top = (this.pcbBarraMarcaMinimoAtras.Top - this.pcbBarraMarca.Top);
            // this.lblBarraMarca.Text = mAcumulado.ToString(GlobalClass.FormatoMoneda);
        }

        private void CargarMetas()
        {
            var oMetaGen = Datos.GetEntity<MetaSucursal>(c => c.SucursalID == this.SucursalID);
            if (oMetaGen == null) return;

            decimal mMeta = (this.oMetaSucursal.UtilSucursal - this.mGastosTotal);
            mMeta = ((mMeta / this.oMetaSucursal.UtilSucursalLargoPlazo) * 100);
            decimal mMarca = (mMeta / 6);
            // Se llenan las marcas del tacómetro
            /*
            this.lblMarca01.Text = (this.mGastosTotal * -1).ToString(GlobalClass.FormatoEntero);
            this.lblMarca02.Text = (this.mGastosSueldos * -1).ToString(GlobalClass.FormatoEntero);
            this.lblMarca04.Text = mMarca.ToString(GlobalClass.FormatoEntero);
            this.lblMarca05.Text = (mMarca * 2).ToString(GlobalClass.FormatoEntero);
            this.lblMarca06.Text = (mMarca * 3).ToString(GlobalClass.FormatoEntero);
            this.lblMarca07.Text = (mMarca * 4).ToString(GlobalClass.FormatoEntero);
            this.lblMarca08.Text = (mMarca * 5).ToString(GlobalClass.FormatoEntero);
            this.lblMarca09.Text = this.mMetaSucursal.ToString(GlobalClass.FormatoEntero);
            */

            this.lblMarca01.Text = (((this.mGastosTotal / this.oMetaSucursal.UtilSucursalLargoPlazo) * 100) * -1).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca02.Text = (((this.mGastosSueldos / this.oMetaSucursal.UtilSucursalLargoPlazo) * 100) * -1).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca04.Text = mMarca.ToString(GlobalClass.FormatoDecimal);
            this.lblMarca05.Text = (mMarca * 2).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca06.Text = (mMarca * 3).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca07.Text = (mMarca * 4).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca08.Text = (mMarca * 5).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca09.Text = mMeta.ToString(GlobalClass.FormatoDecimal);
        }

        private void CargarTacometro()
        {
            var oImg = Properties.Resources.MetasTacometroMarca;

            // Se obtiene el ángulo para la marca mínima
            decimal mPorGastos = (this.mGastosTotal / this.oMetaSucursal.UtilSucursalLargoPlazo);
            decimal mPorFinal = ((this.oMetaSucursal.UtilSucursal / this.oMetaSucursal.UtilSucursalLargoPlazo) - mPorGastos);
            decimal mMarcaMinimo = ((((this.oMetaSucursal.UtilSucursalMinimo / this.oMetaSucursal.UtilSucursalLargoPlazo) - mPorGastos) * Metas.RotacionMaxima) / mPorFinal);
            //decimal mMarcaMinimo = (((this.oMetaSucursal.UtilSucursalMinimo / this.oMetaSucursal.UtilSucursalLargoPlazo) * Metas.RotacionMaxima) / mPorFinal);
            this.pcbTacometroMinimo.Image = Util.RotateImage(Properties.Resources.MetasTacometroMinimo, (float)mMarcaMinimo);

            // Se obtiene el ángulo para la marca
            decimal mAnguloPos = 0;
            if (this.mUtilidadFinal > 0)
            {
                mAnguloPos = (((this.mUtilidadFinal / this.oMetaSucursal.UtilSucursalLargoPlazo) * Metas.RotacionMaxima) / mPorFinal);
            }
            else if (this.mUtilidadFinal < 0)
            {
                
                mAnguloPos = (((this.mUtilidadFinal / this.mGastosTotal) * Metas.RotacionMinima) * -1);
                //mAnguloPos = ((Metas.RotacionMinima / (this.mGastosTotal / this.mUtilidadFinal)) * -1);
            }

            this.pcbTacometroMarca.Tag = mAnguloPos;

            this.pcbTacometroMarca.Image = Util.RotateImage(oImg, (float)mAnguloPos);
            //this.lblUtilidadTotal.Text = this.mUtilidad.ToString(GlobalClass.FormatoEntero);

            this.lblUtilidadTotal.Text = ((this.mUtilidad / this.oMetaSucursal.UtilSucursalLargoPlazo) * 100).ToString(GlobalClass.FormatoDecimal);
        }

        private void CargarMetasEspecificas()
        {
            var oMetas = Datos.GetListOf<Meta>(c => c.SucursalID == this.SucursalID && (!c.VendedorID.HasValue || c.VendedorID == this.UsuarioID));
            List<VentasPartesView> oDatos;
            decimal mTotal, mCompletado, mRestante;
            this.flpDonas.Controls.Clear();  // Se quitan todas las gráficas actuales
            foreach (var oMeta in oMetas)
            {
                // Se obtienen los datos para la gráfica
                if (oMeta.ParteID.HasValue)
                {
                    oDatos = Datos.GetListOf<VentasPartesView>(c =>
                        (oMeta.VendedorID.HasValue || c.SucursalID == this.SucursalID) 
                        && (!oMeta.VendedorID.HasValue || c.VendedorID == this.UsuarioID)
                        && c.VentaEstatusID == Cat.VentasEstatus.Completada
                        && (EntityFunctions.TruncateTime(c.Fecha) >= this.Desde && c.Fecha <= this.Hasta)
                        && c.ParteID == oMeta.ParteID);
                }
                else if (oMeta.LineaID.HasValue)
                {
                    oDatos = Datos.GetListOf<VentasPartesView>(c =>
                        (oMeta.VendedorID.HasValue || c.SucursalID == this.SucursalID)
                        && (!oMeta.VendedorID.HasValue || c.VendedorID == this.UsuarioID)
                        && c.VentaEstatusID == Cat.VentasEstatus.Completada
                        && (EntityFunctions.TruncateTime(c.Fecha) >= this.Desde && c.Fecha <= this.Hasta)
                        && c.LineaID == oMeta.LineaID);
                }
                else
                {
                    oDatos = Datos.GetListOf<VentasPartesView>(c =>
                        (oMeta.VendedorID.HasValue || c.SucursalID == this.SucursalID)
                        && (!oMeta.VendedorID.HasValue || c.VendedorID == this.UsuarioID)
                        && c.VentaEstatusID == Cat.VentasEstatus.Completada
                        && (EntityFunctions.TruncateTime(c.Fecha) >= this.Desde && c.Fecha <= this.Hasta)
                        && c.MarcaID == oMeta.MarcaParteID);
                }
                mTotal = oMeta.Cantidad.Valor();
                mCompletado = (oDatos.Count > 0 ? oDatos.Sum(c => c.Cantidad) : 0);
                mRestante = (mTotal - mCompletado);
                // Se agrega la gráfica con todos sus componentes
                var oDona = this.AgregarDona(oMeta.NombreMeta, mTotal, mCompletado
                    , (Path.IsPathRooted(oMeta.RutaImagen) ? oMeta.RutaImagen : (UtilLocal.RutaImagenes() + oMeta.RutaImagen)));
            }
        }

        private void CargarMetasEspecificasGeneral()
        {
            var oMetas = Datos.GetListOf<Meta>(c => c.SucursalID == this.SucursalID)
                .GroupBy(c => new { c.MarcaParteID, c.LineaID, c.ParteID }).Select(c => new
                {
                    c.Key.MarcaParteID,
                    c.Key.LineaID,
                    c.Key.ParteID,
                    Cantidad = c.Sum(s => s.Cantidad),
                    RutaImagen = c.Min(m => m.RutaImagen),
                    NombreMeta = c.Min(m => m.NombreMeta)
                });

            var oVentas = Datos.GetListOf<VentasPartesView>(c =>
                c.SucursalID == this.SucursalID && c.VentaEstatusID == Cat.VentasEstatus.Completada
                && (c.Fecha >= this.Desde && EntityFunctions.TruncateTime(c.Fecha) <= this.Hasta));
            // Se quitan las ventas de vendedores que no sean de la sucursal
            var oVendedores = Datos.GetListOf<MetaVendedor>(c => c.SucursalID == this.SucursalID);
            for (int iCont = 0; iCont < oVentas.Count; iCont++)
            {
                var oReg = oVentas[iCont];
                if (!oVendedores.Any(c => c.VendedorID == oReg.VendedorID))
                {
                    oVentas.Remove(oReg);
                    iCont--;
                }
            }
                        
            // Se comienza a hacer los cálculos
            IEnumerable<VentasPartesView> oDatos;
            decimal mTotal, mCompletado, mRestante;
            this.flpDonas.Controls.Clear();  // Se quitan todas las gráficas actuales
            foreach (var oReg in oMetas)
            {
                // Parte
                if (oReg.ParteID.HasValue)
                {
                    oDatos = oVentas.Where(c => c.ParteID == oReg.ParteID);
                }
                // Línea
                else if (oReg.LineaID.HasValue)
                {
                    oDatos = oVentas.Where(c => c.LineaID == oReg.LineaID);
                }
                // Marca
                else
                {
                    oDatos = oVentas.Where(c => c.MarcaID == oReg.MarcaParteID);
                }

                mTotal = oReg.Cantidad.Valor();
                mCompletado = (oDatos.Count() > 0 ? oDatos.Sum(c => c.Cantidad) : 0);
                mRestante = (mTotal - mCompletado);
                // Se agrega la gráfica con todos sus componentes
                var oDona = this.AgregarDona(oReg.NombreMeta, mTotal, mCompletado
                    , (Path.IsPathRooted(oReg.RutaImagen) ? oReg.RutaImagen : (UtilLocal.RutaImagenes() + oReg.RutaImagen)));
            }

            /*
            var oMetas = General.GetListOf<Meta>(c => c.SucursalID == this.SucursalID).GroupBy(g => new { g.MarcaParteID, g.LineaID, g.ParteID }).Select(
                c => new
                {
                    c.Key.MarcaParteID,
                    c.Key.LineaID,
                    c.Key.ParteID,
                    RutaImagen = c.Min(s => s.RutaImagen),
                    NombreMeta = c.Min(s => s.NombreMeta),
                    Cantidad = c.Sum(s => s.Cantidad)
                });
            var oVendedores = General.GetListOf<MetaVendedor>(c => c.SucursalID == this.SucursalID);
            var oVentas = General.GetListOf<VentasPartesView>(c =>
                c.SucursalID == this.SucursalID && c.VentaEstatusID == Cat.VentasEstatus.Completada 
                && (EntityFunctions.TruncateTime(c.Fecha) >= this.Desde && c.Fecha <= this.Hasta));
            // Se obtienen sólo las ventas de vendedores correspondientes a la sucursal
            for (int iCont = 0; iCont < oVentas.Count; iCont++)
            {
                var oReg = oVentas[iCont];
                if (oVendedores.Any(c => c.VendedorID == oReg.VendedorID))
                {
                    oVentas.Remove(oReg);
                    iCont--;
                }
            }

            IEnumerable<VentasPartesView> oDatos;
            decimal mTotal, mCompletado, mRestante;
            this.flpDonas.Controls.Clear();  // Se quitan todas las gráficas actuales
            foreach (var oMeta in oMetas)
            {
                // Se obtienen los datos para la gráfica
                if (oMeta.ParteID.HasValue)
                {
                    oDatos = oVentas.Where(c => c.ParteID == oMeta.ParteID);
                }
                else if (oMeta.LineaID.HasValue)
                {
                    oDatos = oVentas.Where(c => c.LineaID == oMeta.LineaID);
                }
                else
                {
                    oDatos = oVentas.Where(c => c.MarcaID == oMeta.MarcaParteID);
                }
                mTotal = oMeta.Cantidad.Valor();
                mCompletado = (oDatos.Count() > 0 ? oDatos.Sum(c => c.Cantidad) : 0);
                mRestante = (mTotal - mCompletado);
                // Se agrega la gráfica con todos sus componentes
                var oDona = this.AgregarDona(oMeta.NombreMeta, mTotal, mCompletado
                    , (Path.IsPathRooted(oMeta.RutaImagen) ? oMeta.RutaImagen : (UtilLocal.RutaImagenes() + oMeta.RutaImagen)));
            }
            */
        }

        private void CargarPartesPorVentas()
        {
            var oParams = new Dictionary<string, object>();
            oParams.Add("Desde", this.Desde);
            oParams.Add("Hasta", this.Hasta);
            oParams.Add("SucursalID", (this.SucursalID > 0 ? (int?)this.SucursalID : null));
            oParams.Add("UsuarioID", (this.UsuarioID > 0 ? (int?)this.UsuarioID : null));
            var oDatos = Datos.ExecuteProcedure<pauVentasPartesPor_Result>("pauVentasPartesPor", oParams);

            float totalPartesVendidas = 0;

            foreach (var i in oDatos)
            {
                totalPartesVendidas = i.Uno + i.Dos + i.Tres + i.Mas;
            }
            
            this.dgvPartesPorVenta.Rows.Clear();
            this.dgvPartesPorVenta.Rows.Add("Una", ((oDatos[0].Uno * 100) / totalPartesVendidas).ToString("0.##") + "%");
            this.dgvPartesPorVenta.Rows.Add("Dos", ((oDatos[0].Dos * 100) / totalPartesVendidas).ToString("0.##") + "%");
            this.dgvPartesPorVenta.Rows.Add("Tres", ((oDatos[0].Tres * 100) / totalPartesVendidas).ToString("0.##") + "%");
            this.dgvPartesPorVenta.Rows.Add("Más", ((oDatos[0].Mas * 100) / totalPartesVendidas).ToString("0.##") + "%");
        }

        private Chart AgregarDona(string sTitulo, decimal mTotal, decimal mAvance, string sImagen)
        {
            var oArea = new ChartArea();
            oArea.BackColor = Color.Transparent;
            oArea.Position.Height = 100F;
            oArea.Position.Width = 80F;
            var oSerie = new Series();
            oSerie.BackGradientStyle = GradientStyle.DiagonalLeft;
            oSerie.BackSecondaryColor = Color.Transparent;
            oSerie.BorderColor = Color.White;
            oSerie.ChartType = SeriesChartType.Doughnut;
            oSerie.ShadowColor = Color.White;
            oSerie.ShadowOffset = 3;
            /* oSerie.IsValueShownAsLabel = true;
            oSerie.LabelFormat = "N0";
            oSerie.LabelForeColor = Color.White;
            oSerie.Font = new Font(oSerie.Font, FontStyle.Bold);
            */
            // oSerie.CustomProperties = "PieLabelStyle=Outside, PieLineColor=White";
            var oComp = new DataPoint(0, (double)mAvance) { Color = Color.White, CustomProperties = "Exploded=True", LegendText = "#VAL{N0}" };
            var oRest = new DataPoint(0, (double)(mTotal - mAvance)) { Color = Color.Red, CustomProperties = "Exploded=True", LegendText = "#VAL{N0}" };
            oSerie.Points.Add(oComp);
            oSerie.Points.Add(oRest);
            var oTitulo = new Title(sTitulo);
            oTitulo.ForeColor = Color.White;
            oTitulo.Docking = Docking.Bottom;
            oTitulo.Alignment = ContentAlignment.MiddleRight;
            var oLeyenda = new Legend();
            oLeyenda.BackColor = Color.Transparent;
            oLeyenda.ForeColor = Color.White;
            oLeyenda.Docking = Docking.Right;
            oLeyenda.Alignment = StringAlignment.Far;
            var oGrafica = new Chart();
            oGrafica.BackColor = Color.Transparent;
            oGrafica.Size = new Size(184, 160);
            oGrafica.ChartAreas.Add(oArea);
            oGrafica.Series.Add(oSerie);
            oGrafica.Titles.Add(oTitulo);
            oGrafica.Legends.Add(oLeyenda);
            //
            oGrafica.Controls.Add(new PictureBox()
            {
                Size = new Size(154, 160),
                Image = (File.Exists(sImagen) ? Image.FromFile(sImagen) : null),
                SizeMode = PictureBoxSizeMode.CenterImage
            });

            // oGrafica.Name = string.Format("chrDona{0:00}", this.flpDonas.Controls.Count);
            this.flpDonas.Controls.Add(oGrafica);

            return oGrafica;
        }

        private void CambiarVista(bool bModoNormal)
        {   
            this.chrComisionActual.Visible = bModoNormal;
            this.chrComisionSem.Visible = bModoNormal;
            this.chrComisionMensual.Visible = bModoNormal;

            if (bModoNormal)
                this.pcbTacometro.Left = (this.chrComisionActual.Left + this.chrComisionActual.Width + 6);
            else
                this.pcbTacometro.Left = 3;
            int iLeftAnt = this.flpDonas.Left;
            this.flpDonas.Left = (this.pcbTacometro.Left + this.pcbTacometro.Width + 6);
            this.flpDonas.Width += (iLeftAnt - this.flpDonas.Left);
        }

        private void CompletarGraficas()
        {
            decimal mUtilLargoPlazo = 0;
            if (this.oMetaSucursal == null)
            {
                var oMetasSuc = Datos.GetListOf<MetaSucursal>();
                mUtilLargoPlazo = (oMetasSuc.Count > 0 ? oMetasSuc.Sum(c => c.UtilSucursalLargoPlazo) : 0);
            }
            else
            {
                mUtilLargoPlazo = this.oMetaSucursal.UtilSucursalLargoPlazo;
            }

            // Se llenan los AXISLABEL de la gráfica de utilidad semanal
            foreach (var oSerie in this.chrUtilidadSem.Series)
            {
                foreach (var oPunto in oSerie.Points)
                {
                    oPunto.AxisLabel = (((decimal)oPunto.YValues[0] / mUtilLargoPlazo) * 100).ToString(GlobalClass.FormatoDecimal);
                }
            }
        }

        private void CargarUtilidadSemTodasLasSucursales()
        {
            var oUtilAnt = Datos.GetListOf<MetasUtilidadSucursalesView>(c => c.Anio == (this.Hasta.Year - 1));
            var oUtilAct = Datos.GetListOf<MetasUtilidadSucursalesView>(c => c.Anio == this.Hasta.Year);

            // Se obtiene la Utilidad a largo plazo, de todas las sucursales
            var oMetasSuc = Datos.GetListOf<MetaSucursal>();
            decimal mUtilLargoPlazo = (oMetasSuc.Count > 0 ? oMetasSuc.Sum(c => c.UtilSucursalLargoPlazo) : 0);

            // Se llenan las semanas
            decimal mUtilidad;
            this.chrUtilidadSem.Series["Pasado"].Points.Clear();
            this.chrUtilidadSem.Series["Actual"].Points.Clear();
            for (int iSem = 1; iSem <= 52; iSem++)
            {
                // Para el año anterior
                var oRegistro = oUtilAnt.Where(c => c.Semana == iSem).GroupBy(c => new { c.Anio, c.Mes, c.Semana })
                    .Select(c => new { c.Key.Anio, c.Key.Mes, c.Key.Semana, Utilidad = c.Sum(s => s.Utilidad) }).FirstOrDefault();
                mUtilidad = (oRegistro == null ? 0 : oRegistro.Utilidad.Valor());
                this.chrUtilidadSem.Series["Pasado"].Points.Add(new DataPoint((double)iSem, (double)mUtilidad));
                this.chrUtilidadSem.Series["Pasado"].Points[iSem - 1].AxisLabel = ((mUtilidad / mUtilLargoPlazo) * 100).ToString(GlobalClass.FormatoDecimal);
                // Para el año actual
                oRegistro = oUtilAct.Where(c => c.Semana == iSem).GroupBy(c => new { c.Anio, c.Mes, c.Semana })
                    .Select(c => new { c.Key.Anio, c.Key.Mes, c.Key.Semana, Utilidad = c.Sum(s => s.Utilidad) }).FirstOrDefault();
                mUtilidad = (oRegistro == null ? 0 : oRegistro.Utilidad.Valor());
                this.chrUtilidadSem.Series["Actual"].Points.Add(new DataPoint((double)iSem, (double)mUtilidad));
                this.chrUtilidadSem.Series["Actual"].Points[iSem - 1].AxisLabel = ((mUtilidad / mUtilLargoPlazo) * 100).ToString(GlobalClass.FormatoDecimal);
            }
        }

        private void CargarMetasTodasLasSucursales()
        {
            var oMetasSuc = Datos.GetListOf<MetaSucursal>();
            if (oMetasSuc.Count <= 0) return;

            decimal mMeta = (oMetasSuc.Sum(c => c.UtilSucursal) - this.mGastosTotal);
            decimal mUtilLargoPlazo = oMetasSuc.Sum(c => c.UtilSucursalLargoPlazo);
            mMeta = ((mMeta / mUtilLargoPlazo) * 100);
            decimal mMarca = (mMeta / 6);

            this.lblMarca01.Text = (((this.mGastosTotal / mUtilLargoPlazo) * 100) * -1).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca02.Text = (((this.mGastosSueldos / mUtilLargoPlazo) * 100) * -1).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca04.Text = mMarca.ToString(GlobalClass.FormatoDecimal);
            this.lblMarca05.Text = (mMarca * 2).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca06.Text = (mMarca * 3).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca07.Text = (mMarca * 4).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca08.Text = (mMarca * 5).ToString(GlobalClass.FormatoDecimal);
            this.lblMarca09.Text = mMeta.ToString(GlobalClass.FormatoDecimal);
        }

        private void CargarTacometroTodasLasSucursales()
        {
            var oImg = Properties.Resources.MetasTacometroMarca;

            // Se obtiene la Utilidad a largo plazo, de todas las sucursales
            var oMetasSuc = Datos.GetListOf<MetaSucursal>();
            decimal mUtilLargoPlazo = (oMetasSuc.Count > 0 ? oMetasSuc.Sum(c => c.UtilSucursalLargoPlazo) : 0);
            decimal mUtilSucs = (oMetasSuc.Count > 0 ? oMetasSuc.Sum(c => c.UtilSucursal) : 0);
            decimal mUtilMinimo = (oMetasSuc.Count > 0 ? oMetasSuc.Sum(c => c.UtilSucursalMinimo) : 0);

            // Se obtiene el ángulo para la marca mínima
            decimal mPorGastos = (this.mGastosTotal / mUtilLargoPlazo);
            decimal mPorFinal = ((mUtilSucs / mUtilLargoPlazo) - mPorGastos);
            decimal mMarcaMinimo = ((((mUtilMinimo / mUtilLargoPlazo) - mPorGastos) * Metas.RotacionMaxima) / mPorFinal);
            this.pcbTacometroMinimo.Image = Util.RotateImage(Properties.Resources.MetasTacometroMinimo, (float)mMarcaMinimo);

            // Se obtiene el ángulo para la marca
            decimal mAnguloPos = 0;
            if (this.mUtilidadFinal > 0)
            {
                mAnguloPos = (((this.mUtilidadFinal / mUtilLargoPlazo) * Metas.RotacionMaxima) / mPorFinal);
            }
            else if (this.mUtilidadFinal < 0)
            {

                mAnguloPos = (((this.mUtilidadFinal / this.mGastosTotal) * Metas.RotacionMinima) * -1);
                //mAnguloPos = ((Metas.RotacionMinima / (this.mGastosTotal / this.mUtilidadFinal)) * -1);
            }

            this.pcbTacometroMarca.Tag = mAnguloPos;

            this.pcbTacometroMarca.Image = Util.RotateImage(oImg, (float)mAnguloPos);
            // this.lblUtilidadTotal.Text = this.mUtilidad.ToString(GlobalClass.FormatoEntero);

            this.lblUtilidadTotal.Text = ((this.mUtilidad / mUtilLargoPlazo) * 100).ToString(GlobalClass.FormatoDecimal);
        }

        private void CargarMetasEspecificasTodasLasSucursales()
        {
            var oMetas = Datos.GetListOf<Meta>()
                .GroupBy(c => new { c.MarcaParteID, c.LineaID, c.ParteID }).Select(c => new
                {
                    c.Key.MarcaParteID,
                    c.Key.LineaID,
                    c.Key.ParteID,
                    Cantidad = c.Sum(s => s.Cantidad),
                    RutaImagen = c.Min(m => m.RutaImagen),
                    NombreMeta = c.Min(m => m.NombreMeta)
                });
            var oVentas = Datos.GetListOf<VentasPartesView>(c => c.VentaEstatusID == Cat.VentasEstatus.Completada 
                && (c.Fecha >= this.Desde && EntityFunctions.TruncateTime(c.Fecha) <= this.Hasta));
            
            // Se comienza a hacer los cálculos
            IEnumerable<VentasPartesView> oDatos;
            decimal mTotal, mCompletado, mRestante;
            this.flpDonas.Controls.Clear();  // Se quitan todas las gráficas actuales
            foreach (var oReg in oMetas)
            {
                // Parte
                if (oReg.ParteID.HasValue)
                {
                    oDatos = oVentas.Where(c => c.ParteID == oReg.ParteID);
                }
                // Línea
                else if (oReg.LineaID.HasValue)
                {
                    oDatos = oVentas.Where(c => c.LineaID == oReg.LineaID);
                }
                // Marca
                else
                {
                    oDatos = oVentas.Where(c => c.MarcaID == oReg.MarcaParteID);
                }

                mTotal = oReg.Cantidad.Valor();
                mCompletado = (oDatos.Count() > 0 ? oDatos.Sum(c => c.Cantidad) : 0);
                mRestante = (mTotal - mCompletado);
                // Se agrega la gráfica con todos sus componentes
                var oDona = this.AgregarDona(oReg.NombreMeta, mTotal, mCompletado
                    , (Path.IsPathRooted(oReg.RutaImagen) ? oReg.RutaImagen : (UtilLocal.RutaImagenes() + oReg.RutaImagen)));
            }
        }

        #endregion

        #region [ Públicos ]

        public void CargarDatos()
        {
            this.CambiarVista(true);
            this.Refresh();

            // Se cargan los datos y se llenan las gráficas
            this.CargarArbolDeGastos();
            this.CalcularUtilidadComision();

            this.CargarComisionSemanalMensual();
            this.CargarUtilidadSem();

            this.CargarComisionActual();
            this.CargarComisionActualMarca();
            this.CargarComisionActualMarcaMinimo();

            this.CargarMetas();
            this.CargarTacometro();

            this.CargarMetasEspecificas();

            this.CargarPartesPorVentas();

            this.Refresh();

            // Se inicializan las clases para animación de las gráficas
            var oComisionSemanal = new MetasAnimaciones.Lineal(this.chrComisionSem);
            var oComisionMensual = new MetasAnimaciones.Barras(this.chrComisionMensual);
            var oComisionActual = new MetasAnimaciones.BarrasApiladas(this.chrComisionActual);
            var oComisionActualMarca = new MetasAnimaciones.ComisionActualMarca(this.pcbBarraMarca);
            var oTacometro = new MetasAnimaciones.Tacometro(this.pcbTacometro);
            var oUtilidadSemanal = new MetasAnimaciones.Lineal(this.chrUtilidadSem);
            var oDonas = new MetasAnimaciones.Donas(this.flpDonas);
            var oBorde = new MetasAnimaciones.Borde(this);

            // Se muestran las gráficas, con valores de "cero"
            //this.pnlReferenciaGastos.Visible = true;
            this.chrComisionSem.Visible = true;
            this.chrComisionMensual.Visible = true;
            this.chrComisionActual.Visible = true;
            this.pcbTacometro.Visible = true;
            this.flpDonas.Visible = true;

            // Se hacen las animacíones
            while (!(oComisionSemanal.Completada && oComisionMensual.Completada && oComisionActual.Completada && oComisionActualMarca.Completada
                && oTacometro.Completada && oUtilidadSemanal.Completada && oDonas.Completada && oBorde.Completada))
            {
                if (!oComisionSemanal.Completada)
                    oComisionSemanal.EjecutarPaso();
                if (!oComisionMensual.Completada)
                    oComisionMensual.EjecutarPaso();
                if (!oComisionActual.Completada)
                    oComisionActual.EjecutarPaso();
                if (!oComisionActualMarca.Completada)
                    oComisionActualMarca.EjecutarPaso();
                if (!oTacometro.Completada)
                    oTacometro.EjecutarPaso();
                if (!oUtilidadSemanal.Completada)
                    oUtilidadSemanal.EjecutarPaso();
                if (!oDonas.Completada)
                    oDonas.EjecutarPaso();
                if (!oBorde.Completada)
                    oBorde.EjecutarPaso();
            }

            // Se completan las gráficas
            this.CompletarGraficas();
        }

        public void CargarDatosGeneral()
        {
            this.CambiarVista(false);
            this.Refresh();

            // Se cargan los datos y se llenan las gráficas
            this.CargarArbolDeGastos();
            this.CalcularUtilidadComision();

            this.CargarUtilidadSem();
                        
            this.CargarMetas();
            this.CargarTacometro();

            this.CargarMetasEspecificasGeneral();

            this.CargarPartesPorVentas();

            this.Refresh();

            // Se inicializan las clases para animación de las gráficas
            var oTacometro = new MetasAnimaciones.Tacometro(this.pcbTacometro);
            var oUtilidadSemanal = new MetasAnimaciones.Lineal(this.chrUtilidadSem);
            var oDonas = new MetasAnimaciones.Donas(this.flpDonas);
            var oBorde = new MetasAnimaciones.Borde(this);

            // Se muestran las gráficas, con valores de "cero"
            //this.pnlReferenciaGastos.Visible = true;
            this.pcbTacometro.Visible = true;
            this.flpDonas.Visible = true;

            // Se hacen las animacíones
            while (!(oTacometro.Completada && oUtilidadSemanal.Completada && oDonas.Completada && oBorde.Completada))
            {
                if (!oTacometro.Completada)
                    oTacometro.EjecutarPaso();
                if (!oUtilidadSemanal.Completada)
                    oUtilidadSemanal.EjecutarPaso();
                if (!oDonas.Completada)
                    oDonas.EjecutarPaso();
                if (!oBorde.Completada)
                    oBorde.EjecutarPaso();
            }

            // Se completan las gráficas
            this.CompletarGraficas();
        }

        public void CargarDatosTodasLasSucursales()
        {
            this.CambiarVista(false);
            this.Refresh();

            // Se cargan los datos y se llenan las gráficas
            this.CargarArbolDeGastos();
            this.CalcularUtilidadComision();

            // this.CargarUtilidadSem();
            this.CargarUtilidadSemTodasLasSucursales();

            // this.CargarMetas();
            // this.CargarTacometro();
            this.CargarMetasTodasLasSucursales();
            this.CargarTacometroTodasLasSucursales();

            // this.CargarMetasEspecificasGeneral();
            this.CargarMetasEspecificasTodasLasSucursales();

            this.CargarPartesPorVentas();

            this.Refresh();

            // Se inicializan las clases para animación de las gráficas
            var oTacometro = new MetasAnimaciones.Tacometro(this.pcbTacometro);
            var oUtilidadSemanal = new MetasAnimaciones.Lineal(this.chrUtilidadSem);
            var oDonas = new MetasAnimaciones.Donas(this.flpDonas);
            var oBorde = new MetasAnimaciones.Borde(this);

            // Se muestran las gráficas, con valores de "cero"
            //this.pnlReferenciaGastos.Visible = true;
            this.pcbTacometro.Visible = true;
            this.flpDonas.Visible = true;

            // Se hacen las animacíones
            while (!(oTacometro.Completada && oUtilidadSemanal.Completada && oDonas.Completada && oBorde.Completada))
            {
                if (!oTacometro.Completada)
                    oTacometro.EjecutarPaso();
                if (!oUtilidadSemanal.Completada)
                    oUtilidadSemanal.EjecutarPaso();
                if (!oDonas.Completada)
                    oDonas.EjecutarPaso();
                if (!oBorde.Completada)
                    oBorde.EjecutarPaso();
            }

            // Se completan las gráficas
            this.CompletarGraficas();
        }

        public void Preparar(int iUsuarioID)
        {
            this.UsuarioID = iUsuarioID;
            // Se cargan datos de sucursal y usuario
            this.oMetaSucursal = Datos.GetEntity<MetaSucursal>(c => c.SucursalID == this.SucursalID);
            this.oMetaVendedor = Datos.GetEntity<MetaVendedor>(c => c.VendedorID == this.UsuarioID);
        }

        #endregion

        private void dgvPartesPorVenta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }

    class MetasAnimaciones
    {
        const int ComisionMensualPaso = 40;//20;
        const int BarraPaso = 12;//4;
        const int BarraMarcaInicio = 333;//331;
        const int BarraMarcaFin = 8;//8;
        const int BarraMarcaPaso = 8;//2
        const int RotacionMinima = -45;
        const int RotacionMaxima = 180;
        const int RotacionPaso = 4;//2

        public class Borde
        {
            Control oControl;
            int iPasoHor1;
            int iPasoVer1;
            int iPasoHor2;
            int iPasoVer2;
            int iPasosTotal;
            Graphics oGraf;

            public bool Completada;

            public Borde(Control oControl)
            {
                this.oControl = oControl;
                this.iPasosTotal = (oControl.Width + oControl.Height);
                this.iPasoHor1 = 0;
                this.iPasoVer1 = 0;
                this.iPasoHor2 = 0;
                this.iPasoVer2 = 0;

                this.oGraf = Graphics.FromHwnd(this.oControl.Handle);
                this.Completada = false;
            }

            public bool EjecutarPaso()
            {
                // Se dibuja animación uno, ¬
                if (++this.iPasoHor1 < this.oControl.Width)
                    this.oGraf.DrawLine(Pens.White, (this.iPasoHor1 - 1), 0, this.iPasoHor1, 0);
                else if (++this.iPasoVer1 < this.oControl.Height)
                    this.oGraf.DrawLine(Pens.White, (this.oControl.Width - 1), (this.iPasoVer1 - 1), (this.oControl.Width - 1), this.iPasoVer1);
                // Se dibuja animación uno, L
                if (++this.iPasoVer2 < this.oControl.Height)
                    this.oGraf.DrawLine(Pens.White, 0, (this.iPasoVer2 - 1), 0, this.iPasoVer2);
                else if (++this.iPasoHor2 < this.oControl.Width)
                    this.oGraf.DrawLine(Pens.White, (this.iPasoHor2 - 1), (this.oControl.Height - 1), this.iPasoHor2, (this.oControl.Height - 1));
                else
                    this.Completada = true;

                return !this.Completada;
            }
        }

        public class Lineal
        {
            Chart oGrafica;
            Dictionary<int, double> oAnterior;
            Dictionary<int, double> oActual;
            int iPaso;

            public bool Completada;

            public Lineal(Chart oGrafica)
            {
                this.CargarDatos(oGrafica);
            }

            public void CargarDatos(Chart oGrafica)
            {
                this.oGrafica = oGrafica;
                this.oAnterior = new Dictionary<int, double>();
                this.oActual = new Dictionary<int, double>();

                // Se llenan los diccionarios con los datos del gráfico
                foreach (var oPunto in this.oGrafica.Series["Pasado"].Points)
                    oAnterior.Add((int)oPunto.XValue, oPunto.YValues[0]);
                foreach (var oPunto in this.oGrafica.Series["Actual"].Points)
                    oActual.Add((int)oPunto.XValue, oPunto.YValues[0]);

                this.iPaso = 0;

                this.oGrafica.Series["Pasado"].Points.Clear();
                this.oGrafica.Series["Actual"].Points.Clear();

                this.Completada = false;
            }

            public bool EjecutarPaso()
            {
                int iPaso = ++this.iPaso;
                double mValor;
                // Para el año anterior
                mValor = (this.oAnterior.ContainsKey(iPaso) ? this.oAnterior[iPaso] : 0);
                this.oGrafica.Series["Pasado"].Points.Add(new DataPoint(iPaso, mValor));
                // Para el año actual
                mValor = (this.oActual.ContainsKey(iPaso) ? this.oActual[iPaso] : 0);
                this.oGrafica.Series["Actual"].Points.Add(new DataPoint(iPaso, mValor));

                this.oGrafica.Refrescar();
                //Thread.Sleep(MetasAnimaciones.ComisionSemanalEspera);

                this.Completada = !(iPaso < this.oAnterior.Count && iPaso < this.oActual.Count);
                return !this.Completada;
            }
        }

        public class Barras
        {
            Chart oGrafica;
            Dictionary<int, double> oAnterior;
            Dictionary<int, double> oActual;
            int iPaso;

            public bool Completada;

            public Barras(Chart oGrafica)
            {
                this.CargarDatos(oGrafica);
            }

            public void CargarDatos(Chart oGrafica)
            {
                this.oGrafica = oGrafica;
                this.oAnterior = new Dictionary<int, double>();
                this.oActual = new Dictionary<int, double>();

                // Se llenan los diccionarios con los datos del gráfico
                foreach (var oPunto in this.oGrafica.Series["Pasado"].Points)
                    oAnterior.Add((int)oPunto.XValue, oPunto.YValues[0]);
                foreach (var oPunto in this.oGrafica.Series["Actual"].Points)
                    oActual.Add((int)oPunto.XValue, oPunto.YValues[0]);

                this.iPaso = 0;
                                
                // Se generan los puntos de la gráfica de meses
                this.oGrafica.Series["Pasado"].Points.Clear();
                this.oGrafica.Series["Actual"].Points.Clear();
                for (int iMes = 1; iMes <= 12; iMes++)
                {
                    // Para el año anterior
                    this.oGrafica.Series["Pasado"].Points.Add(new DataPoint((double)iMes, 0));
                    if (!this.oAnterior.ContainsKey(iMes))
                        this.oAnterior.Add(iMes, 0);
                    // Para el año actual
                    this.oGrafica.Series["Actual"].Points.Add(new DataPoint((double)iMes, 0));
                    if (!this.oActual.ContainsKey(iMes))
                        this.oActual.Add(iMes, 0);
                }
                double mMaxAnt = (this.oAnterior.Count > 0 ? this.oAnterior.Max(c => c.Value) : 0);
                double mMaxAct = (this.oActual.Count > 0 ? this.oActual.Max(c => c.Value) : 0);
                this.oGrafica.ChartAreas[0].AxisY.ScaleView.Size = (mMaxAnt > mMaxAct ? mMaxAnt : mMaxAct);

                this.Completada = false;
            }

            public bool EjecutarPaso()
            {
                this.iPaso += MetasAnimaciones.ComisionMensualPaso;
                bool bPendiente = false;
                for (int iMes = 1; iMes <= 12; iMes++)
                {
                    // Año anterior
                    if (this.oGrafica.Series["Pasado"].Points[iMes - 1].YValues[0] < this.oAnterior[iMes])
                    {
                        this.oGrafica.Series["Pasado"].Points[iMes - 1].SetValueY(this.iPaso);
                        bPendiente = true;
                    }
                    // Año actual
                    if (this.oGrafica.Series["Actual"].Points[iMes - 1].YValues[0] < this.oActual[iMes])
                    {
                        this.oGrafica.Series["Actual"].Points[iMes - 1].SetValueY(this.iPaso);
                        bPendiente = true;
                    }
                }

                // Se ajustan los valores, por si se pasaron algunos. Sólo cuando ya se completó
                if (bPendiente)
                    this.oGrafica.Refrescar();
                else
                    this.CompletarAnimacion();

                this.Completada = !bPendiente;
                return bPendiente;
            }

            private void CompletarAnimacion()
            {
                for (int iMes = 1; iMes <= 12; iMes++)
                {
                    this.oGrafica.Series["Pasado"].Points[iMes - 1].SetValueY(this.oAnterior[iMes]);
                    this.oGrafica.Series["Actual"].Points[iMes - 1].SetValueY(this.oActual[iMes]);
                }
                this.oGrafica.Refrescar();
            }
        }

        public class BarrasApiladas
        {
            Chart oGrafica;
            int iSerie;
            List<double> oValores;
            int iPaso;
            double mPasoAcum;

            public bool Completada;

            public BarrasApiladas(Chart oGrafica)
            {
                this.CargarDatos(oGrafica);
            }

            public void CargarDatos(Chart oGrafica)
            {
                this.oGrafica = oGrafica;

                this.iPaso = (int)this.oGrafica.Series[0].Points[0].YValues[0];
                this.iSerie = 0;
                this.oValores = new List<double>();
                this.oValores.Add(this.oGrafica.Series[0].Points[0].YValues[0]);

                for (int i = 1; i < this.oGrafica.Series.Count; i++)
                {
                    if (this.oGrafica.Series[i].Points.Count > 0)
                        this.oValores.Add(this.oGrafica.Series[i].Points[0].YValues[0]);
                    else
                        this.oValores.Add(0);
                    this.oGrafica.Series[i].Enabled = false;
                    this.oGrafica.Series[i].Points.Clear();
                    this.oGrafica.Series[i].Points.AddY(0);
                }

                this.Completada = false;
            }

            public bool EjecutarPaso()
            {
                int iPaso = (this.iPaso += MetasAnimaciones.BarraPaso);

                if (iPaso < this.oValores[this.iSerie])
                {  // Se continúa con la animación
                    this.oGrafica.Series[this.iSerie].Points[0].SetValueY(iPaso);
                    double mPasoAcum = (this.mPasoAcum + iPaso);
                    // this.oGrafica.Series[this.iSerie].Label = mPasoAcum.ToString(GlobalClass.FormatoMoneda);
                }
                else
                {  // Ya se completó la serie
                    this.oGrafica.Series[this.iSerie].Points[0].SetValueY(this.oValores[this.iSerie]);
                    this.mPasoAcum += this.oValores[this.iSerie];
                    // this.oGrafica.Series[this.iSerie].Label = this.mPasoAcum.ToString(GlobalClass.FormatoMoneda);
                    if (this.iSerie < (this.oValores.Count - 1))
                    {  // Hay más series que dibujar
                        this.iSerie++;
                        this.oGrafica.Series[this.iSerie].Enabled = (this.oValores[this.iSerie] > 0);
                        this.iPaso = 0;
                    }
                    else
                    {  // Ya no hay más series
                        this.Completada = true;
                    }
                }

                this.oGrafica.Refrescar();

                return !this.Completada;
            }
        }

        public class ComisionActualMarca
        {
            Chart oGrafica;
            PictureBox pcbBarraMarca;
            Label lblBarraMarca;

            bool bAnimacionInicial;
            int iPaso;
            int iAltoFinal;
            double mImporte;
            double mImporteInc;
            double mTotalBarra;
            double mAcumulado;

            public bool Completada;

            public ComisionActualMarca(PictureBox pcbMarca)
            {
                this.CargarDatos(pcbMarca);
            }

            public void CargarDatos(PictureBox pcbMarca)
            {
                this.pcbBarraMarca = pcbMarca;
                this.lblBarraMarca = (pcbMarca.Controls["lblBarraMarca"] as Label);
                this.oGrafica = (pcbMarca.Parent as Chart);

                this.bAnimacionInicial = true;
                this.iPaso = MetasAnimaciones.BarraMarcaInicio;
                this.mImporte = 0;

                // Se calcula el total de la barra
                this.mTotalBarra = 0;
                foreach (var oSerie in this.oGrafica.Series)
                    this.mTotalBarra += oSerie.Points[0].YValues[0];

                this.mImporteInc = (this.mTotalBarra / 
                    ((MetasAnimaciones.BarraMarcaInicio - MetasAnimaciones.BarraMarcaFin) / MetasAnimaciones.BarraMarcaPaso));
                this.iAltoFinal = this.pcbBarraMarca.Top;
                this.mAcumulado = Util.Doble(this.lblBarraMarca.Text.SoloNumeric());

                this.Completada = false;
            }

            public bool EjecutarPaso()
            {
                if (this.bAnimacionInicial)
                {
                    int iPaso = (this.iPaso -= MetasAnimaciones.BarraMarcaPaso);
                    if (iPaso > MetasAnimaciones.BarraMarcaFin)
                    {
                        this.mImporte += this.mImporteInc;
                    }
                    else
                    {
                        this.iPaso = MetasAnimaciones.BarraMarcaFin;
                        this.mImporte = this.mTotalBarra;
                        this.bAnimacionInicial = false;
                    }
                    this.pcbBarraMarca.Top = this.iPaso;
                    this.lblBarraMarca.Text = this.mImporte.ToString(GlobalClass.FormatoMoneda);
                }
                else
                {
                    int iPaso = (this.iPaso += MetasAnimaciones.BarraMarcaPaso);
                    if (iPaso < this.iAltoFinal)
                    {
                        this.mImporte -= this.mImporteInc;
                    }
                    else
                    {
                        this.iPaso = this.iAltoFinal;
                        this.mImporte = this.mAcumulado;
                        this.Completada = true;
                    }
                    this.pcbBarraMarca.Top = this.iPaso;
                    this.lblBarraMarca.Text = this.mImporte.ToString(GlobalClass.FormatoMoneda);
                }

                this.oGrafica.Refrescar();

                return !this.Completada;
            }
        }

        public class Tacometro
        {
            PictureBox pcbTacometro;
            PictureBox pcbTacometroMarca;
            Label lblUtilidadTotal;
            Image oFlecha;

            bool bAnimacionInicial;
            int iPaso;
            decimal mAnguloFinal;
            decimal mImporte;
            decimal mImporteInc;
            decimal mUtilidad;

            public bool Completada;

            public Tacometro(PictureBox pcbTacometro)
            {
                this.CargarDatos(pcbTacometro);
            }

            public void CargarDatos(PictureBox pcbTacometro)
            {
                // Tacometro.oGrafica = oGrafica;
                this.pcbTacometro = pcbTacometro;
                this.pcbTacometroMarca = (pcbTacometro.Controls["pcbTacometroMarca"] as PictureBox);
                this.lblUtilidadTotal = (this.pcbTacometroMarca.Controls["lblUtilidadTotal"] as Label);
                this.oFlecha = Properties.Resources.MetasTacometroMarca;

                decimal mAnguloFinal = Util.Decimal(this.pcbTacometroMarca.Tag);
                decimal mPasos = (((MetasAnimaciones.RotacionMaxima - MetasAnimaciones.RotacionMinima) + (MetasAnimaciones.RotacionMaxima - mAnguloFinal)) 
                    / MetasAnimaciones.RotacionPaso);
                decimal mUtilidad = Util.Decimal(this.lblUtilidadTotal.Text.SoloNumeric());

                this.bAnimacionInicial = true;
                this.iPaso = MetasAnimaciones.RotacionMinima - MetasAnimaciones.RotacionPaso;
                this.mAnguloFinal = mAnguloFinal;
                this.mImporte = 0;
                this.mImporteInc = (mUtilidad / mPasos);
                this.mUtilidad = mUtilidad;

                this.Completada = false;
            }

            public bool EjecutarPaso()
            {
                if (this.bAnimacionInicial)
                {  // Animación inicial, de principio a fin
                    int iPaso = (this.iPaso += MetasAnimaciones.RotacionPaso);
                    if (iPaso < MetasAnimaciones.RotacionMaxima)
                    {  // Se continúa con la animación inicial
                        this.mImporte += this.mImporteInc;
                        this.lblUtilidadTotal.Text = this.mImporte.ToString(GlobalClass.FormatoDecimal);
                    }
                    else
                    {  // Se terminó la animación inicial. Se continuará con la animación final
                        this.iPaso = MetasAnimaciones.RotacionMaxima;
                        this.bAnimacionInicial = false;
                    }
                    this.pcbTacometroMarca.Image = Util.RotateImage(this.oFlecha, iPaso);
                }
                else
                {  // Animación final, de fin a valor correspondiente
                    int iPaso = (this.iPaso -= MetasAnimaciones.RotacionPaso);
                    if (iPaso > this.mAnguloFinal)
                    {  // Se continúa con la animación final
                        this.pcbTacometroMarca.Image = Util.RotateImage(this.oFlecha, iPaso);
                        this.mImporte += this.mImporteInc;
                    }
                    else
                    {  // Se completó la animación final
                        this.pcbTacometroMarca.Image = Util.RotateImage(this.oFlecha, (float)this.mAnguloFinal);
                        this.mImporte = this.mUtilidad;
                        this.Completada = true;
                    }
                    this.lblUtilidadTotal.Text = this.mImporte.ToString(GlobalClass.FormatoDecimal);
                }

                this.pcbTacometro.Refresh();

                return !this.Completada;
            }
        }

        public class Donas
        {
            Control oContenedor;
            List<Chart> oGraficas;
            int iPaso;
            class DonaPasos
            {
                public double mTotal;
                public double mCompletado;
                public bool bAnimacionCompleta;
            }
            List<DonaPasos> oPasos;

            public bool Completada;

            public Donas(Control oContenedor)
            {
                this.CargarDatos(oContenedor);
            }

            public void CargarDatos(Control oContenedor)
            {
                this.oContenedor = oContenedor;
                this.oGraficas = new List<Chart>();
                this.oPasos = new List<DonaPasos>();

                foreach (Control oControl in oContenedor.Controls)
                {
                    if (oControl is Chart)
                    {
                        Chart oDona = (oControl as Chart);
                        DonaPasos oDonaPasos = new DonaPasos()
                        {
                            mTotal = (oDona.Series[0].Points[0].YValues[0] + oDona.Series[0].Points[1].YValues[0]),
                            mCompletado = oDona.Series[0].Points[0].YValues[0]
                        };
                        
                        this.oGraficas.Add(oDona);
                        this.oPasos.Add(oDonaPasos);
                        oDona.Series[0].Points[0].SetValueY(0);
                        oDona.Series[0].Points[1].SetValueY(oDonaPasos.mCompletado);
                    }
                }
                this.iPaso = 0;

                this.Completada = false;
            }

            public bool EjecutarPaso()
            {
                bool bPendiente = false;
                int iPaso = (this.iPaso += 1);
                for (int i = 0; i < this.oGraficas.Count; i++)
                {
                    var oDona = this.oGraficas[i];
                    var oDonaPasos = this.oPasos[i];

                    if (oDonaPasos.bAnimacionCompleta) continue;

                    if (iPaso < oDonaPasos.mCompletado)
                    {  // Se continúa con la animación
                        oDona.Series[0].Points[0].SetValueY(iPaso);
                        oDona.Series[0].Points[1].SetValueY(oDonaPasos.mTotal - iPaso);
                        bPendiente = true;
                    }
                    else
                    {  // Se acabó la animación de la dona actual
                        oDona.Series[0].Points[0].SetValueY(oDonaPasos.mCompletado);
                        oDona.Series[0].Points[1].SetValueY(oDonaPasos.mTotal - oDonaPasos.mCompletado);
                        oDonaPasos.bAnimacionCompleta = true;
                    }
                    oDona.Refrescar();
                }

                this.Completada = !bPendiente;
                return !this.Completada;
            }
        }
    }
}

