using System;

using Refaccionaria.Negocio;

namespace Refaccionaria.Modelo
{
    public class PreciosParte
    {
        public int ParteID { get; set; }
        public decimal[] Precios { get; set; }

        public PreciosParte() { }
             
        public PreciosParte(int iParteID)
        {
            var oPrecios = General.GetEntity<PartePrecio>(q => q.ParteID == iParteID && q.Estatus);
            this.ParteID = oPrecios.ParteID;
            this.Precios = new decimal[] {
                oPrecios.PrecioUno.Valor(),
                oPrecios.PrecioDos.Valor(),
                oPrecios.PrecioTres.Valor(),
                oPrecios.PrecioCuatro.Valor(),
                oPrecios.PrecioCinco.Valor()
            };
        }

        public decimal ObtenerPrecio(int iNumeroDePrecio)
        {
            return this.Precios[iNumeroDePrecio - 1];
        }
    }
}
