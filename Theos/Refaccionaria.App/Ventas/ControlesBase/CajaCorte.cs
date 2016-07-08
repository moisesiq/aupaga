using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

using TheosProc;
using LibUtil;

namespace Refaccionaria.App
{
    public partial class CajaCorte : UserControl
    {
        public int Paso = 0;
        public CajaConteoPagos ctlConteo;
        // public CajaFacturaGlobal ctlFacturaGlobal;

        public CajaCorte()
        {
            InitializeComponent();
        }

        #region [ Propiedades ]

        public decimal Total { get; set; }
        public decimal Conteo { get; set; }
        public decimal Diferencia { get; set; }
        
        #endregion

        #region [ Eventos ]

        private void CajaCorte_Load(object sender, EventArgs e)
        {
            this.ActualizarDatos();

            this.dgvDetalle.ClearSelection();
        }

        private void dgvDetalle_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (!e.Row.Selected) return;
            foreach (DataGridViewCell Celda in e.Row.Cells)
                Celda.Style.SelectionForeColor = Celda.InheritedStyle.ForeColor;
        }

        private void dgvDetalle_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvDetalle.CurrentRow == null) return;
            if (Util.Cadena(this.dgvDetalle.CurrentRow.Cells["Contenido"].Value).Trim().ToLower() == "hay en caja")
            {
                if (this.ctlConteo == null)
                {
                    this.ctlConteo = new CajaConteoPagos() { Dock = DockStyle.Fill, MostrarTotal = true };
                    this.Parent.Controls.Add(this.ctlConteo);
                }
                this.ctlConteo.BringToFront();
                this.Paso = 1;
            }
        }

        private void dgvDetalle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.dgvDetalle_DoubleClick(sender, e);
                e.Handled = true;
            }
        }

        #endregion

        #region [ Métodos de uso interno ]

        protected virtual int AgregarLineaTitulo(string sTexto, string sTotal)
        {
            sTexto = sTexto.ToUpper();
            if (sTotal != null)
                sTotal = sTotal.ToUpper();
            int iFila = this.dgvDetalle.Rows.Add("Titulo", null, sTexto, sTotal);
            this.dgvDetalle.Rows[iFila].DefaultCellStyle.Font = new Font(this.dgvDetalle.DefaultCellStyle.Font.FontFamily, (float)10, FontStyle.Bold);
            this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.CadetBlue;
            return iFila;
        }

        protected virtual int AgregarLineaTitulo(string sTexto)
        {
            return this.AgregarLineaTitulo(sTexto, null);
        }

        protected virtual int AgregarLineaEncabezado(string sContenido, string sTotal)
        {
            sContenido = sContenido.ToUpper();
            sTotal = sTotal.ToUpper();
            int iFila = this.dgvDetalle.Rows.Add("Encabezado", null, sContenido, sTotal);
            this.dgvDetalle.Rows[iFila].DefaultCellStyle.Font = new Font(this.dgvDetalle.DefaultCellStyle.Font, FontStyle.Bold);
            return iFila;
        }

        protected virtual int AgregarLineaDetalle(int iPendientes, string sTexto, string sTotal)
        {
            sTexto = sTexto.ToUpper();
            int iFila = this.dgvDetalle.Rows.Add("Detalle", string.Format("[{0}]", iPendientes), sTexto, sTotal);

            if (iPendientes > 0)
                this.dgvDetalle.Rows[iFila].Cells["Pendientes"].Style.ForeColor = Color.Red;

            this.dgvDetalle.Rows[iFila].Height = 17;
            return iFila;
        }

        protected virtual int AgregarLineaEspacio()
        {
            int iFila = this.dgvDetalle.Rows.Add("Espacio");
            this.dgvDetalle.Rows[iFila].Height = 14;
            return iFila;
        }

        #endregion

        #region [ Públicos ]
        
        public void ActualizarDatos()
        {
            // Se muestra la ventana de "Cargando.."
            Cargando.Mostrar();
                        
            this.dgvDetalle.Rows.Clear();
            this.Total = 0;
            decimal mTickets = 0, mFacturas = 0;
            int iFila;
            var oParams = new Dictionary<string, object>();
            oParams.Add("SucursalID", GlobalClass.SucursalID);
            oParams.Add("Dia", DateTime.Now.Date);

            // 
            var Corte = Datos.ExecuteProcedure<pauCajaCorte_Result>("pauCajaCorte", oParams);
            
            // Se comienza a llenar los datos
            foreach (var oMov in Corte)
            {
                switch (oMov.Orden)
                {
                    // Fondo de caja
                    case 1:
                        iFila = this.AgregarLineaTitulo(oMov.Concepto);
                        this.dgvDetalle["Totales", iFila].Value = ("$" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(10));

                        // this.AgregarLineaTitulo("Ingresos");
                        this.AgregarLineaTitulo(
                            "Ingresos".PadRight(20 + 1)
                            + "Tickets".PadRight(12 + 1)
                            + "Facturas".PadRight(12 + 1)
                            + "Suma".PadRight(12 + 1)
                        , "Total");
                        this.Total += oMov.Total.Valor();

                        break;
                    // Pagos que cuentan en el corte
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        this.AgregarLineaDetalle(
                            oMov.Pendientes.Valor()
                            , oMov.Concepto.RellenarCortarDerecha(20)
                                + " $" + oMov.Tickets.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Facturas.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " [+]"
                            , ""
                        );
                        mTickets += oMov.Tickets.Valor();
                        mFacturas += oMov.Facturas.Valor();
                        this.Total += (oMov.Tickets.Valor() + oMov.Facturas.Valor());

                        if (oMov.Orden == 6)
                        {
                            this.AgregarLineaEncabezado(
                                " ".PadRight(20)
                                    + " $" + mTickets.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                    + " $" + mFacturas.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                , "$" + (mTickets + mFacturas).ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(12)
                            );
                            mTickets = mFacturas = 0;
                        }

                        break;
                    // Refuerzos / Otros ingresos
                    case 7:
                    case 8:
                        iFila = this.AgregarLineaDetalle(
                            oMov.Pendientes.Valor()
                            , oMov.Concepto.RellenarCortarDerecha(46)
                                + " $" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " [+]"
                            , "$" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(12)
                        );
                        this.Total += oMov.Total.Valor();
                        break;
                    // Pagos con notas de crédito y ventas a crédito
                    case 9:
                    case 10:
                        this.AgregarLineaDetalle(
                            oMov.Pendientes.Valor()
                            , oMov.Concepto.RellenarCortarDerecha(20)
                                + " $" + oMov.Tickets.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Facturas.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                            , ""
                        );

                        if (oMov.Orden == 10)
                        {
                            // this.AgregarLineaTitulo("Egresos");
                            this.AgregarLineaTitulo(
                                "Egresos".PadRight(20 + 1)
                                + "Tickets".PadRight(12 + 1)
                                + "Facturas".PadRight(12 + 1)
                                + "Suma".PadRight(12 + 1)
                            , "Total");
                        }

                        break;
                    // Devoluciones
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        iFila = this.AgregarLineaDetalle(
                            oMov.Pendientes.Valor()
                            , oMov.Concepto.RellenarCortarDerecha(20)
                                + " $" + oMov.Tickets.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Facturas.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                            , ""
                        );
                        mTickets += oMov.Tickets.Valor();
                        mFacturas += oMov.Facturas.Valor();

                        if (oMov.Orden == 12 || oMov.Orden == 14)
                        {
                            this.dgvDetalle["Contenido", iFila].Value += " [-]";
                            this.dgvDetalle["Totales", iFila].Value = ("$" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(12));
                            this.Total -= oMov.Total.Valor();
                        }

                        if (oMov.Orden == 14)
                        {
                            this.AgregarLineaEncabezado(
                                " ".PadRight(20)
                                    + " $" + mTickets.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                    + " $" + mFacturas.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                    + " $" + (mTickets + mFacturas).ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                , ""
                            );
                            mTickets = mFacturas = 0;
                        }

                        break;
                    // Resguardos / Gastos
                    case 15:
                    case 16:
                        iFila = this.AgregarLineaDetalle(
                            oMov.Pendientes.Valor()
                            , oMov.Concepto.RellenarCortarDerecha(46)
                                + " $" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " [-]"
                            , "$" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(12)
                        );
                        this.Total -= oMov.Total.Valor();
                        break;
                    case 17:
                    case 18:
                        this.AgregarLineaDetalle(
                            oMov.Pendientes.Valor()
                            , oMov.Concepto.RellenarCortarDerecha(20)
                                + " $" + oMov.Tickets.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Facturas.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                                + " $" + oMov.Total.Valor().ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(11)
                            , ""
                        );
                        break;
                }
            }

            // Se llenan los totales
            this.AgregarLineaEncabezado("Debe haber en Caja".PadLeft(60), "$" + this.Total.ToString(GlobalClass.FormatoDecimal).PadLeft(12));
            this.AgregarLineaEncabezado("Hay en Caja".PadLeft(60), "$" + "0.00".PadLeft(12));
            this.AgregarLineaEncabezado("Diferencia".PadLeft(60), "$" + this.Total.ToString(GlobalClass.FormatoDecimal).PadLeft(12));

            // Se llenan los cambios
            this.dgvCambios.Rows.Clear();
            oParams.Add("OpcionID", CajaDetalleCorte.Opciones.Cambios);
            var oCambios = Datos.ExecuteProcedure<cdcCambios>("pauCajaDetalleCorte", oParams);
            // Se agrega el título
            this.dgvCambios.Rows.Add("Titulo", "Ventas con cambio".ToUpper());
            // Se agrega el encabezado
            this.dgvCambios.Rows.Add(
                "Encabezado", (
                "Folio".PadRight(7 + 1)
                + "Nombre del Cliente".PadRight(20 + 1)
                + "F. pago".PadRight(32 + 1)
                + "Usuario".PadRight(12 + 1)
                + "Comisionista".PadRight(12)
                ).ToUpper()
            );
            // Se llenan los datos
            foreach (var oCambio in oCambios)
            {
                this.dgvCambios.Rows.Add(
                    "Detalle"
                    , oCambio.Folio.RellenarCortarDerecha(7)
                        + " " + oCambio.Cliente.RellenarCortarDerecha(20)
                        + " " + oCambio.FormasDePago.RellenarCortarDerecha(32)
                        + " " + oCambio.Vendedor.RellenarCortarDerecha(12)
                        + " " + oCambio.Comisionista.RellenarCortarDerecha(12)
                );
            }

            // Se cierra la ventana de "Cargando.."
            Cargando.Cerrar();
        }

        public virtual void RegistrarConteo(decimal mConteo)
        {
            this.Conteo = mConteo;
            this.Diferencia = (this.Conteo - this.Total);
            this.dgvDetalle["Totales", this.dgvDetalle.Rows.Count - 2].Value = "$" + this.Conteo.ToString(GlobalClass.FormatoDecimal).PadLeft(12);
            this.dgvDetalle["Totales", this.dgvDetalle.Rows.Count - 1].Value = "$" + this.Diferencia.ToString(GlobalClass.FormatoDecimal).PadLeft(12);

            if (this.Diferencia == 0)
                this.dgvDetalle.Rows[this.dgvDetalle.Rows.Count - 1].DefaultCellStyle.ForeColor = this.dgvDetalle.DefaultCellStyle.ForeColor;
            else
                this.dgvDetalle.Rows[this.dgvDetalle.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.Red;

            this.ctlConteo.SendToBack();
        }

        public DataTable GenerarDatosCorte()
        {
            return this.dgvDetalle.ADataTable();
        }

        public DataTable GenerarDatosCambios()
        {
            return this.dgvCambios.ADataTable();
        }

        #endregion

        protected class LineaDetalle
        {
            public string Concepto { get; set; }
            public int Pendientes { get; set; }
            public decimal Tickets { get; set; }
            public decimal Facturas { get; set; }
            public decimal Otros { get; set; }
            public decimal Total { get { return (this.Tickets + this.Facturas + this.Otros); } }

            public LineaDetalle(string sConcepto)
            {
                this.Concepto = sConcepto;
            }
        }

    }
}
