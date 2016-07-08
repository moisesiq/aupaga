
namespace FacturacionElectronica
{
    public class ResAcc<T>
    {
        public bool Exito { get; set; }
        public bool Error { get { return !this.Exito; } }
        public int CodigoDeEstatus { get; set; }
        public string Mensaje { get; set; }
        public T Respuesta { get; set; }

        public ResAcc() { }

        public ResAcc(bool bExito)
        {
            this.Exito = bExito;
        }

        public ResAcc(bool bExito, string sMensaje)
        {
            this.Exito = bExito;
            this.Mensaje = sMensaje;
        }

        public ResAcc(bool bExito, T Respuesta)
        {
            this.Exito = bExito;
            this.Respuesta = Respuesta;
        }


    }
}
