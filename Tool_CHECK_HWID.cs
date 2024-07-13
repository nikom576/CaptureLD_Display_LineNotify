using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOT_CBM_DATAGRID.Tools_Class
{
    internal static class Tool_CHECK_HWID
    {

        public static IntPtr GetWindowHandleByTitle(string windowTitle)
        {
            IntPtr desktopHandle = IntPtr.Zero;
            IntPtr windowHandle = IntPtr.Zero;

            windowHandle = FindWindow(null, windowTitle);
            return windowHandle;
        } // GetWindowHandleByTitle
        
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

       

    }
}
