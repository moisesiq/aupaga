using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Refaccionaria.App
{
    public partial class Log : Form
    {
        public bool finalizo;
        
        class Nested
        {
            static Nested() { }
            internal static readonly Log instance = new Log();
        }

        public Log()
        {
            InitializeComponent();
            finalizo = false;
        }

        public void AppendTextBox(string value)
        {
            if (IsHandleCreated)
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                    return;
                }
                this.txtLog.AppendText(string.Format("{0}{1}", value, Environment.NewLine));
                Application.DoEvents();
            }
        }

        private void Log_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!finalizo)
                e.Cancel = true;
        }
    }
}
