using System;
using System.Web;
using System.Web.Mvc;

using LibUtil;
using TheosProc;

namespace TheosWeb
{
    public class BaseController : Controller
    {
        public string IdSesion { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Theos.UsuarioID = Theos.SucursalID = 0;

            var oToken = this.HttpContext.Request.Headers["IdSesion"];
            if (oToken != null)
            {
                // Se valida que exista el objeto de las sesiones
                if (TheosWeb.oAccesos == null)
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new ResAcc("Sesión inválida o caducada.", Cat.CodigosRes.SesionCaducada)
                    };
                    return;
                }

                string sToken = Util.Cadena(oToken);
                if (TheosWeb.oAccesos.ContainsKey(sToken))
                {
                    this.IdSesion = sToken;
                    var oAcceso = TheosWeb.oAccesos[sToken];
                    Theos.UsuarioID = oAcceso.UsuarioID;
                    Theos.SucursalID = oAcceso.SucursalID;
                }
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            // Exception ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;
            // var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");
            
            filterContext.Result = new JsonResult()
            {
                Data = new ResAcc(string.Format("{0}\n{1}", filterContext.Exception.Message, filterContext.Exception.InnerException.Message))
            };
        }
    }
}