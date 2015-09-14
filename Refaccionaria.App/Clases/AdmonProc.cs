using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Refaccionaria.Modelo;
using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    public static class AdmonProc
    {
        #region [ Relacionado con Partes ]

        public static void AfectarExistenciaYKardex(int iParteID, int iSucursalID, int iOperacionID, string sFolio, int iUsuarioID, string sEntidad
            , string sOrigen, string sDestino, decimal mCantidad, decimal mImporte)
        {
            var oParteEx = General.GetEntity<ParteExistencia>(c => c.ParteID == iParteID && c.SucursalID == iSucursalID && c.Estatus);
            if (iOperacionID == Cat.OperacionesKardex.Venta || iOperacionID == Cat.OperacionesKardex.DevolucionAProveedor
                || iOperacionID == Cat.OperacionesKardex.SalidaInventario || iOperacionID == Cat.OperacionesKardex.SalidaTraspaso)
                oParteEx.Existencia -= mCantidad;
            else
                oParteEx.Existencia += mCantidad;
            Guardar.Generico<ParteExistencia>(oParteEx);

            var oSucursal = General.GetEntity<Sucursal>(c => c.SucursalID == iSucursalID && c.Estatus);
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
                Importe = mImporte
            };
            AdmonProc.RegistrarKardex(oKardex);
        }

        public static void RegistrarKardex(ParteKardex oParteKardex)
        {
            oParteKardex.Folio = (oParteKardex.Folio == null ? "" : oParteKardex.Folio);

            // Se verifica si la parte es servicio, para no afectar la existencia nueva
            if (General.Exists<Parte>(c => c.ParteID == oParteKardex.ParteID && c.EsServicio.Value && c.Estatus))
                oParteKardex.Cantidad = 0;

            // Se calcula la existencia nueva
            var oParteKardexAnt = General.GetListOf<ParteKardex>(c => c.ParteID == oParteKardex.ParteID && c.SucursalID == oParteKardex.SucursalID)
                .OrderByDescending(c => c.ParteKardexID).FirstOrDefault();
            decimal mExistencia = (oParteKardexAnt == null ? 0 : oParteKardexAnt.ExistenciaNueva);
            decimal mCantidad = oParteKardex.Cantidad;
            if (oParteKardex.OperacionID == Cat.OperacionesKardex.Venta || oParteKardex.OperacionID == Cat.OperacionesKardex.DevolucionAProveedor
                || oParteKardex.OperacionID == Cat.OperacionesKardex.SalidaInventario || oParteKardex.OperacionID == Cat.OperacionesKardex.SalidaTraspaso)
                mCantidad *= -1;
            oParteKardex.ExistenciaNueva = (mExistencia + mCantidad);

            Guardar.Generico<ParteKardex>(oParteKardex);

            // Se valida la última entrada del kárdex con la existencia
            var oParteEx = General.GetEntity<ParteExistencia>(c => c.ParteID == oParteKardex.ParteID && c.SucursalID == oParteKardex.SucursalID && c.Estatus);
            if (oParteKardex.ExistenciaNueva != oParteEx.Existencia)
                UtilLocal.MensajeError(string.Format("Hay una diferencia entre la existencia del Kárdex y la existencia del sistema. ¡Verificar!"
                    + "\n\nExistencia Kárdex: {0}\nExistencia Sistema: {1}", oParteKardex.ExistenciaNueva, oParteEx.Existencia));
        }

        public static void CopiarAplicacionesDeEquivalentes(int iParteID)
        {
            var oPartesEq = General.GetListOf<PartesEquivalentesView>(c => c.ParteID == iParteID);
            var oAplicaciones = General.GetListOf<ParteVehiculo>(c => c.ParteID == iParteID);
            foreach (var oParteEq in oPartesEq)
            {
                foreach (var oAplicacion in oAplicaciones)
                {
                    if (General.Exists<ParteVehiculo>(c => c.ParteID == oParteEq.ParteIDEquivalente && c.ModeloID == oAplicacion.ModeloID
                        && c.MotorID == oAplicacion.MotorID && c.Anio == oAplicacion.Anio))
                        continue;
                    var oAplicacionNueva = new ParteVehiculo()
                    {
                        ParteID = oParteEq.ParteIDEquivalente,
                        ModeloID = oAplicacion.ModeloID,
                        MotorID = oAplicacion.MotorID,
                        Anio = oAplicacion.Anio,
                        TipoFuenteID = oAplicacion.TipoFuenteID,
                        RegistroUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID,
                    };
                    Guardar.Generico<ParteVehiculo>(oAplicacionNueva);
                }
            }
        }

        public static void CopiarCodigosAlternosDeEquivalentes(int iParteID)
        {
            var oPartesEq = General.GetListOf<PartesEquivalentesView>(c => c.ParteID == iParteID);
            var oCodigosAlt = General.GetListOf<ParteCodigoAlterno>(c => c.ParteID == iParteID && c.Estatus);
            foreach (var oParteEq in oPartesEq)
            {
                foreach (var oCodigoAlt in oCodigosAlt)
                {
                    if (General.Exists<ParteCodigoAlterno>(c => c.ParteID == oParteEq.ParteIDEquivalente && c.MarcaParteID == oCodigoAlt.MarcaParteID
                        && c.CodigoAlterno == oCodigoAlt.CodigoAlterno))
                        continue;
                    var oCodigoAltNuevo = new ParteCodigoAlterno()
                    {
                        ParteID = oParteEq.ParteIDEquivalente,
                        MarcaParteID = oCodigoAlt.MarcaParteID,
                        CodigoAlterno = oCodigoAlt.CodigoAlterno,
                        RealizoUsuarioID = GlobalClass.UsuarioGlobal.UsuarioID
                    };
                    Guardar.Generico<ParteCodigoAlterno>(oCodigoAltNuevo);
                }
            }
        }

        public static void CopiarPartesComplementariasDeEquivalentes(int iParteID)
        {
            var oPartesEq = General.GetListOf<PartesEquivalentesView>(c => c.ParteID == iParteID);
            var oPartesCom = General.GetListOf<PartesComplementariasView>(c => c.ParteID == iParteID);
            foreach (var oParteEq in oPartesEq)
            {
                foreach (var oParteComp in oPartesCom)
                {
                    Guardar.ParteComplementaria(oParteEq.ParteIDEquivalente, oParteComp.ParteIDComplementaria);
                    // Se verifica si se debe guardar la forma inversa
                    if (General.Exists<ParteComplementaria>(c => c.ParteID == oParteComp.ParteIDComplementaria && c.ParteIDComplementaria == iParteID))
                        Guardar.ParteComplementaria(oParteComp.ParteIDComplementaria, oParteEq.ParteIDEquivalente);
                }
            }
        }

        public static List<string> ObtenerImagenesParte(int iParteID)
        {
            return Directory.GetFiles(GlobalClass.ConfiguracionGlobal.pathImagenes, (iParteID.ToString() + "#*"), SearchOption.TopDirectoryOnly).ToList();
        }

        public static string ObtenerImagenParte(int iParteID)
        {
            var oImagenes = AdmonProc.ObtenerImagenesParte(iParteID);
            return (oImagenes.Count > 0 ? oImagenes[0] : "");
        }

        public static Dictionary<int, int> ObtenerCantidadDeImagenesPorParte()
        {
            var oCantImagenes = new Dictionary<int, int>();
            var aImagenes = Directory.GetFiles(GlobalClass.ConfiguracionGlobal.pathImagenes);
            foreach (string sImagen in aImagenes)
            {
                string sParteID = Regex.Match(Path.GetFileName(sImagen), @"^\d+").Value;
                int iParteID = Helper.ConvertirEntero(sParteID);
                if (oCantImagenes.ContainsKey(iParteID))
                    oCantImagenes[iParteID]++;
                else
                    oCantImagenes.Add(iParteID, 1);
            }

            return oCantImagenes;
        }

        public static bool VerGuardar9500(int iParteID)
        {
            var oParte = General.GetEntity<Parte>(c => c.ParteID == iParteID && c.Estatus);
            oParte.Es9500 = !General.Exists<ParteMaxMin>(c => c.ParteID == iParteID && c.Maximo > 0);
            Guardar.Generico<Parte>(oParte);
            return oParte.Es9500.Valor();
        }

        #endregion

        #region [ Relacionado con Proveedores ]

        public static void MostrarRecordatorioPedidos(object state)
        {
            int iEventoID = Helper.ConvertirEntero(state);
            var oEvento = General.GetEntity<ProveedorEventoCalendario>(c => c.ProveedorEventoCalendarioID == iEventoID);

            // Se valida que no se haya hecho ya un pedido el día de hoy
            if (General.Exists<Pedido>(c => c.ProveedorID == oEvento.ProveedorID && c.PedidoEstatusID != Cat.PedidosEstatus.Cancelado && c.Estatus))
            {
                oEvento.Revisado = true;
                Guardar.Generico<ProveedorEventoCalendario>(oEvento);
                return;
            }

            var oProveedor = General.GetEntity<Proveedor>(c => c.ProveedorID == oEvento.ProveedorID && c.Estatus);
            var frmEvento = new RecordatorioEvento(oEvento);

            var oMetodo = new Action(() =>
            {
                frmEvento.ShowDialog(Principal.Instance);
                frmEvento.Dispose();
            });

            if (Principal.Instance.InvokeRequired)
                Principal.Instance.Invoke(oMetodo);
            else
                oMetodo.Invoke();
        }

        public static void MostrarRecordatorioClientes(object state)
        {
            int iEventoID = Helper.ConvertirEntero(state);
            var oEvento = General.GetEntity<ClienteEventoCalendario>(c => c.ClienteEventoCalendarioID == iEventoID);
            var oCliente = General.GetEntity<Cliente>(c => c.ClienteID == oEvento.ClienteID && c.Estatus);

            var oMetodo = new Action(() =>
            {
                Helper.MensajeInformacion(string.Format("{0}\nCliente: {1}\nFecha: {2}", oEvento.Evento, oCliente.Nombre, oEvento.Fecha)
                    , "Recordatorio de Evento");
                // Se marca como revisado
                oEvento.Revisado = true;
                Guardar.Generico<ClienteEventoCalendario>(oEvento);
            });

            if (Principal.Instance.InvokeRequired)
                Principal.Instance.Invoke(oMetodo);
            else
                oMetodo.Invoke();
        }

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
            Guardar.Generico<MovimientoInventario>(oMov);

            return oMov;
        }

        public static ProveedorNotaDeCredito CrearNotaDeCreditoProveedor(ProveedorNotaDeCredito oNota)
        {
            // Se completan datos que pudieran faltar
            oNota.Fecha = (oNota.Fecha == DateTime.MinValue ? DateTime.Now : oNota.Fecha);
            oNota.RealizoUsuarioID = (oNota.RealizoUsuarioID == 0 ? GlobalClass.UsuarioGlobal.UsuarioID : oNota.RealizoUsuarioID);

            Guardar.Generico<ProveedorNotaDeCredito>(oNota);
            return oNota;
        }

        public static void VerMarcarDisponibilidadNotaDeCreditoProveedor(int iNotaDeCreditoID)
        {
            if (General.Exists<ProveedoresNotasDeCreditoView>(c => c.ProveedorNotaDeCreditoID == iNotaDeCreditoID && c.Restante <= 0))
            {
                var oNota = General.GetEntity<ProveedorNotaDeCredito>(c => c.ProveedorNotaDeCreditoID == iNotaDeCreditoID);
                oNota.Disponible = false;
                Guardar.Generico<ProveedorNotaDeCredito>(oNota);
            }
        }

        public static ProveedorParteGanancia ObtenerParteDescuentoGanancia(int? iProveedorID, int? iMarcaID, int? iLineaID, int? iParteID, bool bCompletarConParte)
        {
            if (bCompletarConParte)
            {
                var oParte = General.GetEntity<Parte>(c => c.ParteID == iParteID && c.Estatus);
                if (!iProveedorID.HasValue)
                    iProveedorID = oParte.ProveedorID;
                if (!iMarcaID.HasValue)
                    iMarcaID = oParte.MarcaParteID;
                if (!iLineaID.HasValue)
                    iLineaID = oParte.LineaID;
            }

            var oParteGan = General.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID && c.MarcaParteID == iMarcaID
                && c.LineaID == iLineaID && c.ParteID == iParteID);
            if (oParteGan == null)
            {
                oParteGan = General.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID && c.MarcaParteID == iMarcaID
                    && c.LineaID == iLineaID);
                if (oParteGan == null)
                {
                    oParteGan = General.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID && c.MarcaParteID == iMarcaID);
                    if (oParteGan == null)
                    {
                        oParteGan = General.GetEntity<ProveedorParteGanancia>(c => c.ProveedorID == iProveedorID);
                    }
                }
            }

            return oParteGan;
        }

        public static ProveedorParteGanancia ObtenerParteDescuentoGanancia(int? iProveedorID, int? iMarcaID, int? iLineaID, int? iParteID)
        {
            return AdmonProc.ObtenerParteDescuentoGanancia(iProveedorID, iMarcaID, iLineaID, iParteID, false);
        }

        #endregion
    }
}
