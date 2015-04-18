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

namespace Text_Cleaner
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
            //Translating
            #if Umax
                label1.Text = "Входной файл";
                label2.Text = "Результат";
                label3.Text = "Кодировка";
                label4.Text = "Удалять строки короче";
                checkBox1.Text = "Удалять строки без рус. гласных";
                checkBox2.Text = "Удалять строки без анг. гласных";
                button3.Text = "Старт";
                button4.Text = "Стоп";
            #else
                label1.Text = "Input file";
                label2.Text = "Result";
                label3.Text = "Encoding";
                label4.Text = "Delete strings less";
                checkBox1.Text = "Delete string without RU letters";
                checkBox2.Text = "Delete string without EN letters";
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

        private void button3_Click(object sender, EventArgs e)
        {
            //Checking Setings
            if (textBox1.Text == string.Empty || textBox2.Text == string.Empty)
            {
                return;
            }
            WorkData.InputFile = textBox1.Text;
            WorkData.OutputFile = textBox2.Text;
            WorkData.SettingsEncodingType = comboBox1.SelectedIndex;
            WorkData.SettingsMinStringLength = (int)numericUpDown1.Value;
            WorkData.SettingDeleteStringWithOutEn = checkBox2.Checked;
            WorkData.SettingDeleteStringWithOutRu = checkBox1.Checked;
            //Starting
            WorkData.Done = false;
            WorkData.CurrentStatus = 0;
            WorkData.TotalStatus = 0;
            WorkData.MainThread = new Thread(Work);
            WorkData.MainThread.Start();
            timer1.Start();
        }

        struct WData
        {
            public bool Done;
            public int CurrentStatus;
            public int TotalStatus;

            public Thread MainThread;

            public string InputFile;
            public string OutputFile;

            public int SettingsEncodingType;
            public int SettingsMinStringLength;
            public bool SettingDeleteStringWithOutEn;
            public bool SettingDeleteStringWithOutRu;
        }

        private WData WorkData;

        private void Work()
        {
            try
            {
                //Loading data
                string[] data;
                if (WorkData.SettingsEncodingType == 0)
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
                        if (data[i].Length < WorkData.SettingsMinStringLength)
                        {
                            data[i] = string.Empty;
                        }
                        if (data[i] != string.Empty)
                        {
                            if (WorkData.SettingDeleteStringWithOutEn)
                            {
                                string tempString = data[i].ToLower();
                                if (!tempString.Contains("e") || !tempString.Contains("i") || !tempString.Contains("o")
                                    || !tempString.Contains("a") || !tempString.Contains("u"))
                                {
                                    data[i] = string.Empty;
                                }
                            }
                        }
                        if (data[i] != string.Empty)
                        {
                            if (WorkData.SettingDeleteStringWithOutRu)
                            {
                                string tempString = data[i].ToLower();
                                if (!tempString.Contains("у") || !tempString.Contains("е") || !tempString.Contains("ы")
                                    || !tempString.Contains("а") || !tempString.Contains("о") || !tempString.Contains("и"))
                                {
                                    data[i] = string.Empty;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    WorkData.CurrentStatus++;
                }
                //Saving
                StringBuilder outData = new StringBuilder(100000);
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != string.Empty)
                    {
                        outData.Append(data[i] + "\r\n");
                    }
                }
                if (WorkData.SettingsEncodingType == 0)
                {
                    File.WriteAllText(WorkData.OutputFile, outData.ToString(), Encoding.Default);
                }
                else
                {
                    File.WriteAllText(WorkData.OutputFile, outData.ToString(), Encoding.UTF8);
                }
            }
            catch (Exception)
            {
            }
            WorkData.Done = true;
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
    }
}
