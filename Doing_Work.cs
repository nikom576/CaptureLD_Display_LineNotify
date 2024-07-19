using BOT_CBM_DATAGRID.Tools_Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Line_Notify
{
    internal static class Doing_Work
    {
        public static async Task Doing_Main(Line_Notify form)
        {

            Line_Notify.Start_count.Start(); // เริ่มจับเวลา

            while (form.Start_Request)
            {
                int time_text2_input = int.Parse(await JBot.GetTextBoxItemAsync(form.TextBox2));
                int time_second = time_text2_input * 60; // เวลาที่รับจาก TextInput  ทำให้เป็น วินาที
                Console.WriteLine("time = " + time_second);

                long time_elapsed = Line_Notify.Start_count.ElapsedMilliseconds / 1000; // เวลาที่ผ่านไปจากการจับเวลา เป็น วินาที

                long timeToNotice = (time_second) - (time_elapsed);
                TimeSpan NoticeTimeSpan = TimeSpan.FromSeconds(timeToNotice);
                string NoticetTime = $"อีก {NoticeTimeSpan.Minutes} นาที {NoticeTimeSpan.Seconds} วินาที";
                await JBot.SetLabelAsync(form.Label2, NoticetTime);

                if (time_elapsed >= time_second && !Line_Notify.flag_Time_Line_Send)
                {
                    await JBot.SetLabelAsync(form.Label2, "กำลังส่งแจ้งเตือน");
                    await Cap_n_Send(form); // call cap and send
                    Line_Notify.flag_Time_Line_Send = true;
                }
                else if (Line_Notify.flag_Time_Line_Send)
                {
                    Line_Notify.Start_count.Restart();
                    Line_Notify.flag_Time_Line_Send = false;
                }

                await Task.Delay(2000); // delay 2 s  จะได้ไม่กินทรัพยากร
            }

        }

        public static async Task Cap_n_Send(Line_Notify form)  // cap and send
        {          
            if (form.ComboBox1.Text.Contains("Display")) // ถ้าหากเลือก  display
            {
                string selectedDisplay = form.ComboBox1.SelectedItem.ToString();
                Capture_LineNotify.CaptureDisplayAndSave(selectedDisplay);
                await Task.Delay(1000);
                await Capture_LineNotify.SendImageToLineNotify(form.TextBox1.Text, form.ComboBox1.Text);
            }
            else
            {
                Capture_LineNotify.CaptureAndSaveWindow(Line_Notify.GameHandle, form.ComboBox1.Text);
                await Task.Delay(1000);
                await Capture_LineNotify.SendImageToLineNotify(form.TextBox1.Text, form.ComboBox1.Text);
            }
        }

    }
}
