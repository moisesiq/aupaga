using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Refaccionaria.App
{
    public partial class BusquedaVIN : Form
    {

        public string VIN;//VIN de prueba: WDBUF87XX9B374875
        int loadedTimes;
        public BusquedaVIN()
        {
            InitializeComponent();
        }

        public BusquedaVIN(string iVIN)
        {
            
            InitializeComponent();            
            VIN = iVIN != null?iVIN:"";

            wkbVIN.Navigate(@"http://www.google.com");
            loadedTimes = 0;
            
        }

        private void wkbVIN_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebKit.DOM.Node header = wkbVIN.Document.GetElementsByTagName("head")[0];
            var script = wkbVIN.Document.CreateElement("script");
            switch (loadedTimes)
            {
                case 0:
                case 2:
                    script.TextContent = @"function changeUrl(){ document.location.href = 'http://mfr.epicor.com/alliance/servlet?command=vin'; return true; }";
                    header.AppendChild(script);
                    var res2 = wkbVIN.Document.InvokeScriptMethod("changeUrl");
                    break;
                case 3:
                    script.TextContent = @"function rellenarVIN(){ document.vsForm.vinNumber.value = '" + VIN + "'; document.vsForm.submit();  return true; }";
                    header.AppendChild(script);
                    var res3 = wkbVIN.Document.InvokeScriptMethod("rellenarVIN");
                    break;
            }
            loadedTimes++;
        }

    }
}
