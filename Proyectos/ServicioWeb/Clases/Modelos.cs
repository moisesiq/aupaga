using System.Runtime.Serialization;

namespace ServicioWeb
{
    [DataContract]
    public class dcUsuario
    {
        [DataMember]
        public int UsuarioID { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string Usuario { get; set; }
    }
}