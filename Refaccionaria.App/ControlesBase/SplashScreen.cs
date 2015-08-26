using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Reflection;

namespace Refaccionaria.App
{
    public class SplashScreen
    {
        private static BackgroundWorker worker;
        private static ISplashForm displayForm;
        private delegate void UpdateText(string text);
        private delegate void UpdateInt(int number);
        private const int WAIT_TIME = 2000;
        private static ManualResetEvent windowCreated;

        public static void Show(ISplashForm DisplayForm)
        {
            if (!EnsureWorker() && worker.IsBusy)
                return;

            if (!(DisplayForm is Form))
                throw new ArgumentException("Debe ser un windows form", "Display");

            displayForm = DisplayForm;
            windowCreated = new ManualResetEvent(false);
            ((Form)displayForm).HandleCreated += SplashScreenController_HandleCreated;
            
            worker.RunWorkerAsync(displayForm);
            if (windowCreated.WaitOne(WAIT_TIME))
            {
                ((Form)displayForm).HandleCreated -= SplashScreenController_HandleCreated;
            }
            else
            {
                //throw new ApplicationException("Tiempo de espera agotado.");
            }
        }

        static void SplashScreenController_HandleCreated(object sender, EventArgs e)
        {
            windowCreated.Set();
        }

        public static void Close()
        {
            if (displayForm != null && ((Form)displayForm).IsHandleCreated && worker != null)
                displayForm.Cerrar();                
        }

        private static bool EnsureWorker()
        {
            try
            {
                if (worker == null)
                {
                    worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = false;
                    worker.WorkerSupportsCancellation = true;
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                }
            }
            catch { }
            return true;
        }

        private static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "SplashScreen";         
            Application.Run((Form)displayForm);
        }

        public static void UpdateStatus(string status)
        {
            if (displayForm != null && ((Form)displayForm).IsHandleCreated)
                displayForm.Invoke(new UpdateText(displayForm.UpdateStatus), new object[] { status });
        }

        public static void UpdateProgress(int progress)
        {
            if (displayForm != null && ((Form)displayForm).IsHandleCreated)
                displayForm.Invoke(new UpdateInt(displayForm.UpdateProgress), new object[] { progress });
        }

        public static void UpdateInfo(string info)
        {
            if (displayForm != null && ((Form)displayForm).IsHandleCreated)
                displayForm.Invoke(new UpdateText(displayForm.UpdateInfo), new object[] { info });
        }

    }

}