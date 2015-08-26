using System.Windows.Forms;

namespace Refaccionaria.Negocio
{
    public class Validador
    {
        public ControlError ControlE = new ControlError();
        public int Errores { get { return this.ControlE.NumeroDeErrores; } }
        public bool Valido { get; set; }

        public enum Tipos { Texto, Entero, Decimal, NoNulo }

        public void LimpiarErrores()
        {
            this.ControlE.LimpiarErrores();
        }

        public bool Validar(Control oControl, Tipos TipoDeValidacion)
        {
            string sPropiedad = "";
            Tipos TipoVal = Tipos.Texto;

            string sControl = oControl.GetType().Name;
            if (sControl == "TextBox" || sControl == "TextoMod")
            {
                sPropiedad = "Text";
                TipoVal = Tipos.Texto;
            }
            else if (sControl == "ComboBox" || sControl == "ComboEtiqueta")
            {
                sPropiedad = "SelectedValue";
                TipoVal = Tipos.NoNulo;
            }

            if (sPropiedad == "")
            {
                this.ControlE.PonerError(oControl);
                return false;
            }

            return this.Validar(oControl, sPropiedad, TipoVal);
        }

        public bool Validar(Control oControl, string sPropiedad)
        {
            return this.Validar(oControl, sPropiedad, Tipos.Texto);
        }

        public bool Validar(Control oControl, string sPropiedad, Tipos TipoDeValidacion)
        {
            return this.Validar(oControl, sPropiedad, TipoDeValidacion, ControlError.ErrorPredefinido);
        }

        public bool Validar(Control oControl, string sPropiedad, Tipos TipoDeValidacion, string sMensajeDeError)
        {
            return this.Validar(oControl, sPropiedad, TipoDeValidacion, sMensajeDeError);
        }

        public bool Validar(Control oControl, string sPropiedad, Tipos TipoDeValidacion, string sMensajeDeError, ErrorIconAlignment AlineacionError)
        {
            object AValidar = oControl.GetType().GetProperty(sPropiedad).GetValue(oControl, null);
            if (AValidar == null) return false;

            this.Valido = true;
            switch (TipoDeValidacion)
            {
                case Tipos.Texto:
                    if (Helper.ConvertirCadena(AValidar) == "")
                        this.Valido = false;
                    break;
                case Tipos.Entero:
                    if (!Helper.ValidarEntero(Helper.ConvertirCadena(AValidar)))
                        this.Valido = false;
                    break;
                case Tipos.Decimal:
                    if (!Helper.ValidarDecimal(Helper.ConvertirCadena(AValidar)))
                        this.Valido = false;
                    break;
                case Tipos.NoNulo:
                    if (AValidar == null)
                        this.Valido = false;
                    break;
                default:
                    this.Valido = false;
                    break;
            }

            // Se pone o quita el error
            if (this.Valido)
                this.ControlE.QuitarError(oControl);
            else
                this.ControlE.PonerError(oControl, sMensajeDeError, AlineacionError);

            return this.Valido;
        }
    }
}
