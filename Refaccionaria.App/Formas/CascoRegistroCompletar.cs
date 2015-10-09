using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FastReport;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public partial class CascoRegistroCompletar : Form
    {
        CascoRegistro oCascoRegistro;

        public CascoRegistroCompletar(int iCascoRegistroID)
        {
            InitializeComponent();

            this.oCascoRegistro = General.GetEntity<CascoRegistro>(c => c.CascoRegistroID == iCascoRegistroID);
        }

        #region [ Eventos ]

        private void CascoRegistroCompletar_Load(object sender, EventArgs e)
        {
            // Se cargan los controles
            var oParte = General.GetEntity<Parte>(c => c.ParteID == this.oCascoRegistro.ParteID && c.Estatus);
            this.cmbCascoRecibido.CargarDatos("ParteID", "NumeroParte + NombreParte", General.GetListOf<Parte>(c => c.EsCascoPara == oParte.LineaID && c.Estatus));

            this.CargarImportesAFavor();
        }

        private void cmbCascoRecibido_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((e.ListItem as Parte).NumeroParte + " - " + (e.ListItem as Parte).NombreParte);
        }

        private void dgvPendientes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string sCol = this.dgvImportesAFavor.Columns[e.ColumnIndex].Name;
            if (sCol == "Sel" || sCol == "ImporteAUsar")
            {
                if (sCol == "ImporteAUsar" && !Helper.ConvertirBool(this.dgvImportesAFavor["Sel", e.RowIndex].Value))
                {
                    this.dgvImportesAFavor["Sel", e.RowIndex].Value = true;
                    return;
                }
                this.CalcularTotalAFavor();
            }
        }
                
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (this.AccionAceptar())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            Cargando.Cerrar();
        }
                
        private void btnCerrrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region [ Métodos ]

        private void CargarImportesAFavor()
        {
            // Se revisan los importes negativos, para ajustarlos según sea necesario
            var oNegativos = General.GetListOf<CascoImporte>(c => (c.Importe - c.ImporteUsado) < 0);
            foreach (var oReg in oNegativos)
            {
                decimal mImporteNeg = (oReg.Importe * -1);
                var oImp = General.GetEntity<CascoImporte>(c => c.Importe == mImporteNeg && c.ImporteUsado == 0);
                if (oImp == null)
                {
                    var oPositivos = General.GetListOf<CascoImporte>(c => (c.Importe - c.ImporteUsado) > 0);
                    foreach (var oPositivo in oPositivos)
                    {
                        decimal mAFavor = (oPositivo.Importe - oPositivo.ImporteUsado);
                        oReg.ImporteUsado -= mAFavor;
                        if (oReg.ImporteUsado < oReg.Importe)
                        {
                            mAFavor += (oReg.ImporteUsado - oReg.Importe);
                            oReg.ImporteUsado = oReg.Importe;
                        }
                        oPositivo.ImporteUsado += mAFavor;
                        Guardar.Generico<CascoImporte>(oPositivo);

                        if (oReg.ImporteUsado == oReg.Importe)
                            break;
                    }
                    Guardar.Generico<CascoImporte>(oReg);
                }
                else
                {
                    oImp.ImporteUsado = mImporteNeg;
                    Guardar.Generico<CascoImporte>(oImp);
                    oReg.ImporteUsado = oReg.Importe;
                    Guardar.Generico<CascoImporte>(oReg);
                }
            }

            // Se cargan los importes a favor
            var oDatos = General.GetListOf<CascoImporte>(c => (c.Importe - c.ImporteUsado) > 0);
            this.dgvImportesAFavor.Rows.Clear();
            foreach (var oReg in oDatos)
                this.dgvImportesAFavor.Rows.Add(false, oReg.CascoImporteID, (oReg.Importe - oReg.ImporteUsado));
        }

        private void CalcularTotalAFavor()
        {
            decimal mTotal = 0;
            foreach (DataGridViewRow oFila in this.dgvImportesAFavor.Rows)
            {
                if (Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                {
                    decimal mImporte = Helper.ConvertirDecimal(oFila.Cells["Importe"].Value);
                    decimal mImporteAUsar = Helper.ConvertirDecimal(oFila.Cells["ImporteAUsar"].Value);
                    if (mImporteAUsar > mImporte)
                        oFila.Cells["ImporteAUsar"].Value = mImporte;

                    mTotal += Helper.ConvertirDecimal(oFila.Cells["ImporteAUsar"].Value);
                }
            }
            this.lblTotalAFavor.Text = mTotal.ToString();
        }

        private bool AccionAceptar()
        {
            // Cargando.Mostrar();

            decimal mFaltante = 0;
            int iClienteID = 0;
            int iCobroVentaID = 0;
            bool bUsarCascosAFavor = false;
            bool bCrearVale = false;
            bool bCascoAFavor = false;

            int iCascoRecibido = Helper.ConvertirEntero(this.cmbCascoRecibido.SelectedValue);
            int iSucursalID = GlobalClass.SucursalID;
            int iCascoRegistroID = this.oCascoRegistro.CascoRegistroID;
            int iParteID = this.oCascoRegistro.ParteID;
            var oParte = General.GetEntity<Parte>(c => c.ParteID == iParteID && c.Estatus);
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == this.oCascoRegistro.VentaID && c.Estatus);
            iClienteID = oVenta.ClienteID;
            var oCliente = General.GetEntity<Cliente>(c => c.ClienteID == oVenta.ClienteID && c.Estatus);

            if (iCascoRecibido != oParte.RequiereCascoDe)
            {
                // Si se recibió algún casco
                if (iCascoRecibido > 0)
                {
                    // Se obtienen el importe requerido
                    var oPrecioReq = General.GetEntity<PartePrecio>(c => c.ParteID == oParte.RequiereCascoDe && c.Estatus);
                    var oPrecioRec = General.GetEntity<PartePrecio>(c => c.ParteID == iCascoRecibido && c.Estatus);
                    if (oPrecioReq == null || oPrecioRec == null)
                    {
                        UtilLocal.MensajeAdvertencia("Hubo un error al obtener el Casco requerido.");
                        return false;
                    }
                    
                    // Se calcula el importe faltante
                    decimal?[] aPreciosReq = new decimal?[5] { oPrecioReq.PrecioUno, oPrecioReq.PrecioDos, oPrecioReq.PrecioTres, oPrecioReq.PrecioCuatro, oPrecioReq.PrecioCinco };
                    decimal?[] aPreciosRec = new decimal?[5] { oPrecioRec.PrecioUno, oPrecioRec.PrecioDos, oPrecioRec.PrecioTres, oPrecioRec.PrecioCuatro, oPrecioRec.PrecioCinco };
                    mFaltante = (aPreciosReq[oCliente.ListaDePrecios - 1] - aPreciosRec[oCliente.ListaDePrecios - 1]).Valor();

                    // Se evalúa el importe faltante, para tomar alguna acción
                    if (mFaltante != 0)
                    {
                        // Si el casco recibido es menor que el casco esperado
                        if (mFaltante > 0)
                        {
                            // Se busca por folio de cobro o por cascos a favor, según sea el caso
                            if (this.txtFolioDeCobro.Text == "")
                            {
                                decimal mTotalAFavor = Helper.ConvertirDecimal(this.lblTotalAFavor.Text);
                                if (mTotalAFavor != mFaltante)
                                {
                                    UtilLocal.MensajeAdvertencia("El importe seleccionado con los Cascos a favor debe ser de " + mFaltante.ToString(GlobalClass.FormatoMoneda));
                                    return false;
                                }
                                bUsarCascosAFavor = true;
                            }
                            else
                            {
                                // Se obtiene el importe de la venta
                                var oVentaCobroV = General.GetEntity<VentasView>(c => c.Folio == this.txtFolioDeCobro.Text && c.SucursalID == GlobalClass.SucursalID
                                    && (c.VentaEstatusID == Cat.VentasEstatus.Cobrada || c.VentaEstatusID == Cat.VentasEstatus.Completada));
                                if (oVentaCobroV == null)
                                {
                                    UtilLocal.MensajeAdvertencia("La venta específicada no existe, no es de esta sucursal o no ha sido cobrada.");
                                    return false;
                                }
                                iCobroVentaID = oVentaCobroV.VentaID;

                                // Se valida que la parte de la venta sea una "diferencia de casco"
                                if (!General.Exists<VentaDetalle>(c => c.VentaID == iCobroVentaID && c.ParteID == Cat.Partes.DiferenciaDeCascos && c.Estatus))
                                {
                                    UtilLocal.MensajeAdvertencia("La venta especificada no corresponde a una Diferencia de Casco.");
                                    return false;
                                }
                                // Se valida que el importe cobrado sea el correspondiente
                                if (oVentaCobroV.Total != mFaltante)
                                {
                                    UtilLocal.MensajeAdvertencia("El cobro por la diferencia debe de ser de " + mFaltante.ToString(GlobalClass.FormatoMoneda));
                                    return false;
                                }
                            }
                        }
                        // Si el casco recibido es mayor que el casco esperado
                        else if (mFaltante < 0)
                        {
                            var oRes = UtilLocal.MensajePreguntaCancelar("El importe del casco recibido es mayor al esperado. ¿Deseas crear un Vale a favor del Cliente?");
                            if (oRes == DialogResult.Cancel)
                                return false;
                            bCrearVale = (oRes == DialogResult.Yes);
                            bCascoAFavor = !bCrearVale;
                        }
                    }
                }
                else  // Si no se recibió ningún casco, se evalúa la venta
                {
                    // Se obtiene la VentaID
                    var oVentaCobro = General.GetEntity<Venta>(c => c.Folio == this.txtFolioDeCobro.Text && c.SucursalID == GlobalClass.SucursalID 
                        && (c.VentaEstatusID == Cat.VentasEstatus.Cobrada || c.VentaEstatusID == Cat.VentasEstatus.Completada) && c.Estatus);
                    if (oVentaCobro == null)
                    {
                        UtilLocal.MensajeAdvertencia("La venta específicada no existe, no es de esta sucursal o no ha sido cobrada.");
                        return false;
                    }
                    iCobroVentaID = oVentaCobro.VentaID;
                    
                    // Se valida que la venta tenga la Parte correspondiente al casco
                    if (!General.Exists<VentaDetalle>(c => c.VentaID == iCobroVentaID && c.ParteID == oParte.RequiereDepositoDe && c.Estatus))
                    {
                        UtilLocal.MensajeAdvertencia("El cobro del Casco no corresonde al Artículo.");
                        return false;
                    }
                }
            }

            // Cargando.Cerrar();
            // Se solicita la contraseña
            var oResU = UtilDatos.ValidarObtenerUsuario("Ventas.ControlDeCascos.Completar");
            if (oResU.Error) return false;
            Cargando.Mostrar();

            // Se completa el registro de casco
            DateTime dAhora = DateTime.Now;
            // Se registran y modifican los Cascos a favor usados, si aplica
            if (bUsarCascosAFavor)
            {
                foreach (DataGridViewRow oFila in this.dgvImportesAFavor.Rows)
                {
                    if (Helper.ConvertirBool(oFila.Cells["Sel"].Value))
                    {
                        // Se registra el importe usado para el CascoImporte
                        int iCascoImporteID = Helper.ConvertirEntero(oFila.Cells["CascoImporteID"].Value);
                        decimal mAUsar = Helper.ConvertirDecimal(oFila.Cells["ImporteAUsar"].Value);
                        var oCascoImporte = General.GetEntity<CascoImporte>(c => c.CascoImporteID == iCascoImporteID);
                        oCascoImporte.ImporteUsado += mAUsar;
                        Guardar.Generico<CascoImporte>(oCascoImporte);
                        // Se registra la relación entre el CascoRegistro y el CascoImporte
                        var oCascoRegImp = new CascoRegistroImporte()
                        {
                            CascoRegistroID = this.oCascoRegistro.CascoRegistroID,
                            CascoImporteID = oCascoImporte.CascoImporteID,
                            Importe = mAUsar
                        };
                        Guardar.Generico<CascoRegistroImporte>(oCascoRegImp);
                    }
                }
            }
            // Se crea el vale, si aplica
            if (bCrearVale)
            {
                var oRes = VentasProc.GenerarNotaDeCredito(iClienteID, (mFaltante * -1), "", Cat.OrigenesNotaDeCredito.CascoDeMayorValor
                    , this.oCascoRegistro.CascoRegistroID.ToString());
                VentasProc.GenerarTicketNotaDeCredito(oRes.Respuesta);
            }
            // Se registra el casco a favor, si aplica
            if (bCascoAFavor)
            {
                var oCascoImporte = new CascoImporte()
                {
                    Fecha = dAhora,
                    OrigenID = this.oCascoRegistro.CascoRegistroID,
                    Importe = (mFaltante * -1)
                };
                Guardar.Generico<CascoImporte>(oCascoImporte);
            }
            // Se afecta la existencia y el kárdex del casco recibido
            if (iCascoRecibido > 0)
            {
                var oSucursal = General.GetEntity<Sucursal>(c => c.SucursalID == iSucursalID && c.Estatus);
                var oPrecioRec = General.GetEntity<PartePrecio>(c => c.ParteID == iCascoRecibido && c.Estatus);
                AdmonProc.AfectarExistenciaYKardex(iCascoRecibido, GlobalClass.SucursalID, Cat.OperacionesKardex.EntradaInventario, iCascoRegistroID.ToString()
                    , oResU.Respuesta.UsuarioID, oCliente.Nombre, "CONTROL DE CASCOS", oSucursal.NombreSucursal, 1, oPrecioRec.Costo.Valor()
                    , Cat.Tablas.CascoRegistro, iCascoRegistroID);
            }
            
            // Se guardan los datos del registro de casco
            this.oCascoRegistro.RecibidoCascoID = (iCascoRecibido > 0 ? (int?)iCascoRecibido : null);
            this.oCascoRegistro.RealizoUsuarioID = oResU.Respuesta.UsuarioID;
            this.oCascoRegistro.CobroVentaID = (iCobroVentaID > 0 ? (int?)iCobroVentaID : null);
            Guardar.Generico<CascoRegistro>(this.oCascoRegistro);

            // Se manda a imprimir el ticket correspondiente
            var oCascoRegV = General.GetEntity<CascosRegistrosView>(c => c.CascoRegistroID == iCascoRegistroID);
            var oRep = new Report();
            oRep.Load(GlobalClass.ConfiguracionGlobal.pathReportes + "ControlDeCascos.frx");
            VentasProc.TicketAgregarLeyendas(ref oRep);
            oRep.RegisterData(new List<CascosRegistrosView>() { oCascoRegV }, "ControlDeCasco");
            UtilLocal.EnviarReporteASalida("Reportes.ControlDeCascos.Completar.Salida", oRep);

            Cargando.Cerrar();
            UtilLocal.MostrarNotificacion("Control de casco completado correctamente.");

            return true;
        }
        
        #endregion
              
                
    }
}

