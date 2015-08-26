using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data.Objects;
using System.Collections;
using System.Linq;
using System.Data;
using System.Collections.Generic;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CajaDetalleCorte : CajaDetalle
    {
        const string FormatoHora = "HH:mm";
        public class Opciones
        {
            public const int Ventas = 1;
            public const int Devoluciones = 2;
            public const int Garantias = 3;
            public const int _9500 = 4;
            public const int Cobranza = 5;
            public const int Egresos = 6;
            public const int Ingresos = 7;
            public const int Cambios = 8;
        }

        bool Actualizando;

        public CajaDetalleCorte()
        {
            InitializeComponent();
        }

        public override void FilaVistoBuenoCambiado(DataGridViewRow Fila, bool bVistoBueno)
        {
            if (this.Actualizando) return;

            string sCatTabla = Helper.ConvertirCadena(Fila.Cells["Tabla"].Value);
            int iRegistroID = Helper.ConvertirEntero(Fila.Cells["RegistroID"].Value);
            int iUsuarioVBId = GlobalClass.UsuarioGlobal.UsuarioID;

            // Se obtiene el visto bueno a marcar/desmarcar, si es que ya existe
            DateTime dHoy = DateTime.Today;
            var oVistoBueno = General.GetEntity<CajaVistoBueno>(q => q.CatTabla == sCatTabla && q.TablaRegistroID == iRegistroID
                && EntityFunctions.TruncateTime(q.Fecha) == dHoy);

            if (bVistoBueno)
            {
                if (oVistoBueno == null)
                {
                    // Se guarda el dato de visto bueno
                    oVistoBueno = new CajaVistoBueno()
                    {
                        CatTabla = sCatTabla,
                        TablaRegistroID = iRegistroID,
                        UsuarioVistoBuenoID = iUsuarioVBId,
                        Fecha = DateTime.Now
                    };
                    Guardar.Generico<CajaVistoBueno>(oVistoBueno);
                }
                else
                {
                    UtilLocal.MensajeAdvertencia("Ya existe un Visto Bueno para el registro especificado.");
                }
                Fila.Cells["TextoCheck"].Value = oVistoBueno.Fecha.ToString(CajaDetalleCorte.FormatoHora);
            }
            else
            {
                // Se borra el dato de visto bueno
                if (oVistoBueno == null)
                {
                    UtilLocal.MensajeAdvertencia("No se encontró el Visto Bueno especificado, en la base de datos.");
                }
                else
                {
                    Guardar.Eliminar<CajaVistoBueno>(oVistoBueno, false);
                }
                Fila.Cells["TextoCheck"].Value = "";
            }
        }

        private void CajaDetalleCorte_Load(object sender, System.EventArgs e)
        {
            // Se llenan los datos
            this.ActualizarDatos();
        }

        #region [ Métodos de uso interno ]

        

        #endregion

        #region [ Públicos ]

        public DataTable GenerarDatosDetalle()
        {
            return this.dgvDetalle.ADataTable();
        }
        
        public void ActualizarDatos()
        {
            // Se muestra la ventana de "Cargando.."
            Cargando.Mostrar();

            this.Actualizando = true;
            this.dgvDetalle.Rows.Clear();

            decimal mTotal = 0;
            int iFila;
            var oParams = new Dictionary<string, object>();
            oParams.Add("OpcionID", CajaDetalleCorte.Opciones.Ventas);
            oParams.Add("SucursalID", GlobalClass.SucursalID);
            oParams.Add("Dia", DateTime.Now.Date);
            oParams.Add("DevDiasAnt", false);

            #region [ Ventas ]
            // Se llenan las ventas
            var Pagos = General.ExecuteProcedure<cdcVentas>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Relación de Ventas");
            this.AgregarLineaEncabezado(
                "Folio".PadRight(8 + 1)
                + "Nombre del Cliente".PadRight(22 + 1)
                + "Forma Pago".PadRight(19 + 1)
                + "Usuario".PadRight(10 + 1)
                + "Caract.".PadRight(7 + 1)
                + "Importe".PadRight(10)
            );
            int iUltOrden = 0;
            foreach (var oPago in Pagos)
            {
                // Se establecen un título para cada grupo de ventas
                if (oPago.Orden != iUltOrden)
                {
                    iUltOrden = oPago.Orden;
                    switch (iUltOrden)
                    {
                        case 1: this.AgregarLineaEncabezado("**** FACTURAS DE VENTAS ANTERIORES ****"); break;
                        case 2: this.AgregarLineaEncabezado("**** VENTAS PAGADAS DE CONTADO ****"); break;
                        case 3: this.AgregarLineaEncabezado("**** VENTAS A CRÉDITO ****"); break;
                    }
                }

                iFila = this.AgregarLineaDetalle(
                    Helper.ConvertirCadena(oPago.Folio).RellenarCortarDerecha(8)
                        + " " + oPago.Cliente.RellenarCortarDerecha(22)
                        + " " + oPago.FormaDePago.RellenarCortarDerecha(19)
                        + " " + oPago.Usuario.RellenarCortarDerecha(10)
                        + " " + oPago.Caracteristica.RellenarCortarDerecha(7)
                        + " $" + oPago.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oPago.FechaVistoBueno == null ? "" : oPago.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , (oPago.CatTabla)
                    , oPago.RegistroID
                );
                if (oPago.FormaDePago != "")
                    mTotal += oPago.Importe;
                if (oPago.Devolucion == 1)
                {
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
                    this.dgvDetalle["Caracteristica", iFila].Value = "D";
                }
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));
            #endregion

            #region [ Devoluciones ]
            // Se llenan las devoluciones - del día
            mTotal = 0;
            this.AgregarLineaEspacio();
            oParams["OpcionID"] = CajaDetalleCorte.Opciones.Devoluciones;
            var DevDia = General.ExecuteProcedure<cdcDevoluciones>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Devoluciones y Cancelaciones del día");
            this.AgregarLineaEncabezado(
                "Folio".PadRight(8 + 1)
                + "Tipo".PadRight(8 + 1)
                + "Cliente".PadRight(20 + 1)
                + "Salida".PadRight(15 + 1)
                + "Autorización".PadRight(15 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in DevDia)
            {
                iFila = this.AgregarLineaDetalle(
                    Helper.ConvertirCadena(oMov.Folio).RellenarCortarDerecha(8)
                        + " " + oMov.Tipo.RellenarCortarDerecha(8)
                        + " " + oMov.Cliente.RellenarCortarDerecha(20)
                        + " " + oMov.Salida.RellenarCortarDerecha(15)
                        + " " + oMov.Autorizacion.RellenarCortarDerecha(15)
                        + " $" + oMov.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("Devoluciones." + Cat.Tablas.VentaDevolucion)
                    , oMov.VentaDevolucionID
                );
                mTotal += oMov.Importe;

                if (oMov.Autorizacion.EndsWith("> "))
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));

            // Se llenan las devoluciones - días anteriores
            mTotal = 0;
            this.AgregarLineaEspacio();
            // oParams["OpcionID"] = 2;
            oParams["DevDiasAnt"] = true;
            var DevAnt = General.ExecuteProcedure<cdcDevoluciones>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Devoluciones y Cancelaciones de días anteriores");
            this.AgregarLineaEncabezado(
                "Folio".PadRight(8 + 1)
                + "Tipo".PadRight(8 + 1)
                + "Cliente".PadRight(20 + 1)
                + "Salida".PadRight(15 + 1)
                + "Autorización".PadRight(15 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in DevAnt)
            {
                iFila = this.AgregarLineaDetalle(
                    Helper.ConvertirCadena(oMov.Folio).RellenarCortarDerecha(8)
                        + " " + oMov.Tipo.RellenarCortarDerecha(8)
                        + " " + oMov.Cliente.RellenarCortarDerecha(20)
                        + " " + oMov.Salida.RellenarCortarDerecha(15)
                        + " " + oMov.Autorizacion.RellenarCortarDerecha(15)
                        + " $" + oMov.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("Devoluciones." + Cat.Tablas.VentaDevolucion)
                    , oMov.VentaDevolucionID
                );
                mTotal += oMov.Importe;

                if (oMov.Autorizacion.EndsWith("> "))
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));
            #endregion

            #region [ Garantías ]
            // Se llenan las Garantías - del día
            mTotal = 0;
            this.AgregarLineaEspacio();
            oParams["OpcionID"] = CajaDetalleCorte.Opciones.Garantias;
            oParams["DevDiasAnt"] = false;
            var oGarantiasDia = General.ExecuteProcedure<cdcGarantias>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Garantías del día");
            this.AgregarLineaEncabezado(
                "Folio".PadRight(8 + 1)
                + "Cliente".PadRight(20 + 1)
                + "Salida".PadRight(15 + 1)
                + "Autorización".PadRight(15 + 9 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in oGarantiasDia)
            {
                iFila = this.AgregarLineaDetalle(
                    Helper.ConvertirCadena(oMov.Folio).RellenarCortarDerecha(8)
                        + " " + oMov.Cliente.RellenarCortarDerecha(20)
                        + " " + oMov.Salida.RellenarCortarDerecha(15)
                        + " " + oMov.Autorizacion.RellenarCortarDerecha(15 + 9)
                        + " $" + oMov.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("Garantias." + Cat.Tablas.VentaGarantia)
                    , oMov.VentaGarantiaID
                );
                mTotal += oMov.Importe;

                if (oMov.Autorizacion.EndsWith("> "))
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));

            // Se llenan las Garantías - días anteriores
            mTotal = 0;
            this.AgregarLineaEspacio();
            // oParams["OpcionID"] = 2;
            oParams["DevDiasAnt"] = true;
            var oGarantiasAnt = General.ExecuteProcedure<cdcGarantias>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Garantías de días anteriores");
            this.AgregarLineaEncabezado(
                "Folio".PadRight(8 + 1)
                + "Cliente".PadRight(20 + 1)
                + "Salida".PadRight(15 + 1)
                + "Autorización".PadRight(15 + 9 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in oGarantiasAnt)
            {
                iFila = this.AgregarLineaDetalle(
                    Helper.ConvertirCadena(oMov.Folio).RellenarCortarDerecha(8)
                        + " " + oMov.Cliente.RellenarCortarDerecha(20)
                        + " " + oMov.Salida.RellenarCortarDerecha(15)
                        + " " + oMov.Autorizacion.RellenarCortarDerecha(15 + 9)
                        + " $" + oMov.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("Garantias." + Cat.Tablas.VentaGarantia)
                    , oMov.VentaGarantiaID
                );
                mTotal += oMov.Importe;

                if (oMov.Autorizacion.EndsWith("> "))
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));
            #endregion

            #region [ 9500 ]
            // Se llenan los 9500
            mTotal = 0;
            this.AgregarLineaEspacio();
            oParams["OpcionID"] = CajaDetalleCorte.Opciones._9500;
            var Ventas9500 = General.ExecuteProcedure<cdc9500>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("9500");
            this.AgregarLineaEncabezado(
                "Folio".PadRight(8 + 1)
                + "Cliente".PadRight(61 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in Ventas9500)
            {
                this.AgregarLineaDetalle(
                    Helper.ConvertirCadena(oMov.Folio).RellenarCortarDerecha(8)
                        + " " + oMov.Cliente.RellenarCortarDerecha(61)
                        + " $" + oMov.Anticipo.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("9500." + Cat.Tablas.Tabla9500)
                    , oMov.Cotizacion9500ID
                );
                mTotal += oMov.Anticipo;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));
            #endregion

            #region [ Cobranza ]
            // Se llenan los pagos de ventas a crédito (Cobranza)
            mTotal = 0;
            this.AgregarLineaEspacio();
            oParams["OpcionID"] = CajaDetalleCorte.Opciones.Cobranza;
            var Cobranza = General.ExecuteProcedure<cdcCobranza>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Cobranza");
            this.AgregarLineaEncabezado(
                "Folio".PadRight(8 + 1)
                + "Nombre del Cliente".PadRight(41 + 1)
                + "Forma Pago".PadRight(19 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in Cobranza)
            {
                this.AgregarLineaDetalle(
                    Helper.ConvertirCadena(oMov.Folio).RellenarCortarDerecha(8)
                        + " " + oMov.Cliente.RellenarCortarDerecha(41)
                        + " " + oMov.FormaDePago.RellenarCortarDerecha(19)
                        + " $" + oMov.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("Cobranza." + Cat.Tablas.VentaPago)
                    , oMov.VentaPagoID
                );
                mTotal += oMov.Importe;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));
            #endregion

            #region [ Gastos ]
            // Se llenan los gastos
            mTotal = 0;
            this.AgregarLineaEspacio();
            oParams["OpcionID"] = CajaDetalleCorte.Opciones.Egresos;
            var Egresos = General.ExecuteProcedure<cdcIngresosEgresos>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Gastos");
            this.AgregarLineaEncabezado(
                "Concepto".PadRight(45 + 1)
                + "Autorización".PadRight(24 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in Egresos)
            {
                iFila = this.AgregarLineaDetalle(
                    oMov.Concepto.RellenarCortarDerecha(45)
                        + " " + oMov.Autorizacion.RellenarCortarDerecha(23)
                        + " $" + oMov.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("Egresos." + Cat.Tablas.CajaEgreso)
                    , oMov.RegistroID
                );
                mTotal += oMov.Importe;

                if (oMov.Autorizacion.EndsWith("> "))
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));
            #endregion

            #region [ Ingresos ]
            // Se llenan los otros ingresos
            mTotal = 0;
            this.AgregarLineaEspacio();
            oParams["OpcionID"] = CajaDetalleCorte.Opciones.Ingresos;
            var Ingresos = General.ExecuteProcedure<cdcIngresosEgresos>("pauCajaDetalleCorte", oParams);
            this.AgregarLineaTitulo("Otros Ingresos");
            this.AgregarLineaEncabezado(
                "Concepto".PadRight(45 + 1)
                + "Autorización".PadRight(24 + 1)
                + "Importe".PadRight(10)
            );
            foreach (var oMov in Ingresos)
            {
                iFila = this.AgregarLineaDetalle(
                    oMov.Concepto.RellenarCortarDerecha(45)
                        + " " + oMov.Autorizacion.RellenarCortarDerecha(23)
                        + " $" + oMov.Importe.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9)
                    , true
                    , (oMov.FechaVistoBueno == null ? "" : oMov.FechaVistoBueno.Valor().ToString(CajaDetalleCorte.FormatoHora))
                    , ("Ingresos." + Cat.Tablas.CajaIngreso)
                    , oMov.RegistroID
                );
                mTotal += oMov.Importe;

                if (oMov.Autorizacion.EndsWith("> "))
                    this.dgvDetalle.Rows[iFila].DefaultCellStyle.ForeColor = Color.Red;
            }
            this.AgregarLineaEncabezado("Total".PadLeft(70) + " $" + mTotal.ToString(GlobalClass.FormatoDecimal).RellenarCortarIzquierda(9));
            #endregion

            this.Actualizando = false;

            // Se cierra la ventana de "Cargando.."
            Cargando.Cerrar();
        }

        #endregion
    }
}
