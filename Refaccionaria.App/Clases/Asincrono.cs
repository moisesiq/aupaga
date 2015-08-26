using System;
using System.Collections.Generic;
using System.Threading;

using Refaccionaria.Negocio;

namespace Refaccionaria.App
{
    static class Asincrono
    {
        static List<Timer> oTemporizadores = new List<Timer>();

        #region [ Estáticos para agregar procesos asíncronos ]

        public static int RecordatorioMensaje(string sTitulo, string sMensaje, int iHora, int iMinuto)
        {
            // Se determina el tiempo para llegar a la hora
            DateTime dAhora = DateTime.Now;
            DateTime dHora = new DateTime(dAhora.Year, dAhora.Month, dAhora.Day, iHora, iMinuto, 0);
            if (dHora < dAhora)
                dHora = dHora.AddDays(1);
            int iMiliseg = (int)(dHora - dAhora).TotalMilliseconds;

            // Se agrega el timer
            /* Asincrono.oTemporizadores.Add(new Timer(o => {
                var oMensaje = (o as DosVal<string, string>);
                Principal.Instance.MostrarMensaje(oMensaje.Valor1, oMensaje.Valor2);
            }, new DosVal<string, string>(sTitulo, sMensaje), iMiliseg, Timeout.Infinite));
            */

            return (Asincrono.oTemporizadores.Count - 1);
        }

        public static int ProcesoHora(TimerCallback oMetodo, object oParam, int iHora, int iMinuto)
        {
            // Se determina el tiempo para llegar a la hora
            DateTime dAhora = DateTime.Now;
            DateTime dHora = new DateTime(dAhora.Year, dAhora.Month, dAhora.Day, iHora, iMinuto, 0);
            if (dHora < dAhora)
                dHora = dHora.AddDays(1);
            int iMiliseg = (int)(dHora - dAhora).TotalMilliseconds;

            // Se agrega el timer
            Asincrono.oTemporizadores.Add(new Timer(oMetodo, oParam, iMiliseg, Timeout.Infinite));

            return (Asincrono.oTemporizadores.Count - 1);
        }

        #endregion

        #region [ Privados auxiliares ]

        

        #endregion
    }
}
