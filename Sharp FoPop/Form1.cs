using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sharp_FoPop
{
    public partial class Form1 : Form
    {
        public TextBox[,]board;
        public Dictionary<string, Bitfield> Bitfields;
        public bool IsBoardValid;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            board = new TextBox[5,5];
            Bitfields = new Dictionary<string, Bitfield>();
            IsBoardValid = false;

            board[0, 0] = textBox00;
            board[0, 1] = textBox01;
            board[0, 2] = textBox02;
            board[0, 3] = textBox03;
            board[0, 4] = textBox04;
            board[1, 0] = textBox10;
            board[1, 1] = textBox11;
            board[1, 2] = textBox12;
            board[1, 3] = textBox13;
            board[1, 4] = textBox14;
            board[2, 0] = textBox20;
            board[2, 1] = textBox21;
            board[2, 2] = textBox22;
            board[2, 3] = textBox23;
            board[2, 4] = textBox24;
            board[3, 0] = textBox30;
            board[3, 1] = textBox31;
            board[3, 2] = textBox32;
            board[3, 3] = textBox33;
            board[3, 4] = textBox34;
            board[4, 0] = textBox40;
            board[4, 1] = textBox41;
            board[4, 2] = textBox42;
            board[4, 3] = textBox43;
            board[4, 4] = textBox44;


            forceButton.Enabled = false;
            nextButton.Enabled = false;
            backButton.Enabled = false;
            clearButton.Enabled = false;

            binLabel.Text = "";
            positionLabel.Text = "";
            bitseedLabel.Text = "";
            timeLabel.Text = "";
            freespinlabel1.Text = "";
            freespinlabel5.Text = "";
            freespinlabel10.Text = "";
            freespinlabel20.Text = "";
            freespinlabel25.Text = "";

            freespinGroupbox.Visible = false;
         
        }

        private void Board_TextChanged(object sender, EventArgs e)
        {
            IsBoardValid = true;

            foreach (TextBox texts in board)
            {
                if (texts.Text == "")
                {
                    IsBoardValid = false;
                }
                else if(texts.Text.Length == 2)
                {
                    ((TextBox)sender).Text = int.Parse(((TextBox)sender).Text).ToString();
                }
            }

            if (IsBoardValid)
            {
                forceButton.Enabled = true;
                nextButton.Enabled = true;
                backButton.Enabled = true;
            }
            else
            {
                forceButton.Enabled = false;
                nextButton.Enabled = false;
                backButton.Enabled = false;
            }
        }

        private void PushToFile(int seed)
        {
            foreach (TextBox texts in board)
            {
                texts.BackColor = Color.White;
            }

            string[] goodones = new string[30];
            int numcount = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (((1 << (j + i * 5)) & seed)>=1)
                    {
                        board[i, j].BackColor = Color.Maroon;
                        goodones[numcount] = board[i,j].Text;
                        numcount++;
                    }
                }
            }

            bool notIncluded;
            for (int k = 1; (k < 75 && numcount < 30); k++)
            {
                notIncluded = true;
                foreach (TextBox texts in board)
                {
                    if (texts.Text == k.ToString())
                    {
                        notIncluded = false;
                    }
                }
                if (notIncluded)
                {
                    goodones[numcount] = k.ToString();
                    numcount++;
                }
            }

            string result = "";
            foreach (string number in goodones)
            {
                result += number + " ";
            }

            TextWriter tw = new StreamWriter("fo.txt");
            tw.WriteLine(result);
            tw.Close();
        }

        private void forceButton_Click(object sender, EventArgs e)
        {
            bitseedLabel.Text = bitseedUpDown.Value.ToString();
            positionLabel.Text = "1\\1";
            binLabel.Text = "NA";
            PushToFile(Decimal.ToInt32(bitseedUpDown.Value));
            nextButton.Enabled = false;
            backButton.Enabled = false;
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8) e.Handled = true;
            if (((TextBox)sender).Text.Length == 1)
            {
                if (((TextBox)sender).TabIndex < 24)
                {
                    foreach (TextBox texts in board)
                    {
                        if (texts.TabIndex - ((TextBox)sender).TabIndex == 1)
                            texts.Focus();
                    }
                }
            }
        }

        private void bitseedUpDown_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                forceButton_Click(sender, e);
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Browse = new OpenFileDialog();
            Browse.Title = "Load Pattern List";
            Browse.FilterIndex = 2;
            Browse.RestoreDirectory = true;

            if (Browse.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(Browse.OpenFile());
                ParsingPatternList(reader);
            }

        }

        private void ParsingPatternList(StreamReader FileStream)
        {
            string line = "";
            int position = 1;
            int progwin;
            Bitfield temp;
            Match m;

            while ((line = FileStream.ReadLine()) != null)
            {
                progwin = 0;
                if (line.Length > 11)
                {
                    if (line.Substring(0, 10) == "Bit Field:")
                    {
                        temp = new Bitfield(int.Parse(line.Substring(11)), position);
                        line = FileStream.ReadLine();//board 1
                        line = FileStream.ReadLine();//board 2
                        line = FileStream.ReadLine();//board 3
                        line = FileStream.ReadLine();//board 4
                        line = FileStream.ReadLine();//board 5
                        line = FileStream.ReadLine();
                        if (!line.Contains("Special"))
                        {
                            if (line.Contains("progressive"))
                            {
                                temp.progressive = true;
                                m = Regex.Match(line, @"^.*\((\d+),*$", RegexOptions.IgnoreCase);
                                Debug.Print(m.Groups[1].ToString());
                                //progwin = int.Parse(m.Groups[1].ToString());
                            }
                            temp.bins = line.Substring(line.IndexOf(' '), 3).Trim();
                            
                            if(line.Contains("Bonus"))
                            {
                                m = Regex.Match(line, @"^.*and (\d+) plays.*$", RegexOptions.IgnoreCase);
                                temp.popSpins(m.Groups[1].ToString());
                            }

                            Bitfields.Add(temp.bitnum.ToString(), temp);
                            position++;
                        }
                    }                    
                }
            }

            RefreshListBox();
        }

        private void RefreshListBox()
        {
            bitseedBox.Items.Clear();
            foreach (string bits in Bitfields.Keys)
            {
                bitseedBox.Items.Add(bits);
            }
        }

        private void bitseedBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bitseedBox.SelectedIndex >= 0)
            {
                if (IsBoardValid)
                {
                    nextButton.Enabled = true;
                    backButton.Enabled = true;
                }
                positionLabel.Text = Bitfields[bitseedBox.SelectedItem.ToString()].position + "\\" + Bitfields.Count;
                bitseedLabel.Text = bitseedBox.SelectedItem.ToString();
                progressiveLabel.Visible = false;
                freespinGroupbox.Visible = false;
                binLabel.Text = Bitfields[bitseedBox.SelectedItem.ToString()].bins;
                if (Bitfields[bitseedBox.SelectedItem.ToString()].progressive)
                {
                    progressiveLabel.Visible = true;
                }
                if (Bitfields[bitseedBox.SelectedItem.ToString()].freespins!="0")
                {
                    freespinGroupbox.Visible = true;
                    freespinlabel1.Text = Bitfields[bitseedBox.SelectedItem.ToString()].freespins;
                    freespinlabel5.Text = Bitfields[bitseedBox.SelectedItem.ToString()].freespins;
                    freespinlabel10.Text = Bitfields[bitseedBox.SelectedItem.ToString()].freespins;
                    freespinlabel20.Text = Bitfields[bitseedBox.SelectedItem.ToString()].freespins;
                    freespinlabel25.Text = Bitfields[bitseedBox.SelectedItem.ToString()].freespins;
                }
                timeLabel.Text = CalculateTime(Bitfields[bitseedBox.SelectedItem.ToString()].position);
                PushToFile(int.Parse(bitseedBox.SelectedItem.ToString()));
            }
        }

        private string CalculateTime(int pos)
        {
            int time = 0;
            foreach (Bitfield bits in Bitfields.Values)
            {
                if (bits.position >= pos)
                {
                    time += bits.seconds;
                }
            }

            int hours = time / 3600;
            int minutes = (time % 3600) / 60;
            int seconds = time % 60;

            return hours + ":" + minutes + ":" + seconds;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (bitseedBox.SelectedIndex != 0 && bitseedBox.Items.Count>0)
            {
                bitseedBox.SelectedIndex--;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (bitseedBox.SelectedIndex != bitseedBox.Items.Count - 1)
            {
                bitseedBox.SelectedIndex++;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)&& !bitseedUpDown.Focused)
            {
                nextButton_Click(sender, e);
            }
        }
    }
}
