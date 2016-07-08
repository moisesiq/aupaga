namespace LibUtil
{
    public class EnteroCadena
    {
        public int Entero { get; set; }
        public string Cadena { get; set; }

        public EnteroCadena() { }

        public EnteroCadena(int iEntero, string sCadena)
        {
            this.Entero = iEntero;
            this.Cadena = sCadena;
        }
    }

    public class DosVal<T1, T2>
    {
        public T1 Valor1 { get; set; }
        public T2 Valor2 { get; set; }

        public DosVal() { }

        public DosVal(T1 oVal1, T2 oVal2)
        {
            this.Valor1 = oVal1;
            this.Valor2 = oVal2;
        }
    }
}
