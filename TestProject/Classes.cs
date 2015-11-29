using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TestProject
{
    class Program
    {

        const double samplesLength = 0.25;
        const double silenceTime = 3;
        public static void Main()
        {
            var numberPattern = @"\d\.eE\-\+";
            var regex = new Regex(@"([" + numberPattern + "]+)[ ]+([" + numberPattern + "]+)");
            var result = new List<Tuple<double, double>>();
            double sumAmplitude = 0;
            double startTime = 0;
            var sampleStarts = true;

            var process = new Process();
            process.StartInfo.FileName = @"C:\sox\sox.exe";
            process.StartInfo.Arguments = @"C:\Users\Yura\Desktop\TestMontage\Input\Hackerdom\09\09-2\voice.wav -t dat -";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.StandardOutput.ReadLine();
            process.StandardOutput.ReadLine();
            while(true)
            {
                if (process.HasExited) break;
                var line = process.StandardOutput.ReadLine();
                var c = line[line.Length - 1];
                var match = regex.Match(line);
                if (!match.Success) 
                    continue;
                var time=double.Parse(match.Groups[1].Value,CultureInfo.InvariantCulture);
                var amplitute = double.Parse(match.Groups[2].Value,CultureInfo.InvariantCulture);
                if (sampleStarts)
                {
                    startTime = time;
                    sampleStarts = false;
                }
                sumAmplitude += Math.Abs(amplitute);
                if (time-startTime>samplesLength)
                {
                    result.Add(Tuple.Create(startTime, sumAmplitude / (time - startTime)));
                    sampleStarts = true;
                    sumAmplitude = 0;
                    if (startTime > 60) break;
                }
            }

            var silenceLevel = result.Where(z => z.Item1 < silenceTime).Max(z => z.Item2);
       
            var max = result.Max(z => z.Item2);

            var chart = new Chart();
            chart.ChartAreas.Add(new ChartArea());
            var series = new Series();
            var voice = new Series { ChartType = SeriesChartType.FastLine };
            foreach (var e in result)
            {
                series.Points.AddXY(e.Item1, e.Item2);
                voice.Points.AddXY(e.Item1, e.Item2 > silenceLevel ? max : 0);
            }
            chart.Series.Add(series);
            chart.Series.Add(voice);
            chart.Dock = System.Windows.Forms.DockStyle.Fill;
            var form = new Form();
            form.Controls.Add(chart);
            System.Windows.Forms.Application.Run(form);
        }
    }
}
