using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

using TheosProc;

namespace ServicioWeb
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    public class Traspasos // : ITraspasos
    {
        public Traspasos()
        {
            string sModo = System.Configuration.ConfigurationManager.AppSettings["Modo"];
            Datos.CadenaDeConexion = System.Configuration.ConfigurationManager.ConnectionStrings["Theos" + sModo].ConnectionString;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public string ConexionCad(string sCadena)
        {
            return ("from WebService: " + sCadena);
        }
        
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat=WebMessageFormat.Json)]
        public string Traspaso(int iTraspasoID)
        {
            var o = Datos.GetEntity<MovimientoInventarioTraspasosView>(c => c.MovimientoInventarioID == iTraspasoID);
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(o);
        }
    }
}
