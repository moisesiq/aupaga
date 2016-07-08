namespace LibUtil
{
    public class Con
    {
        public class Formatos
        {
            public const string Entero = "###,###,###,##0";
            public const string Decimal = "###,###,###,##0.00";
            public const string Moneda = "$###,###,###,##0.00";
            public const string Porcentaje = "##0.00%";
            public const string Fecha = "dd/MM/yyyy";
            public const string FechaHora = "dd/MM/yyyy HH:mm:ss";
        }

        public class Afectaciones
        {
            public const int NoEspecificado = 0;
            public const int SinCambios = 1;
            public const int Agregar = 2;
            public const int Modificar = 3;
            public const int Borrar = 4;
        }
    }
}
