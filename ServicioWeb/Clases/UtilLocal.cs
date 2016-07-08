using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace ServicioWeb
{
    public static class UtilLocal
    {
        public static string Json(object oObjeto)
        {
            return new JavaScriptSerializer().Serialize(oObjeto);
        }
    }
}