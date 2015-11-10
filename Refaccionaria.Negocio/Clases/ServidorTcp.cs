using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace Refaccionaria.Negocio
{
    public class ServidorTcp
    {
        bool bCerrar;

        public ServidorTcp(int iPuerto)
        {
            this.Puerto = iPuerto;
        }

        #region [ Propiedades ]

        public event Action<Socket, string> ConexionRecibida;

        public TcpListener Escucha { get; set; }
        public int Puerto { get; set; }

        public List<string> Errores { get; private set; }

        #endregion
        
        #region [ Públicos ]

        public void Escuchar()
        {
            // Se cierra la conexión previa, si hubiera
            if (this.Escucha != null)
                this.Detener();
            
            //
            this.Escucha = new TcpListener(IPAddress.Any, this.Puerto);
            var oHiloEscucha = new Thread(this.ProcEscuchar);
            oHiloEscucha.Start();
        }

        public void Detener()
        {
            this.bCerrar = true;
            string sIpLocal = Helper.IpLocal();
            var oCliente = new TcpClient(sIpLocal, this.Puerto);
            oCliente.Close();
            // Se mete un retraso para que se alcance a salir del ciclo y detener la "escucha", para que las próximas conexiones no marquen error
            Thread.Sleep(100);
            // this.Escucha.Stop();
        }

        #endregion

        #region [ Privados ]

        private void ProcEscuchar()
        {
            try
            {
                this.Escucha.Start();
            }
            catch (Exception oEx)
            {
                this.AgregarError(oEx);
                return;
            }
            
            while (!this.bCerrar)
            {
                var oSocket = this.Escucha.AcceptSocket();
                // Se usa el mensaje
                var oBytes = new byte[oSocket.ReceiveBufferSize];
                try
                {
                    int iBytes = oSocket.Receive(oBytes);
                    string sMensaje = UTF8Encoding.UTF8.GetString(oBytes, 0, iBytes);

                    if (this.ConexionRecibida != null)
                        this.ConexionRecibida(oSocket, sMensaje);
                }
                catch (Exception oEx)
                {
                    this.AgregarError(oEx);
                }
            }
            this.Escucha.Stop();
        }

        private void AgregarError(Exception oEx)
        {
            if (this.Errores == null)
                this.Errores = new List<string>();
            this.Errores.Add(oEx.Message);
        }

        #endregion
    }
}
