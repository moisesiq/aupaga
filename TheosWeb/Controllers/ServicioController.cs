using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TheosProc;
using LibUtil;

namespace TheosWeb.Controllers
{
    public class ServicioController : BaseController
    {
        public void Probar()
        {
            this.Response.Write("Servicio funcionando correctamente.<br />" + DateTime.Now.ToString() + "<br />:)");
        }

        public JsonResult ValidarUsuario(string eParam1, string eParam2)
        {
            var oUsuario = Consultas.ObtenerUsuarioDeContrasenia(eParam1);
            if (oUsuario == null)
                return this.Json(new ResAcc("Usuario inválido."));
            if (!oUsuario.Activo)
                return this.Json(new ResAcc("El usuario especificado no está activo."));
            // Si se especifica un permiso, se valida
            if (!string.IsNullOrEmpty(eParam2))
            {
                var oRes = Consultas.ValidarPermiso(oUsuario.UsuarioID, eParam2);
                if (oRes.Error)
                    return this.Json(new ResAcc(oRes.Mensaje));
            }
            
            return this.Json(new ResAcc(new { oUsuario.UsuarioID, Usuario = oUsuario.NombreUsuario }));
        }

        public JsonResult Acceso(string eParam1, int eParam2)
        {
            var oUsuario = Consultas.ObtenerUsuarioDeContrasenia(eParam1);
            if (oUsuario == null)
                return this.Json(new ResAcc("Usuario inválido."));
            if (!oUsuario.Activo)
                return this.Json(new ResAcc("El usuario especificado no está activo."));

            // Se configura la sesión
            // this.Session["UsuarioID"] = oUsuario.UsuarioID;
            // this.Session["SucursalID"] = eParam2;
            var oSucursal = Datos.GetEntity<Sucursal>(c => c.SucursalID == eParam2 && c.Estatus);
            string sIdSesion = this.Session.SessionID;
            var oAcceso = new modAcceso()
            {
                UsuarioID = oUsuario.UsuarioID,
                Usuario = oUsuario.NombreUsuario,
                SucursalID = oSucursal.SucursalID,
                Sucursal = oSucursal.NombreSucursal
            };
            if (TheosWeb.oAccesos == null)
                TheosWeb.oAccesos = new Dictionary<string, modAcceso>();
            TheosWeb.oAccesos[sIdSesion] = oAcceso;

            return this.Json(new ResAcc(new { IdSesion = sIdSesion, oAcceso.UsuarioID, oAcceso.Usuario, oAcceso.SucursalID, oAcceso.Sucursal }));
        }

        public JsonResult DatosDeAcceso()
        {
            if (Theos.UsuarioID <= 0 || Theos.SucursalID <= 0)
                return this.Json(new ResAcc("Sesión inválida."));

            var oAcceso = TheosWeb.oAccesos[this.IdSesion];

            return this.Json(new ResAcc(oAcceso));
        }
                
        public JsonResult CerrarSesion()
        {
            Theos.UsuarioID = 0;
            Theos.SucursalID = 0;
            this.Session.Clear();
            TheosWeb.oAccesos.Remove(this.IdSesion);
            return this.Json(new ResAcc());
        }

        public JsonResult Sucursales()
        {
            var oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            return this.Json(from c in oSucursales
                             select new { c.SucursalID, Sucursal = c.NombreSucursal });
        }

        public JsonResult Sucursal(int id)
        {
            var oSucursal = Datos.GetEntity<Sucursal>(c => c.SucursalID == id && c.Estatus);
            return this.Json(new { oSucursal.SucursalID, Sucursal = oSucursal.NombreSucursal });
        }

        public JsonResult Usuario(int id)
        {
            var oUsuario = Datos.GetEntity<Usuario>(c => c.UsuarioID == id && c.Estatus);
            return this.Json(new { oUsuario.UsuarioID, Usuario = oUsuario.NombreUsuario });
        }

        public JsonResult Error(string id)
        {
            return this.Json(new ResAcc(id));
        }
	}
}