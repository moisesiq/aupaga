using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Refaccionaria.Negocio;
using GoogleMaps.LocationServices;
// using mshtml;

namespace Refaccionaria.App
{
    public partial class Maps : Form
    {
        bool bEsDireccion;
        string sDireccion;

        public string latlong;
        
       public Maps()
        {
            InitializeComponent();
        }

        public Maps(string address,bool esDireccion)//esDirección true: usa los campos de dirección del cliente, false: usa la latitud y longitud en la bd.
        {
            InitializeComponent();
            this.bEsDireccion = esDireccion;
            this.sDireccion = address;
        }

        private void Maps_Shown(object sender, EventArgs e)
        {
            MapPoint point;
            latlong = "";
            if (this.bEsDireccion)
            {
                var locationservice = new GoogleLocationService();
                point = locationservice.GetLatLongFromAddress(this.sDireccion);
                if (point == null)
                    point = locationservice.GetLatLongFromAddress("Ciudad Guzman");
                latlong = point.Latitude + "," + point.Longitude;
            }
            else
            {
                latlong = this.sDireccion;
            }
            string uri = (@"file:///" + UtilLocal.RutaRecursos("maps.html"));
            wkbMapa.Navigate(uri);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            string res = (string)wkbMapa.Document.InvokeScriptMethod("saveCoords");//regresa las nuevas coordenadas del marcador del mapa
            latlong = res;            
            this.DialogResult = DialogResult.OK;            
            this.Close();
        }
        
        private void wkbMapa_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.latlong == null) return;
            
            string[] param = latlong.Split(',');
            WebKit.DOM.Element elm = wkbMapa.Document.GetElementById("lat");
            elm.SetAttribute("value", param[0]);
            WebKit.DOM.Element elm2 = wkbMapa.Document.GetElementById("lon");
            elm2.SetAttribute("value", param[1]);
            wkbMapa.Document.InvokeScriptMethod("setCoords",new object[]{"1","1"});
            
        }

    }
}
