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

namespace Key_Selector
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
                label1.Text = "Входный файл";
                label2.Text = "Результат";
                label3.Text = "Кодировка";
                button3.Text = "Старт";
                button4.Text = "Стоп";

                groupBox1.Text = "Выбирать";
                checkBox1.Text = "Все кейворды";
                groupBox2.Text = "Длина кейвордов";
                comboBox3.Items.Add("Слов");
                comboBox3.Items.Add("Символов");

                groupBox3.Text = "Фильтр";
                label4.Text = "Вхождение";
                comboBox4.Items.Add("Входят");
                comboBox4.Items.Add("Не входят");
                comboBox5.Items.Add("И");
                comboBox5.Items.Add("Или");
            #else
                label1.Text = "Input file";
                label2.Text = "Result";
                label3.Text = "Encoding";
                button3.Text = "Start";
                button4.Text = "Stop";

                groupBox1.Text = "Select";
                checkBox1.Text = "All keywords";
                groupBox2.Text = "Keywords length";
                comboBox3.Items.Add("Words");
                comboBox3.Items.Add("Symbols");

                groupBox3.Text = "Filter";
                label4.Text = "Entrance";
                comboBox4.Items.Add("Contains");
                comboBox4.Items.Add("Don't contains");
                comboBox5.Items.Add("And");
                comboBox5.Items.Add("OR");
            #endif

            checkBox1.Checked = true;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                groupBox2.Enabled = false;
            }
            else
            {
                groupBox2.Enabled = true;
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
            //Preparing
            if (textBox1.Text == string.Empty || textBox2.Text == string.Empty)
            {
                return;
            }
            WorkData.InputFile = textBox1.Text;
            WorkData.OutputFile = textBox2.Text;
            WorkData.EncodingType = comboBox1.SelectedIndex;
            //Filter
            if (textBox3.Text == string.Empty)
            {
                WorkData.Filter = false;
            }
            else
            {
                WorkData.Filter = true;
                WorkData.FilterWords = textBox3.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                WorkData.FilterEntranceTypeOne = comboBox4.SelectedIndex;
                WorkData.FilterEntranceTypeTwo = comboBox5.SelectedIndex;
            }
            //SelectType
            if (checkBox1.Checked)
            {
                WorkData.SelectAll = true;
                WorkData.SelectLengthType = comboBox2.SelectedIndex;
                WorkData.SelectLength = (int)numericUpDown1.Value;
                WorkData.SelectType = comboBox3.SelectedIndex;
            }
            else
            {
                WorkData.SelectAll = false;
            }
            //Starting
            WorkData.Done = false;
            WorkData.TotalStatus = 0;
            WorkData.CurrentStatus = 0;
            WorkData.MainThread = new Thread(Work);
            WorkData.MainThread.Start();
            timer1.Start();
        }

        struct WData
        {
            public bool Done;

            public int TotalStatus;
            public int CurrentStatus;

            public string InputFile;
            public string OutputFile;
            public int EncodingType;

            public bool Filter;
            public string[] FilterWords;
            /// <summary>
            /// Входят/Не входят
            /// </summary>
            public int FilterEntranceTypeOne;
            /// <summary>
            /// И/Или
            /// </summary>
            public int FilterEntranceTypeTwo;

            public bool SelectAll;
            /// <summary>
            /// Больше/Меньше/Равно
            /// </summary>
            public int SelectLengthType;
            public int SelectLength;
            /// <summary>
            /// Слов/Символов
            /// </summary>
            public int SelectType;

            public Thread MainThread;
        }

        WData WorkData;

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
                for (int i = 0; i < data.Length; i++)
                {
                    //Фильтр
                    if (WorkData.Filter)
                    {
                        if (WorkData.FilterEntranceTypeOne == 0)
                        {
                            //Входят
                            bool Contains;
                            if (WorkData.FilterEntranceTypeTwo == 0)
                            {
                                //И
                                Contains = true;
                                for (int k = 0; k < WorkData.FilterWords.Length; k++)
                                {
                                    if (!data[i].Contains(WorkData.FilterWords[k]))
                                    {
                                        Contains = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //Или
                                Contains = false;
                                for (int k = 0; k < WorkData.FilterWords.Length; k++)
                                {
                                    if (data[i].Contains(WorkData.FilterWords[k]))
                                    {
                                        Contains = true;
                                        break;
                                    }
                                }
                            }
                            if (!Contains)
                            {
                                data[i] = string.Empty;
                            }
                        }
                        else
                        {
                            //Не входят
                            bool Contains;
                            if (WorkData.FilterEntranceTypeTwo == 0)
                            {
                                Contains = true;
                                //И
                                for (int k = 0; k < WorkData.FilterWords.Length; k++)
                                {
                                    if (data[i].Contains(WorkData.FilterWords[k]))
                                    {
                                        Contains = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Contains = false;
                                //Или
                                for (int k = 0; k < WorkData.FilterWords.Length; k++)
                                {
                                    if (!data[i].Contains(WorkData.FilterWords[k]))
                                    {
                                        Contains = true;
                                        break;
                                    }
                                }
                            }
                            if (!Contains)
                            {
                                data[i] = string.Empty;
                            }
                        }
                    }
                    //Выборка
                    if (!WorkData.SelectAll)
                    {
                        if (WorkData.SelectType == 0)
                        {
                            //Words
                            switch (WorkData.SelectLengthType)
                            {
                                case 0:
                                    {
                                        //>
                                        if (data[i].Split(new char[]{ ' ' },  StringSplitOptions.RemoveEmptyEntries).Length <= WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        //>=
                                        if (data[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        //==
                                        if (data[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length != WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        //<=
                                        if (data[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        //<
                                        if (data[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length >= WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            //Symbols
                            switch (WorkData.SelectLengthType)
                            {
                                case 0:
                                    {
                                        //>
                                        if (data[i].Length <= WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        //>=
                                        if (data[i].Length < WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        //==
                                        if (data[i].Length != WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        //<=
                                        if (data[i].Length > WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        //<
                                        if (data[i].Length >= WorkData.SelectLength)
                                        {
                                            data[i] = string.Empty;
                                        }
                                        break;
                                    }
                            }
                        }
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
