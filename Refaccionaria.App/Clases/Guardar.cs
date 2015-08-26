using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Reflection;
using System.Data;
using System.Linq;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public static class Guardar
    {
        #region [ Genéricos ]

        public static ResAcc Generico<T>(T Objeto, bool bLlenarDatosPredefinidos) where T : EntityObject
        {
            if (bLlenarDatosPredefinidos)
                Guardar.LlenarDatosPredefinidos(Objeto);

            General.SaveOrUpdate<T>(Objeto, Objeto);
            return new ResAcc(true);
        }

        public static ResAcc Generico<T>(T Objeto) where T : EntityObject
        {
            return Guardar.Generico<T>(Objeto, true); ;
        }

        public static ResAcc Eliminar<T>(T oObjeto, bool bLogico) where T : EntityObject
        {
            if (bLogico)
            {
                var oTipo = oObjeto.GetType();
                var oProp = oObjeto.GetType().GetProperty("Estatus");
                if (oProp != null)
                    oProp.SetValue(oObjeto, false, null);
                return Guardar.Generico<T>(oObjeto);
            }
            else
            {
                General.Delete<T>(oObjeto);
                return new ResAcc(true);
            }
        }

        public static ResAcc Eliminar<T>(T oObjeto) where T : EntityObject
        {
            return Guardar.Eliminar<T>(oObjeto, false);
        }

        #endregion

        #region [ Guardar específicos ]

        public static ResAcc Parte(Parte oParte, PartePrecio oPartePrecio)
        {
            // Se llenan datos predeterminados, si no han sido llenados
            oParte.ParteEstatusID = (oParte.ParteEstatusID > 0 ? oParte.ParteEstatusID : Cat.PartesEstatus.Activo);
            oParte.MedidaID = (oParte.MedidaID > 0 ? oParte.MedidaID : Cat.Medidas.Pieza);
            oParte.UnidadEmpaque = (oParte.UnidadEmpaque > 0 ? oParte.UnidadEmpaque : 1);
            oParte.AplicaComision = (oParte.AplicaComision.HasValue ? oParte.AplicaComision : true);
            oParte.Etiqueta = (oParte.Etiqueta.HasValue ? oParte.Etiqueta : true);

            // Se guarda el registro de la parte
            Guardar.Generico<Parte>(oParte);

            // Se guarda el registro del precio
            if (oPartePrecio != null)
            {
                oPartePrecio.ParteID = oParte.ParteID;
                oPartePrecio.CostoConDescuento = (oPartePrecio.CostoConDescuento.HasValue ? oPartePrecio.CostoConDescuento : oPartePrecio.Costo);
                Guardar.Generico<PartePrecio>(oPartePrecio);
            }

            // Se generan los registros de existencia, uno por cada sucursal
            var oSucursales = General.GetListOf<Sucursal>(q => q.Estatus);
            foreach (var oSucursal in oSucursales)
            {
                Guardar.Generico<ParteExistencia>(new ParteExistencia()
                {
                    ParteID = oParte.ParteID,
                    SucursalID = oSucursal.SucursalID,
                    Existencia = 0
                });
            }

            // Se generan los registros para Máximos y Mínimos, uno por cada sucursal
            foreach (var oSucursal in oSucursales)
            {
                // Se buscan los criterios generales predefinidos, para asignarlos
                var oCriterioPred = General.GetEntity<ParteMaxMinCriterioPredefinido>(q => q.SucursalID == oSucursal.SucursalID
                    && q.ProveedorID == oParte.ProveedorID && q.MarcaID == oParte.MarcaParteID && q.LineaID == oParte.LineaID);
                //
                Guardar.Generico<ParteMaxMin>(new ParteMaxMin()
                {
                    ParteID = oParte.ParteID,
                    SucursalID = oSucursal.SucursalID,
                    Calcular = (oCriterioPred == null ? null : oCriterioPred.Calcular),
                    VentasGlobales = (oCriterioPred == null ? null : oCriterioPred.VentasGlobales)
                });
            }

            // Se genera el registro de Abc (ParteAbc)
            Guardar.Generico<ParteAbc>(new ParteAbc()
            {
                ParteID = oParte.ParteID,
                AbcDeVentas = "Z"
            });

            return new ResAcc(true);
        }

        public static ResAcc ParteVehiculo(ParteVehiculo Objeto)
        {
            bool bEsMod = (Objeto.ParteVehiculoID > 0);

            if (!bEsMod)
            {
                // Se verifica que no exista ya un registro así
                var Existe = General.GetEntity<ParteVehiculo>(q => q.ParteID == Objeto.ParteID && q.MotorID == Objeto.MotorID && q.Anio == Objeto.Anio);
                if (Existe != null)
                    return new ResAcc(false, "Ya existe un registro con esas características.");
            }

            return Guardar.Generico<ParteVehiculo>(Objeto);
        }

        public static ResAcc ParteEquivalencia(int iParteID, int iParteIDEquivalente)
        {
            // Se obtiene el grupo de cualquiera de las partes
            var oParteEq1 = General.GetEntity<ParteEquivalente>(c => c.ParteID == iParteID && c.Estatus);
            var oParteEq2 = General.GetEntity<ParteEquivalente>(c => c.ParteID == iParteIDEquivalente && c.Estatus);
            // Si ninguna de las partes tiene grupo, se crea uno nuevo
            int iGrupoID = 0;
            if (oParteEq1 == null && oParteEq2 == null)
            {
                var oEquivalentes = General.GetListOf<ParteEquivalente>(c => c.Estatus);
                iGrupoID = (oEquivalentes.Count > 0 ? oEquivalentes.Max(c => c.GrupoID) : 0);
                iGrupoID++;
            }
            else
            {
                iGrupoID = (oParteEq1 == null ? oParteEq2.GrupoID : oParteEq1.GrupoID);
            }

            // Se agrega la equivalencia de la parte 1, por si no existiera
            if (oParteEq1 == null)
            {
                oParteEq1 = new ParteEquivalente() { GrupoID = iGrupoID, ParteID = iParteID };
                oParteEq1.RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                Guardar.Generico<ParteEquivalente>(oParteEq1);
            }

            // Se agrega la equivalencia de la parte 2, según aplique
            if (oParteEq2 == null)
            {
                oParteEq2 = new ParteEquivalente() { GrupoID = iGrupoID, ParteID = iParteIDEquivalente };
                oParteEq2.RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                Guardar.Generico<ParteEquivalente>(oParteEq2);
            }

            // Se verifica si las dos ya tenían grupo, pero diferente, en cuyo caso todos se van a un mismo grupo
            if (oParteEq1.GrupoID != oParteEq2.GrupoID)
            {
                var oPartesEq2 = General.GetListOf<ParteEquivalente>(c => c.GrupoID == oParteEq2.GrupoID && c.Estatus);
                foreach (var oEq in oPartesEq2)
                {
                    oEq.GrupoID = oParteEq1.GrupoID;
                    oEq.RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID;
                    Guardar.Generico<ParteEquivalente>(oEq);
                }
            }

            return new ResAcc(true);
        }

        public static ResAcc ParteComplementaria(int iParteID, int iParteIDComplementaria)
        {
            // Se verifica si son las partes son iguales, en cuyo caso no se hace nada
            if (iParteIDComplementaria == iParteID)
                return new ResAcc(false);
            // Se verifica si ya existe la relación de complementarios
            var oComp = General.GetEntity<ParteComplementaria>(c => c.ParteID == iParteID && c.ParteIDComplementaria == iParteIDComplementaria);
            if (oComp != null)
                return new ResAcc(true);

            // Se agrega el registro de la parte complementaria
            oComp = new ParteComplementaria()
            {
                ParteID = iParteID,
                ParteIDComplementaria = iParteIDComplementaria,
                RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
            };
            Guardar.Generico<ParteComplementaria>(oComp);

            return new ResAcc(true);
        }

        public static ResAcc Venta(Venta oVenta, List<VentaDetalle> Detalle)
        {
            bool bMod = (oVenta.VentaID > 0);
            if (bMod) throw new Exception("No se ha programado funcionalidad para cuando la Venta ya exista.");

            // Se generan datos predeterminados o globales, en caso de que apliquen
            oVenta.Fecha = (oVenta.Fecha != DateTime.MinValue ? oVenta.Fecha : DateTime.Now);
            oVenta.VentaEstatusID = (oVenta.VentaEstatusID > 0 ? oVenta.VentaEstatusID : Cat.VentasEstatus.Realizada);
            oVenta.RealizoUsuarioID = (oVenta.RealizoUsuarioID > 0 ? oVenta.RealizoUsuarioID : GlobalClass.UsuarioGlobal.UsuarioID);
            oVenta.SucursalID = (oVenta.SucursalID > 0 ? oVenta.SucursalID : GlobalClass.SucursalID);
            oVenta.ComisionistaClienteID = (oVenta.ComisionistaClienteID > 0 ? oVenta.ComisionistaClienteID : null);
            oVenta.ClienteVehiculoID = (oVenta.ClienteVehiculoID > 0 ? oVenta.ClienteVehiculoID : null);
            
            // Se obtiene el folio correspondiente
            /* string sFolio = Config.Valor("Ventas.Folio");
            Config.EstablecerValor("Ventas.Folio", (Helper.ConvertirEntero(sFolio) + 1).ToString().PadLeft(7, '0'));
            oVenta.Folio = sFolio; */

            // Se guarda la venta
            Guardar.Generico<Venta>(oVenta);

            // Se guarda el detalle
            foreach (var ParteDetalle in Detalle)
            {
                if (ParteDetalle.VentaDetalleID > 0) continue;  // No es una venta nueva, no se ha especificado que hacer en estos casos

                ParteDetalle.VentaID = oVenta.VentaID;
                Guardar.Generico<VentaDetalle>(ParteDetalle);

                // Se afecta la existencia
                VentasProc.AgregarExistencia(ParteDetalle.ParteID, oVenta.SucursalID, (ParteDetalle.Cantidad * -1));
            }

            // Se generar datos relevantes al cliente comisionista, si hubiera
            if (oVenta.ComisionistaClienteID.Valor() > 0)
            {
                // Se calcula el importe de la comisión
                decimal mComision = UtilDatos.VentaComisionCliente(oVenta.VentaID, oVenta.ComisionistaClienteID.Valor());
                // Se genera una nota de crédito, por la comisión
                if (mComision > 0)
                    VentasProc.GenerarNotaDeCredito(oVenta.ComisionistaClienteID.Valor(), mComision, "", Cat.OrigenesNotaDeCredito.Comision, oVenta.VentaID.ToString());
            }

            return new ResAcc(true);
        }

        public static ResAcc VentaPago(VentaPago oPago, List<VentaPagoDetalle> Detalle)
        {
            // Se generan datos predeterminados o globales, en caso de que apliquen
            oPago.Fecha = (oPago.Fecha != DateTime.MinValue ? oPago.Fecha : DateTime.Now);
            oPago.SucursalID = (oPago.SucursalID > 0 ? oPago.SucursalID : GlobalClass.SucursalID);

            // Se guarda el pago
            Guardar.Generico<VentaPago>(oPago);

            // Se guarda el detalle
            var oVentaV = General.GetEntity<VentasView>(q => q.VentaID == oPago.VentaID);
            foreach (var PartePago in Detalle)
            {
                PartePago.VentaPagoID = oPago.VentaPagoID;
                Guardar.Generico<VentaPagoDetalle>(PartePago);

                // Se afectan las notas de crédito, si hay alguna
                if (PartePago.TipoFormaPagoID == Cat.FormasDePago.Vale && PartePago.Importe > 0)
                {
                    int iNotaID = PartePago.NotaDeCreditoID.Valor();
                    var oNota = General.GetEntity<NotaDeCredito>(q => q.NotaDeCreditoID == iNotaID);
                    if (oNota != null)
                    {
                        // Se verifica si se usó el importe total o sólo una parte
                        if (PartePago.Importe < oNota.Importe)
                        {
                            // Se crea una nueva nota, con el importe restante
                            VentasProc.GenerarNotaDeCredito(oNota.ClienteID, (oNota.Importe - PartePago.Importe), "", Cat.OrigenesNotaDeCredito.ImporteRestante
                                , oNota.OrigenVentaID.ToString());
                            //
                            oNota.Importe = PartePago.Importe;
                        }
                        //
                        oNota.Valida = false;
                        oNota.FechaDeUso = DateTime.Now;
                        oNota.UsoVentaID = oPago.VentaID;
                        Guardar.Generico<NotaDeCredito>(oNota);
                    }
                }

                // Si es un pago bancario, se genera el movimiento correspondiente
                if (PartePago.TipoFormaPagoID == Cat.FormasDePago.Tarjeta || PartePago.TipoFormaPagoID == Cat.FormasDePago.Transferencia
                    || PartePago.TipoFormaPagoID == Cat.FormasDePago.Cheque)
                {
                    var oBanco = General.GetEntity<Banco>(c => c.BancoID == PartePago.BancoID && c.Estatus);

                    var oMovBanc = new BancoCuentaMovimiento()
                    {
                        BancoCuentaID = (PartePago.TipoFormaPagoID == Cat.FormasDePago.Tarjeta ? (int?)Cat.CuentasBancarias.Banamex : null),
                        EsIngreso = true,
                        Fecha = oPago.Fecha,
                        FechaAsignado = (PartePago.TipoFormaPagoID == Cat.FormasDePago.Tarjeta ? (DateTime?)oPago.Fecha : null),
                        SucursalID = oPago.SucursalID,
                        Importe = PartePago.Importe,
                        Concepto = oVentaV.Cliente,
                        Referencia = oVentaV.Folio,
                        TipoFormaPagoID = PartePago.TipoFormaPagoID,
                        DatosDePago = string.Format("{0}-{1}-{2}", oBanco.NombreBanco, PartePago.Folio, PartePago.Cuenta),
                        RelacionTabla = Cat.Tablas.VentaPagoDetalle,
                        RelacionID = PartePago.VentaPagoDetalleID,
                    };
                    ContaProc.RegistrarMovimientoBancario(oMovBanc);
                }
            }

            // Se verifica el estatus de la venta, por si debe cambiar según el pago
            if (oVentaV.VentaEstatusID == Cat.VentasEstatus.Cobrada)
            {
                // Se obtiene el total de los pagos
                decimal mPagado = General.GetListOf<VentasPagosView>(q => q.VentaID == oVentaV.VentaID).Sum(q => q.Importe);
                if (mPagado >= oVentaV.Total)
                {
                    var oVenta = General.GetEntity<Venta>(q => q.Estatus && q.VentaID == oPago.VentaID);
                    oVenta.VentaEstatusID = Cat.VentasEstatus.Completada;
                    // Se guarda con el nuevo estatus
                    Guardar.Generico<Venta>(oVenta);
                }
            }

            return new ResAcc(true);
        }

        public static ResAcc VentaDevolucion(VentaDevolucion oDevolucion, List<VentaDevolucionDetalle> oDetalle)
        {
            // Se generan datos predeterminados o globales, en caso de que apliquen
            oDevolucion.Fecha = (oDevolucion.Fecha != DateTime.MinValue ? oDevolucion.Fecha : DateTime.Now);
            oDevolucion.SucursalID = (oDevolucion.SucursalID > 0 ? oDevolucion.SucursalID : GlobalClass.SucursalID);

            // Se guarda la devolución
            Guardar.Generico<VentaDevolucion>(oDevolucion);

            // Se guarda el detalle
            VentaDetalle oParteVenta;
            foreach (var ParteDetalle in oDetalle)
            {
                ParteDetalle.VentaDevolucionID = oDevolucion.VentaDevolucionID;
                Guardar.Generico<VentaDevolucionDetalle>(ParteDetalle);

                // Se quita el producto de la venta
                oParteVenta = General.GetEntity<VentaDetalle>(q => q.Estatus
                    && q.VentaID == oDevolucion.VentaID
                    && q.ParteID == ParteDetalle.ParteID
                    && q.Cantidad == ParteDetalle.Cantidad
                    && q.PrecioUnitario == ParteDetalle.PrecioUnitario
                    && q.Iva == ParteDetalle.Iva);
                
                oParteVenta.Estatus = false;
                Guardar.Generico<VentaDetalle>(oParteVenta, false);

                // Se afecta la existencia
                VentasProc.AgregarExistencia(ParteDetalle.ParteID, GlobalClass.SucursalID, ParteDetalle.Cantidad);
            }

            // Si es cancelación, se cambia el estatus de la venta
            var oVenta = General.GetEntity<Venta>(c => c.VentaID == oDevolucion.VentaID && c.Estatus);
            if (oDevolucion.EsCancelacion)
            {
                // Se verifica si la venta ha tenido pagos
                var oVentaV = General.GetEntity<VentasView>(c => c.VentaID == oDevolucion.VentaID);
                oVenta.VentaEstatusID = (oVentaV.Pagado > 0 ? Cat.VentasEstatus.Cancelada : Cat.VentasEstatus.CanceladaSinPago);
                Guardar.Generico<Venta>(oVenta);
            }

            // Se verifican notas de crédito que pudieran cancelarse, por cliente comisionista
            if (oVenta.ComisionistaClienteID > 0)
            {
                // Se calcula el importe de la comisión que se debe quitar
                var oComisionista = General.GetEntity<Cliente>(q => q.ClienteID == oVenta.ComisionistaClienteID && q.Estatus);
                decimal mComision = 0;
                PreciosParte oPrecios;
                foreach (var ParteD in oDetalle)
                {
                    oPrecios = new PreciosParte(ParteD.ParteID);
                    mComision += (((ParteD.PrecioUnitario + ParteD.Iva) - oPrecios.ObtenerPrecio(oComisionista.ListaDePrecios)) * ParteD.Cantidad);
                }
                // Se genera una nota de crédito negativa
                if (mComision > 0)
                    VentasProc.GenerarNotaDeCredito(oComisionista.ClienteID, (mComision * -1), "", Cat.OrigenesNotaDeCredito.Devolucion, oVenta.VentaID.ToString());
            }

            return new ResAcc(true);
        }

        public static ResAcc VentaGarantia(VentaGarantia oGarantia)
        {
            // Se generan datos predeterminados o globales, en caso de que apliquen
            oGarantia.Fecha = (oGarantia.Fecha != DateTime.MinValue ? oGarantia.Fecha : DateTime.Now);
            oGarantia.SucursalID = (oGarantia.SucursalID > 0 ? oGarantia.SucursalID : GlobalClass.SucursalID);

            // Se guarda la devolución
            Guardar.Generico<VentaGarantia>(oGarantia);

            // Se afectan los datos en el detalle de la venta
            
            // Se quita el producto de la venta, si aplica
            if (oGarantia.AccionID != Cat.VentasGarantiasAcciones.RevisionDeProveedor)
            {
                var oParteVenta = General.GetEntity<VentaDetalle>(q => q.Estatus
                    && q.VentaID == oGarantia.VentaID
                    && q.ParteID == oGarantia.ParteID
                    && q.PrecioUnitario == oGarantia.PrecioUnitario
                    && q.Iva == oGarantia.Iva);
                if (oParteVenta.Cantidad > 1)
                {
                    oParteVenta.Cantidad--;
                    Guardar.Generico<VentaDetalle>(oParteVenta);
                }
                else
                {
                    Guardar.Eliminar<VentaDetalle>(oParteVenta, true);
                }
            }
            
            return new ResAcc(true);
        }

        public static ResAcc c9500(Cotizacion9500 o9500, List<Cotizacion9500Detalle> Detalle)
        {
            // Se guarda la cotización 9500
            Guardar.Generico<Cotizacion9500>(o9500);

            // Se guarda el detalle
            foreach (var Parte in Detalle)
            {
                Parte.Cotizacion9500ID = o9500.Cotizacion9500ID;
                Guardar.Generico<Cotizacion9500Detalle>(Parte);
            }
            
            return new ResAcc(true);
        }

        public static ResAcc Factura(VentaFactura oFactura, List<VentaFacturaDetalle> oDetalle)
        {
            // Se generan datos predeterminados o globales, en caso de que apliquen
            oFactura.Fecha = (oFactura.Fecha != DateTime.MinValue ? oFactura.Fecha : DateTime.Now);
            oFactura.EstatusGenericoID = (oFactura.EstatusGenericoID > 0 ? oFactura.EstatusGenericoID : Cat.EstatusGenericos.Completada);
            oFactura.RealizoUsuarioID = (oFactura.RealizoUsuarioID > 0 ? oFactura.RealizoUsuarioID : GlobalClass.UsuarioGlobal.UsuarioID);
            oFactura.SucursalID = (oFactura.SucursalID > 0 ? oFactura.SucursalID : GlobalClass.SucursalID);

            // Aquí tal vez se podría llenar el número de Factura*
            

            // Se guarda la venta
            Guardar.Generico<VentaFactura>(oFactura);

            // Se guarda el detalle
            foreach (var oVenta in oDetalle)
            {
                oVenta.VentaFacturaID = oFactura.VentaFacturaID;
                Guardar.Generico<VentaFacturaDetalle>(oVenta);

                // Aquí se debe guardar el dato de número de factura en el folio de la venta original
                // ...
            }

            return new ResAcc(true);
        }

        public static ResAcc FacturaDevolucion(VentaFacturaDevolucion oFacturaDev, List<VentaFacturaDevolucionDetalle> oDetalle)
        {
            // Se generan datos predeterminados o globales, en caso de que apliquen
            oFacturaDev.Fecha = (oFacturaDev.Fecha != DateTime.MinValue ? oFacturaDev.Fecha : DateTime.Now);
            oFacturaDev.SucursalID = (oFacturaDev.SucursalID > 0 ? oFacturaDev.SucursalID : GlobalClass.SucursalID);

            // Aquí tal vez se podría llenar el número de Factura*


            // Se guarda la nota de crédito
            Guardar.Generico<VentaFacturaDevolucion>(oFacturaDev);

            // Se guarda el detalle
            foreach (var oDevolucion in oDetalle)
            {
                oDevolucion.VentaFacturaDevolucionID = oFacturaDev.VentaFacturaDevolucionID;
                Guardar.Generico<VentaFacturaDevolucionDetalle>(oDevolucion);

                // Aquí se debe guardar el dato de número de factura en el folio de la venta original
                // ...
            }

            return new ResAcc(true);
        }
                
        #endregion

        #region [ Eliminar específicos ]

        public static ResAcc EliminarParte(int iParteID)
        {
            // Se borran los registros MaxMin
            var oMaxMins = General.GetListOf<ParteMaxMin>(q => q.ParteID == iParteID);
            foreach (var oParteMM in oMaxMins)
                Guardar.Eliminar<ParteMaxMin>(oParteMM, false);
            
            // Se borran los registros de existencias
            var oExistencias = General.GetListOf<ParteExistencia>(q => q.ParteID == iParteID && q.Estatus);
            foreach (var oParteEx in oExistencias)
                Guardar.Eliminar<ParteExistencia>(oParteEx, true);

            // Se borran los datos del precio
            var oPartePrecio = General.GetEntity<PartePrecio>(q => q.ParteID == iParteID && q.Estatus);
            if (oPartePrecio != null)
                Guardar.Eliminar<PartePrecio>(oPartePrecio, true);

            // Se borra el registro de la parte
            var oParte = General.GetEntity<Parte>(q => q.ParteID == iParteID && q.Estatus);
            if (oParte != null)
            Guardar.Eliminar<Parte>(oParte, true);

            return new ResAcc(true);
        }

        #endregion

        #region [ Uso interno ]

        private static void LlenarDatosPredefinidos(EntityObject Objeto)
        {
            Type TipoObjeto = Objeto.GetType();
            PropertyInfo Propiedad;
            DateTime dAhora = DateTime.Now;

            Propiedad = TipoObjeto.GetProperty("UsuarioID");
            if (Propiedad != null)
                Propiedad.SetValue(Objeto, GlobalClass.UsuarioGlobal.UsuarioID, null);
            Propiedad = TipoObjeto.GetProperty("FechaModificacion");
            if (Propiedad != null)
                Propiedad.SetValue(Objeto, dAhora, null);
            
            // Valores que tiene Default en la bd
            if (Objeto.EntityState == EntityState.Detached)
            {
                Propiedad = TipoObjeto.GetProperty("Estatus");
                if (Propiedad != null)
                    Propiedad.SetValue(Objeto, true, null);
                Propiedad = TipoObjeto.GetProperty("Actualizar");
                if (Propiedad != null)
                    Propiedad.SetValue(Objeto, true, null);
                if (Objeto.EntityState == EntityState.Detached)
                {
                    Propiedad = TipoObjeto.GetProperty("FechaRegistro");
                    if (Propiedad != null)
                        Propiedad.SetValue(Objeto, dAhora, null);
                }
            }
        }

        #endregion

    }

    public class ResAcc
    {
        public bool Exito { get; set; }
        public bool Error { get { return !this.Exito; } }
        // public int CodigoDeEstatus { get; set; }
        public string Mensaje { get; set; }
        // public int Respuesta { get; set; }

        public ResAcc(bool bExito)
        {
            this.Exito = bExito;
        }

        public ResAcc(bool bExito, string sMensaje)
        {
            this.Exito = bExito;
            this.Mensaje = sMensaje;
        }
    }

    public class ResAcc<T>
    {
        public bool Exito { get; set; }
        public bool Error { get { return !this.Exito; } }
        public int CodigoDeEstatus { get; set; }
        public string Mensaje { get; set; }
        public T Respuesta { get; set; }

        public ResAcc() { }

        public ResAcc(bool bExito)
        {
            this.Exito = bExito;
        }

        public ResAcc(bool bExito, string sMensaje)
        {
            this.Exito = bExito;
            this.Mensaje = sMensaje;
        }

        public ResAcc(bool bExito, T Respuesta)
        {
            this.Exito = bExito;
            this.Respuesta = Respuesta;
        }

        
    }
}