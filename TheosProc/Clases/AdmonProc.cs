using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using LibUtil;

namespace TheosProc
{
    public static class AdmonProc
    {
        #region [ Relacionado con Partes ]

        public static void AfectarExistenciaYKardex(int iParteID, int iSucursalID, int iOperacionID, string sFolio, int iUsuarioID, string sEntidad
            , string sOrigen, string sDestino, decimal mCantidad, decimal mImporte, string sTabla, int iId)
        {
            // Se manda a afectar la existencia
            AdmonProc.AgregarExistencia(iParteID, iSucursalID, mCantidad, sTabla, iId);

            // Se manda a afectar en el kardex
            var oSucursal = Datos.GetEntity<Sucursal>(c => c.SucursalID == iSucursalID && c.Estatus);
            var oKardex = new ParteKardex()
            {
                ParteID = iParteID,
                OperacionID = iOperacionID,
                SucursalID = iSucursalID,
                Folio = sFolio,
                Fecha = DateTime.Now,
                RealizoUsuarioID = iUsuarioID,
                Entidad = sEntidad,
                Origen = sOrigen,
                Destino = sDestino,
                Cantidad = mCantidad,
                Importe = mImporte,
                RelacionTabla = sTabla,
                RelacionID = iId
            };
            AdmonProc.RegistrarKardex(oKardex);
        }

        public static void AgregarExistencia(int iParteID, int iSucursalID, decimal mAgregar, string sTabla, int iId)
        {
            // var oParte = General.GetEntity<Parte>(q => q.ParteID == iParteID && q.Estatus);
            // if (!oParte.EsServicio.Valor())
            if (Datos.Exists<Parte>(c => c.ParteID == iParteID && (!c.EsServicio.HasValue || !c.EsServicio.Value) && c.Estatus))
            {
                var oParteEx = Datos.GetEntity<ParteExistencia>(q => q.SucursalID == iSucursalID && q.ParteID == iParteID && q.Estatus);
                oParteEx.Existencia += mAgregar;
                Datos.Guardar<ParteExistencia>(oParteEx);

                // Se registra el histórico de la existencia
                var oExHis = new ParteExistenciaHistorico()
                {
                    ParteID = iParteID,
                    Cantidad = mAgregar,
                    ExistenciaNueva = oParteEx.Existencia.Valor(),
                    RelacionTabla = sTabla,
                    RelacionID = iId
                };
                Datos.Guardar<ParteExistenciaHistorico>(oExHis);
            }
        }

        public static void RegistrarKardex(ParteKardex oParteKardex)
        {
            oParteKardex.Folio = (oParteKardex.Folio == null ? "" : oParteKardex.Folio);

            // Se verifica si la parte es servicio, para no afectar la existencia nueva
            if (Datos.Exists<Parte>(c => c.ParteID == oParteKardex.ParteID && c.EsServicio.Value && c.Estatus))
                oParteKardex.Cantidad = 0;

            // Se calcula la existencia nueva
            var oParteKardexAnt = Datos.GetListOf<ParteKardex>(c => c.ParteID == oParteKardex.ParteID && c.SucursalID == oParteKardex.SucursalID)
                .OrderByDescending(c => c.ParteKardexID).FirstOrDefault();
            decimal mExistencia = (oParteKardexAnt == null ? 0 : oParteKardexAnt.ExistenciaNueva);
            decimal mCantidad = oParteKardex.Cantidad;
            /* if (oParteKardex.OperacionID == Cat.OperacionesKardex.Venta || oParteKardex.OperacionID == Cat.OperacionesKardex.DevolucionAProveedor
                || oParteKardex.OperacionID == Cat.OperacionesKardex.SalidaInventario || oParteKardex.OperacionID == Cat.OperacionesKardex.SalidaTraspaso)
                mCantidad *= -1;
            */
            oParteKardex.ExistenciaNueva = (mExistencia + mCantidad);

            Datos.Guardar<ParteKardex>(oParteKardex);

            // Se valida la última entrada del kárdex con la existencia
            /* 
            var oParteEx = General.GetEntity<ParteExistencia>(c => c.ParteID == oParteKardex.ParteID && c.SucursalID == oParteKardex.SucursalID && c.Estatus);
            if (oParteKardex.ExistenciaNueva != oParteEx.Existencia)
                UtilLocal.MensajeError(string.Format("Hay una diferencia entre la existencia del Kárdex y la existencia del sistema. ¡Verificar!"
                    + "\n\nExistencia Kárdex: {0}\nExistencia Sistema: {1}", oParteKardex.ExistenciaNueva, oParteEx.Existencia));
            */
        }

        public static void CopiarAplicacionesDeEquivalentes(int iParteID)
        {
            var oPartesEq = Datos.GetListOf<PartesEquivalentesView>(c => c.ParteID == iParteID);
            var oAplicaciones = Datos.GetListOf<ParteVehiculo>(c => c.ParteID == iParteID);
            foreach (var oParteEq in oPartesEq)
            {
                var oAplisEq = Datos.GetListOf<ParteVehiculo>(c => c.ParteID == oParteEq.ParteIDEquivalente);
                foreach (var oAplicacion in oAplicaciones)
                {
                    if (oAplisEq.Any(c => c.ModeloID == oAplicacion.ModeloID
                        && ((!c.Anio.HasValue && !oAplicacion.Anio.HasValue) || c.Anio == oAplicacion.Anio)
                        && ((!c.MotorID.HasValue && !oAplicacion.MotorID.HasValue) || c.MotorID == oAplicacion.MotorID)))
                        continue;
                    var oAplicacionNueva = new ParteVehiculo()
                    {
                        ParteID = oParteEq.ParteIDEquivalente,
                        ModeloID = oAplicacion.ModeloID,
                        MotorID = oAplicacion.MotorID,
                        Anio = oAplicacion.Anio,
                        TipoFuenteID = oAplicacion.TipoFuenteID,
                        RegistroUsuarioID = Theos.UsuarioID,
                    };
                    Datos.Guardar<ParteVehiculo>(oAplicacionNueva);
                }
            }
        }

        public static void CopiarCodigosAlternosDeEquivalentes(int iParteID)
        {
            var oPartesEq = Datos.GetListOf<PartesEquivalentesView>(c => c.ParteID == iParteID);
            var oCodigosAlt = Datos.GetListOf<ParteCodigoAlterno>(c => c.ParteID == iParteID);
            foreach (var oParteEq in oPartesEq)
            {
                foreach (var oCodigoAlt in oCodigosAlt)
                {
                    if (Datos.Exists<ParteCodigoAlterno>(c => c.ParteID == oParteEq.ParteIDEquivalente && c.MarcaParteID == oCodigoAlt.MarcaParteID
                        && c.CodigoAlterno == oCodigoAlt.CodigoAlterno))
                        continue;
                    var oCodigoAltNuevo = new ParteCodigoAlterno()
                    {
                        ParteID = oParteEq.ParteIDEquivalente,
                        MarcaParteID = oCodigoAlt.MarcaParteID,
                        CodigoAlterno = oCodigoAlt.CodigoAlterno,
                        RealizoUsuarioID = Theos.UsuarioID
                    };
                    Datos.Guardar<ParteCodigoAlterno>(oCodigoAltNuevo);
                }
            }
        }

        public static void CopiarPartesComplementariasDeEquivalentes(int iParteID)
        {
            var oPartesEq = Datos.GetListOf<PartesEquivalentesView>(c => c.ParteID == iParteID);
            var oPartesCom = Datos.GetListOf<PartesComplementariasView>(c => c.ParteID == iParteID);
            foreach (var oParteEq in oPartesEq)
            {
                foreach (var oParteComp in oPartesCom)
                {
                    Guardar.ParteComplementaria(oParteEq.ParteIDEquivalente, oParteComp.ParteIDComplementaria);
                    // Se verifica si se debe guardar la forma inversa
                    if (Datos.Exists<ParteComplementaria>(c => c.ParteID == oParteComp.ParteIDComplementaria && c.ParteIDComplementaria == iParteID))
                        Guardar.ParteComplementaria(oParteComp.ParteIDComplementaria, oParteEq.ParteIDEquivalente);
                }
            }
        }

        public static List<string> ObtenerImagenesParte(int iParteID)
        {
            return Directory.GetFiles(Theos.RutaImagenes, (iParteID.ToString() + "#*"), SearchOption.TopDirectoryOnly).ToList();
        }

        public static string ObtenerImagenParte(int iParteID)
        {
            var oImagenes = AdmonProc.ObtenerImagenesParte(iParteID);
            return (oImagenes.Count > 0 ? oImagenes[0] : "");
        }

        public static Dictionary<int, int> ObtenerCantidadDeImagenesPorParte()
        {
            var oCantImagenes = new Dictionary<int, int>();
            var aImagenes = Directory.GetFiles(Theos.RutaImagenes);
            foreach (string sImagen in aImagenes)
            {
                string sParteID = Regex.Match(Path.GetFileName(sImagen), @"^\d+").Value;
                int iParteID = Util.Entero(sParteID);
                if (oCantImagenes.ContainsKey(iParteID))
                    oCantImagenes[iParteID]++;
                else
                    oCantImagenes.Add(iParteID, 1);
            }

            return oCantImagenes;
        }

        public static bool VerGuardar9500(int iParteID)
        {
            var oParte = Datos.GetEntity<Parte>(c => c.ParteID == iParteID && c.Estatus);
            oParte.Es9500 = !Datos.Exists<ParteMaxMin>(c => c.ParteID == iParteID && c.Maximo > 0);
            Datos.Guardar<Parte>(oParte);
            return oParte.Es9500.Valor();
        }

        #endregion

        #region [ Relacionado con Proveedores ]

        public static MovimientoInventario CrearDevolucionProveedor(int iProveedorID, int iOrigenID, decimal mImporte, string sObservacion)
        {
            var oMov = new MovimientoInventario()
            {
                TipoOperacionID = Cat.TiposDeOperacionMovimientos.DevolucionAProveedor,
                ProveedorID = iProveedorID,
                // DevolucionOrigenID = iOrigenID,
                ImporteTotal = mImporte,
                Observacion = sObservacion
            };
            Datos.Guardar<MovimientoInventario>(oMov);

            return oMov;
        }

        public static ProveedorNotaDeCredito CrearNotaDeCreditoProveedor(ProveedorNotaDeCredito oNota)
        {
            // Se completan datos que pudieran faltar
            oNota.Fecha = (oNota.Fecha == DateTime.MinValue ? DateTime.Now : oNota.Fecha);
            oNota.RealizoUsuarioID = (oNota.RealizoUsuarioID == 0 ? Theos.UsuarioID : oNota.RealizoUsuarioID);

            Datos.Guardar<ProveedorNotaDeCredito>(oNota);
            return oNota;
        }

        public static void VerMarcarDisponibilidadNotaDeCreditoProveedor(int iNotaDeCreditoID)
        {
            var oNotaView = Datos.GetEntity<ProveedoresNotasDeCreditoView>(c => c.ProveedorNotaDeCreditoID == iNotaDeCreditoID);
            bool bDisponible = (oNotaView.Restante > 0);
            if (oNotaView.Disponible != bDisponible)
            {
                var oNota = Datos.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iNotaDeCreditoID);
                oNota.Disponible = bDisponible;
                Datos.Guardar<ProveedorNotaDeCredito>(oNota);
            }
        }

        public static void VerMarcarDisponibilidadGastoDeCajaParaProveedor(int iCajaEgresoID)
        {
            var oGastoView = Datos.GetEntity<CajaEgresosProveedoresView>(c => c.CajaEgresoID == iCajaEgresoID);
            bool bDisponible = (oGastoView.Restante > 0);
            if (oGastoView.AfectadoEnProveedores.Valor() == bDisponible)
            {
                var oGasto = Datos.GetEntity<CajaEgreso>(c => c.CajaEgresoID == iCajaEgresoID && c.Estatus);
                oGasto.AfectadoEnProveedores = !bDisponible;
                Datos.Guardar<CajaEgreso>(oGasto);
            }
        }

        public static ProveedorParteGanancia ObtenerParteDescuentoGanancia(int? iProveedorID, int? iMarcaID, int? iLineaID, int? iParteID, bool bCompletarConParte)
        {
            if (bCompletarConParte)
            {
                var oParte = Datos.GetEntity<Parte>(c => c.ParteID == iParteID && c.Estatus);
                if (!iProveedorID.HasValue)
                    iProveedorID = oParte.ProveedorID;
                if (!iMarcaID.HasValue)
                    iMarcaID = oParte.MarcaParteID;
                if (!iLineaID.HasValue)
                    iLineaID = oParte.LineaID;
            }

            var oParteGan = Datos.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID && c.MarcaParteID == iMarcaID
                && c.LineaID == iLineaID && c.ParteID == iParteID);
            if (oParteGan == null)
            {
                oParteGan = Datos.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID && c.MarcaParteID == iMarcaID
                    && c.LineaID == iLineaID);
                if (oParteGan == null)
                {
                    oParteGan = Datos.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID && c.MarcaParteID == iMarcaID);
                    if (oParteGan == null)
                    {
                        oParteGan = Datos.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID);
                    }
                }
            }

            return oParteGan;
        }

        public static ProveedorParteGanancia ObtenerParteDescuentoGanancia(int? iProveedorID, int? iMarcaID, int? iLineaID, int? iParteID)
        {
            return AdmonProc.ObtenerParteDescuentoGanancia(iProveedorID, iMarcaID, iLineaID, iParteID, false);
        }

        public static DateTime SugerirVencimientoCompra(DateTime dBase, int iDiasDePlazo)
        {
            dBase = dBase.AddDays(iDiasDePlazo);
            if (dBase.DayOfWeek == DayOfWeek.Saturday)
                dBase = dBase.AddDays(-1);
            else if (dBase.DayOfWeek == DayOfWeek.Sunday)
                dBase = dBase.AddDays(1);
            return dBase;
        }

        #endregion

        #region [ Web ]

        public static ResAcc RecibirTraspaso(int iUsuarioID, string sMotivo, List<modDetalleTraspaso> oDetalle, bool bValidarContingencia)
        {
            if (oDetalle.Count <= 0)
                return new ResAcc("No hay nada que recibir.");

            // Validar que lo recibido sea menor o igual que lo enviado
            foreach (var oReg in oDetalle)
            {
                if (oReg.Recibido > oReg.Cantidad)
                    return new ResAcc("Existen uno o más artículos que tienen una cantidad recibida mayor a lo enviado.");
            }

            // Se verifica si existe alguna contingencia
            bool bExisteContingencia = false;
            foreach (var oReg in oDetalle)
            {
                if (oReg.Recibido < oReg.Cantidad)
                {
                    bExisteContingencia = true;
                    break;
                }
            }

            if (bExisteContingencia && bValidarContingencia)
                return new ResAcc("El traspaso tiene uno o más conflictos.", Cat.CodigosRes.ConflictoEnTraspasos);

            // Se valida el motivo, en caso de que haya habido contingencia
            if (bExisteContingencia && string.IsNullOrEmpty(sMotivo))
                return new ResAcc("Debes especificar un motivo.");

            // Se obtiene el MovimientoInventario correspondiente al traspaso
            int iRecibirUnoID = oDetalle[0].MovimientoInventarioDetalleID;
            var oRecibirUno = Datos.GetEntity<MovimientoInventarioDetalle>(c => c.MovimientoInventarioDetalleID == iRecibirUnoID && c.Estatus);
            int iMovID = oRecibirUno.MovimientoInventarioID;
            var oTraspasoV = Datos.GetEntity<MovimientoInventarioView>(c => c.MovimientoInventarioID == iMovID);
            int iSucursalID = oTraspasoV.SucursalDestinoID.Valor();

            //
            decimal mCostoTotal = 0;
            foreach (var oReg in oDetalle)
            {
                // Si la cantidad recibida es menor a lo enviado, almacenar la contingencia
                if (oReg.Recibido < oReg.Cantidad)
                {
                    var contingencia = new MovimientoInventarioTraspasoContingencia()
                    {
                        MovimientoInventarioID = iMovID,
                        MovimientoInventarioDetalleID = oReg.MovimientoInventarioDetalleID,
                        ParteID = oReg.ParteID,
                        CantidadEnviada = oReg.Cantidad,
                        CantidadRecibida = oReg.Recibido,
                        CantidadDiferencia = (oReg.Cantidad - oReg.Recibido),
                        Comentario = sMotivo,
                        UsuarioID = iUsuarioID,
                        MovimientoInventarioEstatusContingenciaID = Cat.TraspasosContingenciasEstatus.NoSolucionado
                    };
                    Datos.Guardar<MovimientoInventarioTraspasoContingencia>(contingencia);
                }

                //Aumentar la existencia actual de la sucursal destino
                var oParte = Datos.GetEntity<Parte>(c => c.ParteID == oReg.ParteID && c.Estatus);
                if (!oParte.EsServicio.Valor())
                {
                    var existencia = Datos.GetEntity<ParteExistencia>(p => p.ParteID == oReg.ParteID && p.SucursalID == iSucursalID);
                    if (existencia != null)
                    {
                        var inicial = existencia.Existencia;
                        existencia.Existencia += oReg.Recibido;
                        existencia.UsuarioID = iUsuarioID;
                        existencia.FechaModificacion = DateTime.Now;
                        Datos.Guardar<ParteExistencia>(existencia);

                        var historial = new MovimientoInventarioHistorial()
                        {
                            MovmientoInventarioID = iMovID,
                            ParteID = oReg.ParteID,
                            ExistenciaInicial = Util.Decimal(inicial),
                            ExistenciaFinal = Util.Decimal(existencia.Existencia),
                            SucursalID = iSucursalID,
                            UsuarioID = iUsuarioID
                        };
                        Datos.Guardar<MovimientoInventarioHistorial>(historial);
                    }
                }

                // Se agrega al Kardex
                var oPartePrecio = Datos.GetEntity<PartePrecio>(c => c.ParteID == oReg.ParteID && c.Estatus);
                AdmonProc.RegistrarKardex(new ParteKardex()
                {
                    ParteID = oReg.ParteID,
                    OperacionID = Cat.OperacionesKardex.EntradaTraspaso,
                    SucursalID = iSucursalID,
                    Folio = iMovID.ToString(),
                    Fecha = DateTime.Now,
                    RealizoUsuarioID = iUsuarioID,
                    Entidad = Util.Cadena(oTraspasoV.NombreProveedor),
                    Origen = oTraspasoV.SucursalOrigen,
                    Destino = oTraspasoV.SucursalDestino,
                    Cantidad = oReg.Recibido,
                    Importe = oPartePrecio.Costo.Valor()
                });

                // Se suma el importe de cada parte, para crear la póliza
                mCostoTotal += oPartePrecio.Costo.Valor();
            }

            // Se genera la póliza especial correspondiente (AfeConta)
            var oUsuario = Datos.GetEntity<Usuario>(c => c.UsuarioID == iUsuarioID && c.Estatus);
            var oPoliza = ContaProc.CrearPoliza(Cat.ContaTiposDePoliza.Diario, string.Format("TRASPASO ORIGEN {0:00} DESTINO {1:00}"
                , oTraspasoV.SucursalOrigenID, oTraspasoV.SucursalDestinoID), Cat.ContaCuentasAuxiliares.Inventario, 0, mCostoTotal
                , oUsuario.NombreUsuario, Cat.Tablas.MovimientoInventario, iMovID, iSucursalID);

            //Actualizar el movimiento con los datos (fecha y usuario que recibio)
            var movimiento = Datos.GetEntity<MovimientoInventario>(m => m.MovimientoInventarioID == iMovID);
            if (null != movimiento)
            {
                movimiento.ExisteContingencia = bExisteContingencia;
                movimiento.UsuarioRecibioTraspasoID = iUsuarioID;
                movimiento.FechaRecepcion = DateTime.Now;
                movimiento.FechaModificacion = DateTime.Now;
                Datos.Guardar<MovimientoInventario>(movimiento);
            }

            return new ResAcc();
        }

        public static ResAcc RecibirConteoInventario(int iUsuarioID, List<modConteoInventario> oConteo)
        {
            // Se obtiene la sucursal correspondiente a los conteos, pues debe ser la misma para todos
            if (oConteo.Count <= 0)
                return new ResAcc();
            int iPrimerInvID = oConteo[0].InventarioLineaID;
            var oPrimerInv = Datos.GetEntity<InventarioLinea>(c => c.InventarioLineaID == iPrimerInvID);
            int iSucursalID = oPrimerInv.SucursalID;

            // Se registra lo del conteo
            oConteo = oConteo.OrderBy(c => c.InventarioLineaID).ToList();
            int iInventarioLineaID = 0;
            List<InventarioLinea> oInvConteos = new List<InventarioLinea>();
            InventarioLinea oInventarioLinea = null;
            List<InventarioConteo> oDiferencias1 = new List<InventarioConteo>();
            List<InventarioConteo> oDiferencias2 = new List<InventarioConteo>();
            List<InventarioConteo> oDiferencias3 = new List<InventarioConteo>();
            foreach (var oReg in oConteo)
            {
                if (oReg.InventarioLineaID != iInventarioLineaID)
                {
                    oInventarioLinea = Datos.GetEntity<InventarioLinea>(c => c.InventarioLineaID == oReg.InventarioLineaID);
                    oInvConteos.Add(oInventarioLinea);
                    iInventarioLineaID = oReg.InventarioLineaID;
                }

                // Se obtiene el conteo y la existencia, para saber si hay diferencia
                var oParteConteo = Datos.GetEntity<InventarioConteo>(c => c.InventarioConteoID == oReg.InventarioConteoID);
                var oExistencia = Datos.GetEntity<ParteExistencia>(c => c.ParteID == oParteConteo.ParteID && c.SucursalID == oInventarioLinea.SucursalID && c.Estatus);
                decimal mDiferencia = (oReg.Conteo - oExistencia.Existencia.Valor());

                // Se evalua la diferencia
                oParteConteo.Valido = true;
                switch (oParteConteo.Revision)
                {
                    case null:
                    case Cat.InventarioConteosRevisiones.SinRevision:
                        if (mDiferencia != 0)
                        {
                            if (oInventarioLinea.LineaID.HasValue)
                                oDiferencias1.Add(oParteConteo);
                            else
                                // Es por devolución o cancelación del día anterior
                                oDiferencias2.Add(oParteConteo);

                            oParteConteo.Valido = false;
                        }
                        break;
                    case Cat.InventarioConteosRevisiones.Confirmacion:
                        var oParteConteoAnt = Datos.GetEntity<InventarioConteo>(c => c.InventarioLineaID == oParteConteo.InventarioLineaID && !c.Revision.HasValue);
                        if (oParteConteoAnt.Diferencia == mDiferencia)
                        {
                            oDiferencias3.Add(oParteConteo);
                        }
                        else
                        {
                            oDiferencias2.Add(oParteConteo);
                            oParteConteo.Valido = false;
                        }
                        break;
                    case Cat.InventarioConteosRevisiones.ConfirmacionGerente:
                        if (oInventarioLinea.LineaID.HasValue)
                        {
                            if (mDiferencia != 0)
                                oDiferencias3.Add(oParteConteo);
                        }
                        break;
                }

                // Se guarda la diferencia
                oParteConteo.RealizoUsuarioID = iUsuarioID;
                oParteConteo.Diferencia = mDiferencia;
                Datos.Guardar<InventarioConteo>(oParteConteo);
            }

            // Se obtienen los usuarios para inventario de todas las sucursales
            var oUsuariosInv = Datos.GetListOf<InventarioUsuario>();

            // Se obtienen los usuarios para inventario de la sucursal
            var oUsuariosSuc = oUsuariosInv.Where(c => c.SucursalID == iSucursalID).ToList();
            int iCantidadPorUsuario = (oDiferencias1.Count / oUsuariosSuc.Count);
            // Se agregan los conteos para revisión 1 (de confirmación)
            int iUsuario = 0, iCantidad = 0;
            foreach (var oReg in oDiferencias1)
            {
                var oNuevoConteo = new InventarioConteo()
                {
                    InventarioLineaID = oReg.InventarioLineaID,
                    Dia = DateTime.Now.AddDays(1),
                    ConteoUsuarioID = oUsuariosSuc[iUsuario].InvUsuarioID,
                    ParteID = oReg.ParteID,
                    Revision = Cat.InventarioConteosRevisiones.Confirmacion
                };
                Datos.Guardar<InventarioConteo>(oNuevoConteo);

                if (++iCantidad >= iCantidadPorUsuario)
                {
                    if (++iUsuario >= oUsuariosSuc.Count)
                        iUsuario = 0;
                }
            }

            // Se agregan los conteos para revisión 2 (confirmación del gerente)
            var oSucursal = Datos.GetEntity<Sucursal>(c => c.SucursalID == iSucursalID && c.Estatus);
            int iGerenteID = oSucursal.GerenteID;
            foreach (var oReg in oDiferencias2)
            {
                var oNuevoConteo = new InventarioConteo()
                {
                    InventarioLineaID = oReg.InventarioLineaID,
                    Dia = DateTime.Now,
                    ConteoUsuarioID = iGerenteID,
                    ParteID = oReg.ParteID,
                    Revision = Cat.InventarioConteosRevisiones.ConfirmacionGerente
                };
                Datos.Guardar<InventarioConteo>(oNuevoConteo);
            }

            // Se agregan los conteos para revisión 3 (otras sucursales si la existencia es 0)
            var oIndiceUsuarioSucursal = new Dictionary<int, int>();
            var oInvAbiertos = new List<int>();  // Para marcar como incompletos los inventarios que se "abran" por meter un nuevo conteo tipo 3
            foreach (var oReg in oDiferencias3)
            {
                // Se valida si la existencia en otras sucursales es cero
                var oExistencias = Datos.GetListOf<ParteExistencia>(c => c.ParteID == oReg.ParteID && c.SucursalID != iSucursalID);
                foreach (var oExist in oExistencias)
                {
                    if (oExist.Existencia == 0)
                    {
                        // Para determinar el usuario al cual asignar
                        oUsuariosSuc = oUsuariosInv.Where(c => c.SucursalID == oExist.SucursalID).ToList();
                        if (!oIndiceUsuarioSucursal.ContainsKey(oExist.SucursalID))
                            oIndiceUsuarioSucursal.Add(oExist.SucursalID, 0);

                        var oNuevoConteo = new InventarioConteo()
                        {
                            InventarioLineaID = oReg.InventarioLineaID,
                            Dia = DateTime.Now.AddDays(1),
                            ConteoUsuarioID = oUsuariosSuc[oIndiceUsuarioSucursal[oExist.SucursalID]].InvUsuarioID,
                            ParteID = oReg.ParteID,
                            Revision = Cat.InventarioConteosRevisiones.OtraSucursal
                        };
                        Datos.Guardar<InventarioConteo>(oNuevoConteo);

                        //
                        if (!oInvAbiertos.Contains(oReg.InventarioLineaID))
                            oInvAbiertos.Add(oReg.InventarioLineaID);

                        // 
                        if (++oIndiceUsuarioSucursal[oExist.SucursalID] >= oUsuariosSuc.Count)
                            oIndiceUsuarioSucursal[oExist.SucursalID] = 0;
                    }
                }
            }
            // Se marcan como incompletos los inventarios abiertos por este proceso
            foreach (int iInvID in oInvAbiertos)
            {
                var oInv = Datos.GetEntity<InventarioLinea>(c => c.InventarioLineaID == iInvID);
                oInv.EstatusGenericoID = Cat.EstatusGenericos.EnRevision;
                oInv.FechaCompletado = null;
                Datos.Guardar<InventarioLinea>(oInv);
            }

            // Se marcan como completado los inventarios ya concluidos, si hubiera
            foreach (var oReg in oInvConteos)
            {
                var oInvConteoV = Datos.GetEntity<InventarioLineasConteosView>(c => c.InventarioLineaID == oReg.InventarioLineaID);
                // Se valida si ya se completó el conteo
                if (oInvConteoV.PartesLinea > oInvConteoV.Conteo)
                {
                    continue;
                }
                else
                {
                    oReg.EstatusGenericoID = Cat.EstatusGenericos.Completada;
                    oReg.FechaCompletado = DateTime.Now;
                }

                Datos.Guardar<InventarioLinea>(oReg);
            }

            return new ResAcc();
        }

        #endregion
    }
}
