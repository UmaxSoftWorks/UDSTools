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

namespace Text_Filter
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
            label4.Text = "Фильтр";

            comboBox2.Items.Add("Удалять слова");
            comboBox2.Items.Add("Удалять строки");
#else
                label1.Text = "Input file";
                label2.Text = "Result";
                label3.Text = "Encoding";
                button3.Text = "Start";
                button4.Text = "Stop";
                label4.Text = "Filter";
            comboBox2.Items.Add("Remove words");
            comboBox2.Items.Add("Remove strings");
#endif
            comboBox2.SelectedIndex = 0;
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
            if (textBox1.Text == string.Empty || textBox2.Text == string.Empty || textBox3.Text == string.Empty)
            {
                return;
            }
            WorkData.InputFile = textBox1.Text;
            WorkData.OutputFile = textBox2.Text;
            WorkData.EncodingType = comboBox1.SelectedIndex;
            WorkData.FilterType = comboBox2.SelectedIndex;
            WorkData.Filter = textBox3.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
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

            public int FilterType;
            public string[] Filter;

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
                        if (WorkData.FilterType == 0)
                        {
                            //Words
                            for (int k = 0; k < WorkData.Filter.Length; k++)
                            {
                                data[i] = data[i].Replace(WorkData.Filter[k], string.Empty);
                            }
                        }
                        else
                        {
                            //String
                            bool contains = false;
                            for (int k = 0; k < WorkData.Filter.Length; k++)
                            {
                                if (data[i].Contains(WorkData.Filter[k]))
                                {
                                    contains = true;
                                    break;
                                }
                            }
                            if (contains)
                            {
                                data[i] = string.Empty;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    WorkData.CurrentStatus++;
                }
                //Saving
                StringBuilder outData = new StringBuilder(10000);
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != string.Empty)
                    {
                        outData.Append(data[i] + "\r\n");
                    }
                }
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
    }
}
