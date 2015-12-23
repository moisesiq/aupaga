using System;
using System.Collections.Generic;
using System.Linq;

using LibUtil;

namespace TheosProc
{
    public static class AdmonProc
    {
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
            if (oParteKardex.OperacionID == Cat.OperacionesKardex.Venta || oParteKardex.OperacionID == Cat.OperacionesKardex.DevolucionAProveedor
                || oParteKardex.OperacionID == Cat.OperacionesKardex.SalidaInventario || oParteKardex.OperacionID == Cat.OperacionesKardex.SalidaTraspaso)
                mCantidad *= -1;
            oParteKardex.ExistenciaNueva = (mExistencia + mCantidad);

            Datos.Guardar<ParteKardex>(oParteKardex);
        }

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
                        MovimientoInventarioEstatusContingenciaID = Cat.TraspasoEstatusContingencias.NoSolucionado
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
                            oDiferencias1.Add(oParteConteo);
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
                        if (mDiferencia != 0)
                            oDiferencias3.Add(oParteConteo);
                        break;
                }

                // Se guarda la diferencia
                oParteConteo.Diferencia = mDiferencia;
                Datos.Guardar<InventarioConteo>(oParteConteo);
            }

            // Se obtienen los usuarios para inventario de todas las sucursales
            var oUsuariosInv = Datos.GetListOf<InventarioUsuario>();

            // Se obtienen los usuario para inventario de la sucursal
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
    }
}
