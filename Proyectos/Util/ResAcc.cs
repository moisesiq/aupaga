using System;
using System.Collections.Generic;

namespace LibUtil
{
    public class ResAcc<T>
    {
        #region [ Constructores ]

        public ResAcc() { this.Exito = true; }
        public ResAcc(bool bExito) { this.Exito = bExito; }
        public ResAcc(string sMensaje) { this.Propiedades(false, sMensaje); }
        public ResAcc(T oRespuesta) { this.Propiedades(true, oRespuesta); }
        public ResAcc(bool bExito, string sMensaje) { this.Propiedades(bExito, sMensaje); }
        public ResAcc(bool bExito, T oRespuesta) { this.Propiedades(bExito, oRespuesta); }
        public ResAcc(string sMensaje, int iCodigo) { this.Propiedades(sMensaje, iCodigo); }
        
        #endregion

        #region [ Propiedades ]

        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public int Codigo { get; set; }
        public T Respuesta { get; set; }

        public bool Error
        {
            get { return !this.Exito; }
        }

        #endregion

        #region [ Métodos ]

        protected void Propiedades(bool bExito, string sMensaje)
        {
            this.Exito = bExito;
            this.Mensaje = sMensaje;
        }

        protected void Propiedades(bool bExito, T oRespuesta)
        {
            this.Exito = bExito;
            this.Respuesta = oRespuesta;
        }

        protected void Propiedades(string sMensaje, int iCodigo)
        {
            this.Exito = false;
            this.Mensaje = sMensaje;
            this.Codigo = iCodigo;
        }

        #endregion
    }

    public class ResAcc : ResAcc<object>
    {
        #region [ Constructores ]

        public ResAcc() { this.Exito = true; }
        public ResAcc(bool bExito) { this.Exito = bExito; }
        public ResAcc(string sMensaje) { this.Propiedades(false, sMensaje); }
        public ResAcc(object oRespuesta) { this.Propiedades(true, oRespuesta); }
        public ResAcc(bool bExito, string sMensaje) { this.Propiedades(bExito, sMensaje); }
        public ResAcc(bool bExito, object oRespuesta) { this.Propiedades(bExito, oRespuesta); }
        public ResAcc(string sMensaje, int iCodigo) { this.Propiedades(sMensaje, iCodigo); }

        #endregion
    }
}
