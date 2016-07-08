using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibUtil
{
    public class ControlError : ErrorProvider
    {
        public const string ErrorPredefinido = "Dato inválido.";
        public const ErrorIconAlignment ErrorAlineacionPredefinida = ErrorIconAlignment.MiddleRight;
        public const int PaddingPredefinido = 0;
        private const int AnchoIcono = 16;

        Dictionary<Control, bool> ControlesAfectadosTamanio = new Dictionary<Control, bool>();

        public ControlError()
        {
            base.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        #region [ Propiedades ]

        private int _NumeroDeErrores;
        public int NumeroDeErrores { get { return this._NumeroDeErrores; } }

        public bool Valido { get { return this.NumeroDeErrores == 0; } }

        #endregion

        #region [ Métodos ]

        private void VerRestaurarTamanio(Control ControlE)
        {
            if (this.ControlesAfectadosTamanio.ContainsKey(ControlE))
            {
                var ErrorAlineacion = base.GetIconAlignment(ControlE);
                if (ErrorAlineacion == ErrorIconAlignment.MiddleRight || ErrorAlineacion == ErrorIconAlignment.TopRight || ErrorAlineacion == ErrorIconAlignment.BottomRight)
                    ControlE.Width += (ControlError.AnchoIcono - ControlError.PaddingPredefinido);

                this.ControlesAfectadosTamanio.Remove(ControlE);
            }
        }

        #endregion

        #region [ Públicos ]

        public void PonerError(Control ControlE)
        {
            this.PonerError(ControlE, ControlError.ErrorPredefinido);
        }

        public void PonerError(Control ControlE, string sError)
        {
            this.PonerError(ControlE, sError, ControlError.ErrorAlineacionPredefinida);
        }

        public void PonerError(Control ControlE, string sError, ErrorIconAlignment AlineacionError)
        {
            base.SetIconAlignment(ControlE, AlineacionError);
            base.SetIconPadding(ControlE, ControlError.PaddingPredefinido);
            base.SetError(ControlE, sError);
            this._NumeroDeErrores++;
        }

        public void PonerError(Control ControlE, string sError, bool bAfectarTamanio)
        {
            // Se cambia el tamaño, si es que no se ha hecho antes
            if (base.GetError(ControlE) == "" && sError != "")
                ControlE.Width -= (ControlError.AnchoIcono - ControlError.PaddingPredefinido);
            
            // Se registra la afectación del tamaño
            this.ControlesAfectadosTamanio[ControlE] = true;

            // Se pone el error
            this.PonerError(ControlE, sError);
        }

        public void QuitarError(Control ControlE)
        {
            if (base.GetError(ControlE) == "")
                return;  // No hay error en el control especificado

            base.SetError(ControlE, "");
            if (this._NumeroDeErrores > 0)
                this._NumeroDeErrores--;

            // Se verifica si se debe actualizar el tamaño
            this.VerRestaurarTamanio(ControlE);
        }

        public bool TieneError(Control ControlE)
        {
            return (base.GetError(ControlE) != "");
        }

        public void LimpiarErrores()
        {
            // Se revisa si se deben ajustar tamaños
            while (this.ControlesAfectadosTamanio.Count > 0)
                this.VerRestaurarTamanio(this.ControlesAfectadosTamanio.Keys.First());

            base.Clear();
            this._NumeroDeErrores = 0;
        }

        #endregion

    }

    public class GeneracionCFDIException : ApplicationException
    { 
        public GeneracionCFDIException() : base() {}
        public GeneracionCFDIException(string s) : base(s) {}
        public GeneracionCFDIException(string s, Exception ex) : base(s, ex) { }
    }
}
