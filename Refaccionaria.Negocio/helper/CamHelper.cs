﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace Refaccionaria.Negocio
{
    /// <see cref="http://www.pinvoke.net/default.aspx/Constants.WM"/>
    public class Constants
    {
        public const uint WM_CAP = 0x400;
        public const uint WM_CAP_DRIVER_CONNECT = 0x40a;
        public const uint WM_CAP_DRIVER_DISCONNECT = 0x40b;
        public const uint WM_CAP_EDIT_COPY = 0x41e;
        public const uint WM_CAP_SET_PREVIEW = 0x432;
        public const uint WM_CAP_SET_OVERLAY = 0x433;
        public const uint WM_CAP_SET_PREVIEWRATE = 0x434;
        public const uint WM_CAP_SET_SCALE = 0x435;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const int WM_USER = 1024;
        public const int WM_CAP_START = WM_USER;
        public const int WM_CAP_DLG_VIDEOFORMAT = (WM_CAP_START + 41);
        public const int WM_CAP_DLG_VIDEOSOURCE = (WM_CAP_START + 42);

    }

    /// <see cref="http://windowssdk.msdn.microsoft.com/en-us/library/ms713477(VS.80).aspx"/>
    public class Avicap32
    {
        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_capgetdriverdescription.asp"/>
        [DllImport("avicap32.dll")]
        public extern static IntPtr capGetDriverDescription(
            ushort index,
            StringBuilder name,
            int nameCapacity,
            StringBuilder description,
            int descriptionCapacity
        );

        /// <see cref="http://msdn.microsoft.com/library/en-us/multimed/htm/_win32_capcreatecapturewindow.asp?frame=true"/>
        [DllImport("avicap32.dll")]
        public extern static IntPtr capCreateCaptureWindow(
            string title,
            uint style,
            int x,
            int y,
            int width,
            int height,
            IntPtr window,
            int id
        );
    }

    /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/messagesandmessagequeues.asp"/>
    public class User32
    {
        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/messagesandmessagequeues/messagesandmessagequeuesreference/messagesandmessagequeuesfunctions/sendmessage.asp"/>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam
        );

        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowfunctions/setwindowpos.asp"/>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags
        );

        /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowfunctions/destroywindow.asp"/>
        [DllImport("user32")]
        public static extern IntPtr DestroyWindow(
            IntPtr hWnd
        );
    }
}

namespace Refaccionaria.Negocio
{
    public class CamHelper : IDisposable
    {
        private static int MAX_DEVICES = 10;

        private ushort deviceNumber;
        private string name;
        private string description;
        private IntPtr deviceHandle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceNumber">the number</param>
        /// <param name="name">the name</param>
        /// <param name="description">the description</param>
        private CamHelper(ushort deviceNumber, string name, string description)
        {
            this.deviceNumber = deviceNumber;
            this.name = name;
            this.description = description;
        }

        //CamHelper() { Detach(); }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Setter and Getter for the Device Number
        /// </summary>
        public ushort DeviceNumber
        {
            get { return deviceNumber; }
            set { deviceNumber = value; }
        }

        /// <summary>
        /// Setter and Getter for the Device name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Setter and Getter for the Device description
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Attaches the preview stream to the given control
        /// </summary>
        /// <param name="control">the control</param>
        public void Attach(System.Windows.Forms.PictureBox control)
        {
            deviceHandle = Avicap32.capCreateCaptureWindow("", Constants.WS_VISIBLE | Constants.WS_CHILD, 0, 0, control.Width, control.Height, control.Handle, 0);

            if (User32.SendMessage(deviceHandle, Constants.WM_CAP_DRIVER_CONNECT, (IntPtr)deviceNumber, (IntPtr)0).ToInt32() > 0)
            {
                User32.SendMessage(deviceHandle, Constants.WM_CAP_SET_SCALE, (IntPtr)(-1), (IntPtr)0);
                User32.SendMessage(deviceHandle, Constants.WM_CAP_SET_PREVIEWRATE, (IntPtr)0x42, (IntPtr)0);
                User32.SendMessage(deviceHandle, Constants.WM_CAP_SET_PREVIEW, (IntPtr)(-1), (IntPtr)0);
                User32.SetWindowPos(deviceHandle, new IntPtr(0), 0, 0, control.Width, control.Height, 6);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            // Destroy preview window
            Detach();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Detaches from the control
        /// </summary>
        public void Detach()
        {
            if (deviceHandle.ToInt32() != 0)
            {
                User32.SendMessage(deviceHandle, Constants.WM_CAP_DRIVER_DISCONNECT, (IntPtr)deviceNumber, (IntPtr)0);
                User32.DestroyWindow(deviceHandle);
            }
            deviceHandle = new IntPtr(0);
        }

        /// <summary>
        /// Returns a captured image
        /// </summary>
        /// <returns>an image, null if capture failed</returns>
        public Bitmap Capture()
        {
            if (deviceHandle.ToInt32() != 0)
            {
                User32.SendMessage(deviceHandle, Constants.WM_CAP_EDIT_COPY, (IntPtr)0, (IntPtr)0);

                IDataObject ido = Clipboard.GetDataObject();
                if (ido.GetDataPresent(DataFormats.Bitmap))
                {
                    Bitmap bmp = ((Bitmap)ido.GetData(DataFormats.Bitmap));
                    return ((Bitmap)ido.GetData(DataFormats.Bitmap));
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowVideoFormatDialog()
        {            
            User32.SendMessage(deviceHandle, Constants.WM_CAP_DLG_VIDEOFORMAT, (IntPtr)deviceNumber, (IntPtr)0);                     
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowVideoSourceDialog()
        {
            User32.SendMessage(deviceHandle, Constants.WM_CAP_DLG_VIDEOSOURCE, (IntPtr)deviceNumber, (IntPtr)0);            
        }

        /// <summary>
        /// Returns an array with available capture devices
        /// </summary>
        /// <returns>the device names</returns>
        public static List<CamHelper> GetDevices()
        {
            List<CamHelper> devices = new List<CamHelper>();

            for (ushort i = 0; i < MAX_DEVICES; ++i)
            {
                int capacity = 200;
                StringBuilder name = new StringBuilder(capacity);
                StringBuilder description = new StringBuilder(capacity);

                if (Avicap32.capGetDriverDescription(i, name, capacity, description, capacity).ToInt32() > 0)
                {
                    devices.Add(new CamHelper(i, name.ToString(), description.ToString()));
                }
            }

            return devices;
        }
        
    }
}