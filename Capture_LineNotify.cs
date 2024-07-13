using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_Notify
{
    internal static class Capture_LineNotify
    {
        public static async Task<bool> SendImageToLineNotify(string accessToken,string title)
        {
            string configFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
            }

            string imagePath = Path.Combine(configFolderPath, title + ".png");


            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var form = new MultipartFormDataContent())
                    {
                        using (var imageStream = File.OpenRead(imagePath))
                        {
                            using (var imageContent = new StreamContent(imageStream))
                            {
                                // แนบรูปภาพ
                                form.Add(imageContent, "imageFile", Path.GetFileName(imagePath));

                                // แนบข้อความ
                                form.Add(new StringContent(title), "message");

                                // ส่งคำขอ POST ไปยัง API Line Notify พร้อมกับแนบ Access Token ใน header
                                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                                var response = await httpClient.PostAsync("https://notify-api.line.me/api/notify", form);

                                // ตรวจสอบสถานะการตอบกลับจาก API Line Notify
                                if (response.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("ส่งรูปภาพไปยัง Line Notify เรียบร้อยแล้ว");
                                    return true;
                                }
                                else
                                {
                                    Console.WriteLine($"เกิดข้อผิดพลาดในการส่งรูปภาพ: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception ตรงนี้");
                Console.WriteLine($"เกิดข้อผิดพลาดในการส่งรูปภาพ: {ex.Message}");
            }

            return false;
        }
        public static void CaptureAndSaveWindow(IntPtr handle, string title)
        {
            RECT rect;
            GetWindowRect(handle, out rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    IntPtr hdc = graphics.GetHdc();
                    PrintWindow(handle, hdc, 0);
                    graphics.ReleaseHdc(hdc);
                }

                string imgFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
                if (!Directory.Exists(imgFolderPath))
                {
                    Directory.CreateDirectory(imgFolderPath);
                }

                string ImgFilePath = Path.Combine(imgFolderPath, title + ".png");

                bitmap.Save(ImgFilePath); // บันทึกภาพลงเป็นไฟล์
            }
        }

        public static void CaptureDisplayAndSave(string displayName)
        {
            // Get all screens
            Screen[] screens = Screen.AllScreens;

            // Find the display index from the display name
            int displayNumber = -1;
            if (displayName.StartsWith("Display") && int.TryParse(displayName.Substring(7), out displayNumber))
            {
                // Adjust to 0-based index
                displayNumber -= 1;
            }

            // Check if the display number is valid
            if (displayNumber < 0 || displayNumber >= screens.Length)
            {
                //MessageBox.Show("Invalid display name.");
                return;
            }

            // Get the selected screen
            Screen screen = screens[displayNumber];

            // Create a bitmap of the screen size
            Bitmap bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap
            Graphics graphics = Graphics.FromImage(bitmap);

            // Copy the screen to the bitmap
            graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size, CopyPixelOperation.SourceCopy);

            string imgFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");

            // ตรวจสอบว่ามีโฟลเดอร์ img อยู่หรือไม่ ถ้าไม่มีให้สร้าง
            if (!Directory.Exists(imgFolderPath))
            {
                Directory.CreateDirectory(imgFolderPath);
            }

            // บันทึกไฟล์ Bitmap ไปยังโฟลเดอร์ img
            string filePath = Path.Combine(imgFolderPath, $"{displayName}.png");

            bitmap.Save(filePath, ImageFormat.Png);

            // Release resources
            graphics.Dispose();
            bitmap.Dispose();

            //MessageBox.Show($"Screenshot of {displayName} saved to {filePath}");
        }


        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

    }
}
