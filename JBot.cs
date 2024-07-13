using Line_Notify;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BOT_CBM_DATAGRID.Tools_Class
{
    internal static class JBot
    {
        public static List<string> FindWindowsByClass(string className)
        {
            List<string> titles = new List<string>();

            IntPtr desktopHandle = IntPtr.Zero;
            IntPtr windowHandle = IntPtr.Zero;

            for (windowHandle = FindWindowEx(desktopHandle, IntPtr.Zero, className, null); windowHandle != IntPtr.Zero; windowHandle = FindWindowEx(desktopHandle, windowHandle, className, null))
            {
                // Get the process ID of the window.
                uint processId;
                GetWindowThreadProcessId(windowHandle, out processId);

                // Get the window title.
                StringBuilder title = new StringBuilder(256);
                GetWindowText(windowHandle, title, 256);

                titles.Add(title.ToString());
            }

            return titles;
        } // FindWindowsByClass

        public static int GetNumberFromTitle(string title) //ให้ title เรียงตามตัวอักษรแบบถูกต้อง
        {
            Match match = Regex.Match(title, @"\d+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            else
            {
                // Handle cases where no digits are found in the title
                return 0; // Or any other default value you prefer
            }
        }


        public static Task SetLabelAsync(System.Windows.Forms.Label label, string text, string tBackColor = "Transparent", string tColor = "Black")
        {
            var tcs = new TaskCompletionSource<object>();
            label.BeginInvoke((Action)delegate
            {
                label.Text = text;
                label.ForeColor = System.Drawing.Color.FromName(tColor);
                if (tBackColor == "Green")
                {
                    label.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);  // เพิ่ม Background Color ให้กับ Label
                }
                else if (tBackColor == "Red")
                {
                    label.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);  // เพิ่ม Background Color ให้กับ Label
                }
                else if (tBackColor == "Green_UI")
                {
                    label.BackColor = System.Drawing.Color.PaleGreen;  // เพิ่ม Background Color ให้กับ Label
                }
                else if (tBackColor == "Red_UI")
                {
                    label.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);  // เพิ่ม Background Color ให้กับ Label
                }
                else if (tBackColor == "Blue_UI")
                {
                    label.BackColor = System.Drawing.Color.LightSkyBlue;  // เพิ่ม Background Color ให้กับ Label
                }
                else if (tBackColor == "Yellow_UI")
                {
                    label.BackColor = System.Drawing.Color.Yellow;
                }
                else if (tBackColor == "Yellow")
                {
                    label.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
                }
                else if (tBackColor == "Blue")
                {
                    label.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
                }

                else
                {
                    label.BackColor = System.Drawing.Color.FromName(tBackColor); // เพิ่ม Background Color ให้กับ Label
                }


                tcs.SetResult(null);
            });
            return tcs.Task;
        }

        public static Task<string> GetTextBoxItemAsync(System.Windows.Forms.TextBox textBox)
        {
            var tcs = new TaskCompletionSource<string>();
            textBox.BeginInvoke((MethodInvoker)delegate
            {
                tcs.SetResult(textBox.Text.ToString());
            });
            return tcs.Task;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);


        //// Display
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            PrimaryDevice = 0x4,
            MirroringDriver = 0x8,
            VGACompatible = 0x10,
            Removable = 0x20,
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, int iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        public static List<string> GetDisplayNumbers()
        {
            List<string> displayNumbers = new List<string>();
            int displayIndex = 1;

            MonitorEnumDelegate callback = (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
            {
                displayNumbers.Add($"Display {displayIndex}");
                displayIndex++;
                return true;
            };

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);
            return displayNumbers;
        }



        public static void SaveSettings(Line_Notify.Line_Notify form)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string settingsFilePath = Path.Combine(filePath, "Settings.config");

                if (!File.Exists(settingsFilePath))
                {
                    CreateDefaultSettings(settingsFilePath);
                }

                Configuration config = ConfigurationManager.OpenExeConfiguration(settingsFilePath);

                if (!string.IsNullOrEmpty(form.TextBox1.Text) && !string.IsNullOrEmpty(form.TextBox2.Text))
                {
                    config.AppSettings.Settings["Line_Token"].Value = form.TextBox1.Text;
                    config.AppSettings.Settings["Time"].Value = form.TextBox2.Text;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        public static  void CreateDefaultSettings(string settingsFilePath)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = settingsFilePath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            config.AppSettings.Settings.Add("Line_Token", "");
            config.AppSettings.Settings.Add("Time", "");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void LoadSettings(Line_Notify.Line_Notify form)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");
                string settingsFilePath = Path.Combine(filePath, "Settings.config");

                if (File.Exists(settingsFilePath))
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(settingsFilePath);

                    string lineToken = config.AppSettings.Settings["Line_Token"].Value;
                    string time = config.AppSettings.Settings["Time"].Value;

                    // นำค่าที่โหลดได้มากำหนดให้กับ TextBox ใน form
                    form.TextBox1.Text = lineToken;
                    form.TextBox2.Text = time;
                }
                else
                {
                    // หากไฟล์ Settings.config ไม่มีอยู่ ให้สร้างค่าตั้งต้น
                    CreateDefaultSettings(settingsFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }



    }

}
