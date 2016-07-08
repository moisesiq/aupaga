using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LibUtil;
using TheosProc;

namespace TheosWeb.Controllers
{
    public class InventarioController : BaseController
    {
        public JsonResult ConteoDelDia(int id)
        {
            DateTime dHoy = DateTime.Today;
            var oConteo = Datos.GetListOf<InventarioConteosView>(c => c.Dia == dHoy && c.ConteoUsuarioID == id && !c.Diferencia.HasValue);
            return this.Json(oConteo);
        }

        public JsonResult RecibirConteo(int eParam1, List<modConteoInventario> eParam2)
        {
            var oRes = AdmonProc.RecibirConteoInventario(eParam1, eParam2);
            return this.Json(oRes);
        }

        public JsonResult UsuariosConteo(int id)
        {
            var oUsuarios = Datos.GetListOf<InventarioUsuariosView>(c => c.SucursalID == id);
            return this.Json(new ResAcc(oUsuarios));
        }
	}
}