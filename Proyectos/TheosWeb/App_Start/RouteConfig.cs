using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TheosWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(name: "2Params", url: "{controller}/{action}/{eParam1}/{eParam2}"
                , defaults: new { controller = "Servicio", action = "Index", eParam1 = UrlParameter.Optional, eParam2 = UrlParameter.Optional });

            routes.MapRoute(name: "3Params", url: "{controller}/{action}/{eParam1}/{eParam2}/{eParam3}"
                , defaults: new { controller = "Servicio", action = "Index", eParam1 = UrlParameter.Optional, eParam2 = UrlParameter.Optional, eParam3 = UrlParameter.Optional });

            routes.MapRoute(name: "4Params", url: "{controller}/{action}/{eParam1}/{eParam2}/{eParam3}/{eParam4}"
                , defaults: new { controller = "Servicio", action = "Index", eParam1 = UrlParameter.Optional, eParam2 = UrlParameter.Optional
                    , eParam3 = UrlParameter.Optional, eParam4 = UrlParameter.Optional });
        }
    }
}
