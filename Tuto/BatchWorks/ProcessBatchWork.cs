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
    public abstract class ProcessBatchWork : BatchWork
    {
        public Process Process { get; set; }
        private double TotalSeconds { get; set; }
        private bool durationLoaded;

        public ProcessBatchWork()
        {
            Process = new Process();
        }

        public virtual void RunProcess(string args, string path)
        {
            Process.StartInfo.FileName = path;
            Process.StartInfo.Arguments = args;
            Process.StartInfo.UseShellExecute = Model.Videotheque.Data.WorkSettings.ShowProcesses;
            Process.StartInfo.CreateNoWindow = !Model.Videotheque.Data.WorkSettings.ShowProcesses;
            Process.Start();
            Process.WaitForExit();
            if (Process.ExitCode != 0)
                throw new ArgumentException(
                    string.Format("Process' exit code not equals zero. \n Exe: \"{0}\" \n Args: {1} \n Full: \"{0}\" {1}", path, args));
        }


        public void FinishProcess()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
        }
    }
}
