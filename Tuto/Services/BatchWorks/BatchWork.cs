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

namespace Tuto.BatchWorks
{
    public enum BatchWorkStatus
    {
        Pending,
        Running,
        Success,
        Failure,
        Aborted,
        Cancelled
    }

    public abstract class BatchWork : NotifierModel
    {
        public virtual Process Process { get; set; }
        public virtual EditorModel Model { get; set; }
        public bool NeedToRewrite { get; set; }

        protected void RunProcess(string args, string path)
        {
            Process = new Process();
            Process.StartInfo.FileName = path;
            Process.StartInfo.Arguments = args;
            Process.StartInfo.UseShellExecute = Model.Global.ShowProcesses;
            Process.StartInfo.CreateNoWindow = !Model.Global.ShowProcesses;
            Process.Start();
            Process.WaitForExit();
            if (Process.ExitCode != 0)
                throw new ArgumentException(
                    string.Format("Process' exit code not equals zero. \n Exe:{0} \n Args:{1}", path, args));
        }

        public virtual void Work() { }
        public virtual void Clean() { }
        public virtual bool WorkIsRequired() { return true; }

        public FileInfo GetTempFile(FileInfo info, string suffix)
        {
            var newPath = info.FullName.Split('\\');
            var nameAndExt = info.Name.Split('.');
            nameAndExt[0] = nameAndExt[0] + suffix;
            newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
            return new FileInfo(string.Join("\\", newPath));
        }

        public FileInfo GetTempFile(FileInfo info)
        {
            return GetTempFile(info, "-tmp");
        }

        public List<BatchWork> BeforeWorks = new List<BatchWork>();
        public List<BatchWork> AfterWorks = new List<BatchWork>();

        public virtual bool Finished() { return false; }
        public bool Forced { get; set; }

        public string Name { get; set; }
        BatchWorkStatus status;
        public BatchWorkStatus Status
        {
            get { return status; }
            set { status = value; NotifyPropertyChanged(); }
        }

        public void TryToDelete(FileInfo info)
        {
            TryToDelete(info.FullName);
        }

        public void TryToDelete(string fileName)
        {
            var tries = 0;
            while (tries < 5)
            {
                try
                {
                    tries++;
                    File.Delete(fileName);
                    Thread.Sleep(200);
                }
                catch { }
            }
        }

        public void FinishProcess()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
        }

        string exceptionMessage;
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; NotifyPropertyChanged(); }
        }


        public event EventHandler TaskFinished;
        public void OnTaskFinished()
        {
            if (TaskFinished != null)
                TaskFinished(this, EventArgs.Empty);
        }
    }
}
