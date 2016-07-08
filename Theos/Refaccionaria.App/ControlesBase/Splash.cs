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
    public partial class Splash : Form, ISplashForm
    {
        public Splash()
        {
            InitializeComponent();
        }

        public void UpdateStatus(string status)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(int progress)
        {
            //this.progressBar1.Value = progress;
        }

        public void UpdateInfo(string info)
        {
            throw new NotImplementedException();
        }

        public void Cerrar()
        {
            if (!this.IsHandleCreated) return;
            if (this.InvokeRequired)
                this.Invoke(new Action(this.Close));
            else
                this.Close();
        }
    }
}
