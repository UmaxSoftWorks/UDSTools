using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;

namespace Mixer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Resource.MainIcon;
            comboBox1.SelectedIndex = 0;
            //Title
            this.Text = "Umax " + this.Text;
            //Language
            #if Umax
                label1.Text = "Входный файл";
                label2.Text = "Результат";
                label3.Text = "Кодировка";
                button3.Text = "Старт";
                button4.Text = "Стоп";           
            #else
                label1.Text = "Input file";
                label2.Text = "Result";
                label3.Text = "Encoding";
                button3.Text = "Start";
                button4.Text = "Stop";
            #endif
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == string.Empty)
            {
                return;
            }
            textBox1.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = string.Empty;
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName == string.Empty)
            {
                return;
            }
            textBox2.Text = saveFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Checking settings
            if (textBox1.Text == string.Empty || textBox2.Text == string.Empty)
            {
                return;
            }
            WorkData.InputFile = textBox1.Text;
            WorkData.OutputFile = textBox2.Text;
            WorkData.EncodingType = comboBox1.SelectedIndex;
            //Start
            WorkData.Done = false;
            WorkData.CurrentStatus = 0;
            WorkData.TotalStatus = 0;
            WorkData.MainThread = new Thread(Work);
            WorkData.MainThread.Start();
            timer1.Start();
        }

        private struct WData
        {
            public bool Done;

            public int CurrentStatus;
            public int TotalStatus;

            public int EncodingType;
            public string InputFile;
            public string OutputFile;

            public Thread MainThread;
        }

        private WData WorkData;

        private void Work()
        {
            try
            {
                Random random = new Random();
                //Loading
                string[] data;
                if (WorkData.EncodingType == 0)
                {
                    data = File.ReadAllLines(WorkData.InputFile, Encoding.Default);
                }
                else
                {
                    data = File.ReadAllLines(WorkData.InputFile, Encoding.UTF8);
                }
                WorkData.TotalStatus = data.Length;
                //Working
                for (int i = 0; i < data.Length; i++)
                {
                    try
                    {
                        SwapStrings(ref data[i], ref data[random.Next(data.Length)]);
                    }
                    catch (Exception)
                    {
                    }
                    WorkData.CurrentStatus++;
                }
                //Saving
                if (WorkData.EncodingType == 0)
                {
                    File.WriteAllLines(WorkData.OutputFile, data, Encoding.Default);
                }
                else
                {
                    File.WriteAllLines(WorkData.OutputFile, data, Encoding.UTF8);
                }
            }
            catch (Exception)
            {
            }
            WorkData.Done = true;
        }

        private void SwapStrings(ref string StringOne, ref string StringTwo)
        {
            string tempString = StringOne;
            StringOne = StringTwo;
            StringTwo = tempString;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            try
            {
                WorkData.MainThread.Abort();
            }
            catch (Exception)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (WorkData.Done)
            {
                timer1.Stop();
            }
            try
            {
                progressBar1.Maximum = WorkData.TotalStatus;
                progressBar1.Value = WorkData.CurrentStatus;
            }
            catch (Exception)
            {
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            try
            {
                WorkData.MainThread.Abort();
            }
            catch (Exception)
            {
            }
        }
    }
}
