using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Refaccionaria.App
{
    public class UpdateConfig
    {
        private string _AvailableVersion;
        public string AvailableVersion
        { get { return _AvailableVersion; } set { _AvailableVersion = value; } }

        private string _AppFileURL;
        public string AppFileURL
        { get { return _AppFileURL; } set { _AppFileURL = value; } }

        private string _LatestChanges;
        public string LatestChanges
        { get { return _LatestChanges; } set { _LatestChanges = value; } }

        private string _ChangeLogURL;
        public string ChangeLogURL
        { get { return _ChangeLogURL; } set { _ChangeLogURL = value; } }

        private string _MsjError;
        public string MsjError
        { get { return _MsjError; } set { _MsjError = value; } }
                
        public bool LoadConfig(string url, string user, string pass, string proxyURL, bool proxyEnabled)
        {
            try
            {
                //Load the xml config file
                XmlDocument XmlDoc = new XmlDocument();
                HttpWebResponse Response;
                HttpWebRequest Request;
                //Retrieve the File

                Request = (HttpWebRequest)HttpWebRequest.Create(url);
                //Request.Headers.Add("Translate: f"); //Commented out 11/16/2004 Matt Palmerlee, this Header is more for DAV and causes a known security issue
                if (user != null && user != "")
                    Request.Credentials = new NetworkCredential(user, pass);
                else
                    Request.Credentials = CredentialCache.DefaultCredentials;

                //Added 11/16/2004 For Proxy Clients, Thanks George for submitting these changes
                if (proxyEnabled == true)
                    Request.Proxy = new WebProxy(proxyURL, true);

                Response = (HttpWebResponse)Request.GetResponse();

                Stream respStream = null;
                respStream = Response.GetResponseStream();

                //Load the XML from the stream
                XmlDoc.Load(respStream);

                //Parse out the AvailableVersion
                XmlNode AvailableVersionNode = XmlDoc.SelectSingleNode(@"//AvailableVersion");
                this.AvailableVersion = AvailableVersionNode.InnerText;

                //Parse out the AppFileURL
                XmlNode AppFileURLNode = XmlDoc.SelectSingleNode(@"//AppFileURL");
                this.AppFileURL = AppFileURLNode.InnerText;

                //Parse out the LatestChanges
                XmlNode LatestChangesNode = XmlDoc.SelectSingleNode(@"//LatestChanges");
                if (LatestChangesNode != null)
                    this.LatestChanges = LatestChangesNode.InnerText;
                else
                    this.LatestChanges = "";

                //Parse out the ChangLogURL
                XmlNode ChangeLogURLNode = XmlDoc.SelectSingleNode(@"//ChangeLogURL");
                if (ChangeLogURLNode != null)
                    this.ChangeLogURL = ChangeLogURLNode.InnerText;
                else
                    this.ChangeLogURL = "";
            }
            catch (Exception ex)
            {                
                this._MsjError = "Error al tratar de leeer el archivo de configuración: " + url + "\r\nAsegurate de que el archivo de configuracion tenga un formato valido." + "\r\n" + ex.Message;
                return false;
            }
            return true;
        }//LoadConfig(string url, string user, string pass)

    }
}
