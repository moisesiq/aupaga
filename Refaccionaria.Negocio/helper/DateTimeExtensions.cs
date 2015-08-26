using System;
using System.Text;
using System.Globalization;

namespace Refaccionaria.Negocio
{
    public static class DateTimeExtensions
    {
        public static string ToMonthName(this DateTime dateTime)
        {
            DateTimeFormatInfo c = new CultureInfo("es-MX", false).DateTimeFormat;
            return c.GetMonthName(dateTime.Month);
        }

        public static string ToShortMonthName(this DateTime dateTime)
        {
            DateTimeFormatInfo c = new CultureInfo("es-MX", false).DateTimeFormat;
            return c.GetAbbreviatedMonthName(dateTime.Month);
        }
    }
}
