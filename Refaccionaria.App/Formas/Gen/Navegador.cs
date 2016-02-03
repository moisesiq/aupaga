using System;
using System.Windows.Forms;

namespace Refaccionaria.App
{
    public partial class Navegador : Form
    {
        public Navegador()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Ico_ControlRefaccionaria;
            this.webPagina.Navigating += webPagina_Navigating;
            this.webPagina.DocumentCompleted += webPagina_DocumentCompleted;
        }

        void webPagina_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            const string sUrlInicio = "https://cfdiau.sat.gob.mx/nidp/app/login?id=SATUPCFDiCon&sid=0&option=credential&sid=0";
            const string sUrlMed = "https://cfdiau.sat.gob.mx/nidp/app?sid=0";
            const string sUrlEmitidas = "https://portalcfdi.facturaelectronica.sat.gob.mx/ConsultaEmisor.aspx";
            const string sUrlVacio = "about:blank";

            string sUrl = e.Url.ToString();
            switch (sUrl)
            {
                case sUrlInicio:
                    var oJs = this.webPagina.Document.CreateElement("script");
                    oJs.SetAttribute("type", "text/javascript");
                    string sJs = System.IO.File.ReadAllText(@"\\VBOXSVR\Garibaldi\Theos\FacturacionElectronica\ConSat.js");
                    sJs += "\n\nwindow.IniciarSesion('GOPM880202L26', 'Castx092');";
                    oJs.SetAttribute("text", sJs);
                    this.webPagina.Document.GetElementsByTagName("head")[0].AppendChild(oJs);
                    break;
                case sUrlMed:
                    this.webPagina.Navigate(sUrlEmitidas);
                    break;
                case sUrlEmitidas:
                    var oJs2 = this.webPagina.Document.CreateElement("script");
                    oJs2.SetAttribute("type", "text/javascript");
                    string sJs2 = System.IO.File.ReadAllText(@"\\VBOXSVR\Garibaldi\Theos\FacturacionElectronica\ConSat.js");
                    sJs2 += "\n\nwindow.ObtenerEmitidasPorFecha('01/12/2015', '31/12/2015');";
                    oJs2.SetAttribute("text", sJs2);
                    this.webPagina.Document.GetElementsByTagName("head")[0].AppendChild(oJs2);
                    break;
                case sUrlVacio:
                    break;
            }

            var x = 3;

        }

        void webPagina_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            const string sUrlVacio = "about:blank";
            if (e.Url.ToString() == sUrlVacio)
            {
                string sXmls = this.webPagina.Document.GetElementById("preXmls").InnerText;
                var aXmls = sXmls.Split('\n');
                UtilLocal.MensajeInformacion(sXmls);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.webPagina.Navigate("https://cfdiau.sat.gob.mx/nidp/app/login?id=SATUPCFDiCon&sid=0&option=credential&sid=0");
            
        }
    }
}
