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

    public abstract class BatchWork : INotifyPropertyChanged
    {
        public virtual Process Process { get; set; }
        public virtual EditorModel Model { get; set; }
        public bool NeedToRewrite { get; set; }
        public void RunProcess(string args, string path)
        {
            Process = new Process();
            Process.StartInfo.FileName = path;
            Process.StartInfo.Arguments = args;
            Process.StartInfo.UseShellExecute = Model.Global.ShowProcesses;
            Process.StartInfo.CreateNoWindow = !Model.Global.ShowProcesses;
            Process.Start();
            Process.WaitForExit();
            try {
                if (Process.ExitCode != 0)
                    throw new ArgumentException("Failed");
            }
            catch { throw new ArgumentException("Failed"); }
        }

        public virtual void Work() {}
        public virtual void Clean() {}

        public List<BatchWork> BeforeWorks = new List<BatchWork>();
        public List<BatchWork> AfterWorks = new List<BatchWork>();

        public virtual bool Finished() { return true; }

        public string Name { get; set; }
        BatchWorkStatus status;
        public BatchWorkStatus Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }


        string exceptionMessage;
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; OnPropertyChanged("ExceptionMessage"); }
        }


        public event EventHandler TaskFinished;
        public void OnTaskFinished()
        {
            if (TaskFinished != null)
                TaskFinished(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
