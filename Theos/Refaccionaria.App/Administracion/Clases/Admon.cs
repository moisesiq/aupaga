using System;
using System.Linq;

using LibUtil;
using TheosProc;

namespace Refaccionaria.App
{
    static class Admon
    {
        #region [ Relacionado con Clientes ]

        public static void MostrarEventosClientes(bool bAbiertoManual)
        {
            DateTime dHoy = DateTime.Now.Date;
            DateTime dManiana = dHoy.AddDays(1);
            var oAlertas = Datos.GetListOf<ClientesEventosCalendarioView>(c => c.Fecha >= dHoy && c.Fecha < dManiana && !c.Revisado).OrderBy(c => c.Fecha);

            Eventos.Instance.LimpiarEventos();
            foreach (var oReg in oAlertas)
            {
                /* if (oReg.Fecha < DateTime.Now)
                    AdmonProc.MostrarRecordatorioClientes(oReg.ClienteEventoCalendarioID);
                else
                    Program.oTimers.Add("AlertaPedido" + Program.oTimers.Count.ToString(), new System.Threading.Timer(new TimerCallback(AdmonProc.MostrarRecordatorioClientes)
                        , oReg.ClienteEventoCalendarioID, (int)(oReg.Fecha - DateTime.Now).TotalMilliseconds, Timeout.Infinite));
                */

                Eventos.Instance.AgregarEvento(oReg.ClienteEventoCalendarioID, oReg.Fecha, oReg.Cliente, oReg.Evento);
            }

            // Se muestra el formulario de eventos
            if (oAlertas.Count() > 0)
                Eventos.Instance.Show();
            else if (bAbiertoManual)
                UtilLocal.MensajeAdvertencia("No hay avisos pendientes por notificar.");
        }

        #endregion

        #region [ Relacionado con Proveedores ]

        public static void MostrarRecordatorioPedidos(object state)
        {
            int iEventoID = Util.Entero(state);
            var oEvento = Datos.GetEntity<ProveedorEventoCalendario>(c => c.ProveedorEventoCalendarioID == iEventoID);

            // Se valida que no se haya hecho ya un pedido el día de hoy
            if (Datos.Exists<Pedido>(c => c.ProveedorID == oEvento.ProveedorID && c.PedidoEstatusID != Cat.PedidosEstatus.Cancelado && c.Estatus))
            {
                oEvento.Revisado = true;
                Datos.Guardar<ProveedorEventoCalendario>(oEvento);
                return;
            }

            var oProveedor = Datos.GetEntity<Proveedor>(c => c.ProveedorID == oEvento.ProveedorID && c.Estatus);
            var frmEvento = new RecordatorioEvento(oEvento);

            var oMetodo = new Action(() =>
            {
                frmEvento.ShowDialog(Principal.Instance);
                frmEvento.Dispose();
            });

            if (Principal.Instance.InvokeRequired)
                Principal.Instance.Invoke(oMetodo);
            else
                oMetodo.Invoke();
        }

        public static void MostrarRecordatorioClientes(object state)
        {
            int iEventoID = Util.Entero(state);
            var oEvento = Datos.GetEntity<ClienteEventoCalendario>(c => c.ClienteEventoCalendarioID == iEventoID);
            var oCliente = Datos.GetEntity<Cliente>(c => c.ClienteID == oEvento.ClienteID && c.Estatus);

            var oMetodo = new Action(() =>
            {
                Util.MensajeInformacion(string.Format("{0}\nCliente: {1}\nFecha: {2}", oEvento.Evento, oCliente.Nombre, oEvento.Fecha)
                    , "Recordatorio de Evento");
                // Se marca como revisado
                oEvento.Revisado = true;
                Datos.Guardar<ClienteEventoCalendario>(oEvento);
            });

            if (Principal.Instance.InvokeRequired)
                Principal.Instance.Invoke(oMetodo);
            else
                oMetodo.Invoke();
        }

        #endregion
    }
}
