using BOT_CBM_DATAGRID.Tools_Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BOT_CBM_DATAGRID.Tools_Class.JBot;

namespace Line_Notify
{
    public partial class Line_Notify : Form
    {
        public Line_Notify()
        {
            InitializeComponent();
            Start_count = new Stopwatch();
        }

        public static IntPtr GameHandle { get; set; }

        public static Stopwatch Start_count { get; set; } 
        public static bool flag_Time_Line_Send { get; set; } = false;

        public bool Start_Request { get; set; } = false; // Start
        public bool Stop_Request { get; set; } = false; // Start
        public bool Ready_Request { get; set; } = false; // Ready

        public System.Windows.Forms.ComboBox ComboBox1 => comboBox1; // หน้าต่างที่จะทำการ Capture
        public System.Windows.Forms.Label Label2 => label2; // Status
        public System.Windows.Forms.TextBox TextBox1 => textBox1; // Line_Token
        public System.Windows.Forms.TextBox TextBox2 => textBox2; // Time

        private void button1_Click(object sender, EventArgs e) // Reload
        {
            comboBox1.Items.Clear();  // clear ข้อมูลออกให้หมดก่อนโหลดลงใหม่

            List<string> displayNumbers = JBot.GetDisplayNumbers();

            foreach (var displayNumber in displayNumbers)
            {
                comboBox1.Items.Add(displayNumber);
            }


            List<string> titles = JBot.FindWindowsByClass("LDPlayerMainFrame"); // ld
            var sortedTitles = titles
                .OrderBy(t => JBot.GetNumberFromTitle(t))
                .ThenBy(t => t)
                .ToList(); // เรียงลำดับ titles โดยใช้เลขก่อนแล้วค่อยใช้ตัวอักษรตามปกติ

            foreach (string title in sortedTitles)
            {
                comboBox1.Items.Add(title);
            }

            try
            {
                comboBox1.SelectedIndex = 0;  // เลือก combo 1
            }
            catch (Exception)
            {
                Console.WriteLine("Exception ตรงนี้");
                Console.WriteLine("ไม่พบหน้าต่าง LD");
            }

            label2.Text = "Reload";

            
        }
        private void button2_Click(object sender, EventArgs e)  // Select
        {
            GameHandle = Tool_CHECK_HWID.GetWindowHandleByTitle(comboBox1.Text);
            Console.WriteLine(GameHandle.ToString("X"));

            ChangeTitle(this, comboBox1.Text);
            label2.Text = comboBox1.Text;

            Ready_Request = true;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private  async void button4_Click(object sender, EventArgs e) // Start
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                Properties.Settings.Default.Line_Token = textBox1.Text;
                Properties.Settings.Default.Time = textBox2.Text;
                Properties.Settings.Default.Save();
            }

            if (!Start_Request && Ready_Request)
            {
                Start_Request = true;
                Stop_Request = false;
                await Doing_Work.Doing_Main(this); // go to main function
            }


        }

        private void Line_Notify_Load(object sender, EventArgs e)  // Form Load
        {

            List<string> displayNumbers = JBot.GetDisplayNumbers();

            foreach (var displayNumber in displayNumbers)
            {
                comboBox1.Items.Add(displayNumber);
            }

            List<string> titles = JBot.FindWindowsByClass("LDPlayerMainFrame"); // ld
            var sortedTitles = titles
                .OrderBy(t => JBot.GetNumberFromTitle(t))
                .ThenBy(t => t)
                .ToList(); // เรียงลำดับ titles โดยใช้เลขก่อนแล้วค่อยใช้ตัวอักษรตามปกติ

            foreach (string title in sortedTitles)
            {
                comboBox1.Items.Add(title);
            }

            try
            {
                comboBox1.SelectedIndex = 0;  // เลือก combo 1
            }
            catch (Exception)
            {
                Console.WriteLine("Exception ตรงนี้");
                Console.WriteLine("ไม่พบหน้าต่าง LD");
            }


            try
            {
                string Line_Token = Properties.Settings.Default.Line_Token;
                string Time = Properties.Settings.Default.Time;

                // ตรวจสอบว่ามีข้อมูลบันทึกไว้หรือไม่
                if (!string.IsNullOrEmpty(Line_Token) && !string.IsNullOrEmpty(Time))
                {
                    textBox1.Text = Line_Token;
                    textBox2.Text = Time;
                }
            }
            catch 
            {

                Console.WriteLine("Exception ตรงนี้");
            }
            
        }   

        private async void button3_Click(object sender, EventArgs e)  // Test
        {
            await Doing_Work.Cap_n_Send(this);


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        private void Line_Notify_FormClosed(object sender, FormClosedEventArgs e) // Close
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
                {
                    Properties.Settings.Default.Line_Token = textBox1.Text;
                    Properties.Settings.Default.Time = textBox2.Text;
                    Properties.Settings.Default.Save();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

        }

        public static void ChangeTitle(Form form, string newTitle)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action(() => form.Text = newTitle));
            }
            else
            {
                form.Text = newTitle;
            }
        }



    }
}
