using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

using LibUtil;

namespace TheosProc
{
    public static class UtilTheos
    {
        #region [ General ] 

        public static decimal ObtenerPrecioSinIva(decimal mPrecio, int iDecimales)
        {
            return Math.Round(mPrecio / (1 + (Theos.Iva / 100)), iDecimales);
        }

        public static decimal ObtenerPrecioSinIva(decimal mPrecio)
        {
            return UtilTheos.ObtenerPrecioSinIva(mPrecio, 2);
        }

        public static decimal ObtenerIvaDePrecio(decimal mPrecio, int iDecimales)
        {
            // return Math.Round(mPrecio - UtilLocal.ObtenerPrecioSinIva(mPrecio), 2);
            return (mPrecio - UtilTheos.ObtenerPrecioSinIva(mPrecio, iDecimales));
        }

        public static decimal ObtenerIvaDePrecio(decimal mPrecio)
        {
            return UtilTheos.ObtenerIvaDePrecio(mPrecio, 2);
        }

        public static decimal ObtenerImporteMasIva(decimal mImporte)
        {
            return Math.Round(mImporte * (1 + (Theos.Iva / 100)), 2);
        }

        public static decimal ObtenerIvaDeSubtotal(decimal mImporte)
        {
            return Math.Round(mImporte * (Theos.Iva / 100), 2);
        }

        public static int SemanaSabAVie(DateTime dFecha)
        {
            int iAnio = dFecha.Year;
            int iDia = dFecha.DayOfYear;
            // Se obtiene el día del primer Sábado del año
            int iDiaUno = (6 - (int)(new DateTime(iAnio, 1, 1).DayOfWeek) + 1);
            DateTime dFechaUno = new DateTime(iAnio, 1, iDiaUno);

            int iSemana;
            if (iDia < iDiaUno)
            {  // Es un día antes de la semana uno
                // Se obtiene el primer día del año pasado
                int iDiaUnoPas = (6 - (int)(new DateTime(iAnio - 1, 1, 1).DayOfWeek) + 1);
                DateTime dFechaUnoPas = new DateTime(iAnio - 1, 1, iDiaUnoPas);
                // Se obtiene la semana
                double mDiasDif = (dFecha - dFechaUnoPas).TotalDays;
                if (mDiasDif % 7 == 0) mDiasDif += 0.1;
                iSemana = (int)Math.Ceiling(mDiasDif / 7);
            }
            else
            {
                // Se obtiene la semana
                double mDiasDif = (dFecha - dFechaUno).TotalDays;
                if (mDiasDif % 7 == 0) mDiasDif += 0.1;
                iSemana = (int)Math.Ceiling(mDiasDif / 7);
            }

            return iSemana;
        }

        public static DateTime InicioSemanaSabAVie(int iAnio, int iSemana)
        {
            // Se obtiene el día del primer Sábado del año
            int iDiaUno = (6 - (int)(new DateTime(iAnio, 1, 1).DayOfWeek) + 1);
            DateTime dFechaUno = new DateTime(iAnio, 1, iDiaUno);

            return dFechaUno.AddDays((iSemana - 1) * 7).Date;
        }

        public static DateTime InicioSemanaSabAVie(DateTime dBase)
        {
            DateTime d = (dBase.DayOfWeek == DayOfWeek.Saturday ? dBase : dBase.AddDays((int)dBase.DayOfWeek * -1).AddDays(-1));
            return d.Date;
        }

        public static int ObtenerTrimestre(DateTime dFecha)
        {
            int iAnio = dFecha.Year;
            DateTime dTri2 = new DateTime(iAnio, 3, 1);
            DateTime dTri3 = dTri2.AddMonths(3);
            DateTime dTri4 = dTri3.AddMonths(3);
            if (dFecha < dTri2)
                return 1;
            else if (dFecha < dTri3)
                return 4;
            else if (dFecha < dTri4)
                return 7;
            else
                return 10;
        }

        public static int ObtenerSemestre(DateTime dFecha)
        {
            DateTime dSem2 = new DateTime(dFecha.Year, 6, 1);
            return (dFecha < dSem2 ? 1 : 7);
        }

        public static decimal GastoCalcularImporteDiario(DateTime dFecha, decimal mImporte, int iPeriodicidadMes)
        {
            DateTime dInicioPer = dFecha.DiaPrimero().Date;
            DateTime dFinPer = dInicioPer.AddMonths(iPeriodicidadMes).AddDays(-1);
            decimal mImporteDiario = (mImporte / ((dFinPer - dInicioPer).Days + 1));
            return mImporteDiario;
        }

        public static decimal AplicarRedondeo(decimal cantidad)
        {
            decimal resultado = cantidad;
            try
            {
                if (cantidad >= 0.01M && cantidad <= 1.00M)
                {
                    string[] param = cantidad.ToString().Split('.');
                    if (param.Length == 2)
                    {
                        var arr = param[1].ToArray();
                        if (arr.Length > 0)
                        {
                            int decimo = 0;
                            int centesimo = 0;

                            int.TryParse(arr[0].ToString(), out decimo);
                            int.TryParse(arr[1].ToString(), out centesimo);

                            //Si los centesimos son iguales a 0, que no redondee
                            if (centesimo == 0 && centesimo == 5)
                                return cantidad;

                            if (decimo != 9)
                            {
                                if (centesimo < 5)
                                {
                                    centesimo = 5;
                                }
                                else
                                {
                                    decimo = decimo + 1;
                                    centesimo = 0;
                                }
                                resultado = Util.Decimal(string.Format("{0}.{1}{2}", param[0], decimo, centesimo));
                            }
                            else
                            {
                                resultado = Math.Ceiling(cantidad);
                            }
                        }
                    }
                }
                else if (cantidad >= 1.01M && cantidad <= 30.00M)
                {
                    string[] param = cantidad.ToString().Split('.');
                    if (param.Length == 2)
                    {
                        var arr = param[1].ToArray();
                        if (arr.Length > 0)
                        {
                            int decimo = 0;
                            int centesimo = 0;

                            int.TryParse(arr[0].ToString(), out decimo);
                            int.TryParse(arr[1].ToString(), out centesimo);

                            //Si los centesimos son iguales a 0, que no redondee
                            if (centesimo == 0)
                                return cantidad;

                            if (decimo != 9)
                            {
                                decimo = decimo + 1;
                                centesimo = 0;

                                resultado = Util.Decimal(string.Format("{0}.{1}{2}", param[0], decimo, centesimo));
                            }
                            else
                            {
                                resultado = Math.Ceiling(cantidad);
                            }
                        }
                    }
                }
                ////else if (cantidad >= 20.01M && cantidad <= 100.00M)
                ////{
                ////    string[] param = cantidad.ToString().Split('.');
                ////    if (param.Length == 2)
                ////    {
                ////        var unitario = ConvertirDecimal(param[0].ToString());
                ////        resultado = ConvertirDecimal(string.Format("{0}.00", unitario + 1));
                ////    }
                ////}
                //else if (cantidad >= 100.01M && cantidad <= 200.00M)
                //{
                //    string[] param = cantidad.ToString().Split('.');
                //    if (param.Length == 2)
                //    {
                //        var unitario = ConvertirDecimal(param[0].ToString());
                //        resultado = ConvertirDecimal(string.Format("{0}.00", unitario + 2));
                //    }
                //}
                else if (cantidad >= 30.01M && cantidad <= 100000.00M)
                {
                    string[] param = cantidad.ToString().Split('.');
                    if (param.Length == 2)
                    {
                        var arr = param[1].ToArray();
                        if (arr.Length > 0)
                        {
                            int decimo = 0;
                            int centesimo = 0;

                            int.TryParse(arr[0].ToString(), out decimo);
                            int.TryParse(arr[1].ToString(), out centesimo);

                            //Si los centesimos son iguales a 0, que no redondee
                            if (decimo == 0 && centesimo == 0)
                                return cantidad;
                        }
                        var unitario = Util.Decimal(param[0].ToString());
                        resultado = Util.Decimal(string.Format("{0}.00", unitario + 1));
                    }
                }
                //else if (cantidad >= 2000.01M && cantidad <= 10000.00M)
                //{
                //    string[] param = cantidad.ToString().Split('.');
                //    if (param.Length == 2)
                //    {
                //        var unitario = ConvertirDecimal(param[0].ToString());
                //        resultado = ConvertirDecimal(string.Format("{0}.00", unitario + 1));
                //    }
                //}
            }
            catch (Exception ex)
            {
            }

            return resultado;
        }

        #endregion

        #region [ Encripatar ]

        public static string Encriptar(string cadena)
        {
            string key = "fon";
            byte[] keyArray;
            byte[] arrCifrar = UTF8Encoding.UTF8.GetBytes(cadena);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] ArrayResultado = cTransform.TransformFinalBlock(arrCifrar, 0, arrCifrar.Length);
            tdes.Clear();

            return Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
        }

        public static string Desencriptar(string cadenaEncriptada)
        {
            string key = "fon";
            byte[] keyArray;
            byte[] arrDescifrar = Convert.FromBase64String(cadenaEncriptada);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(arrDescifrar, 0, arrDescifrar.Length);
            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        #endregion
    }
}
