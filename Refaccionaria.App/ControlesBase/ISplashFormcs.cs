using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Refaccionaria.App
{
    public interface ISplashForm : ISynchronizeInvoke
    {
        void UpdateStatus(string status);
        void UpdateProgress(int progress);
        void UpdateInfo(string info);
        void Cerrar();
    }
}
