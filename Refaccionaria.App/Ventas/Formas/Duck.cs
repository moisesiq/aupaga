using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Refaccionaria.Negocio
{
    public partial class Duck : Form
    {
        public Duck(string url)
        {
            InitializeComponent();
            this.wmpDuck.uiMode = "none";

            this.wmpDuck.URL = url;
        }

        private void wmpDuck_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //Helper.MensajeInformacion("wmpDuck_PlayStateChange " + e.newState, "");
            switch (e.newState)
            {
                case 8:    // MediaEnded
                    this.Close();
                    break;
            }
        }
    }
}
