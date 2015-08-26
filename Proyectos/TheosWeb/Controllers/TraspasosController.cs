using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TheosProc;

namespace TheosWeb.Controllers
{
    public class TraspasosController : BaseController
    {
        public JsonResult TraspasosARecibir(int id)
        {
            var oTraspasos = Datos.GetListOf<MovimientoInventarioTraspasosView>(c => c.TipoOperacionID == Cat.TipoOperacionMovimiento.Traspaso
                && c.SucursalDestinoID == id && !c.FechaRecepcion.HasValue);
            return this.Json(oTraspasos);
        }

        public JsonResult DetalleTraspaso(int id)
        {
            var oDetalle = Datos.GetListOf<MovimientosInventarioDetalleAvanzadoView>(c => c.MovimientoInventarioID == id);
            return this.Json(oDetalle);
        }

        public JsonResult RecibirTraspaso(int eParam1, string eParam2, List<modDetalleTraspaso> eParam3, bool eParam4)
        {
            var oRes = AdmonProc.RecibirTraspaso(eParam1, eParam2, eParam3, eParam4);
            return this.Json(oRes);
        }
	}
}