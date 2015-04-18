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

namespace Key_Maker
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
            //Interface
            #if Umax
                label1.Text = "Входной файл";
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

        struct WData
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

        private void button3_Click(object sender, EventArgs e)
        {
            //Prepare
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

        private void Work()
        {
            try
            {
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
                StringBuilder outData = new StringBuilder(100000);
                for (int i = 0; i < data.Length; i++)
                {
                    try
                    {
                        AddKeywordsToData(ref outData, MakeKeywords(data[i]));
                    }
                    catch (Exception)
                    {
                    }
                    WorkData.CurrentStatus++;
                }
                //Saving
                if (WorkData.EncodingType == 0)
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

        private void AddKeywordsToData(ref StringBuilder Data, string[] Keywords)
        {
            for (int i = 0; i < Keywords.Length; i++)
            {
                Data.Append(Keywords[i] + "\r\n");
            }
        }

        private string[] MakeKeywords(string Keyword)
        {
            string[] outKeywords = new string[1];
            if (Keyword.Contains(" "))
            {
                string[] parts = Keyword.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 5)
                {
                    switch (parts.Length)
                    {
                        case 2:
                            {
                                outKeywords = new string[2];
                                outKeywords[0] = parts[0] + " " + parts[1];
                                outKeywords[1] = parts[1] + " " + parts[0];
                                break;
                            }
                        case 3:
                            {
                                outKeywords = new string[6];
                                outKeywords[0] = parts[0] + " " + parts[1] + " " + parts[2];
                                outKeywords[1] = parts[0] + " " + parts[2] + " " + parts[1];
                                outKeywords[2] = parts[1] + " " + parts[0] + " " + parts[2];
                                outKeywords[3] = parts[1] + " " + parts[2] + " " + parts[0];
                                outKeywords[4] = parts[2] + " " + parts[0] + " " + parts[1];
                                outKeywords[5] = parts[2] + " " + parts[1] + " " + parts[0];
                                break;
                            }
                        case 4:
                            {
                                outKeywords = new string[24];
                                outKeywords[0] = parts[0] + " " + parts[1] + " " + parts[2] + " " + parts[3];
                                outKeywords[1] = parts[0] + " " + parts[1] + " " + parts[3] + " " + parts[2];
                                outKeywords[2] = parts[0] + " " + parts[2] + " " + parts[1] + " " + parts[3];
                                outKeywords[3] = parts[0] + " " + parts[2] + " " + parts[3] + " " + parts[1];
                                outKeywords[4] = parts[0] + " " + parts[3] + " " + parts[1] + " " + parts[2];
                                outKeywords[5] = parts[0] + " " + parts[3] + " " + parts[2] + " " + parts[1];
                                outKeywords[6] = parts[1] + " " + parts[0] + " " + parts[2] + " " + parts[3];
                                outKeywords[7] = parts[1] + " " + parts[0] + " " + parts[3] + " " + parts[2];
                                outKeywords[8] = parts[1] + " " + parts[2] + " " + parts[0] + " " + parts[3];
                                outKeywords[9] = parts[1] + " " + parts[2] + " " + parts[3] + " " + parts[0];
                                outKeywords[10] = parts[1] + " " + parts[3] + " " + parts[0] + " " + parts[2];
                                outKeywords[11] = parts[1] + " " + parts[3] + " " + parts[2] + " " + parts[0];
                                outKeywords[12] = parts[2] + " " + parts[0] + " " + parts[1] + " " + parts[3];
                                outKeywords[13] = parts[2] + " " + parts[0] + " " + parts[3] + " " + parts[1];
                                outKeywords[14] = parts[2] + " " + parts[1] + " " + parts[0] + " " + parts[3];
                                outKeywords[15] = parts[2] + " " + parts[1] + " " + parts[3] + " " + parts[0];
                                outKeywords[16] = parts[2] + " " + parts[3] + " " + parts[0] + " " + parts[1];
                                outKeywords[17] = parts[2] + " " + parts[3] + " " + parts[1] + " " + parts[0];
                                outKeywords[18] = parts[3] + " " + parts[0] + " " + parts[1] + " " + parts[2];
                                outKeywords[19] = parts[3] + " " + parts[0] + " " + parts[2] + " " + parts[1];
                                outKeywords[20] = parts[3] + " " + parts[1] + " " + parts[0] + " " + parts[2];
                                outKeywords[21] = parts[3] + " " + parts[1] + " " + parts[2] + " " + parts[0];
                                outKeywords[22] = parts[3] + " " + parts[2] + " " + parts[0] + " " + parts[1];
                                outKeywords[23] = parts[3] + " " + parts[2] + " " + parts[1] + " " + parts[0];
                                break;
                            }
                    }
                }
                else
                {
                    outKeywords[0] = Keyword;
                }

            }
            else
            {
                outKeywords[0] = Keyword;
            }
            return outKeywords;
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
