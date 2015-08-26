using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using LibUtil;
using TheosProc;

namespace TheosWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Se inicializa conexión a los datos
            string sModo = System.Configuration.ConfigurationManager.AppSettings["Modo"];
            Datos.CadenaDeConexion = System.Configuration.ConfigurationManager.ConnectionStrings["Theos" + sModo].ConnectionString;
            // Parámetros globales
            Theos.Iva = Util.Decimal(Config.Valor("Iva"));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            /* Exception oError = this.Server.GetLastError();
            this.Server.ClearError();
            string sError = string.Format("<pre>{0}\n{1}\n{2}</pre>", "Error de aplicación", oError.Message, oError.InnerException.Message);
            this.Response.Redirect("/Servicio/Error/" + sError);
            */
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Se inicializan datos globales
            /*
            if (this.Session["UsuarioID"] != null)
                Theos.UsuarioID = Util.Entero(this.Session["UsuarioID"]);
            if (this.Session["SucursalID"] != null)
                Theos.SucursalID = Util.Entero(this.Session["SucursalID"]);
            */
        }
    }
}
