using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Balance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Load("start.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Load("start.jpg");
        }

        private static string pattern = @"-i start.jpg -vf curves=r='0.5/0.{0}':g='0.5/0.{1}':b='0.5/0.{2}' tmp1.jpg -y";
        private static string curvesModePattern = @"curves=r='0.5/0.{0}':g='0.5/0.{1}':b='0.5/0.{2}'";
        
        private object[] getArgs()
        {
            var red = trackBar1.Value;
            var blue = trackBar2.Value;
            var green = trackBar3.Value;
            return new object[] { red, green, blue };
        }

        private void change()
        {
            pictureBox2.Load("clear.jpg");
            File.Delete("tmp1.jpg");
            ApplyCommand(pattern, getArgs());
            pictureBox2.Load("tmp1.jpg");
        }

        private static string ApplyCommand(string argsFormat, object[] args)
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = @"C:\ffmpeg\bin\ffmpeg.exe",
                    Arguments = string.Format(argsFormat, args),
                    CreateNoWindow = true
                }
            };
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            change();
        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            change();
        }

        private void trackBar3_MouseUp(object sender, MouseEventArgs e)
        {
            change();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format(curvesModePattern, getArgs()));
        }
    }
}
