using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

using TheosProc;
using LibUtil;

namespace ServicioWeb
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class General // : IGeneral
    {
        public General()
        {
            string sModo = System.Configuration.ConfigurationManager.AppSettings["Modo"];
            Datos.CadenaDeConexion = System.Configuration.ConfigurationManager.ConnectionStrings["Theos" + sModo].ConnectionString;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public string Acceso(string sContrasenia)
        {
            var oUsuario = Consultas.ObtenerUsuarioDeContrasenia(sContrasenia);
            return Util.JsonSimple(oUsuario);
            /* if (oUsuario != null)
            {
                return new dcUsuario()
                {
                    UsuarioID = oUsuario.UsuarioID,
                    Usuario = oUsuario.NombreUsuario,
                    Nombre = oUsuario.NombrePersona
                };
            } */
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public string Sucursales()
        {
            var oSucursales = Datos.GetListOf<Sucursal>(c => c.Estatus);
            return Util.ListaJsonSimple(oSucursales);
        }
    }
}
