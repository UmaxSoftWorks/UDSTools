using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace Info.txt_Generator
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
                groupBox1.Text = "Папка с шаблоном";
                groupBox2.Text = "Файлы";
                groupBox3.Text = "Действия";
                label1.Text = "Тип";
                button2.Text = "Сохранить Info.txt";
                Column1.HeaderText = "Файл";
                Column2.HeaderText = "Тип";
            #else
                groupBox1.Text = "Folder with template";
                groupBox2.Text = "Files";
                groupBox3.Text = "Actions";
                label1.Text = "Type";
                button2.Text = "Save Info.txt";
                Column1.HeaderText = "File";
                Column2.HeaderText = "Type";        
            #endif
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = string.Empty;
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath == string.Empty)
            {
                return;
            }
            if (!folderBrowserDialog1.SelectedPath.EndsWith("\\"))
            {
                folderBrowserDialog1.SelectedPath += "\\";
            }
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            //Filling up table
            
            dataGridView1.Rows.Clear();
            string[] files = new string[0];
            try
            {
                files = Directory.GetFiles(textBox1.Text, "*", SearchOption.AllDirectories);
            }
            catch (Exception)
            {
#if Umax
                MessageBox.Show("Ошибка!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                MessageBox.Show("Error!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith("info.txt"))
                {
                    continue;
                }
                dataGridView1.Rows.Add();

                if (files[i].Substring(textBox1.Text.Length).Contains("|"))
                {
                    string value = files[i].Substring(textBox1.Text.Length);
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = value.Substring(0, value.IndexOf("|"));
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Value = value.Substring(value.IndexOf("|") + 1);
                }
                else
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = files[i].Substring(textBox1.Text.Length);
                }

                if (files[i].EndsWith("index.html") || files[i].EndsWith("index.htm"))
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = "Index";
                }
                else if (files[i].EndsWith("html") || files[i].EndsWith("htm") || files[i].Contains("|"))
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = "Page";
                }
                else if (files[i].EndsWith("jpg") || files[i].EndsWith("jpeg") || files[i].EndsWith("bmp")
                     || files[i].EndsWith("png") || files[i].EndsWith("gif"))
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = "Image";
                }
                else
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = "File";
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                return;
            }
            //Find Selected rows
            DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
            //Changing
            for (int i = 0; i < selectedRows.Count; i++)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        {
                            selectedRows[i].Cells[1].Value = "Index";
                            break;
                        }
                    case 1:
                        {
                            selectedRows[i].Cells[1].Value = "Page";
                            break;
                        }
                    case 2:
                        {
                            selectedRows[i].Cells[1].Value = "Static Page";
                            break;
                        }
                    case 3:
                        {
                            selectedRows[i].Cells[1].Value = "Category";
                            break;
                        }
                    case 4:
                        {
                            selectedRows[i].Cells[1].Value = "Map";
                            break;
                        }
                    case 5:
                        {
                            selectedRows[i].Cells[1].Value = "File";
                            break;
                        }
                    case 6:
                        {
                            selectedRows[i].Cells[1].Value = "Image";
                            break;
                        }

                    case 7:
                        {
                            selectedRows[i].Cells[1].Value = "Custom";
                            break;
                        }
                }
            }
            //Return rows to table
            for (int i = 0; i < selectedRows.Count; i++)
            {
                //Search row
                for (int k = 0; k < dataGridView1.Rows.Count; k++)
                {
                    if (dataGridView1.Rows[k].Cells[0].Value.ToString() == selectedRows[i].Cells[0].Value.ToString())
                    {
                        //Changing
                        dataGridView1.Rows[k].Cells[1].Value = selectedRows[i].Cells[1].Value.ToString();
                        break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int indexCount = 0;
                //Saving
                List<string> files = new List<string>(dataGridView1.Rows.Count);
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    switch (dataGridView1.Rows[i].Cells[1].Value.ToString())
                    {
                        case "Index":
                            {
                                files.Add("index=" + dataGridView1.Rows[i].Cells[0].Value.ToString());
                                indexCount++;
                                break;
                            }
                        case "Page":
                            {
                                string value = "page=" + dataGridView1.Rows[i].Cells[0].Value.ToString();
                                int percentage = 0;
                                if (dataGridView1.Rows[i].Cells[0].Value != null && int.TryParse(dataGridView1.Rows[i].Cells[0].Value as string, out percentage))
                                {
                                    value += "|" + percentage.ToString();
                                }

                                files.Add(value);
                                break;
                            }
                        case "Static Page":
                            {
                                files.Add("static=" + dataGridView1.Rows[i].Cells[0].Value.ToString());
                                break;
                            }
                        case "Category":
                            {
                                files.Add("category=" + dataGridView1.Rows[i].Cells[0].Value.ToString());
                                break;
                            }
                        case "Map":
                            {
                                files.Add("map=" + dataGridView1.Rows[i].Cells[0].Value.ToString());
                                break;
                            }
                        case "File":
                            {
                                files.Add("file=" + dataGridView1.Rows[i].Cells[0].Value.ToString());
                                break;
                            }
                        case "Image":
                            {
                                files.Add("image=" + dataGridView1.Rows[i].Cells[0].Value.ToString());
                                break;
                            }
                        case "Custom":
                            {
                                files.Add("custom=" + dataGridView1.Rows[i].Cells[0].Value.ToString());
                                break;
                            }
                    }
                }
                if (indexCount == 0 || indexCount > 1)
                {
                    throw new Exception();
                }
                File.WriteAllLines(folderBrowserDialog1.SelectedPath + "info.txt", files.ToArray<string>(), Encoding.Default);
                //Show message
                #if Umax
                    MessageBox.Show("Файл Info.txt успешно сохранен!", "Готово!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                #else
                    MessageBox.Show("File Info.txt saved!", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                #endif
            }
            catch (Exception)
            {
                #if Umax
                    MessageBox.Show("Ошибка!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                #else
                    MessageBox.Show("Error!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                #endif
            }
        }
    }
}
