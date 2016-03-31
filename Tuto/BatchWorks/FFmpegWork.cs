using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.IO;
using Tuto.Model;
using System.ComponentModel;
using System.Threading;
using System.Text.RegularExpressions;

namespace Tuto.BatchWorks
{
    public abstract class FFmpegWork : ProcessBatchWork
    {
        private double TotalSeconds { get; set; }
        private bool durationLoaded;

        public override void RunProcess(string args, string path)
        {
            ProcessStartInfo oInfo = new ProcessStartInfo(path, args);
            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            oInfo.RedirectStandardError = true;
            Process.StartInfo = oInfo;
            Process.EnableRaisingEvents = true;
            Process.ErrorDataReceived += new DataReceivedEventHandler(DataReceived);
            Process.Start();
            Process.BeginErrorReadLine();
            Process.WaitForExit();
            if (Process.ExitCode != 0)
                throw new ArgumentException(
                    string.Format("Process' exit code not equals zero. \n Exe: \"{0}\" \n Args: {1} \n Full: \"{0}\" {1}", path, args));
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (!durationLoaded)
                {
                    try
                    {
                        TotalSeconds = ExtractDuration(e.Data).TotalSeconds;
                        if (TotalSeconds > 0)
                            durationLoaded = true;
                    }
                    catch { }
                }

                if (e.Data.StartsWith("frame"))
                {
                    string[] parts = e.Data.Split(new string[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
                    var timeDraft = TimeSpan.Parse(parts[9]);
                    var currentTimeInSeconds = timeDraft.TotalSeconds;
                    Progress = (short)Math.Round(currentTimeInSeconds * 100 / TotalSeconds, 0);
                }
            }
        }

        private TimeSpan ExtractDuration(string rawInfo)
        {
            TimeSpan timeSpan = new TimeSpan(0);
            Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)", RegexOptions.Compiled);
            Match match = re.Match(rawInfo);
            if (match.Success)
            {
                string duration = match.Groups[1].Value;
                string[] timePieces = duration.Split(new char[] { ':', '.' });
                if (timePieces.Length == 4)
                {
                    timeSpan = new TimeSpan(
                        0,
                        Convert.ToInt16(timePieces[0]),
                        Convert.ToInt16(timePieces[1]),
                        Convert.ToInt16(timePieces[2]),
                        Convert.ToInt16(timePieces[3]));
                }
            }
            return timeSpan;
        }
    }
}
